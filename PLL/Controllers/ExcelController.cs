using BLL.Helpers;
using BLL.Helpers.Master;
using BLL.Services;
 
using ClosedXML.Excel;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

using Syncfusion.XlsIO;
using System.Data.SqlClient;


namespace PLL.Controllers
{
    public class ExcelController : Controller
    {
        // GET: Excel
        public ActionResult Index()
        {
            return View();
        }

        //private void ConvertToExcel(DataTable dt, string fileName)
        //{
        //    // instantiate the GridView control from System.Web.UI.WebControls namespace
        //    // set the data source
        //    GridView gridview = new GridView();
        //    gridview.DataSource = dt;
        //    gridview.DataBind();

        //    Response.Clear();
        //    Response.AddHeader("content-disposition", "attachment;filename=" + fileName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xls");
        //    //Response.ContentType = "application/ms-excel";
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    Response.ContentEncoding = System.Text.Encoding.Unicode;
        //    Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

        //    using (StringWriter sw = new StringWriter())
        //    {
        //        using (HtmlTextWriter htw = new HtmlTextWriter(sw))
        //        {
        //            // render the GridView to the HtmlTextWriter
        //            gridview.RenderControl(htw);
        //            // Output the GridView content saved into StringWriter

        //            string style = @"<meta http-equiv='Content-Type' content='text/html;charset=UTF-8'> <style> td { white-space: nowrap; } </style>";
        //            Response.Write(style);
        //            Response.Output.Write(sw.ToString());
        //            Response.Flush();
        //            Response.End();
        //        }
        //    }
        //}

        public void ConvertToExcel(DataTable dt, string fileName, string sheetName)
        {
            // instantiate the GridView control from System.Web.UI.WebControls namespace
            // set the data source
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, sheetName);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx");


                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }
        public void ExportGridToExcel(DataTable dt, string fileName, string sheetName)
        {
            DataGrid dg = new DataGrid();
            dg.DataSource = dt;
            dg.DataBind();

            Response.Clear();
            Response.Buffer = true;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Charset = "";
            string FileName = fileName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xls";
            StringWriter strwritter = new StringWriter();
            HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);

            dg.GridLines = GridLines.Both;
            dg.HeaderStyle.Font.Bold = true;
            dg.RenderControl(htmltextwrtter);
            

