using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class TypeOf_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string MaterialGroup { get; set; }
        public string MaterialType { get; set; }
        public string DescriptionText { get; set; }
        public string Inactive { get; set; }



        public string Brand { get; set; }

    }
    public class TypeOf_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public TypeOf_MODEL data { get; set; }
    }
    public class TypeOf_REQUEST_LIST : REQUEST_MODEL
    {
        public List<TypeOf_MODEL> data { get; set; }
    }
    public class TypeOf_RESULT : RESULT_MODEL
    {
        public List<TypeOf_MODEL> data { get; set; }
    }
}
