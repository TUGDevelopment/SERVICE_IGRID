using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public partial class ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2 : ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
    {
        public string BRAND_DISPLAY_TXT { get; set; }

        //---------------------------- tuning performance 2022 by aof---------------------------------/
        public string SOLD_TO_DISPLAY_TXT { get; set; }
        public string SHIP_TO_DISPLAY_TXT { get; set; }
        public string PRODUCTION_PLANT_DISPLAY_TXT { get; set; }   //20230121_3V_SOREPAT INC-93118
        public string PRODUCT_TYPE { get; set; }   //20230121_3V_SOREPAT INC-93118
        //---------------------------- tuning performance 2022 by aof---------------------------------//
    }

    public class ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2_METADATA
    {

    }

    public class ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2> data { get; set; }
    }
}
