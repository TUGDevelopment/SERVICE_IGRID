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
    
    public class IGRIDMasterDataChangeLogReportHelper
    {
        public static IGRID_CBB_DATA_MODEL_RESULT GetMasterData(IGRID_CBB_DATA_MODEL_REQUEST param)
        {
            IGRID_CBB_DATA_MODEL_RESULT Results = new IGRID_CBB_DATA_MODEL_RESULT();

            try
            {

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        Results.data = context.Database.SqlQuery<IGRID_CBB_DATA_MODEL>("sp_IGRID_GET_MASTER_DATA_CHANGE_LOG2 @SEARCH_TYPE"
                      , new SqlParameter("@SEARCH_TYPE", param.data.SEARCH_TYPE) 
                      ).ToList();

                    }
                }


                if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                {
                    Results.data = (from u1 in Results.data
                                    where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                    select u1).ToList();
                }

                Results.data = Results.data.OrderBy(x => x.DISPLAY_TXT).ToList();
                IGRID_CBB_DATA_MODEL all = new IGRID_CBB_DATA_MODEL();
                if (param.data.SEARCH_TYPE == "TransMaster")
                {
                    all.ID = "All";
                    all.DISPLAY_TXT = "All";
                    Results.data.Insert(0, all);
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


        public static IGRID_CBB_DATA_MODEL_RESULT GetUserMasterData(IGRID_CBB_DATA_MODEL_REQUEST param)
        {
            IGRID_CBB_DATA_MODEL_RESULT Results = new IGRID_CBB_DATA_MODEL_RESULT();

            try
            {

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        Results.data = context.Database.SqlQuery<IGRID_CBB_DATA_MODEL>("sp_IGRID_GET_USER_MASTER_DATA_CHANGE_LOG").ToList();
                    }
                }

                if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                {
                    Results.data = (from u1 in Results.data
                                    where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                    select u1).ToList();
                }

                Results.data = Results.data.OrderBy(x => x.DISPLAY_TXT).ToList();

                IGRID_CBB_DATA_MODEL all = new IGRID_CBB_DATA_MODEL();
                all.ID = "All";
                all.DISPLAY_TXT = "All";
                Results.data.Insert(0, all);

                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }



        public static IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL_RESULT GetUserMasterChangeLogReport(IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL_REQUEST param)
        {
            IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL_RESULT Results = new IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL_RESULT();

            if (param != null && param.data != null &&param.data.FIRST_LOAD == "X")
            {

                Results.status = "S";
                Results.data = new List<IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL>();
                Results.draw = param.draw;
                return Results;

            }

            try
            {

                if (param.data.SEARCH_TYPE == "TransChanged")
                {
                    using (SqlConnection con = new SqlConnection(CNService.strConn))
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spGetReportTranschanged";
                        cmd.Parameters.AddWithValue("@Material", string.Format("{0}", param.data.SEARCH_KEYWORD));
                        cmd.Parameters.AddWithValue("@FrDt", CNService.ConvertStringToDate(param.data.SEARCH_DATE_FROM).ToString("yyyyMMdd"));
                        cmd.Parameters.AddWithValue("@ToDt", CNService.ConvertStringToDate(param.data.SEARCH_DATE_TO).ToString("yyyyMMdd"));
                        cmd.Parameters.AddWithValue("@User", string.Format("{0}", param.data.SEARCH_USER));
                         
                        cmd.Parameters.AddWithValue("@Shortname", string.Format("{0}", param.data.SEARCH_MASTER));
                        cmd.Connection = con;
                        con.Open();
                        DataTable dt = new DataTable();
                        SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                        oAdapter.Fill(dt);
                        con.Close();
                        switch (string.Format("{0}", param.data.SEARCH_MASTER))
                            {
                            case "Packing Style":
                                if(dt.Rows.Count>0)
                                Results.data = (from DataRow dr in dt.Rows
                                                select new IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL()
                                                {
                                                    ID = string.Format("{0}", dr["Id"]),
                                                    CHANGED_ID = Convert.ToInt32(dr["Changed_Id"]),
                                                    SHORTNAME = string.Format("{0}", dr["Shortname"]),
                                                    CHANGED_CHARNAME = string.Format("{0}", dr["changed_charname"]),
                                                    CHANGED_TABNAME = string.Format("{0}", dr["changed_tabname"]),
                                                    CHANGED_ACTION = string.Format("{0}", dr["Changed_Action"]),
                                                    
                                                    PrimaryCode = string.Format("{0}",dr["PrimaryCode"]),
                                                    GroupStyle = string.Format("{0}",dr["GroupStyle"]),
                                                    PackingStyle = string.Format("{0}",dr["PackingStyle"]),
                                                    RefStyle = string.Format("{0}",dr["RefStyle"]),
                                                    PackSize = string.Format("{0}",dr["PackSize"]),
                                                    BaseUnit = string.Format("{0}",dr["BaseUnit"]),
                                                    TypeofPrimary = string.Format("{0}",dr["TypeofPrimary"]),
                                                    Inactive = string.Format("{0}", dr["Inactive"]),
                                                    CHANGED_ON = string.Format("{0}", dr["Changed_On"]),
                                                    CHANGED_BY = string.Format("{0}", dr["Changed_By"]),
                                                }).ToList();
                                else
                                    Results.data = new List<IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL>();
                                break;
                            case "Plant Register Address":
                                if(dt.Rows.Count>0)
                                Results.data = (from DataRow dr in dt.Rows
                                                select new IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL()
                                                {
                                                    ID = string.Format("{0}", dr["Id"]),
                                                    CHANGED_ID = Convert.ToInt32(dr["Changed_Id"]),
                                                    SHORTNAME = string.Format("{0}", dr["Shortname"]),
                                                    CHANGED_CHARNAME = string.Format("{0}", dr["changed_charname"]),
                                                    CHANGED_TABNAME = string.Format("{0}", dr["changed_tabname"]),
                                                    CHANGED_ACTION = string.Format("{0}", dr["Changed_Action"]),
                                                    RegisteredNo = string.Format("{0}", dr["RegisteredNo"]),
                                                    Address = string.Format("{0}", dr["Address"]),
                                                    Plant = string.Format("{0}", dr["Plant"]),
                                                    Inactive = string.Format("{0}", dr["Inactive"]),
                                                    CHANGED_ON = string.Format("{0}", dr["Changed_On"]),
                                                    CHANGED_BY = string.Format("{0}", dr["Changed_By"]),
                                                }).ToList();
                                else
                                    Results.data = new List<IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL>();
                                break;
                            case "Primary Size":
                                if(dt.Rows.Count>0)
                                Results.data = (from DataRow dr in dt.Rows
                                                select new IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL()
                                                {
                                                    ID = string.Format("{0}", dr["Id"]),
                                                    CHANGED_ID = Convert.ToInt32(dr["Changed_Id"]),
                                                    SHORTNAME = string.Format("{0}", dr["Shortname"]),
                                                    CHANGED_CHARNAME = string.Format("{0}", dr["changed_charname"]),
                                                    CHANGED_TABNAME = string.Format("{0}", dr["changed_tabname"]),
                                                    CHANGED_ACTION = string.Format("{0}", dr["Changed_Action"]),

                                                    Code = string.Format("{0}", dr["Code"]),
                                                    Can = string.Format("{0}", dr["Can"]),
                                                    DESCRIPTION = string.Format("{0}", dr["Description"]),
                                                    LidType = string.Format("{0}", dr["LidType"]),
                                                    ContainerType = string.Format("{0}", dr["ContainerType"]),
                                                    DescriptionType = string.Format("{0}", dr["DescriptionType"]),
                                                    Inactive = string.Format("{0}", dr["Inactive"]),
                                                    CHANGED_ON = string.Format("{0}", dr["Changed_On"]),
                                                    CHANGED_BY = string.Format("{0}", dr["Changed_By"]),

                                                }).ToList();
                                else
                                    Results.data = new List<IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL>();
                                break;
                        }
                    }
                }
                else
                {

                    using (var context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            DateTime mydate;
                            string datefrom, dateto, master_name, user_change_log, keyword;

                            mydate = CNService.ConvertStringToDate(param.data.SEARCH_DATE_FROM);
                            datefrom = mydate.ToString("yyyyMMdd");

                            mydate = CNService.ConvertStringToDate(param.data.SEARCH_DATE_TO);
                            dateto = mydate.ToString("yyyyMMdd");

                            if (string.IsNullOrEmpty(param.data.SEARCH_MASTER) || param.data.SEARCH_MASTER == "null")
                            {
                                master_name = "All";
                            }
                            else
                            {
                                master_name = param.data.SEARCH_MASTER;
                            }

                            if (string.IsNullOrEmpty(param.data.SEARCH_USER) || param.data.SEARCH_USER == "null")
                            {
                                user_change_log = "All";
                            }
                            else
                            {
                                user_change_log = param.data.SEARCH_USER;
                            }

                            if (string.IsNullOrEmpty(param.data.SEARCH_KEYWORD) || param.data.SEARCH_KEYWORD == "null")
                            {
                                keyword = "";
                            }
                            else
                            {
                                keyword = param.data.SEARCH_KEYWORD;
                            }


                            var q = context.Database.SqlQuery<IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL>
                          ("sp_IGRID_REPORT_MASTER_DATA_CHANGE_LOG @Material, @FrDt, @ToDt, @User, @Shortname"
                          , new SqlParameter("@Material", "")
                          , new SqlParameter("@FrDt", datefrom)
                          , new SqlParameter("@ToDt", dateto)
                          , new SqlParameter("@User", user_change_log)
                          , new SqlParameter("@Shortname", master_name)
                          ).ToList();


                            if (q != null && q.Count > 0 && keyword != "")
                            {

                                //q = (from m in q
                                //     where m.CHANGED_TABNAME.Contains(keyword)

                                //     select m).ToList();


                                q = q.Where(w => w.CHANGED_ID.ToString().Contains(keyword)

                                   || w.CHANGED_TABNAME.ToLower().Contains(keyword.ToLower())
                                    || w.CHANGED_CHARNAME.ToLower().Contains(keyword.ToLower())
                                      || w.CHANGED_ACTION.ToLower().Contains(keyword.ToLower())
                                        || w.OLD_ID.Contains(keyword.ToLower())
                                          || w.OLD_DESCRIPTION.ToLower().Contains(keyword.ToLower())
                                            || w.ID.Contains(keyword.ToLower())
                                              || w.DESCRIPTION.ToLower().Contains(keyword.ToLower())
                                               || w.CHANGED_BY.ToLower().Contains(keyword.ToLower())
                                                 //|| w.CHANGED_REASON.Contains(keyword)
                                                 || w.CHANGED_ON.ToLower().Contains(keyword.ToLower())
                                                   || (!string.IsNullOrEmpty(w.SHORTNAME) && w.SHORTNAME.ToLower().Contains(keyword.ToLower()))
                                ).ToList();


                            }


                            Results.data = q;
                            //Results.data = context.Database.SqlQuery<IGRID_CBB_DATA_MODEL>("sp_IGRID_GET_USER_MASTER_DATA_CHANGE_LOG").ToList();
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
