using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BLL.Services;
using BLL.Helpers;
using DAL;
using DAL.Model;

namespace PLL.API.Common
{
    public class RequestFormController : ApiController
    {
        [Route("api/common/searchrequest")]
        public SEARCH_REQUEST_FORM_RESULT PostCheckListExist(SEARCH_REQUEST_FORM_REQUEST param)
        {
            return RequestFormHelper.GetRequestFormExist(param);
        }
    }
}
