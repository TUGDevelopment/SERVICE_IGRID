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
    public class SendToMKByQCController : ApiController
    {


        [Route("api/taskform/qc/sendtomk")]
        public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT PostSendToMKFromQC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_REQUEST param)
        {
            return MKByQCHelper.SaveMKbyQC(param);
        }


        [Route("api/taskform/qc/sendtomk")]
        public ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_RESULT GetSendToMK([FromUri]ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_REQUEST param)
        {
            return MKByQCHelper.GetMKbyQC(param);
        }
    }
}
