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
    public class PrimarySizeController : ApiController
    {
        [Route("api/lov/primarysize")]
        public SAP_M_CHARACTERISTIC_RESULT GetPrimarySize([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return PrimarySizeHelper.GetPrimarySize(param);
        }

        [Route("api/lov/primarysizeXecm")]
        public SAP_M_3P_RESULT GetPrimarySizeXecm([FromUri]SAP_M_3P_REQUEST param)
        {
            return PrimarySizeHelper.GetPrimarySizeXecm(param);
        }
    }
}
