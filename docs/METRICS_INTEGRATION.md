# Gateway Metrics Integration

## Overview

The Gateway now uses **real OpenTelemetry metrics** instead of duplicate metric recording. The system leverages the existing comprehensive telemetry infrastructure already in place across all Gateway modules.

## Architecture

### Components

1. **MetricsAggregatorService** (`Gateway.Metrics/Services/MetricsAggregatorService.cs`)
   - Uses `MeterListener` to subscribe to all OpenTelemetry meters
   - Collects real-time metric values from all Gateway modules
   - Aggregates and calculates derived metrics (rates, averages, scores)
   - No duplicate recording - reads from existing telemetry

2. **IGatewayMetricsProvider** (`Gateway.Metrics/IGatewayMetricsProvider.cs`)
   - Simplified interface with single method: `GetCurrentMetrics()`
   - Removed recording methods (RecordRequest, RecordCache, etc.)
   - Returns comprehensive `GatewayMetricsSnapshot`

3. **MetricsEndpoint** (`Gateway.Api/Endpoints/MetricsEndpoint.cs`)
   - Exposes `/api/metrics` endpoint
   - Returns JSON representation of current metrics
   - Consumed by React frontend for real-time dashboard

## Metrics Sources

The system reads from existing OpenTelemetry meters:

### RateLimiting Metrics (`Gateway.RateLimiting`)
- `gateway_rate_limit_requests_total` - Counter for all rate limit checks
- `gateway_rate_limit_duration` - Histogram for rate limit check duration
- `gateway_rate_limit_errors_total` - Counter for rate limit errors

### Caching Metrics (`Gateway.Caching`)
- `gateway_cache_hits_total` - Counter for cache hits
- `gateway_cache_misses_total` - Counter for cache misses
- `gateway_cache_size_bytes` - UpDownCounter for cache size

### LoadBalancing Metrics (`Gateway.LoadBalancing`)
- `gateway_load_balancer_requests_total` - Counter for load balancing requests
- `gateway_instance_failures_total` - Counter for instance failures
- `gateway_instances_available` - UpDownCounter for available instances
- `gateway_load_balancer_duration` - Histogram for load balancing duration

### Proxy Metrics (`Gateway.Proxy`)
- `gateway_backend_requests_total` - Counter for backend requests
- `gateway_backend_errors_total` - Counter for backend errors
- `gateway_health_check_requests_total` - Counter for health checks
- `gateway_backend_request_duration` - Histogram for request duration

### Pipeline Metrics (`Gateway.Metrics`)
- `gateway_route_resolution_duration` - Histogram for route resolution time
- `gateway_stage_duration` - Histogram for pipeline stage execution time

## Calculated Metrics

The `MetricsAggregatorService` derives additional metrics:

1. **Requests Per Minute** - Calculated from total requests / uptime
2. **Average Response Time** - Derived from duration histograms
3. **Cache Hit Rate** - `(hits / (hits + misses)) * 100`
4. **Error Rate** - `(errors / total_requests) * 100`
5. **Performance Score** - Weighted combination of response time, error rate, and cache hit rate
6. **Load Balancing Efficiency** - `((requests - failures) / requests) * 100`
7. **Trend Indicators** - Percentage change compared to previous values

## Usage

### Backend (C#)

```csharp
// In Program.cs
builder.Services.AddGateway(builder.Configuration);
builder.Services.AddGatewayMetrics(); // Registers MetricsAggregatorService

// In endpoints
app.MapGet("/api/metrics", (IGatewayMetricsProvider provider) =>
{
    var snapshot = provider.GetCurrentMetrics();
    return Results.Ok(snapshot);
});
```

### Frontend (React)

```typescript
// Fetch metrics from API
const response = await axios.get<MetricsResponse>(
  'https://localhost:7158/api/metrics'
);

const metrics = response.data;
console.log('Requests/min:', metrics.requestsPerMinute);
console.log('Cache hit rate:', metrics.cacheHitRatePercentage);
console.log('Performance score:', metrics.overallPerformanceScore);
```

## Removed Files

The following duplicate recording services were removed:

- ❌ `Gateway.Metrics/Services/GatewayMetricsProvider.cs` - Had manual recording methods
- ❌ `Gateway.Metrics/Services/MetricsSimulationService.cs` - Simulated fake activity
- ❌ `Gateway.Metrics/Middleware/MetricsTrackingMiddleware.cs` - Duplicate middleware tracking
- ❌ `Gateway.Api/Services/*` - No duplicate recording services in API layer

## Benefits

1. **No Duplication** - Uses existing OpenTelemetry infrastructure
2. **Real Data** - Actual metrics from production telemetry, not simulated
3. **Consistent** - Same metrics exposed via Prometheus `/metrics` and JSON `/api/metrics`
4. **Lightweight** - Minimal overhead using `MeterListener`
5. **Maintainable** - Single source of truth for metrics

## Testing

To verify metrics:

1. **Start Gateway API**
   ```powershell
   cd src/Gateway.Api
   dotnet run
   ```

2. **Send some requests** through the gateway to generate metrics

3. **Check JSON metrics**
   ```powershell
   curl https://localhost:7158/api/metrics
   ```

4. **Check Prometheus metrics**
   ```powershell
   curl https://localhost:7158/metrics
   ```

5. **View in Frontend Dashboard**
   - Open demo web page: http://localhost:5173
   - Navigate to "Gateway Metrics" section
   - Metrics refresh automatically

## Future Enhancements

Potential improvements:

1. **Historical Data** - Store metrics time series in database
2. **Alerting** - Configure thresholds and alerts
3. **Custom Metrics** - Add business-specific metrics
4. **Dashboards** - Grafana dashboards for Prometheus metrics
5. **Distributed Tracing** - Integrate with Jaeger/Zipkin for request traces
