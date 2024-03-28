using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    [MetadataType(typeof(ART_M_MESSAGE_2_METADATA))]
    public partial class ART_M_MESSAGE_2 : ART_M_MESSAGE
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
    }

    public class ART_M_MESSAGE_2_METADATA
    {
      
    }

    public class ART_M_MESSAGE_REQUEST : REQUEST_MODEL
    {
        public ART_M_MESSAGE_2 data { get; set; }
    }

    public class ART_M_MESSAGE_RESULT : RESULT_MODEL
    {
        public List<ART_M_MESSAGE_2> data { get; set; }
    }
}