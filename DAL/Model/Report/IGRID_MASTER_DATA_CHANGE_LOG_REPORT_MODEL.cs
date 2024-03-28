using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL
    {
        public int CHANGED_ID { get; set; }
        public string SHORTNAME { get; set; }
        public string CHANGED_TABNAME { get; set; }
        public string CHANGED_CHARNAME { get; set; }
        public string CHANGED_ACTION { get; set; }
        public string OLD_ID { get; set; }
        public string OLD_DESCRIPTION { get; set; }
        public string ID { get; set; }
        public string DESCRIPTION { get; set; }
        public string CHANGED_BY { get; set; }
        public string CHANGED_REASON { get; set; }
        public string CHANGED_ON { get; set; }
        //primarySize
        public string Code { get; set; }
        public string Can { get; set; }
        //public string Description { get; set; }
        public string LidType { get; set; }
        public string ContainerType { get; set; }
        public string DescriptionType { get; set; }
        //PackingStyle
        public string PrimaryCode { get; set; }
        public string GroupStyle { get; set; }
        public string PackingStyle { get; set; }
        public string RefStyle { get; set; }
        public string PackSize { get; set; }
        public string BaseUnit { get; set; }
        public string TypeofPrimary { get; set; }
        //PlantRegisteredNo
        public string RegisteredNo { get; set; }
        public string Address { get; set; }
        public string Plant { get; set; }
        public string Inactive { get; set; }
        public string FIRST_LOAD { get; set; }
        public string SEARCH_KEYWORD { get; set; }
        public string SEARCH_MASTER { get; set; }
        public string SEARCH_USER { get; set; }
        public string SEARCH_TYPE { get; set; }
        public string SEARCH_DATE_FROM { get; set; }
        public string SEARCH_DATE_TO { get; set; }


    }


    public class IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL_REQUEST : REQUEST_MODEL
    {
        public IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL data { get; set; }

    }

    public class IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL_RESULT : RESULT_MODEL
    {

        public List<IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL> data { get; set; }
        //public int ORDER_COLUMN { get; set; }
    }

    //cbb for binding with combobox
    public class IGRID_CBB_DATA_MODEL
    {
        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string SEARCH_TYPE { get; set; }
    }

    public class IGRID_CBB_DATA_MODEL_REQUEST : REQUEST_MODEL
    {
        public IGRID_CBB_DATA_MODEL data { get; set; }

    }

    public class IGRID_CBB_DATA_MODEL_RESULT : RESULT_MODEL
    {

        public List<IGRID_CBB_DATA_MODEL> data { get; set; }
        //public int ORDER_COLUMN { get; set; }
    }


}
