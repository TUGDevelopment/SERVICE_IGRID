using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services;
using DAL;
using DAL.Model;
using System.Web;


namespace BLL.Helpers
{
    public class SelectMasterHelper
    {


        public static ulogin_RESULT GetAuthrolizeEditMaster(ulogin_REQUEST param)
        {
            ulogin_RESULT Results = new ulogin_RESULT();
            Results.haveAuthrolizeEditMaster = "";

            try
            {

                var artwork_username = CNService.curruser(); //HttpContext.Current.User.Identity.Name;
                ART_M_USER userAW = new ART_M_USER();
                using (var context = new ARTWORKEntities())
                {
                    userAW = context.ART_M_USER.Where(w => w.USERNAME == artwork_username).ToList().FirstOrDefault();
                }

                if (userAW != null && userAW.USER_ID >= 0 && userAW.EMAIL != null)
                {

                    using (var context = new IGRIDEntities())
                    {
                        ulogin_MODEL userIG = new ulogin_MODEL();
                        
                        userIG = context.Database.SqlQuery<ulogin_MODEL>("SELECT * FROM ulogin WHERE email='" + userAW.EMAIL + "'").ToList().FirstOrDefault(); ;
                        if (userIG != null && userIG.Id >= 0)
                        {
                            if (userIG.Authorize_ChangeMaster == "Y")
                            {
                                Results.haveAuthrolizeEditMaster = "Y";
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


        public static SelectMaster_RESULT BuildSelectMaster(SelectMaster_REQUEST param)
        {
            SelectMaster_RESULT Results = new SelectMaster_RESULT();
            try
            {
                Results.data = CNService.GetSelectMaster(param);
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;



        }
        public static Condition_RESULT BuildCondition(Condition_REQUEST param)
        {
            Condition_RESULT Results = new Condition_RESULT();
            try
            {
                Results.data = CNService.GetCondition(param);
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static Condition_RESULT GetByIGrid(Condition_REQUEST param)
        {
            Condition_RESULT Results = new Condition_RESULT();
            try
            {
                using (var context = new IGRIDEntities())
                {
                    context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                    var q = context.Database.SqlQuery<Condition_MODEL>
                  ("spSearchresults @table, @where"
                  , new SqlParameter("@table", string.Format("{0}", "ulogin"))
                  , new SqlParameter("@where", string.Format("{0}", ""))
                  ).ToList();

                    foreach (var item in q)
                    {
                        item.ID = item.user_name;
                        item.DISPLAY_TXT = string.Format("{0} {1}", item.FirstName, item.LastName);
                    }
                    Results.data = q;

                    // added by aof

                    if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                    {
                        Results.data = (from u1 in Results.data
                                        where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                        select u1).ToList();
                    }
                    // added by aof


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
        public static Condition_RESULT GetBy(Condition_REQUEST param)
        {
            Condition_RESULT Results = new Condition_RESULT();
            try
            {
                Results.data = CNService.GetSearchresults(param);

                // added by aof

                if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                {
                    Results.data = (from u1 in Results.data
                                    where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                    select u1).ToList();
                }
                // added by aof

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
