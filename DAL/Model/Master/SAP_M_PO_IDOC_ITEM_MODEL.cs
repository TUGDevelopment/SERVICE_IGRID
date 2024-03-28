using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class SAP_M_PO_IDOC_ITEM_2 : SAP_M_PO_IDOC_ITEM
    {
        public bool FIRST_LOAD { get; set; }
    }

    public class SAP_M_PO_IDOC_ITEM_2_METADATA
    {
      
    }

    public class SAP_M_PO_IDOC_ITEM_REQUEST : REQUEST_MODEL
    {
        public SAP_M_PO_IDOC_ITEM_2 data { get; set; }
    }

    public class SAP_M_PO_IDOC_ITEM_RESULT : RESULT_MODEL
    {
        public List<SAP_M_PO_IDOC_ITEM_2> data { get; set; }
    }
}
