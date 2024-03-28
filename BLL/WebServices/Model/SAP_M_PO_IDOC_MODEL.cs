using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Model;
using DAL;
using BLL.Services;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace WebServices.Model
{
    [Serializable]
    public class SAP_M_PO_IDOC_MODEL
    {
        [XmlArray("PO_IDOCS")]
        [XmlArrayItem(typeof(PO_IDOC), ElementName = "PO_IDOC")]
        public List<PO_IDOC> PO_IDOCS { get; set; }
    }

    //[Serializable]
    public class PO_IDOC
    {
        public string PurchaseOrderNo { get; set; }
        public string Currency { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedTime { get; set; }
        public string PurchasingOrg { get; set; }
        public string CompanyCode { get; set; }
        public string VendorNo { get; set; }
        public string VendorName { get; set; }
        public string Purchaser { get; set; }

        [XmlArray("ITEMS")]
        [XmlArrayItem(typeof(PO_IDOC_ITEM), ElementName = "ITEM")]
        public List<PO_IDOC_ITEM> ITEM { get; set; }
    }

    public class PO_IDOC_ITEM
    {
        public string ItemNoOfPO { get; set; }
        public string RecordType { get; set; }
        public string DeletionIndicator { get; set; }
        public string Quantity { get; set; }
        public string OrderUnit { get; set; }
        public string OrderPriceUnit { get; set; }
        public string NetOrderPrice { get; set; }
        public string Priceunit { get; set; }
        public string Amount { get; set; }
        public string MaterialGroup { get; set; }
        public string DenominatorQuantityConversion { get; set; }
        public string NumberatorQuantityConversion { get; set; }
        public string Plant { get; set; }
        public string MaterialNumber { get; set; }
        public string ShortText { get; set; }
        public string DeliveryDate { get; set; }
        public string SalesDocumentNo { get; set; }
        public string SalesDocumentItem { get; set; }
    }

}