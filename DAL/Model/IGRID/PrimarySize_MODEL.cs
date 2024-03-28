using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class PrimarySize_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Can { get; set; }
        public string Description { get; set; }
        public string LidType { get; set; }
        public string ContainerType { get; set; }
        public string DescriptionType { get; set; }
        public string Inactive { get; set; }

        public bool IsTop1 { get; set; }

    }
    public class PrimarySize_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public PrimarySize_MODEL data { get; set; }
    }
    public class PrimarySize_REQUEST_LIST : REQUEST_MODEL
    {
        public List<PrimarySize_MODEL> data { get; set; }
    }
    public class PrimarySize_RESULT : RESULT_MODEL
    {
        public List<PrimarySize_MODEL> data { get; set; }
    }
}
