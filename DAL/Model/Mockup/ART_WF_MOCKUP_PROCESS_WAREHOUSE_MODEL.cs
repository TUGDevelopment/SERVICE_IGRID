using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_MOCKUP_PROCESS_WAREHOUSE_2 : ART_WF_MOCKUP_PROCESS_WAREHOUSE
    {
        public string ACTION_NAME { get; set; }
        public string TEST_PACK_RESULT_DISPLAY_TXT { get; set; }
        public string RECEIVE_PHYSICAL_DISPLAY_TXT { get; set; }
        public string NEED_COMMISSIONING_DISPLAY_TXT { get; set; }
        public string IS_DIFFICULT_DISPLAY_TXT { get; set; }
        public string CREATE_BY_DISPLAY_TXT { get; set; }
        public string COMMENT_BY_PG { get; set; }
        public string REASON_BY_PG { get; set; }
        public string REMARK_REASON_BY_PG { get; set; }
        public string REASON_BY_OTHER { get; set; }
        public string REMARK_REASON { get; set; }
        public bool ENDTASKFORM { get; set; }
        public DateTime CREATE_DATE_BY_PG { get; set; }
        public string TEST_PACK_SIZING_DISPLAY_TXT { get; set; }
        public string TEST_PACK_HARD_EASY_DISPLAY_TXT { get; set; }
        public string TEST_PACK_SIZING { get; set; }
        public string TEST_PACK_HARD_EASY { get; set; }
        public string SUPPLIER_PRIMARY_CONTAINER { get; set; }
        public string SUPPLIER_PRIMARY_LID { get; set; }
        public string SHIP_TO_FACTORY { get; set; }
        public string TEST_PACK_FAIL_DISPLAY_TXT { get; set; }
        
    }

    public class ART_WF_MOCKUP_PROCESS_WAREHOUSE_2_METADATA
    {

    }

    public class ART_WF_MOCKUP_PROCESS_WAREHOUSE_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_PROCESS_WAREHOUSE_2 data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_WAREHOUSE_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_WAREHOUSE_2> data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_WAREHOUSE_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_WAREHOUSE_2> data { get; set; }
    }
}
