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
    public class CountryHelper
    {
        public static SAP_M_COUNTRY_RESULT GetCountry(SAP_M_COUNTRY_REQUEST param)
        {
            SAP_M_COUNTRY_RESULT Results = new SAP_M_COUNTRY_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            SAP_M_COUNTRY_2 country_2 = new SAP_M_COUNTRY_2();
                            country_2.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_COUNTRY(SAP_M_COUNTRY_SERVICE.GetByItem(country_2, context));
                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_COUNTRY(SAP_M_COUNTRY_SERVICE.GetByItem(MapperServices.SAP_M_COUNTRY(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].COUNTRY_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].COUNTRY_CODE + ":" + Results.data[i].NAME;
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

        public static SAP_M_COUNTRY_RESULT GetCountrySO(SAP_M_COUNTRY_REQUEST param)
        {
            SAP_M_COUNTRY_RESULT Results = new SAP_M_COUNTRY_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        Results.data = (from p in context.V_SAP_SALES_ORDER
                                        join m in context.SAP_M_COUNTRY on p.COUNTRY equals m.COUNTRY_CODE
                                        select new SAP_M_COUNTRY_2() { DISPLAY_TXT = p.COUNTRY + ":" + m.NAME }).Distinct().ToList();

                        int i = 1;
                        foreach (var item in Results.data)
                        {

                            item.ID = i;
                            i++;
                        }
                    }
                }

                if (Results.data.Count > 0)
                {
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

        public static SAP_M_COUNTRY_RESULT GetCountryPIC(SAP_M_COUNTRY_REQUEST param)
        {
            SAP_M_COUNTRY_RESULT Results = new SAP_M_COUNTRY_RESULT();
            SAP_M_COUNTRY_REQUEST paramZone = new SAP_M_COUNTRY_REQUEST();
            SAP_M_COUNTRY_2 dataZone = new SAP_M_COUNTRY_2();
            SAP_M_COUNTRY_RESULT resultZone = new SAP_M_COUNTRY_RESULT();

            List<string> listZone = new List<string>();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            SAP_M_COUNTRY_2 country_2 = new SAP_M_COUNTRY_2();
                            country_2.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_COUNTRY(SAP_M_COUNTRY_SERVICE.GetByItem(country_2, context));
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(param.data.ZONE))
                            {
                                paramZone.data = dataZone;
                                resultZone = ZoneHelper.GetZone(paramZone);

                                if (resultZone.data.Count > 0)
                                {
                                    List<string> listRawZone = new List<string>();
                                    listRawZone = param.data.ZONE.Split(new string[] { "||" }, StringSplitOptions.None).ToList();

                                    if (listRawZone.Count > 0)
                                    {
                                        List<Int32> listZoneID = new List<Int32>();

                                        foreach (var itemZone in listRawZone)
                                        {
                                            int j;
                                            if (Int32.TryParse(itemZone, out j))
                                            {
                                                listZoneID.Add(j);
                                            }

                                        }
                                        listZone = resultZone.data.Where(w => listZoneID.Contains(w.ID)).Select(s => s.ZONE).ToList();
                                    }
                                }
                            }

                            if (listZone.Count > 0)
                            {
                                var countryies = (from s in context.SAP_M_COUNTRY
                                                  where listZone.Contains(s.ZONE)
                                                  && s.IS_ACTIVE == "X"
                                                  select s).ToList();

                                if (countryies != null)
                                {
                                    Results.data = MapperServices.SAP_M_COUNTRY(countryies);
                                }
                            }
                            else
                            {
                                param.data.IS_ACTIVE = "X";
                                Results.data = MapperServices.SAP_M_COUNTRY(SAP_M_COUNTRY_SERVICE.GetByItem(MapperServices.SAP_M_COUNTRY(param.data), context));
                            }
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].COUNTRY_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].COUNTRY_CODE + ":" + Results.data[i].NAME;
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