            Response.Write(strwritter.ToString());
            Response.End();
        }
        public void ExportToExcel(DataTable dt, string fileName, string sheetName)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                //Initialize Application
                IApplication application = excelEngine.Excel;

                //Set the default application version as Excel 2016
                application.DefaultVersion = ExcelVersion.Excel2016;

                //Create a workbook with a worksheet
                IWorkbook workbook = application.Workbooks.Create(1);

                //Access first worksheet from the workbook instance
                IWorksheet worksheet = workbook.Worksheets[0];

                //Export data to Excel
                worksheet.ImportDataTable(dt, true, 1, 1);
                worksheet.UsedRange.AutofitColumns();

                //Save the workbook to disk in xlsx format
                workbook.SaveAs(@fileName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx");
                //workbook.SaveAs(@fileName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx", ExcelSaveType.SaveAsXLS, HttpContext.ApplicationInstance.Response, ExcelDownloadType.Open);
            }
        }
        public ActionResult TrackingReport([FromUri]TRACKING_REPORT_REQUEST param)
        {
            try
            {
                var allStepMockup = new List<ART_M_STEP_MOCKUP>();
                var allStepArtwork = new List<ART_M_STEP_ARTWORK>();

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        allStepMockup = ART_M_STEP_MOCKUP_SERVICE.GetAll(context);
                        allStepArtwork = ART_M_STEP_ARTWORK_SERVICE.GetAll(context);
                    }
                }

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
                    allMockupTrans = TrackingReportHelper.QueryTrackingReportMockup(param, true);

                if (!searchOnlyMockup)
                    allArtworkTrans = TrackingReportHelper.QueryTrackingReportArtwork(param, true);

                var msg = MessageHelper.GetMessage("MSG_005");

                allMockupTrans = allMockupTrans.OrderBy(m => m.MOCKUP_NO).ThenBy(m => m.CREATE_DATE_PROCESS).ToList();
                allArtworkTrans = allArtworkTrans.OrderBy(m => m.REQUEST_ITEM_NO).ThenBy(m => m.CREATE_DATE_PROCESS).ToList();

                List<TRACKING_REPORT> Result = new List<TRACKING_REPORT>();

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("Creator"));
                dt.Columns.Add(new DataColumn("Workflow number"));
                dt.Columns.Add(new DataColumn("Packaging type"));
                if (param.data.VIEW == "mk") dt.Columns.Add(new DataColumn("Primary type"));
                dt.Columns.Add(new DataColumn("WF status"));//WF Status
                dt.Columns.Add(new DataColumn("Current step"));//Current step
                dt.Columns.Add(new DataColumn("Current assign"));
                dt.Columns.Add(new DataColumn("Due date"));
                dt.Columns.Add(new DataColumn("Sold to"));
                dt.Columns.Add(new DataColumn("Ship to"));
                if (param.data.VIEW == "mk") dt.Columns.Add(new DataColumn("Route description"));
                dt.Columns.Add(new DataColumn("In transit to"));
                dt.Columns.Add(new DataColumn("SO no."));
                dt.Columns.Add(new DataColumn("SO create date"));
                dt.Columns.Add(new DataColumn("Brand"));
                dt.Columns.Add(new DataColumn("Additional brand"));
                dt.Columns.Add(new DataColumn("Product code"));
                dt.Columns.Add(new DataColumn("Prod./insp.memo"));
                dt.Columns.Add(new DataColumn("RD Reference no."));
                dt.Columns.Add(new DataColumn("RDD"));
                if (param.data.VIEW == "pg") dt.Columns.Add(new DataColumn("Vendor RFQ"));
                if (param.data.VIEW == "pg") dt.Columns.Add(new DataColumn("Selected vendor"));

                string currentAssign = "";
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        foreach (var item in allMockupTrans)
                        {
                            if (!string.IsNullOrEmpty(item.MOCKUP_NO))
                            {
                                DataRow dr = dt.NewRow();
                                dr["Creator"] = item.CREATE_BY_CHECK_LIST_TITLE + " " + item.CREATE_BY_CHECK_LIST_FIRST_NAME + " " + item.CREATE_BY_CHECK_LIST_LAST_NAME;
                                dr["Workflow number"] = item.MOCKUP_NO;
                                var tempMockup = allStepMockup.Where(m => m.STEP_MOCKUP_ID == item.CURRENT_STEP_ID).FirstOrDefault();
                                if (tempMockup != null)
                                {
                                    dr["Current step"] = tempMockup.STEP_MOCKUP_NAME;
                                }

                                currentAssign = "";
                                if (item.CURRENT_USER_ID == null)
                                {
                                    currentAssign = msg;
                                }
                                else
                                {
                                    currentAssign = CNService.GetUserName(item.CURRENT_USER_ID, context);
                                }

                                dr["Current assign"] = currentAssign;

                                dr["Brand"] = item.BRAND_DISPLAY_TXT;
                                dr["Sold to"] = item.SOLD_TO_DISPLAY_TXT;
                                dr["Ship to"] = item.SHIP_TO_DISPLAY_TXT;

                                if (param.data.VIEW == "mk")
                                {
                                    if (item.PRIMARY_TYPE_ID > 0)
                                    {
                                        var PRIMARY_TYPE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PRIMARY_TYPE_ID, context);
                                        if (PRIMARY_TYPE != null)
                                            dr["Primary type"] = PRIMARY_TYPE.DESCRIPTION;
                                    }
                                }

                                if (item.CREATE_DATE_PROCESS != null)
                                {
                                    DateTime? dtReceiveWf = item.CREATE_DATE_PROCESS;
                                    DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION.Value));

                                    if (!String.IsNullOrEmpty(item.IS_STEP_DURATION_EXTEND) && item.IS_STEP_DURATION_EXTEND.Equals("X"))
                                    {
                                        dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION_EXTEND.Value));
                                    }

                                    dr["Due date"] = CNService.GetWorkingDays(DateTime.Now, dtWillFinish.Value) + " (" + dtWillFinish.Value.ToString("dd/MM/yyyy HH:mm") + ")";
                                }
                                if (param.data.VIEW == "pg") dr["Vendor RFQ"] = CNService.GetVendorRFQ(item.MOCKUP_ID, item.MOCKUP_SUB_ID, context).Replace("<br/>", ", ").Replace("<br />", ", ");
                                if (param.data.VIEW == "pg") dr["Selected vendor"] = CNService.GetVendorSelected(item.MOCKUP_ID, item.MOCKUP_SUB_ID, context);
                                if (item.IS_END == "X")
                                {
                                    if (item.IS_TERMINATE == "X")
                                    {
                                        dr["WF status"] = "Terminated";
                                    }
                                    else
                                    {
                                        dr["WF status"] = "Completed";
                                    }

                                    dr["Current step"] = "";
                                    dr["Current assign"] = "";
                                    dr["Due date"] = "";
                                }
                                else
                                {
                                    dr["WF status"] = "In progress";
                                }

                                var PACKING_TYPE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PACKING_TYPE_ID, context);
                                if (PACKING_TYPE != null)
                                    dr["Packaging type"] = PACKING_TYPE.VALUE + ":" + PACKING_TYPE.DESCRIPTION;

                                if (item.REQUEST_DELIVERY_DATE != null)
                                    dr["RDD"] = item.REQUEST_DELIVERY_DATE.Value.ToString("dd/MM/yyyy");

                                var tempProduct = MapperServices.ART_WF_MOCKUP_CHECK_LIST_PRODUCT(ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_PRODUCT() { CHECK_LIST_ID = item.CHECK_LIST_ID }, context));
                                dr["Product code"] = tempProduct.Aggregate("", (a, b) => a + ((a.Length > 0 && b.PRODUCT_CODE != null && b.PRODUCT_CODE.Length > 0) ? ", " : "") + b.PRODUCT_CODE);

                                var tempRD = MapperServices.ART_WF_MOCKUP_CHECK_LIST_REFERENCE(ART_WF_MOCKUP_CHECK_LIST_REFERENCE_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_REFERENCE() { CHECK_LIST_ID = item.CHECK_LIST_ID }, context));
                                dr["RD Reference no."] = tempRD.Aggregate("", (a, b) => a + ((a.Length > 0 && b.REFERENCE_NO != null && b.REFERENCE_NO.Length > 0) ? ", " : "") + b.REFERENCE_NO);

                                dt.Rows.Add(dr);
                            }
                        }

                        var tempARTWORK_ITEM_ID = allArtworkTrans.Select(m => m.ARTWORK_ITEM_ID);
                        var tempARTWORK_SUB_ID = allArtworkTrans.Select(m => m.ARTWORK_SUB_ID);
                        var listAssignedSOHeader = (from p in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
                                                    where tempARTWORK_SUB_ID.Contains(p.ARTWORK_SUB_ID)
                                                    select p).ToList();
                        var listAssignedSOItem = (from p in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM
                                                  where tempARTWORK_SUB_ID.Contains(p.ARTWORK_SUB_ID)
                                                  select p).ToList();
                        foreach (var item in allArtworkTrans)
                        {
                            if (!string.IsNullOrEmpty(item.REQUEST_ITEM_NO))
                            {
                                DataRow dr = dt.NewRow();
                                dr["Creator"] = dr["Creator"] = item.CREATE_BY_ARTWORK_REQUEST_TITLE + " " + item.CREATE_BY_ARTWORK_REQUEST_FIRST_NAME + " " + item.CREATE_BY_ARTWORK_REQUEST_LAST_NAME;
                                dr["Workflow number"] = item.REQUEST_ITEM_NO;
                                var tempMockup = allStepArtwork.Where(m => m.STEP_ARTWORK_ID == item.CURRENT_STEP_ID).FirstOrDefault();
                                if (tempMockup != null)
                                    dr["Current step"] = tempMockup.STEP_ARTWORK_NAME;

                                currentAssign = "";
                                if (item.CURRENT_USER_ID == null)
                                {
                                    currentAssign = msg;
                                }
                                else
                                {
                                    currentAssign = CNService.GetUserName(item.CURRENT_USER_ID, context);
                                }

                                dr["Current assign"] = currentAssign;

                                dr["Brand"] = item.BRAND_DISPLAY_TXT;
                                dr["Sold to"] = item.SOLD_TO_DISPLAY_TXT;
                                dr["Ship to"] = item.SHIP_TO_DISPLAY_TXT;

                                if (param.data.VIEW == "mk")
                                {
                                    if (item.PRIMARY_TYPE_ID > 0)
                                    {
                                        var PRIMARY_TYPE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PRIMARY_TYPE_ID, context);
                                        if (PRIMARY_TYPE != null)
                                            dr["Primary type"] = PRIMARY_TYPE.DESCRIPTION;
                                    }
                                }

                                if (item.CREATE_DATE_PROCESS != null)
                                {
                                    DateTime? dtReceiveWf = item.CREATE_DATE_PROCESS;
                                    DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION.Value));

                                    if (!String.IsNullOrEmpty(item.IS_STEP_DURATION_EXTEND) && item.IS_STEP_DURATION_EXTEND.Equals("X"))
                                    {
                                        dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION_EXTEND.Value));
                                    }

                                    dr["Due date"] = CNService.GetWorkingDays(DateTime.Now, dtWillFinish.Value) + " (" + dtWillFinish.Value.ToString("dd/MM/yyyy HH:mm") + ")";
                                }
                                //dr["Vendor RFQ"] = CNService.GetVendorRFQ(item.MOCKUP_ID, item.MOCKUP_SUB_ID).Replace("<br/>", ", ").Replace("<br />", ", ");
                                //dr["Selected vendor"] = CNService.GetVendorSelected(item.MOCKUP_ID, item.MOCKUP_SUB_ID);
                                if (item.IS_END == "X")
                                {
                                    if (item.IS_TERMINATE == "X")
                                    {
                                        dr["WF status"] = "Terminated";
                                    }
                                    else
                                    {
                                        dr["WF status"] = "Completed";
                                    }

                                    dr["Current step"] = "";
                                    dr["Current assign"] = "";
                                    dr["Due date"] = "";
                                }
                                else
                                {
                                    dr["WF status"] = "In progress";
                                }

                                if (item.REQUEST_DELIVERY_DATE != null)
                                    dr["RDD"] = item.REQUEST_DELIVERY_DATE.Value.ToString("dd/MM/yyyy");


                                var tempProduct = MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCT(ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_PRODUCT() { ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID }, context));
                                foreach (var itemProduct in tempProduct)
                                {
                                    var temp = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(itemProduct.PRODUCT_CODE_ID, context);
                                    if (temp != null)
                                        if (string.IsNullOrEmpty(dr["Product code"].ToString()))
                                            dr["Product code"] = temp.PRODUCT_CODE;
                                        else
                                            dr["Product code"] += ", " + temp.PRODUCT_CODE;
                                }


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



                                var tempRD = MapperServices.ART_WF_ARTWORK_REQUEST_REFERENCE(ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_REFERENCE() { ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID }, context));
                                dr["RD Reference no."] = tempRD.Aggregate("", (a, b) => a + ((a.Length > 0 && b.REFERENCE_NO != null && b.REFERENCE_NO.Length > 0) ? ", " : "") + b.REFERENCE_NO);

                                var tempListAssignedSOHeader = listAssignedSOHeader.Where(m => m.ARTWORK_SUB_ID == item.ARTWORK_SUB_ID).ToList();
                                var tempListAssignedSOItem = listAssignedSOItem.Where(m => m.ARTWORK_SUB_ID == item.ARTWORK_SUB_ID).ToList();
                                dr["In transit to"] = CNService.ConcatArray(tempListAssignedSOItem.Select(m => m.IN_TRANSIT_TO).ToArray());


                                if (listAssignSO != null && listAssignSO.Count > 0)
                                {
                                    dr["SO no."] = CNService.ConcatArray(listAssignSO.ToArray());
                                }
                                else
                                {
                                    dr["SO no."] = CNService.ConcatArray(tempListAssignedSOHeader.Select(m => m.SALES_ORDER_NO).ToArray());
                                }

                                dr["SO create date"] = CNService.ConcatArray(tempListAssignedSOHeader.Select(m => m.CREATE_ON).Distinct().ToArray());
                                //  dr["SO item"] = CNService.ConcatArray(tempListAssignedSOItem.Select(m => m.ITEM).ToArray());
                                dr["Additional brand"] = CNService.ConcatArray(tempListAssignedSOItem.Select(m => m.ADDITIONAL_BRAND_DESCRIPTION).ToArray());
                                dr["Prod./insp.memo"] = CNService.ConcatArray(tempListAssignedSOItem.Select(m => m.PROD_INSP_MEMO).Distinct().ToArray());
                                if (param.data.VIEW == "mk") dr["Route description"] = CNService.ConcatArray(tempListAssignedSOItem.Select(m => m.PORT).ToArray());

                                var processPA = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PA() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                if (processPA != null)
                                {
                                    var PACKING_TYPE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(processPA.MATERIAL_GROUP_ID, context);
                                    if (PACKING_TYPE != null)
                                        dr["Packaging type"] = PACKING_TYPE.VALUE + ":" + PACKING_TYPE.DESCRIPTION;
                                }

                                dr["Packaging type"] = dupValue(dt, dr, "Packaging type", item.REQUEST_ITEM_NO);
                                dr["In transit to"] = dupValue(dt, dr, "In transit to", item.REQUEST_ITEM_NO);
                                dr["SO no."] = dupValue(dt, dr, "SO no.", item.REQUEST_ITEM_NO);
                                dr["SO create date"] = dupValue(dt, dr, "SO create date", item.REQUEST_ITEM_NO);
                                // dr["SO item"] = dupValue(dt, dr, "SO item", item.REQUEST_ITEM_NO);
                                dr["Additional brand"] = dupValue(dt, dr, "Additional brand", item.REQUEST_ITEM_NO);

                                dr["Prod./insp.memo"] = dupValue(dt, dr, "Prod./insp.memo", item.REQUEST_ITEM_NO);
                                if (param.data.VIEW == "mk") dr["Route description"] = dupValue(dt, dr, "Route description", item.REQUEST_ITEM_NO);

                                dt.Rows.Add(dr);
                            }
                        }
                    }
                }

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "Tracking_report", "Tracking_report");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }


        public ActionResult IGridHistorySummarizeOwnerReport([FromUri]KPILog_Summarize_REPORT_REQUEST param)
        {
            try
            {

               
                var list = HistoryReportHelper.GetKPISumerizeOwnereport(param);

                DataTable dt = new DataTable();

                if (param.data.LayOut == "PA")
                {
                    dt.Columns.Add(new DataColumn("PA"));
                    dt.Columns.Add(new DataColumn("PA FullName"));
                    dt.Columns.Add(new DataColumn("Modify By"));
                    dt.Columns.Add(new DataColumn("Modify By FullName"));
                    dt.Columns.Add(new DataColumn("No. of modified records"));
                    dt.Columns.Add(new DataColumn("No. of all created records"));
                    dt.Columns.Add(new DataColumn("Percentage of error"));

                }
                else
                {
                    dt.Columns.Add(new DataColumn("PG"));
                    dt.Columns.Add(new DataColumn("PG FullName"));
                    dt.Columns.Add(new DataColumn("Modify By"));
                    dt.Columns.Add(new DataColumn("Modify By FullName"));
                    dt.Columns.Add(new DataColumn("No. of modified records"));
                    dt.Columns.Add(new DataColumn("No. of all assigned records"));
                    dt.Columns.Add(new DataColumn("Percentage of error"));

                }


                var msg = MessageHelper.GetMessage("MSG_005");
                DataRow dr;
                list.data.ForEach(item =>
                {
                    dr = dt.NewRow();

                    if (param.data.LayOut == "PA")
                    {
                        dr[0] ="PA FullName : ";
                        dr[1] = item.CREATE_FULLNAME; 
                    }
                    else
                    {
                        dr[0] = "PG FullName : ";
                        dr[1] = item.CREATE_FULLNAME;
                    }
                    dt.Rows.Add(dr);
                    //----------------------------------
                    if (item.data != null && item.data.Count() > 0)
                    {
                        foreach (var v in item.data)
                        {
                            dr = dt.NewRow();
                            dr[0] = v.CREATE_USERNAME ;
                            dr[1] = v.CREATE_FULLNAME;
                            dr[2] = v.MODIFY_BY_USERNAME;
                            dr[3] = v.MODIFY_BY_FULLNAME;
                            dr[4] = v.Count;
                            dr[5] = v.SAPMat_Count;
                            dr[6] = "";
                            dt.Rows.Add(dr);
                        }
                    }
                    dr = dt.NewRow();
                    dr[0] = item.CREATE_USERNAME;
                    dr[1] = item.CREATE_FULLNAME;
                    dr[2] = item.MODIFY_BY_USERNAME;
                    dr[3] = item.MODIFY_BY_FULLNAME;
                    dr[4] = item.MODIFYED_RECOORD;
                    dr[5] = item.CREATED_RECOORD;
                    dr[6] = item.PECENTAGE_ERROR;
                    dt.Rows.Add(dr);

                });

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "IGrid_Summarize_By_Owner_Level", "IGrid_Summarize_By_Owner_Level");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }



        public ActionResult IGridHistorySummarizeApproveReport([FromUri]KPILog_Summarize_REPORT_REQUEST param)
        {
            try
            {


                var list = HistoryReportHelper.GetKPISumerizeApproveReport(param);

                DataTable dt = new DataTable();


                dt.Columns.Add(new DataColumn("Modify By"));
                dt.Columns.Add(new DataColumn("Modify By FullName"));

                if (param.data.LayOut == "PA_SUMApprove")
                {
                    dt.Columns.Add(new DataColumn("PA"));
                    dt.Columns.Add(new DataColumn("PA FullName"));
                }
                else
                {
                    dt.Columns.Add(new DataColumn("PG"));
                    dt.Columns.Add(new DataColumn("PG FullName"));
                }
             
                dt.Columns.Add(new DataColumn("No. of modified records"));
             

                var msg = MessageHelper.GetMessage("MSG_005");
                DataRow dr;
                list.data.ForEach(item =>
                {
       
                  
                    dr = dt.NewRow();
                    dr[0] = item.MODIFY_BY_USERNAME;
                    dr[1] = item.MODIFY_BY_FULLNAME;
                    dr[2] = "";
                    dr[3] = "";
                    dr[4] = "";
                    dt.Rows.Add(dr);

                    if (item.data != null && item.data.Count() > 0)
                    {
                        foreach (var v in item.data)
                        {
                            dr = dt.NewRow();
                            dr[0] = v.MODIFY_BY_USERNAME;
                            dr[1] = v.MODIFY_BY_FULLNAME;
                            dr[2] = v.CREATE_USERNAME;
                            dr[3] = v.CREATE_FULLNAME;
                            dr[4] = v.Count;

                            dt.Rows.Add(dr);
                        }
                    }

                });

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "IGrid_Summarize_By_Approval_Level", "Summarize_By_Approval");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }


        public ActionResult IGridSummaryAllReport([FromUri]IGridSummary_REPORT_REQUEST param)
        {
            try
            {


                var list = IGridSummaryReportHelper.GetIGridSummaryGroupReport(param);

                DataTable dt = new DataTable();

                dt.Columns.Add(new DataColumn("Id")); //0
                dt.Columns.Add(new DataColumn("Condition"));  //1
                dt.Columns.Add(new DataColumn("RequestType"));
                dt.Columns.Add(new DataColumn("DocumentNo"));  //3
                dt.Columns.Add(new DataColumn("DMS No./Artwork"));
                dt.Columns.Add(new DataColumn("Material No."));
                dt.Columns.Add(new DataColumn("Description"));
                dt.Columns.Add(new DataColumn("Group"));
                dt.Columns.Add(new DataColumn("Brand"));
                dt.Columns.Add(new DataColumn("Role"));
                dt.Columns.Add(new DataColumn("CreateOn"));
                dt.Columns.Add(new DataColumn("ActiveBy"));
                dt.Columns.Add(new DataColumn("StatusApp"));  //12




                var msg = MessageHelper.GetMessage("MSG_005");
                DataRow dr;
                list.data.ForEach(item =>
                {


                    dr = dt.NewRow();
                    dr[0] = "";
                    dr[1] = item.Condition;
                    dr[2] = "";
                    dr[3] = item.DocumentNo;
                    dr[4] = "";
                    dt.Rows.Add(dr);

                    if (item.data != null && item.data.Count() > 0)
                    {
                        foreach (var v in item.data)
                        {
                            dr = dt.NewRow();
                            dr[0] = v.Id;
                            dr[1] = v.Condition;
                            dr[2] = v.RequestType;
                            dr[3] = v.DocumentNo;
                            dr[4] = v.DMSNo;
                            dr[5] = v.Material;
                            dr[6] = v.Description;
                            dr[7] = v.MaterialGroup;
                            dr[8] = v.Brand;
                            dr[9] = v.fn;
                            dr[10] = v.Submitdate;
                            dr[11] = v.ActiveBy;
                            dr[12] = v.StatusApp;

                            dt.Rows.Add(dr);
                        }
                    }

                });

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "IGrid_Summary_Report", "IGrid_Summary_Report");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }



        public ActionResult TrackingNewReportV3([FromUri]TU_TRACKING_WF_REPORT_MODEL_REQUEST param)
        {
            try
            {
                var list = TrackingReportHelper.GetViewTrackingReportV3(param);

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("Creator"));
                dt.Columns.Add(new DataColumn("WF no."));
                dt.Columns.Add(new DataColumn("Packaging type"));
                if (param.data.VIEW == "mk") dt.Columns.Add(new DataColumn("Primary type"));
                dt.Columns.Add(new DataColumn("WF status"));//WF Status
                dt.Columns.Add(new DataColumn("Current step"));//Current step
                dt.Columns.Add(new DataColumn("Current assign"));
                dt.Columns.Add(new DataColumn("Current duration"));
                dt.Columns.Add(new DataColumn("Current due date"));
                dt.Columns.Add(new DataColumn("Sold to"));
                dt.Columns.Add(new DataColumn("Ship to"));
                if (param.data.VIEW == "mk") dt.Columns.Add(new DataColumn("Route description"));
                dt.Columns.Add(new DataColumn("In transit to"));
                dt.Columns.Add(new DataColumn("SO no."));
                dt.Columns.Add(new DataColumn("SO create date"));
                dt.Columns.Add(new DataColumn("Brand"));
                dt.Columns.Add(new DataColumn("Additional brand"));
                dt.Columns.Add(new DataColumn("Product code"));
                dt.Columns.Add(new DataColumn("Prod./insp. memo"));
                dt.Columns.Add(new DataColumn("RD reference no."));
                dt.Columns.Add(new DataColumn("RDD"));
                if (param.data.VIEW == "pg") dt.Columns.Add(new DataColumn("Vendor RFQ"));
                if (param.data.VIEW == "pg") dt.Columns.Add(new DataColumn("Selected vendor"));

                var msg = MessageHelper.GetMessage("MSG_005");

                list.data.ForEach(item =>
                {
                    DataRow dr = dt.NewRow();
                    dr["Creator"] = item.CREATOR_NAME;
                    dr["WF no."] = item.WF_NO.Trim();
                    dr["Packaging type"] = item.PACKAGING_TYPE;
                    if (param.data.VIEW == "mk") dr["Primary type"] = item.PRIMARY_TYPE_TXT;
                    dr["WF status"] = item.WF_STATUS;
                    dr["Current step"] = item.CURRENT_STEP == null ? "" : item.CURRENT_STEP.Trim();
                    dr["Current assign"] = item.CURRENT_ASSING;
                    dr["Current duration"] = item.CURRENT_DURATION ;
                    dr["Current due date"] = item.CURRENT_DUE_DATE;
                    dr["Sold to"] = item.SOLD_TO;
                    dr["Ship to"] = item.SHIP_TO;
                    if (param.data.VIEW == "mk") dr["Route description"] = item.PORT;
                    dr["In transit to"] = item.IN_TRANSIT_TO;
                    dr["So no."] = item.SO_NO;
                    dr["SO create date"] = item.SO_CREATE_DATE;
                    dr["Brand"] = item.BRAND_NAME;
                    dr["Additional brand"] = item.ADDITIONAL_BRAND;
                    dr["Product code"] = item.PRODUCT_CODE;
                    dr["Prod./insp. memo"] = item.PROD_INSP_MEMO;
                    dr["RD reference no."] = item.REFERENCE_NO;
                    dr["RDD"] = item.RDD;
                    if (param.data.VIEW == "pg") dr["Vendor RFQ"] = item.VENDOR_RFQ;
                    if (param.data.VIEW == "pg") dr["Selected vendor"] = item.SELECTED_VENDOR;
                    dt.Rows.Add(dr);
                });

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "Tracking_report", "Tracking_report");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }



        public ActionResult TrackingNewReport([FromUri]V_ART_ENDTOEND_REPORT_REQUEST param)
        {
            try
            {
                var list = TrackingReportHelper.GetViewTrackingReport(param);

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("Creator"));
                dt.Columns.Add(new DataColumn("WF no."));
                dt.Columns.Add(new DataColumn("Packaging type"));
                if (param.data.VIEW == "mk") dt.Columns.Add(new DataColumn("Primary type"));
                dt.Columns.Add(new DataColumn("WF status"));//WF Status
                dt.Columns.Add(new DataColumn("Current step"));//Current step
                dt.Columns.Add(new DataColumn("Current assign"));
                dt.Columns.Add(new DataColumn("Current duration"));
                dt.Columns.Add(new DataColumn("Current due date"));
                dt.Columns.Add(new DataColumn("Sold to"));
                dt.Columns.Add(new DataColumn("Ship to"));
                if (param.data.VIEW == "mk") dt.Columns.Add(new DataColumn("Route description"));
                dt.Columns.Add(new DataColumn("In transit to"));
                dt.Columns.Add(new DataColumn("SO no."));
                dt.Columns.Add(new DataColumn("SO create date"));
                dt.Columns.Add(new DataColumn("Brand"));
                dt.Columns.Add(new DataColumn("Additional brand"));
                dt.Columns.Add(new DataColumn("Product code"));
                dt.Columns.Add(new DataColumn("Prod./insp. memo"));
                dt.Columns.Add(new DataColumn("RD reference no."));
                dt.Columns.Add(new DataColumn("RDD"));
                if (param.data.VIEW == "pg") dt.Columns.Add(new DataColumn("Vendor RFQ"));
                if (param.data.VIEW == "pg") dt.Columns.Add(new DataColumn("Selected vendor"));

                var msg = MessageHelper.GetMessage("MSG_005");

                list.dataExcel.ForEach(item =>
                {
                    DataRow dr = dt.NewRow();
                    dr["Creator"] = item.CREATOR_NAME;
                    dr["WF no."] = item.WF_NO.Trim();
                    dr["Packaging type"] = item.PACKAGING_TYPE;
                    if (param.data.VIEW == "mk") dr["Primary type"] = item.PRIMARY_TYPE_TXT;
                    dr["WF status"] = item.WF_STAUTS;
                    dr["Current step"] = item.CURRENT_STEP_NAME_1 == null ? "" : item.CURRENT_STEP_NAME_1.Trim();
                    dr["Current assign"] = item.CURRENT_USER_NAME_1;
                    dr["Current duration"] = item.DURATION_1;
                    dr["Current due date"] = item.DUE_DATE_1;
                    dr["Sold to"] = item.SOLD_TO;
                    dr["Ship to"] = item.SHIP_TO;
                    if (param.data.VIEW == "mk") dr["Route description"] = item.PORT;
                    dr["In transit to"] = item.IN_TRANSIT_TO;
                    dr["So no."] = item.SALES_ORDER_NO;
                    dr["SO create date"] = item.CREATE_ON;
                    dr["Brand"] = item.BRAND_NAME;
                    dr["Additional brand"] = item.ADDITIONAL_BRAND_NAME;
                    dr["Product code"] = item.PRODUCT_CODE;
                    dr["Prod./insp. memo"] = item.PROD_INSP_MEMO;
                    dr["RD reference no."] = item.REFERENCE_NO;
                    dr["RDD"] = item.RDD;
                    if (param.data.VIEW == "pg") dr["Vendor RFQ"] = item.VENDOR_RFQ;
                    if (param.data.VIEW == "pg") dr["Selected vendor"] = item.SELECTED_VENDOR;
                    dt.Rows.Add(dr);
                });

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "Tracking_report", "Tracking_report");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        private string dupValue(DataTable dt, DataRow dr, string colname, string REQUEST_ITEM_NO)
        {
            var str = "";
            if (string.IsNullOrEmpty(dr[colname].ToString()))
            {
                foreach (DataRow drr in dt.Rows)
                {
                    if (drr["Workflow number"].ToString() == REQUEST_ITEM_NO)
                    {
                        str = drr[colname].ToString();
                        break;
                    }
                }
            }
            else
            {
                str = dr[colname].ToString();
            }
            return str;
        }

        public ActionResult OutstandingReport([FromUri]V_ART_ASSIGNED_SO_REQUEST param)
        {
            try
            {
                var list = OutstandingReportHelper.GetOutstandingReport(param);

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("SO no."));
                dt.Columns.Add(new DataColumn("Item"));
                dt.Columns.Add(new DataColumn("Material"));
                dt.Columns.Add(new DataColumn("Description"));
                dt.Columns.Add(new DataColumn("SO Qty"));
                dt.Columns.Add(new DataColumn("SU"));
                dt.Columns.Add(new DataColumn("RJ"));
                dt.Columns.Add(new DataColumn("Plant"));
                dt.Columns.Add(new DataColumn("BOM item"));
                dt.Columns.Add(new DataColumn("Component"));
                dt.Columns.Add(new DataColumn("Component description"));
                dt.Columns.Add(new DataColumn("New BOM item"));
                dt.Columns.Add(new DataColumn("Quantity"));
                dt.Columns.Add(new DataColumn("Current WF status"));
                dt.Columns.Add(new DataColumn("WF no."));
                dt.Columns.Add(new DataColumn("Flag for send to PP"));
                dt.Columns.Add(new DataColumn("Sent to PP"));
                dt.Columns.Add(new DataColumn("PO created"));
                dt.Columns.Add(new DataColumn("Stock"));
                dt.Columns.Add(new DataColumn("Created on"));
                dt.Columns.Add(new DataColumn("RDD"));
                dt.Columns.Add(new DataColumn("Sold to name"));
                dt.Columns.Add(new DataColumn("Ship to name"));
                dt.Columns.Add(new DataColumn("PIC"));

                foreach (var item in list.data)
                {
                    DataRow dr = dt.NewRow();
                    dr["SO no."] = item.SALES_ORDER_NO;
                    dr["Item"] = item.ITEM;
                    dr["Material"] = item.PRODUCT_CODE;
                    dr["Description"] = item.MATERIAL_DESCRIPTION;
                    dr["SO Qty"] = item.ORDER_QTY;
                    dr["SU"] = item.ORDER_UNIT;
                    dr["RJ"] = item.REJECTION_CODE;
                    dr["Plant"] = item.PRODUCTION_PLANT;
                    dr["BOM item"] = item.COMPONENT_ITEM;
                    dr["Component"] = item.COMPONENT_MATERIAL;
                    dr["Component description"] = item.DECRIPTION;
                    dr["Quantity"] = item.QUANTITY_DISPLAY_TXT;
                    dr["Current WF status"] = item.CURRENT_WF_STATUS;
                    dr["WF no."] = item.REQUEST_ITEM_NO;
                    dr["Flag for send to PP"] = item.READY_CREATE_PO_DISPLAY_TXT;
                    dr["Sent to PP"] = item.SEND_TO_PP_DISPLAY_TXT;
                    dr["PO created"] = item.PO_CREATED_DISPLAY_TXT;
                    dr["Stock"] = item.STOCK_DISPLAY_TXT;
                    dr["Created on"] = item.CREATE_ON;
                    dr["RDD"] = item.RDD;
                    dr["Sold to name"] = item.SOLD_TO_DISPLAY_TXT;
                    dr["Ship to name"] = item.SHIP_TO_DISPLAY_TXT;
                    dr["PIC"] = item.PIC_DISPLAY_TXT;
                    dr["New BOM item"] = item.BOM_ITEM_CUSTOM_1;
                    dt.Rows.Add(dr);
                }

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "Outstanding_report", "Outstanding_report");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        public ActionResult EndToEndReport([FromUri]TRACKING_REPORT_REQUEST param)
        {
            try
            {
                var list = EndToEndReportHelper.GetEndToEndReport(param);

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("WF no."));
                dt.Columns.Add(new DataColumn("Packaging type"));
                dt.Columns.Add(new DataColumn("WF Status"));
                dt.Columns.Add(new DataColumn("Order Bom Component"));
                dt.Columns.Add(new DataColumn("Current step"));
                dt.Columns.Add(new DataColumn("Current assign"));
                dt.Columns.Add(new DataColumn("Due Date"));
                dt.Columns.Add(new DataColumn("So no."));
                dt.Columns.Add(new DataColumn("Brand"));
                dt.Columns.Add(new DataColumn("Additional Brand"));
                dt.Columns.Add(new DataColumn("Product code"));
                dt.Columns.Add(new DataColumn("Prod./Insp. memo"));
                dt.Columns.Add(new DataColumn("RD Reference no."));
                dt.Columns.Add(new DataColumn("Production Plant"));
                dt.Columns.Add(new DataColumn("Sold to"));
                dt.Columns.Add(new DataColumn("Ship to"));
                dt.Columns.Add(new DataColumn("Country"));
                dt.Columns.Add(new DataColumn("Port"));
                dt.Columns.Add(new DataColumn("In transit to"));
                dt.Columns.Add(new DataColumn("SO create date"));
                dt.Columns.Add(new DataColumn("RDD"));
                dt.Columns.Add(new DataColumn("Request form/checklist form no."));
                dt.Columns.Add(new DataColumn("PA Owner"));
                dt.Columns.Add(new DataColumn("PG Owner"));
                dt.Columns.Add(new DataColumn("Step"));
                dt.Columns.Add(new DataColumn("Start Date"));
                dt.Columns.Add(new DataColumn("End Date"));
                dt.Columns.Add(new DataColumn("Step Duration(days)"));
                dt.Columns.Add(new DataColumn("Reason Code"));
                dt.Columns.Add(new DataColumn("Total(days)"));
                dt.Columns.Add(new DataColumn("Marketing"));
                dt.Columns.Add(new DataColumn("Project name"));
                dt.Columns.Add(new DataColumn("Creator"));

                var msg = MessageHelper.GetMessage("MSG_005");

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var allStepMockup = (from a in context.ART_M_STEP_MOCKUP
                                             select a).ToList();

                        foreach (var item in list.data)
                        {
                            if (item.WORKFLOW_NUMBER.StartsWith("M"))
                            {
                                var requestItem = context.ART_WF_MOCKUP_CHECK_LIST_ITEM
                                       .Where(r => r.MOCKUP_NO == item.WORKFLOW_NUMBER)
                                       .Select(s => s.MOCKUP_ID)
                                       .FirstOrDefault();

                                var processesMO = (from p in context.ART_WF_MOCKUP_PROCESS
                                                   where p.MOCKUP_ID == requestItem
                                                   select p).OrderBy(o => o.CREATE_DATE).ToList();
                                if (processesMO != null && processesMO.Count > 0)
                                {
                                    int total_day = 0;
                                    string currentAssignMO = "";
                                    decimal currentDuration = 0;
                                    string currentStepDisplay = "";
                                    string currentStepUserDisplay = "";
                                    string currentDurationDisplay = "";

                                    foreach (ART_WF_MOCKUP_PROCESS iProcessMO in processesMO)
                                    {
                                        if (iProcessMO.CURRENT_USER_ID == null)
                                        {
                                            currentAssignMO = msg;
                                        }
                                        else
                                        {
                                            currentAssignMO = CNService.GetUserName(iProcessMO.CURRENT_USER_ID, context);
                                        }

                                        var stepTmp = allStepMockup.Where(w => w.STEP_MOCKUP_ID == iProcessMO.CURRENT_STEP_ID).FirstOrDefault();

                                        currentStepDisplay = "";
                                        currentStepUserDisplay = "";
                                        currentDurationDisplay = "";

                                        DateTime? dtReceiveWf = iProcessMO.CREATE_DATE;
                                        int stepID = Convert.ToInt32(iProcessMO.CURRENT_STEP_ID);

                                        int? stepDuration = null;
                                        if (stepTmp.DURATION != null)
                                        {
                                            stepDuration = Convert.ToInt32(stepTmp.DURATION);
                                        }

                                        if (!String.IsNullOrEmpty(iProcessMO.IS_STEP_DURATION_EXTEND) && iProcessMO.IS_STEP_DURATION_EXTEND.Equals("X"))
                                        {
                                            stepDuration = Convert.ToInt32(stepTmp.DURATION_EXTEND);
                                        }

                                        if (string.IsNullOrEmpty(iProcessMO.IS_END))
                                        {
                                            currentStepDisplay = (from c in allStepMockup
                                                                  where c.STEP_MOCKUP_ID == iProcessMO.CURRENT_STEP_ID
                                                                  select c.STEP_MOCKUP_NAME).FirstOrDefault();

                                            if (CNService.GetUserName(iProcessMO.CURRENT_USER_ID, context) != "")
                                            {
                                                currentStepUserDisplay = CNService.GetUserName(iProcessMO.CURRENT_USER_ID, context);
                                            }
                                            else
                                            {
                                                currentStepUserDisplay = msg;
                                            }

                                            currentDuration = Convert.ToDecimal(stepDuration);

                                            dtReceiveWf = iProcessMO.CREATE_DATE;
                                            DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(currentDuration));
                                            currentDurationDisplay = CNService.GetWorkingDays(DateTime.Now, dtWillFinish.Value) + " (" + dtWillFinish.Value.ToString("dd/MM/yyyy HH:mm") + ")";
                                        }

                                        DataRow dr = dt.NewRow();
                                        dr["WF no."] = item.WORKFLOW_NUMBER;
                                        dr["Packaging type"] = item.PACKING_TYPE_DISPLAY_TXT;
                                        dr["WF Status"] = item.CURRENT_STEP_DISPLAY_TXT;
                                        dr["Order Bom Component"] = item.SALES_ORDER_ITEM_COMPONENT;
                                        dr["Current step"] = currentStepDisplay; //item.CURRENT_STATUS_DISPLAY_TXT;
                                        dr["Current assign"] = currentStepUserDisplay; //item.CURRENT_ASSIGN_DISPLAY_TXT;
                                        dr["Due Date"] = currentDurationDisplay; //item.DUEDATE_DISPLAY_TXT;
                                        dr["So no."] = item.SALES_ORDER_NO;
                                        dr["Brand"] = item.BRAND_DISPLAY_TXT;
                                        dr["Additional Brand"] = item.ADDITIONAL_BRAND_DISPLAY_TXT;
                                        dr["Product code"] = item.PRODUCT_CODE_DISPLAY_TXT;
                                        dr["Prod./Insp. memo"] = item.PRODUCTION_MEMO_DISPLAY_TXT;
                                        dr["RD Reference no."] = item.RD_NUMBER_DISPLAY_TXT;
                                        dr["Production Plant"] = item.PLANT;
                                        dr["Sold to"] = item.SOLD_TO_DISPLAY_TXT;
                                        dr["Ship to"] = item.SHIP_TO_DISPLAY_TXT;
                                        dr["Country"] = item.COUNTRY;
                                        dr["Port"] = item.PORT;
                                        dr["In transit to"] = item.IN_TRANSIT_TO_DISPLAY_TXT;
                                        dr["SO create date"] = item.SALES_ORDER_CREATE_DATE;
                                        dr["RDD"] = item.RDD_DISPLAY_TXT;

                                        dr["Request form/checklist form no."] = item.REQUEST_NUMBER;
                                        dr["PA Owner"] = item.PA_OWNER;
                                        dr["PG Owner"] = item.PG_OWNER;

                                        var step = context.ART_M_STEP_MOCKUP
                                                    .Where(w => w.STEP_MOCKUP_ID == iProcessMO.CURRENT_STEP_ID)
                                                    .Select(s => s.STEP_MOCKUP_NAME)
                                                    .FirstOrDefault();

                                        dr["Step"] = step;
                                        dr["Start Date"] = iProcessMO.CREATE_DATE.ToString("dd/MM/yyyy");

                                        DateTime realFinishdate = DateTime.Now;
                                        if (iProcessMO.IS_END == "X")
                                        {
                                            realFinishdate = iProcessMO.UPDATE_DATE;
                                            dr["End Date"] = iProcessMO.UPDATE_DATE.ToString("dd/MM/yyyy");
                                        }
                                        else
                                        {
                                            dr["End Date"] = "";
                                        }

                                        dtReceiveWf = iProcessMO.CREATE_DATE;
                                        stepID = Convert.ToInt32(iProcessMO.CURRENT_STEP_ID);

                                        stepDuration = null;
                                        if (stepTmp.DURATION != null)
                                        {
                                            stepDuration = Convert.ToInt32(stepTmp.DURATION);
                                        }

                                        if (!String.IsNullOrEmpty(iProcessMO.IS_STEP_DURATION_EXTEND) && iProcessMO.IS_STEP_DURATION_EXTEND.Equals("X"))
                                        {
                                            stepDuration = Convert.ToInt32(stepTmp.DURATION_EXTEND);
                                        }

                                        if (iProcessMO.IS_END == "X")
                                        {
                                            total_day = (CNService.GetWorkingDays(iProcessMO.CREATE_DATE, realFinishdate));
                                            dr["Step Duration(days)"] = total_day.ToString() + " [" + stepDuration + "]"; //(CNService.GetWorkingDays(realFinishdate.Date, iProcess.CREATE_DATE.Date) + 1) + " (" + item_allStep.DURATION.Value + ")";
                                        }
                                        else
                                        {
                                            dr["Step Duration(days)"] = "  [" + stepDuration + "]";
                                        }

                                        var reason = context.ART_M_DECISION_REASON
                                                        .Where(w => w.ART_M_DECISION_REASON_ID == iProcessMO.REASON_ID)
                                                        .Select(s => s.DESCRIPTION)
                                                        .FirstOrDefault();

                                        dr["Reason Code"] = reason;
                                        dr["Marketing"] = item.MARKETING_NAME;
                                        dr["Project name"] = item.PROJECT_NAME;
                                        dr["Creator"] = item.CREATOR_DISPLAY_TXT;

                                        dt.Rows.Add(dr);
                                    }
                                }
                            }
                            else
                            {
                                var requestItem = context.ART_WF_ARTWORK_REQUEST_ITEM
                                        .Where(r => r.REQUEST_ITEM_NO == item.WORKFLOW_NUMBER)
                                        .Select(s => s.ARTWORK_ITEM_ID)
                                        .FirstOrDefault();

                                var processesAW = (from p in context.ART_WF_ARTWORK_PROCESS
                                                   where p.ARTWORK_ITEM_ID == requestItem
                                                   select p).OrderBy(o => o.ARTWORK_SUB_ID).ToList();

                                var allStepArtwork = (from a in context.ART_M_STEP_ARTWORK
                                                      select a).ToList();


                                if (processesAW != null && processesAW.Count > 0)
                                {
                                    int total_day = 0;
                                    decimal currentDuration = 0;
                                    string currentStepDisplay = "";
                                    string currentStepUserDisplay = "";
                                    string currentDurationDisplay = "";
                                    foreach (ART_WF_ARTWORK_PROCESS iProcess in processesAW)
                                    {
                                        currentStepDisplay = "";
                                        currentStepUserDisplay = "";
                                        currentDurationDisplay = "";

                                        DateTime? dtReceiveWf = iProcess.CREATE_DATE;
                                        int stepID = Convert.ToInt32(iProcess.CURRENT_STEP_ID);

                                        int? stepDuration = null;
                                        if (item.DURATION != null)
                                        {
                                            stepDuration = Convert.ToInt32(item.DURATION);
                                        }

                                        if (!String.IsNullOrEmpty(item.IS_STEP_DURATION_EXTEND) && item.IS_STEP_DURATION_EXTEND.Equals("X"))
                                        {
                                            stepDuration = Convert.ToInt32(item.DURATION_EXTEND);
                                        }

                                        if (string.IsNullOrEmpty(iProcess.IS_END))
                                        {
                                            currentStepDisplay = (from c in allStepArtwork
                                                                  where c.STEP_ARTWORK_ID == iProcess.CURRENT_STEP_ID
                                                                  select c.STEP_ARTWORK_DESCRIPTION).FirstOrDefault();

                                            if (CNService.GetUserName(iProcess.CURRENT_USER_ID, context) != "")
                                            {
                                                currentStepUserDisplay = CNService.GetUserName(iProcess.CURRENT_USER_ID, context);
                                            }
                                            else
                                            {
                                                currentStepUserDisplay = msg;
                                            }

                                            currentDuration = Convert.ToDecimal(stepDuration);

                                            dtReceiveWf = iProcess.CREATE_DATE;
                                            DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(currentDuration));
                                            currentDurationDisplay = CNService.GetWorkingDays(DateTime.Now, dtWillFinish.Value) + " (" + dtWillFinish.Value.ToString("dd/MM/yyyy HH:mm") + ")";

                                        }

                                        DataRow dr = dt.NewRow();
                                        dr["WF no."] = item.WORKFLOW_NUMBER;
                                        dr["Packaging type"] = item.PACKING_TYPE_DISPLAY_TXT;
                                        dr["WF Status"] = item.CURRENT_STEP_DISPLAY_TXT;
                                        dr["Order Bom Component"] = item.SALES_ORDER_ITEM_COMPONENT;
                                        dr["Current step"] = currentStepDisplay; //item.CURRENT_STATUS_DISPLAY_TXT;
                                        dr["Current assign"] = currentStepUserDisplay; //item.CURRENT_ASSIGN_DISPLAY_TXT;
                                        dr["Due Date"] = currentDurationDisplay; //item.DUEDATE_DISPLAY_TXT;
                                        dr["So no."] = item.SALES_ORDER_NO;
                                        dr["Brand"] = item.BRAND_DISPLAY_TXT;
                                        dr["Additional Brand"] = item.ADDITIONAL_BRAND_DISPLAY_TXT;
                                        dr["Product code"] = item.PRODUCT_CODE_DISPLAY_TXT;
                                        dr["Prod./Insp. memo"] = item.PRODUCTION_MEMO_DISPLAY_TXT;
                                        dr["RD Reference no."] = item.RD_NUMBER_DISPLAY_TXT;
                                        dr["Production Plant"] = item.PLANT;
                                        dr["Sold to"] = item.SOLD_TO_DISPLAY_TXT;
                                        dr["Ship to"] = item.SHIP_TO_DISPLAY_TXT;
                                        dr["Country"] = item.COUNTRY;
                                        dr["Port"] = item.PORT;
                                        dr["In transit to"] = item.IN_TRANSIT_TO_DISPLAY_TXT;
                                        dr["SO create date"] = item.SALES_ORDER_CREATE_DATE;
                                        dr["RDD"] = item.RDD_DISPLAY_TXT;
                                        dr["Request form/checklist form no."] = item.REQUEST_NUMBER;
                                        dr["PA Owner"] = item.PA_OWNER;
                                        dr["PG Owner"] = item.PG_OWNER;

                                        var stepName = allStepArtwork
                                                        .Where(s => s.STEP_ARTWORK_ID == iProcess.CURRENT_STEP_ID)
                                                        .Select(l => l.STEP_ARTWORK_NAME).FirstOrDefault();

                                        if (iProcess.IS_TERMINATE == "X" || (iProcess.IS_END == "X" && !String.IsNullOrEmpty(iProcess.REMARK_KILLPROCESS)))
                                        {
                                            stepName = stepName + " [Terminated]";
                                        }

                                        dr["Step"] = stepName;


                                        dr["Start Date"] = iProcess.CREATE_DATE.ToString("dd/MM/yyyy");

                                        DateTime realFinishdate = DateTime.Now;

                                        if (iProcess.IS_END == "X")
                                        {
                                            realFinishdate = iProcess.UPDATE_DATE;
                                            dr["End Date"] = iProcess.UPDATE_DATE.ToString("dd/MM/yyyy");
                                        }
                                        else
                                        {
                                            dr["End Date"] = "";
                                        }

                                        if (iProcess.IS_END == "X")
                                        {
                                            total_day = (CNService.GetWorkingDays(iProcess.CREATE_DATE, realFinishdate));
                                            dr["Step Duration(days)"] = total_day.ToString() + " [" + stepDuration + "]"; //(CNService.GetWorkingDays(realFinishdate.Date, iProcess.CREATE_DATE.Date) + 1) + " (" + item_allStep.DURATION.Value + ")";
                                        }
                                        else
                                        {
                                            dr["Step Duration(days)"] = "  [" + stepDuration + "]";
                                        }

                                        var reason = context.ART_M_DECISION_REASON
                                                        .Where(r => r.ART_M_DECISION_REASON_ID == iProcess.REASON_ID)
                                                        .Select(s => s.DESCRIPTION)
                                                        .FirstOrDefault();

                                        dr["Reason Code"] = reason;

                                        dr["Marketing"] = item.MARKETING_NAME;
                                        dr["Project name"] = item.PROJECT_NAME;
                                        dr["Creator"] = item.CREATOR_DISPLAY_TXT;
                                        dt.Rows.Add(dr);
                                    }
                                }
                            }
                        }
                    }
                }

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "EndToEnd_report", "EndToEnd_report");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        public ActionResult EndToEndNewReport([FromUri]V_ART_ENDTOEND_REPORT_REQUEST param)
        {
            try
            {
                var list = EndToEndReportHelper.GetViewEndToEndReport(param);

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("WF no."));
                dt.Columns.Add(new DataColumn("Packaging type"));
                dt.Columns.Add(new DataColumn("WF status"));
                dt.Columns.Add(new DataColumn("Order bom component"));
                dt.Columns.Add(new DataColumn("Current step"));
                dt.Columns.Add(new DataColumn("Current assign"));
                dt.Columns.Add(new DataColumn("Current dutaion"));
                dt.Columns.Add(new DataColumn("Current due date"));
                dt.Columns.Add(new DataColumn("So no."));
                dt.Columns.Add(new DataColumn("Brand"));
                dt.Columns.Add(new DataColumn("Additional brand"));
                dt.Columns.Add(new DataColumn("Product code"));
                dt.Columns.Add(new DataColumn("Prod./insp. memo"));
                dt.Columns.Add(new DataColumn("RD reference no."));
                dt.Columns.Add(new DataColumn("Production plant"));
                dt.Columns.Add(new DataColumn("Sold to"));
                dt.Columns.Add(new DataColumn("Ship to"));
                dt.Columns.Add(new DataColumn("Country"));
                dt.Columns.Add(new DataColumn("Port"));
                dt.Columns.Add(new DataColumn("In transit to"));
                dt.Columns.Add(new DataColumn("SO create date"));
                dt.Columns.Add(new DataColumn("RDD"));
                dt.Columns.Add(new DataColumn("PA owner"));
                dt.Columns.Add(new DataColumn("PG owner"));
                dt.Columns.Add(new DataColumn("Request form/checklist form no."));
                dt.Columns.Add(new DataColumn("Step"));
                dt.Columns.Add(new DataColumn("Assign"));
                dt.Columns.Add(new DataColumn("Duration"));
                dt.Columns.Add(new DataColumn("Extend Duration"));
                dt.Columns.Add(new DataColumn("Start date"));
                dt.Columns.Add(new DataColumn("Start time"));
                dt.Columns.Add(new DataColumn("End date"));
                dt.Columns.Add(new DataColumn("End time"));
                dt.Columns.Add(new DataColumn("Used days"));
                dt.Columns.Add(new DataColumn("Due date"));
                dt.Columns.Add(new DataColumn("Due date time"));
                dt.Columns.Add(new DataColumn("Total(days)"));
                dt.Columns.Add(new DataColumn("Sender reason"));  //CR#19743 by aof  dt.Columns.Add(new DataColumn("Reason code"))
                dt.Columns.Add(new DataColumn("Project name"));
                dt.Columns.Add(new DataColumn("Creator"));
                dt.Columns.Add(new DataColumn("Marketing"));
                dt.Columns.Add(new DataColumn("Receiver reason")); //CR#19743 by aof 
                dt.Columns.Add(new DataColumn("Receiver comment")); //CR#19743 by aof 
                dt.Columns.Add(new DataColumn("Terminate reason")); //#INC-55439 by aof 
                dt.Columns.Add(new DataColumn("Terminate comment")); //#INC-55439 by aof 

                var msg = MessageHelper.GetMessage("MSG_005");
                System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex("<[^>]*>");

              

                list.dataExcel.ForEach(item =>
                {
                    DataRow dr = dt.NewRow();
                    dr["WF no."] = item.WF_NO.Trim();
                    dr["Packaging type"] = item.PACKAGING_TYPE;
                    dr["WF status"] = item.WF_STAUTS;
                    dr["Order bom component"] = item.ORDER_BOM_COMPONENT;
                    dr["Current step"] = item.CURRENT_STEP_NAME_1 == null ? "" : item.CURRENT_STEP_NAME_1.Trim();
                    dr["Current assign"] = item.CURRENT_USER_NAME_1;
                    dr["Current dutaion"] = item.DURATION_1;
                    dr["Current due date"] = item.DUE_DATE_1;
                    dr["So no."] = item.SALES_ORDER_NO;
                    dr["Brand"] = item.BRAND_NAME;
                    dr["Additional brand"] = item.ADDITIONAL_BRAND_NAME;
                    dr["Product code"] = item.PRODUCT_CODE;
                    dr["Prod./insp. memo"] = item.PROD_INSP_MEMO;
                    dr["RD reference no."] = item.REFERENCE_NO;
                    dr["Production plant"] = item.PLANT;
                    dr["Sold to"] = item.SOLD_TO;
                    dr["Ship to"] = item.SHIP_TO;
                    dr["Country"] = item.COUNTRY;
                    dr["Port"] = item.PORT;
                    dr["In transit to"] = item.IN_TRANSIT_TO;
                    dr["SO create date"] = item.CREATE_ON;
                    dr["RDD"] = item.RDD;
                    dr["PA owner"] = item.PA_NAME;
                    dr["PG owner"] = item.PG_NAME;
                    dr["Request form/checklist form no."] = item.REQUEST_NO;
                    dr["Step"] = item.CURRENT_STEP_NAME == null ? "" : item.CURRENT_STEP_NAME.Trim();
                    dr["Assign"] = item.CURRENT_USER_NAME;
                    dr["Duration"] = item.DURATION_STANDARD;
                    dr["Extend Duration"] = item.IS_STEP_DURATION_EXTEND;

                    if (item.STEP_CREATE_DATE != null) dr["Start date"] = item.STEP_CREATE_DATE.ToString("dd/MM/yyyy");
                    if (item.STEP_END_DATE != null) dr["End date"] = item.STEP_END_DATE.Value.ToString("dd/MM/yyyy");

                    if (item.STEP_CREATE_DATE != null) dr["Start time"] = item.STEP_CREATE_DATE.ToString("HH:mm:ss");
                    if (item.STEP_END_DATE != null) dr["End time"] = item.STEP_END_DATE.Value.ToString("HH:mm:ss");

                    dr["Used days"] = item.USEDAY == null ? 0 : item.USEDAY;

                    if (item.DUE_DATE != null) dr["Due date"] = item.DUE_DATE.Value.ToString("dd/MM/yyyy");
                    if (item.DUE_DATE != null) dr["Due date time"] = item.DUE_DATE.Value.ToString("HH:mm:ss");

                    dr["Total(days)"] = item.TOTALDAY==null? 0: item.TOTALDAY;
                    dr["Sender reason"] = item.REASON;  //dr["Reason code"] = item.REASON;   //CR#19743 by aof end.
                    dr["Project name"] = item.PROJECT_NAME;
                    dr["Creator"] = item.CREATOR_NAME;
                    dr["Marketing"] = item.MARKETTING;
                    // CR#19743 by aof start.
                    dr["Receiver reason"] = item.RECEIVER_REASON; 
                    if (!string.IsNullOrEmpty(item.RECEIVER_COMMENT))
                    {
                        dr["Receiver comment"] = rx.Replace(item.RECEIVER_COMMENT, ""); //System.Web.HttpUtility.HtmlEncode(item.RECEIVER_COMMENT)
                    }
                    //CR#19743 by aof end.
                    //#INC-55439 by aof start. 
                    dr["Terminate reason"] = item.TERMINATE_REASON;
                    if (!string.IsNullOrEmpty(item.TERMINATE_COMMENT))
                    {
                        dr["Terminate comment"] = rx.Replace(item.TERMINATE_COMMENT, ""); 
                    }              
                    //#INC-55439 by aof end. 
                    dt.Rows.Add(dr);
                });

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "EndToEnd_report", "EndToEnd_report");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        public ActionResult WarehouseReport([FromUri]V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
            try
            {
                var list = WarehouseReportHelper.GetWarehouseReport(param);

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("Sales org."));
                dt.Columns.Add(new DataColumn("SO no."));
                dt.Columns.Add(new DataColumn("SO Item"));
                dt.Columns.Add(new DataColumn("Product code"));
                dt.Columns.Add(new DataColumn("Material code"));
                dt.Columns.Add(new DataColumn("Material description"));
                dt.Columns.Add(new DataColumn("Pkg. Type"));
                dt.Columns.Add(new DataColumn("AW WF no."));
                dt.Columns.Add(new DataColumn("Brand"));
                dt.Columns.Add(new DataColumn("Project name"));
                dt.Columns.Add(new DataColumn("Sold-to"));
                dt.Columns.Add(new DataColumn("Sold-to name"));
                dt.Columns.Add(new DataColumn("Ship-to"));
                dt.Columns.Add(new DataColumn("Ship-to name"));
                dt.Columns.Add(new DataColumn("Port"));
                dt.Columns.Add(new DataColumn("PO no."));
                dt.Columns.Add(new DataColumn("PO Item"));
                dt.Columns.Add(new DataColumn("Purch. Org."));
                dt.Columns.Add(new DataColumn("Vendor"));
                dt.Columns.Add(new DataColumn("Vendor name"));
                dt.Columns.Add(new DataColumn("Doc. date"));
                dt.Columns.Add(new DataColumn("Deliv. date"));
                dt.Columns.Add(new DataColumn("Quantity"));
                dt.Columns.Add(new DataColumn("Unit"));

                foreach (var item in list.data)
                {
                    DataRow dr = dt.NewRow();
                    dr["Sales org."] = item.SALES_ORG;
                    dr["SO no."] = item.SALES_ORDER_NO;
                    dr["SO Item"] = item.SALES_ORDER_ITEM;
                    dr["Product code"] = item.PRODUCT_CODE;
                    dr["Material code"] = item.MATERIAL_CODE;
                    dr["Material description"] = item.MATERIAL_DECRIPTION;
                    dr["Pkg. Type"] = item.PACKAGING_TYPE_NAME;
                    dr["AW WF no."] = item.REQUEST_ITEM_NO;
                    dr["Brand"] = item.BRAND_NAME;
                    dr["Project name"] = item.PROJECT_NAME;
                    dr["Sold-to"] = item.SOLD_TO;
                    dr["Sold-to name"] = item.SOLD_TO_NAME;
                    dr["Ship-to"] = item.SHIP_TO;
                    dr["Ship-to name"] = item.SHIP_TO_NAME;
                    dr["Port"] = item.PORT;
                    dr["PO no."] = item.PURCHASE_ORDER_NO;
                    dr["PO Item"] = item.PO_ITEM_NO;
                    dr["Purch. Org."] = item.PURCHASING_ORG;
                    dr["Vendor"] = item.VENDOR_NO;
                    dr["Vendor name"] = item.VENDOR_NAME;
                    dr["Doc. date"] = item.DOC_DATE;
                    dr["Deliv. date"] = item.DELIVERY_DATE;
                    dr["Quantity"] = item.QUANTITY;
                    dr["Unit"] = item.ORDER_UNIT;

                    dt.Rows.Add(dr);
                }

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "Warehouse_report", "Warehouse_report");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        public ActionResult MaterialLockReport([FromUri]ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST param)
        {
            try
            {
                string formatDate = "dd/MM/yyyy";
                string unlock_date_from = "";
                string unlock_date_to = "";

                var list = MaterialLockReportHelper.GetMaterialLockReport(param);

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("Packaging Material"));
                dt.Columns.Add(new DataColumn("Packaging Description"));
                dt.Columns.Add(new DataColumn("Status"));
                dt.Columns.Add(new DataColumn("Request Form No."));
                dt.Columns.Add(new DataColumn("Artwork No."));
                dt.Columns.Add(new DataColumn("Mockup No."));
                dt.Columns.Add(new DataColumn("Unlock Date From"));
                dt.Columns.Add(new DataColumn("Unlock Date To"));
                dt.Columns.Add(new DataColumn("Remark unlock"));
                dt.Columns.Add(new DataColumn("Remark lock"));
                dt.Columns.Add(new DataColumn("Last updated"));
                dt.Columns.Add(new DataColumn("Product Code"));
                dt.Columns.Add(new DataColumn("Sold to"));
                dt.Columns.Add(new DataColumn("Ship to"));
                dt.Columns.Add(new DataColumn("Brand"));
                dt.Columns.Add(new DataColumn("Country"));
                dt.Columns.Add(new DataColumn("Zone"));
                dt.Columns.Add(new DataColumn("Packaging Type"));
                dt.Columns.Add(new DataColumn("Primary Type"));
                dt.Columns.Add(new DataColumn("Primary Size"));
                dt.Columns.Add(new DataColumn("Pack Size"));
                dt.Columns.Add(new DataColumn("Packaging Style"));
                dt.Columns.Add(new DataColumn("PA Owner"));
                dt.Columns.Add(new DataColumn("PG Owner"));

                foreach (var item in list.data)
                {
                    unlock_date_from = "";
                    unlock_date_to = "";

                    if (item.UNLOCK_DATE_FROM != null)
                    {
                        unlock_date_from = Convert.ToDateTime(item.UNLOCK_DATE_FROM).ToString(formatDate);
                    }
                    if (item.UNLOCK_DATE_TO != null)
                    {
                        unlock_date_to = Convert.ToDateTime(item.UNLOCK_DATE_TO).ToString(formatDate);
                    }

                    DataRow dr = dt.NewRow();

                    dr["Packaging Material"] = item.MATERIAL_NO;
                    dr["Packaging Description"] = item.MATERIAL_DESCRIPTION;
                    dr["Status"] = item.STATUS_DISPLAY_TXT;
                    dr["Unlock Date From"] = unlock_date_from;
                    dr["Unlock Date To"] = unlock_date_to;
                    dr["Remark unlock"] = CNService.RemoveHTMLTag(item.REMARK_UNLOCK);
                    dr["Remark lock"] = CNService.RemoveHTMLTag(item.REMARK_LOCK);
                    dr["Last updated"] = item.LOG_DATE;
                    dr["Request Form No."] = item.REQUEST_FORM_NO;
                    dr["Artwork No."] = item.ARTWORK_NO;
                    dr["Mockup No."] = item.MOCKUP_NO;
                    dr["Packaging Type"] = item.PACKAGING_TYPE;
                    dr["Primary Type"] = item.PRIMARY_TYPE;
                    dr["Primary Size"] = item.PRIMARY_SIZE;
                    dr["Pack Size"] = item.PACK_SIZE;
                    dr["Packaging Style"] = item.PACKAGING_STYLE;
                    dr["Product Code"] = item.PRODUCT_CODE;
                    dr["Sold to"] = item.SOLD_TO;
                    dr["Ship to"] = item.SHIP_TO;
                    dr["Brand"] = item.BRAND;
                    dr["Country"] = item.COUNTRY;
                    dr["Zone"] = item.ZONE;
                    dr["PA Owner"] = item.PIC_DISPLAY_TXT;
                    dr["PG Owner"] = item.PG_OWNER_DISPLAY_TXT;

                    dt.Rows.Add(dr);
                }

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "Lists_status_of_packaging_material_report", "Lists_material");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }
        //public ActionResult ppviewReport([FromUri] PP_REQUEST_LIST param)
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
        //        if (dt.Rows.Count > 0)
        //            ConvertToExcel(dt, "ppview", "ppview");
        //        else
        //            return View("NotFound");
        //    }
        //    catch (Exception ex)
        //    {
        //        CNService.GetErrorMessage(ex);
        //        throw ex;
        //    }
        //    return View();
        //}
        public ActionResult igridviewReport([FromUri] IGRID_REQUEST param)
        {
            IGRID_RESULT Results = new IGRID_RESULT();
            List<IGRID_MODEL> listIGrid = new List<IGRID_MODEL>();
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(CNService.strConn))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spGetpersonal2";
                    cmd.Parameters.AddWithValue("@user", CNService.curruser());
                    cmd.Parameters.AddWithValue("@where", string.Format("{0}", param.Keyword));
                    cmd.Connection = con;
                    con.Open();
                    SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                    oAdapter.Fill(dt);
                    con.Close();
                   
                }

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "IGridView", "IGridView");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }
        public ActionResult ppviewReport([FromUri] PP_REQUEST param)
        {
            PP_RESULT Results = new PP_RESULT();
            List<PP_MODEL> listPP = new List<PP_MODEL>();
            PP_MODEL2 PPview = new PP_MODEL2();
            List<PP_MODEL> listppmodel = new List<PP_MODEL>();
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Sold to"));
            dt.Columns.Add(new DataColumn("Ship to"));
            dt.Columns.Add(new DataColumn("Sales order"));
            dt.Columns.Add(new DataColumn("Item"));
            dt.Columns.Add(new DataColumn("Production Plant"));
            dt.Columns.Add(new DataColumn("Sales org"));
            dt.Columns.Add(new DataColumn("RDD"));
            dt.Columns.Add(new DataColumn("Brand"));
            dt.Columns.Add(new DataColumn("PKG Type"));
            dt.Columns.Add(new DataColumn("Product code"));
            dt.Columns.Add(new DataColumn("PKG code"));
            dt.Columns.Add(new DataColumn("Vendor"));
            dt.Columns.Add(new DataColumn("Workflow no."));
            dt.Columns.Add(new DataColumn("Receiving date-time"));
            dt.Columns.Add(new DataColumn("Comment by PA"));

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

                    if (!string.IsNullOrEmpty(param.data.SALES_ORG))
                        listPP = listPP.Where(m => m.SALES_ORG.Contains(param.data.SALES_ORG)).ToList();
                    if (!string.IsNullOrEmpty(param.data.SOLD_TO_DISPLAY_TXT))
                        listPP = listPP.Where(m => m.SOLD_TO_DISPLAY_TXT.Contains(param.data.SOLD_TO_DISPLAY_TXT)).ToList();
                    if (!string.IsNullOrEmpty(param.data.SHIP_TO_DISPLAY_TXT))
                        listPP = listPP.Where(m => m.SHIP_TO_DISPLAY_TXT.Contains(param.data.SHIP_TO_DISPLAY_TXT)).ToList();
                    if (!string.IsNullOrEmpty(param.data.BRAND_DISPLAY_TXT))
                        listPP = listPP.Where(m => m.BRAND_DISPLAY_TXT.Contains(param.data.BRAND_DISPLAY_TXT)).ToList();
                    if (!string.IsNullOrEmpty(param.data.SALES_ORG))
                        listPP = listPP.Where(m => m.SALES_ORG.Contains(param.data.SALES_ORG)).ToList();
                    if (!string.IsNullOrEmpty(param.data.PKG_TYPE_DISPLAY_TXT))
                        listPP = listPP.Where(m => m.PKG_TYPE_DISPLAY_TXT.Contains(param.data.PKG_TYPE_DISPLAY_TXT)).ToList();
                    if (!string.IsNullOrEmpty(param.data.PRODUCT_CODE))
                        listPP = listPP.Where(m => m.PRODUCT_CODE.Contains(param.data.PRODUCT_CODE)).ToList();
                    if (!string.IsNullOrEmpty(param.data.PKG_CODE))
                        listPP = listPP.Where(m => m.PKG_CODE.Contains(param.data.PKG_CODE)).ToList();
                    if (!string.IsNullOrEmpty(param.data.VENDOR_DISPLAY_TXT))
                        listPP = listPP.Where(m => string.Format("{0}", m.VENDOR_DISPLAY_TXT).Contains(param.data.VENDOR_DISPLAY_TXT)).ToList();
                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_NO))
                        listPP = listPP.Where(m => m.WORKFLOW_NO.Contains(param.data.WORKFLOW_NO)).ToList();
                    if (!string.IsNullOrEmpty(param.data.REMARK_BY_PA))
                        listPP = listPP.Where(m => m.REMARK_BY_PA.Contains(param.data.REMARK_BY_PA)).ToList();
                    //sort
                    listPP = listPP.OrderBy(o => o.GROUPING).ThenBy(c => c.PKG_CODE).ThenBy(u => u.WORKFLOW_NO).ToList();
                    foreach (PP_MODEL pp in listPP)
                    {
                        string[] arrSO = pp.SALES_ORDER_ITEM.Replace("<br>", "|").Split('|');
                        bool firstrow = true;
                        foreach (string s in arrSO)
                        {
                            DataRow dr = dt.NewRow();
                            dr["RDD"] = pp.RDD;
                            dr["Sold to"] = pp.SOLD_TO_DISPLAY_TXT;
                            dr["Ship to"] = pp.SHIP_TO_DISPLAY_TXT;
                            dr["Sales org"] = pp.SALES_ORG;
                            dr["Brand"] = pp.BRAND_DISPLAY_TXT;
                            dr["PKG Type"] = pp.PKG_TYPE_DISPLAY_TXT;
                            dr["Product code"] = pp.PRODUCT_CODE;
                            dr["PKG code"] = pp.PKG_CODE;
                            dr["Vendor"] = pp.VENDOR_DISPLAY_TXT;
                            dr["Workflow no."] = pp.WORKFLOW_NO;
                            dr["Receiving date-time"] = pp.RECEIVE_DATE;
                            dr["Comment by PA"] = pp.REMARK_BY_PA;
                            PPview = new PP_MODEL2();
                            PPview = MapperServices.PPVIEW_TEMPLATE(pp);
                            if (firstrow == true)
                            {
                                pp.REMARK_BY_PA = "";
                            }
                            firstrow = false;
                            PPview.ARTWORK_SUB_ID = pp.ARTWORK_SUB_ID;
                            if (s.Trim().Length > 0)
                            {
                                PPview.SALES_ORDER = s.Trim().Substring(0, 9);
                                //PPview.SALES_ORDER_ITEM = s.Trim().Substring(10, (Convert.ToInt32(s.Trim().Length) - 10)).Replace(")", "");   // commeted by aof CR#19439
                                PPview.SALES_ORDER_ITEM = CNService.getDataFromSALES_ORDER_ITEM_RDD(s.Trim(), "ITEM"); // by aof CR#19439
                                PPview.PLANT = CNService.GetSaleOrderItems(PPview, context);
                                dr["Sales order"] = PPview.SALES_ORDER;
                                dr["Item"] = PPview.SALES_ORDER_ITEM;   
                                dr["Production Plant"] = PPview.PLANT;                    
                                dr["RDD"] = CNService.getDataFromSALES_ORDER_ITEM_RDD(s.Trim(), "RDD"); // by aof CR#19439
                             
                            }
                            //listppmodel.Add(PPview);
                            if (!string.IsNullOrEmpty(param.data.SALES_ORDER))
                                if (!dr["Sales order"].ToString().Contains(param.data.SALES_ORDER)) goto end_of_loop;
                            if (!string.IsNullOrEmpty(param.data.SALES_ORDER_ITEM))
                                if (!dr["Item"].ToString().Contains(param.data.SALES_ORDER_ITEM)) goto end_of_loop;
                            if (!string.IsNullOrEmpty(param.data.PLANT))
                                if (!dr["Production Plant"].ToString().Contains(param.data.PLANT)) goto end_of_loop;
                            dt.Rows.Add(dr);
                            end_of_loop:
                            continue;
                        }
                    }

                }
                //dt = CNService.ConvertToDataTable(listppmodel);
                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "ppview", "ppview");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }
        public ActionResult MaterialLockReportV2([FromUri]TU_MATERIAL_LOCK_REPORT_MODEL_REQUEST param)
        {
            try
            {
                string formatDate = "dd/MM/yyyy";
                string unlock_date_from = "";
                string unlock_date_to = "";

                var list = MaterialLockReportHelper.GetMaterialLockReportV2(param);
                Boolean isCheckAW = param.data.SEARCH_CHECK_ARTWORK_FILE == "X"; 

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("Packaging Material"));
                dt.Columns.Add(new DataColumn("Packaging Description"));
                if (isCheckAW) dt.Columns.Add(new DataColumn("Artwork"));
                dt.Columns.Add(new DataColumn("Status"));
                dt.Columns.Add(new DataColumn("Request Form No."));
                dt.Columns.Add(new DataColumn("Artwork No."));
                dt.Columns.Add(new DataColumn("Mockup No."));
                dt.Columns.Add(new DataColumn("Unlock Date From"));
                dt.Columns.Add(new DataColumn("Unlock Date To"));
                dt.Columns.Add(new DataColumn("Remark unlock"));
                dt.Columns.Add(new DataColumn("Remark lock"));
                dt.Columns.Add(new DataColumn("Last updated"));
                dt.Columns.Add(new DataColumn("Product Code"));
                dt.Columns.Add(new DataColumn("Sold to"));
                dt.Columns.Add(new DataColumn("Ship to"));
                dt.Columns.Add(new DataColumn("Brand"));
                dt.Columns.Add(new DataColumn("Country"));
                dt.Columns.Add(new DataColumn("Zone"));  
                dt.Columns.Add(new DataColumn("Primary Size"));
                dt.Columns.Add(new DataColumn("Pack Size"));
                dt.Columns.Add(new DataColumn("Packaging Style"));
                dt.Columns.Add(new DataColumn("Packaging Type"));
                dt.Columns.Add(new DataColumn("Primary Type"));
                dt.Columns.Add(new DataColumn("PA Owner"));
                dt.Columns.Add(new DataColumn("PG Owner"));

                foreach (var item in list.data)
                {
                    unlock_date_from = "";
                    unlock_date_to = "";

                    if (item.UNLOCK_DATE_FROM != null)
                    {
                        unlock_date_from = Convert.ToDateTime(item.UNLOCK_DATE_FROM).ToString(formatDate);
                    }
                    if (item.UNLOCK_DATE_TO != null)
                    {
                        unlock_date_to = Convert.ToDateTime(item.UNLOCK_DATE_TO).ToString(formatDate);
                    }

                    DataRow dr = dt.NewRow();

                    dr["Packaging Material"] = item.MATERIAL_NO;
                    dr["Packaging Description"] = item.MATERIAL_DESCRIPTION;
                    if (isCheckAW)  dr["Artwork"] = item.IS_HAS_FILES;
                    dr["Status"] = item.STATUS;
                    dr["Unlock Date From"] = unlock_date_from;
                    dr["Unlock Date To"] = unlock_date_to;
                    dr["Remark unlock"] = CNService.RemoveHTMLTag(item.REMARK_UNLOCK);
                    dr["Remark lock"] = CNService.RemoveHTMLTag(item.REMARK_LOCK);
                    dr["Last updated"] = item.LOG_DATE;
                    dr["Request Form No."] = item.REQUEST_FORM_NO;
                    dr["Artwork No."] = item.ARTWORK_NO;
                    dr["Mockup No."] = item.MOCKUP_NO;
                    dr["Packaging Type"] = item.PACKAGING_TYPE;
                    dr["Primary Type"] = item.PRIMARY_TYPE;
                    dr["Primary Size"] = item.PRIMARY_SIZE;
                    dr["Pack Size"] = item.PACK_SIZE;
                    dr["Packaging Style"] = item.PACKAGING_STYLE;
                    dr["Product Code"] = item.PRODUCT_CODE;
                    dr["Sold to"] = item.SOLD_TO;
                    dr["Ship to"] = item.SHIP_TO;
                    dr["Brand"] = item.BRAND;
                    dr["Country"] = item.COUNTRY;
                    dr["Zone"] = item.ZONE;
                    dr["PA Owner"] = item.PA_OWNER;
                    dr["PG Owner"] = item.PG_OWNER;

                    dt.Rows.Add(dr);

                    if (item.listDETAIL != null) {
                        if (item.listDETAIL.Count> 0 )
                        {
                            foreach (var l in item.listDETAIL)
                            {
                                dr = dt.NewRow();

                                dr["Packaging Material"] = item.MATERIAL_NO;
                                dr["Packaging Description"] = item.MATERIAL_DESCRIPTION;
                                if (isCheckAW) dr["Artwork"] = item.IS_HAS_FILES;
                                dr["Status"] = item.STATUS;
                                dr["Unlock Date From"] = unlock_date_from;
                                dr["Unlock Date To"] = unlock_date_to;
                                dr["Remark unlock"] = CNService.RemoveHTMLTag(item.REMARK_UNLOCK);
                                dr["Remark lock"] = CNService.RemoveHTMLTag(item.REMARK_LOCK);
                                dr["Last updated"] = item.LOG_DATE;
                                dr["Request Form No."] = item.REQUEST_FORM_NO;
                                dr["Artwork No."] = item.ARTWORK_NO;
                                dr["Mockup No."] = item.MOCKUP_NO;

                                dr["Packaging Type"] = item.PACKAGING_TYPE;
                                dr["Primary Type"] = item.PRIMARY_TYPE;

                                dr["Primary Size"] = l.PRIMARY_SIZE;
                                dr["Pack Size"] = l.PACK_SIZE;
                                dr["Packaging Style"] = l.PACKAGING_STYLE;
                                dr["Product Code"] = l.PRODUCT_CODE;
                                dr["Sold to"] = l.SOLD_TO;
                                dr["Ship to"] = l.SHIP_TO;
                                dr["Brand"] = l.BRAND;
                                dr["Country"] = l.COUNTRY;
                                dr["Zone"] = l.ZONE;


                                dr["PA Owner"] = item.PA_OWNER;
                                dr["PG Owner"] = item.PG_OWNER;

                                dt.Rows.Add(dr);
                            }

                          
                        }
                    }

                }

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "Lists_status_of_packaging_material_report", "Lists_material");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        public ActionResult KPIReport([FromUri]KPI_REPORT_MODEL_REQUEST param)
        {
            try
            {
                DataTable dt = new DataTable();

                var list = KPIReportHelper.GetKPIReport(param);

                DateTime monthfrom = CNService.ConvertStringToDate(param.data.DATE_FROM);

                dt.Columns.Add(new DataColumn("Employee ID"));
                dt.Columns.Add(new DataColumn("Employee name"));
                dt.Columns.Add(new DataColumn("Position"));
                for (var i = 0; i < 12; i++)
                {
                    dt.Columns.Add(new DataColumn(Convert.ToDateTime(monthfrom.AddMonths(i)).ToString("MMM yyyy")));
                }
                dt.Columns.Add(new DataColumn(Convert.ToDateTime(monthfrom).ToString("MMM yyyy") + " - " + Convert.ToDateTime(monthfrom.AddMonths(5)).ToString("MMM yyyy") + " Percentage"));
                dt.Columns.Add(new DataColumn(Convert.ToDateTime(monthfrom).ToString("MMM yyyy") + " - " + Convert.ToDateTime(monthfrom.AddMonths(5)).ToString("MMM yyyy") + " Grade"));
                dt.Columns.Add(new DataColumn(Convert.ToDateTime(monthfrom).ToString("MMM yyyy") + " - " + Convert.ToDateTime(monthfrom.AddMonths(11)).ToString("MMM yyyy") + " Percentage"));
                dt.Columns.Add(new DataColumn(Convert.ToDateTime(monthfrom).ToString("MMM yyyy") + " - " + Convert.ToDateTime(monthfrom.AddMonths(11)).ToString("MMM yyyy") + " Grade"));


                foreach (var item in list.data)
                {

                    DataRow dr = dt.NewRow();

                    dr["Employee ID"] = item.EMPLOYEE_ID_DISPLAY_TEXT;
                    dr["Employee name"] = item.EMPLOYEE_NAME_DISPLAY_TEXT;
                    dr["Position"] = item.POSITION_DISPLAY_TEXT;

                    dr[Convert.ToDateTime(monthfrom.AddMonths(0)).ToString("MMM yyyy")] = item.Month1;
                    dr[Convert.ToDateTime(monthfrom.AddMonths(1)).ToString("MMM yyyy")] = item.Month2;
                    dr[Convert.ToDateTime(monthfrom.AddMonths(2)).ToString("MMM yyyy")] = item.Month3;
                    dr[Convert.ToDateTime(monthfrom.AddMonths(3)).ToString("MMM yyyy")] = item.Month4;
                    dr[Convert.ToDateTime(monthfrom.AddMonths(4)).ToString("MMM yyyy")] = item.Month5;
                    dr[Convert.ToDateTime(monthfrom.AddMonths(5)).ToString("MMM yyyy")] = item.Month6;
                    dr[Convert.ToDateTime(monthfrom.AddMonths(6)).ToString("MMM yyyy")] = item.Month7;
                    dr[Convert.ToDateTime(monthfrom.AddMonths(7)).ToString("MMM yyyy")] = item.Month8;
                    dr[Convert.ToDateTime(monthfrom.AddMonths(8)).ToString("MMM yyyy")] = item.Month9;
                    dr[Convert.ToDateTime(monthfrom.AddMonths(9)).ToString("MMM yyyy")] = item.Month10;
                    dr[Convert.ToDateTime(monthfrom.AddMonths(10)).ToString("MMM yyyy")] = item.Month11;
                    dr[Convert.ToDateTime(monthfrom.AddMonths(11)).ToString("MMM yyyy")] = item.Month12;
                    dr[Convert.ToDateTime(monthfrom).ToString("MMM yyyy") + " - " + Convert.ToDateTime(monthfrom.AddMonths(5)).ToString("MMM yyyy") + " Percentage"] = item.AVG1;
                    dr[Convert.ToDateTime(monthfrom).ToString("MMM yyyy") + " - " + Convert.ToDateTime(monthfrom.AddMonths(5)).ToString("MMM yyyy") + " Grade"] = item.GRADE1;
                    dr[Convert.ToDateTime(monthfrom).ToString("MMM yyyy") + " - " + Convert.ToDateTime(monthfrom.AddMonths(11)).ToString("MMM yyyy") + " Percentage"] = item.AVG2;
                    dr[Convert.ToDateTime(monthfrom).ToString("MMM yyyy") + " - " + Convert.ToDateTime(monthfrom.AddMonths(11)).ToString("MMM yyyy") + " Grade"] = item.GRADE2;

                    dt.Rows.Add(dr);
                }

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "KPI_Report", param.data.KPI_TYPE == "reject" ? "% Mock Up reject or revise" : param.data.KPI_TYPE == "mockstand" ? "% Standard Mock Up" : param.data.KPI_TYPE == "sendquostand" ? "% Standard quotation" : param.data.KPI_TYPE == "artworkstand" ? "% Standard artwork" : param.data.KPI_TYPE == "postand" ? "% Standard PO" : "");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        public ActionResult KPIPriceCompareReport([FromUri]ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST param)
        {
            try
            {

                var list = KPIReportHelper.GetKPIPriceTemplateCompare(param);

                DateTime monthfrom = CNService.ConvertStringToDate(param.data.DATE_FROM);
                var countwf = 0;
                var rowN = 0;

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (var item in list.data) //wf
                    {
                        DataTable dt = new DataTable();

                        var stringMOCKUPSUBID = "";
                        List<int> stringScale1 = new List<int>();

                        rowN++;
                        dt.Columns.Add(new DataColumn("WF No"));
                        dt.Columns.Add(new DataColumn("Final info"));
                        dt.Columns.Add(new DataColumn("Scale"));

                        foreach (var i in item.ALL_PRICE)
                        {
                            if (!stringMOCKUPSUBID.Contains(Convert.ToString(i.MOCKUP_SUB_ID)))
                            {
                                dt.Columns.Add(new DataColumn(i.VENDOR_DISPLAY_TXT + " (" + i.ROUND + ")"));
                                if (stringMOCKUPSUBID == "")
                                    stringMOCKUPSUBID = i.MOCKUP_SUB_ID + "";
                                else
                                    stringMOCKUPSUBID = stringMOCKUPSUBID + "-" + i.MOCKUP_SUB_ID;
                            }

                        }

                        var listVendor = stringMOCKUPSUBID.Split('-');

                        foreach (var a in item.ALL_PRICE)
                        {
                            if (!stringScale1.Contains(a.SCALE))
                            {
                                DataRow dr = dt.NewRow();

                                if (stringScale1.Count() == 0)
                                {
                                    if (item.ALL_PRICE[0].SELECTED == "X")
                                        dr["WF No"] = item.ALL_PRICE[0].WF_NO;
                                    else
                                        dr["WF No"] = item.ALL_PRICE[0].WF_NO + "*Not selected vendor";

                                    dr["Final info"] = item.ALL_PRICE[0].FINAL_INFO;
                                }


                                foreach (var e in listVendor)
                                {
                                    foreach (var i in item.ALL_PRICE)
                                    {

                                        if (e == Convert.ToString(i.MOCKUP_SUB_ID))
                                        {
                                            if (a.SCALE == i.SCALE)
                                            {
                                                dr["Scale"] = i.SCALE;
                                                dr[i.VENDOR_DISPLAY_TXT + " (" + i.ROUND + ")"] = i.PRICE;
                                            }
                                        }
                                    }
                                }
                                stringScale1.Add(a.SCALE);
                                dt.Rows.Add(dr);
                            }
                        }
                        if (countwf == 0)
                            wb.Worksheets.Add(dt, "Saving Workflow");
                        else
                            wb.Worksheet(1).Cell(rowN, 1).InsertTable(dt);

                        rowN = rowN + stringScale1.Count() + 1;
                        countwf++;
                    }

                    //if (ds.Tables.Count > 0)
                    //{
                    //ConvertToExcel(dt, "KPI_Saving_Report", "Saving Workflow");

                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;

                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=KPI_Saving_Report_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx");

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }

                    //}
                    //else
                    //    return View("NotFound");
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        public ActionResult PICMaster([FromUri]ART_M_PIC_REQUEST param)
        {
            try
            {
                var list = PICHelper.GetPIC(param);

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("ID"));
                dt.Columns.Add(new DataColumn("User name"));
                dt.Columns.Add(new DataColumn("First name"));
                dt.Columns.Add(new DataColumn("Last name"));
                dt.Columns.Add(new DataColumn("Sold-to"));
                dt.Columns.Add(new DataColumn("Sold-to name"));
                dt.Columns.Add(new DataColumn("Ship-to"));
                dt.Columns.Add(new DataColumn("Ship-to name"));
                dt.Columns.Add(new DataColumn("Zone"));
                dt.Columns.Add(new DataColumn("Country"));
                dt.Columns.Add(new DataColumn("Country name"));

                foreach (var item in list.data)
                {
                    DataRow dr = dt.NewRow();
                    dr["ID"] = item.PIC_ID;
                    dr["User name"] = item.USER_DISPLAY_TXT;
                    dr["First name"] = item.FIRST_NAME_DISPLAY_TXT;
                    dr["Last name"] = item.LAST_NAME_DISPLAY_TXT;
                    dr["Sold-to"] = item.SOLD_TO_CODE;
                    dr["Sold-to name"] = item.SOLD_TO_DISPLAY_TXT;
                    dr["Ship-to"] = item.SHIP_TO_CODE;
                    dr["Ship-to name"] = item.SHIP_TO_DISPLAY_TXT;
                    dr["Zone"] = item.ZONE;
                    dr["Country"] = item.COUNTRY;
                    dr["Country name"] = item.COUNTRY_DISPLAY_TXT;

                    dt.Rows.Add(dr);
                }

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "PIC_Master", "PIC_Master");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        public ActionResult AllCustomerXECM()
        {
            try
            {

                var res = new List<XECM_M_CUSTOMER>();
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var query = @" select * from V_ART_XECM_CUSTOMER ORDER BY CUSTOMER_CODE";
                        res = context.Database.SqlQuery<XECM_M_CUSTOMER>(query).ToList();
                    }
                }

                var dt = CNService.ConvertToDataTable(res);

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "All customer data", "All customer data");
                else
                    return View("NotFound");
            

            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }

            return View();
        }


        public ActionResult AllCustomers()
        {
            //INC-114746
            try
            {
                var dt = QueryAllCustomer();

       
                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "All customer data", "All customer data");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        public static DataTable QueryAllCustomer()
        {
            //INC-114746
            var res = new List<ART_M_USER_EXCEL>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var query = @" SELECT * FROM V_ART_ALL_CUSTOMER ORDER BY USERNAME";
                    res = context.Database.SqlQuery<ART_M_USER_EXCEL>(query).ToList();
                }
            }

            var dt = CNService.ConvertToDataTable(res);

            return dt;
        }



        public ActionResult AllUsers()
        {
            try
            {
                var dt = QueryAllUser();

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "All user data", "All user data");
                else
                    return View("NotFound");

            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        public static DataTable QueryAllUser()
        {
            var res = new List<ART_M_USER_EXCEL>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var query = @" SELECT a.[USER_ID]	
                 , a.[USERNAME]	
      ,a.[TITLE]	
      ,replace(replace(a.[FIRST_NAME], char(10), ''), char (13), '') FIRST_NAME
      ,replace(replace(a.[LAST_NAME], char(10), ''), char (13), '') LAST_NAME
      ,replace(replace(a.[EMAIL], char(10), ''), char (13), '') EMAIL	
      ,ISNULL((select ART_M_POSITION_NAME from ART_M_POSITION b where a.POSITION_ID = b.ART_M_POSITION_ID),'') POSITION_DISPLAY_TXT	
	  ,ISNULL(c2.DESCRIPTION,'') COMPANY_DISPLAY_TXT
	  ,ISNULL(d2.CUSTOMER_CODE + ':' + d2.CUSTOMER_NAME,'') CUSTOMER_DISPLAY_TXT
	  ,ISNULL(e2.DESCRIPTION,'') ROLE_DISPLAY_TXT
	  ,ISNULL(f2.TYPE_OF_PRODUCT,'') TYPE_OF_PRODUCT_DISPLAY_TXT
	  ,ISNULL(g2.FIRST_NAME + ' ' + g2.LAST_NAME,'') USER_LEADER_DISPLAY_TXT
	  ,ISNULL(h2.VENDOR_CODE + ':' + h2.VENDOR_NAME,'') VENDOR_DISPLAY_TXT
      ,ISNULL(a.[IS_ACTIVE],'') IS_ACTIVE
      ,ISNULL(a.[IS_ADUSER],'') IS_ADUSER
        FROM [dbo].[ART_M_USER] a
left join ART_M_USER_COMPANY c on a.USER_ID = c.USER_ID
left join ART_M_USER_CUSTOMER d on a.USER_ID = d.USER_ID
left join ART_M_USER_ROLE e on a.USER_ID = e.USER_ID
left join ART_M_USER_TYPE_OF_PRODUCT f on a.USER_ID = f.USER_ID
left join ART_M_USER_UPPER_LEVEL g on a.USER_ID = g.USER_ID
left join ART_M_USER_VENDOR h on a.USER_ID = h.USER_ID
left join SAP_M_COMPANY c2 on c.COMPANY_ID = c2.COMPANY_ID
left join XECM_M_CUSTOMER d2 on d.CUSTOMER_ID = d2.CUSTOMER_ID
left join ART_M_ROLE e2 on e.ROLE_ID = e2.ROLE_ID
left join SAP_M_TYPE_OF_PRODUCT f2 on f.TYPE_OF_PRODUCT_ID = f2.TYPE_OF_PRODUCT_ID
left join ART_M_USER g2 on g.UPPER_USER_ID = g2.USER_ID
left join XECM_M_VENDOR h2 on h.VENDOR_ID = h2.VENDOR_ID
order by[USERNAME]	";

                    res = context.Database.SqlQuery<ART_M_USER_EXCEL>(query).ToList();
                }
            }

            var dt = CNService.ConvertToDataTable(res);

            return dt;
        }

        public ActionResult SummaryReport([FromUri]SUMMARY_REPORT_MODEL_REQUEST param)
        {
            try
            {
                param.length = Int32.MaxValue;
                var list = SummaryReportHelper.GetSummaryReportDataDetail(param);

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("Workflow no."));
                dt.Columns.Add(new DataColumn("Current step"));
                dt.Columns.Add(new DataColumn("Workflow status"));
                dt.Columns.Add(new DataColumn("Current assign"));
                dt.Columns.Add(new DataColumn("Customer / Vendor"));
                dt.Columns.Add(new DataColumn("Assign date")); dt.Columns.Add(new DataColumn("Assign time"));
                dt.Columns.Add(new DataColumn("Duration"));
                dt.Columns.Add(new DataColumn("Extend duration"));
                dt.Columns.Add(new DataColumn("Due date")); dt.Columns.Add(new DataColumn("Due date time"));
                dt.Columns.Add(new DataColumn("End date")); dt.Columns.Add(new DataColumn("End time"));
                dt.Columns.Add(new DataColumn("Total day"));
                dt.Columns.Add(new DataColumn("Sales order no"));
                dt.Columns.Add(new DataColumn("Brand"));
                dt.Columns.Add(new DataColumn("Sold to"));
                dt.Columns.Add(new DataColumn("Ship to"));
                dt.Columns.Add(new DataColumn("Packaging type"));
                dt.Columns.Add(new DataColumn("Primary type"));
                dt.Columns.Add(new DataColumn("Product code"));
                dt.Columns.Add(new DataColumn("RDD"));
                dt.Columns.Add(new DataColumn("PA name"));
                dt.Columns.Add(new DataColumn("PG name"));
                dt.Columns.Add(new DataColumn("Marketing name"));

                foreach (var item in list.data)
                {
                    DataRow dr = dt.NewRow();
                    dr["Workflow no."] = item.WF_NO;
                    dr["Current step"] = item.CURRENT_STEP == null ? "" : item.CURRENT_STEP.Trim();
                    dr["Workflow status"] = item.WORKFLOW_STATUS;
                    dr["Current assign"] = item.CURRENT_ASSIGN;
                    dr["Customer / Vendor"] = item.CUS_OR_VEN_DISPLAY_TXT;
                    //dr["Assign date"] = item.CREATE_DATE;
                    dr["Duration"] = item.DURATION_STANDARD;
                    //dr["Due date"] = item.DUE_DATE;
                    dr["Sales order no"] = item.SALES_ORDER_NO;
                    dr["Brand"] = item.BRAND;
                    dr["Sold to"] = item.SOLD_TO;
                    dr["Ship to"] = item.SHIP_TO;
                    dr["Packaging type"] = item.PACKAGING_TYPE;
                    dr["Primary type"] = item.PRIMARY_TYPE;
                    dr["Product code"] = item.PRODUCT_CODE;
                    dr["RDD"] = item.RDD;
                    dr["PA name"] = item.PA_NAME;
                    dr["PG name"] = item.PG_NAME;
                    dr["Marketing name"] = item.MARKETTING;
                    //dr["End date"] = item.END_DATE;
                    dr["Total day"] = item.TOTAL_DAY;
                    if (!string.IsNullOrEmpty(item.EXTEND_DURATION))
                    {
                        dr["Extend duration"] = item.EXTEND_DURATION;
                    }
                    else
                        dr["Extend duration"] = "";

                    if (item.CREATE_DATE != null) dr["Assign date"] = item.CREATE_DATE.Value.ToString("dd/MM/yyyy");
                    if (item.CREATE_DATE != null) dr["Assign time"] = item.CREATE_DATE.Value.ToString("HH:mm:ss");

                    if (item.DUE_DATE != null) dr["Due date"] = item.DUE_DATE.Value.ToString("dd/MM/yyyy");
                    if (item.DUE_DATE != null) dr["Due date time"] = item.DUE_DATE.Value.ToString("HH:mm:ss");

                    if (item.END_DATE != null) dr["End date"] = item.END_DATE.Value.ToString("dd/MM/yyyy");
                    if (item.END_DATE != null) dr["End time"] = item.END_DATE.Value.ToString("HH:mm:ss");

                    dt.Rows.Add(dr);
                }

                if (dt.Rows.Count > 0)
                    ConvertToExcel(dt, "Summary_report", "Summary_report");
                else
                    return View("NotFound");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        public ActionResult CustomerArtworkReport([FromUri]CUST_ARTWORK_REPORT_REQUEST param)
        {
            try
            {
                var list = VendorCustomerCollaborationReportHelper.GetCustomerArtworkReport(param);

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("No."));
                dt.Columns.Add(new DataColumn("Customer"));
                dt.Columns.Add(new DataColumn("Total workflow"));
                dt.Columns.Add(new DataColumn("Review"));
                dt.Columns.Add(new DataColumn("Revise - Want to Adjust"));
                dt.Columns.Add(new DataColumn("Revise - Incorrect Artwork"));
                dt.Columns.Add(new DataColumn("Aprove Artwork"));
                dt.Columns.Add(new DataColumn("Approve Shade Limit"));
                dt.Columns.Add(new DataColumn("Cancel"));

                var msg = MessageHelper.GetMessage("MSG_005");
                int row = 1;
                list.data.ForEach(item =>
                {
                    DataRow dr = dt.NewRow();
                    dr["No."] = row;
                    dr["Customer"] = item.Customer.Trim();
                    dr["Total workflow"] = item.Total.ToString();
                    dr["Review"] = item.Revise_ChangeOption.ToString();
                    dr["Revise - Want to Adjust"] = item.Revise_WantToAdjust.ToString();
                    dr["Revise - Incorrect Artwork"] = item.Revise_IncorrectArtwork.ToString();
                    dr["Aprove Artwork"] = item.Approve_Artwork.ToString();
                    dr["Approve Shade Limit"] = item.Approve_ShadeLimit.ToString();
                    dr["Cancel"] = item.Cancel.ToString();

                    dt.Rows.Add(dr);
                    row++;
                });

                if (dt.Rows.Count > 0)
                {
                    ConvertToExcel(dt, "CustomerArtwork_report", "CustomerArtwork_report");
                }
                else
                {
                    return View("NotFound");
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        public ActionResult CustomerMockupReport([FromUri]CUST_MOCKUP_REPORT_REQUEST param)
        {
            try
            {
                var list = VendorCustomerCollaborationReportHelper.GetCustomerMockupReport(param);

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("No."));
                dt.Columns.Add(new DataColumn("Customer"));
                dt.Columns.Add(new DataColumn("Total workflow"));
                dt.Columns.Add(new DataColumn("Approve Mock up - No artwork"));
                dt.Columns.Add(new DataColumn("Approve Mock up - Artwork"));
                dt.Columns.Add(new DataColumn("Approve Physical mock up"));
                dt.Columns.Add(new DataColumn("Revise - Want to Adjust"));
                dt.Columns.Add(new DataColumn("Revise - Incorrect Mockup"));
                dt.Columns.Add(new DataColumn("Cancel"));

                var msg = MessageHelper.GetMessage("MSG_005");
                int row = 1;
                list.data.ForEach(item =>
                {
                    DataRow dr = dt.NewRow();
                    dr["No."] = row;
                    dr["Customer"] = item.CustomerCode.Trim();
                    dr["Total workflow"] = item.Total;
                    dr["Approve Mock up - No artwork"] = item.ApproveDieLine_NoArtwork;
                    dr["Approve Mock up - Artwork"] = item.ApproveDieLine_Artwork;
                    dr["Approve Physical mock up"] = item.ApprovePhysical_Mockup;
                    dr["Revise - Want to Adjust"] = item.Revise_WanttoAdjust;
                    dr["Revise - Incorrect Mockup"] = item.Revise_IncorrectMockup;
                    dr["Cancel"] = item.Cancel;

                    dt.Rows.Add(dr);
                    row++;
                });

                if (dt.Rows.Count > 0)
                {
                    ConvertToExcel(dt, "CustomerMockup_report", "CustomerMockup_report");
                }
                else
                {
                    return View("NotFound");
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        public ActionResult VendorArtworkReport([FromUri]VENDOR_ARTWORK_REPORT_REQUEST param)
        {
            try
            {
                var list = VendorCustomerCollaborationReportHelper.GetVendorArtworkReport(param);

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("No."));
                dt.Columns.Add(new DataColumn("Vendor"));
                dt.Columns.Add(new DataColumn("Total workflow"));
                dt.Columns.Add(new DataColumn("Approve"));
                dt.Columns.Add(new DataColumn("Revise - by PA"));
                dt.Columns.Add(new DataColumn("Revise - by PG"));
                dt.Columns.Add(new DataColumn("Revise - by QC"));
                dt.Columns.Add(new DataColumn("Revise - by Customer"));
                dt.Columns.Add(new DataColumn("Revise - by Marketing"));
                dt.Columns.Add(new DataColumn("Revise - by Vendor"));
                dt.Columns.Add(new DataColumn("Revise - by Warehouse"));
                dt.Columns.Add(new DataColumn("Revise - by Planning"));
                dt.Columns.Add(new DataColumn("Day process\r\nVendor send print master\r\nall"));
                dt.Columns.Add(new DataColumn("Day process\r\nVendor send print master\r\nontime"));
                dt.Columns.Add(new DataColumn("Day process\r\nVendor confirm PO\r\nall"));
                dt.Columns.Add(new DataColumn("Day process\r\nVendor confirm PO\r\nontime"));
                dt.Columns.Add(new DataColumn("Day process\r\nVendor send Shade Limit\r\nall"));
                dt.Columns.Add(new DataColumn("Day process\r\nVendor send Shade Limit\r\nontime"));

                var msg = MessageHelper.GetMessage("MSG_005");
                int row = 1;
                list.data.ForEach(item =>
                {
                    DataRow dr = dt.NewRow();
                    dr["No."] = row;
                    dr["Vendor"] = item.Vendor;
                    dr["Total workflow"] = item.Total;
                    dr["Approve"] = item.Approve;
                    dr["Revise - by PA"] = item.Revise_PA;
                    dr["Revise - by PG"] = item.Revise_PG;
                    dr["Revise - by QC"] = item.Revise_QC;
                    dr["Revise - by Customer"] = item.Revise_Customer;
                    dr["Revise - by Marketing"] = item.Revise_Marketing;
                    dr["Revise - by Vendor"] = item.Revise_Vendor;
                    dr["Revise - by Warehouse"] = item.Revise_Warehouse;
                    dr["Revise - by Planning"] = item.Revise_Planning;
                    dr["Day process\r\nVendor send print master\r\nall"] = item.Day_Send_Print_All;
                    dr["Day process\r\nVendor send print master\r\nontime"] = item.Day_Send_Print_Ontime;
                    dr["Day process\r\nVendor confirm PO\r\nall"] = item.Day_Confirm_PO_All;
                    dr["Day process\r\nVendor confirm PO\r\nontime"] = item.Day_Confirm_PO_Ontime;
                    dr["Day process\r\nVendor send Shade Limit\r\nall"] = item.Day_Send_Shade_All;
                    dr["Day process\r\nVendor send Shade Limit\r\nontime"] = item.Day_Send_Shade_Ontime;

                    dt.Rows.Add(dr);
                    row++;
                });

                if (dt.Rows.Count > 0)
                {
                    ConvertToExcel(dt, "VendorArtwork_report", "VendorArtwork_report");
                }
                else
                {
                    return View("NotFound");
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        public ActionResult VendorMockupReport([FromUri]VENDOR_MOCKUP_REPORT_REQUEST param)
        {
            try
            {
                var list = VendorCustomerCollaborationReportHelper.GetVendorMockupReport(param);

                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("No."));
                dt.Columns.Add(new DataColumn("Vendor"));
                dt.Columns.Add(new DataColumn("Total workflow"));
                dt.Columns.Add(new DataColumn("Approve"));
                dt.Columns.Add(new DataColumn("Revise - by Vendor"));
                dt.Columns.Add(new DataColumn("Revise - by PG"));
                dt.Columns.Add(new DataColumn("Revise - by Customer"));
                dt.Columns.Add(new DataColumn("Revise - by Marketing"));
                dt.Columns.Add(new DataColumn("Revise - by Warehouse"));
                dt.Columns.Add(new DataColumn("Revise - by Planning"));
                dt.Columns.Add(new DataColumn("Revise - by RD"));
                dt.Columns.Add(new DataColumn("Day process\r\nQuotations\r\nall"));
                dt.Columns.Add(new DataColumn("Day process\r\nQuotations\r\nontime"));
                dt.Columns.Add(new DataColumn("Day process\r\nMockup\r\nall"));
                dt.Columns.Add(new DataColumn("Day process\r\nMockup\r\nontime"));
                dt.Columns.Add(new DataColumn("Day process\r\nDie line\r\nall"));
                dt.Columns.Add(new DataColumn("Day process\r\nDie line\r\nontime"));
                dt.Columns.Add(new DataColumn("Day process\r\nMatch board\r\nall"));
                dt.Columns.Add(new DataColumn("Day process\r\nMatch board\r\nontime"));

                var msg = MessageHelper.GetMessage("MSG_005");
                int row = 1;
                list.data.ForEach(item =>
                {
                    DataRow dr = dt.NewRow();
                    dr["No."] = row;
                    dr["Vendor"] = item.Vendor;
                    dr["Total workflow"] = item.Total;
                    dr["Approve"] = item.Approve;
                    dr["Revise - by Vendor"] = item.Revise_Vendor;
                    dr["Revise - by PG"] = item.Revise_PG;
                    dr["Revise - by Customer"] = item.Revise_Customer;
                    dr["Revise - by Marketing"] = item.Revise_Marketing;
                    dr["Revise - by Warehouse"] = item.Revise_Warehouse;
                    dr["Revise - by Planning"] = item.Revise_Planning;
                    dr["Revise - by RD"] = item.Revise_RD;
                    dr["Day process\r\nQuotations\r\nall"] = item.Day_Quotations_All;
                    dr["Day process\r\nQuotations\r\nontime"] = item.Day_Quotations_Ontime;
                    dr["Day process\r\nMockup\r\nall"] = item.Day_Mockup_All;
                    dr["Day process\r\nMockup\r\nontime"] = item.Day_Mockup_Ontime;
                    dr["Day process\r\nDie line\r\nall"] = item.Day_Dieline_All;
                    dr["Day process\r\nDie line\r\nontime"] = item.Day_Dieline_Ontime;
                    dr["Day process\r\nMatch board\r\nall"] = item.Day_Matchboard_All;
                    dr["Day process\r\nMatch board\r\nontime"] = item.Day_Matchboard_Ontime;

                    dt.Rows.Add(dr);
                    row++;
                });

                if (dt.Rows.Count > 0)
                {
                    ConvertToExcel(dt, "VendorMockup_report", "VendorMockup_report");
                }
                else
                {
                    return View("NotFound");
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }

        public ActionResult DataKPIReport([FromUri]KPI_REPORT_MODEL_REQUEST param)
        {
            try
            {
                var list = KPIReportHelper.GetKPIReport(param);
                DataTable dt = new DataTable();
                dt.Columns.Add(new DataColumn("No."));
                dt.Columns.Add(new DataColumn("NAME"));
                dt.Columns.Add(new DataColumn("WF"));
                if (param.data.KPI_TYPE == "reject")
                    dt.Columns.Add(new DataColumn("REJECT_WF"));
                else
                    dt.Columns.Add(new DataColumn("CORRECT_WF"));
                if (param.data.KPI_TYPE == "postand" || param.data.KPI_TYPE == "mockstand")
                    dt.Columns.Add(new DataColumn("SEND_BACK"));

                var msg = MessageHelper.GetMessage("MSG_005");
                int row = 1;
                list.data.ForEach(item =>
                {
                    var empname = "";
                    foreach (var i in item.LIST_ALL_WF_NO)
                    {
                        DataRow dr = dt.NewRow();
                        if (empname != item.EMPLOYEE_NAME_DISPLAY_TEXT)
                        {
                            dr["No."] = row;
                            dr["NAME"] = item.EMPLOYEE_NAME_DISPLAY_TEXT;
                            empname = item.EMPLOYEE_NAME_DISPLAY_TEXT;
                            row++;
                        }

                        dr["WF"] = i;
                        if (item.LIST_CORRECT_WF_NO.Contains(i))
                        {
                            if (param.data.KPI_TYPE == "reject")
                                dr["REJECT_WF"] = "X";
                            else
                                dr["CORRECT_WF"] = "X";
                            int counter = item.LIST_CORRECT_WF_NO.IndexOf(i);
                            if (counter != -1)
                                item.LIST_CORRECT_WF_NO[counter] = "";


                        }
                        if (item.LIST_SENDBACK_WF_NO != null)
                        {
                            if (item.LIST_SENDBACK_WF_NO.Contains(i))
                            {
                                dr["SEND_BACK"] = "X";
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                });

                if (dt.Rows.Count > 0)
                {
                    ConvertToExcel(dt, "Data_KPI_Report_(" + param.data.KPI_TYPE.ToUpper() + ")", param.data.KPI_TYPE);
                }
                else
                {
                    return View("NotFound");
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                throw ex;
            }
            return View();
        }
    }
}