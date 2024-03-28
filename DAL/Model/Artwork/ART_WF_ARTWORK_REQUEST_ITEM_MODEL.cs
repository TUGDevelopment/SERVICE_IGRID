using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
   
    public partial class ART_WF_ARTWORK_REQUEST_ITEM_2 : ART_WF_ARTWORK_REQUEST_ITEM
    {
        public string CREATE_BY_DISPLAY_TXT { get; set; }
        public int ARTWORK_SUB_ID { get; set; }

        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }

        public int? MATERIAL_GROUP_ID { get; set; }
        public string NODE_ID_TXT { get; set; }


        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//
        public ART_WF_ARTWORK_PROCESS_2 PROCESS_STEP_PA { get; set; }
        public ART_WF_ARTWORK_PROCESS_2 PROCESS_STEP_PG { get; set; }
        public ART_WF_ARTWORK_PROCESS_2 PROCESS_STEP_PP { get; set; }

        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//
    }

    public class ART_WF_ARTWORK_REQUEST_ITEM_2_METADATA
    {

    }

    public class ART_WF_ARTWORK_REQUEST_ITEM_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_REQUEST_ITEM_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_REQUEST_ITEM_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_REQUEST_ITEM_2> data { get; set; }
    }
}
