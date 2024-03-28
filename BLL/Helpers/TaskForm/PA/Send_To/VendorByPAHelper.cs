using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using BLL.Helpers;

namespace BLL.Helpers
{
    public class VendorByPAHelper
    {
        public static ART_WF_ARTWORK_PROCESS_VENDOR_RESULT GetVendorByPA(ART_WF_ARTWORK_PROCESS_VENDOR_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_VENDOR_RESULT Results = new ART_WF_ARTWORK_PROCESS_VENDOR_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_VENDOR(ART_WF_ARTWORK_PROCESS_VENDOR_SERVICE.GetAll(context));
                        }
                        else
                        {
                            var ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            param.data.ARTWORK_SUB_ID = 0;

                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_VENDOR(ART_WF_ARTWORK_PROCESS_VENDOR_SERVICE.GetByItemContain(MapperServices.ART_WF_ARTWORK_PROCESS_VENDOR(param.data), context));

                            param.data.ARTWORK_SUB_ID = ARTWORK_SUB_ID;
                            Results.data = Results.data.Where(m => CNService.FindArtworkSubId(ARTWORK_SUB_ID, context).Contains(m.ARTWORK_SUB_ID)).ToList();
                        }

                        ART_WF_ARTWORK_PROCESS_VENDOR p = new ART_WF_ARTWORK_PROCESS_VENDOR();

                        Results.status = "S";
                        var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = param.data.STEP_ARTWORK_CODE }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var PAstepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                //var vendorByPA = ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }).FirstOrDefault();
                                //if (vendorByPA != null)
                                //{
                                //var SEND_TO_VENDOR_TYPE = vendorByPA.SEND_TO_VENDOR_TYPE;


