using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
   public class TU_MATERIAL_LOCK_REPORT_MODEL
    {
        public string GROUPING { get; set; }
        public int MATERIAL_LOCK_ID { get; set; }
        public string MATERIAL_NO { get; set; }
        public string MATERIAL_DESCRIPTION { get; set; }
        public string IS_HAS_FILES { get; set; }
        public string STATUS { get; set; }
        public Nullable<System.DateTime> UNLOCK_DATE_FROM { get; set; }
        public Nullable<System.DateTime> UNLOCK_DATE_TO { get; set; }
        public string REMARK_UNLOCK { get; set; }
        public string REMARK_LOCK { get; set; }
        public Nullable<System.DateTime> LOG_DATE { get; set; }


        public string REQUEST_FORM_NO { get; set; }
        public string ARTWORK_NO { get; set; }
        public string MOCKUP_NO { get; set; }
        public Nullable<int> REQUEST_FORM_ID { get; set; }
        public Nullable<int> ARTWORK_ID { get; set; }
        public Nullable<int> MOCKUP_ID { get; set; }


        public string PRODUCT_CODE { get; set; }
        public string SOLD_TO { get; set; }
        public string SHIP_TO { get; set; }
        public string BRAND { get; set; }
        public string COUNTRY { get; set; }
        public string ZONE { get; set; }


        public string PACKAGING_TYPE { get; set; }
        public string PRIMARY_TYPE { get; set; }
        public string PRIMARY_SIZE { get; set; }
        public string PACK_SIZE { get; set; }
        public string PACKAGING_STYLE { get; set; }

        public string PA_OWNER { get; set; }
        public string PG_OWNER { get; set; }




        public string SEARCH_SOLD_TO { get; set; }
        public string SEARCH_SHIP_TO { get; set; }
        public string SEARCH_MATERIAL_NO { get; set; }
        public string SEARCH_BRAND { get; set; }
        public string SEARCH_COUNTRY { get; set; }
        public string SEARCH_PAOWNER { get; set; }
        public string SEARCH_ZONE { get; set; }
        public string SEARCH_STATUS { get; set; }
        public string SEARCH_PRODUCT_CODE { get; set; }
        public string SEARCH_PKG_TYPE { get; set; }
        public string SEARCH_REMARK_UNLOCK { get; set; }
        public string SEARCH_REMARK_LOCK { get; set; }
        public string SEARCH_CHECK_ARTWORK_FILE { get; set; }
        public string SEARCH_SERVER_SIDE { get; set; }


        public List<TU_MATERIAL_LOCK_DETAIL_REPORT_MODEL> listDETAIL { get; set; }
    }


    public class TU_MATERIAL_LOCK_DETAIL_REPORT_MODEL
    {


        public string GROUPING { get; set; }
        public string MATERIAL_NO { get; set; }

        public int MATERIAL_LOCK_ID { get; set; }
        public string SALES_ORDER_NO { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string SOLD_TO { get; set; }
        public string SHIP_TO { get; set; }
        public string BRAND { get; set; }
        public string COUNTRY { get; set; }
        public string ZONE { get; set; }
     


        public string REQUEST_FORM_NO { get; set; }
        public string ARTWORK_NO { get; set; }
        public string MOCKUP_NO { get; set; }
        public Nullable<int> REQUEST_FORM_ID { get; set; }
        public Nullable<int> ARTWORK_ID { get; set; }
        public Nullable<int> MOCKUP_ID { get; set; }

    
        public string PACKAGING_TYPE { get; set; }
        public string PRIMARY_TYPE { get; set; }
        public string PRIMARY_SIZE { get; set; }
        public string PACK_SIZE { get; set; }
        public string PACKAGING_STYLE { get; set; }

        public string PA_OWNER { get; set; }
        public string PG_OWNER { get; set; }
    }


    public class TU_MATERIAL_LOCK_REPORT_MODEL_REQUEST : REQUEST_MODEL
    {
        public TU_MATERIAL_LOCK_REPORT_MODEL data { get; set; }
    }

    public class TU_MATERIAL_LOCK_REPORT_MODEL_RESULT : RESULT_MODEL
    {

        public List<TU_MATERIAL_LOCK_REPORT_MODEL> data { get; set; }
    }


}
