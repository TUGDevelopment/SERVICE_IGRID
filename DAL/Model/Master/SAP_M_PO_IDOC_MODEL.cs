using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class SAP_M_PO_IDOC_2 : SAP_M_PO_IDOC
    {
        public List<SAP_M_PO_IDOC_ITEM_2> ITEMS { get; set; }
        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public int CURRENT_USER_ID { get; set; }
        public bool FIRST_LOAD { get; set; }

        public string PO_NO2 { get; set; }
        public string GET_BY_CREATE_DATE_FROM { get; set; }
        public string GET_BY_CREATE_DATE_TO { get; set; }
    }

    public class SAP_M_PO_IDOC_2_METADATA
    {

    }

    public class SAP_M_PO_IDOC_REQUEST : REQUEST_MODEL
    {
        public SAP_M_PO_IDOC_2 data { get; set; }
    }

    public class SAP_M_PO_IDOC_RESULT : RESULT_MODEL
    {
        public List<SAP_M_PO_IDOC_2> data { get; set; }
    }
}
