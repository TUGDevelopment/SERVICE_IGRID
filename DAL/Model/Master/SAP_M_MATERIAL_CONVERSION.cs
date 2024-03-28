using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class SAP_M_MATERIAL_CONVERSION_2 : SAP_M_MATERIAL_CONVERSION
    {
      
    }

    public class SAP_M_MATERIAL_CONVERSION_2_METADATA
    {
      
    }

    public class SAP_M_MATERIAL_CONVERSION_REQUEST : REQUEST_MODEL
    {
        public SAP_M_MATERIAL_CONVERSION_2 data { get; set; }
    }

    public class SAP_M_MATERIAL_CONVERSION_REQUEST_LIST : REQUEST_MODEL
    {
        public List<SAP_M_MATERIAL_CONVERSION_2> data  { get; set; }
    }

    public class SAP_M_MATERIAL_CONVERSION_RESULT : RESULT_MODEL
    {
        public List<SAP_M_MATERIAL_CONVERSION_2> data { get; set; }
    }
}
