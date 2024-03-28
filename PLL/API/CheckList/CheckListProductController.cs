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
    public class CheckListProductController : ApiController
    {
        [Route("api/checklist/product")]
        public XECM_M_PRODUCT_RESULT PostCheckListProduct(XECM_M_PRODUCT_REQUEST_LIST param) //
        {
            return ProductHelper.GetProduct(param);
        }
    }
}
