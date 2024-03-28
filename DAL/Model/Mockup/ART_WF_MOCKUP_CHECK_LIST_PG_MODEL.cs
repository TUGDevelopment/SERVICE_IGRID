using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    [MetadataType(typeof(ART_WF_MOCKUP_CHECK_LIST_PG_2_METADATA))]
    public partial class ART_WF_MOCKUP_CHECK_LIST_PG_2 : ART_WF_MOCKUP_CHECK_LIST_PG
    {
        public string PACKING_TYPE_DISPLAY_TXT { get; set; }
        public string NUMBER_OF_COLOR_DISPLAY_TXT { get; set; }
        public string PRINT_SYSTEM_DISPLAY_TXT { get; set; }
        public string BOX_COLOR_DISPLAY_TXT { get; set; }
        public string COATING_DISPLAY_TXT { get; set; }
        public string STYLE_DISPLAY_TXT { get; set; }
       
    }

    public class ART_WF_MOCKUP_CHECK_LIST_PG_2_METADATA
    {
        
    }

    public class ART_WF_MOCKUP_CHECK_LIST_PG_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_CHECK_LIST_PG_2 data { get; set; }
    }

    public class ART_WF_MOCKUP_CHECK_LIST_PG_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_CHECK_LIST_PG_2> data { get; set; }
    }

}
