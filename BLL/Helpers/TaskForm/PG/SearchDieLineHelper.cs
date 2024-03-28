using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class SearchDieLineHelper
    {
        public static SEARCH_DIE_LINE_RESULT GetDieLine(SEARCH_DIE_LINE_REQUEST param)
        {
            var listOfCheckListItem = new List<ART_WF_MOCKUP_CHECK_LIST_ITEM>();
            SEARCH_DIE_LINE_RESULT Results = new SEARCH_DIE_LINE_RESULT();
            try
            {
                List<ART_WF_MOCKUP_CHECK_LIST_2> checkList2 = new List<ART_WF_MOCKUP_CHECK_LIST_2>();
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var PRIMARY_SIZE_TXT = "";
                        var PACK_SIZE_TXT = "";

                        if (param.data.PRIMARY_SIZE_ID > 0) PRIMARY_SIZE_TXT = SAP_M_3P_SERVICE.GetByTHREE_P_ID(param.data.PRIMARY_SIZE_ID, context).PRIMARY_SIZE_VALUE;
                        if (param.data.PACK_SIZE_ID > 0) PACK_SIZE_TXT = SAP_M_2P_SERVICE.GetByTWO_P_ID(param.data.PACK_SIZE_ID, context).PACK_SIZE_VALUE;

                        var PACKING_STYLE_DISPLAY_TXT = param.data.PACKING_STYLE_DISPLAY_TXT;
                        var PRIMARY_SIZE_DISPLAY_TXT = param.data.PRIMARY_SIZE_DISPLAY_TXT;

                        var processes = (from p in context.ART_WF_MOCKUP_PROCESS
                                         where p.PARENT_MOCKUP_SUB_ID == null
                                         && (String.IsNullOrEmpty(p.IS_TERMINATE))
                                         && (String.IsNullOrEmpty(p.REMARK_KILLPROCESS))
                                         select p.MOCKUP_ID).Distinct().ToList();

                        var checklist = (from ci in context.ART_WF_MOCKUP_CHECK_LIST_ITEM
                                         where processes.Contains(ci.MOCKUP_ID)
                                         select ci.CHECK_LIST_ID).Distinct().ToList();

                        if (param.data.DIE_LINE_MOCKUP_ID > 0)
                        {
                            var tempCheckListId = CNService.ConvertMockupIdToCheckListId(param.data.DIE_LINE_MOCKUP_ID, context);

                            var checkListTmp = (from cl in context.ART_WF_MOCKUP_CHECK_LIST
                                                where checklist.Contains(cl.CHECK_LIST_ID)
                                                   && cl.CHECK_LIST_ID == tempCheckListId
                                                select cl).ToList();
                            if (checkListTmp != null)
                            {
                                checkList2 = MapperServices.ART_WF_MOCKUP_CHECK_LIST(checkListTmp);
                            }
                        }
                        else
                        {
                            if (param.data.PRIMARY_TYPE_ID == null)
                                param.data.PRIMARY_TYPE_ID = 0;
                            if (param.data.PACKING_TYPE_ID == null)
                                param.data.PACKING_TYPE_ID = 0;

                            if (param.data.PRIMARY_TYPE_ID > 0 && param.data.PACKING_TYPE_ID > 0)
                            {
                                var q = (from cl in context.ART_WF_MOCKUP_CHECK_LIST
                                         join m in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on cl.CHECK_LIST_ID equals m.CHECK_LIST_ID
                                         where checklist.Contains(cl.CHECK_LIST_ID)
                                         && !m.MOCKUP_NO.StartsWith("MO-D")
                                         && cl.PRIMARY_TYPE_ID == param.data.PRIMARY_TYPE_ID
                                         && m.PACKING_TYPE_ID == param.data.PACKING_TYPE_ID
                                         select cl);

                                checkList2 = MapperServices.ART_WF_MOCKUP_CHECK_LIST(q.ToList());
                            }
                            else if (param.data.PRIMARY_TYPE_ID > 0 && param.data.PACKING_TYPE_ID == 0)
                            {
                                var q = (from cl in context.ART_WF_MOCKUP_CHECK_LIST
                                         join m in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on cl.CHECK_LIST_ID equals m.CHECK_LIST_ID
                                         where checklist.Contains(cl.CHECK_LIST_ID)
                                         && !m.MOCKUP_NO.StartsWith("MO-D")
                                         && cl.PRIMARY_TYPE_ID == param.data.PRIMARY_TYPE_ID
                                         select cl);

                                checkList2 = MapperServices.ART_WF_MOCKUP_CHECK_LIST(q.ToList());
                            }
                            else if (param.data.PRIMARY_TYPE_ID == 0 && param.data.PACKING_TYPE_ID > 0)
                            {
                                var q = (from cl in context.ART_WF_MOCKUP_CHECK_LIST
                                         join m in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on cl.CHECK_LIST_ID equals m.CHECK_LIST_ID
                                         where checklist.Contains(cl.CHECK_LIST_ID)
                                         && !m.MOCKUP_NO.StartsWith("MO-D")
                                         && m.PACKING_TYPE_ID == param.data.PACKING_TYPE_ID
                                         select cl);

                                checkList2 = MapperServices.ART_WF_MOCKUP_CHECK_LIST(q.ToList());
                            }
                            else
                            {
                                var q = (from cl in context.ART_WF_MOCKUP_CHECK_LIST
                                         join m in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on cl.CHECK_LIST_ID equals m.CHECK_LIST_ID
                                         where checklist.Contains(cl.CHECK_LIST_ID)
                                         && !m.MOCKUP_NO.StartsWith("MO-D")
                                         select cl);

                                checkList2 = MapperServices.ART_WF_MOCKUP_CHECK_LIST(q.ToList());
                            }
                        }

                        foreach (var item in checkList2)
                        {
                            ART_WF_MOCKUP_CHECK_LIST_PRODUCT product = new ART_WF_MOCKUP_CHECK_LIST_PRODUCT();
                            product.CHECK_LIST_ID = item.CHECK_LIST_ID;

                            var productTmp = ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(product, context).FirstOrDefault();
                            if (productTmp != null)
                            {
                                item.PRIMARY_SIZE_DISPLAY_TXT = productTmp.PRIMARY_SIZE;
                                item.PACK_SIZE_DISPLAY_TXT = productTmp.PACK_SIZE;
                                item.PACKING_STYLE_DISPLAY_TXT = productTmp.PACKING_STYLE;

                                item.CONTAINER_TYPE_DISPLAY_TXT = productTmp.CONTAINER_TYPE;
                                item.LID_TYPE_DISPLAY_TXT = productTmp.LID_TYPE;
                            }
                            else
                            {
                                if (item.THREE_P_ID > 0)
                                {
                                    item.PRIMARY_SIZE_DISPLAY_TXT = SAP_M_3P_SERVICE.GetByTHREE_P_ID(item.THREE_P_ID, context).PRIMARY_SIZE_DESCRIPTION;
                                    item.CONTAINER_TYPE_DISPLAY_TXT = SAP_M_3P_SERVICE.GetByTHREE_P_ID(item.THREE_P_ID, context).CONTAINER_TYPE_DESCRIPTION;
                                    item.LID_TYPE_DISPLAY_TXT = SAP_M_3P_SERVICE.GetByTHREE_P_ID(item.THREE_P_ID, context).LID_TYPE_DESCRIPTION;
                                }
                                else
                                {
                                    if (item.PRIMARY_SIZE_ID > 0)
                                    {
                                        item.PRIMARY_SIZE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, item.PRIMARY_SIZE_ID);
                                    }
                                    else
                                    {
                                        item.PRIMARY_SIZE_DISPLAY_TXT = item.PRIMARY_SIZE_OTHER;
                                    }

                                    if (item.CONTAINER_TYPE_ID > 0)
                                    {
                                        item.CONTAINER_TYPE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, item.CONTAINER_TYPE_ID);
                                    }
                                    else
                                    {
                                        item.CONTAINER_TYPE_DISPLAY_TXT = item.CONTAINER_TYPE_OTHER;
                                    }

                                    if (item.LID_TYPE_ID > 0)
                                    {
                                        item.LID_TYPE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, item.LID_TYPE_ID);
                                    }
                                    else
                                    {
                                        item.LID_TYPE_DISPLAY_TXT = item.LID_TYPE_OTHER;
                                    }
                                }

                                if (item.TWO_P_ID > 0)
                                {
                                    var temp = SAP_M_2P_SERVICE.GetByTWO_P_ID(item.TWO_P_ID, context);
                                    if (temp != null)
                                    {
                                        item.PACK_SIZE_DISPLAY_TXT = temp.PACK_SIZE_DESCRIPTION;
                                        item.PACKING_STYLE_DISPLAY_TXT = temp.PACKING_SYLE_DESCRIPTION;
                                    }
                                }
                                else
                                {
                                    if (item.PACK_SIZE_ID > 0)
                                        item.PACK_SIZE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, item.PACK_SIZE_ID);
                                    else
                                        item.PACK_SIZE_DISPLAY_TXT = item.PACK_SIZE_OTHER;

                                    if (item.PACKING_STYLE_ID > 0)
                                        item.PACKING_STYLE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, item.PACKING_STYLE_ID);
                                    else
                                        item.PACKING_STYLE_DISPLAY_TXT = item.PACKING_STYLE_OTHER;
                                }
                            }

                            item.PRIMARY_TYPE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, item.PRIMARY_TYPE_ID);

                            var priceTemplate = (from p in context.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG
                                                 where p.MOCKUP_SUB_ID == item.MOCKUP_SUB_ID
                                                 select p).FirstOrDefault();

                            if (priceTemplate != null && priceTemplate.STYLE_OF_PRINTING != null)
                            {
                                if (priceTemplate.STYLE_OF_PRINTING > 0)
                                {
                                    item.STYLE_OF_PRINTING_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, priceTemplate.STYLE_OF_PRINTING);
                                }
                                else
                                {
                                    item.STYLE_OF_PRINTING_DISPLAY_TXT = priceTemplate.STYLE_OF_PRINTING_OTHER;
                                }
                            }
                        }

                        if (param.data.PRIMARY_SIZE_ID > 0)
                        {
                            checkList2 = checkList2.Where(m => !string.IsNullOrEmpty(m.PRIMARY_SIZE_DISPLAY_TXT)).ToList();
                            checkList2 = checkList2.Where(m => m.PRIMARY_SIZE_DISPLAY_TXT.Replace(" ", String.Empty).ToLower().Trim().Contains(PRIMARY_SIZE_TXT.Replace(" ", String.Empty).ToLower().Trim())).ToList();
                        }
                        if (!string.IsNullOrEmpty(PRIMARY_SIZE_DISPLAY_TXT))
                        {
                            checkList2 = checkList2.Where(m => !string.IsNullOrEmpty(m.PRIMARY_SIZE_DISPLAY_TXT)).ToList();
                            checkList2 = checkList2.Where(m => m.PRIMARY_SIZE_DISPLAY_TXT.Replace(" ", String.Empty).ToLower().Trim().Contains(PRIMARY_SIZE_DISPLAY_TXT.Replace(" ", String.Empty).ToLower().Trim())).ToList();
                        }
                        if (param.data.PACK_SIZE_ID > 0)
                        {
                            checkList2 = checkList2.Where(m => !string.IsNullOrEmpty(m.PACK_SIZE_DISPLAY_TXT)).ToList();
                            checkList2 = checkList2.Where(m => m.PACK_SIZE_DISPLAY_TXT.Replace(" ", String.Empty).ToLower().Trim().Contains(PACK_SIZE_TXT.Replace(" ", String.Empty).ToLower().Trim())).ToList();
                        }
                        if (!string.IsNullOrEmpty(PACKING_STYLE_DISPLAY_TXT))
                        {
                            checkList2 = checkList2.Where(m => !string.IsNullOrEmpty(m.PACKING_STYLE_DISPLAY_TXT)).ToList();
                            checkList2 = checkList2.Where(m => m.PACKING_STYLE_DISPLAY_TXT.Replace(" ", String.Empty).ToLower().Trim().Contains(PACKING_STYLE_DISPLAY_TXT.Replace(" ", String.Empty).ToLower().Trim())).ToList();
                        }

                        var temp2 = checkList2.Select(m => m.CHECK_LIST_ID).ToList();

                        if (param.data.PACKING_TYPE_ID > 0)
                        {
                            listOfCheckListItem = (from cl in context.ART_WF_MOCKUP_CHECK_LIST
                                                   join m in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on cl.CHECK_LIST_ID equals m.CHECK_LIST_ID
                                                   where temp2.Contains(cl.CHECK_LIST_ID)
                                                   && !m.MOCKUP_NO.StartsWith("MO-D")
                                                   && m.PACKING_TYPE_ID == param.data.PACKING_TYPE_ID
                                                   select m).ToList();
                        }
                        else
                        {
                            listOfCheckListItem = (from cl in context.ART_WF_MOCKUP_CHECK_LIST
                                                   join m in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on cl.CHECK_LIST_ID equals m.CHECK_LIST_ID
                                                   where temp2.Contains(cl.CHECK_LIST_ID)
                                                   && !m.MOCKUP_NO.StartsWith("MO-D")
                                                   select m).ToList();
                        }


                        listOfCheckListItem = (from l in listOfCheckListItem
                                               where l.CHECK_LIST_ID != param.data.CHECK_LIST_ID
                                                && l.MOCKUP_ID != param.data.MOCKUP_ID
                                                && l.MOCKUP_NO != null
                                               select l).ToList();

                        List<SEARCH_DIE_LINE> listSearchDieLine = new List<SEARCH_DIE_LINE>();
                        foreach (var item in listOfCheckListItem)
                        {
                            var searchDieLine = GetDieLine(item, checkList2, context);
                            if (searchDieLine != null)
                                listSearchDieLine.Add(searchDieLine);
                        }

                        listSearchDieLine = listSearchDieLine.ToList();
                        Results.data = listSearchDieLine;
                        Results.status = "S";
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

        public static SEARCH_DIE_LINE_RESULT GetDieLineDataForPAStep(SEARCH_DIE_LINE_REQUEST param)
        {
            List<ART_WF_MOCKUP_CHECK_LIST_ITEM> listOfCheckListItem = new List<ART_WF_MOCKUP_CHECK_LIST_ITEM>();
            SEARCH_DIE_LINE_RESULT Results = new SEARCH_DIE_LINE_RESULT();
            try
            {
                List<ART_WF_MOCKUP_CHECK_LIST_2> listCheckList2 = new List<ART_WF_MOCKUP_CHECK_LIST_2>();
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var process = (from p in context.ART_WF_MOCKUP_PROCESS
                                       where p.MOCKUP_SUB_ID == param.data.MOCKUP_SUB_ID
                                       && (String.IsNullOrEmpty(p.REMARK_KILLPROCESS))
                                       select p).FirstOrDefault();

                        var checklistItem = (from c in context.ART_WF_MOCKUP_CHECK_LIST_ITEM
                                             where c.MOCKUP_ID == process.MOCKUP_ID
                                             select c).FirstOrDefault();


                        var checklist = (from c in context.ART_WF_MOCKUP_CHECK_LIST
                                         where c.CHECK_LIST_ID == checklistItem.CHECK_LIST_ID
                                         select c).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                        ART_WF_MOCKUP_CHECK_LIST_2 checklist_2 = new ART_WF_MOCKUP_CHECK_LIST_2();
                        checklist_2 = MapperServices.ART_WF_MOCKUP_CHECK_LIST(checklist);

                        listOfCheckListItem.Add(checklistItem);
                        listCheckList2.Add(checklist_2);

                        foreach (var item in listCheckList2)
                        {
                            ART_WF_MOCKUP_CHECK_LIST_PRODUCT product = new ART_WF_MOCKUP_CHECK_LIST_PRODUCT();
                            product.CHECK_LIST_ID = item.CHECK_LIST_ID;

                            var productTmp = ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(product, context).FirstOrDefault();
                            if (productTmp != null)
                            {
                                item.PRIMARY_SIZE_DISPLAY_TXT = productTmp.PRIMARY_SIZE;
                                item.PACK_SIZE_DISPLAY_TXT = productTmp.PACK_SIZE;
                                item.PACKING_STYLE_DISPLAY_TXT = productTmp.PACKING_STYLE;
                            }
                            else
                            {
                                if (item.THREE_P_ID > 0)
                                    item.PRIMARY_SIZE_DISPLAY_TXT = SAP_M_3P_SERVICE.GetByTHREE_P_ID(item.THREE_P_ID, context).PRIMARY_SIZE_DESCRIPTION;
                                else
                                {
                                    if (item.PRIMARY_SIZE_ID > 0)
                                        item.PRIMARY_SIZE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, item.PRIMARY_SIZE_ID);
                                    else
                                        item.PRIMARY_SIZE_DISPLAY_TXT = item.PRIMARY_SIZE_OTHER;
                                }

                                if (item.TWO_P_ID > 0)
                                {
                                    var temp = SAP_M_2P_SERVICE.GetByTWO_P_ID(item.TWO_P_ID, context);
                                    if (temp != null)
                                    {
                                        item.PACK_SIZE_DISPLAY_TXT = temp.PACK_SIZE_DESCRIPTION;
                                        item.PACKING_STYLE_DISPLAY_TXT = temp.PACKING_SYLE_DESCRIPTION;
                                    }
                                }
                                else
                                {
                                    if (item.PACK_SIZE_ID > 0)
                                        item.PACK_SIZE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, item.PACK_SIZE_ID);
                                    else
                                        item.PACK_SIZE_DISPLAY_TXT = item.PACK_SIZE_OTHER;

                                    if (item.PACKING_STYLE_ID > 0)
                                        item.PACKING_STYLE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, item.PACKING_STYLE_ID);
                                    else
                                        item.PACKING_STYLE_DISPLAY_TXT = item.PACKING_STYLE_OTHER;
                                }
                            }

                            item.PRIMARY_TYPE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, item.PRIMARY_TYPE_ID);
                        }

                        List<SEARCH_DIE_LINE> listSearchDieLine = new List<SEARCH_DIE_LINE>();
                        foreach (var item in listOfCheckListItem)
                        {
                            var searchDieLine = GetDieLine(item, listCheckList2, context);
                            if (searchDieLine != null)
                                listSearchDieLine.Add(searchDieLine);
                        }

                        listSearchDieLine = listSearchDieLine.OrderBy(m => m.CHECK_LIST_NO).ThenBy(m => m.MOCKUP_NO).ToList();
                        Results.data = listSearchDieLine;
                        Results.status = "S";
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

        public static SEARCH_DIE_LINE GetDieLine(ART_WF_MOCKUP_CHECK_LIST_ITEM item, List<ART_WF_MOCKUP_CHECK_LIST_2> paramCheckList, ARTWORKEntities context)
        {
            SEARCH_DIE_LINE searchDieLine = new SEARCH_DIE_LINE();

            searchDieLine.CHECK_LIST_ID = item.CHECK_LIST_ID;
            var checkList = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(item.CHECK_LIST_ID, context);
            if (checkList == null)
            {
                return null;
            }
            searchDieLine.CHECK_LIST_NO = checkList.CHECK_LIST_NO;
            searchDieLine.MOCKUP_ID = item.MOCKUP_ID;
            searchDieLine.MOCKUP_NO = item.MOCKUP_NO;

            if (item.MOCKUP_NO.StartsWith("MO-D"))
            {
                return null;
            }

            searchDieLine.PRIMARY_TYPE_ID = checkList.PRIMARY_TYPE_ID;
            searchDieLine.PRIMARY_SIZE_ID = checkList.PRIMARY_SIZE_ID;
            searchDieLine.PACK_SIZE_ID = checkList.PACK_SIZE_ID;
            searchDieLine.PACKING_STYLE_ID = checkList.PACKING_STYLE_ID;
            searchDieLine.PACKING_TYPE_ID = item.PACKING_TYPE_ID;

            var process2 = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = item.MOCKUP_ID }, context).Where(m => m.PARENT_MOCKUP_SUB_ID == null).FirstOrDefault();
            if (process2 != null)
            {
                searchDieLine.MOCKUP_SUB_ID = process2.MOCKUP_SUB_ID;
                if (!String.IsNullOrEmpty(process2.IS_END))
                {
                    searchDieLine.STATUS_DISPLAY_TXT = "Completed";
                }
                else if (!String.IsNullOrEmpty(process2.IS_TERMINATE))
                {
                    searchDieLine.STATUS_DISPLAY_TXT = "Terminated";
                }
                else
                {
                    searchDieLine.STATUS_DISPLAY_TXT = "In progress";
                }
            }

            searchDieLine.PRIMARY_TYPE_DISPLAY_TXT = paramCheckList.Where(m => m.CHECK_LIST_ID == checkList.CHECK_LIST_ID).FirstOrDefault().PRIMARY_TYPE_DISPLAY_TXT;
            searchDieLine.PRIMARY_SIZE_DISPLAY_TXT = paramCheckList.Where(m => m.CHECK_LIST_ID == checkList.CHECK_LIST_ID).FirstOrDefault().PRIMARY_SIZE_DISPLAY_TXT;
            searchDieLine.PACK_SIZE_DISPLAY_TXT = paramCheckList.Where(m => m.CHECK_LIST_ID == checkList.CHECK_LIST_ID).FirstOrDefault().PACK_SIZE_DISPLAY_TXT;
            searchDieLine.CONTAINER_TYPE_DISPLAY_TXT = paramCheckList.Where(m => m.CHECK_LIST_ID == checkList.CHECK_LIST_ID).FirstOrDefault().CONTAINER_TYPE_DISPLAY_TXT;
            searchDieLine.LID_TYPE_DISPLAY_TXT = paramCheckList.Where(m => m.CHECK_LIST_ID == checkList.CHECK_LIST_ID).FirstOrDefault().LID_TYPE_DISPLAY_TXT;
            searchDieLine.STYLE_OF_PRINTING_DISPLAY_TXT = paramCheckList.Where(m => m.CHECK_LIST_ID == checkList.CHECK_LIST_ID).FirstOrDefault().STYLE_OF_PRINTING_DISPLAY_TXT;

            if (String.IsNullOrEmpty(searchDieLine.STYLE_OF_PRINTING_DISPLAY_TXT))
            {
                var tempProcessTemplate_pg = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG() { MOCKUP_ID = item.MOCKUP_ID }, context);
                var processTemplate_pg = (from p in tempProcessTemplate_pg orderby p.UPDATE_DATE descending select p).FirstOrDefault();

                if (processTemplate_pg != null)
                {
                    if (!string.IsNullOrEmpty(processTemplate_pg.STYLE_OF_PRINTING_OTHER))
                    {
                        searchDieLine.STYLE_OF_PRINTING_DISPLAY_TXT = processTemplate_pg.STYLE_OF_PRINTING_OTHER;
                    }
                    else if (processTemplate_pg.STYLE_OF_PRINTING > 0)
                    {
                        var characteristic = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(processTemplate_pg.STYLE_OF_PRINTING, context);
                        if (characteristic != null)
                        {
                            searchDieLine.STYLE_OF_PRINTING_DISPLAY_TXT = characteristic.DESCRIPTION;
                        }
                    }
                }
            }

            searchDieLine.PACKING_STYLE_DISPLAY_TXT = paramCheckList.Where(m => m.CHECK_LIST_ID == checkList.CHECK_LIST_ID).FirstOrDefault().PACKING_STYLE_DISPLAY_TXT;
            searchDieLine.PACKAGING_TYPE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, item.PACKING_TYPE_ID);

            var process = (from p in context.ART_WF_MOCKUP_PROCESS
                           where p.MOCKUP_ID == item.MOCKUP_ID
                           && (String.IsNullOrEmpty(p.REMARK_KILLPROCESS))
                           select p).FirstOrDefault();

            if (process != null)
            {
                var processPG = (from pg in context.ART_WF_MOCKUP_PROCESS_PG
                                 where pg.MOCKUP_SUB_ID == process.MOCKUP_SUB_ID
                                 select pg).OrderByDescending(o => o.MOCKUP_SUB_PG_ID).FirstOrDefault();

                if (processPG != null)
                {

                    //---------by aof 20220118 for CR sustain-- - start

                    searchDieLine.SUSTAIN_MATERIAL = processPG.SUSTAIN_MATERIAL;
                    searchDieLine.PLASTIC_TYPE = processPG.PLASTIC_TYPE;
                    searchDieLine.REUSEABLE = processPG.REUSEABLE;
                    searchDieLine.RECYCLABLE = processPG.RECYCLABLE;
                    searchDieLine.COMPOSATABLE = processPG.COMPOSATABLE;
                    searchDieLine.RECYCLE_CONTENT = processPG.RECYCLE_CONTENT;
                    searchDieLine.CERT = processPG.CERT;
                    searchDieLine.CERT_SOURCE = processPG.CERT_SOURCE;
                    searchDieLine.PKG_WEIGHT = processPG.PKG_WEIGHT;
                    searchDieLine.SUSTAIN_OTHER = processPG.SUSTAIN_OTHER;
                    if (processPG.SUSTAIN_MATERIAL != null)
                    {
                        var SUSTAIN_MATERIAL = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(processPG.SUSTAIN_MATERIAL, context);
                        if (SUSTAIN_MATERIAL != null)
                        {
                            searchDieLine.SUSTAIN_MATERIAL_DISPLAY_TXT = SUSTAIN_MATERIAL.DESCRIPTION;
                        }
                    }

                    if (processPG.PLASTIC_TYPE != null)
                    {
                        var PLASTIC_TYPE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(processPG.PLASTIC_TYPE, context);
                        if (PLASTIC_TYPE != null)
                        {
                            searchDieLine.PLASTIC_TYPE_DISPLAY_TXT = PLASTIC_TYPE.DESCRIPTION;
                        }
                    }

                    if (processPG.CERT_SOURCE != null)
                    {
                        var CERT_SOURCE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(processPG.CERT_SOURCE, context);
                        if (CERT_SOURCE != null)
                        {
                            searchDieLine.CERT_SOURCE_DISPLAY_TXT = CERT_SOURCE.DESCRIPTION;
                        }
                    }


                    //---------by aof 20220118 for CR sustain-- - end

                    searchDieLine.CUSTOMER_SIZE_REMARK = processPG.CUSTOMER_SIZE_REMARK;
                    searchDieLine.CUSTOMER_SPEC_REMARK = processPG.CUSTOMER_SPEC_REMARK;
                    searchDieLine.CUSTOMER_NOMINATES_VENDOR_REMARK = processPG.CUSTOMER_NOMINATES_VENDOR_REMARK;
                    searchDieLine.CUSTOMER_NOMINATES_VENDOR = processPG.CUSTOMER_NOMINATES_VENDOR;
                    searchDieLine.CUSTOMER_SPEC = processPG.CUSTOMER_SPEC;
                    searchDieLine.CUSTOMER_SIZE = processPG.CUSTOMER_SIZE;

                    searchDieLine.ROLL_SHEET = processPG.ROLL_SHEET;
                    searchDieLine.SHEET_SIZE = processPG.SHEET_SIZE;
                    searchDieLine.ACCESSORIES = processPG.ACCESSORIES;
                    searchDieLine.PRINT_SYSTEM = processPG.PRINT_SYSTEM;

                    searchDieLine.DIMENSION_OF = processPG.DIMENSION_OF;
                    searchDieLine.FINAL_INFO = processPG.FINAL_INFO;
                    searchDieLine.REMARK_PG = processPG.REMARK_PG;
                    searchDieLine.ID_MM = processPG.ID_MM;

                    searchDieLine.DIMENSION_OF_DISPLAY_TXT = processPG.DIMENSION_OF;
                    if (processPG.VENDOR > 0)
                    {
                        searchDieLine.VENDOR_DISPLAY_TXT = CNService.GetVendorCodeName(processPG.VENDOR, context);
                    }
                    else
                    {
                        searchDieLine.VENDOR_DISPLAY_TXT = processPG.VENDOR_OTHER;
                    }

                    searchDieLine.GRADE_OF = processPG.GRADE_OF;
                    if (processPG.GRADE_OF > 0)
                    {
                        searchDieLine.GRADE_OF_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, processPG.GRADE_OF);
                    }
                    else
                    {
                        searchDieLine.GRADE_OF_DISPLAY_TXT = processPG.GRADE_OF_OTHER;
                    }

                    searchDieLine.FLUTE = processPG.FLUTE;
                    if (processPG.FLUTE > 0)
                    {
                        searchDieLine.FLUTE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, processPG.FLUTE);
                    }
                    else
                    {
                        searchDieLine.FLUTE_DISPLAY_TXT = processPG.FLUTE_OTHER;
                    }

                    searchDieLine.DI_CUT = processPG.DI_CUT;
                    if (processPG.DI_CUT > 0)
                    {
                        searchDieLine.DI_CUT_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, processPG.DI_CUT);
                    }
                    else
                    {
                        searchDieLine.DI_CUT_DISPLAY_TXT = processPG.DI_CUT_OTHER;
                    }

                }
            }

            var checklist_pg = (from p in context.ART_WF_MOCKUP_CHECK_LIST_PG
                                where p.MOCKUP_ID == item.MOCKUP_ID
                                && p.CHECK_LIST_ID == item.CHECK_LIST_ID
                                select p).FirstOrDefault();

            if (checklist_pg != null)
            {
                if (!String.IsNullOrEmpty(checklist_pg.STYLE_OTHER))
                {
                    searchDieLine.STYLE_DISPLAY_TXT = checklist_pg.STYLE_OTHER;
                }
                else
                {
                    if (checklist_pg.STYLE_ID != null)
                    {
                        searchDieLine.STYLE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, checklist_pg.STYLE_ID);
                    }
                }

                if (!String.IsNullOrEmpty(checklist_pg.NUMBER_OF_COLOR_OTHER))
                {
                    searchDieLine.NUMBER_OF_COLOR_DISPLAY_TXT = checklist_pg.NUMBER_OF_COLOR_OTHER;
                }
                else
                {
                    if (checklist_pg.NUMBER_OF_COLOR_ID != null)
                    {
                        searchDieLine.NUMBER_OF_COLOR_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, checklist_pg.NUMBER_OF_COLOR_ID);
                    }
                }
            }


            return searchDieLine;
        }

        private static string GetDescriptionFromCharacteristic(ARTWORKEntities context, Nullable<int> id)
        {
            if (id == null)
            { return ""; }

            var desc = (from g in context.SAP_M_CHARACTERISTIC
                        where g.CHARACTERISTIC_ID == id
                        select g.DESCRIPTION).FirstOrDefault();
            if (desc == null) return "";
            else return desc;
        }



        //-------------------------------------- by aof #INC-4849 -----------------------------------------------------------
        public static V_ART_SEARCH_DIELINE_RESULT GetDieLine_TUTuning(V_ART_SEARCH_DIELINE_REQUEST param)
        {
            V_ART_SEARCH_DIELINE_RESULT Results = new V_ART_SEARCH_DIELINE_RESULT();
            try
            {

                if (param.data.FIRST_LOAD)
                {
                    Results.data = new List<V_ART_SEARCH_DIELINE_2>();
                    Results.status = "S";
                    Results.draw = param.draw;
                    return Results;
                }


                var cnt = 0;
                var listResultAll = QueryDieLine_TUTuning(param,ref cnt);

                Results.recordsTotal = cnt;
                Results.recordsFiltered = cnt;
                Results.status = "S";

                Results.data = listResultAll;
                Results.draw = param.draw;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        private static List<V_ART_SEARCH_DIELINE_2> QueryDieLine_TUTuning(V_ART_SEARCH_DIELINE_REQUEST param, ref int cnt)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;
                    string where_mo = "";


                    //------------------------------------------------------WHERE---------------------------------------------------
                    if (param.data.PRIMARY_TYPE_ID > 0)
                    {
                        where_mo = CNService.getSQLWhereByJoinStringWithAnd(where_mo, "PRIMARY_TYPE_ID =" + param.data.PRIMARY_TYPE_ID);
                    }

                    if (!string.IsNullOrEmpty(param.data.PRIMARY_SIZE_DISPLAY_TXT))
                    {
                        param.data.PRIMARY_SIZE_DISPLAY_TXT = param.data.PRIMARY_SIZE_DISPLAY_TXT.Replace(" ", String.Empty).ToLower();
                        var PRODUCT_PRIMARY_SIZE = CNService.getSQLWhereLikeByConvertString(param.data.PRIMARY_SIZE_DISPLAY_TXT.Trim(), setReplaceSpace("PRODUCT_PRIMARY_SIZE"), false, true,false);
                        var SAP3P_PRIMARY_SIZE = CNService.getSQLWhereLikeByConvertString(param.data.PRIMARY_SIZE_DISPLAY_TXT.Trim(), setReplaceSpace("SAP3P_PRIMARY_SIZE"), false, true,false);
                        var CHECKLIST_PRIMARY_SIZE_OTHER = CNService.getSQLWhereLikeByConvertString(param.data.PRIMARY_SIZE_DISPLAY_TXT.Trim(), setReplaceSpace("CHECKLIST_PRIMARY_SIZE_OTHER"), false, true,false);
                    
                        where_mo = CNService.getSQLWhereByJoinStringWithAnd(where_mo, "((" + PRODUCT_PRIMARY_SIZE + ") OR (" + SAP3P_PRIMARY_SIZE + ") OR ("+ CHECKLIST_PRIMARY_SIZE_OTHER + "))");
                    }

                    if (!string.IsNullOrEmpty(param.data.PACK_SIZE_DISPLAY_TXT))
                    {
                        param.data.PACK_SIZE_DISPLAY_TXT = param.data.PACK_SIZE_DISPLAY_TXT.Replace(" ", String.Empty).ToLower();
                        var PRODUCT_PACK_SIZE = "PRODUCT_PACK_SIZE='" + param.data.PACK_SIZE_DISPLAY_TXT + "'";
                        var SAP2P_PACK_SIZE = "SAP2P_PACK_SIZE='" + param.data.PACK_SIZE_DISPLAY_TXT + "'";
                        var CHECKLIST_PACK_SIZE = "CHECKLIST_PACK_SIZE='" + param.data.PACK_SIZE_DISPLAY_TXT + "'";
                        var CHECKLIST_PACK_SIZE_OTHER = "CHECKLIST_PACK_SIZE_OTHER='" + param.data.PACK_SIZE_DISPLAY_TXT + "'";

                        where_mo = CNService.getSQLWhereByJoinStringWithAnd(where_mo, "((" + PRODUCT_PACK_SIZE + ") OR (" + SAP2P_PACK_SIZE + ") OR (" + CHECKLIST_PACK_SIZE + ") OR ("+ CHECKLIST_PACK_SIZE_OTHER + "))");
                    }


                    if (!string.IsNullOrEmpty(param.data.PACKING_STYLE_DISPLAY_TXT))
                    {
                        param.data.PACKING_STYLE_DISPLAY_TXT = param.data.PACKING_STYLE_DISPLAY_TXT.Replace(" ", String.Empty).ToLower();
                        var PRODUCT_PACKING_STYLE = CNService.getSQLWhereLikeByConvertString(param.data.PACKING_STYLE_DISPLAY_TXT.Trim(), setReplaceSpace("PRODUCT_PACKING_STYLE"), false, true,false );
                        var SAP2P_PACKING_STYLE = CNService.getSQLWhereLikeByConvertString(param.data.PACKING_STYLE_DISPLAY_TXT.Trim(), setReplaceSpace("SAP2P_PACKING_STYLE"), false, true,false);
                        var CHECKLIST_PACKING_STYLE_OTHER = CNService.getSQLWhereLikeByConvertString(param.data.PACKING_STYLE_DISPLAY_TXT.Trim(), setReplaceSpace("CHECKLIST_PACKING_STYLE_OTHER"), false, true,false);

                        where_mo = CNService.getSQLWhereByJoinStringWithAnd(where_mo, "((" + PRODUCT_PACKING_STYLE + ") OR (" + SAP2P_PACKING_STYLE + ") OR (" + CHECKLIST_PACKING_STYLE_OTHER + "))");
                    }


                    if (param.data.PACKING_TYPE_ID > 0)
                    {
                        where_mo = CNService.getSQLWhereByJoinStringWithAnd(where_mo, "PACKING_TYPE_ID =" + param.data.PACKING_TYPE_ID);
                    }

                    if (!string.IsNullOrEmpty(param.data.DIMENSION_OF_DISPLAY_TXT))
                    {
                        param.data.DIMENSION_OF_DISPLAY_TXT = param.data.DIMENSION_OF_DISPLAY_TXT.Replace(" ", String.Empty).ToLower();
                        var DIMENSION_OF = CNService.getSQLWhereLikeByConvertString(param.data.DIMENSION_OF_DISPLAY_TXT.Trim(), setReplaceSpace("DIMENSION_OF"), false, true,false);
                    
                        where_mo = CNService.getSQLWhereByJoinStringWithAnd(where_mo, DIMENSION_OF);
                    }


                    if (!string.IsNullOrEmpty(param.data.FINAL_INFO_GROUP_DISPLAY_TXT))
                    {
                        param.data.FINAL_INFO_GROUP_DISPLAY_TXT = param.data.FINAL_INFO_GROUP_DISPLAY_TXT.Replace(" ", String.Empty).ToLower();
                        var FINAL_INFO = CNService.getSQLWhereLikeByConvertString(param.data.FINAL_INFO_GROUP_DISPLAY_TXT.Trim(), setReplaceSpace("FINAL_INFO"), false, true,false);
                        where_mo = CNService.getSQLWhereByJoinStringWithAnd(where_mo, FINAL_INFO);
                    }

                    //------------------------------------------------------WHERE---------------------------------------------------


                    var q = context.Database.SqlQuery<V_ART_SEARCH_DIELINE_2>("sp_ART_WF_ARTWORK_SEARCH_DIELINE @where_mo", new System.Data.SqlClient.SqlParameter("@where_mo", where_mo)).ToList();
                    cnt = q.Count();
                    // q = q.OrderBy(i => i.MOCKUP_NO).Skip(param.start).Take(param.length).ToList();
                    return q;
                }
            }
        }

        public static string setReplaceSpace(string val)
        {
            return "replace(lower(" + val + "),' ','')" ;
        }

        //-------------------------------------- by aof #INC-4849 -----------------------------------------------------------

        public static V_ART_SEARCH_DIELINE_RESULT GetDieLine2(V_ART_SEARCH_DIELINE_REQUEST param)
        {
            V_ART_SEARCH_DIELINE_RESULT Results = new V_ART_SEARCH_DIELINE_RESULT();
            try
            {
                var cnt = 0;
                var listResultAll = QueryDieLine2(param, ref cnt);

                Results.recordsTotal = cnt;
                Results.recordsFiltered = cnt;
                Results.status = "S";

                Results.data = listResultAll;
                Results.draw = param.draw;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        private static List<V_ART_SEARCH_DIELINE_2> QueryDieLine2(V_ART_SEARCH_DIELINE_REQUEST param, ref int cnt)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;

                    IQueryable<V_ART_SEARCH_DIELINE> q = null;

                    q = (from m in context.V_ART_SEARCH_DIELINE select m);

                    if (param.data.PRIMARY_TYPE_ID > 0)
                    {
                        q = q.Where(m => m.PRIMARY_TYPE_ID == param.data.PRIMARY_TYPE_ID);
                    }
                    if (param.data.PACKING_TYPE_ID > 0)
                    {
                        q = q.Where(m => m.PACKING_TYPE_ID == param.data.PACKING_TYPE_ID);
                    }
                    if (param.data.PACK_SIZE_ID > 0)
                    {
                        var temp = GetDescriptionFromCharacteristic(context, param.data.PACKING_TYPE_ID);
                        q = q.Where(m => m.PACK_SIZE_DISPLAY_TXT.Replace(" ", String.Empty).ToLower().Trim().Contains(temp.Replace(" ", String.Empty).ToLower().Trim()));
                    }
                    if (!string.IsNullOrEmpty(param.data.PRIMARY_SIZE_DISPLAY_TXT))
                    {
                        q = q.Where(m => m.PRIMARY_SIZE_DISPLAY_TXT.Replace(" ", String.Empty).ToLower().Trim().Contains(param.data.PRIMARY_SIZE_DISPLAY_TXT.Replace(" ", String.Empty).ToLower().Trim()));
                    }
                    if (!string.IsNullOrEmpty(param.data.PACKING_STYLE_DISPLAY_TXT))
                    {
                        q = q.Where(m => m.PACKING_STYLE_DISPLAY_TXT.Replace(" ", String.Empty).ToLower().Trim().Contains(param.data.PACKING_STYLE_DISPLAY_TXT.Replace(" ", String.Empty).ToLower().Trim()));
                    }

                    if (!string.IsNullOrEmpty(param.search.value))
                    {
                        param.search.value = param.search.value.Trim();

                        q = q.Where(m => m.CHECK_LIST_NO.Contains(param.search.value)
                        || m.MOCKUP_NO.Contains(param.search.value)
                        || m.PRIMARY_TYPE_DISPLAY_TXT.Contains(param.search.value)
                        || m.PRIMARY_SIZE_DISPLAY_TXT.Contains(param.search.value)
                        || m.CONTAINER_TYPE_DISPLAY_TXT.Contains(param.search.value)
                        || m.LID_TYPE_DISPLAY_TXT.Contains(param.search.value)
                        || m.PACK_SIZE_DISPLAY_TXT.Contains(param.search.value)
                        || m.PACKING_STYLE_DISPLAY_TXT.Contains(param.search.value)
                        || m.PACKAGING_TYPE_DISPLAY_TXT.Contains(param.search.value)
                        || m.VENDOR_DISPLAY_TXT.Contains(param.search.value)
                        || m.GRADE_OF_DISPLAY_TXT.Contains(param.search.value)
                        || m.DIMENSION_OF_DISPLAY_TXT.Contains(param.search.value)
                        || m.SHEET_SIZE.Contains(param.search.value)
                        || m.FLUTE_DISPLAY_TXT.Contains(param.search.value)
                        || m.STYLE_DISPLAY_TXT.Contains(param.search.value)
                        || m.STATUS_DISPLAY_TXT.Contains(param.search.value)
                        || m.NUMBER_OF_COLOR_DISPLAY_TXT.Contains(param.search.value)
                        || m.STYLE_OF_PRINTING_DISPLAY_TXT.Contains(param.search.value)
                        );
                    }

                    cnt = q.Count();

                    return OrderByDieLine2(q, param);
                }
            }
        }

        private static List<V_ART_SEARCH_DIELINE_2> OrderByDieLine2(IQueryable<V_ART_SEARCH_DIELINE> q, V_ART_SEARCH_DIELINE_REQUEST param)
        {
            var orderColumn = 1;
            var orderDir = "asc";
            if (param.order != null && param.order.Count > 0)
            {
                orderColumn = param.order[0].column;
                orderDir = param.order[0].dir; //desc ,asc
            }

            string orderASC = "asc";
            string orderDESC = "desc";
            if (orderColumn == 1)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.CHECK_LIST_NO).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.CHECK_LIST_NO).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 2)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.MOCKUP_NO).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.MOCKUP_NO).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 3)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.PRIMARY_TYPE_DISPLAY_TXT).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.PRIMARY_TYPE_DISPLAY_TXT).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 4)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.PRIMARY_SIZE_DISPLAY_TXT).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.PRIMARY_SIZE_DISPLAY_TXT).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 5)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.CONTAINER_TYPE_DISPLAY_TXT).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.CONTAINER_TYPE_DISPLAY_TXT).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 6)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.LID_TYPE_DISPLAY_TXT).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.LID_TYPE_DISPLAY_TXT).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 7)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.PACK_SIZE_DISPLAY_TXT).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.PACK_SIZE_DISPLAY_TXT).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 8)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.PACKING_STYLE_DISPLAY_TXT).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.PACKING_STYLE_DISPLAY_TXT).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 9)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.PACKAGING_TYPE_DISPLAY_TXT).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.PACKAGING_TYPE_DISPLAY_TXT).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 10)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.VENDOR_DISPLAY_TXT).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.VENDOR_DISPLAY_TXT).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 11)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.GRADE_OF_DISPLAY_TXT).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.GRADE_OF_DISPLAY_TXT).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 12)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.DIMENSION_OF_DISPLAY_TXT).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.DIMENSION_OF_DISPLAY_TXT).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 13)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.SHEET_SIZE).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.SHEET_SIZE).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 14)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.FLUTE_DISPLAY_TXT).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.FLUTE_DISPLAY_TXT).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 15)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.STYLE_DISPLAY_TXT).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.STYLE_DISPLAY_TXT).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 16)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.STATUS_DISPLAY_TXT).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.STATUS_DISPLAY_TXT).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 17)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.NUMBER_OF_COLOR_DISPLAY_TXT).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.NUMBER_OF_COLOR_DISPLAY_TXT).Skip(param.start).Take(param.length);
            }
            if (orderColumn == 18)
            {
                if (orderDir == orderASC)
                    q = q.OrderBy(i => i.STYLE_OF_PRINTING_DISPLAY_TXT).Skip(param.start).Take(param.length);
                else if (orderDir == orderDESC)
                    q = q.OrderByDescending(i => i.STYLE_OF_PRINTING_DISPLAY_TXT).Skip(param.start).Take(param.length);
            }

            return (from p in q
                    select new V_ART_SEARCH_DIELINE_2
                    {
                        MOCKUP_ID = p.MOCKUP_ID,
                        PACKING_TYPE_ID = p.PACKING_TYPE_ID,
                        CHECK_LIST_ID = p.CHECK_LIST_ID,
                        MOCKUP_SUB_ID = p.MOCKUP_SUB_ID,
                        CHECK_LIST_NO = p.CHECK_LIST_NO,
                        MOCKUP_NO = p.MOCKUP_NO,
                        PRIMARY_TYPE_DISPLAY_TXT = p.PRIMARY_TYPE_DISPLAY_TXT,
                        PRIMARY_SIZE_DISPLAY_TXT = p.PRIMARY_SIZE_DISPLAY_TXT,
                        CONTAINER_TYPE_DISPLAY_TXT = p.CONTAINER_TYPE_DISPLAY_TXT,
                        LID_TYPE_DISPLAY_TXT = p.LID_TYPE_DISPLAY_TXT,
                        PACK_SIZE_DISPLAY_TXT = p.PACK_SIZE_DISPLAY_TXT,
                        PACKING_STYLE_DISPLAY_TXT = p.PACKING_STYLE_DISPLAY_TXT,
                        PACKAGING_TYPE_DISPLAY_TXT = p.PACKAGING_TYPE_DISPLAY_TXT,
                        VENDOR_DISPLAY_TXT = p.VENDOR_DISPLAY_TXT,
                        GRADE_OF_DISPLAY_TXT = p.GRADE_OF_DISPLAY_TXT,
                        DIMENSION_OF_DISPLAY_TXT = p.DIMENSION_OF_DISPLAY_TXT,
                        SHEET_SIZE = p.SHEET_SIZE,
                        FLUTE_DISPLAY_TXT = p.FLUTE_DISPLAY_TXT,
                        STYLE_DISPLAY_TXT = p.STYLE_DISPLAY_TXT,
                        STATUS_DISPLAY_TXT = p.STATUS_DISPLAY_TXT,
                        NUMBER_OF_COLOR_DISPLAY_TXT = p.NUMBER_OF_COLOR_DISPLAY_TXT,
                        STYLE_OF_PRINTING_DISPLAY_TXT = p.STYLE_OF_PRINTING_DISPLAY_TXT,
                    }).ToList();
        }

    }
}
