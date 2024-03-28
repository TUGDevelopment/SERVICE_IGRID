using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServices.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Data;
using DAL;
using System.Globalization;

namespace WebServices.Helper.Tests
{
    [TestClass()]
    public class SD_127_HelperTests
    {
        [TestMethod()]
        public void Test()
        {
            string dateString = "";
            dateString = "20180825";

            if (!String.IsNullOrEmpty(dateString))
            {
                DateTime decOut;
                DateTime.TryParseExact(dateString,
                                              "yyyyMMdd",
                                              CultureInfo.InvariantCulture,
                                              DateTimeStyles.None, out decOut);


                if (DateTime.TryParse(dateString, out decOut))
                {
                    // _soHeader.RDD = decOut;
                }
            }
        }


        [TestMethod()]
        public void SavePOCompleteSOTest()
        {


            //string fileName = "SO.xml"; //"SO_20180917_104854.xml";
            string fileName = "SO_20180917_104854.xml";
            StringBuilder SB = new StringBuilder();

            // Set a variable to the My Documents path.
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = "";

            path = mydocpath + "\\" + fileName;
            try
            {
                //var xmlString = File.ReadAllText(path);
                //var stringReader = new StringReader(xmlString);
                //var dsSet = new DataSet();
                //dsSet.ReadXml(stringReader);

                using (XmlReader reader = XmlReader.Create(path))
                {
                    SAP_M_PO_COMPLETE_SO_HEADER _header = new SAP_M_PO_COMPLETE_SO_HEADER();
                    List<SAP_M_PO_COMPLETE_SO_HEADER> _listHeader = new List<SAP_M_PO_COMPLETE_SO_HEADER>();
                    List<SAP_M_PO_COMPLETE_SO_ITEM> _listItem = new List<SAP_M_PO_COMPLETE_SO_ITEM>();
                    List<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT> _listComponent = new List<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT>();

                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == "HEADER")
                            {
                                // Get element name and switch on it.
                                switch (reader.Name)
                                {
                                    case "VBELN":
                                        if (reader.Read())
                                        {
                                            _header.SALES_ORDER_NO = reader.Value.Trim();
                                        }
                                        break;

                                    case "KUNNR_SOLD":
                                        if (reader.Read())
                                        {
                                            _header.SOLD_TO = reader.Value.Trim();
                                        }
                                        break;

                                    case "KUNNR_SOLD_NAME":
                                        if (reader.Read())
                                        {
                                            _header.SOLD_TO_NAME = reader.Value.Trim();
                                        }
                                        break;

                                    case "AYDAT":
                                        if (reader.Read())
                                        {
                                            _header.LC_NO = reader.Value.Trim();
                                        }
                                        break;

                                    case "AHDAT":
                                        if (reader.Read())
                                        {
                                            _header.LAST_SHIPMENT_DATE = Convert.ToDecimal(reader.Value.Trim());
                                        }
                                        break;

                                    case "ERDAT":
                                        if (reader.Read())
                                        {
                                            // _header.CREATE_ON = Convert.ToDateTime(reader.Value.Trim());
                                        }
                                        break;

                                    case "VDATU":
                                        if (reader.Read())
                                        {
                                            // _header.date = reader.Value.Trim();
                                        }
                                        break;

                                    case "VTEXT":
                                        if (reader.Read())
                                        {
                                            _header.PAYMENT_TERM = reader.Value.Trim();
                                        }
                                        break;

                                    case "BAANR":
                                        if (reader.Read())
                                        {
                                            _header.LC_NO = reader.Value.Trim();
                                        }
                                        break;

                                    case "AXDAT":
                                        if (reader.Read())
                                        {
                                            _header.EXPIRED_DATE = Convert.ToDecimal(reader.Value.Trim());
                                        }
                                        break;

                                    case "KUNNR_SHIP":
                                        if (reader.Read())
                                        {
                                            _header.SHIP_TO = reader.Value.Trim();
                                        }
                                        break;

                                    case "KUNNR_SHIP_NAME":
                                        if (reader.Read())
                                        {
                                            _header.SHIP_TO_NAME = reader.Value.Trim();
                                        }
                                        break;

                                    case "BSTKD":
                                        if (reader.Read())
                                        {
                                            _header.SOLD_TO_PO = reader.Value.Trim();
                                        }
                                        break;
                                    case "BSTKD_E":
                                        if (reader.Read())
                                        {
                                            _header.SHIP_TO_PO = reader.Value.Trim();
                                        }
                                        break;

                                    case "VKGRP":
                                        if (reader.Read())
                                        {
                                            _header.SALES_GROUP = reader.Value.Trim();
                                        }
                                        break;

                                    case "KUNNR_MAR_CO":
                                        if (reader.Read())
                                        {
                                            _header.MARKETING_CO = reader.Value.Trim();
                                        }
                                        break;

                                    case "KUNNR_MAR_CO_NAME":
                                        if (reader.Read())
                                        {
                                            _header.MARKETING_CO_NAME = reader.Value.Trim();
                                        }
                                        break;

                                    case "KUNNR_MAR":
                                        if (reader.Read())
                                        {
                                            _header.MARKETING_CO = reader.Value.Trim();
                                        }
                                        break;

                                    case "KUNNR_MAR_NAME":
                                        if (reader.Read())
                                        {
                                            _header.MARKETING_CO_NAME = reader.Value.Trim();
                                        }
                                        break;

                                    case "KUNNR_MAR_ORD":
                                        if (reader.Read())
                                        {
                                            _header.MARKETING_ORDER_SAP = reader.Value.Trim();
                                        }
                                        break;

                                    case "KUNNR_MAR_ORD_NAME":
                                        if (reader.Read())
                                        {
                                            _header.MARKETING_ORDER_SAP_NAME = reader.Value.Trim();
                                        }
                                        break;

                                    case "VKORG":
                                        if (reader.Read())
                                        {
                                            _header.SALES_ORG = reader.Value.Trim();
                                        }
                                        break;

                                    case "VTWEG":
                                        if (reader.Read())
                                        {
                                            _header.DISTRIBUTION_CHANNEL = reader.Value.Trim();
                                        }
                                        break;

                                    case "SPART":
                                        if (reader.Read())
                                        {
                                            _header.DIVITION = reader.Value.Trim();
                                        }
                                        break;

                                    case "AUART":
                                        if (reader.Read())
                                        {
                                            _header.SALES_ORDER_TYPE = reader.Value.Trim();
                                        }
                                        break;

                                    case "HEADER_CUSTOM_1":
                                        if (reader.Read())
                                        {
                                            _header.HEADER_CUSTOM_1 = reader.Value.Trim();
                                        }
                                        break;

                                    case "HEADER_CUSTOM_2":
                                        if (reader.Read())
                                        {
                                            _header.HEADER_CUSTOM_2 = reader.Value.Trim();
                                        }
                                        break;

                                    case "HEADER_CUSTOM_3":
                                        if (reader.Read())
                                        {
                                            _header.HEADER_CUSTOM_3 = reader.Value.Trim();


                                            _listHeader.Add(_header);
                                            _header = new SAP_M_PO_COMPLETE_SO_HEADER();
                                        }
                                        break;


                                    case "ITEM":


                                        if (reader.Read())
                                        {
                                            SAP_M_PO_COMPLETE_SO_ITEM _item = new SAP_M_PO_COMPLETE_SO_ITEM();

                                            while (reader.Read())
                                            {

                                                if (reader.IsStartElement())
                                                {
                                                    switch (reader.Name)
                                                    {
                                                        case "POSNR":
                                                            if (reader.Read())
                                                            {
                                                                _item.ITEM = Convert.ToDecimal(reader.Value);
                                                            }
                                                            break;
                                                        case "ARKTX":
                                                            if (reader.Read())
                                                            {
                                                                _item.MATERIAL_DESCRIPTION = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "MATNR":
                                                            if (reader.Read())
                                                            {
                                                                _item.PRODUCT_CODE = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "ATWRT_NET":
                                                            if (reader.Read())
                                                            {
                                                                _item.NET_WEIGHT = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "KWMENG":
                                                            if (reader.Read())
                                                            {
                                                                _item.ORDER_QTY = Convert.ToDecimal(reader.Value.Trim());
                                                            }
                                                            break;
                                                        case "VRKME":
                                                            if (reader.Read())
                                                            {
                                                                _item.ORDER_UNIT = reader.Value.Trim();
                                                            }
                                                            break;

                                                        case "ZZETD_FR":
                                                            if (reader.Read())
                                                            {
                                                                // _item.ETD_DATE_FROM = Convert.ToDateTime(reader.Value.Trim());
                                                            }
                                                            break;
                                                        case "ZZETD_TO":
                                                            if (reader.Read())
                                                            {
                                                                //  _item.ETD_DATE_TO = Convert.ToDateTime(reader.Value.Trim());
                                                            }
                                                            break;
                                                        case "WERKS":
                                                            if (reader.Read())
                                                            {
                                                                _item.PLANT = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "BISMT":
                                                            if (reader.Read())
                                                            {
                                                                _item.OLD_MATERIAL_CODE = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "ATWRT_PACK":
                                                            if (reader.Read())
                                                            {
                                                                _item.PACK_SIZE = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "VOLUMN":
                                                            if (reader.Read())
                                                            {
                                                                _item.VALUME_PER_UNIT = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "VOLEH":
                                                            if (reader.Read())
                                                            {
                                                                _item.VALUME_UNIT = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "GROES":
                                                            if (reader.Read())
                                                            {
                                                                _item.SIZE_DRAIN_WT = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "FERTH":
                                                            if (reader.Read())
                                                            {
                                                                _item.PROD_INSP_MEMO = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "ABGRU":
                                                            if (reader.Read())
                                                            {
                                                                _item.REJECTION_CODE = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "BEZEI_REJ":
                                                            if (reader.Read())
                                                            {
                                                                _item.REJECTION_DESCRIPTION = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "BEZEI_PORT":
                                                            if (reader.Read())
                                                            {
                                                                _item.PORT = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "ZZVIA":
                                                            if (reader.Read())
                                                            {
                                                                _item.VIA = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "BEZEI_TRAN":
                                                            if (reader.Read())
                                                            {
                                                                _item.IN_TRANSIT_TO = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "ZZBAND_ID":
                                                            if (reader.Read())
                                                            {
                                                                _item.BRAND_ID = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "ZBAND_DESC":
                                                            if (reader.Read())
                                                            {
                                                                _item.BRAND_DESCRIPTION = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "ZZADDI_BRAND_ID":
                                                            if (reader.Read())
                                                            {
                                                                _item.ADDITIONAL_BRAND_ID = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "ZADDI_BRAND_DESC":
                                                            if (reader.Read())
                                                            {
                                                                _item.ADDITIONAL_BRAND_DESCRIPTION = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "ZZWERKS1":
                                                            if (reader.Read())
                                                            {
                                                                _item.PRODUCTION_PLANT = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "ROUTE":
                                                            if (reader.Read())
                                                            {
                                                                _item.ZONE = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "COUNTRY":
                                                            if (reader.Read())
                                                            {
                                                                _item.COUNTRY = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "PRODH":
                                                            if (reader.Read())
                                                            {
                                                                _item.PRODUCTION_HIERARCHY = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "DISPO":
                                                            if (reader.Read())
                                                            {
                                                                _item.MRP_CONTROLLER = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "ITEM_STOCK":
                                                            if (reader.Read())
                                                            {
                                                                _item.STOCK = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "KDMAT":
                                                            if (reader.Read())
                                                            {
                                                                _item.ITEM_CUSTOM_1 = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "ITEM_CUSTOM_2":
                                                            if (reader.Read())
                                                            {
                                                                _item.ITEM_CUSTOM_2 = reader.Value.Trim();
                                                            }
                                                            break;
                                                        case "ITEM_CUSTOM_3":
                                                            if (reader.Read())
                                                            {
                                                                _item.ITEM_CUSTOM_3 = reader.Value.Trim();

                                                                // _item.PO_COMPLETE_SO_HEADER_ID = _header.PO_COMPLETE_SO_HEADER_ID;

                                                            }
                                                            break;
                                                            //case "COMPONENTS":
                                                            //    _listItem.Add(_item);
                                                            //    _item = new SAP_M_PO_COMPLETE_SO_ITEM();

                                                            //    if (reader.Read())
                                                            //    {
                                                            //        SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT _component = new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT();

                                                            //        while (reader.Read())
                                                            //        {

                                                            //            if (reader.IsStartElement())
                                                            //            {
                                                            //                switch (reader.Name)
                                                            //                {
                                                            //                    case "COM_ITEM_NO":
                                                            //                        if (reader.Read())
                                                            //                        {
                                                            //                            _component.COMPONENT_ITEM = reader.Value.Trim();
                                                            //                        }
                                                            //                        break;
                                                            //                    case "COM_MAT":
                                                            //                        if (reader.Read())
                                                            //                        {
                                                            //                            _component.COMPONENT_MATERIAL = reader.Value.Trim();
                                                            //                        }
                                                            //                        break;
                                                            //                    case "COM_MAT_DESC":
                                                            //                        if (reader.Read())
                                                            //                        {
                                                            //                            _component.DECRIPTION = reader.Value.Trim();
                                                            //                        }
                                                            //                        break;
                                                            //                    case "COM_QUANTITY":
                                                            //                        if (reader.Read())
                                                            //                        {
                                                            //                            _component.QUANTITY = Convert.ToDecimal(reader.Value.Trim());
                                                            //                        }
                                                            //                        break;
                                                            //                    case "COM_UNIT":
                                                            //                        if (reader.Read())
                                                            //                        {
                                                            //                            _component.UNIT = reader.Value.Trim();
                                                            //                        }
                                                            //                        break;
                                                            //                    case "COM_STOCK":
                                                            //                        if (reader.Read())
                                                            //                        {
                                                            //                            _component.STOCK = reader.Value.Trim();
                                                            //                        }
                                                            //                        break;
                                                            //                    case "COM_CUSTOM_1":
                                                            //                        if (reader.Read())
                                                            //                        {
                                                            //                            _component.BOM_ITEM_CUSTOM_1 = reader.Value.Trim();
                                                            //                        }
                                                            //                        break;
                                                            //                    case "COM_CUSTOM_2":
                                                            //                        if (reader.Read())
                                                            //                        {
                                                            //                            _component.BOM_ITEM_CUSTOM_2 = reader.Value.Trim();
                                                            //                        }
                                                            //                        break;
                                                            //                    case "COM_CUSTOM_3":
                                                            //                        if (reader.Read())
                                                            //                        {
                                                            //                            _component.BOM_ITEM_CUSTOM_3 = reader.Value.Trim();

                                                            //                            _listComponent.Add(_component);

                                                            //                            _component = new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT();
                                                            //                        }
                                                            //                        break;
                                                            //                }
                                                            //            }
                                                            //        }
                                                            //    }
                                                            //    break;
                                                    }

                                                }
                                            }

                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [TestMethod()]
        public void SavePOCompleteSOTest_2()
        {
            //string fileName = "SO.xml"; //"SO_20180917_104854.xml";
            string fileName = "SO_20180917_104854.xml";
            StringBuilder SB = new StringBuilder();

            // Set a variable to the My Documents path.
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = "";

            path = mydocpath + "\\" + fileName;
            try
            {
                //var xmlString = File.ReadAllText(path);
                //var stringReader = new StringReader(xmlString);
                //var dsSet = new DataSet();
                //dsSet.ReadXml(stringReader);

                using (XmlReader reader = XmlReader.Create(path))
                {
                    SAP_M_PO_COMPLETE_SO_HEADER _header = new SAP_M_PO_COMPLETE_SO_HEADER();
                    List<SAP_M_PO_COMPLETE_SO_HEADER> _listHeader = new List<SAP_M_PO_COMPLETE_SO_HEADER>();
                    List<SAP_M_PO_COMPLETE_SO_ITEM> _listItem = new List<SAP_M_PO_COMPLETE_SO_ITEM>();
                    List<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT> _listComponent = new List<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT>();

                    SAP_M_PO_COMPLETE_SO_ITEM _item = new SAP_M_PO_COMPLETE_SO_ITEM();

                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            // Get element name and switch on it.
                            switch (reader.Name)
                            {
                                case "VBELN":
                                    if (reader.Read())
                                    {
                                        _header.SALES_ORDER_NO = reader.Value.Trim();
                                    }
                                    break;

                                case "KUNNR_SOLD":
                                    if (reader.Read())
                                    {
                                        _header.SOLD_TO = reader.Value.Trim();
                                    }
                                    break;

                                case "KUNNR_SOLD_NAME":
                                    if (reader.Read())
                                    {
                                        _header.SOLD_TO_NAME = reader.Value.Trim();
                                    }
                                    break;

                                case "AYDAT":
                                    if (reader.Read())
                                    {
                                        _header.LC_NO = reader.Value.Trim();
                                    }
                                    break;

                                case "AHDAT":
                                    if (reader.Read())
                                    {
                                        _header.LAST_SHIPMENT_DATE = Convert.ToDecimal(reader.Value.Trim());
                                    }
                                    break;

                                case "ERDAT":
                                    if (reader.Read())
                                    {
                                        // _header.CREATE_ON = Convert.ToDateTime(reader.Value.Trim());
                                    }
                                    break;

                                case "VDATU":
                                    if (reader.Read())
                                    {
                                        // _header.date = reader.Value.Trim();
                                    }
                                    break;

                                case "VTEXT":
                                    if (reader.Read())
                                    {
                                        _header.PAYMENT_TERM = reader.Value.Trim();
                                    }
                                    break;

                                case "BAANR":
                                    if (reader.Read())
                                    {
                                        _header.LC_NO = reader.Value.Trim();
                                    }
                                    break;

                                case "AXDAT":
                                    if (reader.Read())
                                    {
                                        _header.EXPIRED_DATE = Convert.ToDecimal(reader.Value.Trim());
                                    }
                                    break;

                                case "KUNNR_SHIP":
                                    if (reader.Read())
                                    {
                                        _header.SHIP_TO = reader.Value.Trim();
                                    }
                                    break;

                                case "KUNNR_SHIP_NAME":
                                    if (reader.Read())
                                    {
                                        _header.SHIP_TO_NAME = reader.Value.Trim();
                                    }
                                    break;

                                case "BSTKD":
                                    if (reader.Read())
                                    {
                                        _header.SOLD_TO_PO = reader.Value.Trim();
                                    }
                                    break;
                                case "BSTKD_E":
                                    if (reader.Read())
                                    {
                                        _header.SHIP_TO_PO = reader.Value.Trim();
                                    }
                                    break;

                                case "VKGRP":
                                    if (reader.Read())
                                    {
                                        _header.SALES_GROUP = reader.Value.Trim();
                                    }
                                    break;

                                case "KUNNR_MAR_CO":
                                    if (reader.Read())
                                    {
                                        _header.MARKETING_CO = reader.Value.Trim();
                                    }
                                    break;

                                case "KUNNR_MAR_CO_NAME":
                                    if (reader.Read())
                                    {
                                        _header.MARKETING_CO_NAME = reader.Value.Trim();
                                    }
                                    break;

                                case "KUNNR_MAR":
                                    if (reader.Read())
                                    {
                                        _header.MARKETING_CO = reader.Value.Trim();
                                    }
                                    break;

                                case "KUNNR_MAR_NAME":
                                    if (reader.Read())
                                    {
                                        _header.MARKETING_CO_NAME = reader.Value.Trim();
                                    }
                                    break;

                                case "KUNNR_MAR_ORD":
                                    if (reader.Read())
                                    {
                                        _header.MARKETING_ORDER_SAP = reader.Value.Trim();
                                    }
                                    break;

                                case "KUNNR_MAR_ORD_NAME":
                                    if (reader.Read())
                                    {
                                        _header.MARKETING_ORDER_SAP_NAME = reader.Value.Trim();
                                    }
                                    break;

                                case "VKORG":
                                    if (reader.Read())
                                    {
                                        _header.SALES_ORG = reader.Value.Trim();
                                    }
                                    break;

                                case "VTWEG":
                                    if (reader.Read())
                                    {
                                        _header.DISTRIBUTION_CHANNEL = reader.Value.Trim();
                                    }
                                    break;

                                case "SPART":
                                    if (reader.Read())
                                    {
                                        _header.DIVITION = reader.Value.Trim();
                                    }
                                    break;

                                case "AUART":
                                    if (reader.Read())
                                    {
                                        _header.SALES_ORDER_TYPE = reader.Value.Trim();
                                    }
                                    break;

                                case "HEADER_CUSTOM_1":
                                    if (reader.Read())
                                    {
                                        _header.HEADER_CUSTOM_1 = reader.Value.Trim();
                                    }
                                    break;

                                case "HEADER_CUSTOM_2":
                                    if (reader.Read())
                                    {
                                        _header.HEADER_CUSTOM_2 = reader.Value.Trim();
                                    }
                                    break;

                                case "HEADER_CUSTOM_3":
                                    if (reader.Read())
                                    {
                                        _header.HEADER_CUSTOM_3 = reader.Value.Trim();


                                        _listHeader.Add(_header);
                                        _header = new SAP_M_PO_COMPLETE_SO_HEADER();
                                    }
                                    break;
                                case "POSNR":
                                    if (reader.Read())
                                    {

                                        _item.ITEM = Convert.ToDecimal(reader.Value);
                                    }
                                    break;
                                case "ARKTX":
                                    if (reader.Read())
                                    {
                                        _item.MATERIAL_DESCRIPTION = reader.Value.Trim();
                                    }
                                    break;
                                case "MATNR":
                                    if (reader.Read())
                                    {
                                        _item.PRODUCT_CODE = reader.Value.Trim();
                                    }
                                    break;
                                case "ATWRT_NET":
                                    if (reader.Read())
                                    {
                                        _item.NET_WEIGHT = reader.Value.Trim();
                                    }
                                    break;
                                case "KWMENG":
                                    if (reader.Read())
                                    {
                                        _item.ORDER_QTY = Convert.ToDecimal(reader.Value.Trim());
                                    }
                                    break;
                                case "VRKME":
                                    if (reader.Read())
                                    {
                                        _item.ORDER_UNIT = reader.Value.Trim();
                                    }
                                    break;

                                case "ZZETD_FR":
                                    if (reader.Read())
                                    {
                                        //_item.ETD_DATE_FROM = Convert.ToDateTime(reader.Value.Trim());
                                    }
                                    break;
                                case "ZZETD_TO":
                                    if (reader.Read())
                                    {
                                        // _item.ETD_DATE_TO = Convert.ToDateTime(reader.Value.Trim());
                                    }
                                    break;
                                case "WERKS":
                                    if (reader.Read())
                                    {
                                        _item.PLANT = reader.Value.Trim();
                                    }
                                    break;
                                case "BISMT":
                                    if (reader.Read())
                                    {
                                        _item.OLD_MATERIAL_CODE = reader.Value.Trim();
                                    }
                                    break;
                                case "ATWRT_PACK":
                                    if (reader.Read())
                                    {
                                        _item.PACK_SIZE = reader.Value.Trim();
                                    }
                                    break;
                                case "VOLUMN":
                                    if (reader.Read())
                                    {
                                        _item.VALUME_PER_UNIT = reader.Value.Trim();
                                    }
                                    break;
                                case "VOLEH":
                                    if (reader.Read())
                                    {
                                        _item.VALUME_UNIT = reader.Value.Trim();
                                    }
                                    break;
                                case "GROES":
                                    if (reader.Read())
                                    {
                                        _item.SIZE_DRAIN_WT = reader.Value.Trim();
                                    }
                                    break;
                                case "FERTH":
                                    if (reader.Read())
                                    {
                                        _item.PROD_INSP_MEMO = reader.Value.Trim();
                                    }
                                    break;
                                case "ABGRU":
                                    if (reader.Read())
                                    {
                                        _item.REJECTION_CODE = reader.Value.Trim();
                                    }
                                    break;
                                case "BEZEI_REJ":
                                    if (reader.Read())
                                    {
                                        _item.REJECTION_DESCRIPTION = reader.Value.Trim();
                                    }
                                    break;
                                case "BEZEI_PORT":
                                    if (reader.Read())
                                    {
                                        _item.PORT = reader.Value.Trim();
                                    }
                                    break;
                                case "ZZVIA":
                                    if (reader.Read())
                                    {
                                        _item.VIA = reader.Value.Trim();
                                    }
                                    break;
                                case "BEZEI_TRAN":
                                    if (reader.Read())
                                    {
                                        _item.IN_TRANSIT_TO = reader.Value.Trim();
                                    }
                                    break;
                                case "ZZBAND_ID":
                                    if (reader.Read())
                                    {
                                        _item.BRAND_ID = reader.Value.Trim();
                                    }
                                    break;
                                case "ZBAND_DESC":
                                    if (reader.Read())
                                    {
                                        _item.BRAND_DESCRIPTION = reader.Value.Trim();
                                    }
                                    break;
                                case "ZZADDI_BRAND_ID":
                                    if (reader.Read())
                                    {
                                        _item.ADDITIONAL_BRAND_ID = reader.Value.Trim();
                                    }
                                    break;
                                case "ZADDI_BRAND_DESC":
                                    if (reader.Read())
                                    {
                                        _item.ADDITIONAL_BRAND_DESCRIPTION = reader.Value.Trim();
                                    }
                                    break;
                                case "ZZWERKS1":
                                    if (reader.Read())
                                    {
                                        _item.PRODUCTION_PLANT = reader.Value.Trim();
                                    }
                                    break;
                                case "ROUTE":
                                    if (reader.Read())
                                    {
                                        _item.ZONE = reader.Value.Trim();
                                    }
                                    break;
                                case "COUNTRY":
                                    if (reader.Read())
                                    {
                                        _item.COUNTRY = reader.Value.Trim();
                                    }
                                    break;
                                case "PRODH":
                                    if (reader.Read())
                                    {
                                        _item.PRODUCTION_HIERARCHY = reader.Value.Trim();
                                    }
                                    break;
                                case "DISPO":
                                    if (reader.Read())
                                    {
                                        _item.MRP_CONTROLLER = reader.Value.Trim();
                                    }
                                    break;
                                case "ITEM_STOCK":
                                    if (reader.Read())
                                    {
                                        _item.STOCK = reader.Value.Trim();
                                    }
                                    break;
                                case "KDMAT":
                                    if (reader.Read())
                                    {
                                        _item.ITEM_CUSTOM_1 = reader.Value.Trim();
                                    }
                                    break;
                                case "ITEM_CUSTOM_2":
                                    if (reader.Read())
                                    {
                                        _item.ITEM_CUSTOM_2 = reader.Value.Trim();
                                    }
                                    break;
                                case "ITEM_CUSTOM_3":
                                    if (reader.Read())
                                    {
                                        _item.ITEM_CUSTOM_3 = reader.Value.Trim();

                                        // _item.PO_COMPLETE_SO_HEADER_ID = _header.PO_COMPLETE_SO_HEADER_ID;
                                        _listItem.Add(_item);
                                        _item = new SAP_M_PO_COMPLETE_SO_ITEM();
                                    }
                                    break;
                                    //case "COMPONENTS":
                                    //    _listItem.Add(_item);
                                    //    _item = new SAP_M_PO_COMPLETE_SO_ITEM();

                                    //    if (reader.Read())
                                    //    {
                                    //        SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT _component = new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT();

                                    //        while (reader.Read())
                                    //        {

                                    //            if (reader.IsStartElement())
                                    //            {
                                    //                switch (reader.Name)
                                    //                {
                                    //                    case "COM_ITEM_NO":
                                    //                        if (reader.Read())
                                    //                        {
                                    //                            _component.COMPONENT_ITEM = reader.Value.Trim();
                                    //                        }
                                    //                        break;
                                    //                    case "COM_MAT":
                                    //                        if (reader.Read())
                                    //                        {
                                    //                            _component.COMPONENT_MATERIAL = reader.Value.Trim();
                                    //                        }
                                    //                        break;
                                    //                    case "COM_MAT_DESC":
                                    //                        if (reader.Read())
                                    //                        {
                                    //                            _component.DECRIPTION = reader.Value.Trim();
                                    //                        }
                                    //                        break;
                                    //                    case "COM_QUANTITY":
                                    //                        if (reader.Read())
                                    //                        {
                                    //                            _component.QUANTITY = Convert.ToDecimal(reader.Value.Trim());
                                    //                        }
                                    //                        break;
                                    //                    case "COM_UNIT":
                                    //                        if (reader.Read())
                                    //                        {
                                    //                            _component.UNIT = reader.Value.Trim();
                                    //                        }
                                    //                        break;
                                    //                    case "COM_STOCK":
                                    //                        if (reader.Read())
                                    //                        {
                                    //                            _component.STOCK = reader.Value.Trim();
                                    //                        }
                                    //                        break;
                                    //                    case "COM_CUSTOM_1":
                                    //                        if (reader.Read())
                                    //                        {
                                    //                            _component.BOM_ITEM_CUSTOM_1 = reader.Value.Trim();
                                    //                        }
                                    //                        break;
                                    //                    case "COM_CUSTOM_2":
                                    //                        if (reader.Read())
                                    //                        {
                                    //                            _component.BOM_ITEM_CUSTOM_2 = reader.Value.Trim();
                                    //                        }
                                    //                        break;
                                    //                    case "COM_CUSTOM_3":
                                    //                        if (reader.Read())
                                    //                        {
                                    //                            _component.BOM_ITEM_CUSTOM_3 = reader.Value.Trim();

                                    //                            _listComponent.Add(_component);

                                    //                            _component = new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT();
                                    //                        }
                                    //                        break;
                                    //                }
                                    //            }
                                    //        }
                                    //    }
                                    //    break;
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }


}