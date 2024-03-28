using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_MOCKUP_PROCESS_RD_2 : ART_WF_MOCKUP_PROCESS_RD
    {
        public string ACTION_NAME { get; set; }
        public string CREATE_BY_DISPLAY_TXT { get; set; }
        public string COMMENT_BY_PG { get; set; }
        public string REASON_BY_PG { get; set; }
        public string REASON_BY_OTHER { get; set; }
        public string IS_FINAL_SAMPLE_DISPLAY_TXT { get; set; }
        public string TYPE_OF_SAMPLE_DISPLAY_TXT { get; set; }
        public string REMARK_REASON_BY_PG { get; set; }
        public string REMARK_REASON { get; set; }
        public DateTime CREATE_DATE_BY_PG { get; set; }
        public bool ENDTASKFORM { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_RD_2_METADATA
    {

    }

    public class ART_WF_MOCKUP_PROCESS_RD_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_PROCESS_RD_2 data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_RD_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_RD_2> data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_RD_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_RD_2> data { get; set; }
    }
}
