using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class PrimaryType_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Inactive { get; set; }

    }
    public class PrimaryType_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public PrimaryType_MODEL data { get; set; }
    }
    public class PrimaryType_REQUEST_LIST : REQUEST_MODEL
    {
        public List<PrimaryType_MODEL> data { get; set; }
    }
    public class PrimaryType_RESULT : RESULT_MODEL
    {
        public List<PrimaryType_MODEL> data { get; set; }
    }
}
