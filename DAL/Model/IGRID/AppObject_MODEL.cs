using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class AppObject_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public string ID { get; set; }
        public string ActiveBy { get; set; }
        public string Description { get; set; }
        public string fn { get; set; }
        public string StatusApp { get; set; }
        public string Remark { get; set; }
        public string event_log { get; set; }
    }
    public class AppObject_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public AppObject_MODEL data { get; set; }
    }
    public class AppObject_REQUEST_LIST : REQUEST_MODEL
    {
        public List<AppObject_MODEL> data { get; set; }
    }
    public class AppObject_RESULT : RESULT_MODEL
    {
        public List<AppObject_MODEL> data { get; set; }
    }
}
