using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAL.Model
{
    public class IGRID_OVERALL_MASTER_REPORT_MODEL
    {
        public int ID { get; set; }
        public string IDS { get; set; }
        public string DESCRIPTION { get; set; }
        public string PRODUCTGROUP { get; set; }

        public string CODE { get; set; }
        public string CAN { get; set; }
        public string LIDTYPE { get; set; }
        public string CONTRAINERTYPE { get; set; }
        public string DESCRIPTIONTYPE { get; set; }

        public string MATERIALGROUP { get; set; }
        public string PRIMARYCODE { get; set; }
        public string GROUPSTYLE { get; set; }
        public string PACKINGSTYLE { get; set; }
        public string REFSTYLE { get; set; }
        public string PACKSIZE { get; set; }
        public string BASEUNIT { get; set; }
        public string TYPEOFPRIMARY { get; set; }

        public string REGISTEREDNO { get; set; }
        public string ADDRESS { get; set; }
        public string PLANT { get; set; }  
       

        public string MATERIALTYPE { get; set; }
        public string DESCRIPTIONTEXT { get; set; }

        public string PRODUCT_GROUP { get; set; }
        public string PRODUCT_GROUPDESC { get; set; }
        public string PRD_PLANT { get; set; }

       

        public string USER_NAME { get; set; }
        public string PASSWORD { get; set; }
        public string FN { get; set; }
        public string USERLEVEL { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string EMAIL { get; set; }
        public string AUTHORIZE_CHANGEMASTER { get; set; }
        public string SAP_EDPUSERNAME { get; set; }

        public string NAME { get; set; }
        public string WHNUMBER { get; set; }
        public string STORAGETYPE { get; set; }
        public int LE_QTY { get; set; }
        public string STORAGE_UNITTYPE { get; set; }

        public string VALUE { get; set; }
        public string INACTIVE { get; set; }   //TOTAL 36 FIELDS


        public string FIRST_LOAD { get; set; }
        public string SEARCH_MASTER { get; set; }
        public string SEARCH_KEYWORD { get; set; }
        public string SEARCH_STATUS { get; set; }

    }


    public class IGRID_OVERALL_MASTER_REPORT_MODEL_REQUEST : REQUEST_MODEL
    {
        public IGRID_OVERALL_MASTER_REPORT_MODEL data { get; set; }

    }

    public class IGRID_OVERALL_MASTER_REPORT_MODEL_RESULT : RESULT_MODEL
    {

        public List<IGRID_OVERALL_MASTER_REPORT_MODEL> data { get; set; }
     
    }
}
