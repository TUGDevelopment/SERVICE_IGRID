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
    public class ItemCoatingController : ApiController
    {
        [Route("api/lov/coating")]
        public SAP_M_CHARACTERISTIC_ITEM_2_RESULT GetCoating([FromUri]SAP_M_CHARACTERISTIC_ITEM_2_REQUEST param)
        {
            return ItemCoatingHelper.GetCoating(param);
        }
    }
}
