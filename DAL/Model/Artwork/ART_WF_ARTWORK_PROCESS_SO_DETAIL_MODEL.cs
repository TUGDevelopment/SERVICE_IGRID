using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 : ART_WF_ARTWORK_PROCESS_SO_DETAIL
    {
        public int CURRENT_USER_ID { get; set; }
        public string SOLD_TO_NAME { get; set; }
        public string SHIP_TO_NAME { get; set; }
        public string SALES_ORG { get; set; }
        public string SO_NUMBER { get; set; }
        public string SO_ITEM_NO { get; set; }
        public string BRAND { get; set; }
        //public string MATERIAL_NO { get; set; }
        public string MATERIAL_DESC { get; set; }
        public string PORT { get; set; }
        public string PRODUCTION_PLANT { get; set; }
        public string ORDER_BOM_NO { get; set; }
        public int ORDER_BOM_ID { get; set; }
        public string ORDER_BOM_DESC { get; set; }
        public string BOM_ITEM_CUSTOM_1 { get; set; }
        public string ITEM_CUSTOM_1 { get; set; }
        public string COUNTRY { get; set; }
        public string QUANTITY { get; set; }
        public string STOCK_PO { get; set; }
        public Nullable<DateTime> RDD { get; set; }
        public string GROUPING_DISPLAY_TXT { get; set; }
        public string GROUPING { get; set; }
        public string ASSIGN_ID { get; set; }
        public string VALIDATE_MESSAGE { get; set; }
        public string FOC_ITEM { get; set; }
        public string GROUPINGTEMP { get; set; }
        public string ORDER_QTY { get; set; }
        public string CREATE_DATE_DISPLAY_TXT { get; set; }
        public string ETD_DATE_FROM { get; set; }
        public string ETD_DATE_TO { get; set; }
        public string RDD_DISPLAY_TXT { get; set; }
        public string LC { get; set; }
        public string WAREHOUSE_TEXT { get; set; }
        public string SOLD_TO_PARTY { get; set; }
        public string SHIP_TO_PARTY { get; set; }
        public string MATERIAL { get; set; }
        public string OLD_MATERIAL { get; set; }
        public string PLANT { get; set; }
        public string DRAIN_WEIGHT { get; set; }
        public string INSP_MEMO { get; set; }
        public string REASON_REJECTION { get; set; }
        public string BRAND_ADDITIONAL { get; set; }
        public string SOLD_TO_PO { get; set; }
        public string SHIP_TO_PO { get; set; }
        public string VIA { get; set; }
        public string IN_TRANSIT_TO { get; set; }
        public string PAYMENT_TERM { get; set; }
        public string LC_NO { get; set; }
        public string EXP { get; set; }
        public string GENERAL_TEXT { get; set; }
        public string PACK_SIZE { get; set; }



        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//
        public int TEMP_RUNNING_ID { get; set; }
        public ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2 ASSIGN_SO_HEADER { get; set; }

        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//

    }

    public class ART_WF_ARTWORK_PROCESS_SO_DETAIL_2_METADATA
    {

    }


    public class ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2> data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2> data { get; set; }

        public List<ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2> plants { get; set; }   //by aof for iTail AssignSaleOrderProductionPlant
    }
}
