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
    public class ZoneHelper
    {
        public static SAP_M_COUNTRY_RESULT GetZone(SAP_M_COUNTRY_REQUEST param)
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
                                if (Results.data.Where(m => m.ZONE == Results.data[i].ZONE).Count() > 1)
                                {
                                    Results.data[i].ZONE = "Delete";
                                }
                            }

                            Results.data = Results.data.Where(m => !string.IsNullOrEmpty(m.ZONE)).ToList();
                            Results.data = Results.data.Where(m => m.ZONE != "Delete").ToList();

                            int cnt = 1;
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = cnt;
                                cnt++;
                                Results.data[i].DISPLAY_TXT = Results.data[i].ZONE;
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
    }
}
