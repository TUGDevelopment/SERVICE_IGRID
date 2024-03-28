using BLL.DocumentManagement;
using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace BLL.Helpers
{
    public class WarehouseReportHelper
    {


        public static List<V_ART_WAREHOUSE_REPORT_2> GetWarehouseReportV2_List(V_ART_WAREHOUSE_REPORT_REQUEST param)
        {

            List<V_ART_WAREHOUSE_REPORT_2> list = new List<V_ART_WAREHOUSE_REPORT_2>();

            try
            {
                var cnt = 0;

                list = QueryWarehouseReport_StoredProcedure(param, ref cnt);   //Ticket 447876

             

                    //  Warehouse = q.OrderBy(i => i.SALES_ORDER_NO)


            }
            catch (Exception ex)
            {
              
            }



            return list;
        }


        public static V_ART_WAREHOUSE_REPORT_RESULT GetWarehouseReportV2(V_ART_WAREHOUSE_REPORT_REQUEST param)
        {

            V_ART_WAREHOUSE_REPORT_RESULT Results = new V_ART_WAREHOUSE_REPORT_RESULT();
            Results.data = new List<V_ART_WAREHOUSE_REPORT_2>();
          

            try
            {
                var cnt = 0;

                var list = QueryWarehouseReport_StoredProcedure(param, ref cnt);   //Ticket 447876



                Results.recordsTotal = cnt;
                Results.recordsFiltered = cnt;

                Results.status = "S";
                Results.data = list;
                Results.draw = param.draw;

            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static V_ART_WAREHOUSE_REPORT_RESULT GetWarehouseReport(V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
            V_ART_WAREHOUSE_REPORT_RESULT Results = new V_ART_WAREHOUSE_REPORT_RESULT();
            Results.data = new List<V_ART_WAREHOUSE_REPORT_2>();

            try
            {
                var cnt = 0;

             
                var listResultAll = QueryWarehouseReport(param, ref cnt);   //Ticket 447876  

                Results.recordsTotal = cnt;
                Results.recordsFiltered = cnt;

                bool filter = false;
                if (param.columns != null)
                {
                    foreach (var item in param.columns)
                    {
                        if (!string.IsNullOrEmpty(item.search.value))
                        {
                            filter = true;
                            break;
                        }
                    }
                }

                if (filter)
                {
                    listResultAll = FilterDataPGView(listResultAll, param, ref cnt);
                    Results.recordsFiltered = cnt;
                }

                var listSO = new List<V_SAP_SALES_ORDER_2>();
                var SO = listResultAll.Select(m => m.SALES_ORDER_NO + "" + m.SALES_ORDER_ITEM).Distinct().ToList();
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        listSO = (from m in context.SAP_M_PO_COMPLETE_SO_HEADER
                                  join m2 in context.SAP_M_PO_COMPLETE_SO_ITEM on m.PO_COMPLETE_SO_HEADER_ID equals m2.PO_COMPLETE_SO_HEADER_ID
                                  where SO.Contains(m.SALES_ORDER_NO + "" + m2.ITEM)
                                  && !string.IsNullOrEmpty(m2.REJECTION_CODE)
                                  select new V_SAP_SALES_ORDER_2()
                                  {
                                      SALES_ORDER_NO = m.SALES_ORDER_NO,
                                      ITEM = m2.ITEM,
                                      REJECTION_CODE = m2.REJECTION_CODE,
                                      REJECTION_DESCRIPTION = m2.REJECTION_DESCRIPTION,
                                  }).ToList();


                        int stepSendPOVendor = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_VN_PO" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        foreach (var item in listResultAll)
                        {
                            if (string.IsNullOrEmpty(param.data.GENERATE_EXCEL))
                            {
                                item.REJECTION_CODE = "";
                                item.REJECTION_DESC = "";
                                var temp = listSO.Where(m => m.SALES_ORDER_NO == item.SALES_ORDER_NO && m.ITEM == item.SALES_ORDER_ITEM).FirstOrDefault();
                                if (temp != null)
                                {
                                    if (!string.IsNullOrEmpty(temp.REJECTION_CODE)) item.REJECTION_CODE = temp.REJECTION_CODE;
                                    if (!string.IsNullOrEmpty(temp.REJECTION_DESCRIPTION)) item.REJECTION_DESC = temp.REJECTION_DESCRIPTION;
                                }

                                //find artwork item id
                                if (item.ARTWORK_SUB_ID != null)
                                {
                                    var artworkItemId = CNService.FindArtworkItemId((int)item.ARTWORK_SUB_ID, context);
                                    if (artworkItemId > 0)
                                    {
                                        var countSendPOVendor = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = artworkItemId, CURRENT_STEP_ID = stepSendPOVendor }, context);
                                        item.IS_SHOW_FILE_PRINT_MASTER = countSendPOVendor.Count > 0 ? true : false;
                                    }
                                }
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



        //start ticket 447876 by aof for tuning QueryWarehouseReport_StoredProcedure

        public static string getSQLWhereByJoinStringWithAnd(string curWhere , string newWhere)
        {
            string retWhere = curWhere;

            if (!string.IsNullOrEmpty(newWhere))
            {
                if (!string.IsNullOrEmpty(retWhere))
                {
                    retWhere += " and (" + newWhere + ")";
                } else
                {
                    retWhere = newWhere;
                }
            }

            return retWhere;
        }

        public static string getWhereSO(V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
            string where_so = "";

            if (!string.IsNullOrEmpty(param.data.SALES_ORDER_NO))
            {
                // where += " AND (SALES_ORDER_NO LIKE '%" + param.data.SALES_ORDER_NO + "%')";
                // where_so += " AND " + getSQLWhereLikeByConvertString(param.data.SALES_ORDER_NO, "SALES_ORDER_NO");
                where_so = getSQLWhereByJoinStringWithAnd(where_so, getSQLWhereLikeByConvertString(param.data.SALES_ORDER_NO, "SALES_ORDER_NO"));
            }

            //if (!string.IsNullOrEmpty(param.data.SALES_ORG))
            //{
            //    //where_so += " AND (SALES_ORG = '" + param.data.SALES_ORG + "')";
            //    where_so = getSQLWhereByJoinStringWithAnd(where_so, "(SALES_ORG = '" + param.data.SALES_ORG + "')");
            //}

            //if (param.data.SALES_ORDER_ITEM != null)
            //{
            //    if (Regex.IsMatch(param.data.SALES_ORDER_ITEM.ToString(), @"^\d+$"))
            //    {
            //        where_so += " AND (SALES_ORDER_ITEM = '" + param.data.SALES_ORDER_ITEM.ToString() + "')";
            //        where_so = getSQLWhereByJoinStringWithAnd(where_so, "(SALES_ORDER_ITEM = '" + param.data.SALES_ORDER_ITEM.ToString() + "')");
            //    }
            //}


            return where_so;
        }

        public static string getWherePO(V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
            string where_po = "";
            if (!string.IsNullOrEmpty(param.data.PURCHASE_ORDER_NO))
            {
                //where += " AND (PURCHASE_ORDER_NO LIKE '%" + param.data.PURCHASE_ORDER_NO + "%')";
                where_po = getSQLWhereByJoinStringWithAnd(where_po, getSQLWhereLikeByConvertString(param.data.PURCHASE_ORDER_NO, "PURCHASE_ORDER_NO"));
            }



            if (param.data.COMPANY_ID != 0 && param.data.COMPANY_ID != null)
            {
              //  q = q.Where(m => m.COMPANY_ID == param.data.COMPANY_ID);
                where_po = getSQLWhereByJoinStringWithAnd(where_po, "(COMPANY_ID = '" + param.data.COMPANY_ID + "')");
            }

            if (!string.IsNullOrEmpty(param.data.DOC_DATE))
            {
                var s = param.data.DOC_DATE.Split('/');
                if (s.Length > 2)
                {
                    var stringDate = s[2] + s[1] + s[0];
                    //  where += " AND (DOC_DATE = '" + stringDate + "')";
                    where_po = getSQLWhereByJoinStringWithAnd(where_po, "(DOC_DATE = '" + stringDate + "')");
                }
            }


            return where_po;
        }

        public static List<V_ART_WAREHOUSE_REPORT_2> QueryWarehouseReport_StoredProcedure(V_ART_WAREHOUSE_REPORT_REQUEST param, ref int cnt)
        {
            List<V_ART_WAREHOUSE_REPORT_2> q;
            // string strSQL = getSQLWarehouseReport_V2(param);
            string where_so = getWhereSO(param);    //"sales_order_no  LIKE '5004029%'";
            string where_po = getWherePO(param);
           
            using (var context = new ARTWORKEntities())
            {
             
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;
                    q = context.Database.SqlQuery<V_ART_WAREHOUSE_REPORT_2>("sp_ART_REPORT_WAREHOUSE @where_so, @where_po", new SqlParameter("@where_so", where_so), new SqlParameter("@where_po", where_po)).ToList();
                    q = FilterQueryWarehouseReport(q.AsQueryable(), param);
                    q = q.OrderBy(x => (x.SALES_ORDER_NO, x.SALES_ORDER_ITEM)).ToList();
                }
            }

            cnt = q.Distinct().Count();
            return q;
        }


        public static string rebuildStringArray(string oldStr)
        {
           string newStr = "";
           if (!string.IsNullOrEmpty(oldStr))
            {
                var arrStr = oldStr.Split(',');
                if (arrStr != null)
                {
                    if (arrStr.Length > 0)
                    {
                        foreach (string s in arrStr)
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                if (string.IsNullOrEmpty(newStr))
                                {
                                    newStr = s;
                                }
                                else
                                {
                                    newStr += "," + s;
                                }
                            }
                        }

                    }
                }
            }
           
                      

            return newStr;
        }

        public static List<V_ART_WAREHOUSE_REPORT_2> FilterQueryWarehouseReport(IQueryable<V_ART_WAREHOUSE_REPORT_2> q, V_ART_WAREHOUSE_REPORT_REQUEST param)
        {


            string temp_SALES_ORDER_NO = rebuildStringArray(param.data.SALES_ORDER_NO);
            if (!string.IsNullOrEmpty(temp_SALES_ORDER_NO))
            {
                var arrSO = temp_SALES_ORDER_NO.Split(',');
                q = q.Where(m => arrSO.Contains(m.SALES_ORDER_NO));
            }

            string temp_PURCHASE_ORDER_NO = rebuildStringArray(param.data.PURCHASE_ORDER_NO);
            if (!string.IsNullOrEmpty(temp_PURCHASE_ORDER_NO))
            {
                var arrPO = temp_PURCHASE_ORDER_NO.Split(',');
                q = q.Where(m => arrPO.Contains(m.PURCHASE_ORDER_NO));
            }

            if (!string.IsNullOrEmpty(param.data.SALES_ORG))
            {
                q = q.Where(m => m.SALES_ORG == param.data.SALES_ORG);
            }

            if (param.data.SALES_ORDER_ITEM != null)
            {
                if (Regex.IsMatch(param.data.SALES_ORDER_ITEM.ToString(), @"^\d+$"))
                {
                    q = q.Where(m => m.SALES_ORDER_ITEM == param.data.SALES_ORDER_ITEM);
                }
            }

            if (param.data.SHIP_TO_ID != null)
            {
                q = q.Where(m => m.SHIP_TO_ID == param.data.SHIP_TO_ID);
            }

            if (param.data.BRAND_ID != 0 && param.data.BRAND_ID != null)
            {
                q = q.Where(m => m.BRAND_ID == param.data.BRAND_ID);
            }

            if (!string.IsNullOrEmpty(param.data.PROJECT_NAME))
            {
                // q = q.Where(m => m.PROJECT_NAME.Contains(param.data.PROJECT_NAME));
                q = q.Where(m => m.PROJECT_NAME.IndexOf(param.data.PROJECT_NAME, StringComparison.OrdinalIgnoreCase) != -1);
            }



          
            if (!string.IsNullOrEmpty(param.data.PURCHASING_ORG))
            {
                q = q.Where(m => m.PURCHASING_ORG == param.data.PURCHASING_ORG);
            }

            if (!string.IsNullOrEmpty(param.data.PO_ITEM_NO))
            {
                q = q.Where(m => m.PO_ITEM_NO== param.data.PO_ITEM_NO.PadLeft(5, '0'));
            }

            if (!string.IsNullOrEmpty(param.data.MATERIAL_CODE))
            {
                q = q.Where(m => m.MATERIAL_CODE == param.data.MATERIAL_CODE);
            }

            if (param.data.COMPANY_ID != 0 && param.data.COMPANY_ID != null)
            {
                q = q.Where(m => m.COMPANY_ID == param.data.COMPANY_ID);
            }

            if (!string.IsNullOrEmpty(param.data.DOC_DATE))
            {
                var s = param.data.DOC_DATE.Split('/');
                if (s.Length > 2)
                {
                    var stringDate = s[2] + s[1] + s[0];
                    q = q.Where(m => m.DOC_DATE.Equals(stringDate));
                }
            }


            return q.ToList();
        }

        #region QueryWarehouseReport_V2
        public static List<V_ART_WAREHOUSE_REPORT_2> QueryWarehouseReport_V2(V_ART_WAREHOUSE_REPORT_REQUEST param, ref int cnt)
        {
            List<V_ART_WAREHOUSE_REPORT_2> q;
            string strSQL = getSQLWarehouseReport_V2(param);
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;
                    q = context.Database.SqlQuery<V_ART_WAREHOUSE_REPORT_2>(strSQL).ToList();
                }
            }

            cnt = q.Distinct().Count();
            return q;
        }


        public static String getSQLWarehouseReport_V2(V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
            string sql = "";
            string where = "";

            sql += " select * ";
            sql += " from V_REPORT_WH";

            where = getSQLWhereWarehouseReport_V2(param);
            if (where != "")
            {
                sql += " where " + where;
            }

            return sql;
        }


        public static String getSQLWhereWarehouseReport_V2(V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
            string where = " (PO_COMPLETE_SO_HEADER_ID > 0 )";

            if (!string.IsNullOrEmpty(param.data.SALES_ORDER_NO))
            {
                // where += " AND (SALES_ORDER_NO LIKE '%" + param.data.SALES_ORDER_NO + "%')";
                where += " AND " + getSQLWhereInByConvertString(param.data.SALES_ORDER_NO, "SALES_ORDER_NO");
            }

            if (!string.IsNullOrEmpty(param.data.SALES_ORG))
            {
                where += " AND (SALES_ORG = '" + param.data.SALES_ORG + "')";
            }

            if (param.data.SALES_ORDER_ITEM != null)
            {
                if (Regex.IsMatch(param.data.SALES_ORDER_ITEM.ToString(), @"^\d+$"))
                {
                    where += " AND (SALES_ORDER_ITEM = '" + param.data.SALES_ORDER_ITEM.ToString() + "')";
                }
            }

            if (param.data.SHIP_TO_ID != null)
            {
                where += " AND (SHIP_TO_ID = '" + param.data.SHIP_TO_ID + "')";
            }

            if (param.data.BRAND_ID != 0 && param.data.BRAND_ID != null)
            {
                where += " AND (BRAND_ID = '" + param.data.BRAND_ID + "')";
            }

            if (!string.IsNullOrEmpty(param.data.PROJECT_NAME))
            {
                where += " AND (PROJECT_NAME LIKE '%" + param.data.PROJECT_NAME + "%')";
            }

            if (!string.IsNullOrEmpty(param.data.PURCHASE_ORDER_NO))
            {
                where += " AND (PURCHASE_ORDER_NO LIKE '%" + param.data.PURCHASE_ORDER_NO + "%')";
            }

            if (!string.IsNullOrEmpty(param.data.PURCHASING_ORG))
            {
                where += " AND (PURCHASING_ORG = '" + param.data.PURCHASING_ORG + "')";
            }

            if (!string.IsNullOrEmpty(param.data.PO_ITEM_NO))
            {
                where += " AND (PO_ITEM_NO LIKE '%" + param.data.PO_ITEM_NO + "%')";
            }

            if (!string.IsNullOrEmpty(param.data.MATERIAL_CODE))
            {
                where += " AND (MATERIAL_CODE = '" + param.data.MATERIAL_CODE + "')";
            }

            if (param.data.COMPANY_ID != 0 && param.data.COMPANY_ID != null)
            {
                where += " AND (COMPANY_ID = '" + param.data.COMPANY_ID + "')";
            }

            if (!string.IsNullOrEmpty(param.data.DOC_DATE))
            {
                var s = param.data.DOC_DATE.Split('/');
                if (s.Length > 2)
                {
                    var stringDate = s[2] + s[1] + s[0];
                    where += " AND (DOC_DATE = '" + stringDate + "')";
                }
            }

            return where;
        }


        public static string getSQLWhereLikeByConvertString(string strPattern, string fldname)
        {
            string where = "";

            if (!string.IsNullOrEmpty(strPattern))
            {

                var arrStr = strPattern.Replace(" ", "").Split(',');

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
                                    where = "(" + fldname + " like '" + s + "%')";
                                }
                                else
                                {
                                    where += " or (" + fldname + " like '" + s + "%')";
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


        public static string getSQLWhereInByConvertString(string strPattern, string fldname)
        {
            string where = "'X'";

            if (!string.IsNullOrEmpty(strPattern))
            {
                var arrStr = strPattern.Split(',');

                if (arrStr != null)
                {
                    if (arrStr.Length > 0)
                    {
                        foreach (string s in arrStr)
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                where += ",'" + s + "'";
                                //if (string.IsNullOrEmpty(where))
                                //{
                                //    where ="("+fldname + " like '%" + s + "%')";
                                //}
                                //else
                                //{
                                //    where += " or ("+ fldname + " like '%" + s + "%')";
                                //}
                            }
                        }
                    }
                }

            }

            if (!string.IsNullOrEmpty(where))
            {
                where = "(" + fldname + " in (" + where + "))";
            }

            return where;
        }


        public static List<V_ART_WAREHOUSE_REPORT_2> QueryWarehouseReport_V3(V_ART_WAREHOUSE_REPORT_REQUEST param, ref int cnt)
        {
            List<V_ART_WAREHOUSE_REPORT_2> Outstanding = new List<V_ART_WAREHOUSE_REPORT_2>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;



                    IQueryable<V_ART_WAREHOUSE_REPORT_2> q = context.Database.SqlQuery<V_ART_WAREHOUSE_REPORT_2>("select * from V_REPORT_WH").ToList().AsQueryable();



                    if (!string.IsNullOrEmpty(param.data.SALES_ORDER_NO))
                    {
                        var arrSO = param.data.SALES_ORDER_NO.Split(',');
                        q = q.Where(m => arrSO.Contains(m.SALES_ORDER_NO));
                    }

                    if (!string.IsNullOrEmpty(param.data.SALES_ORG))
                    {
                        q = q.Where(m => m.SALES_ORG == param.data.SALES_ORG);
                    }

                    if (param.data.SALES_ORDER_ITEM != null)
                    {
                        if (Regex.IsMatch(param.data.SALES_ORDER_ITEM.ToString(), @"^\d+$"))
                        {
                            q = q.Where(m => m.SALES_ORDER_ITEM == param.data.SALES_ORDER_ITEM);
                        }
                    }

                    if (param.data.SHIP_TO_ID != null)
                    {
                        q = q.Where(m => m.SHIP_TO_ID == param.data.SHIP_TO_ID);
                    }

                    if (param.data.BRAND_ID != 0 && param.data.BRAND_ID != null)
                    {
                        q = q.Where(m => m.BRAND_ID == param.data.BRAND_ID);
                    }

                    if (!string.IsNullOrEmpty(param.data.PROJECT_NAME))
                    {
                        q = q.Where(m => m.PROJECT_NAME.Contains(param.data.PROJECT_NAME));
                    }

                    if (!string.IsNullOrEmpty(param.data.PURCHASE_ORDER_NO))
                    {
                        q = q.Where(m => m.PURCHASE_ORDER_NO.Contains(param.data.PURCHASE_ORDER_NO));
                    }

                    if (!string.IsNullOrEmpty(param.data.PURCHASING_ORG))
                    {
                        q = q.Where(m => m.PURCHASING_ORG == param.data.PURCHASING_ORG);
                    }

                    if (!string.IsNullOrEmpty(param.data.PO_ITEM_NO))
                    {
                        q = q.Where(m => m.PO_ITEM_NO.Contains(param.data.PO_ITEM_NO));
                    }

                    if (!string.IsNullOrEmpty(param.data.MATERIAL_CODE))
                    {
                        q = q.Where(m => m.MATERIAL_CODE == param.data.MATERIAL_CODE);
                    }

                    if (param.data.COMPANY_ID != 0 && param.data.COMPANY_ID != null)
                    {
                        q = q.Where(m => m.COMPANY_ID == param.data.COMPANY_ID);
                    }

                    if (!string.IsNullOrEmpty(param.data.DOC_DATE))
                    {
                        var s = param.data.DOC_DATE.Split('/');
                        if (s.Length > 2)
                        {
                            var stringDate = s[2] + s[1] + s[0];
                            q = q.Where(m => m.DOC_DATE.Equals(stringDate));
                        }
                    }

                    cnt = q.Distinct().Count();
                    return q.ToList();
                }
            }
        }


        #endregion

        //start ticket 447876 by aof for tuning QueryWarehouseReport_StoredProcedure


        public static List<V_ART_WAREHOUSE_REPORT_2> QueryWarehouseReport(V_ART_WAREHOUSE_REPORT_REQUEST param, ref int cnt)
        {
            List<V_ART_WAREHOUSE_REPORT_2> Outstanding = new List<V_ART_WAREHOUSE_REPORT_2>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;

                   

                    IQueryable<V_ART_WAREHOUSE_REPORT_2> q = (from m in context.V_ART_WAREHOUSE_REPORT
                                                              select new V_ART_WAREHOUSE_REPORT_2()
                                                              {
                                                                  SALES_ORG = m.SALES_ORG,
                                                                  SALES_ORDER_NO = m.SALES_ORDER_NO,
                                                                  SALES_ORDER_ITEM = m.SALES_ORDER_ITEM,
                                                                  PRODUCT_CODE = m.PRODUCT_CODE,
                                                                  MATERIAL_CODE = m.MATERIAL_CODE,
                                                                  MATERIAL_DECRIPTION = m.MATERIAL_DECRIPTION,
                                                                  PACKAGING_TYPE_ID = m.PACKAGING_TYPE_ID,
                                                                  PACKAGING_TYPE_NAME = m.PACKAGING_TYPE_NAME,
                                                                  REQUEST_ITEM_NO = m.REQUEST_ITEM_NO,
                                                                  BRAND_ID = m.BRAND_ID,
                                                                  BRAND_NAME = m.BRAND_NAME,
                                                                  PROJECT_NAME = m.PROJECT_NAME,
                                                                  SOLD_TO_ID = m.SOLD_TO_ID,
                                                                  SOLD_TO = m.SOLD_TO,
                                                                  SOLD_TO_NAME = m.SOLD_TO_NAME,
                                                                  SHIP_TO_ID = m.SHIP_TO_ID,
                                                                  SHIP_TO = m.SHIP_TO,
                                                                  SHIP_TO_NAME = m.SHIP_TO_NAME,
                                                                  PORT = m.PORT,
                                                                  PURCHASE_ORDER_NO = m.PURCHASE_ORDER_NO,
                                                                  PO_ITEM_NO = m.PO_ITEM_NO,
                                                                  PURCHASING_ORG = m.PURCHASING_ORG,
                                                                  VENDOR_NO = m.VENDOR_NO,
                                                                  VENDOR_NAME = m.VENDOR_NAME,
                                                                  DOC_DATE = m.DOC_DATE,
                                                                  DELIVERY_DATE = m.DELIVERY_DATE,
                                                                  QUANTITY = m.QUANTITY,
                                                                  ORDER_UNIT = m.ORDER_UNIT,
                                                                  COMPANY_ID = m.COMPANY_ID,
                                                                  ARTWORK_SUB_ID = m.ARTWORK_SUB_ID
                                                              });

            

                    if (!string.IsNullOrEmpty(param.data.SALES_ORDER_NO))
                    {
                        var arrSO = param.data.SALES_ORDER_NO.Split(',');
                        q = q.Where(m => arrSO.Contains(m.SALES_ORDER_NO));
                    }

                    if (!string.IsNullOrEmpty(param.data.SALES_ORG))
                    {
                        q = q.Where(m => m.SALES_ORG == param.data.SALES_ORG);
                    }

                    if (param.data.SALES_ORDER_ITEM != null)
                    {
                        if (Regex.IsMatch(param.data.SALES_ORDER_ITEM.ToString(), @"^\d+$"))
                        {
                            q = q.Where(m => m.SALES_ORDER_ITEM == param.data.SALES_ORDER_ITEM);
                        }
                    }

                    if (param.data.SHIP_TO_ID != null)
                    {
                        q = q.Where(m => m.SHIP_TO_ID == param.data.SHIP_TO_ID);
                    }

                    if (param.data.BRAND_ID != 0 && param.data.BRAND_ID != null)
                    {
                        q = q.Where(m => m.BRAND_ID == param.data.BRAND_ID);
                    }

                    if (!string.IsNullOrEmpty(param.data.PROJECT_NAME))
                    {
                        q = q.Where(m => m.PROJECT_NAME.Contains(param.data.PROJECT_NAME));
                    }

                    if (!string.IsNullOrEmpty(param.data.PURCHASE_ORDER_NO))
                    {
                        q = q.Where(m => m.PURCHASE_ORDER_NO.Contains(param.data.PURCHASE_ORDER_NO));
                    }

                    if (!string.IsNullOrEmpty(param.data.PURCHASING_ORG))
                    {
                        q = q.Where(m => m.PURCHASING_ORG == param.data.PURCHASING_ORG);
                    }

                    if (!string.IsNullOrEmpty(param.data.PO_ITEM_NO))
                    {
                        q = q.Where(m => m.PO_ITEM_NO.Contains(param.data.PO_ITEM_NO));
                    }

                    if (!string.IsNullOrEmpty(param.data.MATERIAL_CODE))
                    {
                        q = q.Where(m => m.MATERIAL_CODE == param.data.MATERIAL_CODE);
                    }

                    if (param.data.COMPANY_ID != 0 && param.data.COMPANY_ID != null)
                    {
                        q = q.Where(m => m.COMPANY_ID == param.data.COMPANY_ID);
                    }

                    if (!string.IsNullOrEmpty(param.data.DOC_DATE))
                    {
                        var s = param.data.DOC_DATE.Split('/');
                        if (s.Length > 2)
                        {
                            var stringDate = s[2] + s[1] + s[0];
                            q = q.Where(m => m.DOC_DATE.Equals(stringDate));
                        }
                    }

                    cnt = q.Distinct().Count();
                    return OrderByWarehouse(q, param);
                }
            }
        }

        public static List<V_ART_WAREHOUSE_REPORT_2> OrderByWarehouse(IQueryable<V_ART_WAREHOUSE_REPORT_2> q, V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
            var Warehouse = new List<V_ART_WAREHOUSE_REPORT_2>();
            var orderColumn = 1;
            var orderDir = "asc";
            if (param.order != null && param.order.Count > 0)
            {
                orderColumn = param.order[0].column;
                orderDir = param.order[0].dir; //desc ,asc
            }

            string orderASC = "asc";
            string orderDESC = "desc";

            if (orderColumn == 1)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.SALES_ORG).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.SALES_ORG).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.SALES_ORG).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.SALES_ORG).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.SALES_ORG).ToList();
                }
            }
            else if (orderColumn == 2)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.SALES_ORDER_NO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.SALES_ORDER_NO).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.SALES_ORDER_NO).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.SALES_ORDER_NO).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.SALES_ORDER_NO).ToList();
                }
            }
            else if (orderColumn == 3)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.SALES_ORDER_ITEM).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.SALES_ORDER_ITEM).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.SALES_ORDER_ITEM).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.SALES_ORDER_ITEM).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.SALES_ORDER_ITEM).ToList();
                }
            }
            else if (orderColumn == 6)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.PRODUCT_CODE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.PRODUCT_CODE).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.PRODUCT_CODE).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.PRODUCT_CODE).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.PRODUCT_CODE).ToList();
                }
            }
            else if (orderColumn == 7)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.MATERIAL_CODE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.MATERIAL_CODE).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.MATERIAL_CODE).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.MATERIAL_CODE).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.MATERIAL_CODE).ToList();
                }
            }
            else if (orderColumn == 8)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.MATERIAL_DECRIPTION).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.MATERIAL_DECRIPTION).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.MATERIAL_DECRIPTION).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.MATERIAL_DECRIPTION).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.MATERIAL_DECRIPTION).ToList();
                }
            }
            else if (orderColumn == 9)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.PACKAGING_TYPE_NAME).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.PACKAGING_TYPE_NAME).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.PACKAGING_TYPE_NAME).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.PACKAGING_TYPE_NAME).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.PACKAGING_TYPE_NAME).ToList();
                }
            }
            else if (orderColumn == 10)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.REQUEST_ITEM_NO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.REQUEST_ITEM_NO).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.REQUEST_ITEM_NO).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.REQUEST_ITEM_NO).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.REQUEST_ITEM_NO).ToList();
                }
            }
            else if (orderColumn == 11)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.BRAND_NAME).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.BRAND_NAME).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.BRAND_NAME).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.BRAND_NAME).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.BRAND_NAME).ToList();
                }
            }
            else if (orderColumn == 12)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.PROJECT_NAME).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.PROJECT_NAME).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.PROJECT_NAME).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.PROJECT_NAME).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.PROJECT_NAME).ToList();
                }
            }
            else if (orderColumn == 13)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.SOLD_TO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.SOLD_TO).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.SOLD_TO).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.SOLD_TO).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.SOLD_TO).ToList();
                }
            }
            else if (orderColumn == 14)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.SOLD_TO_NAME).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.SOLD_TO_NAME).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.SOLD_TO_NAME).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.SOLD_TO_NAME).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.SOLD_TO_NAME).ToList();
                }
            }
            else if (orderColumn == 15)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.SHIP_TO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.SHIP_TO).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.SHIP_TO).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.SHIP_TO).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.SHIP_TO).ToList();
                }
            }
            else if (orderColumn == 16)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.SHIP_TO_NAME).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.SHIP_TO_NAME).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.SHIP_TO_NAME).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.SHIP_TO_NAME).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.SHIP_TO_NAME).ToList();
                }
            }
            else if (orderColumn == 17)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.PORT).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.PORT).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.PORT).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.PORT).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.PORT).ToList();
                }
            }
            else if (orderColumn == 18)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.PURCHASE_ORDER_NO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.PURCHASE_ORDER_NO).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.PURCHASE_ORDER_NO).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.PURCHASE_ORDER_NO).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.PURCHASE_ORDER_NO).ToList();
                }
            }
            else if (orderColumn == 19)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.PO_ITEM_NO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.PO_ITEM_NO).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.PO_ITEM_NO).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.PO_ITEM_NO).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.PO_ITEM_NO).ToList();
                }
            }
            else if (orderColumn == 21)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.PURCHASING_ORG).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.PURCHASING_ORG).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.PURCHASING_ORG).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.PURCHASING_ORG).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.PURCHASING_ORG).ToList();
                }
            }
            else if (orderColumn == 22)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.VENDOR_NO).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.VENDOR_NO).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.VENDOR_NO).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.VENDOR_NO).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.VENDOR_NO).ToList();
                }
            }
            else if (orderColumn == 23)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.VENDOR_NAME).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.VENDOR_NAME).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.VENDOR_NAME).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.VENDOR_NAME).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.VENDOR_NAME).ToList();
                }
            }
            else if (orderColumn == 24)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.DOC_DATE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.DOC_DATE).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.DOC_DATE).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.DOC_DATE).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.DOC_DATE).ToList();
                }
            }
            else if (orderColumn == 25)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.DELIVERY_DATE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.DELIVERY_DATE).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.DELIVERY_DATE).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.DELIVERY_DATE).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.DELIVERY_DATE).ToList();
                }
            }
            else if (orderColumn == 26)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.QUANTITY).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.QUANTITY).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.QUANTITY).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.QUANTITY).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.QUANTITY).ToList();
                }
            }
            else if (orderColumn == 27)
            {
                if (param.data.GENERATE_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.ORDER_UNIT).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.ORDER_UNIT).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.ORDER_UNIT).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.ORDER_UNIT).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.ORDER_UNIT).ToList();
                }
            }

            return Warehouse;
        }

        private static List<V_ART_WAREHOUSE_REPORT_2> FilterDataPGView(List<V_ART_WAREHOUSE_REPORT_2> data, V_ART_WAREHOUSE_REPORT_REQUEST param, ref int cnt)
        {
            var filterValue = param.columns[1].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SALES_ORG) && m.SALES_ORG.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[2].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SALES_ORDER_NO) && m.SALES_ORDER_NO.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[3].search.value;
            if (!string.IsNullOrEmpty(filterValue))
            {
                var temp = Convert.ToDecimal(filterValue);
                data = data.Where(m => m.SALES_ORDER_ITEM == temp).ToList();
            }

            filterValue = param.columns[6].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PRODUCT_CODE) && m.PRODUCT_CODE.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[7].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.MATERIAL_CODE) && m.MATERIAL_CODE.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[8].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.MATERIAL_DECRIPTION) && m.MATERIAL_DECRIPTION.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[9].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PACKAGING_TYPE_NAME) && m.PACKAGING_TYPE_NAME.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[10].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.REQUEST_ITEM_NO) && m.REQUEST_ITEM_NO.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[11].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.BRAND_NAME) && m.BRAND_NAME.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[12].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PROJECT_NAME) && m.PROJECT_NAME.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[13].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SOLD_TO) && m.SOLD_TO.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[14].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SOLD_TO_NAME) && m.SOLD_TO_NAME.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[15].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SHIP_TO) && m.SHIP_TO.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[16].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SHIP_TO_NAME) && m.SHIP_TO_NAME.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[17].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PORT) && m.PORT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[18].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PURCHASE_ORDER_NO) && m.PURCHASE_ORDER_NO.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[19].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PO_ITEM_NO) && m.PO_ITEM_NO.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[21].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PURCHASING_ORG) && m.PURCHASING_ORG.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[22].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.VENDOR_NO) && m.VENDOR_NO.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[23].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.VENDOR_NAME) && m.VENDOR_NAME.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[24].search.value;
            if (!string.IsNullOrEmpty(filterValue))
            {
                var s = filterValue.Split('/');
                if (s.Length > 2)
                {
                    var stringDate = s[2] + s[1] + s[0];
                    data = data.Where(m => m.DOC_DATE.Equals(stringDate)).ToList();
                }
                else if (s.Length > 1)
                {
                    var stringDate = s[1] + s[0];
                    data = data.Where(m => m.DOC_DATE.Contains(stringDate)).ToList();
                }
                else
                {
                    data = data.Where(m => m.DOC_DATE.Contains(filterValue)).ToList();
                }
            }

            filterValue = param.columns[25].search.value;
            if (!string.IsNullOrEmpty(filterValue))
            {
                var s = filterValue.Split('/');
                if (s.Length > 2)
                {
                    var stringDate = s[2] + s[1] + s[0];
                    data = data.Where(m => m.DELIVERY_DATE.Equals(stringDate)).ToList();
                }
                else if (s.Length > 1)
                {
                    var stringDate = s[1] + s[0];
                    data = data.Where(m => m.DELIVERY_DATE.Contains(stringDate)).ToList();
                }
                else
                {
                    data = data.Where(m => m.DELIVERY_DATE.Contains(filterValue)).ToList();
                }
            }

            filterValue = param.columns[26].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.QUANTITY) && m.QUANTITY.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[27].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.ORDER_UNIT) && m.ORDER_UNIT.ToLower().Contains(filterValue.ToLower())).ToList();

            cnt = data.Count();

            return data;
        }

        public static V_ART_WAREHOUSE_REPORT_RESULT GetWarehouseReport_SOAtt(V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
            V_ART_WAREHOUSE_REPORT_RESULT Results = new V_ART_WAREHOUSE_REPORT_RESULT();
            List<V_ART_WAREHOUSE_REPORT_2> data = new List<V_ART_WAREHOUSE_REPORT_2>();
            try
            {
                var parentSOID = ConfigurationManager.AppSettings["SONodeID"];
                var folderSOName = ConfigurationManager.AppSettings["ArtworkFolderNameSO"];
                var token = CWSService.getAuthToken();
                Node nodeParentSO = CWSService.getNodeByName(Convert.ToInt64(parentSOID), param.data.SALES_ORDER_NO, token);
                if (nodeParentSO != null)
                {
                    Node node = CWSService.getNodeByName(nodeParentSO.ID * (-1), folderSOName, token);
                    if (node != null)
                    {
                        Node[] nodeSOFiles = CWSService.getAllNodeInFolder(node.ID, token);
                        if (nodeSOFiles != null && nodeSOFiles.Count() > 0)
                        {
                            foreach (Node iSOFile in nodeSOFiles)
                            {
                                V_ART_WAREHOUSE_REPORT_2 d = new V_ART_WAREHOUSE_REPORT_2()
                                {
                                    FILE_NAME = iSOFile.Name,
                                    CREATED_DATE = iSOFile.CreateDate,
                                    CREATED_BY_DISPLAY_TXT = "",
                                    NODE_ID_TXT = EncryptionService.EncryptAndUrlEncode(iSOFile.ID.ToString())

                                };
                                data.Add(d);
                            }
                        }
                    }
                }
                Results.data = data;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static V_ART_WAREHOUSE_REPORT_RESULT GetWarehouseReport_POAtt(V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
            V_ART_WAREHOUSE_REPORT_RESULT Results = new V_ART_WAREHOUSE_REPORT_RESULT();
            List<V_ART_WAREHOUSE_REPORT_2> data = new List<V_ART_WAREHOUSE_REPORT_2>();
            try
            {
                var parentPOID = ConfigurationManager.AppSettings["PONodeID"];
                var folderPOName = ConfigurationManager.AppSettings["ArtworkFolderNamePO"];
                var token = CWSService.getAuthToken();
                Node nodeParentPO = CWSService.getNodeByName(Convert.ToInt64(parentPOID), param.data.PURCHASE_ORDER_NO, token);
                if (nodeParentPO != null)
                {
                    Node node = CWSService.getNodeByName(nodeParentPO.ID * (-1), folderPOName, token);
                    if (node != null)
                    {
                        Node[] nodePOFiles = CWSService.getAllNodeInFolder(node.ID, token);
                        if (nodePOFiles != null && nodePOFiles.Count() > 0)
                        {
                            foreach (Node iPOFile in nodePOFiles)
                            {
                                V_ART_WAREHOUSE_REPORT_2 d = new V_ART_WAREHOUSE_REPORT_2()
                                {
                                    FILE_NAME = iPOFile.Name,
                                    CREATED_DATE = iPOFile.CreateDate,
                                    CREATED_BY_DISPLAY_TXT = "",
                                    NODE_ID_TXT = EncryptionService.EncryptAndUrlEncode(iPOFile.ID.ToString())

                                };
                                data.Add(d);
                            }
                        }
                    }
                }
                Results.data = data;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static V_ART_WAREHOUSE_REPORT_RESULT GetWarehouseReport_AWAtt(V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
            V_ART_WAREHOUSE_REPORT_RESULT Results = new V_ART_WAREHOUSE_REPORT_RESULT();
            List<V_ART_WAREHOUSE_REPORT_2> data = new List<V_ART_WAREHOUSE_REPORT_2>();
            try
            {
                var parentSecondaryPackagingID = ConfigurationManager.AppSettings["SecondaryPackagingNodeID"];
                var folderSPAW = ConfigurationManager.AppSettings["SecondaryPkgArtworkFolderName"];

                var mat_desc = "";
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        mat_desc = CNService.getMatDesc(param.data.MATERIAL_CODE, context);
                    }
                }

                var token = CWSService.getAuthToken();
                //var mat_desc = SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_SERVICE.GetByItem(new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT() { COMPONENT_MATERIAL = param.data.MATERIAL_CODE }).FirstOrDefault();
                if (!string.IsNullOrEmpty(mat_desc))
                {
                    param.data.MATERIAL_DECRIPTION = mat_desc;
                    Node nodeParentSPAW = CWSService.getNodeByName(Convert.ToInt64(parentSecondaryPackagingID), param.data.MATERIAL_CODE + " - " + param.data.MATERIAL_DECRIPTION, token);
                    if (nodeParentSPAW != null)
                    {
                        Node node = CWSService.getNodeByName(nodeParentSPAW.ID * (-1), folderSPAW, token);
                        if (node != null)
                        {
                            Node[] nodeSPAWFiles = CWSService.getAllNodeInFolder(node.ID, token);
                            if (nodeSPAWFiles != null && nodeSPAWFiles.Count() > 0)
                            {
                                foreach (Node iPOFile in nodeSPAWFiles)
                                {
                                    V_ART_WAREHOUSE_REPORT_2 d = new V_ART_WAREHOUSE_REPORT_2()
                                    {
                                        FILE_NAME = iPOFile.Name,
                                        CREATED_DATE = iPOFile.CreateDate,
                                        TITLE = "Final Artwork",
                                        NODE_ID_TXT = EncryptionService.EncryptAndUrlEncode(iPOFile.ID.ToString())
                                    };
                                    data.Add(d);
                                }
                            }
                        }
                    }
                }
                Results.data = data;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }
    }
}


