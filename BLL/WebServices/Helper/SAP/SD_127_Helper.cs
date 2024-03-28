using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Model;
using BLL.Services;
using WebServices.Model;
using DAL;
using System.Data.Entity;
using BLL.Helpers;
using System.Globalization;
using System.Web.Script.Serialization;
using System.IO;
using System.Xml.Serialization;

namespace WebServices.Helper
{
    public class SD_127_Helper
    {
        public static void SaveLog(SAP_M_PO_COMPLETE_SO_MODEL param, string GUID)
        {
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    ART_SYS_LOG Item = new ART_SYS_LOG();
                    Item.ACTION = "Interface Inbound-SD127";
                    Item.TABLE_NAME = "SAP_M_PO_COMPLETE_SO_TMP";
                    if (param.SO_HEADERS != null) Item.NEW_VALUE = CNService.SubString(CNService.Serialize(param.SO_HEADERS), 4000);
                    Item.UPDATE_DATE = DateTime.Now;
                    Item.UPDATE_BY = -2;
                    Item.CREATE_DATE = DateTime.Now;
                    Item.CREATE_BY = -2;
                    Item.OLD_VALUE = GUID;
                    context.ART_SYS_LOG.Add(Item);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }

        public static SERVICE_RESULT_MODEL SavePOCompleteSO(SAP_M_PO_COMPLETE_SO_MODEL param)
        {
            return aSavePOCompleteSO(param);
        }
        private static SERVICE_RESULT_MODEL aSavePOCompleteSO(SAP_M_PO_COMPLETE_SO_MODEL param)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();
            Results.start = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string guid = Guid.NewGuid().ToString();
            SaveLog(param, guid);  // aof open commented
            int userID = -2;
            int soHeaderID = 0;
            int soItemID = 0;
            string dateFormat = "yyyyMMdd";
            if (param != null)
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        context.Database.CommandTimeout = 600;

