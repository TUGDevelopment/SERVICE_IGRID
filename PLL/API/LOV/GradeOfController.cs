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
    public class GradeOfController : ApiController
    {
        [Route("api/lov/gradeof")]
        public SAP_M_CHARACTERISTIC_RESULT GetGradeOf([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return GradeOfHelper.GetGradeOf(param);
        } 
    }
}
