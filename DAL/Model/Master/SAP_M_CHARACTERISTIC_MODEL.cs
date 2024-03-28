using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    [MetadataType(typeof(SAP_M_CHARACTERISTIC_2_METADATA))]
    public partial class SAP_M_CHARACTERISTIC_2 : SAP_M_CHARACTERISTIC
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public int MATERIAL_GROUP_ID { get; set; }
        public string PACKAGING_TYPE_VALUE { get; set; }
        public int PACKAGING_TYPE_ID { get; set; }
        public string PRODUCT_TYPE { get; set; }

        public string MATERIAL_NO { get; set; }
        public string REF_MATERIAL_NO { get; set; }
        public string WHERE_NOT_IN_CHARACTERISTIC_ID { get; set; }   // by aof 202306 for CR#IGRID_REIM---PA*@ pattern 120,121,122,123
        public string STR_PRODUCT_CODE { get; set; }   // by aof 202306 for CR#IGRID_REIM---PA*@
        public int REGISTER_CHARACTERISTIC_ID { get; set; }   // by aof 202306 for CR#IGRID_REIM---PA*@
        public string BRAND_DESCRIPTION { get; set; }  // by aof 202306 for CR#IGRID_REIM_SPRINT2*@

    }

    public class SAP_M_CHARACTERISTIC_2_METADATA
    {
         

    }

    public class SAP_M_CHARACTERISTIC_REQUEST : REQUEST_MODEL
    {
        public SAP_M_CHARACTERISTIC_2 data { get; set; }
    }

    public class SAP_M_CHARACTERISTIC_RESULT : RESULT_MODEL
    {
        public List<SAP_M_CHARACTERISTIC_2> data { get; set; }
    }
}
