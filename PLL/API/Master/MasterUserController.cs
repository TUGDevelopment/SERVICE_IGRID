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
using PLL.Controllers;
using System.Data;
using System.ComponentModel;

namespace PLL.API
{
    public class MasterUserController : ApiController
    {
        [Route("api/master/user")]
        public UsersResultModel GetMasterUser([FromUri]UsersRequestModel param)
        {
            UsersResultModel Results = new UsersResultModel();
            try
            {
                int cnt = 0;
                var model = GetList2(param, ref cnt);

                Results.recordsTotal = cnt;
                Results.recordsFiltered = cnt;

                Results.data = model.ObjUserAll;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        private UsersModel GetList2(UsersRequestModel param, ref int cnt)
        {
            UsersModel model = new UsersModel();

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    model.ObjUserAll = MapperServices.ART_M_USER(QueryUser(param, ref cnt));

                    var allRole = ART_M_ROLE_SERVICE.GetAll(context);
                    var allCompany = SAP_M_COMPANY_SERVICE.GetAll(context);
                    var allTypeOfProduct = SAP_M_TYPE_OF_PRODUCT_SERVICE.GetAll(context);
                    var allPosition = ART_M_POSITION_SERVICE.GetAll(context);

                    List<ART_M_USER_2> listUser = new List<ART_M_USER_2>();
                    foreach (var userall in model.ObjUserAll)
                    {
                        var listRoleID = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { USER_ID = userall.USER_ID }, context);
                        var listLeadID = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { USER_ID = userall.USER_ID }, context);
                        var listCompanyID = ART_M_USER_COMPANY_SERVICE.GetByItem(new ART_M_USER_COMPANY() { USER_ID = userall.USER_ID }, context);
                        var listTypyProductID = ART_M_USER_TYPE_OF_PRODUCT_SERVICE.GetByItem(new ART_M_USER_TYPE_OF_PRODUCT() { USER_ID = userall.USER_ID }, context);
                        var listCustomerID = ART_M_USER_CUSTOMER_SERVICE.GetByItem(new ART_M_USER_CUSTOMER() { USER_ID = userall.USER_ID }, context);
                        var listVendorID = ART_M_USER_VENDOR_SERVICE.GetByItem(new ART_M_USER_VENDOR() { USER_ID = userall.USER_ID }, context);
                        var listPositionID = ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { USER_ID = userall.USER_ID }, context);

                        foreach (var item in listRoleID)
                        {
                            var temp = allRole.Where(m => m.ROLE_ID == item.ROLE_ID).FirstOrDefault();
                            if (temp != null)
                                if (string.IsNullOrEmpty(userall.ROLE_DISPLAY_TXT))
                                    userall.ROLE_DISPLAY_TXT = temp.DESCRIPTION;
                                else
                                    userall.ROLE_DISPLAY_TXT += "<br/>" + temp.DESCRIPTION;
                        }

                        foreach (var item in listLeadID)
                        {
                            var temp = ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { USER_ID = item.UPPER_USER_ID }, context).FirstOrDefault();
                            if (temp != null)
                                if (string.IsNullOrEmpty(userall.USER_LEADER_DISPLAY_TXT))
                                    userall.USER_LEADER_DISPLAY_TXT = temp.TITLE + " " + temp.FIRST_NAME + " " + temp.LAST_NAME;
                                else
                                    userall.USER_LEADER_DISPLAY_TXT += "<br/>" + temp.TITLE + " " + temp.FIRST_NAME + " " + temp.LAST_NAME;
                        }

                        foreach (var item in listCompanyID)
                        {
                            var temp = allCompany.Where(m => m.COMPANY_ID == item.COMPANY_ID).FirstOrDefault();
                            if (temp != null)
                                if (string.IsNullOrEmpty(userall.COMPANY_DISPLAY_TXT))
                                    userall.COMPANY_DISPLAY_TXT = temp.DESCRIPTION;
                                else
                                    userall.COMPANY_DISPLAY_TXT += "<br/>" + temp.DESCRIPTION;
                        }

