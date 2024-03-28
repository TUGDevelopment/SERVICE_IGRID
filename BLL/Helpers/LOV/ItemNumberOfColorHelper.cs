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
    public class ItemNumberOfColorHelper
    {
        public static SAP_M_CHARACTERISTIC_RESULT GetNumberOfColor(SAP_M_CHARACTERISTIC_REQUEST param)
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
                            SAP_M_CHARACTERISTIC_REQUEST paramTmp = new SAP_M_CHARACTERISTIC_REQUEST();
                            string characteristic_name = "";

                            if (param.data.PACKAGING_TYPE_ID != 0)
                            {
                                param.data.PACKAGING_TYPE_VALUE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(param.data.PACKAGING_TYPE_ID, context).VALUE;
                                switch (param.data.PACKAGING_TYPE_VALUE.ToUpper())
                                {
                                    case "F":
                                        characteristic_name = "ZPKG_SEC_CAR_TOTAL_COLOUR";
                                        break;
                                    case "C":
                                        characteristic_name = "ZPKG_SEC_CARD_TOTAL_COLOUR";
                                        break;
                                    case "D":
                                        characteristic_name = "ZPKG_SEC_DHEAD_TOTAL_COLOUR";
                                        break;
                                    case "R":
                                        characteristic_name = "ZPKG_SEC_INN_NO_TOTAL_COLOUR";
                                        break;
                                    case "N":
                                        characteristic_name = "ZPKG_SEC_INNER_TOTAL_COLOUR";
                                        break;
                                    case "P":
                                        characteristic_name = "ZPKG_SEC_INST_TOTAL_COLOUR";
                                        break;
                                    case "K":
                                        characteristic_name = "ZPKG_SEC_LABE_TOTAL_COLOUR";
                                        break;
                                    case "L":
                                        characteristic_name = "ZPKG_SEC_LEA_TOTAL_COLOUR";
                                        break;
                                    case "H":
                                        characteristic_name = "ZPKG_SEC_SLEV_TOTAL_COLOUR";
                                        break;
                                    case "J":
                                        characteristic_name = "ZPKG_SEC_STKC_TOTAL_COLOUR";
                                        break;
                                    case "G":
                                        characteristic_name = "ZPKG_SEC_TRAY_TOTAL_COLOUR";
                                        break;
                                    case "M":
                                        characteristic_name = "ZPKG_SEC_PLAST_TOTAL_COLOUR";
                                        break;
                                }
                            }
                            else
                            {
                                characteristic_name = "XXXXXXXXXXXXXXXXXX";
                            }

                            param.data.NAME = characteristic_name;
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItem(MapperServices.SAP_M_CHARACTERISTIC(param.data), context));
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
