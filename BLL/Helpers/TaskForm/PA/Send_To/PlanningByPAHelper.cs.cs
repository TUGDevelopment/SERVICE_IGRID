using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using BLL.Helpers;

namespace BLL.Helpers
{
    public class PlanningByPAHelper
    {
        public static ART_WF_ARTWORK_PROCESS_PLANNING_RESULT GetPlanningByPA(ART_WF_ARTWORK_PROCESS_PLANNING_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PLANNING_RESULT Results = new ART_WF_ARTWORK_PROCESS_PLANNING_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_PLANNING(ART_WF_ARTWORK_PROCESS_PLANNING_SERVICE.GetAll(context));
                        }
                        else
                        {
                            var ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            param.data.ARTWORK_SUB_ID = 0;

                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_PLANNING(ART_WF_ARTWORK_PROCESS_PLANNING_SERVICE.GetByItemContain(MapperServices.ART_WF_ARTWORK_PROCESS_PLANNING(param.data), context));

                            param.data.ARTWORK_SUB_ID = ARTWORK_SUB_ID;
                            Results.data = Results.data.Where(m => CNService.FindArtworkSubId(ARTWORK_SUB_ID, context).Contains(m.ARTWORK_SUB_ID)).ToList();
                        }

                        ART_WF_ARTWORK_PROCESS_PLANNING p = new ART_WF_ARTWORK_PROCESS_PLANNING();

                        Results.status = "S";
                        var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PN" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var PAstepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                var reason_pa = CNService.getReason(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).REASON_ID, context);
                                ART_SYS_ACTION act = new ART_SYS_ACTION();
                                act.ACTION_CODE = Results.data[i].ACTION_CODE;
                                Results.data[i].ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItem(act, context).FirstOrDefault().ACTION_NAME;

                                var processFormPA = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context);

                                Results.data[i].COMMENT_BY_PA = processFormPA.REMARK;
                                Results.data[i].REASON_BY_PA = reason_pa;
                                Results.data[i].REMARK_REASON_BY_PA = "-";
                                Results.data[i].REMARK_REASON_BY_OTHER = "-";
                                Results.data[i].REASON_BY_OTHER = CNService.getReason(Results.data[i].REASON_ID, context);
                                if (reason_pa == "อื่นๆ โปรดระบุ (Others)")
                                {
                                    Results.data[i].REMARK_REASON_BY_PA = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = Results.data[i].ARTWORK_SUB_ID, WF_STEP = PAstepId }, context);
                                }
                                if (Results.data[i].REASON_BY_OTHER == "อื่นๆ โปรดระบุ (Others)")
                                {
                                    Results.data[i].REMARK_REASON_BY_OTHER = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = Results.data[i].ARTWORK_SUB_ID, WF_STEP = stepId }, context);
                                }
                                Results.data[i].CREATE_DATE_BY_PA = processFormPA.CREATE_DATE;
                            }
                        }

                        var list = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context), CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var result = list.Where(q => !Results.data.Any(q2 => q2.ARTWORK_SUB_ID == q.ARTWORK_SUB_ID)).FirstOrDefault();
                        if (result != null)
                        {
                            ART_WF_ARTWORK_PROCESS_PLANNING_2 item = new ART_WF_ARTWORK_PROCESS_PLANNING_2();
                            item.CREATE_DATE_BY_PA = result.CREATE_DATE;
                            item.COMMENT_BY_PA = result.REMARK;
                            item.REASON_BY_PA = CNService.getReason(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(result.ARTWORK_SUB_ID, context).REASON_ID, context);
                            item.REMARK_REASON_BY_PA = "-";
                            if (item.REASON_BY_PA == "อื่นๆ โปรดระบุ (Others)")
                            {
                                item.REMARK_REASON_BY_PA = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = result.ARTWORK_SUB_ID, WF_STEP = PAstepId }, context);
                            }
                            item.ARTWORK_SUB_ID = result.ARTWORK_SUB_ID;
                            item.ARTWORK_REQUEST_ID = result.ARTWORK_REQUEST_ID;

                            Results.data.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_RESULT SavePlanningByPA(ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        string msg = ArtworkProcessHelper.checkDupWF(param.data.PROCESS, context);
                        if (msg != "")
                        {
                            Results.status = "E";
                            Results.msg = msg;
                            return Results;
                        }

                        ART_WF_ARTWORK_PROCESS_RESULT processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
                        if (param.data.PROCESS != null)
                        {
                            processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                        }

                        ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA PlanningData = new ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA();
                        PlanningData = MapperServices.ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA(param.data);

                        if (processResults != null && processResults.data != null && processResults.data.Count > 0)
                        {
                            PlanningData.ARTWORK_SUB_ID = processResults.data[0].ARTWORK_SUB_ID;
                        }

                        ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_SERVICE.SaveOrUpdate(PlanningData, context);

                        Results.data = new List<ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_2>();
                        ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_2 item = new ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_2();
                        List<ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_2> listItem = new List<ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_2>();

                        item.ARTWORK_PROCESS_PLANNING_ID = PlanningData.ARTWORK_PROCESS_PLANNING_ID;
                        listItem.Add(item);

                        Results.data = listItem;

                        dbContextTransaction.Commit();

                        foreach (var process in processResults.data)
                            EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO", context);

                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_001",context);
                    }
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_PLANNING_RESULT PostPlanningSendToPA(ART_WF_ARTWORK_PROCESS_PLANNING_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PLANNING_RESULT Results = new ART_WF_ARTWORK_PROCESS_PLANNING_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var pnData = MapperServices.ART_WF_ARTWORK_PROCESS_PLANNING(param.data);

                        var check = ART_WF_ARTWORK_PROCESS_PLANNING_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PLANNING() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                        if (check.Count > 0)
                            pnData.ARTWORK_PROCESS_PLANNING_ID = check.FirstOrDefault().ARTWORK_PROCESS_PLANNING_ID;

                        ART_WF_ARTWORK_PROCESS_PLANNING_SERVICE.SaveOrUpdate(pnData, context);

                        if (param.data.ENDTASKFORM)
                            ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);

                        dbContextTransaction.Commit();

                        if (param.data.ACTION_CODE == "SEND_BACK")
                            EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_SEND_BACK", context, param.data.COMMENT);
                        else if (param.data.ACTION_CODE == "SAVE")
                            EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_OTHER_SAVE", context);
                        else
                            EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_OTHER_SUBMIT", context);

                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_001",context);
                    }
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
