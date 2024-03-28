using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_ARTWORK_PROCESS_CUSTOMER_2 : ART_WF_ARTWORK_PROCESS_CUSTOMER
    {
        public string CUSTOMER_DISPLAY_TXT { get; set; }
        public string SOLD_TO_PO { get; set; }
        public string SHIP_TO_PO { get; set; }
        public int MAIN_ARTWORK_SUB_ID { get; set; }
        public bool ENDTASKFORM { get; set; }
        public string ACTION_NAME { get; set; }
        public string COMMENT_BY_PA { get; set; }
        public string REASON_BY_PA { get; set; }
        public string REASON_BY_OTHER { get; set; }
        public string REMARK_REASON_BY_OTHER { get; set; }
        public string COMMENT_ADJUST_DISPLAY { get; set; }
        public string COMMENT_NONCOMPLIANCE_DISPLAY { get; set; }
        public string COMMENT_FORMLABEL_DISPLAY { get; set; }

        public string NUTRITION_COMMENT_DISPLAY { get; set; }
        public string INGREDIENTS_COMMENT_DISPLAY { get; set; }
        public string ANALYSIS_COMMENT_DISPLAY { get; set; }
        public string HEALTH_CLAIM_COMMENT_DISPLAY { get; set; }
        public string NUTRIENT_CLAIM_COMMENT_DISPLAY { get; set; }
        public string SPECIES_COMMENT_DISPLAY { get; set; }
        public string CATCHING_AREA_COMMENT_DISPLAY { get; set; }
        public string CHECK_DETAIL_COMMENT_DISPLAY { get; set; }
        public string QC_COMMENT { get; set; }

        public string DECISION_FORMLABEL_DISPLAY { get; set; }
        public string DECISION_NONCOMPLIANCE_DISPLAY { get; set; }
        public string DECISION_ADJUST_DISPLAY { get; set; }
        public string IS_FORMLABEL { get; set; }
        public string IS_CHANGEDETAIL { get; set; }
        public string IS_NONCOMPLIANCE { get; set; }
        public string IS_ADJUST { get; set; }

        public string DECISION_ACTION_DISPLAY { get; set; }
        public string APPROVE_SHADE_LIMIT_DISPLAY { get; set; }
        public string IS_REPEAT { get; set; }

        public int CURRENT_USER_ID { get; set; }


        public string SEND_TO_CUSTOMER_TYPE { get; set; }
        public string STEP_ARTWORK_CODE { get; set; }
        public string COURIER_NUMBER { get; set; }
        public DateTime CREATE_DATE_BY_PA { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_CUSTOMER_2_METADATA
    {

    }

    public class ART_WF_ARTWORK_PROCESS_CUSTOMER_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_CUSTOMER_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_CUSTOMER_2> data { get; set; }
    }
}
