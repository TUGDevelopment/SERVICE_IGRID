using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class V_ART_ASSIGNED_SO_2 : V_ART_ASSIGNED_SO
    {
        public bool FIRST_LOAD { get; set; }
        public Nullable<int> COMPANY_ID_2 { get; set; }
        public string PIC_DISPLAY_TXT { get; set; }
        public string SOLD_TO_DISPLAY_TXT { get; set; }
        public string SHIP_TO_DISPLAY_TXT { get; set; }
        public string READY_CREATE_PO_DISPLAY_TXT { get; set; }
        public string STOCK_DISPLAY_TXT { get; set; }
        public string QUANTITY_DISPLAY_TXT { get; set; }
        
        public bool WORKFLOW_CREATED { get; set; }
        public bool FLAG_SEND_TO_PP { get; set; }
        public bool SEND_TO_PP { get; set; }
        public bool PO_CREATED { get; set; }

        public string CURRENT_WF_STATUS { get; set; }

        public string SEND_TO_PP_DISPLAY_TXT { get; set; }
        public string PO_CREATED_DISPLAY_TXT { get; set; }

        public string SEARCH_BRAND_NAME { get; set; }
        public string SEARCH_COUNTRY_NAME { get; set; }
        public string SEARCH_SOLD_TO_NAME { get; set; }
        public string SEARCH_SHIP_TO_NAME { get; set; }
        public string SEARCH_PRODUCT_CODE { get; set; }
        public string SEARCH_BOM_COMPONENT { get; set; }
        public string SEARCH_PIC { get; set; }
        public int SEARCH_PIC_ID { get; set; }

        public string SEARCH_RDD_DATE_FROM { get; set; }
        public string SEARCH_RDD_DATE_TO { get; set; }
        public string SEARCH_SO_CREATE_DATE_FROM { get; set; }
        public string SEARCH_SO_CREATE_DATE_TO { get; set; }
        public string SEARCH_SO_ITEM_FROM { get; set; }
        public string SEARCH_SO_ITEM_TO { get; set; }
        public string SEARCH_COMPANY { get; set; }

        public string GENERATE_EXCEL { get; set; }
        public string CHECK_WF { get; set; }
    }

    public class V_ART_ASSIGNED_SO_REQUEST : REQUEST_MODEL
    {
        public V_ART_ASSIGNED_SO_2 data { get; set; }
    }

    public class V_ART_ASSIGNED_SO_RESULT : RESULT_MODEL
    {
        public List<V_ART_ASSIGNED_SO_2> data { get; set; }
    }
}
