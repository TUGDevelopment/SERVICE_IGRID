using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_MATERIAL_LOCK_LOG_2 : ART_WF_ARTWORK_MATERIAL_LOCK_LOG
    {
      
    }

    public class ART_WF_ARTWORK_MATERIAL_LOCK_LOG_2_METADATA
    {
         
    }

    public class ART_WF_ARTWORK_MATERIAL_LOCK_LOG_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_MATERIAL_LOCK_LOG_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_MATERIAL_LOCK_LOG_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_ARTWORK_MATERIAL_LOCK_LOG_2> data { get; set; }
    }

    public class ART_WF_ARTWORK_MATERIAL_LOCK_LOG_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_MATERIAL_LOCK_LOG_2> data { get; set; }
    }
}
