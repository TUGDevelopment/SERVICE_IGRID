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
using PLL.Controllers;

namespace PLL.API
{
    public class PPFormController : ApiController
    {
        [Route("api/taskform/internal/pp/info")]
        public ART_WF_ARTWORK_PROCESS_PP_RESULT PostSendToPA(ART_WF_ARTWORK_PROCESS_PP_REQUEST param)
        {
            return PPByPAHelper.SavePPSendToPA(param);
        }

        [Route("api/taskform/pp/incoming")]
        public PP_RESULT GetPP([FromUri]PP_REQUEST param)
        {
            return PPFormHelper.GetWorkflowPending(param);
        }

        //[Route("api/taskform/pp/exporttoexcel")]
        //public PP_RESULT Postexporttoexcel(PP_REQUEST_LIST param)
        //{
        //    return PPFormHelper.GetExportToExcel(param);
        //}

        [Route("api/taskform/pp/accepttask")]
        public ART_WF_ARTWORK_PROCESS_PP_RESULT PostAcceptTask(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            return PPByPAHelper.AcceptTask(param);
        }
        
        [Route("api/taskform/pp/sendtovendorview")]
        public PP_VENDOR_RESULT GetSendVendorTask([FromUri]ART_WF_ARTWORK_PROCESS_PP_REQUEST param)
        {
            if (param == null) param = new ART_WF_ARTWORK_PROCESS_PP_REQUEST();
            if (param.data == null) param.data = new ART_WF_ARTWORK_PROCESS_PP_2();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    param.data.UPDATE_BY = CNService.getCurrentUser(context);
                }
            }
            return PPFormHelper.GetWorkflowPendingToVendor(param);
        }

        [Route("api/taskform/pp/multisendtovendor")]
        public ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_RESULT PostMultiVendorSendToPA(ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_REQUEST_LIST param)
        {
            return PPFormHelper.SaveMultiVendorByPP(param);
        }
    }
}
