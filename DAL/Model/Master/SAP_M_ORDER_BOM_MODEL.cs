using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class SAP_M_ORDER_BOM_2 : SAP_M_ORDER_BOM
    {
        public string ALL_DATA { get; set; }
        public int ARTWORK_SUB_ID { get; set; }
        public int ARTWORK_REQUEST_ID { get; set; }
        //public int PRODUCT_CODE_ID { get; set; }  //ticket#437764 added by aof on 30/03/2021

        public string BRAND_DISPLAY_TXT { get; set; }
        public string SHIP_TO_DISPLAY_TXT { get; set; }
        public string SOLD_TO_DISPLAY_TXT { get; set; }

        public bool FIRST_LOAD { get; set; }


        public string SEARCH_MAT_FG { get; set; }
        public string SEARCH_MAT_PK { get; set; }
        public int? SEARCH_BRAND_ID { get; set; }
        public int? SEARCH_SHIP_TO_ID { get; set; }
        public int? SEARCH_SOLD_TO_ID { get; set; }


    }

    public class SAP_M_ORDER_BOM_2_METADATA
    {

    }

    public class SAP_M_ORDER_BOM_REQUEST : REQUEST_MODEL
    {
        public SAP_M_ORDER_BOM_2 data { get; set; }
    }

    public class SAP_M_ORDER_BOM_RESULT : RESULT_MODEL
    {
        public List<SAP_M_ORDER_BOM_2> data { get; set; }
    }


}
