using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_2 : ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG
    {
        public string SPECIFICATION_FOR_REQ_SAMPLE_DIELINE_DISPLAY_TXT { get; set; }
        public string FLUTE_FOR_REQ_SAMPLE_DIELINE_DISPLAY_TXT { get; set; }
        public ART_WF_MOCKUP_PROCESS_2 PROCESS { get; set; }
        public ART_WF_MOCKUP_PROCESS_VENDOR_2 PROCESS_VENDOR { get; set; }   //appended by aof #INC-11265
        public string REMARK_REASON { get; set; }   //appended by aof #INC-11265

    }

    public class ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_2_METADATA
    {

    }

    public class ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_2 data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_2> data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_2> data { get; set; }
    }
}
