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
    public class LidTypeHelper
    {
        public static SAP_M_CHARACTERISTIC_RESULT GetLidType(SAP_M_CHARACTERISTIC_REQUEST param)
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
                            characteristic.NAME = "ZPKG_SEC_LID_TYPE";
                            characteristic.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItem(MapperServices.SAP_M_CHARACTERISTIC(characteristic), context));
                        }
                        else
                        {
                            SAP_M_CHARACTERISTIC_2 characteristic = new SAP_M_CHARACTERISTIC_2();
                            param.data.NAME = "ZPKG_SEC_LID_TYPE";
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItem(MapperServices.SAP_M_CHARACTERISTIC(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].CHARACTERISTIC_ID;
                                Results.data[i].DISPLAY_TXT =
                                Results.data[i].VALUE.Trim().ToLower() == Results.data[i].DESCRIPTION.Trim().ToLower() ?
                                Results.data[i].DESCRIPTION : Results.data[i].VALUE + ":" + Results.data[i].DESCRIPTION;
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
