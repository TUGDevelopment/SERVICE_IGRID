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
    public class IGRIDMatStatusReportHelper
    {


        public static IGRID_MATSTATUS_REPORT_MODEL_RESULT PostMatStatusReactive(IGRID_MATSTATUS_REPORT_MODEL_REQUEST param)
        {
            IGRID_MATSTATUS_REPORT_MODEL_RESULT Results = new IGRID_MATSTATUS_REPORT_MODEL_RESULT();
         
           try
            {


               if (param != null && param.data != null)
                {
                    SqlParameter[] sp_param = new SqlParameter[] { new SqlParameter("@Material", string.Format("{0}", param.data.MATERIAL)) };
                    CNService.GetExecuteNonQuery("sp_IGRID_EXECUTE_MATSTATUS_REACTIVE", sp_param);
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


        public static IGRID_MATSTATUS_REPORT_MODEL_RESULT GetMatStatusReport(IGRID_MATSTATUS_REPORT_MODEL_REQUEST param)
        {
            IGRID_MATSTATUS_REPORT_MODEL_RESULT Results = new IGRID_MATSTATUS_REPORT_MODEL_RESULT();

            if (param != null && param.data != null && param.data.FIRST_LOAD == "X")
            {

                Results.status = "S";
                Results.data = new List<IGRID_MATSTATUS_REPORT_MODEL>();
                Results.draw = param.draw;
                return Results;

            }

            try
            {

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        //DateTime mydate;
                       string  by_status, keyword;


                        by_status = param.data.SEARCH_BY_STATUS;


                        if (string.IsNullOrEmpty(param.data.SEARCH_KEYWORD) || param.data.SEARCH_KEYWORD == "null")
                        {
                            keyword = "";
                        }
                        else
                        {
                            keyword = param.data.SEARCH_KEYWORD;
                        }


                        var q = context.Database.SqlQuery<IGRID_MATSTATUS_REPORT_MODEL>
                      ("sp_IGRID_REPORT_MATSTATUS @name, @Condition"
                      , new SqlParameter("@name", by_status)
                      , new SqlParameter("@Condition", keyword)
          
                      ).ToList();


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


    }
}
