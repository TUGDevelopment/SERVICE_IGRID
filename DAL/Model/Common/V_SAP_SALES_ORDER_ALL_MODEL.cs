using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    public partial class V_SAP_SALES_ORDER_ALL_2 : V_SAP_SALES_ORDER_ALL
    {
        public string GROUPINGTEMP { get; set; }
        public string GROUPING { get; set; }
        public string GROUPING_DISPLAY_TXT { get; set; }
        public string SOLD_TO_DISPLAY_TXT { get; set; }
        public string RECHECK_ARTWORK { get; set; }
        public string SHIP_TO_DISPLAY_TXT { get; set; }
        public string BRAND_DISPLAY_TXT { get; set; }
        public string ADDITIONAL_BRAND_DISPLAY_TXT { get; set; }

        public int CURRENT_USER_ID { get; set; }
        public string GET_BY_CREATE_DATE_FROM { get; set; }
        public string GET_BY_CREATE_DATE_TO { get; set; }

        public string GET_BY_RDD_FROM { get; set; }
        public string GET_BY_RDD_TO { get; set; }
        public string GET_BY_PACKAGING_TYPE { get; set; }
        public string GET_BY_SOLD_TO { get; set; }
        public string GET_BY_SHIP_TO { get; set; }
        public string GET_BY_BRAND { get; set; }
        public bool FIRST_LOAD { get; set; }
        public int GROUP_MIN_ROW { get; set; }
        public int GROUP_MAX_ROW { get; set; }
        public int SELECTED_GROUP { get; set; }
    }

    public class V_SAP_SALES_ORDER_ALL_REQUEST : REQUEST_MODEL
    {
        public V_SAP_SALES_ORDER_ALL_2 data { get; set; }
    }

    public class V_SAP_SALES_ORDER_ALL_RESULT : RESULT_MODEL
    {
        public List<V_SAP_SALES_ORDER_ALL_2> data { get; set; }
    }
}
