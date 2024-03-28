using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BLL.Helpers;


namespace PLL.API
{
    public class TypeOfProductController : ApiController
    {
        [Route("api/lov/typeofproduct")]
        public SAP_M_TYPE_OF_PRODUCT_RESULT GetTypeOfProduct([FromUri]SAP_M_TYPE_OF_PRODUCT_REQUEST param)
        {
            return TypeOfProductHelper.GetTypeOfProduct(param);
        }

    
    }
}
