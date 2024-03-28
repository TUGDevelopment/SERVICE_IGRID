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
    public class ItemNumberOfColorController : ApiController
    {
        [Route("api/lov/numberofcolor")]
        public SAP_M_CHARACTERISTIC_RESULT GetItemNumberOfColor([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return ItemNumberOfColorHelper.GetNumberOfColor(param);
        }
    }
}
