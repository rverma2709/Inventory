using App.AdminPortal.Common;
using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Mvc;
using Root.Models.StoredProcedures;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Models.ViewModels;
using Root.Services.DBContext;
using Root.Services.Interfaces;

namespace App.AdminPortal.Controllers
{
    public class VendorMasterController :  AdminPortalController
    {
        private readonly IDataService<DBEntities, VendorDetail> _VendorDetail;
        public VendorMasterController(AdminPortalStaticService staticService, IHttpContextAccessor httpContextAccessor, IDataService<DBEntities, VendorDetail> VendorDetail) : base(staticService, httpContextAccessor, "Vendor Details")
        {
            _VendorDetail = VendorDetail;
        }
        [TypeFilter(typeof(Authorize), Arguments = new object[] { false })]
        public async Task<IActionResult> VendorDetails(SFGetVendorDetails sFGetVendorDetails)
        {
            List<VendorDetail> vendorDetails = new List<VendorDetail>();

            try
            {
                ResJsonOutput result = await _staticService.FetchList<VendorDetail>(_VendorDetail, sFGetVendorDetails);
                if (result.Status.IsSuccess)
                {
                    vendorDetails = await FetchList<VendorDetail>(result, sFGetVendorDetails);

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
                return PartialView("_VendorDetailTable", Tuple.Create(vendorDetails, sFGetVendorDetails));
            }
            return View(Tuple.Create(vendorDetails, sFGetVendorDetails));
        }

        [HttpPost]
        [PreventDuplicateRequests]
        public async Task<IActionResult> AddVendorMaster(ViewVendorDetail deviceType)
        {
            try
            {
                Tuple<bool, string> tuple = ValidateModel(deviceType);
                if(tuple.Item1)
                {
                    VendorDetail vendorDetail = new VendorDetail() { 
                       VendorName = deviceType.VendorName,
                       CompanyName = deviceType.CompanyName,
                       ContactNo1 = deviceType.ContactNo1,
                       ContactNo2   = deviceType.ContactNo2,
                       EmailId  = deviceType.EmailId,
                       GSTNo = deviceType.GSTNo,
                       VendorAddress = deviceType.VendorAddress
                    };
                   await  _VendorDetail.Create(vendorDetail);
                    await _VendorDetail.Save();
                    HttpContext.Session.SetObject(ProgConstants.SuccMsg, "Data successfully save");
                }
                else
                {
                    HttpContext.Session.SetObject(ProgConstants.ErrMsg, tuple.Item2);
                }
               
            }
            catch (Exception ex)
            {
                await CatchError(ex);
                HttpContext.Session.SetObject(ProgConstants.ErrMsg, "Something Error");
            }

            return RedirectToAction("VendorDetails", "VendorMaster");
        }

       
    }
}
