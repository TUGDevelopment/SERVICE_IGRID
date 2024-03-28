using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using DAL;
using System.Web.Script.Serialization;
using System;
using System.Net.Mail;
using System.Text;
using System.Configuration;
using System.Threading;
using BLL.Helpers;
using DAL.Model;

namespace BLL.Services
{
    public class EmailService
    {
        private static void Send(int mockupSubId, int artworkSubId, int artworkRequestId, string templateCode, string Subject, string From, string To, string Body, string CC)
        {
            if (!string.IsNullOrEmpty(To))
            {
                string msg_error = "";
                string status = "1";
                //To = "voravut.somboornpong@thaiunion.com";
                //CC = "pornnicha.thanarak@thaiunion.com";
                Thread email = new Thread(delegate ()
                {
                    try
                    {
                        Subject = Subject.Replace("\n", " ");
                        MailMessage msg = new MailMessage(From, To, Subject, Body);
                        msg.SubjectEncoding = Encoding.UTF8;
                        msg.BodyEncoding = Encoding.UTF8;
                        msg.IsBodyHtml = true;

                        if (!string.IsNullOrEmpty(CC))
                        {
                            string[] email_cc = CC.Split(',');
                            foreach (string s in email_cc)
                                msg.CC.Add(new MailAddress(s));
                        }
                        //msg.Bcc.Add("voravut.somboornpong@thaiunion.com");
                        //msg.Bcc.Add("pornnicha.thanarak@thaiunion.com");
                        int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                        SmtpClient sc = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"], Port);
                        sc.UseDefaultCredentials = false;

                        bool IsUseSSL = false;
                        var SMTPSSL = ConfigurationManager.AppSettings["SMTPSSL"];
                        if (SMTPSSL.ToUpper().Trim() == "TRUE")
                        {
                            IsUseSSL = true;
                        }
                        sc.EnableSsl = IsUseSSL;
                        sc.Send(msg);
                    }
                    catch (Exception ex)
                    {
                        msg_error = CNService.GetErrorMessage(ex);
                        status = "0";
                    }
                    finally
                    {
                        if (mockupSubId > 0) saveLogSendEmailMockup(mockupSubId, templateCode, Subject, From, To, Body, CC, status, msg_error);
                        if (artworkSubId > 0) saveLogSendEmailArtwork(artworkSubId, templateCode, Subject, From, To, Body, CC, status, msg_error);
                        if (artworkRequestId > 0) saveLogSendEmailArtworkRequest(artworkRequestId, templateCode, Subject, From, To, Body, CC, status, msg_error);
                    }
                });

                email.IsBackground = true;
                email.Start();

                if (templateCode.Contains("WF_OVERDUE"))
                    email.Join();
            }
        }

        public static void sendEmailMockup(int mockupId, int mockupSubId, string templateCode, ARTWORKEntities context, string forReason = "")
        {
            try
            {
                var allStepMockup = ART_M_STEP_MOCKUP_SERVICE.GetAll(context);
                var process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(mockupSubId, context);
                if (templateCode == "WF_SEND_TO")
                {
                    if (process.IS_DELEGATE == "X")
                    {
                        templateCode = "WF_DELEGATE";
                        var log = ART_WF_LOG_DELEGATE_SERVICE.GetByItem(new ART_WF_LOG_DELEGATE() { WF_TYPE = "M", WF_SUB_ID = mockupSubId }, context).OrderByDescending(m => m.ART_WF_LOG_DELEGATE_ID).FirstOrDefault();
                        if (log != null)
                        {
                            forReason = log.REMARK;
                        }
                    }
                }

                var template = ART_M_EMAIL_TEMPLATE_SERVICE.GetByItem(new ART_M_EMAIL_TEMPLATE() { EMAIL_TEMPLATE_CODE = templateCode }, context).FirstOrDefault();
                if (template != null)
                {
                    string from = ConfigurationManager.AppSettings["SMTPFrom"];
                    string to = "";

                    var checkListId = CNService.ConvertMockupIdToCheckListId(mockupId, context);
                    var checkList = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(checkListId, context);
                    string cc = "";
                    if (checkList.REVIEWER > 0)
                    {
                        var email = CNService.GetEmailUserActive(checkList.REVIEWER, context);
                        if (!string.IsNullOrEmpty(email))
                        {
                            cc = email;
                        }
                    }

                    if (process.CURRENT_USER_ID > 0)
                    {
                        var email = CNService.GetEmailUserActive(process.CURRENT_USER_ID, context);
                        if (!string.IsNullOrEmpty(email))
                        {
                            to = email;
                        }
                    }
                    else if (process.CURRENT_ROLE_ID > 0)
                    {
                        var userRole = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { ROLE_ID = Convert.ToInt32(process.CURRENT_ROLE_ID) }, context);
                        foreach (var item in userRole)
                        {
                            var listUserCompany = ART_M_USER_COMPANY_SERVICE.GetByItem(new ART_M_USER_COMPANY() { USER_ID = Convert.ToInt32(item.USER_ID) }, context);
                            var listUserTypeofProduct = ART_M_USER_TYPE_OF_PRODUCT_SERVICE.GetByItem(new ART_M_USER_TYPE_OF_PRODUCT() { USER_ID = Convert.ToInt32(item.USER_ID) }, context);

                            var valid = CNService.CheckTypeOfProductAndCompanyMockup(item.USER_ID, checkListId, process.MOCKUP_SUB_ID, context, allStepMockup, listUserCompany, listUserTypeofProduct);
                            if (valid)
                            {
                                var email = CNService.GetEmailUserActive(item.USER_ID, context);
                                if (!string.IsNullOrEmpty(email))
                                {
                                    if (to == "") to = email;
                                    else to += "," + email;
                                }
                            }
                        }
                    }

                    if (templateCode == "WF_OVERDUE1" || templateCode == "WF_OVERDUE2")
                    {
                        if (process.CURRENT_USER_ID > 0)
                        {
                            var leader = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { USER_ID = Convert.ToInt32(process.CURRENT_USER_ID) }, context);
                            foreach (var item in leader)
                            {
                                var email = CNService.GetEmailUserActive(item.UPPER_USER_ID, context);
                                if (!string.IsNullOrEmpty(email))
                                {
                                    if (to == "") to = email;
                                    else to += "," + email;
                                }
                            }
                        }
                    }
                    else if (templateCode == "WF_OVERDUE3")
                    {
                        if (process.CURRENT_USER_ID > 0)
                        {
                            var leader = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { USER_ID = Convert.ToInt32(process.CURRENT_USER_ID) }, context);
                            foreach (var item in leader)
                            {
                                var email = CNService.GetEmailUserActive(item.UPPER_USER_ID, context);
                                if (!string.IsNullOrEmpty(email))
                                {
                                    if (to == "") to = email;
                                    else to += "," + email;
                                }
                                var leader2 = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { USER_ID = Convert.ToInt32(item.UPPER_USER_ID) }, context);
                                foreach (var item2 in leader2)
                                {
                                    var email2 = CNService.GetEmailUserActive(item2.UPPER_USER_ID, context);
                                    if (!string.IsNullOrEmpty(email2))
                                    {
                                        if (to == "") to = email2;
                                        else to += "," + email2;
                                    }
                                }
                            }
                        }
                    }

                    if (templateCode == "WF_TEMINATED")
                    {
                        var PGStep = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        if (PGStep == process.CURRENT_STEP_ID)
                        {
                            var email = CNService.GetEmailUserActive(checkList.CREATOR_ID, context);
                            if (!string.IsNullOrEmpty(email))
                            {
                                to = email;
                            }
                        }
                    }

                    if (templateCode == "WF_OTHER_SAVE" || templateCode == "WF_OTHER_SUBMIT")
                    {
                        var mainProcess = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(process.PARENT_MOCKUP_SUB_ID, context);
                        var email = CNService.GetEmailUserActive(mainProcess.CURRENT_USER_ID, context);
                        if (!string.IsNullOrEmpty(email))
                        {
                            to = email;
                        }
                    }

                    if (templateCode == "WF_SEND_BACK")
                    {
                        var SEND_BACK_MK = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_BACK_MK" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        if (SEND_BACK_MK != process.CURRENT_STEP_ID)
                        {
                            var mainProcess = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(process.PARENT_MOCKUP_SUB_ID, context);
                            var email = CNService.GetEmailUserActive(mainProcess.CURRENT_USER_ID, context);
                            if (!string.IsNullOrEmpty(email))
                            {
                                to = email;
                            }
                        }
                    }

