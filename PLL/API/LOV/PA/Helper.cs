using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PLL.API.LOV.PA
{
    public class Helper
    {
        public static SAP_M_CHARACTERISTIC_RESULT QueryByName(string name, SAP_M_CHARACTERISTIC_REQUEST param)
        {
            SAP_M_CHARACTERISTIC_RESULT Results = new SAP_M_CHARACTERISTIC_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        List<int> listCharID_exclude = null;  //by aof 202306 for CR#IGRID_REIM_PA
                        var product_group = "";  //by aof 202306 for CR#IGRID_REIM_PA
                        var registered_no = "";  //by aof 202306 for CR#IGRID_REIM_PA

                        if (param == null || param.data == null)
                        {
                            param = new SAP_M_CHARACTERISTIC_REQUEST();
                            param.data = new SAP_M_CHARACTERISTIC_2();
                        }
                        else
                        {
                            //start by aof 202306 for CR#IGRID_REIM_PA
                            if (!string.IsNullOrEmpty(param.data.WHERE_NOT_IN_CHARACTERISTIC_ID))
                            {
                                listCharID_exclude = param.data.WHERE_NOT_IN_CHARACTERISTIC_ID?.Split(',')?.Select(Int32.Parse)?.ToList();
                            }

                            if (!string.IsNullOrEmpty(param.data.STR_PRODUCT_CODE))
                            {
                                var listProductCode = param.data.STR_PRODUCT_CODE.Split('@');


                                if (listProductCode.Length > 0)
                                {
                                    //if product code length less than 18 digit change data to 'X' cos  data'X' cannot find in XECM_M_PRODUCT
                                    for (int i = 0; i <= listProductCode.Length -1 ; i++)
                                    {
                                        if (listProductCode[i].Length < 18)
                                        {
                                            listProductCode[i] = "X";
                                        }
                                    }

                                
                                }

                                if (listProductCode.Length > 0 )
                                {

                                    var product_code = context.XECM_M_PRODUCT.Where(w => listProductCode.Contains(w.PRODUCT_CODE)).Select(s => s.PRODUCT_CODE).FirstOrDefault();

                                    if (!string.IsNullOrEmpty(product_code) && product_code.Length > 1)
                                    {
                                        product_group = product_code.Substring(1, 1);
                                    }
                                    else
                                    {

                                        // check in case not found product in XECM_M_PORUDCT
                                        foreach (var p in listProductCode)
                                        {
                                            if (p.Length >= 18)
                                            {
                                                var tempProduct = p.Substring(0, 18);
                                                tempProduct = tempProduct.Replace(" ", "").TrimStart().TrimEnd().Trim();
                                                if (tempProduct.Length == 18)
                                                {
                                                    var firstDigit = tempProduct.Substring(0, 1);
                                                    if (firstDigit == "2" || firstDigit == "3")
                                                    {
                                                        product_group = tempProduct.Substring(1, 1);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            
                            }

                            if (param.data.REGISTER_CHARACTERISTIC_ID > 0)
                            {
                                registered_no = context.SAP_M_CHARACTERISTIC.Where(w => w.CHARACTERISTIC_ID == param.data.REGISTER_CHARACTERISTIC_ID).Select(s => s.VALUE).FirstOrDefault();
                            }
                            //end by aof 202306 for CR#IGRID_REIM_PA
                        }
                        param.data.NAME = name;

                        if (!String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                        {
                            param.data.DESCRIPTION = param.data.DISPLAY_TXT;
                        }

                        param.data.IS_ACTIVE = "X";
                        if(param.data.PRODUCT_TYPE==null)
                        param.data.PRODUCT_TYPE = "X";

                        //start by aof 202306 for CR#IGRID_REIM_PA
                        // -------------AOF Commented------------------------------------------------
                        //if (name== "ZPKG_SEC_PLANT_REGISTER" || name== "ZPKG_SEC_COMPANY_ADR")
                        //Results.data = MapperServices.SAP_M_CHARACTERISTIC(CNService.GetQueryByName(name, param.data.PRODUCT_TYPE));
                        //else
                        //Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItemContain(MapperServices.SAP_M_CHARACTERISTIC(param.data), context));
                        // -------------AOF Commented------------------------------------------------
                        if (name == "ZPKG_SEC_PLANT_REGISTER")
                        {
                            if (param.data.PRODUCT_TYPE != "X")
                            {
                                Results.data = MapperServices.SAP_M_CHARACTERISTIC(CNService.GetQueryByName(name, param.data.PRODUCT_TYPE));
                            } else
                            {
                               
                                var list = context.Database.SqlQuery<SAP_M_CHARACTERISTIC_2>("sp_ART_IGRID_PLANT_REGISTER @PRODUCT_GROUP", new System.Data.SqlClient.SqlParameter("@PRODUCT_GROUP", product_group)).ToList();
                                Results.data = list;
                            }                       
                        }
                        else if (name == "ZPKG_SEC_COMPANY_ADR")
                        {

                            if (param.data.PRODUCT_TYPE != "X")
                            {
                                Results.data = MapperServices.SAP_M_CHARACTERISTIC(CNService.GetQueryByName(name, param.data.PRODUCT_TYPE));
                            } else
                            {                              
                                var list = context.Database.SqlQuery<SAP_M_CHARACTERISTIC_2>("sp_ART_IGRID_PLANT_REGISTER_ADDRESS @PRODUCT_GROUP,@REGISTERED_NO", new System.Data.SqlClient.SqlParameter("@PRODUCT_GROUP", product_group),new  System.Data.SqlClient.SqlParameter("@REGISTERED_NO", registered_no)).ToList();
                                Results.data = list;
                            }
                        }
                        else
                        {
                            Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItemContain(MapperServices.SAP_M_CHARACTERISTIC(param.data), context));
                        }
                          

                        Results.status = "S";

                        //start by aof 202306 for CR#IGRID_REIM_PA
                        if (listCharID_exclude !=null && listCharID_exclude.Count() > 0)
                        {
                            var listChars = MapperServices.SAP_M_CHARACTERISTIC(context.SAP_M_CHARACTERISTIC.Where(w => w.NAME == name).ToList());
                            var listConstaints = context.ART_M_CONSTANT.Where(w => w.VARIABLE_NAME == "ZPKG_SEC_SYMBOL" && w.PROGRAM_NAME == "TASKFORMPA" && w.IS_ACTIVE == "X" && w.OPTION == "EQ").ToList();
                            if (listConstaints != null && listConstaints.Count > 0)
                            {
                                
                                var listConstaintValues = listConstaints.Select(s => s.LOWVALUE).Distinct().ToList();
                                var grpFuncs = listConstaints.Select(s => s.FUNCAREA).Distinct().ToList();
                                foreach (var funcs in grpFuncs)
                                {
                                    var tempListConstainValues = listConstaints.Where(w => w.FUNCAREA == funcs).Select(s => s.LOWVALUE).Distinct().ToList();
                                    List<SAP_M_CHARACTERISTIC_2> tempListChars;

                                    tempListChars = listChars.Where(w => tempListConstainValues.Contains(w.VALUE)).Distinct().ToList();

                                    if (tempListChars.Where(w => listCharID_exclude.Contains(w.CHARACTERISTIC_ID)).Count() > 0)
                                    {
                                        //hard code NO SYMBOL
                                        if (listChars.Where(w => listCharID_exclude.Contains(w.CHARACTERISTIC_ID) && w.VALUE == "NO SYMBOL").Count() > 0 && funcs == "Symbol")
                                        {
                                            var tempListCharIds = listChars.Where(w => !listConstaintValues.Contains(w.VALUE)).Select(s => s.CHARACTERISTIC_ID).ToList();
                                            listCharID_exclude.AddRange(tempListCharIds);
                                        }
                                        else
                                        {
                                            var tempListCharIds = tempListChars.Select(s => s.CHARACTERISTIC_ID).Distinct().ToList();
                                            listCharID_exclude.AddRange(tempListCharIds);

                                        }

                                    }
                                    else
                                     {
                                        if (funcs == "Symbol")
                                        {
                                            var listCharIDs = listChars.Where(w => listConstaintValues.Contains(w.VALUE)).Select(s => s.CHARACTERISTIC_ID).ToList();
                                            var tempListChars_exclude = listChars.Where(w => listCharID_exclude.Contains(w.CHARACTERISTIC_ID)).ToList();
                                            if (tempListChars_exclude.Where(w=>!listCharIDs.Contains(w.CHARACTERISTIC_ID)).Count() > 0)
                                            {
                                                var tempListCharIds = tempListChars.Select(s => s.CHARACTERISTIC_ID).Distinct().ToList();
                                                listCharID_exclude.AddRange(tempListCharIds);
                                            }                                                                    
                                        }
                                    }

                                }
                            }
                            Results.data = Results.data.Where(w => !listCharID_exclude.Contains(w.CHARACTERISTIC_ID)).ToList();

                        }
                        //end by aof 202306 for CR#IGRID_REIM_PA

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].CHARACTERISTIC_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].DESCRIPTION;
                            }


                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                Results.data = (from u1 in Results.data
                                                where u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())                                      
                                                select u1).ToList();
                            }

                            Results.data = Results.data.OrderBy(x => x.DESCRIPTION).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static SAP_M_CHARACTERISTIC_RESULT MapData(SAP_M_CHARACTERISTIC_RESULT Results)
        {
            if (Results.data.Count > 0)
            {
                var result = Results.data.GroupBy(p => p.DESCRIPTION)
                   .Select(grp => grp.First())
                   .ToList();

                Results.data = result.OrderBy(x => x.DESCRIPTION).ToList();

                for (int i = 0; i < Results.data.Count; i++)
                {
                    Results.data[i].ID = Results.data[i].CHARACTERISTIC_ID;
                    Results.data[i].DISPLAY_TXT = Results.data[i].DESCRIPTION;
                }
            }

            return Results;
        }

        public static SAP_M_CHARACTERISTIC_RESULT GetFixedData(string[] data)
        {
            SAP_M_CHARACTERISTIC_RESULT Results = new SAP_M_CHARACTERISTIC_RESULT();
            try
            {

                Results.data = new List<SAP_M_CHARACTERISTIC_2>();

                int i = 1;
                foreach (var item in data)
                {
                    Results.data.Add(new SAP_M_CHARACTERISTIC_2 { ID = i, DISPLAY_TXT = item });
                    i++;
                }

                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static string GetMaterialCode(int id)
        {
            SAP_M_CHARACTERISTIC_REQUEST param = new SAP_M_CHARACTERISTIC_REQUEST();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        SAP_M_CHARACTERISTIC_2 characteristic = new SAP_M_CHARACTERISTIC_2();
                        characteristic.NAME = "ZPKG_SEC_GROUP";
                        characteristic.IS_ACTIVE = "X";
                        var data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItem(MapperServices.SAP_M_CHARACTERISTIC(characteristic), context));

                        SAP_M_CHARACTERISTIC_2 result = new SAP_M_CHARACTERISTIC_2();
                        if (data.Count > 0)
                        {
                            result = data.Where(w => w.CHARACTERISTIC_ID == id).FirstOrDefault();
                        }
                        return result.VALUE.ToUpper();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}