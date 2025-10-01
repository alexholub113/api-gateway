using Microsoft.Extensions.Logging;

namespace Gateway.Proxy.Services;

/// <summary>
/// HTTP proxy service that forwards requests to target services
/// </summary>
internal class ProxyHandler(IHttpClientFactory httpClientFactory, IOptionsMonitor<ProxyOptions> options, ILogger<ProxyHandler> logger) : IProxyHandler
{
    public async Task<Result> ProxyRequestAsync(HttpContext context, Uri uri, string downstreamPath)
    {
        var targetUrl = BuildTargetUrl(uri, downstreamPath);
        var method = context.Request.Method;
        var startTime = DateTime.UtcNow;

        logger.LogInformation("Proxying {Method} request to '{TargetUrl}'", method, targetUrl);

        try
        {
            var httpClient = httpClientFactory.CreateClient(ProxyConstants.HttpClientName);
            var proxyRequest = await CreateProxyRequestAsync(context.Request, targetUrl);

            using var response = await httpClient.SendAsync(proxyRequest, HttpCompletionOption.ResponseHeadersRead);
            var duration = DateTime.UtcNow - startTime;


            await CopyResponseAsync(response, context.Response);

            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            var duration = DateTime.UtcNow - startTime;
            logger.LogError(ex, "HTTP request failed for '{TargetUrl}' after {Duration}ms: {Error}", targetUrl, duration.TotalMilliseconds, ex.Message);
            return Result.Failure($"HTTP request failed: {ex.Message}");
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            var duration = DateTime.UtcNow - startTime;
            logger.LogWarning("Request to '{TargetUrl}' timed out after {Duration}ms", targetUrl, duration.TotalMilliseconds);
            return Result.Failure("Request timeout");
        }
        catch (Exception ex)
        {
            var duration = DateTime.UtcNow - startTime;
            logger.LogError(ex, "Proxy request to '{TargetUrl}' failed after {Duration}ms: {Error}", targetUrl, duration.TotalMilliseconds, ex.Message);
            return Result.Failure($"Proxy request failed: {ex.Message}");
        }
    }

    private static string BuildTargetUrl(Uri uri, string downstreamPath)
    {
        var baseUrl = uri.ToString().TrimEnd('/');
        if (string.IsNullOrEmpty(downstreamPath))
        {
            return baseUrl;
        }

        downstreamPath = downstreamPath.TrimStart('/') ?? string.Empty;
        return $"{baseUrl}/{downstreamPath}";
    }

    private async Task<HttpRequestMessage> CreateProxyRequestAsync(HttpRequest request, string targetUrl)
    {
        var proxyRequest = new HttpRequestMessage(new HttpMethod(request.Method), targetUrl);
        var contentHeaders = new List<KeyValuePair<string, string[]>>();

        // Copy headers (separate content headers for later)
        foreach (var header in request.Headers)
        {
            if (!options.CurrentValue.ExcludedHeaders.Contains(header.Key, StringComparer.OrdinalIgnoreCase))
            {
                // Try to add as request header first
                if (!proxyRequest.Headers.TryAddWithoutValidation(header.Key, [.. header.Value]))
                {
                    // If it fails, it might be a content header - store it for later
                    contentHeaders.Add(new KeyValuePair<string, string[]>(header.Key, header.Value.ToArray()!));
                }
            }
        }

        // Copy body for non-GET requests
        if (request.Method != HttpMethods.Get && request.Method != HttpMethods.Head &&
            request.ContentLength > 0)
        {
            // Create content with the request body
            var bodyBytes = new byte[request.ContentLength ?? 0];
            if (request.ContentLength > 0)
            {
                await request.Body.ReadExactlyAsync(bodyBytes, 0, (int)request.ContentLength.Value);
            }

            var content = new ByteArrayContent(bodyBytes);

            // Add content headers
            foreach (var contentHeader in contentHeaders)
            {
                content.Headers.TryAddWithoutValidation(contentHeader.Key, contentHeader.Value);
            }

            // Ensure essential content headers are set
            if (request.ContentType != null)
            {
                content.Headers.TryAddWithoutValidation("Content-Type", request.ContentType);
            }

            proxyRequest.Content = content;
        }

        return proxyRequest;
    }

    private static async Task CopyResponseAsync(HttpResponseMessage response, HttpResponse httpResponse)
    {
        httpResponse.StatusCode = (int)response.StatusCode;

        // Copy response headers
        foreach (var header in response.Headers)
        {
            httpResponse.Headers.TryAdd(header.Key, header.Value.ToArray());
        }

        // Copy content headers
        if (response.Content != null)
        {
            foreach (var header in response.Content.Headers)
            {
                httpResponse.Headers.TryAdd(header.Key, header.Value.ToArray());
            }

            // Copy response body
            await response.Content.CopyToAsync(httpResponse.Body);
        }
    }
}