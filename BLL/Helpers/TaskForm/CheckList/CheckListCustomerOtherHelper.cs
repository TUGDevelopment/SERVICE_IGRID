using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace BLL.Helpers
{
    public class CheckListCustomerOtherHelper
    {
        public static ART_M_USER_RESULT GetCheckListMailCustomerOtherUser(ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();
            List<ART_M_USER_2> listUser_2 = new List<ART_M_USER_2>();
            ART_M_USER_2 user_2 = new ART_M_USER_2();

            try
            {
                List<int> listCustID = new List<int>();

                if (param.data.SOLD_TO_ID != null)
                {
                    listCustID.Add(Convert.ToInt32(param.data.SOLD_TO_ID));
                }

                if (param.data.SHIP_TO_ID != null)
                {
                    listCustID.Add(Convert.ToInt32(param.data.SHIP_TO_ID));
                }

                if (param.data.CUSTOMER_OTHER_ID != null)
                {
                    listCustID.Add(Convert.ToInt32(param.data.CUSTOMER_OTHER_ID));
                }

                List<string> listFilterID = new List<string>();
                if (param.data.FILTER_ID != null)
                {
                    listFilterID = param.data.FILTER_ID.Split(new string[] { "||" }, StringSplitOptions.None).ToList();
                }

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param.data.CUSTOMER_OTHER_ID == 3253) //# CR-150109
                        {
                            var query = context.Database.SqlQuery<ART_M_USER_2>("spGetART_M_USER @IS_ACTIVE",
                                        new SqlParameter("@IS_ACTIVE", string.Format("{0}", "X"))
                                        )
                                        .ToList();
                            Results.data = query.Select(m => new ART_M_USER_2()
                            {
                                ID = m.USER_ID,

                                DISPLAY_TXT = string.Format("{0}", (m.TITLE + " " + m.FIRST_NAME + " " + m.LAST_NAME).Trim() + " (" + m.USERNAME + ")")
                            }).ToList();

                            if (param.data.USER_DISPLAY_TXT != null)
                            {
                                Results.data = (from u1 in Results.data
                                                where (u1.DISPLAY_TXT.ToLower().Contains(param.data.USER_DISPLAY_TXT.ToLower()))
                                                select u1).ToList();
                            }
                            Results.data = Results.data.OrderBy(x => x.DISPLAY_TXT).ToList();
                        }
                        else
                        {
                            var customerUser = (from c in context.ART_M_USER_CUSTOMER
                                                where listCustID.Contains(c.CUSTOMER_ID)
                                                && !listFilterID.Contains(c.USER_ID.ToString())
                                                select c.USER_ID).Distinct().ToList();

                            foreach (var iCust in customerUser)
                            {
                                user_2 = new ART_M_USER_2();
                                user_2 = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByUSER_ID(iCust, context));
                                user_2.ID = user_2.USER_ID;

                                if (user_2.IS_ACTIVE == "X")
                                {
                                    user_2.DISPLAY_TXT = CNService.GetUserName(user_2.USER_ID, context) + " (" + user_2.USERNAME + ")";
                                    listUser_2.Add(user_2);
                                }
                            }

                            if (param.data.USER_DISPLAY_TXT != null)
                            {
                                var filteredUserList = listUser_2.Where(user => (user.DISPLAY_TXT.ToLower().Contains(param.data.USER_DISPLAY_TXT.ToLower()))).ToList();
                                Results.data = filteredUserList;
                            }
                            else
                            {
                                Results.data = listUser_2;
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

        public static ART_M_USER_RESULT MailToCCFFC(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var PositionID = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "FFC" }, context).FirstOrDefault().ART_M_POSITION_ID;

                        Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { POSITION_ID = PositionID, IS_ACTIVE = "X" }, context));

                        foreach (var item in Results.data)
                        {
                            item.ID = item.USER_ID;
                            item.DISPLAY_TXT = CNService.GetUserName(item.USER_ID, context) + " (" + item.USERNAME + ")";
                        }

                        if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                        {
                            Results.data = (from u1 in Results.data
                                            where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                            select u1).ToList();
                        }

                        Results.data = Results.data.OrderBy(m => m.DISPLAY_TXT).ToList();
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

        public static ART_M_USER_RESULT MailToCCTHolding(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var PositionID = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "T-HOLDING" }, context).FirstOrDefault().ART_M_POSITION_ID;

                        Results.data = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { POSITION_ID = PositionID, IS_ACTIVE = "X" }, context));

                        foreach (var item in Results.data)
                        {
                            item.ID = item.USER_ID;
                            item.DISPLAY_TXT = CNService.GetUserName(item.USER_ID, context) + " (" + item.USERNAME + ")";
                        }

                        if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                        {
                            Results.data = (from u1 in Results.data
                                            where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                            select u1).ToList();
                        }

                        Results.data = Results.data.OrderBy(m => m.DISPLAY_TXT).ToList();
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
