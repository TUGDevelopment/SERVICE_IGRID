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
    public class CheckListInfoController : ApiController
    {
        [Route("api/taskform/checklist/info")]
        public ART_WF_MOCKUP_CHECK_LIST_RESULT GetCheckListInfo([FromUri]ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            return CheckListInfoHelper.GetCheckListInfo(param);
        }
    }
}
