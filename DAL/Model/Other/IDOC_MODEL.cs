using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class IDOC_MODEL
    {
        public int PO_IDOC_ID { get; set; }
        public string PURCHASE_ORDER_NO { get; set; }
        public string CURRENCY { get; set; }
        public string DATE { get; set; }
        public string TIME { get; set; }
        public string PURCHASING_ORG { get; set; }
        public string COMPANY_CODE { get; set; }
        public string VENDOR_NO { get; set; }
        public string VENDOR_NAME { get; set; }
        public string PURCHASER { get; set; }

        public int PO_IDOC_ITEM_ID { get; set; }
        public string PO_ITEM_NO { get; set; }
        public string RECORD_TYPE { get; set; }
        public string DELETION_INDICATOR { get; set; }
        public string QUANTITY { get; set; }
        public string ORDER_UNIT { get; set; }
        public string ORDER_PRICE_UNIT { get; set; }
        public string NET_ORDER_PRICE { get; set; }
        public string PRICE_UNIT { get; set; }
        public string AMOUNT { get; set; }
        public string MATERIAL_GROUP { get; set; }
        public Nullable<decimal> DENOMINATOR_QUANTITY_CONVERSION { get; set; }
        public Nullable<decimal> NUMERATOR_QUANTITY_CONVERSION { get; set; }
        public string PLANT { get; set; }
        public string METERIAL_NUMBER { get; set; }
        public string SHORT_TEXT { get; set; }
        public string DELIVERY_DATE { get; set; }
        public string SALES_DOCUMENT_NO { get; set; }
        public Nullable<decimal> SALES_DOCUMENT_ITEM { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public int CREATE_BY { get; set; }
        public System.DateTime UPDATE_DATE { get; set; }
        public int UPDATE_BY { get; set; }

    }

    public class IDOC_METADATA
    {

    }

    public class IDOC_REQUEST : REQUEST_MODEL
    {
        public IDOC_MODEL data { get; set; }
    }

    public class IDOC_RESULT : RESULT_MODEL
    {
        public List<IDOC_MODEL> data { get; set; }
    }
}
