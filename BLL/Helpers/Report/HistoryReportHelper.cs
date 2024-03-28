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
    public class HistoryReportHelper
    {


        public static KPILog_SummarizeGroup_Report_RESULT GetKPISumerizeApproveReport(KPILog_Summarize_REPORT_REQUEST param)
        {
            {
                KPILog_SummarizeGroup_Report_RESULT Results = new KPILog_SummarizeGroup_Report_RESULT();

                if (param != null && param.data != null && param.data.first_load == "1")
                {

                    Results.status = "S";
                    Results.data = new List<KPILog_SummarizeGroup_Report>();
                    Results.draw = param.draw;
                    return Results;

                }

                try
                {

                    using (var context = new IGRIDEntities())
                    {
                        context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                        var qList = context.Database.SqlQuery<KPILog_Summarize_REPORT>
                      ("spGetKPILog_Summarize @LayOut, @FrDt, @ToDt"
                      , new SqlParameter("@LayOut", param.data.LayOut)
                      , new SqlParameter("@FrDt", Convert.ToDateTime(param.data.FrDt).ToString("yyyyMMdd"))
                      , new SqlParameter("@ToDt", Convert.ToDateTime(param.data.ToDt).ToString("yyyyMMdd"))

                      ).ToList();


                        if (param.data.LayOut == "PA_SUMApprove")
                            qList = qList.OrderBy(o => o.Log_PA_ModifyByFullName).ToList();
                        else
                            qList = qList.OrderBy(o => o.Log_PG_ModifyByFullName).ToList();




                        var prev_modify_username = "";
                        List<KPILog_SummarizeGroup_Report> listGroup = new List<KPILog_SummarizeGroup_Report>();

                        KPILog_SummarizeGroup_Report group = new KPILog_SummarizeGroup_Report();

                        foreach (var q in qList)
                        {

                            if (param.data.LayOut == "PA_SUMApprove")
                            {
                                if (q.Log_PA_ModifyBy.Trim().ToLower() != prev_modify_username.Trim().ToLower())
                                {

                                    group = new KPILog_SummarizeGroup_Report();
                                    group.data = new List<KPILog_Summarize_REPORT>();
                                    listGroup.Add(group);

                                    group.MODIFY_BY_FULLNAME = q.Log_PA_ModifyByFullName;
                                    group.MODIFY_BY_USERNAME = "Modify by FullName:";
                                }

                                q.MODIFY_BY_USERNAME = q.Log_PA_ModifyBy;
                                q.MODIFY_BY_FULLNAME = q.Log_PA_ModifyByFullName;
                                q.CREATE_USERNAME = q.CreateBy;
                                q.CREATE_FULLNAME = q.CreateByFullName;
                                q.MODIFYED_RECOORD = q.Count;

                                group.data.Add(q);
                                
                                if (!string.IsNullOrEmpty(param.data.EXPORT_EXCEL))
                                    group.data = group.data.OrderBy(o => o.CREATE_USERNAME).ToList();
                                else
                                    group.data = group.data.OrderByDescending(o => o.CREATE_USERNAME).ToList();

                                prev_modify_username = q.MODIFY_BY_USERNAME;
                            }
                            else if (param.data.LayOut == "PG_SUMApprove")
                            {
                                if (q.Log_PG_ModifyBy != prev_modify_username)
                                {

                                    group = new KPILog_SummarizeGroup_Report();
                                    group.data = new List<KPILog_Summarize_REPORT>();
                                    listGroup.Add(group);

                                    group.MODIFY_BY_FULLNAME = q.Log_PG_ModifyByFullName;
                                    group.MODIFY_BY_USERNAME = "Modify by FullName:";
                                }

                                q.MODIFY_BY_USERNAME = q.Log_PG_ModifyBy;
                                q.MODIFY_BY_FULLNAME = q.Log_PG_ModifyByFullName;
                                q.CREATE_USERNAME = q.Assignee;
                                q.CREATE_FULLNAME = q.AssigneeFullName;
                                q.MODIFYED_RECOORD = q.Count;

                                group.data.Add(q);
                                if (!string.IsNullOrEmpty(param.data.EXPORT_EXCEL))
                                    group.data = group.data.OrderBy(o => o.CREATE_USERNAME).ToList();
                                else
                                    group.data = group.data.OrderByDescending(o => o.CREATE_USERNAME).ToList();

                                prev_modify_username = q.MODIFY_BY_USERNAME;

                            }

                        }


                        Results.data = listGroup;
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

        public static KPILog_SummarizeGroup_Report_RESULT GetKPISumerizeOwnereport(KPILog_Summarize_REPORT_REQUEST param)
        {
            {
                KPILog_SummarizeGroup_Report_RESULT Results = new KPILog_SummarizeGroup_Report_RESULT();

                if (param != null && param.data != null && param.data.first_load == "1")
                {

                    Results.status = "S";
                    Results.data = new List<KPILog_SummarizeGroup_Report>();
                    Results.draw = param.draw;
                    return Results;

                }

                try
                {

                    using (var context = new IGRIDEntities())
                    {
                        context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                        var qList = context.Database.SqlQuery<KPILog_Summarize_REPORT>
                      ("spGetKPILog_Summarize @LayOut, @FrDt, @ToDt"
                      , new SqlParameter("@LayOut", param.data.LayOut)
                      , new SqlParameter("@FrDt", Convert.ToDateTime(param.data.FrDt).ToString("yyyyMMdd"))
                      , new SqlParameter("@ToDt", Convert.ToDateTime(param.data.ToDt).ToString("yyyyMMdd"))

                      ).ToList();


                        if (param.data.LayOut == "PA")
                            qList = qList.OrderBy(o => o.CreateByFullName).ToList();
                        else
                            qList = qList.OrderBy(o => o.AssigneeFullName).ToList();




                        var prev_create_username = "";
                        List<KPILog_SummarizeGroup_Report> listGroup = new List<KPILog_SummarizeGroup_Report>();

                        KPILog_SummarizeGroup_Report group = new KPILog_SummarizeGroup_Report();

                        foreach (var q in qList)
                        {

                            if (param.data.LayOut == "PA")
                            {
                                if (q.CreateBy.Trim().ToLower() != prev_create_username.Trim().ToLower())
                                {

                                    group = new KPILog_SummarizeGroup_Report();
                                    group.data = new List<KPILog_Summarize_REPORT>();
                                    listGroup.Add(group);
                                    group.CREATE_USERNAME = q.CreateBy;
                                    group.CREATE_FULLNAME = q.CreateByFullName;


                                    group.MODIFYED_RECOORD = q.Count_All;
                                    group.CREATED_RECOORD = q.SAPMat_Count;
                                    if (q.SAPMat_Count > 0)
                                    {
                                        group.PECENTAGE_ERROR = Convert.ToDouble(100.00 * q.Count_All / q.SAPMat_Count).ToString("00.00") + "%";
                                    }
                                    else
                                    {
                                        group.PECENTAGE_ERROR = "00.00%";
                                    }

                                }
                        
                                q.MODIFY_BY_USERNAME = q.Log_PA_ModifyBy;
                                q.MODIFY_BY_FULLNAME = q.Log_PA_ModifyByFullName;
                                q.CREATE_USERNAME = q.CreateBy;
                                q.CREATE_FULLNAME = q.CreateByFullName;

                                group.data.Add(q);
                                prev_create_username = group.CREATE_USERNAME;
                            }
                            else if (param.data.LayOut == "PG")
                            {
                                if (q.Assignee != prev_create_username)
                                {

                                    group = new KPILog_SummarizeGroup_Report();
                                    group.data = new List<KPILog_Summarize_REPORT>();
                                    listGroup.Add(group);
                                    group.CREATE_USERNAME = q.Assignee;
                                    group.CREATE_FULLNAME = q.AssigneeFullName;


                                    group.MODIFYED_RECOORD = q.Count_All;
                                    group.CREATED_RECOORD = q.SAPMat_Count;
                                    if (q.SAPMat_Count > 0)
                                    {
                                        group.PECENTAGE_ERROR = Convert.ToDouble(100.00 * q.Count_All / q.SAPMat_Count).ToString("00.00") + "%";
                                    }
                                    else
                                    {
                                        group.PECENTAGE_ERROR = "00.00%";
                                    }

                                }

                                q.MODIFY_BY_USERNAME = q.Log_PG_ModifyBy;
                                q.MODIFY_BY_FULLNAME = q.Log_PG_ModifyByFullName;
                                q.CREATE_USERNAME = q.Assignee;
                                q.CREATE_FULLNAME = q.AssigneeFullName;

                                group.data.Add(q);
                                prev_create_username = group.CREATE_USERNAME;

                            }

                        }


                        Results.data = listGroup;
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



        public static KPILog_Summarize_REPORT_RESULT GetkpisummaryReport(KPILog_Summarize_REPORT_REQUEST param)
        {
            {
                KPILog_Summarize_REPORT_RESULT Results = new KPILog_Summarize_REPORT_RESULT();

                if (param != null && param.data != null && param.data.first_load == "1")
                {

                    Results.status = "S";
                    Results.data = new List<KPILog_Summarize_REPORT>();
                    Results.draw = param.draw;
                    return Results;

                }

                try
                {

                    using (var context = new IGRIDEntities())
                    {
                        context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
                        var q = context.Database.SqlQuery<KPILog_Summarize_REPORT>
                      ("spGetKPILog_Summarize @LayOut, @FrDt, @ToDt"
                      , new SqlParameter("@LayOut", param.data.LayOut)
                      , new SqlParameter("@FrDt", Convert.ToDateTime(param.data.FrDt).ToString("yyyyMMdd"))
                      , new SqlParameter("@ToDt", Convert.ToDateTime(param.data.ToDt).ToString("yyyyMMdd"))

                      ).ToList();
                        Results.data = q;
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
        public static HISTORY_REPORT_RESULT GetHistoryReport(HISTORY_REPORT_REQUEST param)
        {
            {
                HISTORY_REPORT_RESULT Results = new HISTORY_REPORT_RESULT();

                if (param != null && param.data != null && param.data.first_load == "1")
                {

                    Results.status = "S";
                    Results.data = new List<HISTORY_REPORT>();
                    Results.draw = param.draw;
                    return Results;

                }

                try
                {

                    using (var context = new IGRIDEntities())
                    {
                        context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);


                        var qlist = context.Database.SqlQuery<HISTORY_REPORT>
                      ("spGetKPILog2 @LayOut, @FrDt, @ToDt"
                      , new SqlParameter("@LayOut", param.data.LayOut)
                      , new SqlParameter("@FrDt", Convert.ToDateTime(param.data.FrDt).ToString("yyyyMMdd"))
                      , new SqlParameter("@ToDt", Convert.ToDateTime(param.data.ToDt).ToString("yyyyMMdd"))

                      ).ToList();
                        foreach (var q in qlist)
                        {
                            if (string.Format("{0}", q.Old_Brand) == "")
                            {
                                q.Brand = "";
                            }
                            if (string.Format("{0}", q.Old_ChangePoint) == "")
                                q.ChangePoint = "";

                            if (string.Format("{0}", q.Old_MaterialGroup) == "")
                                q.MaterialGroup = "";

                            if (string.Format("{0}", q.Old_PrimarySize) == "")
                                q.PrimarySize = "";

                            if (string.Format("{0}", q.Old_ContainerType) == "")
                                q.ContainerType = "";

                            if (string.Format("{0}", q.Old_LidType) == "")
                                q.LidType = "";

                            if (string.Format("{0}", q.Old_PackingStyle) == "")
                                q.PackingStyle = "";

                            if (string.Format("{0}", q.Old_Packing) == "")
                                q.Packing = "";

                            if (string.Format("{0}", q.Old_StyleofPrinting) == "")
                                q.StyleofPrinting = "";

                            if (string.Format("{0}", q.Old_ProductCode) == "")
                                q.ProductCode = "";

                            if (string.Format("{0}", q.Old_FAOZone) == "")
                                q.FAOZone = "";

                            if (string.Format("{0}", q.Old_Plant) == "")
                                q.Plant = "";


                            if (string.Format("{0}", q.Old_PMScolour) == "")
                                q.PMScolour = "";
                            if (string.Format("{0}", q.Old_Processcolour) == "")
                                q.Processcolour = "";

                            if (string.Format("{0}", q.Old_Totalcolour) == "")
                                q.Totalcolour = "";

                            if (string.Format("{0}", q.Old_PlantRegisteredNo) == "")
                                q.PlantRegisteredNo = "";

                            if (string.Format("{0}", q.Old_CompanyNameAddress) == "")
                                q.CompanyNameAddress = "";

                            if (string.Format("{0}", q.Old_Symbol) == "")
                                q.Symbol = "";

                            if (string.Format("{0}", q.Old_CatchingArea) == "")
                                q.CatchingArea = "";

                            if (string.Format("{0}", q.Old_CatchingPeriodDate) == "")
                                q.CatchingPeriodDate = "";
                            if (string.Format("{0}", q.Old_PrintingStyleofPrimary) == "")
                                q.PrintingStyleofPrimary = "";
                            if (string.Format("{0}", q.Old_PrintingStyleofSecondary) == "")
                                q.PrintingStyleofSecondary = "";
                            if (string.Format("{0}", q.Old_Typeof) == "")
                                q.Typeof = "";
                            if (string.Format("{0}", q.Old_TypeofCarton2) == "")
                                q.TypeofCarton2 = "";
                            if (string.Format("{0}", q.Old_DMSNo) == "")
                                q.DMSNo = "";
                            if (string.Format("{0}", q.Old_TypeofPrimary) == "")
                                q.TypeofPrimary = "";
                            if (string.Format("{0}", q.Old_Direction) == "")
                                q.Direction = "";
                            if (string.Format("{0}", q.Old_PlantAddress) == "")
                                q.PlantAddress = "";
                            if (string.Format("{0}", q.Old_Catching_Method) == "")
                                q.Catching_Method = "";
                            if (string.Format("{0}", q.Old_Scientific_Name) == "")
                                q.Scientific_Name = "";
                            if (string.Format("{0}", q.Old_Specie) == "")
                                q.Specie = "";

                            if (string.Format("{0}", q.Old_Grandof) == "")
                                q.Grandof = "";

                            if (string.Format("{0}", q.Old_SheetSize) == "")
                                q.SheetSize = "";

                            if (string.Format("{0}", q.Old_Vendor) == "")
                                q.Vendor = "";
                            if (string.Format("{0}", q.Old_Flute) == "")
                                q.Flute = "";
                            if (string.Format("{0}", q.Old_Dimension) == "")
                                q.Dimension = "";
                            if (string.Format("{0}", q.Old_RSC) == "")
                                q.RSC = "";
                            if (string.Format("{0}", q.Old_Accessories) == "")
                                q.Accessories = "";
                            if (string.Format("{0}", q.Old_PrintingSystem) == "")
                                q.PrintingSystem = "";
                            if (string.Format("{0}", q.Old_RollSheet) == "")
                                q.RollSheet = "";


                        } 

                            Results.data = qlist;

                      
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
            //HISTORY_REPORT_RESULT Results = new HISTORY_REPORT_RESULT();

            //try
            //{
            //    Results.status = "S";
            //    Results.data = CNService.GetKPILog(param);
            //    Results.draw = param.draw;
            //}
            //catch (Exception ex)
            //{
            //    Results.status = "E";
            //    Results.msg = CNService.GetErrorMessage(ex);
            //}

            //return Results;
        }
    }
}