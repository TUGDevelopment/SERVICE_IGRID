using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class SustainPlastic_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public string ID { get; set; }
        public string value { get; set; }
        public string Description { get; set; }
        public string MaterialGroup { get; set; }
        public string Inactive { get; set; }
        public string DISPLAY_TXT { get; set; }

    }
    public class SustainPlastic_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public SustainPlastic_MODEL data { get; set; }
    }
    public class SustainPlastic_REQUEST_LIST : REQUEST_MODEL
    {
        public List<SustainPlastic_MODEL> data { get; set; }
    }
    public class SustainPlastic_RESULT : RESULT_MODEL
    {
        public List<SustainPlastic_MODEL> data { get; set; }
    }
}
