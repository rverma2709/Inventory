using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.AdminPortal.Controllers
{
    public class VendorMasterController :  AdminPortalController
    {
        public VendorMasterController(AdminPortalStaticService staticService, IHttpContextAccessor httpContextAccessor) : base(staticService, httpContextAccessor, "Vendor Details")
        {
        }

        public IActionResult VendorDetails()
        {
            return View();
        }
    }
}
