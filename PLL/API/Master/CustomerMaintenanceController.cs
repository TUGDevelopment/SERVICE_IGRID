using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;
using DAL;

namespace PLL.API
{
    public class CustomerMaintenanceController : ApiController
    {
        [Route("api/master/customer")]
        public ART_M_USER_CUSTOMER_RESULT PostCustomerRequest(ART_M_USER_CUSTOMER_REQUEST_LIST param)
        {
            ART_M_USER_CUSTOMER_RESULT Results = new ART_M_USER_CUSTOMER_RESULT();
            try
            {
                var isNewCustomer = false;
                var customerName = "";
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                      
                        if (param != null || param.data != null)
                        {
                            foreach (var item in param.data)
                            {
                                var tempUSER_COMPANY = ART_M_USER_COMPANY_SERVICE.GetByItem(new ART_M_USER_COMPANY() { COMPANY_ID = item.CUSTOMER_ID }, context);
                                if (tempUSER_COMPANY.Count == 0)
                                {
                                    isNewCustomer = true;
                                    customerName = CNService.GetCustomerCodeName(item.CUSTOMER_ID, context);
                                }

                                var temp = ART_M_USER_CUSTOMER_SERVICE.GetByItem(new ART_M_USER_CUSTOMER() { USER_ID = item.USER_ID }, context).FirstOrDefault();
                                if (item.CUSTOMER_ID != 0)
                                {
                                    if (temp != null)
                                    {
                                        item.USER_CUSTOMER_ID = temp.USER_CUSTOMER_ID;
                                        ART_M_USER_CUSTOMER_SERVICE.SaveOrUpdate(MapperServices.ART_M_USER_CUSTOMER(item), context);
                                    }
                                    else
                                        ART_M_USER_CUSTOMER_SERVICE.SaveOrUpdate(MapperServices.ART_M_USER_CUSTOMER(item), context);
                                }
                                else if (temp != null && item.CUSTOMER_ID == 0)
                                {
                                    item.USER_CUSTOMER_ID = ART_M_USER_CUSTOMER_SERVICE.GetByItem(new ART_M_USER_CUSTOMER() { USER_ID = item.USER_ID }, context).FirstOrDefault().USER_CUSTOMER_ID;
                                    ART_M_USER_CUSTOMER_SERVICE.DeleteByUSER_CUSTOMER_ID(item.USER_CUSTOMER_ID, context);
                                }
                            }
                        }

                        dbContextTransaction.Commit();

                        Results.status = "S";
                        //Results.msg = MessageHelper.GetMessage("MSG_001");
                    }
                }

                if (isNewCustomer)
                {
                    var emailto = "";
                    using (var context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            var tempUser = ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_SERVICE.GetAll(context);
                          
                            foreach (var item in tempUser)
                            {
                                var userInfo = ART_M_USER_SERVICE.GetByUSER_ID(item.USER_ID, context);
                                if (userInfo.IS_ACTIVE == "X")
                                {
                                    if (emailto == "") emailto = userInfo.EMAIL;
                                    else emailto += "," + userInfo.EMAIL;
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(customerName))
                        BLL.Services.EmailService.SendEmailNewCustomer(emailto, customerName);
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }


        [Route("api/master/getcustomers")]
        public XECM_M_CUSTOMER_RESULT GetCustomerRequest([FromUri]XECM_M_CUSTOMER_REQUEST param)
        {
            XECM_M_CUSTOMER_RESULT Results = new XECM_M_CUSTOMER_RESULT();


       

            var list = new List<XECM_M_CUSTOMER>();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;
                        if (param != null || param.data != null)
                        {
                            IQueryable<XECM_M_CUSTOMER> q = null;

                            q = (from m in context.XECM_M_CUSTOMER select m);


                            if (!string.IsNullOrEmpty(param.data.CUSTOMER_CODE))
                            {
                                q = (from r in q where (r.CUSTOMER_CODE.Contains(param.data.CUSTOMER_CODE.Trim())) select r);
                            }


                            if (!string.IsNullOrEmpty(param.data.CUSTOMER_NAME))
                            {
                                q = (from r in q where (r.CUSTOMER_NAME.Contains(param.data.CUSTOMER_NAME.Trim())) select r);
                            }

                            if (!string.IsNullOrEmpty(param.search.value))
                            {
                                param.search.value = param.search.value.Trim();

                                q = q.Where(m => m.CUSTOMER_CODE.Contains(param.search.value)
                                || m.CUSTOMER_NAME.Contains(param.search.value));                            
                            }

                            int cnt = q.Count();

                            var orderColumn = 1;
                            var orderDir = "asc";
                            if (param.order != null && param.order.Count > 0)
                            {
                                orderColumn = param.order[0].column;
                                orderDir = param.order[0].dir; //desc ,asc
                            }

                            string orderASC = "asc";
                            string orderDESC = "desc";

                            if (orderColumn == 1)
                            {
                                if (orderDir == orderASC)
                                    list = q.OrderBy(m => m.CUSTOMER_CODE).Skip(param.start).Take(param.length).ToList();
                                else if (orderDir == orderDESC)
                                    list = q.OrderByDescending(m => m.CUSTOMER_CODE).Skip(param.start).Take(param.length).ToList();
                            }
                            if (orderColumn == 2)
                            {
                                if (orderDir == orderASC)
                                    list = q.OrderBy(m => m.CUSTOMER_NAME).Skip(param.start).Take(param.length).ToList();
                                else if (orderDir == orderDESC)
                                    list = q.OrderByDescending(m => m.CUSTOMER_NAME).Skip(param.start).Take(param.length).ToList();
                            }
                            if (orderColumn == 3)
                            {
                                if (orderDir == orderASC)
                                    list = q.OrderBy(m => m.IS_SHADE_LIMIT).Skip(param.start).Take(param.length).ToList();
                                else if (orderDir == orderDESC)
                                    list = q.OrderByDescending(m => m.IS_SHADE_LIMIT).Skip(param.start).Take(param.length).ToList();
                            }
                            if (orderColumn == 4)
                            {
                                if (orderDir == orderASC)
                                    list = q.OrderBy(m => m.IS_ACTIVE).Skip(param.start).Take(param.length).ToList();
                                else if (orderDir == orderDESC)
                                    list = q.OrderByDescending(m => m.IS_ACTIVE).Skip(param.start).Take(param.length).ToList();
                            }

                            Results.draw = param.draw;
                            Results.recordsTotal = cnt;
                            Results.recordsFiltered = cnt;
                            Results.data = MapperServices.XECM_M_CUSTOMER(list);

                            Results.status = "S";

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

