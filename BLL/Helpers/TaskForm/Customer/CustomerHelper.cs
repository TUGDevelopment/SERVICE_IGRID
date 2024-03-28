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
    public class CustomerHelper
    {
        public static ART_WF_MOCKUP_PROCESS_CUSTOMER_RESULT GetCustomer(ART_WF_MOCKUP_PROCESS_CUSTOMER_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_CUSTOMER_RESULT Results = new ART_WF_MOCKUP_PROCESS_CUSTOMER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_CUSTOMER(ART_WF_MOCKUP_PROCESS_CUSTOMER_SERVICE.GetAll(context));
                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_CUSTOMER(ART_WF_MOCKUP_PROCESS_CUSTOMER_SERVICE.GetByItemContain(MapperServices.ART_WF_MOCKUP_PROCESS_CUSTOMER(param.data), context));
                        }
                        var STEP_MOCKUP_CODE = "SEND_CUS_APP";
                        if (param.data.isProjectNoCus == "1") STEP_MOCKUP_CODE = "SEND_MK_APP";
                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                ART_SYS_ACTION act = new ART_SYS_ACTION();
                                act.ACTION_CODE = Results.data[i].ACTION_CODE;
                                Results.data[i].ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItem(act, context).FirstOrDefault().ACTION_NAME;

                                var processFormPG = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context);

                                if (ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).CURRENT_CUSTOMER_ID > 0)
                                {
                                    Results.data[i].CUSTOMER_DISPLAY_TXT = CNService.GetUserName(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).CURRENT_USER_ID, context);
                                    Results.data[i].CUSTOMER_DISPLAY_TXT += "<br/>" + XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).CURRENT_CUSTOMER_ID, context).CUSTOMER_NAME;
                                }
                                else
                                {
                                    Results.data[i].CUSTOMER_DISPLAY_TXT = CNService.GetUserName(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).CURRENT_USER_ID, context);
                                    Results.data[i].CUSTOMER_DISPLAY_TXT += "<br/>" + "Marketing";
                                }
                                Results.data[i].COMMENT_BY_PG = processFormPG.REMARK;
                                Results.data[i].REASON_BY_PG = CNService.getReason(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).REASON_ID, context);
                                Results.data[i].CREATE_DATE_BY_PG = processFormPG.CREATE_DATE;
                                Results.data[i].REMARK_REASON_BY_PG = CNService.getRemarkReason(Results.data[i].MOCKUP_SUB_ID, "M", STEP_MOCKUP_CODE, context);

                                if (Results.data[i].DECISION == "APPROVE")
                                    Results.data[i].DECISION_DISPLAY_TXT = "Approve";
                                if (Results.data[i].DECISION == "REVISE")
                                {
                                    var desc = "";
                                    var temp = ART_M_DECISION_REASON_SERVICE.GetByART_M_DECISION_REASON_ID(Results.data[i].REVISE_ID, context);
                                    if (temp != null)
                                    {
                                        desc = " [" + temp.DESCRIPTION + "]";
                                    }
                                    Results.data[i].DECISION_DISPLAY_TXT = "Revise" + desc;
                                    Results.data[i].DECISION_DISPLAY_TXT2 = temp.DESCRIPTION;
                                }
                                if (Results.data[i].DECISION == "CANCEL")
                                {
                                    var desc = "";
                                    var temp = ART_M_DECISION_REASON_SERVICE.GetByART_M_DECISION_REASON_ID(Results.data[i].CANCEL_ID, context);
                                    if (temp != null)
                                    {
                                        desc = " [" + temp.DESCRIPTION + "]";
                                    }
                                    Results.data[i].DECISION_DISPLAY_TXT = "Cancel" + desc;
                                    Results.data[i].DECISION_DISPLAY_TXT2 = temp.DESCRIPTION;
                                }

                                var formPG = ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG() { MOCKUP_SUB_ID = Results.data[i].MOCKUP_SUB_ID }, context);
                                if (formPG != null)
                                {
                                    Results.data[i].PACKING_STYLE = formPG.FirstOrDefault().PACKING_STYLE;
                                    Results.data[i].PURPOSE_OF = formPG.FirstOrDefault().PURPOSE_OF;
                                    if (formPG.FirstOrDefault().IS_SEND_DIE_LINE == "X")
                                        Results.data[i].TOPIC_DISPLAY_TXT = "Send die-line";
                                    else Results.data[i].TOPIC_DISPLAY_TXT = "Send physical/soft file";
                                }
                            }
                        }
                        
                        var stepId = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = STEP_MOCKUP_CODE }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var list = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID, CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var results = list.Where(p => !Results.data.Any(p2 => p2.MOCKUP_SUB_ID == p.MOCKUP_SUB_ID));

                        var checkListId = CNService.ConvertMockupIdToCheckListId(param.data.MOCKUP_ID, context);
                        var checkListCustomer = ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER() { CHECK_LIST_ID = checkListId }, context);

                        foreach (var result in results)
                        {
                            var checklistid = CNService.ConvertMockupIdToCheckListId(result.MOCKUP_ID, context);
                            var isTo = false;
                            var isCC = false;
                            if (ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(result.MOCKUP_SUB_ID, context).CURRENT_CUSTOMER_ID > 0)
                            {
                                if (checkListCustomer.Where(m => m.CUSTOMER_USER_ID == result.CURRENT_USER_ID && m.MAIL_TO == "X" && m.CHECK_LIST_ID == checklistid).Count() > 0 && result.CURRENT_USER_ID == param.data.CURRENT_USER_ID)
                                    isTo = true;
                                else if (checkListCustomer.Where(m => m.CUSTOMER_USER_ID == result.CURRENT_USER_ID && m.MAIL_CC == "X" && m.CHECK_LIST_ID == checklistid).Count() > 0)
                                    isCC = true;
                                else if (ART_WF_LOG_REASSIGN_SERVICE.GetByItem(new ART_WF_LOG_REASSIGN { WF_TYPE = "M", WF_SUB_ID = result.MOCKUP_SUB_ID, TO_USER_ID = result.CURRENT_USER_ID }, context).Count() > 0 && result.CURRENT_USER_ID == param.data.CURRENT_USER_ID)
                                    isTo = true;
                            }
                            else
                            {
                                isTo = true;
                            }

                            if (isTo)
                            {
                                ART_WF_MOCKUP_PROCESS_CUSTOMER_2 item = new ART_WF_MOCKUP_PROCESS_CUSTOMER_2();
                                item.CREATE_DATE_BY_PG = result.CREATE_DATE;
                                item.COMMENT_BY_PG = result.REMARK;
                                if (ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(result.MOCKUP_SUB_ID, context).REASON_ID > 0)
                                    item.REASON_BY_PG = CNService.getReason(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(result.MOCKUP_SUB_ID, context).REASON_ID, context);

                                var formPG = ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG() { MOCKUP_SUB_ID = result.MOCKUP_SUB_ID }, context);
                                if (formPG != null)
                                {
                                    item.PACKING_STYLE = formPG.FirstOrDefault().PACKING_STYLE;
                                    item.PURPOSE_OF = formPG.FirstOrDefault().PURPOSE_OF;
                                    if (formPG.FirstOrDefault().IS_SEND_DIE_LINE == "X")
                                        item.TOPIC_DISPLAY_TXT = "Send die-line";
                                    else item.TOPIC_DISPLAY_TXT = "Send physical/soft file";
                                }

                                if (ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(result.MOCKUP_SUB_ID, context).CURRENT_CUSTOMER_ID > 0)
                                {
                                    item.CUSTOMER_DISPLAY_TXT = CNService.GetUserName(result.CURRENT_USER_ID, context);
                                    item.CUSTOMER_DISPLAY_TXT += "<br/>" + XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(result.MOCKUP_SUB_ID, context).CURRENT_CUSTOMER_ID, context).CUSTOMER_NAME;
                                }
                                else
                                {
                                    item.CUSTOMER_DISPLAY_TXT = CNService.GetUserName(result.CURRENT_USER_ID, context);
                                    item.CUSTOMER_DISPLAY_TXT += "<br/>" + "Marketing";
                                }

                                Results.data.Add(item);
                            }
                            if (isCC)
                            {
                                ART_WF_MOCKUP_PROCESS_CUSTOMER_2 item = new ART_WF_MOCKUP_PROCESS_CUSTOMER_2();
                                item.CREATE_DATE_BY_PG = result.CREATE_DATE;
                                item.COMMENT_BY_PG = result.REMARK;
                                if (ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(result.MOCKUP_SUB_ID, context).REASON_ID > 0)
                                    item.REASON_BY_PG = CNService.getReason(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(result.MOCKUP_SUB_ID, context).REASON_ID, context);

                                var formPG = ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG() { MOCKUP_SUB_ID = result.MOCKUP_SUB_ID }, context);
                                if (formPG != null)
                                {
                                    item.PACKING_STYLE = formPG.FirstOrDefault().PACKING_STYLE;
                                    item.PURPOSE_OF = formPG.FirstOrDefault().PURPOSE_OF;
                                    if (formPG.FirstOrDefault().IS_SEND_DIE_LINE == "X")
                                        item.TOPIC_DISPLAY_TXT = "Send die-line";
                                    else item.TOPIC_DISPLAY_TXT = "Send physical/soft file";
                                }

                                if (ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(result.MOCKUP_SUB_ID, context).CURRENT_CUSTOMER_ID > 0)
                                {
                                    item.CUSTOMER_DISPLAY_TXT = CNService.GetUserName(result.CURRENT_USER_ID, context);
                                    item.CUSTOMER_DISPLAY_TXT += "<br/>" + XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(result.MOCKUP_SUB_ID, context).CURRENT_CUSTOMER_ID, context).CUSTOMER_NAME;
                                }
                                else
                                {
                                    item.CUSTOMER_DISPLAY_TXT = CNService.GetUserName(result.CURRENT_USER_ID, context);
                                    item.CUSTOMER_DISPLAY_TXT += "<br/>" + "Marketing";
                                }

                                Results.data.Add(item);
                            }
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

        public static ART_WF_MOCKUP_PROCESS_CUSTOMER_RESULT SaveCustomer(ART_WF_MOCKUP_PROCESS_CUSTOMER_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_CUSTOMER_RESULT Results = new ART_WF_MOCKUP_PROCESS_CUSTOMER_RESULT();

            try
            {
                ART_WF_MOCKUP_PROCESS newProcessMK = new ART_WF_MOCKUP_PROCESS();
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        ART_WF_MOCKUP_PROCESS_CUSTOMER customerData = new ART_WF_MOCKUP_PROCESS_CUSTOMER();
                        customerData = MapperServices.ART_WF_MOCKUP_PROCESS_CUSTOMER(param.data);

                        ART_WF_MOCKUP_PROCESS_CUSTOMER_SERVICE.SaveOrUpdate(customerData, context);

                        Results.data = new List<ART_WF_MOCKUP_PROCESS_CUSTOMER_2>();
                        ART_WF_MOCKUP_PROCESS_CUSTOMER_2 item = new ART_WF_MOCKUP_PROCESS_CUSTOMER_2();
                        item.MOCKUP_SUB_CUSTOMER_ID = customerData.MOCKUP_SUB_CUSTOMER_ID;

                        Results.data.Add(item);

                        var checkListId = CNService.ConvertMockupIdToCheckListId(param.data.MOCKUP_ID, context);
                        var checkList = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(checkListId, context);
                        if (param.data.ENDTASKFORM)
                        {
                            MockUpProcessHelper.EndTaskForm(param.data.MOCKUP_SUB_ID, param.data.UPDATE_BY, context);

                            //end process for customer user id not action.
                            var SEND_CUS_APP = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_CUS_APP" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                            var otherProcessCus = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID, CURRENT_STEP_ID = SEND_CUS_APP }, context);
                            otherProcessCus = otherProcessCus.Where(m => string.IsNullOrEmpty(m.IS_END)).ToList();
                            foreach (var itemProcessCus in otherProcessCus)
                            {
                                if (itemProcessCus.MOCKUP_SUB_ID != param.data.MOCKUP_SUB_ID)
                                    MockUpProcessHelper.EndTaskForm(itemProcessCus.MOCKUP_SUB_ID, -1, context);
                            }

                            if (param.data.ACTION_CODE == "SUBMIT")
                            {
                                if (param.data.DECISION == "APPROVE")
                                {
                                    //send wf to mk
                                    var isSelectRD = false;
                                    if (ART_WF_MOCKUP_CHECK_LIST_REFERENCE_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_REFERENCE() { CHECK_LIST_ID = checkListId }, context).Count > 0)
                                    {
                                        isSelectRD = true;
                                    }

                                    if (isSelectRD)
                                    {
                                        if (checkList.TWO_P_ID == -1)
                                        {
                                            var temp = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_MK_UPD_PACK_STYLE" }, context).FirstOrDefault();
                                            newProcessMK.CURRENT_STEP_ID = temp.STEP_MOCKUP_ID;
                                            //process.CURRENT_ROLE_ID = temp.ROLE_ID_RESPONSE;
                                            newProcessMK.CURRENT_USER_ID = checkList.CREATOR_ID;
                                            newProcessMK.UPDATE_BY = -1;
                                            newProcessMK.CREATE_BY = -1;
                                            newProcessMK.REMARK = "Automatic create workitem by system.";
                                            newProcessMK.MOCKUP_ID = param.data.MOCKUP_ID;
                                            newProcessMK.PARENT_MOCKUP_SUB_ID = param.data.MOCKUP_SUB_ID;
                                            CNService.CheckDelegateBeforeRounting(newProcessMK, context);
                                        }
                                        else
                                        {
                                            //complete wf
                                            var SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                                            var MOCKUP_SUB_ID = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID, CURRENT_STEP_ID = SEND_PG }, context).FirstOrDefault().MOCKUP_SUB_ID;
                                            MockUpProcessHelper.EndTaskForm(MOCKUP_SUB_ID, -1, context);
                                        }
                                    }
                                    else
                                    {
                                        //complete wf
                                        var SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                                        var MOCKUP_SUB_ID = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID, CURRENT_STEP_ID = SEND_PG }, context).FirstOrDefault().MOCKUP_SUB_ID;
                                        MockUpProcessHelper.EndTaskForm(MOCKUP_SUB_ID, -1, context);
                                    }
                                }
                            }
                        }

                        dbContextTransaction.Commit();

                        if (param.data.ACTION_CODE == "SUBMIT")
                        {
                            var tempProcess = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(param.data.MOCKUP_SUB_ID, context);
                            if (param.data.DECISION == "APPROVE")
                            {
                                EmailService.sendEmailMockup(param.data.MOCKUP_ID, Convert.ToInt32(tempProcess.PARENT_MOCKUP_SUB_ID), "WF_CUSTOMER_APPROVE", context);

                                var isSelectRD = false;
                                if (ART_WF_MOCKUP_CHECK_LIST_REFERENCE_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_REFERENCE() { CHECK_LIST_ID = checkListId }, context).Count > 0)
                                {
                                    isSelectRD = true;
                                }

                                if (isSelectRD)
                                {
                                    if (checkList.TWO_P_ID == -1)
                                    {
                                        EmailService.sendEmailMockup(param.data.MOCKUP_ID, newProcessMK.MOCKUP_SUB_ID, "WF_SEND_TO", context);
                                    }
                                }
                                else
                                {
                                    EmailService.sendEmailMockup(param.data.MOCKUP_ID, Convert.ToInt32(tempProcess.PARENT_MOCKUP_SUB_ID), "WF_COMPLETED", context);
                                }
                            }
                            else
                            {
                                EmailService.sendEmailMockup(param.data.MOCKUP_ID, Convert.ToInt32(tempProcess.PARENT_MOCKUP_SUB_ID), "WF_CUSTOMER_CANCEL", context, param.data.COMMENT);
                            }
                        }

                        if (param.data.ACTION_CODE == "SEND_BACK")
                            EmailService.sendEmailMockup(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "WF_SEND_BACK", context, param.data.COMMENT);
                        else if (param.data.ACTION_CODE == "SAVE")
                            EmailService.sendEmailMockup(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "WF_OTHER_SAVE", context);
                        else
                            EmailService.sendEmailMockup(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "WF_OTHER_SUBMIT", context);

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
