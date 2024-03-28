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
    public class ProductCodeController : ApiController
    {
        [Route("api/lov/pa/productcode")]
        public ART_WF_ARTWORK_REQUEST_PRODUCT_RESULT Get([FromUri]ART_WF_ARTWORK_REQUEST_PRODUCT_REQUEST param)
        {
            ART_WF_ARTWORK_REQUEST_PRODUCT_RESULT Results = new ART_WF_ARTWORK_REQUEST_PRODUCT_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        ART_WF_ARTWORK_REQUEST_PRODUCT_2 request = new ART_WF_ARTWORK_REQUEST_PRODUCT_2();
                        request.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;

                        Results.data = MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCT(ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCT(request), context));

                        Results.data = Results.data.ToList();

                        if (Results.data.Count > 0)
                        {
                            XECM_M_PRODUCT xProduct = new XECM_M_PRODUCT();

                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                if (Results.data[i].PRODUCT_CODE_ID > 0)
                                {
                                    xProduct = new XECM_M_PRODUCT();
                                    xProduct.XECM_PRODUCT_ID = Results.data[i].PRODUCT_CODE_ID;
                                    xProduct = XECM_M_PRODUCT_SERVICE.GetByItem(xProduct, context).FirstOrDefault();

                                    if (xProduct != null)
                                    {
                                        var isAdd = true;
                                        if (param != null && param.data != null && param.data.DISPLAY_TXT != null)
                                        {
                                            if (!xProduct.PRODUCT_CODE.ToUpper().Contains(param.data.DISPLAY_TXT.ToUpper()))
                                            {
                                                isAdd = false;
                                            }
                                        }

                                        if (isAdd)
                                        {
                                            Results.data[i].NET_WEIGHT = xProduct.NET_WEIGHT;
                                            Results.data[i].DRAINED_WEIGHT = xProduct.DRAINED_WEIGHT;
                                            Results.data[i].PRODUCT_CODE_ID = xProduct.XECM_PRODUCT_ID;
                                            Results.data[i].ID = xProduct.XECM_PRODUCT_ID;
                                            Results.data[i].DISPLAY_TXT = xProduct.PRODUCT_CODE;
                                            Results.data[i].PRODUCT_DESCRIPTION = xProduct.PRODUCT_DESCRIPTION;
                                            Results.data[i].PRODUCT_TYPE = Results.data[i].PRODUCT_TYPE;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                Results.status = "S";
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