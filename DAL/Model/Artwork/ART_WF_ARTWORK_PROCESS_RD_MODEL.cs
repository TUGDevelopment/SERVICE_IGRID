using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_PROCESS_RD_2 : ART_WF_ARTWORK_PROCESS_RD
    {
        public bool ENDTASKFORM { get; set; }
        public string ACTION_NAME { get; set; }
        public string COMMENT_BY_PA { get; set; }
        public string REASON_BY_PA { get; set; }
        public string REASON_BY_OTHER { get; set; }
        public string REMARK_REASON_BY_PA { get; set; }
        public string REMARK_REASON_BY_OTHER { get; set; }
        public int ARTWORK_SUB_ID_RD { get; set; }
        public int PARENT_RD_ID { get; set; }
        public DateTime CREATE_DATE_BY_PA { get; set; }

        public string NUTRITION_DISPLAY_TXT { get; set; }
        public string HEALTH_CLAIM_DISPLAY_TXT { get; set; }
        public string CATCHING_AREA_DISPLAY_TXT { get; set; }
        public string INGREDIENTS_DISPLAY_TXT { get; set; }
        public string NUTRIENT_CLAIM_DISPLAY_TXT { get; set; }
        public string ANALYSIS_DISPLAY_TXT { get; set; }
        public string SPECIES_DISPLAY_TXT { get; set; }
        public string CHECK_DETAIL_DISPLAY_TXT { get; set; }
        public string IS_CONFIRED_DISPLAY_TXT { get; set; }

        public string NUTRITION_RD_DISPLAY_TXT { get; set; }
        public string HEALTH_CLAIM_RD_DISPLAY_TXT { get; set; }
        public string CATCHING_AREA_RD_DISPLAY_TXT { get; set; }
        public string INGREDIENTS_RD_DISPLAY_TXT { get; set; }
        public string NUTRIENT_CLAIM_RD_DISPLAY_TXT { get; set; }
        public string ANALYSIS_RD_DISPLAY_TXT { get; set; }
        public string SPECIES_RD_DISPLAY_TXT { get; set; }
        public string CHECK_DETAIL_RD_DISPLAY_TXT { get; set; }
        public string IS_CONFIRED_RD_DISPLAY_TXT { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_RD_2_METADATA
    {
         
    }

    public class ART_WF_ARTWORK_PROCESS_RD_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_RD_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_RD_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_RD_2> data { get; set; }
    }
}
