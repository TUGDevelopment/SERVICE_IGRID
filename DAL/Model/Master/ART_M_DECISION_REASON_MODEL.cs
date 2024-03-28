using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    [MetadataType(typeof(ART_M_DECISION_REASON_2_METADATA))]
    public partial class ART_M_DECISION_REASON_2 : ART_M_DECISION_REASON
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
    }

    public class ART_M_DECISION_REASON_2_METADATA
    {
        [Required(ErrorMessage = "Workflow is required")]
        public string WF { get; set; }

        [Required(ErrorMessage = "Step is required")]
        public string STEP_CODE { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string DESCRIPTION { get; set; }
    }

    public class ART_M_DECISION_REASON_REQUEST : REQUEST_MODEL
    {
        public ART_M_DECISION_REASON_2 data { get; set; }
    }

    public class ART_M_DECISION_REASON_RESULT : RESULT_MODEL
    {
        public List<ART_M_DECISION_REASON_2> data { get; set; }
    }
}