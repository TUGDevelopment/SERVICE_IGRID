using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class PP_MODEL
    {
        public string SALES_ORG { get; set; }
        public int SOLD_TO_ID { get; set; }
        public int SHIP_TO_ID { get; set; }
        public string SOLD_TO_DISPLAY_TXT { get; set; }
        public string SHIP_TO_DISPLAY_TXT { get; set; }
        public string SALES_ORDER { get; set; }
        public string SALES_ORDER_ITEM { get; set; }
        public Nullable<System.DateTime> RDD { get; set; }
        public int BRAND_ID { get; set; }
        public string PRODUCT_CODE { get; set; }
        public int PKG_TYPE_ID { get; set; }
        public string PKG_TYPE_DISPLAY_TXT { get; set; }
        public string BRAND_DISPLAY_TXT { get; set; }
        public string PKG_CODE { get; set; }
        public string VENDOR_DISPLAY_TXT { get; set; }
        public string WORKFLOW_NO { get; set; }
        public Nullable<System.DateTime> RECEIVE_DATE { get; set; }
        public int ARTWORK_REQUEST_ID { get; set; }
        public int ARTWORK_SUB_ID { get; set; }
        public int ARTWORK_ITEM_ID { get; set; }
        public string GROUPING { get; set; }
        public string IS_SALES_ORDER_CHANGE { get; set; }

        public string GET_BY_CREATE_DATE_FROM { get; set; }
        public string GET_BY_CREATE_DATE_TO { get; set; }
        public string IS_LOCK { get; set; }
        public int CURRENT_USER_ID { get; set; }
        public string REMARK_BY_PA { get; set; }
        public int STATUS { get; set; }
        public string PLANT { get; set; }
    }
    public partial class PP_MODEL2 : PP_MODEL
    {
    }
    public class PP_METADATA
    {

    }

    public class PP_REQUEST : REQUEST_MODEL
    {
        public PP_MODEL data { get; set; }
    }

    public class PP_RESULT : RESULT_MODEL
    {
        public List<PP_MODEL> data { get; set; }
    }
}
