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
    public class ItemPrintSystemController : ApiController
    {
        [Route("api/lov/printsystem")]
        public SAP_M_CHARACTERISTIC_ITEM_2_RESULT GetPrintSystem([FromUri]SAP_M_CHARACTERISTIC_ITEM_2_REQUEST param)
        {
            return ItemPrintSystemHelper.GetPrintSystem(param);
        }
    }
}
