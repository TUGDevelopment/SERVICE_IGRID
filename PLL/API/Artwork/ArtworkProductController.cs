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
    public class ArtworkProductController : ApiController
    {
        [Route("api/artwork/product")]
        public XECM_M_PRODUCT_RESULT PostCheckListProduct(XECM_M_PRODUCT_REQUEST_LIST param) //
        {
            return ProductHelper.GetProduct(param);
        }
        [Route("api/artwork/product_vap")]
        public CN_VAP_MODEL_RESULT PostCheckProduct_vap(XECM_M_PRODUCT_REQUEST_LIST param) //
        {
            return ProductHelper.GetProduct_vap(param);
        }

    }
}
