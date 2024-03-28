using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
   
    public partial class ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_2 : ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER
    {
        public Nullable<int> SOLD_TO_ID { get; set; }
        public Nullable<int> SHIP_TO_ID { get; set; }
        public Nullable<int> CUSTOMER_OTHER_ID { get; set; }

        public string USER_DISPLAY_TXT { get; set; }
    }

    public class ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_2_METADATA
    {

    }

    public class ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_2> data { get; set; }
    }
}
