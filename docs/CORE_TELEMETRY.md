# Gateway Core Telemetry Implementation

## Problem

Request metrics (`gateway_requests_total`) were always showing 0 because:

1. **RateLimiting was optional** - The `RateLimitingTelemetry.RecordRequest()` method was only called when rate limiting was enabled for a service
2. **No central metric recording** - There was no guaranteed place where ALL gateway requests were recorded

## Solution

Added **Core Telemetry** to `Gateway.Core` module that records metrics for ALL requests passing through the gateway, regardless of module configurations.

## Implementation

### 1. Created `CoreTelemetry` class

**Location:** `src/Gateway.Core/Telemetry/CoreTelemetry.cs`

**Metrics Recorded:**
- `gateway_requests_total` (Counter) - Total requests by service_id, method, status_code
- `gateway_errors_total` (Counter) - Error count by service_id, error_type, status_code  
- `gateway_request_duration` (Histogram) - Request duration in milliseconds
- `gateway_route_resolution_duration` (Histogram) - Route resolution time

**Meter Name:** `Gateway.Core`

### 2. Created `CoreTelemetryMiddleware`

**Location:** `src/Gateway.Core/Middleware/CoreTelemetryMiddleware.cs`

**Purpose:** 
- Wraps every gateway request with telemetry recording
- Measures total request duration
- Records metrics in the `finally` block to ensure it always executes
- Only records if a `serviceId` is present (indicates it's a gateway request)

**Key Features:**
- Uses `Stopwatch` for precise duration measurement
- Automatically records errors for 4xx/5xx status codes
- Logs debug information for each request

### 3. Registered Services and Middleware

**Services Registration** (`Gateway.Core/Extensions/ServiceCollectionExtensions.cs`):
```csharp
services.AddSingleton<CoreTelemetry>();
```

**Middleware Pipeline** (`Gateway.Core/Extensions/ApplicationBuilderExtensions.cs`):
```csharp
app.UseMiddleware<CoreTelemetryMiddleware>()  // First - records ALL requests
   .UseGatewayTelemetry()                     // OpenTelemetry setup
   .UseRateLimiting()                         // Rate limiting (optional per service)
   .UseCaching();                             // Caching (optional)
```

### 4. Updated OpenTelemetry Configuration

**Location:** `Gateway.Metrics/Extensions/ServiceCollectionExtensions.cs`

Added `Gateway.Core` meter to OpenTelemetry:
```csharp
.AddMeter("Gateway.Core")            // Core gateway metrics (ALL requests)
.AddMeter("Gateway.RateLimiting")    // Rate limiting metrics
.AddMeter("Gateway.Caching")         // Caching metrics
.AddMeter("Gateway.LoadBalancing")   // Load balancing metrics  
.AddMeter("Gateway.Proxy")           // Proxy metrics
```

### 5. Updated MetricsAggregatorService

**Location:** `Gateway.Metrics/Services/MetricsAggregatorService.cs`

**Changes:**
- Now properly accumulates counter values using `MetricAccumulator`
- Tracks histogram values for averaging
- Supports tag-based filtering (e.g., status="denied")
- Forces telemetry service instantiation to ensure meters are created before listener starts
- Added comprehensive logging for debugging

**Key Fix:** The `MeterListener` needs to be started BEFORE the Meter instruments are created, so we force instantiation of telemetry services after starting the listener.

## Architecture Principles

✅ **Each module records its own metrics**
- Gateway.Core → Core request metrics
- Gateway.RateLimiting → Rate limit decisions  
- Gateway.Caching → Cache hits/misses
- Gateway.LoadBalancing → Instance selection
- Gateway.Proxy → Backend communication

✅ **Gateway.Metrics only configures and exports**
- Sets up OpenTelemetry
- Configures Prometheus exporter
- Provides aggregation service
- Does NOT record any metrics itself

## Metrics Flow

```
1. Request arrives at Gateway API
   ↓
2. CoreTelemetryMiddleware starts (records START time)
   ↓
3. Request passes through other middlewares:
   - UseGatewayTelemetry() - Prometheus endpoint
   - UseRateLimiting() - Records rate limit metrics (optional)
   - UseCaching() - Records cache metrics (optional)
   ↓
4. GatewayHandler routes request
   ↓
5. ProxyHandler forwards to backend (records proxy metrics)
   ↓
6. CoreTelemetryMiddleware completes (records END time + metrics)
   ↓
7. Metrics available via:
   - /metrics (Prometheus format)
   - /api/metrics (JSON for frontend)
```

## Testing

To verify metrics are being recorded:

1. **Start Gateway API**
   ```powershell
   cd src/Gateway.Api
   dotnet run
   ```

2. **Send requests**
   ```powershell
   curl -H "X-Gateway-TargetServiceId: my-service" https://localhost:7158/api/users
   ```

3. **Check metrics**
   - Prometheus: `https://localhost:7158/metrics`
   - JSON API: `https://localhost:7158/api/metrics`
   - Frontend: `http://localhost:5173`

4. **Look for**:
   ```
   gateway_requests_total{service_id="my-service",method="GET",status_code="200"} 1
   gateway_request_duration_bucket{service_id="my-service",method="GET",status_code="200",le="100"} 1
   ```

## Benefits

✅ **Guaranteed metric recording** - Every gateway request is counted  
✅ **Module independence** - Works even if rate limiting/caching is disabled
✅ **Accurate duration** - Measures total gateway processing time
✅ **Error tracking** - Automatically categorizes 4xx/5xx errors
✅ **Clean architecture** - Each module owns its metrics
