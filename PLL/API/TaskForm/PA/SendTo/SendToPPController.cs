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
    public class PASendToPPController : ApiController
    {
        [Route("api/taskform/pa/sendtopp")]
        public ART_WF_ARTWORK_PROCESS_PP_BY_PA_RESULT PostSendToPP(ART_WF_ARTWORK_PROCESS_PP_BY_PA_REQUEST param)
        {
            return PPByPAHelper.SavePPByPA(param);
        }

        [Route("api/taskform/pa/sendtovnbypp")]
        public ART_WF_ARTWORK_PROCESS_PP_BY_PA_RESULT PostSendToVendorByPP(ART_WF_ARTWORK_PROCESS_PP_BY_PA_REQUEST param)
        {
            return PPByPAHelper.SavePPByPA(param);
        }

        [Route("api/taskform/pa/sendtopp")]
        public ART_WF_ARTWORK_PROCESS_PP_RESULT GetSendToPP([FromUri]ART_WF_ARTWORK_PROCESS_PP_REQUEST param)
        {
            return PPByPAHelper.GetPPByPA(param);
        }

        //[Route("api/taskform/internal/wh/info")]
        //public ART_WF_ARTWORK_PROCESS_WH_RESULT PostWHSendToPA(ART_WF_ARTWORK_PROCESS_WH_REQUEST param)
        //{
        //    return WarehouseByPAHelper.PostWHSendToPA(param);
        //}

      
    }
}
