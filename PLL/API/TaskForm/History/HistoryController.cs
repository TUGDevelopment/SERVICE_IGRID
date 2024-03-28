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
    public class HistoryController : ApiController
    {
        [Route("api/taskform/history/info")]
        public ART_WF_MOCKUP_PROCESS_RESULT GeTaskFormHistory([FromUri]ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            return HistoryHelper.GetTaskFormHistory(param);
        }

        [Route("api/taskform/history/artworkinfo")]
        public ART_WF_ARTWORK_PROCESS_RESULT GeTaskFormHistory([FromUri]ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            return HistoryHelper.GetTaskFormArtworkHistory(param);
        }
    }
}
