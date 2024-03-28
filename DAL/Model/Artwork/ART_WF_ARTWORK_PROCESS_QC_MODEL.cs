using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_PROCESS_QC_2 : ART_WF_ARTWORK_PROCESS_QC
    {
        public bool ENDTASKFORM { get; set; }
        public string ACTION_NAME { get; set; }
        public string COMMENT_BY_PA { get; set; }
        public string REASON_BY_PA { get; set; }
        public string REASON_BY_OTHER { get; set; }
        public string REMARK_REASON_BY_PA { get; set; }
        public string REMARK_REASON_BY_OTHER { get; set; }
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

        public string NUTRITION_QC_DISPLAY_TXT { get; set; }
        public string HEALTH_CLAIM_QC_DISPLAY_TXT { get; set; }
        public string CATCHING_AREA_QC_DISPLAY_TXT { get; set; }
        public string INGREDIENTS_QC_DISPLAY_TXT { get; set; }
        public string NUTRIENT_CLAIM_QC_DISPLAY_TXT { get; set; }
        public string ANALYSIS_QC_DISPLAY_TXT { get; set; }
        public string SPECIES_QC_DISPLAY_TXT { get; set; }
        public string CHECK_QC_DETAIL_DISPLAY_TXT { get; set; }


        // ticket 462433 by aof start added field
        public string DEFALUT_QC_COMMENT { get; set; } 
        public string DEFALUT_QC_IS_CONFIRED { get; set; }
        public string DEFALUT_QC_NUTRITION { get; set; }
        public string DEFALUT_QC_NUTRITION_COMMENT { get; set; }
        public string DEFALUT_QC_INGREDIENTS { get; set; }
        public string DEFALUT_QC_INGREDIENTS_COMMENT { get; set; }
        public string DEFALUT_QC_ANALYSIS { get; set; }
        public string DEFALUT_QC_ANALYSIS_COMMENT { get; set; }
        public string DEFALUT_QC_HEALTH_CLAIM { get; set; }
        public string DEFALUT_QC_HEALTH_CLAIM_COMMENT { get; set; }
        public string DEFALUT_QC_NUTRIENT_CLAIM { get; set; }
        public string DEFALUT_QC_NUTRIENT_CLAIM_COMMENT { get; set; }

        public string DEFALUT_QC_SPECIES { get; set; }
        public string DEFALUT_QC_SPECIES_COMMENT { get; set; }
        public string DEFALUT_QC_CATCHING_AREA { get; set; }
        public string DEFALUT_QC_CATCHING_AREA_COMMENT { get; set; }
    
        public string DEFALUT_QC_CHECK_DETAIL { get; set; }
        public string DEFALUT_QC_CHECK_DETAIL_COMMENT { get; set; }

       // ticket 462433 by aof finish added field

    }

    public class ART_WF_ARTWORK_PROCESS_QC_2_METADATA
    {
         
    }

    public class ART_WF_ARTWORK_PROCESS_QC_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_QC_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_QC_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_QC_2> data { get; set; }
    }
}
