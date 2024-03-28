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
    public class AttachmentArtworkHelper
    {
        public static ART_WF_ARTWORK_ATTACHMENT_RESULT GetAttachmentInfo(ART_WF_ARTWORK_ATTACHMENT_REQUEST param)
        {
            ART_WF_ARTWORK_ATTACHMENT_RESULT Results = new ART_WF_ARTWORK_ATTACHMENT_RESULT();
            List<ART_WF_ARTWORK_ATTACHMENT_2> listAttachment = new List<ART_WF_ARTWORK_ATTACHMENT_2>();
            List<ART_WF_ARTWORK_ATTACHMENT_2> listUpload = new List<ART_WF_ARTWORK_ATTACHMENT_2>();
            try
            {
                var createById = param.data.currentUserId;

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var isCustomer = CNService.IsCustomer(createById, context);
                        var isVendor = CNService.IsVendor(createById, context);

                        var artworkSubId = param.data.ARTWORK_SUB_ID;
                        param.data.ARTWORK_SUB_ID = 0;

                        var isOwner = false;
                        var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var process2 = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = artworkSubId }, context);
                        var process3 = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { CURRENT_STEP_ID = SEND_PA, ARTWORK_ITEM_ID = process2.FirstOrDefault().ARTWORK_ITEM_ID }, context);
                        if (process3.FirstOrDefault().CURRENT_USER_ID == param.data.currentUserId)
                        {
                            isOwner = true;
                        }

                        if (isVendor)
                        {
                            var oldRoleId = param.data.ROLE_ID;
                            param.data.ROLE_ID = null;
                            param.data.IS_VENDOR = null;
                            listAttachment.AddRange(MapperServices.ART_WF_ARTWORK_ATTACHMENT(ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_ATTACHMENT(param.data), context)));

                            var vendorId = ART_M_USER_VENDOR_SERVICE.GetByItem(new ART_M_USER_VENDOR() { USER_ID = createById }, context).FirstOrDefault().VENDOR_ID;
                            var listUserVendor = ART_M_USER_VENDOR_SERVICE.GetByItem(new ART_M_USER_VENDOR() { VENDOR_ID = vendorId }, context).Select(m => m.USER_ID);

                            listAttachment = listAttachment.Where(m => listUserVendor.Contains(m.CREATE_BY) || m.IS_VENDOR == "X").ToList();
                        }
                        else if (isCustomer)
                        {
                            var oldRoleId = param.data.ROLE_ID;
                            param.data.ROLE_ID = null;
                            param.data.IS_CUSTOMER = null;
                            listAttachment.AddRange(MapperServices.ART_WF_ARTWORK_ATTACHMENT(ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_ATTACHMENT(param.data), context)));

                            var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(artworkSubId, context);
                            var listUserCustomer = ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER() { ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID }, context).Select(m => m.CUSTOMER_USER_ID);

                            listAttachment = listAttachment.Where(m => listUserCustomer.Contains(m.CREATE_BY) || m.IS_CUSTOMER == "X").ToList();
                        }
                        else
                        {
                            if (param.data.ROLE_ID == null)
                            {
                                //tab att
                                param.data.ROLE_ID = null;

                                var item = (from i in context.ART_WF_ARTWORK_PROCESS
                                            where i.ARTWORK_SUB_ID == artworkSubId
                                            select i.ARTWORK_ITEM_ID).FirstOrDefault();

                                var sub = (from s in context.ART_WF_ARTWORK_PROCESS
                                           where s.ARTWORK_ITEM_ID == item
                                           select s.ARTWORK_SUB_ID).ToList();

                                if (sub != null && sub.Count > 0)
                                {
                                    ART_WF_ARTWORK_ATTACHMENT att = new ART_WF_ARTWORK_ATTACHMENT();
                                    foreach (int subID in sub)
                                    {
                                        att = new ART_WF_ARTWORK_ATTACHMENT();
                                        att.ARTWORK_SUB_ID = subID;
                                        att.ROLE_ID = null;
                                        listAttachment.AddRange(MapperServices.ART_WF_ARTWORK_ATTACHMENT(ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_ATTACHMENT(att), context)));
                                    }
                                }
                            }
                            else
                            {
                                //internal popup
                                var oldRoleId = param.data.ROLE_ID;
                                param.data.ROLE_ID = null;
                                param.data.IS_INTERNAL = null;
                                param.data.IS_VENDOR = null;
                                param.data.IS_CUSTOMER = null;

                                var item = (from i in context.ART_WF_ARTWORK_PROCESS
                                            where i.ARTWORK_SUB_ID == artworkSubId
                                            select i.ARTWORK_ITEM_ID).FirstOrDefault();

                                var sub = (from s in context.ART_WF_ARTWORK_PROCESS
                                           where s.ARTWORK_ITEM_ID == item
                                           select s.ARTWORK_SUB_ID).ToList();

                                if (sub != null && sub.Count > 0)
                                {
                                    ART_WF_ARTWORK_ATTACHMENT att = new ART_WF_ARTWORK_ATTACHMENT();
                                    foreach (int subID in sub)
                                    {
                                        att = new ART_WF_ARTWORK_ATTACHMENT();
                                        att.ARTWORK_SUB_ID = subID;
                                        listAttachment.AddRange(MapperServices.ART_WF_ARTWORK_ATTACHMENT(ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_ATTACHMENT(att), context)));
                                    }
                                }

                                if (isOwner)
                                    listAttachment = listAttachment.ToList();
                                else
                                    listAttachment = listAttachment.Where(m => m.IS_INTERNAL == "X").ToList();
                            }
                        }



                        //ticket464524 by aof 
                        //listAttachment = listAttachment.Where(m => m.VERSION == 1).ToList();  tikcet#438889 
                        //listAttachment = listAttachment.Where(m => m.VERSION2 == "1.0").ToList();//ticket#438889     //ticket464524
                       // this list is query distinct group by  node_id  change from where by m.VERSION =="1.0"
                        var listAttachementGroupByNode = listAttachment.GroupBy(g => g.NODE_ID, (group_node_id, attc) => new
                        {
                            attc_id = attc.Max(m => m.ARTWORK_ATTACHMENT_ID),
                            node_id = group_node_id
                        }).ToList();

                        var listAttachementID = listAttachementGroupByNode.Select(s => s.attc_id).ToList();
                        listAttachment = listAttachment.Where(m => listAttachementID.Contains(m.ARTWORK_ATTACHMENT_ID)).ToList();
                        //ticket464524  by aof

                        listAttachment = listAttachment.Where(m => CNService.FindArtworkSubId(artworkSubId, context).Contains(m.ARTWORK_SUB_ID)).ToList();

                        foreach (var itemTmp in listAttachment)
                        {
                            //if (itemTmp.CREATE_BY == createById)
                            //    itemTmp.canDelete = true;
                            itemTmp.canDownload = true;
                            itemTmp.canAddVersion = true;

                            if (isOwner) itemTmp.canDelete = true;
                            else
                            {
                              
                                if (artworkSubId == itemTmp.ARTWORK_SUB_ID)
                                {
                                    itemTmp.canDelete = true;
                                    //#INC-36800 by aof for checking if have add versions from step send_pa that is cannot delete file #start code.
                                    var tmpAttach = context.ART_WF_ARTWORK_ATTACHMENT.Where(w => w.NODE_ID == itemTmp.NODE_ID).OrderBy(o => o.ARTWORK_SUB_ID).ToList().FirstOrDefault();
                                    if (tmpAttach != null)
                                    {
                                        var tmpProcessPA = context.ART_WF_ARTWORK_PROCESS.Where(w => w.ARTWORK_SUB_ID == tmpAttach.ARTWORK_SUB_ID).ToList().FirstOrDefault();
                                        if (tmpProcessPA !=null)
                                        {
                                            if (tmpProcessPA.CURRENT_STEP_ID == SEND_PA)
                                            {
                                                itemTmp.canDelete = false;
                                            } else if(tmpProcessPA.ARTWORK_SUB_ID != artworkSubId) 
                                            {
                                                itemTmp.canDelete = false;
                                            }
                                        }
                                    }
                                    //#INC-36800 by aof check #end code.
                                }
                            }

                            var att_last_version = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(new ART_WF_ARTWORK_ATTACHMENT() { NODE_ID = itemTmp.NODE_ID }, context).OrderByDescending(m => m.CREATE_DATE).FirstOrDefault();
                            itemTmp.VERSION2 = att_last_version.VERSION2;
                            itemTmp.FILE_NAME = att_last_version.FILE_NAME;
                            itemTmp.EXTENSION = att_last_version.EXTENSION;
                            itemTmp.CREATE_BY = att_last_version.CREATE_BY;
                            itemTmp.CREATE_DATE = att_last_version.CREATE_DATE;

                            if (itemTmp.STEP_ARTWORK_ID != null)
                            {
                                itemTmp.step_code = ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(itemTmp.STEP_ARTWORK_ID, context).STEP_ARTWORK_CODE;    //#INC-36800 by aof.
                                itemTmp.step = ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(itemTmp.STEP_ARTWORK_ID, context).STEP_ARTWORK_NAME;
                                itemTmp.remark = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(itemTmp.ARTWORK_SUB_ID, context).REMARK;

                                if (itemTmp.STEP_ARTWORK_ID == ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "UPLOAD_AW" }, context).FirstOrDefault().STEP_ARTWORK_ID)
                                {
                                    itemTmp.canDelete = false;
                                }
                            }

                            if (itemTmp.CREATE_BY > 0)
                            {
                                if (CNService.IsVendor(itemTmp.CREATE_BY, context))
                                {
                                    var listUserVendor = ART_M_USER_VENDOR_SERVICE.GetByItem(new DAL.ART_M_USER_VENDOR() { USER_ID = Convert.ToInt32(itemTmp.CREATE_BY) }, context);
                                    if (listUserVendor.Count() > 0)
                                    {
                                        itemTmp.create_by_desc_txt = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(listUserVendor.FirstOrDefault().VENDOR_ID, context).VENDOR_NAME;
                                    }
                                }
                                else if (CNService.IsCustomer(itemTmp.CREATE_BY, context))
                                {
                                    var listUserCustomer = ART_M_USER_CUSTOMER_SERVICE.GetByItem(new DAL.ART_M_USER_CUSTOMER() { USER_ID = Convert.ToInt32(itemTmp.CREATE_BY) }, context);
                                    if (listUserCustomer.Count() > 0)
                                    {
                                        itemTmp.create_by_desc_txt = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(listUserCustomer.FirstOrDefault().CUSTOMER_ID, context).CUSTOMER_NAME;
                                    }
                                }
                            }

                            itemTmp.create_by_display_txt = CNService.GetUserName(itemTmp.CREATE_BY, context);
                            itemTmp.create_date_display_txt = itemTmp.CREATE_DATE;

                            itemTmp.NODE_ID_TXT = EncryptionService.EncryptAndUrlEncode(itemTmp.NODE_ID.ToString());
                        }
                    }
                }

                if (listAttachment.Count > 0)
                {
                    Results.data = listAttachment;
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

        public static ART_WF_ARTWORK_ATTACHMENT_RESULT GetAttachmentInfoFileVersion(ART_WF_ARTWORK_ATTACHMENT_REQUEST param)
        {
            ART_WF_ARTWORK_ATTACHMENT_RESULT Results = new ART_WF_ARTWORK_ATTACHMENT_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var artworkSubId = param.data.ARTWORK_SUB_ID;
                        param.data.ARTWORK_SUB_ID = 0;

                        if (param == null || param.data == null)
                        {

                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_ATTACHMENT(ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_ATTACHMENT(param.data), context));
                            Results.data = Results.data.OrderBy(m => m.VERSION).ToList();
                        }

                        var createById = param.data.currentUserId;
                        var isCustomer = CNService.IsCustomer(createById, context);
                        var isVendor = CNService.IsVendor(createById, context);

                        if (isVendor || isCustomer)
                        {
                            var max = Results.data.Select(m => m.VERSION).Max();
                            Results.data = Results.data.Where(m => m.VERSION == 1 || m.VERSION == max).ToList();
                        }

                        var isOwner = false;
                        var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var process2 = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = artworkSubId }, context);
                        var process3 = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { CURRENT_STEP_ID = SEND_PA, ARTWORK_ITEM_ID = process2.FirstOrDefault().ARTWORK_ITEM_ID }, context);
                        if (process3.FirstOrDefault().CURRENT_USER_ID == param.data.currentUserId)
                        {
                            isOwner = true;
                        }

                        foreach (var item in Results.data)
                        {
                            if (item.STEP_ARTWORK_ID != null)
                            {
                                item.step = ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(item.STEP_ARTWORK_ID, context).STEP_ARTWORK_NAME;
                                item.remark = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(item.ARTWORK_SUB_ID, context).REMARK;
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

                            //if (item.CREATE_BY == createById)
                            //    item.canDelete = true;
                            item.canDownload = true;
                            item.canAddVersion = true;

                            if (isOwner) item.canDelete = true;
                            else
                            {
                                if (artworkSubId == item.ARTWORK_SUB_ID)
                                {
                                    item.canDelete = true;
                                }
                            }

                            // #INC-36800 by aof start.
                            if (item.canDelete)
                            {
                                if (item.VERSION == 1)
                                {
                                    item.canDelete = false;
                                }

                            }
                            // #INC-36800 by aof end.

                            if (item.STEP_ARTWORK_ID != null)
                            {
                                if (item.STEP_ARTWORK_ID == 1)
                                {
                                    item.canDelete = false;
                                }
                            }

                            item.create_by_display_txt = CNService.GetUserName(item.CREATE_BY, context);
                            item.create_date_display_txt = item.CREATE_DATE;

                            item.NODE_ID_TXT = EncryptionService.EncryptAndUrlEncode(item.NODE_ID.ToString());
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

        public static ART_WF_ARTWORK_ATTACHMENT SaveAttachment(string fileName, string extension, string contentType, int artworkRequestID
                                                                   , int artworksubID, int size, long nodeID, int userID, int? roleId
                                                                   , int? version, string is_internal, string is_customer, string is_vendor
                                                                   , ARTWORKEntities context)
        {
            ART_WF_ARTWORK_ATTACHMENT attachmentData = new ART_WF_ARTWORK_ATTACHMENT();

            try
            {
                //using (var context = new ARTWORKEntities())
                //{
                //using (var dbContextTransaction = CNService.IsolationLevel(context))
                //{
                attachmentData.ARTWORK_REQUEST_ID = artworkRequestID;
                attachmentData.ARTWORK_SUB_ID = artworksubID;
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
                attachmentData.STEP_ARTWORK_ID = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(artworksubID, context).CURRENT_STEP_ID;

                ART_WF_ARTWORK_ATTACHMENT_SERVICE.SaveOrUpdate(attachmentData, context);

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

        public static ART_WF_ARTWORK_ATTACHMENT SaveAttachmentFileVersion(string fileName, string extension, string contentType, int artworkRequestID
                                                             , int artworksubID, int size, long nodeID, int userID, long version
                                                             , ARTWORKEntities context)
        {
            ART_WF_ARTWORK_ATTACHMENT attachmentData = new ART_WF_ARTWORK_ATTACHMENT();

            try
            {
                //using (var context = new ARTWORKEntities())
                //{
                //using (var dbContextTransaction = CNService.IsolationLevel(context))
                //{

                //ticket#438889
                //var att_first_version = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(new ART_WF_ARTWORK_ATTACHMENT() { NODE_ID = nodeID, VERSION = 1 }, context).FirstOrDefault(); //comement by aof
                var att_first_version = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(new ART_WF_ARTWORK_ATTACHMENT() { NODE_ID = nodeID, VERSION2 = "1.0" }, context).FirstOrDefault(); // rewrite by aof change version->version2
                //ticket#438889

                var att_last_version = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(new ART_WF_ARTWORK_ATTACHMENT() { NODE_ID = nodeID }, context).OrderByDescending(m => m.CREATE_DATE).FirstOrDefault();
                double version2 = Convert.ToDouble(att_last_version.VERSION2);

                if (CNService.IsCustomer(userID, context))
                {
                    attachmentData.VERSION2 = (Convert.ToInt32(version2) + 1).ToString() + ".0";
                }
                else
                {
                    attachmentData.VERSION2 = (version2 + 0.1).ToString();
                }

                attachmentData.ARTWORK_REQUEST_ID = artworkRequestID;
                attachmentData.ARTWORK_SUB_ID = artworksubID;
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
                attachmentData.STEP_ARTWORK_ID = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(artworksubID, context).CURRENT_STEP_ID;

                ART_WF_ARTWORK_ATTACHMENT_SERVICE.SaveOrUpdate(attachmentData, context);

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

        public static void SaveAttachmentFileVersionStampPrintMS(string fileName, string extension, string contentType, int artworkRequestID
                                                           , int artworksubID, int size, long nodeID, int userID, long version, int STEP_ARTWORK_ID, ARTWORKEntities context)
        {
            //ticket#438889 by aof
            //var att_first_version = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(new ART_WF_ARTWORK_ATTACHMENT() { NODE_ID = nodeID, VERSION = 1 }, context).FirstOrDefault();  // comment by aof
            var att_first_version = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(new ART_WF_ARTWORK_ATTACHMENT() { NODE_ID = nodeID, VERSION2 = "1.0" }, context).FirstOrDefault(); //rewrite by aof change VERSION->VERSION2
            //ticket#438889 by aof
            var att_last_version = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(new ART_WF_ARTWORK_ATTACHMENT() { NODE_ID = nodeID }, context).OrderByDescending(m => m.CREATE_DATE).FirstOrDefault();
            double version2 = Convert.ToDouble(att_last_version.VERSION2);
            var attachmentData = new ART_WF_ARTWORK_ATTACHMENT();
            if (CNService.IsCustomer(userID, context))
            {
                attachmentData.VERSION2 = (Convert.ToInt32(version2) + 1).ToString() + ".0";
            }
            else
            {
                //attachmentData.VERSION2 = (version2 + 0.1).ToString();   //ticket438889 comment by aof
                attachmentData.VERSION2 = (version2 + 0.1).ToString("#.0");   //ticket438889 rewrite by aof                                                                  
            }
            
            attachmentData.ARTWORK_REQUEST_ID = artworkRequestID;
            attachmentData.ARTWORK_SUB_ID = artworksubID;
            attachmentData.NODE_ID = nodeID;
            attachmentData.SIZE = size;  //ticket439778 by aof change first version to last version
            attachmentData.CONTENT_TYPE = att_last_version.CONTENT_TYPE; //contentType;  ticket439778 by by aof change first version to last version
            attachmentData.FILE_NAME = att_last_version.FILE_NAME; //fileName;  ticket439778 by by aof change first version to last version
            attachmentData.EXTENSION = att_last_version.EXTENSION; //extension;  ticket439778 by by aof change first version to last version
            attachmentData.CREATE_BY = userID;
            attachmentData.UPDATE_BY = userID;
            attachmentData.ROLE_ID = att_first_version.ROLE_ID;

            attachmentData.VERSION = version;
            attachmentData.IS_INTERNAL = att_first_version.IS_INTERNAL;
            attachmentData.IS_CUSTOMER = att_first_version.IS_CUSTOMER;
            attachmentData.IS_VENDOR = att_first_version.IS_VENDOR;
            attachmentData.STEP_ARTWORK_ID = STEP_ARTWORK_ID;

            ART_WF_ARTWORK_ATTACHMENT_SERVICE.SaveOrUpdate(attachmentData, context);
        }

        public static ART_WF_ARTWORK_ATTACHMENT_RESULT DeleteAttachment(ART_WF_ARTWORK_ATTACHMENT_REQUEST param)
        {
            ART_WF_ARTWORK_ATTACHMENT_RESULT Results = new ART_WF_ARTWORK_ATTACHMENT_RESULT();
            List<ART_WF_ARTWORK_ATTACHMENT_2> listAttachment = new List<ART_WF_ARTWORK_ATTACHMENT_2>();
            List<ART_WF_ARTWORK_ATTACHMENT> attachments = new List<ART_WF_ARTWORK_ATTACHMENT>();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        long node_id = 0;
                        node_id = Convert.ToInt64(param.data.NODE_ID);

                        attachments = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(new ART_WF_ARTWORK_ATTACHMENT() { NODE_ID = node_id }, context);
                        foreach (var attachment in attachments)
                        {
                            ART_WF_ARTWORK_ATTACHMENT_SERVICE.DeleteByARTWORK_ATTACHMENT_ID(attachment.ARTWORK_ATTACHMENT_ID, context);
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

        public static ART_WF_ARTWORK_ATTACHMENT_RESULT DeleteAttachmentVersion(ART_WF_ARTWORK_ATTACHMENT_REQUEST param)
        {
            ART_WF_ARTWORK_ATTACHMENT_RESULT Results = new ART_WF_ARTWORK_ATTACHMENT_RESULT();
            List<ART_WF_ARTWORK_ATTACHMENT_2> listAttachment = new List<ART_WF_ARTWORK_ATTACHMENT_2>();
            List<ART_WF_ARTWORK_ATTACHMENT> attachments = new List<ART_WF_ARTWORK_ATTACHMENT>();

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
                        attachments = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(new ART_WF_ARTWORK_ATTACHMENT() { NODE_ID = node_id, VERSION = version }, context);
                        foreach (var attachment in attachments)
                        {
                            ART_WF_ARTWORK_ATTACHMENT_SERVICE.DeleteByARTWORK_ATTACHMENT_ID(attachment.ARTWORK_ATTACHMENT_ID, context);
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

        public static ART_WF_ARTWORK_ATTACHMENT_RESULT PostVisibility(ART_WF_ARTWORK_ATTACHMENT_REQUEST param)
        {
            ART_WF_ARTWORK_ATTACHMENT_RESULT Results = new ART_WF_ARTWORK_ATTACHMENT_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        long node_id = 0;
                        node_id = Convert.ToInt64(param.data.NODE_ID);

                        var attachments = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(new ART_WF_ARTWORK_ATTACHMENT() { NODE_ID = node_id }, context);
                        foreach (var attachment in attachments)
                        {
                            attachment.IS_CUSTOMER = param.data.IS_CUSTOMER;
                            attachment.IS_INTERNAL = param.data.IS_INTERNAL;
                            attachment.IS_VENDOR = param.data.IS_VENDOR;
                            ART_WF_ARTWORK_ATTACHMENT_SERVICE.SaveOrUpdate(attachment, context);
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
