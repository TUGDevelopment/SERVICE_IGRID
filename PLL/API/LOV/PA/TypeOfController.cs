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
    public class TypeOfController : ApiController
    {

        [Route("api/lov/pa/typeofandbrand")]
        public SAP_M_CHARACTERISTIC_RESULT GetTypeofBrand([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            // aof IGRID_REIM_SPLINT2
            SAP_M_CHARACTERISTIC_RESULT Results = new SAP_M_CHARACTERISTIC_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    var brand_id = 0;
                    var typeof_id = 0;

                    if (param.data.ID > 0 )
                    {
                        typeof_id = param.data.ID;
                    }

                    if (!string.IsNullOrEmpty(param.data.BRAND_DESCRIPTION))
                    {
                        var listData = param.data.BRAND_DESCRIPTION.Split(':');
                        if (listData.Length >= 1)
                        {
                            var mat_group = listData[0];
                            brand_id = context.SAP_M_BRAND.Where(w => w.MATERIAL_GROUP == mat_group).Select(s => s.BRAND_ID).FirstOrDefault();
                        }
                    }


                  
                    var list = context.Database.SqlQuery<SAP_M_CHARACTERISTIC_2>("sp_ART_IGRID_TYPEOF_BRAND_MATDESC @AW_TYPEOF_ID,@AW_BRAND_ID,@MATERIAL,@REF_MATERIAL,@ARTWORK_SUB_ID", 
                        new System.Data.SqlClient.SqlParameter("@AW_TYPEOF_ID", typeof_id), 
                        new System.Data.SqlClient.SqlParameter("@AW_BRAND_ID", brand_id),
                        new System.Data.SqlClient.SqlParameter("@MATERIAL", string.Format("{0}", param.data.MATERIAL_NO)),
                        new System.Data.SqlClient.SqlParameter("@REF_MATERIAL", string.Format("{0}", param.data.REF_MATERIAL_NO)),
                        new System.Data.SqlClient.SqlParameter("@ARTWORK_SUB_ID", string.Format("{0}", param.data.WHERE_NOT_IN_CHARACTERISTIC_ID))).ToList();
                    Results.data = list;
                    Results.status = "S";
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }


        [Route("api/lov/pa/typeof")]
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
                            case "C":
                                param.data.NAME = "ZPKG_SEC_CARDBOARD_TYPE_1";
                                break;
                            case "D":
                                param.data.NAME = "ZPKG_SEC_DISPLAYER_TYPE_1";
                                break;
                            case "F":
                                param.data.NAME = "ZPKG_SEC_CARTON_TYPE_1";
                                break;
                            case "G":
                                param.data.NAME = "ZPKG_SEC_TRAY_TYPE";
                                break;
                            case "H":
                                param.data.NAME = "ZPKG_SEC_SLEEVE_BOX_TYPE";
                                break;
                            case "J":
                                param.data.NAME = "ZPKG_SEC_STICKER_TYPE";
                                break;
                            case "K":
                                param.data.NAME = "ZPKG_SEC_LABEL_TYPE";
                                break;
                            case "L":
                                param.data.NAME = "ZPKG_SEC_LEAFTLET_TYPE";
                                break;
                            case "M":
                                param.data.NAME = "ZPKG_SEC_STYLE_PLASTIC";
                                break;
                            case "N":
                                param.data.NAME = "ZPKG_SEC_INNER_TYPE_1";
                                break;
                            case "P":
                                param.data.NAME = "ZPKG_SEC_INSERT_TYPE";
                                break;
                            case "R":
                                param.data.NAME = "ZPKG_SEC_INNER_NON_TYPE";
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

        [Route("api/lov/pa/typeof2")]
        public SAP_M_CHARACTERISTIC_RESULT Get2([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
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
                            case "C":
                                param.data.NAME = "ZPKG_SEC_CARDBOARD_TYPE_2";
                                break;
                            case "D":
                                param.data.NAME = "ZPKG_SEC_DISPLAYER_TYPE_2";
                                break;
                            case "F":
                                param.data.NAME = "ZPKG_SEC_CARTON_TYPE_2";
                                break;
                            case "G":
                                param.data.NAME = "ZPKG_SEC_TRAY_CARTON_TYPE";
                                break;
                            //case "H":
                            //    param.data.NAME = "ZPKG_SEC_SLEEVE_BOX_TYPE";
                            //    break;
                            //case "J":
                            //    param.data.NAME = "ZPKG_SEC_STICKER_TYPE";
                            //    break;
                            //case "K":
                            //    param.data.NAME = "ZPKG_SEC_LABEL_TYPE";
                            //    break;
                            //case "L":
                            //    param.data.NAME = "ZPKG_SEC_LEAFTLET_TYPE";
                            //    break;
                            //case "M":
                            //    param.data.NAME = "ZPKG_SEC_STYLE_PLASTIC";
                            //    break;
                            case "N":
                                param.data.NAME = "ZPKG_SEC_INNER_TYPE_2";
                                break;
                            //case "P":
                            //    param.data.NAME = "ZPKG_SEC_INSERT_TYPE";
                            //    break;
                            case "R":
                                param.data.NAME = "ZPKG_SEC_INNER_TYPE_2";
                                break;
                            //default:
                            //    param.data.NAME = "XXXXXXXXXXXXXXXXXXXXXXX";
                            //    break;

                        }

                        Results.data = new List<SAP_M_CHARACTERISTIC_2>();

                        if (!String.IsNullOrEmpty(param.data.NAME))
                        {
                            param.data.DESCRIPTION = param.data.DISPLAY_TXT;
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItemContain(MapperServices.SAP_M_CHARACTERISTIC(param.data), context));
                        }

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