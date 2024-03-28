using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    public partial class V_ART_WF_DASHBOARD_2 : V_ART_WF_DASHBOARD
    {
        public string COUNTRY_CODE_SET { get; set; }
        public string PRODUCT_CODE_SET { get; set; }

        public string CREATE_BY_PROCESS_DISPLAY_TXT { get; set; }
        public string CREATE_BY_CHECK_LIST_DISPLAY_TXT { get; set; }

        public string PRIMARY_SIZE_DISPLAY_TXT { get; set; }
        public string PRIMARY_TYPE_DISPLAY_TXT { get; set; }


        public int CNT_TOTAL_SUB_WF { get; set; }
        public int CNT_TOTAL_SUB_WF_END { get; set; }
        public int CNT_TOTAL_SUB_WF_NOT_END { get; set; }
        public string WAITING_STEP { get; set; }
        public string END_STEP { get; set; }
        public string WORKFLOW_TYPE { get; set; }

        //FOR : Tracking Report
        public string CURRENT_ASSIGN_DISPLAY_TXT { get; set; }
        public string VENDOR_RFQ { get; set; }
        public string VENDOR_SELECTED { get; set; }

        public int USER_ID { get; set; }

        public string DUEDATE_DISPLAY_TXT { get; set; }

        public string DESTINATION { get; set; }
        public string REQUEST_ITEM_NO { get; set; }
        public string REQUEST_FORM_NO { get; set; }
        public string PA_PG_DISPLAY_TXT { get; set; }

        public Nullable<System.DateTime> LAST_UPDATE_DATE_WF { get; set; }
        public string PLANT { get; set; }
        public string CURRENT_ASSIGN { get; set; }
        public string GET_BY_CREATE_DATE_FROM { get; set; }
        public string GET_BY_CREATE_DATE_TO { get; set; }
        public List<int> LIST_ARTWORK_SUB_ID { get; set; }


        public string PRODUCT_CODE { get; set; }
        public string SO_AND_ITEM_NO { get; set; }
        public string MAT5 { get; set; }
        public string SALES_ORG { get; set; }

        public bool PP_SEND_BACK { get; set; }
        public string PP_SEND_BACK_COMMENT { get; set; }
        public bool SENT_PP { get; set; }

        public string WF_STATUS { get; set; }  //#TSK-1511 #SR-70695 by aof in 09/2022

    }

    public class V_ART_WF_DASHBOARD_REQUEST : REQUEST_MODEL
    {
        public V_ART_WF_DASHBOARD_2 data { get; set; }
    }

    public class V_ART_WF_DASHBOARD_RESULT : RESULT_MODEL
    {
        public List<V_ART_WF_DASHBOARD_2> data { get; set; }
    }
}