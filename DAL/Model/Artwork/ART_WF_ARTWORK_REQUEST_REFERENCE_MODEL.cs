using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
   
    public partial class ART_WF_ARTWORK_REQUEST_REFERENCE_2 : ART_WF_ARTWORK_REQUEST_REFERENCE
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
    }

    public class ART_WF_ARTWORK_REQUEST_REFERENCE_2_METADATA
    {

    }

    public class ART_WF_ARTWORK_REQUEST_REFERENCE_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_REQUEST_REFERENCE_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_REQUEST_REFERENCE_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_REQUEST_REFERENCE_2> data { get; set; }
    }
}
