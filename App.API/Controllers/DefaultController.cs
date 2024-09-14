using App.APIServices.Models;
using App.APIServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.API.Controllers
{
    [Produces("application/json")]
    [Route("API/Default")]
    public class DefaultController : Root.Services.Controller.DefaultController
    {
        
        protected readonly AppConfig _appConfig;
        protected new readonly App.APIServices.Services.APIStaticService _staticService;
        public DefaultController(APIStaticService staticService, IHttpContextAccessor httpContextAccessor) : base(staticService, httpContextAccessor)
        {
            _appConfig = staticService._appConfig;
            _staticService = staticService;
        }
    }
}