                                var vendorProcess = ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                if (vendorProcess != null)
                                {
                                    var SEND_TO_VENDOR_TYPE = vendorProcess.SEND_TO_VENDOR_TYPE;
                                    if (SEND_TO_VENDOR_TYPE == param.data.SEND_TO_VENDOR_TYPE)
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
                                        Results.data[i].SEND_TO_VENDOR_TYPE = param.data.SEND_TO_VENDOR_TYPE;

                                        Results.data[i].VENDOR_DISPLAY_TXT = CNService.GetUserName(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).CURRENT_USER_ID, context);
                                        Results.data[i].VENDOR_DISPLAY_TXT += "<br/>" + XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).CURRENT_VENDOR_ID, context).VENDOR_NAME;
                                    }
                                    else
                                    {
                                        Results.data[i].SEND_TO_VENDOR_TYPE = "delete";
                                    }
                                }
                            }

                            Results.data = Results.data.Where(m => m.SEND_TO_VENDOR_TYPE != "delete").ToList();
                        }

                        var list = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context), CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var results = list.Where(q => !Results.data.Any(q2 => q2.ARTWORK_SUB_ID == q.ARTWORK_SUB_ID));
                        foreach (var result in results)
                        {
                            ART_WF_ARTWORK_PROCESS_VENDOR_2 item = new ART_WF_ARTWORK_PROCESS_VENDOR_2();
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

                            var tempVendorByPA = ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault();
                            if (tempVendorByPA != null)
                                item.SEND_TO_VENDOR_TYPE = tempVendorByPA.SEND_TO_VENDOR_TYPE;

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

        public static ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_RESULT SaveVendorByPA(ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_REQUEST param)
        {
            int vendorID = 0;

            ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        int stepPGID = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == "SEND_PG").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();

                        var itemID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context);

                        var artworkProcess = (from p in context.ART_WF_ARTWORK_PROCESS
                                              where p.ARTWORK_ITEM_ID == itemID
                                                && p.CURRENT_STEP_ID == stepPGID
                                                && p.IS_END == "X"
                                                && String.IsNullOrEmpty(p.REMARK_KILLPROCESS)
                                              select p).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                        var stepPA = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_PA").FirstOrDefault();

                        var processPA = (from p in context.ART_WF_ARTWORK_PROCESS
                                         where p.ARTWORK_ITEM_ID == itemID
                                            && p.CURRENT_STEP_ID == stepPA.STEP_ARTWORK_ID
                                         select p).OrderByDescending(o => o.ARTWORK_SUB_ID).FirstOrDefault();

                        var processPAData = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                             where p.ARTWORK_SUB_ID == processPA.ARTWORK_SUB_ID
                                             select p).FirstOrDefault();

                        XECM_M_VENDOR vendorMigrate = new XECM_M_VENDOR();

                        bool isVenverByPG = false;
                        if (artworkProcess != null)
                        {
                            var tempProcessPG = context.ART_WF_ARTWORK_PROCESS_PG.Where(g => g.ARTWORK_SUB_ID == artworkProcess.ARTWORK_SUB_ID).FirstOrDefault();

                            if (tempProcessPG != null && tempProcessPG.DIE_LINE_MOCKUP_ID != null)
                            {
                                var mockup = context.ART_WF_MOCKUP_PROCESS.Where(m => m.MOCKUP_ID == tempProcessPG.DIE_LINE_MOCKUP_ID).FirstOrDefault();

                                var mockupPG = context.ART_WF_MOCKUP_PROCESS_PG.Where(m => m.MOCKUP_SUB_ID == mockup.MOCKUP_SUB_ID).OrderByDescending(o => o.MOCKUP_SUB_PG_ID).FirstOrDefault();

                                if (mockupPG != null && mockupPG.VENDOR != null)
                                {
                                    isVenverByPG = true;
                                    vendorID = Convert.ToInt32(mockupPG.VENDOR);
                                }
                            }
                        }

                        if (isVenverByPG == false && !String.IsNullOrEmpty(processPAData.MATERIAL_NO))
                        {
                            vendorMigrate = CNService.GetVendorMigrationByMaterial(processPAData.MATERIAL_NO, context);

                            if (vendorMigrate != null)
                            {
                                vendorID = Convert.ToInt32(vendorMigrate.VENDOR_ID);
                            }
                        }

                        if (vendorID <= 0 && artworkProcess != null)
                        {
                            if (artworkProcess == null)
                            {
                                Results.status = "E";
                                Results.msg = MessageHelper.GetMessage("MSG_010", context);
                                return Results;
                            }
                            var tempProcessPG = context.ART_WF_ARTWORK_PROCESS_PG.Where(g => g.ARTWORK_SUB_ID == artworkProcess.ARTWORK_SUB_ID);

                            var processPG = tempProcessPG.FirstOrDefault();

                            if (processPG == null)
                            {

                                Results.status = "E";
                                Results.msg = MessageHelper.GetMessage("MSG_010", context);
                                return Results;
                            }

                            var mockup = context.ART_WF_MOCKUP_PROCESS.Where(m => m.MOCKUP_ID == processPG.DIE_LINE_MOCKUP_ID).FirstOrDefault();

                            if (mockup == null)
                            {
                                Results.status = "E";
                                Results.msg = MessageHelper.GetMessage("MSG_010", context);
                                return Results;
                            }

                            var mockupPG = context.ART_WF_MOCKUP_PROCESS_PG.Where(m => m.MOCKUP_SUB_ID == mockup.MOCKUP_SUB_ID).OrderByDescending(o => o.MOCKUP_SUB_PG_ID).FirstOrDefault();

                            if (mockupPG != null)
                            {
                                vendorID = Convert.ToInt32(mockupPG.VENDOR);
                            }

                            if (artworkProcess == null || processPG == null || mockup == null && mockupPG == null || (mockupPG.VENDOR == null || mockupPG.VENDOR_OTHER == null))
                            {
                                Results.status = "E";
                                Results.msg = MessageHelper.GetMessage("MSG_007",context);
                                return Results;
                            }
                        }
                        else if (artworkProcess == null && vendorID <= 0)
                        {
                            Results.status = "E";
                            Results.msg = MessageHelper.GetMessage("MSG_010", context);
                            return Results;
                        }

                        var usersByVendor = context.ART_M_USER_VENDOR.Where(v => v.VENDOR_ID == vendorID).Select(s => s.USER_ID).ToList();
                        if (usersByVendor.Count == 0)
                        {
                            Results.status = "E";
                            Results.msg = "Vendor not found.";
                            return Results;
                        }

                        string msg = ArtworkProcessHelper.checkDupWF(param.data.PROCESS, context);
                        if (msg != "")
                        {
                            Results.status = "E";
                            Results.msg = msg;
                            return Results;
                        }

                        ART_WF_ARTWORK_PROCESS_RESULT processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
                        processResults.data = new List<ART_WF_ARTWORK_PROCESS_2>();
                        if (param.data.PROCESS != null)
                        {
                            if (usersByVendor != null && usersByVendor.Count > 0)
                            {
                                foreach (int iUserID in usersByVendor)
                                {
                                    ART_WF_ARTWORK_PROCESS_2 process2 = new ART_WF_ARTWORK_PROCESS_2();

                                    process2 = param.data.PROCESS;
                                    process2.CURRENT_USER_ID = iUserID;
                                    process2.CURRENT_VENDOR_ID = vendorID;

                                    var temp = ArtworkProcessHelper.SaveProcess(process2, context);
                                    foreach (var itemTemp in temp.data)
                                    {
                                        processResults.data.Add(itemTemp);
                                    }
                                }
                            }
                            else
                            {
                                //_processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                            }
                        }

                        foreach (var item2 in processResults.data)
                        {
                            ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA VendorData = new ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA();
                            VendorData = MapperServices.ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA(param.data);
                            VendorData.ARTWORK_SUB_ID = item2.ARTWORK_SUB_ID;
                            ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_SERVICE.SaveOrUpdate(VendorData, context);
                        }

                        //Results.data = new List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2>();
                        //ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2 item = new ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2();
                        //List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2> listItem = new List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2>();
                        //item.ARTWORK_PROCESS_VENDOR_ID = VendorData.ARTWORK_PROCESS_VENDOR_ID;
                        //listItem.Add(item);
                        //Results.data = listItem;


                        //---------------------------------------------------ticket# 473360 by aof ----------------------------------------------------------
                        // move function copyDielineFileToArtowrk from "Send_To_Vendor" button to after Submit. 
                        var step_SEND_VN_PM_ID = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == "SEND_VN_PM").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
                        if (step_SEND_VN_PM_ID == param.data.PROCESS.CURRENT_STEP_ID) {
                            ART_WF_ARTWORK_PROCESS_PG_REQUEST requestPG = new ART_WF_ARTWORK_PROCESS_PG_REQUEST();
                            requestPG.data = new ART_WF_ARTWORK_PROCESS_PG_2();
                            requestPG.data.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            BLL.Helpers.PGFormHelper.CopyDielineFileToArtwork(requestPG, context);
                        }
                        //---------------------------------------------------ticket# 473360 by aof ----------------------------------------------------------


                        dbContextTransaction.Commit();

                        foreach (var process in processResults.data)
                            EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO_VENDOR", context);

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

        public static ART_WF_ARTWORK_PROCESS_VENDOR_RESULT PostVendorSendToPA(ART_WF_ARTWORK_PROCESS_VENDOR_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_VENDOR_RESULT Results = new ART_WF_ARTWORK_PROCESS_VENDOR_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var vnData = MapperServices.ART_WF_ARTWORK_PROCESS_VENDOR(param.data);

                        var check = ART_WF_ARTWORK_PROCESS_VENDOR_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_VENDOR() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                        if (check.Count > 0)
                            vnData.ARTWORK_PROCESS_VENDOR_ID = check.FirstOrDefault().ARTWORK_PROCESS_VENDOR_ID;

                        ART_WF_ARTWORK_PROCESS_VENDOR_SERVICE.SaveOrUpdate(vnData, context);

                        if (param.data.ENDTASKFORM)
                        {
                            ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                            EndTaskVendorOtherUser(param, context);

                        }
                        dbContextTransaction.Commit();

                        if (param.data.ACTION_CODE == "SEND_BACK")
                            EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_SEND_BACK", context, param.data.COMMENT);
                        else if (param.data.ACTION_CODE == "SAVE") { }
                        //EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_OTHER_SAVE", context);
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

        public static ART_WF_ARTWORK_PROCESS_VENDOR_RESULT EndTaskVendorOtherUser(ART_WF_ARTWORK_PROCESS_VENDOR_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_PROCESS_VENDOR_RESULT Results = new ART_WF_ARTWORK_PROCESS_VENDOR_RESULT();

            List<int> listSubID = new List<int>();

            try
            {
                listSubID = CNService.FindArtworkSubId(param.data.ARTWORK_SUB_ID, context);

                var currentStepID = (from v in context.ART_WF_ARTWORK_PROCESS
                                     where v.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                     select v.CURRENT_STEP_ID).FirstOrDefault();

                var processVendors = (from v in context.ART_WF_ARTWORK_PROCESS
                                      where listSubID.Contains(v.ARTWORK_SUB_ID)
                                         && v.CURRENT_STEP_ID == currentStepID
                                         && String.IsNullOrEmpty(v.IS_END)
                                      select v).ToList();

                if (processVendors != null)
                {
                    ART_WF_ARTWORK_PROCESS _processVN = new ART_WF_ARTWORK_PROCESS();
                    foreach (ART_WF_ARTWORK_PROCESS iProcessVN in processVendors)
                    {
                        iProcessVN.IS_END = "X";
                        iProcessVN.UPDATE_DATE = DateTime.Now;
                        iProcessVN.UPDATE_BY = -1;
                        ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(iProcessVN, context);
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
    }



}
