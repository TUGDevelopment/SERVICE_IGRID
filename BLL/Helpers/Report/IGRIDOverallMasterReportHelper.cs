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
namespace BLL.Helpers
{
    public class IGRIDOverallMasterReportHelper
    {

     
   public static IGRID_OVERALL_MASTER_REPORT_MODEL_RESULT GetOverallMasterReport(IGRID_OVERALL_MASTER_REPORT_MODEL_REQUEST param)
        {
            IGRID_OVERALL_MASTER_REPORT_MODEL_RESULT Results = new IGRID_OVERALL_MASTER_REPORT_MODEL_RESULT();

            if (param != null && param.data != null && param.data.FIRST_LOAD == "X")
            {

                Results.status = "S";
                Results.data = new List<IGRID_OVERALL_MASTER_REPORT_MODEL>();
                Results.draw = param.draw;
                return Results;

            }

            try
            {

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {

                        string master, keyword,status;
                        master = param.data.SEARCH_MASTER;

                        if (string.IsNullOrEmpty(param.data.SEARCH_KEYWORD) || param.data.SEARCH_KEYWORD == "null")
                        {
                            keyword = "";
                        }
                        else
                        {
                            keyword = param.data.SEARCH_KEYWORD;
                        }

                        if (string.IsNullOrEmpty(param.data.SEARCH_STATUS) || param.data.SEARCH_STATUS == "null")
                        {
                            status = "All";
                        }
                        else
                        {
                            status = param.data.SEARCH_STATUS;
                        }


                        var q = context.Database.SqlQuery<IGRID_OVERALL_MASTER_REPORT_MODEL>
                      ("[sp_IGRID_REPORT_OVERALL_MASTER] @master, @keyword"
                      , new SqlParameter("@master", master)
                      , new SqlParameter("@keyword", keyword)
                      ).ToList();

                   
                        if (status == "Active")
                        {
                            q = q.Where(w => string.IsNullOrEmpty(w.INACTIVE)).ToList();
                        }
                        else if (status == "Inactive")
                        {
                            q = q.Where(w => !string.IsNullOrEmpty(w.INACTIVE)).ToList();
                        }
                        Results.data = q;
                    }
                }


                //Results.data = new List<IGRID_MATSTATUS_REPORT_MODEL>();
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }



        public static IGRID_CBB_DATA_MODEL_RESULT GetMasterData(IGRID_CBB_DATA_MODEL_REQUEST param)
        {
            IGRID_CBB_DATA_MODEL_RESULT Results = new IGRID_CBB_DATA_MODEL_RESULT();

            try
            {

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        Results.data = context.Database.SqlQuery<IGRID_CBB_DATA_MODEL>("sp_IGRID_GET_MASTER").ToList();

                    }
                }


                if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                {
                    Results.data = (from u1 in Results.data
                                    where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                    select u1).ToList();
                }

                Results.data = Results.data.OrderBy(x => x.DISPLAY_TXT).ToList();


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
