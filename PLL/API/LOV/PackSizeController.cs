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
    public class PackSizeController : ApiController
    {
        [Route("api/lov/packsize")]
        public SAP_M_CHARACTERISTIC_RESULT GetPackSize([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return PackSizeHelper.GetPackSize(param);
        }

        [Route("api/lov/packsizeXecm")]
        public SAP_M_2P_RESULT GetPackSizeXecm([FromUri]SAP_M_2P_REQUEST param)
        {
            return PackSizeHelper.GetPackSizeXecm(param);
        }
    }
}
