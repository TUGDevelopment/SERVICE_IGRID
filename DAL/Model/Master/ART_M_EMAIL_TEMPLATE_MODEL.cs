using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    [MetadataType(typeof(ART_M_EMAIL_TEMPLATE_2_METADATA))]
    public partial class ART_M_EMAIL_TEMPLATE_2 : ART_M_EMAIL_TEMPLATE
    {

    }

    public class ART_M_EMAIL_TEMPLATE_2_METADATA
    {
        [Required(ErrorMessage = "Subject is required")]
        public string M_SUBJECT { get; set; }

        [Required(ErrorMessage = "Dear is required")]
        public string M_DEAR { get; set; }

        [Required(ErrorMessage = "Email notification is required")]
        public string M_BODY_01 { get; set; }
    }

    public class ART_M_EMAIL_TEMPLATE_REQUEST : REQUEST_MODEL
    {
        public ART_M_DECISION_REASON_2 data { get; set; }
    }

    public class ART_M_EMAIL_TEMPLATE_RESULT : RESULT_MODEL
    {
        public List<ART_M_DECISION_REASON_2> data { get; set; }
    }
}