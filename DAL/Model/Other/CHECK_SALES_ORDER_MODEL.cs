using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    
    public class SALES_ORDER
    {
        public string REVIEWER { get; set; }
        public int SOLD_TO_ID { get; set; }
        public int SHIP_TO_ID { get; set; }
        public int COUNTRY_ID { get; set; }
        public int PRODUCTION_PLANT_ID { get; set; }
        public int BRAND_ID { get; set; }
        public string SOLD_TO_DISPLAY_TXT { get; set; }
        public string SHIP_TO_DISPLAY_TXT { get; set; }
        public string COUNTRY_DISPLAY_TXT { get; set; }
        public string PRODUCTION_PLANT_DISPLAY_TXT { get; set; }
        public string BRAND_DISPLAY_TXT { get; set; }
    }

    public class SALES_ORDER_METADATA
    {
       
    }

    public class SALES_ORDER_REQUEST : REQUEST_MODEL
    {
        public SALES_ORDER data { get; set; }
    }

    public class SALES_ORDER_RESULT : RESULT_MODEL
    {
        public List<SALES_ORDER> data { get; set; }
    }
}