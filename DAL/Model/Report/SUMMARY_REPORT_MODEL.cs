using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class SUMMARY_REPORT_MODEL
    {
        public string CURRENT_STEP_DISPLAY_TEXT { get; set; }
        public string CURRENT_STEP_VALUE { get; set; }
        public string STEP_DISPLAY_TEXT { get; set; }
        public string WORKFLOW_TYPE { get; set; }
        public double INPROCESS_ONTIME { get; set; }
        public double INPROCESS_ALMOST_DUE { get; set; }
        public double INPROCESS_OVER_DUE { get; set; }
        public double COMPLETED_ONTIME { get; set; }
        public double COMPLETED_OVER_DUE { get; set; }
        public double ORDERBY { get; set; }
        public double COMPLETED_TOTAL { get; set; }
        public double COMPLETED_PERCEN { get; set; }
        public int? CURRENT_STEP_ID { get; set; }
        public string DEPARTMENT { get; set; }
        public string DEPARTMENT2 { get; set; }
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public string CURRENT_STEP_TXT { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public Nullable<System.DateTime> END_DATE { get; set; }
        public string WF_NO { get; set; }
        public Nullable<System.DateTime> REQUEST_CREATE_DATE { get; set; }
        public Nullable<System.DateTime> STEP_END_DATE { get; set; }
        public string IS_TERMINATE { get; set; }
        public string REMARK_KILLPROCESS { get; set; }
        public int? TOTAL_DAY { get; set; }
        public string CURRENT_STEP { get; set; }
        public string SO_NO { get; set; }
        public string SO_ITEM { get; set; }
        public string BRAND { get; set; }
        public string SOLD_TO { get; set; }
        public string SHIP_TO { get; set; }
        public string PACKAGING_TYPE { get; set; }
        public string PRIMARY_TYPE { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string ROUTE { get; set; }
        public string RDD { get; set; }
        public Nullable<System.DateTime> DUE_DATE { get; set; }
        public string CURRENT_ASSIGN { get; set; }
        public Nullable<decimal> DURATION { get; set; }
        public int ARTWORK_SUB_ID { get; set; }
        public int MOCKUP_SUB_ID { get; set; }

        public int ROWNUM { get; set; }
        public string SALES_ORDER_NO { get; set; }
        public string IS_END { get; set; }
        public string PA_NAME { get; set; }
        public string PG_NAME { get; set; }
        public string MARKETTING { get; set; }
        public string CUS_OR_VEN_DISPLAY_TXT { get; set; }
        public int WF_SUB_ID { get; set; }
        public string WF_TYPE { get; set; }
        public string WORKFLOW_STATUS { get; set; }
        public string REQUEST_TYPE { get; set; }
        public string EXTEND_DURATION { get; set; }
        public Nullable<decimal> DURATION_STANDARD { get; set; }
        public string LIST_WF_SUB_ID { get; set; }
        public string LIST_WF_SUB_ID_INPROCESS_ONTIME { get; set; }
        public string LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE { get; set; }
        public string LIST_WF_SUB_ID_INPROCESS_OVERDUE { get; set; }
        public string LIST_WF_SUB_ID_COMPLETED_ONTIME { get; set; }
        public string LIST_WF_SUB_ID_COMPLETED_OVERDUE { get; set; }
        public int CNT_WF { get; set; }
        public bool FIRST_LOAD { get; set; }
        public string LIST_WF_SUB_ID_ALL { get; set; }
        public double WF_TOTAL_INPROCESS_BY_WORKFLOW_TYPE { get; set; }
        public double WF_TOTAL_INPROCESS_BY_WORKFLOW_STEP { get; set; }
        public double WF_TOTAL_COMPLETE_BY_WORKFLOW_STEP { get; set; }
        public double WF_TOTAL { get; set; }
    }

    public class SUMMARY_REPORT_MODEL_REQUEST : REQUEST_MODEL
    {
        public SUMMARY_REPORT_MODEL data { get; set; }
    }

    public class SUMMARY_REPORT_MODEL_RESULT : RESULT_MODEL
    {
        public int ORDER_COLUMN { get; set; }



        public double WF_NEW_ONTIME { get; set; }
        public double WF_NEW_ALMOST_DUE { get; set; }
        public double WF_NEW_OVER_DUE { get; set; }

        public double WF_REPEAT_ONTIME { get; set; }
        public double WF_REPEAT_ALMOST_DUE { get; set; }
        public double WF_REPEAT_OVER_DUE { get; set; }

        public double WF_REPEATR6_ONTIME { get; set; }
        public double WF_REPEATR6_ALMOST_DUE { get; set; }
        public double WF_REPEATR6_OVER_DUE { get; set; }

        public double WF_MONORMAL_ONTIME { get; set; }
        public double WF_MONORMAL_ALMOST_DUE { get; set; }
        public double WF_MONORMAL_OVER_DUE { get; set; }

        public double WF_MODIELINE_ONTIME { get; set; }
        public double WF_MODIELINE_ALMOST_DUE { get; set; }
        public double WF_MODIELINE_OVER_DUE { get; set; }

        public double WF_MODESIGN_ONTIME { get; set; }
        public double WF_MODESIGN_ALMOST_DUE { get; set; }
        public double WF_MODESIGN_OVER_DUE { get; set; }


        public List<SUMMARY_REPORT_MODEL> data { get; set; }


        public double CNT_PK_ONTIME { get; set; }
        public double CNT_MK_ONTIME { get; set; }
        public double CNT_QC_ONTIME { get; set; }
        public double CNT_RD_ONTIME { get; set; }
        public double CNT_WH_ONTIME { get; set; }
        public double CNT_PN_ONTIME { get; set; }
        public double CNT_CUS_ONTIME { get; set; }
        public double CNT_VN_ONTIME { get; set; }

        public double CNT_PK_ALMOSTDUE { get; set; }
        public double CNT_MK_ALMOSTDUE { get; set; }
        public double CNT_QC_ALMOSTDUE { get; set; }
        public double CNT_RD_ALMOSTDUE { get; set; }
        public double CNT_WH_ALMOSTDUE { get; set; }
        public double CNT_PN_ALMOSTDUE { get; set; }
        public double CNT_CUS_ALMOSTDUE { get; set; }
        public double CNT_VN_ALMOSTDUE { get; set; }

        public double CNT_PK_OVERDUE { get; set; }
        public double CNT_MK_OVERDUE { get; set; }
        public double CNT_QC_OVERDUE { get; set; }
        public double CNT_RD_OVERDUE { get; set; }
        public double CNT_WH_OVERDUE { get; set; }
        public double CNT_PN_OVERDUE { get; set; }
        public double CNT_CUS_OVERDUE { get; set; }
        public double CNT_VN_OVERDUE { get; set; }
    }
}
