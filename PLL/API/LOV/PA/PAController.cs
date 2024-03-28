using BLL.Helpers;
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
    public class PAController : ApiController
    {
        [Route("api/lov/pa/pa")]
        public SAP_M_CHARACTERISTIC_RESULT Get([FromUri]string materialGroup)
        {
            SAP_M_CHARACTERISTIC_RESULT Results = new SAP_M_CHARACTERISTIC_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        SAP_M_CHARACTERISTIC_REQUEST param = new SAP_M_CHARACTERISTIC_REQUEST();
                        switch (materialGroup.ToUpper())
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

                        if (Results.data.Count > 0)
                        {
                            var result = Results.data.GroupBy(p => p.DESCRIPTION)
                               .Select(grp => grp.First())
                               .ToList();

                            Results.data = result.OrderBy(x => x.DESCRIPTION).ToList();

                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].CHARACTERISTIC_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].DESCRIPTION;
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

        [Route("api/lov/pa/marketing")]
        public ART_M_USER_RESULT GetRecipient([FromUri]ART_M_USER_REQUEST param)
        {
            return MarketingHelper.GetMarketingHelper(param);
        }

    }
}