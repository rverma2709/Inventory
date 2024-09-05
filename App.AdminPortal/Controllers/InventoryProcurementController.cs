using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Root.Models.StoredProcedures;
using Root.Models.Tables;

namespace App.AdminPortal.Controllers
{
    public class InventoryProcurementController : AdminPortalController
    {
        public InventoryProcurementController(AdminPortalStaticService staticService, IHttpContextAccessor httpContextAccessor) : base(staticService, httpContextAccessor, "Inventory Procurement")
        {
        }
        public async Task FormInitialise()
        {
            List<DeviceType> deviceTypes = (await _staticService._cacheRepo.DeviceTypeList());
            List<BrandDetail> brandDetails = (await _staticService._cacheRepo.BrandDetails(true));
            List<DeviceProcessorDetail> deviceProcessorDetails = (await _staticService._cacheRepo.DeviceProcessorDetails(true));
            List<GenerationDetail> generationDetails = (await _staticService._cacheRepo.GenerationDetails(true));
            List<RAMDetail> rAMDetails = (await _staticService._cacheRepo.RAMDetails(true));
            List<HardDiskDetail> hardDiskDetails = (await _staticService._cacheRepo.HardDiskDetails(true));
            List<VendorDetail> vendorDetails = (await _staticService._cacheRepo.VendorDetails(true));
            List<ProcurementType> procurementTypes = (await _staticService._cacheRepo.ProcurementTypes(true));
            ViewBag.DeviceType = (deviceTypes.Select(x => new { x.DeviceTypeId, x.DeviceName }).ToList());
            ViewBag.brandDetails = (brandDetails.Select(x => new { x.BrandDetailId, x.BrandName }).ToList());
            ViewBag.deviceProcessorDetails = (deviceProcessorDetails.Select(x => new { x.DeviceProcessorDetailId, x.DeviceProcessorName }).ToList());
            ViewBag.generationDetails = (generationDetails.Select(x => new { x.GenerationDetailId, x.GenerationName }).ToList());
            ViewBag.rAMDetails = (rAMDetails.Select(x => new { x.RAMDetailId, x.RAMSize }).ToList());
            ViewBag.hardDiskDetails = hardDiskDetails.Select(x => new {
                x.HardDiskDetailId,
                HardDiskInfo = x.HardDiskSize + " " + x.HardDiskType
            }).ToList();
            ViewBag.vendorDetails = (vendorDetails.Select(x => new { x.VendorDetailId, x.VendorName }).ToList());
            ViewBag.procurementTypes = (procurementTypes.Select(x => new { x.ProcurementTypeId, x.ProcurementNameType }).ToList());

            List<SelectListItem> QualityCheckStatus = new List<SelectListItem>();

            QualityCheckStatus.Add(new SelectListItem { Text = "Yes", Value = "True" });
            QualityCheckStatus.Add(new SelectListItem { Text = "No", Value = "False" });
            ViewBag.QualityCheckStatus = QualityCheckStatus;


        }
        public async Task<IActionResult> Index(SFGetProcurementDetils sFGetProcurementDetils)
        {
            await FormInitialise();
            InventoryProcurementDetail procurementDetail = new InventoryProcurementDetail();

            return View(procurementDetail);
        }
    }
}
