using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    [MetadataType(typeof(ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_2_METADATA))]
    public partial class ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_2 : ART_M_RECEIVE_EMAIL_NEW_CUSTOMER
    {
        public string EMAIL_DISPLAY_TXT { get; set; }
        public string USER_DISPLAY_TXT { get; set; }
    }

    public class ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_2_METADATA
    {
        [Required(ErrorMessage = "Username is required")]
        public string USER_ID { get; set; }
    }

    public class ART_M_RECEIVE_EMAIL_NEW_REQUEST : REQUEST_MODEL
    {
        public ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_2 data { get; set; }
    }

    public class ART_M_RECEIVE_EMAIL_NEW_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_2> data { get; set; }
    }

    public class ART_M_RECEIVE_EMAIL_NEW_RESULT : RESULT_MODEL
    {
        public List<ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_2> data { get; set; }
    }
}