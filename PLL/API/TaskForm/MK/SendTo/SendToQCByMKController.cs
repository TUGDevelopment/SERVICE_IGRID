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
    public class SendToQCByMKController : ApiController
    {
        //Send back
        //[Route("api/taskform/mk/sendtoqc")]
        //public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK_RESULT PostSendToQC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK_REQUEST param)
        //{
        //    return QCByMKHelper.SaveQCByMK(param);
        //}

        [Route("api/taskform/mk/sendtoqc")]
        public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_RESULT PostSendToQC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_REQUEST param)
        {
            return QCByMKHelper.SaveQCByMK(param);
        }

        [Route("api/taskform/mk/sendtoqc")]
        public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT GetQCVerify([FromUri]ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_REQUEST param)
        {
            return QCByMKHelper.GetQCVerify(param);
        }
    }
}
