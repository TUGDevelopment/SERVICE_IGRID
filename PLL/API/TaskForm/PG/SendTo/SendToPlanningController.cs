//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using DAL.Model;
//using BLL.Services;
//using BLL.Helpers;

//namespace PLL.API.TaskForm.PG
//{
//    public class SendToPlanningController : ApiController
//    {

//        [Route("api/taskform/pg/sendtoplanning")]
//        public ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_RESULT PostSendToPlanning(ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_REQUEST param)
//        {
//            return PlanningByPGHelper.SavePlanningByPG(param);
//        }

//        [Route("api/taskform/pg/sendtoplanning")]
//        public ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_RESULT GetSendToPlanning([FromUri]ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_REQUEST param)
//        {
//            return PlanningByPGHelper.GetPlanningByPG(param);
//        }
//    }
//}
