using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class XECM_M_PRODUCT5_2 : XECM_M_PRODUCT5
    {
        public bool FIRST_LOAD { get; set; }

        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }
    }



    public class XECM_M_PRODUCT5_2_METADATA
    {
      
    }

    public class XECM_M_PRODUCT5_REQUEST : REQUEST_MODEL
    {
        public XECM_M_PRODUCT5_2 data { get; set; }
    }

    public class XECM_M_PRODUCT5_REQUEST_LIST : REQUEST_MODEL
    {
        public List<XECM_M_PRODUCT5_2> data { get; set; }
    }

    public class XECM_M_PRODUCT5_RESULT : RESULT_MODEL
    {
        public List<XECM_M_PRODUCT5_2> data { get; set; }
    }
}
