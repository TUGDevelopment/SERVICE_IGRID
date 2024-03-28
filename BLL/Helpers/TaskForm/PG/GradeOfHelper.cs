using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class GradeOfHelper
    {

        public static SAP_M_CHARACTERISTIC_RESULT GetGradeOf(SAP_M_CHARACTERISTIC_REQUEST param)
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
                            SAP_M_CHARACTERISTIC_2 characteristic = new SAP_M_CHARACTERISTIC_2();
                            characteristic.NAME = "_GRADE";

                            Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItemContain(MapperServices.SAP_M_CHARACTERISTIC(characteristic), context));
                        }
                        else
                        {
                            string characteristic_name = "";

                            param.data.PACKAGING_TYPE_VALUE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(param.data.PACKAGING_TYPE_ID, context).VALUE;
                            switch (param.data.PACKAGING_TYPE_VALUE)
                            {
                                case "C":
                                    characteristic_name = "ZPKG_SEC_CARDBOARD_GRADE";
                                    break;
                                case "F":
                                    characteristic_name = "ZPKG_SEC_CARTON_GRADE";
                                    break;
                                case "D":
                                    characteristic_name = "ZPKG_SEC_DISPLAYER_GRADE";
                                    break;
                                case "N":
                                    characteristic_name = "ZPKG_SEC_INNER_GRADE";
                                    break;
                                case "P":
                                    characteristic_name = "ZPKG_SEC_INSERT_GRADE";
                                    break;
                                case "K":
                                    characteristic_name = "ZPKG_SEC_LABEL_GRADE";
                                    break;
                                case "L":
                                    characteristic_name = "ZPKG_SEC_LEAFTLET_GRADE";
                                    break;
                                case "R":
                                    characteristic_name = "ZPKG_SEC_INNER_NON_GRADE";
                                    break;
                                case "M":
                                    characteristic_name = "ZPKG_SEC_PLAST_GRADE";
                                    break;
                                case "H":
                                    characteristic_name = "ZPKG_SEC_SLEEVE_BOX_GRADE";
                                    break;
                                case "J":
                                    characteristic_name = "ZPKG_SEC_STICKER_GRADE";
                                    break;
                                case "G":
                                    characteristic_name = "ZPKG_SEC_TRAY_GRADE";
                                    break;
                                default:
                                    characteristic_name = "XXXXXXXXXXXXXXXXXXX";
                                    break;
                            }

                            param.data.NAME = characteristic_name;
                            param.data.DESCRIPTION = param.data.DISPLAY_TXT;
                            Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItemContain(MapperServices.SAP_M_CHARACTERISTIC(param.data), context));
                        }

                        Results.status = "S";

                        if (Results.data.Count > 0)
                        {
                            var result = Results.data.GroupBy(p => p.VALUE)
                               .Select(grp => grp.First())
                               .ToList();

                            Results.data = result.OrderBy(x => x.VALUE).ToList();

                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                Results.data = (from u1 in Results.data
                                                where (u1.VALUE.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.DESCRIPTION.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                                select u1).ToList();
                            }

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

    }
}
