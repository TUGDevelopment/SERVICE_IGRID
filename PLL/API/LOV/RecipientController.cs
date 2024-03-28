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
    public class RecipientController : ApiController
    {
        [Route("api/lov/recipient")]
        public ART_M_USER_RESULT GetRecipient([FromUri]ART_M_USER_REQUEST param)
        {
            return RecipientHelper.GetRecipientHelper(param);
        }
    }
}
