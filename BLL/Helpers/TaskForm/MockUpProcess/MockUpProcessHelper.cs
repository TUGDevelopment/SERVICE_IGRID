using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services;
using DAL;
using DAL.Model;

namespace BLL.Helpers
{
    public class MockUpProcessHelper
    {
        public static ART_WF_MOCKUP_PROCESS_RESULT SubmitProcessAndPriceTemplate(ART_WF_MOCKUP_PROCESS_REQUEST_LIST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();

            try
            {
                var listProcess = new List<ART_WF_MOCKUP_PROCESS>();
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param.data != null && param.data.Count > 0)
                        {
                            foreach (ART_WF_MOCKUP_PROCESS_2 iProcess in param.data)
                            {
                                var process = MapperServices.ART_WF_MOCKUP_PROCESS(iProcess);

                                string msg = checkDupWF(iProcess, context);
                                if (msg != "")
                                {
                                    Results.status = "E";
                                    Results.msg = msg;
                                    return Results;
                                }

                                var oldMOCKUP_SUB_ID = process.MOCKUP_SUB_ID;
                                process.MOCKUP_SUB_ID = 0;
                                CNService.CheckDelegateBeforeRounting(process, context);
                                listProcess.Add(process);

                                //start save price template
                                ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE filter = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE();
                                filter.MOCKUP_SUB_ID = Convert.ToInt32(oldMOCKUP_SUB_ID);
                                var list = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetByItem(filter, context);
                                foreach (ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE item in list)
                                {
                                    var temp = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE();
                                    temp.PRICE = item.PRICE;
                                    temp.SCALE = item.SCALE;
                                    temp.USER_ID = process.CURRENT_USER_ID;
                                    temp.VENDOR_ID = process.CURRENT_VENDOR_ID;
                                    temp.CREATE_BY = item.CREATE_BY;
                                    temp.UPDATE_BY = item.UPDATE_BY;
                                    temp.MOCKUP_SUB_ID = process.MOCKUP_SUB_ID;
                                    temp.MOCKUP_ID = process.MOCKUP_ID;
                                    ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.SaveOrUpdate(temp, context);
                                }
                                //end save price template
                            }

                            dbContextTransaction.Commit();

                            foreach (var process in listProcess)
                            {
                                EmailService.sendEmailMockup(process.MOCKUP_ID, process.MOCKUP_SUB_ID, "WF_SEND_TO_VENDOR", context);
                            }
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

        public static ART_WF_MOCKUP_PROCESS_RESULT SubmitProcess(ART_WF_MOCKUP_PROCESS_REQUEST_LIST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        string MOCKUP_SUB_ID_NEW_TRANS = "";
                        string MOCKUP_SUB_ID_NO_SEND_QUO = "";
                        if (param.data != null && param.data.Count > 0)
                        {
                            ART_WF_MOCKUP_PROCESS_2 _mockup = new ART_WF_MOCKUP_PROCESS_2();
                            foreach (ART_WF_MOCKUP_PROCESS_2 iProcess in param.data)
                            {
                                ART_WF_MOCKUP_PROCESS process = new ART_WF_MOCKUP_PROCESS();
                                process = MapperServices.ART_WF_MOCKUP_PROCESS(iProcess);

                                string msg = checkDupWF(iProcess, context);
                                if (msg != "")
                                {
                                    Results.status = "E";
                                    Results.msg = msg;
                                    return Results;
                                }

                                var stepMoclupId = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG_SUP_SEL_VENDOR" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                                if (stepMoclupId == process.CURRENT_STEP_ID)
                                {
                                    var SEND_VN_QUO = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_VN_QUO" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                                    var listVendorNotSendQuo = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = process.MOCKUP_ID, CURRENT_STEP_ID = SEND_VN_QUO }, context);
                                    foreach (var item in listVendorNotSendQuo)
                                    {
                                        if (item.IS_END != "X")
                                        {
                                            item.IS_END = "X";
                                            item.UPDATE_BY = -1;
                                            ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(item, context);

                                            MOCKUP_SUB_ID_NO_SEND_QUO += "," + item.MOCKUP_SUB_ID;
                                        }
                                    }
                                }

                                var SEND_RD_PRI_PKG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_RD_PRI_PKG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                                if (SEND_RD_PRI_PKG == process.CURRENT_STEP_ID)
                                {
                                    var checkListId = CNService.ConvertMockupIdToCheckListId(process.MOCKUP_ID, context);
                                    var checkList = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(checkListId, context);
                                    if (checkList.RD_PERSON_ID > 0)
                                    {
                                        process.CURRENT_USER_ID = checkList.RD_PERSON_ID;
                                    }
                                }

                                process.CURRENT_USER_ID = CNService.GetLastestAction(process, context);
                                CNService.CheckDelegateBeforeRounting(process, context);
                                MOCKUP_SUB_ID_NEW_TRANS += "," + process.MOCKUP_SUB_ID;
                            }
                        }

                        dbContextTransaction.Commit();

                        foreach (var s in MOCKUP_SUB_ID_NO_SEND_QUO.Split(','))
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                EmailService.sendEmailMockup(param.data[0].MOCKUP_ID, Convert.ToInt32(s), "WF_SEND_TO_VENDOR_NO_SEND_QUO", context);
                            }
                        }
                        foreach (var s in MOCKUP_SUB_ID_NEW_TRANS.Split(','))
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                EmailService.sendEmailMockup(param.data[0].MOCKUP_ID, Convert.ToInt32(s), "WF_SEND_TO", context);
                            }
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

