using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    public partial class V_ART_WAREHOUSE_REPORT_2 : V_ART_WAREHOUSE_REPORT
    {
        public string FILE_NAME { get; set; }
        public string CREATED_BY_DISPLAY_TXT { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string NODE_ID_TXT { get; set; }
        public string TITLE { get; set; }

        public string REJECTION_CODE { get; set; }
        public string REJECTION_DESC { get; set; }
        public string BOM_ITEM_CUSTOM_1 { get; set; }

        public string GENERATE_EXCEL { get; set; }
        public bool IS_SHOW_FILE_PRINT_MASTER { get; set; }
    }

    public class V_ART_WAREHOUSE_REPORT_REQUEST : REQUEST_MODEL
    {
        public V_ART_WAREHOUSE_REPORT_2 data { get; set; }
    }

    public class V_ART_WAREHOUSE_REPORT_RESULT : RESULT_MODEL
    {
        public List<V_ART_WAREHOUSE_REPORT_2> data { get; set; }
    }
}