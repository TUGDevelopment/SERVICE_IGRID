using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
   
    public partial class ART_WF_ARTWORK_REQUEST_COUNTRY_2 : ART_WF_ARTWORK_REQUEST_COUNTRY
    {
        public string COUNTRY_DISPLAY_TXT { get; set; }
    }

    public class ART_WF_ARTWORK_REQUEST_COUNTRY_2_METADATA
    {

    }

    public class ART_WF_ARTWORK_REQUEST_COUNTRY_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_REQUEST_COUNTRY_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_REQUEST_COUNTRY_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_REQUEST_COUNTRY_2> data { get; set; }
    }
}
