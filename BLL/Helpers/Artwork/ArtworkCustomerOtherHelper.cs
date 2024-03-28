using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace BLL.Helpers
{
    public class ArtworkCustomerOtherHelper
    {
        public static ART_M_USER_RESULT GetArtworkMailCustomerOtherUser(ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();
            List<ART_M_USER_2> listUser_2 = new List<ART_M_USER_2>();
            ART_M_USER_2 user_2 = new ART_M_USER_2();

            try
            {
                //if (param == null && param.data == null)
                //{
                //    return Results;
                //}
                //else
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

                    using (var context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            var customerUser = (from c in context.ART_M_USER_CUSTOMER
                                                where listCustID.Contains(c.CUSTOMER_ID)
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
                        }
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

                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
                return Results;
            }
            return Results;
        }
    }
}
