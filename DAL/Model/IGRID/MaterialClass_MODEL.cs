using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class MaterialClass_MODEL
    {
        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }

        public string Fields { get; set; }
        public string unlockcol { get; set; }
    }
    public class MaterialClass_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public MaterialClass_MODEL data { get; set; }
    }
    public class MaterialClass_REQUEST_LIST : REQUEST_MODEL
    {
        public List<MaterialClass_MODEL> data { get; set; }
    }
    public class MaterialClass_RESULT : RESULT_MODEL
    {
        public List<MaterialClass_MODEL> data { get; set; }
    }
}
