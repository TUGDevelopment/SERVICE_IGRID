using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;

namespace PLL.API
{
    public class PlanningFormController : ApiController
    {
        [Route("api/taskform/internal/planning")]
        public ART_WF_MOCKUP_PROCESS_PLANNING_RESULT GetPlanningForm([FromUri]ART_WF_MOCKUP_PROCESS_PLANNING_REQUEST param)
        {
            return PlanningHelper.GetPlanningForm(param);
        }

        [Route("api/taskform/internal/planning")]
        public ART_WF_MOCKUP_PROCESS_PLANNING_RESULT PostPlanningForm(ART_WF_MOCKUP_PROCESS_PLANNING_REQUEST_LIST param)
        {
            return PlanningHelper.SavePlanningForm(param);
        }


    }
}
