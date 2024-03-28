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
    public class BrandController : ApiController
    {
        [Route("api/lov/brand")]
        public SAP_M_BRAND_RESULT GetBrand([FromUri]SAP_M_BRAND_REQUEST param)
        {
            return BrandHelper.GetBrand(param);
        }

        [Route("api/lov/brandso")]
        public SAP_M_BRAND_RESULT GetBrandSO([FromUri]SAP_M_BRAND_REQUEST param)
        {
            return BrandHelper.GetBrandSO(param);
        }
    }
}
