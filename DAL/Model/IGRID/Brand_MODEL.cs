using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class Brand_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string Inactive { get; set; }

}
    public class Brand_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public Brand_MODEL data { get; set; }
    }
    public class Brand_REQUEST_LIST : REQUEST_MODEL
    {
        public List<Brand_MODEL> data { get; set; }
    }
    public class Brand_RESULT : RESULT_MODEL
    {
        public List<Brand_MODEL> data { get; set; }
    }
}
