using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class ViewBrandDetail
    {
        public long? BrandDetailId { get; set; }
        public long? DeviceTypeId { get; set; }
        [DisplayName("Brand Name")]
        public string? BrandName { get; set; }
    }
}
