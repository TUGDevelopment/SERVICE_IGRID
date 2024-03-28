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
    public class SendToGMMKByMKController : ApiController
    {
        //[Route("api/taskform/mk/sendtogmmk")]
        //public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK_RESULT PostSendToGMMK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK_REQUEST param)
        //{
        //    return GMMKByMKHelper.SaveGMMKByMK(param);
        //}

        [Route("api/taskform/mk/sendtogmmk")]
        public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_RESULT PostSendToGMMK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_REQUEST param)
        {
            return GMMKByMKHelper.SaveGMMKByMK_(param);
        }

        [Route("api/taskform/mk/sendtopabygmmk")]
        public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_RESULT PostGMMKSendToPA(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_REQUEST param)
        {
            return GMMKByMKHelper.PostGMMKSendToPA(param);
        }

        [Route("api/taskform/mk/sendtopabygmmkmulti")]
        public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_RESULT PostGMMKSendToPAMulti(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_REQUEST_LIST param)
        {
            return GMMKByMKHelper.PostGMMKSendToPAMulti(param);
        }

        [Route("api/taskform/mk/sendtogmmk")]
        public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_RESULT GetSendToGMMK([FromUri]ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_REQUEST param)
        {
            return GMMKByMKHelper.GetGMMKByMK(param);
        }
    }
}
