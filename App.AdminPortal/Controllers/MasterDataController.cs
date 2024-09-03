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
using System.Linq.Expressions;

namespace App.AdminPortal.Controllers
{
    public class MasterDataController : AdminPortalController
    {
        private readonly IDataService<DBEntities, DeviceType> _DeviceType;
        private readonly IDataService<DBEntities, BrandDetail> _BrandDetail;
        public MasterDataController(AdminPortalStaticService staticService, IHttpContextAccessor httpContextAccessor, IDataService<DBEntities, DeviceType> DeviceType, IDataService<DBEntities, BrandDetail> brandDetail) : base(staticService, httpContextAccessor, "Device Master")
        {
            _DeviceType = DeviceType;
            _BrandDetail = brandDetail;
        }
        public async Task FormInitialise()
        {
            ViewBag.DeviceType = (await _staticService._cacheRepo.DeviceTypeList());
           
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
            try
            {
                DeviceType device = new DeviceType()
                {
                    DeviceName = deviceType.DeviceName
                };

                await _DeviceType.Create(device);
                await _DeviceType.Save();
                ViewBag.DeviceType = (await _staticService._cacheRepo.DeviceTypeList(true));
                HttpContext.Session.SetObject(ProgConstants.SuccMsg, "Data successfully save");
            }
            catch (Exception ex)
            {
                await CatchError(ex);
                HttpContext.Session.SetObject(ProgConstants.ErrMsg, "Something Error");
            }
           
            return RedirectToAction("DeviceMaster", "MasterData");
        }
        [HttpPost]
        public async Task<IActionResult> RemoveDeviceMaster(RequestById requestById)
        {
           
            return RedirectToAction("DeviceMaster", "MasterData");
        }

        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public async Task<IActionResult> BrandDetails(SFGetBrandDetails sFGetBrandDetails)
        {
            ViewBag.PageModelName = "Brand Details";
            List<BrandDetail> brandDetails = new List<BrandDetail>();

            try
            {
                ResJsonOutput result = await _staticService.FetchList<BrandDetail>(_BrandDetail, sFGetBrandDetails, new Expression<Func<BrandDetail, object>>[] { a=>a.DeviceType});
                if (result.Status.IsSuccess)
                {
                    brandDetails = await FetchList<BrandDetail>(result, sFGetBrandDetails);

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
                return PartialView("_DeviceTablePartial", Tuple.Create(brandDetails, sFGetBrandDetails));
            }
            return View(Tuple.Create(brandDetails, sFGetBrandDetails));
        }


    }
}
