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
    public class ItemPackTypeController : ApiController
    {
        [Route("api/lov/packtype")]
        public SAP_M_CHARACTERISTIC_RESULT GetPackType([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return ItemPackingTypeHelper.GetPackType(param);
        }
    }
}
