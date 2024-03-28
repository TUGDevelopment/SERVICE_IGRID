using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_ARTWORK_PROCESS_PG_2 : ART_WF_ARTWORK_PROCESS_PG
    {
        public bool ENDTASKFORM { get; set; }
        //public string ACTION_NAME { get; set; }
        //public string COMMENT_BY_PA { get; set; }
        //public string REASON_BY_PA { get; set; }
        //public string REASON_BY_OTHER { get; set; }
        //public DateTime CREATE_DATE_BY_PA { get; set; }


        public Nullable<int> DIE_LINE_MOCKUP_SUB_ID { get; set; }
        public Nullable<int> PRIMARY_TYPE_ID { get; set; }
        public Nullable<int> PRIMARY_SIZE_ID { get; set; }
        public Nullable<int> PACK_SIZE_ID { get; set; }
        public Nullable<int> PACKING_STYLE_ID { get; set; }
        public Nullable<int> PACKAGING_TYPE_ID { get; set; }
        public string PRIMARY_TYPE_DISPLAY_TXT { get; set; }
        public string PACKAGING_TYPE_DISPLAY_TXT { get; set; }

        public string WARNING_MSG_DIELINE_FILE { get; set; }

        public List<PG_HISTORY> HISTORIES { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_PG_2_METADATA
    {

    }

    public class ART_WF_ARTWORK_PROCESS_PG_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_PG_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_PG_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_PG_2> data { get; set; }
    }

    public class PG_HISTORY
    {
        public string NO { get; set; }
        public string REASON_BY_PA { get; set; }
        public string COMMENT_BY_PA { get; set; }
        public DateTime CREATE_DATE_BY_PA { get; set; }
        public string ACTION_NAME { get; set; }
        public string REASON_BY_PG { get; set; }
        public string COMMENT_BY_PG { get; set; }
        public DateTime CREATE_DATE_BY_PG { get; set; }

    }
}
