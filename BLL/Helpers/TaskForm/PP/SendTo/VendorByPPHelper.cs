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
    public static class VendorByPPHelper
    {
        public static ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT GetVendorByPP(ART_WF_ARTWORK_PROCESS_VENDOR_PO_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT Results = new ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_VENDOR_PO(ART_WF_ARTWORK_PROCESS_VENDOR_PO_SERVICE.GetAll(context));
                        }
                        else
                        {
                            var ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            param.data.ARTWORK_SUB_ID = 0;

                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_VENDOR_PO(ART_WF_ARTWORK_PROCESS_VENDOR_PO_SERVICE.GetByItemContain(MapperServices.ART_WF_ARTWORK_PROCESS_VENDOR_PO(param.data), context));

                            param.data.ARTWORK_SUB_ID = ARTWORK_SUB_ID;
                            Results.data = Results.data.Where(m => CNService.FindArtworkSubId(ARTWORK_SUB_ID, context).Contains(m.ARTWORK_SUB_ID)).ToList();
                        }

                        Results.status = "S";
                        var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = param.data.STEP_ARTWORK_CODE }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var PPstepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PP" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                      
                        if (Results.data.Count > 0)
                        {

                            // ticket#19383 by aof -- start 
                            var ARTWORK_ITEM_ID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context);
                            var processSENDPP = (from p in context.ART_WF_ARTWORK_PROCESS
                                                 where p.ARTWORK_ITEM_ID == ARTWORK_ITEM_ID && p.CURRENT_STEP_ID == PPstepId && string.IsNullOrEmpty(p.IS_TERMINATE) && string.IsNullOrEmpty(p.REMARK_KILLPROCESS)
                                                 select p).OrderByDescending(o => o.ARTWORK_SUB_ID).FirstOrDefault();

                            var REQUEST_SHADE_LIMIT_REFERENCE = "";

                            if (processSENDPP != null) {
                               var processPPbyPA= ART_WF_ARTWORK_PROCESS_PP_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PP_BY_PA() { ARTWORK_SUB_ID = processSENDPP.ARTWORK_SUB_ID }, context).FirstOrDefault();

                                if (processPPbyPA.REQUEST_SHADE_LIMIT != null)
                                {
                                    if (processPPbyPA.REQUEST_SHADE_LIMIT == "X")
                                        REQUEST_SHADE_LIMIT_REFERENCE = "Yes";
                                    else if (processPPbyPA.REQUEST_SHADE_LIMIT != "X")
                                        REQUEST_SHADE_LIMIT_REFERENCE = "No";
                                }
                            }
                            // ticket#19383 by aof -- last 



                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                var reason_pp = CNService.getReason(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).REASON_ID, context);

                                ART_SYS_ACTION act = new ART_SYS_ACTION();
                                act.ACTION_CODE = Results.data[i].ACTION_CODE;
                                Results.data[i].ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItem(act, context).FirstOrDefault().ACTION_NAME;

                                var processFormPA = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context);

                                Results.data[i].COMMENT_BY_PP = processFormPA.REMARK;
                                Results.data[i].REASON_BY_PP = reason_pp;
                                Results.data[i].REMARK_REASON_BY_PP = "-";
                                Results.data[i].REMARK_REASON_BY_OTHER = "-";
                                Results.data[i].REASON_BY_OTHER = CNService.getReason(Results.data[i].REASON_ID, context);
                                if (reason_pp == "อื่นๆ โปรดระบุ (Others)")
                                {
                                    Results.data[i].REMARK_REASON_BY_PP = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = Results.data[i].ARTWORK_SUB_ID, WF_STEP = PPstepId }, context);
                                }
                                if (Results.data[i].REASON_BY_OTHER == "อื่นๆ โปรดระบุ (Others)")
                                {
                                    Results.data[i].REMARK_REASON_BY_OTHER = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = Results.data[i].ARTWORK_SUB_ID, WF_STEP = stepId }, context);
                                }

                                Results.data[i].CREATE_DATE_BY_PP = processFormPA.CREATE_DATE;
                                Results.data[i].SEND_TO_VENDOR_TYPE = param.data.SEND_TO_VENDOR_TYPE;

                                Results.data[i].VENDOR_DISPLAY_TXT = CNService.GetUserName(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).CURRENT_USER_ID, context);
                                Results.data[i].VENDOR_DISPLAY_TXT += "<br/>" + XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).CURRENT_VENDOR_ID, context).VENDOR_NAME;

                                // by aof       
                                Results.data[i].REQUEST_SHADE_LIMIT_REFERENCE = REQUEST_SHADE_LIMIT_REFERENCE;  //ticket#19383 by aof
                                // by aof
                            }
                        }

                        var list = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context), CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var results = list.Where(q => !Results.data.Any(q2 => q2.ARTWORK_SUB_ID == q.ARTWORK_SUB_ID));
                        foreach (var result in results)
                        {
                            ART_WF_ARTWORK_PROCESS_VENDOR_PO_2 item = new ART_WF_ARTWORK_PROCESS_VENDOR_PO_2();
                            item.CREATE_DATE_BY_PP = result.CREATE_DATE;
                            item.COMMENT_BY_PP = result.REMARK;

                            item.REMARK_REASON_BY_PP = "-";

                            if (result.REASON_ID != null)
                            {
                                item.REASON_BY_PP = CNService.getReason(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(result.ARTWORK_SUB_ID, context).REASON_ID, context);
                                if (item.REASON_BY_PP == "อื่นๆ โปรดระบุ (Others)")
                                {
                                    item.REMARK_REASON_BY_PP = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = result.ARTWORK_SUB_ID, WF_STEP = PPstepId }, context);
                                }
                            }


                            item.ARTWORK_SUB_ID = result.ARTWORK_SUB_ID;
                            item.ARTWORK_REQUEST_ID = result.ARTWORK_REQUEST_ID;

                            item.VENDOR_DISPLAY_TXT = CNService.GetUserName(result.CURRENT_USER_ID, context);
                            item.VENDOR_DISPLAY_TXT += "<br/>" + XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(result.CURRENT_VENDOR_ID, context).VENDOR_NAME;

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

        public static ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT PostVendorSendToPA(ART_WF_ARTWORK_PROCESS_VENDOR_PO_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT Results = new ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var vnData = MapperServices.ART_WF_ARTWORK_PROCESS_VENDOR_PO(param.data);

                        var check = ART_WF_ARTWORK_PROCESS_VENDOR_PO_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_VENDOR_PO() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                        if (check.Count > 0)
                        {
                            vnData.ARTWORK_PROCESS_VENDOR_PO_ID = check.FirstOrDefault().ARTWORK_PROCESS_VENDOR_PO_ID;
                        }

                        ART_WF_ARTWORK_PROCESS_VENDOR_PO_SERVICE.SaveOrUpdate(vnData, context);

                        if (param.data.ENDTASKFORM)
                        {
                            ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                            EndTaskParent(param.data, context);

                            ART_WF_ARTWORK_PROCESS_VENDOR_REQUEST vnREQ = new ART_WF_ARTWORK_PROCESS_VENDOR_REQUEST();
                            ART_WF_ARTWORK_PROCESS_VENDOR_2 vn2 = new ART_WF_ARTWORK_PROCESS_VENDOR_2();
                            vn2.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            vnREQ.data = vn2;
                            VendorByPAHelper.EndTaskVendorOtherUser(vnREQ, context);
                        }

                        dbContextTransaction.Commit();

                        if (param.data.ACTION_CODE == "SEND_BACK")
                            EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_SEND_BACK", context, param.data.COMMENT);
                        else if (param.data.ACTION_CODE == "SAVE") { }
                        //EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_OTHER_SAVE", context);
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

        public static ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT EndTaskParent(ART_WF_ARTWORK_PROCESS_VENDOR_PO_2 param, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT Results = new ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT();

            int parentID = 0;

            try
            {
                parentID = CNService.FindParentArtworkSubId(param.ARTWORK_SUB_ID, context);

                ART_WF_ARTWORK_PROCESS process = new ART_WF_ARTWORK_PROCESS();
                process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(parentID, context);

                process.IS_END = "X";
                ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(process, context);
                //#437016
                if (process.CURRENT_STEP_ID == 2 && process.IS_END == "X") {  
                    ART_WF_ARTWORK_PROCESS_REQUEST p = new ART_WF_ARTWORK_PROCESS_REQUEST();
                    ART_WF_ARTWORK_PROCESS_2 data = new ART_WF_ARTWORK_PROCESS_2();
                    data.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                    p.data = data;
                    CNService.CompletePOForm(p, context);
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT PostMultiVendorSendToPA(ART_WF_ARTWORK_PROCESS_VENDOR_PO_REQUEST_LIST param)
        {
            var completeWF = false;
            var listComplete = new List<ART_WF_ARTWORK_PROCESS>();
            ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT Results = new ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT();
            List<ART_WF_ARTWORK_PROCESS_VENDOR_PO_2> listPO = new List<ART_WF_ARTWORK_PROCESS_VENDOR_PO_2>();
            ART_WF_ARTWORK_PROCESS_VENDOR_PO_2 data = new ART_WF_ARTWORK_PROCESS_VENDOR_PO_2();

            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }
                else
                {
                    
                    using (var context = new ARTWORKEntities())
                    {
                        using (var dbContextTransaction = CNService.IsolationLevel(context))
                        {
                            var currentUserId = CNService.getCurrentUser(context);
                            foreach (ART_WF_ARTWORK_PROCESS_VENDOR_PO_2 iVnPO in param.data)
                            {
                                var awMappings = (from a in context.ART_WF_ARTWORK_MAPPING_PO
                                                  where a.PO_NO == iVnPO.PURCHASE_ORDER_NO
                                                   && a.IS_ACTIVE == "X"
                                                  select a.ARTWORK_NO).ToList();

                                if (awMappings != null && awMappings.Count > 0)
                                {
                                    var requestItemIDs = (from t in context.ART_WF_ARTWORK_REQUEST_ITEM
                                                          where awMappings.Contains(t.REQUEST_ITEM_NO)
                                                          select t.ARTWORK_ITEM_ID).ToList();

                                    if (requestItemIDs != null && requestItemIDs.Count > 0)
                                    {
                                        foreach (int itemID in requestItemIDs)
                                        {
                                            var subIDs = (from s in context.ART_WF_ARTWORK_PROCESS
                                                          where s.ARTWORK_ITEM_ID == itemID
                                                          select s.ARTWORK_SUB_ID).ToList();

                                            var stepCustID = context.ART_M_STEP_ARTWORK
                                                                .Where(w => w.STEP_ARTWORK_CODE == "SEND_CUS_PRINT")
                                                                .Select(s => s.STEP_ARTWORK_ID)
                                                                .FirstOrDefault();

                                            var processCust = (from c in context.ART_WF_ARTWORK_PROCESS
                                                               where subIDs.Contains(c.ARTWORK_SUB_ID)
                                                                && c.CURRENT_STEP_ID == stepCustID
                                                               select c.ARTWORK_SUB_ID).ToList();

                                            var CustShadeLimit = (from l in context.ART_WF_ARTWORK_PROCESS_CUSTOMER
                                                                  where processCust.Contains(l.ARTWORK_SUB_ID)
                                                                   && l.ACTION_CODE == "SUBMIT"
                                                                  select l).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();


                                            var step = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == "SEND_VN_PO").FirstOrDefault();

                                            var listProcessPO = (from p in context.ART_WF_ARTWORK_PROCESS
                                                                 where p.CURRENT_STEP_ID == step.STEP_ARTWORK_ID
                                                                   && subIDs.Contains(p.ARTWORK_SUB_ID)
                                                                   && String.IsNullOrEmpty(p.IS_END)
                                                                 select p).ToList();

                                            if (listProcessPO != null && listProcessPO.Count > 0)
                                            {
                                                foreach (ART_WF_ARTWORK_PROCESS item in listProcessPO)
                                                {
                                                    ART_WF_ARTWORK_PROCESS_VENDOR_PO vnData = new ART_WF_ARTWORK_PROCESS_VENDOR_PO();
                                                    ART_WF_ARTWORK_PROCESS_VENDOR_PO_2 vnData2 = new ART_WF_ARTWORK_PROCESS_VENDOR_PO_2();
                                                    ART_WF_ARTWORK_PROCESS_2 process2 = new ART_WF_ARTWORK_PROCESS_2();

                                                    var request = (from q in context.ART_WF_ARTWORK_REQUEST
                                                                   where q.ARTWORK_REQUEST_ID == item.ARTWORK_REQUEST_ID
                                                                   select q).FirstOrDefault();

                                                    if (request != null)
                                                    {
                                                        if (request.TYPE_OF_ARTWORK == "NEW")
                                                        {
                                                            if (CustShadeLimit == null)
                                                            {
                                                                var itemAW = (from i in context.ART_WF_ARTWORK_REQUEST_ITEM
                                                                              where i.ARTWORK_ITEM_ID == item.ARTWORK_ITEM_ID
                                                                              select i.REQUEST_ITEM_NO).FirstOrDefault();

                                                                Results.status = "E";
                                                                Results.msg = itemAW + " does not customer approve shade limit.";
                                                                return Results;
                                                            }

                                                            if (CustShadeLimit.DECISION_ACTION == "0")
                                                            {
                                                                process2 = MapperServices.ART_WF_ARTWORK_PROCESS(item);
                                                                process2.ENDTASKFORM = true;
                                                                vnData2.ARTWORK_SUB_ID = item.ARTWORK_SUB_ID;
                                                                vnData2.ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID;
                                                                vnData2.ENDTASKFORM = true;
                                                                vnData2.PROCESS = process2;
                                                                var check = ART_WF_ARTWORK_PROCESS_VENDOR_PO_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_VENDOR_PO() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context);
                                                                if (check.Count > 0)
                                                                {
                                                                    vnData.ARTWORK_PROCESS_VENDOR_PO_ID = check.FirstOrDefault().ARTWORK_PROCESS_VENDOR_PO_ID;
                                                                    vnData.CREATE_BY = check.FirstOrDefault().CREATE_BY;
                                                                }
                                                                else
                                                                {
                                                                    vnData.CREATE_BY = currentUserId;
                                                                }

                                                                vnData = MapperServices.ART_WF_ARTWORK_PROCESS_VENDOR_PO(vnData2);
                                                                vnData.UPDATE_BY = currentUserId;
                                                                vnData.CONFIRM_PO = "X";
                                                                vnData.ACTION_CODE = "SUBMIT";
                                                                ART_WF_ARTWORK_PROCESS_VENDOR_PO_SERVICE.SaveOrUpdate(vnData, context);

                                                                if (vnData2.ENDTASKFORM)
                                                                {
                                                                    ArtworkProcessHelper.EndTaskForm(item.ARTWORK_SUB_ID, currentUserId, context);

                                                                    if (CustShadeLimit.APPROVE_SHADE_LIMIT == "1")
                                                                    {
                                                                        EndTaskParent(vnData2, context);

                                                                        var tempProcess = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(item.ARTWORK_SUB_ID, context);
                                                                        var error_msg = ""; // by aof 20220317
                                                                        ArtworkProcessHelper.moveFileArtworkToMatWorkspace(tempProcess.ARTWORK_ITEM_ID, context,ref error_msg);

                                                                        listComplete.Add(item);
                                                                        completeWF = true;
                                                                    }

                                                                    ART_WF_ARTWORK_PROCESS_VENDOR_REQUEST vnREQ = new ART_WF_ARTWORK_PROCESS_VENDOR_REQUEST();
                                                                    ART_WF_ARTWORK_PROCESS_VENDOR_2 vn2 = new ART_WF_ARTWORK_PROCESS_VENDOR_2();
                                                                    vn2.ARTWORK_SUB_ID = item.ARTWORK_SUB_ID;
                                                                    vnREQ.data = vn2;
                                                                    VendorByPAHelper.EndTaskVendorOtherUser(vnREQ, context);
                                                                }
                                                            }
                                                            else if (CustShadeLimit.DECISION_ACTION == "1" || CustShadeLimit.DECISION_ACTION == "2")
                                                            {
                                                                process2 = MapperServices.ART_WF_ARTWORK_PROCESS(item);
                                                                process2.ENDTASKFORM = true;
                                                                vnData2.ARTWORK_SUB_ID = item.ARTWORK_SUB_ID;
                                                                vnData2.ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID;
                                                                vnData2.ENDTASKFORM = true;
                                                                vnData2.PROCESS = process2;

                                                                var check = ART_WF_ARTWORK_PROCESS_VENDOR_PO_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_VENDOR_PO() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context);
                                                                if (check.Count > 0)
                                                                {
                                                                    vnData.ARTWORK_PROCESS_VENDOR_PO_ID = check.FirstOrDefault().ARTWORK_PROCESS_VENDOR_PO_ID;
                                                                    vnData.CREATE_BY = check.FirstOrDefault().CREATE_BY;
                                                                }
                                                                else
                                                                {
                                                                    vnData.CREATE_BY = currentUserId;
                                                                }

                                                                vnData = MapperServices.ART_WF_ARTWORK_PROCESS_VENDOR_PO(vnData2);
                                                                vnData.UPDATE_BY = currentUserId;
                                                                vnData.CONFIRM_PO = "X";
                                                                vnData.ACTION_CODE = "SUBMIT";
                                                                ART_WF_ARTWORK_PROCESS_VENDOR_PO_SERVICE.SaveOrUpdate(vnData, context);

                                                                if (vnData2.ENDTASKFORM)
                                                                {
                                                                    ART_WF_ARTWORK_PROCESS_VENDOR_REQUEST vnREQ = new ART_WF_ARTWORK_PROCESS_VENDOR_REQUEST();
                                                                    ART_WF_ARTWORK_PROCESS_VENDOR_2 vn2 = new ART_WF_ARTWORK_PROCESS_VENDOR_2();
                                                                    vn2.ARTWORK_SUB_ID = item.ARTWORK_SUB_ID;
                                                                    vnREQ.data = vn2;
                                                                    VendorByPAHelper.EndTaskVendorOtherUser(vnREQ, context);
                                                                }
                                                            }
                                                        }
                                                        else if (request.TYPE_OF_ARTWORK == "REPEAT")
                                                        {
                                                            process2 = MapperServices.ART_WF_ARTWORK_PROCESS(item);
                                                            process2.ENDTASKFORM = true;
                                                            vnData2.ARTWORK_SUB_ID = item.ARTWORK_SUB_ID;
                                                            vnData2.ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID;
                                                            vnData2.ENDTASKFORM = true;
                                                            vnData2.PROCESS = process2;

                                                            var check = ART_WF_ARTWORK_PROCESS_VENDOR_PO_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_VENDOR_PO() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context);
                                                            if (check.Count > 0)
                                                            {
                                                                vnData.ARTWORK_PROCESS_VENDOR_PO_ID = check.FirstOrDefault().ARTWORK_PROCESS_VENDOR_PO_ID;
                                                                vnData.CREATE_BY = check.FirstOrDefault().CREATE_BY;
                                                            }
                                                            else
                                                            {
                                                                vnData.CREATE_BY = currentUserId;
                                                            }

                                                            vnData = MapperServices.ART_WF_ARTWORK_PROCESS_VENDOR_PO(vnData2);
                                                            vnData.UPDATE_BY = currentUserId;
                                                            vnData.CONFIRM_PO = "X";
                                                            vnData.ACTION_CODE = "SUBMIT";
                                                            ART_WF_ARTWORK_PROCESS_VENDOR_PO_SERVICE.SaveOrUpdate(vnData, context);

                                                            if (vnData2.ENDTASKFORM)
                                                            {
                                                                ArtworkProcessHelper.EndTaskForm(item.ARTWORK_SUB_ID, currentUserId, context);
                                                                EndTaskParent(vnData2, context);

                                                                ART_WF_ARTWORK_PROCESS_VENDOR_REQUEST vnREQ = new ART_WF_ARTWORK_PROCESS_VENDOR_REQUEST();
                                                                ART_WF_ARTWORK_PROCESS_VENDOR_2 vn2 = new ART_WF_ARTWORK_PROCESS_VENDOR_2();
                                                                vn2.ARTWORK_SUB_ID = item.ARTWORK_SUB_ID;
                                                                vnREQ.data = vn2;
                                                                VendorByPAHelper.EndTaskVendorOtherUser(vnREQ, context);

                                                                listComplete.Add(item);
                                                                completeWF = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            dbContextTransaction.Commit();

                            if (completeWF)
                            {
                                var first = true;
                                foreach (var process in listComplete)
                                {
                                    if (first)
                                    {
                                        first = false;
                                        EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_COMPLETED", context);
                                    }
                                }
                            }
                        }
                        
                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_001", context);
                    }
                    

                    Results.status = "S";

                    data.AW_ENCRYPT = param.data[0].AW_ENCRYPT;
                    listPO.Add(data);

                    Results.data = listPO;
                    //Results.msg = MessageHelper.GetMessage("MSG_001", context);
                    return Results;
                }

            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT PostPORelateArtwork(ART_WF_ARTWORK_PROCESS_VENDOR_PO_REQUEST_LIST param)
        {
            ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT Results = new ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT();
            ART_WF_ARTWORK_PROCESS_VENDOR_PO_2 data = new ART_WF_ARTWORK_PROCESS_VENDOR_PO_2();
            List<ART_WF_ARTWORK_PROCESS_VENDOR_PO_2> listData = new List<ART_WF_ARTWORK_PROCESS_VENDOR_PO_2>();
            List<string> listAW = new List<string>();
           
            List<string> listPO = new List<string>();
            List<string> poSelected = new List<string>(); 
            string AWEncrypt = "";

            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    using (var context = new ARTWORKEntities())
                    {

                        using (CNService.IsolationLevel(context))
                        {
                            sb = new StringBuilder();

                            sb.AppendLine("<table>");
                            sb.AppendLine("<tr>");
                            sb.AppendLine("<th style=width:150px>Artwork No.</th>");
                            sb.AppendLine("<th style=width:150px>PO No.</th>");
                            sb.AppendLine("</tr>");

                            foreach (var item in param.data)
                            {
                                poSelected.Add(item.PURCHASE_ORDER_NO);
                            }
                            var step = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == "SEND_VN_PO").FirstOrDefault();
                            var awList = (from p in context.ART_WF_ARTWORK_MAPPING_PO
                                          where poSelected.Contains(p.PO_NO)
                                          && p.IS_ACTIVE == "X"
                                          orderby p.ARTWORK_NO
                                          select p).Distinct().ToList();

                            //-------------- by aof 461630
                            var arrAW = awList.Select(s => s.ARTWORK_NO).Distinct().ToList();

                            awList = (from p in context.ART_WF_ARTWORK_MAPPING_PO
                                      where arrAW.Contains(p.ARTWORK_NO)
                                      && p.IS_ACTIVE == "X"
                                      orderby p.ARTWORK_NO
                                      select p).Distinct().ToList();

                            var poSelectedTemp = awList.Select(s => s.PO_NO).Distinct().ToList();
                            awList = (from p in context.ART_WF_ARTWORK_MAPPING_PO
                                          where poSelectedTemp.Contains(p.PO_NO)
                                          && p.IS_ACTIVE == "X"
                                          orderby p.ARTWORK_NO
                                          select p).Distinct().ToList();
                            //-------------- by aof 461630




                            List<ART_WF_ARTWORK_MAPPING_PO> duplicatePO = new List<ART_WF_ARTWORK_MAPPING_PO>(); 
                            List<string> po = new List<string>();
                            foreach (var aw in awList)
                            {
                                //#Ticket No 460392
                                var requestItemIDs = (from t in context.ART_WF_ARTWORK_REQUEST_ITEM
                                                      where aw.ARTWORK_NO.Equals(t.REQUEST_ITEM_NO)
                                                      select t.ARTWORK_ITEM_ID).FirstOrDefault();

                                var listProcessPO = (from p in context.ART_WF_ARTWORK_PROCESS
                                                     where p.CURRENT_STEP_ID == step.STEP_ARTWORK_ID
                                                       && p.ARTWORK_ITEM_ID == requestItemIDs
                                                       && String.IsNullOrEmpty(p.IS_END)
                                                     select p).ToList();

                                if (listProcessPO != null && listProcessPO.Count > 0)
                                    if (aw != null)
                                    {
                                        po = new List<string>();

                                        var po_0 = (from p in context.ART_WF_ARTWORK_MAPPING_PO
                                                    where p.ARTWORK_NO == aw.ARTWORK_NO
                                                    && poSelected.Contains(p.PO_NO)
                                                     && p.IS_ACTIVE == "X"
                                                    orderby p.PO_NO
                                                    select p.PO_NO).Distinct().ToList();

                                        var po_1 = (from p in context.ART_WF_ARTWORK_MAPPING_PO
                                                    where p.ARTWORK_NO == aw.ARTWORK_NO
                                                     && !poSelected.Contains(p.PO_NO)
                                                     && p.IS_ACTIVE == "X"
                                                    orderby p.PO_NO
                                                    select p.PO_NO).Distinct().ToList();

                                        if (po_0 != null)
                                        {
                                            po.AddRange(po_0);
                                        }

                                        if (po_1 != null)
                                        {
                                            po.AddRange(po_1);
                                        }

                                        foreach (var poNo in po)
                                        {
                                            var index = duplicatePO.FindIndex(a => a.ARTWORK_NO == aw.ARTWORK_NO && a.PO_NO == poNo);
                                            if (index == -1)
                                            {
                                                sb.AppendLine("<tr>");
                                                sb.AppendLine("<td>" + aw.ARTWORK_NO + "</td>");
                                                // sb.AppendLine("<td></td>");

                                                if (poSelected.Contains(poNo))
                                                {
                                                    sb.AppendLine("<td>" + poNo + "</td>");
                                                }
                                                else
                                                {
                                                    sb.AppendLine("<td style=\"color:red;\">" + poNo + "</td>");
                                                }

                                                sb.AppendLine("</tr>");

                                                listAW.Add(aw.ARTWORK_NO);
                                                listPO.Add(poNo);

                                                if (!String.IsNullOrEmpty(AWEncrypt))
                                                {
                                                    if (!AWEncrypt.Contains(aw.ARTWORK_NO))
                                                    {
                                                        AWEncrypt = AWEncrypt + "||" + aw.ARTWORK_NO;   //by aof 461630
                                                    }
                                                }
                                                else
                                                {
                                                    AWEncrypt = aw.ARTWORK_NO;
                                                }

                                                ART_WF_ARTWORK_MAPPING_PO poDtl1 = new ART_WF_ARTWORK_MAPPING_PO { };
                                                poDtl1.ARTWORK_NO = aw.ARTWORK_NO;
                                                poDtl1.PO_NO = poNo;
                                                duplicatePO.Add(poDtl1);
                                            }
                                        }
                                    }
                            }

                            sb.AppendLine("<tr>");
                            sb.AppendLine("<td>&nbsp;</td>");
                            sb.AppendLine("<td>&nbsp;</td>");
                            sb.AppendLine("</tr>");
                            sb.AppendLine("</table>");
                        }

                        AWEncrypt= EncryptionService.EncryptAndUrlEncode(AWEncrypt);

                        data.AW_ENCRYPT = AWEncrypt;
                        data.CONFIRM_PO_DISPLAY_TXT = sb.ToString();
                        data.LIST_AW_CONFIRM_PO_DISPLAY_TXT = listAW.Distinct().ToList();
                        data.LIST_CONFIRM_PO_DISPLAY_TXT = listPO.Distinct().ToList();
                        listData.Add(data);
                        Results.data = listData;

                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_001", context);
                    }

                    return Results;
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