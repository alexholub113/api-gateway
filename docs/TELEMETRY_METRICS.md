# Gateway Telemetry Metrics

This document lists all telemetry metrics implemented across the Gateway modules.

## 🔢 Rate Limiting Metrics (`Gateway.RateLimiting`)

### Counters
- **`gateway_requests_total`** - Total requests processed by gateway  
  - Tags: `service_id`, `method`, `status_code`
- **`gateway_rate_limit_requests_total`** - Rate limit checks (allowed/denied)
  - Tags: `service_id`, `policy_name`, `status`, `client_id`  
- **`gateway_rate_limit_policy_usage`** - Usage count per rate limiting policy
  - Tags: `service_id`, `policy_name`
- **`gateway_errors_total`** - Total errors by type
  - Tags: `service_id`, `error_type`, `status_code`

### Histograms
- **`gateway_request_duration`** - Request processing duration (ms)
  - Tags: `service_id`, `method`, `status_code`

## 💾 Caching Metrics (`Gateway.Caching`)

### Counters
- **`gateway_cache_requests_total`** - Cache requests by hit/miss status
  - Tags: `service_id`, `policy`, `status`
- **`gateway_cache_hits_total`** - Total cache hits
  - Tags: `service_id`, `policy`, `status`
- **`gateway_cache_misses_total`** - Total cache misses
  - Tags: `service_id`, `policy`, `status`

### Gauges
- **`gateway_cache_size_bytes`** - Total cache size in bytes
  - Tags: `service_id`, `policy`

### Histograms
- **`gateway_cache_hit_ratio`** - Cache hit percentage
  - Tags: `service_id`, `policy`

## ⚖️ Load Balancing Metrics (`Gateway.LoadBalancing`)

### Counters
- **`gateway_load_balancer_requests_total`** - Requests by strategy and instance
  - Tags: `service_id`, `strategy`, `instance_id`
- **`gateway_instance_failures_total`** - Instance failure count
  - Tags: `service_id`, `instance_id`, `failure_type`

### Gauges
- **`gateway_instances_available`** - Available instances per service
  - Tags: `service_id`
- **`gateway_instances_total`** - Total configured instances per service
  - Tags: `service_id`
- **`gateway_instances_healthy`** - Healthy instances per service
  - Tags: `service_id`

### Histograms
- **`gateway_load_balancer_duration`** - Time to select instance (ms)
  - Tags: `service_id`, `strategy`, `instance_id`
- **`gateway_instance_selection_duration`** - Time to select instance (ms)
  - Tags: `service_id`, `strategy`, `instance_id`

## 🔄 Proxy Metrics (`Gateway.Proxy`)

### Counters
- **`gateway_backend_requests_total`** - Backend requests by service and instance
  - Tags: `service_id`, `instance_id`, `method`, `status_code`
- **`gateway_backend_errors_total`** - Backend errors by service and type
  - Tags: `service_id`, `instance_id`, `error_type`
- **`gateway_health_checks_total`** - Health checks by result and service
  - Tags: `service_id`, `instance_id`, `result`

### Histograms
- **`gateway_proxy_duration`** - Time spent proxying to backend (ms)
  - Tags: `service_id`, `instance_id`
- **`gateway_backend_duration`** - Backend response time (ms)
  - Tags: `service_id`, `instance_id`, `method`, `status_code`
- **`gateway_health_check_duration`** - Health check duration (ms)
  - Tags: `service_id`, `instance_id`, `result`

## 🔄 Pipeline Metrics (`Gateway.Metrics`)

### Histograms
- **`gateway_route_resolution_duration`** - Time to resolve routes (ms)
  - Tags: `route`
- **`gateway_pipeline_stage_duration`** - Duration by pipeline stage (ms)
  - Tags: `stage`, `service_id`

## 📊 Built-in ASP.NET Core Metrics

The Gateway also includes standard ASP.NET Core and HTTP client instrumentation:
- HTTP request metrics
- HTTP client metrics
- System metrics

## 🎯 Accessing Metrics

### Prometheus Endpoint
- **URL**: `http://localhost:5000/metrics`
- **Format**: Prometheus text format
- **Usage**: Scraping by Prometheus, Grafana, or custom monitoring tools

### Console Output (Development)
- Metrics are also exported to console for development/debugging
- Enabled alongside Prometheus exporter

## 🔧 Configuration

Telemetry is configured in `Gateway.Metrics` and integrated via:
- `services.AddGateway()` - Registers all telemetry services
- `app.UseGateway()` - Configures telemetry endpoints

Each module registers its own telemetry service and the metrics are automatically collected by OpenTelemetry.