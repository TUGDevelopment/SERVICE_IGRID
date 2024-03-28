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
    public class SearchDieLine_ForArtworkHelper
    {
        //public static SEARCH_DIE_LINE_RESULT GetDieLine(SEARCH_DIE_LINE_REQUEST param)
        //{
        //    var listOfCheckListItem = new List<ART_WF_MOCKUP_CHECK_LIST_ITEM>();
        //    SEARCH_DIE_LINE_RESULT Results = new SEARCH_DIE_LINE_RESULT();
        //    try
        //    {
        //        List<ART_WF_MOCKUP_CHECK_LIST_2> checkList2 = new List<ART_WF_MOCKUP_CHECK_LIST_2>();
        //        using (var context = new ARTWORKEntities())
        //        {
        //            using (CNService.IsolationLevel(context))
        //            {
        //                context.Database.CommandTimeout = 300;

        //                var PRIMARY_SIZE_TXT = GetDescriptionFromCharacteristic(context, param.data.PRIMARY_SIZE_ID); PRIMARY_SIZE_TXT = Regex.Replace(PRIMARY_SIZE_TXT, @"\s+", "");
        //                var PACK_SIZE_TXT = GetDescriptionFromCharacteristic(context, param.data.PACK_SIZE_ID); PACK_SIZE_TXT = Regex.Replace(PACK_SIZE_TXT, @"\s+", "");
        //                var PACKING_STYLE_DISPLAY_TXT = param.data.PACKING_STYLE_DISPLAY_TXT; PACKING_STYLE_DISPLAY_TXT = Regex.Replace(PACKING_STYLE_DISPLAY_TXT, @"\s+", "");

        //                if (param.data.PRIMARY_TYPE_ID > 0)
        //                {
        //                    checkList2 = MapperServices.ART_WF_MOCKUP_CHECK_LIST(ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST() { PRIMARY_TYPE_ID = param.data.PRIMARY_TYPE_ID }, context));
        //                }
        //                else
        //                {
        //                    checkList2 = MapperServices.ART_WF_MOCKUP_CHECK_LIST(ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetAll(context));
        //                }
        //                foreach (var item in checkList2)
        //                {
        //                    ART_WF_MOCKUP_CHECK_LIST_PRODUCT product = new ART_WF_MOCKUP_CHECK_LIST_PRODUCT();
        //                    product.CHECK_LIST_ID = item.CHECK_LIST_ID;

        //                    var productTmp = ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(product, context).FirstOrDefault();
        //                    if (productTmp != null)
        //                    {
        //                        item.PRIMARY_SIZE_DISPLAY_TXT = productTmp.PRIMARY_SIZE;
        //                        item.PACK_SIZE_DISPLAY_TXT = productTmp.PACK_SIZE;
        //                        item.PACKING_STYLE_DISPLAY_TXT = productTmp.PACKING_STYLE;
        //                    }
        //                    else
        //                    {
        //                        if (item.THREE_P_ID > 0)
        //                            item.PRIMARY_SIZE_DISPLAY_TXT = SAP_M_3P_SERVICE.GetByTHREE_P_ID(item.THREE_P_ID, context).PRIMARY_SIZE_DESCRIPTION;
        //                        else
        //                        {
        //                            if (item.PRIMARY_SIZE_ID > 0)
        //                                item.PRIMARY_SIZE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, param.data.PRIMARY_SIZE_ID);
        //                            else
        //                                item.PRIMARY_SIZE_DISPLAY_TXT = item.PRIMARY_SIZE_OTHER;
        //                        }

        //                        if (item.TWO_P_ID > 0)
        //                        {
        //                            var temp = SAP_M_2P_SERVICE.GetByTWO_P_ID(item.TWO_P_ID, context);
        //                            if (temp != null)
        //                            {
        //                                item.PACK_SIZE_DISPLAY_TXT = temp.PACK_SIZE_DESCRIPTION;
        //                                item.PACKING_STYLE_DISPLAY_TXT = temp.PACKING_SYLE_DESCRIPTION;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (item.PACK_SIZE_ID > 0)
        //                                item.PACK_SIZE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, param.data.PACK_SIZE_ID);
        //                            else
        //                                item.PACK_SIZE_DISPLAY_TXT = item.PACK_SIZE_OTHER;

        //                            if (item.PACKING_STYLE_ID > 0)
        //                                item.PACKING_STYLE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, param.data.PACKING_STYLE_ID);
        //                            else
        //                                item.PACKING_STYLE_DISPLAY_TXT = item.PACKING_STYLE_OTHER;
        //                        }
        //                    }
        //                }

        //                if (param.data.PRIMARY_SIZE_ID > 0)
        //                {
        //                    checkList2 = checkList2.Where(m => Regex.Replace(m.PRIMARY_TYPE_DISPLAY_TXT ?? "", @"\s+", "") == PRIMARY_SIZE_TXT).ToList();
        //                }
        //                if (param.data.PACK_SIZE_ID > 0)
        //                {
        //                    checkList2 = checkList2.Where(m => Regex.Replace(m.PACK_SIZE_DISPLAY_TXT ?? "", @"\s+", "") == PACK_SIZE_TXT).ToList();
        //                }
        //                if (!string.IsNullOrEmpty(PACKING_STYLE_DISPLAY_TXT))
        //                {
        //                    checkList2 = checkList2.Where(m => Regex.Replace(m.PACKING_STYLE_DISPLAY_TXT ?? "", @"\s+", "") == PACKING_STYLE_DISPLAY_TXT).ToList();
        //                }

        //                foreach (var item in checkList2)
        //                {
        //                    var checkListItemFilter = new ART_WF_MOCKUP_CHECK_LIST_ITEM();
        //                    checkListItemFilter.CHECK_LIST_ID = item.CHECK_LIST_ID;
        //                    checkListItemFilter.PACKING_TYPE_ID = param.data.PACKING_TYPE_ID;
        //                    listOfCheckListItem.AddRange(ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByItem(checkListItemFilter, context));
        //                }

        //                List<SEARCH_DIE_LINE> listSearchDieLine = new List<SEARCH_DIE_LINE>();
        //                foreach (var item in listOfCheckListItem)
        //                {
        //                    var searchDieLine = GetDieLine(item, context);
        //                    if (searchDieLine != null)
        //                        listSearchDieLine.Add(searchDieLine);
        //                }

        //                listSearchDieLine = listSearchDieLine.OrderBy(m => m.CHECK_LIST_NO).ThenBy(m => m.MOCKUP_NO).ToList();
        //                Results.data = listSearchDieLine;
        //            }
        //        }

        //        Results.status = "S";
        //    }
        //    catch (Exception ex)
        //    {
        //        Results.status = "E";
        //        Results.msg = CNService.GetErrorMessage(ex);
        //    }

        //    return Results;
        //}

        //public static SEARCH_DIE_LINE GetDieLine(ART_WF_MOCKUP_CHECK_LIST_ITEM item, ARTWORKEntities context)
        //{
        //    SEARCH_DIE_LINE searchDieLine = new SEARCH_DIE_LINE();

        //    searchDieLine.CHECK_LIST_ID = item.CHECK_LIST_ID;
        //    var checkList = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(item.CHECK_LIST_ID, context);
        //    if (checkList == null)
        //    {
        //        return null;
        //    }
        //    searchDieLine.CHECK_LIST_NO = checkList.CHECK_LIST_NO;
        //    searchDieLine.MOCKUP_ID = item.MOCKUP_ID;
        //    searchDieLine.MOCKUP_NO = item.MOCKUP_NO;

        //    searchDieLine.PRIMARY_TYPE_ID = checkList.PRIMARY_TYPE_ID;
        //    searchDieLine.PRIMARY_SIZE_ID = checkList.PRIMARY_SIZE_ID;
        //    searchDieLine.PACK_SIZE_ID = checkList.PACK_SIZE_ID;
        //    searchDieLine.PACKING_STYLE_ID = checkList.PACKING_STYLE_ID;
        //    searchDieLine.PACKING_TYPE_ID = item.PACKING_TYPE_ID;

        //    searchDieLine.PRIMARY_TYPE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, checkList.PRIMARY_TYPE_ID);

        //    if (checkList.PRIMARY_SIZE_ID > 0)
        //        searchDieLine.PRIMARY_SIZE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, checkList.PRIMARY_SIZE_ID);
        //    if (checkList.PACK_SIZE_ID > 0)
        //        searchDieLine.PACK_SIZE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, checkList.PACK_SIZE_ID);
        //    if (checkList.PACKING_STYLE_ID > 0)
        //        searchDieLine.PACKING_STYLE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, checkList.PACKING_STYLE_ID);
        //    if (item.PACKING_TYPE_ID > 0)
        //        searchDieLine.PACKAGING_TYPE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, item.PACKING_TYPE_ID);


        //    if (checkList.THREE_P_ID > 0)
        //        searchDieLine.PRIMARY_SIZE_DISPLAY_TXT = SAP_M_3P_SERVICE.GetByTHREE_P_ID(checkList.THREE_P_ID, context).PRIMARY_SIZE_DESCRIPTION;

        //    if (checkList.TWO_P_ID > 0)
        //    {
        //        searchDieLine.PACK_SIZE_DISPLAY_TXT = SAP_M_2P_SERVICE.GetByTWO_P_ID(checkList.TWO_P_ID, context).PACK_SIZE_DESCRIPTION;
        //        searchDieLine.PACKING_STYLE_DISPLAY_TXT = SAP_M_2P_SERVICE.GetByTWO_P_ID(checkList.TWO_P_ID, context).PACKING_SYLE_DESCRIPTION;
        //    }

        //    var process = (from p in context.ART_WF_MOCKUP_PROCESS
        //                   where p.MOCKUP_ID == item.MOCKUP_ID
        //                   select p).FirstOrDefault();

        //    if (process != null)
        //    {
        //        var processPG = (from pg in context.ART_WF_MOCKUP_PROCESS_PG
        //                         where pg.MOCKUP_SUB_ID == process.MOCKUP_SUB_ID
        //                         select pg).OrderByDescending(o => o.MOCKUP_SUB_PG_ID).FirstOrDefault();

        //        var style = (from g in context.SAP_M_CHARACTERISTIC_ITEM
        //                     where g.CHARACTERISTIC_ITEM_ID == item.STYLE_ID
        //                     select g).FirstOrDefault();

        //        var step = (from g in context.ART_M_STEP_MOCKUP
        //                    where g.STEP_MOCKUP_ID == process.CURRENT_STEP_ID
        //                    select g).FirstOrDefault();

        //        if (processPG != null)
        //        {
        //            if (processPG.VENDOR != null)
        //            {
        //                searchDieLine.VENDOR_DISPLAY_TXT = CNService.GetVendorName(processPG.VENDOR, context);
        //            }

        //            if (processPG.GRADE_OF != null)
        //            {
        //                searchDieLine.GRADE_OF_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, processPG.GRADE_OF);
        //            }

        //            if (processPG.FLUTE != null)
        //            {
        //                searchDieLine.FLUTE_DISPLAY_TXT = GetDescriptionFromCharacteristic(context, processPG.FLUTE);
        //            }

        //            //searchDieLine.SIZE_DISPLAY_TXT = processPG.ID_MM;
        //        }

        //        if (style != null)
        //        {
        //            searchDieLine.STYLE_DISPLAY_TXT = style.DESCRIPTION;
        //        }

        //        if (step != null)
        //        {
        //            searchDieLine.STATUS_DISPLAY_TXT = step.STEP_MOCKUP_NAME;
        //        }
        //    }

        //    return searchDieLine;
        //}

        //private static string GetDescriptionFromCharacteristic(ARTWORKEntities context, Nullable<int> id)
        //{
        //    if (id == null)
        //    { return ""; }

        //    var desc = (from g in context.SAP_M_CHARACTERISTIC
        //                where g.CHARACTERISTIC_ID == id
        //                select g).FirstOrDefault();
        //    if (desc == null) return "";
        //    else return desc.DESCRIPTION;
        //}

        //private static SEARCH_DIE_LINE_REQUEST GetRequestData(ARTWORKEntities context, SEARCH_DIE_LINE_REQUEST param)
        //{
        //    SEARCH_DIE_LINE_REQUEST _request = new SEARCH_DIE_LINE_REQUEST();

        //    return _request;
        //}
    }
}
