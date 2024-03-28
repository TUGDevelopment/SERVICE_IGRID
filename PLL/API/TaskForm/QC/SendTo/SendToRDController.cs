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
    public class QCSendToRDController : ApiController
    {

        [Route("api/taskform/qc/sendtoqc")]
        public ART_WF_ARTWORK_PROCESS_RD_RESULT PostSendToQC(ART_WF_ARTWORK_PROCESS_RD_REQUEST param)
        {
            return RDByQCHelper.SaveRDSendToQC(param);
        }

        [Route("api/taskform/qc/sendtord")]
        public ART_WF_ARTWORK_PROCESS_RD_BY_QC_RESULT PostSendToRD(ART_WF_ARTWORK_PROCESS_RD_BY_QC_REQUEST param)
        {
            return RDByQCHelper.SaveRDByQC(param);
        }

        [Route("api/taskform/qc/sendtord")]
        public ART_WF_ARTWORK_PROCESS_RD_RESULT GetSendToQC([FromUri]ART_WF_ARTWORK_PROCESS_RD_REQUEST param)
        {
            return RDByQCHelper.GetRDByQC(param);
        }
    }
}
