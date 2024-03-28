using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class MKByPAHelper
    {
        public static ART_WF_ARTWORK_PROCESS_MARKETING_RESULT GetSendToMKInfo(ART_WF_ARTWORK_PROCESS_MARKETING_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_MARKETING_RESULT Results = new ART_WF_ARTWORK_PROCESS_MARKETING_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                        param.data.ARTWORK_SUB_ID = 0;

                        Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_MARKETING(ART_WF_ARTWORK_PROCESS_MARKETING_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_PROCESS_MARKETING(param.data), context));

                        param.data.ARTWORK_SUB_ID = ARTWORK_SUB_ID;
                        Results.data = Results.data.Where(m => CNService.FindArtworkSubId(ARTWORK_SUB_ID, context).Contains(m.ARTWORK_SUB_ID)).ToList();

                        Results.status = "S";

                        var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_MK" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var PAstepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                var reason_pa = CNService.getReason(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).REASON_ID, context);
                                ART_SYS_ACTION act = new ART_SYS_ACTION();
                                act.ACTION_CODE = Results.data[i].ACTION_CODE;
                                Results.data[i].ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItem(act, context).FirstOrDefault().ACTION_NAME;
                                Results.data[i].COMMENT_BY_PA = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).REMARK;
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

                                Results.data[i].CREATE_DATE_BY_PA = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).CREATE_DATE;
                            }
                        }

                        var list = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context), CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var result = list.Where(p => !Results.data.Any(p2 => p2.ARTWORK_SUB_ID == p.ARTWORK_SUB_ID)).FirstOrDefault();
                        if (result != null)
                        {
                            ART_WF_ARTWORK_PROCESS_MARKETING_2 item = new ART_WF_ARTWORK_PROCESS_MARKETING_2();
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

        public static ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_RESULT SaveMKByPA(ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_RESULT();
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
                            var requestForm = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(param.data.ARTWORK_REQUEST_ID, context);
                            //  if (requestForm.TYPE_OF_ARTWORK == "REPEAT")  //461704 by aof 
                            if (!CNService.IsMarketingCreatedArtworkRequest(requestForm, context))  //461704 by aof 
                            {
                                if (requestForm.REVIEWER_ID != null)
                                    param.data.PROCESS.CURRENT_USER_ID = requestForm.REVIEWER_ID;
                                else
                                    param.data.PROCESS.CURRENT_USER_ID = null;

                                //start ticket.445558 by aof 
                                //check is SEND_MK step
                                 var isStep_SEND_MK = false;
                                  isStep_SEND_MK = ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(param.data.PROCESS.CURRENT_STEP_ID , context).STEP_ARTWORK_CODE == "SEND_MK";
                                  if (isStep_SEND_MK)
                                   {
                                    //select StepID from SEND_BACK_MK 
                                    var SEND_BACK_MK_StepID = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_BACK_MK" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                                    //query to select latest current_user_id of SEND_BACK_MK
                                    var objAWProcess_SEND_BACK_MK = (from m in context.ART_WF_ARTWORK_PROCESS
                                                                     where m.ARTWORK_ITEM_ID == param.data.PROCESS.ARTWORK_ITEM_ID && m.CURRENT_STEP_ID == SEND_BACK_MK_StepID && string.IsNullOrEmpty(m.IS_TERMINATE )
                                                                     select new ART_WF_ARTWORK_PROCESS_2 {CURRENT_USER_ID = m.CURRENT_USER_ID,CREATE_DATE = m.CREATE_DATE}).OrderByDescending(m => m.CREATE_DATE).FirstOrDefault();
                                                                 
                                    if (objAWProcess_SEND_BACK_MK != null && objAWProcess_SEND_BACK_MK.CURRENT_USER_ID != -1)
                                    {
                                        param.data.PROCESS.CURRENT_USER_ID = objAWProcess_SEND_BACK_MK.CURRENT_USER_ID;
                                    }
                                }

                                //last ticket.445558 by aof 
                            }
                            else
                            {
                                var IsMK = CNService.IsMarketing(Convert.ToInt32(requestForm.CREATOR_ID), context);
                                var IsRoleMK = CNService.IsRoleMK(Convert.ToInt32(requestForm.CREATOR_ID), context);
                                if (IsMK || IsRoleMK)
                                {
                                    param.data.PROCESS.CURRENT_USER_ID = requestForm.CREATOR_ID;
                                }
                                else
                                {
                                    if (requestForm.REVIEWER_ID != null)
                                        param.data.PROCESS.CURRENT_USER_ID = requestForm.REVIEWER_ID;
                                    else
                                        param.data.PROCESS.CURRENT_USER_ID = requestForm.CREATOR_ID;
                                }
                            }
                            processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                        }

                        ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA MKData = new ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA();
                        MKData = MapperServices.ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA(param.data);

                        if (processResults != null && processResults.data != null && processResults.data.Count > 0)
                        {
                            MKData.ARTWORK_SUB_ID = processResults.data[0].ARTWORK_SUB_ID;
                        }

                        ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_SERVICE.SaveOrUpdate(MKData, context);

                        Results.data = new List<ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_2>();
                        ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_2 item = new ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_2();
                        List<ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_2> listItem = new List<ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_2>();

                        item.ARTWORK_PROCESS_MARKETING_BY_PA_ID = MKData.ARTWORK_PROCESS_MARKETING_BY_PA_ID;
                        listItem.Add(item);

                        Results.data = listItem;

                        dbContextTransaction.Commit();

                        foreach (var process in processResults.data)
                            EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO", context);

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

        public static ART_WF_ARTWORK_PROCESS_MARKETING_RESULT PostMKSendToPA(ART_WF_ARTWORK_PROCESS_MARKETING_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_MARKETING_RESULT Results = new ART_WF_ARTWORK_PROCESS_MARKETING_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var mkData = MapperServices.ART_WF_ARTWORK_PROCESS_MARKETING(param.data);

                        var check = ART_WF_ARTWORK_PROCESS_MARKETING_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_MARKETING() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                        if (check.Count > 0)
                            mkData.ARTWORK_PROCESS_MARKETING_ID = check.FirstOrDefault().ARTWORK_PROCESS_MARKETING_ID;

                        ART_WF_ARTWORK_PROCESS_MARKETING_SERVICE.SaveOrUpdate(mkData, context);

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