                        foreach (var item in listTypyProductID)
                        {
                            var temp = allTypeOfProduct.Where(m => m.TYPE_OF_PRODUCT_ID == item.TYPE_OF_PRODUCT_ID).FirstOrDefault();
                            if (temp != null)
                                if (string.IsNullOrEmpty(userall.TYPE_OF_PRODUCT_DISPLAY_TXT))
                                    userall.TYPE_OF_PRODUCT_DISPLAY_TXT = temp.DESCRIPTION;
                                else
                                    userall.TYPE_OF_PRODUCT_DISPLAY_TXT += "<br/>" + temp.DESCRIPTION;
                        }

                        foreach (var item in listCustomerID)
                        {
                            var temp = XECM_M_CUSTOMER_SERVICE.GetByItem(new XECM_M_CUSTOMER() { CUSTOMER_ID = item.CUSTOMER_ID }, context).FirstOrDefault();
                            if (temp != null)
                                if (string.IsNullOrEmpty(userall.CUSTOMER_DISPLAY_TXT))
                                    userall.CUSTOMER_DISPLAY_TXT = temp.CUSTOMER_CODE + ":" + temp.CUSTOMER_NAME;
                                else
                                    userall.CUSTOMER_DISPLAY_TXT += "<br/>" + temp.CUSTOMER_CODE + ":" + temp.CUSTOMER_NAME;
                        }

                        foreach (var item in listVendorID)
                        {
                            var temp = XECM_M_VENDOR_SERVICE.GetByItem(new XECM_M_VENDOR() { VENDOR_ID = item.VENDOR_ID }, context).FirstOrDefault();
                            if (temp != null)
                                if (string.IsNullOrEmpty(userall.VENDOR_DISPLAY_TXT))
                                    userall.VENDOR_DISPLAY_TXT = temp.VENDOR_CODE + ":" + temp.VENDOR_NAME;
                                else
                                    userall.VENDOR_DISPLAY_TXT += "<br/>" + temp.VENDOR_CODE + ":" + temp.VENDOR_NAME;
                        }

                        foreach (var item in listPositionID)
                        {
                            var temp = allPosition.Where(m => m.ART_M_POSITION_ID == item.POSITION_ID).FirstOrDefault();
                            if (temp != null)
                                if (string.IsNullOrEmpty(userall.POSITION_DISPLAY_TXT))
                                    userall.POSITION_DISPLAY_TXT = temp.ART_M_POSITION_NAME;
                        }

                        listUser.Add(userall);
                    }

