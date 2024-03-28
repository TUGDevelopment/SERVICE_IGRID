using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace BLL.Helpers
{
    public class ReviewerHelper
    {
        public static ART_M_USER_RESULT GetReviewerHelper(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();

            try
            {
                ART_M_ROLE _role = new ART_M_ROLE();
                List<ART_M_ROLE> _roleList = new List<ART_M_ROLE>();

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var mk_id = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "MK" }, context).FirstOrDefault().ART_M_POSITION_ID;
                        var users = (from u in context.ART_M_USER
                                     where u.POSITION_ID == mk_id
                                     select u).ToList();

                        if (param != null && param.data != null)
                        {
                            if (param.data.TYPE_OF_PRODUCT_ID > 0)
                            {
                                users = (from u in users
                                         join m in context.ART_M_USER_TYPE_OF_PRODUCT on u.USER_ID equals m.USER_ID
                                         where m.TYPE_OF_PRODUCT_ID == param.data.TYPE_OF_PRODUCT_ID
                                         select u).ToList();
                            }
                        }

                        if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                        {
                            users = (from u1 in users
                                     where (u1.TITLE.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                           || u1.FIRST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                           || u1.LAST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                     select u1).ToList();


                        }

                        if (users.Count > 0) users = users.Where(m => m.IS_ACTIVE == "X").ToList();
                        Results.data = MapperServices.ART_M_USER(users);
                    }
                }

                if (Results.data.Count > 0)
                {
                    List<ART_M_USER_2> newList = new List<ART_M_USER_2>();
                    ART_M_USER_2 item = new ART_M_USER_2();

                    for (int i = 0; i < Results.data.Count; i++)
                    {
                        item = new ART_M_USER_2();
                        item.ID = Results.data[i].USER_ID;
                        item.DISPLAY_TXT = Results.data[i].TITLE + " " + Results.data[i].FIRST_NAME + " " + Results.data[i].LAST_NAME;
                        item.DISPLAY_TXT = item.DISPLAY_TXT.Trim();
                        newList.Add(item);
                    }

                    Results.data = newList;
                    Results.status = "S";
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static ART_M_USER_RESULT GetReviewerHelperFFC(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();

            try
            {
                ART_M_ROLE _role = new ART_M_ROLE();
                List<ART_M_ROLE> _roleList = new List<ART_M_ROLE>();

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var roleMK = ART_M_ROLE_SERVICE.GetByItem(new ART_M_ROLE() { ROLE_CODE = "MK_CD" }, context).FirstOrDefault().ROLE_ID;

                        var userRole = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { ROLE_ID = roleMK }, context).Select(m => m.USER_ID);

                        var FFC = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "FFC" }, context).FirstOrDefault().ART_M_POSITION_ID;

                        var users = ART_M_USER_SERVICE.GetAll(context).Where(m => userRole.Contains(m.USER_ID)).ToList();

                        if (param != null && param.data != null)
                        {
                            if (param.data.TYPE_OF_PRODUCT_ID > 0)
                            {
                                users = (from u in users
                                         join m in context.ART_M_USER_TYPE_OF_PRODUCT on u.USER_ID equals m.USER_ID
                                         where m.TYPE_OF_PRODUCT_ID == param.data.TYPE_OF_PRODUCT_ID
                                         select u).ToList();
                            }
                        }

                        if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                        {
                            users = (from u1 in users
                                     where (u1.TITLE.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                           || u1.FIRST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                           || u1.LAST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                     select u1).ToList();
                        }

                        if (users.Count > 0) users = users.Where(m => m.IS_ACTIVE == "X").ToList();
                        Results.data = MapperServices.ART_M_USER(users);
                    }

                    if (Results.data.Count > 0)
                    {
                        List<ART_M_USER_2> newList = new List<ART_M_USER_2>();
                        ART_M_USER_2 item = new ART_M_USER_2();

                        for (int i = 0; i < Results.data.Count; i++)
                        {
                            item = new ART_M_USER_2();
                            item.ID = Results.data[i].USER_ID;
                            item.DISPLAY_TXT = Results.data[i].TITLE + " " + Results.data[i].FIRST_NAME + " " + Results.data[i].LAST_NAME;
                            item.DISPLAY_TXT = item.DISPLAY_TXT.Trim();
                            newList.Add(item);
                        }

                        Results.data = newList;
                        Results.status = "S";
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

        public static ART_M_USER_RESULT GetReviewerHelperTHolding(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();

            try
            {
                ART_M_ROLE _role = new ART_M_ROLE();
                List<ART_M_ROLE> _roleList = new List<ART_M_ROLE>();

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var roleMK = ART_M_ROLE_SERVICE.GetByItem(new ART_M_ROLE() { ROLE_CODE = "MK_CD" }, context).FirstOrDefault().ROLE_ID;

                        var userRole = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { ROLE_ID = roleMK }, context).Select(m => m.USER_ID);

                        var THOLDING = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "T-HOLDING" }, context).FirstOrDefault().ART_M_POSITION_ID;

                        var users = ART_M_USER_SERVICE.GetAll(context).Where(m => userRole.Contains(m.USER_ID) && m.POSITION_ID == THOLDING).ToList();

                        if (param != null && param.data != null)
                        {
                            if (param.data.TYPE_OF_PRODUCT_ID > 0)
                            {
                                users = (from u in users
                                         join m in context.ART_M_USER_TYPE_OF_PRODUCT on u.USER_ID equals m.USER_ID
                                         where m.TYPE_OF_PRODUCT_ID == param.data.TYPE_OF_PRODUCT_ID
                                         select u).ToList();
                            }
                        }

                        if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                        {
                            users = (from u1 in users
                                     where (u1.TITLE.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                           || u1.FIRST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                           || u1.LAST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                     select u1).ToList();
                        }

                        if (users.Count > 0) users = users.Where(m => m.IS_ACTIVE == "X").ToList();
                        Results.data = MapperServices.ART_M_USER(users);
                    }
                }
                if (Results.data.Count > 0)
                {
                    List<ART_M_USER_2> newList = new List<ART_M_USER_2>();
                    ART_M_USER_2 item = new ART_M_USER_2();

                    for (int i = 0; i < Results.data.Count; i++)
                    {
                        item = new ART_M_USER_2();
                        item.ID = Results.data[i].USER_ID;
                        item.DISPLAY_TXT = Results.data[i].TITLE + " " + Results.data[i].FIRST_NAME + " " + Results.data[i].LAST_NAME;
                        item.DISPLAY_TXT = item.DISPLAY_TXT.Trim();
                        newList.Add(item);
                    }

                    Results.data = newList;
                    Results.status = "S";
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
