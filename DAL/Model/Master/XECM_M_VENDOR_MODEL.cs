using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    
    [MetadataType(typeof(XECM_M_VENDOR_2_METADATA))]
    public partial class XECM_M_VENDOR_2 : XECM_M_VENDOR
    {
        public bool disabled { get; set; }
        public int MOCKUP_ID { get; set; }
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public int MATGROUP_ID { get; set; }
    }
    
    public class XECM_M_VENDOR_2_METADATA
    {
        [Required(ErrorMessage = "Vendor name is required")]
        public string VENDOR_NAME { get; set; }
    }

    public class XECM_M_VENDOR_REQUEST : REQUEST_MODEL
    {
        public XECM_M_VENDOR_2 data { get; set; }
    }

    public class XECM_M_VENDOR_RESULT : RESULT_MODEL
    {
        public List<XECM_M_VENDOR_2> data { get; set; }
    }
}
