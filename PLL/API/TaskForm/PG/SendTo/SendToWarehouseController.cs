using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;

namespace PLL.API.TaskForm.PG
{
    public class SendToWarehouseController : ApiController
    {
        [Route("api/taskform/pg/sendtowarehouse")]
        public ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_RESULT PostSendToVendor(ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_REQUEST param)
        {
            return WarehouseByPGHelper.SaveWarehouseByPG(param);
        }

        [Route("api/taskform/pg/sendtowarehouse")]
        public ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_RESULT GetSendToVendor([FromUri]ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_REQUEST param)
        {
            return WarehouseByPGHelper.GetWarehouseByPG(param);
        }
    }
}
