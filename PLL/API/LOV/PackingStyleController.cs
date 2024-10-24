﻿using BLL.Helpers;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PLL.API
{
    public class PacckingStyleController : ApiController
    {
        [Route("api/lov/packingstyle")]
        public SAP_M_CHARACTERISTIC_RESULT GetPackingStyle([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return PackingStyleHelper.GetPackingStyle(param);
        }
    }
}
