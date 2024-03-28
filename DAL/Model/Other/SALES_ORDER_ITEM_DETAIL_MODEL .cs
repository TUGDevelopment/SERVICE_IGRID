using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    
    public class SALES_ORDER_ITEM_DETAIL
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

        public SAP_M_PO_COMPLETE_SO_HEADER_2 SO_HEADER { get; set; }
        public SAP_M_PO_COMPLETE_SO_ITEM_2 SO_ITEM { get; set; }
        public SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2 SO_ITEM_COMPONENT { get; set; }

    }

    public class SALES_ORDER_ITEM_DETAIL_METADATA
    {
       
    }

    public class SALES_ORDER_ITEM_DETAIL_REQUEST : REQUEST_MODEL
    {
        public SALES_ORDER_ITEM_DETAIL data { get; set; }
    }

    public class SALES_ORDER_ITEM_DETAIL_REQUEST_LIST : REQUEST_MODEL
    {
        public List<SALES_ORDER_ITEM_DETAIL> data { get; set; }
    }

    public class SALES_ORDER_ITEM_DETAIL_RESULT : RESULT_MODEL
    {
        public List<SALES_ORDER_ITEM_DETAIL> data { get; set; }
    }
}