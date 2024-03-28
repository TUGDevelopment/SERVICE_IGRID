using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2 : ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK
    {
        public bool ENDTASKFORM { get; set; }
        public int ARTWORK_ITEM_ID { get; set; }
        public string ACTION_NAME { get; set; }
        public string ACTION_NAME_MK { get; set; }
        public string COMMENT_BY_MK { get; set; }
        public string REASON_BY_MK { get; set; }
        public string REASON_BY_OTHER { get; set; }
        public DateTime CREATE_DATE_BY_MK { get; set; }

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

        //GMQC
        public string COMMENT_GM_QC_DISPLAY { get; set; }
        public string APPROVAL_GM_QC_DISPLAY { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2_METADATA
    {

    }

    public class ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2> data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2> data { get; set; }
    }
}
