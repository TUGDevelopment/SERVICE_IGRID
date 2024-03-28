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

namespace BLL.Helpers
{
    public class SOReportHelper
    {
        public static V_ART_ASSIGNED_SO_RESULT GetAssignSOReport(V_ART_ASSIGNED_SO_REQUEST param)
        {
            V_ART_ASSIGNED_SO_RESULT Results = new V_ART_ASSIGNED_SO_RESULT();

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
                var listResultAll = QueryAssignSO(param, ref cnt);

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
                                if (item.IS_END == "X")
                                    if (item.IS_TERMINATE == "X")
                                        item.CURRENT_WF_STATUS = "Terminate";
                                    else
                                        item.CURRENT_WF_STATUS = "Completed";
                                else
                                    item.CURRENT_WF_STATUS = "In progress";

                                item.ARTWORK_SUB_ID = item.ARTWORK_SUB_ID;
                                item.REQUEST_ITEM_NO = item.REQUEST_ITEM_NO;

                                if (string.IsNullOrEmpty(item.READY_CREATE_PO))
                                    item.READY_CREATE_PO_DISPLAY_TXT = "";
                                else
                                    item.READY_CREATE_PO_DISPLAY_TXT = "Yes";

                                var temp = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { CURRENT_STEP_ID = SEND_PP, PARENT_ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).ToList();
                                if (temp.Count > 0)
                                {
                                    item.SEND_TO_PP_DISPLAY_TXT = "Yes";
                                }

                                var tempPO = ART_WF_ARTWORK_MAPPING_PO_SERVICE.GetByItem(new ART_WF_ARTWORK_MAPPING_PO() { ARTWORK_NO = item.REQUEST_ITEM_NO }, context).ToList();
                                if (tempPO.Count > 0)
                                {
                                    item.PO_CREATED_DISPLAY_TXT = "Yes";
                                }
                            }
                            else
                            {
                                item.REQUEST_ITEM_NO = "";
                                item.CURRENT_WF_STATUS = "No WF created";
                            }

                            item.SOLD_TO_DISPLAY_TXT = item.SOLD_TO + ":" + item.SOLD_TO_NAME;
                            item.SHIP_TO_DISPLAY_TXT = item.SHIP_TO + ":" + item.SHIP_TO_NAME;

                            if (string.IsNullOrEmpty(item.BOM_STOCK))
                                item.STOCK_DISPLAY_TXT = "";
                            else
                                item.STOCK_DISPLAY_TXT = item.BOM_STOCK;

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

        public static List<V_ART_ASSIGNED_SO_2> QueryAssignSO(V_ART_ASSIGNED_SO_REQUEST param, ref int cnt)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;

                    IQueryable<V_ART_ASSIGNED_SO> q = null;

                    q = ((from m in context.V_ART_ASSIGNED_SO
                          select m));

                    if (!string.IsNullOrEmpty(param.data.SALES_ORDER_NO))
                    {
                        var arrSO = param.data.SALES_ORDER_NO.Split(',');
                        q = q.Where(m => arrSO.Contains(m.SALES_ORDER_NO));
                    }

                    cnt = q.Distinct().Count();
                    var temp = q.Distinct().OrderBy(i => i.SALES_ORDER_NO).Distinct().ToList();

