using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PLL.API.LOV.PA
{
    public class DirectionOfStickerController : ApiController
    {
        [Route("api/lov/pa/directionofsticker")]
        public SAP_M_CHARACTERISTIC_RESULT Get([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return Helper.QueryByName("ZPKG_SEC_DIRECTION", param);
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}