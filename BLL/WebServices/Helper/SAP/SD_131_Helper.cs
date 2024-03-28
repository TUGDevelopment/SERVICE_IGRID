using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DAL;
using System.Data.Entity;
using System.Configuration;
using WebServices.Model;
using BLL.Helpers;
using BLL.Services;
using System.Globalization;
using System.Web.Script.Serialization;

namespace BLL.WebServices.Helper
{
    public static class SD_131_Helper
    {
        private static int UserID = -2;

        public static void SaveLog(ORDER_BOM_MODEL param, string GUID)
        {
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    ART_SYS_LOG Item = new ART_SYS_LOG();
                    Item.ACTION = "Interface Inbound";
                    Item.TABLE_NAME = "SAP_M_ORDER_BOM";
                    if (param.ORDER_BOMS != null) Item.NEW_VALUE = CNService.SubString(CNService.Serialize(param.ORDER_BOMS), 4000);
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

        public static SERVICE_RESULT_MODEL SaveOrderBom(ORDER_BOM_MODEL ORDER_BOMS)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();
            Results.start = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            string guid = Guid.NewGuid().ToString();
            SaveLog(ORDER_BOMS, guid);

            if (ORDER_BOMS != null && ORDER_BOMS.ORDER_BOMS != null && ORDER_BOMS.ORDER_BOMS.Count > 0)
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        foreach (ORDER_BOM iOrderBOM in ORDER_BOMS.ORDER_BOMS)
                        {
                            var orderBOM = new SAP_M_ORDER_BOM();
                            var orderBOMTmp = new SAP_M_ORDER_BOM();

                            orderBOMTmp.DATE = iOrderBOM.Date;
                            orderBOMTmp.TIME = iOrderBOM.Time;

                            orderBOM = MapperOrderBOM(iOrderBOM);

                            if (!String.IsNullOrEmpty(iOrderBOM.Counter))
                            {
                                Decimal decOut;
                                if (Decimal.TryParse(iOrderBOM.Counter.Trim(), out decOut))
                                {
                                    orderBOMTmp.COUNTER = decOut;
                                }
                            }

                            orderBOMTmp = SAP_M_ORDER_BOM_SERVICE.GetByItem(orderBOMTmp, context).FirstOrDefault();

                            if (orderBOMTmp != null)
                            {
                                orderBOM.CREATE_BY = orderBOMTmp.CREATE_BY;
                                orderBOM.CREATE_DATE = orderBOMTmp.CREATE_DATE;
                                orderBOM.ORDER_BOM_ID = orderBOMTmp.ORDER_BOM_ID;
                                SAP_M_ORDER_BOM_SERVICE.UpdateNoLog(orderBOM, context);
                                Results.cnt++;
                            }
                            else
                            {
                                SAP_M_ORDER_BOM_SERVICE.SaveNoLog(orderBOM, context);
                                Results.cnt++;
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

            CNService.SaveLogReturnInterface(Results, "SAP_M_ORDER_BOM", guid,"SD131");

            return Results;
        }

        private static SAP_M_ORDER_BOM MapperOrderBOM(ORDER_BOM orderBOMResp)
        {
            SAP_M_ORDER_BOM orderBOM = new SAP_M_ORDER_BOM();

            orderBOM.CHANGE_TYPE = orderBOMResp.ChangeType;
            orderBOM.DATE = orderBOMResp.Date;
            orderBOM.TIME = orderBOMResp.Time;
            string dateFormat = "yyyyMMdd";

            if (!String.IsNullOrEmpty(orderBOMResp.Counter))
            {
                Decimal decOut;
                if (Decimal.TryParse(orderBOMResp.Counter.Trim(), out decOut))
                {
                    orderBOM.COUNTER = decOut;
                }
            }

            orderBOM.MATERIAL = orderBOMResp.MaterialFG;
            orderBOM.SOLD_TO_PARTY = orderBOMResp.SoldToParty;
            orderBOM.SHIP_TO_PARTY = orderBOMResp.ShipToParty;
            orderBOM.BRAND_ID = orderBOMResp.BrandID;
            orderBOM.ADDITIONAL_BRAND_ID = orderBOMResp.AdditionalBrand;
            orderBOM.ROUTE = orderBOMResp.Route;
            orderBOM.INTRANSIT_PORT = orderBOMResp.IntransitPort;
            orderBOM.SALES_ORGANIZATION = orderBOMResp.SalesOrganization;
            orderBOM.PLANT = orderBOMResp.Plant;
            orderBOM.MATERIAL_NUMBER = orderBOMResp.MaterialNumber;
            orderBOM.COUNTRY_KEY = orderBOMResp.CountryKey;

            if (!String.IsNullOrEmpty(orderBOMResp.PackagingQuantity))
            {
                Decimal decOut;
                if (Decimal.TryParse(orderBOMResp.PackagingQuantity.Trim(), out decOut))
                {
                    orderBOM.PACKAGING_QUANTITY = decOut;
                }
            }

            orderBOM.PACKAGING_UNIT = orderBOMResp.PackagingUnit;

            if (!String.IsNullOrEmpty(orderBOMResp.FGQuantity))
            {
                Decimal decOut;
                if (Decimal.TryParse(orderBOMResp.FGQuantity.Trim(), out decOut))
                {
                    orderBOM.FG_QUANTITY = decOut;
                }
            }

            orderBOM.FG_UNIT = orderBOMResp.FGUnit;

            if (!String.IsNullOrEmpty(orderBOMResp.StartDate))
            {
                DateTime decOut;
                if (DateTime.TryParseExact(orderBOMResp.StartDate,
                                                    dateFormat,
                                                    CultureInfo.InvariantCulture,
                                                    DateTimeStyles.None, out decOut))
                {
                    orderBOM.START_DATE = decOut;
                }
            }

            if (!String.IsNullOrEmpty(orderBOMResp.EndDate))
            {
                DateTime decOut;
                if (DateTime.TryParseExact(orderBOMResp.EndDate,
                                                  dateFormat,
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.None, out decOut))
                {
                    orderBOM.END_DATE = decOut;
                }
            }

            if (!String.IsNullOrEmpty(orderBOMResp.WastePercent))
            {
                Decimal decOut;
                if (Decimal.TryParse(orderBOMResp.WastePercent.Trim(), out decOut))
                {
                    orderBOM.WASTE_PERCENT = decOut;
                }
            }

            if (!String.IsNullOrEmpty(orderBOMResp.CounterReference))
            {
                Decimal decOut;
                if (Decimal.TryParse(orderBOMResp.CounterReference.Trim(), out decOut))
                {
                    orderBOM.COUNTER_REFERENCE = decOut;
                }
            }

            orderBOM.CREATE_BY = UserID;
            orderBOM.UPDATE_BY = UserID;

            return orderBOM;
        }
    }
}
