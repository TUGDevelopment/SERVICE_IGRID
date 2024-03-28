using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_2 : ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK
    {
        public bool ENDTASKFORM { get; set; }
        public string ACTION_NAME { get; set; }
        public string COMMENT_BY_PA { get; set; }
        public string REASON_BY_PA { get; set; }
        public string REASON_BY_OTHER { get; set; }
        public DateTime CREATE_DATE_BY_PA { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_2_METADATA
    {

    }

    public class ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_2> data { get; set; }
    }
}
