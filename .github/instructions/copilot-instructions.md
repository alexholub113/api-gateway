I inspired by Zuul API Gateway from netflix.
In this project I want to implement lightweight Api Gateway on Asp.net Core Web Api.
The gateway will have bunch of the functions, but the key-feature is distributed AuthZN service that is between gateway and target services, AuthZN service will proxy request to the target service if the request is authorized, otherwise it will return 401 or 403.
AuthZN is out of scope.

# Cases.

1. Service A -> API Gateway -> ServiceB
2. Service A -> API Gateway -> AuthZN Service -> ServiceB

# Functional requirements:

- basic routing to the target services (origins) by path.
- rate limitting
- load balancing
- forwarding headers
- circuit breaker
- forward request to AuthZN service if validation policy is set for the route
- caching responses
- logging requests and responses
- metrics collection
- health checks for target services
- configuration via appsettings.json
- if target service is down, return 503 with message "Service Unavailable"

# Proposed Application Structure:

src/
├── Gateway.Api/ # Main API project
├── Gateway.Core/ # Core abstractions and models
├── Gateway.RateLimiting/ # Rate limiting module
├── Gateway.LoadBalancing/ # Load balancing strategies
├── Gateway.CircuitBreaker/ # Circuit breaker pattern
├── Gateway.Authentication/ # AuthZN integration
├── Gateway.Caching/ # Response caching
├── Gateway.Logging/ # Request/response logging
├── Gateway.Metrics/ # Metrics collection
├── Gateway.HealthChecks/ # Health monitoring
└── Gateway.Proxy/ # HTTP proxy functionality

Gateway.Api - is web layer, responsible for web logic, using minimal api endpoints and use Gateway.Core.
Gateway.Core - entry point to the domain business logic, responsible for core logic of gateway, it references other Gateway modules. Gateway.Core provides public interface 'IGatewayHandler' that is called by Gateway.Api to handle gateway requests.

# Module structure:

## Example:

Gateway.XXX/
├── Configuration/
├── Services/ (Internal implementation, services, middleware, etc., if any)
├── Extensions/ (Public registration methods)
├── Models/ (Internal models, if any)
└── Gateway.XXX.csproj
└── IPublicModuleServiceInterface.cs

# Integration with Gateway.Api:

var builder = WebApplication.CreateBuilder(args);

// Add modules
builder.Services
.AddGatewayCore()
.AddRateLimiting()
.AddLoadBalancing()
.AddAuthentication()
.AddCaching()
.AddMetrics()
.AddProxy();

var app = builder.Build();

// We could use middlewares for some scenarios
Just an example:
app.Use{ModuleMiddleware}()

# Error handling strategy:

- Global exception handling middleware to catch unhandled exceptions.
- Use Result<T> pattern for expected errors.
- Standard HTTP status codes

# Metrics

## Request/Response Metrics

gateway_requests_total (counter) - Total requests by service, method, status code
gateway_request_duration (histogram) - Request latency distribution
gateway_errors_total (counter) - Error count by type (4xx, 5xx, timeout, etc.)

## Pipeline Stage Metrics

gateway_route_resolution_duration (histogram) - Time to resolve routes
gateway_load_balancer_duration (histogram) - Time to select instance
gateway_proxy_duration (histogram) - Time spent proxying to backend
gateway_pipeline_stage_duration (histogram) - Duration by pipeline stage

## Rate Limiting Metrics

gateway_rate_limit_requests_total (counter) - Requests by allowed/denied
gateway_rate_limit_policy_usage (counter) - Usage per rate limit policy

## Caching Metrics

gateway_cache_requests_total (counter) - Cache hits/misses by service/policy
gateway_cache_hit_ratio (gauge) - Cache hit percentage
gateway_cache_size_bytes (gauge) - Total cache size in bytes

## Load Balancing Metrics

gateway_load_balancer_requests_total (counter) - Requests by strategy/instance
gateway_instance_selection_duration (histogram) - Time to select instance
gateway_instances_available (gauge) - Available instances per service
gateway_instances_total (gauge) - Total configured instances per service

### Instance Health

gateway_health_checks_total (counter) - Health checks by result/service
gateway_health_check_duration (histogram) - Health check duration
gateway_instances_healthy (gauge) - Healthy instances per service
gateway_instance_failures_total (counter) - Instance failure count

## Proxy Metrics

Backend Communication
gateway_backend_requests_total (counter) - Backend requests by service/instance
gateway_backend_duration (histogram) - Backend response time
gateway_backend_errors_total (counter) - Backend errors by service/type

# Frontend demo goals:

## Screenshot-Worthy Pages:

## Components/Panels

Use vit+react+typescript+tailwindcss.

1. Demo request simulator - provides buttons to send request to the target service.
2. Live request flow - shows wich request hit cache or rate limit or proxy, also shows the duration of the request.
3. Service Health Monitor - list available services and health status of the instances.
4. API Gateway Control Center - shows available metrics, could be updated each 5 seconds or after sending request.
5. Gateway Architecture - visualization of the arcitecture of the gateway for demo purpose.

# Instructions for AI:

- if user uses 'Ask' mode instead of 'Agent', then avoid providing detail implementation, focus on describing of options, approaches, solutions instead.
- create seperate file for each class or interface (except request and response models in endpoints).
- let user create projects (.csproj) by himself manually.
- use primary constructors.
- when user requests to do something, do not include any extra functionality that was not requested, but let user know that this functionality can be added.
- if user ask question be short and to the point, assume user can be wrong, let's brainshtorm problem together.
- if new member (method,property,etc) is not used then it should be removed.
- if configuration/module is used by multiple modules/projects then create it in Core or common project, otherwise create it in the specific module/project.
- services can be without interfaces, add interface only if it makes sense.
- prefer Singletons over Scoped, Transients.
