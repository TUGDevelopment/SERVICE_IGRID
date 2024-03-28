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
    public class KPIReportHelper
    {
        public static KPI_REPORT_MODEL_RESULT GetKPIReport(KPI_REPORT_MODEL_REQUEST param)
        {
            var Results = new KPI_REPORT_MODEL_RESULT();
            try
            {
                var res = new KPI_REPORT_MODEL_RESULT();
                res.data = new List<KPI_REPORT_MODEL>();

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param.data.FIRST_LOAD)
                        {
                            Results.status = "S";
                            Results.data = new List<KPI_REPORT_MODEL>();
                            Results.draw = param.draw;
                            return Results;
                        }

                        DateTime dateFrom = CNService.ConvertStringToDate(param.data.DATE_FROM);
                        DateTime dateTo = CNService.ConvertStringToDate(param.data.DATE_TO);

                        var EmployeeInDep = new List<KPI_REPORT_MODEL>();

                        int difdate = FormNumberHelper.GetMonthDifference(dateFrom, dateTo);
                        var position_ = context.ART_M_POSITION.Where(w => w.ART_M_POSITION_CODE == "PK").FirstOrDefault();

                        if (difdate > 12)
                        {
                            Results.status = "E";
                            Results.msg = "Maximum report display is 12 months.";
                            return Results;
                        }
                        else if (param.data.KPI_TYPE == "reject")
                        {
                            var StepCusApp = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP { STEP_MOCKUP_CODE = "SEND_CUS_APP" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                            var StepMKApp = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP { STEP_MOCKUP_CODE = "SEND_MK_APP" }, context).FirstOrDefault().STEP_MOCKUP_ID;

                            if (param.data.USERID != -1)
                                EmployeeInDep = (from m in context.ART_M_POSITION_ROLE
                                                 join m3 in context.ART_M_POSITION on m.POSITION_ID equals m3.ART_M_POSITION_ID
                                                 join m4 in context.ART_M_USER on m.POSITION_ID equals m4.POSITION_ID
                                                 where m4.USER_ID == param.data.USERID
                                                 select new KPI_REPORT_MODEL
                                                 {
                                                     EMPLOYEE_ID_DISPLAY_TEXT = m4.USERNAME,
                                                     EMPLOYEE_NAME_DISPLAY_TEXT = m4.TITLE + m4.FIRST_NAME + " " + m4.LAST_NAME,
                                                     POSITION_DISPLAY_TEXT = m3.ART_M_POSITION_NAME,
                                                     USERID = m4.USER_ID
                                                 }).Distinct().ToList();
                            else
                            {
                                var role_param = (from p in context.ART_M_ROLE
                                                  where p.ROLE_CODE.StartsWith("PG_")
                                                  select p.ROLE_ID).ToList();

                                var userRolePA = (from p in context.ART_M_USER_ROLE
                                                  where role_param.Contains(p.ROLE_ID)
                                                  select p.USER_ID).ToList();


                                EmployeeInDep = (from p in context.ART_M_USER
                                                 where userRolePA.Contains(p.USER_ID) && p.IS_ACTIVE == "X" && p.POSITION_ID == position_.ART_M_POSITION_ID
                                                 select new KPI_REPORT_MODEL
                                                 {
                                                     EMPLOYEE_ID_DISPLAY_TEXT = p.USERNAME,
                                                     EMPLOYEE_NAME_DISPLAY_TEXT = p.TITLE + p.FIRST_NAME + " " + p.LAST_NAME,
                                                     POSITION_DISPLAY_TEXT = position_.ART_M_POSITION_NAME,
                                                     USERID = p.USER_ID
                                                 }).Distinct().ToList();
                            }

                            //foreach (คน)
                            foreach (var userid in EmployeeInDep)
                            {
                                double AVG1 = 0;
                                double AVG2 = 0;
                                var monthStart = dateFrom.Month;
                                var monthEnd = dateTo.Month;

                                var temp = new KPI_REPORT_MODEL();
                                temp.EMPLOYEE_ID_DISPLAY_TEXT = userid.EMPLOYEE_ID_DISPLAY_TEXT;
                                temp.EMPLOYEE_NAME_DISPLAY_TEXT = userid.EMPLOYEE_NAME_DISPLAY_TEXT;
                                temp.POSITION_DISPLAY_TEXT = userid.POSITION_DISPLAY_TEXT;

                                DateTime createdateto = new DateTime();

                                var CounterMonth1 = 0;
                                var CounterMonth2 = 0;
                                double Total6_1 = 0;
                                double Total6_2 = 0;
                                for (int i = 0; i <= difdate; i++)
                                {

                                    var createdatefrom = dateFrom.AddMonths(i);
                                    if (i > 0)
                                        createdatefrom = createdateto.AddSeconds(1);
                                    else
                                        createdatefrom = dateFrom.AddMonths(i).AddDays(-8).AddSeconds(-1);
                                    createdateto = dateFrom.AddMonths(i + 1).AddDays(-7).AddSeconds(-1);

                                    var weekends = 0;

                                    weekends = CNService.GetBusinessDays(createdateto, createdateto.AddDays(7));
                                    if (weekends != 7)
                                    {
                                        createdateto = createdateto.AddDays(-7 + weekends);
                                        if (createdateto.DayOfWeek == DayOfWeek.Saturday)
                                        {
                                            createdateto = createdateto.AddDays(-1);
                                        }
                                        else if (createdateto.DayOfWeek == DayOfWeek.Sunday)
                                        {
                                            createdateto = createdateto.AddDays(-2);
                                        }
                                    }

                                    weekends = 0;

                                    weekends = CNService.GetBusinessDays(createdatefrom, createdatefrom.AddDays(8));
                                    if (weekends != 7)
                                    {
                                        createdatefrom = createdatefrom.AddDays(-7 + weekends);
                                        if (createdatefrom.DayOfWeek == DayOfWeek.Saturday)
                                        {
                                            createdatefrom = new DateTime(createdatefrom.Year, createdatefrom.Month, createdatefrom.Day, 23, 59, 59);
                                            createdatefrom = createdatefrom.AddDays(-1);
                                        }
                                        else if (createdatefrom.DayOfWeek == DayOfWeek.Sunday)
                                        {
                                            createdatefrom = new DateTime(createdatefrom.Year, createdatefrom.Month, createdatefrom.Day, 23, 59, 59);
                                            createdatefrom = createdatefrom.AddDays(-2);
                                        }
                                        else if (i == 0)
                                            createdatefrom = createdatefrom.AddSeconds(1);
                                    }


                                    var Terminate_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                                      where m.IS_TERMINATE != null && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                      select m.MOCKUP_ID).Distinct().ToList();

                                    var Completed_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                                      where m.REMARK_KILLPROCESS == "Completed workflow by PG" && (m.CURRENT_STEP_ID == StepCusApp || m.CURRENT_STEP_ID == StepMKApp) && m.CREATE_BY == userid.USERID && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                      group m by new { m.MOCKUP_ID, m.REMARK } into m3
                                                      select m3.Key.MOCKUP_ID).ToList();

                                    var allMockup_ = (from m in context.ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG
                                                      join m3 in context.ART_WF_MOCKUP_PROCESS on m.MOCKUP_SUB_ID equals m3.MOCKUP_SUB_ID
                                                      where !Terminate_.Contains(m3.MOCKUP_ID) && m3.REMARK_KILLPROCESS == null && m.CREATE_BY == userid.USERID && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto && m3.UPDATE_BY != -1
                                                      select m3).ToList();

                                    var Complete_no_ = (from m3 in Completed_
                                                join m4 in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on m3 equals m4.MOCKUP_ID
                                                orderby m4.MOCKUP_NO
                                                select m4.MOCKUP_NO).ToList();

                                    temp.LIST_ALL_WF_NO = (from m in allMockup_
                                                           join m2 in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on m.MOCKUP_ID equals m2.MOCKUP_ID
                                                           orderby m2.MOCKUP_NO
                                                           select m2.MOCKUP_NO).ToList();

                                    foreach (var e in Complete_no_)
                                        temp.LIST_ALL_WF_NO.Add(e);

                                    double allMockup = allMockup_.Count() + Completed_.Count();

                                    var Incorrect_ = (from m in context.ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG
                                                      join m2 in context.ART_WF_MOCKUP_PROCESS_CUSTOMER on m.MOCKUP_SUB_ID equals m2.MOCKUP_SUB_ID
                                                      join m3 in context.ART_WF_MOCKUP_PROCESS on m.MOCKUP_SUB_ID equals m3.MOCKUP_SUB_ID
                                                      where !Terminate_.Contains(m3.MOCKUP_ID) && m3.REMARK_KILLPROCESS == null && m.CREATE_BY == userid.USERID && m2.ACTION_CODE == "SUBMIT" && (m2.DECISION == "REVISE" || m2.DECISION == "CANCEL") && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto && m3.UPDATE_BY != -1
                                                      select m3).ToList();

                                    temp.LIST_CORRECT_WF_NO = (from m in Incorrect_
                                                               join m2 in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on m.MOCKUP_ID equals m2.MOCKUP_ID
                                                               orderby m2.MOCKUP_NO
                                                               select m2.MOCKUP_NO).ToList();

                                    double Incorrect = Incorrect_.Count();

                                    double tempCal = Math.Round((Incorrect * 100 / allMockup) * 100) / 100;
                                    if (Double.IsNaN(tempCal))
                                        tempCal = 0;

                                    if (i == 0) temp.Month1 = "" + tempCal + "";
                                    else if (i == 1) temp.Month2 = "" + tempCal + "";
                                    else if (i == 2) temp.Month3 = "" + tempCal + "";
                                    else if (i == 3) temp.Month4 = "" + tempCal + "";
                                    else if (i == 4) temp.Month5 = "" + tempCal + "";
                                    else if (i == 5) temp.Month6 = "" + tempCal + "";
                                    else if (i == 6) temp.Month7 = "" + tempCal + "";
                                    else if (i == 7) temp.Month8 = "" + tempCal + "";
                                    else if (i == 8) temp.Month9 = "" + tempCal + "";
                                    else if (i == 9) temp.Month10 = "" + tempCal + "";
                                    else if (i == 10) temp.Month11 = "" + tempCal + "";
                                    else if (i == 11) temp.Month12 = "" + tempCal + "";

                                    if (i <= 5 && (tempCal != 0 || allMockup > 0))
                                    {
                                        CounterMonth1++;
                                        Total6_1 += tempCal;
                                    }
                                    else if (i > 5 && (tempCal != 0 || allMockup > 0))
                                    {
                                        CounterMonth2++;
                                        Total6_2 += tempCal;
                                    }

                                }
                                if (CounterMonth1 != 0)
                                    AVG1 += Math.Round((Total6_1 / CounterMonth1) * 100) / 100;
                                else if (CounterMonth2 != 0)
                                    AVG2 += Math.Round((Total6_2 / CounterMonth2) * 100) / 100;



                                if (AVG1 != 0 || CounterMonth1 != 0)
                                {
                                    temp.GRADE1 = AVG1 <= param.data.SCORE5 ? "5" : AVG1 <= param.data.SCORE4 ? "4" : AVG1 <= param.data.SCORE3 ? "3" : AVG1 <= param.data.SCORE2 ? "2" : AVG1 > param.data.SCORE1 ? "1" : "0";
                                    //temp.GRADE1 = AVG1 <= 0.50 ? "5" : AVG1 <= 3.50 ? "4" : AVG1 <= 6.49 ? "3" : AVG1 <= 9.49 ? "2" : AVG1 > 9.49 ? "1" : "0";
                                    temp.AVG1 = "" + AVG1 + "";
                                }
                                if (AVG2 != 0 || CounterMonth2 != 0)
                                {
                                    temp.GRADE2 = AVG2 <= param.data.SCORE5 ? "5" : AVG2 <= param.data.SCORE4 ? "4" : AVG2 <= param.data.SCORE3 ? "3" : AVG2 <= param.data.SCORE2 ? "2" : AVG2 > param.data.SCORE1 ? "1" : "0";
                                    //temp.GRADE2 = AVG2 <= 0.50 ? "5" : AVG2 <= 3.50 ? "4" : AVG2 <= 6.49 ? "3" : AVG2 <= 9.49 ? "2" : AVG2 > 9.49 ? "1" : "0";
                                    temp.AVG2 = "" + AVG2 + "";
                                }
                                temp.MONTH_FROM = dateFrom.Month + "/" + dateFrom.Day + "/" + dateFrom.Year;
                                temp.TARGET = 0.5;
                                res.data.Add(temp);
                            }
                        }
                        else if (param.data.KPI_TYPE == "mockstand")
                        {
                            //var Duration_PG = 0;
                            //Duration_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().DURATION;

                            var StepPG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                            var StepMK_UPD = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP { STEP_MOCKUP_CODE = "SEND_MK_UPD_PACK_STYLE" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                            var StepPN = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP { STEP_MOCKUP_CODE = "SEND_PN_PRI_PKG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                            var StepRD = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP { STEP_MOCKUP_CODE = "SEND_RD_PRI_PKG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                            var StepCusApp = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP { STEP_MOCKUP_CODE = "SEND_CUS_APP" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                            var StepMKApp = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP { STEP_MOCKUP_CODE = "SEND_MK_APP" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                            var StepSendBack = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP { STEP_MOCKUP_CODE = "SEND_BACK_MK" }, context).FirstOrDefault().STEP_MOCKUP_ID;

                            if (param.data.USERID != -1)
                                EmployeeInDep = (from m in context.ART_M_POSITION_ROLE
                                                 join m3 in context.ART_M_POSITION on m.POSITION_ID equals m3.ART_M_POSITION_ID
                                                 join m4 in context.ART_M_USER on m.POSITION_ID equals m4.POSITION_ID
                                                 where m4.USER_ID == param.data.USERID
                                                 select new KPI_REPORT_MODEL
                                                 {
                                                     EMPLOYEE_ID_DISPLAY_TEXT = m4.USERNAME,
                                                     EMPLOYEE_NAME_DISPLAY_TEXT = m4.TITLE + m4.FIRST_NAME + " " + m4.LAST_NAME,
                                                     POSITION_DISPLAY_TEXT = m3.ART_M_POSITION_NAME,
                                                     USERID = m4.USER_ID
                                                 }).Distinct().ToList();
                            else
                            {
                                var role_param = (from p in context.ART_M_ROLE
                                                  where p.ROLE_CODE.StartsWith("PG_")
                                                  select p.ROLE_ID).ToList();

                                var userRolePA = (from p in context.ART_M_USER_ROLE
                                                  where role_param.Contains(p.ROLE_ID)
                                                  select p.USER_ID).ToList();


                                EmployeeInDep = (from p in context.ART_M_USER
                                                 where userRolePA.Contains(p.USER_ID) && p.IS_ACTIVE == "X" && p.POSITION_ID == position_.ART_M_POSITION_ID
                                                 select new KPI_REPORT_MODEL
                                                 {
                                                     EMPLOYEE_ID_DISPLAY_TEXT = p.USERNAME,
                                                     EMPLOYEE_NAME_DISPLAY_TEXT = p.TITLE + p.FIRST_NAME + " " + p.LAST_NAME,
                                                     POSITION_DISPLAY_TEXT = position_.ART_M_POSITION_NAME,
                                                     USERID = p.USER_ID
                                                 }).Distinct().ToList();
                            }

                            //foreach (คน)
                            foreach (var userid in EmployeeInDep)
                            {
                                double AVG1 = 0;
                                double AVG2 = 0;
                                var monthStart = dateFrom.Month;
                                var monthEnd = dateTo.Month;

                                var temp = new KPI_REPORT_MODEL();
                                temp.EMPLOYEE_ID_DISPLAY_TEXT = userid.EMPLOYEE_ID_DISPLAY_TEXT;
                                temp.EMPLOYEE_NAME_DISPLAY_TEXT = userid.EMPLOYEE_NAME_DISPLAY_TEXT;
                                temp.POSITION_DISPLAY_TEXT = userid.POSITION_DISPLAY_TEXT;

                                DateTime createdateto = new DateTime();

                                var CounterMonth1 = 0;
                                var CounterMonth2 = 0;
                                double Total6_1 = 0;
                                double Total6_2 = 0;
                                for (int i = 0; i <= difdate; i++)
                                {

                                    var createdatefrom = dateFrom.AddMonths(i);
                                    if (i > 0)
                                        createdatefrom = createdateto.AddSeconds(1);
                                    else
                                        createdatefrom = dateFrom.AddMonths(i).AddDays(-6).AddSeconds(-1);
                                    createdateto = dateFrom.AddMonths(i + 1).AddDays(-5).AddSeconds(-1);

                                    var weekends = 0;

                                    weekends = CNService.GetBusinessDays(createdateto, createdateto.AddDays(5));
                                    if (weekends != 5)
                                    {
                                        createdateto = createdateto.AddDays(-5 + weekends);
                                        if (createdateto.DayOfWeek == DayOfWeek.Saturday)
                                        {
                                            createdateto = createdateto.AddDays(-1);
                                        }
                                        else if (createdateto.DayOfWeek == DayOfWeek.Sunday)
                                        {
                                            createdateto = createdateto.AddDays(-2);
                                        }
                                    }


                                    weekends = 0;

                                    weekends = CNService.GetBusinessDays(createdatefrom, createdatefrom.AddDays(6));
                                    if (weekends != 5)
                                    {
                                        createdatefrom = createdatefrom.AddDays(-5 + weekends);
                                        if (createdatefrom.DayOfWeek == DayOfWeek.Saturday)
                                        {
                                            createdatefrom = new DateTime(createdatefrom.Year, createdatefrom.Month, createdatefrom.Day, 23, 59, 59);
                                            createdatefrom = createdatefrom.AddDays(-1);
                                        }
                                        else if (createdatefrom.DayOfWeek == DayOfWeek.Sunday)
                                        {
                                            createdatefrom = new DateTime(createdatefrom.Year, createdatefrom.Month, createdatefrom.Day, 23, 59, 59);
                                            createdatefrom = createdatefrom.AddDays(-2);
                                        }
                                        else if (i == 0)
                                            createdatefrom = createdatefrom.AddSeconds(1);
                                    }

                                    var ExtendMockup = (from m in context.ART_WF_MOCKUP_PROCESS
                                                        where m.IS_TERMINATE == null && m.REMARK_KILLPROCESS == null && (m.CURRENT_USER_ID == userid.USERID || m.CREATE_BY == userid.USERID || m.CURRENT_STEP_ID == StepMK_UPD) && m.IS_STEP_DURATION_EXTEND != null
                                                       && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                        select m.MOCKUP_ID).Distinct().ToList();

                                    var CheckStep = (from m in context.ART_WF_MOCKUP_PROCESS
                                                     where m.IS_TERMINATE == null && m.REMARK_KILLPROCESS == null && (m.CURRENT_STEP_ID == StepPN || m.CURRENT_STEP_ID == StepRD) && m.IS_END == null && m.IS_STEP_DURATION_EXTEND != null
                                                    && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                     select m.MOCKUP_ID).Distinct().ToList();

                                    var SpecialPN = (from m in context.ART_WF_MOCKUP_PROCESS_PLANNING
                                                     join m3 in context.ART_WF_MOCKUP_PROCESS on m.MOCKUP_ID equals m3.MOCKUP_ID
                                                     where m3.IS_TERMINATE == null && m3.REMARK_KILLPROCESS == null && m3.IS_STEP_DURATION_EXTEND == null && m.ACTION_CODE != "SAVE"
                                                       && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                     select m.MOCKUP_ID).Distinct().ToList();

                                    var SpecialRD = (from m in context.ART_WF_MOCKUP_PROCESS_RD
                                                     join m3 in context.ART_WF_MOCKUP_PROCESS on m.MOCKUP_ID equals m3.MOCKUP_ID
                                                     where m3.IS_TERMINATE == null && m3.REMARK_KILLPROCESS == null && m3.IS_STEP_DURATION_EXTEND == null && m.ACTION_CODE != "SAVE"
                                                       && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                     select m.MOCKUP_ID).Distinct().ToList();

                                    var createdatefrom_terminate = createdatefrom.AddMonths(-1);
                                    var Terminate_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                                      where m.IS_TERMINATE != null && m.CREATE_DATE > createdatefrom_terminate && m.CREATE_DATE <= createdateto
                                                      select m.MOCKUP_ID).Distinct().ToList();

                                    //var CusRevise = (from m in context.ART_WF_MOCKUP_PROCESS_CUSTOMER
                                    //                 join m1 in context.ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG on m.MOCKUP_SUB_ID equals m1.MOCKUP_SUB_ID
                                    //                 join m2 in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on m.MOCKUP_ID equals m2.MOCKUP_ID
                                    //                 join m3 in context.ART_WF_MOCKUP_PROCESS on m.MOCKUP_ID equals m3.MOCKUP_ID
                                    //                 where m1.CREATE_BY == userid.USERID && m.DECISION == "REVISE" && m3.IS_TERMINATE == null && m3.REMARK_KILLPROCESS == null && m3.CURRENT_STEP_ID == StepPG && (m3.CREATE_BY == userid.USERID || m3.CURRENT_USER_ID == userid.USERID) && m3.IS_STEP_DURATION_EXTEND == null && (m2.MOCKUP_NO.Contains("MO-N") || m2.MOCKUP_NO.Contains("MO-P"))
                                    //                 && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                    //                 select m.MOCKUP_ID).ToList();

                                    var allMockup_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                                      join m2 in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on m.MOCKUP_ID equals m2.MOCKUP_ID
                                                      where !Terminate_.Contains(m.MOCKUP_ID) && m.REMARK_KILLPROCESS == null && m.CURRENT_STEP_ID == StepPG && m.CURRENT_USER_ID == userid.USERID && m.IS_STEP_DURATION_EXTEND == null && (m2.MOCKUP_NO.Contains("MO-N") || m2.MOCKUP_NO.Contains("MO-P")) && m.IS_END == "X"
                                                      && !ExtendMockup.Contains(m.MOCKUP_ID) && !CheckStep.Contains(m.MOCKUP_ID) && ((SpecialPN.Contains(m.MOCKUP_ID) || SpecialRD.Contains(m.MOCKUP_ID)) || (m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto))
                                                      select new ART_WF_MOCKUP_PROCESS_2
                                                      {
                                                          MOCKUP_ID = m.MOCKUP_ID,
                                                          MOCKUP_SUB_ID = m.MOCKUP_SUB_ID
                                                      }).Distinct().ToList();

                                    temp.LIST_ALL_WF_NO = (from m in allMockup_
                                                           join m1 in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on m.MOCKUP_ID equals m1.MOCKUP_ID
                                                           orderby m1.MOCKUP_NO
                                                           select m1.MOCKUP_NO).ToList();

                                    var is_reassign = false;
                                    foreach (var e in allMockup_)
                                    {
                                        var reassign = ART_WF_LOG_REASSIGN_SERVICE.GetByItem(new ART_WF_LOG_REASSIGN { WF_SUB_ID = e.MOCKUP_SUB_ID, WF_TYPE = "M", STEP_ID = StepPG }, context).ToList();
                                        if (reassign.Count != 0)
                                            is_reassign = true;
                                    }

                                    //double allMockup = allMockup_.Count() + CusRevise.Count();
                                    double allMockup = allMockup_.Count();

                                    #region Cusrevise
                                    //var CusReviseCorrect_ = (from m in context.ART_WF_MOCKUP_PROCESS_CUSTOMER
                                    //                         join m1 in context.ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG on m.MOCKUP_SUB_ID equals m1.MOCKUP_SUB_ID
                                    //                         join m2 in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on m.MOCKUP_ID equals m2.MOCKUP_ID
                                    //                         join m3 in context.ART_WF_MOCKUP_PROCESS on m.MOCKUP_ID equals m3.MOCKUP_ID
                                    //                         where m1.CREATE_BY == userid.USERID && m.DECISION == "REVISE" && m3.IS_TERMINATE == null && m3.REMARK_KILLPROCESS == null && m3.CURRENT_STEP_ID == StepPG && (m3.CREATE_BY == userid.USERID || m3.CURRENT_USER_ID == userid.USERID) && m3.IS_STEP_DURATION_EXTEND == null && (m2.MOCKUP_NO.Contains("MO-N") || m2.MOCKUP_NO.Contains("MO-P"))
                                    //                        && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                    //                         select new ART_WF_MOCKUP_PROCESS_2
                                    //                         {
                                    //                             MOCKUP_ID = m.MOCKUP_ID,
                                    //                             CREATE_DATE = m3.CREATE_DATE,
                                    //                             UPDATE_DATE = m.UPDATE_DATE
                                    //                         }).ToList();

                                    //foreach (var e in CusReviseCorrect_)
                                    //{
                                    //    var duration = ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(StepPG, context).DURATION;

                                    //    var addDuration = 0;

                                    //    var PN_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                    //               join m2 in context.ART_WF_MOCKUP_PROCESS_PLANNING on m.MOCKUP_SUB_ID equals m2.MOCKUP_SUB_ID
                                    //               where m.MOCKUP_ID == e.MOCKUP_ID && m.CURRENT_STEP_ID == StepPN && m.IS_TERMINATE == null && m.REMARK_KILLPROCESS == null
                                    //               && m2.ACTION_CODE == "SEND_PRI"
                                    //               select new ART_WF_MOCKUP_PROCESS_2
                                    //               {
                                    //                   MOCKUP_SUB_ID = m.MOCKUP_SUB_ID,
                                    //                   CREATE_DATE = m.CREATE_DATE,
                                    //                   UPDATE_DATE = m2.CREATE_DATE
                                    //               }).ToList();

                                    //    var RD_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                    //               join m3 in context.ART_WF_MOCKUP_PROCESS_RD on m.MOCKUP_SUB_ID equals m3.MOCKUP_SUB_ID
                                    //               where m.MOCKUP_ID == e.MOCKUP_ID && m.CURRENT_STEP_ID == StepRD && m.IS_TERMINATE == null && m.REMARK_KILLPROCESS == null
                                    //               && m3.ACTION_CODE == "SEND_PRI"
                                    //               select new ART_WF_MOCKUP_PROCESS_2
                                    //               {
                                    //                   MOCKUP_SUB_ID = m.MOCKUP_SUB_ID,
                                    //                   CREATE_DATE = m.CREATE_DATE,
                                    //                   UPDATE_DATE = m3.CREATE_DATE
                                    //               }).ToList();

                                    //    var CusApp_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                    //                   join m3 in context.ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG on m.MOCKUP_SUB_ID equals m3.MOCKUP_SUB_ID
                                    //                   where m.MOCKUP_ID == e.MOCKUP_ID && m.CURRENT_STEP_ID == StepCusApp && m.IS_TERMINATE == null && m.REMARK_KILLPROCESS == null
                                    //                   && m3.IS_SEND_DIE_LINE == "X"
                                    //                   select new ART_WF_MOCKUP_PROCESS_2
                                    //                   {
                                    //                       MOCKUP_SUB_ID = m.MOCKUP_SUB_ID,
                                    //                       CREATE_DATE = m.CREATE_DATE,
                                    //                       UPDATE_DATE = m3.CREATE_DATE
                                    //                   }).ToList();

                                    //    foreach (var e1 in PN_)
                                    //    {
                                    //        var start = new DateTime(e1.CREATE_DATE.Year, e1.CREATE_DATE.Month, e1.CREATE_DATE.Day, 0, 0, 0);
                                    //        var end = new DateTime(e1.UPDATE_DATE.Year, e1.UPDATE_DATE.Month, e1.UPDATE_DATE.Day, 0, 0, 0);
                                    //        addDuration += CNService.GetBusinessDays(start, end.AddDays(1));
                                    //        //addDuration += CNService.GetBusinessDays(e1.CREATE_DATE, e1.UPDATE_DATE);
                                    //    }

                                    //    foreach (var e1 in RD_)
                                    //    {
                                    //        var start = new DateTime(e1.CREATE_DATE.Year, e1.CREATE_DATE.Month, e1.CREATE_DATE.Day, 0, 0, 0);
                                    //        var end = new DateTime(e1.UPDATE_DATE.Year, e1.UPDATE_DATE.Month, e1.UPDATE_DATE.Day, 0, 0, 0);
                                    //        addDuration += CNService.GetBusinessDays(start, end.AddDays(1));
                                    //        //addDuration += CNService.GetBusinessDays(e1.CREATE_DATE, e1.UPDATE_DATE);
                                    //    }

                                    //    foreach (var e1 in CusApp_)
                                    //    {
                                    //        var start = new DateTime(e1.CREATE_DATE.Year, e1.CREATE_DATE.Month, e1.CREATE_DATE.Day, 0, 0, 0);
                                    //        var end = new DateTime(e1.UPDATE_DATE.Year, e1.UPDATE_DATE.Month, e1.UPDATE_DATE.Day, 0, 0, 0);
                                    //        addDuration += CNService.GetBusinessDays(start, end.AddDays(1));
                                    //    }
                                    //    e.DUEDATE = CNService.AddBusinessDays(e.CREATE_DATE, (int)Math.Ceiling(5.00 + addDuration));
                                    //}

                                    //var correct_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                    //                join m3 in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on m.MOCKUP_ID equals m3.MOCKUP_ID
                                    //                join m2 in context.ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG on m.MOCKUP_ID equals m2.MOCKUP_ID
                                    //                where m2.CREATE_BY == userid.USERID && m.IS_TERMINATE == null && m.REMARK_KILLPROCESS == null && m.CURRENT_STEP_ID == StepPG && m2.UPDATE_DATE != null && m.CURRENT_USER_ID == userid.USERID && m2.IS_SEND_DIE_LINE == "X" && m.IS_STEP_DURATION_EXTEND == null && (m3.MOCKUP_NO.Contains("MO-N") || m3.MOCKUP_NO.Contains("MO-P"))
                                    //                && !ExtendMockup.Contains(m.MOCKUP_ID) && !CheckStep.Contains(m.MOCKUP_ID) && ((SpecialPN.Contains(m.MOCKUP_ID) || SpecialRD.Contains(m.MOCKUP_ID)) || (m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto))
                                    //                group new { m, m2 } by new { m.MOCKUP_ID } into m3
                                    //                let min_m = m3.Min(x => x.m.CREATE_DATE)
                                    //                let min_m2 = m3.Min(x => x.m2.CREATE_DATE)
                                    //                select new ART_WF_MOCKUP_PROCESS_2
                                    //                {
                                    //                    MOCKUP_ID = m3.Key.MOCKUP_ID,
                                    //                    CREATE_DATE = min_m,
                                    //                    UPDATE_DATE = min_m2,
                                    //                }).Distinct().ToList();
                                    #endregion

                                    var correct_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                                    join m3 in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on m.MOCKUP_ID equals m3.MOCKUP_ID
                                                    where m.IS_END == "X" && m.CURRENT_USER_ID == userid.USERID && !Terminate_.Contains(m.MOCKUP_ID) && m.REMARK_KILLPROCESS == null && m.CURRENT_STEP_ID == StepPG && m.IS_STEP_DURATION_EXTEND == null && (m3.MOCKUP_NO.Contains("MO-N") || m3.MOCKUP_NO.Contains("MO-P"))
                                                    && !ExtendMockup.Contains(m.MOCKUP_ID) && !CheckStep.Contains(m.MOCKUP_ID) && ((SpecialPN.Contains(m.MOCKUP_ID) || SpecialRD.Contains(m.MOCKUP_ID)) || (m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto))
                                                    select new ART_WF_MOCKUP_PROCESS_2
                                                    {
                                                        MOCKUP_ID = m.MOCKUP_ID,
                                                        CREATE_DATE = m.CREATE_DATE,
                                                        UPDATE_DATE = m.UPDATE_DATE
                                                    }).ToList();

                                    List<int> SEND_BACK_WF = new List<int>();

                                    foreach (var e in correct_)
                                    {
                                        //if (e.MOCKUP_ID == 4662)
                                        //    e.MOCKUP_ID = 4662;
                                        var duration = ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(StepPG, context).DURATION;

                                        var addDuration = 0;

                                        var PN_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                                   join m2 in context.ART_WF_MOCKUP_PROCESS_PLANNING on m.MOCKUP_SUB_ID equals m2.MOCKUP_SUB_ID
                                                   where m.MOCKUP_ID == e.MOCKUP_ID && m.CURRENT_STEP_ID == StepPN && m.IS_TERMINATE == null && m.REMARK_KILLPROCESS == null && m.UPDATE_BY != -1
                                                   && m2.ACTION_CODE != "SAVE"
                                                   select new ART_WF_MOCKUP_PROCESS_2
                                                   {
                                                       MOCKUP_SUB_ID = m.MOCKUP_SUB_ID,
                                                       CREATE_DATE = m.CREATE_DATE,
                                                       UPDATE_DATE = m2.CREATE_DATE
                                                   }).ToList();

                                        var RD_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                                   join m3 in context.ART_WF_MOCKUP_PROCESS_RD on m.MOCKUP_SUB_ID equals m3.MOCKUP_SUB_ID
                                                   where m.MOCKUP_ID == e.MOCKUP_ID && m.CURRENT_STEP_ID == StepRD && m.IS_TERMINATE == null && m.REMARK_KILLPROCESS == null && m.UPDATE_BY != -1
                                                   && m3.ACTION_CODE != "SAVE"
                                                   select new ART_WF_MOCKUP_PROCESS_2
                                                   {
                                                       MOCKUP_SUB_ID = m.MOCKUP_SUB_ID,
                                                       CREATE_DATE = m.CREATE_DATE,
                                                       UPDATE_DATE = m3.CREATE_DATE
                                                   }).ToList();

                                        var CusApp_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                                       join m3 in context.ART_WF_MOCKUP_PROCESS_CUSTOMER on m.MOCKUP_SUB_ID equals m3.MOCKUP_SUB_ID
                                                       where m.MOCKUP_ID == e.MOCKUP_ID && (m.CURRENT_STEP_ID == StepCusApp || m.CURRENT_STEP_ID == StepMKApp) && m.IS_TERMINATE == null && m.REMARK_KILLPROCESS == null && m.UPDATE_BY != -1
                                                       select new ART_WF_MOCKUP_PROCESS_2
                                                       {
                                                           MOCKUP_SUB_ID = m.MOCKUP_SUB_ID,
                                                           CREATE_DATE = m.CREATE_DATE,
                                                           UPDATE_DATE = m3.CREATE_DATE
                                                       }).ToList();

                                        var SendBack_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                                         join m3 in context.ART_WF_MOCKUP_PROCESS_CUSTOMER on m.MOCKUP_SUB_ID equals m3.MOCKUP_SUB_ID
                                                         where m.MOCKUP_ID == e.MOCKUP_ID && m.CURRENT_STEP_ID == StepSendBack && m.IS_TERMINATE == null && m.REMARK_KILLPROCESS == null
                                                         select new ART_WF_MOCKUP_PROCESS_2
                                                         {
                                                             MOCKUP_ID = m.MOCKUP_ID,
                                                             MOCKUP_SUB_ID = m.MOCKUP_SUB_ID,
                                                             CREATE_DATE = m.CREATE_DATE,
                                                             UPDATE_DATE = m3.CREATE_DATE
                                                         }).ToList();

                                        foreach (var e1 in PN_)
                                        {
                                            var start = new DateTime(e1.CREATE_DATE.Year, e1.CREATE_DATE.Month, e1.CREATE_DATE.Day, 0, 0, 0);
                                            var end = new DateTime(e1.UPDATE_DATE.Year, e1.UPDATE_DATE.Month, e1.UPDATE_DATE.Day, 0, 0, 0);
                                            if (start.DayOfWeek == DayOfWeek.Saturday)
                                            {
                                                start = start.AddDays(-1);
                                            }
                                            else if (start.DayOfWeek == DayOfWeek.Sunday)
                                            {
                                                start = start.AddDays(-2);
                                            }
                                            if (end.DayOfWeek == DayOfWeek.Saturday)
                                            {
                                                end = end.AddDays(2);
                                            }
                                            else if (end.DayOfWeek == DayOfWeek.Sunday)
                                            {
                                                end = end.AddDays(1);
                                            }
                                            addDuration += CNService.GetBusinessDays(start, end.AddDays(1));
                                            //addDuration += CNService.GetBusinessDays(e1.CREATE_DATE, e1.UPDATE_DATE);
                                        }

                                        foreach (var e1 in RD_)
                                        {
                                            var start = new DateTime(e1.CREATE_DATE.Year, e1.CREATE_DATE.Month, e1.CREATE_DATE.Day, 0, 0, 0);
                                            var end = new DateTime(e1.UPDATE_DATE.Year, e1.UPDATE_DATE.Month, e1.UPDATE_DATE.Day, 0, 0, 0);
                                            if (start.DayOfWeek == DayOfWeek.Saturday)
                                            {
                                                start = start.AddDays(-1);
                                            }
                                            else if (start.DayOfWeek == DayOfWeek.Sunday)
                                            {
                                                start = start.AddDays(-2);
                                            }
                                            if (end.DayOfWeek == DayOfWeek.Saturday)
                                            {
                                                end = end.AddDays(2);
                                            }
                                            else if (end.DayOfWeek == DayOfWeek.Sunday)
                                            {
                                                end = end.AddDays(1);
                                            }
                                            addDuration += CNService.GetBusinessDays(start, end.AddDays(1));
                                            //addDuration += CNService.GetBusinessDays(e1.CREATE_DATE, e1.UPDATE_DATE);
                                        }

                                        foreach (var e1 in CusApp_)
                                        {
                                            var start = new DateTime(e1.CREATE_DATE.Year, e1.CREATE_DATE.Month, e1.CREATE_DATE.Day, 0, 0, 0);
                                            var end = new DateTime(e1.UPDATE_DATE.Year, e1.UPDATE_DATE.Month, e1.UPDATE_DATE.Day, 0, 0, 0);
                                            if (start.DayOfWeek == DayOfWeek.Saturday)
                                            {
                                                start = start.AddDays(-1);
                                            }
                                            else if (start.DayOfWeek == DayOfWeek.Sunday)
                                            {
                                                start = start.AddDays(-2);
                                            }
                                            if (end.DayOfWeek == DayOfWeek.Saturday)
                                            {
                                                end = end.AddDays(2);
                                            }
                                            else if (end.DayOfWeek == DayOfWeek.Sunday)
                                            {
                                                end = end.AddDays(1);
                                            }
                                            addDuration += CNService.GetBusinessDays(start, end.AddDays(1));
                                        }

                                        foreach (var e1 in SendBack_)
                                        {
                                            SEND_BACK_WF.Add(e1.MOCKUP_ID);
                                            var start = new DateTime(e1.CREATE_DATE.Year, e1.CREATE_DATE.Month, e1.CREATE_DATE.Day, 0, 0, 0);
                                            var end = new DateTime(e1.UPDATE_DATE.Year, e1.UPDATE_DATE.Month, e1.UPDATE_DATE.Day, 0, 0, 0);
                                            if (start.DayOfWeek == DayOfWeek.Saturday)
                                            {
                                                start = start.AddDays(-1);
                                            }
                                            else if (start.DayOfWeek == DayOfWeek.Sunday)
                                            {
                                                start = start.AddDays(-2);
                                            }
                                            if (end.DayOfWeek == DayOfWeek.Saturday)
                                            {
                                                end = end.AddDays(2);
                                            }
                                            else if (end.DayOfWeek == DayOfWeek.Sunday)
                                            {
                                                end = end.AddDays(1);
                                            }
                                            addDuration += CNService.GetBusinessDays(start, end.AddDays(1));
                                        }

                                        e.DUEDATE = CNService.AddBusinessDays(e.CREATE_DATE, (int)Math.Ceiling(5.00 + addDuration));
                                    }

                                    temp.LIST_CORRECT_WF_NO = (from m in correct_.Where(p => p.UPDATE_DATE <= p.DUEDATE)
                                                               join m2 in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on m.MOCKUP_ID equals m2.MOCKUP_ID
                                                               orderby m2.MOCKUP_NO
                                                               select m2.MOCKUP_NO).ToList();

                                    temp.LIST_SENDBACK_WF_NO = (from m in SEND_BACK_WF
                                                                join m2 in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on m equals m2.MOCKUP_ID
                                                                orderby m2.MOCKUP_NO
                                                                select m2.MOCKUP_NO).ToList();

                                    //double CusReviseCorrect = CusReviseCorrect_.Where(p => p.UPDATE_DATE <= p.DUEDATE).ToList().Count();
                                    //double correct = correct_.Where(p => p.UPDATE_DATE <= p.DUEDATE).ToList().Count() + CusReviseCorrect;
                                    double correct = correct_.Where(p => p.UPDATE_DATE <= p.DUEDATE).ToList().Count();

                                    double tempCal = Math.Round((correct * 100 / allMockup) * 100) / 100;
                                    if (Double.IsNaN(tempCal))
                                        tempCal = 0;

                                    if (i == 0) temp.Month1 = is_reassign ? param.data.GENERATE_EXCEL == "X" ? "" + tempCal + "*" : "" + tempCal + "<span title=\"Re-Assign\" style=\"color: red;\">*</span>" : "" + tempCal + "";
                                    else if (i == 1) temp.Month2 = is_reassign ? param.data.GENERATE_EXCEL == "X" ? "" + tempCal + "*" : "" + tempCal + "<span title=\"Re-Assign\" style=\"color: red;\">*</span>" : "" + tempCal + "";
                                    else if (i == 2) temp.Month3 = is_reassign ? param.data.GENERATE_EXCEL == "X" ? "" + tempCal + "*" : "" + tempCal + "<span title=\"Re-Assign\" style=\"color: red;\">*</span>" : "" + tempCal + "";
                                    else if (i == 3) temp.Month4 = is_reassign ? param.data.GENERATE_EXCEL == "X" ? "" + tempCal + "*" : "" + tempCal + "<span title=\"Re-Assign\" style=\"color: red;\">*</span>" : "" + tempCal + "";
                                    else if (i == 4) temp.Month5 = is_reassign ? param.data.GENERATE_EXCEL == "X" ? "" + tempCal + "*" : "" + tempCal + "<span title=\"Re-Assign\" style=\"color: red;\">*</span>" : "" + tempCal + "";
                                    else if (i == 5) temp.Month6 = is_reassign ? param.data.GENERATE_EXCEL == "X" ? "" + tempCal + "*" : "" + tempCal + "<span title=\"Re-Assign\" style=\"color: red;\">*</span>" : "" + tempCal + "";
                                    else if (i == 6) temp.Month7 = is_reassign ? param.data.GENERATE_EXCEL == "X" ? "" + tempCal + "*" : "" + tempCal + "<span title=\"Re-Assign\" style=\"color: red;\">*</span>" : "" + tempCal + "";
                                    else if (i == 7) temp.Month8 = is_reassign ? param.data.GENERATE_EXCEL == "X" ? "" + tempCal + "*" : "" + tempCal + "<span title=\"Re-Assign\" style=\"color: red;\">*</span>" : "" + tempCal + "";
                                    else if (i == 8) temp.Month9 = is_reassign ? param.data.GENERATE_EXCEL == "X" ? "" + tempCal + "*" : "" + tempCal + "<span title=\"Re-Assign\" style=\"color: red;\">*</span>" : "" + tempCal + "";
                                    else if (i == 9) temp.Month10 = is_reassign ? param.data.GENERATE_EXCEL == "X" ? "" + tempCal + "*" : "" + tempCal + "<span title=\"Re-Assign\" style=\"color: red;\">*</span>" : "" + tempCal + "";
                                    else if (i == 10) temp.Month11 = is_reassign ? param.data.GENERATE_EXCEL == "X" ? "" + tempCal + "*" : "" + tempCal + "<span title=\"Re-Assign\" style=\"color: red;\">*</span>" : "" + tempCal + "";
                                    else if (i == 11) temp.Month12 = is_reassign ? param.data.GENERATE_EXCEL == "X" ? "" + tempCal + "*" : "" + tempCal + "<span title=\"Re-Assign\" style=\"color: red;\">*</span>" : "" + tempCal + "";

                                    if (i <= 5 && (tempCal != 0 || allMockup > 0))
                                    {
                                        CounterMonth1++;
                                        Total6_1 += tempCal;
                                    }
                                    else if (i > 5 && (tempCal != 0 || allMockup > 0))
                                    {
                                        CounterMonth2++;
                                        Total6_2 += tempCal;
                                    }

                                }
                                if (CounterMonth1 != 0)
                                    AVG1 += Math.Round((Total6_1 / CounterMonth1) * 100) / 100;
                                else if (CounterMonth2 != 0)
                                    AVG2 += Math.Round((Total6_2 / CounterMonth2) * 100) / 100;


                                if (AVG1 != 0 || CounterMonth1 != 0)
                                {
                                    temp.AVG1 = "" + AVG1 + "";
                                    temp.GRADE1 = AVG1 >= param.data.SCORE5 ? "5" : AVG1 >= param.data.SCORE4 ? "4" : AVG1 >= param.data.SCORE3 ? "3" : AVG1 >= param.data.SCORE2 ? "2" : AVG1 < param.data.SCORE1 ? "1" : "0";
                                    //temp.GRADE1 = AVG1 >= 93.00 ? "5" : AVG1 >= 84.00 ? "4" : AVG1 >= 75.00 ? "3" : AVG1 >= 66.00 ? "2" : AVG1 < 66.00 ? "1" : "0";
                                }
                                if (AVG2 != 0 || CounterMonth2 != 0)
                                {
                                    temp.AVG2 = "" + AVG2 + "";
                                    temp.GRADE2 = AVG2 >= param.data.SCORE5 ? "5" : AVG2 >= param.data.SCORE4 ? "4" : AVG2 >= param.data.SCORE3 ? "3" : AVG2 >= param.data.SCORE2 ? "2" : AVG2 < param.data.SCORE1 ? "1" : "0";
                                    //temp.GRADE2 = AVG2 >= 93.00 ? "5" : AVG2 >= 84.00 ? "4" : AVG2 >= 75.00 ? "3" : AVG2 >= 66.00 ? "2" : AVG2 < 66.00 ? "1" : "0";
                                }
                                temp.MONTH_FROM = dateFrom.Month + "/" + dateFrom.Day + "/" + dateFrom.Year;
                                temp.TARGET = 80;
                                res.data.Add(temp);
                            }
                        }
                        else if (param.data.KPI_TYPE == "sendquostand")
                        {
                            var StepVN = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP { STEP_MOCKUP_CODE = "SEND_VN_QUO" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                            var StepPG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                            var StepNeedDesign = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP { STEP_MOCKUP_CODE = "SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                            var PKGTYPE_sticker = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(new SAP_M_CHARACTERISTIC { NAME = "ZPKG_SEC_GROUP", DESCRIPTION = "Sticker", VALUE = "J" }, context).FirstOrDefault().CHARACTERISTIC_ID;
                            var PKGTYPE_label = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(new SAP_M_CHARACTERISTIC { NAME = "ZPKG_SEC_GROUP", DESCRIPTION = "Label", VALUE = "K" }, context).FirstOrDefault().CHARACTERISTIC_ID;
                            var PKGTYPE_leaflet = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(new SAP_M_CHARACTERISTIC { NAME = "ZPKG_SEC_GROUP", DESCRIPTION = "Leaflet", VALUE = "L" }, context).FirstOrDefault().CHARACTERISTIC_ID;
                            var PKGTYPE_insertpaper = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(new SAP_M_CHARACTERISTIC { NAME = "ZPKG_SEC_GROUP", DESCRIPTION = "Insert Paper", VALUE = "P" }, context).FirstOrDefault().CHARACTERISTIC_ID;

                            if (param.data.USERID != -1)
                                EmployeeInDep = (from m in context.ART_M_POSITION_ROLE
                                                 join m3 in context.ART_M_POSITION on m.POSITION_ID equals m3.ART_M_POSITION_ID
                                                 join m4 in context.ART_M_USER on m.POSITION_ID equals m4.POSITION_ID
                                                 where m4.USER_ID == param.data.USERID
                                                 select new KPI_REPORT_MODEL
                                                 {
                                                     EMPLOYEE_ID_DISPLAY_TEXT = m4.USERNAME,
                                                     EMPLOYEE_NAME_DISPLAY_TEXT = m4.TITLE + m4.FIRST_NAME + " " + m4.LAST_NAME,
                                                     POSITION_DISPLAY_TEXT = m3.ART_M_POSITION_NAME,
                                                     USERID = m4.USER_ID
                                                 }).Distinct().ToList();
                            else
                            {
                                var role_param = (from p in context.ART_M_ROLE
                                                  where p.ROLE_CODE.StartsWith("PG_")
                                                  select p.ROLE_ID).ToList();

                                var userRolePA = (from p in context.ART_M_USER_ROLE
                                                  where role_param.Contains(p.ROLE_ID)
                                                  select p.USER_ID).ToList();


                                EmployeeInDep = (from p in context.ART_M_USER
                                                 where userRolePA.Contains(p.USER_ID) && p.IS_ACTIVE == "X" && p.POSITION_ID == position_.ART_M_POSITION_ID
                                                 select new KPI_REPORT_MODEL
                                                 {
                                                     EMPLOYEE_ID_DISPLAY_TEXT = p.USERNAME,
                                                     EMPLOYEE_NAME_DISPLAY_TEXT = p.TITLE + p.FIRST_NAME + " " + p.LAST_NAME,
                                                     POSITION_DISPLAY_TEXT = position_.ART_M_POSITION_NAME,
                                                     USERID = p.USER_ID
                                                 }).Distinct().ToList();
                            }

                            //foreach (คน)
                            foreach (var userid in EmployeeInDep)
                            {
                                double AVG1 = 0;
                                double AVG2 = 0;
                                var monthStart = dateFrom.Month;
                                var monthEnd = dateTo.Month;

                                var temp = new KPI_REPORT_MODEL();
                                temp.EMPLOYEE_ID_DISPLAY_TEXT = userid.EMPLOYEE_ID_DISPLAY_TEXT;
                                temp.EMPLOYEE_NAME_DISPLAY_TEXT = userid.EMPLOYEE_NAME_DISPLAY_TEXT;
                                temp.POSITION_DISPLAY_TEXT = userid.POSITION_DISPLAY_TEXT;

                                DateTime createdateto = new DateTime();

                                var CounterMonth1 = 0;
                                var CounterMonth2 = 0;
                                double Total6_1 = 0;
                                double Total6_2 = 0;
                                for (int i = 0; i <= difdate; i++)
                                {

                                    var createdatefrom = dateFrom.AddMonths(i);
                                    if (i > 0)
                                        createdatefrom = createdateto.AddSeconds(1);
                                    else
                                        createdatefrom = dateFrom.AddMonths(i).AddDays(-2).AddSeconds(-1);
                                    createdateto = dateFrom.AddMonths(i + 1).AddDays(-1).AddSeconds(-1);

                                    var weekends = 0;

                                    weekends = CNService.GetBusinessDays(createdateto, createdateto.AddDays(1));
                                    if (weekends != 1)
                                    {
                                        createdateto = createdateto.AddDays(-1 + weekends);
                                        if (createdateto.DayOfWeek == DayOfWeek.Saturday)
                                        {
                                            createdateto = createdateto.AddDays(-1);
                                        }
                                        else if (createdateto.DayOfWeek == DayOfWeek.Sunday)
                                        {
                                            createdateto = createdateto.AddDays(-2);
                                        }
                                    }


                                    weekends = 0;

                                    weekends = CNService.GetBusinessDays(createdatefrom, createdatefrom.AddDays(2));
                                    if (weekends != 1)
                                    {
                                        createdatefrom = createdatefrom.AddDays(-1 + weekends);
                                        if (createdatefrom.DayOfWeek == DayOfWeek.Saturday)
                                        {
                                            createdatefrom = new DateTime(createdatefrom.Year, createdatefrom.Month, createdatefrom.Day, 23, 59, 59);
                                            createdatefrom = createdatefrom.AddDays(-1);
                                        }
                                        else if (createdatefrom.DayOfWeek == DayOfWeek.Sunday)
                                        {
                                            createdatefrom = new DateTime(createdatefrom.Year, createdatefrom.Month, createdatefrom.Day, 23, 59, 59);
                                            createdatefrom = createdatefrom.AddDays(-2);
                                        }
                                        else if (i == 0)
                                            createdatefrom = createdatefrom.AddSeconds(1);
                                    }

                                    //compare = 0;
                                    //while (weekends != 1)
                                    //{
                                    //    createdatefrom = createdatefrom.AddDays(-compare);

                                    //    weekends = CNService.GetBusinessDays(createdatefrom.AddDays(-1), createdatefrom.AddDays(1 + compare));
                                    //    compare = 1 - weekends;
                                    //}
                                    //createdatefrom = createdatefrom.AddSeconds(1);


                                    var NeedDesign_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                                       where m.CURRENT_STEP_ID == StepNeedDesign && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                       select m.MOCKUP_ID).Distinct().ToList();


                                    var Terminate_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                                      where m.IS_TERMINATE != null && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                      select m.MOCKUP_ID).Distinct().ToList();

                                    var allMockup_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                                      join m2 in context.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE on m.MOCKUP_ID equals m2.MOCKUP_ID
                                                      where !NeedDesign_.Contains(m.MOCKUP_ID) && !Terminate_.Contains(m.MOCKUP_ID) && m.REMARK_KILLPROCESS == null && m.CURRENT_USER_ID == userid.USERID && m.CURRENT_STEP_ID == StepPG
                                                      && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                      select m.MOCKUP_ID).Distinct().ToList();

                                    temp.LIST_ALL_WF_NO = (from m in allMockup_
                                                           join m2 in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on m equals m2.MOCKUP_ID
                                                           orderby m2.MOCKUP_NO
                                                           select m2.MOCKUP_NO).ToList();


                                    double allMockup = allMockup_.Count();

                                    var Incorrect_normal_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                                             join m2 in context.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE on m.MOCKUP_ID equals m2.MOCKUP_ID
                                                             join m3 in context.ART_WF_MOCKUP_CHECK_LIST_PG on m.MOCKUP_ID equals m3.MOCKUP_ID
                                                             join m4 in context.ART_WF_MOCKUP_PROCESS on m2.MOCKUP_SUB_ID equals m4.MOCKUP_SUB_ID
                                                             where !NeedDesign_.Contains(m.MOCKUP_ID) && !Terminate_.Contains(m.MOCKUP_ID) && m4.REMARK_KILLPROCESS == null && m.CURRENT_USER_ID == userid.USERID && (m3.PACKING_TYPE_ID != PKGTYPE_sticker && m3.PACKING_TYPE_ID != PKGTYPE_label && m3.PACKING_TYPE_ID != PKGTYPE_insertpaper && m3.PACKING_TYPE_ID != PKGTYPE_leaflet)
                                                             && m4.UPDATE_BY != -1 && m4.CURRENT_STEP_ID == StepVN
                                                             && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                             group new { m4, m3, m2, m } by new { m2.MOCKUP_ID, m2.MOCKUP_SUB_ID } into m5
                                                             let min_m = m5.Min(x => x.m2.CREATE_DATE)
                                                             let min_m2 = m5.Min(x => x.m4.UPDATE_DATE)
                                                             select new ART_WF_MOCKUP_PROCESS_2
                                                             {
                                                                 MOCKUP_ID = m5.Key.MOCKUP_ID,
                                                                 MOCKUP_SUB_ID = m5.Key.MOCKUP_SUB_ID,
                                                                 CREATE_DATE = min_m,
                                                                 UPDATE_DATE = min_m2
                                                             }
                                                        ).Distinct().ToList();

                                    foreach (var e in Incorrect_normal_)
                                    {
                                        e.DUEDATE = CNService.AddBusinessDays(e.CREATE_DATE, 1);
                                    }
                                    //ncorrect_normal_ = Incorrect_normal_.Where(p => p.UPDATE_DATE <= p.DUEDATE).ToList();
                                    Incorrect_normal_ = (from m in Incorrect_normal_.Where(p => p.UPDATE_DATE <= p.DUEDATE)
                                                         group new { m } by new { m.MOCKUP_ID } into m2
                                                         select new ART_WF_MOCKUP_PROCESS_2
                                                         {
                                                             MOCKUP_ID = m2.Key.MOCKUP_ID,
                                                             NUMBER_VENDOR = m2.Select(q => q.m.MOCKUP_SUB_ID).Distinct().Count()
                                                         }).Where(p => p.NUMBER_VENDOR >= 4).ToList();

                                    double Incorrect_normal = Incorrect_normal_.Count();

                                    var Incorrect_PKG_ = (from m in context.ART_WF_MOCKUP_PROCESS
                                                          join m2 in context.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE on m.MOCKUP_ID equals m2.MOCKUP_ID
                                                          join m3 in context.ART_WF_MOCKUP_CHECK_LIST_PG on m.MOCKUP_ID equals m3.MOCKUP_ID
                                                          join m4 in context.ART_WF_MOCKUP_PROCESS on m2.MOCKUP_SUB_ID equals m4.MOCKUP_SUB_ID
                                                          where !NeedDesign_.Contains(m.MOCKUP_ID) && !Terminate_.Contains(m.MOCKUP_ID) && m4.REMARK_KILLPROCESS == null && m.CURRENT_USER_ID == userid.USERID && (m3.PACKING_TYPE_ID == PKGTYPE_sticker || m3.PACKING_TYPE_ID == PKGTYPE_label || m3.PACKING_TYPE_ID == PKGTYPE_insertpaper || m3.PACKING_TYPE_ID == PKGTYPE_leaflet)
                                                          && m4.UPDATE_BY != -1 && m4.CURRENT_STEP_ID == StepVN
                                                          && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                          group new { m4, m3, m2, m } by new { m2.MOCKUP_ID, m2.MOCKUP_SUB_ID } into m5
                                                          let min_m = m5.Min(x => x.m2.CREATE_DATE)
                                                          let min_m2 = m5.Min(x => x.m4.UPDATE_DATE)
                                                          select new ART_WF_MOCKUP_PROCESS_2
                                                          {
                                                              MOCKUP_ID = m5.Key.MOCKUP_ID,
                                                              MOCKUP_SUB_ID = m5.Key.MOCKUP_SUB_ID,
                                                              CREATE_DATE = min_m,
                                                              UPDATE_DATE = min_m2
                                                          }
                                                        ).Distinct().ToList();

                                    foreach (var e in Incorrect_PKG_)
                                    {
                                        e.DUEDATE = CNService.AddBusinessDays(e.CREATE_DATE, 1);
                                    }
                                    //ncorrect_normal_ = Incorrect_normal_.Where(p => p.UPDATE_DATE <= p.DUEDATE).ToList();
                                    Incorrect_PKG_ = (from m in Incorrect_PKG_.Where(p => p.UPDATE_DATE <= p.DUEDATE)
                                                      group new { m } by new { m.MOCKUP_ID } into m2
                                                      select new ART_WF_MOCKUP_PROCESS_2
                                                      {
                                                          MOCKUP_ID = m2.Key.MOCKUP_ID,
                                                          NUMBER_VENDOR = m2.Select(q => q.m.MOCKUP_SUB_ID).Distinct().Count()
                                                      }).Where(p => p.NUMBER_VENDOR >= 3).ToList();

                                    double Incorrect_PKG = Incorrect_PKG_.Count();

                                    var listcorrect = (from m in Incorrect_normal_
                                                       join m2 in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on m.MOCKUP_ID equals m2.MOCKUP_ID
                                                       orderby m2.MOCKUP_NO
                                                       select m2.MOCKUP_NO)
                                                      .Union(from m in Incorrect_PKG_
                                                             join m2 in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on m.MOCKUP_ID equals m2.MOCKUP_ID
                                                             orderby m2.MOCKUP_NO
                                                             select m2.MOCKUP_NO).ToList();
                                    temp.LIST_CORRECT_WF_NO = listcorrect;


                                    double Incorrect = Incorrect_normal + Incorrect_PKG;

                                    double tempCal = Math.Round((Incorrect * 100 / allMockup) * 100) / 100;
                                    if (Double.IsNaN(tempCal))
                                        tempCal = 0;
                                    else if(Double.IsInfinity(tempCal))
                                        tempCal = 0;

                                    if (i == 0) temp.Month1 = "" + tempCal + "";
                                    else if (i == 1) temp.Month2 = "" + tempCal + "";
                                    else if (i == 2) temp.Month3 = "" + tempCal + "";
                                    else if (i == 3) temp.Month4 = "" + tempCal + "";
                                    else if (i == 4) temp.Month5 = "" + tempCal + "";
                                    else if (i == 5) temp.Month6 = "" + tempCal + "";
                                    else if (i == 6) temp.Month7 = "" + tempCal + "";
                                    else if (i == 7) temp.Month8 = "" + tempCal + "";
                                    else if (i == 8) temp.Month9 = "" + tempCal + "";
                                    else if (i == 9) temp.Month10 = "" + tempCal + "";
                                    else if (i == 10) temp.Month11 = "" + tempCal + "";
                                    else if (i == 11) temp.Month12 = "" + tempCal + "";

                                    if (i <= 5 && (tempCal != 0 || allMockup > 0))
                                    {
                                        CounterMonth1++;
                                        Total6_1 += tempCal;
                                    }
                                    else if (i > 5 && (tempCal != 0 || allMockup > 0))
                                    {
                                        CounterMonth2++;
                                        Total6_2 += tempCal;
                                    }

                                }
                                if (CounterMonth1 != 0)
                                    AVG1 += Math.Round((Total6_1 / CounterMonth1) * 100) / 100;
                                else if (CounterMonth2 != 0)
                                    AVG2 += Math.Round((Total6_2 / CounterMonth2) * 100) / 100;


                                if (AVG1 != 0 || CounterMonth1 != 0)
                                {
                                    temp.AVG1 = "" + AVG1 + "";
                                    temp.GRADE1 = AVG1 >= param.data.SCORE5 ? "5" : AVG1 >= param.data.SCORE4 ? "4" : AVG1 >= param.data.SCORE3 ? "3" : AVG1 >= param.data.SCORE2 ? "2" : AVG1 < param.data.SCORE1 ? "1" : "0";
                                    //temp.GRADE1 = AVG1 >= 93.00 ? "5" : AVG1 >= 84.00 ? "4" : AVG1 >= 75.00 ? "3" : AVG1 >= 66.00 ? "2" : AVG1 < 66.00 ? "1" : "0";
                                }
                                if (AVG2 != 0 || CounterMonth2 != 0)
                                {
                                    temp.AVG2 = "" + AVG2 + "";
                                    temp.GRADE2 = AVG2 >= param.data.SCORE5 ? "5" : AVG2 >= param.data.SCORE4 ? "4" : AVG2 >= param.data.SCORE3 ? "3" : AVG2 >= param.data.SCORE2 ? "2" : AVG2 < param.data.SCORE1 ? "1" : "0";
                                    //temp.GRADE2 = AVG2 >= 93.00 ? "5" : AVG2 >= 84.00 ? "4" : AVG2 >= 75.00 ? "3" : AVG2 >= 66.00 ? "2" : AVG2 < 66.00 ? "1" : "0";
                                }
                                temp.MONTH_FROM = dateFrom.Month + "/" + dateFrom.Day + "/" + dateFrom.Year;
                                temp.TARGET = 80;
                                res.data.Add(temp);
                            }
                        }
                        else if (param.data.KPI_TYPE == "artworkstand")
                        {
                            var StepPA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                            var StepRD = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK { STEP_ARTWORK_CODE = "SEND_RD" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                            var StepCusPrint = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK { STEP_ARTWORK_CODE = "SEND_CUS_PRINT" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                            var StepCusReview = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK { STEP_ARTWORK_CODE = "SEND_CUS_REVIEW" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                            var KPIDuration = 7;   // by aof #INC-119064
                            KPIDuration = Int32.Parse(context.ART_M_CONSTANT.Where(w => w.PROGRAM_NAME == "KPIREPORT" && w.FUNCAREA == "ARTWORK" && w.VARIABLE_NAME == "KPI_DURATION").Select(s => s.LOWVALUE).FirstOrDefault()); // by aof #INC-119064

                            if (param.data.USERID != -1)
                                EmployeeInDep = (from m in context.ART_M_POSITION_ROLE
                                                 join m3 in context.ART_M_POSITION on m.POSITION_ID equals m3.ART_M_POSITION_ID
                                                 join m4 in context.ART_M_USER on m.POSITION_ID equals m4.POSITION_ID
                                                 where m4.USER_ID == param.data.USERID
                                                 select new KPI_REPORT_MODEL
                                                 {
                                                     EMPLOYEE_ID_DISPLAY_TEXT = m4.USERNAME,
                                                     EMPLOYEE_NAME_DISPLAY_TEXT = m4.TITLE + m4.FIRST_NAME + " " + m4.LAST_NAME,
                                                     POSITION_DISPLAY_TEXT = m3.ART_M_POSITION_NAME,
                                                     USERID = m4.USER_ID
                                                 }).Distinct().ToList();
                            else
                            {
                                var role_param = (from p in context.ART_M_ROLE
                                                  where p.ROLE_CODE.StartsWith("PA_")
                                                  select p.ROLE_ID).ToList();

                                var userRolePA = (from p in context.ART_M_USER_ROLE
                                                  where role_param.Contains(p.ROLE_ID)
                                                  select p.USER_ID).ToList();


                                EmployeeInDep = (from p in context.ART_M_USER
                                                 where userRolePA.Contains(p.USER_ID) && p.IS_ACTIVE == "X" && p.POSITION_ID == position_.ART_M_POSITION_ID
                                                 select new KPI_REPORT_MODEL
                                                 {
                                                     EMPLOYEE_ID_DISPLAY_TEXT = p.USERNAME,
                                                     EMPLOYEE_NAME_DISPLAY_TEXT = p.TITLE + p.FIRST_NAME + " " + p.LAST_NAME,
                                                     POSITION_DISPLAY_TEXT = position_.ART_M_POSITION_NAME,
                                                     USERID = p.USER_ID
                                                 }).Distinct().ToList();
                            }

                            //foreach (คน)
                            foreach (var userid in EmployeeInDep)
                            {
                                //if (userid.USERID == 81)
                                //    userid.USERID = 81;
                                double AVG1 = 0;
                                double AVG2 = 0;
                                var monthStart = dateFrom.Month;
                                var monthEnd = dateTo.Month;

                                var temp = new KPI_REPORT_MODEL();
                                temp.EMPLOYEE_ID_DISPLAY_TEXT = userid.EMPLOYEE_ID_DISPLAY_TEXT;
                                temp.EMPLOYEE_NAME_DISPLAY_TEXT = userid.EMPLOYEE_NAME_DISPLAY_TEXT;
                                temp.POSITION_DISPLAY_TEXT = userid.POSITION_DISPLAY_TEXT;

                                DateTime createdateto = new DateTime();

                                var CounterMonth1 = 0;
                                var CounterMonth2 = 0;
                                double Total6_1 = 0;
                                double Total6_2 = 0;
                                for (int i = 0; i <= difdate; i++)
                                {

                                    var createdatefrom = dateFrom.AddMonths(i);
                                    if (i > 0)
                                        createdatefrom = createdateto.AddSeconds(1);
                                    else
                                        createdatefrom = dateFrom.AddMonths(i).AddDays(-KPIDuration - 1).AddSeconds(-1);  //createdatefrom = dateFrom.AddMonths(i).AddDays(-7).AddSeconds(-1);   // by aof #INC-119064
                                    createdateto = dateFrom.AddMonths(i + 1).AddDays(-KPIDuration).AddSeconds(-1); //createdateto = dateFrom.AddMonths(i + 1).AddDays(-6).AddSeconds(-1); // by aof #INC-119064

                                    var weekends = 0;

                                    weekends = CNService.GetBusinessDays(createdateto, createdateto.AddDays(KPIDuration )); //weekends = CNService.GetBusinessDays(createdateto, createdateto.AddDays(6));  // by aof #INC-119064
                                    if (weekends != 6)
                                    {
                                        createdateto = createdateto.AddDays(-6 + weekends);
                                        if (createdateto.DayOfWeek == DayOfWeek.Saturday)
                                        {
                                            createdateto = createdateto.AddDays(-1);
                                        }
                                        else if (createdateto.DayOfWeek == DayOfWeek.Sunday)
                                        {
                                            createdateto = createdateto.AddDays(-2);
                                        }
                                    }

                                    //var compare = 0;
                                    //while (weekends != 6)
                                    //{
                                    //    createdateto = createdateto.AddDays(-compare);

                                    //    weekends = CNService.GetBusinessDays(createdateto.AddDays(-1).AddSeconds(1), createdateto.AddDays(6 + compare));
                                    //    compare = 6 - weekends;
                                    //}

                                    weekends = 0;


                                    weekends = CNService.GetBusinessDays(createdatefrom, createdatefrom.AddDays(KPIDuration +1)); //weekends = CNService.GetBusinessDays(createdatefrom, createdatefrom.AddDays(7)); // by aof #INC-119064
                                    if (weekends != 6)
                                    {
                                        createdatefrom = createdatefrom.AddDays(-KPIDuration + weekends); //createdatefrom = createdatefrom.AddDays(-6 + weekends);  // by aof #INC-119064
                                        if (createdatefrom.DayOfWeek == DayOfWeek.Saturday)
                                        {
                                            createdatefrom = new DateTime(createdatefrom.Year, createdatefrom.Month, createdatefrom.Day, 23, 59, 59);
                                            createdatefrom = createdatefrom.AddDays(-1);
                                        }
                                        else if (createdatefrom.DayOfWeek == DayOfWeek.Sunday)
                                        {
                                            createdatefrom = new DateTime(createdatefrom.Year, createdatefrom.Month, createdatefrom.Day, 23, 59, 59);
                                            createdatefrom = createdatefrom.AddDays(-2);
                                        }
                                        else if (i == 0)
                                            createdatefrom = createdatefrom.AddSeconds(1);
                                    }

                                    var ExtendItem = (from m in context.ART_WF_ARTWORK_PROCESS
                                                      where m.IS_TERMINATE == null && m.REMARK_KILLPROCESS == null && (m.CURRENT_USER_ID == userid.USERID || m.CREATE_BY == userid.USERID || m.CURRENT_STEP_ID == StepRD) && m.IS_STEP_DURATION_EXTEND != null
                                                       && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                      select m.ARTWORK_ITEM_ID).Distinct().ToList();

                                    var Terminate_ = (from m in context.ART_WF_ARTWORK_PROCESS
                                                      where m.IS_TERMINATE != null && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                      select m.ARTWORK_ITEM_ID).Distinct().ToList();

                                    var allartwork_ = (from m in context.ART_WF_ARTWORK_PROCESS
                                                       join m3 in context.ART_WF_ARTWORK_REQUEST_ITEM on m.ARTWORK_ITEM_ID equals m3.ARTWORK_ITEM_ID
                                                       where !Terminate_.Contains(m.ARTWORK_ITEM_ID) && m.REMARK_KILLPROCESS == null && m.CURRENT_STEP_ID == StepPA && m.CURRENT_USER_ID == userid.USERID && m3.REQUEST_ITEM_NO.Contains("AW-N") && m.IS_STEP_DURATION_EXTEND == null
                                                      && !ExtendItem.Contains(m.ARTWORK_ITEM_ID) && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                       select m.ARTWORK_ITEM_ID).Distinct().ToList();

                                    temp.LIST_ALL_WF_NO = (from m in allartwork_
                                                           join m2 in context.ART_WF_ARTWORK_REQUEST_ITEM on m equals m2.ARTWORK_ITEM_ID
                                                           orderby m2.ARTWORK_ITEM_ID
                                                           select m2.REQUEST_ITEM_NO).ToList();

                                    double allartwork = allartwork_.Count();

                                    var listartwork_correct = (from m in context.ART_WF_ARTWORK_PROCESS
                                                               join m3 in context.ART_WF_ARTWORK_REQUEST_ITEM on m.ARTWORK_ITEM_ID equals m3.ARTWORK_ITEM_ID
                                                               join m2 in (from m1 in context.ART_WF_ARTWORK_PROCESS where m1.CURRENT_STEP_ID == StepCusPrint && m1.CREATE_BY == userid.USERID select m1) on m.ARTWORK_SUB_ID equals m2.PARENT_ARTWORK_SUB_ID
                                                               where !Terminate_.Contains(m.ARTWORK_ITEM_ID) && m.REMARK_KILLPROCESS == null && m.CURRENT_STEP_ID == StepPA && m.CURRENT_USER_ID == userid.USERID && m3.REQUEST_ITEM_NO.Contains("AW-N") && m.IS_STEP_DURATION_EXTEND == null
                                                               && !ExtendItem.Contains(m.ARTWORK_ITEM_ID) && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                               group new { m, m2, m3 } by new { m.ARTWORK_ITEM_ID, m3.REQUEST_ITEM_NO } into m4
                                                               let firstgroup = m4.FirstOrDefault()
                                                               let type = m4.Min(x => x.m3.REQUEST_ITEM_NO)
                                                               let min_m = m4.Min(x => x.m.CREATE_DATE)
                                                               let min_m2 = m4.Min(x => x.m2.CREATE_DATE)
                                                               select new ART_WF_ARTWORK_PROCESS_2
                                                               {
                                                                   ARTWORK_ITEM_ID = m4.Key.ARTWORK_ITEM_ID,
                                                                   ARTWORK_NO_DISPLAY_TXT = type,
                                                                   CREATE_DATE = min_m,
                                                                   UPDATE_DATE = min_m2
                                                               }).ToList();

                                    foreach (var e in listartwork_correct)
                                    {
                                        var addDuration = 0;

                                        var q = (from m in context.ART_WF_ARTWORK_PROCESS
                                                 join m1 in context.ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER on new { x1 = m.ARTWORK_REQUEST_ID, x2 = m.CURRENT_USER_ID } equals new { x1 = m1.ARTWORK_REQUEST_ID, x2 = (int?)m1.CUSTOMER_USER_ID }
                                                 where m.ARTWORK_ITEM_ID == e.ARTWORK_ITEM_ID && m.CURRENT_STEP_ID == StepCusReview && m.IS_TERMINATE == null && m.REMARK_KILLPROCESS == null && m1.MAIL_TO == "X"
                                                 select new ART_WF_ARTWORK_PROCESS_2
                                                 {
                                                     ARTWORK_ITEM_ID = m.ARTWORK_ITEM_ID,
                                                     CREATE_DATE = m.CREATE_DATE,
                                                     UPDATE_DATE = m.UPDATE_DATE
                                                 }).ToList();

                                        foreach (var e1 in q)
                                        {
                                            var start = new DateTime(e1.CREATE_DATE.Year, e1.CREATE_DATE.Month, e1.CREATE_DATE.Day, 0, 0, 0);
                                            var end = new DateTime(e1.UPDATE_DATE.Year, e1.UPDATE_DATE.Month, e1.UPDATE_DATE.Day, 0, 0, 0);
                                            if (start.DayOfWeek == DayOfWeek.Saturday)
                                            {
                                                start = start.AddDays(-1);
                                            }
                                            else if (start.DayOfWeek == DayOfWeek.Sunday)
                                            {
                                                start = start.AddDays(-2);
                                            }
                                            if (end.DayOfWeek == DayOfWeek.Saturday)
                                            {
                                                end = end.AddDays(2);
                                            }
                                            else if (end.DayOfWeek == DayOfWeek.Sunday)
                                            {
                                                end = end.AddDays(1);
                                            }
                                            addDuration += CNService.GetBusinessDays(start, end.AddDays(1));
                                        }

                                        e.DUEDATE = CNService.AddBusinessDays(e.CREATE_DATE, KPIDuration + addDuration); // e.DUEDATE = CNService.AddBusinessDays(e.CREATE_DATE, 6 + addDuration);   // by aof #INC-119064
                                    }

                                    var correct_ = listartwork_correct.Where(p => p.UPDATE_DATE <= p.DUEDATE).ToList();

                                    temp.LIST_CORRECT_WF_NO = (from m in correct_
                                                               join m2 in context.ART_WF_ARTWORK_REQUEST_ITEM on m.ARTWORK_ITEM_ID equals m2.ARTWORK_ITEM_ID
                                                               orderby m2.ARTWORK_ITEM_ID
                                                               select m2.REQUEST_ITEM_NO).ToList();

                                    double correct = correct_.Count();

                                    double tempCal = Math.Round((correct * 100 / allartwork) * 100) / 100;
                                    if (Double.IsNaN(tempCal))
                                        tempCal = 0;

                                    if (i == 0) temp.Month1 = "" + tempCal + "";
                                    else if (i == 1) temp.Month2 = "" + tempCal + "";
                                    else if (i == 2) temp.Month3 = "" + tempCal + "";
                                    else if (i == 3) temp.Month4 = "" + tempCal + "";
                                    else if (i == 4) temp.Month5 = "" + tempCal + "";
                                    else if (i == 5) temp.Month6 = "" + tempCal + "";
                                    else if (i == 6) temp.Month7 = "" + tempCal + "";
                                    else if (i == 7) temp.Month8 = "" + tempCal + "";
                                    else if (i == 8) temp.Month9 = "" + tempCal + "";
                                    else if (i == 9) temp.Month10 = "" + tempCal + "";
                                    else if (i == 10) temp.Month11 = "" + tempCal + "";
                                    else if (i == 11) temp.Month12 = "" + tempCal + "";

                                    if (i <= 5 && (tempCal != 0 || allartwork > 0))
                                    {
                                        CounterMonth1++;
                                        Total6_1 += tempCal;
                                    }
                                    else if (i > 5 && (tempCal != 0 || allartwork > 0))
                                    {
                                        CounterMonth2++;
                                        Total6_2 += tempCal;
                                    }

                                }
                                if (CounterMonth1 != 0)
                                    AVG1 += Math.Round((Total6_1 / CounterMonth1) * 100) / 100;
                                else if (CounterMonth2 != 0)
                                    AVG2 += Math.Round((Total6_2 / CounterMonth2) * 100) / 100;


                                if (AVG1 != 0 || CounterMonth1 != 0)
                                {
                                    temp.AVG1 = "" + AVG1 + "";
                                    temp.GRADE1 = AVG1 >= param.data.SCORE5 ? "5" : AVG1 >= param.data.SCORE4 ? "4" : AVG1 >= param.data.SCORE3 ? "3" : AVG1 >= param.data.SCORE2 ? "2" : AVG1 < param.data.SCORE1 ? "1" : "0";
                                    //temp.GRADE1 = AVG1 >= 99.00 ? "5" : AVG1 >= 96.00 ? "4" : AVG1 >= 93.00 ? "3" : AVG1 >= 90.00 ? "2" : AVG1 < 90 ? "1" : "0";
                                }
                                if (AVG2 != 0 || CounterMonth2 != 0)
                                {
                                    temp.AVG2 = "" + AVG2 + "";
                                    temp.GRADE2 = AVG2 >= param.data.SCORE5 ? "5" : AVG2 >= param.data.SCORE4 ? "4" : AVG2 >= param.data.SCORE3 ? "3" : AVG2 >= param.data.SCORE2 ? "2" : AVG2 < param.data.SCORE1 ? "1" : "0";
                                    //temp.GRADE2 = AVG2 >= 99.00 ? "5" : AVG2 >= 96.00 ? "4" : AVG2 >= 93.00 ? "3" : AVG2 >= 90.00 ? "2" : AVG2 < 90 ? "1" : "0";
                                }
                                temp.MONTH_FROM = dateFrom.Month + "/" + dateFrom.Day + "/" + dateFrom.Year;
                                temp.TARGET = 95;
                                res.data.Add(temp);
                            }
                        }
                        else if (param.data.KPI_TYPE == "postand")
                        {
                            var StepPP = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK { STEP_ARTWORK_CODE = "SEND_PP" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                            var StepVNPO = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK { STEP_ARTWORK_CODE = "SEND_VN_PO" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                            var StepSendbackMK = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK { STEP_ARTWORK_CODE = "SEND_BACK_MK" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                            if (param.data.USERID != -1)
                                EmployeeInDep = (from m in context.ART_M_POSITION_ROLE
                                                 join m3 in context.ART_M_POSITION on m.POSITION_ID equals m3.ART_M_POSITION_ID
                                                 join m4 in context.ART_M_USER on m.POSITION_ID equals m4.POSITION_ID
                                                 where m4.USER_ID == param.data.USERID
                                                 select new KPI_REPORT_MODEL
                                                 {
                                                     EMPLOYEE_ID_DISPLAY_TEXT = m4.USERNAME,
                                                     EMPLOYEE_NAME_DISPLAY_TEXT = m4.TITLE + m4.FIRST_NAME + " " + m4.LAST_NAME,
                                                     POSITION_DISPLAY_TEXT = m3.ART_M_POSITION_NAME,
                                                     USERID = m4.USER_ID
                                                 }).Distinct().ToList();
                            else
                            {
                                var role_param = (from p in context.ART_M_ROLE
                                                  where p.ROLE_CODE.StartsWith("PP_")
                                                  select p.ROLE_ID).ToList();

                                var userRolePA = (from p in context.ART_M_USER_ROLE
                                                  where role_param.Contains(p.ROLE_ID)
                                                  select p.USER_ID).ToList();


                                EmployeeInDep = (from p in context.ART_M_USER
                                                 where userRolePA.Contains(p.USER_ID) && p.IS_ACTIVE == "X" && p.POSITION_ID == position_.ART_M_POSITION_ID
                                                 select new KPI_REPORT_MODEL
                                                 {
                                                     EMPLOYEE_ID_DISPLAY_TEXT = p.USERNAME,
                                                     EMPLOYEE_NAME_DISPLAY_TEXT = p.TITLE + p.FIRST_NAME + " " + p.LAST_NAME,
                                                     POSITION_DISPLAY_TEXT = position_.ART_M_POSITION_NAME,
                                                     USERID = p.USER_ID
                                                 }).Distinct().ToList();
                            }

                            //foreach (คน)
                            foreach (var userid in EmployeeInDep)
                            {
                                double AVG1 = 0;
                                double AVG2 = 0;
                                var monthStart = dateFrom.Month;
                                var monthEnd = dateTo.Month;

                                var temp = new KPI_REPORT_MODEL();
                                temp.EMPLOYEE_ID_DISPLAY_TEXT = userid.EMPLOYEE_ID_DISPLAY_TEXT;
                                temp.EMPLOYEE_NAME_DISPLAY_TEXT = userid.EMPLOYEE_NAME_DISPLAY_TEXT;
                                temp.POSITION_DISPLAY_TEXT = userid.POSITION_DISPLAY_TEXT;

                                DateTime createdateto = new DateTime();

                                var CounterMonth1 = 0;
                                var CounterMonth2 = 0;
                                double Total6_1 = 0;
                                double Total6_2 = 0;
                                for (int i = 0; i <= difdate; i++)
                                {

                                    var createdatefrom = dateFrom.AddMonths(i);
                                    createdateto = dateFrom.AddMonths(i + 1).AddSeconds(-1);

                                    var Terminate_ = (from m in context.ART_WF_ARTWORK_PROCESS
                                                      where m.IS_TERMINATE != null && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                      select m.ARTWORK_ITEM_ID).Distinct().ToList();


                                    var listallartwork_ = (from m in context.ART_WF_ARTWORK_PROCESS
                                                           where m.REMARK_KILLPROCESS == null && m.CURRENT_STEP_ID == StepPP && m.CURRENT_USER_ID == userid.USERID && !Terminate_.Contains(m.ARTWORK_ITEM_ID)
                                                          && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                           group new { m } by m.ARTWORK_ITEM_ID into m3
                                                           let firstgroup = m3.FirstOrDefault()
                                                           let min_m = m3.Min(x => x.m.CREATE_DATE)
                                                           let min_m2 = m3.Min(x => x.m.UPDATE_DATE)
                                                           select new ART_WF_ARTWORK_PROCESS_2
                                                           {
                                                               ARTWORK_ITEM_ID = m3.Key,
                                                               CREATE_DATE = min_m,
                                                               UPDATE_DATE = min_m2,
                                                           }).ToList();

                                    temp.LIST_ALL_WF_NO = (from m in listallartwork_
                                                           join m2 in context.ART_WF_ARTWORK_REQUEST_ITEM on m.ARTWORK_ITEM_ID equals m2.ARTWORK_ITEM_ID
                                                           orderby m2.ARTWORK_ITEM_ID
                                                           select m2.REQUEST_ITEM_NO).ToList();

                                    double listallartwork = listallartwork_.Count();


                                    var ontime_ = (from m in context.ART_WF_ARTWORK_PROCESS
                                                   join m2 in (from m1 in context.ART_WF_ARTWORK_PROCESS where m1.CURRENT_STEP_ID == StepVNPO select m1) on m.ARTWORK_SUB_ID equals m2.PARENT_ARTWORK_SUB_ID
                                                   where m.REMARK_KILLPROCESS == null && m.CURRENT_STEP_ID == StepPP && m.CURRENT_USER_ID == userid.USERID && !Terminate_.Contains(m.ARTWORK_ITEM_ID)
                                                  && m.CREATE_DATE > createdatefrom && m.CREATE_DATE <= createdateto
                                                   group new { m, m2 } by m.ARTWORK_ITEM_ID into m3
                                                   let firstgroup = m3.FirstOrDefault()
                                                   let min_m = m3.Min(x => x.m.CREATE_DATE)
                                                   let min_m2 = m3.Min(x => x.m2.CREATE_DATE)
                                                   select new ART_WF_ARTWORK_PROCESS_2
                                                   {
                                                       ARTWORK_ITEM_ID = m3.Key,
                                                       CREATE_DATE = min_m,
                                                       UPDATE_DATE = min_m2,
                                                   }).ToList();
                                    List<int> SEND_BACK_WF = new List<int>();
                                    foreach (var e in ontime_)
                                    {
                                        var addDuration = 0;

                                        var q = (from m in context.ART_WF_ARTWORK_PROCESS
                                                 join m1 in context.ART_WF_ARTWORK_PROCESS_PP on m.ARTWORK_SUB_ID equals m1.ARTWORK_SUB_ID
                                                 where m.ARTWORK_ITEM_ID == e.ARTWORK_ITEM_ID && m.CURRENT_STEP_ID == StepPP && m.IS_TERMINATE == null && m.REMARK_KILLPROCESS == null && m1.ACTION_CODE == "SEND_BACK"
                                                 select new ART_WF_ARTWORK_PROCESS_2
                                                 {
                                                     ARTWORK_ITEM_ID = m.ARTWORK_ITEM_ID,
                                                     CREATE_DATE = m.CREATE_DATE,
                                                     UPDATE_DATE = m1.UPDATE_DATE
                                                 }).ToList();



                                        foreach (var e1 in q)
                                        {
                                            SEND_BACK_WF.Add(e1.ARTWORK_ITEM_ID);
                                            TimeSpan ts = e1.UPDATE_DATE - e1.CREATE_DATE;

                                            addDuration += (int)ts.TotalHours;
                                        }

                                        e.DUEDATE = CNService.AddBusinessDays(e.CREATE_DATE, 1);
                                        e.DUEDATE = e.DUEDATE.AddHours(addDuration);

                                        if (e.DUEDATE.DayOfWeek == DayOfWeek.Saturday)
                                        {
                                            e.DUEDATE = e.DUEDATE.AddDays(2);
                                        }
                                        else if (createdatefrom.DayOfWeek == DayOfWeek.Sunday)
                                        {
                                            e.DUEDATE = e.DUEDATE.AddDays(1);
                                        }
                                    }

                                    double ontime = ontime_.Where(p => p.UPDATE_DATE <= p.DUEDATE).ToList().Count();

                                    temp.LIST_CORRECT_WF_NO = (from m in ontime_.Where(p => p.UPDATE_DATE <= p.DUEDATE)
                                                               join m2 in context.ART_WF_ARTWORK_REQUEST_ITEM on m.ARTWORK_ITEM_ID equals m2.ARTWORK_ITEM_ID
                                                               orderby m2.ARTWORK_ITEM_ID
                                                               select m2.REQUEST_ITEM_NO).ToList();

                                    temp.LIST_SENDBACK_WF_NO = (from m in SEND_BACK_WF
                                                                join m2 in context.ART_WF_ARTWORK_REQUEST_ITEM on m equals m2.ARTWORK_ITEM_ID
                                                                orderby m2.ARTWORK_ITEM_ID
                                                                select m2.REQUEST_ITEM_NO).ToList();


                                    double tempCal = Math.Round((ontime * 100 / listallartwork) * 100) / 100;

                                    if (Double.IsNaN(tempCal))
                                        tempCal = 0;

                                    if (i == 0) temp.Month1 = "" + tempCal + "";
                                    else if (i == 1) temp.Month2 = "" + tempCal + "";
                                    else if (i == 2) temp.Month3 = "" + tempCal + "";
                                    else if (i == 3) temp.Month4 = "" + tempCal + "";
                                    else if (i == 4) temp.Month5 = "" + tempCal + "";
                                    else if (i == 5) temp.Month6 = "" + tempCal + "";
                                    else if (i == 6) temp.Month7 = "" + tempCal + "";
                                    else if (i == 7) temp.Month8 = "" + tempCal + "";
                                    else if (i == 8) temp.Month9 = "" + tempCal + "";
                                    else if (i == 9) temp.Month10 = "" + tempCal + "";
                                    else if (i == 10) temp.Month11 = "" + tempCal + "";
                                    else if (i == 11) temp.Month12 = "" + tempCal + "";

                                    if (i <= 5 && (tempCal != 0 || ontime > 0))
                                    {
                                        CounterMonth1++;
                                        Total6_1 += tempCal;
                                    }
                                    else if (i > 5 && (tempCal != 0 || ontime > 0))
                                    {
                                        CounterMonth2++;
                                        Total6_2 += tempCal;
                                    }

                                }
                                if (CounterMonth1 != 0)
                                    AVG1 += Math.Round((Total6_1 / CounterMonth1) * 100) / 100;
                                else if (CounterMonth2 != 0)
                                    AVG2 += Math.Round((Total6_2 / CounterMonth2) * 100) / 100;


                                if (AVG1 != 0 || CounterMonth1 != 0)
                                {
                                    temp.AVG1 = "" + AVG1 + "";
                                    temp.GRADE1 = AVG1 >= param.data.SCORE5 ? "5" : AVG1 >= param.data.SCORE4 ? "4" : AVG1 >= param.data.SCORE3 ? "3" : AVG1 >= param.data.SCORE2 ? "2" : AVG1 < param.data.SCORE1 ? "1" : "0";
                                    //temp.GRADE1 = AVG1 >= 90.99 ? "5" : AVG1 >= 86.99 ? "4" : AVG1 >= 83.00 ? "3" : AVG1 >= 79.00 ? "2" : AVG1 < 79.00 ? "1" : "0";
                                }
                                if (AVG2 != 0 || CounterMonth2 != 0)
                                {
                                    temp.AVG2 = "" + AVG2 + "";
                                    temp.GRADE2 = AVG2 >= param.data.SCORE5 ? "5" : AVG2 >= param.data.SCORE4 ? "4" : AVG2 >= param.data.SCORE3 ? "3" : AVG2 >= param.data.SCORE2 ? "2" : AVG2 < param.data.SCORE1 ? "1" : "0";
                                    //temp.GRADE2 = AVG2 >= 90.99 ? "5" : AVG2 >= 86.99 ? "4" : AVG2 >= 83.00 ? "3" : AVG2 >= 79.00 ? "2" : AVG2 < 79.00 ? "1" : "0";
                                }
                                temp.MONTH_FROM = dateFrom.Month + "/" + dateFrom.Day + "/" + dateFrom.Year;
                                temp.TARGET = 85;
                                res.data.Add(temp);
                            }
                        }

                    }

                    Results.status = "S";
                    Results.data = res.data;
                    Results.draw = param.draw;
                    Results.recordsTotal = res.data.Count();
                    Results.recordsFiltered = res.data.Count();
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }
        public static KPI_REPORT_MODEL_RESULT GetKPIPriceTemplateCompare(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST param)
        {
            KPI_REPORT_MODEL_RESULT Results = new KPI_REPORT_MODEL_RESULT();
            using (ARTWORKEntities context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {

                    try
                    {
                        if (param.data.FIRST_LOAD)
                        {
                            Results.status = "S";
                            Results.data = new List<KPI_REPORT_MODEL>();
                            Results.draw = param.draw;
                            return Results;
                        }

                        if (param == null || param.data == null)
                        {
                            //  Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetAll());
                        }
                        else
                        {
                            DateTime dateFrom = CNService.ConvertStringToDate(param.data.DATE_FROM);
                            DateTime dateTo = CNService.ConvertStringToDate(param.data.DATE_TO);

                            var listMockupID = (from q in context.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE
                                                where q.SELECTED == "X" && q.CREATE_DATE > dateFrom && q.CREATE_DATE <= dateTo
                                                select new KPI_REPORT_MODEL
                                                {
                                                    MOCKUP_ID = q.MOCKUP_ID
                                                }).Distinct().ToList();

                            Results.data = listMockupID;
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                var temp = GetByItemContainKPI(Results.data[i].MOCKUP_ID);
                                Results.data[i].ALL_PRICE = temp;
                                if (temp.Count() == 0)
                                    Results.data[i].WF_NO = "NO SELECTED";
                            }

                        }
                        Results.data = Results.data.Where(m => m.WF_NO != "NO SELECTED").ToList();

                        Results.status = "S";
                    }
                    catch (Exception ex)
                    {
                        Results.status = "E";
                        Results.msg = CNService.GetErrorMessage(ex);
                    }
                }
            }
            return Results;
        }

        public static List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2> GetByItemContainKPI(int Item)
        {
            List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2> listComparePrice = new List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2>();
            using (ARTWORKEntities context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var StepPG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;

                    listComparePrice = MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE((from p in context.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE
                                                                                               where p.MOCKUP_ID == Item && p.USER_ID != null && ((p.UPDATE_BY != -1 && p.UPDATE_BY != p.CREATE_BY) || p.SELECTED == "X")
                                                                                               select p).ToList());

                    listComparePrice = listComparePrice.OrderBy(m => m.MOCKUP_SUB_ID).ToList();
                    int old = 0;
                    int oldMockupSubId = 0;
                    if (listComparePrice.Count > 0)
                    {
                        ART_WF_MOCKUP_PROCESS tempProcess = new ART_WF_MOCKUP_PROCESS();
                        ART_M_USER_VENDOR vendorObj = new ART_M_USER_VENDOR();
                        for (int i = 0; i < listComparePrice.Count; i++)
                        {
                            if (old != listComparePrice[i].MOCKUP_SUB_ID)
                            {
                                old = listComparePrice[i].MOCKUP_SUB_ID;
                                tempProcess = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(listComparePrice[i].MOCKUP_SUB_ID, context);
                            }

                            if (string.IsNullOrEmpty(tempProcess.IS_END))
                            {
                                listComparePrice[i].PRICE = 0;
                            }

                            if (tempProcess.REMARK == "Manaul add price template." && CNService.IsPGSup(tempProcess.CREATE_BY, context))
                            {
                                listComparePrice[i].IS_MANUAL = "X";
                            }

                            var vendor = tempProcess;

                            listComparePrice[i].VENDOR_ID = vendor.CURRENT_VENDOR_ID;
                            listComparePrice[i].VENDOR_DISPLAY_TXT = CNService.GetVendorName(listComparePrice[i].VENDOR_ID, context);
                            listComparePrice[i].USER_DISPLAY_TXT = CNService.GetUserName(vendor.CURRENT_USER_ID, context);

                            var checklist = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_ITEM() { MOCKUP_ID = Item }, context).FirstOrDefault();
                            if (checklist != null)
                                listComparePrice[i].WF_NO = checklist.MOCKUP_NO;

                            var mockupsubpg = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS { MOCKUP_ID = Item, CURRENT_STEP_ID = StepPG }, context).FirstOrDefault();
                            if (mockupsubpg != null)
                            {
                                var final_info = ART_WF_MOCKUP_PROCESS_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG() { MOCKUP_SUB_ID = mockupsubpg.MOCKUP_SUB_ID }, context).FirstOrDefault();
                                if (final_info != null)
                                    listComparePrice[i].FINAL_INFO = final_info.FINAL_INFO;
                            }

                            if (oldMockupSubId != listComparePrice[i].MOCKUP_SUB_ID)
                            {
                                oldMockupSubId = listComparePrice[i].MOCKUP_SUB_ID;
                                var max = listComparePrice.Where(m => m.VENDOR_ID == listComparePrice[i].VENDOR_ID).Select(m => m.ROUND).Max();
                                listComparePrice[i].ROUND = max + 1;
                            }
                            else
                            {
                                listComparePrice[i].ROUND = listComparePrice.Where(m => m.VENDOR_ID == listComparePrice[i].VENDOR_ID && m.MOCKUP_SUB_ID == oldMockupSubId).Select(m => m.ROUND).FirstOrDefault();
                            }
                        }
                    }
                }
            }

            return listComparePrice.OrderByDescending(m => m.SELECTED).ThenByDescending(m => m.IS_MANUAL).ThenBy(m => m.VENDOR_DISPLAY_TXT).ThenBy(m => m.ROUND).ToList();
        }





    }
}