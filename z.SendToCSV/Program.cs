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

namespace z.SendToCSV
{
    public class Program
    {
        public static string InterfacePathInbound = ConfigurationSettings.AppSettings["InterfacePathInbound"];
        public static string InterfacePathOutbound = ConfigurationSettings.AppSettings["InterfacePathOutbound"];
        static void Main(string[] args)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(CNService.strConn))
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

                    //GetmasterUpdateToCSV(oDataset.Tables[0]);
                    SQ01(oDataset.Tables[0]);
                    CT04(oDataset.Tables[0]);
                }
                GetQuery("X"); //BAPI_UpdateMATCharacteristics, MM01
                GetImpactmat("X"); //CLMM, MM02
                GetUpdateTOSQL("X"); //ReadCSVToIgrid
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine($"Message :{e.Message} ");
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
        public static void SQ01(DataTable Results)
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
                string file = InterfacePathOutbound + "SQ01_ListMat" + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";                
                CNService.ToCSV(listMat, file);
            }
        }
        public static void CT04(DataTable Results)
        {
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
                        dtCT04Insert.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString().Replace("Insert", "I")),
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
    }
}

