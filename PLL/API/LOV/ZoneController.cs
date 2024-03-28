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
    public class ZoneController : ApiController
    {
        [Route("api/lov/zone")]
        public SAP_M_COUNTRY_RESULT GetZone([FromUri]SAP_M_COUNTRY_REQUEST param)
        {
            return ZoneHelper.GetZone(param);
        }
    }
}
