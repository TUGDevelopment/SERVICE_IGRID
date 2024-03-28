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
    public class LidTypeController : ApiController
    {
        [Route("api/lov/lidtype")]
        public SAP_M_CHARACTERISTIC_RESULT GetLidType([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return LidTypeHelper.GetLidType(param);
        }
    }
}
