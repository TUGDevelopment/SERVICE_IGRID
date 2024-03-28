using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class SAP_M_BRAND_2 : SAP_M_BRAND
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
    }

    public class SAP_M_BRAND_2_METADATA
    {
        [Required]
        public string DISPLAY_NAME { get; set; }
    }

    public class SAP_M_BRAND_REQUEST : REQUEST_MODEL
    {
        public SAP_M_BRAND_2 data { get; set; }
    }

    public class SAP_M_BRAND_RESULT : RESULT_MODEL
    {
        public List<SAP_M_BRAND_2> data { get; set; }
    }
}
