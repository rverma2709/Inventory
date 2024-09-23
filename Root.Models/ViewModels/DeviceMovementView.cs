using Root.Models.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.ViewModels
{
    public class DeviceMovementView
    {
        public long? InventoryRoleId { get;set;}
        public long? ReceivingDeviceDetailId { get; set; }
        public long? SenderUserId { get; set; }
        public long? ReciverUserId { get; set; }
        public DateTime SendDate { get; set; }
        public bool? QCStatus { get; set; }
        public string? QCUserRemarks { get; set; }
        public string? DeviceOldserialNumber { get; set; }
        [ForeignKey("InventoryRoleId")]
        public virtual InventoryRole? InventoryRole { get; set; }
    }
}
