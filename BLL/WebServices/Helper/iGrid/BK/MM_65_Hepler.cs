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
using WebServices.Helper;
using BLL.BizMM65Service;
using System.Web.Script.Serialization;
using System.Data;

namespace WebServices.Helper
{
    public static class MM_65_Hepler
    {
        public static MM65_RESULT RequestMaterial(MM65_REQUEST param)
        {
            MM65_RESULT Results = new MM65_RESULT();

            string urlForm = ConfigurationManager.AppSettings["ArtworkURLTaskForm"];
            MM65Client client = new MM65Client();
            ArtworkObject artworkObj = new ArtworkObject();
            List<InboundArtwork> inboundArtworks = new List<InboundArtwork>();
            List<InboundArtwork> inboundArtworks_PG = new List<InboundArtwork>();

            if (param.data.ARTWORK_SUB_ID > 0)
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        var stepPG = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == "SEND_PG").FirstOrDefault();
                        var stepPA = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == "SEND_PA").FirstOrDefault();

                        var process = (from p in context.ART_WF_ARTWORK_PROCESS
                                       where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                       select p).FirstOrDefault();

                        var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                         where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                         select p).FirstOrDefault();

                        var requestItem = (from p in context.ART_WF_ARTWORK_REQUEST_ITEM
                                           where p.ARTWORK_ITEM_ID == process.ARTWORK_ITEM_ID
                                           select p).FirstOrDefault();


                        if (param.data.RECORD_TYPE == "I")
                        {
                            if (processPA.REQUEST_MATERIAL_STATUS != "Canceled")
                            {
                                if (!String.IsNullOrEmpty(processPA.REQUEST_MATERIAL_STATUS))
                                {
                                    Results.status = "E";
                                    Results.msg = MessageHelper.GetMessage("MSG_030", context);

                                    return Results;
                                }
                            }

                            if (processPA.RD_REFERENCE_NO_ID != null)
                            {
                                var paProduct = (from p in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT
                                                 where p.ARTWORK_SUB_ID == processPA.ARTWORK_SUB_ID
                                                 select p.ARTWORK_SUB_PA_PRODUCT_ID).Count();

                                var paProductOther = (from p in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER
                                                      where p.ARTWORK_SUB_ID == processPA.ARTWORK_SUB_ID
                                                      select p.ARTWORK_SUB_PA_PRODUCT_OTHER_ID).Count();

                                if (paProduct <= 0 && paProductOther <= 0)
                                {
                                    Results.status = "E";
                                    Results.msg = "Please select required field in PA Data (Product Code) before request material."; ;

                                    return Results;
                                }

                            }
                        }
                        else if (param.data.RECORD_TYPE == "U")
                        {
                            var matPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                         where p.MATERIAL_NO == processPA.MATERIAL_NO
                                         && p.ARTWORK_SUB_ID != param.data.ARTWORK_SUB_ID
                                         && p.REQUEST_MATERIAL_STATUS.Contains("Waiting")
                                         select p).FirstOrDefault();

                            if (matPA != null)
                            {
                                Results.status = "E";
                                Results.msg = "Unable to update Material No. " + processPA.MATERIAL_NO + ", another workflow is updating " + processPA.MATERIAL_NO + " on iGrid";
                                //"Material No. " + processPA.MATERIAL_NO + " can not update material because waiting for approve in another Artwork.";
                                return Results;
                            }
                        }
                        var existData = (from e in context.IGRID_M_OUTBOUND_HEADER
                                         where e.ARTWORK_NO == requestItem.REQUEST_ITEM_NO
                                         select e).FirstOrDefault();
                        List<int> allSubID = new List<int>();

                        allSubID = CNService.FindArtworkSubId(param.data.ARTWORK_SUB_ID, context);

                        var processPG = (from p in context.ART_WF_ARTWORK_PROCESS_PG
                                         where allSubID.Contains(p.ARTWORK_SUB_ID)
                                         && p.DIE_LINE_MOCKUP_ID != null
                                         select p).OrderByDescending(o => o.ARTWORK_SUB_PA_ID).FirstOrDefault();

                        if (!String.IsNullOrEmpty(urlForm))
                        {
                            urlForm = urlForm + param.data.ARTWORK_SUB_ID.ToString();
                        }

                        string dateStr = "";
                        string noteOfPG = "";
                        string timeStr = "";
                        string PrintingStyleofPrimary = "";
                        string PrintingStyleofSecondary = "";

                        string Status = "";


                        dateStr = DateTime.Now.ToString("yyyyMMdd");
                        timeStr = DateTime.Now.ToString("HH:mm:ss");

                        //check require field
                        string msgError = "Please select required field in PA Data ({0}) before request material or update material";
                        string msg = "";

                        if (processPA.MATERIAL_GROUP_ID == null)
                        {
                            if (msg.Length > 0) msg += ", ";
                            msg += "Material group";
                        }
                        else
                        {
                            var matGroupCode = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(processPA.MATERIAL_GROUP_ID, context).VALUE;
                            if (matGroupCode != null)
                            {
                                switch (matGroupCode.ToUpper())
                                {
                                    case "K":
                                        if (processPA.STYLE_OF_PRINTING_ID == null)
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }
                                        else if (processPA.STYLE_OF_PRINTING_ID == -1 && String.IsNullOrEmpty(processPA.STYLE_OF_PRINTING_OTHER))
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }
                                        break;
                                    case "L":
                                        //nothing
                                        break;
                                    case "P":
                                        if (processPA.STYLE_OF_PRINTING_ID == null)
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }
                                        else if (processPA.STYLE_OF_PRINTING_ID == -1 && String.IsNullOrEmpty(processPA.STYLE_OF_PRINTING_OTHER))
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }
                                        break;
                                    case "J":
                                        if (processPA.STYLE_OF_PRINTING_ID == null)
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }
                                        else if (processPA.STYLE_OF_PRINTING_ID == -1 && String.IsNullOrEmpty(processPA.STYLE_OF_PRINTING_OTHER))
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }

                                        if (processPA.DIRECTION_OF_STICKER_ID == null)
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Direction of sticker";
                                        }
                                        else if (processPA.DIRECTION_OF_STICKER_ID == -1 && String.IsNullOrEmpty(processPA.DIRECTION_OF_STICKER_OTHER))
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Direction of sticker";
                                        }
                                        //if (processPA.CUSTOMER_DESIGN == null)
                                        //{
                                        //    if (msg.Length > 0) msg += ", ";
                                        //    msg += "Customer's design";
                                        //}
                                        //if (processPA.CUSTOMER_DESIGN_OTHER == null || processPA.CUSTOMER_DESIGN_OTHER == "")
                                        //{
                                        //    if (msg.Length > 0) msg += ", ";
                                        //    msg += "Customer's design detail";
                                        //}
                                        break;
                                    default:
                                        if (processPA.TWO_P_ID == null)
                                        {
                                            msg += "Packing style or Pack size";

                                        }
                                        else if (processPA.TWO_P_ID != null && processPA.TWO_P_ID == -1)
                                        {
                                            if (String.IsNullOrEmpty(processPA.PACKING_STYLE_OTHER))
                                            {
                                                msg += "Packing style";
                                            }

                                            if (processPA.PACK_SIZE_ID == null)
                                            {
                                                if (msg.Length > 0) msg += ", ";
                                                msg += "Pack size";
                                            }
                                            else if (processPA.TWO_P_ID == -1 && processPA.PACK_SIZE_ID == -1 && String.IsNullOrEmpty(processPA.PACK_SIZE_OTHER))
                                            {
                                                if (msg.Length > 0) msg += ", ";
                                                msg += "Pack size";
                                            }
                                        }

                                        if (processPA.STYLE_OF_PRINTING_ID == null)
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }
                                        else if (processPA.STYLE_OF_PRINTING_ID == -1 && String.IsNullOrEmpty(processPA.STYLE_OF_PRINTING_OTHER))
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }
                                        break;
                                }

                            }
                        }
                        if (processPA.THREE_P_ID == null)
                        {
                            if (msg.Length > 0) msg += ", ";
                            msg += "Primary size";
                        }
                        else if (processPA.THREE_P_ID == -1)
                        {
                            if (String.IsNullOrEmpty(processPA.PRIMARY_SIZE_OTHER))
                            {
                                msg += "Packing style";
                            }

                            if (processPA.CONTAINER_TYPE_ID == null)
                            {
                                if (msg.Length > 0) msg += ", ";
                                msg += "Container type";
                            }
                            else if (processPA.THREE_P_ID == -1 && processPA.CONTAINER_TYPE_ID == -1 && String.IsNullOrEmpty(processPA.CONTAINER_TYPE_OTHER))
                            {
                                if (msg.Length > 0) msg += ", ";
                                msg += "Container type";
                            }

                            if (processPA.LID_TYPE_ID == null)
                            {
                                if (msg.Length > 0) msg += ", ";
                                msg += "Lid type";
                            }
                            else if (processPA.THREE_P_ID == -1 && processPA.LID_TYPE_ID == -1 && String.IsNullOrEmpty(processPA.LID_TYPE_OTHER))
                            {
                                if (msg.Length > 0) msg += ", ";
                                msg += "Lid type";
                            }

                        }


                        if (processPA.TOTAL_COLOUR_ID == null)
                        {
                            if (msg.Length > 0) msg += ", ";
                            msg += "Total colour";
                        }
                        else if (processPA.TOTAL_COLOUR_ID == -1 && String.IsNullOrEmpty(processPA.TOTAL_COLOUR_OTHER))
                        {
                            if (msg.Length > 0) msg += ", ";
                            msg += "Total colour";
                        }

                        if (msg.Length > 0)
                        {
                            Results.status = "E";
                            Results.msg = string.Format(msgError, msg);
                            return Results;
                        }

                        if (processPA.PRINTING_STYLE_OF_PRIMARY_ID != null)
                        {
                            var print_1 = context.ART_M_PRINTING_STYLE.Where(p => p.PRINTING_STYLE_ID == processPA.PRINTING_STYLE_OF_PRIMARY_ID).FirstOrDefault();
                            PrintingStyleofPrimary = print_1.PRINTING_STYLE_DESCRIPTION;
                        }
                        else if (!String.IsNullOrEmpty(processPA.PRINTING_STYLE_OF_PRIMARY_OTHER))
                        {
                            PrintingStyleofPrimary = processPA.PRINTING_STYLE_OF_PRIMARY_OTHER;
                        }

                        if (processPA.PRINTING_STYLE_OF_SECONDARY_ID != null)
                        {
                            var print_2 = context.ART_M_PRINTING_STYLE.Where(p => p.PRINTING_STYLE_ID == processPA.PRINTING_STYLE_OF_SECONDARY_ID).FirstOrDefault();
                            PrintingStyleofSecondary = print_2.PRINTING_STYLE_DESCRIPTION;
                        }
                        else if (!String.IsNullOrEmpty(processPA.PRINTING_STYLE_OF_SECONDARY_OTHER))
                        {
                            PrintingStyleofSecondary = processPA.PRINTING_STYLE_OF_SECONDARY_OTHER;
                        }

                        var plants = (from p in context.ART_WF_ARTWORK_PROCESS_PA_PLANT
                                      where p.ARTWORK_SUB_ID == processPA.ARTWORK_SUB_ID
                                         && p.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID
                                      select p).ToList();

                        if (plants != null && plants.Count > 0)
                        {
                            string plantCode = "";

                            foreach (ART_WF_ARTWORK_PROCESS_PA_PLANT item in plants)
                            {
                                if (String.IsNullOrEmpty(item.PLANT_OTHER))
                                {
                                    var plant = context.SAP_M_PLANT.Where(p => p.PLANT_ID == item.PLANT_ID).FirstOrDefault();
                                    plantCode += plant.PLANT + ";";
                                }
                                else
                                {
                                    plantCode += item.PLANT_OTHER + ";";
                                }
                            }

                            plantCode = plantCode.Substring(0, plantCode.Length - 1);

                            artworkObj.Plant = plantCode;
                        }
                        else
                        {
                            Results.status = "E";
                            Results.msg = string.Format(msgError, "Plant");
                            return Results;
                        }

                        bool is_total_colour = false;
                        bool is_style_of_printing = false;
                        if (processPG != null)
                        {
                            noteOfPG = processPG.COMMENT;

                            if (processPG.DIE_LINE_MOCKUP_ID != null)
                            {
                                var checklist_pg = (from p in context.ART_WF_MOCKUP_CHECK_LIST_PG
                                                    where p.MOCKUP_ID == processPG.DIE_LINE_MOCKUP_ID
                                                    select p).FirstOrDefault();

                                if (checklist_pg != null)
                                {
                                    is_total_colour = (checklist_pg.NUMBER_OF_COLOR_ID == processPA.TOTAL_COLOUR_ID);
                                }

                                var tempProcessTemplate_pg = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG() { MOCKUP_ID = processPG.DIE_LINE_MOCKUP_ID.GetValueOrDefault() }, context);
                                var processTemplate_pg = (from p in tempProcessTemplate_pg orderby p.UPDATE_DATE descending select p).FirstOrDefault();
                                if (processTemplate_pg != null)
                                {
                                    if (!string.IsNullOrEmpty(processTemplate_pg.STYLE_OF_PRINTING_OTHER))
                                    {
                                        is_style_of_printing = (processPA.STYLE_OF_PRINTING_OTHER == processTemplate_pg.STYLE_OF_PRINTING_OTHER);
                                    }
                                    else if (processTemplate_pg.STYLE_OF_PRINTING > 0)
                                    {
                                        is_style_of_printing = (processPA.STYLE_OF_PRINTING_ID == processTemplate_pg.STYLE_OF_PRINTING);
                                    }
                                }
                            }
                        }

                        string msgValidatePA_PG = "";

                        if (!is_total_colour)
                        {
                            msgValidatePA_PG = "Total colour (PA Data) and (PG Data) is mismatch <br>";
                        }

                        if (!is_style_of_printing)
                        {
                            msgValidatePA_PG += "Style of printing (PA Data) and (PG Data) is mismatch <br>";
                        }

                        if (!String.IsNullOrEmpty(msgValidatePA_PG))
                        {
                            Results.status = "E";
                            Results.msg = msgValidatePA_PG.Substring(0, msgValidatePA_PG.Length - 4);
                            return Results;
                        }
                        //if (existData != null)
                        //{
                        //    Status = "I";
                        //}
                        Status = "";

                        artworkObj.ArtworkNumber = requestItem.REQUEST_ITEM_NO;
                        artworkObj.Date = dateStr;
                        artworkObj.Time = timeStr;
                        artworkObj.RecordType = param.data.RECORD_TYPE;

                        if (param.data.RECORD_TYPE != "I" && !String.IsNullOrEmpty(processPA.MATERIAL_NO))
                        {
                            artworkObj.MaterialNumber = processPA.MATERIAL_NO;

                            var iGridHeader = (from g in context.IGRID_M_OUTBOUND_HEADER
                                               where g.ARTWORK_NO == requestItem.REQUEST_ITEM_NO
                                                && g.MATERIAL_NUMBER == processPA.MATERIAL_NO
                                               select g).FirstOrDefault();

                            if (iGridHeader != null)
                            {
                                artworkObj.MaterialDescription = iGridHeader.MATERIAL_DESCRIPTION;
                            }

                        }
                        else
                        {
                            artworkObj.MaterialNumber = "";
                            artworkObj.MaterialDescription = "";
                        }

                        artworkObj.MaterialCreatedDate = "";
                        artworkObj.ArtworkURL = urlForm;
                        artworkObj.Status = Status;
                        // artworkObj. = Status;

                        artworkObj.PAUserName = CNService.GetUserLogin(processPA.PA_USER_ID, context); //"MO600965";
                        artworkObj.PGUserName = CNService.GetUserLogin(processPA.PG_USER_ID, context); // "MO530515"; 

                        artworkObj.ReferenceMaterial = "";
                        if (param.data.RECORD_TYPE == "I")
                        {
                            artworkObj.ReferenceMaterial = param.data.REFERENCE_MATERIAL;
                        }
                        // artworkObj.Plant = Plant;
                        artworkObj.PrintingStyleofPrimary = PrintingStyleofPrimary;
                        artworkObj.PrintingStyleofSecondary = PrintingStyleofSecondary;
                        artworkObj.CustomersDesign = GetYesNoValue(processPA.CUSTOMER_DESIGN);
                        artworkObj.CustomersDesignDetail = processPA.CUSTOMER_DESIGN_OTHER;
                        artworkObj.CustomersSpec = GetYesNoValue(processPA.CUSTOMER_SPEC);
                        artworkObj.CustomersSpecDetail = processPA.CUSTOMER_SPEC_OTHER;
                        artworkObj.CustomersSize = GetYesNoValue(processPA.CUSTOMER_SIZE);
                        artworkObj.CustomersSizeDetail = processPA.CUSTOMER_SIZE_OTHER;
                        artworkObj.CustomerNominatesVendor = GetYesNoValue(processPA.CUSTOMER_NOMINATES_VENDOR);
                        artworkObj.CustomerNominatesVendorDetail = processPA.CUSTOMER_NOMINATES_VENDOR_OTHER;
                        artworkObj.CustomerNominatesColorPantone = GetYesNoValue(processPA.CUSTOMER_NOMINATES_COLOR);
                        artworkObj.CustomerNominatesColorPantoneDetail = processPA.CUSTOMER_NOMINATES_COLOR_OTHER;
                        artworkObj.CustomersBarcodeScanable = GetYesNoValue(processPA.CUSTOMER_BARCODE_SCANABLE);
                        artworkObj.CustomersBarcodeScanableDetail = processPA.CUSTOMER_BARCODE_SCANABLE_OTHER;
                        artworkObj.CustomersBarcodeSpec = GetYesNoValue(processPA.CUSTOMER_BARCODE_SCANABLE);
                        artworkObj.CustomersBarcodeSpecDetail = processPA.CUSTOMER_BARCODE_SPEC_OTHER;
                        artworkObj.FirstInfoGroup = processPA.FIRST_INFOGROUP_OTHER;
                        artworkObj.SONumber = "";
                        artworkObj.SOitem = "";
                        artworkObj.SOPlant = "";
                        artworkObj.PICMKT = "";
                        artworkObj.Destination = "";
                        artworkObj.RemarkNoteofPA = processPA.NOTE_OF_PA;
                        artworkObj.FinalInfoGroup = processPA.FIRST_INFOGROUP_OTHER;
                        artworkObj.RemarkNoteofPG = CNService.RemoveHTMLTag(noteOfPG);
                        artworkObj.CompleteInfoGroup = processPA.COMPLETE_INFOGROUP;
                        artworkObj.ProductionExpirydatesystem = processPA.PRODUCTION_EXPIRY_DATE_SYSTEM;
                        artworkObj.Seriousnessofcolorprinting = GetYesNoValue(processPA.SERIOUSNESS_OF_COLOR_PRINTING);
                        artworkObj.CustIngreNutritionAnalysis = GetYesNoValue(processPA.NUTRITION_ANALYSIS);
                        artworkObj.ShadeLimit = GetApproveValue(processPA.SHADE_LIMIT);
                        artworkObj.PackageQuantity = processPA.PACKAGE_QUANTITY;
                        artworkObj.WastePercent = processPA.WASTE_PERCENT;

                        string _validate = ValidateMM65_Header(artworkObj);
                        if (!String.IsNullOrEmpty(_validate))
                        {
                            Results.status = "E";
                            Results.msg = _validate + " " + MessageHelper.GetMessage("MSG_009", context);
                            return Results;
                        }

                        inboundArtworks = GetCharacteristicsValue_PA(artworkObj, processPA, context);

                        inboundArtworks_PG = GetCharacteristicsValue_PG(artworkObj, processPA, processPG, context);

                        if (inboundArtworks_PG != null && inboundArtworks_PG.Count > 0)
                        {
                            inboundArtworks.AddRange(inboundArtworks_PG);
                        }

                        inputArtworkNumber inArtwork = new inputArtworkNumber();

                        inArtwork._artworkObject = artworkObj;
                        inArtwork._itemsArtwork = inboundArtworks.ToArray();

                        inputArtworkNumberResponse inArtworkResp = new inputArtworkNumberResponse();

                        var json = CNService.Serialize(inArtwork);

                        _validate = "";
                        _validate = ValidateMM65_Item(json.ToString(), param);
                        if (!String.IsNullOrEmpty(_validate))
                        {
                            Results.status = "E";
                            Results.msg = _validate + " " + MessageHelper.GetMessage("MSG_009", context);
                            return Results;
                        }
                        //changed by voravut on 2020-05-08 move if return +++++++++++++++++++
                        if (param.data.RECORD_TYPE == "I")
                        {
                            if (processPA.REQUEST_MATERIAL_STATUS != "Canceled")
                            {
                                if (!String.IsNullOrEmpty(processPA.REQUEST_MATERIAL_STATUS))
                                {
                                    Results.status = "E";
                                    Results.msg = MessageHelper.GetMessage("MSG_030", context);

                                    return Results;
                                }
                            }
                        }
                        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        inArtworkResp = client.inputArtworkNumber(inArtwork);

                        ART_WF_ARTWORK_PROCESS_PA processPATmp = new ART_WF_ARTWORK_PROCESS_PA();
                        processPATmp.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                        processPATmp = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(processPATmp, context).FirstOrDefault();

                        string action = "";

                        if (param.data.ACTION == "REQUEST")
                        {
                            action = "REQUEST_MATERIAL";
                        }
                        else if (param.data.ACTION == "UPDATE")
                        {
                            action = "UPDATE_MATERIAL";
                        }
                        else
                        {
                            action = "REQUEST_UPDATE_MATERIAL";
                        }

                        string resultIgrid = inArtworkResp.inputArtworkNumberResult;
                        //Check Duplicate before interface iGrid
                        //if (param.data.RECORD_TYPE == "I")
                        //{
                        //    if (processPA.REQUEST_MATERIAL_STATUS != "Canceled")
                        //    {
                        //        if (!String.IsNullOrEmpty(processPA.REQUEST_MATERIAL_STATUS))
                        //        {
                        //            Results.status = "E";
                        //            Results.msg = MessageHelper.GetMessage("MSG_030", context);

                        //            return Results;
                        //        }
                        //        else
                        //        {
                        //            resultIgrid = inArtworkResp.inputArtworkNumberResult;
                        //        }
                        //    }
                        //    resultIgrid = inArtworkResp.inputArtworkNumberResult;
                        //}
                        //else
                        //{
                        //    resultIgrid = inArtworkResp.inputArtworkNumberResult;
                        //}

                        ART_SYS_LOG_INTERFACE log = new ART_SYS_LOG_INTERFACE();
                        log.MODULE = "MM65";
                        log.FUNCTION_NAME = action;
                        log.INPUT_FOR_TRACE = CNService.SubString(json.ToString(), 4000);
                        log.OUTPUT_FOR_TRACE = resultIgrid;
                        log.CALL_DATETIME = DateTime.Now;
                        log.CREATE_BY = -1;
                        log.UPDATE_BY = -1;

                        ART_SYS_LOG_INTERFACE_SERVICE.SaveOrUpdate(log, context);

                        if (!String.IsNullOrEmpty(resultIgrid) && processPATmp != null)
                        {
                            //if (!resultIgrid.Contains("error") || (resultIgrid.Length == 16 && resultIgrid.Substring(1, 2).ToString().Equals("MM")))
                            if (resultIgrid.Length == 16 && resultIgrid.Substring(1, 2).ToString().Equals("MM"))
                            {
                                processPATmp.REQUEST_MATERIAL_STATUS = "Waiting for approval";
                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPATmp, context);
                            }
                            else
                            {
                                Results.status = "E";
                                Results.msg = resultIgrid;
                                return Results;
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

            return Results;
        }
        public static MM65_RESULT RequestMaterial2(MM65_REQUEST param)
        {
            MM65_RESULT Results = new MM65_RESULT();

            string urlForm = ConfigurationManager.AppSettings["ArtworkURLTaskForm"];
            //MM65Client client = new MM65Client();
            ArtworkObject artworkObj = new ArtworkObject();
            List<InboundArtwork> inboundArtworks = new List<InboundArtwork>();
            List<InboundArtwork> inboundArtworks_PG = new List<InboundArtwork>();

            if (param.data.ARTWORK_SUB_ID > 0)
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        var stepPG = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == "SEND_PG").FirstOrDefault();
                        var stepPA = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == "SEND_PA").FirstOrDefault();

                        var process = (from p in context.ART_WF_ARTWORK_PROCESS
                                       where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                       select p).FirstOrDefault();

                        var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                         where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                         select p).FirstOrDefault();

                        var requestItem = (from p in context.ART_WF_ARTWORK_REQUEST_ITEM
                                           where p.ARTWORK_ITEM_ID == process.ARTWORK_ITEM_ID
                                           select p).FirstOrDefault();


                        if (param.data.RECORD_TYPE == "I")
                        {
                            if (processPA.REQUEST_MATERIAL_STATUS != "Canceled")
                            {
                                if (!String.IsNullOrEmpty(processPA.REQUEST_MATERIAL_STATUS))
                                {
                                    Results.status = "E";
                                    Results.msg = MessageHelper.GetMessage("MSG_030", context);

                                    return Results;
                                }
                            }

                            if (processPA.RD_REFERENCE_NO_ID != null)
                            {
                                var paProduct = (from p in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT
                                                 where p.ARTWORK_SUB_ID == processPA.ARTWORK_SUB_ID
                                                 select p.ARTWORK_SUB_PA_PRODUCT_ID).Count();

                                var paProductOther = (from p in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER
                                                      where p.ARTWORK_SUB_ID == processPA.ARTWORK_SUB_ID
                                                      select p.ARTWORK_SUB_PA_PRODUCT_OTHER_ID).Count();

                                if (paProduct <= 0 && paProductOther <= 0)
                                {
                                    Results.status = "E";
                                    Results.msg = "Please select required field in PA Data (Product Code) before request material."; ;

                                    return Results;
                                }

                            }
                        }
                        else if (param.data.RECORD_TYPE == "U")
                        {
                            var matPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                         where p.MATERIAL_NO == processPA.MATERIAL_NO
                                         && p.ARTWORK_SUB_ID != param.data.ARTWORK_SUB_ID
                                         && p.REQUEST_MATERIAL_STATUS.Contains("Waiting")
                                         select p).FirstOrDefault();

                            if (matPA != null)
                            {
                                Results.status = "E";
                                Results.msg = "Unable to update Material No. " + processPA.MATERIAL_NO + ", another workflow is updating " + processPA.MATERIAL_NO + " on iGrid";
                                //"Material No. " + processPA.MATERIAL_NO + " can not update material because waiting for approve in another Artwork.";
                                return Results;
                            }
                        }
                        var existData = (from e in context.IGRID_M_OUTBOUND_HEADER
                                         where e.ARTWORK_NO == requestItem.REQUEST_ITEM_NO
                                         select e).FirstOrDefault();
                        List<int> allSubID = new List<int>();

                        allSubID = CNService.FindArtworkSubId(param.data.ARTWORK_SUB_ID, context);

                        var processPG = (from p in context.ART_WF_ARTWORK_PROCESS_PG
                                         where allSubID.Contains(p.ARTWORK_SUB_ID)
                                         && p.DIE_LINE_MOCKUP_ID != null
                                         select p).OrderByDescending(o => o.ARTWORK_SUB_PA_ID).FirstOrDefault();

                        if (!String.IsNullOrEmpty(urlForm))
                        {
                            urlForm = urlForm + param.data.ARTWORK_SUB_ID.ToString();
                        }

                        string dateStr = "";
                        string noteOfPG = "";
                        string timeStr = "";
                        string PrintingStyleofPrimary = "";
                        string PrintingStyleofSecondary = "";

                        string Status = "";


                        dateStr = DateTime.Now.ToString("yyyyMMdd");
                        timeStr = DateTime.Now.ToString("HH:mm:ss");

                        //check require field
                        string msgError = "Please select required field in PA Data ({0}) before request material or update material";
                        string msg = "";

                        if (processPA.MATERIAL_GROUP_ID == null)
                        {
                            if (msg.Length > 0) msg += ", ";
                            msg += "Material group";
                        }
                        else
                        {
                            var matGroupCode = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(processPA.MATERIAL_GROUP_ID, context).VALUE;
                            if (matGroupCode != null)
                            {
                                switch (matGroupCode.ToUpper())
                                {
                                    case "K":
                                        if (processPA.STYLE_OF_PRINTING_ID == null)
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }
                                        else if (processPA.STYLE_OF_PRINTING_ID == -1 && String.IsNullOrEmpty(processPA.STYLE_OF_PRINTING_OTHER))
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }
                                        break;
                                    case "L":
                                        //nothing
                                        break;
                                    case "P":
                                        if (processPA.STYLE_OF_PRINTING_ID == null)
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }
                                        else if (processPA.STYLE_OF_PRINTING_ID == -1 && String.IsNullOrEmpty(processPA.STYLE_OF_PRINTING_OTHER))
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }
                                        break;
                                    case "J":
                                        if (processPA.STYLE_OF_PRINTING_ID == null)
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }
                                        else if (processPA.STYLE_OF_PRINTING_ID == -1 && String.IsNullOrEmpty(processPA.STYLE_OF_PRINTING_OTHER))
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }

                                        if (processPA.DIRECTION_OF_STICKER_ID == null)
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Direction of sticker";
                                        }
                                        else if (processPA.DIRECTION_OF_STICKER_ID == -1 && String.IsNullOrEmpty(processPA.DIRECTION_OF_STICKER_OTHER))
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Direction of sticker";
                                        }
                                        //if (processPA.CUSTOMER_DESIGN == null)
                                        //{
                                        //    if (msg.Length > 0) msg += ", ";
                                        //    msg += "Customer's design";
                                        //}
                                        //if (processPA.CUSTOMER_DESIGN_OTHER == null || processPA.CUSTOMER_DESIGN_OTHER == "")
                                        //{
                                        //    if (msg.Length > 0) msg += ", ";
                                        //    msg += "Customer's design detail";
                                        //}
                                        break;
                                    default:
                                        if (processPA.TWO_P_ID == null)
                                        {
                                            msg += "Packing style or Pack size";

                                        }
                                        else if (processPA.TWO_P_ID != null && processPA.TWO_P_ID == -1)
                                        {
                                            if (String.IsNullOrEmpty(processPA.PACKING_STYLE_OTHER))
                                            {
                                                msg += "Packing style";
                                            }

                                            if (processPA.PACK_SIZE_ID == null)
                                            {
                                                if (msg.Length > 0) msg += ", ";
                                                msg += "Pack size";
                                            }
                                            else if (processPA.TWO_P_ID == -1 && processPA.PACK_SIZE_ID == -1 && String.IsNullOrEmpty(processPA.PACK_SIZE_OTHER))
                                            {
                                                if (msg.Length > 0) msg += ", ";
                                                msg += "Pack size";
                                            }
                                        }

                                        if (processPA.STYLE_OF_PRINTING_ID == null)
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }
                                        else if (processPA.STYLE_OF_PRINTING_ID == -1 && String.IsNullOrEmpty(processPA.STYLE_OF_PRINTING_OTHER))
                                        {
                                            if (msg.Length > 0) msg += ", ";
                                            msg += "Style of printing";
                                        }
                                        break;
                                }

                            }
                        }
                        if (processPA.THREE_P_ID == null)
                        {
                            if (msg.Length > 0) msg += ", ";
                            msg += "Primary size";
                        }
                        else if (processPA.THREE_P_ID == -1)
                        {
                            if (String.IsNullOrEmpty(processPA.PRIMARY_SIZE_OTHER))
                            {
                                msg += "Packing style";
                            }

                            if (processPA.CONTAINER_TYPE_ID == null)
                            {
                                if (msg.Length > 0) msg += ", ";
                                msg += "Container type";
                            }
                            else if (processPA.THREE_P_ID == -1 && processPA.CONTAINER_TYPE_ID == -1 && String.IsNullOrEmpty(processPA.CONTAINER_TYPE_OTHER))
                            {
                                if (msg.Length > 0) msg += ", ";
                                msg += "Container type";
                            }

                            if (processPA.LID_TYPE_ID == null)
                            {
                                if (msg.Length > 0) msg += ", ";
                                msg += "Lid type";
                            }
                            else if (processPA.THREE_P_ID == -1 && processPA.LID_TYPE_ID == -1 && String.IsNullOrEmpty(processPA.LID_TYPE_OTHER))
                            {
                                if (msg.Length > 0) msg += ", ";
                                msg += "Lid type";
                            }

                        }


                        if (processPA.TOTAL_COLOUR_ID == null)
                        {
                            if (msg.Length > 0) msg += ", ";
                            msg += "Total colour";
                        }
                        else if (processPA.TOTAL_COLOUR_ID == -1 && String.IsNullOrEmpty(processPA.TOTAL_COLOUR_OTHER))
                        {
                            if (msg.Length > 0) msg += ", ";
                            msg += "Total colour";
                        }

                        if (msg.Length > 0)
                        {
                            Results.status = "E";
                            Results.msg = string.Format(msgError, msg);
                            return Results;
                        }

                        if (processPA.PRINTING_STYLE_OF_PRIMARY_ID != null)
                        {
                            var print_1 = context.ART_M_PRINTING_STYLE.Where(p => p.PRINTING_STYLE_ID == processPA.PRINTING_STYLE_OF_PRIMARY_ID).FirstOrDefault();
                            PrintingStyleofPrimary = print_1.PRINTING_STYLE_DESCRIPTION;
                        }
                        else if (!String.IsNullOrEmpty(processPA.PRINTING_STYLE_OF_PRIMARY_OTHER))
                        {
                            PrintingStyleofPrimary = processPA.PRINTING_STYLE_OF_PRIMARY_OTHER;
                        }

                        if (processPA.PRINTING_STYLE_OF_SECONDARY_ID != null)
                        {
                            var print_2 = context.ART_M_PRINTING_STYLE.Where(p => p.PRINTING_STYLE_ID == processPA.PRINTING_STYLE_OF_SECONDARY_ID).FirstOrDefault();
                            PrintingStyleofSecondary = print_2.PRINTING_STYLE_DESCRIPTION;
                        }
                        else if (!String.IsNullOrEmpty(processPA.PRINTING_STYLE_OF_SECONDARY_OTHER))
                        {
                            PrintingStyleofSecondary = processPA.PRINTING_STYLE_OF_SECONDARY_OTHER;
                        }

                        var plants = (from p in context.ART_WF_ARTWORK_PROCESS_PA_PLANT
                                      where p.ARTWORK_SUB_ID == processPA.ARTWORK_SUB_ID
                                         && p.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID
                                      select p).ToList();

                        if (plants != null && plants.Count > 0)
                        {
                            string plantCode = "";

                            foreach (ART_WF_ARTWORK_PROCESS_PA_PLANT item in plants)
                            {
                                if (String.IsNullOrEmpty(item.PLANT_OTHER))
                                {
                                    var plant = context.SAP_M_PLANT.Where(p => p.PLANT_ID == item.PLANT_ID).FirstOrDefault();
                                    plantCode += plant.PLANT + ";";
                                }
                                else
                                {
                                    plantCode += item.PLANT_OTHER + ";";
                                }
                            }

                            plantCode = plantCode.Substring(0, plantCode.Length - 1);

                            artworkObj.Plant = plantCode;
                        }
                        else
                        {
                            Results.status = "E";
                            Results.msg = string.Format(msgError, "Plant");
                            return Results;
                        }

                        bool is_total_colour = false;
                        bool is_style_of_printing = false;
                        if (processPG != null)
                        {
                            noteOfPG = processPG.COMMENT;

                            if (processPG.DIE_LINE_MOCKUP_ID != null)
                            {
                                var checklist_pg = (from p in context.ART_WF_MOCKUP_CHECK_LIST_PG
                                                    where p.MOCKUP_ID == processPG.DIE_LINE_MOCKUP_ID
                                                    select p).FirstOrDefault();

                                if (checklist_pg != null)
                                {
                                    is_total_colour = (checklist_pg.NUMBER_OF_COLOR_ID == processPA.TOTAL_COLOUR_ID);
                                }

                                var tempProcessTemplate_pg = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG() { MOCKUP_ID = processPG.DIE_LINE_MOCKUP_ID.GetValueOrDefault() }, context);
                                var processTemplate_pg = (from p in tempProcessTemplate_pg orderby p.UPDATE_DATE descending select p).FirstOrDefault();
                                if (processTemplate_pg != null)
                                {
                                    if (!string.IsNullOrEmpty(processTemplate_pg.STYLE_OF_PRINTING_OTHER))
                                    {
                                        is_style_of_printing = (processPA.STYLE_OF_PRINTING_OTHER == processTemplate_pg.STYLE_OF_PRINTING_OTHER);
                                    }
                                    else if (processTemplate_pg.STYLE_OF_PRINTING > 0)
                                    {
                                        is_style_of_printing = (processPA.STYLE_OF_PRINTING_ID == processTemplate_pg.STYLE_OF_PRINTING);
                                    }
                                }
                            }
                        }

                        string msgValidatePA_PG = "";

                        if (!is_total_colour)
                        {
                            msgValidatePA_PG = "Total colour (PA Data) and (PG Data) is mismatch <br>";
                        }

                        if (!is_style_of_printing)
                        {
                            msgValidatePA_PG += "Style of printing (PA Data) and (PG Data) is mismatch <br>";
                        }

                        if (!String.IsNullOrEmpty(msgValidatePA_PG))
                        {
                            Results.status = "E";
                            Results.msg = msgValidatePA_PG.Substring(0, msgValidatePA_PG.Length - 4);
                            return Results;
                        }
                        //if (existData != null)
                        //{
                        //    Status = "I";
                        //}
                        Status = "";

                        artworkObj.ArtworkNumber = requestItem.REQUEST_ITEM_NO;
                        artworkObj.Date = dateStr;
                        artworkObj.Time = timeStr;
                        artworkObj.RecordType = param.data.RECORD_TYPE;

                        if (param.data.RECORD_TYPE != "I" && !String.IsNullOrEmpty(processPA.MATERIAL_NO))
                        {
                            artworkObj.MaterialNumber = processPA.MATERIAL_NO;

                            var iGridHeader = (from g in context.IGRID_M_OUTBOUND_HEADER
                                               where g.ARTWORK_NO == requestItem.REQUEST_ITEM_NO
                                                && g.MATERIAL_NUMBER == processPA.MATERIAL_NO
                                               select g).FirstOrDefault();

                            if (iGridHeader != null)
                            {
                                artworkObj.MaterialDescription = iGridHeader.MATERIAL_DESCRIPTION;
                            }

                        }
                        else
                        {
                            artworkObj.MaterialNumber = "";
                            artworkObj.MaterialDescription = "";
                        }

                        artworkObj.MaterialCreatedDate = "";
                        artworkObj.ArtworkURL = urlForm;
                        artworkObj.Status = Status;
                        // artworkObj. = Status;

                        artworkObj.PAUserName = CNService.GetUserLogin(processPA.PA_USER_ID, context); //"MO600965";
                        artworkObj.PGUserName = CNService.GetUserLogin(processPA.PG_USER_ID, context); // "MO530515"; 

                        artworkObj.ReferenceMaterial = "";
                        if (param.data.RECORD_TYPE == "I")
                        {
                            artworkObj.ReferenceMaterial = param.data.REFERENCE_MATERIAL;
                        }
                        // artworkObj.Plant = Plant;
                        artworkObj.PrintingStyleofPrimary = PrintingStyleofPrimary;
                        artworkObj.PrintingStyleofSecondary = PrintingStyleofSecondary;
                        artworkObj.CustomersDesign = GetYesNoValue(processPA.CUSTOMER_DESIGN);
                        artworkObj.CustomersDesignDetail = processPA.CUSTOMER_DESIGN_OTHER;
                        artworkObj.CustomersSpec = GetYesNoValue(processPA.CUSTOMER_SPEC);
                        artworkObj.CustomersSpecDetail = processPA.CUSTOMER_SPEC_OTHER;
                        artworkObj.CustomersSize = GetYesNoValue(processPA.CUSTOMER_SIZE);
                        artworkObj.CustomersSizeDetail = processPA.CUSTOMER_SIZE_OTHER;
                        artworkObj.CustomerNominatesVendor = GetYesNoValue(processPA.CUSTOMER_NOMINATES_VENDOR);
                        artworkObj.CustomerNominatesVendorDetail = processPA.CUSTOMER_NOMINATES_VENDOR_OTHER;
                        artworkObj.CustomerNominatesColorPantone = GetYesNoValue(processPA.CUSTOMER_NOMINATES_COLOR);
                        artworkObj.CustomerNominatesColorPantoneDetail = processPA.CUSTOMER_NOMINATES_COLOR_OTHER;
                        artworkObj.CustomersBarcodeScanable = GetYesNoValue(processPA.CUSTOMER_BARCODE_SCANABLE);
                        artworkObj.CustomersBarcodeScanableDetail = processPA.CUSTOMER_BARCODE_SCANABLE_OTHER;
                        artworkObj.CustomersBarcodeSpec = GetYesNoValue(processPA.CUSTOMER_BARCODE_SCANABLE);
                        artworkObj.CustomersBarcodeSpecDetail = processPA.CUSTOMER_BARCODE_SPEC_OTHER;
                        artworkObj.FirstInfoGroup = processPA.FIRST_INFOGROUP_OTHER;
                        artworkObj.SONumber = "";
                        artworkObj.SOitem = "";
                        artworkObj.SOPlant = "";
                        artworkObj.PICMKT = "";
                        artworkObj.Destination = "";
                        artworkObj.RemarkNoteofPA = processPA.NOTE_OF_PA;
                        artworkObj.FinalInfoGroup = processPA.FIRST_INFOGROUP_OTHER;
                        artworkObj.RemarkNoteofPG = CNService.RemoveHTMLTag(noteOfPG);
                        artworkObj.CompleteInfoGroup = processPA.COMPLETE_INFOGROUP;
                        artworkObj.ProductionExpirydatesystem = processPA.PRODUCTION_EXPIRY_DATE_SYSTEM;
                        artworkObj.Seriousnessofcolorprinting = GetYesNoValue(processPA.SERIOUSNESS_OF_COLOR_PRINTING);
                        artworkObj.CustIngreNutritionAnalysis = GetYesNoValue(processPA.NUTRITION_ANALYSIS);
                        artworkObj.ShadeLimit = GetApproveValue(processPA.SHADE_LIMIT);
                        artworkObj.PackageQuantity = processPA.PACKAGE_QUANTITY;
                        artworkObj.WastePercent = processPA.WASTE_PERCENT;

                        string _validate = ValidateMM65_Header(artworkObj);
                        if (!String.IsNullOrEmpty(_validate))
                        {
                            Results.status = "E";
                            Results.msg = _validate + " " + MessageHelper.GetMessage("MSG_009", context);
                            return Results;
                        }

                        inboundArtworks = GetCharacteristicsValue_PA(artworkObj, processPA, context);

                        inboundArtworks_PG = GetCharacteristicsValue_PG(artworkObj, processPA, processPG, context);

                        if (inboundArtworks_PG != null && inboundArtworks_PG.Count > 0)
                        {
                            inboundArtworks.AddRange(inboundArtworks_PG);
                        }

                        inputArtworkNumber inArtwork = new inputArtworkNumber();

                        inArtwork._artworkObject = artworkObj;
                        inArtwork._itemsArtwork = inboundArtworks.ToArray();

                        inputArtworkNumberResponse inArtworkResp = new inputArtworkNumberResponse();

                        var json = CNService.Serialize(inArtwork);

                        _validate = "";
                        _validate = ValidateMM65_Item(json.ToString(), param);
                        if (!String.IsNullOrEmpty(_validate))
                        {
                            Results.status = "E";
                            Results.msg = _validate + " " + MessageHelper.GetMessage("MSG_009", context);
                            return Results;
                        }
                        //changed by voravut on 2020-05-08 move if return +++++++++++++++++++
                        if (param.data.RECORD_TYPE == "I")
                        {
                            if (processPA.REQUEST_MATERIAL_STATUS != "Canceled")
                            {
                                if (!String.IsNullOrEmpty(processPA.REQUEST_MATERIAL_STATUS))
                                {
                                    Results.status = "E";
                                    Results.msg = MessageHelper.GetMessage("MSG_030", context);

                                    return Results;
                                }
                            }
                        }
                        //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                        inArtworkResp = CNService.inputArtworkNumber(inArtwork);

                        ART_WF_ARTWORK_PROCESS_PA processPATmp = new ART_WF_ARTWORK_PROCESS_PA();
                        processPATmp.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                        processPATmp = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(processPATmp, context).FirstOrDefault();

                        string action = "";

                        if (param.data.ACTION == "REQUEST")
                        {
                            action = "REQUEST_MATERIAL";
                        }
                        else if (param.data.ACTION == "UPDATE")
                        {
                            action = "UPDATE_MATERIAL";
                        }
                        else
                        {
                            action = "REQUEST_UPDATE_MATERIAL";
                        }

                        string resultIgrid = inArtworkResp.inputArtworkNumberResult;
                        //Check Duplicate before interface iGrid
                        //if (param.data.RECORD_TYPE == "I")
                        //{
                        //    if (processPA.REQUEST_MATERIAL_STATUS != "Canceled")
                        //    {
                        //        if (!String.IsNullOrEmpty(processPA.REQUEST_MATERIAL_STATUS))
                        //        {
                        //            Results.status = "E";
                        //            Results.msg = MessageHelper.GetMessage("MSG_030", context);

                        //            return Results;
                        //        }
                        //        else
                        //        {
                        //            resultIgrid = inArtworkResp.inputArtworkNumberResult;
                        //        }
                        //    }
                        //    resultIgrid = inArtworkResp.inputArtworkNumberResult;
                        //}
                        //else
                        //{
                        //    resultIgrid = inArtworkResp.inputArtworkNumberResult;
                        //}

                        ART_SYS_LOG_INTERFACE log = new ART_SYS_LOG_INTERFACE();
                        log.MODULE = "MM65";
                        log.FUNCTION_NAME = action;
                        log.INPUT_FOR_TRACE = CNService.SubString(json.ToString(), 4000);
                        log.OUTPUT_FOR_TRACE = resultIgrid;
                        log.CALL_DATETIME = DateTime.Now;
                        log.CREATE_BY = -1;
                        log.UPDATE_BY = -1;

                        ART_SYS_LOG_INTERFACE_SERVICE.SaveOrUpdate(log, context);

                        if (!String.IsNullOrEmpty(resultIgrid) && processPATmp != null)
                        {
                            //if (!resultIgrid.Contains("error") || (resultIgrid.Length == 16 && resultIgrid.Substring(1, 2).ToString().Equals("MM")))
                            if (resultIgrid.Length == 16 && resultIgrid.Substring(1, 2).ToString().Equals("MM")){
                                processPATmp.REQUEST_MATERIAL_STATUS = "Waiting for approval";
                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPATmp, context);
                            }
                            else
                            {
                                Results.status = "E";
                                Results.msg = resultIgrid;
                                return Results;
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

            return Results;
        }

        private static List<InboundArtwork> GetCharacteristicsValue_PA(ArtworkObject artworkObj, ART_WF_ARTWORK_PROCESS_PA dataPA, ARTWORKEntities context)
        {
            InboundArtwork modelItem = new InboundArtwork();
            List<InboundArtwork> listModelItem = new List<InboundArtwork>();
            List<string> _listColumn = new List<string>();
            List<string> _listColumnOther = new List<string>();
            List<string> _listColumnMulti = new List<string>();

            string characPackingStyle = "ZPKG_SEC_PACKING_STYLE";
            string characPackSize = "ZPKG_SEC_PACKING";

            _listColumn.Add("MATERIAL_GROUP_ID");
            _listColumn.Add("TYPE_OF_ID");
            _listColumn.Add("TYPE_OF_2_ID");
            _listColumn.Add("PLANT_REGISTERED_ID");
            _listColumn.Add("PMS_COLOUR_ID");
            _listColumn.Add("COMPANY_ADDRESS_ID");
            _listColumn.Add("PROCESS_COLOUR_ID");
            _listColumn.Add("CATCHING_PERIOD_ID");
            _listColumn.Add("TOTAL_COLOUR_ID");
            _listColumn.Add("CATCHING_METHOD_ID");
            _listColumn.Add("SCIENTIFIC_NAME_ID");
            _listColumn.Add("STYLE_OF_PRINTING_ID");
            _listColumn.Add("SPECIE_ID");
            _listColumn.Add("DIRECTION_OF_STICKER_ID");
            _listColumn.Add("ZPKG_SEC_BRAND");
            _listColumn.Add("ZPKG_SEC_PRIMARY_SIZE");
            _listColumn.Add("ZPKG_SEC_CONTAINER_TYPE");
            _listColumn.Add("ZPKG_SEC_LID_TYPE");
            _listColumn.Add("ZPKG_SEC_CHANGE_POINT");
            _listColumn.Add("ZPKG_SEC_DIRECTION");
            _listColumn.Add("ZPKG_SEC_PRIMARY_TYPE");
            _listColumn.Add("PACKING_STYLE_ID");
            _listColumn.Add("PACK_SIZE_ID");
            _listColumn.Add("ZPKG_SEC_PRODUCT_CODE");

            _listColumnOther.Add("TYPE_OF_OTHER");
            _listColumnOther.Add("TYPE_OF_2_OTHER");
            _listColumnOther.Add("PLANT_REGISTERED_OTHER");
            _listColumnOther.Add("PMS_COLOUR_OTHER");
            _listColumnOther.Add("COMPANY_ADDRESS_OTHER");
            _listColumnOther.Add("PROCESS_COLOUR_OTHER");
            _listColumnOther.Add("CATCHING_PERIOD_OTHER");
            _listColumnOther.Add("TOTAL_COLOUR_OTHER");
            _listColumnOther.Add("CATCHING_METHOD_OTHER");
            _listColumnOther.Add("STYLE_OF_PRINTING_OTHER");
            _listColumnOther.Add("SCIENTIFIC_NAME_OTHER");
            _listColumnOther.Add("DIRECTION_OF_STICKER_OTHER");
            _listColumnOther.Add("SPECIE_OTHER");
            _listColumnOther.Add("PRIMARY_SIZE_OTHER");
            _listColumnOther.Add("CONTAINER_TYPE_OTHER");
            _listColumnOther.Add("LID_TYPE_OTHER");
            _listColumnOther.Add("PACKING_STYLE_OTHER");
            _listColumnOther.Add("PACK_SIZE_OTHER");
            _listColumnOther.Add("DIRECTION_OF_STICKER_OTHER");


            _listColumnMulti.Add("ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE");
            _listColumnMulti.Add("ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA");
            _listColumnMulti.Add("ART_WF_ARTWORK_PROCESS_PA_SYMBOL");
            _listColumnMulti.Add("ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD");  //ticke#425737 added by aof 

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
            //Dictionary<string, string> dicPackingStyle = new Dictionary<string, string>();
            //Dictionary<string, string> dicPackSize = new Dictionary<string, string>();

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

            // dicPackingStyle.Add("C", "ZPKG_SEC_CARDBOARD");


            //  dicPackSize.Add("C", "ZPKG_SEC_CARDBOARD");


            if (_listColumnMulti.Count > 0)
            {
                foreach (string iColumn in _listColumnMulti)
                {

                    if (iColumn == "ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE")
                    {
                        var fao = context.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE.Where(f => f.ARTWORK_SUB_PA_ID == dataPA.ARTWORK_SUB_PA_ID).ToList();

                        if (fao != null && fao.Count > 0)
                        {
                            String charName = "ZPKG_SEC_FAO";
                            SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();
                            foreach (ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE iFAO in fao)
                            {
                                modelItem = new InboundArtwork();

                                modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                modelItem.Date = artworkObj.Date;
                                modelItem.Time = artworkObj.Time;

                                if (iFAO.FAO_ZONE_ID > 0)
                                {
                                    characteristic = new SAP_M_CHARACTERISTIC();
                                    characteristic = CNService.GetCharacteristicData(iFAO.FAO_ZONE_ID, context);

                                    if (characteristic != null)
                                    {
                                        modelItem.Characteristic = charName;
                                        modelItem.Value = characteristic.VALUE;
                                        modelItem.Description = characteristic.DESCRIPTION;
                                    }
                                }
                                else if (iFAO.FAO_ZONE_ID == -1 && !String.IsNullOrEmpty(iFAO.FAO_ZONE_OTHER))
                                {
                                    modelItem.Characteristic = charName;
                                    modelItem.Value = iFAO.FAO_ZONE_OTHER;
                                    modelItem.Description = "";
                                }
                                listModelItem.Add(modelItem);

                            }
                        }
                    }

                    if (iColumn == "ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA")
                    {
                        var catchingAreas = context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA.Where(f => f.ARTWORK_SUB_PA_ID == dataPA.ARTWORK_SUB_PA_ID).ToList();

                        if (catchingAreas != null && catchingAreas.Count > 0)
                        {
                            String charName = "ZPKG_SEC_CATCHING_AREA";
                            SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();
                            foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA iCatching in catchingAreas)
                            {
                                modelItem = new InboundArtwork();

                                modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                modelItem.Date = artworkObj.Date;
                                modelItem.Time = artworkObj.Time;

                                if (iCatching.CATCHING_AREA_ID > 0)
                                {
                                    characteristic = new SAP_M_CHARACTERISTIC();
                                    characteristic = CNService.GetCharacteristicData(iCatching.CATCHING_AREA_ID, context);

                                    if (characteristic != null)
                                    {
                                        modelItem.Characteristic = charName;
                                        modelItem.Value = characteristic.VALUE;
                                        modelItem.Description = characteristic.DESCRIPTION;
                                    }
                                }
                                else if (iCatching.CATCHING_AREA_ID == -1 && !String.IsNullOrEmpty(iCatching.CATCHING_AREA_OTHER))
                                {
                                    modelItem.Characteristic = charName;
                                    modelItem.Value = iCatching.CATCHING_AREA_OTHER;
                                    modelItem.Description = "";
                                }
                                listModelItem.Add(modelItem);

                            }
                        }
                    }

                    //ticke#425737 added by aof 
                    if (iColumn == "ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD")
                    {
                        var catchingMethods = context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD.Where(f => f.ARTWORK_SUB_PA_ID == dataPA.ARTWORK_SUB_PA_ID).ToList();

                        if (catchingMethods != null && catchingMethods.Count > 0)
                        {
                            String charName = "ZPKG_SEC_CATCHING_METHOD";
                            SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();
                            foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD iMethod in catchingMethods)
                            {
                                modelItem = new InboundArtwork();

                                modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                modelItem.Date = artworkObj.Date;
                                modelItem.Time = artworkObj.Time;

                                if (iMethod.CATCHING_METHOD_ID > 0)
                                {
                                    characteristic = new SAP_M_CHARACTERISTIC();
                                    characteristic = CNService.GetCharacteristicData(iMethod.CATCHING_METHOD_ID, context);

                                    if (characteristic != null)
                                    {
                                        modelItem.Characteristic = charName;
                                        modelItem.Value = characteristic.VALUE;
                                        modelItem.Description = characteristic.DESCRIPTION;
                                    }
                                }
                                else if (iMethod.CATCHING_METHOD_ID == -1 && !String.IsNullOrEmpty(iMethod.CATCHING_METHOD_OTHER))
                                {
                                    modelItem.Characteristic = charName;
                                    modelItem.Value = iMethod.CATCHING_METHOD_OTHER;
                                    modelItem.Description = "";
                                }
                                listModelItem.Add(modelItem);

                            }
                        }
                    }
                    //ticke#425737 added by aof 

                    if (iColumn == "ART_WF_ARTWORK_PROCESS_PA_SYMBOL")
                    {
                        var catchingAreas = context.ART_WF_ARTWORK_PROCESS_PA_SYMBOL.Where(f => f.ARTWORK_SUB_PA_ID == dataPA.ARTWORK_SUB_PA_ID).ToList();

                        if (catchingAreas != null && catchingAreas.Count > 0)
                        {
                            String charName = "ZPKG_SEC_SYMBOL";
                            SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();
                            foreach (ART_WF_ARTWORK_PROCESS_PA_SYMBOL iSymbol in catchingAreas)
                            {
                                modelItem = new InboundArtwork();

                                modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                modelItem.Date = artworkObj.Date;
                                modelItem.Time = artworkObj.Time;

                                if (iSymbol.SYMBOL_ID > 0)
                                {
                                    characteristic = new SAP_M_CHARACTERISTIC();
                                    characteristic = CNService.GetCharacteristicData(iSymbol.SYMBOL_ID, context);

                                    if (characteristic != null)
                                    {
                                        modelItem.Characteristic = charName;
                                        modelItem.Value = characteristic.VALUE;
                                        modelItem.Description = characteristic.DESCRIPTION;
                                    }
                                }
                                else if (iSymbol.SYMBOL_ID == -1 && !String.IsNullOrEmpty(iSymbol.SYMBOL_OTHER))
                                {
                                    modelItem.Characteristic = charName;
                                    modelItem.Value = iSymbol.SYMBOL_OTHER;
                                    modelItem.Description = "";
                                }
                                listModelItem.Add(modelItem);

                            }
                        }
                    }

                }

            }

            if (_listColumn.Count > 0)
            {
                string materialGroupID = "";
                materialGroupID = dataPA.MATERIAL_GROUP_ID.ToString();
                var matGroup = context.SAP_M_CHARACTERISTIC.Where(w => w.CHARACTERISTIC_ID == dataPA.MATERIAL_GROUP_ID).Select(s => s.VALUE).FirstOrDefault();
                materialGroupID = matGroup; //data.MATERIAL_GROUP_ID.ToString();

                foreach (string iColumn in _listColumn)
                {
                    try
                    {
                        var process = context.ART_WF_ARTWORK_PROCESS.Where(p => p.ARTWORK_SUB_ID == dataPA.ARTWORK_SUB_ID).FirstOrDefault();
                        var request = context.ART_WF_ARTWORK_REQUEST.Where(r => r.ARTWORK_REQUEST_ID == process.ARTWORK_REQUEST_ID).FirstOrDefault();


                        if (iColumn == "ZPKG_SEC_BRAND")
                        {


                            if (request != null && request.BRAND_ID != null)
                            {
                                var reqBrand = context.SAP_M_BRAND.Where(b => b.BRAND_ID == request.BRAND_ID).FirstOrDefault();
                                bool isBrandBySO = false;

                                var soAssign = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                                where s.ARTWORK_SUB_ID == dataPA.ARTWORK_SUB_ID
                                                select s).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                                if (soAssign != null)
                                {
                                    var soHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                                    where h.SALES_ORDER_NO == soAssign.SALES_ORDER_NO
                                                    select h).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();


                                    if (soHeader != null)
                                    {
                                        decimal itemNO = 0;
                                        itemNO = Convert.ToDecimal(soAssign.SALES_ORDER_ITEM);
                                        var soItem = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
                                                      where i.PO_COMPLETE_SO_HEADER_ID == soHeader.PO_COMPLETE_SO_HEADER_ID
                                                        && i.ITEM == itemNO
                                                        && !String.IsNullOrEmpty(i.BRAND_ID)
                                                      select i).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                                        if (soItem != null)
                                        {
                                            modelItem = new InboundArtwork();

                                            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                            modelItem.Date = artworkObj.Date;
                                            modelItem.Time = artworkObj.Time;
                                            modelItem.Characteristic = iColumn;
                                            modelItem.Value = soItem.BRAND_ID;
                                            modelItem.Description = soItem.BRAND_DESCRIPTION;

                                            listModelItem.Add(modelItem);

                                            isBrandBySO = true;
                                        }
                                    }
                                }

                                if (reqBrand != null && isBrandBySO == false)
                                {

                                    if (request.BRAND_ID == -1)
                                    {
                                        modelItem = new InboundArtwork();

                                        modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                        modelItem.Date = artworkObj.Date;
                                        modelItem.Time = artworkObj.Time;
                                        modelItem.Characteristic = iColumn;
                                        modelItem.Value = request.BRAND_OTHER;
                                        modelItem.Description = request.BRAND_OTHER;

                                        listModelItem.Add(modelItem);
                                    }

                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = iColumn;
                                    modelItem.Value = reqBrand.MATERIAL_GROUP;
                                    modelItem.Description = reqBrand.DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }



                        }
                        else if (iColumn == "ZPKG_SEC_PRODUCT_CODE")
                        {
                            if (dataPA.PRODUCT_CODE_ID != null)
                            {
                                var xProduct = (from p in context.XECM_M_PRODUCT
                                                where p.XECM_PRODUCT_ID == dataPA.PRODUCT_CODE_ID
                                                select p).FirstOrDefault();

                                if (xProduct != null)
                                {
                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = iColumn;
                                    modelItem.Value = xProduct.PRODUCT_CODE;
                                    modelItem.Description = xProduct.PRODUCT_DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }

                            var additionalProduct = (from d in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT
                                                     where d.ARTWORK_SUB_PA_ID == dataPA.ARTWORK_SUB_PA_ID
                                                     select d).ToList();

                            if (additionalProduct != null && additionalProduct.Count > 0)
                            {
                                foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT iProduct in additionalProduct)
                                {
                                    var xProduct = (from p in context.XECM_M_PRODUCT
                                                    where p.XECM_PRODUCT_ID == iProduct.PRODUCT_CODE_ID
                                                    select p).FirstOrDefault();

                                    if (xProduct != null)
                                    {
                                        modelItem = new InboundArtwork();

                                        modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                        modelItem.Date = artworkObj.Date;
                                        modelItem.Time = artworkObj.Time;
                                        modelItem.Characteristic = iColumn;
                                        modelItem.Value = xProduct.PRODUCT_CODE;
                                        modelItem.Description = xProduct.PRODUCT_DESCRIPTION;

                                        listModelItem.Add(modelItem);
                                    }
                                }
                            }

                            var additionalProductOther = (from d in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER
                                                          where d.ARTWORK_SUB_PA_ID == dataPA.ARTWORK_SUB_PA_ID
                                                          select d).ToList();

                            if (additionalProductOther != null && additionalProductOther.Count > 0)
                            {
                                foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER iProductOther in additionalProductOther)
                                {
                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = iColumn;
                                    modelItem.Value = iProductOther.PRODUCT_CODE;
                                    modelItem.Description = iProductOther.PRODUCT_CODE;

                                    listModelItem.Add(modelItem);
                                }
                            }

                            //var refProductIDs = additionalProduct.Select(s => s.PRODUCT_CODE_ID).ToList();
                            //if (refProductIDs != null)
                            //{
                            //    var refNo = (from f in context.ART_WF_ARTWORK_REQUEST_REFERENCE
                            //                 where refProductIDs.Contains(f.ARTWORK_REFERENCE_ID)
                            //                 select f.REFERENCE_NO).ToList();

                            //    if (refNo != null)
                            //    {
                            //        foreach (var iRefNo in refNo)
                            //        {
                            //            modelItem = new InboundArtwork();

                            //            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                            //            modelItem.Date = artworkObj.Date;
                            //            modelItem.Time = artworkObj.Time;
                            //            modelItem.Characteristic = iColumn;
                            //            modelItem.Value = iRefNo;
                            //            modelItem.Description = iRefNo;

                            //            listModelItem.Add(modelItem);
                            //        }

                            //    }
                            //}

                        }
                        else if (iColumn == "ZPKG_SEC_DIRECTION")
                        {
                            if (dataPA.DIRECTION_OF_STICKER_ID != null)
                            {
                                if (dataPA.DIRECTION_OF_STICKER_ID > -1)
                                {
                                    var dirSticker = context.SAP_M_CHARACTERISTIC
                                                    .Where(w => w.CHARACTERISTIC_ID == dataPA.DIRECTION_OF_STICKER_ID)
                                                    .FirstOrDefault();

                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = iColumn;
                                    modelItem.Value = dirSticker.VALUE;
                                    modelItem.Description = dirSticker.DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                        }
                        else if (iColumn == "ZPKG_SEC_PRIMARY_TYPE")
                        {

                            var requestID = (from p in context.ART_WF_ARTWORK_PROCESS
                                             where p.ARTWORK_SUB_ID == dataPA.ARTWORK_SUB_ID
                                             select p.ARTWORK_REQUEST_ID).FirstOrDefault();

                            var req = (from i in context.ART_WF_ARTWORK_REQUEST
                                       where i.ARTWORK_REQUEST_ID == requestID
                                       select i).FirstOrDefault();

                            if (req.PRIMARY_TYPE_ID != null && req.PRIMARY_TYPE_ID > -1)
                            {
                                var characM = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == req.PRIMARY_TYPE_ID).FirstOrDefault();
                                if (characM != null)
                                {

                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = iColumn;
                                    modelItem.Value = characM.VALUE;
                                    modelItem.Description = characM.DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else
                            {
                                modelItem = new InboundArtwork();

                                modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                modelItem.Date = artworkObj.Date;
                                modelItem.Time = artworkObj.Time;
                                modelItem.Characteristic = iColumn;
                                modelItem.Value = req.PRIMARY_TYPE_OTHER;
                                modelItem.Description = "";

                                listModelItem.Add(modelItem);
                            }

                        }
                        else if (iColumn == "ZPKG_SEC_PRIMARY_SIZE")
                        {
                            if (dataPA.PRIMARY_SIZE_ID != null)
                            {
                                var characM = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == dataPA.THREE_P_ID).FirstOrDefault();
                                if (characM != null)
                                {
                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = iColumn;
                                    modelItem.Value = characM.VALUE;
                                    modelItem.Description = characM.DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (dataPA.THREE_P_ID != null)
                            {
                                var charac = context.SAP_M_3P.Where(c => c.THREE_P_ID == dataPA.THREE_P_ID).FirstOrDefault();
                                if (charac != null)
                                {
                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = iColumn;
                                    modelItem.Value = charac.PRIMARY_SIZE_VALUE;
                                    modelItem.Description = charac.PRIMARY_SIZE_DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                        }
                        else if (iColumn == "ZPKG_SEC_CONTAINER_TYPE")
                        {
                            if (dataPA.CONTAINER_TYPE_ID != null)
                            {
                                var characM = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == dataPA.CONTAINER_TYPE_ID).FirstOrDefault();
                                if (characM != null)
                                {
                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = iColumn;
                                    modelItem.Value = characM.VALUE;
                                    modelItem.Description = characM.DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (dataPA.THREE_P_ID != null)
                            {
                                var charac = context.SAP_M_3P.Where(c => c.THREE_P_ID == dataPA.THREE_P_ID).FirstOrDefault();
                                if (charac != null)
                                {
                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = iColumn;
                                    modelItem.Value = charac.CONTAINER_TYPE_VALUE;
                                    modelItem.Description = charac.CONTAINER_TYPE_DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                        }
                        else if (iColumn == "ZPKG_SEC_LID_TYPE")
                        {
                            if (dataPA.LID_TYPE_ID != null)
                            {
                                var characM = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == dataPA.LID_TYPE_ID).FirstOrDefault();
                                if (characM != null)
                                {
                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = iColumn;
                                    modelItem.Value = characM.VALUE;
                                    modelItem.Description = characM.DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (dataPA.THREE_P_ID != null)
                            {
                                var charac = context.SAP_M_3P.Where(c => c.THREE_P_ID == dataPA.THREE_P_ID).FirstOrDefault();
                                if (charac != null)
                                {
                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = iColumn;
                                    modelItem.Value = charac.LID_TYPE_VALUE;
                                    modelItem.Description = charac.LID_TYPE_DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                        }
                        else if (iColumn == "PACKING_STYLE_ID")
                        {
                            if (dataPA.PACKING_STYLE_ID != null)
                            {
                                var characM = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == dataPA.PACKING_STYLE_ID).FirstOrDefault();
                                if (characM != null)
                                {
                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = characPackingStyle;
                                    modelItem.Value = characM.VALUE;
                                    modelItem.Description = characM.DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (dataPA.TWO_P_ID != null)
                            {
                                var charac = context.SAP_M_2P.Where(c => c.TWO_P_ID == dataPA.TWO_P_ID).FirstOrDefault();
                                if (charac != null)
                                {
                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = characPackingStyle;
                                    modelItem.Value = charac.PACKING_SYLE_VALUE;
                                    modelItem.Description = charac.PACKING_SYLE_DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                        }
                        else if (iColumn == "PACK_SIZE_ID")
                        {
                            if (dataPA.PACK_SIZE_ID != null)
                            {
                                var characM = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == dataPA.PACK_SIZE_ID).FirstOrDefault();
                                if (characM != null)
                                {
                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = characPackSize;
                                    modelItem.Value = characM.VALUE;
                                    modelItem.Description = characM.DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (dataPA.TWO_P_ID != null)
                            {
                                var charac = context.SAP_M_2P.Where(c => c.TWO_P_ID == dataPA.TWO_P_ID).FirstOrDefault();
                                if (charac != null)
                                {
                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = characPackSize;
                                    modelItem.Value = charac.PACK_SIZE_VALUE;
                                    modelItem.Description = charac.PACK_SIZE_DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                        }
                        else if (iColumn == "ZPKG_SEC_CHANGE_POINT")
                        {
                            if (!String.IsNullOrEmpty(dataPA.CHANGE_POINT))
                            {
                                string _changePointValue = "";
                                string _changePointDescription = "";
                                if (dataPA.CHANGE_POINT == "1")
                                {
                                    _changePointValue = "C";
                                    _changePointDescription = " Change Point";
                                }
                                else if (dataPA.CHANGE_POINT == "0")
                                {
                                    _changePointValue = "N";
                                    _changePointDescription = "No Change Point";
                                }

                                modelItem = new InboundArtwork();

                                modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                modelItem.Date = artworkObj.Date;
                                modelItem.Time = artworkObj.Time;
                                modelItem.Characteristic = iColumn;
                                modelItem.Value = _changePointValue;
                                modelItem.Description = _changePointDescription;

                                listModelItem.Add(modelItem);
                            }
                        }
                        else
                        {
                            var id = dataPA.GetType().GetProperty(iColumn).GetValue(dataPA);

                            if (id != null)
                            {
                                int charID;
                                bool res = int.TryParse(id.ToString(), out charID);
                                if (res == true && charID > 0)
                                {
                                    var characteristic = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == charID).FirstOrDefault();
                                    if (characteristic != null)
                                    {
                                        modelItem = new InboundArtwork();

                                        modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                        modelItem.Date = artworkObj.Date;
                                        modelItem.Time = artworkObj.Time;
                                        modelItem.Characteristic = characteristic.NAME;
                                        modelItem.Value = characteristic.VALUE;
                                        modelItem.Description = characteristic.DESCRIPTION;

                                        listModelItem.Add(modelItem);
                                    }
                                }
                            }
                        }

                    }
                    catch //(Exception ex)
                    {

                    }


                }
            }

            if (_listColumnOther.Count > 0)
            {
                string materialGroupID = "";
                string characteristicName = "";

                var matGroup = context.SAP_M_CHARACTERISTIC.Where(w => w.CHARACTERISTIC_ID == dataPA.MATERIAL_GROUP_ID).Select(s => s.VALUE).FirstOrDefault();
                materialGroupID = matGroup; //data.MATERIAL_GROUP_ID.ToString();

                foreach (string iColumn in _listColumnOther)
                {
                    try
                    {
                        var otherValue = string.Format("{0}", dataPA.GetType().GetProperty(iColumn).GetValue(dataPA));


                        if (iColumn == "TYPE_OF_OTHER")
                        {
                            if (!String.IsNullOrEmpty(materialGroupID))
                            {
                                characteristicName = dicTypeOf[materialGroupID];
                            }
                        }

                        else if (iColumn == "PRIMARY_SIZE_OTHER")
                        {
                            if (!String.IsNullOrEmpty(materialGroupID))
                            {
                                characteristicName = "ZPKG_SEC_PRIMARY_SIZE";
                            }
                        }
                        else if (iColumn == "CONTAINER_TYPE_OTHER")
                        {
                            if (!String.IsNullOrEmpty(materialGroupID))
                            {
                                characteristicName = "ZPKG_SEC_CONTAINER_TYPE";
                            }
                        }
                        else if (iColumn == "LID_TYPE_OTHER")
                        {
                            if (!String.IsNullOrEmpty(materialGroupID))
                            {
                                characteristicName = "ZPKG_SEC_LID_TYPE";
                            }
                        }
                        else if (iColumn == "PACKING_STYLE_OTHER")
                        {
                            if (!String.IsNullOrEmpty(materialGroupID))
                            {
                                characteristicName = characPackingStyle;
                            }
                        }
                        else if (iColumn == "PACK_SIZE_OTHER")
                        {
                            if (!String.IsNullOrEmpty(materialGroupID))
                            {
                                characteristicName = characPackSize;
                            }
                        }
                        else if (iColumn == "TYPE_OF_2_OTHER")
                        {
                            if (!String.IsNullOrEmpty(materialGroupID))
                            {
                                characteristicName = dicTypeOf2[materialGroupID];
                            }
                        }
                        else if (iColumn == "PLANT_REGISTERED_OTHER")
                        {
                            characteristicName = "ZPKG_SEC_PLANT_REGISTER";
                        }
                        else if (iColumn == "PMS_COLOUR_OTHER")
                        {
                            if (!String.IsNullOrEmpty(materialGroupID))
                            {
                                characteristicName = dicPMSColour[materialGroupID];
                            }
                        }
                        else if (iColumn == "COMPANY_ADDRESS_OTHER")
                        {
                            characteristicName = "ZPKG_SEC_COMPANY_ADR";
                        }
                        else if (iColumn == "PROCESS_COLOUR_OTHER")
                        {
                            if (!String.IsNullOrEmpty(materialGroupID))
                            {
                                characteristicName = dicProcessColour[materialGroupID];
                            }
                        }
                        else if (iColumn == "CATCHING_PERIOD_OTHER")
                        {
                            characteristicName = "ZPKG_SEC_CATCHING_METHOD";
                        }
                        else if (iColumn == "TOTAL_COLOUR_OTHER")
                        {
                            if (!String.IsNullOrEmpty(materialGroupID))
                            {
                                characteristicName = dicTotalColour[materialGroupID];
                            }
                        }
                        else if (iColumn == "CATCHING_METHOD_OTHER")
                        {
                            characteristicName = "ZPKG_SEC_CATCHING_METHOD";
                        }
                        else if (iColumn == "STYLE_OF_PRINTING_OTHER")
                        {
                            if (!String.IsNullOrEmpty(materialGroupID))
                            {
                                characteristicName = dicStyleOfPrinting[materialGroupID];
                            }
                        }
                        else if (iColumn == "SCIENTIFIC_NAME_OTHER")
                        {
                            characteristicName = "ZPKG_SEC_SCIENTIFIC_NAME";
                        }
                        else if (iColumn == "DIRECTION_OF_STICKER_OTHER")
                        {
                            characteristicName = "ZPKG_SEC_DIRECTION";
                        }
                        else if (iColumn == "SPECIE_OTHER")
                        {
                            characteristicName = "ZPKG_SEC_SPECIE";
                        }
                        else if (iColumn == "DIRECTION_OF_STICKER_OTHER")
                        {
                            characteristicName = "ZPKG_SEC_DIRECTION";
                        }

                        if (!String.IsNullOrEmpty(otherValue.ToString()))
                        {
                            if (!String.IsNullOrEmpty(characteristicName))
                            {
                                modelItem = new InboundArtwork();

                                modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                modelItem.Date = artworkObj.Date;
                                modelItem.Time = artworkObj.Time;
                                modelItem.Characteristic = characteristicName;
                                modelItem.Value = otherValue.ToString();
                                modelItem.Description = "";

                                listModelItem.Add(modelItem);
                            }
                        }

                    }
                    catch //(Exception ex)
                    {

                    }


                }
            }




            return listModelItem;
        }

        private static List<InboundArtwork> GetCharacteristicsValue_PG(ArtworkObject artworkObj, ART_WF_ARTWORK_PROCESS_PA dataPA, ART_WF_ARTWORK_PROCESS_PG dataPG, ARTWORKEntities context)
        {

            InboundArtwork modelItem = new InboundArtwork();
            List<InboundArtwork> listModelItem = new List<InboundArtwork>();
            List<string> _listColumn = new List<string>();
            List<string> _listColumnOther = new List<string>();
            List<string> _listColumnMulti = new List<string>();

            Dictionary<string, string> dicGradeOf = new Dictionary<string, string>();
            Dictionary<string, string> dicFlute = new Dictionary<string, string>();
            Dictionary<string, string> dicCount = new Dictionary<string, string>();
            Dictionary<string, string> dicRollSheet = new Dictionary<string, string>();
            Dictionary<string, string> dicDimension = new Dictionary<string, string>();
            Dictionary<string, string> dicNumberOfColour = new Dictionary<string, string>();
            Dictionary<string, string> dicPrintSystem = new Dictionary<string, string>();

            _listColumn.Add("VENDOR");
            _listColumn.Add("GRADE_OF");
            _listColumn.Add("FLUTE");
            _listColumn.Add("DI_CUT");
            _listColumn.Add("ROLL_SHEET");
            _listColumn.Add("DIMENSION_OF");
            _listColumn.Add("NUMBER_OF_COLOR_ID");
            _listColumn.Add("PRINT_SYSTEM");
            _listColumn.Add("ACCESSORIES");
            _listColumn.Add("SHEET_SIZE");
            //---------by aof 20220118 for CR sustain-- start
            _listColumn.Add("SUSTAIN_MATERIAL");
            _listColumn.Add("PLASTIC_TYPE");
            _listColumn.Add("REUSEABLE");
            _listColumn.Add("RECYCLABLE");
            _listColumn.Add("COMPOSATABLE");
            _listColumn.Add("SUSTAIN_OTHER");
            _listColumn.Add("RECYCLE_CONTENT");
            _listColumn.Add("CERT");
            _listColumn.Add("CERT_SOURCE");
            _listColumn.Add("PKG_WEIGHT");

            //---------by aof 20220118 for CR sustain-- - end



            _listColumnOther.Add("VENDOR_OTHER");
            _listColumnOther.Add("GRADE_OF_OTHER");
            _listColumnOther.Add("FLUTE_OTHER");
            _listColumnOther.Add("DI_CUT_OTHER");
            _listColumnOther.Add("ROLL_SHEET_OTHER");
            _listColumnOther.Add("NUMBER_OF_COLOR_OTHER");

            dicPrintSystem.Add("J", "ZPKG_SEC_PRINTING_SYSTEM");

            dicRollSheet.Add("J", "ZPKG_SEC_ROLL_SHEET");

            dicGradeOf.Add("C", "ZPKG_SEC_CARDBOARD_GRADE");
            dicGradeOf.Add("F", "ZPKG_SEC_CARTON_GRADE");
            dicGradeOf.Add("D", "ZPKG_SEC_DISPLAYER_GRADE");
            dicGradeOf.Add("N", "ZPKG_SEC_INNER_GRADE");
            dicGradeOf.Add("P", "ZPKG_SEC_INSERT_GRADE");
            dicGradeOf.Add("K", "ZPKG_SEC_LABEL_GRADE");
            dicGradeOf.Add("L", "ZPKG_SEC_LEAFTLET_GRADE");
            dicGradeOf.Add("R", "ZPKG_SEC_INNER_NON_GRADE");
            dicGradeOf.Add("M", "ZPKG_SEC_PLAST_GRADE");
            dicGradeOf.Add("H", "ZPKG_SEC_SLEEVE_BOX_GRADE");
            dicGradeOf.Add("J", "ZPKG_SEC_STICKER_GRADE");
            dicGradeOf.Add("G", "ZPKG_SEC_TRAY_GRADE");


            dicFlute.Add("C", "ZPKG_SEC_CARDBOARD_FLUTE");
            dicFlute.Add("F", "ZPKG_SEC_CARTON_FLUTE");
            dicFlute.Add("D", "ZPKG_SEC_DISPLAYER_FLUTE");
            dicFlute.Add("N", "ZPKG_SEC_INNER_FLUTE");
            dicFlute.Add("G", "ZPKG_SEC_TRAY_FLUTE");

            dicDimension.Add("N", "ZPKG_SEC_INNER_DIMENSION");
            dicDimension.Add("C", "ZPKG_SEC_CARDBOARD_DIMENSION");
            dicDimension.Add("F", "ZPKG_SEC_CARTON_DIMENSION");
            dicDimension.Add("D", "ZPKG_SEC_DISPLAYER_DIMENSION");
            dicDimension.Add("P", "ZPKG_SEC_INSERST_DIMENSION");
            dicDimension.Add("L", "ZPKG_SEC_LEAFTLET_DIMENSION");
            dicDimension.Add("R", "ZPKG_SEC_INNER_NON_DIMENSION");
            // dicDimension.Add("N", "ZPKG_SEC_PLAST_DIMENSION");
            dicDimension.Add("H", "ZPKG_SEC_SLEEVE_BOX_DIMENSION");
            dicDimension.Add("J", "ZPKG_SEC_STICKER_DIMENSION");
            dicDimension.Add("G", "ZPKG_SEC_TRAY_DIMENSION");

            dicNumberOfColour.Add("F", "ZPKG_SEC_CAR_TOTAL_COLOUR");
            dicNumberOfColour.Add("C", "ZPKG_SEC_CARD_TOTAL_COLOUR");
            dicNumberOfColour.Add("D", "ZPKG_SEC_DISP_TOTAL_COLOUR");
            dicNumberOfColour.Add("R", "ZPKG_SEC_INN_NO_TOTAL_COLOUR");
            dicNumberOfColour.Add("N", "ZPKG_SEC_INNER_TOTAL_COLOUR");
            dicNumberOfColour.Add("P", "ZPKG_SEC_INST_TOTAL_COLOUR");
            dicNumberOfColour.Add("K", "ZPKG_SEC_LABE_TOTAL_COLOUR");
            dicNumberOfColour.Add("L", "ZPKG_SEC_LEA_TOTAL_COLOUR");
            dicNumberOfColour.Add("H", "ZPKG_SEC_SLEV_TOTAL_COLOUR");
            dicNumberOfColour.Add("J", "ZPKG_SEC_STKC_TOTAL_COLOUR");
            dicNumberOfColour.Add("G", "ZPKG_SEC_TRAY_TOTAL_COLOUR");
            dicNumberOfColour.Add("M", "ZPKG_SEC_PLAST_TOTAL_COLOUR");


            var stepMOPG = (from s in context.ART_M_STEP_MOCKUP
                            where s.STEP_MOCKUP_CODE == "SEND_PG"
                            select s).Select(s => s.STEP_MOCKUP_ID).FirstOrDefault();

            var processMO = (from m in context.ART_WF_MOCKUP_PROCESS
                             where m.MOCKUP_ID == dataPG.DIE_LINE_MOCKUP_ID
                               && m.CURRENT_STEP_ID == stepMOPG
                             select m).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

            var mockupData = (from m in context.ART_WF_MOCKUP_PROCESS_PG
                              where m.MOCKUP_SUB_ID == processMO.MOCKUP_SUB_ID
                              select m).OrderByDescending(o => o.MOCKUP_SUB_PG_ID).FirstOrDefault();

            var checkListPG = (from c in context.ART_WF_MOCKUP_CHECK_LIST_PG
                               where c.MOCKUP_ID == processMO.MOCKUP_ID
                               select c).FirstOrDefault();

            artworkObj.FinalInfoGroup = mockupData.FINAL_INFO;

            if (mockupData != null)
            {
                string materialGroupID = "";
                materialGroupID = dataPA.MATERIAL_GROUP_ID.ToString();
                var matGroup = context.SAP_M_CHARACTERISTIC.Where(w => w.CHARACTERISTIC_ID == dataPA.MATERIAL_GROUP_ID).Select(s => s.VALUE).FirstOrDefault();
                materialGroupID = matGroup; //data.MATERIAL_GROUP_ID.ToString();

                if (_listColumn.Count > 0)
                {
                    foreach (string iColumn in _listColumn)
                    {
                        try
                        {
                            if (iColumn == "VENDOR")
                            {
                                if (mockupData.VENDOR != null && mockupData.VENDOR > -1)
                                {
                                    var vendor = (from v in context.XECM_M_VENDOR
                                                  where v.VENDOR_ID == mockupData.VENDOR
                                                  select v).FirstOrDefault();

                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = "ZPKG_SEC_VENDOR";
                                    modelItem.Value = vendor.VENDOR_CODE;
                                    modelItem.Description = vendor.VENDOR_NAME;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "ACCESSORIES")
                            {

                                modelItem = new InboundArtwork();

                                modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                modelItem.Date = artworkObj.Date;
                                modelItem.Time = artworkObj.Time;
                                modelItem.Characteristic = "ZPKG_SEC_ACCESSORIES";
                                modelItem.Value = mockupData.ACCESSORIES;
                                modelItem.Description = mockupData.ACCESSORIES;

                                listModelItem.Add(modelItem);

                            }
                            else if (iColumn == "GRADE_OF")
                            {
                                if (mockupData.GRADE_OF != null && mockupData.GRADE_OF > -1)
                                {
                                    SAP_M_CHARACTERISTIC gradeOf = new SAP_M_CHARACTERISTIC();
                                    gradeOf = CNService.GetCharacteristicData(mockupData.GRADE_OF, context);

                                    if (gradeOf != null)
                                    {
                                        modelItem = new InboundArtwork();

                                        modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                        modelItem.Date = artworkObj.Date;
                                        modelItem.Time = artworkObj.Time;
                                        modelItem.Characteristic = dicGradeOf[materialGroupID];
                                        modelItem.Value = gradeOf.VALUE;
                                        modelItem.Description = gradeOf.DESCRIPTION;

                                        listModelItem.Add(modelItem);
                                    }
                                }
                            }

                            else if (iColumn == "FLUTE")
                            {
                                if (mockupData.FLUTE != null && mockupData.FLUTE > -1)
                                {
                                    SAP_M_CHARACTERISTIC flute = new SAP_M_CHARACTERISTIC();
                                    flute = CNService.GetCharacteristicData(mockupData.FLUTE, context);

                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = dicFlute[materialGroupID];
                                    modelItem.Value = flute.VALUE;
                                    modelItem.Description = flute.DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "DI_CUT")
                            {
                                if (mockupData.DI_CUT != null && mockupData.DI_CUT > -1)
                                {
                                    SAP_M_CHARACTERISTIC diCut = new SAP_M_CHARACTERISTIC();
                                    diCut = CNService.GetCharacteristicData(mockupData.DI_CUT, context);

                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = "ZPKG_SEC_RSC_DI";
                                    modelItem.Value = diCut.VALUE;
                                    modelItem.Description = diCut.DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "SHEET_SIZE")
                            {
                                if (mockupData.SHEET_SIZE != null)
                                {
                                    //SAP_M_CHARACTERISTIC sheetSize = new SAP_M_CHARACTERISTIC();

                                    //sheetSize.VALUE = mockupData.SHEET_SIZE;
                                    //sheetSize.DESCRIPTION = mockupData.SHEET_SIZE;

                                    //var sheetSizeTmp = SAP_M_CHARACTERISTIC_SERVICE.GetByItemContain(sheetSize, context).FirstOrDefault();

                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = "ZPKG_SEC_SHEET_SIZE";

                                    //if (sheetSizeTmp != null)
                                    //{
                                    //    modelItem.Value = sheetSizeTmp.VALUE;
                                    //    modelItem.Description = sheetSizeTmp.DESCRIPTION;
                                    //}
                                    //else
                                    //{
                                    modelItem.Value = mockupData.SHEET_SIZE;
                                    modelItem.Description = mockupData.SHEET_SIZE;
                                    //}

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "ROLL_SHEET")
                            {
                                if (!String.IsNullOrEmpty(mockupData.ROLL_SHEET) && !String.IsNullOrEmpty(dicRollSheet[materialGroupID]))
                                {

                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = dicRollSheet[materialGroupID];
                                    modelItem.Value = mockupData.ROLL_SHEET;
                                    modelItem.Description = mockupData.ROLL_SHEET;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "PRINT_SYSTEM")
                            {
                                if (!String.IsNullOrEmpty(mockupData.PRINT_SYSTEM) && !String.IsNullOrEmpty(dicPrintSystem[materialGroupID]))
                                {

                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = dicPrintSystem[materialGroupID];
                                    modelItem.Value = mockupData.PRINT_SYSTEM;
                                    modelItem.Description = mockupData.PRINT_SYSTEM;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "DIMENSION_OF")
                            {
                                if (!String.IsNullOrEmpty(mockupData.DIMENSION_OF) && !String.IsNullOrEmpty(dicDimension[materialGroupID]))
                                {
                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = dicDimension[materialGroupID];
                                    modelItem.Value = mockupData.DIMENSION_OF;
                                    modelItem.Description = mockupData.DIMENSION_OF;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "NUMBER_OF_COLOR_ID")
                            {
                                if (checkListPG != null)
                                {
                                    if (checkListPG.NUMBER_OF_COLOR_ID != null && checkListPG.NUMBER_OF_COLOR_ID > -1 && !String.IsNullOrEmpty(dicNumberOfColour[materialGroupID]))
                                    {
                                        SAP_M_CHARACTERISTIC numberOfColor = new SAP_M_CHARACTERISTIC();
                                        numberOfColor = CNService.GetCharacteristicData(checkListPG.NUMBER_OF_COLOR_ID, context);

                                        if (numberOfColor != null)
                                        {
                                            modelItem = new InboundArtwork();

                                            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                            modelItem.Date = artworkObj.Date;
                                            modelItem.Time = artworkObj.Time;
                                            modelItem.Characteristic = dicNumberOfColour[materialGroupID];
                                            modelItem.Value = numberOfColor.VALUE;
                                            modelItem.Description = numberOfColor.DESCRIPTION;

                                            listModelItem.Add(modelItem);
                                        }
                                    }
                                }
                            }
                            //---------by aof 20220118 for CR sustain-- start
                            else if (iColumn == "SUSTAIN_MATERIAL")  //1
                            {
                                if (mockupData.SUSTAIN_MATERIAL != null && mockupData.SUSTAIN_MATERIAL > -1)
                                {
                                    SAP_M_CHARACTERISTIC pkgSecMaterial = new SAP_M_CHARACTERISTIC();
                                    pkgSecMaterial = CNService.GetCharacteristicData(mockupData.SUSTAIN_MATERIAL, context);

                                    modelItem = new InboundArtwork();
                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = "ZPKG_SEC_MATERIAL";
                                    modelItem.Value = pkgSecMaterial.VALUE;
                                    modelItem.Description = pkgSecMaterial.DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "PLASTIC_TYPE")   //2
                            {
                                if (mockupData.PLASTIC_TYPE != null && mockupData.PLASTIC_TYPE > -1)
                                {
                                    SAP_M_CHARACTERISTIC pkgSecPlastic = new SAP_M_CHARACTERISTIC();
                                    pkgSecPlastic = CNService.GetCharacteristicData(mockupData.PLASTIC_TYPE, context);

                                    modelItem = new InboundArtwork();
                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = "ZPKG_SEC_PLASTIC";
                                    modelItem.Value = pkgSecPlastic.VALUE;
                                    modelItem.Description = pkgSecPlastic.DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }

                            else if (iColumn == "REUSEABLE")  //3
                            {
                                if (mockupData.REUSEABLE != null)
                                {
                                    var val = GetYesNoValue(mockupData.REUSEABLE);
                                    modelItem = new InboundArtwork();
                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = "ZPKG_SEC_REUSEABLE";
                                    modelItem.Value = val;
                                    modelItem.Description = val;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "RECYCLABLE")  //4
                            {
                                if (mockupData.RECYCLABLE != null)
                                {
                                    var val = GetYesNoValue(mockupData.RECYCLABLE);
                                    modelItem = new InboundArtwork();
                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = "ZPKG_SEC_RECYCLABLE";
                                    modelItem.Value = val;
                                    modelItem.Description = val;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "COMPOSATABLE")  //5
                            {
                                if (mockupData.COMPOSATABLE != null)
                                {
                                    var val = GetYesNoValue(mockupData.COMPOSATABLE);
                                    modelItem = new InboundArtwork();
                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = "ZPKG_SEC_COMPOSATABLE";
                                    modelItem.Value = val;
                                    modelItem.Description = val;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "SUSTAIN_OTHER")  //6
                            {
                                if (mockupData.SUSTAIN_OTHER != null)
                                {
                                  
                                    modelItem = new InboundArtwork();
                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = "ZPKG_SEC_OTHER";
                                    modelItem.Value = mockupData.SUSTAIN_OTHER;
                                    modelItem.Description = mockupData.SUSTAIN_OTHER;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "RECYCLE_CONTENT") //7
                            {
                                if (mockupData.RECYCLE_CONTENT != null)
                                {                    
                                    modelItem = new InboundArtwork();
                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = "ZPKG_SEC_RECYCLED_CONTENT";
                                    modelItem.Value = mockupData.RECYCLE_CONTENT.ToString();
                                    modelItem.Description = mockupData.RECYCLE_CONTENT.ToString();

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "CERT")  //8
                            {
                                if (mockupData.CERT != null)
                                {
                                    var val = GetYesNoValue(mockupData.CERT);
                                    modelItem = new InboundArtwork();
                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = "ZPKG_SEC_CERT";
                                    modelItem.Value = val;
                                    modelItem.Description = val;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "CERT_SOURCE")  //9
                            {
                                if (mockupData.CERT_SOURCE != null && mockupData.CERT_SOURCE > -1)
                                {
                                    SAP_M_CHARACTERISTIC pkgSecCertSource = new SAP_M_CHARACTERISTIC();
                                    pkgSecCertSource = CNService.GetCharacteristicData(mockupData.CERT_SOURCE, context);

                                    modelItem = new InboundArtwork();
                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = "ZPKG_SEC_CERT_SOURCE";
                                    modelItem.Value = pkgSecCertSource.VALUE;
                                    modelItem.Description = pkgSecCertSource.DESCRIPTION;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "PKG_WEIGHT")  //10
                            {
                                if (mockupData.PKG_WEIGHT != null)
                                {
                                    var weg = mockupData.PKG_WEIGHT.GetValueOrDefault(0).ToString("###,###,###,##0.000");
                                    modelItem = new InboundArtwork();
                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = "ZPKG_SEC_WEIGHT";
                                    modelItem.Value = weg;
                                    modelItem.Description = weg;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            //---------by aof 20220118 for CR sustain-- - end
                            else
                            {
                                //var id = dataPA.GetType().GetProperty(iColumn).GetValue(dataPA);

                                //if (id != null)
                                //{
                                //    int charID;
                                //    bool res = int.TryParse(id.ToString(), out charID);
                                //    if (res == true && charID > 0)
                                //    {
                                //        var characteristic = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == charID).FirstOrDefault();
                                //        if (characteristic != null)
                                //        {
                                //            modelItem = new InboundArtwork();

                                //            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                //            modelItem.Date = artworkObj.Date;
                                //            modelItem.Time = artworkObj.Time;
                                //            modelItem.Characteristic = characteristic.NAME;
                                //            modelItem.Value = characteristic.VALUE;
                                //            modelItem.Description = characteristic.DESCRIPTION;

                                //            listModelItem.Add(modelItem);
                                //        }
                                //    }
                                //}
                            }

                        }
                        catch //(Exception ex)
                        {

                        }


                    }
                }

                if (_listColumnOther.Count > 0)
                {
                    foreach (string iColumn in _listColumnOther)
                    {
                        try
                        {
                            if (iColumn == "VENDOR_OTHER")
                            {
                                if (mockupData.VENDOR == -1 && !String.IsNullOrEmpty(mockupData.VENDOR_OTHER))
                                {

                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = "ZPKG_SEC_VENDOR";
                                    modelItem.Value = mockupData.VENDOR_OTHER;
                                    modelItem.Description = mockupData.VENDOR_OTHER;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "GRADE_OF_OTHER")
                            {
                                if (mockupData.GRADE_OF == -1 && !String.IsNullOrEmpty(mockupData.GRADE_OF_OTHER))
                                {

                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = dicGradeOf[materialGroupID];
                                    modelItem.Value = mockupData.GRADE_OF_OTHER;
                                    modelItem.Description = mockupData.GRADE_OF_OTHER;

                                    listModelItem.Add(modelItem);

                                }
                            }
                            else if (iColumn == "FLUTE_OTHER")
                            {
                                if (mockupData.FLUTE == -1 && !String.IsNullOrEmpty(mockupData.FLUTE_OTHER))
                                {
                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = dicFlute[materialGroupID];
                                    modelItem.Value = mockupData.FLUTE_OTHER;
                                    modelItem.Description = mockupData.FLUTE_OTHER;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "DI_CUT_OTHER")
                            {
                                if (mockupData.DI_CUT == -1 && !String.IsNullOrEmpty(mockupData.DI_CUT_OTHER))
                                {
                                    modelItem = new InboundArtwork();

                                    modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                    modelItem.Date = artworkObj.Date;
                                    modelItem.Time = artworkObj.Time;
                                    modelItem.Characteristic = "ZPKG_SEC_RSC_DI";
                                    modelItem.Value = mockupData.DI_CUT_OTHER;
                                    modelItem.Description = mockupData.DI_CUT_OTHER;

                                    listModelItem.Add(modelItem);
                                }
                            }
                            else if (iColumn == "NUMBER_OF_COLOR_OTHER")
                            {
                                if (checkListPG != null)
                                {
                                    if (checkListPG.NUMBER_OF_COLOR_ID == -1 && !String.IsNullOrEmpty(checkListPG.NUMBER_OF_COLOR_OTHER) && !String.IsNullOrEmpty(dicNumberOfColour[materialGroupID]))
                                    {
                                        modelItem = new InboundArtwork();

                                        modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                        modelItem.Date = artworkObj.Date;
                                        modelItem.Time = artworkObj.Time;
                                        modelItem.Characteristic = dicNumberOfColour[materialGroupID];
                                        modelItem.Value = checkListPG.NUMBER_OF_COLOR_OTHER;
                                        modelItem.Description = checkListPG.NUMBER_OF_COLOR_OTHER;

                                        listModelItem.Add(modelItem);
                                    }
                                }
                            }
                            else
                            {
                                //var id = dataPA.GetType().GetProperty(iColumn).GetValue(dataPA);

                                //if (id != null)
                                //{
                                //    int charID;
                                //    bool res = int.TryParse(id.ToString(), out charID);
                                //    if (res == true && charID > 0)
                                //    {
                                //        var characteristic = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == charID).FirstOrDefault();
                                //        if (characteristic != null)
                                //        {
                                //            modelItem = new InboundArtwork();

                                //            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
                                //            modelItem.Date = artworkObj.Date;
                                //            modelItem.Time = artworkObj.Time;
                                //            modelItem.Characteristic = characteristic.NAME;
                                //            modelItem.Value = characteristic.VALUE;
                                //            modelItem.Description = characteristic.DESCRIPTION;

                                //            listModelItem.Add(modelItem);
                                //        }
                                //    }
                                //}
                            }

                        }
                        catch //(Exception ex)
                        {

                        }


                    }
                }

            }
            return listModelItem;
        }
        //private static List<InboundArtwork> GetCharacteristicsValue_BAK(ArtworkObject artworkObj, ART_WF_ARTWORK_PROCESS_PA data, ARTWORKEntities context)
        //{
        //    InboundArtwork modelItem = new InboundArtwork();
        //    List<InboundArtwork> listModelItem = new List<InboundArtwork>();
        //    List<string> _listColumn = new List<string>();
        //    List<string> _listColumnOther = new List<string>();
        //    List<string> _listColumnMulti = new List<string>();

        //    _listColumn.Add("MATERIAL_GROUP_ID");
        //    _listColumn.Add("TYPE_OF_ID");
        //    _listColumn.Add("TYPE_OF_2_ID");
        //    _listColumn.Add("TYPE_OF_2_ID");
        //    _listColumn.Add("PLANT_REGISTERED_ID");
        //    _listColumn.Add("PMS_COLOUR_ID");
        //    _listColumn.Add("COMPANY_ADDRESS_ID");
        //    _listColumn.Add("PROCESS_COLOUR_ID");
        //    _listColumn.Add("CATCHING_PERIOD_ID");
        //    _listColumn.Add("TOTAL_COLOUR_ID");
        //    _listColumn.Add("CATCHING_METHOD_ID");
        //    _listColumn.Add("SCIENTIFIC_NAME_ID");
        //    _listColumn.Add("STYLE_OF_PRINTING_ID");
        //    _listColumn.Add("SPECIE_ID");
        //    _listColumn.Add("DIRECTION_OF_STICKER_ID");
        //    _listColumn.Add("ZPKG_SEC_BRAND");
        //    _listColumn.Add("ZPKG_SEC_PRIMARY_SIZE");
        //    _listColumn.Add("ZPKG_SEC_CONTAINER_TYPE");
        //    _listColumn.Add("ZPKG_SEC_LID_TYPE");
        //    _listColumn.Add("ZPKG_SEC_CHANGE_POINT");
        //    _listColumn.Add("ZPKG_SEC_DIRECTION");
        //    _listColumn.Add("ZPKG_SEC_PRIMARY_TYPE");
        //    _listColumn.Add("PACKING_STYLE_ID");
        //    _listColumn.Add("PACK_SIZE_ID");

        //    _listColumnOther.Add("TYPE_OF_OTHER");
        //    _listColumnOther.Add("TYPE_OF_2_OTHER");
        //    _listColumnOther.Add("PLANT_REGISTERED_OTHER");
        //    _listColumnOther.Add("PMS_COLOUR_OTHER");
        //    _listColumnOther.Add("COMPANY_ADDRESS_OTHER");
        //    _listColumnOther.Add("PROCESS_COLOUR_OTHER");
        //    _listColumnOther.Add("CATCHING_PERIOD_OTHER");
        //    _listColumnOther.Add("TOTAL_COLOUR_OTHER");
        //    _listColumnOther.Add("CATCHING_METHOD_OTHER");
        //    _listColumnOther.Add("STYLE_OF_PRINTING_OTHER");
        //    _listColumnOther.Add("SCIENTIFIC_NAME_OTHER");
        //    _listColumnOther.Add("DIRECTION_OF_STICKER_OTHER");
        //    _listColumnOther.Add("SPECIE_OTHER");
        //    _listColumnOther.Add("PRIMARY_SIZE_OTHER");
        //    _listColumnOther.Add("CONTAINER_TYPE_OTHER");
        //    _listColumnOther.Add("LID_TYPE_OTHER");
        //    _listColumnOther.Add("PACKING_STYLE_OTHER");
        //    _listColumnOther.Add("PACK_SIZE_OTHER");
        //    _listColumnOther.Add("DIRECTION_OF_STICKER_OTHER");


        //    _listColumnMulti.Add("ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE");
        //    _listColumnMulti.Add("ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA");
        //    _listColumnMulti.Add("ART_WF_ARTWORK_PROCESS_PA_SYMBOL");

        //    Dictionary<string, string> dicTypeOf = new Dictionary<string, string>();
        //    Dictionary<string, string> dicTypeOf2 = new Dictionary<string, string>();
        //    Dictionary<string, string> dicPlantRegister = new Dictionary<string, string>();
        //    Dictionary<string, string> dicPMSColour = new Dictionary<string, string>();
        //    Dictionary<string, string> dicCompany = new Dictionary<string, string>();
        //    Dictionary<string, string> dicProcessColour = new Dictionary<string, string>();
        //    Dictionary<string, string> dicCatchingPeriod = new Dictionary<string, string>();
        //    Dictionary<string, string> dicTotalColour = new Dictionary<string, string>();
        //    Dictionary<string, string> dicCatchingMethod = new Dictionary<string, string>();
        //    Dictionary<string, string> dicStyleOfPrinting = new Dictionary<string, string>();
        //    Dictionary<string, string> dicScientificName = new Dictionary<string, string>();
        //    Dictionary<string, string> dicDirectionOfSticker = new Dictionary<string, string>();
        //    Dictionary<string, string> dicSpecie = new Dictionary<string, string>();
        //    Dictionary<string, string> dicPackingStyle = new Dictionary<string, string>();
        //    Dictionary<string, string> dicPackSize = new Dictionary<string, string>();

        //    dicTypeOf.Add("C", "ZPKG_SEC_CARDBOARD_TYPE_1");
        //    dicTypeOf.Add("D", "ZPKG_SEC_DISPLAYER_TYPE_1");
        //    dicTypeOf.Add("F", "ZPKG_SEC_CARTON_TYPE_1");
        //    dicTypeOf.Add("G", "ZPKG_SEC_TRAY_TYPE");
        //    dicTypeOf.Add("H", "ZPKG_SEC_SLEEVE_BOX_TYPE");
        //    dicTypeOf.Add("J", "ZPKG_SEC_STICKER_TYPE");
        //    dicTypeOf.Add("K", "ZPKG_SEC_LABEL_TYPE");
        //    dicTypeOf.Add("L", "ZPKG_SEC_LEAFTLET_TYPE");
        //    dicTypeOf.Add("M", "ZPKG_SEC_STYLE_PLASTIC");
        //    dicTypeOf.Add("N", "ZPKG_SEC_INNER_TYPE_1");
        //    dicTypeOf.Add("P", "ZPKG_SEC_INSERT_TYPE");
        //    dicTypeOf.Add("R", "ZPKG_SEC_INNER_NON_TYPE");

        //    dicTypeOf2.Add("C", "ZPKG_SEC_CARDBOARD_TYPE_2");
        //    dicTypeOf2.Add("D", "ZPKG_SEC_DISPLAYER_TYPE_2");
        //    dicTypeOf2.Add("F", "ZPKG_SEC_CARTON_TYPE_2");
        //    dicTypeOf2.Add("G", "ZPKG_SEC_TRAY_CARTON_TYPE");
        //    dicTypeOf2.Add("N", "ZPKG_SEC_INNER_TYPE_2");
        //    dicTypeOf2.Add("R", "ZPKG_SEC_INNER_TYPE_2");

        //    dicPMSColour.Add("F", "ZPKG_SEC_CAR_PMS_COLOUR");
        //    dicPMSColour.Add("C", "ZPKG_SEC_CARD_PMS_COLOUR");
        //    dicPMSColour.Add("D", "ZPKG_SEC_DISP_PMS_COLOUR");
        //    dicPMSColour.Add("R", "ZPKG_SEC_INN_NO_PMS_COLOUR");
        //    dicPMSColour.Add("N", "ZPKG_SEC_INNER_PMS_COLOUR");
        //    dicPMSColour.Add("P", "ZPKG_SEC_INST_PMS_COLOUR");
        //    dicPMSColour.Add("K", "ZPKG_SEC_LABE_PMS_COLOUR");
        //    dicPMSColour.Add("L", "ZPKG_SEC_LEA_PMS_COLOUR");
        //    dicPMSColour.Add("H", "ZPKG_SEC_SLEV_PMS_COLOUR");
        //    dicPMSColour.Add("J", "ZPKG_SEC_STKC_PMS_COLOUR");
        //    dicPMSColour.Add("G", "ZPKG_SEC_TRAY_PMS_COLOUR");

        //    dicProcessColour.Add("F", "ZPKG_SEC_CAR_PROCESS_COLOUR");
        //    dicProcessColour.Add("C", "ZPKG_SEC_CARD_PROCESS_COLOUR");
        //    dicProcessColour.Add("D", "ZPKG_SEC_DISP_PROCESS_COLOUR");
        //    dicProcessColour.Add("R", "ZPKG_SEC_INN_NO_PROCESS_COLOUR");
        //    dicProcessColour.Add("N", "ZPKG_SEC_INNER_PROCESS_COLOUR");
        //    dicProcessColour.Add("P", "ZPKG_SEC_INST_PROCESS_COLOUR");
        //    dicProcessColour.Add("K", "ZPKG_SEC_LABE_PROCESS_COLOUR");
        //    dicProcessColour.Add("L", "ZPKG_SEC_LEA_PROCESS_COLOUR");
        //    dicProcessColour.Add("H", "ZPKG_SEC_SLEV_PROCESS_COLOUR");
        //    dicProcessColour.Add("J", "ZPKG_SEC_STKC_PROCESS_COLOUR");
        //    dicProcessColour.Add("G", "ZPKG_SEC_TRAY_PROCESS_COLOUR");

        //    dicTotalColour.Add("F", "ZPKG_SEC_CAR_TOTAL_COLOUR");
        //    dicTotalColour.Add("C", "ZPKG_SEC_CARD_TOTAL_COLOUR");
        //    dicTotalColour.Add("D", "ZPKG_SEC_DISP_TOTAL_COLOUR");
        //    dicTotalColour.Add("R", "ZPKG_SEC_INN_NO_TOTAL_COLOUR");
        //    dicTotalColour.Add("N", "ZPKG_SEC_INNER_TOTAL_COLOUR");
        //    dicTotalColour.Add("P", "ZPKG_SEC_INST_TOTAL_COLOUR");
        //    dicTotalColour.Add("K", "ZPKG_SEC_LABE_TOTAL_COLOUR");
        //    dicTotalColour.Add("L", "ZPKG_SEC_LEA_TOTAL_COLOUR");
        //    dicTotalColour.Add("H", "ZPKG_SEC_SLEV_TOTAL_COLOUR");
        //    dicTotalColour.Add("J", "ZPKG_SEC_STKC_TOTAL_COLOUR");
        //    dicTotalColour.Add("G", "ZPKG_SEC_TRAY_TOTAL_COLOUR");
        //    dicTotalColour.Add("M", "ZPKG_SEC_PLAST_TOTAL_COLOUR");

        //    dicStyleOfPrinting.Add("F", "ZPKG_SEC_CAR_PRINTING_STYLE");
        //    dicStyleOfPrinting.Add("C", "ZPKG_SEC_CARD_PRINTING_STYLE");
        //    dicStyleOfPrinting.Add("D", "ZPKG_SEC_DISP_PRINTING_STYLE");
        //    dicStyleOfPrinting.Add("R", "ZPKG_SEC_INN_NO_PRINTING_STYLE");
        //    dicStyleOfPrinting.Add("N", "ZPKG_SEC_INNER_PRINTING_STYLE");
        //    dicStyleOfPrinting.Add("P", "ZPKG_SEC_INST_PRINTING_STYLE");
        //    dicStyleOfPrinting.Add("K", "ZPKG_SEC_LABE_PRINTING_STYLE");
        //    dicStyleOfPrinting.Add("L", "ZPKG_SEC_LEA_PRINTING_STYLE");
        //    dicStyleOfPrinting.Add("H", "ZPKG_SEC_SLEV_PRINTING_STYLE");
        //    dicStyleOfPrinting.Add("J", "ZPKG_SEC_STKC_PRINTING_STYLE");
        //    dicStyleOfPrinting.Add("G", "ZPKG_SEC_TRAY_PRINTING_STYLE");

        //    dicPackingStyle.Add("C", "ZPKG_SEC_CARDBOARD");
        //    dicPackingStyle.Add("F", "ZPKG_SEC_CARTON");
        //    dicPackingStyle.Add("D", "ZPKG_SEC_DISPLAYER");
        //    dicPackingStyle.Add("N", "ZPKG_SEC_INNER_COR");
        //    dicPackingStyle.Add("R", "ZPKG_SEC_INNER_NON");
        //    dicPackingStyle.Add("M", "ZPKG_SEC_PLASTIC");
        //    dicPackingStyle.Add("H", "ZPKG_SEC_SLEEVEBOX");
        //    dicPackingStyle.Add("G", "ZPKG_SEC_TRAY");

        //    dicPackSize.Add("C", "ZPKG_SEC_CARDBOARD");
        //    dicPackSize.Add("F", "ZPKG_SEC_CARTON");
        //    dicPackSize.Add("D", "ZPKG_SEC_DISPLAYER");
        //    dicPackSize.Add("N", "ZPKG_SEC_INNER_COR");
        //    dicPackSize.Add("R", "ZPKG_SEC_INNER_NON");
        //    dicPackSize.Add("P", "ZPKG_SEC_INSERT");
        //    dicPackSize.Add("L", "ZPKG_SEC_LEAFTLET");
        //    dicPackSize.Add("G", "ZPKG_SEC_PLASTIC");
        //    dicPackSize.Add("H", "ZPKG_SEC_SLEEVEBOX");
        //    dicPackSize.Add("J", "ZPKG_SEC_STICKER");
        //    // dicPackSize.Add("G", "ZPKG_SEC_TRAY");

        //    if (_listColumnMulti.Count > 0)
        //    {
        //        foreach (string iColumn in _listColumnMulti)
        //        {

        //            if (iColumn == "ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE")
        //            {
        //                var fao = context.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE.Where(f => f.ARTWORK_SUB_PA_ID == data.ARTWORK_SUB_PA_ID).ToList();

        //                if (fao != null && fao.Count > 0)
        //                {
        //                    String charName = "ZPKG_SEC_FAO";
        //                    SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();
        //                    foreach (ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE iFAO in fao)
        //                    {
        //                        modelItem = new InboundArtwork();

        //                        modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                        modelItem.Date = artworkObj.Date;
        //                        modelItem.Time = artworkObj.Time;

        //                        if (String.IsNullOrEmpty(iFAO.FAO_ZONE_OTHER))
        //                        {
        //                            characteristic = new SAP_M_CHARACTERISTIC();
        //                            characteristic = CNService.GetCharacteristicData(iFAO.FAO_ZONE_ID);

        //                            modelItem.Characteristic = charName;
        //                            modelItem.Value = characteristic.VALUE;
        //                            modelItem.Description = characteristic.DESCRIPTION;
        //                        }
        //                        else
        //                        {
        //                            modelItem.Characteristic = charName;
        //                            modelItem.Value = iFAO.FAO_ZONE_OTHER;
        //                            modelItem.Description = "";
        //                        }
        //                        listModelItem.Add(modelItem);

        //                    }
        //                }
        //            }

        //            if (iColumn == "ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA")
        //            {
        //                var catchingAreas = context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA.Where(f => f.ARTWORK_SUB_PA_ID == data.ARTWORK_SUB_PA_ID).ToList();

        //                if (catchingAreas != null && catchingAreas.Count > 0)
        //                {
        //                    String charName = "ZPKG_SEC_CATCHING_AREA";
        //                    SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();
        //                    foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA iCatching in catchingAreas)
        //                    {
        //                        modelItem = new InboundArtwork();

        //                        modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                        modelItem.Date = artworkObj.Date;
        //                        modelItem.Time = artworkObj.Time;

        //                        if (String.IsNullOrEmpty(iCatching.CATCHING_AREA_OTHER))
        //                        {
        //                            characteristic = new SAP_M_CHARACTERISTIC();
        //                            characteristic = CNService.GetCharacteristicData(iCatching.CATCHING_AREA_ID);

        //                            modelItem.Characteristic = charName;
        //                            modelItem.Value = characteristic.VALUE;
        //                            modelItem.Description = characteristic.DESCRIPTION;
        //                        }
        //                        else
        //                        {
        //                            modelItem.Characteristic = charName;
        //                            modelItem.Value = iCatching.CATCHING_AREA_OTHER;
        //                            modelItem.Description = "";
        //                        }
        //                        listModelItem.Add(modelItem);

        //                    }
        //                }
        //            }

        //            if (iColumn == "ART_WF_ARTWORK_PROCESS_PA_SYMBOL")
        //            {
        //                var catchingAreas = context.ART_WF_ARTWORK_PROCESS_PA_SYMBOL.Where(f => f.ARTWORK_SUB_PA_ID == data.ARTWORK_SUB_PA_ID).ToList();

        //                if (catchingAreas != null && catchingAreas.Count > 0)
        //                {
        //                    String charName = "ZPKG_SEC_SYMBOL";
        //                    SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();
        //                    foreach (ART_WF_ARTWORK_PROCESS_PA_SYMBOL iSymbol in catchingAreas)
        //                    {
        //                        modelItem = new InboundArtwork();

        //                        modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                        modelItem.Date = artworkObj.Date;
        //                        modelItem.Time = artworkObj.Time;

        //                        if (String.IsNullOrEmpty(iSymbol.SYMBOL_OTHER))
        //                        {
        //                            characteristic = new SAP_M_CHARACTERISTIC();
        //                            characteristic = CNService.GetCharacteristicData(iSymbol.SYMBOL_ID);

        //                            modelItem.Characteristic = charName;
        //                            modelItem.Value = characteristic.VALUE;
        //                            modelItem.Description = characteristic.DESCRIPTION;
        //                        }
        //                        else
        //                        {
        //                            modelItem.Characteristic = charName;
        //                            modelItem.Value = iSymbol.SYMBOL_OTHER;
        //                            modelItem.Description = "";
        //                        }
        //                        listModelItem.Add(modelItem);

        //                    }
        //                }
        //            }

        //        }

        //    }

        //    if (_listColumn.Count > 0)
        //    {
        //        string materialGroupID = "";
        //        materialGroupID = data.MATERIAL_GROUP_ID.ToString();
        //        var matGroup = context.SAP_M_CHARACTERISTIC.Where(w => w.CHARACTERISTIC_ID == data.MATERIAL_GROUP_ID).Select(s => s.VALUE).FirstOrDefault();
        //        materialGroupID = matGroup; //data.MATERIAL_GROUP_ID.ToString();

        //        foreach (string iColumn in _listColumn)
        //        {
        //            try
        //            {
        //                var process = context.ART_WF_ARTWORK_PROCESS.Where(p => p.ARTWORK_SUB_ID == data.ARTWORK_SUB_ID).FirstOrDefault();
        //                var request = context.ART_WF_ARTWORK_REQUEST.Where(r => r.ARTWORK_REQUEST_ID == process.ARTWORK_REQUEST_ID).FirstOrDefault();


        //                if (iColumn == "ZPKG_SEC_BRAND")
        //                {


        //                    if (request != null && request.BRAND_ID != null)
        //                    {
        //                        var reqBrand = context.SAP_M_BRAND.Where(b => b.BRAND_ID == request.BRAND_ID).FirstOrDefault();

        //                        if (reqBrand != null)
        //                        {
        //                            modelItem = new InboundArtwork();

        //                            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                            modelItem.Date = artworkObj.Date;
        //                            modelItem.Time = artworkObj.Time;
        //                            modelItem.Characteristic = iColumn;
        //                            modelItem.Value = reqBrand.MATERIAL_GROUP;
        //                            modelItem.Description = reqBrand.DESCRIPTION;

        //                            listModelItem.Add(modelItem);
        //                        }
        //                    }

        //                }
        //                else if (iColumn == "ZPKG_SEC_DIRECTION")
        //                {
        //                    if (data.DIRECTION_OF_STICKER_ID != null)
        //                    {
        //                        if (data.DIRECTION_OF_STICKER_ID > -1)
        //                        {
        //                            var dirSticker = context.SAP_M_CHARACTERISTIC
        //                                            .Where(w => w.CHARACTERISTIC_ID == data.DIRECTION_OF_STICKER_ID)
        //                                            .FirstOrDefault();

        //                            modelItem = new InboundArtwork();

        //                            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                            modelItem.Date = artworkObj.Date;
        //                            modelItem.Time = artworkObj.Time;
        //                            modelItem.Characteristic = iColumn;
        //                            modelItem.Value = dirSticker.VALUE;
        //                            modelItem.Description = dirSticker.DESCRIPTION;

        //                            listModelItem.Add(modelItem);
        //                        }
        //                    }
        //                }
        //                else if (iColumn == "ZPKG_SEC_PRIMARY_TYPE")
        //                {

        //                    var requestID = (from p in context.ART_WF_ARTWORK_PROCESS
        //                                     where p.ARTWORK_SUB_ID == data.ARTWORK_SUB_ID
        //                                     select p.ARTWORK_REQUEST_ID).FirstOrDefault();

        //                    var req = (from i in context.ART_WF_ARTWORK_REQUEST
        //                               where i.ARTWORK_REQUEST_ID == requestID
        //                               select i).FirstOrDefault();

        //                    if (req.PRIMARY_TYPE_ID != null && req.PRIMARY_TYPE_ID > -1)
        //                    {
        //                        var characM = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == req.PRIMARY_TYPE_ID).FirstOrDefault();
        //                        if (characM != null)
        //                        {

        //                            modelItem = new InboundArtwork();

        //                            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                            modelItem.Date = artworkObj.Date;
        //                            modelItem.Time = artworkObj.Time;
        //                            modelItem.Characteristic = iColumn;
        //                            modelItem.Value = characM.VALUE;
        //                            modelItem.Description = characM.DESCRIPTION;

        //                            listModelItem.Add(modelItem);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        modelItem = new InboundArtwork();

        //                        modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                        modelItem.Date = artworkObj.Date;
        //                        modelItem.Time = artworkObj.Time;
        //                        modelItem.Characteristic = iColumn;
        //                        modelItem.Value = req.PRIMARY_TYPE_OTHER;
        //                        modelItem.Description = "";

        //                        listModelItem.Add(modelItem);
        //                    }

        //                }
        //                else if (iColumn == "ZPKG_SEC_PRIMARY_SIZE")
        //                {
        //                    if (data.PRIMARY_SIZE_ID != null)
        //                    {
        //                        var characM = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == data.THREE_P_ID).FirstOrDefault();
        //                        if (characM != null)
        //                        {
        //                            modelItem = new InboundArtwork();

        //                            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                            modelItem.Date = artworkObj.Date;
        //                            modelItem.Time = artworkObj.Time;
        //                            modelItem.Characteristic = iColumn;
        //                            modelItem.Value = characM.VALUE;
        //                            modelItem.Description = characM.DESCRIPTION;

        //                            listModelItem.Add(modelItem);
        //                        }
        //                    }
        //                    else if (data.THREE_P_ID != null)
        //                    {
        //                        var charac = context.SAP_M_3P.Where(c => c.THREE_P_ID == data.THREE_P_ID).FirstOrDefault();
        //                        if (charac != null)
        //                        {
        //                            modelItem = new InboundArtwork();

        //                            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                            modelItem.Date = artworkObj.Date;
        //                            modelItem.Time = artworkObj.Time;
        //                            modelItem.Characteristic = iColumn;
        //                            modelItem.Value = charac.PRIMARY_SIZE_VALUE;
        //                            modelItem.Description = charac.PRIMARY_SIZE_DESCRIPTION;

        //                            listModelItem.Add(modelItem);
        //                        }
        //                    }
        //                }
        //                else if (iColumn == "ZPKG_SEC_CONTAINER_TYPE")
        //                {
        //                    if (data.CONTAINER_TYPE_ID != null)
        //                    {
        //                        var characM = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == data.CONTAINER_TYPE_ID).FirstOrDefault();
        //                        if (characM != null)
        //                        {
        //                            modelItem = new InboundArtwork();

        //                            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                            modelItem.Date = artworkObj.Date;
        //                            modelItem.Time = artworkObj.Time;
        //                            modelItem.Characteristic = iColumn;
        //                            modelItem.Value = characM.VALUE;
        //                            modelItem.Description = characM.DESCRIPTION;

        //                            listModelItem.Add(modelItem);
        //                        }
        //                    }
        //                    else if (data.THREE_P_ID != null)
        //                    {
        //                        var charac = context.SAP_M_3P.Where(c => c.THREE_P_ID == data.THREE_P_ID).FirstOrDefault();
        //                        if (charac != null)
        //                        {
        //                            modelItem = new InboundArtwork();

        //                            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                            modelItem.Date = artworkObj.Date;
        //                            modelItem.Time = artworkObj.Time;
        //                            modelItem.Characteristic = iColumn;
        //                            modelItem.Value = charac.CONTAINER_TYPE_VALUE;
        //                            modelItem.Description = charac.CONTAINER_TYPE_DESCRIPTION;

        //                            listModelItem.Add(modelItem);
        //                        }
        //                    }
        //                }
        //                else if (iColumn == "ZPKG_SEC_LID_TYPE")
        //                {
        //                    if (data.LID_TYPE_ID != null)
        //                    {
        //                        var characM = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == data.LID_TYPE_ID).FirstOrDefault();
        //                        if (characM != null)
        //                        {
        //                            modelItem = new InboundArtwork();

        //                            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                            modelItem.Date = artworkObj.Date;
        //                            modelItem.Time = artworkObj.Time;
        //                            modelItem.Characteristic = iColumn;
        //                            modelItem.Value = characM.VALUE;
        //                            modelItem.Description = characM.DESCRIPTION;

        //                            listModelItem.Add(modelItem);
        //                        }
        //                    }
        //                    else if (data.THREE_P_ID != null)
        //                    {
        //                        var charac = context.SAP_M_3P.Where(c => c.THREE_P_ID == data.THREE_P_ID).FirstOrDefault();
        //                        if (charac != null)
        //                        {
        //                            modelItem = new InboundArtwork();

        //                            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                            modelItem.Date = artworkObj.Date;
        //                            modelItem.Time = artworkObj.Time;
        //                            modelItem.Characteristic = iColumn;
        //                            modelItem.Value = charac.LID_TYPE_VALUE;
        //                            modelItem.Description = charac.LID_TYPE_DESCRIPTION;

        //                            listModelItem.Add(modelItem);
        //                        }
        //                    }
        //                }
        //                else if (iColumn == "PACKING_STYLE_ID")
        //                {
        //                    if (data.PACKING_STYLE_ID != null)
        //                    {
        //                        var characM = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == data.PACKING_STYLE_ID).FirstOrDefault();
        //                        if (characM != null)
        //                        {
        //                            modelItem = new InboundArtwork();

        //                            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                            modelItem.Date = artworkObj.Date;
        //                            modelItem.Time = artworkObj.Time;
        //                            modelItem.Characteristic = dicPackingStyle[materialGroupID];
        //                            modelItem.Value = characM.VALUE;
        //                            modelItem.Description = characM.DESCRIPTION;

        //                            listModelItem.Add(modelItem);
        //                        }
        //                    }
        //                    else if (data.TWO_P_ID != null)
        //                    {
        //                        var charac = context.SAP_M_2P.Where(c => c.TWO_P_ID == data.TWO_P_ID).FirstOrDefault();
        //                        if (charac != null)
        //                        {
        //                            modelItem = new InboundArtwork();

        //                            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                            modelItem.Date = artworkObj.Date;
        //                            modelItem.Time = artworkObj.Time;
        //                            modelItem.Characteristic = dicPackingStyle[materialGroupID];
        //                            modelItem.Value = charac.PACKING_SYLE_VALUE;
        //                            modelItem.Description = charac.PACKING_SYLE_DESCRIPTION;

        //                            listModelItem.Add(modelItem);
        //                        }
        //                    }
        //                }
        //                else if (iColumn == "PACK_SIZE_ID")
        //                {
        //                    if (data.PACK_SIZE_ID != null)
        //                    {
        //                        var characM = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == data.PACK_SIZE_ID).FirstOrDefault();
        //                        if (characM != null)
        //                        {
        //                            modelItem = new InboundArtwork();

        //                            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                            modelItem.Date = artworkObj.Date;
        //                            modelItem.Time = artworkObj.Time;
        //                            modelItem.Characteristic = dicPackSize[materialGroupID];
        //                            modelItem.Value = characM.VALUE;
        //                            modelItem.Description = characM.DESCRIPTION;

        //                            listModelItem.Add(modelItem);
        //                        }
        //                    }
        //                    else if (data.TWO_P_ID != null)
        //                    {
        //                        var charac = context.SAP_M_2P.Where(c => c.TWO_P_ID == data.TWO_P_ID).FirstOrDefault();
        //                        if (charac != null)
        //                        {
        //                            modelItem = new InboundArtwork();

        //                            modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                            modelItem.Date = artworkObj.Date;
        //                            modelItem.Time = artworkObj.Time;
        //                            modelItem.Characteristic = dicPackSize[materialGroupID];
        //                            modelItem.Value = charac.PACK_SIZE_VALUE;
        //                            modelItem.Description = charac.PACK_SIZE_DESCRIPTION;

        //                            listModelItem.Add(modelItem);
        //                        }
        //                    }
        //                }
        //                else if (iColumn == "ZPKG_SEC_CHANGE_POINT")
        //                {
        //                    if (!String.IsNullOrEmpty(data.CHANGE_POINT))
        //                    {
        //                        string _changePointValue = "";
        //                        string _changePointDescription = "";
        //                        if (data.CHANGE_POINT == "1")
        //                        {
        //                            _changePointValue = "C";
        //                            _changePointDescription = " Change Point";
        //                        }
        //                        else if (data.CHANGE_POINT == "0")
        //                        {
        //                            _changePointValue = "N";
        //                            _changePointDescription = "No Change Point";
        //                        }

        //                        modelItem = new InboundArtwork();

        //                        modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                        modelItem.Date = artworkObj.Date;
        //                        modelItem.Time = artworkObj.Time;
        //                        modelItem.Characteristic = iColumn;
        //                        modelItem.Value = _changePointValue;
        //                        modelItem.Description = _changePointDescription;

        //                        listModelItem.Add(modelItem);
        //                    }
        //                }
        //                else
        //                {
        //                    var id = data.GetType().GetProperty(iColumn).GetValue(data);

        //                    if (id != null)
        //                    {
        //                        int charID;
        //                        bool res = int.TryParse(id.ToString(), out charID);
        //                        if (res == true && charID > 0)
        //                        {
        //                            var characteristic = context.SAP_M_CHARACTERISTIC.Where(c => c.CHARACTERISTIC_ID == charID).FirstOrDefault();
        //                            if (characteristic != null)
        //                            {
        //                                modelItem = new InboundArtwork();

        //                                modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                                modelItem.Date = artworkObj.Date;
        //                                modelItem.Time = artworkObj.Time;
        //                                modelItem.Characteristic = characteristic.NAME;
        //                                modelItem.Value = characteristic.VALUE;
        //                                modelItem.Description = characteristic.DESCRIPTION;

        //                                listModelItem.Add(modelItem);
        //                            }
        //                        }
        //                    }
        //                }

        //            }
        //            catch (Exception ex)
        //            {

        //            }


        //        }
        //    }

        //    if (_listColumnOther.Count > 0)
        //    {
        //        string materialGroupID = "";
        //        string characteristicName = "";

        //        var matGroup = context.SAP_M_CHARACTERISTIC.Where(w => w.CHARACTERISTIC_ID == data.MATERIAL_GROUP_ID).Select(s => s.VALUE).FirstOrDefault();
        //        materialGroupID = matGroup; //data.MATERIAL_GROUP_ID.ToString();

        //        foreach (string iColumn in _listColumnOther)
        //        {
        //            try
        //            {
        //                var otherValue = data.GetType().GetProperty(iColumn).GetValue(data);


        //                if (iColumn == "TYPE_OF_OTHER")
        //                {
        //                    if (!String.IsNullOrEmpty(materialGroupID))
        //                    {
        //                        characteristicName = dicTypeOf[materialGroupID];
        //                    }
        //                }

        //                else if (iColumn == "PRIMARY_SIZE_OTHER")
        //                {
        //                    if (!String.IsNullOrEmpty(materialGroupID))
        //                    {
        //                        characteristicName = "ZPKG_SEC_PRIMARY_SIZE";
        //                    }
        //                }
        //                else if (iColumn == "CONTAINER_TYPE_OTHER")
        //                {
        //                    if (!String.IsNullOrEmpty(materialGroupID))
        //                    {
        //                        characteristicName = "ZPKG_SEC_CONTAINER_TYPE";
        //                    }
        //                }
        //                else if (iColumn == "LID_TYPE_OTHER")
        //                {
        //                    if (!String.IsNullOrEmpty(materialGroupID))
        //                    {
        //                        characteristicName = "ZPKG_SEC_LID_TYPE";
        //                    }
        //                }
        //                else if (iColumn == "PACKING_STYLE_OTHER")
        //                {
        //                    if (!String.IsNullOrEmpty(materialGroupID))
        //                    {
        //                        characteristicName = dicPackingStyle[materialGroupID];
        //                    }
        //                }
        //                else if (iColumn == "PACK_SIZE_OTHER")
        //                {
        //                    if (!String.IsNullOrEmpty(materialGroupID))
        //                    {
        //                        characteristicName = dicPackSize[materialGroupID];
        //                    }
        //                }
        //                else if (iColumn == "TYPE_OF_2_OTHER")
        //                {
        //                    if (!String.IsNullOrEmpty(materialGroupID))
        //                    {
        //                        characteristicName = dicTypeOf2[materialGroupID];
        //                    }
        //                }
        //                else if (iColumn == "PLANT_REGISTERED_OTHER")
        //                {
        //                    characteristicName = "ZPKG_SEC_PLANT_REGISTER";
        //                }
        //                else if (iColumn == "PMS_COLOUR_OTHER")
        //                {
        //                    if (!String.IsNullOrEmpty(materialGroupID))
        //                    {
        //                        characteristicName = dicPMSColour[materialGroupID];
        //                    }
        //                }
        //                else if (iColumn == "COMPANY_ADDRESS_OTHER")
        //                {
        //                    characteristicName = "ZPKG_SEC_COMPANY_ADR";
        //                }
        //                else if (iColumn == "PROCESS_COLOUR_OTHER")
        //                {
        //                    if (!String.IsNullOrEmpty(materialGroupID))
        //                    {
        //                        characteristicName = dicProcessColour[materialGroupID];
        //                    }
        //                }
        //                else if (iColumn == "CATCHING_PERIOD_OTHER")
        //                {
        //                    characteristicName = "ZPKG_SEC_CATCHING_METHOD";
        //                }
        //                else if (iColumn == "TOTAL_COLOUR_OTHER")
        //                {
        //                    if (!String.IsNullOrEmpty(materialGroupID))
        //                    {
        //                        characteristicName = dicTotalColour[materialGroupID];
        //                    }
        //                }
        //                else if (iColumn == "CATCHING_METHOD_OTHER")
        //                {
        //                    characteristicName = "ZPKG_SEC_CATCHING_METHOD";
        //                }
        //                else if (iColumn == "STYLE_OF_PRINTING_OTHER")
        //                {
        //                    if (!String.IsNullOrEmpty(materialGroupID))
        //                    {
        //                        characteristicName = dicStyleOfPrinting[materialGroupID];
        //                    }
        //                }
        //                else if (iColumn == "SCIENTIFIC_NAME_OTHER")
        //                {
        //                    characteristicName = "ZPKG_SEC_SCIENTIFIC_NAME";
        //                }
        //                else if (iColumn == "DIRECTION_OF_STICKER_OTHER")
        //                {
        //                    characteristicName = "ZPKG_SEC_DIRECTION";
        //                }
        //                else if (iColumn == "SPECIE_OTHER")
        //                {
        //                    characteristicName = "ZPKG_SEC_SPECIE";
        //                }
        //                else if (iColumn == "DIRECTION_OF_STICKER_OTHER")
        //                {
        //                    characteristicName = "ZPKG_SEC_DIRECTION";
        //                }

        //                if (!String.IsNullOrEmpty(otherValue.ToString()))
        //                {
        //                    if (!String.IsNullOrEmpty(characteristicName))
        //                    {
        //                        modelItem = new InboundArtwork();

        //                        modelItem.ArtworkNumber = artworkObj.ArtworkNumber;
        //                        modelItem.Date = artworkObj.Date;
        //                        modelItem.Time = artworkObj.Time;
        //                        modelItem.Characteristic = characteristicName;
        //                        modelItem.Value = otherValue.ToString();
        //                        modelItem.Description = "";

        //                        listModelItem.Add(modelItem);
        //                    }
        //                }

        //            }
        //            catch (Exception ex)
        //            {

        //            }


        //        }
        //    }




        //    return listModelItem;
        //}

        private static string GetYesNoValue(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return "";
            }
            else
            {
                if (value == "0")
                {
                    return "No";
                }
                else if (value == "1")
                {
                    return "Yes";
                }
                else
                {
                    return "";
                }
            }
        }

        private static string GetApproveValue(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return "";
            }
            else
            {
                if (value == "0")
                {
                    return "Not Approve";
                }
                else if (value == "1")
                {
                    return "Approve";
                }
                else
                {
                    return "";
                }
            }
        }

        private static string ValidateMM65_Header(ArtworkObject _value)
        {
            string msg = "";

            if (String.IsNullOrEmpty(_value.ArtworkNumber))
            {
                msg = "ArtworkNumber";
                return msg;
            }

            if (String.IsNullOrEmpty(_value.Date))
            {
                msg = "Date";
                return msg;
            }

            if (String.IsNullOrEmpty(_value.Time))
            {
                msg = "Time";
                return msg;
            }

            if (String.IsNullOrEmpty(_value.RecordType))
            {
                msg = "Record Type";
                return msg;
            }

            if (String.IsNullOrEmpty(_value.ArtworkURL))
            {
                msg = "Artwork URL";
                return msg;
            }

            if (String.IsNullOrEmpty(_value.PAUserName))
            {
                msg = "PA User Name";
                return msg;
            }

            if (String.IsNullOrEmpty(_value.PGUserName))
            {
                msg = "PG User Name";
                return msg;
            }

            if (String.IsNullOrEmpty(_value.Plant))
            {
                msg = "Plant";
                return msg;
            }

            if (String.IsNullOrEmpty(_value.Plant))
            {
                msg = "Plant";
                return msg;
            }
            return msg;
        }

        private static string ValidateMM65_Item(string _value, MM65_REQUEST param)
        {
            string msg = "";

            if (!_value.Contains("ZPKG_SEC_BRAND"))
            {
                msg = "Brand";
                return msg;
            }

            if (!_value.Contains("ZPKG_SEC_PRIMARY_SIZE"))
            {
                msg = "Primary Size";
                return msg;
            }

            if (param.data.RECORD_TYPE == "I")
            {
                if (!_value.Contains("ZPKG_SEC_CHANGE_POINT"))
                {
                    msg = "Change Point";
                    return msg;
                }
            }

            return msg;
        }

        public static MM65_RESULT ValidateBrandRefMatWithRequestForm(MM65_REQUEST param)
        {
            MM65_RESULT Results = new MM65_RESULT();

            if (param.data.ARTWORK_SUB_ID > 0)
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        string matchBrandTmp = PAFormHelper.CheckBrandRefMaterial_RequestForm(param.data.ARTWORK_SUB_ID, context);
                        matchBrandTmp = matchBrandTmp.Replace(":", "");
                        Results.status = "E";
                        if (!String.IsNullOrEmpty(matchBrandTmp))
                        {
                            Results.status = "S";
                            Results.msg = matchBrandTmp;
                            return Results;
                        }
                        else
                        {
                            Results.status = "S";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Results.status = "E";
                    Results.msg = CNService.GetErrorMessage(ex);
                }
            }


            return Results;
        }
    }
}