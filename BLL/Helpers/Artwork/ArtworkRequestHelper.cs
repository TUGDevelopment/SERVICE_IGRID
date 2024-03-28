using BLL.DocumentManagement;
using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace BLL.Helpers
{
    public class ArtworkRequestHelper
    {
        public static ART_WF_ARTWORK_REQUEST_RESULT SubmitArtworkRequest(ART_WF_ARTWORK_REQUEST_REQUEST param, bool sendEmail)
        {
            int artworkRequestID = 0;
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();

            string request_no = "";
            string allArtwork_no = "";
            string oneArtwork_No = "";
            bool multiArtwork = false;
            bool commitSuccess = false;

            long nodeId = 0;
            long versionPDF = 1; //ticket#438889
            List<long> listNodeId = new List<long>();

            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }
                else
                {
                    artworkRequestID = param.data.ARTWORK_REQUEST_ID;
                    var listProcess = new List<ART_WF_ARTWORK_PROCESS>();

                    using (var context = new ARTWORKEntities())
                    {
                        using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            context.Database.CommandTimeout = 300;

                            var cntProcess = (from p in context.ART_WF_ARTWORK_PROCESS
                                              where p.ARTWORK_REQUEST_ID == artworkRequestID
                                              select p.ARTWORK_SUB_ID).Count();

                            if (cntProcess <= 0)
                            {
                                var soRepeat = (from q in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                                where q.ARTWORK_REQUEST_ID == artworkRequestID
                                                select q).ToList();

                                foreach (var item in soRepeat)
                                {
                                    var so = item.SALES_ORDER_NO;
                                    var soItem = item.SALES_ORDER_ITEM;
                                    var productCode = item.PRODUCT_CODE;
                                    var mat5 = item.COMPONENT_MATERIAL;

                                    var soDetail = (from q in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                                    join m in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT on q.BOM_ID equals m.PO_COMPLETE_SO_ITEM_COMPONENT_ID
                                                    where q.SALES_ORDER_NO == so
                                                    && q.SALES_ORDER_ITEM == soItem
                                                    && q.MATERIAL_NO == productCode
                                                    && m.COMPONENT_MATERIAL == mat5
                                                    select q).ToList();

                                    if (soDetail.Count > 0)
                                    {
                                        var ARTWORK_SUB_ID = soDetail.FirstOrDefault().ARTWORK_SUB_ID;

                                        var process = (from q in context.ART_WF_ARTWORK_PROCESS
                                                       where q.ARTWORK_SUB_ID == ARTWORK_SUB_ID
                                                       select q).FirstOrDefault();

                                        var wfNo = (from q in context.ART_WF_ARTWORK_REQUEST_ITEM
                                                    where q.ARTWORK_ITEM_ID == process.ARTWORK_ITEM_ID
                                                    select q.REQUEST_ITEM_NO).FirstOrDefault();

                                        Results.status = "E";
                                        Results.msg = "Cannot create workflow";
                                        Results.msg += "<br/>Sales order no : " + so;
                                        Results.msg += "<br/>Item : " + soItem;
                                        Results.msg += "<br/>Product code : " + productCode;
                                        Results.msg += "<br/>Material code : " + mat5;
                                        Results.msg += "<br/>Already assigned in workflow : " + wfNo;
                                        return Results;
                                    }
                                }

                            }
                            ART_WF_ARTWORK_REQUEST itemtemp = (from q in context.ART_WF_ARTWORK_REQUEST
                                                               where q.ARTWORK_REQUEST_ID == artworkRequestID
                                                               select q).FirstOrDefault();

                            //Start Ticket#455797 by Aof on 01/11/2020 --> this scope added to block other transaction. we need to finish this transaction as first. 
                            if (itemtemp.ARTWORK_REQUEST_ID > 0)
                            {
                                ART_WF_ARTWORK_REQUEST_SERVICE.SaveOrUpdateNoLog(itemtemp, context);
                            }
                            //last Ticket#455797 by Aof on 01/11/2020

                            if (itemtemp.REQUEST_FORM_CREATE_DATE == null)
                            {
                                itemtemp.REQUEST_FORM_CREATE_DATE = DateTime.Now;
                                ART_WF_ARTWORK_REQUEST_SERVICE.SaveOrUpdateNoLog(itemtemp, context);
                            }

                            if (param.data.PROCESS == null)
                            {
                                var artworkRequest = (from a in context.ART_WF_ARTWORK_REQUEST
                                                      where a.ARTWORK_REQUEST_ID == artworkRequestID
                                                      select a).FirstOrDefault();

                                if (artworkRequest != null)
                                {
                                    request_no = artworkRequest.ARTWORK_REQUEST_NO;
                                }

                                var artworkItems = (from a in context.ART_WF_ARTWORK_REQUEST_ITEM
                                                    where a.ARTWORK_REQUEST_ID == artworkRequestID
                                                    select a).ToList();

                                ART_WF_ARTWORK_PROCESS process = new ART_WF_ARTWORK_PROCESS();
                                ART_WF_ARTWORK_PROCESS_PA processPA = new ART_WF_ARTWORK_PROCESS_PA();
                                var token = CWSService.getAuthToken();
                                foreach (var item in artworkItems)
                                {
                                    if (string.IsNullOrEmpty(item.REQUEST_ITEM_NO))
                                    {
                                        process = new ART_WF_ARTWORK_PROCESS();
                                        string formNO = FormNumberHelper.GenArtworkTaskFormNo(context, item.ARTWORK_REQUEST_ID);
                                        string stepCode = "SEND_PA";

                                        //Create Folder in CS
                                        long folderID = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkNodeID"]);
                                        long templateID = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkTemplateNodeID"]);

                                        var nodeTemplate = CWSService.copyNode(formNO, templateID, folderID, token);
                                        process.ARTWORK_FOLDER_NODE_ID = nodeTemplate.ID;

                                        //Copy file Upload in CS
                                        string artworkFolder = ConfigurationManager.AppSettings["ArtworkFolderName"];
                                        string ArtworkFolderNamePrintMaster = ConfigurationManager.AppSettings["ArtworkFolderNamePrintMaster"];

                                        if (artworkRequest.TYPE_OF_ARTWORK != "NEW")
                                        {
                                            artworkFolder = ArtworkFolderNamePrintMaster;
                                        }

                                        var artworkFolderNode = CWSService.getNodeByName(nodeTemplate.ID, artworkFolder, token);

                                        long FileItemID = Convert.ToInt64(item.REQUEST_FORM_FILE_NODE_ID);

                                        if (artworkFolderNode != null)
                                        {
                                            var nodeFile = CWSService.copyNode(item.FILE_NAME, FileItemID, artworkFolderNode.ID, token);
                                            nodeId = nodeFile.ID;
                                            // start-ticket#438889
                                            if (nodeFile.VersionInfo != null)
                                            {
                                                versionPDF = nodeFile.VersionInfo.VersionNum;
                                            }
                                            // last-ticket#438889 
                                            listNodeId.Add(nodeId);
                                        }

                                        var step = (from s in context.ART_M_STEP_ARTWORK
                                                    where s.STEP_ARTWORK_CODE == stepCode
                                                    select s).FirstOrDefault();

                                        process.ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID;
                                        process.ARTWORK_ITEM_ID = item.ARTWORK_ITEM_ID;
                                        process.CURRENT_STEP_ID = step.STEP_ARTWORK_ID;
                                        process.CURRENT_ROLE_ID = step.ROLE_ID_RESPONSE; //null;
                                        process.CURRENT_USER_ID = CNService.CheckPICArtwork(context, artworkRequest);
                                        process.CREATE_BY = param.data.CREATE_BY;
                                        process.UPDATE_BY = param.data.CREATE_BY;

                                        var requestTmp = context.ART_WF_ARTWORK_REQUEST.Where(r => r.ARTWORK_REQUEST_ID == process.ARTWORK_REQUEST_ID).FirstOrDefault();

                                        var tempProcess = CNService.CheckDelegateBeforeRountingArtwork(process, context);

                                        listProcess.Add(process);

                                        item.REQUEST_ITEM_NO = formNO;
                                        ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.SaveOrUpdateNoLog(item, context);

                                        processPA = new ART_WF_ARTWORK_PROCESS_PA();
                                        processPA.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                                        processPA = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(processPA, context).FirstOrDefault();

                                        if (processPA != null)
                                        {
                                            processPA.PA_USER_ID = tempProcess.CURRENT_USER_ID;
                                            ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA, context);
                                        }
                                        else
                                        {
                                            processPA = new ART_WF_ARTWORK_PROCESS_PA();
                                            processPA.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                                            processPA.PA_USER_ID = tempProcess.CURRENT_USER_ID;
                                            processPA.CREATE_BY = process.UPDATE_BY;
                                            processPA.UPDATE_BY = process.UPDATE_BY;

                                            ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA, context);
                                        }

                                        if (requestTmp != null && !String.IsNullOrEmpty(requestTmp.TYPE_OF_ARTWORK))
                                        {
                                            if (requestTmp.TYPE_OF_ARTWORK == "REPEAT" && !CNService.IsFFC(requestTmp.CREATE_BY, context))
                                            {
                                                CopyFileToAttachment(context, item, process, "REPEAT", nodeId, versionPDF);  //ticket#438889

                                                ART_WF_ARTWORK_PROCESS_PA_REQUEST paReq = new ART_WF_ARTWORK_PROCESS_PA_REQUEST();
                                                ART_WF_ARTWORK_PROCESS_PA_2 padata = new ART_WF_ARTWORK_PROCESS_PA_2();
                                                padata.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                                                paReq.data = padata;
                                                PAFormHelper.RepeatSOUpdatePAData(paReq, context);

                                                Results.data = new List<ART_WF_ARTWORK_REQUEST_2>();
                                                Results.data.Add(new ART_WF_ARTWORK_REQUEST_2() { ARTWORK_SUB_ID = process.ARTWORK_SUB_ID });
                                            }
                                            else
                                            {
                                                CopyFileToAttachment(context, item, process, "NEW", nodeId, 1);  //ticket#438889
                                            }
                                        }

                                        if (allArtwork_no == "")
                                        {
                                            oneArtwork_No = formNO;
                                            allArtwork_no = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + formNO;
                                        }
                                        else
                                        {
                                            multiArtwork = true;
                                            allArtwork_no += "<br/>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + formNO;
                                        }
                                    }
                                    else
                                    {
                                        if (allArtwork_no == "")
                                        {
                                            oneArtwork_No = item.REQUEST_ITEM_NO;
                                            allArtwork_no = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; : " + item.REQUEST_ITEM_NO;
                                        }
                                        else
                                        {
                                            multiArtwork = true;
                                            allArtwork_no += "<br/>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; : " + item.REQUEST_ITEM_NO;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (param.data.PROCESS.ENDTASKFORM)
                                {
                                    ArtworkProcessHelper.EndTaskForm(param.data.PROCESS.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                                }
                            }

                            dbContextTransaction.Commit();
                            commitSuccess = true;

                            if (sendEmail)
                            {
                                if (param.data.PROCESS == null)
                                {
                                    foreach (var item in listProcess)
                                        EmailService.sendEmailArtwork(item.ARTWORK_REQUEST_ID, item.ARTWORK_SUB_ID, "WF_SEND_TO", context);
                                }
                            }
                        }
                    }

                    Results.status = "S";

                    if (param.data.PROCESS == null)
                    {
                        if (!String.IsNullOrEmpty(request_no))
                        {
                            Results.msg += "Request form no. : " + request_no + "<br/>";
                        }

                        if (multiArtwork)
                        {
                            Results.msg += "Artwork request no. : " + "<br/>";
                            Results.msg += allArtwork_no;
                        }
                        else
                        {
                            Results.msg += "Artwork request no. : " + oneArtwork_No;
                        }
                    }
                    else
                    {
                        Results.msg = MessageHelper.GetMessage("MSG_001");
                    }
                }

                try
                {
                    UpdateMaterialLock(param);
                }
                catch (Exception ex) { CNService.GetErrorMessage(ex); }
            }
            catch (Exception ex)
            {
                if (commitSuccess == false)
                {
                    var token = CWSService.getAuthToken();
                    foreach (var s in listNodeId)
                    {
                        CWSService.deleteNode(s, token);
                    }
                }

                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_REQUEST_RESULT SubmitArtworkRequestForRepeat(ART_WF_ARTWORK_REQUEST_REQUEST param, bool sendEmail, ARTWORKEntities context)
        {
            int artworkRequestID = 0;
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();

            string request_no = "";
            string allArtwork_no = "";
            string oneArtwork_No = "";
            bool multiArtwork = false;
            bool commitSuccess = false;

            long nodeId = 0;
            long versionPDF = 1; //ticket#438889
            List<long> listNodeId = new List<long>();
            List<long> listAWNodeId = new List<long>();
            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }
                else
                {
                    artworkRequestID = param.data.ARTWORK_REQUEST_ID;
                    var listProcess = new List<ART_WF_ARTWORK_PROCESS>();

                    //using (var context = new ARTWORKEntities())
                    {
                        //using (var dbContextTransaction = context.Database.BeginTransaction())
                        {
                            //context.Database.CommandTimeout = 300;

                            var soRepeat = (from q in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                            where q.ARTWORK_REQUEST_ID == artworkRequestID
                                            select q).ToList();
                            var recheck = "";
                            foreach (var item in soRepeat)
                            {
                                var so = item.SALES_ORDER_NO;
                                var soItem = item.SALES_ORDER_ITEM;
                                var productCode = item.PRODUCT_CODE;
                                var mat5 = item.COMPONENT_MATERIAL;
                                var soDetail = (from q in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                                join m in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT on q.BOM_ID equals m.PO_COMPLETE_SO_ITEM_COMPONENT_ID
                                                where q.SALES_ORDER_NO == so
                                                && q.SALES_ORDER_ITEM == soItem
                                                && q.MATERIAL_NO == productCode
                                                && m.COMPONENT_MATERIAL == mat5
                                                select q).ToList();

                                if (soDetail.Count > 0)
                                {
                                    var ARTWORK_SUB_ID = soDetail.FirstOrDefault().ARTWORK_SUB_ID;

                                    var process = (from q in context.ART_WF_ARTWORK_PROCESS
                                                   where q.ARTWORK_SUB_ID == ARTWORK_SUB_ID
                                                   select q).FirstOrDefault();

                                    var wfNo = (from q in context.ART_WF_ARTWORK_REQUEST_ITEM
                                                where q.ARTWORK_ITEM_ID == process.ARTWORK_ITEM_ID
                                                select q.REQUEST_ITEM_NO).FirstOrDefault();

                                    Results.status = "E";
                                    Results.msg = "Cannot create workflow";
                                    Results.msg += "<br/>Sales order no : " + so;
                                    Results.msg += "<br/>Item : " + soItem;
                                    Results.msg += "<br/>Product code : " + productCode;
                                    Results.msg += "<br/>Material code : " + mat5;
                                    Results.msg += "<br/>Already assigned in workflow : " + wfNo;
                                    return Results;
                                }
                                recheck = item.RECHECK_ARTWORK;
                            }

                            ART_WF_ARTWORK_REQUEST itemtemp = (from q in context.ART_WF_ARTWORK_REQUEST
                                                               where q.ARTWORK_REQUEST_ID == artworkRequestID
                                                               select q).FirstOrDefault();

                            if (itemtemp.REQUEST_FORM_CREATE_DATE == null)
                            {
                                itemtemp.REQUEST_FORM_CREATE_DATE = DateTime.Now;
                                ART_WF_ARTWORK_REQUEST_SERVICE.SaveOrUpdateNoLog(itemtemp, context);
                            }

                            if (param.data.PROCESS == null)
                            {
                                var artworkRequest = (from a in context.ART_WF_ARTWORK_REQUEST
                                                      where a.ARTWORK_REQUEST_ID == artworkRequestID
                                                      select a).FirstOrDefault();

                                if (artworkRequest != null)
                                {
                                    request_no = artworkRequest.ARTWORK_REQUEST_NO;
                                }

                                var artworkItems = (from a in context.ART_WF_ARTWORK_REQUEST_ITEM
                                                    where a.ARTWORK_REQUEST_ID == artworkRequestID
                                                    select a).ToList();

                                ART_WF_ARTWORK_PROCESS process = new ART_WF_ARTWORK_PROCESS();
                                ART_WF_ARTWORK_PROCESS_PA processPA = new ART_WF_ARTWORK_PROCESS_PA();
                                var token = CWSService.getAuthToken();
                                foreach (var item in artworkItems)
                                {
                                    if (string.IsNullOrEmpty(item.REQUEST_ITEM_NO))
                                    {
                                        process = new ART_WF_ARTWORK_PROCESS();

                                        string requestType = recheck == "Yes" ? "ARTWORK_REPEAT_OVER_6_MONTH" : "ARTWORK_REPEAT";
                                        string formNO = FormNumberHelper.GenArtworkRepeat(context, item.ARTWORK_REQUEST_ID, requestType);
                                        ///string formNO = FormNumberHelper.GenArtworkTaskFormNo(context, item.ARTWORK_REQUEST_ID);
                                        string stepCode = "SEND_PA";

                                        //Create Folder in CS
                                        long folderID = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkNodeID"]);
                                        long templateID = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkTemplateNodeID"]);



                                        var nodeTemplate = CWSService.copyNodeAndDeleteNodeIsExist(formNO, templateID, folderID, token);// ticket 459570 by aof //  var nodeTemplate = CWSService.copyNode(formNO, templateID, folderID, token);
                                        process.ARTWORK_FOLDER_NODE_ID = nodeTemplate.ID;
                                        listAWNodeId.Add(nodeTemplate.ID);

                                        //Copy file Upload in CS
                                        string artworkFolder = ConfigurationManager.AppSettings["ArtworkFolderName"];
                                        string ArtworkFolderNamePrintMaster = ConfigurationManager.AppSettings["ArtworkFolderNamePrintMaster"];

                                        if (artworkRequest.TYPE_OF_ARTWORK != "NEW")
                                        {
                                            artworkFolder = ArtworkFolderNamePrintMaster;
                                        }

                                        var artworkFolderNode = CWSService.getNodeByName(nodeTemplate.ID, artworkFolder, token);

                                        long FileItemID = Convert.ToInt64(item.REQUEST_FORM_FILE_NODE_ID);

                                        if (artworkFolderNode != null)
                                        {
                                            var nodeFile = CWSService.copyNode(item.FILE_NAME, FileItemID, artworkFolderNode.ID, token);
                                            nodeId = nodeFile.ID;
                                            // start-ticket#438889
                                            if (nodeFile.VersionInfo != null)
                                            {
                                                versionPDF = nodeFile.VersionInfo.VersionNum;
                                            }
                                            // last-ticket#438889 
                                            listNodeId.Add(nodeId);
                                        }

                                        var step = (from s in context.ART_M_STEP_ARTWORK
                                                    where s.STEP_ARTWORK_CODE == stepCode
                                                    select s).FirstOrDefault();

                                        process.ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID;
                                        process.ARTWORK_ITEM_ID = item.ARTWORK_ITEM_ID;
                                        process.CURRENT_STEP_ID = step.STEP_ARTWORK_ID;
                                        process.CURRENT_ROLE_ID = step.ROLE_ID_RESPONSE; //null;
                                        process.CURRENT_USER_ID = CNService.CheckPICArtwork(context, artworkRequest);
                                        process.CREATE_BY = param.data.CREATE_BY;
                                        process.UPDATE_BY = param.data.CREATE_BY;

                                        var requestTmp = context.ART_WF_ARTWORK_REQUEST.Where(r => r.ARTWORK_REQUEST_ID == process.ARTWORK_REQUEST_ID).FirstOrDefault();

                                        var tempProcess = CNService.CheckDelegateBeforeRountingArtwork(process, context);

                                        listProcess.Add(process);

                                        item.REQUEST_ITEM_NO = formNO;
                                        ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.SaveOrUpdateNoLog(item, context);

                                        processPA = new ART_WF_ARTWORK_PROCESS_PA();
                                        processPA.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                                        processPA = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(processPA, context).FirstOrDefault();

                                        if (processPA != null)
                                        {
                                            processPA.PA_USER_ID = tempProcess.CURRENT_USER_ID;
                                            ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA, context);
                                        }
                                        else
                                        {
                                            processPA = new ART_WF_ARTWORK_PROCESS_PA();
                                            processPA.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                                            processPA.PA_USER_ID = tempProcess.CURRENT_USER_ID;
                                            processPA.CREATE_BY = process.UPDATE_BY;
                                            processPA.UPDATE_BY = process.UPDATE_BY;

                                            ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA, context);
                                        }

                                        if (requestTmp != null && !String.IsNullOrEmpty(requestTmp.TYPE_OF_ARTWORK))
                                        {
                                            if (requestTmp.TYPE_OF_ARTWORK == "REPEAT" && !CNService.IsFFC(requestTmp.CREATE_BY, context))
                                            {
                                                CopyFileToAttachment(context, item, process, "REPEAT", nodeId, versionPDF);   //ticket#438889

                                                ART_WF_ARTWORK_PROCESS_PA_REQUEST paReq = new ART_WF_ARTWORK_PROCESS_PA_REQUEST();
                                                ART_WF_ARTWORK_PROCESS_PA_2 padata = new ART_WF_ARTWORK_PROCESS_PA_2();
                                                padata.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                                                paReq.data = padata;
                                                PAFormHelper.RepeatSOUpdatePAData(paReq, context);

                                                Results.data = new List<ART_WF_ARTWORK_REQUEST_2>();
                                                Results.data.Add(new ART_WF_ARTWORK_REQUEST_2() { ARTWORK_SUB_ID = process.ARTWORK_SUB_ID });
                                            }
                                            else
                                            {
                                                CopyFileToAttachment(context, item, process, "NEW", nodeId, 1);  //ticket#438889
                                            }
                                        }

                                        if (allArtwork_no == "")
                                        {
                                            oneArtwork_No = formNO;
                                            allArtwork_no = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + formNO;
                                        }
                                        else
                                        {
                                            multiArtwork = true;
                                            allArtwork_no += "<br/>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + formNO;
                                        }
                                    }
                                    else
                                    {
                                        if (allArtwork_no == "")
                                        {
                                            oneArtwork_No = item.REQUEST_ITEM_NO;
                                            allArtwork_no = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; : " + item.REQUEST_ITEM_NO;
                                        }
                                        else
                                        {
                                            multiArtwork = true;
                                            allArtwork_no += "<br/>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; : " + item.REQUEST_ITEM_NO;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (param.data.PROCESS.ENDTASKFORM)
                                {
                                    ArtworkProcessHelper.EndTaskForm(param.data.PROCESS.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                                }
                            }

                            //dbContextTransaction.Commit();
                            commitSuccess = true;

                            if (sendEmail)
                            {
                                if (param.data.PROCESS == null)
                                {
                                    foreach (var item in listProcess)
                                        EmailService.sendEmailArtwork(item.ARTWORK_REQUEST_ID, item.ARTWORK_SUB_ID, "WF_SEND_TO", context);
                                }
                            }
                        }
                    }

                    Results.listAWNodeId = listAWNodeId;
                    Results.status = "S";

                    if (param.data.PROCESS == null)
                    {
                        if (!String.IsNullOrEmpty(request_no))
                        {
                            Results.msg += "Request form no. : " + request_no + "<br/>";
                        }

                        if (multiArtwork)
                        {
                            Results.msg += "Artwork request no. : " + "<br/>";
                            Results.msg += allArtwork_no;
                        }
                        else
                        {
                            Results.msg += "Artwork request no. : " + oneArtwork_No;
                        }
                    }
                    else
                    {
                        Results.msg = MessageHelper.GetMessage("MSG_001");
                    }
                }

                try
                {
                    UpdateMaterialLock(param);
                }
                catch (Exception ex) { CNService.GetErrorMessage(ex); }
            }
            catch (Exception ex)
            {
                if (commitSuccess == false)
                {
                    var token = CWSService.getAuthToken();
                    foreach (var s in listNodeId)
                    {
                        CWSService.deleteNode(s, token);
                    }
                }

                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        private static void UpdateMaterialLock(ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            var ARTWORK_SUB_ID = 0;
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var subIDs = (from q in context.ART_WF_ARTWORK_PROCESS
                                  where q.ARTWORK_REQUEST_ID == param.data.ARTWORK_REQUEST_ID
                                  select q.ARTWORK_SUB_ID).ToList();
                    if (subIDs.Count > 0)
                    {
                        var subPAs = (from q in context.ART_WF_ARTWORK_PROCESS_PA
                                      where subIDs.Contains(q.ARTWORK_SUB_ID)
                                      select q).FirstOrDefault();
                        if (subPAs != null)
                        {
                            ARTWORK_SUB_ID = subPAs.ARTWORK_SUB_ID;
                        }
                    }
                }
            }

            if (ARTWORK_SUB_ID > 0)
            {
                CNService.UpdateMaterialLock(ARTWORK_SUB_ID);
            }
        }

        private static void CopyFileToAttachment(ARTWORKEntities context, ART_WF_ARTWORK_REQUEST_ITEM item, ART_WF_ARTWORK_PROCESS process, string type, long nodeID, long versionPDF = 1)
        {
            var itemTmp = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(item.ARTWORK_ITEM_ID, context);
            var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "UPLOAD_AW" }, context).FirstOrDefault().STEP_ARTWORK_ID;

            if (itemTmp != null)
            {
                ART_WF_ARTWORK_ATTACHMENT attach = new ART_WF_ARTWORK_ATTACHMENT();

                attach.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                attach.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                attach.CONTENT_TYPE = itemTmp.CONTENT_TYPE;
                attach.CREATE_BY = itemTmp.CREATE_BY;
                attach.UPDATE_BY = itemTmp.CREATE_BY;
                attach.EXTENSION = itemTmp.EXTENSION;
                attach.FILE_NAME = itemTmp.FILE_NAME;
                attach.IS_CUSTOMER = "X";
                attach.IS_INTERNAL = "X";
                attach.IS_VENDOR = "X";
                attach.NODE_ID = nodeID;
                attach.SIZE = Convert.ToInt64(itemTmp.FILE_SIZE);
                attach.STEP_ARTWORK_ID = stepId;
                attach.VERSION = versionPDF; //attach.VERSION = 1; ticket#438889
                attach.VERSION2 = "1.0";

                ART_WF_ARTWORK_ATTACHMENT_SERVICE.SaveOrUpdateNoLog(attach, context);
            }
        }

        public static ART_WF_ARTWORK_REQUEST_ITEM_RESULT DeleteFileArtworkRequest(ART_WF_ARTWORK_REQUEST_ITEM_REQUEST param)
        {
            ART_WF_ARTWORK_REQUEST_ITEM_RESULT Results = new ART_WF_ARTWORK_REQUEST_ITEM_RESULT();

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
                            ART_WF_ARTWORK_REQUEST_ITEM item = new ART_WF_ARTWORK_REQUEST_ITEM();
                            item.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;
                            item.REQUEST_FORM_FILE_NODE_ID = param.data.REQUEST_FORM_FILE_NODE_ID;
                            item = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(item, context).FirstOrDefault();

                            if (item != null)
                            {
                                ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.DeleteByARTWORK_ITEM_ID(item.ARTWORK_ITEM_ID, context);
                                var token = CWSService.getAuthToken();
                                CWSService.deleteNode(item.REQUEST_FORM_FILE_NODE_ID, token);
                            }

                            dbContextTransaction.Commit();
                        }
                    }

                    Results.status = "S";
                    Results.msg = MessageHelper.GetMessage("MSG_001");

                }
            }
            catch (Exception ex)
            {

                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_REQUEST_RESULT DeleteArtworkRequest(ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();

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
                            ArtworkUploadHelper.DeleteArtworkRequestOperation(param, context);
                            dbContextTransaction.Commit();
                        }
                    }

                    Results.status = "S";
                    Results.msg = MessageHelper.GetMessage("MSG_001");
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static SALES_ORDER_REQUEST_FORM_RESULT CheckSalesOrderRequest(SALES_ORDER_REQUEST_FORM_REQUEST_LIST param)
        {
            List<SALES_ORDER_REQUEST_FORM> listData = new List<SALES_ORDER_REQUEST_FORM>();
            List<ART_WF_ARTWORK_REQUEST_COUNTRY_2> listCountry = new List<ART_WF_ARTWORK_REQUEST_COUNTRY_2>();
            List<XECM_M_PRODUCT_2> listProduct = new List<XECM_M_PRODUCT_2>();
            List<int> listCountryID = new List<int>();
            List<int> listProductID = new List<int>();

            SALES_ORDER_REQUEST_FORM_RESULT Results = new SALES_ORDER_REQUEST_FORM_RESULT();
            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }
                else
                {
                    int row = 0;
                    var strMessage = "MSG_001";
                    var strMessageDetail = "";  //by aof
                    if (param.data.Count > 0)
                    {
                        using (var context = new ARTWORKEntities())
                        {
                            using (CNService.IsolationLevel(context))
                            {
                                SAP_M_PO_COMPLETE_SO_HEADER soHeader = new SAP_M_PO_COMPLETE_SO_HEADER();
                                SAP_M_PO_COMPLETE_SO_ITEM soItem = new SAP_M_PO_COMPLETE_SO_ITEM();
                                ART_WF_ARTWORK_REQUEST req = new ART_WF_ARTWORK_REQUEST();
                                bool is_repeat = false;
                                foreach (SALES_ORDER_REQUEST_FORM iSalesOrder in param.data)
                                {
                                    row++;
                                    if (row == 1)
                                    {
                                        req = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(iSalesOrder.ARTWORK_REQUEST_ID, context);
                                        if (req.TYPE_OF_ARTWORK == "REPEAT")
                                        {
                                            is_repeat = true;
                                        }
                                    }

                                    soHeader = new SAP_M_PO_COMPLETE_SO_HEADER();
                                    soHeader.SALES_ORDER_NO = iSalesOrder.SALES_ORDER_NO;
                                    soHeader = SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.GetByItem(soHeader, context).FirstOrDefault();
                                    if (soHeader != null)
                                    {
                                        //REVIEWER_ID
                                        if (soHeader.MARKETING_NAME != null && soHeader.MARKETING_NAME != "")
                                        {
                                            string[] arrMarketingName = soHeader.MARKETING_NAME.Split(' ');
                                            if (arrMarketingName.Length > 1)
                                            {
                                                ART_M_USER userReviewer = new ART_M_USER();
                                                userReviewer.FIRST_NAME = arrMarketingName[0];
                                                userReviewer.LAST_NAME = arrMarketingName[1];

                                                var reviewer = ART_M_USER_SERVICE.GetByItem(userReviewer, context).FirstOrDefault();
                                                if (reviewer != null)
                                                {
                                                    iSalesOrder.REVIEWER_ID = reviewer.USER_ID;
                                                    iSalesOrder.REVIEWER_DISPLAY_TXT = reviewer.TITLE + ' ' + reviewer.FIRST_NAME + ' ' + reviewer.LAST_NAME;
                                                }
                                            }
                                        }

                                        //SOLD_TO
                                        if (soHeader.SOLD_TO != null && soHeader.SOLD_TO != "")
                                        {
                                            var customerSoldTo = XECM_M_CUSTOMER_SERVICE.GetByItem(new XECM_M_CUSTOMER() { CUSTOMER_CODE = soHeader.SOLD_TO }, context).FirstOrDefault();
                                            if (customerSoldTo != null)
                                            {
                                                iSalesOrder.SOLD_TO_ID = customerSoldTo.CUSTOMER_ID;
                                                iSalesOrder.SOLD_TO_DISPLAY_TXT = customerSoldTo.CUSTOMER_CODE + ':' + customerSoldTo.CUSTOMER_NAME;
                                            }
                                        }

                                        //SHIP_TO
                                        if (soHeader.SHIP_TO != null && soHeader.SHIP_TO != "")
                                        {
                                            var customerShipTo = XECM_M_CUSTOMER_SERVICE.GetByItem(new XECM_M_CUSTOMER() { CUSTOMER_CODE = soHeader.SHIP_TO }, context).FirstOrDefault();
                                            if (customerShipTo != null)
                                            {
                                                iSalesOrder.SHIP_TO_ID = customerShipTo.CUSTOMER_ID;
                                                iSalesOrder.SHIP_TO_DISPLAY_TXT = customerShipTo.CUSTOMER_CODE + ':' + customerShipTo.CUSTOMER_NAME;
                                            }
                                        }

                                        //RDD
                                        if (soHeader.RDD != null)
                                        {
                                            iSalesOrder.REQUEST_DELIVERY_DATE = soHeader.RDD;
                                        }

                                        List<string> list_brand = new List<string>();
                                        List<string> soItemRequest = new List<string>();
                                        List<decimal?> items = new List<decimal?>();

                                        if (is_repeat)
                                        {
                                            soItemRequest = (from s in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                                             where s.ARTWORK_REQUEST_ID == iSalesOrder.ARTWORK_REQUEST_ID
                                                             && s.SALES_ORDER_NO == iSalesOrder.SALES_ORDER_NO
                                                             select s.SALES_ORDER_ITEM).ToList();

                                            if (soItemRequest != null)
                                            {
                                                foreach (var item in soItemRequest)
                                                {
                                                    items.Add(Convert.ToDecimal(item));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var soItem2 = (from s in context.V_SAP_SALES_ORDER
                                                           where s.SALES_ORDER_NO == iSalesOrder.SALES_ORDER_NO
                                                           select s.ITEM).ToList();
                                            if (soItem2 != null)
                                            {
                                                items = soItem2;
                                            }
                                        }

                                        var soItem_Tmp = context.SAP_M_PO_COMPLETE_SO_ITEM.Where(i => i.PO_COMPLETE_SO_HEADER_ID == soHeader.PO_COMPLETE_SO_HEADER_ID && items.Contains(i.ITEM)).ToList();
                                        if (soItem_Tmp != null && soItem_Tmp.Count > 0)
                                        {
                                            foreach (var itemSO in soItem_Tmp)
                                            {
                                                if (itemSO.COUNTRY != null && itemSO.COUNTRY != "")
                                                {
                                                    var countrySO = SAP_M_COUNTRY_SERVICE.GetByItem(new SAP_M_COUNTRY() { COUNTRY_CODE = itemSO.COUNTRY }, context).FirstOrDefault();
                                                    if (countrySO != null)
                                                    {
                                                        ART_WF_ARTWORK_REQUEST_COUNTRY_2 country = new ART_WF_ARTWORK_REQUEST_COUNTRY_2();
                                                        country.COUNTRY_ID = countrySO.COUNTRY_ID;
                                                        country.COUNTRY_DISPLAY_TXT = countrySO.COUNTRY_CODE + ':' + countrySO.NAME;
                                                        if (iSalesOrder.COUNTRY == null)
                                                        {
                                                            iSalesOrder.COUNTRY = new List<ART_WF_ARTWORK_REQUEST_COUNTRY_2>();
                                                        }
                                                        if (!iSalesOrder.COUNTRY.Exists(x => x.COUNTRY_ID.Equals(country.COUNTRY_ID)))
                                                        {
                                                            iSalesOrder.COUNTRY.Add(country);
                                                        }
                                                    }
                                                }

                                                //if (itemSO.PRODUCTION_PLANT != null && itemSO.PRODUCTION_PLANT != "")
                                                //{
                                                //    var plantSO = SAP_M_PLANT_SERVICE.GetByItem(new SAP_M_PLANT() { PLANT = itemSO.PRODUCTION_PLANT }, context).FirstOrDefault();
                                                //    if (plantSO != null)
                                                //    {
                                                //        ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2 plant = new ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2();
                                                //        plant.PRODUCTION_PLANT_ID = plantSO.PLANT_ID;
                                                //        plant.PRODUCTION_PLANT_DISPLAY_TXT = plantSO.PLANT + ':' + plantSO.NAME;
                                                //        if (iSalesOrder.PRODUCTION_PLANT == null)
                                                //        {
                                                //            iSalesOrder.PRODUCTION_PLANT = new List<ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2>();
                                                //        }
                                                //        if (!iSalesOrder.PRODUCTION_PLANT.Exists(x => x.PRODUCTION_PLANT_ID.Equals(plant.PRODUCTION_PLANT_ID)))
                                                //        {
                                                //            iSalesOrder.PRODUCTION_PLANT.Add(plant);
                                                //        }
                                                //    }
                                                //}

                                                if (itemSO.BRAND_ID != null && itemSO.BRAND_ID != "")
                                                {
                                                    if (list_brand.Count > 0)
                                                    {
                                                        if (!list_brand.Exists(x => x.Equals(itemSO.BRAND_ID)))
                                                        {
                                                            strMessage = ""; //Sales orders are inconsistency
                                                        }
                                                    }
                                                    else
                                                    {
                                                        list_brand.Add(itemSO.BRAND_ID);
                                                        var brandSO = SAP_M_BRAND_SERVICE.GetByItem(new SAP_M_BRAND() { MATERIAL_GROUP = itemSO.BRAND_ID }, context).FirstOrDefault();
                                                        if (brandSO != null)
                                                        {
                                                            if (iSalesOrder.BRAND_ID == null)
                                                            {
                                                                iSalesOrder.BRAND_ID = brandSO.BRAND_ID;
                                                                iSalesOrder.BRAND_DISPLAY_TXT = brandSO.MATERIAL_GROUP + ':' + brandSO.DESCRIPTION;
                                                            }
                                                        }
                                                    }
                                                }


                                                if (itemSO.IN_TRANSIT_TO != null && itemSO.IN_TRANSIT_TO != "")
                                                {
                                                    if (iSalesOrder.IN_TRANSIT == null)
                                                    {
                                                        iSalesOrder.IN_TRANSIT = itemSO.IN_TRANSIT_TO;
                                                    }
                                                }

                                                if (itemSO.VIA != null && itemSO.VIA != "")
                                                {
                                                    if (iSalesOrder.VIA == null)
                                                    {
                                                        iSalesOrder.VIA = itemSO.VIA;
                                                    }
                                                }

                                                if (itemSO.PRODUCT_CODE != null && itemSO.PRODUCT_CODE != "")
                                                {
                                                    if (itemSO.PRODUCT_CODE.StartsWith("3"))
                                                    {
                                                        var componentSO = SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_SERVICE.GetByItem(new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT() { PO_COMPLETE_SO_ITEM_ID = itemSO.PO_COMPLETE_SO_ITEM_ID }, context);
                                                        if (componentSO.Count > 0)
                                                        {
                                                            var isNewBomCustom = false;
                                                            if (is_repeat)
                                                            {
                                                                isNewBomCustom = true;
                                                            }
                                                            else
                                                            {
                                                                var checknewbom = componentSO.Where(m => m.BOM_ITEM_CUSTOM_1.StartsWith("NEW")).ToList();
                                                                if (checknewbom.Count > 0)
                                                                {
                                                                    isNewBomCustom = true;
                                                                }
                                                            }
                                                            //foreach (var itemComponentSO in componentSO)
                                                            //{
                                                            //    if (is_repeat)
                                                            //    {
                                                            //        isNewBomCustom = true;
                                                            //    }
                                                            //    else if (itemComponentSO.BOM_ITEM_CUSTOM_1 != null)
                                                            //    {
                                                            //        if (itemComponentSO.BOM_ITEM_CUSTOM_1.StartsWith("NEW"))
                                                            //        {
                                                            //            isNewBomCustom = true;
                                                            //        }
                                                            //    }
                                                            //}

                                                            if (isNewBomCustom)
                                                            {
                                                                if (itemSO.PRODUCTION_PLANT != null && itemSO.PRODUCTION_PLANT != "")
                                                                {
                                                                    var plantSO = SAP_M_PLANT_SERVICE.GetByItem(new SAP_M_PLANT() { PLANT = itemSO.PRODUCTION_PLANT }, context).FirstOrDefault();
                                                                    if (plantSO != null)
                                                                    {
                                                                        ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2 plant = new ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2();
                                                                        plant.PRODUCTION_PLANT_ID = plantSO.PLANT_ID;
                                                                        plant.PRODUCTION_PLANT_DISPLAY_TXT = plantSO.PLANT + ':' + plantSO.NAME;
                                                                        if (iSalesOrder.PRODUCTION_PLANT == null)
                                                                        {
                                                                            iSalesOrder.PRODUCTION_PLANT = new List<ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2>();
                                                                        }
                                                                        if (!iSalesOrder.PRODUCTION_PLANT.Exists(x => x.PRODUCTION_PLANT_ID.Equals(plant.PRODUCTION_PLANT_ID)))
                                                                        {
                                                                            iSalesOrder.PRODUCTION_PLANT.Add(plant);
                                                                        }
                                                                    }
                                                                }
                                                                var product = XECM_M_PRODUCT_SERVICE.GetByItem(new XECM_M_PRODUCT() { PRODUCT_CODE = itemSO.PRODUCT_CODE }, context).FirstOrDefault();
                                                                if (itemSO.PRODUCT_CODE.Substring(1, 1) != "E" && param.data[0].PRODUCT_TYPE == "FFC")
                                                                {
                                                                    strMessageDetail = strMessageDetail + " - " + iSalesOrder.SALES_ORDER_NO + ", PRODUCT_CODE " + itemSO.PRODUCT_CODE + " SO item has no BOM component.<br>";  // by aof
                                                                }
                                                                else if (product != null)
                                                                {
                                                                    XECM_M_PRODUCT_2 product2 = new XECM_M_PRODUCT_2();
                                                                    product2 = MapperServices.XECM_M_PRODUCT(product);
                                                                    product2.PRODUCT_CODE_ID = product.XECM_PRODUCT_ID;

                                                                    var checktype = param.data[0].PRODUCTION_PLANT.Where(m => m.PRODUCTION_PLANT_ID.Equals(3)).FirstOrDefault();
                                                                    if (checktype != null)
                                                                        product2.PRODUCT_TYPE = CNService.Getcheck_product_vap(product.PRODUCT_CODE, checktype.PRODUCTION_PLANT_ID.ToString());

                                                                    else
                                                                        product2.PRODUCT_TYPE = "";


                                                                    if (iSalesOrder.PRODUCT == null)
                                                                    {
                                                                        iSalesOrder.PRODUCT = new List<XECM_M_PRODUCT_2>();
                                                                    }

                                                                    if (!iSalesOrder.PRODUCT.Exists(x => x.PRODUCT_CODE_ID.Equals(product2.PRODUCT_CODE_ID)))
                                                                    {
                                                                        iSalesOrder.PRODUCT.Add(product2);
                                                                        listProduct.Add(product2);
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                strMessageDetail = strMessageDetail + " - " + iSalesOrder.SALES_ORDER_NO + ", PRODUCT_CODE " + itemSO.PRODUCT_CODE + " SO item has no BOM component.<br>";  // by aof
                                                            }
                                                        }
                                                        else
                                                        {
                                                            strMessageDetail = strMessageDetail + "-" + iSalesOrder.SALES_ORDER_NO + ", PRODUCT_CODE " + itemSO.PRODUCT_CODE + " SO item has no BOM component.<br>";  // by aof
                                                        }
                                                    }
                                                    else
                                                    {
                                                        strMessageDetail = strMessageDetail + " - " + iSalesOrder.SALES_ORDER_NO + ", PRODUCT_CODE " + itemSO.PRODUCT_CODE + " SO item has no product code or product code is not 3*.<br>";  // by aof
                                                    }
                                                }
                                                else
                                                {
                                                    strMessageDetail = strMessageDetail + " - " + iSalesOrder.SALES_ORDER_NO + ", SO item has no product code or product code is not 3*.<br>";  // by aof
                                                }
                                            } //end for soitem


                                        }
                                        else
                                        {
                                            strMessageDetail = strMessageDetail + " - " + iSalesOrder.SALES_ORDER_NO + ", No available SO item found.<br>";  // by aof
                                        }
                                    }
                                    else
                                    {
                                        strMessageDetail = strMessageDetail + " - "+ iSalesOrder.SALES_ORDER_NO + ", SO data is not found.<br>";  // by aof
                                    }
                                    if (listProduct.Count == 0)
                                    {
                                        Results.status = "E";
                                        Results.msg = "Sales order is mismatch.";
                                        if (!string.IsNullOrEmpty(strMessageDetail))
                                        {
                                            Results.msg = Results.msg + "<br>" + strMessageDetail;  // by aof
                                        }
                                        return Results;
                                    }
                                    strMessage = "";
                                    if (listData.Count <= 0) //first salesorder
                                    {
                                        listData = new List<SALES_ORDER_REQUEST_FORM> { iSalesOrder };
                                        if (iSalesOrder.COUNTRY != null)
                                        {
                                            listCountry.Add(iSalesOrder.COUNTRY[0]);
                                            listCountryID.Add(iSalesOrder.COUNTRY[0].COUNTRY_ID);
                                        }
                                        strMessage = "MSG_001";
                                    }
                                    else if (listData.Exists(x => x.SOLD_TO_ID.Equals(iSalesOrder.SOLD_TO_ID) && x.SHIP_TO_ID.Equals(iSalesOrder.SHIP_TO_ID) && x.BRAND_ID.Equals(iSalesOrder.BRAND_ID)))
                                    {
                                        var itemData = listData[0];

                                        if (iSalesOrder.COUNTRY != null)
                                        {
                                            foreach (var iCountry in iSalesOrder.COUNTRY)
                                            {
                                                if (!itemData.COUNTRY.Exists(x => x.COUNTRY_ID.Equals(iCountry.COUNTRY_ID)))
                                                {
                                                    strMessage = "MSG_001"; //Sales orders are not inconsistency
                                                }
                                            }
                                        }

                                        if (is_repeat)
                                        {
                                            strMessage = "MSG_001";
                                        }

                                        if (iSalesOrder.COUNTRY != null && !listCountryID.Contains(iSalesOrder.COUNTRY[0].COUNTRY_ID))
                                        {
                                            listCountry.Add(iSalesOrder.COUNTRY[0]);
                                            listCountryID.Add(iSalesOrder.COUNTRY[0].COUNTRY_ID);
                                        }

                                        listData.Add(iSalesOrder);
                                        strMessage = "MSG_001";
                                    }
                                    else
                                    {
                                        strMessage = "";
                                    }
                                }
                            }
                        }
                    }

                    Results.status = "S";
                    for (int i = 0; i <= listData.Count - 1; i++)
                    {
                        listData[i].COUNTRY = listCountry;
                        listData[i].PRODUCT = listProduct;
                    }
                    //if (index == -1 && UserPosition == "FFC") {
                    //    alertError2("Sales order is mismatch");
                    //} else if (arr_index == -1 && UserPosition == "FFC") {
                    //    alertError2("Product code is not found.Please contact your system administrator.");
                    //} else if (plantList.length > 1 && arr_index != -1 && index !=-1) {
                    //    alertError2("If your product is a VAP, Please select the VAP product flag. Default product flag function is not supported multiple production plants.<br/>");
                    //}  
                    Results.data = listData;
                    if (strMessage.Length > 0)
                    {
                        Results.msg = MessageHelper.GetMessage(strMessage);
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

        public static ART_WF_ARTWORK_REQUEST_RESULT GetMaterialBySalesOrder(ART_WF_ARTWORK_REQUEST_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();

            string secondaryPkgNodeID = "";
            string secondaryPkgArtworkFolderName = "";
            string configArtworkOther = "";
            Int64 intSecondaryPkgNodeID = 0;

            if (param == null && param.data == null)
            {
                //return 
            }

            try
            {
                secondaryPkgNodeID = ConfigurationManager.AppSettings["SecondaryPackagingNodeID"];
                secondaryPkgArtworkFolderName = ConfigurationManager.AppSettings["SecondaryPkgArtworkFolderName"];
                configArtworkOther = ConfigurationManager.AppSettings["ArtworkFolderNamePrintMaster"];

                if (!String.IsNullOrEmpty(secondaryPkgNodeID))
                {
                    intSecondaryPkgNodeID = Convert.ToInt64(secondaryPkgNodeID);
                }

                var artworkRequest = context.ART_WF_ARTWORK_REQUEST.Where(a => a.ARTWORK_REQUEST_ID == param.data.ARTWORK_REQUEST_ID).FirstOrDefault();
                List<string> DistinctCOMPONENT_MATERIAL = new List<string>();
                foreach (var item in param.data.SALES_ORDER_REPEAT)
                {
                    if (String.IsNullOrEmpty(item.COMPONENT_MATERIAL) && !String.IsNullOrEmpty(item.PRODUCT_CODE) && item.PRODUCT_CODE.StartsWith("5"))
                    {
                        if (!DistinctCOMPONENT_MATERIAL.Contains(item.PRODUCT_CODE))
                        {
                            DistinctCOMPONENT_MATERIAL.Add(item.PRODUCT_CODE);
                        }
                    }
                    else
                    {
                        if (!DistinctCOMPONENT_MATERIAL.Contains(item.COMPONENT_MATERIAL))
                        {
                            DistinctCOMPONENT_MATERIAL.Add(item.COMPONENT_MATERIAL);
                        }
                    }
                }

                var token = CWSService.getAuthToken();
                foreach (var COMPONENT_MATERIAL in DistinctCOMPONENT_MATERIAL)
                {
                    var DECRIPTION = (from s in context.XECM_M_PRODUCT5
                                      where s.PRODUCT_CODE == COMPONENT_MATERIAL
                                      select s.PRODUCT_DESCRIPTION).FirstOrDefault();

                    if (DECRIPTION != null)
                    {
                        string matDesc = "";
                        matDesc = COMPONENT_MATERIAL + " - " + DECRIPTION;

                        Node nodeMat = CWSService.getNodeByName(intSecondaryPkgNodeID, matDesc, token);

                        if (nodeMat != null)
                        {
                            Node nodeMatAWFolder = CWSService.getNodeByName(-nodeMat.ID, secondaryPkgArtworkFolderName, token);
                            if (nodeMatAWFolder != null)
                            {
                                Node[] nodeMatAWFile = CWSService.getAllNodeInFolder(nodeMatAWFolder.ID, token);

                                if (nodeMatAWFile != null)
                                {
                                    ART_WF_ARTWORK_REQUEST_ITEM itemRequest = new ART_WF_ARTWORK_REQUEST_ITEM();
                                    foreach (Node iNodeAW in nodeMatAWFile)
                                    {
                                        long nodeID = 0;

                                        if (artworkRequest.REQUEST_FORM_FOLDER_NODE_ID != null)
                                        {
                                            nodeID = Convert.ToInt64(artworkRequest.REQUEST_FORM_FOLDER_NODE_ID);
                                        }

                                        Node nodeOthers = CWSService.getNodeByName(nodeID, configArtworkOther, token);
                                        if (nodeID > 0)
                                        {
                                            if (nodeOthers != null)
                                            {

                                                // start by aof #INC-89321
                                                BLL.DocumentManagement.Version version = CWSService.getVersion(iNodeAW.ID, token, iNodeAW.VersionInfo.VersionNum);
                                                iNodeAW.Name = version.Filename;
                                                var newNode = CWSService.copyNode(iNodeAW.Name, iNodeAW.ID, nodeOthers.ID, token);
                                                //var newNode = CWSService.copyNode(iNodeAW.Name, iNodeAW.ID, nodeOthers.ID, token); // commeted by aof #INC-89321
                                                // end by aof #INC-89321

                                                if (newNode.VersionInfo != null)
                                                {
                                                    string extension = Path.GetExtension(iNodeAW.Name);
                                                    string contentType = newNode.VersionInfo.MimeType;

                                                    extension = extension.Replace(".", "");

                                                    itemRequest = new ART_WF_ARTWORK_REQUEST_ITEM();
                                                    itemRequest.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;
                                                    itemRequest.CONTENT_TYPE = contentType;
                                                    itemRequest.FILE_NAME = iNodeAW.Name;
                                                    itemRequest.FILE_SIZE = Convert.ToInt64(newNode.VersionInfo.FileDataSize);
                                                    itemRequest.REQUEST_FORM_FILE_NODE_ID = newNode.ID;
                                                    itemRequest.EXTENSION = extension;
                                                    itemRequest.REPEAT_SO_MATERIAL_NO = COMPONENT_MATERIAL;
                                                    itemRequest.CREATE_BY = param.data.UPDATE_BY;
                                                    itemRequest.UPDATE_BY = param.data.UPDATE_BY;

                                                    ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.SaveOrUpdateNoLog(itemRequest, context);
                                                }
                                            }
                                        }
                                    }
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

        public static ART_WF_ARTWORK_REQUEST_RESULT ValidateMaterialBySalesOrder(ART_WF_ARTWORK_REQUEST_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();

            string secondaryPkgNodeID = "";
            string secondaryPkgArtworkFolderName = "";
            string configArtworkOther = "";
            Int64 intSecondaryPkgNodeID = 0;

            if (param == null && param.data == null)
            {
                //return 
            }

            try
            {
                secondaryPkgNodeID = ConfigurationManager.AppSettings["SecondaryPackagingNodeID"];
                secondaryPkgArtworkFolderName = ConfigurationManager.AppSettings["SecondaryPkgArtworkFolderName"];
                configArtworkOther = ConfigurationManager.AppSettings["ArtworkFolderName"];

                if (!String.IsNullOrEmpty(secondaryPkgNodeID))
                {
                    intSecondaryPkgNodeID = Convert.ToInt64(secondaryPkgNodeID);
                }

                string message = "";

                var artworkRequest = context.ART_WF_ARTWORK_REQUEST.Where(a => a.ARTWORK_REQUEST_ID == param.data.ARTWORK_REQUEST_ID).FirstOrDefault();
                List<string> DistinctCOMPONENT_MATERIAL = new List<string>();
                foreach (var item in param.data.SALES_ORDER_REPEAT)
                {
                    if (String.IsNullOrEmpty(item.COMPONENT_MATERIAL) && !String.IsNullOrEmpty(item.PRODUCT_CODE) && item.PRODUCT_CODE.StartsWith("5"))
                    {
                        DistinctCOMPONENT_MATERIAL.Add(item.PRODUCT_CODE);
                    }
                    else
                    {
                        DistinctCOMPONENT_MATERIAL.Add(item.COMPONENT_MATERIAL);
                    }
                }

                DistinctCOMPONENT_MATERIAL = DistinctCOMPONENT_MATERIAL.Select(m => m).Distinct().ToList();
                var token = CWSService.getAuthToken();
                foreach (var COMPONENT_MATERIAL in DistinctCOMPONENT_MATERIAL)
                {
                    var DECRIPTION = (from s in context.XECM_M_PRODUCT5
                                      where s.PRODUCT_CODE == COMPONENT_MATERIAL
                                      select s.PRODUCT_DESCRIPTION).FirstOrDefault();

                    if (DECRIPTION == null)
                    {
                        message = MessageHelper.GetMessage("MSG_020", context) + "<br>" + String.Format(MessageHelper.GetMessage("MSG_021", context), COMPONENT_MATERIAL);

                        Results.status = "E";
                        Results.msg = message;
                        return Results;
                    }

                    string matDesc = COMPONENT_MATERIAL + " - " + DECRIPTION;
                    Node nodeMat = CWSService.getNodeByName(intSecondaryPkgNodeID, matDesc, token);
                    if (nodeMat == null)
                    {
                        message = MessageHelper.GetMessage("MSG_020", context) + "<br>" + String.Format(MessageHelper.GetMessage("MSG_021", context), COMPONENT_MATERIAL);
                        Results.status = "E";
                        Results.msg = message;
                        return Results;
                    }

                    Node nodeMatAWFolder = CWSService.getNodeByName(-nodeMat.ID, secondaryPkgArtworkFolderName, token);
                    if (nodeMatAWFolder == null)
                    {
                        message = MessageHelper.GetMessage("MSG_020", context) + "<br>" + String.Format(MessageHelper.GetMessage("MSG_021", context), COMPONENT_MATERIAL);
                        Results.status = "E";
                        Results.msg = message;
                        return Results;
                    }

                    Node[] nodeMatAWFile = CWSService.getAllNodeInFolder(nodeMatAWFolder.ID, token);
                    if (nodeMatAWFile == null)
                    {
                        message = MessageHelper.GetMessage("MSG_020", context) + "<br>" + "System not found a file in material master workspace." + "(" + COMPONENT_MATERIAL + ")";
                        Results.status = "E";
                        Results.msg = message;
                        return Results;
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

        public static string ValidateMatAndSaleOrder(ART_WF_ARTWORK_REQUEST_REQUEST param, ARTWORKEntities context)
        {
            var res = "";

            var tempMat5 = param.data.SALES_ORDER_REPEAT.Select(m => m.COMPONENT_MATERIAL).Distinct().ToList();
            tempMat5.AddRange(param.data.SALES_ORDER_REPEAT.Where(m => m.PRODUCT_CODE.StartsWith("5")).Select(m => m.PRODUCT_CODE).Distinct().ToList());

            if (tempMat5.Where(m => !String.IsNullOrEmpty(m)).Distinct().Count() > 1)
            {
                res = "Cannot select multiple material code.";
                return res;
            }

            var tempSO = param.data.SALES_ORDER.Select(m => m.SALES_ORDER_NO).ToList();
            var tempListSO = (from m in context.V_SAP_SALES_ORDER
                              where tempSO.Contains(m.SALES_ORDER_NO)
                              select new V_SAP_SALES_ORDER_2 { SOLD_TO = m.SOLD_TO, SHIP_TO = m.SHIP_TO, BRAND_ID = m.BRAND_ID }).ToList();

            if (tempListSO.Where(w => !String.IsNullOrEmpty(w.SOLD_TO)).Select(m => m.SOLD_TO).Distinct().Count() > 1)
            {
                res = "Cannot select multiple sold to.";
                return res;
            }
            if (tempListSO.Where(w => !String.IsNullOrEmpty(w.SHIP_TO)).Select(m => m.SHIP_TO).Distinct().Count() > 1)
            {
                res = "Cannot select multiple ship to.";
                return res;
            }
            if (param.data.SALES_ORDER_REPEAT.Where(w => !String.IsNullOrEmpty(w.BRAND_DISPLAY_TXT)).Select(m => m.BRAND_DISPLAY_TXT).Distinct().Count() > 1)
            {
                res = "Cannot select multiple brand.";
                return res;
            }

            var general = false;
            var DistinctCOMPONENT_MATERIAL = param.data.SALES_ORDER_REPEAT.Select(m => m.COMPONENT_MATERIAL).Distinct().ToList().FirstOrDefault();
            if (DistinctCOMPONENT_MATERIAL == null)
                DistinctCOMPONENT_MATERIAL = param.data.SALES_ORDER_REPEAT.Select(m => m.PRODUCT_CODE).Distinct().ToList().FirstOrDefault();
            else
            {
                general = true;
            }

            foreach (var SALES_ORDER in param.data.SALES_ORDER_REPEAT)
            {
                ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT temp = new ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT();

                //check in workflow
                var soDetail = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL>();
                if (general)
                {
                    soDetail = (from q in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                join m in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT on q.BOM_ID equals m.PO_COMPLETE_SO_ITEM_COMPONENT_ID
                                where q.SALES_ORDER_NO == SALES_ORDER.SALES_ORDER_NO
                                && q.SALES_ORDER_ITEM == SALES_ORDER.SALES_ORDER_ITEM
                                && q.MATERIAL_NO == SALES_ORDER.PRODUCT_CODE
                                && m.COMPONENT_MATERIAL == SALES_ORDER.COMPONENT_MATERIAL
                                select q).ToList();
                }
                else
                {
                    soDetail = (from q in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                where q.SALES_ORDER_NO == SALES_ORDER.SALES_ORDER_NO
                                && q.SALES_ORDER_ITEM == SALES_ORDER.SALES_ORDER_ITEM
                                && q.MATERIAL_NO == SALES_ORDER.COMPONENT_MATERIAL
                                select q).ToList();
                }

                if (soDetail.Count > 0)
                {
                    var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(soDetail[0].ARTWORK_SUB_ID, context);
                    var item = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(process.ARTWORK_ITEM_ID, context);

                    res = "Cannot select the below item" + "<br/>";
                    res += "SO : " + SALES_ORDER.SALES_ORDER_NO + "<br/>";
                    res += "Material no : " + soDetail[0].MATERIAL_NO + "<br/>";
                    res += "Please check on workflow no : " + "<a target='_blank' href='/TaskFormArtwork/" + soDetail[0].ARTWORK_SUB_ID + "'>" + item.REQUEST_ITEM_NO + "</a>";

                    return res;
                }
                else
                {
                    //not found in workflow
                    //find in request form
                    if (general)
                    {
                        temp = ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT()
                        { SALES_ORDER_NO = SALES_ORDER.SALES_ORDER_NO, SALES_ORDER_ITEM = SALES_ORDER.SALES_ORDER_ITEM, COMPONENT_MATERIAL = DistinctCOMPONENT_MATERIAL }, context).FirstOrDefault();
                    }
                    else
                    {
                        temp = ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT()
                        { SALES_ORDER_NO = SALES_ORDER.SALES_ORDER_NO, SALES_ORDER_ITEM = SALES_ORDER.SALES_ORDER_ITEM, PRODUCT_CODE = DistinctCOMPONENT_MATERIAL }, context).FirstOrDefault();
                    }

                    if (temp != null)
                    {
                        var request = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(temp.ARTWORK_REQUEST_ID, context);
                        if (request != null)
                        {
                            res = "Cannot select the below item" + "<br/>";
                            res += "SO : " + SALES_ORDER.SALES_ORDER_NO + "<br/>";
                            res += "Material no : " + DistinctCOMPONENT_MATERIAL + "<br/>";
                            res += "Please check on request form no : " + "<a target='_blank' href='/Artwork/" + temp.ARTWORK_REQUEST_ID + "'>" + request.ARTWORK_REQUEST_NO + "</a>";

                            return res;
                        }
                    }
                }
            }

            if (CNService.IsLock(DistinctCOMPONENT_MATERIAL, context))
            {
                res = "This material number is locked. Please contact your PA supervisor.";
                return res;
            }

            return res;
        }

        private static void CopyRequestForm(string materialCode, int newRepeatRequestID, ARTWORKEntities context)
        {
            if (!string.IsNullOrEmpty(materialCode))
            {
                var processPA = (from a in context.ART_WF_ARTWORK_PROCESS_PA
                                 where a.MATERIAL_NO == materialCode
                                 select a).OrderByDescending(o => o.ARTWORK_SUB_PA_ID).FirstOrDefault();

                var reqForm_New = (from r in context.ART_WF_ARTWORK_REQUEST
                                   where r.ARTWORK_REQUEST_ID == newRepeatRequestID
                                   select r)
                                  .FirstOrDefault();

                var listSORepeat = (from r in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                    where r.ARTWORK_REQUEST_ID == reqForm_New.ARTWORK_REQUEST_ID
                                    select r.SALES_ORDER_NO).Distinct().ToList();

                var soHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                where listSORepeat.Contains(h.SALES_ORDER_NO)
                                select h).OrderBy(o => o.RDD).FirstOrDefault();

                var soRepeat = (from r in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                where r.ARTWORK_REQUEST_ID == reqForm_New.ARTWORK_REQUEST_ID
                                    && r.SALES_ORDER_NO == soHeader.SALES_ORDER_NO
                                select r).FirstOrDefault();

                var soRepeatHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                      where h.SALES_ORDER_NO == soRepeat.SALES_ORDER_NO
                                      select h).FirstOrDefault();

                //by aof 03/11/2021 for Itail change save plant from PA to SO #start.
                var listPlantbySORepeat = (from s in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                           join h in context.SAP_M_PO_COMPLETE_SO_HEADER on s.SALES_ORDER_NO equals h.SALES_ORDER_NO
                                           join i in context.SAP_M_PO_COMPLETE_SO_ITEM on h.PO_COMPLETE_SO_HEADER_ID equals i.PO_COMPLETE_SO_HEADER_ID
                                           join p in context.SAP_M_PLANT on i.PRODUCTION_PLANT equals p.PLANT
                                           where listSORepeat.Contains(h.SALES_ORDER_NO) && p.IS_ACTIVE == "X" && s.SALES_ORDER_ITEM == i.ITEM.ToString()
                                           select p
                                  ).Distinct().ToList();
                //by aof 03/11/2021 for Itail change save plant from PA to SO #end.


                if (processPA != null)
                {
                    int? reqId = CNService.FindArtworkRequestId(processPA.ARTWORK_SUB_ID, context);

                    if (reqId != null)
                    {
                        var reqForm_Exist = (from r in context.ART_WF_ARTWORK_REQUEST
                                             where r.ARTWORK_REQUEST_ID == reqId
                                             select r)
                                       .OrderByDescending(o => o.UPDATE_DATE)
                                       .FirstOrDefault();

                        ART_WF_ARTWORK_REQUEST reqInit = new ART_WF_ARTWORK_REQUEST();
                        reqInit = reqForm_New;

                        if (reqForm_Exist != null)
                        {
                            // set copy value
                            //reqForm_New = reqForm_Exist;
                            reqForm_New.PROJECT_NAME = reqForm_Exist.PROJECT_NAME;
                            reqForm_New.TYPE_OF_PRODUCT_ID = reqForm_Exist.TYPE_OF_PRODUCT_ID;
                            reqForm_New.BRAND_ID = reqForm_Exist.BRAND_ID;
                            reqForm_New.REVIEWER_ID = reqForm_Exist.REVIEWER_ID;
                            reqForm_New.COMPANY_ID = reqForm_Exist.COMPANY_ID;
                            reqForm_New.SOLD_TO_ID = reqForm_Exist.SOLD_TO_ID;
                            reqForm_New.SHIP_TO_ID = reqForm_Exist.SHIP_TO_ID;
                            reqForm_New.CUSTOMER_OTHER_ID = reqForm_Exist.CUSTOMER_OTHER_ID;
                            reqForm_New.PRIMARY_TYPE_ID = reqForm_Exist.PRIMARY_TYPE_ID;
                            reqForm_New.PRIMARY_TYPE_OTHER = reqForm_Exist.PRIMARY_TYPE_OTHER;
                            reqForm_New.TYPE_OF_PRODUCT_ID = reqForm_Exist.TYPE_OF_PRODUCT_ID;
                            reqForm_New.REQUEST_DELIVERY_DATE = reqForm_Exist.REQUEST_DELIVERY_DATE;
                            reqForm_New.REQUEST_FORM_CREATE_DATE = reqForm_Exist.REQUEST_FORM_CREATE_DATE;


                            //CR#19739 by aof
                            if (reqForm_New.TYPE_OF_ARTWORK == "REPEAT")
                            {
                                reqForm_New.REVIEWER_ID = null;
                            }
                            //CR#19739 by aof

                            decimal itemNO = 0;
                            itemNO = Convert.ToDecimal(soRepeat.SALES_ORDER_ITEM);

                            var soItem = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
                                          where i.PO_COMPLETE_SO_HEADER_ID == soHeader.PO_COMPLETE_SO_HEADER_ID
                                          where i.ITEM == itemNO
                                          select i).FirstOrDefault();

                            if (soHeader != null)
                            {
                                if (!String.IsNullOrEmpty(soHeader.SOLD_TO))
                                {
                                    var xSoldTo = (from s in context.XECM_M_CUSTOMER
                                                   where s.CUSTOMER_CODE == soHeader.SOLD_TO
                                                   select s).FirstOrDefault();

                                    if (xSoldTo != null)
                                    {
                                        reqForm_New.SOLD_TO_ID = xSoldTo.CUSTOMER_ID;
                                    }
                                }

                                if (!String.IsNullOrEmpty(soHeader.SHIP_TO))
                                {
                                    var xShipTo = (from s in context.XECM_M_CUSTOMER
                                                   where s.CUSTOMER_CODE == soHeader.SHIP_TO
                                                   select s).FirstOrDefault();

                                    if (xShipTo != null)
                                    {
                                        reqForm_New.SHIP_TO_ID = xShipTo.CUSTOMER_ID;
                                    }
                                }
                            }

                            if (soItem != null)
                            {
                                if (!String.IsNullOrEmpty(soItem.BRAND_ID))
                                {
                                    var brand = (from b in context.SAP_M_BRAND
                                                 where b.MATERIAL_GROUP == soItem.BRAND_ID
                                                 select b).FirstOrDefault();

                                    if (brand != null)
                                    {
                                        reqForm_New.BRAND_ID = brand.BRAND_ID;
                                    }
                                }
                            }

                            ART_WF_ARTWORK_REQUEST_SERVICE.SaveOrUpdateNoLog(reqForm_New, context);

                            //Save Other customer (mail to/cc)
                            var otherCustomers = (from c in context.ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER
                                                  where c.ARTWORK_REQUEST_ID == reqForm_Exist.ARTWORK_REQUEST_ID
                                                  select c).ToList();

                            if (otherCustomers != null && otherCustomers.Count > 0)
                            {
                                ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER otherCustomer = new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER();
                                foreach (ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER iOtherCust in otherCustomers)
                                {
                                    otherCustomer = new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER();

                                    otherCustomer.ARTWORK_REQUEST_ID = reqForm_New.ARTWORK_REQUEST_ID;
                                    otherCustomer.CUSTOMER_USER_ID = iOtherCust.CUSTOMER_USER_ID;
                                    otherCustomer.MAIL_CC = iOtherCust.MAIL_CC;
                                    otherCustomer.MAIL_TO = iOtherCust.MAIL_TO;
                                    otherCustomer.CREATE_BY = reqForm_New.CREATE_BY;
                                    otherCustomer.UPDATE_BY = reqForm_New.UPDATE_BY;

                                    ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.SaveOrUpdateNoLog(otherCustomer, context);
                                }
                            }


                            // by aof 03/11/2021 for Itail change save plant from PA to SO #start.
                            //var plant = (from c in context.ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT
                            //             where c.ARTWORK_REQUEST_ID == reqForm_Exist.ARTWORK_REQUEST_ID
                            //             select c).ToList();

                            //foreach (var item in plant)
                            //{
                            //    ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT model = new ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT();
                            //    model.ARTWORK_REQUEST_ID = reqForm_New.ARTWORK_REQUEST_ID;
                            //    model.PRODUCTION_PLANT_ID = item.PRODUCTION_PLANT_ID;
                            //    model.CREATE_BY = reqForm_New.CREATE_BY;
                            //    model.UPDATE_BY = reqForm_New.UPDATE_BY;
                            //    ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_SERVICE.SaveOrUpdateNoLog(model, context);
                            //}

                            if (listPlantbySORepeat != null && listPlantbySORepeat.Count > 0) {

                                foreach (var item in listPlantbySORepeat)
                                {
                                    ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT model = new ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT();
                                    model.ARTWORK_REQUEST_ID = reqForm_New.ARTWORK_REQUEST_ID;
                                    model.PRODUCTION_PLANT_ID = item.PLANT_ID;
                                    model.CREATE_BY = reqForm_New.CREATE_BY;
                                    model.UPDATE_BY = reqForm_New.UPDATE_BY;
                                    ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_SERVICE.SaveOrUpdateNoLog(model, context);
                                }
                            }
                            // by aof 03/11/2021 for Itail change save plant from PA to SO #end.

                            var soRepeat_Country = (from r in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                                    where r.ARTWORK_REQUEST_ID == reqForm_New.ARTWORK_REQUEST_ID
                                                    select r).ToList();

                            if (soRepeat_Country != null)
                            {
                                foreach (ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT iSORC in soRepeat_Country)
                                {
                                    var soH = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                               where h.SALES_ORDER_NO == iSORC.SALES_ORDER_NO
                                               select h).OrderByDescending(o => o.PO_COMPLETE_SO_HEADER_ID).FirstOrDefault();

                                    if (soH != null)
                                    {
                                        decimal itemNO_C = 0;
                                        itemNO_C = Convert.ToDecimal(iSORC.SALES_ORDER_ITEM);
                                        var soI = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
                                                   where i.PO_COMPLETE_SO_HEADER_ID == soH.PO_COMPLETE_SO_HEADER_ID
                                                    && i.ITEM == itemNO_C
                                                   select i).FirstOrDefault();

                                        if (soI != null)
                                        {
                                            if (!String.IsNullOrEmpty(soI.COUNTRY))
                                            {
                                                var country = (from c in context.SAP_M_COUNTRY
                                                               where c.COUNTRY_CODE == soI.COUNTRY
                                                               select c).FirstOrDefault();

                                                var countryExist = (from c in context.ART_WF_ARTWORK_REQUEST_COUNTRY
                                                                    where c.ARTWORK_REQUEST_ID == reqForm_New.ARTWORK_REQUEST_ID
                                                                    select c.COUNTRY_ID).ToList();

                                                if (country != null)
                                                {
                                                    if (!countryExist.Contains(country.COUNTRY_ID))
                                                    {
                                                        ART_WF_ARTWORK_REQUEST_COUNTRY model = new ART_WF_ARTWORK_REQUEST_COUNTRY();
                                                        model.ARTWORK_REQUEST_ID = reqForm_New.ARTWORK_REQUEST_ID;
                                                        model.COUNTRY_ID = country.COUNTRY_ID;
                                                        model.CREATE_BY = reqForm_New.CREATE_BY;
                                                        model.UPDATE_BY = reqForm_New.UPDATE_BY;
                                                        ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.SaveOrUpdateNoLog(model, context);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (soRepeat != null)
                    {
                        decimal itemNO = 0;
                        itemNO = Convert.ToDecimal(soRepeat.SALES_ORDER_ITEM);

                        var soRepeatItem = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
                                            where i.PO_COMPLETE_SO_HEADER_ID == soRepeatHeader.PO_COMPLETE_SO_HEADER_ID
                                                && i.ITEM == itemNO
                                            select i).FirstOrDefault();

                        if (soRepeatHeader != null)
                        {
                            if (!String.IsNullOrEmpty(soRepeatHeader.SOLD_TO))
                            {
                                var xSoldTo = (from s in context.XECM_M_CUSTOMER
                                               where s.CUSTOMER_CODE == soRepeatHeader.SOLD_TO // where s.CUSTOMER_CODE.Contains(soRepeatHeader.SOLD_TO) //Ticket 446829 by aof
                                               select s).FirstOrDefault();

                                if (xSoldTo != null)
                                {
                                    reqForm_New.SOLD_TO_ID = xSoldTo.CUSTOMER_ID;
                                }
                            }

                            if (!String.IsNullOrEmpty(soRepeatHeader.SHIP_TO))
                            {
                                var xShipTo = (from s in context.XECM_M_CUSTOMER
                                               where s.CUSTOMER_CODE == soRepeatHeader.SHIP_TO // where s.CUSTOMER_CODE.Contains(soRepeatHeader.SHIP_TO) //Ticket 446829 by aof
                                               select s).FirstOrDefault();

                                if (xShipTo != null)
                                {
                                    reqForm_New.SHIP_TO_ID = xShipTo.CUSTOMER_ID;
                                }
                            }
                        }

                        if (soRepeatItem != null)
                        {
                            if (!String.IsNullOrEmpty(soRepeatItem.BRAND_ID))
                            {
                                var brand = (from b in context.SAP_M_BRAND
                                             where b.MATERIAL_GROUP == soRepeatItem.BRAND_ID
                                             select b).FirstOrDefault();

                                if (brand != null)
                                {
                                    reqForm_New.BRAND_ID = brand.BRAND_ID;
                                }
                            }

                            var country = (from c in context.SAP_M_COUNTRY
                                           where c.COUNTRY_CODE == soRepeatItem.COUNTRY
                                           select c).FirstOrDefault();

                            ART_WF_ARTWORK_REQUEST_COUNTRY model = new ART_WF_ARTWORK_REQUEST_COUNTRY();
                            model.ARTWORK_REQUEST_ID = reqForm_New.ARTWORK_REQUEST_ID;
                            model.COUNTRY_ID = country.COUNTRY_ID;
                            model.CREATE_BY = reqForm_New.CREATE_BY;
                            model.UPDATE_BY = reqForm_New.UPDATE_BY;
                            ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.SaveOrUpdateNoLog(model, context);
                        }


                        // by aof 03/11/2021 for Itail change save plant from PA to SO #start.

                        //var listSOItem = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
                        //                  where i.PO_COMPLETE_SO_HEADER_ID == soRepeatHeader.PO_COMPLETE_SO_HEADER_ID
                        //                      && i.ITEM == itemNO
                        //                  select i).ToList();

                        //foreach (var item in listSOItem)
                        //{
                        //    var plant = (from c in context.SAP_M_PLANT
                        //                 where c.PLANT == item.PRODUCTION_PLANT   // by aof iTail Change from Plant to Production Plant 
                        //                 select c).FirstOrDefault();

                        //    if (plant != null)
                        //    {
                        //        ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT modelPlant = new ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT();
                        //        modelPlant.ARTWORK_REQUEST_ID = reqForm_New.ARTWORK_REQUEST_ID;
                        //        modelPlant.PRODUCTION_PLANT_ID = plant.PLANT_ID;
                        //        modelPlant.CREATE_BY = reqForm_New.CREATE_BY;
                        //        modelPlant.UPDATE_BY = reqForm_New.UPDATE_BY;
                        //        ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_SERVICE.SaveOrUpdateNoLog(modelPlant, context);
                        //    }
                        //}

                        if (listPlantbySORepeat != null && listPlantbySORepeat.Count > 0)
                        {

                            foreach (var item in listPlantbySORepeat)
                            {
                                ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT model = new ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT();
                                model.ARTWORK_REQUEST_ID = reqForm_New.ARTWORK_REQUEST_ID;
                                model.PRODUCTION_PLANT_ID = item.PLANT_ID;
                                model.CREATE_BY = reqForm_New.CREATE_BY;
                                model.UPDATE_BY = reqForm_New.UPDATE_BY;
                                ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_SERVICE.SaveOrUpdateNoLog(model, context);
                            }
                        }
                        // by aof 03/11/2021 for Itail change save plant from PA to SO #end.

                    }

                    if (reqForm_New != null)
                    {
                        reqForm_New.REQUEST_DELIVERY_DATE = soRepeatHeader.RDD;
                        ART_WF_ARTWORK_REQUEST_SERVICE.SaveOrUpdateNoLog(reqForm_New, context);
                    }
                }

                if (reqForm_New != null)
                {
                    reqForm_New.REQUEST_DELIVERY_DATE = soRepeatHeader.RDD;
                    ART_WF_ARTWORK_REQUEST_SERVICE.SaveOrUpdateNoLog(reqForm_New, context);
                }
            }
        }

        public static void CopyRequestProductCode(int newRepeatRequestID, ARTWORKEntities context, ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            var COMPONENT_MATERIAL = param.data.SALES_ORDER_REPEAT.Select(m => m.COMPONENT_MATERIAL).FirstOrDefault();
            if (string.IsNullOrEmpty(COMPONENT_MATERIAL))
                COMPONENT_MATERIAL = param.data.SALES_ORDER_REPEAT.Select(m => m.PRODUCT_CODE).FirstOrDefault();

            CopyRequestForm(COMPONENT_MATERIAL, newRepeatRequestID, context);

            var Distinct_PRODUCT_CODE = param.data.SALES_ORDER_REPEAT.Select(m => m.PRODUCT_CODE).Distinct().ToList();
            foreach (var item in Distinct_PRODUCT_CODE)
            {
                if (!String.IsNullOrEmpty(item))
                {
                    var xProduct = (from p in context.XECM_M_PRODUCT
                                    where p.PRODUCT_CODE == item
                                    select p).FirstOrDefault();

                    if (xProduct != null)
                    {
                        var product = new ART_WF_ARTWORK_REQUEST_PRODUCT();
                        product.ARTWORK_REQUEST_ID = newRepeatRequestID;
                        product.PRODUCT_CODE_ID = xProduct.XECM_PRODUCT_ID;
                        product.CREATE_BY = param.data.SALES_ORDER_REPEAT[0].CREATE_BY;
                        product.UPDATE_BY = param.data.SALES_ORDER_REPEAT[0].UPDATE_BY;

                        ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.SaveOrUpdateNoLog(product, context);
                    }
                }
            }
        }

        //----------------------------start tuning performance sorepeat 2022 by aof-------------------------------------------------------------------------//


        private static void CopyRequestFormByLastWF(ref ART_WF_ARTWORK_REQUEST_2 obj,string materialCode, ARTWORKEntities context)
        {
            if (!string.IsNullOrEmpty(materialCode))
            {
                var processPA = (from a in context.ART_WF_ARTWORK_PROCESS_PA
                                 where a.MATERIAL_NO == materialCode
                                 select a).OrderByDescending(o => o.ARTWORK_SUB_PA_ID).FirstOrDefault();

                //var reqForm_New = (from r in context.ART_WF_ARTWORK_REQUEST
                //                   where r.ARTWORK_REQUEST_ID == newRepeatRequestID
                //                   select r)
                //                  .FirstOrDefault();

                var listSORepeat = (from r in obj.SALES_ORDER_REPEAT //context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                    //where r.ARTWORK_REQUEST_ID == reqForm_New.ARTWORK_REQUEST_ID
                                    select r.SALES_ORDER_NO).Distinct().ToList();

                var listSOItemRepeat = (from r in obj.SALES_ORDER_REPEAT                                                                         
                                        select r.SALES_ORDER_ITEM).Distinct().ToList();

                var soHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                where listSORepeat.Contains(h.SALES_ORDER_NO)
                                select h).OrderBy(o => o.RDD).FirstOrDefault();

                var soRepeat = (from r in obj.SALES_ORDER_REPEAT //context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                 // where r.ARTWORK_REQUEST_ID == reqForm_New.ARTWORK_REQUEST_ID
                                 //&& r.SALES_ORDER_NO == soHeader.SALES_ORDER_NO
                                where r.SALES_ORDER_NO == soHeader.SALES_ORDER_NO 
                                select r).FirstOrDefault();

                var soRepeatHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                      where h.SALES_ORDER_NO == soRepeat.SALES_ORDER_NO
                                      select h).FirstOrDefault();

                //by aof 03/11/2021 for Itail change save plant from PA to SO #start.
                //var listPlantbySORepeat = (from s in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                //                           join h in context.SAP_M_PO_COMPLETE_SO_HEADER on s.SALES_ORDER_NO equals h.SALES_ORDER_NO
                //                           join i in context.SAP_M_PO_COMPLETE_SO_ITEM on h.PO_COMPLETE_SO_HEADER_ID equals i.PO_COMPLETE_SO_HEADER_ID
                //                           join p in context.SAP_M_PLANT on i.PRODUCTION_PLANT equals p.PLANT
                //                           where listSORepeat.Contains(h.SALES_ORDER_NO) && p.IS_ACTIVE == "X" && s.SALES_ORDER_ITEM == i.ITEM.ToString()
                //                           select p
                //                  ).Distinct().ToList();
                var listPlantbySORepeat = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                           join i in context.SAP_M_PO_COMPLETE_SO_ITEM on h.PO_COMPLETE_SO_HEADER_ID equals i.PO_COMPLETE_SO_HEADER_ID
                                           join p in context.SAP_M_PLANT on i.PRODUCTION_PLANT equals p.PLANT
                                           where listSORepeat.Contains(h.SALES_ORDER_NO) && p.IS_ACTIVE == "X" && listSOItemRepeat.Contains(i.ITEM.ToString())
                                           select p).Distinct().ToList();

                //by aof 03/11/2021 for Itail change save plant from PA to SO #end.

                obj.COUNTRY = new List<ART_WF_ARTWORK_REQUEST_COUNTRY_2>();
                obj.MAIL_TO_CUSTOMER = new List<ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_2>();
                obj.PRODUCTION_PLANT = new List<ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2>();

                if (processPA != null)
                {
                    int? reqId = CNService.FindArtworkRequestId(processPA.ARTWORK_SUB_ID, context);

                    if (reqId != null)
                    {
                        var reqForm_Exist = (from r in context.ART_WF_ARTWORK_REQUEST
                                             where r.ARTWORK_REQUEST_ID == reqId
                                             select r)
                                       .OrderByDescending(o => o.UPDATE_DATE)
                                       .FirstOrDefault();

                        //ART_WF_ARTWORK_REQUEST reqInit = new ART_WF_ARTWORK_REQUEST();
                        //reqInit = reqForm_New;

                        if (reqForm_Exist != null)
                        {
                            // set copy value
                            //reqForm_New = reqForm_Exist;
                            obj.PROJECT_NAME = reqForm_Exist.PROJECT_NAME;
                            obj.TYPE_OF_PRODUCT_ID = reqForm_Exist.TYPE_OF_PRODUCT_ID;
                            obj.BRAND_ID = reqForm_Exist.BRAND_ID;
                            obj.REVIEWER_ID = null;    //CR#19739 by aof
                            obj.COMPANY_ID = reqForm_Exist.COMPANY_ID;
                            obj.SOLD_TO_ID = reqForm_Exist.SOLD_TO_ID;
                            obj.SHIP_TO_ID = reqForm_Exist.SHIP_TO_ID;
                            obj.CUSTOMER_OTHER_ID = reqForm_Exist.CUSTOMER_OTHER_ID;
                            obj.PRIMARY_TYPE_ID = reqForm_Exist.PRIMARY_TYPE_ID;
                            obj.PRIMARY_TYPE_OTHER = reqForm_Exist.PRIMARY_TYPE_OTHER;
                            obj.TYPE_OF_PRODUCT_ID = reqForm_Exist.TYPE_OF_PRODUCT_ID;
                            obj.REQUEST_DELIVERY_DATE = reqForm_Exist.REQUEST_DELIVERY_DATE;
                            obj.REQUEST_FORM_CREATE_DATE = reqForm_Exist.REQUEST_FORM_CREATE_DATE;
                        


                            decimal itemNO = 0;
                            itemNO = Convert.ToDecimal(soRepeat.SALES_ORDER_ITEM);

                            var soItem = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
                                          where i.PO_COMPLETE_SO_HEADER_ID == soHeader.PO_COMPLETE_SO_HEADER_ID
                                          where i.ITEM == itemNO
                                          select i).FirstOrDefault();

                            if (soHeader != null)
                            {
                                if (!String.IsNullOrEmpty(soHeader.SOLD_TO))
                                {
                                    var xSoldTo = (from s in context.XECM_M_CUSTOMER
                                                   where s.CUSTOMER_CODE == soHeader.SOLD_TO
                                                   select s).FirstOrDefault();

                                    if (xSoldTo != null)
                                    {
                                        obj.SOLD_TO_ID = xSoldTo.CUSTOMER_ID;
                                    }
                                }

                                if (!String.IsNullOrEmpty(soHeader.SHIP_TO))
                                {
                                    var xShipTo = (from s in context.XECM_M_CUSTOMER
                                                   where s.CUSTOMER_CODE == soHeader.SHIP_TO
                                                   select s).FirstOrDefault();

                                    if (xShipTo != null)
                                    {
                                        obj.SHIP_TO_ID = xShipTo.CUSTOMER_ID;
                                    }
                                }
                            }

                            if (soItem != null)
                            {
                                if (!String.IsNullOrEmpty(soItem.BRAND_ID))
                                {
                                    var brand = (from b in context.SAP_M_BRAND
                                                 where b.MATERIAL_GROUP == soItem.BRAND_ID
                                                 select b).FirstOrDefault();

                                    if (brand != null)
                                    {
                                        obj.BRAND_ID = brand.BRAND_ID;
                                    }
                                }
                            }

                           // ART_WF_ARTWORK_REQUEST_SERVICE.SaveOrUpdateNoLog(reqForm_New, context);

                            //Save Other customer (mail to/cc)

                            var otherCustomers = (from c in context.ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER
                                                  where c.ARTWORK_REQUEST_ID == reqForm_Exist.ARTWORK_REQUEST_ID
                                                  select c).ToList();

                            if (otherCustomers != null && otherCustomers.Count > 0)
                            {
                              
                                ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER otherCustomer = new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER();
                                foreach (ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER iOtherCust in otherCustomers)
                                {
                                    otherCustomer = new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER();

                                    //otherCustomer.ARTWORK_REQUEST_ID = reqForm_New.ARTWORK_REQUEST_ID;
                                    otherCustomer.CUSTOMER_USER_ID = iOtherCust.CUSTOMER_USER_ID;
                                    otherCustomer.MAIL_CC = iOtherCust.MAIL_CC;
                                    otherCustomer.MAIL_TO = iOtherCust.MAIL_TO;
                                    otherCustomer.CREATE_BY = obj.CREATE_BY;
                                    otherCustomer.UPDATE_BY = obj.UPDATE_BY;

                                    //ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.SaveOrUpdateNoLog(otherCustomer, context);
                                    obj.MAIL_TO_CUSTOMER.Add(MapperServices.ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER(otherCustomer));
                                }
                            }


                            // by aof 03/11/2021 for Itail change save plant from PA to SO #start.
                            //var plant = (from c in context.ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT
                            //             where c.ARTWORK_REQUEST_ID == reqForm_Exist.ARTWORK_REQUEST_ID
                            //             select c).ToList();

                            //foreach (var item in plant)
                            //{
                            //    ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT model = new ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT();
                            //    model.ARTWORK_REQUEST_ID = reqForm_New.ARTWORK_REQUEST_ID;
                            //    model.PRODUCTION_PLANT_ID = item.PRODUCTION_PLANT_ID;
                            //    model.CREATE_BY = reqForm_New.CREATE_BY;
                            //    model.UPDATE_BY = reqForm_New.UPDATE_BY;
                            //    ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_SERVICE.SaveOrUpdateNoLog(model, context);
                            //}

                            if (listPlantbySORepeat != null && listPlantbySORepeat.Count > 0)
                            {

                           
                                foreach (var item in listPlantbySORepeat)
                                {
                                    ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT model = new ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT();
                                    //model.ARTWORK_REQUEST_ID = obj.ARTWORK_REQUEST_ID;
                                    model.PRODUCTION_PLANT_ID = item.PLANT_ID;
                                    model.CREATE_BY = obj.CREATE_BY;
                                    model.UPDATE_BY = obj.UPDATE_BY;
                                    // ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_SERVICE.SaveOrUpdateNoLog(model, context);
                                    obj.PRODUCTION_PLANT.Add(MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT(model));
                                }
                            }
                            // by aof 03/11/2021 for Itail change save plant from PA to SO #end.

                            //var soRepeat_Country = (from r in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                            //                        where r.ARTWORK_REQUEST_ID == reqForm_New.ARTWORK_REQUEST_ID
                            //                        select r).ToList();

                            if (obj.SALES_ORDER_REPEAT != null)
                            {
                             

                                foreach (ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2 iSORC in obj.SALES_ORDER_REPEAT)
                                {
                                    var soH = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                               where h.SALES_ORDER_NO == iSORC.SALES_ORDER_NO
                                               select h).OrderByDescending(o => o.PO_COMPLETE_SO_HEADER_ID).FirstOrDefault();

                                    if (soH != null)
                                    {
                                        decimal itemNO_C = 0;
                                        itemNO_C = Convert.ToDecimal(iSORC.SALES_ORDER_ITEM);
                                        var soI = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
                                                   where i.PO_COMPLETE_SO_HEADER_ID == soH.PO_COMPLETE_SO_HEADER_ID
                                                    && i.ITEM == itemNO_C
                                                   select i).FirstOrDefault();

                                        if (soI != null)
                                        {
                                            if (!String.IsNullOrEmpty(soI.COUNTRY))
                                            {
                                                var country = (from c in context.SAP_M_COUNTRY
                                                               where c.COUNTRY_CODE == soI.COUNTRY
                                                               select c).FirstOrDefault();

                                                var countryExist = (from c in obj.COUNTRY                                                               
                                                                    select c.COUNTRY_ID).ToList();

                                                if (country != null)
                                                {
                                                    if (!countryExist.Contains(country.COUNTRY_ID))
                                                    {
                                                        ART_WF_ARTWORK_REQUEST_COUNTRY model = new ART_WF_ARTWORK_REQUEST_COUNTRY();
                                                        //model.ARTWORK_REQUEST_ID = reqForm_New.ARTWORK_REQUEST_ID;
                                                        model.COUNTRY_ID = country.COUNTRY_ID;
                                                        model.CREATE_BY = obj.CREATE_BY;
                                                        model.UPDATE_BY = obj.UPDATE_BY;
                                                        // ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.SaveOrUpdateNoLog(model, context);
                                                        obj.COUNTRY.Add(MapperServices.ART_WF_ARTWORK_REQUEST_COUNTRY(model));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (soRepeat != null)
                    {
                        decimal itemNO = 0;
                        itemNO = Convert.ToDecimal(soRepeat.SALES_ORDER_ITEM);

                        var soRepeatItem = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
                                            where i.PO_COMPLETE_SO_HEADER_ID == soRepeatHeader.PO_COMPLETE_SO_HEADER_ID
                                                && i.ITEM == itemNO
                                            select i).FirstOrDefault();

                        if (soRepeatHeader != null)
                        {
                            if (!String.IsNullOrEmpty(soRepeatHeader.SOLD_TO))
                            {
                                var xSoldTo = (from s in context.XECM_M_CUSTOMER
                                               where s.CUSTOMER_CODE == soRepeatHeader.SOLD_TO // where s.CUSTOMER_CODE.Contains(soRepeatHeader.SOLD_TO) //Ticket 446829 by aof
                                               select s).FirstOrDefault();

                                if (xSoldTo != null)
                                {
                                    obj.SOLD_TO_ID = xSoldTo.CUSTOMER_ID;
                                }
                            }

                            if (!String.IsNullOrEmpty(soRepeatHeader.SHIP_TO))
                            {
                                var xShipTo = (from s in context.XECM_M_CUSTOMER
                                               where s.CUSTOMER_CODE == soRepeatHeader.SHIP_TO // where s.CUSTOMER_CODE.Contains(soRepeatHeader.SHIP_TO) //Ticket 446829 by aof
                                               select s).FirstOrDefault();

                                if (xShipTo != null)
                                {
                                    obj.SHIP_TO_ID = xShipTo.CUSTOMER_ID;
                                }
                            }
                        }

                        if (soRepeatItem != null)
                        {
                            if (!String.IsNullOrEmpty(soRepeatItem.BRAND_ID))
                            {
                                var brand = (from b in context.SAP_M_BRAND
                                             where b.MATERIAL_GROUP == soRepeatItem.BRAND_ID
                                             select b).FirstOrDefault();

                                if (brand != null)
                                {
                                    obj.BRAND_ID = brand.BRAND_ID;
                                }
                            }

                         

                            var country = (from c in context.SAP_M_COUNTRY
                                           where c.COUNTRY_CODE == soRepeatItem.COUNTRY
                                           select c).FirstOrDefault();

                            ART_WF_ARTWORK_REQUEST_COUNTRY model = new ART_WF_ARTWORK_REQUEST_COUNTRY();
                            // model.ARTWORK_REQUEST_ID = reqForm_New.ARTWORK_REQUEST_ID;
                            model.COUNTRY_ID = country.COUNTRY_ID;
                            model.CREATE_BY = obj.CREATE_BY;
                            model.UPDATE_BY = obj.UPDATE_BY;
                            obj.COUNTRY.Add(MapperServices.ART_WF_ARTWORK_REQUEST_COUNTRY(model));
                           // ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.SaveOrUpdateNoLog(model, context);
                        }


                        // by aof 03/11/2021 for Itail change save plant from PA to SO #start.

                        //var listSOItem = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
                        //                  where i.PO_COMPLETE_SO_HEADER_ID == soRepeatHeader.PO_COMPLETE_SO_HEADER_ID
                        //                      && i.ITEM == itemNO
                        //                  select i).ToList();

                        //foreach (var item in listSOItem)
                        //{
                        //    var plant = (from c in context.SAP_M_PLANT
                        //                 where c.PLANT == item.PRODUCTION_PLANT   // by aof iTail Change from Plant to Production Plant 
                        //                 select c).FirstOrDefault();

                        //    if (plant != null)
                        //    {
                        //        ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT modelPlant = new ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT();
                        //        modelPlant.ARTWORK_REQUEST_ID = reqForm_New.ARTWORK_REQUEST_ID;
                        //        modelPlant.PRODUCTION_PLANT_ID = plant.PLANT_ID;
                        //        modelPlant.CREATE_BY = reqForm_New.CREATE_BY;
                        //        modelPlant.UPDATE_BY = reqForm_New.UPDATE_BY;
                        //        ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_SERVICE.SaveOrUpdateNoLog(modelPlant, context);
                        //    }
                        //}

                        if (listPlantbySORepeat != null && listPlantbySORepeat.Count > 0)
                        {

                            foreach (var item in listPlantbySORepeat)
                            {
                                ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT model = new ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT();
                                //model.ARTWORK_REQUEST_ID = reqForm_New.ARTWORK_REQUEST_ID;
                                model.PRODUCTION_PLANT_ID = item.PLANT_ID;
                                model.CREATE_BY = obj.CREATE_BY;
                                model.UPDATE_BY = obj.UPDATE_BY;
                                // ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_SERVICE.SaveOrUpdateNoLog(model, context);
                                obj.PRODUCTION_PLANT.Add(MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT(model));
                            }
                        }
                        // by aof 03/11/2021 for Itail change save plant from PA to SO #end.

                    }

                    if (obj != null)
                    {
                        obj.REQUEST_DELIVERY_DATE = soRepeatHeader.RDD;
                        //ART_WF_ARTWORK_REQUEST_SERVICE.SaveOrUpdateNoLog(reqForm_New, context);
                    }
                }

                if (obj != null)
                {
                    obj.REQUEST_DELIVERY_DATE = soRepeatHeader.RDD;
                    //ART_WF_ARTWORK_REQUEST_SERVICE.SaveOrUpdateNoLog(reqForm_New, context);
                }
            }
        }

        public static string PrepareArtworkItemProcessbySORepeat(ref ART_WF_ARTWORK_REQUEST_2 objReq, ARTWORKEntities context)
        {

            var msg = "";
            try
            {
                string stepCode = "SEND_PA";

                var stepPA = (from s in context.ART_M_STEP_ARTWORK
                            where s.STEP_ARTWORK_CODE == stepCode
                            select s).FirstOrDefault();

                foreach (var item in objReq.REQUEST_ITEMS)
                {

                    //item.PROCESS.ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID;
                    //item.PROCESS.ARTWORK_ITEM_ID = item.ARTWORK_ITEM_ID;
                    item.PROCESS_STEP_PA.CURRENT_STEP_ID = stepPA.STEP_ARTWORK_ID;
                    item.PROCESS_STEP_PA.CURRENT_ROLE_ID = stepPA.ROLE_ID_RESPONSE; //null;
                    item.PROCESS_STEP_PA.CURRENT_USER_ID = CNService.CheckPICArtworkBySORepeat(context, objReq);
                    item.PROCESS_STEP_PA.CREATE_BY = objReq.CREATE_BY;
                    item.PROCESS_STEP_PA.UPDATE_BY = objReq.CREATE_BY;

                    // var requestTmp = context.ART_WF_ARTWORK_REQUEST.Where(r => r.ARTWORK_REQUEST_ID == process.ARTWORK_REQUEST_ID).FirstOrDefault();

                    var listDelegate = ART_WF_DELEGATE_SERVICE.GetByItem(new ART_WF_DELEGATE() { IS_ACTIVE = "X", CURRENT_USER_ID = Convert.ToInt32(item.PROCESS_STEP_PA.CURRENT_USER_ID) }, context);
                    listDelegate = listDelegate.Where(m => DateTime.Now.Date >= m.FROM_DATE.Date && DateTime.Now.Date <= m.TO_DATE.Date).ToList();

                    if (listDelegate.FirstOrDefault() != null)
                    {
                        item.PROCESS_STEP_PA.IS_DELEGATE = "X";
                        item.PROCESS_STEP_PA.CURRENT_USER_ID = listDelegate.FirstOrDefault().TO_USER_ID;
                        // ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(process, context);

                        item.PROCESS_STEP_PA.LOG_DELEGATE = new ART_WF_LOG_DELEGATE();
                        item.PROCESS_STEP_PA.LOG_DELEGATE.WF_TYPE = "A";
                        //item.PROCESS.LOG_DELEGATE.WF_SUB_ID = process.ARTWORK_SUB_ID;
                        item.PROCESS_STEP_PA.LOG_DELEGATE.FROM_USER_ID = listDelegate.FirstOrDefault().CURRENT_USER_ID;
                        item.PROCESS_STEP_PA.LOG_DELEGATE.TO_USER_ID = listDelegate.FirstOrDefault().TO_USER_ID;
                        item.PROCESS_STEP_PA.LOG_DELEGATE.DELEGATE_BY = listDelegate.FirstOrDefault().TO_USER_ID;
                        item.PROCESS_STEP_PA.LOG_DELEGATE.STEP_ID = item.PROCESS_STEP_PA.CURRENT_STEP_ID;
                        item.PROCESS_STEP_PA.LOG_DELEGATE.REMARK = listDelegate.FirstOrDefault().REASON;
                        item.PROCESS_STEP_PA.LOG_DELEGATE.CREATE_BY = listDelegate.FirstOrDefault().TO_USER_ID;
                        item.PROCESS_STEP_PA.LOG_DELEGATE.UPDATE_BY = listDelegate.FirstOrDefault().TO_USER_ID;
                        //ART_WF_LOG_DELEGATE_SERVICE.SaveOrUpdate(model, context);
                    }


                    // listProcess.Add(process);



                    item.PROCESS_STEP_PA.PROCESS_PA = new ART_WF_ARTWORK_PROCESS_PA_2();
                    //item.PROCESS.PROCESS_PA.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;             
                    item.PROCESS_STEP_PA.PROCESS_PA.PA_USER_ID = item.PROCESS_STEP_PA.CURRENT_USER_ID;
                    item.PROCESS_STEP_PA.PROCESS_PA.CREATE_BY = objReq.UPDATE_BY;
                    item.PROCESS_STEP_PA.PROCESS_PA.UPDATE_BY = objReq.UPDATE_BY;

                    //ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA, context);

                    if (objReq.TYPE_OF_ARTWORK == "REPEAT" && !CNService.IsFFC(objReq.CREATE_BY, context))
                    {


                        item.PROCESS_STEP_PA.PROCESS_PA.REQUEST_MATERIAL_STATUS = "Completed";
                        item.PROCESS_STEP_PA.PROCESS_PA.MATERIAL_NO = item.REPEAT_SO_MATERIAL_NO;

                        #region "01.prepare ART_WF_ARTWORK_PROCESS_SO_DETAIL"

                        if (objReq.SALES_ORDER_REPEAT != null && objReq.SALES_ORDER_REPEAT.Count() > 0)
                        {
                            decimal itemNO = 0;
                            var productCode = "";

                            if (item.PROCESS_STEP_PA.LIST_PROCESS_SO_DETAIL == null)
                            {
                                item.PROCESS_STEP_PA.LIST_PROCESS_SO_DETAIL = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>();
                            }


                            ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 soDetail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
                            //var so_hd_id = 0;
                            var temp_running_id = 1;
                            foreach (var iSORepeat in objReq.SALES_ORDER_REPEAT)
                            {
                                productCode = iSORepeat.PRODUCT_CODE;

                                soDetail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
                                //soDetail.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                                //soDetail.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                                soDetail.SALES_ORDER_NO = iSORepeat.SALES_ORDER_NO;
                                soDetail.SALES_ORDER_ITEM = iSORepeat.SALES_ORDER_ITEM;
                                soDetail.MATERIAL_NO = iSORepeat.PRODUCT_CODE;

                                if (!String.IsNullOrEmpty(productCode))
                                {
                                    soDetail.BOM_NO = null;
                                    if (productCode.StartsWith("3"))
                                    {
                                        itemNO = Convert.ToDecimal(iSORepeat.SALES_ORDER_ITEM);
                                        var bom = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                                   join i in context.SAP_M_PO_COMPLETE_SO_ITEM on h.PO_COMPLETE_SO_HEADER_ID equals i.PO_COMPLETE_SO_HEADER_ID
                                                   join c in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT on i.PO_COMPLETE_SO_ITEM_ID equals c.PO_COMPLETE_SO_ITEM_ID
                                                   where h.SALES_ORDER_NO == iSORepeat.SALES_ORDER_NO
                                                   && i.ITEM == itemNO
                                                   && c.COMPONENT_MATERIAL == iSORepeat.COMPONENT_MATERIAL && c.COMPONENT_ITEM == iSORepeat.COMPONENT_ITEM
                                                   select c).FirstOrDefault();

                                        if (bom != null)
                                        {
                                            soDetail.BOM_ID = bom.PO_COMPLETE_SO_ITEM_COMPONENT_ID;
                                        }
                                    }
                                }

                                soDetail.CREATE_BY = objReq.CREATE_BY;
                                soDetail.UPDATE_BY = objReq.UPDATE_BY;
                                soDetail.TEMP_RUNNING_ID = temp_running_id;
                                temp_running_id += 1;
                                item.PROCESS_STEP_PA.LIST_PROCESS_SO_DETAIL.Add(soDetail);


                                #region "prepare SO Detail FOC Item"
                                //SAP_M_PO_COMPLETE_SO_HEADER _detailHeaderFOC = new SAP_M_PO_COMPLETE_SO_HEADER();
                                //SAP_M_PO_COMPLETE_SO_ITEM _detailItemFOC = new SAP_M_PO_COMPLETE_SO_ITEM();
                                //var saleOrder = listFOC.Where(f => f.SALES_ORDER_NO == iSORepeat.SALES_ORDER_NO && f.ITEM_CUSTOM_1 == iSORepeat.SALES_ORDER_ITEM && f.PRODUCT_CODE == iSORepeat.COMPONENT_MATERIAL).ToList();
                                var soFOCs = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                             join i in context.SAP_M_PO_COMPLETE_SO_ITEM on h.PO_COMPLETE_SO_HEADER_ID equals i.PO_COMPLETE_SO_HEADER_ID
                                             where h.SALES_ORDER_NO == iSORepeat.SALES_ORDER_NO
                                             && i.ITEM_CUSTOM_1 == iSORepeat.SALES_ORDER_ITEM
                                             && i.PRODUCT_CODE == iSORepeat.COMPONENT_MATERIAL
                                             && i.IS_ACTIVE == "X" && string.IsNullOrEmpty(i.REJECTION_CODE)
                                             select i).ToList();
                                if (soFOCs != null)
                                {
                                    foreach (var so in soFOCs)
                                    {
                                        ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 soFOC = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();

                                        //detail.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                                        //detail.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                                        soFOC.SALES_ORDER_NO = iSORepeat.SALES_ORDER_NO;
                                        soFOC.SALES_ORDER_ITEM = so.ITEM.ToString();
                                        soFOC.MATERIAL_NO = so.PRODUCT_CODE;
                                        soFOC.BOM_NO = "FOC";
                                        soFOC.CREATE_BY = objReq.CREATE_BY;
                                        soFOC.UPDATE_BY = objReq.UPDATE_BY;

                                        var cntFOC = (from m in item.PROCESS_STEP_PA.LIST_PROCESS_SO_DETAIL
                                                      where m.SALES_ORDER_NO == soFOC.SALES_ORDER_NO
                                                      && m.SALES_ORDER_ITEM == soFOC.SALES_ORDER_ITEM
                                                      select m).Count();
                                        if (cntFOC == 0)
                                        {
                                            soFOC.TEMP_RUNNING_ID = temp_running_id;
                                            temp_running_id += 1;
                                            item.PROCESS_STEP_PA.LIST_PROCESS_SO_DETAIL.Add(soFOC);
                                        }

                                    }
                                }

                                #endregion

                                #region"prepare Copy Assign SaleOrder"
                                if (item.PROCESS_STEP_PA.LIST_PROCESS_SO_DETAIL != null && item.PROCESS_STEP_PA.LIST_PROCESS_SO_DETAIL.Count > 0)
                                {
                                    foreach (var so in item.PROCESS_STEP_PA.LIST_PROCESS_SO_DETAIL)
                                    {
                                        if (so != null)
                                        {
                                            #region "preare Assign SO Header"
                                            var soHeader = context.SAP_M_PO_COMPLETE_SO_HEADER.Where(w => w.SALES_ORDER_NO == so.SALES_ORDER_NO).FirstOrDefault();
                                            if (soHeader != null)
                                            {

                                                ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2 assignHeader = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2();

                                                assignHeader.SO_HEADER = new SAP_M_PO_COMPLETE_SO_HEADER();
                                                assignHeader.SO_HEADER = soHeader;


                                                //assignHeader.ARTWORK_PROCESS_SO_ID = so.ARTWORK_PROCESS_SO_ID;
                                                //assignHeader.ARTWORK_REQUEST_ID = param.ARTWORK_REQUEST_ID;
                                                //assignHeader.ARTWORK_SUB_ID = param.ARTWORK_SUB_ID;
                                                assignHeader.SALES_ORDER_NO = soHeader.SALES_ORDER_NO.Trim();
                                                assignHeader.SOLD_TO = soHeader.SOLD_TO.Trim();
                                                assignHeader.SOLD_TO_NAME = soHeader.SOLD_TO_NAME.Trim();
                                                assignHeader.LAST_SHIPMENT_DATE = soHeader.LAST_SHIPMENT_DATE;
                                                assignHeader.DATE_1_2 = soHeader.DATE_1_2;
                                                assignHeader.CREATE_ON = soHeader.CREATE_ON;
                                                assignHeader.RDD = soHeader.RDD;
                                                assignHeader.PAYMENT_TERM = soHeader.PAYMENT_TERM.Trim();
                                                assignHeader.LC_NO = soHeader.LC_NO.Trim();
                                                assignHeader.EXPIRED_DATE = soHeader.EXPIRED_DATE;
                                                assignHeader.SHIP_TO = soHeader.SHIP_TO.Trim();
                                                assignHeader.SHIP_TO_NAME = soHeader.SHIP_TO_NAME.Trim();
                                                assignHeader.SOLD_TO_PO = soHeader.SOLD_TO_PO.Trim();
                                                assignHeader.SHIP_TO_PO = soHeader.SHIP_TO_PO.Trim();
                                                assignHeader.SALES_GROUP = soHeader.SALES_GROUP.Trim();
                                                assignHeader.MARKETING_CO = soHeader.MARKETING_CO.Trim();
                                                assignHeader.MARKETING_CO_NAME = soHeader.MARKETING_CO_NAME.Trim();
                                                assignHeader.MARKETING = soHeader.MARKETING.Trim();
                                                assignHeader.MARKETING_NAME = soHeader.MARKETING_NAME.Trim();
                                                assignHeader.MARKETING_ORDER_SAP = soHeader.MARKETING_ORDER_SAP.Trim();
                                                assignHeader.MARKETING_ORDER_SAP_NAME = soHeader.MARKETING_ORDER_SAP_NAME.Trim();
                                                assignHeader.SALES_ORG = soHeader.SALES_ORG.Trim();
                                                assignHeader.DISTRIBUTION_CHANNEL = soHeader.DISTRIBUTION_CHANNEL.Trim();
                                                assignHeader.DIVITION = soHeader.DIVITION.Trim();
                                                assignHeader.SALES_ORDER_TYPE = soHeader.SALES_ORDER_TYPE.Trim();
                                                assignHeader.HEADER_CUSTOM_1 = soHeader.HEADER_CUSTOM_1.Trim();
                                                assignHeader.HEADER_CUSTOM_2 = soHeader.HEADER_CUSTOM_2.Trim();
                                                assignHeader.HEADER_CUSTOM_3 = soHeader.HEADER_CUSTOM_3.Trim();
                                                assignHeader.CREATE_BY = objReq.UPDATE_BY;
                                                assignHeader.UPDATE_BY = objReq.UPDATE_BY;


                                                #region "prepare Assign Sale Long Text - General Text"
                                                var tempSO = assignHeader.SALES_ORDER_NO.ToString().PadLeft(10, '0');

                                                if (item.PROCESS_STEP_PA.LIST_ASSIGN_SO_LONG_TEXT == null)
                                                {
                                                    item.PROCESS_STEP_PA.LIST_ASSIGN_SO_LONG_TEXT = new List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2>();
                                                }

                                                var temp = (from m in item.PROCESS_STEP_PA.LIST_ASSIGN_SO_LONG_TEXT
                                                            where m.TEXT_NAME == tempSO && m.TEXT_ID == "Z001"
                                                            select m.ASSIGN_SO_LONG_TEXT_ID).ToList();

                                                if (temp.Count() == 0)
                                                {
                                                    // var doWork = false;
                                                    var listLongText_General = context.SAP_M_LONG_TEXT.Where(m => m.TEXT_NAME == tempSO && m.TEXT_ID == "Z001").ToList();
                                                    foreach (var text in listLongText_General)
                                                    {
                                                        ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2 assignLongText = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2();
                                                        //assignLongText.ARTWORK_PROCESS_SO_ID = so.ARTWORK_PROCESS_SO_ID;
                                                        //assignLongText.ARTWORK_REQUEST_ID = param.ARTWORK_REQUEST_ID;
                                                        //assignLongText.ARTWORK_SUB_ID = param.ARTWORK_SUB_ID;
                                                        assignLongText.TEXT_NAME = text.TEXT_NAME.Trim();
                                                        assignLongText.TEXT_LANGUAGE = text.TEXT_LANGUAGE.Trim();
                                                        assignLongText.TEXT_ID = text.TEXT_ID.Trim();
                                                        assignLongText.LINE_ID = text.LINE_ID;
                                                        assignLongText.LINE_TEXT = text.LINE_TEXT.Trim();
                                                        assignLongText.CREATE_BY = objReq.UPDATE_BY;
                                                        assignLongText.UPDATE_BY = objReq.UPDATE_BY;
                                                        //assignLongText.CREATE_DATE = DateTime.Now;
                                                        //assignLongText.UPDATE_DATE = DateTime.Now;

                                                        //doWork = true;
                                                        //context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT.Add(assignLongText);
                                                        //ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_SERVICE.SaveNoLog(assignLongText, context);
                                                        assignLongText.TEMP_RUNNING_ID = so.TEMP_RUNNING_ID;
                                                        item.PROCESS_STEP_PA.LIST_ASSIGN_SO_LONG_TEXT.Add(assignLongText);
                                                    }
                                                    // if (doWork) context.SaveChanges();
                                                }
                                                #endregion

                                                #region "prepare Assign Sale ITEM No"
                                                decimal itemNo = 0;
                                                itemNo = Convert.ToDecimal(so.SALES_ORDER_ITEM);
                                                var soItem = context.SAP_M_PO_COMPLETE_SO_ITEM.Where(m => m.PO_COMPLETE_SO_HEADER_ID == soHeader.PO_COMPLETE_SO_HEADER_ID && m.ITEM == itemNo).FirstOrDefault();

                                                if (soItem != null)
                                                {
                                                    ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2 assignItem = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2();
                                                    //assignItem.ASSIGN_SO_HEADER_ID = assignHeader.ASSIGN_SO_HEADER_ID;
                                                    // assignItem.ARTWORK_PROCESS_SO_ID = so.ARTWORK_PROCESS_SO_ID;
                                                    // assignItem.ARTWORK_REQUEST_ID = param.ARTWORK_REQUEST_ID;
                                                    //assignItem.ARTWORK_SUB_ID = param.ARTWORK_SUB_ID;
                                                    assignItem.ITEM = soItem.ITEM;
                                                    assignItem.PRODUCT_CODE = soItem.PRODUCT_CODE.Trim();
                                                    assignItem.MATERIAL_DESCRIPTION = soItem.MATERIAL_DESCRIPTION.Trim();
                                                    assignItem.NET_WEIGHT = soItem.NET_WEIGHT.Trim();
                                                    assignItem.ORDER_QTY = soItem.ORDER_QTY;
                                                    assignItem.ORDER_UNIT = soItem.ORDER_UNIT.Trim();
                                                    assignItem.ETD_DATE_FROM = soItem.ETD_DATE_FROM;
                                                    assignItem.ETD_DATE_TO = soItem.ETD_DATE_TO;
                                                    assignItem.PLANT = soItem.PLANT.Trim();
                                                    assignItem.OLD_MATERIAL_CODE = soItem.OLD_MATERIAL_CODE.Trim();
                                                    assignItem.PACK_SIZE = soItem.PACK_SIZE.Trim();
                                                    assignItem.VALUME_PER_UNIT = soItem.VALUME_PER_UNIT.Trim();
                                                    assignItem.VALUME_UNIT = soItem.VALUME_UNIT.Trim();
                                                    assignItem.SIZE_DRAIN_WT = soItem.SIZE_DRAIN_WT.Trim();
                                                    assignItem.PROD_INSP_MEMO = soItem.PROD_INSP_MEMO.Trim();
                                                    assignItem.REJECTION_CODE = soItem.REJECTION_CODE.Trim();
                                                    assignItem.REJECTION_DESCRIPTION = soItem.REJECTION_DESCRIPTION.Trim();
                                                    assignItem.PORT = soItem.PORT.Trim();
                                                    assignItem.VIA = soItem.VIA.Trim();
                                                    assignItem.IN_TRANSIT_TO = soItem.IN_TRANSIT_TO.Trim();
                                                    assignItem.BRAND_ID = soItem.BRAND_ID.Trim();
                                                    assignItem.BRAND_DESCRIPTION = soItem.BRAND_DESCRIPTION.Trim();
                                                    assignItem.ADDITIONAL_BRAND_ID = soItem.ADDITIONAL_BRAND_ID.Trim();
                                                    assignItem.ADDITIONAL_BRAND_DESCRIPTION = soItem.ADDITIONAL_BRAND_DESCRIPTION.Trim();
                                                    assignItem.PRODUCTION_PLANT = soItem.PRODUCTION_PLANT.Trim();
                                                    assignItem.ZONE = soItem.ZONE.Trim();
                                                    assignItem.COUNTRY = soItem.COUNTRY.Trim();
                                                    assignItem.PRODUCTION_HIERARCHY = soItem.PRODUCTION_HIERARCHY.Trim();
                                                    assignItem.MRP_CONTROLLER = soItem.MRP_CONTROLLER.Trim();
                                                    assignItem.STOCK = soItem.STOCK.Trim();
                                                    assignItem.ITEM_CUSTOM_1 = soItem.ITEM_CUSTOM_1.Trim();
                                                    assignItem.ITEM_CUSTOM_2 = soItem.ITEM_CUSTOM_2.Trim();
                                                    assignItem.ITEM_CUSTOM_3 = soItem.ITEM_CUSTOM_3.Trim();
                                                    assignItem.CREATE_BY = objReq.UPDATE_BY;
                                                    assignItem.UPDATE_BY = objReq.UPDATE_BY;

                                                    ////ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_SERVICE.SaveNoLog(assignItem, context);
                                                    assignItem.SAP_SO_ITEM_2 = soItem;
                                                    assignItem.SAP_SO_ITEM_2.IS_ASSIGN = "X";
                                                    //assignItem.SAP_SO_ITEM_2.PO_COMPLETE_SO_ITEM_ID = soItem.PO_COMPLETE_SO_ITEM_ID;

                                                    // SAP_M_PO_COMPLETE_SO_ITEM_SERVICE.UpdateNoLog(soItem, context);

                                                    #region "prepare Long Text - Warehouse Text"

                                                    string orderNOTmp = assignHeader.SALES_ORDER_NO.ToString().PadLeft(10, '0');
                                                    string itemNOTmp = soItem.ITEM.ToString().PadLeft(6, '0');
                                                    string textName = orderNOTmp + itemNOTmp;

                                                    //var doWork = false;
                                                    var listLongText_Warehouse = context.SAP_M_LONG_TEXT.Where(m => m.TEXT_NAME == textName && m.TEXT_ID == "Z105").ToList();
                                                    foreach (SAP_M_LONG_TEXT txt in listLongText_Warehouse)
                                                    {
                                                        ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2 assignLongText = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2();
                                                        //assignLongText.ARTWORK_PROCESS_SO_ID = so.ARTWORK_PROCESS_SO_ID;
                                                        //assignLongText.ARTWORK_REQUEST_ID = param.ARTWORK_REQUEST_ID;
                                                        //assignLongText.ARTWORK_SUB_ID = param.ARTWORK_SUB_ID;
                                                        assignLongText.TEXT_NAME = txt.TEXT_NAME.Trim();
                                                        assignLongText.TEXT_LANGUAGE = txt.TEXT_LANGUAGE.Trim();
                                                        assignLongText.TEXT_ID = txt.TEXT_ID.Trim();
                                                        assignLongText.LINE_ID = txt.LINE_ID;
                                                        assignLongText.LINE_TEXT = txt.LINE_TEXT.Trim();
                                                        assignLongText.CREATE_BY = objReq.UPDATE_BY;
                                                        assignLongText.UPDATE_BY = objReq.UPDATE_BY;
                                                        //assignLongText.CREATE_DATE = DateTime.Now;
                                                        //assignLongText.UPDATE_DATE = DateTime.Now;
                                                        assignLongText.TEMP_RUNNING_ID = so.TEMP_RUNNING_ID;
                                                        // doWork = true;
                                                        //context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT.Add(assignLongText);
                                                        //ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_SERVICE.SaveNoLog(assignLongText, context);
                                                        item.PROCESS_STEP_PA.LIST_ASSIGN_SO_LONG_TEXT.Add(assignLongText);
                                                    }
                                                    //if (doWork) context.SaveChanges();

                                                    #endregion

                                                    #region"prepare Assign Item Compoenent"
                                                    if (so.BOM_ID > 0)
                                                    {
                                                        var soItemBom = context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT.Where(m => m.PO_COMPLETE_SO_ITEM_COMPONENT_ID == so.BOM_ID).FirstOrDefault();
                                                        if (soItemBom != null)
                                                        {
                                                            ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2 assignBOM = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2();

                                                            //assignBOM.ASSIGN_SO_ITEM_ID = assignItem.ASSIGN_SO_ITEM_ID;
                                                            //assignBOM.ARTWORK_PROCESS_SO_ID = so.ARTWORK_PROCESS_SO_ID;
                                                            //assignBOM.ARTWORK_REQUEST_ID = param.ARTWORK_REQUEST_ID;
                                                            //assignBOM.ARTWORK_SUB_ID = param.ARTWORK_SUB_ID;
                                                            assignBOM.COMPONENT_ITEM = soItemBom.COMPONENT_ITEM.Trim();
                                                            assignBOM.COMPONENT_MATERIAL = soItemBom.COMPONENT_MATERIAL.Trim();
                                                            assignBOM.DECRIPTION = soItemBom.DECRIPTION.Trim();
                                                            assignBOM.QUANTITY = soItemBom.QUANTITY;
                                                            assignBOM.UNIT = soItemBom.UNIT.Trim();
                                                            assignBOM.STOCK = soItemBom.STOCK.Trim();
                                                            assignBOM.BOM_ITEM_CUSTOM_1 = string.Format("{0}", soItemBom.BOM_ITEM_CUSTOM_1).Trim();
                                                            assignBOM.BOM_ITEM_CUSTOM_2 = soItemBom.BOM_ITEM_CUSTOM_2.Trim();
                                                            assignBOM.BOM_ITEM_CUSTOM_3 = soItemBom.BOM_ITEM_CUSTOM_3.Trim();
                                                            assignBOM.CREATE_BY = objReq.CREATE_BY;
                                                            assignBOM.UPDATE_BY = objReq.UPDATE_BY;

                                                            // ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_SERVICE.SaveNoLog(assignBOM, context);
                                                            //update component

                                                            assignBOM.SAP_SO_ITEM_COMPONENT = soItemBom; //new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2();
                                                            assignBOM.SAP_SO_ITEM_COMPONENT.IS_ASSIGN = "X";
                                                            //assignBOM.SAP_SO_ITEM_COMPONENT.PO_COMPLETE_SO_ITEM_COMPONENT_ID = soItemBom.PO_COMPLETE_SO_ITEM_COMPONENT_ID;

                                                            assignItem.ASSIGN_SO_ITEM_COMPONENT = assignBOM;
                                                            //SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_SERVICE.UpdateNoLog(soItemBom, context);
                                                            //var pitem = CNService.GetAssignsoItem(soItem, context); //#
                                                            //if (pitem == 0)
                                                            //{
                                                            //    soItem.IS_ASSIGN = "X";
                                                            //    SAP_M_PO_COMPLETE_SO_ITEM_SERVICE.UpdateNoLog(soItem, context);
                                                            //}
                                                        }
                                                    }
                                                    #endregion
                                                    assignHeader.ASSIGN_SO_ITEM = assignItem;
                                                    ////ticket 437485
                                                    //var listSO = CNService.GetAssignOrder(soHeader.PO_COMPLETE_SO_HEADER_ID, context);
                                                    //if (listSO == 0)
                                                    //{
                                                    //    soHeader.IS_ASSIGN = "X";
                                                    //    SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.UpdateNoLog(soHeader, context);
                                                    //}
                                                }

                                                #endregion

                                                so.ASSIGN_SO_HEADER = assignHeader;

                                            }
                                            #endregion
                                        }

                                    }
                                }


                                #endregion
                            }
                        }
                        #endregion


                        #region "02.prepare ART_WF_ARTWORK_PROCESS_PA and ART_WF_ARTWORK_PROCESS_PG"
                        var processLastPATmp = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                            where p.MATERIAL_NO == item.REPEAT_SO_MATERIAL_NO                                              
                                            select p).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();


                       
                        if (processLastPATmp != null)  //found last process pa
                        {
                            int stepPGID = 0;
                            stepPGID = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == "SEND_PG")
                                                                    .Select(l => l.STEP_ARTWORK_ID)
                                                                    .FirstOrDefault();

                            var listAllSubID_LastPATmp = CNService.FindArtworkSubId(processLastPATmp.ARTWORK_SUB_ID, context);

                            var processPG = (from p in context.ART_WF_ARTWORK_PROCESS_PG
                                             where listAllSubID_LastPATmp.Contains(p.ARTWORK_SUB_ID)
                                             && p.ACTION_CODE == "SUBMIT"
                                             select p).OrderByDescending(o => o.ARTWORK_SUB_PA_ID).FirstOrDefault();

                            if (processPG != null)
                            {
                                if (processPG.DIE_LINE_MOCKUP_ID != null)
                                {
                                    ART_WF_ARTWORK_PROCESS_2 processNew = new ART_WF_ARTWORK_PROCESS_2();
                                    //processNew.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                                    //processNew.ARTWORK_ITEM_ID = process.ARTWORK_ITEM_ID;
                                    //processNew.PARENT_ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                    //processNew.PARENT_ARTWORK_SUB_ID = 
                                    processNew.CURRENT_USER_ID = -1;
                                    processNew.UPDATE_BY = -1;
                                    processNew.CREATE_BY = -1;
                                    processNew.IS_END = "X";
                                    processNew.CURRENT_STEP_ID = stepPGID;
                                  
                                   // ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdateNoLog(processNew, context);

                                    ART_WF_ARTWORK_PROCESS_PG_2 processPGNew = new ART_WF_ARTWORK_PROCESS_PG_2();
                                    //processPGNew.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                                    processPGNew.ACTION_CODE = "SUBMIT";
                                    //processPGNew.ARTWORK_SUB_ID = processNew.ARTWORK_SUB_ID;
                                    processPGNew.DIE_LINE_MOCKUP_ID = processPG.DIE_LINE_MOCKUP_ID;
                                    processPGNew.CREATE_BY = -1;
                                    processPGNew.UPDATE_BY = -1;
                                    // ART_WF_ARTWORK_PROCESS_PG_SERVICE.SaveOrUpdateNoLog(processPGNew, context);

                                    processNew.PROCESS_PG = processPGNew;
                                    item.PROCESS_STEP_PG = processNew;   // prepare

                       
                                    if (item.PROCESS_STEP_PA.PROCESS_PA != null)
                                    {
                                        int stepMOPGID = 0;
                                        stepMOPGID = context.ART_M_STEP_MOCKUP.Where(s => s.STEP_MOCKUP_CODE == "SEND_PG")
                                                                                .Select(l => l.STEP_MOCKUP_ID)
                                                                                .FirstOrDefault();

                                        var mockupProcess = (from m in context.ART_WF_MOCKUP_PROCESS
                                                             where m.MOCKUP_ID == processPG.DIE_LINE_MOCKUP_ID
                                                               && m.CURRENT_STEP_ID == stepMOPGID
                                                             select m).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                                        var mockupProcessPG = (from g in context.ART_WF_MOCKUP_PROCESS_PG
                                                               where g.MOCKUP_SUB_ID == mockupProcess.MOCKUP_SUB_ID
                                                               select g).OrderByDescending(o => o.MOCKUP_SUB_PG_ID).FirstOrDefault();

                                        item.PROCESS_STEP_PA.PROCESS_PA.PG_USER_ID = mockupProcessPG.UPDATE_BY;
                                        item.PROCESS_STEP_PA.PROCESS_PA.CHANGE_POINT = processLastPATmp.CHANGE_POINT;

                                        if (processLastPATmp.CHANGE_POINT == null)
                                        {
                                            var material_conversion = (from p in context.SAP_M_MATERIAL_CONVERSION
                                                                       where p.MATERIAL_NO == item.PROCESS_STEP_PA.PROCESS_PA.MATERIAL_NO
                                                                        && p.CHAR_NAME == "ZPKG_SEC_CHANGE_POINT"
                                                                       select p).FirstOrDefault();

                                            if (material_conversion != null)
                                            {
                                                if (material_conversion.CHAR_VALUE == "N")
                                                {
                                                    item.PROCESS_STEP_PA.PROCESS_PA.CHANGE_POINT = "0";
                                                }
                                                else
                                                {
                                                    item.PROCESS_STEP_PA.PROCESS_PA.CHANGE_POINT = "1";
                                                }
                                            }
                                        }

                                        //ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA2, context);
                                    }
                                }
                            }
                        }
                        else  //not found last process pa
                        {
                          

                            if (item.PROCESS_STEP_PA.PROCESS_PA != null)
                            {
                                if (item.PROCESS_STEP_PA.PROCESS_PA.CHANGE_POINT == null)
                                {
                                    var material_conversion = (from p in context.SAP_M_MATERIAL_CONVERSION
                                                               where p.MATERIAL_NO == item.PROCESS_STEP_PA.PROCESS_PA.MATERIAL_NO
                                                                && p.CHAR_NAME == "ZPKG_SEC_CHANGE_POINT"
                                                               select p).FirstOrDefault();

                                    if (material_conversion != null)
                                    {
                                        if (material_conversion.CHAR_VALUE == "N")
                                        {
                                            item.PROCESS_STEP_PA.PROCESS_PA.CHANGE_POINT = "0";
                                        }
                                        else
                                        {
                                            item.PROCESS_STEP_PA.PROCESS_PA.CHANGE_POINT = "1";
                                        }

                                        //ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA2, context);
                                    }
                                }
                            }
                        }
                        #endregion


                        #region "03.prepare Accept Task"

                        #endregion


                        #region "04.prepare IS_COMPLETE WF"
                        if (objReq.IS_COMPLETE)
                        {
                            item.PROCESS_STEP_PA.IS_END = "X";
                            //item.PROCESS_STEP_PA.REMARK_KILLPROCESS = "Completed workflow by PA";
                            ////#437016
                            //if (iProcess.CURRENT_STEP_ID == 2 && iProcess.IS_END == "X")
                            //    CNService.CompletePOForm(param, context);
                        }
                        #endregion

                        #region "05.copy and prepare PROCESS_PA"

                        ART_WF_ARTWORK_PROCESS_PA_2 processPA = item.PROCESS_STEP_PA.PROCESS_PA;
                       
                        msg =  PrepareRetriveMaterialRepeatBySoRepeat(ref processPA, context);

                        // start 20230121_3V_SOREPAT INC-93118
                        if (objReq.IS_VAP)
                        {
                            if (processPA.PRODUCTS != null && processPA.PRODUCTS.Count > 0 )
                            {
                                var plant_vap_id = (from m in context.SAP_M_PLANT where m.PLANT == "1021" select m.PLANT_ID).FirstOrDefault();  // VAP = 1021
                                var product_code = "";
                                foreach (var product in processPA.PRODUCTS)
                                {
                                    if ( product.PRODUCT_CODE_ID > 0 )
                                    {
                                        product_code = (from m in context.XECM_M_PRODUCT where m.XECM_PRODUCT_ID == product.PRODUCT_CODE_ID select m.PRODUCT_CODE).FirstOrDefault();
                                        if (!string.IsNullOrEmpty(product_code))
                                        {
                                            product.PRODUCT_TYPE = CNService.Getcheck_product_vap(product_code, plant_vap_id.ToString());
                                        }
                                      
                                    }
                                  
                                }
                            }
                        }
                        // end 20230121_3V_SOREPAT INC-93118

                        #endregion

                        #region  "06.parpare Send To PP"
                        if (objReq.IS_SEND_TO_PP)
                        {
                            if (item.PROCESS_STEP_PA != null)
                            {
                                var stepPP = (from s in context.ART_M_STEP_ARTWORK
                                              where s.STEP_ARTWORK_CODE == "SEND_PP"
                                              select s).FirstOrDefault();


                                item.PROCESS_STEP_PP = new ART_WF_ARTWORK_PROCESS_2();
                                //item.PROCESS_STEP_PP.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                                //item.PROCESS_STEP_PP.ARTWORK_ITEM_ID = process.ARTWORK_ITEM_ID;
                                //item.PROCESS_STEP_PP.PARENT_ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                                item.PROCESS_STEP_PP.CURRENT_ROLE_ID = stepPP.ROLE_ID_RESPONSE;
                                item.PROCESS_STEP_PP.CURRENT_STEP_ID = stepPP.STEP_ARTWORK_ID;
                                item.PROCESS_STEP_PP.REMARK = objReq.COMMENT;
                                item.PROCESS_STEP_PP.CREATE_BY = objReq.UPDATE_BY;
                                item.PROCESS_STEP_PP.UPDATE_BY = objReq.UPDATE_BY;


                                item.PROCESS_STEP_PP.PROCESS_PP_BY_PA = new ART_WF_ARTWORK_PROCESS_PP_BY_PA();
                                //item.PROCESS_STEP_PP.PROCESS_PP_BY_PA.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                                //item.PROCESS_STEP_PP.PROCESS_PP_BY_PA.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                                item.PROCESS_STEP_PP.PROCESS_PP_BY_PA.REQUEST_SHADE_LIMIT = "";
                                item.PROCESS_STEP_PP.PROCESS_PP_BY_PA.CREATE_BY = objReq.UPDATE_BY;
                                item.PROCESS_STEP_PP.PROCESS_PP_BY_PA.UPDATE_BY = objReq.UPDATE_BY;


                            }
                        }
                        #endregion


                    }

                }
            }
            catch (Exception ex)
            {
                msg = CNService.GetErrorMessage_SORepeat(ex, "PrepareArtworkItemProcessbySORepeat") + "{Log}";
            }

            return msg;
        }


        public static string PrepareRetriveMaterialRepeatBySoRepeat(ref ART_WF_ARTWORK_PROCESS_PA_2 processPA, ARTWORKEntities context)      
        {
            var msg = "";

            try
            {
                string material_no = processPA.MATERIAL_NO;
                var currentUserId = processPA.CREATE_BY;

                if (!String.IsNullOrEmpty(material_no))
                {


                    var processPAByMaterial_ALL = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                   where p.MATERIAL_NO == material_no
                                                   select p.ARTWORK_SUB_ID).ToList();

                    if (processPAByMaterial_ALL != null)
                    {
                        var processEnd = (from p in context.ART_WF_ARTWORK_PROCESS
                                          where processPAByMaterial_ALL.Contains(p.ARTWORK_SUB_ID)
                                          && p.IS_END == "X"
                                          && String.IsNullOrEmpty(p.IS_TERMINATE)
                                          select p).OrderByDescending(o => o.ARTWORK_SUB_ID).FirstOrDefault();

                        if (processEnd != null)
                        {
                            var processPAByMaterial = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                       where p.ARTWORK_SUB_ID == processEnd.ARTWORK_SUB_ID
                                                       select p).OrderByDescending(o => o.ARTWORK_SUB_ID).FirstOrDefault();

                            if (processPAByMaterial != null)
                            {
                                processPA.CHANGE_POINT = processPAByMaterial.CHANGE_POINT;
                                processPA.PA_USER_ID = processPAByMaterial.PA_USER_ID;
                                processPA.THREE_P_ID = processPAByMaterial.THREE_P_ID;
                                processPA.TWO_P_ID = processPAByMaterial.TWO_P_ID;
                                processPA.MATERIAL_GROUP_ID = processPAByMaterial.MATERIAL_GROUP_ID;
                                processPA.PRODUCT_CODE_ID = processPAByMaterial.PRODUCT_CODE_ID;
                                // processPA.PA_USER_ID = CNService.ConvertStringToInt(_header.PA_USER_NAME);
                                processPA.REFERENCE_MATERIAL = processPAByMaterial.REFERENCE_MATERIAL;
                                // processPA.PRINTING_STYLE_OF_PRIMARY_ID
                                // processPA.PRINTING_STYLE_OF_SECONDARY_ID
                                processPA.CUSTOMER_DESIGN = processPAByMaterial.CUSTOMER_DESIGN;
                                processPA.CUSTOMER_DESIGN_OTHER = processPAByMaterial.CUSTOMER_DESIGN_OTHER;
                                processPA.CUSTOMER_SPEC = processPAByMaterial.CUSTOMER_SPEC;
                                processPA.CUSTOMER_SPEC_OTHER = processPAByMaterial.CUSTOMER_SPEC_OTHER;
                                processPA.CUSTOMER_SIZE = processPAByMaterial.CUSTOMER_SIZE;
                                processPA.CUSTOMER_SIZE_OTHER = processPAByMaterial.CUSTOMER_SIZE_OTHER;
                                processPA.CUSTOMER_NOMINATES_VENDOR = processPAByMaterial.CUSTOMER_NOMINATES_VENDOR;
                                processPA.CUSTOMER_NOMINATES_VENDOR_OTHER = processPAByMaterial.CUSTOMER_NOMINATES_VENDOR_OTHER;
                                processPA.CUSTOMER_NOMINATES_COLOR = processPAByMaterial.CUSTOMER_NOMINATES_COLOR;
                                processPA.CUSTOMER_NOMINATES_COLOR_OTHER = processPAByMaterial.CUSTOMER_NOMINATES_COLOR_OTHER;
                                processPA.CUSTOMER_BARCODE_SCANABLE = processPAByMaterial.CUSTOMER_BARCODE_SCANABLE;
                                processPA.CUSTOMER_BARCODE_SCANABLE_OTHER = processPAByMaterial.CUSTOMER_BARCODE_SCANABLE_OTHER;
                                processPA.CUSTOMER_BARCODE_SPEC = processPAByMaterial.CUSTOMER_BARCODE_SPEC;
                                processPA.CUSTOMER_BARCODE_SPEC_OTHER = processPAByMaterial.CUSTOMER_BARCODE_SPEC_OTHER;
                                processPA.FIRST_INFOGROUP_OTHER = processPAByMaterial.FIRST_INFOGROUP_OTHER;
                                processPA.NOTE_OF_PA = processPAByMaterial.NOTE_OF_PA;
                                processPA.FIRST_INFOGROUP_OTHER = processPAByMaterial.FIRST_INFOGROUP_OTHER;
                                processPA.COMPLETE_INFOGROUP = processPAByMaterial.COMPLETE_INFOGROUP;
                                processPA.PRODUCTION_EXPIRY_DATE_SYSTEM = processPAByMaterial.PRODUCTION_EXPIRY_DATE_SYSTEM;
                                processPA.SERIOUSNESS_OF_COLOR_PRINTING = processPAByMaterial.SERIOUSNESS_OF_COLOR_PRINTING;
                                processPA.NUTRITION_ANALYSIS = processPAByMaterial.NUTRITION_ANALYSIS;
                                processPA.PACKAGE_QUANTITY = processPAByMaterial.PACKAGE_QUANTITY;
                                processPA.WASTE_PERCENT = processPAByMaterial.WASTE_PERCENT;
                                // processPA.REQUEST_MATERIAL_STATUS = processPAByMaterial.STATUS;
                                processPA.TYPE_OF_ID = processPAByMaterial.TYPE_OF_ID;
                                processPA.TYPE_OF_OTHER = processPAByMaterial.TYPE_OF_OTHER;
                                processPA.TYPE_OF_2_ID = processPAByMaterial.TYPE_OF_2_ID;
                                processPA.TYPE_OF_2_OTHER = processPAByMaterial.TYPE_OF_2_OTHER;
                                processPA.PMS_COLOUR_ID = processPAByMaterial.PMS_COLOUR_ID;
                                processPA.PMS_COLOUR_OTHER = processPAByMaterial.PMS_COLOUR_OTHER;
                                processPA.PROCESS_COLOUR_ID = processPAByMaterial.PROCESS_COLOUR_ID;
                                processPA.PROCESS_COLOUR_OTHER = processPAByMaterial.PROCESS_COLOUR_OTHER;
                                processPA.TOTAL_COLOUR_ID = processPAByMaterial.TOTAL_COLOUR_ID;
                                processPA.TOTAL_COLOUR_OTHER = processPAByMaterial.TOTAL_COLOUR_OTHER;
                                processPA.STYLE_OF_PRINTING_ID = processPAByMaterial.STYLE_OF_PRINTING_ID;
                                processPA.STYLE_OF_PRINTING_OTHER = processPAByMaterial.STYLE_OF_PRINTING_OTHER;
                                processPA.PRIMARY_SIZE_ID = processPAByMaterial.PRIMARY_SIZE_ID;
                                processPA.PRIMARY_SIZE_OTHER = processPAByMaterial.PRIMARY_SIZE_OTHER;
                                processPA.CONTAINER_TYPE_ID = processPAByMaterial.CONTAINER_TYPE_ID;
                                processPA.CONTAINER_TYPE_OTHER = processPAByMaterial.CONTAINER_TYPE_OTHER;
                                processPA.LID_TYPE_ID = processPAByMaterial.LID_TYPE_ID;
                                processPA.LID_TYPE_OTHER = processPAByMaterial.LID_TYPE_OTHER;
                                processPA.PLANT_REGISTERED_ID = processPAByMaterial.PLANT_REGISTERED_ID;
                                processPA.PLANT_REGISTERED_OTHER = processPAByMaterial.PLANT_REGISTERED_OTHER;
                                processPA.COMPANY_ADDRESS_ID = processPAByMaterial.COMPANY_ADDRESS_ID;
                                processPA.COMPANY_ADDRESS_OTHER = processPAByMaterial.COMPANY_ADDRESS_OTHER;
                                processPA.PRODICUTION_PLANT_ID = processPAByMaterial.PRODICUTION_PLANT_ID;  //------by aof 
                                processPA.PRODICUTION_PLANT_OTHER = processPAByMaterial.PRODICUTION_PLANT_OTHER;    //------by aof 
                                processPA.CATCHING_PERIOD_ID = processPAByMaterial.CATCHING_PERIOD_ID;
                                processPA.CATCHING_PERIOD_OTHER = processPAByMaterial.CATCHING_PERIOD_OTHER;
                                processPA.CATCHING_METHOD_ID = processPAByMaterial.CATCHING_METHOD_ID;
                                processPA.CATCHING_METHOD_OTHER = processPAByMaterial.CATCHING_METHOD_OTHER;
                                processPA.SCIENTIFIC_NAME_ID = processPAByMaterial.SCIENTIFIC_NAME_ID;
                                processPA.SCIENTIFIC_NAME_OTHER = processPAByMaterial.SCIENTIFIC_NAME_OTHER;
                                processPA.DIRECTION_OF_STICKER_ID = processPAByMaterial.DIRECTION_OF_STICKER_ID;
                                processPA.DIRECTION_OF_STICKER_OTHER = processPAByMaterial.DIRECTION_OF_STICKER_OTHER;
                                processPA.SPECIE_ID = processPAByMaterial.SPECIE_ID;
                                processPA.SPECIE_OTHER = processPAByMaterial.SPECIE_OTHER;
                                processPA.PRINTING_STYLE_OF_PRIMARY_ID = processPAByMaterial.PRINTING_STYLE_OF_PRIMARY_ID;
                                processPA.PRINTING_STYLE_OF_PRIMARY_OTHER = processPAByMaterial.PRINTING_STYLE_OF_PRIMARY_OTHER;
                                processPA.PRINTING_STYLE_OF_SECONDARY_ID = processPAByMaterial.PRINTING_STYLE_OF_SECONDARY_ID;
                                processPA.PRINTING_STYLE_OF_SECONDARY_OTHER = processPAByMaterial.PRINTING_STYLE_OF_SECONDARY_OTHER;
                                processPA.PIC_MKT = processPAByMaterial.PIC_MKT;
                                processPA.COURIER_NO = processPAByMaterial.COURIER_NO;
                                processPA.STYLE_OF_PRINTING_ID = processPAByMaterial.STYLE_OF_PRINTING_ID;
                                processPA.STYLE_OF_PRINTING_OTHER = processPAByMaterial.STYLE_OF_PRINTING_OTHER;
                                processPA.DIRECTION_OF_STICKER_ID = processPAByMaterial.DIRECTION_OF_STICKER_ID;
                                processPA.DIRECTION_OF_STICKER_OTHER = processPAByMaterial.DIRECTION_OF_STICKER_OTHER;

                               // ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);

                                #region "PLANT"
                              
                                var listPlants = (from p in context.ART_WF_ARTWORK_PROCESS_PA_PLANT
                                                  where p.ARTWORK_SUB_PA_ID == processPAByMaterial.ARTWORK_SUB_PA_ID
                                                  select p).ToList();

                                if (listPlants.Count > 0)
                                {
                                    processPA.PLANTS = new List<ART_WF_ARTWORK_PROCESS_PA_PLANT_2>();

                                    ART_WF_ARTWORK_PROCESS_PA_PLANT plantNew = new ART_WF_ARTWORK_PROCESS_PA_PLANT();
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_PLANT iPlant in listPlants)
                                    {
                                        plantNew = new ART_WF_ARTWORK_PROCESS_PA_PLANT_2();
                                        plantNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        plantNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        plantNew.PLANT_ID = iPlant.PLANT_ID;
                                        plantNew.PLANT_OTHER = iPlant.PLANT_OTHER;
                                        plantNew.CREATE_BY = currentUserId;
                                        plantNew.UPDATE_BY = currentUserId;
                                        processPA.PLANTS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_PLANT(plantNew));
                                       // ART_WF_ARTWORK_PROCESS_PA_PLANT_SERVICE.SaveOrUpdateNoLog(plantNew, context);
                                    }
                                }
                                #endregion

                                #region "FAO_ZONE"
                         

                                var faoNews = (from f in context.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE
                                               where f.ARTWORK_SUB_PA_ID == processPAByMaterial.ARTWORK_SUB_PA_ID
                                               select f).ToList();

                                if (faoNews != null && faoNews.Count > 0)
                                {
                                    processPA.FAOS = new List<ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_2>();

                                    ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE fao_new = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE iFAONew in faoNews)
                                    {
                                        fao_new = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                                        fao_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        fao_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        fao_new.FAO_ZONE_ID = iFAONew.FAO_ZONE_ID;
                                        fao_new.FAO_ZONE_OTHER = iFAONew.FAO_ZONE_OTHER;
                                        fao_new.CREATE_BY = currentUserId;
                                        fao_new.UPDATE_BY = currentUserId;
                                        processPA.FAOS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE(fao_new));
                                        //ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.SaveOrUpdateNoLog(fao_new, context);
                                    }
                                }

                                #endregion

                                #region "CATCHING_AREA"
                          

                                var catchingNews = (from c in context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA
                                                    where c.ARTWORK_SUB_PA_ID == processPAByMaterial.ARTWORK_SUB_PA_ID
                                                    select c).ToList();

                                if (catchingNews != null && catchingNews.Count > 0)
                                {
                                    processPA.CATCHING_AREAS = new List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_2>();

                                    ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA catching_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA iCatchingNew in catchingNews)
                                    {
                                        catching_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                                        catching_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        catching_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        catching_new.CATCHING_AREA_ID = iCatchingNew.CATCHING_AREA_ID;
                                        catching_new.CATCHING_AREA_OTHER = iCatchingNew.CATCHING_AREA_OTHER;
                                        catching_new.CREATE_BY = currentUserId;
                                        catching_new.UPDATE_BY = currentUserId;
                                        // ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE.SaveOrUpdateNoLog(catching_new, context);
                                        processPA.CATCHING_AREAS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA(catching_new));
                                    }
                                }

                                #endregion

                                #region "CATCHING_METHOD"
                                // ticke#425737 added by aof 
                  
                                var methodNews = (from c in context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD
                                                  where c.ARTWORK_SUB_PA_ID == processPAByMaterial.ARTWORK_SUB_PA_ID
                                                  select c).ToList();

                                if (methodNews != null && methodNews.Count > 0)
                                {
                                    processPA.CATCHING_METHODS = new List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_2>();
                                    ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD method_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD iMethodNew in methodNews)
                                    {
                                        method_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                                        method_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        method_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        method_new.CATCHING_METHOD_ID = iMethodNew.CATCHING_METHOD_ID;
                                        method_new.CATCHING_METHOD_OTHER = iMethodNew.CATCHING_METHOD_OTHER;
                                        method_new.CREATE_BY = currentUserId;
                                        method_new.UPDATE_BY = currentUserId;
                                        //ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE.SaveOrUpdateNoLog(method_new, context);
                                        processPA.CATCHING_METHODS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD(method_new));
                                    }
                                }
                                // ticke#425737 added by aof 
                                #endregion

                                #region "SYMBOL"
                   
                                var symbolNews = (from s in context.ART_WF_ARTWORK_PROCESS_PA_SYMBOL
                                                  where s.ARTWORK_SUB_PA_ID == processPAByMaterial.ARTWORK_SUB_PA_ID
                                                  select s).ToList();

                                if (symbolNews != null && symbolNews.Count > 0)
                                {
                                    processPA.SYMBOLS = new List<ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2>();
                                    ART_WF_ARTWORK_PROCESS_PA_SYMBOL symbol_new = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                                    foreach (var iSymbolNew in symbolNews)
                                    {
                                        symbol_new = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                                        symbol_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        symbol_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        symbol_new.SYMBOL_ID = iSymbolNew.SYMBOL_ID;
                                        symbol_new.SYMBOL_OTHER = iSymbolNew.SYMBOL_OTHER;
                                        symbol_new.CREATE_BY = currentUserId;
                                        symbol_new.UPDATE_BY = currentUserId;
                                        //ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE.SaveOrUpdateNoLog(symbol_new, context);
                                        processPA.SYMBOLS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_SYMBOL(symbol_new));
                                    }

                                }

                                #endregion

                                #region "PRODUCT"
                    
                                var listProduct = (from p in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT
                                                   where p.ARTWORK_SUB_PA_ID == processPAByMaterial.ARTWORK_SUB_PA_ID
                                                   select p).ToList();

                                if (listProduct.Count > 0)
                                {
                                    processPA.PRODUCTS = new List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2>();
                                    ART_WF_ARTWORK_PROCESS_PA_PRODUCT productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT iProduct in listProduct)
                                    {
                                        productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                                        productNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        productNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        productNew.PRODUCT_CODE_ID = iProduct.PRODUCT_CODE_ID;
                                        productNew.CREATE_BY = currentUserId;
                                        productNew.UPDATE_BY = currentUserId;
                                        //ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.SaveOrUpdateNoLog(productNew, context);
                                        processPA.PRODUCTS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_PRODUCT(productNew));
                                    }
                                }
                                #endregion

                                #region "PRODUCT OTHER"


                                var listProductOther = (from p in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER
                                                        where p.ARTWORK_SUB_PA_ID == processPAByMaterial.ARTWORK_SUB_PA_ID
                                                        select p).ToList();

                                if (listProductOther.Count > 0)
                                {
                                    processPA.PRODUCT_OTHERS = new List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2>();
                                    ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER iProduct in listProductOther)
                                    {
                                        productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();
                                        productNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        productNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        productNew.PRODUCT_CODE = iProduct.PRODUCT_CODE;
                                        productNew.NET_WEIGHT = iProduct.NET_WEIGHT;
                                        productNew.DRAINED_WEIGHT = iProduct.DRAINED_WEIGHT;
                                        productNew.CREATE_BY = currentUserId;
                                        productNew.UPDATE_BY = currentUserId;
                                        //ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.SaveOrUpdate(productNew, context);
                                        processPA.PRODUCT_OTHERS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER(productNew));
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                processPA.RETRIVE_TYPE = "RETRIVE";
                                msg = PrepareRetriveMaterialIGridBySoRepeat(ref processPA, context);
                            }
                        }
                        else
                        {
                            processPA.RETRIVE_TYPE = "RETRIVE";
                            msg = PrepareRetriveMaterialIGridBySoRepeat(ref processPA, context);
                        }
                    }
                    else
                    {
                        processPA.RETRIVE_TYPE = "RETRIVE";
                        msg = PrepareRetriveMaterialIGridBySoRepeat(ref processPA, context);
                    }

                    processPA.IS_RETRIEVE_BY_AW_REPEAT = "X";
                }
            }
            catch (Exception ex)
            {
                msg = CNService.GetErrorMessage_SORepeat(ex, "PrepareRetriveMaterialRepeatBySoRepeat");
            }
            return msg;
        }


        public static string PrepareRetriveMaterialIGridBySoRepeat(ref ART_WF_ARTWORK_PROCESS_PA_2 processPA, ARTWORKEntities context)
        {
            var msg = "";
            try
            {
              
                //ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();

                Dictionary<string, string> dicTypeOf = new Dictionary<string, string>();
                Dictionary<string, string> dicTypeOf2 = new Dictionary<string, string>();
                Dictionary<string, string> dicPlantRegister = new Dictionary<string, string>();
                Dictionary<string, string> dicPMSColour = new Dictionary<string, string>();
                Dictionary<string, string> dicCompany = new Dictionary<string, string>();
                Dictionary<string, string> dicProcessColour = new Dictionary<string, string>();
                Dictionary<string, string> dicCatchingPeriod = new Dictionary<string, string>();
                Dictionary<string, string> dicTotalColour = new Dictionary<string, string>();
                Dictionary<string, string> dicCatchingMethod = new Dictionary<string, string>();
                Dictionary<string, string> dicStyleOfPrinting = new Dictionary<string, string>();
                Dictionary<string, string> dicScientificName = new Dictionary<string, string>();
                Dictionary<string, string> dicDirectionOfSticker = new Dictionary<string, string>();
                Dictionary<string, string> dicSpecie = new Dictionary<string, string>();

                dicTypeOf.Add("C", "ZPKG_SEC_CARDBOARD_TYPE_1");
                dicTypeOf.Add("D", "ZPKG_SEC_DISPLAYER_TYPE_1");
                dicTypeOf.Add("F", "ZPKG_SEC_CARTON_TYPE_1");
                dicTypeOf.Add("G", "ZPKG_SEC_TRAY_TYPE");
                dicTypeOf.Add("H", "ZPKG_SEC_SLEEVE_BOX_TYPE");
                dicTypeOf.Add("J", "ZPKG_SEC_STICKER_TYPE");
                dicTypeOf.Add("K", "ZPKG_SEC_LABEL_TYPE");
                dicTypeOf.Add("L", "ZPKG_SEC_LEAFTLET_TYPE");
                dicTypeOf.Add("M", "ZPKG_SEC_STYLE_PLASTIC");
                dicTypeOf.Add("N", "ZPKG_SEC_INNER_TYPE_1");
                dicTypeOf.Add("P", "ZPKG_SEC_INSERT_TYPE");
                dicTypeOf.Add("R", "ZPKG_SEC_INNER_NON_TYPE");

                dicTypeOf2.Add("C", "ZPKG_SEC_CARDBOARD_TYPE_2");
                dicTypeOf2.Add("D", "ZPKG_SEC_DISPLAYER_TYPE_2");
                dicTypeOf2.Add("F", "ZPKG_SEC_CARTON_TYPE_2");
                dicTypeOf2.Add("G", "ZPKG_SEC_TRAY_CARTON_TYPE");
                dicTypeOf2.Add("N", "ZPKG_SEC_INNER_TYPE_2");
                dicTypeOf2.Add("R", "ZPKG_SEC_INNER_TYPE_2");

                dicPMSColour.Add("F", "ZPKG_SEC_CAR_PMS_COLOUR");
                dicPMSColour.Add("C", "ZPKG_SEC_CARD_PMS_COLOUR");
                dicPMSColour.Add("D", "ZPKG_SEC_DISP_PMS_COLOUR");
                dicPMSColour.Add("R", "ZPKG_SEC_INN_NO_PMS_COLOUR");
                dicPMSColour.Add("N", "ZPKG_SEC_INNER_PMS_COLOUR");
                dicPMSColour.Add("P", "ZPKG_SEC_INST_PMS_COLOUR");
                dicPMSColour.Add("K", "ZPKG_SEC_LABE_PMS_COLOUR");
                dicPMSColour.Add("L", "ZPKG_SEC_LEA_PMS_COLOUR");
                dicPMSColour.Add("H", "ZPKG_SEC_SLEV_PMS_COLOUR");
                dicPMSColour.Add("J", "ZPKG_SEC_STKC_PMS_COLOUR");
                dicPMSColour.Add("G", "ZPKG_SEC_TRAY_PMS_COLOUR");

                dicProcessColour.Add("F", "ZPKG_SEC_CAR_PROCESS_COLOUR");
                dicProcessColour.Add("C", "ZPKG_SEC_CARD_PROCESS_COLOUR");
                dicProcessColour.Add("D", "ZPKG_SEC_DISP_PROCESS_COLOUR");
                dicProcessColour.Add("R", "ZPKG_SEC_INN_NO_PROCESS_COLOUR");
                dicProcessColour.Add("N", "ZPKG_SEC_INNER_PROCESS_COLOUR");
                dicProcessColour.Add("P", "ZPKG_SEC_INST_PROCESS_COLOUR");
                dicProcessColour.Add("K", "ZPKG_SEC_LABE_PROCESS_COLOUR");
                dicProcessColour.Add("L", "ZPKG_SEC_LEA_PROCESS_COLOUR");
                dicProcessColour.Add("H", "ZPKG_SEC_SLEV_PROCESS_COLOUR");
                dicProcessColour.Add("J", "ZPKG_SEC_STKC_PROCESS_COLOUR");
                dicProcessColour.Add("G", "ZPKG_SEC_TRAY_PROCESS_COLOUR");

                dicTotalColour.Add("F", "ZPKG_SEC_CAR_TOTAL_COLOUR");
                dicTotalColour.Add("C", "ZPKG_SEC_CARD_TOTAL_COLOUR");
                dicTotalColour.Add("D", "ZPKG_SEC_DISP_TOTAL_COLOUR");
                dicTotalColour.Add("R", "ZPKG_SEC_INN_NO_TOTAL_COLOUR");
                dicTotalColour.Add("N", "ZPKG_SEC_INNER_TOTAL_COLOUR");
                dicTotalColour.Add("P", "ZPKG_SEC_INST_TOTAL_COLOUR");
                dicTotalColour.Add("K", "ZPKG_SEC_LABE_TOTAL_COLOUR");
                dicTotalColour.Add("L", "ZPKG_SEC_LEA_TOTAL_COLOUR");
                dicTotalColour.Add("H", "ZPKG_SEC_SLEV_TOTAL_COLOUR");
                dicTotalColour.Add("J", "ZPKG_SEC_STKC_TOTAL_COLOUR");
                dicTotalColour.Add("G", "ZPKG_SEC_TRAY_TOTAL_COLOUR");
                dicTotalColour.Add("M", "ZPKG_SEC_PLAST_TOTAL_COLOUR");

                dicStyleOfPrinting.Add("F", "ZPKG_SEC_CAR_PRINTING_STYLE");
                dicStyleOfPrinting.Add("C", "ZPKG_SEC_CARD_PRINTING_STYLE");
                dicStyleOfPrinting.Add("D", "ZPKG_SEC_DISP_PRINTING_STYLE");
                dicStyleOfPrinting.Add("R", "ZPKG_SEC_INN_NO_PRINTING_STYLE");
                dicStyleOfPrinting.Add("N", "ZPKG_SEC_INNER_PRINTING_STYLE");
                dicStyleOfPrinting.Add("P", "ZPKG_SEC_INST_PRINTING_STYLE");
                dicStyleOfPrinting.Add("K", "ZPKG_SEC_LABE_PRINTING_STYLE");
                dicStyleOfPrinting.Add("L", "ZPKG_SEC_LEA_PRINTING_STYLE");
                dicStyleOfPrinting.Add("H", "ZPKG_SEC_SLEV_PRINTING_STYLE");
                dicStyleOfPrinting.Add("J", "ZPKG_SEC_STKC_PRINTING_STYLE");
                dicStyleOfPrinting.Add("G", "ZPKG_SEC_TRAY_PRINTING_STYLE");

                //ART_WF_ARTWORK_PROCESS_PA processPA = new ART_WF_ARTWORK_PROCESS_PA();

                string material_no = processPA.MATERIAL_NO;
                var currentUserId = processPA.CREATE_BY;

                if ( !String.IsNullOrEmpty(material_no))
                {
                    //string artwork_no = param.data.ARTWORK_NO;

                    var header = (from h in context.IGRID_M_OUTBOUND_HEADER
                                  where h.MATERIAL_NUMBER == material_no
                                  select h).OrderByDescending(o => o.IGRID_OUTBOUND_HEADER_ID).FirstOrDefault();

                    //processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                    //             where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                    //             select p).OrderByDescending(o => o.ARTWORK_SUB_PA_ID).FirstOrDefault();

                    if (header != null)
                    {
                        var items = (from h in context.IGRID_M_OUTBOUND_ITEM
                                     where h.ARTWORK_NO == header.ARTWORK_NO
                                        && h.DATE == header.DATE
                                        && h.TIME == header.TIME
                                     select h).ToList();

                        if (items != null && items.Count() > 0)
                        {
                            processPA.MATERIAL_NO = material_no;
                            processPA.CUSTOMER_DESIGN = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_DESIGN);
                            processPA.CUSTOMER_DESIGN_OTHER = header.CUSTOMER_DESIGN_DETAIL;
                            processPA.CUSTOMER_SPEC = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_SPEC);
                            processPA.CUSTOMER_SPEC_OTHER = header.CUSTOMER_SPEC_DETAIL;
                            processPA.CUSTOMER_SIZE = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_SIZE);
                            processPA.CUSTOMER_SIZE_OTHER = header.CUSTOMER_SIZE_DETAIL;
                            processPA.CUSTOMER_NOMINATES_VENDOR = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_NOMINATES_VENDOR);
                            processPA.CUSTOMER_NOMINATES_VENDOR_OTHER = header.CUSTOMER_NOMINATES_VENDOR_DETAIL;
                            processPA.CUSTOMER_NOMINATES_COLOR = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_NOMINATES_COLOR);
                            processPA.CUSTOMER_NOMINATES_COLOR_OTHER = header.CUSTOMER_NOMINATES_COLOR_DETAIL;
                            processPA.CUSTOMER_BARCODE_SCANABLE = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_BARCODE_SCANABLE);
                            processPA.CUSTOMER_BARCODE_SCANABLE_OTHER = header.CUSTOMER_BARCODE_SCANABLE_DETAIL;
                            processPA.CUSTOMER_BARCODE_SPEC = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_BARCODE_SPEC);
                            processPA.CUSTOMER_BARCODE_SPEC_OTHER = header.CUSTOMER_BARCODE_SPEC_DETAIL;
                            processPA.FIRST_INFOGROUP_OTHER = header.FIRST_INFO_GROUP;
                            processPA.NOTE_OF_PA = header.NOTE_OF_PA;
                            processPA.FIRST_INFOGROUP_OTHER = header.FINAL_INFO_GROUP;
                            processPA.COMPLETE_INFOGROUP = header.COMPLETE_INFO_GROUP;
                            processPA.PRODUCTION_EXPIRY_DATE_SYSTEM = header.EXPIRY_DATE_SYSTEM;
                            processPA.SERIOUSNESS_OF_COLOR_PRINTING = MaterialIGridHelper.ConvertYesNoValue(header.SERIOUSNESS_OF_COLOR_PRINTING);
                            processPA.NUTRITION_ANALYSIS = MaterialIGridHelper.ConvertYesNoValue(header.ANALYSIS);
                            processPA.PACKAGE_QUANTITY = header.PACKAGE_QUANTITY.ToString();
                            processPA.WASTE_PERCENT = header.WASTE_PERCENT.ToString();
                            processPA.REQUEST_MATERIAL_STATUS = header.STATUS;

                            if (processPA.RETRIVE_TYPE == "RETRIVE")
                            {
                                processPA.REQUEST_MATERIAL_STATUS = "Completed";
                            }

                            var ZPKG_SEC_GROUP = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_GROUP", context);
                            if (ZPKG_SEC_GROUP != null)
                            {
                                processPA.MATERIAL_GROUP_ID = ZPKG_SEC_GROUP;

                                var matG = context.SAP_M_CHARACTERISTIC.Where(m => m.CHARACTERISTIC_ID == ZPKG_SEC_GROUP).FirstOrDefault();

                                string typeOf = "";// dicTypeOf[matG.VALUE];
                                if (dicTypeOf.TryGetValue(matG.VALUE, out typeOf))
                                {
                                    typeOf = dicTypeOf[matG.VALUE];
                                }

                                string typeOf2 = "";// dicTypeOf2[matG.VALUE];
                                if (dicTypeOf2.TryGetValue(matG.VALUE, out typeOf2))
                                {
                                    typeOf2 = dicTypeOf2[matG.VALUE];
                                }

                                string pmsColour = "";// dicPMSColour[matG.VALUE];
                                if (dicPMSColour.TryGetValue(matG.VALUE, out pmsColour))
                                {
                                    pmsColour = dicPMSColour[matG.VALUE];
                                }

                                string processColour = "";// dicProcessColour[matG.VALUE];
                                if (dicProcessColour.TryGetValue(matG.VALUE, out processColour))
                                {
                                    processColour = dicProcessColour[matG.VALUE];
                                }

                                string totalColour = "";// dicTotalColour[matG.VALUE];
                                if (dicTotalColour.TryGetValue(matG.VALUE, out totalColour))
                                {
                                    totalColour = dicTotalColour[matG.VALUE];
                                }

                                string stylePrinting = "";// dicStyleOfPrinting[matG.VALUE];
                                if (dicStyleOfPrinting.TryGetValue(matG.VALUE, out stylePrinting))
                                {
                                    stylePrinting = dicStyleOfPrinting[matG.VALUE];
                                }

                                if (!String.IsNullOrEmpty(typeOf))
                                {
                                    var TYPE_OF = MaterialIGridHelper.GetPACharacteristicID(items, typeOf, context);
                                    if (TYPE_OF != null)
                                    {
                                        processPA.TYPE_OF_ID = TYPE_OF;
                                    }
                                    else
                                    {
                                        processPA.TYPE_OF_ID = null;
                                        processPA.TYPE_OF_OTHER = null;
                                    }
                                }

                                if (!String.IsNullOrEmpty(typeOf2))
                                {
                                    var TYPE_OF_2 = MaterialIGridHelper.GetPACharacteristicID(items, typeOf2, context);
                                    if (TYPE_OF_2 != null)
                                    {
                                        processPA.TYPE_OF_2_ID = TYPE_OF_2;
                                    }
                                    else
                                    {
                                        processPA.TYPE_OF_2_ID = null;
                                        processPA.TYPE_OF_2_OTHER = null;
                                    }
                                }

                                if (!String.IsNullOrEmpty(pmsColour))
                                {
                                    var PMS_COLOUR = MaterialIGridHelper.GetPACharacteristicID(items, pmsColour, context);
                                    if (PMS_COLOUR != null)
                                    {
                                        processPA.PMS_COLOUR_ID = PMS_COLOUR;
                                    }
                                    else
                                    {
                                        processPA.PMS_COLOUR_ID = null;
                                        processPA.PMS_COLOUR_OTHER = null;
                                    }
                                }

                                if (!String.IsNullOrEmpty(processColour))
                                {
                                    var PROCESS_COLOUR = MaterialIGridHelper.GetPACharacteristicID(items, processColour, context);
                                    if (PROCESS_COLOUR != null)
                                    {
                                        processPA.PROCESS_COLOUR_ID = PROCESS_COLOUR;
                                    }
                                    else
                                    {
                                        processPA.PROCESS_COLOUR_ID = null;
                                        processPA.PROCESS_COLOUR_OTHER = null;
                                    }
                                }

                                if (!String.IsNullOrEmpty(totalColour))
                                {
                                    var TOTAL_COLOUR = MaterialIGridHelper.GetPACharacteristicID(items, totalColour, context);
                                    if (TOTAL_COLOUR != null)
                                    {
                                        processPA.TOTAL_COLOUR_ID = TOTAL_COLOUR;
                                    }
                                    else
                                    {
                                        processPA.TOTAL_COLOUR_ID = null;
                                        processPA.TOTAL_COLOUR_OTHER = null;
                                    }
                                }

                                if (!String.IsNullOrEmpty(stylePrinting))
                                {
                                    var STYLE_PRINTING = MaterialIGridHelper.GetPACharacteristicID(items, stylePrinting, context);
                                    if (STYLE_PRINTING != null)
                                    {
                                        processPA.STYLE_OF_PRINTING_ID = STYLE_PRINTING;
                                    }
                                    else
                                    {
                                        processPA.STYLE_OF_PRINTING_ID = null;
                                        processPA.STYLE_OF_PRINTING_OTHER = null;
                                    }
                                }
                            }

                            var ZPKG_SEC_PRIMARY_SIZE_VALUE = MaterialIGridHelper.GetIgridItem(items, "ZPKG_SEC_PRIMARY_SIZE", context);
                            var ZPKG_SEC_CONTAINER_TYPE_VALUE = MaterialIGridHelper.GetIgridItem(items, "ZPKG_SEC_CONTAINER_TYPE", context);
                            var ZPKG_SEC_LID_TYPE_VALUE = MaterialIGridHelper.GetIgridItem(items, "ZPKG_SEC_LID_TYPE", context);

                            if ((ZPKG_SEC_PRIMARY_SIZE_VALUE != null && !String.IsNullOrEmpty(ZPKG_SEC_PRIMARY_SIZE_VALUE.CHARACTERISTIC_DESCRIPTION))
                               && (ZPKG_SEC_CONTAINER_TYPE_VALUE != null && !String.IsNullOrEmpty(ZPKG_SEC_CONTAINER_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION))
                               && (ZPKG_SEC_LID_TYPE_VALUE != null && !String.IsNullOrEmpty(ZPKG_SEC_LID_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION)))
                            {
                                var threeP = (from p in context.SAP_M_3P
                                              where p.PRIMARY_SIZE_VALUE == ZPKG_SEC_PRIMARY_SIZE_VALUE.CHARACTERISTIC_DESCRIPTION
                                              && p.CONTAINER_TYPE_VALUE == ZPKG_SEC_CONTAINER_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION
                                              && p.LID_TYPE_VALUE == ZPKG_SEC_LID_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION
                                              select p).FirstOrDefault();

                                if (threeP != null)
                                {
                                    processPA.THREE_P_ID = threeP.THREE_P_ID;
                                }
                                else
                                {
                                    processPA.THREE_P_ID = null;
                                    processPA.PRIMARY_SIZE_ID = null;
                                    processPA.CONTAINER_TYPE_ID = null;
                                    processPA.LID_TYPE_ID = null;

                                    processPA.PRIMARY_SIZE_OTHER = null;
                                    processPA.CONTAINER_TYPE_OTHER = null;
                                    processPA.LID_TYPE_OTHER = null;

                                    var ZPKG_SEC_PRIMARY_SIZE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PRIMARY_SIZE", context);
                                    if (ZPKG_SEC_PRIMARY_SIZE != null)
                                    {
                                        processPA.PRIMARY_SIZE_ID = ZPKG_SEC_PRIMARY_SIZE;
                                    }

                                    var ZPKG_SEC_CONTAINER_TYPE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_CONTAINER_TYPE", context);
                                    if (ZPKG_SEC_CONTAINER_TYPE != null)
                                    {
                                        processPA.CONTAINER_TYPE_ID = ZPKG_SEC_CONTAINER_TYPE;
                                    }

                                    var ZPKG_SEC_LID_TYPE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_LID_TYPE", context);
                                    if (ZPKG_SEC_LID_TYPE != null)
                                    {
                                        processPA.LID_TYPE_ID = ZPKG_SEC_LID_TYPE;
                                    }
                                }
                            }
                            else
                            {
                                processPA.THREE_P_ID = null;
                                processPA.PRIMARY_SIZE_ID = null;
                                processPA.CONTAINER_TYPE_ID = null;
                                processPA.LID_TYPE_ID = null;

                                processPA.PRIMARY_SIZE_OTHER = null;
                                processPA.CONTAINER_TYPE_OTHER = null;
                                processPA.LID_TYPE_OTHER = null;

                                var ZPKG_SEC_PRIMARY_SIZE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PRIMARY_SIZE", context);
                                if (ZPKG_SEC_PRIMARY_SIZE != null)
                                {
                                    processPA.PRIMARY_SIZE_ID = ZPKG_SEC_PRIMARY_SIZE;
                                }

                                var ZPKG_SEC_CONTAINER_TYPE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_CONTAINER_TYPE", context);
                                if (ZPKG_SEC_CONTAINER_TYPE != null)
                                {
                                    processPA.CONTAINER_TYPE_ID = ZPKG_SEC_CONTAINER_TYPE;
                                }

                                var ZPKG_SEC_LID_TYPE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_LID_TYPE", context);
                                if (ZPKG_SEC_LID_TYPE != null)
                                {
                                    processPA.LID_TYPE_ID = ZPKG_SEC_LID_TYPE;
                                }
                            }

                            var ZPKG_SEC_PACKING_STYLE_VALUE = MaterialIGridHelper.GetIgridItem(items, "ZPKG_SEC_PACKING_STYLE", context);
                            var ZPKG_SEC_PACKING_VALUE = MaterialIGridHelper.GetIgridItem(items, "ZPKG_SEC_PACKING", context);

                            if ((ZPKG_SEC_PACKING_STYLE_VALUE != null && !String.IsNullOrEmpty(ZPKG_SEC_PACKING_STYLE_VALUE.CHARACTERISTIC_DESCRIPTION))
                               && (ZPKG_SEC_PACKING_VALUE != null && !String.IsNullOrEmpty(ZPKG_SEC_PACKING_VALUE.CHARACTERISTIC_DESCRIPTION)))
                            {
                                var twoP = (from p in context.SAP_M_2P
                                            where p.PACKING_SYLE_VALUE == ZPKG_SEC_PACKING_STYLE_VALUE.CHARACTERISTIC_DESCRIPTION
                                            && p.PACK_SIZE_VALUE == ZPKG_SEC_PACKING_VALUE.CHARACTERISTIC_DESCRIPTION
                                            select p).FirstOrDefault();

                                if (twoP != null)
                                {
                                    processPA.TWO_P_ID = twoP.TWO_P_ID;
                                }
                                else
                                {
                                    processPA.TWO_P_ID = null;
                                    processPA.PACKING_STYLE_ID = null;
                                    processPA.PACK_SIZE_ID = null;

                                    processPA.PACKING_STYLE_OTHER = null;
                                    processPA.PACK_SIZE_OTHER = null;

                                    var ZPKG_SEC_PACKING_STYLE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PACKING_STYLE", context);
                                    if (ZPKG_SEC_PACKING_STYLE != null)
                                    {
                                        processPA.PACKING_STYLE_ID = ZPKG_SEC_PACKING_STYLE;
                                    }

                                    var ZPKG_SEC_PACKING = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PACKING", context);
                                    if (ZPKG_SEC_PACKING != null)
                                    {
                                        processPA.PACK_SIZE_ID = ZPKG_SEC_PACKING;
                                    }

                                }
                            }
                            else
                            {
                                processPA.TWO_P_ID = null;
                                processPA.PACKING_STYLE_ID = null;
                                processPA.PACK_SIZE_ID = null;

                                processPA.PACKING_STYLE_OTHER = null;
                                processPA.PACK_SIZE_OTHER = null;

                                var ZPKG_SEC_PACKING_STYLE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PACKING_STYLE", context);
                                if (ZPKG_SEC_PACKING_STYLE != null)
                                {
                                    processPA.PACKING_STYLE_ID = ZPKG_SEC_PACKING_STYLE;
                                }

                                var ZPKG_SEC_PACKING = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PACKING", context);
                                if (ZPKG_SEC_PACKING != null)
                                {
                                    processPA.PACK_SIZE_ID = ZPKG_SEC_PACKING;
                                }
                            }

                            var ZPKG_SEC_PLANT_REGISTER = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PLANT_REGISTER", context);
                            if (ZPKG_SEC_PLANT_REGISTER != null)
                            {
                                processPA.PLANT_REGISTERED_ID = ZPKG_SEC_PLANT_REGISTER;
                            }
                            else
                            {
                                processPA.PLANT_REGISTERED_ID = null;
                                processPA.PLANT_REGISTERED_OTHER = null;
                            }

                            var ZPKG_SEC_COMPANY_ADR = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_COMPANY_ADR", context);
                            if (ZPKG_SEC_COMPANY_ADR != null)
                            {
                                processPA.COMPANY_ADDRESS_ID = ZPKG_SEC_COMPANY_ADR;
                            }
                            else
                            {
                                processPA.COMPANY_ADDRESS_ID = null;
                                processPA.COMPANY_ADDRESS_OTHER = null;
                            }

                            var ZPKG_SEC_CATCHING_PERIOD = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_CATCHING_PERIOD", context);
                            if (ZPKG_SEC_CATCHING_PERIOD != null)
                            {
                                processPA.CATCHING_PERIOD_ID = ZPKG_SEC_CATCHING_PERIOD;
                            }
                            else
                            {
                                processPA.CATCHING_PERIOD_ID = null;
                                processPA.CATCHING_PERIOD_OTHER = null;
                            }

                            var ZPKG_SEC_CATCHING_METHOD = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_CATCHING_METHOD", context);
                            if (ZPKG_SEC_CATCHING_METHOD != null)
                            {
                                processPA.CATCHING_METHOD_ID = ZPKG_SEC_CATCHING_METHOD;
                            }
                            else
                            {
                                processPA.CATCHING_METHOD_ID = null;
                                processPA.CATCHING_METHOD_OTHER = null;
                            }

                            var ZPKG_SEC_SCIENTIFIC_NAME = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_SCIENTIFIC_NAME", context);
                            if (ZPKG_SEC_SCIENTIFIC_NAME != null)
                            {
                                processPA.SCIENTIFIC_NAME_ID = ZPKG_SEC_SCIENTIFIC_NAME;
                            }
                            else
                            {
                                processPA.SCIENTIFIC_NAME_ID = null;
                                processPA.SCIENTIFIC_NAME_OTHER = null;
                            }

                            var ZPKG_SEC_DIRECTION = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_DIRECTION", context);
                            if (ZPKG_SEC_DIRECTION != null)
                            {
                                processPA.DIRECTION_OF_STICKER_ID = ZPKG_SEC_DIRECTION;
                            }

                            var ZPKG_SEC_SPECIE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_SPECIE", context);
                            if (ZPKG_SEC_SPECIE != null)
                            {
                                processPA.SPECIE_ID = ZPKG_SEC_SPECIE;
                            }
                            else
                            {
                                processPA.SPECIE_ID = null;
                                processPA.SPECIE_OTHER = null;
                            }

                            //if (param.data.RETRIVE_TYPE == "SUGGEST")
                            //{
                            //    processPA.MATERIAL_NO = "";
                            //    processPA.REQUEST_MATERIAL_STATUS = "";
                            //}

                            var productionPlants = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_PRODUCTION_PLANT").ToList();

                            if (productionPlants != null)
                            {
                                string productionPlant = "";
                                foreach (var iProductPlant in productionPlants)
                                {
                                    if (!String.IsNullOrEmpty(iProductPlant.CHARACTERISTIC_VALUE) && !productionPlant.Contains(iProductPlant.CHARACTERISTIC_VALUE))
                                    {
                                        if (String.IsNullOrEmpty(productionPlant))
                                        {
                                            productionPlant = iProductPlant.CHARACTERISTIC_VALUE;
                                        }
                                        else
                                        {
                                            productionPlant += "," + iProductPlant.CHARACTERISTIC_VALUE;
                                        }
                                    }
                                }

                                if (!String.IsNullOrEmpty(productionPlant))
                                {
                                    processPA.PRODICUTION_PLANT_ID = -1;
                                    processPA.PRODICUTION_PLANT_OTHER = productionPlant;
                                }
                            }
                            else
                            {
                                processPA.PRODICUTION_PLANT_ID = null;
                                processPA.PRODICUTION_PLANT_OTHER = null;
                            }

                           // ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA, context);

                            #region "PLANT"
                      

                            if (!String.IsNullOrEmpty(header.PLANT))
                            {
                                List<string> listPlants = new List<string>();

                                listPlants = header.PLANT.Split(new string[] { ";" }, StringSplitOptions.None).ToList();

                                if (listPlants.Count > 0)
                                {
                                    processPA.PLANTS = new List<ART_WF_ARTWORK_PROCESS_PA_PLANT_2>();
                                    ART_WF_ARTWORK_PROCESS_PA_PLANT plantNew = new ART_WF_ARTWORK_PROCESS_PA_PLANT();
                                    foreach (string iPlant in listPlants)
                                    {

                                        var plantTmp = (from p in context.SAP_M_PLANT
                                                        where p.PLANT == iPlant
                                                        select p).FirstOrDefault();

                                        if (plantTmp != null)
                                        {
                                            plantNew = new ART_WF_ARTWORK_PROCESS_PA_PLANT();
                                            plantNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                            plantNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                            plantNew.PLANT_ID = plantTmp.PLANT_ID;
                                            plantNew.CREATE_BY = currentUserId;
                                            plantNew.UPDATE_BY = currentUserId;
                                            //ART_WF_ARTWORK_PROCESS_PA_PLANT_SERVICE.SaveOrUpdateNoLog(plantNew, context);
                                            processPA.PLANTS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_PLANT(plantNew));
                                        }
                                    }
                                }
                            }


                            #endregion

                            #region "FAO_ZONE"
                         
                            var faoNews = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_FAO").ToList();
                            if (faoNews != null && faoNews.Count > 0)
                            {
                                processPA.FAOS = new List<ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_2>();
                                ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE fao_new = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                                foreach (var iFAONew in faoNews)
                                {
                                    var fap_tmp = (from c in context.SAP_M_CHARACTERISTIC
                                                   where c.NAME == "ZPKG_SEC_FAO"
                                                     && c.VALUE == iFAONew.CHARACTERISTIC_VALUE
                                                   select c).FirstOrDefault();

                                    if (fap_tmp != null)
                                    {
                                        fao_new = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                                        fao_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        fao_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        fao_new.FAO_ZONE_ID = fap_tmp.CHARACTERISTIC_ID;
                                        fao_new.CREATE_BY = currentUserId;
                                        fao_new.UPDATE_BY = currentUserId;
                                        //ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.SaveOrUpdateNoLog(fao_new, context);
                                        processPA.FAOS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE(fao_new));
                                    }
                                }
                            }

                            #endregion

                            #region "CATCHING_AREA"
                          
                            var catchingNews = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_CATCHING_AREA").ToList();

                            if (catchingNews != null && catchingNews.Count > 0)
                            {
                                processPA.CATCHING_AREAS = new List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_2>();
                                ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA catching_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                                foreach (var iCatchingNew in catchingNews)
                                {
                                    var catching_tmp = (from c in context.SAP_M_CHARACTERISTIC
                                                        where c.NAME == "ZPKG_SEC_CATCHING_AREA"
                                                          && c.VALUE == iCatchingNew.CHARACTERISTIC_VALUE
                                                        select c).FirstOrDefault();

                                    if (catching_tmp != null)
                                    {
                                        catching_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                                        catching_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        catching_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        catching_new.CATCHING_AREA_ID = catching_tmp.CHARACTERISTIC_ID;
                                        catching_new.CREATE_BY = currentUserId;
                                        catching_new.UPDATE_BY = currentUserId;
                                        // ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE.SaveOrUpdateNoLog(catching_new, context);
                                        processPA.CATCHING_AREAS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA(catching_new));
                                    }
                                }
                            }

                            #endregion

                            #region "CATCHING_METHOD"
                            // ticke#425737 added by aof 
                           
                            var methodNews = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_CATCHING_METHOD").ToList();

                            if (methodNews != null && methodNews.Count > 0)
                            {
                                processPA.CATCHING_METHODS = new List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_2>();
                                ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD method_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                                foreach (var iMethodNew in methodNews)
                                {
                                    var method_tmp = (from c in context.SAP_M_CHARACTERISTIC
                                                      where c.NAME == "ZPKG_SEC_CATCHING_METHOD"
                                                        && c.VALUE == iMethodNew.CHARACTERISTIC_VALUE
                                                      select c).FirstOrDefault();

                                    if (method_tmp != null)
                                    {
                                        method_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                                        method_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        method_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        method_new.CATCHING_METHOD_ID = method_tmp.CHARACTERISTIC_ID;
                                        method_new.CREATE_BY = currentUserId;
                                        method_new.UPDATE_BY = currentUserId;
                                        //ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE.SaveOrUpdateNoLog(method_new, context);
                                        processPA.CATCHING_METHODS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD(method_new));
                                    }
                                }
                            }
                            // ticke#425737 added by aof 
                            #endregion

                            #region "SYMBOL"
                            var symbolNews = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_SYMBOL").ToList();

                            if (symbolNews != null && symbolNews.Count > 0)
                            {
                                processPA.SYMBOLS = new List<ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2>();
                                ART_WF_ARTWORK_PROCESS_PA_SYMBOL symbol_new = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                                foreach (var iSymbolNew in symbolNews)
                                {
                                    var symbol_tmp = (from c in context.SAP_M_CHARACTERISTIC
                                                      where c.NAME == "ZPKG_SEC_SYMBOL"
                                                        && c.VALUE == iSymbolNew.CHARACTERISTIC_VALUE
                                                      select c).FirstOrDefault();

                                    if (symbol_tmp != null)
                                    {
                                        symbol_new = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                                        symbol_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        symbol_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        symbol_new.SYMBOL_ID = symbol_tmp.CHARACTERISTIC_ID;
                                        symbol_new.CREATE_BY = currentUserId;
                                        symbol_new.UPDATE_BY = currentUserId;
                                        //ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE.SaveOrUpdateNoLog(symbol_new, context);
                                        processPA.SYMBOLS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_SYMBOL(symbol_new));
                                    }
                                }
                            }

                            #endregion

                            #region "PRODUCT"
               

                            var listProduct = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_PRODUCT_CODE").ToList();
                            if (listProduct != null)
                            {
                                var listProductCode = listProduct.Select(s => s.CHARACTERISTIC_VALUE).ToList();

                                if (listProductCode != null)
                                {
                                    var xProduct = (from p in context.XECM_M_PRODUCT
                                                    where listProductCode.Contains(p.PRODUCT_CODE)
                                                    select p).ToList();

                                    processPA.PRODUCTS = new List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2>();
                                    processPA.PRODUCT_OTHERS = new List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2>();

                                    ART_WF_ARTWORK_PROCESS_PA_PRODUCT productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                                    foreach (IGRID_M_OUTBOUND_ITEM iProduct in listProduct)
                                    {
                                        int productID = xProduct.Where(w => w.PRODUCT_CODE == iProduct.CHARACTERISTIC_VALUE)
                                                                .Select(s => s.XECM_PRODUCT_ID).FirstOrDefault();

                                        if (productID > 0)
                                        {
                                            productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                                            productNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                            productNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                            productNew.PRODUCT_CODE_ID = productID;
                                            productNew.CREATE_BY = currentUserId;
                                            productNew.UPDATE_BY = currentUserId;
                                            //ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.SaveOrUpdateNoLog(productNew, context);
                                            processPA.PRODUCTS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_PRODUCT(productNew));
                                        }
                                        else
                                        {
                                            ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER productOtherNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();

                                            productOtherNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();
                                            productOtherNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                            productOtherNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                            productOtherNew.PRODUCT_CODE = iProduct.CHARACTERISTIC_VALUE;
                                            productOtherNew.CREATE_BY = currentUserId;
                                            productOtherNew.UPDATE_BY = currentUserId;
                                            //ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.SaveOrUpdateNoLog(productOtherNew, context);
                                            processPA.PRODUCT_OTHERS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER(productOtherNew));
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            msg = PrepareDataConversionBySoRepeat(ref processPA, context, dicTypeOf, dicTypeOf2, dicPMSColour, dicProcessColour, dicStyleOfPrinting,  dicTotalColour);
                        }
                    }
                    else
                    {
                        msg = PrepareDataConversionBySoRepeat(ref processPA, context, dicTypeOf, dicTypeOf2, dicPMSColour, dicProcessColour, dicStyleOfPrinting, dicTotalColour);
                    }

                }
            }
            catch (Exception ex)
            {
                msg = CNService.GetErrorMessage_SORepeat(ex, "PrepareRetriveMaterialIGridBySoRepeat");
            }

            return msg;
        }


  
        private static string PrepareDataConversionBySoRepeat(ref ART_WF_ARTWORK_PROCESS_PA_2 processPA, ARTWORKEntities context, Dictionary<string, string> dicTypeOf, Dictionary<string, string> dicTypeOf2, Dictionary<string, string> dicPMSColour, Dictionary<string, string> dicProcessColour, Dictionary<string, string> dicStyleOfPrinting,  Dictionary<string, string> dicTotalColour)
        {
            var msg = "";
            try
            {

                string material_no = processPA.MATERIAL_NO;
                var currentUserId = processPA.CREATE_BY;

                //if (param.data.RETRIVE_TYPE == "SUGGEST")
                //{
                //    matNO = processPA.REFERENCE_MATERIAL;
                //}
                //else
                if (processPA.RETRIVE_TYPE == "RETRIVE")
                {
                   // matNO = param.data.MATERIAL_NO;
                    processPA.REQUEST_MATERIAL_STATUS = "Completed";
                }

                if (!String.IsNullOrEmpty(material_no))
                {
                    var matConversion = (from m in context.SAP_M_MATERIAL_CONVERSION
                                         where m.MATERIAL_NO == material_no
                                         select m).ToList();

                    SAP_M_CHARACTERISTIC charMat = new SAP_M_CHARACTERISTIC();

                    if (matConversion.Count > 0)
                    {
                        var matG = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_GROUP").FirstOrDefault();

                        var ZPKG_SEC_GROUP = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_GROUP").FirstOrDefault();
                        if (ZPKG_SEC_GROUP != null)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(ZPKG_SEC_GROUP.CHAR_NAME, ZPKG_SEC_GROUP.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                processPA.MATERIAL_GROUP_ID = charMat.CHARACTERISTIC_ID;
                            }
                        }
                        else
                        {
                            processPA.MATERIAL_GROUP_ID = null;
                        }

                        var ZPKG_SEC_PRIMARY_SIZE = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_PRIMARY_SIZE").FirstOrDefault();
                        var ZPKG_SEC_CONTAINER_TYPE = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_CONTAINER_TYPE").FirstOrDefault();
                        var ZPKG_SEC_LID_TYPE = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_LID_TYPE").FirstOrDefault();

                        if (ZPKG_SEC_PRIMARY_SIZE != null && ZPKG_SEC_CONTAINER_TYPE != null && ZPKG_SEC_LID_TYPE != null)
                        {
                            var three_p = (from p in context.SAP_M_3P
                                           where p.PRIMARY_SIZE_VALUE == ZPKG_SEC_PRIMARY_SIZE.CHAR_VALUE
                                             && p.CONTAINER_TYPE_VALUE == ZPKG_SEC_CONTAINER_TYPE.CHAR_VALUE
                                             && p.LID_TYPE_VALUE == ZPKG_SEC_LID_TYPE.CHAR_VALUE
                                           select p).FirstOrDefault();

                            if (three_p != null)
                            {
                                processPA.THREE_P_ID = three_p.THREE_P_ID;
                            }
                            else
                            {
                                processPA.THREE_P_ID = null;
                                processPA.PRIMARY_SIZE_ID = null;
                                processPA.PRIMARY_SIZE_OTHER = null;
                                processPA.CONTAINER_TYPE_ID = null;
                                processPA.CONTAINER_TYPE_OTHER = null;
                                processPA.LID_TYPE_ID = null;
                                processPA.LID_TYPE_OTHER = null;

                                if (ZPKG_SEC_PRIMARY_SIZE != null)
                                {
                                    charMat = new SAP_M_CHARACTERISTIC();
                                    charMat = CNService.GetCharacteristicData(ZPKG_SEC_PRIMARY_SIZE.CHAR_NAME, ZPKG_SEC_PRIMARY_SIZE.CHAR_VALUE, context);

                                    if (charMat != null)
                                    {
                                        processPA.PRIMARY_SIZE_ID = charMat.CHARACTERISTIC_ID;
                                    }
                                    else
                                    {
                                        processPA.THREE_P_ID = -1;
                                        processPA.PRIMARY_SIZE_ID = -1;
                                        processPA.PRIMARY_SIZE_OTHER = ZPKG_SEC_PRIMARY_SIZE.CHAR_VALUE;
                                    }
                                }

                                if (ZPKG_SEC_CONTAINER_TYPE != null)
                                {
                                    charMat = new SAP_M_CHARACTERISTIC();
                                    charMat = CNService.GetCharacteristicData(ZPKG_SEC_CONTAINER_TYPE.CHAR_NAME, ZPKG_SEC_CONTAINER_TYPE.CHAR_VALUE, context);

                                    if (charMat != null)
                                    {
                                        processPA.CONTAINER_TYPE_ID = charMat.CHARACTERISTIC_ID;
                                    }
                                    else
                                    {
                                        processPA.CONTAINER_TYPE_ID = -1;
                                        processPA.CONTAINER_TYPE_OTHER = ZPKG_SEC_CONTAINER_TYPE.CHAR_VALUE;
                                    }
                                }

                                if (ZPKG_SEC_LID_TYPE != null)
                                {
                                    charMat = new SAP_M_CHARACTERISTIC();
                                    charMat = CNService.GetCharacteristicData(ZPKG_SEC_LID_TYPE.CHAR_NAME, ZPKG_SEC_LID_TYPE.CHAR_VALUE, context);

                                    if (charMat != null)
                                    {
                                        processPA.LID_TYPE_ID = charMat.CHARACTERISTIC_ID;
                                    }
                                    else
                                    {
                                        processPA.LID_TYPE_ID = -1;
                                        processPA.LID_TYPE_OTHER = ZPKG_SEC_LID_TYPE.CHAR_VALUE;
                                    }
                                }
                            }
                        }
                        else
                        {
                            processPA.THREE_P_ID = null;
                            processPA.PRIMARY_SIZE_ID = null;
                            processPA.PRIMARY_SIZE_OTHER = null;
                            processPA.CONTAINER_TYPE_ID = null;
                            processPA.CONTAINER_TYPE_OTHER = null;
                            processPA.LID_TYPE_ID = null;
                            processPA.LID_TYPE_OTHER = null;

                            if (ZPKG_SEC_PRIMARY_SIZE != null)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(ZPKG_SEC_PRIMARY_SIZE.CHAR_NAME, ZPKG_SEC_PRIMARY_SIZE.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    processPA.PRIMARY_SIZE_ID = charMat.CHARACTERISTIC_ID;
                                }
                                else
                                {
                                    processPA.THREE_P_ID = -1;
                                    processPA.PRIMARY_SIZE_ID = -1;
                                    processPA.PRIMARY_SIZE_OTHER = ZPKG_SEC_PRIMARY_SIZE.CHAR_VALUE;
                                }
                            }

                            if (ZPKG_SEC_CONTAINER_TYPE != null)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(ZPKG_SEC_CONTAINER_TYPE.CHAR_NAME, ZPKG_SEC_CONTAINER_TYPE.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    processPA.CONTAINER_TYPE_ID = charMat.CHARACTERISTIC_ID;
                                }
                                else
                                {
                                    processPA.CONTAINER_TYPE_ID = -1;
                                    processPA.CONTAINER_TYPE_OTHER = ZPKG_SEC_CONTAINER_TYPE.CHAR_VALUE;
                                }
                            }

                            if (ZPKG_SEC_LID_TYPE != null)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(ZPKG_SEC_LID_TYPE.CHAR_NAME, ZPKG_SEC_LID_TYPE.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    processPA.LID_TYPE_ID = charMat.CHARACTERISTIC_ID;
                                }
                                else
                                {
                                    processPA.LID_TYPE_ID = -1;
                                    processPA.LID_TYPE_OTHER = ZPKG_SEC_LID_TYPE.CHAR_VALUE;
                                }
                            }
                        }

                        var ZPKG_SEC_PACKING_STYLE = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_PACKING_STYLE").FirstOrDefault();
                        var ZPKG_SEC_PACKING_SIZE = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_PACKING").FirstOrDefault();

                        if (ZPKG_SEC_PACKING_STYLE != null && ZPKG_SEC_PACKING_SIZE != null)
                        {
                            var two_p = (from p in context.SAP_M_2P
                                         where p.PACKING_SYLE_VALUE == ZPKG_SEC_PACKING_STYLE.CHAR_VALUE
                                          && p.PACK_SIZE_VALUE == ZPKG_SEC_PACKING_SIZE.CHAR_VALUE
                                         select p).FirstOrDefault();

                            if (two_p != null)
                            {
                                processPA.TWO_P_ID = two_p.TWO_P_ID;
                            }
                            else
                            {

                                processPA.TWO_P_ID = null;
                                processPA.PACKING_STYLE_ID = null;
                                processPA.PACKING_STYLE_OTHER = null;
                                processPA.PACK_SIZE_ID = null;
                                processPA.PACK_SIZE_OTHER = null;

                                if (ZPKG_SEC_PACKING_STYLE != null)
                                {
                                    charMat = new SAP_M_CHARACTERISTIC();
                                    charMat = CNService.GetCharacteristicData(ZPKG_SEC_PACKING_STYLE.CHAR_NAME, ZPKG_SEC_PACKING_STYLE.CHAR_VALUE, context);

                                    if (charMat != null)
                                    {
                                        processPA.PACKING_STYLE_ID = charMat.CHARACTERISTIC_ID;
                                    }
                                    else
                                    {
                                        processPA.TWO_P_ID = -1;
                                        processPA.PACKING_STYLE_ID = -1;
                                        processPA.PACKING_STYLE_OTHER = ZPKG_SEC_PACKING_STYLE.CHAR_VALUE;
                                    }
                                }

                                if (ZPKG_SEC_PACKING_SIZE != null)
                                {
                                    charMat = new SAP_M_CHARACTERISTIC();
                                    charMat = CNService.GetCharacteristicData(ZPKG_SEC_PACKING_SIZE.CHAR_NAME, ZPKG_SEC_PACKING_SIZE.CHAR_VALUE, context);

                                    if (charMat != null)
                                    {
                                        processPA.PACK_SIZE_ID = charMat.CHARACTERISTIC_ID;
                                    }
                                    else
                                    {
                                        processPA.PACK_SIZE_ID = -1;
                                        processPA.PACK_SIZE_OTHER = ZPKG_SEC_PACKING_SIZE.CHAR_VALUE;
                                    }
                                }
                            }
                        }
                        else
                        {
                            processPA.TWO_P_ID = null;
                            processPA.PACKING_STYLE_ID = null;
                            processPA.PACKING_STYLE_OTHER = null;
                            processPA.PACK_SIZE_ID = null;
                            processPA.PACK_SIZE_OTHER = null;

                            if (ZPKG_SEC_PACKING_STYLE != null)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(ZPKG_SEC_PACKING_STYLE.CHAR_NAME, ZPKG_SEC_PACKING_STYLE.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    processPA.PACKING_STYLE_ID = charMat.CHARACTERISTIC_ID;
                                }
                                else
                                {
                                    processPA.TWO_P_ID = -1;
                                    processPA.PACKING_STYLE_ID = -1;
                                    processPA.PACKING_STYLE_OTHER = ZPKG_SEC_PACKING_STYLE.CHAR_VALUE;
                                }
                            }

                            if (ZPKG_SEC_PACKING_SIZE != null)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(ZPKG_SEC_PACKING_SIZE.CHAR_NAME, ZPKG_SEC_PACKING_SIZE.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    processPA.PACK_SIZE_ID = charMat.CHARACTERISTIC_ID;
                                }
                                else
                                {
                                    processPA.PACK_SIZE_ID = -1;
                                    processPA.PACK_SIZE_OTHER = ZPKG_SEC_PACKING_SIZE.CHAR_VALUE;
                                }
                            }
                        }

                        //var ZPKG_SEC_PRODUCT_CODE = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_PRODUCT_CODE").ToList();
                        //if (ZPKG_SEC_PRODUCT_CODE != null)
                        //{
                        //    ART_WF_ARTWORK_PROCESS_PA_PRODUCT productConv = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                        //    ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER productOtherConv = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();

                        //    foreach (var iProduct in ZPKG_SEC_PRODUCT_CODE)
                        //    {
                        //        if (!String.IsNullOrEmpty(iProduct.CHAR_VALUE))
                        //        {
                        //            var xecmProduct_P = (from p in context.XECM_M_PRODUCT
                        //                                 where p.PRODUCT_CODE == iProduct.CHAR_VALUE
                        //                                 select p).FirstOrDefault();

                        //            if (xecmProduct_P != null)
                        //            {
                        //                productConv = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                        //                productConv.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                        //                productConv.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                        //                productConv.PRODUCT_CODE_ID = xecmProduct_P.XECM_PRODUCT_ID;
                        //                productConv.CREATE_BY = currentUserId;
                        //                productConv.UPDATE_BY = currentUserId;
                        //                ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.SaveOrUpdateNoLog(productConv, context);
                        //            }
                        //            else
                        //            {
                        //                productOtherConv = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();
                        //                productOtherConv.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                        //                productOtherConv.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                        //                productOtherConv.PRODUCT_CODE = iProduct.CHAR_VALUE;
                        //                productOtherConv.CREATE_BY = currentUserId;
                        //                productOtherConv.UPDATE_BY = currentUserId;
                        //                ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.SaveOrUpdateNoLog(productOtherConv, context);
                        //            }
                        //        }
                        //    }
                        //}

                        var ZPKG_SEC_PLANT_REGISTER = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_PLANT_REGISTER").FirstOrDefault();
                        if (ZPKG_SEC_PLANT_REGISTER != null)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(ZPKG_SEC_PLANT_REGISTER.CHAR_NAME, ZPKG_SEC_PLANT_REGISTER.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                processPA.PLANT_REGISTERED_ID = charMat.CHARACTERISTIC_ID;
                            }
                            else
                            {
                                processPA.PLANT_REGISTERED_ID = -1;
                                processPA.PLANT_REGISTERED_OTHER = ZPKG_SEC_PLANT_REGISTER.CHAR_VALUE;
                            }
                        }
                        else
                        {
                            processPA.PLANT_REGISTERED_ID = null;
                            processPA.PLANT_REGISTERED_OTHER = null;
                        }

                        var ZPKG_SEC_COMPANY_ADR = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_COMPANY_ADR").FirstOrDefault();
                        if (ZPKG_SEC_COMPANY_ADR != null)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(ZPKG_SEC_COMPANY_ADR.CHAR_NAME, ZPKG_SEC_COMPANY_ADR.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                processPA.COMPANY_ADDRESS_ID = charMat.CHARACTERISTIC_ID;
                            }
                            else
                            {
                                processPA.COMPANY_ADDRESS_ID = -1;
                                processPA.COMPANY_ADDRESS_OTHER = ZPKG_SEC_COMPANY_ADR.CHAR_VALUE;
                            }
                        }
                        else
                        {
                            processPA.COMPANY_ADDRESS_ID = null;
                            processPA.COMPANY_ADDRESS_OTHER = null;
                        }

                        var ZPKG_SEC_CATCHING_PERIOD = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_CATCHING_PERIOD").FirstOrDefault();
                        if (ZPKG_SEC_CATCHING_PERIOD != null)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(ZPKG_SEC_CATCHING_PERIOD.CHAR_NAME, ZPKG_SEC_CATCHING_PERIOD.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                processPA.CATCHING_PERIOD_ID = charMat.CHARACTERISTIC_ID;
                            }
                            else
                            {
                                processPA.CATCHING_PERIOD_ID = -1;
                                processPA.CATCHING_PERIOD_OTHER = ZPKG_SEC_CATCHING_PERIOD.CHAR_VALUE;
                            }
                        }
                        else
                        {
                            processPA.CATCHING_PERIOD_ID = null;
                            processPA.CATCHING_PERIOD_OTHER = null;
                        }

                        var ZPKG_SEC_CHANGE_POINT = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_CHANGE_POINT").FirstOrDefault();
                        if (ZPKG_SEC_CHANGE_POINT != null)
                        {
                            if (!String.IsNullOrEmpty(ZPKG_SEC_CHANGE_POINT.CHAR_VALUE))
                            {
                                processPA.CHANGE_POINT = ZPKG_SEC_CHANGE_POINT.CHAR_VALUE;
                            }
                        }
                        else
                        {
                            processPA.CHANGE_POINT = null;
                        }

                        var ZPKG_SEC_CATCHING_METHOD = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_CATCHING_METHOD").FirstOrDefault();
                        if (ZPKG_SEC_CATCHING_METHOD != null)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(ZPKG_SEC_CATCHING_METHOD.CHAR_NAME, ZPKG_SEC_CATCHING_METHOD.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                processPA.CATCHING_METHOD_ID = charMat.CHARACTERISTIC_ID;
                            }
                            else
                            {
                                processPA.CATCHING_METHOD_ID = -1;
                                processPA.CATCHING_METHOD_OTHER = ZPKG_SEC_CATCHING_METHOD.CHAR_VALUE;
                            }
                        }
                        else
                        {
                            processPA.CATCHING_METHOD_ID = null;
                            processPA.CATCHING_METHOD_ID = null;
                        }

                        var ZPKG_SEC_SCIENTIFIC_NAME = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_SCIENTIFIC_NAME").FirstOrDefault();
                        if (ZPKG_SEC_SCIENTIFIC_NAME != null)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(ZPKG_SEC_SCIENTIFIC_NAME.CHAR_NAME, ZPKG_SEC_SCIENTIFIC_NAME.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                processPA.SCIENTIFIC_NAME_ID = charMat.CHARACTERISTIC_ID;
                            }
                            else
                            {
                                processPA.SCIENTIFIC_NAME_ID = -1;
                                processPA.SCIENTIFIC_NAME_OTHER = ZPKG_SEC_SCIENTIFIC_NAME.CHAR_VALUE;
                            }
                        }
                        else
                        {
                            processPA.SCIENTIFIC_NAME_ID = null;
                            processPA.SCIENTIFIC_NAME_OTHER = null;
                        }

                        var ZPKG_SEC_DIRECTION = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_DIRECTION").FirstOrDefault();
                        if (ZPKG_SEC_DIRECTION != null)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(ZPKG_SEC_DIRECTION.CHAR_NAME, ZPKG_SEC_DIRECTION.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                processPA.DIRECTION_OF_STICKER_ID = charMat.CHARACTERISTIC_ID;
                            }
                            else
                            {
                                processPA.DIRECTION_OF_STICKER_ID = -1;
                                processPA.DIRECTION_OF_STICKER_OTHER = ZPKG_SEC_DIRECTION.CHAR_VALUE;
                            }
                        }
                        else
                        {
                            processPA.DIRECTION_OF_STICKER_ID = null;
                            processPA.DIRECTION_OF_STICKER_OTHER = null;
                        }

                        var ZPKG_SEC_SPECIE = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_SPECIE").FirstOrDefault();
                        if (ZPKG_SEC_SPECIE != null)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(ZPKG_SEC_SPECIE.CHAR_NAME, ZPKG_SEC_SPECIE.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                processPA.SPECIE_ID = charMat.CHARACTERISTIC_ID;
                            }
                            else
                            {
                                processPA.SPECIE_ID = -1;
                                processPA.SPECIE_OTHER = ZPKG_SEC_SPECIE.CHAR_VALUE;
                            }
                        }
                        else
                        {
                            processPA.SPECIE_ID = null;
                            processPA.SPECIE_OTHER = null;
                        }

                        if (matG != null)
                        {
                            string typeOf = "";// dicTypeOf[matG.VALUE];
                            if (dicTypeOf.TryGetValue(matG.CHAR_VALUE, out typeOf))
                            {
                                typeOf = dicTypeOf[matG.CHAR_VALUE];
                                var TYPE_OF = matConversion.Where(w => w.CHAR_NAME == typeOf).FirstOrDefault();
                                if (TYPE_OF != null)
                                {
                                    charMat = new SAP_M_CHARACTERISTIC();
                                    charMat = CNService.GetCharacteristicData(TYPE_OF.CHAR_NAME, TYPE_OF.CHAR_VALUE, context);

                                    if (charMat != null)
                                    {
                                        processPA.TYPE_OF_ID = charMat.CHARACTERISTIC_ID;
                                    }
                                    else
                                    {
                                        processPA.TYPE_OF_ID = -1;
                                        processPA.TYPE_OF_OTHER = TYPE_OF.CHAR_VALUE;
                                    }
                                }
                                else
                                {
                                    processPA.TYPE_OF_ID = null;
                                    processPA.TYPE_OF_OTHER = null;
                                }
                            }

                            string typeOf2 = "";
                            if (dicTypeOf2.TryGetValue(matG.CHAR_VALUE, out typeOf2))
                            {
                                typeOf2 = dicTypeOf2[matG.CHAR_VALUE];
                                var TYPE_OF_2 = matConversion.Where(w => w.CHAR_NAME == typeOf2).FirstOrDefault();
                                if (TYPE_OF_2 != null)
                                {
                                    charMat = new SAP_M_CHARACTERISTIC();
                                    charMat = CNService.GetCharacteristicData(TYPE_OF_2.CHAR_NAME, TYPE_OF_2.CHAR_VALUE, context);

                                    if (charMat != null)
                                    {
                                        processPA.TYPE_OF_2_ID = charMat.CHARACTERISTIC_ID;
                                    }
                                    else
                                    {
                                        processPA.TYPE_OF_2_ID = -1;
                                        processPA.TYPE_OF_2_OTHER = TYPE_OF_2.CHAR_VALUE;
                                    }
                                }
                                else
                                {
                                    processPA.TYPE_OF_2_ID = null;
                                    processPA.TYPE_OF_2_OTHER = null;
                                }
                            }

                            string styleOfPrinting = "";
                            if (dicStyleOfPrinting.TryGetValue(matG.CHAR_VALUE, out styleOfPrinting))
                            {
                                styleOfPrinting = dicStyleOfPrinting[matG.CHAR_VALUE];
                                var STYLE_OF_PRINTING = matConversion.Where(w => w.CHAR_NAME == styleOfPrinting).FirstOrDefault();
                                if (STYLE_OF_PRINTING != null)
                                {
                                    charMat = new SAP_M_CHARACTERISTIC();
                                    charMat = CNService.GetCharacteristicData(STYLE_OF_PRINTING.CHAR_NAME, STYLE_OF_PRINTING.CHAR_VALUE, context);

                                    if (charMat != null)
                                    {
                                        processPA.STYLE_OF_PRINTING_ID = charMat.CHARACTERISTIC_ID;
                                    }
                                    else
                                    {
                                        processPA.STYLE_OF_PRINTING_ID = -1;
                                        processPA.STYLE_OF_PRINTING_OTHER = STYLE_OF_PRINTING.CHAR_VALUE;
                                    }
                                }
                                else
                                {
                                    processPA.STYLE_OF_PRINTING_ID = null;
                                    processPA.STYLE_OF_PRINTING_OTHER = null;
                                }
                            }

                            string pmsColour = "";
                            if (dicPMSColour.TryGetValue(matG.CHAR_VALUE, out pmsColour))
                            {
                                pmsColour = dicPMSColour[matG.CHAR_VALUE];
                                var PMS_COLOUR = matConversion.Where(w => w.CHAR_NAME == pmsColour).FirstOrDefault();
                                if (PMS_COLOUR != null)
                                {
                                    charMat = new SAP_M_CHARACTERISTIC();
                                    charMat = CNService.GetCharacteristicData(PMS_COLOUR.CHAR_NAME, PMS_COLOUR.CHAR_VALUE, context);

                                    if (charMat != null)
                                    {
                                        processPA.PMS_COLOUR_ID = charMat.CHARACTERISTIC_ID;
                                    }
                                    else
                                    {
                                        processPA.PMS_COLOUR_ID = -1;
                                        processPA.PMS_COLOUR_OTHER = PMS_COLOUR.CHAR_VALUE;
                                    }
                                }
                                else
                                {
                                    processPA.PMS_COLOUR_ID = null;
                                    processPA.PMS_COLOUR_OTHER = null;
                                }
                            }

                            string processColour = "";
                            if (dicProcessColour.TryGetValue(matG.CHAR_VALUE, out processColour))
                            {
                                processColour = dicProcessColour[matG.CHAR_VALUE];
                                var PROCESS_COLOUR = matConversion.Where(w => w.CHAR_NAME == processColour).FirstOrDefault();
                                if (PROCESS_COLOUR != null)
                                {
                                    charMat = new SAP_M_CHARACTERISTIC();
                                    charMat = CNService.GetCharacteristicData(PROCESS_COLOUR.CHAR_NAME, PROCESS_COLOUR.CHAR_VALUE, context);

                                    if (charMat != null)
                                    {
                                        processPA.PROCESS_COLOUR_ID = charMat.CHARACTERISTIC_ID;
                                    }
                                    else
                                    {
                                        processPA.PROCESS_COLOUR_ID = -1;
                                        processPA.PROCESS_COLOUR_OTHER = PROCESS_COLOUR.CHAR_VALUE;
                                    }
                                }
                                else
                                {
                                    processPA.PROCESS_COLOUR_ID = null;
                                    processPA.PROCESS_COLOUR_OTHER = null;
                                }
                            }

                            string totalColour = "";
                            if (dicTotalColour.TryGetValue(matG.CHAR_VALUE, out totalColour))
                            {
                                totalColour = dicTotalColour[matG.CHAR_VALUE];
                                var TOTAL_COLOUR = matConversion.Where(w => w.CHAR_NAME == totalColour).FirstOrDefault();
                                if (TOTAL_COLOUR != null)     // ticket#438889 when debug found TOTAL_COLOUR is null then TOTAL_COLOUR.CHAR_NAME.Equals(totalColour) is error.
                                {
                                    if (TOTAL_COLOUR.CHAR_NAME.Equals(totalColour))
                                    {
                                        charMat = new SAP_M_CHARACTERISTIC();
                                        charMat = CNService.GetCharacteristicData(TOTAL_COLOUR.CHAR_NAME, TOTAL_COLOUR.CHAR_VALUE, context);

                                        if (charMat != null)
                                        {
                                            processPA.TOTAL_COLOUR_ID = charMat.CHARACTERISTIC_ID;
                                        }
                                        else
                                        {
                                            processPA.TOTAL_COLOUR_ID = -1;
                                            processPA.TOTAL_COLOUR_OTHER = TOTAL_COLOUR.CHAR_VALUE;
                                        }
                                    }
                                    else
                                    {
                                        processPA.TOTAL_COLOUR_ID = null;
                                        processPA.TOTAL_COLOUR_OTHER = null;
                                    }
                                }
                                else
                                {
                                    processPA.TOTAL_COLOUR_ID = null;
                                    processPA.TOTAL_COLOUR_OTHER = null;
                                }

                            }
                        }

                        processPA.MATERIAL_NO = material_no;
                        //if (param.data.RETRIVE_TYPE == "SUGGEST")
                        //{
                        //    processPA.MATERIAL_NO = "";
                        //    processPA.REQUEST_MATERIAL_STATUS = "";
                        //}

                        var productionPlants = matConversion.Where(i => i.CHAR_NAME == "ZPKG_SEC_PRODUCTION_PLANT").ToList();
                        if (productionPlants != null)
                        {
                            string productionPlant = "";
                            foreach (var iProductPlant in productionPlants)
                            {
                                if (!String.IsNullOrEmpty(iProductPlant.CHAR_VALUE) && !productionPlant.Contains(iProductPlant.CHAR_VALUE))
                                {
                                    if (String.IsNullOrEmpty(productionPlant))
                                    {
                                        productionPlant = iProductPlant.CHAR_VALUE;
                                    }
                                    else
                                    {
                                        productionPlant += "," + iProductPlant.CHAR_VALUE;
                                    }
                                }
                            }

                            if (!String.IsNullOrEmpty(productionPlant))
                            {
                                processPA.PRODICUTION_PLANT_ID = -1;
                                processPA.PRODICUTION_PLANT_OTHER = productionPlant;
                            }
                            else
                            {
                                processPA.PRODICUTION_PLANT_ID = null;
                                processPA.PRODICUTION_PLANT_OTHER = null;
                            }
                        }

                       // ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA, context);

                        #region "FAO_ZONE"
                        var matFAO = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_FAO").ToList();

                        if (matFAO != null && matFAO.Count > 0)
                        {


                            processPA.FAOS = new List<ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_2>();
                            ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE fao_new = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                            foreach (var iFAONew in matFAO)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(iFAONew.CHAR_NAME, iFAONew.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    fao_new = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                                    fao_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                    fao_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                    fao_new.FAO_ZONE_ID = charMat.CHARACTERISTIC_ID;
                                    fao_new.CREATE_BY = currentUserId;
                                    fao_new.UPDATE_BY = currentUserId;
                                    //ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.SaveOrUpdateNoLog(fao_new, context);
                                    processPA.FAOS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE(fao_new));
                                }
                            }

                        }
                        #endregion

                        #region "CATCHING_AREA"

                        var matCatchingArea = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_CATCHING_AREA").ToList();

                        if (matCatchingArea != null && matCatchingArea.Count > 0)
                        {

                            processPA.CATCHING_AREAS = new List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_2>();
                            ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA catching_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                            foreach (var iCatchingNew in matCatchingArea)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(iCatchingNew.CHAR_NAME, iCatchingNew.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    catching_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                                    catching_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                    catching_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                    catching_new.CATCHING_AREA_ID = charMat.CHARACTERISTIC_ID;
                                    catching_new.CREATE_BY = currentUserId;
                                    catching_new.UPDATE_BY = currentUserId;
                                    // ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE.SaveOrUpdateNoLog(catching_new, context);
                                    processPA.CATCHING_AREAS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA(catching_new));
                                }
                            }
                        }

                        #endregion

                        #region "CATCHING_METHOD"
                        // ticke#425737 added by aof 
                        var matCatchingMethod = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_CATCHING_METHOD").ToList();

                        if (matCatchingMethod != null && matCatchingMethod.Count > 0)
                        {
                            processPA.CATCHING_METHODS = new List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_2>();
                            ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD method_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                            foreach (var iMathodNew in matCatchingMethod)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(iMathodNew.CHAR_NAME, iMathodNew.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    method_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                                    method_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                    method_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                    method_new.CATCHING_METHOD_ID = charMat.CHARACTERISTIC_ID;
                                    method_new.CREATE_BY = currentUserId;
                                    method_new.UPDATE_BY = currentUserId;
                                    //ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE.SaveOrUpdateNoLog(method_new, context);
                                    processPA.CATCHING_METHODS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD(method_new));
                                }
                            }
                        }
                        // ticke#425737 added by aof 
                        #endregion

                        #region "SYMBOL"

                        var matSymbol = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_SYMBOL").ToList();

                        if (matSymbol != null && matSymbol.Count > 0)
                        {

                            processPA.SYMBOLS = new List<ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2>();
                            ART_WF_ARTWORK_PROCESS_PA_SYMBOL symbol_new = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                            foreach (var iSymbolNew in matSymbol)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(iSymbolNew.CHAR_NAME, iSymbolNew.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    symbol_new = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                                    symbol_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                    symbol_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                    symbol_new.SYMBOL_ID = charMat.CHARACTERISTIC_ID;
                                    symbol_new.CREATE_BY = currentUserId;
                                    symbol_new.UPDATE_BY = currentUserId;
                                    //ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE.SaveOrUpdate(symbol_new, context);
                                    processPA.SYMBOLS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_SYMBOL(symbol_new));
                                }
                            }
                        }
                        #endregion

                        #region "PRODUCT"
                      
                        var listProduct = matConversion.Where(i => i.CHAR_NAME == "ZPKG_SEC_PRODUCT_CODE").ToList();

                        if (listProduct != null)
                        {
                            var listProductCode = listProduct.Select(s => s.CHAR_VALUE).ToList();

                            if (listProductCode != null)
                            {
                                processPA.PRODUCTS = new List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2>();
                                processPA.PRODUCT_OTHERS = new List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2>();

                                var xProduct = (from p in context.XECM_M_PRODUCT
                                                where listProductCode.Contains(p.PRODUCT_CODE)
                                                select p).ToList();

                                ART_WF_ARTWORK_PROCESS_PA_PRODUCT productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                                foreach (SAP_M_MATERIAL_CONVERSION iProduct in listProduct)
                                {
                                    int productID = xProduct.Where(w => w.PRODUCT_CODE == iProduct.CHAR_VALUE)
                                                            .Select(s => s.XECM_PRODUCT_ID).FirstOrDefault();

                                    if (productID > 0)
                                    {
                                        productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                                        productNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        productNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        productNew.PRODUCT_CODE_ID = productID;
                                        productNew.CREATE_BY = currentUserId;
                                        productNew.UPDATE_BY = currentUserId;
                                        // ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.SaveOrUpdateNoLog(productNew, context);
                                        processPA.PRODUCTS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_PRODUCT(productNew));
                                    }
                                    else
                                    {
                                        ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER productOtherNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();

                                        productOtherNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();
                                        productOtherNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        productOtherNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        productOtherNew.PRODUCT_CODE = iProduct.CHAR_VALUE;
                                        productOtherNew.CREATE_BY = currentUserId;
                                        productOtherNew.UPDATE_BY = currentUserId;
                                        //ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.SaveOrUpdateNoLog(productOtherNew, context);
                                        processPA.PRODUCT_OTHERS.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER(productOtherNew));
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                msg = CNService.GetErrorMessage_SORepeat(ex, "PrepareDataConversionBySoRepeat");

            }
            return msg;
        }



        public static string PrepareArtworkRequestbySORepeat(ref ART_WF_ARTWORK_REQUEST_2 obj, ARTWORKEntities context)
        {
            var msg = "";

            try
            {
                //-------------------------------------------------------------
         
                var Distinct_PRODUCT_CODE = obj.SALES_ORDER_REPEAT.Select(m => m.PRODUCT_CODE).Distinct().ToList();
                obj.REQUEST_PRODUCT = new List<ART_WF_ARTWORK_REQUEST_PRODUCT_2>();
                foreach (var item in Distinct_PRODUCT_CODE)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        var xProduct = (from p in context.XECM_M_PRODUCT
                                        where p.PRODUCT_CODE == item
                                        select p).FirstOrDefault();

                        if (xProduct != null)
                        {
                            var product = new ART_WF_ARTWORK_REQUEST_PRODUCT();
                            //product.ARTWORK_REQUEST_ID = newRepeatRequestID;
                            product.PRODUCT_CODE_ID = xProduct.XECM_PRODUCT_ID;
                            product.CREATE_BY = obj.SALES_ORDER_REPEAT[0].CREATE_BY;
                            product.UPDATE_BY = obj.SALES_ORDER_REPEAT[0].UPDATE_BY;
                            // start 20230121_3V_SOREPAT INC-93118
                            if (obj.IS_VAP)
                            {
                                product.PRODUCT_TYPE = "VAP"; 
                            } else
                            if (obj.HAS_VAP_PLANT )
                            {
                                product.PRODUCT_TYPE = CNService.Getcheck_product_vap(xProduct.PRODUCT_CODE, "3");
                            }
                            //end 20230121_3V_SOREPAT INC-93118

                            obj.REQUEST_PRODUCT.Add(MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCT(product));

                            //ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.SaveOrUpdateNoLog(product, context);
                        }
                    }
                }

                //-------------------------------------------------------------
                var COMPONENT_MATERIAL = obj.SALES_ORDER_REPEAT.Select(m => m.COMPONENT_MATERIAL).FirstOrDefault();
                if (string.IsNullOrEmpty(COMPONENT_MATERIAL))
                    COMPONENT_MATERIAL = obj.SALES_ORDER_REPEAT.Select(m => m.PRODUCT_CODE).FirstOrDefault();

                CopyRequestFormByLastWF(ref obj, COMPONENT_MATERIAL, context);


                if (obj.REQUEST_FORM_CREATE_DATE == null)
                {
                    obj.REQUEST_FORM_CREATE_DATE = DateTime.Now;
                }
            }

            catch (Exception ex)
            {
                msg = CNService.GetErrorMessage_SORepeat(ex, "PrepareArtworkRequestbySORepeat") + "{Log}";
            }

            return msg;
        }

        public static string CreateOpentextNodeRequestFormbySoRepeat(ref ART_WF_ARTWORK_REQUEST_2 obj, ARTWORKEntities context, string token, List<string> listDistinctMaterialDescription,string username)
        {
            var msg = "";

            try
            {
                var stepId_UPLOAD_AW = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "UPLOAD_AW" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                //-------code below to create node request form no in openetext             
                long folderID = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkRequestFormNodeID"]);
                long templateID = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkTemplateNodeID"]);
                var user_yyyyMMddHHmmssffff_ctrlname = username + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "_" + obj.CONTROL_NAME; //generate node name
                var tempNodeRequestFormName = "RF_" + user_yyyyMMddHHmmssffff_ctrlname;

                var nodeTemplate = CWSService.copyNode(tempNodeRequestFormName, templateID, folderID, token);

                //-------------------------------------------------
                obj.ARTWORK_REQUEST_NO = tempNodeRequestFormName;
                obj.REQUEST_FORM_FOLDER_NODE_ID = nodeTemplate.ID;
                //--------------------------------------------------

                string secondaryPkgNodeID = "";
                string secondaryPkgArtworkFolderName = "";
                string configArtworkOther = "";
                Int64 intSecondaryPkgNodeID = 0;

                secondaryPkgNodeID = ConfigurationManager.AppSettings["SecondaryPackagingNodeID"];
                secondaryPkgArtworkFolderName = ConfigurationManager.AppSettings["SecondaryPkgArtworkFolderName"];
                configArtworkOther = ConfigurationManager.AppSettings["ArtworkFolderNamePrintMaster"];

                if (!String.IsNullOrEmpty(secondaryPkgNodeID))
                {
                    intSecondaryPkgNodeID = Convert.ToInt64(secondaryPkgNodeID);
                }



                obj.REQUEST_ITEMS = new List<ART_WF_ARTWORK_REQUEST_ITEM_2>();

                foreach (var matDesc in listDistinctMaterialDescription)
                {
                    Node nodeMat = CWSService.getNodeByName(intSecondaryPkgNodeID, matDesc, token);
                    var arrStr = matDesc.Split('-');
                    var component_material = arrStr[0];
                    if (nodeMat != null)
                    {
                        Node nodeMatAWFolder = CWSService.getNodeByName(-nodeMat.ID, secondaryPkgArtworkFolderName, token);
                        if (nodeMatAWFolder != null)
                        {
                            Node[] nodeMatAWFile = CWSService.getAllNodeInFolder(nodeMatAWFolder.ID, token);

                            if (nodeMatAWFile != null)
                            {
                                ART_WF_ARTWORK_REQUEST_ITEM_2 itemRequest = new ART_WF_ARTWORK_REQUEST_ITEM_2();
                                long nodeID_atta = 0;
                                long versionPDF = 1;
                                var cntFile = 1;
                                obj.LIST_AW_NODE_ID = new List<long>();
                                foreach (Node iNodeAW in nodeMatAWFile)
                                {
                                    long nodeID = 0;

                                    if (obj.REQUEST_FORM_FOLDER_NODE_ID != null)
                                    {
                                        nodeID = Convert.ToInt64(obj.REQUEST_FORM_FOLDER_NODE_ID);
                                    }

                                    Node nodeOthers = CWSService.getNodeByName(nodeID, configArtworkOther, token);
                                    if (nodeID > 0)
                                    {
                                        if (nodeOthers != null)
                                        {

                                            // start by aof #INC-89321
                                            BLL.DocumentManagement.Version version = CWSService.getVersion(iNodeAW.ID, token, iNodeAW.VersionInfo.VersionNum);
                                            iNodeAW.Name = version.Filename;
                                            var newNode = CWSService.copyNode(iNodeAW.Name, iNodeAW.ID, nodeOthers.ID, token); 
                                            // end by aof #INC-89321

                                            if (newNode.VersionInfo != null)
                                            {
                                                string extension = Path.GetExtension(iNodeAW.Name);
                                                string contentType = newNode.VersionInfo.MimeType;

                                                extension = extension.Replace(".", "");

                                                itemRequest = new ART_WF_ARTWORK_REQUEST_ITEM_2();
                                                //itemRequest.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;
                                                itemRequest.CONTENT_TYPE = contentType;
                                                itemRequest.FILE_NAME = iNodeAW.Name;
                                                itemRequest.FILE_SIZE = Convert.ToInt64(newNode.VersionInfo.FileDataSize);
                                                itemRequest.REQUEST_FORM_FILE_NODE_ID = newNode.ID;
                                                itemRequest.EXTENSION = extension;
                                                itemRequest.REPEAT_SO_MATERIAL_NO = component_material.Trim();
                                                itemRequest.CREATE_BY = obj.UPDATE_BY ;
                                                itemRequest.UPDATE_BY = obj.UPDATE_BY;


                                                if (obj.IS_COMPLETE || obj.IS_SEND_TO_PP)
                                                {
                                                    itemRequest.PROCESS_STEP_PA = new ART_WF_ARTWORK_PROCESS_2();
                                                    itemRequest.PROCESS_STEP_PA.ATTACHMENT = new ART_WF_ARTWORK_ATTACHMENT_2();
                                                 
                                                    var tempNodeREQUEST_ITEM_NO = "AW_" + user_yyyyMMddHHmmssffff_ctrlname + "_" + cntFile.ToString();
                                                    long folderAWID = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkNodeID"]);
                                                    var nodeTemplateAW = CWSService.copyNodeAndDeleteNodeIsExist(tempNodeREQUEST_ITEM_NO, templateID, folderAWID, token);// ticket 459570 by aof //  var nodeTemplate = CWSService.copyNode(formNO, templateID, folderID, token);
                                                    itemRequest.PROCESS_STEP_PA.ARTWORK_FOLDER_NODE_ID = nodeTemplateAW.ID;
                                                    obj.LIST_AW_NODE_ID.Add(nodeTemplateAW.ID);

                                                    //Copy file Upload in CS
                                                    string artworkFolder = ConfigurationManager.AppSettings["ArtworkFolderName"];
                                                    string ArtworkFolderNamePrintMaster = ConfigurationManager.AppSettings["ArtworkFolderNamePrintMaster"];

                                                    if (obj.TYPE_OF_ARTWORK != "NEW")
                                                    {
                                                        artworkFolder = ArtworkFolderNamePrintMaster;
                                                    }

                                                    var artworkFolderNode = CWSService.getNodeByName(nodeTemplateAW.ID, artworkFolder, token);

                                                    long FileItemID = Convert.ToInt64(itemRequest.REQUEST_FORM_FILE_NODE_ID);

                                                    if (artworkFolderNode != null)
                                                    {
                                                        var nodeFile = CWSService.copyNode(itemRequest.FILE_NAME, FileItemID, artworkFolderNode.ID, token);
                                                        if (nodeFile != null)
                                                        {
                                                            nodeID_atta = nodeFile.ID;
                                                            // start-ticket#438889
                                                            if (nodeFile.VersionInfo != null)
                                                            {
                                                                versionPDF = nodeFile.VersionInfo.VersionNum;
                                                            }
                                                            // last-ticket#438889 
                                                            //listNodeId.Add(nodeId);


                                                            //itemRequest.ATTACHMENT.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                                                            //itemRequest.ATTACHMENT.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                                                            itemRequest.PROCESS_STEP_PA.ATTACHMENT.CONTENT_TYPE = itemRequest.CONTENT_TYPE;
                                                            itemRequest.PROCESS_STEP_PA.ATTACHMENT.CREATE_BY = itemRequest.CREATE_BY;
                                                            itemRequest.PROCESS_STEP_PA.ATTACHMENT.UPDATE_BY = itemRequest.CREATE_BY;
                                                            itemRequest.PROCESS_STEP_PA.ATTACHMENT.EXTENSION = itemRequest.EXTENSION;
                                                            itemRequest.PROCESS_STEP_PA.ATTACHMENT.FILE_NAME = itemRequest.FILE_NAME;
                                                            itemRequest.PROCESS_STEP_PA.ATTACHMENT.IS_CUSTOMER = "X";
                                                            itemRequest.PROCESS_STEP_PA.ATTACHMENT.IS_INTERNAL = "X";
                                                            itemRequest.PROCESS_STEP_PA.ATTACHMENT.IS_VENDOR = "X";
                                                            itemRequest.PROCESS_STEP_PA.ATTACHMENT.NODE_ID = nodeID_atta;
                                                            itemRequest.PROCESS_STEP_PA.ATTACHMENT.SIZE = Convert.ToInt64(itemRequest.FILE_SIZE);
                                                            itemRequest.PROCESS_STEP_PA.ATTACHMENT.STEP_ARTWORK_ID = stepId_UPLOAD_AW;
                                                            itemRequest.PROCESS_STEP_PA.ATTACHMENT.VERSION = versionPDF; //attach.VERSION = 1; ticket#438889
                                                            itemRequest.PROCESS_STEP_PA.ATTACHMENT.VERSION2 = "1.0";
                                                        }
                                             
                                                    }
                                                    itemRequest.REQUEST_ITEM_NO = tempNodeREQUEST_ITEM_NO;
                                                }

                                                obj.REQUEST_ITEMS.Add(itemRequest);
                                                cntFile += 1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }

            }
            
            catch (Exception ex)
            {
                msg = CNService.GetErrorMessage_SORepeat(ex, "CreateOpentextNodeRequestFormbySoRepeat") + "{Log}";
            }

            return msg;
        }

        public static string ValidatingSaleOrderBySORepeat(ART_WF_ARTWORK_REQUEST_2 obj, ARTWORKEntities context,string token, ref List<string> listDistinctMaterialDescription)
        {
            var msg = "";

            try
            {

                var tempMat5 = obj.SALES_ORDER_REPEAT.Select(m => m.COMPONENT_MATERIAL).Distinct().ToList();
                tempMat5.AddRange(obj.SALES_ORDER_REPEAT.Where(m => m.PRODUCT_CODE.StartsWith("5")).Select(m => m.PRODUCT_CODE).Distinct().ToList());

                if (tempMat5.Where(m => !String.IsNullOrEmpty(m)).Distinct().Count() > 1)
                {
                    msg += "Cannot select multiple material code." + "<br/>";
                    //return res;
                }

                var tempSO = obj.SALES_ORDER.Select(m => m.SALES_ORDER_NO).ToList();


                if (obj.SALES_ORDER_REPEAT.Where(w => !String.IsNullOrEmpty(w.SOLD_TO_DISPLAY_TXT)).Select(m => m.SOLD_TO_DISPLAY_TXT).Distinct().Count() > 1)
                {
                    msg += "Cannot select multiple sold to." + "<br/>";
                    //return res;
                }
                if (obj.SALES_ORDER_REPEAT.Where(w => !String.IsNullOrEmpty(w.SHIP_TO_DISPLAY_TXT)).Select(m => m.SHIP_TO_DISPLAY_TXT).Distinct().Count() > 1)
                {
                    msg += "Cannot select multiple ship to." + "<br/>";
                    //return res;
                }
                if (obj.SALES_ORDER_REPEAT.Where(w => !String.IsNullOrEmpty(w.BRAND_DISPLAY_TXT)).Select(m => m.BRAND_DISPLAY_TXT).Distinct().Count() > 1)
                {
                    msg += "Cannot select multiple brand." + "<br/>";
                    // return res;
                }

                var general = false;
                var DistinctCOMPONENT_MATERIAL = obj.SALES_ORDER_REPEAT.Select(m => m.COMPONENT_MATERIAL).Distinct().ToList().FirstOrDefault();
                if (DistinctCOMPONENT_MATERIAL == null)
                    DistinctCOMPONENT_MATERIAL = obj.SALES_ORDER_REPEAT.Select(m => m.PRODUCT_CODE).Distinct().ToList().FirstOrDefault();
                else
                {
                    general = true;
                }

                foreach (var SALES_ORDER in obj.SALES_ORDER_REPEAT)
                {
                    ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT temp = new ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT();

                    //check in workflow
                    var soDetail = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL>();
                    if (general)
                    {
                        soDetail = (from q in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                    join m in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT on q.BOM_ID equals m.PO_COMPLETE_SO_ITEM_COMPONENT_ID
                                    where q.SALES_ORDER_NO == SALES_ORDER.SALES_ORDER_NO
                                    && q.SALES_ORDER_ITEM == SALES_ORDER.SALES_ORDER_ITEM
                                    && q.MATERIAL_NO == SALES_ORDER.PRODUCT_CODE
                                    && m.COMPONENT_MATERIAL == SALES_ORDER.COMPONENT_MATERIAL
                                    select q).ToList();
                    }
                    else
                    {
                        soDetail = (from q in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                    where q.SALES_ORDER_NO == SALES_ORDER.SALES_ORDER_NO
                                    && q.SALES_ORDER_ITEM == SALES_ORDER.SALES_ORDER_ITEM
                                    && q.MATERIAL_NO == SALES_ORDER.COMPONENT_MATERIAL
                                    select q).ToList();
                    }

                    if (soDetail.Count > 0)
                    {
                        var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(soDetail[0].ARTWORK_SUB_ID, context);
                        var item = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(process.ARTWORK_ITEM_ID, context);

                        msg += "Cannot select the below item" + ".<br/>";
                        msg += "SO : " + SALES_ORDER.SALES_ORDER_NO + "<br/>";
                        msg += "Material no : " + soDetail[0].MATERIAL_NO + "<br/>";
                        msg += "Please check on workflow no : " + "<a target='_blank' href='/TaskFormArtwork/" + soDetail[0].ARTWORK_SUB_ID + "'>" + item.REQUEST_ITEM_NO + "</a><br/>";

                        //return res;
                    }
                    else
                    {
                        //not found in workflow
                        //find in request form
                        if (general)
                        {
                            temp = ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT()
                            { SALES_ORDER_NO = SALES_ORDER.SALES_ORDER_NO, SALES_ORDER_ITEM = SALES_ORDER.SALES_ORDER_ITEM, COMPONENT_MATERIAL = DistinctCOMPONENT_MATERIAL }, context).FirstOrDefault();
                        }
                        else
                        {
                            temp = ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT()
                            { SALES_ORDER_NO = SALES_ORDER.SALES_ORDER_NO, SALES_ORDER_ITEM = SALES_ORDER.SALES_ORDER_ITEM, PRODUCT_CODE = DistinctCOMPONENT_MATERIAL }, context).FirstOrDefault();
                        }

                        if (temp != null)
                        {
                            var request = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(temp.ARTWORK_REQUEST_ID, context);
                            if (request != null)
                            {
                                msg += "Cannot select the below item" + "<br/>";
                                msg += "SO : " + SALES_ORDER.SALES_ORDER_NO + "<br/>";
                                msg += "Material no : " + DistinctCOMPONENT_MATERIAL + "<br/>";
                                msg += "Please check on request form no : " + "<a target='_blank' href='/Artwork/" + temp.ARTWORK_REQUEST_ID + "'>" + request.ARTWORK_REQUEST_NO + "</a><br/>";

                                //return res;
                            }
                        }
                    }
                }

                if (CNService.IsLock(DistinctCOMPONENT_MATERIAL, context))
                {
                    msg += "This material number is locked. Please contact your PA supervisor.<br/>";
                    //return res;
                }


                //--------------------------------------------------------------------------------------------------------
                string secondaryPkgNodeID = "";
                string secondaryPkgArtworkFolderName = "";
                string configArtworkOther = "";
                Int64 intSecondaryPkgNodeID = 0;


                secondaryPkgNodeID = ConfigurationManager.AppSettings["SecondaryPackagingNodeID"];
                secondaryPkgArtworkFolderName = ConfigurationManager.AppSettings["SecondaryPkgArtworkFolderName"];
                configArtworkOther = ConfigurationManager.AppSettings["ArtworkFolderName"];

                if (!String.IsNullOrEmpty(secondaryPkgNodeID))
                {
                    intSecondaryPkgNodeID = Convert.ToInt64(secondaryPkgNodeID);
                }

               // string message = "";

                //var artworkRequest = context.ART_WF_ARTWORK_REQUEST.Where(a => a.ARTWORK_REQUEST_ID == param.data.ARTWORK_REQUEST_ID).FirstOrDefault();
                List<string> ListDistinctCOMPONENT_MATERIAL = new List<string>();
             
                foreach (var item in obj.SALES_ORDER_REPEAT)
                {
                    if (String.IsNullOrEmpty(item.COMPONENT_MATERIAL) && !String.IsNullOrEmpty(item.PRODUCT_CODE) && item.PRODUCT_CODE.StartsWith("5"))
                    {
                        ListDistinctCOMPONENT_MATERIAL.Add(item.PRODUCT_CODE);
                    }
                    else
                    {
                        ListDistinctCOMPONENT_MATERIAL.Add(item.COMPONENT_MATERIAL);
                    }

                }


                ListDistinctCOMPONENT_MATERIAL = ListDistinctCOMPONENT_MATERIAL.Select(m => m).Distinct().ToList();
                //var token = CWSService.getAuthToken();
                foreach (var COMPONENT_MATERIAL in ListDistinctCOMPONENT_MATERIAL)
                {
                    var DECRIPTION = (from s in context.XECM_M_PRODUCT5
                                      where s.PRODUCT_CODE == COMPONENT_MATERIAL
                                      select s.PRODUCT_DESCRIPTION).FirstOrDefault();
                
                  

                    if (DECRIPTION == null)
                    {
                        msg += "Cannot create the repeat request form. Please check Material No.<span style='color: blue;'>" + COMPONENT_MATERIAL + "</span> desciption is null.<br/>";
                        //msg += "{10}" + MessageHelper.GetMessage("MSG_020", context) + "<br>" + String.Format(MessageHelper.GetMessage("MSG_021", context), COMPONENT_MATERIAL) + "<br>";                    

                    }
                    else
                    {
                        string matDesc = COMPONENT_MATERIAL + " - " + DECRIPTION;
                        listDistinctMaterialDescription.Add(matDesc);   // for out parameter

                        Node nodeMat = CWSService.getNodeByName(intSecondaryPkgNodeID, matDesc, token);
                        if (nodeMat == null)
                        {
                            msg += "Cannot create the repeat request form. System not found node <span style='color: blue;'>" + matDesc + "</span> in secondary packaging of material master workspace.<br/>";
                            //msg += "{11}" + MessageHelper.GetMessage("MSG_020", context) + "<br>" + String.Format(MessageHelper.GetMessage("MSG_021", context), COMPONENT_MATERIAL) + "<br>";
                        }
                        else
                        {
                            Node nodeMatAWFolder = CWSService.getNodeByName(-nodeMat.ID, secondaryPkgArtworkFolderName, token);
                            if (nodeMatAWFolder == null)
                            {
                                msg += "Cannot create the repeat request form. System not found node <span style='color: blue;'>" + secondaryPkgArtworkFolderName + "</span> in " + matDesc + " in secondary packaging of material master workspace.<br/>";
                                //msg += "{12}" + MessageHelper.GetMessage("MSG_020", context) + "<br>" + String.Format(MessageHelper.GetMessage("MSG_021", context), COMPONENT_MATERIAL) + "<br>";
                            }
                            else
                            {
                                Node[] nodeMatAWFile = CWSService.getAllNodeInFolder(nodeMatAWFolder.ID, token);
                                if (nodeMatAWFile == null)
                                {
                                    msg += "Cannot create the repeat request form. System not found <span style='color: blue;'>file</span> in " + secondaryPkgArtworkFolderName + " of " + matDesc + " in secondary packaging of material master workspace.<br/>";
                                    //msg += "{13}" + MessageHelper.GetMessage("MSG_020", context) + "<br>" + "System not found a file in material master workspace." + "(" + COMPONENT_MATERIAL + ")" + "<br>";

                                }
                                else
                                {
                                    if (nodeMatAWFile.Count() != 1)
                                    {
                                        msg += "Cannot create the repeat request form. There is more than <span style='color: blue;'>1 file</span> in " + secondaryPkgArtworkFolderName + " of " + matDesc + " in secondary packaging of material master workspace.<br/>";                                  
                                    }
                                }

                            }
                        }
                    }
                
                }


                // start 20230121_3V_SOREPAT INC-93118
              
                var tempSALES_ORDER_REPEAT = obj.SALES_ORDER_REPEAT;
                checkAndAssignProductVAP(ref tempSALES_ORDER_REPEAT, context);    //check and assign product type if product is vap and multi plant  
                obj.SALES_ORDER_REPEAT = tempSALES_ORDER_REPEAT;
                var msg_check_vap = "";

                //start check product is vap and multi plant
                var cntVAP = 0;
                obj.IS_VAP = false;
                obj.HAS_VAP_PLANT = false;
                foreach (var salerepeat in obj.SALES_ORDER_REPEAT)
                {
                    if (salerepeat.PRODUCT_TYPE == "VAP")
                    {
                        cntVAP += 1;
                    }
                    if (salerepeat.PRODUCTION_PLANT_DISPLAY_TXT == "1021")
                    {
                        obj.HAS_VAP_PLANT = true;
                    }
                }

                if (cntVAP > 0 && cntVAP != obj.SALES_ORDER_REPEAT.Count)
                {
                    msg_check_vap = "Cannot create the repeat request form. VAP product with multiple plants are not allowed for bypass function." + "<br/>"; ;
                    obj.IS_VAP = false;
                }
                else if (cntVAP > 0 && cntVAP == obj.SALES_ORDER_REPEAT.Count)
                {
                    obj.IS_VAP = true;
                }
                else
                {
                    obj.IS_VAP = false;
                }

                

                // end 20230121_3V_SOREPAT INC-93118



                //--------------------------------------------------------------------------------------------------------
                //validate sale order have been created the workflow already.
                if (string.IsNullOrEmpty(msg))
                {
                    if (obj.IS_COMPLETE || obj.IS_SEND_TO_PP)
                    {
                        var soRepeat = (from q in obj.SALES_ORDER_REPEAT
                                        select q).ToList();
                        foreach (var item in soRepeat)
                        {
                            var so = item.SALES_ORDER_NO;
                            var soItem = item.SALES_ORDER_ITEM;
                            var productCode = item.PRODUCT_CODE;
                            var mat5 = item.COMPONENT_MATERIAL;
                            var soDetail = (from q in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                            join m in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT on q.BOM_ID equals m.PO_COMPLETE_SO_ITEM_COMPONENT_ID
                                            where q.SALES_ORDER_NO == so
                                            && q.SALES_ORDER_ITEM == soItem
                                            && q.MATERIAL_NO == productCode
                                            && m.COMPONENT_MATERIAL == mat5
                                            select q).ToList();

                            if (soDetail.Count > 0)
                            {
                                var ARTWORK_SUB_ID = soDetail.FirstOrDefault().ARTWORK_SUB_ID;

                                var process = (from q in context.ART_WF_ARTWORK_PROCESS
                                               where q.ARTWORK_SUB_ID == ARTWORK_SUB_ID
                                               select q).FirstOrDefault();

                                var wfNo = (from q in context.ART_WF_ARTWORK_REQUEST_ITEM
                                            where q.ARTWORK_ITEM_ID == process.ARTWORK_ITEM_ID
                                            select q.REQUEST_ITEM_NO).FirstOrDefault();


                                msg = "Cannot create workflow";
                                msg += "<br/>Sales order no : " + so;
                                msg += "<br/>Item : " + soItem;
                                msg += "<br/>Product code : " + productCode;
                                msg += "<br/>Material code : " + mat5;
                                msg += "<br/>Already assigned in workflow : " + wfNo;

                                break;
                            }
                        }


                        if (string.IsNullOrEmpty(msg))
                        {
                            
                            msg = msg_check_vap;  // 20230121_3V_SOREPAT INC-93118
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                msg += CNService.GetErrorMessage_SORepeat(ex, "ValidatingSaleOrderBySORepeat") + "{Log}";
            }
            return msg;  
        }


        public static void checkAndAssignProductVAP(ref  List<ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2> listSaleOrderRepeat, ARTWORKEntities context)
        {
            // start 20230121_3V_SOREPAT INC-93118
            //var plantVAP = "1021";
            var listPlant = (from m in context.SAP_M_PLANT select m).ToList();

            foreach (var salerepeat in listSaleOrderRepeat)
            {
                var plant_id = listPlant.Where(w => w.PLANT == salerepeat.PRODUCTION_PLANT_DISPLAY_TXT).Select(s => s.PLANT_ID).FirstOrDefault();
                salerepeat.PRODUCT_TYPE = CNService.Getcheck_product_vap(salerepeat.PRODUCT_CODE, plant_id.ToString());
              
            }

        }

        public static string saveWorkFlowProcessbySORepeat(ref ART_WF_ARTWORK_REQUEST_2 objReq, ARTWORKEntities context, string token)
        {
            var msg = "";
            try
            {

                var recheck = "";
                foreach (var soRepeat in objReq.SALES_ORDER_REPEAT)
                {
                    recheck = soRepeat.RECHECK_ARTWORK;
                }

                string request_No = "";
                string oneArtwork_No = "";
                string allArtwork_no = "";
                bool multiArtwork = false;

                request_No = objReq.ARTWORK_REQUEST_NO;

                if (objReq.REQUEST_ITEMS != null)
                {


                    List<SAP_M_PO_COMPLETE_SO_HEADER> listSOHeader = new List<SAP_M_PO_COMPLETE_SO_HEADER>();

                    foreach (var item in objReq.REQUEST_ITEMS)
                    {
                        //save  ART_WF_ARTWORK_REQUEST_ITEM
                        string tempWFNo = item.REQUEST_ITEM_NO;
                        string requestType = recheck == "Yes" ? "ARTWORK_REPEAT_OVER_6_MONTH" : "ARTWORK_REPEAT";
                        string newWFNo = FormNumberHelper.GenArtworkRepeat(context, objReq.ARTWORK_REQUEST_ID, requestType);
                        int NEW_ARTWORK_SUB_ID = 0;
                        item.REQUEST_ITEM_NO = newWFNo;

                        item.ARTWORK_REQUEST_ID = objReq.ARTWORK_REQUEST_ID;
                        ART_WF_ARTWORK_REQUEST_ITEM itemTemp = MapperServices.ART_WF_ARTWORK_REQUEST_ITEM(item);
                        ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.SaveOrUpdate(itemTemp, context);
                        item.ARTWORK_ITEM_ID = itemTemp.ARTWORK_ITEM_ID;

                     


                        if (item.PROCESS_STEP_PA != null)
                        {

                          
                            //Rename folder WF name in CS.
                            long nodeID = 0;
                            var nodeIDTmp = item.PROCESS_STEP_PA.ARTWORK_FOLDER_NODE_ID;
                            if (nodeIDTmp > 0)
                            {
                                nodeID = Convert.ToInt64(nodeIDTmp);
                                CWSService.renameFolder(nodeID, newWFNo, token);
                            }

                            //save ART_WF_ARTWORK_PROCESS_SERVICE Process
                            item.PROCESS_STEP_PA.ARTWORK_ITEM_ID = item.ARTWORK_ITEM_ID;
                            item.PROCESS_STEP_PA.ARTWORK_REQUEST_ID = objReq.ARTWORK_REQUEST_ID;

                            ART_WF_ARTWORK_PROCESS processTemp = MapperServices.ART_WF_ARTWORK_PROCESS(item.PROCESS_STEP_PA);
                            ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(processTemp, context);
                            item.PROCESS_STEP_PA.ARTWORK_SUB_ID = processTemp.ARTWORK_SUB_ID;
                            NEW_ARTWORK_SUB_ID = item.PROCESS_STEP_PA.ARTWORK_SUB_ID;
                            //save ART_WF_ARTWORK_ATTACHMENT_SERVICE
                            if (item.PROCESS_STEP_PA.ATTACHMENT != null)
                            {
                                item.PROCESS_STEP_PA.ATTACHMENT.ARTWORK_REQUEST_ID = objReq.ARTWORK_REQUEST_ID;
                                item.PROCESS_STEP_PA.ATTACHMENT.ARTWORK_SUB_ID = item.PROCESS_STEP_PA.ARTWORK_SUB_ID;
                                ART_WF_ARTWORK_ATTACHMENT_SERVICE.SaveOrUpdate(MapperServices.ART_WF_ARTWORK_ATTACHMENT(item.PROCESS_STEP_PA.ATTACHMENT), context);
                            }


                            //save ART_WF_LOG_DELEGATE_SERVICE Delegation
                            if (item.PROCESS_STEP_PA.LOG_DELEGATE != null)
                            {
                                item.PROCESS_STEP_PA.LOG_DELEGATE.WF_SUB_ID = item.PROCESS_STEP_PA.ARTWORK_SUB_ID;
                                ART_WF_LOG_DELEGATE_SERVICE.SaveOrUpdate(item.PROCESS_STEP_PA.LOG_DELEGATE, context);
                            }

                            //save ART_WF_ARTWORK_PROCESS_PA_SERVICE Process PA
                            if (item.PROCESS_STEP_PA.PROCESS_PA != null)
                            {
                                item.PROCESS_STEP_PA.PROCESS_PA.ARTWORK_SUB_ID = item.PROCESS_STEP_PA.ARTWORK_SUB_ID;

                                ART_WF_ARTWORK_PROCESS_PA processPATemp = MapperServices.ART_WF_ARTWORK_PROCESS_PA(item.PROCESS_STEP_PA.PROCESS_PA);
                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPATemp, context);
                                item.PROCESS_STEP_PA.PROCESS_PA.ARTWORK_SUB_PA_ID = processPATemp.ARTWORK_SUB_PA_ID;

                                var processPA = item.PROCESS_STEP_PA.PROCESS_PA;
                                //save ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE
                                if (processPA.CATCHING_AREAS != null && processPA.CATCHING_AREAS.Count > 0)
                                {
                                    foreach (var catching_area in processPA.CATCHING_AREAS)
                                    {
                                        catching_area.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        catching_area.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA(catching_area), context);
                                    }
                                }

                                //save ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE
                                if (processPA.FAOS != null && processPA.FAOS.Count > 0)
                                {
                                    foreach (var faos in processPA.FAOS)
                                    {
                                        faos.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        faos.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE(faos), context);
                                    }
                                }

                                //save ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE
                                if (processPA.CATCHING_METHODS != null && processPA.CATCHING_METHODS.Count > 0)
                                {
                                    foreach (var catching_method in processPA.CATCHING_METHODS)
                                    {
                                        catching_method.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        catching_method.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD(catching_method), context);
                                    }
                                }

                                //save ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE
                                if (processPA.SYMBOLS != null && processPA.SYMBOLS.Count > 0)
                                {
                                    foreach (var symbol in processPA.SYMBOLS)
                                    {
                                        symbol.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        symbol.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_PROCESS_PA_SYMBOL(symbol), context);
                                    }
                                }

                                //save ART_WF_ARTWORK_PROCESS_PA_PLANT_SERVICE
                                if (processPA.PLANTS != null && processPA.PLANTS.Count > 0)
                                {
                                    foreach (var plant in processPA.PLANTS)
                                    {
                                        plant.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        plant.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        ART_WF_ARTWORK_PROCESS_PA_PLANT_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_PROCESS_PA_PLANT(plant), context);
                                    }
                                }

                                //save ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE
                                if (processPA.PRODUCTS != null && processPA.PRODUCTS.Count > 0)
                                {
                                    foreach (var product in processPA.PRODUCTS)
                                    {
                                        product.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        product.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_PROCESS_PA_PRODUCT(product), context);
                                    }
                                }

                                //save ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE
                                if (processPA.PRODUCT_OTHERS != null && processPA.PRODUCT_OTHERS.Count > 0)
                                {
                                    foreach (var product_other in processPA.PRODUCT_OTHERS)
                                    {
                                        product_other.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        product_other.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;

                                        ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER(product_other), context);
                                    }
                                }

                            }

                            //save ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE
                            if (item.PROCESS_STEP_PA.LIST_PROCESS_SO_DETAIL != null && item.PROCESS_STEP_PA.LIST_PROCESS_SO_DETAIL.Count > 0)
                            {
                                foreach (var soDetail in item.PROCESS_STEP_PA.LIST_PROCESS_SO_DETAIL)
                                {
                                    soDetail.ARTWORK_REQUEST_ID = objReq.ARTWORK_REQUEST_ID;
                                    soDetail.ARTWORK_SUB_ID = item.PROCESS_STEP_PA.ARTWORK_SUB_ID;

                                    ART_WF_ARTWORK_PROCESS_SO_DETAIL soDetailTemp = MapperServices.ART_WF_ARTWORK_PROCESS_SO_DETAIL(soDetail);
                                    ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.SaveOrUpdateNoLog(soDetailTemp, context);
                                    soDetail.ARTWORK_PROCESS_SO_ID = soDetailTemp.ARTWORK_PROCESS_SO_ID;

                                    // save ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_SERVICE
                                    if (item.PROCESS_STEP_PA.LIST_ASSIGN_SO_LONG_TEXT != null && item.PROCESS_STEP_PA.LIST_ASSIGN_SO_LONG_TEXT.Count > 0)
                                    {
                                        var tempSOLongText = item.PROCESS_STEP_PA.LIST_ASSIGN_SO_LONG_TEXT.Where(w => w.TEMP_RUNNING_ID == soDetail.TEMP_RUNNING_ID).ToList();
                                        foreach (var soLongText in tempSOLongText)
                                        {
                                            soLongText.ARTWORK_PROCESS_SO_ID = soDetail.ARTWORK_PROCESS_SO_ID;
                                            soLongText.ARTWORK_REQUEST_ID = soDetail.ARTWORK_REQUEST_ID;
                                            soLongText.ARTWORK_SUB_ID = soDetail.ARTWORK_SUB_ID;
                                            ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT soLongTextTemp = MapperServices.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT(soLongText);
                                            ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_SERVICE.SaveOrUpdateNoLog(soLongTextTemp, context);
                                        }
                                    }

                                    //save ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_SERVICE
                                    if (soDetail.ASSIGN_SO_HEADER != null)
                                    {
                                        if (soDetail.ASSIGN_SO_HEADER.SO_HEADER != null)
                                        {
                                            listSOHeader.Add(soDetail.ASSIGN_SO_HEADER.SO_HEADER);
                                        }

                                        soDetail.ASSIGN_SO_HEADER.ARTWORK_PROCESS_SO_ID = soDetail.ARTWORK_PROCESS_SO_ID;
                                        soDetail.ASSIGN_SO_HEADER.ARTWORK_REQUEST_ID = soDetail.ARTWORK_REQUEST_ID;
                                        soDetail.ASSIGN_SO_HEADER.ARTWORK_SUB_ID = soDetail.ARTWORK_SUB_ID;

                                        ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER assignSOHeaderTemp = MapperServices.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER(soDetail.ASSIGN_SO_HEADER);
                                        ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_SERVICE.SaveOrUpdateNoLog(assignSOHeaderTemp, context);
                                        soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_HEADER_ID = assignSOHeaderTemp.ASSIGN_SO_HEADER_ID;

                                        //save ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_SERVICE
                                        if (soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM != null)
                                        {
                                            soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ASSIGN_SO_HEADER_ID = soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_HEADER_ID;
                                            soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ARTWORK_PROCESS_SO_ID = soDetail.ARTWORK_PROCESS_SO_ID;
                                            soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ARTWORK_REQUEST_ID = soDetail.ARTWORK_REQUEST_ID;
                                            soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ARTWORK_SUB_ID = soDetail.ARTWORK_SUB_ID;

                                            ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM assignSOItemTemp = MapperServices.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM(soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM);
                                            ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_SERVICE.SaveOrUpdateNoLog(assignSOItemTemp, context);
                                            soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ASSIGN_SO_ITEM_ID = assignSOItemTemp.ASSIGN_SO_ITEM_ID;

                                            ////update SAP_M_PO_COMPLETE_SO_ITEM_SERVICE for is_assign field.
                                            if (soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.SAP_SO_ITEM_2 != null)
                                            {
                                                if (soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.SAP_SO_ITEM_2.PO_COMPLETE_SO_ITEM_ID > 0)
                                                {
                                                   // SAP_M_PO_COMPLETE_SO_ITEM soitemTemp = MapperServices.SAP_M_PO_COMPLETE_SO_ITEM(soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.SAP_SO_ITEM_2);
                                                    SAP_M_PO_COMPLETE_SO_ITEM_SERVICE.UpdateNoLog(soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.SAP_SO_ITEM_2, context);
                                                }
                                            }

                                            //save ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_SERVICE
                                            if (soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ASSIGN_SO_ITEM_COMPONENT != null)
                                            {
                                                soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ASSIGN_SO_ITEM_COMPONENT.ASSIGN_SO_ITEM_ID = soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ASSIGN_SO_ITEM_ID;
                                                soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ASSIGN_SO_ITEM_COMPONENT.ARTWORK_PROCESS_SO_ID = soDetail.ARTWORK_PROCESS_SO_ID;
                                                soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ASSIGN_SO_ITEM_COMPONENT.ARTWORK_REQUEST_ID = soDetail.ARTWORK_REQUEST_ID;
                                                soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ASSIGN_SO_ITEM_COMPONENT.ARTWORK_SUB_ID = soDetail.ARTWORK_SUB_ID;

                                                ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT assignSOItemCompTemp = MapperServices.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT(soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ASSIGN_SO_ITEM_COMPONENT);
                                                ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_SERVICE.SaveOrUpdateNoLog(assignSOItemCompTemp, context);

                                                //update SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_SERVICE for is_assign field.
                                                if (soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ASSIGN_SO_ITEM_COMPONENT.SAP_SO_ITEM_COMPONENT != null)
                                                {
                                                    if (soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ASSIGN_SO_ITEM_COMPONENT.SAP_SO_ITEM_COMPONENT.PO_COMPLETE_SO_ITEM_COMPONENT_ID  > 0)
                                                    {
                                                       // SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT soItemCompTemp = MapperServices.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT(soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ASSIGN_SO_ITEM_COMPONENT.SAP_SO_ITEM_COMPONENT);
                                                        SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_SERVICE.UpdateNoLog(soDetail.ASSIGN_SO_HEADER.ASSIGN_SO_ITEM.ASSIGN_SO_ITEM_COMPONENT.SAP_SO_ITEM_COMPONENT, context);
                                                    }
                                                }
                                            }



                                        }

                                    }

                                }
                            }                    
                        }

                        //save ART_WF_ARTWORK_PROCESS_SERVICE
                        if (item.PROCESS_STEP_PG != null)
                        {
                            item.PROCESS_STEP_PG.ARTWORK_ITEM_ID = item.ARTWORK_ITEM_ID;                     
                            item.PROCESS_STEP_PG.ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID;
                            item.PROCESS_STEP_PG.PARENT_ARTWORK_SUB_ID = item.PROCESS_STEP_PA.ARTWORK_SUB_ID;

                            ART_WF_ARTWORK_PROCESS processStepPGTemp = MapperServices.ART_WF_ARTWORK_PROCESS(item.PROCESS_STEP_PG);
                            ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(processStepPGTemp, context);
                            item.PROCESS_STEP_PG.ARTWORK_SUB_ID = processStepPGTemp.ARTWORK_SUB_ID;

                            //save ART_WF_ARTWORK_PROCESS_PG_SERVICE
                            if (item.PROCESS_STEP_PG.PROCESS_PG != null)
                            {
                                item.PROCESS_STEP_PG.PROCESS_PG.ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID;
                                item.PROCESS_STEP_PG.PROCESS_PG.ARTWORK_SUB_ID = item.PROCESS_STEP_PG.ARTWORK_SUB_ID;

                                ART_WF_ARTWORK_PROCESS_PG processPGTemp = MapperServices.ART_WF_ARTWORK_PROCESS_PG(item.PROCESS_STEP_PG.PROCESS_PG);
                                ART_WF_ARTWORK_PROCESS_PG_SERVICE.SaveOrUpdate(processPGTemp, context);
                            }


                        }

                        //ART_WF_ARTWORK_PROCESS_SERVICE
                        if (item.PROCESS_STEP_PP != null)
                        {
                            item.PROCESS_STEP_PP.PARENT_ARTWORK_SUB_ID = item.PROCESS_STEP_PA.PROCESS_PA.ARTWORK_SUB_ID;
                            item.PROCESS_STEP_PP.ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID;
                            item.PROCESS_STEP_PP.ARTWORK_ITEM_ID = item.ARTWORK_ITEM_ID;
                            ART_WF_ARTWORK_PROCESS processStepPPTemp = MapperServices.ART_WF_ARTWORK_PROCESS(item.PROCESS_STEP_PP);
                            ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(processStepPPTemp, context);
                            item.PROCESS_STEP_PP.ARTWORK_SUB_ID = processStepPPTemp.ARTWORK_SUB_ID;

                            if (item.PROCESS_STEP_PP.PROCESS_PP_BY_PA != null )
                            {
                                item.PROCESS_STEP_PP.PROCESS_PP_BY_PA.ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID;
                                item.PROCESS_STEP_PP.PROCESS_PP_BY_PA.ARTWORK_SUB_ID = item.PROCESS_STEP_PP.ARTWORK_SUB_ID;
                                ART_WF_ARTWORK_PROCESS_PP_BY_PA_SERVICE.SaveOrUpdate(item.PROCESS_STEP_PP.PROCESS_PP_BY_PA, context);
                            }
                        }


                        
                        var tagLinkWF = "<a target='_blank' href='/TaskFormArtwork/" + NEW_ARTWORK_SUB_ID + "'>" + newWFNo + "</a>";
                        if (allArtwork_no == "")
                        {
                            //oneArtwork_No = newWFNo;
                            oneArtwork_No = tagLinkWF;
                           
                            allArtwork_no = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + oneArtwork_No;
                        }
                        else
                        {
                            multiArtwork = true;
                            allArtwork_no += "<br/>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + tagLinkWF;
                        }

                    }



                    // complete PA form for Repeat
                    // end task form
                    // send mail


                    // update assign so header
                    if (listSOHeader != null && listSOHeader.Count > 0 )
                    {
                        listSOHeader = listSOHeader.Distinct().ToList();
                        foreach (var soHeader in listSOHeader)
                        {
                            var cntSO = CNService.GetAssignOrder(soHeader.PO_COMPLETE_SO_HEADER_ID, context);
                            if (cntSO == 0)
                            {
                                soHeader.IS_ASSIGN = "X";
                                SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.UpdateNoLog(soHeader, context);
                            }
                        }
                    }

                  

                    var result_msg_wf = "";
                    if (!String.IsNullOrEmpty(request_No))
                    {
                        //result_msg_wf += "Request form no. : " + request_No + "<br/>";
                        result_msg_wf += "Request Form No : <a target='_blank' href='/Artwork/" + objReq.ARTWORK_REQUEST_ID + "?so=sorepeat'>" + request_No + "</a><br/>";

                    }

                    if (multiArtwork)
                    {
                        result_msg_wf += "Artwork request no. : " + "<br/>";
                        result_msg_wf += allArtwork_no;
                    }
                    else
                    {
                        result_msg_wf += "Artwork request no. : " + oneArtwork_No;
                    }


                    if (objReq.IS_SEND_TO_PP)
                    {
                        result_msg_wf += "<br/>Send to PP successfully.";
                    }


                    objReq.RESULT_CREATE_WF_WFNO = result_msg_wf;

                }


            }
            catch (Exception ex)
            {
                msg = CNService.GetErrorMessage_SORepeat(ex, "saveWorkFlowProcessbySORepeat") + "{Log}";
            }
            return msg;

        }

        public static string saveArtworkRequestbySORepeat(ref ART_WF_ARTWORK_REQUEST_2 obj, ARTWORKEntities context, string token)
        {
            var msg = "";
            try
            {
                //save ART_WF_ARTWORK_REQUEST_SERVICE
                string newFormRequestNo = FormNumberHelper.GenArtworkRequestFormNo(context);
                string tempFormRequestNo = obj.ARTWORK_REQUEST_NO;

                obj.REQUEST_FROM_NO_ERROR = obj.ARTWORK_REQUEST_NO;
                obj.ARTWORK_REQUEST_NO = newFormRequestNo;

                ART_WF_ARTWORK_REQUEST objReqTemp = MapperServices.ART_WF_ARTWORK_REQUEST(obj);
                ART_WF_ARTWORK_REQUEST_SERVICE.SaveOrUpdate(objReqTemp, context);
                obj.ARTWORK_REQUEST_ID = objReqTemp.ARTWORK_REQUEST_ID;
              

                //Rename folder name in CS.
                long nodeID = 0;
                var nodeIDTmp = obj.REQUEST_FORM_FOLDER_NODE_ID;

                if (nodeIDTmp > 0)
                {
                    nodeID = Convert.ToInt64(nodeIDTmp);
                    CWSService.renameFolder(nodeID, newFormRequestNo, token);
                    obj.REQUEST_FROM_NO_ERROR = newFormRequestNo;
                }

                //save ART_WF_ARTWORK_REQUEST_RECIPIENT_SERVICE.
                if (obj.REQUEST_RECIPIENT != null && obj.REQUEST_RECIPIENT.Count > 0)
                {
                    foreach (ART_WF_ARTWORK_REQUEST_RECIPIENT_2 item in obj.REQUEST_RECIPIENT)
                    {
                        item.ARTWORK_REQUEST_ID = obj.ARTWORK_REQUEST_ID;
                        ART_WF_ARTWORK_REQUEST_RECIPIENT_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_REQUEST_MARKETING(item), context);
                    }

                }

                //ART_WF_ARTWORK_REQUEST_SALES_ORDER
                if (obj.SALES_ORDER != null && obj.SALES_ORDER.Count > 0)
                {
                    foreach (ART_WF_ARTWORK_REQUEST_SALES_ORDER_2 item in obj.SALES_ORDER)
                    {
                        item.ARTWORK_REQUEST_ID = obj.ARTWORK_REQUEST_ID;
                        ART_WF_ARTWORK_REQUEST_SALES_ORDER_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_REQUEST_SALES_ORDER(item), context);
                    }

                }

                //ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                if (obj.SALES_ORDER_REPEAT != null && obj.SALES_ORDER_REPEAT.Count > 0)
                {
                    foreach (ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2 item in obj.SALES_ORDER_REPEAT)
                    {
                        item.ARTWORK_REQUEST_ID = obj.ARTWORK_REQUEST_ID;
                        ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT(item), context);
                    }

                }


                //ART_WF_ARTWORK_REQUEST_ITEM
                if (obj.REQUEST_ITEMS != null && obj.REQUEST_ITEMS.Count > 0)
                {
                    foreach (ART_WF_ARTWORK_REQUEST_ITEM_2 item in obj.REQUEST_ITEMS)
                    {
                        item.ARTWORK_REQUEST_ID = obj.ARTWORK_REQUEST_ID;
                        ART_WF_ARTWORK_REQUEST_ITEM itemTemp = MapperServices.ART_WF_ARTWORK_REQUEST_ITEM(item);
                        ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.SaveOrUpdateNoLog(itemTemp, context);
                        item.ARTWORK_ITEM_ID = itemTemp.ARTWORK_ITEM_ID;
                    }

                }

                //ART_WF_ARTWORK_REQUEST_PRODUCT  
                if (obj.REQUEST_PRODUCT  != null && obj.REQUEST_PRODUCT.Count > 0)
                {
                    foreach (ART_WF_ARTWORK_REQUEST_PRODUCT_2 item in obj.REQUEST_PRODUCT)
                    {
                        item.ARTWORK_REQUEST_ID = obj.ARTWORK_REQUEST_ID;
                        ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCT(item), context);
                    }
                }

                //ART_WF_ARTWORK_REQUEST_CUSTOMER
                if (obj.MAIL_TO_CUSTOMER != null && obj.MAIL_TO_CUSTOMER.Count > 0)
                {
                    foreach (ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_2 item in obj.MAIL_TO_CUSTOMER)
                    {
                        item.ARTWORK_REQUEST_ID = obj.ARTWORK_REQUEST_ID;
                        ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER(item), context);
                    }
                }
                //ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT
                if (obj.PRODUCTION_PLANT != null && obj.PRODUCTION_PLANT.Count > 0)
                {
                    foreach (ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2 item in obj.PRODUCTION_PLANT)
                    {
                        item.ARTWORK_REQUEST_ID = obj.ARTWORK_REQUEST_ID;
                        ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT(item), context);
                    }
                }
                //ART_WF_ARTWORK_REQUEST_COUNTRY
                if (obj.COUNTRY !=null && obj.COUNTRY.Count > 0)
                {
                    foreach (ART_WF_ARTWORK_REQUEST_COUNTRY_2 item in obj.COUNTRY)
                    {
                        item.ARTWORK_REQUEST_ID = obj.ARTWORK_REQUEST_ID;
                        ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_REQUEST_COUNTRY(item), context);
                    }
                }



                //obj.RESULT_CREATE_WF_WFNO = "Request Form No : " + newFormRequestNo;
                obj.RESULT_CREATE_WF_WFNO = "Request Form No : <a target='_blank' href='/Artwork/" + obj.ARTWORK_REQUEST_ID + "?so=sorepeat'>" + newFormRequestNo + "</a>";

            }

            catch (Exception ex)

            {
                msg = CNService.GetErrorMessage_SORepeat(ex, "saveArtworkRequestbySORepeat") + "{Log}";
            }

            return msg;

        }

        //----------------------------end tuning performance sorepeat 2022 by aof-------------------------------------------------------------------------//


    }
}