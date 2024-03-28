using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    public partial class ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_2 : ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD
    {
        public DateTime CREATE_DATE_BY_PG { get; set; }
        public string ACTION_NAME { get; set; }
        public string COMMENT_BY_PG { get; set; }
        public string REASON_BY_PG { get; set; }
        public string REASON_BY_OTHER { get; set; }
        public bool ENDTASKFORM { get; set; }
        public string REMARK_REASON_BY_PG { get; set; }
        public string REMARK_REASON { get; set; }

    }

    public class ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_2 data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_2> data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_2> data { get; set; }
    }
}