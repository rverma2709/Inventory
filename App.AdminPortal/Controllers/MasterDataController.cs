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
        private readonly IDataService<DBEntities, DeviceModeldetail> _DeviceModeldetail;
        public MasterDataController(
            AdminPortalStaticService staticService, 
            IHttpContextAccessor httpContextAccessor, 
            IDataService<DBEntities, DeviceType> DeviceType, 
            IDataService<DBEntities, DeviceModeldetail> DeviceModeldetail) : base(staticService, httpContextAccessor, "Device Master")
        {
            _DeviceType = DeviceType;
            _DeviceModeldetail = DeviceModeldetail;
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

        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public async Task<IActionResult> DeviceModelMaster(SFGetDeviceModeldetails sFGetDeviceModel)
        {
            List<DeviceModeldetail> deviceModeldetail = new List<DeviceModeldetail>();

            try
            {
                ViewBag.PageModelName = "Device Model Master";
                ResJsonOutput result = await _staticService.FetchList<DeviceModeldetail>(_DeviceModeldetail, sFGetDeviceModel);
                if (result.Status.IsSuccess)
                {
                    deviceModeldetail = await FetchList<DeviceModeldetail>(result, sFGetDeviceModel);

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
                return PartialView("_DeviceTablePartial", Tuple.Create(deviceModeldetail, sFGetDeviceModel));
            }
            return View(Tuple.Create(deviceModeldetail, sFGetDeviceModel));
        }

        [HttpPost]
        public async Task<IActionResult> AddDeviceModelMaster(DeviceModelTypeData deviceModelData)
        {
            DeviceModeldetail deviceModels =  new DeviceModeldetail()
            {
                ModelName = deviceModelData.ModelName
            };

            await _DeviceModeldetail.Create(deviceModels);
            await _DeviceModeldetail.Save();
            HttpContext.Session.SetObject(ProgConstants.SuccMsg, "Data successfully save");
            return RedirectToAction("DeviceModelMaster", "MasterData");
        }
    }
}
