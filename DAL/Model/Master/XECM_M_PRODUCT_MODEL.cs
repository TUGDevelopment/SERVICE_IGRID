using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class XECM_M_PRODUCT_2 : XECM_M_PRODUCT
    {
        public int ROW { get; set; }
        public int PRODUCT_CODE_ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public int ID { get; set; }
        public string PRODUCTION_PLANT { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public bool FIRST_LOAD { get; set; }
    }

    public class XECM_M_PRODUCT_2_METADATA
    {
      
    }

    public class XECM_M_PRODUCT_REQUEST : REQUEST_MODEL
    {
        public XECM_M_PRODUCT_2 data { get; set; }
    }

    public class XECM_M_PRODUCT_REQUEST_LIST : REQUEST_MODEL
    {
        public List<XECM_M_PRODUCT_2> data { get; set; }
    }

    public class XECM_M_PRODUCT_RESULT : RESULT_MODEL
    {
        public List<XECM_M_PRODUCT_2> data { get; set; }
    }
}
