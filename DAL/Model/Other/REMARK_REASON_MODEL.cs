using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_REMARK_REASON_OTHER_2 : ART_WF_REMARK_REASON_OTHER
    {
        public int REASON_ID { get; set; }
        public int ARTWORK_SUB_ID { get; set; }
        public int MOCKUP_SUB_ID { get; set; }
        public bool IS_SENDER { get; set; }
    }

    public class ART_WF_REMARK_REASON_OTHER_METADATA
    {

    }

    public class ART_WF_REMARK_REASON_OTHER_REQUEST : REQUEST_MODEL
    {
        public ART_WF_REMARK_REASON_OTHER_2 data { get; set; }
    }

    public class ART_WF_REMARK_REASON_OTHER_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_REMARK_REASON_OTHER_2> data { get; set; }
    }

    public class ART_WF_REMARK_REASON_OTHER_RESULT : RESULT_MODEL
    {
        public List<ART_WF_REMARK_REASON_OTHER_2> data { get; set; }
    }
}
