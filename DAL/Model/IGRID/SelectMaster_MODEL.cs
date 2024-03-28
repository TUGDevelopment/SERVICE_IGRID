using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class SelectMaster_MODEL
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }

    }
    public class SelectMaster_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public SelectMaster_MODEL data { get; set; }
    }
    public class SelectMaster_REQUEST_LIST : REQUEST_MODEL
    {
        public List<SelectMaster_MODEL> data { get; set; }
    }
    public class SelectMaster_RESULT : RESULT_MODEL
    {
        public List<SelectMaster_MODEL> data { get; set; }
    }
}
