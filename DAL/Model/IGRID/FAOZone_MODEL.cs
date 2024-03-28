using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class FAOZone_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Inactive { get; set; }

        //public string IsCheckAuthorize { get; set; }
        //public string Authorize_ChangeMaster { get; set; }
    }
    public class FAOZone_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public FAOZone_MODEL data { get; set; }
    }
    public class FAOZone_REQUEST_LIST : REQUEST_MODEL
    {
        public List<FAOZone_MODEL> data { get; set; }
    }
    public class FAOZone_RESULT : RESULT_MODEL
    {
        public List<FAOZone_MODEL> data { get; set; }
    }
}
