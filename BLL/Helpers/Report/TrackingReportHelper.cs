using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;
using BLL.Services;
using DAL;
using System.Globalization;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;
namespace BLL.Helpers
{
    public class TrackingReportHelper
    {
        public static TRACKING_REPORT_RESULT GetTrackingReport(TRACKING_REPORT_REQUEST param)
        {
            TRACKING_REPORT_RESULT Results = new TRACKING_REPORT_RESULT();

            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<TRACKING_REPORT>();
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                var listResult = new List<TRACKING_REPORT>();
                var msg = MessageHelper.GetMessage("MSG_005");

                var searchOnlyMockup = false;
                var searchOnlyArtwork = false;
                if (!string.IsNullOrEmpty(param.data.SEARCH_SO))
                {
                    searchOnlyArtwork = true;
                }

                if (!string.IsNullOrEmpty(param.data.SEARCH_ORDER_BOM))
                {
                    searchOnlyArtwork = true;
                }

                if (!string.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE))
                {
                    if (param.data.CURRENT_STEP_WF_TYPE.StartsWith("A"))
                        searchOnlyArtwork = true;
                    else if (param.data.CURRENT_STEP_WF_TYPE.StartsWith("M"))
                        searchOnlyMockup = true;
                }

                List<V_ART_WF_DASHBOARD> allMockupTrans = new List<V_ART_WF_DASHBOARD>();
                List<V_ART_WF_DASHBOARD_ARTWORK> allArtworkTrans = new List<V_ART_WF_DASHBOARD_ARTWORK>();

                if (!searchOnlyArtwork)
                    allMockupTrans = QueryTrackingReportMockup(param, false);

                if (!searchOnlyMockup)
                    allArtworkTrans = QueryTrackingReportArtwork(param, false);

                Results.recordsTotal = allMockupTrans.Count + allArtworkTrans.Count;

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var allStepMockup = ART_M_STEP_MOCKUP_SERVICE.GetAll(context);
                        var allStepArtwork = ART_M_STEP_ARTWORK_SERVICE.GetAll(context);

                        var tempMOCKUP_ID = allMockupTrans.Select(m => m.MOCKUP_ID);
                        var listDashboardMockup = (from p in context.V_ART_WF_DASHBOARD
                                                   where tempMOCKUP_ID.Contains(p.MOCKUP_ID)
                                                   select p).ToList();
                        foreach (var item in allMockupTrans)
                        {
                            TRACKING_REPORT itemResult = new TRACKING_REPORT();
                            itemResult.CHECK_LIST_ID = item.CHECK_LIST_ID;
                            itemResult.MOCKUP_SUB_ID = item.MOCKUP_SUB_ID;

                            if (item.MOCKUP_SUB_ID == 0)
                            {
                                if (!String.IsNullOrEmpty(item.CHECK_LIST_NO))
                                {
                                    itemResult.WORKFLOW_NUMBER = item.CHECK_LIST_NO;
                                }
                            }
                            else
                            {
                                itemResult.WORKFLOW_NUMBER = item.MOCKUP_NO;
                            }

                            var PACKING_TYPE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PACKING_TYPE_ID, context);
                            if (PACKING_TYPE != null) itemResult.PACKING_TYPE_DISPLAY_TXT = PACKING_TYPE.VALUE + ":" + PACKING_TYPE.DESCRIPTION;
                            itemResult.PIC_DISPLAY_TXT = item.CREATE_BY_CHECK_LIST_TITLE + " " + item.CREATE_BY_CHECK_LIST_FIRST_NAME + " " + item.CREATE_BY_CHECK_LIST_LAST_NAME;
                            if (item.IS_END == "X")
                            {
                                if (item.IS_TERMINATE == "X")
                                {
                                    itemResult.CURRENT_STEP_DISPLAY_TXT = "Terminated";
                                }
                                else
                                {
                                    itemResult.CURRENT_STEP_DISPLAY_TXT = "Completed";
                                }
                            }
                            else
                            {
                                itemResult.CURRENT_STEP_DISPLAY_TXT = "In progress";
                            }

                            itemResult.BRAND_DISPLAY_TXT = item.BRAND_DISPLAY_TXT;
                            itemResult.SOLD_TO_DISPLAY_TXT = item.SOLD_TO_DISPLAY_TXT;
                            itemResult.SHIP_TO_DISPLAY_TXT = item.SHIP_TO_DISPLAY_TXT;
                            itemResult.RDD = item.REQUEST_DELIVERY_DATE;
                            itemResult.VENDOR_RFQ = CNService.GetVendorRFQ(item.MOCKUP_ID, item.MOCKUP_SUB_ID, context);
                            itemResult.SELECTED_VENDOR = CNService.GetVendorSelected(item.MOCKUP_ID, item.MOCKUP_SUB_ID, context);

                            string table = "";
                            if (item.MOCKUP_ID > 0)
                            {
                                var temp_dashboard = listDashboardMockup.Where(m => m.MOCKUP_ID == item.MOCKUP_ID).ToList();
                                var list_waiting = temp_dashboard.Where(m => string.IsNullOrEmpty(m.IS_END)).ToList();

                                //if (!string.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE))
                                //{
                                //    if (param.data.CURRENT_STEP_WF_TYPE.StartsWith("M"))
                                //    {
                                //        var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP_WF_TYPE.Replace("M", ""));
                                //        list_waiting = list_waiting.Where(m => m.CURRENT_STEP_ID == current_step_id).ToList();
                                //    }
                                //}
                                //if (param.data.CURRENT_ASSIGN_ID > 0)
                                //{
                                //    list_waiting = list_waiting.Where(m => m.CURRENT_USER_ID == param.data.CURRENT_ASSIGN_ID).ToList();
                                //}

                                //if (param.data.WORKING_GROUP_ID > 0)
                                //{
                                //    list_waiting = list_waiting.Where(m => m.CURRENT_USER_ID == param.data.CURRENT_ASSIGN_ID).ToList();
                                //}

                                if (list_waiting.Count > 0)
                                    table += "<table class='cls_table_in_table' style='min-width:750px;'>";

                                foreach (var item_waiting in list_waiting)
                                {
                                    table += "<tr>";
                                    var tempMockup = allStepMockup.Where(m => m.STEP_MOCKUP_ID == item_waiting.CURRENT_STEP_ID).FirstOrDefault();
                                    if (tempMockup != null)
                                    {
                                        table += "<td style='max-width:330px;width:330px;min-width:330px;white-space:initial;'>";
                                        table += "<a target='_blank' href='" + System.Configuration.ConfigurationManager.AppSettings["suburl"] + "/TaskForm/" + item_waiting.MOCKUP_SUB_ID + "'>" + tempMockup.STEP_MOCKUP_NAME + "</a>";

                                        if (CNService.GetUserName(item_waiting.CURRENT_USER_ID, context) != "")
                                            table += "<td style='max-width:250px;width:250px;min-width:250px;white-space:initial;'>" + CNService.GetUserName(item_waiting.CURRENT_USER_ID, context) + "</td>";
                                        else
                                            table += "<td style='max-width:250px;width:250px;min-width:250px;white-space:initial;'>" + msg + " </td>";
                                    }

                                    if (item_waiting.CREATE_DATE_PROCESS != null)
                                    {
                                        DateTime? dtReceiveWf = item_waiting.CREATE_DATE_PROCESS;
                                        DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item_waiting.DURATION.Value));

                                        if (!String.IsNullOrEmpty(item_waiting.IS_STEP_DURATION_EXTEND) && item_waiting.IS_STEP_DURATION_EXTEND.Equals("X"))
                                        {
                                            dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item_waiting.DURATION_EXTEND.Value));
                                        }

