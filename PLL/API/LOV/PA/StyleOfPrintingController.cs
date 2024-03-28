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
    public class StyleOfPrintingController : ApiController
    {
        [Route("api/lov/pa/styleofprintting")]
        public SAP_M_CHARACTERISTIC_RESULT Get([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            SAP_M_CHARACTERISTIC_RESULT Results = new SAP_M_CHARACTERISTIC_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {

                        if (param == null || param.data == null)
                        {
                            Results.status = "S";
                            Results.data = new List<SAP_M_CHARACTERISTIC_2>();
                            return Results;
                        }

                        if (param == null)
                        {
                            param = new SAP_M_CHARACTERISTIC_REQUEST();
                        }
                        if (param.data == null)
                        {
                            param.data = new SAP_M_CHARACTERISTIC_2();
                        }
                        var code = Helper.GetMaterialCode(param.data.MATERIAL_GROUP_ID);
                        switch (code)
                        {
                            case "F":
                                param.data.NAME = "ZPKG_SEC_CAR_PRINTING_STYLE";
                                break;
                            case "C":
                                param.data.NAME = "ZPKG_SEC_CARD_PRINTING_STYLE";
                                break;
                            case "D":
                                param.data.NAME = "ZPKG_SEC_DISP_PRINTING_STYLE";
                                break;
                            case "R":
                                param.data.NAME = "ZPKG_SEC_INN_NO_PRINTING_STYLE";
                                break;
                            case "N":
                                param.data.NAME = "ZPKG_SEC_INNER_PRINTING_STYLE";
                                break;
                            case "P":
                                param.data.NAME = "ZPKG_SEC_INST_PRINTING_STYLE";
                                break;
                            case "K":
                                param.data.NAME = "ZPKG_SEC_LABE_PRINTING_STYLE";
                                break;
                            case "L":
                                param.data.NAME = "ZPKG_SEC_LEA_PRINTING_STYLE";
                                break;
                            case "H":
                                param.data.NAME = "ZPKG_SEC_SLEV_PRINTING_STYLE";
                                break;
                            case "J":
                                param.data.NAME = "ZPKG_SEC_STKC_PRINTING_STYLE";
                                break;
                            case "G":
                                param.data.NAME = "ZPKG_SEC_TRAY_PRINTING_STYLE";
                                break;
                            case "M":
                                param.data.NAME = "ZPKG_SEC_PLAST_PRINTING_STYLE";
                                break;
                            default:
                                param.data.NAME = "XXXXXXXXXXXXXXXXXXXXXXX";
                                break;
                        }
                        param.data.DESCRIPTION = param.data.DISPLAY_TXT;
                        param.data.IS_ACTIVE = "X";
                        Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItemContain(MapperServices.SAP_M_CHARACTERISTIC(param.data), context));

                        Results.status = "S";

                        Results = Helper.MapData(Results);
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