using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;
using DAL;

namespace PLL.API
{
    public class POController : ApiController
    {
        [Route("api/master/purchasingorg")]
        public SAP_M_PO_IDOC_RESULT GetPurchasingOrg([FromUri]SAP_M_PO_IDOC_REQUEST param)
        {
            return POHelper.GetPurchasingOrg(param);
            
        }
    }
}
