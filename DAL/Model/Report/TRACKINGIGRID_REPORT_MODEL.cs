using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public partial class TRANSAPPROVE_MODEL
    {
       public int ID { get; set; }
        public int MatDoc { get; set; }
        public int StatusApp { get; set; }
        public int Condition { get; set; }
        public string fn { get; set; }
        public string ActiveBy { get; set; }
        public DateTime SubmitDate { get; set; }
        public int levelApp { get; set; }
    }

    public partial class TRACKINGIGRID_REPORT
    {
        public int ID { get; set; }
        public string DocumentNo { get; set; }
        public string Material { get; set; }
        public string Description { get; set; }
        public string Status_upd { get; set; }
        public string PA_InputBy { get; set; }
        public string PA_Actdate { get; set; }
        public string PA_SubmitDate { get; set; }
        public string PA_InputDate { get; set; }
        public int PAStart_to_PA { get; set; }
        public string PA_ApproveBy { get; set; }
        public string PA_ApproveDate { get; set; }
        public int PA_to_PAApprove { get; set; }
        public string PG_AssignTo { get; set; }
        public string PG_AssignDate { get; set; }
        public int PA_Submit_to_PGAssign { get; set; }
        public string PG_InputBy { get; set; }
        public string PG_InputDate { get; set; }
        public int PGAssign_to_PGInput { get; set; }
        public string PG_ApproveBy { get; set; }
        public string PG_ApproveDate { get; set; }
        public int PGInput_to_PGApprove { get; set; }
        public string Final_ApprovedBy { get; set; }
        public string Final_ApprovedName { get; set; }
        public string Final_ApprovedDate { get; set; }
        public int PA_to_FinalApprove { get; set; }
        public string InfoGroupBy { get; set; }
        public string InfoGroupDate { get; set; }
        public string Brand { get; set; }
        public string MaterialGroup { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string ChangePoint { get; set; }
        public string SheetSize { get; set; }
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

        public int Completed_Record { get; set; }
        public int Canceled_Record { get; set; }
        public int InProcess_Record { get; set; }
        public int Failed_Record { get; set; }
        public int Total_Record { get; set; }
        public string Role { get; set; }
        public string By { get; set; }
        public string Status { get; set; }
        public string FrDt { get; set; }
        public string ToDt { get; set; }
        public string Keyword { get; set; }
        public string first_load { get; set; }

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

        public int Completed_Record { get; set; }
        public int Failed_Record { get; set; }
        public int Canceled_Record { get; set; }
        public int InProcess_Record { get; set; }
      
    }
}
