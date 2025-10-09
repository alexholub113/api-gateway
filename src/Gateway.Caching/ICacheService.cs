using Gateway.Caching.Models;
using Gateway.Common.Configuration;

namespace Gateway.Caching;

/// <summary>
/// Cache service for storing and retrieving HTTP responses
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Attempts to get cached response and write it directly to HTTP context if found
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="serviceId">Target service ID</param>
    /// <param name="policy">Cache policy</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating if cache was hit and response was written</returns>
    ValueTask<Result<bool>> TryGetAndWriteAsync(
        HttpContext context,
        string serviceId,
        CachePolicy policy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a response in the cache
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="serviceId">Target service ID</param>
    /// <param name="policy">Cache policy</param>
    /// <param name="response">Response to cache</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result of cache operation</returns>
    ValueTask<Result> SetAsync(
        HttpContext context,
        string serviceId,
        CachePolicy policy,
        CachedResponse response,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines if the request is cacheable based on the policy
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="policy">Cache policy</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if request is cacheable</returns>
    ValueTask<Result<bool>> IsCacheableAsync(
        HttpContext context,
        CachePolicy? policy,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears cache entries for a specific service or pattern
    /// </summary>
    /// <param name="pattern">Cache key pattern to clear</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of entries cleared</returns>
    ValueTask<Result<int>> ClearAsync(string pattern, CancellationToken cancellationToken = default);
}