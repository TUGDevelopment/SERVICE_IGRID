using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    [MetadataType(typeof(ART_M_USER_VENDOR_2_METADATA))]
    public partial class ART_M_USER_VENDOR_2 : ART_M_USER_VENDOR
    {
        public int MOCKUP_ID { get; set; }
        public string EMAIL { get; set; }
        public string USER_DISPLAY_TXT { get; set; }
        public string VENDOR_DISPLAY_TXT { get; set; }
        public bool CHECKED { get; set; }
    }

    public class ART_M_USER_VENDOR_2_METADATA
    {

    }

    public class ART_M_USER_VENDOR_REQUEST : REQUEST_MODEL
    {
        public ART_M_USER_VENDOR_2 data { get; set; }
    }

    public class ART_M_USER_VENDOR_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_M_USER_VENDOR_2> data { get; set; }
    }

    public class ART_M_USER_VENDOR_RESULT : RESULT_MODEL
    {
        public List<ART_M_USER_VENDOR_2> data { get; set; }
    }
}