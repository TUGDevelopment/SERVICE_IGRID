using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class Symbol_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Inactive { get; set; }

    }
    public class Symbol_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public Symbol_MODEL data { get; set; }
    }
    public class Symbol_REQUEST_LIST : REQUEST_MODEL
    {
        public List<Symbol_MODEL> data { get; set; }
    }
    public class Symbol_RESULT : RESULT_MODEL
    {
        public List<Symbol_MODEL> data { get; set; }
    }
}
