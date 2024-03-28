using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class TransApprove_Model
    {
        public int Id { get; set; }
        public int MatDoc { get; set; }
        public string StatusApp { get; set; }
        public string Condition { get; set; }
        public string fn { get; set; }
        public string ActiveBy { get; set; }
        public DateTime SubmitDate { get; set; }
        public string levelApp { get; set; }



        //-----------by aof---------------------------
        public string CreateBy { get; set; }
        public string Assignee { get; set; }
        public string IsStep_PA { get; set; }
        public string IsStep_PG { get; set; }
        public string IsStep_PG_Assign { get; set; }
        public string IsStep_PA_Approve { get; set; }
        public string IsStep_PG_Approve { get; set; }
        public string IsStep_InfoGroup { get; set; }
        public string IsCancel { get; set; }
        public string IsComplete { get; set; }
        public string CurrentUser { get; set; }
        public string CurrentUser_Fn { get; set; }
        public int CntCompleteInfoGroup { get; set; }
        //-----------by aof---------------------------
    }

    public class TransApprove_REQUEST : REQUEST_MODEL
    {
        public TransApprove_Model data { get; set; }
    }
    public class TransApprove_REQUEST_LIST : REQUEST_MODEL
    {
        public List<TransApprove_Model> data { get; set; }
    }
    public class TransApprove_RESULT : RESULT_MODEL
    {
        public List<TransApprove_Model> data { get; set; }
      
    }
}
