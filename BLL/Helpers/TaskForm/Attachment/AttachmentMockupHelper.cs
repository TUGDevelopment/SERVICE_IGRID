using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace BLL.Helpers
{
    public class AttachmentMockupHelper
    {
        public static ART_WF_MOCKUP_ATTACHMENT_RESULT GetAttachmentInfo(ART_WF_MOCKUP_ATTACHMENT_REQUEST param)
        {
            ART_WF_MOCKUP_ATTACHMENT_RESULT Results = new ART_WF_MOCKUP_ATTACHMENT_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var createById = param.data.currentUserId;
                        var isCustomer = CNService.IsCustomer(createById, context);
                        var isVendor = CNService.IsVendor(createById, context);

                        var isOwner = false;
                        var SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var process2 = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID, CURRENT_STEP_ID = SEND_PG }, context);
                        if (process2.FirstOrDefault().CURRENT_USER_ID == param.data.currentUserId)
                        {
                            isOwner = true;
                        }

                        var mockupSubId = param.data.MOCKUP_SUB_ID;
                        param.data.MOCKUP_SUB_ID = 0;
                        var oldRoleId = param.data.ROLE_ID;

                        if (isVendor)
                        {
                            param.data.ROLE_ID = null;
                            param.data.IS_VENDOR = null;
                            Results.data = MapperServices.ART_WF_MOCKUP_ATTACHMENT(ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetByItem(MapperServices.ART_WF_MOCKUP_ATTACHMENT(param.data), context));

                            var vendorId = ART_M_USER_VENDOR_SERVICE.GetByItem(new ART_M_USER_VENDOR() { USER_ID = createById }, context).FirstOrDefault().VENDOR_ID;
                            var listUserVendor = ART_M_USER_VENDOR_SERVICE.GetByItem(new ART_M_USER_VENDOR() { VENDOR_ID = vendorId }, context).Select(m => m.USER_ID);

                            Results.data = Results.data.Where(m => listUserVendor.Contains(m.CREATE_BY) || m.IS_VENDOR == "X").ToList();
                        }
                        else if (isCustomer)
                        {
                            param.data.ROLE_ID = null;
                            param.data.IS_CUSTOMER = null;
                            Results.data = MapperServices.ART_WF_MOCKUP_ATTACHMENT(ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetByItem(MapperServices.ART_WF_MOCKUP_ATTACHMENT(param.data), context));

                            var process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(mockupSubId, context);
                            var checkListId = CNService.ConvertMockupIdToCheckListId(process.MOCKUP_ID, context);
                            var listUserCustomer = ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER() { CHECK_LIST_ID = checkListId }, context).Select(m => m.CUSTOMER_USER_ID);

                            Results.data = Results.data.Where(m => listUserCustomer.Contains(m.CREATE_BY) || m.IS_CUSTOMER == "X").ToList();
                        }
                        else
                        {
                            if (param.data.ROLE_ID == null)
                            {
                                //tab att
                                param.data.ROLE_ID = null;
                                Results.data = MapperServices.ART_WF_MOCKUP_ATTACHMENT(ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetByItem(MapperServices.ART_WF_MOCKUP_ATTACHMENT(param.data), context));
                            }
                            else
                            {
                                //internal popup
                                param.data.ROLE_ID = null;
                                param.data.IS_INTERNAL = null;
                                param.data.IS_VENDOR = null;
                                param.data.IS_CUSTOMER = null;
                                Results.data = MapperServices.ART_WF_MOCKUP_ATTACHMENT(ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetByItem(MapperServices.ART_WF_MOCKUP_ATTACHMENT(param.data), context));

                                if (isOwner)
                                    Results.data = Results.data.ToList();
                                else
                                    Results.data = Results.data.Where(m => m.IS_INTERNAL == "X").ToList();
                            }
                        }

                        Results.data = Results.data.Where(m => m.VERSION == 1).ToList();

                        foreach (var item in Results.data)
                        {
                            //if (item.CREATE_BY == createById)
                            //    item.canDelete = true;
                            item.canDownload = true;
                            item.canAddVersion = true;

                            if (isOwner)
                                item.canDelete = true;
                            else
                            {
                                if (mockupSubId == item.MOCKUP_SUB_ID)
                                {
                                    item.canDelete = true;
                                }
                            }

                            var att_last_version = ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetByItem(new ART_WF_MOCKUP_ATTACHMENT() { NODE_ID = item.NODE_ID }, context).OrderByDescending(m => m.CREATE_DATE).FirstOrDefault();
                            item.VERSION2 = att_last_version.VERSION2;
                            item.FILE_NAME = att_last_version.FILE_NAME;
                            item.EXTENSION = att_last_version.EXTENSION;
                            item.CREATE_BY = att_last_version.CREATE_BY;
                            item.CREATE_DATE = att_last_version.CREATE_DATE;

                            if (item.STEP_MOCKUP_ID != null)
                            {
                                item.step = ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(item.STEP_MOCKUP_ID, context).STEP_MOCKUP_NAME;
                                item.remark = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(item.MOCKUP_SUB_ID, context).REMARK;
                            }

                            if (item.ROLE_ID != null)
                            {
                                var openUpload = ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(item.ROLE_ID, context).STEP_MOCKUP_NAME;

                                if (!String.IsNullOrEmpty(openUpload))
                                {
                                    if (openUpload == "Opened")
                                    {
                                        item.step = openUpload;
                                        item.canAddVersion = false;
                                    }
                                }
                            }


                            item.create_by_display_txt = CNService.GetUserName(item.CREATE_BY, context);
                            if (CNService.IsVendor(item.CREATE_BY, context))
                            {
                                var listUserVendor = ART_M_USER_VENDOR_SERVICE.GetByItem(new DAL.ART_M_USER_VENDOR() { USER_ID = Convert.ToInt32(item.CREATE_BY) }, context);
                                if (listUserVendor.Count() > 0)
                                {
                                    item.create_by_desc_txt = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(listUserVendor.FirstOrDefault().VENDOR_ID, context).VENDOR_NAME;
                                }
                            }
                            else if (CNService.IsCustomer(item.CREATE_BY, context))
                            {
                                var listUserCustomer = ART_M_USER_CUSTOMER_SERVICE.GetByItem(new DAL.ART_M_USER_CUSTOMER() { USER_ID = Convert.ToInt32(item.CREATE_BY) }, context);
                                if (listUserCustomer.Count() > 0)
                                {
                                    item.create_by_desc_txt = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(listUserCustomer.FirstOrDefault().CUSTOMER_ID, context).CUSTOMER_NAME;
                                }
                            }
                            item.create_date_display_txt = item.CREATE_DATE;

                            item.NODE_ID_TXT = EncryptionService.EncryptAndUrlEncode(item.NODE_ID.ToString());
                        }
                    }
                }

                Results.data = Results.data.OrderBy(m => m.CREATE_DATE).ToList();
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static ART_WF_MOCKUP_ATTACHMENT_RESULT GetAttachmentInfoFileVersion(ART_WF_MOCKUP_ATTACHMENT_REQUEST param)
        {
            ART_WF_MOCKUP_ATTACHMENT_RESULT Results = new ART_WF_MOCKUP_ATTACHMENT_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var mockupSubId = param.data.MOCKUP_SUB_ID;
                        param.data.MOCKUP_SUB_ID = 0;

                        if (param == null || param.data == null)
                        {
                            //Results.data = MapperServices.ART_WF_MOCKUP_ATTACHMENT(ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetAll());
                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_ATTACHMENT(ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetByItem(MapperServices.ART_WF_MOCKUP_ATTACHMENT(param.data), context));
                            Results.data = Results.data.OrderBy(m => m.VERSION).ToList();
                        }

                        var createById = param.data.currentUserId;
                        var isCustomer = CNService.IsCustomer(createById, context);
                        var isVendor = CNService.IsVendor(createById, context);

                        if (isCustomer || isVendor)
                        {
                            var max = Results.data.Select(m => m.VERSION).Max();
                            Results.data = Results.data.Where(m => m.VERSION == 1 || m.VERSION == max).ToList();
                        }

                        var isOwner = false;
                        var SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID, CURRENT_STEP_ID = SEND_PG }, context);
                        if (process.FirstOrDefault().CURRENT_USER_ID == param.data.currentUserId)
                        {
                            isOwner = true;
                        }

                        foreach (var item in Results.data)
                        {
                            //if (item.CREATE_BY == createById)
                            //    item.canDelete = true;

                            item.canDownload = true;
                            item.canAddVersion = true;
                            item.create_by_display_txt = CNService.GetUserName(item.CREATE_BY, context);

                            if (isOwner) item.canDelete = true;
                            else
                            {
                                if (mockupSubId == item.MOCKUP_SUB_ID)
                                {
                                    item.canDelete = true;
                                }
                            }

                            if (item.STEP_MOCKUP_ID != null)
                            {
                                item.step = ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(item.STEP_MOCKUP_ID, context).STEP_MOCKUP_NAME;
                                item.remark = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(item.MOCKUP_SUB_ID, context).REMARK;
                            }

                            if (CNService.IsVendor(item.CREATE_BY, context))
                            {
                                var listUserVendor = ART_M_USER_VENDOR_SERVICE.GetByItem(new DAL.ART_M_USER_VENDOR() { USER_ID = Convert.ToInt32(item.CREATE_BY) }, context);
                                if (listUserVendor.Count() > 0)
                                {
                                    item.create_by_desc_txt = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(listUserVendor.FirstOrDefault().VENDOR_ID, context).VENDOR_NAME;
                                }
                            }
                            else if (CNService.IsCustomer(item.CREATE_BY, context))
                            {
                                var listUserCustomer = ART_M_USER_CUSTOMER_SERVICE.GetByItem(new DAL.ART_M_USER_CUSTOMER() { USER_ID = Convert.ToInt32(item.CREATE_BY) }, context);
                                if (listUserCustomer.Count() > 0)
                                {
                                    item.create_by_desc_txt = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(listUserCustomer.FirstOrDefault().CUSTOMER_ID, context).CUSTOMER_NAME;
                                }
                            }

                            item.create_date_display_txt = item.CREATE_DATE;
                            item.NODE_ID_TXT = EncryptionService.EncryptAndUrlEncode(item.NODE_ID.ToString());
                        }
                    }
                }

                Results.data = Results.data.OrderBy(m => m.CREATE_DATE).ToList();
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static ART_WF_MOCKUP_ATTACHMENT SaveAttachment(string fileName, string extension, string contentType, int mockupID
                                                                    , int mockupsubID, int size, long nodeID, int userID, int? roleId
                                                                    , int? version, string is_internal, string is_customer, string is_vendor
                                                                    , ARTWORKEntities context)
        {
            ART_WF_MOCKUP_ATTACHMENT attachmentData = new ART_WF_MOCKUP_ATTACHMENT();

            try
            {
                //using (var context = new ARTWORKEntities())
                //{
                //using (var dbContextTransaction = CNService.IsolationLevel(context))
                //{
                attachmentData.MOCKUP_ID = mockupID;
                attachmentData.MOCKUP_SUB_ID = mockupsubID;
                attachmentData.NODE_ID = nodeID;
                attachmentData.SIZE = size;
                attachmentData.CONTENT_TYPE = contentType;
                attachmentData.FILE_NAME = fileName;
                attachmentData.EXTENSION = extension;
                attachmentData.CREATE_BY = userID;
                attachmentData.UPDATE_BY = userID;
                attachmentData.ROLE_ID = roleId;

                attachmentData.VERSION2 = "1.0";
                attachmentData.VERSION = 1;
                attachmentData.IS_INTERNAL = is_internal;
                attachmentData.IS_CUSTOMER = is_customer;
                attachmentData.IS_VENDOR = is_vendor;
                attachmentData.STEP_MOCKUP_ID = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(mockupsubID, context).CURRENT_STEP_ID;

                ART_WF_MOCKUP_ATTACHMENT_SERVICE.SaveOrUpdate(attachmentData, context);

                //dbContextTransaction.Commit();
                //}
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return attachmentData;
        }

        public static ART_WF_MOCKUP_ATTACHMENT SaveAttachmentFileVersion(string fileName, string extension, string contentType, int mockupID
                                                               , int mockupsubID, int size, long nodeID, int userID, long version
                                                               , ARTWORKEntities context)
        {
            ART_WF_MOCKUP_ATTACHMENT attachmentData = new ART_WF_MOCKUP_ATTACHMENT();

            try
            {
                //using (var context = new ARTWORKEntities())
                //{
                //using (var dbContextTransaction = CNService.IsolationLevel(context))
                //{
                var att_first_version = ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetByItem(new ART_WF_MOCKUP_ATTACHMENT() { NODE_ID = nodeID, VERSION = 1 }, context).FirstOrDefault();
                var att_last_version = ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetByItem(new ART_WF_MOCKUP_ATTACHMENT() { NODE_ID = nodeID }, context).OrderByDescending(m => m.CREATE_DATE).FirstOrDefault();
                double version2 = Convert.ToDouble(att_last_version.VERSION2);

                if (CNService.IsCustomer(userID, context))
                {
                    attachmentData.VERSION2 = (Convert.ToInt32(version2) + 1).ToString();
                }
                else
                {
                    attachmentData.VERSION2 = (version2 + 0.1).ToString();
                }

                attachmentData.MOCKUP_ID = mockupID;
                attachmentData.MOCKUP_SUB_ID = mockupsubID;
                attachmentData.NODE_ID = nodeID;
                attachmentData.SIZE = size;
                attachmentData.CONTENT_TYPE = contentType;
                attachmentData.FILE_NAME = fileName;
                attachmentData.EXTENSION = extension;
                attachmentData.CREATE_BY = userID;
                attachmentData.UPDATE_BY = userID;
                attachmentData.ROLE_ID = att_first_version.ROLE_ID;
                attachmentData.VERSION = version;
                attachmentData.IS_INTERNAL = att_first_version.IS_INTERNAL;
                attachmentData.IS_CUSTOMER = att_first_version.IS_CUSTOMER;
                attachmentData.IS_VENDOR = att_first_version.IS_VENDOR;
                attachmentData.STEP_MOCKUP_ID = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(mockupsubID, context).CURRENT_STEP_ID;

                ART_WF_MOCKUP_ATTACHMENT_SERVICE.SaveOrUpdate(attachmentData, context);

                //dbContextTransaction.Commit();
                //}
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return attachmentData;
        }

        public static ART_WF_MOCKUP_ATTACHMENT_RESULT DeleteAttachment(ART_WF_MOCKUP_ATTACHMENT_REQUEST param)
        {
            ART_WF_MOCKUP_ATTACHMENT_RESULT Results = new ART_WF_MOCKUP_ATTACHMENT_RESULT();
            List<ART_WF_MOCKUP_ATTACHMENT_2> listAttachment = new List<ART_WF_MOCKUP_ATTACHMENT_2>();
            List<ART_WF_MOCKUP_ATTACHMENT> attachments = new List<ART_WF_MOCKUP_ATTACHMENT>();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        long node_id = 0;
                        node_id = Convert.ToInt64(param.data.NODE_ID);

                        attachments = ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetByItem(new ART_WF_MOCKUP_ATTACHMENT() { NODE_ID = node_id }, context);
                        foreach (var attachment in attachments)
                        {
                            ART_WF_MOCKUP_ATTACHMENT_SERVICE.DeleteByMOCKUP_ATTACHMENT_ID(attachment.MOCKUP_ATTACHMENT_ID, context);
                        }
                        //Delete file on CS

                        var token = CWSService.getAuthToken();
                        CWSService.deleteNode(node_id, token);

                        dbContextTransaction.Commit();
                    }
                }

                Results.status = "S";
                Results.msg = MessageHelper.GetMessage("MSG_001");
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static ART_WF_MOCKUP_ATTACHMENT_RESULT DeleteAttachmentVersion(ART_WF_MOCKUP_ATTACHMENT_REQUEST param)
        {
            ART_WF_MOCKUP_ATTACHMENT_RESULT Results = new ART_WF_MOCKUP_ATTACHMENT_RESULT();
            List<ART_WF_MOCKUP_ATTACHMENT_2> listAttachment = new List<ART_WF_MOCKUP_ATTACHMENT_2>();
            List<ART_WF_MOCKUP_ATTACHMENT> attachments = new List<ART_WF_MOCKUP_ATTACHMENT>();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        long node_id = 0;
                        node_id = Convert.ToInt64(param.data.NODE_ID);
                        long version = 0;
                        version = Convert.ToInt64(param.data.VERSION);
                        attachments = ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetByItem(new ART_WF_MOCKUP_ATTACHMENT() { NODE_ID = node_id, VERSION = version }, context);
                        foreach (var attachment in attachments)
                        {
                            ART_WF_MOCKUP_ATTACHMENT_SERVICE.DeleteByMOCKUP_ATTACHMENT_ID(attachment.MOCKUP_ATTACHMENT_ID, context);
                        }
                        //Delete file on CS

                        var token = CWSService.getAuthToken();
                        CWSService.deleteNodeVersion(node_id, version, token);

                        dbContextTransaction.Commit();
                    }
                }

                Results.status = "S";
                Results.msg = MessageHelper.GetMessage("MSG_001");
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static ART_WF_MOCKUP_ATTACHMENT_RESULT PostVisibility(ART_WF_MOCKUP_ATTACHMENT_REQUEST param)
        {
            ART_WF_MOCKUP_ATTACHMENT_RESULT Results = new ART_WF_MOCKUP_ATTACHMENT_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        long node_id = 0;
                        node_id = Convert.ToInt64(param.data.NODE_ID);

                        var attachments = ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetByItem(new ART_WF_MOCKUP_ATTACHMENT() { NODE_ID = node_id }, context);
                        foreach (var attachment in attachments)
                        {
                            attachment.IS_CUSTOMER = param.data.IS_CUSTOMER;
                            attachment.IS_INTERNAL = param.data.IS_INTERNAL;
                            attachment.IS_VENDOR = param.data.IS_VENDOR;
                            ART_WF_MOCKUP_ATTACHMENT_SERVICE.SaveOrUpdate(attachment, context);
                        }

                        dbContextTransaction.Commit();
                    }
                }

                Results.status = "S";
                Results.msg = MessageHelper.GetMessage("MSG_001");
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
