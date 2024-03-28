using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class Vendor_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Inactive { get; set; }

    }
    public class Vendor_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public Vendor_MODEL data { get; set; }
    }
    public class Vendor_REQUEST_LIST : REQUEST_MODEL
    {
        public List<Vendor_MODEL> data { get; set; }
    }
    public class Vendor_RESULT : RESULT_MODEL
    {
        public List<Vendor_MODEL> data { get; set; }
    }
}
