using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Model;
using BLL.Services;
using WebServices.Model;
using DAL;
using System.Data.Entity;
using BLL.Helpers;
using System.Web.Script.Serialization;
using System.Text;
using System.Threading;
//using WebServices.Model;

namespace WebServices.Helper
{
    public static class MM_73_Hepler
    {
        public static void SaveLog(IGRID_OUTBOUND_MODEL param, string GUID)
        {
            try
            {
                using (var dc = new ARTWORKEntities())
                {
                    ART_SYS_LOG Item = new ART_SYS_LOG();
                    Item.ACTION = "Interface Inbound";
                    Item.TABLE_NAME = "IGRID_M_OUTBOUND_HEADER";
                    if (param.OUTBOUND_HEADERS != null) Item.NEW_VALUE = CNService.SubString(CNService.Serialize(param.OUTBOUND_HEADERS), 4000);
                    Item.UPDATE_DATE = DateTime.Now;
                    Item.UPDATE_BY = UserID;
                    Item.CREATE_DATE = DateTime.Now;
                    Item.CREATE_BY = UserID;
                    Item.OLD_VALUE = GUID;
                    dc.ART_SYS_LOG.Add(Item);
                    dc.SaveChanges();

                    Item = new ART_SYS_LOG();
                    Item.ACTION = "Interface Inbound";
                    Item.TABLE_NAME = "IGRID_M_OUTBOUND_ITEM";
                    if (param.OUTBOUND_ITEMS != null) Item.NEW_VALUE = CNService.SubString(CNService.Serialize(param.OUTBOUND_ITEMS), 4000);
                    Item.UPDATE_DATE = DateTime.Now;
                    Item.UPDATE_BY = UserID;
                    Item.CREATE_DATE = DateTime.Now;
                    Item.CREATE_BY = UserID;
                    Item.OLD_VALUE = GUID;
                    dc.ART_SYS_LOG.Add(Item);
                    dc.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }

        private static int UserID = -2;

        public static SERVICE_RESULT_MODEL SaveMaterial(IGRID_OUTBOUND_MODEL param)
        {
            SERVICE_RESULT_MODEL results = new SERVICE_RESULT_MODEL();

            results =  SaveMaterial_Worker2(param);

            Thread task = new Thread(delegate ()
            {
                UpdatePAData(param);
                InsertVendorInMaterialConvertion(param);
            });

            task.IsBackground = true;
            task.Start();

            return results;
        }

        public static SERVICE_RESULT_MODEL SaveMaterial_Worker(IGRID_OUTBOUND_MODEL param)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();
            Results.start = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            string guid = Guid.NewGuid().ToString();
            SaveLog(param, guid);

            IGRID_M_OUTBOUND_HEADER header = new IGRID_M_OUTBOUND_HEADER();
            IGRID_M_OUTBOUND_ITEM item = new IGRID_M_OUTBOUND_ITEM();

            IGRID_M_OUTBOUND_HEADER headerFilter = new IGRID_M_OUTBOUND_HEADER();
            IGRID_M_OUTBOUND_ITEM itemFilter = new IGRID_M_OUTBOUND_ITEM();

            if (param != null)
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        context.Database.CommandTimeout = 600;

                        if (param.OUTBOUND_HEADERS != null && param.OUTBOUND_HEADERS.Count > 0)
                        {
                            foreach (IGRID_OUTBOUND_HEADER_MODEL iHeader in param.OUTBOUND_HEADERS)
                            {
                                header = new IGRID_M_OUTBOUND_HEADER();
                                header = MapperHeader(iHeader);

                                headerFilter = new IGRID_M_OUTBOUND_HEADER();

                                headerFilter.ARTWORK_NO = header.ARTWORK_NO;
                                headerFilter.DATE = header.DATE;
                                headerFilter.TIME = header.TIME;
                                var temp = IGRID_M_OUTBOUND_HEADER_SERVICE.GetByItem(headerFilter, context).FirstOrDefault();
                                if (temp != null)
                                {
                                    header.IGRID_OUTBOUND_HEADER_ID = temp.IGRID_OUTBOUND_HEADER_ID;
                                }
                                IGRID_M_OUTBOUND_HEADER_SERVICE.SaveOrUpdateNoLog(header, context);
                            }
                        }

                        if (param.OUTBOUND_ITEMS != null && param.OUTBOUND_ITEMS.Count > 0)
                        {
                            if (header.STATUS == "Completed")
                            {
                                #region "Delete Item before insert" 

                                var existItem = (from p in context.IGRID_M_OUTBOUND_ITEM
                                                 where p.ARTWORK_NO == header.ARTWORK_NO
                                                 && p.DATE == header.DATE
                                                 && p.TIME == header.TIME
                                                 select p.IGRID_OUTBOUND_ITEM_ID).ToList();

                                if (existItem != null)
                                {
                                    string strListID = "";
                                    StringBuilder sbItemID = new StringBuilder();
                                    foreach (var itemExist in existItem)
                                    {
                                        sbItemID.Append(itemExist + ",");
                                    }

                                    if (!String.IsNullOrEmpty(sbItemID.ToString()))
                                    {
                                        strListID = sbItemID.ToString().Substring(0, sbItemID.ToString().Length - 1);
                                    }

                                    if (!String.IsNullOrEmpty(strListID))
                                    {
                                        context.Database.ExecuteSqlCommand("DELETE FROM IGRID_M_OUTBOUND_ITEM WHERE IGRID_OUTBOUND_ITEM_ID  IN (" + strListID + ")");
                                    }
                                }

                                #endregion

                                foreach (IGRID_OUTBOUND_ITEM_MODEL iItem in param.OUTBOUND_ITEMS)
                                {
                                    item = new IGRID_M_OUTBOUND_ITEM();
                                    item = MapperItem(iItem);

                                    itemFilter = new IGRID_M_OUTBOUND_ITEM();
                                    itemFilter.ARTWORK_NO = item.ARTWORK_NO;
                                    itemFilter.DATE = item.DATE;
                                    itemFilter.TIME = item.TIME;
                                    itemFilter.CHARACTERISTIC_NAME = item.CHARACTERISTIC_NAME;
                                    itemFilter.CHARACTERISTIC_VALUE = item.CHARACTERISTIC_VALUE;
                                    itemFilter.CHARACTERISTIC_DESCRIPTION = item.CHARACTERISTIC_DESCRIPTION;
                                    //var temp = IGRID_M_OUTBOUND_ITEM_SERVICE.GetByItem(itemFilter, context).FirstOrDefault();
                                    //if (temp != null)
                                    //{
                                    //    item.IGRID_OUTBOUND_ITEM_ID = temp.IGRID_OUTBOUND_ITEM_ID;
                                    //}

                                    IGRID_M_OUTBOUND_ITEM_SERVICE.SaveOrUpdateNoLog(item, context);
                                    Results.cnt++;
                                }
                            }
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

            }

            UpdatePAData(param);

            InsertVendorInMaterialConvertion(param);
            
            Results.finish = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            CNService.SaveLogReturnInterface(Results, "IGRID_M_OUTBOUND", guid,"MM73");

            return Results;
        }

        public static SERVICE_RESULT_MODEL SaveMaterial_Worker2(IGRID_OUTBOUND_MODEL param)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();
            Results.start = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            string guid = Guid.NewGuid().ToString();
            SaveLog(param, guid);

            IGRID_M_OUTBOUND_HEADER header = new IGRID_M_OUTBOUND_HEADER();
            IGRID_M_OUTBOUND_ITEM item = new IGRID_M_OUTBOUND_ITEM();

            IGRID_M_OUTBOUND_HEADER headerFilter = new IGRID_M_OUTBOUND_HEADER();
            IGRID_M_OUTBOUND_ITEM itemFilter = new IGRID_M_OUTBOUND_ITEM();

            if (param != null)
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        context.Database.CommandTimeout = 600;

                        if (param.OUTBOUND_HEADERS != null && param.OUTBOUND_HEADERS.Count > 0)
                        {
                            foreach (IGRID_OUTBOUND_HEADER_MODEL iHeader in param.OUTBOUND_HEADERS)
                            {
                                header = new IGRID_M_OUTBOUND_HEADER();
                                header = MapperHeader(iHeader);

                                headerFilter = new IGRID_M_OUTBOUND_HEADER();

                                headerFilter.ARTWORK_NO = header.ARTWORK_NO;
                                headerFilter.DATE = header.DATE;
                                headerFilter.TIME = header.TIME;
                                var temp = IGRID_M_OUTBOUND_HEADER_SERVICE.GetByItem(headerFilter, context).FirstOrDefault();
                                if (temp != null)
                                {
                                    header.IGRID_OUTBOUND_HEADER_ID = temp.IGRID_OUTBOUND_HEADER_ID;
                                }
                                IGRID_M_OUTBOUND_HEADER_SERVICE.SaveOrUpdateNoLog(header, context);
                            }
                        }

                        if (param.OUTBOUND_ITEMS != null && param.OUTBOUND_ITEMS.Count > 0)
                        {
                            if (header.STATUS == "Completed")
                            {
                                #region "Delete Item before insert" 

                                var existItem = (from p in context.IGRID_M_OUTBOUND_ITEM
                                                 where p.ARTWORK_NO == header.ARTWORK_NO
                                                 && p.DATE == header.DATE
                                                 && p.TIME == header.TIME
                                                 select p.IGRID_OUTBOUND_ITEM_ID).ToList();

                                if (existItem != null)
                                {
                                    string strListID = "";
                                    StringBuilder sbItemID = new StringBuilder();
                                    foreach (var itemExist in existItem)
                                    {
                                        sbItemID.Append(itemExist + ",");
                                    }

                                    if (!String.IsNullOrEmpty(sbItemID.ToString()))
                                    {
                                        strListID = sbItemID.ToString().Substring(0, sbItemID.ToString().Length - 1);
                                    }

                                    if (!String.IsNullOrEmpty(strListID))
                                    {
                                        context.Database.ExecuteSqlCommand("DELETE FROM IGRID_M_OUTBOUND_ITEM WHERE IGRID_OUTBOUND_ITEM_ID  IN (" + strListID + ")");
                                    }
                                }

                                #endregion

                                foreach (IGRID_OUTBOUND_ITEM_MODEL iItem in param.OUTBOUND_ITEMS)
                                {
                                    item = new IGRID_M_OUTBOUND_ITEM();
                                    item = MapperItem(iItem);

                                    itemFilter = new IGRID_M_OUTBOUND_ITEM();
                                    itemFilter.ARTWORK_NO = item.ARTWORK_NO;
                                    itemFilter.DATE = item.DATE;
                                    itemFilter.TIME = item.TIME;
                                    itemFilter.CHARACTERISTIC_NAME = item.CHARACTERISTIC_NAME;
                                    itemFilter.CHARACTERISTIC_VALUE = item.CHARACTERISTIC_VALUE;
                                    itemFilter.CHARACTERISTIC_DESCRIPTION = item.CHARACTERISTIC_DESCRIPTION;
                                    //var temp = IGRID_M_OUTBOUND_ITEM_SERVICE.GetByItem(itemFilter, context).FirstOrDefault();
                                    //if (temp != null)
                                    //{
                                    //    item.IGRID_OUTBOUND_ITEM_ID = temp.IGRID_OUTBOUND_ITEM_ID;
                                    //}

                                    IGRID_M_OUTBOUND_ITEM_SERVICE.SaveOrUpdateNoLog(item, context);
                                    Results.cnt++;
                                }
                            }
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

            }

            Results.finish = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            CNService.SaveLogReturnInterface(Results, "IGRID_M_OUTBOUND", guid,"MM73");

            return Results;
        }


