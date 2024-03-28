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
    public class DecisionReasonController : ApiController
    {
        [Route("api/lov/decisionreason")]
        public ART_M_DECISION_REASON_RESULT GetDecisionReason([FromUri]ART_M_DECISION_REASON_REQUEST param)
        {
            return DecisionReasonHelper.GetDecisionReason(param);
        }
    }
}