                        foreach (SO_HEADER itemSOHeader in param.SO_HEADERS)
                        {
                            var soHeader = new SAP_M_PO_COMPLETE_SO_HEADER_TMP();
                            soHeader.SALES_ORDER_NO = itemSOHeader.SALES_ORDER_NO;

                            if (!String.IsNullOrEmpty(itemSOHeader.LAST_SHIPMENT_DATE))
                            {
                                decimal decOut;
                                if (Decimal.TryParse(itemSOHeader.LAST_SHIPMENT_DATE, out decOut))
                                {
                                    soHeader.LAST_SHIPMENT_DATE = decOut;
                                }
                            }

                            if (!String.IsNullOrEmpty(itemSOHeader.DATE_1_2))
                            {
                                decimal decOut;
                                if (Decimal.TryParse(itemSOHeader.DATE_1_2, out decOut))
                                {
                                    soHeader.DATE_1_2 = decOut;
                                }
                            }

                            if (!String.IsNullOrEmpty(itemSOHeader.CREATE_ON))
                            {
                                itemSOHeader.CREATE_ON = itemSOHeader.CREATE_ON.Replace("-", "");
                                DateTime decOut;
                                if (DateTime.TryParseExact(itemSOHeader.CREATE_ON,
                                      dateFormat,
                                      CultureInfo.InvariantCulture,
                                      DateTimeStyles.None, out decOut))
                                {
                                    soHeader.CREATE_ON = decOut;
                                }
                            }

                            if (!String.IsNullOrEmpty(itemSOHeader.RDD))
                            {
                                itemSOHeader.RDD = itemSOHeader.RDD.Replace("-", "");
                                DateTime decOut;
                                if (DateTime.TryParseExact(itemSOHeader.RDD,
                                      dateFormat,
                                      CultureInfo.InvariantCulture,
                                      DateTimeStyles.None, out decOut))
                                {
                                    soHeader.RDD = decOut;
                                }
                            }

                            if (!String.IsNullOrEmpty(itemSOHeader.EXPIRED_DATE))
                            {
                                decimal decOut;
                                if (Decimal.TryParse(itemSOHeader.EXPIRED_DATE, out decOut))
                                {
                                    soHeader.EXPIRED_DATE = decOut;
                                }
                            }
                            soHeader.SOLD_TO = itemSOHeader.SOLD_TO;
                            soHeader.SOLD_TO_NAME = itemSOHeader.SOLD_TO_NAME;
                            soHeader.PAYMENT_TERM = itemSOHeader.PAYMENT_TERM;
                            soHeader.LC_NO = itemSOHeader.LC_NO;
                            soHeader.SHIP_TO = itemSOHeader.SHIP_TO;
                            soHeader.SHIP_TO_NAME = itemSOHeader.SHIP_TO_NAME;
                            soHeader.SOLD_TO_PO = itemSOHeader.SOLD_TO_PO;
                            soHeader.SHIP_TO_PO = itemSOHeader.SHIP_TO_PO;
                            soHeader.SALES_GROUP = itemSOHeader.SALES_GROUP;
                            soHeader.MARKETING_CO = itemSOHeader.MARKETING_CO;
                            soHeader.MARKETING_CO_NAME = itemSOHeader.MARKETING_CO_NAME;
                            soHeader.MARKETING = itemSOHeader.MARKETING;
                            soHeader.MARKETING_NAME = itemSOHeader.MARKETING_NAME;
                            soHeader.MARKETING_ORDER_SAP = itemSOHeader.MARKETING_ORDER_SAP;
                            soHeader.MARKETING_ORDER_SAP_NAME = itemSOHeader.MARKETING_ORDER_SAP_NAME;
                            soHeader.SALES_ORG = itemSOHeader.SALES_ORG;
                            soHeader.DISTRIBUTION_CHANNEL = itemSOHeader.DISTRIBUTION_CHANNEL;
                            soHeader.DIVITION = itemSOHeader.DIVITION;
                            soHeader.SALES_ORDER_TYPE = itemSOHeader.SALES_ORDER_TYPE;
                            soHeader.HEADER_CUSTOM_1 = itemSOHeader.HEADER_CUSTOM_1;
                            soHeader.HEADER_CUSTOM_2 = itemSOHeader.HEADER_CUSTOM_2;
                            soHeader.HEADER_CUSTOM_3 = itemSOHeader.HEADER_CUSTOM_3;
                            soHeader.CREATE_BY = userID;
                            soHeader.UPDATE_BY = userID;
                            SAP_M_PO_COMPLETE_SO_HEADER_TMP_SERVICE.SaveNoLog(soHeader, context);
                            soHeaderID = soHeader.PO_COMPLETE_SO_HEADER_ID;
                            foreach (SO_ITEM itemSOItem in itemSOHeader.SO_ITEMS)
                            {
                                var soItem = new SAP_M_PO_COMPLETE_SO_ITEM_TMP();
                                soItem.PO_COMPLETE_SO_HEADER_ID = soHeaderID;
                                if (!String.IsNullOrEmpty(itemSOItem.ITEM))
                                {
                                    decimal decOut;
                                    if (Decimal.TryParse(itemSOItem.ITEM, out decOut))
                                    {
                                        soItem.ITEM = decOut;
                                    }
                                }

                                if (!String.IsNullOrEmpty(itemSOItem.ORDER_QTY))
                                {
                                    decimal decOut;
                                    if (Decimal.TryParse(itemSOItem.ORDER_QTY, out decOut))
                                    {
                                        soItem.ORDER_QTY = decOut;
                                    }
                                }

                                if (!String.IsNullOrEmpty(itemSOItem.ETD_DATE_FROM))
                                {
                                    itemSOItem.ETD_DATE_FROM = itemSOItem.ETD_DATE_FROM.Replace("-", "");
                                    DateTime decOut;
                                    if (DateTime.TryParseExact(itemSOItem.ETD_DATE_FROM,
                                            dateFormat,
                                            CultureInfo.InvariantCulture,
                                            DateTimeStyles.None, out decOut))
                                    {
                                        soItem.ETD_DATE_FROM = decOut;
                                    }
                                }

                                if (!String.IsNullOrEmpty(itemSOItem.ETD_DATE_TO))
                                {
                                    itemSOItem.ETD_DATE_TO = itemSOItem.ETD_DATE_TO.Replace("-", "");
                                    DateTime decOut;
                                    if (DateTime.TryParseExact(itemSOItem.ETD_DATE_TO,
                                         dateFormat,
                                         CultureInfo.InvariantCulture,
                                         DateTimeStyles.None, out decOut))
                                    {
                                        soItem.ETD_DATE_TO = decOut;
                                    }
                                }
                                soItem.PRODUCT_CODE = itemSOItem.PRODUCT_CODE;
                                soItem.MATERIAL_DESCRIPTION = itemSOItem.MATERIAL_DESCRIPTION;
                                soItem.NET_WEIGHT = itemSOItem.NET_WEIGHT;
                                soItem.ORDER_UNIT = itemSOItem.ORDER_UNIT;
                                soItem.PLANT = itemSOItem.PLANT;
                                soItem.OLD_MATERIAL_CODE = itemSOItem.OLD_MATERIAL_CODE;
                                soItem.PACK_SIZE = itemSOItem.PACK_SIZE;
                                soItem.VALUME_PER_UNIT = itemSOItem.VALUME_PER_UNIT;
                                soItem.VALUME_UNIT = itemSOItem.VALUME_UNIT;
                                soItem.SIZE_DRAIN_WT = itemSOItem.SIZE_DRAIN_WT;
                                soItem.PROD_INSP_MEMO = itemSOItem.PROD_INSP_MEMO;
                                soItem.REJECTION_CODE = itemSOItem.REJECTION_CODE;
                                soItem.REJECTION_DESCRIPTION = itemSOItem.REJECTION_DESCRIPTION;
                                soItem.PORT = itemSOItem.PORT;
                                soItem.VIA = itemSOItem.VIA;
                                soItem.IN_TRANSIT_TO = itemSOItem.IN_TRANSIT_TO;
                                soItem.BRAND_ID = itemSOItem.BRAND_ID;
                                soItem.BRAND_DESCRIPTION = itemSOItem.BRAND_DESCRIPTION;
                                soItem.ADDITIONAL_BRAND_ID = itemSOItem.ADDITIONAL_BRAND_ID;
                                soItem.ADDITIONAL_BRAND_DESCRIPTION = itemSOItem.ADDITIONAL_BRAND_DESCRIPTION;
                                soItem.PRODUCTION_PLANT = itemSOItem.PRODUCTION_PLANT;

                                soItem.ZONE = itemSOItem.ZONE;
                                soItem.COUNTRY = itemSOItem.COUNTRY;
                                soItem.PRODUCTION_HIERARCHY = itemSOItem.PRODUCTION_HIERARCHY;
                                soItem.MRP_CONTROLLER = itemSOItem.MRP_CONTROLLER;
                                soItem.STOCK = itemSOItem.STOCK;
                                soItem.ITEM_CUSTOM_1 = itemSOItem.ITEM_CUSTOM_1;
                                soItem.ITEM_CUSTOM_2 = itemSOItem.ITEM_CUSTOM_2;
                                soItem.ITEM_CUSTOM_3 = itemSOItem.ITEM_CUSTOM_3;
                                soItem.CREATE_BY = userID;
                                soItem.UPDATE_BY = userID;
                                SAP_M_PO_COMPLETE_SO_ITEM_TMP_SERVICE.SaveNoLog(soItem, context);
                                soItemID = soItem.PO_COMPLETE_SO_ITEM_ID;
                                foreach (COMPONENT itemSOComponent in itemSOItem.COMPONENTS)
                                {
                                    var soItemComponent = new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_TMP();
                                    soItemComponent.PO_COMPLETE_SO_ITEM_ID = soItemID;
                                    soItemComponent.COMPONENT_ITEM = itemSOComponent.COMPONENT_ITEM;
                                    soItemComponent.COMPONENT_MATERIAL = itemSOComponent.COMPONENT_MATERIAL;
                                    soItemComponent.DECRIPTION = itemSOComponent.DECRIPTION;

                                    if (!String.IsNullOrEmpty(itemSOComponent.QUANTITY))
                                    {
                                        decimal decOut;
                                        if (Decimal.TryParse(itemSOComponent.QUANTITY, out decOut))
                                        {
                                            soItemComponent.QUANTITY = decOut;
                                        }
                                    }

                                    soItemComponent.UNIT = itemSOComponent.UNIT;
                                    soItemComponent.STOCK = itemSOComponent.STOCK;
                                    soItemComponent.BOM_ITEM_CUSTOM_1 = itemSOComponent.BOM_ITEM_CUSTOM_1;
                                    soItemComponent.BOM_ITEM_CUSTOM_2 = itemSOComponent.BOM_ITEM_CUSTOM_2;
                                    soItemComponent.BOM_ITEM_CUSTOM_3 = itemSOComponent.BOM_ITEM_CUSTOM_3;
                                    soItemComponent.CREATE_BY = userID;
                                    soItemComponent.UPDATE_BY = userID;

                                    SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_TMP_SERVICE.SaveNoLog(soItemComponent, context);
                                }
                            }
                        }
                    }
                    Results.status = "S";
                    Results.msg = MessageHelper.GetMessage("MSG_001");
                }
                catch (Exception ex)
                {
                    Results.cnt = 0;
                    Results.status = "E";
                    Results.msg = CNService.GetErrorMessage(ex);
                }
            }

            Results.finish = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            CNService.SaveLogReturnInterface(Results, "SAP_M_PO_COMPLETE_SO_TMP", guid,"SD127");

            return Results;
        }
        public static SERVICE_RESULT_MODEL aSavePOCompleteSOtmp(SAP_M_PO_COMPLETE_SO_MODEL param)
        {
            //xml
            //string datapath = "~/FilePath/SALES_ORDER_NO" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
            //string path = System.Web.HttpContext.Current.Server.MapPath(datapath);
            //using (FileStream fs = new FileStream(path, FileMode.Create))
            //{
            //    new XmlSerializer(typeof(WebServices.Model.SAP_M_PO_COMPLETE_SO_MODEL)).Serialize(fs, param);
            //}
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();
            Results.start = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            string guid = Guid.NewGuid().ToString();
            SaveLog(param, guid);

            int userID = -2;
            int soHeaderID = 0;
            int soItemID = 0;
            string dateFormat = "yyyyMMdd";
         
            if (param != null)
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        context.Database.CommandTimeout = 600;

                        foreach (SO_HEADER itemSOHeader in param.SO_HEADERS)
                        {
                            var soHeader = new SAP_M_PO_COMPLETE_SO_HEADER();

                            soHeader.SALES_ORDER_NO = itemSOHeader.SALES_ORDER_NO;
                            soHeader.SOLD_TO = itemSOHeader.SOLD_TO;
                            soHeader.SOLD_TO_NAME = itemSOHeader.SOLD_TO_NAME;

                            if (!String.IsNullOrEmpty(itemSOHeader.LAST_SHIPMENT_DATE))
                            {
                                decimal decOut;
                                if (Decimal.TryParse(itemSOHeader.LAST_SHIPMENT_DATE, out decOut))
                                {
                                    soHeader.LAST_SHIPMENT_DATE = decOut;
                                }
                            }

                            if (!String.IsNullOrEmpty(itemSOHeader.DATE_1_2))
                            {
                                decimal decOut;
                                if (Decimal.TryParse(itemSOHeader.DATE_1_2, out decOut))
                                {
                                    soHeader.DATE_1_2 = decOut;
                                }
                            }

                            if (!String.IsNullOrEmpty(itemSOHeader.CREATE_ON))
                            {
                                itemSOHeader.CREATE_ON = itemSOHeader.CREATE_ON.Replace("-", "");
                                DateTime decOut;
                                if (DateTime.TryParseExact(itemSOHeader.CREATE_ON,
                                      dateFormat,
                                      CultureInfo.InvariantCulture,
                                      DateTimeStyles.None, out decOut))
                                {
                                    soHeader.CREATE_ON = decOut;
                                }
                            }

                            if (!String.IsNullOrEmpty(itemSOHeader.RDD))
                            {
                                itemSOHeader.RDD = itemSOHeader.RDD.Replace("-", "");
                                DateTime decOut;
                                if (DateTime.TryParseExact(itemSOHeader.RDD,
                                      dateFormat,
                                      CultureInfo.InvariantCulture,
                                      DateTimeStyles.None, out decOut))
                                {
                                    soHeader.RDD = decOut;
                                }
                            }

                            if (!String.IsNullOrEmpty(itemSOHeader.EXPIRED_DATE))
                            {
                                decimal decOut;
                                if (Decimal.TryParse(itemSOHeader.EXPIRED_DATE, out decOut))
                                {
                                    soHeader.EXPIRED_DATE = decOut;
                                }
                            }

                            soHeader.PAYMENT_TERM = itemSOHeader.PAYMENT_TERM;
                            soHeader.LC_NO = itemSOHeader.LC_NO;
                            soHeader.SHIP_TO = itemSOHeader.SHIP_TO;
                            soHeader.SHIP_TO_NAME = itemSOHeader.SHIP_TO_NAME;
                            soHeader.SOLD_TO_PO = itemSOHeader.SOLD_TO_PO;
                            soHeader.SHIP_TO_PO = itemSOHeader.SHIP_TO_PO;
                            soHeader.SALES_GROUP = itemSOHeader.SALES_GROUP;
                            soHeader.MARKETING_CO = itemSOHeader.MARKETING_CO;
                            soHeader.MARKETING_CO_NAME = itemSOHeader.MARKETING_CO_NAME;
                            soHeader.MARKETING = itemSOHeader.MARKETING;
                            soHeader.MARKETING_NAME = itemSOHeader.MARKETING_NAME;
                            soHeader.MARKETING_ORDER_SAP = itemSOHeader.MARKETING_ORDER_SAP;
                            soHeader.MARKETING_ORDER_SAP_NAME = itemSOHeader.MARKETING_ORDER_SAP_NAME;
                            soHeader.SALES_ORG = itemSOHeader.SALES_ORG;
                            soHeader.DISTRIBUTION_CHANNEL = itemSOHeader.DISTRIBUTION_CHANNEL;
                            soHeader.DIVITION = itemSOHeader.DIVITION;
                            soHeader.SALES_ORDER_TYPE = itemSOHeader.SALES_ORDER_TYPE;
                            soHeader.HEADER_CUSTOM_1 = itemSOHeader.HEADER_CUSTOM_1;
                            soHeader.HEADER_CUSTOM_2 = itemSOHeader.HEADER_CUSTOM_2;
                            soHeader.HEADER_CUSTOM_3 = itemSOHeader.HEADER_CUSTOM_3;
                            soHeader.CREATE_BY = userID;
                            soHeader.UPDATE_BY = userID;
                            //var chkHeader = SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.GetByItem(new SAP_M_PO_COMPLETE_SO_HEADER() { SALES_ORDER_NO = itemSOHeader.SALES_ORDER_NO }, context).FirstOrDefault();
                            var chkHeader = (from p in context.SAP_M_PO_COMPLETE_SO_HEADER
                                             where p.SALES_ORDER_NO == itemSOHeader.SALES_ORDER_NO
                                             select new SAP_M_PO_COMPLETE_SO_HEADER_2() { CREATE_BY = p.CREATE_BY, CREATE_DATE = p.CREATE_DATE, PO_COMPLETE_SO_HEADER_ID = p.PO_COMPLETE_SO_HEADER_ID, IS_MIGRATION = p.IS_MIGRATION,IS_ASSIGN=p.IS_ASSIGN }).FirstOrDefault();

                            if (chkHeader != null)
                            {
                                soHeader.CREATE_BY = chkHeader.CREATE_BY;
                                soHeader.CREATE_DATE = chkHeader.CREATE_DATE;
                                soHeader.PO_COMPLETE_SO_HEADER_ID = chkHeader.PO_COMPLETE_SO_HEADER_ID;
                                soHeader.IS_MIGRATION = chkHeader.IS_MIGRATION;
                                soHeader.IS_ASSIGN = chkHeader.IS_ASSIGN;
                                SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.UpdateNoLog(soHeader, context);
                                Results.cnt++;
                            }
                            else
                            {
                                
                                SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.SaveNoLog(soHeader, context);
                                Results.cnt++;
                            }

                            soHeaderID = soHeader.PO_COMPLETE_SO_HEADER_ID;

                            var tempSOItem = SAP_M_PO_COMPLETE_SO_ITEM_SERVICE.GetByItem(new SAP_M_PO_COMPLETE_SO_ITEM() { PO_COMPLETE_SO_HEADER_ID = soHeader.PO_COMPLETE_SO_HEADER_ID }, context);
                            foreach (var item in tempSOItem)
                            {
                                item.IS_ACTIVE = null;
                                SAP_M_PO_COMPLETE_SO_ITEM_SERVICE.UpdateNoLog(item, context);
                            }

                            foreach (SO_ITEM itemSOItem in itemSOHeader.SO_ITEMS)
                            {
                                var soItem = new SAP_M_PO_COMPLETE_SO_ITEM();

                                soItem.PO_COMPLETE_SO_HEADER_ID = soHeaderID;

                                if (!String.IsNullOrEmpty(itemSOItem.ITEM))
                                {
                                    decimal decOut;
                                    if (Decimal.TryParse(itemSOItem.ITEM, out decOut))
                                    {
                                        soItem.ITEM = decOut;
                                    }
                                }

                                if (!String.IsNullOrEmpty(itemSOItem.ORDER_QTY))
                                {
                                    decimal decOut;
                                    if (Decimal.TryParse(itemSOItem.ORDER_QTY, out decOut))
                                    {
                                        soItem.ORDER_QTY = decOut;
                                    }
                                }

                                if (!String.IsNullOrEmpty(itemSOItem.ETD_DATE_FROM))
                                {
                                    itemSOItem.ETD_DATE_FROM = itemSOItem.ETD_DATE_FROM.Replace("-", "");
                                    DateTime decOut;
                                    if (DateTime.TryParseExact(itemSOItem.ETD_DATE_FROM,
                                            dateFormat,
                                            CultureInfo.InvariantCulture,
                                            DateTimeStyles.None, out decOut))
                                    {
                                        soItem.ETD_DATE_FROM = decOut;
                                    }
                                }

                                if (!String.IsNullOrEmpty(itemSOItem.ETD_DATE_TO))
                                {
                                    itemSOItem.ETD_DATE_TO = itemSOItem.ETD_DATE_TO.Replace("-", "");
                                    DateTime decOut;
                                    if (DateTime.TryParseExact(itemSOItem.ETD_DATE_TO,
                                         dateFormat,
                                         CultureInfo.InvariantCulture,
                                         DateTimeStyles.None, out decOut))
                                    {
                                        soItem.ETD_DATE_TO = decOut;
                                    }
                                }

                                soItem.PRODUCT_CODE = itemSOItem.PRODUCT_CODE;
                                soItem.MATERIAL_DESCRIPTION = itemSOItem.MATERIAL_DESCRIPTION;
                                soItem.NET_WEIGHT = itemSOItem.NET_WEIGHT;
                                soItem.ORDER_UNIT = itemSOItem.ORDER_UNIT;
                                soItem.PLANT = itemSOItem.PLANT;
                                soItem.OLD_MATERIAL_CODE = itemSOItem.OLD_MATERIAL_CODE;
                                soItem.PACK_SIZE = itemSOItem.PACK_SIZE;
                                soItem.VALUME_PER_UNIT = itemSOItem.VALUME_PER_UNIT;
                                soItem.VALUME_UNIT = itemSOItem.VALUME_UNIT;
                                soItem.SIZE_DRAIN_WT = itemSOItem.SIZE_DRAIN_WT;
                                soItem.PROD_INSP_MEMO = itemSOItem.PROD_INSP_MEMO;
                                soItem.REJECTION_CODE = itemSOItem.REJECTION_CODE;
                                soItem.REJECTION_DESCRIPTION = itemSOItem.REJECTION_DESCRIPTION;
                                soItem.PORT = itemSOItem.PORT;
                                soItem.VIA = itemSOItem.VIA;
                                soItem.IN_TRANSIT_TO = itemSOItem.IN_TRANSIT_TO;
                                soItem.BRAND_ID = itemSOItem.BRAND_ID;
                                soItem.BRAND_DESCRIPTION = itemSOItem.BRAND_DESCRIPTION;
                                soItem.ADDITIONAL_BRAND_ID = itemSOItem.ADDITIONAL_BRAND_ID;
                                soItem.ADDITIONAL_BRAND_DESCRIPTION = itemSOItem.ADDITIONAL_BRAND_DESCRIPTION;
                                soItem.PRODUCTION_PLANT = itemSOItem.PRODUCTION_PLANT;
                                
                                soItem.ZONE = itemSOItem.ZONE;
                                soItem.COUNTRY = itemSOItem.COUNTRY;
                                soItem.PRODUCTION_HIERARCHY = itemSOItem.PRODUCTION_HIERARCHY;
                                soItem.MRP_CONTROLLER = itemSOItem.MRP_CONTROLLER;
                                soItem.STOCK = itemSOItem.STOCK;
                                soItem.ITEM_CUSTOM_1 = itemSOItem.ITEM_CUSTOM_1;
                                soItem.ITEM_CUSTOM_2 = itemSOItem.ITEM_CUSTOM_2;
                                soItem.ITEM_CUSTOM_3 = itemSOItem.ITEM_CUSTOM_3;
                                soItem.CREATE_BY = userID;
                                soItem.UPDATE_BY = userID;
                                // PIC 
                                //var chkItem = SAP_M_PO_COMPLETE_SO_ITEM_SERVICE.GetByItem(new SAP_M_PO_COMPLETE_SO_ITEM() { PO_COMPLETE_SO_HEADER_ID = soItem.PO_COMPLETE_SO_HEADER_ID, ITEM = soItem.ITEM }, context).FirstOrDefault();
                                var chkItem = (from p in context.SAP_M_PO_COMPLETE_SO_ITEM
                                               where p.PO_COMPLETE_SO_HEADER_ID == soItem.PO_COMPLETE_SO_HEADER_ID
                                               && p.ITEM == soItem.ITEM
                                               select new SAP_M_PO_COMPLETE_SO_ITEM_2() { CREATE_BY = p.CREATE_BY, CREATE_DATE = p.CREATE_DATE, PO_COMPLETE_SO_ITEM_ID = p.PO_COMPLETE_SO_ITEM_ID,IS_ASSIGN = p.IS_ASSIGN }).FirstOrDefault();

                                if (chkItem != null)
                                {
                                    if (string.IsNullOrEmpty(soItem.REJECTION_CODE))
                                        soItem.IS_ACTIVE = "X";
                                    else
                                        soItem.IS_ACTIVE = null;
                                    soItem.IS_ASSIGN = chkItem.IS_ASSIGN;
                                    soItem.CREATE_BY = chkItem.CREATE_BY;
                                    soItem.CREATE_DATE = chkItem.CREATE_DATE;
                                    soItem.PO_COMPLETE_SO_ITEM_ID = chkItem.PO_COMPLETE_SO_ITEM_ID;
                                    SAP_M_PO_COMPLETE_SO_ITEM_SERVICE.UpdateNoLog(soItem, context);
                                    Results.cnt++;
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(soItem.REJECTION_CODE))
                                        soItem.IS_ACTIVE = "X";
                                    else
                                        soItem.IS_ACTIVE = null;

                                    SAP_M_PO_COMPLETE_SO_ITEM_SERVICE.SaveNoLog(soItem, context);
                                    Results.cnt++;
                                }

                                soItemID = soItem.PO_COMPLETE_SO_ITEM_ID;

                                var tempCOMPONENT = SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_SERVICE.GetByItem(new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT() { PO_COMPLETE_SO_ITEM_ID = soItem.PO_COMPLETE_SO_ITEM_ID }, context);
                                foreach (var item in tempCOMPONENT)
                                {
                                    item.IS_ACTIVE = null;
                                    SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_SERVICE.UpdateNoLog(item, context);
                                }

                                foreach (COMPONENT itemSOComponent in itemSOItem.COMPONENTS)
                                {
                                    var soItemComponent = new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT();

                                    soItemComponent.PO_COMPLETE_SO_ITEM_ID = soItemID;
                                    soItemComponent.COMPONENT_ITEM = itemSOComponent.COMPONENT_ITEM;
                                    soItemComponent.COMPONENT_MATERIAL = itemSOComponent.COMPONENT_MATERIAL;
                                    soItemComponent.DECRIPTION = itemSOComponent.DECRIPTION;

                                    if (!String.IsNullOrEmpty(itemSOComponent.QUANTITY))
                                    {
                                        decimal decOut;
                                        if (Decimal.TryParse(itemSOComponent.QUANTITY, out decOut))
                                        {
                                            soItemComponent.QUANTITY = decOut;
                                        }
                                    }

                                    soItemComponent.UNIT = itemSOComponent.UNIT;
                                    soItemComponent.STOCK = itemSOComponent.STOCK;
                                    soItemComponent.BOM_ITEM_CUSTOM_1 = itemSOComponent.BOM_ITEM_CUSTOM_1;
                                    soItemComponent.BOM_ITEM_CUSTOM_2 = itemSOComponent.BOM_ITEM_CUSTOM_2;
                                    soItemComponent.BOM_ITEM_CUSTOM_3 = itemSOComponent.BOM_ITEM_CUSTOM_3;
                                    soItemComponent.CREATE_BY = userID;
                                    soItemComponent.UPDATE_BY = userID;
                                    //CNService.sendemail("", "", "step1", "update component step1","");
                                    //var chkComponent = SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_SERVICE.GetByItem(new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT() { PO_COMPLETE_SO_ITEM_ID = soItemComponent.PO_COMPLETE_SO_ITEM_ID, COMPONENT_ITEM = soItemComponent.COMPONENT_ITEM }, context).FirstOrDefault();
                                    var chkComponent = (from p in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                                                        where p.PO_COMPLETE_SO_ITEM_ID == soItemComponent.PO_COMPLETE_SO_ITEM_ID
                                                        && p.COMPONENT_ITEM == soItemComponent.COMPONENT_ITEM
                                                        select new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2() { CREATE_BY = p.CREATE_BY, CREATE_DATE = p.CREATE_DATE, PO_COMPLETE_SO_ITEM_COMPONENT_ID = p.PO_COMPLETE_SO_ITEM_COMPONENT_ID,IS_ASSIGN=p.IS_ASSIGN }).FirstOrDefault();

                                    if (chkComponent != null)
                                    {
                                        soItemComponent.IS_ASSIGN = chkComponent.IS_ASSIGN;
                                        soItemComponent.IS_ACTIVE = "X";
                                        soItemComponent.CREATE_BY = chkComponent.CREATE_BY;
                                        soItemComponent.CREATE_DATE = chkComponent.CREATE_DATE;
                                        soItemComponent.PO_COMPLETE_SO_ITEM_COMPONENT_ID = chkComponent.PO_COMPLETE_SO_ITEM_COMPONENT_ID;
                                        SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_SERVICE.UpdateNoLog(soItemComponent, context);
                                        Results.cnt++;
                                    }
                                    else
                                    {
                                        soItemComponent.IS_ACTIVE = "X";
                                        SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_SERVICE.SaveNoLog(soItemComponent, context);
                                        Results.cnt++;
                                    }
                                }
                            }

                            //### check component is_assign artwork
                            if (soHeader != null)
                            {
                                var h = (from s in context.SAP_M_PO_COMPLETE_SO_HEADER
                                                 where s.SALES_ORDER_NO == soHeader.SALES_ORDER_NO
                                                 select s).FirstOrDefault();
                                if (h != null)
                                {
                                    var listSO = CNService.GetAssignOrder(h.PO_COMPLETE_SO_HEADER_ID, context);
                                    if (listSO == 0)
                                        soHeader.IS_ASSIGN = "X";
                                    else
                                        soHeader.IS_ASSIGN = null;
                                    SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.UpdateNoLog(soHeader, context);
                                }
                            }
                        }
                    }

                    try
                    {
                        //insert mat lock
                        var listMat5AndSO = new List<string>();
                        foreach (SO_HEADER itemSOHeader in param.SO_HEADERS)
                        {
                            foreach (SO_ITEM itemSOItem in itemSOHeader.SO_ITEMS)
                            {
                                foreach (COMPONENT itemSOComponent in itemSOItem.COMPONENTS)
                                {
                                    if (!string.IsNullOrEmpty(itemSOComponent.COMPONENT_MATERIAL))
                                    {
                                        if (itemSOComponent.COMPONENT_MATERIAL.StartsWith("5"))
                                        {
                                            if (itemSOComponent.COMPONENT_MATERIAL.Length == 18)
                                            {
                                                var strChk = itemSOComponent.COMPONENT_MATERIAL + itemSOHeader.SALES_ORDER_NO+itemSOItem.PRODUCT_CODE;
                                                if (listMat5AndSO.Where(m => m == strChk).ToList().Count() == 0)
                                                {
                                                    using (var context = new ARTWORKEntities())
                                                    {
                                                        using (var dbContextTransaction = CNService.IsolationLevel(context))
                                                        {
                                                            context.Database.CommandTimeout = 600;

                                                            CNService.InsertMaterialLock(itemSOComponent.COMPONENT_MATERIAL, itemSOHeader.SALES_ORDER_NO, itemSOItem.PRODUCT_CODE, context,"127");
                                                            dbContextTransaction.Commit();

                                                            var str = itemSOComponent.COMPONENT_MATERIAL + itemSOHeader.SALES_ORDER_NO + itemSOItem.PRODUCT_CODE;
                                                            listMat5AndSO.Add(str);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex) { CNService.GetErrorMessage(ex); }

                    try
                    {
                        var listMat5 = new List<string>();
                        foreach (SO_HEADER itemSOHeader in param.SO_HEADERS)
                        {
                            using (var context = new ARTWORKEntities())
                            {
                                context.Database.CommandTimeout = 600;

                                DeleteFOC(itemSOHeader.SALES_ORDER_NO, context);
                                foreach (SO_ITEM itemSOItem in itemSOHeader.SO_ITEMS)
                                {
                                    foreach (COMPONENT itemSOComponent in itemSOItem.COMPONENTS)
                                    {
                                        //if (CNService.IsDevOrQAS())
                                        //{
                                        if (!string.IsNullOrEmpty(itemSOComponent.COMPONENT_MATERIAL) && string.IsNullOrEmpty(itemSOComponent.BOM_ITEM_CUSTOM_1))
                                        {
                                            if (listMat5.Where(m => m == itemSOComponent.COMPONENT_MATERIAL).ToList().Count() == 0)
                                            {
                                                CNService.RepairMaterialBOM_1(itemSOHeader, itemSOComponent.COMPONENT_MATERIAL, context);
                                                listMat5.Add(itemSOComponent.COMPONENT_MATERIAL);
                                            }
                                        }
                                        else if (!string.IsNullOrEmpty(itemSOComponent.BOM_ITEM_CUSTOM_1))
                                        {
                                            CNService.RepairMaterialBOM2_1(itemSOHeader.SALES_ORDER_NO, itemSOItem.ITEM, itemSOComponent.COMPONENT_ITEM, itemSOComponent.BOM_ITEM_CUSTOM_1, itemSOComponent.COMPONENT_MATERIAL, context);
                                        }
                                        //}
                                        //else
                                        //{
                                        //    if (!string.IsNullOrEmpty(itemSOComponent.COMPONENT_MATERIAL) && string.IsNullOrEmpty(itemSOComponent.BOM_ITEM_CUSTOM_1))
                                        //    {
                                        //        if (listMat5.Where(m => m == itemSOComponent.COMPONENT_MATERIAL).ToList().Count() == 0)
                                        //        {
                                        //            CNService.RepairMaterialBOM(itemSOComponent.COMPONENT_MATERIAL, context);
                                        //            listMat5.Add(itemSOComponent.COMPONENT_MATERIAL);
                                        //        }
                                        //    }
                                        //    else if (!string.IsNullOrEmpty(itemSOComponent.BOM_ITEM_CUSTOM_1))
                                        //    {
                                        //        CNService.RepairMaterialBOM2(itemSOHeader.SALES_ORDER_NO, itemSOItem.ITEM, itemSOComponent.COMPONENT_ITEM, itemSOComponent.BOM_ITEM_CUSTOM_1, itemSOComponent.COMPONENT_MATERIAL, context);
                                        //    }
                                        //}
                                    }
                                    // Add free item into sales order assigned
                                    AddFOCIntoSOAssign(itemSOHeader, itemSOItem, context);
                                }
                            }
                        }
                    }
                    catch (Exception ex) { CNService.GetErrorMessage(ex); }

                    Results.status = "S";
                    Results.msg = MessageHelper.GetMessage("MSG_001");
                }
                catch (Exception ex)
                {
                    Results.cnt = 0;
                    Results.status = "E";
                    Results.msg = CNService.GetErrorMessage(ex);
                }
            }

            Results.finish = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            CNService.SaveLogReturnInterface(Results, "SAP_M_PO_COMPLETE_SO", guid,"SD127");

            return Results;
        }

        public static void DeleteFOC(string SALES_ORDER_NO, ARTWORKEntities context)
        {
            //if (CNService.IsDevOrQAS())
            //{
            var listFOC = (from f in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                           join m in context.ART_WF_ARTWORK_PROCESS on f.ARTWORK_SUB_ID equals m.ARTWORK_SUB_ID
                           where f.SALES_ORDER_NO == SALES_ORDER_NO
                              && f.BOM_NO == "FOC"
                              && string.IsNullOrEmpty(m.IS_END)
                           select f).ToList();

            var listNotFOC = (from f in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                              join m in context.ART_WF_ARTWORK_PROCESS on f.ARTWORK_SUB_ID equals m.ARTWORK_SUB_ID
                              where f.SALES_ORDER_NO == SALES_ORDER_NO
                                 && string.IsNullOrEmpty(f.BOM_NO)
                                 && string.IsNullOrEmpty(m.IS_END)
                              select f).ToList();

            foreach (var item2 in listFOC)
            {
                var cntNotFOC = listNotFOC.Where(m => m.ARTWORK_SUB_ID == item2.ARTWORK_SUB_ID).Count();
                if (cntNotFOC > 0)
                {
                    //delete foc
                    ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.DeleteByARTWORK_PROCESS_SO_ID(item2.ARTWORK_PROCESS_SO_ID, context);

                    context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER WHERE ARTWORK_PROCESS_SO_ID  = '" + item2.ARTWORK_PROCESS_SO_ID + "'");
                    context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM WHERE ARTWORK_PROCESS_SO_ID  = '" + item2.ARTWORK_PROCESS_SO_ID + "'");
                    context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT WHERE ARTWORK_PROCESS_SO_ID  = '" + item2.ARTWORK_PROCESS_SO_ID + "'");
                }
            }
            //}
            //else
            //{
            //    var listFOC = (from f in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
            //                   join m in context.ART_WF_ARTWORK_PROCESS on f.ARTWORK_SUB_ID equals m.ARTWORK_SUB_ID
            //                   where f.SALES_ORDER_NO == SALES_ORDER_NO
            //                      && f.BOM_NO == "FOC"
            //                      && string.IsNullOrEmpty(m.IS_END)
            //                   select f).ToList();

            //    foreach (var item2 in listFOC)
            //    {
            //        //delete foc
            //        ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.DeleteByARTWORK_PROCESS_SO_ID(item2.ARTWORK_PROCESS_SO_ID, context);

            //        context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER WHERE ARTWORK_PROCESS_SO_ID  = '" + item2.ARTWORK_PROCESS_SO_ID + "'");
            //        context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM WHERE ARTWORK_PROCESS_SO_ID  = '" + item2.ARTWORK_PROCESS_SO_ID + "'");
            //        context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT WHERE ARTWORK_PROCESS_SO_ID  = '" + item2.ARTWORK_PROCESS_SO_ID + "'");
            //    }
            //}
        }

        public static void AddFOCIntoSOAssign(SO_HEADER soHeader, SO_ITEM soItem, ARTWORKEntities context)
        {
            var soHeaderDetail = (from h in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                  join m in context.ART_WF_ARTWORK_PROCESS on h.ARTWORK_SUB_ID equals m.ARTWORK_SUB_ID
                                  where h.SALES_ORDER_NO == soHeader.SALES_ORDER_NO
                                     && string.IsNullOrEmpty(m.IS_END)
                                  select h).FirstOrDefault();

            if (soHeaderDetail != null)
            {
                if (!String.IsNullOrEmpty(soItem.ITEM_CUSTOM_1))
                {
                    var listSOFOC = (from f in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                     join m in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT on f.BOM_ID equals m.PO_COMPLETE_SO_ITEM_COMPONENT_ID
                                     where f.SALES_ORDER_NO == soHeaderDetail.SALES_ORDER_NO
                                        && f.SALES_ORDER_ITEM == soItem.ITEM_CUSTOM_1
                                        && m.COMPONENT_MATERIAL == soItem.PRODUCT_CODE
                                     select f).ToList();

                    var tempListSO_DETAIL = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL>();
                    if (listSOFOC.Count > 0)
                    {
                        var listSO = listSOFOC.Select(m => m.SALES_ORDER_NO).Distinct().ToList();
                        tempListSO_DETAIL = (from f in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                             where listSO.Contains(f.SALES_ORDER_NO)
                                             select f).ToList();
                    }

                    foreach (var soFOC in listSOFOC)
                    {
                        ART_WF_ARTWORK_PROCESS_SO_DETAIL assignFOC = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
                        ART_WF_ARTWORK_PROCESS_SO_DETAIL assignFOCTmp = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();

                        assignFOC.SALES_ORDER_NO = soFOC.SALES_ORDER_NO;
                        assignFOC.SALES_ORDER_ITEM = Convert.ToInt32(soItem.ITEM).ToString();
                        assignFOC.ARTWORK_REQUEST_ID = soFOC.ARTWORK_REQUEST_ID;
                        assignFOC.ARTWORK_SUB_ID = soFOC.ARTWORK_SUB_ID;

                        assignFOCTmp = tempListSO_DETAIL.Where(m => m.SALES_ORDER_NO == assignFOC.SALES_ORDER_NO
                                                             && m.SALES_ORDER_ITEM == assignFOC.SALES_ORDER_ITEM
                                                             && m.ARTWORK_REQUEST_ID == assignFOC.ARTWORK_REQUEST_ID
                                                             && m.ARTWORK_SUB_ID == assignFOC.ARTWORK_SUB_ID).FirstOrDefault();

                        assignFOC.CREATE_BY = -2;
                        assignFOC.UPDATE_BY = -2;
                        assignFOC.MATERIAL_NO = soItem.PRODUCT_CODE;
                        assignFOC.BOM_NO = "FOC";
                        if (assignFOCTmp == null)
                        {
                            //if (CNService.IsDevOrQAS())
                            //{
                            var cntFOC = tempListSO_DETAIL.Where(m => m.SALES_ORDER_NO == assignFOC.SALES_ORDER_NO && m.SALES_ORDER_ITEM == assignFOC.SALES_ORDER_ITEM).Count();
                            if (cntFOC == 0)
                            {
                                ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.SaveNoLog(assignFOC, context);
                                var assignFOCTmp2 = ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByARTWORK_PROCESS_SO_ID(assignFOC.ARTWORK_PROCESS_SO_ID, context);
                                if (assignFOCTmp2 != null)
                                {
                                    SaveFOCAssign(context, assignFOCTmp2);
                                }
                            }
                            //}
                            //else
                            //{
                            //    ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.SaveNoLog(assignFOC, context);
                            //    var assignFOCTmp2 = ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByARTWORK_PROCESS_SO_ID(assignFOC.ARTWORK_PROCESS_SO_ID, context);
                            //    if (assignFOCTmp2 != null)
                            //    {
                            //        SaveFOCAssign(context, assignFOCTmp2);
                            //    }
                            //}
                        }
                    }
                }
            }
        }

        private static void SaveFOCAssign(ARTWORKEntities context, ART_WF_ARTWORK_PROCESS_SO_DETAIL assignFOCTmp2)
        {
            SAP_M_PO_COMPLETE_SO_HEADER focHeader = new SAP_M_PO_COMPLETE_SO_HEADER();
            SAP_M_PO_COMPLETE_SO_ITEM focItem = new SAP_M_PO_COMPLETE_SO_ITEM();
            SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT focComponent = new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT();

            focHeader = (from p in context.SAP_M_PO_COMPLETE_SO_HEADER
                         where p.SALES_ORDER_NO == assignFOCTmp2.SALES_ORDER_NO
                         select p).FirstOrDefault();

            if (focHeader != null)
            {
                decimal itemNO = 0;
                itemNO = Convert.ToDecimal(assignFOCTmp2.SALES_ORDER_ITEM);

                focItem = (from p in context.SAP_M_PO_COMPLETE_SO_ITEM
                           where p.PO_COMPLETE_SO_HEADER_ID == focHeader.PO_COMPLETE_SO_HEADER_ID
                            && p.ITEM == itemNO
                           select p).FirstOrDefault();

                if (focItem != null)
                {
                    focComponent = (from p in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                                    where p.PO_COMPLETE_SO_ITEM_ID == focItem.PO_COMPLETE_SO_ITEM_ID
                                     && p.PO_COMPLETE_SO_ITEM_COMPONENT_ID == assignFOCTmp2.BOM_ID
                                    select p).FirstOrDefault();
                }

            }

            if (focHeader != null)
            {
                ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER assignHeader = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER();

                assignHeader.ARTWORK_PROCESS_SO_ID = assignFOCTmp2.ARTWORK_PROCESS_SO_ID;
                assignHeader.ARTWORK_REQUEST_ID = assignFOCTmp2.ARTWORK_REQUEST_ID;
                assignHeader.ARTWORK_SUB_ID = assignFOCTmp2.ARTWORK_SUB_ID;
                assignHeader.SALES_ORDER_NO = focHeader.SALES_ORDER_NO;
                assignHeader.SOLD_TO = focHeader.SOLD_TO;
                assignHeader.SOLD_TO_NAME = focHeader.SOLD_TO_NAME;
                assignHeader.LAST_SHIPMENT_DATE = focHeader.LAST_SHIPMENT_DATE;
                assignHeader.DATE_1_2 = focHeader.DATE_1_2;
                assignHeader.CREATE_ON = focHeader.CREATE_ON;
                assignHeader.RDD = focHeader.RDD;
                assignHeader.PAYMENT_TERM = focHeader.PAYMENT_TERM;
                assignHeader.LC_NO = focHeader.LC_NO;
                assignHeader.EXPIRED_DATE = focHeader.EXPIRED_DATE;
                assignHeader.SHIP_TO = focHeader.SHIP_TO;
                assignHeader.SHIP_TO_NAME = focHeader.SHIP_TO_NAME;
                assignHeader.SOLD_TO_PO = focHeader.SOLD_TO_PO;
                assignHeader.SHIP_TO_PO = focHeader.SHIP_TO_PO;
                assignHeader.SALES_GROUP = focHeader.SALES_GROUP;
                assignHeader.MARKETING_CO = focHeader.MARKETING_CO;
                assignHeader.MARKETING_CO_NAME = focHeader.MARKETING_CO_NAME;
                assignHeader.MARKETING = focHeader.MARKETING;
                assignHeader.MARKETING_NAME = focHeader.MARKETING_NAME;
                assignHeader.MARKETING_ORDER_SAP = focHeader.MARKETING_ORDER_SAP;
                assignHeader.MARKETING_ORDER_SAP_NAME = focHeader.MARKETING_ORDER_SAP_NAME;
                assignHeader.SALES_ORG = focHeader.SALES_ORG;
                assignHeader.DISTRIBUTION_CHANNEL = focHeader.DISTRIBUTION_CHANNEL;
                assignHeader.DIVITION = focHeader.DIVITION;
                assignHeader.SALES_ORDER_TYPE = focHeader.SALES_ORDER_TYPE;
                assignHeader.HEADER_CUSTOM_1 = focHeader.HEADER_CUSTOM_1;
                assignHeader.HEADER_CUSTOM_2 = focHeader.HEADER_CUSTOM_2;
                assignHeader.HEADER_CUSTOM_3 = focHeader.HEADER_CUSTOM_3;
                assignHeader.CREATE_BY = assignFOCTmp2.UPDATE_BY;
                assignHeader.UPDATE_BY = assignFOCTmp2.UPDATE_BY;

                ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_SERVICE.SaveOrUpdateNoLog(assignHeader, context);

                if (focItem != null)
                {
                    ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM assignItem = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM();
                    assignItem.ASSIGN_SO_HEADER_ID = assignHeader.ASSIGN_SO_HEADER_ID;
                    assignItem.ARTWORK_PROCESS_SO_ID = assignFOCTmp2.ARTWORK_PROCESS_SO_ID;
                    assignItem.ARTWORK_REQUEST_ID = assignFOCTmp2.ARTWORK_REQUEST_ID;
                    assignItem.ARTWORK_SUB_ID = assignFOCTmp2.ARTWORK_SUB_ID;
                    assignItem.ITEM = focItem.ITEM;
                    assignItem.PRODUCT_CODE = focItem.PRODUCT_CODE;
                    assignItem.MATERIAL_DESCRIPTION = focItem.MATERIAL_DESCRIPTION;
                    assignItem.NET_WEIGHT = focItem.NET_WEIGHT;
                    assignItem.ORDER_QTY = focItem.ORDER_QTY;
                    assignItem.ORDER_UNIT = focItem.ORDER_UNIT;
                    assignItem.ETD_DATE_FROM = focItem.ETD_DATE_FROM;
                    assignItem.ETD_DATE_TO = focItem.ETD_DATE_TO;
                    assignItem.PLANT = focItem.PLANT;
                    assignItem.OLD_MATERIAL_CODE = focItem.OLD_MATERIAL_CODE;
                    assignItem.PACK_SIZE = focItem.PACK_SIZE;
                    assignItem.VALUME_PER_UNIT = focItem.VALUME_PER_UNIT;
                    assignItem.VALUME_UNIT = focItem.VALUME_UNIT;
                    assignItem.SIZE_DRAIN_WT = focItem.SIZE_DRAIN_WT;
                    assignItem.PROD_INSP_MEMO = focItem.PROD_INSP_MEMO;
                    assignItem.REJECTION_CODE = focItem.REJECTION_CODE;
                    assignItem.REJECTION_DESCRIPTION = focItem.REJECTION_DESCRIPTION;
                    assignItem.PORT = focItem.PORT;
                    assignItem.VIA = focItem.VIA;
                    assignItem.IN_TRANSIT_TO = focItem.IN_TRANSIT_TO;
                    assignItem.BRAND_ID = focItem.BRAND_ID;
                    assignItem.BRAND_DESCRIPTION = focItem.BRAND_DESCRIPTION;
                    assignItem.ADDITIONAL_BRAND_ID = focItem.ADDITIONAL_BRAND_ID;
                    assignItem.ADDITIONAL_BRAND_DESCRIPTION = focItem.ADDITIONAL_BRAND_DESCRIPTION;
                    assignItem.PRODUCTION_PLANT = focItem.PRODUCTION_PLANT;
                    assignItem.ZONE = focItem.ZONE;
                    assignItem.COUNTRY = focItem.COUNTRY;
                    assignItem.PRODUCTION_HIERARCHY = focItem.PRODUCTION_HIERARCHY;
                    assignItem.MRP_CONTROLLER = focItem.MRP_CONTROLLER;
                    assignItem.STOCK = focItem.STOCK;
                    assignItem.ITEM_CUSTOM_1 = focItem.ITEM_CUSTOM_1;
                    assignItem.ITEM_CUSTOM_2 = focItem.ITEM_CUSTOM_2;
                    assignItem.ITEM_CUSTOM_3 = focItem.ITEM_CUSTOM_3;
                    assignItem.CREATE_BY = assignFOCTmp2.UPDATE_BY;
                    assignItem.UPDATE_BY = assignFOCTmp2.UPDATE_BY;

                    ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_SERVICE.SaveOrUpdateNoLog(assignItem, context);

                    if (focComponent != null)
                    {
                        ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT assignBOM = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT();

                        assignBOM.ASSIGN_SO_ITEM_ID = assignItem.ASSIGN_SO_ITEM_ID;
                        assignBOM.ARTWORK_PROCESS_SO_ID = assignFOCTmp2.ARTWORK_PROCESS_SO_ID;
                        assignBOM.ARTWORK_REQUEST_ID = assignFOCTmp2.ARTWORK_REQUEST_ID;
                        assignBOM.ARTWORK_SUB_ID = assignFOCTmp2.ARTWORK_SUB_ID;
                        assignBOM.COMPONENT_ITEM = focComponent.COMPONENT_ITEM;
                        assignBOM.COMPONENT_MATERIAL = focComponent.COMPONENT_MATERIAL;
                        assignBOM.DECRIPTION = focComponent.DECRIPTION;
                        assignBOM.QUANTITY = focComponent.QUANTITY;
                        assignBOM.UNIT = focComponent.UNIT;
                        assignBOM.STOCK = focComponent.STOCK;
                        assignBOM.BOM_ITEM_CUSTOM_1 = focComponent.BOM_ITEM_CUSTOM_1;
                        assignBOM.BOM_ITEM_CUSTOM_2 = focComponent.BOM_ITEM_CUSTOM_2;
                        assignBOM.BOM_ITEM_CUSTOM_3 = focComponent.BOM_ITEM_CUSTOM_3;
                        assignBOM.CREATE_BY = assignFOCTmp2.CREATE_BY;
                        assignBOM.UPDATE_BY = assignFOCTmp2.UPDATE_BY;

                        ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_SERVICE.SaveOrUpdateNoLog(assignBOM, context);
                    }
                }

            }
        }
    }
}





