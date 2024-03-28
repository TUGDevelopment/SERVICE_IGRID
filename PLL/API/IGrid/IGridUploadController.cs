using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;
using DAL;
//using PLL.Controllers;

namespace PLL.API
{
    public class IGridUploadController : ApiController
    {
        [Route("api/taskigrid/upload")]
        public Attachment_RESULT GetUploadIGrid([FromUri] Attachment_REQUEST param)
        {
            return IGridFormHelper.GetAttachment(param);
        }
        //[Route("api/taskform/igrid/result")]
        //public IGRID_RESULT GetResult([FromUri] IGRID_REQUEST param)
        //{
        //    return IGridFormHelper.GetWorkflowPending(param);
        //}

    }
}