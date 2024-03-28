using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class ProcessColour_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string MaterialGroup { get; set; }
        public string Inactive { get; set; }

    }
    public class ProcessColour_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public ProcessColour_MODEL data { get; set; }
    }
    public class ProcessColour_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ProcessColour_MODEL> data { get; set; }
    }
    public class ProcessColour_RESULT : RESULT_MODEL
    {
        public List<ProcessColour_MODEL> data { get; set; }
    }
}
