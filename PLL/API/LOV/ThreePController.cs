using BLL.Helpers;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PLL.API
{
    public class ThreePController : ApiController
    {
        [Route("api/lov/3p")]
        public SAP_M_3P_RESULT GetThreeP([FromUri]SAP_M_3P_REQUEST param)
        {
            return ThreePHelper.GetThreeP(param);
        }


        [Route("api/lov/3p_primarysize_igrid")]
        public SAP_M_3P_RESULT GetThreePPrimarySizeIGrid([FromUri]SAP_M_3P_REQUEST param)
        {
            // by aof 202306 for CR#IGRID_REIM----ADD NEW Function@
            return ThreePHelper.GetThreePPrimarySizeIGrid(param);
        }


    }
}
