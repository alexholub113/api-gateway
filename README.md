# Lightweight API Gateway

A modern, production-ready API Gateway built with ASP.NET Core 9.0, inspired by Netflix's Zuul. Features modular architecture, real-time metrics, and comprehensive telemetry powered by OpenTelemetry.

## 🎯 Key Features

- **Smart Routing** - Route requests to backend services with flexible path-based configuration
- **Rate Limiting** - Token bucket algorithm with per-service policies
- **Load Balancing** - Round-robin, least-connections, and weighted strategies
- **Response Caching** - Memory-based caching with TTL policies
- **Circuit Breaker** - Fault tolerance with automatic failure detection
- **AuthZN Integration** - Distributed authorization service for policy-based access control
- **Real-Time Metrics** - OpenTelemetry + Prometheus for observability
- **Health Checks** - Monitor backend service availability

## 🏗️ Architecture

The gateway follows a modular design where each feature is a separate project:

```
Gateway.Api         → Web layer (Minimal APIs)
Gateway.Core        → Core orchestration logic
Gateway.RateLimiting → Token bucket rate limiting
Gateway.LoadBalancing → Multiple balancing strategies
Gateway.Caching     → Response caching layer
Gateway.Proxy       → HTTP forwarding engine
Gateway.Metrics     → OpenTelemetry integration
Gateway.Common      → Shared utilities
```

Each module records its own telemetry and can be configured independently via `appsettings.json`.

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK
- Node.js 18+ (for demo frontend)

### Run the Gateway

```bash
cd src/Gateway.Api
dotnet run
```

Gateway starts on `http://localhost:5000`

### Run the Demo Frontend

```bash
cd demo/GatewayDemoWebPage
npm install
npm run dev
```

Frontend starts on `http://localhost:5173`

### Run a Target Service

```bash
cd demo/TargetService.Api
dotnet run
```

## 📸 Live Demo Dashboard

The project includes a React-based demo dashboard showcasing real-time gateway capabilities:

### Request Simulator
Test different endpoints and see how the gateway handles them:

![Demo Request Simulator](Images/demo-request-simulator-screenshot.png)

### Gateway Metrics
Real-time metrics powered by OpenTelemetry - track requests, latency, cache efficiency, and error rates:

![Gateway Metrics](Images/gateway-metrics-screenshot.png)

### Service Health Monitor
Monitor all backend services and their health status with auto-refresh:

![Service Health Monitor](Images/service-health-monitor-screenshot.png)

### Architecture Visualization
Visual representation of request flow through the gateway pipeline:

![Gateway Architecture](Images/api-gateway-architecture-screenshot.png)

## ⚙️ Configuration

Configure routes and policies in `appsettings.json`:

```json
{
  "Gateway": {
    "Routes": [
      {
        "Path": "/api/users",
        "Origin": "http://localhost:5001",
        "RateLimitPolicy": "default",
        "CachePolicy": "standard"
      }
    ]
  },
  "RateLimiting": {
    "Policies": {
      "default": {
        "RequestsPerMinute": 100,
        "BurstSize": 20
      }
    }
  }
}
```

## 📊 Observability

- **Prometheus Metrics** - Available at `/metrics` endpoint
- **Health Checks** - Available at `/health-status` endpoint
- **Structured Logging** - All modules use `ILogger<T>` for consistent logging

### Key Metrics

- `gateway_requests_total` - Total request count by service/status
- `gateway_request_duration` - Request latency histogram
- `gateway_cache_hits_total` / `gateway_cache_misses_total` - Cache performance
- `gateway_backend_errors_total` - Backend failure tracking
- `gateway_rate_limit_requests_total` - Rate limiting statistics

## 🛠️ Technical Stack

**Backend:**
- ASP.NET Core 9.0 with Minimal APIs
- OpenTelemetry for metrics & tracing
- Prometheus exporter
- Memory-based caching

**Frontend:**
- Vite + React 18 + TypeScript
- TailwindCSS v4 for styling
- Axios for API calls
- Lucide React for icons

## 💡 Design Decisions

- **Modular Architecture** - Each feature is isolated for maintainability and testability
- **OpenTelemetry First** - Built-in observability without vendor lock-in
- **Configuration Over Code** - Routes and policies configured via JSON
- **Primary Constructors** - Modern C# 12 features throughout
- **Result Pattern** - Explicit error handling without exceptions for flow control
- **Singleton Services** - Most services are stateless singletons for performance

## 🔄 Request Flow

```
Client → Gateway.Api → CoreTelemetry → RateLimiting → Caching → LoadBalancer → Proxy → Backend
```

Each middleware can short-circuit the pipeline (e.g., return cached response, reject rate-limited request).

## 📝 License

MIT

---

Built with ❤️ to demonstrate modern .NET architecture patterns and microservices best practices.
