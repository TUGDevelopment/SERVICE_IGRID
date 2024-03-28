using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class PP_VENDOR_MODEL
    {
        public string VENDOR_DISPLAY_TXT { get; set; }
        public string GROUPING { get; set; }
        public string PO { get; set; }
        public string PKG_CODE { get; set; }
        public string SALES_ORDER_NO { get; set; }
        public Nullable<DateTime> RDD { get; set; }
        public string BRAND { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string WORKFLOW_NO { get; set; }
        public int ARTWORK_SUB_ID { get; set; }
        public int ARTWORK_ITEM_ID { get; set; }
        public int ARTWORK_REQUEST_ID { get; set; }
        public int VENDOR_ID { get; set; }
        public string IS_LOCK { get; set; }
    }

    public class PP_VENDOR_METADATA
    {

    }

    public class PP_VENDOR_REQUEST : REQUEST_MODEL
    {
        public PP_VENDOR_MODEL data { get; set; }
    }

    public class PP_VENDOR_REQUEST_LIST : REQUEST_MODEL
    {
        public List<PP_VENDOR_MODEL> data { get; set; }
    }

    public class PP_VENDOR_RESULT : RESULT_MODEL
    {
        public List<PP_VENDOR_MODEL> data { get; set; }
    }
}
