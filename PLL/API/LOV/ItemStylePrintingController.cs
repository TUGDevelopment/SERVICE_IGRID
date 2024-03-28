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
    public class ItemStylePrintingController : ApiController
    {
        [Route("api/lov/styleprinting")]
        public SAP_M_CHARACTERISTIC_RESULT GetItemStylePrinting([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return ItemStylePrintingHelper.GetStylePrinting(param);
        }
    }
}
