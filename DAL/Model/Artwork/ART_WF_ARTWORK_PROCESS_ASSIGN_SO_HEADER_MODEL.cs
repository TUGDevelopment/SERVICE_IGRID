using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2 : ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
    {
        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//
        public ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2 ASSIGN_SO_ITEM { get; set; }
        public SAP_M_PO_COMPLETE_SO_HEADER SO_HEADER { get; set; }

        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//

    }

    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2_METADATA
    {
         
    }


    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2> data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2> data { get; set; }
    }
}
