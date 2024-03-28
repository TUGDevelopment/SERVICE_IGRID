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
    public class AttachmentArtworkController : ApiController
    {
        [Route("api/taskform/attachment/artwork/info")]
        public ART_WF_ARTWORK_ATTACHMENT_RESULT GetAttachmentInfo([FromUri]ART_WF_ARTWORK_ATTACHMENT_REQUEST param)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    param.data.currentUserId = CNService.getCurrentUser(context);
                }
            }
            return AttachmentArtworkHelper.GetAttachmentInfo(param);
        }

        [Route("api/taskform/attachment/artwork/info_version")]
        public ART_WF_ARTWORK_ATTACHMENT_RESULT GetAttachmentInfoFileVersion([FromUri]ART_WF_ARTWORK_ATTACHMENT_REQUEST param)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    param.data.currentUserId = CNService.getCurrentUser(context);
                }
            }
            return AttachmentArtworkHelper.GetAttachmentInfoFileVersion(param);
        }

        [Route("api/taskform/attachment/artwork/delete")]
        public ART_WF_ARTWORK_ATTACHMENT_RESULT DeleteAttachment(ART_WF_ARTWORK_ATTACHMENT_REQUEST param)
        {
            return AttachmentArtworkHelper.DeleteAttachment(param);
        }

        [Route("api/taskform/attachment/artwork/deleteversion")]
        public ART_WF_ARTWORK_ATTACHMENT_RESULT DeleteAttachmentVersion(ART_WF_ARTWORK_ATTACHMENT_REQUEST param)
        {
            return AttachmentArtworkHelper.DeleteAttachmentVersion(param);
        }

        [Route("api/taskform/attachment/artwork/visibility")]
        public ART_WF_ARTWORK_ATTACHMENT_RESULT PostVisibility(ART_WF_ARTWORK_ATTACHMENT_REQUEST param)
        {
            return AttachmentArtworkHelper.PostVisibility(param);
        }
    }
}
