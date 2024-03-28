using BLL.Services;
using DAL;
using BLL.Helpers;
using PLL.Helpers;
using PLL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace PLL.Controllers
{
    public class FileUploadController : Controller
    {
        FilesHelper filesHelper;
        public FileUploadController()
        {
            filesHelper = new FilesHelper();
            //filesHelper = new FilesHelper(DeleteURL, DeleteType, StorageRoot, UrlBase, tempPath, serverMapPath);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Upload(int mockupSubId, int userId, int? roldId, int? version, string is_internal, string is_customer, string is_vendor)
        {
            try
            {
                var resultList = new List<ViewDataUploadFilesResult>();

                var CurrentContext = HttpContext;

                filesHelper.UploadAndShowResults(mockupSubId, userId, roldId, CurrentContext, resultList, version, is_internal, is_customer, is_vendor);
                JsonFiles files = new JsonFiles(resultList);

                bool isEmpty = !resultList.Any();
                if (isEmpty)
                {
                    return Json("Error");
                }
                else
                {
                    return Json(files);
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(userId, ex, context);
                }

                var resultList = new List<ViewDataUploadFilesResult>();
                var item = new ViewDataUploadFilesResult();
                item.error = ex.Message;
                resultList.Add(item);
                JsonFiles files = new JsonFiles(resultList);
                return Json(files);
            }
        }

        [HttpPost]
        public JsonResult Upload_Version(int mockupSubId, int userId, int nodeId)
        {
            try
            {
                var resultList = new List<ViewDataUploadFilesResult>();

                var CurrentContext = HttpContext;

                filesHelper.UploadAndShowResultsFileVersion(mockupSubId, userId, nodeId, CurrentContext, resultList);
                JsonFiles files = new JsonFiles(resultList);

                bool isEmpty = !resultList.Any();
                if (isEmpty)
                {
                    return Json("Error");
                }
                else
                {
                    return Json(files);
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(userId, ex, context);
                }

                var resultList = new List<ViewDataUploadFilesResult>();
                var item = new ViewDataUploadFilesResult();
                item.error = ex.Message;
                resultList.Add(item);
                JsonFiles files = new JsonFiles(resultList);
                return Json(files);
            }
        }


        [HttpPost]
        public JsonResult Upload_IGrid_Attachment(int SapMaterialId, int userId)
        {
            try
            {
                var resultList = new List<ViewDataUploadFilesResult>();

                var CurrentContext = HttpContext;

                filesHelper.UploadAndShowResultsIGrid(SapMaterialId, userId, CurrentContext, resultList);
                JsonFiles files = new JsonFiles(resultList);

                bool isEmpty = !resultList.Any();
                if (isEmpty)
                {
                    return Json("Error");
                }
                else
                {
                    return Json(files);
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(userId, ex, context);
                }

                var resultList = new List<ViewDataUploadFilesResult>();
                var item = new ViewDataUploadFilesResult();
                item.error = ex.Message;
                resultList.Add(item);
                JsonFiles files = new JsonFiles(resultList);
                return Json(files);
            }
        }


        [HttpPost]
        public JsonResult Upload_Artwork_Attachment(int artworkSubId, int userId, int? roldId, int? version, string is_internal, string is_customer, string is_vendor)
        {
            try
            {
                var resultList = new List<ViewDataUploadFilesResult>();

                var CurrentContext = HttpContext;

                filesHelper.UploadAndShowResultsArtwork(artworkSubId, userId, roldId, CurrentContext, resultList, version, is_internal, is_customer, is_vendor);
                JsonFiles files = new JsonFiles(resultList);

                bool isEmpty = !resultList.Any();
                if (isEmpty)
                {
                    return Json("Error");
                }
                else
                {
                    return Json(files);
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(userId, ex, context);
                }

                var resultList = new List<ViewDataUploadFilesResult>();
                var item = new ViewDataUploadFilesResult();
                item.error = ex.Message;
                resultList.Add(item);
                JsonFiles files = new JsonFiles(resultList);
                return Json(files);
            }
        }

        [HttpPost]
        public JsonResult Upload_PIC(int userId,string action)
        {
            try
            {
                var resultList = new List<ViewDataUploadFilesResult>();

                var CurrentContext = HttpContext;

                filesHelper.UploadAndShowResultsFilePIC(userId, action, CurrentContext, resultList);
                JsonFiles files = new JsonFiles(resultList);

                var validateMsg = resultList.Where(w => !String.IsNullOrEmpty(w.msg)).Select(s => s.msg).ToList();
                if (validateMsg.Count > 0)
                {
                    return Json(files);
                }
                else
                {
                    return Json(files);
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(userId, ex, context);
                }

                var resultList = new List<ViewDataUploadFilesResult>();
                var item = new ViewDataUploadFilesResult();
                item.msg = ex.Message;
                resultList.Add(item);
                JsonFiles files = new JsonFiles(resultList);
                return Json(files);
            }
        }

        [HttpPost]
        public JsonResult Upload_Artwork(int userId, int? requestFormId)
        {
            try
            {
                var resultList = new List<ViewDataUploadFilesResult>();

                var CurrentContext = HttpContext;

                filesHelper.UploadAndShowResultsFileArtwork(userId, requestFormId, CurrentContext, resultList);
                JsonFiles files = new JsonFiles(resultList);

                bool isEmpty = !resultList.Any();
                if (isEmpty)
                {
                    return Json("Error");
                }
                else
                {
                    return Json(files);
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(userId, ex, context);
                }

                var resultList = new List<ViewDataUploadFilesResult>();
                var item = new ViewDataUploadFilesResult();
                item.error = ex.Message;
                resultList.Add(item);
                JsonFiles files = new JsonFiles(resultList);
                return Json(files);
            }
        }

        private void SaveLog(int userId, Exception ex, ARTWORKEntities context)
        {
            ART_SYS_LOG model = new ART_SYS_LOG();
            model.ACTION = "E";
            model.TABLE_NAME = "Upload file [FileUploadController]";
            model.CREATE_BY = userId;
            model.UPDATE_BY = userId;
            var user = ART_M_USER_SERVICE.GetByUSER_ID(userId, context);
            model.NEW_VALUE = user.USERNAME;


            //if (ex.InnerException == null)
            //    model.ERROR_MSG = ex.Message + "<br/>" + ex.StackTrace;
            //else model.ERROR_MSG = ex.InnerException.Message + "<br/>" + ex.StackTrace;

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
        }

        [HttpPost]
        public JsonResult Upload_Artwork_Version(int artworkSubId, int userId, int nodeId)
        {
            try
            {
                var resultList = new List<ViewDataUploadFilesResult>();

                var CurrentContext = HttpContext;

                filesHelper.UploadAndShowResultsFileVersion_Artwork(artworkSubId, userId, nodeId, CurrentContext, resultList);
                JsonFiles files = new JsonFiles(resultList);

                bool isEmpty = !resultList.Any();
                if (isEmpty)
                {
                    return Json("Error");
                }
                else
                {
                    return Json(files);
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(userId, ex, context);
                }

                var resultList = new List<ViewDataUploadFilesResult>();
                var item = new ViewDataUploadFilesResult();
                item.error = ex.Message;
                resultList.Add(item);
                JsonFiles files = new JsonFiles(resultList);
                return Json(files);
            }
        }

        [HttpGet]
        public ActionResult Download(string nodeIdTxt)
        {
            try
            {
                nodeIdTxt = ConvertNodeIDText(nodeIdTxt);
                long nodeID = Convert.ToInt64(EncryptionService.Decrypt(nodeIdTxt));
                ART_WF_MOCKUP_ATTACHMENT attachment = new ART_WF_MOCKUP_ATTACHMENT();
                attachment.NODE_ID = nodeID;

                ART_WF_MOCKUP_ATTACHMENT temp;
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        temp = ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetByItem(attachment, context).OrderByDescending(m => m.VERSION).FirstOrDefault();
                    }
                }

                string contentType = temp.CONTENT_TYPE;
                string fileName = temp.FILE_NAME;
                var dataStream = filesHelper.DownloadResult(nodeID);

                if (temp.EXTENSION.ToLower() == "jpg" || temp.EXTENSION.ToLower() == "png")
                {
                    return new FileStreamResult(dataStream, contentType);
                }
                else
                {
                    return File(dataStream, contentType, fileName);
                }

            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult Download_Artwork_Attachment(string nodeIdTxt)
        {
            try
            {
                nodeIdTxt = ConvertNodeIDText(nodeIdTxt);
                long nodeID = Convert.ToInt64(EncryptionService.Decrypt(nodeIdTxt));
                ART_WF_ARTWORK_ATTACHMENT attachment = new ART_WF_ARTWORK_ATTACHMENT();
                attachment.NODE_ID = nodeID;

                ART_WF_ARTWORK_ATTACHMENT temp;
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        temp = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(attachment, context).OrderByDescending(m => m.VERSION).FirstOrDefault();
                    }
                }

                string contentType = temp.CONTENT_TYPE;
                string fileName = temp.FILE_NAME;
                var dataStream = filesHelper.DownloadResult(nodeID);

                if (temp.EXTENSION.ToLower() == "jpg" || temp.EXTENSION.ToLower() == "png")
                {
                    return new FileStreamResult(dataStream, contentType);
                }
                else
                {
                    return File(dataStream, contentType, fileName);
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult Download_Artwork(string nodeIdTxt)
        {
            try
            {
                nodeIdTxt = ConvertNodeIDText(nodeIdTxt);
                long nodeID = Convert.ToInt64(EncryptionService.Decrypt(nodeIdTxt));
                ART_WF_ARTWORK_REQUEST_ITEM fileArtwork = new ART_WF_ARTWORK_REQUEST_ITEM();
                fileArtwork.REQUEST_FORM_FILE_NODE_ID = nodeID;

                ART_WF_ARTWORK_REQUEST_ITEM tempArtwork;
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        tempArtwork = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(fileArtwork, context).FirstOrDefault();
                    }
                }

                string contentType = tempArtwork.CONTENT_TYPE;
                string fileName = tempArtwork.FILE_NAME;
                var dataStream = filesHelper.DownloadResult(nodeID);

                if (tempArtwork.EXTENSION.ToLower() == "jpg" || tempArtwork.EXTENSION.ToLower() == "png")
                {
                    return new FileStreamResult(dataStream, contentType);
                }
                else
                {
                    return File(dataStream, contentType, fileName);
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                return View("Error");
            }
        }


        [HttpGet]
        public ActionResult Download_IGrid(int tblfils_id)
        {
            try
            {

                DAL.Model.TblFiles_REQUEST param = new DAL.Model.TblFiles_REQUEST();
                param.data = new DAL.Model.TblFiles_MODEL();
                param.data.id = tblfils_id;
                var result  = IGridFormHelper.GetIGridAttachmentInfo(param);

                var tblfile = result.data.FirstOrDefault();

                string contentType = tblfile.contenttype;
                string fileName = tblfile.name;

           
                Stream dataStream = new MemoryStream(tblfile.data);

           

                if (tblfile.extension.ToLower() == "jpg" || tblfile.extension.ToLower() == "png")
                {
                    return new FileStreamResult(dataStream, contentType);
                }
                else
                {
                    return File(dataStream, contentType, fileName);
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                return View("Error");
            }
        }


        [HttpGet]
        public ActionResult Download_Artwork_Test(string nodeIdTxt)
        {
            try
            {
                nodeIdTxt = ConvertNodeIDText(nodeIdTxt);
                long nodeID = Convert.ToInt64(EncryptionService.Decrypt(nodeIdTxt));
                ART_WF_ARTWORK_REQUEST_ITEM fileArtwork = new ART_WF_ARTWORK_REQUEST_ITEM();
                fileArtwork.REQUEST_FORM_FILE_NODE_ID = nodeID;

                ART_WF_ARTWORK_REQUEST_ITEM tempArtwork;
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        tempArtwork = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(fileArtwork, context).FirstOrDefault();
                    }
                }

                string contentType = tempArtwork.CONTENT_TYPE;
                string fileName = tempArtwork.FILE_NAME;
                var dataStream = filesHelper.DownloadResult(nodeID);

              

                //if (tempArtwork.EXTENSION.ToLower() == "jpg" || tempArtwork.EXTENSION.ToLower() == "png")
                //{
                //    return new FileStreamResult(dataStream, contentType);
                //}
                //else
                //{
                //    return File(dataStream, contentType, fileName);
                //}

               // var x = FileStreamResult(dataStream, contentType);

                return new FileStreamResult(dataStream, contentType);
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult Download_Artwork_Test2(long nodeIdTxt)
        {
            try
            {
                //nodeIdTxt = ConvertNodeIDText(nodeIdTxt);
                long nodeID = nodeIdTxt; //Convert.ToInt64(EncryptionService.Decrypt(nodeIdTxt));
                //ART_WF_ARTWORK_REQUEST_ITEM fileArtwork = new ART_WF_ARTWORK_REQUEST_ITEM();
                //fileArtwork.REQUEST_FORM_FILE_NODE_ID = nodeID;

                //ART_WF_ARTWORK_REQUEST_ITEM tempArtwork;
                //using (var context = new ARTWORKEntities())
                //{
                //    using (CNService.IsolationLevel(context))
                //    {
                //        tempArtwork = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(fileArtwork, context).FirstOrDefault();
                //    }
                //}

                string contentType = "application/pdf"; //tempArtwork.CONTENT_TYPE;
                //string fileName = tempArtwork.FILE_NAME;
                var dataStream = filesHelper.DownloadResult(nodeID);



                //if (tempArtwork.EXTENSION.ToLower() == "jpg" || tempArtwork.EXTENSION.ToLower() == "png")
                //{
                //    return new FileStreamResult(dataStream, contentType);
                //}
                //else
                //{
                //    return File(dataStream, contentType, fileName);
                //}

                // var x = FileStreamResult(dataStream, contentType);

                return new FileStreamResult(dataStream, contentType);
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                return View("Error");
            }
        }




        [HttpGet]
        public ActionResult DownloadFileTemplate(long nodeID)
        {
            try
            {
                var token = CWSService.getAuthToken();

                string fileName = CWSService.getNode(nodeID, token).Name;

                string contentType = MimeMapping.GetMimeMapping(fileName);

                var dataStream = filesHelper.DownloadResult(nodeID);

                return File(dataStream, contentType, fileName);
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                return View("Error");
            }
        }

        [HttpGet]
        public FileStreamResult DownloadVersion(string nodeIdTxt, long version)
        {
            try
            {
                nodeIdTxt = ConvertNodeIDText(nodeIdTxt);
                long nodeID = Convert.ToInt64(EncryptionService.Decrypt(nodeIdTxt));
                ART_WF_MOCKUP_ATTACHMENT attachment = new ART_WF_MOCKUP_ATTACHMENT();
                attachment.NODE_ID = nodeID;
                attachment.VERSION = version;

                ART_WF_MOCKUP_ATTACHMENT temp;
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        temp = ART_WF_MOCKUP_ATTACHMENT_SERVICE.GetByItem(attachment, context).OrderByDescending(m => m.VERSION).FirstOrDefault();
                    }
                }

                string contentType = temp.CONTENT_TYPE;
                string fileName = temp.FILE_NAME;
                var dataStream = filesHelper.DownloadResult(nodeID, version);

                if (temp.EXTENSION.ToLower() == "jpg" || temp.EXTENSION.ToLower() == "png")
                {
                    return new FileStreamResult(dataStream, contentType);
                }
                else
                {
                    return File(dataStream, contentType, fileName);
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                throw ex;
            }
        }

        [HttpGet]
        public FileStreamResult DownloadArtworkVersion(string nodeIdTxt, long version)
        {
            try
            {
                nodeIdTxt = ConvertNodeIDText(nodeIdTxt);
                long nodeID = Convert.ToInt64(EncryptionService.Decrypt(nodeIdTxt));
                ART_WF_ARTWORK_ATTACHMENT attachment = new ART_WF_ARTWORK_ATTACHMENT();
                attachment.NODE_ID = nodeID;
                attachment.VERSION = version;

                ART_WF_ARTWORK_ATTACHMENT temp;
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        temp = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(attachment, context).OrderByDescending(m => m.VERSION).FirstOrDefault();
                    }
                }

                string contentType = temp.CONTENT_TYPE;
                string fileName = temp.FILE_NAME;
                var dataStream = filesHelper.DownloadResult(nodeID, version);

                if (temp.EXTENSION.ToLower() == "jpg" || temp.EXTENSION.ToLower() == "png")
                {
                    return new FileStreamResult(dataStream, contentType);
                }
                else
                {
                    return File(dataStream, contentType, fileName);
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult DownloadPO(string po)
        {
            try
            {
                int cntFiles = 0;

                var po_number = EncryptionService.Decrypt(po);

                cntFiles = filesHelper.countPOFiles(po_number);

                if (cntFiles == 1)
                {
                    var dataStreamFile = filesHelper.DownloadPO(po_number);
                  
                    if (dataStreamFile != null)
                    {
                        return File(dataStreamFile, "blank_", "TUG PO_" + po_number + ".pdf");
                    }
                    else
                    {
                        return View("NotFound");
                    }
                }
                else if (cntFiles > 1)
                {
                    var dataStreamZip = filesHelper.DownloadPOZip(po_number, ref cntFiles);
                    if (dataStreamZip != null)
                    {
                        return File(dataStreamZip, "application/zip", "PO_file_" + DateTime.Now.Date.ToString("yyyyMMdd_HHmm") + ".zip");
                    }
                    else
                    {
                        return View("NotFound");
                    }
                }
                 
               
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                return View("Error");
            }

            return View("NotFound");
        }

        public ActionResult DownloadPOByAW(string artworkNo)
        {
            try
            {
                if (!String.IsNullOrEmpty(artworkNo))
                {
                    var dataStreamFile = filesHelper.DownloadPOByAWZip(artworkNo);
                    if (dataStreamFile.Length > 0)
                    {
                        return File(dataStreamFile, "application/zip", "Confirm_PO" + DateTime.Now.ToString("dd.MM.yyyy_HH.mm") + ".zip");
                    }
                    else
                    {
                        return View("NotFound");
                    }

                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                return View("Error");
            }

            return View("NotFound");
        }

        [HttpGet]
        public ActionResult DownloadAWZip(string aw_list)
        {
            try
            {
                var dataStream = filesHelper.DownloadAW(aw_list);
                if (dataStream.Length > 0)
                {
                    return File(dataStream, "application/zip", "Artwork_file_" + DateTime.Now.Date.ToString("yyyyMMdd_HHmm") + ".zip");
                }
                else
                {
                    return View("NotFound");
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult DownloadLockMat(string mat_list)
        {
            try
            {
                var dataStream = filesHelper.DownloadAWMaterialLockReport(mat_list);
                if (dataStream.Length > 0)
                {
                    return File(dataStream, "application/zip", "Lists_status_of_packaging_material_report_" + DateTime.Now.Date.ToString("yyyyMMdd_HHmm") + ".zip");
                }
                else
                {
                    return View("NotFound");
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult DownloadArtworkVendor(string po)
        {
            try
            {
                var onefile = false;
                var filename = "";
                var po_number = EncryptionService.Decrypt(po);
                var dataStream = filesHelper.DownloadArtworkVendor(po_number, ref onefile, ref filename);
                if (dataStream.Length > 0 && onefile)
                {
                    return File(dataStream, "blank_", filename);
                }
                else if (dataStream.Length > 0)
                {
                    return File(dataStream, "application/zip", po_number + "_artwork.zip");
                }
                else
                {
                    return View("NotFound");
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult DownloadFile(string nodeIdTxt)
        {
            try
            {
                var token = CWSService.getAuthToken();
                nodeIdTxt = ConvertNodeIDText(nodeIdTxt);
                long nodeID = Convert.ToInt64(EncryptionService.Decrypt(nodeIdTxt));
                string fileName = CWSService.getNode(nodeID, token).Name;
                string contentType = MimeMapping.GetMimeMapping(fileName);
                var dataStream = filesHelper.DownloadResult(nodeID);
                return File(dataStream, contentType, fileName);
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                return View("Error");
            }
        }

        private string ConvertNodeIDText(string txt)
        {
            if (String.IsNullOrEmpty(txt))
            {
                return "";
            }

            return txt.Replace(" ", "+");
        }

        [HttpGet]
        public ActionResult DownloadArtworkPrintMaster(string workflowNo)
        {
            try
            {
                var onefile = false;
                var filename = "";
                var dataStream = filesHelper.DownloadArtworkPrintMaster(workflowNo);
                if (dataStream.Length > 0 && onefile)
                {
                    return File(dataStream, "blank_", filename);
                }
                else if (dataStream.Length > 0)
                {
                    return File(dataStream, "application/zip", workflowNo + "_printmaster.zip");
                }
                else
                {
                    return View("NotFound");
                }
            }
            catch (Exception ex)
            {
                using (var context = new ARTWORKEntities())
                {
                    SaveLog(CNService.getCurrentUser(context), ex, context);
                }
                return View("Error");
            }
        }
    }
}