        private static IGRID_M_OUTBOUND_HEADER MapperHeader(IGRID_OUTBOUND_HEADER_MODEL headerResp)
        {
            IGRID_M_OUTBOUND_HEADER header = new IGRID_M_OUTBOUND_HEADER();

            header.ARTWORK_NO = headerResp.ArtworkNumber;
            header.RECORD_TYPE = headerResp.RecordType;
            header.DATE = headerResp.Date;
            header.TIME = headerResp.Time;
            header.MATERIAL_NUMBER = headerResp.MaterialNumber;
            header.MATERIAL_DESCRIPTION = headerResp.MaterialDescription;

            if (!String.IsNullOrEmpty(headerResp.MaterialCreatedDate))
            {
                DateTime decOut;
                if (DateTime.TryParse(headerResp.MaterialCreatedDate.Trim(), out decOut))
                {
                    header.MATERIAL_CREATE_DATE = decOut;
                }
            }

            header.CHANGE_POINT = headerResp.ChangePoint;
            header.ARTWORK_URL = headerResp.ArtworkURL;
            header.STATUS = headerResp.Status;
            header.PA_USER_NAME = headerResp.PAUserName;
            header.PG_USER_NAME = headerResp.PGUserName;
            header.REFERENCE_MATERIAL = headerResp.ReferenceMaterial;
            header.PLANT = headerResp.Plant;
            header.PRINTING_STYLE_OF_PRIMARY = headerResp.PrintingStyleofPrimary;
            header.PRINTING_STYLE_OF_SECONDARY = headerResp.PrintingStyleofSecondary;
            header.CUSTOMER_DESIGN = headerResp.CustomersDesign;
            header.CUSTOMER_DESIGN_DETAIL = headerResp.CustomersDesignDetail;
            header.CUSTOMER_SPEC = headerResp.CustomersSpec;
            header.CUSTOMER_SPEC_DETAIL = headerResp.CustomersSpecDetail;
            header.CUSTOMER_SIZE = headerResp.CustomersSize;
            header.CUSTOMER_SIZE_DETAIL = headerResp.CustomersSizeDetail;
            header.CUSTOMER_NOMINATES_VENDOR = headerResp.CustomerNominatesVendor;
            header.CUSTOMER_NOMINATES_VENDOR_DETAIL = headerResp.CustomerNominatesVendorDetail;
            header.CUSTOMER_NOMINATES_COLOR = headerResp.CustomerNominatesColorPantone;
            header.CUSTOMER_NOMINATES_COLOR_DETAIL = headerResp.CustomerNominatesColorPantoneDetail;
            header.CUSTOMER_BARCODE_SCANABLE = headerResp.CustomersBarcodeScanable;
            header.CUSTOMER_BARCODE_SCANABLE_DETAIL = headerResp.CustomersBarcodeScanableDetail;
            header.CUSTOMER_BARCODE_SPEC = headerResp.CustomersBarcodeSpec;
            header.CUSTOMER_BARCODE_SPEC_DETAIL = headerResp.CustomersBarcodeSpecDetail;
            header.FIRST_INFO_GROUP = headerResp.FirstInfoGroup;
            header.SO_NUMBER = headerResp.SONumber;
            header.SO_ITEM = headerResp.SOitem;
            header.SO_PLANT = headerResp.SOPlant;
            header.PIC_MKT = headerResp.PICMKT;
            header.DESTINATION = headerResp.Destination;
            header.NOTE_OF_PA = headerResp.RemarkNoteofPA;
            header.FINAL_INFO_GROUP = headerResp.FinalInfoGroup;
            header.NOTE_OF_PG = headerResp.RemarkNoteofPG;
            header.COMPLETE_INFO_GROUP = headerResp.CompleteInfoGroup;
            header.EXPIRY_DATE_SYSTEM = headerResp.ProductionExpirydatesystem;
            header.SERIOUSNESS_OF_COLOR_PRINTING = headerResp.Seriousnessofcolorprinting;
            header.ANALYSIS = headerResp.CustIngreNutritionAnalysis;
            header.SHADE_LIMIT = headerResp.ShadeLimit;

            if (!String.IsNullOrEmpty(headerResp.PackageQuantity))
            {
                Decimal decOut;
                if (Decimal.TryParse(headerResp.PackageQuantity.Trim(), out decOut))
                {
                    header.PACKAGE_QUANTITY = decOut;
                }
            }

            if (!String.IsNullOrEmpty(headerResp.WastePercent))
            {
                decimal decOut;
                if (Decimal.TryParse(headerResp.WastePercent.Trim(), out decOut))
                {
                    header.WASTE_PERCENT = decOut;
                }
            }

            header.CREATE_BY = UserID;
            header.UPDATE_BY = UserID;

            return header;
        }

