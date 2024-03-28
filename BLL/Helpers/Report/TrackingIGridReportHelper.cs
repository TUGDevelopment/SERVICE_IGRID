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
    public class TrackingIGridReportHelper
    {
        public static TRACKINGIGRID_REPORT_RESULT saveImpactedMatDesc(TRACKINGIGRID_REPORT_REQUEST param)
        {
            TRACKINGIGRID_REPORT_RESULT Results = new TRACKINGIGRID_REPORT_RESULT();
            try
            {
                CNService.ReUpload(param.data.ID);
                //Results.status = "S";
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
        public static CHARTTRACKING_REPORT_RESULT GetCountTrackingIGridReport(CHARTTRACKING_REPORT_REQUEST param)
        {
            CHARTTRACKING_REPORT_RESULT Results = new CHARTTRACKING_REPORT_RESULT();
            List<CHARTTRACKING_REPORT> listIGrid = new List<CHARTTRACKING_REPORT>();
            try
            {

                //Results.data = CNService.GetTrackingReport(param);
                //Results.draw = param.draw;
                using (var context = new IGRIDEntities())
                {
                    context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);


                    var q = context.Database.SqlQuery<CHARTTRACKING_REPORT>
                  ("spGetCountTrackingReport @Material, @Condition, @User, @FrDt, @ToDt"
                  , new SqlParameter("@Material", string.Format("{0}", param.data.Keyword))
                  , new SqlParameter("@Condition", param.data.Status)
                  , new SqlParameter("@User", string.Format("{0}", param.data.By == null || param.data.By == "null" ? "All" : param.data.By))
                  , new SqlParameter("@FrDt", Convert.ToDateTime(param.data.FrDt).ToString("yyyyMMdd"))
                  , new SqlParameter("@ToDt", Convert.ToDateTime(param.data.ToDt).ToString("yyyyMMdd"))

                  ).ToList();

                    Results.status = "S";
                    Results.data = q;


                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }
        public static TRACKINGIGRID_REPORT_RESULT GetTrackingIGridReport(TRACKINGIGRID_REPORT_REQUEST param)
        {
            TRACKINGIGRID_REPORT_RESULT Results = new TRACKINGIGRID_REPORT_RESULT();
            Results.Canceled_Record = 0;
            Results.Completed_Record = 0;
            Results.Failed_Record = 0;
            Results.InProcess_Record = 0;
            if (param != null && param.data != null && param.data.first_load == "1")
            {

                Results.status = "S";
                Results.data = new List<TRACKINGIGRID_REPORT>();
                Results.draw = param.draw;
                return Results;

            }
            try
            {

                //Results.data = CNService.GetTrackingReport(param);
                //Results.draw = param.draw;

                if (string.IsNullOrEmpty(param.data.Keyword) || param.data.Keyword == "null")
                {
                    param.data.Keyword = "";
                }
                if (string.IsNullOrEmpty(param.data.By) || param.data.By == "null")
                {
                    param.data.By = "";
                }


                using (var context = new IGRIDEntities())
                {
                    context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                    



                    var q = context.Database.SqlQuery<TRACKINGIGRID_REPORT>
                  ("spGetTrackingReport @Material, @Condition, @User, @FrDt, @ToDt"
                  , new SqlParameter("@Material", string.Format("{0}", param.data.Keyword))
                  , new SqlParameter("@Condition", param.data.Role)
                  , new SqlParameter("@User", string.Format("{0}", param.data.By == null || param.data.By == "null" ? "" : param.data.By))
                  , new SqlParameter("@FrDt", Convert.ToDateTime(param.data.FrDt).ToString("yyyyMMdd"))
                  , new SqlParameter("@ToDt", Convert.ToDateTime(param.data.ToDt).ToString("yyyyMMdd"))

                  ).ToList();


                    if (!string.IsNullOrEmpty(param.data.Keyword))
                    {
                        var kw = param.data.Keyword;
                        q = q.Where(w => w.DocumentNo.Contains(kw) 
                        || w.Material.Contains(kw)
                        || w.Description.Contains(kw)
                        ).ToList();
                    }

                

                    var transApprove = context.Database.SqlQuery<TRANSAPPROVE_MODEL>("select matdoc, fn from TransApprove where statusapp = '5'").ToList();


                    //var str_status = "";
                    //var Completed_Record = 0;
                    //var Failed_Record = 0;
                    //var Canceled_Record = 0;
                    //var InProcess_Record = 0;
                    var Check_Cancel = "";
                    TRACKINGIGRID_REPORT data;
                    for (int i = q.Count -1 ; i >=0; i--)
                    {
                        data = q[i];

                        if (data.Final_ApprovedBy == "MDC_Approve")
                        {
                            data.Status_upd = "Completed";
                            Results.Completed_Record += 1;
                        }
                        else if (data.Final_ApprovedBy == "PS_Approve" && data.Material != "")
                        {
                            data.Status_upd = "Completed";
                            Results.Completed_Record += 1;
                        }
                        else
                        {
                            if (data.Status_upd == "Fail")
                            {
                                data.Status_upd = "Failed";
                                Results.Failed_Record += 1;
                                if (param.data.Status != "Failed" && param.data.Status != "All")
                                {
                                    //remove
                                    q.Remove(data);
                                }
                            }
                            else if (data.Status_upd == "Re-Uploading")
                            {
                                data.Status_upd = "Re-Uploading";
                                Results.Failed_Record += 1;
                                if (param.data.Status != "Re-Uploading" && param.data.Status != "All")
                                {
                                    //remove
                                    q.Remove(data);
                                }
                            }
                            else
                            {
                                Check_Cancel = "";
                                var fn = transApprove.Where(w => w.MatDoc == data.ID).Select(s => s.fn).FirstOrDefault();
                                switch (fn)
                                {
                                    case "PA_Approve":
                                        Check_Cancel = "Canceled by PA_Approve";
                                        break;
                                    case "PG_Approve":
                                        Check_Cancel = "Canceled by PG_Approve";
                                        break;
                                    case "PS_Approve":
                                        Check_Cancel = "Canceled by PS_Approve";
                                        break;
                                    case "MDC_Approve":
                                        Check_Cancel = "Canceled by MDC_Approve";
                                        break;
                                }

                                if (Check_Cancel == "" && !string.IsNullOrEmpty(data.PG_ApproveBy) && !string.IsNullOrEmpty(data.PA_ApproveBy) && !string.IsNullOrEmpty(data.Final_ApprovedBy))
                                {
                                    data.Status_upd = "Completed";
                                    Results.Completed_Record += 1;

                                }
                                else
                                {
                                    if (Check_Cancel != "")
                                    {
                                        data.Status_upd = Check_Cancel;
                                        Results.Canceled_Record += 1;
                                    }
                                    else if (string.IsNullOrEmpty(data.Status_upd) && string.IsNullOrEmpty(data.Final_ApprovedBy) && data.Final_ApprovedBy != "PA_Approve" && data.Final_ApprovedBy != "PG_Approve"
                                      || (data.Final_ApprovedBy == "PA_Approve" && string.IsNullOrEmpty(data.PG_ApproveBy) && data.Status_upd == "")
                                      || (data.Final_ApprovedBy == "PG_Approve" && string.IsNullOrEmpty(data.PA_ApproveBy) && data.Status_upd == ""))
                                    {

                                        data.Status_upd = "In Process";
                                        Results.InProcess_Record += 1;

                                    }
                                    else if (string.IsNullOrEmpty(data.Final_ApprovedBy))
                                    {
                                        data.Status_upd = "In Process";
                                        Results.InProcess_Record += 1;
                                    }
                                    else

                                    {
                                        data.Status_upd = "In Process";
                                        Results.InProcess_Record += 1;

                                    }
                                }

                            }
                        }

                    }
                   

                    var check_user = "";
                    bool check_status = true;

                    if ((!string.IsNullOrEmpty(param.data.By)) || (param.data.Status != "All" ))
                    {
                        for (int i = q.Count - 1; i >= 0; i--)
                        {
                            data = q[i];

                            check_status = true;
                            // check status
                            if (param.data.Status != "All")
                            {

                                if (param.data.Status == "Completed" && data.Status_upd == "Completed")
                                {
                                }
                                else if (param.data.Status == "Canceled" && data.Status_upd.IndexOf("Canceled") != -1)
                                {
                                }
                                else if (param.data.Status == "In Process" && data.Status_upd == "In Process")
                                {
                                }
                                else if (param.data.Status == "Failed" && data.Status_upd == "Failed")
                                {
                                }
                                else if (param.data.Status == "Re-Uploading" && data.Status_upd == "Re-Uploading")
                                {
                                }
                                else
                                {
                                    q.Remove(data);
                                    check_status = false;
                                }
                            }

                            //check user
                            if (!string.IsNullOrEmpty(param.data.By) && check_status && !string.IsNullOrEmpty(param.data.Name))
                            {


                               // var name = "";


                                if (param.data.Role == "PA" || param.data.Role == "PA_Submit")
                                {
                                    check_user = data.PA_InputBy;
                                }
                                else if (param.data.Role == "PA_Approve")
                                {
                                    check_user = data.PA_ApproveBy;
                                }
                                else if (param.data.Role == "PG")
                                {
                                    check_user = data.PG_InputBy;
                                }
                                else if (param.data.Role == "PG_Approve")
                                {
                                    check_user = data.PG_ApproveBy;
                                }
                                else if (param.data.Role == "Final_Approve")
                                {
                                    check_user = data.Final_ApprovedName;
                                }
                                else if (param.data.Role == "InfoGroup")
                                {
                                    check_user = data.InfoGroupBy;
                                }

                                if (check_user == param.data.Name)
                                {
                                }
                                else
                                {
                                    q.Remove(data);
                                    check_status = false;
                                }

                            }


                            //check keywork // aof move filter at after query from store
                            if (!string.IsNullOrEmpty(param.data.Keyword) && check_status)
                            {
                            }

                            //if (q.Count() == 0)
                            //{
                            //    break;
                            //}
                        }



                        // re-count recoard
                        Results.Canceled_Record = 0;
                        Results.Completed_Record = 0;
                        Results.Failed_Record = 0;
                        Results.InProcess_Record = 0;
                        for (int i = q.Count - 1; i >= 0; i--)
                        {
                            data = q[i];
                            if (data.Status_upd == "Completed")
                            {
                                Results.Completed_Record += 1;
                            }
                            else if (data.Status_upd.IndexOf("Canceled") != -1)
                            {
                                Results.Canceled_Record += 1;
                            }
                            else if (data.Status_upd == "In Process")
                            {
                                Results.InProcess_Record += 1;
                            }
                            else if (data.Status_upd == "Failed")
                            {
                                Results.Failed_Record += 1;
                            }
                            else if (data.Status_upd == "Re-Uploading")
                            {
                                Results.Failed_Record += 1;
                            }
                        }

                    }


                
                    Results.status = "S";
                    Results.data = q;
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