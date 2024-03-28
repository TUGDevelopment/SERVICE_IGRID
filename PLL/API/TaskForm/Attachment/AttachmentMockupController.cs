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

namespace PLL.API
{
    public class AttachmentMockupController : ApiController
    {
        [Route("api/taskform/attachment/info")]
        public ART_WF_MOCKUP_ATTACHMENT_RESULT GetAttachmentInfo([FromUri]ART_WF_MOCKUP_ATTACHMENT_REQUEST param)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    param.data.currentUserId = CNService.getCurrentUser(context);
                }
            }
            return AttachmentMockupHelper.GetAttachmentInfo(param);
        }

        [Route("api/taskform/attachment/info_version")]
        public ART_WF_MOCKUP_ATTACHMENT_RESULT GetAttachmentInfoFileVersion([FromUri]ART_WF_MOCKUP_ATTACHMENT_REQUEST param)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    param.data.currentUserId = CNService.getCurrentUser(context);
                }
            }
            return AttachmentMockupHelper.GetAttachmentInfoFileVersion(param);
        }

        [Route("api/taskform/attachment/delete")]
        public ART_WF_MOCKUP_ATTACHMENT_RESULT DeleteAttachment(ART_WF_MOCKUP_ATTACHMENT_REQUEST param)
        {
            return AttachmentMockupHelper.DeleteAttachment(param);
        }

        [Route("api/taskform/attachment/deleteversion")]
        public ART_WF_MOCKUP_ATTACHMENT_RESULT DeleteAttachmentVersion(ART_WF_MOCKUP_ATTACHMENT_REQUEST param)
        {
            return AttachmentMockupHelper.DeleteAttachmentVersion(param);
        }

        [Route("api/taskform/attachment/visibility")]
        public ART_WF_MOCKUP_ATTACHMENT_RESULT PostVisibility(ART_WF_MOCKUP_ATTACHMENT_REQUEST param)
        {
            return AttachmentMockupHelper.PostVisibility(param);
        }
    }
}
