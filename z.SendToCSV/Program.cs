using BLL.Services;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections;

namespace z.SendToCSV
{
    public class Program
    {
        public static string strConn = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        public static string InterfacePathInbound = ConfigurationManager.AppSettings["InterfacePathInbound"];
        public static string InterfacePathOutbound = ConfigurationManager.AppSettings["InterfacePathOutbound"];
        static void Main(string[] args)
        {
            
            #region Outbound

            try
            {
                using (SqlConnection con = new SqlConnection(strConn))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spGetMasterData";
                    cmd.Parameters.AddWithValue("@Active", "X");
                    cmd.Connection = con;
                    con.Open();
                    DataSet oDataset = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(oDataset);
                    con.Close();

                    SQ01_ListMAT(oDataset.Tables[0]); //done
                    CT04(oDataset.Tables[0]); //Insert,Remove //done                   
                }

                using (SqlConnection con = new SqlConnection(strConn))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spQuery";
                    cmd.Parameters.AddWithValue("@Material", "X");
                    cmd.Connection = con;
                    con.Open();
                    DataSet oDataset = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(oDataset);
                    con.Close();

                    // Implement : wait for k.Tony get structure and summary with SAP,CPI, how to condition get data for gen csv
                    //add extend sale view
                    //Use 3. BAPI_OBJCL_CREATE (Create Classification View),  BAPI_TRANSACTION_COMMIT(TU In Use),
                    MM01(oDataset.Tables[0]);

                    // Implement : Wait for K.Tony confirm structure and summary with SAP,CPI, how to condition get data gen csv 
                    //Andy:(Update just only 1 mat/row)                     
                    BAPI_UpdateMATCharacteristics(oDataset.Tables[0]); 
                }

                using (SqlConnection con = new SqlConnection(strConn))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spGetImpactmat";
                    cmd.Parameters.AddWithValue("@Active", "X");
                    cmd.Connection = con;
                    con.Open();
                    DataSet oDataset = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(oDataset);
                    con.Close();

                    CLMM_ChangeMatClass(oDataset.Tables[0]);//done
                    MM02_ImpactMatDesc(oDataset.Tables[0]);//done
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine($"Message :{e.Message} ");
            }

            #endregion

            #region Inbound
            try
            {
                //var dir = @"D:\SAPInterfaces\Inbound";
                //    var filePaths = Directory.GetFiles(dir, "CT04*_Result*.csv");
                //    CT04_Inbound();

                string imported = "";
                string bodyMsg = "";
                FileInfo fileI = null;
                string fileN = "";

                // IMPORT DOWNLOADED FILES
                var filesToImport = Directory.GetFiles(InterfacePathInbound, "*_Result.csv");
                if (filesToImport != null)
                {
                    foreach (string file in filesToImport)
                    {                        
                        fileI = new FileInfo(file);
                        fileN = fileI.Name;
                        switch (fileN.Substring(0,6))
                        {
                            case "SQ01_L":
                                imported = Import_SQ01(file);                               
                                break;
                            case "CT04_I":
                                //imported = Import_MM17(fileI.FullName, out bodyMsg);
                                break;
                            case "CT04_R":
                                //imported = Import_QM25(fileI.FullName, out bodyMsg);
                                break;
                            //case "MM01_C":
                            //    imported = Import_MM02(fileI.FullName, out bodyMsg);
                            //    break;
                            //case "BAPI_U":
                            //    imported = Import_PP01(fileI.FullName, out bodyMsg);
                            //    break;
                            //case "CLMM_C":
                            //    imported = Import_SD21(fileI.FullName, out bodyMsg);
                            //    break;
                            //case "MM02":
                            //    imported = Import_PP21(fileI.FullName, out bodyMsg);
                            //    break;
                                                    
                        }

                        //if (imported == "")
                        //{
                        //    //GeneralTools.MoveFile(fileI.FullName, dr["InterfaceFolder"] + "\\IMPORTED\\" + fileN, false);

                        //    //if ((AppKeys.SAP_WS_NotifySuccessImport + "").ToLower() == "true")
                        //    //    ITF_Data.SendNotification(dr["OkMessage"] + "", bodyMsg, dr["InterfaceCode"] + "");

                        //    //StreamWriter sw = new StreamWriter(dr["InterfaceFolder"] + "\\IMPORTED\\" + fileN + ".ok");
                        //    //sw.Write("");
                        //    //sw.Close();
                        //    //sw.Dispose();

                        //    //if (dr["FtpSuccessFolderPath"] + "" != "")
                        //    //{
                        //    //    GeneralTools.UploadLocalFileToFTP(dr["InterfaceFolder"] + "\\IMPORTED\\" + fileN + ".ok", dr["FtpSuccessFolderPath"] + "", dr["FtpUsername"] + "", dr["FtpPassword"] + "");
                        //    //}

                        //}
                        //else
                        //{
                        //    //GeneralTools.MoveFile(fileI.FullName, dr["InterfaceFolder"] + "\\ERROR\\" + fileN, false);



                        //    ////if (imported == "ACTIVE_NOTIFY")
                        //    ////{
                        //    ////    ITF_Data.SendNotification("WISEUp - Error Importing " + fileN + " (New Retry)", "", "");
                        //    ////}
                        //    ////else
                        //    ////{
                        //    ////    if (imported != "ACTIVE")
                        //    ////    {
                        //    ////        // SAVE ERROR / SEND EMAIL
                        //    ////        ITF_Data.SendNotification("WISEUp - Error Importing " + fileN, imported, "");
                        //    ////        ITF_Data.InsertReceivedError(dr["InterfaceCode"] + "", fileN, "ACTIVE", imported, 1);
                        //    ////    }
                        //    ////}

                        //    //StreamWriter sw = new StreamWriter(dr["InterfaceFolder"] + "\\ERROR\\" + fileN + ".notok");
                        //    //sw.Write("");
                        //    //sw.Close();
                        //    //sw.Dispose();

                        //    //if (dr["FtpSuccessFolderPath"] + "" != "")
                        //    //{
                        //    //    GeneralTools.UploadLocalFileToFTP(dr["InterfaceFolder"] + "\\ERROR\\" + fileN + ".notok", dr["FtpSuccessFolderPath"] + "", dr["FtpUsername"] + "", dr["FtpPassword"] + "");
                        //    //}

                        //    //ErrorLogger.LOGGER.Error("Error Import " + dr["InterfaceCode"] + "", new Exception("Error: " + imported));
                        //}
                    }

                    //// Move MM01 & BAPI_UpdateMATCharacteristics
                    //filePaths = Directory.GetFiles(dir, "MM01*_Result*.csv");
                    //foreach (string s in filePaths)
                    //{
                    //    try
                    //    {
                    //        using (var reader = new StreamReader(s))
                    //        {
                    //            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    //            {
                    //                while (csv.Read())
                    //                {//This will advance the reader to the next record.

                    //                    //You can use an indexer to get by position or name. 
                    //                    //This will return the field as a string

                    //                    // By position
                    //                    var field = csv[0];
                    //                    //var AppID = csv["AppID"];
                    //                    // By header name
                    //                    //csv.Read();
                    //                    csv.ReadHeader();
                    //                    string[] headerRow = csv.Context.Reader.HeaderRecord;
                    //                    string[] filteredValues = Array.FindAll(headerRow, x => x.Contains("Result"));
                    //                    var Condition = "";// csv["Condition TypeRV13A-KSCHL"];
                    //                    if (filteredValues.Length > 0)
                    //                        if (csv["Result"] == "Condition records saved")
                    //                        {
                    //                        }
                    //                }
                    //            }
                    //        }
                    //        //File.Move(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s));
                    //        //HttpContext.Current.Server.MapPath("~/ExcelFiles/Processed/" + Path.GetFileName(s)));
                    //        // Copy the file and overwrite if it exists
                    //        File.Copy(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s), true);

                    //        // Delete the source file
                    //        File.Delete(s);
                    //    }
                    //    catch (IOException iox)
                    //    {
                    //        Console.WriteLine(iox.Message);
                    //    }
                    //}

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine($"Message :{e.Message} ");
                //ErrorLogger.LOGGER.Error(ex.Message, ex);
                //ITF_Data.SendNotification("SAP Import Service - Error Executing Interface", ex.Message + "<br />" + ex.StackTrace, "");
            }
            #endregion
        
        }

