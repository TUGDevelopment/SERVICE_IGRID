using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{

    public class SALES_ORDER_ITEM_POPUP
    {
        public string SOLD_TO_NAME { get; set; }
        public string SHIP_TO_NAME { get; set; }
        public string SALES_ORG { get; set; }
        public string SO_NUMBER { get; set; }
        public string SO_ITEM_NO { get; set; }
        public string BRAND { get; set; }
        public string MATERIAL_NO { get; set; }
        public string MATERIAL_DESC { get; set; }
        public string PORT { get; set; }
        public string PRODUCTION_PLANT { get; set; }
        public string ORDER_BOM_NO { get; set; }
        public string ORDER_BOM_DESC { get; set; }
        public string QUANTITY { get; set; }
        public string STOCK_PO { get; set; }
        public string RDD { get; set; }
        public string GROUPING_DISPLAY_TXT { get; set; }
        public string ASSIGN_ID { get; set; }

        public string VALIDATE_MESSAGE { get; set; }
    }

    public class SALES_ORDER_ITEM_POPUP_METADATA
    {

    }

    public class SALES_ORDER_ITEM_POPUP_REQUEST : REQUEST_MODEL
    {
        public SALES_ORDER_ITEM_POPUP data { get; set; }
    }

    public class SALES_ORDER_ITEM_POPUP_REQUEST_LIST : REQUEST_MODEL
    {
        public List<SALES_ORDER_ITEM_POPUP> data { get; set; }
    }

    public class SALES_ORDER_ITEM_POPUP_RESULT : RESULT_MODEL
    {
        public List<SALES_ORDER_ITEM_POPUP> data { get; set; }
    }
}