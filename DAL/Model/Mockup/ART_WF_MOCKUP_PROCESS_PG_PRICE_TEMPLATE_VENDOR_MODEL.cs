using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2 : ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG
    {
        public string COMMENT_BY_PG_SUP { get; set; }
        public int MOCKUPSUBID2 { get; set; }
        public int PRICE_TEMPLATE_ID { get; set; }
        public int USER_ID { get; set; }
        public int VENDOR_ID { get; set; }
        
        public string VENDOR_DISPLAY_TXT { get; set; }
        public string USER_DISPLAY_TXT { get; set; }
        public string EMAIL { get; set; }
        public string CANSELECT { get; set; }

        public string DIRECTION_OF_STICKER_DISPLAY_TXT { get; set; }
        public string STYLE_OF_PRINTING_DISPLAY_TXT { get; set; }
        public string STYLE_OF_PRINTING_OTHER_DISPLAY_TXT { get; set; }

        public string ACTION_CODE { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2 data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2> data { get; set; }
    }

    public class ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2> data { get; set; }
    }
}
