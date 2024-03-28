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
    public class ItemStyleController : ApiController
    {
        [Route("api/lov/style")]
        public SAP_M_CHARACTERISTIC_RESULT GetStyle([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return ItemStyleHelper.GetStyle(param);
        }
    }
}
