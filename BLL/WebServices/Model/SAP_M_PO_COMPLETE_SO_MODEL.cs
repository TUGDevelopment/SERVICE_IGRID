using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Model;
using DAL;
using BLL.Services;
using System.Xml.Serialization;

namespace WebServices.Model
{

    public class SAP_M_PO_COMPLETE_SO_MODEL
    {
        public List<SO_HEADER> SO_HEADERS { get; set; }
    }

    public class SO_HEADER //: SAP_M_PO_COMPLETE_SO_HEADER
    {
        public int PO_COMPLETE_SO_HEADER_ID { get; set; }
        public string SALES_ORDER_NO { get; set; }
        public string SOLD_TO { get; set; }
        public string SOLD_TO_NAME { get; set; }
        public string LAST_SHIPMENT_DATE { get; set; }
        public string DATE_1_2 { get; set; }
        public string CREATE_ON { get; set; }
        public string RDD { get; set; }
        public string PAYMENT_TERM { get; set; }
        public string LC_NO { get; set; }
        public string EXPIRED_DATE { get; set; }
        public string SHIP_TO { get; set; }
        public string SHIP_TO_NAME { get; set; }
        public string SOLD_TO_PO { get; set; }
        public string SHIP_TO_PO { get; set; }
        public string SALES_GROUP { get; set; }
        public string MARKETING_CO { get; set; }
        public string MARKETING_CO_NAME { get; set; }
        public string MARKETING { get; set; }
        public string MARKETING_NAME { get; set; }
        public string MARKETING_ORDER_SAP { get; set; }
        public string MARKETING_ORDER_SAP_NAME { get; set; }
        public string SALES_ORG { get; set; }
        public string DISTRIBUTION_CHANNEL { get; set; }
        public string DIVITION { get; set; }
        public string SALES_ORDER_TYPE { get; set; }
        public string HEADER_CUSTOM_1 { get; set; }
        public string HEADER_CUSTOM_2 { get; set; }
        public string HEADER_CUSTOM_3 { get; set; }

        public List<SO_ITEM> SO_ITEMS { get; set; }
    }

    public class SO_ITEM
    {
        [XmlIgnore()]
        public int PO_COMPLETE_SO_HEADER_ID { get; set; }
        public string ITEM { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string MATERIAL_DESCRIPTION { get; set; }
        public string NET_WEIGHT { get; set; }
        public string ORDER_QTY { get; set; }
        public string ORDER_UNIT { get; set; }
        public string ETD_DATE_FROM { get; set; }
        public string ETD_DATE_TO { get; set; }
        public string PLANT { get; set; }
        public string OLD_MATERIAL_CODE { get; set; }
        public string PACK_SIZE { get; set; }
        public string VALUME_PER_UNIT { get; set; }
        public string VALUME_UNIT { get; set; }
        public string SIZE_DRAIN_WT { get; set; }
        public string PROD_INSP_MEMO { get; set; }
        public string REJECTION_CODE { get; set; }
        public string REJECTION_DESCRIPTION { get; set; }
        public string PORT { get; set; }
        public string VIA { get; set; }
        public string IN_TRANSIT_TO { get; set; }
        public string BRAND_ID { get; set; }
        public string BRAND_DESCRIPTION { get; set; }
        public string ADDITIONAL_BRAND_ID { get; set; }
        public string ADDITIONAL_BRAND_DESCRIPTION { get; set; }
        public string PRODUCTION_PLANT { get; set; }
        public string ZONE { get; set; }
        public string COUNTRY { get; set; }
        public string PRODUCTION_HIERARCHY { get; set; }
        public string MRP_CONTROLLER { get; set; }
        public string STOCK { get; set; }
        public string ITEM_CUSTOM_1 { get; set; }
        public string ITEM_CUSTOM_2 { get; set; }
        public string ITEM_CUSTOM_3 { get; set; }

         public List<COMPONENT> COMPONENTS { get; set; }
    }

    public class COMPONENT //: SAP_M_PO_COMPLETE_SO_ITEM
    {
        [XmlIgnore()]
        public int PO_COMPLETE_SO_ITEM_ID { get; set; }
        public string COMPONENT_ITEM { get; set; }
        public string COMPONENT_MATERIAL { get; set; }
        public string DECRIPTION { get; set; }
        public string QUANTITY { get; set; }
        public string UNIT { get; set; }
        public string STOCK { get; set; }
        public string BOM_ITEM_CUSTOM_1 { get; set; }
        public string BOM_ITEM_CUSTOM_2 { get; set; }
        public string BOM_ITEM_CUSTOM_3 { get; set; }
    }
}