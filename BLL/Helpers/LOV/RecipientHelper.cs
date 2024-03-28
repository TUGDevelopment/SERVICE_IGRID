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
    public class RecipientHelper
    {
        public static ART_M_USER_RESULT GetRecipientHelper(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();

            try
            {

                //Get User by role MK_CD
                ART_M_ROLE _role = new ART_M_ROLE();
                List<ART_M_ROLE> _roleList = new List<ART_M_ROLE>();

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        List<string> _typeOfProduct = new List<string>();

                        if (param.data.IS_HF == "true")
                        {
                            _typeOfProduct.Add("HF");
                        }

                        if (param.data.IS_PF == "true")
                        {
                            _typeOfProduct.Add("PF");
                        }

                        var typeOfProductID = context.SAP_M_TYPE_OF_PRODUCT.Where(t => _typeOfProduct.Contains(t.TYPE_OF_PRODUCT)).Select(s => s.TYPE_OF_PRODUCT_ID).ToList();

                        var usersByTypeOfProduct = (from u in context.ART_M_USER_TYPE_OF_PRODUCT
                                                    where typeOfProductID.Contains(u.TYPE_OF_PRODUCT_ID)
                                                    select u.USER_ID).ToList();

                        var rolesID = (from r in context.ART_M_ROLE
                                       where r.ROLE_CODE.StartsWith("MK_")
                                               || r.ROLE_CODE.StartsWith("MC_")
                                               || r.ROLE_CODE.StartsWith("MARKETING_")
                                               || r.ROLE_CODE.StartsWith("PM")
                                       select r.ROLE_ID).ToList();

                        List<int> userRole = new List<int>();

                        if (usersByTypeOfProduct != null && usersByTypeOfProduct.Count > 0)
                        {
                            userRole = (from p in context.ART_M_USER_ROLE
                                        where rolesID.Contains(p.ROLE_ID)
                                         && usersByTypeOfProduct.Contains(p.USER_ID)
                                        select p.USER_ID).ToList();
                        }
                        else
                        {
                            userRole = (from p in context.ART_M_USER_ROLE
                                        where rolesID.Contains(p.ROLE_ID)
                                        select p.USER_ID).ToList();
                        }

                        var users = (from u in context.ART_M_USER
                                     where userRole.Contains(u.USER_ID)
                                     select u).ToList();

                        if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                        {
                            users = (from u1 in users
                                     where ((u1.TITLE != null && u1.TITLE.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                           || u1.FIRST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                           || u1.LAST_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                           || u1.EMAIL.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                     select u1).ToList();

                        }

                        if (users.Count > 0) users = users.Where(m => m.IS_ACTIVE == "X").ToList();
                        Results.data = MapperServices.ART_M_USER(users);

                        if (Results.data.Count > 0)
                        {
                            List<ART_M_USER_2> newList = new List<ART_M_USER_2>();
                            ART_M_USER_2 item = new ART_M_USER_2();

                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                item = new ART_M_USER_2();
                                item.ID = Results.data[i].USER_ID;
                                item.USER_ID = Results.data[i].USER_ID;
                                item.EMAIL = Results.data[i].EMAIL;

                                if (Results.data[i].POSITION_ID != null)
                                {
                                    item.POSITION_ID = Results.data[i].POSITION_ID;
                                    item.POSITION_DISPLAY_TXT = ART_M_POSITION_SERVICE.GetByART_M_POSITION_ID(Results.data[i].POSITION_ID, context).ART_M_POSITION_NAME;
                                }

                                item.DISPLAY_TXT = CNService.GetUserName(Results.data[i].USER_ID, context).Trim();
                                item.DISPLAY_TXT = item.DISPLAY_TXT + " (" + item.EMAIL + ")";
                                newList.Add(item);
                            }

                            Results.data = newList;
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
