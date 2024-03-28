using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using DAL;
using System.Web.Script.Serialization;
using System;
using BLL.Authentication;
using BLL.ContentService;
using BLL.DocumentManagement;
using System.IO;
using System.Configuration;
using System.Threading;

namespace BLL.Services
{
    public class CWSService
    {
        public static Node createFolder(long parentID, string name, string comment, string token)
        {
            name = name.Trim();
            DocumentManagementClient docClient = new DocumentManagementClient();

            try
            {
                DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
                docOTAuthen.AuthenticationToken = token;
                Node nodeCheck = docClient.GetNodeByName(ref docOTAuthen, parentID, name);
                if (nodeCheck == null)
                    return docClient.CreateFolder(ref docOTAuthen, parentID, name, comment, null);
             
                else
                    return nodeCheck;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docClient.Close();
            }
        }

        public static Node renameFolder(long nodeID, string newName, string token)
        {
            newName = newName.Trim();
            DocumentManagementClient docClient = new DocumentManagementClient();

            try
            {
                DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
                docOTAuthen.AuthenticationToken = token;
                Node nodeCheck = docClient.GetNode(ref docOTAuthen, nodeID);
                if (nodeCheck != null)
                    docClient.RenameNode(ref docOTAuthen, nodeID, newName);

                return docClient.GetNode(ref docOTAuthen, nodeID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docClient.Close();
            }
        }

        //public static Node uploadFile(Stream stream, string fileName, long parentId, int userID)
        //{
        //    fileName = fileName.Trim();
        //    ContentServiceClient contentServiceClient = new ContentServiceClient();
        //    DocumentManagementClient docManClient = new DocumentManagementClient();

        //    try
        //    {
        //        bool loop = true;
        //        int i = 1;
        //        string oriName = fileName;
        //        while (loop)
        //        {
        //            var nodeChk = getNodeByName(parentId, fileName);
        //            if (nodeChk == null)
        //                loop = false;
        //            else
        //            {
        //                string fnWithoutExtension = Path.GetFileNameWithoutExtension(oriName);
        //                string extension = Path.GetExtension(fileName);
        //                fileName = fnWithoutExtension + " (" + i.ToString() + ")" + extension;
        //                i++;
        //            }
        //        }

        //        string Token = getAuthToken(userID);
        //        DocumentManagement.OTAuthentication otAuth = new DocumentManagement.OTAuthentication();
        //        otAuth.AuthenticationToken = Token;

        //        string contextID = docManClient.CreateDocumentContext(ref otAuth, parentId, fileName, null, false, null);

        //        FileAtts fileAtts = new FileAtts();
        //        fileAtts.CreatedDate = DateTime.Now;
        //        fileAtts.FileName = fileName;
        //        fileAtts.FileSize = stream.Length;
        //        fileAtts.ModifiedDate = DateTime.Now;

        //        ContentService.OTAuthentication otAuth2 = new ContentService.OTAuthentication();
        //        otAuth2.AuthenticationToken = Token;
        //        contentServiceClient.UploadContent(ref otAuth2, contextID, fileAtts, stream);

        //        return getNodeByName(parentId, fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        docManClient.Close();
        //        contentServiceClient.Close();
        //    }

        //}

        public static Node uploadFile(Stream stream, string fileName, long parentId, string token)
        {
            fileName = fileName.Trim();
            ContentServiceClient contentServiceClient = new ContentServiceClient();
            DocumentManagementClient docManClient = new DocumentManagementClient();

            try
            {
                bool loop = true;
                int i = 1;
                string oriName = fileName;
                while (loop)
                {
                    var nodeChk = getNodeByName(parentId, fileName, token);
                    if (nodeChk == null)
                        loop = false;
                    else
                    {
                        string fnWithoutExtension = Path.GetFileNameWithoutExtension(oriName);
                        string extension = Path.GetExtension(fileName);
                        fileName = fnWithoutExtension + " (" + i.ToString() + ")" + extension;
                        i++;
                    }
                }

                string Token = token;
                DocumentManagement.OTAuthentication otAuth = new DocumentManagement.OTAuthentication();
                otAuth.AuthenticationToken = Token;

                string contextID = docManClient.CreateDocumentContext(ref otAuth, parentId, fileName, null, false, null);

                FileAtts fileAtts = new FileAtts();
                fileAtts.CreatedDate = DateTime.Now;
                fileAtts.FileName = fileName;
                fileAtts.FileSize = stream.Length;
                fileAtts.ModifiedDate = DateTime.Now;

                ContentService.OTAuthentication otAuth2 = new ContentService.OTAuthentication();
                otAuth2.AuthenticationToken = Token;
                contentServiceClient.UploadContent(ref otAuth2, contextID, fileAtts, stream);

                return getNodeByName(parentId, fileName, token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docManClient.Close();
                contentServiceClient.Close();
            }

        }

        //public static BLL.DocumentManagement.Version addVersionFile(Stream stream, string fileName, long parentId, long nodeId, int userID)
        //{
        //    fileName = fileName.Trim();
        //    ContentServiceClient contentServiceClient = new ContentServiceClient();
        //    DocumentManagementClient docManClient = new DocumentManagementClient();

        //    try
        //    {
        //        bool loop = true;
        //        int i = 1;
        //        string oriName = fileName;
        //        while (loop)
        //        {
        //            var nodeChk = getNodeByName(parentId, fileName);
        //            if (nodeChk == null)
        //                loop = false;
        //            else
        //            {
        //                string fnWithoutExtension = Path.GetFileNameWithoutExtension(oriName);
        //                string extension = Path.GetExtension(fileName);
        //                fileName = fnWithoutExtension + " (" + i.ToString() + ")" + extension;
        //                i++;
        //            }
        //        }

        //        string Token = getAuthToken(userID);
        //        DocumentManagement.OTAuthentication otAuth = new DocumentManagement.OTAuthentication();
        //        otAuth.AuthenticationToken = Token;

        //        string contextID = docManClient.AddVersionContext(ref otAuth, nodeId, null);

        //        FileAtts fileAtts = new FileAtts();
        //        fileAtts.CreatedDate = DateTime.Now;
        //        fileAtts.FileName = fileName;
        //        fileAtts.FileSize = stream.Length;
        //        fileAtts.ModifiedDate = DateTime.Now;

        //        ContentService.OTAuthentication otAuth2 = new ContentService.OTAuthentication();
        //        otAuth2.AuthenticationToken = Token;
        //        contentServiceClient.UploadContent(ref otAuth2, contextID, fileAtts, stream);

        //        var maxVersion = getNode(nodeId).VersionInfo.VersionNum;
        //        return docManClient.GetVersion(ref otAuth, nodeId, maxVersion);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        docManClient.Close();
        //        contentServiceClient.Close();
        //    }
        //}

        public static BLL.DocumentManagement.Version addVersionFile(Stream stream, string fileName, long parentId, long nodeId, string token)
        {
            fileName = fileName.Trim();
            ContentServiceClient contentServiceClient = new ContentServiceClient();
            DocumentManagementClient docManClient = new DocumentManagementClient();

            try
            {
                //bool loop = true;
                //int i = 1;
                //string oriName = fileName;
                //while (loop)
                //{
                //    var nodeChk = getNodeByName(parentId, fileName, token);
                //    if (nodeChk == null)
                //        loop = false;
                //    else
                //    {
                //        string fnWithoutExtension = Path.GetFileNameWithoutExtension(oriName);
                //        string extension = Path.GetExtension(fileName);
                //        fileName = fnWithoutExtension + " (" + i.ToString() + ")" + extension;
                //        i++;
                //    }
                //}

                string Token = token;
                DocumentManagement.OTAuthentication otAuth = new DocumentManagement.OTAuthentication();
                otAuth.AuthenticationToken = Token;

                string contextID = docManClient.AddVersionContext(ref otAuth, nodeId, null);

                FileAtts fileAtts = new FileAtts();
                fileAtts.CreatedDate = DateTime.Now;
                fileAtts.FileName = fileName;
                fileAtts.FileSize = stream.Length;
                fileAtts.ModifiedDate = DateTime.Now;

                ContentService.OTAuthentication otAuth2 = new ContentService.OTAuthentication();
                otAuth2.AuthenticationToken = Token;
                contentServiceClient.UploadContent(ref otAuth2, contextID, fileAtts, stream);

                var maxVersion = getNode(nodeId, token).VersionInfo.VersionNum;
                return docManClient.GetVersion(ref otAuth, nodeId, maxVersion);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docManClient.Close();
                contentServiceClient.Close();
            }
        }
        //public static BLL.DocumentManagement.Version addVersionFile(string filePath, string fileName, long parentId, long nodeId, int userID, string comment)
        //{
        //    Stream stream = new FileStream(filePath, FileMode.Open);

        //    fileName = fileName.Trim();
        //    ContentServiceClient contentServiceClient = new ContentServiceClient();
        //    DocumentManagementClient docManClient = new DocumentManagementClient();

        //    try
        //    {
        //        bool loop = true;
        //        int i = 1;
        //        string oriName = fileName;
        //        while (loop)
        //        {
        //            var nodeChk = getNodeByName(parentId, fileName);
        //            if (nodeChk == null)
        //                loop = false;
        //            else
        //            {
        //                string fnWithoutExtension = Path.GetFileNameWithoutExtension(oriName);
        //                string extension = Path.GetExtension(fileName);
        //                fileName = fnWithoutExtension + " (" + i.ToString() + ")" + extension;
        //                i++;
        //            }
        //        }

        //        string Token = getAuthToken(userID);
        //        DocumentManagement.OTAuthentication otAuth = new DocumentManagement.OTAuthentication();
        //        otAuth.AuthenticationToken = Token;

        //        string contextID = docManClient.AddVersionContext(ref otAuth, nodeId, null);

        //        FileAtts fileAtts = new FileAtts();
        //        fileAtts.CreatedDate = DateTime.Now;
        //        fileAtts.FileName = fileName;
        //        fileAtts.FileSize = stream.Length;
        //        fileAtts.ModifiedDate = DateTime.Now;

        //        ContentService.OTAuthentication otAuth2 = new ContentService.OTAuthentication();
        //        otAuth2.AuthenticationToken = Token;
        //        contentServiceClient.UploadContent(ref otAuth2, contextID, fileAtts, stream);

        //        var maxVersion = getNode(nodeId).VersionInfo.VersionNum;

        //        if (!string.IsNullOrEmpty(comment))
        //        {
        //            var tempNode = docManClient.GetVersion(ref otAuth, nodeId, maxVersion);
        //            tempNode.Comment = comment;
        //            docManClient.UpdateVersion(ref otAuth, tempNode);
        //        }

        //        return docManClient.GetVersion(ref otAuth, nodeId, maxVersion);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (docManClient != null) docManClient.Close();
        //        if (contentServiceClient != null) contentServiceClient.Close();
        //        if (stream != null) stream.Close(); stream.Dispose();
        //        try
        //        {
        //            if (File.Exists(filePath))
        //            {
        //                File.Delete(filePath);
        //            }
        //        }
        //        catch (Exception ex) { CNService.GetErrorMessage(ex); }
        //    }
        //}

        public static BLL.DocumentManagement.Version addVersionFile(string filePath, string fileName, long parentId, long nodeId, string comment, string token)
        {
            Stream stream = new FileStream(filePath, FileMode.Open);

            fileName = fileName.Trim();
            ContentServiceClient contentServiceClient = new ContentServiceClient();
            DocumentManagementClient docManClient = new DocumentManagementClient();

            try
            {
                //bool loop = true;
                //int i = 1;
                //string oriName = fileName;
                //while (loop)
                //{
                //    var nodeChk = getNodeByName(parentId, fileName, token);
                //    if (nodeChk == null)
                //        loop = false;
                //    else
                //    {
                //        string fnWithoutExtension = Path.GetFileNameWithoutExtension(oriName);
                //        string extension = Path.GetExtension(fileName);
                //        fileName = fnWithoutExtension + " (" + i.ToString() + ")" + extension;
                //        i++;
                //    }
                //}

                string Token = token;
                DocumentManagement.OTAuthentication otAuth = new DocumentManagement.OTAuthentication();
                otAuth.AuthenticationToken = Token;

                string contextID = docManClient.AddVersionContext(ref otAuth, nodeId, null);

                FileAtts fileAtts = new FileAtts();
                fileAtts.CreatedDate = DateTime.Now;
                fileAtts.FileName = fileName;
                fileAtts.FileSize = stream.Length;
                fileAtts.ModifiedDate = DateTime.Now;

                ContentService.OTAuthentication otAuth2 = new ContentService.OTAuthentication();
                otAuth2.AuthenticationToken = Token;
                contentServiceClient.UploadContent(ref otAuth2, contextID, fileAtts, stream);

                var maxVersion = getNode(nodeId, token).VersionInfo.VersionNum;

                if (!string.IsNullOrEmpty(comment))
                {
                    var tempNode = docManClient.GetVersion(ref otAuth, nodeId, maxVersion);
                    tempNode.Comment = comment;
                    docManClient.UpdateVersion(ref otAuth, tempNode);
                }

                return docManClient.GetVersion(ref otAuth, nodeId, maxVersion);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (docManClient != null) docManClient.Close();
                if (contentServiceClient != null) contentServiceClient.Close();
                if (stream != null) stream.Close(); stream.Dispose();
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                catch (Exception ex) { CNService.GetErrorMessage(ex); }
            }
        }

        //public static Stream downloadFile(long nodeID)
        //{
        //    DocumentManagementClient docManClient = new DocumentManagementClient();

        //    DocumentManagement.OTAuthentication otAuth = new DocumentManagement.OTAuthentication();
        //    otAuth.AuthenticationToken = getAuthToken();
        //    string contextID = null;

        //    try
        //    {
        //        contextID = docManClient.GetVersionContentsContext(ref otAuth, nodeID, 0);
        //    }
        //    catch
        //    {

        //    }
        //    finally
        //    {
        //        docManClient.Close();
        //    }

        //    ContentService.OTAuthentication otAuth2 = new ContentService.OTAuthentication();
        //    otAuth2.AuthenticationToken = otAuth.AuthenticationToken;
        //    ContentServiceClient contentServiceClient = new ContentServiceClient();
        //    Stream downloadStream = null;

        //    try
        //    {
        //        downloadStream = contentServiceClient.DownloadContent(ref otAuth2, contextID);
        //    }
        //    finally
        //    {
        //        contentServiceClient.Close();
        //    }
        //    return downloadStream;
        //}

        public static Stream downloadFile(long nodeID, string token)
        {
            DocumentManagementClient docManClient = new DocumentManagementClient();

            DocumentManagement.OTAuthentication otAuth = new DocumentManagement.OTAuthentication();
            otAuth.AuthenticationToken = token;
            string contextID = null;

            try
            {
                contextID = docManClient.GetVersionContentsContext(ref otAuth, nodeID, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docManClient.Close();
            }

            ContentService.OTAuthentication otAuth2 = new ContentService.OTAuthentication();
            otAuth2.AuthenticationToken = otAuth.AuthenticationToken;
            ContentServiceClient contentServiceClient = new ContentServiceClient();
            Stream downloadStream = null;

            try
            {
                downloadStream = contentServiceClient.DownloadContent(ref otAuth2, contextID);
            }
            finally
            {
                contentServiceClient.Close();
            }
            return downloadStream;
        }

        public static Stream downloadFile(long nodeID, long version, string token)
        {
            DocumentManagementClient docManClient = new DocumentManagementClient();

            DocumentManagement.OTAuthentication otAuth = new DocumentManagement.OTAuthentication();
            otAuth.AuthenticationToken = token;
            string contextID = null;

            try
            {
                contextID = docManClient.GetVersionContentsContext(ref otAuth, nodeID, version);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docManClient.Close();
            }

            ContentService.OTAuthentication otAuth2 = new ContentService.OTAuthentication();
            otAuth2.AuthenticationToken = otAuth.AuthenticationToken;
            ContentServiceClient contentServiceClient = new ContentServiceClient();
            Stream downloadStream = null;

            try
            {
                downloadStream = contentServiceClient.DownloadContent(ref otAuth2, contextID);
            }
            finally
            {
                contentServiceClient.Close();
            }
            return downloadStream;
        }

        //public static string getAuthToken()
        //{
        //    string username = ConfigurationManager.AppSettings["OTUSERNAME"];
        //    string password = ConfigurationManager.AppSettings["OTPASSWORD"];

        //    AuthenticationClient authClient = new AuthenticationClient();

        //    try
        //    {
        //        return authClient.AuthenticateUser(username, password);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        authClient.Close();
        //    }
        //}

        public static string getAuthToken()
        {
            string username = ConfigurationManager.AppSettings["OTUSERNAME"];
            string password = ConfigurationManager.AppSettings["OTPASSWORD"];
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    int userID = CNService.getCurrentUser(context);
                    if (userID > 0)
                    {
                        if (CNService.IsVendor(userID, context))
                        {
                            username = ConfigurationManager.AppSettings["OTUSERNAME_Vendor"];
                            password = ConfigurationManager.AppSettings["OTPASSWORD_Vendor"];
                        }
                        else if (CNService.IsCustomer(userID, context))
                        {
                            username = ConfigurationManager.AppSettings["OTUSERNAME_Customer"];
                            password = ConfigurationManager.AppSettings["OTPASSWORD_Customer"];
                        }
                        else
                        {
                            username = ConfigurationManager.AppSettings["OTUSERNAME_Internal"];
                            password = ConfigurationManager.AppSettings["OTPASSWORD_Internal"];
                        }
                    }
                }
            }

            AuthenticationClient authClient = new AuthenticationClient();

            try
            {
                return authClient.AuthenticateUser(username, password);
            }
            catch
            {
                try
                {
                    return authClient.AuthenticateUser(username, password);
                }
                catch
                {
                    try
                    {
                        return authClient.AuthenticateUser(username, password);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            finally
            {
                authClient.Close();
            }
        }

        //private static string getAuthToken(string username, string password)
        //{
        //    AuthenticationClient authClient = new AuthenticationClient();

        //    try
        //    {
        //        return authClient.AuthenticateUser(username, password);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        authClient.Close();
        //    }
        //}

        //public static Node getNodeByName(long parentID, string name)
        //{
        //    name = name.Trim();
        //    DocumentManagementClient docClient = new DocumentManagementClient();

        //    try
        //    {
        //        DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
        //        docOTAuthen.AuthenticationToken = getAuthToken();
        //        return docClient.GetNodeByName(ref docOTAuthen, parentID, name);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        docClient.Close();
        //    }
        //}

        public static Node getNodeByName(long parentID, string name, string token)
        {
            name = name.Trim();
            DocumentManagementClient docClient = new DocumentManagementClient();

            try
            {
                DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
                docOTAuthen.AuthenticationToken = token;
                return docClient.GetNodeByName(ref docOTAuthen, parentID, name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docClient.Close();
            }
        }

        //public static void deleteNode(long nodeID, int userID)
        //{
        //    DocumentManagementClient docClient = new DocumentManagementClient();

        //    try
        //    {
        //        DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
        //        docOTAuthen.AuthenticationToken = getAuthToken(userID);
        //        docClient.DeleteNode(ref docOTAuthen, nodeID);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        docClient.Close();
        //    }
        //}

        public static void deleteNode(long nodeID, string token)
        {
            DocumentManagementClient docClient = new DocumentManagementClient();

            try
            {
                DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
                docOTAuthen.AuthenticationToken = token;
                docClient.DeleteNode(ref docOTAuthen, nodeID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docClient.Close();
            }
        }

        public static void deleteNodeVersion(long nodeID, long version, string token)
        {
            DocumentManagementClient docClient = new DocumentManagementClient();

            try
            {
                DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
                docOTAuthen.AuthenticationToken = token;
                docClient.DeleteVersion(ref docOTAuthen, nodeID, version);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docClient.Close();
            }
        }

        //public static Node[] getAllNodeInFolder(long parentId)
        //{
        //    DocumentManagementClient docClient = new DocumentManagementClient();

        //    try
        //    {
        //        DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
        //        docOTAuthen.AuthenticationToken = getAuthToken();
        //        return docClient.ListNodes(ref docOTAuthen, parentId, true);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        docClient.Close();
        //    }
        //}

        public static Node[] getAllNodeInFolder(long parentId, string token)
        {
            DocumentManagementClient docClient = new DocumentManagementClient();

            try
            {
                DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
                docOTAuthen.AuthenticationToken = token;
                return docClient.ListNodes(ref docOTAuthen, parentId, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docClient.Close();
            }
        }

        //public static Node copyNode(string toNodeName, long copyNodeId, long toNodeId, int userID)
        //{
        //    DocumentManagementClient docClient = new DocumentManagementClient();

        //    try
        //    {
        //        Node node = getNodeByName(toNodeId, toNodeName);
        //        if (node == null)
        //        {
        //            DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
        //            docOTAuthen.AuthenticationToken = getAuthToken(userID);
        //            return docClient.CopyNode(ref docOTAuthen, copyNodeId, toNodeId, toNodeName, new CopyOptions() { AttrSourceType = AttributeSourceType.DESTINATION });
        //        }
        //        return node;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        docClient.Close();
        //    }
        //}

        public static Node copyNodeAndDeleteNodeIsExist(string toNodeName, long copyNodeId, long toNodeId, string token)
        {
            // ticket 459570 by aof
            DocumentManagementClient docClient = new DocumentManagementClient();

            try
            {
                Node node = getNodeByName(toNodeId, toNodeName, token);

            
                if (node !=null)
                {
                   //check if node is exist. it will be deleted.
                   CWSService.deleteNode(node.ID, token);
                }

                DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
                docOTAuthen.AuthenticationToken = token;
                return docClient.CopyNode(ref docOTAuthen, copyNodeId, toNodeId, toNodeName, new CopyOptions() { AttrSourceType = AttributeSourceType.DESTINATION });

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docClient.Close();
            }
        }

        public static Node copyNode(string toNodeName, long copyNodeId, long toNodeId, string token)
        {
            DocumentManagementClient docClient = new DocumentManagementClient();

            try
            {
                Node node = getNodeByName(toNodeId, toNodeName, token);
                if (node == null)
                {
                    DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
                    docOTAuthen.AuthenticationToken = token;
                    return docClient.CopyNode(ref docOTAuthen, copyNodeId, toNodeId, toNodeName, new CopyOptions() { AttrSourceType = AttributeSourceType.DESTINATION });
                }
                else
                {
                    return node;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docClient.Close();
            }
        }

        public static Node copyNodeToWorkspace(string toNodeName, long copyNodeId, long toNodeId, string mat5, string token)
        {
            DocumentManagementClient docClient = new DocumentManagementClient();

            try
            {
                Node node = getNodeByName(toNodeId, toNodeName, token);
                if (node != null)
                {
                    CWSService.deleteNode(node.ID, token);
                }

                DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
                docOTAuthen.AuthenticationToken = token;
                node = docClient.CopyNode(ref docOTAuthen, copyNodeId, toNodeId, toNodeName, new CopyOptions() { AttrSourceType = AttributeSourceType.DESTINATION });

                var changePoint = true;
                if (CNService.SubString(mat5, 9).EndsWith("N"))
                {
                    changePoint = false;
                }
                else if (CNService.SubString(mat5, 9).EndsWith("C"))
                {
                    changePoint = true;
                }

                if (changePoint)
                {
                     var minVer = node.VersionInfo.Versions.Select(s => s.VerMinor).OrderBy(o => o).FirstOrDefault();
                    foreach (var version in node.VersionInfo.Versions)
                    { 
                        if (version.VerMinor > minVer)
                            docClient.DeleteVersion(ref docOTAuthen, node.ID, version.VerMinor);
                    }
                }
                else
                {
                   
                    foreach (var version in node.VersionInfo.Versions)
                    {
                        if (version.Comment == "StampInformation")
                            docClient.DeleteVersion(ref docOTAuthen, node.ID, version.VerMinor);
                    }
                }

                return node;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docClient.Close();
            }
        }

        //public static Node getNode(long nodeId)
        //{
        //    DocumentManagementClient docClient = new DocumentManagementClient();

        //    try
        //    {
        //        DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
        //        docOTAuthen.AuthenticationToken = getAuthToken();
        //        return docClient.GetNode(ref docOTAuthen, nodeId);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        docClient.Close();
        //    }
        //}

        public static Node getNode(long nodeId, string token)
        {
            DocumentManagementClient docClient = new DocumentManagementClient();

            try
            {
                DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
                docOTAuthen.AuthenticationToken = token;
                return docClient.GetNode(ref docOTAuthen, nodeId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docClient.Close();
            }
        }


        // by aof #INC-89321
        public static DocumentManagement.Version getVersion(long nodeId, string token,long version)
        {
            DocumentManagementClient docClient = new DocumentManagementClient();

            try
            {
                DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
                docOTAuthen.AuthenticationToken = token;
                return docClient.GetVersion(ref docOTAuthen, nodeId, version);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docClient.Close();
            }
        }
        // by aof #INC-89321

        //public static BLL.DocumentManagement.Version getNodeVersion(long nodeId, long version)
        //{
        //    DocumentManagementClient docClient = new DocumentManagementClient();

        //    try
        //    {
        //        DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
        //        docOTAuthen.AuthenticationToken = getAuthToken();
        //        return docClient.GetVersion(ref docOTAuthen, nodeId, version);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    finally
        //    {
        //        docClient.Close();
        //    }
        //}

        public static void updateCategoryMockup(string mockupNo, int MOCKUP_ID, string token)
        {
            string msg_error = "";

            Thread email = new Thread(delegate ()
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            long folderID = Convert.ToInt64(ConfigurationManager.AppSettings["MockUpNodeID"]);
                            var node = CWSService.getNodeByName(folderID, mockupNo, token);
                            var cat = CWSService.setCategory(MOCKUP_ID, context, token);
                            CWSService.updateCategory(node.ID, cat, token);
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg_error = CNService.GetErrorMessage(ex);
                }
                finally
                {

                }
            });

            email.IsBackground = true;
            email.Start();
        }

        public static void updateCategoryArtwork(string ARTWORK_REQUEST_NO, string REQUEST_ITEM_NO, int ARTWORK_REQUEST_ID, int ARTWORK_ITEM_ID, string token)
        {
            string msg_error = "";

            Thread email = new Thread(delegate ()
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            long ArtworkNodeID = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkNodeID"]);
                            long ArtworkRequestFormNodeID = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkRequestFormNodeID"]);

                            var nodeRequest = CWSService.getNodeByName(ArtworkRequestFormNodeID, ARTWORK_REQUEST_NO, token);
                            var nodeArwork = CWSService.getNodeByName(ArtworkNodeID, REQUEST_ITEM_NO, token);

                            var catRequest = CWSService.setCategoryArtwork(ARTWORK_REQUEST_ID, context, 0, token);
                            var catArtwork = CWSService.setCategoryArtwork(ARTWORK_REQUEST_ID, context, ARTWORK_ITEM_ID, token);

                            if (nodeRequest != null && catRequest != null && nodeArwork != null && catArtwork != null)
                            {
                                CWSService.updateCategory(nodeRequest.ID, catRequest, token);
                                CWSService.updateCategory(nodeArwork.ID, catArtwork, token);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg_error = CNService.GetErrorMessage(ex);
                }
                finally
                {

                }
            });

            email.IsBackground = true;
            email.Start();
        }

        public static Metadata setCategory(int mockUpID, ARTWORKEntities context, string token)
        {
            var mockup = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(mockUpID, context);
            var checkList = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(mockup.CHECK_LIST_ID, context);

            long cateTemplateID = Convert.ToInt64(ConfigurationManager.AppSettings["MockUpCattTemplateNodeID"]);

            Metadata inputCategory = new Metadata();

            DocumentManagementClient docClient = new DocumentManagementClient();
            DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
            docOTAuthen.AuthenticationToken = token;

            AttributeGroup cateTemplate = docClient.GetCategoryTemplate(ref docOTAuthen, cateTemplateID);

            (cateTemplate.Values[0] as StringValue).Values = new string[] { CNService.SubString(mockup.MOCKUP_NO, 200) };
            (cateTemplate.Values[1] as StringValue).Values = new string[] { CNService.SubString(checkList.CHECK_LIST_NO, 200) };
            if (checkList.COMPANY_ID > 0)
            {
                var temp = SAP_M_COMPANY_SERVICE.GetByCOMPANY_ID(checkList.COMPANY_ID, context);
                if (temp != null)
                {
                    (cateTemplate.Values[2] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                }
            }
            if (checkList.SOLD_TO_ID > 0)
            {
                var temp = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(checkList.SOLD_TO_ID, context);
                if (temp != null)
                {
                    (cateTemplate.Values[3] as StringValue).Values = new string[] { CNService.SubString(temp.CUSTOMER_NAME, 200) };
                }
            }

            if (1 == 1)
            {
                var list = ART_WF_MOCKUP_CHECK_LIST_COUNTRY_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_COUNTRY() { CHECK_LIST_ID = checkList.CHECK_LIST_ID }, context);
                string[] arr = new string[list.Count];
                int i = 0;
                foreach (var s in list)
                {
                    var temp = SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(s.COUNTRY_ID, context);
                    if (temp != null)
                    {
                        arr[i] = CNService.SubString(temp.NAME, 200);
                        i++;
                    }
                }
                StringValue submitPer = cateTemplate.Values[4] as StringValue;
                submitPer.Values = arr;
            }

            if (checkList.BRAND_ID > 0)
            {
                var temp = SAP_M_BRAND_SERVICE.GetByBRAND_ID(checkList.BRAND_ID, context);
                if (temp != null)
                {
                    (cateTemplate.Values[5] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                }
            }
            else if (checkList.BRAND_ID == -1)
                (cateTemplate.Values[5] as StringValue).Values = new string[] { CNService.SubString(checkList.BRAND_OTHER, 200) };


            if (1 == 1)
            {
                var list = ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_PRODUCT() { CHECK_LIST_ID = checkList.CHECK_LIST_ID }, context);
                string[] arr = new string[list.Count];
                int i = 0;
                foreach (var item in list)
                {
                    arr[i] = CNService.SubString(item.PRODUCT_CODE, 200);
                    i++;
                }
                StringValue submitPer = cateTemplate.Values[6] as StringValue;
                submitPer.Values = arr;
            }

            if (1 == 1)
            {
                var list = ART_WF_MOCKUP_CHECK_LIST_REFERENCE_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_REFERENCE() { CHECK_LIST_ID = checkList.CHECK_LIST_ID }, context);
                string[] arr = new string[list.Count];
                int i = 0;
                foreach (var item in list)
                {
                    arr[i] = CNService.SubString(item.REFERENCE_NO, 200);
                    i++;
                }
                StringValue submitPer = cateTemplate.Values[7] as StringValue;
                submitPer.Values = arr;
            }

            if (checkList.PRIMARY_TYPE_ID > 0)
            {
                var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(checkList.PRIMARY_TYPE_ID, context);
                if (temp != null)
                {
                    (cateTemplate.Values[8] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                }
            }
            else if (checkList.PRIMARY_TYPE_ID == -1)
                (cateTemplate.Values[8] as StringValue).Values = new string[] { CNService.SubString(checkList.PRIMARY_TYPE_OTHER, 200) };


            if (1 == 1)
            {
                var listProduct = ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_PRODUCT() { CHECK_LIST_ID = checkList.CHECK_LIST_ID }, context);
                var listRD = ART_WF_MOCKUP_CHECK_LIST_REFERENCE_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_REFERENCE() { CHECK_LIST_ID = checkList.CHECK_LIST_ID }, context);

                if (listProduct.Count > 0)
                {
                    (cateTemplate.Values[9] as StringValue).Values = new string[] { CNService.SubString(listProduct[0].PRIMARY_SIZE, 200) };
                    (cateTemplate.Values[10] as StringValue).Values = new string[] { CNService.SubString(listProduct[0].CONTAINER_TYPE, 200) };
                    (cateTemplate.Values[11] as StringValue).Values = new string[] { CNService.SubString(listProduct[0].LID_TYPE, 200) };

                    (cateTemplate.Values[14] as StringValue).Values = new string[] { CNService.SubString(listProduct[0].PACKING_STYLE, 200) };
                    (cateTemplate.Values[15] as StringValue).Values = new string[] { CNService.SubString(listProduct[0].PACK_SIZE, 200) };
                }
                else
                {
                    if (checkList.THREE_P_ID > 0)
                    {
                        var temp = SAP_M_3P_SERVICE.GetByTHREE_P_ID(checkList.THREE_P_ID, context);
                        if (temp != null)
                        {
                            (cateTemplate.Values[9] as StringValue).Values = new string[] { CNService.SubString(temp.PRIMARY_SIZE_VALUE, 200) };
                            (cateTemplate.Values[10] as StringValue).Values = new string[] { CNService.SubString(temp.CONTAINER_TYPE_VALUE, 200) };
                            (cateTemplate.Values[11] as StringValue).Values = new string[] { CNService.SubString(temp.LID_TYPE_VALUE, 200) };
                        }
                    }
                    else
                    {
                        if (checkList.THREE_P_ID == -1)
                            (cateTemplate.Values[9] as StringValue).Values = new string[] { CNService.SubString(checkList.PRIMARY_SIZE_OTHER, 200) };

                        if (checkList.CONTAINER_TYPE_ID == -1)
                            (cateTemplate.Values[10] as StringValue).Values = new string[] { CNService.SubString(checkList.CONTAINER_TYPE_OTHER, 200) };
                        else
                        {
                            var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(checkList.CONTAINER_TYPE_ID, context);
                            if (temp != null)
                            {
                                (cateTemplate.Values[10] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                            }
                        }

                        if (checkList.LID_TYPE_ID == -1)
                            (cateTemplate.Values[11] as StringValue).Values = new string[] { CNService.SubString(checkList.LID_TYPE_OTHER, 200) };
                        else
                        {
                            var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(checkList.LID_TYPE_ID, context);
                            if (temp != null)
                            {
                                (cateTemplate.Values[11] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                            }
                        }
                    }

                    if (checkList.TWO_P_ID > 0)
                    {
                        var temp = SAP_M_2P_SERVICE.GetByTWO_P_ID(checkList.TWO_P_ID, context);
                        if (temp != null)
                        {
                            (cateTemplate.Values[14] as StringValue).Values = new string[] { CNService.SubString(temp.PACKING_SYLE_VALUE, 200) };
                            (cateTemplate.Values[15] as StringValue).Values = new string[] { CNService.SubString(temp.PACK_SIZE_VALUE, 200) };
                        }
                    }
                    else
                    {
                        if (checkList.TWO_P_ID == -1)
                            (cateTemplate.Values[14] as StringValue).Values = new string[] { CNService.SubString(checkList.PACKING_STYLE_OTHER, 200) };

                        if (checkList.PACK_SIZE_ID == -1)
                            (cateTemplate.Values[15] as StringValue).Values = new string[] { CNService.SubString(checkList.PACK_SIZE_OTHER, 200) };
                        else
                        {
                            var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(checkList.PACK_SIZE_ID, context);
                            if (temp != null)
                            {
                                (cateTemplate.Values[15] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                            }
                        }
                    }
                }
            }


            (cateTemplate.Values[12] as StringValue).Values = new string[] { CNService.SubString(checkList.PRIMARY_MATERIAL, 200) };
            (cateTemplate.Values[13] as StringValue).Values = new string[] { CNService.SubString(checkList.OTHER_REQUESTS, 200) };

            if (checkList.PRIMARY_TYPE_ID > 0)
            {
                var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(checkList.PRIMARY_TYPE_ID, context);
                if (temp != null)
                {
                    (cateTemplate.Values[8] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                }
            }
            else if (checkList.PRIMARY_TYPE_ID == -1)
                (cateTemplate.Values[8] as StringValue).Values = new string[] { CNService.SubString(checkList.PRIMARY_TYPE_OTHER, 200) };

            if (mockup.PACKING_TYPE_ID > 0)
            {
                var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(mockup.PACKING_TYPE_ID, context);
                if (temp != null)
                {
                    (cateTemplate.Values[21] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                }
            }
            else if (mockup.PACKING_TYPE_ID == -1)
                (cateTemplate.Values[21] as StringValue).Values = new string[] { CNService.SubString(mockup.PACKING_TYPE_OTHER, 200) };



            var listMockupSubId = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = mockUpID }, context);
            var mockupSubId = listMockupSubId.Where(m => m.PARENT_MOCKUP_SUB_ID == null).FirstOrDefault().MOCKUP_SUB_ID;
            var processPG = ART_WF_MOCKUP_PROCESS_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG() { MOCKUP_SUB_ID = mockupSubId }, context).FirstOrDefault();
            if (processPG != null)
            {
                (cateTemplate.Values[17] as StringValue).Values = new string[] { CNService.SubString(processPG.CUSTOMER_SPEC_REMARK, 200) };
                (cateTemplate.Values[18] as StringValue).Values = new string[] { CNService.SubString(processPG.CUSTOMER_SIZE_REMARK, 200) };
                (cateTemplate.Values[19] as StringValue).Values = new string[] { CNService.SubString(processPG.CUSTOMER_NOMINATES_VENDOR_REMARK, 200) };

                (cateTemplate.Values[20] as StringValue).Values = new string[] { CNService.SubString(processPG.FINAL_INFO, 200) };

                if (processPG.GRADE_OF > 0)
                {
                    var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(processPG.GRADE_OF, context);
                    if (temp != null)
                    {
                        (cateTemplate.Values[22] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                    }
                }
                else if (processPG.GRADE_OF == -1)
                    (cateTemplate.Values[22] as StringValue).Values = new string[] { CNService.SubString(processPG.GRADE_OF_OTHER, 200) };

                (cateTemplate.Values[23] as StringValue).Values = new string[] { CNService.SubString(processPG.SHEET_SIZE, 200) };

                if (processPG.VENDOR > 0)
                {
                    var temp = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(processPG.VENDOR, context);
                    if (temp != null)
                    {
                        (cateTemplate.Values[24] as StringValue).Values = new string[] { CNService.SubString(temp.VENDOR_CODE, 200) };
                    }
                }
                else if (processPG.VENDOR == -1)
                    (cateTemplate.Values[24] as StringValue).Values = new string[] { CNService.SubString(processPG.VENDOR_OTHER, 200) };

                if (processPG.FLUTE > 0)
                {
                    var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(processPG.FLUTE, context);
                    if (temp != null)
                    {
                        (cateTemplate.Values[25] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                    }
                }
                else if (processPG.FLUTE == -1)
                    (cateTemplate.Values[25] as StringValue).Values = new string[] { CNService.SubString(processPG.FLUTE_OTHER, 200) };

                (cateTemplate.Values[26] as StringValue).Values = new string[] { CNService.SubString(processPG.DIMENSION_OF, 200) };

                if (processPG.DI_CUT > 0)
                {
                    var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(processPG.DI_CUT, context);
                    if (temp != null)
                    {
                        (cateTemplate.Values[27] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                    }
                }
                else if (processPG.DI_CUT == -1)
                    (cateTemplate.Values[27] as StringValue).Values = new string[] { CNService.SubString(processPG.DI_CUT_OTHER, 200) };

                (cateTemplate.Values[28] as StringValue).Values = new string[] { CNService.SubString(processPG.ACCESSORIES, 200) };
                (cateTemplate.Values[29] as StringValue).Values = new string[] { CNService.SubString(processPG.PRINT_SYSTEM, 200) };
                (cateTemplate.Values[30] as StringValue).Values = new string[] { CNService.SubString(processPG.ROLL_SHEET, 200) };
            }

            inputCategory.AttributeGroups = new AttributeGroup[] { cateTemplate };
            return inputCategory;
        }

        public static Node updateCategory(long nodeID, Metadata inputCategory, string token)
        {
            DocumentManagementClient docClient = new DocumentManagementClient();

            try
            {
                DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
                docOTAuthen.AuthenticationToken = token;
                Node node = docClient.GetNode(ref docOTAuthen, nodeID);

                node.Metadata = inputCategory;
                docClient.UpdateNode(ref docOTAuthen, node);

                return node;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                docClient.Close();
            }
        }

        private static Metadata setCategoryArtwork(int artworkRequestID, ARTWORKEntities context, int artworkItemId, string token)
        {
            var artworkRequest = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(artworkRequestID, context);

            long cateTemplateID = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkCattTemplateNodeID"]);

            Metadata inputCategory = new Metadata();

            DocumentManagementClient docClient = new DocumentManagementClient();
            DocumentManagement.OTAuthentication docOTAuthen = new DocumentManagement.OTAuthentication();
            docOTAuthen.AuthenticationToken = token;

            AttributeGroup cateTemplate = docClient.GetCategoryTemplate(ref docOTAuthen, cateTemplateID);

            if (artworkItemId > 0)
            {
                var artworkWF = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(artworkItemId, context);
                if (artworkWF != null)
                    (cateTemplate.Values[0] as StringValue).Values = new string[] { CNService.SubString(artworkWF.REQUEST_ITEM_NO, 200) };
            }
            else
            {
                (cateTemplate.Values[0] as StringValue).Values = new string[] { CNService.SubString(artworkRequest.ARTWORK_REQUEST_NO, 200) };
            }

            if (artworkRequest.COMPANY_ID > 0)
            {
                var temp = SAP_M_COMPANY_SERVICE.GetByCOMPANY_ID(artworkRequest.COMPANY_ID, context);
                if (temp != null)
                {
                    (cateTemplate.Values[1] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                }
            }

            if (1 == 1)
            {
                var list = ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT() { ARTWORK_REQUEST_ID = artworkRequest.ARTWORK_REQUEST_ID }, context);
                string[] arr = new string[list.Count];
                int i = 0;
                foreach (var s in list)
                {
                    var temp = SAP_M_PLANT_SERVICE.GetByPLANT_ID(s.PRODUCTION_PLANT_ID, context);
                    if (temp != null)
                    {
                        arr[i] = CNService.SubString(temp.NAME, 200);
                        i++;
                    }
                }
                StringValue submitPer = cateTemplate.Values[2] as StringValue;
                submitPer.Values = arr;
            }

            if (artworkRequest.SOLD_TO_ID > 0)
            {
                var temp = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(artworkRequest.SOLD_TO_ID, context);
                if (temp != null)
                {
                    (cateTemplate.Values[3] as StringValue).Values = new string[] { CNService.SubString(temp.CUSTOMER_NAME, 200) };
                }
            }

            if (artworkRequest.SHIP_TO_ID > 0)
            {
                var temp = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(artworkRequest.SHIP_TO_ID, context);
                if (temp != null)
                {
                    (cateTemplate.Values[4] as StringValue).Values = new string[] { CNService.SubString(temp.CUSTOMER_NAME, 200) };
                }
            }

            if (1 == 1)
            {
                var list = ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_COUNTRY() { ARTWORK_REQUEST_ID = artworkRequest.ARTWORK_REQUEST_ID }, context);
                string[] arr = new string[list.Count];
                int i = 0;
                foreach (var s in list)
                {
                    var temp = SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(s.COUNTRY_ID, context);
                    if (temp != null)
                    {
                        arr[i] = CNService.SubString(temp.NAME, 200);
                        i++;
                    }
                }
                StringValue submitPer = cateTemplate.Values[5] as StringValue;
                submitPer.Values = arr;
            }

            if (artworkRequest.BRAND_ID > 0)
            {
                var temp = SAP_M_BRAND_SERVICE.GetByBRAND_ID(artworkRequest.BRAND_ID, context);
                if (temp != null)
                {
                    (cateTemplate.Values[6] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                }
            }

            if (artworkRequest.PRIMARY_TYPE_ID > 0)
            {
                var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(artworkRequest.PRIMARY_TYPE_ID, context);
                if (temp != null)
                {
                    (cateTemplate.Values[7] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                }
            }
            else if (artworkRequest.PRIMARY_TYPE_ID == -1)
                (cateTemplate.Values[7] as StringValue).Values = new string[] { CNService.SubString(artworkRequest.PRIMARY_TYPE_OTHER, 200) };

            if (1 == 1)
            {
                var listProduct = ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_PRODUCT() { ARTWORK_REQUEST_ID = artworkRequest.ARTWORK_REQUEST_ID }, context);
                var listRD = ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_REFERENCE() { ARTWORK_REQUEST_ID = artworkRequest.ARTWORK_REQUEST_ID }, context);

                if (listProduct.Count > 0)
                {
                    var temp = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(listProduct[0].PRODUCT_CODE_ID, context);
                    if (temp != null)
                    {
                        (cateTemplate.Values[8] as StringValue).Values = new string[] { CNService.SubString(temp.PRIMARY_SIZE, 200) };
                        (cateTemplate.Values[9] as StringValue).Values = new string[] { CNService.SubString(temp.CONTAINER_TYPE, 200) };
                        (cateTemplate.Values[10] as StringValue).Values = new string[] { CNService.SubString(temp.LID_TYPE, 200) };
                        (cateTemplate.Values[12] as StringValue).Values = new string[] { CNService.SubString(temp.PACKING_STYLE, 200) };
                        (cateTemplate.Values[13] as StringValue).Values = new string[] { CNService.SubString(temp.PACK_SIZE, 200) };
                    }
                }
                else
                {
                    if (artworkRequest.THREE_P_ID > 0)
                    {
                        var temp = SAP_M_3P_SERVICE.GetByTHREE_P_ID(artworkRequest.THREE_P_ID, context);
                        if (temp != null)
                        {
                            (cateTemplate.Values[8] as StringValue).Values = new string[] { CNService.SubString(temp.PRIMARY_SIZE_VALUE, 200) };
                            (cateTemplate.Values[9] as StringValue).Values = new string[] { CNService.SubString(temp.CONTAINER_TYPE_VALUE, 200) };
                            (cateTemplate.Values[10] as StringValue).Values = new string[] { CNService.SubString(temp.LID_TYPE_VALUE, 200) };
                        }
                    }
                    else
                    {
                        if (artworkRequest.THREE_P_ID == -1)
                            (cateTemplate.Values[8] as StringValue).Values = new string[] { CNService.SubString(artworkRequest.PRIMARY_SIZE_OTHER, 200) };

                        if (artworkRequest.CONTAINER_TYPE_ID == -1)
                            (cateTemplate.Values[9] as StringValue).Values = new string[] { CNService.SubString(artworkRequest.CONTAINER_TYPE_OTHER, 200) };
                        else
                        {
                            var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(artworkRequest.CONTAINER_TYPE_ID, context);
                            if (temp != null)
                            {
                                (cateTemplate.Values[9] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                            }
                        }

                        if (artworkRequest.LID_TYPE_ID == -1)
                            (cateTemplate.Values[10] as StringValue).Values = new string[] { CNService.SubString(artworkRequest.LID_TYPE_OTHER, 200) };
                        else
                        {
                            var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(artworkRequest.LID_TYPE_ID, context);
                            if (temp != null)
                            {
                                (cateTemplate.Values[10] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                            }
                        }
                    }

                    if (artworkRequest.TWO_P_ID > 0)
                    {
                        var temp = SAP_M_2P_SERVICE.GetByTWO_P_ID(artworkRequest.TWO_P_ID, context);
                        if (temp != null)
                        {
                            (cateTemplate.Values[12] as StringValue).Values = new string[] { CNService.SubString(temp.PACKING_SYLE_VALUE, 200) };
                            (cateTemplate.Values[13] as StringValue).Values = new string[] { CNService.SubString(temp.PACK_SIZE_VALUE, 200) };
                        }
                    }
                    else
                    {
                        if (artworkRequest.TWO_P_ID == -1)
                            (cateTemplate.Values[12] as StringValue).Values = new string[] { CNService.SubString(artworkRequest.PACKING_STYLE_OTHER, 200) };

                        if (artworkRequest.PACK_SIZE_ID == -1)
                            (cateTemplate.Values[13] as StringValue).Values = new string[] { CNService.SubString(artworkRequest.PACK_SIZE_OTHER, 200) };
                        else
                        {
                            var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(artworkRequest.PACK_SIZE_ID, context);
                            if (temp != null)
                            {
                                (cateTemplate.Values[13] as StringValue).Values = new string[] { CNService.SubString(temp.DESCRIPTION, 200) };
                            }
                        }
                    }
                }
            }

            inputCategory.AttributeGroups = new AttributeGroup[] { cateTemplate };
            return inputCategory;
        }
    }
}