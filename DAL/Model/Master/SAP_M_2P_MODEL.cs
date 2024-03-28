using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class SAP_M_2P_2 : SAP_M_2P
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string SEARCH_DISPLAY_TXT { get; set; }
        public int PRIMARY_TYPE_ID { get; set; }   // by aof  packing style 01/20/2021
    }

    public class SAP_M_2P_2_METADATA
    {
      
    }

    public class SAP_M_2P_REQUEST : REQUEST_MODEL
    {
        public SAP_M_2P_2 data { get; set; }
    }

    public class SAP_M_2P_RESULT : RESULT_MODEL
    {
        public List<SAP_M_2P_2> data { get; set; }
    }
}
