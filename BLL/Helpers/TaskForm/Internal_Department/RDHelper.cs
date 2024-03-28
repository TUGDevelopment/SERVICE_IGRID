using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services;
using DAL;
using DAL.Model;

namespace BLL.Helpers
{
    public static class RDHelper
    {
        public static ART_WF_MOCKUP_PROCESS_RD_RESULT GetRDForm(ART_WF_MOCKUP_PROCESS_RD_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_RD_RESULT Results = new ART_WF_MOCKUP_PROCESS_RD_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            //Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_RD(ART_WF_MOCKUP_PROCESS_RD_SERVICE.GetAll());
                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_RD(ART_WF_MOCKUP_PROCESS_RD_SERVICE.GetByItem(MapperServices.ART_WF_MOCKUP_PROCESS_RD(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                ART_SYS_ACTION act = new ART_SYS_ACTION();
                                act.ACTION_CODE = Results.data[i].ACTION_CODE;
                                Results.data[i].ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItem(act, context).FirstOrDefault().ACTION_NAME;
                                Results.data[i].CREATE_BY_DISPLAY_TXT = CNService.GetUserName(Results.data[i].CREATE_BY, context);
                                Results.data[i].COMMENT_BY_PG = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).REMARK;
                                Results.data[i].REASON_BY_PG = CNService.getReason(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).REASON_ID, context);
                                Results.data[i].REASON_BY_OTHER = CNService.getReason(Results.data[i].REASON_ID, context);
                                Results.data[i].CREATE_DATE_BY_PG = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).CREATE_DATE;
                                Results.data[i].REMARK_REASON_BY_PG = CNService.getRemarkReason(Results.data[i].MOCKUP_SUB_ID,"M", "SEND_PG", context);
                                Results.data[i].REMARK_REASON = CNService.getRemarkReason(Results.data[i].MOCKUP_SUB_ID, "M", "SEND_RD_PRI_PKG", context);

                                if (Results.data[i].IS_FINAL_SAMPLE == "1")
                                {
                                    Results.data[i].IS_FINAL_SAMPLE_DISPLAY_TXT = "Yes";
                                }
                                else if (Results.data[i].IS_FINAL_SAMPLE == "0")
                                {
                                    Results.data[i].IS_FINAL_SAMPLE_DISPLAY_TXT = "No";
                                }

                                if (Results.data[i].TYPE_OF_SAMPLE == "RD")
                                {
                                    Results.data[i].TYPE_OF_SAMPLE_DISPLAY_TXT = "RD Sample";
                                }
                                else if (Results.data[i].TYPE_OF_SAMPLE == "PRD")
                                {
                                    Results.data[i].TYPE_OF_SAMPLE_DISPLAY_TXT = "Production Sample";
                                }
                            }
                        }

                        var stepId = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_RD_PRI_PKG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var list = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID, CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var result = list.Where(p => !Results.data.Any(p2 => p2.MOCKUP_SUB_ID == p.MOCKUP_SUB_ID)).FirstOrDefault();
                        if (result != null)
                        {
                            ART_WF_MOCKUP_PROCESS_RD_2 item = new ART_WF_MOCKUP_PROCESS_RD_2();
                            item.CREATE_DATE_BY_PG = result.CREATE_DATE;
                            item.COMMENT_BY_PG = result.REMARK;
                            item.REASON_BY_PG = CNService.getReason(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(result.MOCKUP_SUB_ID, context).REASON_ID, context);
                            item.REMARK_REASON_BY_PG = CNService.getRemarkReason(result.MOCKUP_SUB_ID, "M", "SEND_PG", context);

                            Results.data.Add(item);
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

        public static ART_WF_MOCKUP_PROCESS_RD_RESULT SaveRDForm(ART_WF_MOCKUP_PROCESS_RD_REQUEST_LIST param_)
        {
            ART_WF_MOCKUP_PROCESS_RD_RESULT Results = new ART_WF_MOCKUP_PROCESS_RD_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        foreach (var param in param_.data)
                        {
                            ART_WF_MOCKUP_PROCESS_RD rdData = new ART_WF_MOCKUP_PROCESS_RD();
                            rdData = MapperServices.ART_WF_MOCKUP_PROCESS_RD(param);

                            var check = ART_WF_MOCKUP_PROCESS_RD_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_RD() { MOCKUP_SUB_ID = param.MOCKUP_SUB_ID }, context);
                            if (check.Count > 0)
                                rdData.MOCKUP_SUB_RD_ID = check.FirstOrDefault().MOCKUP_SUB_RD_ID;

                            ART_WF_MOCKUP_PROCESS_RD_SERVICE.SaveOrUpdate(rdData, context);

                            if (param.ENDTASKFORM)
                                MockUpProcessHelper.EndTaskForm(param.MOCKUP_SUB_ID, param.UPDATE_BY, context);

                            dbContextTransaction.Commit();

                            if (param.ACTION_CODE == "SEND_BACK")
                                EmailService.sendEmailMockup(param.MOCKUP_ID, param.MOCKUP_SUB_ID, "WF_SEND_BACK", context, param.COMMENT);
                            else if (param.ACTION_CODE == "SAVE")
                                EmailService.sendEmailMockup(param.MOCKUP_ID, param.MOCKUP_SUB_ID, "WF_OTHER_SAVE", context);
                            else
                                EmailService.sendEmailMockup(param.MOCKUP_ID, param.MOCKUP_SUB_ID, "WF_OTHER_SUBMIT", context);
                        }
                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_001", context);
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
