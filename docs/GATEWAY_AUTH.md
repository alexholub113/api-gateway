# Gateway.Auth - JWT Authentication Module

## Overview

The `Gateway.Auth` module provides JWT Bearer token authentication for the API Gateway. It validates JWT tokens against configured policies before forwarding requests to backend services.

## Features

- **JWT Token Validation** - Validates Bearer tokens using Microsoft.IdentityModel packages
- **Flexible Configuration** - Per-service authentication policies with issuer/audience validation
- **OpenTelemetry Metrics** - Records authentication attempts, duration, and errors
- **Claims Extraction** - Extracts authenticated user information for downstream services

## Configuration

Authentication policies are configured directly on service settings:

```json
{
  "Gateway": {
    "TargetServices": [
      {
        "ServiceId": "secure-api",
        "Instances": [
          { "Url": "http://localhost:5001" }
        ],
        "AuthPolicy": {
          "ValidIssuers": ["https://your-identity-provider.com"],
          "ValidAudiences": ["your-api-audience"]
        }
      }
    ]
  }
}
```

### AuthPolicy Properties

- **ValidIssuers** - List of accepted token issuers (optional - if empty, issuer validation is disabled)
- **ValidAudiences** - List of accepted audiences (optional - if empty, audience validation is disabled)

**Note:** All other JWT validation (signature, lifetime, etc.) is handled automatically by Microsoft.IdentityModel. The gateway performs basic issuer/audience validation only.

## Usage

### Register in Program.cs

```csharp
// Add authentication services
builder.Services.AddGatewayAuth();

// Add to pipeline (after rate limiting, before proxying)
app.UseGatewayAuth();
```

### Request Flow

1. Client sends request with `Authorization: Bearer <token>` header
2. Middleware checks if service has `AuthPolicy` configured
3. If configured, validates token against policy
4. On success, adds `AuthenticatedUser` to `HttpContext.Items`
5. On failure, returns 401 Unauthorized

### Example Request

```bash
curl -H "Authorization: Bearer eyJhbGc..." \
     -H "X-Gateway-TargetServiceId: secure-api" \
     http://localhost:5000/api/data
```

## Metrics

The module records the following metrics:

- `gateway_auth_requests_total` - Total authentication attempts (by service, result, policy)
- `gateway_auth_duration` - Token validation duration in milliseconds
- `gateway_auth_errors_total` - Authentication errors (by service, error_type, policy)

### Metric Tags

- **service** - Target service ID
- **result** - `success`, `invalid_token`, `missing_token`, or `error`
- **error_type** - Type of authentication error
- **policy** - Policy name used for validation

## Error Responses

### 401 Unauthorized - Missing Token

```json
{
  "error": "Authorization header is required"
}
```

### 401 Unauthorized - Invalid Token

```json
{
  "error": "Token has expired"
}
```

### 500 Internal Server Error

```json
{
  "error": "Authentication error"
}
```

## Integration with Other Modules

The authentication middleware should be placed in the pipeline **after** rate limiting but **before** proxying:

```
Request → CoreMetrics → RateLimiting → Auth → Caching → LoadBalancing → Proxy
```

This ensures:
- All requests are counted (even unauthenticated)
- Rate limits apply before expensive token validation
- Authenticated requests can be cached if applicable
