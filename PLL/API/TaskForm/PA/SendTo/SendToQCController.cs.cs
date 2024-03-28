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
    public class PASendToQCController : ApiController
    {
        [Route("api/taskform/pa/sendtoqc")]
        public ART_WF_ARTWORK_PROCESS_QC_BY_PA_RESULT PostSendToQC(ART_WF_ARTWORK_PROCESS_QC_BY_PA_REQUEST param)
        {
            return QCByPAHelper.SaveQCByPA(param);
        }

        [Route("api/taskform/pa/sendtoqc")]
        public ART_WF_ARTWORK_PROCESS_QC_RESULT GetSendToQC([FromUri]ART_WF_ARTWORK_PROCESS_QC_REQUEST param)
        {
            return QCByPAHelper.GetQCByPA(param);
        }
        
        [Route("api/taskform/pa/sendqctopa")]
        public ART_WF_ARTWORK_PROCESS_QC_RESULT PostSendToPA(ART_WF_ARTWORK_PROCESS_QC_REQUEST param)
        {
            return QCByPAHelper.SaveQCSendToPA(param);
        }
    }
}
