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
    public class PlantController : ApiController
    {
        [Route("api/lov/plant")]
        public SAP_M_PLANT_RESULT GetPlant([FromUri]SAP_M_PLANT_REQUEST param)
        {
            return PlantHelper.GetPlant(param);
        }
    }
}
