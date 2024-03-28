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
    public class PackagingSecHelper
    {
        public static SAP_M_CHARACTERISTIC_RESULT GetPackaingSecMaterial(SAP_M_CHARACTERISTIC_REQUEST param)
        {
            SAP_M_CHARACTERISTIC_RESULT Results = new SAP_M_CHARACTERISTIC_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    if (param == null || param.data == null)
                    {
                        SAP_M_CHARACTERISTIC_2 characteristic = new SAP_M_CHARACTERISTIC_2();
                        characteristic.NAME = "ZPKG_SEC_MATERIAL";
                        characteristic.IS_ACTIVE = "X";
                        Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItem(MapperServices.SAP_M_CHARACTERISTIC(characteristic), context));
                    }
                    else
                    {
                        
                        param.data.NAME = "ZPKG_SEC_MATERIAL";
                        param.data.IS_ACTIVE = "X";
                        Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItem(MapperServices.SAP_M_CHARACTERISTIC(param.data), context));
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
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static SAP_M_CHARACTERISTIC_RESULT GetPackaingSecPlasticType(SAP_M_CHARACTERISTIC_REQUEST param)
        {
            SAP_M_CHARACTERISTIC_RESULT Results = new SAP_M_CHARACTERISTIC_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    if (param == null || param.data == null)
                    {
                        SAP_M_CHARACTERISTIC_2 characteristic = new SAP_M_CHARACTERISTIC_2();
                        characteristic.NAME = "ZPKG_SEC_PLASTIC";
                        characteristic.IS_ACTIVE = "X";
                        Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItem(MapperServices.SAP_M_CHARACTERISTIC(characteristic), context));
                    }
                    else
                    {

                        param.data.NAME = "ZPKG_SEC_PLASTIC";
                        param.data.IS_ACTIVE = "X";
                        Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItem(MapperServices.SAP_M_CHARACTERISTIC(param.data), context));
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
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }


            return Results;
        }

        public static SAP_M_CHARACTERISTIC_RESULT GetPackaingSecCertSource(SAP_M_CHARACTERISTIC_REQUEST param)
        {
            SAP_M_CHARACTERISTIC_RESULT Results = new SAP_M_CHARACTERISTIC_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    if (param == null || param.data == null)
                    {
                        SAP_M_CHARACTERISTIC_2 characteristic = new SAP_M_CHARACTERISTIC_2();
                        characteristic.NAME = "ZPKG_SEC_CERT_SOURCE";
                        characteristic.IS_ACTIVE = "X";
                        Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItem(MapperServices.SAP_M_CHARACTERISTIC(characteristic), context));
                    }
                    else
                    {

                        param.data.NAME = "ZPKG_SEC_CERT_SOURCE";
                        param.data.IS_ACTIVE = "X";
                        Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItem(MapperServices.SAP_M_CHARACTERISTIC(param.data), context));
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
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }


            return Results;
        }
    }
}
