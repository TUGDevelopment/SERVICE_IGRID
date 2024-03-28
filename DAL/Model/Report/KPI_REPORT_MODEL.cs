using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class KPI_REPORT_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2> ALL_PRICE { get; set; }
        public string WF_NO { get; set; }
        public List<string> LIST_ALL_WF_NO { get; set; }
        public List<string> LIST_CORRECT_WF_NO { get; set; }
        public List<string> LIST_SENDBACK_WF_NO { get; set; }
        public int MOCKUP_ID { get; set; }

        public bool FIRST_LOAD { get; set; }
        public string GENERATE_EXCEL { get; set; }

        public int USERID { get; set; }

        public string KPI_TYPE { get; set; }

        public double TARGET { get; set; }

        public string DATE_TO { get; set; }
        public string DATE_FROM { get; set; }

        public string MONTH_START_DISPLAY_TEXT { get; set; }
        public string EMPLOYEE_ID_DISPLAY_TEXT { get; set; }
        public string EMPLOYEE_NAME_DISPLAY_TEXT { get; set; }
        public string POSITION_DISPLAY_TEXT { get; set; }

        public string Month1 { get; set; }
        public string Month2 { get; set; }
        public string Month3 { get; set; }
        public string Month4 { get; set; }
        public string Month5 { get; set; }
        public string Month6 { get; set; }
        public string Month7 { get; set; }
        public string Month8 { get; set; }
        public string Month9 { get; set; }
        public string Month10 { get; set; }
        public string Month11 { get; set; }
        public string Month12 { get; set; }

        public double SCORE1 { get; set; }
        public double SCORE2 { get; set; }
        public double SCORE3 { get; set; }
        public double SCORE4 { get; set; }
        public double SCORE5 { get; set; }

        public string AVG1 { get; set; }
        public string AVG2 { get; set; }
        public string GRADE1 { get; set; }
        public string GRADE2 { get; set; }
        public string MONTH_FROM { get; set; }

        public DateTime CREATE_DATE { get; set; }
        public DateTime UPDATE_DATE { get; set; }
    }

    public class KPI_REPORT_MODEL_REQUEST : REQUEST_MODEL
    {
        public KPI_REPORT_MODEL data { get; set; }
    }

    public class KPI_REPORT_MODEL_RESULT : RESULT_MODEL
    {
        
        public List<KPI_REPORT_MODEL> data { get; set; }
    }
}
