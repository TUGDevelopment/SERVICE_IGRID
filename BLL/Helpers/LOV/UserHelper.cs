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
    public class UserHelper
    {
        public static ART_M_USER_RESULT GetUser(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            ART_M_USER_2 user_2 = new ART_M_USER_2();
                            user_2.IS_ACTIVE = "X";
                            Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(MapperServices.ART_M_USER(user_2), context));
                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(MapperServices.ART_M_USER(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            var allPosition = ART_M_POSITION_SERVICE.GetAll(context);
                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                Results.data = (from u1 in Results.data
                                                join m in allPosition on u1.POSITION_ID equals m.ART_M_POSITION_ID
                                                where (u1.USERNAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.TITLE != null && u1.TITLE.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.FIRST_NAME != null && u1.FIRST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.LAST_NAME != null && u1.LAST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || (u1.FIRST_NAME + ' ' + u1.LAST_NAME).ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || m.ART_M_POSITION_NAME != null && m.ART_M_POSITION_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                )
                                                select u1).ToList();
                            }

                            Results.data = Results.data.OrderBy(m => m.FIRST_NAME).ThenBy(m => m.LAST_NAME).ToList();

                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                string positionName = allPosition.Where(m => m.ART_M_POSITION_ID == Results.data[i].POSITION_ID).FirstOrDefault() != null ? allPosition.Where(m => m.ART_M_POSITION_ID == Results.data[i].POSITION_ID).FirstOrDefault().ART_M_POSITION_NAME : string.Empty;

                                Results.data[i].ID = Results.data[i].USER_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].TITLE + " " + Results.data[i].FIRST_NAME + " " + Results.data[i].LAST_NAME + " (" + positionName + "-" + Results.data[i].USERNAME + ")";  //by aof added USERNAME

                            }
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

        public static ART_M_USER_RESULT GetUserWithParam(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        ART_M_USER_2 user_2 = new ART_M_USER_2();

                        if (param == null || param.data == null || param.data.ROLE_CODE == null)
                        {
                            user_2.IS_ACTIVE = "X";
                            Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(MapperServices.ART_M_USER(user_2), context));
                        }
                        else
                        {
                            var positionID = context.ART_M_POSITION.Where(w => w.ART_M_POSITION_CODE == "PK").Select(s => s.ART_M_POSITION_ID).FirstOrDefault();

                            var role_param = (from p in context.ART_M_ROLE
                                              where p.ROLE_CODE.StartsWith(param.data.ROLE_CODE)
                                              select p.ROLE_ID).ToList();

                            user_2.IS_ACTIVE = "X";
                            Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(MapperServices.ART_M_USER(user_2), context));

                            //Results.data = (from m in Results.data
                            //                join m2 in context.ART_M_POSITION on m.POSITION_ID equals m2.ART_M_POSITION_ID
                            //                join m3 in context.ART_M_POSITION_ROLE on m.POSITION_ID equals m3.POSITION_ID
                            //                join m4 in context.ART_M_ROLE on m3.ROLE_ID equals m4.ROLE_ID
                            //                where role_param.Contains(m4.ROLE_ID) && m4.ROLE_CODE != "ADMINISTRATOR"
                            //                select m).Distinct().ToList();


                            var userRolePA = (from p in context.ART_M_USER_ROLE
                                              where role_param.Contains(p.ROLE_ID)
                                              select p.USER_ID).ToList();


                            Results.data = (from p in Results.data
                                            where userRolePA.Contains(p.USER_ID)
                                            && p.IS_ACTIVE == "X"
                                            && p.POSITION_ID == positionID
                                            select p).ToList();

                        }

                        if (Results.data.Count > 0)
                        {
                            var allPosition = ART_M_POSITION_SERVICE.GetAll(context);
                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                Results.data = (from u1 in Results.data
                                                join m in allPosition on u1.POSITION_ID equals m.ART_M_POSITION_ID
                                                where (u1.USERNAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.TITLE != null && u1.TITLE.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.FIRST_NAME != null && u1.FIRST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.LAST_NAME != null && u1.LAST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || (u1.FIRST_NAME + ' ' + u1.LAST_NAME).ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || m.ART_M_POSITION_NAME != null && m.ART_M_POSITION_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                )
                                                select u1).ToList();
                            }

                            Results.data = Results.data.OrderBy(m => m.FIRST_NAME).ThenBy(m => m.LAST_NAME).ToList();

                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                string positionName = allPosition.Where(m => m.ART_M_POSITION_ID == Results.data[i].POSITION_ID).FirstOrDefault() != null ? allPosition.Where(m => m.ART_M_POSITION_ID == Results.data[i].POSITION_ID).FirstOrDefault().ART_M_POSITION_NAME : string.Empty;

                                Results.data[i].ID = Results.data[i].USER_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].TITLE + " " + Results.data[i].FIRST_NAME + " " + Results.data[i].LAST_NAME + " (" + positionName + ")";
                            }
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

        public static ART_M_USER_RESULT GetUserPIC(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();
            List<ART_M_USER_2> listUser2 = new List<ART_M_USER_2>();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {

                        var positionID = context.ART_M_POSITION.Where(w => w.ART_M_POSITION_CODE == "PK").Select(s => s.ART_M_POSITION_ID).FirstOrDefault();
                        var rolePA = (from p in context.ART_M_ROLE
                                      where p.ROLE_CODE.StartsWith("PA_")
                                      select p.ROLE_ID).ToList();

                        var userRolePA = (from p in context.ART_M_USER_ROLE
                                          where rolePA.Contains(p.ROLE_ID)
                                          select p.USER_ID).ToList();


                        var user1 = (from p in context.ART_M_USER
                                     where userRolePA.Contains(p.USER_ID)
                                     && p.IS_ACTIVE == "X"
                                     && p.POSITION_ID == positionID
                                     select p).ToList();

                        if (user1 != null)
                        {
                            listUser2 = MapperServices.ART_M_USER(user1);
                        }

                        Results.data = listUser2;

                        if (Results.data.Count > 0)
                        {
                            var allPosition = ART_M_POSITION_SERVICE.GetAll(context);
                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                Results.data = (from u1 in Results.data
                                                join m in allPosition on u1.POSITION_ID equals m.ART_M_POSITION_ID
                                                where (u1.USERNAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.TITLE != null && u1.TITLE.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.FIRST_NAME != null && u1.FIRST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.LAST_NAME != null && u1.LAST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || (u1.FIRST_NAME + ' ' + u1.LAST_NAME).ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || m.ART_M_POSITION_NAME != null && m.ART_M_POSITION_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                )
                                                select u1).ToList();
                            }

                            Results.data = Results.data.OrderBy(m => m.FIRST_NAME).ThenBy(m => m.LAST_NAME).ToList();

                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                string positionName = allPosition.Where(m => m.ART_M_POSITION_ID == Results.data[i].POSITION_ID).FirstOrDefault() != null ? allPosition.Where(m => m.ART_M_POSITION_ID == Results.data[i].POSITION_ID).FirstOrDefault().ART_M_POSITION_NAME : string.Empty;

                                Results.data[i].ID = Results.data[i].USER_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].TITLE + " " + Results.data[i].FIRST_NAME + " " + Results.data[i].LAST_NAME + " (" + positionName + ")";

                                Results.data[i].DISPLAY_TXT = Results.data[i].USERNAME + ":" + Results.data[i].DISPLAY_TXT.Trim();
                            }
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

        public static ART_M_USER_RESULT GetUserPICSO(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        Results.data = (from p in context.V_ART_ASSIGNED_SO select new ART_M_USER_2() { DISPLAY_TXT = p.TITLE + " " + p.FIRST_NAME + " " + p.LAST_NAME }).Distinct().ToList();
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

        public static ART_M_USER_RESULT GetUserReassign(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var allPosition = ART_M_POSITION_SERVICE.GetAll(context);
                        if (param == null || param.data == null)
                        {
                            ART_M_USER_2 user_2 = new ART_M_USER_2();
                            var position_id = ART_M_USER_SERVICE.GetByItem(new ART_M_USER { USER_ID = CNService.getCurrentUser(context) }, context).FirstOrDefault().POSITION_ID;
                            var position_code = allPosition.Where(m => m.ART_M_POSITION_ID == position_id).FirstOrDefault().ART_M_POSITION_CODE;

                            if (position_code == "ADMIN" || position_code == "PK" || position_code == "MK")
                            {
                                user_2.IS_ACTIVE = "X";
                                Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(MapperServices.ART_M_USER(user_2), context));
                            }
                            else
                            {
                                user_2.POSITION_ID = position_id;
                                user_2.IS_ACTIVE = "X";
                                Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(MapperServices.ART_M_USER(user_2), context));
                            }


                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(MapperServices.ART_M_USER(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                Results.data = (from u1 in Results.data
                                                where (u1.USERNAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.TITLE != null && u1.TITLE.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.FIRST_NAME != null && u1.FIRST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.LAST_NAME != null && u1.LAST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || (u1.FIRST_NAME + ' ' + u1.LAST_NAME).ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                )
                                                select u1).ToList();
                            }

                            Results.data = Results.data.OrderBy(m => m.FIRST_NAME).ThenBy(m => m.LAST_NAME).ToList();

                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].USER_ID;
                                if (allPosition.Where(m => m.ART_M_POSITION_ID == Results.data[i].POSITION_ID).FirstOrDefault() != null)
                                    //  Results.data[i].DISPLAY_TXT = Results.data[i].TITLE + " " + Results.data[i].FIRST_NAME + " " + Results.data[i].LAST_NAME + " (" + allPosition.Where(m => m.ART_M_POSITION_ID == Results.data[i].POSITION_ID).FirstOrDefault().ART_M_POSITION_NAME + ")";
                                    Results.data[i].DISPLAY_TXT =  Results.data[i].TITLE + " " + Results.data[i].FIRST_NAME + " " + Results.data[i].LAST_NAME + " (" + allPosition.Where(m => m.ART_M_POSITION_ID == Results.data[i].POSITION_ID).FirstOrDefault().ART_M_POSITION_NAME + " - " + Results.data[i].USERNAME   + ")";
                                else
                                    Results.data[i].DISPLAY_TXT = Results.data[i].TITLE + " " + Results.data[i].FIRST_NAME + " " + Results.data[i].LAST_NAME + " (" + Results.data[i].USERNAME + ")";

                            }

                            ART_M_USER_2 item = new ART_M_USER_2();
                            item.ID = 0;
                            item.DISPLAY_TXT = "---zPool---";
                            Results.data.Add(item);
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

        public static ART_M_USER_RESULT GetUserPackaging(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var PK = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "PK" }, context).FirstOrDefault().ART_M_POSITION_ID;
                        if (param == null || param.data == null)
                        {
                            ART_M_USER_2 user_2 = new ART_M_USER_2();
                            user_2.IS_ACTIVE = "X";
                            user_2.POSITION_ID = PK;
                            Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(MapperServices.ART_M_USER(user_2), context));
                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            param.data.POSITION_ID = PK;
                            Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(MapperServices.ART_M_USER(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            var listPic = (
                            from x in context.ART_M_PIC
                            select x.USER_ID).Distinct().ToList();
                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                
                                Results.data = (from u1 in Results.data
                                                where listPic.Contains(u1.USER_ID)
                                                &&
                                                (u1.USERNAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.TITLE != null && u1.TITLE.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.FIRST_NAME != null && u1.FIRST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.LAST_NAME != null && u1.LAST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || (u1.FIRST_NAME + ' ' + u1.LAST_NAME).ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                )
                                                select u1).ToList();
                            }

                            Results.data = Results.data.OrderBy(m => m.FIRST_NAME).ThenBy(m => m.LAST_NAME).ToList();

                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                    if (listPic.Contains(Results.data[i].USER_ID))
                                    {
                                        Results.data[i].ID = Results.data[i].USER_ID;
                                        Results.data[i].DISPLAY_TXT = Results.data[i].TITLE + " " + Results.data[i].FIRST_NAME + " " + Results.data[i].LAST_NAME;
                                    }
                            }
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
