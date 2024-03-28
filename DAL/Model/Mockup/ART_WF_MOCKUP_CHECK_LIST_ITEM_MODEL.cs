using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    [MetadataType(typeof(ART_WF_MOCKUP_CHECK_LIST_ITEM_2_METADATA))]
    public partial class ART_WF_MOCKUP_CHECK_LIST_ITEM_2 : ART_WF_MOCKUP_CHECK_LIST_ITEM
    {
        public string CHECK_LIST_NO { get; set; }
        public string PACKING_TYPE_DISPLAY_TXT { get; set; }
        public string PRINT_SYSTEM_DISPLAY_TXT { get; set; }
        public string NUMBER_OF_COLOR_DISPLAY_TXT { get; set; }
        public string BOX_COLOR_DISPLAY_TXT { get; set; }
        public string COATING_DISPLAY_TXT { get; set; }
        public string STYLE_DISPLAY_TXT { get; set; }
        public string VENDOR_DISPLAY_TXT { get; set; }
    }


    public class ART_WF_MOCKUP_CHECK_LIST_ITEM_2_METADATA
    {

    }

    public class ART_WF_MOCKUP_CHECK_LIST_ITEM_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_CHECK_LIST_ITEM_2 data { get; set; }
    }

    public class ART_WF_MOCKUP_CHECK_LIST_ITEM_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_CHECK_LIST_ITEM_2> data { get; set; }
    }

}
