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

namespace BLL.Helpers
{
    public class SummaryReportHelper
    {
        public static SUMMARY_REPORT_MODEL_RESULT GetSummaryReportChartByStep(SUMMARY_REPORT_MODEL_REQUEST param)
        {
            var Results = new SUMMARY_REPORT_MODEL_RESULT();

            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<SUMMARY_REPORT_MODEL>();
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                var res = new SUMMARY_REPORT_MODEL_RESULT();
                res.data = new List<SUMMARY_REPORT_MODEL>();

                var currentDate = DateTime.Now;
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 1800;

                        if (1 == 1)
                        {
                            param.data.DEPARTMENT = "PK"; var tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                            param.data.DEPARTMENT = "MK"; tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                            param.data.DEPARTMENT = "QC"; tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                            param.data.DEPARTMENT = "RD"; tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                            param.data.DEPARTMENT = "WH"; tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                            param.data.DEPARTMENT = "PN"; tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                            param.data.DEPARTMENT = "CUS"; tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                            param.data.DEPARTMENT = "VN"; tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                        }

                        if (1 == 1)
                        {
                            param.data.DEPARTMENT = "PK"; var tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                            param.data.DEPARTMENT = "MK"; tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                            param.data.DEPARTMENT = "QC"; tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                            param.data.DEPARTMENT = "RD"; tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                            param.data.DEPARTMENT = "WH"; tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                            param.data.DEPARTMENT = "PN"; tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                            param.data.DEPARTMENT = "CUS"; tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                            param.data.DEPARTMENT = "VN"; tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                        }
                    }
                }

                foreach (var item in res.data)
                {
                    if (item.DEPARTMENT2 == "PK")
                    {
                        res.CNT_PK_ONTIME += item.INPROCESS_ONTIME + item.COMPLETED_ONTIME;
                        res.CNT_PK_ALMOSTDUE += item.INPROCESS_ALMOST_DUE;
                        res.CNT_PK_OVERDUE += item.INPROCESS_OVER_DUE + item.COMPLETED_OVER_DUE;
                    }
                    if (item.DEPARTMENT2 == "MK")
                    {
                        res.CNT_MK_ONTIME += item.INPROCESS_ONTIME + item.COMPLETED_ONTIME;
                        res.CNT_MK_ALMOSTDUE += item.INPROCESS_ALMOST_DUE;
                        res.CNT_MK_OVERDUE += item.INPROCESS_OVER_DUE + item.COMPLETED_OVER_DUE;
                    }
                    if (item.DEPARTMENT2 == "QC")
                    {
                        res.CNT_QC_ONTIME += item.INPROCESS_ONTIME + item.COMPLETED_ONTIME;
                        res.CNT_QC_ALMOSTDUE += item.INPROCESS_ALMOST_DUE;
                        res.CNT_QC_OVERDUE += item.INPROCESS_OVER_DUE + item.COMPLETED_OVER_DUE;
                    }
                    if (item.DEPARTMENT2 == "RD")
                    {
                        res.CNT_RD_ONTIME += item.INPROCESS_ONTIME + item.COMPLETED_ONTIME;
                        res.CNT_RD_ALMOSTDUE += item.INPROCESS_ALMOST_DUE;
                        res.CNT_RD_OVERDUE += item.INPROCESS_OVER_DUE + item.COMPLETED_OVER_DUE;
                    }
                    if (item.DEPARTMENT2 == "WH")
                    {
                        res.CNT_WH_ONTIME += item.INPROCESS_ONTIME + item.COMPLETED_ONTIME;
                        res.CNT_WH_ALMOSTDUE += item.INPROCESS_ALMOST_DUE;
                        res.CNT_WH_OVERDUE += item.INPROCESS_OVER_DUE + item.COMPLETED_OVER_DUE;
                    }
                    if (item.DEPARTMENT2 == "PN")
                    {
                        res.CNT_PN_ONTIME += item.INPROCESS_ONTIME + item.COMPLETED_ONTIME;
                        res.CNT_PN_ALMOSTDUE += item.INPROCESS_ALMOST_DUE;
                        res.CNT_PN_OVERDUE += item.INPROCESS_OVER_DUE + item.COMPLETED_OVER_DUE;
                    }
                    if (item.DEPARTMENT2 == "CUS")
                    {
                        res.CNT_CUS_ONTIME += item.INPROCESS_ONTIME + item.COMPLETED_ONTIME;
                        res.CNT_CUS_ALMOSTDUE += item.INPROCESS_ALMOST_DUE;
                        res.CNT_CUS_OVERDUE += item.INPROCESS_OVER_DUE + item.COMPLETED_OVER_DUE;
                    }
                    if (item.DEPARTMENT2 == "VN")
                    {
                        res.CNT_VN_ONTIME += item.INPROCESS_ONTIME + item.COMPLETED_ONTIME;
                        res.CNT_VN_ALMOSTDUE += item.INPROCESS_ALMOST_DUE;
                        res.CNT_VN_OVERDUE += item.INPROCESS_OVER_DUE + item.COMPLETED_OVER_DUE;
                    }
                }