        private static IGRID_M_OUTBOUND_ITEM MapperItem(IGRID_OUTBOUND_ITEM_MODEL itemResp)
        {
            IGRID_M_OUTBOUND_ITEM item = new IGRID_M_OUTBOUND_ITEM();

            item.ARTWORK_NO = itemResp.ArtworkNumber;
            item.DATE = itemResp.Date;
            item.TIME = itemResp.Time;
            item.CHARACTERISTIC_NAME = itemResp.Characteristic;
            item.CHARACTERISTIC_VALUE = itemResp.Value;
            item.CHARACTERISTIC_DESCRIPTION = itemResp.Description;
            item.CREATE_BY = UserID;
            item.UPDATE_BY = UserID;

            return item;
        }

        public static SERVICE_RESULT_MODEL UpdatePAData(IGRID_OUTBOUND_MODEL param)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();

            Dictionary<string, string> dicTypeOf = new Dictionary<string, string>();
            Dictionary<string, string> dicTypeOf2 = new Dictionary<string, string>();
            Dictionary<string, string> dicPlantRegister = new Dictionary<string, string>();
            Dictionary<string, string> dicPMSColour = new Dictionary<string, string>();
            Dictionary<string, string> dicCompany = new Dictionary<string, string>();
            Dictionary<string, string> dicProcessColour = new Dictionary<string, string>();
            Dictionary<string, string> dicCatchingPeriod = new Dictionary<string, string>();
            Dictionary<string, string> dicTotalColour = new Dictionary<string, string>();
            Dictionary<string, string> dicCatchingMethod = new Dictionary<string, string>();
            Dictionary<string, string> dicStyleOfPrinting = new Dictionary<string, string>();
            Dictionary<string, string> dicScientificName = new Dictionary<string, string>();
            Dictionary<string, string> dicDirectionOfSticker = new Dictionary<string, string>();
            Dictionary<string, string> dicSpecie = new Dictionary<string, string>();

            dicTypeOf.Add("C", "ZPKG_SEC_CARDBOARD_TYPE_1");
            dicTypeOf.Add("D", "ZPKG_SEC_DISPLAYER_TYPE_1");
            dicTypeOf.Add("F", "ZPKG_SEC_CARTON_TYPE_1");
            dicTypeOf.Add("G", "ZPKG_SEC_TRAY_TYPE");
            dicTypeOf.Add("H", "ZPKG_SEC_SLEEVE_BOX_TYPE");
            dicTypeOf.Add("J", "ZPKG_SEC_STICKER_TYPE");
            dicTypeOf.Add("K", "ZPKG_SEC_LABEL_TYPE");
            dicTypeOf.Add("L", "ZPKG_SEC_LEAFTLET_TYPE");
            dicTypeOf.Add("M", "ZPKG_SEC_STYLE_PLASTIC");
            dicTypeOf.Add("N", "ZPKG_SEC_INNER_TYPE_1");
            dicTypeOf.Add("P", "ZPKG_SEC_INSERT_TYPE");
            dicTypeOf.Add("R", "ZPKG_SEC_INNER_NON_TYPE");

            dicTypeOf2.Add("C", "ZPKG_SEC_CARDBOARD_TYPE_2");
            dicTypeOf2.Add("D", "ZPKG_SEC_DISPLAYER_TYPE_2");
            dicTypeOf2.Add("F", "ZPKG_SEC_CARTON_TYPE_2");
            dicTypeOf2.Add("G", "ZPKG_SEC_TRAY_CARTON_TYPE");
            dicTypeOf2.Add("N", "ZPKG_SEC_INNER_TYPE_2");
            dicTypeOf2.Add("R", "ZPKG_SEC_INNER_TYPE_2");

            dicPMSColour.Add("F", "ZPKG_SEC_CAR_PMS_COLOUR");
            dicPMSColour.Add("C", "ZPKG_SEC_CARD_PMS_COLOUR");
            dicPMSColour.Add("D", "ZPKG_SEC_DISP_PMS_COLOUR");
            dicPMSColour.Add("R", "ZPKG_SEC_INN_NO_PMS_COLOUR");
            dicPMSColour.Add("N", "ZPKG_SEC_INNER_PMS_COLOUR");
            dicPMSColour.Add("P", "ZPKG_SEC_INST_PMS_COLOUR");
            dicPMSColour.Add("K", "ZPKG_SEC_LABE_PMS_COLOUR");
            dicPMSColour.Add("L", "ZPKG_SEC_LEA_PMS_COLOUR");
            dicPMSColour.Add("H", "ZPKG_SEC_SLEV_PMS_COLOUR");
            dicPMSColour.Add("J", "ZPKG_SEC_STKC_PMS_COLOUR");
            dicPMSColour.Add("G", "ZPKG_SEC_TRAY_PMS_COLOUR");

            dicProcessColour.Add("F", "ZPKG_SEC_CAR_PROCESS_COLOUR");
            dicProcessColour.Add("C", "ZPKG_SEC_CARD_PROCESS_COLOUR");
            dicProcessColour.Add("D", "ZPKG_SEC_DISP_PROCESS_COLOUR");
            dicProcessColour.Add("R", "ZPKG_SEC_INN_NO_PROCESS_COLOUR");
            dicProcessColour.Add("N", "ZPKG_SEC_INNER_PROCESS_COLOUR");
            dicProcessColour.Add("P", "ZPKG_SEC_INST_PROCESS_COLOUR");
            dicProcessColour.Add("K", "ZPKG_SEC_LABE_PROCESS_COLOUR");
            dicProcessColour.Add("L", "ZPKG_SEC_LEA_PROCESS_COLOUR");
            dicProcessColour.Add("H", "ZPKG_SEC_SLEV_PROCESS_COLOUR");
            dicProcessColour.Add("J", "ZPKG_SEC_STKC_PROCESS_COLOUR");
            dicProcessColour.Add("G", "ZPKG_SEC_TRAY_PROCESS_COLOUR");

            dicTotalColour.Add("F", "ZPKG_SEC_CAR_TOTAL_COLOUR");
            dicTotalColour.Add("C", "ZPKG_SEC_CARD_TOTAL_COLOUR");
            dicTotalColour.Add("D", "ZPKG_SEC_DISP_TOTAL_COLOUR");
            dicTotalColour.Add("R", "ZPKG_SEC_INN_NO_TOTAL_COLOUR");
            dicTotalColour.Add("N", "ZPKG_SEC_INNER_TOTAL_COLOUR");
            dicTotalColour.Add("P", "ZPKG_SEC_INST_TOTAL_COLOUR");
            dicTotalColour.Add("K", "ZPKG_SEC_LABE_TOTAL_COLOUR");
            dicTotalColour.Add("L", "ZPKG_SEC_LEA_TOTAL_COLOUR");
            dicTotalColour.Add("H", "ZPKG_SEC_SLEV_TOTAL_COLOUR");
            dicTotalColour.Add("J", "ZPKG_SEC_STKC_TOTAL_COLOUR");
            dicTotalColour.Add("G", "ZPKG_SEC_TRAY_TOTAL_COLOUR");
            dicTotalColour.Add("M", "ZPKG_SEC_PLAST_TOTAL_COLOUR");

