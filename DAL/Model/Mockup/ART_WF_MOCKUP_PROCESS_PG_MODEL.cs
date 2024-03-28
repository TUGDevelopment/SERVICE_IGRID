using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_MOCKUP_PROCESS_PG_2 : ART_WF_MOCKUP_PROCESS_PG
    {
        public string GRADE_OF_DISPLAY_TXT { get; set; }
        public string DI_CUT_DISPLAY_TXT { get; set; }
        public string VENDOR_DISPLAY_TXT { get; set; }
        public string FLUTE_DISPLAY_TXT { get; set; }

        //---------by aof 20220118 for CR sustain-- - start
        public string SUSTAIN_MATERIAL_DISPLAY_TXT { get; set; }
        public string PLASTIC_TYPE_DISPLAY_TXT { get; set; }
        public string CERT_SOURCE_DISPLAY_TXT { get; set; }
        //---------by aof 20220118 for CR sustain-- - end

        public ART_WF_MOCKUP_CHECK_LIST_2 CHECKLIST { get; set; }
        public ART_WF_MOCKUP_CHECK_LIST_PG CHECKLIST_PG { get; set; }
        //public List<ART_WF_MOCKUP_CHECK_LIST_ITEM_2> CHECKLIST_ITEM { get; set; }
        public SEARCH_DIE_LINE DIE_LINE { get; set; }

        public string BRAND_DISPLAY_TXT { get; set; }
        public string PRIMARY_TYPE_DISPLAY_TXT { get; set; }
        public string PRIMARY_SIZE_DISPLAY_TXT { get; set; }
        public string PACK_SIZE_DISPLAY_TXT { get; set; }
        public string STYLE_OF_PRINTING_DISPLAY_TXT { get; set; }

        public string PG_USER_DISPLAY_TXT { get; set; }
        public string SOLD_TO { get; set; }

       public string ARTWORK_SUB_ID { get; set; }

        public string IS_ONLOAD_PA { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_PG_2_METADATA
    {

    }

    public class ART_WF_MOCKUP_PROCESS_PG_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_PROCESS_PG_2 data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_PG_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_PG_2> data { get; set; }
    }
}
