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

namespace BLL.Helpers
{
    public class OutstandingReportHelper
    {
        public static V_ART_ASSIGNED_SO_RESULT GetOutstandingReport(V_ART_ASSIGNED_SO_REQUEST param)
        {
            V_ART_ASSIGNED_SO_RESULT Results = new V_ART_ASSIGNED_SO_RESULT();
            Results.data = new List<V_ART_ASSIGNED_SO_2>();

            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<V_ART_ASSIGNED_SO_2>();
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                var cnt = 0;
                var listResultAll = QueryOutstandingReport(param, ref cnt);

                Results.recordsTotal = cnt;
                Results.recordsFiltered = cnt;

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var SEND_PP = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PP" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        foreach (var item in listResultAll)
                        {
                            if (item.ARTWORK_SUB_ID > 0)
                            {
                                item.ARTWORK_SUB_ID = item.ARTWORK_SUB_ID;
                                item.REQUEST_ITEM_NO = item.REQUEST_ITEM_NO;

                                //if (CNService.IsDevOrQAS())
                                //{
                                //    var mat5FormWF = (from m in context.ART_WF_ARTWORK_PROCESS_PA where m.ARTWORK_SUB_ID == item.ARTWORK_SUB_ID select m.MATERIAL_NO).FirstOrDefault();
                                //    var mat5FromSO = (from m in context.V_SAP_SALES_ORDER
                                //                      where m.SALES_ORDER_NO == item.SALES_ORDER_NO
                                //                      && m.ITEM == item.ITEM
                                //                      && m.COMPONENT_ITEM == item.COMPONENT_ITEM
                                //                      && !string.IsNullOrEmpty(m.COMPONENT_MATERIAL)
                                //                      select m.COMPONENT_MATERIAL).FirstOrDefault();

                                //    if (!string.IsNullOrEmpty(mat5FormWF) && !string.IsNullOrEmpty(mat5FromSO))
                                //    {
                                //        if (mat5FormWF.StartsWith("5") && mat5FromSO.StartsWith("5"))
                                //        {
                                //            if (mat5FormWF != mat5FromSO)
                                //            {
                                //                item.CHECK_WF = "1";
                                //            }
                                //        }
                                //    }
                                //}


                                //if (CNService.IsDevOrQAS())
                                //{
                                //    if (!String.IsNullOrEmpty(item.BOM_ITEM_CUSTOM_1) && item.BOM_ITEM_CUSTOM_1.Contains("NEW")) //
                                //    {
                                //        if (item.REQUEST_ITEM_NO.StartsWith("AW-R-"))
                                //        {
                                //            item.REQUEST_ITEM_NO = "";
                                //            item.CURRENT_WF_STATUS = "No WF created";
                                //        }
                                //    }
                                //}

                                if (string.IsNullOrEmpty(item.READY_CREATE_PO))
                                    item.READY_CREATE_PO_DISPLAY_TXT = "";
                                else
                                    item.READY_CREATE_PO_DISPLAY_TXT = "Yes";

                                var temp = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { CURRENT_STEP_ID = SEND_PP, PARENT_ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).ToList();
                                if (temp.Count > 0)
                                {
                                    var lastSentToPP = temp.OrderByDescending(m => m.CREATE_DATE).FirstOrDefault();
                                    if (lastSentToPP != null)
                                    {
                                        if (string.IsNullOrEmpty(lastSentToPP.REMARK_KILLPROCESS))
                                        {
                                            var pp = ART_WF_ARTWORK_PROCESS_PP_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PP() { ARTWORK_SUB_ID = lastSentToPP.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                            if (pp != null)
                                            {
                                                if (pp.ACTION_CODE != "SEND_BACK")
                                                {
                                                    item.SEND_TO_PP_DISPLAY_TXT = "Yes";
                                                }
                                            }
                                            else
                                            {
                                                item.SEND_TO_PP_DISPLAY_TXT = "Yes";
                                            }
                                        }
                                    }
                                }
                                //current status
                                item.CURRENT_WF_STATUS = item.IS_END == "X" ? (item.IS_TERMINATE == "X" ? "Terminate" : "Completed") : item.CURRENT_WF_STATUS;
                                //item.CURRENT_WF_STATUS = (from assign in context.V_ART_ASSIGNED_SO
                                //                     join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                //                     where assign.SALES_ORDER_NO == item.SALES_ORDER_NO
                                //                     && assign.ITEM == item.ITEM
                                //                     && assign.COMPONENT_ITEM == item.COMPONENT_ITEM
                                //                     && assign.COMPONENT_MATERIAL.ToString() == item.COMPONENT_MATERIAL.ToString()
                                //                     && assign.BOM_ITEM_CUSTOM_1 == item.BOM_ITEM_CUSTOM_1
                                //                     && (item.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (item.PRODUCT_CODE.StartsWith("5") && item.PRODUCT_CODE == assign.PRODUCT_CODE))
                                //                     //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                //                     select assign.IS_END).FirstOrDefault() == "X" ? ((from assign in context.V_ART_ASSIGNED_SO
                                //                                                                       join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                //                                                                       where assign.SALES_ORDER_NO == item.SALES_ORDER_NO
                                //                                                                       && assign.ITEM == item.ITEM
                                //                                                                       && assign.COMPONENT_ITEM == item.COMPONENT_ITEM
                                //                                                                       && assign.COMPONENT_MATERIAL.ToString() == item.COMPONENT_MATERIAL.ToString()
                                //                                                                       && assign.BOM_ITEM_CUSTOM_1 == item.BOM_ITEM_CUSTOM_1
                                //                                                                       && (item.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (item.PRODUCT_CODE.StartsWith("5") && item.PRODUCT_CODE == assign.PRODUCT_CODE))
                                //                                                                       //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                //                                                                       select assign.IS_TERMINATE).FirstOrDefault() == "X" ? "Terminate" : "Completed")

                                //                                                           : !string.IsNullOrEmpty((from assign in context.V_ART_ASSIGNED_SO
                                //                                                                                    join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                //                                                                                    where assign.SALES_ORDER_NO == item.SALES_ORDER_NO
                                //                                                                                    && assign.ITEM == item.ITEM
                                //                                                                                    && assign.COMPONENT_ITEM == item.COMPONENT_ITEM
                                //                                                                                    && assign.COMPONENT_MATERIAL.ToString() == item.COMPONENT_MATERIAL.ToString()
                                //                                                                                    && assign.BOM_ITEM_CUSTOM_1 == item.BOM_ITEM_CUSTOM_1
                                //                                                                                    && (item.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (item.PRODUCT_CODE.StartsWith("5") && item.PRODUCT_CODE == assign.PRODUCT_CODE))
                                //                                                                                    //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                //                                                                                    select assign.REQUEST_ITEM_NO).FirstOrDefault()) ? "In progress" : "No WF created";
                                //if (temp.Count > 0)
                                //{
                                //    item.SEND_TO_PP_DISPLAY_TXT = "Yes";
                                //}

                                var tempPO = ART_WF_ARTWORK_MAPPING_PO_SERVICE.GetByItem(new ART_WF_ARTWORK_MAPPING_PO() { ARTWORK_NO = item.REQUEST_ITEM_NO }, context).ToList();
                                if (tempPO.Count > 0)
                                {
                                    item.PO_CREATED_DISPLAY_TXT = "Yes";
                                }
                            }
                            else
                            {
                                item.REQUEST_ITEM_NO = "";
                            }

                            item.SOLD_TO_DISPLAY_TXT = item.SOLD_TO + ":" + item.SOLD_TO_NAME;
                            item.SHIP_TO_DISPLAY_TXT = item.SHIP_TO + ":" + item.SHIP_TO_NAME;

                            if (item.QUANTITY != null)
                                item.QUANTITY_DISPLAY_TXT = item.QUANTITY.ToString();
                            else if (item.ORDER_QTY != null)
                                item.QUANTITY_DISPLAY_TXT = item.ORDER_QTY.ToString();

                            if (string.IsNullOrEmpty(item.STOCK)) item.STOCK = "";
                            if (string.IsNullOrEmpty(item.BOM_STOCK)) item.BOM_STOCK = "";

                            if (!string.IsNullOrEmpty(item.BOM_STOCK))
                                item.STOCK_DISPLAY_TXT = item.BOM_STOCK;
                            else if (!string.IsNullOrEmpty(item.STOCK))
                                item.STOCK_DISPLAY_TXT = item.STOCK;

                            if (string.IsNullOrEmpty(item.PIC_DISPLAY_TXT))
                            {
                                var picID = CNService.CheckPICSO(context, item.SOLD_TO, item.SHIP_TO, item.ZONE);
                                item.PIC_DISPLAY_TXT = CNService.GetUserName(picID, context);
                            }
                        }
                    }
                }

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
        public static List<V_ART_ASSIGNED_SO_2> QueryOutstandingReport(V_ART_ASSIGNED_SO_REQUEST param, ref int cnt)
        {
            List<V_ART_ASSIGNED_SO_2> Outstanding = new List<V_ART_ASSIGNED_SO_2>();

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;
                    var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                    var SEND_PP = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PP" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                   
                    List<V_ART_ASSIGNED_SO_2> q = new List<V_ART_ASSIGNED_SO_2>();
                    var query = context.Database.SqlQuery<V_ART_ASSIGNED_SO_2>(@"spQueryOutstandingReport @filterSaleOrder,@filterItem,
	                    @filterProductCode,@filterMatherialDesc,@filterOrderQty,@filterOrderUnit,
	                    @filterRejectionCode,
	                    @filterProductionPlant,
	                    @filterComponentItem,
	                    @filterComponentMath,
	                    @filterDesc,
	                    @filterBomItemCustom,
	                    @filterQuantity,
	                    @filterStock,
	                    @filterCreateOn,
	                    @filterCreateOnTo,
	                    @filterRDD,
	                    @filterRDDTo,
	                    @filterSoldTo,
	                    @filterShipTo",
                        new SqlParameter("@filterSaleOrder", string.Format("{0}", param.data.SALES_ORDER_NO)),
                        new SqlParameter("@filterItem", string.Format("{0}", param.data.ITEM)),
                        new SqlParameter("@filterProductCode", string.Format("{0}", param.data.SEARCH_PRODUCT_CODE == null ? null : param.data.SEARCH_PRODUCT_CODE.Split(':')[0])),
                        new SqlParameter("@filterMatherialDesc", string.Format("{0}", param.data.MATERIAL_DESCRIPTION)),
                        new SqlParameter("@filterOrderQty", string.Format("{0}", param.data.ORDER_QTY)),
                        new SqlParameter("@filterStock", string.Format("{0}", param.data.STOCK)),
                        new SqlParameter("@filterOrderUnit", string.Format("{0}", param.data.ORDER_UNIT)),
                        new SqlParameter("@filterRejectionCode", string.Format("{0}", param.data.REJECTION_CODE)),
                        new SqlParameter("@filterProductionPlant", string.Format("{0}", param.data.PRODUCTION_PLANT)),
                        new SqlParameter("@filterComponentItem", string.Format("{0}", param.data.COMPONENT_ITEM)),
                        new SqlParameter("@filterComponentMath", string.Format("{0}", param.data.SEARCH_BOM_COMPONENT == null?null: param.data.SEARCH_BOM_COMPONENT.Split(':')[0])),
                        new SqlParameter("@filterDesc", string.Format("{0}", param.data.DECRIPTION)),
                        new SqlParameter("@filterBomItemCustom", string.Format("{0}", param.data.BOM_ITEM_CUSTOM_1)),
                        new SqlParameter("@filterQuantity", string.Format("{0}", param.data.QUANTITY)),
                        new SqlParameter("@filterCreateOn", string.Format("{0}",param.data.SEARCH_SO_CREATE_DATE_FROM)),
                        new SqlParameter("@filterCreateOnTo", string.Format("{0}",param.data.SEARCH_SO_CREATE_DATE_TO)),
                        new SqlParameter("@filterRDD", string.Format("{0}",param.data.SEARCH_RDD_DATE_FROM)),
                        new SqlParameter("@filterRDDTo", string.Format("{0}",param.data.SEARCH_RDD_DATE_TO)),
                        new SqlParameter("@filterSoldTo", string.Format("{0}", param.data.SEARCH_SOLD_TO_NAME == null ? null : param.data.SEARCH_SOLD_TO_NAME.Split(':')[0])),
                        new SqlParameter("@filterShipTo", string.Format("{0}", param.data.SEARCH_SHIP_TO_NAME == null ? null : param.data.SEARCH_SHIP_TO_NAME.Split(':')[0]))).ToList();
        
                    q = query.Select(m => new V_ART_ASSIGNED_SO_2()
                    {
                        ZONE = string.Format("{0}", m.ZONE),
                        COUNTRY = string.Format("{0}", m.COUNTRY),
                        SALES_ORDER_NO = m.SALES_ORDER_NO,
                        ITEM = m.ITEM,
                        PRODUCT_CODE = m.PRODUCT_CODE,
                        MATERIAL_DESCRIPTION = m.MATERIAL_DESCRIPTION,
                        ORDER_QTY = m.ORDER_QTY,
                        ORDER_UNIT = m.ORDER_UNIT,
                        REJECTION_CODE = m.REJECTION_CODE + " " + m.REJECTION_DESCRIPTION,
                        REJECTION_DESCRIPTION = m.REJECTION_DESCRIPTION,
                        PRODUCTION_PLANT = m.PRODUCTION_PLANT,
                        COMPONENT_ITEM = m.COMPONENT_ITEM,
                        COMPONENT_MATERIAL = m.COMPONENT_MATERIAL,
                        DECRIPTION = m.DECRIPTION,
                        QUANTITY = m.QUANTITY,
                        RDD = m.RDD,
                        CREATE_ON = m.CREATE_ON,
                        BOM_STOCK = m.BOM_STOCK,
                        STOCK = m.STOCK,
                        SOLD_TO = string.Format("{0}", m.SOLD_TO),
                        SHIP_TO = string.Format("{0}", m.SHIP_TO),
                        SOLD_TO_NAME = m.SOLD_TO_NAME,
                        SHIP_TO_NAME = m.SHIP_TO_NAME,
                        BRAND_ID = m.BRAND_ID,
                        BRAND_DESCRIPTION = m.BRAND_DESCRIPTION,
                        BOM_ITEM_CUSTOM_1 = m.BOM_ITEM_CUSTOM_1,
                        COMPANY_ID_2 = (int?)m.COMPANY_ID_2,
                        IS_TERMINATE = m.IS_TERMINATE,
                        IS_END = m.IS_END,
                        READY_CREATE_PO = m.READY_CREATE_PO,
                        ARTWORK_SUB_ID = m.ARTWORK_SUB_ID,
                        REQUEST_ITEM_NO = m.REQUEST_ITEM_NO,
                        PIC_DISPLAY_TXT = m.PIC_DISPLAY_TXT,
                        CURRENT_WF_STATUS = m.CURRENT_WF_STATUS,
                        SEND_TO_PP_DISPLAY_TXT = m.SEND_TO_PP_DISPLAY_TXT,
                        PO_CREATED_DISPLAY_TXT = m.PO_CREATED_DISPLAY_TXT
                    }).ToList();

                    if (!string.IsNullOrEmpty(param.data.SEARCH_PIC))
                    {
                        var defaultPIC = (from m in context.ART_M_PIC
                                          where m.USER_ID == param.data.SEARCH_PIC_ID && m.IS_ACTIVE == "X" 
                                          && string.IsNullOrEmpty(m.ZONE) && string.IsNullOrEmpty(m.SOLD_TO_CODE) && string.IsNullOrEmpty(m.SHIP_TO_CODE) && string.IsNullOrEmpty(m.COUNTRY)                            
                                          select new ART_M_PIC_2()
                                          {
                                              ZONE = m.ZONE + m.SOLD_TO_CODE + m.SHIP_TO_CODE + m.COUNTRY,
                                          }).ToList();
                        if (defaultPIC.Count == 1)
                        {
                            var listPIC = (from o in context.ART_M_PIC
                                           where o.USER_ID != param.data.SEARCH_PIC_ID && o.IS_ACTIVE == "X" 
                                           select new ART_M_PIC_2()
                                              {
                                               ZONE = o.ZONE ,
                                               SOLD_TO_CODE= o.SOLD_TO_CODE,
                                               SHIP_TO_CODE= o.SHIP_TO_CODE ,
                                               COUNTRY=o.COUNTRY,
                                               PIC_ID= o.PIC_ID,
                                           }).ToList();

                            q = (from m in q where !(from o in listPIC where !string.IsNullOrEmpty(o.ZONE) && !string.IsNullOrEmpty(o.SOLD_TO_CODE) && !string.IsNullOrEmpty(o.SHIP_TO_CODE) && !string.IsNullOrEmpty(o.COUNTRY)
                                 select new { x1 = o.ZONE.Substring(0, 2), x2 = o.SOLD_TO_CODE, x3 = o.SHIP_TO_CODE, x4 = o.COUNTRY })
                                         .Contains(new { x1 = m.ZONE.Substring(0, 2), x2 = m.SOLD_TO, x3 = m.SHIP_TO, x4 = m.COUNTRY })
                                         select m
                                         ).ToList();
 
                            var test = listPIC.Where(m=>m.PIC_ID== 284).ToList();
                            q = (from m in q
                                 where !(from o in listPIC
                                         where !string.IsNullOrEmpty(o.ZONE) && !string.IsNullOrEmpty(o.SOLD_TO_CODE) && !string.IsNullOrEmpty(o.SHIP_TO_CODE) && string.IsNullOrEmpty(o.COUNTRY)
                                           select new { x1 = o.ZONE.Substring(0, 2), x2 = o.SOLD_TO_CODE, x3 = o.SHIP_TO_CODE })
                                        .Contains(new { x1 = m.ZONE.Substring(0, 2), x2 = m.SOLD_TO, x3 = m.SHIP_TO })
                                 select m).ToList();

                            q = (from m in q
                                 where !(from o in listPIC where !string.IsNullOrEmpty(o.ZONE) && !string.IsNullOrEmpty(o.SOLD_TO_CODE) && string.IsNullOrEmpty(o.SHIP_TO_CODE) && string.IsNullOrEmpty(o.COUNTRY)
                                           select new { x1 = o.ZONE.Substring(0, 2), x2 = o.SOLD_TO_CODE })
                                        .Contains(new { x1 = m.ZONE.Substring(0, 2), x2 = m.SOLD_TO })
                                 select m).ToList();

                            q = (from m in q
                                 where !(from o in listPIC where !string.IsNullOrEmpty(o.ZONE) && string.IsNullOrEmpty(o.SOLD_TO_CODE) && string.IsNullOrEmpty(o.SHIP_TO_CODE) && string.IsNullOrEmpty(o.COUNTRY)
                                           select new { x1 = o.ZONE.Substring(0, 2) })
                                        .Contains(new { x1 = m.ZONE.Substring(0, 2) })
                                 select m).ToList();


                            //q = (from m in q
                            //     where !(from o in context.ART_M_PIC
                            //             where o.USER_ID != param.data.SEARCH_PIC_ID && o.IS_ACTIVE == "X"
                            //                 && !string.IsNullOrEmpty(o.ZONE) && !string.IsNullOrEmpty(o.SOLD_TO_CODE) && !string.IsNullOrEmpty(o.SHIP_TO_CODE) && !string.IsNullOrEmpty(o.COUNTRY)
                            //             select new { x1 = o.ZONE.Substring(0, 2), x2 = o.SOLD_TO_CODE, x3 = o.SHIP_TO_CODE, x4 = o.COUNTRY }
                            //             )
                            //             .Contains(new { x1 = m.ZONE.Substring(0, 2), x2 = m.SOLD_TO, x3 = m.SHIP_TO, x4 = m.COUNTRY })
                            //     select m).ToList();

                            //q = (from m in q
                            //     where !(from o in context.ART_M_PIC
                            //             where o.USER_ID != param.data.SEARCH_PIC_ID && o.IS_ACTIVE == "X"
                            //                 && !string.IsNullOrEmpty(o.ZONE) && !string.IsNullOrEmpty(o.SOLD_TO_CODE) && !string.IsNullOrEmpty(o.SHIP_TO_CODE) && string.IsNullOrEmpty(o.COUNTRY)
                            //             select new { x1 = o.ZONE.Substring(0, 2), x2 = o.SOLD_TO_CODE, x3 = o.SHIP_TO_CODE })
                            //             .Contains(new { x1 = m.ZONE.Substring(0, 2), x2 = m.SOLD_TO, x3 = m.SHIP_TO })
                            //     select m).ToList();

                            //q = (from m in q
                            //     where !(from o in context.ART_M_PIC
                            //             where o.USER_ID != param.data.SEARCH_PIC_ID && o.IS_ACTIVE == "X"
                            //                 && !string.IsNullOrEmpty(o.ZONE) && !string.IsNullOrEmpty(o.SOLD_TO_CODE) && string.IsNullOrEmpty(o.SHIP_TO_CODE) && string.IsNullOrEmpty(o.COUNTRY)
                            //             select new { x1 = o.ZONE.Substring(0, 2), x2 = o.SOLD_TO_CODE })
                            //             .Contains(new { x1 = m.ZONE.Substring(0, 2), x2 = m.SOLD_TO })
                            //     select m).ToList();

                            //q = (from m in q
                            //     where !(from o in context.ART_M_PIC
                            //             where o.USER_ID != param.data.SEARCH_PIC_ID && o.IS_ACTIVE == "X"
                            //                 && !string.IsNullOrEmpty(o.ZONE) && string.IsNullOrEmpty(o.SOLD_TO_CODE) && string.IsNullOrEmpty(o.SHIP_TO_CODE) && string.IsNullOrEmpty(o.COUNTRY)
                            //             select new { x1 = o.ZONE.Substring(0, 2) })
                            //             .Contains(new { x1 = m.ZONE.Substring(0, 2) })
                            //     select m).ToList();
                        }
                        else
                        {
                            var tempMasterPIC = (from m in context.ART_M_PIC
                                                 where m.USER_ID == param.data.SEARCH_PIC_ID && m.IS_ACTIVE == "X"
                                                 && !string.IsNullOrEmpty(m.ZONE) && !string.IsNullOrEmpty(m.SOLD_TO_CODE) && !string.IsNullOrEmpty(m.SHIP_TO_CODE) && !string.IsNullOrEmpty(m.COUNTRY)
                                                 select new ART_M_PIC_2()
                                                 {
                                                     ZONE = m.ZONE + m.SOLD_TO_CODE + m.SHIP_TO_CODE + m.COUNTRY,
                                                 }).ToList();
                            if (tempMasterPIC.Count == 0)
                            {
                                tempMasterPIC.Add(new ART_M_PIC_2() { ZONE = "XXXX" });
                            }
                            var masterPIC = tempMasterPIC.Select(m => m.ZONE).ToList();


                            var tempMasterPIC2 = (from m in context.ART_M_PIC
                                                  where m.USER_ID == param.data.SEARCH_PIC_ID && m.IS_ACTIVE == "X"
                                                  && !string.IsNullOrEmpty(m.ZONE) && !string.IsNullOrEmpty(m.SOLD_TO_CODE) && !string.IsNullOrEmpty(m.SHIP_TO_CODE) && string.IsNullOrEmpty(m.COUNTRY)
                                                  select new ART_M_PIC_2()
                                                  {
                                                      ZONE = m.ZONE + m.SOLD_TO_CODE + m.SHIP_TO_CODE,
                                                  }).ToList();
                            if (tempMasterPIC2.Count == 0)
                            {
                                tempMasterPIC2.Add(new ART_M_PIC_2() { ZONE = "XXXX" });
                            }
                            var masterPIC2 = tempMasterPIC2.Select(m => m.ZONE).ToList();

                            var tempMasterPIC3 = (from m in context.ART_M_PIC
                                                  where m.USER_ID == param.data.SEARCH_PIC_ID && m.IS_ACTIVE == "X"
                                                  && !string.IsNullOrEmpty(m.ZONE) && !string.IsNullOrEmpty(m.SOLD_TO_CODE) && string.IsNullOrEmpty(m.SHIP_TO_CODE) && string.IsNullOrEmpty(m.COUNTRY)
                                                  select new ART_M_PIC_2()
                                                  {
                                                      ZONE = m.ZONE + m.SOLD_TO_CODE,
                                                  }).ToList();
                            if (tempMasterPIC3.Count == 0)
                            {
                                tempMasterPIC3.Add(new ART_M_PIC_2() { ZONE = "XXXX" });
                            }
                            var masterPIC3 = tempMasterPIC3.Select(m => m.ZONE).ToList();

                            var tempMasterPIC4 = (from m in context.ART_M_PIC
                                                  where m.USER_ID == param.data.SEARCH_PIC_ID && m.IS_ACTIVE == "X"
                                                  && !string.IsNullOrEmpty(m.ZONE) && string.IsNullOrEmpty(m.SOLD_TO_CODE) && string.IsNullOrEmpty(m.SHIP_TO_CODE) && string.IsNullOrEmpty(m.COUNTRY)
                                                  select new ART_M_PIC_2()
                                                  {
                                                      ZONE = m.ZONE,
                                                  }).ToList();
                            if (tempMasterPIC4.Count == 0)
                            {
                                tempMasterPIC4.Add(new ART_M_PIC_2() { ZONE = "XXXX" });
                            }
                            var masterPIC4 = tempMasterPIC4.Select(m => m.ZONE).ToList();

                            //q = q.Where(m => masterPIC.Contains(m.ZONE.Length > 2 ? m.ZONE.Substring(0, 2) : m.ZONE + m.SOLD_TO + m.SHIP_TO + m.COUNTRY)
                            //             || masterPIC2.Contains(m.ZONE.Length > 2 ? m.ZONE.Substring(0, 2) : m.ZONE + m.SOLD_TO + m.SHIP_TO)
                            //             || masterPIC3.Contains(m.ZONE.Length > 2 ? m.ZONE.Substring(0, 2) : m.ZONE + m.SOLD_TO)
                            //             || masterPIC4.Contains(m.ZONE.Length > 2 ? m.ZONE.Substring(0, 2) : m.ZONE)).ToList();
                            q = q.Where(m => masterPIC.Contains(m.ZONE.Substring(0, 2) + m.SOLD_TO + m.SHIP_TO + m.COUNTRY)
                                           || masterPIC2.Contains(m.ZONE.Substring(0, 2) + m.SOLD_TO + m.SHIP_TO)
                                           || masterPIC3.Contains(m.ZONE.Substring(0, 2) + m.SOLD_TO)
                                           || masterPIC4.Contains(m.ZONE.Substring(0, 2))).ToList();
                        }
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_COMPANY))
                    {
                        var tempSerachCompany = Convert.ToInt32(param.data.SEARCH_COMPANY);
                        q = q.Where(m => m.COMPANY_ID_2 == tempSerachCompany).ToList();
                    }

