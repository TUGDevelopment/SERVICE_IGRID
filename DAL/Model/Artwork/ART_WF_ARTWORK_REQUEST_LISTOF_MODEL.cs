using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
   
    public   class ART_WF_ARTWORK_REQUEST_LISTOF
    {
        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }
    }

    public class ART_WF_ARTWORK_REQUEST_LISTOF_METADATA
    {

    }

    public class ART_WF_ARTWORK_REQUEST_LISTOF_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_REQUEST_LISTOF data { get; set; }
    }

    public class ART_WF_ARTWORK_REQUEST_LISTOF_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_REQUEST_LISTOF> data { get; set; }
    }
}
