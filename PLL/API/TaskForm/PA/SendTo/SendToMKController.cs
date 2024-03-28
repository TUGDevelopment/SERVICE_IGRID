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
    public class PASendToMKController : ApiController
    {
        [Route("api/taskform/pa/sendtomk")]
        public ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_RESULT PostSendToMK(ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_REQUEST param)
        {
            return MKByPAHelper.SaveMKByPA(param);
        }

        [Route("api/taskform/internal/mk/info")]
        public ART_WF_ARTWORK_PROCESS_MARKETING_RESULT GetSendToMKInfo([FromUri]ART_WF_ARTWORK_PROCESS_MARKETING_REQUEST param)
        {
            return MKByPAHelper.GetSendToMKInfo(param);
        }

        [Route("api/taskform/internal/mk/info")]
        public ART_WF_ARTWORK_PROCESS_MARKETING_RESULT PostMKSendToPA(ART_WF_ARTWORK_PROCESS_MARKETING_REQUEST param)
        {
            return MKByPAHelper.PostMKSendToPA(param);
        }
    }
}