                    if (!string.IsNullOrEmpty(param.data.SALES_ORDER_NO))
                    {
                        var arrSO = param.data.SALES_ORDER_NO.Split(',');
                        q = q.Where(m => arrSO.Contains(m.SALES_ORDER_NO)).ToList();
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_BRAND_NAME))
                    {
                        q = q.Where(m => m.BRAND_ID + ":" + m.BRAND_DESCRIPTION == param.data.SEARCH_BRAND_NAME).ToList();
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_COUNTRY_NAME))
                    {
                        var tempSearchCountry = param.data.SEARCH_COUNTRY_NAME.Split(':')[0];
                        q = q.Where(m => m.COUNTRY == tempSearchCountry).ToList();
                    }

                    //if (!string.IsNullOrEmpty(param.data.SEARCH_SOLD_TO_NAME))
                    //{
                    //    q = q.Where(m => m.SOLD_TO + ":" + m.SOLD_TO_NAME == param.data.SEARCH_SOLD_TO_NAME).ToList();
                    //}

                    //if (!string.IsNullOrEmpty(param.data.SEARCH_SHIP_TO_NAME))
                    //{
                    //    q = q.Where(m => m.SHIP_TO + ":" + m.SHIP_TO_NAME == param.data.SEARCH_SHIP_TO_NAME).ToList();
                    //}

                    //if (!string.IsNullOrEmpty(param.data.SEARCH_PRODUCT_CODE))
                    //{
                    //    q = q.AsQueryable().Where(m => m.PRODUCT_CODE + ":" + m.MATERIAL_DESCRIPTION == param.data.SEARCH_PRODUCT_CODE).ToList();
                    //}

                    //if (!string.IsNullOrEmpty(param.data.SEARCH_BOM_COMPONENT))
                    //{
                    //    q = q.AsQueryable().Where(m => m.COMPONENT_MATERIAL + ":" + m.DECRIPTION == param.data.SEARCH_BOM_COMPONENT).ToList();
                    //}

                    //DateTime SEARCH_RDD_DATE_FROM = new DateTime();
                    //DateTime SEARCH_RDD_DATE_TO = new DateTime();
                    //if (!string.IsNullOrEmpty(param.data.SEARCH_RDD_DATE_FROM)) SEARCH_RDD_DATE_FROM = CNService.ConvertStringToDate(param.data.SEARCH_RDD_DATE_FROM);
                    //if (!string.IsNullOrEmpty(param.data.SEARCH_RDD_DATE_TO)) SEARCH_RDD_DATE_TO = CNService.ConvertStringToDate(param.data.SEARCH_RDD_DATE_TO);
                    //if (!string.IsNullOrEmpty(param.data.SEARCH_RDD_DATE_FROM))
                    //    //q = (from r in q where DbFunctions.TruncateTime(r.RDD) >= SEARCH_RDD_DATE_FROM select r);
                    //    q = q.AsQueryable().Where(r => DbFunctions.TruncateTime(r.RDD) >= SEARCH_RDD_DATE_FROM).ToList();

                    //if (!string.IsNullOrEmpty(param.data.SEARCH_RDD_DATE_TO))
                    //    //q = (from r in q where DbFunctions.TruncateTime(r.RDD) <= SEARCH_RDD_DATE_TO select r);
                    //    q = q.AsQueryable().Where(r => DbFunctions.TruncateTime(r.RDD) <= SEARCH_RDD_DATE_TO).ToList();

                    //DateTime SEARCH_SO_CREATE_DATE_FROM = new DateTime();
                    //DateTime SEARCH_SO_CREATE_DATE_TO = new DateTime();
                    //if (!string.IsNullOrEmpty(param.data.SEARCH_SO_CREATE_DATE_FROM)) SEARCH_SO_CREATE_DATE_FROM = CNService.ConvertStringToDate(param.data.SEARCH_SO_CREATE_DATE_FROM);
                    //if (!string.IsNullOrEmpty(param.data.SEARCH_SO_CREATE_DATE_TO)) SEARCH_SO_CREATE_DATE_TO = CNService.ConvertStringToDate(param.data.SEARCH_SO_CREATE_DATE_TO);
                    //if (!string.IsNullOrEmpty(param.data.SEARCH_SO_CREATE_DATE_FROM))
                    //    //q = (from r in q where DbFunctions.TruncateTime(r.CREATE_ON) >= SEARCH_SO_CREATE_DATE_FROM select r);
                    //    q = q.AsQueryable().Where(r => DbFunctions.TruncateTime(r.CREATE_ON) >= SEARCH_SO_CREATE_DATE_FROM).ToList();

                    //if (!string.IsNullOrEmpty(param.data.SEARCH_SO_CREATE_DATE_TO))
                    //    //q = (from r in q where DbFunctions.TruncateTime(r.CREATE_ON) <= SEARCH_SO_CREATE_DATE_TO select r);
                    //    q = q.AsQueryable().Where(r => DbFunctions.TruncateTime(r.CREATE_ON) <= SEARCH_SO_CREATE_DATE_TO).ToList();

                    if (!string.IsNullOrEmpty(param.data.SEARCH_SO_ITEM_FROM))
                    {
                        decimal tempItemFrom = Convert.ToDecimal(param.data.SEARCH_SO_ITEM_FROM);
                        //q = (from r in q where r.ITEM >= tempItemFrom select r);
                        q = q.AsQueryable().Where(r => r.ITEM >= tempItemFrom).ToList();
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_SO_ITEM_TO))
                    {
                        decimal tempItemTo = Convert.ToDecimal(param.data.SEARCH_SO_ITEM_TO);
                        //q = (from r in q where r.ITEM <= tempItemTo select r);
                        q = q.AsQueryable().Where(r => r.ITEM <= tempItemTo).ToList();
                    }
                    //condition status
                    if (param.data.WORKFLOW_CREATED)
                    {
                        q = q.AsQueryable().Where(m => !string.IsNullOrEmpty(m.REQUEST_ITEM_NO)).ToList();
                    }

                    if (param.data.FLAG_SEND_TO_PP)
                        q = q.AsQueryable().Where(m => m.READY_CREATE_PO == "X").ToList();

                    if (param.data.SEND_TO_PP)
                    {
                        var listArtworkSubId = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { CURRENT_STEP_ID = SEND_PP }, context).Select(m => m.PARENT_ARTWORK_SUB_ID).ToList();
                        q = q.AsQueryable().Where(m => listArtworkSubId.Contains(m.ARTWORK_SUB_ID)).ToList();
                    }

                    if (param.data.PO_CREATED)
                    {
                        var tempPO = (from e in q
                                      join e2 in context.ART_WF_ARTWORK_MAPPING_PO on e.REQUEST_ITEM_NO equals e2.ARTWORK_NO
                                      select e.REQUEST_ITEM_NO).ToList();
                        if (tempPO.Count > 0)
                        {
                            q = q.AsQueryable().Where(m => tempPO.Contains(m.REQUEST_ITEM_NO)).ToList();
                        }
                    }

                    //cnt = q.Distinct().Count();
                    //return OrderByOutstanding(q, param);
                    return q.ToList();
                }
            }
        }
        public static List<V_ART_ASSIGNED_SO_2> QueryOutstandingReport2(V_ART_ASSIGNED_SO_REQUEST param)
        {
            List<V_ART_ASSIGNED_SO_2> Outstanding = new List<V_ART_ASSIGNED_SO_2>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;

                    IQueryable<V_ART_ASSIGNED_SO_2> q = null;
                    var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                    var SEND_PP = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PP" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                    

                    q = ((from m in context.V_SAP_SALES_ORDER_ALL
                          where !context.V_ART_ASSIGNED_SO
                          .Select(mm => new { mm.SALES_ORDER_NO, mm.ITEM, mm.COMPONENT_ITEM })
                          .Contains(new { m.SALES_ORDER_NO, m.ITEM, m.COMPONENT_ITEM })
                                  &&
                                  (
                                  (m.PRODUCT_CODE.StartsWith("3") && !m.PRODUCT_CODE.StartsWith("3W") && !string.IsNullOrEmpty(m.COMPONENT_MATERIAL) && m.COMPONENT_MATERIAL.StartsWith("5") && m.COMPONENT_MATERIAL.Length == 18 && (m.SO_ITEM_IS_ACTIVE == "X" || !string.IsNullOrEmpty(m.REJECTION_CODE)) && m.BOM_IS_ACTIVE == "X")
                                  || (m.PRODUCT_CODE.StartsWith("3") && !m.PRODUCT_CODE.StartsWith("3W") && (m.SO_ITEM_IS_ACTIVE == "X" || !string.IsNullOrEmpty(m.REJECTION_CODE)) && m.BOM_IS_ACTIVE == "X" && string.IsNullOrEmpty(m.COMPONENT_MATERIAL))
                                  || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE.Length == 18 && (m.SO_ITEM_IS_ACTIVE == "X" || !string.IsNullOrEmpty(m.REJECTION_CODE)) && string.IsNullOrEmpty(m.COMPONENT_MATERIAL))
                                  || (m.PRODUCT_CODE.StartsWith("3V") && (m.SO_ITEM_IS_ACTIVE == "X" || !string.IsNullOrEmpty(m.REJECTION_CODE)))
                                  )
                                  && string.IsNullOrEmpty(m.IS_MIGRATION)
                          select new V_ART_ASSIGNED_SO_2()
                          {
                              ZONE = m.ZONE,
                              COUNTRY = m.COUNTRY,
                              SALES_ORDER_NO = m.SALES_ORDER_NO,
                              ITEM = m.ITEM,
                              PRODUCT_CODE = m.PRODUCT_CODE,
                              MATERIAL_DESCRIPTION = m.MATERIAL_DESCRIPTION,
                              ORDER_QTY = m.ORDER_QTY,
                              ORDER_UNIT = m.ORDER_UNIT,
                              REJECTION_CODE = m.REJECTION_CODE + " " + m.REJECTION_DESCRIPTION,
                              REJECTION_DESCRIPTION = m.REJECTION_DESCRIPTION,
                              PRODUCTION_PLANT = m.PRODUCTION_PLANT,
                              COMPONENT_ITEM = m.COMPONENT_ITEM,
                              COMPONENT_MATERIAL = m.COMPONENT_MATERIAL,
                              DECRIPTION = m.DECRIPTION,
                              QUANTITY = m.QUANTITY,
                              RDD = m.RDD,
                              CREATE_ON = m.CREATE_ON,
                              BOM_STOCK = m.BOM_STOCK,
                              STOCK = m.STOCK,
                              SOLD_TO = m.SOLD_TO,
                              SHIP_TO = m.SHIP_TO,
                              SOLD_TO_NAME = m.SOLD_TO_NAME,
                              SHIP_TO_NAME = m.SHIP_TO_NAME,
                              BRAND_ID = m.BRAND_ID,
                              BRAND_DESCRIPTION = m.BRAND_DESCRIPTION,
                              BOM_ITEM_CUSTOM_1 = m.BOM_ITEM_CUSTOM_1,
                              COMPANY_ID_2 = null,
                              IS_TERMINATE = "",
                              IS_END = "",
                              READY_CREATE_PO = "",
                              ARTWORK_SUB_ID = 0,
                              REQUEST_ITEM_NO = "",
                              PIC_DISPLAY_TXT = "",
                              CURRENT_WF_STATUS = "No WF created",
                              SEND_TO_PP_DISPLAY_TXT = "",
                              PO_CREATED_DISPLAY_TXT = ""
                          })
                    .Union(from m3 in context.V_ART_ASSIGNED_SO
                           join m2 in context.ART_WF_ARTWORK_REQUEST on m3.ARTWORK_REQUEST_ID equals m2.ARTWORK_REQUEST_ID
                           join m in context.V_SAP_SALES_ORDER_ALL on new { m3.SALES_ORDER_NO, m3.ITEM, m3.COMPONENT_ITEM } equals new { m.SALES_ORDER_NO, m.ITEM, m.COMPONENT_ITEM }
                           where !m.BOM_ITEM_CUSTOM_1.Contains("MULTI")
                           select new V_ART_ASSIGNED_SO_2()
                           {
                               ZONE = m.ZONE,
                               COUNTRY = m.COUNTRY,
                               SALES_ORDER_NO = m.SALES_ORDER_NO,
                               ITEM = m.ITEM,
                               PRODUCT_CODE = m.PRODUCT_CODE,
                               MATERIAL_DESCRIPTION = m.MATERIAL_DESCRIPTION,
                               ORDER_QTY = m.ORDER_QTY,
                               ORDER_UNIT = m.ORDER_UNIT,
                               REJECTION_CODE = m.REJECTION_CODE + " " + m.REJECTION_DESCRIPTION,
                               REJECTION_DESCRIPTION = m.REJECTION_DESCRIPTION,
                               PRODUCTION_PLANT = m.PRODUCTION_PLANT,
                               COMPONENT_ITEM = m.COMPONENT_ITEM,
                               COMPONENT_MATERIAL = m.COMPONENT_MATERIAL,
                               DECRIPTION = m.DECRIPTION,
                               QUANTITY = m.QUANTITY,
                               RDD = m.RDD,
                               CREATE_ON = m.CREATE_ON,
                               BOM_STOCK = m.BOM_STOCK,
                               STOCK = m.STOCK,
                               SOLD_TO = m.SOLD_TO,
                               SHIP_TO = m.SHIP_TO,
                               SOLD_TO_NAME = m.SOLD_TO_NAME,
                               SHIP_TO_NAME = m.SHIP_TO_NAME,
                               BRAND_ID = m.BRAND_ID,
                               BRAND_DESCRIPTION = m.BRAND_DESCRIPTION,
                               BOM_ITEM_CUSTOM_1 = m.BOM_ITEM_CUSTOM_1,

                               COMPANY_ID_2 = (from assign in context.V_ART_ASSIGNED_SO
                                               join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                               join request in context.ART_WF_ARTWORK_REQUEST on assign.ARTWORK_REQUEST_ID equals request.ARTWORK_REQUEST_ID
                                               where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                               && assign.ITEM == m.ITEM
                                               && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                               && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                               && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                               && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                               //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                               select request.COMPANY_ID).FirstOrDefault(),

                               IS_TERMINATE = (from assign in context.V_ART_ASSIGNED_SO
                                               join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                               where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                               && assign.ITEM == m.ITEM
                                               && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                               && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                               && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                               && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                               //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                               select assign.IS_TERMINATE).FirstOrDefault(),

                               IS_END = (from assign in context.V_ART_ASSIGNED_SO
                                         join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                         where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                         && assign.ITEM == m.ITEM
                                         && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                         && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                         && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                         && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                         //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                         select assign.IS_END).FirstOrDefault(),

                               READY_CREATE_PO = (from assign in context.V_ART_ASSIGNED_SO
                                                  join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                  where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                  && assign.ITEM == m.ITEM
                                                  && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                  && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                  && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                  && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                  //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                  select assign.READY_CREATE_PO).FirstOrDefault(),

                               ARTWORK_SUB_ID = (from assign in context.V_ART_ASSIGNED_SO
                                                 join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                 where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                 && assign.ITEM == m.ITEM
                                                 && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                 && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                 && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                 && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                 //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                 select assign.ARTWORK_SUB_ID).FirstOrDefault(),

                               REQUEST_ITEM_NO = (from assign in context.V_ART_ASSIGNED_SO
                                                  join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                  where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                  && assign.ITEM == m.ITEM
                                                  && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                  && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                  && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                  && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                  //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                  select assign.REQUEST_ITEM_NO).FirstOrDefault(),

                               PIC_DISPLAY_TXT = "",

                               CURRENT_WF_STATUS = (from assign in context.V_ART_ASSIGNED_SO
                                                    join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                    where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                    && assign.ITEM == m.ITEM
                                                    && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                    && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                    && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                    && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                    //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                    select assign.IS_END).FirstOrDefault() == "X" ? ((from assign in context.V_ART_ASSIGNED_SO
                                                                                                      join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                                                                      where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                                                                      && assign.ITEM == m.ITEM
                                                                                                      && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                                                                      && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                                                                      && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                                                                      && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                                                                      //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                                                                      select assign.IS_TERMINATE).FirstOrDefault() == "X" ? "Terminate" : "Completed")

                                                                                          : !string.IsNullOrEmpty((from assign in context.V_ART_ASSIGNED_SO
                                                                                                                   join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                                                                                   where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                                                                                   && assign.ITEM == m.ITEM
                                                                                                                   && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                                                                                   && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                                                                                   && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                                                                                   && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                                                                                   //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                                                                                   select assign.REQUEST_ITEM_NO).FirstOrDefault()) ? "In progress" : "No WF created",

                               SEND_TO_PP_DISPLAY_TXT = "",
                               PO_CREATED_DISPLAY_TXT = ""
                           })
                     .Union(from m3 in context.V_ART_ASSIGNED_SO
                            join m2 in context.ART_WF_ARTWORK_REQUEST on m3.ARTWORK_REQUEST_ID equals m2.ARTWORK_REQUEST_ID
                            join m in context.V_SAP_SALES_ORDER_ALL on new { m3.SALES_ORDER_NO, m3.ITEM } equals new { m.SALES_ORDER_NO, m.ITEM }
                            where  m3.PRODUCT_CODE.StartsWith("5") && m3.PRODUCT_CODE.Length == 18 && string.IsNullOrEmpty(m3.COMPONENT_MATERIAL)
                            select new V_ART_ASSIGNED_SO_2()
                            {
                                ZONE = m.ZONE,
                                COUNTRY = m.COUNTRY,
                                SALES_ORDER_NO = m.SALES_ORDER_NO,
                                ITEM = m.ITEM,
                                PRODUCT_CODE = m.PRODUCT_CODE,
                                MATERIAL_DESCRIPTION = m.MATERIAL_DESCRIPTION,
                                ORDER_QTY = m.ORDER_QTY,
                                ORDER_UNIT = m.ORDER_UNIT,
                                REJECTION_CODE = m.REJECTION_CODE + " " + m.REJECTION_DESCRIPTION,
                                REJECTION_DESCRIPTION = m.REJECTION_DESCRIPTION,
                                PRODUCTION_PLANT = m.PRODUCTION_PLANT,
                                COMPONENT_ITEM = m.COMPONENT_ITEM,
                                COMPONENT_MATERIAL = m.COMPONENT_MATERIAL,
                                DECRIPTION = m.DECRIPTION,
                                QUANTITY = m.QUANTITY,
                                RDD = m.RDD,
                                CREATE_ON = m.CREATE_ON,
                                BOM_STOCK = m.BOM_STOCK,
                                STOCK = m.STOCK,
                                SOLD_TO = m.SOLD_TO,
                                SHIP_TO = m.SHIP_TO,
                                SOLD_TO_NAME = m.SOLD_TO_NAME,
                                SHIP_TO_NAME = m.SHIP_TO_NAME,
                                BRAND_ID = m.BRAND_ID,
                                BRAND_DESCRIPTION = m.BRAND_DESCRIPTION,
                                BOM_ITEM_CUSTOM_1 = m.BOM_ITEM_CUSTOM_1,

                                COMPANY_ID_2 = (from assign in context.V_ART_ASSIGNED_SO
                                                join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                join request in context.ART_WF_ARTWORK_REQUEST on assign.ARTWORK_REQUEST_ID equals request.ARTWORK_REQUEST_ID
                                                where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                && assign.ITEM == m.ITEM
                                                && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                select request.COMPANY_ID).FirstOrDefault(),

                                IS_TERMINATE = (from assign in context.V_ART_ASSIGNED_SO
                                                join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                && assign.ITEM == m.ITEM
                                                && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                select assign.IS_TERMINATE).FirstOrDefault(),

                                IS_END = (from assign in context.V_ART_ASSIGNED_SO
                                          join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                          where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                          && assign.ITEM == m.ITEM
                                          && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                          && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                          && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                          && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                          //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                          select assign.IS_END).FirstOrDefault(),

                                READY_CREATE_PO = (from assign in context.V_ART_ASSIGNED_SO
                                                   join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                   where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                   && assign.ITEM == m.ITEM
                                                   && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                   && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                   && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                   && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                   //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                   select assign.READY_CREATE_PO).FirstOrDefault(),

                                ARTWORK_SUB_ID = (from assign in context.V_ART_ASSIGNED_SO
                                                  join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                  where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                  && assign.ITEM == m.ITEM
                                                  && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                  && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                  && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                  && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                  //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                  select assign.ARTWORK_SUB_ID).FirstOrDefault(),

                                REQUEST_ITEM_NO = (from assign in context.V_ART_ASSIGNED_SO
                                                   join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                   where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                   && assign.ITEM == m.ITEM
                                                   && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                   && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                   && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                   && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                   //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                   select assign.REQUEST_ITEM_NO).FirstOrDefault(),

                                PIC_DISPLAY_TXT = "",

                                CURRENT_WF_STATUS = (from assign in context.V_ART_ASSIGNED_SO
                                                     join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                     where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                     && assign.ITEM == m.ITEM
                                                     && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                     && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                     && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                     && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                     //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                     select assign.IS_END).FirstOrDefault() == "X" ? ((from assign in context.V_ART_ASSIGNED_SO
                                                                                                       join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                                                                       where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                                                                       && assign.ITEM == m.ITEM
                                                                                                       && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                                                                       && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                                                                       && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                                                                       && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                                                                       //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                                                                       select assign.IS_TERMINATE).FirstOrDefault() == "X" ? "Terminate" : "Completed")

                                                                                           : !string.IsNullOrEmpty((from assign in context.V_ART_ASSIGNED_SO
                                                                                                                    join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                                                                                    where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                                                                                    && assign.ITEM == m.ITEM
                                                                                                                    && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                                                                                    && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                                                                                    && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                                                                                    && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                                                                                   //&& assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                                                                                   select assign.REQUEST_ITEM_NO).FirstOrDefault()) ? "In progress" : "No WF created",

                                SEND_TO_PP_DISPLAY_TXT = "",
                                PO_CREATED_DISPLAY_TXT = ""
                            })
                    .Union(from m3 in context.V_ART_ASSIGNED_SO
                           join m2 in context.ART_WF_ARTWORK_REQUEST on m3.ARTWORK_REQUEST_ID equals m2.ARTWORK_REQUEST_ID
                           join m in context.V_SAP_SALES_ORDER_ALL on new { m3.SALES_ORDER_NO, m3.ITEM, m3.COMPONENT_ITEM } equals new { m.SALES_ORDER_NO, m.ITEM, m.COMPONENT_ITEM }
                           where m.BOM_ITEM_CUSTOM_1.Contains("MULTI")
                           select new V_ART_ASSIGNED_SO_2()
                           {
                               ZONE = m.ZONE,
                               COUNTRY = m.COUNTRY,
                               SALES_ORDER_NO = m.SALES_ORDER_NO,
                               ITEM = m.ITEM,
                               PRODUCT_CODE = m.PRODUCT_CODE,
                               MATERIAL_DESCRIPTION = m.MATERIAL_DESCRIPTION,
                               ORDER_QTY = m.ORDER_QTY,
                               ORDER_UNIT = m.ORDER_UNIT,
                               REJECTION_CODE = m.REJECTION_CODE + " " + m.REJECTION_DESCRIPTION,
                               REJECTION_DESCRIPTION = m.REJECTION_DESCRIPTION,
                               PRODUCTION_PLANT = m.PRODUCTION_PLANT,
                               COMPONENT_ITEM = m.COMPONENT_ITEM,
                               COMPONENT_MATERIAL = m.COMPONENT_MATERIAL,
                               DECRIPTION = m.DECRIPTION,
                               QUANTITY = m.QUANTITY,
                               RDD = m.RDD,
                               CREATE_ON = m.CREATE_ON,
                               BOM_STOCK = m.BOM_STOCK,
                               STOCK = m.STOCK,
                               SOLD_TO = m.SOLD_TO,
                               SHIP_TO = m.SHIP_TO,
                               SOLD_TO_NAME = m.SOLD_TO_NAME,
                               SHIP_TO_NAME = m.SHIP_TO_NAME,
                               BRAND_ID = m.BRAND_ID,
                               BRAND_DESCRIPTION = m.BRAND_DESCRIPTION,
                               BOM_ITEM_CUSTOM_1 = m.BOM_ITEM_CUSTOM_1,

                               COMPANY_ID_2 = (from assign in context.V_ART_ASSIGNED_SO
                                               join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                               join request in context.ART_WF_ARTWORK_REQUEST on assign.ARTWORK_REQUEST_ID equals request.ARTWORK_REQUEST_ID
                                               where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                               && assign.ITEM == m.ITEM
                                               && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                               && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                               && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                               && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                               && assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                               select request.COMPANY_ID).FirstOrDefault(),

                               IS_TERMINATE = (from assign in context.V_ART_ASSIGNED_SO
                                               join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                               where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                               && assign.ITEM == m.ITEM
                                               && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                               && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                               && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                               && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                               && assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                               select assign.IS_TERMINATE).FirstOrDefault(),

                               IS_END = (from assign in context.V_ART_ASSIGNED_SO
                                         join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                         where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                         && assign.ITEM == m.ITEM
                                         && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                         && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                         && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                         && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                         && assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                         select assign.IS_END).FirstOrDefault(),

                               READY_CREATE_PO = (from assign in context.V_ART_ASSIGNED_SO
                                                  join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                  where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                  && assign.ITEM == m.ITEM
                                                  && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                  && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                  && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                  && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                  && assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                  select assign.READY_CREATE_PO).FirstOrDefault(),

                               ARTWORK_SUB_ID = (from assign in context.V_ART_ASSIGNED_SO
                                                 join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                 where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                 && assign.ITEM == m.ITEM
                                                 && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                 && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                 && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                 && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                 && assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                 select assign.ARTWORK_SUB_ID).FirstOrDefault(),

                               REQUEST_ITEM_NO = (from assign in context.V_ART_ASSIGNED_SO
                                                  join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                  where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                  && assign.ITEM == m.ITEM
                                                  && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                  && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                  && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                  && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                  && assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                  select assign.REQUEST_ITEM_NO).FirstOrDefault(),

                               PIC_DISPLAY_TXT = "",

                               CURRENT_WF_STATUS = (from assign in context.V_ART_ASSIGNED_SO
                                                    join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                    where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                    && assign.ITEM == m.ITEM
                                                    && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                    && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                    && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                    && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                    && assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                    select assign.IS_END).FirstOrDefault() == "X" ? ((from assign in context.V_ART_ASSIGNED_SO
                                                                                                      join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                                                                      where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                                                                      && assign.ITEM == m.ITEM
                                                                                                      && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                                                                      && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                                                                      && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                                                                      && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                                                                      && assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                                                                      select assign.IS_TERMINATE).FirstOrDefault() == "X" ? "Terminate" : "Completed")

                                                                                          : !string.IsNullOrEmpty((from assign in context.V_ART_ASSIGNED_SO
                                                                                                                   join pa in context.ART_WF_ARTWORK_PROCESS_PA on assign.ARTWORK_SUB_ID equals pa.ARTWORK_SUB_ID
                                                                                                                   where assign.SALES_ORDER_NO == m.SALES_ORDER_NO
                                                                                                                   && assign.ITEM == m.ITEM
                                                                                                                   && assign.COMPONENT_ITEM == m.COMPONENT_ITEM
                                                                                                                   && assign.COMPONENT_MATERIAL.ToString() == m.COMPONENT_MATERIAL.ToString()
                                                                                                                   && assign.BOM_ITEM_CUSTOM_1 == m.BOM_ITEM_CUSTOM_1
                                                                                                                   && (m.COMPONENT_MATERIAL.ToString() == assign.COMPONENT_MATERIAL.ToString() || string.IsNullOrEmpty(pa.MATERIAL_NO) || (m.PRODUCT_CODE.StartsWith("5") && m.PRODUCT_CODE == assign.PRODUCT_CODE))
                                                                                                                            && assign.ARTWORK_SUB_ID == m3.ARTWORK_SUB_ID
                                                                                                                   select assign.REQUEST_ITEM_NO).FirstOrDefault()) ? "In progress" : "No WF created",

                               SEND_TO_PP_DISPLAY_TXT = "",
                               PO_CREATED_DISPLAY_TXT = ""
                           }));

                    //if (CNService.IsDevOrQAS())
                    //{
                    //q = q.Where(m => !(from mm3 in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                    //                   join mm4 in context.ART_WF_ARTWORK_PROCESS_PA on mm3.ARTWORK_SUB_ID equals mm4.ARTWORK_SUB_ID
                    //                   join mm in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT on mm3.BOM_ID equals mm.PO_COMPLETE_SO_ITEM_COMPONENT_ID
                    //                   where (!string.IsNullOrEmpty(mm4.MATERIAL_NO)
                    //                      && !string.IsNullOrEmpty(mm.COMPONENT_MATERIAL)
                    //                      && mm4.MATERIAL_NO.StartsWith("5")
                    //                      && mm.COMPONENT_MATERIAL.StartsWith("5")
                    //                      && mm4.MATERIAL_NO != mm.COMPONENT_MATERIAL)
                    //                      && mm3.SALES_ORDER_NO == m.SALES_ORDER_NO
                    //                   select mm3.ARTWORK_SUB_ID
                    //       ).Contains(m.ARTWORK_SUB_ID));

                    //q = q.Where(m => !(from mm3 in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                    //                   join mm4 in context.ART_WF_ARTWORK_PROCESS_PA on mm3.ARTWORK_SUB_ID equals mm4.ARTWORK_SUB_ID
                    //                   join mm in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT on mm3.BOM_ID equals mm.PO_COMPLETE_SO_ITEM_COMPONENT_ID
                    //                   where (!string.IsNullOrEmpty(mm4.MATERIAL_NO)
                    //                      && !string.IsNullOrEmpty(mm.COMPONENT_MATERIAL)
                    //                      && mm4.MATERIAL_NO.StartsWith("5")
                    //                      && mm.COMPONENT_MATERIAL.StartsWith("5")
                    //                      && mm4.MATERIAL_NO != mm.COMPONENT_MATERIAL)
                    //                      && mm3.SALES_ORDER_NO == m.SALES_ORDER_NO
                    //                   select mm3.ARTWORK_SUB_ID
                    //        ).Contains(m.ARTWORK_SUB_ID));
                    //}

                    //Filter header
                    if (param.columns != null)
                    {
                        var filterSaleOrder = param.columns[1] != null ? param.columns[1].search.value : null;
                        var filterItem = param.columns[2] != null ? param.columns[2].search.value : null;
                        var filterProductCode = param.columns[3] != null ? param.columns[3].search.value : null;
                        var filterMatherialDesc = param.columns[4] != null ? param.columns[4].search.value : null;
                        var filterOrderQty = param.columns[5] != null ? param.columns[5].search.value : null;
                        var filterOrderUnit = param.columns[6] != null ? param.columns[6].search.value : null;
                        var filterRejectionCode = param.columns[7] != null ? param.columns[7].search.value : null;
                        var filterProductionPlant = param.columns[8] != null ? param.columns[8].search.value : null;
                        var filterComponentItem = param.columns[9] != null ? param.columns[9].search.value : null;
                        var filterComponentMath = param.columns[10] != null ? param.columns[10].search.value : null;
                        var filterDesc = param.columns[11] != null ? param.columns[11].search.value : null;
                        var filterBomItemCustom = param.columns[12] != null ? param.columns[12].search.value : null;
                        var filterQuantity = param.columns[13] != null ? param.columns[13].search.value : null;
                        var filterCurrentWFStatus = param.columns[14] != null ? param.columns[14].search.value : null;
                        var filterReqItemNo = param.columns[15] != null ? param.columns[15].search.value : null;
                        var filterReadyCreatePO = param.columns[16] != null ? param.columns[16].search.value : null;
                        var filterSendToPP = param.columns[17] != null ? param.columns[17].search.value : null;
                        var filterPOCreated = param.columns[18] != null ? param.columns[18].search.value : null;
                        var filterStock = param.columns[19] != null ? param.columns[19].search.value : null;
                        var filterCreateOn = param.columns[20] != null ? (param.columns[20].search.value != null ? ConvertFormatDate(param.columns[20].search.value) : null) : null;
                        var filterRDD = param.columns[21] != null ? (param.columns[21].search.value != null ? ConvertFormatDate(param.columns[21].search.value) : null) : null;
                        var filterSoldTo = param.columns[22] != null ? param.columns[22].search.value : null;
                        var filterShipTo = param.columns[23] != null ? param.columns[23].search.value : null;
                        var filterPicDisplay = param.columns[24] != null ? param.columns[24].search.value : null;

                        if (!string.IsNullOrEmpty(filterSaleOrder)) q = q.Where(m => m.SALES_ORDER_NO.ToLower().Contains(filterSaleOrder.ToLower()));
                        if (!string.IsNullOrEmpty(filterItem)) q = q.Where(m => m.ITEM.ToString().Contains(filterItem));
                        if (!string.IsNullOrEmpty(filterProductCode)) q = q.Where(m => m.PRODUCT_CODE.ToLower().Contains(filterProductCode.ToLower()));
                        if (!string.IsNullOrEmpty(filterMatherialDesc)) q = q.Where(m => m.MATERIAL_DESCRIPTION.ToLower().Contains(filterMatherialDesc.ToLower()));
                        if (!string.IsNullOrEmpty(filterOrderQty)) q = q.Where(m => m.ORDER_QTY.ToString().Contains(filterOrderQty));
                        if (!string.IsNullOrEmpty(filterOrderUnit)) q = q.Where(m => m.ORDER_UNIT.ToLower().Contains(filterOrderUnit.ToLower()));
                        if (!string.IsNullOrEmpty(filterRejectionCode)) q = q.Where(m => m.REJECTION_CODE.ToLower().Contains(filterRejectionCode.ToLower()) || m.REJECTION_DESCRIPTION.ToLower().Contains(filterRejectionCode.ToLower()));
                        if (!string.IsNullOrEmpty(filterProductionPlant)) q = q.Where(m => m.PRODUCTION_PLANT.ToLower().Contains(filterProductionPlant.ToLower()));
                        if (!string.IsNullOrEmpty(filterComponentItem)) q = q.Where(m => m.COMPONENT_ITEM.ToLower().Contains(filterComponentItem.ToLower()));
                        if (!string.IsNullOrEmpty(filterComponentMath)) q = q.Where(m => m.COMPONENT_MATERIAL.ToLower().Contains(filterComponentMath.ToLower()));
                        if (!string.IsNullOrEmpty(filterDesc)) q = q.Where(m => m.DECRIPTION.ToLower().Contains(filterDesc.ToLower()));
                        if (!string.IsNullOrEmpty(filterBomItemCustom)) q = q.Where(m => m.BOM_ITEM_CUSTOM_1.ToLower().Contains(filterBomItemCustom.ToLower()));
                        if (!string.IsNullOrEmpty(filterQuantity)) q = q.Where(m => m.QUANTITY.ToString().Contains(filterQuantity));
                        if (!string.IsNullOrEmpty(filterReqItemNo)) q = q.Where(m => m.REQUEST_ITEM_NO.ToLower().Contains(filterReqItemNo.ToLower()));
                        if (!string.IsNullOrEmpty(filterCurrentWFStatus)) q = q.Where(m => m.CURRENT_WF_STATUS.ToLower().Contains(filterCurrentWFStatus.ToLower()));
                        if (!string.IsNullOrEmpty(filterReadyCreatePO))
                            if (filterReadyCreatePO.ToLower() == "yes")
                            {
                                q = q.Where(m => m.READY_CREATE_PO == "X");
                            }
                        //if (!string.IsNullOrEmpty(filterSendToPP)) q = q.Where(m => m.SEND_TO_PP_DISPLAY_TXT == "Yes");
                        //if (!string.IsNullOrEmpty(filterPOCreated)) q = q.Where(m => m.PO_CREATED_DISPLAY_TXT == "Yes");
                        if (!string.IsNullOrEmpty(filterStock)) q = q.Where(m => m.BOM_STOCK.ToLower().Contains(filterStock.ToLower()));
                        if (!string.IsNullOrEmpty(filterCreateOn)) q = q.Where(m => m.CREATE_ON.ToString() == filterCreateOn.ToLower());
                        if (!string.IsNullOrEmpty(filterRDD)) q = q.Where(m => m.RDD.ToString() == filterRDD.ToLower());
                        if (!string.IsNullOrEmpty(filterSoldTo)) q = q.Where(m => m.SOLD_TO.ToLower().Contains(filterSoldTo.ToLower()) || m.SOLD_TO_NAME.ToLower().Contains(filterSoldTo.ToLower()));
                        if (!string.IsNullOrEmpty(filterShipTo)) q = q.Where(m => m.SHIP_TO.ToLower().Contains(filterShipTo.ToLower()) || m.SHIP_TO_NAME.ToLower().Contains(filterShipTo.ToLower()));
                        //if (!string.IsNullOrEmpty(filterPicDisplay)) q = q.Where(m => m.PIC_DISPLAY_TXT.ToLower().Contains(filterPicDisplay.ToLower()));
                    }

                    if (param.data.WORKFLOW_CREATED)
                    {
                        q = q.Where(m => !string.IsNullOrEmpty(m.REQUEST_ITEM_NO));
                    }

                    if (param.data.FLAG_SEND_TO_PP)
                        q = q.Where(m => m.READY_CREATE_PO == "X");

                    if (param.data.SEND_TO_PP)
                    {
                        var listArtworkSubId = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { CURRENT_STEP_ID = SEND_PP }, context).Select(m => m.PARENT_ARTWORK_SUB_ID).ToList();
                        q = q.Where(m => listArtworkSubId.Contains(m.ARTWORK_SUB_ID));
                    }

                    if (param.data.PO_CREATED)
                    {
                        var tempPO = (from e in q
                                      join e2 in context.ART_WF_ARTWORK_MAPPING_PO on e.REQUEST_ITEM_NO equals e2.ARTWORK_NO
                                      select e.REQUEST_ITEM_NO).ToList();
                        if (tempPO.Count > 0)
                        {
                            q = q.Where(m => tempPO.Contains(m.REQUEST_ITEM_NO));
                        }
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_PIC))
                    {
                        var defaultPIC = (from m in context.ART_M_PIC
                                          where m.USER_ID == param.data.SEARCH_PIC_ID && m.IS_ACTIVE == "X"
                                          && string.IsNullOrEmpty(m.ZONE) && string.IsNullOrEmpty(m.SOLD_TO_CODE) && string.IsNullOrEmpty(m.SHIP_TO_CODE) && string.IsNullOrEmpty(m.COUNTRY)
                                          select new ART_M_PIC_2()
                                          {
                                              ZONE = m.ZONE + m.SOLD_TO_CODE + m.SHIP_TO_CODE + m.COUNTRY,
                                          }).ToList();
                        if (defaultPIC.Count == 1)
                        {
                            q = (from m in q
                                 where !(from o in context.ART_M_PIC
                                         where o.USER_ID != param.data.SEARCH_PIC_ID && o.IS_ACTIVE == "X"
                                             && !string.IsNullOrEmpty(o.ZONE) && !string.IsNullOrEmpty(o.SOLD_TO_CODE) && !string.IsNullOrEmpty(o.SHIP_TO_CODE) && !string.IsNullOrEmpty(o.COUNTRY)
                                         select new { x1 = o.ZONE.Substring(0, 2), x2 = o.SOLD_TO_CODE, x3 = o.SHIP_TO_CODE, x4 = o.COUNTRY })
                                         .Contains(new { x1 = m.ZONE.Substring(0, 2), x2 = m.SOLD_TO, x3 = m.SHIP_TO, x4 = m.COUNTRY })
                                 select m);

                            q = (from m in q
                                 where !(from o in context.ART_M_PIC
                                         where o.USER_ID != param.data.SEARCH_PIC_ID && o.IS_ACTIVE == "X"
                                             && !string.IsNullOrEmpty(o.ZONE) && !string.IsNullOrEmpty(o.SOLD_TO_CODE) && !string.IsNullOrEmpty(o.SHIP_TO_CODE) && string.IsNullOrEmpty(o.COUNTRY)
                                         select new { x1 = o.ZONE.Substring(0, 2), x2 = o.SOLD_TO_CODE, x3 = o.SHIP_TO_CODE })
                                         .Contains(new { x1 = m.ZONE.Substring(0, 2), x2 = m.SOLD_TO, x3 = m.SHIP_TO })
                                 select m);

                            q = (from m in q
                                 where !(from o in context.ART_M_PIC
                                         where o.USER_ID != param.data.SEARCH_PIC_ID && o.IS_ACTIVE == "X"
                                             && !string.IsNullOrEmpty(o.ZONE) && !string.IsNullOrEmpty(o.SOLD_TO_CODE) && string.IsNullOrEmpty(o.SHIP_TO_CODE) && string.IsNullOrEmpty(o.COUNTRY)
                                         select new { x1 = o.ZONE.Substring(0, 2), x2 = o.SOLD_TO_CODE })
                                         .Contains(new { x1 = m.ZONE.Substring(0, 2), x2 = m.SOLD_TO })
                                 select m);

                            q = (from m in q
                                 where !(from o in context.ART_M_PIC
                                         where o.USER_ID != param.data.SEARCH_PIC_ID && o.IS_ACTIVE == "X"
                                             && !string.IsNullOrEmpty(o.ZONE) && string.IsNullOrEmpty(o.SOLD_TO_CODE) && string.IsNullOrEmpty(o.SHIP_TO_CODE) && string.IsNullOrEmpty(o.COUNTRY)
                                         select new { x1 = o.ZONE.Substring(0, 2) })
                                         .Contains(new { x1 = m.ZONE.Substring(0, 2) })
                                 select m);
                        }
                        else
                        {
                            var tempMasterPIC = (from m in context.ART_M_PIC
                                                 where m.USER_ID == param.data.SEARCH_PIC_ID && m.IS_ACTIVE == "X"
                                                 && !string.IsNullOrEmpty(m.ZONE) && !string.IsNullOrEmpty(m.SOLD_TO_CODE) && !string.IsNullOrEmpty(m.SHIP_TO_CODE) && !string.IsNullOrEmpty(m.COUNTRY)
                                                 select new ART_M_PIC_2()
                                                 {
                                                     ZONE = m.ZONE + m.SOLD_TO_CODE + m.SHIP_TO_CODE + m.COUNTRY,
                                                 }).ToList();
                            if (tempMasterPIC.Count == 0)
                            {
                                tempMasterPIC.Add(new ART_M_PIC_2() { ZONE = "XXXX" });
                            }
                            var masterPIC = tempMasterPIC.Select(m => m.ZONE).ToList();


                            var tempMasterPIC2 = (from m in context.ART_M_PIC
                                                  where m.USER_ID == param.data.SEARCH_PIC_ID && m.IS_ACTIVE == "X"
                                                  && !string.IsNullOrEmpty(m.ZONE) && !string.IsNullOrEmpty(m.SOLD_TO_CODE) && !string.IsNullOrEmpty(m.SHIP_TO_CODE) && string.IsNullOrEmpty(m.COUNTRY)
                                                  select new ART_M_PIC_2()
                                                  {
                                                      ZONE = m.ZONE + m.SOLD_TO_CODE + m.SHIP_TO_CODE,
                                                  }).ToList();
                            if (tempMasterPIC2.Count == 0)
                            {
                                tempMasterPIC2.Add(new ART_M_PIC_2() { ZONE = "XXXX" });
                            }
                            var masterPIC2 = tempMasterPIC2.Select(m => m.ZONE).ToList();

                            var tempMasterPIC3 = (from m in context.ART_M_PIC
                                                  where m.USER_ID == param.data.SEARCH_PIC_ID && m.IS_ACTIVE == "X"
                                                  && !string.IsNullOrEmpty(m.ZONE) && !string.IsNullOrEmpty(m.SOLD_TO_CODE) && string.IsNullOrEmpty(m.SHIP_TO_CODE) && string.IsNullOrEmpty(m.COUNTRY)
                                                  select new ART_M_PIC_2()
                                                  {
                                                      ZONE = m.ZONE + m.SOLD_TO_CODE,
                                                  }).ToList();
                            if (tempMasterPIC3.Count == 0)
                            {
                                tempMasterPIC3.Add(new ART_M_PIC_2() { ZONE = "XXXX" });
                            }
                            var masterPIC3 = tempMasterPIC3.Select(m => m.ZONE).ToList();

                            var tempMasterPIC4 = (from m in context.ART_M_PIC
                                                  where m.USER_ID == param.data.SEARCH_PIC_ID && m.IS_ACTIVE == "X"
                                                  && !string.IsNullOrEmpty(m.ZONE) && string.IsNullOrEmpty(m.SOLD_TO_CODE) && string.IsNullOrEmpty(m.SHIP_TO_CODE) && string.IsNullOrEmpty(m.COUNTRY)
                                                  select new ART_M_PIC_2()
                                                  {
                                                      ZONE = m.ZONE,
                                                  }).ToList();
                            if (tempMasterPIC4.Count == 0)
                            {
                                tempMasterPIC4.Add(new ART_M_PIC_2() { ZONE = "XXXX" });
                            }
                            var masterPIC4 = tempMasterPIC4.Select(m => m.ZONE).ToList();

                            q = q.Where(m => masterPIC.Contains(m.ZONE.Substring(0, 2) + m.SOLD_TO + m.SHIP_TO + m.COUNTRY)
                                        || masterPIC2.Contains(m.ZONE.Substring(0, 2) + m.SOLD_TO + m.SHIP_TO)
                                        || masterPIC3.Contains(m.ZONE.Substring(0, 2) + m.SOLD_TO)
                                        || masterPIC4.Contains(m.ZONE.Substring(0, 2)));
                        }
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_COMPANY))
                    {
                        var tempSerachCompany = Convert.ToInt32(param.data.SEARCH_COMPANY);
                        q = q.Where(m => m.COMPANY_ID_2 == tempSerachCompany);
                    }

                    if (!string.IsNullOrEmpty(param.data.SALES_ORDER_NO))
                    {
                        var arrSO = param.data.SALES_ORDER_NO.Split(',');
                        q = q.Where(m => arrSO.Contains(m.SALES_ORDER_NO));
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_BRAND_NAME))
                    {
                        q = q.Where(m => m.BRAND_ID + ":" + m.BRAND_DESCRIPTION == param.data.SEARCH_BRAND_NAME);
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_COUNTRY_NAME))
                    {
                        var tempSearchCountry = param.data.SEARCH_COUNTRY_NAME.Split(':')[0];
                        q = q.Where(m => m.COUNTRY == tempSearchCountry);
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_SOLD_TO_NAME))
                    {
                        q = q.Where(m => m.SOLD_TO + ":" + m.SOLD_TO_NAME == param.data.SEARCH_SOLD_TO_NAME);
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_SHIP_TO_NAME))
                    {
                        q = q.Where(m => m.SHIP_TO + ":" + m.SHIP_TO_NAME == param.data.SEARCH_SHIP_TO_NAME);
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_PRODUCT_CODE))
                    {
                        q = q.Where(m => m.PRODUCT_CODE + ":" + m.MATERIAL_DESCRIPTION == param.data.SEARCH_PRODUCT_CODE);
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_BOM_COMPONENT))
                    {
                        q = q.Where(m => m.COMPONENT_MATERIAL + ":" + m.DECRIPTION == param.data.SEARCH_BOM_COMPONENT);
                    }

                    DateTime SEARCH_RDD_DATE_FROM = new DateTime();
                    DateTime SEARCH_RDD_DATE_TO = new DateTime();
                    if (!string.IsNullOrEmpty(param.data.SEARCH_RDD_DATE_FROM)) SEARCH_RDD_DATE_FROM = CNService.ConvertStringToDate(param.data.SEARCH_RDD_DATE_FROM);
                    if (!string.IsNullOrEmpty(param.data.SEARCH_RDD_DATE_TO)) SEARCH_RDD_DATE_TO = CNService.ConvertStringToDate(param.data.SEARCH_RDD_DATE_TO);
                    if (!string.IsNullOrEmpty(param.data.SEARCH_RDD_DATE_FROM))
                        q = (from r in q where DbFunctions.TruncateTime(r.RDD) >= SEARCH_RDD_DATE_FROM select r);

                    if (!string.IsNullOrEmpty(param.data.SEARCH_RDD_DATE_TO))
                        q = (from r in q where DbFunctions.TruncateTime(r.RDD) <= SEARCH_RDD_DATE_TO select r);

                    DateTime SEARCH_SO_CREATE_DATE_FROM = new DateTime();
                    DateTime SEARCH_SO_CREATE_DATE_TO = new DateTime();
                    if (!string.IsNullOrEmpty(param.data.SEARCH_SO_CREATE_DATE_FROM)) SEARCH_SO_CREATE_DATE_FROM = CNService.ConvertStringToDate(param.data.SEARCH_SO_CREATE_DATE_FROM);
                    if (!string.IsNullOrEmpty(param.data.SEARCH_SO_CREATE_DATE_TO)) SEARCH_SO_CREATE_DATE_TO = CNService.ConvertStringToDate(param.data.SEARCH_SO_CREATE_DATE_TO);
                    if (!string.IsNullOrEmpty(param.data.SEARCH_SO_CREATE_DATE_FROM))
                        q = (from r in q where DbFunctions.TruncateTime(r.CREATE_ON) >= SEARCH_SO_CREATE_DATE_FROM select r);

                    if (!string.IsNullOrEmpty(param.data.SEARCH_SO_CREATE_DATE_TO))
                        q = (from r in q where DbFunctions.TruncateTime(r.CREATE_ON) <= SEARCH_SO_CREATE_DATE_TO select r);

                    if (!string.IsNullOrEmpty(param.data.SEARCH_SO_ITEM_FROM))
                    {
                        decimal tempItemFrom = Convert.ToDecimal(param.data.SEARCH_SO_ITEM_FROM);
                        q = (from r in q where r.ITEM >= tempItemFrom select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_SO_ITEM_TO))
                    {
                        decimal tempItemTo = Convert.ToDecimal(param.data.SEARCH_SO_ITEM_TO);
                        q = (from r in q where r.ITEM <= tempItemTo select r);
                    }
                    //cnt = q.Distinct().Count();
                    return q.ToList();
                    //return OrderByOutstanding(q, param);
                }
            }
        }

        public static List<V_ART_ASSIGNED_SO_2> OrderByOutstanding(//IQueryable<V_ART_ASSIGNED_SO_2> q, 
            List<V_ART_ASSIGNED_SO_2> q,
            V_ART_ASSIGNED_SO_REQUEST param)
        {
            var Outstanding = new List<V_ART_ASSIGNED_SO_2>();
            var orderColumn = 1;
            var orderDir = "asc";
            if (param.order != null && param.order.Count > 0)
            {
                orderColumn = param.order[0].column;
                orderDir = param.order[0].dir; //desc ,asc
            }

            string orderASC = "asc";
            string orderDESC = "desc";

            //if (param.start > cnt)//Fixed display data not found when data < current page
            //{
            //    param.start = 0;
            //}

            if (orderColumn == 1)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.SALES_ORDER_NO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.SALES_ORDER_NO).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.SALES_ORDER_NO).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.SALES_ORDER_NO).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.SALES_ORDER_NO).ToList();
                }
            }
            if (orderColumn == 2)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.ITEM).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.ITEM).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.ITEM).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.ITEM).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.ITEM).ToList();
                }
            }
            if (orderColumn == 3)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.PRODUCT_CODE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.PRODUCT_CODE).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.PRODUCT_CODE).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.PRODUCT_CODE).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.PRODUCT_CODE).ToList();
                }
            }
            if (orderColumn == 4)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.MATERIAL_DESCRIPTION).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.MATERIAL_DESCRIPTION).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.MATERIAL_DESCRIPTION).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.MATERIAL_DESCRIPTION).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.MATERIAL_DESCRIPTION).ToList();
                }
            }
            if (orderColumn == 5)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.ORDER_QTY).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.ORDER_QTY).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.ORDER_QTY).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.ORDER_QTY).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.ORDER_QTY).ToList();
                }
            }
            if (orderColumn == 6)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.ORDER_UNIT).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.ORDER_UNIT).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.ORDER_UNIT).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.ORDER_UNIT).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.ORDER_UNIT).ToList();
                }
            }
            if (orderColumn == 7)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.REJECTION_CODE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.REJECTION_CODE).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.REJECTION_CODE).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.REJECTION_CODE).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.REJECTION_CODE).ToList();
                }
            }
            if (orderColumn == 8)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.PRODUCTION_PLANT).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.PRODUCTION_PLANT).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.PRODUCTION_PLANT).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.PRODUCTION_PLANT).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.PRODUCTION_PLANT).ToList();
                }
            }
            if (orderColumn == 9)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.COMPONENT_ITEM).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.COMPONENT_ITEM).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.COMPONENT_ITEM).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.COMPONENT_ITEM).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.COMPONENT_ITEM).ToList();
                }
            }
            if (orderColumn == 10)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.COMPONENT_MATERIAL).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.COMPONENT_MATERIAL).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.COMPONENT_MATERIAL).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.COMPONENT_MATERIAL).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.COMPONENT_MATERIAL).ToList();
                }
            }
            if (orderColumn == 11)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.DECRIPTION).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.DECRIPTION).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.DECRIPTION).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.DECRIPTION).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.DECRIPTION).ToList();
                }
            }
            if (orderColumn == 12)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.BOM_ITEM_CUSTOM_1).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.BOM_ITEM_CUSTOM_1).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.BOM_ITEM_CUSTOM_1).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.BOM_ITEM_CUSTOM_1).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.BOM_ITEM_CUSTOM_1).ToList();
                }
            }
            if (orderColumn == 13)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.QUANTITY).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.QUANTITY).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.QUANTITY).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.QUANTITY).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.QUANTITY).ToList();
                }
            }
            if (orderColumn == 14)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.CURRENT_WF_STATUS).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.CURRENT_WF_STATUS).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.CURRENT_WF_STATUS).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.CURRENT_WF_STATUS).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.CURRENT_WF_STATUS).ToList();
                }
            }
            if (orderColumn == 15)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.REQUEST_ITEM_NO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.REQUEST_ITEM_NO).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.REQUEST_ITEM_NO).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.REQUEST_ITEM_NO).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.REQUEST_ITEM_NO).ToList();
                }
            }
            if (orderColumn == 16)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.READY_CREATE_PO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.READY_CREATE_PO).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.READY_CREATE_PO).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.READY_CREATE_PO).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.READY_CREATE_PO).ToList();
                }
            }
            if (orderColumn == 17)
            {
                if (orderDir == orderASC)
                    Outstanding = q.OrderBy(i => i.SEND_TO_PP_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().ToList();
                else if (orderDir == orderDESC)
                    Outstanding = q.OrderByDescending(i => i.SEND_TO_PP_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().ToList();
            }
            if (orderColumn == 18)
            {
                if (orderDir == orderASC)
                    Outstanding = q.OrderBy(i => i.PO_CREATED_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().ToList();
                else if (orderDir == orderDESC)
                    Outstanding = q.OrderByDescending(i => i.PO_CREATED_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().ToList();
            }
            if (orderColumn == 19)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.BOM_STOCK).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.BOM_STOCK).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.BOM_STOCK).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.BOM_STOCK).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.BOM_STOCK).ToList();
                }
            }
            if (orderColumn == 20)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.CREATE_ON).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.CREATE_ON).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.CREATE_ON).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.CREATE_ON).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.CREATE_ON).ToList();
                }
            }
            if (orderColumn == 21)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.RDD).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.RDD).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.RDD).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.RDD).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.RDD).ToList();
                }
            }

            if (orderColumn == 22)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.SOLD_TO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.SOLD_TO).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.SOLD_TO).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.SOLD_TO).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.SOLD_TO).ToList();
                }
            }
            if (orderColumn == 23)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.SHIP_TO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.SHIP_TO).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.SHIP_TO).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.SHIP_TO).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.SHIP_TO).ToList();
                }
            }
            if (orderColumn == 24)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Outstanding = q.Distinct().OrderBy(i => i.PIC_DISPLAY_TXT).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Outstanding = q.OrderBy(i => i.PIC_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.PIC_DISPLAY_TXT).ToList();
                    else if (orderDir == orderDESC)
                        Outstanding = q.OrderByDescending(i => i.PIC_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.PIC_DISPLAY_TXT).ToList();
                }
            }
            return Outstanding;
        }

        private static List<V_ART_ASSIGNED_SO_2> FilterDataPGView(List<V_ART_ASSIGNED_SO_2> data, V_ART_ASSIGNED_SO_REQUEST param, ref int cnt)
        {
            var filterValue = param.columns[1].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SALES_ORDER_NO) && m.SALES_ORDER_NO.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[2].search.value;
            if (!string.IsNullOrEmpty(filterValue))
            {
                var temp = Convert.ToDecimal(filterValue);
                data = data.Where(m => m.ITEM == temp).ToList();
            }

            filterValue = param.columns[3].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PRODUCT_CODE) && m.PRODUCT_CODE.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[4].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.MATERIAL_DESCRIPTION) && m.MATERIAL_DESCRIPTION.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[5].search.value;
            if (!string.IsNullOrEmpty(filterValue))
            {
                var temp = Convert.ToDecimal(filterValue);
                data = data.Where(m => m.ORDER_QTY == temp).ToList();
            }

            filterValue = param.columns[6].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.ORDER_UNIT) && m.ORDER_UNIT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[7].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.REJECTION_CODE) && m.REJECTION_CODE.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[8].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PRODUCTION_PLANT) && m.PRODUCTION_PLANT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[9].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.COMPONENT_ITEM) && m.COMPONENT_ITEM.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[10].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.COMPONENT_MATERIAL) && m.COMPONENT_MATERIAL.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[11].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.DECRIPTION) && m.DECRIPTION.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[12].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.BOM_ITEM_CUSTOM_1) && m.BOM_ITEM_CUSTOM_1.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[13].search.value;
            if (!string.IsNullOrEmpty(filterValue))
            {
                var temp = Convert.ToDecimal(filterValue);
                data = data.Where(m => m.QUANTITY == temp).ToList();
            }
            filterValue = param.columns[14].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.CURRENT_WF_STATUS) && m.CURRENT_WF_STATUS.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[15].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => m.REQUEST_ITEM_NO.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[16].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.READY_CREATE_PO_DISPLAY_TXT) && m.READY_CREATE_PO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[17].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SEND_TO_PP_DISPLAY_TXT) && m.SEND_TO_PP_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[18].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PO_CREATED_DISPLAY_TXT) && m.PO_CREATED_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[19].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.BOM_STOCK) && m.BOM_STOCK.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[20].search.value;
            if (!string.IsNullOrEmpty(filterValue))
            {
                var temp = CNService.ConvertStringToDate2(filterValue);
                if (temp != null)
                    data = data.Where(m => DbFunctions.TruncateTime(m.CREATE_ON) == temp).ToList();
            }

            filterValue = param.columns[21].search.value;
            if (!string.IsNullOrEmpty(filterValue))
            {
                var temp = CNService.ConvertStringToDate2(filterValue);
                if (temp != null)
                    data = data.Where(m => DbFunctions.TruncateTime(m.RDD) == temp).ToList();
            }

            filterValue = param.columns[22].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SOLD_TO_DISPLAY_TXT) && m.SOLD_TO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[23].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SHIP_TO_DISPLAY_TXT) && m.SHIP_TO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[24].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PIC_DISPLAY_TXT) && m.PIC_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            cnt = data.Count();

            return data;
        }

        private static string ConvertFormatDate(string filterDate)
        {
            string result = string.Empty;

            string[] arrFilterDate = filterDate.Split('/');
            if (arrFilterDate.Length > 0)
            {
                for (int i = 0; i < arrFilterDate.Length; i++)
                {
                    if (arrFilterDate[i].Length > 0)
                    {
                        result = arrFilterDate[i] + "-" + result;
                    }
                }
                result = result.Length > 0 ? result.Substring(0, result.Length - 1) : result;
            }
            return result;
        }
    }
}


