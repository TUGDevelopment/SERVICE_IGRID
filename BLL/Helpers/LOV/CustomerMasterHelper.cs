using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services;
using DAL;
using DAL.Model;

namespace BLL.Helpers
{
    public class CustomerMasterHelper
    {
        public static XECM_M_CUSTOMER_RESULT GetCustomer(XECM_M_CUSTOMER_REQUEST param)
        {
            XECM_M_CUSTOMER_RESULT Results = new XECM_M_CUSTOMER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            XECM_M_CUSTOMER_2 customer = new XECM_M_CUSTOMER_2();
                            customer.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_CUSTOMER(XECM_M_CUSTOMER_SERVICE.GetByItem(MapperServices.SAP_M_CUSTOMER(customer), context));
                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_CUSTOMER(XECM_M_CUSTOMER_SERVICE.GetByItem(MapperServices.SAP_M_CUSTOMER(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].CUSTOMER_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].CUSTOMER_CODE + ":" + Results.data[i].CUSTOMER_NAME;
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
        public static XECM_M_CUSTOMER_RESULT GetCustomerOther(XECM_M_CUSTOMER_REQUEST param)
        {
            XECM_M_CUSTOMER_RESULT Results = new XECM_M_CUSTOMER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var artwork_username = CNService.curruser();
                        var query = context.Database.SqlQuery<XECM_M_CUSTOMER_2>("spGetSAP_M_CUSTOMER @user,@IS_ACTIVE",
                                        new SqlParameter("@user", string.Format("{0}", artwork_username)),
                                        new SqlParameter("@IS_ACTIVE", string.Format("{0}", "X"))                                       
                                        )
                                        .ToList();
                        Results.data = query.Select(m => new XECM_M_CUSTOMER_2()
                        {
                
                            ID = m.CUSTOMER_ID,
                            DISPLAY_TXT = string.Format("{0}", m.CUSTOMER_CODE + ":" + m.CUSTOMER_NAME),

                        }).ToList();

                        if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                        {
                            Results.data = (from u1 in Results.data
                                            where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                            select u1).ToList();
                        }

                        //if (param == null || param.data == null)
                        //{
                        //    XECM_M_CUSTOMER_2 customer = new XECM_M_CUSTOMER_2();
                        //    customer.IS_ACTIVE = "X";
                        //    Results.data = MapperServices.SAP_M_CUSTOMER(XECM_M_CUSTOMER_SERVICE.GetByItem(MapperServices.SAP_M_CUSTOMER(customer), context));
                        //}
                        //else
                        //{
                        //    param.data.IS_ACTIVE = "X";
                        //    Results.data = MapperServices.SAP_M_CUSTOMER(XECM_M_CUSTOMER_SERVICE.GetByItem(MapperServices.SAP_M_CUSTOMER(param.data), context));
                        //}

                        //if (Results.data.Count > 0)
                        //{
                        //    for (int i = 0; i < Results.data.Count; i++)
                        //    {
                        //        Results.data[i].ID = Results.data[i].CUSTOMER_ID;
                        //        Results.data[i].DISPLAY_TXT = Results.data[i].CUSTOMER_CODE + ":" + Results.data[i].CUSTOMER_NAME;
                        //    }

                        //    if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                        //    {
                        //        Results.data = (from u1 in Results.data
                        //                        where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                        //                        select u1).ToList();
                        //    }

                        //    Results.data = Results.data.OrderBy(x => x.DISPLAY_TXT).ToList();
                        //}
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

        public static XECM_M_CUSTOMER_RESULT GetCompanySoldToSO(XECM_M_CUSTOMER_REQUEST param)
        {
            XECM_M_CUSTOMER_RESULT Results = new XECM_M_CUSTOMER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        Results.data = (from p in context.V_SAP_SALES_ORDER select new XECM_M_CUSTOMER_2() { DISPLAY_TXT = p.SOLD_TO + ":" + p.SOLD_TO_NAME }).Distinct().ToList();
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

        public static XECM_M_CUSTOMER_RESULT GetCompanyShipToSO(XECM_M_CUSTOMER_REQUEST param)
        {
            XECM_M_CUSTOMER_RESULT Results = new XECM_M_CUSTOMER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        Results.data = (from p in context.V_SAP_SALES_ORDER select new XECM_M_CUSTOMER_2() { DISPLAY_TXT = p.SHIP_TO + ":" + p.SHIP_TO_NAME }).Distinct().ToList();
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
