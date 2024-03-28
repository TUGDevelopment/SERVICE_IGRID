using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class CatchingArea_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Inactive { get; set; }

   

    }
    public class CatchingArea_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public CatchingArea_MODEL data { get; set; }
    }
    public class CatchingArea_REQUEST_LIST : REQUEST_MODEL
    {
        public List<CatchingArea_MODEL> data { get; set; }
    }
    public class CatchingArea_RESULT : RESULT_MODEL
    {
        public List<CatchingArea_MODEL> data { get; set; }
    }
}
