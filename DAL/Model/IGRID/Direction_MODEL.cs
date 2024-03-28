using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class Direction_MODEL
    {
        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string MaterialGroup { get; set; }
        public string Inactive { get; set; }

    }
    public class Direction_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public Direction_MODEL data { get; set; }
    }
    public class Direction_REQUEST_LIST : REQUEST_MODEL
    {
        public List<Direction_MODEL> data { get; set; }
    }
    public class Direction_RESULT : RESULT_MODEL
    {
        public List<Direction_MODEL> data { get; set; }
    }
}
