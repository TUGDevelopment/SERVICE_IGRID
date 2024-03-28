using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2 : ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC
    {
        public ART_WF_ARTWORK_PROCESS_2 PROCESS { get; set; }
        public int ARTWORK_SUB_ID2 { get; set; }
        public int ARTWORK_ITEM_ID { get; set; }
        public bool ENDTASKFORM { get; set; }
        public string ACTION_NAME { get; set; }
        public string COMMENT_BY_PA { get; set; }
        public string REASON_BY_PA { get; set; }
        public string REASON_BY_OTHER { get; set; }
        public string PREV_STEP_DISPLAY { get; set; }
        public string SEND_TO_CUSTOMER_TYPE { get; set; }
        public DateTime CREATE_DATE_BY_PA { get; set; }

        //PA Side
        public string COMMENT_CHANGE_DETAIL { get; set; }
        public string COMMENT_NONCOMPLIANCE { get; set; }
        public string COMMENT_ADJUST { get; set; }

        public string IS_FORMLABEL { get; set; }
        public string IS_CHANGEDETAIL { get; set; }
        public string IS_NONCOMPLIANCE { get; set; }
        public string IS_ADJUST { get; set; }

        public string NUTRITION_COMMENT_DISPLAY { get; set; }
        public string INGREDIENTS_COMMENT_DISPLAY { get; set; }
        public string ANALYSIS_COMMENT_DISPLAY { get; set; }
        public string HEALTH_CLAIM_COMMENT_DISPLAY { get; set; }
        public string NUTRIENT_CLAIM_COMMENT_DISPLAY { get; set; }
        public string SPECIES_COMMENT_DISPLAY { get; set; }
        public string CATCHING_AREA_COMMENT_DISPLAY { get; set; }
        public string CHECK_DETAIL_COMMENT_DISPLAY { get; set; }
        public string QC_COMMENT { get; set; }
        //CUS Side
        public string DECISION_FORMLABEL_DISPLAY { get; set; }
        public string DECISION_NONCOMPLIANCE_DISPLAY { get; set; }
        public string DECISION_ADJUST_DISPLAY { get; set; }
        public string COMMENT_ADJUST_DISPLAY { get; set; }
        public string COMMENT_NONCOMPLIANCE_DISPLAY { get; set; }
        public string COMMENT_FORMLABEL_DISPLAY { get; set; }
        public string IS_CHECK_VERIFY { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2_METADATA
    {

    }

    public class ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2> data { get; set; }
    }
}
