using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    [MetadataType(typeof(XECM_M_CUSTOMER_2_METADATA))]
    public partial class XECM_M_CUSTOMER_2 : XECM_M_CUSTOMER
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
    }

    public class XECM_M_CUSTOMER_2_METADATA
    {
        [Required(ErrorMessage = "Customer name is required")]
        public string CUSTOMER_NAME { get; set; }
    }

    public class XECM_M_CUSTOMER_REQUEST : REQUEST_MODEL
    {
        public XECM_M_CUSTOMER_2 data { get; set; }
    }

    public class XECM_M_CUSTOMER_RESULT : RESULT_MODEL
    {
        public List<XECM_M_CUSTOMER_2> data { get; set; }
    }
}

