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
    public class SendToVendorController : ApiController
    {
        [Route("api/taskform/pg/sendtovendor")]
        public ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_RESULT PostSendToVendor(ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_REQUEST_LIST param)
        {
            return VendorByPGHelper.SaveVendorByPG(param);
        }

        [Route("api/taskform/pg/sendtovendor")]
        public ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_RESULT GetSendToVendor([FromUri]ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_REQUEST param)
        {
            return VendorByPGHelper.GetVendorByPG(param);
        }
    }
}
