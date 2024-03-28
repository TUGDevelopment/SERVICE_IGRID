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
    public class WorkFlowStepController : ApiController
    {
        [Route("api/lov/step")]
        public WORK_FLOW_STEP_RESULT GetStep([FromUri]WORK_FLOW_STEP_REQUEST param)
        {
            return WorkFlowStepHelper.GetStep(param);
        }
    }
}
