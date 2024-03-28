using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;
using System.Web.Script.Serialization;

namespace PLL.API
{
    public class RemarkReasonController : ApiController
    {

        [Route("api/taskform/remarkreasonaw/info")]
        public ART_WF_REMARK_REASON_OTHER_RESULT PostRemarkReasonAW(ART_WF_REMARK_REASON_OTHER_REQUEST param)
        {
            return ArtworkRemarkReason.SaveRemarkReasonAW(param);
        }

        [Route("api/taskform/remarkreasonmockup/info")]
        public ART_WF_REMARK_REASON_OTHER_RESULT PostRemarkReasonMockup(ART_WF_REMARK_REASON_OTHER_REQUEST_LIST param)
        {
            return ArtworkRemarkReason.SaveRemarkReasonMockup(param);
        }

        [Route("api/taskform/remarkreason/info")]
        public ART_WF_REMARK_REASON_OTHER_RESULT GetProcess([FromUri]ART_WF_REMARK_REASON_OTHER_REQUEST param)
        {
            return ArtworkRemarkReason.GetRemarkReason(param);
        }
    }
}
