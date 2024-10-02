using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.Tables
{
    public class InquiryDetail
    {
        [Key]
        [ScaffoldColumn(false)]
        public long InquiryDetailId { get; set; }
        public string UserName { get; set; }
        public string? ContactNo { get; set; }
        public string? EmailId { get; set; }

        public string? DeviceType { get; set; }
        public long? Quantity { get; set; }

    }
}