            dicStyleOfPrinting.Add("F", "ZPKG_SEC_CAR_PRINTING_STYLE");
            dicStyleOfPrinting.Add("C", "ZPKG_SEC_CARD_PRINTING_STYLE");
            dicStyleOfPrinting.Add("D", "ZPKG_SEC_DISP_PRINTING_STYLE");
            dicStyleOfPrinting.Add("R", "ZPKG_SEC_INN_NO_PRINTING_STYLE");
            dicStyleOfPrinting.Add("N", "ZPKG_SEC_INNER_PRINTING_STYLE");
            dicStyleOfPrinting.Add("P", "ZPKG_SEC_INST_PRINTING_STYLE");
            dicStyleOfPrinting.Add("K", "ZPKG_SEC_LABE_PRINTING_STYLE");
            dicStyleOfPrinting.Add("L", "ZPKG_SEC_LEA_PRINTING_STYLE");
            dicStyleOfPrinting.Add("H", "ZPKG_SEC_SLEV_PRINTING_STYLE");
            dicStyleOfPrinting.Add("J", "ZPKG_SEC_STKC_PRINTING_STYLE");
            dicStyleOfPrinting.Add("G", "ZPKG_SEC_TRAY_PRINTING_STYLE");

            ART_WF_ARTWORK_PROCESS_PA processPA = new ART_WF_ARTWORK_PROCESS_PA();
            try
            {
                if (param == null)
                {
                    return Results;
                }

                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 600;

                        foreach (IGRID_OUTBOUND_HEADER_MODEL iHeader in param.OUTBOUND_HEADERS)
                        {
                            if (!String.IsNullOrEmpty(iHeader.ArtworkNumber))
                            {
                                var header = (from h in context.IGRID_M_OUTBOUND_HEADER
                                              where h.ARTWORK_NO == iHeader.ArtworkNumber
                                              select h).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                                var items = (from h in context.IGRID_M_OUTBOUND_ITEM
                                             where h.ARTWORK_NO == header.ARTWORK_NO
                                                && h.DATE == header.DATE
                                                && h.TIME == header.TIME
                                             select h).ToList();

                                //var items = (from h in context.IGRID_M_OUTBOUND_ITEM
                                //             where h.ARTWORK_NO == header.ARTWORK_NO
                                //                && h.DATE == header.DATE
                                //                && h.TIME == header.TIME
                                //                && h.UPDATE_DATE.Day == header.UPDATE_DATE.Day
                                //                && h.UPDATE_DATE.Month == header.UPDATE_DATE.Month
                                //                && h.UPDATE_DATE.Year == header.UPDATE_DATE.Year
                                //                && h.UPDATE_DATE.Hour == header.UPDATE_DATE.Hour
                                //                && h.UPDATE_DATE.Minute == header.UPDATE_DATE.Minute
                                //             select h).ToList();

                                var artwork = (from a in context.ART_WF_ARTWORK_REQUEST_ITEM
                                               where a.REQUEST_ITEM_NO == header.ARTWORK_NO
                                               select a).FirstOrDefault();

                                if (artwork != null)
                                {
                                    var stepPAID = context.ART_M_STEP_ARTWORK.Where(p => p.STEP_ARTWORK_CODE == "SEND_PA")
                                                                                .Select(s => s.STEP_ARTWORK_ID)
                                                                                .FirstOrDefault();

                                    var processSub = (from s in context.ART_WF_ARTWORK_PROCESS
                                                      where s.ARTWORK_ITEM_ID == artwork.ARTWORK_ITEM_ID
                                                        && s.CURRENT_STEP_ID == stepPAID
                                                      select s).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                                    if (processSub != null)
                                    {
                                        processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                     where p.ARTWORK_SUB_ID == processSub.ARTWORK_SUB_ID
                                                     select p).OrderByDescending(o => o.UPDATE_BY).FirstOrDefault();

                                        if (processPA != null)
                                        {
                                            processPA.REQUEST_MATERIAL_STATUS = header.STATUS;
                                            // processPA.PRINTING_STYLE_OF_PRIMARY_ID
                                            // processPA.PRINTING_STYLE_OF_SECONDARY_ID

                                            if (header.STATUS.Contains("Cancel"))
                                            {
                                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA, context);
                                            }

                                            if (!header.STATUS.Contains("Cancel"))
                                            {
                                                processPA.PA_USER_ID = CNService.GetUserID(header.PA_USER_NAME, context);
                                                processPA.PG_USER_ID = CNService.GetUserID(header.PG_USER_NAME, context);
                                                processPA.MATERIAL_NO = header.MATERIAL_NUMBER;
                                                processPA.MATERIAL_DESCRIPTION = header.MATERIAL_DESCRIPTION;
                                                processPA.REFERENCE_MATERIAL = header.REFERENCE_MATERIAL;
                                                processPA.CUSTOMER_DESIGN = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_DESIGN);
                                                processPA.CUSTOMER_DESIGN_OTHER = header.CUSTOMER_DESIGN_DETAIL;
                                                processPA.CUSTOMER_SPEC = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_SPEC);
                                                processPA.CUSTOMER_SPEC_OTHER = header.CUSTOMER_SPEC_DETAIL;
                                                processPA.CUSTOMER_SIZE = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_SIZE);
                                                processPA.CUSTOMER_SIZE_OTHER = header.CUSTOMER_SIZE_DETAIL;
                                                processPA.CUSTOMER_NOMINATES_VENDOR = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_NOMINATES_VENDOR);
                                                processPA.CUSTOMER_NOMINATES_VENDOR_OTHER = header.CUSTOMER_NOMINATES_VENDOR_DETAIL;
                                                processPA.CUSTOMER_NOMINATES_COLOR = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_NOMINATES_COLOR);
                                                processPA.CUSTOMER_NOMINATES_COLOR_OTHER = header.CUSTOMER_NOMINATES_COLOR_DETAIL;
                                                processPA.CUSTOMER_BARCODE_SCANABLE = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_BARCODE_SCANABLE);
                                                processPA.CUSTOMER_BARCODE_SCANABLE_OTHER = header.CUSTOMER_BARCODE_SCANABLE_DETAIL;
                                                processPA.CUSTOMER_BARCODE_SPEC = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_BARCODE_SPEC);
                                                processPA.CUSTOMER_BARCODE_SPEC_OTHER = header.CUSTOMER_BARCODE_SPEC_DETAIL;
                                                processPA.FIRST_INFOGROUP_OTHER = header.FIRST_INFO_GROUP;
                                                processPA.NOTE_OF_PA = header.NOTE_OF_PA;
                                                processPA.FIRST_INFOGROUP_OTHER = header.FINAL_INFO_GROUP;
                                                processPA.COMPLETE_INFOGROUP = header.COMPLETE_INFO_GROUP;
                                                processPA.PRODUCTION_EXPIRY_DATE_SYSTEM = header.EXPIRY_DATE_SYSTEM;
                                                processPA.SERIOUSNESS_OF_COLOR_PRINTING = MaterialIGridHelper.ConvertYesNoValue(header.SERIOUSNESS_OF_COLOR_PRINTING);
                                                processPA.NUTRITION_ANALYSIS = MaterialIGridHelper.ConvertYesNoValue(header.ANALYSIS);
                                                processPA.PACKAGE_QUANTITY = header.PACKAGE_QUANTITY.ToString();
                                                processPA.WASTE_PERCENT = header.WASTE_PERCENT.ToString();
                                                processPA.CHANGE_POINT = header.CHANGE_POINT;

                                                var ZPKG_SEC_GROUP = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_GROUP", context);
                                                if (ZPKG_SEC_GROUP != null)
                                                {
                                                    processPA.MATERIAL_GROUP_ID = ZPKG_SEC_GROUP;

                                                    var matG = context.SAP_M_CHARACTERISTIC.Where(m => m.CHARACTERISTIC_ID == ZPKG_SEC_GROUP).FirstOrDefault();

                                                    string typeOf = "";
                                                    string typeOf2 = "";
                                                    string processColour = "";
                                                    string totalColour = "";
                                                    string stylePrinting = "";
                                                    string pmsColour = "";

                                                    if (dicTypeOf.ContainsKey(matG.VALUE))
                                                    {
                                                        typeOf = dicTypeOf[matG.VALUE];
                                                    }

                                                    if (dicTypeOf2.ContainsKey(matG.VALUE))
                                                    {
                                                        typeOf2 = dicTypeOf2[matG.VALUE];
                                                    }

                                                    if (dicPMSColour.ContainsKey(matG.VALUE))
                                                    {
                                                        pmsColour = dicPMSColour[matG.VALUE];
                                                    }

                                                    if (dicProcessColour.ContainsKey(matG.VALUE))
                                                    {
                                                        processColour = dicProcessColour[matG.VALUE];
                                                    }

                                                    if (dicTotalColour.ContainsKey(matG.VALUE))
                                                    {
                                                        totalColour = dicTotalColour[matG.VALUE];
                                                    }

                                                    if (dicStyleOfPrinting.ContainsKey(matG.VALUE))
                                                    {
                                                        stylePrinting = dicStyleOfPrinting[matG.VALUE];
                                                    }

                                                    if (!String.IsNullOrEmpty(typeOf))
                                                    {
                                                        var TYPE_OF = MaterialIGridHelper.GetPACharacteristicID(items, typeOf, context);
                                                        if (TYPE_OF != null)
                                                        {
                                                            processPA.TYPE_OF_ID = TYPE_OF;
                                                            processPA.TYPE_OF_OTHER = null;
                                                        }
                                                    }

                                                    if (!String.IsNullOrEmpty(typeOf2))
                                                    {
                                                        var TYPE_OF_2 = MaterialIGridHelper.GetPACharacteristicID(items, typeOf2, context);
                                                        if (TYPE_OF_2 != null)
                                                        {
                                                            processPA.TYPE_OF_2_ID = TYPE_OF_2;
                                                            processPA.TYPE_OF_2_OTHER = null;
                                                        }
                                                    }

                                                    if (!String.IsNullOrEmpty(pmsColour))
                                                    {
                                                        var PMS_COLOUR = MaterialIGridHelper.GetPACharacteristicID(items, pmsColour, context);
                                                        if (PMS_COLOUR != null)
                                                        {
                                                            processPA.PMS_COLOUR_ID = PMS_COLOUR;
                                                            processPA.PMS_COLOUR_OTHER = null;
                                                        }
                                                    }

                                                    if (!String.IsNullOrEmpty(processColour))
                                                    {
                                                        var PROCESS_COLOUR = MaterialIGridHelper.GetPACharacteristicID(items, processColour, context);
                                                        if (PROCESS_COLOUR != null)
                                                        {
                                                            processPA.PROCESS_COLOUR_ID = PROCESS_COLOUR;
                                                            processPA.PROCESS_COLOUR_OTHER = null;
                                                        }
                                                    }

                                                    if (!String.IsNullOrEmpty(totalColour))
                                                    {
                                                        var TOTAL_COLOUR = MaterialIGridHelper.GetPACharacteristicID(items, totalColour, context);
                                                        if (TOTAL_COLOUR != null)
                                                        {
                                                            processPA.TOTAL_COLOUR_ID = TOTAL_COLOUR;
                                                            processPA.TOTAL_COLOUR_OTHER = null;
                                                        }
                                                    }

                                                    if (!String.IsNullOrEmpty(stylePrinting))
                                                    {
                                                        var STYLE_PRINTING = MaterialIGridHelper.GetPACharacteristicID(items, stylePrinting, context);
                                                        if (STYLE_PRINTING != null)
                                                        {
                                                            processPA.STYLE_OF_PRINTING_ID = STYLE_PRINTING;
                                                            processPA.STYLE_OF_PRINTING_OTHER = null;
                                                        }
                                                    }
                                                }

                                                var ZPKG_SEC_PRIMARY_SIZE_VALUE = MaterialIGridHelper.GetIgridItem(items, "ZPKG_SEC_PRIMARY_SIZE", context);
                                                var ZPKG_SEC_CONTAINER_TYPE_VALUE = MaterialIGridHelper.GetIgridItem(items, "ZPKG_SEC_CONTAINER_TYPE", context);
                                                var ZPKG_SEC_LID_TYPE_VALUE = MaterialIGridHelper.GetIgridItem(items, "ZPKG_SEC_LID_TYPE", context);

                                                if ((ZPKG_SEC_PRIMARY_SIZE_VALUE != null && !String.IsNullOrEmpty(ZPKG_SEC_PRIMARY_SIZE_VALUE.CHARACTERISTIC_DESCRIPTION))
                                                   && (ZPKG_SEC_CONTAINER_TYPE_VALUE != null && !String.IsNullOrEmpty(ZPKG_SEC_CONTAINER_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION))
                                                   && (ZPKG_SEC_LID_TYPE_VALUE != null && !String.IsNullOrEmpty(ZPKG_SEC_LID_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION)))
                                                {
                                                    var threeP = (from p in context.SAP_M_3P
                                                                  where p.PRIMARY_SIZE_VALUE == ZPKG_SEC_PRIMARY_SIZE_VALUE.CHARACTERISTIC_DESCRIPTION
                                                                  && p.CONTAINER_TYPE_VALUE == ZPKG_SEC_CONTAINER_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION
                                                                  && p.LID_TYPE_VALUE == ZPKG_SEC_LID_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION
                                                                  select p).FirstOrDefault();

                                                    if (threeP != null)
                                                    {
                                                        processPA.THREE_P_ID = threeP.THREE_P_ID;
                                                    }
                                                    else
                                                    {
                                                        var ZPKG_SEC_PRIMARY_SIZE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PRIMARY_SIZE", context);
                                                        if (ZPKG_SEC_PRIMARY_SIZE != null)
                                                        {
                                                            processPA.PRIMARY_SIZE_ID = ZPKG_SEC_PRIMARY_SIZE;
                                                            processPA.PRIMARY_SIZE_OTHER = null;
                                                        }
                                                        else
                                                        {
                                                            if (ZPKG_SEC_PRIMARY_SIZE_VALUE != null)
                                                            {
                                                                processPA.THREE_P_ID = -1;
                                                                processPA.PRIMARY_SIZE_ID = -1;
                                                                processPA.PRIMARY_SIZE_OTHER = ZPKG_SEC_PRIMARY_SIZE_VALUE.CHARACTERISTIC_DESCRIPTION;
                                                            }
                                                        }

                                                        var ZPKG_SEC_CONTAINER_TYPE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_CONTAINER_TYPE", context);
                                                        if (ZPKG_SEC_CONTAINER_TYPE != null)
                                                        {
                                                            processPA.CONTAINER_TYPE_ID = ZPKG_SEC_CONTAINER_TYPE;
                                                            processPA.CONTAINER_TYPE_OTHER = null;
                                                        }
                                                        else
                                                        {
                                                            if (ZPKG_SEC_CONTAINER_TYPE_VALUE != null)
                                                            {
                                                                processPA.CONTAINER_TYPE_ID = -1;
                                                                processPA.CONTAINER_TYPE_OTHER = ZPKG_SEC_CONTAINER_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION;
                                                            }
                                                        }

                                                        var ZPKG_SEC_LID_TYPE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_LID_TYPE", context);
                                                        if (ZPKG_SEC_LID_TYPE != null)
                                                        {
                                                            processPA.LID_TYPE_ID = ZPKG_SEC_LID_TYPE;
                                                           processPA.LID_TYPE_OTHER = null;
                                                        }
                                                        else
                                                        {
                                                            if (ZPKG_SEC_CONTAINER_TYPE_VALUE != null)
                                                            {
                                                                processPA.LID_TYPE_ID = -1;
                                                                processPA.LID_TYPE_OTHER = ZPKG_SEC_LID_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION;
                                                            }
                                                        }
                                                    }

                                                }
                                                else
                                                {

                                                    var ZPKG_SEC_PRIMARY_SIZE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PRIMARY_SIZE", context);
                                                    if (ZPKG_SEC_PRIMARY_SIZE != null)
                                                    {
                                                        processPA.PRIMARY_SIZE_ID = ZPKG_SEC_PRIMARY_SIZE;
                                                        processPA.PRIMARY_SIZE_OTHER = null;
                                                    }
                                                    else
                                                    {
                                                        if (ZPKG_SEC_PRIMARY_SIZE_VALUE != null)
                                                        {
                                                            processPA.THREE_P_ID = -1;
                                                            processPA.PRIMARY_SIZE_ID = -1;
                                                            processPA.PRIMARY_SIZE_OTHER = ZPKG_SEC_PRIMARY_SIZE_VALUE.CHARACTERISTIC_DESCRIPTION;
                                                        }
                                                    }

                                                    var ZPKG_SEC_CONTAINER_TYPE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_CONTAINER_TYPE", context);
                                                    if (ZPKG_SEC_CONTAINER_TYPE != null)
                                                    {
                                                        processPA.CONTAINER_TYPE_ID = ZPKG_SEC_CONTAINER_TYPE;
                                                        processPA.CONTAINER_TYPE_OTHER = null;
                                                    }
                                                    else
                                                    {
                                                        if (ZPKG_SEC_CONTAINER_TYPE_VALUE != null)
                                                        {
                                                            processPA.CONTAINER_TYPE_ID = -1;
                                                            processPA.CONTAINER_TYPE_OTHER = ZPKG_SEC_CONTAINER_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION;
                                                        }
                                                    }

                                                    var ZPKG_SEC_LID_TYPE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_LID_TYPE", context);
                                                    if (ZPKG_SEC_LID_TYPE != null)
                                                    {
                                                        processPA.LID_TYPE_ID = ZPKG_SEC_LID_TYPE;
                                                        processPA.LID_TYPE_OTHER = null;
                                                    }
                                                    else
                                                    {
                                                        if (ZPKG_SEC_LID_TYPE_VALUE != null)
                                                        {
                                                            processPA.LID_TYPE_ID = -1;
                                                            processPA.LID_TYPE_OTHER = ZPKG_SEC_LID_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION;
                                                        }
                                                    }
                                                }


                                                var ZPKG_SEC_PACKING_STYLE_VALUE = MaterialIGridHelper.GetIgridItem(items, "ZPKG_SEC_PACKING_STYLE", context);
                                                var ZPKG_SEC_PACKING_VALUE = MaterialIGridHelper.GetIgridItem(items, "ZPKG_SEC_PACKING", context);

                                                if ((ZPKG_SEC_PACKING_STYLE_VALUE != null && !String.IsNullOrEmpty(ZPKG_SEC_PACKING_STYLE_VALUE.CHARACTERISTIC_DESCRIPTION))
                                                   && (ZPKG_SEC_PACKING_VALUE != null && !String.IsNullOrEmpty(ZPKG_SEC_PACKING_VALUE.CHARACTERISTIC_DESCRIPTION)))
                                                {
                                                    var twoP = (from p in context.SAP_M_2P
                                                                where p.PACKING_SYLE_VALUE == ZPKG_SEC_PACKING_STYLE_VALUE.CHARACTERISTIC_DESCRIPTION
                                                                && p.PACK_SIZE_VALUE == ZPKG_SEC_PACKING_VALUE.CHARACTERISTIC_DESCRIPTION
                                                                select p).FirstOrDefault();

                                                    if (twoP != null)
                                                    {
                                                        processPA.TWO_P_ID = twoP.TWO_P_ID;
                                                    }
                                                    else
                                                    {
                                                        var ZPKG_SEC_PACKING_STYLE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PACKING_STYLE", context);
                                                        if (ZPKG_SEC_PACKING_STYLE != null)
                                                        {
                                                            processPA.PACKING_STYLE_ID = ZPKG_SEC_PACKING_STYLE;
                                                            processPA.PACKING_STYLE_OTHER = null;
                                                        }
                                                        else
                                                        {
                                                            if (ZPKG_SEC_PACKING_STYLE_VALUE != null)
                                                            {
                                                                processPA.TWO_P_ID = -1;
                                                                processPA.PACKING_STYLE_ID = -1;
                                                                processPA.PACKING_STYLE_OTHER = ZPKG_SEC_PACKING_STYLE_VALUE.CHARACTERISTIC_DESCRIPTION;
                                                            }
                                                        }

                                                        var ZPKG_SEC_PACKING = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PACKING", context);
                                                        if (ZPKG_SEC_PACKING != null)
                                                        {
                                                            processPA.PACK_SIZE_ID = ZPKG_SEC_PACKING;
                                                            processPA.PACK_SIZE_OTHER = null;
                                                        }
                                                        else
                                                        {
                                                            if (ZPKG_SEC_PACKING_VALUE != null)
                                                            {
                                                                processPA.PACK_SIZE_ID = -1;
                                                                processPA.PACK_SIZE_OTHER = ZPKG_SEC_PACKING_VALUE.CHARACTERISTIC_DESCRIPTION;
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    var ZPKG_SEC_PACKING_STYLE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PACKING_STYLE", context);
                                                    if (ZPKG_SEC_PACKING_STYLE != null)
                                                    {
                                                        processPA.PACKING_STYLE_ID = ZPKG_SEC_PACKING_STYLE;
                                                        processPA.PACKING_STYLE_OTHER = null;
                                                    }
                                                    else
                                                    {
                                                        if (ZPKG_SEC_PACKING_STYLE_VALUE != null)
                                                        {
                                                            processPA.TWO_P_ID = -1;
                                                            processPA.PACKING_STYLE_ID = -1;
                                                            processPA.PACKING_STYLE_OTHER = ZPKG_SEC_PACKING_STYLE_VALUE.CHARACTERISTIC_DESCRIPTION;
                                                        }
                                                    }

                                                    var ZPKG_SEC_PACKING = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PACKING", context);
                                                    if (ZPKG_SEC_PACKING != null)
                                                    {
                                                        processPA.PACK_SIZE_ID = ZPKG_SEC_PACKING;
                                                        processPA.PACK_SIZE_OTHER = null;
                                                    }
                                                    else
                                                    {
                                                        if (ZPKG_SEC_PACKING_VALUE != null)
                                                        {
                                                            processPA.PACK_SIZE_ID = -1;
                                                            processPA.PACK_SIZE_OTHER = ZPKG_SEC_PACKING_VALUE.CHARACTERISTIC_DESCRIPTION;
                                                        }
                                                    }
                                                }

                                                var ZPKG_SEC_PLANT_REGISTER = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PLANT_REGISTER", context);
                                                if (ZPKG_SEC_PLANT_REGISTER != null)
                                                {
                                                    processPA.PLANT_REGISTERED_ID = ZPKG_SEC_PLANT_REGISTER;
                                                    processPA.PLANT_REGISTERED_OTHER = null;
                                                }

                                                var ZPKG_SEC_COMPANY_ADR = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_COMPANY_ADR", context);
                                                if (ZPKG_SEC_COMPANY_ADR != null)
                                                {
                                                    processPA.COMPANY_ADDRESS_ID = ZPKG_SEC_COMPANY_ADR;
                                                    processPA.COMPANY_ADDRESS_OTHER = null;
                                                }

                                                var ZPKG_SEC_CATCHING_PERIOD = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_CATCHING_PERIOD", context);
                                                if (ZPKG_SEC_CATCHING_PERIOD != null)
                                                {
                                                    processPA.CATCHING_PERIOD_ID = ZPKG_SEC_CATCHING_PERIOD;
                                                    processPA.CATCHING_PERIOD_OTHER = null;
                                                }

                                                var ZPKG_SEC_CATCHING_METHOD = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_CATCHING_METHOD", context);
                                                if (ZPKG_SEC_CATCHING_METHOD != null)
                                                {
                                                    processPA.CATCHING_METHOD_ID = ZPKG_SEC_CATCHING_METHOD;
                                                    processPA.CATCHING_METHOD_OTHER = null;
                                                }

                                                var ZPKG_SEC_SCIENTIFIC_NAME = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_SCIENTIFIC_NAME", context);
                                                if (ZPKG_SEC_SCIENTIFIC_NAME != null)
                                                {
                                                    processPA.SCIENTIFIC_NAME_ID = ZPKG_SEC_SCIENTIFIC_NAME;
                                                    processPA.SCIENTIFIC_NAME_OTHER = null;
                                                }

                                                var ZPKG_SEC_DIRECTION = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_DIRECTION", context);
                                                if (ZPKG_SEC_DIRECTION != null)
                                                {
                                                    processPA.DIRECTION_OF_STICKER_ID = ZPKG_SEC_DIRECTION;
                                                    processPA.DIRECTION_OF_STICKER_OTHER = null;
                                                }

                                                var ZPKG_SEC_SPECIE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_SPECIE", context);
                                                if (ZPKG_SEC_SPECIE != null)
                                                {
                                                    processPA.SPECIE_ID = ZPKG_SEC_SPECIE;
                                                    processPA.SPECIE_OTHER = null;
                                                }

                                                var productionPlants = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_PRODUCTION_PLANT").ToList();

                                                if (productionPlants != null)
                                                {
                                                    string productionPlant = "";

                                                    foreach (var iProductPlant in productionPlants)
                                                    {
                                                        if (!String.IsNullOrEmpty(iProductPlant.CHARACTERISTIC_VALUE) && !productionPlant.Contains(iProductPlant.CHARACTERISTIC_VALUE))
                                                        {
                                                            if (String.IsNullOrEmpty(productionPlant))
                                                            {
                                                                productionPlant = iProductPlant.CHARACTERISTIC_VALUE;

                                                            }
                                                            else
                                                            {
                                                                productionPlant += "," + iProductPlant.CHARACTERISTIC_VALUE;
                                                            }
                                                        }
                                                    }

                                                    if (!String.IsNullOrEmpty(productionPlant))
                                                    {
                                                        processPA.PRODICUTION_PLANT_ID = -1;
                                                        processPA.PRODICUTION_PLANT_OTHER = productionPlant;
                                                    }
                                                }

                                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA, context);

                                                #region "PLANT"
                                                var plant = context.ART_WF_ARTWORK_PROCESS_PA_PLANT
                                                            .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                                            .ToList();
                                                if (plant != null && plant.Count > 0)
                                                {
                                                    foreach (ART_WF_ARTWORK_PROCESS_PA_PLANT iPlant in plant)
                                                    {
                                                        ART_WF_ARTWORK_PROCESS_PA_PLANT_SERVICE.DeleteByARTWORK_SUB_PA_PLANT_ID(iPlant.ARTWORK_SUB_PA_PLANT_ID, context);
                                                    }
                                                }

                                                if (!String.IsNullOrEmpty(header.PLANT))
                                                {
                                                    List<string> listPlants = new List<string>();

                                                    listPlants = header.PLANT.Split(new string[] { ";" }, StringSplitOptions.None).ToList();

                                                    if (listPlants.Count > 0)
                                                    {
                                                        ART_WF_ARTWORK_PROCESS_PA_PLANT plantNew = new ART_WF_ARTWORK_PROCESS_PA_PLANT();
                                                        foreach (string iPlant in listPlants)
                                                        {
                                                            var plantTmp = (from p in context.SAP_M_PLANT
                                                                            where p.PLANT == iPlant
                                                                            select p).FirstOrDefault();

                                                            if (plantTmp != null)
                                                            {
                                                                plantNew = new ART_WF_ARTWORK_PROCESS_PA_PLANT();
                                                                plantNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                                                plantNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                                                plantNew.PLANT_ID = plantTmp.PLANT_ID;
                                                                ART_WF_ARTWORK_PROCESS_PA_PLANT_SERVICE.SaveOrUpdateNoLog(plantNew, context);
                                                            }
                                                        }
                                                    }
                                                }


                                                #endregion

                                                #region "FAO_ZONE"
                                                var fao = context.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE
                                                            .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                                            .ToList();
                                                if (fao != null && fao.Count > 0)
                                                {
                                                    foreach (ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE iFAO in fao)
                                                    {
                                                        ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.DeleteByARTWORK_SUB_PA_FAO_ID(iFAO.ARTWORK_SUB_PA_FAO_ID, context);
                                                    }
                                                }

                                                var faoNews = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_FAO").ToList();

                                                if (faoNews != null && faoNews.Count > 0)
                                                {
                                                    ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE fao_new = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                                                    foreach (var iFAONew in faoNews)
                                                    {
                                                        var fap_tmp = (from c in context.SAP_M_CHARACTERISTIC
                                                                       where c.NAME == "ZPKG_SEC_FAO"
                                                                         && c.VALUE == iFAONew.CHARACTERISTIC_VALUE
                                                                       select c).FirstOrDefault();

                                                        if (fap_tmp != null)
                                                        {
                                                            fao_new = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                                                            fao_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                                            fao_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                                            fao_new.FAO_ZONE_ID = fap_tmp.CHARACTERISTIC_ID;
                                                            ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.SaveOrUpdateNoLog(fao_new, context);
                                                        }
                                                    }
                                                }

                                                #endregion

                                                #region "CATCHING_AREA"
                                                var catchingArea = context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA
                                                          .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                                          .ToList();
                                                if (catchingArea != null && catchingArea.Count > 0)
                                                {
                                                    foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA iChatching in catchingArea)
                                                    {
                                                        ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE.DeleteByARTWORK_SUB_PA_CATCHING_AREA_ID(iChatching.ARTWORK_SUB_PA_CATCHING_AREA_ID, context);
                                                    }
                                                }

                                                var catchingNews = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_CATCHING_AREA").ToList();

                                                if (catchingNews != null && catchingNews.Count > 0)
                                                {
                                                    ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA catching_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                                                    foreach (var iCatchingNew in catchingNews)
                                                    {
                                                        var catching_tmp = (from c in context.SAP_M_CHARACTERISTIC
                                                                            where c.NAME == "ZPKG_SEC_CATCHING_AREA"
                                                                              && c.VALUE == iCatchingNew.CHARACTERISTIC_VALUE
                                                                            select c).FirstOrDefault();

                                                        if (catching_tmp != null)
                                                        {
                                                            catching_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                                                            catching_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                                            catching_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                                            catching_new.CATCHING_AREA_ID = catching_tmp.CHARACTERISTIC_ID;
                                                            ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE.SaveOrUpdateNoLog(catching_new, context);
                                                        }
                                                    }
                                                }

                                                #endregion


                                                #region "CATCHING_AREA"
                                                //ticke#425737 added by aof 
                                                var catchingMethod = context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD
                                                          .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                                          .ToList();
                                                if (catchingMethod != null && catchingMethod.Count > 0)
                                                {
                                                    foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD iMethod in catchingMethod)
                                                    {
                                                        ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE.DeleteByARTWORK_SUB_PA_CATCHING_METHOD_ID(iMethod.ARTWORK_SUB_PA_CATCHING_METHOD_ID, context);
                                                    }
                                                }

                                                var methodNews = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_CATCHING_METHOD").ToList();

                                                if (methodNews != null && methodNews.Count > 0)
                                                {
                                                    ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD method_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                                                    foreach (var iMethodNew in methodNews)
                                                    {
                                                        var method_tmp = (from c in context.SAP_M_CHARACTERISTIC
                                                                            where c.NAME == "ZPKG_SEC_CATCHING_METHOD"
                                                                              && c.VALUE == iMethodNew.CHARACTERISTIC_VALUE
                                                                            select c).FirstOrDefault();

                                                        if (method_tmp != null)
                                                        {
                                                            method_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                                                            method_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                                            method_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                                            method_new.CATCHING_METHOD_ID = method_tmp.CHARACTERISTIC_ID;
                                                            ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE.SaveOrUpdateNoLog(method_new, context);
                                                        }
                                                    }
                                                }
                                                //ticke#425737 added by aof 
                                                #endregion

                                                #region "SYMBOL"
                                                var symbol = context.ART_WF_ARTWORK_PROCESS_PA_SYMBOL
                                                          .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                                          .ToList();
                                                if (symbol != null && symbol.Count > 0)
                                                {
                                                    foreach (ART_WF_ARTWORK_PROCESS_PA_SYMBOL iChatching in symbol)
                                                    {
                                                        ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE.DeleteByARTWORK_SUB_PA_SYMBOL_ID(iChatching.ARTWORK_SUB_PA_SYMBOL_ID, context);
                                                    }
                                                }

                                                var symbolNews = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_SYMBOL").ToList();

                                                if (symbolNews != null && symbolNews.Count > 0)
                                                {
                                                    ART_WF_ARTWORK_PROCESS_PA_SYMBOL symbol_new = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                                                    foreach (var iSymbolNew in symbolNews)
                                                    {
                                                        var symbol_tmp = (from c in context.SAP_M_CHARACTERISTIC
                                                                          where c.NAME == "ZPKG_SEC_SYMBOL"
                                                                            && c.VALUE == iSymbolNew.CHARACTERISTIC_VALUE
                                                                          select c).FirstOrDefault();

                                                        if (symbol_tmp != null)
                                                        {
                                                            symbol_new = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                                                            symbol_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                                            symbol_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                                            symbol_new.SYMBOL_ID = symbol_tmp.CHARACTERISTIC_ID;
                                                            ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE.SaveOrUpdateNoLog(symbol_new, context);

                                                        }
                                                    }
                                                }

                                                #endregion
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        dbContextTransaction.Commit();
                    }

                }

                if (param.OUTBOUND_HEADERS != null && param.OUTBOUND_HEADERS.Count > 0)
                {
                    foreach (IGRID_OUTBOUND_HEADER_MODEL iHeader in param.OUTBOUND_HEADERS)
                    {
                        if (!String.IsNullOrEmpty(iHeader.MaterialNumber))
                        {
                            using (var context = new ARTWORKEntities())
                            {
                                using (var dbContextTransaction = CNService.IsolationLevel(context))
                                {
                                    context.Database.CommandTimeout = 600;

                                    CNService.InsertMaterialLock(iHeader.MaterialNumber, "", "", context,"73");
                                    dbContextTransaction.Commit();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }


            return Results;
        }

        private static SERVICE_RESULT_MODEL InsertVendorInMaterialConvertion(IGRID_OUTBOUND_MODEL param)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 600;

                        SAP_M_MATERIAL_CONVERSION materialConversionNew = new SAP_M_MATERIAL_CONVERSION();

                        foreach (IGRID_OUTBOUND_HEADER_MODEL iHeader in param.OUTBOUND_HEADERS)
                        {
                            var header = (from h in context.IGRID_M_OUTBOUND_HEADER
                                          where h.ARTWORK_NO == iHeader.ArtworkNumber
                                          && h.DATE == iHeader.Date
                                          && h.TIME == iHeader.Time
                                          select h).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                            if (header != null)
                            {
                                var items = (from h in context.IGRID_M_OUTBOUND_ITEM
                                             where h.ARTWORK_NO == header.ARTWORK_NO
                                                && h.DATE == header.DATE
                                                && h.TIME == header.TIME
                                             select h).ToList();

                                if (items.Count() > 0)
                                {
                                    string materialNo = header.MATERIAL_NUMBER.Trim();
                                    string charVendorName = "ZPKG_SEC_VENDOR";
                                    string charChangePoint = "ZPKG_SEC_CHANGE_POINT";

                                    var iVendor = items.Where(w => w.CHARACTERISTIC_NAME == charVendorName).FirstOrDefault();
                                    var iChangePoint = items.Where(w => w.CHARACTERISTIC_NAME == charChangePoint).FirstOrDefault();

                                    if (iVendor != null)
                                    {
                                        var matCoversion_Vendor = (from m in context.SAP_M_MATERIAL_CONVERSION
                                                                   where m.MATERIAL_NO == materialNo && m.CHAR_NAME == charVendorName
                                                                   select m).FirstOrDefault();

                                        if (matCoversion_Vendor != null)
                                        {
                                            materialConversionNew.SAP_M_MATERIAL_CONVERSION_ID = matCoversion_Vendor.SAP_M_MATERIAL_CONVERSION_ID;
                                        }

                                        materialConversionNew.MATERIAL_NO = materialNo;
                                        materialConversionNew.CHAR_NAME = charVendorName;
                                        materialConversionNew.CHAR_VALUE = iVendor.CHARACTERISTIC_VALUE;
                                        materialConversionNew.CHAR_DESCRIPTION = iVendor.CHARACTERISTIC_DESCRIPTION;
                                        materialConversionNew.IS_ACTIVE = "X";
                                        materialConversionNew.CREATE_BY = -2;
                                        materialConversionNew.UPDATE_BY = -2;

                                        SAP_M_MATERIAL_CONVERSION_SERVICE.SaveOrUpdateNoLog(materialConversionNew, context);


                                    }

                                    if (iChangePoint != null)
                                    {
                                        materialConversionNew = new SAP_M_MATERIAL_CONVERSION();

                                        var matCoversion_ChangPoint = (from m in context.SAP_M_MATERIAL_CONVERSION
                                                                       where m.MATERIAL_NO == materialNo && m.CHAR_NAME == charChangePoint
                                                                       select m).FirstOrDefault();

                                        if (matCoversion_ChangPoint != null)
                                        {
                                            materialConversionNew.SAP_M_MATERIAL_CONVERSION_ID = matCoversion_ChangPoint.SAP_M_MATERIAL_CONVERSION_ID;
                                        }

                                        materialConversionNew.MATERIAL_NO = materialNo;
                                        materialConversionNew.CHAR_NAME = charChangePoint;
                                        materialConversionNew.CHAR_VALUE = iChangePoint.CHARACTERISTIC_VALUE;
                                        materialConversionNew.CHAR_DESCRIPTION = iChangePoint.CHARACTERISTIC_DESCRIPTION;
                                        materialConversionNew.IS_ACTIVE = "X";
                                        materialConversionNew.CREATE_BY = -2;
                                        materialConversionNew.UPDATE_BY = -2;

                                        SAP_M_MATERIAL_CONVERSION_SERVICE.SaveOrUpdateNoLog(materialConversionNew, context);
                                    }
                                }
                            }
                        }

                        dbContextTransaction.Commit();
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
    }

}