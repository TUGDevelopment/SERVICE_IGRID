using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ImpactedMatDesc_REPORT
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string Changed_By { get; set; }
        public string Changed_On { get; set; }
        public string Changed_Tabname { get; set; }
        public string Changed_Action { get; set; }
        public string Char_NewValue { get; set; }
        public string Char_Description { get; set; }
        public string DMSNo { get; set; }
        public string New_Material { get; set; }
        public string New_Description { get; set; }
        public string NewMat_JobId { get; set; }
        public string Old_Id { get; set; }
        public string Old_Description { get; set; }
        public string Material { get; set; }
        public string User { get; set; }
        public string Description { get; set; }
        public string MasterName { get; set; }
        public string Action { get; set; }
        public string FrDt { get; set; }
        public string ToDt { get; set; }
        public string first_load { get; set; }
        public string Keyword { get; set; }
    }
    public class ImpactedMatDesc_REPORT_REQUEST : REQUEST_MODEL
    {
        public string name { get; set; }
        public string Keyword { get; set; }
        public ImpactedMatDesc_REPORT data { get; set; }
    }

    public class ImpactedMatDesc_REPORT_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ImpactedMatDesc_REPORT> data { get; set; }
    }

    public class ImpactedMatDesc_REPORT_RESULT : RESULT_MODEL
    {
        public List<ImpactedMatDesc_REPORT> data { get; set; }
    }
}
