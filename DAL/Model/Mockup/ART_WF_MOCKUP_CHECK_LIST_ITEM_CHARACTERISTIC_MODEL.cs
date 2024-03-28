using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    [MetadataType(typeof(SAP_M_CHARACTERISTIC_ITEM_2_METADATA))]
    public partial class SAP_M_CHARACTERISTIC_ITEM_2 : SAP_M_CHARACTERISTIC_ITEM
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
    }

    public class SAP_M_CHARACTERISTIC_ITEM_2_METADATA
    {
         

    }

    public class SAP_M_CHARACTERISTIC_ITEM_2_REQUEST : REQUEST_MODEL
    {
        public SAP_M_CHARACTERISTIC_ITEM_2 data { get; set; }
    }

    public class SAP_M_CHARACTERISTIC_ITEM_2_RESULT : RESULT_MODEL
    {
        public List<SAP_M_CHARACTERISTIC_ITEM_2> data { get; set; }
    }
}
