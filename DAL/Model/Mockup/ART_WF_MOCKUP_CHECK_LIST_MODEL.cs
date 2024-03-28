using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    [MetadataType(typeof(ART_WF_MOCKUP_CHECK_LIST_2_METADATA))]
    public partial class ART_WF_MOCKUP_CHECK_LIST_2 : ART_WF_MOCKUP_CHECK_LIST
    {
        public List<ART_WF_MOCKUP_CHECK_LIST_COUNTRY_2> COUNTRY { get; set; }
        public List<ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_2> PLANT { get; set; }
        public List<ART_WF_MOCKUP_CHECK_LIST_ITEM_2> ITEM { get; set; }
        public List<ART_WF_MOCKUP_CHECK_LIST_PRODUCT_2> PRODUCT { get; set; }
        public List<ART_WF_MOCKUP_CHECK_LIST_REFERENCE_2> REFERENCE { get; set; }
        public List<ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_2> MAIL_TO_CUSTOMER { get; set; }
        //public string CREATOR_NAME { get; set; }
        //public string COUNTRY_CODE_SET { get; set; }
        //public string PRODUCT_CODE_SET { get; set; }
        //public string SOLD_TO_NAME { get; set; }
        //public string BRAND_NAME { get; set; }
        //public string CREATED_BY_NAME { get; set; }
        public string MOCKUP_NO_DISPLAY_TXT { get; set; }
        public string REFERENCE_CHECK_LIST_DISPLAY_TXT { get; set; }
        public string TYPE_OF_PRODUCT_DISPLAY_TXT { get; set; }
        public string COMPANY_DISPLAY_TXT { get; set; }
        public string SOLD_TO_DISPLAY_TXT { get; set; }
        public string SHIP_TO_DISPLAY_TXT { get; set; }
        public string RD_PERSON_DISPLAY_TXT { get; set; }
        public string PRIMARY_TYPE_DISPLAY_TXT { get; set; }
        public string PRIMARY_SIZE_DISPLAY_TXT { get; set; }
        public string CONTAINER_TYPE_DISPLAY_TXT { get; set; }
        public string LID_TYPE_DISPLAY_TXT { get; set; }
        public string PACKING_STYLE_DISPLAY_TXT { get; set; }
        public string PACK_SIZE_DISPLAY_TXT { get; set; }
        public string BRAND_DISPLAY_TXT { get; set; }
        public string BRAND_OEM_DISPLAY_TXT { get; set; }
        public string REVIEWER_DISPLAY_TXT { get; set; }
        public string CREATOR_NAME { get; set; }
        public string STYLE_OF_PRINTING_DISPLAY_TXT { get; set; }

        public string TWO_P_DISPLAY_TXT { get; set; }
        public string THREE_P_DISPLAY_TXT { get; set; }
        public int MOCKUP_ID { get; set; }
        public int MOCKUP_SUB_ID { get; set; }
        public string CUSTOMER_OTHER_DISPLAY_TXT { get; set; }
        public bool ENDTASKFORM { get; set; }

        public string TO_DISPLAY_TXT { get; set; }
        public string CC_DISPLAY_TXT { get; set; }
        public string IS_REFERENCE { get; set; }

    }

    public class ART_WF_MOCKUP_CHECK_LIST_2_METADATA
    {
        //[Required(ErrorMessage = "Company is required....")]
        //[Required]
        //public int COMPANY_ID { get; set; }

        //[Required]
        //public int SOLD_TO_ID { get; set; }

        //[Required]
        //public int LID_TYPE_ID { get; set; }

        //[Required]
        //public int PACKING_STYLE_ID { get; set; }

        //[Required]
        //public int PACK_SIZE_ID { get; set; }
    }

    public class ART_WF_MOCKUP_CHECK_LIST_REQUEST : REQUEST_MODEL
    {
        public ART_WF_MOCKUP_CHECK_LIST_2 data { get; set; }
    }

    public class ART_WF_MOCKUP_CHECK_LIST_RESULT : RESULT_MODEL
    {
        public List<ART_WF_MOCKUP_CHECK_LIST_2> data { get; set; }
    }

}
