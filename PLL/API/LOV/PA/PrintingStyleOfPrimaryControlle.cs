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
    public class PrintingStyleOfPrimaryController : ApiController
    {
        [Route("api/lov/pa/printingstyleofprimary")]
        public ART_M_PRINTING_STYLE_RESULT Get([FromUri]ART_M_PRINTING_STYLE_REQUEST param)
        {
            ART_M_PRINTING_STYLE_RESULT Results = new ART_M_PRINTING_STYLE_RESULT();
            ART_M_PRINTING_STYLE print = new ART_M_PRINTING_STYLE();
            ART_M_PRINTING_STYLE_2 print2 = new ART_M_PRINTING_STYLE_2();
            List<ART_M_PRINTING_STYLE_2> listPrint2 = new List<ART_M_PRINTING_STYLE_2>();
            List<ART_M_PRINTING_STYLE> listPrint = new List<ART_M_PRINTING_STYLE>();

            bool is_contain = false;
            if (param != null && param.data != null && param.data.DISPLAY_TXT != null)
            {
                print.PRINTING_STYLE_DESCRIPTION = param.data.DISPLAY_TXT;
                is_contain = true;
            }

            print.PRINTING_STYLE_TYPE = "PRIMARY";
            print.IS_ACTIVE = "X";

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (is_contain)
                        {
                            listPrint = ART_M_PRINTING_STYLE_SERVICE.GetByItemContain(print, context).ToList();
                        }
                        else
                        {
                            listPrint = ART_M_PRINTING_STYLE_SERVICE.GetByItem(print, context).ToList();
                        }

                        if (listPrint.Count > 0)
                        {
                            foreach (ART_M_PRINTING_STYLE item in listPrint)
                            {
                                print2 = new ART_M_PRINTING_STYLE_2();

                                print2 = MapperServices.ART_M_PRINTING_STYLE(item);
                                print2.ID = item.PRINTING_STYLE_ID;
                                print2.DISPLAY_TXT = item.PRINTING_STYLE_DESCRIPTION;

                                listPrint2.Add(print2);
                            }
                        }
                        Results.status = "S";
                        Results.data = listPrint2;
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