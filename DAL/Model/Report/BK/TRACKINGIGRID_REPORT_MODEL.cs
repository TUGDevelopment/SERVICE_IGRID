using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
   public partial class TRACKINGIGRID_REPORT
    {
        public int ID { get; set; }
        public string DocumentNo { get; set; }
        public string Material { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        public string MaterialGroup { get; set; }
        public string PrimarySize { get; set; }
        public string Version { get; set; }
        public string ChangePoint { get; set; }
        public string SheetSize { get; set; }
        public string Assignee { get; set; }
        public string PackingStyle { get; set; }
        public string Packing { get; set; }
        public string StyleofPrinting { get; set; }
        public string ContainerType { get; set; }
        public string LidType { get; set; }
        public string Condition { get; set; }
        public string ProductCode { get; set; }
        public string FAOZone { get; set; }
        public string Plant { get; set; }
        public string Totalcolour { get; set; }
        public string Processcolour { get; set; }
        public string PlantRegisteredNo { get; set; }
        public string CompanyNameAddress { get; set; }
        public string PMScolour { get; set; }
        public string Symbol { get; set; }
        public string CatchingArea { get; set; }
        public string CatchingPeriodDate { get; set; }
        public string Grandof { get; set; }
        public string Flute { get; set; }
        public string Vendor { get; set; }
        public string Dimension { get; set; }
        public string RSC { get; set; }
        public string Accessories { get; set; }
        public string PrintingStyleofPrimary { get; set; }
        public string PrintingStyleofSecondary { get; set; }
        public string CustomerDesign { get; set; }
        public string CustomerSpec { get; set; }
        public string CustomerSize { get; set; }
        public string CustomerVendor { get; set; }
        public string CustomerColor { get; set; }
        public string CustomerScanable { get; set; }
        public string CustomerBarcodeSpec { get; set; }
        public string FirstInfoGroup { get; set; }
        public string SO { get; set; }
        public string PICMkt { get; set; }
        public string SOPlant { get; set; }
        public string Destination { get; set; }
        public string Remark { get; set; }
        public string GrossWeight { get; set; }
        public string FinalInfoGroup { get; set; }
        public string Note { get; set; }
        public string Typeof { get; set; }
        public string TypeofCarton2 { get; set; }
        public string DMSNo { get; set; }
        public string TypeofPrimary { get; set; }
        public string PrintingSystem { get; set; }
        public string Direction { get; set; }
        public string RollSheet { get; set; }
        public string RequestType { get; set; }
        public string PlantAddress { get; set; }
        public string Status_upd { get; set; }
        public string Role { get; set; }
        public string By { get; set; }
        public string Status { get; set; }
        public DateTime? FrDt { get; set; }
        public DateTime? ToDt { get; set; }
        public string Keyword { get; set; }

    }
    public class TRACKINGIGRID_REPORT_REQUEST : REQUEST_MODEL
    {
        public string name { get; set; }
        
        public string Keyword { get; set; }
        public TRACKINGIGRID_REPORT data { get; set; }
    }

    public class TRACKINGIGRID_REPORT_REQUEST_LIST : REQUEST_MODEL
    {
        public List<TRACKINGIGRID_REPORT> data { get; set; }
    }

    public class TRACKINGIGRID_REPORT_RESULT : RESULT_MODEL
    {
        public List<TRACKINGIGRID_REPORT> data { get; set; }
    }
}
