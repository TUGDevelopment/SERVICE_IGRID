using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    [MetadataType(typeof(ART_WF_ARTWORK_ATTACHMENTT_2_METADATA))]
    public partial class ART_WF_ARTWORK_ATTACHMENT_2 : ART_WF_ARTWORK_ATTACHMENT
    {
        public int currentUserId { get; set; }
        public bool canDelete { get; set; }
        public string step { get; set; }
        public string remark { get; set; }
        public string create_by_display_txt { get; set; }
        public DateTime create_date_display_txt { get; set; }
        public bool canDownload { get; set; }
        public bool canAddVersion { get; set; }
        public string create_by_desc_txt { get; set; }
        public string NODE_ID_TXT { get; set; }
        public string step_code { get; set; }   // #INC-36800 by aof.
    }

    public class ART_WF_ARTWORK_ATTACHMENTT_2_METADATA
    {
    }

    public class ART_WF_ARTWORK_ATTACHMENT_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_ATTACHMENT_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_ATTACHMENT_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_ARTWORK_ATTACHMENT_2> data { get; set; }
    }

    public class ART_WF_ARTWORK_ATTACHMENT_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_ATTACHMENT_2> data { get; set; }
    }
}
