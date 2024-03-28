using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;


namespace PLL.API
{
    public class SAPVendorController : ApiController
    {

        [Route("api/lov/pg_vendor")]
        public SAP_M_CHARACTERISTIC_RESULT GetVendor([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return SAPVendorHelper.GetSAPVendor(param);
        }
    }
}
