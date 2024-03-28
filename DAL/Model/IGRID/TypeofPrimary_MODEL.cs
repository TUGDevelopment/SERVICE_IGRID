using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class TypeofPrimary_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string MaterialGroup { get; set; }
        public string Inactive { get; set; }

    }
    public class TypeofPrimary_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public TypeofPrimary_MODEL data { get; set; }
    }
    public class TypeofPrimary_REQUEST_LIST : REQUEST_MODEL
    {
        public List<TypeofPrimary_MODEL> data { get; set; }
    }
    public class TypeofPrimary_RESULT : RESULT_MODEL
    {
        public List<TypeofPrimary_MODEL> data { get; set; }
    }
}
