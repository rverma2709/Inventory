using App.AdminPortal.Common;
using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Root.Models.StoredProcedures;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Services.DBContext;

namespace App.AdminPortal.Controllers
{
    public class DeviceItemController : AdminPortalController
    {
        public DeviceItemController(AdminPortalStaticService staticService, IHttpContextAccessor httpContextAccessor) : base(staticService, httpContextAccessor, "Device Item List")
        {
        }

        public async Task<IActionResult> DeviceItemList(SFGetDeviceItem sFGetDeviceItem)
        {
            sFGetDeviceItem.ReciverUserId = LoginUserId;
            List<DeviceItems> deviceItems = new List<DeviceItems>();
            List<InventoryRole> InventoryRoles=   (await _staticService._cacheRepo.InventoryUsersRoles()).Where(x=>x.InventoryRoleId!=LoginUser.InventoryRoleId).ToList();
            ViewBag.InventoryRoles = (InventoryRoles.Select(x => new { x.InventoryRoleId, x.RoleName }).ToList());
            List<SelectListItem> PoDetails = await _staticService.ExecuteSPAsync<DBEntities, SelectListItem>(new SFGetPoNumber());
            ViewBag.PoDetails = (PoDetails.Select(x => new { x.Value, x.Text }).ToList());
            


            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") // Check for AJAX requests
            {
                try
                {
                    deviceItems = await _staticService.ExecuteSPAsync<DBEntities, DeviceItems>(sFGetDeviceItem);

                }
                catch (Exception ex)
                {
                    await CatchError(ex);
                }
                return PartialView("_DeviceItemTable", Tuple.Create(deviceItems, sFGetDeviceItem));
            }
            return View(Tuple.Create(deviceItems, sFGetDeviceItem));
           
        }
    }
}
