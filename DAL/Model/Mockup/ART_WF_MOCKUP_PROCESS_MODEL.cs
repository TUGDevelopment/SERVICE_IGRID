using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    [MetadataType(typeof(ART_WF_MOCKUP_PROCESS_2_METADATA))]
    public partial class ART_WF_MOCKUP_PROCESS_2 : ART_WF_MOCKUP_PROCESS
    {
        public string CURRENT_STEP_CODE_DISPLAY_TXT { get; set; }
        public string CURRENT_STEP_DISPLAY_TXT { get; set; }
        public string CURRENT_USER_DISPLAY_TXT { get; set; }
        public int CHECK_LIST_ID { get; set; }
        public string REMARK_OTHERS { get; set; }
        public string STEP_DURATION_REMARK_REASON { get; set; }
        public string IS_CG_OWNER { get; set; }
        public string OLD_OWNER_CG_OWNER { get; set; }
        public string NEW_OWNER_CG_OWNER { get; set; }
        public string CG_OWNER_BY { get; set; }
        public string CG_OWNER_REASON { get; set; }

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

        public string CURRENT_VENDOR_DISPLAY_TXT { get; set; }
        public string CURRENT_CUSTOMER_DISPLAY_TXT { get; set; }

        public string MOCKUP_NO_DISPLAY_TXT { get; set; }
        public string CHECK_LIST_NO_DISPLAY_TXT { get; set; }
        public Nullable<long> NODE_ID_MOCKUP { get; set; }

        public string WORKFLOW_NO { get; set; }
        public string WORKFLOW_TYPE { get; set; }
        public int WORKFLOW_SUB_ID { get; set; }
        public int WORKFLOW_ID { get; set; }
        public string REMARK_REASSIGN { get; set; }

        public string IS_OVER_DUE { get; set; }
        public string CREATE_DATE_FROM { get; set; }
        public string CREATE_DATE_TO { get; set; }

        public DateTime DUEDATE { get; set; }
        public int NUMBER_VENDOR { get; set; }
        public string STEP_CODE { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_2_METADATA
    {
    }

    public class ART_WF_MOCKUP_PROCESS_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_PROCESS_2 data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_2> data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_2> data { get; set; }
    }
}
