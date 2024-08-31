using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class DeviceTypeData
    {
        public long? DeviceTypeId { get; set; }
        [DisplayName("Device Type Name")]
        public string? DeviceName { get; set; }
    }
}
