using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_2 : ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL
    {
        public string Status { get; set; }
        public DateTime? Unlock_Date_From { get; set; }
        public DateTime? Unlock_Date_To { get; set; }
      
      
      
    }

    public class ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_2_METADATA
    {
         
    }

    public class ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_2> data { get; set; }
    }

    public class ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_2> data { get; set; }
    }
}