        public static string checkDupWF(ART_WF_MOCKUP_PROCESS_2 process, ARTWORKEntities context)
        {
            string msg = "[Duplicate] Cannot send workitem ";
            if (process.CURRENT_VENDOR_ID > 0 || process.CURRENT_CUSTOMER_ID > 0)
            {
                //cus or ven
                ART_WF_MOCKUP_PROCESS filter = new ART_WF_MOCKUP_PROCESS();
                filter.MOCKUP_ID = process.MOCKUP_ID;
                filter.CURRENT_USER_ID = process.CURRENT_USER_ID;
                filter.CURRENT_STEP_ID = process.CURRENT_STEP_ID;
                if (ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(filter, context).Where(m => m.IS_END == null).ToList().Count > 0)
                {
                    msg += " to " + CNService.GetUserName(process.CURRENT_USER_ID, context) + ".";
                    return msg;
                }
            }
            else
            {
                //internal
                ART_WF_MOCKUP_PROCESS filter = new ART_WF_MOCKUP_PROCESS();
                filter.MOCKUP_ID = process.MOCKUP_ID;
                filter.CURRENT_STEP_ID = process.CURRENT_STEP_ID;
                if (ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(filter, context).Where(m => m.IS_END == null).ToList().Count > 0)
                {
                    msg += " for " + ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(process.CURRENT_STEP_ID, context).STEP_MOCKUP_NAME + ".";
                    return msg;
                }
            }
            return "";
        }

