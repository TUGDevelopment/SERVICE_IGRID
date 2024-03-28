using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class HISTORY_REPORT
    {
        public int Tab_Id { get; set; }
        public int Log_Id { get; set; }
        public string Log_PA_ModifyBy { get; set; }
        public string Log_PA_ModifyByFirstName { get; set; }
        public string Log_PA_ModifyByfn { get; set; }
        public string Log_PA_ModifyOn { get; set; }

        public string Log_PG_ModifyBy { get; set; }
        public string Log_PG_ModifyByFirstName { get; set; }
        public string Log_PG_ModifyByfn { get; set; }
        public string Log_PG_ModifyOn { get; set; }


        public string Material { get; set; }
        public string Description { get; set; }
        public string CreateBy { get; set; }
        public string CreateByFirstName { get; set; }
        public string CreateByfn { get; set; }
        public string CreateOn { get; set; }
        public string Final_ApprovedDate { get; set; }
        public string Final_ApprovedBy { get; set; }
        public string Final_ApprovedName { get; set; }
        public string StatusApp { get; set; }
        public string Condition { get; set; }
        public string Old_Brand { get; set; }
        public string Brand { get; set; }
        public string Old_ChangePoint { get; set; }
        public string ChangePoint { get; set; }
        public string Old_MaterialGroup { get; set; }
        public string MaterialGroup { get; set; }
        public string Old_PrimarySize { get; set; }
        public string PrimarySize { get; set; }
        public string Old_ContainerType { get; set; }
        public string ContainerType { get; set; }
        public string Old_LidType { get; set; }
        public string LidType { get; set; }
        public string Old_PackingStyle { get; set; }
        public string PackingStyle { get; set; }
        public string Old_Packing { get; set; }
        public string Packing { get; set; }
        public string Old_StyleofPrinting { get; set; }
        public string StyleofPrinting { get; set; }
        public string Old_ProductCode { get; set; }
        public string ProductCode { get; set; }
        public string Old_FAOZone { get; set; }
        public string FAOZone { get; set; }
        public string Old_Plant { get; set; }
        public string Plant { get; set; }
        public string Old_PMScolour { get; set; }
        public string PMScolour { get; set; }
        public string Old_Processcolour { get; set; }
        public string Processcolour { get; set; }
        public string Old_Totalcolour { get; set; }
        public string Totalcolour { get; set; }
        public string Old_PlantRegisteredNo { get; set; }
        public string PlantRegisteredNo { get; set; }
        public string Old_CompanyNameAddress { get; set; }
        public string CompanyNameAddress { get; set; }
        public string Old_Symbol { get; set; }
        public string Symbol { get; set; }
        public string Old_CatchingArea { get; set; }
        public string CatchingArea { get; set; }
        public string Old_CatchingPeriodDate { get; set; }
        public string CatchingPeriodDate { get; set; }
        public string Old_PrintingStyleofPrimary { get; set; }
        public string PrintingStyleofPrimary { get; set; }
        public string Old_PrintingStyleofSecondary { get; set; }
        public string PrintingStyleofSecondary { get; set; }
        public string Old_Typeof { get; set; }
        public string Typeof { get; set; }
        public string Old_TypeofCarton2 { get; set; }
        public string TypeofCarton2 { get; set; }
        public string Old_DMSNo { get; set; }
        public string DMSNo { get; set; }
        public string Old_TypeofPrimary { get; set; }
        public string TypeofPrimary { get; set; }
        public string Old_Direction { get; set; }
        public string Direction { get; set; }
        public string Old_PlantAddress { get; set; }
        public string PlantAddress { get; set; }
        public string Old_Catching_Method { get; set; }
        public string Catching_Method { get; set; }
        public string Old_Scientific_Name { get; set; }
        public string Scientific_Name { get; set; }
        public string Old_Specie { get; set; }
        public string Specie { get; set; }
        public string Refnumber { get; set; }
        public string Remark { get; set; }
        public string Note { get; set; }
        public int CountTotal { get; set; }

        //------------------------------------------PG
        public string Assignee { get; set; }
        public string AssigneeFirstName { get; set; }
        public string AssigneeLastName { get; set; }


        public string Old_Grandof { get; set; }
        public string Grandof { get; set; }
        public string Old_SheetSize { get; set; }
        public string SheetSize { get; set; }
        public string Old_Vendor { get; set; }
        public string Vendor { get; set; }
        public string Old_Flute { get; set; }
        public string Flute { get; set; }
        public string Old_Dimension { get; set; }
        public string Dimension { get; set; }
        public string Old_RSC { get; set; }
        public string RSC { get; set; }
        public string Old_Accessories { get; set; }
        public string Accessories { get; set; }
        public string Old_PrintingSystem { get; set; }
        public string PrintingSystem { get; set; }
        public string Old_RollSheet { get; set; }
        public string RollSheet { get; set; }
      



        //-----------------------------------Filter
        public string LayOut { get; set; }
        public string FrDt { get; set; }
        public string ToDt { get; set; }
        public string first_load { get; set; }

    }
    public class HISTORY_REPORT_REQUEST : REQUEST_MODEL
    {
        public string name { get; set; }
        public string Keyword { get; set; }
        public HISTORY_REPORT data { get; set; }
    }

    public class HISTORY_REPORT_REQUEST_LIST : REQUEST_MODEL
    {
        public List<HISTORY_REPORT> data { get; set; }
    }

    public class HISTORY_REPORT_RESULT : RESULT_MODEL
    {
        public List<HISTORY_REPORT> data { get; set; }
    }
}
