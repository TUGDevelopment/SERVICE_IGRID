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
    public class ApproveMatchboardController : ApiController
    {
        [Route("api/taskform/internal/approvematchboard")]
        public ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_RESULT GetApproveMatchboard([FromUri]ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_REQUEST param)
        {
            return ApproveMatchboardHelper.GetApproveMatchboard(param);
        }

        [Route("api/taskform/internal/approvematchboard")]
        public ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_RESULT PostApproveMatchboard(ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_REQUEST_LIST param)
        {
            return ApproveMatchboardHelper.PostApproveMatchboard(param);
        }


    }
}
