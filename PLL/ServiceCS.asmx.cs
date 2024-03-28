using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using DAL.Model;
using DAL;
using WebServices.Model;
using System.IO;
using BLL.Services;
using BLL.BizMM65Service;
using WebServices.Helper;
using System.Xml.Serialization;
using ClosedXML.Excel;

namespace PLL
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        //string strConn = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        [WebMethod]
        public  void FixSOInterface(string param)
        {
            CNService.buildinterface(param);
        }
        [WebMethod]
        public  DataSet GetMasterData(string sName)
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
                //GetmasterUpdateToCSV(oDataset.Tables[0]);
                return oDataset;
            }
        }
        [WebMethod]
        public  string convertthai(string text)
        {
            try
            {
                dynamic dynJson = JsonConvert.DeserializeObject(text);
                foreach (var item in dynJson)
                {
                    text = item.tmpstr.ToString();
                }    //otherstuff        

            }
            catch (Exception ex)
            {
                //error loging stuff
            }
            return text;
        }
        [WebMethod()]
        public  void Getjson(string sName)
        {
            //sName = sName.Replace('@', '%');
            sName = convertthai(sName);
            using (SqlConnection oConn = new SqlConnection(CNService.strConn))
            {
                oConn.Open();
                //sName = "PrimarySize Where Description like '%307%'";
                string strQuery = "select * from " + sName;
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, oConn);
                // Fill the dataset.
                oAdapter.Fill(dt);
                oConn.Close();
                System.Web.HttpContext.Current.Response.Write(JsonConvert.SerializeObject(dt));
            }
        }
        //[WebMethod()]
        //public  void SetUpdateToCSV()
        //{
        //    CNService.SetUpdateMasterToCSV("");
        //}
        public  void GetmasterUpdateToCSV(DataTable Results)
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

                        CNService.ToCSV(dtclone, file);
                    }
                }
        }
        [WebMethod]
        public  void OutboundArtwork(string Keys)
        {
            CNService.OutboundArtwork(string.Format("{0}", Keys));
        }
        [WebMethod]
        public  void SendEmail(string _name)
        {
            //string datapath = "~/FileTest/" + _name;
            string _email = "";
            string _Material = "";
            string _Description = "";
            string _Body = "";
            string _Attached = "";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(CNService.strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spsendemail";
                cmd.Parameters.AddWithValue("@Material", _name.ToString());
                cmd.Connection = con;
                con.Open();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
            }
            foreach (DataRow dr in dt.Rows)
            {
                _email = dr["Email"].ToString();
                _Material = dr["Material"].ToString();

                _Description = dr["Description"].ToString();
                _Body = dr["body"].ToString();
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
            //        msg.Subject = "System SEC PKG Template is created No. : " + _Material.ToString() + "/" + _Description.ToString() + "/" + "Create Material";
            //        //msg.Body = "Material  " + _Material.ToString() + " Created";
            //        msg.Body = _Body;
            //        msg.Attachments.Add(new System.Net.Mail.Attachment(_Attached));
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
                "PRD System SEC PKG Template is created No. : " + _Material.ToString() + "/" + _Description.ToString() + "/" + "Create Material",
                _Attached);
            string _DocumentNo = CNService.ReadItems("select DocumentNo from SapMaterial Where Material='" + _Material.ToString() + "' and StatusApp<>5");
            CNService.OutboundArtwork(_DocumentNo.ToString());
            //string senderID = "voravut.somb@gmail.com";
            //string senderPassword = "063446620";
            //string result = "Email Sent Successfully";

            //string body = " " + _name + " has sent an email from " + _email;
            //body += "Phone : " + _phone;
            //body += _description;
            //try
            //{
            //    MailMessage mail = new MailMessage();
            //    mail.To.Add("voravut.somboornpong@thaiunion.com");
            //    mail.From = new MailAddress(senderID);
            //    mail.Subject = "My Test Email!";
            //    mail.Body = body;
            //    mail.IsBodyHtml = true;
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = "smtp.gmail.com"; //Or Your SMTP Server Address
            //    smtp.Credentials = new System.Net.NetworkCredential(senderID, senderPassword);
            //    smtp.Port = 587;
            //    smtp.EnableSsl = true;
            //    smtp.Send(mail);
            //}
            //catch (Exception ex)
            //{
            //    result = "problem occurred";
            //    Context.Response.Write("Exception in sendEmail:" + ex.Message);
            //}
            //Context.Response.Write(result);
        }
         DataTable GetTable()
        {
            DataTable table = new DataTable("Test");//DataTable with name - works fine
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Condition", typeof(string));
            table.Columns.Add("RequestType", typeof(string));
            table.Columns.Add("DocumentNo", typeof(string));
            table.Columns.Add("DMS No./ Artwork", typeof(string));
            table.Columns.Add("Material No.", typeof(string));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("Group", typeof(string));
            table.Columns.Add("Brand", typeof(string));
            table.Columns.Add("Assignee(PG Name)", typeof(string));
            table.Columns.Add("CreateOn", typeof(string));
            table.Columns.Add("ActiveBy(PA Name)", typeof(string));
            table.Columns.Add("FinalInfoGroup", typeof(string));
            table.Columns.Add("ReferenceMaterial", typeof(string));
            table.Columns.Add("Vendor", typeof(string));
            table.Columns.Add("Vendor description", typeof(string));
            // Here we add five DataRows.
            return table;
        }
        public  void GetsaveInfoGrouprpa(string Id, string InfoGroup, string user, string Check_PChanged)
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

        [WebMethod]
        public  void massinfogroup()
        {
            //Save the uploaded Excel file.
            string rootFolderSuccess = @"\\192.168.5.20\Packaging\TU_002_AssignMaterial\Output\";
            //string rootFolderSuccess = HttpContext.Current.Server.MapPath(@"~/Output/");
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
                            //if ((string.Format("{0}", row.Cell(18).Value) == "Yes" && string.Format("{0}", row.Cell(19).Value) == "Yes" && row.Cell(2).GetValue<string>().ToLower().Trim().Contains("known so"))
                            //    || (string.Format("{0}", row.Cell(17).Value) == "Yes" && string.Format("{0}", row.Cell(18).Value) == "Yes" && string.Format("{0}", row.Cell(19).Value) == "Yes"))
                            //{
                            if ((row.Cell(18).ValueCached == "Yes" && row.Cell(19).ValueCached == "Yes" && row.Cell(2).GetValue<string>().ToLower().Trim().Contains("known so"))
                            || (row.Cell(17).ValueCached == "Yes" && row.Cell(18).ValueCached == "Yes" && row.Cell(19).ValueCached == "Yes"))
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
                string result = Path.GetFileNameWithoutExtension(_file);
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

                if (HttpContext.Current.Session != null) { HttpContext.Current.Session.Clear(); }
                HttpContext.Current.Response.Write("success");

            }

        }
        [WebMethod]
        public  void ExportDataSetToExcel()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(CNService.strConn))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spinfogroupRPA";
                    //cmd.Parameters.AddWithValue("@user", user);
                    cmd.Parameters.AddWithValue("@where", "");
                    cmd.Connection = con;
                    con.Open();
                    DataTable dtx = GetTable();
                    DataTable dt = new DataTable();
                    SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                    oAdapter.Fill(dt);
                    con.Close();
                    foreach (DataRow row in dt.Rows)
                    {
                        dtx.Rows.Add(
                        row["Id"],
                        row["Condition"],
                        row["RequestType"],
                        row["DocumentNo"],
                        row["DMSNo"],
                        row["Material"],
                        row["Description"],
                        row["MaterialGroup"],
                        row["Brand"],
                        row["Assign"],
                        row["CreateOn"],
                        row["CreateBy"],
                        row["FinalInfoGroup"],
                        row["ReferenceMaterial"],
                        row["Vendor"],
                        row["Vendor description"]);
                    }
                    string rootFolder = @"\\192.168.5.20\Packaging\TU_002_AssignMaterial\Input\";
                    string _d = DateTime.Now.ToString("yyyyMMddHHmmss");
                    //string rootFolder = HttpContext.Current.Server.MapPath(@"~/ExcelFiles/");
                    string Pathfilename = string.Format("{0}infogroup_{1}.xlsx", rootFolder, _d);
                    string[] files = Directory.GetFiles(rootFolder);
                    foreach (string _file in files)
                    {
                        File.Delete(_file);
                    }
                    var workbook = new XLWorkbook();
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dtx);
                        var worksheet = wb.Worksheets.Add(_d);
                        wb.SaveAs(@Pathfilename);
                    }
                }
                HttpContext.Current.Response.Write("success");
            }
            catch (Exception e)
            {
                HttpContext.Current.Response.Write(e.Message);
                // Action after the exception is caught  
            }
        }
        [WebMethod]
        public  void GetSchSendMail(string args)
        {
            string[] eventlog = { "Inactive", "Re-Active"};
            for (int i = 0; i < eventlog.Length; i++)
            {
                //Console.WriteLine(eventlog[i]);
                DataSet dataSet = new DataSet();
                using (SqlConnection con = new SqlConnection(CNService.strConn))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "spsendemailobsolete";
                    cmd.Parameters.AddWithValue("@Changed_Action", string.Format("{0}", eventlog[i]));
                    cmd.Connection = con;
                    con.Open();
                    SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                    oAdapter.Fill(dataSet, "header");
                    con.Close();
                }
                //DataSet dataSet = CNService.getDataSet(@"select * from TransChanged_log where active='N'");
                //string htmlString = CNService.getHtml(dataSet);
                string htmlString = "Send file obsolete mat";
                if (string.Format("{0}", eventlog[i]) == "Re-Active")
                    htmlString = "Send Mat of "  + string.Format("{0} Master", eventlog[i]);
                //XLWorkbook wb = new XLWorkbook();
                //DataTable dt = dataSet.Tables[0]; 
                //wb.Worksheets.Add(dt, "WorksheetName");
                //CNService.SendAutomatedEmail(htmlString, "email@domain.com");
                try
                {
                    using (var context = new ARTWORKEntities())

                    {
                        var listConstaints = context.ART_M_CONSTANT.Where(w => w.VARIABLE_NAME == "MAIL" && w.PROGRAM_NAME == "SENDMAIL" && w.IS_ACTIVE == "X" && w.OPTION == "EQ").ToList();
                        if (listConstaints != null && listConstaints.Count > 0)

                            foreach (var funcs in listConstaints)
                            {
                                string filepath = CNService.ExportDataSetToExcel2(dataSet, eventlog[i]);
                                CNService.sendemail(funcs.LOWVALUE, "", htmlString, "PRD System "+ htmlString, filepath);
                                //Context.Response.Write("success");
                                //if (File.Exists(filepath))
                                //{
                                //    File.Delete(filepath);
                                //}
                            }
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.Current.Response.Write(ex);
                }
            }
        }

       
        [WebMethod]
        public  void SendEmailUpdateMaster(string _name)
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
        [WebMethod]
        public  void saveImpactedMatDesc(string Id, string Reason, string Status)
        {
            using (SqlConnection con = new SqlConnection(CNService.strConn))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spSaveImpactedMatDesc";
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Parameters.AddWithValue("@Reason", Reason);
                cmd.Parameters.AddWithValue("@Status", Status);
                cmd.Connection = con;
                con.Open();
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
                HttpContext.Current.Response.Write(JsonConvert.SerializeObject(dt));
            }
        }
        [WebMethod]
        public  DataSet GetQuery(string sName)
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
                //GetQueryToSQL(oDataset.Tables[0]);
                return oDataset;
            }
        }
        public  void GetQueryToSQL(DataTable Results)
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
        [WebMethod]
        public  void testsendmaster(string SubChanged_Id)
        {
            string strSQL = " select Id,Changed_Charname,Description,Changed_Action,Old_Description from TransMaster where Changed_id ='" + SubChanged_Id + "'";
            DataTable dt = CNService.builditems(strSQL);
            foreach (DataRow dr in dt.Rows)
            {
                string _Id = dr["Id"].ToString();
                string _Description = dr["Description"].ToString();
                string _Changed_Action = dr["Changed_Action"].ToString();
                string _old_id = dr["Old_Description"].ToString();
                string[] value = { dr["Changed_Charname"].ToString(), _Description, _Id, _Changed_Action,_old_id };
                CNService.master_artwork(value);
            }
            //string strSQL = " select * from TransMaster where Changed_id ='" + SubChanged_Id + "'";
            //int userID = -2;
            //DataTable dt = CNService.builditems(strSQL);
            //foreach (DataRow dr in dt.Rows)
            //{
            //    if (string.Format("{0}", dr["Changed_Tabname"]).Equals("MasBrand"))
            //    {
            //            string _Id = dr["Id"].ToString();
            //            string _Description = dr["Description"].ToString();
            //            string[] value = { dr["Changed_Charname"].ToString(), _Description, _Id };
            //            CNService.master_artwork(value);   
            //    }
            //    else if (string.Format("{0}", dr["Changed_action"]).Equals("Insert"))
            //    {
            //        if (string.Format("{0}", dr["Changed_Tabname"]).Equals("MasPackingStyle"))
            //        {
            //            DataTable dtPackingStyle  = CNService.builditems(@"select * from MasPackingStyle where Id='"+
            //                string.Format("{0}", dr["Id"])
            //                + "'");
            //            foreach (DataRow drPackingStyle in dtPackingStyle.Rows)
            //            {
            //                using (var context = new ARTWORKEntities())
            //                {
            //                    SAP_M_2P sapm2p = new SAP_M_2P();
            //                    sapm2p.PACKING_SYLE_VALUE = string.Format("{0}", drPackingStyle["RefStyle"]);
            //                    sapm2p.PACKING_SYLE_DESCRIPTION = string.Format("{0}", drPackingStyle["RefStyle"]);
            //                    sapm2p.PACK_SIZE_VALUE = string.Format("{0}", drPackingStyle["PackSize"]);
            //                    sapm2p.PACK_SIZE_DESCRIPTION = string.Format("{0}", drPackingStyle["PackSize"]);
            //                    var existItem = SAP_M_2P_SERVICE.GetByItem(sapm2p, context).FirstOrDefault();
            //                    if (existItem == null)
            //                    {
            //                        sapm2p.IS_ACTIVE = "X";
            //                        sapm2p.CREATE_BY = userID;
            //                        sapm2p.CREATE_DATE = DateTime.Today;
            //                        sapm2p.UPDATE_BY = userID;
            //                        sapm2p.UPDATE_DATE = DateTime.Today;
            //                        SAP_M_2P_SERVICE.SaveOrUpdateNoLog(sapm2p, context);
            //                    }
            //                }
            //            }
            //        }
            //        else if (string.Format("{0}", dr["Changed_Tabname"]).Equals("MasPrimarySize"))
            //        {
            //            DataTable dtPrimarySize = CNService.builditems(@"select * from MasPrimarySize where [Id]='" +
            //                string.Format("{0}", dr["Id"])
            //                + "'");
            //            foreach (DataRow drPrimarySize in dtPrimarySize.Rows)
            //            {
            //                using (var context = new ARTWORKEntities())
            //                {
            //                    SAP_M_3P sapm3p = new SAP_M_3P();
            //                    sapm3p.PRIMARY_SIZE_VALUE = string.Format("{0}", drPrimarySize["Description"]);
            //                    sapm3p.PRIMARY_SIZE_DESCRIPTION = string.Format("{0}", drPrimarySize["Description"]);
            //                    sapm3p.CONTAINER_TYPE_VALUE = string.Format("{0}", drPrimarySize["ContainerType"]);
            //                    sapm3p.CONTAINER_TYPE_DESCRIPTION = string.Format("{0}", drPrimarySize["ContainerType"]);
            //                    sapm3p.LID_TYPE_VALUE = string.Format("{0}", drPrimarySize["DescriptionType"]);
            //                    sapm3p.LID_TYPE_DESCRIPTION = string.Format("{0}", drPrimarySize["DescriptionType"]);
            //                    var existItem = SAP_M_3P_SERVICE.GetByItem(sapm3p, context).FirstOrDefault();
            //                    if (existItem == null)
            //                    {
            //                        sapm3p.IS_ACTIVE = "X";
            //                        sapm3p.CREATE_BY = userID;
            //                        sapm3p.CREATE_DATE = DateTime.Today;
            //                        sapm3p.UPDATE_BY = userID;
            //                        sapm3p.UPDATE_DATE = DateTime.Today;
            //                        SAP_M_3P_SERVICE.SaveOrUpdateNoLog(sapm3p, context);
            //                    }
            //                }
            //            }
            //        }

            //        string _Id = dr["Id"].ToString();
            //        string _Description = dr["Description"].ToString();
            //        string[] value = { dr["Changed_Charname"].ToString(), _Description, _Id };
            //        CNService.master_artwork(value);

            //    }
            //    else if (string.Format("{0}", dr["Changed_action"]).Equals("Re-Active"))
            //    {
            //        using (var context = new ARTWORKEntities())
            //        {
            //            context.Database.CommandTimeout = 600;
            //            if (string.Format("{0}", dr["Changed_Tabname"]).Equals("MasPackingStyle"))
            //            {
            //                DataTable dtStyle = CNService.builditems(@"select * from MasPackingStyle where Id='" +
            //                    string.Format("{0}", dr["Old_Id"])
            //                    + "'");
            //                foreach (DataRow drStyle in dtStyle.Rows)
            //                {

            //                    SAP_M_2P sapm2p = new SAP_M_2P();
            //                    sapm2p.PACKING_SYLE_VALUE = string.Format("{0}", drStyle["RefStyle"]);
            //                    sapm2p.PACKING_SYLE_DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", drStyle["PackSize"]);
            //                    sapm2p.PACK_SIZE_VALUE = string.Format("{0}", drStyle["PackSize"]);
            //                    sapm2p.PACK_SIZE_DESCRIPTION = string.Format("{0}", drStyle["PackSize"]);
            //                    var existItem2 = SAP_M_2P_SERVICE.GetByItem(sapm2p, context).FirstOrDefault();
            //                    if (existItem2 != null)
            //                    {
            //                        sapm2p.PACKING_SYLE_DESCRIPTION = string.Format("{0}", drStyle["RefStyle"]);
            //                        sapm2p.IS_ACTIVE = "X";
            //                        sapm2p.CREATE_BY = userID;
            //                        sapm2p.CREATE_DATE = DateTime.Today;
            //                        sapm2p.UPDATE_BY = userID;
            //                        sapm2p.UPDATE_DATE = DateTime.Today;
            //                        SAP_M_2P_SERVICE.SaveOrUpdateNoLog(sapm2p, context);
            //                    }

            //                }
            //            }
            //            SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();

            //            characteristic.NAME = string.Format("{0}", dr["Changed_Charname"]);
            //            characteristic.VALUE = string.Format("{0}-XXX Do Not Use XXX", dr["Old_Id"]);
            //            characteristic.DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", dr["Old_Id"]);

            //            var existItem = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(characteristic, context).FirstOrDefault();

            //            if (existItem != null)
            //            {
            //                characteristic.CHARACTERISTIC_ID = existItem.CHARACTERISTIC_ID;
            //            }
            //            characteristic.VALUE = string.Format("{0}", dr["Id"]);
            //            characteristic.DESCRIPTION = string.Format("{0}", dr["Description"]);
            //            characteristic.IS_ACTIVE = "X";
            //            characteristic.CREATE_BY = userID;
            //            characteristic.CREATE_DATE = DateTime.Today;
            //            characteristic.UPDATE_BY = userID;
            //            characteristic.UPDATE_DATE = DateTime.Today;

            //            SAP_M_CHARACTERISTIC_SERVICE.SaveOrUpdateNoLog(characteristic, context);
            //        }
            //    }
            //    else
            //    {

            //        using (var context = new ARTWORKEntities())
            //        {
            //            context.Database.CommandTimeout = 600;
            //            if (string.Format("{0}", dr["Changed_Tabname"]).Equals("MasPackingStyle"))
            //            {
            //                DataTable dtStyle = CNService.builditems(@"select * from MasPackingStyle where Id='" +
            //                    string.Format("{0}", dr["Old_Id"])
            //                    + "'");
            //                foreach (DataRow drStyle in dtStyle.Rows)
            //                {

            //                    SAP_M_2P sapm2p = new SAP_M_2P();
            //                    sapm2p.PACKING_SYLE_VALUE = string.Format("{0}", drStyle["RefStyle"]);
            //                    sapm2p.PACKING_SYLE_DESCRIPTION = string.Format("{0}", drStyle["RefStyle"]);
            //                    sapm2p.PACK_SIZE_VALUE = string.Format("{0}", drStyle["PackSize"]);
            //                    sapm2p.PACK_SIZE_DESCRIPTION = string.Format("{0}", drStyle["PackSize"]);
            //                    var existItem2 = SAP_M_2P_SERVICE.GetByItem(sapm2p, context).FirstOrDefault();
            //                    if (existItem2 != null)
            //                    {
            //                        sapm2p.PACKING_SYLE_DESCRIPTION = string.Format("{0}", drStyle["PackingStyle"]);
            //                        sapm2p.IS_ACTIVE = "X";
            //                        sapm2p.CREATE_BY = userID;
            //                        sapm2p.CREATE_DATE = DateTime.Today;
            //                        sapm2p.UPDATE_BY = userID;
            //                        sapm2p.UPDATE_DATE = DateTime.Today;
            //                        SAP_M_2P_SERVICE.SaveOrUpdateNoLog(sapm2p, context);
            //                    }

            //                }
            //            }

            //            SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();

            //            characteristic.NAME = string.Format("{0}", dr["Changed_Charname"]);
            //            characteristic.VALUE = string.Format("{0}", dr["Old_Description"]);
            //            characteristic.DESCRIPTION = string.Format("{0}", dr["Old_Description"]);

            //            var existItem = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(characteristic, context).FirstOrDefault();

            //            if (existItem != null)
            //            {
            //                characteristic.CHARACTERISTIC_ID = existItem.CHARACTERISTIC_ID;
            //            }
            //            characteristic.VALUE = string.Format("{0}", dr["Description"]);
            //            characteristic.DESCRIPTION = string.Format("{0}", dr["Description"]);
            //            characteristic.IS_ACTIVE = "X";
            //            characteristic.CREATE_BY = userID;
            //            characteristic.CREATE_DATE = DateTime.Today;
            //            characteristic.UPDATE_BY = userID;
            //            characteristic.UPDATE_DATE = DateTime.Today;

            //            SAP_M_CHARACTERISTIC_SERVICE.SaveOrUpdateNoLog(characteristic, context);

            //        }
            //    }
            //}
        }
        [WebMethod]
        public  DataSet GetImpactmat(string sName)
        {
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
                return oDataset;
            }
        }
        [WebMethod]
        public  void UpdateImpactedmat2(string Changed_Id, 
            string Changed_Action, 
            string Material, 
            string Description, 
            string DMSNo, 
            string New_Material, 
            string New_Description, 
            string Status, 
            string Reason, 
            string NewMat_JobId, 
            string Char_Name, 
            string Char_OldValue, 
            string Char_NewValue)
        {
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
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
                HttpContext.Current.Response.Write(JsonConvert.SerializeObject(dt));
            }
        }
    }
}
