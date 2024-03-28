using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    public class V_ART_ENDTOEND_REPORT_2 : V_ART_ENDTOEND_REPORT
    {
        public string WORKFLOW_NO { get; set; }
        public string WORKFLOW_TYPE { get; set; }
        public string WORKFLOW_NO_2 { get; set; }
        public string REQUEST_DATE_FROM { get; set; }
        public string REQUEST_DATE_TO { get; set; }
        public bool WORKFLOW_COMPLETED { get; set; }
        public bool WORKFLOW_IN_PROCESS { get; set; }
        public string SEARCH_SO { get; set; }
        public string SEARCH_ORDER_BOM { get; set; }
        public string ZONE_TXT { get; set; }
        public Nullable<int> COUNTRY_ID { get; set; }
        public Nullable<int> PACKAGING_TYPE_ID { get; set; }
        public Nullable<int> WORKING_GROUP_ID { get; set; }
        public string PRIMARY_SIZE_TXT { get; set; }
        public string RD_NUMBER { get; set; }
        public string NET_WEIGHT_TXT { get; set; }
        public string CURRENT_STEP_WF_TYPE { get; set; }
        public Nullable<int> SUPERVISED_ID { get; set; }
        public Nullable<int> CURRENT_ASSIGN_ID { get; set; }
        public bool WORKFLOW_OVERDUE { get; set; }
        public bool workflow_process { get; set; }
        public bool workflow_terminated { get; set; }
        public string REF_WF_NO { get; set; }
        public string GENERATE_EXCEL { get; set; }
        public string SO_CREATE_DATE_FROM { get; set; }
        public string SO_CREATE_DATE_TO { get; set; }
        public Nullable<decimal> CUSTOMER_APPROVE_FROM { get; set; }
        public Nullable<decimal> CUSTOMER_APPROVE_TO { get; set; }
        public Nullable<decimal> END_TO_END_FROM { get; set; }
        public Nullable<decimal> END_TO_END_TO { get; set; }
        public bool FIRST_LOAD { get; set; }
        public bool WORKFLOW_ACTION_BY_ME { get; set; }
        public System.DateTime STEP_CREATE_DATE_ORDERBY { get; set; }
        public string VIEW { get; set; }
        public string VENDOR_RFQ { get; set; }
        public string SELECTED_VENDOR { get; set; }
        public string STEP_DATE_FROM { get; set; }
        public string STEP_DATE_TO { get; set; }
        public Nullable<System.DateTime> STEP_END_DATE_STATIC { get; set; }
        //public string TypeWorkflow { get; set; }
        public Nullable<decimal> TOTALDAY { get; set; }
        public Nullable<decimal> USEDAY { get; set; }
        public string RECEIVER_REASON { get; set; }   //---CR#19743 by aof 
        public string RECEIVER_COMMENT { get; set; }  //---CR#19743 by aof
        public string TERMINATE_REASON { get; set; }  //---#INC-55439 by aof
        public string TERMINATE_COMMENT { get; set; }  //---#INC-55439 by aof
    }

    public class V_ART_ENDTOEND_REPORT_REQUEST : REQUEST_MODEL
    {
        public V_ART_ENDTOEND_REPORT_2 data { get; set; }
    }

    public class V_ART_ENDTOEND_REPORT_RESULT : RESULT_MODEL
    {
        public int ORDER_COLUMN { get; set; }
        public List<V_ART_ENDTOEND_REPORT_2> data { get; set; }
        public List<V_ART_ENDTOEND_REPORT_2> dataExcel { get; set; }
    }

    public class V_ART_ENDTOEND_REPORT_3 : V_ART_ENDTOEND_REPORT
    {

    }
}