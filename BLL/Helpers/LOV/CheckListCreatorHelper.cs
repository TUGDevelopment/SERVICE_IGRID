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
    public class CheckListCreatorHelper
    {
        public static ART_M_USER_RESULT GetCheckListCreator(ART_M_USER_REQUEST param)
        {
            ART_M_USER_RESULT Results = new ART_M_USER_RESULT();
            List<ART_M_USER_2> listUserCreator = new List<ART_M_USER_2>();
            ART_M_USER_2 user_2 = new ART_M_USER_2();
            ART_M_USER user = new ART_M_USER();
            List<int> listUserCheckList = new List<int>();
            List<int> listUserArtwork = new List<int>();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        listUserCheckList = (from c in context.ART_WF_MOCKUP_CHECK_LIST select c.CREATE_BY).Distinct().ToList();

                        var cusUsers = context.ART_M_USER_CUSTOMER.Select(c => c.USER_ID).Distinct().ToList();

                        listUserArtwork = (from c in context.ART_WF_ARTWORK_REQUEST
                                           where !cusUsers.Contains(c.UPDATE_BY)
                                           select c.UPDATE_BY).Distinct().ToList();
                        if (listUserArtwork.Count > 0)
                        {
                            listUserCheckList = listUserCheckList.Union(listUserArtwork).ToList();
                        }

                        if (listUserCheckList.Count > 0)
                        {
                            foreach (int iUserID in listUserCheckList)
                            {
                                user_2 = new ART_M_USER_2();

                                user_2.ID = iUserID;
                                user_2.DISPLAY_TXT = CNService.GetUserName(iUserID, context);
                                listUserCreator.Add(user_2);
                            }
                        }
                    }
                }

                if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                {
                    listUserCreator = (from u1 in listUserCreator
                                       where ((u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())))
                                       select u1).ToList();
                }

                Results.data = listUserCreator;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            
            Results.status = "S";
            return Results;
        }
    }
}
