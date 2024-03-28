using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class SEARCH_DIE_LINE
    {
        public string CHECK_LIST_NO { get; set; }
        public int CHECK_LIST_ID { get; set; }
        public int MOCKUP_ID { get; set; }
        public int MOCKUP_SUB_ID { get; set; }
        public string MOCKUP_NO { get; set; }

        public string PRIMARY_TYPE_DISPLAY_TXT { get; set; }
        public string PRIMARY_SIZE_DISPLAY_TXT { get; set; }
        public string PACK_SIZE_DISPLAY_TXT { get; set; }
        public string PACKING_STYLE_DISPLAY_TXT { get; set; }
        public string CONTAINER_TYPE_DISPLAY_TXT { get; set; }
        public string LID_TYPE_DISPLAY_TXT { get; set; }

        public string PACKAGING_TYPE_DISPLAY_TXT { get; set; }
        public string VENDOR_DISPLAY_TXT { get; set; }
        public string ACCESSORIES { get; set; }

        public string SHEET_SIZE { get; set; }
        public string PRINT_SYSTEM { get; set; }
        public string DIMENSION_OF { get; set; }
        public string FINAL_INFO { get; set; }
        public string REMARK_PG { get; set; }
        public string ID_MM { get; set; }
        public string DIMENSION_OF_DISPLAY_TXT { get; set; }
        
        public Nullable<int> DI_CUT { get; set; }
        public string DI_CUT_DISPLAY_TXT { get; set; }
        public string ROLL_SHEET { get; set; }
        
        public Nullable<int> GRADE_OF { get; set; }
        public string GRADE_OF_DISPLAY_TXT { get; set; }
        public string SIZE_DISPLAY_TXT { get; set; }

        public Nullable<int> FLUTE { get; set; }
        public string FLUTE_DISPLAY_TXT { get; set; }
        public string STYLE_DISPLAY_TXT { get; set; }
        public string STATUS_DISPLAY_TXT { get; set; }
        public string NUMBER_OF_COLOR_DISPLAY_TXT { get; set; }

        public int? PRIMARY_TYPE_ID { get; set; }
        public int? PRIMARY_SIZE_ID { get; set; }
        public int? PACK_SIZE_ID { get; set; }
        public int? PACKING_STYLE_ID { get; set; }
        public int? PACKING_TYPE_ID { get; set; }
        public int? ARTWORK_SUB_ID { get; set; }

        public int DIE_LINE_MOCKUP_ID { get; set; }

        public string STYLE_OF_PRINTING_DISPLAY_TXT { get; set; }

        public string CUSTOMER_SIZE_REMARK { get; set; }
        public string CUSTOMER_SPEC_REMARK { get; set; }
        public string CUSTOMER_NOMINATES_VENDOR_REMARK { get; set; }

        public string CUSTOMER_NOMINATES_VENDOR { get; set; }
        public string CUSTOMER_SPEC { get; set; }
        public string CUSTOMER_SIZE { get; set; }


        //---------by aof 20220118 for CR sustain-- - start

        public Nullable<int> SUSTAIN_MATERIAL { get; set; }
        public Nullable<int> PLASTIC_TYPE { get; set; }
        public string REUSEABLE { get; set; }
        public string RECYCLABLE { get; set; }
        public string COMPOSATABLE { get; set; }
        public Nullable<int> RECYCLE_CONTENT { get; set; }
        public string CERT { get; set; }
        public Nullable<int> CERT_SOURCE { get; set; }
        public Nullable<decimal> PKG_WEIGHT { get; set; }
        public string SUSTAIN_OTHER { get; set; }

        public string SUSTAIN_MATERIAL_DISPLAY_TXT { get; set; }
        public string PLASTIC_TYPE_DISPLAY_TXT { get; set; }
        public string CERT_SOURCE_DISPLAY_TXT { get; set; }
        //---------by aof 20220118 for CR sustain-- - end



    }

    public class SEARCH_DIE_LINE_METADATA
    {

    }

    public class SEARCH_DIE_LINE_REQUEST : REQUEST_MODEL
    {
        public SEARCH_DIE_LINE data { get; set; }
    }

    public class SEARCH_DIE_LINE_RESULT : RESULT_MODEL
    {
        public List<SEARCH_DIE_LINE> data { get; set; }
    }
}
