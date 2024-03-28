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

namespace BLL.Helpers.Tests
{
    [TestClass()]
    public class ArtworkRequestHelperTests
    {
        [TestMethod()]
        public void GetMaterialBySalesOrderTest()
        {
            ART_WF_ARTWORK_REQUEST_REQUEST param = new ART_WF_ARTWORK_REQUEST_REQUEST();
            ART_WF_ARTWORK_REQUEST_2 data = new ART_WF_ARTWORK_REQUEST_2();

            data.ARTWORK_REQUEST_ID = 459;
            data.UPDATE_BY = 133;
            param.data = data;
            // ArtworkRequestHelper.GetMaterialBySalesOrder(param);
        }

        [TestMethod()]
        public void CreateFolderInCS()
        {
            //Create Folder in CS

            string formNo = "";

            long folderID = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkRequestFormNodeID"]);
            long templateID = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkTemplateNodeID"]);

            var token = CWSService.getAuthToken();
            var nodeTemplate = CWSService.copyNode(formNo, templateID, folderID, token);

        }

        [TestMethod()]
        public void DeleteArtworkRequestTest()
        {
            List<int> listReqID = new List<int>();
            string msg = "";
            try
            {
                listReqID.Add(24557);
                listReqID.Add(24560);
                listReqID.Add(24562);
                listReqID.Add(24561);
                listReqID.Add(24563);
                listReqID.Add(24564);
                listReqID.Add(24565);
                listReqID.Add(24566);
                listReqID.Add(24567);
                listReqID.Add(24568);
                listReqID.Add(24570);
                listReqID.Add(24569);
                listReqID.Add(24571);
                listReqID.Add(24572);
                listReqID.Add(24573);
                listReqID.Add(24574);
                listReqID.Add(24575);
                listReqID.Add(24576);
                listReqID.Add(24578);
                listReqID.Add(24577);
                listReqID.Add(24579);
                listReqID.Add(24580);
                listReqID.Add(24581);
                listReqID.Add(24582);
                listReqID.Add(24583);
                listReqID.Add(24585);
                listReqID.Add(24594);
                listReqID.Add(24595);
                listReqID.Add(24596);
                listReqID.Add(24597);
                listReqID.Add(24598);
                listReqID.Add(24637);
                listReqID.Add(24642);
                listReqID.Add(24674);
                listReqID.Add(24677);
                listReqID.Add(24676);
                listReqID.Add(23218);
                listReqID.Add(23180);
                listReqID.Add(23223);
                listReqID.Add(23229);
                listReqID.Add(23232);
                listReqID.Add(23233);
                listReqID.Add(23458);
                listReqID.Add(23485);
                listReqID.Add(23769);
                listReqID.Add(24694);
                listReqID.Add(24690);
                listReqID.Add(24696);
                listReqID.Add(24700);
                listReqID.Add(24703);
                listReqID.Add(24705);
                listReqID.Add(24708);
                listReqID.Add(24720);
                listReqID.Add(24727);
                listReqID.Add(24729);
                listReqID.Add(24736);
                listReqID.Add(24731);
                listReqID.Add(24757);
                listReqID.Add(24759);
                listReqID.Add(23219);
                listReqID.Add(23183);
                listReqID.Add(23199);
                listReqID.Add(23219);
                listReqID.Add(23199);
                listReqID.Add(23183);
                listReqID.Add(24732);
                listReqID.Add(24738);
                listReqID.Add(24758);
                listReqID.Add(24441);
                listReqID.Add(24441);
                listReqID.Add(24441);
                listReqID.Add(24690);
                listReqID.Add(24694);
                listReqID.Add(24722);
                listReqID.Add(24691);
                listReqID.Add(24695);
                listReqID.Add(24725);
                listReqID.Add(24717);
                listReqID.Add(24734);
                listReqID.Add(24743);

                ART_WF_ARTWORK_REQUEST_REQUEST param = new ART_WF_ARTWORK_REQUEST_REQUEST();
                ART_WF_ARTWORK_REQUEST_2 data = new ART_WF_ARTWORK_REQUEST_2();
                foreach (var ReqID in listReqID)
                {
                    param = new ART_WF_ARTWORK_REQUEST_REQUEST();
                    data = new ART_WF_ARTWORK_REQUEST_2();
                    data.ARTWORK_REQUEST_ID = ReqID;
                    param.data = data;

                    ArtworkRequestHelper.DeleteArtworkRequest(param);
                }

                msg = "Success";
            }
            catch (Exception ex)
            {
                  msg = CNService.GetErrorMessage(ex);

                Console.WriteLine(msg);
            }

            Console.WriteLine(msg);
        }
    }
}