using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    public partial class V_ART_WF_DASHBOARD_ARTWORK_2 : V_ART_WF_DASHBOARD_ARTWORK
    {
        public string PRODUCT_CODE_SET { get; set; }
        public string COUNTRY_CODE_SET { get; set; }

        public string CREATE_BY_ARTWORK_REQUEST_DISPLAY_TXT { get; set; }
        public string CREATE_BY_PROCESS_DISPLAY_TXT { get; set; }
        public int USER_ID { get; set; }

        public string GET_BY_CREATE_DATE_FROM { get; set; }
        public string GET_BY_CREATE_DATE_TO { get; set; }


        public int CNT_TOTAL_SUB_WF { get; set; }
        public int CNT_TOTAL_SUB_WF_END { get; set; }
        public int CNT_TOTAL_SUB_WF_NOT_END { get; set; }
        public string WAITING_STEP { get; set; }
        public string END_STEP { get; set; }
        public string PLANT { get; set; }
        public string CURRENT_ASSIGN { get; set; }
        public bool PP_SEND_BACK { get; set; }
        public string PP_SEND_BACK_COMMENT { get; set; }


        public string PACKING_TYPE_DISPLAY_TXT { get; set; } //#INC-65918 by aof 20220711
    }

    public class V_ART_WF_DASHBOARD_ARTWORK_REQUEST : REQUEST_MODEL
    {
        public V_ART_WF_DASHBOARD_ARTWORK_2 data { get; set; }
    }

    public class V_ART_WF_DASHBOARD_ARTWORK_RESULT : RESULT_MODEL
    {
        public List<V_ART_WF_DASHBOARD_ARTWORK_2> data { get; set; }
    }
}