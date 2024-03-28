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
    public class WarehouseFormController : ApiController
    {
        [Route("api/taskform/internal/warehouse")]
        public ART_WF_MOCKUP_PROCESS_WAREHOUSE_RESULT GetWarehouseForm([FromUri]ART_WF_MOCKUP_PROCESS_WAREHOUSE_REQUEST param)
        {
            return WarehouseHelper.GetWarehouseForm(param);
        }

        [Route("api/taskform/internal/warehouse")]
        public ART_WF_MOCKUP_PROCESS_WAREHOUSE_RESULT PostWarehouseForm(ART_WF_MOCKUP_PROCESS_WAREHOUSE_REQUEST_LIST param)
        {
            return WarehouseHelper.SaveWarehouseForm(param);
        }

        //[Route("api/taskform/internal/warehouse/packstyle")]
        //public ART_WF_MOCKUP_CHECK_LIST_RESULT PostWarehouseUpdatePackStyle(ART_WF_MOCKUP_CHECK_LIST_REQUEST param)
        //{
        //    return WarehouseHelper.PostWarehouseUpdatePackStyle(param);
        //}

    }
}
