using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2 : ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM
    {
        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//
        public SAP_M_PO_COMPLETE_SO_ITEM SAP_SO_ITEM_2 { get; set; }
        public ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2 ASSIGN_SO_ITEM_COMPONENT { get; set; }
       // public SAP_M_PO_COMPLETE_SO_ITEM_2 SO_ITEM { get; set; }
        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//
    }

    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2_METADATA
    {
         
    }


    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2> data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2> data { get; set; }
    }
}
