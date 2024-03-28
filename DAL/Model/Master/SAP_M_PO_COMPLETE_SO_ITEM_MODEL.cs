using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class SAP_M_PO_COMPLETE_SO_ITEM_2 : SAP_M_PO_COMPLETE_SO_ITEM
    {
        public List<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2> COMPONENTS { get; set; }

        public string XECM_NET_WEIGHT { get; set; }
        public string XECM_DRAINED_WEIGHT { get; set; }
        public string XECM_PRIMARY_SIZE { get; set; }
        public string XECM_CONTAINER_TYPE { get; set; }
        public string XECM_LID_TYPE { get; set; }
        public string XECM_PACKING_STYLE { get; set; }
        public string XECM_PACK_SIZE { get; set; }

        public string WAREHOUSE_TEXT { get; set; }
    }

    public class SAP_M_PO_COMPLETE_SO_ITEM_2_METADATA
    {
      
    }

    public class SAP_M_PO_COMPLETE_SO_ITEM_REQUEST : REQUEST_MODEL
    {
        public SAP_M_PO_COMPLETE_SO_ITEM_2 data { get; set; }
    }

    public class SAP_M_PO_COMPLETE_SO_ITEM_RESULT : RESULT_MODEL
    {
        public List<SAP_M_PO_COMPLETE_SO_ITEM_2> data { get; set; }
    }
}
