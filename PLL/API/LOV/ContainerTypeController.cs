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
    public class ContainerTypeController : ApiController
    {
        [Route("api/lov/containerType")]
        public SAP_M_CHARACTERISTIC_RESULT GetContainerType([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return ContainerTypeHelper.GetContainerType(param);
        }
    }
}
