using Gateway.Common.Models.Result;
using Microsoft.AspNetCore.Http;

namespace Gateway.Core;

public interface IGatewayHandler
{
    ValueTask<Result> RouteRequestAsync(HttpContext context);
}
