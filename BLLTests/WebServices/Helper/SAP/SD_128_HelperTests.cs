using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServices.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using DAL;
using System.Data;
using WebServices.Model;

namespace WebServices.Helper.Tests
{
    [TestClass()]
    public class SD_128_HelperTests
    {
        //[TestMethod()]
        //public void ImportLongText()
        //{
        //    SAP_M_LONG_TEXT _longText = new SAP_M_LONG_TEXT();
        //    string fileName = "TX_20180917_105424.xml";
        //    StringBuilder SB = new StringBuilder();

        //    // Set a variable to the My Documents path.
        //    string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //    string path = "";

        //    path = mydocpath + "\\" + fileName;
        //    try
        //    {

        //        var xmlString = File.ReadAllText(path);
        //        var stringReader = new StringReader(xmlString);
        //        var dsSet = new DataSet();
        //        dsSet.ReadXml(stringReader);


        //        using (XmlReader reader = XmlReader.Create(path))
        //        {
        //            while (reader.Read())
        //            {
        //                // Only detect start elements.
        //                if (reader.IsStartElement())
        //                {

        //                    // Get element name and switch on it.
        //                    switch (reader.Name)
        //                    {

        //                        case "TEXT_NAME":

        //                            if (reader.Read())
        //                            {
        //                                SB.AppendLine("Text Name: " + reader.Value.Trim());
        //                            }
        //                            break;
        //                        case "TEXT_ID":
        //                            if (reader.Read())
        //                            {
        //                                SB.AppendLine("  Text ID: " + reader.Value.Trim());
        //                            }
        //                            break;
        //                        case "LANGUAGE":
        //                            if (reader.Read())
        //                            {
        //                                SB.AppendLine("  Language: " + reader.Value.Trim());
        //                            }
        //                            break;
        //                        //case "ID":
        //                        //    if (reader.Read())
        //                        //    {
        //                        //        SB.AppendLine("     Line ID: " + reader.Value.Trim());
        //                        //    }
        //                        //    break;
        //                        case "TEXT":
        //                            if (reader.Read())
        //                            {
        //                                SB.AppendLine("     Line Text: " + reader.Value);
        //                            }
        //                            break;
        //                    }

        //                    //SAP_M_LONG_TEXT_SERVICE.SaveOrUpdate(_longText);
        //                }


        //            }
        //        }

        //        var ss = SB.ToString();

        //        string[] arrLongText = ss.Split(new string[] { "Text Name" }, StringSplitOptions.None);

        //        foreach (string item in arrLongText)
        //        {
        //            if (!String.IsNullOrEmpty(item))
        //            {
        //                string[] arrItem = item.Split(new string[] { "Line Text: " }, StringSplitOptions.None);

        //                if (arrItem.Count() > 0)
        //                {

        //                    bool isFirst = false;
        //                    int intLine = 0;

        //                    foreach (string line in arrItem)
        //                    {

        //                        // string[] longTextHeader = line.Split(new string[] { "\r\n" }, StringSplitOptions.None);

        //                        if (isFirst == false)
        //                        {
        //                            _longText = new SAP_M_LONG_TEXT();
        //                            _longText.TEXT_NAME = arrItem[0].Split(new string[] { "\r\n" }, StringSplitOptions.None)[0].Replace(":", "").Trim();
        //                            _longText.TEXT_ID = arrItem[0].Split(new string[] { "\r\n" }, StringSplitOptions.None)[1].Replace("Text ID:", "").Trim();
        //                            _longText.TEXT_LANGUAGE = arrItem[0].Split(new string[] { "\r\n" }, StringSplitOptions.None)[2].Replace("Language:", "").Trim();
        //                            isFirst = true;
        //                        }
        //                        else
        //                        {
        //                            intLine += 1;
        //                            _longText.LINE_ID = intLine;
        //                            _longText.LINE_TEXT = line;
        //                            _longText.CREATE_BY = -2;
        //                            _longText.UPDATE_BY = -2;

        //                            SAP_M_LONG_TEXT_SERVICE.SaveOrUpdate(_longText);
        //                            isFirst = false;
        //                        }
        //                    }

        //                }

        //            }

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        string exResults = ex.Message;
        //    }
        //}
        //if (File.Exists(path))
        //{
        //    using (StreamWriter outputFile = new StreamWriter(Path.Combine(mydocpath, fileName), true))
        //    {

        //    }

        //}

