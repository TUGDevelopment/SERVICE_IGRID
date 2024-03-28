using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public partial class ART_WF_ARTWORK_REQUEST_2 : ART_WF_ARTWORK_REQUEST
    {
        public bool IS_FFC { get; set; }
        public string SaveEmpty { get; set; }
        public ART_WF_ARTWORK_PROCESS_2 PROCESS { get; set; }
        public List<ART_WF_ARTWORK_REQUEST_ITEM_2> REQUEST_ITEMS { get; set; }
        public List<ART_WF_ARTWORK_REQUEST_RECIPIENT_2> REQUEST_RECIPIENT { get; set; }
        public List<ART_WF_ARTWORK_REQUEST_COUNTRY_2> COUNTRY { get; set; }
        public List<XECM_M_PRODUCT_2> PRODUCT { get; set; }
        public List<ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2> PRODUCTION_PLANT { get; set; }
        public List<ART_WF_ARTWORK_REQUEST_REFERENCE_2> REFERENCE { get; set; }
        public List<ART_WF_ARTWORK_REQUEST_SALES_ORDER_2> SALES_ORDER { get; set; }
        public List<ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2> SALES_ORDER_REPEAT { get; set; }
        public List<ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_2> MAIL_TO_CUSTOMER { get; set; }

        public string CREATOR_NAME { get; set; }
        public string CUSTOMER_OTHER_DISPLAY_TXT { get; set; }
        public string COMPANY_DISPLAY_TXT { get; set; }
        public string SOLD_TO_DISPLAY_TXT { get; set; }
        public string SHIP_TO_DISPLAY_TXT { get; set; }
        public string BRAND_DISPLAY_TXT { get; set; }
        public string REFERENCE_ARTWORK_REQUEST_DISPLAY_TXT { get; set; }
        public string REVIEWER_DISPLAY_TXT { get; set; }
        public string TYPE_OF_DISPLAY_TXT { get; set; }
        public string TYPE_OF_PRODUCT_DISPLAY_TXT { get; set; }

        public string CONTAINER_TYPE_DISPLAY_TXT { get; set; }
        public string LID_TYPE_DISPLAY_TXT { get; set; }
        public string PACKING_STYLE_DISPLAY_TXT { get; set; }
        public string PACK_SIZE_DISPLAY_TXT { get; set; }
        public string PRIMARY_SIZE_DISPLAY_TXT { get; set; }
        public string PRIMARY_TYPE_DISPLAY_TXT { get; set; }

        public string THREE_P_DISPLAY_TXT { get; set; }
        public string TWO_P_DISPLAY_TXT { get; set; }
        public string ARTWORK_UPLOADED_BY_DISPLAY_TXT { get; set; }

        public bool ENDTASKFORM { get; set; }
        public int ARTWORK_SUB_ID { get; set; }
        public bool IS_COMPLETE { get; set; }
        public bool IS_SEND_TO_PP { get; set; }
        public string COMMENT { get; set; }
        public string CONTROL_NAME { get; set; }

        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//
        public string RESULT_CREATE_WF_WFNO { get; set; }
        public string RESULT_CREATE_WF_STATUS { get; set; }
        public string RESULT_CREATE_WF_MESSAGE { get; set; }
        public List<ART_WF_ARTWORK_REQUEST_PRODUCT_2> REQUEST_PRODUCT { get; set; }
        public List<long> LIST_AW_NODE_ID { get; set; }
        public string REQUEST_FROM_NO_ERROR { get; set; }
        public bool IS_VAP { get; set; }  //20230121_3V_SOREPAT INC-93118
        public bool HAS_VAP_PLANT { get; set; }  //20230121_3V_SOREPAT INC-93118
        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//

    }

    public class ART_WF_ARTWORK_REQUEST_2_METADATA
    {

    }

    public class ART_WF_ARTWORK_REQUEST_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_REQUEST_2 data { get; set; }
    }

    //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//
    public class ART_WF_ARTWORK_REQUEST_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_ARTWORK_REQUEST_2> data { get; set; }
    }

    public class ART_WF_ARTWORK_REQUEST_RESULT_LIST : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_REQUEST_2> data  {get; set; }
    }
    //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//

    public class ART_WF_ARTWORK_REQUEST_RESULT : RESULT_MODEL
    {
        public string WF_NO { get; set; }
        public int ARTWORK_SUB_ID { get; set; }
        public ART_WF_ARTWORK_REQUEST_REQUEST param { get; set; }
        public List<ART_WF_ARTWORK_REQUEST_2> data { get; set; }
        public List<long> listAWNodeId { get; set; }
        public string requestFormNo { get; set; }
    }
}