                                        table += "<td style='text-align:right;max-width:170px;width:170px;min-width:170px;white-space:initial;'>" + CNService.GetWorkingDays(DateTime.Now, dtWillFinish.Value) + " [" + dtWillFinish.Value.ToString("dd/MM/yyyy HH:mm") + "]" + "</td>";
                                    }

                                    table += "</tr>";
                                }

                                if (list_waiting.Count > 0)
                                    itemResult.CURRENT_STATUS_DISPLAY_TXT = table;
                            }

                            var tempProduct = MapperServices.ART_WF_MOCKUP_CHECK_LIST_PRODUCT(ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_PRODUCT() { CHECK_LIST_ID = item.CHECK_LIST_ID }, context));
                            itemResult.PRODUCT_CODE_DISPLAY_TXT = tempProduct.Aggregate("", (a, b) => a + ((a.Length > 0 && b.PRODUCT_CODE != null && b.PRODUCT_CODE.Length > 0) ? "<br/>" : "") + b.PRODUCT_CODE);

                            var tempRD = MapperServices.ART_WF_MOCKUP_CHECK_LIST_REFERENCE(ART_WF_MOCKUP_CHECK_LIST_REFERENCE_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_REFERENCE() { CHECK_LIST_ID = item.CHECK_LIST_ID }, context));
                            itemResult.RD_NUMBER_DISPLAY_TXT = tempRD.Aggregate("", (a, b) => a + ((a.Length > 0 && b.REFERENCE_NO != null && b.REFERENCE_NO.Length > 0) ? "<br/>" : "") + b.REFERENCE_NO);

                            //if (item.PRIMARY_SIZE_ID > 0)
                            //{
                            //    var PRIMARY_SIZE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PRIMARY_SIZE_ID, context);
                            //    if (PRIMARY_SIZE != null)
                            //        itemResult.PRIMARY_SIZE_DISPLAY_TXT = PRIMARY_SIZE.DESCRIPTION;
                            //}
                            //else
                            //{
                            //    var temp = SAP_M_3P_SERVICE.GetByTHREE_P_ID(item.THREE_P_ID, context);
                            //    if (temp != null)
                            //        itemResult.PRIMARY_SIZE_DISPLAY_TXT = temp.PRIMARY_SIZE_DESCRIPTION;
                            //}

                            //if (string.IsNullOrEmpty(itemResult.PRIMARY_SIZE_DISPLAY_TXT))
                            //{
                            //    var temp = ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_PRODUCT() { CHECK_LIST_ID = item.CHECK_LIST_ID }, context).FirstOrDefault();
                            //    if (temp != null) itemResult.PRIMARY_SIZE_DISPLAY_TXT = temp.PRIMARY_SIZE;
                            //}

                            if (item.PRIMARY_TYPE_ID > 0)
                            {
                                var PRIMARY_TYPE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PRIMARY_TYPE_ID, context);
                                if (PRIMARY_TYPE != null)
                                    itemResult.PRIMARY_TYPE_DISPLAY_TXT = PRIMARY_TYPE.DESCRIPTION;
                            }

                            listResult.Add(itemResult);
                        }

                        var tempARTWORK_ITEM_ID = allArtworkTrans.Select(m => m.ARTWORK_ITEM_ID);
                        var tempARTWORK_SUB_ID = allArtworkTrans.Select(m => m.ARTWORK_SUB_ID);
                        var listDashboardArtwork = (from p in context.V_ART_WF_DASHBOARD_ARTWORK
                                                    where tempARTWORK_ITEM_ID.Contains(p.ARTWORK_ITEM_ID)
                                                    select p).ToList();
                        var listAssignedSOHeader = (from p in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
                                                    where tempARTWORK_SUB_ID.Contains(p.ARTWORK_SUB_ID)
                                                    select p).ToList();
                        var listAssignedSOItem = (from p in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM
                                                  where tempARTWORK_SUB_ID.Contains(p.ARTWORK_SUB_ID)
                                                  select p).ToList();
                        foreach (var item in allArtworkTrans)
                        {
                            TRACKING_REPORT itemResult = new TRACKING_REPORT();

                            itemResult.CHECK_LIST_ID = item.ARTWORK_REQUEST_ID;
                            itemResult.MOCKUP_SUB_ID = item.ARTWORK_SUB_ID;

                            if (item.ARTWORK_SUB_ID == 0)
                            {
                                if (!String.IsNullOrEmpty(item.ARTWORK_REQUEST_NO))
                                {
                                    itemResult.WORKFLOW_NUMBER = item.ARTWORK_REQUEST_NO;
                                }
                            }
                            else
                            {
                                itemResult.WORKFLOW_NUMBER = item.REQUEST_ITEM_NO;
                            }

                            var processPA = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PA() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                            if (processPA != null)
                            {
                                var PACKING_TYPE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(processPA.MATERIAL_GROUP_ID, context);
                                if (PACKING_TYPE != null) itemResult.PACKING_TYPE_DISPLAY_TXT = PACKING_TYPE.VALUE + ":" + PACKING_TYPE.DESCRIPTION;
                            }

                            //itemResult.PIC_DISPLAY_TXT = item.CREATE_BY_ARTWORK_REQUEST_TITLE + " " + item.CREATE_BY_ARTWORK_REQUEST_FIRST_NAME + " " + item.CREATE_BY_ARTWORK_REQUEST_LAST_NAME;
                            itemResult.PIC_DISPLAY_TXT = CNService.GetUserName(ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(item.ARTWORK_REQUEST_ID, context).CREATOR_ID, context);

                            if (item.IS_END == "X")
                            {
                                if (item.IS_TERMINATE == "X")
                                {
                                    itemResult.CURRENT_STEP_DISPLAY_TXT = "Terminated";
                                }
                                else
                                {
                                    itemResult.CURRENT_STEP_DISPLAY_TXT = "Completed";
                                }
                            }
                            else
                            {
                                itemResult.CURRENT_STEP_DISPLAY_TXT = "In progress";
                            }

                            itemResult.BRAND_DISPLAY_TXT = item.BRAND_DISPLAY_TXT;
                            itemResult.SOLD_TO_DISPLAY_TXT = item.SOLD_TO_DISPLAY_TXT;
                            itemResult.SHIP_TO_DISPLAY_TXT = item.SHIP_TO_DISPLAY_TXT;
                            itemResult.RDD = item.REQUEST_DELIVERY_DATE;
                            //itemResult.VENDOR_RFQ = CNService.GetVendorRFQ(item.MOCKUP_ID, item.MOCKUP_SUB_ID);
                            //itemResult.SELECTED_VENDOR = CNService.GetVendorSelected(item.MOCKUP_ID, item.MOCKUP_SUB_ID);



                            string table = "";
                            if (item.ARTWORK_ITEM_ID > 0)
                            {
                                var temp_dashboard = listDashboardArtwork.Where(m => m.ARTWORK_ITEM_ID == item.ARTWORK_ITEM_ID).ToList();
                                var list_waiting = temp_dashboard.Where(m => string.IsNullOrEmpty(m.IS_END)).ToList();

                                //if (!string.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE))
                                //{
                                //    if (param.data.CURRENT_STEP_WF_TYPE.StartsWith("A"))
                                //    {
                                //        var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP_WF_TYPE.Replace("A", ""));
                                //        list_waiting = list_waiting.Where(m => m.CURRENT_STEP_ID == current_step_id).ToList();
                                //    }
                                //}
                                //if (param.data.CURRENT_ASSIGN_ID > 0)
                                //{
                                //    list_waiting = list_waiting.Where(m => m.CURRENT_USER_ID == param.data.CURRENT_ASSIGN_ID).ToList();
                                //}

                                //if (param.data.WORKING_GROUP_ID > 0)
                                //{
                                //    list_waiting = list_waiting.Where(m => m.CURRENT_USER_ID == param.data.CURRENT_ASSIGN_ID).ToList();
                                //}

                                if (list_waiting.Count > 0)
                                    table += "<table class='cls_table_in_table' style='min-width:750px;'>";

                                foreach (var item_waiting in list_waiting)
                                {
                                    table += "<tr>";
                                    var tempMockup = allStepArtwork.Where(m => m.STEP_ARTWORK_ID == item_waiting.CURRENT_STEP_ID).FirstOrDefault();
                                    if (tempMockup != null)
                                    {
                                        table += "<td style='max-width:330px;width:330px;min-width:330px;white-space:initial;'>";
                                        table += "<a target='_blank' href='" + System.Configuration.ConfigurationManager.AppSettings["suburl"] + "/TaskFormArtwork/" + item_waiting.ARTWORK_SUB_ID + "'>" + tempMockup.STEP_ARTWORK_NAME + "</a>";

                                        if (CNService.GetUserName(item_waiting.CURRENT_USER_ID, context) != "")
                                            table += "<td style='max-width:250px;width:250px;min-width:250px;white-space:initial;'>" + CNService.GetUserName(item_waiting.CURRENT_USER_ID, context) + "</td>";
                                        else
                                            table += "<td style='max-width:250px;width:250px;min-width:250px;white-space:initial;'>" + msg + " </td>";
                                    }

                                    if (item_waiting.CREATE_DATE_PROCESS != null)
                                    {
                                        DateTime? dtReceiveWf = item_waiting.CREATE_DATE_PROCESS;
                                        DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item_waiting.DURATION.Value));

                                        if (!String.IsNullOrEmpty(item_waiting.IS_STEP_DURATION_EXTEND) && item_waiting.IS_STEP_DURATION_EXTEND.Equals("X"))
                                        {
                                            dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item_waiting.DURATION_EXTEND.Value));
                                        }

                                        table += "<td style='text-align:right;max-width:170px;width:170px;min-width:170px;white-space:initial;'>" + CNService.GetWorkingDays(DateTime.Now, dtWillFinish.Value) + " [" + dtWillFinish.Value.ToString("dd/MM/yyyy HH:mm") + "]" + "</td>";
                                    }

                                    table += "</tr>";
                                }

                                if (list_waiting.Count > 0)
                                    itemResult.CURRENT_STATUS_DISPLAY_TXT = table;
                            }

                            var tempProduct = MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCT(ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_PRODUCT() { ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID }, context));
                            foreach (var itemProduct in tempProduct)
                            {
                                var temp = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(itemProduct.PRODUCT_CODE_ID, context);
                                if (temp != null)
                                    if (string.IsNullOrEmpty(itemResult.PRODUCT_CODE_DISPLAY_TXT))
                                        itemResult.PRODUCT_CODE_DISPLAY_TXT = temp.PRODUCT_CODE;
                                    else
                                        itemResult.PRODUCT_CODE_DISPLAY_TXT += "<br/>" + temp.PRODUCT_CODE;
                            }

                            var tempRD = MapperServices.ART_WF_ARTWORK_REQUEST_REFERENCE(ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_REFERENCE() { ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID }, context));
                            itemResult.RD_NUMBER_DISPLAY_TXT = tempRD.Aggregate("", (a, b) => a + ((a.Length > 0 && b.REFERENCE_NO != null && b.REFERENCE_NO.Length > 0) ? "<br/>" : "") + b.REFERENCE_NO);

                            //if (item.PRIMARY_SIZE_ID > 0)
                            //{
                            //    var PRIMARY_SIZE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PRIMARY_SIZE_ID, context);
                            //    if (PRIMARY_SIZE != null)
                            //        itemResult.PRIMARY_SIZE_DISPLAY_TXT = PRIMARY_SIZE.DESCRIPTION;
                            //}
                            //else
                            //{
                            //    var temp = SAP_M_3P_SERVICE.GetByTHREE_P_ID(item.THREE_P_ID, context);
                            //    if (temp != null)
                            //        itemResult.PRIMARY_SIZE_DISPLAY_TXT = temp.PRIMARY_SIZE_DESCRIPTION;
                            //}

                            //if (string.IsNullOrEmpty(itemResult.PRIMARY_SIZE_DISPLAY_TXT))
                            //{
                            //    var temp = ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_PRODUCT() { CHECK_LIST_ID = item.CHECK_LIST_ID }, context).FirstOrDefault();
                            //    if (temp != null) itemResult.PRIMARY_SIZE_DISPLAY_TXT = temp.PRIMARY_SIZE;
                            //}

                            if (item.PRIMARY_TYPE_ID > 0)
                            {
                                var PRIMARY_TYPE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PRIMARY_TYPE_ID, context);
                                if (PRIMARY_TYPE != null)
                                    itemResult.PRIMARY_TYPE_DISPLAY_TXT = PRIMARY_TYPE.DESCRIPTION;
                            }

                            //sales order information
                            var tempListAssignedSOHeader = listAssignedSOHeader.Where(m => m.ARTWORK_SUB_ID == item.ARTWORK_SUB_ID).ToList();
                            var tempListAssignedSOItem = listAssignedSOItem.Where(m => m.ARTWORK_SUB_ID == item.ARTWORK_SUB_ID).ToList();


                            var soAssign = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                            where s.ARTWORK_SUB_ID == item.ARTWORK_SUB_ID
                                            select s).Select(x => new { x.SALES_ORDER_NO, x.SALES_ORDER_ITEM }).ToList();

                            List<string> listAssignSO = new List<string>();
                            if (soAssign != null && soAssign.Count > 0)
                            {
                                foreach (var itemSO in soAssign)
                                {
                                    if (!String.IsNullOrEmpty(itemSO.SALES_ORDER_NO) && !String.IsNullOrEmpty(itemSO.SALES_ORDER_NO))
                                    {
                                        listAssignSO.Add(itemSO.SALES_ORDER_NO + "(" + itemSO.SALES_ORDER_ITEM + ")");
                                    }
                                }
                            }


                            itemResult.IN_TRANSIT_TO_DISPLAY_TXT = CNService.ConcatArray(tempListAssignedSOItem.Select(m => m.IN_TRANSIT_TO).ToArray());

                            if (param.data.GENERATE_EXCEL != "X")
                            {
                                if (listAssignSO != null && listAssignSO.Count > 0)
                                {
                                    itemResult.SALES_ORDER_NO = CNService.ConcatArrayEnterLine(listAssignSO.ToArray());
                                }
                                else
                                {
                                    itemResult.SALES_ORDER_NO = CNService.ConcatArrayEnterLine(tempListAssignedSOHeader.Select(m => m.SALES_ORDER_NO).ToArray());
                                }
                            }
                            else
                            {
                                if (listAssignSO != null && listAssignSO.Count > 0)
                                {
                                    itemResult.SALES_ORDER_NO = CNService.ConcatArray(listAssignSO.ToArray());
                                }
                                else
                                {
                                    itemResult.SALES_ORDER_NO = CNService.ConcatArray(tempListAssignedSOHeader.Select(m => m.SALES_ORDER_NO).ToArray());
                                }
                            }

                            itemResult.SALES_ORDER_CREATE_DATE = CNService.ConcatArray(tempListAssignedSOHeader.Select(m => m.CREATE_ON).Distinct().ToArray());
                            //itemResult.SALES_ORDER_ITEM = CNService.ConcatArray(tempListAssignedSOItem.Select(m => m.ITEM).ToArray());
                            itemResult.ADDITIONAL_BRAND_DISPLAY_TXT = CNService.ConcatArray(tempListAssignedSOItem.Select(m => m.ADDITIONAL_BRAND_DESCRIPTION).Distinct().ToArray());
                            itemResult.PRODUCTION_MEMO_DISPLAY_TXT = CNService.ConcatArray(tempListAssignedSOItem.Select(m => m.PROD_INSP_MEMO).Distinct().ToArray());
                            itemResult.ROUTE_DESC_DISPLAY_TXT = CNService.ConcatArray(tempListAssignedSOItem.Select(m => m.PORT).Distinct().ToArray());

                            listResult.Add(itemResult);
                        }
                    }
                }

                var orderColumn = 1;
                var orderDir = "asc";
                if (param.order != null && param.order.Count > 0)
                {
                    orderColumn = param.order[0].column;
                    orderDir = param.order[0].dir; //desc ,asc
                }

                if (param.data.VIEW == "PG")
                {
                    listResult = FilterDataPGView(listResult, param);
                    listResult = OrderByDataPGView(listResult, orderColumn, orderDir);
                }
                else if (param.data.VIEW == "MK")
                {
                    listResult = FilterDataMKView(listResult, param);
                    listResult = OrderByDataMKView(listResult, orderColumn, orderDir);
                }

                Results.recordsFiltered = listResult.Count;
                listResult = listResult.Skip(param.start).Take(param.length).ToList();

                Results.status = "S";
                Results.data = listResult;
                Results.draw = param.draw;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        private static List<TRACKING_REPORT> FilterDataPGView(List<TRACKING_REPORT> data, TRACKING_REPORT_REQUEST param)
        {
            var filterValue = param.columns[1].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PIC_DISPLAY_TXT) && m.PIC_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[2].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.WORKFLOW_NUMBER) && m.WORKFLOW_NUMBER.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[3].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PACKING_TYPE_DISPLAY_TXT) && m.PACKING_TYPE_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[4].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.CURRENT_STEP_DISPLAY_TXT) && m.CURRENT_STEP_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[5].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.CURRENT_STATUS_DISPLAY_TXT) && m.CURRENT_STATUS_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[6].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SOLD_TO_DISPLAY_TXT) && m.SOLD_TO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[7].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SHIP_TO_DISPLAY_TXT) && m.SHIP_TO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[8].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.IN_TRANSIT_TO_DISPLAY_TXT) && m.IN_TRANSIT_TO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[9].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SALES_ORDER_NO) && m.SALES_ORDER_NO.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[10].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SALES_ORDER_CREATE_DATE) && m.SALES_ORDER_CREATE_DATE.ToLower().Contains(filterValue.ToLower())).ToList();

            //filterValue = param.columns[11].search.value;
            //if (!string.IsNullOrEmpty(filterValue))
            //    data = data.Where(m => !string.IsNullOrEmpty(m.SALES_ORDER_ITEM) && m.SALES_ORDER_ITEM.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[11].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.BRAND_DISPLAY_TXT) && m.BRAND_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[12].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.ADDITIONAL_BRAND_DISPLAY_TXT) && m.ADDITIONAL_BRAND_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[13].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PRODUCT_CODE_DISPLAY_TXT) && m.PRODUCT_CODE_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[14].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PRODUCTION_MEMO_DISPLAY_TXT) && m.PRODUCTION_MEMO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[15].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.RD_NUMBER_DISPLAY_TXT) && m.RD_NUMBER_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[16].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => m.RDD != null && m.RDD.Value.ToString("dd/MM/yyyy").ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[17].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.VENDOR_RFQ) && m.VENDOR_RFQ.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[18].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SELECTED_VENDOR) && m.SELECTED_VENDOR.ToLower().Contains(filterValue.ToLower())).ToList();

            return data;
        }

        private static List<TRACKING_REPORT> OrderByDataPGView(List<TRACKING_REPORT> data, int orderColumn, string orderDir)
        {
            string orderASC = "asc";
            string orderDESC = "desc";

            if (orderColumn == 1)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PIC_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PIC_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 2)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.WORKFLOW_NUMBER).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.WORKFLOW_NUMBER).ToList();
            }

            if (orderColumn == 3)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PACKING_TYPE_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PACKING_TYPE_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 4)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.CURRENT_STEP_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.CURRENT_STEP_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 5)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.CURRENT_STATUS_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.CURRENT_STATUS_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 6)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.SOLD_TO_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.SOLD_TO_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 7)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.SHIP_TO_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.SHIP_TO_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 8)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.IN_TRANSIT_TO_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.IN_TRANSIT_TO_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 9)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.SALES_ORDER_NO).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.SALES_ORDER_NO).ToList();
            }

            if (orderColumn == 10)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.SALES_ORDER_CREATE_DATE).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.SALES_ORDER_CREATE_DATE).ToList();
            }

            //if (orderColumn == 11)
            //{
            //    if (orderDir == orderASC)
            //        data = data.OrderBy(i => i.SALES_ORDER_ITEM).ToList();
            //    else if (orderDir == orderDESC)
            //        data = data.OrderByDescending(i => i.SALES_ORDER_ITEM).ToList();
            //}

            if (orderColumn == 11)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.BRAND_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.BRAND_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 12)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.ADDITIONAL_BRAND_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.ADDITIONAL_BRAND_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 13)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PRODUCT_CODE_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PRODUCT_CODE_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 14)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PRODUCTION_MEMO_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PRODUCTION_MEMO_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 15)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.RD_NUMBER_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.RD_NUMBER_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 16)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.RDD).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.RDD).ToList();
            }

            if (orderColumn == 17)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.VENDOR_RFQ).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.VENDOR_RFQ).ToList();
            }

            if (orderColumn == 18)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.SELECTED_VENDOR).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.SELECTED_VENDOR).ToList();
            }

            return data;
        }

        private static List<TRACKING_REPORT> FilterDataMKView(List<TRACKING_REPORT> data, TRACKING_REPORT_REQUEST param)
        {
            var filterValue = param.columns[1].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PIC_DISPLAY_TXT) && m.PIC_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[2].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.WORKFLOW_NUMBER) && m.WORKFLOW_NUMBER.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[3].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PACKING_TYPE_DISPLAY_TXT) && m.PACKING_TYPE_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[4].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PRIMARY_TYPE_DISPLAY_TXT) && m.PRIMARY_TYPE_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[5].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.CURRENT_STEP_DISPLAY_TXT) && m.CURRENT_STEP_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[6].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.CURRENT_STATUS_DISPLAY_TXT) && m.CURRENT_STATUS_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[7].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SOLD_TO_DISPLAY_TXT) && m.SOLD_TO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[8].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SHIP_TO_DISPLAY_TXT) && m.SHIP_TO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[9].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.ROUTE_DESC_DISPLAY_TXT) && m.ROUTE_DESC_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[10].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.IN_TRANSIT_TO_DISPLAY_TXT) && m.IN_TRANSIT_TO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[11].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SALES_ORDER_NO) && m.SALES_ORDER_NO.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[12].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SALES_ORDER_CREATE_DATE) && m.SALES_ORDER_CREATE_DATE.ToLower().Contains(filterValue.ToLower())).ToList();

            //filterValue = param.columns[13].search.value;
            //if (!string.IsNullOrEmpty(filterValue))
            //    data = data.Where(m => !string.IsNullOrEmpty(m.SALES_ORDER_ITEM) && m.SALES_ORDER_ITEM.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[13].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.BRAND_DISPLAY_TXT) && m.BRAND_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[14].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.ADDITIONAL_BRAND_DISPLAY_TXT) && m.ADDITIONAL_BRAND_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[15].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PRODUCT_CODE_DISPLAY_TXT) && m.PRODUCT_CODE_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[16].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PRODUCTION_MEMO_DISPLAY_TXT) && m.PRODUCTION_MEMO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[17].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.RD_NUMBER_DISPLAY_TXT) && m.RD_NUMBER_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[18].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => m.RDD != null && m.RDD.Value.ToString("dd/MM/yyyy").ToLower().Contains(filterValue.ToLower())).ToList();

            return data;
        }

        private static List<TRACKING_REPORT> OrderByDataMKView(List<TRACKING_REPORT> data, int orderColumn, string orderDir)
        {
            string orderASC = "asc";
            string orderDESC = "desc";

            if (orderColumn == 1)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PIC_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PIC_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 2)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.WORKFLOW_NUMBER).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.WORKFLOW_NUMBER).ToList();
            }

            if (orderColumn == 3)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PACKING_TYPE_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PACKING_TYPE_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 4)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PRIMARY_TYPE_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PRIMARY_TYPE_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 5)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.CURRENT_STEP_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.CURRENT_STEP_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 6)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.CURRENT_STATUS_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.CURRENT_STATUS_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 7)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.SOLD_TO_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.SOLD_TO_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 8)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.SHIP_TO_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.SHIP_TO_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 9)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.ROUTE_DESC_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.ROUTE_DESC_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 10)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.IN_TRANSIT_TO_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.IN_TRANSIT_TO_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 11)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.SALES_ORDER_NO).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.SALES_ORDER_NO).ToList();
            }

            if (orderColumn == 12)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.SALES_ORDER_CREATE_DATE).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.SALES_ORDER_CREATE_DATE).ToList();
            }

            //if (orderColumn == 13)
            //{
            //    if (orderDir == orderASC)
            //        data = data.OrderBy(i => i.SALES_ORDER_ITEM).ToList();
            //    else if (orderDir == orderDESC)
            //        data = data.OrderByDescending(i => i.SALES_ORDER_ITEM).ToList();
            //}

            if (orderColumn == 13)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.BRAND_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.BRAND_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 14)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.ADDITIONAL_BRAND_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.ADDITIONAL_BRAND_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 15)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PRODUCT_CODE_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PRODUCT_CODE_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 16)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PRODUCTION_MEMO_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PRODUCTION_MEMO_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 17)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.RD_NUMBER_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.RD_NUMBER_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 18)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.RDD).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.RDD).ToList();
            }

            return data;
        }

        public static List<V_ART_WF_DASHBOARD> QueryTrackingReportMockup(TRACKING_REPORT_REQUEST param, bool excel)
        {
            List<V_ART_WF_DASHBOARD> allMockupTrans = new List<V_ART_WF_DASHBOARD>();
            if (!String.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE) && param.data.CURRENT_STEP_WF_TYPE.StartsWith("A"))
            {
                return allMockupTrans;
            }

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;

                    DateTime REQUEST_DATE_FROM = new DateTime();
                    DateTime REQUEST_DATE_TO = new DateTime();
                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_FROM)) REQUEST_DATE_FROM = CNService.ConvertStringToDate(param.data.REQUEST_DATE_FROM);
                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_TO)) REQUEST_DATE_TO = CNService.ConvertStringToDate(param.data.REQUEST_DATE_TO);

                    IQueryable<V_ART_WF_DASHBOARD> q = null;

                    if (excel)
                        q = (from m in context.V_ART_WF_DASHBOARD
                             where m.CURRENT_STEP_ID == SEND_PG || (m.CURRENT_STEP_ID != SEND_PG && string.IsNullOrEmpty(m.IS_END))
                             select m);
                    else
                        q = (from m in context.V_ART_WF_DASHBOARD
                             where m.CURRENT_STEP_ID == SEND_PG
                             select m);

                    if (param.data.WORKFLOW_OVERDUE)
                    {
                        var tempDashboard = V_ART_WF_DASHBOARD_SERVICE.GetAll(context);

                        var newTemp = new List<V_ART_WF_DASHBOARD>();
                        foreach (var item in tempDashboard)
                        {
                            if (string.IsNullOrEmpty(item.IS_END))
                            {
                                if (item.CREATE_DATE_PROCESS != null)
                                {
                                    DateTime? dtReceiveWf = item.CREATE_DATE_PROCESS;
                                    DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION.Value));

                                    if (!String.IsNullOrEmpty(item.IS_STEP_DURATION_EXTEND) && item.IS_STEP_DURATION_EXTEND.Equals("X"))
                                    {
                                        dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION_EXTEND.Value));
                                    }

                                    var overdue = CNService.GetWorkingDays(DateTime.Now, dtWillFinish.Value);
                                    if (overdue < 0)
                                    {
                                        newTemp.Add(item);
                                    }
                                }
                            }
                        }
                        if (newTemp.Count > 0)
                        {
                            var listMockupId = newTemp.Select(m => m.MOCKUP_ID).ToList();
                            q = (from r in q
                                 where listMockupId.Contains(r.MOCKUP_ID)
                                 select r);
                        }
                        else
                        {
                            q = (from r in q
                                 where 1 == 2
                                 select r);
                        }
                    }

                    if (!string.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE))
                    {
                        if (param.data.CURRENT_STEP_WF_TYPE.StartsWith("M"))
                        {
                            var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP_WF_TYPE.Replace("M", ""));
                            var temp = V_ART_WF_DASHBOARD_SERVICE.GetByItem(new V_ART_WF_DASHBOARD() { CURRENT_STEP_ID = current_step_id }, context);
                            var listMockupId = temp.Where(m => string.IsNullOrEmpty(m.IS_END)).Select(m => m.MOCKUP_ID);
                            q = (from r in q
                                 where listMockupId.Contains(r.MOCKUP_ID)
                                 select r);
                        }
                    }

                    if (param.data.WORKFLOW_ACTION_BY_ME)
                    {
                        param.data.CURRENT_ASSIGN_ID = param.data.CURRENT_USER_ID;
                    }

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_TYPE))
                        q = (from r in q where r.MOCKUP_NO.StartsWith(param.data.WORKFLOW_TYPE) select r);

                    if (param.data.CURRENT_ASSIGN_ID > 0)
                    {
                        var temp = V_ART_WF_DASHBOARD_SERVICE.GetByItem(new V_ART_WF_DASHBOARD() { CURRENT_USER_ID = param.data.CURRENT_ASSIGN_ID }, context);
                        var listMockupId = temp.Where(m => string.IsNullOrEmpty(m.IS_END)).Select(m => m.MOCKUP_ID);
                        q = (from r in q
                             where listMockupId.Contains(r.MOCKUP_ID)
                             select r);
                    }

                    if (param.data.WORKING_GROUP_ID > 0)
                    {
                        var listMockupId = V_ART_WF_DASHBOARD_SERVICE.GetByItem(new V_ART_WF_DASHBOARD() { CURRENT_USER_ID = param.data.WORKING_GROUP_ID }, context).ToList().Select(m => m.MOCKUP_ID);
                        var listCheckListId = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST() { CREATOR_ID = param.data.WORKING_GROUP_ID }, context).ToList().Select(m => m.CHECK_LIST_ID);

                        q = (from r in q
                             where listMockupId.Contains(r.MOCKUP_ID) || listCheckListId.Contains(r.CHECK_LIST_ID)
                             select r);
                    }

                    if (param.data.SUPERVISED_ID > 0)
                    {
                        //var listUserId = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { UPPER_USER_ID = param.data.SUPERVISED_ID }, context).Select(m => m.USER_ID);
                        //q = (from r in q
                        //     where listUserId.Contains(r.CURRENT_USER_ID.Value) || listUserId.Contains(r.CREATOR_ID.Value)
                        //     select r);

                        //var listUserId = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { UPPER_USER_ID = param.data.SUPERVISED_ID }, context).Select(m => m.USER_ID);
                        //var listMockupId = (from m in context.ART_WF_MOCKUP_PROCESS
                        //                    where listUserId.Contains(m.CURRENT_USER_ID.Value) || listUserId.Contains(m.CREATE_BY)
                        //                    select m.MOCKUP_ID);
                        //q = (from r in q
                        //     where listMockupId.Contains(r.MOCKUP_ID)
                        //     select r);
                        var listuserlowerId = new List<int>();
                        //tier1
                        var listUserId = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { UPPER_USER_ID = param.data.SUPERVISED_ID }, context).Select(m => m.USER_ID).ToList();
                        if (listUserId != null)
                        {
                            foreach (var item1 in listUserId)
                            {
                                //tier2
                                var listuserlowerId2 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item1)).Select(t => t.USER_ID).ToList();
                                if (listuserlowerId2.Count() > 0)
                                {
                                    foreach (var item2 in listuserlowerId2)
                                    {
                                        listuserlowerId.Add(item2);
                                        //tier3
                                        var listuserlowerId3 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item2)).Select(t => t.USER_ID).ToList();
                                        if (listuserlowerId3.Count() > 0)
                                        {
                                            foreach (var item3 in listuserlowerId3)
                                            {
                                                listuserlowerId.Add(item3);
                                                //tier4
                                                var listuserlowerId4 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item3)).Select(t => t.USER_ID).ToList();
                                                if (listuserlowerId4.Count() > 0)
                                                {
                                                    foreach (var item4 in listuserlowerId4)
                                                    {
                                                        listuserlowerId.Add(item4);
                                                        //tier5
                                                        var listuserlowerId5 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item4)).Select(t => t.USER_ID).ToList();
                                                        if (listuserlowerId5.Count() > 0)
                                                        {
                                                            foreach (var item5 in listuserlowerId5)
                                                            {
                                                                listuserlowerId.Add(item5);
                                                                //tier6
                                                                var listuserlowerId6 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item5)).Select(t => t.USER_ID).ToList();
                                                                if (listuserlowerId6.Count() > 0)
                                                                {
                                                                    foreach (var item6 in listuserlowerId6)
                                                                    {
                                                                        listuserlowerId.Add(item6);
                                                                        //tier7
                                                                        var listuserlowerId7 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item6)).Select(t => t.USER_ID).ToList();
                                                                        if (listuserlowerId7.Count() > 0)
                                                                        {
                                                                            foreach (var item7 in listuserlowerId7)
                                                                            {
                                                                                listuserlowerId.Add(item7);
                                                                                //tier8
                                                                                var listuserlowerId8 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item7)).Select(t => t.USER_ID).ToList();
                                                                                if (listuserlowerId8.Count() > 0)
                                                                                {
                                                                                    foreach (var item8 in listuserlowerId8)
                                                                                    {
                                                                                        listuserlowerId.Add(item8);
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
                                            }
                                        }


                                    }
                                }
                            }
                            foreach (var a in listuserlowerId)
                                listUserId.Add(a);
                        }
                        var Final = listUserId.Distinct().ToList();
                        //q = (from r in q
                        //     where Final.Contains(r.CURRENT_USER_ID.Value) || Final.Contains(r.CREATOR_ID.Value)
                        //     select r);

                        var listMockupId = (from w in context.V_ART_WF_DASHBOARD
                                            where Final.Contains(w.CURRENT_USER_ID.Value) && string.IsNullOrEmpty(w.IS_END)
                                            select w.MOCKUP_ID);

                        var listCheckListId = (from e in context.ART_WF_MOCKUP_CHECK_LIST
                                               where Final.Contains(e.CREATOR_ID.Value)
                                               select e.CHECK_LIST_ID);

                        q = (from r in q
                             where listMockupId.Contains(r.MOCKUP_ID) || listCheckListId.Contains(r.CHECK_LIST_ID)
                             select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.REF_WF_NO))
                    {
                        var list = new List<ART_WF_MOCKUP_CHECK_LIST>();
                        var checklist = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST() { CHECK_LIST_NO = param.data.REF_WF_NO.Trim() }, context).FirstOrDefault();
                        if (checklist != null)
                        {
                            list.Add(new ART_WF_MOCKUP_CHECK_LIST() { CHECK_LIST_NO = checklist.CHECK_LIST_NO });
                            var refChecklist = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST() { REFERENCE_REQUEST_NO = param.data.REF_WF_NO.Trim() }, context).ToList();

                            refChecklist.ForEach(r =>
                            {
                                list.Add(new ART_WF_MOCKUP_CHECK_LIST() { CHECK_LIST_NO = r.CHECK_LIST_NO });
                            });

                            if (!string.IsNullOrEmpty(checklist.REFERENCE_REQUEST_NO))
                            {
                                var temp = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST() { CHECK_LIST_NO = checklist.REFERENCE_REQUEST_NO }, context).FirstOrDefault();
                                if (temp != null)
                                {
                                    list.Add(new ART_WF_MOCKUP_CHECK_LIST() { CHECK_LIST_NO = temp.CHECK_LIST_NO });
                                }
                            }
                        }

                        var tempList = list.Select(m => m.CHECK_LIST_NO).ToList();
                        q = (from r in q where tempList.Contains(r.CHECK_LIST_NO) select r);

                        //q = (from r in q where r.REFERENCE_REQUEST_NO.Contains(param.data.REF_WF_NO.Trim()) || r.CHECK_LIST_NO.Contains(param.data.REF_WF_NO.Trim()) select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_NO))
                        q = (from r in q where r.MOCKUP_NO.Contains(param.data.WORKFLOW_NO.Trim()) select r);

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_NO_2))
                        q = (from r in q where r.CHECK_LIST_NO.Contains(param.data.WORKFLOW_NO_2.Trim()) select r);

                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_FROM))
                        q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE_CHECK_LIST) >= REQUEST_DATE_FROM select r);

                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_TO))
                        q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE_CHECK_LIST) <= REQUEST_DATE_TO select r);

                    if (param.data.WORKFLOW_COMPLETED)
                        q = (from r in q where r.IS_END == "X" && string.IsNullOrEmpty(r.IS_TERMINATE) select r);

                    if (param.data.SOLD_TO_ID > 0)
                        q = (from r in q where r.SOLD_TO_ID == param.data.SOLD_TO_ID select r);

                    if (param.data.SHIP_TO_ID > 0)
                        q = (from r in q where r.SHIP_TO_ID == param.data.SHIP_TO_ID select r);

                    if (param.data.COUNTRY_ID > 0)
                        q = (from r in q join j in context.ART_WF_MOCKUP_CHECK_LIST_COUNTRY on r.CHECK_LIST_ID equals j.CHECK_LIST_ID where j.COUNTRY_ID == param.data.COUNTRY_ID select r);

                    if (!string.IsNullOrEmpty(param.data.ZONE_TXT))
                    {
                        var listCountryId = SAP_M_COUNTRY_SERVICE.GetByItem(new SAP_M_COUNTRY() { ZONE = param.data.ZONE_TXT }, context).Select(m => m.COUNTRY_ID);
                        var listCheckListId = (from r in context.ART_WF_MOCKUP_CHECK_LIST_COUNTRY
                                               where listCountryId.Contains(r.COUNTRY_ID)
                                               select r.CHECK_LIST_ID).ToList();

                        q = (from r in q
                             where listCheckListId.Contains(r.CHECK_LIST_ID)
                             select r);
                    }

                    if (param.data.COMPANY_ID > 0)
                        q = (from r in q where r.COMPANY_ID == param.data.COMPANY_ID select r);

                    if (param.data.BRAND_ID > 0)
                        q = (from r in q where r.BRAND_ID == param.data.BRAND_ID select r);

                    if (!string.IsNullOrEmpty(param.data.PROJECT_NAME))
                        q = (from r in q where r.PROJECT_NAME.Contains(param.data.PROJECT_NAME.Trim()) select r);

                    if (!string.IsNullOrEmpty(param.data.PRODUCT_CODE))
                    {
                        var arrPRODUCT_CODE = param.data.PRODUCT_CODE.Replace(" ", "").Split(',');

                        var listCheckListID = (from m in context.ART_WF_MOCKUP_CHECK_LIST_PRODUCT
                                               where arrPRODUCT_CODE.Contains(m.PRODUCT_CODE.Trim())
                                               select m.CHECK_LIST_ID);
                        q = (from r in q
                             where listCheckListID.Contains(r.CHECK_LIST_ID)
                             select r);

                        //q = (from r in q join j in context.ART_WF_MOCKUP_CHECK_LIST_PRODUCT on r.CHECK_LIST_ID equals j.CHECK_LIST_ID where param.data.PRODUCT_CODE.Contains(j.PRODUCT_CODE) select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.RD_NUMBER))
                    {
                        var listCheckListID = (from m in context.ART_WF_MOCKUP_CHECK_LIST_REFERENCE
                                               where param.data.RD_NUMBER.Contains(m.REFERENCE_NO.Trim())
                                               select m.CHECK_LIST_ID);
                        q = (from r in q
                             where listCheckListID.Contains(r.CHECK_LIST_ID)
                             select r);
                        //q = (from r in q join j in context.ART_WF_MOCKUP_CHECK_LIST_REFERENCE on r.CHECK_LIST_ID equals j.CHECK_LIST_ID where param.data.RD_NUMBER.Contains(j.REFERENCE_NO) select r);
                    }

                    if (param.data.PACKAGING_TYPE_ID > 0)
                        q = (from r in q join j in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on r.MOCKUP_NO equals j.MOCKUP_NO where !string.IsNullOrEmpty(r.MOCKUP_NO) && j.PACKING_TYPE_ID == param.data.PACKAGING_TYPE_ID select r);

                    if (param.data.CREATOR_ID > 0)
                        q = (from r in q where r.CREATOR_ID == param.data.CREATOR_ID select r);

                    if (!string.IsNullOrEmpty(param.data.PRIMARY_SIZE_TXT))
                    {
                        q = (from r in q
                             join j in context.SAP_M_3P on r.THREE_P_ID equals j.THREE_P_ID into ps
                             from j in ps.DefaultIfEmpty()
                             join j2 in context.ART_WF_MOCKUP_CHECK_LIST_PRODUCT on r.CHECK_LIST_ID equals j2.CHECK_LIST_ID into ps2
                             from j2 in ps2.DefaultIfEmpty()
                             where (j.PRIMARY_SIZE_VALUE.Contains(param.data.PRIMARY_SIZE_TXT.Trim()) || j2.PRIMARY_SIZE.Contains(param.data.PRIMARY_SIZE_TXT.Trim()))
                             select r).Distinct();
                    }

                    if (!string.IsNullOrEmpty(param.data.NET_WEIGHT_TXT))
                    {
                        q = (from r in q
                             join j in context.ART_WF_MOCKUP_CHECK_LIST_PRODUCT on r.CHECK_LIST_ID equals j.CHECK_LIST_ID into ps
                             from j in ps.DefaultIfEmpty()
                             join j2 in context.ART_WF_MOCKUP_CHECK_LIST_REFERENCE on r.CHECK_LIST_ID equals j2.CHECK_LIST_ID into ps2
                             from j2 in ps2.DefaultIfEmpty()
                             where (j2.NET_WEIGHT.Contains(param.data.NET_WEIGHT_TXT.Trim()) || j.NET_WEIGHT.Contains(param.data.NET_WEIGHT_TXT.Trim()))
                             select r).Distinct();
                    }

                    allMockupTrans = q.OrderBy(i => i.MOCKUP_NO).ToList();
                }
            }
            return allMockupTrans;
        }

        public static List<V_ART_WF_DASHBOARD_ARTWORK> QueryTrackingReportArtwork(TRACKING_REPORT_REQUEST param, bool excel)
        {
            List<V_ART_WF_DASHBOARD_ARTWORK> allArtworkTrans = new List<V_ART_WF_DASHBOARD_ARTWORK>();

            if (!String.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE) && param.data.CURRENT_STEP_WF_TYPE.StartsWith("M"))
            {
                return allArtworkTrans;
            }

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                    DateTime REQUEST_DATE_FROM = new DateTime();
                    DateTime REQUEST_DATE_TO = new DateTime();
                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_FROM)) REQUEST_DATE_FROM = CNService.ConvertStringToDate(param.data.REQUEST_DATE_FROM);
                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_TO)) REQUEST_DATE_TO = CNService.ConvertStringToDate(param.data.REQUEST_DATE_TO);

                    IQueryable<V_ART_WF_DASHBOARD_ARTWORK> q = null;

                    if (excel)
                        q = (from m in context.V_ART_WF_DASHBOARD_ARTWORK
                             where m.CURRENT_STEP_ID == SEND_PA || (m.CURRENT_STEP_ID != SEND_PA && string.IsNullOrEmpty(m.IS_END))
                             select m);
                    else
                        q = (from m in context.V_ART_WF_DASHBOARD_ARTWORK
                             where m.CURRENT_STEP_ID == SEND_PA
                             select m);

                    if (param.data.WORKFLOW_OVERDUE)
                    {
                        var tempDashboard = V_ART_WF_DASHBOARD_ARTWORK_SERVICE.GetAll(context);

                        var newTemp = new List<V_ART_WF_DASHBOARD_ARTWORK>();
                        foreach (var item in tempDashboard)
                        {
                            if (string.IsNullOrEmpty(item.IS_END))
                            {
                                if (item.CREATE_DATE_PROCESS != null)
                                {
                                    DateTime? dtReceiveWf = item.CREATE_DATE_PROCESS;
                                    DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION.Value));

                                    if (!String.IsNullOrEmpty(item.IS_STEP_DURATION_EXTEND) && item.IS_STEP_DURATION_EXTEND.Equals("X"))
                                    {
                                        dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION_EXTEND.Value));
                                    }

                                    var overdue = CNService.GetWorkingDays(DateTime.Now, dtWillFinish.Value);
                                    if (overdue < 0)
                                    {
                                        newTemp.Add(item);
                                    }
                                }
                            }
                        }
                        if (newTemp.Count > 0)
                        {
                            var listArtworkItemId = newTemp.Select(m => m.ARTWORK_ITEM_ID).ToList();
                            q = (from r in q
                                 where listArtworkItemId.Contains(r.ARTWORK_ITEM_ID)
                                 select r);
                        }
                        else
                        {
                            q = (from r in q
                                 where 1 == 2
                                 select r);
                        }
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_SO))
                    {
                        var arrSO = param.data.SEARCH_SO.Replace(" ", "").Split(',');
                        var listArtworkSubId = (from m in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
                                                where arrSO.Contains(m.SALES_ORDER_NO)
                                                select m.ARTWORK_SUB_ID);

                        //var temp = ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_SERVICE.GetByItemContain(new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER() { SALES_ORDER_NO = param.data.SEARCH_SO });
                        //var listArtworkSubId = temp.Select(m => m.ARTWORK_SUB_ID);
                        q = (from r in q
                             where listArtworkSubId.Contains(r.ARTWORK_SUB_ID)
                             select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_ORDER_BOM))
                    {
                        var arrSEARCH_ORDER_BOM = param.data.SEARCH_ORDER_BOM.Replace(" ", "").Split(',');
                        var listArtworkSubId = (from m in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT
                                                where arrSEARCH_ORDER_BOM.Contains(m.COMPONENT_MATERIAL)
                                                select m.ARTWORK_SUB_ID)
                                                  .Union(from m in context.ART_WF_ARTWORK_PROCESS_PA
                                                         where arrSEARCH_ORDER_BOM.Contains(m.MATERIAL_NO)
                                                         select m.ARTWORK_SUB_ID); ;

                        //var temp = ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_SERVICE.GetByItemContain(new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT() { COMPONENT_MATERIAL = param.data.SEARCH_ORDER_BOM });
                        //var listArtworkSubId = temp.Select(m => m.ARTWORK_SUB_ID);
                        q = (from r in q
                             where listArtworkSubId.Contains(r.ARTWORK_SUB_ID)
                             select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE))
                    {
                        if (param.data.CURRENT_STEP_WF_TYPE.StartsWith("A"))
                        {
                            var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP_WF_TYPE.Replace("A", ""));
                            var temp = V_ART_WF_DASHBOARD_ARTWORK_SERVICE.GetByItem(new V_ART_WF_DASHBOARD_ARTWORK() { CURRENT_STEP_ID = current_step_id }, context);
                            var listArtworkItemId = temp.Where(m => string.IsNullOrEmpty(m.IS_END)).Select(m => m.ARTWORK_ITEM_ID);
                            q = (from r in q
                                 where listArtworkItemId.Contains(r.ARTWORK_ITEM_ID)
                                 select r);
                        }
                    }

                    if (param.data.WORKFLOW_ACTION_BY_ME)
                    {
                        param.data.CURRENT_ASSIGN_ID = param.data.CURRENT_USER_ID;
                    }

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_TYPE))
                        q = (from r in q where r.REQUEST_ITEM_NO.StartsWith(param.data.WORKFLOW_TYPE) select r);

                    if (param.data.CURRENT_ASSIGN_ID > 0)
                    {
                        var temp = V_ART_WF_DASHBOARD_ARTWORK_SERVICE.GetByItem(new V_ART_WF_DASHBOARD_ARTWORK() { CURRENT_USER_ID = param.data.CURRENT_ASSIGN_ID }, context);
                        var listArtworkItemId = temp.Where(m => string.IsNullOrEmpty(m.IS_END)).Select(m => m.ARTWORK_ITEM_ID);
                        q = (from r in q
                             where listArtworkItemId.Contains(r.ARTWORK_ITEM_ID)
                             select r);
                    }

                    if (param.data.WORKING_GROUP_ID > 0)
                    {
                        var listArtworkItemId = V_ART_WF_DASHBOARD_ARTWORK_SERVICE.GetByItem(new V_ART_WF_DASHBOARD_ARTWORK() { CURRENT_USER_ID = param.data.WORKING_GROUP_ID }, context).ToList().Select(m => m.ARTWORK_ITEM_ID);
                        var listRequestId = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { CREATOR_ID = param.data.WORKING_GROUP_ID }, context).ToList().Select(m => m.ARTWORK_REQUEST_ID);

                        q = (from r in q
                             where listArtworkItemId.Contains(r.ARTWORK_ITEM_ID) || listRequestId.Contains(r.ARTWORK_REQUEST_ID)
                             select r);
                    }

                    if (param.data.SUPERVISED_ID > 0)
                    {
                        //var listUserId = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { UPPER_USER_ID = param.data.SUPERVISED_ID }, context).Select(m => m.USER_ID);

                        //var listArtworkItemId = (from m in context.ART_WF_ARTWORK_PROCESS
                        //                         where listUserId.Contains(m.CURRENT_USER_ID.Value) || listUserId.Contains(m.CREATE_BY)
                        //                         select m.ARTWORK_ITEM_ID);
                        //q = (from r in q
                        //     where listArtworkItemId.Contains(r.ARTWORK_ITEM_ID)
                        //     select r);
                        //q = (from r in q
                        //     where listUserId.Contains(r.CURRENT_USER_ID.Value) || listUserId.Contains(r.CREATE_BY_ARTWORK_REQUEST)
                        //     select r);
                        var listuserlowerId = new List<int>();
                        //tier1
                        var listUserId = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { UPPER_USER_ID = param.data.SUPERVISED_ID }, context).Select(m => m.USER_ID).ToList();
                        if (listUserId != null)
                        {
                            foreach (var item1 in listUserId)
                            {
                                //tier2
                                var listuserlowerId2 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item1)).Select(t => t.USER_ID).ToList();
                                if (listuserlowerId2.Count() > 0)
                                {
                                    foreach (var item2 in listuserlowerId2)
                                    {
                                        listuserlowerId.Add(item2);
                                        //tier3
                                        var listuserlowerId3 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item2)).Select(t => t.USER_ID).ToList();
                                        if (listuserlowerId3.Count() > 0)
                                        {
                                            foreach (var item3 in listuserlowerId3)
                                            {
                                                listuserlowerId.Add(item3);
                                                //tier4
                                                var listuserlowerId4 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item3)).Select(t => t.USER_ID).ToList();
                                                if (listuserlowerId4.Count() > 0)
                                                {
                                                    foreach (var item4 in listuserlowerId4)
                                                    {
                                                        listuserlowerId.Add(item4);
                                                        //tier5
                                                        var listuserlowerId5 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item4)).Select(t => t.USER_ID).ToList();
                                                        if (listuserlowerId5.Count() > 0)
                                                        {
                                                            foreach (var item5 in listuserlowerId5)
                                                            {
                                                                listuserlowerId.Add(item5);
                                                                //tier6
                                                                var listuserlowerId6 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item5)).Select(t => t.USER_ID).ToList();
                                                                if (listuserlowerId6.Count() > 0)
                                                                {
                                                                    foreach (var item6 in listuserlowerId6)
                                                                    {
                                                                        listuserlowerId.Add(item6);
                                                                        //tier7
                                                                        var listuserlowerId7 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item6)).Select(t => t.USER_ID).ToList();
                                                                        if (listuserlowerId7.Count() > 0)
                                                                        {
                                                                            foreach (var item7 in listuserlowerId7)
                                                                            {
                                                                                listuserlowerId.Add(item7);
                                                                                //tier8
                                                                                var listuserlowerId8 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item7)).Select(t => t.USER_ID).ToList();
                                                                                if (listuserlowerId8.Count() > 0)
                                                                                {
                                                                                    foreach (var item8 in listuserlowerId8)
                                                                                    {
                                                                                        listuserlowerId.Add(item8);
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
                                            }
                                        }


                                    }
                                }
                            }
                            foreach (var a in listuserlowerId)
                                listUserId.Add(a);
                        }
                        var Final = listUserId.Distinct().ToList();
                        //q = (from r in q
                        //     where Final.Contains(r.CURRENT_USER_ID.Value) || Final.Contains(r.CREATE_BY_ARTWORK_REQUEST)
                        //     select r);

                        var listArtworkItemId = (from w in context.V_ART_WF_DASHBOARD_ARTWORK
                                                 where Final.Contains(w.CURRENT_USER_ID.Value) && string.IsNullOrEmpty(w.IS_END)
                                                 select w.ARTWORK_ITEM_ID);

                        var listRequestId = (from e in context.ART_WF_ARTWORK_REQUEST
                                             where Final.Contains(e.CREATOR_ID.Value)
                                             select e.ARTWORK_REQUEST_ID);

                        q = (from r in q
                             where listArtworkItemId.Contains(r.ARTWORK_ITEM_ID) || listRequestId.Contains(r.ARTWORK_REQUEST_ID)
                             select r);
                    }


                    if (!string.IsNullOrEmpty(param.data.REF_WF_NO))
                    {
                        var list = new List<ART_WF_ARTWORK_REQUEST>();
                        var checklist = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_NO = param.data.REF_WF_NO.Trim() }, context).FirstOrDefault();
                        if (checklist != null)
                        {
                            list.Add(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_NO = checklist.ARTWORK_REQUEST_NO });
                            var refChecklist = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { REFERENCE_REQUEST_NO = param.data.REF_WF_NO.Trim() }, context).FirstOrDefault();
                            if (refChecklist != null)
                            {
                                list.Add(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_NO = refChecklist.ARTWORK_REQUEST_NO });
                            }
                            if (!string.IsNullOrEmpty(checklist.REFERENCE_REQUEST_NO))
                            {
                                var temp = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_NO = checklist.REFERENCE_REQUEST_NO }, context).FirstOrDefault();
                                if (temp != null)
                                {
                                    list.Add(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_NO = temp.ARTWORK_REQUEST_NO });
                                }
                            }
                        }

                        var tempList = list.Select(m => m.ARTWORK_REQUEST_NO).ToList();
                        q = (from r in q where tempList.Contains(r.ARTWORK_REQUEST_NO) select r);

                        //q = (from r in q where r.REFERENCE_REQUEST_NO.Contains(param.data.REF_WF_NO.Trim()) || r.ARTWORK_REQUEST_NO.Contains(param.data.REF_WF_NO.Trim()) select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_NO))
                        q = (from r in q where r.REQUEST_ITEM_NO.Contains(param.data.WORKFLOW_NO.Trim()) select r);

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_NO_2))
                        q = (from r in q where r.ARTWORK_REQUEST_NO.Contains(param.data.WORKFLOW_NO_2.Trim()) select r);

                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_FROM))
                        q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE_ARTWORK_REQUEST) >= REQUEST_DATE_FROM select r);

                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_TO))
                        q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE_ARTWORK_REQUEST) <= REQUEST_DATE_TO select r);

                    if (param.data.WORKFLOW_COMPLETED)
                        q = (from r in q where r.IS_END == "X" && string.IsNullOrEmpty(r.IS_TERMINATE) select r);

                    if (param.data.SOLD_TO_ID > 0)
                        q = (from r in q where r.SOLD_TO_ID == param.data.SOLD_TO_ID select r);

                    if (param.data.SHIP_TO_ID > 0)
                        q = (from r in q where r.SHIP_TO_ID == param.data.SHIP_TO_ID select r);

                    if (param.data.COUNTRY_ID > 0)
                        q = (from r in q join j in context.ART_WF_ARTWORK_REQUEST_COUNTRY on r.ARTWORK_REQUEST_ID equals j.ARTWORK_REQUEST_ID where j.COUNTRY_ID == param.data.COUNTRY_ID select r);

                    if (!string.IsNullOrEmpty(param.data.ZONE_TXT))
                    {
                        var listCountryId = SAP_M_COUNTRY_SERVICE.GetByItem(new SAP_M_COUNTRY() { ZONE = param.data.ZONE_TXT }, context).Select(m => m.COUNTRY_ID);
                        var listARTWORK_REQUEST_ID = (from r in context.ART_WF_ARTWORK_REQUEST_COUNTRY
                                                      where listCountryId.Contains(r.COUNTRY_ID)
                                                      select r.ARTWORK_REQUEST_ID).ToList();

                        q = (from r in q
                             where listARTWORK_REQUEST_ID.Contains(r.ARTWORK_REQUEST_ID)
                             select r);
                    }

                    if (param.data.COMPANY_ID > 0)
                        q = (from r in q where r.COMPANY_ID == param.data.COMPANY_ID select r);

                    if (param.data.BRAND_ID > 0)
                        q = (from r in q where r.BRAND_ID == param.data.BRAND_ID select r);

                    if (!string.IsNullOrEmpty(param.data.PROJECT_NAME))
                        q = (from r in q where r.PROJECT_NAME.Contains(param.data.PROJECT_NAME.Trim()) select r);

                    if (!string.IsNullOrEmpty(param.data.PRODUCT_CODE))
                    {
                        var arrPRODUCT_CODE = param.data.PRODUCT_CODE.Replace(" ", "").Split(',');
                        var listArtworkSubId = (from r in q
                                                join j2 in context.ART_WF_ARTWORK_REQUEST_PRODUCT on r.ARTWORK_REQUEST_ID equals j2.ARTWORK_REQUEST_ID into ps2
                                                from j2 in ps2.DefaultIfEmpty()
                                                join j3 in context.XECM_M_PRODUCT on j2.PRODUCT_CODE_ID equals j3.XECM_PRODUCT_ID into ps3
                                                from j3 in ps3.DefaultIfEmpty()
                                                where arrPRODUCT_CODE.Contains(j3.PRODUCT_CODE)
                                                select r.ARTWORK_SUB_ID).Distinct();
                        q = (from r in q
                             where listArtworkSubId.Contains(r.ARTWORK_SUB_ID)
                             select r);


                        //q = (from r in q
                        //     join j2 in context.ART_WF_ARTWORK_REQUEST_PRODUCT on r.ARTWORK_REQUEST_ID equals j2.ARTWORK_REQUEST_ID into ps2
                        //     from j2 in ps2.DefaultIfEmpty()
                        //     join j3 in context.XECM_M_PRODUCT on j2.PRODUCT_CODE_ID equals j3.XECM_PRODUCT_ID into ps3
                        //     from j3 in ps3.DefaultIfEmpty()
                        //     where (j3.PRODUCT_CODE.Contains(param.data.PRODUCT_CODE.Trim()))
                        //     select r).Distinct();



                        //var listCheckListID = (from m in context.ART_WF_ARTWORK_REQUEST_PRODUCT
                        //                       where param.data.PRODUCT_CODE.Contains(m.PRODUCT_CODE.Trim())
                        //                       select m.CHECK_LIST_ID);
                        //q = (from r in q
                        //     where listCheckListID.Contains(r.CHECK_LIST_ID)
                        //     select r);
                        //q = (from r in q join j in context.ART_WF_MOCKUP_CHECK_LIST_PRODUCT on r.CHECK_LIST_ID equals j.CHECK_LIST_ID where param.data.PRODUCT_CODE.Contains(j.PRODUCT_CODE) select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.RD_NUMBER))
                    {
                        var listArtworkRequestID = (from m in context.ART_WF_ARTWORK_REQUEST_REFERENCE
                                                    where param.data.RD_NUMBER.Contains(m.REFERENCE_NO.Trim())
                                                    select m.ARTWORK_REQUEST_ID);
                        q = (from r in q
                             where listArtworkRequestID.Contains(r.ARTWORK_REQUEST_ID)
                             select r);
                        //q = (from r in q join j in context.ART_WF_MOCKUP_CHECK_LIST_REFERENCE on r.CHECK_LIST_ID equals j.CHECK_LIST_ID where param.data.RD_NUMBER.Contains(j.REFERENCE_NO) select r);
                    }

                    if (param.data.PACKAGING_TYPE_ID > 0)
                    {
                        var listARTWORK_SUB_ID = (from m in context.ART_WF_ARTWORK_PROCESS_PA
                                                  where m.MATERIAL_GROUP_ID != null && m.MATERIAL_GROUP_ID == param.data.PACKAGING_TYPE_ID
                                                  select m.ARTWORK_SUB_ID);

                        q = (from r in q
                             where listARTWORK_SUB_ID.Contains(r.ARTWORK_SUB_ID)
                             select r);
                    }

                    if (param.data.CREATOR_ID > 0)
                    {
                        var listArtworkRequestID = (from m in context.ART_WF_ARTWORK_REQUEST
                                                    where m.CREATOR_ID == param.data.CREATOR_ID
                                                    select m.ARTWORK_REQUEST_ID);

                        q = (from r in q where listArtworkRequestID.Contains(r.ARTWORK_REQUEST_ID) select r);

                        //q = (from r in q where r.CREATE_BY_ARTWORK_REQUEST == param.data.CREATOR_ID select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.PRIMARY_SIZE_TXT))
                    {
                        q = (from r in q
                             join j in context.SAP_M_3P on r.THREE_P_ID equals j.THREE_P_ID into ps
                             from j in ps.DefaultIfEmpty()
                             join j2 in context.ART_WF_ARTWORK_REQUEST_PRODUCT on r.ARTWORK_REQUEST_ID equals j2.ARTWORK_REQUEST_ID into ps2
                             from j2 in ps2.DefaultIfEmpty()
                             join j3 in context.XECM_M_PRODUCT on j2.PRODUCT_CODE_ID equals j3.XECM_PRODUCT_ID into ps3
                             from j3 in ps3.DefaultIfEmpty()
                             where (j.PRIMARY_SIZE_VALUE.Contains(param.data.PRIMARY_SIZE_TXT.Trim()) || j3.PRIMARY_SIZE.Contains(param.data.PRIMARY_SIZE_TXT.Trim()))
                             select r).Distinct();
                    }

                    if (!string.IsNullOrEmpty(param.data.NET_WEIGHT_TXT))
                    {
                        q = (from r in q
                             join j in context.ART_WF_ARTWORK_REQUEST_PRODUCT on r.ARTWORK_REQUEST_ID equals j.ARTWORK_REQUEST_ID into ps
                             from j in ps.DefaultIfEmpty()
                             join j3 in context.XECM_M_PRODUCT on j.PRODUCT_CODE_ID equals j3.XECM_PRODUCT_ID into ps3
                             from j3 in ps3.DefaultIfEmpty()
                             join j2 in context.ART_WF_ARTWORK_REQUEST_REFERENCE on r.ARTWORK_REQUEST_ID equals j2.ARTWORK_REQUEST_ID into ps2
                             from j2 in ps2.DefaultIfEmpty()
                             where (j2.NET_WEIGHT.Contains(param.data.NET_WEIGHT_TXT.Trim()) || j3.NET_WEIGHT.Contains(param.data.NET_WEIGHT_TXT.Trim()))
                             select r).Distinct();
                    }

                    allArtworkTrans = q.OrderBy(i => i.REQUEST_ITEM_NO).ToList();
                }
            }
            return allArtworkTrans;
        }


        public static TU_TRACKING_WF_REPORT_MODEL_RESULT GetViewTrackingReportV3(TU_TRACKING_WF_REPORT_MODEL_REQUEST param)
        {
            TU_TRACKING_WF_REPORT_MODEL_RESULT Results = new TU_TRACKING_WF_REPORT_MODEL_RESULT();
            Results.data = new List<TU_TRACKING_WF_REPORT_MODEL>();
            // Results.dataExcel = new List<TU_TRACKING_WF_REPORT_MODEL>();

            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                 Results.data = QueryTrackingReportV3(param, ref Results);

               // QueryTrackingReportV3(param, ref Results);

                Results.status = "S";
                Results.draw = param.draw;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }


        public static string getSQLWherePA(TU_TRACKING_WF_REPORT_MODEL_REQUEST param, ARTWORKEntities context)
        {
            string where = "";
            DateTime mydate;
            if (!String.IsNullOrEmpty(param.data.SEARCH_REQUEST_DATE_FROM))
            {
                mydate = CNService.ConvertStringToDate(param.data.SEARCH_REQUEST_DATE_FROM);
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "CONVERT(DATE,REQUEST_CREATE_DATE) >= '" + mydate.ToString("yyyy-MM-dd") + "'");
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_REQUEST_DATE_TO))
            {
                mydate = CNService.ConvertStringToDate(param.data.SEARCH_REQUEST_DATE_TO);
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "CONVERT(DATE,REQUEST_CREATE_DATE) <= '" + mydate.ToString("yyyy-MM-dd") + "'");
            }



            if (!String.IsNullOrEmpty(param.data.SEARCH_WF_SUB_TYPE))
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_WF_SUB_TYPE.Trim(), "WF_NO"));
            }



            if (!String.IsNullOrEmpty(param.data.SEARCH_WF_NO))
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_WF_NO.Trim(), "WF_NO",true,true));
            }


            if (!String.IsNullOrEmpty(param.data.SEARCH_REQUEST_NO))
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_REQUEST_NO.Trim(), "REQUEST_NO",true,true));
            }


            if (!String.IsNullOrEmpty(param.data.SEARCH_REFERENCE_FORM_NO))
            {
                string where_ref_request_no = CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_REFERENCE_FORM_NO.Trim(), "REFERENCE_REQUEST_NO",true,true);
                string where_request_no = CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_REFERENCE_FORM_NO.Trim(), "REQUEST_NO",true,true);
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "((" + where_ref_request_no + ") or (" + where_request_no + "))");

            }

            if (param.data.SEARCH_WF_IS_COMPLETED == "true")
            {

                where = CNService.getSQLWhereByJoinStringWithAnd(where, "ISNULL(IS_END,'') = 'X' AND IS_TERMINATE IS NULL AND PARENT_SUB_ID IS NULL");
            }

            if (param.data.SEARCH_WF_IN_PROCESS == "true")
            {

                where = CNService.getSQLWhereByJoinStringWithAnd(where, "IS_END IS NULL");
            }


            if (param.data.SEARCH_COMPANY_ID > 0)
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "COMPANY_ID =" + param.data.SEARCH_COMPANY_ID);
            }

            if (param.data.SEARCH_SOLD_TO_ID > 0)
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "SOLD_TO_ID =" + param.data.SEARCH_SOLD_TO_ID);
            }

            if (param.data.SEARCH_SHIP_TO_ID > 0)
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "SHIP_TO_ID =" + param.data.SEARCH_SHIP_TO_ID);
            }

            if (param.data.SEARCH_BRAND_ID > 0)
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "BRAND_ID =" + param.data.SEARCH_BRAND_ID);
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_PROJECT_NAME))
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_PROJECT_NAME.Trim(), "PROJECT_NAME",false,true));
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_PACKAGING_TYPE))
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_PACKAGING_TYPE.Trim(), "PACKAGING_TYPE",false,true));
            }

            if (param.data.SEARCH_CREATOR_ID > 0)
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "CREATOR_ID =" + param.data.SEARCH_CREATOR_ID);
            }


            //if (!String.IsNullOrEmpty(param.data.SEARCH_PRIMARY_SIZE))
            //{
            //    where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_PRIMARY_SIZE, "PRIMARY_SIZE"));
            //}

            return where;
        }


        public static string getSQLWhereSO(TU_TRACKING_WF_REPORT_MODEL_REQUEST param, ARTWORKEntities context)
        {
            string where = "";
            DateTime mydate;

            if (!String.IsNullOrEmpty(param.data.SEARCH_SO_NO))
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_SO_NO.Trim(), "SALES_ORDER_NO"));
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_SO_MATERIAL))
            {
                string mat_bom = CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_SO_MATERIAL.Trim(), "COMPONENT_MATERIAL");
                string mat_pa = CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_SO_MATERIAL.Trim(), "PA_MATERIAL");

                where = CNService.getSQLWhereByJoinStringWithAnd(where, "((" + mat_bom + ") or (" + mat_pa + "))");
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_SO_CREATE_DATE_FROM))
            {
                mydate = CNService.ConvertStringToDate(param.data.SEARCH_SO_CREATE_DATE_FROM);
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "CREATE_DATE >= '" + mydate.ToString("yyyy-MM-dd") + "'");
            }


            if (!String.IsNullOrEmpty(param.data.SEARCH_SO_CREATE_DATE_TO))
            {
                mydate = CNService.ConvertStringToDate(param.data.SEARCH_SO_CREATE_DATE_TO);
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "CREATE_DATE <= '" + mydate.ToString("yyyy-MM-dd") + "'");
            }


            return where;

        }


        public static string getSQLWhereCountry(TU_TRACKING_WF_REPORT_MODEL_REQUEST param, ARTWORKEntities context)
        {
            string where = "";

            if (param.data.SEARCH_COUNTRY_ID > 0)
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "COUNTRY_ID =" + param.data.SEARCH_COUNTRY_ID);
            }

            if (!string.IsNullOrEmpty(param.data.SEARCH_ZONE))
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "ZONE ='"+ param.data.SEARCH_ZONE.Trim() + "'");
            }


            return where;
        }

        public static string getSQLWhereProduct(TU_TRACKING_WF_REPORT_MODEL_REQUEST param, ARTWORKEntities context)
        {
            string where = "";


            if (!String.IsNullOrEmpty(param.data.SEARCH_PRODUCT))
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_PRODUCT.Trim(), "PRODUCT_CODE"));
            }


            if (!String.IsNullOrEmpty(param.data.SEARCH_PRIMARY_SIZE))
            {
                var primary_size = param.data.SEARCH_PRIMARY_SIZE.Replace(" ", "").Trim();
                var product_primary_size = CNService.getSQLWhereLikeByConvertString(primary_size, "REPLACE(PRODUCT_PRIMARY_SIZE,' ','')", true, false, false);
                var request_primary_size = CNService.getSQLWhereLikeByConvertString(primary_size, "REPLACE(REQUEST_PRIMARY_SIZE,' ','')", true, false, false);
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "((" + product_primary_size + ") OR (" + request_primary_size + "))");
            }

            return where;

        }


        public static string getSQLWhereReference(TU_TRACKING_WF_REPORT_MODEL_REQUEST param, ARTWORKEntities context)
        {
            string where = "";

            if (!String.IsNullOrEmpty(param.data.SEARCH_REFERENCE_NO))
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_REFERENCE_NO.Trim(), "REFERENCE_NO", false,true));
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_NET_WEIGHT))
            {
                //var rd_netweight = CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_NET_WEIGHT.Trim(), "dbo.UDF_ExtractNumbers(RF_NET_WEIGHT)", false, true);
                //var pd_netweight = CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_NET_WEIGHT.Trim(), "dbo.UDF_ExtractNumbers(PD_NET_WEIGHT)", false, true);

                var rd_netweight = "dbo.UDF_ExtractNumbers(RF_NET_WEIGHT) = '" + param.data.SEARCH_NET_WEIGHT.Trim() + "'";
                var pd_netweight = "dbo.UDF_ExtractNumbers(PD_NET_WEIGHT) = '" + param.data.SEARCH_NET_WEIGHT.Trim() + "'";

                where = CNService.getSQLWhereByJoinStringWithAnd(where, "((" + rd_netweight + ") OR (" + pd_netweight + "))");
            }

            return where;

        }


        public static string getSQLWhereProcess(TU_TRACKING_WF_REPORT_MODEL_REQUEST param, ARTWORKEntities context)
        {
            string where = "";

            if (param.data.SEARCH_CURRENT_STEP_ID > 0)
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "CURRENT_STEP_ID =" + param.data.SEARCH_CURRENT_STEP_ID);
            }

            if (param.data.SEARCH_CURRENT_ASSING_ID > 0)
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "CURRENT_USER_ID =" + param.data.SEARCH_CURRENT_ASSING_ID);
            }

            if (param.data.SEARCH_SUPERVISED_BY_ID > 0)
            {
                string sqlUserOfSupervisor = "SELECT USER_ID FROM ART_M_USER_UPPER_LEVEL WHERE UPPER_USER_ID = " + param.data.SEARCH_SUPERVISED_BY_ID;
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "CURRENT_USER_ID IN (" + sqlUserOfSupervisor + ")");
            }


            if (param.data.SEARCH_WORKING_GROUP_ID > 0)
            {

                where = CNService.getSQLWhereByJoinStringWithAnd(where, "CURRENT_USER_ID = " + param.data.SEARCH_WORKING_GROUP_ID + " OR CREATOR_ID =" + param.data.SEARCH_WORKING_GROUP_ID);
            }

            if (param.data.SEARCH_ACTION_BY_ME == "true")
            {
                if (param.data.SEARCH_LOGIN_USER_ID > 0)
                {

                    where = CNService.getSQLWhereByJoinStringWithAnd(where, "CURRENT_USER_ID =" + param.data.SEARCH_LOGIN_USER_ID);
                }

            }

            if (param.data.SEARCH_WORKFLOW_IS_OVERDUE == "true")
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "ISNULL(DUE_DATE,GETDATE()) < STEP_END_DATE");
            }





            return where;

        }


        public static List<TU_TRACKING_WF_REPORT_MODEL> QueryTrackingReportV3(TU_TRACKING_WF_REPORT_MODEL_REQUEST param, ref TU_TRACKING_WF_REPORT_MODEL_RESULT Results)
        {
            List<TU_TRACKING_WF_REPORT_MODEL> q = new List<TU_TRACKING_WF_REPORT_MODEL>();

            //string where_so = getWhereSO(param);    //"sales_order_no  LIKE '5004029%'";
            //string where_po = getWherePO(param);

            string where_pa = "";  //"CONVERT(DATE,REQUEST_CREATE_DATE) >= '2021-03-01' AND CONVERT(DATE,REQUEST_CREATE_DATE) <= '2021-03-01'";
            string where_so = "";
            string where_product = "";
            string where_process = "";
            string where_reference = "";
            string where_country = "";
            string AW = "AW";
            bool isViewFullStep = false;
            //var wf_type = param.data.SEARCH_WF_TYPE_X;

            using (var context = new ARTWORKEntities())
            {

                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;


                    isViewFullStep = TU_TRACKING_USER_MODEL.IsRoleViewFullStep(param.data.SEARCH_LOGIN_USER_ID.GetValueOrDefault(0),context);
                    where_pa = getSQLWherePA(param, context);
                    where_so = getSQLWhereSO(param, context);
                    where_product = getSQLWhereProduct(param, context);
                    where_process = getSQLWhereProcess(param, context);
                    where_reference = getSQLWhereReference(param, context);
                    where_country = getSQLWhereCountry(param, context);



                    if (param.data.SEARCH_WF_TYPE_X == AW)
                    {
                        q = context.Database.SqlQuery<TU_TRACKING_WF_REPORT_MODEL>
                        ("sp_ART_REPORT_TRACKING_AW @where_pa, @where_so, @where_product, @where_process, @where_reference, @where_country"
                        , new SqlParameter("@where_pa", where_pa)
                        , new SqlParameter("@where_so", where_so)
                        , new SqlParameter("@where_product", where_product)
                        , new SqlParameter("@where_process", where_process)
                        , new SqlParameter("@where_reference", where_reference)
                        , new SqlParameter("@where_country", where_country)
                        ).ToList();
                    }
                    else
                    {
                        q = context.Database.SqlQuery<TU_TRACKING_WF_REPORT_MODEL>
                        ("sp_ART_REPORT_TRACKING_MC @where_pg, @where_product, @where_process, @where_reference, @where_country"
                        , new SqlParameter("@where_pg", where_pa)
                        , new SqlParameter("@where_product", where_product)
                        , new SqlParameter("@where_process", where_process)
                        , new SqlParameter("@where_reference", where_reference)
                        , new SqlParameter("@where_country", where_country)
                        ).ToList();
                    }



                    //Results.recordsTotal = q.Count();
                    //Results.recordsFiltered = q.Count();
                    //q = q.Distinct().OrderBy(o => o.WF_NO).Skip(param.start).Take(param.length).ToList();
                    List<TU_TRACKING_WF_REPORT_MODEL> qTemp = new List<TU_TRACKING_WF_REPORT_MODEL>();
                    bool IsSkipPage = false;

                    if (!string.IsNullOrEmpty(param.data.GENERATE_EXCEL))
                    {
                        resetDataToListTrackingAWReportV3_Excel(param, ref q, ref qTemp, context,where_pa,where_so,where_product,where_process,where_reference,where_country);
                    }
                    else
                    {
                        if (isSortColumnSkipPageV3(param, ref q, ref Results, true))
                        {
                            IsSkipPage = true;
                            resetDataToListTrackingAWReportV3(param, ref q, ref qTemp, context);
                        }
                        else
                        {
                            resetDataToListTrackingAWReportV3(param, ref q, ref qTemp, context);
                            //isSortColumnSkipPageV3(param, ref q, ref Results, false);
                        }
                    }



                    //------------------------------------------- Filter Step Again --------------------------------

                    filterProcessToListTrackingAWReportV3(isViewFullStep, param, ref q,context);

                    //------------------------------------------- Filter Step Again --------------------------------


                    orderbyColumnTrackingReportV3(param, ref q, ref Results, IsSkipPage);
                   // q = q.OrderBy(o => (o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                }
            }


            return q;
        }


        public static void filterProcessToListTrackingAWReportV3(bool isViewFullStep, TU_TRACKING_WF_REPORT_MODEL_REQUEST param, ref List<TU_TRACKING_WF_REPORT_MODEL> q,ARTWORKEntities context)
        {
            if (isViewFullStep)
            {
                if (param.data.SEARCH_CURRENT_STEP_ID > 0)
                {
                    List<int> list_WF_SUB_PA_ID;
                    //qTemp1 = q.Where(w => w.CURRENT_STEP_ID == param.data.SEARCH_CURRENT_STEP_ID).ToList();
                    switch (param.data.SEARCH_WF_TYPE_X)
                    {                    
                        case "AW":
                            //q = q.Where(w => w.CURRENT_STEP_ID == param.data.SEARCH_CURRENT_STEP_ID || w.CURRENT_STEP_CODE == "SEND_PA").ToList();
                            list_WF_SUB_PA_ID = q.Where(w => w.CURRENT_STEP_CODE == "SEND_PA").Select(s=> s.WF_SUB_PA_ID).ToList();
                            break;
                        default:
                            // q = q.Where(w => w.CURRENT_STEP_ID == param.data.SEARCH_CURRENT_STEP_ID || w.CURRENT_STEP_CODE == "SEND_PG").ToList();
                            list_WF_SUB_PA_ID = q.Where(w => w.CURRENT_STEP_CODE == "SEND_PG").Select(s => s.WF_SUB_PA_ID).ToList();
                            break;
                    }

                    if (list_WF_SUB_PA_ID !=null && list_WF_SUB_PA_ID.Count > 0)
                    {
                        q = q.Where(w => w.CURRENT_STEP_ID == param.data.SEARCH_CURRENT_STEP_ID || list_WF_SUB_PA_ID.Contains(w.WF_SUB_PA_ID)).ToList();
                    } else
                    {
                        q = q.Where(w => w.CURRENT_STEP_ID == param.data.SEARCH_CURRENT_STEP_ID).ToList();
                    }

                
                }
                //if (param.data.SEARCH_CURRENT_ASSING_ID > 0)
                //{
                //    switch (param.data.SEARCH_WF_TYPE_X)
                //    {
                //        case "AW":
                //            q = q.Where(w => w.CURRENT_USER_ID == param.data.SEARCH_CURRENT_ASSING_ID || w.CURRENT_STEP_CODE == "SEND_PA").ToList();
                //            break;
                //        default:
                //            q = q.Where(w => w.CURRENT_USER_ID == param.data.SEARCH_CURRENT_ASSING_ID || w.CURRENT_STEP_CODE == "SEND_PG").ToList();
                //            break;
                //    }

                //}

                if (param.data.SEARCH_ACTION_BY_ME == "true")
                {
                    if (param.data.SEARCH_LOGIN_USER_ID > 0)
                    {
                        //switch (param.data.SEARCH_WF_TYPE_X)
                        //{
                        //    case "AW":
                        //        q = q.Where(w => w.CURRENT_USER_ID == param.data.SEARCH_LOGIN_USER_ID || w.CURRENT_STEP_CODE == "SEND_PA").ToList();
                        //        break;
                        //    default:
                        //        q = q.Where(w => w.CURRENT_USER_ID == param.data.SEARCH_LOGIN_USER_ID || w.CURRENT_STEP_CODE == "SEND_PG").ToList();
                        //        break;
                        //}
                        q = q.Where(w => w.CURRENT_USER_ID == param.data.SEARCH_LOGIN_USER_ID).ToList();

                    }
                }



                //if (param.data.SEARCH_SUPERVISED_BY_ID > 0)
                //{

                //    var listUserId = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { UPPER_USER_ID = (int)param.data.SEARCH_SUPERVISED_BY_ID }, context).Select(m => m.USER_ID).ToList();
                //    //q = q.Where(e => e.CURRENT_USER_ID != null && listUserId.Contains((int)e.CURRENT_USER_ID));
                //    switch (param.data.SEARCH_WF_TYPE_X)
                //    {
                //        case "AW":
                //            q = q.Where(w => w.CURRENT_USER_ID != null).Where(w => listUserId.Contains((int)w.CURRENT_USER_ID) || w.CURRENT_STEP_CODE == "SEND_PA").ToList();
                //            break;
                //        default:
                //            q = q.Where(w => w.CURRENT_USER_ID != null).Where(w => listUserId.Contains((int)w.CURRENT_USER_ID) || w.CURRENT_STEP_CODE == "SEND_PG").ToList();
                //            break;
                //    }

                //}

                //if (param.data.SEARCH_WORKFLOW_IS_OVERDUE == "true")
                //{
                //    switch (param.data.SEARCH_WF_TYPE_X)
                //    {
                //        case "AW":
                //            q = q.Where(w => w.DUE_DATE < w.STEP_END_DATE || w.CURRENT_STEP_CODE == "SEND_PA").ToList();
                //            break;
                //        default:
                //            q = q.Where(w => w.DUE_DATE < w.STEP_END_DATE || w.CURRENT_STEP_CODE == "SEND_PG").ToList();
                //            break;
                //    }
                //}

            }
            else
            {

                if (param.data.SEARCH_CURRENT_STEP_ID > 0)
                {
                    q = q.Where(w => w.CURRENT_STEP_ID == param.data.SEARCH_CURRENT_STEP_ID).ToList();
                }

                if (param.data.SEARCH_CURRENT_ASSING_ID > 0)
                {
                 
                    q = q.Where(w => w.CURRENT_USER_ID == param.data.SEARCH_CURRENT_ASSING_ID).ToList();
                }

                if (param.data.SEARCH_SUPERVISED_BY_ID > 0)
                {
                    var listUserId = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { UPPER_USER_ID = (int)param.data.SEARCH_SUPERVISED_BY_ID }, context).Select(m => m.USER_ID).ToList();
                    q = q.Where(w => w.CURRENT_USER_ID != null && listUserId.Contains((int)w.CURRENT_USER_ID)).ToList();
                }


                if (param.data.SEARCH_ACTION_BY_ME == "true")
                {
                    if (param.data.SEARCH_LOGIN_USER_ID > 0)
                    {
                        q = q.Where(w => w.CURRENT_USER_ID == param.data.SEARCH_LOGIN_USER_ID).ToList();
                    }
                }

                if (param.data.SEARCH_WORKFLOW_IS_OVERDUE == "true")
                {
                    q = q.Where(w => w.DUE_DATE  < w.STEP_END_DATE).ToList();
                }

            }
        }


        public static void resetDataToListTrackingAWReportV3_Excel(TU_TRACKING_WF_REPORT_MODEL_REQUEST param, ref List<TU_TRACKING_WF_REPORT_MODEL> q, ref List<TU_TRACKING_WF_REPORT_MODEL> qTemp, ARTWORKEntities context
        ,string where_pa,string where_so,string where_product,string where_process,string where_reference, string where_country)
        {

            //----------------------------------------------------------------------
            string AW = "AW";
            //string IN_WF_ID = "0";
            //string IN_WF_SUB_ID = "0";
            //string IN_REQUEST_ID = "0";

            List<TU_TRACKING_WF_ASSING_SO_MODEL> listSOTemp, listSO;      // for PA only
            List<TU_TRACKING_WF_PRODUCT_MODEL> listProductTemp, listProduct;
            List<TU_TRACKING_WF_REFERENCE_MODEL> listReferenceTemp, listReference;
            List<TU_TRACKING_WF_PROCESS_MODEL> listProcessTemp, listProcess;
            List<TU_TRACKING_WF_PROCESS_VENDOR_MODEL> listVendorTemp, listVendor;   // for PG only

            //string sqlAssingSO, sqlProduct, sqlProcess, sqlReference, sqlVendor;

            if (q != null && q.Count > 0)
            {
            

                listSO = new List<TU_TRACKING_WF_ASSING_SO_MODEL>();
                listVendor = new List<TU_TRACKING_WF_PROCESS_VENDOR_MODEL>();
                if (param.data.SEARCH_WF_TYPE_X == AW)
                {
                    
                    listSO = context.Database.SqlQuery<TU_TRACKING_WF_ASSING_SO_MODEL>
                     ("sp_ART_REPORT_TRACKING_AW_ASSIGN_SO @where_pa, @where_so, @where_product, @where_process, @where_reference, @where_country"
                      , new SqlParameter("@where_pa", where_pa)
                      , new SqlParameter("@where_so", where_so)
                      , new SqlParameter("@where_product", where_product)
                      , new SqlParameter("@where_process", where_process)
                      , new SqlParameter("@where_reference", where_reference)
                      , new SqlParameter("@where_country", where_country)
                      ).ToList();

                    listProduct = context.Database.SqlQuery<TU_TRACKING_WF_PRODUCT_MODEL>
                   ("sp_ART_REPORT_TRACKING_AW_PRODUCT @where_pa, @where_so, @where_product, @where_process, @where_reference, @where_country"
                    , new SqlParameter("@where_pa", where_pa)
                    , new SqlParameter("@where_so", where_so)
                    , new SqlParameter("@where_product", where_product)
                    , new SqlParameter("@where_process", where_process)
                    , new SqlParameter("@where_reference", where_reference)
                    , new SqlParameter("@where_country", where_country)
                    ).ToList();

                    listProcess = context.Database.SqlQuery<TU_TRACKING_WF_PROCESS_MODEL>
                   ("sp_ART_REPORT_TRACKING_AW_PROCESS @where_pa, @where_so, @where_product, @where_process, @where_reference, @where_country"
                    , new SqlParameter("@where_pa", where_pa)
                    , new SqlParameter("@where_so", where_so)
                    , new SqlParameter("@where_product", where_product)
                    , new SqlParameter("@where_process", where_process)
                    , new SqlParameter("@where_reference", where_reference)
                    , new SqlParameter("@where_country", where_country)
                    ).ToList();

                    listReference = context.Database.SqlQuery<TU_TRACKING_WF_REFERENCE_MODEL>
                   ("sp_ART_REPORT_TRACKING_AW_REFERENCE @where_pa, @where_so, @where_product, @where_process, @where_reference, @where_country"
                    , new SqlParameter("@where_pa", where_pa)
                    , new SqlParameter("@where_so", where_so)
                    , new SqlParameter("@where_product", where_product)
                    , new SqlParameter("@where_process", where_process)
                    , new SqlParameter("@where_reference", where_reference)
                    , new SqlParameter("@where_country", where_country)
                    ).ToList();

                }
                else
                {              
                    listVendor = context.Database.SqlQuery<TU_TRACKING_WF_PROCESS_VENDOR_MODEL>
                    ("sp_ART_REPORT_TRACKING_MO_PROCESS_VENDOR @where_pg, @where_product, @where_process, @where_reference, @where_country"
                     , new SqlParameter("@where_pg", where_pa)
                     , new SqlParameter("@where_product", where_product)
                     , new SqlParameter("@where_process", where_process)
                     , new SqlParameter("@where_reference", where_reference)
                     , new SqlParameter("@where_country", where_country)
                     ).ToList();

                    listProduct = context.Database.SqlQuery<TU_TRACKING_WF_PRODUCT_MODEL>
                   ("sp_ART_REPORT_TRACKING_MO_PRODUCT @where_pg, @where_product, @where_process, @where_reference, @where_country"
                    , new SqlParameter("@where_pg", where_pa)
                    , new SqlParameter("@where_product", where_product)
                    , new SqlParameter("@where_process", where_process)
                    , new SqlParameter("@where_reference", where_reference)
                    , new SqlParameter("@where_country", where_country)
                    ).ToList();

                    listProcess = context.Database.SqlQuery<TU_TRACKING_WF_PROCESS_MODEL>
                   ("sp_ART_REPORT_TRACKING_MO_PROCESS @where_pg, @where_product, @where_process, @where_reference, @where_country"
                    , new SqlParameter("@where_pg", where_pa)
                    , new SqlParameter("@where_product", where_product)
                    , new SqlParameter("@where_process", where_process)
                    , new SqlParameter("@where_reference", where_reference)
                    , new SqlParameter("@where_country", where_country)
                    ).ToList();

                    listReference = context.Database.SqlQuery<TU_TRACKING_WF_REFERENCE_MODEL>
                   ("sp_ART_REPORT_TRACKING_MO_REFERENCE @where_pg, @where_product, @where_process, @where_reference, @where_country"
                    , new SqlParameter("@where_pg", where_pa)
                    , new SqlParameter("@where_product", where_product)
                    , new SqlParameter("@where_process", where_process)
                    , new SqlParameter("@where_reference", where_reference)
                    , new SqlParameter("@where_country", where_country)
                    ).ToList();
                }


           
                for (var i = 0; i < q.Count(); ++i)
                {

                    var WF_ID = q[i].WF_ID;
                    var WF_SUB_ID = q[i].WF_SUB_ID;
                    var REQUEST_ID = q[i].REQUEST_ID;


                    q[i].WF_SUB_PA_ID = q[i].WF_SUB_ID;

                    if (param.data.SEARCH_WF_TYPE_X == AW)
                    {
                        listSOTemp = listSO.Where(w => w.WF_SUB_ID == WF_SUB_ID).Distinct().OrderBy(o => (o.SALES_ORDER_NO, o.ITEM)).ToList();
                        TU_TRACKING_WF_ASSING_SO_MODEL.setDataWFAssignSO(q[i], listSOTemp);
                    }
                    else
                    {
                        listVendorTemp = listVendor.Where(w => w.WF_ID == WF_ID).Distinct().ToList();
                        TU_TRACKING_WF_PROCESS_VENDOR_MODEL.setDataWFProcessVendor(q[i], listVendorTemp);
                    }


                    listProductTemp = listProduct.Where(w => w.WF_ID == WF_ID).Distinct().OrderBy(o => o.PRODUCT_CODE).ToList();
                    TU_TRACKING_WF_PRODUCT_MODEL.setDataWFProduct(q[i], listProductTemp);

                    listReferenceTemp = listReference.Where(w => w.REQUEST_ID == REQUEST_ID).Distinct().ToList();
                    TU_TRACKING_WF_REFERENCE_MODEL.setDataWFReference(q[i], listReferenceTemp);

                    if (string.IsNullOrEmpty(q[i].IS_END))
                    {
                        listProcessTemp = listProcess.Where(w => w.WF_ID == WF_ID).Distinct().ToList();
                    }
                    else
                    {
                        listProcessTemp = new List<TU_TRACKING_WF_PROCESS_MODEL>();
                    }



                    TU_TRACKING_WF_REPORT_MODEL.setUserAssignAndDuration(q[i]);


                    //---------------------------
                    if (listProcessTemp != null && listProcessTemp.Count > 0)
                    {
                        TU_TRACKING_WF_REPORT_MODEL obj;
                        foreach (TU_TRACKING_WF_PROCESS_MODEL process in listProcessTemp)
                        {

                            if (process.WF_SUB_ID != q[i].WF_SUB_ID)
                            {
                                obj = new TU_TRACKING_WF_REPORT_MODEL();
                                obj = (TU_TRACKING_WF_REPORT_MODEL)q[i].Clone();
                                obj.CURRENT_STEP = process.CURRENT_STEP_NAME;
                                obj.CURRENT_ASSING = process.CURRENT_USER_NAME;
                                obj.DURATION = process.DURATION;
                                obj.DURATION_EXTEND = process.DURATION_EXTEND;
                                obj.IS_STEP_DURATION_EXTEND = process.IS_STEP_DURATION_EXTEND;
                                obj.WF_SUB_ID = process.WF_SUB_ID;
                                obj.PROCESS_CREATE_DATE = process.PROCESS_CREATE_DATE;
                                obj.CURRENT_STEP_ID = process.CURRENT_STEP_ID;
                                obj.CURRENT_STEP_CODE = process.CURRENT_STEP_CODE;
                                obj.CURRENT_DURATION = process.CURRENT_DURATION;
                                obj.CURRENT_DUE_DATE = process.CURRENT_DUE_DATE;
                                obj.CURRENT_USER_ID = process.CURRENT_USER_ID;
                                obj.DUE_DATE = process.DUE_DATE;
                                TU_TRACKING_WF_REPORT_MODEL.setUserAssignAndDuration(obj);
                                qTemp.Add(obj);
                            }
                        }

                    }
                }



            }


            //----------------------------------------------------

            if (qTemp != null && qTemp.Count > 0)
            {

                //var ListWF = q.Select(s => s.WF_NO).Distinct();
                //qTemp = qTemp.Where(w => ListWF.Contains(w.WF_NO)).ToList();
                foreach (var obj in qTemp)
                {
                    q.Add(obj);
                }
            }



        }

        public static void resetDataToListTrackingAWReportV3(TU_TRACKING_WF_REPORT_MODEL_REQUEST param, ref List<TU_TRACKING_WF_REPORT_MODEL> q, ref List<TU_TRACKING_WF_REPORT_MODEL> qTemp, ARTWORKEntities context)
        {

            //----------------------------------------------------------------------
            string AW = "AW";
            string IN_WF_ID = "0";
            string IN_WF_SUB_ID = "0";
            string IN_REQUEST_ID = "0";

            List<TU_TRACKING_WF_ASSING_SO_MODEL> listSOTemp, listSO;      // for PA only
            List<TU_TRACKING_WF_PRODUCT_MODEL> listProductTemp, listProduct;
            List<TU_TRACKING_WF_REFERENCE_MODEL> listReferenceTemp, listReference;
            List<TU_TRACKING_WF_PROCESS_MODEL> listProcessTemp, listProcess;
            List<TU_TRACKING_WF_PROCESS_VENDOR_MODEL> listVendorTemp, listVendor;   // for PG only

            string sqlAssingSO, sqlProduct, sqlProcess, sqlReference, sqlVendor;

            if (q != null && q.Count > 0)
            {
                for (var i = 0; i < q.Count(); ++i)
                {
                    IN_WF_ID += "," + q[i].WF_ID;
                    IN_WF_SUB_ID += "," + q[i].WF_SUB_ID;
                    IN_REQUEST_ID += "," + q[i].REQUEST_ID;
                }


                listSO = new List<TU_TRACKING_WF_ASSING_SO_MODEL>();
                listVendor = new List<TU_TRACKING_WF_PROCESS_VENDOR_MODEL>();
                if (param.data.SEARCH_WF_TYPE_X == AW)
                {
                    //string w = " wf_id in (select wf_id from V_ART_REPORT_TRACKING_AW_PROCESS_PA where REQUEST_CREATE_DATE >= '2021-01-01 00:00:00.000' and REQUEST_CREATE_DATE <= '2021-07-01 00:00:00.000') AND ISNULL(SALES_ORDER_NO,'') <> ''";
                    //sqlAssingSO = TU_TRACKING_WF_ASSING_SO_MODEL.getSQLWFAssingSO(w);
                    sqlAssingSO = TU_TRACKING_WF_ASSING_SO_MODEL.getSQLWFAssingSO("WF_SUB_ID IN (" + IN_WF_SUB_ID + ") AND ISNULL(SALES_ORDER_NO,'') <> '' ");
                    listSO = context.Database.SqlQuery<TU_TRACKING_WF_ASSING_SO_MODEL>(sqlAssingSO).ToList();
                }
                else
                {
                    sqlVendor = TU_TRACKING_WF_PROCESS_VENDOR_MODEL.getSQLWFProcessVendor("WF_ID IN (" + IN_WF_ID + ")");
                    listVendor = context.Database.SqlQuery<TU_TRACKING_WF_PROCESS_VENDOR_MODEL>(sqlVendor).ToList();
                }


                sqlProduct = TU_TRACKING_WF_PRODUCT_MODEL.getSQLWFProduct("WF_ID IN (" + IN_WF_ID + ") AND ISNULL(PRODUCT_CODE,'')  <> '' ", param.data.SEARCH_WF_TYPE_X);
                sqlProcess = TU_TRACKING_WF_PROCESS_MODEL.getSQLWFProcess("WF_ID IN (" + IN_WF_ID + ")", param.data.SEARCH_WF_TYPE_X);
                sqlReference = TU_TRACKING_WF_REFERENCE_MODEL.getSQLWFReference("REQUEST_ID IN (" + IN_REQUEST_ID + ")", param.data.SEARCH_WF_TYPE_X);

                listProduct = context.Database.SqlQuery<TU_TRACKING_WF_PRODUCT_MODEL>(sqlProduct).ToList();
                listProcess = context.Database.SqlQuery<TU_TRACKING_WF_PROCESS_MODEL>(sqlProcess).ToList();
                listReference = context.Database.SqlQuery<TU_TRACKING_WF_REFERENCE_MODEL>(sqlReference).ToList();
                //string ALL_SO_NO, ALL_PRODUCT;



                for (var i = 0; i < q.Count(); ++i)
                {

                    var WF_ID = q[i].WF_ID;
                    var WF_SUB_ID = q[i].WF_SUB_ID;
                    var REQUEST_ID = q[i].REQUEST_ID;


                    q[i].WF_SUB_PA_ID = q[i].WF_SUB_ID;

                    if (param.data.SEARCH_WF_TYPE_X == AW)
                    {
                        listSOTemp = listSO.Where(w => w.WF_SUB_ID == WF_SUB_ID).Distinct().OrderBy(o => (o.SALES_ORDER_NO, o.ITEM)).ToList();
                        TU_TRACKING_WF_ASSING_SO_MODEL.setDataWFAssignSO(q[i], listSOTemp);
                    }
                    else
                    {
                        listVendorTemp = listVendor.Where(w => w.WF_ID == WF_ID).Distinct().ToList();
                        TU_TRACKING_WF_PROCESS_VENDOR_MODEL.setDataWFProcessVendor(q[i], listVendorTemp);
                    }


                    listProductTemp = listProduct.Where(w => w.WF_ID == WF_ID).Distinct().OrderBy(o => o.PRODUCT_CODE).ToList();
                    TU_TRACKING_WF_PRODUCT_MODEL.setDataWFProduct(q[i], listProductTemp);

                    listReferenceTemp = listReference.Where(w => w.REQUEST_ID == REQUEST_ID).Distinct().ToList();
                    TU_TRACKING_WF_REFERENCE_MODEL.setDataWFReference(q[i], listReferenceTemp);

                    if (string.IsNullOrEmpty(q[i].IS_END))
                    {
                        listProcessTemp = listProcess.Where(w => w.WF_ID == WF_ID).Distinct().ToList();
                    }
                    else {
                        listProcessTemp = new List<TU_TRACKING_WF_PROCESS_MODEL>();
                    }
                

                    //if (q[i].IS_END == null)
                    //{
                    //    q[i].WF_STATUS = "In process";
                    //}
                    //else if (q[i].IS_TERMINATE != null)
                    //{
                    //    q[i].WF_STATUS = "Teminated";
                    //    q[i].CURRENT_STEP = "";
                    //    q[i].CURRENT_ASSING = "";
                    //}
                    //else
                    //{
                    //    q[i].WF_STATUS = "Completed";
                    //    q[i].CURRENT_STEP = "";
                    //    q[i].CURRENT_ASSING = "";
                    //}

                    //if (q[i].CURRENT_DUE_DATE != null)
                    //{
                       
                    //}

                    TU_TRACKING_WF_REPORT_MODEL.setUserAssignAndDuration(q[i]);


                    //---------------------------
                    if (listProcessTemp != null && listProcessTemp.Count > 0)
                    {
                        TU_TRACKING_WF_REPORT_MODEL obj;
                        foreach (TU_TRACKING_WF_PROCESS_MODEL process in listProcessTemp)
                        {

                            if (process.WF_SUB_ID != q[i].WF_SUB_ID)
                            {
                                obj = new TU_TRACKING_WF_REPORT_MODEL();
                                obj = (TU_TRACKING_WF_REPORT_MODEL)q[i].Clone();
                                obj.CURRENT_STEP = process.CURRENT_STEP_NAME;
                                obj.CURRENT_ASSING = process.CURRENT_USER_NAME;
                                obj.DURATION = process.DURATION;
                                obj.DURATION_EXTEND = process.DURATION_EXTEND;
                                obj.IS_STEP_DURATION_EXTEND = process.IS_STEP_DURATION_EXTEND;
                                obj.WF_SUB_ID = process.WF_SUB_ID;
                                obj.PROCESS_CREATE_DATE = process.PROCESS_CREATE_DATE;
                                obj.CURRENT_STEP_ID = process.CURRENT_STEP_ID;
                                obj.CURRENT_STEP_CODE = process.CURRENT_STEP_CODE;
                                obj.CURRENT_DURATION = process.CURRENT_DURATION;
                                obj.CURRENT_DUE_DATE = process.CURRENT_DUE_DATE;
                                obj.CURRENT_USER_ID = process.CURRENT_USER_ID;
                                obj.DUE_DATE = process.DUE_DATE;
                                TU_TRACKING_WF_REPORT_MODEL.setUserAssignAndDuration(obj);
                                qTemp.Add(obj);
                            }
                        }

                    }
                }



            }


            //----------------------------------------------------

            if (qTemp != null && qTemp.Count > 0)
            {

                //var ListWF = q.Select(s => s.WF_NO).Distinct();
                //qTemp = qTemp.Where(w => ListWF.Contains(w.WF_NO)).ToList();
                foreach (var obj in qTemp)
                {
                    q.Add(obj);
                }
            }



        }


        public static void orderbyColumnTrackingReportV3(TU_TRACKING_WF_REPORT_MODEL_REQUEST param, ref List<TU_TRACKING_WF_REPORT_MODEL> q, ref TU_TRACKING_WF_REPORT_MODEL_RESULT Results,bool isSkipPage)
        {

            if (!string.IsNullOrEmpty(param.data.GENERATE_EXCEL))
            {
                q = q.OrderBy(m => m.WF_NO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
            }
            else {

                var orderColumn = 1;
                var orderDir = "asc";
                Results.ORDER_COLUMN = 1;
                if (param.order != null && param.order.Count > 0)
                {
                    orderColumn = param.order[0].column;
                    orderDir = param.order[0].dir; //desc ,asc
                    Results.ORDER_COLUMN = param.order[0].column;
                }

                string orderASC = "asc";
                string orderDESC = "desc";
                List<string> listWFNO = new List<string>();


                switch (Results.ORDER_COLUMN)
                {
                    case 0:
                        if (orderDir == orderASC)
                        {
                             if (isSkipPage )
                                 q = q.OrderBy(o => (o.CREATOR_NAME, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                             else 
                             {
                                listWFNO = q.OrderBy(o => o.CREATOR_NAME).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.CREATOR_NAME).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                             }

                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else { 
                                listWFNO = q.OrderByDescending(o => o.CREATOR_NAME).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                 q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.CREATOR_NAME).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                            //
                        }
                        break;
                    case 1:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                listWFNO = q.OrderBy(o => o.WF_NO).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.WF_NO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                                //
                            }

                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                listWFNO = q.OrderByDescending(o => o.WF_NO).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.WF_NO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                                //
                            }
                        }
                        break;
                    case 2:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.PACKAGING_TYPE, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderBy(o => o.PACKAGING_TYPE).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.PACKAGING_TYPE).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.PACKAGING_TYPE, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderByDescending(o => o.PACKAGING_TYPE).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.PACKAGING_TYPE).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 3:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.PRIMARY_TYPE_TXT, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderBy(o => o.PRIMARY_TYPE_TXT).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.PRIMARY_TYPE_TXT).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }

                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.PRIMARY_TYPE_TXT, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderByDescending(o => o.PRIMARY_TYPE_TXT).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.PRIMARY_TYPE_TXT).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 4:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.WF_STATUS, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //  
                                listWFNO = q.OrderBy(o => o.WF_STATUS).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.WF_STATUS).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.WF_STATUS, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderByDescending(o => o.WF_STATUS).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.WF_STATUS).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 5:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.CURRENT_STEP, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderBy(o => o.CURRENT_STEP).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.CURRENT_STEP).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.CURRENT_STEP, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderByDescending(o => o.CURRENT_STEP).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.CURRENT_STEP).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 6:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.CURRENT_ASSING, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderBy(o => o.CURRENT_ASSING).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.CURRENT_ASSING).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.CURRENT_ASSING, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderByDescending(o => o.CURRENT_ASSING).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.CURRENT_ASSING).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 7:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.CURRENT_DURATION, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderBy(o => o.CURRENT_DURATION).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.CURRENT_DURATION).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.CURRENT_DURATION, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderByDescending(o => o.CURRENT_DURATION).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.CURRENT_DURATION).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 8:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.CURRENT_DUE_DATE, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderBy(o => o.CURRENT_DUE_DATE).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.CURRENT_DUE_DATE).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }

                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.CURRENT_DUE_DATE, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderByDescending(o => o.CURRENT_DUE_DATE).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.CURRENT_DUE_DATE).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 9:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.SOLD_TO, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //  
                                listWFNO = q.OrderBy(o => o.SOLD_TO).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.SOLD_TO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.SOLD_TO, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderByDescending(o => o.SOLD_TO).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.SOLD_TO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 10:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.SHIP_TO, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderBy(o => o.SHIP_TO).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.SHIP_TO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.SHIP_TO, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderByDescending(o => o.SHIP_TO).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.SHIP_TO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 11:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.PORT, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderBy(o => o.PORT).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.PORT).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.PORT, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderByDescending(o => o.PORT).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.PORT).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 12:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.IN_TRANSIT_TO, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderBy(o => o.IN_TRANSIT_TO).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.IN_TRANSIT_TO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.IN_TRANSIT_TO, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderByDescending(o => o.IN_TRANSIT_TO).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.IN_TRANSIT_TO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 13:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.SO_NO, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderBy(o => o.SO_NO).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.SO_NO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.SO_NO, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderByDescending(o => o.SO_NO).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.SO_NO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 14:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.SO_CREATE_DATE, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderBy(o => o.SO_CREATE_DATE).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.SO_CREATE_DATE).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.SO_CREATE_DATE, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderByDescending(o => o.SO_CREATE_DATE).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.SO_CREATE_DATE).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 15:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.BRAND_NAME, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderBy(o => o.BRAND_NAME).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.BRAND_NAME).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.BRAND_NAME, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderByDescending(o => o.BRAND_NAME).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.BRAND_NAME).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 16:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.ADDITIONAL_BRAND, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderBy(o => o.ADDITIONAL_BRAND).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.ADDITIONAL_BRAND).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.ADDITIONAL_BRAND, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderByDescending(o => o.ADDITIONAL_BRAND).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.ADDITIONAL_BRAND).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 17:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.PRODUCT_CODE, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderBy(o => o.PRODUCT_CODE).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.PRODUCT_CODE).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.PRODUCT_CODE, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderByDescending(o => o.PRODUCT_CODE).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.PRODUCT_CODE).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 18:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.PROD_INSP_MEMO, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //  
                                listWFNO = q.OrderBy(o => o.PROD_INSP_MEMO).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.PROD_INSP_MEMO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.PROD_INSP_MEMO, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderByDescending(o => o.PROD_INSP_MEMO).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.PROD_INSP_MEMO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 19:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.REFERENCE_NO, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderBy(o => o.REFERENCE_NO).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.REFERENCE_NO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.REFERENCE_NO, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderByDescending(o => o.REFERENCE_NO).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.REFERENCE_NO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 20:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.RDD, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //  
                                listWFNO = q.OrderBy(o => o.RDD).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.RDD).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.RDD, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderByDescending(o => o.RDD).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.RDD).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 21:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                               q = q.OrderBy(o => (o.VENDOR_RFQ, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // q
                                listWFNO = q.OrderBy(o => o.VENDOR_RFQ).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.VENDOR_RFQ).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.VENDOR_RFQ, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                //
                                listWFNO = q.OrderByDescending(o => o.VENDOR_RFQ).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.VENDOR_RFQ).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    case 22:
                        if (orderDir == orderASC)
                        {
                            if (isSkipPage)
                                q = q.OrderBy(o => (o.SELECTED_VENDOR, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderBy(o => o.SELECTED_VENDOR).Select(s => s.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderBy(m => m.SELECTED_VENDOR).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        else if (orderDir == orderDESC)
                        {
                            if (isSkipPage)
                                q = q.OrderByDescending(o => (o.SELECTED_VENDOR, o.WF_NO, o.PROCESS_CREATE_DATE)).ToList();
                            else
                            {
                                // 
                                listWFNO = q.OrderByDescending(o => o.SELECTED_VENDOR).Select(s => s.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                                q = q.Where(w => listWFNO.Contains(w.WF_NO)).OrderByDescending(m => m.SELECTED_VENDOR).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
                            }
                        }
                        break;
                    default:

                        break;
                }


            }
        }

        public static bool isSortColumnSkipPageV3(TU_TRACKING_WF_REPORT_MODEL_REQUEST param, ref List<TU_TRACKING_WF_REPORT_MODEL> q, ref TU_TRACKING_WF_REPORT_MODEL_RESULT Results,bool isGoodPerformance)
        {
            bool isOrderby = true;


            var orderColumn = 1;
            var orderDir = "asc";
            Results.ORDER_COLUMN = 1;
            if (param.order != null && param.order.Count > 0)
            {
                orderColumn = param.order[0].column;
                orderDir = param.order[0].dir; //desc ,asc
                Results.ORDER_COLUMN = param.order[0].column;
            }

            string orderASC = "asc";
            string orderDESC = "desc";

            Results.recordsTotal = q.Count();
            Results.recordsFiltered = q.Count();

            switch (Results.ORDER_COLUMN)
            {
                case 0:
          
                    if (orderDir == orderASC)
                    {
                        q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    break;
                case 1:
                    if (orderDir == orderASC)
                    {
                        q = q.Distinct().OrderBy(o => (o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        q = q.Distinct().OrderByDescending(o => (o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    break;
                case 2:
                    if (orderDir == orderASC)
                    {
                        q = q.Distinct().OrderBy(o => (o.PACKAGING_TYPE, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        q = q.Distinct().OrderByDescending(o => (o.PACKAGING_TYPE, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    break;
                case 3:
                    if (orderDir == orderASC)
                    {
                        q = q.Distinct().OrderBy(o => (o.PRIMARY_TYPE_TXT, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        q = q.Distinct().OrderByDescending(o => (o.PRIMARY_TYPE_TXT, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    break;
                case 4:
                    if (orderDir == orderASC)
                    {
                        q = q.Distinct().OrderBy(o => (o.WF_STATUS, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        q = q.Distinct().OrderByDescending(o => (o.WF_STATUS, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }

                    break;
                case 5:
                    if (!isGoodPerformance)
                    {
                        if (orderDir == orderASC)
                        {
                            q = q.Distinct().OrderBy(o => (o.CURRENT_STEP, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                        else if (orderDir == orderDESC)
                        {
                            q = q.Distinct().OrderByDescending(o => (o.CURRENT_STEP, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                    }
                    isOrderby = false;

                    break;
                case 6:
                    if (!isGoodPerformance)
                    {
                        if (orderDir == orderASC)
                        {
                            q = q.Distinct().OrderBy(o => (o.CURRENT_ASSING, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                        else if (orderDir == orderDESC)
                        {
                            q = q.Distinct().OrderByDescending(o => (o.CURRENT_ASSING, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                    }

                    isOrderby = false;
                    break;
                case 7:

                    if (!isGoodPerformance)
                    {
                        if (orderDir == orderASC)
                        {
                            q = q.Distinct().OrderBy(o => (o.CURRENT_DURATION, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                        else if (orderDir == orderDESC)
                        {
                            q = q.Distinct().OrderByDescending(o => (o.CURRENT_DURATION, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                    }
                    isOrderby = false;
                    break;
                case 8:
                    if (!isGoodPerformance)
                    {
                        if (orderDir == orderASC)
                        {
                            q = q.Distinct().OrderBy(o => (o.CURRENT_DUE_DATE, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                        else if (orderDir == orderDESC)
                        {
                            q = q.Distinct().OrderByDescending(o => (o.CURRENT_DUE_DATE, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                    }

                    isOrderby = false;
                    break;
                case 9:
                    if (orderDir == orderASC)
                    {
                        q = q.Distinct().OrderBy(o => (o.SOLD_TO, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        q = q.Distinct().OrderByDescending(o => (o.SOLD_TO, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    break;
                case 10:
                    if (orderDir == orderASC)
                    {
                        q = q.Distinct().OrderBy(o => (o.SHIP_TO, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        q = q.Distinct().OrderByDescending(o => (o.SHIP_TO, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    break;
                case 11:
                    if (!isGoodPerformance)
                    {
                        if (orderDir == orderASC)
                        {
                            q = q.Distinct().OrderBy(o => (o.PORT, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                        else if (orderDir == orderDESC)
                        {
                            q = q.Distinct().OrderByDescending(o => (o.PORT, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                    }
                  
                    isOrderby = false;
                    break;
                case 12:
                    if (!isGoodPerformance)
                    {
                        if (orderDir == orderASC)
                        {
                            q = q.Distinct().OrderBy(o => (o.IN_TRANSIT_TO, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                        else if (orderDir == orderDESC)
                        {
                            q = q.Distinct().OrderByDescending(o => (o.IN_TRANSIT_TO, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                    }                    
                    isOrderby = false;
                    break;
                case 13:
                    if (!isGoodPerformance)
                    {
                        if (orderDir == orderASC)
                        {
                            q = q.Distinct().OrderBy(o => (o.SO_NO, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                        else if (orderDir == orderDESC)
                        {

                        }
                        q = q.Distinct().OrderByDescending(o => (o.SO_NO, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    isOrderby = false;
                    break;
                case 14:
                    if (!isGoodPerformance)
                    {
                        if (orderDir == orderASC)
                        {
                            q = q.Distinct().OrderBy(o => (o.SO_CREATE_DATE, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                        else if (orderDir == orderDESC)
                        {
                            q = q.Distinct().OrderByDescending(o => (o.SO_CREATE_DATE, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                    }
                      
                    isOrderby = false;
                    break;                    
                case 15:
                    if (orderDir == orderASC)
                    {
                        q = q.Distinct().OrderBy(o => (o.BRAND_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        q = q.Distinct().OrderByDescending(o => (o.BRAND_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                    }
                    isOrderby = true;
                    break;                        
                case 16:
                    if (!isGoodPerformance)
                    {
                        if (orderDir == orderASC)
                        {
                            q = q.Distinct().OrderBy(o => (o.ADDITIONAL_BRAND, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                        else if (orderDir == orderDESC)
                        {
                            q = q.Distinct().OrderByDescending(o => (o.ADDITIONAL_BRAND, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                    }
                
                    isOrderby = false;
                    break;
                case 17:
                    if (!isGoodPerformance)
                    {
                        if (orderDir == orderASC)
                        {
                            q = q.Distinct().OrderBy(o => (o.PRODUCT_CODE, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                        else if (orderDir == orderDESC)
                        {
                            q = q.Distinct().OrderByDescending(o => (o.PRODUCT_CODE, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                    }
               
                    isOrderby = false;
                    break;
                case 18:
                    if (!isGoodPerformance)
                    {
                        if (orderDir == orderASC)
                        {
                            q = q.Distinct().OrderBy(o => (o.PROD_INSP_MEMO, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                        else if (orderDir == orderDESC)
                        {
                            q = q.Distinct().OrderByDescending(o => (o.PROD_INSP_MEMO, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                    }
             
                    isOrderby = false;
                    break;
                case 19:
                    if (!isGoodPerformance)
                    {
                        if (orderDir == orderASC)
                        {
                            q = q.Distinct().OrderBy(o => (o.REFERENCE_NO, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                        else if (orderDir == orderDESC)
                        {
                            q = q.Distinct().OrderByDescending(o => (o.REFERENCE_NO, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                    }
                  
                    isOrderby = false;
                    break;
                case 20:
                    if (!isGoodPerformance)
                    {
                        if (orderDir == orderASC)
                        {
                            q = q.Distinct().OrderBy(o => (o.RDD, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                        else if (orderDir == orderDESC)
                        {
                            q = q.Distinct().OrderByDescending(o => (o.RDD, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                    }
                   
                    isOrderby = false;
                    break;
                case 21:
                    if (!isGoodPerformance)
                    {
                        if (orderDir == orderASC)
                        {
                            q = q.Distinct().OrderBy(o => (o.VENDOR_RFQ, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                        else if (orderDir == orderDESC)
                        {
                            q = q.Distinct().OrderByDescending(o => (o.VENDOR_RFQ, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                    }                
                    isOrderby = false;
                    break;
                case 22:
                    if (!isGoodPerformance)
                    {
                        if (orderDir == orderASC)
                        {
                            q = q.Distinct().OrderBy(o => (o.SELECTED_VENDOR, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                        else if (orderDir == orderDESC)
                        {
                            q = q.Distinct().OrderByDescending(o => (o.SELECTED_VENDOR, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
                        }
                    }          
                    isOrderby = false;
                    break;
                default:
                    isOrderby = false;
                    break;
            }


            return isOrderby;
        }



        public static V_ART_ENDTOEND_REPORT_RESULT GetViewTrackingReport(V_ART_ENDTOEND_REPORT_REQUEST param)
        {
            V_ART_ENDTOEND_REPORT_RESULT Results = new V_ART_ENDTOEND_REPORT_RESULT();
            Results.data = new List<V_ART_ENDTOEND_REPORT_2>();
            Results.dataExcel = new List<V_ART_ENDTOEND_REPORT_2>();

            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                QueryTrackingReport(param, ref Results);// q.OrderBy(i => i.REQUEST_NO).ThenBy(i => i.WF_NO).ThenBy(i => i.STEP_CREATE_DATE).ToList();

                Results.status = "S";
                Results.draw = param.draw;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static List<V_ART_ENDTOEND_REPORT_2> QueryTrackingReport(V_ART_ENDTOEND_REPORT_REQUEST param, ref V_ART_ENDTOEND_REPORT_RESULT Results)
        {
            IQueryable<V_ART_ENDTOEND_REPORT_2> q;

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;

                    DateTime REQUEST_DATE_FROM = DateTime.MinValue;
                    DateTime REQUEST_DATE_TO = DateTime.MaxValue;
                    var currDate = DateTime.Now;

                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_FROM)) REQUEST_DATE_FROM = CNService.ConvertStringToDate(param.data.REQUEST_DATE_FROM);
                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_TO)) REQUEST_DATE_TO = CNService.ConvertStringToDate(param.data.REQUEST_DATE_TO);

                    q = (from m in context.V_ART_ENDTOEND_REPORT
                         where DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= REQUEST_DATE_FROM && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= REQUEST_DATE_TO
                         && ((m.PARENT_SUB_ID > 0 && string.IsNullOrEmpty(m.IS_END)) || m.PARENT_SUB_ID == null)
                         select new V_ART_ENDTOEND_REPORT_2
                         {
                             PARENT_SUB_ID = m.PARENT_SUB_ID,
                             DUE_DATE = m.DUE_DATE,
                             WF_NO = m.WF_NO,
                             CURRENT_USER_ID = m.CURRENT_USER_ID,
                             REFERENCE_REQUEST_NO = m.REFERENCE_REQUEST_NO,
                             REQUEST_NO = m.REQUEST_NO,
                             IS_END = m.IS_END,
                             IS_TERMINATE = m.IS_TERMINATE,
                             SOLD_TO_ID = m.SOLD_TO_ID,
                             SHIP_TO_ID = m.SHIP_TO_ID,
                             SOLD_TO = m.SOLD_TO,
                             SHIP_TO = m.SHIP_TO,
                             COUNTRY = m.COUNTRY,
                             REQUEST_ID = m.REQUEST_ID,
                             COMPANY_ID = m.COMPANY_ID,
                             BRAND_ID = m.BRAND_ID,
                             PROJECT_NAME = m.PROJECT_NAME,
                             PRODUCT_CODE = m.PRODUCT_CODE,
                             WF_ID = m.WF_ID,
                             REFERENCE_NO = m.REFERENCE_NO,
                             PACKAGING_TYPE = m.PACKAGING_TYPE,
                             CREATOR_ID = m.CREATOR_ID,
                             THREE_P_ID = m.THREE_P_ID,
                             CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                             SALES_ORDER_NO = m.SALES_ORDER_NO,
                             ORDER_BOM_COMPONENT = m.ORDER_BOM_COMPONENT,
                             WF_STAUTS = m.WF_STAUTS,
                             CURRENT_STEP_NAME_1 = m.CURRENT_STEP_NAME_1,
                             CURRENT_USER_NAME_1 = m.CURRENT_USER_NAME_1,
                             DUE_DATE_1 = m.DUE_DATE_1,
                             DURATION_1 = m.DURATION_1,
                             BRAND_NAME = m.BRAND_NAME,
                             ADDITIONAL_BRAND_NAME = m.ADDITIONAL_BRAND_NAME,
                             PROD_INSP_MEMO = m.PROD_INSP_MEMO,
                             PLANT = m.PLANT,
                             PORT = m.PORT,
                             IN_TRANSIT_TO = m.IN_TRANSIT_TO,
                             RDD = m.RDD,
                             PA_NAME = m.PA_NAME,
                             PG_NAME = m.PG_NAME,
                             CREATE_ON = m.CREATE_ON,
                             //CURRENT_STEP_NAME = m.CURRENT_STEP_NAME,
                             //CURRENT_USER_NAME = m.CURRENT_USER_NAME,
                             //STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                             //STEP_END_DATE = m.STEP_END_DATE,
                             //DURATION = m.DURATION,
                             //REASON = m.REASON,
                             //TOTAL_DAY = m.TOTAL_DAY,
                             //MARKETTING = m.MARKETTING,
                             CREATOR_NAME = m.CREATOR_NAME,
                             //USE_DAY = m.USE_DAY,
                             WF_SUB_ID = m.WF_SUB_ID,
                             STEP_CREATE_DATE_ORDERBY = m.STEP_CREATE_DATE,
                             PRIMARY_TYPE_TXT = m.PRIMARY_TYPE_TXT,
                             WF_TYPE = m.WF_TYPE,
                             STEP_END_DATE_STATIC = m.STEP_END_DATE == null ? currDate : m.STEP_END_DATE,
                         });

                    if (param.data.WORKFLOW_OVERDUE)
                    {
                        q = q.Where(e => e.DUE_DATE < e.STEP_END_DATE_STATIC);
                    }

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_TYPE))
                    {
                        q = q.Where(e => e.WF_NO.StartsWith(param.data.WORKFLOW_TYPE));
                    }

                    if (param.data.WORKFLOW_ACTION_BY_ME)
                    {
                        param.data.CURRENT_ASSIGN_ID = param.data.CURRENT_USER_ID;
                    }

                    if (param.data.CURRENT_ASSIGN_ID > 0)
                    {
                        q = q.Where(e => e.CURRENT_USER_ID == param.data.CURRENT_ASSIGN_ID);
                    }

                    if (param.data.SUPERVISED_ID > 0)
                    {
                        var listUserId = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { UPPER_USER_ID = (int)param.data.SUPERVISED_ID }, context).Select(m => m.USER_ID).ToList();
                        q = q.Where(e => e.CURRENT_USER_ID != null && listUserId.Contains((int)e.CURRENT_USER_ID));
                    }

                    if (!string.IsNullOrEmpty(param.data.REF_WF_NO))
                    {
                        q = q.Where(e => ((!string.IsNullOrEmpty(e.REFERENCE_REQUEST_NO) && e.REFERENCE_REQUEST_NO.Contains(param.data.REF_WF_NO.Trim()))
                       || (string.IsNullOrEmpty(e.REFERENCE_REQUEST_NO) && e.REQUEST_NO.Contains(param.data.REF_WF_NO.Trim()))));
                    }

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_NO))
                    {
                        q = q.Where(e => e.WF_NO.Contains(param.data.WORKFLOW_NO.Trim()));
                    }

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_NO_2))
                    {
                        q = q.Where(e => e.REQUEST_NO.Contains(param.data.WORKFLOW_NO_2.Trim()));
                    }

                    if (param.data.WORKFLOW_COMPLETED)
                    {
                        //var tempListCompleted = (from e in context.V_ART_ENDTOEND_REPORT
                        //                         where e.IS_END == "X"
                        //                         && string.IsNullOrEmpty(e.IS_TERMINATE)
                        //                         && DbFunctions.TruncateTime(e.REQUEST_CREATE_DATE) >= REQUEST_DATE_FROM && DbFunctions.TruncateTime(e.REQUEST_CREATE_DATE) <= REQUEST_DATE_TO
                        //                         && e.PARENT_SUB_ID == null
                        //                         select e.WF_ID).ToList();
                        //q = q.Where(e => tempListCompleted.Contains(e.WF_ID));
                        q = q.Where(e => e.IS_END == "X" && string.IsNullOrEmpty(e.IS_TERMINATE) && e.PARENT_SUB_ID == null);
                    }

                    if (param.data.WORKFLOW_IN_PROCESS)
                    {
                        q = q.Where(e => string.IsNullOrEmpty(e.IS_END));
                    }

                    if (param.data.SOLD_TO_ID > 0)
                    {
                        q = q.Where(e => e.SOLD_TO_ID == (int)(param.data.SOLD_TO_ID));
                    }

                    if (param.data.SHIP_TO_ID > 0)
                    {
                        q = q.Where(e => e.SHIP_TO_ID == (int)(param.data.SHIP_TO_ID));
                    }

                    if (param.data.COUNTRY_ID > 0)
                    {
                        var country = SAP_M_COUNTRY_SERVICE.GetByItem(new SAP_M_COUNTRY() { COUNTRY_ID = (int)param.data.COUNTRY_ID }, context).FirstOrDefault();
                        q = q.Where(e => e.COUNTRY.Contains(country.COUNTRY_CODE + ":" + country.NAME));
                    }

                    if (!string.IsNullOrEmpty(param.data.ZONE_TXT))
                    {
                        var listCountryId = SAP_M_COUNTRY_SERVICE.GetByItem(new SAP_M_COUNTRY() { ZONE = param.data.ZONE_TXT }, context).Select(m => m.COUNTRY_ID);
                        //mockup
                        var listCheckListId = (from r in context.ART_WF_MOCKUP_CHECK_LIST_COUNTRY
                                               where listCountryId.Contains(r.COUNTRY_ID)
                                               select r.CHECK_LIST_ID).ToList();

                        //artwork
                        var listARTWORK_REQUEST_ID = (from r in context.ART_WF_ARTWORK_REQUEST_COUNTRY
                                                      where listCountryId.Contains(r.COUNTRY_ID)
                                                      select r.ARTWORK_REQUEST_ID).ToList();

                        q = q.Where(e => listCheckListId.Contains(e.REQUEST_ID) || listARTWORK_REQUEST_ID.Contains(e.REQUEST_ID));
                    }

                    if (param.data.COMPANY_ID > 0)
                    {
                        q = q.Where(e => e.COMPANY_ID == (int)(param.data.COMPANY_ID));
                    }

                    if (param.data.BRAND_ID > 0)
                    {
                        q = q.Where(e => e.BRAND_ID == (int)(param.data.BRAND_ID));
                    }

                    if (!string.IsNullOrEmpty(param.data.PROJECT_NAME))
                    {
                        q = q.Where(e => e.PROJECT_NAME.Contains(param.data.PROJECT_NAME));
                    }

                    if (!string.IsNullOrEmpty(param.data.PRODUCT_CODE))//********************* multi *********************
                    {
                        //string[] arrProductCode = param.data.PRODUCT_CODE.Split(',');
                        //arrProductCode = arrProductCode.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        //List<int> arrWFId = new List<int>();
                        //for (var i = 0; i < arrProductCode.Length; i++)
                        //{
                        //    string prodCode = arrProductCode[i];
                        //    List<int> listWFId = q.Where(e => !string.IsNullOrEmpty(e.PRODUCT_CODE) && e.PRODUCT_CODE.Contains(prodCode)).Select(e => e.WF_ID).ToList();
                        //    arrWFId.InsertRange(0, listWFId);
                        //}
                        //q = q.Where(e => arrWFId.Contains(e.WF_ID));

                        var arrProductCode = param.data.PRODUCT_CODE.Replace(" ", "").Split(',');
                        var listArtworkRequestId = (from m in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM
                                                    join m2 in context.ART_WF_ARTWORK_PROCESS on m.ARTWORK_SUB_ID equals m2.ARTWORK_SUB_ID
                                                    where arrProductCode.Contains(m.PRODUCT_CODE)
                                                    select m2.ARTWORK_ITEM_ID).ToList();
                        q = (from r in q
                             where listArtworkRequestId.Contains(r.WF_ID)
                             select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.RD_NUMBER))
                    {
                        q = q.Where(e => !string.IsNullOrEmpty(e.REFERENCE_NO) && e.REFERENCE_NO.Contains(param.data.RD_NUMBER.Trim()));
                    }

                    if (param.data.PACKAGING_TYPE_ID > 0)
                    {
                        var packaging = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(new SAP_M_CHARACTERISTIC() { CHARACTERISTIC_ID = (int)param.data.PACKAGING_TYPE_ID }, context).FirstOrDefault();
                        q = q.Where(e => !string.IsNullOrEmpty(e.PACKAGING_TYPE) && e.PACKAGING_TYPE.Contains(packaging.VALUE + ":" + packaging.DESCRIPTION));
                    }

                    if (param.data.WORKING_GROUP_ID > 0)
                    {
                        var tempListWorking = (from e in context.V_ART_ENDTOEND_REPORT
                                               where e.CURRENT_USER_ID == param.data.WORKING_GROUP_ID
                                               && DbFunctions.TruncateTime(e.REQUEST_CREATE_DATE) >= REQUEST_DATE_FROM && DbFunctions.TruncateTime(e.REQUEST_CREATE_DATE) <= REQUEST_DATE_TO
                                               select e.WF_ID).ToList();

                        q = (from r in q where tempListWorking.Contains(r.WF_ID) select r);
                    }

                    if (param.data.CREATOR_ID > 0)
                    {
                        q = q.Where(e => e.CREATOR_ID == param.data.CREATOR_ID);
                    }

                    if (!string.IsNullOrEmpty(param.data.PRIMARY_SIZE_TXT))
                    {
                        //search from SAP_M_3P
                        List<int> listThreePID = SAP_M_3P_SERVICE.GetAll(context).Where(s => s.PRIMARY_SIZE_VALUE.Contains(param.data.PRIMARY_SIZE_TXT.Trim())).Select(s => s.THREE_P_ID).ToList();

                        //search from request id
                        List<int> listChecklistId = ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetAll(context).Where(p => p.PRIMARY_SIZE.Contains(param.data.PRIMARY_SIZE_TXT.Trim())).Select(p => p.CHECK_LIST_ID).ToList();
                        List<int> listProductId = XECM_M_PRODUCT_SERVICE.GetAll(context).Where(p => p.PRIMARY_SIZE != null && p.PRIMARY_SIZE.Contains(param.data.PRIMARY_SIZE_TXT.Trim())).Select(p => p.XECM_PRODUCT_ID).ToList();
                        List<int> listRequestId = ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetAll(context).Where(p => listProductId.Contains(p.PRODUCT_CODE_ID)).Select(p => p.ARTWORK_REQUEST_ID).ToList();

                        q = q.Where(e => (e.THREE_P_ID != null && listThreePID.Contains((int)e.THREE_P_ID)) || listChecklistId.Contains(e.REQUEST_ID) || listRequestId.Contains(e.REQUEST_ID));
                    }

                    if (!string.IsNullOrEmpty(param.data.NET_WEIGHT_TXT))
                    {
                        List<int> listAWNetWeight = ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_REFERENCE() { NET_WEIGHT = param.data.NET_WEIGHT_TXT.Trim() }, context).Select(e => e.ARTWORK_REQUEST_ID).ToList();
                        List<int> listMockNetWeight = ART_WF_MOCKUP_CHECK_LIST_REFERENCE_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_REFERENCE() { NET_WEIGHT = param.data.NET_WEIGHT_TXT.Trim() }, context).Select(e => e.CHECK_LIST_ID).ToList();

                        q = q.Where(e => listAWNetWeight.Contains(e.REQUEST_ID) || listMockNetWeight.Contains(e.REQUEST_ID));
                    }

                    if (!string.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE))
                    {
                        if (param.data.CURRENT_STEP_WF_TYPE.StartsWith("M"))
                        {
                            var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP_WF_TYPE.Replace("M", ""));
                            q = q.Where(e => e.CURRENT_STEP_ID == current_step_id && e.WF_TYPE.Equals("Mockup"));
                        }
                        else
                        {
                            var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP_WF_TYPE.Replace("A", ""));
                            q = q.Where(e => e.CURRENT_STEP_ID == current_step_id && e.WF_TYPE.Equals("Artwork"));
                        }
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_SO))//********************* multi *********************
                    {
                        //string[] arrSO = param.data.SEARCH_SO.Split(',');
                        //arrSO = arrSO.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        //List<int> arrWFId = new List<int>();
                        //for (var i = 0; i < arrSO.Length; i++)
                        //{
                        //    string so = arrSO[i];
                        //    arrWFId.InsertRange(0, q.Where(e => !string.IsNullOrEmpty(e.SALES_ORDER_NO) && e.SALES_ORDER_NO.Contains(so)).Select(e => e.WF_ID).ToList());
                        //}
                        //q = q.Where(e => arrWFId.Contains(e.WF_ID));

                        var arrSO = param.data.SEARCH_SO.Replace(" ", "").Split(',');
                        var listArtworkRequestId = (from m in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
                                                    join m2 in context.ART_WF_ARTWORK_PROCESS on m.ARTWORK_SUB_ID equals m2.ARTWORK_SUB_ID
                                                    where arrSO.Contains(m.SALES_ORDER_NO)
                                                    select m2.ARTWORK_ITEM_ID).ToList();
                        q = (from r in q
                             where listArtworkRequestId.Contains(r.WF_ID)
                             select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_ORDER_BOM))//********************* multi *********************
                    {
                        //string[] arrOrderBOM = param.data.SEARCH_ORDER_BOM.Split(',');
                        //arrOrderBOM = arrOrderBOM.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        //List<int> arrWFId = new List<int>();
                        //for (var i = 0; i < arrOrderBOM.Length; i++)
                        //{
                        //    string orderBom = arrOrderBOM[i];
                        //    arrWFId.InsertRange(0, q.Where(e => !string.IsNullOrEmpty(e.ORDER_BOM_COMPONENT) && e.ORDER_BOM_COMPONENT.Contains(orderBom)).Select(e => e.WF_ID).ToList());
                        //}
                        //q = q.Where(e => arrWFId.Contains(e.WF_ID));

                        var arrOrderBOM = param.data.SEARCH_ORDER_BOM.Replace(" ", "").Split(',');
                        var listArtworkRequestId = (from m in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT
                                                    join m2 in context.ART_WF_ARTWORK_PROCESS on m.ARTWORK_SUB_ID equals m2.ARTWORK_SUB_ID
                                                    where arrOrderBOM.Contains(m.COMPONENT_MATERIAL)
                                                    select m2.ARTWORK_ITEM_ID).ToList();
                        q = (from r in q
                             where listArtworkRequestId.Contains(r.WF_ID)
                             select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.SO_CREATE_DATE_FROM))
                    {
                        //q = q.Where(e => !string.IsNullOrEmpty(e.CREATE_ON) && CNService.ConvertStringToDate(e.CREATE_ON) >= CNService.ConvertStringToDate(param.data.SO_CREATE_DATE_FROM));
                    }

                    if (!string.IsNullOrEmpty(param.data.SO_CREATE_DATE_TO))
                    {
                        //q = q.Where(e => !string.IsNullOrEmpty(e.CREATE_ON) && CNService.ConvertStringToDate(e.CREATE_ON) <= CNService.ConvertStringToDate(param.data.SO_CREATE_DATE_TO));
                    }

                    if (param.data.CUSTOMER_APPROVE_FROM != 0)
                    {

                    }

                    if (param.data.CUSTOMER_APPROVE_TO != 0)
                    {

                    }

                    if (param.data.END_TO_END_FROM != 0)
                    {

                    }

                    if (param.data.END_TO_END_TO != 0)
                    {

                    }

                    OrderByTrackingReport(param, q, ref Results);
                }
            }

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    foreach (var item in Results.data)
                    {
                        item.COUNTRY = CNService.removeLastComma(item.COUNTRY);
                        item.PORT = CNService.removeLastComma(item.PORT);
                        item.IN_TRANSIT_TO = CNService.removeLastComma(item.IN_TRANSIT_TO);
                        item.PLANT = CNService.removeLastComma(item.PLANT);
                        item.PROD_INSP_MEMO = CNService.removeLastComma(item.PROD_INSP_MEMO);
                        item.PRODUCT_CODE = CNService.removeLastComma(item.PRODUCT_CODE);
                        item.ADDITIONAL_BRAND_NAME = CNService.removeLastComma(item.ADDITIONAL_BRAND_NAME);
                        item.SALES_ORDER_NO = CNService.removeLastComma(item.SALES_ORDER_NO);
                        item.REFERENCE_NO = CNService.removeLastComma(item.REFERENCE_NO);
                        item.RDD = CNService.removeLastComma(item.RDD);
                        item.CREATE_ON = CNService.removeLastComma(item.CREATE_ON);

                        item.VENDOR_RFQ = CNService.GetVendorRFQ(item.WF_ID, item.WF_SUB_ID, context);
                        item.SELECTED_VENDOR = CNService.GetVendorSelected(item.WF_ID, item.WF_SUB_ID, context);
                    }
                }
            }

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    foreach (var item in Results.dataExcel)
                    {
                        item.COUNTRY = CNService.removeLastComma(item.COUNTRY);
                        item.PORT = CNService.removeLastComma(item.PORT);
                        item.IN_TRANSIT_TO = CNService.removeLastComma(item.IN_TRANSIT_TO);
                        item.PLANT = CNService.removeLastComma(item.PLANT);
                        item.PROD_INSP_MEMO = CNService.removeLastComma(item.PROD_INSP_MEMO);
                        item.PRODUCT_CODE = CNService.removeLastComma(item.PRODUCT_CODE);
                        item.ADDITIONAL_BRAND_NAME = CNService.removeLastComma(item.ADDITIONAL_BRAND_NAME);
                        item.SALES_ORDER_NO = CNService.removeLastComma(item.SALES_ORDER_NO);
                        item.RD_NUMBER = CNService.removeLastComma(item.RD_NUMBER);
                        item.RDD = CNService.removeLastComma(item.RDD);
                        item.CREATE_ON = CNService.removeLastComma(item.CREATE_ON);

                        item.VENDOR_RFQ = CNService.GetVendorRFQ(item.WF_ID, item.WF_SUB_ID, context);
                        item.SELECTED_VENDOR = CNService.GetVendorSelected(item.WF_ID, item.WF_SUB_ID, context);
                    }
                }
            }

            return Results.data;
        }

        private static void OrderByTrackingReport(V_ART_ENDTOEND_REPORT_REQUEST param, IQueryable<V_ART_ENDTOEND_REPORT_2> q, ref V_ART_ENDTOEND_REPORT_RESULT Results)
        {
            if (!string.IsNullOrEmpty(param.data.GENERATE_EXCEL))
            {
                Results.dataExcel = q.OrderBy(m => m.WF_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
            }
            else
            {
                var orderColumn = 1;
                var orderDir = "asc";
                Results.ORDER_COLUMN = 1;
                if (param.order != null && param.order.Count > 0)
                {
                    orderColumn = param.order[0].column;
                    orderDir = param.order[0].dir; //desc ,asc
                    Results.ORDER_COLUMN = param.order[0].column;
                }

                string orderASC = "asc";
                string orderDESC = "desc";
                List<string> temp = new List<string>();

                if (orderColumn == 0)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.CREATOR_NAME).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.CREATOR_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.CREATOR_NAME).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.CREATOR_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 1)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.WF_NO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.WF_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.WF_NO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.WF_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 2)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.PACKAGING_TYPE).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PACKAGING_TYPE).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.PACKAGING_TYPE).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PACKAGING_TYPE).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 3)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.PRIMARY_TYPE_TXT).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PRIMARY_TYPE_TXT).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.PRIMARY_TYPE_TXT).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PRIMARY_TYPE_TXT).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 4)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.WF_STAUTS).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.WF_STAUTS).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.WF_STAUTS).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.WF_STAUTS).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 5)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.CURRENT_STEP_NAME_1).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.CURRENT_STEP_NAME_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.CURRENT_STEP_NAME_1).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.CURRENT_STEP_NAME_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 6)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.CURRENT_USER_NAME_1).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.CURRENT_USER_NAME_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.CURRENT_USER_NAME_1).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.CURRENT_USER_NAME_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 7)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.DURATION_1).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.DURATION_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.DURATION_1).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.DURATION_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 8)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.DUE_DATE_1).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.DUE_DATE_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.DUE_DATE_1).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.DUE_DATE_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 9)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.SOLD_TO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.SOLD_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.SOLD_TO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.SOLD_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 10)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.SHIP_TO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.SHIP_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.SHIP_TO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.SHIP_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 11)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.PORT).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PORT).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.PORT).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PORT).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 12)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.IN_TRANSIT_TO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.IN_TRANSIT_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.IN_TRANSIT_TO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.IN_TRANSIT_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 13)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.SALES_ORDER_NO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.SALES_ORDER_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.SALES_ORDER_NO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.SALES_ORDER_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 14)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.CREATE_ON).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.CREATE_ON).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.CREATE_ON).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.CREATE_ON).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 15)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.BRAND_NAME).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.BRAND_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.BRAND_NAME).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.BRAND_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 16)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.ADDITIONAL_BRAND_NAME).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.ADDITIONAL_BRAND_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.ADDITIONAL_BRAND_NAME).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.ADDITIONAL_BRAND_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 17)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.PRODUCT_CODE).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PRODUCT_CODE).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.PRODUCT_CODE).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PRODUCT_CODE).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 18)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.PROD_INSP_MEMO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PROD_INSP_MEMO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.PROD_INSP_MEMO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PROD_INSP_MEMO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                //if (orderColumn == 19)
                //{
                //    if (orderDir == orderASC)
                //    {
                //        temp = q.OrderBy(m => m.RD_NUMBER).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                //        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.RD_NUMBER).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                //    }
                //    else if (orderDir == orderDESC)
                //    {
                //        temp = q.OrderByDescending(m => m.RD_NUMBER).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                //        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.RD_NUMBER).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                //    }
                //}
                if (orderColumn == 19)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.REFERENCE_NO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.REFERENCE_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.REFERENCE_NO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.REFERENCE_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 20)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.RDD).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.RDD).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.RDD).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.RDD).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 21)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.WF_NO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.WF_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.WF_NO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.WF_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 22)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.WF_NO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.WF_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.WF_NO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.WF_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                Results.recordsTotal = q.Select(m => m.WF_NO).Distinct().Count();
                Results.recordsFiltered = Results.recordsTotal;
            }
        }


        //private static void OrderByTrackingReportV3(TU_TRACKING_WF_REPORT_MODEL_REQUEST param, ref List<TU_TRACKING_WF_REPORT_MODEL> q, ref TU_TRACKING_WF_REPORT_MODEL_RESULT Results)
        //{
        //    if (!string.IsNullOrEmpty(param.data.GENERATE_EXCEL))
        //    {
        //        Results.data= q.OrderBy(m => m.WF_NO).ThenBy(m => m.PROCESS_CREATE_DATE).ToList();
        //    }
        //    else
        //    {
        //        var orderColumn = 1;
        //        var orderDir = "asc";
        //        Results.ORDER_COLUMN = 1;
        //        if (param.order != null && param.order.Count > 0)
        //        {
        //            orderColumn = param.order[0].column;
        //            orderDir = param.order[0].dir; //desc ,asc
        //            Results.ORDER_COLUMN = param.order[0].column;
        //        }

        //        string orderASC = "asc";
        //        string orderDESC = "desc";


        //        if (orderColumn == 0)
        //        {
        //            if (orderDir == orderASC)
        //            {

        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();

        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();

        //            }
        //        }
        //        if (orderColumn == 1)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => o.WF_NO).Skip(param.start).Take(param.length).ToList();

        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => o.WF_NO).Skip(param.start).Take(param.length).ToList();

        //            }
        //        }
        //        if (orderColumn == 2)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.PACKAGING_TYPE, o.WF_NO)).Skip(param.start).Take(param.length).ToList();

        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.PACKAGING_TYPE, o.WF_NO)).Skip(param.start).Take(param.length).ToList();

        //            }
        //        }
        //        if (orderColumn == 3)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.PRIMARY_TYPE_TXT, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.PRIMARY_TYPE_TXT).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PRIMARY_TYPE_TXT).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.PRIMARY_TYPE_TXT, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.PRIMARY_TYPE_TXT).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PRIMARY_TYPE_TXT).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 4)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.WF_STAUTS).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.WF_STAUTS).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.WF_STAUTS).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.WF_STAUTS).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 5)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.CURRENT_STEP_NAME_1).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.CURRENT_STEP_NAME_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.CURRENT_STEP_NAME_1).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.CURRENT_STEP_NAME_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 6)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.CURRENT_USER_NAME_1).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.CURRENT_USER_NAME_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.CURRENT_USER_NAME_1).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.CURRENT_USER_NAME_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 7)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.DURATION_1).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.DURATION_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.DURATION_1).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.DURATION_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 8)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.DUE_DATE_1).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.DUE_DATE_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.DUE_DATE_1).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.DUE_DATE_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 9)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.SOLD_TO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.SOLD_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.SOLD_TO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.SOLD_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 10)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.SHIP_TO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.SHIP_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.SHIP_TO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.SHIP_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 11)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.PORT).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PORT).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.PORT).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PORT).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 12)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.IN_TRANSIT_TO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.IN_TRANSIT_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.IN_TRANSIT_TO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.IN_TRANSIT_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 13)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.SALES_ORDER_NO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.SALES_ORDER_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.SALES_ORDER_NO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.SALES_ORDER_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 14)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.CREATE_ON).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.CREATE_ON).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.CREATE_ON).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.CREATE_ON).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 15)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.BRAND_NAME).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.BRAND_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.BRAND_NAME).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.BRAND_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 16)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.ADDITIONAL_BRAND_NAME).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.ADDITIONAL_BRAND_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.ADDITIONAL_BRAND_NAME).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.ADDITIONAL_BRAND_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 17)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.PRODUCT_CODE).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PRODUCT_CODE).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.PRODUCT_CODE).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PRODUCT_CODE).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 18)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.PROD_INSP_MEMO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PROD_INSP_MEMO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.PROD_INSP_MEMO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PROD_INSP_MEMO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }

        //        if (orderColumn == 19)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.REFERENCE_NO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.REFERENCE_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.REFERENCE_NO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.REFERENCE_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 20)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.RDD).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.RDD).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.RDD).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.RDD).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 21)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.WF_NO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.WF_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.WF_NO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.WF_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }
        //        if (orderColumn == 22)
        //        {
        //            if (orderDir == orderASC)
        //            {
        //                q = q.Distinct().OrderBy(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderBy(m => m.WF_NO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.WF_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //            else if (orderDir == orderDESC)
        //            {
        //                q = q.Distinct().OrderByDescending(o => (o.CREATOR_NAME, o.WF_NO)).Skip(param.start).Take(param.length).ToList();
        //                temp = q.OrderByDescending(m => m.WF_NO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
        //                Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.WF_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
        //            }
        //        }


        //        Results.recordsTotal = q.Count();
        //        Results.recordsFiltered = Results.recordsTotal;
        //    }
        //}

      

    }
}


