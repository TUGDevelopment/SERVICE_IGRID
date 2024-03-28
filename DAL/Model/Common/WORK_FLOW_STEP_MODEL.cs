using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{

    public partial class WORK_FLOW_STEP
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string WF_TYPE { get; set; }
        public int STEP_ID { get; set; }  // by aof reassign
    }

    public class WORK_FLOW_STEP_METADATA
    {
       
    }

    public class WORK_FLOW_STEP_REQUEST : REQUEST_MODEL
    {
        public WORK_FLOW_STEP data { get; set; }
    }

    public class WORK_FLOW_STEP_RESULT : RESULT_MODEL
    {
        public List<WORK_FLOW_STEP> data { get; set; }
    }
}