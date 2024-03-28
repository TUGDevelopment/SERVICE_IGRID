using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_MATERIAL_LOCK_2 : ART_WF_ARTWORK_MATERIAL_LOCK
    {
        public string SEARCH_SOLD_TO { get; set; }
        public string SEARCH_SHIP_TO { get; set; }
        public string SEARCH_MATERIAL_NO { get; set; }
        public string SEARCH_BRAND { get; set; }
        public string SEARCH_COUNTRY { get; set; }
        public string SEARCH_PIC { get; set; }
        public string SEARCH_ZONE { get; set; }
        public string SEARCH_STATUS { get; set; }
        public string SEARCH_PRODUCT_CODE { get; set; }
        public string SEARCH_PKG_TYPE { get; set; }

        public int MATERIAL_LOCK_DETAIL_ID { get; set; }
        public string SALES_ORDER_NO { get; set; }
        public string SOLD_TO { get; set; }
        public string SHIP_TO { get; set; }
        public string BRAND { get; set; }
        public string COUNTRY { get; set; }
        public string ZONE { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string GROUPING { get; set; }
        public string GROUPINGTEMP { get; set; }
        public string STATUS_DISPLAY_TXT { get; set; }
        public string IS_HAS_FILES_DISPLAY_TXT { get; set; }
        public string PIC_DISPLAY_TXT { get; set; }
        public string PG_OWNER_DISPLAY_TXT { get; set; }

        public string UNLOCK_DATE_FROM_PARAM { get; set; }
        public string UNLOCK_DATE_TO_PARAM { get; set; }
        public string UPDATE_DATE_LOCK_PARAM { get; set; }
        public DateTime LOG_DATE { get; set; }

        public string GENERATE_EXCEL { get; set; }
        public bool FIRST_LOAD { get; set; }

        public bool IS_SEARCH { get; set; }


    }

    public class ART_WF_ARTWORK_MATERIAL_LOCK_2_METADATA
    {
         
    }

    public class ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_MATERIAL_LOCK_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_ARTWORK_MATERIAL_LOCK_2> data { get; set; }
       // public string SUMMIT_TYPE { get; set; }

    }

    public class ART_WF_ARTWORK_MATERIAL_LOCK_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_MATERIAL_LOCK_2> data { get; set; }
       // public string SUMMIT_TYPE { get; set; }
    }
}
