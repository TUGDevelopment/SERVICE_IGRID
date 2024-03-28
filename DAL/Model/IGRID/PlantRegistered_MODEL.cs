using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class PlantRegistered_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public int ID { get; set; }
        public string RegisteredNo { get; set; }
        public string Address { get; set; }
        public string Plant { get; set; }
        public string Inactive { get; set; }
        public string DISPLAY_TXT { get; set; }

        public string STR_PRODUCT_CODE { get; set; }  // by aof

    }
    public class PlantRegistered_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public PlantRegistered_MODEL data { get; set; }
    }
    public class PlantRegistered_REQUEST_LIST : REQUEST_MODEL
    {
        public List<PlantRegistered_MODEL> data { get; set; }
    }
    public class PlantRegistered_RESULT : RESULT_MODEL
    {
        public List<PlantRegistered_MODEL> data { get; set; }
    }
}
