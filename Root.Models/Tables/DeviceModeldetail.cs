using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Root.Models.Tables
{
    public class DeviceModeldetail : CommonProperties
    {
        [Key]
        [ScaffoldColumn(false)]
        public long DeviceModeldetailId { get; set; }

        [DisplayName("Device Model Name")]
        public string? ModelName { get; set; }
    }
}
