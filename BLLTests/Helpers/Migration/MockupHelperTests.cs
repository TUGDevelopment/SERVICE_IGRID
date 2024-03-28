using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;
using DAL;
using BLL.DocumentManagement;
using BLL.Services;
using System.Configuration;
using System.IO;
using System.Web;

namespace BLL.Helpers.Tests
{
    [TestClass()]
    public class MockupHelperTests
    {
        [TestMethod()]
        public void CreateFolderInCS()
        {
            int create_by = 301;
            long folderID = Convert.ToInt64(ConfigurationManager.AppSettings["MockUpNodeID"]);
            long templateID = Convert.ToInt64(ConfigurationManager.AppSettings["MockUpTemplateNodeID"]);
            string dieline_folderName = ConfigurationManager.AppSettings["MockupFolderNameDieline"];

            //get mockup_no = MO-C
            var mockup_list = new List<ART_WF_MOCKUP_CHECK_LIST_ITEM>();
            using (var context = new ARTWORKEntities())
            {
                mockup_list = (from h in context.ART_WF_MOCKUP_CHECK_LIST_ITEM
                               where h.MOCKUP_NO.Contains("MO-C-2019")
                               select h).ToList();
            }
            var token = CWSService.getAuthToken();
            foreach (var itemMockup in mockup_list)
            {
                var mockup_no_node_id = CWSService.getNodeByName(folderID, itemMockup.MOCKUP_NO, token);
                if (mockup_no_node_id == null)
                {
                    using (var context = new ARTWORKEntities())
                    {
                        //create folder in cs
                        var nodeTemplate = CWSService.copyNode(itemMockup.MOCKUP_NO, templateID, folderID, token);

                        CWSService.updateCategoryMockup(itemMockup.MOCKUP_NO, itemMockup.MOCKUP_ID, token);

                        //update node id to mockup
                        var temp = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(itemMockup.MOCKUP_ID, context);
                        temp.NODE_ID = nodeTemplate.ID;
                        ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.SaveOrUpdate(temp, context);

                        //get folder dieline
                        Node nodeOthers = BLL.Services.CWSService.getNodeByName(Convert.ToInt64(nodeTemplate.ID), dieline_folderName, token);

                        //get folder name by packing type
                        var packing_type = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(itemMockup.PACKING_TYPE_ID, context);

                        var arrInfogroup = itemMockup.MOCKUP_NO.Split('-');
                        if (arrInfogroup.Length > 4)
                        {
                            DirectoryInfo d = new DirectoryInfo(@"D:\ECM\TUArtwork\Migration\Dieline\Migrate die line Phase 1 as of 26.02.19  [CK Revised]");// + packing_type.DESCRIPTION);//Assuming Test is your Folder
                            FileInfo[] Files = d.GetFiles("*" + arrInfogroup[3] + "*"); //Getting Text files

                            foreach (FileInfo file in Files)
                            {
                                //Open the stream and read it back.
                                using (FileStream fs = file.OpenRead())
                                {
                                    Node node = BLL.Services.CWSService.uploadFile(fs, file.Name, nodeOthers.ID, token);

                                    string extension = file.Extension.Replace(".", "");
                                    string contentType = MimeMapping.GetMimeMapping(file.Name);

                                    var mockup_process = new List<ART_WF_MOCKUP_PROCESS>();
                                    mockup_process = (from p in context.ART_WF_MOCKUP_PROCESS
                                                      where p.MOCKUP_ID.Equals(itemMockup.MOCKUP_ID)
                                                      && p.PARENT_MOCKUP_SUB_ID == null
                                                      select p).ToList();

                                    var att = AttachmentMockupHelper.SaveAttachment(node.Name, extension, contentType, itemMockup.MOCKUP_ID, mockup_process.FirstOrDefault().MOCKUP_SUB_ID, Convert.ToInt32(file.Length), node.ID, create_by, mockup_process.FirstOrDefault().CURRENT_ROLE_ID
                                            , 1, "X", "", "", context);
                                }

                            }
                        }
                    }
                }
            }

        }

        [TestMethod()]
        public void UpdateCategoryInCS()
        {
            long folderID = Convert.ToInt64(ConfigurationManager.AppSettings["MockUpNodeID"]);
            long templateID = Convert.ToInt64(ConfigurationManager.AppSettings["MockUpTemplateNodeID"]);
            string dieline_folderName = ConfigurationManager.AppSettings["MockupFolderNameDieline"];
            //get mockup_no = MO-C
            var mockup_list = new List<ART_WF_MOCKUP_CHECK_LIST_ITEM>();
            using (var context = new ARTWORKEntities())
            {
                //string[] mockup_list_1 = new string[] { "MO-C-2019-1000006705-001", "MO-C-2019-1000006779-001" };

                //mockup_list = (from h in context.ART_WF_MOCKUP_CHECK_LIST_ITEM
                //               where mockup_list_1.Contains(h.MOCKUP_NO)
                //               select h).ToList();

                mockup_list = (from h in context.ART_WF_MOCKUP_CHECK_LIST_ITEM
                               where h.MOCKUP_NO.Contains("MO-C-")
                               select h).ToList();
                var token = CWSService.getAuthToken();
                foreach (var itemMockup in mockup_list)
                {
                    var mockup_no_node_id = CWSService.getNodeByName(folderID, itemMockup.MOCKUP_NO, token);
                    if (mockup_no_node_id != null)
                    {
                        //CWSService.updateCategoryMockup(itemMockup.MOCKUP_NO, itemMockup.MOCKUP_ID, create_by);
                        var cat = CWSService.setCategory(itemMockup.MOCKUP_ID, context, token);
                        CWSService.updateCategory(mockup_no_node_id.ID, cat, token);
                    }
                }
            }



        }
    }
}