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
    public class RDFormController : ApiController
    {
        [Route("api/taskform/internal/rd")]
        public ART_WF_MOCKUP_PROCESS_RD_RESULT GetRDForm([FromUri]ART_WF_MOCKUP_PROCESS_RD_REQUEST param)
        {
            return RDHelper.GetRDForm(param);
        }

        [Route("api/taskform/internal/rd")]
        public ART_WF_MOCKUP_PROCESS_RD_RESULT PostRDForm(ART_WF_MOCKUP_PROCESS_RD_REQUEST_LIST param)
        {
            return RDHelper.SaveRDForm(param);
        }
    }
}
