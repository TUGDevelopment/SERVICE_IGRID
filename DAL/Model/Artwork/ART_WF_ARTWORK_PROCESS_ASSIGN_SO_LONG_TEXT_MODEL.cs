using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2 : ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT
    {

        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//
        public int TEMP_RUNNING_ID { get; set; }
        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//
    }

    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2_METADATA
    {
         
    }


    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2> data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2> data { get; set; }
    }
}
