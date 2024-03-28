using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL;
using BLL.Services;
using DAL;

namespace BLL.Helpers
{
    public class UserRoleHelper
    {
        public static ART_M_USER_ROLE_RESULT GetUserRole(ART_M_USER_ROLE_REQUEST param)
        {
            ART_M_USER_ROLE_RESULT Results = new ART_M_USER_ROLE_RESULT();
            List<ART_M_USER_ROLE_2> listUserRoleResult = new List<ART_M_USER_ROLE_2>();

            try
            {

                if (param != null || param.data != null)
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            List<ART_M_USER_ROLE_2> listUserRole2 = new List<ART_M_USER_ROLE_2>();
                            listUserRole2 = MapperServices.ART_M_USER_ROLE(ART_M_USER_ROLE_SERVICE.GetByItem(param.data, context));

                            ART_M_USER_ROLE_2 userRole2 = new ART_M_USER_ROLE_2();
                            var listRoleId = listUserRole2.Select(m => m.ROLE_ID).ToList();
                            var listRole = (from m in context.ART_M_ROLE where listRoleId.Contains(m.ROLE_ID) select m).ToList();
                            if (listUserRole2.Count > 0)
                            {
                                foreach (ART_M_USER_ROLE_2 item in listUserRole2)
                                {
                                    userRole2 = new ART_M_USER_ROLE_2();
                                    userRole2 = item;
                                    userRole2.ROLE_CODE = listRole.Where(m => m.ROLE_ID == userRole2.ROLE_ID).FirstOrDefault().ROLE_CODE;

                                    //start #TSK-1511 #SR-70695 by aof in 09/2022 
                                    userRole2.POSITION_CODE = (from m in context.ART_M_POSITION_ROLE
                                                               join n in context.ART_M_POSITION on m.POSITION_ID equals n.ART_M_POSITION_ID
                                                               where m.ROLE_ID == item.ROLE_ID
                                                               select n.ART_M_POSITION_CODE).ToList().FirstOrDefault();
                                    //end #TSK-1511 #SR-70695 by aof in 09/2022 

                                    userRole2.IGRID_USER_FN = CNService.getIGridUserFN();  //  IGRID REIM by aof in 08/2023

                                    listUserRoleResult.Add(userRole2);
                                }
                            }
                        }
                    }
                }

                Results.data = listUserRoleResult;
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
