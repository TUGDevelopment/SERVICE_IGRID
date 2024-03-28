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
    public class ArtworkMailToCustomerController : ApiController
    {
        [Route("api/artwork/mailtocustomer")]
        public ART_M_USER_RESULT GetArtworkMailToCustomerController([FromUri]ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_REQUEST param) //
        {
            return ArtworkCustomerOtherHelper.GetArtworkMailCustomerOtherUser(param);
        }

       
    }
}
