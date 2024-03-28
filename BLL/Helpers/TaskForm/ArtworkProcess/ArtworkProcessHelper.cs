using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services;
using DAL;
using DAL.Model;

namespace BLL.Helpers
{
    public class ArtworkProcessHelper
    {
        private static string BR = "<br>";
        public static ART_WF_ARTWORK_PROCESS_RESULT GetProcess(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            string msgMatchingSO = "";
            string msgMachingBrand = "";
            ART_WF_ARTWORK_PROCESS_RESULT Results = new ART_WF_ARTWORK_PROCESS_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS(ART_WF_ARTWORK_PROCESS_SERVICE.GetAll(context));
                        }
                        else
                        {

                            var results = (from p in context.ART_WF_ARTWORK_PROCESS
                                           where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                           select p).ToList();

                            if (results.Count > 0)
                            {
                                Results.data = MapperServices.ART_WF_ARTWORK_PROCESS(results);
                            }
                        }

                        foreach (ART_WF_ARTWORK_PROCESS_2 item in Results.data)
                        {
                            ART_WF_ARTWORK_REQUEST_ITEM requestItem = new ART_WF_ARTWORK_REQUEST_ITEM();
                            requestItem.ARTWORK_ITEM_ID = item.ARTWORK_ITEM_ID;
                            var reqItem = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(requestItem, context).FirstOrDefault();
                            item.ARTWORK_NO_DISPLAY_TXT = reqItem.REQUEST_ITEM_NO;
                            item.CURRENT_STEP_DISPLAY_TXT = ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(item.CURRENT_STEP_ID, context).STEP_ARTWORK_NAME;
                            item.CURRENT_STEP_CODE_DISPLAY_TXT = ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(item.CURRENT_STEP_ID, context).STEP_ARTWORK_CODE;
                            var artwork_req = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(item.ARTWORK_REQUEST_ID, context);
                            item.ARTWORK_REQUEST_FORM_NO_DISPLAY_TXT = artwork_req.ARTWORK_REQUEST_NO;
                            item.ARTWORK_REFERENCE_REQUEST_NO_DISPLAY_TXT = artwork_req.REFERENCE_REQUEST_NO;
                            item.ARTWORK_FOLDER_NODE_ID = reqItem.REQUEST_FORM_FILE_NODE_ID;
                            item.NODE_ID_TXT = EncryptionService.EncryptAndUrlEncode(reqItem.REQUEST_FORM_FILE_NODE_ID.ToString());
                            item.FILE_NAME = reqItem.FILE_NAME;
                            item.FILE_EXTENSION = reqItem.EXTENSION;
                            if (item.STEP_DURATION_EXTEND_REASON_ID != null)
                            {
                                var reason = ART_M_DECISION_REASON_SERVICE.GetByART_M_DECISION_REASON_ID(item.STEP_DURATION_EXTEND_REASON_ID, context);
                                if (reason != null)
                                {
                                    item.STEP_DURATION_REMARK_REASON = reason.DESCRIPTION;
                                }
                            }
                            item.IS_SO_CHANGE = SalesOrderHelper.CheckIsSalesOrderChange(item.ARTWORK_SUB_ID, context);

                            ART_M_STEP_ARTWORK stepPA = new ART_M_STEP_ARTWORK();
                            ART_WF_ARTWORK_PROCESS_PA pa = new ART_WF_ARTWORK_PROCESS_PA();
                            ART_WF_ARTWORK_PROCESS proecss = new ART_WF_ARTWORK_PROCESS();
                            int stepPAID = 0;
                            int reqItemID = 0;

                            stepPA.STEP_ARTWORK_CODE = "SEND_PA";
                            stepPAID = ART_M_STEP_ARTWORK_SERVICE.GetByItem(stepPA, context).FirstOrDefault().STEP_ARTWORK_ID;
                            proecss = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(item.ARTWORK_SUB_ID, context);

                            reqItemID = proecss.ARTWORK_ITEM_ID;
                            proecss = new ART_WF_ARTWORK_PROCESS();
                            proecss.CURRENT_STEP_ID = stepPAID;
                            proecss.ARTWORK_ITEM_ID = reqItemID;
                            proecss = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(proecss, context).FirstOrDefault();

                            pa.ARTWORK_SUB_ID = proecss.ARTWORK_SUB_ID;
                            pa = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(pa, context).FirstOrDefault();

                            if (pa != null)
                            {
                                item.READY_CREATE_PO = pa.READY_CREATE_PO;
                                item.RECEIVE_SHADE_LIMIT = pa.RECEIVE_SHADE_LIMIT;
                                item.SHADE_LIMIT = pa.SHADE_LIMIT;
                                item.CHANGE_POINT = pa.CHANGE_POINT;
                                item.REFERENCE_MATERIAL = pa.REFERENCE_MATERIAL;
                                item.MATERIAL_STATUS = pa.REQUEST_MATERIAL_STATUS;
                                item.IS_LOCK_PRODUCT_CODE = pa.IS_LOCK_PRODUCT_CODE;
                                item.MATERIAL_NO = pa.MATERIAL_NO;
                            }

                            var SEND_PG = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                            var processPG = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { PARENT_ARTWORK_SUB_ID = item.ARTWORK_SUB_ID, ARTWORK_ITEM_ID = item.ARTWORK_ITEM_ID, CURRENT_STEP_ID = SEND_PG, IS_END = "X" }, context).OrderByDescending(i => i.UPDATE_DATE).FirstOrDefault();
                            if (processPG != null)
                            {
                                var dieline = ART_WF_ARTWORK_PROCESS_PG_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PG() { ARTWORK_SUB_ID = processPG.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                if (dieline != null)
                                {
                                    if (dieline.DIE_LINE_MOCKUP_ID != null)
                                    {
                                        item.MOCKUP_NO_DISPLAY_TXT = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(dieline.DIE_LINE_MOCKUP_ID, context).MOCKUP_NO;
                                    }
                                }
                                else
                                {
                                    var dieline_2 = ART_WF_ARTWORK_PROCESS_PG_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PG() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (dieline_2 != null)
                                    {
                                        item.MOCKUP_NO_DISPLAY_TXT = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(dieline_2.DIE_LINE_MOCKUP_ID, context).MOCKUP_NO;
                                    }
                                }
                            }
                            var processPG2 = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { PARENT_ARTWORK_SUB_ID = item.PARENT_ARTWORK_SUB_ID, ARTWORK_ITEM_ID = item.ARTWORK_ITEM_ID, CURRENT_STEP_ID = SEND_PG, IS_END = "X" }, context).OrderByDescending(i => i.UPDATE_DATE).FirstOrDefault();
                            if (processPG2 != null)
                            {
                                var dieline = ART_WF_ARTWORK_PROCESS_PG_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PG() { ARTWORK_SUB_ID = processPG2.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                if (dieline != null)
                                {
                                    if (dieline.DIE_LINE_MOCKUP_ID != null)
                                    {
                                        item.MOCKUP_NO_DISPLAY_TXT = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(dieline.DIE_LINE_MOCKUP_ID, context).MOCKUP_NO;
                                    }
                                }
                                else
                                {
                                    var dieline_2 = ART_WF_ARTWORK_PROCESS_PG_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PG() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (dieline_2 != null)
                                    {
                                        item.MOCKUP_NO_DISPLAY_TXT = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(dieline_2.DIE_LINE_MOCKUP_ID, context).MOCKUP_NO;
                                    }
                                }
                            }

                            item.READY_CREATE_PO_VALIDATE_MSG = ValidateReadyToCreatePO(item, artwork_req, item.IS_SO_CHANGE);

                            if (String.IsNullOrEmpty(item.READY_CREATE_PO_VALIDATE_MSG))
                            {
                                item.IS_READY_CREATE_PO = "X";
                            }
                            else
                            {
                                item.IS_READY_CREATE_PO = "";
                            }

                            string matchSOTmp = PAFormHelper.CheckMatchingSO_RequestForm(item.ARTWORK_SUB_ID, context);

                            if (!String.IsNullOrEmpty(matchSOTmp))
                            {
                                msgMatchingSO += matchSOTmp + "<br>";
                            }

                            string matchBrandTmp = PAFormHelper.CheckBrandRefMaterial_RequestForm(item.ARTWORK_SUB_ID, context);

                            if (!String.IsNullOrEmpty(matchBrandTmp))
                            {
                                msgMachingBrand += matchBrandTmp + "<br>";
                            }

                            //start ticket.445558 by aof 
                            item.CHECK_SO_REPEAT_IS_NOT_SEND_BACK_MK  = CheckSORepeat_IS_NOT_SEND_BACK_MK(item,artwork_req);
                            //last ticket.445558 by aof 

                        }
                    }
                }

                Results.status = "S";

                if (!String.IsNullOrEmpty(msgMatchingSO))
                {
                    msgMatchingSO = msgMatchingSO.Substring(0, msgMatchingSO.Length - 4);
                    if (!String.IsNullOrEmpty(msgMatchingSO))
                    {
                        if (!String.IsNullOrEmpty(msgMachingBrand))
                        {
                            msgMachingBrand = msgMachingBrand.Substring(0, msgMachingBrand.Length - 4);

                            msgMatchingSO = msgMatchingSO + "<br><br>" + msgMachingBrand;
                        }

                        Results.status = "E";
                        Results.msg = msgMatchingSO;
                        return Results;
                    }
                }

                if (!String.IsNullOrEmpty(msgMachingBrand))
                {
                    msgMachingBrand = msgMachingBrand.Substring(0, msgMachingBrand.Length - 4);
                    if (!String.IsNullOrEmpty(msgMachingBrand))
                    {
                        msgMachingBrand = msgMachingBrand.Replace(":", "");
                        Results.status = "E";
                        Results.msg = msgMachingBrand;
                        return Results;
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



        //ticket 445558 by aof writed CheckSORepeat_IS_NOT_SEND_BACK_MK function.
        public static string CheckSORepeat_IS_NOT_SEND_BACK_MK(ART_WF_ARTWORK_PROCESS_2 item, ART_WF_ARTWORK_REQUEST artwork_req)
        {
           string msg = "";

           using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {

                    // if (artwork_req.TYPE_OF_ARTWORK == "REPEAT")
                    if (!CNService.IsMarketingCreatedArtworkRequest(artwork_req,context))   //461704 by aof 
                    {
                        var stepSendBackMK = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_BACK_MK").FirstOrDefault();

                        if (stepSendBackMK != null)
                        {
                            var listProcess_SendBackMK = context.ART_WF_ARTWORK_PROCESS.Where(w => w.ARTWORK_REQUEST_ID == artwork_req.ARTWORK_REQUEST_ID
                                                        && w.CURRENT_STEP_ID == stepSendBackMK.STEP_ARTWORK_ID 
                                                        && w.ARTWORK_ITEM_ID == item.ARTWORK_ITEM_ID
                                                        && string.IsNullOrEmpty(w.IS_TERMINATE)).OrderBy(w=>w.CREATE_DATE);

                            if (listProcess_SendBackMK != null)
                            {
                                Boolean f_found = false;
                                foreach (ART_WF_ARTWORK_PROCESS process in listProcess_SendBackMK)
                                {
                                    if (CNService.IsMarketing(Convert.ToInt32(process.CURRENT_USER_ID), context) || CNService.IsRoleMK(Convert.ToInt32(process.CURRENT_USER_ID), context))
                                    {
                                        f_found = true;
                                        break;
                                    }
                                }
                                if (f_found ==false)
                                {
                                    msg = MessageHelper.GetMessage("MSG_036", context);
                                }
                            }

                        //if (cntProcess_SendBackMK == 0)
                            //{
                            //    msg = MessageHelper.GetMessage("MSG_036", context);
                            //}
                        }
                    }

                }
            }
           return msg;
        }
        //ticket 445558 by aof writed CheckSORepeat_IS_NOT_SEND_BACK_MK function.

        public static string ValidateReadyToCreatePO(ART_WF_ARTWORK_PROCESS_2 item, ART_WF_ARTWORK_REQUEST artwork_req, string isSOChange)
        {
            string msg = "";
            string msgResult = "";

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (artwork_req.TYPE_OF_ARTWORK == "NEW")
                    {
                        var stepCustPrint = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_CUS_PRINT").FirstOrDefault();
                        var stepCustShade = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_CUS_SHADE").FirstOrDefault();

                        var itemID = CNService.FindArtworkItemId(item.ARTWORK_SUB_ID, context);

                        var processCustPrint = (from p in context.ART_WF_ARTWORK_PROCESS
                                                where p.ARTWORK_ITEM_ID == itemID
                                                 && p.CURRENT_STEP_ID == stepCustPrint.STEP_ARTWORK_ID
                                                 && p.IS_END == "X"
                                                 && string.IsNullOrEmpty(p.REMARK_KILLPROCESS)   // by aof #INC-10373
                                                 && p.UPDATE_BY != -1  // added by aof 10/05/2022 support multi customer
                                                select p.ARTWORK_SUB_ID).ToList();

                        if (processCustPrint != null) //send to customer approve print master
                        {
                            var custPrint = (from c in context.ART_WF_ARTWORK_PROCESS_CUSTOMER
                                             where processCustPrint.Contains(c.ARTWORK_SUB_ID)
                                             select c).OrderByDescending(o => o.ARTWORK_PROCESS_CUSTOMER_ID).FirstOrDefault();

                            if (custPrint != null)
                            {
                                if (custPrint.DECISION_ACTION  == "0")  // by aof #INC-10373  customer decision approve  
                                {   
                                    if (custPrint.APPROVE_SHADE_LIMIT == "0") //customer not request approve shade limit
                                    {
                                        msg += ValidateReadyToCreatePO2(item, context, isSOChange);
                                    }
                                    else if (custPrint.APPROVE_SHADE_LIMIT == "1")  //customer request approve shade limit
                                    {
                                        var processCustShade = (from p in context.ART_WF_ARTWORK_PROCESS
                                                                where p.ARTWORK_ITEM_ID == itemID
                                                                 && p.CURRENT_STEP_ID == stepCustShade.STEP_ARTWORK_ID
                                                                 && p.IS_END == "X"
                                                                 && string.IsNullOrEmpty(p.REMARK_KILLPROCESS)   // by aof #INC-10373 on 03/09/2021 for shrade limit
                                                                 && p.UPDATE_DATE >= custPrint.UPDATE_DATE // by aof #INC-10373 on 03/09/2021 for shrade limit
                                                                 && p.UPDATE_BY != -1  // added by aof 10/05/2022 support multi customer
                                                                select p).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();



                                        if (processCustShade != null) // step send to customer approve shade limit
                                        {
                                            //---------------------  by aof #INC-10373 on 03/09/2021 for shrade limit

                                            var custShade = (from c in context.ART_WF_ARTWORK_PROCESS_CUSTOMER
                                                             where c.ARTWORK_SUB_ID.Equals(processCustShade.ARTWORK_SUB_ID)
                                                             select c).OrderByDescending(o => o.ARTWORK_PROCESS_CUSTOMER_ID).FirstOrDefault();

                                            var isApproveShade = false;

                                            if (custShade != null) {
                                                if (custShade.DECISION_ACTION == "0")  // customer decision approve shade limit
                                                {
                                                    isApproveShade = true;
                                                }
                                            }

                                            if (isApproveShade == true)
                                            {
                                                msg += ValidateReadyToCreatePO2(item, context, isSOChange);
                                            }
                                            else
                                            {
                                                msg += MessageHelper.GetMessage("MSG_015", context) + BR;
                                            }

                                            //--------------------- by aof #INC-10373 on 03/09/2021 for shrade limit

                                            //msg += ValidateReadyToCreatePO2(item, context, isSOChange);
                                        }
                                        else // step not send to customer approve shade limit
                                        {
                                            msg += MessageHelper.GetMessage("MSG_015", context) + BR;
                                        }
                                    }
                                } else
                                {

                                    msg += MessageHelper.GetMessage("MSG_014", context) + BR;   // by aof #INC-10373  customer decision not approve  
                                }
                                
                            }
                        }
                        else // step not send to customer approve print master
                        {
                            msg += MessageHelper.GetMessage("MSG_014", context) + BR;
                        }

                        if (processCustPrint.Count <= 0)
                        {
                            msg += MessageHelper.GetMessage("MSG_014", context) + BR;
                        }
                    }
                    else
                    {
                        msg += ValidateReadyToCreatePO2(item, context, isSOChange);
                    }

                    //-------------------- tikcet# 463880 by aof.

                    var mat = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                              where p.ARTWORK_SUB_ID  == item.ARTWORK_SUB_ID
                              select p.MATERIAL_NO).FirstOrDefault();

                    if (string.IsNullOrEmpty(mat))
                    {
                        msg += MessageHelper.GetMessage("MSG_037", context) + BR;  //MSG_037
                    }


                    //if (!string.IsNullOrEmpty(msg))
                    //{
                    //    var itemID = CNService.FindArtworkItemId(item.ARTWORK_SUB_ID, context);
                    //    var stepPA = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_PA").FirstOrDefault();
                    //    var process_pa = (from p in context.ART_WF_ARTWORK_PROCESS
                    //                      where p.ARTWORK_ITEM_ID == itemID
                    //                      select p
                    //                      ).ToList().FirstOrDefault();
                    //    if (process_pa != null)
                    //    {
                    //        var pa = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                    //                  where p.ARTWORK_SUB_ID == process_pa.ARTWORK_SUB_ID
                    //                  select p
                    //                  ).ToList().FirstOrDefault();
                    //        if (pa != null)
                    //        {
                    //            if (pa.READY_CREATE_PO != "X")
                    //            {

                    //                var pa_update = new ART_WF_ARTWORK_PROCESS_PA();
                    //                pa_update.ARTWORK_SUB_ID = pa
                    //                pa_update.READY_CREATE_PO = "";
                    //                //pa.CREATE_BY = param.data.UPDATE_BY;
                    //                //pa.UPDATE_BY = param.data.UPDATE_BY;
                    //                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(pa, context);
                    //            }
                    //        }
                    //    }
            
                    //}

                    //-------------------- tikcet# 463880 by aof.
                }
            }


        


            if (!String.IsNullOrEmpty(msg))
            {
                msgResult = MessageHelper.GetMessage("MSG_019") + BR + msg;
            }
            return msgResult;
        }

        public static string ValidateReadyToCreatePO2(ART_WF_ARTWORK_PROCESS_2 item, ARTWORKEntities context, string isSOChange)
        {
            string msg = "";

            //Check Sales Order Change
            // string soChange = "";
            //soChange = SalesOrderHelper.CheckIsSalesOrderChange(item.ARTWORK_SUB_ID, context);

            if (isSOChange == "X")
            {
                msg += MessageHelper.GetMessage("MSG_016", context) + BR;
            }

            //Check Workflow internal send to PA
            var itemID = CNService.FindArtworkItemId(item.ARTWORK_SUB_ID, context);

            var subs = (from p in context.ART_WF_ARTWORK_PROCESS
                        where p.ARTWORK_ITEM_ID == itemID
                         && p.PARENT_ARTWORK_SUB_ID != null
                         && p.IS_END != "X"
                        select p).Count();

            if (subs > 0)
            {
                msg += MessageHelper.GetMessage("MSG_017", context) + BR;
            }

            //Check Final Dieline
            var stepPG = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_PG").FirstOrDefault();

            var subPG = (from p in context.ART_WF_ARTWORK_PROCESS
                         where p.ARTWORK_ITEM_ID == itemID
                          && p.CURRENT_STEP_ID == stepPG.STEP_ARTWORK_ID
                         select p).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

            if (subPG != null)
            {
                var processPG = (from g in context.ART_WF_ARTWORK_PROCESS_PG
                                 where g.ARTWORK_SUB_ID == subPG.ARTWORK_SUB_ID
                                 select g).FirstOrDefault();

                if (processPG != null)
                {
                    var mockup = (from m in context.ART_WF_MOCKUP_PROCESS
                                  where m.MOCKUP_ID == processPG.DIE_LINE_MOCKUP_ID
                                  select m).FirstOrDefault();

                    if (mockup != null)
                    {
                        var stepMOPG = context.ART_M_STEP_MOCKUP.Where(w => w.STEP_MOCKUP_CODE == "SEND_PG").FirstOrDefault();

                        var processMO = (from m in context.ART_WF_MOCKUP_PROCESS
                                         where m.MOCKUP_ID == mockup.MOCKUP_ID
                                            && m.CURRENT_STEP_ID == stepMOPG.STEP_MOCKUP_ID
                                            && m.PARENT_MOCKUP_SUB_ID == null
                                         select m).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                        if (processMO != null)
                        {
                            if (processMO.IS_END != "X")
                            {
                                msg += MessageHelper.GetMessage("MSG_018", context) + BR;
                            }
                        }
                    }
                }
            }

            return msg;
        }

        public static ART_WF_ARTWORK_PROCESS_RESULT AcceptTask(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_RESULT Results = new ART_WF_ARTWORK_PROCESS_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        context.Database.CommandTimeout = 300;

                        var stepPAId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var stepPGId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var stepPPId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PP" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var temp = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);

                        if (temp.CURRENT_STEP_ID == stepPAId)
                        {
                            AcceptTask_PA_AllInArtwork(param, context);
                        }
                        else if (temp.CURRENT_STEP_ID == stepPGId)
                        {
                            AcceptTask_PG_AllInArtwork(param, context);
                        }
                        else if (temp.CURRENT_STEP_ID == stepPPId)
                        {
                            AcceptTask_PP_AllInArtwork(param, stepPPId, context);
                        }
                        else
                        {
                            temp.CURRENT_USER_ID = param.data.CURRENT_USER_ID;
                            temp.UPDATE_BY = param.data.UPDATE_BY;

                            ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(temp, context);
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

        public static ART_WF_ARTWORK_PROCESS_RESULT AcceptTaskForRepeat(ART_WF_ARTWORK_PROCESS_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_PROCESS_RESULT Results = new ART_WF_ARTWORK_PROCESS_RESULT();

            try
            {
                //using (var context = new ARTWORKEntities())
                {
                    //using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        //context.Database.CommandTimeout = 300;

                        //var stepPAId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        //var stepPGId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        //var stepPPId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PP" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var temp = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);

                        //if (temp.CURRENT_STEP_ID == stepPAId)
                        //{
                        //    AcceptTask_PA_AllInArtwork(param, context);
                        //}
                        //else if (temp.CURRENT_STEP_ID == stepPGId)
                        //{
                        //    AcceptTask_PG_AllInArtwork(param, context);
                        //}
                        //else if (temp.CURRENT_STEP_ID == stepPPId)
                        //{
                        //    AcceptTask_PP_AllInArtwork(param, stepPPId, context);
                        //}
                        //else
                        //{
                        temp.CURRENT_USER_ID = param.data.CURRENT_USER_ID;
                        temp.UPDATE_BY = param.data.UPDATE_BY;

                        ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(temp, context);
                        //}

                        //dbContextTransaction.Commit();

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

        private static void AcceptTask_PA_AllInArtwork(ART_WF_ARTWORK_PROCESS_REQUEST param, ARTWORKEntities context)
        {
            var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
            var temp = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);

            var listProcess = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_REQUEST_ID = temp.ARTWORK_REQUEST_ID, CURRENT_STEP_ID = stepId }, context);
            foreach (var item in listProcess)
            {
                if (item.CURRENT_USER_ID == null && item.CURRENT_STEP_ID == stepId)
                {
                    item.CURRENT_USER_ID = param.data.CURRENT_USER_ID;
                    item.UPDATE_BY = param.data.UPDATE_BY;
                    ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(item, context);
                }

                ART_WF_ARTWORK_PROCESS_PA processPA = new ART_WF_ARTWORK_PROCESS_PA();
                processPA.ARTWORK_SUB_ID = item.ARTWORK_SUB_ID;
                processPA = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(processPA, context).FirstOrDefault();
                if (processPA != null)
                {
                    processPA.PA_USER_ID = param.data.CURRENT_USER_ID;
                    ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);
                }
            }
        }

        private static void AcceptTask_PG_AllInArtwork(ART_WF_ARTWORK_PROCESS_REQUEST param, ARTWORKEntities context)
        {
            var stepPGId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_ARTWORK_ID;
            var stepPAId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;

            var process_1 = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);

            //lock table
            //var query = " update [ART_WF_ARTWORK_PROCESS]";
            //query += " set [CURRENT_STEP_ID] = '" + stepPGId + "'";
            //query += " where [CURRENT_STEP_ID] = '" + stepPGId + "'";
            //query += " and [CURRENT_USER_ID] is null ";
            //query += " and [IS_END] is null ";
            //query += " and [ARTWORK_REQUEST_ID] = '" + process_1.ARTWORK_REQUEST_ID + "' ";
            //context.Database.ExecuteSqlCommand(query);

            var listProcessPG = (from g in context.ART_WF_ARTWORK_PROCESS
                                 where g.CURRENT_STEP_ID == stepPGId
                                    && String.IsNullOrEmpty(g.IS_END)
                                    && g.CURRENT_USER_ID == null
                                    && g.ARTWORK_REQUEST_ID == process_1.ARTWORK_REQUEST_ID
                                 select g).ToList();

            foreach (var item in listProcessPG)
            {
                item.CURRENT_USER_ID = param.data.CURRENT_USER_ID;
                item.UPDATE_BY = param.data.UPDATE_BY;
                ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(item, context);

                //Stamp PG user in Process PA 
                var itemID = CNService.FindArtworkItemId(item.ARTWORK_SUB_ID, context);

                var processSubPA = (from p in context.ART_WF_ARTWORK_PROCESS
                                    where p.ARTWORK_ITEM_ID == itemID
                                    && p.CURRENT_STEP_ID == stepPAId
                                    select p.ARTWORK_SUB_ID).FirstOrDefault();

                if (processSubPA > 0)
                {
                    var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                     where p.ARTWORK_SUB_ID == processSubPA
                                     select p).FirstOrDefault();

                    if (processPA != null)
                    {
                        processPA.PG_USER_ID = param.data.CURRENT_USER_ID;
                        ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);
                    }
                }
            }
        }

        private static void AcceptTask_PP_AllInArtwork(ART_WF_ARTWORK_PROCESS_REQUEST param, int stepArtworkPP, ARTWORKEntities context)
        {
            List<int> list_artwork_sub_id = new List<int>();
            list_artwork_sub_id.Add(param.data.ARTWORK_SUB_ID);

            string accept_soldto = "";
            string accept_shipto = "";
            int accept_pkgtype = 0;

            SetSoldToShipToPkgTypeByArtworkSubId(param.data.ARTWORK_SUB_ID, ref accept_soldto, ref accept_shipto, ref accept_pkgtype, context);
            int accept_vendor = GetVendorByArtworkSubId(param.data.ARTWORK_SUB_ID, context);

            //lock table
            //var query = " update [ART_WF_ARTWORK_PROCESS]";
            //query += " set [CURRENT_STEP_ID] = '" + stepArtworkPP + "'";
            //query += " where [CURRENT_STEP_ID] = '" + stepArtworkPP + "'";
            //query += " and [CURRENT_USER_ID] is null ";
            //query += " and [IS_END] is null ";
            //context.Database.ExecuteSqlCommand(query);

            var listProcess = (from p in context.ART_WF_ARTWORK_PROCESS
                               where p.CURRENT_STEP_ID == stepArtworkPP
                                && p.CURRENT_USER_ID == null
                                && p.IS_END == null
                                && p.ARTWORK_SUB_ID != param.data.ARTWORK_SUB_ID
                               select p).ToList();

            foreach (var item in listProcess)
            {
                string process_soldto = "";
                string process_shipto = "";
                int process_pkgtype = 0;
                SetSoldToShipToPkgTypeByArtworkSubId(item.ARTWORK_SUB_ID, ref process_soldto, ref process_shipto, ref process_pkgtype, context);
                int process_vendor = GetVendorByArtworkSubId(item.ARTWORK_SUB_ID, context);

                if (accept_soldto == process_soldto && accept_shipto == process_shipto && accept_vendor == process_vendor && accept_pkgtype == process_pkgtype)
                {
                    list_artwork_sub_id.Add(item.ARTWORK_SUB_ID);
                }
            }

            foreach (var i in list_artwork_sub_id)
            {
                var item = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(i, context);
                item.CURRENT_USER_ID = param.data.CURRENT_USER_ID;
                item.UPDATE_BY = param.data.UPDATE_BY;
                item.ARTWORK_SUB_ID = i;
                ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(item, context);
            }
        }

        private static void SetSoldToShipToPkgTypeByArtworkSubId(int artwork_sub_id, ref string soldto, ref string shipto, ref int pkg, ARTWORKEntities context)
        {
            int parent_artworksubid = CNService.FindParentArtworkSubId(artwork_sub_id, context);

            var so_detail = ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_SO_DETAIL() { ARTWORK_SUB_ID = parent_artworksubid }, context).FirstOrDefault();

            if (so_detail != null)
            {
                var so_header = SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.GetByItem(new SAP_M_PO_COMPLETE_SO_HEADER() { SALES_ORDER_NO = so_detail.SALES_ORDER_NO }, context).FirstOrDefault();
                if (so_header != null)
                {
                    soldto = so_header.SOLD_TO;
                    shipto = so_header.SHIP_TO;
                }
            }
            var pa = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PA() { ARTWORK_SUB_ID = parent_artworksubid }, context).FirstOrDefault();
            if (pa != null)
            {
                pkg = pa.MATERIAL_GROUP_ID.GetValueOrDefault();
            }
        }

        private static int GetVendorByArtworkSubId(int artwork_sub_id, ARTWORKEntities context)
        {
            var stepPG = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_ARTWORK_ID;
            var stepMockupPG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
            var artwork_process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = artwork_sub_id }, context).FirstOrDefault();
            if (artwork_process != null)
            {
                var artwork_process2 = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = artwork_process.ARTWORK_ITEM_ID, CURRENT_STEP_ID = stepPG }, context).OrderByDescending(x => x.UPDATE_DATE).FirstOrDefault();

                if (artwork_process2 != null)
                {
                    var artwork_pg = ART_WF_ARTWORK_PROCESS_PG_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PG() { ARTWORK_SUB_ID = artwork_process2.ARTWORK_SUB_ID }, context).FirstOrDefault();
                    if (artwork_pg != null)
                    {
                        if (artwork_pg.DIE_LINE_MOCKUP_ID != null)
                        {
                            var mockup_process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = artwork_pg.DIE_LINE_MOCKUP_ID.GetValueOrDefault(), CURRENT_STEP_ID = stepMockupPG }, context).OrderBy(x => x.UPDATE_DATE).FirstOrDefault();

                            if (mockup_process != null)
                            {
                                var mockup_pg = ART_WF_MOCKUP_PROCESS_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG() { MOCKUP_SUB_ID = mockup_process.MOCKUP_SUB_ID }, context).FirstOrDefault();
                                if (mockup_pg != null)
                                {
                                    return mockup_pg.VENDOR.GetValueOrDefault();
                                }
                            }
                        }
                    }
                }
            }

            return 0;
        }

        private static string GetValueText(ART_WF_ARTWORK_REQUEST request_2, ART_WF_ARTWORK_PROCESS_PA processPA_2)
        {
            string Value_2 = "";

            if (processPA_2.PACK_SIZE_ID != null && processPA_2.PACK_SIZE_ID > -1)
            {
                Value_2 += "[" + processPA_2.PACK_SIZE_ID + "]";
            }
            else
            {
                Value_2 += "[" + processPA_2.PACK_SIZE_OTHER + "]";
            }

            if (processPA_2.PRIMARY_SIZE_ID != null && processPA_2.PRIMARY_SIZE_ID > -1)
            {
                Value_2 += "[" + processPA_2.PRIMARY_SIZE_ID.ToString() + "]";
            }
            else
            {
                Value_2 += "[" + processPA_2.PRIMARY_SIZE_OTHER + "]";
            }

            if (request_2.PRIMARY_TYPE_ID != null && request_2.PRIMARY_TYPE_ID > -1)
            {
                Value_2 += "[" + request_2.PRIMARY_TYPE_ID.ToString() + "]";
            }
            else
            {
                Value_2 += "[" + request_2.PRIMARY_TYPE_OTHER + "]";
            }

            if (processPA_2.PACKING_STYLE_ID != null && processPA_2.PACKING_STYLE_ID > -1)
            {
                Value_2 += "[" + processPA_2.PACKING_STYLE_ID.ToString() + "]";
            }
            else
            {
                Value_2 += "[" + processPA_2.PACKING_STYLE_OTHER + "]";
            }

            if (processPA_2.MATERIAL_GROUP_ID != null)
            {
                Value_2 += "[" + processPA_2.MATERIAL_GROUP_ID.ToString() + "]";
            }

            if (processPA_2.THREE_P_ID != null)
            {
                Value_2 += "[" + processPA_2.THREE_P_ID.ToString() + "]";
            }

            if (processPA_2.TWO_P_ID != null)
            {
                Value_2 += "[" + processPA_2.TWO_P_ID.ToString() + "]";
            }

            return Value_2;
        }

        public static ART_WF_ARTWORK_PROCESS_RESULT SaveProcess(ART_WF_ARTWORK_PROCESS_2 tempParam, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_PROCESS_RESULT Results = new ART_WF_ARTWORK_PROCESS_RESULT();
            ART_WF_ARTWORK_PROCESS process = new ART_WF_ARTWORK_PROCESS();
            process = MapperServices.ART_WF_ARTWORK_PROCESS(tempParam);

            process.CURRENT_USER_ID = CNService.GetLastestActionArtwork(process, context);

            CNService.CheckDelegateBeforeRountingArtwork(process, context);
            //ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(process, context);

            List<ART_WF_ARTWORK_PROCESS_2> listProcess = new List<ART_WF_ARTWORK_PROCESS_2>();
            listProcess.Add(MapperServices.ART_WF_ARTWORK_PROCESS(process));

            Results.data = listProcess;

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_RESULT EndTaskFormPG(ART_WF_ARTWORK_PROCESS_PG_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_RESULT Results = new ART_WF_ARTWORK_PROCESS_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var temp = ART_WF_ARTWORK_PROCESS_PG_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PG() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context).FirstOrDefault();
                        if (temp != null)
                        {
                            temp.COMMENT = param.data.COMMENT;
                            temp.REASON_ID = param.data.REASON_ID;
                            temp.ACTION_CODE = param.data.ACTION_CODE;
                            temp.UPDATE_BY = param.data.UPDATE_BY;
                            ART_WF_ARTWORK_PROCESS_PG_SERVICE.SaveOrUpdate(temp, context);

                            EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                        }
                        else
                        {
                            int itemID = 0;
                            int mockup_dieline_id = 0;

                            itemID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context);

                            var stepPG = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_PG").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();

                            var processPGSubIDs = (from p in context.ART_WF_ARTWORK_PROCESS
                                                   where p.ARTWORK_ITEM_ID == itemID
                                                      && p.CURRENT_STEP_ID == stepPG
                                                      && p.IS_END == "X"
                                                   select p.ARTWORK_SUB_ID).ToList();
                            if (processPGSubIDs != null && processPGSubIDs.Count > 0)
                            {
                                var processPGData = (from p in context.ART_WF_ARTWORK_PROCESS_PG
                                                     where processPGSubIDs.Contains(p.ARTWORK_SUB_ID)
                                                     where p.DIE_LINE_MOCKUP_ID != null
                                                     select p).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                                if (processPGData != null && processPGData.DIE_LINE_MOCKUP_ID != null)
                                {
                                    mockup_dieline_id = Convert.ToInt32(processPGData.DIE_LINE_MOCKUP_ID);
                                }
                            }

                            if (param.data.ACTION_CODE == "SEND_BACK")
                            {
                                temp = new ART_WF_ARTWORK_PROCESS_PG();
                                temp.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;
                                temp.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                temp.COMMENT = param.data.COMMENT;
                                temp.REASON_ID = param.data.REASON_ID;
                                temp.ACTION_CODE = param.data.ACTION_CODE;
                                temp.UPDATE_BY = param.data.UPDATE_BY;
                                temp.CREATE_BY = param.data.CREATE_BY;

                                if (mockup_dieline_id > 0)
                                {
                                    temp.DIE_LINE_MOCKUP_ID = mockup_dieline_id;
                                }

                                ART_WF_ARTWORK_PROCESS_PG_SERVICE.SaveOrUpdate(temp, context);

                                EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                            }
                            else if (param.data.ACTION_CODE == "SUBMIT")
                            {
                                if (mockup_dieline_id > 0)
                                {
                                    temp = new ART_WF_ARTWORK_PROCESS_PG();
                                    temp.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;
                                    temp.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                    temp.COMMENT = param.data.COMMENT;
                                    temp.REASON_ID = param.data.REASON_ID;
                                    temp.ACTION_CODE = param.data.ACTION_CODE;
                                    temp.UPDATE_BY = param.data.UPDATE_BY;
                                    temp.CREATE_BY = param.data.CREATE_BY;
                                    temp.DIE_LINE_MOCKUP_ID = mockup_dieline_id;

                                    ART_WF_ARTWORK_PROCESS_PG_SERVICE.SaveOrUpdate(temp, context);

                                    EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                                }
                                else
                                {
                                    Results.status = "E";
                                    Results.msg = "Please assign dieline before send to PA.";
                                    return Results;
                                }

                             
                            }

                        }

                        Results.status = "S";

                        if (Results.status == "S")
                        {
                            PGFormHelper.CopyDielineFileToArtwork(param, context);
                        }

                        dbContextTransaction.Commit();

                        if (param.data.ACTION_CODE == "SEND_BACK")
                        {
                            EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_SEND_BACK", context);
                        }
                        else if (param.data.ACTION_CODE == "SUBMIT")
                        {
                            EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_OTHER_SUBMIT", context);
                        }
                    }
                }

                CNService.UpdateMaterialLock(param.data.ARTWORK_SUB_ID);
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }
        public static void EndTaskForm(int ARTWORK_SUB_ID, int UPDATE_BY, ARTWORKEntities context)
        {
            var temp = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(ARTWORK_SUB_ID, context);
            temp.IS_END = "X";
            temp.UPDATE_BY = UPDATE_BY;
            //#437016
            ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(temp, context);
            if(temp.CURRENT_STEP_ID == 2 && temp.IS_END == "X")
            {
                ART_WF_ARTWORK_PROCESS_REQUEST p = new ART_WF_ARTWORK_PROCESS_REQUEST();
                ART_WF_ARTWORK_PROCESS_2 data = new ART_WF_ARTWORK_PROCESS_2();
                data.ARTWORK_SUB_ID = temp.ARTWORK_SUB_ID;
                p.data = data;
               CNService.CompletePOForm(p, context);
            }
        }
        public static string checkDupWF(ART_WF_ARTWORK_PROCESS_2 process, ARTWORKEntities context)
        {
            string msg = "";

            msg = MessageHelper.GetMessage("MSG_008", context);

            if (process.CURRENT_VENDOR_ID > 0 || process.CURRENT_CUSTOMER_ID > 0)
            {
                //cus or ven
                ART_WF_ARTWORK_PROCESS filter = new ART_WF_ARTWORK_PROCESS();
                filter.ARTWORK_ITEM_ID = process.ARTWORK_ITEM_ID;
                filter.CURRENT_USER_ID = process.CURRENT_USER_ID;
                filter.CURRENT_STEP_ID = process.CURRENT_STEP_ID;
                if (ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(filter, context).Where(m => m.IS_END == null).ToList().Count > 0)
                {
                    msg += " to " + CNService.GetUserName(process.CURRENT_USER_ID, context) + ".";
                    return msg;
                }
            }
            else
            {
                //internal
                ART_WF_ARTWORK_PROCESS filter = new ART_WF_ARTWORK_PROCESS();
                filter.ARTWORK_ITEM_ID = process.ARTWORK_ITEM_ID;
                filter.CURRENT_STEP_ID = process.CURRENT_STEP_ID;
                if (ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(filter, context).Where(m => m.IS_END == null).ToList().Count > 0)
                {
                    msg += " for " + ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(process.CURRENT_STEP_ID, context).STEP_ARTWORK_NAME + ".";
                    return msg;
                }
            }
            return "";
        }

        public static bool moveFileArtworkToMatWorkspace_Old(int artworkItemId, ARTWORKEntities context)
        {
            try
            {
                var SecondaryPackagingNodeID = ConfigurationManager.AppSettings["SecondaryPackagingNodeID"];
                var SecondaryPkgArtworkFolderName = ConfigurationManager.AppSettings["SecondaryPkgArtworkFolderName"];//10 - Final Artwork
                var ArtworkFolderName = ConfigurationManager.AppSettings["ArtworkFolderNamePrintMaster"];//20 - Print Master

                var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = artworkItemId, CURRENT_STEP_ID = SEND_PA }, context).FirstOrDefault();

                if (process != null)
                {
                    var MATERIAL_NO = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                       where p.ARTWORK_SUB_ID == process.ARTWORK_SUB_ID
                                       select p.MATERIAL_NO).FirstOrDefault();

                    if (!string.IsNullOrEmpty(MATERIAL_NO))
                    {
                        var mat5 = MATERIAL_NO;
                        if (!string.IsNullOrEmpty(mat5))
                        {
                            var materialDesc = CNService.getMatDesc(mat5, context);
                            if (!string.IsNullOrEmpty(materialDesc))
                            {
                                var token = CWSService.getAuthToken();
                                var matDesc = materialDesc;

                                var nodeMat5 = CWSService.getNodeByName(Convert.ToInt64(SecondaryPackagingNodeID), mat5 + " - " + matDesc, token);

                                var nodeDes = CWSService.getNodeByName(-nodeMat5.ID, SecondaryPkgArtworkFolderName, token);

                                var artworkWF = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(artworkItemId, context);

                                var artworkNo = artworkWF.REQUEST_ITEM_NO;
                                var artworkNodeId = Convert.ToInt64(process.ARTWORK_FOLDER_NODE_ID);

                                var nodeFileArtwork = CWSService.getNodeByName(artworkNodeId, ArtworkFolderName, token);
                                var allFileArtwork = CWSService.getAllNodeInFolder(nodeFileArtwork.ID, token);

                                if (allFileArtwork == null)
                                {
                                    return false;
                                }
                                else
                                {
                                    foreach (var item in allFileArtwork)
                                    {
                                        var nodeIdFrom = item.ID;
                                        var nodeIdTo = nodeDes.ID;

                                        CWSService.copyNodeToWorkspace(item.Name, nodeIdFrom, nodeIdTo, mat5, token);
                                    }
                                }

                                CNService.updateIS_HAS_FILES_WITHOUT_CHECK(mat5, context);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ART_SYS_LOG model = new ART_SYS_LOG();
                model.ACTION = "E";
                model.TABLE_NAME = "Move File Artwork To Material Workspace";
                model.CREATE_BY = -1;
                model.UPDATE_BY = -1;
                model.NEW_VALUE = artworkItemId.ToString();
                if (!string.IsNullOrEmpty(ex.StackTrace))
                {
                    model.ERROR_MSG = ex.Message + "<br/>" + ex.StackTrace;
                }
                else
                {
                    model.ERROR_MSG = ex.Message;
                }
                model.ERROR_MSG = CNService.SubString(model.ERROR_MSG, 4000);

                if (ex.InnerException != null)
                {
                    if (!string.IsNullOrEmpty(ex.InnerException.Message))
                    {
                        model.OLD_VALUE = CNService.SubString(ex.InnerException.Message, 4000);
                    }
                }
                ART_SYS_LOG_SERVICE.SaveNoLog(model, context);

                throw ex;
            }
        }

        public static bool moveFileArtworkToMatWorkspace(int artworkItemId, ARTWORKEntities context,ref string error_msg) // added by aof on 17/03/2022.
        //public static bool moveFileArtworkToMatWorkspace(int artworkItemId, ARTWORKEntities context)
        {



            try
            {

                // added by aof INC-118584 request type is 'NEW' move to workspace only. 
                var ARTWORK_REQUEST_ID = (from p in context.ART_WF_ARTWORK_REQUEST_ITEM
                                          where p.ARTWORK_ITEM_ID == artworkItemId
                                          select p.ARTWORK_REQUEST_ID).FirstOrDefault();

                var TYPE_OF_ARTWORK = (from p in context.ART_WF_ARTWORK_REQUEST
                                       where p.ARTWORK_REQUEST_ID == ARTWORK_REQUEST_ID
                                       select p.TYPE_OF_ARTWORK).FirstOrDefault();            
                if (TYPE_OF_ARTWORK != "NEW")
                {
                    return true;
                }
                // added by aof INC-118584


                error_msg = "";  // added by aof on 17/03/2022.
                var SecondaryPackagingNodeID = ConfigurationManager.AppSettings["SecondaryPackagingNodeID"];
                var SecondaryPkgArtworkFolderName = ConfigurationManager.AppSettings["SecondaryPkgArtworkFolderName"];//10 - Final Artwork
                var ArtworkFolderName = ConfigurationManager.AppSettings["ArtworkFolderNamePrintMaster"];//20 - Print Master

                var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = artworkItemId, CURRENT_STEP_ID = SEND_PA }, context).FirstOrDefault();

                if (process != null)
                {
                    var MATERIAL_NO = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                       where p.ARTWORK_SUB_ID == process.ARTWORK_SUB_ID
                                       select p.MATERIAL_NO).FirstOrDefault();

                    if (!string.IsNullOrEmpty(MATERIAL_NO))
                    {
                        var mat5 = MATERIAL_NO;
                        if (!string.IsNullOrEmpty(mat5))
                        {
                            var materialDesc = CNService.getMatDesc(mat5, context);
                            if (!string.IsNullOrEmpty(materialDesc))
                            {
                                var token = CWSService.getAuthToken();
                                var matDesc = materialDesc;

                                var nodeMat5 = CWSService.getNodeByName(Convert.ToInt64(SecondaryPackagingNodeID), mat5 + " - " + matDesc, token);

                                if (nodeMat5 != null)   // this block if is added to show a problem message correctly by aof on 17/03/2022.
                                {
                                    var nodeDes = CWSService.getNodeByName(-nodeMat5.ID, SecondaryPkgArtworkFolderName, token);

                                    var artworkWF = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(artworkItemId, context);

                                    var artworkNo = artworkWF.REQUEST_ITEM_NO;
                                    var artworkNodeId = Convert.ToInt64(process.ARTWORK_FOLDER_NODE_ID);

                                    var nodeFileArtwork = CWSService.getNodeByName(artworkNodeId, ArtworkFolderName, token);
                                    var allFileArtwork = CWSService.getAllNodeInFolder(nodeFileArtwork.ID, token);

                                    if (allFileArtwork == null)
                                    {
                                        error_msg = "System not found print master file in this workflow "+ artworkNo + "."; // added by aof on 17/03/2022.
                                        return false;
                                    }
                                    else
                                    {
                                        foreach (var item in allFileArtwork)
                                        {
                                            var nodeIdFrom = item.ID;
                                            var nodeIdTo = nodeDes.ID;

                                            CWSService.copyNodeToWorkspace(item.Name, nodeIdFrom, nodeIdTo, mat5, token);
                                        }
                                    }

                                    CNService.updateIS_HAS_FILES_WITHOUT_CHECK(mat5, context);
                                }  
                                else
                                {
                                    error_msg = "System not found material master node {" + mat5 + " - " + matDesc + "}."; // added by aof on 17/03/2022.
                                    return false;
                                } // this block if is added to show a problem message correctly by aof on 17/03/2022.
                            }                  
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ART_SYS_LOG model = new ART_SYS_LOG();
                model.ACTION = "E";
                model.TABLE_NAME = "Move File Artwork To Material Workspace";
                model.CREATE_BY = -1;
                model.UPDATE_BY = -1;
                model.NEW_VALUE = artworkItemId.ToString();
                if (!string.IsNullOrEmpty(ex.StackTrace))
                {
                    model.ERROR_MSG = ex.Message + "<br/>" + ex.StackTrace;
                }
                else
                {
                    model.ERROR_MSG = ex.Message;
                }
                model.ERROR_MSG = CNService.SubString(model.ERROR_MSG, 4000);

                if (ex.InnerException != null)
                {
                    if (!string.IsNullOrEmpty(ex.InnerException.Message))
                    {
                        model.OLD_VALUE = CNService.SubString(ex.InnerException.Message, 4000);
                    }
                }
                ART_SYS_LOG_SERVICE.SaveNoLog(model, context);

                throw ex;
            }
        }

        public static string CheckOverDue(ART_WF_ARTWORK_PROCESS process, ARTWORKEntities context)
        {
            string result = "0";//0:hide,1:show

            string isStepDurationExtend = process.IS_STEP_DURATION_EXTEND;
            string WFCompletedOrTerminated = process.IS_END == "X" || process.IS_TERMINATE == "X" || !string.IsNullOrEmpty(process.REMARK_KILLPROCESS) ? "1" : "0";
            var duration = !string.IsNullOrEmpty(isStepDurationExtend) ? ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(process.CURRENT_STEP_ID, context).DURATION_EXTEND : ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(process.CURRENT_STEP_ID, context).DURATION;
            DateTime dtReceiveWf = process.CREATE_DATE;
            DateTime dtWillFinish = CNService.AddBusinessDays(dtReceiveWf, (int)Math.Ceiling(duration.Value));
            if (DateTime.Now > dtWillFinish && WFCompletedOrTerminated.Equals("0"))
            {
                result = "1";
            }

            return result;
        }

        public static ART_WF_ARTWORK_PROCESS_RESULT SaveStepDurationExtend(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_RESULT Results = new ART_WF_ARTWORK_PROCESS_RESULT();
            ART_WF_ARTWORK_PROCESS process = new ART_WF_ARTWORK_PROCESS();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            process.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(process, context).FirstOrDefault();

                            if (process != null)
                            {
                                process.STEP_DURATION_EXTEND_REASON_ID = param.data.STEP_DURATION_EXTEND_REASON_ID;
                                process.STEP_DURATION_EXTEND_REMARK = param.data.STEP_DURATION_EXTEND_REMARK;
                                process.IS_STEP_DURATION_EXTEND = param.data.IS_STEP_DURATION_EXTEND;
                                ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(process, context);

                                // by aof #INC-37988 extend duration RD to QC start code.

                                var stepSEND_RD_ID = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE.Equals("SEND_RD")).Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
                                if (process.CURRENT_STEP_ID == stepSEND_RD_ID)
                                {
                                    var processQC = context.ART_WF_ARTWORK_PROCESS.Where(w => w.ARTWORK_SUB_ID == process.PARENT_ARTWORK_SUB_ID).ToList().FirstOrDefault();
                                    if (processQC != null)
                                    {

                                        var RD_STEP_DURATION_EXTEND_REASON_ID = process.STEP_DURATION_EXTEND_REASON_ID;
                                        var QC_STEP_DURATION_EXTEND_REASON_ID = 0;

                                        if (RD_STEP_DURATION_EXTEND_REASON_ID > 0)
                                        {
                                            var RD_STEP_DURATION_EXTEND_REASON = context.ART_M_DECISION_REASON.Where(w => w.ART_M_DECISION_REASON_ID == RD_STEP_DURATION_EXTEND_REASON_ID && w.IS_ACTIVE == "X").ToList().FirstOrDefault();
                                            if (RD_STEP_DURATION_EXTEND_REASON != null)
                                            {
                                                if (RD_STEP_DURATION_EXTEND_REASON.WF != "-")  // - is "no reason"
                                                {
                                                    var STEP_CODE_DURATION_EXTEND = "ARTWORK_SEND_QC".Trim().ToUpper();
                                                    var QC_STEP_DURATION_EXTEND_REASON = context.ART_M_DECISION_REASON
                                                                                            .Where(w => w.DESCRIPTION.Trim().ToLower() == RD_STEP_DURATION_EXTEND_REASON.DESCRIPTION.Trim().ToLower()
                                                                                            && w.STEP_CODE.Trim().ToUpper() == STEP_CODE_DURATION_EXTEND
                                                                                            && w.IS_ACTIVE == "X"
                                                                                             ).ToList().FirstOrDefault();
                                                    if (QC_STEP_DURATION_EXTEND_REASON == null)
                                                    {
                                                        QC_STEP_DURATION_EXTEND_REASON = new ART_M_DECISION_REASON();
                                                        QC_STEP_DURATION_EXTEND_REASON.WF = "A";
                                                        QC_STEP_DURATION_EXTEND_REASON.STEP_CODE = STEP_CODE_DURATION_EXTEND;
                                                        QC_STEP_DURATION_EXTEND_REASON.DESCRIPTION = RD_STEP_DURATION_EXTEND_REASON.DESCRIPTION;
                                                        QC_STEP_DURATION_EXTEND_REASON.IS_OVERDUE = RD_STEP_DURATION_EXTEND_REASON.IS_OVERDUE;
                                                        QC_STEP_DURATION_EXTEND_REASON.IS_DEFAULT = RD_STEP_DURATION_EXTEND_REASON.IS_DEFAULT;
                                                        QC_STEP_DURATION_EXTEND_REASON.IS_ACTIVE = RD_STEP_DURATION_EXTEND_REASON.IS_ACTIVE;
                                                        QC_STEP_DURATION_EXTEND_REASON.CREATE_BY = -6;  // auto save by system.
                                                        QC_STEP_DURATION_EXTEND_REASON.UPDATE_BY = -6;  // auto save by system.
                                                        ART_M_DECISION_REASON_SERVICE.SaveOrUpdate(QC_STEP_DURATION_EXTEND_REASON, context);
                                                    }
                                                    QC_STEP_DURATION_EXTEND_REASON_ID = QC_STEP_DURATION_EXTEND_REASON.ART_M_DECISION_REASON_ID;
                                                }
                                                else
                                                {
                                                    QC_STEP_DURATION_EXTEND_REASON_ID = RD_STEP_DURATION_EXTEND_REASON.ART_M_DECISION_REASON_ID;
                                                }
                                            }
                                        }
                                        
                                        processQC.STEP_DURATION_EXTEND_REASON_ID = QC_STEP_DURATION_EXTEND_REASON_ID;
                                        processQC.STEP_DURATION_EXTEND_REMARK = param.data.STEP_DURATION_EXTEND_REMARK;
                                        processQC.IS_STEP_DURATION_EXTEND = param.data.IS_STEP_DURATION_EXTEND;
                                        ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(processQC, context);
                                    }
                                }

                                // by aof #INC-37988 extend duration RD to QC end code.

                                Results.data = new List<ART_WF_ARTWORK_PROCESS_2>();
                                Results.data.Add(new ART_WF_ARTWORK_PROCESS_2 { IS_OVER_DUE = CheckOverDue(process, context) });
                            }
                        }
                        else
                        {
                            return Results;
                        }

                        dbContextTransaction.Commit();

                        //SendEail
                        var emailto = "";

                        var q = (from m in context.ART_WF_ARTWORK_PROCESS
                                 join m1 in context.ART_WF_ARTWORK_REQUEST on m.ARTWORK_REQUEST_ID equals m1.ARTWORK_REQUEST_ID
                                 join m2 in context.ART_WF_ARTWORK_REQUEST_ITEM on m.ARTWORK_ITEM_ID equals m2.ARTWORK_ITEM_ID
                                 join m3 in context.ART_M_USER on m.UPDATE_BY equals m3.USER_ID
                                 join m4 in context.ART_M_POSITION on m3.POSITION_ID equals m4.ART_M_POSITION_ID
                                 where m.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                 select new
                                 {
                                     REQUEST_ID = m.ARTWORK_REQUEST_ID,
                                     ARTWORK_SUB_ID = m.ARTWORK_SUB_ID,
                                     CREATE_DATE = m.CREATE_DATE,
                                     CREATE_BY = m1.CREATE_BY,
                                     REQUEST_ITEM_NO = m2.REQUEST_ITEM_NO,
                                     POSITION_NAME = m4.ART_M_POSITION_NAME
                                 }).FirstOrDefault();

                        var stepArtwork = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = param.data.STEPNAME }, context).FirstOrDefault();
                        if (q != null && stepArtwork != null)
                        {
                            DateTime dtReceiveWf = q.CREATE_DATE;
                            DateTime dtWillFinish = CNService.AddBusinessDays(dtReceiveWf, (int)Math.Ceiling(stepArtwork.DURATION_EXTEND.Value));

                            var userInfo = ART_M_USER_SERVICE.GetByUSER_ID(q.CREATE_BY, context);
                            if (userInfo.IS_ACTIVE == "X")
                            {
                                emailto = userInfo.EMAIL;
                            }

                            EmailService.SendEmailExtendDuration(emailto, q.REQUEST_ITEM_NO, dtWillFinish, q.POSITION_NAME, stepArtwork.STEP_ARTWORK_NAME, null, q.REQUEST_ID, q.ARTWORK_SUB_ID);
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
