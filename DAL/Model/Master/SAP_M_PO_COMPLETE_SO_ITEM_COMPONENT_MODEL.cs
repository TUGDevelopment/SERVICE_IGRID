using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2 : SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
    {
        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }
    }

    public class SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2_METADATA
    {
      
    }

    public class SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_REQUEST : REQUEST_MODEL
    {
        public SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2 data { get; set; }
    }

    public class SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_RESULT : RESULT_MODEL
    {
        public List<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2> data { get; set; }
    }
}
