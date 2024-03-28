using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    [MetadataType(typeof(ART_M_TEMPLATE_2_METADATA))]
    public partial class ART_M_TEMPLATE_2 : ART_M_TEMPLATE
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
    }

    public class ART_M_TEMPLATE_2_METADATA
    {
        [Required(ErrorMessage = "Temaplate name is required")]
        public string TEMPLATE_NAME { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string DESCRIPTION { get; set; }
    }

    public class ART_M_TEMPLATE_REQUEST : REQUEST_MODEL
    {
        public ART_M_TEMPLATE_2 data { get; set; }
    }

    public class ART_M_TEMPLATE_RESULT : RESULT_MODEL
    {
        public List<ART_M_TEMPLATE_2> data { get; set; }
    }
}