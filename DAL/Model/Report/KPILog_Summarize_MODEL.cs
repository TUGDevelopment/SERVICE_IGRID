using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class KPILog_Summarize_REPORT
    {
        public string Log_PA_ModifyBy { get; set; }
        public string Log_PA_ModifyByFullName { get; set; }
        public string Log_PG_ModifyBy { get; set; }
        public string Log_PG_ModifyByFullName { get; set; }
        public string CreateBy { get; set; }
        public string CreateByFullName { get; set; }
        public string Assignee { get; set; }
        public string AssigneeFullName { get; set; }
        public int Count { get; set; }
        public int SAPMat_Count { get; set; }
        public int Count_All { get; set; }
        public string LayOut { get; set; }
        public string FrDt { get; set; }
        public string ToDt { get; set; }
        public string first_load { get; set; }


        public string CREATE_USERNAME { get; set; }
        public string CREATE_FULLNAME { get; set; }
        public string MODIFY_BY_USERNAME { get; set; }
        public string MODIFY_BY_FULLNAME { get; set; }
        public int MODIFYED_RECOORD { get; set; }
        //public string CREATED_RECOORD { get; set; }
        //public string PECENTAGE_ERROR { get; set; }

        public string EXPORT_EXCEL { get; set; }
    }



    public class KPILog_SummarizeGroup_Report
    {
        public string CREATE_USERNAME { get; set; }
        public string CREATE_FULLNAME { get; set; }
        public string MODIFY_BY_USERNAME { get; set; }
        public string MODIFY_BY_FULLNAME { get; set; }
        public int MODIFYED_RECOORD { get; set; }
        public int CREATED_RECOORD { get; set; }
        public string PECENTAGE_ERROR { get; set; }

        public List<KPILog_Summarize_REPORT> data { get; set; }
    }


    public class KPILog_Summarize_REPORT_REQUEST : REQUEST_MODEL
    {
     
        public string name { get; set; }
        public string Keyword { get; set; }
        public KPILog_Summarize_REPORT data { get; set; }
    }

    public class KPILog_Summarize_REPORT_REQUEST_LIST : REQUEST_MODEL
    {
        public List<KPILog_Summarize_REPORT> data { get; set; }
    }

    public class KPILog_Summarize_REPORT_RESULT : RESULT_MODEL
    {
        public List<KPILog_Summarize_REPORT> data { get; set; }
    }

    public class KPILog_SummarizeGroup_Report_RESULT : RESULT_MODEL
    {
        public List<KPILog_SummarizeGroup_Report> data { get; set; }
    }

}
