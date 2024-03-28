using BLL.Helpers;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PLL.API
{
    public class TwoPController : ApiController
    {
        [Route("api/lov/2p")]
        public SAP_M_2P_RESULT GetThreeP([FromUri]SAP_M_2P_REQUEST param)
        {
            return TwoPHelper.GetTwoP(param);
        }

        [Route("api/lov/2p_new")]
        public SAP_M_2P_RESULT GetTwoP_New([FromUri]SAP_M_2P_REQUEST param)
        {
            return TwoPHelper.GetTwoP_New(param);
        }

    }
}
