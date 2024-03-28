using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class IGridSummary_REPORT
    {
        public int Id { get; set; }
        public string fn { get; set; }
        public string Title { get; set; }
        public string Condition { get; set; }
        public string RequestType { get; set; }
        public string DocumentNo { get; set; }
        public string DMSNo { get; set; }
        public string Material { get; set; }
        public string Description { get; set; }
        public string MaterialGroup { get; set; }
        public string Brand { get; set; }
        public string Assignee { get; set; }
        public string CreateOn { get; set; }
        public string CreateBy { get; set; }
        public string ActiveBy { get; set; }
        public string StatusApp { get; set; }
        public string FinalInfoGroup { get; set; }
        public string Remark { get; set; }
        public string Action { get; set; }
        public string ReferenceMaterial { get; set; }
        public string VendorCode { get; set; }
        public string VendorDescription { get; set; }
        public string where { get; set; }
        public string first_load { get; set; }
        public int seq { get; set; }
        public string export_excel { get; set; }
        public string Submitdate { get; set; }
    }


    public partial class IGridSummary_Group_REPORT
    {
  
        public string Condition { get; set; }
        public string RequestType { get; set; }
        public string DocumentNo { get; set; }
        public string DMSNo { get; set; }
        public string Material { get; set; }
        public string Description { get; set; }
        public string MaterialGroup { get; set; }
        public string Brand { get; set; }
        public string fn { get; set; }
        public string CreateOn { get; set; }
        public string ActiveBy { get; set; }
        public string StatusApp { get; set; }
        public string Submitdate { get; set; }


        public List<IGridSummary_REPORT> data { get; set; }
    }

    public class IGridSummary_REPORT_REQUEST : REQUEST_MODEL
    {
        public string name { get; set; }
        public string Keyword { get; set; }
        public IGridSummary_REPORT data { get; set; }
    }

    public class IGridSummary_REPORT_REQUEST_LIST : REQUEST_MODEL
    {
        public List<IGridSummary_REPORT> data { get; set; }
    }

    public class IGridSummary_REPORT_RESULT : RESULT_MODEL
    {
        public List<IGridSummary_REPORT> data { get; set; }
    }

    public class IGridSummary_Group_REPORT_RESULT : RESULT_MODEL
    {
        public List<IGridSummary_Group_REPORT> data { get; set; }
    }

}
