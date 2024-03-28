using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServices.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLLTests.MM73_ServiceReference;

namespace WebServices.Helper.Tests
{
    [TestClass()]
    public class MM_73_HeplerTests
    {
        [TestMethod()]
        public void SaveMaterialTest()
        {

            try
            {
                SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();
                MM73Client client = new MM73Client();
                IGRID_OUTBOUND_MODEL iGrid_Model = new IGRID_OUTBOUND_MODEL();

                List<IGRID_OUTBOUND_HEADER_MODEL> iGrid_Header_List = new List<IGRID_OUTBOUND_HEADER_MODEL>();
                List<IGRID_OUTBOUND_ITEM_MODEL> iGrid_Item_List = new List<IGRID_OUTBOUND_ITEM_MODEL>();

                IGRID_OUTBOUND_HEADER_MODEL iGrid_Header_Model = new IGRID_OUTBOUND_HEADER_MODEL();
                IGRID_OUTBOUND_ITEM_MODEL iGrid_Item_Model = new IGRID_OUTBOUND_ITEM_MODEL();

                MM73_OUTBOUND_MATERIAL_NUMBER matNumber = new MM73_OUTBOUND_MATERIAL_NUMBER();
                MM73_OUTBOUND_MATERIAL_NUMBERResponse resp = new MM73_OUTBOUND_MATERIAL_NUMBERResponse();

                string artworkNO = "AW-N-2018-00000110";
                string date = DateTime.Now.ToString("yyyyMMdd");
                string time = DateTime.Now.ToString("HH:mm:ss");

                //iGrid_Header_Model.ArtworkNumber = artworkNO;
                //iGrid_Header_Model.Date = date;
                //iGrid_Header_Model.Time = time;
                //iGrid_Header_Model.RecordType = "I";
                //iGrid_Header_Model.ArtworkURL = "http://idc-app-dv3-114.thaiunion.co.th:8089/TaskFormArtwork/85";
                //iGrid_Header_Model.PAUserName = "MO123";
                //iGrid_Header_Model.PGUserName = "MO456";
                //iGrid_Header_Model.Plant = "1001";

                iGrid_Header_Model.ArtworkNumber = "100002221";
                iGrid_Header_Model.Date = date;
                iGrid_Header_Model.Time = time;
                iGrid_Header_Model.RecordType = "I";
                //iGrid_Header_Model.MaterialNumber = "5F198114N000000401";
                iGrid_Header_Model.MaterialDescription = "CTN3 - 60960,LUCKY";
                //iGrid_Header_Model.MaterialCreatedDate = "20180701";
                iGrid_Header_Model.ArtworkURL = "http://artwork.thaiunion.com/content/aw-file.pdf";
                //iGrid_Header_Model.Status = "Completed";
                iGrid_Header_Model.PAUserName = "MO600532";
                iGrid_Header_Model.PGUserName = "MO600313";
                // iGrid_Header_Model.ReferenceMaterial = "test";
                iGrid_Header_Model.Plant = "1021; 1022; 1031";
                //iGrid_Header_Model.PrintingStyleofPrimary = "xxxxx";
                //iGrid_Header_Model.PrintingStyleofSecondary = "xxxxx";
                //iGrid_Header_Model.CustomersDesign = "xxxxx";
                //iGrid_Header_Model.CustomersDesignDetail = "xxxxx";
                //iGrid_Header_Model.CustomersSpec = "xxxxx";

                //iGrid_Header_Model.CustomersSpecDetail = "xxxxx";

                //iGrid_Header_Model.CustomersSize = "xxxxx";

                //iGrid_Header_Model.CustomersSizeDetail = "xxxxx";

                //iGrid_Header_Model.CustomerNominatesVendor = "xxxxx";

                //iGrid_Header_Model.CustomerNominatesVendorDetail = "xxxxx";

                //iGrid_Header_Model.CustomerNominatesColorPantone = "xxxxx";

                //iGrid_Header_Model.CustomerNominatesColorPantoneDetail = "xxxxx";

                iGrid_Header_Model.CustomersBarcodeScanable = "xxx";

                //iGrid_Header_Model.CustomersBarcodeScanableDetail = "xxxxx";

                //iGrid_Header_Model.CustomersBarcodeSpec = "xxxxx";

                //iGrid_Header_Model.CustomersBarcodeSpecDetail = "xxxxx";

                //iGrid_Header_Model.FirstInfoGroup = "xxxxx";

                //iGrid_Header_Model.SONumber = "xxxxx";

                //iGrid_Header_Model.SOitem = "xxxxx";

                //iGrid_Header_Model.SOPlant = "xxxxx";

                //iGrid_Header_Model.PICMKT = "xxxxx";

                //iGrid_Header_Model.Destination = "xxxxx";

                //iGrid_Header_Model.RemarkNoteofPA = "xxxxx";

                //iGrid_Header_Model.FinalInfoGroup = "xxxxx";

                //iGrid_Header_Model.RemarkNoteofPG = "xxxxx";

                //iGrid_Header_Model.CompleteInfoGroup = "xxxxx";

                //iGrid_Header_Model.ProductionExpirydatesystem = "xxxxx";

                //iGrid_Header_Model.Seriousnessofcolorprinting = "xxxxx";

                //iGrid_Header_Model.CustIngreNutritionAnalysis = "xxxxx";

                //iGrid_Header_Model.ShadeLimit = "xxxxx";

                // iGrid_Header_Model.PackageQuantity = "";

                //   iGrid_Header_Model.WastePercent = "";


                iGrid_Header_List.Add(iGrid_Header_Model);
                iGrid_Model.OUTBOUND_HEADERS = iGrid_Header_List.ToArray();


                iGrid_Item_Model.ArtworkNumber = artworkNO;
                iGrid_Item_Model.Date = date;
                iGrid_Item_Model.Time = time;
                iGrid_Item_Model.Characteristic = "ZPKG_SEC_GROUP";
                iGrid_Item_Model.Value = "C";
                iGrid_Item_Model.Description = "Displayer";

                iGrid_Item_List.Add(iGrid_Item_Model);
                iGrid_Model.OUTBOUND_ITEMS = iGrid_Item_List.ToArray();


                matNumber.param = iGrid_Model;
                resp = client.MATERIAL_NUMBER(matNumber);
            }
            catch //(Exception ex)
            {

            }
        }

        [TestMethod()]
        public void UpdatePADataTest()
        {
            WebServices.Model.SERVICE_RESULT_MODEL model = new WebServices.Model.SERVICE_RESULT_MODEL();
            WebServices.Model.IGRID_OUTBOUND_MODEL param = new WebServices.Model.IGRID_OUTBOUND_MODEL();
            WebServices.Model.IGRID_OUTBOUND_HEADER_MODEL header = new WebServices.Model.IGRID_OUTBOUND_HEADER_MODEL();
            List<WebServices.Model.IGRID_OUTBOUND_HEADER_MODEL> listHeader = new List<WebServices.Model.IGRID_OUTBOUND_HEADER_MODEL>();

            header.ArtworkNumber = "AW-N-2019-00001863";
            listHeader.Add(header);

            //header = new WebServices.Model.IGRID_OUTBOUND_HEADER_MODEL();
            //header.ArtworkNumber = "AW-N-2019-00001048";
            //listHeader.Add(header);

            param.OUTBOUND_HEADERS = listHeader;
            MM_73_Hepler.UpdatePAData(param);
        }
    }



}