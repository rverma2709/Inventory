using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Root.Services.Interfaces;

namespace App.API.Models
{
    public class UniqueKeyMatching : Attribute, IAsyncActionFilter
    {
        private readonly ICacheService _cacheService;

        public UniqueKeyMatching(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            
            var requestHeaders = context.HttpContext.Request.Headers;

            if (requestHeaders.TryGetValue("X-Unique-Key", out var requestUniqueKey))
            {
                
                var cachedUniqueKey =  await _cacheService.Read(CacheChannels.AdminPortal, "X-Unique-Key"); ;

               
                if (cachedUniqueKey == requestUniqueKey)
                {
                    
                    await next();
                }
                else
                {
                    
                    context.Result = new ContentResult
                    {
                        StatusCode = StatusCodes.Status403Forbidden,
                        Content = "Forbidden: Unique key mismatch"
                    };
                }
            }
            else
            {
                
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Content = "Bad Request: X-Unique-Key header missing"
                };
            }
        }
    }
}
