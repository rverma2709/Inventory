﻿using App.AdminPortal.Common;
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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace App.AdminPortal.Controllers
{
    public class MasterDataController : AdminPortalController
    {
        private readonly IDataService<DBEntities, DeviceType> _DeviceType;
        private readonly IDataService<DBEntities, DeviceModeldetail> _DeviceModeldetail;
        private readonly IDataService<DBEntities, BrandDetail> _BrandDetail;
        private readonly IDataService<DBEntities, DeviceProcessorDetail> _DeviceProcessorDetail;
        private readonly IDataService<DBEntities, GenerationDetail> _GenerationDetail;
        private readonly IDataService<DBEntities, RAMDetail> _RAMDetail;
        private readonly IDataService<DBEntities, HardDiskDetail> _HardDiskDetail;
        private readonly IDataService<DBEntities, ProcurementType> _ProcurementType;
        public MasterDataController(AdminPortalStaticService staticService, IHttpContextAccessor httpContextAccessor, IDataService<DBEntities, DeviceType> DeviceType,IDataService<DBEntities, DeviceModeldetail> DeviceModeldetail, IDataService<DBEntities, BrandDetail> brandDetail, IDataService<DBEntities, DeviceProcessorDetail> deviceProcessorDetail, IDataService<DBEntities, GenerationDetail> generationDetail, IDataService<DBEntities, RAMDetail> rAMDetail, IDataService<DBEntities, HardDiskDetail> hardDiskDetail, IDataService<DBEntities, ProcurementType> procurementType) : base(staticService, httpContextAccessor, "Device Master")
        {
            _DeviceType = DeviceType;
            _DeviceModeldetail = DeviceModeldetail;
            _BrandDetail = brandDetail;
            _DeviceProcessorDetail = deviceProcessorDetail;
            _GenerationDetail = generationDetail;
            _RAMDetail = rAMDetail;
            _HardDiskDetail = hardDiskDetail;
            _ProcurementType = procurementType;
        }
        public async Task FormInitialise()
        {
            List<DeviceType> deviceTypes = (await _staticService._cacheRepo.DeviceTypeList());
            ViewBag.DeviceType = (deviceTypes.Select(x => new { x.DeviceTypeId, x.DeviceName }).ToList());

          

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
        public async Task<IActionResult> DeviceModelMaster(SFGetDeviceModeldetails sFGetDeviceModel)
        {
            List<DeviceModeldetail> deviceModeldetail = new List<DeviceModeldetail>();

            try
            {
                ViewBag.PageModelName = "Device Model Master";
                ResJsonOutput result = await _staticService.FetchList<DeviceModeldetail>(_DeviceModeldetail, sFGetDeviceModel, new Expression<Func<DeviceModeldetail, object>>[] { a => a.DeviceType});
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
        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public async Task<IActionResult> BrandDetails(SFGetBrandDetails sFGetBrandDetails)
        {
           await FormInitialise();
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
                return PartialView("_BrandDetailTablePartial", Tuple.Create(brandDetails, sFGetBrandDetails));
            }
            return View(Tuple.Create(brandDetails, sFGetBrandDetails));
        }
        [HttpPost]
        public async Task<IActionResult> AddBrandDetails(ViewBrandDetail  viewBrandDetail)
        {
            try
            {
                BrandDetail brandDetail = new BrandDetail()
                {
                    DeviceTypeId = (long)viewBrandDetail.DeviceTypeId,
                    BrandName = viewBrandDetail.BrandName,


                };

                await _BrandDetail.Create(brandDetail);
                await _BrandDetail.Save();
               // ViewBag.DeviceType = (await _staticService._cacheRepo.DeviceTypeList(true));
                HttpContext.Session.SetObject(ProgConstants.SuccMsg, "Data successfully save");
            }
            catch (Exception ex)
            {
                await CatchError(ex);
                HttpContext.Session.SetObject(ProgConstants.ErrMsg, "Something Error");
            }

            return RedirectToAction("BrandDetails", "MasterData");
        }

        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public async Task<IActionResult> DeviceProcessorDetails(SFGetDeviceProcessorDetails sFGetDeviceProcessorDetails)
        {
            ViewBag.PageModelName = "Device Processor Details";
            List<DeviceProcessorDetail> deviceProcessorDetails = new List<DeviceProcessorDetail>();

            try
            {
                ResJsonOutput result = await _staticService.FetchList<DeviceProcessorDetail>(_DeviceProcessorDetail, sFGetDeviceProcessorDetails, new Expression<Func<DeviceProcessorDetail, object>>[] { a => a.DeviceType });
                if (result.Status.IsSuccess)
                {
                    deviceProcessorDetails = await FetchList<DeviceProcessorDetail>(result, sFGetDeviceProcessorDetails);

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
                return PartialView("_DeviceProcessorTable", Tuple.Create(deviceProcessorDetails, sFGetDeviceProcessorDetails));
            }
            return View(Tuple.Create(deviceProcessorDetails, sFGetDeviceProcessorDetails));
        }

        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public async Task<IActionResult> GenerationDetails(SFGetGenerationDetails sFGetGenerationDetails)
        {
            ViewBag.PageModelName = "Generation Details";
            List<GenerationDetail> generationDetails = new List<GenerationDetail>();

            try
            {
                ResJsonOutput result = await _staticService.FetchList<GenerationDetail>(_GenerationDetail, sFGetGenerationDetails, new Expression<Func<GenerationDetail, object>>[] { a => a.DeviceType });
                if (result.Status.IsSuccess)
                {
                    generationDetails = await FetchList<GenerationDetail>(result, sFGetGenerationDetails);

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
                return PartialView("_GenerationTable", Tuple.Create(generationDetails, sFGetGenerationDetails));
            }
            return View(Tuple.Create(generationDetails, sFGetGenerationDetails));
        }

        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public async Task<IActionResult> RAMDetails(SFGetRAMDetails sFGetRAMDetails)
        {
            ViewBag.PageModelName = "RAM Details";
            List<RAMDetail> rAMDetails = new List<RAMDetail>();

            try
            {
                ResJsonOutput result = await _staticService.FetchList<RAMDetail>(_RAMDetail, sFGetRAMDetails, new Expression<Func<RAMDetail, object>>[] { a => a.DeviceType });
                if (result.Status.IsSuccess)
                {
                    rAMDetails = await FetchList<RAMDetail>(result, sFGetRAMDetails);

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
                return PartialView("_GenerationTable", Tuple.Create(rAMDetails, sFGetRAMDetails));
            }
            return View(Tuple.Create(rAMDetails, sFGetRAMDetails));
        }

        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public async Task<IActionResult> HardDiskDetails(SFGetHardDiskDetails sFGetHardDiskDetails)
        {
            ViewBag.PageModelName = "HardDisk Details";
            List<HardDiskDetail> hardDiskDetails = new List<HardDiskDetail>();

            try
            {
                ResJsonOutput result = await _staticService.FetchList<HardDiskDetail>(_HardDiskDetail, sFGetHardDiskDetails, new Expression<Func<HardDiskDetail, object>>[] { a => a.DeviceType });
                if (result.Status.IsSuccess)
                {
                    hardDiskDetails = await FetchList<HardDiskDetail>(result, sFGetHardDiskDetails);

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
                return PartialView("_HardDiskTable", Tuple.Create(hardDiskDetails, sFGetHardDiskDetails));
            }
            return View(Tuple.Create(hardDiskDetails, sFGetHardDiskDetails));
        }

        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public async Task<IActionResult> ProcurementTypes(SFGetProcurementType sFGetProcurementType)
        {
            ViewBag.PageModelName = "Procurement Types";
            List<ProcurementType> procurementTypes = new List<ProcurementType>();

            try
            {
                ResJsonOutput result = await _staticService.FetchList<ProcurementType>(_ProcurementType, sFGetProcurementType);
                if (result.Status.IsSuccess)
                {
                    procurementTypes = await FetchList<ProcurementType>(result, sFGetProcurementType);

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
                return PartialView("_ProcurementTypesPartial", Tuple.Create(procurementTypes, sFGetProcurementType));
            }
            return View(Tuple.Create(procurementTypes, sFGetProcurementType));
        }


    }
}
