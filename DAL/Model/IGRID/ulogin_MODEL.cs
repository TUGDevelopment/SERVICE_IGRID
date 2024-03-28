using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class ulogin_MODEL : IGRID_AUTHROLIZE_CHANGE
    {
        public int Id { get; set; }
        public string user_name { get; set; }
        public string fn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        //public string Authorize_ChangeMaster { get; set; }
        public string SAP_EDPUsername { get; set; }
        public string Inactive { get; set; }



        public int Matdoc { get; set; }
        public List<TransApprove_Model> ListTransApproves { get; set; }

    }
    public class ulogin_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public ulogin_MODEL data { get; set; }

     

}
    public class ulogin_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ulogin_MODEL> data { get; set; }
    }
    public class ulogin_RESULT : RESULT_MODEL
    {
        public List<ulogin_MODEL> data { get; set; }
        public string haveAuthrolizeEditMaster { get; set; }
    }
}
