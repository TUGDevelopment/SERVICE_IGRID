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
using System.Configuration;
using BLL.DocumentManagement;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace BLL.Helpers
{
    public class MaterialLockReportHelper
    {

 

        public static ART_M_USER_ROLE_RESULT GetRoleViwerMaterialLockReport(ART_M_USER_ROLE_REQUEST param)
        {
            ART_M_USER_ROLE_RESULT Results = new ART_M_USER_ROLE_RESULT();
            Results.data = new List<ART_M_USER_ROLE_2>();

            try
            {

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;
                        int userID = param.data.USER_ID;
                        List<ART_M_USER_ROLE_2> list;
                        int roleID_VIEWER = context.ART_M_ROLE.Where(w => w.ROLE_CODE == "REP_VIEWER_MAT_LIST").Select(s => s.ROLE_ID).First();

                   
                        list = (from m in context.ART_M_USER_ROLE
                        where m.USER_ID == userID && m.ROLE_ID == roleID_VIEWER
                        select new ART_M_USER_ROLE_2()
                        {
                            USER_ROLE_ID = m.USER_ROLE_ID,
                            USER_ID = m.USER_ID,
                            ROLE_ID = m.ROLE_ID

                        }).ToList();
                       


                        Results.data = list;
                        Results.status = "S";
                        Results.draw = param.draw;

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



        public static string getWhereMat(TU_MATERIAL_LOCK_REPORT_MODEL_REQUEST param, ARTWORKEntities context)
        {
            string where = "";

            if (!String.IsNullOrEmpty(param.data.SEARCH_SOLD_TO))
            {
                List<string> listSold = new List<string>();
                listSold = param.data.SEARCH_SOLD_TO.Split(':').ToList();

                if (listSold.Count > 0)
                {
                    string soldNO = listSold[0];
                    where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(soldNO, "SOLD_TO"));
                }
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_SHIP_TO))
            {
                List<string> listShip = new List<string>();
                listShip = param.data.SEARCH_SHIP_TO.Split(':').ToList();

                if (listShip.Count > 0)
                {
                    string shipNO = listShip[0];
                    where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(shipNO, "SHIP_TO"));
                }
            }


            if (!string.IsNullOrEmpty(param.data.SEARCH_MATERIAL_NO))
            {

                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_MATERIAL_NO, "MATERIAL_NO"));
            }


            if (!String.IsNullOrEmpty(param.data.SEARCH_BRAND))
            {
                //q = q.Where(w => w.BRAND == param.data.SEARCH_BRAND);
                var tempBrand = param.data.SEARCH_BRAND.Split(':')[0];
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "SUBSTRING(MATERIAL_NO,3,3) = '" + tempBrand + "'");
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_COUNTRY))
            {

                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_COUNTRY, "COUNTRY"));
            }

            //if (!String.IsNullOrEmpty(param.data.SEARCH_PIC))
            //{
            //    var arrName = param.data.SEARCH_PIC.Split(' ');
            //    var i = ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { FIRST_NAME = arrName[1] }, context).FirstOrDefault().USERNAME;
            //    where = CNService.getSQLWhereByJoinStringWithAnd(where, "PIC='" + i + "'");
            //}

            if (!String.IsNullOrEmpty(param.data.SEARCH_ZONE))
            {
                //q = q.Where(w => w.ZONE == param.data.SEARCH_ZONE);
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "ZONE='" + param.data.SEARCH_ZONE + "'");
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_PRODUCT_CODE))
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_PRODUCT_CODE, "PRODUCT_CODE"));
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_STATUS) && !param.data.SEARCH_STATUS.Equals("ALL"))
            {
                //  q = q.Where(w => w.STATUS == param.data.SEARCH_STATUS);

                where = CNService.getSQLWhereByJoinStringWithAnd(where, "STATUS='" + param.data.SEARCH_STATUS + "'");

            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_PKG_TYPE))
            {
                //var arrMat_ = param.data.SEARCH_PKG_TYPE.Split(',');

                //var resultMat5 = (from e in arrMat_
                //                  where !String.IsNullOrEmpty(e)
                //                  select e).ToList();

                //if (resultMat5 != null)
                //{
                //    q = q.Where(m => resultMat5.Contains(m.MATERIAL_NO.Substring(0, 2)));
                //}
                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_PKG_TYPE, "SUBSTRING(MATERIAL_NO,1,2)"));
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_REMARK_LOCK))
            {
                // q = q.Where(w => w.REMARK_LOCK.Contains(param.data.REMARK_LOCK));

                where = CNService.getSQLWhereByJoinStringWithAnd(where, "REMARK_LOCK LIKE N'%" + param.data.SEARCH_REMARK_LOCK.Replace(" ","%") + "%'");
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_REMARK_UNLOCK))
            {
                //q = q.Where(w => w.REMARK_UNLOCK.Contains(param.data.REMARK_UNLOCK));
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "REMARK_UNLOCK LIKE N'%" + param.data.SEARCH_REMARK_UNLOCK.Replace(" ", "%" )+ "%'");
            }
            return where;
        }



        public static TU_MATERIAL_LOCK_REPORT_MODEL_RESULT GetMaterialLockReportV2(TU_MATERIAL_LOCK_REPORT_MODEL_REQUEST param)
        {
            //isRecursive = true;
            TU_MATERIAL_LOCK_REPORT_MODEL_RESULT Results = new TU_MATERIAL_LOCK_REPORT_MODEL_RESULT();
            Results.data = new List<TU_MATERIAL_LOCK_REPORT_MODEL>();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    //using (var dbContextTransaction = CNService.IsolationLevel(context))
                    //{
                    context.Database.CommandTimeout = 300;
                    int cnt = 0;
                    List<TU_MATERIAL_LOCK_REPORT_MODEL> list;
                    List<TU_MATERIAL_LOCK_DETAIL_REPORT_MODEL> listDetail;

                    string where_mat = getWhereMat(param, context);

                    bool f_check_artwork = param.data.SEARCH_CHECK_ARTWORK_FILE=="X";
                    bool f_server_side = param.data.SEARCH_SERVER_SIDE == "X";

                    var token = CWSService.getAuthToken();
                    //modelHeader.IS_HAS_FILES = null;param.data.SEARCH_CHECK_ARTWORK_FILE=="X";                       
                    string SecondaryPackagingNodeID = ConfigurationManager.AppSettings["SecondaryPackagingNodeID"];
                    string SecondaryPkgArtworkFolderName = ConfigurationManager.AppSettings["SecondaryPkgArtworkFolderName"];
                    string sqlExecute = "";
                      
                    string where_matid = "0";

                    list = context.Database.SqlQuery<TU_MATERIAL_LOCK_REPORT_MODEL>("sp_ART_REPORT_MATERIAL_LOCK @where_mat", new SqlParameter("@where_mat", where_mat)).ToList();

                    if (!string.IsNullOrEmpty(param.data.SEARCH_PAOWNER))
                    {
                        list = list.Where(w => w.PA_OWNER == param.data.SEARCH_PAOWNER.Trim()).ToList();
                    }


                    if (list !=null)
                    {
                        if (f_server_side)
                        {
                            Results.recordsTotal = list.Count();

                            var clientFliter = false;
                            if (param.columns != null)
                            {

                                string val;
                                for (var i = 0; i<param.columns.Count ();i++)
                                {
                                    val = param.columns[i].search.value;

                                    if (!string.IsNullOrEmpty(val))
                                    {
                                        val = val.Trim();
                                        switch (i)
                                        {
                                            case 1:
                                                list = list.Where(w => w.MATERIAL_NO.Contains(val)).ToList();
                                                break;
                                            case 2:
                                                list = list.Where(w => w.MATERIAL_DESCRIPTION.Contains(val)).ToList();
                                                break;
                                            case 3:
                                                if ("yes".Contains(val.Trim().ToLower()))
                                                {
                                                    list = list.Where(w => w.IS_HAS_FILES.Equals("X")).ToList();
                                                }
                                                else if ("no".Contains(val.Trim().ToLower()))
                                                {
                                                    list = list.Where(w => w.IS_HAS_FILES != "X").ToList();
                                                }
                                                else
                                                {
                                                    list = list.Where(w => w.IS_HAS_FILES == "Z").ToList();
                                                }

                                                break;
                                            case 4:
                                                if ("in use".Contains(val.Trim().ToLower()))
                                                {
                                                    list = list.Where(w => w.STATUS == "I").ToList();
                                                }
                                                else if ("obsolete".Contains(val.Trim().ToLower()))
                                                {
                                                    list = list.Where(w => w.STATUS == "O").ToList();
                                                }
                                                else if ("conversion".Contains(val.Trim().ToLower()))
                                                {
                                                    list = list.Where(w => w.STATUS != "C").ToList();
                                                }
                                                else
                                                {
                                                   list = list.Where(w => w.STATUS != "Z").ToList();
                                                }
                                                break;
                                            case 5:
                                                list = list.Where(w => w.REQUEST_FORM_NO.Contains(val)).ToList();
                                                break;
                                            case 6:
                                                list = list.Where(w => w.ARTWORK_NO.Contains(val)).ToList();
                                                break;
                                            case 7:
                                                list = list.Where(w => w.MOCKUP_NO.Contains(val)).ToList();
                                                break;
                                            case 8:
                                                list = list.Where(w => w.UNLOCK_DATE_FROM != null).ToList();
                                                if (list.Count > 0)
                                                {
                                                    list = list.Where(w => w.UNLOCK_DATE_FROM.ToString().Contains(val)).ToList();
                                                }
                                               
                                                break;
                                            case 9:
                                                list = list.Where(w => w.UNLOCK_DATE_TO  != null).ToList();
                                                if (list.Count > 0)
                                                {
                                                    list = list.Where(w => w.UNLOCK_DATE_TO.ToString().Contains(val)).ToList();
                                                }
                                                break;
                                            case 10:
                                                list = list.Where(w => w.REMARK_UNLOCK.Contains(val)).ToList();
                                                break;
                                            case 11:
                                                list = list.Where(w => w.REMARK_LOCK.Contains(val)).ToList();
                                                break;
                                            case 12:
                                                list = list.Where(w => w.LOG_DATE != null).ToList();
                                                if (list.Count > 0)
                                                {
                                                    list = list.Where(w => w.LOG_DATE.ToString().Contains(val)).ToList();
                                                }
                                                break;
                                            case 19:
                                                list = list.Where(w => w.PRIMARY_SIZE.Contains(val)).ToList();
                                                break;
                                            case 20:
                                                list = list.Where(w => w.PACK_SIZE.Contains(val)).ToList();
                                                break;
                                            case 21:
                                                list = list.Where(w => w.PACKAGING_STYLE.Contains(val)).ToList();
                                                break;
                                            case 22:
                                                list = list.Where(w => w.PACKAGING_TYPE.Contains(val)).ToList();
                                                break;
                                            case 23:
                                                list = list.Where(w => w.PRIMARY_TYPE.Contains(val)).ToList();
                                                break;
                                            case 24:
                                                list = list.Where(w => w.PA_OWNER.Contains(val)).ToList();
                                                break;
                                            case 25:
                                                list = list.Where(w => w.PG_OWNER.Contains(val)).ToList();
                                                break;

                                            default:
                                                break;
                                        }
                                    }
                                }

                             
                            }
                            Results.recordsFiltered = list.Count();

                            list = list.Distinct().OrderBy(i => i.MATERIAL_NO).Skip(param.start).Take(param.length).ToList();
                            // Results.recordsFiltered = list.Count();
                          
                            for (var i = 0; i < list.Count(); ++i)
                            {
                                where_matid += "," + list[i].MATERIAL_LOCK_ID;
                            }

                            where_mat = "MATERIAL_LOCK_ID in ("+ where_matid +")";
                        }
                    }
                


                    //list = FilterQueryWarehouseReport(q.AsQueryable(), param);
                    // where_matid = "(MATERIAL_LOCK_ID in (" + where_matid + "))";
                    // where_matid = where_mat;
                    listDetail = context.Database.SqlQuery<TU_MATERIAL_LOCK_DETAIL_REPORT_MODEL>("sp_ART_REPORT_MATERIAL_LOCK_DETAIL @where_mat", new SqlParameter("@where_mat", where_mat)).ToList();
                    listDetail = FilterQueryMaterialLockDetail(listDetail.AsQueryable(), param);

                        
                    if (list != null)
                    {
                        if (list.Count > 0)
                        {
                            var cntList = list.Count;
                            for (var i = 0; i < cntList; ++i)
                                {
                                    // where_matid += "," + list[i].MATERIAL_LOCK_ID;

                                if (f_check_artwork)
                                    {
                                        var nodeMat5 = CWSService.getNodeByName(Convert.ToInt64(SecondaryPackagingNodeID), list[i].MATERIAL_NO + " - " + list[i].MATERIAL_DESCRIPTION, token);

                                        if (nodeMat5 != null)
                                        {
                                            var FinalArtwork = CWSService.getNodeByName(-nodeMat5.ID, SecondaryPkgArtworkFolderName, token);
                                            if (FinalArtwork != null)
                                            {
                                                var fileInNodeMat5 = CWSService.getAllNodeInFolder(FinalArtwork.ID, token);

                                                if (fileInNodeMat5 != null)
                                                {
                                                    if (string.IsNullOrEmpty(list[i].IS_HAS_FILES))
                                                    {
                                                        sqlExecute += "UPDATE ART_WF_ARTWORK_MATERIAL_LOCK SET IS_HAS_FILES = 'X', UPDATE_DATE = getdate() WHERE MATERIAL_LOCK_ID=" + list[i].MATERIAL_LOCK_ID + ";";
                                                        list[i].IS_HAS_FILES = "X";
                                                    }

                                                    else
                                                    {
                                                        if (list[i].IS_HAS_FILES != "X")
                                                        {
                                                            sqlExecute += "UPDATE ART_WF_ARTWORK_MATERIAL_LOCK SET IS_HAS_FILES = 'X', UPDATE_DATE = getdate() WHERE MATERIAL_LOCK_ID=" + list[i].MATERIAL_LOCK_ID + ";";
                                                            list[i].IS_HAS_FILES = "X";
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (!string.IsNullOrEmpty(list[i].IS_HAS_FILES))
                                                    {
                                                        if (list[i].IS_HAS_FILES == "X")
                                                        {
                                                            sqlExecute += "UPDATE ART_WF_ARTWORK_MATERIAL_LOCK SET IS_HAS_FILES = NULL, UPDATE_DATE = getdate() WHERE MATERIAL_LOCK_ID=" + list[i].MATERIAL_LOCK_ID + ";";
                                                            list[i].IS_HAS_FILES = null;
                                                        }

                                                    }

                                                }
                                            }
                                            else
                                            {

                                                if (!string.IsNullOrEmpty(list[i].IS_HAS_FILES))
                                                {
                                                    if (list[i].IS_HAS_FILES == "X")
                                                    {
                                                        sqlExecute += "UPDATE ART_WF_ARTWORK_MATERIAL_LOCK SET IS_HAS_FILES = NULL, UPDATE_DATE = getdate() WHERE MATERIAL_LOCK_ID=" + list[i].MATERIAL_LOCK_ID + ";";
                                                        list[i].IS_HAS_FILES = null;
                                                    }

                                                }

                                            }

                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(list[i].IS_HAS_FILES))
                                            {
                                                if (list[i].IS_HAS_FILES == "X")
                                                {
                                                    sqlExecute += "UPDATE ART_WF_ARTWORK_MATERIAL_LOCK SET IS_HAS_FILES = NULL, UPDATE_DATE = getdate() WHERE MATERIAL_LOCK_ID=" + list[i].MATERIAL_LOCK_ID + ";";
                                                    list[i].IS_HAS_FILES = null;
                                                }

                                            }

                                        }


                                        if ((i + 1) % 25 == 0)
                                        {
                                            if (!string.IsNullOrEmpty(sqlExecute))
                                            {

                                                context.Database.ExecuteSqlCommand(sqlExecute);
                                                sqlExecute = "";


                                            }
                                            token = CWSService.getAuthToken();
                                        }


                                    }



                                    if (!string.IsNullOrEmpty(list[i].IS_HAS_FILES))
                                    {
                                        if (list[i].IS_HAS_FILES.ToUpper().Equals("X"))
                                        {
                                            list[i].IS_HAS_FILES = "Yes";
                                        }
                                        else
                                        {
                                            list[i].IS_HAS_FILES = "No";
                                        }
                                    }
                                    else
                                    {
                                        list[i].IS_HAS_FILES = "No";
                                    }


                                    if (!string.IsNullOrEmpty(list[i].STATUS))
                                    {
                                        if (list[i].STATUS.ToUpper().Equals("I"))
                                        {
                                            list[i].STATUS = "In use";
                                        }
                                        else if (list[i].STATUS.ToUpper().Equals("O"))
                                        {
                                            list[i].STATUS = "Obsolete";
                                        }
                                            else if (list[i].STATUS.ToUpper().Equals("C"))
                                        {
                                            list[i].STATUS = "Conversion";
                                        }
                                    }


                                    //int mat_lock_id;
                                    if (listDetail != null)
                                    {
                                        if (listDetail.Count > 0)
                                        {

                                            if (f_server_side)
                                            {
                                                // server side

                                                var temp_listDetail  = listDetail.Where(w => w.MATERIAL_LOCK_ID.Equals(list[i].MATERIAL_LOCK_ID)).OrderByDescending(o => (o.PRODUCT_CODE, o.SOLD_TO)).ToList();

                                                TU_MATERIAL_LOCK_REPORT_MODEL temp_list;

                                                if (temp_listDetail !=null) 
                                                {
                                                    foreach (var obj in temp_listDetail)
                                                    {
                                                        temp_list = new TU_MATERIAL_LOCK_REPORT_MODEL();
                                                        temp_list.GROUPING = list[i].GROUPING;
                                                        //temp_list.MATERIAL_NO = obj.MATERIAL_NO;
                                                        temp_list.PRODUCT_CODE = obj.PRODUCT_CODE;
                                                        temp_list.SOLD_TO = setValue(obj.SOLD_TO);
                                                           
                                                        temp_list.SHIP_TO = setValue(obj.SHIP_TO);
                                                        temp_list.BRAND = setValue(obj.BRAND);
                                                        temp_list.COUNTRY = setValue(obj.COUNTRY);
                                                        temp_list.ZONE = setValue(obj.ZONE);
                                                        temp_list.PACK_SIZE = obj.PACK_SIZE;
                                                        temp_list.PRIMARY_SIZE = obj.PRIMARY_SIZE;
                                                        temp_list.PACKAGING_STYLE = obj.PACKAGING_STYLE;
                                                        list.Add(temp_list);
                                                    
                                                    }
                                                }


                                            }
                                            else
                                            {
                                                // client side
                                                list[i].listDETAIL = new List<TU_MATERIAL_LOCK_DETAIL_REPORT_MODEL>();
                                                list[i].listDETAIL = listDetail.Where(w => w.MATERIAL_LOCK_ID.Equals(list[i].MATERIAL_LOCK_ID)).OrderByDescending(o => (o.PRODUCT_CODE, o.SOLD_TO)).ToList();

                                                if (list[i].listDETAIL != null)
                                                {
                                                    foreach (var obj in list[i].listDETAIL)
                                                    {
                                                        obj.SOLD_TO = setValue(obj.SOLD_TO);
                                                        obj.SHIP_TO = setValue(obj.SHIP_TO);
                                                        obj.BRAND = setValue(obj.BRAND);
                                                        obj.COUNTRY = setValue(obj.COUNTRY);
                                                        obj.ZONE = setValue(obj.ZONE);
                                                    }                                            
                                                }
                                                }


                                            }
                                    }

                                }                          
                                

                        }
                    }

                  
                     


                    if (f_check_artwork == true)
                    {
                        if (!string.IsNullOrEmpty(sqlExecute))
                        {
                            context.Database.ExecuteSqlCommand(sqlExecute);
                            sqlExecute = "";
                               
                        }

                    }
                    // dbContextTransaction.Commit();


                    list = list.OrderBy(o => (o.GROUPING,o.PRODUCT_CODE,o.SOLD_TO)).ToList();
                    cnt = list.Distinct().Count();


                    Results.data = list;
                    Results.status = "S";
                    Results.draw = param.draw;
                   // }
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }


        private static string setValue(string val)
        {
            string valReturn = "";

            if (val != null)
            {
                if (val.Trim() != ":")
                {
                    valReturn = val;
                }
            }
            return valReturn;
        }


        public static List<TU_MATERIAL_LOCK_DETAIL_REPORT_MODEL> FilterQueryMaterialLockDetail(IQueryable<TU_MATERIAL_LOCK_DETAIL_REPORT_MODEL> q, TU_MATERIAL_LOCK_REPORT_MODEL_REQUEST param)
        {

            if (!String.IsNullOrEmpty(param.data.SEARCH_SOLD_TO))
            {
                List<string> listSold = new List<string>();
                listSold = param.data.SEARCH_SOLD_TO.Split(':').ToList();

                if (listSold.Count > 0)
                {
                    string soldNO = listSold[0];
                    q = q.Where(w => w.SOLD_TO.StartsWith(soldNO));
                }
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_SHIP_TO))
            {
                List<string> listShip = new List<string>();
                listShip = param.data.SEARCH_SHIP_TO.Split(':').ToList();

                if (listShip.Count > 0)
                {
                    string shipNO = listShip[0];
                    q = q.Where(w => w.SHIP_TO.StartsWith(shipNO));
                }
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_PRODUCT_CODE))
            {
                var arrProduct = param.data.SEARCH_PRODUCT_CODE.Split(',');

                var resultProduct = (from e in arrProduct
                                     where !String.IsNullOrEmpty(e)
                                     select e).ToList();

                if (resultProduct != null)
                {
                    q = q.Where(m => resultProduct.Contains(m.PRODUCT_CODE));
                }
            }


            if (!String.IsNullOrEmpty(param.data.SEARCH_COUNTRY))
            {
                var arrCountry = param.data.SEARCH_COUNTRY.Split(',');

                var resultCountry = (from e in arrCountry
                                     where !String.IsNullOrEmpty(e)
                                     select e).ToList();

                if (resultCountry != null)
                {
                    q = q.Where(m => resultCountry.Contains(m.COUNTRY.Substring(0,2)));
                }
            }


            if (!String.IsNullOrEmpty(param.data.SEARCH_ZONE))
            {
                q = q.Where(m => m.ZONE == param.data.SEARCH_ZONE);
            }


            if (!String.IsNullOrEmpty(param.data.SEARCH_BRAND))
            {

              //  q = q.Where(w => w.BRAND != ":");

                //q = q.Where(w => w.BRAND == param.data.SEARCH_BRAND);
                var tempBrand = param.data.SEARCH_BRAND.Split(':')[0];
                q = q.Where(w => w.BRAND.StartsWith(tempBrand));
                
            }

            //try
            //{
            //  var cnt = q.ToList().Count;
            //}
            //catch (Exception ex)
            //{
            //    var err = ex.Message;
            //}
          

            return q.ToList();
        }


        public static ART_WF_ARTWORK_MATERIAL_LOCK_RESULT GetMaterialLockReportV2_BACKUP(ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST param)
        {
            //isRecursive = true;
            ART_WF_ARTWORK_MATERIAL_LOCK_RESULT Results = new ART_WF_ARTWORK_MATERIAL_LOCK_RESULT();
            Results.data = new List<ART_WF_ARTWORK_MATERIAL_LOCK_2>();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;
                        int cnt = 0;
                        List<ART_WF_ARTWORK_MATERIAL_LOCK_2> list;
                        

                        string where_matlock = getWhereMatLock(param, context);


                        list = context.Database.SqlQuery<ART_WF_ARTWORK_MATERIAL_LOCK_2>("sp_ART_REPORT_MATERIAL_LOCK @where_matlock", new SqlParameter("@where_matlock", where_matlock)).ToList();
                        //list = FilterQueryWarehouseReport(q.AsQueryable(), param);
                        list = list.OrderBy(o => (o.MATERIAL_NO, o.PRODUCT_CODE)).ToList();


                        cnt = list.Distinct().Count();


                        Results.data = list;
                        Results.status = "S";
                        Results.draw = param.draw;
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


        public static string getWhereMatLock(ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST param,ARTWORKEntities context)
        {
            string where = "";

            if (!String.IsNullOrEmpty(param.data.SEARCH_SOLD_TO))
            {
                List<string> listSold = new List<string>();
                listSold = param.data.SEARCH_SOLD_TO.Split(':').ToList();

                if (listSold.Count > 0)
                {
                    string soldNO = listSold[0];
                    where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(soldNO, "SOLD_TO"));
                }
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_SHIP_TO))
            {
                List<string> listShip = new List<string>();
                listShip = param.data.SEARCH_SHIP_TO.Split(':').ToList();

                if (listShip.Count > 0)
                {
                    string shipNO = listShip[0];
                    where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(shipNO, "SHIP_TO"));
                }
            }


            if (!string.IsNullOrEmpty(param.data.SEARCH_MATERIAL_NO))
            {
                
                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_MATERIAL_NO, "MATERIAL_NO"));
            }


            if (!String.IsNullOrEmpty(param.data.SEARCH_BRAND))
            {
                //q = q.Where(w => w.BRAND == param.data.SEARCH_BRAND);
                var tempBrand = param.data.SEARCH_BRAND.Split(':')[0];
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "SUBSTRING(MATERIAL_NO,3,3) = '" + tempBrand + "'");
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_COUNTRY))
            {
 
                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_COUNTRY, "COUNTRY"));
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_PIC))
            {
                var arrName = param.data.SEARCH_PIC.Split(' ');
                var i = ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { FIRST_NAME = arrName[1] }, context).FirstOrDefault().USERNAME;
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "PIC='"+ i +"'");
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_ZONE))
            {
                //q = q.Where(w => w.ZONE == param.data.SEARCH_ZONE);
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "ZONE='" + param.data.SEARCH_ZONE + "'");
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_PRODUCT_CODE))
            {
                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_PRODUCT_CODE, "PRODUCT_CODE"));
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_STATUS) && !param.data.SEARCH_STATUS.Equals("ALL"))
            {
              //  q = q.Where(w => w.STATUS == param.data.SEARCH_STATUS);

                where = CNService.getSQLWhereByJoinStringWithAnd(where, "STATUS='" + param.data.SEARCH_STATUS + "'");

            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_PKG_TYPE))
            {
                //var arrMat_ = param.data.SEARCH_PKG_TYPE.Split(',');

                //var resultMat5 = (from e in arrMat_
                //                  where !String.IsNullOrEmpty(e)
                //                  select e).ToList();

                //if (resultMat5 != null)
                //{
                //    q = q.Where(m => resultMat5.Contains(m.MATERIAL_NO.Substring(0, 2)));
                //}
                where = CNService.getSQLWhereByJoinStringWithAnd(where, CNService.getSQLWhereLikeByConvertString(param.data.SEARCH_PKG_TYPE, "SUBSTRING(MATERIAL_NO,1,2)"));
            }

            if (!String.IsNullOrEmpty(param.data.REMARK_LOCK))
            {
               // q = q.Where(w => w.REMARK_LOCK.Contains(param.data.REMARK_LOCK));

                where = CNService.getSQLWhereByJoinStringWithAnd(where, "REMARK_LOCK LIKE N'%" + param.data.REMARK_LOCK + "%'");
            }

            if (!String.IsNullOrEmpty(param.data.REMARK_UNLOCK))
            {
                //q = q.Where(w => w.REMARK_UNLOCK.Contains(param.data.REMARK_UNLOCK));
                where = CNService.getSQLWhereByJoinStringWithAnd(where, "REMARK_UNLOCK LIKE N'%" + param.data.REMARK_UNLOCK + "%'");
            }
            return where;
        }


        private static bool isRecursive { get; set; }
        public static ART_WF_ARTWORK_MATERIAL_LOCK_RESULT GetMaterialLockReport(ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST param)
        {
            isRecursive = true;
            ART_WF_ARTWORK_MATERIAL_LOCK_RESULT Results = new ART_WF_ARTWORK_MATERIAL_LOCK_RESULT();
            Results.data = new List<ART_WF_ARTWORK_MATERIAL_LOCK_2>();

            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<ART_WF_ARTWORK_MATERIAL_LOCK_2>();
                return Results;
            }

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;
                        int cnt = 0;
                        var q = (from md in context.ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL
                                 join mh in context.ART_WF_ARTWORK_MATERIAL_LOCK on md.MATERIAL_LOCK_ID equals mh.MATERIAL_LOCK_ID
                                 //join s in context.SAP_M_ORDER_BOM on new { X1 = md.MATERIAL_NO, X2 = md.PRODUCT_CODE }
                                 //                                       equals new { X1 = s.MATERIAL_NUMBER, X2 = s.MATERIAL } into u1
                                 //from f1 in u1.DefaultIfEmpty()
                                 //join s in context.SAP_M_ORDER_BOM on new { X1 = md.MATERIAL_NO, X2 = md.PRODUCT_CODE }
                                 //           equals new { X1 = s.MATERIAL_NUMBER, X2 = s.MATERIAL }
                                 where md.CREATE_BY == -1
                                 select new ART_WF_ARTWORK_MATERIAL_LOCK_2
                                 {
                                     MATERIAL_LOCK_ID = mh.MATERIAL_LOCK_ID,
                                     REMARK_UNLOCK = mh.REMARK_UNLOCK,
                                     REMARK_LOCK = mh.REMARK_LOCK,
                                     SOLD_TO = md.SOLD_TO,
                                     SHIP_TO = md.SHIP_TO,
                                     MATERIAL_NO = md.MATERIAL_NO,
                                     BRAND = md.BRAND,
                                     COUNTRY = md.COUNTRY,
                                     PIC = mh.PIC,
                                     ZONE = md.ZONE,
                                     STATUS = mh.STATUS,
                                     PRODUCT_CODE = md.PRODUCT_CODE
                                 });
                      
                        #region "Filter"
                        if (!String.IsNullOrEmpty(param.data.SEARCH_SOLD_TO))
                        {
                            List<string> listSold = new List<string>();
                            listSold = param.data.SEARCH_SOLD_TO.Split(':').ToList();

                            if (listSold.Count > 0)
                            {
                                string soldNO = listSold[0];
                                q = q.Where(w => w.SOLD_TO.StartsWith(soldNO));
                            }
                        }

                        if (!String.IsNullOrEmpty(param.data.SEARCH_SHIP_TO))
                        {
                            List<string> listShip = new List<string>();
                            listShip = param.data.SEARCH_SHIP_TO.Split(':').ToList();

                            if (listShip.Count > 0)
                            {
                                string shipNO = listShip[0];
                                q = q.Where(w => w.SHIP_TO.StartsWith(shipNO));
                            }
                        }

                        if (!string.IsNullOrEmpty(param.data.SEARCH_MATERIAL_NO))
                        {
                            var arrMat = param.data.SEARCH_MATERIAL_NO.Split(',');

                            var resultMat = (from e in arrMat
                                             where !String.IsNullOrEmpty(e)
                                             select e).ToList();

                            if (resultMat != null)
                            {
                                q = q.Where(m => resultMat.Contains(m.MATERIAL_NO));
                            }
                        }

                        if (!String.IsNullOrEmpty(param.data.SEARCH_BRAND))
                        {
                            //q = q.Where(w => w.BRAND == param.data.SEARCH_BRAND);
                            var tempBrand = param.data.SEARCH_BRAND.Split(':')[0];
                            q = q.Where(w => w.MATERIAL_NO.Substring(2, 3) == tempBrand);
                        }

                        if (!String.IsNullOrEmpty(param.data.SEARCH_COUNTRY))
                        {
                            var arrCountry = param.data.SEARCH_COUNTRY.Split(',');

                            var resultCountry = (from e in arrCountry
                                                 where !String.IsNullOrEmpty(e)
                                                 select e).ToList();

                            if (resultCountry != null)
                            {
                                q = q.Where(m => resultCountry.Contains(m.COUNTRY));
                            }
                        }

                        if (!String.IsNullOrEmpty(param.data.SEARCH_PIC))
                        {
                            var arrName = param.data.SEARCH_PIC.Split(' ');
                            var i = ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { FIRST_NAME = arrName[1] }, context).FirstOrDefault().USERNAME;
                            q = q.Where(w => w.PIC == i);
                        }

                        if (!String.IsNullOrEmpty(param.data.SEARCH_ZONE))
                        {
                            q = q.Where(w => w.ZONE == param.data.SEARCH_ZONE);
                        }

                        if (!String.IsNullOrEmpty(param.data.SEARCH_PRODUCT_CODE))
                        {
                            var arrProduct = param.data.SEARCH_PRODUCT_CODE.Split(',');

                            var resultProduct = (from e in arrProduct
                                                 where !String.IsNullOrEmpty(e)
                                                 select e).ToList();

                            if (resultProduct != null)
                            {
                                q = q.Where(m => resultProduct.Contains(m.PRODUCT_CODE));
                            }
                        }

                        if (!String.IsNullOrEmpty(param.data.SEARCH_STATUS) && !param.data.SEARCH_STATUS.Equals("ALL"))
                        {
                            q = q.Where(w => w.STATUS == param.data.SEARCH_STATUS);
                        }

                        if (!String.IsNullOrEmpty(param.data.SEARCH_PKG_TYPE))
                        {
                            var arrMat_ = param.data.SEARCH_PKG_TYPE.Split(',');

                            var resultMat5 = (from e in arrMat_
                                              where !String.IsNullOrEmpty(e)
                                              select e).ToList();

                            if (resultMat5 != null)
                            {
                                q = q.Where(m => resultMat5.Contains(m.MATERIAL_NO.Substring(0, 2)));
                            }
                        }

                        if (!String.IsNullOrEmpty(param.data.REMARK_LOCK))
                        {
                            q = q.Where(w => w.REMARK_LOCK.Contains(param.data.REMARK_LOCK));
                        }

                        if (!String.IsNullOrEmpty(param.data.REMARK_UNLOCK))
                        {
                            q = q.Where(w => w.REMARK_UNLOCK.Contains(param.data.REMARK_UNLOCK));
                        }

                        #endregion

                        List<ART_WF_ARTWORK_MATERIAL_LOCK_2> listMatLockResults = new List<ART_WF_ARTWORK_MATERIAL_LOCK_2>();

                        string GENERATE_EXCEL_BY_PARAM = param.data.GENERATE_EXCEL;
                        List<string> matResults = new List<string>();
                        List<string> productcodeResults = new List<string>();
                        List<string> soldtoResults = new List<string>();
                        List<string> shiptoResults = new List<string>();

                        param.data.GENERATE_EXCEL = "X";
                        listMatLockResults = OrderByMaterialLockReport(q, param);
                        bool filter = false;
                        if (param.columns != null)
                        {
                            if (!string.IsNullOrEmpty(param.columns[2].search.value))
                            {
                                filter = true;
                            }
                        }

                        if (filter)
                        {
                            listMatLockResults = FilterDataMatlock(listMatLockResults, param, ref cnt);
                            Results.recordsFiltered = cnt;
                        }

                        if (GENERATE_EXCEL_BY_PARAM == "X")
                        {
                            matResults = listMatLockResults.Select(m => m.MATERIAL_NO).Distinct().ToList();
                            productcodeResults = listMatLockResults.Select(m => m.PRODUCT_CODE).Distinct().ToList();
                            soldtoResults = listMatLockResults.Select(m => m.SOLD_TO).Distinct().ToList();
                            shiptoResults = listMatLockResults.Select(m => m.SHIP_TO).Distinct().ToList();
                        }
                        else
                        {
                            matResults = listMatLockResults.Select(m => m.MATERIAL_NO).Distinct().Skip(param.start).Take(param.length).ToList();
                            productcodeResults = listMatLockResults.Select(m => m.PRODUCT_CODE).Distinct().Skip(param.start).Take(param.length).ToList();
                            soldtoResults = listMatLockResults.Select(m => m.SOLD_TO).Distinct().Skip(param.start).Take(param.length).ToList();
                            shiptoResults = listMatLockResults.Select(m => m.SHIP_TO).Distinct().Skip(param.start).Take(param.length).ToList();
                        }

                        if (matResults != null)
                        {
                            //var q_log = (from m1 in context.ART_WF_ARTWORK_MATERIAL_LOCK_LOG
                            //             group m1 by m1.MATERIAL_LOCK_ID into m2
                            //             let max_m = m2.Max(x => x.UPDATE_DATE)
                            //             select new ART_WF_ARTWORK_MATERIAL_LOCK_LOG_2
                            //             { MATERIAL_LOCK_ID = m2.Key, UPDATE_DATE = max_m });

                        
                            

                            IQueryable<ART_WF_ARTWORK_MATERIAL_LOCK_2> q2 = SearchMatNoResults(param, context, ref q, matResults);
                            var qq = (from q3 in q select q3.MATERIAL_NO).Distinct().ToList();

                            

                            cnt = qq.Count();

                            if (cnt > 0)
                            {
                                listMatLockResults = new List<ART_WF_ARTWORK_MATERIAL_LOCK_2>();
                                listMatLockResults = OrderByMaterialLockReport(q2, param);
                            }
                        }

                        Results.recordsTotal = cnt;
                        if (!filter)
                        {
                            Results.recordsFiltered = cnt;
                        }

                        // List<ART_WF_ARTWORK_MATERIAL_LOCK_2> listMatLockResults_2 = new List<ART_WF_ARTWORK_MATERIAL_LOCK_2>();
                        if (listMatLockResults != null)
                        {
                            var listMatNo = listMatLockResults.Select(s => s.MATERIAL_NO).Distinct().ToList();
                            foreach (var itemMatNo in listMatNo)
                            {
                                if (itemMatNo != null)
                                {
                                    CNService.updateIS_HAS_FILES_WITH_CHECK(itemMatNo);
                                }
                            }

                            IQueryable<ART_WF_ARTWORK_MATERIAL_LOCK_2> q2 = SearchMatLockResults(param, context, ref q, matResults);
                            var qq = (from q3 in q select q3.MATERIAL_NO).Distinct().ToList();

                            cnt = qq.Count();

                            if (cnt > 0)
                            {
                                listMatLockResults = new List<ART_WF_ARTWORK_MATERIAL_LOCK_2>();
                                listMatLockResults = OrderByMaterialLockReport(q2, param);
                            }
                        }


                        Results.data = listMatLockResults;
                        Results.status = "S";
                        Results.draw = param.draw;
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

        private static IQueryable<ART_WF_ARTWORK_MATERIAL_LOCK_2> SearchMatNoResults(ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST param, ARTWORKEntities context, ref IQueryable<ART_WF_ARTWORK_MATERIAL_LOCK_2> q, List<string> matResults)
        {
            var q2 = ((from mh in context.ART_WF_ARTWORK_MATERIAL_LOCK
                       join md in context.ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL on mh.MATERIAL_LOCK_ID equals md.MATERIAL_LOCK_ID
                       join _so in context.V_SAP_SALES_ORDER on new { X1 = mh.MATERIAL_NO, X2 = md.PRODUCT_CODE }
                                                                equals new { X1 = _so.COMPONENT_MATERIAL, X2 = _so.PRODUCT_CODE }
                        into s
                       from so in s.DefaultIfEmpty() 
                       join uPA in context.ART_M_USER on mh.PIC equals uPA.USERNAME into u1
                       from f1 in u1.DefaultIfEmpty()
                       join uPG in context.ART_M_USER on mh.PG_OWNER equals uPG.USERNAME into u2
                       from f2 in u2.DefaultIfEmpty() 
                       where matResults.Contains(mh.MATERIAL_NO) 
                       && so.BOM_IS_ACTIVE == "X"
                       && so.SO_ITEM_IS_ACTIVE == "X"
                       && so.PRODUCT_CODE.StartsWith("3")
                       select new ART_WF_ARTWORK_MATERIAL_LOCK_2
                       {
                           MATERIAL_LOCK_ID = mh.MATERIAL_LOCK_ID,
                           REMARK_UNLOCK = mh.REMARK_UNLOCK,
                           REMARK_LOCK = mh.REMARK_LOCK,
                           SOLD_TO = md.SOLD_TO,
                           SHIP_TO = md.SHIP_TO,
                           MATERIAL_NO = md.MATERIAL_NO,
                           BRAND = md.BRAND,
                           COUNTRY = md.COUNTRY,
                           PIC = mh.PIC,
                           ZONE = md.ZONE,
                           STATUS = mh.STATUS,
                           PRODUCT_CODE = md.PRODUCT_CODE
                       })
                      .Union(from mh in context.ART_WF_ARTWORK_MATERIAL_LOCK
                             join md in context.ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL on mh.MATERIAL_LOCK_ID equals md.MATERIAL_LOCK_ID
                             join uPA in context.ART_M_USER on mh.PIC equals uPA.USERNAME into u1
                             from f1 in u1.DefaultIfEmpty()
                             join uPG in context.ART_M_USER on mh.PG_OWNER equals uPG.USERNAME into u2
                             from f2 in u2.DefaultIfEmpty()
                             where matResults.Contains(mh.MATERIAL_NO)
                             && md.SALES_ORDER_NO == "000"
                             select new ART_WF_ARTWORK_MATERIAL_LOCK_2
                             {
                                 MATERIAL_LOCK_ID = mh.MATERIAL_LOCK_ID,
                                 REMARK_UNLOCK = mh.REMARK_UNLOCK,
                                 REMARK_LOCK = mh.REMARK_LOCK,
                                 SOLD_TO = md.SOLD_TO,
                                 SHIP_TO = md.SHIP_TO,
                                 MATERIAL_NO = md.MATERIAL_NO,
                                 BRAND = md.BRAND,
                                 COUNTRY = md.COUNTRY,
                                 PIC = mh.PIC,
                                 ZONE = md.ZONE,
                                 STATUS = mh.STATUS,
                                 PRODUCT_CODE = md.PRODUCT_CODE
                             })
                      .Union((from mh in context.ART_WF_ARTWORK_MATERIAL_LOCK
                              join md in context.ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL on mh.MATERIAL_LOCK_ID equals md.MATERIAL_LOCK_ID
                              join so in context.V_SAP_SALES_ORDER on new { X1 = md.SALES_ORDER_NO, X2 = md.PRODUCT_CODE } equals new { X1 = so.SALES_ORDER_NO, X2 = so.PRODUCT_CODE }
                               
                              join uPA in context.ART_M_USER on mh.PIC equals uPA.USERNAME into u1
                              from f1 in u1.DefaultIfEmpty()
                              join uPG in context.ART_M_USER on mh.PG_OWNER equals uPG.USERNAME into u2
                              from f2 in u2.DefaultIfEmpty()
                              where matResults.Contains(mh.MATERIAL_NO) 
                              && so.BOM_IS_ACTIVE == "X"
                              && so.SO_ITEM_IS_ACTIVE == "X"
                              && so.PRODUCT_CODE.StartsWith("3")
                              select new ART_WF_ARTWORK_MATERIAL_LOCK_2
                              {
                                  MATERIAL_LOCK_ID = mh.MATERIAL_LOCK_ID,
                                  REMARK_UNLOCK = mh.REMARK_UNLOCK,
                                  REMARK_LOCK = mh.REMARK_LOCK,
                                  SOLD_TO = md.SOLD_TO,
                                  SHIP_TO = md.SHIP_TO,
                                  MATERIAL_NO = md.MATERIAL_NO,
                                  BRAND = md.BRAND,
                                  COUNTRY = md.COUNTRY,
                                  PIC = mh.PIC,
                                  ZONE = md.ZONE,
                                  STATUS = mh.STATUS,
                                  PRODUCT_CODE = md.PRODUCT_CODE
                              }
                              )
                      )
                      );


            #region "Filter"
            if (!String.IsNullOrEmpty(param.data.SEARCH_SOLD_TO))
            {
                List<string> listSold = new List<string>();
                listSold = param.data.SEARCH_SOLD_TO.Split(':').ToList();

                if (listSold.Count > 0)
                {
                    string soldNO = listSold[0];
                    q2 = q2.Where(w => w.SOLD_TO.StartsWith(soldNO));
                }
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_SHIP_TO))
            {
                List<string> listShip = new List<string>();
                listShip = param.data.SEARCH_SHIP_TO.Split(':').ToList();

                if (listShip.Count > 0)
                {
                    string shipNO = listShip[0];
                    q2 = q2.Where(w => w.SHIP_TO.StartsWith(shipNO));
                }
            }

            if (!string.IsNullOrEmpty(param.data.SEARCH_MATERIAL_NO))
            {
                var arrMat = param.data.SEARCH_MATERIAL_NO.Split(',');

                var resultMat = (from e in arrMat
                                 where !String.IsNullOrEmpty(e)
                                 select e).ToList();

                if (resultMat != null)
                {
                    q2 = q2.Where(m => resultMat.Contains(m.MATERIAL_NO));
                }
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_BRAND))
            {
                //q2 = q2.Where(w => w.BRAND == param.data.SEARCH_BRAND);
                var tempBrand = param.data.SEARCH_BRAND.Split(':')[0];
                q = q.Where(w => w.MATERIAL_NO.Substring(2, 3) == tempBrand);
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_COUNTRY))
            {
                var arrCountry = param.data.SEARCH_COUNTRY.Split(',');

                var resultCountry = (from e in arrCountry
                                     where !String.IsNullOrEmpty(e)
                                     select e).ToList();

                if (resultCountry != null)
                {
                    q2 = q2.Where(m => resultCountry.Contains(m.COUNTRY));
                }
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_PIC))
            {
                var arrName = param.data.SEARCH_PIC.Split(' ');
                var i = ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { FIRST_NAME = arrName[1] }, context).FirstOrDefault().USERNAME;
                q2 = q2.Where(w => w.PIC == i);
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_ZONE))
            {
                q2 = q2.Where(w => w.ZONE == param.data.SEARCH_ZONE);
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_PRODUCT_CODE))
            {
                var arrProduct = param.data.SEARCH_PRODUCT_CODE.Split(',');

                var resultProduct = (from e in arrProduct
                                     where !String.IsNullOrEmpty(e)
                                     select e).ToList();

                if (resultProduct != null)
                {
                    q2 = q2.Where(m => resultProduct.Contains(m.PRODUCT_CODE));
                }
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_STATUS) && !param.data.SEARCH_STATUS.Equals("ALL"))
            {
                q2 = q2.Where(w => w.STATUS == param.data.SEARCH_STATUS);
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_PKG_TYPE))
            {
                var arrMat_ = param.data.SEARCH_PKG_TYPE.Split(',');

                var resultMat5 = (from e in arrMat_
                                  where !String.IsNullOrEmpty(e)
                                  select e).ToList();

                if (resultMat5 != null)
                {
                    q2 = q2.Where(m => resultMat5.Contains(m.MATERIAL_NO.Substring(0, 2)));
                }
            }

            if (!String.IsNullOrEmpty(param.data.REMARK_LOCK))
            {
                q2 = q2.Where(w => w.REMARK_LOCK.Contains(param.data.REMARK_LOCK));
            }

            if (!String.IsNullOrEmpty(param.data.REMARK_UNLOCK))
            {
                q2 = q2.Where(w => w.REMARK_UNLOCK.Contains(param.data.REMARK_UNLOCK));
            }

            #endregion
            return q2;
        }

        private static IQueryable<ART_WF_ARTWORK_MATERIAL_LOCK_2> SearchMatLockResults(ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST param, ARTWORKEntities context, ref IQueryable<ART_WF_ARTWORK_MATERIAL_LOCK_2> q, List<string> matResults)
        {
            var q2 = ((from mh in context.ART_WF_ARTWORK_MATERIAL_LOCK
                       join md in context.ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL on mh.MATERIAL_LOCK_ID equals md.MATERIAL_LOCK_ID
                       join _so in context.V_SAP_SALES_ORDER on new { X1 = mh.MATERIAL_NO, X2 = md.PRODUCT_CODE }
                                                                equals new { X1 = _so.COMPONENT_MATERIAL, X2 = _so.PRODUCT_CODE }
                        into s
                       from so in s.DefaultIfEmpty()
                           //join so in context.V_SAP_SALES_ORDER on mh.MATERIAL_NO equals so.COMPONENT_MATERIAL  &&  md.PRODUCT_CODE equals so.PRODUCT_CODE
                           // join so2 in context.V_SAP_SALES_ORDER on md.PRODUCT_CODE equals so2.PRODUCT_CODE
                           //join mg in q_log on mh.MATERIAL_LOCK_ID equals mg.MATERIAL_LOCK_ID into u
                           //from f in u.DefaultIfEmpty()
                       join uPA in context.ART_M_USER on mh.PIC equals uPA.USERNAME into u1
                       from f1 in u1.DefaultIfEmpty()
                       join uPG in context.ART_M_USER on mh.PG_OWNER equals uPG.USERNAME into u2
                       from f2 in u2.DefaultIfEmpty()
                           //join s in context.SAP_M_ORDER_BOM on new { X1 = md.MATERIAL_NO, X2 = md.PRODUCT_CODE }
                           //                              equals new { X1 = s.MATERIAL_NUMBER, X2 = s.MATERIAL } into s1
                           //from s2 in s1.DefaultIfEmpty()
                           //join s in context.V_SAP_SALES_ORDER_ALL on new { X1 = md.MATERIAL_NO }
                           //          equals new { X1 = s.COMPONENT_MATERIAL }
                           //where md.SOLD_TO.Contains(s2.SOLD_TO_PARTY.Substring(2, 8)) && md.SHIP_TO.Contains(s2.SHIP_TO_PARTY.Substring(2, 8))
                           //where s2.SOLD_TO_PARTY.Contains(md.SOLD_TO) && s2.SHIP_TO_PARTY.Contains(md.SHIP_TO)
                       where matResults.Contains(mh.MATERIAL_NO)
                       //&& md.CREATE_BY == -1
                       && so.BOM_IS_ACTIVE == "X"
                       && so.SO_ITEM_IS_ACTIVE == "X"
                       && so.PRODUCT_CODE.StartsWith("3")
                       select new ART_WF_ARTWORK_MATERIAL_LOCK_2
                       {
                           MATERIAL_LOCK_ID = mh.MATERIAL_LOCK_ID,
                           SOLD_TO = so.SOLD_TO + ":" + so.SOLD_TO_NAME,
                           SHIP_TO = so.SHIP_TO + ":" + so.SHIP_TO_NAME,
                           MATERIAL_NO = mh.MATERIAL_NO,
                           MATERIAL_DESCRIPTION = mh.MATERIAL_DESCRIPTION,
                           REMARK_UNLOCK = mh.REMARK_UNLOCK,
                           REMARK_LOCK = mh.REMARK_LOCK,
                           LOG_DATE = mh.UPDATE_DATE,
                           //LOG_DATE = mh.UPDATE_DATE != null ? mh.UPDATE_DATE : default(DateTime),
                           //BRAND = md.BRAND,
                           BRAND = so.BRAND_ID + ":" + so.BRAND_DESCRIPTION,
                           //BRAND = (from mm in context.SAP_M_ORDER_BOM
                           //         join mm2 in context.SAP_M_BRAND on mm.BRAND_ID equals mm2.MATERIAL_GROUP
                           //         where mm.MATERIAL_NUMBER == mh.MATERIAL_NO
                           //         select mm2.MATERIAL_GROUP + ":" + mm2.DESCRIPTION).FirstOrDefault(),
                           COUNTRY = string.IsNullOrEmpty(so.ZONE) ? "" : (from mm in context.SAP_M_COUNTRY
                                                                           where mm.COUNTRY_CODE == so.ZONE.Substring(2, 2)
                                                                           select mm.NAME).FirstOrDefault(),

                           PIC = mh.PIC,
                           ZONE = string.IsNullOrEmpty(so.ZONE) ? "" : so.ZONE.Substring(0, 2),
                           STATUS = mh.STATUS,
                           IS_HAS_FILES = mh.IS_HAS_FILES,
                           //PRODUCT_CODE = (from mm in context.V_SAP_M_ORDER_BOM
                           //                where mm.MATERIAL_NUMBER == md.MATERIAL_NO
                           //                && mm.LAST_UPDATE == "X"
                           //                && mm.CHANGE_TYPE != "D"
                           //                select mm).OrderByDescending(o => o.COUNTER).ToList().Select(s => s.MATERIAL).FirstOrDefault(),

                           //PRODUCT_CODE = (from mm in context.V_SAP_SALES_ORDER
                           //                where mm.COMPONENT_MATERIAL == md.MATERIAL_NO
                           //                select mm.PRODUCT_CODE).FirstOrDefault(),
                           PRODUCT_CODE = so.PRODUCT_CODE,
                           GROUPING = mh.MATERIAL_NO,
                           STATUS_DISPLAY_TXT = mh.STATUS == "I" ? "In use" : "Obsolete",
                           IS_HAS_FILES_DISPLAY_TXT = mh.IS_HAS_FILES == "X" ? "Yes" : "No",
                           REQUEST_FORM_NO = mh.REQUEST_FORM_NO,
                           ARTWORK_NO = mh.ARTWORK_NO,
                           MOCKUP_NO = mh.MOCKUP_NO,
                           REQUEST_FORM_ID = mh.REQUEST_FORM_ID,
                           ARTWORK_ID = mh.ARTWORK_ID,
                           MOCKUP_ID = mh.MOCKUP_ID,
                           UNLOCK_DATE_FROM = mh.UNLOCK_DATE_FROM,
                           UNLOCK_DATE_TO = mh.UNLOCK_DATE_TO,
                           PACKAGING_STYLE = mh.PACKAGING_STYLE,
                           PACKAGING_TYPE = mh.PACKAGING_TYPE,
                           PRIMARY_SIZE = mh.PRIMARY_SIZE,
                           PACK_SIZE = mh.PACK_SIZE,
                           PRIMARY_TYPE = mh.PRIMARY_TYPE,
                           PG_OWNER = mh.PG_OWNER,
                           PG_OWNER_DISPLAY_TXT = (f2.TITLE + " " + f2.FIRST_NAME + " " + f2.LAST_NAME).Trim(),
                           PIC_DISPLAY_TXT = (f1.TITLE + " " + f1.FIRST_NAME + " " + f1.LAST_NAME).Trim()//,
                                                                                                         //SALES_ORDER_NO = so.SALES_ORDER_NO
                       })
                      .Union(from mh in context.ART_WF_ARTWORK_MATERIAL_LOCK
                             join md in context.ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL on mh.MATERIAL_LOCK_ID equals md.MATERIAL_LOCK_ID
                             join uPA in context.ART_M_USER on mh.PIC equals uPA.USERNAME into u1
                             from f1 in u1.DefaultIfEmpty()
                             join uPG in context.ART_M_USER on mh.PG_OWNER equals uPG.USERNAME into u2
                             from f2 in u2.DefaultIfEmpty()
                             where matResults.Contains(mh.MATERIAL_NO)
                             && md.SALES_ORDER_NO == "000"
                             select new ART_WF_ARTWORK_MATERIAL_LOCK_2
                             {
                                 MATERIAL_LOCK_ID = mh.MATERIAL_LOCK_ID,
                                 SOLD_TO = "",//md.SOLD_TO,
                                 SHIP_TO = "", //md.SHIP_TO,
                                 MATERIAL_NO = mh.MATERIAL_NO,
                                 MATERIAL_DESCRIPTION = mh.MATERIAL_DESCRIPTION,
                                 REMARK_UNLOCK = mh.REMARK_UNLOCK,
                                 REMARK_LOCK = mh.REMARK_LOCK,
                                 LOG_DATE = mh.UPDATE_DATE,
                                 BRAND = "", //(from mm in context.SAP_M_ORDER_BOM
                                             //join mm2 in context.SAP_M_BRAND on mm.BRAND_ID equals mm2.MATERIAL_GROUP
                                             //where mm.MATERIAL_NUMBER == mh.MATERIAL_NO
                                             //select mm2.MATERIAL_GROUP + ":" + mm2.DESCRIPTION).FirstOrDefault(),
                                 COUNTRY = "", // md.COUNTRY,
                                 PIC = mh.PIC,
                                 ZONE = "", //md.ZONE,
                                 STATUS = mh.STATUS,
                                 IS_HAS_FILES = mh.IS_HAS_FILES,
                                 PRODUCT_CODE = "", // md.PRODUCT_CODE,
                                 GROUPING = mh.MATERIAL_NO,
                                 STATUS_DISPLAY_TXT = mh.STATUS == "I" ? "In use" : "Obsolete",
                                 IS_HAS_FILES_DISPLAY_TXT = mh.IS_HAS_FILES == "X" ? "Yes" : "No",
                                 REQUEST_FORM_NO = mh.REQUEST_FORM_NO,
                                 ARTWORK_NO = mh.ARTWORK_NO,
                                 MOCKUP_NO = mh.MOCKUP_NO,
                                 REQUEST_FORM_ID = mh.REQUEST_FORM_ID,
                                 ARTWORK_ID = mh.ARTWORK_ID,
                                 MOCKUP_ID = mh.MOCKUP_ID,
                                 UNLOCK_DATE_FROM = mh.UNLOCK_DATE_FROM,
                                 UNLOCK_DATE_TO = mh.UNLOCK_DATE_TO,
                                 PACKAGING_STYLE = mh.PACKAGING_STYLE,
                                 PACKAGING_TYPE = mh.PACKAGING_TYPE,
                                 PRIMARY_SIZE = mh.PRIMARY_SIZE,
                                 PACK_SIZE = mh.PACK_SIZE,
                                 PRIMARY_TYPE = mh.PRIMARY_TYPE,
                                 PG_OWNER = mh.PG_OWNER,
                                 PG_OWNER_DISPLAY_TXT = (f2.TITLE + " " + f2.FIRST_NAME + " " + f2.LAST_NAME).Trim(),
                                 PIC_DISPLAY_TXT = (f1.TITLE + " " + f1.FIRST_NAME + " " + f1.LAST_NAME).Trim()//,
                                                                                                               //SALES_ORDER_NO = "000"
                             })
                      .Union((from mh in context.ART_WF_ARTWORK_MATERIAL_LOCK
                              join md in context.ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL on mh.MATERIAL_LOCK_ID equals md.MATERIAL_LOCK_ID
                              join so in context.V_SAP_SALES_ORDER on new { X1 = md.SALES_ORDER_NO, X2 = md.PRODUCT_CODE } equals new { X1 = so.SALES_ORDER_NO, X2 = so.PRODUCT_CODE }

                              //join _so in context.V_SAP_SALES_ORDER on new { X1 = md.SALES_ORDER_NO, X2 = md.PRODUCT_CODE } equals new { X1 = _so.SALES_ORDER_NO, X2 = _so.PRODUCT_CODE }
                              // into s
                              //from so in s.DefaultIfEmpty()
                              join uPA in context.ART_M_USER on mh.PIC equals uPA.USERNAME into u1
                              from f1 in u1.DefaultIfEmpty()
                              join uPG in context.ART_M_USER on mh.PG_OWNER equals uPG.USERNAME into u2
                              from f2 in u2.DefaultIfEmpty()
                              where matResults.Contains(mh.MATERIAL_NO)
                              //&& md.CREATE_BY == -1
                              && so.BOM_IS_ACTIVE == "X"
                              && so.SO_ITEM_IS_ACTIVE == "X"
                              && so.PRODUCT_CODE.StartsWith("3")
                              select new ART_WF_ARTWORK_MATERIAL_LOCK_2
                              {
                                  MATERIAL_LOCK_ID = mh.MATERIAL_LOCK_ID,
                                  SOLD_TO = "", // md.SOLD_TO,
                                  SHIP_TO = "", //md.SHIP_TO,
                                  MATERIAL_NO = mh.MATERIAL_NO,
                                  MATERIAL_DESCRIPTION = mh.MATERIAL_DESCRIPTION,
                                  REMARK_UNLOCK = mh.REMARK_UNLOCK,
                                  REMARK_LOCK = mh.REMARK_LOCK,
                                  LOG_DATE = mh.UPDATE_DATE,
                                  BRAND = "",
                                  COUNTRY = "", //md.COUNTRY,
                                  PIC = mh.PIC,
                                  ZONE = "", //md.ZONE,
                                  STATUS = mh.STATUS,
                                  IS_HAS_FILES = mh.IS_HAS_FILES,
                                  PRODUCT_CODE = "", //md.PRODUCT_CODE,
                                  GROUPING = mh.MATERIAL_NO,
                                  STATUS_DISPLAY_TXT = mh.STATUS == "I" ? "In use" : "Obsolete",
                                  IS_HAS_FILES_DISPLAY_TXT = mh.IS_HAS_FILES == "X" ? "Yes" : "No",
                                  REQUEST_FORM_NO = mh.REQUEST_FORM_NO,
                                  ARTWORK_NO = mh.ARTWORK_NO,
                                  MOCKUP_NO = mh.MOCKUP_NO,
                                  REQUEST_FORM_ID = mh.REQUEST_FORM_ID,
                                  ARTWORK_ID = mh.ARTWORK_ID,
                                  MOCKUP_ID = mh.MOCKUP_ID,
                                  UNLOCK_DATE_FROM = mh.UNLOCK_DATE_FROM,
                                  UNLOCK_DATE_TO = mh.UNLOCK_DATE_TO,
                                  PACKAGING_STYLE = mh.PACKAGING_STYLE,
                                  PACKAGING_TYPE = mh.PACKAGING_TYPE,
                                  PRIMARY_SIZE = mh.PRIMARY_SIZE,
                                  PACK_SIZE = mh.PACK_SIZE,
                                  PRIMARY_TYPE = mh.PRIMARY_TYPE,
                                  PG_OWNER = mh.PG_OWNER,
                                  PG_OWNER_DISPLAY_TXT = (f2.TITLE + " " + f2.FIRST_NAME + " " + f2.LAST_NAME).Trim(),
                                  PIC_DISPLAY_TXT = (f1.TITLE + " " + f1.FIRST_NAME + " " + f1.LAST_NAME).Trim()//,
                                                                                                                //SALES_ORDER_NO = "000"
                              }
                              )
                      )
                      );


            #region "Filter"
            if (!String.IsNullOrEmpty(param.data.SEARCH_SOLD_TO))
            {
                List<string> listSold = new List<string>();
                listSold = param.data.SEARCH_SOLD_TO.Split(':').ToList();

                if (listSold.Count > 0)
                {
                    string soldNO = listSold[0];
                    q2 = q2.Where(w => w.SOLD_TO.StartsWith(soldNO));
                }
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_SHIP_TO))
            {
                List<string> listShip = new List<string>();
                listShip = param.data.SEARCH_SHIP_TO.Split(':').ToList();

                if (listShip.Count > 0)
                {
                    string shipNO = listShip[0];
                    q2 = q2.Where(w => w.SHIP_TO.StartsWith(shipNO));
                }
            }

            if (!string.IsNullOrEmpty(param.data.SEARCH_MATERIAL_NO))
            {
                var arrMat = param.data.SEARCH_MATERIAL_NO.Split(',');

                var resultMat = (from e in arrMat
                                 where !String.IsNullOrEmpty(e)
                                 select e).ToList();

                if (resultMat != null)
                {
                    q2 = q2.Where(m => resultMat.Contains(m.MATERIAL_NO));
                }
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_BRAND))
            {
                //q2 = q2.Where(w => w.BRAND == param.data.SEARCH_BRAND);
                var tempBrand = param.data.SEARCH_BRAND.Split(':')[0];
                q = q.Where(w => w.MATERIAL_NO.Substring(2, 3) == tempBrand);
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_COUNTRY))
            {
                var arrCountry = param.data.SEARCH_COUNTRY.Split(',');

                var resultCountry = (from e in arrCountry
                                     where !String.IsNullOrEmpty(e)
                                     select e).ToList();

                if (resultCountry != null)
                {
                    q2 = q2.Where(m => resultCountry.Contains(m.COUNTRY));
                }
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_PIC))
            {
                var arrName = param.data.SEARCH_PIC.Split(' ');
                var i = ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { FIRST_NAME = arrName[1] }, context).FirstOrDefault().USERNAME;
                q2 = q2.Where(w => w.PIC == i);
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_ZONE))
            {
                q2 = q2.Where(w => w.ZONE == param.data.SEARCH_ZONE);
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_PRODUCT_CODE))
            {
                var arrProduct = param.data.SEARCH_PRODUCT_CODE.Split(',');

                var resultProduct = (from e in arrProduct
                                     where !String.IsNullOrEmpty(e)
                                     select e).ToList();

                if (resultProduct != null)
                {
                    q2 = q2.Where(m => resultProduct.Contains(m.PRODUCT_CODE));
                }
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_STATUS) && !param.data.SEARCH_STATUS.Equals("ALL"))
            {
                q2 = q2.Where(w => w.STATUS == param.data.SEARCH_STATUS);
            }

            if (!String.IsNullOrEmpty(param.data.SEARCH_PKG_TYPE))
            {
                var arrMat_ = param.data.SEARCH_PKG_TYPE.Split(',');

                var resultMat5 = (from e in arrMat_
                                  where !String.IsNullOrEmpty(e)
                                  select e).ToList();

                if (resultMat5 != null)
                {
                    q2 = q2.Where(m => resultMat5.Contains(m.MATERIAL_NO.Substring(0, 2)));
                }
            }

            if (!String.IsNullOrEmpty(param.data.REMARK_LOCK))
            {
                q2 = q2.Where(w => w.REMARK_LOCK.Contains(param.data.REMARK_LOCK));
            }

            if (!String.IsNullOrEmpty(param.data.REMARK_UNLOCK))
            {
                q2 = q2.Where(w => w.REMARK_UNLOCK.Contains(param.data.REMARK_UNLOCK));
            }

            #endregion
            return q2;
        }

        private static List<ART_WF_ARTWORK_MATERIAL_LOCK_2> FilterDataMatlock(List<ART_WF_ARTWORK_MATERIAL_LOCK_2> data, ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST param, ref int cnt)
        {
            var filterValueMat = param.columns[2].search.value;
            var filterValueUnlock = param.columns[11].search.value;
            var filterValueLock = param.columns[12].search.value;

            if (!string.IsNullOrEmpty(filterValueMat))
                data = data.Where(m => !string.IsNullOrEmpty(m.MATERIAL_NO) && m.MATERIAL_NO.ToLower().Contains(filterValueMat.ToLower())).ToList();
            if (!string.IsNullOrEmpty(filterValueLock))
                data = data.Where(m => !string.IsNullOrEmpty(m.REMARK_LOCK) && m.REMARK_LOCK.ToLower().Contains(filterValueLock.ToLower())).ToList();
            if (!string.IsNullOrEmpty(filterValueUnlock))
                data = data.Where(m => !string.IsNullOrEmpty(m.REMARK_UNLOCK) && m.REMARK_UNLOCK.ToLower().Contains(filterValueUnlock.ToLower())).ToList();


            cnt = data.Count();

            return data;
        }

        public static ART_WF_ARTWORK_MATERIAL_LOCK_RESULT UpdateMaterialLockReport(ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST_LIST param)
        {
            ART_WF_ARTWORK_MATERIAL_LOCK_RESULT Results = new ART_WF_ARTWORK_MATERIAL_LOCK_RESULT();
            ART_WF_ARTWORK_MATERIAL_LOCK_LOG log = new ART_WF_ARTWORK_MATERIAL_LOCK_LOG();

            try
            {

                if (param == null || param.data == null || param.data.Count <= 0)
                {
                    Results.status = "E";
                    return Results;
                }

                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        foreach (var iData in param.data)
                        {
                            var dataExist = (from d in context.ART_WF_ARTWORK_MATERIAL_LOCK
                                             where d.MATERIAL_LOCK_ID == iData.MATERIAL_LOCK_ID
                                             select d).FirstOrDefault();

                            if (dataExist != null)
                            {
                                if (!String.IsNullOrEmpty(iData.STATUS))
                                {
                                    dataExist.STATUS = iData.STATUS;
                                }

                                if (iData.UNLOCK_DATE_FROM_PARAM != null)
                                {
                                    dataExist.UNLOCK_DATE_FROM = CNService.ConvertStringToDate2(iData.UNLOCK_DATE_FROM_PARAM);
                                }

                                if (iData.UNLOCK_DATE_TO_PARAM != null)
                                {
                                    dataExist.UNLOCK_DATE_TO = CNService.ConvertStringToDate2(iData.UNLOCK_DATE_TO_PARAM);
                                }


                                // added by aof ticket#440197,440423
                                if (iData.UPDATE_DATE_LOCK_PARAM != null)  
                                {
                                    dataExist.UPDATE_DATE_LOCK = DateTime.ParseExact(iData.UPDATE_DATE_LOCK_PARAM, "yyyy-MM-dd HH:mm:ss", null);
                                    dataExist.UPDATE_BY_LOCK = iData.UPDATE_BY;
                                }
                                // added by aof ticket#440197,440423

                                if (iData.REMARK_UNLOCK != null)
                                    dataExist.REMARK_UNLOCK = iData.REMARK_UNLOCK;
                                if (iData.REMARK_LOCK != null)
                                    dataExist.REMARK_LOCK = iData.REMARK_LOCK;
                                dataExist.UPDATE_BY = iData.UPDATE_BY;

                                ART_WF_ARTWORK_MATERIAL_LOCK_SERVICE.SaveOrUpdateNoLog(dataExist, context);

                                log = new ART_WF_ARTWORK_MATERIAL_LOCK_LOG();
                                log.MATERIAL_LOCK_ID = dataExist.MATERIAL_LOCK_ID;
                                log.CREATE_BY = dataExist.CREATE_BY;
                                log.IS_ACTIVE = dataExist.IS_ACTIVE;
                                log.MATERIAL_NO = dataExist.MATERIAL_NO;
                                log.STATUS = dataExist.STATUS;
                                if (dataExist.STATUS == "I")
                                    log.REMARK_UNLOCK = dataExist.REMARK_UNLOCK;
                                else
                                    log.REMARK_UNLOCK = dataExist.REMARK_LOCK;
                                log.UNLOCK_DATE_FROM = dataExist.UNLOCK_DATE_FROM;
                                log.UNLOCK_DATE_TO = dataExist.UNLOCK_DATE_TO;
                                log.UPDATE_BY = dataExist.UPDATE_BY;

                                ART_WF_ARTWORK_MATERIAL_LOCK_LOG_SERVICE.SaveOrUpdate(log, context);
                            }

                        }

                        dbContextTransaction.Commit();
                    }

                }

              //  Results.SUMMIT_TYPE = param.SUMMIT_TYPE;
                Results.status = "S";
                Results.msg = MessageHelper.GetMessage("MSG_001");
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static List<ART_WF_ARTWORK_MATERIAL_LOCK_2> OrderByMaterialLockReport(IQueryable<ART_WF_ARTWORK_MATERIAL_LOCK_2> q
                                                                                    , ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST param)
        {
            var ResultsMaterialLock = new List<ART_WF_ARTWORK_MATERIAL_LOCK_2>();
            var orderColumn = 2;
            var orderDir = "asc";
            if (param.order != null && param.order.Count > 0)
            {
                orderColumn = param.order[0].column;
                orderDir = param.order[0].dir; //desc ,asc
            }

            if (orderColumn == 1)
            { orderColumn = 2; }
            //if (param.data.IS_SEARCH)
            //{
            //    orderColumn = 2;
            //    orderDir = param.order[0].dir;
            //}

            string orderASC = "asc";
            string orderDESC = "desc";

            if (orderColumn == 2)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.MATERIAL_NO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.MATERIAL_NO).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.MATERIAL_NO).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 3)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.MATERIAL_DESCRIPTION).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.MATERIAL_DESCRIPTION).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.MATERIAL_DESCRIPTION).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 4)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.IS_HAS_FILES_DISPLAY_TXT).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.IS_HAS_FILES_DISPLAY_TXT).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.IS_HAS_FILES_DISPLAY_TXT).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 5)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.STATUS_DISPLAY_TXT).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.STATUS_DISPLAY_TXT).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.STATUS_DISPLAY_TXT).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 6)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.REQUEST_FORM_NO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.REQUEST_FORM_NO).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.REQUEST_FORM_NO).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 7)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.ARTWORK_NO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.ARTWORK_NO).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.ARTWORK_NO).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 8)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.MOCKUP_NO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.MOCKUP_NO).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.MOCKUP_NO).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 9)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.UNLOCK_DATE_FROM).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.UNLOCK_DATE_FROM).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.UNLOCK_DATE_FROM).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 10)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.UNLOCK_DATE_TO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.UNLOCK_DATE_TO).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.UNLOCK_DATE_TO).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 11)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.REMARK_UNLOCK).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.REMARK_UNLOCK).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.REMARK_UNLOCK).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 12)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.REMARK_LOCK).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.REMARK_LOCK).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.REMARK_LOCK).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 13)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.LOG_DATE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.LOG_DATE).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.LOG_DATE).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 14)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.PRODUCT_CODE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.PRODUCT_CODE).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.PRODUCT_CODE).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 15)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.SOLD_TO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.SOLD_TO).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.SOLD_TO).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 16)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.SHIP_TO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.SHIP_TO).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.SHIP_TO).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 17)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.BRAND).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.BRAND).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.BRAND).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 18)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.COUNTRY).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.COUNTRY).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.COUNTRY).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 19)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.ZONE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.ZONE).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.ZONE).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 20)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.PACKAGING_TYPE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.PACKAGING_TYPE).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.PACKAGING_TYPE).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 21)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.PRIMARY_TYPE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.PRIMARY_TYPE).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.PRIMARY_TYPE).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 22)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.PRIMARY_SIZE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.PRIMARY_SIZE).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.PRIMARY_SIZE).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 23)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.PACK_SIZE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.PACK_SIZE).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.PACK_SIZE).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 24)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.PACKAGING_STYLE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.PACKAGING_STYLE).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.PACKAGING_STYLE).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 25)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.PIC_DISPLAY_TXT).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.PIC_DISPLAY_TXT).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.PIC_DISPLAY_TXT).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }

            if (orderColumn == 26)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    ResultsMaterialLock = q.Distinct().OrderBy(i => i.PG_OWNER_DISPLAY_TXT).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderBy(i => i.PG_OWNER_DISPLAY_TXT).Skip(param.start).Take(param.length).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        ResultsMaterialLock = q.Distinct().OrderByDescending(i => i.PG_OWNER_DISPLAY_TXT).Skip(param.start).Take(param.length).ToList();
                    }
                }
            }
            return ResultsMaterialLock.Distinct().ToList();
        }

    }
}


