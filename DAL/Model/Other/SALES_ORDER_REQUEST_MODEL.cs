using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class SALES_ORDER_REQUEST_FORM
   {
        public string SALES_ORDER_NO { get; set; }
        public string IN_TRANSIT { get; set; }
        public string VIA { get; set; }
        public bool SALES_ORDER_INCONSISTENCY { get; set; }
        public Nullable<int> REVIEWER_ID { get; set; }
        public string REVIEWER_DISPLAY_TXT { get; set; }
        public Nullable<int> SOLD_TO_ID { get; set; }
        public Nullable<int> SHIP_TO_ID { get; set; }
        public string SOLD_TO_DISPLAY_TXT { get; set; }
        public string SHIP_TO_DISPLAY_TXT { get; set; }
        public List<ART_WF_ARTWORK_REQUEST_COUNTRY_2> COUNTRY { get; set; }
        public List<XECM_M_PRODUCT_2> PRODUCT { get; set; }
        public List<ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2> PRODUCTION_PLANT { get; set; }
        public Nullable<int> BRAND_ID { get; set; }
        public string BRAND_OTHER { get; set; }
        public string BRAND_DISPLAY_TXT { get; set; }
        public Nullable<System.DateTime> REQUEST_DELIVERY_DATE { get; set; }
        public int ARTWORK_REQUEST_ID { get; set; }
        public string PRODUCT_TYPE { get; set; }
    }

    public class SALES_ORDER_REQUEST_FORM_METADATA
    {

    }

    public class SALES_ORDER_REQUEST_FORM_REQUEST : REQUEST_MODEL
    {
        public SALES_ORDER_REQUEST_FORM data { get; set; }
    }
    public class SALES_ORDER_REQUEST_FORM_REQUEST_LIST : REQUEST_MODEL
    {
        public List<SALES_ORDER_REQUEST_FORM> data { get; set; }
    }

    public class SALES_ORDER_REQUEST_FORM_RESULT : RESULT_MODEL
    {
        public List<SALES_ORDER_REQUEST_FORM> data { get; set; }
    }

   
}
