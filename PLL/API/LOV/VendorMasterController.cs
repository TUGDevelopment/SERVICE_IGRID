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
    public class VendorMasterController : ApiController
    {
        [Route("api/lov/vendor")]
        public XECM_M_VENDOR_RESULT GetVendor([FromUri]XECM_M_VENDOR_REQUEST param)
        {
            return VendorByPGHelper.GetVendorMaster(param);
        }

        [Route("api/lov/vendorhasuser")]
        public XECM_M_VENDOR_RESULT GetVendorHasUser([FromUri]XECM_M_VENDOR_REQUEST param)
        {
            return VendorByPGHelper.GetVendorHasUser(param);
        }

        [Route("api/lov/vendorhasuser_bymatgroup")]
        public XECM_M_VENDOR_RESULT GetVendorHasUserByMatGroup([FromUri]XECM_M_VENDOR_REQUEST param)
        {
            return VendorByPGHelper.GetVendorHasUserByMatGroup(param);
        }

        [Route("api/lov/vendoruser")]
        public ART_M_USER_VENDOR_RESULT GetVendorUser([FromUri]ART_M_USER_VENDOR_REQUEST param)
        {
            return VendorByPGHelper.GetVendorUser(param);
        }

        [Route("api/lov/vendoruser_quo")]
        public ART_M_USER_VENDOR_RESULT GetVendorUserQuo([FromUri]ART_M_USER_VENDOR_REQUEST param)
        {
            return VendorByPGHelper.GetVendorUserQuo(param);
        }
    }
}
