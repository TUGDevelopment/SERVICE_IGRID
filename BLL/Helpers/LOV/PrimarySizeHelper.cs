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
    public class PrimarySizeHelper
    {
        public static SAP_M_CHARACTERISTIC_RESULT GetPrimarySize(SAP_M_CHARACTERISTIC_REQUEST param)
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
                            characteristic.NAME = "ZPKG_SEC_PRIMARY_SIZE";
                            characteristic.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItem(MapperServices.SAP_M_CHARACTERISTIC(characteristic), context));
                        }
                        else
                        {
                            param.data.NAME = "ZPKG_SEC_PRIMARY_SIZE";
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

        public static SAP_M_3P_RESULT GetPrimarySizeXecm(SAP_M_3P_REQUEST param)
        {
            SAP_M_3P_RESULT Results = new SAP_M_3P_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        Results.data = MapperServices.SAP_M_3P(SAP_M_3P_SERVICE.GetAll(context));
                    }
                }
                foreach (var item in Results.data)
                {
                    if (Results.data.Where(m => m.PRIMARY_SIZE_VALUE.Trim() == item.PRIMARY_SIZE_VALUE.Trim()).Count() > 1)
                    {
                        item.PRIMARY_SIZE_VALUE = "Delete";
                    }
                }
                Results.data = Results.data.Where(m => m.PRIMARY_SIZE_VALUE != "Delete").ToList();

                if (Results.data.Count > 0)
                {
                    for (int i = 0; i < Results.data.Count; i++)
                    {
                        Results.data[i].ID = Results.data[i].THREE_P_ID;
                        Results.data[i].DISPLAY_TXT = Results.data[i].PRIMARY_SIZE_VALUE;
                    }

                    if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                    {
                        Results.data = (from u1 in Results.data
                                        where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                        select u1).ToList();
                    }

                    Results.data = Results.data.OrderBy(x => x.DISPLAY_TXT).ToList();
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
