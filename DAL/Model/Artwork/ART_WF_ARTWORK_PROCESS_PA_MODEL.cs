using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_PROCESS_PA_2 : ART_WF_ARTWORK_PROCESS_PA
    {
        public List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2> PRODUCTS { get; set; }
        public List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2> PRODUCT_OTHERS { get; set; }
        public List<ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_2> FAOS { get; set; }
        public List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_2> CATCHING_AREAS { get; set; }
        public List<ART_WF_ARTWORK_PROCESS_PA_PLANT_2> PLANTS { get; set; }
        public List<ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2> SYMBOLS { get; set; }
        public List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_2> CATCHING_METHODS { get; set; }  // ticke#425737 added by aof 

        //public string IS_LOCK_PRODUCT_CODE { get; set; }
        public string MATERIAL_GROUP_CODE { get; set; }
        public string MATERIAL_GROUP_DISPLAY_TXT { get; set; }
        public string MATERIAL_GROUP_FROMSO_DISPLAY_TXT { get; set; }
        public string RD_REFERENCE_NO_DISPLAY_TXT { get; set; }
        public string PRODUCT_CODE_DISPLAY_TXT { get; set; }
        public string TYPE_OF_DISPLAY_TXT { get; set; }
        public string PRIMARY_SIZE_DISPLAY_TXT { get; set; }
        public string CONTAINER_TYPE_DISPLAY_TXT { get; set; }
        public string LID_TYPE_DISPLAY_TXT { get; set; }
        public string PRODUCT_DESCRIPTION_DISPLAY_TXT { get; set; }
        public string NET_WEIGHT_DISPLAY_TXT { get; set; }
        public string TYPE_OF_2_DISPLAY_TXT { get; set; }
        public string PACKING_STYLE_DISPLAY_TXT { get; set; }
        public string PACKING_STYLE_REQUESTFORM { get; set; }
        public string PACK_SIZE_DISPLAY_TXT { get; set; }
        public string BRAND_DISPLAY_TXT { get; set; }
        public string BRAND_WARNING_DISPLAY_TXT { get; set; }
        public string DRAIN_WEIGHT_DISPLAY_TXT { get; set; }
        public string PLANT_REGISTERED_DISPLAY_TXT { get; set; }
        public string COMPANY_ADDRESS_DISPLAY_TXT { get; set; }
        //public string SYMBOL_DISPLAY_TXT { get; set; }
        public string PRODICUTION_PLANT_DISPLAY_TXT { get; set; }
        //public string PLANT_DISPLAY_TXT { get; set; }
        //public string FAO_ZONE_DISPLAY_TXT { get; set; }
        //public string CATCHING_AREA_DISPLAY_TXT { get; set; }
        public string CATCHING_PERIOD_DISPLAY_TXT { get; set; }
        public string CATCHING_METHOD_DISPLAY_TXT { get; set; }
        public string SCIENTIFIC_NAME_DISPLAY_TXT { get; set; }
        public string SPECIE_DISPLAY_TXT { get; set; }
        public string PMS_COLOUR_DISPLAY_TXT { get; set; }
        public string PROCESS_COLOUR_DISPLAY_TXT { get; set; }
        public string TOTAL_COLOUR_DISPLAY_TXT { get; set; }
        public string STYLE_OF_PRINTING_DISPLAY_TXT { get; set; }
        public string DIRECTION_OF_STICKER_DISPLAY_TXT { get; set; }
        public string PRINTING_STYLE_OF_PRIMARY_DISPLAY_TXT { get; set; }
        public string PRINTING_STYLE_OF_SECONDARY_DISPLAY_TXT { get; set; }
        public string FIRST_INFOGROUP_DISPLAY_TXT { get; set; }
        public string THREE_P_DISPLAY_TXT { get; set; }
        public string TWO_P_DISPLAY_TXT { get; set; }
        public string COUNTRY { get; set; }

        //public string NUTRITION_COMMENT { get; set; }
        //public string INGREDIENTS_COMMENT { get; set; }
        //public string ANALYSIS_COMMENT { get; set; }
        //public string HEALTH_CLAIM_COMMENT { get; set; }
        //public string NUTRIENT_CLAIM_COMMENT { get; set; }
        //public string SPECIES_COMMENT { get; set; }
        //public string CATCHING_AREA_COMMENT { get; set; }

        public string PA_DISPLAY_TXT { get; set; }
        public string PG_DISPLAY_TXT { get; set; }

        public string ARTWORK_NO { get; set; }

        public string MATCHING_WARNNING { get; set; }

        public int DECISION_DEFAULT { get; set; }

        public string IS_READY_CREATE_PO { get; set; }
        public string READY_CREATE_PO_VALIDATE_MSG { get; set; }
        public string RETRIVE_TYPE { get; set; }
        
        public string VENDOR_BY_MIGRATION_DISPLAY_TXT { get; set; }

        public bool CHECK_DIFFERNT_REQ_PA { get; set; }

        public int? ARTWORK_ITEM_ID_COPY { get; set; }

    }

    public class ART_WF_ARTWORK_PROCESS_PA_2_METADATA
    {
         
    }

    public class ART_WF_ARTWORK_PROCESS_PA_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_PA_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_PA_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_PA_2> data { get; set; }
    }
}
