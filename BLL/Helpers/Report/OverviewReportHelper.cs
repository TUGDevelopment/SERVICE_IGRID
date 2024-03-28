using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;
using BLL.Services;
using DAL;
using System.Globalization;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Configuration;
using BLL.DocumentManagement;
using System.Data.SqlClient;
using System.Data;
using System.Web;

namespace BLL.Helpers
{
    public class OverviewReportHelper
    {

        public static ulogin_RESULT GetOverviewRole(ulogin_RESULT param)
        {
            ulogin_RESULT Results = new ulogin_RESULT();
     
            try
            {
                //var user_name = HttpContext.Current.User.Identity.Name;
                var user_name = CNService.curruser();
                using (var igrid = new IGRIDEntities())
                {
                    var u = igrid.Database.SqlQuery<ulogin_MODEL>("select * from ulogin where user_name = '"+ user_name + "'").ToList();
                    Results.data = u;
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

        public static OVERVIEW_REPORT_RESULT GetOverviewReport(OVERVIEW_REPORT_REQUEST param)
        {
            OVERVIEW_REPORT_RESULT Results = new OVERVIEW_REPORT_RESULT();

            if (param != null && param.data != null && param.data.first_load == "1")
            {

                Results.status = "S";
                Results.data = new List<OVERVIEW_REPORT>();
                Results.draw = param.draw;
                return Results;

            }
            try
            {
                Results.status = "S";
                Results.data = CNService.Getsapmaterial2(param);
                Results.draw = param.draw;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static OVERVIEW_REPORT_RESULT PostOverviewReport(OVERVIEW_REPORT_REQUEST param)
        {
            OVERVIEW_REPORT_RESULT Results = new OVERVIEW_REPORT_RESULT();

            try
            {
                //Results.status = "S";
                 Results.data = CNService.InactiveMat(param);
                //Results.draw = param.draw;
                using (var context = new ARTWORKEntities())
                {
                    Results.status = "S";
                    Results.msg = MessageHelper.GetMessage("MSG_001", context);
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static OVERVIEW_REPORT_RESULT PostCreateDocByPA(OVERVIEW_REPORT_REQUEST param)
        {
            OVERVIEW_REPORT_RESULT Results = new OVERVIEW_REPORT_RESULT();

            try
            {
                //Results.status = "S";
                Results.data = new List<OVERVIEW_REPORT>();//CNService.InactiveMat(param);
                                                           //Results.draw = param.draw;
                using (var igird = new IGRIDEntities())
                {
                    var q = igird.Database.SqlQuery<OVERVIEW_REPORT>("spCreateDocument @CreateBy, @Condition, @Code"
                 
                    , new SqlParameter("@CreateBy", CNService.curruser())
                    , new SqlParameter("@Condition", param.data.Condition)
                    , new SqlParameter("@Code", param.data.Material)

                    ).ToList();

                    Results.data = q;
                }

                using (var context = new ARTWORKEntities())
                {
                    Results.status = "S";
                    Results.msg = MessageHelper.GetMessage("MSG_001", context);
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