        [TestMethod()]
        public void SaveLongTextTest()
        {
            bool result = false;
            List<LONG_TXT> listLongText = new List<LONG_TXT>();

            try
            {
                //================================================= case update =================================================
                var line1 = new List<LINE> {
                new LINE { ID = 1, TEXT = "1.LBL UP-SIDE DOWN--'update1" },
                new LINE { ID = 2, TEXT = "2.UV LBL--update1" },
                new LINE { ID = 3, TEXT = "3.HALF PRINTED CAN USE SINGAPORE PKG VERSION--update1" },
                new LINE { ID = 4, TEXT = "4.LBL W/HALAL LOGO--update1" },
                new LINE { ID = 5, TEXT = "5.(400)USE WHITE CARTON--update1" },
                new LINE { ID = 6, TEXT = "--update1" },
                new LINE { ID = 7, TEXT = "7.INSERT DIVIDER PAPER BETWEEN THE LAYER--update1" },
                new LINE { ID = 8, TEXT = "8.ให้ระบุเลขที่ PO ของลูกค้าลงในเอกสารส่งสินค้าด้วยทุกครั้ง ขอให้ WAREHOUSE ช่วยส่งเอกสารการส่ง--update1" },
                new LINE { ID = 9, TEXT = "สินค้าที่มีลายเซ็นรับสินค้าจาก ลูกค้าให้ MKT & MS เพื่อให้แนบตอนเรียกเก็บค่าสินค้าจากลูกค้าด้วย--update1" }};

                LONG_TXT longTxt1 = new LONG_TXT();
                longTxt1.TEXT_NAME = "0500013657000401";
                longTxt1.TEXT_ID = "Z105";
                longTxt1.LANGUAGE = "EN";
                longTxt1.LINES = line1;

                var line2 = new List<LINE> {
                new LINE { ID = 1, TEXT = "1.(500)FOR PRODUCT IN HALF PRINTED CAN--update2" },
                new LINE { ID = 2, TEXT = "2.LBL UP-SIDE DOWN--update2" },
                new LINE { ID = 3, TEXT = "3.UV LBL'update2" },
                new LINE { ID = 4, TEXT = "4.HALF PRINTED CAN USE SINGAPORE PKG VERSION--update2" },
                new LINE { ID = 5, TEXT = "5.LBL W/HALAL LOGO--update2" },
                new LINE { ID = 6, TEXT = "6.(500)USE WHITE CARTON--update2" },
                new LINE { ID = 7, TEXT = "7.INSERT DIVIDER PAPER' BETWEEN THE LAYER--update2" },
                new LINE { ID = 8, TEXT = "8.ให้ระบุเลขที่ PO ของลูกค้าลงในเอกสารส่งสินค้าด้วยทุกครั้ง ขอให้ WAREHOUSE ช่วยส่งเอกสารการส่ง--update2" },
                new LINE { ID = 9, TEXT = "สินค้าที่มีล@#ายเซ็นรับสินค้าจาก ลูกค้าให้ MKT & MS เพื่อให้แนบตอนเรียกเก็บค่าสินค้าจากลูกค้าด้วย--update2" }};
                LONG_TXT longTxt2 = new LONG_TXT();
                longTxt2.TEXT_NAME = "0500013657000500";
                longTxt2.TEXT_ID = "Z105";
                longTxt2.LANGUAGE = "EN";
                longTxt2.LINES = line2;

                listLongText.Add(longTxt1);
                listLongText.Add(longTxt2);

                //================================================= case insert =================================================
                var line3 = new List<LINE> {
                new LINE { ID = 1, TEXT = "1.LBL UP-SID!@#E$%^&*()_+'?>< DOWN--insert3" },
                new LINE { ID = 2, TEXT = "2.UV LBL--insert3" },
                new LINE { ID = 3, TEXT = "3.HALF PRINTED CAN USE SINGAPORE PKG VERSION--insert3" },
                new LINE { ID = 4, TEXT = "4.LBL W/HALAL LOGO--insert3" },
                new LINE { ID = 5, TEXT = "5.(400)USE WHITE CARTON--insert3" },
                new LINE { ID = 6, TEXT = "--insert3" },
                new LINE { ID = 7, TEXT = "7.INSERT DIVIDER PAPER BETWEEN THE LAYER--insert3" },
                new LINE { ID = 8, TEXT = "8.ให้ระบุเลขที่ PO ของลูกค้าลงในเอกสารส่งสินค้าด้วยทุกครั้ง ขอให้ WAREHOUSE ช่วยส่งเอกสารการส่ง--insert3" },
                new LINE { ID = 9, TEXT = "สินค้าที่มีลายเซ็นรับสินค้าจาก ลูกค้าให้ MKT & MS เพื่อให้แนบตอนเรียกเก็บค่าสินค้าจากลูกค้าด้วย--insert3" }};

                LONG_TXT longTxt3 = new LONG_TXT();
                longTxt3.TEXT_NAME = "3333333333333333";
                longTxt3.TEXT_ID = "Zzzz";
                longTxt3.LANGUAGE = "EN";
                longTxt3.LINES = line3;

                var line4 = new List<LINE> {
                new LINE { ID = 1, TEXT = "1.'(500)FOR PRODUCT IN HALF PRINTED CAN'--insert4" },
                new LINE { ID = 2, TEXT = "2.@LBL UP-SIDE DOWN--insert4" },
                new LINE { ID = 3, TEXT = "3.UV L#BL--insert4" },
                new LINE { ID = 4, TEXT = "4.HALF$ PRINTED CAN USE SINGAPORE PKG VERSION--insert4" },
                new LINE { ID = 5, TEXT = "5.LBL W/%HALAL LOGO--insert4" },
                new LINE { ID = 6, TEXT = "6.(5!00)USE WHITE CARTON--insert4" },
                new LINE { ID = 7, TEXT = "7.INSE*RT DIVIDER PAPER BETWEEN THE LAYER--insert4" },
                new LINE { ID = 8, TEXT = "8.ให้ระบุเลขที่ P<>O ของลูก'ค้าลงในเอกสารส่งสินค้าด้วยทุกครั้ง ขอให้ WAREHOUSE ช่วยส่งเอกสารการส่ง--insert4" },
                new LINE { ID = 9, TEXT = "สินค้าที่มีลา'ยเซ็นรับสิ?นค้าจาก ลูกค้าให้ MKT & MS เพื่อให้แนบตอนเรียกเก็บค่าสินค้าจากลูกค้าด้วย--insert4" }};
                LONG_TXT longTxt4 = new LONG_TXT();
                longTxt4.TEXT_NAME = "'4444444444444444";
                longTxt4.TEXT_ID = "Yyy'y";
                longTxt4.LANGUAGE = "E'N";
                longTxt4.LINES = line4;

                listLongText.Add(longTxt3);
                listLongText.Add(longTxt4);

                //string ss = @"aaaa\bbnn";
                ////char split = @'\';
                //string[] xx = ss.Split('\\');
                SD_128_Helper.SaveLongText(listLongText);
                result = true;
            }
            catch 
            {

            }

            Assert.IsTrue(result);
        }
    }
}