                    string subject = template.M_SUBJECT;
                    string body = template.M_DEAR + "<br/><br/>";
                    body += template.M_BODY_01;

                    subject = replaceValueMockup(subject, mockupId, mockupSubId, context, templateCode, forReason);
                    body = replaceValueMockup(body, mockupId, mockupSubId, context, templateCode, forReason);

                    EmailService.Send(mockupSubId, 0, 0, templateCode, subject, from, to, body, cc);
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }
        private static string replaceValueMockup(string value, int mockupId, int mockupSubId, ARTWORKEntities context, string templateCode, string forReason)
        {
            var checklist = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(CNService.ConvertMockupIdToCheckListId(mockupId, context), context);
            var checklistItem = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(mockupId, context);
            var process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(mockupSubId, context);

            string dear_name = "";
            if (process.CURRENT_USER_ID != null)
                dear_name = CNService.GetUserName(process.CURRENT_USER_ID, context);
            else
                dear_name = ART_M_ROLE_SERVICE.GetByROLE_ID(process.CURRENT_ROLE_ID, context).DESCRIPTION;

            var brandName = "";
            if (checklist.BRAND_ID > 0)
                brandName = SAP_M_BRAND_SERVICE.GetByBRAND_ID(checklist.BRAND_ID, context).DESCRIPTION;
            else
                brandName = checklist.BRAND_OTHER;

            var country = "";
            var listCountry = ART_WF_MOCKUP_CHECK_LIST_COUNTRY_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_COUNTRY() { CHECK_LIST_ID = checklist.CHECK_LIST_ID }, context);
            foreach (var item in listCountry)
            {
                if (country == "") country = SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(item.COUNTRY_ID, context).NAME;
                else country += ", " + SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(item.COUNTRY_ID, context).NAME;
            }

            var pkgType = "";
            if (checklistItem.PACKING_TYPE_ID > 0)
            {
                SAP_M_CHARACTERISTIC_REQUEST param = new SAP_M_CHARACTERISTIC_REQUEST();
                param.data = new SAP_M_CHARACTERISTIC_2();
                param.data.CHARACTERISTIC_ID = Convert.ToInt32(checklistItem.PACKING_TYPE_ID);
                pkgType = ItemPackingTypeHelper.GetPackType(param, context).data[0].DESCRIPTION;
            }

            var listProductCode = ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_PRODUCT() { CHECK_LIST_ID = checklist.CHECK_LIST_ID }, context);
            var listRef = ART_WF_MOCKUP_CHECK_LIST_REFERENCE_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_REFERENCE() { CHECK_LIST_ID = checklist.CHECK_LIST_ID }, context);

            var nw = "";
            var productCode = "";
            foreach (var item in listProductCode.Select(m => m.NET_WEIGHT).Distinct().ToList())
            {
                if (nw == "") nw = item;
                else nw += ", " + item;
            }
            foreach (var item in listRef.Select(m => m.NET_WEIGHT).Distinct().ToList())
            {
                if (nw == "") nw = item;
                else nw += ", " + item;
            }

            foreach (var item in listProductCode.Select(m => m.PRODUCT_CODE).Distinct().ToList())
            {
                if (productCode == "") productCode = item;
                else productCode += ", " + item;
            }
            foreach (var item in listRef.Select(m => m.REFERENCE_NO).Distinct().ToList())
            {
                if (productCode == "") productCode = item;
                else productCode += ", " + item;
            }

            var cusName = checklist.SOLD_TO_ID != null ? XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(checklist.SOLD_TO_ID, context).CUSTOMER_NAME : "";
            var soldToName = checklist.SOLD_TO_ID != null ? XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(checklist.SOLD_TO_ID, context).CUSTOMER_NAME : "";
            var shipToName = checklist.SHIP_TO_ID != null ? XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(checklist.SHIP_TO_ID, context).CUSTOMER_NAME : "";

            string url = ConfigurationManager.AppSettings["MockupURLTaskForm"] + mockupSubId;
            string urls = ConfigurationManager.AppSettings["MockupURLTaskForms"] + mockupSubId;
            if (templateCode == "WF_OTHER_SAVE" || templateCode == "WF_OTHER_SUBMIT")
            {
                url = ConfigurationManager.AppSettings["MockupURLTaskForm"] + process.PARENT_MOCKUP_SUB_ID;
                urls = ConfigurationManager.AppSettings["MockupURLTaskForms"] + process.PARENT_MOCKUP_SUB_ID;
            }
            if (templateCode == "WF_SEND_BACK")
            {
                var SEND_BACK_MK = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_BACK_MK" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                if (SEND_BACK_MK != process.CURRENT_STEP_ID)
                {
                    //send back not is to mk
                    url = ConfigurationManager.AppSettings["MockupURLTaskForm"] + process.PARENT_MOCKUP_SUB_ID;
                    urls = ConfigurationManager.AppSettings["MockupURLTaskForms"] + process.PARENT_MOCKUP_SUB_ID;
                }
            }
            string multiple_wf_number_link = "";
            if (templateCode == "WF_RECALL")
            {
                multiple_wf_number_link = getMultipleWFLink(checklist.CHECK_LIST_ID, "CHECKLIST", context);
            }

            var duration = ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(process.CURRENT_STEP_ID, context).DURATION;
            var finishDate = CNService.AddBusinessDays(process.CREATE_DATE, (int)Math.Ceiling(duration.Value));

            var delegateByName = CNService.GetUserName(process.CREATE_BY, context);
            var reAssignByName = CNService.GetUserName(process.UPDATE_BY, context);

            string owner_mockup = "";
            if (process.PARENT_MOCKUP_SUB_ID != null)
            {
                var mainProcess = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(process.PARENT_MOCKUP_SUB_ID, context);
                owner_mockup = CNService.GetUserName(mainProcess.CURRENT_USER_ID, context);
            }

            string creator_checkList = CNService.GetUserName(checklist.CREATE_BY, context);

            if (string.IsNullOrEmpty(forReason)) forReason = "";
            return value.Replace("[BRAND]", brandName)
              .Replace("[PIC_NAME]", dear_name)
              .Replace("[OWNER_WF]", owner_mockup)
              .Replace("[CREATOR_REQUEST]", creator_checkList)
              .Replace("[CUS_NAME]", cusName)
              .Replace("[COUNTRY]", country)
              .Replace("[PKGTYPE]", pkgType)
              .Replace("[NW]", nw)
              .Replace("[PRODUCT_CODE]", productCode)
              .Replace("[SOLD_TO]", soldToName)
              .Replace("[SHIP_TO]", shipToName)
              .Replace("[DURATION]", duration.ToString())
              .Replace("[LINK]", url)
              .Replace("[LINKS]", urls)
              .Replace("[DELEGATE_FROM]", delegateByName)
              .Replace("[REASSIGN_FROM]", reAssignByName)
              .Replace("[DUE_DATE]", finishDate.ToString("MMMM-dd-yyyy HH:mm:ss"))
              .Replace("[WF_NUMBER]", checklistItem.MOCKUP_NO)
              .Replace("[FOR_REASON]", forReason)
              .Replace("[MULTIPLE_WF_NUMBER_LINK]", multiple_wf_number_link);
        }
        private static void saveLogSendEmailMockup(int mockupSubId, string templateCode, string Subject, string From, string To, string Body, string CC, string status, string msg)
        {
            using (var context = new ARTWORKEntities())
            {
                ART_SYS_LOG_EMAIL item = new ART_SYS_LOG_EMAIL();
                item.MOCKUP_SUB_ID = mockupSubId;
                item.EMAIL_TEMPLATE_CODE = templateCode;
                item.CREATE_BY = -1;
                item.UPDATE_BY = -1;
                item.SUBJECT = Subject;
                item.SEND_FROM = From;
                item.SEND_TO = To;
                item.BODY = Body;
                item.CC = CC;
                item.STATUS = status;
                item.MSG = msg;
                ART_SYS_LOG_EMAIL_SERVICE.SaveOrUpdateNoLog(item, context);
            }
        }

        public static void sendEmailDieline(int mockupId, int mockupSubId, string templateCode, int dielineId, ARTWORKEntities context, string forReason = "")
        {
            try
            {
                var checkListid = CNService.ConvertMockupIdToCheckListId(mockupId, context);
                var checkList = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(checkListid, context);
                var template = ART_M_EMAIL_TEMPLATE_SERVICE.GetByItem(new ART_M_EMAIL_TEMPLATE() { EMAIL_TEMPLATE_CODE = templateCode }, context).FirstOrDefault();
                if (template != null)
                {
                    string from = ConfigurationManager.AppSettings["SMTPFrom"];
                    string to = CNService.GetEmailUserActive(checkList.CREATOR_ID, context);

                    string subject = template.M_SUBJECT;
                    string body = template.M_DEAR + "<br/><br/>";
                    body += template.M_BODY_01;

                    subject = replaceValueMockup(subject, mockupId, mockupSubId, context, templateCode, forReason);
                    body = replaceValueMockup(body, mockupId, mockupSubId, context, templateCode, forReason);

                    EmailService.SendDieline(mockupSubId, templateCode, subject, from, to, body, "", dielineId);
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }
        private static void SendDieline(int mockupSubId, string templateCode, string Subject, string From, string To, string Body, string CC, int dielineId)
        {
            if (!string.IsNullOrEmpty(To))
            {
                string msg_error = "";
                string status = "1";

                Thread email = new Thread(delegate ()
                {
                    try
                    {
                        //mailTo is email1@hotmial.com,email2@hotmial.com
                        MailMessage msg = new MailMessage(From, To, Subject, Body);
                        msg.SubjectEncoding = Encoding.UTF8;
                        msg.BodyEncoding = Encoding.UTF8;
                        msg.IsBodyHtml = true;

                        if (!string.IsNullOrEmpty(CC))
                        {
                            string[] email_cc = CC.Split(',');
                            foreach (string s in email_cc)
                                msg.CC.Add(new MailAddress(s));
                        }

                        int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                        SmtpClient sc = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"], Port);
                        //SmtpClient sc = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"], Port);
                        sc.UseDefaultCredentials = false;
                        //sc.Credentials = new System.Net.NetworkCredential("artwork.thaiunion@gmail.com", "@rtwork123");

                        var token = CWSService.getAuthToken();
                        using (var context = new ARTWORKEntities())
                        {
                            using (CNService.IsolationLevel(context))
                            {
                                var list = ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetByItem(new ART_WF_MOCKUP_ATTACHMENT() { MOCKUP_ID = dielineId }, context);
                                foreach (var item in list)
                                {
                                    var node = CWSService.getNode(Convert.ToInt64(item.NODE_ID), token);
                                    var parentName = CWSService.getNode(Convert.ToInt64(node.ParentID), token).Name;
                                    if (parentName == ConfigurationManager.AppSettings["MockupFolderNameDieline"])
                                    {
                                        var file = CWSService.downloadFile(Convert.ToInt64(item.NODE_ID), token);
                                        System.Net.Mail.Attachment attachment;
                                        attachment = new System.Net.Mail.Attachment(file, item.FILE_NAME);
                                        msg.Attachments.Add(attachment);
                                    }
                                }
                            }
                        }

                        bool IsUseSSL = false;
                        var SMTPSSL = ConfigurationManager.AppSettings["SMTPSSL"];
                        if (SMTPSSL.ToUpper().Trim() == "TRUE")
                        {
                            IsUseSSL = true;
                        }
                        sc.EnableSsl = IsUseSSL;
                        sc.Send(msg);
                    }
                    catch (Exception ex)
                    {
                        msg_error = CNService.GetErrorMessage(ex);
                        status = "0";
                    }
                    finally
                    {
                        saveLogSendEmailMockup(mockupSubId, templateCode, Subject, From, To, Body, CC, status, msg_error);
                    }
                });

                email.IsBackground = true;
                email.Start();
            }
        }

        public static void sendEmailArtworkRequest(int artworkRequestId, string templateCode, ARTWORKEntities context)
        {
            try
            {
                var template = ART_M_EMAIL_TEMPLATE_SERVICE.GetByItem(new ART_M_EMAIL_TEMPLATE() { EMAIL_TEMPLATE_CODE = templateCode }, context).FirstOrDefault();
                if (template != null)
                {
                    string from = ConfigurationManager.AppSettings["SMTPFrom"];
                    string to = "";

                    var request = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(artworkRequestId, context);
                    string cc = "";
                    if (request.REVIEWER_ID > 0)
                    {
                        var email = CNService.GetEmailUserActive(request.REVIEWER_ID, context);
                        if (!string.IsNullOrEmpty(email))
                        {
                            cc = email;
                        }
                    }

                    var RECIPIENT = ART_WF_ARTWORK_REQUEST_RECIPIENT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_RECIPIENT() { ARTWORK_REQUEST_ID = artworkRequestId }, context);
                    foreach (var item in RECIPIENT)
                    {
                        var email = CNService.GetEmailUserActive(item.RECIPIENT_USER_ID, context);
                        if (!string.IsNullOrEmpty(email))
                        {
                            if (to == "")
                                to = email;
                            else
                                to += "," + email;
                        }
                    }

                    string subject = template.M_SUBJECT;
                    string body = template.M_DEAR + "<br/><br/>";
                    body += template.M_BODY_01;

                    subject = replaceValueArtworkRequest(subject, artworkRequestId, context);
                    body = replaceValueArtworkRequest(body, artworkRequestId, context);

                    EmailService.Send(0, 0, artworkRequestId, templateCode, subject, from, to, body, cc);
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }
        private static string replaceValueArtworkRequest(string value, int artworkRequestId, ARTWORKEntities context)
        {
            var request = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(artworkRequestId, context);

            string dear_name = "";
            var RECIPIENT = ART_WF_ARTWORK_REQUEST_RECIPIENT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_RECIPIENT() { ARTWORK_REQUEST_ID = artworkRequestId }, context);
            foreach (var item in RECIPIENT)
            {
                if (dear_name == "")
                    dear_name = CNService.GetUserName(item.RECIPIENT_USER_ID, context);
                else
                    dear_name += "," + CNService.GetUserName(item.RECIPIENT_USER_ID, context);
            }

            var brandName = "";
            if (request.BRAND_ID > 0)
                brandName = SAP_M_BRAND_SERVICE.GetByBRAND_ID(request.BRAND_ID, context).DESCRIPTION;
            else
                brandName = request.BRAND_OTHER;

            var country = "";
            var listCountry = ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_COUNTRY() { ARTWORK_REQUEST_ID = request.ARTWORK_REQUEST_ID }, context);
            foreach (var item in listCountry)
            {
                if (country == "") country = SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(item.COUNTRY_ID, context).NAME;
                else country += ", " + SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(item.COUNTRY_ID, context).NAME;
            }

            var pkgType = "";
            var listProductCode = ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_PRODUCT() { ARTWORK_REQUEST_ID = request.ARTWORK_REQUEST_ID }, context);
            var listRef = ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_REFERENCE() { ARTWORK_REQUEST_ID = request.ARTWORK_REQUEST_ID }, context);

            var nw = "";
            if (listProductCode.Count > 0)
                nw = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(listProductCode.FirstOrDefault().PRODUCT_CODE_ID, context).NET_WEIGHT;

            var productCode = "";
            foreach (var item in listRef.Select(m => m.NET_WEIGHT).Distinct().ToList())
            {
                if (nw == "") nw = item;
                else nw += ", " + item;
            }
            foreach (var item in listProductCode.Select(m => m.PRODUCT_CODE_ID).Distinct().ToList())
            {
                if (productCode == "") productCode = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(item, context).PRODUCT_CODE;
                else productCode += ", " + XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(item, context).PRODUCT_CODE;
            }
            foreach (var item in listRef.Select(m => m.REFERENCE_NO).Distinct().ToList())
            {
                if (productCode == "") productCode = item;
                else productCode += ", " + item;
            }

            var cusName = request.SOLD_TO_ID != null ? XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(request.SOLD_TO_ID, context).CUSTOMER_NAME : "";
            var soldToName = request.SOLD_TO_ID != null ? XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(request.SOLD_TO_ID, context).CUSTOMER_NAME : "";
            var shipToName = request.SHIP_TO_ID != null ? XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(request.SHIP_TO_ID, context).CUSTOMER_NAME : "";

            string url = ConfigurationManager.AppSettings["ArtworkURLArtworkRequest"] + artworkRequestId;
            string urls = ConfigurationManager.AppSettings["ArtworkURLArtworkRequests"] + artworkRequestId;

            //var duration = ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(process.CURRENT_STEP_ID, context).DURATION;
            //var finishDate = CNService.AddBusinessDays(process.CREATE_DATE, Convert.ToInt32(duration));

            //var delegateByName = CNService.GetUserName(process.CREATE_BY, context);
            //var reAssignByName = CNService.GetUserName(process.UPDATE_BY, context);

            //string owner_mockup = "";
            //if (process.PARENT_ARTWORK_SUB_ID != null)
            //{
            //    var mainProcess = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(process.PARENT_ARTWORK_SUB_ID, context);
            //    owner_mockup = CNService.GetUserName(mainProcess.CURRENT_USER_ID, context);
            //}

            string creator_checkList = CNService.GetUserName(request.CREATE_BY, context);

            var duration = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK { STEP_ARTWORK_CODE = "UPLOAD_AW" }, context).FirstOrDefault().DURATION;
            var finishDate = CNService.AddBusinessDays(request.CREATE_DATE, Convert.ToInt32(duration));

            return value.Replace("[BRAND]", brandName)
              .Replace("[PIC_NAME]", dear_name)
              //.Replace("[OWNER_WF]", owner_mockup)
              .Replace("[CREATOR_REQUEST]", creator_checkList)
              .Replace("[CUS_NAME]", cusName)
              .Replace("[COUNTRY]", country)
              .Replace("[PKGTYPE]", pkgType)
              .Replace("[NW]", nw)
              .Replace("[PRODUCT_CODE]", productCode)
              .Replace("[SOLD_TO]", soldToName)
              .Replace("[SHIP_TO]", shipToName)
              //.Replace("[DURATION]", duration.ToString())
              .Replace("[LINK]", url)
              .Replace("[LINKS]", urls)
            //.Replace("[DELEGATE_FROM]", delegateByName)
            //.Replace("[REASSIGN_FROM]", reAssignByName)
              .Replace("[DUE_DATE]", finishDate.ToString("MMMM-dd-yyyy HH:mm:ss"))
              .Replace("[WF_NUMBER]", request.ARTWORK_REQUEST_NO);
        }
        private static void saveLogSendEmailArtworkRequest(int artworkRequestId, string templateCode, string Subject, string From, string To, string Body, string CC, string status, string msg)
        {
            using (var context = new ARTWORKEntities())
            {
                ART_SYS_LOG_EMAIL item = new ART_SYS_LOG_EMAIL();
                item.ARTWORK_REQUEST_ID = artworkRequestId;
                item.EMAIL_TEMPLATE_CODE = templateCode;
                item.CREATE_BY = -1;
                item.UPDATE_BY = -1;
                item.SUBJECT = Subject;
                item.SEND_FROM = From;
                item.SEND_TO = To;
                item.BODY = Body;
                item.CC = CC;
                item.STATUS = status;
                item.MSG = msg;
                ART_SYS_LOG_EMAIL_SERVICE.SaveOrUpdateNoLog(item, context);
            }
        }

        public static void sendEmailArtwork(int artworkRequestId, int artworkSubId, string templateCode, ARTWORKEntities context, string forReason = "")
        {
            try
            {
                if (templateCode == "WF_COMPLETED")
                {
                    artworkSubId = CNService.FindParentArtworkSubId(artworkSubId, context);
                }

                var allStepArtwork = ART_M_STEP_ARTWORK_SERVICE.GetAll(context);
                var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(artworkSubId, context);
                if (templateCode == "WF_SEND_TO")
                {
                    if (process.IS_DELEGATE == "X")
                    {
                        templateCode = "WF_DELEGATE";
                        var log = ART_WF_LOG_DELEGATE_SERVICE.GetByItem(new ART_WF_LOG_DELEGATE() { WF_TYPE = "A", WF_SUB_ID = artworkSubId }, context).OrderByDescending(m => m.ART_WF_LOG_DELEGATE_ID).FirstOrDefault();
                        if (log != null)
                        {
                            forReason = log.REMARK;
                        }
                    }
                }

                var template = ART_M_EMAIL_TEMPLATE_SERVICE.GetByItem(new ART_M_EMAIL_TEMPLATE() { EMAIL_TEMPLATE_CODE = templateCode }, context).FirstOrDefault();
                if (template != null)
                {
                    string from = ConfigurationManager.AppSettings["SMTPFrom"];
                    string to = "";

                    var request = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(artworkRequestId, context);
                    string cc = "";
                    if (request.REVIEWER_ID > 0)
                    {
                        var email = CNService.GetEmailUserActive(request.REVIEWER_ID, context);
                        if (!string.IsNullOrEmpty(email))
                        {
                            cc = email;
                        }
                    }
                    if (process.CURRENT_USER_ID > 0)
                    {
                        var email = CNService.GetEmailUserActive(process.CURRENT_USER_ID, context);
                        if (!string.IsNullOrEmpty(email))
                        {
                            to = email;
                        }
                    }
                    else if (process.CURRENT_ROLE_ID > 0)
                    {
                        var userRole = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { ROLE_ID = Convert.ToInt32(process.CURRENT_ROLE_ID) }, context);
                        foreach (var item in userRole)
                        {
                            var listUserCompany = ART_M_USER_COMPANY_SERVICE.GetByItem(new ART_M_USER_COMPANY() { USER_ID = Convert.ToInt32(item.USER_ID) }, context);
                            var listUserTypeofProduct = ART_M_USER_TYPE_OF_PRODUCT_SERVICE.GetByItem(new ART_M_USER_TYPE_OF_PRODUCT() { USER_ID = Convert.ToInt32(item.USER_ID) }, context);

                            var valid = CNService.CheckTypeOfProductAndCompanyArtwork(item.USER_ID, artworkRequestId, artworkSubId, context, allStepArtwork, listUserCompany, listUserTypeofProduct);
                            if (valid)
                            {
                                var email = CNService.GetEmailUserActive(item.USER_ID, context);
                                if (!string.IsNullOrEmpty(email))
                                {
                                    if (to == "") to = email;
                                    else to += "," + email;
                                }
                            }
                        }
                    }

                    if (templateCode == "WF_OVERDUE1" || templateCode == "WF_OVERDUE2")
                    {
                        if (process.CURRENT_USER_ID > 0)
                        {
                            var leader = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { USER_ID = Convert.ToInt32(process.CURRENT_USER_ID) }, context);
                            foreach (var item in leader)
                            {
                                var email = CNService.GetEmailUserActive(item.UPPER_USER_ID, context);
                                if (!string.IsNullOrEmpty(email))
                                {
                                    if (to == "") to = email;
                                    else to += "," + email;
                                }
                            }
                        }
                    }
                    else if (templateCode == "WF_OVERDUE3")
                    {
                        if (process.CURRENT_USER_ID > 0)
                        {
                            var leader = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { USER_ID = Convert.ToInt32(process.CURRENT_USER_ID) }, context);
                            foreach (var item in leader)
                            {
                                var email = CNService.GetEmailUserActive(item.UPPER_USER_ID, context);
                                if (!string.IsNullOrEmpty(email))
                                {
                                    if (to == "") to = email;
                                    else to += "," + email;
                                }
                                var leader2 = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { USER_ID = Convert.ToInt32(item.UPPER_USER_ID) }, context);
                                foreach (var item2 in leader2)
                                {
                                    var email2 = CNService.GetEmailUserActive(item2.UPPER_USER_ID, context);
                                    if (!string.IsNullOrEmpty(email2))
                                    {
                                        if (to == "") to = email2;
                                        else to += "," + email2;
                                    }
                                }
                            }
                        }
                    }

                    if (templateCode == "WF_TEMINATED")
                    {
                        var PAStep = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        if (PAStep == process.CURRENT_STEP_ID)
                        {
                            var email = CNService.GetEmailUserActive(request.CREATOR_ID, context);
                            if (!string.IsNullOrEmpty(email))
                            {
                                to = email;
                            }
                        }
                    }

                    if (templateCode == "WF_OTHER_SAVE" || templateCode == "WF_OTHER_SUBMIT")
                    {
                        var mainProcess = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(process.PARENT_ARTWORK_SUB_ID, context);
                        var email = CNService.GetEmailUserActive(mainProcess.CURRENT_USER_ID, context);
                        if (!string.IsNullOrEmpty(email))
                        {
                            to = email;
                        }
                    }

                    if (templateCode == "WF_SEND_BACK")
                    {
                        var SEND_BACK_MK = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_BACK_MK" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        if (SEND_BACK_MK != process.CURRENT_STEP_ID)
                        {
                            var mainProcess = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(process.PARENT_ARTWORK_SUB_ID, context);
                            var email = CNService.GetEmailUserActive(mainProcess.CURRENT_USER_ID, context);
                            if (!string.IsNullOrEmpty(email))
                            {
                                to = email;
                            }
                        }
                    }

                    string subject = template.M_SUBJECT;
                    string body = template.M_DEAR + "<br/><br/>";
                    body += template.M_BODY_01;

                    subject = replaceValueArtwork(subject, artworkRequestId, artworkSubId, context, templateCode, forReason);
                    body = replaceValueArtwork(body, artworkRequestId, artworkSubId, context, templateCode, forReason);

                    EmailService.Send(0, artworkSubId, 0, templateCode, subject, from, to, body, cc);
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }
        private static string replaceValueArtwork(string value, int artworkRequestId, int artworkSubId, ARTWORKEntities context, string templateCode, string forReason)
        {
            var artworkRequestItemId = CNService.FindArtworkItemId(artworkSubId, context);
            var request = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(artworkRequestId, context);
            var checklistItem = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(artworkRequestItemId, context);
            var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(artworkSubId, context);
            var customer_cc = ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER { ARTWORK_REQUEST_ID = artworkRequestId, CUSTOMER_USER_ID = Convert.ToInt32(process.CURRENT_USER_ID), MAIL_CC = "X" }, context).FirstOrDefault();
            var customer_to = ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER { ARTWORK_REQUEST_ID = artworkRequestId, MAIL_TO = "X" }, context).FirstOrDefault();

            string dear_name = "";
            if (process.CURRENT_USER_ID != null)
                if (customer_cc != null)
                    dear_name = CNService.GetUserName(customer_to.CUSTOMER_USER_ID, context);
                else
                    dear_name = CNService.GetUserName(process.CURRENT_USER_ID, context);
            else
                dear_name = ART_M_ROLE_SERVICE.GetByROLE_ID(process.CURRENT_ROLE_ID, context).DESCRIPTION;

            var brandName = "";
            if (request.BRAND_ID > 0)
                brandName = SAP_M_BRAND_SERVICE.GetByBRAND_ID(request.BRAND_ID, context).DESCRIPTION;
            else
                brandName = request.BRAND_OTHER;

            var country = "";
            var listCountry = ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_COUNTRY() { ARTWORK_REQUEST_ID = request.ARTWORK_REQUEST_ID }, context);
            foreach (var item in listCountry)
            {
                if (country == "") country = SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(item.COUNTRY_ID, context).NAME;
                else country += ", " + SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(item.COUNTRY_ID, context).NAME;
            }

            var pkgType = "";
            var mainArtworkSubId = CNService.FindParentArtworkSubId(artworkSubId, context);
            var processPA = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PA() { ARTWORK_SUB_ID = mainArtworkSubId }, context).FirstOrDefault();
            if (processPA != null)
            {
                if (processPA.MATERIAL_GROUP_ID > 0)
                {
                    SAP_M_CHARACTERISTIC_REQUEST param = new SAP_M_CHARACTERISTIC_REQUEST();
                    param.data = new SAP_M_CHARACTERISTIC_2();
                    param.data.CHARACTERISTIC_ID = Convert.ToInt32(processPA.MATERIAL_GROUP_ID);
                    pkgType = ItemPackingTypeHelper.GetPackType(param, context).data[0].DESCRIPTION;
                }
            }

            var nw = "";
            var productCode = "";
            var listStrProductCode = new List<string>();
            var tempPA = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PA() { ARTWORK_SUB_ID = mainArtworkSubId }, context);
            if (tempPA.FirstOrDefault() != null)
            {
                if (tempPA.FirstOrDefault().PRODUCT_CODE_ID > 0)
                {
                    var temp = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(tempPA.FirstOrDefault().PRODUCT_CODE_ID, context);
                    if (temp != null)
                        listStrProductCode.Add(temp.PRODUCT_CODE);
                }
                else if (tempPA.FirstOrDefault().RD_REFERENCE_NO_ID > 0)
                {
                    var temp = ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByARTWORK_REFERENCE_ID(tempPA.FirstOrDefault().RD_REFERENCE_NO_ID, context);
                    if (temp != null)
                        listStrProductCode.Add(temp.REFERENCE_NO);
                }
            }

            var listProductCode = ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PA_PRODUCT() { ARTWORK_SUB_ID = mainArtworkSubId }, context);
            foreach (var item in listProductCode)
            {
                var temp = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(item.PRODUCT_CODE_ID, context);
                if (temp != null)
                    listStrProductCode.Add(temp.PRODUCT_CODE);
            }

            var listProductOther = ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER() { ARTWORK_SUB_ID = mainArtworkSubId }, context);
            foreach (var item in listProductOther.Select(m => m.PRODUCT_CODE).ToList())
            {
                listStrProductCode.Add(item);
            }

            foreach (var item in listStrProductCode.Select(m => m).Distinct().ToList())
            {
                if (productCode == "") productCode = item;
                else productCode += ", " + item;
            }

            var listStrNW = new List<string>();
            if (tempPA.FirstOrDefault() != null)
            {
                if (tempPA.FirstOrDefault().RD_REFERENCE_NO_ID > 0)
                {
                    var temp = ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByARTWORK_REFERENCE_ID(tempPA.FirstOrDefault().RD_REFERENCE_NO_ID, context);
                    if (temp != null)
                        listStrNW.Add(temp.NET_WEIGHT);
                }

                if (tempPA.FirstOrDefault().PRODUCT_CODE_ID > 0)
                {
                    var temp = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(tempPA.FirstOrDefault().PRODUCT_CODE_ID, context);
                    if (temp != null)
                        listStrNW.Add(temp.NET_WEIGHT);
                }

                foreach (var item in listProductCode.Select(m => m.PRODUCT_CODE_ID).ToList())
                {
                    var temp = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(item, context);
                    if (temp != null)
                        listStrNW.Add(temp.NET_WEIGHT);
                }
            }

            foreach (var item in listStrNW.Select(m => m).Distinct().ToList())
            {
                if (nw == "") nw = item;
                else nw += ", " + item;
            }

            var cusName = request.SOLD_TO_ID != null ? XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(request.SOLD_TO_ID, context).CUSTOMER_NAME : "";
            var soldToName = request.SOLD_TO_ID != null ? XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(request.SOLD_TO_ID, context).CUSTOMER_NAME : "";
            var shipToName = request.SHIP_TO_ID != null ? XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(request.SHIP_TO_ID, context).CUSTOMER_NAME : "";

            string url = ConfigurationManager.AppSettings["ArtworkURLTaskForm"] + artworkSubId;
            string urls = ConfigurationManager.AppSettings["ArtworkURLTaskForms"] + artworkSubId;
            if (templateCode == "WF_OTHER_SAVE" || templateCode == "WF_OTHER_SUBMIT")
            {
                url = ConfigurationManager.AppSettings["ArtworkURLTaskForm"] + process.PARENT_ARTWORK_SUB_ID;
                urls = ConfigurationManager.AppSettings["ArtworkURLTaskForms"] + process.PARENT_ARTWORK_SUB_ID;
            }
            if (templateCode == "WF_SEND_BACK")
            {
                var SEND_BACK_MK = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_BACK_MK" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                if (SEND_BACK_MK != process.CURRENT_STEP_ID)
                {
                    //send back not is to mk
                    url = ConfigurationManager.AppSettings["ArtworkURLTaskForm"] + process.PARENT_ARTWORK_SUB_ID;
                    urls = ConfigurationManager.AppSettings["ArtworkURLTaskForms"] + process.PARENT_ARTWORK_SUB_ID;
                }
            }
            string multiple_wf_number_link = "";
            if (templateCode == "WF_RECALL")
            {
                multiple_wf_number_link = getMultipleWFLink(request.ARTWORK_REQUEST_ID, "ARTWORK", context);
            }

            var duration = ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(process.CURRENT_STEP_ID, context).DURATION;
            var finishDate = CNService.AddBusinessDays(process.CREATE_DATE, (int)Math.Ceiling(duration.Value));

            var delegateByName = CNService.GetUserName(process.CREATE_BY, context);
            var reAssignByName = CNService.GetUserName(process.UPDATE_BY, context);

            string owner_mockup = "";
            if (process.PARENT_ARTWORK_SUB_ID != null)
            {
                var mainProcess = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(process.PARENT_ARTWORK_SUB_ID, context);
                owner_mockup = CNService.GetUserName(mainProcess.CURRENT_USER_ID, context);
            }

            string creator_checkList = CNService.GetUserName(request.CREATE_BY, context);

            if (string.IsNullOrEmpty(forReason)) forReason = "";
            return value.Replace("[BRAND]", brandName)
              .Replace("[PIC_NAME]", dear_name)
              .Replace("[OWNER_WF]", owner_mockup)
              .Replace("[CREATOR_REQUEST]", creator_checkList)
              .Replace("[CUS_NAME]", cusName)
              .Replace("[COUNTRY]", country)
              .Replace("[PKGTYPE]", pkgType)
              .Replace("[NW]", nw)
              .Replace("[PRODUCT_CODE]", productCode)
              .Replace("[SOLD_TO]", soldToName)
              .Replace("[SHIP_TO]", shipToName)
              .Replace("[DURATION]", duration.ToString())
              .Replace("[LINK]", url)
              .Replace("[LINKS]", urls)
              .Replace("[DELEGATE_FROM]", delegateByName)
              .Replace("[REASSIGN_FROM]", reAssignByName)
              .Replace("[DUE_DATE]", finishDate.ToString("MMMM-dd-yyyy HH:mm:ss"))
              .Replace("[WF_NUMBER]", checklistItem.REQUEST_ITEM_NO)
              .Replace("[FOR_REASON]", forReason)
              .Replace("[MULTIPLE_WF_NUMBER_LINK]", multiple_wf_number_link);
        }
        private static void saveLogSendEmailArtwork(int artworkSubId, string templateCode, string Subject, string From, string To, string Body, string CC, string status, string msg)
        {
            using (var context = new ARTWORKEntities())
            {
                ART_SYS_LOG_EMAIL item = new ART_SYS_LOG_EMAIL();
                item.ARTWORK_SUB_ID = artworkSubId;
                item.EMAIL_TEMPLATE_CODE = templateCode;
                item.CREATE_BY = -1;
                item.UPDATE_BY = -1;
                item.SUBJECT = Subject;
                item.SEND_FROM = From;
                item.SEND_TO = To;
                item.BODY = Body;
                item.CC = CC;
                item.STATUS = status;
                item.MSG = msg;
                ART_SYS_LOG_EMAIL_SERVICE.SaveOrUpdateNoLog(item, context);
            }
        }

        public static void SendEmailForgotPassword(string emailTo, string userName, string verifyCode)
        {
            try
            {
                if (!string.IsNullOrEmpty(emailTo))
                {
                    string Subject = "Reset password [Artwork management system]";
                    string From = ConfigurationManager.AppSettings["SMTPFrom"];
                    string To = emailTo;
                    string Body = "";
                    Body += "<a href='" + ConfigurationManager.AppSettings["ArtworkURLs"] + "Account/ResetPassword?d=" + verifyCode + "'>Please click here to reset your password</a>";
                    Body += "<br/><br/>Best regards";
                    string CC = "";
                    string msg_error = "";
                    string status = "1";

                    Thread email = new Thread(delegate ()
                    {
                        try
                        {
                            //mailTo is email1@hotmial.com,email2@hotmial.com
                            MailMessage msg = new MailMessage(From, To, Subject, Body);
                            msg.SubjectEncoding = Encoding.UTF8;
                            msg.BodyEncoding = Encoding.UTF8;
                            msg.IsBodyHtml = true;

                            if (!string.IsNullOrEmpty(CC))
                            {
                                string[] email_cc = CC.Split(',');
                                foreach (string s in email_cc)
                                    msg.CC.Add(new MailAddress(s));
                            }

                            int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                            SmtpClient sc = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"], Port);
                            sc.UseDefaultCredentials = false;

                            bool IsUseSSL = false;
                            var SMTPSSL = ConfigurationManager.AppSettings["SMTPSSL"];
                            if (SMTPSSL.ToUpper().Trim() == "TRUE")
                            {
                                IsUseSSL = true;
                            }
                            sc.EnableSsl = IsUseSSL;
                            sc.Send(msg);
                        }
                        catch (Exception ex)
                        {
                            msg_error = CNService.GetErrorMessage(ex);
                            status = "0";
                        }
                        finally
                        {
                            saveLogSendEmailForgotPassword(Subject, From, To, Body, CC, status, msg_error);
                        }
                    });

                    email.IsBackground = true;
                    email.Start();
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }
        private static void saveLogSendEmailForgotPassword(string Subject, string From, string To, string Body, string CC, string status, string msg)
        {
            using (var context = new ARTWORKEntities())
            {
                ART_SYS_LOG_EMAIL item = new ART_SYS_LOG_EMAIL();
                item.EMAIL_TEMPLATE_CODE = "ForgotPassword";
                item.CREATE_BY = -1;
                item.UPDATE_BY = -1;
                item.SUBJECT = Subject;
                item.SEND_FROM = From;
                item.SEND_TO = To;
                item.BODY = Body;
                item.CC = CC;
                item.STATUS = status;
                item.MSG = msg;
                ART_SYS_LOG_EMAIL_SERVICE.SaveOrUpdateNoLog(item, context);
            }
        }

        public static void SendEmailNewCustomer(string emailTo, string CustomerName)
        {
            try
            {
                if (!string.IsNullOrEmpty(emailTo))
                {
                    string Subject = "System found new customer [Artwork management system]";
                    string From = ConfigurationManager.AppSettings["SMTPFrom"];
                    string To = emailTo;
                    string Body = "";
                    Body += "System found new customer : " + CustomerName;
                    Body += "<br/><br/><a href='" + ConfigurationManager.AppSettings["ArtworkURL"] + "PIC'>Please click here to set account allocation</a>";
                    Body += "<br/><br/>Best regards";
                    string CC = "";
                    string msg_error = "";
                    string status = "1";

                    Thread email = new Thread(delegate ()
                    {
                        try
                        {
                            MailMessage msg = new MailMessage(From, To, Subject, Body);
                            msg.SubjectEncoding = Encoding.UTF8;
                            msg.BodyEncoding = Encoding.UTF8;
                            msg.IsBodyHtml = true;

                            if (!string.IsNullOrEmpty(CC))
                            {
                                string[] email_cc = CC.Split(',');
                                foreach (string s in email_cc)
                                    msg.CC.Add(new MailAddress(s));
                            }

                            int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                            SmtpClient sc = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"], Port);
                            sc.UseDefaultCredentials = false;

                            bool IsUseSSL = false;
                            var SMTPSSL = ConfigurationManager.AppSettings["SMTPSSL"];
                            if (SMTPSSL.ToUpper().Trim() == "TRUE")
                            {
                                IsUseSSL = true;
                            }
                            sc.EnableSsl = IsUseSSL;
                            sc.Send(msg);
                        }
                        catch (Exception ex)
                        {
                            msg_error = CNService.GetErrorMessage(ex);
                            status = "0";
                        }
                        finally
                        {
                            saveLogSendEmailNewCustomer(Subject, From, To, Body, CC, status, msg_error);
                        }
                    });

                    email.IsBackground = true;
                    email.Start();
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }
        private static void saveLogSendEmailNewCustomer(string Subject, string From, string To, string Body, string CC, string status, string msg)
        {
            using (var context = new ARTWORKEntities())
            {
                ART_SYS_LOG_EMAIL item = new ART_SYS_LOG_EMAIL();
                item.EMAIL_TEMPLATE_CODE = "NewCustomer";
                item.CREATE_BY = -1;
                item.UPDATE_BY = -1;
                item.SUBJECT = Subject;
                item.SEND_FROM = From;
                item.SEND_TO = To;
                item.BODY = Body;
                item.CC = CC;
                item.STATUS = status;
                item.MSG = msg;
                ART_SYS_LOG_EMAIL_SERVICE.SaveOrUpdateNoLog(item, context);
            }
        }

        private static void saveLogSendEmailExtendStepDuration(string Subject, string From, string To, string Body, string CC, string status, string msg, int? mockupSubId, int? requestId, int? artworkSubId)
        {
            using (var context = new ARTWORKEntities())
            {
                ART_SYS_LOG_EMAIL item = new ART_SYS_LOG_EMAIL();
                item.MOCKUP_SUB_ID = mockupSubId;
                item.ARTWORK_REQUEST_ID = requestId;
                item.ARTWORK_SUB_ID = artworkSubId;
                item.EMAIL_TEMPLATE_CODE = "RequestExtendStepDuration";
                item.CREATE_BY = -1;
                item.UPDATE_BY = -1;
                item.SUBJECT = Subject;
                item.SEND_FROM = From;
                item.SEND_TO = To;
                item.BODY = Body;
                item.CC = CC;
                item.STATUS = status;
                item.MSG = msg;
                ART_SYS_LOG_EMAIL_SERVICE.SaveOrUpdateNoLog(item, context);
            }
        }

        private static string getMultipleWFLink(int wf_id, string wf_type, ARTWORKEntities context)
        {
            string multiple_wf_number_link = "";
            string template_link = "&emsp;<a href='{0}'>{1}</a>";
            List<ART_WF_ARTWORK_REQUEST> artwork_ref = new List<ART_WF_ARTWORK_REQUEST>();
            List<ART_WF_MOCKUP_CHECK_LIST> checklist_ref = new List<ART_WF_MOCKUP_CHECK_LIST>();
            if (wf_type == "ARTWORK")
            {
                var artwork_child = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { REFERENCE_REQUEST_ID = wf_id, REFERENCE_REQUEST_TYPE = wf_type }, context);
                artwork_ref.AddRange(artwork_child);

                var artwork_self = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_ID = wf_id }, context);
                artwork_ref.AddRange(artwork_self);

                if (artwork_self.FirstOrDefault().REFERENCE_REQUEST_ID != null)
                {
                    if (artwork_self.FirstOrDefault().REFERENCE_REQUEST_TYPE == "ARTWORK")
                    {
                        var artwork_parent = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_ID = artwork_self.FirstOrDefault().REFERENCE_REQUEST_ID.GetValueOrDefault() }, context);
                        artwork_ref.AddRange(artwork_parent);
                    }

                    if (artwork_self.FirstOrDefault().REFERENCE_REQUEST_TYPE == "CHECKLIST")
                    {
                        var checklist_parent = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST() { CHECK_LIST_ID = artwork_self.FirstOrDefault().REFERENCE_REQUEST_ID.GetValueOrDefault() }, context);
                        checklist_ref.AddRange(checklist_parent);
                    }
                }
            }
            else if (wf_type == "CHECKLIST")
            {
                var checklist_child = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST() { REFERENCE_REQUEST_ID = wf_id, REFERENCE_REQUEST_TYPE = wf_type }, context);
                checklist_ref.AddRange(checklist_child);

                var checklist_self = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST() { CHECK_LIST_ID = wf_id }, context);
                checklist_ref.AddRange(checklist_self);

                if (checklist_ref.FirstOrDefault().REFERENCE_REQUEST_ID != null)
                {
                    if (checklist_ref.FirstOrDefault().REFERENCE_REQUEST_TYPE == "ARTWORK")
                    {
                        var artwork_parent = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_ID = checklist_ref.FirstOrDefault().REFERENCE_REQUEST_ID.GetValueOrDefault() }, context);
                        artwork_ref.AddRange(artwork_parent);
                    }

                    if (checklist_ref.FirstOrDefault().REFERENCE_REQUEST_TYPE == "CHECKLIST")
                    {
                        var checklist_parent = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST() { CHECK_LIST_ID = checklist_ref.FirstOrDefault().REFERENCE_REQUEST_ID.GetValueOrDefault() }, context);
                        checklist_ref.AddRange(checklist_parent);
                    }
                }
            }

            if (artwork_ref != null)
            {
                foreach (var item_ref in artwork_ref)
                {
                    if (multiple_wf_number_link == "")
                    {
                        multiple_wf_number_link = item_ref.ARTWORK_REQUEST_NO;
                    }
                    else
                    {
                        multiple_wf_number_link += "<br/>" + item_ref.ARTWORK_REQUEST_NO;
                    }
                    var request_item = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_ITEM() { ARTWORK_REQUEST_ID = item_ref.ARTWORK_REQUEST_ID }, context);
                    if (request_item != null)
                    {
                        foreach (var item in request_item)
                        {
                            var process_sub = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID, ARTWORK_ITEM_ID = item.ARTWORK_ITEM_ID, PARENT_ARTWORK_SUB_ID = null }, context).FirstOrDefault();
                            if (process_sub != null)
                            {
                                string template_link_aw = string.Format(template_link, ConfigurationManager.AppSettings["ArtworkURLTaskForm"] + process_sub.ARTWORK_SUB_ID, item.REQUEST_ITEM_NO);
                                multiple_wf_number_link += "<br/>" + template_link_aw;
                            }
                        }
                    }
                }

            }

            if (checklist_ref != null)
            {
                foreach (var item_ref in checklist_ref)
                {
                    if (multiple_wf_number_link == "")
                    {
                        multiple_wf_number_link = item_ref.CHECK_LIST_NO;
                    }
                    else
                    {
                        multiple_wf_number_link += "<br/>" + item_ref.CHECK_LIST_NO;
                    }
                    var checklist_item = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_ITEM() { CHECK_LIST_ID = item_ref.CHECK_LIST_ID }, context);
                    if (checklist_item != null)
                    {
                        foreach (var item in checklist_item)
                        {
                            var process_sub = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = item.MOCKUP_ID, PARENT_MOCKUP_SUB_ID = null }, context).FirstOrDefault();
                            if (process_sub != null)
                            {
                                string template_link_mo = string.Format(template_link, ConfigurationManager.AppSettings["MockupURLTaskForm"] + process_sub.MOCKUP_SUB_ID, item.MOCKUP_NO);
                                multiple_wf_number_link += "<br/>" + template_link_mo;
                            }
                        }
                    }
                }

            }

            return multiple_wf_number_link;
        }

        public static void sendEmailChangeOwner(int requestId, int subId, string wfTpye, int? fromUserId, int? toUserId, string templateCode, ARTWORKEntities context, string forReason = "")
        {
            try
            {
                var template = ART_M_EMAIL_TEMPLATE_SERVICE.GetByItem(new ART_M_EMAIL_TEMPLATE() { EMAIL_TEMPLATE_CODE = templateCode }, context).FirstOrDefault();
                if (template != null)
                {
                    string from = ConfigurationManager.AppSettings["SMTPFrom"];
                    string to = CNService.GetEmailUserActive(toUserId, context);

                    string subject = template.M_SUBJECT;
                    string body = template.M_DEAR + "<br/><br/>";
                    body += template.M_BODY_01;

                    var brandName = ""; var dear_name = ""; var cusName = ""; var country = ""; var pkgType = ""; var nw = ""; var productCode = ""; var soldToName = ""; var shipToName = "";
                    var url = ""; var urls = ""; var reAssignByName = ""; var wfNo = "";

                    dear_name = CNService.GetUserName(toUserId, context);
                    reAssignByName = CNService.GetUserName(fromUserId, context);

                    if (wfTpye == "Artwork")
                    {
                        var request = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(requestId, context);

                        if (request.BRAND_ID > 0)
                            brandName = SAP_M_BRAND_SERVICE.GetByBRAND_ID(request.BRAND_ID, context).DESCRIPTION;
                        else
                            brandName = request.BRAND_OTHER;

                        var listCountry = ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_COUNTRY() { ARTWORK_REQUEST_ID = request.ARTWORK_REQUEST_ID }, context);
                        foreach (var item in listCountry)
                        {
                            if (country == "") country = SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(item.COUNTRY_ID, context).NAME;
                            else country += ", " + SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(item.COUNTRY_ID, context).NAME;
                        }

                        var listProductCode = ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_PRODUCT() { ARTWORK_REQUEST_ID = request.ARTWORK_REQUEST_ID }, context);
                        var listRef = ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_REFERENCE() { ARTWORK_REQUEST_ID = request.ARTWORK_REQUEST_ID }, context);

                        if (listProductCode.Count > 0)
                            nw = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(listProductCode.FirstOrDefault().PRODUCT_CODE_ID, context).NET_WEIGHT;

                        foreach (var item in listRef.Select(m => m.NET_WEIGHT).Distinct().ToList())
                        {
                            if (nw == "") nw = item;
                            else nw += ", " + item;
                        }
                        foreach (var item in listProductCode.Select(m => m.PRODUCT_CODE_ID).Distinct().ToList())
                        {
                            if (productCode == "") productCode = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(item, context).PRODUCT_CODE;
                            else productCode += ", " + XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(item, context).PRODUCT_CODE;
                        }
                        foreach (var item in listRef.Select(m => m.REFERENCE_NO).Distinct().ToList())
                        {
                            if (productCode == "") productCode = item;
                            else productCode += ", " + item;
                        }

                        cusName = request.SOLD_TO_ID != null ? XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(request.SOLD_TO_ID, context).CUSTOMER_NAME : "";
                        soldToName = request.SOLD_TO_ID != null ? XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(request.SOLD_TO_ID, context).CUSTOMER_NAME : "";
                        shipToName = request.SHIP_TO_ID != null ? XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(request.SHIP_TO_ID, context).CUSTOMER_NAME : "";

                        url = ConfigurationManager.AppSettings["ArtworkURLTaskForm"] + subId;
                        urls = ConfigurationManager.AppSettings["ArtworkURLTaskForms"] + subId;
                    }
                    else
                    {
                        var checklist = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(requestId, context);
                        var process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(subId, context);
                        var checklistItem = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(process.MOCKUP_ID, context);

                        if (checklist.BRAND_ID > 0)
                            brandName = SAP_M_BRAND_SERVICE.GetByBRAND_ID(checklist.BRAND_ID, context).DESCRIPTION;
                        else
                            brandName = checklist.BRAND_OTHER;

                        var listCountry = ART_WF_MOCKUP_CHECK_LIST_COUNTRY_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_COUNTRY() { CHECK_LIST_ID = checklist.CHECK_LIST_ID }, context);
                        foreach (var item in listCountry)
                        {
                            if (country == "") country = SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(item.COUNTRY_ID, context).NAME;
                            else country += ", " + SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(item.COUNTRY_ID, context).NAME;
                        }

                        if (checklistItem.PACKING_TYPE_ID > 0)
                        {
                            SAP_M_CHARACTERISTIC_REQUEST param = new SAP_M_CHARACTERISTIC_REQUEST();
                            param.data = new SAP_M_CHARACTERISTIC_2();
                            param.data.CHARACTERISTIC_ID = Convert.ToInt32(checklistItem.PACKING_TYPE_ID);
                            pkgType = ItemPackingTypeHelper.GetPackType(param, context).data[0].DESCRIPTION;
                        }

                        var listProductCode = ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_PRODUCT() { CHECK_LIST_ID = checklist.CHECK_LIST_ID }, context);
                        var listRef = ART_WF_MOCKUP_CHECK_LIST_REFERENCE_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_REFERENCE() { CHECK_LIST_ID = checklist.CHECK_LIST_ID }, context);

                        foreach (var item in listProductCode.Select(m => m.NET_WEIGHT).Distinct().ToList())
                        {
                            if (nw == "") nw = item;
                            else nw += ", " + item;
                        }
                        foreach (var item in listRef.Select(m => m.NET_WEIGHT).Distinct().ToList())
                        {
                            if (nw == "") nw = item;
                            else nw += ", " + item;
                        }

                        foreach (var item in listProductCode.Select(m => m.PRODUCT_CODE).Distinct().ToList())
                        {
                            if (productCode == "") productCode = item;
                            else productCode += ", " + item;
                        }
                        foreach (var item in listRef.Select(m => m.REFERENCE_NO).Distinct().ToList())
                        {
                            if (productCode == "") productCode = item;
                            else productCode += ", " + item;
                        }

                        cusName = checklist.SOLD_TO_ID != null ? XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(checklist.SOLD_TO_ID, context).CUSTOMER_NAME : "";
                        soldToName = checklist.SOLD_TO_ID != null ? XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(checklist.SOLD_TO_ID, context).CUSTOMER_NAME : "";
                        shipToName = checklist.SHIP_TO_ID != null ? XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(checklist.SHIP_TO_ID, context).CUSTOMER_NAME : "";

                        url = ConfigurationManager.AppSettings["MockupURLTaskForm"] + subId;
                        urls = ConfigurationManager.AppSettings["MockupURLTaskForms"] + subId;
                    }

                    subject = subject.Replace("[BRAND]", brandName)
                                  .Replace("[PIC_NAME]", dear_name)
                                  .Replace("[CUS_NAME]", cusName)
                                  .Replace("[COUNTRY]", country)
                                  .Replace("[PKGTYPE]", pkgType)
                                  .Replace("[NW]", nw)
                                  .Replace("[PRODUCT_CODE]", productCode)
                                  .Replace("[SOLD_TO]", soldToName)
                                  .Replace("[SHIP_TO]", shipToName)
                                  .Replace("[LINK]", url)
                                  .Replace("[LINKS]", urls)
                                  .Replace("[CHANGE_OWNER_FROM]", reAssignByName)
                                  .Replace("[WF_NUMBER]", wfNo)
                                  .Replace("[FOR_REASON]", forReason);

                    body = body.Replace("[BRAND]", brandName)
                               .Replace("[PIC_NAME]", dear_name)
                               .Replace("[CUS_NAME]", cusName)
                               .Replace("[COUNTRY]", country)
                               .Replace("[PKGTYPE]", pkgType)
                               .Replace("[NW]", nw)
                               .Replace("[PRODUCT_CODE]", productCode)
                               .Replace("[SOLD_TO]", soldToName)
                               .Replace("[SHIP_TO]", shipToName)
                               .Replace("[LINK]", url)
                               .Replace("[LINKS]", urls)
                               .Replace("[CHANGE_OWNER_FROM]", reAssignByName)
                               .Replace("[WF_NUMBER]", wfNo)
                               .Replace("[FOR_REASON]", forReason);

                    if (wfTpye == "Artwork")
                    {
                        EmailService.Send(0, subId, 0, templateCode, subject, from, to, body, "");
                    }
                    else
                    {
                        EmailService.Send(subId, 0, 0, templateCode, subject, from, to, body, "");
                    }
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }

        public static void SendEmailExtendDuration(string emailTo, string WFNo, DateTime duaDate, string department, string stepName, int? mockupSubId, int? requestId, int? artworkSubId)
        {
            try
            {
                if (!string.IsNullOrEmpty(emailTo))
                {
                    string Subject = string.Format("{0} is requested to extend to {1}", WFNo, duaDate.Date.ToShortDateString());
                    string From = ConfigurationManager.AppSettings["SMTPFrom"];
                    string To = emailTo;
                    string Body = "";
                    Body = string.Format("The workflow {0} has been extend to {1} by {2} on {3}", WFNo, duaDate.Date.ToShortDateString(), department.ToLower(), stepName.ToLower());

                    string CC = "";
                    string msg_error = "";
                    string status = "1";

                    Thread email = new Thread(delegate ()
                    {
                        try
                        {
                            MailMessage msg = new MailMessage(From, To, Subject, Body);
                            msg.SubjectEncoding = Encoding.UTF8;
                            msg.BodyEncoding = Encoding.UTF8;
                            msg.IsBodyHtml = true;

                            if (!string.IsNullOrEmpty(CC))
                            {
                                string[] email_cc = CC.Split(',');
                                foreach (string s in email_cc)
                                    msg.CC.Add(new MailAddress(s));
                            }

                            int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
                            SmtpClient sc = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"], Port);
                            sc.UseDefaultCredentials = false;

                            bool IsUseSSL = false;
                            var SMTPSSL = ConfigurationManager.AppSettings["SMTPSSL"];
                            if (SMTPSSL.ToUpper().Trim() == "TRUE")
                            {
                                IsUseSSL = true;
                            }
                            sc.EnableSsl = IsUseSSL;
                            sc.Send(msg);
                        }
                        catch (Exception ex)
                        {
                            msg_error = CNService.GetErrorMessage(ex);
                            status = "0";
                        }
                        finally
                        {
                            saveLogSendEmailExtendStepDuration(Subject, From, To, Body, CC, status, msg_error, mockupSubId, requestId, artworkSubId);
                        }
                    });

                    email.IsBackground = true;
                    email.Start();
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }
    }
}