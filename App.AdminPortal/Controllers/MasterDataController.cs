using App.AdminPortal.Common;
using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.AdminPortal.Controllers
{
    public class MasterDataController : AdminPortalController
    {
        public MasterDataController(AdminPortalStaticService staticService, IHttpContextAccessor httpContextAccessor) : base(staticService, httpContextAccessor, "Device Master")
        {

        }
        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public IActionResult DeviceMaster()
        {
            return View();
        }
    }
}
