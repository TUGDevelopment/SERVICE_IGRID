using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class V_ART_WF_ALL_PROCESS_2 : V_ART_WF_ALL_PROCESS
    {
        public string CREATE_DATE_FROM { get; set; }
        public string CREATE_DATE_TO { get; set; }
        public string CURRENT_STEP_DISPLAY_TXT { get; set; }

        public string CURRENT_USER_DISPLAY_TXT { get; set; }
        public string WORKFLOW_NO { get; set; }
        public string WORKFLOW_TYPE { get; set; }
        public int WORKFLOW_SUB_ID { get; set; }
        public string ASSIGN_FROM_USER_DISPLAY_TXT { get; set; }
        public string ASSIGN_TO_USER_DISPLAY_TXT { get; set; }

        public int USER_ID { get; set; }

        public bool FIRST_LOAD { get; set; }  // by aof reassign
        
        public string SOLD_TO_NAME { get; set; }   //BY AOF #INC-130655 
        public string SHIP_TO_NAME { get; set; }  //BY AOF #INC-130655 
        public string COUNTRY { get; set; }  //BY AOF #INC-130655 

    }

    public class V_ART_WF_ALL_PROCESS_REQUEST : REQUEST_MODEL
    {
        public V_ART_WF_ALL_PROCESS_2 data { get; set; }
    }

    public class V_ART_WF_ALL_PROCESS_RESULT : RESULT_MODEL
    {
        public List<V_ART_WF_ALL_PROCESS_2> data { get; set; }
    }
}
