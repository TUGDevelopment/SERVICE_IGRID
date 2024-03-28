using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_PROCESS_VENDOR_PO_2 : ART_WF_ARTWORK_PROCESS_VENDOR_PO
    {
        public ART_WF_ARTWORK_PROCESS_2 PROCESS { get; set; }
        public bool ENDTASKFORM { get; set; }
        public string ACTION_NAME { get; set; }
       
        public string REASON_BY_OTHER { get; set; }
        public string SEND_TO_VENDOR_TYPE { get; set; }
        public string STEP_ARTWORK_CODE { get; set; }
        public string PURCHASE_ORDER_NO { get; set; }
        

        public string REQUEST_SHADE_LIMIT_REFERENCE { get; set; }
        public string COMMENT_BY_PP { get; set; }
        public string REASON_BY_PP { get; set; }

        public string REMARK_REASON_BY_PP { get; set; }
        public string REMARK_REASON_BY_OTHER { get; set; }
        public DateTime CREATE_DATE_BY_PP { get; set; }
        public string VENDOR_DISPLAY_TXT { get; set; }

        public string CONFIRM_PO_DISPLAY_TXT { get; set; }

        public List<string> LIST_AW_CONFIRM_PO_DISPLAY_TXT { get; set; }
        public List<string> LIST_CONFIRM_PO_DISPLAY_TXT { get; set; }
        public string AW_ENCRYPT { get; set; }


    }

    public class ART_WF_ARTWORK_PROCESS_VENDOR_PO_2_METADATA
    {
         
    }

    public class ART_WF_ARTWORK_PROCESS_VENDOR_PO_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_VENDOR_PO_2 data { get; set; }
    }


    public class ART_WF_ARTWORK_PROCESS_VENDOR_PO_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_VENDOR_PO_2> data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_VENDOR_PO_2> data { get; set; }
    }
}
