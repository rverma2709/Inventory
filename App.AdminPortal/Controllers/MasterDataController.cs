using App.AdminPortal.Common;
using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Mvc;
using Root.Models.StoredProcedures;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Services.DBContext;
using Root.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Root.Models.ViewModels;
using Microsoft.AspNetCore.Http.HttpResults;

namespace App.AdminPortal.Controllers
{
    public class MasterDataController : AdminPortalController
    {
        private readonly IDataService<DBEntities, DeviceType> _DeviceType;
        public MasterDataController(AdminPortalStaticService staticService, IHttpContextAccessor httpContextAccessor, IDataService<DBEntities, DeviceType> DeviceType) : base(staticService, httpContextAccessor, "Device Master")
        {
            _DeviceType = DeviceType;
        }
        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public async Task<IActionResult> DeviceMaster(SFGetDeviceType sFGetDevice)
        {
            List<DeviceType> deviceTypes = new List<DeviceType>();
           
            try
            {
                ResJsonOutput result = await _staticService.FetchList<DeviceType>(_DeviceType, sFGetDevice);
                if (result.Status.IsSuccess)
                {
                    deviceTypes = await FetchList<DeviceType>(result, sFGetDevice);

                }
                else
                {
                    HttpContext.Session.SetObject(ProgConstants.ErrMsg, result.Status.Message);
                }
            }
            catch (Exception ex)
            {
                await CatchError(ex);
            }
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") // Check for AJAX requests
            {
                return PartialView("_DeviceTablePartial", Tuple.Create(deviceTypes, sFGetDevice));
            }
            return View(Tuple.Create(deviceTypes, sFGetDevice));
        }

        [HttpPost]
        public async Task<IActionResult> AddDeviceMaster(DeviceTypeData deviceType)
        {
            DeviceType device = new DeviceType() { 
              DeviceName = deviceType.DeviceName
            };

            await _DeviceType.Create(device);
            await _DeviceType.Save();
            HttpContext.Session.SetObject(ProgConstants.SuccMsg, "Data successfully save");
            return RedirectToAction("DeviceMaster", "MasterData");
        }



    }
}
