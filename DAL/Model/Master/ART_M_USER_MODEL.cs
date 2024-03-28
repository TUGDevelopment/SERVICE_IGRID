using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    [MetadataType(typeof(ART_M_USER_2_METADATA))]
    public partial class ART_M_USER_2 : ART_M_USER
    {
        public Nullable<int> COMPANY_ID { get; set; }
        public Nullable<int> TYPE_OF_PRODUCT_ID { get; set; }
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string POSITION_DISPLAY_TXT { get; set; }
        public string USER_LEADER_DISPLAY_TXT { get; set; }
        public string COMPANY_DISPLAY_TXT { get; set; }
        public string TYPE_OF_PRODUCT_DISPLAY_TXT { get; set; }
        public string CUSTOMER_DISPLAY_TXT { get; set; }
        public string VENDOR_DISPLAY_TXT { get; set; }
        public string ROLE_DISPLAY_TXT { get; set; }
        public string NAME_EMAIL_DISPLAY_TXT { get; set; }

        public string IS_HF { get; set; }
        public string IS_PF { get; set; }

        public string ROLE_CODE { get; set; }
        
    }

    public partial class ART_M_USER_EXCEL
    {
        public int USER_ID { get; set; }
        public string USERNAME { get; set; }
        public string TITLE { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string EMAIL { get; set; }
        public string POSITION_DISPLAY_TXT { get; set; }
        public string ROLE_DISPLAY_TXT { get; set; }
        public string USER_LEADER_DISPLAY_TXT { get; set; }
        public string TYPE_OF_PRODUCT_DISPLAY_TXT { get; set; }
        public string COMPANY_DISPLAY_TXT { get; set; }
        public string CUSTOMER_DISPLAY_TXT { get; set; }
        public string VENDOR_DISPLAY_TXT { get; set; }
        public string IS_ACTIVE { get; set; }
        public string IS_ADUSER { get; set; }
    }

    public class ART_M_USER_2_METADATA
    {
        [Required(ErrorMessage = "Position is required")]
        public string POSITION_ID { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string USERNAME { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string PASSWORD { get; set; }

        public string TITLE { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FIRST_NAME { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LAST_NAME { get; set; }

        //[RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$",
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string EMAIL { get; set; }
    }

    public class ART_M_USER_REQUEST : REQUEST_MODEL
    {
        public ART_M_USER_2 data { get; set; }
    }

    public class ART_M_USER_RESULT : RESULT_MODEL
    {
        public List<ART_M_USER_2> data { get; set; }
    }
}