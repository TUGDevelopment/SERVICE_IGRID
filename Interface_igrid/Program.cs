using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.Mail;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Xml.Linq;
using System.Net;
using System.Windows.Interop;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Presentation;
using System.Net.Http.Headers;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2013.Word;
using System.Reflection;
using System.Security.Policy;
//using DocumentFormat.OpenXml.Office2013.Excel;
//using BLL.MemberService;

namespace Interface_igrid
{
    public class Program
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            try
            {


                #region Inbound    
                if (bool.Parse(ConfigurationManager.AppSettings["runFlageInbound"]) == true) // flage for true run or false not run
                {
                    if (bool.Parse(ConfigurationManager.AppSettings["runFileInbound_MM01"]) == true)
                    {
                        await MyQueryMacro();
                    }
                    if (bool.Parse(ConfigurationManager.AppSettings["runFileInbound_CT04"]) == true)
                    {
                        await MyQuery2Macro();
                    }
                    if (bool.Parse(ConfigurationManager.AppSettings["runFileInbound_MM02"]) == true)
                    {
                        await MyQuery3Macro();
                    }
                    string imported = "";
                    try
                    {

                        var filesToImport = Directory.GetFiles(ConfigurationManager.AppSettings["InterfacePathInbound"], "*_Result.csv");
                        if (filesToImport != null)
                        {

                            FileInfo fileI = null;
                            string fileN = "";
                            foreach (string file in filesToImport)
                            {
                                fileI = new FileInfo(file);
                                fileN = fileI.Name;
                                string InterfaceCode = fileN.Substring(0, 6);
                                switch (InterfaceCode)
                                {
                                    case "MM01_C":
                                        if (bool.Parse(ConfigurationManager.AppSettings["runFileInbound_MM01"]) == true)
                                        {
                                            imported = Import_MM01_C(file, InterfaceCode);
                                        }
                                        break;
                                    case "BAPI_U":
                                        if (bool.Parse(ConfigurationManager.AppSettings["runFileInbound_MM01"]) == true)
                                        {
                                            imported = Import_BAPI_U(file, InterfaceCode);
                                        }
                                        break;
                                    case "BAPI_B":
                                        if (bool.Parse(ConfigurationManager.AppSettings["runFileInbound_MM01"]) == true)
                                        {
                                            imported = Import_BAPI_B(file, InterfaceCode);
                                        }
                                        break;
                                    case "MM02_I":
                                        if (bool.Parse(ConfigurationManager.AppSettings["runFileInbound_MM02"]) == true)
                                        {
                                            imported = Import_MM02_I(file, InterfaceCode);
                                        }
                                        break;
                                    case "CLMM_C":
                                        if (bool.Parse(ConfigurationManager.AppSettings["runFileInbound_MM02"]) == true)
                                        {
                                            imported = Import_CLMM_C(file, InterfaceCode);
                                        }
                                        break;
                                    case "CT04_I":
                                        if (bool.Parse(ConfigurationManager.AppSettings["runFileInbound_CT04"]) == true)
                                        {
                                            imported = Import_CT04_I(file, InterfaceCode);
                                        }
                                        break;
                                    //case "CT04_R": // Cancel CT04 Remove
                                    //    imported = Import_CT04_R(file, InterfaceCode);
                                    //    break;
                                    case "SQ01_L":
                                        if (bool.Parse(ConfigurationManager.AppSettings["runFileInbound_CT04"]) == true)
                                        {
                                            imported = Import_SQ01_L(file, InterfaceCode);
                                        }
                                        break;
                                }
                            }
                        }
                        Console.WriteLine("Inbound All Completed");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine($"Message :{e.Message} " + e.StackTrace);
                        Console.WriteLine("Inbound - Not success" + imported);
                        //4.send email to IT //5.sent email insert log 
                        if (bool.Parse(ConfigurationManager.AppSettings["ITEmailsNotifySuccessImport"]) == true)
                        {
                            string from = ConfigurationManager.AppSettings["SMTPFrom"];
                            string to = ConfigurationManager.AppSettings["ITEmailsNotify"];
                            string subject = "Inbound Not success";
                            string body = e.Message + e.StackTrace;
                            SendEmail(from, to, subject, body);
                            SendToLog(from, to, subject, body);
                        }
                    }
                }
                #endregion


                #region Outbound
                if (bool.Parse(ConfigurationManager.AppSettings["runFlageOutbound"]) == true) // flage for true run or false not run
                {
                    try
                    {
                        if (bool.Parse(ConfigurationManager.AppSettings["runFileOutbound_MM01"]) == true) // flage for true run or false not run
                        {
                            //MM01,Create material
                            DataSet dsspQuery = GetData("spQuery", "@Material", "X");
                            MM01_CreateMAT_ExtensionPlant(dsspQuery.Tables[0]);
                            BAPI_UpdateMATCharacteristics(dsspQuery.Tables[0]);
                            BAPI_BATCHCLASS(dsspQuery.Tables[0]);
                        }
                        if (bool.Parse(ConfigurationManager.AppSettings["runFileOutbound_MM02"]) == true) // flage for true run or false not run
                        {
                            //MM02-Update material description
                            DataSet dsspGetImpactmat = GetData("spGetImpactmat_desc", "@Active", "X");
                            MM02_ImpactMatDesc(dsspGetImpactmat.Tables[0]);
                            CLMM_ChangeMatClass(dsspGetImpactmat.Tables[0]);
                        }
                        if (bool.Parse(ConfigurationManager.AppSettings["runFileOutbound_CT04"]) == true) // flage for true run or false not run
                        {
                            //CT04-Characteristic master (I, U, D, Inactive, Re-Active)
                            DataSet dsspGetMasterData = GetData("spGetMasterData", "@Active", "X");
                            CT04(dsspGetMasterData.Tables[0]);
                            SQ01_ListMAT(dsspGetMasterData.Tables[0]);
                        }
                        Console.WriteLine("Outbound Completed");
                    }
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("\nException Caught!");
                        Console.WriteLine($"Message :{e.Message} ");
                        Console.WriteLine("Outbound - Not success");
                        //4.send email to IT //5.sent email insert log 
                        if (bool.Parse(ConfigurationManager.AppSettings["ITEmailsNotifySuccessImport"]) == true)
                        {
                            string from = ConfigurationManager.AppSettings["SMTPFrom"];
                            string to = ConfigurationManager.AppSettings["ITEmailsNotify"];
                            string subject = "Outbound Not success";
                            string body = e.Message + e.StackTrace;
                            SendEmail(from, to, subject, body);
                            SendToLog(from, to, subject, body);
                        }
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #region Macro 
        #region  MyQuery
        public async static Task<string> MyQueryMacro()
        {
            try
            {
                string filePathResult = ConfigurationManager.AppSettings["FilePathResult"];
                //Get Data From spQuery Store
                //Parameter
                string condition = "";
                string extended_Plant = "";
                bool statusup = false;
                string result = "";
                string material = "";
                List<string> lstFileName = await GetAllFileName("BAPI_UpdateMATCharacteristics");
                foreach (string fileName in lstFileName)
                {
                    DataTable dtResultMATCharacteristics = await GetDataTableFromResult(fileName);
                    if (dtResultMATCharacteristics != null)
                    {
                        foreach (DataRow rowMat in dtResultMATCharacteristics.Rows)
                        {
                            if (rowMat != null && !string.IsNullOrEmpty(rowMat["Result"].ToString()))
                            {
                                DataSet dsspQuery = GetData("spQuery", "@Material", "X");
                                var table = dsspQuery.Tables[0];
                                if (table == null || table.Rows.Count == 0)
                                {
                                    return "No Data In spGetImpactmat";
                                }
                                var mat = rowMat["Material Number RMMG1-MATNR"].ToString();
                                foreach (DataRow row in table.Rows)
                                {
                                    if (row["Material"].ToString() == mat)
                                    {
                                        material = row["Material"].ToString();
                                        condition = row["Condition"].ToString();
                                        extended_Plant = row["Extended_Plant"].ToString();
                                        //string cellValue = row["DocumentNo"].ToString(); // Get the cell value as a string
                                    }
                                }
                                var resultMat = rowMat["Result"].ToString();
                                result = resultMat;
                                //Get StartUp For Update DB
                                if (condition == "7")
                                {
                                    //Run MM02_ChangeMATDesc2
                                    //Set StartUp
                                    if (resultMat.ToLower().Contains("does not exist") && !statusup) { statusup = false; }
                                    //else if (resultMat.Contains("Material") && !statusup) { statusup = true; }
                                    else if (resultMat.ToLower().Contains("material " + material.ToLower()) && !statusup) { statusup = true; }
                                    //else if (resultMat.Contains("*Saving changes to assignments Assignment changed*") && !statusup) { statusup = true; }
                                    else if (resultMat.ToLower().Contains("assignment changed") && !statusup) { statusup = true; }
                                    else
                                    {
                                        //Get DataFilePath From MM02_ChangeMATDesc
                                        var dtResultChangeMATDesc = await GetDataTableFromResult("MM02_ChangeMATDesc");
                                        if (dtResultChangeMATDesc != null)
                                        {
                                            foreach (DataRow rowChangeMATDesc in dtResultChangeMATDesc.Rows)
                                            {
                                                //Split Data With ||
                                                if (rowChangeMATDesc != null && rowChangeMATDesc[0] != null)
                                                {
                                                    //Col Material Number RMMG1-MATNR
                                                    var dataInRow = rowChangeMATDesc[0].ToString();
                                                    if (!string.IsNullOrEmpty(dataInRow))
                                                    {
                                                        var getListResult = dataInRow.Split(new string[] { "||" }, StringSplitOptions.None).ToList();
                                                        if (getListResult != null && getListResult.Count > 0)
                                                        {
                                                            if (getListResult.Count > 0 && mat == getListResult[0])
                                                            {
                                                                //Col Result
                                                                string colResult = getListResult[getListResult.Count - 1];
                                                                //if (colResult.Contains("Material")) { statusup = true; }
                                                                result = colResult;
                                                                if (colResult.ToLower().Contains("material " + material.ToLower()))
                                                                {
                                                                    result = "P_" + colResult;
                                                                    statusup = true;
                                                                }
                                                                else { statusup = false; }

                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //**** Wassana P. insert on 11.01.2018 (Not change class but extend plant)
                                    if (!statusup && extended_Plant.ToLower() == "true")
                                    {
                                        List<string> listTableResult = new List<string> { "MM01_CreateMAT_ExtensionPlant", "MM01_ExtendSaleOrg" };
                                        foreach (var tableResult in listTableResult)
                                        {
                                            var dtResult = await GetDataTableFromResult(tableResult);
                                            string resultStr = ReadMM01Result(statusup, mat, material, dtResult);
                                            if (resultStr.ToLower().Contains("material " + material.ToLower()))
                                            {
                                                statusup = true;
                                                result = "P_" + resultStr;
                                            }
                                            {
                                                statusup = true;
                                                result = resultStr;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //Check BAPI_UpdateMATCharacteristics
                                    if (resultMat.ToLower().Contains("material " + material.ToLower()) && resultMat.ToLower().Contains("assignment changed"))
                                    {
                                        statusup = true;
                                        result = resultMat;
                                    }

                                    //MM01_CreateMAT_ExtensionPlant Col 11 MM01_ExtendSaleOrg Col 12

                                    List<string> listTableResult = new List<string> { "MM01_CreateMAT_ExtensionPlant", "MM01_ExtendSaleOrg" };
                                    foreach (var tableResult in listTableResult)
                                    {
                                        var dtResultExtensionPlant = await GetDataTableFromResult(tableResult);

                                        if (dtResultExtensionPlant != null)
                                        {
                                            var dtResult = await GetDataTableFromResult(tableResult);
                                            string resultStr = ReadMM01Result(statusup, mat, material, dtResult);
                                            if (resultStr.ToLower().Contains("material " + material.ToLower()))
                                            {
                                                statusup = true;
                                                result = "P_" + resultStr;
                                            }
                                        }
                                    }

                                    //if (resultMat.Contains("Material " + material + "  changed") || resultMat.Contains("*Saving changes to assignments Assignment changed*"))
                                    //if ( resultMat.ToLower().Contains("assignment changed"))
                                    //{
                                    //    statusup = true;
                                    //}
                                }
                                //Update DB
                                if (statusup)
                                {
                                    await MoveFile(fileName);
                                    //GenerateAttachedmentFile
                                    if (condition == "7")
                                    {
                                        await updateDB("U" + material, fileName);
                                    }
                                    else
                                    {
                                        //if (result.Contains("Material " + material + "  changed") || result.Contains("*Saving changes to assignments Assignment changed*"))
                                        if ((result.ToLower().Contains("material " + material.ToLower()) || result.ToLower().Contains("assignment changed")) && result.Substring(0, 2) != "P_")
                                        {
                                            await updateDB("O" + material, fileName);
                                        }
                                        else
                                        {
                                            await updateDB("P" + material, fileName);
                                        }
                                    }
                                }
                                else
                                {
                                    await MoveFile(fileName);
                                    //GenerateAttachedmentFile
                                    if (condition == "7")
                                    {
                                        //if (result.Contains("No changes made") || result.Contains("*Saving changes to assignments Assignment changed*"))
                                        if (result.ToLower().Contains("no changes made"))
                                        {
                                            await updateDB("Y" + material, fileName);
                                        }
                                        else
                                        {
                                            await updateDB("Z" + material, fileName);
                                        }
                                    }
                                    else
                                    {
                                        await updateDB("X" + material, fileName);
                                    }
                                }
                            }
                        }
                    }
                }
                await MoveFile("BAPI_UpdateMATCharacteristics");
                await MoveFile("MM02_ChangeMATDesc");
                await MoveFile("MM01_CreateMAT_ExtensionPlant");
                await MoveFile("MM01_ExtendSaleOrg");
                return "success";
            }
            catch (Exception ex)
            {
                //WriteLog("MyQueryMacro : " + ex.ToString());
                return ex.ToString();
            }
        }
        public async static Task<string> MyQuery2Macro()
        {
            try
            {
                string servicePathUrl = ConfigurationManager.AppSettings["ServicePathUrl"];
                string filePath = ConfigurationManager.AppSettings["FilePath"];

                DataTable dtResult = await GetDataTableFromResult("CT04_Insert");
                foreach (DataRow rowResult in dtResult.Rows)
                {
                    var checkStatus = rowResult["Result"].ToString();
                    if (string.IsNullOrEmpty(checkStatus) || checkStatus == "Result")
                    {
                        continue;
                    }
                    //Get Data From spGetMasterData
                    DataSet dsspGetMasterData = GetData("spGetMasterData", "@Active", "X");
                    var table = dsspGetMasterData.Tables[0];
                    if (table == null || table.Rows.Count == 0)
                    {
                        return "No Data In spGetImpactmat";
                    }

                    foreach (DataRow row in table.Rows)
                    {
                        if (rowResult["AppId"].ToString() == row["Changed_Id"].ToString())
                        {
                            // assign parameter
                            var action = row["Changed_Action"].ToString();
                            var changed_Id = row["Changed_Id"].ToString();
                            var char_name = row["Changed_Charname"].ToString();
                            var char_var = "";
                            if (rowResult["AppId"].ToString() == changed_Id)
                            {
                                if (char_name == "ZPKG_SEC_BRAND")
                                {
                                    char_var = row["Old_Id"].ToString();
                                }
                                else
                                {
                                    char_var = row["Old_Description"].ToString();
                                }
                                var char_newvar = row["Description"].ToString();

                                var old_matdesc = row["Old_Description"].ToString();
                                var new_matdesc = row["Description"].ToString();
                                //File Parameter
                                var fileName = "";
                                var char_filename = changed_Id + "_" + char_name;

                                //Check ulogin Characteristic Name
                                if (char_name != "ulogin")
                                {
                                    //Check Action And Run CT04
                                    if (action == "Insert")
                                    {
                                        //RunNormalTXRfile ("CT04_Insert")
                                    }
                                    else if (action == "Update" || action == "Inactive" || action == "Re-Active")
                                    {
                                        //RunNormalTXRfile ("SQ01_ListMAT")
                                        fileName = filePath + char_filename + ".txt";
                                        //RunNormalTXRfile ("CT04_ExactlyUpdated") 
                                    }
                                    else if (action == "Remove")
                                    {
                                        //RunNormalTXRfile ("CT04_Remove") 
                                    }

                                    //Update Paramenter
                                    string Str_MaterialNo = "";
                                    string Str_Class = "";
                                    string Str_CharName = "";
                                    string Str_CharValue = "";
                                    string Str_MaterialDesc = "";
                                    string Str_DMSno = "";
                                    string Str_NewMaterialNo = "";
                                    string Str_NewMaterialDesc = "";
                                    string Str_Action = "";
                                    string Str_Status = "";
                                    string Str_Reason = "";
                                    string Str_NewMat_JobId = "";
                                    string Str_Char_Name = "";
                                    string Str_Char_OldValue = "";
                                    string Str_Char_NewValue = "";

                                    //Update Master
                                    //Check Status
                                    if (checkStatus.Contains("changed") || checkStatus.Contains("already exists"))
                                    {
                                        //Check Action
                                        if (action == "Update" || action == "Inactive" || action == "Re-Active")
                                        {
                                            //Check FIle
                                            DataTable dtResultListMat = await GetDataTableFromResult("SQ01_ListMat");
                                            //Update Master
                                            //Row in File
                                            foreach (DataRow itemRow in dtResultListMat.Rows)
                                            {
                                                //Column In File

                                                Str_MaterialNo = itemRow[0].ToString();
                                                Str_Class = itemRow[2].ToString();
                                                Str_CharName = itemRow[3].ToString();
                                                Str_CharValue = itemRow[4].ToString();
                                                Str_MaterialDesc = itemRow[5].ToString();
                                                if (string.IsNullOrEmpty(Str_MaterialDesc))
                                                {
                                                    Str_DMSno = Str_MaterialDesc.Substring(0, Str_MaterialDesc.IndexOf(","));
                                                    if (Str_DMSno.Contains("-COR"))
                                                    {
                                                        Str_DMSno.Replace("-COR", "@");
                                                    }
                                                    else if (Str_DMSno.Contains("COR-"))
                                                    {
                                                        Str_DMSno.Replace("COR-", "@");
                                                    }
                                                    else
                                                    {
                                                        Str_DMSno.Replace("-", "@");
                                                    }
                                                }
                                                else
                                                {
                                                    Str_DMSno = "";
                                                }

                                                //Call Service Update
                                                if (action == "Inactive")
                                                {
                                                    Str_NewMaterialNo = "";
                                                    Str_NewMaterialDesc = "";
                                                    Str_Action = "Master Inactive";
                                                    Str_Status = "Pending";
                                                }
                                                else if (action == "Re-Active")
                                                {
                                                    Str_NewMaterialNo = "";
                                                    Str_NewMaterialDesc = "";
                                                    Str_Action = "Master Re-Active";
                                                    Str_Status = "Completed";
                                                }
                                                else if (action == "Update")
                                                {
                                                    if (Str_CharName == "ZPKG_SEC_BRAND")
                                                    {
                                                        Str_Action = "Update Description";
                                                        Str_Status = "Not Start";
                                                    }
                                                    else
                                                    {
                                                        Str_Action = "Update Characteristic Master";
                                                        Str_Status = "Not Start";
                                                    }
                                                }
                                                else
                                                {
                                                    Str_NewMaterialNo = "";
                                                    if (Str_MaterialDesc.Contains(","))
                                                    {
                                                        Str_NewMaterialDesc = Str_MaterialDesc.Substring(0, Str_MaterialDesc.IndexOf(",")) + new_matdesc;
                                                    }
                                                    else
                                                    {
                                                        Str_NewMaterialDesc = Str_MaterialDesc;
                                                    }
                                                }

                                                Str_Reason = "";
                                                Str_NewMat_JobId = "";
                                                Str_Char_Name = "";
                                                Str_Char_OldValue = "";
                                                Str_Char_NewValue = "";

                                                //Call Service UpdateImpactedmat2
                                                //GET Strquery = "http://" + sServer + "/WebService_jQuery_Ajax/ServiceCS.asmx/UpdateImpactedmat2?Changed_Id=" + Changed_Id + "&Changed_Action=" + Str_Action + "&Material=" + Str_MaterialNo + "&Description=" + Str_MaterialDesc + "&DMSNo=" + Str_DMSno + "&New_Material=" + Str_NewMaterialNo + "&New_Description=" + Str_NewMaterialDesc + "&Status=" + Str_Status + "&Reason=" + Str_Reason + "&NewMat_JobId=" + Str_NewMat_JobId + "&Char_Name=" + Str_Char_Name + "&Char_OldValue=" + Str_Char_OldValue + "&Char_NewValue=" + Str_Char_NewValue

                                                using (var httpClient = new HttpClient())
                                                {
                                                    using (var response = await httpClient.GetAsync(servicePathUrl + "UpdateImpactedmat2?Changed_Id=" + changed_Id + "&Changed_Action=" + Str_Action + "&Material=" + Str_MaterialNo + "&Description=" + Str_MaterialDesc + "&DMSNo=" + Str_DMSno + "&New_Material=" + Str_NewMaterialNo + "&New_Description=" + Str_NewMaterialDesc + "&Status=" + Str_Status + "&Reason=" + Str_Reason + "&NewMat_JobId=" + Str_NewMat_JobId + "&Char_Name=" + Str_Char_Name + "&Char_OldValue=" + Str_Char_OldValue + "&Char_NewValue=" + Str_Char_NewValue))
                                                    {
                                                        var apiResponse = await response.Content.ReadAsStringAsync();

                                                    }
                                                }

                                                Str_MaterialNo = "";
                                                Str_Class = "";
                                                Str_CharName = "";
                                                Str_CharValue = "";
                                                Str_MaterialDesc = "";
                                                Str_DMSno = "";
                                                Str_NewMaterialNo = "";
                                                Str_NewMaterialDesc = "";
                                                Str_Status = "";
                                                Str_Reason = "";
                                                Str_NewMat_JobId = "";
                                                Str_Char_Name = "";
                                                Str_Char_OldValue = "";
                                                Str_Char_NewValue = "";
                                            }
                                            await UpdateMaster("U" + changed_Id);
                                        }
                                        else
                                        {
                                            //Update Master C
                                            await UpdateMaster("C" + changed_Id);
                                        }
                                    }
                                    else
                                    {
                                        //Update Master E
                                        await UpdateMaster("E" + changed_Id);
                                    }
                                }
                                else
                                {
                                    //Update Master C
                                    await UpdateMaster("C" + changed_Id);
                                }
                            }
                        }
                    }
                }
                await MoveFile("CT04_Insert");
                await MoveFile("SQ01_ListMat");
                return "Success";
            }
            catch (Exception e)
            {
                //WriteLog("MyQuery2Macro : " + e.ToString());
                return e.ToString();
            }
        }
        public async static Task<string> MyQuery3Macro()
        {
            try
            {
                string scriptName = "";
                //Parameter
                var material = "";
                var status_ChgMatDesc = "";
                var tab_Id = "";
                var getId = "";
                var reason = "";



                DataSet dsspGetImpactmat = GetData("spGetImpactmat", "@Active", "X");
                var table = dsspGetImpactmat.Tables[0];
                if (table == null || table.Rows.Count == 0)
                {
                    return "No Data In spGetImpactmat";
                }
                DataTable dtImpactMatDesc = table.Clone();
                DataTable dtChangeMatClass = table.Clone();
                //Get Script
                foreach (DataRow row in table.Rows)
                {
                    if (!string.IsNullOrEmpty(row["New_Description"].ToString()))
                    {
                        if ((row["Status"].ToString() == "In process"))
                        {
                            scriptName = "MM02_ImpactMatDesc";
                            material = row["Material"].ToString();
                            getId = row["Id"].ToString();
                            dtImpactMatDesc.Rows.Add(row);
                        }
                    }
                    else
                    {
                        scriptName = "CLMM_ChangeMatClass";
                        material = row["Material"].ToString();
                        getId = row["Id"].ToString();
                        dtChangeMatClass.Rows.Add(row);
                    }
                }



                if (scriptName == "MM02_ImpactMatDesc")
                {
                    DataTable dtResultImpactMatDesc = await GetDataTableFromResult("MM02_ImpactMatDesc");
                    foreach (DataRow rowResult in dtResultImpactMatDesc.Rows)
                    {
                        var getListResult = getDataRow(rowResult);
                        foreach (DataRow rowMatDesc in table.Rows)
                        {
                            if (rowMatDesc["Material"].ToString() == getListResult[0])
                            {
                                var checkResult = getListResult[getListResult.Count - 1];
                                if (checkResult.Contains("Material") || checkResult.Contains("No changes made"))
                                {
                                    status_ChgMatDesc = "Completed";
                                    tab_Id = rowMatDesc["Id"].ToString();
                                    reason = "";
                                    await SaveImpactedMatDesc(tab_Id, reason, status_ChgMatDesc);
                                }
                                else
                                {
                                    status_ChgMatDesc = "Failed";
                                }
                            }
                        }
                    }
                }
                //CLMM
                else if (scriptName == "CLMM_ChangeMatClass")
                {
                    List<string> lstFileName = await GetAllFileName("CLMM_ChangeMatClass");
                    bool isFoundData = false;
                    foreach (string fileName in lstFileName)
                    {
                        if (isFoundData) { break; }
                        DataTable dtResultChangeMatClass = await GetDataTableFromResult(fileName);
                        foreach (DataRow rowResultMatClass in dtResultChangeMatClass.Rows)
                        {
                            //Match With MM02_ImpactMatDesc
                            if (!string.IsNullOrEmpty(rowResultMatClass[0].ToString()))
                            {
                                isFoundData = true;
                                foreach (DataRow rowChangeMatClass in table.Rows)
                                {
                                    if (!string.IsNullOrEmpty(rowChangeMatClass["Material"].ToString()) && rowChangeMatClass["Material"].ToString() == rowResultMatClass[0].ToString())
                                    {
                                        var checkResult = rowResultMatClass[0].ToString();
                                        if (checkResult.Contains("Material") || checkResult.Contains("No message is returned from SAP"))
                                        {
                                            status_ChgMatDesc = "Completed";
                                            tab_Id = rowChangeMatClass["Id"].ToString();
                                            reason = "";
                                            await SaveImpactedMatDesc(tab_Id, reason, status_ChgMatDesc);
                                        }
                                        else
                                        {
                                            status_ChgMatDesc = "Failed";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }



                await MoveFile("MM02_ImpactMatDesc");
                await MoveFile("CLMM_ChangeMatClass");

                return "success";
            }
            catch (Exception ex)
            {
                //WriteLog("MyQuery3Macro : " + ex.ToString());
                return ex.ToString();
            }
        }
        #endregion
        public static string ReadMM01Result(bool statusup, string mat, string material, DataTable dtResultExtensionPlant)
        {
            if (dtResultExtensionPlant != null)
            {
                foreach (DataRow rowExtensionPlant in dtResultExtensionPlant.Rows)
                {
                    //Split Data With ||
                    if (rowExtensionPlant != null && rowExtensionPlant[0] != null && !statusup)
                    {
                        var getListResult = getDataRow(rowExtensionPlant);
                        //Col Material Number RMMG1-MATNR
                        if (getListResult.Count > 0 && mat == getListResult[1])
                        {
                            //Col Result
                            string colResult = getListResult[getListResult.Count - 1];
                            //if (colResult.Contains("Material " + material)) 
                            return colResult;
                        }
                    }
                }
            }
            return "";
        }
        public static void WriteLog(string message)
        {
            string logFile = ConfigurationManager.AppSettings["LogFile"];
            string filePath = logFile + "example.txt";

            string textToWrite = DateTime.Now.ToString() + message + Environment.NewLine;

            using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                writer.WriteLine(textToWrite);
            }

            Console.WriteLine("Text written to file successfully.");
        }
        public static List<string> getDataRow(DataRow row)
        {
            List<string> getListResult = new List<string>();
            //Split Data With ||
            if (row != null && row[0] != null)
            {
                //Col Material Number RMMG1-MATNR
                var dataInRow = row[0].ToString();
                if (!string.IsNullOrEmpty(dataInRow))
                {
                    getListResult = dataInRow.Split(new string[] { "|" }, StringSplitOptions.None).ToList();
                    if (getListResult != null && getListResult.Count > 0)
                    {
                        return getListResult;
                    }
                }
            }
            return getListResult;
        }
        public static List<FileInfo> ListAllFile(string filePathResult)
        {
            //Get Result
            DirectoryInfo d = new DirectoryInfo(filePathResult);

            FileInfo[] Files = d.GetFiles("*.csv");
            return Files.ToList();
        }
        public static async Task<string> MoveFile(string fileName)
        {
            List<string> getlstFullFileName = await GetAllFileName(fileName);
            if (getlstFullFileName.Count > 0)
            {
                foreach (var getFullFileName in getlstFullFileName)
                {
                    if (getFullFileName != null)
                    {
                        string filePathResult = ConfigurationManager.AppSettings["FilePathResult"];
                        string destinationPathResult = ConfigurationManager.AppSettings["DestinationPathResult"];
                        string sourceFilePath = filePathResult + getFullFileName;
                        string destinationFilePath = destinationPathResult + getFullFileName;

                        try
                        {
                            //Delete File If Redundant
                            DirectoryInfo d = new DirectoryInfo(destinationPathResult);
                            FileInfo[] Files = d.GetFiles("*.csv");

                            var fileResult = Files.Where(x => x.Name.Contains(getFullFileName)).OrderByDescending(x => x.CreationTime).FirstOrDefault();
                            if (fileResult != null)
                            {
                                File.Delete(destinationFilePath);
                            }
                            // Move the file
                            File.Move(sourceFilePath, destinationFilePath);
                        }

                        catch (IOException ex)
                        {
                            return ex.ToString();
                        }
                    }
                    else
                    {
                        return "File Not Found";
                    }
                }
            }
            else
            {
                return "File Not Found";
            }
            return "File Not Found";
        }
        public static DataSet GetMasCharacteristic(string materialType)
        {
            //select* from MasCharacteristic where MaterialType like '@" + VBA.Mid(.Cells(irow, "B").value, 2, 1) + "@')#a order by Id"
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("select* from MasCharacteristic where MaterialType like VALUES(@materialType) order by Id;", con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@materialType", materialType);
                    cmd.Connection = con;
                    con.Open();
                    DataSet oDataset = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(oDataset);
                    con.Close();
                    return oDataset;
                }
            }
        }
        private async static Task<DataTable> GetDataTableFromFile(FileInfo fileResult)
        {
            string filePathResult = fileResult.DirectoryName;
            string fileNameResult = "";
            if (fileResult != null)
            {
                fileNameResult = fileResult.Name;
            }
            DataTable dtResult = ConvertCSVtoDataTable(filePathResult + fileNameResult);
            return dtResult;
        }
        private async static Task<string> GetFileName(string name)
        {
            string filePathResult = ConfigurationManager.AppSettings["FilePathResult"];
            //Get Result
            DirectoryInfo d = new DirectoryInfo(filePathResult);

            FileInfo[] Files = d.GetFiles("*.csv");
            string str = "";

            var fileResult = Files.Where(x => x.Name.Contains(name)).OrderByDescending(x => x.CreationTime).FirstOrDefault();
            string fileNameResult = "";
            if (fileResult != null)
            {
                fileNameResult = fileResult.Name;
            }
            return fileNameResult;
        }

        private async static Task<List<string>> GetAllFileName(string name)
        {
            List<DataTable> dtResult = new List<DataTable>();
            string filePathResult = ConfigurationManager.AppSettings["FilePathResult"];
            //Get Result
            DirectoryInfo d = new DirectoryInfo(filePathResult);

            FileInfo[] Files = d.GetFiles("*.csv");
            string str = "";

            var fileResult = Files.Where(x => x.Name.Contains(name)).OrderByDescending(x => x.CreationTime).Select(x => x.Name).ToList();
            return fileResult;
        }
        private async static Task<DataTable> GetDataTableFromResult(string name)
        {
            string filePathResult = ConfigurationManager.AppSettings["FilePathResult"];
            string fileNameResult = await GetFileName(name);
            if (!string.IsNullOrEmpty(fileNameResult))
            {
                DataTable dtResult = ConvertCSVtoDataTable(filePathResult + fileNameResult);
                return dtResult;
            }
            return new DataTable();
        }

        private async static Task<string> SaveImpactedMatDesc(string str_TabId, string reason, string status_ChgMatDesc)
        {
            string servicePathUrl = ConfigurationManager.AppSettings["ServicePathUrl"];
            string apiResponse = "";
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(servicePathUrl + "saveImpactedMatDesc?Id=" + str_TabId + "&Reason=" + reason + "&Status=" + status_ChgMatDesc + ""))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                        WriteLog("MyQuery3Macro : SaveImpactedMatDesc : " + apiResponse);
                    }
                }
            }
            return apiResponse;
        }

        private async static Task<string> UpdateMaster(string changeId)
        {
            string servicePathUrl = ConfigurationManager.AppSettings["ServicePathUrl"];
            string apiResponse = "";
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(servicePathUrl + "SendEmailUpdateMaster?_name=" + changeId))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                        WriteLog("MyQuery2Macro : UpdateMaster : " + apiResponse);
                    }
                }
            }
            return apiResponse;
        }


        private async static Task<string> updateDB(string name, string filename)
        {
            string servicePathUrl = ConfigurationManager.AppSettings["ServicePathUrl"];
            string apiResponse = "";
            using (var httpClient = new HttpClient())
            {

                using (var response = await httpClient.GetAsync(servicePathUrl + "SendEmail3NewVer?_name=" + name + "&filename=" + filename))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                        WriteLog("MyQueryMacro : updateDB : " + apiResponse);
                    }
                }
            }
            return apiResponse;
        }

        #endregion


        #region IMPORT INTERFACES
        public static string Import_CT04_I(string file, string InterfaceCode)
        {
            try
            {
                using (DataTable dt = ConvertCSVtoDataTable(file))
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string Condition = row[0].ToString().Replace("I", "Insert");
                            string Name = row[1].ToString();
                            string Value = row[2].ToString();
                            string Description = row[3].ToString();
                            string AppId = row[4].ToString();
                            string Result = row[5].ToString();

                            //1.Update to db
                            DataTable dtGetMail = UpdateToDB("spInterface_Igrid", AppId, InterfaceCode, dt);

                            //2.get data from db to dataTable prepare sent email to user
                            string from = ConfigurationManager.AppSettings["SMTPFrom"];
                            string to = GetCommaSeparatedEmails(dtGetMail);
                            string subject = InterfaceCode + " - Characteristic master is maintained in SAP " + "[" + Condition + "]";
                            string body = "[" + Condition + "]-" + " Characteristic master: " + Name + ", Value: " + Value + ", Description: " + Description + ", Result: " + Result.Replace("changed", "Insert in SAP completed") + ".";
                            //string AttachedFile = ""; 

                            //3.sent email to user
                            if (bool.Parse(ConfigurationManager.AppSettings["EmailsNotifySuccessImport" + InterfaceCode]) == true)
                            {
                                SendEmail(from, to, subject, body);
                            }

                            //4.send email to IT //5.sent email insert log 
                            if (bool.Parse(ConfigurationManager.AppSettings["ITEmailsNotifySuccessImport"]) == true)
                            {
                                SendEmail(from, ConfigurationManager.AppSettings["ITEmailsNotify"], subject, body);
                                SendToLog(from, to, subject, body);
                            }
                        }
                    }
                }
                if (File.Exists(file))
                {
                    File.Move(file, ConfigurationManager.AppSettings["InterfacePathInbound"] + @"Processed\" + Path.GetFileName(file));
                }
                return InterfaceCode + " Success";
            }
            catch (IOException e)
            {
                return InterfaceCode + e.Message;
            }
        }
        public static string Import_SQ01_L(string file, string InterfaceCode)
        {
            try
            {
                using (DataTable dt = ConvertCSVtoDataTable(file))
                {
                    if (dt.Rows.Count > 0)
                    {
                        string AppId = "0";
                        DataTable dtGetMail = UpdateToDB("spInterface_Igrid", AppId, InterfaceCode, dt);
                        string from = ConfigurationManager.AppSettings["SMTPFrom"];
                        string to = ConfigurationManager.AppSettings["ITEmailsNotify"];
                        string subject = InterfaceCode + " - SQ01 List Material is done";
                        string body = "SQ01 List Material is done";
                        //string AttachedFile = "";                        

                        if (bool.Parse(ConfigurationManager.AppSettings["ITEmailsNotifySuccessImport"]) == true)
                        {
                            SendEmail(from, ConfigurationManager.AppSettings["ITEmailsNotify"], subject, body);
                            SendToLog(from, to, subject, body);
                        }
                    }
                }
                if (File.Exists(file))
                {
                    File.Move(file, ConfigurationManager.AppSettings["InterfacePathInbound"] + @"Processed\" + Path.GetFileName(file));
                }
                return InterfaceCode + " Success";
            }
            catch (IOException e)
            {
                return InterfaceCode + e.Message;
            }
        }
        public static string Import_CT04_R(string file, string InterfaceCode)
        {
            try
            {
                using (DataTable dt = ConvertCSVtoDataTable(file))
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string Condition = row[0].ToString().Replace("D", "Remove");
                            string Name = row[1].ToString();
                            string Value = row[2].ToString();
                            string AppId = row[3].ToString();
                            string Result = row[4].ToString();

                            //1.Update to db
                            DataTable dtGetMail = UpdateToDB("spInterface_Igrid", AppId, InterfaceCode, dt);

                            //2.get data from db to dataTable prepare sent email to user
                            string from = ConfigurationManager.AppSettings["SMTPFrom"];
                            string to = "";
                            foreach (DataRow dr in dtGetMail.Rows)  //get email from db
                            {
                                to = dr["Email"].ToString();
                            }
                            string subject = InterfaceCode + " - Characteristic master is maintained in SAP " + "[" + Condition + "]";
                            string body = "[" + Condition + "]-" + " Characteristic master: " + Name + ", Value: " + Value + ", Result: " + Result + ".";
                            //string AttachedFile = "";

                            //3.sent email to user
                            if (bool.Parse(ConfigurationManager.AppSettings["EmailsNotifySuccessImport" + InterfaceCode]) == true)
                            {
                                SendEmail(from, to, subject, body);
                            }

                            //4.send email to IT //5.sent email insert log 
                            if (bool.Parse(ConfigurationManager.AppSettings["ITEmailsNotifySuccessImport"]) == true)
                            {
                                SendEmail(from, ConfigurationManager.AppSettings["ITEmailsNotify"], subject, body);
                                SendToLog(from, to, subject, body);
                            }
                        }
                    }
                }
                if (File.Exists(file))
                {
                    File.Move(file, ConfigurationManager.AppSettings["InterfacePathInbound"] + @"Processed\" + Path.GetFileName(file));
                }
                return InterfaceCode + " Success";
            }
            catch (IOException e)
            {
                return InterfaceCode + e.Message + e.StackTrace;
            }

        }

        public static string Import_MM02_I(string file, string InterfaceCode)
        {
            try
            {
                using (DataTable dt = ConvertCSVtoDataTableWithPipe(file))
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string MatNumber = row[0].ToString();
                            string MatDesc = row[1].ToString();
                            string AppId = row[2].ToString();
                            string Result = row[3].ToString();

                            //1.Update to db
                            DataTable dtGetMail = UpdateToDB("spInterface_Igrid", AppId, InterfaceCode, dt);

                            //2.get data from db to dataTable prepare sent email to user
                            string from = ConfigurationManager.AppSettings["SMTPFrom"];
                            string to = "";
                            foreach (DataRow dr in dtGetMail.Rows)  //get email from db
                            {
                                to = dr["Email"].ToString();
                            }

                            string subject = InterfaceCode + " - Material " + "[" + MatNumber + "] Saving changes to assignments Assignment changed";
                            string body = " Material: " + MatNumber + ", Description: " + MatDesc + ", Result: " + Result + ".";
                            //string AttachedFile = "";

                            //3.sent email to user
                            if (bool.Parse(ConfigurationManager.AppSettings["EmailsNotifySuccessImport" + InterfaceCode]) == true)
                            {
                                SendEmail(from, to, subject, body);
                            }

                            //4.send email to IT //5.sent email insert log 
                            if (bool.Parse(ConfigurationManager.AppSettings["ITEmailsNotifySuccessImport"]) == true)
                            {
                                SendEmail(from, ConfigurationManager.AppSettings["ITEmailsNotify"], subject, body);
                                SendToLog(from, to, subject, body);
                            }
                        }
                    }
                }
                if (File.Exists(file))
                {
                    File.Move(file, ConfigurationManager.AppSettings["InterfacePathInbound"] + @"Processed\" + Path.GetFileName(file));
                }
                return InterfaceCode + " Success";
            }
            catch (IOException e)
            {
                return InterfaceCode + e.Message + e.StackTrace;
            }
        }
        public static string Import_CLMM_C(string file, string InterfaceCode)
        {
            try
            {
                using (DataTable dt = ConvertCSVtoDataTable(file))
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string MatNumber = row[0].ToString();
                            string ClassNum = row[1].ToString();
                            string LoopIdColumn = row[2].ToString();
                            string Name = row[3].ToString();
                            string Value = row[4].ToString();
                            string AppId = row[5].ToString();
                            string Result = row[6].ToString();

                            if (MatNumber != "" && ClassNum != "" && LoopIdColumn == "H" && AppId != "" && Result != "")
                            {
                                //1.Update to db
                                DataTable dtGetMail = UpdateToDB("spInterface_Igrid", AppId, InterfaceCode, dt);

                                //2.get data from db to dataTable prepare sent email to user
                                string from = ConfigurationManager.AppSettings["SMTPFrom"];
                                string to = "";
                                foreach (DataRow dr in dtGetMail.Rows)  //get email from db
                                {
                                    to = dr["Email"].ToString();
                                }
                                string subject = InterfaceCode + " - Characteristic master is maintained in SAP " + "[" + MatNumber + "]";
                                string body = " Material: " + MatNumber + ", Characteristic master: " + Name + ", Value: " + Value + ", Result: " + Result + ".";
                                //string AttachedFile = "";

                                //3.sent email to user
                                if (bool.Parse(ConfigurationManager.AppSettings["EmailsNotifySuccessImport" + InterfaceCode]) == true)
                                {
                                    SendEmail(from, to, subject, body);
                                }

                                //4.send email to IT //5.sent email insert log 
                                if (bool.Parse(ConfigurationManager.AppSettings["ITEmailsNotifySuccessImport"]) == true)
                                {
                                    SendEmail(from, ConfigurationManager.AppSettings["ITEmailsNotify"], subject, body);
                                    SendToLog(from, to, subject, body);
                                }
                            }
                        }
                    }
                }
                if (File.Exists(file))
                {
                    File.Move(file, ConfigurationManager.AppSettings["InterfacePathInbound"] + @"Processed\" + Path.GetFileName(file));
                }
                return InterfaceCode + " Success";
            }
            catch (IOException e)
            {
                return InterfaceCode + e.Message + e.StackTrace;
            }
        }

        public static string Import_MM01_C(string file, string InterfaceCode)
        {
            try
            {
                using (DataTable dt = ConvertCSVtoDataTableWithPipe(file))
                {
                    if (dt.Rows.Count > 0)
                    {
                        string previousDocNum = null;
                        foreach (DataRow row in dt.Rows)
                        {
                            string DocNum = row[0].ToString();
                            string MatNum = row[1].ToString();
                            string MatDesc = row[2].ToString();
                            string MatRef = row[3].ToString();
                            string Plant = row[4].ToString();
                            string Org = row[5].ToString();
                            string Distribution = row[6].ToString();
                            string AppId = row[7].ToString();
                            string Result = row[8].ToString();

                            var currentDocNum = DocNum;
                            if (currentDocNum != null)
                            {
                                if (previousDocNum != currentDocNum)
                                {

                                    if (DocNum != "" && MatNum != "" && MatDesc != "" && Plant != "" && Distribution != "" && AppId != "" && Result != "")
                                    {
                                        //1.Update to db
                                        DataTable dtGetMail = UpdateToDB("spInterface_Igrid", AppId, InterfaceCode, dt);

                                        //2.get data from db to dataTable prepare sent email to user
                                        string from = ConfigurationManager.AppSettings["SMTPFrom"];
                                        string to = GetCommaSeparatedEmails(dtGetMail);

                                        string subject = InterfaceCode + " - System is created no : " + MatNum + " / " + MatDesc + " - [Create Material]";
                                        string body = CreateEmailBody(to, ".", subject, from, dt.Select("AppId=" + AppId).CopyToDataTable());
                                        //string AttachedFile = "";

                                        //3.sent email to user
                                        if (bool.Parse(ConfigurationManager.AppSettings["EmailsNotifySuccessImport" + InterfaceCode]) == true)
                                        {
                                            SendEmail(from, to, subject, body);
                                        }

                                        //4.send email to IT //5.sent email insert log 
                                        if (bool.Parse(ConfigurationManager.AppSettings["ITEmailsNotifySuccessImport"]) == true)
                                        {
                                            SendEmail(from, ConfigurationManager.AppSettings["ITEmailsNotify"], subject, body);
                                            SendToLog(from, to, subject, body);
                                        }

                                        //5.sent to artwork
                                        OutboundArtwork(DocNum);
                                    }
                                }
                            }
                            previousDocNum = currentDocNum.ToString();
                        }
                    }
                }
                if (File.Exists(file))
                {
                    File.Move(file, ConfigurationManager.AppSettings["InterfacePathInbound"] + @"Processed\" + Path.GetFileName(file));
                }
                return InterfaceCode + " Success";
            }
            catch (IOException e)
            {
                return InterfaceCode + e.Message + e.StackTrace;
            }
        }
        public static string Import_BAPI_U(string file, string InterfaceCode)
        {
            try
            {
                using (DataTable dt = ConvertCSVtoDataTable(file))
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string MatNumber = row[0].ToString();
                            string ClassNum = row[1].ToString();
                            string LoopIdColumn = row[2].ToString();
                            string Name = row[3].ToString();
                            string Value = row[4].ToString();
                            string AppId = row[5].ToString();
                            string Result = row[6].ToString();

                            if (MatNumber != "" && ClassNum != "" && LoopIdColumn == "H" && AppId != "" && Result != "")
                            {

                                //1.Update to db
                                DataTable dtGetMail = UpdateToDB("spInterface_Igrid", AppId, InterfaceCode, dt);

                                //2.get data from db to dataTable prepare sent email to user
                                string from = ConfigurationManager.AppSettings["SMTPFrom"];
                                string to = "";
                                foreach (DataRow dr in dtGetMail.Rows)  //get email from db
                                {
                                    to = dr["Email"].ToString();
                                }

                                string subject = InterfaceCode + " - Material " + "[" + MatNumber + "] " + Result;
                                string body = " Material: " + MatNumber + ", ClassNum: " + ClassNum + ", Result: " + Result + ".";
                                //string AttachedFile = "";

                                //3.sent email to user
                                if (bool.Parse(ConfigurationManager.AppSettings["EmailsNotifySuccessImport" + InterfaceCode]) == true)
                                {
                                    SendEmail(from, to, subject, body);
                                }

                                //4.send email to IT //5.sent email insert log 
                                if (bool.Parse(ConfigurationManager.AppSettings["ITEmailsNotifySuccessImport"]) == true)
                                {
                                    SendEmail(from, ConfigurationManager.AppSettings["ITEmailsNotify"], subject, body);
                                    SendToLog(from, to, subject, body);
                                }
                            }
                        }
                    }
                }

                if (File.Exists(file))
                {
                    File.Move(file, ConfigurationManager.AppSettings["InterfacePathInbound"] + @"Processed\" + Path.GetFileName(file));
                }
                return InterfaceCode + " Success";
            }
            catch (IOException e)
            {
                return InterfaceCode + e.Message + e.StackTrace;
            }
        }
        public static string Import_BAPI_B(string file, string InterfaceCode)
        {
            try
            {
                using (DataTable dt = ConvertCSVtoDataTable(file))
                {
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string MatNumber = row[0].ToString();
                            string ClassNum = row[1].ToString();
                            string LoopIdColumn = row[2].ToString();
                            string Name = row[3].ToString();
                            string Value = row[4].ToString();
                            string AppId = row[5].ToString();
                            string Result = row[6].ToString();

                            if (MatNumber != "" && ClassNum != "" && LoopIdColumn == "H" && AppId != "" && Result != "")
                            {

                                //1.Update to db
                                //DataTable dtGetMail = UpdateToDB("spInterface_Igrid", AppId, InterfaceCode, dt);

                                //2.get data from db to dataTable prepare sent email to user
                                string from = ConfigurationManager.AppSettings["SMTPFrom"];
                                string to = "";
                                //foreach (DataRow dr in dtGetMail.Rows)  //get email from db
                                //{
                                //    to = dr["Email"].ToString();
                                //}

                                string subject = InterfaceCode + " - Material " + "[" + MatNumber + "] " + Result;
                                string body = " Material: " + MatNumber + ", ClassNum: " + ClassNum + ", Result: " + Result + ".";
                                //string AttachedFile = "";

                                //3.sent email to user
                                if (bool.Parse(ConfigurationManager.AppSettings["EmailsNotifySuccessImport" + InterfaceCode]) == true)
                                {
                                    //SendEmail(from, to, subject, body);
                                }

                                //4.send email to IT //5.sent email insert log 
                                if (bool.Parse(ConfigurationManager.AppSettings["ITEmailsNotifySuccessImport"]) == true)
                                {
                                    to = ConfigurationManager.AppSettings["ITEmailsNotify"];
                                    SendEmail(from, to, subject, body);
                                    SendToLog(from, to, subject, body);
                                }
                            }
                        }
                    }
                }

                if (File.Exists(file))
                {
                    File.Move(file, ConfigurationManager.AppSettings["InterfacePathInbound"] + @"Processed\" + Path.GetFileName(file));
                }
                return InterfaceCode + " Success";
            }
            catch (IOException e)
            {
                return InterfaceCode + e.Message + e.StackTrace;
            }
        }
        static async Task OutboundArtwork(string keys)
        {
            try
            {
                var handler = new HttpClientHandler();
                handler.Credentials = new System.Net.NetworkCredential(@"thaiunion\service.webbase", "Tuna@40wb*");
                HttpClient client = new HttpClient(handler);
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("http://192.168.1.170:8888/ServiceCS.asmx/OutboundArtwork?Keys=" + keys);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response received: {responseBody}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine($"Message :{e.Message} ");
            }
        }

        #endregion

        #region EXPORT INTERFACES
        public static void CT04(DataTable Results)
        {

            DataTable dtCT04Insert = new DataTable();

            //Cancel CT04Update
            //Cancel CT04Remove
            //DataTable dtCT04Update = new DataTable();
            //DataTable dtCT04Remove = new DataTable();

            dtCT04Insert.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
                new DataColumn(@"Characteristic Name RCTAV-ATNAM"),
                new DataColumn(@"Characteristic Value CAWN-ATWRT(01)"),
                new DataColumn(@"Characteristic value description CAWNT-ATWTB(01)"),
                new DataColumn(@"AppId"),
            });
            //dtCT04Update.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
            //    new DataColumn(@"Characteristic Name RCTAV-ATNAM"),
            //    new DataColumn(@"Characteristic Value CAWN-ATWRT(01)"),
            //    new DataColumn(@"Text for a table entry CLHP-CR_STATUS_TEXT"),
            //    new DataColumn(@"Characteristic value description CAWNT-ATWTB(01)"),
            //});
            //dtCT04Remove.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
            //    new DataColumn(@"Characteristic Name RCTAV-ATNAM"),
            //    new DataColumn(@"Characteristic Value CAWN-ATWRT(01)"),
            //    new DataColumn(@"Text for a table entry CLHP-CR_STATUS_TEXT"),
            //    new DataColumn(@"AppId"),
            //});
            foreach (DataRow row in Results.Rows)
            {
                var ifColumn = "I";
                if (row["Changed_Action"].ToString() != "Insert" && row["Changed_Action"].ToString() != "Update")
                {
                    ifColumn = "U";
                }
                dtCT04Insert.Rows.Add(string.Format("{0}", ifColumn),
                            string.Format("{0}", row["Changed_Charname"].ToString()),
                            string.Format("{0}", row["id"].ToString()),
                            string.Format("{0}", row["Description"].ToString()),
                            string.Format("{0}", row["Changed_Id"].ToString()));

                //switch (row["Changed_Action"].ToString())
                //{
                //    case "Insert":
                //        dtCT04Insert.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString().Replace("Insert", "I")),
                //            string.Format("{0}", row["Changed_Charname"].ToString()),
                //            string.Format("{0}", row["id"].ToString()),
                //            string.Format("{0}", row["Description"].ToString()),
                //            string.Format("{0}", row["Changed_Id"].ToString()));
                //        break;
                //    //case "Update":
                //    //    dtCT04Update.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString()),
                //    //       string.Format("{0}", row["Changed_Charname"].ToString()),
                //    //       string.Format("{0}", row["id"].ToString()),
                //    //       string.Format("{0}", row["Old_Description"].ToString()),
                //    //       string.Format("{0}", row["Description"].ToString()));
                //    //    break;
                //    //case "Remove":
                //    //    dtCT04Remove.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString().Replace("Remove", "D")),
                //    //       string.Format("{0}", row["Changed_Charname"].ToString()),
                //    //       string.Format("{0}", row["Description"].ToString()),
                //    //        string.Format("{0}", row["Changed_Id"].ToString()));
                //    //    break;
                //    default:
                //        break;
                //}
            }
            if (dtCT04Insert.Rows.Count > 0)
            {
                string file = ConfigurationManager.AppSettings["InterfacePathOutbound"] + "CT04_Insert_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                ToCSV(dtCT04Insert, file);
            }
            //if (dtCT04Update.Rows.Count > 0)
            //{
            //    string file = @"D:\SAPInterfaces\Outbound\CT04_Update_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
            //    ToCSV(dtCT04Update, file);
            //}
            //if (dtCT04Remove.Rows.Count > 0)
            //{
            //    string file = ConfigurationManager.AppSettings["InterfacePathOutbound"] + "CT04_Remove_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
            //    ToCSV(dtCT04Remove, file);
            //}
        }
        public static void SQ01_ListMAT(DataTable Results)
        {
            DataTable dtListMat = new DataTable();
            dtListMat.Columns.AddRange(new DataColumn[]
            {
                new DataColumn (@"Characteristic Name txtSP$00005-LOW.text"),
                new DataColumn(@"txtSP$00003 - LOW.text"),
                new DataColumn(@"AppId"),
            });
            foreach (DataRow row in Results.Rows)
            {
                if ((row["Changed_Action"].ToString() != "Insert" || row["Changed_Action"].ToString() != "Remove") && !string.IsNullOrEmpty(row["Old_Description"].ToString()))
                {
                    dtListMat.Rows.Add(
                    string.Format("{0}", row["Changed_Charname"].ToString()),
                    string.Format("{0}", row["Old_Description"].ToString()),
                    string.Format("{0}", row["Changed_Id"].ToString()));
                }
            }
            if (dtListMat.Rows.Count > 0)
            {
                string file = ConfigurationManager.AppSettings["InterfacePathOutbound"] + "SQ01_ListMat" + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                ToCSV(dtListMat, file);
            }
            //Best Edit 28-09-2024
            //Add data to list
            //List<SQ01_OUTBOUND> lstData = new List<SQ01_OUTBOUND>();
            //foreach (DataRow row in Results.Rows)
            //{
            //    if ((row["Changed_Action"].ToString() != "Insert" || row["Changed_Action"].ToString() != "Remove") && !string.IsNullOrEmpty(row["Old_Description"].ToString()))
            //    {
            //        lstData.Add(new SQ01_OUTBOUND
            //        {
            //            APPID = string.Format("{0}", row["Changed_Id"].ToString()),
            //            CHARACTERISTIC_NAME = string.Format("{0}", row["Changed_Charname"].ToString()),
            //            TXTSP00003 = string.Format("{0}", row["Old_Description"].ToString())
            //        });
            //    }
            //}
            ////Add List to seperate AppId
            //List<string> lstAppID = new List<string>();
            //lstAppID = lstData.Select(x => x.APPID).ToList();
            //foreach (string appID in lstAppID)
            //{
            //    //create table to each appID
            //    DataTable dtListMatFilterData = new DataTable();
            //    dtListMatFilterData.Columns.AddRange(new DataColumn[]
            //    {
            //        new DataColumn (@"Characteristic Name txtSP$00005-LOW.text"),
            //        new DataColumn(@"txtSP$00003 - LOW.text"),
            //        new DataColumn(@"AppId")
            //    });
            //    var lstFilter = lstData.Where(x => x.APPID == appID).ToList();
            //    foreach (SQ01_OUTBOUND row in lstFilter)
            //    {
            //        dtListMatFilterData.Rows.Add(
            //            string.Format("{0}", row.CHARACTERISTIC_NAME),
            //            string.Format("{0}", row.TXTSP00003),
            //            string.Format("{0}", row.APPID));
            //    }
            //    if (dtListMatFilterData.Rows.Count > 0)
            //    {
            //        //create excel each appID
            //        string file = ConfigurationManager.AppSettings["InterfacePathOutbound"] + "SQ01_ListMat_" + appID + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
            //        ToCSV(dtListMatFilterData, file);
            //    }
            //}
        }

        public static void MM01_CreateMAT_ExtensionPlant(DataTable Results)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn (@"IfColumn"),
                new DataColumn(@"Material Number RMMG1-MATNR"),
                new DataColumn(@"Material Description (Short Text) MAKT-MAKTX"),
                new DataColumn(@"Reference material RMMG1_REF-MATNR"),
                new DataColumn(@"IfColumn and Plant RMMG1-WERKS and Reference plant RMMG1_REF-WERKS"),
                new DataColumn(@"IfColumn and Sales Organization RMMG1-VKORG and Reference sales organization RMMG1_REF-VKORG"),
                new DataColumn(@"Distribution Channel RMMG1-VTWEG and Reference distribution channel RMMG1_REF-VTWEG"),
                new DataColumn(@"AppId"),
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
                            string.Format("{0}", row["Ref"].ToString().Trim()),
                            string.Format("{0}", s.ToString().Trim()),
                            string.Format("{0}", row["Plant"]).Substring(0, 3),
                            string.Format("{0}", o),
                            string.Format("{0}", row["Id"])
                            );
                        }
                    }
                }
            }
            if (dt.Rows.Count > 0)
            {
                string file = ConfigurationManager.AppSettings["InterfacePathOutbound"] + "MM01_CreateMAT_ExtensionPlant_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                ToCSVWithPipe(dt, file);
            }
        }
        public static void BAPI_UpdateMATCharacteristics(DataTable Results)
        {
            DataTable dtClass = new DataTable();
            dtClass.Columns.AddRange(new DataColumn[] { new DataColumn(@"Material Number RMMG1-MATNR"),
                new DataColumn(@"CLASSNUM"),
                new DataColumn(@"Loop Id Column"),
                new DataColumn(@"Characteristic Name ALLOCVALUESCHARNEW-CHARACT"),
                new DataColumn(@"Characteristic Value ALLOCVALUESCHARNEW-VALUE_CHAR"),
                new DataColumn(@"AppId")
            });
            int i = 1;
            foreach (DataRow row in Results.Rows)
            {
                dtClass.Rows.Add(string.Format("{0}", row["Material"].ToString()),
                string.Format("{0}", row["ClassType"].ToString()),
                string.Format("{0}", "H"),
                string.Format("{0}", ""),
                string.Format("{0}", ""),
                string.Format("{0}", row["Id"].ToString())
                );
                DataTable dtCharacteristic = builditems(@"select * from MasCharacteristic where MaterialType  like '%" + row["Material"].ToString().Substring(1, 1) + "%' order by Id");
                foreach (DataRow dr in dtCharacteristic.Rows)
                {
                    string value = string.Format("{0}", dr["shortname"]);
                    if (dr["Single_Value"].ToString() == "X")
                    {
                        dtClass.Rows.Add(
                        string.Format("{0}", ""),
                        string.Format("{0}", ""),
                        string.Format("{0}", "D"),
                        string.Format("{0}", dr["Title"]),
                        string.Format("{0}", row[value]),
                        string.Format("{0}", dr["Id"])
                        );
                    }
                    else
                    {
                        string[] splitPlant = string.Format("{0}", row[value].ToString()).Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string pl in splitPlant)
                        {
                            dtClass.Rows.Add(
                            string.Format("{0}", ""),
                            string.Format("{0}", ""),
                            string.Format("{0}", "D"),
                            string.Format("{0}", dr["Title"]),
                            string.Format("{0}", pl),
                            string.Format("{0}", dr["Id"])
                            );
                        }
                    }
                }
                if (dtClass.Rows.Count > 0)
                {
                    string file = ConfigurationManager.AppSettings["InterfacePathOutbound"] + "BAPI_UpdateMATCharacteristics_" + DateTime.Now.ToString("yyyyMMddhhmm") + "_" + i + ".csv";
                    ToCSV(dtClass, file);
                }
                dtClass.Clear();
                i++;
            }
        }
        public static void BAPI_BATCHCLASS(DataTable Results)
        {
            DataTable dtClass = new DataTable();
            dtClass.Columns.AddRange(new DataColumn[] { new DataColumn(@"Material Number RMMG1-MATNR"),
                new DataColumn(@"CLASSNUM"),
                new DataColumn(@"Loop Id Column"),
                new DataColumn(@"Characteristic Name ALLOCVALUESCHARNEW-CHARACT"),
                new DataColumn(@"Characteristic Value ALLOCVALUESCHARNEW-VALUE_CHAR"),
                new DataColumn(@"AppId")
            });
            int i = 1;
            foreach (DataRow row in Results.Rows)
            {
                dtClass.Rows.Add(string.Format("{0}", row["Material"].ToString()),
                string.Format("{0}", row["BatchClass"].ToString()),
                string.Format("{0}", "H"),
                string.Format("{0}", ""),
                string.Format("{0}", ""),
                string.Format("{0}", row["Id"].ToString())
                );

                if (dtClass.Rows.Count > 0)
                {
                    string file = ConfigurationManager.AppSettings["InterfacePathOutbound"] + "BAPI_BATCHCLASS_" + DateTime.Now.ToString("yyyyMMddhhmm") + "_" + i + ".csv";
                    ToCSV(dtClass, file);
                }
                dtClass.Clear();
                i++;
            }
        }

        public static void MM02_ImpactMatDesc(DataTable Results)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn (@"Material Number RMMG1 - MATNR"),
                new DataColumn(@"Material description MAKT - MAKTX"),
                new DataColumn(@"AppId"),});
            foreach (DataRow row in Results.Rows)
            {
                dt.Rows.Add(
                string.Format("{0}", row["Material"].ToString()),
                string.Format("{0}", row["Description"].ToString()),
                string.Format("{0}", row["Id"].ToString()));
            }
            if (dt.Rows.Count > 0)
            {
                string file = ConfigurationManager.AppSettings["InterfacePathOutbound"] + "MM02_ImpactMatDesc" + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
                ToCSVWithPipe(dt, file);
            }
        }
        public static void CLMM_ChangeMatClass(DataTable Results)
        {
            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]
            {
                new DataColumn(@"Table cell -TextField txtG_TARGET_TAB - OBJECT"),
                new DataColumn (@"Char.Name ctxtG_CHAR_TAB - ATNAM"),
                new DataColumn(@"Loop Id Column"),
                new DataColumn(@"Characteristic Name ALLOCVALUESCHARNEW-CHARACT"),
                new DataColumn(@"New Value ctxtG_CHAR_TAB - NEWATWRT"),
                new DataColumn(@"AppId"),
             });
            int i = 1;
            foreach (DataRow row in Results.Rows)
            {
                dt.Rows.Add(string.Format("{0}", row["Material"].ToString()),
                string.Format("{0}", row["ClassType"].ToString()),
                string.Format("{0}", "H"),
                string.Format("{0}", ""),
                string.Format("{0}", ""),
                string.Format("{0}", row["Id"].ToString())
                );
                DataTable dtCharacteristic = builditems(@"select * from MasCharacteristic where MaterialType  like '%" + row["Material"].ToString().Substring(1, 1) + "%' order by Id");
                foreach (DataRow dr in dtCharacteristic.Rows)
                {
                    string value, shortname;
                    if (dr["Title"].ToString() == row["Char_Name"].ToString())
                    {
                        value = string.Format("{0}", row["Char_NewValue"]);
                    }
                    else
                    {
                        shortname = string.Format("{0}", dr["shortname"]);
                        value = string.Format("{0}", row[shortname]);
                    }
                    if (dr["Single_Value"].ToString() == "X")
                    {
                        dt.Rows.Add(
                        string.Format("{0}", ""),
                        string.Format("{0}", ""),
                        string.Format("{0}", "D"),
                        string.Format("{0}", dr["Title"]),
                        string.Format("{0}", value),
                        string.Format("{0}", dr["Id"])
                        );


                    }
                    else
                    {
                        string[] splitPlant = string.Format("{0}", value).Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string pl in splitPlant)
                        {
                            dt.Rows.Add(
                            string.Format("{0}", ""),
                            string.Format("{0}", ""),
                            string.Format("{0}", "D"),
                            string.Format("{0}", dr["Title"]),
                            string.Format("{0}", pl),
                            string.Format("{0}", dr["Id"])
                            );
                        }
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    string file = ConfigurationManager.AppSettings["InterfacePathOutbound"] + "CLMM_ChangeMatClass_" + DateTime.Now.ToString("yyyyMMddhhmm") + "_" + i + ".csv";
                    ToCSV(dt, file);
                }
                dt.Clear();
                i++;
            }
        }
        #endregion

        #region Util METHODS
        public static DataSet GetData(string sp, string field, string sName)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = sp;
                cmd.Parameters.AddWithValue(field, sName);
                cmd.Connection = con;
                con.Open();
                DataSet oDataset = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(oDataset);
                con.Close();
                return oDataset;
            }
        }
        public static DataTable builditems(string data)
        {
            using (SqlConnection oConn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                oConn.Open();
                string strQuery = data;
                DataTable dt = new DataTable();
                SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, oConn);
                oAdapter.Fill(dt);
                oConn.Close();
                oConn.Dispose();
                return dt;
            }
        }
        public static string ReadItems(string strQuery)
        {
            string result = "";
            // (ByVal FieldName As String, ByVal TableName As String, ByVal Cur As String, ByVal Value As String) As String
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
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
        public static void ToCSVWithPipe(DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false, new UTF8Encoding(true));
            //headers
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write("|");
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
                        sw.Write("|");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
        public static DataTable ConvertCSVtoDataTableWithPipe(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split('|');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split('|');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
        public static void UpdateToDB(string sp, string AppId, string InterfaceCode)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = sp;
                cmd.Parameters.AddWithValue("@AppId", AppId);
                cmd.Parameters.AddWithValue("@InterfaceCode", InterfaceCode);
                cmd.Connection = con;
                con.Open();
                cmd.ExecuteReader();
                con.Close();
            }
        }
        public static DataTable UpdateToDB(string sp, string AppId, string InterfaceCode, DataTable dt)
        {
            DataTable dt2 = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = sp;
                cmd.Parameters.AddWithValue("@AppId", AppId);
                cmd.Parameters.AddWithValue("@InterfaceCode", InterfaceCode);
                cmd.Connection = con;
                con.Open();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt2);
                con.Close();
            }
            return dt2;
        }
        public static DataSet GetDataToEmail(string sp, string AppId, string InterfaceCode)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = sp;
                cmd.Parameters.AddWithValue("@AppId", AppId);
                cmd.Parameters.AddWithValue("@InterfaceCode", InterfaceCode);
                cmd.Connection = con;
                con.Open();
                DataSet oDataset = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(oDataset);
                con.Close();
                return oDataset;
            }
        }

        public static void SendEmail(string from, string to, string subject, string body)
        {

            using (MailMessage mailMsg = new MailMessage())
            {
                mailMsg.From = new MailAddress(from);
                var testMail = ConfigurationManager.AppSettings["MailTest"];
                if (testMail == "true")
                {
                    to = "thana.sinukit@iamconsulting.co.th;thanapong.tong-u-thai@iamconsulting.co.th";
                }
                string[] tos = to.Split(';');
                foreach (string s in tos)
                {
                    mailMsg.To.Add(new MailAddress(s.Trim()));
                }
                mailMsg.Subject = ConfigurationManager.AppSettings["EnvironmentName"] + "-" + subject;
                mailMsg.Body = body;
                mailMsg.IsBodyHtml = true;

                var smtp = new SmtpClient
                {
                    Host = ConfigurationManager.AppSettings["SMTPServer"],
                    Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]),
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Timeout = 20000
                };
                smtp.Send(mailMsg);
            }
        }

        public static void SendEmail(string MailTo, string MailCc, string _Body, string _Subject, string _Attachments)
        {

            //MailSettings.SMTPServer = Convert.ToString(ConfigurationManager.AppSettings["HostName"]);
            //MailMessage Msg = new MailMessage();
            //// Sender e-mail address.
            //Msg.From = new MailAddress("pqr@gmail.com");
            //// Recipient e-mail address.
            //Msg.To.Add("abc@gmail.com");
            //Msg.CC.Add("zcd@gmail.com");
            //Msg.Subject = "Timesheet Payment Instruction updated";
            //Msg.IsBodyHtml = true;
            //Msg.Body = emailMessage.ToString();
            //NetworkCredential loginInfo = new NetworkCredential(Convert.ToString(ConfigurationManager.AppSettings["UserName"]), Convert.ToString(ConfigurationManager.AppSettings["Password"])); // password for connection smtp if u dont have have then pass blank

            //SmtpClient smtp = new SmtpClient();
            //smtp.UseDefaultCredentials = true;
            //smtp.Credentials = loginInfo;
            ////smtp.EnableSsl = true;
            ////No need for port
            ////smtp.Host = ConfigurationManager.AppSettings["HostName"];
            ////smtp.Port = int.Parse(ConfigurationManager.AppSettings["PortNumber"]);
            //smtp.Send(Msg);

            //---
            //int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
            //SmtpClient sc = new SmtpClient(ConfigurationManager.AppSettings["SMTPServer"], Port);
            //sc.UseDefaultCredentials = false;

            //bool IsUseSSL = false;
            //var SMTPSSL = ConfigurationManager.AppSettings["SMTPSSL"];
            //if (SMTPSSL.ToUpper().Trim() == "TRUE")
            //{
            //    IsUseSSL = true;
            //}
            //EnableSsl = IsUseSSL;

            //-----
            var fromAddress = new MailAddress("kriengkrai.ritthaphrom@thaiunion.com", "My Name");
            var toAddress = new MailAddress("kriengkrai.ritthaphrom@thaiunion.com", "Mr Test");
            const string fromPassword = "";
            const string subject = "test";
            const string body = "HEY, LISTEN!";

            var smtp = new SmtpClient
            {

                Host = string.Format("{0}", ConfigurationManager.AppSettings["SMTPServer"]),
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                //Attachment attachment = new Attachment(filePath);
                //message.Attachments.Add(attachment);

                smtp.Send(message);
            }


            //    MailMessage msg = new MailMessage();
            //    SmtpClient smtp = new SmtpClient();
            //    if (string.IsNullOrEmpty(MailTo)) return;
            //    string[] words = MailTo.Split(';');
            //    foreach (string word in words)
            //    {
            //        if (!string.IsNullOrEmpty(word))
            //            msg.To.Add(new MailAddress(word));
            //    }
            //    List<string> myList = new List<string>();
            //    string[] c = MailCc.Split(';');
            //    foreach (string s in c)
            //        if (!string.IsNullOrEmpty(s))
            //        {
            //            msg.CC.Add(new MailAddress(s));
            //            myList.Add(s);
            //        }
            //    msg.From = new MailAddress("wshuttleadm@thaiunion.com");
            //    msg.Subject = string.Format("{0}", _Subject);
            //    msg.Body = _Body;
            //    if (!string.IsNullOrEmpty(_Attachments))
            //    {
            //        msg.Attachments.Add(new Attachment(_Attachments));
            //    }
            //    msg.IsBodyHtml = true;

            //    smtp.Host = string.Format("{0}", ConfigurationManager.AppSettings["SMTPServer"]);
            //    smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]);
            //    smtp.UseDefaultCredentials = false;
            //    smtp.Send(msg);
            //    smtp.Dispose();

            //    //insert to maildata
            string ReternMsg = SendEmailInsertLog(MailTo, MailCc, _Body, _Subject);



        }
        public static string SendToLog(string from, string to, string subject, string body)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Insert into MailData values(@Sender,@To,@Cc,'',@Subject,@Body,getdate(),1,getdate(),'TEXT',1,0)";
                cmd.Parameters.AddWithValue("@Sender", String.Format("{0}", 10));
                cmd.Parameters.AddWithValue("@To", to.ToString());
                cmd.Parameters.AddWithValue("@Cc", "");
                cmd.Parameters.AddWithValue("@Subject", subject.ToString());
                cmd.Parameters.AddWithValue("@Body", body.ToString());
                cmd.Connection = con;
                con.Open();
                var getValue = cmd.ExecuteScalar();
                con.Close();
                return ((string)getValue == null) ? string.Empty : getValue.ToString();
            }
        }
        public static string SendEmailInsertLog(string MailTo, string MailCc, string _Body, string _Subject)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
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
        public static void SendEmail(string _name)
        {
            //string datapath = "~/FileTest/" + _name;
            string _email = "";
            //string _Material = "";
            string _Description = "";
            string _Body = "";
            string _Attached = "";

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spalertemail";

                cmd.Connection = con;
                con.Open();
                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                oAdapter.Fill(dt);
                con.Close();
            }
            foreach (DataRow dr in dt.Rows)
            {
                _email = dr["Email"].ToString();
                //_Material = dr["Material"].ToString();

                _Description = dr["Description"].ToString();
                _Body = dr["body"].ToString();
                _Attached = dr["attached"].ToString();
            }
            MailMessage msg = new MailMessage();
            string[] words = _email.Split(';');
            foreach (string word in words)
            {
                msg.To.Add(new MailAddress(word));
                //Console.WriteLine(word);
            }
            //msg.To.Add(new MailAddress(_email));
            msg.From = new MailAddress("wshuttleadm@thaiunion.com");
            msg.Subject = "System SEC PKG Template is created No. : ";
            //msg.Body = "Material  " + _Material.ToString() + " Created";
            msg.Body = _Body;
            //msg.Attachments.Add(new System.Net.Mail.Attachment(_Attached));
            msg.IsBodyHtml = true;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            //client.Credentials = new System.Net.NetworkCredential("wshuttleadm@thaiunion.com", "WSP@ss2018");
            client.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
            client.Host = "192.168.1.39";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = false;
            try
            {
                client.Send(msg);

            }
            catch (Exception ex)
            {
                ex.ToString();
            }
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
        public static void Jobalertemail(string data)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "spalertemail";
                cmd.Connection = con;
                con.Open();
                //cmd.ExecuteNonQuery();
                var getValue = cmd.ExecuteScalar();
                if (getValue.ToString() == "0")
                {
                    MailMessage msg = new MailMessage();
                    msg.To.Add(new MailAddress("kriengkrai.ritthaphrom@thaiunion.com"));
                    List<string> li = new List<string>();
                    li.Add("mes.support@thaiunion.com");
                    msg.CC.Add(string.Join<string>(",", li)); // Sending CC  
                    msg.From = new MailAddress("wshuttleadm@thaiunion.com");
                    msg.Subject = "[iGrid Support] Winshuttle down";
                    msg.Body = "Dear All <br/>job Winshuttle fail.";
                    msg.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient();
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential("wshuttleadm@thaiunion.com", "WSP@ss2018");
                    client.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
                    client.Host = "smtp.office365.com";
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = true;
                    client.Send(msg);
                }
                con.Close();
            }
            Console.Write("success");
        }
        public static string ReplaceHeader(string header)
        {
            switch (header)
            {
                case "IfColumn":
                    header = header.Replace("IfColumn", "Action");
                    break;
                case "Material Number RMMG1-MATNR":
                    header = header.Replace("Material Number RMMG1-MATNR", "MatNum");
                    break;
                case "Material Description (Short Text) MAKT-MAKTX":
                    header = header.Replace("Material Description (Short Text) MAKT-MAKTX", "MatDesc");
                    break;
                case "Reference material RMMG1_REF-MATNR":
                    header = header.Replace("Reference material RMMG1_REF-MATNR", "RefMat");
                    break;
                case "IfColumn and Plant RMMG1-WERKS and Reference plant RMMG1_REF-WERKS":
                    header = header.Replace("IfColumn and Plant RMMG1-WERKS and Reference plant RMMG1_REF-WERKS", "Plant");
                    break;
                case "IfColumn and Sales Organization RMMG1-VKORG and Reference sales organization RMMG1_REF-VKORG":
                    header = header.Replace("IfColumn and Sales Organization RMMG1-VKORG and Reference sales organization RMMG1_REF-VKORG", "Sales Org");
                    break;
                case "Distribution Channel RMMG1-VTWEG and Reference distribution channel RMMG1_REF-VTWEG":
                    header = header.Replace("Distribution Channel RMMG1-VTWEG and Reference distribution channel RMMG1_REF-VTWEG", "Dist Channel");
                    break;
                default:
                    // Handle other cases if necessary
                    break;
            }
            return header;
        }
        public static string CreateEmailBody(string user, string title, string message, string system_name, DataTable dt)
        {
            string body = string.Empty;
            string header = string.Empty;
            string row = string.Empty;
            foreach (DataColumn item in dt.Columns)
            {
                header += "<th>" + ReplaceHeader(item.ColumnName) + "</th>";
            }
            foreach (DataRow myRow in dt.Rows)
            {
                row += "<tr>";
                foreach (DataColumn myColumn in dt.Columns)
                {
                    row += "<td>" + myRow[myColumn.ColumnName].ToString() + "</td>";
                }
                row += "</tr>";
            }
            var EmailTemplatepath = Directory.GetFiles(ConfigurationManager.AppSettings["EmailTemplatepath"], "HTMLTemplate.html");
            string EmailTemplate = EmailTemplatepath[0];
            using (StreamReader sr = new StreamReader(EmailTemplate))
            {
                body = sr.ReadToEnd();
            }
            body = body.Replace("{user}", user);
            body = body.Replace("{title}", title);
            body = body.Replace("{message}", message);
            body = body.Replace("{header}", header);
            body = body.Replace("{row}", row);
            body = body.Replace("{system_name}", system_name);
            return body;
        }
        public static string GetCommaSeparatedEmails(DataTable dtGetMail)
        {
            StringBuilder to = new StringBuilder();
            foreach (DataRow dr in dtGetMail.Rows)  // get email from db
            {
                if (to.Length > 0)
                {
                    to.Append(", ");
                }
                to.Append(dr["Email"].ToString());
            }
            return to.ToString();
        }

        #endregion

        #region Temp METHODS
        public static void CT04_Inbound(string data)
        {
            var dir = @"D:\SAPInterfaces\Inbound";
            var filePaths = Directory.GetFiles(dir, "CT04*_Result*.csv");
            //foreach (string s in filePaths)
            //{
            //    using (var reader = new StreamReader(s))
            //    {
            //        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            //        {
            //            while (csv.Read())
            //            {//This will advance the reader to the next record.

            //                //You can use an indexer to get by position or name. 
            //                //This will return the field as a string

            //                // By position
            //                var field = csv[0];
            //                //var AppID = csv["AppID"];
            //                // By header name
            //                //csv.Read();
            //                csv.ReadHeader();
            //                string[] headerRow = csv.Context.Reader.HeaderRecord;
            //                string[] filteredValues = Array.FindAll(headerRow, x => x.Contains("Result"));
            //                var Condition = "";// csv["Condition TypeRV13A-KSCHL"];
            //                if (filteredValues.Length > 0)
            //                    if (csv["Result"] == "Condition records saved")
            //                    {
            //                        string Changed_Id = "";
            //                        string Changed_Action = "";
            //                        string Material = "";
            //                        string Description = "";
            //                        string DMSNo = "";
            //                        string New_Material = "";
            //                        string New_Description = "";
            //                        string Status = "";
            //                        string Reason = "";
            //                        string NewMat_JobId = "";
            //                        string Char_Name = "";
            //                        string Char_OldValue = "";
            //                        string Char_NewValue = "";
            //                        //SendEmailUpdateMaster("U" + Material);
            //                        using (SqlConnection con = new SqlConnection(strConn))
            //                        {
            //                            SqlCommand cmd = new SqlCommand();
            //                            cmd.CommandType = CommandType.StoredProcedure;
            //                            cmd.CommandText = "spUpdateImpactedmat";

            //                            cmd.Parameters.AddWithValue("@Changed_Id", Changed_Id);
            //                            cmd.Parameters.AddWithValue("@Changed_Action", Changed_Action);
            //                            cmd.Parameters.AddWithValue("@Material", Material);
            //                            cmd.Parameters.AddWithValue("@Description", Description);
            //                            cmd.Parameters.AddWithValue("@DMSNo", DMSNo);
            //                            cmd.Parameters.AddWithValue("@New_Material", New_Material);
            //                            cmd.Parameters.AddWithValue("@New_Description", New_Description);
            //                            cmd.Parameters.AddWithValue("@Status", Status);
            //                            cmd.Parameters.AddWithValue("@Reason", Reason);
            //                            cmd.Parameters.AddWithValue("@NewMat_JobId", NewMat_JobId);
            //                            cmd.Parameters.AddWithValue("@Char_Name", Char_Name);
            //                            cmd.Parameters.AddWithValue("@Char_OldValue", Char_OldValue);
            //                            cmd.Parameters.AddWithValue("@Char_NewValue", Char_NewValue);


            //                            cmd.Connection = con;
            //                            con.Open();
            //                            DataTable dtResult = new DataTable();
            //                            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            //                            oAdapter.Fill(dtResult);
            //                            con.Close();
            //                        }
            //                        //    using (SqlConnection con = new SqlConnection(strConn))
            //                        //    {
            //                        //        SqlCommand cmd = new SqlCommand();
            //                        //        cmd.CommandType = CommandType.StoredProcedure;
            //                        //        cmd.CommandText = "spUpdateQuotationfromJob";
            //                        //        cmd.Parameters.AddWithValue("@MinPrice", string.Format("{0}", 0));
            //                        //        cmd.Parameters.AddWithValue("@subID", string.Format("{0}", Condition.ToString() == "zpm1" ? "0" : AppID.ToString()));
            //                        //        cmd.Parameters.AddWithValue("@ID", AppID.ToString());
            //                        //        cmd.Connection = con;
            //                        //        con.Open();
            //                        //        cmd.ExecuteNonQuery();
            //                        //        con.Close();
            //                        //    }
            //                        //}
            //                    }
            //                //var records = csv.GetRecords<dynamic>();
            //                //foreach (var item in records)
            //                //{
            //                //    var da = item.Result;
            //                //    var condition = item[@"Condition Type RV13A-KSCHL"];  
            //                //}
            //                //foreach (dynamic record in records.ToList())
            //                //{
            //                //    var data = record["IfColumn"];
            //                //}
            //            }
            //        }
            //    }
            //    try
            //    {
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
            // Move MM01 & BAPI_UpdateMATCharacteristics
            filePaths = Directory.GetFiles(dir, "MM01*_Result*.csv");
            foreach (string s in filePaths)
            {
                try
                {
                    using (var reader = new StreamReader(s))
                    {
                        //using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                        //{
                        //    while (csv.Read())
                        //    {//This will advance the reader to the next record.

                        //        //You can use an indexer to get by position or name. 
                        //        //This will return the field as a string

                        //        // By position
                        //        var field = csv[0];
                        //        //var AppID = csv["AppID"];
                        //        // By header name
                        //        //csv.Read();
                        //        csv.ReadHeader();
                        //        string[] headerRow = csv.Context.Reader.HeaderRecord;
                        //        string[] filteredValues = Array.FindAll(headerRow, x => x.Contains("Result"));
                        //        var Condition = "";// csv["Condition TypeRV13A-KSCHL"];
                        //        if (filteredValues.Length > 0)
                        //            if (csv["Result"] == "Condition records saved")
                        //            {
                        //            }
                        //    }
                        //}
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
            //foreach (string s in filePaths)
            //{
            //    using (var reader = new StreamReader(s))
            //    {
            //        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            //        {
            //            while (csv.Read())
            //            {//This will advance the reader to the next record.

            //                //You can use an indexer to get by position or name. 
            //                //This will return the field as a string

            //                // By position
            //                var field = csv[0];
            //                //var AppID = csv["AppID"];
            //                // By header name
            //                //csv.Read();
            //                csv.ReadHeader();
            //                string[] headerRow = csv.Context.Reader.HeaderRecord;
            //                string[] filteredValues = Array.FindAll(headerRow, x => x.Contains("Result"));
            //                var Condition = "";// csv["Condition TypeRV13A-KSCHL"];
            //                if (filteredValues.Length > 0)
            //                    if (csv["Result"] == "Condition records saved")
            //                    {
            //                        string Changed_Id = "";
            //                        string Changed_Action = "";
            //                        string Material = "";
            //                        string Description = "";
            //                        string DMSNo = "";
            //                        string New_Material = "";
            //                        string New_Description = "";
            //                        string Status = "";
            //                        string Reason = "";
            //                        string NewMat_JobId = "";
            //                        string Char_Name = "";
            //                        string Char_OldValue = "";
            //                        string Char_NewValue = "";
            //                        //SendEmailUpdateMaster("U" + Material);
            //                        using (SqlConnection con = new SqlConnection(strConn))
            //                        {
            //                            SqlCommand cmd = new SqlCommand();
            //                            cmd.CommandType = CommandType.StoredProcedure;
            //                            cmd.CommandText = "spUpdateImpactedmat";

            //                            cmd.Parameters.AddWithValue("@Changed_Id", Changed_Id);
            //                            cmd.Parameters.AddWithValue("@Changed_Action", Changed_Action);
            //                            cmd.Parameters.AddWithValue("@Material", Material);
            //                            cmd.Parameters.AddWithValue("@Description", Description);
            //                            cmd.Parameters.AddWithValue("@DMSNo", DMSNo);
            //                            cmd.Parameters.AddWithValue("@New_Material", New_Material);
            //                            cmd.Parameters.AddWithValue("@New_Description", New_Description);
            //                            cmd.Parameters.AddWithValue("@Status", Status);
            //                            cmd.Parameters.AddWithValue("@Reason", Reason);
            //                            cmd.Parameters.AddWithValue("@NewMat_JobId", NewMat_JobId);
            //                            cmd.Parameters.AddWithValue("@Char_Name", Char_Name);
            //                            cmd.Parameters.AddWithValue("@Char_OldValue", Char_OldValue);
            //                            cmd.Parameters.AddWithValue("@Char_NewValue", Char_NewValue);


            //                            cmd.Connection = con;
            //                            con.Open();
            //                            DataTable dtResult = new DataTable();
            //                            SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
            //                            oAdapter.Fill(dtResult);
            //                            con.Close();
            //                        }
            //                        //    using (SqlConnection con = new SqlConnection(strConn))
            //                        //    {
            //                        //        SqlCommand cmd = new SqlCommand();
            //                        //        cmd.CommandType = CommandType.StoredProcedure;
            //                        //        cmd.CommandText = "spUpdateQuotationfromJob";
            //                        //        cmd.Parameters.AddWithValue("@MinPrice", string.Format("{0}", 0));
            //                        //        cmd.Parameters.AddWithValue("@subID", string.Format("{0}", Condition.ToString() == "zpm1" ? "0" : AppID.ToString()));
            //                        //        cmd.Parameters.AddWithValue("@ID", AppID.ToString());
            //                        //        cmd.Connection = con;
            //                        //        con.Open();
            //                        //        cmd.ExecuteNonQuery();
            //                        //        con.Close();
            //                        //    }
            //                        //}
            //                    }
            //                //var records = csv.GetRecords<dynamic>();
            //                //foreach (var item in records)
            //                //{
            //                //    var da = item.Result;
            //                //    var condition = item[@"Condition Type RV13A-KSCHL"];  
            //                //}
            //                //foreach (dynamic record in records.ToList())
            //                //{
            //                //    var data = record["IfColumn"];
            //                //}
            //            }
            //        }
            //    }
            //    try
            //    {
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
            // Move MM01 & BAPI_UpdateMATCharacteristics
            filePaths = Directory.GetFiles(dir, "MM01*_Result*.csv");
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
        public static void CLMM_ChangeMatClassold(DataTable Results)
        {

            //DataTable dt = new DataTable();
            //dt.Columns.AddRange(new DataColumn[] 
            //{ 
            //    new DataColumn (@"Char.Name ctxtG_CHAR_TAB - ATNAM"),
            //    new DataColumn(@"Old Value ctxtG_CHAR_TAB - OLDATWRT"),
            //    new DataColumn(@"New Value ctxtG_CHAR_TAB - NEWATWRT"),
            //    new DataColumn(@"Table cell -TextField txtG_TARGET_TAB - OBJECT"),
            // });
            //foreach (DataRow row in Results.Rows)
            //{
            //    dt.Rows.Add(
            //        string.Format("{0}", row["Char_Name"].ToString()),
            //        string.Format("{0}", row["Char_OldValue"].ToString()),
            //        string.Format("{0}", row["Char_NewValue"].ToString()),
            //        string.Format("{0}", row["Material"].ToString())
            //        //string.Format("{0}", row["Description"].ToString())
            //        );
            //}
            //if (dt.Rows.Count > 0)
            //{
            //    string file = InterfacePathOutbound + "CLMM_ChangeMatClass" + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
            //    ToCSV(dt, file);
            //}
        }
        public static void master_artwork(string[] name)
        {
            ////loop details
            //CHARACTERISTICS list = new CHARACTERISTICS();
            //List<CHARACTERISTIC> iGrid_CHARACTERISTICS = new List<CHARACTERISTIC>();
            //CHARACTERISTIC item = new CHARACTERISTIC();
            //item.NAME = name[0].ToString();
            //item.DESCRIPTION = name[1].ToString();
            //if (name[0].ToString() == "ZPKG_SEC_BRAND")
            //{
            //    item.VALUE = name[2].ToString();
            //}
            //else
            //    item.VALUE = name[1].ToString();

            //item.ID = name[2].ToString();
            //item.Old_ID = string.Format("{0}", name[4]);
            //item.Changed_Action = string.Format("{0}", name[3]);
            //iGrid_CHARACTERISTICS.Add(item);
            //list.Characteristics = iGrid_CHARACTERISTICS;
            ////MM72_OUTBOUND_MATERIAL_CHARACTERISTIC matNumber = new MM72_OUTBOUND_MATERIAL_CHARACTERISTIC();
            //SERVICE_RESULT_MODEL resp = new SERVICE_RESULT_MODEL();
            ////MM72Client client = new MM72Client();

            ////matNumber.param = list;
            //resp = MM_72_Hepler.SaveCharacteristics(list);
            ////++++++++++++++++++++++++++++++++

            //string datapath = "~/FileTest/master" + name[0].ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
            ////using (FileStream fs = new FileStream(Server.MapPath(datapath), FileMode.Create))
            ////{
            ////    new XmlSerializer(typeof(myService.CHARACTERISTICS)).Serialize(fs, list);
            ////}

            //string sendemailmaster = ConfigurationManager.AppSettings["SendEmailMaster"];
            //sendemail(sendemailmaster, "",
            //    string.Format("Name : {0}<br/>Status: {1},<br/>msg: {2}", name[0].ToString(), resp.status, resp.msg),
            //    string.Format("Master {0} is completely created in SAP and Artwork", name[1].ToString()), (!File.Exists(datapath)) ? "" : System.Web.HttpContext.Current.Server.MapPath(datapath));

        }
        public static string OutboundArtworkold(string Keys)
        {
            ////master_artwork();
            ////header
            //IGRID_OUTBOUND_MODEL iGrid_Model = new IGRID_OUTBOUND_MODEL();
            ////myh.OUTBOUND_HEADERS = new ServiceReference.IGRID_OUTBOUND_HEADER_MODEL;

            //IGRID_OUTBOUND_HEADER_MODEL result = new IGRID_OUTBOUND_HEADER_MODEL();
            //List<IGRID_OUTBOUND_HEADER_MODEL> iGrid_Header_List = new List<IGRID_OUTBOUND_HEADER_MODEL>();

            //IGRID_OUTBOUND_MODEL matNumber = new IGRID_OUTBOUND_MODEL();
            //SERVICE_RESULT_MODEL resp = new SERVICE_RESULT_MODEL();
            try
            {
                //    var _table = builditems("select *,case when statusapp=4 then 'Completed' when statusapp=5 then 'Canceled' end as 'Status' from SapMaterial Where DocumentNo='" + Keys + "'");
                //    string _ArtworkNumber = "", _Date = "", _Time = "", _Material = "", _PAUserName = "", _Subject = "Material {0} data send to Artwork Complete";

                //    foreach (DataRow dr in _table.Rows)
                //    {
                //        if (dr["StatusApp"].ToString() == "5")
                //            _Subject = "Cancel form iGrid and send info to Artwork Complete";
                //        _ArtworkNumber = string.Format("{0}", dr["DMSNo"]);
                //        _Date = String.Format("{0:yyyyMMdd}", dr["CreateOn"]);
                //        _Time = String.Format("{0:HH:mm:ss}", dr["CreateOn"]); //"10:22:03";
                //        _Material = string.Format("{0}", dr["Material"]);
                //        _PAUserName = string.Format("{0}", dr["CreateBy"]);


                //        DataTable _dt = builditems(@"select isnull(url,'')url,isnull(ReferenceMaterial,'')ReferenceMaterial from TransArtworkURL where Matdoc="
                //        + string.Format("{0}", dr["Id"]));
                //        if (_dt.Rows.Count > 0)
                //        {
                //            DataRow r = _dt.Rows[0];
                //            result.ArtworkURL = string.Format("{0}", r["url"]);//"http://artwork.thaiunion.com/content/aw-file.pdf";
                //            result.ReferenceMaterial = string.Format("{0}", r["ReferenceMaterial"]);
                //        }
                //        else
                //        {
                //            result.ArtworkURL = "";
                //            result.ReferenceMaterial = "";
                //        }
                //        result.ArtworkNumber = _ArtworkNumber;
                //        result.Date = _Date;
                //        result.Time = _Time; //"10:22:03";
                //        result.RecordType = "I";
                //        result.MaterialNumber = dr["StatusApp"].ToString() == "5" ? "" : string.Format("{0}", dr["Material"]);
                //        result.MaterialDescription = string.Format("{0}", dr["Description"]); //"CTN3 - 60960,LUCKY";
                //        result.ChangePoint = string.Format("{0}", dr["ChangePoint"]) == "C" ? "1" : "0";
                //        result.MaterialCreatedDate = String.Format("{0:yyyyMMdd}", dr["ModifyOn"]);
                //        result.Status = dr["Status"].ToString();
                //        result.PAUserName = string.Format("{0}", dr["CreateBy"]);
                //        result.PGUserName = string.Format("{0}", dr["Assignee"]);
                //        //            result.Plant = string.Format("{0}", dr["Plant"].ToString().Replace(';',','));
                //        result.Plant = string.Format("{0}", dr["Plant"].ToString());
                //        result.PrintingStyleofPrimary = string.Format("{0}", dr["PrintingStyleofPrimary"]);
                //        result.PrintingStyleofSecondary = string.Format("{0}", dr["PrintingStyleofSecondary"]);

                //        //string CustomerDesign = string.Format("{0}", dr["CustomerDesign"]);
                //        //string[] words = CustomerDesign.Split('|');
                //        result.CustomersDesign = splittext(dr["CustomerDesign"].ToString(), 0);
                //        result.CustomersDesignDetail = splittext(dr["CustomerDesign"].ToString(), 1);

                //        result.CustomersSpec = splittext(dr["CustomerSpec"].ToString(), 0);
                //        result.CustomersSpecDetail = CNService.splittext(dr["CustomerSpec"].ToString(), 1);
                //        result.CustomersSize = splittext(dr["CustomerSize"].ToString(), 0);
                //        result.CustomersSizeDetail = splittext(dr["CustomerSize"].ToString(), 1);
                //        result.CustomerNominatesVendor = splittext(dr["CustomerVendor"].ToString(), 0);
                //        result.CustomerNominatesVendorDetail = splittext(dr["CustomerVendor"].ToString(), 1);
                //        result.CustomerNominatesColorPantone = splittext(dr["CustomerColor"].ToString(), 0);
                //        result.CustomerNominatesColorPantoneDetail = splittext(dr["CustomerColor"].ToString(), 1);
                //        result.CustomersBarcodeScanable = splittext(dr["CustomerScanable"].ToString(), 0);
                //        result.CustomersBarcodeScanableDetail = splittext(dr["CustomerScanable"].ToString(), 1);
                //        result.CustomersBarcodeSpec = splittext(dr["CustomerBarcodeSpec"].ToString(), 0);
                //        result.CustomersBarcodeSpecDetail = splittext(dr["CustomerBarcodeSpec"].ToString(), 1);
                //        result.FirstInfoGroup = string.Format("{0}", dr["FirstInfoGroup"]);
                //        result.SONumber = string.Format("{0}", dr["SO"]);
                //        result.SOitem = "";
                //        result.SOPlant = string.Format("{0}", dr["SOPlant"]);
                //        result.PICMKT = string.Format("{0}", dr["PICMkt"]);
                //        result.Destination = string.Format("{0}", dr["Destination"]);
                //        result.RemarkNoteofPA = string.Format("{0}", dr["Remark"]);
                //        result.FinalInfoGroup = string.Format("{0}", dr["FinalInfoGroup"]);
                //        result.RemarkNoteofPG = "";
                //        result.CompleteInfoGroup = "";
                //        result.ProductionExpirydatesystem = "";
                //        result.Seriousnessofcolorprinting = "";
                //        result.CustIngreNutritionAnalysis = "";
                //        result.ShadeLimit = "";
                //        result.PackageQuantity = "";
                //        result.WastePercent = "";
                //        iGrid_Header_List.Add(result);
                //        iGrid_Model.OUTBOUND_HEADERS = iGrid_Header_List;
                //    }

                //    List<IGRID_OUTBOUND_ITEM_MODEL> iGrid_Item_List = new List<IGRID_OUTBOUND_ITEM_MODEL>();
                //    DataTable dt = new DataTable();
                //    using (SqlConnection con = new SqlConnection(strConn))
                //    {
                //        SqlCommand cmd = new SqlCommand();
                //        cmd.CommandType = CommandType.StoredProcedure;
                //        cmd.CommandText = "spInboundArtwork";
                //        cmd.Parameters.AddWithValue("@Keys", string.Format("{0}", Keys.ToString()));
                //        cmd.Connection = con;
                //        con.Open();
                //        SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
                //        oAdapter.Fill(dt);
                //        con.Close();
                //        List<InboundArtwork> _itemsArtwork = new List<InboundArtwork>();
                //        for (int i = 0; i < dt.Rows.Count; i++)
                //        {
                //            var detail = new IGRID_OUTBOUND_ITEM_MODEL();
                //            DataRow dr = dt.Rows[i];

                //            detail.ArtworkNumber = string.Format("{0}", _ArtworkNumber);
                //            detail.Date = _Date;
                //            detail.Time = _Time;
                //            detail.Characteristic = dr["cols"].ToString();
                //            //detail.Description = dr["Description"].ToString();
                //            //detail.Value = dr["value"].ToString();
                //            string[] splitHeader = dr["value"].ToString().Split(';');
                //            if (splitHeader != null && splitHeader.Length > 1)
                //                foreach (string word in splitHeader)
                //                {
                //                    detail = new IGRID_OUTBOUND_ITEM_MODEL();
                //                    detail.ArtworkNumber = string.Format("{0}", _ArtworkNumber);
                //                    detail.Date = _Date;
                //                    detail.Time = _Time;
                //                    detail.Characteristic = dr["cols"].ToString();
                //                    detail.Value = word.ToString();
                //                    detail.Description = detail.Value.ToString();
                //                    iGrid_Item_List.Add(detail);
                //                }
                //            else
                //            {
                //                detail.Description = dr["Description"].ToString();
                //                detail.Value = dr["value"].ToString();
                //                iGrid_Item_List.Add(detail);
                //            }
                //        }
                //    }
                //    iGrid_Model.OUTBOUND_ITEMS = iGrid_Item_List;
                //    //MM_73_Hepler.SaveMaterial client = new MM_73_Hepler.SaveMaterial();
                //    //matNumber.param = iGrid_Model;
                //    //resp = client.MATERIAL_NUMBER(matNumber);
                //    string Start = DateTime.Now.ToString();
                //    resp = MM_73_Hepler.SaveMaterial(iGrid_Model);
                //    string dtEnd = DateTime.Now.ToString();
                //    //Context.Response.Write(JsonConvert.SerializeObject(resp));
                //    //DirectoryInfo dir = new DirectoryInfo(@"\\SERVER\Data\");

                //    string datapath = @"\\192.168.1.170\FileTest\iGrid_Model" + Keys.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
                //    FileInfo myFile = new FileInfo(datapath);
                //    using (FileStream fs = new FileStream(datapath, FileMode.Create))
                //    {
                //        new XmlSerializer(typeof(IGRID_OUTBOUND_MODEL)).Serialize(fs, iGrid_Model);
                //    }
                //    Checkpath check1 = GetInfo(datapath);
                //    SqlParameter[] param = { new SqlParameter("@keys", string.Format("{0}", Keys)) };

                //    executeProcedure("spupdateOutbound", param);
                //    string MailCc = CNService.GetModulEmail(CNService.Getusermail("PA_Approve"));
                //    sendemail(Getuser(_PAUserName, "email"), MailCc,
                //        string.Format("Artwork Number {3} <br/> Workflow IGrid {0} <br/> Status: {1},<br/>msg: {2} <br/>Start Time: {4}<br/>End Time: {5}", Keys.ToString(), resp.status, resp.msg, _ArtworkNumber, Start, dtEnd),
                //        string.Format(_Subject, _Material.ToString()), datapath);
                //return resp.msg;
                return "";
            }
            catch (Exception e)
            {
                //sendemail("Nongrat.Jantarasuwan@thaiunion.com;Pornpimon.Bouban@thaiunion.com", "",
                //string.Format("{0}", e.Message), string.Format("iGrid can't send to Artwork , status fail , iGrid No : {0}", Keys.Substring(0, 16)), "");
                return e.Message;
                // Action after the exception is caught  
            }
        }
        #endregion

        //public static void GetmasterUpdateToCSV(DataTable Results)
        //{
        //    DataTable dt = new DataTable();
        //    dt.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
        //        new DataColumn(@"Characteristic Name RCTAV-ATNAM"),
        //        new DataColumn(@"Characteristic Value CAWN-ATWRT(01)"),
        //        new DataColumn(@"Text for a table entry CLHP-CR_STATUS_TEXT"),
        //        new DataColumn(@"Characteristic value description CAWNT-ATWTB(01)"),
        //    });
        //    DataTable listMat = new DataTable();
        //    listMat.Columns.AddRange(new DataColumn[] { new DataColumn (@"Characteristic Name txtSP$00005-LOW.text"),
        //    new DataColumn(@"txtSP$00003 - LOW.text"),
        //    });
        //    foreach (DataRow row in Results.Rows)
        //    {
        //        dt.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString()),
        //        string.Format("{0}", row["Changed_Charname"].ToString()),
        //        string.Format("{0}", row["id"].ToString()),
        //        string.Format("{0}", row["Old_Description"].ToString()),
        //        string.Format("{0}", row["Description"].ToString()));

        //        listMat.Rows.Add(string.Format("{0}", row["Changed_Charname"].ToString()),
        //        string.Format("{0}", row["Old_Description"].ToString()));
        //    }
        //    if (listMat.Rows.Count > 0)
        //    {
        //        string file = @"D:\SAPInterfaces\Outbound\SQ01_ListMat" + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
        //        CNService.ToCSV(listMat, file);
        //    }

        //    DataTable dtCT04Insert = new DataTable();
        //    DataTable dtCT04Update = new DataTable();
        //    DataTable dtCT04Remove = new DataTable();
        //    dtCT04Insert.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
        //        new DataColumn(@"Characteristic Name RCTAV-ATNAM"),
        //        new DataColumn(@"Characteristic Value CAWN-ATWRT(01)"),
        //        new DataColumn(@"Characteristic value description CAWNT-ATWTB(01)"),
        //    });
        //    dtCT04Update.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
        //        new DataColumn(@"Characteristic Name RCTAV-ATNAM"),
        //        new DataColumn(@"Characteristic Value CAWN-ATWRT(01)"),
        //        new DataColumn(@"Text for a table entry CLHP-CR_STATUS_TEXT"),
        //        new DataColumn(@"Characteristic value description CAWNT-ATWTB(01)"),
        //    });
        //    dtCT04Remove.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
        //        new DataColumn(@"Characteristic Name RCTAV-ATNAM"),
        //        new DataColumn(@"Characteristic Value CAWN-ATWRT(01)"),
        //        new DataColumn(@"Text for a table entry CLHP-CR_STATUS_TEXT"),                
        //    });
        //    foreach (DataRow row in Results.Rows)
        //    {
        //        switch (row["Changed_Action"].ToString())
        //        {
        //            case "Insert":
        //                dtCT04Insert.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString().Replace("Insert","I")),
        //                    string.Format("{0}", row["Changed_Charname"].ToString()),
        //                    string.Format("{0}", row["id"].ToString()),                           
        //                    string.Format("{0}", row["Description"].ToString()));
        //                break;
        //            case "Update":
        //                dtCT04Update.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString()),
        //                   string.Format("{0}", row["Changed_Charname"].ToString()),
        //                   string.Format("{0}", row["id"].ToString()),
        //                   string.Format("{0}", row["Old_Description"].ToString()),
        //                   string.Format("{0}", row["Description"].ToString()));
        //                break;
        //            case "Remove":
        //                dtCT04Remove.Rows.Add(string.Format("{0}", row["Changed_Action"].ToString().Replace("Remove", "D")),
        //                   string.Format("{0}", row["Changed_Charname"].ToString()),                           
        //                   string.Format("{0}", row["Description"].ToString()));
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    if (dtCT04Insert.Rows.Count > 0)
        //    {              
        //        string file = @"D:\SAPInterfaces\Outbound\CT04_Insert_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
        //        CNService.ToCSV(dtCT04Insert, file);
        //    }
        //    if (dtCT04Update.Rows.Count > 0)
        //    {
        //        string file = @"D:\SAPInterfaces\Outbound\CT04_Update_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
        //        CNService.ToCSV(dtCT04Update, file);ฉ
        //    }
        //    if (dtCT04Remove.Rows.Count > 0)
        //    {
        //        string file = @"D:\SAPInterfaces\Outbound\CT04_Remove_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
        //        CNService.ToCSV(dtCT04Remove, file);
        //    }


        //    //string[] ColumnsToBeDeleted = { "Insert", "Update", "Remove" };
        //    //if (dt.Rows.Count > 0)
        //    //    foreach (string ColName in ColumnsToBeDeleted)
        //    //    {
        //    //        foreach(DataRow row in dt.Rows)
        //    //        {
        //    //            row[0].ToString();
        //    //        }
        //    //         //DataRow["IfColumn"][1].ToString().Replace(ColName, "I");
        //    //        //dt.Rows.Add(dt.Rows[][0].ToString().Replace("Insert", "I")
        //    //            //);

        //    //        //dt.Rows.Add(row["DocumentNo"].ToString(),
        //    //        //    row["DocumentNo"].ToString()
        //    //        //    );

        //    //        //var dtclone = new DataTable();
        //    //        //if (dt.Select("IfColumn='" + ColName + "'").ToList().Count > 0)
        //    //        //{
        //    //        //    //if (ColName == "Update")
        //    //        //    //{
        //    //        //    //    dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
        //    //        //    //}
        //    //        //    //else if (ColName == "Insert")
        //    //        //    //{
        //    //        //    //    dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
        //    //        //    //    //dtclone = dt.Select("IfColumn='Insert'").CopyToDataTable();
        //    //        //    //    //dtclone.Columns[0]..ItemArray[0].ToString().Replace("a","I");
        //    //        //    //    //dtclone.Rows[0].ItemArray[0].ToString().Replace('Insert', 'I');
        //    //        //    //    //dtclone = dt.Rows[0][0].ToString().CopyToDataTable();
        //    //        //    //    dtclone.Rows[0][0].ToString().Replace(ColName, "I");
        //    //        //    //    dtclone.Columns.Remove(@"Text for a table entry CLHP-CR_STATUS_TEXT");
        //    //        //    //}
        //    //        //    //else if (ColName == "Remove")
        //    //        //    //{
        //    //        //    //    dtclone = dt.Select("IfColumn='" + ColName + "'").CopyToDataTable();
        //    //        //    //    dtclone.Columns.Remove(@"Characteristic Value CAWN-ATWRT(01)");
        //    //        //    //    dtclone.Columns.Remove(@"Characteristic value description CAWNT-ATWTB(01)");
        //    //        //    //}
        //    //        //    //string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/CT04_" + ColName + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv");
        //    //        //    string file = @"D:\SAPInterfaces\Outbound\CT04_" + ColName + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";

        //    //        //    CNService.ToCSV(dtclone, file);
        //    //        //}
        //    //    }


        //}
        //public static void GetImpactmat(string sName)
        //{
        //    //Char.Name ctxtG_CHAR_TAB - ATNAM[0, 0].text
        //    //Old Value ctxtG_CHAR_TAB - OLDATWRT[1, 0].text
        //    //New Value ctxtG_CHAR_TAB - NEWATWRT[2, 0].text
        //    //Table cell -TextField txtG_TARGET_TAB - OBJECT[0, 0].text
        //    using (SqlConnection con = new SqlConnection(CNService.strConn))
        //    {
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = "spGetImpactmat";
        //        cmd.Parameters.AddWithValue("@Active", sName);
        //        cmd.Connection = con;
        //        con.Open();
        //        DataSet oDataset = new DataSet();
        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        da.Fill(oDataset);
        //        con.Close();
        //        DataTable dt = new DataTable();
        //        dt.Columns.AddRange(new DataColumn[] { new DataColumn (@"Char.Name ctxtG_CHAR_TAB - ATNAM"),
        //        new DataColumn(@"Old Value ctxtG_CHAR_TAB - OLDATWRT"),
        //        new DataColumn(@"New Value ctxtG_CHAR_TAB - NEWATWRT"),
        //        new DataColumn(@"Table cell -TextField txtG_TARGET_TAB - OBJECT"),
        //        });

        //        DataTable dtImpactMatDesc = new DataTable(); 
        //        dtImpactMatDesc.Columns.AddRange(new DataColumn[] { new DataColumn (@"Material Number RMMG1 - MATNR"),
        //        new DataColumn(@"Material description MAKT - MAKTX"),});
        //       // foreach (DataRow row in oDataset.Tables[0].Rows)
        //       // {
        //       //     dt.Rows.Add(string.Format("{0}", row["Char_Name"].ToString()),
        //       //string.Format("{0}", row["Char_OldValue"].ToString()),
        //       //string.Format("{0}", row["Char_NewValue"].ToString()),
        //       //string.Format("{0}", row["Material"].ToString()),
        //       //string.Format("{0}", row["Description"].ToString()));


        //       //     dtImpactMatDesc.Rows.Add(
        //       //string.Format("{0}", row["Material"].ToString()),
        //       //string.Format("{0}", row["Description"].ToString()));
        //       // }
        //        //if (dt.Rows.Count > 0)
        //        //{
        //        //    string file = @"D:\SAPInterfaces\Outbound\CLMM_ChangeMatClass" + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
        //        //    CNService.ToCSV(dt, file);
        //        //}


        //        //if (dtImpactMatDesc.Rows.Count > 0)
        //        //{
        //        //    string file = @"D:\SAPInterfaces\Outbound\MM02_ImpactMatDesc" + "_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
        //        //    CNService.ToCSV(dtImpactMatDesc, file);
        //        //}
        //    }
        //}
        //public static void GetQuery(string sName)
        //{
        //    using (SqlConnection con = new SqlConnection(CNService.strConn))
        //    {
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = "spQuery";
        //        cmd.Parameters.AddWithValue("@Material", sName);
        //        cmd.Connection = con;
        //        con.Open();
        //        DataSet oDataset = new DataSet();
        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        da.Fill(oDataset);
        //        con.Close();
        //        GetQueryToSQL(oDataset.Tables[0]);
        //    }
        //}
        //public static void GetQueryToSQL(DataTable Results)
        //{

        //    DataTable dt = new DataTable();
        //    dt.Columns.AddRange(new DataColumn[] { new DataColumn (@"IfColumn"),
        //        new DataColumn(@"Material Number RMMG1-MATNR"),
        //    new DataColumn(@"Material Description (Short Text) MAKT-MAKTX"),
        //    new DataColumn(@"Reference material RMMG1_REF-MATNR"),
        //    new DataColumn(@"IfColumn and Plant RMMG1-WERKS and Reference plant RMMG1_REF-WERKS"),
        //    new DataColumn(@"IfColumn and Sales Organization RMMG1-VKORG and Reference sales organization RMMG1_REF-VKORG"),
        //    new DataColumn(@"Distribution Channel RMMG1-VTWEG and Reference distribution channel RMMG1_REF-VTWEG"),
        //    });
        //    foreach (DataRow row in Results.Rows)
        //    {
        //        string[] split = string.Format("{0}", row["Plant"].ToString()).Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        //        foreach (string s in split)
        //        {

        //            if (s.Trim() != "")
        //            {
        //                string[] splitSOOrg = "DM;EX".Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        //                foreach (string o in splitSOOrg)
        //                {
        //                    dt.Rows.Add(string.Format("{0}", row["DocumentNo"].ToString()),
        //        string.Format("{0}", row["Material"].ToString()),
        //        string.Format("{0}", row["Description"].ToString()),
        //        string.Format("{0}", row["Ref"].ToString()),
        //        string.Format("{0}", s),
        //        string.Format("{0}", row["Plant"]).Substring(0, 3),
        //        //string.Format("{0}", row["Plant"].ToString())

        //        string.Format("{0}", o)
        //        );
        //                }
        //            }
        //        }
        //    }
        //    if (dt.Rows.Count > 0)
        //    {
        //        string file = @"D:\SAPInterfaces\Outbound\MM01_CreateMAT_ExtensionPlant_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
        //        //string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/MM01_CreateMAT_ExtensionPlant_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv");
        //        CNService.ToCSV(dt, file);
        //    }
        //    //sale org
        //    //DataTable dtSaleOrg = new DataTable();
        //    //dtSaleOrg.Columns.AddRange(new DataColumn[] { new DataColumn(@"Material Number RMMG1-MATNR"),
        //    //    new DataColumn(@"Reference material RMMG1_REF-MATNR"),
        //    //new DataColumn(@"IfColumn and Sales Organization RMMG1-VKORG and Reference sales organization RMMG1_REF-VKORG"),
        //    //new DataColumn(@"Distribution Channel RMMG1-VTWEG and Reference distribution channel RMMG1_REF-VTWEG"),
        //    //});
        //    //DataTable dtSOOrg = new DataTable();
        //    //dtSOOrg.Columns.AddRange(new DataColumn[] { new DataColumn(@"SaleOrg") });
        //    //foreach (DataRow row in Results.Rows)
        //    //{
        //    //    string[] str = string.Format("{0}", row["Plant"].ToString()).Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        //    //    foreach (string s in str)
        //    //    {

        //    //        if (s.Trim() != "" && dtSOOrg.Select("SaleOrg='" + string.Format("{0}", s).Substring(0, 3) + "'").ToList().Count == 0)
        //    //        {
        //    //            dtSOOrg.Rows.Add(string.Format("{0}", s).Substring(0, 3));
        //    //        }
        //    //    }
        //    //    foreach (DataRow soorg in dtSOOrg.Rows)
        //    //    {
        //    //        string[] split = "DM;EX".Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        //    //        foreach (string s in split)
        //    //        {
        //    //            dtSaleOrg.Rows.Add(string.Format("{0}", row["Material"].ToString()),
        //    //        string.Format("{0}", row["Ref"].ToString()),
        //    //        string.Format("{0}", soorg["SaleOrg"].ToString()),
        //    //        //string.Format("{0}", row["Plant"].ToString())
        //    //        string.Format("{0}", s)
        //    //        );
        //    //        }
        //    //    }
        //    //    if (dtSaleOrg.Rows.Count > 0)
        //    //    {
        //    //        string file = @"D:\SAPInterfaces\Outbound\MM01_ExtendSaleOrg_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
        //    //        //string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/MM01_ExtendSaleOrg_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv");
        //    //        CNService.ToCSV(dtSaleOrg, file);
        //    //    }
        //    //}
        //    //Characteristics
        //    DataTable dtClass = new DataTable();
        //    dtClass.Columns.AddRange(new DataColumn[] { new DataColumn(@"Material Number RMMG1-MATNR"),
        //        new DataColumn(@"Loop Id Column"),
        //        new DataColumn(@"Characteristic Name ALLOCVALUESCHARNEW-CHARACT"),
        //        new DataColumn(@"Characteristic Value ALLOCVALUESCHARNEW-VALUE_CHAR") });
        //    foreach (DataRow row in Results.Rows)
        //    {
        //        dtClass.Rows.Add(string.Format("{0}", row["Material"].ToString()),
        //        string.Format("{0}", "H"),
        //        string.Format("{0}", ""),
        //        string.Format("{0}", "")
        //        );
        //        DataTable dtCharacteristic = CNService.builditems(@"select * from MasCharacteristic where MaterialType  like '%" +
        //            row["Material"].ToString().Substring(1, 1) + "%' order by Id");
        //        foreach (DataRow dr in dtCharacteristic.Rows)
        //        {
        //            string value = string.Format("{0}", dr["shortname"]);
        //            dtClass.Rows.Add(string.Format("{0}", ""),
        //            string.Format("{0}", "D"),
        //            string.Format("{0}", dr["Title"]),
        //            string.Format("{0}", row[value])
        //            );
        //        }
        //        if (dtClass.Rows.Count > 0)
        //        {
        //            string file = @"D:\SAPInterfaces\Outbound\BAPI_UpdateMATCharacteristics_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv";
        //            //string file = HttpContext.Current.Server.MapPath("~/ExcelFiles/BAPI_UpdateMATCharacteristics_" + DateTime.Now.ToString("yyyyMMddhhmm") + ".csv");
        //            CNService.ToCSV(dtClass, file);
        //        }
        //    }
        //}
        //public static void GetUpdateTOSQL(string data)
        //{
        //    var dir = @"D:\SAPInterfaces\Inbound";
        //    //HttpContext.Current.Server.MapPath("~/ExcelFiles");
        //    var filePaths = Directory.GetFiles(dir, "CT04*_Result*.csv");
        //    foreach (string s in filePaths)
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
        //                            string Changed_Id = "";
        //                            string Changed_Action = "";
        //                            string Material = "";
        //                            string Description = "";
        //                            string DMSNo = "";
        //                            string New_Material = "";
        //                            string New_Description = "";
        //                            string Status = "";
        //                            string Reason = "";
        //                            string NewMat_JobId = "";
        //                            string Char_Name = "";
        //                            string Char_OldValue = "";
        //                            string Char_NewValue = "";
        //                            SendEmailUpdateMaster("U" + Material);
        //                            using (SqlConnection con = new SqlConnection(CNService.strConn))
        //                            {
        //                                SqlCommand cmd = new SqlCommand();
        //                                cmd.CommandType = CommandType.StoredProcedure;
        //                                cmd.CommandText = "spUpdateImpactedmat";

        //                                cmd.Parameters.AddWithValue("@Changed_Id", Changed_Id);
        //                                cmd.Parameters.AddWithValue("@Changed_Action", Changed_Action);
        //                                cmd.Parameters.AddWithValue("@Material", Material);
        //                                cmd.Parameters.AddWithValue("@Description", Description);
        //                                cmd.Parameters.AddWithValue("@DMSNo", DMSNo);
        //                                cmd.Parameters.AddWithValue("@New_Material", New_Material);
        //                                cmd.Parameters.AddWithValue("@New_Description", New_Description);
        //                                cmd.Parameters.AddWithValue("@Status", Status);
        //                                cmd.Parameters.AddWithValue("@Reason", Reason);
        //                                cmd.Parameters.AddWithValue("@NewMat_JobId", NewMat_JobId);
        //                                cmd.Parameters.AddWithValue("@Char_Name", Char_Name);
        //                                cmd.Parameters.AddWithValue("@Char_OldValue", Char_OldValue);
        //                                cmd.Parameters.AddWithValue("@Char_NewValue", Char_NewValue);


        //                                cmd.Connection = con;
        //                                con.Open();
        //                                DataTable dtResult = new DataTable();
        //                                SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
        //                                oAdapter.Fill(dtResult);
        //                                con.Close();
        //                            }
        //                            //    using (SqlConnection con = new SqlConnection(strConn))
        //                            //    {
        //                            //        SqlCommand cmd = new SqlCommand();
        //                            //        cmd.CommandType = CommandType.StoredProcedure;
        //                            //        cmd.CommandText = "spUpdateQuotationfromJob";
        //                            //        cmd.Parameters.AddWithValue("@MinPrice", string.Format("{0}", 0));
        //                            //        cmd.Parameters.AddWithValue("@subID", string.Format("{0}", Condition.ToString() == "zpm1" ? "0" : AppID.ToString()));
        //                            //        cmd.Parameters.AddWithValue("@ID", AppID.ToString());
        //                            //        cmd.Connection = con;
        //                            //        con.Open();
        //                            //        cmd.ExecuteNonQuery();
        //                            //        con.Close();
        //                            //    }
        //                            //}
        //                        }
        //                    //var records = csv.GetRecords<dynamic>();
        //                    //foreach (var item in records)
        //                    //{
        //                    //    var da = item.Result;
        //                    //    var condition = item[@"Condition Type RV13A-KSCHL"];  
        //                    //}
        //                    //foreach (dynamic record in records.ToList())
        //                    //{
        //                    //    var data = record["IfColumn"];
        //                    //}
        //                }
        //            }
        //        }
        //        try
        //        {
        //            //File.Move(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s));
        //            //HttpContext.Current.Server.MapPath("~/ExcelFiles/Processed/" + Path.GetFileName(s)));
        //            // Copy the file and overwrite if it exists
        //            File.Copy(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s), true);

        //            // Delete the source file
        //            File.Delete(s);
        //        }
        //        catch (IOException iox)
        //        {
        //            Console.WriteLine(iox.Message);
        //        }
        //    }
        //    // Move MM01 & BAPI_UpdateMATCharacteristics
        //    filePaths = Directory.GetFiles(dir, "MM01*_Result*.csv");
        //    foreach (string s in filePaths)
        //    {
        //        try
        //        {
        //            using (var reader = new StreamReader(s))
        //            {
        //                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        //                {
        //                    while (csv.Read())
        //                    {//This will advance the reader to the next record.

        //                        //You can use an indexer to get by position or name. 
        //                        //This will return the field as a string

        //                        // By position
        //                        var field = csv[0];
        //                        //var AppID = csv["AppID"];
        //                        // By header name
        //                        //csv.Read();
        //                        csv.ReadHeader();
        //                        string[] headerRow = csv.Context.Reader.HeaderRecord;
        //                        string[] filteredValues = Array.FindAll(headerRow, x => x.Contains("Result"));
        //                        var Condition = "";// csv["Condition TypeRV13A-KSCHL"];
        //                        if (filteredValues.Length > 0)
        //                            if (csv["Result"] == "Condition records saved")
        //                            {
        //                            }
        //                    }
        //                }
        //            }
        //            //File.Move(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s));
        //            //HttpContext.Current.Server.MapPath("~/ExcelFiles/Processed/" + Path.GetFileName(s)));
        //            // Copy the file and overwrite if it exists
        //            File.Copy(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s), true);

        //            // Delete the source file
        //            File.Delete(s);
        //        }
        //        catch (IOException iox)
        //        {
        //            Console.WriteLine(iox.Message);
        //        }
        //    }
        //    // Move BAPI_UpdateMATCharacteristics
        //    filePaths = Directory.GetFiles(dir, "BAPI*_Result*.csv");
        //    foreach (string s in filePaths)
        //    {
        //        try
        //        {
        //            //File.Move(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s));
        //            //HttpContext.Current.Server.MapPath("~/ExcelFiles/Processed/" + Path.GetFileName(s)));
        //            // Copy the file and overwrite if it exists
        //            File.Copy(s, @"D:\SAPInterfaces\Inbound\Processed\" + Path.GetFileName(s), true);

        //            // Delete the source file
        //            File.Delete(s);
        //        }
        //        catch (IOException iox)
        //        {
        //            Console.WriteLine(iox.Message);
        //        }
        //    }
        //}
        //public static void localProcessKill(string processName)
        //{
        //    foreach (Process p in Process.GetProcessesByName(processName))
        //    {
        //        p.Kill();
        //    }
        //}
        //public static void testsendmaster(string SubChanged_Id)
        //{
        //    string strSQL = " select Id,Changed_Charname,Description,Changed_Action,Old_Description from TransMaster where Changed_id ='" + SubChanged_Id + "'";
        //    DataTable dt = CNService.builditems(strSQL);
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        string _Id = dr["Id"].ToString();
        //        string _Description = dr["Description"].ToString();
        //        string _Changed_Action = dr["Changed_Action"].ToString();
        //        string _old_id = dr["Old_Description"].ToString();
        //        string[] value = { dr["Changed_Charname"].ToString(), _Description, _Id, _Changed_Action, _old_id };
        //        CNService.master_artwork(value);
        //    }
        //}
        //public static void SendEmailUpdateMaster(string _name)
        //{
        //    //string datapath = "~/FileTest/" + _name;
        //    string _email = "";
        //    string _Id = "";
        //    string _Description = "";
        //    string _Body = "";
        //    string _Attached = "";
        //    string SubChanged_Id = CNService.ReadItems(@"select cast(substring('"
        //    + _name.ToString() + "',2,len('" + _name.ToString() + "')-1) as nvarchar(max)) value");
        //    testsendmaster(SubChanged_Id);
        //    DataTable dt = new DataTable();
        //    using (SqlConnection con = new SqlConnection(CNService.strConn))
        //    {
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = "spsendemail_upm";
        //        cmd.Parameters.AddWithValue("@Changed_Id", _name.ToString());
        //        cmd.Connection = con;
        //        con.Open();
        //        SqlDataAdapter oAdapter = new SqlDataAdapter(cmd);
        //        oAdapter.Fill(dt);
        //        con.Close();
        //    }
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        _email = dr["Email"].ToString();
        //        _Id = dr["Id"].ToString();
        //        _Description = dr["Description"].ToString();
        //        _Body = dr["Body"].ToString();
        //        _Attached = dr["attached"].ToString();
        //    }
        //    //        MailMessage msg = new MailMessage();
        //    //        string[] words = _email.Split(';');
        //    //        foreach (string word in words)
        //    //        {
        //    //            msg.To.Add(new MailAddress(word));
        //    //            //Console.WriteLine(word);
        //    //        }
        //    //        //msg.To.Add(new MailAddress(_email));
        //    //        msg.From = new MailAddress("wshuttleadm@thaiunion.com");
        //    //        msg.Subject = "Maintained characteristic master data in SAP" + "[" + _Body.Substring(0, 6) + "]";
        //    //        //msg.Body = "Id  " + _Id.ToString() + "Description  " + _Description.ToString() + " Changed";
        //    //        //msg.Body = "Maintained characteristic master completed";
        //    //        msg.Body = _Body;
        //    //        //msg.Attachments.Add(new System.Net.Mail.Attachment(_Attached));
        //    //        msg.IsBodyHtml = true;
        //    //
        //    //        SmtpClient client = new SmtpClient();
        //    //        client.UseDefaultCredentials = false;
        //    //        client.Credentials = new System.Net.NetworkCredential("wshuttleadm@thaiunion.com", "WSP@ss2018");
        //    //        client.Port = 587; // You can use Port 25 if 587 is blocked (mine is!)
        //    //        client.Host = "smtp.office365.com";
        //    //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    //        client.EnableSsl = true;
        //    //        try
        //    //        {
        //    //            client.Send(msg);
        //    //            Context.Response.Write("Message Sent Succesfully");
        //    //        }
        //    //        catch (Exception ex)
        //    //        {
        //    //            Context.Response.Write(ex.ToString());
        //    //        }
        //    CNService.sendemail(_email, "", _Body,
        //        "PRD Characteristic master is maintained in SAP " + "[" + _Body.Substring(0, 6) + "]",
        //        _Attached);
        //}

    }
}


public partial class SQ01_OUTBOUND
{
    public string CHARACTERISTIC_NAME { get; set; }
    public string TXTSP00003 { get; set; }
    public string APPID { get; set; }

}