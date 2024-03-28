using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using DAL;
using BLL.Services;
using BLL.BizMM65Service;
using System.Web.Script.Serialization;
using System;
using System.IO;
using DAL.Model;
using System.Text;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Configuration;
using WebServices.Model;
using System.Text.RegularExpressions;
using BLL.Helpers;
using System.Threading;
using System.IO.Compression;
using System.Data;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Reflection;
using System.Net.Mail;
using System.Xml.Serialization;
using System.Globalization;
using ClosedXML.Excel;
using WebServices.Helper;
using System.Net;
using System.Collections;
//using CsvHelper;
//using CsvHelper.Configuration;
//using Newtonsoft.Json;
//using System.Dynamic;

namespace BLL.Services
{
    public class CNService
    {
        public static string strConn = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //public static string curruser = HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");

        public static string getIGridStrConn()
        {
            return ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        }
        public static DataSet GetMasterData(string sName)
        {
            using (SqlConnection con = new SqlConnection(CNService.strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetMasterData";
                cmd.Parameters.AddWithValue("@Active", sName);
                cmd.Connection = con;
                con.Open();
                DataSet oDataset = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(oDataset);
                con.Close();
                GetmasterUpdateToCSV(oDataset.Tables[0]);
                return oDataset;
            }
        }
        public static void GetmasterUpdateToCSV(DataTable Results)
        {

            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
                new DataColumn(@"Characteristic Name RCTAV-ATNAM"),
            new DataColumn(@"Characteristic Value CAWN-ATWRT(01)"),
            new DataColumn(@"Text for a table entry CLHP-CR_STATUS_TEXT"),
            new DataColumn(@"Characteristic value description CAWNT-ATWTB(01)"),
            });
            foreach (DataRow row in Results.Rows)
            {
                dt.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString()),
                string.Format("{0}", row["Changed_Charname"].ToString()),
                string.Format("{0}", row["id"].ToString()),
                string.Format("{0}", row["Old_Description"].ToString()),
                string.Format("{0}", row["Description"].ToString()));
            }
            string[] ColumnsToBeDeleted = { "Insert", "Update", "Remove" };
            if (dt.Rows.Count > 0)
                foreach (string ColName in ColumnsToBeDeleted)
                {
                    var dtclone = new DataTable();
                    if (dt.Select("IfColumn='" + ColName + "'").ToList().Count > 0)
                    {
                        if (ColName == "Update")
                        {
                            dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
                        }
                        else if (ColName == "Insert")
                        {
                            dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
                            dtclone.Columns.Remove(@"Text for a table entry CLHP-CR_STATUS_TEXT");
                        }
                        else if (ColName == "Remove")
                        {
                            dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
                            dtclone.Columns.Remove(@"Characteristic Value CAWN-ATWRT(01)");
                            dtclone.Columns.Remove(@"Characteristic value description CAWNT-ATWTB(01)");
                        }
                        //string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/CT04_" + ColName + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv");
                        string file = @"D:\SAPInterfaces\Outbound\CT04_" + ColName + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";

                        ToCSV(dtclone, file);
                    }
                }
        }
        public static DataSet GetQuery(string sName)
        {
            using (SqlConnection con = new SqlConnection(CNService.strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spQuery";
                cmd.Parameters.AddWithValue("@Material", sName);
                cmd.Connection = con;
                con.Open();
                DataSet oDataset = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(oDataset);
                con.Close();
                GetQueryToSQL(oDataset.Tables[0]);
                return oDataset;
            }
        }
        public static void GetQueryToSQL(DataTable Results)
        {

            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
                new DataColumn(@"Material Number RMMG1-MATNR"),
            new DataColumn(@"Material Description (Short Text) MAKT-MAKTX"),
            new DataColumn(@"Reference material RMMG1_REF-MATNR"),
            new DataColumn(@"IfColumn and Plant RMMG1-WERKS and Reference plant RMMG1_REF-WERKS"),
            });
            foreach (DataRow row in Results.Rows)
            {
                string[] split = string.Format("{0}", row["Plant"].ToString()).Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in split)
                {

                    if (s.Trim() != "")
                    {
                        dt.Rows.Add(string.Format("{0}", row["DocumentNo"].ToString()),
                string.Format("{0}", row["Material"].ToString()),
                string.Format("{0}", row["Description"].ToString()),
                string.Format("{0}", row["Ref"].ToString()),
                //string.Format("{0}", row["Plant"].ToString())
                string.Format("{0}", s)
                );
                    }
                }
            }
            if (dt.Rows.Count > 0)
            {
                string file = @"D:\SAPInterfaces\Outbound\MM01_CreateMAT_ExtensionPlant_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                //string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/MM01_CreateMAT_ExtensionPlant_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv");
                CNService.ToCSV(dt, file);
            }
            //sale org
            DataTable dtSaleOrg = new DataTable();
            dtSaleOrg.Columns.AddRange(new DataColumn[] { new DataColumn(@"Material Number RMMG1-MATNR"),
                new DataColumn(@"Reference material RMMG1_REF-MATNR"),
            new DataColumn(@"IfColumn and Sales Organization RMMG1-VKORG and Reference sales organization RMMG1_REF-VKORG"),
            new DataColumn(@"Distribution Channel RMMG1-VTWEG and Reference distribution channel RMMG1_REF-VTWEG"),
            });
            DataTable dtSOOrg = new DataTable();
            dtSOOrg.Columns.AddRange(new DataColumn[] { new DataColumn(@"SaleOrg") });
            foreach (DataRow row in Results.Rows)
            {
                string[] str = string.Format("{0}", row["Plant"].ToString()).Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in str)
                {

                    if (s.Trim() != "" && dtSOOrg.Select("SaleOrg='" + string.Format("{0}", s).Substring(0, 3) + "'").ToList().Count == 0)
                    {
                        dtSOOrg.Rows.Add(string.Format("{0}", s).Substring(0, 3));
                    }
                }
                foreach (DataRow soorg in dtSOOrg.Rows)
                {
                    string[] split = "DM;EX".Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string s in split)
                    {
                        dtSaleOrg.Rows.Add(string.Format("{0}", row["Material"].ToString()),
                    string.Format("{0}", row["Ref"].ToString()),
                    string.Format("{0}", soorg["SaleOrg"].ToString()),
                    //string.Format("{0}", row["Plant"].ToString())
                    string.Format("{0}", s)
                    );
                    }
                }
                if (dtSaleOrg.Rows.Count > 0)
                {
                    string file = @"D:\SAPInterfaces\Outbound\MM01_ExtendSaleOrg_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                    //string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/MM01_ExtendSaleOrg_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv");
                    CNService.ToCSV(dtSaleOrg, file);
                }
            }
            //sale org
            DataTable dtClass = new DataTable();
            dtClass.Columns.AddRange(new DataColumn[] { new DataColumn(@"Material Number RMMG1-MATNR"),
                new DataColumn(@"Loop Id Column"),
                new DataColumn(@"Characteristic Name ALLOCVALUESCHARNEW-CHARACT"),
                new DataColumn(@"Characteristic Value ALLOCVALUESCHARNEW-VALUE_CHAR") });
            foreach (DataRow row in Results.Rows)
            {
                dtClass.Rows.Add(string.Format("{0}", row["Material"].ToString()),
                string.Format("{0}", "H"),
                string.Format("{0}", ""),
                string.Format("{0}", "")
                );
                DataTable dtCharacteristic = CNService.builditems(@"select * from MasCharacteristic where MaterialType  like '%" +
                    row["Material"].ToString().Substring(1, 1) + "%' order by Id");
                foreach (DataRow dr in dtCharacteristic.Rows)
                {
                    string value = string.Format("{0}", dr["shortname"]);
                    dtClass.Rows.Add(string.Format("{0}", ""),
                    string.Format("{0}", "D"),
                    string.Format("{0}", dr["Title"]),
                    string.Format("{0}", row[value])
                    );
                }
                if (dtClass.Rows.Count > 0)
                {
                    string file = @"D:\SAPInterfaces\Outbound\BAPI_UpdateMATCharacteristics_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                    //string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/BAPI_UpdateMATCharacteristics_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv");
                    CNService.ToCSV(dtClass, file);
                }
            }
        }
        public static string curruser()
        {
            return HttpContext.Current.User.Identity.Name.Replace(@"THAIUNION\", @"");
        }
        public static void GetsaveInfoGrouprpa(string Id, string InfoGroup, string user, string Check_PChanged)
        {
            using (SqlConnection con = new SqlConnection(CNService.strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spsaveInfoGroup";
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Parameters.AddWithValue("@InfoGroup", InfoGroup);
                cmd.Parameters.AddWithValue("@user", user);
                cmd.Parameters.AddWithValue("@Check_PChanged", Check_PChanged);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        public static void massinfogroup()
        {
            //Save the uploaded Excel file.
            //string rootFolderSuccess = @"\\192.168.5.20\Packaging\TU_002_AssignMaterial\Output\";
            string rootFolderSuccess = HttpContext.Current.Server.MapPath(@"~/Output/");
            string[] files = Directory.GetFiles(rootFolderSuccess);
            foreach (string _file in files)
            {
                //string filePath = HttpContext.Current.Server.MapPath(@"~/ExcelFiles/VK11_20211208142528.xlsx");
                //Open the Excel file using ClosedXML.
                using (XLWorkbook workBook = new XLWorkbook(_file))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(3);

                    //Create a new DataTable.
                    DataTable dt = new DataTable();

                    //Loop through the Worksheet rows.
                    bool firstRow = true;
                    foreach (IXLRow row in workSheet.Rows())
                    {
                        //Use the first row to add columns to DataTable.
                        if (firstRow)
                        {
                            foreach (IXLCell cell in row.Cells())
                            {
                                dt.Columns.Add(cell.Value.ToString());
                            }
                            firstRow = false;
                        }
                        else
                        {
                            //Add rows to DataTable.
                            bool checkstatus = false;
                            if ((string.Format("{0}", row.Cell(18).Value) == "Yes" && string.Format("{0}", row.Cell(19).Value) == "Yes" && row.Cell(2).GetValue<string>().ToLower().Trim().Contains("known so"))
                                || (string.Format("{0}", row.Cell(17).Value) == "Yes" && string.Format("{0}", row.Cell(18).Value) == "Yes" && string.Format("{0}", row.Cell(19).Value) == "Yes"))
                            {
                                checkstatus = true;
                                GetsaveInfoGrouprpa(row.Cell(1).GetValue<string>().ToLower().Trim(), "0", CNService.curruser(), "True");
                                //row.Cell(20).SetValue("Yes").SetDataType(XLCellValues.Text);
                                row.Cell(20).SetValue("Yes");
                                string _subject = string.Format(@"SEC PKG Info already saved PKG Material no.: {0} / {1}<br /><br />E-Mail Material Info already saved",
                                        row.Cell(6).GetValue<string>().Trim(), row.Cell(7).GetValue<string>().Trim());
                                string material_query = @" select abc =STUFF(((SELECT DISTINCT  ';' + (select top 1  b.Email from ulogin b where b.[user_name]=f.ActiveBy)
											 from TransApprove f where MatDoc='" + row.Cell(1).GetValue<string>().ToLower().Trim() + "'  and fn in ('PA','PG','PA_Approve','PG_Approve') FOR XML PATH(''))), 1, 1, '')";
                                var table = CNService.builditems(material_query);
                                foreach (DataRow dr in table.Rows)
                                    CNService.sendemail(@dr["abc"].ToString(), "", "<br/>Comment : ", _subject, "");
                            }
                            //dt.Rows.Add();
                            //int i = 0;
                            //foreach (IXLCell cell in row.Cells())
                            //{
                            //    dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                            //    i++;
                            //}
                        }
                    }
                    //foreach (DataRow _r in dt.Rows)
                    //{
                    //    Context.Response.Write("\n" + _r["Id"] + _r["Status Delete"] + _r["Status Assign"] + _r["Status Source List"]);
                    //    
                    //}
                    workBook.Save();
                    //result = Path.GetFileNameWithoutExtension(_file);
                    //workBook.SaveAs(string.Format(@"\\192.168.5.22\Packaging\TU_002_AssignMaterial\Output\Temp\{0}.xlsx", result));
                    workBook.Dispose();
                }
                string result = System.IO.Path.GetFileNameWithoutExtension(_file);
                //string _body = string.Format("Dear All, <br/>web service update file {0} in Grid Complete.", result);
                //cs.sendemail(@"nithi.kittivorachate@thaiunion.com,voravut.somboornpong@thaiunion.com", "", _body, "RPA process _"+ result, _file);
                //var pathnew = string.Format(@"\\192.168.5.22\Packaging\TU_002_AssignMaterial\Output\Temp\{0}.xlsx", result);
                //string  path2 = @"c:\temp2\MySample.txt";
                //File.Move(_file, pathnew);
                var process = System.Diagnostics.Process.GetProcessesByName("Excel");
                foreach (var p in process)
                {
                    if (!string.IsNullOrEmpty(p.ProcessName))
                    {
                        try
                        {
                            p.Kill();
                        }
                        catch { }
                    }
                }

                //if (Session != null) { Session.Clear(); }
                //Context.Response.Write("success");

            }

        }
        public static void uploadfile(Attachment_REQUEST ro)
        {
            using (SqlConnection CN = new SqlConnection(strConn))
            {
                string qry = "insert into tblFiles2 values (@Name,@ContentType,@Data,@MatDoc,@ActiveBy)";
                SqlCommand SqlCom = new SqlCommand(qry, CN);
                //We are passing Original File Path and file byte data as sql parameters.
                SqlCom.Parameters.Add(new SqlParameter("@Name", ro.data.Name));
                SqlCom.Parameters.Add(new SqlParameter("@ContentType", ro.data.ContentType));
                SqlCom.Parameters.Add(new SqlParameter("@Data", ro.data.Data));
                SqlCom.Parameters.Add(new SqlParameter("@MatDoc", ro.data.MatDoc));
                SqlCom.Parameters.Add(new SqlParameter("@ActiveBy", ro.data.ActiveBy));
                //Open connection and execute insert query.
                CN.Open();
                SqlCom.ExecuteNonQuery();
                CN.Close();
            }
        }
        public static DataSet builddataset()
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(strConn))
            {

                using (SqlCommand command = new SqlCommand("select * from MasBrand", conn))
                {
                    command.Connection = conn;
                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        sda.Fill(ds);
                    }
                }
            }
            return ds;
        }
        public static DataTable builditems(string data)
        {
            using (SqlConnection oConn = new SqlConnection(strConn))
            {
                oConn.Open();
                string strQuery = data;
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, oConn);
                // Fill the dataset.
                oAdapter.Fill(dt);
                oConn.Close();
                oConn.Dispose();
                return dt;
            }
        }
        public static void buildtest()
        {
            Serializer ser = new Serializer();
            string xmlInputData = string.Empty;
            string xmlOutputData = string.Empty;
            //string path = Directory.GetCurrentDirectory() + @"\FileText\SALES_ORDER_NO20210209160109.xml";
            string datapath = "~/FilePath/SALES_ORDER_NO20210209160109.xml";
            string path = System.Web.HttpContext.Current.Server.MapPath(datapath);
            xmlInputData = File.ReadAllText(path);

            SAP_M_PO_COMPLETE_SO_MODEL customer = ser.Deserialize<SAP_M_PO_COMPLETE_SO_MODEL>(xmlInputData);
            var rxx = WebServices.Helper.SD_129_Helper.SavePOCompleteSO(customer);
            //xmlOutputData = ser.Serialize<SAP_M_PO_COMPLETE_SO_MODEL>(customer);
        }
        //public static List<KPILog_Summarize_REPORT>GetKPILog_Summarize(KPILog_Summarize_REPORT_REQUEST param)
        //{
        //    using (SqlConnection con = new SqlConnection(strConn))
        //    {
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = "spGetKPILog_Summarize";
        //        cmd.Parameters.AddWithValue("@LayOut", param.data.LayOut);
        //        cmd.Parameters.AddWithValue("@FrDt", Convert.ToDateTime(param.data.FrDt).ToString("yyyyMMdd"));
        //        cmd.Parameters.AddWithValue("@ToDt", Convert.ToDateTime(param.data.ToDt).ToString("yyyyMMdd"));
        //        cmd.Connection = con;
        //        con.Open();
        //        DataTable dt = new DataTable();
        //        SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
        //        oAdapter.Fill(dt);
        //        con.Close();
        //        return (from DataRow rs in dt.Rows
        //                select new KPILog_Summarize_REPORT()
        //                {
        //                    Log_XX_ModifyBy = rs[string.Format("Log_{0}_ModifyBy", param.data.LayOut)].ToString(),
        //                }).ToList();
        //    }
        //}

        public static List<Condition_MODEL> GetSearchresults(Condition_REQUEST ro)
        {
            string Str_Role = "";
            switch (string.Format("{0}", ro.data.ROLE))
            {
                case "PA":
                    Str_Role = "Where fn like '%PA%'";
                    break;
                case "PA_Submit":
                    Str_Role = "Where fn like '%PA%'";
                    break;
                case "PA_Approve":
                    Str_Role = "Where fn like '%PA_Approve%'";
                    break;
                case "PG":
                    Str_Role = "Where fn like '%PG%'";
                    break;
                case "PG_Approve":
                    Str_Role = "Where fn like '%PG_Approve%'";
                    break;
                case "PG_Assign":
                    Str_Role = "Where fn like '%PG%'";
                    break;
                case "InfoGroup":
                    Str_Role = "Where fn like '%PG_Approve%'";
                    break;
                case "PS_Approve":
                    Str_Role = "Where fn like '%PS_Approve%'";
                    break;
                case "MDC_Approve":
                    Str_Role = "Where fn like '%MDC_Approve%'";
                    break;
                case "Final_Approve":
                    Str_Role = "Where fn like '%PA_Approve%' or fn like '%PG_Approve%'";
                    break;

            }

            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spSearchresults";
                cmd.Parameters.AddWithValue("@table", "ulogin");
                cmd.Parameters.AddWithValue("@where", string.Format("{0}", Str_Role));
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
                //if (dt.Rows.Count > 0)
                //{
                //    DataRow _ravi = dt.NewRow();
                //    _ravi["au_id"] = "0";
                //    _ravi["FirstName"] = "All";
                //    dt.Rows.Add(_ravi);
                //}
                return (from DataRow dr in dt.Rows
                        select new Condition_MODEL()
                        {
                            ID = string.Format("{0}", dr["user_name"]),
                            DISPLAY_TXT = string.Format("{0} {1}", dr["FirstName"], dr["LastName"])
                        }).ToList();
            }
        }
        public static void ReUpload(int Id)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spReUpload";
                cmd.Parameters.AddWithValue("@Id", string.Format("{0}", Id));
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
            }
        }
        //public static List<TRACKINGIGRID_REPORT> GetTrackingReport(TRACKINGIGRID_REPORT_REQUEST param)
        //{
        //    using (SqlConnection con = new SqlConnection(strConn))
        //    {
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = "spGetTrackingReport";
        //        cmd.Parameters.AddWithValue("@Material", string.Format("{0}", param.data.Keyword));
        //        cmd.Parameters.AddWithValue("@Condition", param.data.Status);
        //        cmd.Parameters.AddWithValue("@User", string.Format("{0}", param.data.By == null || param.data.By == "null" ? "All" : param.data.By));
        //        cmd.Parameters.AddWithValue("@FrDt", param.data.FrDt);
        //        cmd.Parameters.AddWithValue("@ToDt", param.data.ToDt);
        //        cmd.Connection = con;
        //        con.Open();
        //        DataTable dt = new DataTable();
        //        SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
        //        oAdapter.Fill(dt);
        //        con.Close();
        //        //if (string.Format("{0}", param.data.Keyword) == "")
        //            return (from DataRow rs in dt.Rows
        //             select new TRACKINGIGRID_REPORT()
        //             {
        //                 ID = Convert.ToInt32(rs["Id"]),
        //                 DocumentNo = string.Format("{0}",rs["DocumentNo"]),
        //                 Material = string.Format("{0}", rs["Material"]),
        //                 Description = rs["Description"].ToString(),
        //                 Brand= string.Format("{0}", rs["Brand"]),
        //                 MaterialGroup= string.Format("{0}", rs["MaterialGroup"]),
        //                 PrimarySize= string.Format("{0}", rs["PrimarySize"]),
        //                 Version= string.Format("{0}", rs["Version"]),
        //                 ChangePoint= string.Format("{0}", rs["ChangePoint"]),
        //                 SheetSize= string.Format("{0}", rs["SheetSize"]),
        //                 Assignee= string.Format("{0}", rs["Assignee"]),
        //                 PackingStyle= string.Format("{0}", rs["PackingStyle"]),
        //                 Packing= string.Format("{0}", rs["Packing"]),
        //                 StyleofPrinting= string.Format("{0}", rs["StyleofPrinting"]),
        //                 ContainerType= string.Format("{0}", rs["ContainerType"]),
        //                 LidType= string.Format("{0}", rs["LidType"]),
        //                 Condition= string.Format("{0}", rs["Condition"]),
        //                 ProductCode= string.Format("{0}", rs["ProductCode"]),
        //                 FAOZone= string.Format("{0}", rs["FAOZone"]),
        //                 Plant= string.Format("{0}", rs["Plant"]),
        //                 Totalcolour= string.Format("{0}", rs["Totalcolour"]),
        //                 Processcolour= string.Format("{0}", rs["Processcolour"]),
        //                 PlantRegisteredNo= string.Format("{0}", rs["PlantRegisteredNo"]),
        //                 CompanyNameAddress= string.Format("{0}", rs["CompanyNameAddress"]),
        //                 PMScolour= string.Format("{0}", rs["PMScolour"]),
        //                 Symbol= string.Format("{0}", rs["Symbol"]),
        //                 CatchingArea= string.Format("{0}", rs["CatchingArea"]),
        //                 CatchingPeriodDate= string.Format("{0}", rs["CatchingPeriodDate"]),
        //                 Grandof= string.Format("{0}", rs["Grandof"]),
        //                 Flute= string.Format("{0}", rs["Flute"]),
        //                 Vendor= string.Format("{0}", rs["Vendor"]),
        //                 Dimension= string.Format("{0}", rs["Dimension"]),
        //                 RSC= string.Format("{0}", rs["RSC"]),
        //                 Accessories= string.Format("{0}", rs["Accessories"]),
        //                 PrintingStyleofPrimary= string.Format("{0}", rs["PrintingStyleofPrimary"]),
        //                 PrintingStyleofSecondary= string.Format("{0}", rs["PrintingStyleofSecondary"]),
        //                 CustomerDesign= string.Format("{0}", rs["CustomerDesign"]),
        //                 CustomerSpec= string.Format("{0}", rs["CustomerSpec"]),
        //                 CustomerSize= string.Format("{0}", rs["CustomerSize"]),
        //                 CustomerVendor= string.Format("{0}", rs["CustomerVendor"]),
        //                 CustomerColor= string.Format("{0}", rs["CustomerColor"]),
        //                 CustomerScanable= string.Format("{0}", rs["CustomerScanable"]),
        //                 CustomerBarcodeSpec= string.Format("{0}", rs["CustomerBarcodeSpec"]),
        //                 FirstInfoGroup= string.Format("{0}", rs["FirstInfoGroup"]),
        //                 SO= string.Format("{0}", rs["SO"]),
        //                 PICMkt= string.Format("{0}", rs["PICMkt"]),
        //                 SOPlant= string.Format("{0}", rs["SOPlant"]),
        //                 Destination= string.Format("{0}", rs["Destination"]),
        //                 Remark= string.Format("{0}", rs["Remark"]),
        //                 GrossWeight= string.Format("{0}", rs["GrossWeight"]),
        //                 FinalInfoGroup= string.Format("{0}", rs["FinalInfoGroup"]),
        //                 Note= string.Format("{0}", rs["Note"]),
        //                 Typeof= string.Format("{0}", rs["Typeof"]),
        //                 TypeofCarton2= string.Format("{0}", rs["TypeofCarton2"]),
        //                 DMSNo= string.Format("{0}", rs["DMSNo"]),
        //                 TypeofPrimary= string.Format("{0}", rs["TypeofPrimary"]),
        //                 PrintingSystem= string.Format("{0}", rs["PrintingSystem"]),
        //                 Direction= string.Format("{0}", rs["Direction"]),
        //                 RollSheet= string.Format("{0}", rs["RollSheet"]),
        //                 RequestType= string.Format("{0}", rs["RequestType"]),
        //                 PlantAddress= string.Format("{0}", rs["PlantAddress"]),
        //                 Status_upd = string.Format("{0}", rs["Status_upd"]),
        //             }).ToList();
        //        //else
        //        //{
        //        //    DataTable dtTarget = new DataTable();
        //        //    dtTarget = dt.Clone();
        //        //    DataRow[] rowsToCopy;
        //        //    rowsToCopy = dt.Select("Material LIKE '%" + param.data.Keyword.ToString().Trim() 
        //        //        + "%' or DMSNo LIKE '%" + param.data.Keyword.ToString().Trim() + "%' or DocumentNo LIKE '%" + param.data.Keyword.ToString().Trim() + "%'");
        //        //    foreach (DataRow temp in rowsToCopy)
        //        //    {
        //        //        dtTarget.ImportRow(temp);
        //        //    }
        //        //    return (from DataRow rs in dtTarget.Rows
        //        //            select new TRACKINGIGRID_REPORT()
        //        //            {
        //        //                ID = Convert.ToInt32(rs["Id"]),
        //        //            }).ToList();
        //        //}
        //    }
        //}
        public static List<OVERVIEW_REPORT> Getsapmaterial2(OVERVIEW_REPORT_REQUEST param)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spSapMaterial";
                cmd.Parameters.AddWithValue("@material", string.Format("{0}", param.data.Material));
                cmd.Parameters.AddWithValue("@Condition", string.Format("{0}", param.data.Condition));

                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
                //int row = 0;
                return (from DataRow rs in dt.Rows
                        select new OVERVIEW_REPORT()
                        {
                            ID = Convert.ToInt32(rs["Id"]),
                            DocumentNo = string.Format("{0}", rs["DocumentNo"]),
                            Material = string.Format("{0}", rs["Material"]),
                            Description = string.Format("{0}", rs["Description"]),
                            Brand = string.Format("{0}", rs["Brand"]),

                            MaterialGroup = string.Format("{0}", rs["MaterialGroup"]),
                            PrimarySize = string.Format("{0}", rs["Name"]),
                            Version = string.Format("{0}", rs["Version"]),
                            ChangePoint = string.Format("{0}", rs["ChangePoint"]),
                            SheetSize = string.Format("{0}", rs["SheetSize"]),
                            Assignee = string.Format("{0}", rs["Assignee"]),
                            PackingStyle = string.Format("{0}", rs["PackingStyle"]),
                            Packing = string.Format("{0}", rs["Packing"]),
                            StyleofPrinting = string.Format("{0}", rs["StyleofPrinting"]),
                            ContainerType = string.Format("{0}", rs["ContainerType"]),
                            LidType = string.Format("{0}", rs["LidType"]),
                            Condition = string.Format("{0}", rs["Condition"]),
                            ProductCode = string.Format("{0}", rs["ProductCode"]),
                            FAOZone = string.Format("{0}", rs["FAOZone"]),
                            Plant = string.Format("{0}", rs["Plant"]),
                            Totalcolour = string.Format("{0}", rs["Totalcolour"]),
                            Processcolour = string.Format("{0}", rs["Processcolour"]),
                            PlantRegisteredNo = string.Format("{0}", rs["PlantRegisteredNo"]),
                            CompanyNameAddress = string.Format("{0}", rs["CompanyNameAddress"]),
                            PMScolour = string.Format("{0}", rs["PMScolour"]),
                            Symbol = string.Format("{0}", rs["Symbol"]),
                            CatchingArea = string.Format("{0}", rs["CatchingArea"]),
                            CatchingPeriodDate = string.Format("{0}", rs["CatchingPeriodDate"]),
                            Grandof = string.Format("{0}", rs["Grandof"]),
                            Flute = string.Format("{0}", rs["Flute"]),
                            Vendor = string.Format("{0}", rs["Vendor"]),
                            Dimension = string.Format("{0}", rs["Dimension"]),
                            RSC = string.Format("{0}", rs["RSC"]),
                            Accessories = string.Format("{0}", rs["Accessories"]),
                            PrintingStyleofPrimary = string.Format("{0}", rs["PrintingStyleofPrimary"]),
                            PrintingStyleofSecondary = string.Format("{0}", rs["PrintingStyleofSecondary"]),
                            CustomerDesign = string.Format("{0}", rs["CustomerDesign"]),
                            CustomerSpec = string.Format("{0}", rs["CustomerSpec"]),
                            CustomerSize = string.Format("{0}", rs["CustomerSize"]),
                            CustomerVendor = string.Format("{0}", rs["CustomerVendor"]),
                            CustomerColor = string.Format("{0}", rs["CustomerColor"]),
                            CustomerScanable = string.Format("{0}", rs["CustomerScanable"]),
                            CustomerBarcodeSpec = string.Format("{0}", rs["CustomerBarcodeSpec"]),
                            FirstInfoGroup = string.Format("{0}", rs["FirstInfoGroup"]),
                            SO = string.Format("{0}", rs["SO"]),
                            PICMkt = string.Format("{0}", rs["PICMkt"]),
                            SOPlant = string.Format("{0}", rs["SOPlant"]),
                            Destination = string.Format("{0}", rs["Destination"]),
                            Remark = string.Format("{0}", rs["Remark"]),
                            GrossWeight = string.Format("{0}", rs["GrossWeight"]),
                            FinalInfoGroup = string.Format("{0}", rs["FinalInfoGroup"]),
                            Note = string.Format("{0}", rs["Note"]),
                            Typeof = string.Format("{0}", rs["Typeof"]),
                            TypeofCarton2 = string.Format("{0}", rs["TypeofCarton2"]),
                            DMSNo = string.Format("{0}", rs["DMSNo"]),
                            TypeofPrimary = string.Format("{0}", rs["TypeofPrimary"]),
                            PrintingSystem = string.Format("{0}", rs["PrintingSystem"]),
                            Direction = string.Format("{0}", rs["Direction"]),
                            RollSheet = string.Format("{0}", rs["RollSheet"]),
                            RequestType = string.Format("{0}", rs["RequestType"]),
                            PlantAddress = string.Format("{0}", rs["PlantAddress"]),

                            ModifyBy = string.Format("{0}", rs["ModifyBy"]),
                            ModifyOn = string.Format("{0}", rs["ModifyOn"])
                        }).ToList();
            }
        }
        public static List<OVERVIEW_REPORT> InactiveMat(OVERVIEW_REPORT_REQUEST param)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spInactiveMat";
                cmd.Parameters.AddWithValue("@Material", param.data.Material);
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
                return (from DataRow rs in dt.Rows
                        select new OVERVIEW_REPORT()
                        {
                            ID = Convert.ToInt32(rs["Id"]),
                        }).ToList();
            }
        }
        public static List<Condition_MODEL> GetCondition(Condition_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {

                con.Open();
                string strQuery = @"select Id,Title from MasCondition union select 0,'All'";
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();


                //con.Close();
                return (from DataRow dr in dt.Rows
                        select new Condition_MODEL()
                        {
                            ID = string.Format("{0}", dr["Id"]),
                            DISPLAY_TXT = dr["Title"].ToString()
                        }).ToList();
            }
        }
        public static List<History_MODEL> GetHistory(History_REQUEST param)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spHistory";
                cmd.Parameters.AddWithValue("@Id", param.data.ID);
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
                return (from DataRow dr in dt.Rows
                        select new History_MODEL()
                        {
                            //ID = Convert.ToInt32(dr["num_row"]),
                            Name = dr["Name"].ToString(),
                            ActiveBy = dr["ActiveBy"].ToString(),
                            Result = dr["Result"].ToString(),
                            //LidType = dr["LidType"].ToString(),
                            //ContainerType = string.Format("{0}", dr["ContainerType"]),
                            //DescriptionType = dr["DescriptionType"].ToString(),
                            //ChangePoint = dr["ChangePoint"].ToString(),
                            ModifyOn = Convert.ToDateTime(dr["ModifyOn"]),
                            //MaterialGroup = string.Format("{0}", dr["MaterialGroup"])
                        }).ToList();
            }
        }
        public static List<SelectMaster_MODEL> GetSelectMaster(SelectMaster_REQUEST param)
        {
            SelectMaster_RESULT Results = new SelectMaster_RESULT();
            DataTable resp = new DataTable();

            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spSelectMaster";
                cmd.Parameters.AddWithValue("@user", string.Format("{0}", curruser()));
                cmd.Connection = con;
                con.Open();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(resp);
                con.Close();
            }
            return (from DataRow dr in resp.Rows
                    select new SelectMaster_MODEL()
                    {
                        ID = Convert.ToInt32(dr["Id"]),
                        DISPLAY_TXT = dr["Name"].ToString(),
                        //Can = dr["Can"].ToString(),
                        //Description = dr["Description"].ToString(),
                        //LidType = dr["LidType"].ToString(),
                        //ContainerType = string.Format("{0}", dr["ContainerType"]),
                        //DescriptionType = dr["DescriptionType"].ToString(),
                        //ChangePoint = dr["ChangePoint"].ToString(),
                        //CreateBy = dr["CreateBy"].ToString(),
                        //MaterialGroup = string.Format("{0}", dr["MaterialGroup"])
                    }).ToList();
        }



        public static List<CatchingArea_MODEL> GetCatchingArea(CatchingArea_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from MasCatchingArea";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                return (from DataRow dr in dt.Rows
                        select new CatchingArea_MODEL()
                        {
                            Id = string.Format("{0}", dr["Id"]),
                            Description = dr["Description"].ToString(),
                            Inactive = dr["Inactive"].ToString(),
                        }).ToList();
            }
        }

        public static List<Plant_MODEL> GetPlant(Plant_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from MasPlant";
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                return (from DataRow dr in dt.Rows
                        select new Plant_MODEL()
                        {
                            Id = string.Format("{0}", dr["Code"]),
                            Description = dr["Title"].ToString(),
                        }).ToList();
            }
        }
        public static List<CatchingPeriod_MODEL> GetCatchingPeriod(CatchingPeriod_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasCatchingperiodDate";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new CatchingPeriod_MODEL()
                            {
                                ID = string.Format("{0}", dr["Id"]),
                                DISPLAY_TXT = dr["Description"].ToString(),// + dr["inactive_text"].ToString(),
                                Inactive = dr["Inactive"].ToString(),
                            }).ToList();
                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }

                return list;

            }
        }
        public static List<Symbol_MODEL> GetSymbol(Symbol_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from MasSymbol";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                strQuery = strQuery + @" order by CASE Description WHEN  'HEALTH CLAIM' THEN 0 
                                          when 'NO HEALTH CLAIM'  then 1
                                          when 'NUTRIENT CLAIM' then 2
                                          when 'NO NUTRIENT CLAIM' then 3
                                          when 'NO SYMBOL' then 4 else 99 END,
                                                  Description ";
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                return (from DataRow dr in dt.Rows
                        select new Symbol_MODEL()
                        {
                            Id = string.Format("{0}", dr["Id"]),
                            Description = dr["Description"].ToString(),
                            Inactive = dr["Inactive"].ToString(),
                        }).ToList();
            }
        }

        public static List<FAOZone_MODEL> GetFAOZone(FAOZone_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from MasFAOZone";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
               
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                return (from DataRow dr in dt.Rows
                        select new FAOZone_MODEL()
                        {
                            Id = string.Format("{0}", dr["Id"]),
                            Description = dr["Description"].ToString(),
                            Inactive = dr["Inactive"].ToString(),
                        }).ToList();
            }
        }
        public static List<PrimaryType_MODEL> GetPrimaryType(PrimaryType_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from MasTypeofPrimary";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                return (from DataRow dr in dt.Rows
                        select new PrimaryType_MODEL()
                        {
                            Id = string.Format("{0}", dr["Id"]),
                            Description = dr["Description"].ToString(),
                            Inactive = dr["Inactive"].ToString(),
                        }).ToList();
            }
        }
        public static List<PMSColour_MODEL> GetPMSColour(PMSColour_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasPMSColour ";
                if (ro != null)
                {
                    strQuery = strQuery + " where MaterialGroup " +
                    "like (case when N'" + ro.data.MaterialGroup + "' <> '' then N'%" + ro.data.MaterialGroup + "%' else MaterialGroup end )";
                    if (ro != null && ro.data != null)
                    { 
                        if (ro.data.IsCheckAuthorize == null)
                        {
                            strQuery = strQuery + " and isnull(inactive,'') <> 'X'";

                        } else
                        {
                            if (ro.data.IsCheckAuthorize == "X")
                            {
                                if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " AND isnull(inactive,'') <> 'X'";
                            }
                            
                        }

                     
                    }
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new PMSColour_MODEL()
                            {
                                ID = string.Format("{0}", dr["Id"]),
                                DISPLAY_TXT = dr["Description"].ToString(),// + dr["inactive_text"].ToString(),
                                MaterialGroup = string.Format("{0}", dr["MaterialGroup"]),
                                Inactive = dr["Inactive"].ToString(),
                            }).ToList();

                // added by aof
                if (ro != null && ro.data != null && !String.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = (from u1 in list
                            where (u1.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower()))
                            select u1).ToList();
                }
                // added by aof


                return list;
            }
        }
        public static List<ProcessColour_MODEL> GetProcessColour(ProcessColour_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasProcessColour";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new ProcessColour_MODEL()
                            {
                                ID = string.Format("{0}", dr["Id"]),
                                DISPLAY_TXT = dr["Description"].ToString(),// + dr["inactive_text"].ToString(),
                                MaterialGroup = string.Format("{0}", dr["MaterialGroup"]),
                                Inactive = dr["Inactive"].ToString(),
                            }).ToList();

                // added by aof
                if (ro != null && ro.data != null && !String.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = (from u1 in list
                            where (u1.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower()))
                            select u1).ToList();
                }
                // added by aof

                return list;
            }
        }

        public static List<TotalColour_MODEL> GetTotalColour(TotalColour_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasTotalColour where MaterialGroup " +
                "like (case when N'" + ro.data.MaterialGroup + "' <> '' then N'%" + ro.data.MaterialGroup + "%' else MaterialGroup end )";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " and isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " and isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new TotalColour_MODEL()
                            {
                                ID = string.Format("{0}", dr["Id"]),
                                DISPLAY_TXT = dr["Description"].ToString(),// + dr["inactive_text"].ToString(),
                                MaterialGroup = string.Format("{0}", dr["MaterialGroup"]),
                                Inactive = dr["Inactive"].ToString(),
                            }).ToList();

                // added by aof
                if (ro != null && ro.data != null && !String.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = (from u1 in list
                            where (u1.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower()))
                            select u1).ToList();
                }
                // added by aof

                return list;
            }
        }

        public static List<MaterialClass_MODEL> GetMaterialClass(MaterialClass_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from MaterialClass";
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new MaterialClass_MODEL()
                            {
                                ID = string.Format("{0}", dr["Id"]),
                                DISPLAY_TXT = dr["Description"].ToString(),
                            }).ToList();


                // added by aof

                if (ro != null && ro.data != null && !String.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = (from u1 in list
                            where (u1.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower()))
                            select u1).ToList();
                }
                // added by aof



                return list;
            }
        }
        public static List<RSCDICUT_MODEL> GetRSCDICUT(RSCDICUT_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from MasRSCDICUT where MaterialGroup " +
                    "like (case when N'" + ro.data.MaterialGroup + "' <> '' then N'%" + ro.data.MaterialGroup + "%' else MaterialGroup end )";

                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new RSCDICUT_MODEL()
                            {
                                ID = Convert.ToInt32(dr["Id"]),
                                DISPLAY_TXT = dr["Description"].ToString(),
                                MaterialGroup = string.Format("{0}", dr["MaterialGroup"]),
                            }).ToList();

                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }

                return list;
            }
        }
        public static List<RollSheet_MODEL> GetRollSheet(RollSheet_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from MasRollSheet where MaterialGroup " +
                    "like (case when N'" + ro.data.MaterialGroup + "' <> '' then N'%" + ro.data.MaterialGroup + "%' else MaterialGroup end )";
                //if (string.IsNullOrEmpty(ro.data.Inactive)) strQuery = strQuery + " and isnull(inactive,'') <> 'X'";
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new RollSheet_MODEL()
                            {
                                ID = Convert.ToInt32(dr["Id"]),
                                DISPLAY_TXT = dr["Description"].ToString(),
                                MaterialGroup = string.Format("{0}", dr["MaterialGroup"]),
                            }).ToList();

                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }

                return list;
            }
        }
        public static List<Gradeof_MODEL> GetGradeof(Gradeof_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasGradeofCarton";
                if (ro != null)
                {
                    strQuery = strQuery + " where MaterialGroup " +
                    "like (case when N'" + ro.data.MaterialGroup + "' <> '' then N'%" + ro.data.MaterialGroup + "%' else MaterialGroup end )";
                    if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                    {
                        if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " AND isnull(inactive,'') <> 'X'";
                    } else
                    {
                        strQuery = strQuery + " and isnull(inactive,'') <> 'X'";
                    }
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }

                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new Gradeof_MODEL()
                            {
                                ID = Convert.ToInt32(dr["Id"]),
                                DISPLAY_TXT = dr["Description"].ToString(),// + dr["inactive_text"].ToString(),
                                MaterialGroup = string.Format("{0}", dr["MaterialGroup"]),
                                Inactive = dr["Inactive"].ToString(),
                            }).ToList();

                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }

                return list;
            }
        }

        public static DataSet getDataSet(string CommandText)
        {
            using (SqlConnection sqlConnection = new SqlConnection(strConn))
            {
                sqlConnection.Open();
                
                SqlCommand sqlCommand = new SqlCommand(CommandText, sqlConnection);

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;

                DataSet dataSet = new DataSet();

                try
                {

                    sqlDataAdapter.Fill(dataSet, "header");
                    sqlConnection.Close();
                }
                catch (Exception _Exception)
                {
                    sqlConnection.Close();

                    return null;
                }

                return dataSet;
            }
        }

        //public static void getSchSendMail(string args)
        //{
        //    DataSet dataSet = new DataSet();
        //    using (SqlConnection con = new SqlConnection(CNService.strConn))
        //    {
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = "spsendemailobsolete";
        //        cmd.Connection = con;
        //        con.Open();
        //        SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
        //        oAdapter.Fill(dataSet, "header");
        //        con.Close();
        //    }
        //    //DataSet dataSet = CNService.getDataSet(@"select * from TransChanged_log where active='N'");
        //    //string htmlString = CNService.getHtml(dataSet);
        //    string htmlString = "send file obsolete mat";
        //    //XLWorkbook wb = new XLWorkbook();
        //    //DataTable dt = dataSet.Tables[0]; 
        //    //wb.Worksheets.Add(dt, "WorksheetName");
        //    //CNService.SendAutomatedEmail(htmlString, "email@domain.com");
        //    try
        //    {
        //        using (var context = new ARTWORKEntities())

        //        {
        //            var listConstaints = context.ART_M_CONSTANT.Where(w => w.VARIABLE_NAME == "MAIL" && w.PROGRAM_NAME == "SENDMAIL" && w.IS_ACTIVE == "X" && w.OPTION == "EQ").ToList();
        //            if (listConstaints != null && listConstaints.Count > 0)

        //                foreach (var funcs in listConstaints)
        //                    CNService.sendemail(funcs.LOWVALUE, "", htmlString, "Test QAS System Obsolete Material", CNService.ExportDataSetToExcel2(dataSet));
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        public static string ExportDataSetToExcel2(DataSet ds,string p)
        {
            //string AppLocation = "";
            //AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            //AppLocation = AppLocation.Replace("file:\\", "");
            //string file = AppLocation + "\\ExcelFiles\\DataFile" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string file = @"C:\\temp\\ExcelFiles\\"+p+"\\DataFile" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(ds.Tables[0]);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;
                wb.SaveAs(file);
            }
            return file;
        }
        public static string getHtml(DataSet dataSet)
        {
            try
            {
                string messageBody = " < font>The following are the records: </font><br><br>";

                if (dataSet.Tables[0].Rows.Count == 0)
                    return messageBody;
                string htmlTableStart = "<table style=\"border-collapse:collapse; text-align:center;\" >";
                string htmlTableEnd = "</table>";
                string htmlHeaderRowStart = "<tr style =\"background-color:#6FA1D2; color:#ffffff;\">";
                string htmlHeaderRowEnd = "</tr>";
                string htmlTrStart = "<tr style =\"color:#555555;\">";
                string htmlTrEnd = "</tr>";
                string htmlTdStart = "<td style=\" border-color:#5c87b2; border-style:solid; border-width:thin; padding: 5px;\">";
                string htmlTdEnd = "</td>";

                messageBody += htmlTableStart;
                messageBody += htmlHeaderRowStart;
                messageBody += htmlTdStart + "Id " + htmlTdEnd;
                messageBody += htmlTdStart + "Material " + htmlTdEnd;
                messageBody += htmlTdStart + "Description " + htmlTdEnd;
                messageBody += htmlTdStart + "Status " + htmlTdEnd;
                messageBody += htmlTdStart + "Remark Lock " + htmlTdEnd;
                messageBody += htmlTdStart + "Update Date " + htmlTdEnd;
                messageBody += htmlTdStart + "Update By " + htmlTdEnd;
                messageBody += htmlHeaderRowEnd;

                foreach (DataRow Row in dataSet.Tables[0].Rows)
                {
                    messageBody = messageBody + htmlTrStart;
                    messageBody = messageBody + htmlTdStart + Row["MATERIAL_LOCK_ID"] + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + Row["MATERIAL_NO"] + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + Row["MATERIAL_DESCRIPTION"] + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + Row["STATUS"] + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + Row["REMARK_LOCK"] + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + Row["UPDATE_DATE"] + htmlTdEnd;
                    messageBody = messageBody + htmlTdStart + Row["UPDATE_BY"] + htmlTdEnd;
                    messageBody = messageBody + htmlTrEnd;
                }
                messageBody = messageBody + htmlTableEnd;


                return messageBody;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void SendAutomatedEmail(string htmlString, string recipient = "user@domain.com")

        {
            try
            {
                string mailServer = "server.com";

                MailMessage message = new MailMessage("it@domain.com", recipient);
                message.IsBodyHtml = true;
                message.Body = htmlString;
                message.Subject = "Test Email";

                SmtpClient client = new SmtpClient(mailServer);
                var AuthenticationDetails = new NetworkCredential("user@domain.com", "password");
                client.Credentials = AuthenticationDetails;
                client.Send(message);
            }
            catch (Exception e)
            {

            }

        }

        public static void saveImpactedMatDesc(ImpactedMatDesc_REPORT_REQUEST_LIST param)
        {

            using (SqlConnection con = new SqlConnection(strConn))
            {
                foreach (var ro in param.data)
                {


                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spSaveImpactedMatDesc";
                    cmd.Parameters.AddWithValue("@Id", string.Format("{0}", ro.Id));
                    cmd.Parameters.AddWithValue("@Reason", string.Format("{0}", ro.Reason));
                    cmd.Parameters.AddWithValue("@Status", string.Format("{0}", ro.Status));
                    cmd.Connection = con;
                    con.Open();
                    DataTable dt = new DataTable();
                    SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                    oAdapter.Fill(dt);
                    con.Close();
                }


                //return (from DataRow dr in dt.Rows
                //        select new ImpactedMatDesc_REPORT()
                //        {
                //            Id = Convert.ToInt32(dr["Id"]),
                //        }).ToList();
            }
        }
        public static List<MasterObject_MODEL> savemaster2(MasterObject_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                using (SqlCommand cmd = new SqlCommand("spUpdateTransMaster2"))//spUpdateTransMaster
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        DataTable dt = new DataTable();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Changed_Tabname", ro.data.Changed_Tabname);
                        cmd.Parameters.AddWithValue("@Changed_Charname", ro.data.Changed_Charname);
                        cmd.Parameters.AddWithValue("@Old_Id", ro.data.Old_Id);
                        cmd.Parameters.AddWithValue("@Id", ro.data.Id);
                        cmd.Parameters.AddWithValue("@Old_Description", string.Format("{0}", ro.data.Old_Description));
                        cmd.Parameters.AddWithValue("@Description", string.Format("{0}", ro.data.Description));
                        cmd.Parameters.AddWithValue("@Changed_Action", ro.data.Changed_Action);
                        //cmd.Parameters.AddWithValue("@Changed_By", HttpContext.Current.User.Identity.Name);// ro.data.Changed_By);
                        cmd.Parameters.AddWithValue("@Changed_By", curruser());
                        cmd.Parameters.AddWithValue("@Active", ro.data.Active);
                        cmd.Parameters.AddWithValue("@Material_Group", ro.data.Material_Group);
                        cmd.Parameters.AddWithValue("@Material_Type", ro.data.Material_Type);
                        cmd.Parameters.AddWithValue("@DescriptionText", string.Format("{0}", ro.data.DescriptionText));
                        cmd.Parameters.AddWithValue("@Can", ro.data.Can);
                        cmd.Parameters.AddWithValue("@LidType", ro.data.LidType);
                        cmd.Parameters.AddWithValue("@ContainerType", ro.data.ContainerType);
                        cmd.Parameters.AddWithValue("@DescriptionType", ro.data.DescriptionType);
                        cmd.Parameters.AddWithValue("@user_name", ro.data.user_name);
                        cmd.Parameters.AddWithValue("@fn", ro.data.fn);
                        cmd.Parameters.AddWithValue("@FirstName", ro.data.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", ro.data.LastName);
                        cmd.Parameters.AddWithValue("@Email", ro.data.Email);
                        cmd.Parameters.AddWithValue("@Authorize_ChangeMaster", ro.data.Authorize_ChangeMaster);
                        cmd.Parameters.AddWithValue("@PrimaryCode", ro.data.PrimaryCode);
                        cmd.Parameters.AddWithValue("@GroupStyle", ro.data.GroupStyle);
                        cmd.Parameters.AddWithValue("@PackingStyle", ro.data.PackingStyle);
                        cmd.Parameters.AddWithValue("@RefStyle", ro.data.RefStyle);
                        cmd.Parameters.AddWithValue("@Packsize", ro.data.Packsize);
                        cmd.Parameters.AddWithValue("@BaseUnit", ro.data.BaseUnit);
                        cmd.Parameters.AddWithValue("@TypeofPrimary", ro.data.TypeofPrimary);
                        cmd.Parameters.AddWithValue("@RegisteredNo", ro.data.RegisteredNo);
                        cmd.Parameters.AddWithValue("@Address", ro.data.Address);
                        cmd.Parameters.AddWithValue("@Plant", ro.data.Plant);

                        cmd.Parameters.AddWithValue("@Product_Group", ro.data.Product_Group);
                        cmd.Parameters.AddWithValue("@Product_GroupDesc", ro.data.Product_GroupDesc);
                        cmd.Parameters.AddWithValue("@PRD_Plant", ro.data.PRD_Plant);

                        cmd.Parameters.AddWithValue("@WHNumber", ro.data.WHNumber);
                        cmd.Parameters.AddWithValue("@StorageType", ro.data.StorageType);
                        cmd.Parameters.AddWithValue("@LE_Qty", ro.data.LE_Qty);
                        cmd.Parameters.AddWithValue("@Storage_UnitType", ro.data.Storage_UnitType);

                        cmd.Parameters.AddWithValue("@Changed_Reason", ro.data.Changed_Reason);
                        cmd.Parameters.AddWithValue("@SAP_EDPUsername", ro.data.SAP_EDPUsername);
                        cmd.Parameters.AddWithValue("@Value", ro.data.Value);
                        //cmd.Parameters.AddWithValue("@userlevel",  0);
                        //cmd.Parameters.AddWithValue("@SAP_EDPPassword", ro.SAP_EDPPassword);	

                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(dt);
                        return (from DataRow dr in dt.Rows
                                select new MasterObject_MODEL()
                                {
                                    Id = string.Format("{0}", dr["Id"]),
                                }).ToList();
                    }
                }
            }
            //}
            //deletefile(datapath);
        }
        public static List<TypeOf_MODEL> GetTypeOf(TypeOf_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasTypeofCarton";
                if (ro != null)
                {
                    strQuery = strQuery + " where MaterialGroup " +
                  "like (case when N'" + ro.data.MaterialGroup + "' <> '' then N'%" + ro.data.MaterialGroup + "%' else MaterialGroup end )" +
                  "and Description = (case when N'" + ro.data.DescriptionText + "' <> '' then N'" + ro.data.DescriptionText + "' else Description end )" +
                  "and MaterialType= (case when N'" + ro.data.MaterialType + "' <> '' then N'" + ro.data.MaterialType + "' else MaterialType end )";
                    if (ro != null && ro.data != null )
                    {
                        if (ro.data.IsCheckAuthorize == null)
                        {
                            strQuery = strQuery + " and isnull(inactive,'') <> 'X'";
                        } else
                        {
                            if (ro.data.IsCheckAuthorize == "X")
                                if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " AND isnull(inactive,'') <> 'X'";
                        }                       
                    }
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }

                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new TypeOf_MODEL()
                            {
                                ID = string.Format("{0}", dr["Id"]),
                                DISPLAY_TXT = dr["Description"].ToString(),// + dr["inactive_text"].ToString(),
                                MaterialGroup = string.Format("{0}", dr["MaterialGroup"]),
                                MaterialType = string.Format("{0}", dr["MaterialType"]),
                                DescriptionText = string.Format("{0}", dr["DescriptionText"]),
                                Inactive = dr["Inactive"].ToString(),
                            }).ToList();

                //added by aof
                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }
                //added by aof
                return list;

            }
        }

        public static List<Direction_MODEL> GetDirection(Direction_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from MasDirection ";
                   
                // if (string.IsNullOrEmpty(ro.data.Inactive)) strQuery = strQuery + " and isnull(inactive,'') <> 'X'";

                if (ro != null && ro.data != null)
                {
                    if (!string.IsNullOrEmpty(ro.data.MaterialGroup))
                    {
                        strQuery = strQuery + " where MaterialGroup " +
                             " like (case when N'" + ro.data.MaterialGroup + "' <> '' then N'%" + ro.data.MaterialGroup + "%' else MaterialGroup end )";
                    }
                }


                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new Direction_MODEL()
                            {
                                ID = string.Format("{0}", dr["Id"]),
                                DISPLAY_TXT = dr["Description"].ToString(),
                                MaterialGroup = string.Format("{0}", dr["MaterialGroup"]),
                                //Inactive = dr["Inactive"].ToString(),
                            }).ToList();

                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }

                return list;
            }
        }
        public static List<Flute_MODEL> GetFlute(Flute_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasFlute";
                if (ro != null)
                {
                    strQuery = strQuery + " where MaterialGroup " +
                 "like (case when N'" + ro.data.MaterialGroup + "' <> '' then N'%" + ro.data.MaterialGroup + "%' else MaterialGroup end )";
                    if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                    {
                        if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " AND isnull(inactive,'') <> 'X'";
                    }
                    else
                    {
                        strQuery = strQuery + " and isnull(inactive,'') <> 'X'";
                    }
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new Flute_MODEL()
                            {
                                ID = string.Format("{0}", dr["Id"]),
                                DISPLAY_TXT = dr["Description"].ToString(),// + dr["inactive_text"].ToString(),
                                MaterialGroup = string.Format("{0}", dr["MaterialGroup"]),
                                Inactive = dr["Inactive"].ToString(),
                            }).ToList();

                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }

                return list;
            }
        }
        public static List<CompanyAddress_MODEL> GetCompanyAddress(CompanyAddress_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasCompanyNameAddress";
                // if (string.IsNullOrEmpty(ro.data.Inactive)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                if (ro != null)
                {
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new CompanyAddress_MODEL()
                            {
                                ID = string.Format("{0}", dr["Id"]),
                                DISPLAY_TXT = dr["Description"].ToString(),// + dr["inactive_text"].ToString(),
                                Inactive = dr["Inactive"].ToString(),
                            }).ToList();

                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }

                return list;
            }
        }
        public static List<StyleofPrinting_MODEL> GetStyleofPrinting(StyleofPrinting_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * , CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasStyleofPrinting";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new StyleofPrinting_MODEL()
                            {
                                ID = string.Format("{0}", dr["Id"]),
                                DISPLAY_TXT = dr["Description"].ToString(), // + dr["inactive_text"].ToString(),
                                MaterialGroup = string.Format("{0}", dr["MaterialGroup"]),
                                Inactive = dr["Inactive"].ToString(),
                            }).ToList();

                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }

                return list;
            }
        }
        public static List<PlantRegistered_MODEL> GetPlantRegistered(PlantRegistered_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasPlantRegisteredNo";
           

                if (ro != null)
                {
                    strQuery = strQuery + " where" +
                   " dbo.fnc_checktype( Plant,case when N'" + ro.data.Plant + "' <> '' then N'" + ro.data.Plant + "' else Plant end )>0";
                    if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                    {
                        if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " AND isnull(inactive,'') <> 'X'";
                    }
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }

                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new PlantRegistered_MODEL()
                            {
                                ID = Convert.ToInt32(dr["Id"]),
                                RegisteredNo = dr["RegisteredNo"].ToString(),
                                Address = string.Format("{0}", dr["Address"]),
                                Plant = string.Format("{0}", dr["Plant"]),
                                Inactive = dr["Inactive"].ToString(),
                                DISPLAY_TXT = dr["RegisteredNo"].ToString(),// + dr["inactive_text"].ToString(),
                            }).ToList();

                // by aof
                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }


                return list;

            }
        }

        public static List<PlantRegistered_MODEL> GetSecPlantName(PlantRegistered_REQUEST ro)
        {

            //var user = HttpContext.Current.User.Identity.Name;
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetProductCode";
                cmd.Parameters.AddWithValue("@ProductCode", string.Format("{0}", ro.data.STR_PRODUCT_CODE));
                cmd.Parameters.AddWithValue("@address", string.Format("{0}", ro.data.Address));
                cmd.Parameters.AddWithValue("@registeredNo", string.Format("{0}", ro.data.RegisteredNo));
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
                return (from DataRow dr in dt.Rows
                        select new PlantRegistered_MODEL()
                        {
                            Plant = string.Format("{0}", dr["plant"])
                        }).ToList();
            }
        }
        public static List<PlantRegistered_MODEL> GetCompanyName(PlantRegistered_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                
                

                string strQuery = @"select * from MasPlantRegisteredNo";
                if (ro != null)
                {
                    strQuery = strQuery + " where" +
                   " dbo.fnc_checktype( Address,case when N'" + ro.data.Address + "' <> '' then N'" + ro.data.Address + "' else Address end )>0" +
                   " and RegisteredNo = (case when N'" + ro.data.RegisteredNo + "' <> '' then N'" + ro.data.RegisteredNo + "' else RegisteredNo end )";
                    if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                    {
                        if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " AND isnull(inactive,'') <> 'X'";
                    }
                }

                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                return (from DataRow dr in dt.Rows
                        select new PlantRegistered_MODEL()
                        {
                            ID = Convert.ToInt32(dr["Id"]),
                            RegisteredNo = dr["RegisteredNo"].ToString(),
                            Address = string.Format("{0}", dr["Address"]),
                            Plant = string.Format("{0}", dr["Plant"]),
                            Inactive = dr["Inactive"].ToString(),
                            DISPLAY_TXT = dr["RegisteredNo"].ToString(),
                        }).ToList();
            }
        }
        public static List<Vendor_MODEL> GetVendor(Vendor_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from MasVendor";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                return (from DataRow dr in dt.Rows
                        select new Vendor_MODEL()
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Code = dr["Code"].ToString(),
                            Name = string.Format("{0}", dr["Name"]),
                            Inactive = dr["Inactive"].ToString(),
                        }).ToList();
            }
        }
        public static List<ProductGroup_MODEL> GetProductGroup(ProductGroup_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from MasProductGroup";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                return (from DataRow dr in dt.Rows
                        select new ProductGroup_MODEL()
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Product_Group = dr["Product_Group"].ToString(),
                            Product_GroupDesc = string.Format("{0}", dr["Product_GroupDesc"]),
                            PRD_Plant = string.Format("{0}", dr["PRD_Plant"]),
                            Inactive = dr["Inactive"].ToString(),
                        }).ToList();
            }
        }
        public static List<WHManagement_MODEL> GetWHManagement(WHManagement_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from MasLogistics";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                return (from DataRow dr in dt.Rows
                        select new WHManagement_MODEL()
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            ProductGroup = dr["ProductGroup"].ToString(),
                            Description = string.Format("{0}", dr["Description"]),
                            Plant = string.Format("{0}", dr["Plant"]),
                            WHNumber = string.Format("{0}", dr["WHNumber"]),
                            StorageType = string.Format("{0}", dr["StorageType"]),
                            Storage_UnitType = string.Format("{0}", dr["Storage_UnitType"]),
                            LE_Qty = string.Format("{0}", dr["LE_Qty"]),
                            Inactive = dr["Inactive"].ToString(),
                        }).ToList();
            }
        }

        public static List<CatchingMethod_MODEL> GetCatchingMethod(CatchingMethod_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from MasCatchingMethod";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                return (from DataRow dr in dt.Rows
                        select new CatchingMethod_MODEL()
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Description = string.Format("{0}", dr["Description"]),
                            Inactive = dr["Inactive"].ToString(),
                        }).ToList();
            }
        }
        public static string ReadItems(string strQuery)
        {
            string result = "";
            // (ByVal FieldName As String, ByVal TableName As String, ByVal Cur As String, ByVal Value As String) As String
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(strConn);
            SqlDataAdapter sda = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand(strQuery);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            con.Open();
            sda.SelectCommand = cmd;
            sda.Fill(dt);
            con.Close();
            con.Dispose();
            StringBuilder sb = new StringBuilder();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    sb.Append(row[0] + ",");
                }
                if (result.Length < 2)
                {
                    result = sb.ToString();
                    result = result.Substring(0, (result.Length - 1));
                }
            }
            return result;
        }
        public static void ArtworkURL(string document, string url, string ReferenceMaterial)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spinsetArtworkURL";
                cmd.Parameters.AddWithValue("@document", string.Format("{0}", document));
                cmd.Parameters.AddWithValue("@url", string.Format("{0}", url));
                cmd.Parameters.AddWithValue("@ReferenceMaterial", string.Format("{0}", ReferenceMaterial));
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        public static void Assign(AppObject_REQUEST param)
        {

            using (SqlConnection con = new SqlConnection(strConn))
            {
                using (SqlCommand cmd = new SqlCommand("spAssignDocument"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        DataTable dt = new DataTable();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Assignee", param.data.ActiveBy);
                        cmd.Parameters.AddWithValue("@Id", param.data.ID);
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        sda.Fill(dt);
                    }
                }
            }
        }
        public static void att(ArtworkObject _artworkObject, string Keys)
        {
            string datapath = @"\\192.168.1.170\hArtwork" + Keys.ToString() + ".xml";
            using (FileStream fs = new FileStream(datapath, FileMode.Create))
            {
                new XmlSerializer(typeof(ArtworkObject)).Serialize(fs, _artworkObject);
            }
        }
        public static inputArtworkNumberResponse inputArtworkNumber(inputArtworkNumber inArtwork)
        {
            inputArtworkNumberResponse inArtworkResp = new inputArtworkNumberResponse();
            string _Subject = "", _Body = "", Keys = "0", //_MailCc = "", 
                _MailTo = "";
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();
            ArtworkObject _artworkObject = inArtwork._artworkObject;
            InboundArtwork[] _itemsArtwork = inArtwork._itemsArtwork;
            try // nueng added try catach
            { 
            
            var _total = ReadItems("select count(*)total from SapMaterial Where statusapp=0 and dmsno='" + _artworkObject.ArtworkNumber + "'");
            if (Convert.ToInt32(_total.ToString()) > 0)
            {
                Keys = string.Format("{0}", ReadItems(@"select top 1 DocumentNo from SapMaterial Where statusapp=0 and dmsno='" + _artworkObject.ArtworkNumber + "'"));
                _Subject = "Artwork number duplicate value";
                _Body = string.Format("artwork number : {0} <br/>CreateBy : {1}", _artworkObject.ArtworkNumber, Getuser(_artworkObject.PAUserName.Replace(@"THAIUNION\", @""), "fn"));
                sendemail(Getuser(_artworkObject.PAUserName.Replace(@"THAIUNION\", @""), "email") + ";" + Getuser(_artworkObject.PGUserName.Replace(@"THAIUNION\", @""), "email"), GetModulEmail(CNService.Getusermail("PA_Approve")),
                _Body, _Subject, "");
            }
            else
            {
                string _Condition = "", _Code = string.Format("{0}", _artworkObject.MaterialNumber) == "" ? _artworkObject.ReferenceMaterial : _artworkObject.MaterialNumber;
                if (_artworkObject.RecordType == "U" && string.Format("{0}", _artworkObject.MaterialNumber) != "")
                    _Condition = "7";
                else if (_artworkObject.RecordType == "I")
                {
                    switch (string.Format("{0}",_artworkObject.ReferenceMaterial))
                    {
                        case "":
                            _Condition = "1";
                            break;

                        default:

                            _Condition = "4";
                            break;
                    }
                }
                DateTime myDate = DateTime.ParseExact(_artworkObject.Date + " " + _artworkObject.Time + ",531", "yyyyMMdd HH:mm:ss,fff",
                                              System.Globalization.CultureInfo.InvariantCulture);
                SqlParameter[] param = { new SqlParameter("@Code", string.Format("{0}",_Code)),
                new SqlParameter("@Condition",string.Format("{0}",_Condition)),
                new SqlParameter("@CreateBy",string.Format("{0}",_artworkObject.PAUserName.Replace(@"THAIUNION\", @"")))};
                var table = executeProcedure("spCreateDocument", param);
                foreach (DataRow value in table.Rows)
                {
                    Keys = string.Format("{0}", value["DocumentNo"]);
                        //string datapath = @"\\192.168.1.212\dArtwork" + Keys.ToString() + ".xml";
                        //using (FileStream fs = new FileStream(datapath, FileMode.Create))
                        //{
                        //    new XmlSerializer(typeof(List<InboundArtwork>)).Serialize(fs, _itemsArtwork);
                        //}
                        //att(_artworkObject, Keys);
                    inArtworkResp.inputArtworkNumberResult = string.Format("{0}", Keys);
                    ArtworkURL(value["ID"].ToString(), _artworkObject.ArtworkURL, _artworkObject.ReferenceMaterial);
                    var item = _itemsArtwork.FirstOrDefault(a => a.Characteristic == "ZPKG_SEC_CHANGE_POINT");
                    using (SqlConnection cn = new SqlConnection(strConn))
                    {
                        using (SqlCommand cmd = new SqlCommand("spAssignDocument"))
                        {
                            cmd.Connection = cn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Assignee", _artworkObject.PGUserName.Replace(@"THAIUNION\", @"").ToString());
                            cmd.Parameters.AddWithValue("@Id", value["ID"].ToString());
                            cn.Open();
                            cmd.ExecuteNonQuery();
                            cn.Close();
                        }
                    }
                    //string[] userlevel = { "PA", "PG" };
                    //foreach (string data in userlevel)
                    //{
                    using (SqlConnection con = new SqlConnection(strConn))
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spInsertMultipleRowsInterface";
                        cmd.Parameters.AddWithValue(@"@Description", string.Format("{0}", _artworkObject.MaterialDescription));
                        cmd.Parameters.AddWithValue("@Brand", "");
                        cmd.Parameters.AddWithValue("@Primarysize", "");
                        cmd.Parameters.AddWithValue("@Primarysize_Id", "");
                        cmd.Parameters.AddWithValue("@Version", value["Version"].ToString());
                        cmd.Parameters.AddWithValue("@ChangePoint", string.Format("{0}", item == null ? "N" : item.Value));
                        cmd.Parameters.AddWithValue("@MaterialGroup", "");
                        cmd.Parameters.AddWithValue("@CreateBy", string.Format("{0}", curruser()));
                        cmd.Parameters.AddWithValue("@RequestNo", value["ID"].ToString());
                        cmd.Parameters.AddWithValue("@userlevel", Getuser(curruser(), "fn"));
                        cmd.Parameters.AddWithValue("@PackingStyle", "");
                        cmd.Parameters.AddWithValue("@Packing", "");
                        cmd.Parameters.AddWithValue("@StyleofPrinting", "");
                        cmd.Parameters.AddWithValue("@ContainerType", "");
                        cmd.Parameters.AddWithValue("@LidType", "");
                        cmd.Parameters.AddWithValue("@TotalColour", "");
                        cmd.Parameters.AddWithValue("@StatusApp", string.Format("{0}", 0));
                        cmd.Parameters.AddWithValue("@ProductCode", "");
                        cmd.Parameters.AddWithValue("@FAOZone", "");
                        //cmd.Parameters.AddWithValue("@Plant", string.Format("{0}", _artworkObject.Plant.Replace(',', ';')));
                        cmd.Parameters.AddWithValue("@Plant", string.Format("{0}", _artworkObject.Plant.ToString()));
                        cmd.Parameters.AddWithValue("@Processcolour", "");
                        cmd.Parameters.AddWithValue("@PlantRegisteredNo", "");
                        cmd.Parameters.AddWithValue("@CompanyNameAddress", "");
                        cmd.Parameters.AddWithValue("@PMScolour", "");
                        cmd.Parameters.AddWithValue("@Symbol", "");
                        cmd.Parameters.AddWithValue("@CatchingArea", "");
                        cmd.Parameters.AddWithValue("@CatchingPeriodDate", "");
                        cmd.Parameters.AddWithValue("@Grandof", "");
                        cmd.Parameters.AddWithValue("@Flute", "");
                        cmd.Parameters.AddWithValue("@Vendor", "");
                        cmd.Parameters.AddWithValue("@Dimension", "");
                        cmd.Parameters.AddWithValue("@RSC", "");
                        cmd.Parameters.AddWithValue("@Accessories", "");
                        cmd.Parameters.AddWithValue("@PrintingStyleofPrimary", string.Format("{0}", _artworkObject.PrintingStyleofPrimary));
                        cmd.Parameters.AddWithValue("@PrintingStyleofSecondary", string.Format("{0}", _artworkObject.PrintingStyleofSecondary));
                        cmd.Parameters.AddWithValue("@CustomerDesign", string.Format("{0}|{1}", _artworkObject.CustomersDesign, _artworkObject.CustomersDesignDetail));
                        cmd.Parameters.AddWithValue("@CustomerSpec", string.Format("{0}|{1}", _artworkObject.CustomersSpec, _artworkObject.CustomersSpecDetail));
                        cmd.Parameters.AddWithValue("@CustomerSize", string.Format("{0}|{1}", _artworkObject.CustomersSize, _artworkObject.CustomersSizeDetail));
                        cmd.Parameters.AddWithValue("@CustomerVendor", string.Format("{0}|{1}", _artworkObject.CustomerNominatesVendor, _artworkObject.CustomerNominatesVendorDetail));
                        cmd.Parameters.AddWithValue("@CustomerColor", string.Format("{0}|{1}", _artworkObject.CustomerNominatesColorPantone, _artworkObject.CustomerNominatesColorPantoneDetail));
                        cmd.Parameters.AddWithValue("@CustomerScanable", string.Format("{0}|{1}", _artworkObject.CustomersBarcodeScanable, _artworkObject.CustomersBarcodeScanableDetail));
                        cmd.Parameters.AddWithValue("@CustomerBarcodeSpec", string.Format("{0}|{1}", _artworkObject.CustomersBarcodeSpec, _artworkObject.CustomersBarcodeSpecDetail));
                        cmd.Parameters.AddWithValue("@FirstInfoGroup", string.Format("{0}", _artworkObject.FirstInfoGroup));
                        cmd.Parameters.AddWithValue("@SO", string.Format("{0}", _artworkObject.SONumber));
                        cmd.Parameters.AddWithValue("@PICMkt", string.Format("{0}", _artworkObject.PICMKT));
                        cmd.Parameters.AddWithValue("@SOPlant", string.Format("{0}", _artworkObject.SOPlant));
                        cmd.Parameters.AddWithValue("@Destination", string.Format("{0}", _artworkObject.Destination));
                        cmd.Parameters.AddWithValue("@Remark", string.Format("{0}", _artworkObject.RemarkNoteofPA));
                        cmd.Parameters.AddWithValue("@GrossWeight", "");
                        cmd.Parameters.AddWithValue("@FinalInfoGroup", string.Format("{0}", _artworkObject.FinalInfoGroup));
                        cmd.Parameters.AddWithValue("@Note", string.Format("{0}", _artworkObject.RemarkNoteofPG));
                        cmd.Parameters.AddWithValue("@SheetSize", "");
                        cmd.Parameters.AddWithValue("@Typeof", "");
                        cmd.Parameters.AddWithValue("@TypeofCarton2", "");
                        cmd.Parameters.AddWithValue("@DMSNo", string.Format("{0}", _artworkObject.ArtworkNumber));

                        cmd.Parameters.AddWithValue("@TypeofPrimary", "");
                        cmd.Parameters.AddWithValue("@PrintingSystem", "");
                        cmd.Parameters.AddWithValue("@Direction", "");
                        cmd.Parameters.AddWithValue("@RollSheet", "");
                        cmd.Parameters.AddWithValue("@RequestType", "");
                        cmd.Parameters.AddWithValue("@PlantAddress", "");

                        cmd.Parameters.AddWithValue("@Fixed_Desc", "");
                        cmd.Parameters.AddWithValue("@Inactive", "");
                        cmd.Parameters.AddWithValue("@Catching_Method", "");
                        cmd.Parameters.AddWithValue("@Scientific_Name", "");
                        cmd.Parameters.AddWithValue("@Specie", "");
                        cmd.Parameters.AddWithValue("@SustainMaterial", string.Format("{0}", _artworkObject.SustainMaterial));
                        cmd.Parameters.AddWithValue("@SustainPlastic", string.Format("{0}", _artworkObject.SustainPlastic));
                        cmd.Parameters.AddWithValue("@SustainReuseable", string.Format("{0}", _artworkObject.SustainReuseable));
                        cmd.Parameters.AddWithValue("@SustainRecyclable", string.Format("{0}", _artworkObject.SustainRecyclable));
                        cmd.Parameters.AddWithValue("@SustainComposatable", string.Format("{0}", _artworkObject.SustainComposatable));
                        cmd.Parameters.AddWithValue("@SustainCertification", string.Format("{0}", _artworkObject.SustainCertification));
                        cmd.Parameters.AddWithValue("@SustainCertSourcing", string.Format("{0}", _artworkObject.SustainCertSourcing));
                        cmd.Parameters.AddWithValue("@SustainOther", string.Format("{0}", _artworkObject.SustainOther));
                        cmd.Parameters.AddWithValue("@SusSecondaryPKGWeight", string.Format("{0}", _artworkObject.SusSecondaryPKGWeight));
                        cmd.Parameters.AddWithValue("@SusRecycledContent", string.Format("{0}", _artworkObject.SusRecycledContent));

                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                    //}
                    foreach (var p in _itemsArtwork)
                    {
                        List<string> listsymbol = new List<string>();
                        string charac = p.Characteristic.ToString();
                        if (charac.ToString().Contains("ZPKG_SEC_PRIMARY_SIZE"))
                        {
                            string _LidType = "", _ContainerType = "";
                            var items = _itemsArtwork.Where(a => (a.Characteristic == "ZPKG_SEC_CONTAINER_TYPE" || a.Characteristic == "ZPKG_SEC_LID_TYPE"));
                            foreach (var r in items)
                            {
                                if (r.Characteristic.ToString() == "ZPKG_SEC_LID_TYPE")
                                    _LidType = string.Format("{0}", r.Value);
                                if (r.Characteristic.ToString() == "ZPKG_SEC_CONTAINER_TYPE")
                                    _ContainerType = string.Format("{0}", r.Value);
                            }
                            p.Value = ReadItems(@"SELECT top 1 code from MasPrimarySize a where isnull(Inactive,'')<>'X' and UPPER(a.Description)=N'" + p.Value.ToString().ToUpper()
                                + "' and UPPER(a.ContainerType)=N'" + _ContainerType.ToUpper() + "' and (a.DescriptionType) =N'" + _LidType.ToUpper() + "'");
                        }
                        if (charac.ToString().Contains("ZPKG_SEC_SYMBOL"))
                        {


                            var items = _itemsArtwork.Where(a => (a.Characteristic == "ZPKG_SEC_SYMBOL"));
                            foreach (var r in items)
                            {
                                listsymbol.Add(string.Format("{0}", r.Value));

                            }
                        }
                        if (charac.ToString().Contains("ZPKG_SEC_ACCESSORIES"))
                        {
                            p.Value = Insert(p.Value.ToString());
                        }
                        //Initialize SQL Server Connection
                        SqlConnection cn = new SqlConnection(strConn);
                        SqlCommand cmd = new SqlCommand("spUpdateSapMaterial", cn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Description", string.Format("{0}", p.Description));
                        //cmd.Parameters.AddWithValue("@ArtworkNumber", string.Format("{0}", _artworkObject.ArtworkNumber));
                        //cmd.Parameters.AddWithValue("@Date", string.Format("{0}", item.Date));
                        cmd.Parameters.AddWithValue("@Value", string.Format("{0}", charac.ToString().Contains("ZPKG_SEC_SYMBOL") ? String.Join(";", listsymbol.ToArray()) : p.Value));
                        cmd.Parameters.AddWithValue("@Group", string.Format("{0}", _itemsArtwork.FirstOrDefault(a => a.Characteristic == "ZPKG_SEC_GROUP").Value));
                        cmd.Parameters.AddWithValue("@Characteristic", string.Format("{0}", charac));
                        cmd.Parameters.AddWithValue("@Keys", string.Format("{0}", value["ID"]));
                        // Running the query.
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        cn.Close();
                    }
                    using (SqlConnection CN = new SqlConnection(strConn))
                    {
                        string qry = "spUpdateArtwork";
                        SqlCommand SqlCom = new SqlCommand(qry, CN);
                        SqlCom.CommandType = CommandType.StoredProcedure;
                        SqlCom.Parameters.Add(new SqlParameter("@Keys", value["ID"].ToString()));
                        CN.Open();
                        SqlCom.ExecuteNonQuery();
                        CN.Close();
                    }

                    _MailTo = Getuser(_artworkObject.PAUserName.Replace(@"THAIUNION\", @""), "email") + ';' + Getuser(_artworkObject.PGUserName.Replace(@"THAIUNION\", @""), "email");
                    _Subject = string.Format("system iGrid Request No.:{0}", value["DocumentNo"]);
                    _Body = @"integration between iGrid and Artwork system (xECM)<br/> CreateBy : "
                    + Getuser(_artworkObject.PAUserName.Replace(@"THAIUNION\", @""), "fullname") + " <br/> Material Group : " + _itemsArtwork.FirstOrDefault(a => a.Characteristic == "ZPKG_SEC_GROUP").Value;
                }
                sendemail(_MailTo, GetModulEmail(CNService.Getusermail("PA_Approve")), _Body, _Subject, "");
            }
                return inArtworkResp;
            }
            catch (Exception ex){
                sendemail(Getuser(_artworkObject.PAUserName.Replace(@"THAIUNION\", @""), "email") + ";voravut.somboornpong@thaiunion.com;pornnicha.thanarak@thaiunion.com", "", string.Format("{0}", ex), string.Format ("[i-Grid] Unable to add data to i-Grid job {0} & aw {1} ", Keys.ToString() , _artworkObject.ArtworkNumber), "");
                return inArtworkResp;
            }
            
        }
        public static string Insert(string value)
        {
            // Insert string at index 6.
            //string adjusted = "";
            int div = value.Length / 30; int a = 0;
            if (value.Length > 30)
                for (int i = 1; i <= div; i++)
                {
                    value = value.Insert((30 * i) + a, ";");
                    a++;
                }
            return value;
        }
        public static string splittext(string data, int n)
        {
            int i = n; i++;
            string CustomerDesign = string.Format("{0}", data);
            string[] words = CustomerDesign.Split('|');
            if (i > words.Length) return "";
            return words[n].ToString();
        }

        public static string Getusermail(String fn) {
            List<string> list = new List<string>();
            string strSQL = "select user_name from ulogin Where (select top 1 value from dbo.FNC_SPLIT(fn,',') where value ='" + fn + "') ='" + fn + "' and userlevel='0'";

            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlDataAdapter da = new SqlDataAdapter(strSQL, strConn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                foreach (DataRow rw in ds.Tables[0].Rows)
                {
                    if (!list.Contains(string.Format("{0}", rw["user_name"])))
                        list.Add(string.Format("{0}", rw["user_name"]));
                }
            }
            return String.Join(";", list.ToArray());
        }

        public static string Getuser(string user_name, string type)
        {
            string strData = "";
            string strSQL = @"select * from ulogin 
            where [user_name]='" + string.Format("{0}", user_name) + "' and isnull(Inactive,'')<>'X'";
            if (strSQL == "") return strData;
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlDataAdapter da = new SqlDataAdapter(strSQL, strConn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    switch (type.ToLower())
                    {
                        case "fullname":
                            strData = string.Format("{0} {1}", dr["FirstName"], dr["LastName"]);
                            break;
                        case "email":
                            strData = dr["email"].ToString();
                            break;
                        case "fn":
                            strData = dr["fn"].ToString();
                            break;
                    }
                }
            }
            return strData;
        }
        public static string insertsendmail(string MailTo, string MailCc, string _Body, string _Subject)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                //SELECT FirstName + '.' + LastName + '@thaiunion.com' AS Email"
                cmd.CommandText = "Insert into MailData values(@Sender,@To,@Cc,'',@Subject,@Body,getdate(),1,getdate(),'TEXT',1,0)";
                cmd.Parameters.AddWithValue("@Sender", String.Format("{0}", 10));
                cmd.Parameters.AddWithValue("@To", MailTo.ToString());
                cmd.Parameters.AddWithValue("@Cc", MailCc.ToString());
                cmd.Parameters.AddWithValue("@Subject", _Subject.ToString());
                cmd.Parameters.AddWithValue("@Body", _Body.ToString());
                cmd.Connection = con;
                con.Open();
                var getValue = cmd.ExecuteScalar();
                con.Close();
                return ((string)getValue == null) ? string.Empty : getValue.ToString();
            }
        }
        public static List<ScientificName_MODEL> GetScientificName(ScientificName_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasScientificName";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new ScientificName_MODEL()
                            {
                                ID = Convert.ToInt32(dr["Id"]),
                                DISPLAY_TXT = string.Format("{0}", dr["Description"]),// + dr["inactive_text"].ToString(),
                                Inactive = dr["Inactive"].ToString(),
                            }).ToList();

                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }

                return list;
            }
        }
        public static List<PrintingSystem_MODEL> GetPrintingSystem(PrintingSystem_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                if (ro == null)
                    return new List<PrintingSystem_MODEL>();
                
                string materialgroup = ro.data.MaterialGroup == null ? "" : ro.data.MaterialGroup;
                string strQuery = string.Format("select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasPrintingSystem where MaterialGroup " +
                    "like (case when N'{0}' <> '' then N'%{0}%' else MaterialGroup end )", materialgroup) ;
                    

                if (ro != null)
                {
                    strQuery = strQuery + " and isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " and isnull(inactive,'') <> 'X'";
                }


                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new PrintingSystem_MODEL()
                            {
                                ID = Convert.ToInt32(dr["Id"]),
                                DISPLAY_TXT = string.Format("{0}", dr["Description"]), // + dr["inactive_text"].ToString(),
                                MaterialGroup = string.Format("{0}", dr["MaterialGroup"]),
                                Inactive = dr["Inactive"].ToString(),
                            }).ToList();

                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }


                return list;
            }
        }
        public static List<Specie_MODEL> GetSpecie(Specie_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasSpecie";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new Specie_MODEL()
                            {
                                ID = Convert.ToInt32(dr["Id"]),
                                DISPLAY_TXT = string.Format("{0}", dr["Description"]),// + dr["inactive_text"].ToString(),
                                Inactive = dr["Inactive"].ToString(),
                            }).ToList();

                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }

                return list;
            }
        }

        //SustainPlastic
        public static List<SustainPlastic_MODEL> GetSustainPlastic(SustainPlastic_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasSustainPlastic";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new SustainPlastic_MODEL()
                            {
                                ID = string.Format("{0}", dr["Id"]),
                                value = dr["value"].ToString(),
                                Description = dr["Description"].ToString(),
                                MaterialGroup = dr["MaterialGroup"].ToString(),
                                Inactive = dr["Inactive"].ToString(),
                                DISPLAY_TXT = dr["Description"].ToString(),// + dr["inactive_text"].ToString(),
                            }).ToList();


                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }

                return list;

            }
        }


        public static List<SustainCertSourcing_MODEL> GetSustainCertSourcing(SustainCertSourcing_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasSustainCertSourcing where MaterialGroup " +
                "like (case when N'" + ro.data.MaterialGroup + "' <> '' then N'%" + ro.data.MaterialGroup + "%' else MaterialGroup end )";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " and isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " and isnull(inactive,'') <> 'X'";
                }
                //string strQuery = @"select * from MasSustainCertSourcing where MaterialGroup " +
                //"= (case when N'" + ro.data.MaterialGroup + "' <> '' then N'" + ro.data.MaterialGroup + "' else MaterialGroup end )";
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new SustainCertSourcing_MODEL()
                            {
                                ID = string.Format("{0}", dr["Id"]),
                                value = dr["value"].ToString(),
                                Description = dr["Description"].ToString(),
                                MaterialGroup = dr["MaterialGroup"].ToString(),
                                Inactive = dr["Inactive"].ToString(),
                                DISPLAY_TXT = dr["Description"].ToString(),// + dr["inactive_text"].ToString(),
                            }).ToList();

                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }

                return list;

            }
        }
        public static List<SustainMaterial_MODEL> GetSustainMaterial(SustainMaterial_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();

                string strQuery = @"select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasSustainMaterial ";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }

                //string strQuery = @"select * from MasSustainMaterial where MaterialGroup " +
                //    "= (case when N'" + ro.data.MaterialGroup + "' <> '' then N'" + ro.data.MaterialGroup + "' else MaterialGroup end )";
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new SustainMaterial_MODEL()
                            {

                                ID = string.Format("{0}", dr["Id"]),
                                value = dr["value"].ToString(),
                                Description = dr["Description"].ToString(),
                                MaterialGroup = dr["MaterialGroup"].ToString(),
                                Inactive = dr["Inactive"].ToString(),
                                DISPLAY_TXT = dr["Description"].ToString(),// + dr["inactive_text"].ToString(),
                            }).ToList();
                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }

                return list;



            }
        }
        public static List<Assign_MODEL> GetAssign(Assign_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from ulogin Where (select count(*) from dbo.FNC_SPLIT(fn,',') where value ='PG')>0";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list = (from DataRow dr in dt.Rows
                            select new Assign_MODEL()
                            {
                                ID = dr["user_name"].ToString(),
                                fn = string.Format("{0}", dr["fn"]),
                                DISPLAY_TXT = string.Format("{0} {1}", dr["FirstName"], dr["LastName"]),
                                Email = string.Format("{0}", dr["Email"]),
                                Authorize_ChangeMaster = string.Format("{0}", dr["Authorize_ChangeMaster"]),
                                SAP_EDPUsername = string.Format("{0}", dr["SAP_EDPUsername"]),
                                Inactive = dr["Inactive"].ToString(),
                            }).ToList();

                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }

                return list;

            }
        }
        public static List<ulogin_MODEL> Getulogin(ulogin_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from ulogin";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                return (from DataRow dr in dt.Rows
                        select new ulogin_MODEL()
                        {
                            Id = Convert.ToInt32(dr["au_Id"]),
                            user_name = dr["user_name"].ToString(),
                            fn = string.Format("{0}", dr["fn"]),
                            FirstName = string.Format("{0}", dr["FirstName"]),
                            LastName = string.Format("{0}", dr["LastName"]),
                            Email = string.Format("{0}", dr["Email"]),
                            Authorize_ChangeMaster = string.Format("{0}", dr["Authorize_ChangeMaster"]),
                            SAP_EDPUsername = string.Format("{0}", dr["SAP_EDPUsername"]),
                            Inactive = dr["Inactive"].ToString(),
                        }).ToList();
            }
        }
        public static string GetApproveLevel(string keys) {
            string Value = @"select value =STUFF(((SELECT DISTINCT  ',' + A.fn 
                                         FROM      ulogin A
                                         WHERE A.[user_name] = '" + curruser() + "' FOR XML PATH(''))), 1, 1, '')";
            //string Value = @"select * from(select value from dbo.FNC_SPLIT((SELECT fn from ulogin where [user_name]='" + curruser() + "'),','))#a";
            foreach (DataRow rs in builditems(Value).Rows) {
                //foreach (DataRow rs2 in builditems(@"select fn from TransApprove where MatDoc=" + keys + " and fn='" + rs["value"] + "' and StatusApp in (0,2)").Rows) {
                return string.Format("{0}", rs["value"]);
                //}
            }
            return "";
        }
        public static void Delete_UnusedJob(AppObject_REQUEST param)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spDeleteUnusedJob";
                cmd.Parameters.AddWithValue("@Id", param.data.ID);
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
            }
        }
        public static void sendemail(string MailTo, string MailCc, string _Body, string _Subject, string _Attachments)
        {


            try // aof added try catach
            {
                insertsendmail(MailTo, MailCc, _Body, _Subject);
                MailMessage msg = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                if (string.IsNullOrEmpty(MailTo)) return;
                string[] words = MailTo.Split(';');
                foreach (string word in words)
                {
                    if (!string.IsNullOrEmpty(word))
                        msg.To.Add(new MailAddress(word));
                }
                List<string> myList = new List<string>();
                string[] c = MailCc.Split(';');
                foreach (string s in c)
                    if (!string.IsNullOrEmpty(s))
                    {
                        msg.CC.Add(new MailAddress(s));
                        myList.Add(s);
                    }

                msg.From = new MailAddress("wshuttleadm@thaiunion.com");
                msg.Subject = string.Format("{0}", _Subject);
                msg.Body = _Body;
                if (!string.IsNullOrEmpty(_Attachments))
                {
                    msg.Attachments.Add(new Attachment(_Attachments));
                }
                msg.IsBodyHtml = true;
                //smtp.Host = "192.168.1.38";
                smtp.Host = string.Format("{0}", ConfigurationManager.AppSettings["SMTPServer"]);
                smtp.Port = 25;
                smtp.Send(msg);
                smtp.Dispose();
            }
            catch (Exception ex)
            {

            }

             
        }
        public static string GetActiveBy(String Value)
        {
            List<string> list = new List<string>();
            using (SqlConnection oConn = new SqlConnection(strConn))
            {
                oConn.Open();
                string strQuery = "select * from " + Value;
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, oConn);
                // Fill the dataset.
                oAdapter.Fill(dt);
                oConn.Close();
                foreach (DataRow rw in dt.Rows)
                {
                    if (!list.Contains(string.Format("{0}", rw["ActiveBy"])))
                        list.Add(string.Format("{0}", rw["ActiveBy"]));
                }
            }
            return String.Join(";", list.ToArray());
        }
        public static void master_artwork(string[] name)
        {
            //loop details
            CHARACTERISTICS list = new CHARACTERISTICS();
            List<CHARACTERISTIC> iGrid_CHARACTERISTICS = new List<CHARACTERISTIC>();
            CHARACTERISTIC item = new CHARACTERISTIC();
            item.NAME = name[0].ToString();
            item.DESCRIPTION = name[1].ToString();
            if (name[0].ToString() == "ZPKG_SEC_BRAND")
            {
                item.VALUE = name[2].ToString();
            }
            else
                item.VALUE = name[1].ToString();

            item.ID = name[2].ToString();
            item.Old_ID = string.Format("{0}", name[4]);
            item.Changed_Action = string.Format("{0}", name[3]);
            iGrid_CHARACTERISTICS.Add(item);
            list.Characteristics = iGrid_CHARACTERISTICS;
            //MM72_OUTBOUND_MATERIAL_CHARACTERISTIC matNumber = new MM72_OUTBOUND_MATERIAL_CHARACTERISTIC();
            SERVICE_RESULT_MODEL resp = new SERVICE_RESULT_MODEL();
            //MM72Client client = new MM72Client();

            //matNumber.param = list;
            resp = MM_72_Hepler.SaveCharacteristics(list);
            //++++++++++++++++++++++++++++++++

            string datapath = "~/FileTest/master" + name[0].ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
            //using (FileStream fs = new FileStream(Server.MapPath(datapath), FileMode.Create))
            //{
            //    new XmlSerializer(typeof(myService.CHARACTERISTICS)).Serialize(fs, list);
            //}

            string sendemailmaster = ConfigurationManager.AppSettings["SendEmailMaster"];
            sendemail(sendemailmaster, "",
                string.Format("Name : {0}<br/>Status: {1},<br/>msg: {2}", name[0].ToString(), resp.status, resp.msg),
                string.Format("Master {0} is completely created in SAP and Artwork", name[1].ToString()), (!File.Exists(datapath)) ? "" : System.Web.HttpContext.Current.Server.MapPath(datapath));
        }
        static Checkpath GetInfo(string path)
        {
            Checkpath checkpath = new Checkpath();
            try
            {
                FileAttributes attr = File.GetAttributes(path);

                //detect whether its a directory or file  
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    checkpath.type = Filetype.Dir;
                else
                    checkpath.type = Filetype.File;
                checkpath.Ifexists = true;
            }
            catch
            {
                bool t = System.IO.Path.HasExtension(path);
                if (t)
                {
                    checkpath.type = Filetype.File;
                }
                else
                {
                    checkpath.type = Filetype.Dir;
                }

                checkpath.Ifexists = false;
            }
            return checkpath;
        }
        public static string GetModulEmail(String user_name) {
            List<string> list = new List<string>();
            string Str_X = "X";
            string[] celda = user_name.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in celda)
            {
                string strQuery = @"select * from ulogin where [user_name] ='" + s + "' and ([Inactive] <>'" + Str_X + "' or [Inactive] is null)";
                using (SqlConnection oConn = new SqlConnection(strConn))
                {
                    oConn.Open();

                    DataTable dt = new DataTable();
                    SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, oConn);
                    oAdapter.Fill(dt);
                    oConn.Close();
                    foreach (DataRow rw in dt.Rows)
                    {
                        if (!list.Contains(string.Format("{0}", rw["Email"])))
                            list.Add(string.Format("{0}", rw["Email"]));
                    }
                }
            }
            return String.Join(";", list.ToArray());
        }

        public static int uploadFileIGridSAPMaterial(string name, string contenttype, Stream data, int matdoc, string activeby)
        {
            int id = 0;
            using (SqlConnection CN = new SqlConnection(strConn))
            {
                string qry = "insert into tblFiles2 values (@Name,@ContentType,@Data,@MatDoc,@ActiveBy)";
                SqlCommand SqlCom = new SqlCommand(qry, CN);
                //We are passing Original File Path and file byte data as sql parameters.
                SqlCom.Parameters.Add(new SqlParameter("@Name", name));
                SqlCom.Parameters.Add(new SqlParameter("@ContentType", contenttype));
                SqlCom.Parameters.Add(new SqlParameter("@Data", data));
                SqlCom.Parameters.Add(new SqlParameter("@MatDoc", matdoc));
                SqlCom.Parameters.Add(new SqlParameter("@ActiveBy", activeby));
                //Open connection and execute insert query.
                CN.Open();
                SqlCom.ExecuteNonQuery();

                string strQuery = @"select top 1 id from tblFiles2 where matdoc =" + matdoc + " order by id desc";

                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, CN);
                oAdapter.Fill(dt);
                if (dt != null && dt.Rows.Count > 0)
                {
                    id = Convert.ToInt32(dt.Rows[0]["id"]);
                }

                CN.Close();
            }
            return id;
        }

        public static void saveActive(AppObject_REQUEST param)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spupdateApprove";
                cmd.Parameters.AddWithValue("@ActiveBy", curruser());
                cmd.Parameters.AddWithValue("@Id", param.data.ID);
                cmd.Parameters.AddWithValue("@fn", param.data.fn);
                cmd.Parameters.AddWithValue("@StatusApp", param.data.StatusApp);
                cmd.Parameters.AddWithValue("@Remark", param.data.Remark);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }
        public static void savechangeresult(string Name,string Result,string Matdoc,String Activeby)
        {

            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spSaveChangeResult";
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.Parameters.AddWithValue("@Result", Result);
                cmd.Parameters.AddWithValue("@MatDoc", Matdoc);
                cmd.Parameters.AddWithValue("@ActiveBy", Activeby);
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
            }
        }
        public static bool ForLoop(string[] firstArray, string[] secondArray)
        {
            if (firstArray.Length != secondArray.Length)
                return false;

            for (int i = 0; i < firstArray.Length; i++)
            {
                if (firstArray[i] != secondArray[i])
                    return false;
            }

            return true;
        }
        public static DataTable saveCompleteInfoGroup(SapMaterial_REQUEST param)
        {
            // created by aof
            DataTable table = new DataTable();
            if (param.data.Id > 0  )
            {

                executeScript("Update SapMaterial set FinalInfoGroup = N'"+ param.data.FinalInfoGroup +"' Where Id=" + param.data.Id);
                if (param.data.IsSaveCompleteInfoGroup == "X")
                {
                    SqlParameter[] p = { new SqlParameter("@Id", param.data.Id),
                    new SqlParameter("@InfoGroup","0"),
                    new SqlParameter("@user",string.Format("{0}",curruser())),
                    new SqlParameter("@Check_PChanged","False"),
                    };
                    table = executeProcedure("spsaveInfoGroup", p);
                    table = builditems(@"select * from sapmaterial where id=" + string.Format("{0}", param.data.Id));
                    foreach (DataRow rm in table.Rows)
                    {
                        string str_material = string.Format("{0}", rm["Material"]);


                        string Subject = "SEC PKG Info already saved PKG Material no.: " + str_material + " /" + rm["Description"] + " / ";
                        string Value = "TransApprove Where MatDoc='" + rm["Id"] + "' and fn in ('PA','PG','PA_Approve','PG_Approve')";
                        string MailTo = GetModulEmail(GetActiveBy(Value));
                        sendemail(MailTo, GetModulEmail(string.Format("{0}", CNService.curruser())), "SEC PKG Info already saved PKG Material no.: " + str_material + "<br /><br />E-Mail Material Info already saved" +
                                                        "<br/>Comment : " + param.data.Remark, Subject, "");
                    }
                }else
                    table = builditems(@"select * from sapmaterial where id=" + string.Format("{0}", param.data.Id));
                
            }
            return table;
        }
        public static DataTable updateArtworkNumber(SapMaterial_REQUEST param)
        {
            DataTable table = new DataTable();
            if (param.data.Id == 0) {
                SqlParameter[] p = { new SqlParameter("@Code", string.Format("{0}", param.data.Material == "" || param.data.Material == null ? param.data.ReferenceMaterial : param.data.Material)),
                new SqlParameter("@Condition",string.Format("{0}",param.data.Condition=="" || param.data.Condition == null ? "1" : param.data.Condition)),
                new SqlParameter("@CreateBy",string.Format("{0}",curruser()))};
                table = executeProcedure("spCreateDocument", p);
            }
            else
            {
                table = builditems(@"select * from sapmaterial where id=" + string.Format("{0}", param.data.Id));
            
            foreach (DataRow dr in table.Rows)
            {
                param.data.Id = Convert.ToInt32(dr["ID"]);
                param.data.DocumentNo = string.Format("{0}", dr["DocumentNo"]);
                    using (SqlConnection con = new SqlConnection(strConn))
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "spInsertMultipleRows";
                        cmd.Parameters.AddWithValue("@Description", string.Format("{0}", param.data.Description));
                        cmd.Parameters.AddWithValue("@Brand", string.Format("{0}", param.data.Brand));
                        cmd.Parameters.AddWithValue("@Primarysize", string.Format("{0}", param.data.PrimarySize));
                        cmd.Parameters.AddWithValue("@Primarysize_id", string.Format("{0}", param.data.PrimarySize_id));
                        cmd.Parameters.AddWithValue("@Version", string.Format("{0}", param.data.Version));
                        cmd.Parameters.AddWithValue("@ChangePoint", string.Format("{0}", param.data.ChangePoint));
                        cmd.Parameters.AddWithValue("@MaterialGroup", string.Format("{0}", param.data.MaterialGroup));
                        cmd.Parameters.AddWithValue("@CreateBy", string.Format("{0}", curruser())); // param.data.CreateBy));  'edit by aof
                        cmd.Parameters.AddWithValue("@RequestNo", string.Format("{0}", param.data.Id));
                        cmd.Parameters.AddWithValue("@userlevel", GetApproveLevel(string.Format("{0}", param.data.Id)));
                        cmd.Parameters.AddWithValue("@PackingStyle", string.Format("{0}", param.data.PackingStyle));
                        cmd.Parameters.AddWithValue("@Packing", string.Format("{0}", param.data.Packing));
                        cmd.Parameters.AddWithValue("@StyleofPrinting", string.Format("{0}", param.data.StyleofPrinting));
                        cmd.Parameters.AddWithValue("@ContainerType", string.Format("{0}", param.data.ContainerType));
                        cmd.Parameters.AddWithValue("@LidType", string.Format("{0}", param.data.LidType));
                        cmd.Parameters.AddWithValue("@TotalColour", string.Format("{0}", param.data.Totalcolour));
                        cmd.Parameters.AddWithValue("@StatusApp", string.Format("{0}", 0));
                        cmd.Parameters.AddWithValue("@ProductCode", string.Format("{0}", param.data.ProductCode));
                        cmd.Parameters.AddWithValue("@FAOZone", string.Format("{0}", param.data.FAOZone));
                        //cmd.Parameters.AddWithValue("@Plant", string.Format("{0}", _artworkObject.Plant.Replace(',', ';')));
                        cmd.Parameters.AddWithValue("@Plant", string.Format("{0}", param.data.Plant));
                        cmd.Parameters.AddWithValue("@Processcolour", string.Format("{0}", param.data.Processcolour));
                        cmd.Parameters.AddWithValue("@PlantRegisteredNo", string.Format("{0}", param.data.PlantRegisteredNo));
                        cmd.Parameters.AddWithValue("@CompanyNameAddress", string.Format("{0}", param.data.CompanyNameAddress));
                        cmd.Parameters.AddWithValue("@PMScolour", string.Format("{0}", param.data.PMScolour));
                        cmd.Parameters.AddWithValue("@Symbol", string.Format("{0}", param.data.Symbol));
                        cmd.Parameters.AddWithValue("@CatchingArea", string.Format("{0}", param.data.CatchingArea));
                        cmd.Parameters.AddWithValue("@CatchingPeriodDate", string.Format("{0}", param.data.CatchingPeriodDate));
                        cmd.Parameters.AddWithValue("@Grandof", string.Format("{0}", param.data.Grandof));
                        cmd.Parameters.AddWithValue("@Flute", string.Format("{0}", param.data.Flute));
                        cmd.Parameters.AddWithValue("@Vendor", string.Format("{0}", param.data.Vendor));
                        cmd.Parameters.AddWithValue("@Dimension", string.Format("{0}", param.data.Dimension));
                        cmd.Parameters.AddWithValue("@RSC", string.Format("{0}", param.data.RSC));
                        cmd.Parameters.AddWithValue("@Accessories", string.Format("{0}", param.data.Accessories));
                        cmd.Parameters.AddWithValue("@PrintingStyleofPrimary", string.Format("{0}", param.data.PrintingStyleofPrimary));
                        cmd.Parameters.AddWithValue("@PrintingStyleofSecondary", string.Format("{0}", param.data.PrintingStyleofSecondary));
                        cmd.Parameters.AddWithValue("@CustomerDesign", string.Format("{0}|{1}", param.data.CustomerDesign, param.data.CustomersDesignDetail));
                        cmd.Parameters.AddWithValue("@CustomerSpec", string.Format("{0}|{1}", param.data.CustomerSpec, param.data.CustomersSpecDetail));
                        cmd.Parameters.AddWithValue("@CustomerSize", string.Format("{0}|{1}", param.data.CustomerSize, param.data.CustomersSizeDetail));
                        cmd.Parameters.AddWithValue("@CustomerVendor", string.Format("{0}|{1}", param.data.CustomerVendor, param.data.CustomerNominatesVendorDetail));
                        cmd.Parameters.AddWithValue("@CustomerColor", string.Format("{0}|{1}", param.data.CustomerColor, param.data.CustomerNominatesColorPantoneDetail));
                        cmd.Parameters.AddWithValue("@CustomerScanable", string.Format("{0}|{1}", param.data.CustomerScanable, param.data.CustomersBarcodeScanableDetail));
                        cmd.Parameters.AddWithValue("@CustomerBarcodeSpec", string.Format("{0}|{1}", param.data.CustomerBarcodeSpec, param.data.CustomersBarcodeSpecDetail));

                        //cmd.Parameters.AddWithValue("@CustomerDesign", string.Format("{0}", param.data.CustomerDesign));
                        //cmd.Parameters.AddWithValue("@CustomerSpec", string.Format("{0}", param.data.CustomerSpec));
                        //cmd.Parameters.AddWithValue("@CustomerSize", string.Format("{0}", param.data.CustomerSize));
                        //cmd.Parameters.AddWithValue("@CustomerVendor", string.Format("{0}", param.data.CustomerVendor));
                        //cmd.Parameters.AddWithValue("@CustomerColor", string.Format("{0}", param.data.CustomerColor));
                        //cmd.Parameters.AddWithValue("@CustomerScanable", string.Format("{0}", param.data.CustomerScanable));
                        //cmd.Parameters.AddWithValue("@CustomerBarcodeSpec", string.Format("{0}", param.data.CustomerBarcodeSpec));
                        cmd.Parameters.AddWithValue("@FirstInfoGroup", string.Format("{0}", param.data.FirstInfoGroup));
                        cmd.Parameters.AddWithValue("@SO", string.Format("{0}", param.data.SO));
                        cmd.Parameters.AddWithValue("@PICMkt", string.Format("{0}", param.data.PICMkt));
                        cmd.Parameters.AddWithValue("@SOPlant", string.Format("{0}", param.data.SOPlant));
                        cmd.Parameters.AddWithValue("@Destination", string.Format("{0}", param.data.Destination));
                        cmd.Parameters.AddWithValue("@Remark", string.Format("{0}", param.data.Remark));
                        cmd.Parameters.AddWithValue("@GrossWeight", string.Format("{0}", param.data.GrossWeight));
                        cmd.Parameters.AddWithValue("@FinalInfoGroup", string.Format("{0}", param.data.FinalInfoGroup));
                        cmd.Parameters.AddWithValue("@Note", string.Format("{0}", param.data.Note));
                        cmd.Parameters.AddWithValue("@SheetSize", string.Format("{0}", param.data.SheetSize));
                        cmd.Parameters.AddWithValue("@Typeof", string.Format("{0}", param.data.Typeof));
                        cmd.Parameters.AddWithValue("@TypeofCarton2", string.Format("{0}", param.data.TypeofCarton2));
                        cmd.Parameters.AddWithValue("@DMSNo", string.Format("{0}", param.data.DMSNo));

                        cmd.Parameters.AddWithValue("@TypeofPrimary", string.Format("{0}", param.data.TypeofPrimary));
                        cmd.Parameters.AddWithValue("@PrintingSystem", string.Format("{0}", param.data.PrintingSystem));
                        cmd.Parameters.AddWithValue("@Direction", string.Format("{0}", param.data.Direction));
                        cmd.Parameters.AddWithValue("@RollSheet", string.Format("{0}", param.data.RollSheet));
                        cmd.Parameters.AddWithValue("@RequestType", string.Format("{0}", param.data.RequestType));
                        cmd.Parameters.AddWithValue("@PlantAddress", string.Format("{0}", param.data.PlantAddress));

                        cmd.Parameters.AddWithValue("@Fixed_Desc", string.Format("{0}", param.data.Fixed_Desc));
                        cmd.Parameters.AddWithValue("@Inactive", string.Format("{0}", param.data.Inactive));
                        cmd.Parameters.AddWithValue("@Catching_Method", string.Format("{0}", param.data.Catching_Method));
                        cmd.Parameters.AddWithValue("@Scientific_Name", string.Format("{0}", param.data.Scientific_Name));
                        cmd.Parameters.AddWithValue("@Specie", string.Format("{0}", param.data.Specie));

                        cmd.Parameters.AddWithValue("@SustainMaterial", string.Format("{0}", param.data.SustainMaterial));
                        cmd.Parameters.AddWithValue("@SustainPlastic", string.Format("{0}", param.data.SustainPlastic));
                        cmd.Parameters.AddWithValue("@SustainReuseable", string.Format("{0}", param.data.SustainReuseable));
                        cmd.Parameters.AddWithValue("@SustainRecyclable", string.Format("{0}", param.data.SustainRecyclable));
                        cmd.Parameters.AddWithValue("@SustainComposatable", string.Format("{0}", param.data.SustainComposatable));
                        cmd.Parameters.AddWithValue("@SustainCertification", string.Format("{0}", param.data.SustainCertification));
                        cmd.Parameters.AddWithValue("@SustainCertSourcing", string.Format("{0}", param.data.SustainCertSourcing));
                        cmd.Parameters.AddWithValue("@SustainOther", string.Format("{0}", param.data.SustainOther));
                        cmd.Parameters.AddWithValue("@SusSecondaryPKGWeight", string.Format("{0}", param.data.SusSecondaryPKGWeight));
                        cmd.Parameters.AddWithValue("@SusRecycledContent", string.Format("{0}", param.data.SusRecycledContent));
                        cmd.Connection = con;
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                    }
                }
               
            }
            return table;
        }
        public static string OutboundArtwork(string Keys)
        {
            //master_artwork();
            //header
            IGRID_OUTBOUND_MODEL iGrid_Model = new IGRID_OUTBOUND_MODEL();
            //myh.OUTBOUND_HEADERS = new ServiceReference.IGRID_OUTBOUND_HEADER_MODEL;

            IGRID_OUTBOUND_HEADER_MODEL result = new IGRID_OUTBOUND_HEADER_MODEL();
            List<IGRID_OUTBOUND_HEADER_MODEL> iGrid_Header_List = new List<IGRID_OUTBOUND_HEADER_MODEL>();

            IGRID_OUTBOUND_MODEL matNumber = new IGRID_OUTBOUND_MODEL();
            SERVICE_RESULT_MODEL resp = new SERVICE_RESULT_MODEL();
            try
            {
                var _table = builditems("select *,case when statusapp=4 then 'Completed' when statusapp=5 then 'Canceled' end as 'Status' from SapMaterial Where DocumentNo='" + Keys + "'");
                string _ArtworkNumber = "", _Date = "", _Time = "", _Material = "", _PAUserName = "", _Subject = "Material {0} data send to Artwork Complete";

                foreach (DataRow dr in _table.Rows)
                {
                    if (dr["StatusApp"].ToString() == "5")
                        _Subject = "Cancel form iGrid and send info to Artwork Complete";
                    _ArtworkNumber = string.Format("{0}", dr["DMSNo"]);
                    _Date = String.Format("{0:yyyyMMdd}", dr["CreateOn"]);
                    _Time = String.Format("{0:HH:mm:ss}", dr["CreateOn"]); //"10:22:03";
                    _Material = string.Format("{0}", dr["Material"]);
                    _PAUserName = string.Format("{0}", dr["CreateBy"]);
                    DataTable _dt = builditems(@"select isnull(url,'')url,isnull(ReferenceMaterial,'')ReferenceMaterial from TransArtworkURL where Matdoc="
                    + string.Format("{0}", dr["Id"]));
                    if (_dt.Rows.Count > 0)
                    {
                        DataRow r = _dt.Rows[0];
                        result.ArtworkURL = string.Format("{0}", r["url"]);//"http://artwork.thaiunion.com/content/aw-file.pdf";
                        result.ReferenceMaterial = string.Format("{0}", r["ReferenceMaterial"]);
                    }
                    else
                    {
                        result.ArtworkURL = "";
                        result.ReferenceMaterial = "";
                    }
                    result.ArtworkNumber = _ArtworkNumber;
                    result.Date = _Date;
                    result.Time = _Time; //"10:22:03";
                    result.RecordType = "I";
                    result.MaterialNumber = dr["StatusApp"].ToString() == "5" ? "" : string.Format("{0}", dr["Material"]);
                    result.MaterialDescription = string.Format("{0}", dr["Description"]); //"CTN3 - 60960,LUCKY";
                    result.ChangePoint = string.Format("{0}", dr["ChangePoint"]) == "C" ? "1" : "0";
                    result.MaterialCreatedDate = String.Format("{0:yyyyMMdd}", dr["ModifyOn"]);
                    result.Status = dr["Status"].ToString();
                    result.PAUserName = string.Format("{0}", dr["CreateBy"]);
                    result.PGUserName = string.Format("{0}", dr["Assignee"]);
                    //            result.Plant = string.Format("{0}", dr["Plant"].ToString().Replace(';',','));
                    result.Plant = string.Format("{0}", dr["Plant"].ToString());
                    result.PrintingStyleofPrimary = string.Format("{0}", dr["PrintingStyleofPrimary"]);
                    result.PrintingStyleofSecondary = string.Format("{0}", dr["PrintingStyleofSecondary"]);

                    //string CustomerDesign = string.Format("{0}", dr["CustomerDesign"]);
                    //string[] words = CustomerDesign.Split('|');
                    result.CustomersDesign = splittext(dr["CustomerDesign"].ToString(), 0);
                    result.CustomersDesignDetail = splittext(dr["CustomerDesign"].ToString(), 1);

                    result.CustomersSpec = splittext(dr["CustomerSpec"].ToString(), 0);
                    result.CustomersSpecDetail = CNService.splittext(dr["CustomerSpec"].ToString(), 1);
                    result.CustomersSize = splittext(dr["CustomerSize"].ToString(), 0);
                    result.CustomersSizeDetail = splittext(dr["CustomerSize"].ToString(), 1);
                    result.CustomerNominatesVendor = splittext(dr["CustomerVendor"].ToString(), 0);
                    result.CustomerNominatesVendorDetail = splittext(dr["CustomerVendor"].ToString(), 1);
                    result.CustomerNominatesColorPantone = splittext(dr["CustomerColor"].ToString(), 0);
                    result.CustomerNominatesColorPantoneDetail = splittext(dr["CustomerColor"].ToString(), 1);
                    result.CustomersBarcodeScanable = splittext(dr["CustomerScanable"].ToString(), 0);
                    result.CustomersBarcodeScanableDetail = splittext(dr["CustomerScanable"].ToString(), 1);
                    result.CustomersBarcodeSpec = splittext(dr["CustomerBarcodeSpec"].ToString(), 0);
                    result.CustomersBarcodeSpecDetail = splittext(dr["CustomerBarcodeSpec"].ToString(), 1);
                    result.FirstInfoGroup = string.Format("{0}", dr["FirstInfoGroup"]);
                    result.SONumber = string.Format("{0}", dr["SO"]);
                    result.SOitem = "";
                    result.SOPlant = string.Format("{0}", dr["SOPlant"]);
                    result.PICMKT = string.Format("{0}", dr["PICMkt"]);
                    result.Destination = string.Format("{0}", dr["Destination"]);
                    result.RemarkNoteofPA = string.Format("{0}", dr["Remark"]);
                    result.FinalInfoGroup = string.Format("{0}", dr["FinalInfoGroup"]);
                    result.RemarkNoteofPG = "";
                    result.CompleteInfoGroup = "";
                    result.ProductionExpirydatesystem = "";
                    result.Seriousnessofcolorprinting = "";
                    result.CustIngreNutritionAnalysis = "";
                    result.ShadeLimit = "";
                    result.PackageQuantity = "";
                    result.WastePercent = "";
                    iGrid_Header_List.Add(result);
                    iGrid_Model.OUTBOUND_HEADERS = iGrid_Header_List;
                }
                List<IGRID_OUTBOUND_ITEM_MODEL> iGrid_Item_List = new List<IGRID_OUTBOUND_ITEM_MODEL>();
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(strConn))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spInboundArtwork";
                    cmd.Parameters.AddWithValue("@Keys", string.Format("{0}", Keys.ToString()));
                    cmd.Connection = con;
                    con.Open();
                    SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                    oAdapter.Fill(dt);
                    con.Close();
                    List<InboundArtwork> _itemsArtwork = new List<InboundArtwork>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var detail = new IGRID_OUTBOUND_ITEM_MODEL();
                        DataRow dr = dt.Rows[i];

                        detail.ArtworkNumber = string.Format("{0}", _ArtworkNumber);
                        detail.Date = _Date;
                        detail.Time = _Time;
                        detail.Characteristic = dr["cols"].ToString();
                        //detail.Description = dr["Description"].ToString();
                        //detail.Value = dr["value"].ToString();
                        string[] splitHeader = dr["value"].ToString().Split(';');
                        if (splitHeader != null && splitHeader.Length > 1)
                            foreach (string word in splitHeader)
                            {
                                detail = new IGRID_OUTBOUND_ITEM_MODEL();
                                detail.ArtworkNumber = string.Format("{0}", _ArtworkNumber);
                                detail.Date = _Date;
                                detail.Time = _Time;
                                detail.Characteristic = dr["cols"].ToString();
                                detail.Value = word.ToString();
                                detail.Description = detail.Value.ToString();
                                iGrid_Item_List.Add(detail);
                            }
                        else
                        {
                            detail.Description = dr["Description"].ToString();
                            detail.Value = dr["value"].ToString();
                            iGrid_Item_List.Add(detail);
                        }
                    }
                }
                iGrid_Model.OUTBOUND_ITEMS = iGrid_Item_List;
                //MM_73_Hepler.SaveMaterial client = new MM_73_Hepler.SaveMaterial();
                //matNumber.param = iGrid_Model;
                //resp = client.MATERIAL_NUMBER(matNumber);
                string Start = DateTime.Now.ToString();
                resp = MM_73_Hepler.SaveMaterial(iGrid_Model);
                string dtEnd = DateTime.Now.ToString();
                //Context.Response.Write(JsonConvert.SerializeObject(resp));
                //DirectoryInfo dir = new DirectoryInfo(@"\\SERVER\Data\");
                
                string datapath = @"\\192.168.1.170\FileTest\iGrid_Model" + Keys.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
                FileInfo myFile = new FileInfo(datapath);
                using (FileStream fs = new FileStream(datapath, FileMode.Create))
                {
                    new XmlSerializer(typeof(IGRID_OUTBOUND_MODEL)).Serialize(fs, iGrid_Model);
                }
                Checkpath check1 = GetInfo(datapath);
                SqlParameter[] param = { new SqlParameter("@keys", string.Format("{0}", Keys)) };

                executeProcedure("spupdateOutbound", param);
                string MailCc = CNService.GetModulEmail(CNService.Getusermail("PA_Approve"));
                sendemail(Getuser(_PAUserName, "email"), MailCc,
                    string.Format("Artwork Number {3} <br/> Workflow IGrid {0} <br/> Status: {1},<br/>msg: {2} <br/>Start Time: {4}<br/>End Time: {5}", Keys.ToString(), resp.status, resp.msg, _ArtworkNumber, Start, dtEnd),
                    string.Format(_Subject, _Material.ToString()), datapath);
                return resp.msg;
            }
            catch (Exception e)
            {
                sendemail("Nongrat.Jantarasuwan@thaiunion.com;Pornpimon.Bouban@thaiunion.com", "",
                string.Format("{0}", e.Message), string.Format("iGrid can't send to Artwork , status fail , iGrid No : {0}", Keys.Substring(0, 16)), "");
                return e.Message;
                // Action after the exception is caught  
            }
        }
        public static List<PackStyle_MODEL> GetPackStyle(PackStyle_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from MasPackingStyle";
                if (ro != null)
                {
                    string where = "";
                    //strQuery = strQuery + " where TypeofPrimary " +
                    //    "= (case when N'" + ro.data.TypeofPrimary + "' <> '' then N'" + ro.data.TypeofPrimary + "' else TypeofPrimary end )";
                    if (ro != null && ro.data != null)
                    {
                        if (!string.IsNullOrEmpty(ro.data.TypeofPrimary))
                        {
                            where = "TypeofPrimary " + "= (case when N'" + ro.data.TypeofPrimary + "' <> '' then N'" + ro.data.TypeofPrimary + "' else TypeofPrimary end )";
                        }
                        if (ro.data.IsCheckAuthorize == null)
                        {
                            //where = where + where == ""? " isnull(inactive,'') <> 'X'" :  " and isnull(inactive,'') <> 'X'";   commmeted by aof
                            if (where != "") where = where + " and ";
                            where = where + "isnull(inactive,'') <> 'X'";
                        } else
                        {
                            if (ro.data.IsCheckAuthorize == "X")
                            {
                                if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster))
                                {
                                    if (where != "") where = where + " and ";
                                    where = where + "isnull(inactive,'') <> 'X'";
                                }
                            }
                        }
                        if (where != "")
                        {
                            strQuery = strQuery + " where " + where;
                        }

                    }
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                return (from DataRow dr in dt.Rows
                        select new PackStyle_MODEL()
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            PrimaryCode = dr["PrimaryCode"].ToString(),
                            GroupStyle = string.Format("{0}", dr["GroupStyle"]),
                            PackingStyle = string.Format("{0}", dr["PackingStyle"]),
                            RefStyle = string.Format("{0}", dr["RefStyle"]),
                            PackSize = string.Format("{0}", dr["PackSize"]),
                            BaseUnit = string.Format("{0}", dr["BaseUnit"]),
                            TypeofPrimary = string.Format("{0}", dr["TypeofPrimary"]),
                            Inactive = dr["Inactive"].ToString(),
                        }).ToList();
            }
        }
        public static List<Attachment_MODEL> GetAttachment(Attachment_REQUEST param )
        {
            List<Attachment_MODEL> listAttachment = new List<Attachment_MODEL>();
            using (SqlConnection con = new SqlConnection(strConn))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    param.data.MatDoc = 261;
                    //Response.Write (Request.QueryString["MatDoc"]);
                    string strQuery = "select Id,ContentType, Name from tblFiles2 where MatDoc=" + param.data.MatDoc;
                    cmd.Connection = con;
                    con.Open();
                    DataTable dt = new DataTable();
                    SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                    // Fill the dataset.
                    oAdapter.Fill(dt);
                    con.Close();
                    return (from DataRow dr in dt.Rows
                            select new Attachment_MODEL()
                            {
                                Id = Convert.ToInt32(dr["id"]),
                                Name = string.Format("{0}", dr["Name"]),
                                ContentType = string.Format("{0}", dr["ContentType"]),
                            }).ToList();
                }
            }
        }
        public static List<TypeofPrimary_MODEL> GetTypeofPrimary(TypeofPrimary_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select *, CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasTypeofPrimary";
                if (ro != null)
                {
                    //strQuery = strQuery + " where MaterialGroup " +
                    //"= (case when N'" + ro.data.MaterialGroup + "' <> '' then N'" + ro.data.MaterialGroup + "' else MaterialGroup end )";
                    if (ro != null && ro.data != null)
                    {
                        if (ro.data.IsCheckAuthorize == null)
                        {
                            strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                        }
                        else
                        {
                            if (ro.data.IsCheckAuthorize == "X")
                                if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                        }

                    }            
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                   

                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var list =  (from DataRow dr in dt.Rows
                        select new TypeofPrimary_MODEL()
                        {
                            ID = string.Format("{0}", dr["Id"]),
                            DISPLAY_TXT = dr["Description"].ToString(),// + dr["inactive_text"].ToString(),
                            //MaterialGroup = dr["MaterialGroup"].ToString(),
                            Inactive =string.Format("{0}", dr["Inactive"])

                        }).ToList();

                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    list = list.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }

                return list;

            }
        }
        public static List<Brand_MODEL> GetBrand(Brand_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select * from MasBrand";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                return (from DataRow dr in dt.Rows
                        select new Brand_MODEL()
                        {
                            ID = string.Format("{0}", dr["Id"]),
                            DISPLAY_TXT = dr["Description"].ToString(),
                            Inactive = string.Format("{0}", dr["Inactive"])
                        }).ToList();
            }
        }

        public static List<Brand_MODEL> GetBrand2(Brand_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select *,CASE WHEN isnull(inactive,'') = 'X' THEN '-XXX Do Not Use XXX' ELSE ''END as inactive_text from MasBrand";
                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                }
                else
                {
                    strQuery = strQuery + " where isnull(inactive,'') <> 'X'";

                }
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                var data = (from DataRow dr in dt.Rows
                        select new Brand_MODEL()
                        {
                            ID = string.Format("{0}", dr["Id"]),
                            DISPLAY_TXT = string.Format("{0}", dr["Id"]) + "," + dr["Description"].ToString(),// + dr["inactive_text"].ToString(),
                            Inactive = string.Format("{0}", dr["Inactive"])
                        }).ToList();


              


                if (ro != null && ro.data != null && !string.IsNullOrEmpty(ro.data.DISPLAY_TXT))
                {
                    data = data.Where(w => w.DISPLAY_TXT.ToLower().Contains(ro.data.DISPLAY_TXT.ToLower())).ToList();
                }



                return data;
            }
        }

        public static List<PrimarySize_MODEL> GetPrimarySize(PrimarySize_REQUEST ro, string whereQuery)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select [Id]
                                          ,[Code]
                                          ,[Can]
                                          ,[Description]
                                          ,[LidType]
                                          ,[ContainerType]
                                          ,[DescriptionType]
                                          ,isnull(Inactive,'')Inactive from MasPrimarySize";
                string where = "";


                if (ro != null && ro.data != null && ro.data.IsCheckAuthorize == "X")
                {
                    // if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) strQuery = strQuery + " where isnull(inactive,'') <> 'X'";
                    if (string.IsNullOrEmpty(ro.data.Authorize_ChangeMaster)) where = "(isnull(inactive,'') <> 'X') and code <>''";
                }
                else
                {
                    where = "(isnull(inactive,'') <> 'X') and code <>''";
                }

                if (ro != null && ro.data != null)
                {

                    if (ro.data.IsTop1)
                    {
                        strQuery = @"select top 1 * from MasPrimarySize";

                        if (!string.IsNullOrEmpty(ro.data.Code))
                        {
                            if (where != "") where = where + " and ";
                            where = where + " (code = '" + ro.data.Code + "')";
                        }

                    }

                   
                }

                if (whereQuery != "")
                {
                    if (where != "")
                    {
                        where = where + " and " + whereQuery;
                    }
                    else
                    {
                        where = whereQuery;
                    }
                }
           

                if (where != "")
                {
                    strQuery = strQuery + " where " + where;
                }


                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();
                    
                
                con.Close();
                return (from DataRow dr in dt.Rows
                        select new PrimarySize_MODEL()
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Code = dr["Code"].ToString(),
                            Can = dr["Can"].ToString(),
                            Description = dr["Description"].ToString(),
                            LidType = dr["LidType"].ToString(),
                            ContainerType = string.Format("{0}", dr["ContainerType"]),
                            DescriptionType = dr["DescriptionType"].ToString(),
                            Inactive = string.Format("{0}", dr["Inactive"])
                            //ChangePoint = dr["ChangePoint"].ToString(),
                            //CreateBy = dr["CreateBy"].ToString(),
                            //MaterialGroup = string.Format("{0}", dr["MaterialGroup"])
                        }).ToList();
            }
        }



        public static string getArtworkURL(int matdoc)
        {
            var url = "";

            using (SqlConnection con = new SqlConnection(strConn))
            {
                con.Open();
                string strQuery = @"select top 1 url from TransArtworkURL where matdoc='" + matdoc + "'";

  

                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                // Fill the dataset.
                oAdapter.Fill(dt);
                con.Close();

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    url = dr["URL"].ToString();
                }

                con.Close();
       
            }


            return url;
        }


        public static List<SapMaterial_MODEL> GetSapMaterial(SapMaterial_REQUEST ro)
        {
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetSapMaterial";
                cmd.Parameters.AddWithValue("@Id", ro.data.Id);
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
                return (from DataRow dr in dt.Rows
                 select new SapMaterial_MODEL()
                 {  
                     Id = Convert.ToInt32(dr["ID"]),
                     DocumentNo = dr["DocumentNo"].ToString(),
                     Material = dr["Material"].ToString(),
                     Description = dr["Description"].ToString(),
                     Brand = dr["Brand"].ToString(),
                     Brand_TXT = dr["Brand_txt"].ToString(),
                     PrimarySize = string.Format("{0}", dr["PrimarySize"]),
                     Version = dr["Version"].ToString(),
                     ChangePoint = dr["ChangePoint"].ToString(),
                     CreateBy = dr["CreateBy"].ToString(),
                     MaterialGroup = string.Format("{0}", dr["MaterialGroup"]),
                     MaterialGroup_TXT = string.Format("{0}", dr["MaterialGroup_txt"]),
                     ReferenceMaterial = string.Format("{0}",dr["ReferenceMaterial"]),
                     SusRecycledContent = string.Format("{0}",dr["SusRecycledContent"]),
                     SusSecondaryPKGWeight = string.Format("{0}",dr["SusSecondaryPKGWeight"]),
                     SustainOther = string.Format("{0}",dr["SustainOther"]),
                     SustainCertSourcing = string.Format("{0}",dr["SustainCertSourcing"]),
                     SustainCertification=string.Format("{0}",dr["SustainCertification"]),
                     SustainComposatable = string.Format("{0}",dr["SustainComposatable"]),
                     SustainRecyclable = string.Format("{0}", dr["SustainRecyclable"]),
                     SustainReuseable = string.Format("{0}", dr["SustainReuseable"]),
                     SustainPlastic = string.Format("{0}", dr["SustainPlastic"]),
                     SustainMaterial = string.Format("{0}",dr["SustainMaterial"]),
                     Specie = string.Format("{0}",dr["Specie"]),
                     Scientific_Name = string.Format("{0}",dr["Scientific_Name"]),
                     Catching_Method = string.Format("{0}",dr["Catching_Method"]),
                     Inactive = string.Format("{0}",dr["Inactive"]),
                     Fixed_Desc = string.Format("{0}",dr["Fixed_Desc"]),
                     StatusApp = string.Format("{0}", dr["StatusApp"]),
                     SheetSize=string.Format("{0}", dr["SheetSize"]),
                     Assignee = string.Format("{0}", dr["Assignee"]),
                     PackingStyle=string.Format("{0}", dr["PackingStyle"]),
                     Packing = string.Format("{0}", dr["Packing"]),
                     StyleofPrinting=string.Format("{0}", dr["StyleofPrinting"]),
                     ContainerType = string.Format("{0}",dr["ContainerType"]),
                     LidType=string.Format("{0}",dr["LidType"]),
                     Condition=string.Format("{0}",dr["Condition"]),
                     ProductCode=string.Format("{0}",dr["ProductCode"]),
                     FAOZone= string.Format("{0}",dr["FAOZone"]),
                     Plant=string.Format("{0}",dr["Plant"]),
                     Totalcolour=string.Format("{0}",dr["Totalcolour"]),
                     Processcolour=string.Format("{0}",dr["Processcolour"]),
                     PlantRegisteredNo=string.Format("{0}",dr["PlantRegisteredNo"]),
                     CompanyNameAddress=string.Format("{0}",dr["CompanyNameAddress"]),
                     PMScolour=string.Format("{0}",dr["PMScolour"]),
                     Symbol=string.Format("{0}",dr["Symbol"]),
                     CatchingArea=string.Format("{0}",dr["CatchingArea"]),
                     CatchingPeriodDate=string.Format("{0}",dr["CatchingPeriodDate"]),
                     Grandof=string.Format("{0}",dr["Grandof"]),
                     Flute=string.Format("{0}",dr["Flute"]),
                     Vendor=string.Format("{0}",dr["Vendor"]),
                     Dimension=string.Format("{0}",dr["Dimension"]),
                     RSC=string.Format("{0}",dr["RSC"]),
                     Accessories=string.Format("{0}",dr["Accessories"]),
                     PrintingStyleofPrimary=string.Format("{0}",dr["PrintingStyleofPrimary"]),
                     PrintingStyleofSecondary=string.Format("{0}",dr["PrintingStyleofSecondary"]),
                     CustomerDesign=string.Format("{0}",dr["CustomerDesign"]),
                     CustomerSpec=string.Format("{0}",dr["CustomerSpec"]),
                     CustomerSize=string.Format("{0}",dr["CustomerSize"]),
                     CustomerVendor=string.Format("{0}",dr["CustomerVendor"]),
                     CustomerColor=string.Format("{0}",dr["CustomerColor"]),
                     CustomerScanable=string.Format("{0}",dr["CustomerScanable"]),
                     CustomerBarcodeSpec=string.Format("{0}",dr["CustomerBarcodeSpec"]),
                     FirstInfoGroup=string.Format("{0}",dr["FirstInfoGroup"]),
                     SO=string.Format("{0}",dr["SO"]),
                     PICMkt =string.Format("{0}",dr["PICMkt"]),
                     SOPlant=string.Format("{0}",dr["SOPlant"]),
                     Destination=string.Format("{0}",dr["Destination"]),
                     Remark=string.Format("{0}",dr["Remark"]),
                     GrossWeight=string.Format("{0}",dr["GrossWeight"]),
                     FinalInfoGroup=string.Format("{0}",dr["FinalInfoGroup"]),
                     Note=string.Format("{0}",dr["Note"]),
                     Assignee_TXT = string.Format("{0}",dr["Assignee_txt"]),
                     //Typeof_ID = Convert.ToInt32(dr["Typeof_ID"]),
                     Typeof = string.Format("{0}",dr["Typeof"]),
                     TypeofCarton2=string.Format("{0}",dr["TypeofCarton2"]),
                     DMSNo=string.Format("{0}",dr["DMSNo"]),
                     TypeofPrimary=string.Format("{0}",dr["TypeofPrimary"]),
                     PrintingSystem=string.Format("{0}",dr["PrintingSystem"]),
                     Direction=string.Format("{0}",dr["Direction"]),
                     RollSheet=string.Format("{0}",dr["RollSheet"]),
                     RequestType=string.Format("{0}",dr["RequestType"]),
                     PlantAddress=string.Format("{0}",dr["PlantAddress"]),
                     Refnumber=string.Format("{0}",dr["Refnumber"]),
                     Extended_Plant=string.Format("{0}",dr["Extended_Plant"])
                 }).ToList();
            }
        }

        public static List<ProductCode_MODEL> Getproductcode(ProductCode_REQUEST ro)
        {

            //var user = HttpContext.Current.User.Identity.Name;
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetProductCode";
                cmd.Parameters.AddWithValue("@ProductCode", string.Format("{0}", ro.data.PRODUCT_CODE));
                cmd.Parameters.AddWithValue("@address", string.Format("{0}", ro.data.Address));
                 
                cmd.Parameters.AddWithValue("@registeredNo", string.Format("{0}", ro.data.RegisteredNo));
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
                return (from DataRow dr in dt.Rows
                        select new ProductCode_MODEL()
                        {
                            prd_plant = string.Format("{0}", dr["prd_plant"]),
                            Count_RegisteredNo = Convert.ToInt32(dr["Count_RegisteredNo"]),
                            Count_Address = Convert.ToInt32(dr["count_Address"]),
                            Address = string.Format("{0}", ro.data.Address),
                            RegisteredNo = string.Format("{0}", ro.data.RegisteredNo),

                        }).ToList();
            }
        }

        public static List<IGRID_MODEL> Getinfogroup(IGRID_REQUEST ro)
        {

            //var user = HttpContext.Current.User.Identity.Name;
            using (SqlConnection con = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spinfogroup";
                cmd.Parameters.AddWithValue("@where", string.Format("{0}", ""));
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
                return (from DataRow dr in dt.Rows
                        select new IGRID_MODEL()
                        {

                            ID = Convert.ToInt32(dr["Id"]),
                            CONDITION = dr["Condition"].ToString(),
                            REQUESTTYPE = dr["RequestType"].ToString(),
                            
                            DOCUMENTNO = dr["DocumentNo"].ToString(),
                            //DMSNO = dr["DMSNo"].ToString(),
                            MATERIAL = string.Format("{0}", dr["Material"]),
                            DESCRIPTION = dr["Description"].ToString(),
                            MATERIALGROUP = string.Format("{0}", dr["MaterialGroup"]),
                            BRAND = dr["Brand"].ToString(),
                            ASSIGNEE = dr["assign"].ToString(),
                            CREATEON = dr["CreateOn"].ToString(),
                            ACTIVEBY = dr["CreateBy"].ToString(),
                            FINALINFOGROUP = dr["FinalInfoGroup"].ToString(),
                            REFERENCEMATERIAL = dr["ReferenceMaterial"].ToString(),


                            VENDER = dr["Vendor"].ToString(),
                            VENDERDESCRIPTION = dr["VendorDescription"].ToString(),

                        }).ToList();
            }
        }
        public static List<IGRID_MODEL> Getpersonal(IGRID_REQUEST ro)
        {

            //var user  = HttpContext.Current.User.Identity.Name;
            using (SqlConnection con = new SqlConnection(strConn))
            {



                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetpersonal2";
                cmd.Parameters.AddWithValue("@user", curruser());
                cmd.Parameters.AddWithValue("@where", string.Format("{0}", ""));
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();

                return (from DataRow dr in dt.Rows
                        select new IGRID_MODEL()
                        {

                            ID = Convert.ToInt32(dr["ID"]),
                            CONDITION = dr["Condition"].ToString(),
                            REQUESTTYPE = dr["RequestType"].ToString(),
                            DMSNO = dr["DMSNo"].ToString(),
                            DOCUMENTNO = dr["DocumentNo"].ToString(),
                            MATERIAL = string.Format("{0}", dr["Material"]),
                            DESCRIPTION = dr["Description"].ToString(),
                            BRAND = dr["Brand"].ToString(),
                            ACTION = dr["action"].ToString(),
                            MATERIALGROUP = string.Format("{0}-{1}", dr["MaterialGroup"], dr["MaterialGroup_t"]),
                            ASSIGNEE = dr["assign"].ToString(),
                            CREATEON = dr["CreateOn"].ToString(),
                            ACTIVEBY = dr["CreateBy"].ToString(),
                            STATUSAPP = dr["StatusApp"].ToString(),
                        }).ToList();
            }
        }
        //public static void testsendmaster(string SubChanged_Id)
        //{
        //    string strSQL = " select * from TransMaster where Changed_id ='" + SubChanged_Id + "'";
        //    int userID = -2;
        //    DataTable dt = CNService.builditems(strSQL);
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        if (string.Format("{0}", dr["Changed_Tabname"]).Equals("MasBrand"))
        //        {
        //            string _Id = dr["Id"].ToString();
        //            string _Description = dr["Description"].ToString();
        //            string[] value = { dr["Changed_Charname"].ToString(), _Description, _Id };
        //            CNService.master_artwork(value);
        //        }
        //        else if (string.Format("{0}", dr["Changed_action"]).Equals("Insert"))
        //        {
        //            if (string.Format("{0}", dr["Changed_Tabname"]).Equals("MasPackingStyle"))
        //            {
        //                DataTable dtPackingStyle = CNService.builditems(@"select * from MasPackingStyle where Id='" +
        //                    string.Format("{0}", dr["Id"])
        //                    + "'");
        //                foreach (DataRow drPackingStyle in dtPackingStyle.Rows)
        //                {
        //                    using (var context = new ARTWORKEntities())
        //                    {
        //                        SAP_M_2P sapm2p = new SAP_M_2P();
        //                        sapm2p.PACKING_SYLE_VALUE = string.Format("{0}", drPackingStyle["RefStyle"]);
        //                        sapm2p.PACKING_SYLE_DESCRIPTION = string.Format("{0}", drPackingStyle["RefStyle"]);
        //                        sapm2p.PACK_SIZE_VALUE = string.Format("{0}", drPackingStyle["PackSize"]);
        //                        sapm2p.PACK_SIZE_DESCRIPTION = string.Format("{0}", drPackingStyle["PackSize"]);
        //                        var existItem = SAP_M_2P_SERVICE.GetByItem(sapm2p, context).FirstOrDefault();
        //                        if (existItem == null)
        //                        {
        //                            sapm2p.IS_ACTIVE = "X";
        //                            sapm2p.CREATE_BY = userID;
        //                            sapm2p.CREATE_DATE = DateTime.Today;
        //                            sapm2p.UPDATE_BY = userID;
        //                            sapm2p.UPDATE_DATE = DateTime.Today;
        //                            SAP_M_2P_SERVICE.SaveOrUpdateNoLog(sapm2p, context);
        //                        }
        //                    }
        //                }
        //            }
        //            else if (string.Format("{0}", dr["Changed_Tabname"]).Equals("MasPrimarySize"))
        //            {
        //                DataTable dtPrimarySize = CNService.builditems(@"select * from MasPrimarySize where [Id]='" +
        //                    string.Format("{0}", dr["Id"])
        //                    + "'");
        //                foreach (DataRow drPrimarySize in dtPrimarySize.Rows)
        //                {
        //                    using (var context = new ARTWORKEntities())
        //                    {
        //                        SAP_M_3P sapm3p = new SAP_M_3P();
        //                        sapm3p.PRIMARY_SIZE_VALUE = string.Format("{0}", drPrimarySize["Description"]);
        //                        sapm3p.PRIMARY_SIZE_DESCRIPTION = string.Format("{0}", drPrimarySize["Description"]);
        //                        sapm3p.CONTAINER_TYPE_VALUE = string.Format("{0}", drPrimarySize["ContainerType"]);
        //                        sapm3p.CONTAINER_TYPE_DESCRIPTION = string.Format("{0}", drPrimarySize["ContainerType"]);
        //                        sapm3p.LID_TYPE_VALUE = string.Format("{0}", drPrimarySize["DescriptionType"]);
        //                        sapm3p.LID_TYPE_DESCRIPTION = string.Format("{0}", drPrimarySize["DescriptionType"]);
        //                        var existItem = SAP_M_3P_SERVICE.GetByItem(sapm3p, context).FirstOrDefault();
        //                        if (existItem == null)
        //                        {
        //                            sapm3p.IS_ACTIVE = "X";
        //                            sapm3p.CREATE_BY = userID;
        //                            sapm3p.CREATE_DATE = DateTime.Today;
        //                            sapm3p.UPDATE_BY = userID;
        //                            sapm3p.UPDATE_DATE = DateTime.Today;
        //                            SAP_M_3P_SERVICE.SaveOrUpdateNoLog(sapm3p, context);
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                string _Id = dr["Id"].ToString();
        //                string _Description = dr["Description"].ToString();
        //                string[] value = { dr["Changed_Charname"].ToString(), _Description, _Id };
        //                CNService.master_artwork(value);
        //            }
        //        }
        //        else if (string.Format("{0}", dr["Changed_action"]).Equals("Re-Active"))
        //        {
        //            using (var context = new ARTWORKEntities())
        //            {
        //                context.Database.CommandTimeout = 600;
        //                SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();

        //                characteristic.NAME = string.Format("{0}", dr["Changed_Charname"]);
        //                characteristic.VALUE = string.Format("{0}-XXX Do Not Use XXX", dr["Old_Id"]);
        //                characteristic.DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", dr["Old_Id"]);

        //                var existItem = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(characteristic, context).FirstOrDefault();

        //                if (existItem != null)
        //                {
        //                    characteristic.CHARACTERISTIC_ID = existItem.CHARACTERISTIC_ID;
        //                }
        //                characteristic.VALUE = string.Format("{0}", dr["Id"]);
        //                characteristic.DESCRIPTION = string.Format("{0}", dr["Description"]);
        //                characteristic.IS_ACTIVE = "X";
        //                characteristic.CREATE_BY = userID;
        //                characteristic.CREATE_DATE = DateTime.Today;
        //                characteristic.UPDATE_BY = userID;
        //                characteristic.UPDATE_DATE = DateTime.Today;

        //                SAP_M_CHARACTERISTIC_SERVICE.SaveOrUpdateNoLog(characteristic, context);
        //            }
        //        }
        //        else
        //        {

        //            using (var context = new ARTWORKEntities())
        //            {
        //                context.Database.CommandTimeout = 600;
        //                SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();

        //                characteristic.NAME = string.Format("{0}", dr["Changed_Charname"]);
        //                characteristic.VALUE = string.Format("{0}", dr["Old_Description"]);
        //                characteristic.DESCRIPTION = string.Format("{0}", dr["Old_Description"]);

        //                var existItem = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(characteristic, context).FirstOrDefault();

        //                if (existItem != null)
        //                {
        //                    characteristic.CHARACTERISTIC_ID = existItem.CHARACTERISTIC_ID;
        //                }
        //                characteristic.VALUE = string.Format("{0}", dr["Description"]);
        //                characteristic.DESCRIPTION = string.Format("{0}", dr["Description"]);
        //                characteristic.IS_ACTIVE = "X";
        //                characteristic.CREATE_BY = userID;
        //                characteristic.CREATE_DATE = DateTime.Today;
        //                characteristic.UPDATE_BY = userID;
        //                characteristic.UPDATE_DATE = DateTime.Today;

        //                SAP_M_CHARACTERISTIC_SERVICE.SaveOrUpdateNoLog(characteristic, context);
        //            }
        //        }
        //    }
        //}
        public static void consoleWriteLineProcessTime(DateTime s,DateTime l, string subject)
        {
            TimeSpan ts = l - s;
            Console.WriteLine(subject + ". of Minutes = {0}", ts.TotalMinutes);
        }
        public static bool IsNumeric(string text)
        {
            double test;
            return double.TryParse(text, out test);
        }
        public static void buildinterface(string paramso)
        {

            MapperServices.Initialize();

            // var msg = MessageHelper.GetMessage("MSG_001"); // Helpers.MessageHelper.GetMessage("MSG_001");


            DateTime dateStart;
            DateTime dateLast;
            int sohCnt = 0;
            int sodCnt = 0;
            int socCnt = 0;
            TimeSpan ts;
            using (var context = new ARTWORKEntities())
            {
                //-------------------------
                Console.WriteLine("App Updated 20220210" + "...." + paramso);
                Console.WriteLine("sv:" + context.Database.Connection.DataSource + "  db:" + context.Database.Connection.Database);

                //dateStart = DateTime.Now;
                // Console.WriteLine("start:" + dateStart);


                // dateLast = DateTime.Now;
                //Console.WriteLine("Last:" + dateLast);

                //TimeSpan ts = dateLast - dateStart;
                //Console.WriteLine("No. of Minutes (Difference) = {0}", ts.TotalMinutes);
                dateStart = DateTime.Now;
                //-------------------------



                context.Database.CommandTimeout = 600;
                //string dateFormat = "yyyyMMdd";
                SAP_M_PO_COMPLETE_SO_MODEL param = new SAP_M_PO_COMPLETE_SO_MODEL();
                //var soHeader = (from f in context.SAP_M_PO_COMPLETE_SO_HEADER_TMP
                //select f).Take(50).ToList();
                string formatDate = "yyyyMMdd";
                var soHeader = context.Database.SqlQuery<SAP_M_PO_COMPLETE_SO_HEADER_TMP>("spGetSalesOrderTMP @temp",
                            new SqlParameter("@temp", paramso.ToString())).ToList();
                List<SO_HEADER> itemSOHeader = new List<SO_HEADER>();
                foreach (var h in soHeader)
                {
                    sohCnt += 1;
                    SO_HEADER soh = new SO_HEADER();
                    soh.LAST_SHIPMENT_DATE = h.LAST_SHIPMENT_DATE.ToString();
                    soh.DATE_1_2 = h.DATE_1_2.ToString();
                    //soh.CREATE_ON = h.CREATE_ON.ToString();

                    soh.CREATE_ON = Convert.ToDateTime(h.CREATE_ON).ToString(formatDate);
                    soh.RDD = Convert.ToDateTime(h.RDD).ToString(formatDate);
                    soh.EXPIRED_DATE = h.EXPIRED_DATE.ToString();
                    soh.PO_COMPLETE_SO_HEADER_ID = h.PO_COMPLETE_SO_HEADER_ID;
                    soh.SALES_ORDER_NO = h.SALES_ORDER_NO;
                    soh.SOLD_TO = h.SOLD_TO;
                    soh.SOLD_TO_NAME = h.SOLD_TO_NAME;
                    soh.PAYMENT_TERM = h.PAYMENT_TERM;
                    soh.LC_NO = h.LC_NO;
                    soh.SHIP_TO = h.SHIP_TO;
                    soh.SHIP_TO_NAME = h.SHIP_TO_NAME;
                    soh.SOLD_TO_PO = h.SOLD_TO_PO;
                    soh.SHIP_TO_PO = h.SHIP_TO_PO;
                    soh.SALES_GROUP = h.SALES_GROUP;
                    soh.MARKETING_CO = h.MARKETING_CO;
                    soh.MARKETING_CO_NAME = h.MARKETING_CO_NAME;
                    soh.MARKETING = h.MARKETING;
                    soh.MARKETING_NAME = h.MARKETING_NAME;
                    soh.MARKETING_ORDER_SAP = h.MARKETING_ORDER_SAP;
                    soh.MARKETING_ORDER_SAP_NAME = h.MARKETING_ORDER_SAP_NAME;
                    soh.SALES_ORG = h.SALES_ORG;
                    soh.DISTRIBUTION_CHANNEL = h.DISTRIBUTION_CHANNEL;
                    soh.DIVITION = h.DIVITION;
                    soh.SALES_ORDER_TYPE = h.SALES_ORDER_TYPE;
                    soh.HEADER_CUSTOM_1 = h.HEADER_CUSTOM_1;
                    soh.HEADER_CUSTOM_2 = h.HEADER_CUSTOM_2;
                    soh.HEADER_CUSTOM_3 = h.HEADER_CUSTOM_3;
                    //soh.CREATE_BY = userID;
                    //soh.UPDATE_BY = userID;
                    List<SO_ITEM> itemSO = new List<SO_ITEM>();
                    var soItem = (from p in context.SAP_M_PO_COMPLETE_SO_ITEM_TMP
                                  where p.PO_COMPLETE_SO_HEADER_ID == h.PO_COMPLETE_SO_HEADER_ID
                                  select p).ToList();
                    foreach (var i in soItem)
                    {
                        //var soItemComponent = (from m in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                        //                    where  m.PO_COMPLETE_SO_ITEM_ID == i.PO_COMPLETE_SO_ITEM_ID
                        //                    select m).ToList();
                        sodCnt += 1;
                        SO_ITEM iSO = new SO_ITEM();
                        iSO.ITEM = i.ITEM.ToString();
                        iSO.ORDER_QTY = i.ORDER_QTY.ToString();
                        if (i.ETD_DATE_FROM != null)
                            iSO.ETD_DATE_FROM = Convert.ToDateTime(i.ETD_DATE_FROM).ToString(formatDate);
                        if (i.ETD_DATE_TO != null)
                            iSO.ETD_DATE_TO = Convert.ToDateTime(i.ETD_DATE_TO).ToString(formatDate);
                        iSO.PRODUCT_CODE = i.PRODUCT_CODE;
                        iSO.MATERIAL_DESCRIPTION = i.MATERIAL_DESCRIPTION;
                        iSO.NET_WEIGHT = i.NET_WEIGHT;
                        iSO.ORDER_UNIT = i.ORDER_UNIT;
                        iSO.PLANT = i.PLANT;
                        iSO.OLD_MATERIAL_CODE = i.OLD_MATERIAL_CODE;
                        iSO.PACK_SIZE = i.PACK_SIZE;
                        iSO.VALUME_PER_UNIT = i.VALUME_PER_UNIT;
                        iSO.VALUME_UNIT = i.VALUME_UNIT;
                        iSO.SIZE_DRAIN_WT = i.SIZE_DRAIN_WT;
                        iSO.PROD_INSP_MEMO = i.PROD_INSP_MEMO;
                        iSO.REJECTION_CODE = i.REJECTION_CODE;
                        iSO.REJECTION_DESCRIPTION = i.REJECTION_DESCRIPTION;
                        iSO.PORT = i.PORT;
                        iSO.VIA = i.VIA;
                        iSO.IN_TRANSIT_TO = i.IN_TRANSIT_TO;
                        iSO.BRAND_ID = i.BRAND_ID;
                        iSO.BRAND_DESCRIPTION = i.BRAND_DESCRIPTION;
                        iSO.ADDITIONAL_BRAND_ID = i.ADDITIONAL_BRAND_ID;
                        iSO.ADDITIONAL_BRAND_DESCRIPTION = i.ADDITIONAL_BRAND_DESCRIPTION;
                        iSO.PRODUCTION_PLANT = i.PRODUCTION_PLANT;

                        iSO.ZONE = i.ZONE;
                        iSO.COUNTRY = i.COUNTRY;
                        iSO.PRODUCTION_HIERARCHY = i.PRODUCTION_HIERARCHY;
                        iSO.MRP_CONTROLLER = i.MRP_CONTROLLER;
                        iSO.STOCK = i.STOCK;
                        iSO.ITEM_CUSTOM_1 = i.ITEM_CUSTOM_1;
                        iSO.ITEM_CUSTOM_2 = i.ITEM_CUSTOM_2;
                        iSO.ITEM_CUSTOM_3 = i.ITEM_CUSTOM_3;
                        //iSO.CREATE_BY = userID;
                        //iSO.UPDATE_BY = userID; 
                        iSO.COMPONENTS = context.Database.SqlQuery<COMPONENT>("spGetSOCOMPONENT @ID",
                            new SqlParameter("@ID", i.PO_COMPLETE_SO_ITEM_ID)).ToList();
                        socCnt += iSO.COMPONENTS.Count;
                        itemSO.Add(iSO);
                        //context.Database.ExecuteSqlCommand("DELETE FROM SAP_M_PO_COMPLETE_SO_ITEM_TMP WHERE PO_COMPLETE_SO_HEADER_ID  = '" + h.PO_COMPLETE_SO_HEADER_ID + "'");
                        //context.Database.ExecuteSqlCommand("DELETE FROM SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_TMP WHERE PO_COMPLETE_SO_HEADER_ID  = '" + i.PO_COMPLETE_SO_ITEM_ID + "'");
                    }
                    soh.SO_ITEMS = itemSO;
                    itemSOHeader.Add(soh);
                    param.SO_HEADERS = itemSOHeader;
                    //
                }

                //-------------------------
                dateLast = DateTime.Now;
                Console.WriteLine("header:" + sohCnt + " rows");
                Console.WriteLine("items:" + sodCnt + " rows");
                Console.WriteLine("component:" + socCnt + " rows");
                consoleWriteLineProcessTime(dateStart, dateLast, "getSaleTemp");
                //-------------------------


                if (param.SO_HEADERS != null)
                {
                    SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();
                    Results = WebServices.Helper.SD_129_Helper.aSavePOCompleteSOtmp(param);
                }



                //context.Database.ExecuteSqlCommand("DELETE FROM SAP_M_PO_COMPLETE_SO_HEADER_TMP");
                //context.Database.ExecuteSqlCommand("DELETE FROM SAP_M_PO_COMPLETE_SO_ITEM_TMP");
                //context.Database.ExecuteSqlCommand("DELETE FROM SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_TMP");
            }

            //-------------------------
            dateLast = DateTime.Now;
            consoleWriteLineProcessTime(dateStart, dateLast, "All Process");
            System.Threading.Thread.Sleep(3000);
            //-------------------------
        }
        public static void ToCSV(DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false, new UTF8Encoding(true));
            //headers
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
        public static void buildinterface2(string paramso)
        {

            MapperServices.Initialize();

           // var msg = MessageHelper.GetMessage("MSG_001"); // Helpers.MessageHelper.GetMessage("MSG_001");


            DateTime dateStart;
            DateTime dateLast;
            int sohCnt = 0;
            int sodCnt = 0;
            int socCnt = 0;
            TimeSpan ts;
            using (var context = new ARTWORKEntities())
            {
                //-------------------------
                Console.WriteLine("App Updated 20210517");
                Console.WriteLine("sv:"+context.Database.Connection.DataSource + "  db:"+context.Database.Connection.Database);

                //dateStart = DateTime.Now;
                // Console.WriteLine("start:" + dateStart);


                // dateLast = DateTime.Now;
                //Console.WriteLine("Last:" + dateLast);

                //TimeSpan ts = dateLast - dateStart;
                //Console.WriteLine("No. of Minutes (Difference) = {0}", ts.TotalMinutes);
                dateStart = DateTime.Now;
                //-------------------------



                context.Database.CommandTimeout = 600;
                //string dateFormat = "yyyyMMdd";
                SAP_M_PO_COMPLETE_SO_MODEL param = new SAP_M_PO_COMPLETE_SO_MODEL();
                //var soHeader = (from f in context.SAP_M_PO_COMPLETE_SO_HEADER_TMP
                //select f).Take(50).ToList();
                string formatDate = "yyyyMMdd";
                var soHeader = context.Database.SqlQuery<SAP_M_PO_COMPLETE_SO_HEADER_TMP>("spGetSalesOrderTMP2 @ID",
                            new SqlParameter("@ID", paramso.ToString())).ToList(); 
                List<SO_HEADER> itemSOHeader = new List<SO_HEADER>();
                foreach (var h in soHeader)
                {
                    sohCnt += 1;
                    SO_HEADER soh = new SO_HEADER();
                    soh.LAST_SHIPMENT_DATE = h.LAST_SHIPMENT_DATE.ToString();
                    soh.DATE_1_2 = h.DATE_1_2.ToString();
                    //soh.CREATE_ON = h.CREATE_ON.ToString();

                    soh.CREATE_ON = Convert.ToDateTime(h.CREATE_ON).ToString(formatDate);
                    soh.RDD = Convert.ToDateTime(h.RDD).ToString(formatDate);
                    soh.EXPIRED_DATE = h.EXPIRED_DATE.ToString();
                    soh.PO_COMPLETE_SO_HEADER_ID = h.PO_COMPLETE_SO_HEADER_ID;
                    soh.SALES_ORDER_NO = h.SALES_ORDER_NO;
                    soh.SOLD_TO = h.SOLD_TO;
                    soh.SOLD_TO_NAME = h.SOLD_TO_NAME;
                    soh.PAYMENT_TERM = h.PAYMENT_TERM;
                    soh.LC_NO = h.LC_NO;
                    soh.SHIP_TO = h.SHIP_TO;
                    soh.SHIP_TO_NAME = h.SHIP_TO_NAME;
                    soh.SOLD_TO_PO = h.SOLD_TO_PO;
                    soh.SHIP_TO_PO = h.SHIP_TO_PO;
                    soh.SALES_GROUP = h.SALES_GROUP;
                    soh.MARKETING_CO = h.MARKETING_CO;
                    soh.MARKETING_CO_NAME = h.MARKETING_CO_NAME;
                    soh.MARKETING = h.MARKETING;
                    soh.MARKETING_NAME = h.MARKETING_NAME;
                    soh.MARKETING_ORDER_SAP = h.MARKETING_ORDER_SAP;
                    soh.MARKETING_ORDER_SAP_NAME = h.MARKETING_ORDER_SAP_NAME;
                    soh.SALES_ORG = h.SALES_ORG;
                    soh.DISTRIBUTION_CHANNEL = h.DISTRIBUTION_CHANNEL;
                    soh.DIVITION = h.DIVITION;
                    soh.SALES_ORDER_TYPE = h.SALES_ORDER_TYPE;
                    soh.HEADER_CUSTOM_1 = h.HEADER_CUSTOM_1;
                    soh.HEADER_CUSTOM_2 = h.HEADER_CUSTOM_2;
                    soh.HEADER_CUSTOM_3 = h.HEADER_CUSTOM_3;
                    //soh.CREATE_BY = userID;
                    //soh.UPDATE_BY = userID;
                    List<SO_ITEM> itemSO = new List<SO_ITEM>();
                    var soItem = (from p in context.SAP_M_PO_COMPLETE_SO_ITEM_TMP
                                  where p.PO_COMPLETE_SO_HEADER_ID == h.PO_COMPLETE_SO_HEADER_ID
                                  select p).ToList();
                    foreach (var i in soItem)
                    {
                        //var soItemComponent = (from m in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                        //                    where  m.PO_COMPLETE_SO_ITEM_ID == i.PO_COMPLETE_SO_ITEM_ID
                        //                    select m).ToList();
                        sodCnt += 1;
                        SO_ITEM iSO = new SO_ITEM();
                        iSO.ITEM = i.ITEM.ToString();
                        iSO.ORDER_QTY = i.ORDER_QTY.ToString();
                        if (i.ETD_DATE_FROM != null)
                            iSO.ETD_DATE_FROM = Convert.ToDateTime(i.ETD_DATE_FROM).ToString(formatDate);
                        if (i.ETD_DATE_TO != null)
                                iSO.ETD_DATE_TO = Convert.ToDateTime(i.ETD_DATE_TO).ToString(formatDate);
                        iSO.PRODUCT_CODE = i.PRODUCT_CODE;
                        iSO.MATERIAL_DESCRIPTION = i.MATERIAL_DESCRIPTION;
                        iSO.NET_WEIGHT = i.NET_WEIGHT;
                        iSO.ORDER_UNIT = i.ORDER_UNIT;
                        iSO.PLANT = i.PLANT;
                        iSO.OLD_MATERIAL_CODE = i.OLD_MATERIAL_CODE;
                        iSO.PACK_SIZE = i.PACK_SIZE;
                        iSO.VALUME_PER_UNIT = i.VALUME_PER_UNIT;
                        iSO.VALUME_UNIT = i.VALUME_UNIT;
                        iSO.SIZE_DRAIN_WT = i.SIZE_DRAIN_WT;
                        iSO.PROD_INSP_MEMO = i.PROD_INSP_MEMO;
                        iSO.REJECTION_CODE = i.REJECTION_CODE;
                        iSO.REJECTION_DESCRIPTION = i.REJECTION_DESCRIPTION;
                        iSO.PORT = i.PORT;
                        iSO.VIA = i.VIA;
                        iSO.IN_TRANSIT_TO = i.IN_TRANSIT_TO;
                        iSO.BRAND_ID = i.BRAND_ID;
                        iSO.BRAND_DESCRIPTION = i.BRAND_DESCRIPTION;
                        iSO.ADDITIONAL_BRAND_ID = i.ADDITIONAL_BRAND_ID;
                        iSO.ADDITIONAL_BRAND_DESCRIPTION = i.ADDITIONAL_BRAND_DESCRIPTION;
                        iSO.PRODUCTION_PLANT = i.PRODUCTION_PLANT;

                        iSO.ZONE = i.ZONE;
                        iSO.COUNTRY = i.COUNTRY;
                        iSO.PRODUCTION_HIERARCHY = i.PRODUCTION_HIERARCHY;
                        iSO.MRP_CONTROLLER = i.MRP_CONTROLLER;
                        iSO.STOCK = i.STOCK;
                        iSO.ITEM_CUSTOM_1 = i.ITEM_CUSTOM_1;
                        iSO.ITEM_CUSTOM_2 = i.ITEM_CUSTOM_2;
                        iSO.ITEM_CUSTOM_3 = i.ITEM_CUSTOM_3;
                        //iSO.CREATE_BY = userID;
                        //iSO.UPDATE_BY = userID; 
                        iSO.COMPONENTS = context.Database.SqlQuery<COMPONENT>("spGetSOCOMPONENT @ID",
                            new SqlParameter("@ID", i.PO_COMPLETE_SO_ITEM_ID)).ToList();
                        socCnt += iSO.COMPONENTS.Count;
                        itemSO.Add(iSO);
                        //context.Database.ExecuteSqlCommand("DELETE FROM SAP_M_PO_COMPLETE_SO_ITEM_TMP WHERE PO_COMPLETE_SO_HEADER_ID  = '" + h.PO_COMPLETE_SO_HEADER_ID + "'");
                        //context.Database.ExecuteSqlCommand("DELETE FROM SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_TMP WHERE PO_COMPLETE_SO_HEADER_ID  = '" + i.PO_COMPLETE_SO_ITEM_ID + "'");
                    }
                    soh.SO_ITEMS = itemSO;
                    itemSOHeader.Add(soh);
                    param.SO_HEADERS = itemSOHeader;
                    //
                }

                //-------------------------
                dateLast = DateTime.Now;
                Console.WriteLine("header:" + sohCnt + " rows");
                Console.WriteLine("items:" + sodCnt + " rows");
                Console.WriteLine("component:" + socCnt + " rows");
                consoleWriteLineProcessTime(dateStart, dateLast, "getSaleTemp");
                //-------------------------


                if (param.SO_HEADERS != null)
                {
                    SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();
                    Results = WebServices.Helper.SD_129_Helper.aSavePOCompleteSOtmp(param);
                }

             
               
                             //context.Database.ExecuteSqlCommand("DELETE FROM SAP_M_PO_COMPLETE_SO_HEADER_TMP");
                //context.Database.ExecuteSqlCommand("DELETE FROM SAP_M_PO_COMPLETE_SO_ITEM_TMP");
                //context.Database.ExecuteSqlCommand("DELETE FROM SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_TMP");
            }

            //-------------------------
            dateLast = DateTime.Now;
            consoleWriteLineProcessTime(dateStart, dateLast, "All Process");
            System.Threading.Thread.Sleep(3000);
            //-------------------------
        }
        public static string Compress(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }
        public static string Decompress(string s)
        {
            var bytes = Convert.FromBase64String(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.Unicode.GetString(mso.ToArray());
            }
        }

       

        public static DbContextTransaction IsolationLevel(ARTWORKEntities context)
        {
            return context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        }

        public static string Serialize(object value)
        {
            var javascriptserializer = new JavaScriptSerializer()
            {
                MaxJsonLength = Int32.MaxValue
            };
            return javascriptserializer.Serialize(value);
        }

        public static string Serialize(RESULT_MODEL value)
        {
            var javascriptserializer = new JavaScriptSerializer()
            {
                MaxJsonLength = Int32.MaxValue
            };
            return javascriptserializer.Serialize(value);
        }

        //public static int getCurrentUser()
        //{
        //    var res = 0;
        //    try
        //    {
        //        if (HttpContext.Current == null) { res = -1; }
        //        else
        //        {
        //            var UserName = HttpContext.Current.User.Identity.Name;
        //            res = GetUserID(UserName).Value;
        //        }
        //    }
        //    catch (Exception ex) { CNService.GetErrorMessage(ex); }

        //    if (res == 0) { res = -1; }
        //    return res;
        //}
        public static void getUpdateIsAssign()
        {

        }
        public static int getCurrentUser(ARTWORKEntities context)
        {
            var res = 0;
            try
            {
                if (HttpContext.Current == null) { res = -1; }
                else
                {
                    var UserName = HttpContext.Current.User.Identity.Name;
                    res = GetUserID(UserName, context).Value;
                }
            }
            catch (Exception ex) { CNService.GetErrorMessage(ex); }

            if (res == 0) { res = -1; }
            return res;
        }

        public static string RemoveHTMLTag(string param)
        {
            if (String.IsNullOrEmpty(param))
            {
                return "";
            }
            else
            {
                return Regex.Replace(param, "<.*?>", String.Empty);
            }
        }

        public static XECM_M_VENDOR GetVendorMigrationByMaterial(string material_no, ARTWORKEntities context)
        {
            XECM_M_VENDOR vendorXECM = new XECM_M_VENDOR();

            var vendorMigrate = (from v in context.SAP_M_MATERIAL_CONVERSION
                                 where v.MATERIAL_NO == material_no
                                    && v.CHAR_NAME == "ZPKG_SEC_VENDOR"
                                 select v).FirstOrDefault();
            if (vendorMigrate != null)
            {
                vendorXECM = (from v in context.XECM_M_VENDOR
                              where v.VENDOR_CODE == vendorMigrate.CHAR_VALUE
                              select v).FirstOrDefault();

                return vendorXECM;
            }
            else
                return null;
        }



        public static int ToInt(string param)
        {
            if (String.IsNullOrEmpty(param))
            { return 0; }

            int result;

            if (Int32.TryParse(param, out result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        public static int? ConvertStringToInt(string param)
        {
            if (String.IsNullOrEmpty(param))
            { return null; }

            int result;

            if (Int32.TryParse(param, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        //public static string GetUserLogin(int? UserID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return GetUserLogin(UserID, context);
        //    }
        //}

        public static string GetUserLogin(int? UserID, ARTWORKEntities context)
        {
            if (UserID == null) { return ""; }
            if (UserID == 0) { return ""; }

            var temp = (from p in context.ART_M_USER
                        where p.USER_ID == UserID
                        select p.USERNAME).FirstOrDefault();
            if (temp != null)
                return (temp).Trim().ToUpper();
            else
                return "";
        }

        public static string GetUserName(int? UserID, List<ART_M_USER> allUsers)
        {
            if (UserID == null) { return ""; }
            if (UserID == 0) { return ""; }

            var temp = allUsers.Where(m => m.USER_ID == UserID).FirstOrDefault();
            if (temp != null)
                return temp.TITLE + " " + temp.FIRST_NAME + " " + temp.LAST_NAME;
            else
                return "";
        }

        //public static string GetUserName(int? UserID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return GetUserName(UserID, context);
        //    }
        //}

        public static string GetUserName(int? UserID, ARTWORKEntities context)
        {
            if (UserID == null) { return ""; }
            if (UserID == 0) { return ""; }

            var temp = (from p in context.ART_M_USER where p.USER_ID == UserID select new ART_M_USER_2 { TITLE = p.TITLE, FIRST_NAME = p.FIRST_NAME, LAST_NAME = p.LAST_NAME }).FirstOrDefault();

            if (temp != null)
                return (temp.TITLE + " " + temp.FIRST_NAME + " " + temp.LAST_NAME).Trim();
            else
                return "";
        }

        public static int? GetPositionID(string PositionName, ARTWORKEntities context)
        {
            //using (var context = new ARTWORKEntities())
            //{
            var temp = (from p in context.ART_M_POSITION
                        where p.ART_M_POSITION_NAME.IndexOf(PositionName) >= 0
                        select p).FirstOrDefault();

            if (temp != null)
            {
                return temp.ART_M_POSITION_ID;
            }
            return 0;
            //}
        }

        //public static string GetPositionUser(int? UserID)
        //{
        //    //if (UserID == null) { return ""; }
        //    //if (UserID == 0) { return ""; }

        //    //var temp = ART_M_USER_SERVICE.GetByUSER_ID(UserID);
        //    using (var context = new ARTWORKEntities())
        //    {
        //        //var temp = (from p in context.ART_M_USER
        //        //            where p.USER_ID == UserID
        //        //            select new ART_M_USER_2 { POSITION_ID = p.POSITION_ID }).FirstOrDefault();

        //        //if (temp != null)
        //        //{
        //        //    if (temp.POSITION_ID != null)
        //        //    {
        //        //        //return ART_M_POSITION_SERVICE.GetByART_M_POSITION_ID(temp.POSITION_ID).ART_M_POSITION_NAME;
        //        //        return (from p in context.ART_M_POSITION
        //        //                where p.ART_M_POSITION_ID == temp.POSITION_ID
        //        //                select p.ART_M_POSITION_NAME).FirstOrDefault();
        //        //    }
        //        //}
        //        //return "";
        //        return GetPositionUser(UserID, context);
        //    }
        //}

        public static string GetPositionUser(int? UserID, ARTWORKEntities context)
        {
            if (UserID == null) { return ""; }
            if (UserID == 0) { return ""; }

            //var temp = ART_M_USER_SERVICE.GetByUSER_ID(UserID, context);
            var temp = (from p in context.ART_M_USER where p.USER_ID == UserID select p.POSITION_ID).FirstOrDefault();

            if (temp != null)
            {
                //return ART_M_POSITION_SERVICE.GetByART_M_POSITION_ID(temp.POSITION_ID, context).ART_M_POSITION_NAME;
                return (from p in context.ART_M_POSITION where p.ART_M_POSITION_ID == temp select p.ART_M_POSITION_NAME).FirstOrDefault();
            }
            return "";
        }

        public static string GetPositionCodeUser(int? UserID, ARTWORKEntities context)
        {
            if (UserID == null) { return ""; }
            if (UserID == 0) { return ""; }

            var temp = (from p in context.ART_M_USER where p.USER_ID == UserID select p.POSITION_ID).FirstOrDefault();

            if (temp != null)
            {
                return (from p in context.ART_M_POSITION where p.ART_M_POSITION_ID == temp select p.ART_M_POSITION_CODE).FirstOrDefault();
            }
            return "";
        }


        //public static int? GetUserID(string UserName)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return GetUserID(UserName, context);
        //    }
        //}

        public static int? GetUserID(string UserName, ARTWORKEntities context)
        {
            UserName = UserName.Replace(ConfigurationManager.AppSettings["TUDomain"], "");
            if (!string.IsNullOrEmpty(UserName))
            {
                var temp = (from m in context.ART_M_USER
                            where m.USERNAME.ToUpper() == UserName.ToUpper().Trim()
                            select new ART_M_USER_2 { USER_ID = m.USER_ID }).FirstOrDefault();

                if (temp != null)
                    return temp.USER_ID;
                else
                {
                    using (var context2 = new ARTWORKEntities())
                    {
                        var POSITION_ID_NOT_FOUND = (from m in context2.ART_M_POSITION where m.ART_M_POSITION_CODE == "NOT_FOUND" select m.ART_M_POSITION_ID).FirstOrDefault();
                        string password = "init1234";

                        ART_M_USER user = new ART_M_USER();
                        user.USERNAME = UserName.ToUpper().Trim();
                        user.POSITION_ID = POSITION_ID_NOT_FOUND;
                        user.PASSWORD = EncryptionService.Encrypt(password);
                        user.TITLE = "";
                        user.FIRST_NAME = UserName;
                        user.LAST_NAME = "";
                        user.IS_ACTIVE = "X";
                        user.IS_ADUSER = "X";
                        user.CREATE_BY = -1;
                        user.UPDATE_BY = -1;
                        ART_M_USER_SERVICE.SaveOrUpdate(user, context2);
                        return user.USER_ID;
                    }
                }
            }
            else { return -1; }
        }

        //public static int GetUserID2(string UserName)
        //{
        //    return GetUserID(UserName).Value;
        //}

        public static int GetUserID2(string UserName, ARTWORKEntities context)
        {
            return GetUserID(UserName, context).Value;
        }

        public static string GetErrorMessage_SORepeat(Exception ex,string fn_name)
        {
            ///---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//
            string ERROR_MSG = "";
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                ERROR_MSG = ex.Message + "<br/>" + ex.StackTrace;
            }
            else
            {
                ERROR_MSG = ex.Message;
            }

            var userId = -1;
            string USERNAME = null;
            if (HttpContext.Current != null)
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        userId = CNService.getCurrentUser(context);
                        var temp = ART_M_USER_SERVICE.GetByUSER_ID(userId, context);
                        if (temp != null)
                            USERNAME = temp.USERNAME;
                    }
                }
            }

            using (var context = new ARTWORKEntities())
            {
                ART_SYS_LOG error = new ART_SYS_LOG();
                error.CREATE_BY = userId;
                error.UPDATE_BY = userId;
                error.NEW_VALUE = USERNAME;
                error.ERROR_MSG = CNService.SubString(ERROR_MSG, 4000);
                error.TABLE_NAME = "GetErrorMessage_SORepeat/" + fn_name;
                error.ACTION = "E";

                if (ex.InnerException != null)
                {
                    if (!string.IsNullOrEmpty(ex.InnerException.Message))
                    {
                        error.OLD_VALUE = CNService.SubString(ex.InnerException.Message, 4000);
                    }
                }
                ART_SYS_LOG_SERVICE.SaveNoLog(error, context);
            }

            var devOrQas = CNService.IsDevOrQAS();
            if (devOrQas)
            {
                return ERROR_MSG;
            }
            else
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                {
                    return "An error occurred while processing your request.";
                }
                else if (ex.Message == "An exception has been raised that is likely due to a transient failure. If you are connecting to a SQL Azure database consider using SqlAzureExecutionStrategy.")
                {
                    return "There was a problem connecting to the server. Please try again.";
                }
                else if (ex.Message == "An error occurred while executing the command definition. See the inner exception for details.")
                {
                    return "There was a problem connecting to the server. Please try again.";
                }
                else if (ex.Message == "An error occurred while reading from the store provider's data reader. See the inner exception for details.")
                {
                    return "There was a problem connecting to the server. Please try again.";
                }
                else
                {
                    return ex.Message;
                }
            }
        }



        public static string GetErrorMessage(Exception ex)
        {
            string ERROR_MSG = "";
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                ERROR_MSG = ex.Message + "<br/>" + ex.StackTrace;
            }
            else
            {
                ERROR_MSG = ex.Message;
            }

            var userId = -1;
            string USERNAME = null;
            if (HttpContext.Current != null)
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        userId = CNService.getCurrentUser(context);
                        var temp = ART_M_USER_SERVICE.GetByUSER_ID(userId, context);
                        if (temp != null)
                            USERNAME = temp.USERNAME;
                    }
                }
            }

            using (var context = new ARTWORKEntities())
            {
                ART_SYS_LOG error = new ART_SYS_LOG();
                error.CREATE_BY = userId;
                error.UPDATE_BY = userId;
                error.NEW_VALUE = USERNAME;
                error.ERROR_MSG = CNService.SubString(ERROR_MSG, 4000);
                error.TABLE_NAME = "Function GetErrorMessage [CNService]";
                error.ACTION = "E";

                if (ex.InnerException != null)
                {
                    if (!string.IsNullOrEmpty(ex.InnerException.Message))
                    {
                        error.OLD_VALUE = CNService.SubString(ex.InnerException.Message, 4000);
                    }
                }
                ART_SYS_LOG_SERVICE.SaveNoLog(error, context);
            }

            var devOrQas = CNService.IsDevOrQAS();
            if (devOrQas)
            {
                return ERROR_MSG;
            }
            else
            {
                if (ex.Message == "Object reference not set to an instance of an object.")
                {
                    return "An error occurred while processing your request.";
                }
                else if (ex.Message == "An exception has been raised that is likely due to a transient failure. If you are connecting to a SQL Azure database consider using SqlAzureExecutionStrategy.")
                {
                    return "There was a problem connecting to the server. Please try again.";
                }
                else if (ex.Message == "An error occurred while executing the command definition. See the inner exception for details.")
                {
                    return "There was a problem connecting to the server. Please try again.";
                }
                else if (ex.Message == "An error occurred while reading from the store provider's data reader. See the inner exception for details.")
                {
                    return "There was a problem connecting to the server. Please try again.";
                }
                else
                {
                    return ex.Message;
                }
            }
        }

        //public static string GetVendorName(int? vendorID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        //if (vendorID == null) { return ""; }

        //        //string vendorName = "";

        //        ////var vendor = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(vendorID);
        //        //var vendor = (from m in context.XECM_M_VENDOR
        //        //              where m.VENDOR_ID == vendorID
        //        //              select new XECM_M_VENDOR_2 { VENDOR_NAME = m.VENDOR_NAME }).FirstOrDefault();

        //        //if (vendor != null) vendorName = vendor.VENDOR_NAME;

        //        //return vendorName;

        //        return GetVendorName(vendorID, context);
        //    }
        //}

        public static string GetVendorName(int? vendorID, ARTWORKEntities context)
        {
            if (vendorID == null) { return ""; }
            
            string vendorName = "";

            //var vendor = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(vendorID, context);
            var vendor = (from m in context.XECM_M_VENDOR
                          where m.VENDOR_ID == vendorID
                          select m.VENDOR_NAME).FirstOrDefault();

            if (vendor != null) vendorName = vendor;

            return vendorName;
        }

        //public static string GetVendorCodeName(int? vendorID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        //if (vendorID == null) { return ""; }

        //        //string vendorName = "";

        //        ////var vendor = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(vendorID);
        //        //var vendor = (from m in context.XECM_M_VENDOR
        //        //              where m.VENDOR_ID == vendorID
        //        //              select new XECM_M_VENDOR_2 { VENDOR_NAME = m.VENDOR_NAME, VENDOR_CODE = m.VENDOR_CODE }).FirstOrDefault();

        //        //if (vendor != null) vendorName = vendor.VENDOR_CODE + ":" + vendor.VENDOR_NAME;

        //        //return vendorName;

        //        return GetVendorCodeName(vendorID, context);
        //    }
        //}

        public static string GetVendorCodeName(int? vendorID, ARTWORKEntities context)
        {
            if (vendorID == null) { return ""; }

            string vendorName = "";

            //var vendor = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(vendorID, context);
            var vendor = (from m in context.XECM_M_VENDOR
                          where m.VENDOR_ID == vendorID
                          select new XECM_M_VENDOR_2 { VENDOR_NAME = m.VENDOR_NAME, VENDOR_CODE = m.VENDOR_CODE }).FirstOrDefault();

            if (vendor != null) vendorName = vendor.VENDOR_CODE + ":" + vendor.VENDOR_NAME;

            return vendorName;
        }

        //public static string GetCustomerName(int? customerID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        //if (customerID == null) { return ""; }

        //        //string customerName = "";

        //        ////var customer = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(customerID);
        //        //var customer = (from m in context.XECM_M_CUSTOMER
        //        //                where m.CUSTOMER_ID == customerID
        //        //                select new XECM_M_CUSTOMER_2 { CUSTOMER_NAME = m.CUSTOMER_NAME }).FirstOrDefault();


        //        //if (customer != null) customerName = customer.CUSTOMER_NAME;

        //        //return customerName;
        //        return GetCustomerName(customerID, context);
        //    }
        //}

        public static string GetCustomerName(int? customerID, ARTWORKEntities context)
        {
            if (customerID == null) { return ""; }

            string customerName = "";

            //var customer = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(customerID, context);
            var customer = (from m in context.XECM_M_CUSTOMER
                            where m.CUSTOMER_ID == customerID
                            select m.CUSTOMER_NAME).FirstOrDefault();

            if (customer != null) customerName = customer;

            return customerName;
        }

        //public static string GetCustomerCodeName(int? customerID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        //if (customerID == null) { return ""; }

        //        //string customerName = "";

        //        ////var customer = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(customerID);
        //        //var customer = (from m in context.XECM_M_CUSTOMER
        //        //                where m.CUSTOMER_ID == customerID
        //        //                select new XECM_M_CUSTOMER_2 { CUSTOMER_NAME = m.CUSTOMER_NAME, CUSTOMER_CODE = m.CUSTOMER_CODE }).FirstOrDefault();

        //        //if (customer != null) customerName = customer.CUSTOMER_CODE + ":" + customer.CUSTOMER_NAME;

        //        //return customerName;
        //        return GetCustomerCodeName(customerID, context);
        //    }
        //}

        public static string GetCustomerCodeName(int? customerID, ARTWORKEntities context)
        {
            if (customerID == null) { return ""; }

            string customerName = "";

            //var customer = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(customerID, context);
            var customer = (from m in context.XECM_M_CUSTOMER
                            where m.CUSTOMER_ID == customerID
                            select new XECM_M_CUSTOMER_2 { CUSTOMER_NAME = m.CUSTOMER_NAME, CUSTOMER_CODE = m.CUSTOMER_CODE }).FirstOrDefault();

            if (customer != null) customerName = customer.CUSTOMER_CODE + ":" + customer.CUSTOMER_NAME;

            return customerName;
        }

        //public static string GetCharacteristicDescription(int? charID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        //if (charID == null) { return ""; }

        //        //string charDesc = "";

        //        ////var vendor = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(charID);
        //        //var vendor = (from m in context.SAP_M_CHARACTERISTIC
        //        //              where m.CHARACTERISTIC_ID == charID
        //        //              select new SAP_M_CHARACTERISTIC_2 { DESCRIPTION = m.DESCRIPTION }).FirstOrDefault();

        //        //if (vendor != null) charDesc = vendor.DESCRIPTION;

        //        //return charDesc;

        //        return GetCharacteristicDescription(charID, context);
        //    }
        //}

        public static string GetCharacteristicDescription(int? charID, ARTWORKEntities context)
        {
            if (charID == null) { return ""; }

            string charDesc = "";

            //var vendor = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(charID, contex);
            var vendor = (from m in context.SAP_M_CHARACTERISTIC
                          where m.CHARACTERISTIC_ID == charID
                          select m.DESCRIPTION).FirstOrDefault();

            if (vendor != null) charDesc = vendor;

            return charDesc;
        }

        //public static string GetCharacteristicCodeAndDescription(int? charID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        //if (charID == null) { return ""; }

        //        //string charDesc = "";

        //        ////var vendor = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(charID);
        //        //var vendor = (from m in context.SAP_M_CHARACTERISTIC
        //        //              where m.CHARACTERISTIC_ID == charID
        //        //              select new SAP_M_CHARACTERISTIC_2 { DESCRIPTION = m.DESCRIPTION, VALUE = m.VALUE }).FirstOrDefault();

        //        //if (vendor != null) charDesc = vendor.VALUE + ":" + vendor.DESCRIPTION;

        //        //return charDesc;

        //        return GetCharacteristicCodeAndDescription(charID, context);
        //    }
        //}

        public static string GetCharacteristicCodeAndDescription(int? charID, ARTWORKEntities context)
        {
            if (charID == null) { return ""; }

            string charDesc = "";

            //var vendor = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(charID, contex);
            var vendor = (from m in context.SAP_M_CHARACTERISTIC
                          where m.CHARACTERISTIC_ID == charID
                          select new SAP_M_CHARACTERISTIC_2 { DESCRIPTION = m.DESCRIPTION, VALUE = m.VALUE }).FirstOrDefault();

            if (vendor != null) charDesc = vendor.VALUE + ":" + vendor.DESCRIPTION;

            return charDesc;
        }

        //public static string GetCharacteristicCode(int? charID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        //if (charID == null) { return ""; }

        //        //string charCode = "";

        //        ////var vendor = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(charID);
        //        //var vendor = (from m in context.SAP_M_CHARACTERISTIC
        //        //              where m.CHARACTERISTIC_ID == charID
        //        //              select new SAP_M_CHARACTERISTIC_2 { VALUE = m.VALUE }).FirstOrDefault();

        //        //if (vendor != null) charCode = vendor.VALUE;

        //        //return charCode;

        //        return GetCharacteristicCode(charID, context);
        //    }
        //}

        public static string GetCharacteristicCode(int? charID, ARTWORKEntities context)
        {
            if (charID == null) { return ""; }

            string charCode = "";

            //var vendor = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(charID, contex);
            var vendor = (from m in context.SAP_M_CHARACTERISTIC
                          where m.CHARACTERISTIC_ID == charID
                          select m.VALUE).FirstOrDefault();

            if (vendor != null) charCode = vendor;

            return charCode;
        }

        //public static SAP_M_CHARACTERISTIC GetCharacteristicData(int? charID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        //SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();

        //        //if (charID == null) { return characteristic; }

        //        //characteristic = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(charID);

        //        //return characteristic;

        //        return GetCharacteristicData(charID, context);
        //    }
        //}

        public static SAP_M_CHARACTERISTIC GetCharacteristicData(int? charID, ARTWORKEntities context)
        {
            SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();

            if (charID == null) { return characteristic; }

            characteristic = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(charID, context);

            return characteristic;
        }

        //public static SAP_M_CHARACTERISTIC GetCharacteristicData(string charName, string charValue)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        //SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();

        //        //if (String.IsNullOrEmpty(charName) && String.IsNullOrEmpty(charName)) { return characteristic; }

        //        //if (!String.IsNullOrEmpty(charName))
        //        //{
        //        //    characteristic.NAME = charName;
        //        //}

        //        //if (!String.IsNullOrEmpty(charValue))
        //        //{
        //        //    characteristic.VALUE = charValue;
        //        //}

        //        //characteristic = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(characteristic).FirstOrDefault();

        //        //return characteristic;
        //        return GetCharacteristicData(charName, charValue, context);
        //    }
        //}

        public static SAP_M_CHARACTERISTIC GetCharacteristicData(string charName, string charValue, ARTWORKEntities context)
        {
            SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();

            if (String.IsNullOrEmpty(charName) && String.IsNullOrEmpty(charName)) { return characteristic; }

            if (!String.IsNullOrEmpty(charName))
            {
                characteristic.NAME = charName;
            }

            if (!String.IsNullOrEmpty(charValue))
            {
                characteristic.VALUE = charValue;
            }

            characteristic = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(characteristic, context).FirstOrDefault();

            return characteristic;
        }

        public static string SubString(string txt, int len)
        {
            if (txt == null) return "";
            return txt.Length > len ? txt.Substring(0, len) : txt;
        }

        //public static int ConvertMockupIdToCheckListId(int mockupId)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        //var temp = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(mockupId);
        //        //var temp = (from m in context.ART_WF_MOCKUP_CHECK_LIST_ITEM
        //        //            where m.MOCKUP_ID == mockupId
        //        //            select new ART_WF_MOCKUP_CHECK_LIST_ITEM_2 { CHECK_LIST_ID = m.CHECK_LIST_ID }).FirstOrDefault();

        //        //if (temp == null)
        //        //    return 0;
        //        //else
        //        //    return temp.CHECK_LIST_ID;

        //        return ConvertMockupIdToCheckListId(mockupId, context);
        //    }
        //}

        public static int ConvertMockupIdToCheckListId(int mockupId, ARTWORKEntities context)
        {
            //var temp = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(mockupId, context);
            var temp = (from m in context.ART_WF_MOCKUP_CHECK_LIST_ITEM
                        where m.MOCKUP_ID == mockupId
                        select new ART_WF_MOCKUP_CHECK_LIST_ITEM_2 { CHECK_LIST_ID = m.CHECK_LIST_ID }).FirstOrDefault();

            if (temp == null)
                return 0;
            else
                return temp.CHECK_LIST_ID;
        }

        public static byte[] ReadAllBytes(Stream instream)
        {
            if (instream is MemoryStream)
                return ((MemoryStream)instream).ToArray();

            using (var memoryStream = new MemoryStream())
            {
                instream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static bool IsValidImage(Stream instream)
        {
            try
            {

            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }

        //public static bool IsAdmin(int userID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return IsAdmin(userID, context);
        //    }
        //}

        public static bool IsAdmin(int userID, ARTWORKEntities context)
        {
            var isAdmin = false;
            //var ADMIN = ART_M_POSITION_SERVICE.GetByItem(new DAL.ART_M_POSITION() { ART_M_POSITION_CODE = "ADMIN" }, context).FirstOrDefault().ART_M_POSITION_ID;
            var ADMIN = (from p in context.ART_M_POSITION where p.ART_M_POSITION_CODE == "ADMIN" select p.ART_M_POSITION_ID).FirstOrDefault();

            var POSITION_ID = (from p in context.ART_M_USER where p.USER_ID == userID select p.POSITION_ID).FirstOrDefault();

            //if (ART_M_USER_SERVICE.GetByUSER_ID(userID, context).POSITION_ID == ADMIN)
            if (POSITION_ID == ADMIN)
            {
                isAdmin = true;
            }

            //var RoleAdminID = ART_M_ROLE_SERVICE.GetByItem(new DAL.ART_M_ROLE() { ROLE_CODE = "ADMINISTRATOR" }, context).FirstOrDefault().ROLE_ID;
            var RoleAdminID = (from p in context.ART_M_ROLE where p.ROLE_CODE == "ADMINISTRATOR" select p.ROLE_ID).FirstOrDefault();

            //var listRole = ART_M_USER_ROLE_SERVICE.GetByItem(new DAL.ART_M_USER_ROLE() { USER_ID = Convert.ToInt32(userID) }, context);
            var listRole = (from p in context.ART_M_USER_ROLE where p.USER_ID == userID select p.ROLE_ID).ToList();

            //if (listRole.Where(m => m.ROLE_ID == RoleAdminID).Count() > 0)
            if (listRole.Where(m => m == RoleAdminID).Count() > 0)
            {
                isAdmin = true;
            }

            return isAdmin;
        }

        //public static bool IsCustomer(int userID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return IsCustomer(userID, context);
        //    }
        //}

        public static bool IsCustomer(int userID, ARTWORKEntities context)
        {
            var isCustomer = false;
            //var CUSTOMER = ART_M_POSITION_SERVICE.GetByItem(new DAL.ART_M_POSITION() { ART_M_POSITION_CODE = "CUSTOMER" }, context).FirstOrDefault().ART_M_POSITION_ID;
            var CUSTOMER = (from p in context.ART_M_POSITION where p.ART_M_POSITION_CODE == "CUSTOMER" select p.ART_M_POSITION_ID).FirstOrDefault();

            //var user = ART_M_USER_SERVICE.GetByUSER_ID(userID, context);
            var POSITION_ID = (from p in context.ART_M_USER where p.USER_ID == userID select p.POSITION_ID).FirstOrDefault();

            if (POSITION_ID == CUSTOMER)
            {
                isCustomer = true;
            }

            return isCustomer;
        }

        public static bool IsTHolding(int userID, ARTWORKEntities context)
        {
            var isT_HOLDING = false;
            //var CUSTOMER = ART_M_POSITION_SERVICE.GetByItem(new DAL.ART_M_POSITION() { ART_M_POSITION_CODE = "CUSTOMER" }, context).FirstOrDefault().ART_M_POSITION_ID;
            var T_HOLDING = (from p in context.ART_M_POSITION where p.ART_M_POSITION_CODE == "T-HOLDING" select p.ART_M_POSITION_ID).FirstOrDefault();

            //var user = ART_M_USER_SERVICE.GetByUSER_ID(userID, context);
            var POSITION_ID = (from p in context.ART_M_USER where p.USER_ID == userID select p.POSITION_ID).FirstOrDefault();

            if (POSITION_ID == T_HOLDING)
            {
                isT_HOLDING = true;
            }

            return isT_HOLDING;
        }

        //public static bool IsVendor(int userID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return IsVendor(userID, context);
        //    }
        //}

        public static bool IsVendor(int userID, ARTWORKEntities context)
        {
            var isVendor = false;
            //var VENDOR = ART_M_POSITION_SERVICE.GetByItem(new DAL.ART_M_POSITION() { ART_M_POSITION_CODE = "VENDOR" }, context).FirstOrDefault().ART_M_POSITION_ID;
            var VENDOR = (from p in context.ART_M_POSITION where p.ART_M_POSITION_CODE == "VENDOR" select p.ART_M_POSITION_ID).FirstOrDefault();

            //var user = ART_M_USER_SERVICE.GetByUSER_ID(userID, context);
            var POSITION_ID = (from p in context.ART_M_USER where p.USER_ID == userID select p.POSITION_ID).FirstOrDefault();

            if (POSITION_ID == VENDOR)
            {
                isVendor = true;
            }

            return isVendor;
        }

        //public static bool IsFFC(int userID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return IsFFC(userID, context);
        //    }
        //    //var isFFC = false;
        //    //var FFC = ART_M_POSITION_SERVICE.GetByItem(new DAL.ART_M_POSITION() { ART_M_POSITION_CODE = "FFC" }).FirstOrDefault().ART_M_POSITION_ID;
        //    //var user = ART_M_USER_SERVICE.GetByUSER_ID(userID);
        //    //if (user != null)
        //    //{
        //    //    if (user.POSITION_ID == FFC)
        //    //    {
        //    //        isFFC = true;
        //    //    }
        //    //}
        //    //return isFFC;
        //    //var isFFC = false;
        //    //var user = ART_M_USER_SERVICE.GetByUSER_ID(userID);
        //    //if (user != null)
        //    //{
        //    //    var tempPosition = ART_M_POSITION_SERVICE.GetByART_M_POSITION_ID(user.POSITION_ID);
        //    //    if (tempPosition != null)
        //    //    {
        //    //        if (tempPosition.ART_M_POSITION_CODE == "FFC")
        //    //        { isFFC = true; }
        //    //    }
        //    //}
        //    //return isFFC;
        //}

        public static bool IsFFC(int userID, ARTWORKEntities context)
        {
            var isFFC = false;
            //var FFC = ART_M_POSITION_SERVICE.GetByItem(new DAL.ART_M_POSITION() { ART_M_POSITION_CODE = "FFC" }, context).FirstOrDefault().ART_M_POSITION_ID;
            var FFC = (from p in context.ART_M_POSITION where p.ART_M_POSITION_CODE == "FFC" select p.ART_M_POSITION_ID).FirstOrDefault();

            //var user = ART_M_USER_SERVICE.GetByUSER_ID(userID, context);
            var POSITION_ID = (from p in context.ART_M_USER where p.USER_ID == userID select p.POSITION_ID).FirstOrDefault();

            if (POSITION_ID == FFC)
            {
                isFFC = true;
            }

            return isFFC;
        }

        //public static bool IsPackaging(int userID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return IsPackaging(userID, context);
        //    }
        //    //var isPackaging = false;
        //    //var PK = ART_M_POSITION_SERVICE.GetByItem(new DAL.ART_M_POSITION() { ART_M_POSITION_CODE = "PK" }).FirstOrDefault().ART_M_POSITION_ID;
        //    //var user = ART_M_USER_SERVICE.GetByUSER_ID(userID);
        //    //if (user != null)
        //    //{
        //    //    if (user.POSITION_ID == PK)
        //    //    {
        //    //        isPackaging = true;
        //    //    }
        //    //}
        //    //return isPackaging;
        //}

        public static bool IsPackaging(int userID, ARTWORKEntities context)
        {
            var isPackaging = false;
            //var PK = ART_M_POSITION_SERVICE.GetByItem(new DAL.ART_M_POSITION() { ART_M_POSITION_CODE = "PK" }, context).FirstOrDefault().ART_M_POSITION_ID;
            var PK = (from p in context.ART_M_POSITION where p.ART_M_POSITION_CODE == "PK" select p.ART_M_POSITION_ID).FirstOrDefault();

            var POSITION_ID = (from p in context.ART_M_USER where p.USER_ID == userID select p.POSITION_ID).FirstOrDefault();

            if (POSITION_ID == PK)
            {
                isPackaging = true;
            }

            return isPackaging;
        }

        //public static bool IsMarketing(int userID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return IsMarketing(userID, context);
        //    }
        //}

        public static bool IsMarketing(int userID, ARTWORKEntities context)
        {
            var isMarketing = false;
            //var MK = ART_M_POSITION_SERVICE.GetByItem(new DAL.ART_M_POSITION() { ART_M_POSITION_CODE = "MK" }, context).FirstOrDefault().ART_M_POSITION_ID;
            var MK = (from p in context.ART_M_POSITION where p.ART_M_POSITION_CODE == "MK" select p.ART_M_POSITION_ID).FirstOrDefault();

            var POSITION_ID = (from p in context.ART_M_USER where p.USER_ID == userID select p.POSITION_ID).FirstOrDefault();

            if (POSITION_ID == MK)
            {
                isMarketing = true;
            }

            return isMarketing;
        }

        public static bool IsRoleMK(int userID, ARTWORKEntities context)
        {
            var MK_CD = (from p in context.ART_M_ROLE where p.ROLE_CODE == "MK_CD" select p.ROLE_ID).FirstOrDefault();
            var MK_CD_SENIOR = (from p in context.ART_M_ROLE where p.ROLE_CODE == "MK_CD_SENIOR" select p.ROLE_ID).FirstOrDefault();
            var MK_CD_AM = (from p in context.ART_M_ROLE where p.ROLE_CODE == "MK_CD_AM" select p.ROLE_ID).FirstOrDefault();
            var MK_GM = (from p in context.ART_M_ROLE where p.ROLE_CODE == "MK_GM" select p.ROLE_ID).FirstOrDefault();

            var MC_STAFF = (from p in context.ART_M_ROLE where p.ROLE_CODE == "MC_STAFF" select p.ROLE_ID).FirstOrDefault();
            var MC_SUPERVISOR = (from p in context.ART_M_ROLE where p.ROLE_CODE == "MC_SUPERVISOR" select p.ROLE_ID).FirstOrDefault();
            var MC_AM = (from p in context.ART_M_ROLE where p.ROLE_CODE == "MC_AM" select p.ROLE_ID).FirstOrDefault();
            var MK_CD_MC_MANAGER = (from p in context.ART_M_ROLE where p.ROLE_CODE == "MK_CD_MC_MANAGER" select p.ROLE_ID).FirstOrDefault();

            var PMC = (from p in context.ART_M_ROLE where p.ROLE_CODE == "PMC" select p.ROLE_ID).FirstOrDefault();
            var PME = (from p in context.ART_M_ROLE where p.ROLE_CODE == "PME" select p.ROLE_ID).FirstOrDefault();
            var MARKETING_SUPPORT_MANAGER = (from p in context.ART_M_ROLE where p.ROLE_CODE == "MARKETING_SUPPORT_MANAGER" select p.ROLE_ID).FirstOrDefault();
            var MARKETING_SUPPORT_ASS_MANAGER = (from p in context.ART_M_ROLE where p.ROLE_CODE == "MARKETING_SUPPORT_ASS_MANAGER" select p.ROLE_ID).FirstOrDefault();
            var MC_TEAM_LEAD = (from p in context.ART_M_ROLE where p.ROLE_CODE == "MC_TEAM_LEAD" select p.ROLE_ID).FirstOrDefault();

            var listRole = (from p in context.ART_M_USER_ROLE where p.USER_ID == userID select p.ROLE_ID).ToList();
            foreach (var item in listRole)
            {
                if (item == MK_CD
                    || item == MK_CD_SENIOR
                    || item == MK_CD_AM
                    || item == MK_GM
                    || item == MC_STAFF
                    || item == MC_SUPERVISOR
                    || item == MC_AM
                    || item == MK_CD_MC_MANAGER
                    || item == PMC
                    || item == PME
                    || item == MARKETING_SUPPORT_MANAGER
                    || item == MARKETING_SUPPORT_ASS_MANAGER
                    || item == MC_TEAM_LEAD
                    )
                {
                    return true;
                }
            }

            return false;
        }

        //public static bool IsPG(int userID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return IsPG(userID, context);
        //    }
        //}

        public static bool IsPG(int userID, ARTWORKEntities context)
        {
            var PG_STAFF = (from p in context.ART_M_ROLE where p.ROLE_CODE == "PG_STAFF" select p.ROLE_ID).FirstOrDefault();
            var PG_TEAM_LEAD = (from p in context.ART_M_ROLE where p.ROLE_CODE == "PG_TEAM_LEAD" select p.ROLE_ID).FirstOrDefault();
            var PG_SUPPERVISOR = (from p in context.ART_M_ROLE where p.ROLE_CODE == "PG_SUPPERVISOR" select p.ROLE_ID).FirstOrDefault();
            var PG_MANAGER = (from p in context.ART_M_ROLE where p.ROLE_CODE == "PG_MANAGER" select p.ROLE_ID).FirstOrDefault();

            var listRole = (from p in context.ART_M_USER_ROLE where p.USER_ID == userID select p.ROLE_ID).ToList();
            foreach (var item in listRole)
            {
                if (item == PG_STAFF || item == PG_TEAM_LEAD || item == PG_SUPPERVISOR || item == PG_MANAGER)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsPGSup(int userID, ARTWORKEntities context)
        {
            var PG_SUPPERVISOR = (from p in context.ART_M_ROLE where p.ROLE_CODE == "PG_SUPPERVISOR" select p.ROLE_ID).FirstOrDefault();

            var listRole = (from p in context.ART_M_USER_ROLE where p.USER_ID == userID select p.ROLE_ID).ToList();
            foreach (var item in listRole)
            {
                if (item == PG_SUPPERVISOR)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsPA(int userID, ARTWORKEntities context)
        {
            var PA_STAFF = (from p in context.ART_M_ROLE where p.ROLE_CODE == "PA_STAFF" select p.ROLE_ID).FirstOrDefault();
            var PA_TEAM_LEAD = (from p in context.ART_M_ROLE where p.ROLE_CODE == "PA_TEAM_LEAD" select p.ROLE_ID).FirstOrDefault();
            var PA_SUPERVISOR = (from p in context.ART_M_ROLE where p.ROLE_CODE == "PA_SUPERVISOR" select p.ROLE_ID).FirstOrDefault();
            var PA_ASS_MANAGER = (from p in context.ART_M_ROLE where p.ROLE_CODE == "PA_ASS_MANAGER" select p.ROLE_ID).FirstOrDefault();

            var listRole = (from p in context.ART_M_USER_ROLE where p.USER_ID == userID select p.ROLE_ID).ToList();
            foreach (var item in listRole)
            {
                if (item == PA_STAFF || item == PA_TEAM_LEAD || item == PA_SUPERVISOR || item == PA_ASS_MANAGER)
                {
                    return true;
                }
            }

            return false;
        }

        //public static DateTime AddBusinessDays(DateTime dt, int nDays)
        //{
        //    int weeks = nDays / 5;
        //    nDays %= 5;
        //    while (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
        //        dt = dt.AddDays(1);

        //    while (nDays-- > 0)
        //    {
        //        dt = dt.AddDays(1);
        //        if (dt.DayOfWeek == DayOfWeek.Saturday)
        //            dt = dt.AddDays(2);
        //    }
        //    return dt.AddDays(weeks * 7);
        //}

        public static int GetBusinessDays(DateTime start, DateTime end)
        {
            if (start.DayOfWeek == DayOfWeek.Saturday)
            {
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 0, 0, 0);
                start = start.AddDays(2);
            }
            else if (start.DayOfWeek == DayOfWeek.Sunday)
            {
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 0, 0, 0);
                start = start.AddDays(1);
            }

            if (end.DayOfWeek == DayOfWeek.Saturday)
            {
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 0, 0, 0);
                end = end.AddDays(-1);
            }
            else if (end.DayOfWeek == DayOfWeek.Sunday)
            {
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 0, 0, 0);
                end = end.AddDays(-2);
            }

            int diff = (int)Math.Ceiling(end.Subtract(start).TotalHours) / 24;

            int result = ((diff / 7) * 5) + (diff % 7);

            if (end.DayOfWeek < start.DayOfWeek)
            {
                return result - 2;
            }
            else if (result == -9)
            {
                return result + 2;
            }
            else
            {
                return result;
            }
        }

        public static DateTime AddBusinessDays(DateTime current, int days)
        {
            if (current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday)
            {
                current = new DateTime(current.Year, current.Month, current.Day, 0, 1, 0);
                //current = current.AddMinutes(1);
                current = current.AddDays(current.DayOfWeek == DayOfWeek.Saturday ? 2 : 1);
            }
            var sign = Math.Sign(days);
            var unsignedDays = Math.Abs(days);
            for (var i = 0; i < unsignedDays; i++)
            {
                do
                {
                    current = current.AddDays(sign);
                }
                while (current.DayOfWeek == DayOfWeek.Saturday ||
                    current.DayOfWeek == DayOfWeek.Sunday);
            }
            return current;
        }

        public static DateTime ConvertStringToDate(string str)
        {
            return new DateTime(Convert.ToInt32(str.Split('/')[2]), Convert.ToInt32(str.Split('/')[1]), Convert.ToInt32(str.Split('/')[0]));
        }

        public static DateTime? ConvertStringToDate2(string str)
        {
            try
            {
                return new DateTime(Convert.ToInt32(str.Split('/')[2]), Convert.ToInt32(str.Split('/')[1]), Convert.ToInt32(str.Split('/')[0]));
            }
            catch { return null; }
        }

        public static int? GetLastestAction(ART_WF_MOCKUP_PROCESS process, ARTWORKEntities context)
        {
            if (process.CURRENT_USER_ID == null)
            {
                //var lastest = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = process.MOCKUP_ID, CURRENT_STEP_ID = process.CURRENT_STEP_ID }, context).OrderByDescending(m => m.CREATE_DATE).FirstOrDefault();
                var lastest = (from m in context.ART_WF_MOCKUP_PROCESS
                               where m.MOCKUP_ID == process.MOCKUP_ID && m.CURRENT_STEP_ID == process.CURRENT_STEP_ID
                               select new ART_WF_MOCKUP_PROCESS_2 { CURRENT_USER_ID = m.CURRENT_USER_ID, CREATE_DATE = m.CREATE_DATE }).OrderByDescending(m => m.CREATE_DATE).FirstOrDefault();

                if (lastest != null)
                {
                    return lastest.CURRENT_USER_ID;
                }
                return null;
            }
            else
            {
                return process.CURRENT_USER_ID;
            }
        }

        public static int? GetLastestActionArtwork(ART_WF_ARTWORK_PROCESS process, ARTWORKEntities context)
        {
            if (process.CURRENT_USER_ID == null)
            {
                //var lastest = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = process.ARTWORK_ITEM_ID, CURRENT_STEP_ID = process.CURRENT_STEP_ID }, context).OrderByDescending(m => m.CREATE_DATE).FirstOrDefault();
                var lastest = (from m in context.ART_WF_ARTWORK_PROCESS
                               where m.ARTWORK_ITEM_ID == process.ARTWORK_ITEM_ID && m.CURRENT_STEP_ID == process.CURRENT_STEP_ID
                               select new ART_WF_ARTWORK_PROCESS_2 { CURRENT_USER_ID = m.CURRENT_USER_ID, CREATE_DATE = m.CREATE_DATE }).OrderByDescending(m => m.CREATE_DATE).FirstOrDefault();

                if (lastest != null && lastest.CURRENT_USER_ID != -1)
                {
                    return lastest.CURRENT_USER_ID;
                }
                return null;
            }
            else
            {
                return process.CURRENT_USER_ID;
            }
        }

        //public static string GetVendorRFQ(int mockupID, int mockupSubID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return GetVendorRFQ(mockupID, mockupSubID, context);
        //    }
        //}

        public static string GetVendorRFQ(int mockupID, int mockupSubID, ARTWORKEntities context)
        {
            string vendorName = "";
            //var vendorRFQ = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE() { MOCKUP_ID = mockupID });
            var vendorRFQ = (from m in context.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE
                             where m.MOCKUP_ID == mockupID
                             select m);

            var groupByVendor = vendorRFQ.GroupBy(item => item.VENDOR_ID)
                 .Select(group => new { ID = group.Key, Items = group.ToList() })
                 .ToList();
            foreach (var item in groupByVendor)
            {
                if (vendorName == "")
                    vendorName += CNService.GetVendorCodeName(item.ID, context);
                else
                    vendorName += "<br/>" + CNService.GetVendorCodeName(item.ID, context);
            }
            return vendorName;
        }

        //public static string GetVendorSelected(int mockupID, int mockupSubID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return GetVendorSelected(mockupID, mockupSubID, context);
        //    }
        //}

        public static string GetVendorSelected(int mockupID, int mockupSubID, ARTWORKEntities context)
        {
            string vendorName = "";
            //var vendorSelected = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE() { MOCKUP_ID = mockupID, SELECTED = "X" }).FirstOrDefault();
            var vendorSelected = (from m in context.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE
                                  where m.MOCKUP_ID == mockupID && m.SELECTED == "X"
                                  select new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2 { VENDOR_ID = m.VENDOR_ID }).FirstOrDefault();

            if (vendorSelected != null)
                vendorName = CNService.GetVendorCodeName(vendorSelected.VENDOR_ID, context);
            else
            {
                //var temp = ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR() { MOCKUP_ID = mockupID }).OrderByDescending(m => m.PG_SELECT_VENDOR_ID).FirstOrDefault();
                var temp = (from m in context.ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR
                            where m.MOCKUP_ID == mockupID
                            select new ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_2 { VENDOR_ID = m.VENDOR_ID, PG_SELECT_VENDOR_ID = m.PG_SELECT_VENDOR_ID }).OrderByDescending(m => m.PG_SELECT_VENDOR_ID).FirstOrDefault();
                if (temp != null)
                {
                    vendorName = CNService.GetVendorCodeName(temp.VENDOR_ID, context);
                }
            }
            return vendorName;
        }

        public static List<ART_M_USER_2> GetAllInternalUser(ARTWORKEntities context)
        {
            //using (var context = new ARTWORKEntities())
            //{
            //var CUSTOMER = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "CUSTOMER" }).FirstOrDefault().ART_M_POSITION_ID;
            //var VENDOR = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "VENDOR" }).FirstOrDefault().ART_M_POSITION_ID;
            var CUSTOMER = (from p in context.ART_M_POSITION where p.ART_M_POSITION_CODE == "CUSTOMER" select p.ART_M_POSITION_ID).FirstOrDefault();
            var VENDOR = (from p in context.ART_M_POSITION where p.ART_M_POSITION_CODE == "VENDOR" select p.ART_M_POSITION_ID).FirstOrDefault();
            //var allUser = ART_M_USER_SERVICE.GetAll(context);

            //allUser = allUser.Where(m => m.POSITION_ID != CUSTOMER && m.POSITION_ID != VENDOR).ToList();
            var allUser = (from m in context.ART_M_USER
                           where m.POSITION_ID != CUSTOMER && m.POSITION_ID != VENDOR
                           select m).ToList();

            var res = MapperServices.ART_M_USER(allUser);
            foreach (var item in res)
            {
                item.DISPLAY_TXT = item.TITLE + " " + item.FIRST_NAME + " " + item.LAST_NAME;
            }
            res = res.OrderBy(m => m.FIRST_NAME).ThenBy(m => m.LAST_NAME).ToList();
            return res;
            //}
        }

        public static List<XECM_M_VENDOR_2> GetAllVendor(ARTWORKEntities context)
        {
            //using (var context = new ARTWORKEntities())
            //{
            //var allVendor = XECM_M_VENDOR_SERVICE.GetAll();
            //allVendor = allVendor.Where(m => m.IS_ACTIVE == "X").ToList();

            var allVendor = (from m in context.XECM_M_VENDOR
                             where m.IS_ACTIVE == "X"
                             select m).ToList();

            var res = MapperServices.XECM_M_VENDOR(allVendor);
            foreach (var item in res)
            {
                item.DISPLAY_TXT = item.VENDOR_CODE + " " + item.VENDOR_NAME;
            }
            res = res.OrderBy(m => m.DISPLAY_TXT).ToList();
            return res;
            //}
        }

        public static List<XECM_M_CUSTOMER_2> GetAllCustomer(ARTWORKEntities context)
        {
            //using (var context = new ARTWORKEntities())
            //{
            //var allCustomer = XECM_M_CUSTOMER_SERVICE.GetAll();
            //allCustomer = allCustomer.Where(m => m.IS_ACTIVE == "X").ToList();

            var allCustomer = (from m in context.XECM_M_CUSTOMER
                               where m.IS_ACTIVE == "X"
                               select m).ToList();

            var res = MapperServices.XECM_M_CUSTOMER(allCustomer);
            foreach (var item in res)
            {
                item.DISPLAY_TXT = item.CUSTOMER_CODE + " " + item.CUSTOMER_NAME;
            }
            res = res.OrderBy(m => m.DISPLAY_TXT).ToList();
            return res;
            //}
        }

        public static List<ART_M_USER_2> GetAllUserInMyPosition(int userId, ARTWORKEntities context)
        {
            //using (var context = new ARTWORKEntities())
            //{
            //var userData = ART_M_USER_SERVICE.GetByUSER_ID(userId);
            var POSITION_ID = (from p in context.ART_M_USER where p.USER_ID == userId select p.POSITION_ID).FirstOrDefault();

            //var CUSTOMER = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "CUSTOMER" }).FirstOrDefault().ART_M_POSITION_ID;
            //var VENDOR = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "VENDOR" }).FirstOrDefault().ART_M_POSITION_ID;
            var CUSTOMER = (from p in context.ART_M_POSITION where p.ART_M_POSITION_CODE == "CUSTOMER" select p.ART_M_POSITION_ID).FirstOrDefault();
            var VENDOR = (from p in context.ART_M_POSITION where p.ART_M_POSITION_CODE == "VENDOR" select p.ART_M_POSITION_ID).FirstOrDefault();

            var allUser = ART_M_USER_SERVICE.GetAll(context);
            if (POSITION_ID == CUSTOMER)
            {
                var customer = ART_M_USER_CUSTOMER_SERVICE.GetByItem(new ART_M_USER_CUSTOMER() { USER_ID = userId }, context).FirstOrDefault();
                if (customer != null)
                {
                    //var listUserCustomer = ART_M_USER_CUSTOMER_SERVICE.GetByItem(new ART_M_USER_CUSTOMER() { CUSTOMER_ID = customer.CUSTOMER_ID }).Select(m => m.USER_ID);
                    var listUserCustomer = (from p in context.ART_M_USER_CUSTOMER where p.CUSTOMER_ID == customer.CUSTOMER_ID select p.USER_ID).ToList();
                    allUser = allUser.Where(m => listUserCustomer.Contains(m.USER_ID)).ToList();
                }
            }
            else if (POSITION_ID == VENDOR)
            {
                var vendor = ART_M_USER_VENDOR_SERVICE.GetByItem(new ART_M_USER_VENDOR() { USER_ID = userId }, context).FirstOrDefault();
                if (vendor != null)
                {
                    //var listUserVendor = ART_M_USER_VENDOR_SERVICE.GetByItem(new ART_M_USER_VENDOR() { VENDOR_ID = vendor.VENDOR_ID }).Select(m => m.USER_ID);
                    var listUserVendor = (from p in context.ART_M_USER_VENDOR where p.VENDOR_ID == vendor.VENDOR_ID select p.USER_ID).ToList();
                    allUser = allUser.Where(m => listUserVendor.Contains(m.USER_ID)).ToList();
                }
            }
            else
            {
                allUser = allUser.Where(m => m.POSITION_ID == POSITION_ID).ToList();
            }

            var res = MapperServices.ART_M_USER(allUser);
            foreach (var item in res)
            {
                item.DISPLAY_TXT = item.TITLE + " " + item.FIRST_NAME + " " + item.LAST_NAME;
            }
            res = res.OrderBy(m => m.FIRST_NAME).ThenBy(m => m.LAST_NAME).ToList();
            return res;
            //}
        }

        public static ART_WF_MOCKUP_PROCESS CheckDelegateBeforeRounting(ART_WF_MOCKUP_PROCESS process, ARTWORKEntities context)
        {
            if (process.CURRENT_USER_ID > 0)
            {
                var ChecklistId = CNService.ConvertMockupIdToCheckListId(process.MOCKUP_ID, context);
                var ChangeOwner = ART_WF_LOG_CHANGE_OWNER_SERVICE.GetByItem(new ART_WF_LOG_CHANGE_OWNER() { WF_ID = ChecklistId, WF_TYPE = "M", IS_ACTIVE = "X", FROM_USER_ID = process.CURRENT_USER_ID }, context).FirstOrDefault();
                if (ChangeOwner != null)
                {
                    process.CURRENT_USER_ID = ChangeOwner.TO_USER_ID;
                }

                var listDelegate = ART_WF_DELEGATE_SERVICE.GetByItem(new ART_WF_DELEGATE() { IS_ACTIVE = "X", CURRENT_USER_ID = Convert.ToInt32(process.CURRENT_USER_ID) }, context);
                listDelegate = listDelegate.Where(m => DateTime.Now.Date >= m.FROM_DATE.Date && DateTime.Now.Date <= m.TO_DATE.Date).ToList();

                if (listDelegate.FirstOrDefault() != null)
                {
                    process.IS_DELEGATE = "X";
                    process.CURRENT_USER_ID = listDelegate.FirstOrDefault().TO_USER_ID;
                    ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(process, context);

                    ART_WF_LOG_DELEGATE model = new ART_WF_LOG_DELEGATE();
                    model.WF_TYPE = "M";
                    model.WF_SUB_ID = process.MOCKUP_SUB_ID;
                    model.FROM_USER_ID = listDelegate.FirstOrDefault().CURRENT_USER_ID;
                    model.TO_USER_ID = listDelegate.FirstOrDefault().TO_USER_ID;
                    model.DELEGATE_BY = listDelegate.FirstOrDefault().TO_USER_ID;
                    model.STEP_ID = process.CURRENT_STEP_ID;
                    model.REMARK = listDelegate.FirstOrDefault().REASON;
                    model.CREATE_BY = listDelegate.FirstOrDefault().TO_USER_ID;
                    model.UPDATE_BY = listDelegate.FirstOrDefault().TO_USER_ID;
                    ART_WF_LOG_DELEGATE_SERVICE.SaveOrUpdate(model, context);
                }
                else
                {
                    ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(process, context);
                }
            }
            else
            {
                ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(process, context);
            }
            return process;
        }

        public static ART_WF_ARTWORK_PROCESS CheckDelegateBeforeRountingArtwork(ART_WF_ARTWORK_PROCESS process, ARTWORKEntities context)
        {
            if (process.CURRENT_USER_ID > 0)
            {
                var ChangeOwner = ART_WF_LOG_CHANGE_OWNER_SERVICE.GetByItem(new ART_WF_LOG_CHANGE_OWNER() { WF_ID = process.ARTWORK_REQUEST_ID, WF_TYPE = "A", IS_ACTIVE = "X", FROM_USER_ID = process.CURRENT_USER_ID }, context).FirstOrDefault();
                if (ChangeOwner != null)
                {
                    process.CURRENT_USER_ID = ChangeOwner.TO_USER_ID;
                }

                var listDelegate = ART_WF_DELEGATE_SERVICE.GetByItem(new ART_WF_DELEGATE() { IS_ACTIVE = "X", CURRENT_USER_ID = Convert.ToInt32(process.CURRENT_USER_ID) }, context);
                listDelegate = listDelegate.Where(m => DateTime.Now.Date >= m.FROM_DATE.Date && DateTime.Now.Date <= m.TO_DATE.Date).ToList();

                if (listDelegate.FirstOrDefault() != null)
                {
                    process.IS_DELEGATE = "X";
                    process.CURRENT_USER_ID = listDelegate.FirstOrDefault().TO_USER_ID;
                    ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(process, context);

                    ART_WF_LOG_DELEGATE model = new ART_WF_LOG_DELEGATE();
                    model.WF_TYPE = "A";
                    model.WF_SUB_ID = process.ARTWORK_SUB_ID;
                    model.FROM_USER_ID = listDelegate.FirstOrDefault().CURRENT_USER_ID;
                    model.TO_USER_ID = listDelegate.FirstOrDefault().TO_USER_ID;
                    model.DELEGATE_BY = listDelegate.FirstOrDefault().TO_USER_ID;
                    model.STEP_ID = process.CURRENT_STEP_ID;
                    model.REMARK = listDelegate.FirstOrDefault().REASON;
                    model.CREATE_BY = listDelegate.FirstOrDefault().TO_USER_ID;
                    model.UPDATE_BY = listDelegate.FirstOrDefault().TO_USER_ID;
                    ART_WF_LOG_DELEGATE_SERVICE.SaveOrUpdate(model, context);
                }
                else
                {
                    ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(process, context);
                }
            }
            else
            {
                ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(process, context);
            }
            return process;
        }

        //public static List<int> FindArtworkSubId(int artworkSubId)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return FindArtworkSubId(artworkSubId, context);
        //    }
        //}

        


        public static string GetSaleOrderItems(PP_MODEL pp, ARTWORKEntities context) {
            var query = context.Database.SqlQuery<string>("spGetSaleOrderItems @so,@item",
                      new SqlParameter("@so", string.Format("{0}", pp.SALES_ORDER)),
                      new SqlParameter("@item", string.Format("{0}", pp.SALES_ORDER_ITEM)))
                      .FirstOrDefault();
            return string.Format("{0}", query);
        }

        //---------------------------------------------------------start code added by Aof for CR#19439 ---------------------------------------------------------
        public static string getDataFromSALES_ORDER_ITEM_RDD(string SALES_ORDER_ITEM_RDD, string ITEM_OR_RDD = "ITEM")
        {
            string val = "";

            if (!string.IsNullOrEmpty(SALES_ORDER_ITEM_RDD))
            {
                string ITEM_RDD = SALES_ORDER_ITEM_RDD.Trim().Substring(10, (Convert.ToInt32(SALES_ORDER_ITEM_RDD.Trim().Length) - 10)).Replace(")", "");
                string[] arrStr = ITEM_RDD.Split('@');

                if (arrStr.Length >= 2)
                {
                    switch (ITEM_OR_RDD.Trim().ToUpper())
                    {
                        case "ITEM":
                            val = arrStr[0];
                            break;
                        case "RDD":
                            val = arrStr[1];
                            break;
                        default:
                            break;
                    }
                }

            }
            return val;
        }
        //---------------------------------------------------------end code added by Aof for CR#19439 ---------------------------------------------------------

        public static List<PP_MODEL> GetArtworkSubId(int stepArtworkPP, PP_REQUEST param, ARTWORKEntities context)
        {
            List<PP_MODEL> p = new List<PP_MODEL>();
            var query = context.Database.SqlQuery<PP_MODEL>("spQueryPPview2 @stepArtworkPP,@requestdatefrom,@requestdateTo",
                                        new SqlParameter("@stepArtworkPP", string.Format("{0}", stepArtworkPP)),
                                        new SqlParameter("@requestdatefrom", string.Format("{0}", param.data.GET_BY_CREATE_DATE_FROM)),
                                        new SqlParameter("@requestdateTo", string.Format("{0}", param.data.GET_BY_CREATE_DATE_TO))
                                        )
                                        .ToList();
            p= query.Select(m => new PP_MODEL()
            {
                ARTWORK_REQUEST_ID = m.ARTWORK_REQUEST_ID,
                ARTWORK_SUB_ID = m.ARTWORK_SUB_ID,
                ARTWORK_ITEM_ID = m.ARTWORK_ITEM_ID,
                GROUPING = m.GROUPING,
                SOLD_TO_ID= m.SOLD_TO_ID ,
                SHIP_TO_ID = m.SHIP_TO_ID,
                SOLD_TO_DISPLAY_TXT = m.SOLD_TO_DISPLAY_TXT,
                SHIP_TO_DISPLAY_TXT = m.SHIP_TO_DISPLAY_TXT,
                WORKFLOW_NO = m.WORKFLOW_NO,
                SALES_ORDER_ITEM = string.Format("{0}", m.SALES_ORDER_ITEM.Replace("|", "<br> ")),
                REMARK_BY_PA=m.REMARK_BY_PA,
                RDD = m.RDD,
                BRAND_ID = m.BRAND_ID,
                BRAND_DISPLAY_TXT = m.BRAND_DISPLAY_TXT,
                PKG_TYPE_ID = m.PKG_TYPE_ID,
                PKG_TYPE_DISPLAY_TXT = m.PKG_TYPE_DISPLAY_TXT,
                SALES_ORG = string.Format("{0}", m.SALES_ORG),
                PRODUCT_CODE = string.Format("{0}", m.PRODUCT_CODE.Replace("|", "<br> ")),
                RECEIVE_DATE = m.RECEIVE_DATE,
                PKG_CODE = m.PKG_CODE.Replace("|", "<br> "),
                VENDOR_DISPLAY_TXT = m.VENDOR_DISPLAY_TXT,
                STATUS =   m.STATUS
            }).ToList();

            return p;
        }
        public static List<int> FindArtworkSubId(int artworkSubId, ARTWORKEntities context)
        {
            //var ARTWORK_ITEM_ID = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(artworkSubId).ARTWORK_ITEM_ID;
            var ARTWORK_ITEM_ID = (from p in context.ART_WF_ARTWORK_PROCESS where p.ARTWORK_SUB_ID == artworkSubId select p.ARTWORK_ITEM_ID).FirstOrDefault();

            //return ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = ARTWORK_ITEM_ID }).Select(m => m.ARTWORK_SUB_ID).ToList();
            return (from p in context.ART_WF_ARTWORK_PROCESS where p.ARTWORK_ITEM_ID == ARTWORK_ITEM_ID select p.ARTWORK_SUB_ID).ToList();
        }

        public static List<int> FindMockupId(int mockupsubId, ARTWORKEntities context)
        {
            //var ARTWORK_ITEM_ID = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(artworkSubId).ARTWORK_ITEM_ID;
            var MOCKUP_ID = (from p in context.ART_WF_MOCKUP_PROCESS where p.MOCKUP_SUB_ID == mockupsubId select p.MOCKUP_ID).FirstOrDefault();

            //return ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = ARTWORK_ITEM_ID }).Select(m => m.ARTWORK_SUB_ID).ToList();
            return (from p in context.ART_WF_MOCKUP_PROCESS where p.MOCKUP_ID == MOCKUP_ID select p.MOCKUP_SUB_ID).ToList();
        }

        //public static int FindParentArtworkSubId(int artworkSubId)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return FindParentArtworkSubId(artworkSubId, context);
        //    }
        //}
        //public static int FindParentArtworkSubId(int artworkSubId)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return FindParentArtworkSubId(artworkSubId, context);
        //    }
        //}

        public static int FindParentArtworkSubId(int artworkSubId, ARTWORKEntities context)
        {
            int parentSubID = 0;
            var ARTWORK_ITEM_ID = (from m in context.ART_WF_ARTWORK_PROCESS
                                   where m.ARTWORK_SUB_ID == artworkSubId
                                   select m.ARTWORK_ITEM_ID).FirstOrDefault();

            var process = (from h in context.ART_WF_ARTWORK_PROCESS
                           where h.ARTWORK_ITEM_ID == ARTWORK_ITEM_ID && h.PARENT_ARTWORK_SUB_ID == null
                           select h).FirstOrDefault();

            if (process != null)
            {
                return process.ARTWORK_SUB_ID;
            }

            return parentSubID;
        }

        //public static int FindArtworkRequestId(int artworkSubId)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return FindArtworkRequestId(artworkSubId, context);
        //    }
        //}

        public static int FindArtworkRequestId(int artworkSubId, ARTWORKEntities context)
        {
            //return ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(artworkSubId, context).ARTWORK_REQUEST_ID;
            return (from m in context.ART_WF_ARTWORK_PROCESS
                    where m.ARTWORK_SUB_ID == artworkSubId
                    select m.ARTWORK_REQUEST_ID).FirstOrDefault();
        }

        //public static int FindArtworkItemId(int artworkSubId)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return FindArtworkItemId(artworkSubId, context);
        //    }
        //}

        public static int FindArtworkItemId(int artworkSubId, ARTWORKEntities context)
        {
            //return ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(artworkSubId, context).ARTWORK_ITEM_ID;
            return (from m in context.ART_WF_ARTWORK_PROCESS
                    where m.ARTWORK_SUB_ID == artworkSubId
                    select m.ARTWORK_ITEM_ID).FirstOrDefault();
        }

        public static bool IsDevOrQAS()
        {
            var devOrQas = false;
            var con = System.Configuration.ConfigurationManager.ConnectionStrings["ARTWORKEntities"].ConnectionString;
            if (con.Contains("ARTWORK_DEV") || con.Contains("ARTWORK_QAS"))
                devOrQas = true;

            return devOrQas;
        }

        public static bool IsEncryptJson()
        {
            var IsEncryptJson = false;
            var EncryptJson = ConfigurationManager.AppSettings["EncryptJson"];
            if (EncryptJson == "TRUE")
                IsEncryptJson = true;

            return IsEncryptJson;
        }
        public static void CompletePOForm(ART_WF_ARTWORK_PROCESS_REQUEST param, ARTWORKEntities context)
        {
                var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);
                //var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context).FirstOrDefault();

                if (process != null)
                {
                    //var REQUEST_ITEM_NO = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(process.ARTWORK_ITEM_ID, context).REQUEST_ITEM_NO;
                    var REQUEST_ITEM_NO = (from p in context.ART_WF_ARTWORK_REQUEST_ITEM
                                           where p.ARTWORK_ITEM_ID == process.ARTWORK_ITEM_ID
                                           select p.REQUEST_ITEM_NO).FirstOrDefault();

                    var listPO = (from o in context.ART_WF_ARTWORK_MAPPING_PO
                                  where o.ARTWORK_NO == REQUEST_ITEM_NO
                                  select o).ToList();

                    if (listPO != null)
                    {
                        foreach (var iPO in listPO)
                        {
                            var mappingPo = new ART_WF_ARTWORK_MAPPING_PO();
                            mappingPo.ARTWORK_MAPPING_PO_ID = iPO.ARTWORK_MAPPING_PO_ID;
                            mappingPo.ARTWORK_NO = iPO.ARTWORK_NO;
                            mappingPo.PO_NO = iPO.PO_NO;
                            mappingPo.SO_NO = iPO.SO_NO;
                            mappingPo.MATERIAL_NO = iPO.MATERIAL_NO;
                            mappingPo.PO_ITEM = iPO.PO_ITEM;
                            mappingPo.SO_ITEM = iPO.SO_ITEM;
                            mappingPo.CREATE_BY = -1;
                            mappingPo.UPDATE_BY = -1;
                            mappingPo.IS_ACTIVE = "";

                            ART_WF_ARTWORK_MAPPING_PO_SERVICE.SaveOrUpdateNoLog(mappingPo, context);
                        }
                    }
                }
        }
        //public static bool CheckTypeOfProductAndCompanyArtwork(int userId, int artworkRequestId, int artworkSubId
        //    , ARTWORKEntities context, List<ART_M_STEP_ARTWORK> allStepArtwork
        //    , List<ART_M_USER_COMPANY> listUserCompanyParam, List<ART_M_USER_TYPE_OF_PRODUCT> listUserTypeofProductParam)
        //{
        //    if (artworkSubId == 0)
        //    {
        //        return true;
        //    }

        //    List<ART_M_USER_COMPANY> listUserCompany = listUserCompanyParam.Select(m => m).ToList();
        //    List<ART_M_USER_TYPE_OF_PRODUCT> listUserTypeofProduct = listUserTypeofProductParam.Select(m => m).ToList();

        //    var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(artworkSubId, context);

        //    var SEND_QC = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_QC").FirstOrDefault().STEP_ARTWORK_ID;
        //    var SEND_RD = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_RD").FirstOrDefault().STEP_ARTWORK_ID;
        //    var SEND_WH = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_WH").FirstOrDefault().STEP_ARTWORK_ID;
        //    var SEND_PN = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_PN").FirstOrDefault().STEP_ARTWORK_ID;

        //    var SEND_QC_VERIFY = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_QC_VERIFY").FirstOrDefault().STEP_ARTWORK_ID;
        //    var SEND_MK_VERIFY = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_MK_VERIFY").FirstOrDefault().STEP_ARTWORK_ID;
        //    var SEND_GM_MK = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_GM_MK").FirstOrDefault().STEP_ARTWORK_ID;
        //    var SEND_GM_QC = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_GM_QC").FirstOrDefault().STEP_ARTWORK_ID;
        //    var SEND_MK = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_MK").FirstOrDefault().STEP_ARTWORK_ID;

        //    bool valid = true;
        //    if (process.CURRENT_STEP_ID == SEND_QC || process.CURRENT_STEP_ID == SEND_RD ||
        //        process.CURRENT_STEP_ID == SEND_WH || process.CURRENT_STEP_ID == SEND_PN ||
        //        process.CURRENT_STEP_ID == SEND_PN ||
        //        process.CURRENT_STEP_ID == SEND_QC_VERIFY || process.CURRENT_STEP_ID == SEND_MK_VERIFY ||
        //        process.CURRENT_STEP_ID == SEND_GM_MK || process.CURRENT_STEP_ID == SEND_GM_QC || process.CURRENT_STEP_ID == SEND_MK)
        //    {
        //        var artworkRequest = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(artworkRequestId, context);

        //        if (1 == 1)
        //        {
        //            //Human food , SCC 
        //            //Use RD and QC of TUM
        //            if (process.CURRENT_STEP_ID == SEND_QC || process.CURRENT_STEP_ID == SEND_RD || process.CURRENT_STEP_ID == SEND_QC_VERIFY)
        //            {
        //                if (artworkRequest.TYPE_OF_PRODUCT_ID > 0)
        //                {
        //                    var TYPE_OF_PRODUCT = (from p in context.SAP_M_TYPE_OF_PRODUCT where p.TYPE_OF_PRODUCT_ID == artworkRequest.TYPE_OF_PRODUCT_ID select p.TYPE_OF_PRODUCT).FirstOrDefault();
        //                    if (TYPE_OF_PRODUCT == "HF")
        //                    //if (SAP_M_TYPE_OF_PRODUCT_SERVICE.GetByTYPE_OF_PRODUCT_ID(artworkRequest.TYPE_OF_PRODUCT_ID, context).TYPE_OF_PRODUCT == "HF")
        //                    {
        //                        var COMPANY_CODE = (from p in context.SAP_M_COMPANY where p.COMPANY_ID == artworkRequest.COMPANY_ID select p.COMPANY_CODE).FirstOrDefault();
        //                        //if (SAP_M_COMPANY_SERVICE.GetByCOMPANY_ID(artworkRequest.COMPANY_ID, context).COMPANY_CODE == "SCC")
        //                        {
        //                            //var TUM = SAP_M_COMPANY_SERVICE.GetByItem(new SAP_M_COMPANY() { COMPANY_CODE = "TUM" }, context).FirstOrDefault();
        //                            //var SCC = SAP_M_COMPANY_SERVICE.GetByItem(new SAP_M_COMPANY() { COMPANY_CODE = "SCC" }, context).FirstOrDefault();

        //                            var TUM = (from p in context.SAP_M_COMPANY where p.COMPANY_CODE == "TUM" select p.COMPANY_ID).FirstOrDefault();
        //                            var SCC = (from p in context.SAP_M_COMPANY where p.COMPANY_CODE == "SCC" select p.COMPANY_ID).FirstOrDefault();

        //                            if (listUserCompany.Where(m => m.COMPANY_ID == TUM).Count() > 0)
        //                            {
        //                                //is tum
        //                                ART_M_USER_COMPANY model = new ART_M_USER_COMPANY();
        //                                model.COMPANY_ID = SCC;
        //                                listUserCompany.Add(model);
        //                            }
        //                            else
        //                            {
        //                                listUserCompany = listUserCompany.Where(m => m.COMPANY_ID != SCC).ToList();
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            if (listUserCompany.Where(m => m.COMPANY_ID == artworkRequest.COMPANY_ID).Count() == 0)
        //            {
        //                return false;
        //            }
        //        }

        //        if (artworkRequest.TYPE_OF_PRODUCT_ID > 0)
        //        {
        //            if (SAP_M_TYPE_OF_PRODUCT_SERVICE.GetByTYPE_OF_PRODUCT_ID(artworkRequest.TYPE_OF_PRODUCT_ID, context).TYPE_OF_PRODUCT == "RTE")
        //            {
        //                var RTE = (from p in context.SAP_M_TYPE_OF_PRODUCT where p.TYPE_OF_PRODUCT == "RTE" select p.TYPE_OF_PRODUCT_ID).FirstOrDefault();
        //                ART_M_USER_TYPE_OF_PRODUCT model = new ART_M_USER_TYPE_OF_PRODUCT();
        //                model.TYPE_OF_PRODUCT_ID = RTE;
        //                listUserTypeofProduct.Add(model);
        //            }

        //            if (listUserTypeofProduct.Where(m => m.TYPE_OF_PRODUCT_ID == artworkRequest.TYPE_OF_PRODUCT_ID).Count() == 0)
        //            {
        //                return false;
        //            }
        //        }

        //        var LoginIsFFC = CNService.IsFFC(Convert.ToInt32(userId), context);
        //        var CheckListCreateByFFC = CNService.IsFFC(Convert.ToInt32(artworkRequest.CREATE_BY), context);
        //        if (!LoginIsFFC)
        //        {
        //            if (CheckListCreateByFFC)
        //            {
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            if (!CheckListCreateByFFC)
        //            {
        //                return false;
        //            }
        //        }

        //        return valid;
        //    }
        //    else
        //    {
        //        return valid;
        //    }
        //}
        //spGetMarketingMailToArtworkRequest
        public static bool GetMarketingMailToArtworkRequest(int artwork_request, ARTWORKEntities context)
        {

            SqlParameter[] arge = { new SqlParameter("@request_id", artwork_request)};
            DataTable dt = GetRelatedResources("spGetMarketingMailToArtworkRequest", arge);
            List<ART_M_USER> luser = (from DataRow dr in dt.Rows
                                                select new ART_M_USER
                                                {
                                                    USER_ID = Convert.ToInt32((dr["USER_ID"]))
                                                }).ToList();
            var isMK = false;
            if (luser.Count() > 0)
            {
                isMK = true;
            }

            return isMK;
        }
        public static bool GetMarketingCreatedArtworkRequest(ART_WF_ARTWORK_REQUEST artwork_request, ARTWORKEntities context)
        { 
            var isMK = false;

            if (artwork_request != null)
            {   
                    isMK = IsMarketing(artwork_request.CREATOR_ID.GetValueOrDefault(0), context);
            }

            return isMK;
        }
        public static bool IsMarketingCreatedArtworkRequest(ART_WF_ARTWORK_REQUEST artwork_request, ARTWORKEntities context)
        {
            //461704 by aof 
            var isMK = false;

            if (artwork_request != null)
            {
                if (artwork_request.TYPE_OF_ARTWORK == "REPEAT")
                {
                    isMK = false;
                }
                else
                {

                    isMK = IsMarketing(artwork_request.CREATOR_ID.GetValueOrDefault(0), context);
                }
               
            }

            return isMK;
        }


        public static bool CheckTypeOfProductAndCompanyArtwork(int userId, int artworkRequestId, int artworkSubId
                    , ARTWORKEntities context, List<ART_M_STEP_ARTWORK> allStepArtwork
                    , List<ART_M_USER_COMPANY> listUserCompanyParam, List<ART_M_USER_TYPE_OF_PRODUCT> listUserTypeofProductParam)
        {
            if (artworkSubId == 0)
            {
                return true;
            }

            List<ART_M_USER_COMPANY> listUserCompany = listUserCompanyParam.Select(m => m).ToList();
            List<ART_M_USER_TYPE_OF_PRODUCT> listUserTypeofProduct = listUserTypeofProductParam.Select(m => m).ToList();

            var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(artworkSubId, context);

            var SEND_QC = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_QC").FirstOrDefault().STEP_ARTWORK_ID;
            var SEND_RD = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_RD").FirstOrDefault().STEP_ARTWORK_ID;
            var SEND_WH = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_WH").FirstOrDefault().STEP_ARTWORK_ID;
            var SEND_PN = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_PN").FirstOrDefault().STEP_ARTWORK_ID;

            var SEND_QC_VERIFY = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_QC_VERIFY").FirstOrDefault().STEP_ARTWORK_ID;
            var SEND_MK_VERIFY = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_MK_VERIFY").FirstOrDefault().STEP_ARTWORK_ID;
            var SEND_GM_MK = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_GM_MK").FirstOrDefault().STEP_ARTWORK_ID;
            var SEND_GM_QC = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_GM_QC").FirstOrDefault().STEP_ARTWORK_ID;
            var SEND_MK = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_MK").FirstOrDefault().STEP_ARTWORK_ID;

            bool valid = true;
            if (process.CURRENT_STEP_ID == SEND_QC || process.CURRENT_STEP_ID == SEND_RD ||
                process.CURRENT_STEP_ID == SEND_WH || process.CURRENT_STEP_ID == SEND_PN ||
                process.CURRENT_STEP_ID == SEND_PN ||
                process.CURRENT_STEP_ID == SEND_QC_VERIFY || process.CURRENT_STEP_ID == SEND_MK_VERIFY ||
                process.CURRENT_STEP_ID == SEND_GM_MK || process.CURRENT_STEP_ID == SEND_GM_QC || process.CURRENT_STEP_ID == SEND_MK)
            {
                var artworkRequest = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(artworkRequestId, context);

                if (process.CURRENT_STEP_ID == SEND_RD || process.CURRENT_STEP_ID == SEND_GM_QC)
                {
                    var artworkItemId = FindArtworkItemId(artworkSubId, context);
                    var listProcessQC = (from p in context.ART_WF_ARTWORK_PROCESS where p.ARTWORK_ITEM_ID == artworkItemId && p.CURRENT_STEP_ID == SEND_QC orderby p.CREATE_DATE descending select p).ToList();
                    var lastProcessQC = new ART_WF_ARTWORK_PROCESS();
                    if (listProcessQC.Count > 0)
                    {
                        lastProcessQC = listProcessQC.FirstOrDefault();
                    }

                    if (lastProcessQC.CURRENT_USER_ID > 0)
                    {
                        var listUserCompanyUserQC = ART_M_USER_COMPANY_SERVICE.GetByItem(new ART_M_USER_COMPANY() { USER_ID = Convert.ToInt32(lastProcessQC.CURRENT_USER_ID) }, context);

                        var found = false;

                        //start by aof 20220711
                        //commeted by aof 20220711
                        foreach (var item in listUserCompanyUserQC)
                        {
                            foreach (var item2 in listUserCompany)
                            {
                                if (item.COMPANY_ID == item2.COMPANY_ID)
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                        //commeted by aof 20220711
                        //rewrite by aof 20220711
                        //if (process.CURRENT_STEP_ID == SEND_RD)
                        //{
                        //    foreach (var item in listUserCompanyUserQC)
                        //    {
                        //        foreach (var item2 in listUserCompany)
                        //        {
                        //            if (item.COMPANY_ID == item2.COMPANY_ID)
                        //            {
                        //                found = true;
                        //                break;
                        //            }
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    // SEND_GM_QC
                        //    foreach (var item in listUserCompany)
                        //    {
                        //        if (item.COMPANY_ID  == artworkRequest.COMPANY_ID )
                        //        {
                        //            found = true;
                        //            break;
                        //        }

                        //    }

                        //}
                        //rewrite by aof 20220711
                        //end by aof 20220711

                        if (!found)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (process.CURRENT_STEP_ID == SEND_QC || process.CURRENT_STEP_ID == SEND_QC_VERIFY)
                    {
                        if (artworkRequest.TYPE_OF_PRODUCT_ID > 0)
                        {
                            var TYPE_OF_PRODUCT = (from p in context.SAP_M_TYPE_OF_PRODUCT where p.TYPE_OF_PRODUCT_ID == artworkRequest.TYPE_OF_PRODUCT_ID select p.TYPE_OF_PRODUCT).FirstOrDefault();
                            if (TYPE_OF_PRODUCT == "HF")
                            //if (SAP_M_TYPE_OF_PRODUCT_SERVICE.GetByTYPE_OF_PRODUCT_ID(artworkRequest.TYPE_OF_PRODUCT_ID, context).TYPE_OF_PRODUCT == "HF")
                            {
                                //var COMPANY_CODE = (from p in context.SAP_M_COMPANY where p.COMPANY_ID == artworkRequest.COMPANY_ID select p.COMPANY_CODE).FirstOrDefault();
                                //if (SAP_M_COMPANY_SERVICE.GetByCOMPANY_ID(artworkRequest.COMPANY_ID, context).COMPANY_CODE == "SCC")
                                {
                                    //var TUM = SAP_M_COMPANY_SERVICE.GetByItem(new SAP_M_COMPANY() { COMPANY_CODE = "TUM" }, context).FirstOrDefault();
                                    //var SCC = SAP_M_COMPANY_SERVICE.GetByItem(new SAP_M_COMPANY() { COMPANY_CODE = "SCC" }, context).FirstOrDefault();

                                    var TUM = (from p in context.SAP_M_COMPANY where p.COMPANY_CODE == "TUM" select p.COMPANY_ID).FirstOrDefault();
                                    var SCC = (from p in context.SAP_M_COMPANY where p.COMPANY_CODE == "ITC" select p.COMPANY_ID).FirstOrDefault();

                                    if (listUserCompany.Where(m => m.COMPANY_ID == TUM).Count() > 0)
                                    {
                                        //is tum
                                        ART_M_USER_COMPANY model = new ART_M_USER_COMPANY();
                                        model.COMPANY_ID = SCC;
                                        listUserCompany.Add(model);
                                    }
                                    else
                                    {
                                        listUserCompany = listUserCompany.Where(m => m.COMPANY_ID != SCC).ToList();
                                    }
                                }
                            }
                        }
                    }
                    if (listUserCompany.Where(m => m.COMPANY_ID == artworkRequest.COMPANY_ID).Count() == 0)
                    {
                        return false;
                    }
                }

                if (artworkRequest.TYPE_OF_PRODUCT_ID > 0)
                {
                    var TYPE_OF_PRODUCT = (from p in context.SAP_M_TYPE_OF_PRODUCT where p.TYPE_OF_PRODUCT_ID == artworkRequest.TYPE_OF_PRODUCT_ID select p.TYPE_OF_PRODUCT).FirstOrDefault();
                    if (TYPE_OF_PRODUCT == "HF")
                    {
                        var RTE = (from p in context.SAP_M_TYPE_OF_PRODUCT where p.TYPE_OF_PRODUCT == "RTE" select p.TYPE_OF_PRODUCT_ID).FirstOrDefault();
                        ART_M_USER_TYPE_OF_PRODUCT model = new ART_M_USER_TYPE_OF_PRODUCT();
                        model.TYPE_OF_PRODUCT_ID = RTE;
                        listUserTypeofProduct.Add(model);
                    }

                    //if (SAP_M_TYPE_OF_PRODUCT_SERVICE.GetByTYPE_OF_PRODUCT_ID(artworkRequest.TYPE_OF_PRODUCT_ID, context).TYPE_OF_PRODUCT == "RTE")
                    //{
                    //    var RTE = (from p in context.SAP_M_TYPE_OF_PRODUCT where p.TYPE_OF_PRODUCT == "RTE" select p.TYPE_OF_PRODUCT_ID).FirstOrDefault();
                    //    ART_M_USER_TYPE_OF_PRODUCT model = new ART_M_USER_TYPE_OF_PRODUCT();
                    //    model.TYPE_OF_PRODUCT_ID = RTE;
                    //    listUserTypeofProduct.Add(model);
                    //}

                    if (listUserTypeofProduct.Where(m => m.TYPE_OF_PRODUCT_ID == artworkRequest.TYPE_OF_PRODUCT_ID).Count() == 0)
                    {
                        return false;
                    }
                }

                var LoginIsFFC = CNService.IsFFC(Convert.ToInt32(userId), context);
                var CheckListCreateByFFC = CNService.IsFFC(Convert.ToInt32(artworkRequest.CREATE_BY), context);
                if (!LoginIsFFC)
                {
                    if (CheckListCreateByFFC)
                    {
                        return false;
                    }
                }
                else
                {
                    if (!CheckListCreateByFFC)
                    {
                        return false;
                    }
                }

                return valid;
            }
            else
            {
                return valid;
            }
        }

        public static bool CheckTypeOfProductAndCompanyMockup(int userId, int checkListId, int mockupSubId
            , ARTWORKEntities context, List<ART_M_STEP_MOCKUP> allStepMockup
            , List<ART_M_USER_COMPANY> listUserCompanyParam, List<ART_M_USER_TYPE_OF_PRODUCT> listUserTypeofProductParam)
        {
            if (mockupSubId == 0)
            {
                return true;
            }

            List<ART_M_USER_COMPANY> listUserCompany = listUserCompanyParam.Select(m => m).ToList();
            List<ART_M_USER_TYPE_OF_PRODUCT> listUserTypeofProduct = listUserTypeofProductParam.Select(m => m).ToList();

            var SEND_RD_PRI_PKG = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_RD_PRI_PKG").FirstOrDefault().STEP_MOCKUP_ID;
            var SEND_PN_PRI_PKG = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_PN_PRI_PKG").FirstOrDefault().STEP_MOCKUP_ID;
            var SEND_WH_TEST_PACK = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_WH_TEST_PACK").FirstOrDefault().STEP_MOCKUP_ID;

            var process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(mockupSubId, context);
            bool valid = true;
            if (process.CURRENT_STEP_ID == SEND_RD_PRI_PKG || process.CURRENT_STEP_ID == SEND_PN_PRI_PKG || process.CURRENT_STEP_ID == SEND_WH_TEST_PACK)
            {
                var checkList = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(checkListId, context);

                if (listUserCompany.Where(m => m.COMPANY_ID == checkList.COMPANY_ID).Count() == 0)
                {
                    return false;
                }

                if (checkList.TYPE_OF_PRODUCT_ID > 0)
                {
                    var TYPE_OF_PRODUCT = (from p in context.SAP_M_TYPE_OF_PRODUCT where p.TYPE_OF_PRODUCT_ID == checkList.TYPE_OF_PRODUCT_ID select p.TYPE_OF_PRODUCT).FirstOrDefault();
                    if (TYPE_OF_PRODUCT == "HF")
                    {
                        var RTE = (from p in context.SAP_M_TYPE_OF_PRODUCT where p.TYPE_OF_PRODUCT == "RTE" select p.TYPE_OF_PRODUCT_ID).FirstOrDefault();
                        ART_M_USER_TYPE_OF_PRODUCT model = new ART_M_USER_TYPE_OF_PRODUCT();
                        model.TYPE_OF_PRODUCT_ID = RTE;
                        listUserTypeofProduct.Add(model);
                    }

                    if (listUserTypeofProduct.Where(m => m.TYPE_OF_PRODUCT_ID == checkList.TYPE_OF_PRODUCT_ID).Count() == 0)
                    {
                        return false;
                    }
                }

                var LoginIsFFC = CNService.IsFFC(Convert.ToInt32(userId), context);
                var CheckListCreateByFFC = CNService.IsFFC(Convert.ToInt32(checkList.CREATE_BY), context);
                if (!LoginIsFFC)
                {
                    if (CheckListCreateByFFC)
                    {
                        return false;
                    }
                }
                else
                {
                    if (!CheckListCreateByFFC)
                    {
                        return false;
                    }
                }

                return valid;
            }
            else
            {
                return valid;
            }
        }

        public static string GetGeneralText(string salesOrder, ARTWORKEntities context)
        {
            StringBuilder sbGeneralTextIDAll = new StringBuilder();
            if (salesOrder != null)
            {
                if (salesOrder.Length < 10)
                {
                    salesOrder = salesOrder.PadLeft(10, '0');
                }
                //var list = SAP_M_LONG_TEXT_SERVICE.GetByItem(new SAP_M_LONG_TEXT() { TEXT_NAME = salesOrder, TEXT_ID = "Z001" }, context);
                var list = (from h in context.SAP_M_LONG_TEXT
                            where h.TEXT_NAME == salesOrder && h.TEXT_ID == "Z001"
                            select h.LINE_TEXT).ToList();

                //list = list.Where(m => m.TEXT_ID == "Z001").ToList();
                foreach (var item in list)
                {
                    string strLineText = item.Replace(@"""", "");
                    sbGeneralTextIDAll.AppendLine(strLineText);
                }
            }

            return sbGeneralTextIDAll.ToString();
        }

        public static string GetWHText(string salesOrderNo, string salesOrderItem, ARTWORKEntities context)
        {
            StringBuilder sbWHTextIDAll = new StringBuilder();
            if (salesOrderNo != null)
            {
                if (salesOrderNo.Length < 10)
                {
                    salesOrderNo = salesOrderNo.PadLeft(10, '0');
                }
                if (salesOrderItem.Length < 6)
                {
                    salesOrderItem = salesOrderItem.PadLeft(6, '0');
                }
                salesOrderNo = salesOrderNo + salesOrderItem;
                //var list = SAP_M_LONG_TEXT_SERVICE.GetByItem(new SAP_M_LONG_TEXT() { TEXT_NAME = salesOrderNo, TEXT_ID = "Z105" }, context);
                var list = (from h in context.SAP_M_LONG_TEXT
                            where h.TEXT_NAME == salesOrderNo && h.TEXT_ID == "Z105"
                            select h.LINE_TEXT).ToList();

                //list = list.Where(m => m.TEXT_ID == "Z105").ToList();
                foreach (var item in list)
                {
                    string strLineText = item.Replace(@"""", "");
                    sbWHTextIDAll.AppendLine(strLineText);
                }
            }

            return sbWHTextIDAll.ToString();
        }

        public static string StampToPDF(string str1, string str2, string str3, Stream fileStream,long node_id)
        {
            var strPath = ConfigurationManager.AppSettings["PathTempFile"];
            bool exists = System.IO.Directory.Exists(strPath);
            if (!exists)
                System.IO.Directory.CreateDirectory(strPath);

            FileStream res = null;
             string newFile = strPath + DateTime.Now.ToString("ddMMyyyy HHmmssffff") + "_" + node_id + ".pdf";  ////#INC-6541 added node_id to concat with filename by aof 
            //string newFile = strPath + DateTime.Now.ToString("ddMMyyyy HHmmssffff") + ".pdf"; ////#INC-6541 commented by aof 
            FileStream fs = null;
            Document document = null;
            PdfReader reader = null;
            PdfWriter writer = null;
            PdfContentByte cb = null;

            try
            {
                // open the reader
                reader = new PdfReader(fileStream);
                PdfReader.unethicalreading = true;


                int widthSpace = 2;

                Rectangle originalSize = reader.GetPageSize(1);
                float originalHeight = originalSize.Height;
                float originalWidth = originalSize.Width;

                //Rectangle newSize = rotationPage1 == 90 || rotationPage1 == 180 ? PageSize.A4.Rotate() : PageSize.A4;

                bool layoutLanscape = Convert.ToBoolean(ConfigurationManager.AppSettings["StapmPdfLandscape"]);
                Rectangle newSize = layoutLanscape ? PageSize.A4.Rotate() : reader.GetPageSizeWithRotation(1);
                float newHeight = newSize.Height;
                float newWidth = newSize.Width;
                float scaleHeight = newHeight / originalHeight;
                float scaleWidth = newWidth / originalWidth;

                //iTextSharp.text.Rectangle size = new Rectangle(PageSize.A4.Rotate());// reader.GetPageSizeWithRotation(1);//new RectangleReadOnly(595, 842); //


                document = new Document(newSize);
                fs = new FileStream(newFile, FileMode.Create, FileAccess.Write);
                writer = PdfWriter.GetInstance(document, fs);
                //Step 3: Open the document
                document.Open();

                //for (int pageNumber = 1; pageNumber < 1 + 1; pageNumber++)
                for (int pageNumber = 1; pageNumber < reader.NumberOfPages + 1; pageNumber++)
                {
                    document.SetPageSize(newSize);//reader.GetPageSizeWithRotation(1)
                    document.NewPage();

                    //Insert to Destination on the first page
                    //if (pageNumber == 1)
                    //{
                    //    Chunk fileRef = new Chunk(" ");
                    //    fileRef.SetLocalDestination(newFile);
                    //    document.Add(fileRef);
                    //}

                    PdfImportedPage page = writer.GetImportedPage(reader, pageNumber);

                    cb = writer.DirectContent;
                    int rotation = reader.GetPageRotation(pageNumber);

                    if (layoutLanscape)
                    {
                        cb.AddTemplate(page, scaleWidth, 0, 0, scaleHeight, 0, 0);
                    }
                    else
                    {
                        if (rotation == 90)
                        {
                            cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(pageNumber).Height);
                        }
                        else if (rotation == 180)
                        {
                            cb.AddTemplate(page, -1f, 0, 0, -1f, reader.GetPageSizeWithRotation(pageNumber).Width, reader.GetPageSizeWithRotation(pageNumber).Height);
                        }
                        else if (rotation == 270)
                        {
                            cb.AddTemplate(page, 0, 1.0F, -1.0F, 0, reader.GetPageSizeWithRotation(pageNumber).Width, 0);
                        }
                        else
                        {
                            cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                        }
                    }

                    // select the font properties
                    BaseFont bf = BaseFont.CreateFont(@"C:\WINDOWS\Fonts\tahoma.ttf", BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    cb.SetColorFill(BaseColor.BLUE);

                    iTextSharp.text.Font font = new iTextSharp.text.Font(bf, 12);

                    int lineTmp = 1;

                    var numItemInRowSO = 7;
                    var strSO = str1.Split(',');
                    int line = 1;
                    int i = 0;
                    var temp = "";

                    var numItemInRowMat3 = 5;
                    var strMat3 = str2.Split('/');

                    StringBuilder sbSpace = new StringBuilder();
                    string whiteSpace = "\u00a0";

                    for (int k = 1; k <= 207; k++)
                    {
                        sbSpace.Append(whiteSpace);
                    }

                    int linesSO = 0;
                    int linesProduct = 0;

                    if (strSO.Length > 0)
                    { linesSO = Convert.ToInt32(strSO.Length / 7); }

                    if (strMat3.Length > 0)
                    { linesProduct = Convert.ToInt32(strMat3.Length / 5); }

                    int allLines = linesSO + linesProduct + 2;

                    for (int j = 0; j <= allLines; j++)
                    {
                        Chunk textAsChunk1 = new Chunk(sbSpace.ToString());
                        textAsChunk1.SetBackground(new BaseColor(255, 255, 255), 0, widthSpace, 0, widthSpace);

                        ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(textAsChunk1), 5, newSize.Top - (15 * lineTmp), 0);
                        lineTmp++;
                    }


                    foreach (var s in strSO)
                    {
                        if (temp == "") temp = s;
                        else if (temp != "") temp += " , " + s;
                        i++;

                        if (i == numItemInRowSO)
                        {
                            Chunk textAsChunk1 = new Chunk(temp, font);
                            textAsChunk1.SetBackground(new BaseColor(255, 255, 255), 0, widthSpace, 0, widthSpace);

                            ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(textAsChunk1), 5, newSize.Top - (15 * line), 0);

                            line++;
                            i = 0;
                            temp = "";
                        }
                    }

                    if (i > 0 && i != numItemInRowSO)
                    {
                        Chunk textAsChunk1 = new Chunk(temp, font);
                        textAsChunk1.SetBackground(new BaseColor(255, 255, 255), 0, widthSpace, 0, widthSpace);

                        ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(textAsChunk1), 5, newSize.Top - (15 * line), 0);
                        line++;
                    }


                    i = 0;
                    temp = "";
                    foreach (var s in strMat3)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            if (temp == "") temp = s;
                            else if (temp != "") temp += " / " + s;
                            i++;

                            if (i == numItemInRowMat3)
                            {
                                Chunk textAsChunk1 = new Chunk(temp, font);
                                textAsChunk1.SetBackground(new BaseColor(255, 255, 255), 0, widthSpace, 0, widthSpace);

                                ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(textAsChunk1), 5, newSize.Top - (15 * line), 0);

                                line++;
                                i = 0;
                                temp = "";
                            }
                        }
                    }

                    if (i > 0 && i != numItemInRowMat3)
                    {
                        Chunk textAsChunk1 = new Chunk(temp, font);
                        textAsChunk1.SetBackground(new BaseColor(255, 255, 255), 0, widthSpace, 0, widthSpace);

                        ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(textAsChunk1), 5, newSize.Top - (15 * line), 0);
                        line++;
                    }

                    Chunk textAsChunk3 = new Chunk(str3, font);
                    textAsChunk3.SetBackground(new BaseColor(255, 255, 255), 0, widthSpace, 0, widthSpace);

                    ColumnText.ShowTextAligned(cb, Element.ALIGN_LEFT, new Phrase(textAsChunk3), 5, newSize.Top - (15 * line), 0);
                    line++;
                }

                return newFile;
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                if (document != null) document.Close();
                if (fs != null) fs.Close();
                if (writer != null) writer.Close();
                if (reader != null) reader.Close();
                if (res != null) res.Close();
            }
        }

        public static string ConcatArray(string[] strArr)
        {
            var res = "";
            foreach (var item in strArr)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (res == "") res = item;
                    else res += ", " + item;
                }
            }
            return res;
        }

        public static string ConcatArrayEnterLine(string[] strArr)
        {
            var res = "";
            foreach (var item in strArr)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    if (res == "") res = item;
                    else res += ", <br>" + item;
                }
            }
            return res;
        }

        public static string ConcatArray(decimal?[] strArr)
        {
            var res = "";
            foreach (var item in strArr)
            {
                if (item != null)
                {
                    if (res == "") res = item.ToString();
                    else res += ", " + item.ToString();
                }
            }
            return res;
        }

        public static string ConcatArray(DateTime?[] strArr)
        {
            var res = "";
            foreach (var item in strArr)
            {
                if (item != null)
                {
                    if (res == "") res = item.Value.ToString("dd/MM/yyyy");
                    else res += ", " + item.Value.ToString("dd/MM/yyyy");
                }
            }
            return res;
        }



        public static void SaveLogAction(string TABLE_NAME,string ACTION,string NEW_VALUE,string OLD_VALUE,string ERROR_MEG)
        {
            using (var context = new ARTWORKEntities())
            {
                ART_SYS_LOG log = new ART_SYS_LOG();
                log.TABLE_NAME = TABLE_NAME;
                log.ACTION =ACTION;
                log.NEW_VALUE = NEW_VALUE;
                log.OLD_VALUE = OLD_VALUE;
                log.ERROR_MSG = ERROR_MEG;
                log.CREATE_BY = -5;
                log.UPDATE_BY = -5;     
                ART_SYS_LOG_SERVICE.SaveNoLog(log, context);
            }
        }

        public static void SaveLogReturnInterface(SERVICE_RESULT_MODEL Results, string TABLE_NAME, string guid,string wsCode)
        {
            using (var context = new ARTWORKEntities())
            {
                ART_SYS_LOG log = new ART_SYS_LOG();
                log.TABLE_NAME = TABLE_NAME;
                log.NEW_VALUE = CNService.Serialize(Results);
                log.NEW_VALUE = CNService.SubString(log.NEW_VALUE, 4000);
                log.ACTION = "Interface Outbound";
                if (!string.IsNullOrEmpty(wsCode))
                {
                    log.ACTION += "-" + wsCode;
                }
                log.CREATE_BY = -2;
                log.UPDATE_BY = -2;
                log.OLD_VALUE = guid;
                ART_SYS_LOG_SERVICE.SaveNoLog(log, context);
            }
        }

        public static Nullable<int> CheckPICArtworkBySORepeat(ARTWORKEntities context, ART_WF_ARTWORK_REQUEST_2 artworkRequest)
        {
            var soldToId = artworkRequest.SOLD_TO_ID;
            var soldToCode = "";
            if (artworkRequest.SOLD_TO_ID > 0) soldToCode = (from m in context.XECM_M_CUSTOMER where m.CUSTOMER_ID == artworkRequest.SOLD_TO_ID select m.CUSTOMER_CODE).FirstOrDefault();
            //soldToCode = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(artworkRequest.SOLD_TO_ID, context).CUSTOMER_CODE;

            var shipToId = artworkRequest.SHIP_TO_ID;
            var shipToCode = "";
            if (artworkRequest.SHIP_TO_ID > 0) shipToCode = (from m in context.XECM_M_CUSTOMER where m.CUSTOMER_ID == artworkRequest.SHIP_TO_ID select m.CUSTOMER_CODE).FirstOrDefault();
            //shipToCode = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(artworkRequest.SHIP_TO_ID, context).CUSTOMER_CODE;

            var zone = "";
            ////var countryRequest = ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_COUNTRY() { ARTWORK_REQUEST_ID = artworkRequest.ARTWORK_REQUEST_ID }, context).FirstOrDefault();
            //var countryRequest = (from m in context.ART_WF_ARTWORK_REQUEST_COUNTRY
            //                      where m.ARTWORK_REQUEST_ID == artworkRequest.ARTWORK_REQUEST_ID
            //                      select new ART_WF_ARTWORK_REQUEST_COUNTRY_2 { COUNTRY_ID = m.COUNTRY_ID }).FirstOrDefault();

            var COUNTRY_ID = artworkRequest.COUNTRY.Select(s => s.COUNTRY_ID).ToList().FirstOrDefault();

            if(COUNTRY_ID > 0)
            //if (countryRequest != null)
            {
                //var country = SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(countryRequest.COUNTRY_ID, context);
                var country = (from m in context.SAP_M_COUNTRY
                               where m.COUNTRY_ID == COUNTRY_ID
                               select new SAP_M_COUNTRY_2 { ZONE = m.ZONE, COUNTRY_CODE = m.COUNTRY_CODE }).FirstOrDefault();
                zone = country.ZONE + country.COUNTRY_CODE;
            }

            return CheckPICSO(context, soldToCode, shipToCode, zone);
        }

        public static Nullable<int> CheckPICArtwork(ARTWORKEntities context, ART_WF_ARTWORK_REQUEST artworkRequest)
        {
            var soldToId = artworkRequest.SOLD_TO_ID;
            var soldToCode = "";
            if (artworkRequest.SOLD_TO_ID > 0) soldToCode = (from m in context.XECM_M_CUSTOMER where m.CUSTOMER_ID == artworkRequest.SOLD_TO_ID select m.CUSTOMER_CODE).FirstOrDefault();
            //soldToCode = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(artworkRequest.SOLD_TO_ID, context).CUSTOMER_CODE;

            var shipToId = artworkRequest.SHIP_TO_ID;
            var shipToCode = "";
            if (artworkRequest.SHIP_TO_ID > 0) shipToCode = (from m in context.XECM_M_CUSTOMER where m.CUSTOMER_ID == artworkRequest.SHIP_TO_ID select m.CUSTOMER_CODE).FirstOrDefault();
            //shipToCode = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(artworkRequest.SHIP_TO_ID, context).CUSTOMER_CODE;

            var zone = "";
            //var countryRequest = ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_COUNTRY() { ARTWORK_REQUEST_ID = artworkRequest.ARTWORK_REQUEST_ID }, context).FirstOrDefault();
            var countryRequest = (from m in context.ART_WF_ARTWORK_REQUEST_COUNTRY
                                  where m.ARTWORK_REQUEST_ID == artworkRequest.ARTWORK_REQUEST_ID
                                  select new ART_WF_ARTWORK_REQUEST_COUNTRY_2 { COUNTRY_ID = m.COUNTRY_ID }).FirstOrDefault();

            if (countryRequest != null)
            {
                //var country = SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(countryRequest.COUNTRY_ID, context);
                var country = (from m in context.SAP_M_COUNTRY
                               where m.COUNTRY_ID == countryRequest.COUNTRY_ID
                               select new SAP_M_COUNTRY_2 { ZONE = m.ZONE, COUNTRY_CODE = m.COUNTRY_CODE }).FirstOrDefault();
                zone = country.ZONE + country.COUNTRY_CODE;
            }

            return CheckPICSO(context, soldToCode, shipToCode, zone);
        }

        public static Nullable<int> CheckPICSO(ARTWORKEntities context, string soldToCode, string shipToCode, string zone)
        {
            try
            {
                var tempZone = zone.Substring(0, 2);
                var tempCountry = zone.Substring(2, 2);

                var PIC = (from p in context.ART_M_PIC
                           where p.IS_ACTIVE == "X"
                           && p.ZONE == tempZone
                           && p.SOLD_TO_CODE == soldToCode
                           && p.SHIP_TO_CODE == shipToCode
                           && p.COUNTRY == tempCountry
                           select new ART_M_PIC_2 { USER_ID = p.USER_ID }).ToList();
                if (PIC.Count == 1)
                {
                    return PIC.FirstOrDefault().USER_ID;
                }

                PIC = (from p in context.ART_M_PIC
                       where p.IS_ACTIVE == "X"
                           && p.ZONE == tempZone
                           && p.SOLD_TO_CODE == soldToCode
                           && p.SHIP_TO_CODE == shipToCode
                           && string.IsNullOrEmpty(p.COUNTRY)
                       select new ART_M_PIC_2 { USER_ID = p.USER_ID }).ToList();
                if (PIC.Count == 1)
                {
                    return PIC.FirstOrDefault().USER_ID;
                }

                PIC = (from p in context.ART_M_PIC
                       where p.IS_ACTIVE == "X"
                           && p.ZONE == tempZone
                           && p.SOLD_TO_CODE == soldToCode
                           && string.IsNullOrEmpty(p.SHIP_TO_CODE)
                           && string.IsNullOrEmpty(p.COUNTRY)
                       select new ART_M_PIC_2 { USER_ID = p.USER_ID }).ToList();
                if (PIC.Count == 1)
                {
                    return PIC.FirstOrDefault().USER_ID;
                }

                PIC = (from p in context.ART_M_PIC
                       where p.IS_ACTIVE == "X"
                           && p.ZONE == tempZone
                            && string.IsNullOrEmpty(p.SOLD_TO_CODE)
                            && string.IsNullOrEmpty(p.SHIP_TO_CODE)
                            && string.IsNullOrEmpty(p.COUNTRY)
                       select new ART_M_PIC_2 { USER_ID = p.USER_ID }).ToList();
                if (PIC.Count == 1)
                {
                    return PIC.FirstOrDefault().USER_ID;
                }

                PIC = (from p in context.ART_M_PIC
                       where p.IS_ACTIVE == "X"
                            && string.IsNullOrEmpty(p.ZONE)
                            && string.IsNullOrEmpty(p.SOLD_TO_CODE)
                            && string.IsNullOrEmpty(p.SHIP_TO_CODE)
                            && string.IsNullOrEmpty(p.COUNTRY)
                       select new ART_M_PIC_2 { USER_ID = p.USER_ID }).ToList();
                return PIC.FirstOrDefault().USER_ID;
            }
            catch
            {
                return null;
                //var PIC = (from p in context.ART_M_PIC
                //           where p.IS_ACTIVE == "X"
                //               && string.IsNullOrEmpty(p.ZONE)
                //               && string.IsNullOrEmpty(p.SOLD_TO_CODE)
                //               && string.IsNullOrEmpty(p.SHIP_TO_CODE)
                //               && string.IsNullOrEmpty(p.COUNTRY)
                //           select new ART_M_PIC_2 { USER_ID = p.USER_ID }).ToList();
                //return PIC.FirstOrDefault().USER_ID;
            }
        }

        public static Nullable<int> CheckPICSO2(List<ART_M_PIC> allPICConfig, string soldToCode, string shipToCode, string zone)
        {
            try
            {
                var tempZone = zone.Substring(0, 2);
                var tempCountry = zone.Substring(2, 2);

                var PIC = (from p in allPICConfig
                           where p.IS_ACTIVE == "X"
                           && p.ZONE == tempZone
                           && p.SOLD_TO_CODE == soldToCode
                           && p.SHIP_TO_CODE == shipToCode
                           && p.COUNTRY == tempCountry
                           select p).ToList();
                if (PIC.Count == 1)
                {
                    return PIC.FirstOrDefault().USER_ID;
                }


                PIC = (from p in allPICConfig
                       where p.IS_ACTIVE == "X"
                           && p.ZONE == tempZone
                           && p.SOLD_TO_CODE == soldToCode
                           && p.SHIP_TO_CODE == shipToCode
                           && string.IsNullOrEmpty(p.COUNTRY)
                       select p).ToList();
                if (PIC.Count == 1)
                {
                    return PIC.FirstOrDefault().USER_ID;
                }


                PIC = (from p in allPICConfig
                       where p.IS_ACTIVE == "X"
                           && p.ZONE == tempZone
                           && p.SOLD_TO_CODE == soldToCode
                           && string.IsNullOrEmpty(p.SHIP_TO_CODE)
                           && string.IsNullOrEmpty(p.COUNTRY)
                       select p).ToList();
                if (PIC.Count == 1)
                {
                    return PIC.FirstOrDefault().USER_ID;
                }

                PIC = (from p in allPICConfig
                       where p.IS_ACTIVE == "X"
                           && p.ZONE == tempZone
                            && string.IsNullOrEmpty(p.SOLD_TO_CODE)
                            && string.IsNullOrEmpty(p.SHIP_TO_CODE)
                            && string.IsNullOrEmpty(p.COUNTRY)
                       select p).ToList();
                if (PIC.Count == 1)
                {
                    return PIC.FirstOrDefault().USER_ID;
                }

                PIC = (from p in allPICConfig
                       where p.IS_ACTIVE == "X"
                            && string.IsNullOrEmpty(p.ZONE)
                            && string.IsNullOrEmpty(p.SOLD_TO_CODE)
                            && string.IsNullOrEmpty(p.SHIP_TO_CODE)
                            && string.IsNullOrEmpty(p.COUNTRY)
                       select p).ToList();
                return PIC.FirstOrDefault().USER_ID;
            }
            catch
            {
                var PIC = (from p in allPICConfig
                           where p.IS_ACTIVE == "X"
                               && string.IsNullOrEmpty(p.ZONE)
                               && string.IsNullOrEmpty(p.SOLD_TO_CODE)
                               && string.IsNullOrEmpty(p.SHIP_TO_CODE)
                               && string.IsNullOrEmpty(p.COUNTRY)
                           select p).ToList();
                return PIC.FirstOrDefault().USER_ID;
            }
        }

        //public static bool HasPermission(int? UserId, string PermissionCode)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return HasPermission(UserId, PermissionCode, context);
        //    }
        //}

        public static bool HasPermission(int? UserId, string PermissionCode, ARTWORKEntities context)
        {
            if (UserId == null) return false;
            var res = false;

            //var listRole = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { USER_ID = Convert.ToInt32(UserId) }, context);
            var tempUser = Convert.ToInt32(UserId);
            var listRole = (from p in context.ART_M_USER_ROLE where p.USER_ID == tempUser select p.ROLE_ID).ToList();
            foreach (var item in listRole)
            {
                var temp = (from p in context.ART_M_PERMISSION where p.ROLE_ID == item && p.PERMISSION_CODE == PermissionCode select p.PERMISSION_ID).ToList();
                //if (ART_M_PERMISSION_SERVICE.GetByItem(new ART_M_PERMISSION() { ROLE_ID = item, PERMISSION_CODE = PermissionCode }, context).ToList().Count > 0)
                if (temp.Count > 0)
                {
                    return true;
                }
            }
            return res;
        }

        //public static bool HasPermissionWFFunction(int? UserId)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return HasPermissionWFFunction(UserId, context);
        //    }
        //}

        public static bool HasPermissionWFFunction(int? UserId, ARTWORKEntities context)
        {
            if (UserId == null) return false;

            if (BLL.Services.CNService.HasPermission(UserId, "DELEGATE", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "REASSIGN", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "REOPEN", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "RECALL", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "CHANGEOWNER", context)) return true;

            return false;
        }

        //public static bool HasPermissionMaster(int? UserId)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return HasPermissionMaster(UserId, context);
        //    }
        //}

        public static bool HasPermissionMaster(int? UserId, ARTWORKEntities context)
        {
            if (UserId == null) return false;

            if (BLL.Services.CNService.HasPermission(UserId, "DECISION", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "EMAILTEMPLATE", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "USERROLE", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "VENDORMASTER", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "CUSTOMERMASTER", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "ZONE", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "PIC", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "WORDINGTEMPLATE", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "STEP", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "PERMISSION", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "ATTACHMENT", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "REVEIVEEMAILNEWCUSTOMER", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "CUSTOMERINFO", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "VENDORINFO", context)) return true;

            return false;
        }

        //public static bool HasPermissionReport(int? UserId)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return HasPermissionReport(UserId, context);
        //    }
        //}

        public static bool HasPermissionReport(int? UserId, ARTWORKEntities context)
        {
            if (UserId == null) return false;

            if (BLL.Services.CNService.HasPermission(UserId, "TRACKING", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "ENDTOEND", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "COLLABORATION", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "SUMMARY", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "WAREHOUSE", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "OUTSTANDING", context)) return true;
            if (BLL.Services.CNService.HasPermission(UserId, "MATCONTROL", context)) return true;

            return false;
        }

        public static int GetWorkingDays(DateTime dtmStart, DateTime dtmEnd)
        {
            try
            {
                var totalDays = 0;
                if (dtmStart <= dtmEnd)
                {
                    for (var date = dtmStart; date < dtmEnd; date = date.AddDays(1))
                    {
                        if (date.DayOfWeek != DayOfWeek.Saturday
                            && date.DayOfWeek != DayOfWeek.Sunday)
                            totalDays++;
                    }

                    return totalDays;
                }
                else
                {
                    for (var date = dtmEnd; date < dtmStart; date = date.AddDays(1))
                    {
                        if (date.DayOfWeek != DayOfWeek.Saturday
                            && date.DayOfWeek != DayOfWeek.Sunday)
                            totalDays--;
                    }
                    totalDays = totalDays + 1;
                    return totalDays;
                }
            }
            catch { return 0; }
        }

        public static string GetEmailUserActive(int? UserId, ARTWORKEntities context)
        {
            if (UserId == null) { return ""; }
            //ART_M_USER filter = new ART_M_USER();
            //filter.USER_ID = Convert.ToInt32(UserId);
            //filter.IS_ACTIVE = "X";
            //var tempUser = ART_M_USER_SERVICE.GetByItem(filter, context).FirstOrDefault();
            var tempUserId = Convert.ToInt32(UserId);
            var EMAIL = (from p in context.ART_M_USER where p.USER_ID == tempUserId && p.IS_ACTIVE == "X" select p.EMAIL).FirstOrDefault();

            //if (tempUser != null)
            //{
            if (!string.IsNullOrEmpty(EMAIL))
                return EMAIL.Trim();
            else
                return "";
            //}
            //else
            //return "";
        }

        //public static string GetBOMNo(int bomID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return GetBOMNo(bomID, context);
        //    }
        //}

        public static string GetBOMNo(int bomID, ARTWORKEntities context)
        {
            string bomNO = "";

            var bom = (from m in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                       where m.PO_COMPLETE_SO_ITEM_COMPONENT_ID == bomID
                       select m.COMPONENT_MATERIAL).FirstOrDefault();

            if (bom != null)
            {
                bomNO = bom;
            }

            return bomNO;
        }

        public static string GetBOMITEMCUSTOM1(int bomID, ARTWORKEntities context)
        {
            string bomNO = "";

            var bom = (from m in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                       where m.PO_COMPLETE_SO_ITEM_COMPONENT_ID == bomID
                       select m.BOM_ITEM_CUSTOM_1).FirstOrDefault();

            if (bom != null)
            {
                bomNO = bom;
            }

            return bomNO;
        }

        public static string GetBOMNoAssign(int bomID, ARTWORKEntities context)
        {
            string bomNO = "";

            var temp = (from m in context.V_SAP_SALES_ORDER
                        where m.PO_COMPLETE_SO_ITEM_COMPONENT_ID == bomID
                        select new V_SAP_SALES_ORDER_2() { SALES_ORDER_NO = m.SALES_ORDER_NO, ITEM = m.ITEM, COMPONENT_ITEM = m.COMPONENT_ITEM }).FirstOrDefault();

            if (temp != null)
            {
                var bom = (from m in context.V_ART_ASSIGNED_SO
                           where m.SALES_ORDER_NO == temp.SALES_ORDER_NO && m.ITEM == temp.ITEM && m.COMPONENT_ITEM == temp.COMPONENT_ITEM
                           select m.COMPONENT_MATERIAL).FirstOrDefault();

                if (bom != null)
                {
                    bomNO = bom;
                }
            }
            return bomNO;
        }
        public static string GetBOMNoAssign2(int bomID, ARTWORKEntities context)
        {
            string bomNO = "";

            var temp = (from m in context.V_SAP_SALES_ORDER
                        where m.PO_COMPLETE_SO_ITEM_COMPONENT_ID == bomID
                        select new V_SAP_SALES_ORDER_2() { SALES_ORDER_NO = m.SALES_ORDER_NO, ITEM = m.ITEM, COMPONENT_ITEM = m.COMPONENT_ITEM }).FirstOrDefault();

            if (temp != null)
            {
                var bom = (from m in context.V_ART_ASSIGNED_SO
                           where m.SALES_ORDER_NO == temp.SALES_ORDER_NO && m.ITEM == temp.ITEM && m.COMPONENT_MATERIAL == temp.COMPONENT_MATERIAL
                           select m.COMPONENT_MATERIAL).FirstOrDefault();

                if (bom != null)
                {
                    bomNO = bom;
                }
            }
            return bomNO;
        }
        public static string GetBOMITEMCUSTOM1Assign(int bomID, ARTWORKEntities context)
        {
            string bomNO = "";

            var temp = (from m in context.V_SAP_SALES_ORDER
                        where m.PO_COMPLETE_SO_ITEM_COMPONENT_ID == bomID
                        select new V_SAP_SALES_ORDER_2() { SALES_ORDER_NO = m.SALES_ORDER_NO, ITEM = m.ITEM, COMPONENT_ITEM = m.COMPONENT_ITEM }).FirstOrDefault();

            if (temp != null)
            {
                var bom = (from m in context.V_ART_ASSIGNED_SO
                           where m.SALES_ORDER_NO == temp.SALES_ORDER_NO && m.ITEM == temp.ITEM && m.COMPONENT_ITEM == temp.COMPONENT_ITEM
                           select m.BOM_ITEM_CUSTOM_1).FirstOrDefault();

                if (bom != null)
                {
                    bomNO = bom;
                }
            }
            return bomNO;
        }

        //suppasit
        public static void InsertMaterialLock(string mat5, string salesOrder, string productCode, ARTWORKEntities context, string servicetype)
        {
            if (string.IsNullOrEmpty(productCode)) productCode = "";
            if (!string.IsNullOrEmpty(productCode))
            {
                if (!productCode.StartsWith("3")) { productCode = ""; }
            }
            if (string.IsNullOrEmpty(salesOrder)) { 
                salesOrder = "000";
            }

            var SOLD_TO = ""; var SHIP_TO = ""; var COUNTRY = ""; var BRAND = ""; var ZONE = ""; var PRODUCT_CODE = "";
            bool isNewMat5 = false; //#INC-92654

            if (!string.IsNullOrEmpty(mat5))
            {
                if (mat5.StartsWith("5"))
                {
                    if (mat5.Length == 18)
                    {
                        var modelHeader = ART_WF_ARTWORK_MATERIAL_LOCK_SERVICE.GetByItem(new ART_WF_ARTWORK_MATERIAL_LOCK() { MATERIAL_NO = mat5 }, context).FirstOrDefault();
                        if (modelHeader == null)
                        {
                            modelHeader = new ART_WF_ARTWORK_MATERIAL_LOCK();
                            modelHeader.MATERIAL_NO = mat5;
                            modelHeader.MATERIAL_DESCRIPTION = getMatDesc(mat5, context);

                            modelHeader.STATUS = "I";
                            modelHeader.UNLOCK_DATE_FROM = null;
                            modelHeader.UNLOCK_DATE_TO = null;
                            modelHeader.IS_ACTIVE = "X";
                            modelHeader.CREATE_BY = -1;
                            modelHeader.UPDATE_BY = -1;
                            isNewMat5 = true;  //#INC-92654
                        }
                        else
                        {
                            modelHeader.MATERIAL_DESCRIPTION = getMatDesc(mat5, context);
                            isNewMat5 = false;  //#INC-92654
                        }

                        if (string.IsNullOrEmpty(modelHeader.IS_HAS_FILES))
                        {
                            updateIS_HAS_FILES_WITHOUT_CHECK(mat5, context);
                        }

                        ART_WF_ARTWORK_MATERIAL_LOCK_SERVICE.SaveOrUpdateNoLog(modelHeader, context);

                        var soHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                        where h.SALES_ORDER_NO == salesOrder
                                        select h).FirstOrDefault();

                        if (soHeader != null)
                        {
                            var soItem = SAP_M_PO_COMPLETE_SO_ITEM_SERVICE.GetByItem(new SAP_M_PO_COMPLETE_SO_ITEM() { PO_COMPLETE_SO_HEADER_ID = soHeader.PO_COMPLETE_SO_HEADER_ID, PRODUCT_CODE = productCode }, context).FirstOrDefault();
                            if (soItem != null)
                            {
                                var listDetail = ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_SERVICE.GetByItem(new ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL() { MATERIAL_NO = mat5, SALES_ORDER_NO = "000" }, context);
                                foreach (var item in listDetail)
                                {
                                    ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_SERVICE.DeleteByMATERIAL_LOCK_DETAIL_ID(item.MATERIAL_LOCK_DETAIL_ID, context);
                                }

                                SOLD_TO = soHeader.SOLD_TO + ":" + soHeader.SOLD_TO_NAME;
                                SHIP_TO = soHeader.SHIP_TO + ":" + soHeader.SHIP_TO_NAME;
                                var country = SAP_M_COUNTRY_SERVICE.GetByItem(new SAP_M_COUNTRY() { COUNTRY_CODE = soItem.COUNTRY }, context).FirstOrDefault();
                                if (country != null)
                                    COUNTRY = country.NAME;
                                BRAND = soItem.BRAND_ID + ":" + soItem.BRAND_DESCRIPTION;

                                if (!string.IsNullOrEmpty(soItem.ZONE))
                                {
                                    if (soItem.ZONE.Length == 6)
                                    {
                                        ZONE = soItem.ZONE.Substring(0, 2);
                                    }
                                }
                                PRODUCT_CODE = soItem.PRODUCT_CODE;
                            }
                        }
                        else
                        {
                            var processPA = (from h in context.ART_WF_ARTWORK_PROCESS_PA
                                             where h.MATERIAL_NO == mat5
                                             orderby h.ARTWORK_SUB_ID descending
                                             select h).FirstOrDefault();
                            if (processPA != null)
                            {
                                var artworkData = (from h in context.V_ART_WF_DASHBOARD_ARTWORK
                                                   where h.ARTWORK_SUB_ID == processPA.ARTWORK_SUB_ID
                                                   orderby h.ARTWORK_SUB_ID descending
                                                   select new V_ART_WF_DASHBOARD_ARTWORK_2
                                                   {
                                                       SOLD_TO_DISPLAY_TXT = h.SOLD_TO_DISPLAY_TXT,
                                                       SHIP_TO_DISPLAY_TXT = h.SHIP_TO_DISPLAY_TXT,
                                                       ARTWORK_REQUEST_ID = h.ARTWORK_REQUEST_ID,
                                                       BRAND_DISPLAY_TXT = h.BRAND_DISPLAY_TXT,
                                                   }).FirstOrDefault();

                                if (artworkData != null)
                                {
                                    SOLD_TO = artworkData.SOLD_TO_DISPLAY_TXT;
                                    SHIP_TO = artworkData.SHIP_TO_DISPLAY_TXT;

                                    var listCountry = (from h in context.ART_WF_ARTWORK_REQUEST_COUNTRY
                                                       where h.ARTWORK_REQUEST_ID == artworkData.ARTWORK_REQUEST_ID
                                                       select h.COUNTRY_ID).ToList();

                                    foreach (var COUNTRY_ID in listCountry)
                                    {
                                        var country = SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(COUNTRY_ID, context);
                                        if (country != null)
                                        {
                                            if (COUNTRY == "") COUNTRY = country.NAME;
                                            else COUNTRY += ", " + country.NAME;

                                            if (ZONE == "") ZONE = country.ZONE;
                                            else ZONE += ", " + country.ZONE;
                                        }
                                    }
                                    BRAND = artworkData.BRAND_DISPLAY_TXT;

                                    var listProduct = (from h in context.ART_WF_ARTWORK_REQUEST_PRODUCT
                                                       where h.ARTWORK_REQUEST_ID == artworkData.ARTWORK_REQUEST_ID
                                                       select h.PRODUCT_CODE_ID).ToList();

                                    foreach (var PRODUCT_CODE_ID in listProduct)
                                    {
                                        var tempPRODUCT_CODE = (from h in context.XECM_M_PRODUCT
                                                                where h.XECM_PRODUCT_ID == PRODUCT_CODE_ID
                                                                select h.PRODUCT_CODE).FirstOrDefault();

                                        if (!string.IsNullOrEmpty(PRODUCT_CODE))
                                        {
                                            if (PRODUCT_CODE == "") PRODUCT_CODE = tempPRODUCT_CODE;
                                            else PRODUCT_CODE += ", " + tempPRODUCT_CODE;
                                        }
                                    }
                                }
                            }
                        }

                        var modelDetail = ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_SERVICE.GetByItem(new ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL()
                        {
                            MATERIAL_NO = mat5,
                            SALES_ORDER_NO = salesOrder,
                            PRODUCT_CODE = productCode
                        }, context).FirstOrDefault();

                        if (modelDetail == null)
                        {
                            modelDetail = new ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL();
                        }

                        modelDetail.MATERIAL_LOCK_ID = modelHeader.MATERIAL_LOCK_ID;
                        modelDetail.MATERIAL_NO = mat5;
                        modelDetail.SALES_ORDER_NO = salesOrder;
                        modelDetail.SOLD_TO = SOLD_TO;
                        modelDetail.SHIP_TO = SHIP_TO;
                        modelDetail.COUNTRY = COUNTRY;
                        modelDetail.BRAND = BRAND;
                        modelDetail.ZONE = ZONE;
                        modelDetail.PRODUCT_CODE = PRODUCT_CODE;
                        modelDetail.CREATE_BY = -1;
                        modelDetail.UPDATE_BY = -1;

                        ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_SERVICE.SaveOrUpdate(modelDetail, context);
                    }
                }
            }
            if (servicetype == "73")
                GetDataMaterialLock(mat5,isNewMat5, context);  //#INC-92654
            //GetDataMaterialLock(mat5, context);
        }

        public static void updateIS_HAS_FILES_WITHOUT_CHECK(string mat5, ARTWORKEntities context)
        {
            if (!string.IsNullOrEmpty(mat5))
            {
                if (mat5.StartsWith("5"))
                {
                    if (mat5.Length == 18)
                    {
                        var modelHeader = ART_WF_ARTWORK_MATERIAL_LOCK_SERVICE.GetByItem(new ART_WF_ARTWORK_MATERIAL_LOCK() { MATERIAL_NO = mat5 }, context).FirstOrDefault();
                        if (modelHeader != null)
                        {
                            var token = CWSService.getAuthToken();
                            modelHeader.IS_HAS_FILES = null;
                            string SecondaryPackagingNodeID = ConfigurationManager.AppSettings["SecondaryPackagingNodeID"];
                            string SecondaryPkgArtworkFolderName = ConfigurationManager.AppSettings["SecondaryPkgArtworkFolderName"];
                            var nodeMat5 = CWSService.getNodeByName(Convert.ToInt64(SecondaryPackagingNodeID), mat5 + " - " + modelHeader.MATERIAL_DESCRIPTION, token);
                            if (nodeMat5 != null)
                            {
                                var FinalArtwork = CWSService.getNodeByName(-nodeMat5.ID, SecondaryPkgArtworkFolderName, token);
                                if (FinalArtwork != null)
                                {
                                    var fileInNodeMat5 = CWSService.getAllNodeInFolder(FinalArtwork.ID, token);
                                    if (fileInNodeMat5 != null)
                                    {
                                        modelHeader.IS_HAS_FILES = "X";
                                    }
                                }
                            }

                            context.Database.ExecuteSqlCommand("UPDATE ART_WF_ARTWORK_MATERIAL_LOCK SET IS_HAS_FILES = '" + modelHeader.IS_HAS_FILES + "', UPDATE_DATE = getdate() WHERE MATERIAL_LOCK_ID  = " + modelHeader.MATERIAL_LOCK_ID);
                            //ART_WF_ARTWORK_MATERIAL_LOCK_SERVICE.SaveOrUpdateNoLog(modelHeader, context);
                        }
                    }
                }
            }
        }

        public static void updateIS_HAS_FILES_WITH_CHECK(string mat5)
        {
            using (var context = new ARTWORKEntities())
            {
                using (var dbContextTransaction = CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;
                    if (!string.IsNullOrEmpty(mat5))
                    {
                        if (mat5.StartsWith("5"))
                        {
                            if (mat5.Length == 18)
                            {
                                var modelHeader = new ART_WF_ARTWORK_MATERIAL_LOCK();
                                modelHeader = ART_WF_ARTWORK_MATERIAL_LOCK_SERVICE.GetByItem(new ART_WF_ARTWORK_MATERIAL_LOCK() { MATERIAL_NO = mat5 }, context).FirstOrDefault();
                                if (modelHeader != null)
                                {
                                    var token = CWSService.getAuthToken();
                                    //modelHeader.IS_HAS_FILES = null;
                                    string SecondaryPackagingNodeID = ConfigurationManager.AppSettings["SecondaryPackagingNodeID"];
                                    string SecondaryPkgArtworkFolderName = ConfigurationManager.AppSettings["SecondaryPkgArtworkFolderName"];
                                    var nodeMat5 = CWSService.getNodeByName(Convert.ToInt64(SecondaryPackagingNodeID), mat5 + " - " + modelHeader.MATERIAL_DESCRIPTION, token);
                                    if (nodeMat5 != null)
                                    {
                                        var FinalArtwork = CWSService.getNodeByName(-nodeMat5.ID, SecondaryPkgArtworkFolderName, token);
                                        if (FinalArtwork != null)
                                        {
                                            var fileInNodeMat5 = CWSService.getAllNodeInFolder(FinalArtwork.ID, token);
                                            if (fileInNodeMat5 != null && modelHeader.IS_HAS_FILES != "X")
                                            {
                                                modelHeader.IS_HAS_FILES = "X";
                                                context.Database.ExecuteSqlCommand("UPDATE ART_WF_ARTWORK_MATERIAL_LOCK SET IS_HAS_FILES = '" + modelHeader.IS_HAS_FILES + "' , UPDATE_DATE = getdate() WHERE MATERIAL_LOCK_ID  = " + modelHeader.MATERIAL_LOCK_ID);
                                            }
                                            else
                                            {
                                                if (fileInNodeMat5 == null && modelHeader.IS_HAS_FILES == "X")
                                                {
                                                    //modelHeader.IS_HAS_FILES = null;
                                                    context.Database.ExecuteSqlCommand("UPDATE ART_WF_ARTWORK_MATERIAL_LOCK SET IS_HAS_FILES = NULL, UPDATE_DATE = getdate() WHERE MATERIAL_LOCK_ID  = " + modelHeader.MATERIAL_LOCK_ID);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (FinalArtwork == null && modelHeader.IS_HAS_FILES == "X")
                                            {
                                                //modelHeader.IS_HAS_FILES = null;
                                                context.Database.ExecuteSqlCommand("UPDATE ART_WF_ARTWORK_MATERIAL_LOCK SET IS_HAS_FILES = NULL, UPDATE_DATE = getdate() WHERE MATERIAL_LOCK_ID  = " + modelHeader.MATERIAL_LOCK_ID);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (nodeMat5 == null && modelHeader.IS_HAS_FILES == "X")
                                        {
                                            //modelHeader.IS_HAS_FILES = null;
                                            context.Database.ExecuteSqlCommand("UPDATE ART_WF_ARTWORK_MATERIAL_LOCK SET IS_HAS_FILES = NULL, UPDATE_DATE = getdate() WHERE MATERIAL_LOCK_ID  = " + modelHeader.MATERIAL_LOCK_ID);
                                        }
                                    }

                                    //context.Database.ExecuteSqlCommand("UPDATE ART_WF_ARTWORK_MATERIAL_LOCK SET IS_HAS_FILES = '" + modelHeader.IS_HAS_FILES + "' WHERE MATERIAL_LOCK_ID  = " + modelHeader.MATERIAL_LOCK_ID);
                                    //ART_WF_ARTWORK_MATERIAL_LOCK_SERVICE.SaveOrUpdateNoLog(modelHeader, context);
                                }
                            }
                        }
                    }
                    dbContextTransaction.Commit();
                }
            }
        }

        private static void GetDataMaterialLock(string mat5,bool isNewMat5, ARTWORKEntities context)  // #INC-92654  by aof added parameter isNewMat5
        {
            if (!string.IsNullOrEmpty(mat5))
            {
                if (mat5.StartsWith("5"))
                {
                    if (mat5.Length == 18)
                    {
                        var modelHeader = ART_WF_ARTWORK_MATERIAL_LOCK_SERVICE.GetByItem(new ART_WF_ARTWORK_MATERIAL_LOCK() { MATERIAL_NO = mat5 }, context).FirstOrDefault();
                        if (modelHeader != null)
                        {
                            var processPA = (from h in context.ART_WF_ARTWORK_PROCESS_PA
                                             where h.MATERIAL_NO == mat5
                                             orderby h.ARTWORK_SUB_ID descending
                                             select h).FirstOrDefault();

                            if (processPA != null)
                            {
                                var processArtworkPA = (from h in context.ART_WF_ARTWORK_PROCESS
                                                        where h.ARTWORK_SUB_ID == processPA.ARTWORK_SUB_ID
                                                        && h.PARENT_ARTWORK_SUB_ID == null
                                                        select h).FirstOrDefault();

                                if (processArtworkPA != null)
                                {
                                    if (processArtworkPA.CURRENT_USER_ID > 0)
                                    {
                                        modelHeader.PIC = ART_M_USER_SERVICE.GetByUSER_ID(processArtworkPA.CURRENT_USER_ID, context).USERNAME;
                                    }

                                    var process = (from h in context.ART_WF_ARTWORK_PROCESS
                                                   join m in context.ART_WF_ARTWORK_PROCESS_PG on h.ARTWORK_SUB_ID equals m.ARTWORK_SUB_ID
                                                   where h.ARTWORK_ITEM_ID == processArtworkPA.ARTWORK_ITEM_ID
                                                   && m.ACTION_CODE == "SUBMIT"
                                                   orderby h.ARTWORK_SUB_ID descending
                                                   select h).FirstOrDefault();

                                    if (process != null)
                                    {
                                        var processPG = (from h in context.ART_WF_ARTWORK_PROCESS_PG
                                                         where h.ARTWORK_SUB_ID == process.ARTWORK_SUB_ID
                                                         && h.ACTION_CODE == "SUBMIT"
                                                         orderby h.ARTWORK_SUB_PA_ID descending
                                                         select h).FirstOrDefault();

                                        if (processPG != null)
                                        {
                                            var mockupId = processPG.DIE_LINE_MOCKUP_ID;
                                            var mockup = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(mockupId, context);

                                            if (mockup != null)
                                            {
                                                modelHeader.MOCKUP_NO = mockup.MOCKUP_NO;
                                                var processMockupPG = (from h in context.ART_WF_MOCKUP_PROCESS
                                                                       where h.MOCKUP_ID == mockupId
                                                                       && h.PARENT_MOCKUP_SUB_ID == null
                                                                       select h).FirstOrDefault();

                                                if (processMockupPG != null)
                                                {
                                                    modelHeader.MOCKUP_ID = processMockupPG.MOCKUP_SUB_ID;
                                                    if (processMockupPG.CURRENT_USER_ID > 0)
                                                        modelHeader.PG_OWNER = ART_M_USER_SERVICE.GetByUSER_ID(processMockupPG.CURRENT_USER_ID, context).USERNAME;
                                                }
                                            }
                                        }
                                    }

                                    var artworkData = (from h in context.V_ART_WF_DASHBOARD_ARTWORK
                                                       where h.ARTWORK_SUB_ID == processPA.ARTWORK_SUB_ID
                                                       orderby h.ARTWORK_SUB_ID descending
                                                       select new V_ART_WF_DASHBOARD_ARTWORK_2
                                                       {
                                                           ARTWORK_REQUEST_NO = h.ARTWORK_REQUEST_NO,
                                                           REQUEST_ITEM_NO = h.REQUEST_ITEM_NO,
                                                           PRIMARY_TYPE_ID = h.PRIMARY_TYPE_ID,
                                                           ARTWORK_SUB_ID = h.ARTWORK_SUB_ID,
                                                           ARTWORK_REQUEST_ID = h.ARTWORK_REQUEST_ID,

                                                       }).FirstOrDefault();

                                    if (artworkData != null)
                                    {
                                        modelHeader.ARTWORK_NO = artworkData.REQUEST_ITEM_NO;
                                        modelHeader.REQUEST_FORM_NO = artworkData.ARTWORK_REQUEST_NO;
                                        modelHeader.ARTWORK_ID = artworkData.ARTWORK_SUB_ID;
                                        modelHeader.REQUEST_FORM_ID = artworkData.ARTWORK_REQUEST_ID;
                                    }

                                    if (processPA != null) modelHeader.PACKAGING_TYPE = CNService.GetCharacteristicDescription(processPA.MATERIAL_GROUP_ID, context);
                                    if (artworkData != null) modelHeader.PRIMARY_TYPE = CNService.GetCharacteristicDescription(artworkData.PRIMARY_TYPE_ID, context);

                                    if (processPA != null)
                                    {
                                        if (processPA.THREE_P_ID > 0)
                                        {
                                            modelHeader.PRIMARY_SIZE = SAP_M_3P_SERVICE.GetByTHREE_P_ID(processPA.THREE_P_ID, context).PRIMARY_SIZE_DESCRIPTION;
                                        }
                                        else
                                        {
                                            modelHeader.PRIMARY_SIZE = processPA.PRIMARY_SIZE_OTHER;
                                        }

                                        if (processPA.TWO_P_ID > 0)
                                        {
                                            var temp2P = SAP_M_2P_SERVICE.GetByTWO_P_ID(processPA.TWO_P_ID, context);
                                            if (temp2P != null)
                                            {
                                                modelHeader.PACKAGING_STYLE = temp2P.PACKING_SYLE_DESCRIPTION;
                                                modelHeader.PACK_SIZE = temp2P.PACK_SIZE_DESCRIPTION;
                                            }
                                        }
                                        else
                                        {
                                            modelHeader.PACKAGING_STYLE = processPA.PACKING_STYLE_OTHER;
                                            if (processPA.PACK_SIZE_ID > 0)
                                            {
                                                modelHeader.PACK_SIZE = CNService.GetCharacteristicDescription(processPA.PACK_SIZE_ID, context);
                                            }
                                            else
                                            {
                                                modelHeader.PACK_SIZE = processPA.PACK_SIZE_OTHER;
                                            }
                                        }
                                    }

                                    ART_WF_ARTWORK_MATERIAL_LOCK_SERVICE.SaveOrUpdateNoLog(modelHeader, context);
                                }
                            }

                            if (isNewMat5)   //#INC-92654  by aof
                            {
                                if (1 == 1)
                                {
                                    //try to change status
                                    var mat5NoVersionN = CNService.SubString(mat5.Remove(8, 1).Insert(8, "N"), 16);
                                    var mat5NoVersionC = CNService.SubString(mat5.Remove(8, 1).Insert(8, "C"), 16);

                                    var listMat5 = (from h in context.ART_WF_ARTWORK_MATERIAL_LOCK
                                                    where h.MATERIAL_NO.Contains(mat5NoVersionN) || h.MATERIAL_NO.Contains(mat5NoVersionC)
                                                    select h).ToList();

                                    listMat5 = listMat5.OrderByDescending(m => int.Parse(m.MATERIAL_NO.Substring(16, 2))).ToList();
                                    var first = true;
                                    foreach (var item in listMat5)
                                    {
                                        //if (item.UPDATE_BY == -1)  //commented by aof #INC-52613
                                        //{                          //commented by aof #INC-52613
                                        if (first)
                                        {
                                            item.STATUS = "I";

                                            item.UPDATE_DATE_LOCK = DateTime.Now;   //added by aof ticket#440197,440423
                                            item.UPDATE_BY_LOCK = -2;               //added by aof ticket#440197,440423
                                            ART_WF_ARTWORK_MATERIAL_LOCK_SERVICE.SaveOrUpdateNoLog(item, context);
                                            first = false;
                                        }
                                        else
                                        {
                                            item.STATUS = "O";

                                            item.UPDATE_DATE_LOCK = DateTime.Now;   //added by aof ticket#440197,440423
                                            item.UPDATE_BY_LOCK = -2;               //added by aof ticket#440197,440423

                                            ART_WF_ARTWORK_MATERIAL_LOCK_SERVICE.SaveOrUpdateNoLog(item, context);
                                        }
                                        //}    //commented by aof #INC-52613
                                        //else  //commented by aof #INC-52613
                                        //{  //commented by aof #INC-52613
                                        //    if (first)  //commented by aof #INC-52613
                                        //    {  //commented by aof #INC-52613
                                        //        first = false;  //commented by aof #INC-52613
                                        //    }  //commented by aof #INC-52613
                                        //}  //commented by aof #INC-52613

                                        //if (item.UPDATE_BY == -1)
                                        //{
                                        //    if (first)
                                        //    {
                                        //        item.STATUS = "I";
                                        //        ART_WF_ARTWORK_MATERIAL_LOCK_SERVICE.SaveOrUpdateNoLog(item, context);
                                        //        first = false;
                                        //    }
                                        //    else
                                        //    {
                                        //        item.STATUS = "O";
                                        //        ART_WF_ARTWORK_MATERIAL_LOCK_SERVICE.SaveOrUpdateNoLog(item, context);
                                        //    }
                                        //}
                                        //else
                                        //{
                                        //    if (first)
                                        //    {
                                        //        first = false;
                                        //    }
                                        //}
                                    }
                                }
                            }

                         
                        }
                    }
                }
            }
        }

        //public static bool IsLock(int artworkSubId)
        //{
        //    string MATERIAL_NO = "";
        //    using (var context = new ARTWORKEntities())
        //    {
        //        var parentId = FindParentArtworkSubId(artworkSubId, context);
        //        MATERIAL_NO = (from h in context.ART_WF_ARTWORK_PROCESS_PA
        //                       where h.ARTWORK_SUB_ID == parentId
        //                       select h.MATERIAL_NO).FirstOrDefault();
        //    }

        //    if (!string.IsNullOrEmpty(MATERIAL_NO))
        //    {
        //        if (MATERIAL_NO.StartsWith("5"))
        //        {
        //            if (MATERIAL_NO.Length == 18)
        //            {
        //                return IsLock(MATERIAL_NO);
        //            }
        //        }
        //    }

        //    return false;
        //}

        public static bool IsLock(int artworkSubId, ARTWORKEntities context)
        {
            string MATERIAL_NO = "";

            var parentId = FindParentArtworkSubId(artworkSubId, context);
            MATERIAL_NO = (from h in context.ART_WF_ARTWORK_PROCESS_PA
                           where h.ARTWORK_SUB_ID == parentId
                           select h.MATERIAL_NO).FirstOrDefault();


            if (!string.IsNullOrEmpty(MATERIAL_NO))
            {
                if (MATERIAL_NO.StartsWith("5"))
                {
                    if (MATERIAL_NO.Length == 18)
                    {
                        return IsLock(MATERIAL_NO, context);
                    }
                }
            }

            return false;
        }

        public static bool IsLock(string mat5, ARTWORKEntities context)
        {
            if (!string.IsNullOrEmpty(mat5))
            {
                if (mat5.StartsWith("5"))
                {
                    if (mat5.Length == 18)
                    {
                        var modelHeader = (from h in context.ART_WF_ARTWORK_MATERIAL_LOCK
                                           where h.MATERIAL_NO == mat5
                                           select h).FirstOrDefault();

                        if (modelHeader != null)
                        {
                            if (modelHeader.STATUS == "O")
                            {
                                if (modelHeader.UNLOCK_DATE_FROM == null && modelHeader.UNLOCK_DATE_TO == null)
                                {
                                    return true;
                                }
                                else
                                {
                                    if (modelHeader.UNLOCK_DATE_FROM.Value.Date <= DateTime.Now.Date && DateTime.Now.Date <= modelHeader.UNLOCK_DATE_TO.Value.Date)
                                    {

                                    }
                                    else
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        //suppasit

        public static void UpdateMaterialLock(int artwork_sud_id)
        {
            Thread thread = new Thread(delegate ()
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var tempProcess = (from q in context.ART_WF_ARTWORK_PROCESS
                                           where q.ARTWORK_SUB_ID == artwork_sud_id
                                           select q).FirstOrDefault();

                        if (tempProcess != null)
                        {
                            var mainProcessPA = (from q in context.ART_WF_ARTWORK_PROCESS
                                                 where q.ARTWORK_ITEM_ID == tempProcess.ARTWORK_ITEM_ID
                                                 && q.PARENT_ARTWORK_SUB_ID == null
                                                 select q).FirstOrDefault();

                            if (mainProcessPA != null)
                            {
                                var processPA = (from q in context.ART_WF_ARTWORK_PROCESS_PA
                                                 where q.ARTWORK_SUB_ID == mainProcessPA.ARTWORK_SUB_ID
                                                 select q).FirstOrDefault();

                                if (processPA != null)
                                {
                                    var soDetails = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                                     where s.ARTWORK_SUB_ID == processPA.ARTWORK_SUB_ID
                                                     select s).ToList();

                                    if (soDetails.Count > 0)
                                    {
                                        foreach (var iSO in soDetails)
                                        {
                                            if (iSO.BOM_ID > 0)
                                            {
                                                var mat5 = CNService.GetBOMNo(Convert.ToInt32(iSO.BOM_ID), context);
                                                CNService.InsertMaterialLock(mat5, iSO.SALES_ORDER_NO, iSO.MATERIAL_NO, context,"");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        CNService.InsertMaterialLock(processPA.MATERIAL_NO, "000", "000", context,"");
                                    }
                                }
                            }
                        }

                        dbContextTransaction.Commit();
                    }
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        public static void RepairMaterialBOM_1(SO_HEADER soHeader, string mat5FromWorkflow, ARTWORKEntities context)
        {
            string dt = mat5FromWorkflow;
            if (!string.IsNullOrEmpty(mat5FromWorkflow))
            {
                if (mat5FromWorkflow.StartsWith("5"))
                {
                    if (mat5FromWorkflow.Length == 18)
                    {
                        saveLogRepairMaterialBOM("Start", null, null, context, dt);

                        var subIDs = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                      join m in context.ART_WF_ARTWORK_PROCESS on p.ARTWORK_SUB_ID equals m.ARTWORK_SUB_ID
                                      join a in context.V_ART_ASSIGNED_SO on p.ARTWORK_SUB_ID equals a.ARTWORK_SUB_ID
                                      where p.MATERIAL_NO == mat5FromWorkflow && a.SALES_ORDER_NO == soHeader.SALES_ORDER_NO
                                      //&& string.IsNullOrEmpty(m.IS_END)
                                      select p.ARTWORK_SUB_ID).ToList();

                        var listSODetails = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                             where subIDs.Contains(s.ARTWORK_SUB_ID)
                                             select s).ToList();

                        var listSONO = listSODetails.Select(s => s.SALES_ORDER_NO).Distinct().ToList();

                        var listBomNewIDs = (from b in context.V_SAP_SALES_ORDER
                                             where listSONO.Contains(b.SALES_ORDER_NO)
                                             select new V_SAP_SALES_ORDER_2
                                             {
                                                 COMPONENT_MATERIAL = b.COMPONENT_MATERIAL,
                                                 SALES_ORDER_NO = b.SALES_ORDER_NO,
                                                 ITEM = b.ITEM,
                                                 PO_COMPLETE_SO_ITEM_COMPONENT_ID = b.PO_COMPLETE_SO_ITEM_COMPONENT_ID,
                                                 BOM_IS_ACTIVE = b.BOM_IS_ACTIVE,
                                                 BOM_ITEM_CUSTOM_1 = b.BOM_ITEM_CUSTOM_1
                                             }).Distinct().ToList();

                        var doWork = false;
                        foreach (var subID in subIDs)
                        {
                            var soDetails = (from s in listSODetails where s.ARTWORK_SUB_ID == subID select s).ToList();

                            foreach (var iSODetail in soDetails)
                            {
                                if (String.IsNullOrEmpty(iSODetail.BOM_NO) && iSODetail.BOM_ID > 0)
                                {
                                    decimal? itemNO = 0;
                                    if (!String.IsNullOrEmpty(iSODetail.SALES_ORDER_ITEM))
                                    {
                                        itemNO = Convert.ToDecimal(iSODetail.SALES_ORDER_ITEM);
                                    }

                                    var temp = listBomNewIDs.Where(c => c.PO_COMPLETE_SO_ITEM_COMPONENT_ID == iSODetail.BOM_ID).FirstOrDefault();
                                    if (temp != null)
                                    {
                                        var componentMatNo = temp.COMPONENT_MATERIAL;
                                        var bomNewIDs = (from b in listBomNewIDs
                                                         where b.COMPONENT_MATERIAL == mat5FromWorkflow
                                                         && b.SALES_ORDER_NO == iSODetail.SALES_ORDER_NO
                                                         && b.ITEM == itemNO
                                                         && b.BOM_IS_ACTIVE == "X"
                                                         && !b.BOM_ITEM_CUSTOM_1.Contains("MULTI")
                                                         select b.PO_COMPLETE_SO_ITEM_COMPONENT_ID).ToList();

                                        if (bomNewIDs.Count == 0)
                                        {
                                            bomNewIDs = (from b in listBomNewIDs
                                                         where b.COMPONENT_MATERIAL == mat5FromWorkflow
                                                          && b.SALES_ORDER_NO == iSODetail.SALES_ORDER_NO
                                                          && b.BOM_IS_ACTIVE == "X"
                                                          && !b.BOM_ITEM_CUSTOM_1.Contains("MULTI")
                                                         select b.PO_COMPLETE_SO_ITEM_COMPONENT_ID).ToList();
                                        }

                                        if (bomNewIDs.Count == 1)
                                        {
                                            var doWork2 = false;
                                            if (!iSODetail.BOM_ID.Equals(bomNewIDs[0]))
                                            {
                                                doWork2 = true;
                                            }
                                            else if (GetBOMNo(Convert.ToInt32(iSODetail.BOM_ID), context) != GetBOMNoAssign(Convert.ToInt32(bomNewIDs[0]), context))
                                            {
                                                doWork2 = true;
                                            }

                                            if (doWork2)
                                            {
                                                var oldValue = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
                                                var newValue = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();

                                                oldValue = iSODetail;

                                                iSODetail.BOM_ID = bomNewIDs[0];
                                                ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.SaveOrUpdateNoLog(iSODetail, context);

                                                newValue = iSODetail;

                                                saveLogRepairMaterialBOM("Add", newValue, oldValue, context, dt + " " + iSODetail.SALES_ORDER_NO + "(" + iSODetail.SALES_ORDER_ITEM + ")");
                                                doWork = true;
                                            }
                                        }
                                        else
                                        {
                                            saveLogRepairMaterialBOM("Found (" + bomNewIDs.Count + ")", iSODetail, null, context, dt + " " + iSODetail.SALES_ORDER_NO + "(" + iSODetail.SALES_ORDER_ITEM + ")");
                                        }
                                    }
                                }
                                else if (!String.IsNullOrEmpty(iSODetail.BOM_NO) && iSODetail.BOM_NO.Contains("FOC"))
                                {
                                    if (!iSODetail.MATERIAL_NO.Equals(mat5FromWorkflow))
                                    {
                                        var oldValue = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
                                        oldValue = iSODetail;

                                        ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.DeleteByARTWORK_PROCESS_SO_ID(iSODetail.ARTWORK_PROCESS_SO_ID, context);

                                        saveLogRepairMaterialBOM("Delete FOC", null, oldValue, context, dt + " " + iSODetail.SALES_ORDER_NO + "(" + iSODetail.SALES_ORDER_ITEM + ")");
                                        doWork = true;
                                    }
                                }
                            }

                            if (doWork)
                            {
                                if (soDetails.Count > 0)
                                {
                                    var soDetail2 = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
                                    //soDetail2 = MapperServices.ART_WF_ARTWORK_PROCESS_SO_DETAIL(soDetails[0]);
                                     
                                    soDetail2.ARTWORK_PROCESS_SO_ID = soDetails[0].ARTWORK_PROCESS_SO_ID;
                                    soDetail2.ARTWORK_REQUEST_ID = soDetails[0].ARTWORK_REQUEST_ID;
                                    soDetail2.ARTWORK_SUB_ID = soDetails[0].ARTWORK_SUB_ID;
                                    soDetail2.BOM_ID = soDetails[0].BOM_ID;
                                    soDetail2.BOM_NO = string.Format("{0}", soDetails[0].BOM_NO);
                                    soDetail2.CREATE_BY = soDetails[0].CREATE_BY;
                                    soDetail2.CREATE_DATE = soDetails[0].CREATE_DATE;
                                    soDetail2.MATERIAL_NO = soDetails[0].MATERIAL_NO;
                                    soDetail2.SALES_ORDER_ITEM = soDetails[0].SALES_ORDER_ITEM;
                                    
                                    soDetail2.SALES_ORDER_NO = soDetails[0].SALES_ORDER_NO;
                                    soDetail2.UPDATE_BY = soDetails[0].UPDATE_BY;
                                    soDetail2.UPDATE_DATE = soDetails[0].UPDATE_DATE;

                                    SalesOrderHelper.DeleteAssignSalesOrder(soDetail2, context);
                                    SalesOrderHelper.CopyAssignSalesOrder(soDetail2, context);

                                    saveLogRepairMaterialBOM("Completed", null, null, context, dt);
                                }
                            }
                            else
                            {
                                saveLogRepairMaterialBOM("Nothing", null, null, context, dt);
                            }
                        }

                        if (subIDs.Count == 0)
                        {
                            saveLogRepairMaterialBOM("Not Found WF", null, null, context, dt);
                        }
                    }
                }
            }
        }

        //public static void RepairMaterialBOM(string mat5FromWorkflow, ARTWORKEntities context)
        //{
        //    string dt = mat5FromWorkflow;
        //    if (!string.IsNullOrEmpty(mat5FromWorkflow))
        //    {
        //        if (mat5FromWorkflow.StartsWith("5"))
        //        {
        //            if (mat5FromWorkflow.Length == 18)
        //            {
        //                saveLogRepairMaterialBOM("Start", null, null, context, dt);

        //                var subIDs = (from p in context.ART_WF_ARTWORK_PROCESS_PA
        //                              join m in context.ART_WF_ARTWORK_PROCESS on p.ARTWORK_SUB_ID equals m.ARTWORK_SUB_ID
        //                              where p.MATERIAL_NO == mat5FromWorkflow
        //                              && string.IsNullOrEmpty(m.IS_END)
        //                              select p.ARTWORK_SUB_ID).ToList();

        //                var listSODetails = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
        //                                     where subIDs.Contains(s.ARTWORK_SUB_ID)
        //                                     select s).ToList();

        //                var listSONO = listSODetails.Select(s => s.SALES_ORDER_NO).Distinct().ToList();

        //                var listBomNewIDs = (from b in context.V_SAP_SALES_ORDER
        //                                     where listSONO.Contains(b.SALES_ORDER_NO)
        //                                     select new V_SAP_SALES_ORDER_2
        //                                     {
        //                                         COMPONENT_MATERIAL = b.COMPONENT_MATERIAL,
        //                                         SALES_ORDER_NO = b.SALES_ORDER_NO,
        //                                         ITEM = b.ITEM,
        //                                         PO_COMPLETE_SO_ITEM_COMPONENT_ID = b.PO_COMPLETE_SO_ITEM_COMPONENT_ID,
        //                                         BOM_IS_ACTIVE = b.BOM_IS_ACTIVE,
        //                                         BOM_ITEM_CUSTOM_1 = b.BOM_ITEM_CUSTOM_1
        //                                     }).Distinct().ToList();

        //                var doWork = false;
        //                foreach (var subID in subIDs)
        //                {
        //                    var soDetails = (from s in listSODetails where s.ARTWORK_SUB_ID == subID select s).ToList();

        //                    foreach (var iSODetail in soDetails)
        //                    {
        //                        if (String.IsNullOrEmpty(iSODetail.BOM_NO) && iSODetail.BOM_ID > 0)
        //                        {
        //                            decimal? itemNO = 0;
        //                            if (!String.IsNullOrEmpty(iSODetail.SALES_ORDER_ITEM))
        //                            {
        //                                itemNO = Convert.ToDecimal(iSODetail.SALES_ORDER_ITEM);
        //                            }

        //                            var temp = listBomNewIDs.Where(c => c.PO_COMPLETE_SO_ITEM_COMPONENT_ID == iSODetail.BOM_ID).FirstOrDefault();
        //                            if (temp != null)
        //                            {
        //                                var componentMatNo = temp.COMPONENT_MATERIAL;
        //                                var bomNewIDs = (from b in listBomNewIDs
        //                                                 where b.COMPONENT_MATERIAL == mat5FromWorkflow
        //                                                 && b.SALES_ORDER_NO == iSODetail.SALES_ORDER_NO
        //                                                 && b.ITEM == itemNO
        //                                                 && b.BOM_IS_ACTIVE == "X"
        //                                                 && !b.BOM_ITEM_CUSTOM_1.Contains("MULTI")
        //                                                 select b.PO_COMPLETE_SO_ITEM_COMPONENT_ID).ToList();

        //                                if (bomNewIDs.Count == 0)
        //                                {
        //                                    bomNewIDs = (from b in listBomNewIDs
        //                                                 where b.COMPONENT_MATERIAL == mat5FromWorkflow
        //                                                  && b.SALES_ORDER_NO == iSODetail.SALES_ORDER_NO
        //                                                  && b.BOM_IS_ACTIVE == "X"
        //                                                  && !b.BOM_ITEM_CUSTOM_1.Contains("MULTI")
        //                                                 select b.PO_COMPLETE_SO_ITEM_COMPONENT_ID).ToList();
        //                                }

        //                                if (bomNewIDs.Count == 1)
        //                                {
        //                                    var doWork2 = false;
        //                                    if (!iSODetail.BOM_ID.Equals(bomNewIDs[0]))
        //                                    {
        //                                        doWork2 = true;
        //                                    }
        //                                    else if (GetBOMNo(Convert.ToInt32(iSODetail.BOM_ID), context) != GetBOMNoAssign(Convert.ToInt32(bomNewIDs[0]), context))
        //                                    {
        //                                        doWork2 = true;
        //                                    }

        //                                    if (doWork2)
        //                                    {
        //                                        var oldValue = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
        //                                        var newValue = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();

        //                                        oldValue = iSODetail;

        //                                        iSODetail.BOM_ID = bomNewIDs[0];
        //                                        ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.SaveOrUpdateNoLog(iSODetail, context);

        //                                        newValue = iSODetail;

        //                                        saveLogRepairMaterialBOM("Add", newValue, oldValue, context, dt + " " + iSODetail.SALES_ORDER_NO + "(" + iSODetail.SALES_ORDER_ITEM + ")");
        //                                        doWork = true;
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    saveLogRepairMaterialBOM("Found (" + bomNewIDs.Count + ")", iSODetail, null, context, dt + " " + iSODetail.SALES_ORDER_NO + "(" + iSODetail.SALES_ORDER_ITEM + ")");
        //                                }
        //                            }
        //                        }
        //                        else if (!String.IsNullOrEmpty(iSODetail.BOM_NO) && iSODetail.BOM_NO.Contains("FOC"))
        //                        {
        //                            if (!iSODetail.MATERIAL_NO.Equals(mat5FromWorkflow))
        //                            {
        //                                var oldValue = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
        //                                oldValue = iSODetail;

        //                                ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.DeleteByARTWORK_PROCESS_SO_ID(iSODetail.ARTWORK_PROCESS_SO_ID, context);

        //                                saveLogRepairMaterialBOM("Delete FOC", null, oldValue, context, dt + " " + iSODetail.SALES_ORDER_NO + "(" + iSODetail.SALES_ORDER_ITEM + ")");
        //                                doWork = true;
        //                            }
        //                        }
        //                    }

        //                    if (doWork)
        //                    {
        //                        if (soDetails.Count > 0)
        //                        {
        //                            var soDetail2 = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
        //                            soDetail2 = MapperServices.ART_WF_ARTWORK_PROCESS_SO_DETAIL(soDetails[0]);

        //                            SalesOrderHelper.DeleteAssignSalesOrder(soDetail2, context);
        //                            SalesOrderHelper.CopyAssignSalesOrder(soDetail2, context);

        //                            saveLogRepairMaterialBOM("Completed", null, null, context, dt);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        saveLogRepairMaterialBOM("Nothing", null, null, context, dt);
        //                    }
        //                }

        //                if (subIDs.Count == 0)
        //                {
        //                    saveLogRepairMaterialBOM("Not Found WF", null, null, context, dt);
        //                }
        //            }
        //        }
        //    }
        //}

        //public static void RepairMaterialBOM2(string SONumber, string SOItemStr, string bomItem, string newText, string refMat, ARTWORKEntities context)
        //{
        //    decimal SOItem = Convert.ToDecimal(SOItemStr);
        //    string dt = SONumber + " " + SOItem + " " + bomItem + " " + newText;
        //    if (!string.IsNullOrEmpty(newText))
        //    {
        //        saveLogRepairMaterialBOM("Start", null, null, context, dt);

        //        var subIDs = (from b in context.V_ART_ASSIGNED_SO
        //                      join m in context.ART_WF_ARTWORK_PROCESS on b.ARTWORK_SUB_ID equals m.ARTWORK_SUB_ID
        //                      where b.SALES_ORDER_NO == SONumber && b.ITEM == SOItem && b.BOM_ITEM_CUSTOM_1 == newText
        //                      && string.IsNullOrEmpty(m.IS_END)
        //                      select b.ARTWORK_SUB_ID).Distinct().ToList();

        //        if (!string.IsNullOrEmpty(refMat))
        //        {
        //            if (newText.Contains("MULTI"))
        //            {
        //                subIDs = (from b in context.V_ART_ASSIGNED_SO
        //                          join m in context.ART_WF_ARTWORK_PROCESS on b.ARTWORK_SUB_ID equals m.ARTWORK_SUB_ID
        //                          where b.SALES_ORDER_NO == SONumber && b.ITEM == SOItem && b.BOM_ITEM_CUSTOM_1 == newText
        //                               && string.IsNullOrEmpty(m.IS_END) && b.COMPONENT_MATERIAL == refMat
        //                          select b.ARTWORK_SUB_ID).Distinct().ToList();
        //            }
        //        }

        //        var listSODetails = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
        //                             where subIDs.Contains(s.ARTWORK_SUB_ID)
        //                             select s).ToList();

        //        var listSONO = listSODetails.Select(s => s.SALES_ORDER_NO).Distinct().ToList();

        //        var listBomNewIDs = (from b in context.V_SAP_SALES_ORDER
        //                             where listSONO.Contains(b.SALES_ORDER_NO)
        //                             select new V_SAP_SALES_ORDER_2
        //                             {
        //                                 COMPONENT_MATERIAL = b.COMPONENT_MATERIAL,
        //                                 SALES_ORDER_NO = b.SALES_ORDER_NO,
        //                                 ITEM = b.ITEM,
        //                                 PO_COMPLETE_SO_ITEM_COMPONENT_ID = b.PO_COMPLETE_SO_ITEM_COMPONENT_ID,
        //                                 BOM_IS_ACTIVE = b.BOM_IS_ACTIVE,
        //                                 BOM_ITEM_CUSTOM_1 = b.BOM_ITEM_CUSTOM_1
        //                             }).Distinct().ToList();

        //        var doWork = false;
        //        foreach (var subID in subIDs)
        //        {
        //            var soDetails = (from s in listSODetails where s.ARTWORK_SUB_ID == subID select s).ToList();

        //            foreach (var iSODetail in soDetails)
        //            {
        //                if (String.IsNullOrEmpty(iSODetail.BOM_NO) && iSODetail.BOM_ID > 0)
        //                {
        //                    decimal? itemNO = 0;
        //                    if (!String.IsNullOrEmpty(iSODetail.SALES_ORDER_ITEM))
        //                    {
        //                        itemNO = Convert.ToDecimal(iSODetail.SALES_ORDER_ITEM);
        //                    }

        //                    var temp = listBomNewIDs.Where(c => c.PO_COMPLETE_SO_ITEM_COMPONENT_ID == iSODetail.BOM_ID).FirstOrDefault();
        //                    if (temp != null)
        //                    {
        //                        var componentMatNo = temp.COMPONENT_MATERIAL;
        //                        var bomNewIDs = (from b in listBomNewIDs
        //                                         where b.SALES_ORDER_NO == iSODetail.SALES_ORDER_NO
        //                                         && b.ITEM == itemNO
        //                                         && b.BOM_IS_ACTIVE == "X"
        //                                         && b.BOM_ITEM_CUSTOM_1 == newText
        //                                         select b.PO_COMPLETE_SO_ITEM_COMPONENT_ID).ToList();

        //                        var doWorkFixing = false;
        //                        if (newText.Contains("MULTI"))
        //                        {
        //                            doWorkFixing = true;

        //                            if (!string.IsNullOrEmpty(refMat))
        //                            {
        //                                bomNewIDs = (from b in listBomNewIDs
        //                                             where b.SALES_ORDER_NO == iSODetail.SALES_ORDER_NO
        //                                             && b.ITEM == itemNO
        //                                             && b.BOM_IS_ACTIVE == "X"
        //                                             && b.BOM_ITEM_CUSTOM_1 == newText
        //                                             && b.COMPONENT_MATERIAL == refMat
        //                                             select b.PO_COMPLETE_SO_ITEM_COMPONENT_ID).ToList();
        //                            }
        //                        }
        //                        if (bomNewIDs.Count == 1)
        //                        {
        //                            doWorkFixing = true;
        //                        }

        //                        if (doWorkFixing)
        //                        {
        //                            if (bomNewIDs.Count > 0)
        //                            {
        //                                var doWork2 = false;
        //                                if (!iSODetail.BOM_ID.Equals(bomNewIDs[0]))
        //                                {
        //                                    doWork2 = true;
        //                                }
        //                                else if (GetBOMITEMCUSTOM1(Convert.ToInt32(iSODetail.BOM_ID), context) != GetBOMITEMCUSTOM1Assign(Convert.ToInt32(bomNewIDs[0]), context))
        //                                {
        //                                    doWork2 = true;
        //                                }
        //                                else if (GetBOMNo(Convert.ToInt32(iSODetail.BOM_ID), context) != GetBOMNoAssign(Convert.ToInt32(bomNewIDs[0]), context))
        //                                {
        //                                    doWork2 = true;
        //                                }
        //                                else
        //                                {
        //                                    try
        //                                    {
        //                                        var MATERIAL_GROUP_ID = (from m in context.ART_WF_ARTWORK_PROCESS_PA where m.ARTWORK_SUB_ID == iSODetail.ARTWORK_SUB_ID select m.MATERIAL_GROUP_ID).FirstOrDefault();
        //                                        if (MATERIAL_GROUP_ID > 0)
        //                                        {
        //                                            var MATERIAL_GROUP_NAME = (from m in context.SAP_M_CHARACTERISTIC where m.CHARACTERISTIC_ID == MATERIAL_GROUP_ID select m.DESCRIPTION).FirstOrDefault();
        //                                            MATERIAL_GROUP_NAME = MATERIAL_GROUP_NAME.ToUpper();
        //                                            if (!newText.Contains(MATERIAL_GROUP_NAME))
        //                                            {
        //                                                doWork2 = true;

        //                                                var tempbomNewIDs = (from b in listBomNewIDs
        //                                                                     where b.SALES_ORDER_NO == iSODetail.SALES_ORDER_NO
        //                                                                     && b.ITEM == itemNO
        //                                                                     && b.BOM_IS_ACTIVE == "X"
        //                                                                     && (!string.IsNullOrEmpty(b.BOM_ITEM_CUSTOM_1) && b.BOM_ITEM_CUSTOM_1.Contains(MATERIAL_GROUP_NAME))
        //                                                                     select b.PO_COMPLETE_SO_ITEM_COMPONENT_ID).ToList();
        //                                                bomNewIDs[0] = tempbomNewIDs[0];
        //                                            }
        //                                        }
        //                                    }
        //                                    catch { }
        //                                }

        //                                if (doWork2)
        //                                {
        //                                    var oldValue = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
        //                                    var newValue = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();

        //                                    oldValue = iSODetail;

        //                                    iSODetail.BOM_ID = bomNewIDs[0];
        //                                    ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.SaveOrUpdateNoLog(iSODetail, context);

        //                                    newValue = iSODetail;

        //                                    saveLogRepairMaterialBOM("Add", newValue, oldValue, context, dt + " " + iSODetail.SALES_ORDER_NO + "(" + iSODetail.SALES_ORDER_ITEM + ")");
        //                                    doWork = true;
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            saveLogRepairMaterialBOM("Found (" + bomNewIDs.Count + ")", iSODetail, null, context, dt + " " + iSODetail.SALES_ORDER_NO + "(" + iSODetail.SALES_ORDER_ITEM + ")");
        //                        }
        //                    }
        //                }
        //            }

        //            if (doWork)
        //            {
        //                if (soDetails.Count > 0)
        //                {
        //                    var soDetail2 = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
        //                    soDetail2 = MapperServices.ART_WF_ARTWORK_PROCESS_SO_DETAIL(soDetails[0]);

        //                    SalesOrderHelper.DeleteAssignSalesOrder(soDetail2, context);
        //                    SalesOrderHelper.CopyAssignSalesOrder(soDetail2, context);

        //                    saveLogRepairMaterialBOM("Completed", null, null, context, dt);
        //                }
        //            }
        //            else
        //            {
        //                saveLogRepairMaterialBOM("Nothing", null, null, context, dt);
        //            }
        //        }

        //        if (subIDs.Count == 0)
        //        {
        //            saveLogRepairMaterialBOM("Not Found WF", null, null, context, dt);
        //        }
        //    }
        //}

        public static void RepairMaterialBOM2_1(string SONumber, string SOItemStr, string bomItem, string newText, string refMat, ARTWORKEntities context)
        {
            decimal SOItem = Convert.ToDecimal(SOItemStr);
            string dt = SONumber + " " + SOItem + " " + bomItem + " " + newText;
            if (!string.IsNullOrEmpty(newText))
            {
                saveLogRepairMaterialBOM("Start", null, null, context, dt);

                var subIDs = (from b in context.V_ART_ASSIGNED_SO
                              join m in context.ART_WF_ARTWORK_PROCESS on b.ARTWORK_SUB_ID equals m.ARTWORK_SUB_ID
                              where b.SALES_ORDER_NO == SONumber && b.ITEM == SOItem && b.BOM_ITEM_CUSTOM_1 == newText
                              //&& string.IsNullOrEmpty(m.IS_END)
                              select b.ARTWORK_SUB_ID).Distinct().ToList();

                if (!string.IsNullOrEmpty(refMat))
                {
                    if (newText.Contains("MULTI"))
                    {
                        subIDs = (from b in context.V_ART_ASSIGNED_SO
                                  join m in context.ART_WF_ARTWORK_PROCESS on b.ARTWORK_SUB_ID equals m.ARTWORK_SUB_ID
                                  where b.SALES_ORDER_NO == SONumber && b.ITEM == SOItem && b.BOM_ITEM_CUSTOM_1 == newText
                                       //&& string.IsNullOrEmpty(m.IS_END) 
                                       && b.COMPONENT_MATERIAL == refMat
                                  select b.ARTWORK_SUB_ID).Distinct().ToList();
                    }
                }

                var listSODetails = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                     where subIDs.Contains(s.ARTWORK_SUB_ID)
                                     select s).ToList();

                var listSONO = listSODetails.Select(s => s.SALES_ORDER_NO).Distinct().ToList();

                var listBomNewIDs = (from b in context.V_SAP_SALES_ORDER
                                     where listSONO.Contains(b.SALES_ORDER_NO)
                                     select new V_SAP_SALES_ORDER_2
                                     {
                                         COMPONENT_MATERIAL = b.COMPONENT_MATERIAL,
                                         SALES_ORDER_NO = b.SALES_ORDER_NO,
                                         ITEM = b.ITEM,
                                         PO_COMPLETE_SO_ITEM_COMPONENT_ID = b.PO_COMPLETE_SO_ITEM_COMPONENT_ID,
                                         BOM_IS_ACTIVE = b.BOM_IS_ACTIVE,
                                         BOM_ITEM_CUSTOM_1 = b.BOM_ITEM_CUSTOM_1
                                     }).Distinct().ToList();

                var doWork = false;
                foreach (var subID in subIDs)
                {
                    var soDetails = (from s in listSODetails where s.ARTWORK_SUB_ID == subID select s).ToList();

                    foreach (var iSODetail in soDetails)
                    {
                        if (String.IsNullOrEmpty(iSODetail.BOM_NO) && iSODetail.BOM_ID > 0)
                        {
                            decimal? itemNO = 0;
                            if (!String.IsNullOrEmpty(iSODetail.SALES_ORDER_ITEM))
                            {
                                itemNO = Convert.ToDecimal(iSODetail.SALES_ORDER_ITEM);
                            }

                            var temp = listBomNewIDs.Where(c => c.PO_COMPLETE_SO_ITEM_COMPONENT_ID == iSODetail.BOM_ID).FirstOrDefault();
                            if (temp != null)
                            {
                                var componentMatNo = temp.COMPONENT_MATERIAL;
                                var bomNewIDs = (from b in listBomNewIDs
                                                 where b.SALES_ORDER_NO == iSODetail.SALES_ORDER_NO
                                                 && b.ITEM == itemNO
                                                 && b.BOM_IS_ACTIVE == "X"
                                                 && b.BOM_ITEM_CUSTOM_1 == newText
                                                 select b.PO_COMPLETE_SO_ITEM_COMPONENT_ID).ToList();

                                var doWorkFixing = false;
                                if (newText.Contains("MULTI"))
                                {
                                    doWorkFixing = true;

                                    if (!string.IsNullOrEmpty(refMat))
                                    {
                                        bomNewIDs = (from b in listBomNewIDs
                                                     where b.SALES_ORDER_NO == iSODetail.SALES_ORDER_NO
                                                     && b.ITEM == itemNO
                                                     && b.BOM_IS_ACTIVE == "X"
                                                     && b.BOM_ITEM_CUSTOM_1 == newText
                                                     && b.COMPONENT_MATERIAL == refMat
                                                     select b.PO_COMPLETE_SO_ITEM_COMPONENT_ID).ToList();
                                    }
                                }
                                if (bomNewIDs.Count == 1)
                                {
                                    doWorkFixing = true;
                                }

                                if (doWorkFixing)
                                {
                                    if (bomNewIDs.Count > 0)
                                    {
                                        var doWork2 = false;
                                        if (!iSODetail.BOM_ID.Equals(bomNewIDs[0]))
                                        {
                                            doWork2 = true;
                                        }
                                        else if (GetBOMITEMCUSTOM1(Convert.ToInt32(iSODetail.BOM_ID), context) != GetBOMITEMCUSTOM1Assign(Convert.ToInt32(bomNewIDs[0]), context))
                                        {
                                            doWork2 = true;
                                        }
                                        else if (GetBOMNo(Convert.ToInt32(iSODetail.BOM_ID), context) != GetBOMNoAssign(Convert.ToInt32(bomNewIDs[0]), context))
                                        {
                                            doWork2 = true;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                var MATERIAL_GROUP_ID = (from m in context.ART_WF_ARTWORK_PROCESS_PA where m.ARTWORK_SUB_ID == iSODetail.ARTWORK_SUB_ID select m.MATERIAL_GROUP_ID).FirstOrDefault();
                                                if (MATERIAL_GROUP_ID > 0)
                                                {
                                                    var MATERIAL_GROUP_NAME = (from m in context.SAP_M_CHARACTERISTIC where m.CHARACTERISTIC_ID == MATERIAL_GROUP_ID select m.DESCRIPTION).FirstOrDefault();
                                                    MATERIAL_GROUP_NAME = MATERIAL_GROUP_NAME.ToUpper();
                                                    if (!newText.Contains(MATERIAL_GROUP_NAME))
                                                    {
                                                        doWork2 = true;

                                                        var tempbomNewIDs = (from b in listBomNewIDs
                                                                             where b.SALES_ORDER_NO == iSODetail.SALES_ORDER_NO
                                                                             && b.ITEM == itemNO
                                                                             && b.BOM_IS_ACTIVE == "X"
                                                                             && (!string.IsNullOrEmpty(b.BOM_ITEM_CUSTOM_1) && b.BOM_ITEM_CUSTOM_1.Contains(MATERIAL_GROUP_NAME))
                                                                             select b.PO_COMPLETE_SO_ITEM_COMPONENT_ID).ToList();
                                                        bomNewIDs[0] = tempbomNewIDs[0];
                                                    }
                                                }
                                            }
                                            catch { }
                                        }

                                        if (doWork2)
                                        {
                                            var oldValue = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
                                            var newValue = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();

                                            oldValue = iSODetail;

                                            iSODetail.BOM_ID = bomNewIDs[0];
                                            ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.SaveOrUpdateNoLog(iSODetail, context);

                                            newValue = iSODetail;

                                            saveLogRepairMaterialBOM("Add", newValue, oldValue, context, dt + " " + iSODetail.SALES_ORDER_NO + "(" + iSODetail.SALES_ORDER_ITEM + ")");
                                            doWork = true;
                                        }
                                    }
                                }
                                else
                                {
                                    saveLogRepairMaterialBOM("Found (" + bomNewIDs.Count + ")", iSODetail, null, context, dt + " " + iSODetail.SALES_ORDER_NO + "(" + iSODetail.SALES_ORDER_ITEM + ")");
                                }
                            }
                        }
                    }

                    if (doWork)
                    {
                        if (soDetails.Count > 0)
                        {
                            var soDetail2 = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
                            soDetail2 = MapperServices.ART_WF_ARTWORK_PROCESS_SO_DETAIL(soDetails[0]);

                            SalesOrderHelper.DeleteAssignSalesOrder(soDetail2, context);
                            SalesOrderHelper.CopyAssignSalesOrder(soDetail2, context);

                            saveLogRepairMaterialBOM("Completed", null, null, context, dt);
                        }
                    }
                    else
                    {
                        saveLogRepairMaterialBOM("Nothing", null, null, context, dt);
                    }
                }

                if (subIDs.Count == 0)
                {
                    saveLogRepairMaterialBOM("Not Found WF", null, null, context, dt);
                }
            }
        }

        public static void saveLogRepairMaterialBOM(string action, ART_WF_ARTWORK_PROCESS_SO_DETAIL newValue, ART_WF_ARTWORK_PROCESS_SO_DETAIL oldValue, ARTWORKEntities context, string str)
        {
            ART_SYS_LOG log = new ART_SYS_LOG();
            log.CREATE_BY = -2;
            log.UPDATE_BY = -2;
            log.TABLE_NAME = "ART_WF_ARTWORK_PROCESS_SO_DETAIL";
            log.ACTION = "Repaired Material BOM - " + action + " (" + str + ")";
            if (newValue != null) log.NEW_VALUE = CNService.SubString(CNService.Serialize(newValue), 4000);
            if (oldValue != null) log.OLD_VALUE = CNService.SubString(CNService.Serialize(oldValue), 4000);
            ART_SYS_LOG_SERVICE.SaveNoLog(log, context);
        }

        //public static string getReason(int REASON_ID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return getReason(REASON_ID, context);
        //    }
        //}

        public static string getReason(int REASON_ID, ARTWORKEntities context)
        {
            var temp = (from p in context.ART_M_DECISION_REASON where p.ART_M_DECISION_REASON_ID == REASON_ID select p.DESCRIPTION).FirstOrDefault();
            if (temp != null)
                return temp;
            else
                return "";
        }

        //public static string getReason(int? REASON_ID)
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        return getReason(REASON_ID, context);
        //    }
        //}

        public static string getReason(int? REASON_ID, ARTWORKEntities context)
        {
            var temp = (from p in context.ART_M_DECISION_REASON where p.ART_M_DECISION_REASON_ID == REASON_ID select p.DESCRIPTION).FirstOrDefault();
            if (temp != null)
                return temp;
            else
                return "";
        }

        public static string getRemarkReason(int WFSubId, string WFType, string stepName, ARTWORKEntities context)//
        {
            var temp = (from reason in context.ART_WF_REMARK_REASON_OTHER
                        join step in context.ART_M_STEP_MOCKUP on reason.WF_STEP equals step.STEP_MOCKUP_ID
                        where reason.WF_SUB_ID == WFSubId && reason.WF_TYPE.Equals(WFType) && step.STEP_MOCKUP_CODE.Equals(stepName)
                        select reason.REMARK_REASON).FirstOrDefault();
            if (temp != null)
                return temp;
            else
                return "";
        }

        //rewrited by aof #INC-11265
        public static string getRemarkReason(int WFSubId, string WFType, int StepID, ARTWORKEntities context)//
        {
            var temp = (from reason in context.ART_WF_REMARK_REASON_OTHER
                        join step in context.ART_M_STEP_MOCKUP on reason.WF_STEP equals step.STEP_MOCKUP_ID
                        where reason.WF_SUB_ID == WFSubId && reason.WF_TYPE.Equals(WFType) && reason.WF_STEP.Equals(StepID)
                        select reason.REMARK_REASON).FirstOrDefault();
            if (temp != null)
                return temp;
            else
                return "";
        }
        //rewrited by aof #INC-11265
       

        public static int GetVendorByArtworkSubId(int artworkSubId, ARTWORKEntities context)
        {
            var vendorId = 0;

            var mainArtworkSubId = CNService.FindParentArtworkSubId(artworkSubId, context);

            var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(mainArtworkSubId, context);
            //var processPG = (from g in context.ART_WF_ARTWORK_PROCESS_PG
            //                 where g.ARTWORK_SUB_ID == process.ARTWORK_SUB_ID
            //                 && g.ACTION_CODE == "SUBMIT"
            //                 orderby g.ARTWORK_SUB_PA_ID descending
            //                 select g).FirstOrDefault();
            var stepMockupPGID = context.ART_M_STEP_MOCKUP.Where(s => s.STEP_MOCKUP_CODE == "SEND_PG").Select(s => s.STEP_MOCKUP_ID).FirstOrDefault();
            var stepPGID = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == "SEND_PG").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
            var processPG = (from p in context.ART_WF_ARTWORK_PROCESS
                             where p.CURRENT_STEP_ID == stepPGID
                                 && p.IS_END == "X"
                                 && p.REMARK_KILLPROCESS == null
                                 && p.ARTWORK_ITEM_ID == process.ARTWORK_ITEM_ID
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
                                if (mockupPG.VENDOR > 0)
                                {
                                    vendorId = Convert.ToInt32(mockupPG.VENDOR);
                                }
                            }
                        }
                    }
                }
            }

            if (vendorId == 0)
            {
                var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                 where p.ARTWORK_SUB_ID == process.ARTWORK_SUB_ID
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
                                vendorId = vendorMaster.VENDOR_ID;
                            }
                        }
                    }
                }
            }

            return vendorId;
        }

        public static string getMatDesc(string mat5, ARTWORKEntities context)
        {
            var xecmProduct = (from h in context.XECM_M_PRODUCT5
                               where h.PRODUCT_CODE == mat5
                               select h.PRODUCT_DESCRIPTION).FirstOrDefault();

            if (!string.IsNullOrEmpty(xecmProduct))
                return xecmProduct;
            else
            {
                var igrid = (from h in context.IGRID_M_OUTBOUND_HEADER
                             where h.MATERIAL_NUMBER == mat5
                             select h.MATERIAL_DESCRIPTION).FirstOrDefault();

                if (!string.IsNullOrEmpty(igrid))
                {
                    return igrid;
                }
                else
                {
                    var bom = (from h in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                               where h.COMPONENT_MATERIAL == mat5
                               select h.DECRIPTION).FirstOrDefault();
                    if (!string.IsNullOrEmpty(bom))
                    {
                        return bom;
                    }
                }
            }

            return "";
        }

        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public static string getReasonRemark(ART_WF_REMARK_REASON_OTHER Item, ARTWORKEntities context)
        {
            var temp = ART_WF_REMARK_REASON_OTHER_SERVICE.GetByItem(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = Item.WF_SUB_ID, WF_STEP = Item.WF_STEP }, context).FirstOrDefault();
            if (temp != null)
                return temp.REMARK_REASON;
            else
                return "";
        }

        public static string removeLastComma(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (str.EndsWith(","))
                {
                    return str.Remove(str.Length - 1);
                }
            }

            return "";
        }
        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        public static List<V_ART_WF_DASHBOARD_ARTWORK_2> BuildIncomingArtwork(int USER_ID, int STEP, DateTime CreateDateFrom,
            DateTime CreateDateTo, ARTWORKEntities context)
        {
            return context.Database.SqlQuery<V_ART_WF_DASHBOARD_ARTWORK_2>("sp_ART_WF_DASHBOARD_ARTWORK @USER,@STEP,@From, @To", new SqlParameter("@USER", USER_ID),
                new SqlParameter("@STEP", STEP), new SqlParameter("@From", CreateDateFrom),
                new SqlParameter("@To", CreateDateTo)).ToList();
        }
        public static string GetProductionPlants(int ARTWORK_REQUEST_ID ,int ARTWORK_SUB_ID, ARTWORKEntities context)
        {
            //List<ListProductionPlant> lplant= context.Database.SqlQuery<ListProductionPlant>("spGetProductionPlant @ARTWORK_REQUEST_ID,@ARTWORK_SUB_ID", 
            //new SqlParameter("@ARTWORK_REQUEST_ID", ARTWORK_REQUEST_ID),
            //new SqlParameter("@ARTWORK_SUB_ID", ARTWORK_SUB_ID)).ToList();
            SqlParameter[] arge = { new SqlParameter("@ARTWORK_REQUEST_ID", ARTWORK_REQUEST_ID),
            new SqlParameter("@ARTWORK_SUB_ID", ARTWORK_SUB_ID)};
            DataTable dt = GetRelatedResources("spGetProductionPlant", arge);
            List<ListProductionPlant> lplant = (from DataRow dr in dt.Rows
                                                select new ListProductionPlant
                                                {
                                                    ID = string.Format("{0}", (dr["c"]))
                                                }).ToList();
            return lplant.Count() > 0 ? lplant[0].ID.ToString() : "";

        }
        //public static List<V_ART_WF_DASHBOARD_ARTWORK_2> BuildIndex(DateTime CreateDateFrom,
        //DateTime CreateDateTo, ARTWORKEntities context)
        //{
        //    return context.Database.SqlQuery<V_ART_WF_DASHBOARD_ARTWORK_2>("sp_ART_WF_DASHBOARD @From, @To", new SqlParameter("@From", CreateDateFrom),
        //        new SqlParameter("@To", CreateDateTo)).ToList();
        //}
        public static List<listSO> GetDataSO(int param)
        {
            DataTable dt = new DataTable();
            SqlParameter[] arge = { new SqlParameter("@user", param) };
            dt = GetRelatedResources("sp_SAP_SALES_ORDER", arge);
            List<listSO> list = new List<listSO>();
            list = (from DataRow dr in dt.Rows
                    select new listSO
                    {
                        Id = Convert.ToInt32(dr["PO_COMPLETE_SO_HEADER_ID"])
                    }).ToList();
            return list;

        }

        public static void ExportDataSetToExcel(DataTable dt)
        {
            //string AppLocation = "";
            //AppLocation = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            //AppLocation = AppLocation.Replace("file:\\", "");
            //string file = AppLocation + "\\ExcelFiles\\DataFile.xlsx";
            //string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/DataFile.xlsx");
            string file = @"C:\\temp\\ExcelFiles\\" + DateTime.Now.ToString("yyyyMMdd") + "\\_endtoend" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            dt.TableName = "EndToEnd_report";
            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt);
                ws.Name = "EndToEnd_report";
                //wb.Worksheets.Add(dt);
                //wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //wb.Style.Font.Bold = true;
                wb.SaveAs(file); 
                wb.Dispose();
            }
        }
        public static void DeleteAssignOrder(int ARTWORK_SUB_ID, ARTWORKEntities context)
        {
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@ARTWORK_SUB_ID",string.Format("{0}", ARTWORK_SUB_ID))};
            GetExecuteNonQuery("spDeleteAssignOrder", param);
        }
        public static void DeleteSO(int PO_COMPLETE_SO_HEADER_ID, ARTWORKEntities context)
        {
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@ID",string.Format("{0}", PO_COMPLETE_SO_HEADER_ID))};
            GetExecuteNonQuery("spDeleteSOCOMPLETE", param);
        }
        public static int GetAssignsoItem(SAP_M_PO_COMPLETE_SO_ITEM soItem, ARTWORKEntities context)
        {
            int p = context.Database.SqlQuery<int>("spUpdatesoItem @PO_COMPLETE_SO_HEADER_ID,@ITEM",
                                             new SqlParameter("@PO_COMPLETE_SO_HEADER_ID", soItem.PO_COMPLETE_SO_HEADER_ID),
                                             new SqlParameter("@ITEM", soItem.ITEM)).FirstOrDefault();
            return p;
        }
        public static int GetAssignOrder(int PO_COMPLETE_SO_HEADER_ID, ARTWORKEntities context)
        {
           int p= context.Database.SqlQuery<int>("spGetAssignOrder @PO_COMPLETE_SO_HEADER_ID",
                                            new SqlParameter("@PO_COMPLETE_SO_HEADER_ID", PO_COMPLETE_SO_HEADER_ID)).FirstOrDefault();
            return p;
        }
        public static List<SAP_M_CHARACTERISTIC> GetQueryByName(string name, string param)
        {
            DataTable dt = new DataTable();
            SqlParameter[] arge = { new SqlParameter("@name", name),
            new SqlParameter("@param", param)};
            dt = GetRelatedResources("sp_QueryByName", arge);
            List<SAP_M_CHARACTERISTIC> list = new List<SAP_M_CHARACTERISTIC>();
            list = (from DataRow dr in dt.Rows
                    select new SAP_M_CHARACTERISTIC
                    {
                        CHARACTERISTIC_ID = Convert.ToInt32(dr["CHARACTERISTIC_ID"]),
                        DESCRIPTION = dr["DESCRIPTION"].ToString()
                    }).ToList();
            return list;
        }
        public static string GetCheckRDD(string param)
        {
            DataTable dt = new DataTable();
            SqlParameter[] arge = { new SqlParameter("@mat5", param) };
            dt = GetRelatedResources("sp_ART_RECHECK_ARTWORK", arge);
            return dt.Rows[0]["RECHECK"].ToString(); 

        }
        
        public static string GetCheckFFC(string param)
        {
            DataTable dt = new DataTable();
            SqlParameter[] arge = { new SqlParameter("@requestno", param) };
            dt = GetRelatedResources("sp_ARTWORK_REQUEST_FFC", arge);
            return dt.Rows[0]["ARTWORK_REQUEST_TYPE"].ToString();

        }
 
        public static string Getcheck_product_vap(string mat,string param)
        {
            DataTable dt = new DataTable();
            SqlParameter[] arge = { new SqlParameter("@plant", param),
            new SqlParameter("@product_code", mat)};
            dt = GetRelatedResources("sp_ARTWORK_check_product_vap", arge);
            if (dt.Rows.Count > 0)
                return dt.Rows[0]["PRODUCT_TYPE"].ToString();
            else
                return "";
        }
        public class listSO
        {
            public int Id { get; set; }
        }
        public class ListProductionPlant
        {
            public string ID { get; set; }
            public string value { get; set; }
        }




        public static void executeScript(string scriptExcute)
        {
            // created by aof
            using (SqlConnection con = new SqlConnection(strConn))
            {
                string query = scriptExcute; // "Update SapMaterial set FinalInfoGroup = @Result Where Id=@Id";
                using (SqlCommand cmd = new SqlCommand(query))
                {
                    cmd.Connection = con; 
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        
        }

        public static DataTable executeProcedure(string StoredProcedure, object[] Parameters)
        {
            var Results = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(strConn))
                {
                    using (SqlCommand cmd = new SqlCommand(StoredProcedure, conn))
                    {
                        conn.Open();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(Parameters);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(Results);
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Results;
        }
        public static DataTable GetRelatedResources(string StoredProcedure, object[] Parameters)
        {
            DataTable Results = new DataTable();
            using (var db = new ARTWORKEntities())
            {
                using (SqlConnection conn = (SqlConnection)db.Database.Connection)
                {
                    using (SqlCommand cmd = new SqlCommand(StoredProcedure, conn))
                    {
                        conn.Open();
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddRange(Parameters);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(Results);
                        conn.Close();
                        conn.Dispose();
                    }
                }
            }
            return Results;
        }
        public static void GetExecuteNonQuery(string StoredProcedure, object[] Parameters)
        {
            try
            {
                using (var db = new ARTWORKEntities())
                {
                    using (SqlConnection conn = (SqlConnection)db.Database.Connection)
                    {
                        using (SqlCommand cmd = new SqlCommand(StoredProcedure, conn))
                        {
                            conn.Open();
                            cmd.Connection = conn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddRange(Parameters);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            conn.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    // write by aof
    public static string getSQLWhereByJoinStringWithAnd(string curWhere, string newWhere)
        {
            string retWhere = curWhere;

            if (!string.IsNullOrEmpty(newWhere))
            {
                if (!string.IsNullOrEmpty(retWhere))
                {
                    retWhere += " and (" + newWhere + ")";
                }
                else
                {
                    retWhere = newWhere;
                }
            }

            return retWhere;
        }


    public static string getSQLWhereLikeByConvertString(string strPattern, string fldname,bool is_replace_space = true,bool is_start_like = false,bool is_split_comma = true)
        {
            string where = "";

            if (!string.IsNullOrEmpty(strPattern))
            {

                string[] arrStr;

                if (is_split_comma)
                {
                    if (is_replace_space)
                    {
                        arrStr = strPattern.Replace(" ", "").Split(',');
                    }
                    else
                    {
                        arrStr = strPattern.Split(',');
                    }
                }
                else {
                    arrStr = new string[1];
                    arrStr[0] = strPattern;
                }
              
                  

               // var arrStr = strPattern.Split(',');

                if (arrStr != null)
                {
                    if (arrStr.Length > 0)
                    {
                        foreach (string s in arrStr)
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                // where += ",'" + s + "'";
                                if (string.IsNullOrEmpty(where))
                                {
                                    if (is_start_like)
                                    {
                                        where = "(" + fldname + " like N'%" + s + "%')";
                                    }
                                    else
                                    {
                                        where = "(" + fldname + " like N'" + s + "%')";
                                    } 
                                  
                                }
                                else
                                {
                                    if (is_start_like)
                                    {
                                        where += " or (" + fldname + " like N'%" + s + "%')";
                                    }
                                    else
                                    {
                                        where += " or (" + fldname + " like N'" + s + "%')";
                                    }
                                       
                                }
                            }
                        }
                    }
                }

            }

            if (!string.IsNullOrEmpty(where))
            {
                where = "(" + where + ")";
            }

            return where;
        }


        
        public static bool checkRequestFormIsVAP(int artwork_request_id, ARTWORKEntities context)
        {
            //by aof 20230121_3V_SOREPAT INC-93118
            var isVAP = false;


            var listPlantID = (from m in context.ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT where m.ARTWORK_REQUEST_ID == artwork_request_id select m.PRODUCTION_PLANT_ID).Distinct().ToList();
            var listProudctType= (from m in context.ART_WF_ARTWORK_REQUEST_PRODUCT where m.ARTWORK_REQUEST_ID == artwork_request_id select m.PRODUCT_TYPE).Distinct().ToList();

            if (listProudctType != null && listProudctType.Count > 0 )
            {
                isVAP = true;
                foreach (var producttype in listProudctType)
                {
                    if (producttype != "VAP")
                    {
                        isVAP = false;
                    }
                }

            }

            if (isVAP)
            {
                if (listPlantID != null && listPlantID.Count > 0 )
                {
                    foreach (var plantid in listPlantID)
                    {
                        if (plantid != 3)
                        {
                            isVAP = false;
                        }
                    }
                }
                
            }

            return isVAP;
        }


        public static string getIGridUserFN()
        {
            var fn = "";
            try
            {
                if (HttpContext.Current == null) { fn = ""; }
                else
                {
                    var UserName = curruser();// HttpContext.Current.User.Identity.Name;

                    using (SqlConnection con = new SqlConnection(strConn))
                    {

                        con.Open();
                        string strQuery = @"select top 1 fn from ulogin where user_name='"+ UserName +"'";
                        DataTable dt = new DataTable();
                        SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                        // Fill the dataset.
                        oAdapter.Fill(dt);
                        con.Close();

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            fn = dt.Rows[0]["fn"].ToString();
                        }

                    }

                }
            }
            catch (Exception ex) { CNService.GetErrorMessage(ex); }

           
            return fn;
        }

    }
}

public class Serializer
{
    public T Deserialize<T>(string input) where T : class
    {
        System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

        using (StringReader sr = new StringReader(input))
        {
            return (T)ser.Deserialize(sr);
        }
    }

    public string Serialize<T>(T ObjectToSerialize)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(ObjectToSerialize.GetType());

        using (StringWriter textWriter = new StringWriter())
        {
            xmlSerializer.Serialize(textWriter, ObjectToSerialize);
            return textWriter.ToString();
        }
    }
}
public static class MyToDataTable
{
    public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
    {
        try
        {
            List<T> list = new List<T>();

            foreach (var row in table.AsEnumerable())
            {
                T obj = new T();

                foreach (var prop in obj.GetType().GetProperties())
                {
                    try
                    {
                        PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                        propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                    }
                    catch
                    {
                        continue;
                    }
                }

                list.Add(obj);
            }

            return list;
        }
        catch
        {
            return null;
        }
    }
}

public class Checkpath
{
    public bool Ifexists { get; set; }

    public Filetype type { get; set; }
}

public enum Filetype
{
    File = 0,
    Dir = 1
}

static class StructuralExtensions
{
    public static bool StructuralEquals<T>(this T a, T b)
        where T : IStructuralEquatable
    {
        return a.Equals(b, StructuralComparisons.StructuralEqualityComparer);
    }

    public static int StructuralCompare<T>(this T a, T b)
        where T : IStructuralComparable
    {
        return a.CompareTo(b, StructuralComparisons.StructuralComparer);
    }
}