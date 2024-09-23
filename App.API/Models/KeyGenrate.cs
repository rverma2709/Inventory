using Microsoft.AspNetCore.Mvc.Filters;
using Root.Models.Utils;
using Root.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.API.Models
{
   

    public class KeyGenrate : Attribute, IAsyncResultFilter
    {
        private readonly ICacheService _cacheService;
        public KeyGenrate(ICacheService cacheService) {
            _cacheService= cacheService;
        }
        

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            
            var uniqueKey = Guid.NewGuid().ToString();
            if(await _cacheService.KeyExists(CacheChannels.AdminPortal, "apikey"))
            {
                await _cacheService.Delete(CacheChannels.AdminPortal, "apikey");
            }
            await _cacheService.Write(CacheChannels.AdminPortal, "apikey", uniqueKey);
            
            context.HttpContext.Response.Headers.Add("apikey", uniqueKey);

            
            await next();
        }
    }

}
