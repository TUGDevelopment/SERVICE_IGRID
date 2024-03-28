using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_PROCESS_2 : ART_WF_ARTWORK_PROCESS
    {
        public string CURRENT_STEP_CODE_DISPLAY_TXT { get; set; }
        public string CURRENT_STEP_DISPLAY_TXT { get; set; }
        public string CURRENT_USER_DISPLAY_TXT { get; set; }
        public string REMARK_OTHERS { get; set; }
        public string MOCKUP_NO_DISPLAY_TXT { get; set; }
        public string ARTWORK_NO_DISPLAY_TXT { get; set; }
        public string REFERENCE_MATERIAL { get; set; }
        public string MATERIAL_NO { get; set; }
        public string MATERIAL_STATUS { get; set; }
        public string IGRID_REFERENCE_NO { get; set; }
        public string ARTWORK_REQUEST_FORM_NO_DISPLAY_TXT { get; set; }
        public string ARTWORK_REFERENCE_REQUEST_NO_DISPLAY_TXT { get; set; }
        //public long NODE_ID { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_EXTENSION { get; set; }

        public string IS_REASSIGN { get; set; }
        public string OLD_OWNER_REASSIGN { get; set; }
        public string NEW_OWNER_REASSIGN { get; set; }
        public string REASSIGNBY { get; set; }
        public string REASSIGNREASON { get; set; }

        public string IS_REOPEN { get; set; }
        public string OLD_OWNER_REOPEN { get; set; }
        public string NEW_OWNER_REOPEN { get; set; }
        public string REOPENBY { get; set; }
        public string REOPENREASON { get; set; }

        public string IS_DELEGATE_ { get; set; }
        public string OLD_OWNER_DELEGATE { get; set; }
        public string NEW_OWNER_DELEGATE { get; set; }
        public string DELEGATEBY { get; set; }
        public string DELEGATEREASON { get; set; }

        public string IS_CG_OWNER { get; set; }
        public string OLD_OWNER_CG_OWNER { get; set; }
        public string NEW_OWNER_CG_OWNER { get; set; }
        public string CG_OWNER_BY { get; set; }
        public string CG_OWNER_REASON { get; set; }

        public string IS_READY_CREATE_PO { get; set; }
        public string READY_CREATE_PO { get; set; }
        public string READY_CREATE_PO_VALIDATE_MSG { get; set; }
        public string IS_LOCK_PRODUCT_CODE { get; set; }
        public string RECEIVE_SHADE_LIMIT { get; set; }
        public string SHADE_LIMIT { get; set; }
        public string CHANGE_POINT { get; set; }
        public bool ENDTASKFORM { get; set; }

        public string IS_SO_CHANGE { get; set; }
        public string IS_OVER_DUE { get; set; }
        public string STEP_DURATION_REMARK_REASON { get; set; }

        public DateTime DUEDATE { get; set; }
        public string STEPNAME { get; set; }

        public string NODE_ID_TXT { get; set; }


        public string CHECK_SO_REPEAT_IS_NOT_SEND_BACK_MK { get; set; }  // ticket.445558 by aof 


        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//
        public ART_WF_LOG_DELEGATE LOG_DELEGATE { get; set; }
        public ART_WF_ARTWORK_PROCESS_PA_2 PROCESS_PA { get; set; }
        public ART_WF_ARTWORK_PROCESS_PG_2 PROCESS_PG { get; set; }
        public ART_WF_ARTWORK_PROCESS_PP_BY_PA PROCESS_PP_BY_PA { get; set; }
        //public ART_WF_ARTWORK_PROCESS_PA_2 PROCESS_PG { get; set; }
        public ART_WF_ARTWORK_ATTACHMENT_2 ATTACHMENT { get; set; }
        public List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2> LIST_PROCESS_SO_DETAIL { get; set; }
        public List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2> LIST_ASSIGN_SO_LONG_TEXT { get; set; }
        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//

    }

    public class ART_WF_ARTWORK_PROCESS_2_METADATA
    {
         
    }

    public class ART_WF_ARTWORK_PROCESS_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_2> data { get; set; }
    }
}