        #region IMPORT METHODS
        public static void ImportFileInterface()
        {
           
        }
        public static void CT04_Inbound(string data)
        {
            var dir = @"D:\SAPInterfaces\Inbound";
            var filePaths = Directory.GetFiles(dir, "CT04*_Result*.csv");
            foreach (string s in filePaths)
            {
                using (var reader = new StreamReader(s))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        while (csv.Read())
                        {//This will advance the reader to the next record.

                            //You can use an indexer to get by position or name. 
                            //This will return the field as a string

                            // By position
                            var field = csv[0];
                            //var AppID = csv["AppID"];
                            // By header name
                            //csv.Read();
                            csv.ReadHeader();
                            string[] headerRow = csv.Context.Reader.HeaderRecord;
                            string[] filteredValues = Array.FindAll(headerRow, x => x.Contains("Result"));
                            var Condition = "";// csv["Condition TypeRV13A-KSCHL"];
                            if (filteredValues.Length > 0)
                                if (csv["Result"] == "Condition records saved")
                                {
                                    string Changed_Id = "";
                                    string Changed_Action = "";
                                    string Material = "";
                                    string Description = "";
                                    string DMSNo = "";
                                    string New_Material = "";
                                    string New_Description = "";
                                    string Status = "";
                                    string Reason = "";
                                    string NewMat_JobId = "";
                                    string Char_Name = "";
                                    string Char_OldValue = "";
                                    string Char_NewValue = "";
                                    //SendEmailUpdateMaster("U" + Material);
                                    using (SqlConnection con = new SqlConnection(CNService.strConn))
                                    {
                                        SqlCommand cmd = new SqlCommand();
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.CommandText = "spUpdateImpactedmat";

                                        cmd.Parameters.AddWithValue("@Changed_Id", Changed_Id);
                                        cmd.Parameters.AddWithValue("@Changed_Action", Changed_Action);
                                        cmd.Parameters.AddWithValue("@Material", Material);
                                        cmd.Parameters.AddWithValue("@Description", Description);
                                        cmd.Parameters.AddWithValue("@DMSNo", DMSNo);
                                        cmd.Parameters.AddWithValue("@New_Material", New_Material);
                                        cmd.Parameters.AddWithValue("@New_Description", New_Description);
                                        cmd.Parameters.AddWithValue("@Status", Status);
                                        cmd.Parameters.AddWithValue("@Reason", Reason);
                                        cmd.Parameters.AddWithValue("@NewMat_JobId", NewMat_JobId);
                                        cmd.Parameters.AddWithValue("@Char_Name", Char_Name);
                                        cmd.Parameters.AddWithValue("@Char_OldValue", Char_OldValue);
                                        cmd.Parameters.AddWithValue("@Char_NewValue", Char_NewValue);


                                        cmd.Connection = con;
                                        con.Open();
                                        DataTable dtResult = new DataTable();
                                        SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                                        oAdapter.Fill(dtResult);
                                        con.Close();
                                    }
                                    //    using (SqlConnection con = new SqlConnection(strConn))
                                    //    {
                                    //        SqlCommand cmd = new SqlCommand();
                                    //        cmd.CommandType = CommandType.StoredProcedure;
                                    //        cmd.CommandText = "spUpdateQuotationfromJob";
                                    //        cmd.Parameters.AddWithValue("@MinPrice", string.Format("{0}", 0));
                                    //        cmd.Parameters.AddWithValue("@subID", string.Format("{0}", Condition.ToString() == "zpm1" ? "0" : AppID.ToString()));
                                    //        cmd.Parameters.AddWithValue("@ID", AppID.ToString());
                                    //        cmd.Connection = con;
                                    //        con.Open();
                                    //        cmd.ExecuteNonQuery();
                                    //        con.Close();
                                    //    }
                                    //}
                                }
                            //var records = csv.GetRecords<dynamic>();
                            //foreach (var item in records)
                            //{
                            //    var da = item.Result;
                            //    var condition = item[@"Condition Type RV13A-KSCHL"];  
                            //}
                            //foreach (dynamic record in records.ToList())
                            //{
                            //    var data = record["IfColumn"];
                            //}
                        }
                    }
                }
                try
                {
                    //File.Move(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s));
                    //HttpContext.Current.Server.MapPath("~/ExcelFiles/Processed/" + Path.GetFileName(s)));
                    // Copy the file and overwrite if it exists
                    File.Copy(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s), true);

