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
    public class ORDER_BOM_MODEL
    {
        [XmlArray("ORDER_BOMS")]
        [XmlArrayItem(typeof(ORDER_BOM), ElementName = "ORDER_BOM")]
        public List<ORDER_BOM> ORDER_BOMS { get; set; }
    }
    
 
    public class ORDER_BOM
    {
        public string ChangeType { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Counter { get; set; }
        public string MaterialFG { get; set; }
        public string SoldToParty { get; set; }
        public string ShipToParty { get; set; }
        public string BrandID { get; set; }
        public string AdditionalBrand { get; set; }
        public string Route { get; set; }
        public string IntransitPort { get; set; }
        public string SalesOrganization { get; set; }
        public string Plant { get; set; }
        public string MaterialNumber { get; set; }
        public string CountryKey { get; set; }
        public string PackagingQuantity { get; set; }
        public string PackagingUnit { get; set; }
        public string FGQuantity { get; set; }
        public string FGUnit { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string WastePercent { get; set; }
        public string CounterReference { get; set; }
    }


}