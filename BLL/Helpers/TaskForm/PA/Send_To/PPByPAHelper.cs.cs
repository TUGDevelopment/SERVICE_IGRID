using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class PPByPAHelper
    {
        public static ART_WF_ARTWORK_PROCESS_PP_RESULT AcceptTask(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PP_RESULT Results = new ART_WF_ARTWORK_PROCESS_PP_RESULT();
            List<ART_WF_ARTWORK_PROCESS_PP_2> list = new List<ART_WF_ARTWORK_PROCESS_PP_2>();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        context.Database.CommandTimeout = 300;
                        var list_artwork_sub_id = param.data.REMARK_OTHERS.Split('|');

                        foreach (var i in list_artwork_sub_id)
                        {                           
                            var query = context.Database.SqlQuery<int>("spCheckMaterialLock @ID",
                                        new SqlParameter("@ID", string.Format("{0}", Convert.ToInt32(i))))
                                        .FirstOrDefault();
                            if (query == 0)
                            {
                                var item = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Convert.ToInt32(i), context);
                                if (item != null)
                                {
                                    item.CURRENT_USER_ID = param.data.CURRENT_USER_ID;
                                    item.UPDATE_BY = param.data.UPDATE_BY;
                                    //item.ARTWORK_SUB_ID = Convert.ToInt32(i);
                                    ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(item, context);
                                }
                            } else
                            {
                                var item2 = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Convert.ToInt32(i), context);
                                if (item2 != null)
                                {
                                    ART_WF_ARTWORK_PROCESS_PP_2 r = new ART_WF_ARTWORK_PROCESS_PP_2();
                                    ART_WF_ARTWORK_REQUEST_ITEM requestItem = new ART_WF_ARTWORK_REQUEST_ITEM();
                                    requestItem.ARTWORK_ITEM_ID = item2.ARTWORK_ITEM_ID;
                                    requestItem.ARTWORK_REQUEST_ID = item2.ARTWORK_REQUEST_ID;
                                    var reqItem = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(requestItem, context).FirstOrDefault();
                                    r.REASON_BY_OTHER = reqItem.REQUEST_ITEM_NO;
                                    list.Add(r);
                                }
                            }
   
                        }
                        Results.data=list;
                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_001", context);
                        dbContextTransaction.Commit();

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
        public static ART_WF_ARTWORK_PROCESS_PP_RESULT GetPPByPA(ART_WF_ARTWORK_PROCESS_PP_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PP_RESULT Results = new ART_WF_ARTWORK_PROCESS_PP_RESULT();

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

                            var ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            param.data.ARTWORK_SUB_ID = 0;

                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_PP(ART_WF_ARTWORK_PROCESS_PP_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_PROCESS_PP(param.data), context));

                            param.data.ARTWORK_SUB_ID = ARTWORK_SUB_ID;
                            Results.data = Results.data.Where(m => CNService.FindArtworkSubId(ARTWORK_SUB_ID, context).Contains(m.ARTWORK_SUB_ID)).ToList();
                        }

                        ART_WF_ARTWORK_PROCESS_PP p = new ART_WF_ARTWORK_PROCESS_PP();

                        Results.status = "S";
                        var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PP" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var PAstepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                var reason_pa = CNService.getReason(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).REASON_ID, context);

                                ART_SYS_ACTION act = new ART_SYS_ACTION();
                                act.ACTION_CODE = Results.data[i].ACTION_CODE;
                                var temp = ART_SYS_ACTION_SERVICE.GetByItem(act, context).FirstOrDefault();
                                if (temp != null) Results.data[i].ACTION_NAME = temp.ACTION_NAME;

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

                                var formPA = ART_WF_ARTWORK_PROCESS_PP_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PP_BY_PA() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context);
                                if (formPA.FirstOrDefault() != null)
                                {
                                    if (formPA.FirstOrDefault().REQUEST_SHADE_LIMIT != null)
                                    {
                                        if (formPA.FirstOrDefault().REQUEST_SHADE_LIMIT.ToUpper().Trim() == "X")
                                            Results.data[i].REQUEST_SHADE_LIMIT_REFERENCE = "Yes";
                                        else if (formPA.FirstOrDefault().REQUEST_SHADE_LIMIT.ToUpper().Trim() != "X")
                                            Results.data[i].REQUEST_SHADE_LIMIT_REFERENCE = "No";
                                    }
                                }
                            }
                        }

                        var list = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context), CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var result_ = list.Where(q => !Results.data.Any(q2 => q2.ARTWORK_SUB_ID == q.ARTWORK_SUB_ID)).ToList();
                        if (result_ != null)
                        {
                            ART_WF_ARTWORK_PROCESS_PP_2 item = new ART_WF_ARTWORK_PROCESS_PP_2();
                            foreach (var result in result_)
                            {
                                item.CREATE_DATE_BY_PA = result.CREATE_DATE;
                                item.COMMENT_BY_PA = result.REMARK;
                                item.REMARK_REASON_BY_PA = "-";

                                if (result.REASON_ID != null)
                                {
                                    item.REASON_BY_PA = CNService.getReason(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(result.ARTWORK_SUB_ID, context).REASON_ID, context);
                                    if (item.REASON_BY_PA == "อื่นๆ โปรดระบุ (Others)")
                                    {
                                        item.REMARK_REASON_BY_PA = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = result.ARTWORK_SUB_ID, WF_STEP = PAstepId }, context);
                                    }
                                }

                                item.ARTWORK_SUB_ID = result.ARTWORK_SUB_ID;
                                item.ARTWORK_REQUEST_ID = result.ARTWORK_REQUEST_ID;
                                var formPA = ART_WF_ARTWORK_PROCESS_PP_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PP_BY_PA() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context);
                                if (formPA.FirstOrDefault() != null)
                                {
                                    if (formPA.FirstOrDefault().REQUEST_SHADE_LIMIT != null)
                                    {
                                        if (formPA.FirstOrDefault().REQUEST_SHADE_LIMIT == "X")
                                            item.REQUEST_SHADE_LIMIT_REFERENCE_DISPLAY_TXT = "Yes";
                                        else if (formPA.FirstOrDefault().REQUEST_SHADE_LIMIT != "X")
                                            item.REQUEST_SHADE_LIMIT_REFERENCE_DISPLAY_TXT = "No";
                                    }
                                }

                                Results.data.Add(item);
                            }
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

        public static ART_WF_ARTWORK_PROCESS_PP_BY_PA_RESULT SavePPByPA(ART_WF_ARTWORK_PROCESS_PP_BY_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PP_BY_PA_REQUEST_LIST param2 = new ART_WF_ARTWORK_PROCESS_PP_BY_PA_REQUEST_LIST();
            param2.data = new List<ART_WF_ARTWORK_PROCESS_PP_BY_PA_2>();
            param2.data.Add(param.data);
            return SaveMultiPPByPA(param2);
        }

        public static ART_WF_ARTWORK_PROCESS_PP_BY_PA_RESULT SaveMultiPPByPA(ART_WF_ARTWORK_PROCESS_PP_BY_PA_REQUEST_LIST param)
        {
            ART_WF_ARTWORK_PROCESS_PP_BY_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PP_BY_PA_RESULT();
            List<ART_WF_ARTWORK_PROCESS_PP_BY_PA_2> listItem = new List<ART_WF_ARTWORK_PROCESS_PP_BY_PA_2>();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var send_vn_po = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK { STEP_ARTWORK_CODE = "SEND_VN_PO" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        ART_WF_ARTWORK_PROCESS_RESULT processResults = new ART_WF_ARTWORK_PROCESS_RESULT();

                        if (param != null && param.data != null && param.data.Count > 0)
                        {
                            foreach (ART_WF_ARTWORK_PROCESS_PP_BY_PA_2 pa in param.data)
                            {
                                var isLock = CNService.IsLock(pa.ARTWORK_SUB_ID, context);
                                if (isLock)
                                {
                                    Results.status = "E";
                                    Results.msg = "Some workitem is locked.<br/>Please refresh your web browser.";
                                    return Results;
                                }

                                var validateSOChange = SalesOrderHelper.CheckIsSalesOrderChange(pa.ARTWORK_SUB_ID, context);
                                if (!String.IsNullOrEmpty(validateSOChange))
                                {
                                    Results.msg = "<b>Can not send to PP</b> <br>" + MessageHelper.GetMessage("MSG_013", context);
                                    Results.status = "E";
                                    return Results;
                                }

                                string msg = ArtworkProcessHelper.checkDupWF(pa.PROCESS, context);
                                if (msg != "")
                                {
                                    Results.status = "E";
                                    Results.msg = msg;
                                    return Results;
                                }

                                var chkProcessVendor = (from m in context.ART_WF_ARTWORK_PROCESS
                                                        where m.CURRENT_STEP_ID == send_vn_po
                                                        && string.IsNullOrEmpty(m.IS_END)
                                                        && m.ARTWORK_ITEM_ID == pa.PROCESS.ARTWORK_ITEM_ID
                                                        select m).ToList();
                                if (chkProcessVendor.Count > 0)
                                {
                                    Results.msg = MessageHelper.GetMessage("MSG_008", context) + " for " + ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(pa.PROCESS.CURRENT_STEP_ID, context).STEP_ARTWORK_NAME + ".";
                                    Results.msg += "<br/>" + "Because PP already send to vendor.";
                                    Results.status = "E";
                                    return Results;
                                }
                            }

                            foreach (ART_WF_ARTWORK_PROCESS_PP_BY_PA_2 pa in param.data)
                            {
                                var tempPA = new ART_WF_ARTWORK_PROCESS_PA();
                                var item = new ART_WF_ARTWORK_PROCESS_PP_BY_PA_2();
                                if (pa.PROCESS != null)
                                {
                                    processResults = ArtworkProcessHelper.SaveProcess(pa.PROCESS, context);
                                }

                                ART_WF_ARTWORK_PROCESS_PP_BY_PA PPData = new ART_WF_ARTWORK_PROCESS_PP_BY_PA();
                                PPData = MapperServices.ART_WF_ARTWORK_PROCESS_PP_BY_PA(pa);

                                if (processResults != null && processResults.data != null && processResults.data.Count > 0)
                                {
                                    PPData.ARTWORK_SUB_ID = processResults.data[0].ARTWORK_SUB_ID;
                                }

                                tempPA.ARTWORK_SUB_ID = pa.ARTWORK_SUB_ID;
                                tempPA = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(tempPA, context).FirstOrDefault();

                                if (tempPA != null)
                                {
                                    if (tempPA.SHADE_LIMIT == "0")
                                        PPData.REQUEST_SHADE_LIMIT = "X";
                                    else if (tempPA.SHADE_LIMIT == "1")
                                        PPData.REQUEST_SHADE_LIMIT = "";
                                }

                                ART_WF_ARTWORK_PROCESS_PP_BY_PA_SERVICE.SaveOrUpdate(PPData, context);

                                //reset READY_CREATE_PO
                                var parentArtworkSubId = CNService.FindParentArtworkSubId(pa.ARTWORK_SUB_ID, context);
                                var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                 where p.ARTWORK_SUB_ID == parentArtworkSubId
                                                 select p).FirstOrDefault();

                                if (processPA != null)
                                {
                                    processPA.READY_CREATE_PO = null;
                                    ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);
                                }
                                //reset READY_CREATE_PO

                                Results.data = new List<ART_WF_ARTWORK_PROCESS_PP_BY_PA_2>();

                                item.ARTWORK_PROCESS_PP_ID = PPData.ARTWORK_PROCESS_PP_ID;
                                listItem.Add(item);
                            }
                        }

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

        public static ART_WF_ARTWORK_PROCESS_PP_RESULT SavePPSendToPA(ART_WF_ARTWORK_PROCESS_PP_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PP_RESULT Results = new ART_WF_ARTWORK_PROCESS_PP_RESULT();

            if (param == null || param.data == null)
            {
                return Results;
            }
            else
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (var dbContextTransaction = CNService.IsolationLevel(context))
                        {
                            var qcData = MapperServices.ART_WF_ARTWORK_PROCESS_PP(param.data);

                            var check = ART_WF_ARTWORK_PROCESS_PP_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PP() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                            if (check.Count > 0)
                                qcData.ARTWORK_PROCESS_PP_ID = check.FirstOrDefault().ARTWORK_PROCESS_PP_ID;

                            ART_WF_ARTWORK_PROCESS_PP_SERVICE.SaveOrUpdate(qcData, context);

                            if (param.data.ACTION_CODE == "SENDTO_VENDOR")
                            {
                                var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);

                                var param2 = new ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_REQUEST_LIST();
                                param2.data = new List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2>();

                                var item = new ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2();
                                item.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;
                                item.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                item.VENDOR_ID = CNService.GetVendorByArtworkSubId(param.data.ARTWORK_SUB_ID, context);
                                item.ENDTASKFORM = true;
                                //item.CONFIRM_PO = "X";
                                //item.ACTION_CODE = "SUBMIT";
                                item.CREATE_BY = param.data.UPDATE_BY;
                                item.UPDATE_BY = param.data.UPDATE_BY;

                                item.PROCESS = new ART_WF_ARTWORK_PROCESS_2();
                                item.PROCESS.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;
                                item.PROCESS.ARTWORK_ITEM_ID = process.ARTWORK_ITEM_ID;
                                item.PROCESS.PARENT_ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                item.PROCESS.CURRENT_ROLE_ID = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_VN_PO" }, context).FirstOrDefault().ROLE_ID_RESPONSE;
                                item.PROCESS.CURRENT_STEP_ID = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_VN_PO" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                                item.PROCESS.CREATE_BY = param.data.UPDATE_BY;
                                item.PROCESS.UPDATE_BY = param.data.UPDATE_BY;
                                param2.data.Add(item);

                                var res = PPFormHelper.aSaveMultiVendorByPP(param2, context, dbContextTransaction);
                                if (res.status == "E")
                                {
                                    Results.status = "E";
                                    Results.msg = res.msg;
                                    return Results;
                                }

                                //int subID = param.data.ARTWORK_SUB_ID;
                                //var requestItemID = (from r in context.ART_WF_ARTWORK_PROCESS
                                //                     where r.ARTWORK_SUB_ID == subID
                                //                     select r.ARTWORK_ITEM_ID).FirstOrDefault();

                                //var requestNo = context.ART_WF_ARTWORK_REQUEST_ITEM
                                //                    .Where(w => w.ARTWORK_ITEM_ID == requestItemID)
                                //                    .Select(s => s.REQUEST_ITEM_NO)
                                //                    .FirstOrDefault();
                                //if (requestNo != null)
                                //{
                                //    ART_WF_ARTWORK_PROCESS_PP_RESULT ResultsVN = new ART_WF_ARTWORK_PROCESS_PP_RESULT();

                                //    var mappingAWPO = (from p in context.ART_WF_ARTWORK_MAPPING_PO
                                //                       where p.ARTWORK_NO == requestNo
                                //                       select p).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                                //    if (mappingAWPO == null)
                                //    {
                                //        ResultsVN.status = "E";
                                //        ResultsVN.msg = MessageHelper.GetMessage("MSG_012");
                                //        return ResultsVN;
                                //    }

                                //  ResultsVN = CreateProcessVendor(param, context);

                                //    if (ResultsVN.status == "E")
                                //    {
                                //        return ResultsVN;
                                //    }
                                //}
                            }

                            else
                            {
                                #region "if ACTION_CODE == SEND_BACK set READY_CREATE_PO == null"
                                if (param.data.ACTION_CODE == "SEND_BACK")
                                {
                                    var stepPA = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_PA").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();

                                    var awItwmID = (from p in context.ART_WF_ARTWORK_PROCESS
                                                    where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                                    select p.ARTWORK_ITEM_ID).FirstOrDefault();

                                    if (awItwmID > 0)
                                    {
                                        var processStepPA = (from p in context.ART_WF_ARTWORK_PROCESS
                                                             where p.ARTWORK_ITEM_ID == awItwmID
                                                             && p.CURRENT_STEP_ID == stepPA
                                                             select p.ARTWORK_SUB_ID).FirstOrDefault();

                                        if (processStepPA > 0)
                                        {
                                            var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                             where p.ARTWORK_SUB_ID == processStepPA
                                                             select p).FirstOrDefault();

                                            if (processPA != null)
                                            {
                                                processPA.READY_CREATE_PO = null;
                                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);
                                            }
                                        }

                                    }
                                }
                                #endregion

                                if (param.data.ENDTASKFORM)
                                    ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);

                                dbContextTransaction.Commit();
                            }

                            //if (param.data.ENDTASKFORM)
                            //    ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);

                            //dbContextTransaction.Commit();

                            if (param.data.ACTION_CODE == "SEND_BACK")
                                EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_SEND_BACK", context, param.data.COMMENT);
                            else if (param.data.ACTION_CODE == "SAVE")
                                EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_OTHER_SAVE", context);
                            //else
                            //    EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_OTHER_SUBMIT", context);

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
            }

            return Results;
        }
    }
}
