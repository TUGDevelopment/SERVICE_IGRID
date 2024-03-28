using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class CompanyAddress_MODEL
    {
        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string Inactive { get; set; }
        public string ProductCode { get; set; }
        public string RegisteredNo { get; set; }

    }
    public class CompanyAddress_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public CompanyAddress_MODEL data { get; set; }
    }
    public class CompanyAddress_REQUEST_LIST : REQUEST_MODEL
    {
        public List<CompanyAddress_MODEL> data { get; set; }
    }
    public class CompanyAddress_RESULT : RESULT_MODEL
    {
        public List<CompanyAddress_MODEL> data { get; set; }
    }
}
