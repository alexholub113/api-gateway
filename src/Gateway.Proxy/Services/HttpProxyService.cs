using Gateway.Core.Abstractions;
using Gateway.Proxy.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Gateway.Proxy.Services;

/// <summary>
/// HTTP proxy service that forwards requests to target services
/// </summary>
internal class HttpProxyService(IHttpClientFactory httpClientFactory, IOptionsMonitor<ProxyOptions> options)
{
    public async Task<Result> ProxyRequestAsync(HttpContext context, IGatewayContext gatewayContext)
    {
        if (gatewayContext.SelectedInstance == null)
        {
            return Result.Failure("No service instance selected for proxying");
        }

        try
        {
            var httpClient = httpClientFactory.CreateClient(ProxyConstants.HttpClientName);
            var targetUrl = BuildTargetUrl(context.Request, gatewayContext.SelectedInstance, gatewayContext.RouteMatch);
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

    private static string BuildTargetUrl(HttpRequest request, ServiceInstance serviceInstance, RouteMatch? routeMatch)
    {
        var baseUrl = serviceInstance.Url.TrimEnd('/');
        var path = request.Path.ToString();
        var queryString = request.QueryString.ToString();

        // If we have a dynamic route pattern, strip the routing prefix
        // e.g., "/route/target-service/users/list" becomes "/users/list"
        if (routeMatch != null && routeMatch.Pattern.Contains("*"))
        {
            // Extract the route prefix from the pattern (e.g., "/route/target-service/*")
            var patternWithoutWildcard = routeMatch.Pattern.TrimEnd('*').TrimEnd('/');

            if (path.StartsWith(patternWithoutWildcard, StringComparison.OrdinalIgnoreCase))
            {
                // Remove the routing prefix, leaving the actual API path
                path = path[patternWithoutWildcard.Length..];

                // Ensure path starts with '/' for the target service
                if (!path.StartsWith('/'))
                    path = "/" + path;
            }
        }

        return $"{baseUrl}{path}{queryString}";
    }

    private Task<HttpRequestMessage> CreateProxyRequestAsync(HttpRequest request, string targetUrl)
    {
        var proxyRequest = new HttpRequestMessage(new HttpMethod(request.Method), targetUrl);

        // Copy headers
        foreach (var header in request.Headers)
        {
            if (!options.CurrentValue.ExcludedHeaders.Contains(header.Key, StringComparer.OrdinalIgnoreCase))
            {
                if (!proxyRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                {
                    // If it's a content header, we'll add it after setting the content
                }
            }
        }

        // Copy body for non-GET requests
        if (request.Method != HttpMethods.Get && request.Method != HttpMethods.Head &&
            request.ContentLength > 0)
        {
            var content = new StreamContent(request.Body);

            // Copy content headers
            if (request.ContentType != null)
            {
                content.Headers.TryAddWithoutValidation("Content-Type", request.ContentType);
            }

            if (request.ContentLength.HasValue)
            {
                content.Headers.TryAddWithoutValidation("Content-Length", request.ContentLength.ToString());
            }

            proxyRequest.Content = content;
        }

        return Task.FromResult(proxyRequest);
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