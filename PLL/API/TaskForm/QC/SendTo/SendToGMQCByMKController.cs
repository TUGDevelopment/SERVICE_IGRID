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
    public class SendToGMQCByMKController : ApiController
    {
        //[Route("api/taskform/qc/sendtogmqc")]
        //public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC_RESULT PostSendToGMMK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC_REQUEST param)
        //{
        //    return GMQCByQCHelper.SaveGMQCByQC(param);
        //}

        [Route("api/taskform/qc/sendtogmqc")]
        public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT PostSendToGMQCFromQC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_REQUEST param)
        {
            return GMQCByQCHelper.SaveGMQCByQC(param);
        }

        [Route("api/taskform/qc/sendtomkbygmqc")]
        public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_RESULT PostGMQCSendToMK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_REQUEST param)
        {
            return GMQCByQCHelper.PostGMQCSendToMK(param);
        }

        [Route("api/taskform/qc/sendtomkbygmqcmulti")]
        public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_RESULT PostMultiGMQCSendToMK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_REQUEST_LIST param)
        {
            return GMQCByQCHelper.PostMultiGMQCSendToMK(param);
        }

        [Route("api/taskform/qc/sendtogmqc")]
        public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_RESULT GetSendToGMMK([FromUri]ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_REQUEST param)
        {
            return GMQCByQCHelper.GetGMQCByQC(param);
        }
    }  
}
