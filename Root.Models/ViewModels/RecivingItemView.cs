using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Root.Models.ViewModels
{
    public class RecivingItemView
    {
        public long? PoDetailId { get; set; }
        public long? PoItemDetilId { get; set; }
        public long? Quantity { get; set; }
        [Required]
        [DisplayName("Upload Document")]
        public string BulkImportDocument { get; set; }
    }
}
