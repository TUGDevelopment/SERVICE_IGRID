using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class StyleofPrinting_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string MaterialGroup { get; set; }
        public string Inactive { get; set; }

    }
    public class StyleofPrinting_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public StyleofPrinting_MODEL data { get; set; }
    }
    public class StyleofPrinting_REQUEST_LIST : REQUEST_MODEL
    {
        public List<StyleofPrinting_MODEL> data { get; set; }
    }
    public class StyleofPrinting_RESULT : RESULT_MODEL
    {
        public List<StyleofPrinting_MODEL> data { get; set; }
    }
}
