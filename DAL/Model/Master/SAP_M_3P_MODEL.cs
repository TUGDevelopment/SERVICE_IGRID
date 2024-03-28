using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class SAP_M_3P_2 : SAP_M_3P
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string SEARCH_DISPLAY_TXT { get; set; }



        // by aof 202306 for CR#IGRID_REIM----ADD FIELD FOR IGID*@
        public string CODE { get; set; }
        public string CAN { get; set; }
        public string DESCRIPTION { get; set; }
        public string LIDTYPE { get; set; }
        public string CONTAINERTYPE { get; set; }
        public string DESCRIPTIONTYPE { get; set; }
        // by aof 202306 for CR#IGRID_REIM----ADD FIELD FOR IGID*@


    }

    public class SAP_M_3P_2_METADATA
    {
      
    }

    public class SAP_M_3P_REQUEST : REQUEST_MODEL
    {
        public SAP_M_3P_2 data { get; set; }
    }

    public class SAP_M_3P_RESULT : RESULT_MODEL
    {
        public List<SAP_M_3P_2> data { get; set; }
    }
}