                    model.ObjUserAll = listUser;
                }
            }
            return model;
        }

        public static List<ART_M_USER> QueryUser(UsersRequestModel param, ref int cnt)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var currentUser = ART_M_USER_SERVICE.GetByUSER_ID(param.data.CURRENT_USER_ID, context);

                    var PosID_Customer = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "CUSTOMER" }, context).FirstOrDefault().ART_M_POSITION_ID;
                    var PosID_VENDOR = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "VENDOR" }, context).FirstOrDefault().ART_M_POSITION_ID;
                    var PosID_NOT_FOUND = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "NOT_FOUND" }, context).FirstOrDefault().ART_M_POSITION_ID;

                    IQueryable<ART_M_USER> q = null;

                    if (param.data.type2 == "Internal")
                    {
                        q = (from u1 in context.ART_M_USER
                             where (u1.POSITION_ID == currentUser.POSITION_ID || u1.POSITION_ID == null || u1.POSITION_ID == PosID_NOT_FOUND)
                             select u1);
                    }
                    else if (param.data.type2 == "Customer")
                    {
                        q = (from u1 in context.ART_M_USER
                             where u1.POSITION_ID == PosID_Customer
                             select u1);
                    }
                    else if (param.data.type2 == "Vendor")
                    {
                        q = (from u1 in context.ART_M_USER
                             where u1.POSITION_ID == PosID_VENDOR
                             select u1);
                    }
                    else if (param.data.type2 == "Admin")
                    {
                        q = (from u1 in context.ART_M_USER
                             select u1);
                    }
                    if (param.data.type2 == "Internal")
                    {
                        if (!string.IsNullOrEmpty(param.search.value))
                        {
                            var gobalSearch = param.search.value.ToUpper();
                            var posid = CNService.GetPositionID(gobalSearch, context);

                            var listUserId = (from m2 in context.ART_M_USER_UPPER_LEVEL
                                              join m3 in context.ART_M_USER on m2.UPPER_USER_ID equals m3.USER_ID into ps2
                                              from m3 in ps2.DefaultIfEmpty()

                                              join t2 in context.ART_M_USER on m2.USER_ID equals t2.USER_ID into ps
                                              from t2 in ps.DefaultIfEmpty()

                                              where t2.TITLE.ToUpper().Contains(gobalSearch) ||
                                                    t2.FIRST_NAME.ToUpper().Contains(gobalSearch) ||
                                                    t2.LAST_NAME.ToUpper().Contains(gobalSearch) ||
                                                    t2.EMAIL.ToUpper().Contains(gobalSearch) ||
                                                    //t2.USERNAME.ToUpper().Contains(gobalSearch) ||
                                                    m3.TITLE.ToUpper().Contains(gobalSearch) ||
                                                    m3.FIRST_NAME.ToUpper().Contains(gobalSearch) ||
                                                    m3.LAST_NAME.ToUpper().Contains(gobalSearch) ||
                                                    m3.EMAIL.ToUpper().Contains(gobalSearch)
                                              //(m3.FIRST_NAME + ' ' + m3.LAST_NAME).ToUpper().Contains(gobalSearch)
                                              select m2.USER_ID).ToList();


                            var listUserId2 = (from t2 in context.ART_M_USER
                                               where t2.TITLE.ToUpper().Contains(gobalSearch) ||
                                                     t2.FIRST_NAME.ToUpper().Contains(gobalSearch) ||
                                                     t2.LAST_NAME.ToUpper().Contains(gobalSearch) ||
                                                     t2.EMAIL.ToUpper().Contains(gobalSearch) ||
                                                     t2.POSITION_ID == posid ||
                                                     t2.USERNAME.ToUpper().Contains(gobalSearch)
                                               select t2.USER_ID).ToList();
                            listUserId.AddRange(listUserId2);

                            q = (from m in q

                                 join t2 in context.ART_M_USER_ROLE on m.USER_ID equals t2.USER_ID into ps
                                 from t2 in ps.DefaultIfEmpty()
                                 join t3 in context.ART_M_ROLE on t2.ROLE_ID equals t3.ROLE_ID into ps2
                                 from t3 in ps2.DefaultIfEmpty()

                                 join t4 in context.ART_M_USER_COMPANY on m.USER_ID equals t4.USER_ID into ps3
                                 from t4 in ps3.DefaultIfEmpty()
                                 join t5 in context.SAP_M_COMPANY on t4.COMPANY_ID equals t5.COMPANY_ID into ps4
                                 from t5 in ps4.DefaultIfEmpty()

                                     //join t6 in context.ART_M_USER_UPPER_LEVEL on m.USER_ID equals t6.USER_ID into ps5
                                     //from t6 in ps5.DefaultIfEmpty()
                                     //join t7 in context.ART_M_USER on t6.USER_ID equals t7.USER_ID into ps6
                                     //from t7 in ps6.DefaultIfEmpty()

                                 join t8 in context.ART_M_USER_TYPE_OF_PRODUCT on m.USER_ID equals t8.USER_ID into ps7
                                 from t8 in ps7.DefaultIfEmpty()
                                 join t9 in context.SAP_M_TYPE_OF_PRODUCT on t8.TYPE_OF_PRODUCT_ID equals t9.TYPE_OF_PRODUCT_ID into ps8
                                 from t9 in ps8.DefaultIfEmpty()

                                 where listUserId.Contains(m.USER_ID) ||

                                            //where m.USERNAME.ToUpper().Contains(gobalSearch) ||
                                            //           m.TITLE.ToUpper().Contains(gobalSearch) ||
                                            //           m.FIRST_NAME.ToUpper().Contains(gobalSearch) ||
                                            //           m.LAST_NAME.ToUpper().Contains(gobalSearch) ||
                                            //           m.EMAIL.ToUpper().Contains(gobalSearch) ||
                                            //           m.USERNAME.ToUpper().Contains(gobalSearch) ||

                                            t3.DESCRIPTION.ToUpper().Contains(gobalSearch) ||
                                            t5.COMPANY_CODE.ToUpper().Contains(gobalSearch) ||
                                            t5.DESCRIPTION.ToUpper().Contains(gobalSearch) ||
                                            //t7.TITLE.ToUpper().Contains(gobalSearch) ||
                                            //t7.FIRST_NAME.ToUpper().Contains(gobalSearch) ||
                                            //t7.LAST_NAME.ToUpper().Contains(gobalSearch) ||
                                            t9.DESCRIPTION.ToUpper().Contains(gobalSearch)
                                 select m).Distinct();
                        }
                    }
                    else if (param.data.type2 == "Customer")
                    {
                        if (!string.IsNullOrEmpty(param.search.value))
                        {
                            var gobalSearch = param.search.value.ToUpper();
                            q = (from m in q
                                 join t2 in context.ART_M_USER_CUSTOMER on m.USER_ID equals t2.USER_ID into ps
                                 from t2 in ps.DefaultIfEmpty()
                                 join t3 in context.XECM_M_CUSTOMER on t2.CUSTOMER_ID equals t3.CUSTOMER_ID into ps2
                                 from t3 in ps2.DefaultIfEmpty()
                                 where m.USERNAME.ToUpper().Contains(gobalSearch) ||
                                            m.TITLE.ToUpper().Contains(gobalSearch) ||
                                            m.FIRST_NAME.ToUpper().Contains(gobalSearch) ||
                                            m.LAST_NAME.ToUpper().Contains(gobalSearch) ||
                                            m.EMAIL.ToUpper().Contains(gobalSearch) ||
                                            t3.CUSTOMER_CODE.ToUpper().Contains(gobalSearch) ||
                                            t3.CUSTOMER_NAME.ToUpper().Contains(gobalSearch)
                                 select m);
                        }
                    }
                    else if (param.data.type2 == "Vendor")
                    {
                        if (!string.IsNullOrEmpty(param.search.value))
                        {
                            var gobalSearch = param.search.value.ToUpper();
                            q = (from m in q
                                 join t2 in context.ART_M_USER_VENDOR on m.USER_ID equals t2.USER_ID into ps
                                 from t2 in ps.DefaultIfEmpty()
                                 join t3 in context.XECM_M_VENDOR on t2.VENDOR_ID equals t3.VENDOR_ID into ps2
                                 from t3 in ps2.DefaultIfEmpty()
                                 where m.USERNAME.ToUpper().Contains(gobalSearch) ||
                                            m.TITLE.ToUpper().Contains(gobalSearch) ||
                                            m.FIRST_NAME.ToUpper().Contains(gobalSearch) ||
                                            m.LAST_NAME.ToUpper().Contains(gobalSearch) ||
                                            m.EMAIL.ToUpper().Contains(gobalSearch) ||
                                            t3.VENDOR_CODE.ToUpper().Contains(gobalSearch) ||
                                            t3.VENDOR_NAME.ToUpper().Contains(gobalSearch)
                                 select m);
                        }
                    }
                    else if (param.data.type2 == "Admin")
                    {
                        if (!string.IsNullOrEmpty(param.search.value))
                        {
                            var gobalSearch = param.search.value.ToUpper();
                            var posid = CNService.GetPositionID(gobalSearch, context);

                            var listUserId = (from m2 in context.ART_M_USER_UPPER_LEVEL
                                              join m3 in context.ART_M_USER on m2.UPPER_USER_ID equals m3.USER_ID into ps2
                                              from m3 in ps2.DefaultIfEmpty()

                                              join t2 in context.ART_M_USER on m2.USER_ID equals t2.USER_ID into ps
                                              from t2 in ps.DefaultIfEmpty()

                                              where t2.TITLE.ToUpper().Contains(gobalSearch) ||
                                                    t2.FIRST_NAME.ToUpper().Contains(gobalSearch) ||
                                                    t2.LAST_NAME.ToUpper().Contains(gobalSearch) ||
                                                    //t2.USERNAME.ToUpper().Contains(gobalSearch) ||
                                                    t2.EMAIL.ToUpper().Contains(gobalSearch) ||
                                                    m3.TITLE.ToUpper().Contains(gobalSearch) ||
                                                    m3.FIRST_NAME.ToUpper().Contains(gobalSearch) ||
                                                    m3.LAST_NAME.ToUpper().Contains(gobalSearch) ||
                                                    m3.EMAIL.ToUpper().Contains(gobalSearch)
                                              select m2.USER_ID).ToList();

                            var listUserId2 = (from t2 in context.ART_M_USER
                                               where t2.TITLE.ToUpper().Contains(gobalSearch) ||
                                                     t2.FIRST_NAME.ToUpper().Contains(gobalSearch) ||
                                                     t2.LAST_NAME.ToUpper().Contains(gobalSearch) ||
                                                     t2.EMAIL.ToUpper().Contains(gobalSearch) ||
                                                     t2.POSITION_ID == posid ||
                                                     t2.USERNAME.ToUpper().Contains(gobalSearch)
                                               select t2.USER_ID).ToList();
                            listUserId.AddRange(listUserId2);

                            q = (from m in q

                                 join t2 in context.ART_M_USER_ROLE on m.USER_ID equals t2.USER_ID into ps
                                 from t2 in ps.DefaultIfEmpty()
                                 join t3 in context.ART_M_ROLE on t2.ROLE_ID equals t3.ROLE_ID into ps2
                                 from t3 in ps2.DefaultIfEmpty()

                                 join t4 in context.ART_M_USER_COMPANY on m.USER_ID equals t4.USER_ID into ps3
                                 from t4 in ps3.DefaultIfEmpty()
                                 join t5 in context.SAP_M_COMPANY on t4.COMPANY_ID equals t5.COMPANY_ID into ps4
                                 from t5 in ps4.DefaultIfEmpty()

                                     //join t6 in context.ART_M_USER_UPPER_LEVEL on m.USER_ID equals t6.USER_ID into ps5
                                     //from t6 in ps5.DefaultIfEmpty()
                                     //join t7 in context.ART_M_USER on t6.USER_ID equals t7.USER_ID into ps6
                                     //from t7 in ps6.DefaultIfEmpty()

                                 join t8 in context.ART_M_USER_TYPE_OF_PRODUCT on m.USER_ID equals t8.USER_ID into ps7
                                 from t8 in ps7.DefaultIfEmpty()
                                 join t9 in context.SAP_M_TYPE_OF_PRODUCT on t8.TYPE_OF_PRODUCT_ID equals t9.TYPE_OF_PRODUCT_ID into ps8
                                 from t9 in ps8.DefaultIfEmpty()


                                 join t222 in context.ART_M_USER_CUSTOMER on m.USER_ID equals t222.USER_ID into ps22
                                 from t222 in ps22.DefaultIfEmpty()
                                 join t322 in context.XECM_M_CUSTOMER on t222.CUSTOMER_ID equals t322.CUSTOMER_ID into ps222
                                 from t322 in ps222.DefaultIfEmpty()


                                 join t233 in context.ART_M_USER_VENDOR on m.USER_ID equals t233.USER_ID into ps33
                                 from t233 in ps33.DefaultIfEmpty()
                                 join t333 in context.XECM_M_VENDOR on t233.VENDOR_ID equals t333.VENDOR_ID into ps233
                                 from t333 in ps233.DefaultIfEmpty()


                                 where listUserId.Contains(m.USER_ID) ||

                            //m.USERNAME.ToUpper().Contains(gobalSearch) ||
                            //m.TITLE.ToUpper().Contains(gobalSearch) ||
                            //m.FIRST_NAME.ToUpper().Contains(gobalSearch) ||
                            //m.LAST_NAME.ToUpper().Contains(gobalSearch) ||
                            //m.EMAIL.ToUpper().Contains(gobalSearch) ||
                            //m.USERNAME.ToUpper().Contains(gobalSearch) ||

                            t3.DESCRIPTION.ToUpper().Contains(gobalSearch) ||
                                            t5.COMPANY_CODE.ToUpper().Contains(gobalSearch) ||
                                            t5.DESCRIPTION.ToUpper().Contains(gobalSearch) ||
                                            //t7.TITLE.ToUpper().Contains(gobalSearch) ||
                                            //t7.FIRST_NAME.ToUpper().Contains(gobalSearch) ||
                                            //t7.LAST_NAME.ToUpper().Contains(gobalSearch) ||
                                            t9.DESCRIPTION.ToUpper().Contains(gobalSearch) ||

                                            t322.CUSTOMER_CODE.ToUpper().Contains(gobalSearch) ||
                                            t322.CUSTOMER_NAME.ToUpper().Contains(gobalSearch) ||

                                            t333.VENDOR_CODE.ToUpper().Contains(gobalSearch) ||
                                            t333.VENDOR_NAME.ToUpper().Contains(gobalSearch)

                                 select m).Distinct();
                        }
                    }
                    cnt = q.Count();

                    return OrderByUser(q, param, context);
                }
            }
        }

        public static List<ART_M_USER> OrderByUser(IQueryable<ART_M_USER> q, UsersRequestModel param, ARTWORKEntities context)
        {
            var listUser = new List<ART_M_USER>();
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
                    listUser = q.OrderBy(i => i.USERNAME).Skip(param.start).Take(param.length).ToList();
                else if (orderDir == orderDESC)
                    listUser = q.OrderByDescending(i => i.USERNAME).Skip(param.start).Take(param.length).ToList();
            }
            if (orderColumn == 2)
            {
                if (orderDir == orderASC)
                    listUser = q.OrderBy(i => i.TITLE).Skip(param.start).Take(param.length).ToList();
                else if (orderDir == orderDESC)
                    listUser = q.OrderByDescending(i => i.TITLE).Skip(param.start).Take(param.length).ToList();
            }
            if (orderColumn == 3)
            {
                if (orderDir == orderASC)
                    listUser = q.OrderBy(i => i.FIRST_NAME).Skip(param.start).Take(param.length).ToList();
                else if (orderDir == orderDESC)
                    listUser = q.OrderByDescending(i => i.FIRST_NAME).Skip(param.start).Take(param.length).ToList();
            }
            if (orderColumn == 4)
            {
                if (orderDir == orderASC)
                    listUser = q.OrderBy(i => i.LAST_NAME).Skip(param.start).Take(param.length).ToList();
                else if (orderDir == orderDESC)
                    listUser = q.OrderByDescending(i => i.LAST_NAME).Skip(param.start).Take(param.length).ToList();
            }
            if (orderColumn == 5)
            {
                if (orderDir == orderASC)
                    listUser = q.OrderBy(i => i.EMAIL).Skip(param.start).Take(param.length).ToList();
                else if (orderDir == orderDESC)
                    listUser = q.OrderByDescending(i => i.EMAIL).Skip(param.start).Take(param.length).ToList();
            }
            if (param.data.type2 == "Internal")
            {
                if (orderColumn == 6)
                {
                    //position
                    if (orderDir == orderASC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_POSITION on t1.POSITION_ID equals t2.ART_M_POSITION_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.ART_M_POSITION_NAME
                                    select t1).Skip(param.start).Take(param.length).ToList();

                    else if (orderDir == orderDESC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_POSITION on t1.POSITION_ID equals t2.ART_M_POSITION_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.ART_M_POSITION_NAME descending
                                    select t1).Skip(param.start).Take(param.length).ToList();
                }

                if (orderColumn == 7)
                {
                    //role
                    if (orderDir == orderASC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_ROLE on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.ROLE_ID
                                    select t1).Skip(param.start).Take(param.length).ToList();

                    else if (orderDir == orderDESC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_ROLE on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.ROLE_ID descending
                                    select t1).Skip(param.start).Take(param.length).ToList();
                }

                if (orderColumn == 8)
                {
                    //leader
                    if (orderDir == orderASC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_UPPER_LEVEL on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.USER_ID
                                    select t1).Skip(param.start).Take(param.length).ToList();

                    else if (orderDir == orderDESC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_UPPER_LEVEL on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.USER_ID descending
                                    select t1).Skip(param.start).Take(param.length).ToList();
                }

                if (orderColumn == 9)
                {
                    //type of product
                    if (orderDir == orderASC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_TYPE_OF_PRODUCT on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.TYPE_OF_PRODUCT_ID
                                    select t1).Skip(param.start).Take(param.length).ToList();

                    else if (orderDir == orderDESC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_TYPE_OF_PRODUCT on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.TYPE_OF_PRODUCT_ID descending
                                    select t1).Skip(param.start).Take(param.length).ToList();
                }

                if (orderColumn == 10)
                {
                    //company
                    if (orderDir == orderASC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_COMPANY on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.COMPANY_ID
                                    select t1).Skip(param.start).Take(param.length).ToList();

                    else if (orderDir == orderDESC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_COMPANY on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.COMPANY_ID descending
                                    select t1).Skip(param.start).Take(param.length).ToList();
                }

                if (orderColumn == 11)
                {
                    if (orderDir == orderASC)
                        listUser = q.OrderBy(i => i.IS_ACTIVE).Skip(param.start).Take(param.length).ToList();
                    else if (orderDir == orderDESC)
                        listUser = q.OrderByDescending(i => i.IS_ACTIVE).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (param.data.type2 == "Vendor")
            {
                if (orderColumn == 6)
                {
                    if (orderDir == orderASC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_VENDOR on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    join t3 in context.XECM_M_VENDOR on t2.VENDOR_ID equals t3.VENDOR_ID into ps2
                                    from t3 in ps2.DefaultIfEmpty()
                                    orderby t3.VENDOR_CODE
                                    select t1).Skip(param.start).Take(param.length).ToList();

                    else if (orderDir == orderDESC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_VENDOR on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    join t3 in context.XECM_M_VENDOR on t2.VENDOR_ID equals t3.VENDOR_ID into ps2
                                    from t3 in ps2.DefaultIfEmpty()
                                    orderby t3.VENDOR_CODE descending
                                    select t1).Skip(param.start).Take(param.length).ToList();
                }
                if (orderColumn == 7)
                {
                    if (orderDir == orderASC)
                        listUser = q.OrderBy(i => i.IS_ACTIVE).Skip(param.start).Take(param.length).ToList();
                    else if (orderDir == orderDESC)
                        listUser = q.OrderByDescending(i => i.IS_ACTIVE).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (param.data.type2 == "Customer")
            {
                if (orderColumn == 6)
                {
                    if (orderDir == orderASC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_CUSTOMER on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    join t3 in context.XECM_M_CUSTOMER on t2.CUSTOMER_ID equals t3.CUSTOMER_ID into ps2
                                    from t3 in ps2.DefaultIfEmpty()
                                    orderby t3.CUSTOMER_CODE
                                    select t1).Skip(param.start).Take(param.length).ToList();

                    else if (orderDir == orderDESC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_CUSTOMER on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    join t3 in context.XECM_M_CUSTOMER on t2.CUSTOMER_ID equals t3.CUSTOMER_ID into ps2
                                    from t3 in ps2.DefaultIfEmpty()
                                    orderby t3.CUSTOMER_CODE descending
                                    select t1).Skip(param.start).Take(param.length).ToList();
                }
                if (orderColumn == 7)
                {
                    if (orderDir == orderASC)
                        listUser = q.OrderBy(i => i.IS_ACTIVE).Skip(param.start).Take(param.length).ToList();
                    else if (orderDir == orderDESC)
                        listUser = q.OrderByDescending(i => i.IS_ACTIVE).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (param.data.type2 == "Admin")
            {
                if (orderColumn == 6)
                {
                    //position
                    if (orderDir == orderASC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_POSITION on t1.POSITION_ID equals t2.ART_M_POSITION_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.ART_M_POSITION_NAME
                                    select t1).Skip(param.start).Take(param.length).ToList();

                    else if (orderDir == orderDESC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_POSITION on t1.POSITION_ID equals t2.ART_M_POSITION_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.ART_M_POSITION_NAME descending
                                    select t1).Skip(param.start).Take(param.length).ToList();
                }

                if (orderColumn == 7)
                {
                    //role
                    if (orderDir == orderASC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_ROLE on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.ROLE_ID
                                    select t1).Skip(param.start).Take(param.length).ToList();

                    else if (orderDir == orderDESC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_ROLE on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.ROLE_ID descending
                                    select t1).Skip(param.start).Take(param.length).ToList();
                }

                if (orderColumn == 8)
                {
                    //leader
                    if (orderDir == orderASC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_UPPER_LEVEL on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.USER_ID
                                    select t1).Skip(param.start).Take(param.length).ToList();

                    else if (orderDir == orderDESC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_UPPER_LEVEL on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.USER_ID descending
                                    select t1).Skip(param.start).Take(param.length).ToList();
                }

                if (orderColumn == 9)
                {
                    //type of product
                    if (orderDir == orderASC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_TYPE_OF_PRODUCT on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.TYPE_OF_PRODUCT_ID
                                    select t1).Skip(param.start).Take(param.length).ToList();

                    else if (orderDir == orderDESC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_TYPE_OF_PRODUCT on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.TYPE_OF_PRODUCT_ID descending
                                    select t1).Skip(param.start).Take(param.length).ToList();
                }

                if (orderColumn == 10)
                {
                    //company
                    if (orderDir == orderASC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_COMPANY on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.COMPANY_ID
                                    select t1).Skip(param.start).Take(param.length).ToList();

                    else if (orderDir == orderDESC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_COMPANY on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    orderby t2.COMPANY_ID descending
                                    select t1).Skip(param.start).Take(param.length).ToList();
                }

                if (orderColumn == 11)
                {
                    if (orderDir == orderASC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_CUSTOMER on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    join t3 in context.XECM_M_CUSTOMER on t2.CUSTOMER_ID equals t3.CUSTOMER_ID into ps2
                                    from t3 in ps2.DefaultIfEmpty()
                                    orderby t3.CUSTOMER_CODE
                                    select t1).Skip(param.start).Take(param.length).ToList();

                    else if (orderDir == orderDESC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_CUSTOMER on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    join t3 in context.XECM_M_CUSTOMER on t2.CUSTOMER_ID equals t3.CUSTOMER_ID into ps2
                                    from t3 in ps2.DefaultIfEmpty()
                                    orderby t3.CUSTOMER_CODE descending
                                    select t1).Skip(param.start).Take(param.length).ToList();
                }

                if (orderColumn == 12)
                {
                    if (orderDir == orderASC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_VENDOR on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    join t3 in context.XECM_M_VENDOR on t2.VENDOR_ID equals t3.VENDOR_ID into ps2
                                    from t3 in ps2.DefaultIfEmpty()
                                    orderby t3.VENDOR_CODE
                                    select t1).Skip(param.start).Take(param.length).ToList();

                    else if (orderDir == orderDESC)
                        listUser = (from t1 in q
                                    join t2 in context.ART_M_USER_VENDOR on t1.USER_ID equals t2.USER_ID into ps
                                    from t2 in ps.DefaultIfEmpty()
                                    join t3 in context.XECM_M_VENDOR on t2.VENDOR_ID equals t3.VENDOR_ID into ps2
                                    from t3 in ps2.DefaultIfEmpty()
                                    orderby t3.VENDOR_CODE descending
                                    select t1).Skip(param.start).Take(param.length).ToList();
                }

                if (orderColumn == 13)
                {
                    if (orderDir == orderASC)
                        listUser = q.OrderBy(i => i.IS_ACTIVE).Skip(param.start).Take(param.length).ToList();
                    else if (orderDir == orderDESC)
                        listUser = q.OrderByDescending(i => i.IS_ACTIVE).Skip(param.start).Take(param.length).ToList();
                }
            }
            return listUser;
        }
    }
}