        public static ART_WF_MOCKUP_PROCESS_RESULT SaveProcess(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();
            try
            {
                var listProcess = new List<ART_WF_MOCKUP_PROCESS>();

                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        context.Database.CommandTimeout = 300;

                        ART_WF_MOCKUP_CHECK_LIST_ITEM filter = new ART_WF_MOCKUP_CHECK_LIST_ITEM();
                        filter.CHECK_LIST_ID = param.data.CHECK_LIST_ID;
                        var checklists = (from item in context.ART_WF_MOCKUP_CHECK_LIST_ITEM
                                          join s in context.SAP_M_CHARACTERISTIC on item.PACKING_TYPE_ID equals s.CHARACTERISTIC_ID
                                          where s.NAME == "ZPKG_SEC_GROUP" && item.CHECK_LIST_ID == param.data.CHECK_LIST_ID
                                          select new { item, s.VALUE, s.DESCRIPTION }).ToList();//ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByItem(filter, context);

                        bool multiMockup = false;
                        string allMockup_no = ""; string oneMockup_no = "";
                        string checklistNo = "";
                        string mockup_no = "";
                        bool first = true;
                        var token = CWSService.getAuthToken();

                        //string aa = "<a data-toggle=\"modal\" data-pic_id=\"\' + row.PIC_ID + \'\" title=\"click\" href=\"#modal_pic_edit\">Upload Files</a>";

                        // string btnUpload = "    <button type=\"button\" class=\"btn btn-default btn-sm\" data-toggle=\"modal\" data-go_to_step=\"UPLOAD_CHECK_LIST\" data-mockup_id=\"{0}\" data-mockup_sub_id=\"{1}\" data-mockup_no=\"{2}\" href=\"#popup_attachment\">View/upload attachment</button><br>";
                        string btnUpload = "    <a  data-toggle=\"modal\" data-go_to_step=\"UPLOAD_CHECK_LIST\" data-mockup_id=\"{0}\" data-mockup_sub_id=\"{1}\" data-mockup_no=\"{2}\" href=\"#popup_attachment_checklist\">View/upload attachment</a>  <label id=\"cls_count_files{1}\"></label> ";

                        foreach (var checklistItem in checklists)
                        {
                            string packagingType = string.Format(" ({0}:{1}) ", checklistItem.VALUE, checklistItem.DESCRIPTION);
                            if (string.IsNullOrEmpty(checklistItem.item.MOCKUP_NO))
                            {
                                if (first)
                                {
                                    checklistNo = FormNumberHelper.UpdateCheckListNo(param.data.CHECK_LIST_ID, context);
                                    first = false;
                                }

                                ART_WF_MOCKUP_PROCESS process = new ART_WF_MOCKUP_PROCESS();

                                //save into process.
                                process.MOCKUP_ID = checklistItem.item.MOCKUP_ID;
                                process.CURRENT_ROLE_ID = param.data.CURRENT_ROLE_ID;
                                process.CURRENT_STEP_ID = param.data.CURRENT_STEP_ID;
                                process.CREATE_BY = param.data.CREATE_BY;
                                process.UPDATE_BY = param.data.UPDATE_BY;
                                if (CNService.IsPackaging(param.data.CREATE_BY, context))
                                {
                                    process.CURRENT_USER_ID = param.data.CREATE_BY;
                                }

                                CNService.CheckDelegateBeforeRounting(process, context);
                                listProcess.Add(process);

                                mockup_no = FormNumberHelper.GenMockUpNo(context, param.data.CHECK_LIST_ID);
                                checklistItem.item.MOCKUP_NO = mockup_no;
                                ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.SaveOrUpdateNoLog(checklistItem.item, context);

                                long folderID = Convert.ToInt64(ConfigurationManager.AppSettings["MockUpNodeID"]);
                                long templateID = Convert.ToInt64(ConfigurationManager.AppSettings["MockUpTemplateNodeID"]);
                                var node = CWSService.copyNode(mockup_no, templateID, folderID, token);

                                var temp = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(checklistItem.item.MOCKUP_ID, context);
                                temp.NODE_ID = node.ID;
                                ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.SaveOrUpdateNoLog(temp, context);

                                //Copy Check List Data to PG.
                                ART_WF_MOCKUP_CHECK_LIST checkList = new ART_WF_MOCKUP_CHECK_LIST();
                                checkList = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(checklistItem.item.CHECK_LIST_ID, context);
                                CheckListRequestHelper.CopyCheckListToPG(checkList, checklistItem.item, context);

                                //send to inbox by ref no
                                if (checkList.REFERENCE_REQUEST_TYPE == "CHECKLIST")
                                {
                                    var checkListId = Convert.ToInt32(checkList.REFERENCE_REQUEST_ID);
                                    var checkListItem = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_ITEM() { CHECK_LIST_ID = checkListId }, context);
                                    if (checkListItem.FirstOrDefault() != null)
                                    {
                                        var mockupId = checkListItem.FirstOrDefault().MOCKUP_ID;
                                        var tempListProcess = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = mockupId }, context);
                                        tempListProcess = tempListProcess.Where(m => m.PARENT_MOCKUP_SUB_ID == null).ToList();
                                        var tempCurrentUserId = tempListProcess.FirstOrDefault().CURRENT_USER_ID;
                                        if (tempCurrentUserId > 0)
                                        {
                                            process.CURRENT_USER_ID = tempCurrentUserId;
                                            CNService.CheckDelegateBeforeRounting(process, context);
                                        }
                                    }
                                }

                                if (allMockup_no == "")
                                {
                                    oneMockup_no = mockup_no + string.Format(btnUpload, checklistItem.item.MOCKUP_ID, process.MOCKUP_SUB_ID, checklistItem.item.MOCKUP_NO) + packagingType;
                                    allMockup_no = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + mockup_no + string.Format(btnUpload, checklistItem.item.MOCKUP_ID, process.MOCKUP_SUB_ID, checklistItem.item.MOCKUP_NO) + packagingType;
                                }
                                else
                                {
                                    multiMockup = true;
                                    allMockup_no += "<br>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + mockup_no + string.Format(btnUpload, checklistItem.item.MOCKUP_ID, process.MOCKUP_SUB_ID, checklistItem.item.MOCKUP_NO) + packagingType;
                                }

                                ART_WF_MOCKUP_PROCESS_PG_REQUEST ItemPGReq = new ART_WF_MOCKUP_PROCESS_PG_REQUEST();
                                ART_WF_MOCKUP_PROCESS_PG_2 ItemPG = new ART_WF_MOCKUP_PROCESS_PG_2();
                                ItemPG.MOCKUP_SUB_ID = process.MOCKUP_SUB_ID;
                                ItemPGReq.data = ItemPG;
                                PGFormHelper.SavePGProcess(ItemPGReq, context);
                            }
                            else
                            {
                                checklistNo = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(param.data.CHECK_LIST_ID, context).CHECK_LIST_NO;
                                var stepPG = context.ART_M_STEP_MOCKUP.Where(w => w.STEP_MOCKUP_CODE == "SEND_PG").Select(s => s.STEP_MOCKUP_ID).FirstOrDefault();

                                var mockupSubID = (from p in context.ART_WF_MOCKUP_PROCESS
                                                   where p.MOCKUP_ID == checklistItem.item.MOCKUP_ID
                                                   && p.CURRENT_STEP_ID == stepPG
                                                   select p.MOCKUP_SUB_ID).FirstOrDefault();

                                if (allMockup_no == "")
                                {
                                    oneMockup_no = checklistItem.item.MOCKUP_NO + string.Format(btnUpload, checklistItem.item.MOCKUP_ID, mockupSubID, checklistItem.item.MOCKUP_NO) + packagingType;
                                    allMockup_no = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + checklistItem.item.MOCKUP_NO + string.Format(btnUpload, checklistItem.item.MOCKUP_ID, mockupSubID, checklistItem.item.MOCKUP_NO) + packagingType;
                                }
                                else
                                {
                                    multiMockup = true;
                                    allMockup_no += "<br>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + checklistItem.item.MOCKUP_NO + string.Format(btnUpload, checklistItem.item.MOCKUP_ID, mockupSubID, checklistItem.item.MOCKUP_NO) + packagingType;
                                }
                            }
                        }

