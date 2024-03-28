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
    public class MaterialController : ApiController
    {
        [Route("api/lov/productso")]
        public XECM_M_PRODUCT_RESULT GetProductSO([FromUri]XECM_M_PRODUCT_REQUEST param)
        {
            return MaterialHelper.GetProductSO(param);
        }

        [Route("api/lov/bomso")]
        public XECM_M_PRODUCT_RESULT GetBomSO([FromUri]XECM_M_PRODUCT_REQUEST param)
        {
            return MaterialHelper.GetBomSO(param);
        }
    }
}
