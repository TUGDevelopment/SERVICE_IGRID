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
    public class FileTemplateController : ApiController
    {
        [Route("api/lov/filetemplatemockup")]
        public CN_LOV_MODEL_RESULT GetFileTemplateMockup([FromUri]CN_LOV_MODEL_REQUEST param)
        {
            return FileTemplateHelper.GetFileTemplateMockup(param);
        }

        [Route("api/lov/filetemplateartwork")]
        public CN_LOV_MODEL_RESULT GetFileTemplateArtwork([FromUri]CN_LOV_MODEL_REQUEST param)
        {
            return FileTemplateHelper.GetFileTemplateArtwork(param);
        }
    }
}
