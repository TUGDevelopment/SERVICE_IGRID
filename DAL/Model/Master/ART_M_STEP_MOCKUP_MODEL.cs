﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    [MetadataType(typeof(ART_M_STEP_MOCKUP_2_METADATA))]
    public partial class ART_M_STEP_MOCKUP_2 : ART_M_STEP_MOCKUP
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
    }

    public class ART_M_STEP_MOCKUP_2_METADATA
    {
        [Required(ErrorMessage = "Step name is required")]
        public string STEP_MOCKUP_NAME { get; set; }

        [Required(ErrorMessage = "Step description is required")]
        public string STEP_MOCKUP_DESCRIPTION { get; set; }
    }

    public class ART_M_STEP_MOCKUP_REQUEST : REQUEST_MODEL
    {
        public ART_M_STEP_MOCKUP_2 data { get; set; }
    }

    public class ART_M_STEP_MOCKUP_RESULT : RESULT_MODEL
    {
        public List<ART_M_STEP_MOCKUP_2> data { get; set; }
    }
}