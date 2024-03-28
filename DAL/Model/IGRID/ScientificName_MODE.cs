using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class ScientificName_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string Inactive { get; set; }

    }
    public class ScientificName_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public ScientificName_MODEL data { get; set; }
    }
    public class ScientificName_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ScientificName_MODEL> data { get; set; }
    }
    public class ScientificName_RESULT : RESULT_MODEL
    {
        public List<ScientificName_MODEL> data { get; set; }
    }
}
