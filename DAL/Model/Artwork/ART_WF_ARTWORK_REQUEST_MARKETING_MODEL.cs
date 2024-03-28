using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
   
    public partial class ART_WF_ARTWORK_REQUEST_RECIPIENT_2 : ART_WF_ARTWORK_REQUEST_RECIPIENT
    {
        public string RECIPIENT_USER_DISPLAY_TXT { get; set; }
        public string RECIPIENT_POSITION_DISPLAY_TXT { get; set; }
        public int RECIPIENT_POSITION_ID { get; set; }
        public int RECIPIENT_ID { get; set; }
        public string RECIPIENT_EMAIL { get; set; }
    }

    public class ART_WF_ARTWORK_REQUEST_RECIPIENT_2_METADATA
    {

    }

    public class ART_WF_ARTWORK_REQUEST_RECIPIENT_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_REQUEST_RECIPIENT_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_REQUEST_RECIPIENT_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_REQUEST_RECIPIENT_2> data { get; set; }
    }
}