                        dbContextTransaction.Commit();

                        foreach (var process in listProcess)
                        {
                            EmailService.sendEmailMockup(process.MOCKUP_ID, process.MOCKUP_SUB_ID, "WF_SEND_TO", context);
                        }

                        Results.status = "S";
                        Results.msg = "Checklist no : " + checklistNo + "<br/>";

                        if (multiMockup)
                        {
                            Results.msg += "Mockup no : " + "<br/>";
                            Results.msg += allMockup_no;
                        }
                        else
                        {
                            Results.msg += "Mockup no : " + oneMockup_no;
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

        public static ART_WF_MOCKUP_PROCESS_RESULT GetProcess(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS(ART_WF_MOCKUP_PROCESS_SERVICE.GetAll(context));
                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS(ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(MapperServices.ART_WF_MOCKUP_PROCESS(param.data), context));
                        }

                        foreach (ART_WF_MOCKUP_PROCESS_2 item in Results.data)
                        {
                            var mockup = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(item.MOCKUP_ID, context);
                            item.MOCKUP_NO_DISPLAY_TXT = mockup.MOCKUP_NO;
                            item.CHECK_LIST_NO_DISPLAY_TXT = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(mockup.CHECK_LIST_ID, context).CHECK_LIST_NO;
                            item.CURRENT_STEP_DISPLAY_TXT = ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(item.CURRENT_STEP_ID, context).STEP_MOCKUP_NAME;
                            item.CURRENT_STEP_CODE_DISPLAY_TXT = ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(item.CURRENT_STEP_ID, context).STEP_MOCKUP_CODE;
                            item.NODE_ID_MOCKUP = mockup.NODE_ID;
                            if (item.STEP_DURATION_EXTEND_REASON_ID != null)
                            {
                                var reason = ART_M_DECISION_REASON_SERVICE.GetByART_M_DECISION_REASON_ID(item.STEP_DURATION_EXTEND_REASON_ID, context);
                                if (reason != null)
                                {
                                    item.STEP_DURATION_REMARK_REASON = reason.DESCRIPTION;
                                }
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

        public static ART_WF_MOCKUP_PROCESS_RESULT AcceptTask(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var stepId = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var temp = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(param.data.MOCKUP_SUB_ID, context);
                        if (temp.CURRENT_STEP_ID == stepId)
                        {
                            AcceptTask_AllInCheckList(param, context);
                        }
                        else
                        {
                            temp.CURRENT_USER_ID = param.data.CURRENT_USER_ID;
                            temp.UPDATE_BY = param.data.UPDATE_BY;
                            ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(temp, context);
                        }

                        dbContextTransaction.Commit();

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

        private static void AcceptTask_AllInCheckList(ART_WF_MOCKUP_PROCESS_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();

            var stepId = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;

            var temp = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(param.data.MOCKUP_SUB_ID, context);

            var mockupId = temp.MOCKUP_ID;
            var chekcListId = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(mockupId, context).CHECK_LIST_ID;

            var list = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_ITEM() { CHECK_LIST_ID = chekcListId }, context);
            foreach (var item in list)
            {
                var list2 = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = item.MOCKUP_ID }, context);
                foreach (var item2 in list2)
                {
                    if (item2.CURRENT_USER_ID == null && item2.CURRENT_STEP_ID == stepId)
                    {
                        item2.CURRENT_USER_ID = param.data.CURRENT_USER_ID;
                        item2.UPDATE_BY = param.data.UPDATE_BY;
                        ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(item2, context);

                        //ART_WF_MOCKUP_PROCESS_PG_REQUEST ItemPGReq = new ART_WF_MOCKUP_PROCESS_PG_REQUEST();
                        //ART_WF_MOCKUP_PROCESS_PG_2 ItemPG = new ART_WF_MOCKUP_PROCESS_PG_2();
                        //ItemPG.MOCKUP_SUB_ID = item2.MOCKUP_SUB_ID;
                        //ItemPGReq.data = ItemPG;
                        //PGFormHelper.SavePGProcess(ItemPGReq, context);
                    }
                }
            }
        }

        public static ART_WF_MOCKUP_PROCESS_RESULT EndTaskForm(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        EndTaskForm(param.data.MOCKUP_SUB_ID, param.data.UPDATE_BY, context);

                        dbContextTransaction.Commit();

                        Results.status = "S";
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

        public static void EndTaskForm(int MOCKUP_SUB_ID, int UPDATE_BY, ARTWORKEntities context)
        {
            var temp = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(MOCKUP_SUB_ID, context);

            temp.IS_END = "X";
            temp.UPDATE_BY = UPDATE_BY;

            ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(temp, context);
        }

        public static ART_WF_MOCKUP_PROCESS_RESULT PostPGSendBackMK(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        string msg = checkDupWF(param.data, context);
                        if (msg != "")
                        {
                            Results.status = "E";
                            Results.msg = msg;
                            return Results;
                        }

                        ART_WF_MOCKUP_PROCESS process = new ART_WF_MOCKUP_PROCESS();
                        process = MapperServices.ART_WF_MOCKUP_PROCESS(param.data);

                        var checkListId = CNService.ConvertMockupIdToCheckListId(param.data.MOCKUP_ID, context);
                        process.CURRENT_USER_ID = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(checkListId, context).CREATOR_ID;
                        process.MOCKUP_SUB_ID = 0;
                        CNService.CheckDelegateBeforeRounting(process, context);

                        dbContextTransaction.Commit();

                        EmailService.sendEmailMockup(process.MOCKUP_ID, process.MOCKUP_SUB_ID, "WF_SEND_BACK", context, param.data.REMARK.Replace("<p>", "").Replace("</p>", ""));

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

        public static ART_WF_MOCKUP_PROCESS_RESULT PostPGCompleteWF(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();

            try
            {
                ART_WF_MOCKUP_PROCESS_PG processPG = new ART_WF_MOCKUP_PROCESS_PG();
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var checkListid = CNService.ConvertMockupIdToCheckListId(param.data.MOCKUP_ID, context);

                        var checkList = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(checkListid, context);

                        if (checkList.REQUEST_FOR_DIE_LINE == "1")
                        {
                            processPG = ART_WF_MOCKUP_PROCESS_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG() { MOCKUP_SUB_ID = param.data.MOCKUP_SUB_ID }, context).FirstOrDefault();
                            if (processPG.DIE_LINE_MOCKUP_ID > 0)
                            {
                                //EmailService.sendEmailDieline(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "WF_SEND_DIELINE", Convert.ToInt32(processPG.DIE_LINE_MOCKUP_ID), context);
                            }
                            else
                            {
                                Results.status = "E";
                                Results.msg = "Please assign dieline before complete workflow.";
                                return Results;
                            }
                        }

                        EndTaskForm(param.data.MOCKUP_SUB_ID, param.data.UPDATE_BY, context);

                        var SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var listProcess = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID }, context);
                        listProcess = listProcess.Where(m => string.IsNullOrEmpty(m.IS_END)).ToList();
                        foreach (var item in listProcess)
                        {
                            if (item.CURRENT_STEP_ID != SEND_PG && item.MOCKUP_SUB_ID != param.data.MOCKUP_SUB_ID)
                                item.REMARK_KILLPROCESS = "Completed workflow by PG";

                            EndTaskForm(item.MOCKUP_SUB_ID, param.data.UPDATE_BY, context);
                        }

                        dbContextTransaction.Commit();

                        if (checkList.REQUEST_FOR_DIE_LINE == "1")
                        {
                            if (processPG.DIE_LINE_MOCKUP_ID > 0)
                            {
                                EmailService.sendEmailDieline(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "WF_SEND_DIELINE", Convert.ToInt32(processPG.DIE_LINE_MOCKUP_ID), context);
                            }
                        }

                        EmailService.sendEmailMockup(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "WF_COMPLETED", context);

                        Results.status = "S";
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

        public static ART_WF_MOCKUP_PROCESS_RESULT PostTerminateWFMockup(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var temp = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(param.data.MOCKUP_SUB_ID, context);
                        temp.IS_END = "X";
                        temp.IS_TERMINATE = "X";
                        temp.REMARK_TERMINATE = param.data.REMARK_TERMINATE;
                        temp.TERMINATE_REASON_CODE = param.data.TERMINATE_REASON_CODE;
                        ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(temp, context);

                        var listProcess = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = temp.MOCKUP_ID }, context);
                        listProcess = listProcess.Where(m => string.IsNullOrEmpty(m.IS_END)).ToList();
                        foreach (var item in listProcess)
                        {
                            EndTaskForm(item.MOCKUP_SUB_ID, -1, context);
                        }
                        dbContextTransaction.Commit();

                        EmailService.sendEmailMockup(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "WF_TEMINATED", context, param.data.REMARK_TERMINATE.Replace("<p>", "").Replace("</p>", ""));

                        Results.status = "S";
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

        public static ART_WF_MOCKUP_PROCESS_RESULT PostKillWFMockup(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param.data.REMARK_KILLPROCESS == "<p><br></p>" || param.data.REMARK_KILLPROCESS == null)
                        {
                            Results.status = "E";
                            Results.msg = "Please fill remark for terminate this workflow.";
                            return Results;
                        }

                        var temp = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(param.data.MOCKUP_SUB_ID, context);
                        temp.IS_END = "X";
                        temp.REMARK_KILLPROCESS = param.data.REMARK_KILLPROCESS;
                        temp.UPDATE_BY = param.data.UPDATE_BY;

                        ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(temp, context);

                        dbContextTransaction.Commit();

                        EmailService.sendEmailMockup(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "WF_TEMINATED_STEP", context, param.data.REMARK_KILLPROCESS.Replace("<p>", "").Replace("</p>", ""));

                        Results.status = "S";
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

        public static string CheckOverDue(ART_WF_MOCKUP_PROCESS process, ARTWORKEntities context)
        {
            string result = "0";//0:hide,1:show

            string isStepDurationExtend = process.IS_STEP_DURATION_EXTEND;
            string WFCompletedOrTerminated = process.IS_END == "X" || process.IS_TERMINATE == "X" || !string.IsNullOrEmpty(process.REMARK_KILLPROCESS) ? "1" : "0";
            var duration = !string.IsNullOrEmpty(isStepDurationExtend) ? ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(process.CURRENT_STEP_ID, context).DURATION_EXTEND : ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(process.CURRENT_STEP_ID, context).DURATION;
            DateTime dtReceiveWf = process.CREATE_DATE;
            DateTime dtWillFinish = CNService.AddBusinessDays(dtReceiveWf, (int)Math.Ceiling(duration.Value));
            if (DateTime.Now > dtWillFinish && WFCompletedOrTerminated.Equals("0"))
            {
                result = "1";
            }

            return result;
        }

        public static ART_WF_MOCKUP_PROCESS_RESULT SaveStepDurationExtend(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();
            ART_WF_MOCKUP_PROCESS process = new ART_WF_MOCKUP_PROCESS();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            process.MOCKUP_SUB_ID = param.data.MOCKUP_SUB_ID;
                            process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(process, context).FirstOrDefault();

                            if (process != null)
                            {
                                process.IS_STEP_DURATION_EXTEND = param.data.IS_STEP_DURATION_EXTEND;
                                process.STEP_DURATION_EXTEND_REASON_ID = param.data.STEP_DURATION_EXTEND_REASON_ID;
                                process.STEP_DURATION_EXTEND_REMARK = param.data.STEP_DURATION_EXTEND_REMARK;
                                ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(process, context);
                                Results.data = new List<ART_WF_MOCKUP_PROCESS_2>();
                                Results.data.Add(new ART_WF_MOCKUP_PROCESS_2 { IS_OVER_DUE = CheckOverDue(process, context) });
                            }
                        }
                        else
                        {
                            return Results;
                        }

                        dbContextTransaction.Commit();

                        //SendEail
                        var emailto = "";

                        var q = (from m in context.ART_WF_MOCKUP_PROCESS
                                 join m2 in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on m.MOCKUP_ID equals m2.MOCKUP_ID
                                 join m3 in context.ART_M_USER on m.UPDATE_BY equals m3.USER_ID
                                 join m4 in context.ART_M_POSITION on m3.POSITION_ID equals m4.ART_M_POSITION_ID
                                 where m.MOCKUP_SUB_ID == param.data.MOCKUP_SUB_ID
                                 select new
                                 {
                                     MOCKUP_NO = m2.MOCKUP_NO,
                                     MOCKUP_SUB_ID = m.MOCKUP_SUB_ID,
                                     CREATE_DATE = m2.CREATE_DATE,
                                     CREATE_BY = m2.CREATE_BY,
                                     POSITION_NAME = m4.ART_M_POSITION_NAME
                                 }).FirstOrDefault();

                        var stepMockup = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = param.data.STEP_CODE }, context).FirstOrDefault();
                        if (q != null && stepMockup != null)
                        {
                            DateTime dtReceiveWf = q.CREATE_DATE;
                            DateTime dtWillFinish = CNService.AddBusinessDays(dtReceiveWf, (int)Math.Ceiling(stepMockup.DURATION_EXTEND.Value));

                            var userInfo = ART_M_USER_SERVICE.GetByUSER_ID(q.CREATE_BY, context);
                            if (userInfo.IS_ACTIVE == "X")
                            {
                                emailto = userInfo.EMAIL;
                            }

                            EmailService.SendEmailExtendDuration(emailto, q.MOCKUP_NO, dtWillFinish, q.POSITION_NAME, stepMockup.STEP_MOCKUP_NAME, q.MOCKUP_SUB_ID, null, null);
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
