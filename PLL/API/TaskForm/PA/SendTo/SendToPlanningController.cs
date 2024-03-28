using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;

namespace PLL.API.TaskForm.PA
{
    public class PASendToPlanningController : ApiController
    {
        [Route("api/taskform/pa/sendtoplanning")]
        public ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_RESULT PostSendToVendor(ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_REQUEST param)
        {
            return PlanningByPAHelper.SavePlanningByPA(param);
        }

        [Route("api/taskform/pa/sendtoplanning")]
        public ART_WF_ARTWORK_PROCESS_PLANNING_RESULT GetSendToVendor([FromUri]ART_WF_ARTWORK_PROCESS_PLANNING_REQUEST param)
        {
            return PlanningByPAHelper.GetPlanningByPA(param);
        }

        [Route("api/taskform/internal/planning/info")]
        public ART_WF_ARTWORK_PROCESS_PLANNING_RESULT PostPNSendToPA(ART_WF_ARTWORK_PROCESS_PLANNING_REQUEST param)
        {
            return PlanningByPAHelper.PostPlanningSendToPA(param);
        }
    }
}
