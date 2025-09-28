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
├── Gateway.ServiceRouting/ # Path-based routing module
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
.AddServiceRouting()
.AddRateLimiting()
.AddLoadBalancing()
.AddCircuitBreaker()
.AddAuthentication()
.AddCaching()
.AddLogging()
.AddMetrics()
.AddHealthChecks()
.AddProxy();

var app = builder.Build();

// We could use middlewares for some scenarios
Just an example:
app.Use{ModuleMiddleware}()

# Shared context for communication between modules:

public interface IGatewayContext
{
...
}

public static class HttpContextExtensions
{
private const string GatewayContextKey = "Gateway.Context";

    public static IGatewayContext GetGatewayContext(this HttpContext context)
    {
        if (!context.Items.TryGetValue(GatewayContextKey, out var ctx))
        {
            ctx = new GatewayContext();
            context.Items[GatewayContextKey] = ctx;
        }
        return (IGatewayContext)ctx;
    }

}

# Error handling strategy:

- Global exception handling middleware to catch unhandled exceptions.
- Use Result<T> pattern for expected errors.
- Standard HTTP status codes

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
