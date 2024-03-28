using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_MOCKUP_PROCESS_RD_BY_PG_2 : ART_WF_MOCKUP_PROCESS_RD_BY_PG
    {
        public ART_WF_MOCKUP_PROCESS_2 PROCESS { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_RD_BY_PG_2_METADATA
    {

    }

    public class ART_WF_MOCKUP_PROCESS_RD_BY_PG_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_PROCESS_RD_BY_PG_2 data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_RD_BY_PG_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_RD_BY_PG_2> data { get; set; }
    }
}
