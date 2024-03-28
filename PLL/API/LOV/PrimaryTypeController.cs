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
    public class PrimaryTypeController : ApiController
    {
        [Route("api/lov/primarytype")]
        public SAP_M_CHARACTERISTIC_RESULT GetPrimaryType([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return PrimaryTypeHelper.GetPrimaryType(param);
        }
    }
}
