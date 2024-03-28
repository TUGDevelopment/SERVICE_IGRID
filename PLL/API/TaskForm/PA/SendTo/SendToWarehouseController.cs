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
    public class PASendToWarehouseController : ApiController
    {
        [Route("api/taskform/pa/sendtowarehouse")]
        public ART_WF_ARTWORK_PROCESS_WH_BY_PA_RESULT PostSendToWH(ART_WF_ARTWORK_PROCESS_WH_BY_PA_REQUEST param)
        {
            return WarehouseByPAHelper.SaveWarehouseByPA(param);
        }

        [Route("api/taskform/pa/sendtowarehouse")]
        public ART_WF_ARTWORK_PROCESS_WH_RESULT GetSendToWH([FromUri]ART_WF_ARTWORK_PROCESS_WH_REQUEST param)
        {
            return WarehouseByPAHelper.GetWarehouseByPA(param);
        }

        [Route("api/taskform/internal/wh/info")]
        public ART_WF_ARTWORK_PROCESS_WH_RESULT PostWHSendToPA(ART_WF_ARTWORK_PROCESS_WH_REQUEST param)
        {
            return WarehouseByPAHelper.PostWHSendToPA(param);
        }
    }
}
