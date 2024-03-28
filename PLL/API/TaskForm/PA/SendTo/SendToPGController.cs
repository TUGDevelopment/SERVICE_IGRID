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
    public class PASendToPGController : ApiController
    {
        [Route("api/taskform/pa/sendtopg")]
        public ART_WF_ARTWORK_PROCESS_PG_BY_PA_RESULT PostSendToVendor(ART_WF_ARTWORK_PROCESS_PG_BY_PA_REQUEST param)
        {
            return PGByPAHelper.SavePGByPA(param);
        }

        [Route("api/taskform/pa/sendtopg")]
        public ART_WF_ARTWORK_PROCESS_PG_BY_PA_RESULT GetSendToVendor([FromUri]ART_WF_ARTWORK_PROCESS_PG_BY_PA_REQUEST param)
        {
            return PGByPAHelper.GetPGByPA(param);
        }
    }
}
