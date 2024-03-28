using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_MOCKUP_PROCESS_CUSTOMER_2 : ART_WF_MOCKUP_PROCESS_CUSTOMER
    {
         //public ART_WF_MOCKUP_CHECK_LIST_2 CHECKLIST_DATA { get; set; }
         //public ART_WF_MOCKUP_CHECK_LIST_ITEM_2 CHECKLIST_ITEM_DATA { get; set; }
        public bool ENDTASKFORM { get; set; }
        public string COMMENT_BY_PG { get; set; }
        public string REASON_BY_PG { get; set; }
        public string PACKING_STYLE { get; set; }
        public string PURPOSE_OF { get; set; }
        public string TOPIC_DISPLAY_TXT { get; set; }
        public string ACTION_NAME { get; set; }
        public string DECISION_DISPLAY_TXT { get; set; }
        public string DECISION_DISPLAY_TXT2 { get; set; }
        public string CUSTOMER_DISPLAY_TXT { get; set; }
        public string isProjectNoCus { get; set; }
        public DateTime CREATE_DATE_BY_PG { get; set; }
        public string REMARK_REASON_BY_PG { get; set; }
        public int CURRENT_USER_ID { get; set; }
    }
    
    public class ART_WF_MOCKUP_PROCESS_CUSTOMER_2_METADATA
    {

    }

    public class ART_WF_MOCKUP_PROCESS_CUSTOMER_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_PROCESS_CUSTOMER_2 data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_CUSTOMER_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_CUSTOMER_2> data { get; set; }
    }
}
