using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class Assign_MODEL : IGRID_AUTHROLIZE_CHANGE
    {        public string ID { get; set; }
        public string fn { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string Email { get; set; }
        public string Authorize_ChangeMaster { get; set; }
        public string SAP_EDPUsername { get; set; }
        public string Inactive { get; set; }

    }
    public class Assign_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public Assign_MODEL data { get; set; }
    }
    public class Assign_REQUEST_LIST : REQUEST_MODEL
    {
        public List<Assign_MODEL> data { get; set; }
    }
    public class Assign_RESULT : RESULT_MODEL
    {
        public List<Assign_MODEL> data { get; set; }
        public string haveAuthrolizeEditMaster { get; set; }
    }
}
