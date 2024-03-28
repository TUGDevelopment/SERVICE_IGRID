using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_MAPPING_PO_2 : ART_WF_ARTWORK_MAPPING_PO
    {
        public string CURRENCY_DISPLAY_TXT { get; set; }
        public string PURCHASING_ORG_DISPLAY_TXT { get; set; }
        public string COMPANY_DISPLAY_TXT { get; set; }
        public string VENDOR_DISPLAY_TXT { get; set; }
        public string PURCHASER_DISPLAY_TXT { get; set; }
        public string PO_NO2 { get; set; }
    }

    public class ART_WF_ARTWORK_MAPPING_PO_2_METADATA
    {
         
    }

    public class ART_WF_ARTWORK_MAPPING_PO_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_MAPPING_PO_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_MAPPING_PO_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_ARTWORK_MAPPING_PO_2> data { get; set; }
    }

    public class ART_WF_ARTWORK_MAPPING_PO_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_MAPPING_PO_2> data { get; set; }
    }
}