                    return MapperServices.V_ART_ASSIGNED_SO(temp);
                }
            }
        }


        public static V_SAP_SALES_ORDER_ALL_RESULT GetSAPSOReport(V_SAP_SALES_ORDER_ALL_REQUEST param)
        {
            V_SAP_SALES_ORDER_ALL_RESULT Results = new V_SAP_SALES_ORDER_ALL_RESULT();

            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<V_SAP_SALES_ORDER_ALL_2>();
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                var cnt = 0;
                var listResultAll = QuerySAPSO(param, ref cnt);

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

        public static List<V_SAP_SALES_ORDER_ALL_2> QuerySAPSO(V_SAP_SALES_ORDER_ALL_REQUEST param, ref int cnt)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;

                    IQueryable<V_SAP_SALES_ORDER_ALL> q = null;

                    q = ((from m in context.V_SAP_SALES_ORDER_ALL
                          select m));

                    if (!string.IsNullOrEmpty(param.data.SALES_ORDER_NO))
                    {
                        var arrSO = param.data.SALES_ORDER_NO.Split(',');
                        q = q.Where(m => arrSO.Contains(m.SALES_ORDER_NO));
                    }

                    if (param.data.ITEM != null)
                    {
                        q = q.Where(m => m.ITEM == param.data.ITEM);
                    }

                    cnt = q.Distinct().Count();
                    var temp = q.Distinct().OrderBy(i => i.SALES_ORDER_NO).Distinct().ToList();

                    return MapperServices.V_SAP_SALES_ORDER_ALL(temp);
                }
            }
        }


        public static IDOC_RESULT GetIDOCReport(SAP_M_PO_IDOC_REQUEST param)
        {
            IDOC_RESULT Results = new IDOC_RESULT();

            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<IDOC_MODEL>();
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                var cnt = 0;
                var listResultAll = QueryIDOC(param, ref cnt);

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

        public static List<IDOC_MODEL> QueryIDOC(SAP_M_PO_IDOC_REQUEST param, ref int cnt)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;

                    var q = ((from w in context.SAP_M_PO_IDOC
                              join m in context.SAP_M_PO_IDOC_ITEM on w.PO_IDOC_ID equals m.PO_IDOC_ID
                              select new IDOC_MODEL
                              {
                                  PO_IDOC_ID = m.PO_IDOC_ID,
                                  PURCHASE_ORDER_NO = w.PURCHASE_ORDER_NO,
                                  CURRENCY = w.CURRENCY,
                                  DATE = w.DATE,
                                  TIME = w.TIME,
                                  PURCHASING_ORG = w.PURCHASING_ORG,
                                  COMPANY_CODE = w.COMPANY_CODE,
                                  VENDOR_NO = w.VENDOR_NO,
                                  VENDOR_NAME = w.VENDOR_NAME,
                                  PURCHASER = w.PURCHASER,
                                  CREATE_DATE = w.CREATE_DATE,
                                  CREATE_BY = w.CREATE_BY,
                                  UPDATE_DATE = w.UPDATE_DATE,
                                  UPDATE_BY = w.UPDATE_BY,

                                  PO_IDOC_ITEM_ID = m.PO_IDOC_ITEM_ID,
                                  PO_ITEM_NO = m.PO_ITEM_NO,
                                  RECORD_TYPE = m.RECORD_TYPE,
                                  DELETION_INDICATOR = m.DELETION_INDICATOR,
                                  QUANTITY = m.QUANTITY,
                                  ORDER_UNIT = m.ORDER_UNIT,
                                  ORDER_PRICE_UNIT = m.ORDER_PRICE_UNIT,
                                  NET_ORDER_PRICE = m.NET_ORDER_PRICE,
                                  PRICE_UNIT = m.PRICE_UNIT,
                                  AMOUNT = m.AMOUNT,
                                  MATERIAL_GROUP = m.MATERIAL_GROUP,
                                  DENOMINATOR_QUANTITY_CONVERSION = m.DENOMINATOR_QUANTITY_CONVERSION,
                                  NUMERATOR_QUANTITY_CONVERSION = m.NUMERATOR_QUANTITY_CONVERSION,
                                  PLANT = m.PLANT,
                                  METERIAL_NUMBER = m.METERIAL_NUMBER,
                                  SHORT_TEXT = m.SHORT_TEXT,
                                  DELIVERY_DATE = m.DELIVERY_DATE,
                                  SALES_DOCUMENT_NO = m.SALES_DOCUMENT_NO,
                                  SALES_DOCUMENT_ITEM = m.SALES_DOCUMENT_ITEM
                              }
                          ));

                    if (!string.IsNullOrEmpty(param.data.PURCHASE_ORDER_NO))
                    {
                        var arrSO = param.data.PURCHASE_ORDER_NO.Split(',');
                        q = q.Where(m => arrSO.Contains(m.PURCHASE_ORDER_NO));
                    }

                    cnt = q.Distinct().Count();
                    var temp = q.Distinct().OrderBy(i => i.PURCHASE_ORDER_NO).Skip(param.start).Take(param.length).Distinct().ToList();

                    return temp;
                }
            }
        }


        public static XECM_M_PRODUCT_RESULT GetMAT3Report(XECM_M_PRODUCT_REQUEST param)
        {
            XECM_M_PRODUCT_RESULT Results = new XECM_M_PRODUCT_RESULT();

            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<XECM_M_PRODUCT_2>();
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                var cnt = 0;
                var listResultAll = QueryMAT3(param, ref cnt);

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

        public static List<XECM_M_PRODUCT_2> QueryMAT3(XECM_M_PRODUCT_REQUEST param, ref int cnt)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;

                    IQueryable<XECM_M_PRODUCT> q = null;

                    q = ((from m in context.XECM_M_PRODUCT
                          select m));

                    if (!string.IsNullOrEmpty(param.data.PRODUCT_CODE))
                    {
                        var arrSO = param.data.PRODUCT_CODE.Split(',');
                        q = q.Where(m => arrSO.Contains(m.PRODUCT_CODE));
                    }

                    cnt = q.Distinct().Count();
                    var temp = q.Distinct().OrderBy(i => i.PRODUCT_CODE).Skip(param.start).Take(param.length).Distinct().ToList();

                    return MapperServices.XECM_M_PRODUCT(temp);
                }
            }
        }


        public static XECM_M_PRODUCT5_RESULT GetMAT5Report(XECM_M_PRODUCT5_REQUEST param)
        {
            XECM_M_PRODUCT5_RESULT Results = new XECM_M_PRODUCT5_RESULT();

            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<XECM_M_PRODUCT5_2>();
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                var cnt = 0;
                var listResultAll = QueryMAT5(param, ref cnt);

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

        public static List<XECM_M_PRODUCT5_2> QueryMAT5(XECM_M_PRODUCT5_REQUEST param, ref int cnt)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;

                    IQueryable<XECM_M_PRODUCT5> q = null;

                    q = ((from m in context.XECM_M_PRODUCT5
                          select m));

                    if (!string.IsNullOrEmpty(param.data.PRODUCT_CODE))
                    {
                        var arrSO = param.data.PRODUCT_CODE.Split(',');
                        q = q.Where(m => arrSO.Contains(m.PRODUCT_CODE));
                    }

                    cnt = q.Distinct().Count();
                    var temp = q.Distinct().OrderBy(i => i.PRODUCT_CODE).Skip(param.start).Take(param.length).Distinct().ToList();

                    return MapperServices.XECM_M_PRODUCT5(temp);
                }
            }
        }


        public static ART_SYS_LOG_RESULT GetSYSLogReport(ART_SYS_LOG_REQUEST param)
        {
            ART_SYS_LOG_RESULT Results = new ART_SYS_LOG_RESULT();

            if (param.data.FIRST_LOAD || string.IsNullOrEmpty(param.data.ACTION))
            {
                Results.status = "S";
                Results.data = new List<ART_SYS_LOG_2>();
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                var cnt = 0;
                var listResultAll = QuerySYSLog(param, ref cnt);

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

        public static List<ART_SYS_LOG_2> QuerySYSLog(ART_SYS_LOG_REQUEST param, ref int cnt)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;

                    IQueryable<ART_SYS_LOG> q = null;

                    q = ((from m in context.ART_SYS_LOG
                          select m));

                    if (!string.IsNullOrEmpty(param.data.ACTION))
                    {
                        DateTime curDate = DateTime.Now.Date;
                        if (param.data.ACTION.ToUpper() == "EE")
                        {
                            q = q.Where(m => m.ACTION == "E" && DbFunctions.TruncateTime(m.CREATE_DATE) == DbFunctions.TruncateTime(curDate));
                        }
                        else
                        {
                            var arrSO = param.data.ACTION.Split(',');
                            q = q.Where(m => arrSO.Contains(m.ACTION));
                        }
                    }

                    cnt = q.Distinct().Count();
                    var temp = q.Distinct().OrderByDescending(i => i.CREATE_DATE).Skip(param.start).Take(param.length).Distinct().ToList();

                    return MapperServices.ART_SYS_LOG(temp);
                }
            }
        }


        public static SAP_M_ORDER_BOM_RESULT GetBomMaster(SAP_M_ORDER_BOM_REQUEST param)
        {
            SAP_M_ORDER_BOM_RESULT Results = new SAP_M_ORDER_BOM_RESULT();

            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<SAP_M_ORDER_BOM_2>();
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                var cnt = 0;
                var listResultAll = QueryBomMaster(param, ref cnt);

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

        public static List<SAP_M_ORDER_BOM_2> QueryBomMaster(SAP_M_ORDER_BOM_REQUEST param, ref int cnt)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;

                    IQueryable<SAP_M_ORDER_BOM> q = null;

                    q = ((from m in context.SAP_M_ORDER_BOM select m));

                    if (!string.IsNullOrEmpty(param.data.MATERIAL))
                    {
                        q = q.Where(m => m.MATERIAL == param.data.MATERIAL || m.MATERIAL_NUMBER == param.data.MATERIAL);
                    }

                    cnt = q.Distinct().Count();
                    var temp = q.Distinct().OrderByDescending(i => i.CREATE_DATE).Skip(param.start).Take(param.length).Distinct().ToList();

                    return MapperServices.SAP_M_ORDER_BOM(temp);
                }
            }
        }
    }
}