using Gateway.Common.Models.Result;
using Gateway.Proxy.Configuration;
using Gateway.ServiceRouting.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Gateway.Proxy.Services;

/// <summary>
/// HTTP proxy service that forwards requests to target services
/// </summary>
internal class ProxyHandler(IHttpClientFactory httpClientFactory, IOptionsMonitor<ProxyOptions> options) : IProxyHandler
{
    public async Task<Result> ProxyRequestAsync(HttpContext context, RouteMatch routeMatch, Uri uri)
    {
        try
        {
            var httpClient = httpClientFactory.CreateClient(ProxyConstants.HttpClientName);
            var targetUrl = BuildTargetUrl(uri, routeMatch);
            var proxyRequest = await CreateProxyRequestAsync(context.Request, targetUrl);

            using var response = await httpClient.SendAsync(proxyRequest, HttpCompletionOption.ResponseHeadersRead);

            await CopyResponseAsync(response, context.Response);

            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            return Result.Failure($"HTTP request failed: {ex.Message}");
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            return Result.Failure("Request timeout");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Proxy request failed: {ex.Message}");
        }
    }

    private static string BuildTargetUrl(Uri uri, RouteMatch routeMatch)
    {
        var baseUrl = uri.ToString().TrimEnd('/');
        var downstreamPath = routeMatch.DownstreamPath?.TrimStart('/') ?? string.Empty;

        if (string.IsNullOrEmpty(downstreamPath))
        {
            return baseUrl;
        }

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