                Results = res;
                Results.status = "S";
                Results.data = res.data;

            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static SUMMARY_REPORT_MODEL_RESULT GetSummaryReportChartByWorkflowType(SUMMARY_REPORT_MODEL_REQUEST param)
        {
            var Results = new SUMMARY_REPORT_MODEL_RESULT();

            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<SUMMARY_REPORT_MODEL>();
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                var res = new SUMMARY_REPORT_MODEL_RESULT();
                res.data = new List<SUMMARY_REPORT_MODEL>();
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 1800;

                        param.data.WORKFLOW_TYPE = "ALL";
                        res = GetSummaryReportDataByWorkflowType(param);
                    }
                }

                foreach (var item in res.data)
                {
                    if (item.WORKFLOW_TYPE == "Artwork new")
                    {
                        res.WF_NEW_ONTIME += item.INPROCESS_ONTIME + item.COMPLETED_ONTIME;
                        res.WF_NEW_ALMOST_DUE += item.INPROCESS_ALMOST_DUE;
                        res.WF_NEW_OVER_DUE += item.INPROCESS_OVER_DUE + item.COMPLETED_OVER_DUE;
                    }
                    if (item.WORKFLOW_TYPE == "Artwork repeat")
                    {
                        res.WF_REPEAT_ONTIME += item.INPROCESS_ONTIME + item.COMPLETED_ONTIME;
                        res.WF_REPEAT_ALMOST_DUE += item.INPROCESS_ALMOST_DUE;
                        res.WF_REPEAT_OVER_DUE += item.INPROCESS_OVER_DUE + item.COMPLETED_OVER_DUE;
                    }
                    if (item.WORKFLOW_TYPE == "Artwork repeat R6")
                    {
                        res.WF_REPEATR6_ONTIME += item.INPROCESS_ONTIME + item.COMPLETED_ONTIME;
                        res.WF_REPEATR6_ALMOST_DUE += item.INPROCESS_ALMOST_DUE;
                        res.WF_REPEATR6_OVER_DUE += item.INPROCESS_OVER_DUE + item.COMPLETED_OVER_DUE;
                    }
                    if (item.WORKFLOW_TYPE == "Mockup design")
                    {
                        res.WF_MODESIGN_ONTIME += item.INPROCESS_ONTIME + item.COMPLETED_ONTIME;
                        res.WF_MODESIGN_ALMOST_DUE += item.INPROCESS_ALMOST_DUE;
                        res.WF_MODESIGN_OVER_DUE += item.INPROCESS_OVER_DUE + item.COMPLETED_OVER_DUE;
                    }
                    if (item.WORKFLOW_TYPE == "Mockup dieline")
                    {
                        res.WF_MODIELINE_ONTIME += item.INPROCESS_ONTIME + item.COMPLETED_ONTIME;
                        res.WF_MODIELINE_ALMOST_DUE += item.INPROCESS_ALMOST_DUE;
                        res.WF_MODIELINE_OVER_DUE += item.INPROCESS_OVER_DUE + item.COMPLETED_OVER_DUE;
                    }
                    if (item.WORKFLOW_TYPE == "Mockup normal")
                    {
                        res.WF_MONORMAL_ONTIME += item.INPROCESS_ONTIME + item.COMPLETED_ONTIME;
                        res.WF_MONORMAL_ALMOST_DUE += item.INPROCESS_ALMOST_DUE;
                        res.WF_MONORMAL_OVER_DUE += item.INPROCESS_OVER_DUE + item.COMPLETED_OVER_DUE;
                    }
                }

                Results = res;
                Results.status = "S";
                Results.data = res.data;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static SUMMARY_REPORT_MODEL_RESULT GetSummaryReportDataByStep(SUMMARY_REPORT_MODEL_REQUEST param)
        {
            var Results = new SUMMARY_REPORT_MODEL_RESULT();

            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<SUMMARY_REPORT_MODEL>();
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                var res = new SUMMARY_REPORT_MODEL_RESULT();
                res.data = new List<SUMMARY_REPORT_MODEL>();

                var paramDEPARTMENT = param.data.DEPARTMENT;
                var currentDate = DateTime.Now;
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 1800;

                        if (1 == 1)
                        {
                            if (param.data.WORKFLOW_TYPE == "MO" || param.data.WORKFLOW_TYPE == "ALL")
                            {
                                if (!string.IsNullOrEmpty(param.data.DEPARTMENT))
                                {
                                    if (paramDEPARTMENT == "ALL")
                                    {
                                        param.data.DEPARTMENT = "PK"; var tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                        param.data.DEPARTMENT = "MK"; tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                        param.data.DEPARTMENT = "QC"; tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                        param.data.DEPARTMENT = "RD"; tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                        param.data.DEPARTMENT = "WH"; tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                        param.data.DEPARTMENT = "PN"; tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                        param.data.DEPARTMENT = "CUS"; tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                        param.data.DEPARTMENT = "VN"; tempRes = mockup(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                    }
                                    else
                                    {
                                        var tempRes = mockup(context, param);
                                        foreach (var item in tempRes.data)
                                        {
                                            res.data.Add(item);
                                        }
                                    }
                                }
                            }
                        }

                        if (1 == 1)
                        {
                            if (param.data.WORKFLOW_TYPE == "ART" || param.data.WORKFLOW_TYPE == "ALL")
                            {
                                if (!string.IsNullOrEmpty(param.data.DEPARTMENT))
                                {
                                    if (paramDEPARTMENT == "ALL")
                                    {
                                        param.data.DEPARTMENT = "PK"; var tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                        param.data.DEPARTMENT = "MK"; tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                        param.data.DEPARTMENT = "QC"; tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                        param.data.DEPARTMENT = "RD"; tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                        param.data.DEPARTMENT = "WH"; tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                        param.data.DEPARTMENT = "PN"; tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                        param.data.DEPARTMENT = "CUS"; tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                        param.data.DEPARTMENT = "VN"; tempRes = artwork(context, param); foreach (var item in tempRes.data) res.data.Add(item);
                                    }
                                    else
                                    {
                                        var tempRes = artwork(context, param);
                                        foreach (var item in tempRes.data)
                                        {
                                            res.data.Add(item);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (var item in res.data)
                    {
                        item.WF_TOTAL_INPROCESS_BY_WORKFLOW_STEP += item.INPROCESS_ONTIME + item.INPROCESS_ALMOST_DUE + item.INPROCESS_OVER_DUE;
                        item.WF_TOTAL_COMPLETE_BY_WORKFLOW_STEP += item.COMPLETED_ONTIME + item.COMPLETED_OVER_DUE;
                        item.WF_TOTAL += item.WF_TOTAL_INPROCESS_BY_WORKFLOW_STEP + item.WF_TOTAL_COMPLETE_BY_WORKFLOW_STEP;
                    }

                    double allInProcess = 0;
                    double allCompleted = 0;
                    var tempTotal = new SUMMARY_REPORT_MODEL();
                    foreach (var item in res.data)
                    {
                        tempTotal.ORDERBY = 8;
                        tempTotal.WORKFLOW_TYPE = "Total";

                        tempTotal.INPROCESS_ONTIME += item.INPROCESS_ONTIME;
                        tempTotal.INPROCESS_ALMOST_DUE += item.INPROCESS_ALMOST_DUE;
                        tempTotal.INPROCESS_OVER_DUE += item.INPROCESS_OVER_DUE;
                        tempTotal.COMPLETED_ONTIME += item.COMPLETED_ONTIME;
                        tempTotal.COMPLETED_OVER_DUE += item.COMPLETED_OVER_DUE;

                        allInProcess += item.INPROCESS_ONTIME + item.INPROCESS_ALMOST_DUE + item.INPROCESS_OVER_DUE;
                        allCompleted += item.COMPLETED_ONTIME + item.COMPLETED_OVER_DUE;

                        tempTotal.WF_TOTAL_INPROCESS_BY_WORKFLOW_STEP += item.INPROCESS_ONTIME + item.INPROCESS_ALMOST_DUE + item.INPROCESS_OVER_DUE;
                        tempTotal.WF_TOTAL_COMPLETE_BY_WORKFLOW_STEP += item.COMPLETED_ONTIME + item.COMPLETED_OVER_DUE;

                        tempTotal.WF_TOTAL += item.WF_TOTAL_INPROCESS_BY_WORKFLOW_STEP + item.WF_TOTAL_COMPLETE_BY_WORKFLOW_STEP;
                    }
                    res.data.Add(tempTotal);

                    tempTotal = new SUMMARY_REPORT_MODEL();
                    foreach (var item in res.data)
                    {
                        tempTotal.ORDERBY = 9;
                        tempTotal.WORKFLOW_TYPE = "Percentage (%)";

                        if (allInProcess > 0) tempTotal.INPROCESS_ONTIME = Math.Round(((double)item.INPROCESS_ONTIME / (double)allInProcess) * 100, 2);
                        if (allInProcess > 0) tempTotal.INPROCESS_ALMOST_DUE = Math.Round(((double)item.INPROCESS_ALMOST_DUE / (double)allInProcess) * 100, 2);
                        if (allInProcess > 0) tempTotal.INPROCESS_OVER_DUE = Math.Round(((double)item.INPROCESS_OVER_DUE / (double)allInProcess) * 100, 2);

                        if (allCompleted > 0) tempTotal.COMPLETED_ONTIME = Math.Round(((double)item.COMPLETED_ONTIME / (double)allCompleted) * 100, 2);
                        if (allCompleted > 0) tempTotal.COMPLETED_OVER_DUE = Math.Round(((double)item.COMPLETED_OVER_DUE / (double)allCompleted) * 100, 2);
                    }
                    res.data.Add(tempTotal);

                    Results = res;
                    Results.status = "S";
                    Results.data = res.data;
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        private static SUMMARY_REPORT_MODEL_RESULT mockup(ARTWORKEntities context, SUMMARY_REPORT_MODEL_REQUEST param)
        {
            var res = new SUMMARY_REPORT_MODEL_RESULT();
            res.data = new List<SUMMARY_REPORT_MODEL>();
            var getByCreateDateFrom = DateTime.MinValue;
            var getByCreateDateTo = DateTime.MaxValue;

            if (!string.IsNullOrEmpty(param.data.DATE_FROM))
                getByCreateDateFrom = CNService.ConvertStringToDate(param.data.DATE_FROM);
            if (!string.IsNullOrEmpty(param.data.DATE_TO))
                getByCreateDateTo = CNService.ConvertStringToDate(param.data.DATE_TO);

            IQueryable<V_ART_ENDTOEND_REPORT_3> q = null;
            var allMockup = new List<V_ART_ENDTOEND_REPORT_3>();

            var allMockupStep = ART_M_STEP_MOCKUP_SERVICE.GetAll(context).ToList();

            if (param.data.DEPARTMENT == "PK")
            {
                allMockupStep = allMockupStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_PG"
                      || m.STEP_MOCKUP_CODE == "SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN"
                      || m.STEP_MOCKUP_CODE == "SEND_PG_SUP_SEL_VENDOR"
                      || m.STEP_MOCKUP_CODE == "SEND_APP_MATCH_BOARD").ToList();

                var temp = allMockupStep.Select(m => m.STEP_MOCKUP_ID).ToList();

                q = (from m in context.V_ART_ENDTOEND_REPORT
                     where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"
                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                     select new V_ART_ENDTOEND_REPORT_3()
                     {
                         DUE_DATE = m.DUE_DATE,
                         WF_ID = m.WF_ID,
                         CURRENT_STEP_ID = m.CURRENT_STEP_ID.Value,
                         IS_END = m.IS_END,
                         IS_TERMINATE = m.IS_TERMINATE,
                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                         WF_TYPE = m.WF_TYPE,
                         STEP_END_DATE = m.STEP_END_DATE,
                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                     });
            }
            else if (param.data.DEPARTMENT == "MK")
            {
                allMockupStep = allMockupStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_MK_UPD_PACK_STYLE"
                || m.STEP_MOCKUP_CODE == "SEND_BACK_MK"
                || m.STEP_MOCKUP_CODE == "SEND_MK_APP").ToList();

                var temp = allMockupStep.Select(m => m.STEP_MOCKUP_ID).ToList();

                q = (from m in context.V_ART_ENDTOEND_REPORT
                     where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"
                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                     select new V_ART_ENDTOEND_REPORT_3()
                     {
                         DUE_DATE = m.DUE_DATE,
                         WF_ID = m.WF_ID,
                         CURRENT_STEP_ID = m.CURRENT_STEP_ID.Value,
                         IS_END = m.IS_END,
                         IS_TERMINATE = m.IS_TERMINATE,
                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                         WF_SUB_ID = m.WF_SUB_ID,
                         WF_TYPE = m.WF_TYPE,
                         STEP_END_DATE = m.STEP_END_DATE,
                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                     });
            }
            else if (param.data.DEPARTMENT == "QC")
            {
                allMockupStep = new List<ART_M_STEP_MOCKUP>();
            }
            else if (param.data.DEPARTMENT == "RD")
            {
                allMockupStep = allMockupStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_RD_PRI_PKG").ToList();

                var temp = allMockupStep.Select(m => m.STEP_MOCKUP_ID).ToList();

                q = (from m in context.V_ART_ENDTOEND_REPORT
                     where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"
                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                     select new V_ART_ENDTOEND_REPORT_3()
                     {
                         DUE_DATE = m.DUE_DATE,
                         WF_ID = m.WF_ID,
                         CURRENT_STEP_ID = m.CURRENT_STEP_ID.Value,
                         IS_END = m.IS_END,
                         IS_TERMINATE = m.IS_TERMINATE,
                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                         WF_SUB_ID = m.WF_SUB_ID,
                         WF_TYPE = m.WF_TYPE,
                         STEP_END_DATE = m.STEP_END_DATE,
                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                     });
            }
            else if (param.data.DEPARTMENT == "WH")
            {
                allMockupStep = allMockupStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_WH_TEST_PACK").ToList();

                var temp = allMockupStep.Select(m => m.STEP_MOCKUP_ID).ToList();

                q = (from m in context.V_ART_ENDTOEND_REPORT
                     where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"
                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                     select new V_ART_ENDTOEND_REPORT_3()
                     {
                         DUE_DATE = m.DUE_DATE,
                         WF_ID = m.WF_ID,
                         CURRENT_STEP_ID = m.CURRENT_STEP_ID.Value,
                         IS_END = m.IS_END,
                         IS_TERMINATE = m.IS_TERMINATE,
                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                         WF_SUB_ID = m.WF_SUB_ID,
                         WF_TYPE = m.WF_TYPE,
                         STEP_END_DATE = m.STEP_END_DATE,
                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                     });
            }
            else if (param.data.DEPARTMENT == "PN")
            {
                allMockupStep = allMockupStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_PN_PRI_PKG").ToList();

                var temp = allMockupStep.Select(m => m.STEP_MOCKUP_ID).ToList();

                q = (from m in context.V_ART_ENDTOEND_REPORT
                     where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"
                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                     select new V_ART_ENDTOEND_REPORT_3()
                     {
                         DUE_DATE = m.DUE_DATE,
                         WF_ID = m.WF_ID,
                         CURRENT_STEP_ID = m.CURRENT_STEP_ID.Value,
                         IS_END = m.IS_END,
                         IS_TERMINATE = m.IS_TERMINATE,
                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                         WF_SUB_ID = m.WF_SUB_ID,
                         WF_TYPE = m.WF_TYPE,
                         STEP_END_DATE = m.STEP_END_DATE,
                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                     });
            }
            else if (param.data.DEPARTMENT == "CUS")
            {
                allMockupStep = allMockupStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_CUS_APP").ToList();

                var temp = allMockupStep.Select(m => m.STEP_MOCKUP_ID).ToList();

                var tempSubId = from m in context.V_ART_ENDTOEND_REPORT
                                where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"
                                && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date
                                && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date
                                && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                group m by
                                new
                                {
                                    DbFunctions.CreateDateTime(m.STEP_CREATE_DATE.Year, m.STEP_CREATE_DATE.Month, m.STEP_CREATE_DATE.Day, m.STEP_CREATE_DATE.Hour, m.STEP_CREATE_DATE.Minute, 0).Value,
                                    m.CURRENT_STEP_ID,
                                    m.WF_ID
                                }
                                into g
                                select new V_ART_ENDTOEND_REPORT_3()
                                {
                                    WF_ID = (from t2 in g select t2.WF_ID).Min()
                                };
                var listSubId = tempSubId.Select(m => m.WF_ID).ToList();

                var guid = PerformBatchJoinWithIds(listSubId);

                q = (from m in context.V_ART_ENDTOEND_REPORT
                     where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"
                     //&& listSubId.Contains((int)m.WF_ID)
                     && (from mm in context.ART_TEMP_CONTAIN where mm.GUID == guid select mm.ID_TO_QUERY).Contains((int)m.WF_ID)
                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                     select new V_ART_ENDTOEND_REPORT_3()
                     {
                         DUE_DATE = m.DUE_DATE,
                         WF_ID = m.WF_ID,
                         CURRENT_STEP_ID = m.CURRENT_STEP_ID.Value,
                         IS_END = m.IS_END,
                         IS_TERMINATE = m.IS_TERMINATE,
                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                         WF_TYPE = m.WF_TYPE,
                         STEP_END_DATE = m.STEP_END_DATE,
                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                     });
            }
            else if (param.data.DEPARTMENT == "VN")
            {
                allMockupStep = allMockupStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_VN_MB"
                || m.STEP_MOCKUP_CODE == "SEND_VN_DL"
                || m.STEP_MOCKUP_CODE == "SEND_VN_RS"
                || m.STEP_MOCKUP_CODE == "SEND_VN_PR"
                || m.STEP_MOCKUP_CODE == "SEND_VN_QUO").ToList();

                var temp = allMockupStep.Select(m => m.STEP_MOCKUP_ID).ToList();

                var tempSubId = from m in context.V_ART_ENDTOEND_REPORT
                                where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"
                                && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date
                                && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date
                                && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                group m by
                                new
                                {
                                    DbFunctions.CreateDateTime(m.STEP_CREATE_DATE.Year, m.STEP_CREATE_DATE.Month, m.STEP_CREATE_DATE.Day, m.STEP_CREATE_DATE.Hour, m.STEP_CREATE_DATE.Minute, 0).Value,
                                    m.CURRENT_STEP_ID,
                                    m.WF_ID
                                }
                              into g
                                select new V_ART_ENDTOEND_REPORT_3()
                                {
                                    WF_ID = (from t2 in g select t2.WF_ID).Min()
                                };
                var listSubId = tempSubId.Select(m => m.WF_ID).ToList();

                var guid = PerformBatchJoinWithIds(listSubId);

                q = (from m in context.V_ART_ENDTOEND_REPORT
                     where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"
                     //&& listSubId.Contains((int)m.WF_ID)
                     && (from mm in context.ART_TEMP_CONTAIN where mm.GUID == guid select mm.ID_TO_QUERY).Contains((int)m.WF_ID)
                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                     select new V_ART_ENDTOEND_REPORT_3()
                     {
                         DUE_DATE = m.DUE_DATE,
                         WF_ID = m.WF_ID,
                         CURRENT_STEP_ID = m.CURRENT_STEP_ID.Value,
                         IS_END = m.IS_END,
                         IS_TERMINATE = m.IS_TERMINATE,
                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                         WF_TYPE = m.WF_TYPE,
                         STEP_END_DATE = m.STEP_END_DATE,
                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                     }).Distinct();
            }

            if (q != null)
            {
                if (!string.IsNullOrEmpty(param.data.DATE_FROM))
                    q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date);
                if (!string.IsNullOrEmpty(param.data.DATE_TO))
                    q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date);

                allMockup = q.Distinct().ToList();
            }

            foreach (var item in allMockupStep)
            {
                var temp = new SUMMARY_REPORT_MODEL();
                temp.STEP_DISPLAY_TEXT = item.STEP_MOCKUP_NAME;
                temp.WORKFLOW_TYPE = "Mockup";
                temp.DEPARTMENT2 = param.data.DEPARTMENT;
                temp.CURRENT_STEP_TXT = "M" + item.STEP_MOCKUP_ID;

                var inprocess = new List<V_ART_ENDTOEND_REPORT_3>();
                var completed = new List<V_ART_ENDTOEND_REPORT_3>();

                if (param.data.DEPARTMENT == "CUS" || param.data.DEPARTMENT == "VN")
                {
                    var tempAllMockupInprocess = allMockup.Where(m => m.CURRENT_STEP_ID == item.STEP_MOCKUP_ID && string.IsNullOrEmpty(m.IS_END));
                    foreach (var s in tempAllMockupInprocess)
                    {
                        if (allMockup.Where(m => m.WF_ID == s.WF_ID && string.IsNullOrEmpty(m.IS_END) && m.CURRENT_STEP_ID == item.STEP_MOCKUP_ID).Count() > 0)
                        {
                            if (inprocess.Where(m => m.WF_ID == s.WF_ID && m.CURRENT_STEP_ID == s.CURRENT_STEP_ID &&
    new DateTime(m.STEP_CREATE_DATE.Year, m.STEP_CREATE_DATE.Month, m.STEP_CREATE_DATE.Day, m.STEP_CREATE_DATE.Hour, m.STEP_CREATE_DATE.Minute, 0)
    == new DateTime(s.STEP_CREATE_DATE.Year, s.STEP_CREATE_DATE.Month, s.STEP_CREATE_DATE.Day, s.STEP_CREATE_DATE.Hour, s.STEP_CREATE_DATE.Minute, 0))
.Count() == 0)
                            {
                                inprocess.Add(s);
                            }
                        }
                    }

                    var tempAllMockupCompleted = allMockup.Where(m => m.CURRENT_STEP_ID == item.STEP_MOCKUP_ID && !string.IsNullOrEmpty(m.IS_END) && string.IsNullOrEmpty(m.IS_TERMINATE));
                    foreach (var s in tempAllMockupCompleted)
                    {
                        if (allMockup.Where(m => m.WF_ID == s.WF_ID && !string.IsNullOrEmpty(m.IS_END) && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS) && m.CURRENT_STEP_ID == item.STEP_MOCKUP_ID).Count() > 0)
                        {
                            if (completed.Where(m => m.WF_ID == s.WF_ID && m.CURRENT_STEP_ID == s.CURRENT_STEP_ID &&
    new DateTime(m.STEP_CREATE_DATE.Year, m.STEP_CREATE_DATE.Month, m.STEP_CREATE_DATE.Day, m.STEP_CREATE_DATE.Hour, m.STEP_CREATE_DATE.Minute, 0)
    == new DateTime(s.STEP_CREATE_DATE.Year, s.STEP_CREATE_DATE.Month, s.STEP_CREATE_DATE.Day, s.STEP_CREATE_DATE.Hour, s.STEP_CREATE_DATE.Minute, 0))
.Count() == 0)
                            {
                                completed.Add(s);
                            }
                        }
                    }
                }
                else
                {
                    inprocess = allMockup.Where(m => string.IsNullOrEmpty(m.IS_END) && m.CURRENT_STEP_ID == item.STEP_MOCKUP_ID).ToList();
                    completed = allMockup.Where(m => !string.IsNullOrEmpty(m.IS_END) && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS) && m.CURRENT_STEP_ID == item.STEP_MOCKUP_ID).ToList();
                }

                foreach (var item2 in inprocess)
                {
                    var dtWillFinish = item2.DUE_DATE;
                    if (DateTime.Now.AddDays(1).Date == dtWillFinish.Value.Date)
                    {
                        temp.INPROCESS_ALMOST_DUE++;
                    }
                    else if (DateTime.Now > dtWillFinish)
                    {
                        temp.INPROCESS_OVER_DUE++;
                    }
                    else
                    {
                        temp.INPROCESS_ONTIME++;
                    }
                }
                foreach (var item2 in completed)
                {
                    var dtWillFinish = item2.DUE_DATE;
                    if (item2.STEP_END_DATE > dtWillFinish)
                    {
                        temp.COMPLETED_OVER_DUE++;
                    }
                    else
                    {
                        temp.COMPLETED_ONTIME++;
                    }
                }
                res.data.Add(temp);
            }

            return res;
        }

        private static SUMMARY_REPORT_MODEL_RESULT artwork(ARTWORKEntities context, SUMMARY_REPORT_MODEL_REQUEST param)
        {
            var res = new SUMMARY_REPORT_MODEL_RESULT();
            res.data = new List<SUMMARY_REPORT_MODEL>();
            var getByCreateDateFrom = DateTime.MinValue;
            var getByCreateDateTo = DateTime.MaxValue;

            if (!string.IsNullOrEmpty(param.data.DATE_FROM))
                getByCreateDateFrom = CNService.ConvertStringToDate(param.data.DATE_FROM);
            if (!string.IsNullOrEmpty(param.data.DATE_TO))
                getByCreateDateTo = CNService.ConvertStringToDate(param.data.DATE_TO);

            IQueryable<V_ART_ENDTOEND_REPORT_3> q = null;
            var allArtwork = new List<V_ART_ENDTOEND_REPORT_3>();
            var allArtworkStep = ART_M_STEP_ARTWORK_SERVICE.GetAll(context).ToList();

            if (param.data.DEPARTMENT == "PK")
            {
                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_PA"
                || m.STEP_ARTWORK_CODE == "SEND_PG"
                || m.STEP_ARTWORK_CODE == "SEND_PP").ToList();

                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();

                q = (from m in context.V_ART_ENDTOEND_REPORT
                     where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork"
                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                     select new V_ART_ENDTOEND_REPORT_3()
                     {
                         DUE_DATE = m.DUE_DATE,
                         WF_ID = m.WF_ID,
                         WF_SUB_ID = m.WF_SUB_ID,
                         CURRENT_STEP_ID = m.CURRENT_STEP_ID.Value,
                         IS_END = m.IS_END,
                         IS_TERMINATE = m.IS_TERMINATE,
                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                         STEP_END_DATE = m.STEP_END_DATE,
                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                     });
            }
            else if (param.data.DEPARTMENT == "MK")
            {
                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_MK_VERIFY"
                || m.STEP_ARTWORK_CODE == "SEND_GM_MK"
                || m.STEP_ARTWORK_CODE == "SEND_BACK_MK"
                || m.STEP_ARTWORK_CODE == "SEND_MK").ToList();

                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();

                q = (from m in context.V_ART_ENDTOEND_REPORT
                     where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork"
                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                     select new V_ART_ENDTOEND_REPORT_3()
                     {
                         DUE_DATE = m.DUE_DATE,
                         WF_ID = m.WF_ID,
                         WF_SUB_ID = m.WF_SUB_ID,
                         CURRENT_STEP_ID = m.CURRENT_STEP_ID.Value,
                         IS_END = m.IS_END,
                         IS_TERMINATE = m.IS_TERMINATE,
                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                         STEP_END_DATE = m.STEP_END_DATE,
                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                     });
            }
            else if (param.data.DEPARTMENT == "QC")
            {
                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_QC"
                || m.STEP_ARTWORK_CODE == "SEND_GM_QC"
                || m.STEP_ARTWORK_CODE == "SEND_QC_VERIFY").ToList();

                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();

                q = (from m in context.V_ART_ENDTOEND_REPORT
                     where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork"
                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                     select new V_ART_ENDTOEND_REPORT_3()
                     {
                         DUE_DATE = m.DUE_DATE,
                         WF_ID = m.WF_ID,
                         WF_SUB_ID = m.WF_SUB_ID,
                         CURRENT_STEP_ID = m.CURRENT_STEP_ID.Value,
                         IS_END = m.IS_END,
                         IS_TERMINATE = m.IS_TERMINATE,
                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                         STEP_END_DATE = m.STEP_END_DATE,
                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                     });
            }
            else if (param.data.DEPARTMENT == "RD")
            {
                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_RD").ToList();

                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();

                q = (from m in context.V_ART_ENDTOEND_REPORT
                     where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork"
                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                     select new V_ART_ENDTOEND_REPORT_3()
                     {
                         DUE_DATE = m.DUE_DATE,
                         WF_ID = m.WF_ID,
                         WF_SUB_ID = m.WF_SUB_ID,
                         CURRENT_STEP_ID = m.CURRENT_STEP_ID.Value,
                         IS_END = m.IS_END,
                         IS_TERMINATE = m.IS_TERMINATE,
                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                         STEP_END_DATE = m.STEP_END_DATE,
                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                     });
            }
            else if (param.data.DEPARTMENT == "WH")
            {
                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_WH").ToList();

                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();

                q = (from m in context.V_ART_ENDTOEND_REPORT
                     where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork"
                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                     select new V_ART_ENDTOEND_REPORT_3()
                     {
                         DUE_DATE = m.DUE_DATE,
                         WF_ID = m.WF_ID,
                         WF_SUB_ID = m.WF_SUB_ID,
                         CURRENT_STEP_ID = m.CURRENT_STEP_ID.Value,
                         IS_END = m.IS_END,
                         IS_TERMINATE = m.IS_TERMINATE,
                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                         STEP_END_DATE = m.STEP_END_DATE,
                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                     });
            }
            else if (param.data.DEPARTMENT == "PN")
            {
                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_PN").ToList();

                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();

                q = (from m in context.V_ART_ENDTOEND_REPORT
                     where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork"
                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                     select new V_ART_ENDTOEND_REPORT_3()
                     {
                         DUE_DATE = m.DUE_DATE,
                         WF_ID = m.WF_ID,
                         WF_SUB_ID = m.WF_SUB_ID,
                         CURRENT_STEP_ID = m.CURRENT_STEP_ID.Value,
                         IS_END = m.IS_END,
                         IS_TERMINATE = m.IS_TERMINATE,
                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                         STEP_END_DATE = m.STEP_END_DATE,
                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                     });
            }
            else if (param.data.DEPARTMENT == "CUS")
            {
                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_CUS_REVIEW"
                                || m.STEP_ARTWORK_CODE == "SEND_CUS_PRINT"
                                || m.STEP_ARTWORK_CODE == "SEND_CUS_SHADE"
                                || m.STEP_ARTWORK_CODE == "SEND_CUS_REF"
                                || m.STEP_ARTWORK_CODE == "SEND_CUS_REQ_REF").ToList();

                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();

                var tempSubId = from m in context.V_ART_ENDTOEND_REPORT
                                where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork"
                                && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date
                                && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date
                                && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                group m by
                                new
                                {
                                    DbFunctions.CreateDateTime(m.STEP_CREATE_DATE.Year, m.STEP_CREATE_DATE.Month, m.STEP_CREATE_DATE.Day, m.STEP_CREATE_DATE.Hour, m.STEP_CREATE_DATE.Minute, 0).Value,
                                    m.CURRENT_STEP_ID,
                                    m.WF_ID
                                }
                                into g
                                select new V_ART_ENDTOEND_REPORT_3()
                                {
                                    WF_ID = (from t2 in g select t2.WF_ID).Min()
                                };
                var listSubId = tempSubId.Select(m => m.WF_ID).ToList();

                var guid = PerformBatchJoinWithIds(listSubId);

                q = (from m in context.V_ART_ENDTOEND_REPORT
                     where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork"
                     //&& listSubId.Contains((int)m.WF_ID)
                     && (from mm in context.ART_TEMP_CONTAIN where mm.GUID == guid select mm.ID_TO_QUERY).Contains((int)m.WF_ID)
                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                     select new V_ART_ENDTOEND_REPORT_3()
                     {
                         DUE_DATE = m.DUE_DATE,
                         WF_ID = m.WF_ID,
                         CURRENT_STEP_ID = m.CURRENT_STEP_ID.Value,
                         IS_END = m.IS_END,
                         IS_TERMINATE = m.IS_TERMINATE,
                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                         WF_TYPE = m.WF_TYPE,
                         STEP_END_DATE = m.STEP_END_DATE,
                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                     });
            }
            else if (param.data.DEPARTMENT == "VN")
            {
                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_VN_PO"
                       || m.STEP_ARTWORK_CODE == "SEND_VN_SL"
                       || m.STEP_ARTWORK_CODE == "SEND_VN_PM").ToList();

                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();

                var tempSubId = from m in context.V_ART_ENDTOEND_REPORT
                                where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork"
                                && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date
                                && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date
                                && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                group m by
                                new
                                {
                                    DbFunctions.CreateDateTime(m.STEP_CREATE_DATE.Year, m.STEP_CREATE_DATE.Month, m.STEP_CREATE_DATE.Day, m.STEP_CREATE_DATE.Hour, m.STEP_CREATE_DATE.Minute, 0).Value,
                                    m.CURRENT_STEP_ID,
                                    m.WF_ID
                                }
                                into g
                                select new V_ART_ENDTOEND_REPORT_3()
                                {
                                    WF_ID = (from t2 in g select t2.WF_ID).Min()
                                };
                var listSubId = tempSubId.Select(m => m.WF_ID).ToList();

                var guid = PerformBatchJoinWithIds(listSubId);

                q = (from m in context.V_ART_ENDTOEND_REPORT
                     where temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork"
                     //&& listSubId.Contains((int)m.WF_ID)
                     && (from mm in context.ART_TEMP_CONTAIN where mm.GUID == guid select mm.ID_TO_QUERY).Contains((int)m.WF_ID)
                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                     select new V_ART_ENDTOEND_REPORT_3()
                     {
                         DUE_DATE = m.DUE_DATE,
                         WF_ID = m.WF_ID,
                         CURRENT_STEP_ID = m.CURRENT_STEP_ID.Value,
                         IS_END = m.IS_END,
                         IS_TERMINATE = m.IS_TERMINATE,
                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                         WF_TYPE = m.WF_TYPE,
                         STEP_END_DATE = m.STEP_END_DATE,
                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                     }).Distinct();
            }

            if (q != null)
            {
                if (!string.IsNullOrEmpty(param.data.DATE_FROM))
                    q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date);
                if (!string.IsNullOrEmpty(param.data.DATE_TO))
                    q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date);

                allArtwork = q.Distinct().ToList();
            }

            foreach (var item in allArtworkStep)
            {
                var temp = new SUMMARY_REPORT_MODEL();
                temp.STEP_DISPLAY_TEXT = item.STEP_ARTWORK_NAME;
                temp.WORKFLOW_TYPE = "Artwork";
                temp.DEPARTMENT2 = param.data.DEPARTMENT;
                temp.CURRENT_STEP_TXT = "A" + item.STEP_ARTWORK_ID;

                //var inprocess = allArtwork.Where(m => string.IsNullOrEmpty(m.IS_END) && m.CURRENT_STEP_ID == item.STEP_ARTWORK_ID).ToList();
                //var completed = allArtwork.Where(m => !string.IsNullOrEmpty(m.IS_END) && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS) && m.CURRENT_STEP_ID == item.STEP_ARTWORK_ID).ToList();
                var inprocess = new List<V_ART_ENDTOEND_REPORT_3>();
                var completed = new List<V_ART_ENDTOEND_REPORT_3>();
                if (param.data.DEPARTMENT == "CUS" || param.data.DEPARTMENT == "VN")
                {
                    var tempallArtworkInprocess = allArtwork.Where(m => m.CURRENT_STEP_ID == item.STEP_ARTWORK_ID && string.IsNullOrEmpty(m.IS_END));
                    foreach (var s in tempallArtworkInprocess)
                    {
                        if (allArtwork.Where(m => m.WF_ID == s.WF_ID && string.IsNullOrEmpty(m.IS_END) && m.CURRENT_STEP_ID == item.STEP_ARTWORK_ID).Count() > 0)
                        {
                            if (inprocess.Where(m => m.WF_ID == s.WF_ID && m.CURRENT_STEP_ID == s.CURRENT_STEP_ID &&
    new DateTime(m.STEP_CREATE_DATE.Year, m.STEP_CREATE_DATE.Month, m.STEP_CREATE_DATE.Day, m.STEP_CREATE_DATE.Hour, m.STEP_CREATE_DATE.Minute, 0)
    == new DateTime(s.STEP_CREATE_DATE.Year, s.STEP_CREATE_DATE.Month, s.STEP_CREATE_DATE.Day, s.STEP_CREATE_DATE.Hour, s.STEP_CREATE_DATE.Minute, 0))
.Count() == 0)
                            {
                                inprocess.Add(s);
                            }
                        }
                    }

                    var tempallArtworkCompleted = allArtwork.Where(m => m.CURRENT_STEP_ID == item.STEP_ARTWORK_ID && !string.IsNullOrEmpty(m.IS_END) && string.IsNullOrEmpty(m.IS_TERMINATE));
                    foreach (var s in tempallArtworkCompleted)
                    {
                        if (allArtwork.Where(m => m.WF_ID == s.WF_ID && !string.IsNullOrEmpty(m.IS_END) && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS) && m.CURRENT_STEP_ID == item.STEP_ARTWORK_ID).Count() > 0)
                        {
                            if (completed.Where(m => m.WF_ID == s.WF_ID && m.CURRENT_STEP_ID == s.CURRENT_STEP_ID &&
    new DateTime(m.STEP_CREATE_DATE.Year, m.STEP_CREATE_DATE.Month, m.STEP_CREATE_DATE.Day, m.STEP_CREATE_DATE.Hour, m.STEP_CREATE_DATE.Minute, 0)
    == new DateTime(s.STEP_CREATE_DATE.Year, s.STEP_CREATE_DATE.Month, s.STEP_CREATE_DATE.Day, s.STEP_CREATE_DATE.Hour, s.STEP_CREATE_DATE.Minute, 0))
.Count() == 0)
                            {
                                completed.Add(s);
                            }
                        }
                    }
                }
                else
                {
                    inprocess = allArtwork.Where(m => string.IsNullOrEmpty(m.IS_END) && m.CURRENT_STEP_ID == item.STEP_ARTWORK_ID).ToList();
                    completed = allArtwork.Where(m => !string.IsNullOrEmpty(m.IS_END) && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS) && m.CURRENT_STEP_ID == item.STEP_ARTWORK_ID).ToList();
                }

                foreach (var item2 in inprocess)
                {
                    var dtWillFinish = item2.DUE_DATE;
                    if (DateTime.Now.AddDays(1).Date == dtWillFinish.Value.Date)
                    {
                        temp.INPROCESS_ALMOST_DUE++;
                    }
                    else if (DateTime.Now > dtWillFinish)
                    {
                        temp.INPROCESS_OVER_DUE++;
                    }
                    else
                    {
                        temp.INPROCESS_ONTIME++;
                    }
                }
                foreach (var item2 in completed)
                {
                    var dtWillFinish = item2.DUE_DATE;
                    if (item2.STEP_END_DATE > dtWillFinish)
                    {
                        temp.COMPLETED_OVER_DUE++;
                    }
                    else
                    {
                        temp.COMPLETED_ONTIME++;
                    }
                }
                res.data.Add(temp);
            }

            return res;
        }

        public static SUMMARY_REPORT_MODEL_RESULT GetSummaryReportDataByWorkflowType(SUMMARY_REPORT_MODEL_REQUEST param)
        {
            var Results = new SUMMARY_REPORT_MODEL_RESULT();

            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<SUMMARY_REPORT_MODEL>();
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                var getByCreateDateFrom = DateTime.MinValue;
                var getByCreateDateTo = DateTime.MaxValue;

                if (!string.IsNullOrEmpty(param.data.DATE_FROM))
                    getByCreateDateFrom = CNService.ConvertStringToDate(param.data.DATE_FROM);
                if (!string.IsNullOrEmpty(param.data.DATE_TO))
                    getByCreateDateTo = CNService.ConvertStringToDate(param.data.DATE_TO);

                var res = new SUMMARY_REPORT_MODEL_RESULT();
                res.data = new List<SUMMARY_REPORT_MODEL>();

                ART_M_STEP_ARTWORK SEND_VN_PO = new ART_M_STEP_ARTWORK();
                ART_M_STEP_ARTWORK SEND_PP = new ART_M_STEP_ARTWORK();
                ART_M_STEP_ARTWORK SEND_PA = new ART_M_STEP_ARTWORK();
                ART_M_STEP_ARTWORK SEND_CUS_PRINT = new ART_M_STEP_ARTWORK();
                ART_M_STEP_MOCKUP SEND_PG = new ART_M_STEP_MOCKUP();
                ART_M_STEP_MOCKUP SEND_CUS_APP = new ART_M_STEP_MOCKUP();
                ART_M_STEP_MOCKUP SEND_RD_PRI_PKG = new ART_M_STEP_MOCKUP();
                ART_M_STEP_MOCKUP SEND_PN_PRI_PKG = new ART_M_STEP_MOCKUP();

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 1800;

                        SEND_CUS_PRINT = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_PRINT" }, context).FirstOrDefault();
                        SEND_PP = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PP" }, context).FirstOrDefault();
                        SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault();
                        SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault();
                        SEND_CUS_APP = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_CUS_APP" }, context).FirstOrDefault();
                        SEND_RD_PRI_PKG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_RD_PRI_PKG" }, context).FirstOrDefault();
                        SEND_PN_PRI_PKG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PN_PRI_PKG" }, context).FirstOrDefault();
                        SEND_VN_PO = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_VN_PO" }, context).FirstOrDefault();
                        var temp = new SUMMARY_REPORT_MODEL();

                        if (param.data.WORKFLOW_TYPE == "ALL" || param.data.WORKFLOW_TYPE == "ARTNEW" || param.data.WORKFLOW_TYPE == "ART")
                        {
                            var q = (from m in context.V_ART_ENDTOEND_REPORT
                                     where m.WF_NO.StartsWith("AW-N-")
                                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                     select new V_ART_ENDTOEND_REPORT_3()
                                     {
                                         WF_ID = m.WF_ID,
                                         WF_SUB_ID = m.WF_SUB_ID,
                                         WF_TYPE = m.WF_TYPE,
                                         DUE_DATE = m.DUE_DATE,
                                         IS_END = m.IS_END,
                                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                                         IS_TERMINATE = m.IS_TERMINATE,
                                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                                         STEP_END_DATE = m.STEP_END_DATE,
                                         CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                                     });

                            if (!string.IsNullOrEmpty(param.data.DATE_FROM))
                                q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date);
                            if (!string.IsNullOrEmpty(param.data.DATE_TO))
                                q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date);

                            var artworkNew = q.ToList();

                            var mainProcess = artworkNew.Where(m => m.CURRENT_STEP_ID == SEND_PA.STEP_ARTWORK_ID).ToList();
                            var customerProcess = artworkNew.Where(m => m.CURRENT_STEP_ID == SEND_CUS_PRINT.STEP_ARTWORK_ID).ToList();

                            temp = new SUMMARY_REPORT_MODEL();
                            foreach (var item in mainProcess)
                            {
                                var fixArtwork = 6;
                                var lastSendToCustomer = customerProcess.Where(m => m.WF_ID == item.WF_ID).OrderByDescending(m => m.STEP_CREATE_DATE).FirstOrDefault();
                                if (lastSendToCustomer != null)
                                {
                                    if (string.IsNullOrEmpty(item.IS_TERMINATE) && string.IsNullOrEmpty(item.REMARK_KILLPROCESS))
                                    {
                                        if (CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixArtwork) >= lastSendToCustomer.STEP_CREATE_DATE)
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_ONTIME += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_ONTIME++;
                                        }
                                        else
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_OVERDUE += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_OVER_DUE++;
                                        }
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(item.IS_END))
                                    {
                                        if (DateTime.Now.AddDays(1).Date == CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixArtwork).Date)
                                        {
                                            temp.LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE += "," + item.WF_SUB_ID;
                                            temp.INPROCESS_ALMOST_DUE++;
                                        }
                                        else if (DateTime.Now > CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixArtwork))
                                        {
                                            temp.LIST_WF_SUB_ID_INPROCESS_OVERDUE += "," + item.WF_SUB_ID;
                                            temp.INPROCESS_OVER_DUE++;
                                        }
                                        else
                                        {
                                            temp.LIST_WF_SUB_ID_INPROCESS_ONTIME += "," + item.WF_SUB_ID;
                                            temp.INPROCESS_ONTIME++;
                                        }
                                    }
                                    else if (string.IsNullOrEmpty(item.IS_TERMINATE) && string.IsNullOrEmpty(item.REMARK_KILLPROCESS))
                                    {
                                        if (item.STEP_END_DATE <= CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixArtwork))
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_ONTIME += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_ONTIME++;
                                        }
                                        else
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_OVERDUE += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_OVER_DUE++;
                                        }
                                    }
                                }
                            }
                            temp.WORKFLOW_TYPE = "Artwork new";
                            temp.CNT_WF = mainProcess.Count();

                            res.data.Add(temp);
                        }

                        if (param.data.WORKFLOW_TYPE == "ALL" || param.data.WORKFLOW_TYPE == "ARTREPEAT" || param.data.WORKFLOW_TYPE == "ART")
                        {
                            var q = (from m in context.V_ART_ENDTOEND_REPORT
                                     where m.WF_NO.StartsWith("AW-R-")
                                         && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                     select new V_ART_ENDTOEND_REPORT_3()
                                     {
                                         WF_ID = m.WF_ID,
                                         WF_SUB_ID = m.WF_SUB_ID,
                                         WF_TYPE = m.WF_TYPE,
                                         DUE_DATE = m.DUE_DATE,
                                         IS_END = m.IS_END,
                                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                                         IS_TERMINATE = m.IS_TERMINATE,
                                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                                         STEP_END_DATE = m.STEP_END_DATE,
                                         CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                                     });

                            if (!string.IsNullOrEmpty(param.data.DATE_FROM))
                                q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date);
                            if (!string.IsNullOrEmpty(param.data.DATE_TO))
                                q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date);

                            var artworkRepeat = q.Distinct().ToList();

                            var mainProcess = artworkRepeat.Where(m => m.CURRENT_STEP_ID == SEND_PA.STEP_ARTWORK_ID).ToList();
                            var ppProcess = artworkRepeat.Where(m => m.CURRENT_STEP_ID == SEND_PP.STEP_ARTWORK_ID).ToList();
                            var vendorProcess = artworkRepeat.Where(m => m.CURRENT_STEP_ID == SEND_VN_PO.STEP_ARTWORK_ID).ToList();

                            //var cntNoPPButComplete = 0;
                            temp = new SUMMARY_REPORT_MODEL();
                            foreach (var item in mainProcess)
                            {
                                temp.LIST_WF_SUB_ID_ALL += "," + item.WF_SUB_ID;

                                var fixArtworkRepeat = 2;
                                var lastSendToPP = ppProcess.Where(m => m.WF_ID == item.WF_ID).OrderByDescending(m => m.STEP_CREATE_DATE).FirstOrDefault();
                                if (lastSendToPP != null)
                                {
                                    var lastVendorProcess = vendorProcess.Where(m => m.WF_ID == item.WF_ID).OrderByDescending(m => m.STEP_CREATE_DATE).FirstOrDefault();
                                    if (lastVendorProcess != null)
                                    {
                                        if (string.IsNullOrEmpty(item.IS_TERMINATE) && string.IsNullOrEmpty(item.REMARK_KILLPROCESS))
                                        {
                                            if (CNService.AddBusinessDays(lastSendToPP.STEP_CREATE_DATE, fixArtworkRepeat) >= lastVendorProcess.STEP_CREATE_DATE)
                                            {
                                                temp.LIST_WF_SUB_ID_COMPLETED_ONTIME += "," + item.WF_SUB_ID;
                                                temp.COMPLETED_ONTIME++;
                                            }
                                            else
                                            {
                                                temp.LIST_WF_SUB_ID_COMPLETED_OVERDUE += "," + item.WF_SUB_ID;
                                                temp.COMPLETED_OVER_DUE++;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(item.IS_END))
                                        {
                                            if (DateTime.Now.AddDays(1).Date == CNService.AddBusinessDays(lastSendToPP.STEP_CREATE_DATE, fixArtworkRepeat).Date)
                                            {
                                                temp.LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE += "," + item.WF_SUB_ID;
                                                temp.INPROCESS_ALMOST_DUE++;
                                            }
                                            else if (DateTime.Now > CNService.AddBusinessDays(lastSendToPP.STEP_CREATE_DATE, fixArtworkRepeat))
                                            {
                                                temp.LIST_WF_SUB_ID_INPROCESS_OVERDUE += "," + item.WF_SUB_ID;
                                                temp.INPROCESS_OVER_DUE++;
                                            }
                                            else
                                            {
                                                temp.LIST_WF_SUB_ID_INPROCESS_ONTIME += "," + item.WF_SUB_ID;
                                                temp.INPROCESS_ONTIME++;
                                            }
                                        }
                                        //else if (string.IsNullOrEmpty(item.IS_TERMINATE) && string.IsNullOrEmpty(item.REMARK_KILLPROCESS))
                                        //{
                                        //temp.LIST_WF_SUB_ID_INPROCESS_ONTIME += "," + item.WF_SUB_ID;
                                        //temp.INPROCESS_ONTIME++;
                                        //}
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(item.IS_END))
                                    {
                                        temp.LIST_WF_SUB_ID_INPROCESS_ONTIME += "," + item.WF_SUB_ID;
                                        temp.INPROCESS_ONTIME++;
                                    }
                                    else
                                    {
                                        //cntNoPPButComplete++;
                                    }
                                    //else
                                    //{
                                    //    temp.LIST_WF_SUB_ID_COMPLETED_ONTIME += "," + item.WF_SUB_ID;
                                    //    temp.COMPLETED_ONTIME++;
                                    //}
                                    //if (string.IsNullOrEmpty(item.IS_END))
                                    //{
                                    //    if (DateTime.Now.AddDays(1).Date == CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixArtworkRepeat).Date)
                                    //    {
                                    //        temp.LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE += "," + item.WF_SUB_ID;
                                    //        temp.INPROCESS_ALMOST_DUE++;
                                    //    }
                                    //    else if (DateTime.Now > CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixArtworkRepeat))
                                    //    {
                                    //        temp.LIST_WF_SUB_ID_INPROCESS_OVERDUE += "," + item.WF_SUB_ID;
                                    //        temp.INPROCESS_OVER_DUE++;
                                    //    }
                                    //    else
                                    //    {
                                    //        temp.LIST_WF_SUB_ID_INPROCESS_ONTIME += "," + item.WF_SUB_ID;
                                    //        temp.INPROCESS_ONTIME++;
                                    //    }
                                    //}
                                    //else if (string.IsNullOrEmpty(item.IS_TERMINATE) && string.IsNullOrEmpty(item.REMARK_KILLPROCESS))
                                    //{
                                    //    if (item.STEP_END_DATE <= CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixArtworkRepeat))
                                    //    {
                                    //        temp.LIST_WF_SUB_ID_COMPLETED_ONTIME += "," + item.WF_SUB_ID;
                                    //        temp.COMPLETED_ONTIME++;
                                    //    }
                                    //    else
                                    //    {
                                    //        temp.LIST_WF_SUB_ID_COMPLETED_OVERDUE += "," + item.WF_SUB_ID;
                                    //        temp.COMPLETED_OVER_DUE++;
                                    //    }
                                    //}
                                }
                            }
                            temp.WORKFLOW_TYPE = "Artwork repeat";
                            temp.CNT_WF = mainProcess.Count();// - cntNoPPButComplete;

                            res.data.Add(temp);
                        }

                        if (param.data.WORKFLOW_TYPE == "ALL" || param.data.WORKFLOW_TYPE == "ARTR6" || param.data.WORKFLOW_TYPE == "ART")
                        {
                            var q = (from m in context.V_ART_ENDTOEND_REPORT
                                     where m.WF_NO.StartsWith("AW-R6-")
                                      && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                     select new V_ART_ENDTOEND_REPORT_3()
                                     {
                                         WF_ID = m.WF_ID,
                                         WF_SUB_ID = m.WF_SUB_ID,
                                         WF_TYPE = m.WF_TYPE,
                                         DUE_DATE = m.DUE_DATE,
                                         IS_END = m.IS_END,
                                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                                         IS_TERMINATE = m.IS_TERMINATE,
                                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                                         STEP_END_DATE = m.STEP_END_DATE,
                                         CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                                     });

                            if (!string.IsNullOrEmpty(param.data.DATE_FROM))
                                q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date);
                            if (!string.IsNullOrEmpty(param.data.DATE_TO))
                                q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date);

                            var artworkRepeatR6 = q.Distinct().ToList();

                            var mainProcess = artworkRepeatR6.Where(m => m.CURRENT_STEP_ID == SEND_PA.STEP_ARTWORK_ID).ToList();
                            var customerProcess = artworkRepeatR6.Where(m => m.CURRENT_STEP_ID == SEND_CUS_PRINT.STEP_ARTWORK_ID).ToList();

                            temp = new SUMMARY_REPORT_MODEL();
                            foreach (var item in mainProcess)
                            {
                                var fixArtworkR6 = 6;
                                var lastSendToCustomer = customerProcess.Where(m => m.WF_ID == item.WF_ID).OrderByDescending(m => m.STEP_CREATE_DATE).FirstOrDefault();
                                if (lastSendToCustomer != null)
                                {
                                    if (string.IsNullOrEmpty(item.IS_TERMINATE) && string.IsNullOrEmpty(item.REMARK_KILLPROCESS))
                                    {
                                        if (CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixArtworkR6) >= lastSendToCustomer.STEP_CREATE_DATE)
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_ONTIME += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_ONTIME++;
                                        }
                                        else
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_OVERDUE += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_OVER_DUE++;
                                        }
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(item.IS_END))
                                    {
                                        if (DateTime.Now.AddDays(1).Date == CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixArtworkR6).Date)
                                        {
                                            temp.LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE += "," + item.WF_SUB_ID;
                                            temp.INPROCESS_ALMOST_DUE++;
                                        }
                                        else if (DateTime.Now > CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixArtworkR6))
                                        {
                                            temp.LIST_WF_SUB_ID_INPROCESS_OVERDUE += "," + item.WF_SUB_ID;
                                            temp.INPROCESS_OVER_DUE++;
                                        }
                                        else
                                        {
                                            temp.LIST_WF_SUB_ID_INPROCESS_ONTIME += "," + item.WF_SUB_ID;
                                            temp.INPROCESS_ONTIME++;
                                        }
                                    }
                                    else if (string.IsNullOrEmpty(item.IS_TERMINATE) && string.IsNullOrEmpty(item.REMARK_KILLPROCESS))
                                    {
                                        if (item.STEP_END_DATE <= CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixArtworkR6))
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_ONTIME += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_ONTIME++;
                                        }
                                        else
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_OVERDUE += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_OVER_DUE++;
                                        }
                                    }
                                }
                            }
                            temp.WORKFLOW_TYPE = "Artwork repeat R6";
                            temp.CNT_WF = mainProcess.Count();

                            res.data.Add(temp);
                        }

                        if (param.data.WORKFLOW_TYPE == "ALL" || param.data.WORKFLOW_TYPE == "MONORMAL" || param.data.WORKFLOW_TYPE == "MO")
                        {
                            var q = (from m in context.V_ART_ENDTOEND_REPORT
                                     where m.WF_NO.StartsWith("MO-N-")
                                     && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                     select new V_ART_ENDTOEND_REPORT_3()
                                     {
                                         WF_ID = m.WF_ID,
                                         WF_SUB_ID = m.WF_SUB_ID,
                                         WF_TYPE = m.WF_TYPE,
                                         DUE_DATE = m.DUE_DATE,
                                         IS_END = m.IS_END,
                                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                                         IS_TERMINATE = m.IS_TERMINATE,
                                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                                         STEP_END_DATE = m.STEP_END_DATE,
                                         CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                                     });

                            if (!string.IsNullOrEmpty(param.data.DATE_FROM))
                                q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date);
                            if (!string.IsNullOrEmpty(param.data.DATE_TO))
                                q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date);

                            var mockupNew = q.Distinct().ToList();

                            var mainProcess = mockupNew.Where(m => m.CURRENT_STEP_ID == SEND_PG.STEP_MOCKUP_ID).ToList();
                            var customerProcess = mockupNew.Where(m => m.CURRENT_STEP_ID == SEND_CUS_APP.STEP_MOCKUP_ID).ToList();
                            var rdProcess = mockupNew.Where(m => m.CURRENT_STEP_ID == SEND_RD_PRI_PKG.STEP_MOCKUP_ID).ToList();
                            var pnProcess = mockupNew.Where(m => m.CURRENT_STEP_ID == SEND_PN_PRI_PKG.STEP_MOCKUP_ID).ToList();

                            temp = new SUMMARY_REPORT_MODEL();
                            foreach (var item in mainProcess)
                            {
                                var fixMoc = 5;
                                var dtWillFinish = item.DUE_DATE;
                                var lastSendToCustomer = customerProcess.Where(m => m.WF_ID == item.WF_ID).OrderByDescending(m => m.STEP_CREATE_DATE).FirstOrDefault();
                                if (lastSendToCustomer != null)
                                {
                                    dtWillFinish = lastSendToCustomer.STEP_CREATE_DATE;
                                }

                                var listRD = rdProcess.Where(m => m.WF_ID == item.WF_ID).ToList();
                                var listPN = pnProcess.Where(m => m.WF_ID == item.WF_ID).ToList();
                                foreach (var itemRD in listRD)
                                {
                                    if (itemRD.STEP_END_DATE != null)
                                        fixMoc += CNService.GetWorkingDays(itemRD.STEP_CREATE_DATE, itemRD.STEP_END_DATE.Value);
                                }
                                foreach (var itemPN in listPN)
                                {
                                    if (itemPN.STEP_END_DATE != null)
                                        fixMoc += CNService.GetWorkingDays(itemPN.STEP_CREATE_DATE, itemPN.STEP_END_DATE.Value);
                                }

                                if (lastSendToCustomer != null)
                                {
                                    if (string.IsNullOrEmpty(item.IS_TERMINATE) && string.IsNullOrEmpty(item.REMARK_KILLPROCESS))
                                    {
                                        if (CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixMoc) >= lastSendToCustomer.STEP_CREATE_DATE)
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_ONTIME += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_ONTIME++;
                                        }
                                        else
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_OVERDUE += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_OVER_DUE++;
                                        }
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(item.IS_END))
                                    {
                                        if (DateTime.Now.AddDays(1).Date == CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixMoc).Date)
                                        {
                                            temp.LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE += "," + item.WF_SUB_ID;
                                            temp.INPROCESS_ALMOST_DUE++;
                                        }
                                        else if (DateTime.Now > CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixMoc))
                                        {
                                            temp.LIST_WF_SUB_ID_INPROCESS_OVERDUE += "," + item.WF_SUB_ID;
                                            temp.INPROCESS_OVER_DUE++;
                                        }
                                        else
                                        {
                                            temp.LIST_WF_SUB_ID_INPROCESS_ONTIME += "," + item.WF_SUB_ID;
                                            temp.INPROCESS_ONTIME++;
                                        }
                                    }
                                    else if (string.IsNullOrEmpty(item.IS_TERMINATE) && string.IsNullOrEmpty(item.REMARK_KILLPROCESS))
                                    {
                                        if (item.STEP_END_DATE <= CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixMoc))
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_ONTIME += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_ONTIME++;
                                        }
                                        else
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_OVERDUE += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_OVER_DUE++;
                                        }
                                    }
                                }
                            }
                            temp.WORKFLOW_TYPE = "Mockup normal";
                            temp.CNT_WF = mainProcess.Count();

                            res.data.Add(temp);
                        }

                        if (param.data.WORKFLOW_TYPE == "ALL" || param.data.WORKFLOW_TYPE == "MODIELINE" || param.data.WORKFLOW_TYPE == "MO")
                        {
                            var q = (from m in context.V_ART_ENDTOEND_REPORT
                                     where m.WF_NO.StartsWith("MO-D-") && m.CURRENT_STEP_ID == SEND_PG.STEP_MOCKUP_ID
                                         && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                     select new V_ART_ENDTOEND_REPORT_3()
                                     {
                                         WF_ID = m.WF_ID,
                                         WF_SUB_ID = m.WF_SUB_ID,
                                         WF_TYPE = m.WF_TYPE,
                                         DUE_DATE = m.DUE_DATE,
                                         IS_END = m.IS_END,
                                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                                         IS_TERMINATE = m.IS_TERMINATE,
                                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                                         STEP_END_DATE = m.STEP_END_DATE,
                                         CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                                     });

                            if (!string.IsNullOrEmpty(param.data.DATE_FROM))
                                q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date);
                            if (!string.IsNullOrEmpty(param.data.DATE_TO))
                                q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date);

                            var mockupDieline = q.Distinct().ToList();

                            temp = new SUMMARY_REPORT_MODEL();
                            foreach (var item in mockupDieline)
                            {
                                var fixMocDieline = 7;
                                if (string.IsNullOrEmpty(item.IS_END))
                                {
                                    if (DateTime.Now.AddDays(1).Date == CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixMocDieline).Date)
                                    {
                                        temp.LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE += "," + item.WF_SUB_ID;
                                        temp.INPROCESS_ALMOST_DUE++;
                                    }
                                    else if (DateTime.Now > CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixMocDieline))
                                    {
                                        temp.LIST_WF_SUB_ID_INPROCESS_OVERDUE += "," + item.WF_SUB_ID;
                                        temp.INPROCESS_OVER_DUE++;
                                    }
                                    else
                                    {
                                        temp.LIST_WF_SUB_ID_INPROCESS_ONTIME += "," + item.WF_SUB_ID;
                                        temp.INPROCESS_ONTIME++;
                                    }
                                }
                                else if (string.IsNullOrEmpty(item.IS_TERMINATE) && string.IsNullOrEmpty(item.REMARK_KILLPROCESS))
                                {
                                    if (item.STEP_END_DATE <= CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixMocDieline))
                                    {
                                        temp.LIST_WF_SUB_ID_COMPLETED_ONTIME += "," + item.WF_SUB_ID;
                                        temp.COMPLETED_ONTIME++;
                                    }
                                    else
                                    {
                                        temp.LIST_WF_SUB_ID_COMPLETED_OVERDUE += "," + item.WF_SUB_ID;
                                        temp.COMPLETED_OVER_DUE++;
                                    }
                                }
                            }
                            temp.WORKFLOW_TYPE = "Mockup dieline";
                            temp.CNT_WF = mockupDieline.Count();

                            res.data.Add(temp);
                        }

                        if (param.data.WORKFLOW_TYPE == "ALL" || param.data.WORKFLOW_TYPE == "MODESIGN" || param.data.WORKFLOW_TYPE == "MO")
                        {
                            var q = (from m in context.V_ART_ENDTOEND_REPORT
                                     where m.WF_NO.StartsWith("MO-P-")
                                         && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                     select new V_ART_ENDTOEND_REPORT_3()
                                     {
                                         WF_ID = m.WF_ID,
                                         WF_SUB_ID = m.WF_SUB_ID,
                                         WF_TYPE = m.WF_TYPE,
                                         DUE_DATE = m.DUE_DATE,
                                         IS_END = m.IS_END,
                                         REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                                         IS_TERMINATE = m.IS_TERMINATE,
                                         REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                                         STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                                         STEP_END_DATE = m.STEP_END_DATE,
                                         CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                                     });

                            if (!string.IsNullOrEmpty(param.data.DATE_FROM))
                                q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date);
                            if (!string.IsNullOrEmpty(param.data.DATE_TO))
                                q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date);

                            var mockupDesign = q.Distinct().ToList();

                            var mainProcess = mockupDesign.Where(m => m.CURRENT_STEP_ID == SEND_PG.STEP_MOCKUP_ID).ToList();
                            var customerProcess = mockupDesign.Where(m => m.CURRENT_STEP_ID == SEND_CUS_APP.STEP_MOCKUP_ID).ToList();
                            var rdProcess = mockupDesign.Where(m => m.CURRENT_STEP_ID == SEND_RD_PRI_PKG.STEP_MOCKUP_ID).ToList();
                            var pnProcess = mockupDesign.Where(m => m.CURRENT_STEP_ID == SEND_PN_PRI_PKG.STEP_MOCKUP_ID).ToList();

                            temp = new SUMMARY_REPORT_MODEL();
                            foreach (var item in mainProcess)
                            {
                                var fixDesign = 5;
                                var dtWillFinish = item.DUE_DATE;
                                var lastSendToCustomer = customerProcess.Where(m => m.WF_ID == item.WF_ID).OrderByDescending(m => m.STEP_CREATE_DATE).FirstOrDefault();
                                if (lastSendToCustomer != null)
                                {
                                    dtWillFinish = lastSendToCustomer.STEP_CREATE_DATE;
                                }

                                var listRD = rdProcess.Where(m => m.WF_ID == item.WF_ID).ToList();
                                var listPN = pnProcess.Where(m => m.WF_ID == item.WF_ID).ToList();
                                foreach (var itemRD in listRD)
                                {
                                    if (itemRD.STEP_END_DATE != null)
                                        fixDesign += CNService.GetWorkingDays(itemRD.STEP_CREATE_DATE, itemRD.STEP_END_DATE.Value);
                                }
                                foreach (var itemPN in listPN)
                                {
                                    if (itemPN.STEP_END_DATE != null)
                                        fixDesign += CNService.GetWorkingDays(itemPN.STEP_CREATE_DATE, itemPN.STEP_END_DATE.Value);
                                }

                                if (lastSendToCustomer != null)
                                {
                                    if (string.IsNullOrEmpty(item.IS_TERMINATE) && string.IsNullOrEmpty(item.REMARK_KILLPROCESS))
                                    {
                                        if (CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixDesign) >= lastSendToCustomer.STEP_CREATE_DATE)
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_ONTIME += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_ONTIME++;
                                        }
                                        else
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_OVERDUE += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_OVER_DUE++;
                                        }
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(item.IS_END))
                                    {
                                        if (DateTime.Now.AddDays(1).Date == CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixDesign).Date)
                                        {
                                            temp.LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE += "," + item.WF_SUB_ID;
                                            temp.INPROCESS_ALMOST_DUE++;
                                        }
                                        else if (DateTime.Now > CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixDesign))
                                        {
                                            temp.LIST_WF_SUB_ID_INPROCESS_OVERDUE += "," + item.WF_SUB_ID;
                                            temp.INPROCESS_OVER_DUE++;
                                        }
                                        else
                                        {
                                            temp.LIST_WF_SUB_ID_INPROCESS_ONTIME += "," + item.WF_SUB_ID;
                                            temp.INPROCESS_ONTIME++;
                                        }
                                    }
                                    else if (string.IsNullOrEmpty(item.IS_TERMINATE) && string.IsNullOrEmpty(item.REMARK_KILLPROCESS))
                                    {
                                        if (item.STEP_END_DATE <= CNService.AddBusinessDays(item.STEP_CREATE_DATE, fixDesign))
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_ONTIME += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_ONTIME++;
                                        }
                                        else
                                        {
                                            temp.LIST_WF_SUB_ID_COMPLETED_OVERDUE += "," + item.WF_SUB_ID;
                                            temp.COMPLETED_OVER_DUE++;
                                        }
                                    }
                                }
                            }
                            temp.WORKFLOW_TYPE = "Mockup design";
                            temp.CNT_WF = mainProcess.Count();

                            res.data.Add(temp);
                        }
                    }
                }

                foreach (var item in res.data)
                {
                    if (item.WORKFLOW_TYPE == "Artwork new")
                    {
                        item.COMPLETED_TOTAL = item.COMPLETED_ONTIME + item.COMPLETED_OVER_DUE;
                        if (item.COMPLETED_TOTAL > 0) item.COMPLETED_PERCEN = Math.Round(((double)item.COMPLETED_ONTIME / (double)item.COMPLETED_TOTAL) * 100, 2);
                        item.WF_TOTAL_INPROCESS_BY_WORKFLOW_TYPE += item.INPROCESS_ONTIME + item.INPROCESS_ALMOST_DUE + item.INPROCESS_OVER_DUE;
                        item.CURRENT_STEP_VALUE = "A" + SEND_PA.STEP_ARTWORK_ID;
                        item.CURRENT_STEP_DISPLAY_TEXT = SEND_PA.STEP_ARTWORK_NAME;
                    }
                    if (item.WORKFLOW_TYPE == "Artwork repeat")
                    {
                        item.COMPLETED_TOTAL = item.COMPLETED_ONTIME + item.COMPLETED_OVER_DUE;
                        if (item.COMPLETED_TOTAL > 0) item.COMPLETED_PERCEN = Math.Round(((double)item.COMPLETED_ONTIME / (double)item.COMPLETED_TOTAL) * 100, 2);
                        item.WF_TOTAL_INPROCESS_BY_WORKFLOW_TYPE += item.INPROCESS_ONTIME + item.INPROCESS_ALMOST_DUE + item.INPROCESS_OVER_DUE;
                        //item.CURRENT_STEP_VALUE = "A" + SEND_PP.STEP_ARTWORK_ID;
                        //item.CURRENT_STEP_DISPLAY_TEXT = SEND_PP.STEP_ARTWORK_NAME;
                        item.CURRENT_STEP_VALUE = "A" + SEND_PA.STEP_ARTWORK_ID;
                        item.CURRENT_STEP_DISPLAY_TEXT = SEND_PA.STEP_ARTWORK_NAME;
                    }
                    if (item.WORKFLOW_TYPE == "Artwork repeat R6")
                    {
                        item.COMPLETED_TOTAL = item.COMPLETED_ONTIME + item.COMPLETED_OVER_DUE;
                        if (item.COMPLETED_TOTAL > 0) item.COMPLETED_PERCEN = Math.Round(((double)item.COMPLETED_ONTIME / (double)item.COMPLETED_TOTAL) * 100, 2);
                        item.WF_TOTAL_INPROCESS_BY_WORKFLOW_TYPE += item.INPROCESS_ONTIME + item.INPROCESS_ALMOST_DUE + item.INPROCESS_OVER_DUE;
                        item.CURRENT_STEP_VALUE = "A" + SEND_PA.STEP_ARTWORK_ID;
                        item.CURRENT_STEP_DISPLAY_TEXT = SEND_PA.STEP_ARTWORK_NAME;
                    }
                    if (item.WORKFLOW_TYPE == "Mockup design")
                    {
                        item.COMPLETED_TOTAL = item.COMPLETED_ONTIME + item.COMPLETED_OVER_DUE;
                        if (item.COMPLETED_TOTAL > 0) item.COMPLETED_PERCEN = Math.Round(((double)item.COMPLETED_ONTIME / (double)item.COMPLETED_TOTAL) * 100, 2);
                        item.WF_TOTAL_INPROCESS_BY_WORKFLOW_TYPE += item.INPROCESS_ONTIME + item.INPROCESS_ALMOST_DUE + item.INPROCESS_OVER_DUE;
                        item.CURRENT_STEP_VALUE = "M" + SEND_PG.STEP_MOCKUP_ID;
                        item.CURRENT_STEP_DISPLAY_TEXT = SEND_PG.STEP_MOCKUP_NAME;
                    }
                    if (item.WORKFLOW_TYPE == "Mockup dieline")
                    {
                        item.COMPLETED_TOTAL = item.COMPLETED_ONTIME + item.COMPLETED_OVER_DUE;
                        if (item.COMPLETED_TOTAL > 0) item.COMPLETED_PERCEN = Math.Round(((double)item.COMPLETED_ONTIME / (double)item.COMPLETED_TOTAL) * 100, 2);
                        item.WF_TOTAL_INPROCESS_BY_WORKFLOW_TYPE += item.INPROCESS_ONTIME + item.INPROCESS_ALMOST_DUE + item.INPROCESS_OVER_DUE;
                        item.CURRENT_STEP_VALUE = "M" + SEND_PG.STEP_MOCKUP_ID;
                        item.CURRENT_STEP_DISPLAY_TEXT = SEND_PG.STEP_MOCKUP_NAME;
                    }
                    if (item.WORKFLOW_TYPE == "Mockup normal")
                    {
                        item.COMPLETED_TOTAL = item.COMPLETED_ONTIME + item.COMPLETED_OVER_DUE;
                        if (item.COMPLETED_TOTAL > 0) item.COMPLETED_PERCEN = Math.Round(((double)item.COMPLETED_ONTIME / (double)item.COMPLETED_TOTAL) * 100, 2);
                        item.WF_TOTAL_INPROCESS_BY_WORKFLOW_TYPE += item.INPROCESS_ONTIME + item.INPROCESS_ALMOST_DUE + item.INPROCESS_OVER_DUE;
                        item.CURRENT_STEP_VALUE = "M" + SEND_PG.STEP_MOCKUP_ID;
                        item.CURRENT_STEP_DISPLAY_TEXT = SEND_PG.STEP_MOCKUP_NAME;
                    }
                }

                double allInProcess = 0;
                double allCompleted = 0;
                var tempTotal = new SUMMARY_REPORT_MODEL();
                foreach (var item in res.data)
                {
                    tempTotal.ORDERBY = 8;
                    tempTotal.WORKFLOW_TYPE = "Total";

                    tempTotal.INPROCESS_ONTIME += item.INPROCESS_ONTIME;
                    tempTotal.INPROCESS_ALMOST_DUE += item.INPROCESS_ALMOST_DUE;
                    tempTotal.INPROCESS_OVER_DUE += item.INPROCESS_OVER_DUE;
                    tempTotal.COMPLETED_ONTIME += item.COMPLETED_ONTIME;
                    tempTotal.COMPLETED_OVER_DUE += item.COMPLETED_OVER_DUE;
                    tempTotal.CNT_WF += item.CNT_WF;

                    tempTotal.WF_TOTAL_INPROCESS_BY_WORKFLOW_TYPE += item.INPROCESS_ONTIME + item.INPROCESS_ALMOST_DUE + item.INPROCESS_OVER_DUE;

                    allInProcess += item.INPROCESS_ONTIME + item.INPROCESS_ALMOST_DUE + item.INPROCESS_OVER_DUE;
                    allCompleted += item.COMPLETED_ONTIME + item.COMPLETED_OVER_DUE;

                    tempTotal.COMPLETED_TOTAL += item.COMPLETED_TOTAL;
                    if (tempTotal.COMPLETED_TOTAL > 0) tempTotal.COMPLETED_PERCEN = Math.Round(((double)tempTotal.COMPLETED_ONTIME / (double)tempTotal.COMPLETED_TOTAL) * 100, 2);
                }
                res.data.Add(tempTotal);

                tempTotal = new SUMMARY_REPORT_MODEL();
                foreach (var item in res.data)
                {
                    tempTotal.ORDERBY = 9;
                    tempTotal.WORKFLOW_TYPE = "Percentage (%)";

                    if (allInProcess > 0) tempTotal.INPROCESS_ONTIME = Math.Round(((double)item.INPROCESS_ONTIME / (double)allInProcess) * 100, 2);
                    if (allInProcess > 0) tempTotal.INPROCESS_ALMOST_DUE = Math.Round(((double)item.INPROCESS_ALMOST_DUE / (double)allInProcess) * 100, 2);
                    if (allInProcess > 0) tempTotal.INPROCESS_OVER_DUE = Math.Round(((double)item.INPROCESS_OVER_DUE / (double)allInProcess) * 100, 2);

                    if (allCompleted > 0) tempTotal.COMPLETED_ONTIME = Math.Round(((double)item.COMPLETED_ONTIME / (double)allCompleted) * 100, 2);
                    if (allCompleted > 0) tempTotal.COMPLETED_OVER_DUE = Math.Round(((double)item.COMPLETED_OVER_DUE / (double)allCompleted) * 100, 2);
                }
                res.data.Add(tempTotal);

                Results.status = "S";
                Results.data = res.data;

            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static SUMMARY_REPORT_MODEL_RESULT GetSummaryReportDataDetail(SUMMARY_REPORT_MODEL_REQUEST param)
        {
            var Results = new SUMMARY_REPORT_MODEL_RESULT();
            Results.data = new List<SUMMARY_REPORT_MODEL>();

            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<SUMMARY_REPORT_MODEL>();
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                var getByCreateDateFrom = DateTime.Now;
                var getByCreateDateTo = DateTime.Now;

                if (!string.IsNullOrEmpty(param.data.DATE_FROM))
                    getByCreateDateFrom = CNService.ConvertStringToDate(param.data.DATE_FROM);
                if (!string.IsNullOrEmpty(param.data.DATE_TO))
                    getByCreateDateTo = CNService.ConvertStringToDate(param.data.DATE_TO);

                var paramDEPARTMENT = param.data.DEPARTMENT;
                var currentDate = DateTime.Now;
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 1800;

                        var q = (from m in context.V_ART_ENDTOEND_REPORT
                                 select new SUMMARY_REPORT_MODEL()
                                 {
                                     WORKFLOW_STATUS = m.IS_END == "X" ? (m.IS_TERMINATE == "X" ? "Terminate" : "Completed") : "In progress",
                                     PRIMARY_TYPE = m.PRIMARY_TYPE_TXT,
                                     WF_SUB_ID = m.WF_SUB_ID,
                                     WF_TYPE = m.WF_TYPE,
                                     PA_NAME = m.PA_NAME,
                                     PG_NAME = m.PG_NAME,
                                     MARKETTING = m.MARKETTING,
                                     PRODUCT_CODE = m.PRODUCT_CODE,
                                     PACKAGING_TYPE = m.PACKAGING_TYPE,
                                     SALES_ORDER_NO = m.SALES_ORDER_NO,
                                     DURATION_STANDARD = m.DURATION_STANDARD,
                                     EXTEND_DURATION = m.IS_STEP_DURATION_EXTEND == "X" ? "Yes" : "",
                                     DUE_DATE = m.DUE_DATE,
                                     WF_NO = m.WF_NO,
                                     CURRENT_STEP = m.CURRENT_STEP_NAME,
                                     BRAND = m.BRAND_NAME,
                                     SOLD_TO = m.SOLD_TO,
                                     SHIP_TO = m.SHIP_TO,
                                     RDD = m.RDD,
                                     CURRENT_ASSIGN = m.CURRENT_USER_NAME,
                                     CREATE_DATE = m.STEP_CREATE_DATE,
                                     END_DATE = m.STEP_END_DATE,
                                     CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                                     IS_TERMINATE = m.IS_TERMINATE,
                                     REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                                     REQUEST_CREATE_DATE = m.REQUEST_CREATE_DATE,
                                     STEP_END_DATE = m.STEP_END_DATE,
                                     TOTAL_DAY = m.USE_DAY,
                                     IS_END = m.IS_END,
                                     CUS_OR_VEN_DISPLAY_TXT = !string.IsNullOrEmpty(m.VENDOR_NAME) ? m.VENDOR_NAME : !string.IsNullOrEmpty(m.CUSTOMER_NAME) ? m.CUSTOMER_NAME : "",
                                 });

                        if (!string.IsNullOrEmpty(param.data.DATE_FROM))
                            q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date);
                        if (!string.IsNullOrEmpty(param.data.DATE_TO))
                            q = q.Where(m => DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date);

                        q = q.Where(m => string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS));



                        if (!string.IsNullOrEmpty(param.data.CURRENT_STEP))
                        {
                            if (param.data.CURRENT_STEP.StartsWith("A"))
                            {
                                var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP.Replace("A", ""));
                                q = q.Where(m => m.WF_TYPE == "Artwork");
                                q = q.Where(m => m.CURRENT_STEP_ID == current_step_id);
                            }
                            else if (param.data.CURRENT_STEP.StartsWith("M"))
                            {
                                var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP.Replace("M", ""));
                                q = q.Where(m => m.WF_TYPE == "Mockup");
                                q = q.Where(m => m.CURRENT_STEP_ID == current_step_id);
                            }
                        }

                        var SEND_PP = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PP" }, context).FirstOrDefault();
                        var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault();
                        var SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault();

                        if (param.data.WORKFLOW_TYPE == "ARTNEW")
                            q = q.Where(m => m.WF_NO.StartsWith("AW-N-") && m.CURRENT_STEP_ID == SEND_PA.STEP_ARTWORK_ID);
                        if (param.data.WORKFLOW_TYPE == "ARTREPEAT")
                            q = q.Where(m => m.WF_NO.StartsWith("AW-R-") && m.CURRENT_STEP_ID == SEND_PA.STEP_ARTWORK_ID);
                        if (param.data.WORKFLOW_TYPE == "ARTR6")
                            q = q.Where(m => m.WF_NO.StartsWith("AW-R6-") && m.CURRENT_STEP_ID == SEND_PA.STEP_ARTWORK_ID);

                        if (param.data.WORKFLOW_TYPE == "MONORMAL")
                            q = q.Where(m => m.WF_NO.StartsWith("MO-N-") && m.CURRENT_STEP_ID == SEND_PG.STEP_MOCKUP_ID);
                        if (param.data.WORKFLOW_TYPE == "MODESIGN")
                            q = q.Where(m => m.WF_NO.StartsWith("MO-P-") && m.CURRENT_STEP_ID == SEND_PG.STEP_MOCKUP_ID);
                        if (param.data.WORKFLOW_TYPE == "MODIELINE")
                            q = q.Where(m => m.WF_NO.StartsWith("MO-D-") && m.CURRENT_STEP_ID == SEND_PG.STEP_MOCKUP_ID);

                        if (!string.IsNullOrEmpty(param.data.LIST_WF_SUB_ID))
                        {
                            var temp = GetSummaryReportDataByWorkflowType(param);
                            var LIST_WF_SUB_ID = "";
                            foreach (var s in temp.data)
                            {
                                if (param.data.WORKFLOW_STATUS == "I" && param.data.REQUEST_TYPE == "ONTIME")
                                {
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_INPROCESS_ONTIME;
                                }
                                if (param.data.WORKFLOW_STATUS == "I" && param.data.REQUEST_TYPE == "ALMOSTDUE")
                                {
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE;
                                }
                                if (param.data.WORKFLOW_STATUS == "I" && param.data.REQUEST_TYPE == "OVERDUE")
                                {
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_INPROCESS_OVERDUE;
                                }

                                if (param.data.WORKFLOW_STATUS == "C" && param.data.REQUEST_TYPE == "ONTIME")
                                {
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_COMPLETED_ONTIME;
                                }
                                if (param.data.WORKFLOW_STATUS == "C" && param.data.REQUEST_TYPE == "OVERDUE")
                                {
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_COMPLETED_OVERDUE;
                                }

                                if (param.data.WORKFLOW_STATUS == "I" && param.data.REQUEST_TYPE == "ALL")
                                {
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_INPROCESS_ONTIME;
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE;
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_INPROCESS_OVERDUE;
                                }
                                if (param.data.WORKFLOW_STATUS == "C" && param.data.REQUEST_TYPE == "ALL")
                                {
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_COMPLETED_ONTIME;
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_COMPLETED_OVERDUE;
                                }

                                if (param.data.WORKFLOW_STATUS == "ALL" && param.data.REQUEST_TYPE == "ALL")
                                {
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_INPROCESS_ONTIME;
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_INPROCESS_ALMOSTDUE;
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_INPROCESS_OVERDUE;
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_COMPLETED_ONTIME;
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_COMPLETED_OVERDUE;
                                    LIST_WF_SUB_ID += "," + s.LIST_WF_SUB_ID_ALL;
                                }
                            }

                            var list = new List<int>();
                            var tempLIST_WF_SUB_ID = LIST_WF_SUB_ID.Split(',');
                            foreach (var s in tempLIST_WF_SUB_ID)
                            {
                                if (!string.IsNullOrEmpty(s))
                                    list.Add(Convert.ToInt32(s));
                            }
                            list = list.Distinct().ToList();
                            //q = q.Where(m => list.Contains(m.WF_SUB_ID));

                            var guid = PerformBatchJoinWithIds(list);
                            q = q.Where(m => (from mm in context.ART_TEMP_CONTAIN where mm.GUID == guid select mm.ID_TO_QUERY).Contains((int)m.WF_SUB_ID));

                            var listSubId = new List<int>();
                            IQueryable<V_ART_ENDTOEND_REPORT_3> tempSubId = null;
                            if (param.data.WORKFLOW_STATUS == "I")
                            {
                                tempSubId = from m in context.V_ART_ENDTOEND_REPORT
                                            where m.WF_TYPE == "Mockup"
                                            && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date
                                            && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date
                                            && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                            //&& string.IsNullOrEmpty(m.IS_END)
                                            group m by
                                            new
                                            {
                                                DbFunctions.CreateDateTime(m.STEP_CREATE_DATE.Year, m.STEP_CREATE_DATE.Month, m.STEP_CREATE_DATE.Day, m.STEP_CREATE_DATE.Hour, m.STEP_CREATE_DATE.Minute, 0).Value,
                                                m.CURRENT_STEP_ID,
                                                m.WF_ID
                                            }
                                            into g
                                            select new V_ART_ENDTOEND_REPORT_3()
                                            {
                                                WF_SUB_ID = (from t2 in g select t2.WF_SUB_ID).Min()
                                            };
                            }
                            else if (param.data.WORKFLOW_STATUS == "C")
                            {
                                tempSubId = from m in context.V_ART_ENDTOEND_REPORT
                                            where m.WF_TYPE == "Mockup"
                                            && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date
                                            && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date
                                            && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                            //&& !string.IsNullOrEmpty(m.IS_END)
                                            group m by
                                            new
                                            {
                                                DbFunctions.CreateDateTime(m.STEP_CREATE_DATE.Year, m.STEP_CREATE_DATE.Month, m.STEP_CREATE_DATE.Day, m.STEP_CREATE_DATE.Hour, m.STEP_CREATE_DATE.Minute, 0).Value,
                                                m.CURRENT_STEP_ID,
                                                m.WF_ID
                                            }
                                            into g
                                            select new V_ART_ENDTOEND_REPORT_3()
                                            {
                                                WF_SUB_ID = (from t2 in g select t2.WF_SUB_ID).Min()
                                            };
                            }
                            else if (param.data.WORKFLOW_STATUS == "ALL")
                            {
                                tempSubId = from m in context.V_ART_ENDTOEND_REPORT
                                            where m.WF_TYPE == "Mockup"
                                            && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date
                                            && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date
                                            && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                            group m by
                                            new
                                            {
                                                DbFunctions.CreateDateTime(m.STEP_CREATE_DATE.Year, m.STEP_CREATE_DATE.Month, m.STEP_CREATE_DATE.Day, m.STEP_CREATE_DATE.Hour, m.STEP_CREATE_DATE.Minute, 0).Value,
                                                m.CURRENT_STEP_ID,
                                                m.WF_ID
                                            }
                                            into g
                                            select new V_ART_ENDTOEND_REPORT_3()
                                            {
                                                WF_SUB_ID = (from t2 in g select t2.WF_SUB_ID).Min()
                                            };
                            }
                            if (tempSubId != null)
                                listSubId = tempSubId.Select(m => m.WF_SUB_ID).ToList();

                            var listSubIdArtwork = new List<int>();
                            IQueryable<V_ART_ENDTOEND_REPORT_3> tempSubIdArtwork = null;
                            if (param.data.WORKFLOW_STATUS == "I")
                            {
                                tempSubIdArtwork = from m in context.ART_WF_ARTWORK_PROCESS
                                                   join m2 in context.ART_WF_ARTWORK_REQUEST on m.ARTWORK_REQUEST_ID equals m2.ARTWORK_REQUEST_ID
                                                   where DbFunctions.TruncateTime(m2.CREATE_DATE) >= getByCreateDateFrom.Date
                                                   && DbFunctions.TruncateTime(m2.CREATE_DATE) <= getByCreateDateTo.Date
                                                   && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                                   //&& string.IsNullOrEmpty(m.IS_END)
                                                   group m by
                                                       new
                                                       {
                                                           DbFunctions.CreateDateTime(m.CREATE_DATE.Year, m.CREATE_DATE.Month, m.CREATE_DATE.Day, m.CREATE_DATE.Hour, m.CREATE_DATE.Minute, 0).Value,
                                                           m.CURRENT_STEP_ID,
                                                           m.ARTWORK_ITEM_ID
                                                       }
                                                   into g
                                                   select new V_ART_ENDTOEND_REPORT_3()
                                                   {
                                                       WF_SUB_ID = (from t2 in g select t2.ARTWORK_SUB_ID).Min()
                                                   };
                            }
                            else if (param.data.WORKFLOW_STATUS == "C")
                            {
                                tempSubIdArtwork = from m in context.ART_WF_ARTWORK_PROCESS
                                                   join m2 in context.ART_WF_ARTWORK_REQUEST on m.ARTWORK_REQUEST_ID equals m2.ARTWORK_REQUEST_ID
                                                   where DbFunctions.TruncateTime(m2.CREATE_DATE) >= getByCreateDateFrom.Date
                                                   && DbFunctions.TruncateTime(m2.CREATE_DATE) <= getByCreateDateTo.Date
                                                   && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                                   //&& !string.IsNullOrEmpty(m.IS_END)
                                                   group m by
                                                       new
                                                       {
                                                           DbFunctions.CreateDateTime(m.CREATE_DATE.Year, m.CREATE_DATE.Month, m.CREATE_DATE.Day, m.CREATE_DATE.Hour, m.CREATE_DATE.Minute, 0).Value,
                                                           m.CURRENT_STEP_ID,
                                                           m.ARTWORK_ITEM_ID
                                                       }
                                                   into g
                                                   select new V_ART_ENDTOEND_REPORT_3()
                                                   {
                                                       WF_SUB_ID = (from t2 in g select t2.ARTWORK_SUB_ID).Min()
                                                   };
                            }
                            else if (param.data.WORKFLOW_STATUS == "ALL")
                            {
                                tempSubIdArtwork = from m in context.ART_WF_ARTWORK_PROCESS
                                                   join m2 in context.ART_WF_ARTWORK_REQUEST on m.ARTWORK_REQUEST_ID equals m2.ARTWORK_REQUEST_ID
                                                   where DbFunctions.TruncateTime(m2.CREATE_DATE) >= getByCreateDateFrom.Date
                                                   && DbFunctions.TruncateTime(m2.CREATE_DATE) <= getByCreateDateTo.Date
                                                   && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                                   group m by
                                                       new
                                                       {
                                                           DbFunctions.CreateDateTime(m.CREATE_DATE.Year, m.CREATE_DATE.Month, m.CREATE_DATE.Day, m.CREATE_DATE.Hour, m.CREATE_DATE.Minute, 0).Value,
                                                           m.CURRENT_STEP_ID,
                                                           m.ARTWORK_ITEM_ID
                                                       }
                                                   into g
                                                   select new V_ART_ENDTOEND_REPORT_3()
                                                   {
                                                       WF_SUB_ID = (from t2 in g select t2.ARTWORK_SUB_ID).Min()
                                                   };
                            }
                            if (tempSubIdArtwork != null)
                                listSubIdArtwork = tempSubIdArtwork.Select(m => m.WF_SUB_ID).ToList();

                            //q = q.Where(m => (listSubIdArtwork.Contains((int)m.WF_SUB_ID) && m.WF_TYPE == "Artwork") || (listSubId.Contains((int)m.WF_SUB_ID) && m.WF_TYPE == "Mockup"));

                            var guid2 = PerformBatchJoinWithIds(listSubIdArtwork);
                            var guid3 = PerformBatchJoinWithIds(listSubId);

                            if (tempSubId != null && tempSubIdArtwork != null)
                            {
                                q = q.Where(m => ((from mm in context.ART_TEMP_CONTAIN where mm.GUID == guid2 select mm.ID_TO_QUERY).Contains((int)m.WF_SUB_ID) && m.WF_TYPE == "Artwork")
                                || (from mm in context.ART_TEMP_CONTAIN where mm.GUID == guid3 select mm.ID_TO_QUERY).Contains((int)m.WF_SUB_ID) && m.WF_TYPE == "Mockup");
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(param.data.WORKFLOW_STATUS))
                            {
                                if (param.data.WORKFLOW_STATUS == "I") q = q.Where(m => m.WORKFLOW_STATUS == "In progress");
                                if (param.data.WORKFLOW_STATUS == "T") q = q.Where(m => m.WORKFLOW_STATUS == "Terminate");
                                if (param.data.WORKFLOW_STATUS == "C") q = q.Where(m => m.WORKFLOW_STATUS == "Completed");
                            }

                            var temp = currentDate.Date.AddDays(1);
                            if (param.data.WORKFLOW_STATUS == "I")
                            {
                                if (param.data.REQUEST_TYPE == "ONTIME")
                                    q = q.Where(m => m.DUE_DATE > currentDate && DbFunctions.TruncateTime(m.DUE_DATE) != DbFunctions.TruncateTime(temp));
                                else if (param.data.REQUEST_TYPE == "ALMOSTDUE")
                                    q = q.Where(m => DbFunctions.TruncateTime(m.DUE_DATE) == DbFunctions.TruncateTime(temp));
                                else if (param.data.REQUEST_TYPE == "OVERDUE")
                                    q = q.Where(m => m.DUE_DATE < currentDate);
                            }
                            else if (param.data.WORKFLOW_STATUS == "C")
                            {
                                if (param.data.REQUEST_TYPE == "ONTIME")
                                    q = q.Where(m => m.DUE_DATE >= m.STEP_END_DATE);
                                else if (param.data.REQUEST_TYPE == "OVERDUE")
                                    q = q.Where(m => m.DUE_DATE < m.STEP_END_DATE);
                            }

                            var listSubId = new List<int>();
                            IQueryable<V_ART_ENDTOEND_REPORT_3> tempSubId = null;
                            if (param.data.WORKFLOW_STATUS == "I")
                            {
                                tempSubId = from m in context.V_ART_ENDTOEND_REPORT
                                            where m.WF_TYPE == "Mockup"
                                            && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date
                                            && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date
                                            && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                            && string.IsNullOrEmpty(m.IS_END)
                                            group m by
                                            new
                                            {
                                                DbFunctions.CreateDateTime(m.STEP_CREATE_DATE.Year, m.STEP_CREATE_DATE.Month, m.STEP_CREATE_DATE.Day, m.STEP_CREATE_DATE.Hour, m.STEP_CREATE_DATE.Minute, 0).Value,
                                                m.CURRENT_STEP_ID,
                                                m.WF_ID
                                            }
                                            into g
                                            select new V_ART_ENDTOEND_REPORT_3()
                                            {
                                                WF_SUB_ID = (from t2 in g select t2.WF_SUB_ID).Min()
                                            };
                            }
                            else if (param.data.WORKFLOW_STATUS == "C")
                            {
                                tempSubId = from m in context.V_ART_ENDTOEND_REPORT
                                            where m.WF_TYPE == "Mockup"
                                            && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date
                                            && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date
                                            && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                            && !string.IsNullOrEmpty(m.IS_END)
                                            group m by
                                            new
                                            {
                                                DbFunctions.CreateDateTime(m.STEP_CREATE_DATE.Year, m.STEP_CREATE_DATE.Month, m.STEP_CREATE_DATE.Day, m.STEP_CREATE_DATE.Hour, m.STEP_CREATE_DATE.Minute, 0).Value,
                                                m.CURRENT_STEP_ID,
                                                m.WF_ID
                                            }
                                            into g
                                            select new V_ART_ENDTOEND_REPORT_3()
                                            {
                                                WF_SUB_ID = (from t2 in g select t2.WF_SUB_ID).Min()
                                            };
                            }
                            else if (param.data.WORKFLOW_STATUS == "ALL")
                            {
                                tempSubId = from m in context.V_ART_ENDTOEND_REPORT
                                            where m.WF_TYPE == "Mockup"
                                            && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= getByCreateDateFrom.Date
                                            && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= getByCreateDateTo.Date
                                            && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                            group m by
                                            new
                                            {
                                                DbFunctions.CreateDateTime(m.STEP_CREATE_DATE.Year, m.STEP_CREATE_DATE.Month, m.STEP_CREATE_DATE.Day, m.STEP_CREATE_DATE.Hour, m.STEP_CREATE_DATE.Minute, 0).Value,
                                                m.CURRENT_STEP_ID,
                                                m.WF_ID
                                            }
                                            into g
                                            select new V_ART_ENDTOEND_REPORT_3()
                                            {
                                                WF_SUB_ID = (from t2 in g select t2.WF_SUB_ID).Min()
                                            };
                            }
                            if (tempSubId != null)
                                listSubId = tempSubId.Select(m => m.WF_SUB_ID).ToList();

                            var listSubIdArtwork = new List<int>();
                            IQueryable<V_ART_ENDTOEND_REPORT_3> tempSubIdArtwork = null;
                            if (param.data.WORKFLOW_STATUS == "I")
                            {
                                tempSubIdArtwork = from m in context.ART_WF_ARTWORK_PROCESS
                                                   join m2 in context.ART_WF_ARTWORK_REQUEST on m.ARTWORK_REQUEST_ID equals m2.ARTWORK_REQUEST_ID
                                                   where DbFunctions.TruncateTime(m2.CREATE_DATE) >= getByCreateDateFrom.Date
                                                   && DbFunctions.TruncateTime(m2.CREATE_DATE) <= getByCreateDateTo.Date
                                                   && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                                   && string.IsNullOrEmpty(m.IS_END)
                                                   group m by
                                                       new
                                                       {
                                                           DbFunctions.CreateDateTime(m.CREATE_DATE.Year, m.CREATE_DATE.Month, m.CREATE_DATE.Day, m.CREATE_DATE.Hour, m.CREATE_DATE.Minute, 0).Value,
                                                           m.CURRENT_STEP_ID,
                                                           m.ARTWORK_ITEM_ID
                                                       }
                                                   into g
                                                   select new V_ART_ENDTOEND_REPORT_3()
                                                   {
                                                       WF_SUB_ID = (from t2 in g select t2.ARTWORK_SUB_ID).Min()
                                                   };
                            }
                            else if (param.data.WORKFLOW_STATUS == "C")
                            {
                                tempSubIdArtwork = from m in context.ART_WF_ARTWORK_PROCESS
                                                   join m2 in context.ART_WF_ARTWORK_REQUEST on m.ARTWORK_REQUEST_ID equals m2.ARTWORK_REQUEST_ID
                                                   where DbFunctions.TruncateTime(m2.CREATE_DATE) >= getByCreateDateFrom.Date
                                                   && DbFunctions.TruncateTime(m2.CREATE_DATE) <= getByCreateDateTo.Date
                                                   && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                                   && !string.IsNullOrEmpty(m.IS_END)
                                                   group m by
                                                       new
                                                       {
                                                           DbFunctions.CreateDateTime(m.CREATE_DATE.Year, m.CREATE_DATE.Month, m.CREATE_DATE.Day, m.CREATE_DATE.Hour, m.CREATE_DATE.Minute, 0).Value,
                                                           m.CURRENT_STEP_ID,
                                                           m.ARTWORK_ITEM_ID
                                                       }
                                                   into g
                                                   select new V_ART_ENDTOEND_REPORT_3()
                                                   {
                                                       WF_SUB_ID = (from t2 in g select t2.ARTWORK_SUB_ID).Min()
                                                   };
                            }
                            else if (param.data.WORKFLOW_STATUS == "ALL")
                            {
                                tempSubIdArtwork = from m in context.ART_WF_ARTWORK_PROCESS
                                                   join m2 in context.ART_WF_ARTWORK_REQUEST on m.ARTWORK_REQUEST_ID equals m2.ARTWORK_REQUEST_ID
                                                   where DbFunctions.TruncateTime(m2.CREATE_DATE) >= getByCreateDateFrom.Date
                                                   && DbFunctions.TruncateTime(m2.CREATE_DATE) <= getByCreateDateTo.Date
                                                   && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                                   group m by
                                                       new
                                                       {
                                                           DbFunctions.CreateDateTime(m.CREATE_DATE.Year, m.CREATE_DATE.Month, m.CREATE_DATE.Day, m.CREATE_DATE.Hour, m.CREATE_DATE.Minute, 0).Value,
                                                           m.CURRENT_STEP_ID,
                                                           m.ARTWORK_ITEM_ID
                                                       }
                                                   into g
                                                   select new V_ART_ENDTOEND_REPORT_3()
                                                   {
                                                       WF_SUB_ID = (from t2 in g select t2.ARTWORK_SUB_ID).Min()
                                                   };
                            }
                            if (tempSubIdArtwork != null)
                                listSubIdArtwork = tempSubIdArtwork.Select(m => m.WF_SUB_ID).ToList();

                            //q = q.Where(m => (listSubIdArtwork.Contains((int)m.WF_SUB_ID) && m.WF_TYPE == "Artwork") || (listSubId.Contains((int)m.WF_SUB_ID) && m.WF_TYPE == "Mockup"));

                            var guid2 = PerformBatchJoinWithIds(listSubIdArtwork);
                            var guid3 = PerformBatchJoinWithIds(listSubId);

                            if (tempSubId != null && tempSubIdArtwork != null)
                            {
                                q = q.Where(m => ((from mm in context.ART_TEMP_CONTAIN where mm.GUID == guid2 select mm.ID_TO_QUERY).Contains((int)m.WF_SUB_ID) && m.WF_TYPE == "Artwork")
                                || (from mm in context.ART_TEMP_CONTAIN where mm.GUID == guid3 select mm.ID_TO_QUERY).Contains((int)m.WF_SUB_ID) && m.WF_TYPE == "Mockup");
                            }
                        }

                        if (string.IsNullOrEmpty(param.data.LIST_WF_SUB_ID))
                        {
                            var allMockupStep = ART_M_STEP_MOCKUP_SERVICE.GetAll(context).ToList();
                            var allArtworkStep = ART_M_STEP_ARTWORK_SERVICE.GetAll(context).ToList();
                            if (param.data.DEPARTMENT == "PK")
                            {
                                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_PA"
                                || m.STEP_ARTWORK_CODE == "SEND_PG"
                                || m.STEP_ARTWORK_CODE == "SEND_PP").ToList();

                                allMockupStep = allMockupStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_PG"
                                || m.STEP_MOCKUP_CODE == "SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN"
                                || m.STEP_MOCKUP_CODE == "SEND_PG_SUP_SEL_VENDOR"
                                || m.STEP_MOCKUP_CODE == "SEND_APP_MATCH_BOARD").ToList();

                                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();
                                var temp2 = allMockupStep.Select(m => m.STEP_MOCKUP_ID).ToList();
                                q = q.Where(m => (temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork") || (temp2.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"));
                            }
                            else if (param.data.DEPARTMENT == "MK")
                            {
                                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_MK_VERIFY"
                                || m.STEP_ARTWORK_CODE == "SEND_GM_MK"
                                || m.STEP_ARTWORK_CODE == "SEND_BACK_MK"
                                || m.STEP_ARTWORK_CODE == "SEND_MK").ToList();

                                allMockupStep = allMockupStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_MK_UPD_PACK_STYLE"
                                || m.STEP_MOCKUP_CODE == "SEND_BACK_MK"
                                || m.STEP_MOCKUP_CODE == "SEND_MK_APP").ToList();

                                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();
                                var temp2 = allMockupStep.Select(m => m.STEP_MOCKUP_ID).ToList();
                                q = q.Where(m => (temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork") || (temp2.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"));
                            }
                            else if (param.data.DEPARTMENT == "QC")
                            {
                                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_QC"
                                || m.STEP_ARTWORK_CODE == "SEND_GM_QC"
                                || m.STEP_ARTWORK_CODE == "SEND_QC_VERIFY").ToList();

                                allMockupStep = new List<ART_M_STEP_MOCKUP>();

                                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();
                                var temp2 = allMockupStep.Select(m => m.STEP_MOCKUP_ID).ToList();
                                q = q.Where(m => (temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork") || (temp2.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"));
                            }
                            else if (param.data.DEPARTMENT == "RD")
                            {
                                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_RD").ToList();

                                allMockupStep = allMockupStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_RD_PRI_PKG").ToList();

                                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();
                                var temp2 = allMockupStep.Select(m => m.STEP_MOCKUP_ID).ToList();
                                q = q.Where(m => (temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork") || (temp2.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"));
                            }
                            else if (param.data.DEPARTMENT == "WH")
                            {
                                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_WH").ToList();

                                allMockupStep = allMockupStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_WH_TEST_PACK").ToList();

                                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();
                                var temp2 = allMockupStep.Select(m => m.STEP_MOCKUP_ID).ToList();
                                q = q.Where(m => (temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork") || (temp2.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"));
                            }
                            else if (param.data.DEPARTMENT == "PN")
                            {
                                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_PN").ToList();

                                allMockupStep = allMockupStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_PN_PRI_PKG").ToList();

                                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();
                                var temp2 = allMockupStep.Select(m => m.STEP_MOCKUP_ID).ToList();
                                q = q.Where(m => (temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork") || (temp2.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"));
                            }
                            else if (param.data.DEPARTMENT == "CUS")
                            {
                                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_CUS_REVIEW"
                                                || m.STEP_ARTWORK_CODE == "SEND_CUS_PRINT"
                                                || m.STEP_ARTWORK_CODE == "SEND_CUS_SHADE"
                                                || m.STEP_ARTWORK_CODE == "SEND_CUS_REF"
                                                || m.STEP_ARTWORK_CODE == "SEND_CUS_REQ_REF").ToList();

                                allMockupStep = allMockupStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_CUS_APP").ToList();

                                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();
                                var temp2 = allMockupStep.Select(m => m.STEP_MOCKUP_ID).ToList();
                                q = q.Where(m => (temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork") || (temp2.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"));
                            }
                            else if (param.data.DEPARTMENT == "VN")
                            {
                                allArtworkStep = allArtworkStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_VN_PO"
                                       || m.STEP_ARTWORK_CODE == "SEND_VN_SL"
                                       || m.STEP_ARTWORK_CODE == "SEND_VN_PM").ToList();

                                allMockupStep = allMockupStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_VN_MB"
                                 || m.STEP_MOCKUP_CODE == "SEND_VN_DL"
                                 || m.STEP_MOCKUP_CODE == "SEND_VN_RS"
                                 || m.STEP_MOCKUP_CODE == "SEND_VN_PR"
                                 || m.STEP_MOCKUP_CODE == "SEND_VN_QUO").ToList();

                                var temp = allArtworkStep.Select(m => m.STEP_ARTWORK_ID).ToList();
                                var temp2 = allMockupStep.Select(m => m.STEP_MOCKUP_ID).ToList();

                                q = q.Where(m => (temp.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Artwork") || (temp2.Contains((int)m.CURRENT_STEP_ID) && m.WF_TYPE == "Mockup"));
                            }
                        }
                        SummaryOrderBy(param, q, ref Results);
                    }

                    foreach (var item in Results.data)
                    {
                        if (string.IsNullOrEmpty(item.IS_END))
                        {
                            item.TOTAL_DAY = CNService.GetBusinessDays(item.CREATE_DATE.Value.Date, DateTime.Now.Date);
                        }
                    }

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

        private static void SummaryOrderBy(SUMMARY_REPORT_MODEL_REQUEST param, IQueryable<SUMMARY_REPORT_MODEL> q, ref SUMMARY_REPORT_MODEL_RESULT Results)
        {
            var orderColumn = 1;
            var orderDir = "asc";
            if (param.order != null && param.order.Count > 0)
            {
                orderColumn = param.order[0].column;
                orderDir = param.order[0].dir; //desc ,asc

                Results.ORDER_COLUMN = param.order[0].column;
            }

            string orderASC = "asc";
            string orderDESC = "desc";
            List<string> temp = new List<string>();

            if (orderColumn == 1)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.WF_NO).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.WF_NO).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 2)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.CURRENT_STEP).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.CURRENT_STEP).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 3)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.WORKFLOW_STATUS).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.WORKFLOW_STATUS).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 4)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.CURRENT_ASSIGN).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.CURRENT_ASSIGN).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 5)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.CUS_OR_VEN_DISPLAY_TXT).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.CUS_OR_VEN_DISPLAY_TXT).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 6)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.CREATE_DATE).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.CREATE_DATE).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 7)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.DURATION_STANDARD).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.DURATION_STANDARD).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 8)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.EXTEND_DURATION).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.EXTEND_DURATION).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 9)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.DUE_DATE).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.DUE_DATE).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 10)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.END_DATE).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.END_DATE).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 11)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.TOTAL_DAY).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.TOTAL_DAY).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 12)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.SALES_ORDER_NO).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.SALES_ORDER_NO).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 13)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.BRAND).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.BRAND).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 14)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.SOLD_TO).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.SOLD_TO).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 15)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.SHIP_TO).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.SHIP_TO).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 16)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.PACKAGING_TYPE).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.PACKAGING_TYPE).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 17)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.PRIMARY_TYPE).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.PRIMARY_TYPE).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 18)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.PRODUCT_CODE).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.PRODUCT_CODE).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 19)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.RDD).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.RDD).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 20)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.PA_NAME).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.PA_NAME).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 21)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.PG_NAME).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.PG_NAME).Skip(param.start).Take(param.length).ToList();
                }
            }
            else if (orderColumn == 22)
            {
                if (orderDir == orderASC)
                {
                    Results.data = q.OrderBy(m => m.MARKETTING).Skip(param.start).Take(param.length).ToList();
                }
                else if (orderDir == orderDESC)
                {
                    Results.data = q.OrderByDescending(m => m.MARKETTING).Skip(param.start).Take(param.length).ToList();
                }
            }

            Results.recordsTotal = q.Select(m => m.WF_SUB_ID).Count();
            Results.recordsFiltered = Results.recordsTotal;
        }

        public static string PerformBatchJoinWithIds_old(IEnumerable<int> ids)
        {
            using (var context = new ARTWORKEntities())
            {
                // Disable auto detection of changes; much faster for batch edits/inserts
                context.Configuration.AutoDetectChangesEnabled = false;
                // A GUID will keep track of this batch operation
                var uniqueId = Guid.NewGuid().ToString();
                // Insert the batchquery objects for each id
                foreach (var id in ids)
                {
                    context.ART_TEMP_CONTAIN.Add(new ART_TEMP_CONTAIN { GUID = uniqueId, ID_TO_QUERY = id, CREATE_DATE = DateTime.Now });
                }
                // Detect all changes in one shot and then save them
                context.ChangeTracker.DetectChanges();
                context.SaveChanges();
                // Now we can re-enable auto detection of changes (in case we use this context elsewhere)
                context.Configuration.AutoDetectChangesEnabled = true;
                // Join the batch queries table with the records we're trying to get

                // Finally, we can delete all of the BatchQuery records matching the GUID
                //context.Database.ExecuteSqlCommand("DELETE FROM ART_TEMP_CONTAIN WHERE GUID = {0}", uniqueId);
                //return entities;
                return uniqueId;
            }
        }

        public static string PerformBatchJoinWithIds(IEnumerable<int> ids)
        {
            StringBuilder sbSQL = new StringBuilder();
            var uniqueId = Guid.NewGuid().ToString();

            foreach (var id in ids)
            {
                sbSQL.AppendLine("INSERT INTO [dbo].[ART_TEMP_CONTAIN]");
                sbSQL.AppendLine("           ([GUID]                  ");
                sbSQL.AppendLine("           ,[ID_TO_QUERY]           ");
                sbSQL.AppendLine("           ,[CREATE_DATE]           ");
                sbSQL.AppendLine("          )                         ");
                sbSQL.AppendLine("		VALUES                        ");
                sbSQL.AppendLine("           ('" + uniqueId + "'                      ");
                sbSQL.AppendLine("           ," + id + "                       ");
                sbSQL.AppendLine("           ,getdate()               ");
                sbSQL.AppendLine("          )                         ");
                sbSQL.AppendLine("");

            }

            if (!string.IsNullOrEmpty(sbSQL.ToString()))
            {
                using (var context = new ARTWORKEntities())
                {
                    // context.Configuration.AutoDetectChangesEnabled = false;
                    context.Database.ExecuteSqlCommand(sbSQL.ToString());
                }
            }

            return uniqueId;
        }

    }
}



