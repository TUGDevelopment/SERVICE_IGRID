using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    public partial class ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_2 : ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR
    {
        public string VENDOR_DISPLAY_TXT { get; set; }
        public string USER_DISPLAY_TXT { get; set; }
        public string EMAIL { get; set; }

        public string ACTION_NAME { get; set; }
        public string CREATE_BY_DISPLAY_TXT { get; set; }
        public string COMMENT_BY_PG { get; set; }
        public string REASON_BY_PG { get; set; }
        public string REASON_BY_OTHER { get; set; }
        public DateTime CREATE_DATE_BY_PG { get; set; }
        public string REMARK_REASON_BY_PG { get; set; }
        public string REMARK_REASON { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_2 data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_REQUEST_LIST : REQUEST_MODEL
    {
        public bool ENDTASKFORM { get; set; }
        public List<ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_2> data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_2> data { get; set; }
    }
}