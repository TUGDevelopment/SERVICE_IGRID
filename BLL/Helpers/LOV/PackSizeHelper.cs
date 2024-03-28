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
    public class PackSizeHelper
    {
        public static SAP_M_CHARACTERISTIC_RESULT GetPackSize(SAP_M_CHARACTERISTIC_REQUEST param)
        {
            SAP_M_CHARACTERISTIC_RESULT Results = new SAP_M_CHARACTERISTIC_RESULT();
            List<SAP_M_CHARACTERISTIC_2> listResult = new List<SAP_M_CHARACTERISTIC_2>();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            SAP_M_CHARACTERISTIC_2 characteristic = new SAP_M_CHARACTERISTIC_2();
                            characteristic.NAME = "ZPKG_SEC_PACKING";
                            characteristic.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItem(MapperServices.SAP_M_CHARACTERISTIC(characteristic), context));
                        }
                        else
                        {
                            param.data.NAME = "ZPKG_SEC_PACKING";
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

                            foreach (var thing in Results.data.OrderBy(x => x.DISPLAY_TXT, new SemiNumericComparer()))
                            {
                                listResult.Add(thing);
                            }

                            Results.data = listResult.ToList();
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

        public static SAP_M_2P_RESULT GetPackSizeXecm(SAP_M_2P_REQUEST param)
        {
            SAP_M_2P_RESULT Results = new SAP_M_2P_RESULT();
            List<SAP_M_2P_2> listResult = new List<SAP_M_2P_2>();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        Results.data = MapperServices.SAP_M_2P(SAP_M_2P_SERVICE.GetAll(context));
                    }
                }

                foreach (var item in Results.data)
                {
                    if (Results.data.Where(m => m.PACK_SIZE_VALUE.Trim() == item.PACK_SIZE_VALUE.Trim()).Count() > 1)
                    {
                        item.PACK_SIZE_VALUE = "Delete";
                    }
                }
                Results.data = Results.data.Where(m => m.PACK_SIZE_VALUE != "Delete").ToList();

                if (Results.data.Count > 0)
                {
                    for (int i = 0; i < Results.data.Count; i++)
                    {
                        Results.data[i].ID = Results.data[i].TWO_P_ID;
                        Results.data[i].DISPLAY_TXT = Results.data[i].PACK_SIZE_VALUE;
                    }

                    if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                    {
                        Results.data = (from u1 in Results.data
                                        where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                        select u1).ToList();
                    }

                    foreach (var thing in Results.data.OrderBy(x => x.DISPLAY_TXT, new SemiNumericComparer()))
                    {
                        listResult.Add(thing);
                    }

                    Results.data = listResult.ToList();

                    //Results.data = Results.data.OrderBy(x => x.DISPLAY_TXT).ToList();
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

        public class SemiNumericComparer : IComparer<string>
        {
            public int Compare(string s1, string s2)
            {
                if (IsNumeric(s1) && IsNumeric(s2))
                {
                    if (Convert.ToInt32(s1) > Convert.ToInt32(s2)) return 1;
                    if (Convert.ToInt32(s1) < Convert.ToInt32(s2)) return -1;
                    if (Convert.ToInt32(s1) == Convert.ToInt32(s2)) return 0;
                }

                if (IsNumeric(s1) && !IsNumeric(s2))
                    return -1;

                if (!IsNumeric(s1) && IsNumeric(s2))
                    return 1;

                return string.Compare(s1, s2, true);
            }

            public static bool IsNumeric(object value)
            {
                try
                {
                    int i = Convert.ToInt32(value.ToString());
                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
            }
        }
    }
}
