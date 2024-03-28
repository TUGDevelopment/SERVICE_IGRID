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
    public class POSITIONController : ApiController
    {
        [Route("api/lov/position")]
        public ART_M_POSITION_RESULT GetCompany([FromUri]ART_M_POSITION_REQUEST param)
        {
            return PositionHelper.getPosition(param);
        }
    }
}
