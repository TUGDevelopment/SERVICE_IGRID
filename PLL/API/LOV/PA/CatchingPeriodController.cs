using BLL.Services;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PLL.API.LOV.PA
{
    public class CatchingPeriodController : ApiController
    {
        [Route("api/lov/pa/catchingperiod")]
        public SAP_M_CHARACTERISTIC_RESULT Get([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return Helper.QueryByName("ZPKG_SEC_CATCHING_PERIOD", param);
            //SAP_M_CHARACTERISTIC_RESULT Results = new SAP_M_CHARACTERISTIC_RESULT();
            //try
            //{
            //    if (param == null || param.data == null)
            //    {
            //        param = new SAP_M_CHARACTERISTIC_REQUEST();
            //        param.data = new SAP_M_CHARACTERISTIC_2();
            //    }
            //    param.data.NAME = "ZPKG_SEC_CATCHING_PERIOD";
            //    Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItemContain(MapperServices.SAP_M_CHARACTERISTIC(param.data)));

            //    Results.status = "S";

            //    if (Results.data.Count > 0)
            //    {
            //        var result = Results.data.GroupBy(p => p.DESCRIPTION)
            //           .Select(grp => grp.First())
            //           .ToList();

            //        Results.data = result.OrderBy(x => x.DESCRIPTION).ToList();

            //        for (int i = 0; i < Results.data.Count; i++)
            //        {
            //            Results.data[i].ID = Results.data[i].CHARACTERISTIC_ID;
            //            Results.data[i].DISPLAY_TXT = Results.data[i].DESCRIPTION;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Results.status = "E";
            //    Results.msg = CNService.GetErrorMessage(ex);
            //}
            //return Results;
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