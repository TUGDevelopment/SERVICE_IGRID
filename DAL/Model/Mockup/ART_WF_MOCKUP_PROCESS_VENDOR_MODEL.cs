using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_MOCKUP_PROCESS_VENDOR_2 : ART_WF_MOCKUP_PROCESS_VENDOR
    {
        public DateTime CREATE_DATE_BY_PG { get; set; }
        public bool ENDTASKFORM { get; set; }
        public string ACTION_NAME { get; set; }
        public string COMMENT_BY_PG { get; set; }
        public string REASON_BY_PG { get; set; }
        public string REASON_BY_OTHER { get; set; }
        public string SAMPLE_AMOUNT_FOR_REQ_SAMPLE_DIELINE { get; set; }
        public string SAMPLE_AMOUNT_FOR_SEND_PRIMARY { get; set; }
        public string STEP_MOCKUP_CODE { get; set; }
        public string SEND_TO_VENDOR_TYPE { get; set; }
        public string VENDOR_DISPLAY_TXT { get; set; }
        public string REMARK_REASON_BY_PG { get; set; }
        public string REMARK_REASON { get; set; }

    }

    public class ART_WF_MOCKUP_PROCESS_VENDOR_2_METADATA
    {

    }

    public class ART_WF_MOCKUP_PROCESS_VENDOR_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_PROCESS_VENDOR_2 data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_VENDOR_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_VENDOR_2> data { get; set; }
    }

    // by aof #INC-11265
    public class ART_WF_MOCKUP_PROCESS_VENDOR_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_VENDOR_2> data { get; set; }
    }
}
