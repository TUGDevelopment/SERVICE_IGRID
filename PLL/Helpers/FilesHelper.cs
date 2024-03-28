using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using BLL.DocumentManagement;
using BLL.Helpers;
using DAL;
using BLL.Services;
using System.Configuration;
using DAL.Model;
using System.IO.Compression;
using System.Text;
using System.Collections;
using ExcelDataReader;
using ClosedXML.Excel;

namespace PLL.Helpers
{
    public class FilesHelper
    {
        public void UploadAndShowResultsold(int mockupSubId, int userId, int? roldId, HttpContextBase ContentBase, List<ViewDataUploadFilesResult> resultList
            , int? version, string is_internal, string is_customer, string is_vendor)
        {
            var request = ContentBase.Request;
            for (int i = 0; i < request.Files.Count; i++)
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        ART_WF_MOCKUP_PROCESS process = new ART_WF_MOCKUP_PROCESS();
                        ART_WF_MOCKUP_CHECK_LIST_ITEM check_list_item = new ART_WF_MOCKUP_CHECK_LIST_ITEM();
                        process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(mockupSubId, context);
                        check_list_item = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(process.MOCKUP_ID, context);
                        var file = request.Files[i];
                        var SEND_VN_QUO = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_VN_QUO" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_VN_DL = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_VN_DL" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_VN_MB = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_VN_MB" }, context).FirstOrDefault().STEP_MOCKUP_ID;

                        var folderName = ConfigurationManager.AppSettings["MockupFolderNameOther"];
                        if (SEND_VN_QUO == process.CURRENT_STEP_ID)
                        {
                            folderName = ConfigurationManager.AppSettings["MockupFolderNameQuotation"];
                        }
                        else if (SEND_VN_DL == process.CURRENT_STEP_ID)
                        {
                            folderName = ConfigurationManager.AppSettings["MockupFolderNameDieline"];
                        }
                        else if (SEND_VN_MB == process.CURRENT_STEP_ID)
                        {
                            folderName = ConfigurationManager.AppSettings["MockupFolderNameMatchboard"];
                        }

                        var token = CWSService.getAuthToken();
                        Node nodeOthers = BLL.Services.CWSService.getNodeByName(Convert.ToInt64(check_list_item.NODE_ID), folderName, token);

                        Node node = BLL.Services.CWSService.uploadFile(file.InputStream, file.FileName, nodeOthers.ID, token);
                        string extension = Path.GetExtension(file.FileName).Replace(".", "");
                        string contentType = file.ContentType;
                        var att = AttachmentMockupHelper.SaveAttachment(node.Name, extension, contentType, process.MOCKUP_ID, mockupSubId, file.ContentLength, node.ID, userId, roldId
                                , version, is_internal, is_customer, is_vendor, context);

                        resultList.Add(UploadResult_Mockup(file.FileName, file.ContentLength, node.ID, att, context));

                        dbContextTransaction.Commit();
                    }
                }
            }
        }
        public void UploadAndShowResults(int mockupSubId, int userId, int? roldId, HttpContextBase ContentBase, List<ViewDataUploadFilesResult> resultList
                         , int? version, string is_internal, string is_customer, string is_vendor)
        {
            var request = ContentBase.Request;
            string fileTypeNotAllowed = "";
            fileTypeNotAllowed = ConfigurationManager.AppSettings["FileTypeNotAllowed"];

            for (int i = 0; i < request.Files.Count; i++)
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        ART_WF_MOCKUP_PROCESS process = new ART_WF_MOCKUP_PROCESS();
                        ART_WF_MOCKUP_CHECK_LIST_ITEM check_list_item = new ART_WF_MOCKUP_CHECK_LIST_ITEM();
                        process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(mockupSubId, context);
                        check_list_item = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(process.MOCKUP_ID, context);
                        var file = request.Files[i];
                        var SEND_VN_QUO = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_VN_QUO" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_VN_DL = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_VN_DL" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_VN_MB = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_VN_MB" }, context).FirstOrDefault().STEP_MOCKUP_ID;

                        var folderName = ConfigurationManager.AppSettings["MockupFolderNameOther"];
                        if (SEND_VN_QUO == process.CURRENT_STEP_ID)
                        {
                            folderName = ConfigurationManager.AppSettings["MockupFolderNameQuotation"];
                        }
                        else if (SEND_VN_DL == process.CURRENT_STEP_ID)
                        {
                            folderName = ConfigurationManager.AppSettings["MockupFolderNameDieline"];
                        }
                        else if (SEND_VN_MB == process.CURRENT_STEP_ID)
                        {
                            folderName = ConfigurationManager.AppSettings["MockupFolderNameMatchboard"];
                        }

                        string extension = Path.GetExtension(file.FileName).Replace(".", "");
                        if (fileTypeNotAllowed.Contains(extension.ToLower()))
                        {
                            var token = CWSService.getAuthToken();
                            Node nodeOthers = BLL.Services.CWSService.getNodeByName(Convert.ToInt64(check_list_item.NODE_ID), folderName, token);

                            Node node = BLL.Services.CWSService.uploadFile(file.InputStream, file.FileName, nodeOthers.ID, token);
                            //string extension = Path.GetExtension(file.FileName).Replace(".", "");
                            string contentType = file.ContentType;
                            var att = AttachmentMockupHelper.SaveAttachment(node.Name, extension, contentType, process.MOCKUP_ID, mockupSubId, file.ContentLength, node.ID, userId, roldId
                                    , version, is_internal, is_customer, is_vendor, context);

                            resultList.Add(UploadResult_Mockup(file.FileName, file.ContentLength, node.ID, att, context));
                        }
                        dbContextTransaction.Commit();
                    }
                }
            }
        }
        public void UploadAndShowResultsFileVersion(int mockupSubId, int userId, int nodeId, HttpContextBase ContentBase, List<ViewDataUploadFilesResult> resultList)
        {
            var request = ContentBase.Request;
            for (int i = 0; i < request.Files.Count; i++)
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        ART_WF_MOCKUP_PROCESS process = new ART_WF_MOCKUP_PROCESS();
                        ART_WF_MOCKUP_CHECK_LIST_ITEM check_list_item = new ART_WF_MOCKUP_CHECK_LIST_ITEM();
                        process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(mockupSubId, context);
                        check_list_item = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(process.MOCKUP_ID, context);
                        var file = request.Files[i];

                        var SEND_VN_QUO = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_VN_QUO" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_VN_DL = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_VN_DL" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_VN_MB = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_VN_MB" }, context).FirstOrDefault().STEP_MOCKUP_ID;

                        var folderName = ConfigurationManager.AppSettings["MockupFolderNameOther"];
                        if (SEND_VN_QUO == process.CURRENT_STEP_ID)
                        {
                            folderName = ConfigurationManager.AppSettings["MockupFolderNameQuotation"];
                        }
                        else if (SEND_VN_DL == process.CURRENT_STEP_ID)
                        {
                            folderName = ConfigurationManager.AppSettings["MockupFolderNameDieline"];
                        }
                        else if (SEND_VN_MB == process.CURRENT_STEP_ID)
                        {
                            folderName = ConfigurationManager.AppSettings["MockupFolderNameMatchboard"];
                        }

                        var token = CWSService.getAuthToken();
                        Node nodeOthers = BLL.Services.CWSService.getNodeByName(Convert.ToInt64(check_list_item.NODE_ID), folderName, token);
                        var node = BLL.Services.CWSService.addVersionFile(file.InputStream, file.FileName, nodeOthers.ID, nodeId, token);
                        long last_version = node.VerMinor;

                        string extension = Path.GetExtension(file.FileName).Replace(".", "");
                        string contentType = file.ContentType;
                        var att = AttachmentMockupHelper.SaveAttachmentFileVersion(node.Filename, extension, contentType, process.MOCKUP_ID, mockupSubId, file.ContentLength, nodeId, userId, last_version, context);

                        resultList.Add(UploadResult_Mockup(file.FileName, file.ContentLength, node.ID, att, context));

                        dbContextTransaction.Commit();
                    }
                }
            }
        }

        public void UploadAndShowResultsFileVersion_Artwork(int mockupSubId, int userId, int nodeId, HttpContextBase ContentBase, List<ViewDataUploadFilesResult> resultList)
        {
            var request = ContentBase.Request;
            for (int i = 0; i < request.Files.Count; i++)
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        ART_WF_ARTWORK_PROCESS process = new ART_WF_ARTWORK_PROCESS();
                        ART_WF_ARTWORK_REQUEST_ITEM request_item = new ART_WF_ARTWORK_REQUEST_ITEM();
                        process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(mockupSubId, context);
                        request_item = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(process.ARTWORK_ITEM_ID, context);
                        var file = request.Files[i];

                        var folderName = ConfigurationManager.AppSettings["ArtworkFolderNameOther"];
                        var SEND_VN_PM = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_VN_PM" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                        if (SEND_VN_PM == process.CURRENT_STEP_ID)
                        {
                            folderName = ConfigurationManager.AppSettings["ArtworkFolderNamePrintMaster"];
                        }

                        var token = CWSService.getAuthToken();
                        Node nodeOthers = new Node();
                        if (process != null)
                        {
                            var processParent = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = process.ARTWORK_ITEM_ID }, context)
                            .Where(m => m.PARENT_ARTWORK_SUB_ID == null).FirstOrDefault();

                            if (processParent.ARTWORK_FOLDER_NODE_ID != null)
                            {
                                nodeOthers = BLL.Services.CWSService.getNodeByName(Convert.ToInt64(processParent.ARTWORK_FOLDER_NODE_ID), folderName, token);
                            }
                            else
                            {
                                nodeOthers = BLL.Services.CWSService.getNodeByName(Convert.ToInt64(process.ARTWORK_FOLDER_NODE_ID), folderName, token);
                            }
                        }

                        var node = BLL.Services.CWSService.addVersionFile(file.InputStream, file.FileName, nodeOthers.ID, nodeId, token);
                        long last_version = node.VerMinor;

                        string extension = Path.GetExtension(file.FileName).Replace(".", "");
                        string contentType = file.ContentType;
                        var att = AttachmentArtworkHelper.SaveAttachmentFileVersion(node.Filename, extension, contentType, process.ARTWORK_REQUEST_ID, mockupSubId, file.ContentLength, nodeId, userId, last_version, context);

                        resultList.Add(UploadResult_Artwork(file.FileName, file.ContentLength, nodeId, att, context));

                        dbContextTransaction.Commit();
                    }
                }
            }
        }

        public void UploadAndShowResultsFilePIC(int userId, string action, HttpContextBase ContentBase, List<ViewDataUploadFilesResult> resultList)
        {
            var request = ContentBase.Request;
            ART_WF_ARTWORK_REQUEST_2 artwork2 = new ART_WF_ARTWORK_REQUEST_2();
            ART_WF_ARTWORK_REQUEST artwork = new ART_WF_ARTWORK_REQUEST();
            ViewDataUploadFilesResult Results = new ViewDataUploadFilesResult();
            StringBuilder sbMsg = new StringBuilder();

            try
            {


                List<string> listUserName = new List<string>();
                List<string> listSoldTo = new List<string>();
                List<string> listShipTo = new List<string>();
                List<string> listZone = new List<string>();
                List<string> listCountry = new List<string>();

                List<ART_M_PIC_2> listPICData = new List<ART_M_PIC_2>();
                ART_M_PIC_2 picData = new ART_M_PIC_2();

                //Upload Files
                for (int i = 0; i < request.Files.Count; i++)
                {

                    var file = request.Files[i];
                    using (var excelWorkbook = new XLWorkbook(file.InputStream))
                    {
                        var nonEmptyDataRows = excelWorkbook.Worksheet(1).RowsUsed();

                        foreach (var dataRow in nonEmptyDataRows)
                        {
                            //for row number check
                            if (dataRow.RowNumber() > 1)
                            {
                                string cellID = dataRow.Cell(1).Value.ToString();
                                string cellUserName = dataRow.Cell(2).Value.ToString();
                                string cellSoldTo = dataRow.Cell(5).Value.ToString();
                                string cellShipTo = dataRow.Cell(7).Value.ToString();
                                string cellZone = dataRow.Cell(9).Value.ToString();
                                string cellCountry = dataRow.Cell(10).Value.ToString();


                                if (!listUserName.Contains(cellUserName))
                                {
                                    listUserName.Add(cellUserName);
                                }
                                if (!listSoldTo.Contains(cellSoldTo))
                                {
                                    listSoldTo.Add(cellSoldTo);
                                }
                                if (!listShipTo.Contains(cellShipTo))
                                {
                                    listShipTo.Add(cellShipTo);
                                }
                                if (!listZone.Contains(cellZone))
                                {
                                    listZone.Add(cellZone);
                                }
                                if (!listCountry.Contains(cellCountry))
                                {
                                    listCountry.Add(cellCountry);
                                }

                                int picID = 0;

                                picData = new ART_M_PIC_2();

                                if (int.TryParse(cellID, out picID))
                                {
                                    picData.PIC_ID = picID;
                                }

                                picData.USER_DISPLAY_TXT = cellUserName;
                                picData.SOLD_TO_CODE = cellSoldTo;
                                picData.SHIP_TO_CODE = cellShipTo;
                                picData.ZONE = cellZone;
                                picData.COUNTRY = cellCountry;
                                listPICData.Add(picData);
                            }
                        }
                    }
                }

                string validate = "";
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        validate = ValidatePIC(context, listUserName, listSoldTo, listShipTo, listZone, listCountry, listPICData, action);
                    }
                }

                //validate = "";
                Results = new ViewDataUploadFilesResult();

                if (!String.IsNullOrEmpty(validate))
                {
                    validate = validate.Replace("\r\n", "<br>");
                    resultList.Add(UploadResult_PIC(request.Files[0].FileName, 100, validate));


                }
                else
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (var dbContextTransaction = CNService.IsolationLevel(context))
                        {

                            var listUser = (from u in context.ART_M_USER
                                            select new ART_M_USER_2
                                            {
                                                USER_ID = u.USER_ID,
                                                USERNAME = u.USERNAME
                                            });
                            ART_M_PIC picdata = new ART_M_PIC();
                            foreach (var itemPICNew in listPICData)
                            {
                                picdata = new ART_M_PIC();

                                picdata.SOLD_TO_CODE = itemPICNew.SOLD_TO_CODE;
                                picdata.SHIP_TO_CODE = itemPICNew.SHIP_TO_CODE;
                                picdata.ZONE = itemPICNew.ZONE;
                                picdata.COUNTRY = itemPICNew.COUNTRY;
                                picdata.USER_ID = listUser.Where(w => w.USERNAME == itemPICNew.USER_DISPLAY_TXT).Select(s => s.USER_ID).FirstOrDefault();
                                picdata.IS_ACTIVE = "X";
                                picdata.CREATE_BY = userId;
                                picdata.UPDATE_BY = userId;
                               
                                if (action.Equals("EDIT") && itemPICNew.PIC_ID > 0)
                                {
                                    picdata.PIC_ID = itemPICNew.PIC_ID;
                                    ART_M_PIC_SERVICE.SaveOrUpdateNoLog(picdata, context);
                                }
                                else if (action.Equals("ADD"))
                                {
                                    ART_M_PIC_SERVICE.SaveOrUpdateNoLog(picdata, context);
                                }
                                //var allPICConfig = ART_M_PIC_SERVICE.GetAll(context);
                                //CNService.UpdatePIC(picdata, allPICConfig);
                            }
                            dbContextTransaction.Commit();
                        }

                    }

                    // Results.msg = MessageHelper.GetMessage("MSG_001");
                    resultList.Add(UploadResult_PIC(request.Files[0].FileName, request.Files[0].ContentLength, "")); // MessageHelper.GetMessage("MSG_001")
                }



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string ValidatePIC(ARTWORKEntities context,
                                            List<string> listUserName,
                                            List<string> listSoldTo,
                                            List<string> listShipTo,
                                            List<string> listZone,
                                            List<string> listCountry,
                                            List<ART_M_PIC_2> listPICData,
                                            string action)
        {
            string strValidateMsg = "";
            StringBuilder sbMsg = new StringBuilder();

            listCountry = (from c in listCountry
                           where !String.IsNullOrEmpty(c)
                           select c).ToList();

            var list_M_Msg = (from p in context.ART_M_MESSAGE
                              select new ART_M_MESSAGE_2
                              {
                                  MSG_ID = p.MSG_ID,
                                  MSG_CODE = p.MSG_CODE,
                                  MSG_DESCRIPTION = p.MSG_DESCRIPTION
                              }).ToList();

            var list_M_CustomerCode = (from p in context.XECM_M_CUSTOMER
                                       select p.CUSTOMER_CODE).Distinct().ToList();

            var list_M_Zone = (from p in context.SAP_M_COUNTRY
                               select p.ZONE).Distinct().ToList();

            var list_M_Country = (from p in context.SAP_M_COUNTRY
                                  select p.COUNTRY_CODE).Distinct().ToList();



            ART_M_USER_REQUEST uParam = new ART_M_USER_REQUEST();
            ART_M_USER_2 uData = new ART_M_USER_2();

            uParam.data = uData;
            var userM = UserHelper.GetUserPIC(uParam);

            List<string> list_M_User = new List<string>();

            if (userM != null && userM.data != null)
            {
                list_M_User = (from p in userM.data
                               select p.USERNAME).Distinct().ToList();
            }

            #region "Check Required not nul or empty"
            var cntUserNull = (from result in listUserName
                               where String.IsNullOrEmpty(result) == true
                               select result).ToList().Count();

            var cntSoldNull = (from result in listSoldTo
                               where String.IsNullOrEmpty(result) == true
                               select result).ToList().Count();

            var cntShipNull = (from result in listShipTo
                               where String.IsNullOrEmpty(result) == true
                               select result).ToList().Count();

            var cntZoneNull = (from result in listZone
                               where String.IsNullOrEmpty(result) == true
                               select result).ToList().Count();

            if (cntUserNull > 0 || cntSoldNull > 0 || cntShipNull > 0 || cntZoneNull > 0)
            {
                sbMsg.AppendLine(list_M_Msg.Where(w => w.MSG_CODE == "MSG_034").Select(s => s.MSG_DESCRIPTION).FirstOrDefault());

                if (cntUserNull > 0)
                {
                    sbMsg.AppendLine("   - Person in charge is require. ");
                }

                if (cntSoldNull > 0)
                {
                    sbMsg.AppendLine("   - Sold-to is require. ");
                }

                if (cntShipNull > 0)
                {
                    sbMsg.AppendLine("   - Ship-to is require. ");
                }

                if (cntZoneNull > 0)
                {
                    sbMsg.AppendLine("   - Zone is require. ");
                }

                strValidateMsg = sbMsg.ToString();

                return strValidateMsg;
            }
            #endregion

            #region "Check Exist data in Master"

            List<string> listSoldInMaster = list_M_CustomerCode.Where(w => listSoldTo.Distinct().Contains(w)).ToList();
            List<string> listShipInMaster = list_M_CustomerCode.Where(w => listShipTo.Distinct().Contains(w)).ToList();
            List<string> listZoneInMaster = list_M_Zone.Where(w => listZone.Distinct().Contains(w)).ToList();
            List<string> listCountryInMaster = list_M_Country.Where(w => listCountry.Distinct().Contains(w)).ToList();
            List<string> listUserInMaster = list_M_User.Where(w => listUserName.Distinct().Contains(w)).ToList();
            string msgHeader = "";

            if (!listSoldInMaster.Count().Equals(listSoldTo.Distinct().ToList().Count())
                || !listShipInMaster.Count().Equals(listShipTo.Distinct().ToList().Count())
                || !listZoneInMaster.Count().Equals(listZone.Distinct().ToList().Count())
                || !listCountryInMaster.Count().Equals(listCountry.Distinct().ToList().Count())
                || !listUserInMaster.Count().Equals(listUserName.Distinct().ToList().Count())
                )
            {
                sbMsg = new StringBuilder();

                if (!listSoldInMaster.Count().Equals(listSoldTo.Distinct().ToList().Count()))
                {
                    var listSoldNotMatch = listSoldTo.Distinct().Where(w => !listSoldInMaster.Contains(w)).ToList();

                    if (listSoldNotMatch != null)
                    {
                        StringBuilder sbSoldNotMatch = new StringBuilder();

                        foreach (var itemSoldNotMatch in listSoldNotMatch)
                        {
                            sbSoldNotMatch.Append(itemSoldNotMatch + ",");
                        }

                        if (!String.IsNullOrEmpty(sbSoldNotMatch.ToString()))
                        {
                            //sbSoldNotMatch.Append("Sold-to : ");
                            sbMsg.AppendLine("   - Sold-to : " + sbSoldNotMatch.ToString().Substring(0, sbSoldNotMatch.ToString().Length - 1) + " ");
                        }
                    }
                }

                if (!listShipInMaster.Count().Equals(listShipTo.Distinct().ToList().Count()))
                {
                    var listShipNotMatch = listShipTo.Distinct().Where(w => !listShipInMaster.Contains(w)).ToList();

                    if (listShipNotMatch != null)
                    {
                        StringBuilder sbShipNotMatch = new StringBuilder();

                        foreach (var itemShipNotMatch in listShipNotMatch)
                        {
                            sbShipNotMatch.Append(itemShipNotMatch + ",");
                        }

                        if (!String.IsNullOrEmpty(sbShipNotMatch.ToString()))
                        {
                            // sbMsg.Append("Ship-to : ");
                            sbMsg.AppendLine("   - Ship-to : " + sbShipNotMatch.ToString().Substring(0, sbShipNotMatch.ToString().Length - 1) + " ");
                        }
                    }
                }

                if (!listZoneInMaster.Count().Equals(listZone.Distinct().ToList().Count()))
                {
                    var listZoneNotMatch = listZone.Distinct().Where(w => !listZoneInMaster.Contains(w)).ToList();

                    if (listZoneNotMatch != null)
                    {
                        StringBuilder sbZoneNotMatch = new StringBuilder();

                        foreach (var itemZoneNotMatch in listZoneNotMatch)
                        {
                            sbZoneNotMatch.Append(itemZoneNotMatch + ",");
                        }

                        if (!String.IsNullOrEmpty(sbZoneNotMatch.ToString()))
                        {
                            //sbMsg.Append("Zone : ");
                            sbMsg.AppendLine("   - Zone : " + sbZoneNotMatch.ToString().Substring(0, sbZoneNotMatch.ToString().Length - 1) + " ");
                        }
                    }
                }

                if (!listCountryInMaster.Count().Equals(listCountry.Distinct().ToList().Count()))
                {
                    var listCountryNotMatch = listCountry.Distinct().Where(w => !listCountryInMaster.Contains(w)).ToList();

                    if (listCountryNotMatch != null)
                    {
                        StringBuilder sbCountryNotMatch = new StringBuilder();

                        foreach (var itemCountryNotMatch in listCountryNotMatch)
                        {
                            if (!String.IsNullOrEmpty(itemCountryNotMatch))
                            {
                                sbCountryNotMatch.Append(itemCountryNotMatch + ",");
                            }
                        }

                        if (!String.IsNullOrEmpty(sbCountryNotMatch.ToString()))
                        {
                            // sbMsg.Append("Country : ");
                            sbMsg.AppendLine("   - Country : " + sbCountryNotMatch.ToString().Substring(0, sbCountryNotMatch.ToString().Length - 1) + " ");
                        }
                    }
                }

                if (!String.IsNullOrEmpty(sbMsg.ToString()))
                {

                    msgHeader = list_M_Msg.Where(w => w.MSG_CODE == "MSG_033").Select(s => s.MSG_DESCRIPTION).FirstOrDefault();


                    strValidateMsg = sbMsg.ToString();

                    if (!String.IsNullOrEmpty(msgHeader))
                    {
                        strValidateMsg = msgHeader + " \r\n " + sbMsg.ToString();
                    }

                    return strValidateMsg;
                }

                if (!listUserInMaster.Count().Equals(listUserName.Distinct().ToList().Count()))
                {
                    var listUserNameNotMatch = listUserName.Distinct().Where(w => !listUserInMaster.Contains(w)).ToList();

                    if (listUserNameNotMatch != null)
                    {
                        StringBuilder sbUserNameNotMatch = new StringBuilder();

                        foreach (var itemUserNAmeNotMatch in listUserNameNotMatch)
                        {
                            if (!String.IsNullOrEmpty(itemUserNAmeNotMatch))
                            {
                                sbUserNameNotMatch.Append(itemUserNAmeNotMatch + ",");
                            }
                        }

                        if (!String.IsNullOrEmpty(sbUserNameNotMatch.ToString()))
                        {
                            sbMsg.AppendLine("   - User Name : " + sbUserNameNotMatch.ToString().Substring(0, sbUserNameNotMatch.ToString().Length - 1) + " ");
                        }
                    }
                }

                msgHeader = "";
                msgHeader = list_M_Msg.Where(w => w.MSG_CODE == "MSG_032").Select(s => s.MSG_DESCRIPTION).FirstOrDefault();

                strValidateMsg = sbMsg.ToString();

                if (!String.IsNullOrEmpty(msgHeader))
                {
                    strValidateMsg = msgHeader + " \r\n " + sbMsg.ToString();
                }

                return strValidateMsg;
            }

            #endregion

            #region "Check duplicate items"

            List<string> listPICDuplicate = new List<string>();
            string itemDupTmp = "";
            StringBuilder sbItemDuplicate = new StringBuilder();

            foreach (var item in listPICData)
            {
                itemDupTmp = item.SOLD_TO_CODE
                                     + item.SHIP_TO_CODE
                                     + item.ZONE
                                     + item.COUNTRY
                                     + item.USER_DISPLAY_TXT;

                if (!listPICDuplicate.Contains(itemDupTmp))
                {
                    var cntItem = (from p in listPICData
                                   where p.SOLD_TO_CODE == item.SOLD_TO_CODE
                                   && p.SHIP_TO_CODE == item.SHIP_TO_CODE
                                   && p.ZONE == item.ZONE
                                   && p.COUNTRY == item.COUNTRY
                                   && p.USER_DISPLAY_TXT == item.USER_DISPLAY_TXT
                                   select p).ToList().Count();

                    if (cntItem > 1)
                    {
                        sbItemDuplicate.AppendLine("Sold-to:" + item.SOLD_TO_CODE + "   "
                                                + "Ship-to:" + item.SHIP_TO_CODE + "   "
                                                + "Zone:" + item.ZONE + "   "
                                                + "Country:" + item.COUNTRY + "   "
                                                + "User Name:" + item.USER_DISPLAY_TXT + "   "
                                                + "Duplicate:" + cntItem + " records");

                        listPICDuplicate.Add(item.SOLD_TO_CODE
                                         + item.SHIP_TO_CODE
                                         + item.ZONE
                                         + item.COUNTRY
                                         + item.USER_DISPLAY_TXT);

                    }
                }


            }

            if (!String.IsNullOrEmpty(sbItemDuplicate.ToString()))
            {
                sbMsg = new StringBuilder();
                sbMsg.AppendLine(list_M_Msg.Where(w => w.MSG_CODE == "MSG_031").Select(s => s.MSG_DESCRIPTION).FirstOrDefault() + " ");
                sbMsg.AppendLine("  " + sbItemDuplicate.ToString() + " ");

                strValidateMsg = sbMsg.ToString();

                return strValidateMsg;
            }


            if (action.Equals("ADD"))
            {
                sbItemDuplicate = new StringBuilder();
                var picExist = (from p in context.ART_M_PIC
                                select new ART_M_PIC_2
                                {
                                    SOLD_TO_CODE = p.SOLD_TO_CODE,
                                    SHIP_TO_CODE = p.SHIP_TO_CODE,
                                    ZONE = p.ZONE,
                                    USER_ID = p.USER_ID,
                                    COUNTRY = p.COUNTRY
                                }).ToList();

                var list_All_User = (from p in context.ART_M_USER
                                     select new ART_M_USER_2
                                     {
                                         USER_ID = p.USER_ID,
                                         USERNAME = p.USERNAME
                                     }).Distinct().ToList();

                foreach (var itemNew in listPICData)
                {
                    var userID = list_All_User.Where(w => w.USERNAME == itemNew.USER_DISPLAY_TXT).Select(s => s.USER_ID).FirstOrDefault();
                    int cntPIC = 0;

                    if (String.IsNullOrEmpty(itemNew.COUNTRY))
                    {
                        cntPIC = (from p in picExist
                                  where p.SOLD_TO_CODE == itemNew.SOLD_TO_CODE
                                  && p.SHIP_TO_CODE == itemNew.SHIP_TO_CODE
                                  && p.USER_ID == userID
                                  && p.ZONE == itemNew.ZONE
                                  && String.IsNullOrEmpty(p.COUNTRY)
                                  select p.PIC_ID).Count();
                    }
                    else
                    {
                        cntPIC = (from p in picExist
                                  where p.SOLD_TO_CODE == itemNew.SOLD_TO_CODE
                                  && p.SHIP_TO_CODE == itemNew.SHIP_TO_CODE
                                  && p.USER_ID == userID
                                  && p.ZONE == itemNew.ZONE
                                  && p.COUNTRY == itemNew.COUNTRY
                                  select p.PIC_ID).Count();
                    }

                    if (cntPIC > 0)
                    {
                        sbItemDuplicate.AppendLine("Sold-to:" + itemNew.SOLD_TO_CODE + "   "
                                                  + "Ship-to:" + itemNew.SHIP_TO_CODE + "   "
                                                  + "Zone:" + itemNew.ZONE + "   "
                                                  + "Country:" + itemNew.COUNTRY + "   "
                                                  + "User Name:" + itemNew.USER_DISPLAY_TXT + "   "
                                                  + "Duplicate:" + cntPIC + " records");
                    }
                }

                if (!String.IsNullOrEmpty(sbItemDuplicate.ToString()))
                {
                    sbMsg = new StringBuilder();
                    sbMsg.AppendLine(list_M_Msg.Where(w => w.MSG_CODE == "MSG_031").Select(s => s.MSG_DESCRIPTION).FirstOrDefault() + " ");
                    //sbMsg.Append(" in the database.");
                    sbMsg.AppendLine("  " + sbItemDuplicate.ToString() + " ");

                    strValidateMsg = sbMsg.ToString();

                    return strValidateMsg;
                }
            }

            #endregion


            return strValidateMsg;
        }


        public ViewDataUploadFilesResult UploadResult_PIC(String FileName, int fileSize, string msg)
        {
            String getType = System.Web.MimeMapping.GetMimeMapping(FileName);
            String getExtension = Path.GetExtension(FileName).Replace(".", "");
            var result = new ViewDataUploadFilesResult()
            {
                create_by_display_txt = "",
                create_date_display_txt = DateTime.Now,
                ID = 0,
                name = FileName,
                size = fileSize,
                type = getType,
                nodeid = 0,
                thumbnailUrl = "",
                canDelete = true,
                canDownload = true,
                canAddVersion = true,
                msg = msg
            };
            return result;
        }
        public void UploadAndShowResultsFileArtwork(int userId, int? requestFormId, HttpContextBase ContentBase, List<ViewDataUploadFilesResult> resultList)
        {
            var request = ContentBase.Request;
            ART_WF_ARTWORK_REQUEST_2 artwork2 = new ART_WF_ARTWORK_REQUEST_2();
            ART_WF_ARTWORK_REQUEST artwork = new ART_WF_ARTWORK_REQUEST();
            string configArtworkOther = "";
            string fileTypeNotAllowed = "";
            configArtworkOther = ConfigurationManager.AppSettings["ArtworkFolderName"];
            fileTypeNotAllowed = ConfigurationManager.AppSettings["FileTypeNotAllowed"];
            int artworkID = 0;

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        artwork.ARTWORK_REQUEST_ID = Convert.ToInt32(requestFormId);
                        artwork = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(Convert.ToInt32(requestFormId), context);
                        artworkID = artwork.ARTWORK_REQUEST_ID;

                        ART_WF_ARTWORK_REQUEST_ITEM itemRequest = new ART_WF_ARTWORK_REQUEST_ITEM();
                        //Upload Files
                        for (int i = 0; i < request.Files.Count; i++)
                        {
                            if (requestFormId != null && requestFormId > 0)
                            {
                                var file = request.Files[i];
                                long nodeID = 0;

                                if (artwork != null && artwork.REQUEST_FORM_FOLDER_NODE_ID != null && artwork.REQUEST_FORM_FOLDER_NODE_ID > 0)
                                {
                                    nodeID = Convert.ToInt64(artwork.REQUEST_FORM_FOLDER_NODE_ID);
                                }

                                var token = CWSService.getAuthToken();
                                Node nodeOthers = BLL.Services.CWSService.getNodeByName(nodeID, configArtworkOther, token);
                                if (nodeID > 0)
                                {
                                    if (nodeOthers != null)
                                    {
                                        Node node = BLL.Services.CWSService.uploadFile(file.InputStream, file.FileName, nodeOthers.ID, token);

                                        string extension = Path.GetExtension(file.FileName).Replace(".", "");
                                        string contentType = file.ContentType;

                                        if (fileTypeNotAllowed.Contains(extension.ToLower()))
                                        {
                                            itemRequest = new ART_WF_ARTWORK_REQUEST_ITEM();
                                            itemRequest.ARTWORK_REQUEST_ID = artworkID;
                                            itemRequest.CONTENT_TYPE = contentType;
                                            itemRequest.FILE_NAME = node.Name;
                                            itemRequest.FILE_SIZE = file.ContentLength;
                                            itemRequest.REQUEST_FORM_FILE_NODE_ID = node.ID;
                                            itemRequest.EXTENSION = extension;
                                            itemRequest.CREATE_BY = userId;
                                            itemRequest.UPDATE_BY = userId;

                                            ART_WF_ARTWORK_REQUEST_ITEM_2 itemRequest_2 = new ART_WF_ARTWORK_REQUEST_ITEM_2();
                                            itemRequest_2 = MapperServices.ART_WF_ARTWORK_REQUEST_ITEM(itemRequest);

                                            ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.SaveOrUpdate(itemRequest, context);
                                        }
                                    }

                                    ART_WF_ARTWORK_REQUEST_ITEM itemRequestTmp = new ART_WF_ARTWORK_REQUEST_ITEM();
                                    ART_WF_ARTWORK_REQUEST requestTmp = new ART_WF_ARTWORK_REQUEST();

                                    requestTmp.ARTWORK_REQUEST_ID = itemRequest.ARTWORK_REQUEST_ID;
                                    requestTmp = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(requestTmp, context).FirstOrDefault();

                                    var fileTmp = request.Files[i];
                                    Node nodeOthersTmp = BLL.Services.CWSService.getNodeByName(Convert.ToInt64(requestTmp.REQUEST_FORM_FOLDER_NODE_ID), configArtworkOther, token);
                                    resultList.Add(UploadResult_Artwork(fileTmp.FileName, fileTmp.ContentLength, itemRequest.REQUEST_FORM_FILE_NODE_ID, itemRequest, context));
                                }
                            }
                        }
                        dbContextTransaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void UploadAndShowResultsFileArtworkold(int userId, int? requestFormId, HttpContextBase ContentBase, List<ViewDataUploadFilesResult> resultList)
        {
            var request = ContentBase.Request;
            ART_WF_ARTWORK_REQUEST_2 artwork2 = new ART_WF_ARTWORK_REQUEST_2();
            ART_WF_ARTWORK_REQUEST artwork = new ART_WF_ARTWORK_REQUEST();
            string configArtworkOther = "";
            configArtworkOther = ConfigurationManager.AppSettings["ArtworkFolderName"];
            int artworkID = 0;

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        artwork.ARTWORK_REQUEST_ID = Convert.ToInt32(requestFormId);
                        artwork = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(Convert.ToInt32(requestFormId), context);
                        artworkID = artwork.ARTWORK_REQUEST_ID;

                        ART_WF_ARTWORK_REQUEST_ITEM itemRequest = new ART_WF_ARTWORK_REQUEST_ITEM();
                        //Upload Files
                        for (int i = 0; i < request.Files.Count; i++)
                        {
                            if (requestFormId != null && requestFormId > 0)
                            {
                                var file = request.Files[i];
                                long nodeID = 0;

                                if (artwork != null && artwork.REQUEST_FORM_FOLDER_NODE_ID != null && artwork.REQUEST_FORM_FOLDER_NODE_ID > 0)
                                {
                                    nodeID = Convert.ToInt64(artwork.REQUEST_FORM_FOLDER_NODE_ID);
                                }

                                var token = CWSService.getAuthToken();
                                Node nodeOthers = BLL.Services.CWSService.getNodeByName(nodeID, configArtworkOther, token);
                                if (nodeID > 0)
                                {
                                    if (nodeOthers != null)
                                    {
                                        Node node = BLL.Services.CWSService.uploadFile(file.InputStream, file.FileName, nodeOthers.ID, token);

                                        string extension = Path.GetExtension(file.FileName).Replace(".", "");
                                        string contentType = file.ContentType;

                                        itemRequest = new ART_WF_ARTWORK_REQUEST_ITEM();
                                        itemRequest.ARTWORK_REQUEST_ID = artworkID;
                                        itemRequest.CONTENT_TYPE = contentType;
                                        itemRequest.FILE_NAME = node.Name;
                                        itemRequest.FILE_SIZE = file.ContentLength;
                                        itemRequest.REQUEST_FORM_FILE_NODE_ID = node.ID;
                                        itemRequest.EXTENSION = extension;
                                        itemRequest.CREATE_BY = userId;
                                        itemRequest.UPDATE_BY = userId;

                                        ART_WF_ARTWORK_REQUEST_ITEM_2 itemRequest_2 = new ART_WF_ARTWORK_REQUEST_ITEM_2();
                                        itemRequest_2 = MapperServices.ART_WF_ARTWORK_REQUEST_ITEM(itemRequest);

                                        ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.SaveOrUpdate(itemRequest, context);
                                    }

                                    ART_WF_ARTWORK_REQUEST_ITEM itemRequestTmp = new ART_WF_ARTWORK_REQUEST_ITEM();
                                    ART_WF_ARTWORK_REQUEST requestTmp = new ART_WF_ARTWORK_REQUEST();

                                    requestTmp.ARTWORK_REQUEST_ID = itemRequest.ARTWORK_REQUEST_ID;
                                    requestTmp = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(requestTmp, context).FirstOrDefault();

                                    var fileTmp = request.Files[i];
                                    Node nodeOthersTmp = BLL.Services.CWSService.getNodeByName(Convert.ToInt64(requestTmp.REQUEST_FORM_FOLDER_NODE_ID), configArtworkOther, token);
                                    resultList.Add(UploadResult_Artwork(fileTmp.FileName, fileTmp.ContentLength, itemRequest.REQUEST_FORM_FILE_NODE_ID, itemRequest, context));
                                }
                            }
                        }
                        dbContextTransaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UploadAndShowResultsIGrid(int SapMaterialId, int userId, HttpContextBase ContentBase, List<ViewDataUploadFilesResult> resultList)

        {
            var request = ContentBase.Request;
            string fileTypeNotAllowed = "";
            ViewDataUploadFilesResult result = new ViewDataUploadFilesResult();
            fileTypeNotAllowed = ConfigurationManager.AppSettings["FileTypeNotAllowed"];

            for (int i = 0; i < request.Files.Count; i++)
            {

                var file = request.Files[i];
                var createdby = CNService.curruser();
                var id = CNService.uploadFileIGridSAPMaterial(file.FileName, file.ContentType, file.InputStream, SapMaterialId, createdby);


                result = new ViewDataUploadFilesResult();
                result.name = file.FileName;
                result.size = file.ContentLength;
                result.nodeid = id;
                result.NODE_ID_TXT = id.ToString();
                result.canDelete = true;
                result.canDownload = true;
                result.create_by_display_txt = createdby;
                result.thumbnailUrl = System.Configuration.ConfigurationManager.AppSettings["suburl"] + "/Content/Free-file-icons/32px/" + Path.GetExtension(file.FileName).Replace(".", "") + ".png";

                resultList.Add(result);
            }
        }


        public void UploadAndShowResultsArtwork(int artworkSubId, int userId, int? roldId, HttpContextBase ContentBase, List<ViewDataUploadFilesResult> resultList
                                                               , int? version, string is_internal, string is_customer, string is_vendor)
        {
            var request = ContentBase.Request;
            string fileTypeNotAllowed = "";
            fileTypeNotAllowed = ConfigurationManager.AppSettings["FileTypeNotAllowed"];

            for (int i = 0; i < request.Files.Count; i++)
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        ART_WF_ARTWORK_PROCESS process = new ART_WF_ARTWORK_PROCESS();
                        Node nodeOthers = new Node();
                        ART_WF_ARTWORK_PROCESS processParent = new ART_WF_ARTWORK_PROCESS();
                        ART_WF_ARTWORK_REQUEST_ITEM request_item = new ART_WF_ARTWORK_REQUEST_ITEM();
                        process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(artworkSubId, context);
                        request_item = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(process.ARTWORK_ITEM_ID, context);
                        var file = request.Files[i];

                        var folderName = ConfigurationManager.AppSettings["ArtworkFolderNameOther"];
                        var SEND_VN_PM = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_VN_PM" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                        if (SEND_VN_PM == process.CURRENT_STEP_ID)
                        {
                            folderName = ConfigurationManager.AppSettings["ArtworkFolderNamePrintMaster"];
                        }

                        var SEND_CUS_REQ_REF = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REQ_REF" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        if (SEND_CUS_REQ_REF == process.CURRENT_STEP_ID)
                        {
                            folderName = ConfigurationManager.AppSettings["ArtworkFolderNameReferenceLetter"];
                        }

                        var token = CWSService.getAuthToken();
                        if (process != null)
                        {
                            processParent = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = process.ARTWORK_ITEM_ID }, context)
                                .Where(m => m.PARENT_ARTWORK_SUB_ID == null).FirstOrDefault();

                            if (processParent.ARTWORK_FOLDER_NODE_ID != null)
                            {
                                nodeOthers = BLL.Services.CWSService.getNodeByName(Convert.ToInt64(processParent.ARTWORK_FOLDER_NODE_ID), folderName, token);
                            }
                            else
                            {
                                nodeOthers = BLL.Services.CWSService.getNodeByName(Convert.ToInt64(process.ARTWORK_FOLDER_NODE_ID), folderName, token);
                            }
                        }

                        if (nodeOthers != null)
                        {
                            string extension = Path.GetExtension(file.FileName).Replace(".", "");
                            if (fileTypeNotAllowed.Contains(extension.ToLower()))
                            {
                                Node node = BLL.Services.CWSService.uploadFile(file.InputStream, file.FileName, nodeOthers.ID, token);
                                // string extension = Path.GetExtension(file.FileName).Replace(".", "");
                                string contentType = file.ContentType;
                                var att = AttachmentArtworkHelper.SaveAttachment(node.Name, extension, contentType, process.ARTWORK_REQUEST_ID, artworkSubId, file.ContentLength, node.ID, userId, roldId
                                        , version, is_internal, is_customer, is_vendor, context);

                                resultList.Add(UploadResult_Artwork(file.FileName, file.ContentLength, node.ID, att, context));
                            }
                        }

                        dbContextTransaction.Commit();
                    }
                }
            }
        }
        public void UploadAndShowResultsArtworkold(int artworkSubId, int userId, int? roldId, HttpContextBase ContentBase, List<ViewDataUploadFilesResult> resultList
                                                , int? version, string is_internal, string is_customer, string is_vendor)
        {
            var request = ContentBase.Request;
            for (int i = 0; i < request.Files.Count; i++)
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        ART_WF_ARTWORK_PROCESS process = new ART_WF_ARTWORK_PROCESS();
                        Node nodeOthers = new Node();
                        ART_WF_ARTWORK_PROCESS processParent = new ART_WF_ARTWORK_PROCESS();
                        ART_WF_ARTWORK_REQUEST_ITEM request_item = new ART_WF_ARTWORK_REQUEST_ITEM();
                        process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(artworkSubId, context);
                        request_item = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(process.ARTWORK_ITEM_ID, context);
                        var file = request.Files[i];

                        var folderName = ConfigurationManager.AppSettings["ArtworkFolderNameOther"];
                        var SEND_VN_PM = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_VN_PM" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                        if (SEND_VN_PM == process.CURRENT_STEP_ID)
                        {
                            folderName = ConfigurationManager.AppSettings["ArtworkFolderNamePrintMaster"];
                        }

                        var SEND_CUS_REQ_REF = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REQ_REF" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        if (SEND_CUS_REQ_REF == process.CURRENT_STEP_ID)
                        {
                            folderName = ConfigurationManager.AppSettings["ArtworkFolderNameReferenceLetter"];
                        }

                        var token = CWSService.getAuthToken();
                        if (process != null)
                        {
                            processParent = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = process.ARTWORK_ITEM_ID }, context)
                                .Where(m => m.PARENT_ARTWORK_SUB_ID == null).FirstOrDefault();

                            if (processParent.ARTWORK_FOLDER_NODE_ID != null)
                            {
                                nodeOthers = BLL.Services.CWSService.getNodeByName(Convert.ToInt64(processParent.ARTWORK_FOLDER_NODE_ID), folderName, token);
                            }
                            else
                            {
                                nodeOthers = BLL.Services.CWSService.getNodeByName(Convert.ToInt64(process.ARTWORK_FOLDER_NODE_ID), folderName, token);
                            }
                        }

                        if (nodeOthers != null)
                        {
                            Node node = BLL.Services.CWSService.uploadFile(file.InputStream, file.FileName, nodeOthers.ID, token);
                            string extension = Path.GetExtension(file.FileName).Replace(".", "");
                            string contentType = file.ContentType;
                            var att = AttachmentArtworkHelper.SaveAttachment(node.Name, extension, contentType, process.ARTWORK_REQUEST_ID, artworkSubId, file.ContentLength, node.ID, userId, roldId
                                    , version, is_internal, is_customer, is_vendor, context);

                            resultList.Add(UploadResult_Artwork(file.FileName, file.ContentLength, node.ID, att, context));
                        }

                        dbContextTransaction.Commit();
                    }
                }
            }
        }

        public ViewDataUploadFilesResult UploadResult_Mockup(String FileName, int fileSize, long nodeId, ART_WF_MOCKUP_ATTACHMENT att, ARTWORKEntities context)
        {
            String getType = System.Web.MimeMapping.GetMimeMapping(FileName);
            String getExtension = Path.GetExtension(FileName).Replace(".", "");

            var att_last_version = ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetByItem(new ART_WF_MOCKUP_ATTACHMENT() { NODE_ID = att.NODE_ID }, context).OrderByDescending(m => m.CREATE_DATE).FirstOrDefault();

            var create_by_desc_txt = "";
            if (CNService.IsVendor(att.CREATE_BY, context))
            {
                var listUserVendor = ART_M_USER_VENDOR_SERVICE.GetByItem(new DAL.ART_M_USER_VENDOR() { USER_ID = Convert.ToInt32(att.CREATE_BY) }, context);
                if (listUserVendor.Count() > 0)
                {
                    create_by_desc_txt = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(listUserVendor.FirstOrDefault().VENDOR_ID, context).VENDOR_NAME;
                }
            }
            else if (CNService.IsCustomer(att.CREATE_BY, context))
            {
                var listUserCustomer = ART_M_USER_CUSTOMER_SERVICE.GetByItem(new DAL.ART_M_USER_CUSTOMER() { USER_ID = Convert.ToInt32(att.CREATE_BY) }, context);
                if (listUserCustomer.Count() > 0)
                {
                    create_by_desc_txt = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(listUserCustomer.FirstOrDefault().CUSTOMER_ID, context).CUSTOMER_NAME;
                }
            }

            var openStep = context.ART_M_STEP_MOCKUP.Where(w => w.STEP_MOCKUP_CODE == "UPLOAD_CHECK_LIST").FirstOrDefault();
            string stepName = "";

            if (att.ROLE_ID != null && att.ROLE_ID == openStep.STEP_MOCKUP_ID)
            {
                stepName = openStep.STEP_MOCKUP_NAME;
            }
            else
            {
                stepName = ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(att.STEP_MOCKUP_ID, context).STEP_MOCKUP_NAME;
            }

            var result = new ViewDataUploadFilesResult()
            {
                version2 = att_last_version.VERSION2,
                step = stepName,
                remark = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(att.MOCKUP_SUB_ID, context).REMARK,
                create_by_display_txt = CNService.GetUserName(att.CREATE_BY, context),
                create_date_display_txt = att.CREATE_DATE,
                version = att.VERSION,
                IS_INTERNAL = att.IS_INTERNAL,
                IS_CUSTOMER = att.IS_CUSTOMER,
                IS_VENDOR = att.IS_VENDOR,
                NODE_ID_TXT = EncryptionService.EncryptAndUrlEncode(nodeId.ToString()),
                name = FileName,
                size = fileSize,
                type = getType,
                nodeid = nodeId,
                thumbnailUrl = System.Configuration.ConfigurationManager.AppSettings["suburl"] + "/Content/Free-file-icons/32px/" + getExtension + ".png",
                canDelete = true,
                canDownload = true,
                canAddVersion = true,
                create_by_desc_txt = create_by_desc_txt,
            };
            return result;
        }

        public ViewDataUploadFilesResult UploadResult_Artwork(String FileName, int fileSize, long nodeId, ART_WF_ARTWORK_ATTACHMENT att, ARTWORKEntities context)
        {
            String getType = System.Web.MimeMapping.GetMimeMapping(FileName);
            String getExtension = Path.GetExtension(FileName).Replace(".", "");

            var create_by_desc_txt = "";
            if (CNService.IsVendor(att.CREATE_BY, context))
            {
                var listUserVendor = ART_M_USER_VENDOR_SERVICE.GetByItem(new DAL.ART_M_USER_VENDOR() { USER_ID = Convert.ToInt32(att.CREATE_BY) }, context);
                if (listUserVendor.Count() > 0)
                {
                    create_by_desc_txt = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(listUserVendor.FirstOrDefault().VENDOR_ID, context).VENDOR_NAME;
                }
            }
            else if (CNService.IsCustomer(att.CREATE_BY, context))
            {
                var listUserCustomer = ART_M_USER_CUSTOMER_SERVICE.GetByItem(new DAL.ART_M_USER_CUSTOMER() { USER_ID = Convert.ToInt32(att.CREATE_BY) }, context);
                if (listUserCustomer.Count() > 0)
                {
                    create_by_desc_txt = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(listUserCustomer.FirstOrDefault().CUSTOMER_ID, context).CUSTOMER_NAME;
                }
            }


            var result = new ViewDataUploadFilesResult()
            {
                version = att.VERSION,
                version2 = att.VERSION2,
                step = ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(att.STEP_ARTWORK_ID, context).STEP_ARTWORK_NAME,
                step_code = ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(att.STEP_ARTWORK_ID, context).STEP_ARTWORK_CODE,   // #INC-36800 by aof.
                create_by_display_txt = CNService.GetUserName(att.CREATE_BY, context),
                create_date_display_txt = att.CREATE_DATE,
                ID = att.ARTWORK_REQUEST_ID,
                name = FileName,
                size = fileSize,
                type = getType,
                nodeid = nodeId,
                thumbnailUrl = System.Configuration.ConfigurationManager.AppSettings["suburl"] + "/Content/Free-file-icons/32px/" + getExtension + ".png",
                canDelete = true,
                canDownload = true,
                canAddVersion = true,
                NODE_ID_TXT = EncryptionService.EncryptAndUrlEncode(nodeId.ToString()),
                IS_INTERNAL = att.IS_INTERNAL,
                IS_CUSTOMER = att.IS_CUSTOMER,
                IS_VENDOR = att.IS_VENDOR,
                IS_SYSTEM = att.IS_SYSTEM,

                create_by_desc_txt = create_by_desc_txt,
            };
            return result;
        }

        public ViewDataUploadFilesResult UploadResult_Artwork(String FileName, int fileSize, long nodeId, ART_WF_ARTWORK_REQUEST_ITEM att, ARTWORKEntities context)
        {
            String getType = System.Web.MimeMapping.GetMimeMapping(FileName);
            String getExtension = Path.GetExtension(FileName).Replace(".", "");
            var result = new ViewDataUploadFilesResult()
            {
                create_by_display_txt = CNService.GetUserName(att.CREATE_BY, context),
                create_date_display_txt = att.CREATE_DATE,
                ID = att.ARTWORK_REQUEST_ID,
                name = FileName,
                size = fileSize,
                type = getType,
                nodeid = nodeId,
                thumbnailUrl = System.Configuration.ConfigurationManager.AppSettings["suburl"] + "/Content/Free-file-icons/32px/" + getExtension + ".png",
                canDelete = true,
                canDownload = true,
                canAddVersion = true,
                NODE_ID_TXT = EncryptionService.EncryptAndUrlEncode(nodeId.ToString()),
            };
            return result;
        }

        public Stream DownloadResult(long nodeID)
        {
            var token = CWSService.getAuthToken();
            return CWSService.downloadFile(nodeID, token);
        }

        public Stream DownloadResult(long nodeID, long version)
        {
            var token = CWSService.getAuthToken();
            return CWSService.downloadFile(nodeID, version, token);
        }

        public int countPOFiles(string po)
        {
            string strPONodeID = ConfigurationManager.AppSettings["PONodeID"];
            string strPOFolder = ConfigurationManager.AppSettings["ArtworkFolderNamePO"];
            var token = CWSService.getAuthToken();
            Node nodeParent = CWSService.getNodeByName(Convert.ToInt64(strPONodeID), po, token);
            if (nodeParent != null)
            {
                //Folder workspace * (-1)
                Node node = CWSService.getNodeByName(nodeParent.ID * (-1), strPOFolder, token);
                if (node != null)
                {
                    Node[] nodeFile = CWSService.getAllNodeInFolder(node.ID, token);

                    return nodeFile.Count();
                }
            }

            return 0;
        }


        public Stream DownloadPO(string po)
        {
            string strPONodeID = ConfigurationManager.AppSettings["PONodeID"];
            string strPOFolder = ConfigurationManager.AppSettings["ArtworkFolderNamePO"];
            var token = CWSService.getAuthToken();
            Node nodeParent = CWSService.getNodeByName(Convert.ToInt64(strPONodeID), po, token);
            if (nodeParent != null)
            {
                //Folder workspace * (-1)
                Node node = CWSService.getNodeByName(nodeParent.ID * (-1), strPOFolder, token);
                if (node != null)
                {
                    Node[] nodeFile = CWSService.getAllNodeInFolder(node.ID, token);
                    if (nodeFile != null)
                    {
                        Stream downloadStream = null;
                        foreach (var file in nodeFile)
                        {
                            downloadStream = CWSService.downloadFile(file.ID, token);
                        }
                        return downloadStream;
                    }
                }
            }

            return null;
        }

        public byte[] DownloadPOZip(string po, ref int cntFiles)
        {
            byte[] compressedBytes;
            byte[] ReadAllbytes = new byte[0];//Capcity buffer
            string strPONodeID = ConfigurationManager.AppSettings["PONodeID"];
            string strPOFolder = ConfigurationManager.AppSettings["ArtworkFolderNamePO"];
            var token = CWSService.getAuthToken();
            Node nodeParent = CWSService.getNodeByName(Convert.ToInt64(strPONodeID), po, token);
            if (nodeParent != null)
            {
                //Folder workspace * (-1)
                Node node = CWSService.getNodeByName(nodeParent.ID * (-1), strPOFolder, token);
                if (node != null)
                {
                    Node[] nodeFile = CWSService.getAllNodeInFolder(node.ID, token);

                    cntFiles = nodeFile.Count();

                    if (nodeFile != null)
                    {
                        using (var outStream = new MemoryStream())
                        {
                            using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                            {
                                foreach (var file in nodeFile)
                                {
                                    Stream downloadStream = null;
                                    downloadStream = CWSService.downloadFile(file.ID, token);

                                    var fileInArchive = archive.CreateEntry(file.Name, CompressionLevel.Optimal);
                                    using (var entryStream = fileInArchive.Open())
                                        downloadStream.CopyTo(entryStream);
                                }
                            }
                            compressedBytes = outStream.ToArray();
                        }
                        return compressedBytes;
                    }
                }
            }

            return ReadAllbytes;
        }

        public byte[] DownloadPOByAWZip(string artworkNo)
        {
            //byte[] compressedBytes;
            byte[] ReadAllbytes = new byte[0];//Capcity buffer
            string strPONodeID = ConfigurationManager.AppSettings["PONodeID"];
            string strPOFolder = ConfigurationManager.AppSettings["ArtworkFolderNamePO"];
            var token = CWSService.getAuthToken();
            List<string> listAW = new List<string>();
            List<ART_WF_ARTWORK_MAPPING_PO_2> listPO = new List<ART_WF_ARTWORK_MAPPING_PO_2>();
            var aw_number = EncryptionService.Decrypt(artworkNo);

            listAW = aw_number.Split(new string[] { "||" }, StringSplitOptions.None).ToList();
            var cntFile = 0;

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    listPO = (from p in context.ART_WF_ARTWORK_MAPPING_PO
                              where listAW.Contains(p.ARTWORK_NO)
                              //&& p.IS_ACTIVE == "X"   byaof
                              select new ART_WF_ARTWORK_MAPPING_PO_2
                              {
                                  PO_NO = p.PO_NO,
                                  ARTWORK_NO = p.ARTWORK_NO,
                                  IS_ACTIVE = p.IS_ACTIVE
                              }).Distinct().ToList();
                }
            }

            using (var outStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    foreach (var itemAW in listAW)
                    {
                        if (!String.IsNullOrEmpty(itemAW))
                        {
                            var listPO2 = (from p in listPO
                                           where p.ARTWORK_NO == itemAW
                                           //&& p.IS_ACTIVE == "X"     byaof
                                           select p.PO_NO).Distinct().ToList();

                            if (listPO2 != null)
                            {
                                foreach (var po in listPO2)
                                {

                                    Node nodeParent = CWSService.getNodeByName(Convert.ToInt64(strPONodeID), po, token);
                                    if (nodeParent != null)
                                    {
                                        //Folder workspace * (-1)
                                        Node node = CWSService.getNodeByName(nodeParent.ID * (-1), strPOFolder, token);
                                        if (node != null)
                                        {
                                            Node[] nodeFile = CWSService.getAllNodeInFolder(node.ID, token);

                                            if (nodeFile != null)
                                            {

                                                foreach (var file in nodeFile)
                                                {
                                                    cntFile++;
                                                    Stream downloadStream = null;
                                                    downloadStream = CWSService.downloadFile(file.ID, token);

                                                    var fileInArchive = archive.CreateEntry(itemAW + "\\" + po + "\\" + file.Name, CompressionLevel.Optimal);
                                                    using (var entryStream = fileInArchive.Open())
                                                    {
                                                        downloadStream.CopyTo(entryStream);
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

                if (cntFile > 0)
                {
                    return outStream.ToArray(); ;
                }
            }

            return ReadAllbytes;
        }
        public byte[] DownloadAW(string aw_list)
        {
            byte[] ReadAllbytes = new byte[0];//Capcity buffer
            Dictionary<string, Stream> dicFile = new Dictionary<string, Stream>();
            var parentSecondaryPackagingID = ConfigurationManager.AppSettings["SecondaryPackagingNodeID"];
            var folderSPAW = ConfigurationManager.AppSettings["SecondaryPkgArtworkFolderName"];

            List<string> listAW = new List<string>();

            listAW = aw_list.Split(new string[] { "||" }, StringSplitOptions.None).ToList();

            if (listAW.Count > 0)
            {
                var cntFile = 0;
                var token = CWSService.getAuthToken();
                using (var outStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var item in listAW)
                        {
                            var aw = item.Trim(); // decoding(item);
                            if (aw.StartsWith("M"))
                            {
                                Node[] nodesMO = DownloadMO(aw);
                                if (nodesMO != null && nodesMO.Count() > 0)
                                {
                                    foreach (var file in nodesMO)
                                    {
                                        cntFile++;
                                        Stream downloadStream = CWSService.downloadFile(file.ID, token);
                                        var fileInArchive = archive.CreateEntry(file.Name, CompressionLevel.Optimal);
                                        using (var entryStream = fileInArchive.Open())
                                            downloadStream.CopyTo(entryStream);
                                    }
                                }
                            }
                            else
                            {
                                string matDesc = "";
                                using (var context = new ARTWORKEntities())
                                {
                                    using (CNService.IsolationLevel(context))
                                    {
                                        var itemID = context.ART_WF_ARTWORK_REQUEST_ITEM
                                           .Where(i => i.REQUEST_ITEM_NO == aw)
                                           .Select(s => s.ARTWORK_ITEM_ID).FirstOrDefault();

                                        var stepPA = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_PA").FirstOrDefault();

                                        var process = (from p in context.ART_WF_ARTWORK_PROCESS
                                                       where p.ARTWORK_ITEM_ID == itemID
                                                        && p.CURRENT_STEP_ID == stepPA.STEP_ARTWORK_ID
                                                       select p).OrderByDescending(o => o.ARTWORK_SUB_ID).FirstOrDefault();

                                        var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                         where p.ARTWORK_SUB_ID == process.ARTWORK_SUB_ID
                                                         select p).FirstOrDefault();

                                        if (processPA != null && !String.IsNullOrEmpty(processPA.MATERIAL_NO))
                                        {
                                            var xProduct = (from p in context.XECM_M_PRODUCT5
                                                            where p.PRODUCT_CODE == processPA.MATERIAL_NO
                                                            select p).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                                            matDesc = processPA.MATERIAL_NO;

                                            if (xProduct != null)
                                            {
                                                matDesc = processPA.MATERIAL_NO + " - " + xProduct.PRODUCT_DESCRIPTION;
                                            }
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(matDesc))
                                {
                                    Node nodeParentSPAW = CWSService.getNodeByName(Convert.ToInt64(parentSecondaryPackagingID), matDesc, token);
                                    if (nodeParentSPAW != null)
                                    {
                                        Node node = CWSService.getNodeByName(nodeParentSPAW.ID * (-1), folderSPAW, token);
                                        if (node != null)
                                        {
                                            Node[] nodeSPAWFiles = CWSService.getAllNodeInFolder(node.ID, token);
                                            if (nodeSPAWFiles != null && nodeSPAWFiles.Count() > 0)
                                            {
                                                foreach (var file in nodeSPAWFiles)
                                                {
                                                    cntFile++;
                                                    Stream downloadStream = CWSService.downloadFile(file.ID, token);
                                                    var fileInArchive = archive.CreateEntry(file.Name, CompressionLevel.Optimal);
                                                    using (var entryStream = fileInArchive.Open())
                                                        downloadStream.CopyTo(entryStream);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (cntFile > 0) return outStream.ToArray();
                }
            }

            return ReadAllbytes;
        }

        public byte[] DownloadAWMaterialLockReport(string mat_list)
        {
            byte[] ReadAllbytes = new byte[0];//Capcity buffer
            Dictionary<string, Stream> dicFile = new Dictionary<string, Stream>();
            var parentSecondaryPackagingID = ConfigurationManager.AppSettings["SecondaryPackagingNodeID"];
            var folderSPAW = ConfigurationManager.AppSettings["SecondaryPkgArtworkFolderName"];
            List<string> listMaterial = new List<string>();
            var tempList = new List<ART_WF_ARTWORK_MATERIAL_LOCK_2>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    listMaterial = mat_list.Split(new string[] { "||" }, StringSplitOptions.None).ToList();

                    //tempList = (from m in context.ART_WF_ARTWORK_MATERIAL_LOCK
                    //            where listMaterial.Contains(m.MATERIAL_NO)
                    //            select new ART_WF_ARTWORK_MATERIAL_LOCK_2 { MATERIAL_NO = m.MATERIAL_NO, MATERIAL_DESCRIPTION = m.MATERIAL_DESCRIPTION }).ToList();

                    tempList = (from m in context.ART_WF_ARTWORK_MATERIAL_LOCK
                                join n in context.XECM_M_PRODUCT5 on m.MATERIAL_NO equals n.PRODUCT_CODE into t
                                from x in t.DefaultIfEmpty()
                                where listMaterial.Contains(m.MATERIAL_NO)
                                select new ART_WF_ARTWORK_MATERIAL_LOCK_2 { MATERIAL_NO = m.MATERIAL_NO, MATERIAL_DESCRIPTION = x.PRODUCT_DESCRIPTION }).ToList();
                

                }
            }

            var cntFile = 0;
            if (tempList.Count > 0)
            {
                var token = CWSService.getAuthToken();
                using (var outStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var item in tempList)
                        {
                            Node nodeParentSPAW = CWSService.getNodeByName(Convert.ToInt64(parentSecondaryPackagingID), item.MATERIAL_NO + " - " + item.MATERIAL_DESCRIPTION, token);
                            if (nodeParentSPAW != null)
                            {
                                Node node = CWSService.getNodeByName(nodeParentSPAW.ID * (-1), folderSPAW, token);
                                if (node != null)
                                {
                                    Node[] nodeSPAWFiles = CWSService.getAllNodeInFolder(node.ID, token);
                                    if (nodeSPAWFiles != null && nodeSPAWFiles.Count() > 0)
                                    {
                                        foreach (var file in nodeSPAWFiles)
                                        {
                                            cntFile++;
                                            Stream downloadStream = CWSService.downloadFile(file.ID, token);
                                            var fileInArchive = archive.CreateEntry("Final Artwork" + "\\" + item.MATERIAL_NO + "\\" + file.Name, CompressionLevel.Optimal);
                                            using (var entryStream = fileInArchive.Open())
                                                downloadStream.CopyTo(entryStream);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (cntFile > 0) return outStream.ToArray();
                }
            }
            return ReadAllbytes;
        }

        private Node[] DownloadMO(string mo)
        {
            Node[] nodeSPMOFiles_Empty = new Node[0];

            var parentMOFolder = ConfigurationManager.AppSettings["MockUpNodeID"];
            var folderDieline = ConfigurationManager.AppSettings["MockupFolderNameDieline"];
            var token = CWSService.getAuthToken();
            Node nodeParentSPMO = CWSService.getNodeByName(Convert.ToInt64(parentMOFolder), mo, token);

            if (nodeParentSPMO != null)
            {
                Node nodeMO = CWSService.getNodeByName(nodeParentSPMO.ID, folderDieline, token);

                if (nodeMO != null)
                {
                    Node[] nodeSPMOFiles = CWSService.getAllNodeInFolder(nodeMO.ID, token);
                    return nodeSPMOFiles;
                }
            }
            return null;
        }

        public string encoding(string toEncode)
        {
            string toReturn = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(toEncode));
            return toReturn;
        }

        public string decoding(string toDecode)
        {
            string toReturn = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(toDecode));
            return toReturn;
        }

        public byte[] DownloadArtworkVendor(string po, ref bool onefile, ref string filename)
        {
            string strArtworkNodeID = ConfigurationManager.AppSettings["ArtworkNodeID"];
            string strPrintMasterFolder = ConfigurationManager.AppSettings["ArtworkFolderNamePrintMaster"];
            Dictionary<string, Stream> dicFile = new Dictionary<string, Stream>();
            var userId = 0;
            var tempMapppingPONew = new List<string>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    userId = CNService.getCurrentUser(context);
                    var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                    var tempMapppingPO = ART_WF_ARTWORK_MAPPING_PO_SERVICE.GetByItem(new ART_WF_ARTWORK_MAPPING_PO() { PO_NO = po, IS_ACTIVE = "X" }, context);

                    var temp = tempMapppingPO.Select(s => s.ARTWORK_NO).Distinct().ToList();
                    foreach (var item in temp)
                    {
                        var ARTWORK_ITEM_ID = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_ITEM() { REQUEST_ITEM_NO = item }, context).FirstOrDefault().ARTWORK_ITEM_ID;
                        var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = ARTWORK_ITEM_ID, CURRENT_STEP_ID = SEND_PA }, context).FirstOrDefault();
                        if (string.IsNullOrEmpty(process.IS_END))
                        {
                            tempMapppingPONew.Add(item);
                        }
                    }
                }
            }

            var hasOneFile = true;
            var cntFile = 0;
            var token = CWSService.getAuthToken();
            foreach (var item in tempMapppingPONew)
            {
                Node nodeParent = CWSService.getNodeByName(Convert.ToInt64(strArtworkNodeID), item, token);
                if (nodeParent != null)
                {
                    Node nodePrintMaster = CWSService.getNodeByName(nodeParent.ID, strPrintMasterFolder, token);
                    if (nodePrintMaster != null)
                    {
                        Node[] nodeFilePrintMaster = CWSService.getAllNodeInFolder(nodePrintMaster.ID, token);
                        if (nodeFilePrintMaster != null)
                        {
                            cntFile += nodeFilePrintMaster.Length;
                            if (cntFile > 1)
                            {
                                hasOneFile = false;
                                break;
                            }
                        }
                    }
                }
            }

            if (cntFile == 0)
            {
                byte[] ReadAllbytes = new byte[0];//Capcity buffer
                return ReadAllbytes;
            }
            else
            {
                if (hasOneFile)
                {
                    using (var outStream = new MemoryStream())
                    {
                        foreach (var item in tempMapppingPONew)
                        {
                            Node nodeParent = CWSService.getNodeByName(Convert.ToInt64(strArtworkNodeID), item, token);
                            if (nodeParent != null)
                            {
                                Node nodePrintMaster = CWSService.getNodeByName(nodeParent.ID, strPrintMasterFolder, token);
                                if (nodePrintMaster != null)
                                {
                                    Node[] nodeFilePrintMaster = CWSService.getAllNodeInFolder(nodePrintMaster.ID, token);
                                    if (nodeFilePrintMaster != null)
                                    {
                                        if (hasOneFile)
                                        {
                                            onefile = true;
                                            foreach (var file in nodeFilePrintMaster)
                                            {
                                                Stream downloadStream = CWSService.downloadFile(file.ID, token);
                                                string path = item + "\\" + file.Name;
                                                filename = file.Name;
                                                dicFile.Add(path, downloadStream);
                                            }

                                            foreach (var file in dicFile)
                                            {
                                                Stream downloadStream = file.Value;
                                                downloadStream.CopyTo(outStream);
                                            }
                                            return outStream.ToArray();
                                        }
                                    }
                                }
                            }
                        }
                        return outStream.ToArray();
                    }
                }
                else
                {
                    using (var outStream = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                        {
                            foreach (var item in tempMapppingPONew)
                            {
                                Node nodeParent = CWSService.getNodeByName(Convert.ToInt64(strArtworkNodeID), item, token);
                                if (nodeParent != null)
                                {
                                    Node nodePrintMaster = CWSService.getNodeByName(nodeParent.ID, strPrintMasterFolder, token);
                                    if (nodePrintMaster != null)
                                    {
                                        Node[] nodeFilePrintMaster = CWSService.getAllNodeInFolder(nodePrintMaster.ID, token);
                                        if (nodeFilePrintMaster != null)
                                        {
                                            foreach (var file in nodeFilePrintMaster)
                                            {
                                                string path = item + "\\" + file.Name;
                                                Stream downloadStream = CWSService.downloadFile(file.ID, token);
                                                var fileInArchive = archive.CreateEntry(path, CompressionLevel.Optimal);
                                                using (var entryStream = fileInArchive.Open())
                                                    downloadStream.CopyTo(entryStream);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return outStream.ToArray();
                    }
                }
            }
        }

        public byte[] DownloadArtworkPrintMaster(string wf_no)
        {
            string strArtworkNodeID = ConfigurationManager.AppSettings["ArtworkNodeID"];
            string strPrintMasterFolder = ConfigurationManager.AppSettings["ArtworkFolderNamePrintMaster"];
            Dictionary<string, Stream> dicFile = new Dictionary<string, Stream>();

            var cntFile = 0;
            var token = CWSService.getAuthToken();

            Node nodeParent = CWSService.getNodeByName(Convert.ToInt64(strArtworkNodeID), wf_no, token);
            if (nodeParent != null)
            {
                Node nodePrintMaster = CWSService.getNodeByName(nodeParent.ID, strPrintMasterFolder, token);
                if (nodePrintMaster != null)
                {
                    Node[] nodeFilePrintMaster = CWSService.getAllNodeInFolder(nodePrintMaster.ID, token);
                    if (nodeFilePrintMaster != null)
                    {
                        cntFile += nodeFilePrintMaster.Length;
                    }
                }
            }

            if (cntFile == 0)
            {
                byte[] ReadAllbytes = new byte[0];//Capcity buffer
                return ReadAllbytes;
            }
            else
            {
                using (var outStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                    {
                        nodeParent = CWSService.getNodeByName(Convert.ToInt64(strArtworkNodeID), wf_no, token);
                        if (nodeParent != null)
                        {
                            Node nodePrintMaster = CWSService.getNodeByName(nodeParent.ID, strPrintMasterFolder, token);
                            if (nodePrintMaster != null)
                            {
                                Node[] nodeFilePrintMaster = CWSService.getAllNodeInFolder(nodePrintMaster.ID, token);
                                if (nodeFilePrintMaster != null)
                                {
                                    foreach (var file in nodeFilePrintMaster)
                                    {
                                        string path = wf_no + "\\" + file.Name;
                                        Stream downloadStream = CWSService.downloadFile(file.ID, token);
                                        var fileInArchive = archive.CreateEntry(path, CompressionLevel.Optimal);
                                        using (var entryStream = fileInArchive.Open())
                                            downloadStream.CopyTo(entryStream);
                                    }
                                }
                            }
                        }
                    }
                    return outStream.ToArray();
                }
            }
        }
    }

    public class ViewDataUploadFilesResult
    {
        public string NODE_ID_TXT { get; set; }
        public int ID { get; set; }
        public string name { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public long nodeid { get; set; }
        public string thumbnailUrl { get; set; }
        public bool canDelete { get; set; }
        public bool canDownload { get; set; }
        public bool canAddVersion { get; set; }
        public Nullable<long> version { get; set; }
        public string version2 { get; set; }
        public string remark { get; set; }
        public string create_by_display_txt { get; set; }
        public DateTime create_date_display_txt { get; set; }
        public string step { get; set; }
        public string step_code { get; set; }  // #INC-36800 by aof.
        public string create_by_desc_txt { get; set; }
        public string IS_INTERNAL { get; set; }
        public string IS_VENDOR { get; set; }
        public string IS_CUSTOMER { get; set; }
        public string IS_SYSTEM { get; set; }
        public string msg { get; set; }
        public string error { get; set; }
    }
    public class JsonFiles
    {
        public ViewDataUploadFilesResult[] files;
        public string TempFolder { get; set; }
        public JsonFiles(List<ViewDataUploadFilesResult> filesList)
        {
            files = new ViewDataUploadFilesResult[filesList.Count];
            for (int i = 0; i < filesList.Count; i++)
            {
                files[i] = filesList.ElementAt(i);
            }

        }
    }
}