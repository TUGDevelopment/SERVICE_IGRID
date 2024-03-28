using BLL.DocumentManagement;
using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BLL.Helpers
{
    public static class PPFormHelper
    {
        //public static PP_RESULT GetExportToExcel(PP_REQUEST_LIST param)
        //{
        //    PP_RESULT Results = new PP_RESULT();
        //    List<PP_MODEL> listPP = new List<PP_MODEL>();
        //    PP_MODEL2 PPview = new PP_MODEL2();
        //    List<PP_MODEL> listppmodel = new List<PP_MODEL>();
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add(new DataColumn("Sold to"));
        //    dt.Columns.Add(new DataColumn("Ship to"));
        //    dt.Columns.Add(new DataColumn("Sales order"));
        //    dt.Columns.Add(new DataColumn("Item"));
        //    dt.Columns.Add(new DataColumn("Production Plant"));
        //    dt.Columns.Add(new DataColumn("Sales org"));
        //    dt.Columns.Add(new DataColumn("RDD"));
        //    dt.Columns.Add(new DataColumn("Brand"));
        //    dt.Columns.Add(new DataColumn("PKG Type"));
        //    dt.Columns.Add(new DataColumn("Product code"));
        //    dt.Columns.Add(new DataColumn("PKG code"));
        //    dt.Columns.Add(new DataColumn("Vendor"));
        //    dt.Columns.Add(new DataColumn("Workflow no."));
        //    dt.Columns.Add(new DataColumn("Receiving date-time"));
        //    dt.Columns.Add(new DataColumn("Comment by PA"));

        //    try
        //    {
        //        using (var context = new ARTWORKEntities())
        //        {
        //            foreach (PP_MODEL pp in param.data)
        //            {
        //                string[] arrSO = pp.SALES_ORDER.Replace("<br>", "|").Split('|');
        //                string[] arrSOItems = pp.SALES_ORDER_ITEM.Replace("<br>", "|").Split('|');
        //                //foreach (string s in arrSO)
        //                for (int i = 0; i < arrSO.Length; i++)
        //                {

        //                    DataRow dr = dt.NewRow();
        //                    dr["RDD"] = pp.RDD;
        //                    dr["Sold to"] = pp.SOLD_TO_DISPLAY_TXT;
        //                    dr["Ship to"] = pp.SHIP_TO_DISPLAY_TXT;
        //                    dr["Sales org"] = pp.SALES_ORG;
        //                    dr["Brand"] = pp.BRAND_DISPLAY_TXT;
        //                    dr["PKG Type"] = pp.PKG_TYPE_DISPLAY_TXT;
        //                    dr["Product code"] = pp.PRODUCT_CODE;
        //                    dr["PKG code"] = pp.PKG_CODE;
        //                    dr["Vendor"] = pp.VENDOR_DISPLAY_TXT;
        //                    dr["Workflow no."] = pp.WORKFLOW_NO;
        //                    dr["Receiving date-time"] = pp.RECEIVE_DATE;
        //                    dr["Comment by PA"] = pp.REMARK_BY_PA;
        //                    PPview = new PP_MODEL2();
        //                    PPview = MapperServices.PPVIEW_TEMPLATE(pp);
        //                    PPview.ARTWORK_SUB_ID = pp.ARTWORK_SUB_ID;
        //                    if (arrSO[i].Trim().Length > 0)
        //                    {
        //                        PPview.SALES_ORDER = arrSO[i].Trim();
        //                        PPview.SALES_ORDER_ITEM = arrSOItems[i].Trim();
        //                        PPview.PLANT = CNService.GetSaleOrderItems(PPview, context);
        //                        dr["Sales order"] = PPview.SALES_ORDER;
        //                        dr["Item"] = PPview.SALES_ORDER_ITEM;
        //                        dr["Production Plant"] = PPview.PLANT;
        //                    }
        //                    dt.Rows.Add(dr);
        //                }
        //            }
        //        }
        //        //dt = CNService.ConvertToDataTable(listppmodel);
        //        if (dt.Rows.Count > 0)
        //        {
        //            SendExcelFile(dt, "ppview");
        //            Results.status = "S";
        //            Results.data = listPP;
        //        }
        //            //ConvertToExcel(dt, "ppview", "ppview");
        //    }
        //    catch (Exception ex)
        //    {
        //        Results.status = "E";
        //        Results.msg = CNService.GetErrorMessage(ex);
        //    }
        //    return Results;
        //}
        //public static void SendExcelFile(DataTable dt,string fileName)
        //{

        //    System.Web.UI.WebControls.DataGrid dg = new System.Web.UI.WebControls.DataGrid();
        //    dg.DataSource = dt;
        //    dg.DataBind();
        //    var context = System.Web.HttpContext.Current;
        //    context.Response.Clear();
        //    context.Response.Buffer = true;
        //    context.Response.ClearContent();
        //    context.Response.ClearHeaders();
        //    context.Response.Charset = "";
        //    string FileName = fileName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xls";
        //    StringWriter strwritter = new StringWriter();
        //    System.Web.UI.HtmlTextWriter htmltextwrtter = new System.Web.UI.HtmlTextWriter(strwritter);
        //    context.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
        //    context.Response.ContentType = "application/vnd.ms-excel";
        //    context.Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);

        //    dg.GridLines = System.Web.UI.WebControls.GridLines.Both;
        //    dg.HeaderStyle.Font.Bold = true;
        //    dg.RenderControl(htmltextwrtter);


        //    context.Response.Write(strwritter.ToString());
        //    context.Response.End();

        //}
        //public static void ExportToExcel(this DataTable Tbl, string ExcelFilePath = null)
        //{
        //    try
        //    {
        //        if (Tbl == null || Tbl.Columns.Count == 0)
        //            throw new Exception("ExportToExcel: Null or empty input table!\n");

        //        // load excel, and create a new workbook
        //        Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
        //        excelApp.Workbooks.Add();

        //        // single worksheet
        //        Microsoft.Office.Interop.Excel._Worksheet workSheet = excelApp.ActiveSheet;

        //        // column headings
        //        for (int i = 0; i < Tbl.Columns.Count; i++)
        //        {
        //            workSheet.Cells[1, (i + 1)] = Tbl.Columns[i].ColumnName;
        //        }

        //        // rows
        //        for (int i = 0; i < Tbl.Rows.Count; i++)
        //        {
        //            // to do: format datetime values before printing
        //            for (int j = 0; j < Tbl.Columns.Count; j++)
        //            {
        //                workSheet.Cells[(i + 2), (j + 1)] = Tbl.Rows[i][j];
        //            }
        //        }

        //        // check fielpath
        //        if (ExcelFilePath != null && ExcelFilePath != "")
        //        {
        //            try
        //            {
        //                workSheet.SaveAs(ExcelFilePath);
        //                excelApp.Quit();
        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception("ExportToExcel: Excel file could not be saved! Check filepath.\n"
        //                    + ex.Message);
        //            }
        //        }
        //        else    // no filepath is given
        //        {
        //            excelApp.Visible = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("ExportToExcel: \n" + ex.Message);
        //    }
        //}

     

        public static PP_RESULT GetWorkflowPending(PP_REQUEST param)
        {
            PP_RESULT Results = new PP_RESULT();
            List<PP_MODEL> listPP = new List<PP_MODEL>();
            PP_MODEL2 PPview = new PP_MODEL2();
            //List<PP_MODEL> listppmodel = new List<PP_MODEL>();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var stepPP = (from s in context.ART_M_STEP_ARTWORK
                                      where s.STEP_ARTWORK_CODE == "SEND_PP"
                                      select s).FirstOrDefault();

                        var stepPA = (from s in context.ART_M_STEP_ARTWORK
                                      where s.STEP_ARTWORK_CODE == "SEND_PA"
                                      select s).FirstOrDefault();
                        listPP = CNService.GetArtworkSubId(stepPP.STEP_ARTWORK_ID, param, context);
                    }
                    foreach (PP_MODEL pp in listPP)
                    {
                        string[] arrSO = pp.SALES_ORDER_ITEM.Replace("<br>", "|").Split('|');
                        string plant_display = "";
                        string SALES_ORDER = "";
                        string SALES_ORDER_ITEM = "";
                        if (pp.SALES_ORDER_ITEM.Length > 0)
                        {
                            foreach (string s in arrSO)
                            {
                                PPview = new PP_MODEL2();
                                PPview = MapperServices.PPVIEW_TEMPLATE(pp);
                                PPview.ARTWORK_SUB_ID = pp.ARTWORK_SUB_ID;
                                if (s.Trim().Length > 0)
                                {
                                    PPview.SALES_ORDER = s.Trim().Substring(0, 9);
                                    //---------------------------------------------------------start code added by Aof for CR#19439 ---------------------------------------------------------
                                    // PPview.SALES_ORDER_ITEM = s.Trim().Substring(10, (Convert.ToInt32(s.Trim().Length) - 10)).Replace(")", "");
                                    PPview.SALES_ORDER_ITEM = CNService.getDataFromSALES_ORDER_ITEM_RDD(s.Trim(), "ITEM");
                                    //---------------------------------------------------------end code added by Aof for CR#19439 ---------------------------------------------------------
                                    plant_display += CNService.GetSaleOrderItems(PPview, context) + "<br> ";
                                    SALES_ORDER += PPview.SALES_ORDER + "<br> ";
                                    SALES_ORDER_ITEM += PPview.SALES_ORDER_ITEM + "<br> ";
                                }
                            }
                            pp.SALES_ORDER = SALES_ORDER.Substring(0, SALES_ORDER.Length - 5);
                            pp.SALES_ORDER_ITEM = SALES_ORDER_ITEM.Substring(0, SALES_ORDER_ITEM.Length - 5);
                            pp.PLANT = plant_display.Substring(0, plant_display.Length - 5);
                        }
                    }
                    //foreach (PP_MODEL pp in listPP)
                    //{
                    //    string mat_display = "";
                    //    string bom_display = "";
                    //    string[] arrItemsmat = pp.PRODUCT_CODE.Split('|');
                    //    foreach (string s in arrItemsmat)
                    //    {
                    //        if (!String.IsNullOrEmpty(s) && !mat_display.Contains(s))
                    //        {
                    //            mat_display += s + "<br> ";
                    //        }

                    //    }
                    //    string[] arrItemsbom = pp.PKG_CODE.Split('|');
                    //    foreach (string s in arrItemsbom)
                    //    {
                    //        if (!String.IsNullOrEmpty(s) && !mat_display.Contains(s))
                    //        {
                    //            bom_display += s + "<br> ";
                    //        }
                    //    }
                    //    if (!String.IsNullOrEmpty(mat_display) && mat_display != "<br> ")
                    //    {
                    //        pp.PRODUCT_CODE = mat_display.Substring(0, mat_display.Length - 7);
                    //    }
                    //    if (!String.IsNullOrEmpty(bom_display) && bom_display != "<br> ")
                    //    {
                    //        pp.PKG_CODE = bom_display.Substring(0, bom_display.Length - 7);
                    //    }
                    //}
                }
                listPP = listPP.OrderBy(o => o.GROUPING).ThenBy(c => c.PKG_CODE).ThenBy(u => u.WORKFLOW_NO).ToList();
                Results.status = "S";
                Results.data = listPP;

            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static PP_RESULT GetWorkflowPending2(PP_REQUEST param)
        {
            PP_RESULT Results = new PP_RESULT();
            List<ART_WF_ARTWORK_PROCESS_PA> listPA = new List<ART_WF_ARTWORK_PROCESS_PA>();

            List<PP_MODEL> listPP = new List<PP_MODEL>();
            PP_MODEL pp = new PP_MODEL();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var stepPP = (from s in context.ART_M_STEP_ARTWORK
                                      where s.STEP_ARTWORK_CODE == "SEND_PP"
                                      select s).FirstOrDefault();

                        var stepPA = (from s in context.ART_M_STEP_ARTWORK
                                      where s.STEP_ARTWORK_CODE == "SEND_PA"
                                      select s).FirstOrDefault();

                        var getByCreateDateFrom = DateTime.Now;
                        var getByCreateDateTo = DateTime.Now;

                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            getByCreateDateFrom = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_FROM);
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            getByCreateDateTo = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_TO);

                        var q = (from p in context.ART_WF_ARTWORK_PROCESS
                                 where p.CURRENT_STEP_ID == stepPP.STEP_ARTWORK_ID
                                     && p.CURRENT_USER_ID == null
                                     && p.REMARK_KILLPROCESS == null
                                     && (String.IsNullOrEmpty(p.IS_END) || p.IS_END != "X")
                                 select p);

                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            q = q.Where(m => DbFunctions.TruncateTime(m.CREATE_DATE) >= getByCreateDateFrom.Date);
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            q = q.Where(m => DbFunctions.TruncateTime(m.CREATE_DATE) <= getByCreateDateTo.Date);

                        var query = CNService.GetArtworkSubId(stepPP.STEP_ARTWORK_ID, param, context);

                        var proceses = q.ToList();

                        if (proceses != null && proceses.Count > 0)
                        {
                            string remarkByPA = "";
                            List<int> listproceses = proceses.GroupBy(g => new { Grouping = g.ARTWORK_SUB_ID }).Select(g => Convert.ToInt32(g.Key.Grouping)).ToList();
                            var listartworkSubId = (from p in context.ART_WF_ARTWORK_PROCESS
                            where listproceses.Contains(p.ARTWORK_SUB_ID)                                
                            select p).ToList();
                            List<int> listitems = listartworkSubId.GroupBy(g => new { Grouping = g.ARTWORK_ITEM_ID }).Select(g => Convert.ToInt32(g.Key.Grouping)).ToList();
                            var allprocess =(from p in context.ART_WF_ARTWORK_PROCESS where listitems.Contains(p.ARTWORK_ITEM_ID) select p).ToList();
                            List<int> listrequest = proceses.GroupBy(g => new { Grouping = g.ARTWORK_REQUEST_ID }).Select(g => Convert.ToInt32(g.Key.Grouping)).ToList();
                            var allrequest = (from r in context.ART_WF_ARTWORK_REQUEST
                                           where listrequest.Contains(r.ARTWORK_REQUEST_ID)
                                           select r).ToList();


                            foreach (ART_WF_ARTWORK_PROCESS iProcess in proceses)
                            {
                                pp = new PP_MODEL();
                                remarkByPA = "";

                                var listARTWORK_ITEM_ID = listartworkSubId.Where(p=> p.ARTWORK_SUB_ID == iProcess.ARTWORK_SUB_ID).FirstOrDefault();
                                var allSubIDs = allprocess.Where(p=> p.ARTWORK_ITEM_ID == listARTWORK_ITEM_ID.ARTWORK_ITEM_ID).Select(kv => kv.ARTWORK_SUB_ID).ToList();

                                //var allSubIDs = CNService.FindArtworkSubId(iProcess.ARTWORK_SUB_ID, context);

                                //var request = (from r in context.ART_WF_ARTWORK_REQUEST
                                //               where r.ARTWORK_REQUEST_ID == iProcess.ARTWORK_REQUEST_ID
                                //               select r).FirstOrDefault();
                                var request = allrequest.Where(p => p.ARTWORK_REQUEST_ID == iProcess.ARTWORK_REQUEST_ID).FirstOrDefault();
                                if (!String.IsNullOrEmpty(iProcess.REMARK))
                                {
                                    remarkByPA = iProcess.REMARK; // CNService.RemoveHTMLTag(iProcess.REMARK);
                                }

                                pp.REMARK_BY_PA = remarkByPA;
                                pp.ARTWORK_REQUEST_ID = iProcess.ARTWORK_REQUEST_ID;
                                pp.ARTWORK_SUB_ID = iProcess.ARTWORK_SUB_ID;
                                pp.ARTWORK_ITEM_ID = iProcess.ARTWORK_ITEM_ID;

                                var requestItemID = listartworkSubId.Where(p=> p.ARTWORK_SUB_ID== iProcess.ARTWORK_SUB_ID).FirstOrDefault();
                                //var requestItemID = CNService.FindArtworkItemId(iProcess.ARTWORK_SUB_ID, context);

                                var processPA = (from a in context.ART_WF_ARTWORK_PROCESS_PA
                                                 where allSubIDs.Contains(a.ARTWORK_SUB_ID)
                                                 select a).OrderByDescending(o => o.ARTWORK_SUB_PA_ID).FirstOrDefault();

                                if (processPA != null)
                                {
                                    if (request != null)
                                    {
                                        var requestItem = (from r in context.ART_WF_ARTWORK_REQUEST_ITEM
                                                           where r.ARTWORK_ITEM_ID == iProcess.ARTWORK_ITEM_ID
                                                           select r).FirstOrDefault();
                                        if (request.SOLD_TO_ID != null && request.SOLD_TO_ID > 0)
                                        {
                                            pp.SOLD_TO_ID = Convert.ToInt32(request.SOLD_TO_ID);

                                            var customer = (from p in context.XECM_M_CUSTOMER
                                                            where p.CUSTOMER_ID == request.SOLD_TO_ID
                                                            select p).FirstOrDefault();
                                            if (customer != null)
                                            {
                                                pp.SOLD_TO_DISPLAY_TXT = customer.CUSTOMER_CODE + ":" + customer.CUSTOMER_NAME;
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }

                                        if (request.SOLD_TO_ID != null && request.SHIP_TO_ID > 0)
                                        {
                                            pp.SHIP_TO_ID = Convert.ToInt32(request.SHIP_TO_ID);

                                            var customer = (from p in context.XECM_M_CUSTOMER
                                                            where p.CUSTOMER_ID == request.SHIP_TO_ID
                                                            select p).FirstOrDefault();
                                            if (customer != null)
                                            {
                                                pp.SHIP_TO_DISPLAY_TXT = customer.CUSTOMER_CODE + ":" + customer.CUSTOMER_NAME;
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }

                                        if (request.REQUEST_DELIVERY_DATE != null)
                                        {
                                            pp.RDD = request.REQUEST_DELIVERY_DATE;// Convert.ToDateTime(request.REQUEST_DELIVERY_DATE).ToString("dd/MM/yyyy");
                                        }

                                        if (!String.IsNullOrEmpty(request.BRAND_OTHER))
                                        {
                                            pp.BRAND_DISPLAY_TXT = request.BRAND_OTHER;
                                        }
                                        else if (request.BRAND_ID != null)
                                        {
                                            pp.BRAND_ID = Convert.ToInt32(request.BRAND_ID);

                                            var brand = (from b in context.SAP_M_BRAND
                                                         where b.BRAND_ID == request.BRAND_ID
                                                         select b).FirstOrDefault();

                                            if (brand != null)
                                            {
                                                pp.BRAND_DISPLAY_TXT = brand.MATERIAL_GROUP + ":" + brand.DESCRIPTION;
                                            }
                                        }

                                        if (processPA != null)
                                        {
                                            if (processPA.MATERIAL_GROUP_ID != null)
                                            {
                                                pp.PKG_TYPE_ID = Convert.ToInt32(processPA.MATERIAL_GROUP_ID);
                                                pp.PKG_TYPE_DISPLAY_TXT = CNService.GetCharacteristicDescription(processPA.MATERIAL_GROUP_ID, context);
                                            }
                                        }

                                        if (!String.IsNullOrEmpty(requestItem.REQUEST_ITEM_NO))
                                        {
                                            pp.WORKFLOW_NO = requestItem.REQUEST_ITEM_NO;
                                        }

                                        var stepPG = (from g in context.ART_M_STEP_ARTWORK
                                                      where g.STEP_ARTWORK_CODE == "SEND_PG"
                                                      select g).FirstOrDefault();
                                        var processParentTmp = allprocess.Where(g=> g.ARTWORK_ITEM_ID == iProcess.ARTWORK_ITEM_ID
                                                                    && g.CURRENT_STEP_ID == stepPG.STEP_ARTWORK_ID)
                                                                .Select(g=>g.ARTWORK_SUB_ID).ToList();
                                        //var processParentTmp = (from g in context.ART_WF_ARTWORK_PROCESS
                                        //                        where g.ARTWORK_ITEM_ID == iProcess.ARTWORK_ITEM_ID
                                        //                            && g.CURRENT_STEP_ID == stepPG.STEP_ARTWORK_ID
                                        //                        select g.ARTWORK_SUB_ID).ToList();

                                        if (processParentTmp != null && processParentTmp.Count > 0)
                                        {
                                            var processPG = (from g in context.ART_WF_ARTWORK_PROCESS_PG
                                                             where processParentTmp.Contains(g.ARTWORK_SUB_ID)
                                                             orderby g.UPDATE_DATE descending
                                                             select g).FirstOrDefault();

                                            if (processPG != null)
                                            {
                                                if (processPG.DIE_LINE_MOCKUP_ID != null)
                                                {
                                                    var processMO = (from p in context.ART_WF_MOCKUP_PROCESS
                                                                     where p.MOCKUP_ID == processPG.DIE_LINE_MOCKUP_ID
                                                                     select p).FirstOrDefault();

                                                    if (processMO != null)
                                                    {
                                                        var processMoPG = (from pg in context.ART_WF_MOCKUP_PROCESS_PG
                                                                           where pg.MOCKUP_SUB_ID == processMO.MOCKUP_SUB_ID
                                                                           select pg).OrderByDescending(o => o.MOCKUP_SUB_PG_ID).FirstOrDefault();

                                                        if (processPG != null)
                                                        {
                                                            if (processMoPG.VENDOR > 0)
                                                            {
                                                                pp.VENDOR_DISPLAY_TXT = CNService.GetVendorCodeName(processMoPG.VENDOR, context);
                                                            }
                                                            else
                                                            {
                                                                pp.VENDOR_DISPLAY_TXT = processMoPG.VENDOR_OTHER;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        if (string.IsNullOrEmpty(pp.VENDOR_DISPLAY_TXT))
                                        {
                                            if (!string.IsNullOrEmpty(processPA.MATERIAL_NO))
                                            {
                                                var SAP_M_MATERIAL_CONVERSION = (from p in context.SAP_M_MATERIAL_CONVERSION
                                                                                 where p.MATERIAL_NO == processPA.MATERIAL_NO
                                                                                 && p.CHAR_NAME == "ZPKG_SEC_VENDOR"
                                                                                 select p).FirstOrDefault();
                                                if (SAP_M_MATERIAL_CONVERSION != null)
                                                {
                                                    var vendorMaster = XECM_M_VENDOR_SERVICE.GetByItem(new XECM_M_VENDOR() { VENDOR_CODE = SAP_M_MATERIAL_CONVERSION.CHAR_VALUE }, context).FirstOrDefault();
                                                    if (vendorMaster != null)
                                                    {
                                                        pp.VENDOR_DISPLAY_TXT = vendorMaster.VENDOR_CODE + ":" + vendorMaster.VENDOR_NAME;
                                                    }
                                                }
                                            }
                                        }

                                        var ARTWORK_SUB_ID = iProcess.ARTWORK_SUB_ID;
                                        if (iProcess.PARENT_ARTWORK_SUB_ID > 0)
                                            ARTWORK_SUB_ID = Convert.ToInt32(iProcess.PARENT_ARTWORK_SUB_ID);

                                        var soDetails = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                                         where s.ARTWORK_SUB_ID == ARTWORK_SUB_ID
                                                         select s).ToList();

                                        if (soDetails != null && soDetails.Count > 0)
                                        {
                                            var fistSO = soDetails.FirstOrDefault().SALES_ORDER_NO;
                                            pp.SALES_ORG = (from s in context.SAP_M_PO_COMPLETE_SO_HEADER
                                                            where s.SALES_ORDER_NO == fistSO
                                                            select s.SALES_ORG).FirstOrDefault();

                                            string so_display = "";
                                            string so_displayTmp = "";
                                            string mat_display = "";
                                            string bom_display = "";

                                            foreach (ART_WF_ARTWORK_PROCESS_SO_DETAIL so in soDetails)
                                            {
                                                so_displayTmp = so.SALES_ORDER_NO + "(" + so.SALES_ORDER_ITEM.ToString().Trim() + ")";
                                                so_display += so_displayTmp + "<br> ";

                                                if (!String.IsNullOrEmpty(so.MATERIAL_NO) && !mat_display.Contains(so.MATERIAL_NO))
                                                {
                                                    mat_display += so.MATERIAL_NO + "<br> ";
                                                }

                                                if (so.BOM_ID > 0)
                                                {
                                                    bom_display += CNService.GetBOMNo(Convert.ToInt32(so.BOM_ID), context) + "<br> ";
                                                }
                                            }

                                            if (!String.IsNullOrEmpty(so_display) && so_display != "<br> ")
                                            {
                                                pp.SALES_ORDER_ITEM = so_display.Substring(0, so_display.Length - 5);
                                            }

                                            if (!String.IsNullOrEmpty(mat_display) && mat_display != "<br> ")
                                            {
                                                pp.PRODUCT_CODE = mat_display.Substring(0, mat_display.Length - 7);
                                            }

                                            if (!String.IsNullOrEmpty(bom_display) && bom_display != "<br> ")
                                            {
                                                pp.PKG_CODE = bom_display.Substring(0, bom_display.Length - 7);
                                            }
                                            pp.RECEIVE_DATE = iProcess.CREATE_DATE;//.ToString("dd/MM/yyyy HH:mm:ss");
                                        }
                                    }

                                    if (iProcess != null)
                                    {
                                        if (processPA != null)
                                        {
                                            if (!String.IsNullOrEmpty(processPA.MATERIAL_NO))
                                            {
                                                pp.PKG_CODE = processPA.MATERIAL_NO;
                                            }
                                        }

                                    }

                                    string vendor = "";
                                    string pkg = "";

                                    if (!String.IsNullOrEmpty(pp.VENDOR_DISPLAY_TXT))
                                    {
                                        vendor = pp.VENDOR_DISPLAY_TXT;
                                    }

                                    if (!String.IsNullOrEmpty(pp.PKG_CODE))
                                    {
                                        pkg = pp.PKG_CODE;
                                    }

                                    pp.GROUPING = pp.SOLD_TO_ID.ToString() +
                                                    pp.SHIP_TO_ID.ToString() +
                                                   vendor;
                                    listPP.Add(pp);
                                }
                            }
                        }
                    }
                }
                listPP = listPP.OrderBy(o => o.GROUPING).ToList();
                Results.status = "S";
                Results.data = listPP;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static PP_VENDOR_RESULT GetWorkflowPendingToVendor(ART_WF_ARTWORK_PROCESS_PP_REQUEST param)
        {
            PP_VENDOR_RESULT Results = new PP_VENDOR_RESULT();
            PP_VENDOR_MODEL ppVendor = new PP_VENDOR_MODEL();
            List<PP_VENDOR_MODEL> listPPVendor = new List<PP_VENDOR_MODEL>();

            int stepPPID = 0;
            int stepPGID = 0;
            int stepMockupPGID = 0;

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        stepMockupPGID = context.ART_M_STEP_MOCKUP.Where(s => s.STEP_MOCKUP_CODE == "SEND_PG").Select(s => s.STEP_MOCKUP_ID).FirstOrDefault();
                        stepPPID = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == "SEND_PP").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
                        stepPGID = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == "SEND_PG").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();

                        var processesPP = (from p in context.ART_WF_ARTWORK_PROCESS
                                           where p.CURRENT_STEP_ID == stepPPID
                                               && p.CURRENT_USER_ID == param.data.UPDATE_BY
                                               && String.IsNullOrEmpty(p.IS_END)
                                           select p.ARTWORK_ITEM_ID).ToList();

                        var requestItemPP = (from i in context.ART_WF_ARTWORK_REQUEST_ITEM
                                             where processesPP.Contains(i.ARTWORK_ITEM_ID)
                                             select i.REQUEST_ITEM_NO).ToList();

                        var mappingAWPO = (from p in context.ART_WF_ARTWORK_MAPPING_PO
                                           where requestItemPP.Contains(p.ARTWORK_NO)
                                           && p.IS_ACTIVE == "X"
                                           select p.ARTWORK_NO).ToList();

                        var requestItemsMapping = (from r in context.ART_WF_ARTWORK_REQUEST_ITEM
                                                   where mappingAWPO.Contains(r.REQUEST_ITEM_NO)
                                                   select r.ARTWORK_ITEM_ID).ToList();

                        var processesPPsMapping = (from p in context.ART_WF_ARTWORK_PROCESS
                                                   where requestItemsMapping.Contains(p.ARTWORK_ITEM_ID)
                                                    && p.CURRENT_STEP_ID == stepPPID
                                                    && String.IsNullOrEmpty(p.IS_END)
                                                   select p).ToList();

                        foreach (ART_WF_ARTWORK_PROCESS iProcessPP in processesPPsMapping)
                        {
                            var isLock = CNService.IsLock(iProcessPP.ARTWORK_SUB_ID, context);
                            ppVendor = new PP_VENDOR_MODEL();
                            if (isLock) ppVendor.IS_LOCK = "X";
                            ppVendor.ARTWORK_SUB_ID = iProcessPP.ARTWORK_SUB_ID;
                            ppVendor.ARTWORK_ITEM_ID = iProcessPP.ARTWORK_ITEM_ID;
                            ppVendor.ARTWORK_REQUEST_ID = iProcessPP.ARTWORK_REQUEST_ID;
                            ppVendor.PO = "";

                            var request = (from r in context.ART_WF_ARTWORK_REQUEST
                                           where r.ARTWORK_REQUEST_ID == iProcessPP.ARTWORK_REQUEST_ID
                                           select r).FirstOrDefault();

                            var requestItem = (from r in context.ART_WF_ARTWORK_REQUEST_ITEM
                                               where r.ARTWORK_ITEM_ID == iProcessPP.ARTWORK_ITEM_ID
                                               select r).FirstOrDefault();

                            if (requestItem != null)
                            {
                                var mappingPO = (from p in context.ART_WF_ARTWORK_MAPPING_PO
                                                 where p.ARTWORK_NO == requestItem.REQUEST_ITEM_NO
                                                  && p.IS_ACTIVE == "X"
                                                 select p).OrderByDescending(o => o.PO_NO).ToList();

                                if (mappingPO != null)
                                {
                                    string concatPO = "";
                                    string po = "";
                                    foreach (var item in mappingPO)
                                    {
                                        po = item.PO_NO + "<br>";
                                        concatPO += po;
                                    }

                                    if (!String.IsNullOrEmpty(concatPO))
                                    {
                                        ppVendor.PO = concatPO.Substring(0, concatPO.Length - 4);
                                    }
                                }

                            }
                            if (request != null)
                            {
                                if (request.BRAND_ID != null)
                                {
                                    var brand = (from b in context.SAP_M_BRAND
                                                 where b.BRAND_ID == request.BRAND_ID
                                                 select b).FirstOrDefault();

                                    if (brand != null)
                                    {
                                        ppVendor.BRAND = brand.MATERIAL_GROUP + ":" + brand.DESCRIPTION;
                                    }
                                }

                                if (request.REQUEST_DELIVERY_DATE != null)
                                {
                                    ppVendor.RDD = request.REQUEST_DELIVERY_DATE;// Convert.ToDateTime(request.REQUEST_DELIVERY_DATE).ToString("dd.MM.yyyy");
                                }
                            }
                            if (requestItem != null)
                            {
                                ppVendor.WORKFLOW_NO = requestItem.REQUEST_ITEM_NO;
                            }

                            var ARTWORK_SUB_ID = iProcessPP.ARTWORK_SUB_ID;
                            if (iProcessPP.PARENT_ARTWORK_SUB_ID > 0)
                            {
                                ARTWORK_SUB_ID = Convert.ToInt32(iProcessPP.PARENT_ARTWORK_SUB_ID);
                            }

                            var soDetails = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                             where s.ARTWORK_SUB_ID == ARTWORK_SUB_ID
                                             select s).ToList();

                            if (soDetails != null)
                            {
                                if (soDetails.Count > 0)
                                {
                                    string so_display = "";
                                    string so_displayTmp = "";
                                    string mat_display = "";
                                    string bom_display = "";

                                    foreach (ART_WF_ARTWORK_PROCESS_SO_DETAIL iSODetail in soDetails)
                                    {
                                        so_displayTmp = iSODetail.SALES_ORDER_NO + "(" + iSODetail.SALES_ORDER_ITEM.ToString().Trim() + ")";
                                        so_display += so_displayTmp + "<br> ";

                                        if (!String.IsNullOrEmpty(iSODetail.MATERIAL_NO) && !mat_display.Contains(iSODetail.MATERIAL_NO))
                                        {
                                            mat_display += iSODetail.MATERIAL_NO + "<br> ";
                                        }

                                        if (iSODetail.BOM_ID > 0)
                                        {
                                            bom_display += CNService.GetBOMNo(Convert.ToInt32(iSODetail.BOM_ID), context) + "<br> ";
                                        }
                                    }

                                    if (!String.IsNullOrEmpty(so_display))
                                    {
                                        ppVendor.SALES_ORDER_NO = so_display.Substring(0, so_display.Length - 5);
                                    }

                                    if (!String.IsNullOrEmpty(mat_display))
                                    {
                                        ppVendor.PRODUCT_CODE = mat_display.Substring(0, mat_display.Length - 5);
                                    }

                                    var paTask = (from g in context.ART_WF_ARTWORK_PROCESS_PA
                                                  where g.ARTWORK_SUB_ID == ARTWORK_SUB_ID
                                                  select g).FirstOrDefault();

                                    if (!String.IsNullOrEmpty(paTask.MATERIAL_NO))
                                    {
                                        ppVendor.PKG_CODE = paTask.MATERIAL_NO;
                                    }
                                    if (String.IsNullOrEmpty(ppVendor.PKG_CODE))
                                    {
                                        if (!String.IsNullOrEmpty(bom_display))
                                        {
                                            ppVendor.PKG_CODE = bom_display.Substring(0, bom_display.Length - 5);
                                        }
                                    }

                                }
                            }

                            var processPG = (from p in context.ART_WF_ARTWORK_PROCESS
                                             where p.CURRENT_STEP_ID == stepPGID
                                                 && p.IS_END == "X"
                                                 && p.REMARK_KILLPROCESS == null
                                                 && p.ARTWORK_ITEM_ID == iProcessPP.ARTWORK_ITEM_ID
                                             select p).OrderByDescending(o => o.ARTWORK_SUB_ID).FirstOrDefault();

                            if (processPG != null)
                            {
                                var pgTask = (from g in context.ART_WF_ARTWORK_PROCESS_PG
                                              where g.ARTWORK_SUB_ID == processPG.ARTWORK_SUB_ID
                                              select g).FirstOrDefault();

                                if (pgTask != null)
                                {
                                    if (pgTask.DIE_LINE_MOCKUP_ID != null && pgTask.DIE_LINE_MOCKUP_ID > 0)
                                    {
                                        var mockup = (from m in context.ART_WF_MOCKUP_PROCESS
                                                      where m.MOCKUP_ID == pgTask.DIE_LINE_MOCKUP_ID
                                                        && m.CURRENT_STEP_ID == stepMockupPGID
                                                      select m).FirstOrDefault();

                                        if (mockup != null)
                                        {
                                            var mockupPG = (from m in context.ART_WF_MOCKUP_PROCESS_PG
                                                            where m.MOCKUP_SUB_ID == mockup.MOCKUP_SUB_ID
                                                            select m).OrderByDescending(o => o.MOCKUP_SUB_PG_ID).FirstOrDefault();
                                            if (mockupPG != null)
                                            {
                                                if (mockupPG.VENDOR != null || mockupPG.VENDOR_OTHER != null)
                                                {
                                                    if (mockupPG.VENDOR != null)
                                                    {
                                                        ppVendor.VENDOR_DISPLAY_TXT = CNService.GetVendorCodeName(mockupPG.VENDOR, context);
                                                        ppVendor.VENDOR_ID = Convert.ToInt32(mockupPG.VENDOR);
                                                    }
                                                    else
                                                    {
                                                        ppVendor.VENDOR_DISPLAY_TXT = mockupPG.VENDOR_OTHER;
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                            }

                            if (string.IsNullOrEmpty(ppVendor.VENDOR_DISPLAY_TXT))
                            {
                                var parentId = CNService.FindParentArtworkSubId(iProcessPP.ARTWORK_SUB_ID, context);
                                var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                 where p.ARTWORK_SUB_ID == parentId
                                                 select p).FirstOrDefault();

                                if (processPA != null)
                                {
                                    if (!string.IsNullOrEmpty(processPA.MATERIAL_NO))
                                    {
                                        var SAP_M_MATERIAL_CONVERSION = (from p in context.SAP_M_MATERIAL_CONVERSION
                                                                         where p.MATERIAL_NO == processPA.MATERIAL_NO
                                                                         && p.CHAR_NAME == "ZPKG_SEC_VENDOR"
                                                                         select p).FirstOrDefault();
                                        if (SAP_M_MATERIAL_CONVERSION != null)
                                        {
                                            var vendorMaster = XECM_M_VENDOR_SERVICE.GetByItem(new XECM_M_VENDOR() { VENDOR_CODE = SAP_M_MATERIAL_CONVERSION.CHAR_VALUE }, context).FirstOrDefault();
                                            if (vendorMaster != null)
                                            {
                                                ppVendor.VENDOR_ID = vendorMaster.VENDOR_ID;
                                                ppVendor.VENDOR_DISPLAY_TXT = vendorMaster.VENDOR_CODE + ":" + vendorMaster.VENDOR_NAME;
                                            }
                                        }
                                    }
                                }
                            }

                            ppVendor.GROUPING = ppVendor.VENDOR_ID + CNService.RemoveHTMLTag(ppVendor.PO);
                            listPPVendor.Add(ppVendor);
                        }
                    }
                }

                Results.data = listPPVendor;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }


            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_RESULT SaveMultiVendorByPP(ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_REQUEST_LIST param)
        {
            using (var context = new ARTWORKEntities())
            {
                using (var dbContextTransaction = CNService.IsolationLevel(context))
                {
                    return aSaveMultiVendorByPP(param, context, dbContextTransaction);
                }
            }
        }

        public static ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_RESULT aSaveMultiVendorByPP(ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_REQUEST_LIST param, ARTWORKEntities context, DbContextTransaction dbContextTransaction)
        {
            ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_RESULT Results = new ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_RESULT();
            ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2 item = new ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2();
            List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2> listItem = new List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2>();
            List<ART_WF_ARTWORK_PROCESS_2> listCompleted = new List<ART_WF_ARTWORK_PROCESS_2>();

            try
            {
                foreach (ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2 pa in param.data)
                {
                    var isLock = CNService.IsLock(pa.ARTWORK_SUB_ID, context);
                    if (isLock)
                    {
                        Results.status = "E";
                        Results.msg = "Some workitem is locked.<br/>Please contact your PA supervisor.";
                        return Results;
                    }
                }

                foreach (ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2 pa in param.data)
                {
                    if (pa.VENDOR_ID > 0)
                    {
                        var vendorUser = (from v in context.ART_M_USER_VENDOR
                                          where v.VENDOR_ID == pa.VENDOR_ID
                                          select v).ToList();

                        if (vendorUser.Count == 0)
                        {
                            Results.status = "E";
                            Results.msg = "Cannot send some workflow to vendor.<br/>System not found user for vendor.";
                            return Results;
                        }
                    }
                    else
                    {
                        Results.status = "E";
                        Results.msg = "Cannot send some workflow to vendor.<br/>System not found vendor.";
                        return Results;
                    }
                }

                foreach (ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2 pa in param.data)
                {
                    var ARTWORK_ITEM_ID = context.ART_WF_ARTWORK_PROCESS.Where(w => w.ARTWORK_SUB_ID == pa.ARTWORK_SUB_ID).Select(s => s.ARTWORK_ITEM_ID).FirstOrDefault();
                    var requestNo = context.ART_WF_ARTWORK_REQUEST_ITEM.Where(w => w.ARTWORK_ITEM_ID == ARTWORK_ITEM_ID).Select(s => s.REQUEST_ITEM_NO).FirstOrDefault();

                    var mappingAWPO = (from p in context.ART_WF_ARTWORK_MAPPING_PO
                                       where p.ARTWORK_NO == requestNo
                                        && p.IS_ACTIVE == "X"
                                       select p).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                    if (mappingAWPO == null)
                    {
                        Results.status = "E";
                        Results.msg = MessageHelper.GetMessage("MSG_012", context);
                        return Results;
                    }
                }

                var token = CWSService.getAuthToken();
                foreach (ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2 vendor in param.data)
                {
                    var folderPOName = ConfigurationManager.AppSettings["ArtworkFolderNamePO"];
                    var parentPOID = ConfigurationManager.AppSettings["PONodeID"];
                    if (vendor.PROCESS != null)
                    {
                        var artworkNo = context.ART_WF_ARTWORK_REQUEST_ITEM
                                        .Where(r => r.ARTWORK_ITEM_ID == vendor.PROCESS.ARTWORK_ITEM_ID)
                                        .FirstOrDefault();

                        var listPO = (from p in context.ART_WF_ARTWORK_MAPPING_PO
                                      where p.ARTWORK_NO == artworkNo.REQUEST_ITEM_NO
                                       && p.IS_ACTIVE == "X"
                                      select p.PO_NO).Distinct().ToList();

                        foreach (string iPO in listPO)
                        {
                            Node nodeParent = CWSService.getNodeByName(Convert.ToInt64(parentPOID), iPO, token);
                            if (nodeParent == null)
                            {
                                Results.status = "E";
                                Results.msg = "System not found PO workspace.<br/>Please try again." + " (" + iPO + ")";
                                return Results;
                            }
                            else
                            {
                                Node node = CWSService.getNodeByName(nodeParent.ID * (-1), folderPOName, token);
                                if (node != null)
                                {
                                    Node[] nodePOFiles = CWSService.getAllNodeInFolder(node.ID, token);

                                    if (nodePOFiles == null || nodePOFiles.Count() == 0)
                                    {
                                        Results.status = "E";
                                        Results.msg = "System not found PO files in PO workspace.<br/>Please try again." + " (" + iPO + ")";
                                        return Results;
                                    }
                                }
                            }
                        }
                    }
                }

                ART_WF_ARTWORK_PROCESS_RESULT processResults = new ART_WF_ARTWORK_PROCESS_RESULT();

                StampPrintMasterData(param, context);
                CopyPOToArtwork(param, context);

                if (param != null && param.data != null && param.data.Count > 0)
                {
                    foreach (ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2 vendor in param.data)
                    {
                        item = new ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2();
                        listItem = new List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2>();

                        if (vendor.VENDOR_ID > 0)
                        {
                            var vendorUser = (from v in context.ART_M_USER_VENDOR
                                              where v.VENDOR_ID == vendor.VENDOR_ID
                                              select v).ToList();

                            if (vendorUser != null && vendorUser.Count > 0)
                            {
                                foreach (ART_M_USER_VENDOR iUser in vendorUser)
                                {
                                    processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
                                    if (vendor.PROCESS != null)
                                    {
                                        vendor.PROCESS.CURRENT_USER_ID = iUser.USER_ID;
                                        vendor.PROCESS.CURRENT_VENDOR_ID = iUser.VENDOR_ID;

                                        processResults = ArtworkProcessHelper.SaveProcess(vendor.PROCESS, context);
                                        if (processResults.data.Count > 0)
                                            listCompleted.Add(processResults.data[0]);
                                    }

                                    ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP vendorData = new ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP();
                                    vendorData = MapperServices.ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP(vendor);

                                    if (processResults != null && processResults.data != null && processResults.data.Count > 0)
                                    {
                                        vendorData.ARTWORK_SUB_ID = processResults.data[0].ARTWORK_SUB_ID;
                                    }

                                    ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_SERVICE.SaveOrUpdate(vendorData, context);

                                    item.ARTWORK_PROCESS_VENDOR_BY_PP_ID = vendorData.ARTWORK_PROCESS_VENDOR_BY_PP_ID;
                                    listItem.Add(item);
                                    vendor.ENDTASKFORM = true;
                                }
                            }

                            if (vendor.ENDTASKFORM)
                            {
                                ArtworkProcessHelper.EndTaskForm(vendor.ARTWORK_SUB_ID, vendor.UPDATE_BY, context);
                            }
                        }
                    }
                }

                Results.data = listItem;

                dbContextTransaction.Commit();

                foreach (var process in listCompleted)
                    EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_PO_TO_VENDOR", context);

                Results.status = "S";
                Results.msg = MessageHelper.GetMessage("MSG_001", context);
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static void StampPrintMasterData(ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_REQUEST_LIST param, ARTWORKEntities context)
        {
            var folderName = ConfigurationManager.AppSettings["ArtworkFolderNamePrintMaster"];
            var parentID = ConfigurationManager.AppSettings["ArtworkNodeID"];

            long parentNodeID = Convert.ToInt64(parentID);
            var currentUserId = CNService.getCurrentUser(context);
            var token = CWSService.getAuthToken();
            if (param != null && param.data != null && param.data.Count > 0)
            {
                foreach (ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2 vendor in param.data)
                {
                    if (vendor.PROCESS != null)
                    {
                        var artworkNo = context.ART_WF_ARTWORK_REQUEST_ITEM
                                        .Where(r => r.ARTWORK_ITEM_ID == vendor.PROCESS.ARTWORK_ITEM_ID)
                                        .FirstOrDefault();

                        var processSubIDs = context.ART_WF_ARTWORK_PROCESS
                                                .Where(p => p.ARTWORK_ITEM_ID == artworkNo.ARTWORK_ITEM_ID)
                                                .Select(s => s.ARTWORK_SUB_ID)
                                                .ToList();

                        var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                         where processSubIDs.Contains(p.ARTWORK_SUB_ID)
                                         select p).OrderByDescending(o => o.ARTWORK_SUB_ID).FirstOrDefault();

                        var soDetail = (from p in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                        where processSubIDs.Contains(p.ARTWORK_SUB_ID)
                                        select p).ToList();

                        string salesOrderNo = "";
                        string materialDesc = "";
                        string materialBomDesc = "";

                        foreach (ART_WF_ARTWORK_PROCESS_SO_DETAIL iSO in soDetail)
                        {
                            if (salesOrderNo == "")
                            {
                                salesOrderNo = iSO.SALES_ORDER_NO + "(" + iSO.SALES_ORDER_ITEM + ")";
                            }
                            else
                            {
                                salesOrderNo += "," + iSO.SALES_ORDER_NO + "(" + iSO.SALES_ORDER_ITEM + ")";
                            }
                        }

                        if (processPA != null)
                        {
                            if (!String.IsNullOrEmpty(processPA.MATERIAL_NO))
                            {
                                materialBomDesc = "Mat. " + processPA.MATERIAL_NO;
                            }

                            if (processPA.PRODUCT_CODE_ID != null)
                            {
                                var xecmProduct = (from d in context.XECM_M_PRODUCT
                                                   where d.XECM_PRODUCT_ID == processPA.PRODUCT_CODE_ID
                                                   select d.PRODUCT_CODE).FirstOrDefault();
                                if (xecmProduct != null)
                                {
                                    materialDesc = xecmProduct + "/";
                                }
                            }

                            var productPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT
                                             where p.ARTWORK_SUB_ID == processPA.ARTWORK_SUB_ID
                                             select p).ToList();

                            if (productPA != null)
                            {
                                foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT iProduct in productPA)
                                {
                                    var xecmProduct = (from d in context.XECM_M_PRODUCT
                                                       where d.XECM_PRODUCT_ID == iProduct.PRODUCT_CODE_ID
                                                       select d.PRODUCT_CODE).FirstOrDefault();

                                    if (xecmProduct != null)
                                    {
                                        if (String.IsNullOrEmpty(materialDesc))
                                        {
                                            materialDesc += xecmProduct;
                                        }
                                        else
                                        {
                                            materialDesc += "/" + xecmProduct;
                                        }

                                    }

                                }
                            }

                            var productOtherPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER
                                                  where p.ARTWORK_SUB_ID == processPA.ARTWORK_SUB_ID
                                                  select p).ToList();

                            if (productOtherPA != null)
                            {
                                foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER iProduct in productOtherPA)
                                {
                                    if (String.IsNullOrEmpty(materialDesc))
                                    {
                                        materialDesc += iProduct.PRODUCT_CODE;
                                    }
                                    else
                                    {
                                        materialDesc += "/" + iProduct.PRODUCT_CODE;
                                    }
                                }
                            }

                        }

                        Node nodeAW = CWSService.getNodeByName(parentNodeID, artworkNo.REQUEST_ITEM_NO, token);

                        var artworkReq = context.ART_WF_ARTWORK_REQUEST
                                       .Where(r => r.ARTWORK_REQUEST_ID == artworkNo.ARTWORK_REQUEST_ID)
                                       .FirstOrDefault();

                        string folderNameTmp = "";
                        if (artworkReq.TYPE_OF_ARTWORK == "NEW")
                        {
                            folderNameTmp = folderName;
                        }
                        else if (artworkReq.TYPE_OF_ARTWORK == "REPEAT")
                        {
                            folderNameTmp = folderName;
                        }

                        Node nodePrintMS = CWSService.getNodeByName(nodeAW.ID, folderNameTmp, token);

                        Node[] nodesFilePrintMS = CWSService.getAllNodeInFolder(nodePrintMS.ID, token);

                        if (nodesFilePrintMS != null && nodesFilePrintMS.Count() > 0)
                        {
                            foreach (Node iNode in nodesFilePrintMS)
                            {
                                if (iNode.VersionInfo.MimeType == "application/pdf")
                                {
                                    Stream downloadStream = null;

                                    downloadStream = CWSService.downloadFile(iNode.ID, token);

                                    var filePath = CNService.StampToPDF(salesOrderNo, materialDesc, materialBomDesc, downloadStream,iNode.ID);
                                    var length = new System.IO.FileInfo(filePath).Length;

                                    DocumentManagement.Version newVersion = CWSService.addVersionFile(filePath, iNode.Name, nodePrintMS.ID, iNode.ID, "StampInformation", token);

                                    long last_version = newVersion.VerMinor;
 
                                    //start ticket#438889  
                                    //var oldNode = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(new ART_WF_ARTWORK_ATTACHMENT() { NODE_ID = iNode.ID, VERSION = 1 }, context).LastOrDefault(); comment by aof
                                    var oldNode = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(new ART_WF_ARTWORK_ATTACHMENT() { NODE_ID = iNode.ID, VERSION2 = "1.0" }, context).LastOrDefault(); // updated where Version is Version2 by aof
                                    //last ticket#438889 
                                    if (oldNode != null)
                                    {
                                        AttachmentArtworkHelper.SaveAttachmentFileVersionStampPrintMS(oldNode.FILE_NAME, oldNode.EXTENSION, oldNode.CONTENT_TYPE, oldNode.ARTWORK_REQUEST_ID,
                                           oldNode.ARTWORK_SUB_ID, Convert.ToInt32(length), Convert.ToInt64(oldNode.NODE_ID), currentUserId, last_version, Convert.ToInt32(oldNode.STEP_ARTWORK_ID), context);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void CopyPOToArtwork(ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_REQUEST_LIST param, ARTWORKEntities context)
        {
            //var folderPOName = ConfigurationManager.AppSettings["ArtworkFolderNamePO"];
            //var folderAWPOName = ConfigurationManager.AppSettings["ArtworkFolderNameOther"];
            //var parentAWID = ConfigurationManager.AppSettings["ArtworkNodeID"];
            //var parentPOID = ConfigurationManager.AppSettings["PONodeID"];

            //long parentNodeID = Convert.ToInt64(parentPOID);

            //ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_RESULT Results = new ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_RESULT();
            //try
            //{
            //    ART_WF_ARTWORK_PROCESS_RESULT processResults = new ART_WF_ARTWORK_PROCESS_RESULT();

            //    if (param != null && param.data != null && param.data.Count > 0)
            //    {
            //        foreach (ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2 vendor in param.data)
            //        {
            //            if (vendor.PROCESS != null)
            //            {
            //                var artworkNo = context.ART_WF_ARTWORK_REQUEST_ITEM
            //                                .Where(r => r.ARTWORK_ITEM_ID == vendor.PROCESS.ARTWORK_ITEM_ID)
            //                                .FirstOrDefault();

            //                var listPO = (from p in context.ART_WF_ARTWORK_MAPPING_PO
            //                              where p.ARTWORK_NO == artworkNo.REQUEST_ITEM_NO
            //                               && p.IS_ACTIVE == "X"
            //                              select p.PO_NO).ToList();

            //                if (listPO != null && listPO.Count > 0)
            //                {
            //                    foreach (string iPO in listPO)
            //                    {
            //                        Node nodeParent = CWSService.getNodeByName(Convert.ToInt64(parentPOID), iPO);
            //                        Node node = CWSService.getNodeByName(nodeParent.ID * (-1), folderPOName);

            //                        if (node != null)
            //                        {
            //                            Node[] nodePOFiles = CWSService.getAllNodeInFolder(node.ID);

            //                            if (nodePOFiles != null && nodePOFiles.Count() > 0)
            //                            {
            //                                foreach (Node iPOFile in nodePOFiles)
            //                                {
            //                                    Node nodeParentAW = CWSService.getNodeByName(Convert.ToInt64(parentAWID), artworkNo.REQUEST_ITEM_NO);

            //                                    if (nodeParentAW != null)
            //                                    {
            //                                        Node nodeParentAWPO = CWSService.getNodeByName(nodeParentAW.ID, folderAWPOName);

            //                                        var newNode = CWSService.copyNode(iPOFile.Name, iPOFile.ID, nodeParentAWPO.ID, vendor.UPDATE_BY);
            //                                        string EXTENSION = Path.GetExtension(iPOFile.Name);
            //                                        string CONTENT_TYPE = newNode.VersionInfo.MimeType;

            //                                        EXTENSION = EXTENSION.Replace(".", "");

            //                                        CopyFileToAttachment(context, vendor, newNode, EXTENSION, CONTENT_TYPE);
            //                                    }
            //                                }
            //                            }

            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Results.status = "E";
            //    Results.msg = CNService.GetErrorMessage(ex);
            //}
        }

        private static void CopyFileToAttachmentXX(ARTWORKEntities context, ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2 vendor, Node nodeAW, string EXTENSION, string CONTENT_TYPE)
        {
            var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_VN_PO" }, context).FirstOrDefault().STEP_ARTWORK_ID;

            ART_WF_ARTWORK_ATTACHMENT attach = new ART_WF_ARTWORK_ATTACHMENT();
            attach.ARTWORK_REQUEST_ID = vendor.ARTWORK_REQUEST_ID;
            attach.ARTWORK_SUB_ID = vendor.ARTWORK_SUB_ID;
            attach.CONTENT_TYPE = CONTENT_TYPE;
            attach.CREATE_BY = vendor.CREATE_BY;
            attach.UPDATE_BY = vendor.CREATE_BY;
            attach.EXTENSION = EXTENSION;
            attach.FILE_NAME = nodeAW.Name;
            attach.IS_CUSTOMER = "";
            attach.IS_INTERNAL = "X";
            attach.IS_VENDOR = "X";
            attach.NODE_ID = nodeAW.ID;
            attach.SIZE = Convert.ToInt64(nodeAW.VersionInfo.FileDataSize);
            attach.STEP_ARTWORK_ID = stepId;
            attach.VERSION = 1;
            attach.VERSION2 = "1.0";
            //start ticket#438889  
          //  var temp = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(new ART_WF_ARTWORK_ATTACHMENT() { NODE_ID = nodeAW.ID, VERSION = 1 }, context).ToList();  //comment by aof
            var temp = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(new ART_WF_ARTWORK_ATTACHMENT() { NODE_ID = nodeAW.ID, VERSION2 = "1.0" }, context).ToList();  //rewrite by aof change VERSION->VERSION2
            //start ticket#438889  

            if (temp.Count == 0)
            {
                ART_WF_ARTWORK_ATTACHMENT_SERVICE.SaveOrUpdate(attach, context);
            }
        }

    }
}
