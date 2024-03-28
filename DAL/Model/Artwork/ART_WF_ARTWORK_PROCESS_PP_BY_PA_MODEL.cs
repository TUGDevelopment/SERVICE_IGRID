using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_PROCESS_PP_BY_PA_2 : ART_WF_ARTWORK_PROCESS_PP_BY_PA
    {

        public bool ENDTASKFORM { get; set; }
        public string ACTION_CODE { get; set; }
        
        public string ACTION_NAME { get; set; }
        public string COMMENT_BY_PA { get; set; }
        public string REASON_BY_PA { get; set; }
        public string REASON_BY_OTHER { get; set; }
        public DateTime CREATE_DATE_BY_PA { get; set; }

        public string REQUEST_SHADE_LIMIT_REFERENCE { get; set; }

        public ART_WF_ARTWORK_PROCESS_2 PROCESS { get; set; }

    }

    public class ART_WF_ARTWORK_PROCESS_PP_BY_PA_2_METADATA
    {
         
    }

    public class ART_WF_ARTWORK_PROCESS_PP_BY_PA_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_PP_BY_PA_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_PP_BY_PA_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_PP_BY_PA_2> data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_PP_BY_PA_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_PP_BY_PA_2> data { get; set; }
    }
}
