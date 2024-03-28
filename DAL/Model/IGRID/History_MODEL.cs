using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class History_MODEL
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime ModifyOn{ get; set; }
        public string ActiveBy { get; set; }
        public string Result { get; set; }

    }
    public class History_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public History_MODEL data { get; set; }
    }
    public class History_REQUEST_LIST : REQUEST_MODEL
    {
        public List<History_MODEL> data { get; set; }
    }
    public class History_RESULT : RESULT_MODEL
    {
        public List<History_MODEL> data { get; set; }
    }
}