                    // Delete the source file
                    File.Delete(s);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }
            }
            // Move MM01 & BAPI_UpdateMATCharacteristics
            filePaths = Directory.GetFiles(dir, "MM01*_Result*.csv");
            foreach (string s in filePaths)
            {
                try
                {
                    using (var reader = new StreamReader(s))
                    {
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            while (csv.Read())
                            {//This will advance the reader to the next record.

                                //You can use an indexer to get by position or name. 
                                //This will return the field as a string

                                // By position
                                var field = csv[0];
                                //var AppID = csv["AppID"];
                                // By header name
                                //csv.Read();
                                csv.ReadHeader();
                                string[] headerRow = csv.Context.Reader.HeaderRecord;
                                string[] filteredValues = Array.FindAll(headerRow, x => x.Contains("Result"));
                                var Condition = "";// csv["Condition TypeRV13A-KSCHL"];
                                if (filteredValues.Length > 0)
                                    if (csv["Result"] == "Condition records saved")
                                    {
                                    }
                            }
                        }
                    }
                    //File.Move(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s));
                    //HttpContext.Current.Server.MapPath("~/ExcelFiles/Processed/" + Path.GetFileName(s)));
                    // Copy the file and overwrite if it exists
                    File.Copy(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s), true);

                    // Delete the source file
                    File.Delete(s);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }
            }
            // Move BAPI_UpdateMATCharacteristics
            filePaths = Directory.GetFiles(dir, "BAPI*_Result*.csv");
            foreach (string s in filePaths)
            {
                try
                {
                    //File.Move(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s));
                    //HttpContext.Current.Server.MapPath("~/ExcelFiles/Processed/" + Path.GetFileName(s)));
                    // Copy the file and overwrite if it exists
                    File.Copy(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s), true);

                    // Delete the source file
                    File.Delete(s);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }
            }
        }
        public static void Inbound(string data)
        {
            var dir = @"D:\SAPInterfaces\Inbound";
            var filePaths = Directory.GetFiles(dir, "CT04*_Result*.csv");
            foreach (string s in filePaths)
            {
                using (var reader = new StreamReader(s))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        while (csv.Read())
                        {//This will advance the reader to the next record.

                            //You can use an indexer to get by position or name. 
                            //This will return the field as a string

                            // By position
                            var field = csv[0];
                            //var AppID = csv["AppID"];
                            // By header name
                            //csv.Read();
                            csv.ReadHeader();
                            string[] headerRow = csv.Context.Reader.HeaderRecord;
                            string[] filteredValues = Array.FindAll(headerRow, x => x.Contains("Result"));
                            var Condition = "";// csv["Condition TypeRV13A-KSCHL"];
                            if (filteredValues.Length > 0)
                                if (csv["Result"] == "Condition records saved")
                                {
                                    string Changed_Id = "";
                                    string Changed_Action = "";
                                    string Material = "";
                                    string Description = "";
                                    string DMSNo = "";
                                    string New_Material = "";
                                    string New_Description = "";
                                    string Status = "";
                                    string Reason = "";
                                    string NewMat_JobId = "";
                                    string Char_Name = "";
                                    string Char_OldValue = "";
                                    string Char_NewValue = "";
                                    SendEmailUpdateMaster("U" + Material);
                                    using (SqlConnection con = new SqlConnection(CNService.strConn))
                                    {
                                        SqlCommand cmd = new SqlCommand();
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.CommandText = "spUpdateImpactedmat";

                                        cmd.Parameters.AddWithValue("@Changed_Id", Changed_Id);
                                        cmd.Parameters.AddWithValue("@Changed_Action", Changed_Action);
                                        cmd.Parameters.AddWithValue("@Material", Material);
                                        cmd.Parameters.AddWithValue("@Description", Description);
                                        cmd.Parameters.AddWithValue("@DMSNo", DMSNo);
                                        cmd.Parameters.AddWithValue("@New_Material", New_Material);
                                        cmd.Parameters.AddWithValue("@New_Description", New_Description);
                                        cmd.Parameters.AddWithValue("@Status", Status);
                                        cmd.Parameters.AddWithValue("@Reason", Reason);
                                        cmd.Parameters.AddWithValue("@NewMat_JobId", NewMat_JobId);
                                        cmd.Parameters.AddWithValue("@Char_Name", Char_Name);
                                        cmd.Parameters.AddWithValue("@Char_OldValue", Char_OldValue);
                                        cmd.Parameters.AddWithValue("@Char_NewValue", Char_NewValue);


                                        cmd.Connection = con;
                                        con.Open();
                                        DataTable dtResult = new DataTable();
                                        SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                                        oAdapter.Fill(dtResult);
                                        con.Close();
                                    }
                                    //    using (SqlConnection con = new SqlConnection(strConn))
                                    //    {
                                    //        SqlCommand cmd = new SqlCommand();
                                    //        cmd.CommandType = CommandType.StoredProcedure;
                                    //        cmd.CommandText = "spUpdateQuotationfromJob";
                                    //        cmd.Parameters.AddWithValue("@MinPrice", string.Format("{0}", 0));
                                    //        cmd.Parameters.AddWithValue("@subID", string.Format("{0}", Condition.ToString() == "zpm1" ? "0" : AppID.ToString()));
                                    //        cmd.Parameters.AddWithValue("@ID", AppID.ToString());
                                    //        cmd.Connection = con;
                                    //        con.Open();
                                    //        cmd.ExecuteNonQuery();
                                    //        con.Close();
                                    //    }
                                    //}
                                }
                            //var records = csv.GetRecords<dynamic>();
                            //foreach (var item in records)
                            //{
                            //    var da = item.Result;
                            //    var condition = item[@"Condition Type RV13A-KSCHL"];  
                            //}
                            //foreach (dynamic record in records.ToList())
                            //{
                            //    var data = record["IfColumn"];
                            //}
                        }
                    }
                }
                try
                {
                    //File.Move(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s));
                    //HttpContext.Current.Server.MapPath("~/ExcelFiles/Processed/" + Path.GetFileName(s)));
                    // Copy the file and overwrite if it exists
                    File.Copy(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s), true);

                    // Delete the source file
                    File.Delete(s);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }
            }
            // Move MM01 & BAPI_UpdateMATCharacteristics
            filePaths = Directory.GetFiles(dir, "MM01*_Result*.csv");
            foreach (string s in filePaths)
            {
                try
                {
                    using (var reader = new StreamReader(s))
                    {
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            while (csv.Read())
                            {//This will advance the reader to the next record.

                                //You can use an indexer to get by position or name. 
                                //This will return the field as a string

                                // By position
                                var field = csv[0];
                                //var AppID = csv["AppID"];
                                // By header name
                                //csv.Read();
                                csv.ReadHeader();
                                string[] headerRow = csv.Context.Reader.HeaderRecord;
                                string[] filteredValues = Array.FindAll(headerRow, x => x.Contains("Result"));
                                var Condition = "";// csv["Condition TypeRV13A-KSCHL"];
                                if (filteredValues.Length > 0)
                                    if (csv["Result"] == "Condition records saved")
                                    {
                                    }
                            }
                        }
                    }
                    //File.Move(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s));
                    //HttpContext.Current.Server.MapPath("~/ExcelFiles/Processed/" + Path.GetFileName(s)));
                    // Copy the file and overwrite if it exists
                    File.Copy(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s), true);

                    // Delete the source file
                    File.Delete(s);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }
            }
            // Move BAPI_UpdateMATCharacteristics
            filePaths = Directory.GetFiles(dir, "BAPI*_Result*.csv");
            foreach (string s in filePaths)
            {
                try
                {
                    //File.Move(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s));
                    //HttpContext.Current.Server.MapPath("~/ExcelFiles/Processed/" + Path.GetFileName(s)));
                    // Copy the file and overwrite if it exists
                    File.Copy(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s), true);

                    // Delete the source file
                    File.Delete(s);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }
            }
        }
        #endregion

        #region IMPORT INTERFACES
        public static string Import_SQ01(string file)
        {
            using (var reader = new StreamReader(file))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    while (csv.Read())
                    {
                        //This will advance the reader to the next record.
                        //You can use an indexer to get by position or name. 
                        //This will return the field as a string
                        // By position
                        var field = csv[0];
                        //var AppID = csv["AppID"];
                        // By header name
                        //csv.Read();
                        csv.ReadHeader();
                        string[] headerRow = csv.Context.Reader.HeaderRecord;
                        string[] filteredValues = Array.FindAll(headerRow, x => x.Contains("Result"));
                        var Condition = "";// csv["Condition TypeRV13A-KSCHL"];
                        if (filteredValues.Length > 0)
                        {
                            if (csv["Result"] == "Condition records saved")
                            {
                                string Changed_Id = "";
                                string Changed_Action = "";
                                string Material = "";
                                string Description = "";
                                string DMSNo = "";
                                string New_Material = "";
                                string New_Description = "";
                                string Status = "";
                                string Reason = "";
                                string NewMat_JobId = "";
                                string Char_Name = "";
                                string Char_OldValue = "";
                                string Char_NewValue = "";

                                //SendEmailUpdateMaster("U" + Material);

                                //using (SqlConnection con = new SqlConnection(strConn))
                                //{
                                //    SqlCommand cmd = new SqlCommand();
                                //    cmd.CommandType = CommandType.StoredProcedure;
                                //    cmd.CommandText = "spUpdateImpactedmat";

                                //    cmd.Parameters.AddWithValue("@Changed_Id", Changed_Id);
                                //    cmd.Parameters.AddWithValue("@Changed_Action", Changed_Action);
                                //    cmd.Parameters.AddWithValue("@Material", Material);
                                //    cmd.Parameters.AddWithValue("@Description", Description);
                                //    cmd.Parameters.AddWithValue("@DMSNo", DMSNo);
                                //    cmd.Parameters.AddWithValue("@New_Material", New_Material);
                                //    cmd.Parameters.AddWithValue("@New_Description", New_Description);
                                //    cmd.Parameters.AddWithValue("@Status", Status);
                                //    cmd.Parameters.AddWithValue("@Reason", Reason);
                                //    cmd.Parameters.AddWithValue("@NewMat_JobId", NewMat_JobId);
                                //    cmd.Parameters.AddWithValue("@Char_Name", Char_Name);
                                //    cmd.Parameters.AddWithValue("@Char_OldValue", Char_OldValue);
                                //    cmd.Parameters.AddWithValue("@Char_NewValue", Char_NewValue);


                                //    cmd.Connection = con;
                                //    con.Open();
                                //    DataTable dtResult = new DataTable();
                                //    SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                                //    oAdapter.Fill(dtResult);
                                //    con.Close();
                                //}

                                //using (SqlConnection con = new SqlConnection(strConn))
                                //{
                                //    SqlCommand cmd = new SqlCommand();
                                //    cmd.CommandType = CommandType.StoredProcedure;
                                //    cmd.CommandText = "spUpdateQuotationfromJob";
                                //    cmd.Parameters.AddWithValue("@MinPrice", string.Format("{0}", 0));
                                //    cmd.Parameters.AddWithValue("@subID", string.Format("{0}", Condition.ToString() == "zpm1" ? "0" : AppID.ToString()));
                                //    cmd.Parameters.AddWithValue("@ID", AppID.ToString());
                                //    cmd.Connection = con;
                                //    con.Open();
                                //    cmd.ExecuteNonQuery();
                                //    con.Close();
                                //}
                            }
                        }
                     }
                
                        //var records = csv.GetRecords<dynamic>();
                        //foreach (var item in records)
                        //{
                        //    var da = item.Result;
                        //    var condition = item[@"Condition Type RV13A-KSCHL"];  
                        //}
                        //foreach (dynamic record in records.ToList())
                        //{
                        //    var data = record["IfColumn"];
                        //}
                    

                    //try
                    //{
                    //    //File.Move(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s));
                    //    //HttpContext.Current.Server.MapPath("~/ExcelFiles/Processed/" + Path.GetFileName(s)));
                    //    // Copy the file and overwrite if it exists
                    //    File.Copy(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s), true);

                    //    // Delete the source file
                    //    File.Delete(s);
                    //}
                    //catch (IOException iox)
                    //{
                    //    Console.WriteLine(iox.Message);
                    //}
                }
            }

            return "test";

            //try
            //{
            //    XmlDocument xDoc = new XmlDocument();
            //    xDoc.Load(file);
            //    XmlNamespaceManager ns = new XmlNamespaceManager(xDoc.NameTable);
            //    ns.AddNamespace("ns1", "http://Microsoft.LobServices.Sap/2007/03/Idoc/3/ZPP_LOIPRO01//751/Receive");
            //    ns.AddNamespace("ns2", "http://Microsoft.LobServices.Sap/2007/03/Types/Idoc/3/ZPP_LOIPRO01//751");

            //    body = "";

            //    if (!xDoc.InnerXml.Contains(ns.LookupNamespace("ns1")) || !xDoc.InnerXml.Contains(ns.LookupNamespace("ns2")))
            //    {
            //        ErrorLogger.LOGGER.Error("Error: Namespace not found in file content!", new Exception("Error: Namespace not found in file content!"));
            //        return "Error: Namespace not found in file content!";
            //    }

            //    ITF_ProcessOrder po = new ITF_ProcessOrder();

            //    XmlNodeList nodeList = xDoc.SelectNodes("/ns1:Receive/ns1:idocData/ns2:E2AFKOL004GRP/ns2:E2AFKOL004", ns);

            //    foreach (XmlNode node in nodeList)
            //    {
            //        foreach (XmlNode subNode in node.ChildNodes)
            //        {
            //            switch (subNode.Name)
            //            {
            //                case "AUFNR": po.OrderNumber = subNode.InnerText; break;
            //                case "AUART": po.OrderType = subNode.InnerText; break;
            //                case "PRUEFLOS": po.InspectionLot = (long)GeneralTools.toLong(subNode.InnerText, 0) > 0 ? subNode.InnerText : ""; break;
            //            }
            //        }
            //    }

            //    nodeList = xDoc.SelectNodes("/ns1:Receive/ns1:idocData/ns2:Z2PPIO_E1AFKOL1000", ns);

            //    foreach (XmlNode node in nodeList)
            //    {
            //        foreach (XmlNode subNode in node.ChildNodes)
            //        {
            //            switch (subNode.Name)
            //            {
            //                case "Z_FGORDIND": po.IsFinishedGood = subNode.InnerText; break;
            //                case "Z_DLFL": po.ToDelete = subNode.InnerText; break;

            //                case "INSMK":
            //                    if (subNode.InnerText == "X")
            //                        po.PostInspection = true;
            //                    else
            //                        po.PostInspection = false;

            //                    break;
            //            }
            //        }
            //    }

            //    nodeList = xDoc.SelectNodes("/ns1:Receive/ns1:idocData/ns2:E2AFKOL004GRP/ns2:E2AFFLLGRP/ns2:E2AFFLL", ns);

            //    foreach (XmlNode node in nodeList)
            //    {
            //        foreach (XmlNode subNode in node.ChildNodes)
            //        {
            //            switch (subNode.Name)
            //            {
            //                case "APLZL": po.InternalCounter = GeneralTools.toInt(subNode.InnerText, 0); break;
            //            }
            //        }
            //    }

            //    nodeList = xDoc.SelectNodes("/ns1:Receive/ns1:idocData/ns2:E2AFKOL004GRP/ns2:E2AFPOL006", ns);

            //    foreach (XmlNode node in nodeList)
            //    {
            //        foreach (XmlNode subNode in node.ChildNodes)
            //        {
            //            switch (subNode.Name)
            //            {
            //                case "LGORT": po.StorageLocation = subNode.InnerText; break;
            //                case "CHARG": po.BatchNumber = subNode.InnerText; break;
            //            }
            //        }
            //    }

            //    DataTable dtActivities = new DataTable("Activities");
            //    dtActivities.Columns.Add("ActivityNumber");
            //    dtActivities.Columns.Add("ActivityDescription");


            //    nodeList = xDoc.SelectNodes("/ns1:Receive/ns1:idocData/ns2:E2AFKOL004GRP/ns2:E2AFFLLGRP/ns2:E2AFVOL004GRP", ns);

            //    foreach (XmlNode node in nodeList)
            //    {

            //        DataRow row = dtActivities.NewRow();

            //        foreach (XmlNode subnode in node.ChildNodes)
            //        {
            //            if (subnode.Name + "" == "E2AFVOL004")
            //            {
            //                foreach (XmlNode subsubNode in subnode.ChildNodes)
            //                {
            //                    switch (subsubNode.Name)
            //                    {
            //                        case "VORNR": row["ActivityNumber"] = subsubNode.InnerText + "" == "" ? DBNull.Value : (object)subsubNode.InnerText; break;
            //                        case "LTXA1": row["ActivityDescription"] = subsubNode.InnerText + "" == "" ? DBNull.Value : (object)subsubNode.InnerText; break;
            //                        case "STEUS": row["ControlKey"] = subsubNode.InnerText + "" == "" ? DBNull.Value : (object)subsubNode.InnerText; break;
            //                        case "ARBPL": row["WorkCenter"] = subsubNode.InnerText + "" == "" ? DBNull.Value : (object)subsubNode.InnerText; break;
            //                    }
            //                }
            //            }
            //            else if (subnode.Name + "" == "Z2PPIO_E1KBEDL000")
            //            {
            //                foreach (XmlNode subsubNode in subnode.ChildNodes)
            //                {
            //                    switch (subsubNode.Name)
            //                    {
            //                        case "Z_KABRSOLL": row["Quantity"] = GeneralTools.toDBDecimal(subsubNode.InnerText, 0); break;
            //                        case "Z_KEINH": row["Unit"] = subsubNode.InnerText + "" == "" ? DBNull.Value : (object)subsubNode.InnerText; break;
            //                    }
            //                }
            //            }
            //        }

            //        dtActivities.Rows.Add(row);
            //    }


            //    DataTable dtComponents = new DataTable("Components");
            //    dtComponents.Columns.Add("RequirementDate", typeof(int));
            //    dtComponents.Columns.Add("BatchNumber");



            //    nodeList = xDoc.SelectNodes("/ns1:Receive/ns1:idocData/ns2:E2AFKOL004GRP/ns2:E2AFFLLGRP/ns2:Z2PPIO_E1RESBL2000", ns);

            //    foreach (XmlNode node in nodeList)
            //    {
            //        DataRow row = dtComponents.NewRow();

            //        foreach (XmlNode subNode in node.ChildNodes)
            //        {
            //            switch (subNode.Name)
            //            {
            //                case "BDTER": row["RequirementDate"] = GeneralTools.toDBInt(subNode.InnerText, 0); break;
            //                case "CHARG": row["BatchNumber"] = subNode.InnerText + "" == "" ? DBNull.Value : (object)subNode.InnerText; break;

            //            }
            //        }

            //        dtComponents.Rows.Add(row);
            //    }

            //    po.Activities = dtActivities;
            //    po.Components = dtComponents;

            //    string ret = ITF_ReceivedFiles.Insert_PP01(file, po, 1);

            //    if (GeneralTools.toInt(ret, 0) <= 0)
            //    {
            //        body = "PP01 Error in procedure: " + ret + "";
            //        ITF_Data.InsertGeneralError("PP01", "", body, 1);
            //        ITF_Data.SendNotification("SAP Import Service - Error Executing Interface PP01 (" + ret + ")", body, "");
            //        return "PP01 Error in procedure: " + ret + "";

            //        //ITF_Data.SendNotification("WISEUp - Error Getting Orders (New Retry)", "", "");
            //    }
            //    else
            //    {
            //        body = "Process Order (" + po.IsFinishedGood + "" == "X" ? "FG" : "SFG" + "): No." + po.OrderNumber + ", From: " + po.OrderStartDate + " to " + po.OrderEndDate;
            //        return "";
            //    }

                //if (ret == "ACTIVE" || ret == "ACTIVE_NOTIFY")
                //{
                //    return ret;
                //}
                //else
                //{
                //    if (GeneralTools.toInt(ret, 0) <= 0)
                //        return "Error in procedure: " + ret + "";
                //}

            //}
            //catch (Exception ex)
            //{
            //    ErrorLogger.LOGGER.Error(ex.Message, ex);
            //    ITF_Data.InsertGeneralError("PP01", "", ex.StackTrace, 1);
            //    ITF_Data.SendNotification("SAP Import Service - Error Executing Interface PP01", ex.Message + "<br />" + ex.StackTrace, "");
            //    body = ex.StackTrace;
            //    return ex.Message;
            //}
        }
        #endregion

        #region INSERT INTERFACES
    //    public static string Insert_PP01(string p_FileName, ITF_ProcessOrder p_Po, int p_UserID)
    //    {
    //        //try
    //        //{
    //        //    SqlDataObject sql = new SqlDataObject(AppKeys.DatabaseConnection, "ITF_PP01_Insert", false);

    //        //    sql.AddInputParameter(false, "@FileName", p_FileName);
    //        //    sql.AddInputParameter(false, "@OrderNumber", p_Po.OrderNumber);
    //        //    sql.AddInputParameter(false, "@IsFinishedGood", p_Po.IsFinishedGood);
    //        //    sql.AddInputParameter(false, "@ToDelete", p_Po.ToDelete);
    //        //    sql.AddInputParameter(false, "@ProductionClosed", p_Po.ProductionClosed);
    //        //    sql.AddInputParameter(false, "@ConsumptionClosed", p_Po.ConsumptionClosed);
    //        //    sql.AddInputParameter(false, "@OrderTypeDescription", p_Po.OrderTypeDescription);
    //        //    sql.AddInputParameter(false, "@OrderType", p_Po.OrderType);
    //        //    sql.AddInputParameter(false, "@Quantity", p_Po.Quantity);
    //        //    sql.AddInputParameter(false, "@Unit", p_Po.Unit);
    //        //    sql.AddInputParameter(false, "@OrderStartDate", p_Po.OrderStartDate);

    //        //    if (p_Po.OrderEndDate > 0)
    //        //        sql.AddInputParameter(false, "@OrderEndDate", p_Po.OrderEndDate);

    //        //    sql.AddInputParameter(false, "@MaterialNumber", p_Po.MaterialNumber);

    //        //    if (p_Po.RecipeHeader + "" != "")
    //        //        sql.AddInputParameter(false, "@RecipeHeader", p_Po.RecipeHeader);
    //        //    if (p_Po.RecipeLine + "" != "")
    //        //        sql.AddInputParameter(false, "@RecipeLine", p_Po.RecipeLine);
    //        //    if (p_Po.InspectionLot + "" != "")
    //        //        sql.AddInputParameter(false, "@InspectionLot", p_Po.InspectionLot);
    //        //    if (p_Po.StorageLocation + "" != "")
    //        //        sql.AddInputParameter(false, "@StorageLocation", p_Po.StorageLocation);
    //        //    if (p_Po.BatchNumber + "" != "")
    //        //        sql.AddInputParameter(false, "@BatchNumber", p_Po.BatchNumber);
    //        //    if (GeneralTools.toInt(p_Po.InternalCounter, 0) > 0)
    //        //        sql.AddInputParameter(false, "@InternalCounter", p_Po.InternalCounter);

    //        //    sql.AddInputParameter(false, "@ProductionType", p_Po.ProductionType);
    //        //    sql.AddInputParameter(false, "@ProductionTypeDescription", p_Po.ProductionTypeDescription);
    //        //    sql.AddInputParameter(false, "@Components", p_Po.Components);
    //        //    sql.AddInputParameter(false, "@Activities", p_Po.Activities);
    //        //    sql.AddInputParameter(false, "@UserID", p_UserID);
    //        //    sql.AddInputParameter(false, "@SapPostInspection", p_Po.PostInspection);

    //        //    return sql.ExecuteScalar() + "";
    //        //}
    //        //catch (Exception ex)
    //        //{
    //        //    ErrorLogger.LOGGER.Error(ex.Message, ex);
    //        //    ITF_Data.SendNotification("SAP Import Service - Error Inserting Interface PP01", ex.Message + "<br />" + ex.StackTrace, "");
    //        //    return ex.Message;
    //        //}
        
    //}

        #endregion

        #region EXPORTTOCSV METHODS
        public static void SQ01_ListMAT(DataTable Results)
        {         
            DataTable dtListMat = new DataTable();
            dtListMat.Columns.AddRange(new DataColumn[] 
            { 
                new DataColumn (@"Characteristic Name txtSP$00005-LOW.text"),
                new DataColumn(@"txtSP$00003 - LOW.text"),
            });
            foreach (DataRow row in Results.Rows)
            {                    
                dtListMat.Rows.Add(
                    string.Format("{0}", row["Changed_Charname"].ToString()),
                    string.Format("{0}", row["Old_Description"].ToString()));
            }
            if (dtListMat.Rows.Count > 0)
            {
                string file = InterfacePathOutbound + "SQ01_ListMat" + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                ToCSV(dtListMat, file);
            }                 
        }
        public static void CT04(DataTable Results)
        {
            //Cancel CT04Update
            DataTable dtCT04Insert = new DataTable();
            //DataTable dtCT04Update = new DataTable();
            DataTable dtCT04Remove = new DataTable();
            dtCT04Insert.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
                new DataColumn(@"Characteristic Name RCTAV-ATNAM"),
                new DataColumn(@"Characteristic Value CAWN-ATWRT(01)"),
                new DataColumn(@"Characteristic value description CAWNT-ATWTB(01)"),
            });
            //dtCT04Update.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
            //    new DataColumn(@"Characteristic Name RCTAV-ATNAM"),
            //    new DataColumn(@"Characteristic Value CAWN-ATWRT(01)"),
            //    new DataColumn(@"Text for a table entry CLHP-CR_STATUS_TEXT"),
            //    new DataColumn(@"Characteristic value description CAWNT-ATWTB(01)"),
            //});
            dtCT04Remove.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
                new DataColumn(@"Characteristic Name RCTAV-ATNAM"),
                new DataColumn(@"Characteristic Value CAWN-ATWRT(01)"),
                new DataColumn(@"Text for a table entry CLHP-CR_STATUS_TEXT"),
            });
            foreach (DataRow row in Results.Rows)
            {
                switch (row["Changed_Action"].ToString())
                {
                    case "Insert":
                        dtCT04Insert.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString().Replace("Insert", "I")),
                            string.Format("{0}", row["Changed_Charname"].ToString()),
                            string.Format("{0}", row["id"].ToString()),
                            string.Format("{0}", row["Description"].ToString()));
                        break;
                    //case "Update":
                    //    dtCT04Update.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString()),
                    //       string.Format("{0}", row["Changed_Charname"].ToString()),
                    //       string.Format("{0}", row["id"].ToString()),
                    //       string.Format("{0}", row["Old_Description"].ToString()),
                    //       string.Format("{0}", row["Description"].ToString()));
                    //    break;
                    case "Remove":
                        dtCT04Remove.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString().Replace("Remove", "D")),
                           string.Format("{0}", row["Changed_Charname"].ToString()),
                           string.Format("{0}", row["Description"].ToString()));
                        break;
                    default:
                        break;
                }
            }
            if (dtCT04Insert.Rows.Count > 0)
            {
                string file = InterfacePathOutbound + "CT04_Insert_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                ToCSV(dtCT04Insert, file);
            }
            //if (dtCT04Update.Rows.Count > 0)
            //{
            //    string file = @"D:\SAPInterfaces\Outbound\CT04_Update_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
            //    ToCSV(dtCT04Update, file);
            //}
            if (dtCT04Remove.Rows.Count > 0)
            {
                string file = InterfacePathOutbound + "CT04_Remove_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                ToCSV(dtCT04Remove, file);
            }
        }
        public static void MM01(DataTable Results)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
                new DataColumn(@"Material Number RMMG1-MATNR"),
            new DataColumn(@"Material Description (Short Text) MAKT-MAKTX"),
            new DataColumn(@"Reference material RMMG1_REF-MATNR"),
            new DataColumn(@"IfColumn and Plant RMMG1-WERKS and Reference plant RMMG1_REF-WERKS"),
            new DataColumn(@"IfColumn and Sales Organization RMMG1-VKORG and Reference sales organization RMMG1_REF-VKORG"),
            new DataColumn(@"Distribution Channel RMMG1-VTWEG and Reference distribution channel RMMG1_REF-VTWEG"),
            });
            foreach (DataRow row in Results.Rows)
            {
                string[] split = string.Format("{0}", row["Plant"].ToString()).Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in split)
                {

                    if (s.Trim() != "")
                    {
                        string[] splitSOOrg = "DM;EX".Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string o in splitSOOrg)
                        {
                            dt.Rows.Add(string.Format("{0}", row["DocumentNo"].ToString()),
                string.Format("{0}", row["Material"].ToString()),
                string.Format("{0}", row["Description"].ToString()),
                string.Format("{0}", row["Ref"].ToString()),
                string.Format("{0}", s),
                string.Format("{0}", row["Plant"]).Substring(0, 3),
                //string.Format("{0}", row["Plant"].ToString())

                string.Format("{0}", o)
                );
                        }
                    }
                }
            }
            if (dt.Rows.Count > 0)
            {
                string file = InterfacePathOutbound + "MM01_CreateMAT_ExtensionPlant_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                CNService.ToCSV(dt, file);
            }
        }
        public static void BAPI_UpdateMATCharacteristics(DataTable Results)
        {
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
                    string file = InterfacePathOutbound + "BAPI_UpdateMATCharacteristics_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                    ToCSV(dtClass, file);
                }
            }
        }
        public static void CLMM_ChangeMatClass(DataTable Results)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] 
            { 
                new DataColumn (@"Char.Name ctxtG_CHAR_TAB - ATNAM"),
                new DataColumn(@"Old Value ctxtG_CHAR_TAB - OLDATWRT"),
                new DataColumn(@"New Value ctxtG_CHAR_TAB - NEWATWRT"),
                new DataColumn(@"Table cell -TextField txtG_TARGET_TAB - OBJECT"),
             });
            foreach (DataRow row in Results.Rows)
            {
                dt.Rows.Add(
                    string.Format("{0}", row["Char_Name"].ToString()),
                    string.Format("{0}", row["Char_OldValue"].ToString()),
                    string.Format("{0}", row["Char_NewValue"].ToString()),
                    string.Format("{0}", row["Material"].ToString()),
                    string.Format("{0}", row["Description"].ToString()));
            }
            if (dt.Rows.Count > 0)
            {
                string file = InterfacePathOutbound + "CLMM_ChangeMatClass" + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                ToCSV(dt, file);
            }
        }
        public static void MM02_ImpactMatDesc(DataTable Results)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] { new DataColumn (@"Material Number RMMG1 - MATNR"),
            new DataColumn(@"Material description MAKT - MAKTX"),});
            foreach (DataRow row in Results.Rows)
            {
                dt.Rows.Add(
                string.Format("{0}", row["Material"].ToString()),
                string.Format("{0}", row["Description"].ToString()));
            }
            if (dt.Rows.Count > 0)
            {
                string file = InterfacePathOutbound + "MM02_ImpactMatDesc" + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                ToCSV(dt, file);
            }
        }
        #endregion

        #region Util METHODS
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
        #endregion

        public static void GetmasterUpdateToCSV(DataTable Results)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
                new DataColumn(@"Characteristic Name RCTAV-ATNAM"),
                new DataColumn(@"Characteristic Value CAWN-ATWRT(01)"),
                new DataColumn(@"Text for a table entry CLHP-CR_STATUS_TEXT"),
                new DataColumn(@"Characteristic value description CAWNT-ATWTB(01)"),
            });
            DataTable listMat = new DataTable();
            listMat.Columns.AddRange(new DataColumn[] { new DataColumn (@"Characteristic Name txtSP$00005-LOW.text"),
            new DataColumn(@"txtSP$00003 - LOW.text"),
            });
            foreach (DataRow row in Results.Rows)
            {
                dt.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString()),
                string.Format("{0}", row["Changed_Charname"].ToString()),
                string.Format("{0}", row["id"].ToString()),
                string.Format("{0}", row["Old_Description"].ToString()),
                string.Format("{0}", row["Description"].ToString()));

                listMat.Rows.Add(string.Format("{0}", row["Changed_Charname"].ToString()),
                string.Format("{0}", row["Old_Description"].ToString()));
            }
            if (listMat.Rows.Count > 0)
            {
                string file = @"D:\SAPInterfaces\Outbound\SQ01_ListMat" + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                CNService.ToCSV(listMat, file);
            }

            DataTable dtCT04Insert = new DataTable();
            DataTable dtCT04Update = new DataTable();
            DataTable dtCT04Remove = new DataTable();
            dtCT04Insert.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
                new DataColumn(@"Characteristic Name RCTAV-ATNAM"),
                new DataColumn(@"Characteristic Value CAWN-ATWRT(01)"),
                new DataColumn(@"Characteristic value description CAWNT-ATWTB(01)"),
            });
            dtCT04Update.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
                new DataColumn(@"Characteristic Name RCTAV-ATNAM"),
                new DataColumn(@"Characteristic Value CAWN-ATWRT(01)"),
                new DataColumn(@"Text for a table entry CLHP-CR_STATUS_TEXT"),
                new DataColumn(@"Characteristic value description CAWNT-ATWTB(01)"),
            });
            dtCT04Remove.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
                new DataColumn(@"Characteristic Name RCTAV-ATNAM"),
                new DataColumn(@"Characteristic Value CAWN-ATWRT(01)"),
                new DataColumn(@"Text for a table entry CLHP-CR_STATUS_TEXT"),                
            });
            foreach (DataRow row in Results.Rows)
            {
                switch (row["Changed_Action"].ToString())
                {
                    case "Insert":
                        dtCT04Insert.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString().Replace("Insert","I")),
                            string.Format("{0}", row["Changed_Charname"].ToString()),
                            string.Format("{0}", row["id"].ToString()),                           
                            string.Format("{0}", row["Description"].ToString()));
                        break;
                    case "Update":
                        dtCT04Update.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString()),
                           string.Format("{0}", row["Changed_Charname"].ToString()),
                           string.Format("{0}", row["id"].ToString()),
                           string.Format("{0}", row["Old_Description"].ToString()),
                           string.Format("{0}", row["Description"].ToString()));
                        break;
                    case "Remove":
                        dtCT04Remove.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString().Replace("Remove", "D")),
                           string.Format("{0}", row["Changed_Charname"].ToString()),                           
                           string.Format("{0}", row["Description"].ToString()));
                        break;
                    default:
                        break;
                }
            }
            if (dtCT04Insert.Rows.Count > 0)
            {              
                string file = @"D:\SAPInterfaces\Outbound\CT04_Insert_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                CNService.ToCSV(dtCT04Insert, file);
            }
            if (dtCT04Update.Rows.Count > 0)
            {
                string file = @"D:\SAPInterfaces\Outbound\CT04_Update_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                CNService.ToCSV(dtCT04Update, file);
            }
            if (dtCT04Remove.Rows.Count > 0)
            {
                string file = @"D:\SAPInterfaces\Outbound\CT04_Remove_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                CNService.ToCSV(dtCT04Remove, file);
            }


            //string[] ColumnsToBeDeleted = { "Insert", "Update", "Remove" };
            //if (dt.Rows.Count > 0)
            //    foreach (string ColName in ColumnsToBeDeleted)
            //    {
            //        foreach(DataRow row in dt.Rows)
            //        {
            //            row[0].ToString();
            //        }
            //         //DataRow["IfColumn"][1].ToString().Replace(ColName, "I");
            //        //dt.Rows.Add(dt.Rows[][0].ToString().Replace("Insert", "I")
            //            //);

            //        //dt.Rows.Add(row["DocumentNo"].ToString(),
            //        //    row["DocumentNo"].ToString()
            //        //    );

            //        //var dtclone = new DataTable();
            //        //if (dt.Select("IfColumn='" + ColName + "'").ToList().Count > 0)
            //        //{
            //        //    //if (ColName == "Update")
            //        //    //{
            //        //    //    dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
            //        //    //}
            //        //    //else if (ColName == "Insert")
            //        //    //{
            //        //    //    dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
            //        //    //    //dtclone = dt.Select("IfColumn='Insert'").CopyToDataTable();
            //        //    //    //dtclone.Columns[0]..ItemArray[0].ToString().Replace("a","I");
            //        //    //    //dtclone.Rows[0].ItemArray[0].ToString().Replace('Insert', 'I');
            //        //    //    //dtclone = dt.Rows[0][0].ToString().CopyToDataTable();
            //        //    //    dtclone.Rows[0][0].ToString().Replace(ColName, "I");
            //        //    //    dtclone.Columns.Remove(@"Text for a table entry CLHP-CR_STATUS_TEXT");
            //        //    //}
            //        //    //else if (ColName == "Remove")
            //        //    //{
            //        //    //    dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
            //        //    //    dtclone.Columns.Remove(@"Characteristic Value CAWN-ATWRT(01)");
            //        //    //    dtclone.Columns.Remove(@"Characteristic value description CAWNT-ATWTB(01)");
            //        //    //}
            //        //    //string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/CT04_" + ColName + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv");
            //        //    string file = @"D:\SAPInterfaces\Outbound\CT04_" + ColName + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";

            //        //    CNService.ToCSV(dtclone, file);
            //        //}
            //    }


        }
        public static void GetImpactmat(string sName)
        {
            //Char.Name ctxtG_CHAR_TAB - ATNAM[0, 0].text
            //Old Value ctxtG_CHAR_TAB - OLDATWRT[1, 0].text
            //New Value ctxtG_CHAR_TAB - NEWATWRT[2, 0].text
            //Table cell -TextField txtG_TARGET_TAB - OBJECT[0, 0].text
            using (SqlConnection con = new SqlConnection(CNService.strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spGetImpactmat";
                cmd.Parameters.AddWithValue("@Active", sName);
                cmd.Connection = con;
                con.Open();
                DataSet oDataset = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(oDataset);
                con.Close();
                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[] { new DataColumn (@"Char.Name ctxtG_CHAR_TAB - ATNAM"),
                new DataColumn(@"Old Value ctxtG_CHAR_TAB - OLDATWRT"),
                new DataColumn(@"New Value ctxtG_CHAR_TAB - NEWATWRT"),
                new DataColumn(@"Table cell -TextField txtG_TARGET_TAB - OBJECT"),
                });

                DataTable dtImpactMatDesc = new DataTable(); 
                dtImpactMatDesc.Columns.AddRange(new DataColumn[] { new DataColumn (@"Material Number RMMG1 - MATNR"),
                new DataColumn(@"Material description MAKT - MAKTX"),});
               // foreach (DataRow row in oDataset.Tables[0].Rows)
               // {
               //     dt.Rows.Add(string.Format("{0}", row["Char_Name"].ToString()),
               //string.Format("{0}", row["Char_OldValue"].ToString()),
               //string.Format("{0}", row["Char_NewValue"].ToString()),
               //string.Format("{0}", row["Material"].ToString()),
               //string.Format("{0}", row["Description"].ToString()));


               //     dtImpactMatDesc.Rows.Add(
               //string.Format("{0}", row["Material"].ToString()),
               //string.Format("{0}", row["Description"].ToString()));
               // }
                //if (dt.Rows.Count > 0)
                //{
                //    string file = @"D:\SAPInterfaces\Outbound\CLMM_ChangeMatClass" + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                //    CNService.ToCSV(dt, file);
                //}
                
                
                //if (dtImpactMatDesc.Rows.Count > 0)
                //{
                //    string file = @"D:\SAPInterfaces\Outbound\MM02_ImpactMatDesc" + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                //    CNService.ToCSV(dtImpactMatDesc, file);
                //}
            }
        }
        public static void GetQuery(string sName)
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
            new DataColumn(@"IfColumn and Sales Organization RMMG1-VKORG and Reference sales organization RMMG1_REF-VKORG"),
            new DataColumn(@"Distribution Channel RMMG1-VTWEG and Reference distribution channel RMMG1_REF-VTWEG"),
            });
            foreach (DataRow row in Results.Rows)
            {
                string[] split = string.Format("{0}", row["Plant"].ToString()).Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in split)
                {

                    if (s.Trim() != "")
                    {
                        string[] splitSOOrg = "DM;EX".Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string o in splitSOOrg)
                        {
                            dt.Rows.Add(string.Format("{0}", row["DocumentNo"].ToString()),
                string.Format("{0}", row["Material"].ToString()),
                string.Format("{0}", row["Description"].ToString()),
                string.Format("{0}", row["Ref"].ToString()),
                string.Format("{0}", s),
                string.Format("{0}", row["Plant"]).Substring(0, 3),
                //string.Format("{0}", row["Plant"].ToString())
                
                string.Format("{0}", o)
                );
                        }
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
            //DataTable dtSaleOrg = new DataTable();
            //dtSaleOrg.Columns.AddRange(new DataColumn[] { new DataColumn(@"Material Number RMMG1-MATNR"),
            //    new DataColumn(@"Reference material RMMG1_REF-MATNR"),
            //new DataColumn(@"IfColumn and Sales Organization RMMG1-VKORG and Reference sales organization RMMG1_REF-VKORG"),
            //new DataColumn(@"Distribution Channel RMMG1-VTWEG and Reference distribution channel RMMG1_REF-VTWEG"),
            //});
            //DataTable dtSOOrg = new DataTable();
            //dtSOOrg.Columns.AddRange(new DataColumn[] { new DataColumn(@"SaleOrg") });
            //foreach (DataRow row in Results.Rows)
            //{
            //    string[] str = string.Format("{0}", row["Plant"].ToString()).Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            //    foreach (string s in str)
            //    {

            //        if (s.Trim() != "" && dtSOOrg.Select("SaleOrg='" + string.Format("{0}", s).Substring(0, 3) + "'").ToList().Count == 0)
            //        {
            //            dtSOOrg.Rows.Add(string.Format("{0}", s).Substring(0, 3));
            //        }
            //    }
            //    foreach (DataRow soorg in dtSOOrg.Rows)
            //    {
            //        string[] split = "DM;EX".Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            //        foreach (string s in split)
            //        {
            //            dtSaleOrg.Rows.Add(string.Format("{0}", row["Material"].ToString()),
            //        string.Format("{0}", row["Ref"].ToString()),
            //        string.Format("{0}", soorg["SaleOrg"].ToString()),
            //        //string.Format("{0}", row["Plant"].ToString())
            //        string.Format("{0}", s)
            //        );
            //        }
            //    }
            //    if (dtSaleOrg.Rows.Count > 0)
            //    {
            //        string file = @"D:\SAPInterfaces\Outbound\MM01_ExtendSaleOrg_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
            //        //string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/MM01_ExtendSaleOrg_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv");
            //        CNService.ToCSV(dtSaleOrg, file);
            //    }
            //}
            //Characteristics
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
        public static void GetUpdateTOSQL(string data)
        {
            var dir = @"D:\SAPInterfaces\Inbound";
            //HttpContext.Current.Server.MapPath("~/ExcelFiles");
            var filePaths = Directory.GetFiles(dir, "CT04*_Result*.csv");
            foreach (string s in filePaths)
            {
                using (var reader = new StreamReader(s))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        while (csv.Read())
                        {//This will advance the reader to the next record.

                            //You can use an indexer to get by position or name. 
                            //This will return the field as a string

                            // By position
                            var field = csv[0];
                            //var AppID = csv["AppID"];
                            // By header name
                            //csv.Read();
                            csv.ReadHeader();
                            string[] headerRow = csv.Context.Reader.HeaderRecord;
                            string[] filteredValues = Array.FindAll(headerRow, x => x.Contains("Result"));
                            var Condition = "";// csv["Condition TypeRV13A-KSCHL"];
                            if (filteredValues.Length > 0)
                                if (csv["Result"] == "Condition records saved")
                                {
                                    string Changed_Id = "";
                                    string Changed_Action = "";
                                    string Material = "";
                                    string Description = "";
                                    string DMSNo = "";
                                    string New_Material = "";
                                    string New_Description = "";
                                    string Status = "";
                                    string Reason = "";
                                    string NewMat_JobId = "";
                                    string Char_Name = "";
                                    string Char_OldValue = "";
                                    string Char_NewValue = "";
                                    SendEmailUpdateMaster("U" + Material);
                                    using (SqlConnection con = new SqlConnection(CNService.strConn))
                                    {
                                        SqlCommand cmd = new SqlCommand();
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.CommandText = "spUpdateImpactedmat";

                                        cmd.Parameters.AddWithValue("@Changed_Id", Changed_Id);
                                        cmd.Parameters.AddWithValue("@Changed_Action", Changed_Action);
                                        cmd.Parameters.AddWithValue("@Material", Material);
                                        cmd.Parameters.AddWithValue("@Description", Description);
                                        cmd.Parameters.AddWithValue("@DMSNo", DMSNo);
                                        cmd.Parameters.AddWithValue("@New_Material", New_Material);
                                        cmd.Parameters.AddWithValue("@New_Description", New_Description);
                                        cmd.Parameters.AddWithValue("@Status", Status);
                                        cmd.Parameters.AddWithValue("@Reason", Reason);
                                        cmd.Parameters.AddWithValue("@NewMat_JobId", NewMat_JobId);
                                        cmd.Parameters.AddWithValue("@Char_Name", Char_Name);
                                        cmd.Parameters.AddWithValue("@Char_OldValue", Char_OldValue);
                                        cmd.Parameters.AddWithValue("@Char_NewValue", Char_NewValue);


                                        cmd.Connection = con;
                                        con.Open();
                                        DataTable dtResult = new DataTable();
                                        SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                                        oAdapter.Fill(dtResult);
                                        con.Close();
                                    }
                                    //    using (SqlConnection con = new SqlConnection(strConn))
                                    //    {
                                    //        SqlCommand cmd = new SqlCommand();
                                    //        cmd.CommandType = CommandType.StoredProcedure;
                                    //        cmd.CommandText = "spUpdateQuotationfromJob";
                                    //        cmd.Parameters.AddWithValue("@MinPrice", string.Format("{0}", 0));
                                    //        cmd.Parameters.AddWithValue("@subID", string.Format("{0}", Condition.ToString() == "zpm1" ? "0" : AppID.ToString()));
                                    //        cmd.Parameters.AddWithValue("@ID", AppID.ToString());
                                    //        cmd.Connection = con;
                                    //        con.Open();
                                    //        cmd.ExecuteNonQuery();
                                    //        con.Close();
                                    //    }
                                    //}
                                }
                            //var records = csv.GetRecords<dynamic>();
                            //foreach (var item in records)
                            //{
                            //    var da = item.Result;
                            //    var condition = item[@"Condition Type RV13A-KSCHL"];  
                            //}
                            //foreach (dynamic record in records.ToList())
                            //{
                            //    var data = record["IfColumn"];
                            //}
                        }
                    }
                }
                try
                {
                    //File.Move(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s));
                    //HttpContext.Current.Server.MapPath("~/ExcelFiles/Processed/" + Path.GetFileName(s)));
                    // Copy the file and overwrite if it exists
                    File.Copy(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s), true);

                    // Delete the source file
                    File.Delete(s);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }
            }
            // Move MM01 & BAPI_UpdateMATCharacteristics
            filePaths = Directory.GetFiles(dir, "MM01*_Result*.csv");
            foreach (string s in filePaths)
            {
                try
                {
                    using (var reader = new StreamReader(s))
                    {
                        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        {
                            while (csv.Read())
                            {//This will advance the reader to the next record.

                                //You can use an indexer to get by position or name. 
                                //This will return the field as a string

                                // By position
                                var field = csv[0];
                                //var AppID = csv["AppID"];
                                // By header name
                                //csv.Read();
                                csv.ReadHeader();
                                string[] headerRow = csv.Context.Reader.HeaderRecord;
                                string[] filteredValues = Array.FindAll(headerRow, x => x.Contains("Result"));
                                var Condition = "";// csv["Condition TypeRV13A-KSCHL"];
                                if (filteredValues.Length > 0)
                                    if (csv["Result"] == "Condition records saved")
                                    {
                                    }
                            }
                        }
                    }
                    //File.Move(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s));
                    //HttpContext.Current.Server.MapPath("~/ExcelFiles/Processed/" + Path.GetFileName(s)));
                    // Copy the file and overwrite if it exists
                    File.Copy(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s), true);

                    // Delete the source file
                    File.Delete(s);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }
            }
            // Move BAPI_UpdateMATCharacteristics
            filePaths = Directory.GetFiles(dir, "BAPI*_Result*.csv");
            foreach (string s in filePaths)
            {
                try
                {
                    //File.Move(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s));
                    //HttpContext.Current.Server.MapPath("~/ExcelFiles/Processed/" + Path.GetFileName(s)));
                    // Copy the file and overwrite if it exists
                    File.Copy(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s), true);

                    // Delete the source file
                    File.Delete(s);
                }
                catch (IOException iox)
                {
                    Console.WriteLine(iox.Message);
                }
            }
        }
        public static void localProcessKill(string processName)
        {
            foreach (Process p in Process.GetProcessesByName(processName))
            {
                p.Kill();
            }
        }
        public static void testsendmaster(string SubChanged_Id)
        {
            string strSQL = " select Id,Changed_Charname,Description,Changed_Action,Old_Description from TransMaster where Changed_id ='" + SubChanged_Id + "'";
            DataTable dt = CNService.builditems(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                string _Id = dr["Id"].ToString();
                string _Description = dr["Description"].ToString();
                string _Changed_Action = dr["Changed_Action"].ToString();
                string _old_id = dr["Old_Description"].ToString();
                string[] value = { dr["Changed_Charname"].ToString(), _Description, _Id, _Changed_Action, _old_id };
                CNService.master_artwork(value);
            }
        }
        public static void SendEmailUpdateMaster(string _name)
        {
            //string datapath = "~/FileTest/" + _name;
            string _email = "";
            string _Id = "";
            string _Description = "";
            string _Body = "";
            string _Attached = "";
            string SubChanged_Id = CNService.ReadItems(@"select cast(substring('"
            + _name.ToString() + "',2,len('" + _name.ToString() + "')-1) as nvarchar(max)) value");
            testsendmaster(SubChanged_Id);
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(CNService.strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spsendemail_upm";
                cmd.Parameters.AddWithValue("@Changed_Id", _name.ToString());
                cmd.Connection = con;
                con.Open();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
            }
            foreach (DataRow dr in dt.Rows)
            {
                _email = dr["Email"].ToString();
                _Id = dr["Id"].ToString();
                _Description = dr["Description"].ToString();
                _Body = dr["Body"].ToString();
                _Attached = dr["attached"].ToString();
            }
            //        MailMessage msg = new MailMessage();
            //        string[] words = _email.Split(';');
            //        foreach (string word in words)
            //        {
            //            msg.To.Add(new MailAddress(word));
            //            //Console.WriteLine(word);
            //        }
            //        //msg.To.Add(new MailAddress(_email));
            //        msg.From = new MailAddress("wshuttleadm@thaiunion.com");
            //        msg.Subject = "Maintained characteristic master data in SAP" + "[" + _Body.Substring(0, 6) + "]";
            //        //msg.Body = "Id  " + _Id.ToString() + "Description  " + _Description.ToString() + " Changed";
            //        //msg.Body = "Maintained characteristic master completed";
            //        msg.Body = _Body;
            //        //msg.Attachments.Add(new System.Net.Mail.Attachment(_Attached));
            //        msg.IsBodyHtml = true;
            //
            //        SmtpClient client = new SmtpClient();
            //        client.UseDefaultCredentials = false;
            //        client.Credentials = new System.Net.NetworkCredential("wshuttleadm@thaiunion.com", "WSP@ss2018");
            //        client.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
            //        client.Host = "smtp.office365.com";
            //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //        client.EnableSsl = true;
            //        try
            //        {
            //            client.Send(msg);
            //            Context.Response.Write("Message Sent Succesfully");
            //        }
            //        catch (Exception ex)
            //        {
            //            Context.Response.Write(ex.ToString());
            //        }
            CNService.sendemail(_email, "", _Body,
                "PRD Characteristic master is maintained in SAP " + "[" + _Body.Substring(0, 6) + "]",
                _Attached);
        }
    }
}

