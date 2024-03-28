using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class SEARCH_REQUEST_FORM
    {
        public Nullable<int> SOLD_TO_ID { get; set; }
        public Nullable<int> SHIP_TO_ID { get; set; }
        public Nullable<int> BRAND_ID { get; set; }
        public Nullable<int> COUNTRY_ID { get; set; }
        public Nullable<int> CREATOR_ID { get; set; }

        public string ID { get; set; }
        public int CHECK_LIST_ID { get; set; }
        public int ARTWORK_ID { get; set; }
        public string REQUEST_FORM_NO { get; set; }
        public string PROJECT_NAME { get; set; }
        public string COMPANY_DISPLAY_TXT { get; set; }
        public string TYPE { get; set; }
        public string PRIMARY_TYPE_DISPLAY_TXT { get; set; }
        public string PRIMARY_SIZE_DISPLAY_TXT { get; set; }
        public string PACKING_STYLE_DISPLAY_TXT { get; set; }
        public string PACK_SIZE_DISPLAY_TXT { get; set; }
    }

    public class SEARCH_REQUEST_FORM_METADATA
    {

    }

    public class SEARCH_REQUEST_FORM_REQUEST : REQUEST_MODEL
    {
        public SEARCH_REQUEST_FORM data { get; set; }
    }

    public class SEARCH_REQUEST_FORM_RESULT : RESULT_MODEL
    {
        public List<SEARCH_REQUEST_FORM> data { get; set; }
    }
}
