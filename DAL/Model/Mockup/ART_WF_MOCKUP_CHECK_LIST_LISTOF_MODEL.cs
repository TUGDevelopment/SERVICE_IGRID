using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    [MetadataType(typeof(ART_WF_MOCKUP_CHECK_LIST_LISTOF_METADATA))]
    public class ART_WF_MOCKUP_CHECK_LIST_LISTOF // : ART_WF_MOCKUP_CHECK_LIST
    {
        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }
    }

    public class ART_WF_MOCKUP_CHECK_LIST_LISTOF_METADATA
    {
        
    }

    public class ART_WF_MOCKUP_CHECK_LIST_LISTOF_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_CHECK_LIST_LISTOF data { get; set; }
    }

    public class ART_WF_MOCKUP_CHECK_LIST_LISTOF_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_CHECK_LIST_LISTOF> data { get; set; }
    }

}
