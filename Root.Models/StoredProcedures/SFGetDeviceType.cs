using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Root.Models.StoredProcedures
{
    public class SFGetDeviceType: SFParameters
    {
        public SFGetDeviceType()
        {
            cols = "AnnexureDetailsId";
            order = "DESC";
        }
        [QueryParam]
        public string BatchNo { get; set; }
        [QueryParam]
        public string AnnexureType { get; set; }
        [QueryParam]
        public long? BankId { get; set; }
    }
}
