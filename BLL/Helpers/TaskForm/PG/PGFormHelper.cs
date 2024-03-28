using BLL.DocumentManagement;
using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace BLL.Helpers
{
    public class PGFormHelper
    {
        public static ART_WF_MOCKUP_PROCESS_PG_RESULT GetPGForm(ART_WF_MOCKUP_PROCESS_PG_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_PG_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var mockUpId = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_SUB_ID = param.data.MOCKUP_SUB_ID }, context).FirstOrDefault().MOCKUP_ID;
                        var checkListId = CNService.ConvertMockupIdToCheckListId(mockUpId, context);

                        var checklistItem = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_ITEM() { CHECK_LIST_ID = checkListId }, context).FirstOrDefault();
                        var checklistPG = ART_WF_MOCKUP_CHECK_LIST_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_PG() { CHECK_LIST_ID = checkListId, MOCKUP_ID = mockUpId }, context).FirstOrDefault();

                        ART_WF_MOCKUP_CHECK_LIST_REQUEST filter = new ART_WF_MOCKUP_CHECK_LIST_REQUEST();
                        filter.data = new ART_WF_MOCKUP_CHECK_LIST_2();
                        filter.data.CHECK_LIST_ID = checkListId;
                        var checkList = CheckListRequestHelper.GetCheckListRequest(filter);
                        checkList.data[0].ITEM = checkList.data[0].ITEM.Where(m => m.MOCKUP_ID == mockUpId).ToList();
                        Results.data = new List<ART_WF_MOCKUP_PROCESS_PG_2>();

                        ART_WF_MOCKUP_CHECK_LIST_PG_2 itemCheckListPG = new ART_WF_MOCKUP_CHECK_LIST_PG_2();
                        if (checklistPG != null)
                        {
                            itemCheckListPG = MapperServices.ART_WF_MOCKUP_CHECK_LIST_PG(checklistPG);

                            itemCheckListPG.MOCKUP_ID = checklistItem.MOCKUP_ID;
                            itemCheckListPG.CHECK_LIST_ID = checklistItem.CHECK_LIST_ID;

                            var listItemPackaingType = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(itemCheckListPG.PACKING_TYPE_ID, context);
                            if (listItemPackaingType != null)
                            {
                                itemCheckListPG.PACKING_TYPE_DISPLAY_TXT = listItemPackaingType.VALUE + ":" + listItemPackaingType.DESCRIPTION;
                            }

                            var listItemNumberOfColor = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(itemCheckListPG.NUMBER_OF_COLOR_ID, context);
                            if (listItemNumberOfColor != null)
                            {
                                itemCheckListPG.NUMBER_OF_COLOR_DISPLAY_TXT = listItemNumberOfColor.DESCRIPTION;
                            }

                            var listItemPrintSystem = SAP_M_CHARACTERISTIC_ITEM_SERVICE.GetByCHARACTERISTIC_ITEM_ID(itemCheckListPG.PRINT_SYSTEM_ID, context);
                            if (listItemPrintSystem != null)
                            {
                                itemCheckListPG.PRINT_SYSTEM_DISPLAY_TXT = listItemPrintSystem.DESCRIPTION;
                            }

                            var listItemBoxColor = SAP_M_CHARACTERISTIC_ITEM_SERVICE.GetByCHARACTERISTIC_ITEM_ID(itemCheckListPG.BOX_COLOR_ID, context);
                            if (listItemBoxColor != null)
                            {
                                itemCheckListPG.BOX_COLOR_DISPLAY_TXT = listItemBoxColor.DESCRIPTION;
                            }

                            var listItemCoating = SAP_M_CHARACTERISTIC_ITEM_SERVICE.GetByCHARACTERISTIC_ITEM_ID(itemCheckListPG.COATING_ID, context);
                            if (listItemCoating != null)
                            {
                                itemCheckListPG.COATING_DISPLAY_TXT = listItemCoating.DESCRIPTION;
                            }

                            var listItemStyle = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(itemCheckListPG.STYLE_ID, context);
                            if (listItemStyle != null)
                            {
                                itemCheckListPG.STYLE_DISPLAY_TXT = listItemStyle.DESCRIPTION;
                            }
                        }

                        var PGData = MapperServices.ART_WF_MOCKUP_PROCESS_PG(ART_WF_MOCKUP_PROCESS_PG_SERVICE.GetByItem(MapperServices.ART_WF_MOCKUP_PROCESS_PG(new ART_WF_MOCKUP_PROCESS_PG_2() { MOCKUP_SUB_ID = param.data.MOCKUP_SUB_ID }), context));

                        foreach (var item in PGData)
                        {
                            if (String.IsNullOrEmpty(param.data.ARTWORK_SUB_ID) && item.DIE_LINE_MOCKUP_ID > 0)
                            {
                                SEARCH_DIE_LINE_REQUEST tempParam = new SEARCH_DIE_LINE_REQUEST();
                                tempParam.data = new SEARCH_DIE_LINE();
                                tempParam.data.DIE_LINE_MOCKUP_ID = Convert.ToInt32(item.DIE_LINE_MOCKUP_ID);
                                var tempDieLineList = SearchDieLineHelper.GetDieLine(tempParam);

                                var tempDieLine = (from p in tempDieLineList.data
                                                   where p.MOCKUP_ID == item.DIE_LINE_MOCKUP_ID
                                                   select p).FirstOrDefault();

                                if (tempDieLine != null)
                                {
                                    item.DIE_LINE = new SEARCH_DIE_LINE();
                                    item.DIE_LINE.CHECK_LIST_ID = tempDieLine.CHECK_LIST_ID;
                                    item.DIE_LINE.MOCKUP_SUB_ID = tempDieLine.MOCKUP_SUB_ID;
                                    item.DIE_LINE.CHECK_LIST_NO = tempDieLine.CHECK_LIST_NO;
                                    item.DIE_LINE.MOCKUP_NO = tempDieLine.MOCKUP_NO;
                                    item.DIE_LINE.MOCKUP_ID = tempDieLine.MOCKUP_ID;
                                    item.DIE_LINE.PRIMARY_TYPE_DISPLAY_TXT = tempDieLine.PRIMARY_TYPE_DISPLAY_TXT;
                                    item.DIE_LINE.PRIMARY_SIZE_DISPLAY_TXT = tempDieLine.PRIMARY_SIZE_DISPLAY_TXT;
                                    item.DIE_LINE.PACK_SIZE_DISPLAY_TXT = tempDieLine.PACK_SIZE_DISPLAY_TXT;
                                    item.DIE_LINE.PACKING_STYLE_DISPLAY_TXT = tempDieLine.PACKING_STYLE_DISPLAY_TXT;
                                    item.DIE_LINE.PACKAGING_TYPE_DISPLAY_TXT = tempDieLine.PACKAGING_TYPE_DISPLAY_TXT;
                                    item.DIE_LINE.VENDOR_DISPLAY_TXT = tempDieLine.VENDOR_DISPLAY_TXT;
                                    item.DIE_LINE.GRADE_OF = tempDieLine.GRADE_OF;
                                    item.DIE_LINE.GRADE_OF_DISPLAY_TXT = tempDieLine.GRADE_OF_DISPLAY_TXT;
                                    item.DIE_LINE.SIZE_DISPLAY_TXT = tempDieLine.SIZE_DISPLAY_TXT;

                                    item.DIE_LINE.FLUTE = tempDieLine.FLUTE;
                                    item.DIE_LINE.FLUTE_DISPLAY_TXT = tempDieLine.FLUTE_DISPLAY_TXT;
                                    item.DIE_LINE.STYLE_DISPLAY_TXT = tempDieLine.STYLE_DISPLAY_TXT;
                                    item.DIE_LINE.STATUS_DISPLAY_TXT = tempDieLine.STATUS_DISPLAY_TXT;
                                    item.DIE_LINE.NUMBER_OF_COLOR_DISPLAY_TXT = tempDieLine.NUMBER_OF_COLOR_DISPLAY_TXT;

                                    item.DIE_LINE.SHEET_SIZE = tempDieLine.SHEET_SIZE;
                                    item.DIE_LINE.ACCESSORIES = tempDieLine.ACCESSORIES;
                                    item.DIE_LINE.PRINT_SYSTEM = tempDieLine.PRINT_SYSTEM;
                                    item.DIE_LINE.DIMENSION_OF = tempDieLine.DIMENSION_OF;
                                    item.DIE_LINE.FINAL_INFO = tempDieLine.FINAL_INFO;
                                    item.DIE_LINE.REMARK_PG = tempDieLine.REMARK_PG;
                                    item.DIE_LINE.ID_MM = tempDieLine.ID_MM;

                                    item.DIE_LINE.DI_CUT = tempDieLine.DI_CUT;
                                    item.DIE_LINE.DI_CUT_DISPLAY_TXT = tempDieLine.DI_CUT_DISPLAY_TXT;

                                    item.DIE_LINE.ROLL_SHEET = tempDieLine.ROLL_SHEET;

                                    item.DIE_LINE.CUSTOMER_SIZE_REMARK = tempDieLine.CUSTOMER_SIZE_REMARK;
                                    item.DIE_LINE.CUSTOMER_SPEC_REMARK = tempDieLine.CUSTOMER_SPEC_REMARK;
                                    item.DIE_LINE.CUSTOMER_NOMINATES_VENDOR_REMARK = tempDieLine.CUSTOMER_NOMINATES_VENDOR_REMARK;

                                    item.DIE_LINE.CUSTOMER_NOMINATES_VENDOR = tempDieLine.CUSTOMER_NOMINATES_VENDOR;
                                    item.DIE_LINE.CUSTOMER_SPEC = tempDieLine.CUSTOMER_SPEC;
                                    item.DIE_LINE.CUSTOMER_SIZE = tempDieLine.CUSTOMER_SIZE;

                                    //---------by aof 20220118 for CR sustain-- - start

                                    item.DIE_LINE.SUSTAIN_MATERIAL = tempDieLine.SUSTAIN_MATERIAL;
                                    item.DIE_LINE.PLASTIC_TYPE = tempDieLine.PLASTIC_TYPE;
                                    item.DIE_LINE.REUSEABLE = tempDieLine.REUSEABLE;
                                    item.DIE_LINE.RECYCLABLE = tempDieLine.RECYCLABLE;
                                    item.DIE_LINE.COMPOSATABLE = tempDieLine.COMPOSATABLE;
                                    item.DIE_LINE.RECYCLE_CONTENT = tempDieLine.RECYCLE_CONTENT;
                                    item.DIE_LINE.CERT = tempDieLine.CERT;
                                    item.DIE_LINE.CERT_SOURCE = tempDieLine.CERT_SOURCE;
                                    item.DIE_LINE.PKG_WEIGHT = tempDieLine.PKG_WEIGHT;
                                    item.DIE_LINE.SUSTAIN_OTHER = tempDieLine.SUSTAIN_OTHER;

                                    item.DIE_LINE.SUSTAIN_MATERIAL_DISPLAY_TXT = tempDieLine.SUSTAIN_MATERIAL_DISPLAY_TXT;
                                    item.DIE_LINE.PLASTIC_TYPE_DISPLAY_TXT = tempDieLine.PLASTIC_TYPE_DISPLAY_TXT;
                                    item.DIE_LINE.CERT_SOURCE_DISPLAY_TXT = tempDieLine.CERT_SOURCE_DISPLAY_TXT;
                                    //---------by aof 20220118 for CR sustain-- - start

                                }
                            }
                            else if (!String.IsNullOrEmpty(param.data.ARTWORK_SUB_ID))
                            {
                                int artwork_sub_id = Convert.ToInt32(param.data.ARTWORK_SUB_ID);
                                var tempProcess2 = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(artwork_sub_id, context);
                                var item2 = tempProcess2.ARTWORK_ITEM_ID;
                                var SEND_PG = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                                int die_line_id = 0;

                                ART_WF_ARTWORK_PROCESS_PG artwork = (from d in context.ART_WF_ARTWORK_PROCESS_PG
                                                                     where d.ARTWORK_SUB_ID == artwork_sub_id
                                                                     select d).FirstOrDefault();

                                if (artwork == null)
                                {
                                    var itemID = (from p in context.ART_WF_ARTWORK_PROCESS
                                                  where p.ARTWORK_SUB_ID == artwork_sub_id
                                                  select p.ARTWORK_ITEM_ID).FirstOrDefault();

                                    var processesPG = (from p in context.ART_WF_ARTWORK_PROCESS
                                                       where p.ARTWORK_ITEM_ID == itemID
                                                        && p.CURRENT_STEP_ID == SEND_PG
                                                       select p.ARTWORK_SUB_ID).ToList();

                                    artwork = (from g in context.ART_WF_ARTWORK_PROCESS_PG
                                               where processesPG.Contains(g.ARTWORK_SUB_ID)
                                                   && (g.ACTION_CODE == "SAVE" || g.ACTION_CODE == "SUBMIT")
                                               select g).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();
                                }

                                if (artwork != null)
                                {
                                    die_line_id = Convert.ToInt32(artwork.DIE_LINE_MOCKUP_ID);
                                    SEARCH_DIE_LINE_REQUEST tempParam = new SEARCH_DIE_LINE_REQUEST();
                                    SEARCH_DIE_LINE_RESULT tempDieLineList = new SEARCH_DIE_LINE_RESULT();

                                    tempParam.data = new SEARCH_DIE_LINE();
                                    tempParam.data.DIE_LINE_MOCKUP_ID = die_line_id;

                                    var step_pg = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_PG").FirstOrDefault();

                                    int subID = Convert.ToInt32(param.data.ARTWORK_SUB_ID);

                                    var stepProcess = (from p in context.ART_WF_ARTWORK_PROCESS
                                                       where p.ARTWORK_SUB_ID == subID
                                                       select p.CURRENT_STEP_ID).FirstOrDefault();

                                    if (stepProcess != step_pg.STEP_ARTWORK_ID)
                                    {
                                        param.data.IS_ONLOAD_PA = "X";
                                    }

                                    if (param.data.IS_ONLOAD_PA == "X")
                                    {
                                        tempParam.data.MOCKUP_SUB_ID = param.data.MOCKUP_SUB_ID;
                                        tempDieLineList = SearchDieLineHelper.GetDieLineDataForPAStep(tempParam);
                                    }
                                    else
                                    {
                                        tempDieLineList = SearchDieLineHelper.GetDieLine(tempParam);
                                    }

                                    var tempDieLine = (from p in tempDieLineList.data
                                                       where p.MOCKUP_ID == artwork.DIE_LINE_MOCKUP_ID
                                                       select p).FirstOrDefault();

                                    if (tempDieLine != null)
                                    {
                                        item.DIE_LINE = new SEARCH_DIE_LINE();
                                        item.DIE_LINE.CHECK_LIST_ID = tempDieLine.CHECK_LIST_ID;
                                        item.DIE_LINE.MOCKUP_SUB_ID = tempDieLine.MOCKUP_SUB_ID;
                                        item.DIE_LINE.CHECK_LIST_NO = tempDieLine.CHECK_LIST_NO;
                                        item.DIE_LINE.MOCKUP_NO = tempDieLine.MOCKUP_NO;
                                        item.DIE_LINE.PRIMARY_TYPE_DISPLAY_TXT = tempDieLine.PRIMARY_TYPE_DISPLAY_TXT;
                                        item.DIE_LINE.PRIMARY_SIZE_DISPLAY_TXT = tempDieLine.PRIMARY_SIZE_DISPLAY_TXT;
                                        item.DIE_LINE.PACK_SIZE_DISPLAY_TXT = tempDieLine.PACK_SIZE_DISPLAY_TXT;
                                        item.DIE_LINE.PACKING_STYLE_DISPLAY_TXT = tempDieLine.PACKING_STYLE_DISPLAY_TXT;
                                        item.DIE_LINE.PACKAGING_TYPE_DISPLAY_TXT = tempDieLine.PACKAGING_TYPE_DISPLAY_TXT;
                                        item.DIE_LINE.VENDOR_DISPLAY_TXT = tempDieLine.VENDOR_DISPLAY_TXT;
                                        item.DIE_LINE.GRADE_OF_DISPLAY_TXT = tempDieLine.GRADE_OF_DISPLAY_TXT;
                                        item.DIE_LINE.SIZE_DISPLAY_TXT = tempDieLine.SIZE_DISPLAY_TXT;
                                        item.DIE_LINE.FLUTE_DISPLAY_TXT = tempDieLine.FLUTE_DISPLAY_TXT;
                                        item.DIE_LINE.STYLE_DISPLAY_TXT = tempDieLine.STYLE_DISPLAY_TXT;
                                        item.DIE_LINE.STATUS_DISPLAY_TXT = tempDieLine.STATUS_DISPLAY_TXT;
                                        item.DIE_LINE.NUMBER_OF_COLOR_DISPLAY_TXT = tempDieLine.NUMBER_OF_COLOR_DISPLAY_TXT;


                                        //---------by aof 20220118 for CR sustain-- - start
                                        item.DIE_LINE.SUSTAIN_MATERIAL = tempDieLine.SUSTAIN_MATERIAL;
                                        item.DIE_LINE.PLASTIC_TYPE = tempDieLine.PLASTIC_TYPE;
                                        item.DIE_LINE.REUSEABLE = tempDieLine.REUSEABLE;
                                        item.DIE_LINE.RECYCLABLE = tempDieLine.RECYCLABLE;
                                        item.DIE_LINE.COMPOSATABLE = tempDieLine.COMPOSATABLE;
                                        item.DIE_LINE.RECYCLE_CONTENT = tempDieLine.RECYCLE_CONTENT;
                                        item.DIE_LINE.CERT = tempDieLine.CERT;
                                        item.DIE_LINE.CERT_SOURCE = tempDieLine.CERT_SOURCE;
                                        item.DIE_LINE.PKG_WEIGHT = tempDieLine.PKG_WEIGHT;
                                        item.DIE_LINE.SUSTAIN_OTHER = tempDieLine.SUSTAIN_OTHER;

                                        item.DIE_LINE.SUSTAIN_MATERIAL_DISPLAY_TXT = tempDieLine.SUSTAIN_MATERIAL_DISPLAY_TXT;
                                        item.DIE_LINE.PLASTIC_TYPE_DISPLAY_TXT = tempDieLine.PLASTIC_TYPE_DISPLAY_TXT;
                                        item.DIE_LINE.CERT_SOURCE_DISPLAY_TXT = tempDieLine.CERT_SOURCE_DISPLAY_TXT;
                                        //---------by aof 20220118 for CR sustain-- - start

                                    }
                                }
                            }

                            var tempProcess = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = mockUpId }, context);
                            tempProcess = tempProcess.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                            var process = (from p in tempProcess where p.PARENT_MOCKUP_SUB_ID == null select p).FirstOrDefault();

                            if (item.PG_USER_ID != null)
                            {
                                item.PG_USER_DISPLAY_TXT = CNService.GetUserName(item.PG_USER_ID, context);
                            }
                            if (String.IsNullOrEmpty(item.PG_USER_DISPLAY_TXT) && process != null)
                            {
                                item.PG_USER_DISPLAY_TXT = CNService.GetUserName(process.CURRENT_USER_ID, context);
                            }
                            if (item.GRADE_OF != null)
                            {
                                var GradeOf = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.GRADE_OF, context);
                                if (GradeOf != null)
                                {
                                    item.GRADE_OF_DISPLAY_TXT = GradeOf.DESCRIPTION;
                                }
                            }

                            if (item.DI_CUT != null)
                            {
                                var DiCut = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.DI_CUT, context);
                                if (DiCut != null)
                                {
                                    item.DI_CUT_DISPLAY_TXT = DiCut.DESCRIPTION;
                                }
                            }

                            if (item.VENDOR != null)
                            {
                                var Vendor = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(item.VENDOR, context);
                                if (Vendor != null)
                                {
                                    item.VENDOR_DISPLAY_TXT = Vendor.VENDOR_CODE + ":" + Vendor.VENDOR_NAME;
                                }
                            }

                            if (item.FLUTE != null)
                            {
                                var Flute = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.FLUTE, context);
                                if (Flute != null)
                                {
                                    item.FLUTE_DISPLAY_TXT = Flute.DESCRIPTION;
                                }
                            }

                            var tempProcessTemplate_pg = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG() { MOCKUP_ID = mockUpId }, context);
                            var processTemplate_pg = (from p in tempProcessTemplate_pg orderby p.UPDATE_DATE descending select p).FirstOrDefault();

                            if (processTemplate_pg != null)
                            {
                                if (!string.IsNullOrEmpty(processTemplate_pg.STYLE_OF_PRINTING_OTHER))
                                {
                                    item.STYLE_OF_PRINTING_DISPLAY_TXT = processTemplate_pg.STYLE_OF_PRINTING_OTHER;
                                }
                                else if (processTemplate_pg.STYLE_OF_PRINTING > 0)
                                {
                                    var characteristic = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(processTemplate_pg.STYLE_OF_PRINTING, context);
                                    if (characteristic != null)
                                    {
                                        item.STYLE_OF_PRINTING_DISPLAY_TXT = characteristic.DESCRIPTION;
                                    }
                                }
                            }

                            //---------by aof 20220118 for CR sustain-- - start
                            if (item.SUSTAIN_MATERIAL != null)
                            {
                                var SUSTAIN_MATERIAL = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.SUSTAIN_MATERIAL, context);
                                if (SUSTAIN_MATERIAL != null)
                                {
                                    item.SUSTAIN_MATERIAL_DISPLAY_TXT  = SUSTAIN_MATERIAL.DESCRIPTION;
                                }
                            }

                            if (item.PLASTIC_TYPE != null)
                            {
                                var PLASTIC_TYPE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PLASTIC_TYPE, context);
                                if (PLASTIC_TYPE != null)
                                {
                                    item.PLASTIC_TYPE_DISPLAY_TXT = PLASTIC_TYPE.DESCRIPTION;
                                }
                            }

                            if (item.CERT_SOURCE != null)
                            {
                                var CERT_SOURCE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.CERT_SOURCE, context);
                                if (CERT_SOURCE != null)
                                {
                                    item.CERT_SOURCE_DISPLAY_TXT = CERT_SOURCE.DESCRIPTION;
                                }
                            }
                            //---------by aof 20220118 for CR sustain-- - start

                        }

                        if (PGData.Count == 0)
                        {
                            var temp = new ART_WF_MOCKUP_PROCESS_PG_2();
                            Results.data.Add(temp);
                        }
                        else
                        {
                            Results.data = PGData;
                        }

                        Results.data[0].CHECKLIST_PG = itemCheckListPG;
                        Results.data[0].CHECKLIST = checkList.data[0];
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

        public static ART_WF_MOCKUP_PROCESS_PG_RESULT SavePGForm(ART_WF_MOCKUP_PROCESS_PG_REQUEST Item)
        {
            ART_WF_MOCKUP_PROCESS_PG_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_RESULT();
            List<ART_WF_MOCKUP_PROCESS_PG_2> listPG_2 = new List<ART_WF_MOCKUP_PROCESS_PG_2>();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        ART_WF_MOCKUP_PROCESS_PG_2 item_2 = SavePGProcess(Item, context);

                        if (Item.data.CHECKLIST_PG != null)
                        {
                            ART_WF_MOCKUP_CHECK_LIST_PG itemPG = new ART_WF_MOCKUP_CHECK_LIST_PG();
                            itemPG = Item.data.CHECKLIST_PG;

                            ART_WF_MOCKUP_CHECK_LIST_PG_SERVICE.SaveOrUpdate(itemPG, context);

                            item_2.CHECKLIST_PG = itemPG;
                        }

                        listPG_2.Add(item_2);

                        Results.data = listPG_2;

                        if (Item.data.CHECKLIST_PG.MOCKUP_ID > 0)
                        {
                            var mockupNo = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(Item.data.CHECKLIST_PG.MOCKUP_ID, context).MOCKUP_NO;
                            if (!string.IsNullOrEmpty(mockupNo))
                            {
                                var token = CWSService.getAuthToken();
                                CWSService.updateCategoryMockup(mockupNo, Item.data.CHECKLIST_PG.MOCKUP_ID, token);
                            }
                        }

                        dbContextTransaction.Commit();

                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_001", context);
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

        public static ART_WF_MOCKUP_PROCESS_PG_2 SavePGProcess(ART_WF_MOCKUP_PROCESS_PG_REQUEST Item, ARTWORKEntities context)
        {
            ART_WF_MOCKUP_PROCESS_PG pgData = new ART_WF_MOCKUP_PROCESS_PG();
            pgData = MapperServices.ART_WF_MOCKUP_PROCESS_PG(Item.data);

            var chkPG = ART_WF_MOCKUP_PROCESS_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG() { MOCKUP_SUB_ID = Item.data.MOCKUP_SUB_ID }, context).FirstOrDefault();
            if (chkPG != null)
                pgData.VENDOR = chkPG.VENDOR;
            ART_WF_MOCKUP_PROCESS_PG_SERVICE.SaveOrUpdate(pgData, context);

            ART_WF_MOCKUP_PROCESS_PG_2 item = new ART_WF_MOCKUP_PROCESS_PG_2();
            item.MOCKUP_SUB_PG_ID = pgData.MOCKUP_SUB_PG_ID;
            return item;
        }

        public static ART_WF_ARTWORK_PROCESS_PG_RESULT GetSendToPGInfo(ART_WF_ARTWORK_PROCESS_PG_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PG_RESULT Results = new ART_WF_ARTWORK_PROCESS_PG_RESULT();
            List<ART_WF_ARTWORK_PROCESS_PG_2> listPG = new List<ART_WF_ARTWORK_PROCESS_PG_2>();
            List<PG_HISTORY> listPGHistory = new List<PG_HISTORY>();

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var tempProcess = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);
                    var isMain = false;
                    if (tempProcess.PARENT_ARTWORK_SUB_ID == null)
                    {
                        isMain = true;
                    }

                    var SEND_PG = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                    var requestItem = (from i in context.ART_WF_ARTWORK_PROCESS
                                       where i.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                       select i.ARTWORK_ITEM_ID).FirstOrDefault();

                    var processesPG = (from p in context.ART_WF_ARTWORK_PROCESS
                                       where p.ARTWORK_ITEM_ID == requestItem
                                        && p.CURRENT_STEP_ID == SEND_PG
                                       select p.ARTWORK_SUB_ID).ToList();

                    ART_WF_ARTWORK_PROCESS_PG lastPGSubmit = (from g in context.ART_WF_ARTWORK_PROCESS_PG
                                                              where processesPG.Contains(g.ARTWORK_SUB_ID)
                                                                  && (g.ACTION_CODE == "SAVE" || g.ACTION_CODE == "SUBMIT")
                                                              select g).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                    if (isMain)
                    {
                        lastPGSubmit = (from g in context.ART_WF_ARTWORK_PROCESS_PG
                                        where processesPG.Contains(g.ARTWORK_SUB_ID)
                                            && g.ACTION_CODE == "SUBMIT"
                                        select g).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();
                    }

                    try
                    {
                        if (lastPGSubmit != null)
                        {
                            param.data.ARTWORK_SUB_ID = lastPGSubmit.ARTWORK_SUB_ID;
                            listPG = MapperServices.ART_WF_ARTWORK_PROCESS_PG(ART_WF_ARTWORK_PROCESS_PG_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_PROCESS_PG(param.data), context)).OrderByDescending(o => o.UPDATE_DATE).ToList();

                            if (listPG.Count <= 0)
                            {
                                ART_WF_ARTWORK_PROCESS_PG_2 pgTmp = new ART_WF_ARTWORK_PROCESS_PG_2();
                                pgTmp.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                pgTmp.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;

                                listPG.Add(pgTmp);
                            }

                            ART_WF_ARTWORK_PROCESS_PA pa = new ART_WF_ARTWORK_PROCESS_PA();
                            ART_WF_ARTWORK_REQUEST req = new ART_WF_ARTWORK_REQUEST();

                            var stepSendPG_ID = context.ART_M_STEP_MOCKUP.Where(w => w.STEP_MOCKUP_CODE == "SEND_PG").Select(s => s.STEP_MOCKUP_ID).FirstOrDefault();    // by aof append order by rewrite 20230322 
                            foreach (ART_WF_ARTWORK_PROCESS_PG_2 itemPG in listPG)
                            {
                                listPGHistory = new List<PG_HISTORY>();
                                ART_WF_ARTWORK_PROCESS_PA paTmp = new ART_WF_ARTWORK_PROCESS_PA();
                                paTmp.ARTWORK_SUB_ID = itemPG.ARTWORK_SUB_ID;
                                pa = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(paTmp, context).FirstOrDefault();

                                
                                if (itemPG.DIE_LINE_MOCKUP_ID != null)
                                {
                                    // by aof append order by commeted 20230322
                                    ////var mockup = MapperServices.ART_WF_MOCKUP_PROCESS(ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS()
                                    ////{ MOCKUP_ID = Convert.ToInt32(itemPG.DIE_LINE_MOCKUP_ID) }, context).FirstOrDefault());  

                                    // by aof append order by rewrite 20230322
                                    var mockup = context.ART_WF_MOCKUP_PROCESS.Where(s => s.MOCKUP_ID == itemPG.DIE_LINE_MOCKUP_ID && s.CURRENT_STEP_ID == stepSendPG_ID).FirstOrDefault();
                                  
                                    itemPG.DIE_LINE_MOCKUP_SUB_ID = mockup.MOCKUP_SUB_ID; 
                                }

                                req = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(itemPG.ARTWORK_REQUEST_ID, context);

                                if (req != null)
                                {
                                    itemPG.PRIMARY_TYPE_ID = req.PRIMARY_TYPE_ID;

                                    if (req.PRIMARY_TYPE_ID != null)
                                    {
                                        itemPG.PRIMARY_TYPE_DISPLAY_TXT = CNService.GetCharacteristicDescription(req.PRIMARY_TYPE_ID, context);
                                    }
                                }

                                if (pa != null)
                                {
                                    itemPG.PRIMARY_SIZE_ID = pa.PRIMARY_SIZE_ID;
                                    itemPG.PACK_SIZE_ID = pa.PACK_SIZE_ID;
                                    itemPG.PACKING_STYLE_ID = pa.PACKING_STYLE_ID;
                                    itemPG.PACKAGING_TYPE_ID = pa.MATERIAL_GROUP_ID;
                                    if (pa.MATERIAL_GROUP_ID != null)
                                    {
                                        itemPG.PACKAGING_TYPE_DISPLAY_TXT = CNService.GetCharacteristicDescription(pa.MATERIAL_GROUP_ID, context);
                                    }
                                    var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                                    var list = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID, CURRENT_STEP_ID = stepId }, context).ToList();

                                    if (list != null && list.Count > 0)
                                    {
                                        //Get PG Process History
                                        PG_HISTORY pgHistoryObj = new PG_HISTORY();
                                        ART_SYS_ACTION act = new ART_SYS_ACTION();
                                        ART_WF_ARTWORK_PROCESS_PG_2 pgHistory = new ART_WF_ARTWORK_PROCESS_PG_2();
                                        ART_WF_ARTWORK_PROCESS_PG_BY_PA_2 pgByPAHistory = new ART_WF_ARTWORK_PROCESS_PG_BY_PA_2();

                                        int row = 0;
                                        foreach (ART_WF_ARTWORK_PROCESS iProcess in list)
                                        {
                                            pgHistoryObj = new PG_HISTORY();
                                            pgHistory = MapperServices.ART_WF_ARTWORK_PROCESS_PG(ART_WF_ARTWORK_PROCESS_PG_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PG()
                                            { ARTWORK_SUB_ID = iProcess.ARTWORK_SUB_ID }, context).FirstOrDefault());

                                            pgByPAHistory = MapperServices.ART_WF_ARTWORK_PROCESS_PG_BY_PA(ART_WF_ARTWORK_PROCESS_PG_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PG_BY_PA()
                                            { ARTWORK_SUB_ID = iProcess.ARTWORK_SUB_ID }, context).FirstOrDefault());
                                            act = new ART_SYS_ACTION();

                                            if (pgHistory != null)
                                            {
                                                act.ACTION_CODE = pgHistory.ACTION_CODE;
                                                pgHistoryObj.ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItemContain(act, context).FirstOrDefault().ACTION_NAME;
                                                pgHistoryObj.COMMENT_BY_PG = pgHistory.COMMENT;
                                                pgHistoryObj.CREATE_DATE_BY_PG = pgHistory.CREATE_DATE;

                                                if (pgHistory.REASON_ID != null)
                                                {
                                                    pgHistoryObj.REASON_BY_PG = ART_M_DECISION_REASON_SERVICE.GetByART_M_DECISION_REASON_ID(pgHistory.REASON_ID, context).DESCRIPTION;
                                                }
                                            }

                                            row += 1;
                                            pgHistoryObj.NO = row.ToString();
                                            pgHistoryObj.REASON_BY_PA = CNService.getReason(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(pgByPAHistory.ARTWORK_SUB_ID, context).REASON_ID, context);
                                            pgHistoryObj.COMMENT_BY_PA = iProcess.REMARK;
                                            pgHistoryObj.CREATE_DATE_BY_PA = pgByPAHistory.CREATE_DATE;

                                            if (pgByPAHistory.REASON_ID != null)
                                            {

                                            }
                                            listPGHistory.Add(pgHistoryObj);
                                        }

                                        itemPG.HISTORIES = listPGHistory;
                                    }
                                }
                            }

                            Results.data = listPG;
                        }
                        Results.status = "S";
                    }
                    catch (Exception ex)
                    {
                        Results.status = "E";
                        Results.msg = CNService.GetErrorMessage(ex);
                    }
                }
            }
            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_PG_RESULT SavePGInfo(ART_WF_ARTWORK_PROCESS_PG_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PG_RESULT Results = new ART_WF_ARTWORK_PROCESS_PG_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var pgData = MapperServices.ART_WF_ARTWORK_PROCESS_PG(param.data);

                        var check = ART_WF_ARTWORK_PROCESS_PG_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PG() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                        if (check.Count > 0)
                        {
                            pgData.ARTWORK_SUB_PA_ID = check.FirstOrDefault().ARTWORK_SUB_PA_ID;
                        }

                        pgData.ACTION_CODE = "SAVE";
                        ART_WF_ARTWORK_PROCESS_PG_SERVICE.SaveOrUpdate(pgData, context);

                        if (param.data.UPDATE_BY > 0)
                        {
                            ART_WF_ARTWORK_PROCESS_PA pa = new ART_WF_ARTWORK_PROCESS_PA();
                            ART_WF_ARTWORK_PROCESS process = new ART_WF_ARTWORK_PROCESS();
                            pa.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;

                            pa = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(pa, context).FirstOrDefault();
                            process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);

                            if (pa != null)
                            {
                                pa.PG_USER_ID = param.data.UPDATE_BY;
                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(pa, context);
                            }
                            else if (process != null)
                            {
                                pa = new ART_WF_ARTWORK_PROCESS_PA();
                                pa.ARTWORK_SUB_ID = Convert.ToInt32(process.PARENT_ARTWORK_SUB_ID);

                                pa = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(pa, context).FirstOrDefault();

                                if (pa != null)
                                {
                                    pa.PG_USER_ID = param.data.UPDATE_BY;
                                    ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(pa, context);
                                }
                            }
                        }


                        dbContextTransaction.Commit();


                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_001", context);
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

        public static ART_WF_ARTWORK_PROCESS_PG_RESULT SaveSendToPGInfo(ART_WF_ARTWORK_PROCESS_PG_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PG_RESULT Results = new ART_WF_ARTWORK_PROCESS_PG_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var pgData = MapperServices.ART_WF_ARTWORK_PROCESS_PG(param.data);

                        var check = ART_WF_ARTWORK_PROCESS_PG_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PG() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                        if (check.Count > 0)
                            pgData.ARTWORK_SUB_PA_ID = check.FirstOrDefault().ARTWORK_SUB_PA_ID;

                        ART_WF_ARTWORK_PROCESS_PG_SERVICE.SaveOrUpdate(pgData, context);

                        if (param.data.ENDTASKFORM)
                            ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);

                        SavePGUser(param, context, pgData);

                        dbContextTransaction.Commit();

                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_001", context);
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

        private static void SavePGUser(ART_WF_ARTWORK_PROCESS_PG_REQUEST param, ARTWORKEntities context, ART_WF_ARTWORK_PROCESS_PG pgData)
        {
            //Update PG Username in Process PA
            ART_WF_ARTWORK_PROCESS_PA processPA = new ART_WF_ARTWORK_PROCESS_PA();
            processPA.ARTWORK_SUB_ID = pgData.ARTWORK_SUB_ID;
            processPA = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(processPA, context).FirstOrDefault();

            if (processPA != null)
            {
                processPA.PG_USER_ID = param.data.UPDATE_BY;
                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);
            }
            else
            {
                if (param.data.ARTWORK_SUB_ID > 0)
                {
                    processPA.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                    processPA.PG_USER_ID = param.data.UPDATE_BY;
                    ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);
                }
            }
        }


        //---------------------------------------------------ticket# 473360 by aof ----------------------------------------------------------

        public static ART_WF_ARTWORK_PROCESS_PG_RESULT CheckDielineFileToArtwork(ART_WF_ARTWORK_PROCESS_PG_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PG_RESULT Results = new ART_WF_ARTWORK_PROCESS_PG_RESULT();

            if (param != null || param.data != null || param.data.DIE_LINE_MOCKUP_ID != null)
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        Results = CheckDielineFileToArtwork(param, context);
                    }

                }
                catch (Exception ex)
                {
                    Results.status = "E";
                    Results.msg = CNService.GetErrorMessage(ex);

                    return Results;
                }
            }

            Results.status = "S";

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_PG_RESULT CheckDielineFileToArtwork(ART_WF_ARTWORK_PROCESS_PG_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_PROCESS_PG_RESULT Results = new ART_WF_ARTWORK_PROCESS_PG_RESULT();

            if (param != null || param.data != null || param.data.DIE_LINE_MOCKUP_ID != null)
            {
                var process = (from p in context.ART_WF_ARTWORK_PROCESS
                               where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                               select p).FirstOrDefault();

                var requestItem = (from r in context.ART_WF_ARTWORK_REQUEST_ITEM
                                   where r.ARTWORK_ITEM_ID == process.ARTWORK_ITEM_ID
                                   select r).FirstOrDefault();

                int itemID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context);

                var stepPG = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_PG").FirstOrDefault();

                var processPGSubIDs = (from p in context.ART_WF_ARTWORK_PROCESS
                                       where p.ARTWORK_ITEM_ID == itemID
                                          && p.CURRENT_STEP_ID == stepPG.STEP_ARTWORK_ID
                                       select p.ARTWORK_SUB_ID).ToList();

                var processPGData = (from g in context.ART_WF_ARTWORK_PROCESS_PG
                                     where processPGSubIDs.Contains(g.ARTWORK_SUB_ID)
                                        && g.DIE_LINE_MOCKUP_ID != null
                                     select g).OrderByDescending(o => o.ARTWORK_SUB_PA_ID).FirstOrDefault();

                ART_WF_MOCKUP_CHECK_LIST_ITEM mockup = new ART_WF_MOCKUP_CHECK_LIST_ITEM();

                if (processPGData != null)
                {
                    mockup = (from m in context.ART_WF_MOCKUP_CHECK_LIST_ITEM
                              where m.MOCKUP_ID == processPGData.DIE_LINE_MOCKUP_ID
                              select m).FirstOrDefault();
                }

                Node[] nodeSPMOFiles_Empty = new Node[0];

                var MO_ParentFolder = ConfigurationManager.AppSettings["MockUpNodeID"];
                var MO_folderDieline = ConfigurationManager.AppSettings["MockupFolderNameDieline"];

                var AW_ParentFolder = ConfigurationManager.AppSettings["ArtworkNodeID"];
                var AW_folderDieline = ConfigurationManager.AppSettings["ArtworkFolderNameDieline"];
                var AW_folderOther = ConfigurationManager.AppSettings["ArtworkFolderNameOther"];

                Node nodeParentSPMO = new Node();
                var token = CWSService.getAuthToken();
                Node nodeParentAWDieline = new Node();
                Node nodeParentAW = CWSService.getNodeByName(Convert.ToInt64(AW_ParentFolder), requestItem.REQUEST_ITEM_NO, token);

                if (nodeParentAW != null)
                {
                    nodeParentAWDieline = CWSService.getNodeByName(nodeParentAW.ID, AW_folderDieline, token);

                    if (nodeParentAWDieline == null) // if folder 40 - Die Line not exist , create folder.
                    {
                        // nodeParentAWDieline = CWSService.getNodeByName(nodeParentAW.ID, AW_folderOther);
                       // nodeParentAWDieline = CWSService.createFolder(nodeParentAW.ID, AW_folderDieline, "", token);
                    }
                }

                if (mockup != null && !String.IsNullOrEmpty(mockup.MOCKUP_NO))
                {
                    nodeParentSPMO = CWSService.getNodeByName(Convert.ToInt64(MO_ParentFolder), mockup.MOCKUP_NO, token);
                }

                if (nodeParentSPMO != null && nodeParentSPMO.ID > 0)
                {
                    Node nodeMO = CWSService.getNodeByName(nodeParentSPMO.ID, MO_folderDieline, token);

                    if (nodeMO != null)
                    {
                        Node[] nodeSPMOFiles = CWSService.getAllNodeInFolder(nodeMO.ID, token);

                        if (nodeSPMOFiles != null && nodeSPMOFiles.Count() > 0)
                        {
                            if (nodeParentAWDieline != null)
                            {
                                Node[] nodeAWDielineExistFiles = CWSService.getAllNodeInFolder(nodeParentAWDieline.ID, token);
                           }
                            else
                            {
                                Results.status = "S";
                                Results.msg = MessageHelper.GetMessage("MSG_027", context);

                                return Results;
                            }
                        }
                    }
                    else
                    {
                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_027", context);

                        return Results;
                    }
                }
                else
                {
                    Results.status = "S";
                    Results.msg = MessageHelper.GetMessage("MSG_027", context);

                    return Results;
                }
            }

            Results.status = "S";

            return Results;
        }

        //---------------------------------------------------ticket# 473360 by aof ----------------------------------------------------------



        public static ART_WF_ARTWORK_PROCESS_PG_RESULT CopyDielineFileToArtwork(ART_WF_ARTWORK_PROCESS_PG_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PG_RESULT Results = new ART_WF_ARTWORK_PROCESS_PG_RESULT();

            if (param != null || param.data != null || param.data.DIE_LINE_MOCKUP_ID != null)
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        Results = CopyDielineFileToArtwork(param, context);
                    }

                }
                catch (Exception ex)
                {
                    Results.status = "E";
                    Results.msg = CNService.GetErrorMessage(ex);

                    return Results;
                }
            }

            Results.status = "S";

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_PG_RESULT CopyDielineFileToArtwork(ART_WF_ARTWORK_PROCESS_PG_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_PROCESS_PG_RESULT Results = new ART_WF_ARTWORK_PROCESS_PG_RESULT();

            if (param != null || param.data != null || param.data.DIE_LINE_MOCKUP_ID != null)
            {
                var process = (from p in context.ART_WF_ARTWORK_PROCESS
                               where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                               select p).FirstOrDefault();

                var requestItem = (from r in context.ART_WF_ARTWORK_REQUEST_ITEM
                                   where r.ARTWORK_ITEM_ID == process.ARTWORK_ITEM_ID
                                   select r).FirstOrDefault();

                int itemID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context);

                var stepPG = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_PG").FirstOrDefault();

                var processPGSubIDs = (from p in context.ART_WF_ARTWORK_PROCESS
                                       where p.ARTWORK_ITEM_ID == itemID
                                          && p.CURRENT_STEP_ID == stepPG.STEP_ARTWORK_ID
                                       select p.ARTWORK_SUB_ID).ToList();

                var processPGData = (from g in context.ART_WF_ARTWORK_PROCESS_PG
                                     where processPGSubIDs.Contains(g.ARTWORK_SUB_ID)
                                        && g.DIE_LINE_MOCKUP_ID != null
                                     select g).OrderByDescending(o => o.ARTWORK_SUB_PA_ID).FirstOrDefault();

                ART_WF_MOCKUP_CHECK_LIST_ITEM mockup = new ART_WF_MOCKUP_CHECK_LIST_ITEM();

                if (processPGData != null)
                {
                    mockup = (from m in context.ART_WF_MOCKUP_CHECK_LIST_ITEM
                              where m.MOCKUP_ID == processPGData.DIE_LINE_MOCKUP_ID
                              select m).FirstOrDefault();
                }

                Node[] nodeSPMOFiles_Empty = new Node[0];

                var MO_ParentFolder = ConfigurationManager.AppSettings["MockUpNodeID"];
                var MO_folderDieline = ConfigurationManager.AppSettings["MockupFolderNameDieline"];

                var AW_ParentFolder = ConfigurationManager.AppSettings["ArtworkNodeID"];
                var AW_folderDieline = ConfigurationManager.AppSettings["ArtworkFolderNameDieline"];
                var AW_folderOther = ConfigurationManager.AppSettings["ArtworkFolderNameOther"];

                Node nodeParentSPMO = new Node();
                var token = CWSService.getAuthToken();
                Node nodeParentAWDieline = new Node();
                Node nodeParentAW = CWSService.getNodeByName(Convert.ToInt64(AW_ParentFolder), requestItem.REQUEST_ITEM_NO, token);

                if (nodeParentAW != null)
                {
                    nodeParentAWDieline = CWSService.getNodeByName(nodeParentAW.ID, AW_folderDieline, token);

                    if (nodeParentAWDieline == null) // if folder 40 - Die Line not exist , create folder.
                    {
                        // nodeParentAWDieline = CWSService.getNodeByName(nodeParentAW.ID, AW_folderOther);
                        nodeParentAWDieline = CWSService.createFolder(nodeParentAW.ID, AW_folderDieline, "", token);
                    }
                }

                if (mockup != null && !String.IsNullOrEmpty(mockup.MOCKUP_NO))
                {
                    nodeParentSPMO = CWSService.getNodeByName(Convert.ToInt64(MO_ParentFolder), mockup.MOCKUP_NO, token);
                }

                if (nodeParentSPMO != null && nodeParentSPMO.ID > 0)
                {
                    Node nodeMO = CWSService.getNodeByName(nodeParentSPMO.ID, MO_folderDieline, token);

                    if (nodeMO != null)
                    {
                        Node[] nodeSPMOFiles = CWSService.getAllNodeInFolder(nodeMO.ID, token);

                        if (nodeSPMOFiles != null && nodeSPMOFiles.Count() > 0)
                        {
                            if (nodeParentAWDieline != null)
                            {
                                Node[] nodeAWDielineExistFiles = CWSService.getAllNodeInFolder(nodeParentAWDieline.ID, token);

                                //Get exist dieline files in AW 
                                if (process.CURRENT_STEP_ID == stepPG.STEP_ARTWORK_ID)
                                {
                                    if (nodeAWDielineExistFiles != null)
                                    {
                                        ART_WF_ARTWORK_ATTACHMENT attachment = new ART_WF_ARTWORK_ATTACHMENT();
                                        List<ART_WF_ARTWORK_ATTACHMENT> listAttachments = new List<ART_WF_ARTWORK_ATTACHMENT>();

                                        foreach (Node iExistNode in nodeAWDielineExistFiles)
                                        {
                                            attachment = new ART_WF_ARTWORK_ATTACHMENT();
                                            listAttachments = new List<ART_WF_ARTWORK_ATTACHMENT>();

                                            attachment.NODE_ID = iExistNode.ID;
                                            listAttachments = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(attachment, context).ToList();
                                            if (listAttachments != null)
                                            {
                                                foreach (ART_WF_ARTWORK_ATTACHMENT iAttach in listAttachments)
                                                {
                                                    ART_WF_ARTWORK_ATTACHMENT_SERVICE.DeleteByARTWORK_ATTACHMENT_ID(iAttach.ARTWORK_ATTACHMENT_ID, context);
                                                }
                                            }

                                            CWSService.deleteNode(iExistNode.ID, token);
                                        }
                                    }

                                    foreach (Node iNodeMO in nodeSPMOFiles)
                                    {
                                        var newNode = CWSService.copyNode(iNodeMO.Name, iNodeMO.ID, nodeParentAWDieline.ID, token);

                                        CopyDielineToAttachment(context, process, processPGData, newNode);
                                    }
                                }

                                nodeAWDielineExistFiles = CWSService.getAllNodeInFolder(nodeParentAWDieline.ID, token);
                                if (nodeAWDielineExistFiles == null)
                                {
                                    foreach (Node iNodeMO in nodeSPMOFiles)
                                    {
                                        var newNode = CWSService.copyNode(iNodeMO.Name, iNodeMO.ID, nodeParentAWDieline.ID, token);

                                        CopyDielineToAttachment(context, process, processPGData, newNode);
                                    }
                                }
                            }
                            else
                            {
                                Results.status = "S";
                                Results.msg = MessageHelper.GetMessage("MSG_027", context);

                                return Results;
                            }
                        }
                    }
                    else
                    {
                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_027", context);

                        return Results;
                    }
                }
                else
                {
                    Results.status = "S";
                    Results.msg = MessageHelper.GetMessage("MSG_027", context);

                    return Results;
                }
            }

            Results.status = "S";

            return Results;
        }

        private static void CopyDielineToAttachment(ARTWORKEntities context, ART_WF_ARTWORK_PROCESS process, ART_WF_ARTWORK_PROCESS_PG processPG, Node nodeFile)
        {
            var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_ARTWORK_ID;

            string EXTENSION = Path.GetExtension(nodeFile.Name);
            string CONTENT_TYPE = nodeFile.VersionInfo.MimeType;

            EXTENSION = EXTENSION.Replace(".", "");


            // ticket464524 by aof
            string filename = nodeFile.Name;
            long? filesize = nodeFile.VersionInfo.FileDataSize;
            string version2 = "1.0";

            if (nodeFile.VersionInfo.Versions != null)
            {
                if (nodeFile.VersionInfo.Versions.Count() > 1 )
                {
                    var index = nodeFile.VersionInfo.Versions.Count()-1;
                    filename = nodeFile.VersionInfo.Versions[index].Filename;
                    filesize = nodeFile.VersionInfo.Versions[index].FileDataSize;

                    var token = CWSService.getAuthToken();
                    CWSService.renameFolder(nodeFile.ID, filename, token);
                }
            }

            if (processPG.DIE_LINE_MOCKUP_ID > 0)
            {
                var version2_mockup = (from m in context.ART_WF_MOCKUP_ATTACHMENT
                         where m.MOCKUP_ID == processPG.DIE_LINE_MOCKUP_ID
                         orderby m.MOCKUP_ATTACHMENT_ID descending
                         select m.VERSION2).FirstOrDefault();
                if (version2_mockup != null)
                {
                    version2 = version2_mockup;
                }
            }
         
            //ticket464524 by aof



            ART_WF_ARTWORK_ATTACHMENT attach = new ART_WF_ARTWORK_ATTACHMENT();
            attach.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
            attach.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
            attach.CONTENT_TYPE = CONTENT_TYPE;
            attach.CREATE_BY = processPG.CREATE_BY;
            attach.UPDATE_BY = processPG.CREATE_BY;
            attach.EXTENSION = EXTENSION;
            attach.FILE_NAME = filename; //nodeFile.Name;    // ticket464524 by aof
            attach.IS_CUSTOMER = "";
            attach.IS_INTERNAL = "X";
            attach.IS_VENDOR = "X";
            attach.NODE_ID = nodeFile.ID;
            attach.SIZE = filesize; //nodeFile.VersionInfo.FileDataSize;   // ticket464524 by aof
            attach.STEP_ARTWORK_ID = stepId;
            attach.VERSION = nodeFile.VersionInfo.VersionNum; // attach.VERSION = 1;  //ticket#461843 by aof
            attach.VERSION2 = version2; //"1.0"; //ticket#461843 by aof



            ART_WF_ARTWORK_ATTACHMENT_SERVICE.SaveOrUpdate(attach, context);
        }
    }
}
