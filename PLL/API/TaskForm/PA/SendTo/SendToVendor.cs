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
    public class PASendToVendorController : ApiController
    {
        [Route("api/taskform/pa/sendtovendor")]
        public ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_RESULT PostSendToVendor(ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_REQUEST param)
        {
            return VendorByPAHelper.SaveVendorByPA(param);
        }

        [Route("api/taskform/pa/sendtovendor")]
        public ART_WF_ARTWORK_PROCESS_VENDOR_RESULT GetSendToVendor([FromUri]ART_WF_ARTWORK_PROCESS_VENDOR_REQUEST param)
        {
            return VendorByPAHelper.GetVendorByPA(param);
        }

        [Route("api/taskform/pa/vendor/info")]
        public ART_WF_ARTWORK_PROCESS_VENDOR_RESULT PostVendorSendToPA(ART_WF_ARTWORK_PROCESS_VENDOR_REQUEST param)
        {
            return VendorByPAHelper.PostVendorSendToPA(param);
        }
    }
}
