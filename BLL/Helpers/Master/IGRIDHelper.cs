using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class IGRIDHelper
    {

        public static ulogin_RESULT GetUser(ulogin_REQUEST param)
        {
            ulogin_RESULT Results = new ulogin_RESULT();

            try
            {
                var user_name = CNService.curruser();
                using (var igrid = new IGRIDEntities())
                {
                    var listUsers = igrid.Database.SqlQuery<ulogin_MODEL>("select * from ulogin where user_name = '" + user_name + "'").ToList();

                    if (listUsers != null && listUsers.Count > 0)
                    {
                        if (param != null && param.data != null && param.data.Matdoc > 0 )
                        {
                            foreach (var user in listUsers)
                            {
                                user.ListTransApproves = igrid.Database.SqlQuery<TransApprove_Model>("sp_IGRID_REIM_GetSAPMAterialStep @matdoc_id", new SqlParameter("@matdoc_id", param.data.Matdoc)).ToList();

                                if (user.ListTransApproves != null &&  user.ListTransApproves.Count > 0)
                                {
                                    foreach (var t in user.ListTransApproves)
                                    {
                                        t.CurrentUser = user_name;
                                    } 
                                }

                            }
                           
                        }
                    }


                    Results.data = listUsers;

                   
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

        //public static TransApprove_RESULT GetTransApproveIGrid(TransApprove_REQUEST param)
        //{
        //    TransApprove_RESULT Results = new TransApprove_RESULT();
        //    try
        //    {
        //        Results.data = new List<TransApprove_Model>();
        //        if (param != null && param.data != null && param.data.MatDoc > 0)
        //        {
        //            using (var context = new IGRIDEntities())
        //            {

        //                Results.data = context.Database.SqlQuery<TransApprove_Model>("sp_IGRID_REIM_GetSAPMAterialStep @matdoc_id", new SqlParameter("@matdoc_id", param.data.MatDoc)).ToList();
        //            }
        //        }

        //        Results.status = "S";
        //    }

        //    catch (Exception ex)
        //    {
        //        Results.status = "E";
        //        Results.msg = CNService.GetErrorMessage(ex);
        //    }
        //    return Results;
        //}



    }
}
