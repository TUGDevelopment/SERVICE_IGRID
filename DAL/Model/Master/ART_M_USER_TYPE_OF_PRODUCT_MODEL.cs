using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    public partial class ART_M_USER_TYPE_OF_PRODUCT_2 : ART_M_USER_TYPE_OF_PRODUCT
    {
        public bool CHECKED { get; set; }
      
    }

    public class ART_M_USER_TYPE_OF_PRODUCT_REQUEST : REQUEST_MODEL
    {
        public ART_M_USER_TYPE_OF_PRODUCT_2 data { get; set; }
    }

    public class ART_M_USER_TYPE_OF_PRODUCT_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_M_USER_TYPE_OF_PRODUCT_2> data { get; set; }
    }

    public class ART_M_USER_TYPE_OF_PRODUCT_RESULT : RESULT_MODEL
    {
        public List<ART_M_USER_TYPE_OF_PRODUCT_2> data { get; set; }
        public bool ONLY_SAVE { get; set; } // ticket 453346  by aof
    }
}