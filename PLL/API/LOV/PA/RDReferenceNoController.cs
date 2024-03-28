using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PLL.API.LOV.PA
{
    public class RDReferenceNoController : ApiController
    {
        [Route("api/lov/pa/rdreferenceno")]
        public ART_WF_ARTWORK_REQUEST_REFERENCE_RESULT Get([FromUri]ART_WF_ARTWORK_REQUEST_REFERENCE_REQUEST param)
        {
            ART_WF_ARTWORK_REQUEST_REFERENCE_RESULT Results = new ART_WF_ARTWORK_REQUEST_REFERENCE_RESULT();
            try
            {
                ART_WF_ARTWORK_REQUEST_REFERENCE_2 request = new ART_WF_ARTWORK_REQUEST_REFERENCE_2();
                request.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null && param.data.DISPLAY_TXT != null)
                        {
                            request.REFERENCE_NO = param.data.DISPLAY_TXT;
                            Results.data = MapperServices.ART_WF_ARTWORK_REQUEST_REFERENCE(ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItemContain(MapperServices.ART_WF_ARTWORK_REQUEST_REFERENCE(request), context));

                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_REQUEST_REFERENCE(ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST_REFERENCE(request), context));
                        }

                        Results.status = "S";

                        Results.data = Results.data.ToList();

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].ARTWORK_REFERENCE_ID;
                                Results.data[i].PRODUCT_TYPE = Results.data[i].PRODUCT_TYPE;
                                Results.data[i].DISPLAY_TXT = Results.data[i].REFERENCE_NO;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
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