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
    public class VendorHelper
    {
        public static ART_WF_MOCKUP_PROCESS_VENDOR_RESULT GetVendor(ART_WF_MOCKUP_PROCESS_VENDOR_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_VENDOR_RESULT Results = new ART_WF_MOCKUP_PROCESS_VENDOR_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_VENDOR(ART_WF_MOCKUP_PROCESS_VENDOR_SERVICE.GetByItemContain(MapperServices.ART_WF_MOCKUP_PROCESS_VENDOR(param.data), context));
                            Results.data = Results.data.Where(s => s.MOCKUP_ID.Equals(param.data.MOCKUP_ID)).ToList(); // by aof 03/11/2021
                        }
                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                var SEND_TO_VENDOR_TYPE = ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG() { MOCKUP_SUB_ID = Results.data[i].MOCKUP_SUB_ID }, context).FirstOrDefault().SEND_TO_VENDOR_TYPE;
                                if (SEND_TO_VENDOR_TYPE == param.data.SEND_TO_VENDOR_TYPE)
                                {
                                    ART_SYS_ACTION act = new ART_SYS_ACTION();
                                    act.ACTION_CODE = Results.data[i].ACTION_CODE;
                                    var temp = ART_SYS_ACTION_SERVICE.GetByItem(act, context);
                                    if (temp != null)
                                        Results.data[i].ACTION_NAME = temp.FirstOrDefault().ACTION_NAME;

                                    Results.data[i].COMMENT_BY_PG = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).REMARK;
                                    Results.data[i].REASON_BY_PG = CNService.getReason(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).REASON_ID, context);
                                    if (Results.data[i].REASON_ID > 0) Results.data[i].REASON_BY_OTHER = CNService.getReason(Results.data[i].REASON_ID, context);
                                    Results.data[i].CREATE_DATE_BY_PG = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).CREATE_DATE;
                                    Results.data[i].SAMPLE_AMOUNT_FOR_REQ_SAMPLE_DIELINE = Results.data[i].SAMPLE_AMOUNT_FOR_REQ_SAMPLE_DIELINE;
                                    Results.data[i].SAMPLE_AMOUNT_FOR_SEND_PRIMARY = Results.data[i].SAMPLE_AMOUNT_FOR_SEND_PRIMARY;

                                    Results.data[i].VENDOR_DISPLAY_TXT = CNService.GetUserName(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).CURRENT_USER_ID, context);
                                    Results.data[i].VENDOR_DISPLAY_TXT += "<br/>" + XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).CURRENT_VENDOR_ID, context).VENDOR_NAME;
                                    Results.data[i].REMARK_REASON_BY_PG = CNService.getRemarkReason(Results.data[i].MOCKUP_SUB_ID, "M", "SEND_PG", context);
                                    //rewrited by aof #INC-11265
                                    //Results.data[i].REMARK_REASON = CNService.getRemarkReason(Results.data[i].MOCKUP_SUB_ID, "M", Results.data[i].STEP_MOCKUP_CODE, context);    
                                    Results.data[i].REMARK_REASON = CNService.getRemarkReason(Results.data[i].MOCKUP_SUB_ID, "M", getStepMockupCodebyProcess(Results.data[i].MOCKUP_SUB_ID,context), context);
                                    //rewrited by aof #INC-11265

                                }
                                else
                                {
                                    Results.data[i].SEND_TO_VENDOR_TYPE = "delete";
                                }
                            }

                            Results.data = Results.data.Where(m => m.SEND_TO_VENDOR_TYPE != "delete").ToList();
                        }

                        var stepId = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = param.data.STEP_MOCKUP_CODE }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        //var stepId = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_VN_RS" }).FirstOrDefault().STEP_MOCKUP_ID;
                        var list = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID, CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var results = list.Where(p => !Results.data.Any(p2 => p2.MOCKUP_SUB_ID == p.MOCKUP_SUB_ID));
                        foreach (var result in results)
                        {
                            var isTo = false;
                            if (ART_M_USER_VENDOR_SERVICE.GetByItem(new ART_M_USER_VENDOR() { USER_ID = Convert.ToInt32(result.CURRENT_USER_ID), IS_EMAIL_TO = "X" }, context).Count > 0)
                                isTo = true;

                            if (isTo)
                            {
                                ART_WF_MOCKUP_PROCESS_VENDOR_2 item = new ART_WF_MOCKUP_PROCESS_VENDOR_2();
                                item.CREATE_DATE_BY_PG = result.CREATE_DATE;
                                item.COMMENT_BY_PG = result.REMARK;
                                item.REASON_BY_PG = CNService.getReason(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(result.MOCKUP_SUB_ID, context).REASON_ID, context);
                                item.REMARK_REASON_BY_PG = CNService.getRemarkReason(result.MOCKUP_SUB_ID, "M", "SEND_PG", context);   //appended by aof #INC-11265

                                item.VENDOR_DISPLAY_TXT = CNService.GetUserName(result.CURRENT_USER_ID, context);
                                item.VENDOR_DISPLAY_TXT += "<br/>" + XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(result.CURRENT_VENDOR_ID, context).VENDOR_NAME;

                                Results.data.Add(item);
                            }
                        }
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

        //rewrited by aof #INC-11265
        public static string getStepMockupCodebyProcess(int MOCKUP_SUB_ID, ARTWORKEntities context) {

            var STEP_MOCKUP_CODE = (from p in context.ART_WF_MOCKUP_PROCESS
                                    join s in context.ART_M_STEP_MOCKUP on p.CURRENT_STEP_ID equals s.STEP_MOCKUP_ID
                                    where p.MOCKUP_SUB_ID == MOCKUP_SUB_ID
                                    select s.STEP_MOCKUP_CODE).FirstOrDefault();

            if (STEP_MOCKUP_CODE == null) {
                STEP_MOCKUP_CODE = "";
            }
            return STEP_MOCKUP_CODE;

        }

        public static PROCESS_VENDOR_RESULT GetVendorHeader(ART_WF_MOCKUP_PROCESS_VENDOR_REQUEST param)
        {
            PROCESS_VENDOR_RESULT Results = new PROCESS_VENDOR_RESULT();
            List<PROCESS_VENDOR> listProcessVendors = new List<PROCESS_VENDOR>();
            PROCESS_VENDOR _processVendor = new PROCESS_VENDOR();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            //Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_VENDOR(ART_WF_MOCKUP_PROCESS_VENDOR_SERVICE.GetAll());
                        }
                        else
                        {
                            // _processVendor.CHECKLIST_DATA = MapperServices.ART_WF_MOCKUP_PROCESS_VENDOR(ART_WF_MOCKUP_PROCESS_VENDOR_SERVICE.GetByItemContain(MapperServices.ART_WF_MOCKUP_PROCESS_VENDOR(param.data)));

                            ART_WF_MOCKUP_CHECK_LIST checklist = new ART_WF_MOCKUP_CHECK_LIST();
                            ART_WF_MOCKUP_CHECK_LIST_ITEM checklist_item = new ART_WF_MOCKUP_CHECK_LIST_ITEM();
                            ART_WF_MOCKUP_PROCESS_PG pg = new ART_WF_MOCKUP_PROCESS_PG();
                            ART_WF_MOCKUP_PROCESS_2 process2 = new ART_WF_MOCKUP_PROCESS_2();

                            checklist_item.MOCKUP_ID = param.data.MOCKUP_ID;
                            checklist_item = MapperServices.ART_WF_MOCKUP_CHECK_LIST_ITEM(ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByItem(checklist_item, context).FirstOrDefault());

                            if (checklist_item != null)
                            {
                                ART_WF_MOCKUP_PROCESS_REQUEST Request_CheckList = new ART_WF_MOCKUP_PROCESS_REQUEST();
                                ART_WF_MOCKUP_CHECK_LIST_RESULT Results_CheckList = new ART_WF_MOCKUP_CHECK_LIST_RESULT();

                                process2.CHECK_LIST_ID = checklist_item.CHECK_LIST_ID;
                                process2.MOCKUP_SUB_ID = param.data.MOCKUP_SUB_ID;
                                Request_CheckList.data = process2;
                                Results_CheckList = CheckListInfoHelper.GetCheckListInfo(Request_CheckList);

                                if (Results_CheckList != null && Results_CheckList.data != null && Results_CheckList.data.Count > 0)
                                {
                                    _processVendor.CHECKLIST_DATA = Results_CheckList.data.FirstOrDefault();
                                }

                                ART_WF_MOCKUP_PROCESS_PG_RESULT Result_PG = new ART_WF_MOCKUP_PROCESS_PG_RESULT();
                                ART_WF_MOCKUP_PROCESS_PG_REQUEST Request_PG = new ART_WF_MOCKUP_PROCESS_PG_REQUEST();

                                Result_PG = PGFormHelper.GetPGForm(Request_PG);

                                if (Result_PG != null && Result_PG.data != null && Result_PG.data.Count > 0)
                                {
                                    _processVendor.PG_DATA = Result_PG.data.FirstOrDefault();
                                }
                                pg.MOCKUP_SUB_ID = param.data.MOCKUP_SUB_ID;
                                _processVendor.PG_DATA = MapperServices.ART_WF_MOCKUP_PROCESS_PG(ART_WF_MOCKUP_PROCESS_PG_SERVICE.GetByItem(pg, context).FirstOrDefault());

                                listProcessVendors.Add(_processVendor);

                                Results.data = listProcessVendors;
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


        //commented by aof #INC-11265
        //public static ART_WF_MOCKUP_PROCESS_VENDOR_RESULT SaveVendor(ART_WF_MOCKUP_PROCESS_VENDOR_REQUEST param)
        //{
        //    ART_WF_MOCKUP_PROCESS_VENDOR_RESULT Results = new ART_WF_MOCKUP_PROCESS_VENDOR_RESULT();

        //    try
        //    {
        //        using (var context = new ARTWORKEntities())
        //        {
        //            using (var dbContextTransaction = CNService.IsolationLevel(context))
        //            {
        //                List<string> listStepMOCheckRequireAttachment = new List<string>();
        //                listStepMOCheckRequireAttachment.Add("SEND_VN_DL");
        //                listStepMOCheckRequireAttachment.Add("SEND_VN_MB");

        //                if (param.data.ACTION_CODE == "SUBMIT" && listStepMOCheckRequireAttachment.Contains(param.data.STEP_MOCKUP_CODE))
        //                {
        //                    var attachments = (from a in context.ART_WF_MOCKUP_ATTACHMENT
        //                                       where a.MOCKUP_SUB_ID == param.data.MOCKUP_SUB_ID
        //                                       select a).Count();

        //                    if (attachments <= 0)
        //                    {
        //                        Results.status = "E";
        //                        Results.msg = MessageHelper.GetMessage("MSG_028", context);

        //                        return Results;
        //                    }
        //                }

        //                ART_WF_MOCKUP_PROCESS_VENDOR vendorData = new ART_WF_MOCKUP_PROCESS_VENDOR();
        //                vendorData = MapperServices.ART_WF_MOCKUP_PROCESS_VENDOR(param.data);

        //                ART_WF_MOCKUP_PROCESS_VENDOR_SERVICE.SaveOrUpdate(vendorData, context);

        //                Results.data = new List<ART_WF_MOCKUP_PROCESS_VENDOR_2>();
        //                ART_WF_MOCKUP_PROCESS_VENDOR_2 item = new ART_WF_MOCKUP_PROCESS_VENDOR_2();
        //                item.MOCKUP_SUB_VENDOR_ID = vendorData.MOCKUP_SUB_VENDOR_ID;

        //                Results.data.Add(item);

        //                if (param.data.ENDTASKFORM)
        //                {
        //                    MockUpProcessHelper.EndTaskForm(param.data.MOCKUP_SUB_ID, param.data.UPDATE_BY, context);

        //                    //end process for vendor user id not action.
        //                    var vendorId = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(param.data.MOCKUP_SUB_ID, context).CURRENT_VENDOR_ID;
        //                    var STEP_MOCKUP_CODE = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = param.data.STEP_MOCKUP_CODE }, context).FirstOrDefault().STEP_MOCKUP_ID;
        //                    var otherProcessCus = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS()
        //                    {
        //                        MOCKUP_ID = param.data.MOCKUP_ID,
        //                        CURRENT_STEP_ID = STEP_MOCKUP_CODE,
        //                        CURRENT_VENDOR_ID = vendorId
        //                    }, context);
        //                    otherProcessCus = otherProcessCus.Where(m => string.IsNullOrEmpty(m.IS_END)).ToList();
        //                    foreach (var itemProcessCus in otherProcessCus)
        //                    {
        //                        if (itemProcessCus.MOCKUP_SUB_ID != param.data.MOCKUP_SUB_ID)
        //                            MockUpProcessHelper.EndTaskForm(itemProcessCus.MOCKUP_SUB_ID, -1, context);
        //                    }
        //                }

        //                dbContextTransaction.Commit();

        //                if (param.data.ACTION_CODE == "SEND_BACK")
        //                    EmailService.sendEmailMockup(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "WF_SEND_BACK", context, param.data.COMMENT);
        //                else if (param.data.ACTION_CODE == "SAVE")
        //                    EmailService.sendEmailMockup(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "WF_OTHER_SAVE", context);
        //                else
        //                    EmailService.sendEmailMockup(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "WF_OTHER_SUBMIT", context);

        //                Results.status = "S";
        //                Results.msg = MessageHelper.GetMessage("MSG_001", context);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Results.status = "E";
        //        Results.msg = CNService.GetErrorMessage(ex);
        //    }
        //    return Results;
        //}

        //rewrited by aof #INC-11265 
        public static ART_WF_MOCKUP_PROCESS_VENDOR_RESULT SaveVendor(ART_WF_MOCKUP_PROCESS_VENDOR_REQUEST_LIST param_)
        {
            ART_WF_MOCKUP_PROCESS_VENDOR_RESULT Results = new ART_WF_MOCKUP_PROCESS_VENDOR_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {

                        foreach (var param in param_.data)
                        {


                            List<string> listStepMOCheckRequireAttachment = new List<string>();
                            listStepMOCheckRequireAttachment.Add("SEND_VN_DL");
                            listStepMOCheckRequireAttachment.Add("SEND_VN_MB");

                            if (param.ACTION_CODE == "SUBMIT" && listStepMOCheckRequireAttachment.Contains(param.STEP_MOCKUP_CODE))
                            {
                                var attachments = (from a in context.ART_WF_MOCKUP_ATTACHMENT
                                                   where a.MOCKUP_SUB_ID == param.MOCKUP_SUB_ID
                                                   select a).Count();

                                if (attachments <= 0)
                                {
                                    Results.status = "E";
                                    Results.msg = MessageHelper.GetMessage("MSG_028", context);

                                    return Results;
                                }
                            }

                            ART_WF_MOCKUP_PROCESS_VENDOR vendorData = new ART_WF_MOCKUP_PROCESS_VENDOR();
                            vendorData = MapperServices.ART_WF_MOCKUP_PROCESS_VENDOR(param);

                            ART_WF_MOCKUP_PROCESS_VENDOR_SERVICE.SaveOrUpdate(vendorData, context);

                            Results.data = new List<ART_WF_MOCKUP_PROCESS_VENDOR_2>();
                            ART_WF_MOCKUP_PROCESS_VENDOR_2 item = new ART_WF_MOCKUP_PROCESS_VENDOR_2();
                            item.MOCKUP_SUB_VENDOR_ID = vendorData.MOCKUP_SUB_VENDOR_ID;

                            Results.data.Add(item);

                            if (param.ENDTASKFORM)
                            {
                                MockUpProcessHelper.EndTaskForm(param.MOCKUP_SUB_ID, param.UPDATE_BY, context);

                                //end process for vendor user id not action.
                                var vendorId = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(param.MOCKUP_SUB_ID, context).CURRENT_VENDOR_ID;
                                var STEP_MOCKUP_CODE = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = param.STEP_MOCKUP_CODE }, context).FirstOrDefault().STEP_MOCKUP_ID;
                                var otherProcessCus = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS()
                                {
                                    MOCKUP_ID = param.MOCKUP_ID,
                                    CURRENT_STEP_ID = STEP_MOCKUP_CODE,
                                    CURRENT_VENDOR_ID = vendorId
                                }, context);
                                otherProcessCus = otherProcessCus.Where(m => string.IsNullOrEmpty(m.IS_END)).ToList();
                                foreach (var itemProcessCus in otherProcessCus)
                                {
                                    if (itemProcessCus.MOCKUP_SUB_ID != param.MOCKUP_SUB_ID)
                                        MockUpProcessHelper.EndTaskForm(itemProcessCus.MOCKUP_SUB_ID, -1, context);
                                }
                            }

                            dbContextTransaction.Commit();

                            if (param.ACTION_CODE == "SEND_BACK")
                                EmailService.sendEmailMockup(param.MOCKUP_ID, param.MOCKUP_SUB_ID, "WF_SEND_BACK", context, param.COMMENT);
                            else if (param.ACTION_CODE == "SAVE")
                                EmailService.sendEmailMockup(param.MOCKUP_ID, param.MOCKUP_SUB_ID, "WF_OTHER_SAVE", context);
                            else
                                EmailService.sendEmailMockup(param.MOCKUP_ID, param.MOCKUP_SUB_ID, "WF_OTHER_SUBMIT", context);

                            Results.status = "S";
                            Results.msg = MessageHelper.GetMessage("MSG_001", context);


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

    }
}
