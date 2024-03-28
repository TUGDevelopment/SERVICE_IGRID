using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class RollSheet_MODEL
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string MaterialGroup { get; set; }

    }
    public class RollSheet_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public RollSheet_MODEL data { get; set; }
    }
    public class RollSheet_REQUEST_LIST : REQUEST_MODEL
    {
        public List<RollSheet_MODEL> data { get; set; }
    }
    public class RollSheet_RESULT : RESULT_MODEL
    {
        public List<RollSheet_MODEL> data { get; set; }
    }
}
