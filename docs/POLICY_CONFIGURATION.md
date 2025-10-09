# Policy Configuration Refactoring

## Overview

The configuration system has been refactored to provide more flexibility. Instead of defining named policies in separate sections and referencing them by name, policies are now defined **inline** within each service configuration.

## Benefits

- **Service Autonomy**: Each service can define its own policies without relying on shared named policies
- **Flexibility**: Different services can have completely custom policy configurations
- **Type Safety**: Policy objects are strongly typed with full IntelliSense support
- **Simplicity**: No need to maintain separate policy dictionaries

## Configuration Examples

### Complete Service Configuration

```json
{
  "Gateway": {
    "TargetServices": [
      {
        "ServiceId": "user-api",
        "Instances": [
          { "Address": "https://localhost:5001", "Weight": 1 }
        ],
        "LoadBalancingStrategy": "RoundRobin",
        
        "RateLimitPolicy": {
          "RequestsPerWindow": 100,
          "WindowSize": "00:01:00",
          "Algorithm": "SlidingWindow"
        },
        
        "CachePolicy": {
          "Duration": "00:05:00",
          "Methods": ["GET"],
          "VaryByHeaders": ["Accept", "Accept-Language"],
          "VaryByQuery": true,
          "VaryByUser": false
        },
        
        "AuthPolicy": {
          "Name": "internal-api",
          "ValidIssuers": ["https://your-issuer.com"],
          "ValidAudiences": ["your-api-audience"],
          "ValidateIssuer": true,
          "ValidateAudience": true,
          "ValidateLifetime": true,
          "ValidateIssuerSigningKey": true,
          "IssuerSigningKey": "your-base64-secret-key"
        }
      }
    ]
  }
}
```

### Service Without Policies

```json
{
  "ServiceId": "public-api",
  "Instances": [{ "Address": "https://localhost:5002", "Weight": 1 }],
  "LoadBalancingStrategy": "RoundRobin",
  "RateLimitPolicy": null,
  "CachePolicy": null,
  "AuthPolicy": null
}
```

### Rate Limit Policy Options

```json
"RateLimitPolicy": {
  "RequestsPerWindow": 50,        // Number of requests allowed
  "WindowSize": "00:00:10",       // Time window (10 seconds)
  "Algorithm": "TokenBucket"      // SlidingWindow, TokenBucket, or FixedWindow
}
```

### Cache Policy Options

```json
"CachePolicy": {
  "Duration": "00:30:00",              // Cache duration (30 minutes)
  "Methods": ["GET", "HEAD"],          // HTTP methods to cache
  "VaryByHeaders": ["Accept"],         // Headers that affect cache key
  "VaryByQuery": true,                 // Include query string in cache key
  "VaryByUser": false                  // Include user identity in cache key
}
```

### Auth Policy Options

```json
"AuthPolicy": {
  "Name": "azure-ad",
  "ValidIssuers": [
    "https://login.microsoftonline.com/{tenant}/v2.0"
  ],
  "ValidAudiences": [
    "api://your-app-id"
  ],
  "ValidateIssuer": true,
  "ValidateAudience": true,
  "ValidateLifetime": true,
  "ValidateIssuerSigningKey": true,
  "Authority": "https://login.microsoftonline.com/{tenant}/v2.0"
}
```

## Migration Guide

### Before (Named Policies)

```json
{
  "Gateway": {
    "TargetServices": [
      {
        "ServiceId": "api",
        "RateLimitPolicy": "Basic",
        "CachePolicy": "Short"
      }
    ],
    "RateLimiting": {
      "Policies": {
        "Basic": { "RequestsPerWindow": 100, "WindowSize": "00:01:00" }
      }
    },
    "Caching": {
      "Policies": {
        "Short": { "Duration": "00:01:00", "Methods": ["GET"] }
      }
    }
  }
}
```

### After (Inline Policies)

```json
{
  "Gateway": {
    "TargetServices": [
      {
        "ServiceId": "api",
        "RateLimitPolicy": {
          "RequestsPerWindow": 100,
          "WindowSize": "00:01:00",
          "Algorithm": "SlidingWindow"
        },
        "CachePolicy": {
          "Duration": "00:01:00",
          "Methods": ["GET"],
          "VaryByHeaders": [],
          "VaryByQuery": true,
          "VaryByUser": false
        }
      }
    ]
  }
}
```

## Minimal Configuration Sections

The `RateLimiting` and `Caching` configuration sections are now minimal:

```json
{
  "Gateway": {
    "Caching": {
      "EnableCaching": true,
      "DefaultProvider": "Memory",
      "Memory": {
        "SizeLimit": 1000,
        "DefaultExpiration": "00:05:00"
      }
    },
    "RateLimiting": {
      "EnableGlobalRateLimit": false
    }
  }
}
```

## Code Changes

### Policy Models Location

All policy models are now in `Gateway.Common.Configuration`:
- `RateLimitPolicy`
- `RateLimitAlgorithm` (enum)
- `CachePolicy`
- `AuthPolicy`

### Service Interface Changes

Services now accept policy objects directly instead of policy names:

```csharp
// Old
Result<RateLimitResult> ApplyRateLimit(HttpContext context, string policyName);

// New
Result<RateLimitResult> ApplyRateLimit(HttpContext context, RateLimitPolicy policy);
```

### Middleware Changes

Middlewares now pass policy objects:

```csharp
// Old
if (targetService?.RateLimitPolicy == null) return;
rateLimitService.ApplyRateLimit(context, targetService.RateLimitPolicy);

// New  
if (targetService?.RateLimitPolicy == null) return;
rateLimitService.ApplyRateLimit(context, targetService.RateLimitPolicy);
```
