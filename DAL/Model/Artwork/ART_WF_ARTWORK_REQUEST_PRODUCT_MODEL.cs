using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
   
    public partial class ART_WF_ARTWORK_REQUEST_PRODUCT_2 : ART_WF_ARTWORK_REQUEST_PRODUCT
    {
        public string PRODUCT_DISPLAY_TXT { get; set; }

        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        
        public string NET_WEIGHT { get; set; }
        public string DRAINED_WEIGHT { get; set; }
        public string PRODUCT_DESCRIPTION { get; set; }
    }

    public class ART_WF_ARTWORK_REQUEST_PRODUCT_2_METADATA
    {

    }

    public class ART_WF_ARTWORK_REQUEST_PRODUCT_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_REQUEST_PRODUCT_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_REQUEST_PRODUCT_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_REQUEST_PRODUCT_2> data { get; set; }
    }
}
