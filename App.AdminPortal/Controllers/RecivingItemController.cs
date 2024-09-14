using App.AdminPortalServices.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Root.Models.StoredProcedures;
using Root.Models.Tables;
using Root.Models.Utils;
using Root.Models.ViewModels;
using Root.Services.DBContext;
using System.Collections.Generic;

namespace App.AdminPortal.Controllers
{
    public class RecivingItemController : AdminPortalController
    {
        public RecivingItemController(AdminPortalStaticService staticService, IHttpContextAccessor httpContextAccessor) : base(staticService, httpContextAccessor, "PageModelName")
        {
        }

        public async Task<IActionResult> Reciving()
        {
            List<SelectListItem> PoDetails = await _staticService.ExecuteSPAsync<DBEntities, SelectListItem>(new SFGetPoNumber());
            ViewBag.PoDetails = (PoDetails.Select(x => new { x.Value, x.Text }).ToList());
            return View(new RecivingItemView());
        }
        [HttpPost]
        public async Task<IActionResult> Reciving(RecivingItemView model, IFormFile BulkImportDocument)
        {
            List<SelectListItem> PoDetails = await _staticService.ExecuteSPAsync<DBEntities, SelectListItem>(new SFGetPoNumber());
            ViewBag.PoDetails = (PoDetails.Select(x => new { x.Value, x.Text }).ToList());
            return View(new RecivingItemView());
        }
        [HttpPost]
        public async Task<ResJsonOutput> GetGramPanchayatData(long? PoDetailId)
        {
            ResJsonOutput DDData = new ResJsonOutput();

            var gpil = (await _staticService.ExecuteSPAsync<DBEntities, GetPoNumberItemList>(new SFGetPoNumberItemDetilas() { PoDetailId = PoDetailId }));
           

            DDData.Data = CommonLib.ConvertObjectToJson(gpil);
            DDData.Status.IsSuccess = true;
            return DDData;
        }
    }
}
