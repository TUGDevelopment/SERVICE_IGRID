using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2 : ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE
    {
        public string COMMENT_BY_PG_SUP { get; set; }
        public string COMMENT_BY_VENDOR { get; set; }
        public string USER_DISPLAY_TXT { get; set; }
        public string VENDOR_DISPLAY_TXT { get; set; }

        public string FINAL_INFO { get; set; }
        public string WF_NO { get; set; }
        public int ROUND { get; set; }

        public int CURRENT_USER_ID { get; set; }
        public int CURRENT_VENDOR_ID { get; set; }
        public int CURRENT_STEP_ID { get; set; }
        public string IS_MANUAL { get; set; }

        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }

        public bool FIRST_LOAD { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2_METADATA
    {

    }

    public class ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2 data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST_LIST : REQUEST_MODEL
    {
        public int UPDATE_BY { get; set; }
        public int CREATE_BY { get; set; }
        public int MOCKUP_ID { get; set; }
        public int MOCKUP_SUB_ID { get; set; }
        public string COMMENT_BY_VENDOR { get; set; }
        public bool ENDTASKFORM { get; set; }
        public List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2> data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2> data { get; set; }
    }
}
