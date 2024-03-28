using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class TRACKING_REPORT
    {
        public bool FIRST_LOAD { get; set; }
        #region "RESULT"
        public string REQUEST_NUMBER { get; set; }
        public string WORKFLOW_NUMBER { get; set; }
        public string CURRENT_STEP_DISPLAY_TXT { get; set; }
        public string BRAND_DISPLAY_TXT { get; set; }
        public string ADDITIONAL_BRAND_DISPLAY_TXT { get; set; }
        public string SOLD_TO_DISPLAY_TXT { get; set; }
        public string SHIP_TO_DISPLAY_TXT { get; set; }
        public string RDD_DISPLAY_TXT { get; set; }
        public Nullable<System.DateTime> RDD { get; set; }
        public string VENDOR_RFQ { get; set; }
        public string SELECTED_VENDOR { get; set; }
        public string IN_TRANSIT_TO_DISPLAY_TXT { get; set; }

        public string SALES_ORDER_NO { get; set; }
        public string SALES_ORDER_CREATE_DATE { get; set; }
        public string SALES_ORDER_ITEM { get; set; }
        public string SALES_ORDER_ITEM_COMPONENT { get; set; }
        public string PRODUCTION_MEMO_DISPLAY_TXT { get; set; }
        public string PORT { get; set; }

        public string PRIMARY_TYPE_DISPLAY_TXT { get; set; }
        public int MOCKUP_SUB_ID { get; set; }
        public int CHECK_LIST_ID { get; set; }
        public string PACKING_TYPE_DISPLAY_TXT { get; set; }
        public string CURRENT_STATUS_DISPLAY_TXT { get; set; }
        public string CURRENT_ASSIGN_DISPLAY_TXT { get; set; }
        public string DUEDATE_DISPLAY_TXT { get; set; }
        public string PRODUCT_CODE_DISPLAY_TXT { get; set; }
        public string RD_NUMBER_DISPLAY_TXT { get; set; }
        public string PRIMARY_SIZE_DISPLAY_TXT { get; set; }
        public string ROUTE_DESC_DISPLAY_TXT { get; set; }
        public string PIC_DISPLAY_TXT { get; set; }
        public string REQUEST_DATE_FROM { get; set; }
        public string REQUEST_DATE_TO { get; set; }
        public string PA_OWNER { get; set; }
        public string PG_OWNER { get; set; }
        public string CREATOR_DISPLAY_TXT { get; set; }
        public string MARKETING_NAME { get; set; }
        public string PLANT { get; set; }
        public string COUNTRY { get; set; }
        #endregion

        #region "REQUEST"
        public string WORKFLOW_NO { get; set; }
        public string WORKFLOW_NO_2 { get; set; }
        public string WORKFLOW_TYPE { get; set; }
        public string REQ_DATE_FROM { get; set; }
        public string REQ_DATE_TO { get; set; }
        public bool WORKFLOW_COMPLETED { get; set; }
        public bool WORKFLOW_OVERDUE { get; set; }
        public bool WORKFLOW_ACTION_BY_ME { get; set; }

        public int SOLD_TO_ID { get; set; }
        public int SHIP_TO_ID { get; set; }
        public int COUNTRY_ID { get; set; }
        public int BRAND_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public string PROJECT_NAME { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string RD_NUMBER { get; set; }

        public int PACKAGING_TYPE_ID { get; set; }

        public string PRIMARY_SIZE_TXT { get; set; }
        public string NET_WEIGHT_TXT { get; set; }


        public int CURRENT_STEP_ID { get; set; }
        public string CURRENT_STEP_WF_TYPE { get; set; }


        public int CREATOR_ID { get; set; }
        public int SUPERVISED_ID { get; set; }
        public int CURRENT_ASSIGN_ID { get; set; }
        public int WORKING_GROUP_ID { get; set; }
        public int CURRENT_USER_ID { get; set; }
        public string REF_WF_NO { get; set; }

        public string VIEW { get; set; }

        public string SEARCH_SO { get; set; }
        public string SEARCH_ORDER_BOM { get; set; }
        #endregion
        public string ZONE_TXT { get; set; }
        public string ALL_STEP_DISPLAY_TXT { get; set; }
        public string ALL_STEP_ASSIGN_DISPLAY_TXT { get; set; }
        public string ALL_STEP_START_DATE_DISPLAY_TXT { get; set; }
        public string ALL_STEP_END_DATE_DISPLAY_TXT { get; set; }
        public string ALL_STEP_DURATION_DISPLAY_TXT { get; set; }
        public string ALL_STEP_REASON_DISPLAY_TXT { get; set; }
        public string TOTAL_DAY_DISPLAY_TXT { get; set; }
        public string FILE_NAME { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_BY_DISPLAY_TXT { get; set; }
        public string NODE_ID_TXT { get; set; }
        public string TITLE { get; set; }
        public string GENERATE_EXCEL { get; set; }
        public string CURRENT_STEP_VALUE { get; set; }

        public string IS_STEP_DURATION_EXTEND { get; set; }

        public decimal? DURATION_EXTEND { get; set; }

        public decimal? DURATION { get; set; }

    }

    public class TRACKING_REPORT_REQUEST : REQUEST_MODEL
    {
        public TRACKING_REPORT data { get; set; }
    }

    public class TRACKING_REPORT_REQUEST_LIST : REQUEST_MODEL
    {
        public List<TRACKING_REPORT> data { get; set; }
    }

    public class TRACKING_REPORT_RESULT : RESULT_MODEL
    {
        public List<TRACKING_REPORT> data { get; set; }
    }
}
