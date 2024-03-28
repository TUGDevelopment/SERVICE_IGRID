using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class PackStyle_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public int Id { get; set; }
        public string PrimaryCode { get; set; }
        public string GroupStyle { get; set; }
        public string PackingStyle { get; set; }
        public string RefStyle { get; set; }
        public string PackSize { get; set; }
        public string BaseUnit { get; set; }
        public string TypeofPrimary { get; set; }
        public string Inactive { get; set; }

    }
    public class PackStyle_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public PackStyle_MODEL data { get; set; }
    }
    public class PackStyle_REQUEST_LIST : REQUEST_MODEL
    {
        public List<PackStyle_MODEL> data { get; set; }
    }
    public class PackStyle_RESULT : RESULT_MODEL
    {
        public List<PackStyle_MODEL> data { get; set; }
    }
}
