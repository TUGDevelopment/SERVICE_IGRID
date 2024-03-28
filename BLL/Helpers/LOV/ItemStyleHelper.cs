using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services;
using DAL;
using DAL.Model;

namespace BLL.Helpers
{
    public class ItemStyleHelper
    {
        public static SAP_M_CHARACTERISTIC_RESULT GetStyle(SAP_M_CHARACTERISTIC_REQUEST param)
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
                            Results.data = new List<SAP_M_CHARACTERISTIC_2>();
                        }
                        else
                        {
                            if (param.data.PACKAGING_TYPE_ID > 0)
                            {
                                param.data.PACKAGING_TYPE_VALUE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(param.data.PACKAGING_TYPE_ID, context).VALUE;
                                switch (param.data.PACKAGING_TYPE_VALUE.ToUpper())
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
                                Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItem(MapperServices.SAP_M_CHARACTERISTIC(param.data), context));
                            }
                            else
                            {
                                Results.data = new List<SAP_M_CHARACTERISTIC_2>();
                            }
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].CHARACTERISTIC_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].DESCRIPTION;
                            }

                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                Results.data = (from u1 in Results.data
                                                where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                                select u1).ToList();
                            }

                            Results.data = Results.data.OrderBy(x => x.DISPLAY_TXT).ToList();
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
    }
}
