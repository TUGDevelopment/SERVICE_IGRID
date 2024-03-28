using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2 : ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT
    {
      

    }

    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2_METADATA
    {
         
    }


    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2> data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2> data { get; set; }
    }
}
