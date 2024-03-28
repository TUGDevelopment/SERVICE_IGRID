using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{


    public class TU_TRACKING_USER_MODEL
    {
        public int USER_ID { get; set; } 
        public string ROLE_CODE { get; set; }
        public int COUNT { get; set; }

        public static bool IsRoleViewFullStep(int user_id, ARTWORKEntities context)
        {
            bool isViewFullStep = false;
           
            var obj = context.Database.SqlQuery<TU_TRACKING_USER_MODEL>("SELECT USER_ID,ROLE_CODE FROM V_ART_REPORT_TRAKCING_USER WHERE USER_ID='"+ user_id + "'").ToList();

            if (obj != null && obj.Count > 0 ) 
            {
                isViewFullStep = true;
            }

            return isViewFullStep;

        }

    }


    public class TU_TRACKING_WF_PROCESS_VENDOR_MODEL
    {
        public int WF_ID { get; set; }
        public string SELECTED { get; set; }
        public string RFQ_VENDOR_NAME { get; set; }
        public string SELECTED_VENDOR_NAME { get; set; }

        public static string getSQLWFProcessVendor(string where = "")
        {
            string sql = "";

            sql += " SELECT DISTINCT * ";
            sql += " FROM V_ART_REPORT_TRACKING_MC_PROCESS_VENDOR";
            if (where != "")
            {
                sql += "  WHERE " + where;
            }


            return sql;
        }


        public static void setDataWFProcessVendor(TU_TRACKING_WF_REPORT_MODEL obj, List<TU_TRACKING_WF_PROCESS_VENDOR_MODEL> listVendor)
        {

            string all_RFQ,all_SELECTED;

            all_RFQ = "";
            all_SELECTED = "";

            var listRFQ = listVendor.Where(w=>!string.IsNullOrEmpty(w.RFQ_VENDOR_NAME)).Select(s => s.RFQ_VENDOR_NAME).Distinct();
          
            foreach (var v in listRFQ)
            {         
                if (all_RFQ == "")
                {
                    all_RFQ = v;
                }
                else
                {
                    all_RFQ += "<br/>"+ v;
                }
            }

            var listSELECTED = listVendor.Where(w => !string.IsNullOrEmpty(w.SELECTED_VENDOR_NAME) && !string.IsNullOrEmpty(w.SELECTED)).Select(s => s.SELECTED_VENDOR_NAME).Distinct().FirstOrDefault();
            if (string.IsNullOrEmpty(listSELECTED))
            {
                all_SELECTED = listVendor.Where(w => !string.IsNullOrEmpty(w.SELECTED_VENDOR_NAME)).Select(s => s.SELECTED_VENDOR_NAME).Distinct().FirstOrDefault(); ;
            }
            else
            {
                all_SELECTED = listSELECTED;
            }
           

            obj.VENDOR_RFQ = all_RFQ;
            obj.SELECTED_VENDOR = all_SELECTED;

        }



    }



    public class TU_TRACKING_WF_ASSING_SO_MODEL
    {
        public int WF_ID { get; set; }
        public int WF_SUB_ID { get; set; }
        public string SALES_ORDER_NO { get; set; }
        public Nullable<decimal> ITEM { get; set; }
        public Nullable<DateTime> RDD { get; set; }
        public Nullable<DateTime> CREATE_DATE { get; set; }
        public string PORT { get; set; }
        public string IN_TRANSIT_TO { get; set; }
        public string ADDITIONAL_BRAND { get; set; }
        public string PROD_INSP_MEMO { get; set; }

        public static string getSQLWFAssingSO(string where = "")
        {
            string sql = "";

            sql += " SELECT DISTINCT WF_ID,WF_SUB_ID,RDD,SALES_ORDER_NO,ITEM,CREATE_DATE";
            sql += " ,PORT,IN_TRANSIT_TO,ADDITIONAL_BRAND,PROD_INSP_MEMO";
            sql += " FROM V_ART_REPORT_TRACKING_AW_ASSIGN_SO";
            if (where != "")
            {
                sql += "  WHERE " + where;
            }


            return sql;
        }

        public static void setDataWFAssignSO(TU_TRACKING_WF_REPORT_MODEL obj, List<TU_TRACKING_WF_ASSING_SO_MODEL> listSO)
        {

            string all_so_no, all_rdd, all_create, all_port, all_intransit, all_additional_barand,all_prod_insp_memo;
            string so_item;
            all_so_no = "";
            all_create = "";
            all_rdd = "";    
            all_port = "";
            all_intransit = "";
            all_additional_barand = "";
            all_prod_insp_memo = "";

            foreach (var so in listSO)
            {
                so_item = so.SALES_ORDER_NO + "(" + so.ITEM + ")";
                if (!string.IsNullOrEmpty(so.SALES_ORDER_NO))
                {
                    if (all_so_no.IndexOf(so_item) == -1) 
                    {
                        all_so_no += so_item + ",";
                    }
                }

                if (so.CREATE_DATE != null)
                {
                    if (all_create.IndexOf(so.CREATE_DATE.Value.ToString("dd/MM/yyyy"))== -1)
                    {
                        all_create += so.CREATE_DATE.Value.ToString("dd/MM/yyyy") + ",";
                    }
                }

                if (so.RDD != null)
                {
                    if (all_rdd.IndexOf(so.RDD.Value.ToString("dd/MM/yyyy")) == -1)
                    {
                        all_rdd += so.RDD.Value.ToString("dd/MM/yyyy") + ",";
                    }
                }

                if (so.PORT != null)
                {
                    if (all_port.IndexOf(so.PORT) == -1)
                    {
                        all_port += so.PORT + ",";
                    }
                }

                if (so.IN_TRANSIT_TO != null)
                {
                    if (all_intransit.IndexOf(so.IN_TRANSIT_TO) == -1)
                    {
                        all_intransit += so.IN_TRANSIT_TO + ",";
                    }
                }

                if (so.ADDITIONAL_BRAND != null)
                {
                    if (all_additional_barand.IndexOf(so.ADDITIONAL_BRAND) == -1)
                    {
                        all_additional_barand += so.ADDITIONAL_BRAND + ",";
                    }
                }

                if (so.PROD_INSP_MEMO != null)
                {
                    if (all_prod_insp_memo.IndexOf(so.PROD_INSP_MEMO) == -1)
                    {
                        all_prod_insp_memo += so.PROD_INSP_MEMO + ",";
                    }
                }
            }

            if (all_so_no.Length > 0) all_so_no = all_so_no.Substring(0, all_so_no.Length - 1);
            if (all_create.Length > 0) all_create = all_create.Substring(0, all_create.Length - 1);
            if (all_rdd.Length > 0) all_rdd = all_rdd.Substring(0, all_rdd.Length - 1);
            if (all_port.Length > 0) all_port = all_port.Substring(0, all_port.Length - 1);
            if (all_intransit.Length > 0) all_intransit = all_intransit.Substring(0, all_intransit.Length - 1);
            if (all_additional_barand.Length > 0) all_additional_barand = all_additional_barand.Substring(0, all_additional_barand.Length - 1);
            if (all_prod_insp_memo.Length > 0) all_prod_insp_memo = all_prod_insp_memo.Substring(0, all_prod_insp_memo.Length - 1);


            obj.SO_NO = all_so_no;
            obj.SO_CREATE_DATE = all_create;
            obj.RDD = all_rdd;
            obj.PORT = all_port;
            obj.IN_TRANSIT_TO = all_intransit;
            obj.ADDITIONAL_BRAND = all_additional_barand;
            obj.PROD_INSP_MEMO = all_prod_insp_memo;

        }     
    }


    public class TU_TRACKING_WF_REFERENCE_MODEL
    {
        public int REQUEST_ID { get; set; }
        public string REFERENCE_NO { get; set; }


        public static string getSQLWFReference(string where = "", string wf_type = "AW")
        {
            string sql = "";

            sql += " SELECT DISTINCT REQUEST_ID,REFERENCE_NO";
            if (wf_type == "AW")
            {
                sql += " FROM V_ART_REPORT_TRACKING_AW_REFERENCE";
            }
            else
            {
                sql += " FROM V_ART_REPORT_TRACKING_MC_REFERENCE";
            }
          
            if (where != "")
            {
                sql += "  WHERE " + where;
            }
            return sql;
        }


        public static void setDataWFReference(TU_TRACKING_WF_REPORT_MODEL obj, List<TU_TRACKING_WF_REFERENCE_MODEL> listReference)
        {

            string all_ref;

            all_ref = "";

            foreach (var r in listReference)
            {
                if (!string.IsNullOrEmpty(r.REFERENCE_NO))
                {
                    if (all_ref.IndexOf(r.REFERENCE_NO) == -1)
                    {
                        all_ref += r.REFERENCE_NO + ",";
                    }
                }
            }

            if (all_ref.Length > 0) all_ref = all_ref.Substring(0, all_ref.Length - 1);


            obj.REFERENCE_NO = all_ref;

        }



    }

    public class TU_TRACKING_WF_PRODUCT_MODEL
    {
        public int WF_ID { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string PRODUCT_NAME { get; set; }

        public static string getSQLWFProduct(string where = "",string wf_type = "AW")
        {
            string sql = "";

            sql += " SELECT DISTINCT WF_ID,PRODUCT_CODE,PRODUCT_NAME";
            if (wf_type == "AW") {
                sql += " FROM V_ART_REPORT_TRACKING_AW_PRODUCT";
            } else {
                sql += " FROM V_ART_REPORT_TRACKING_MC_PRODUCT";
            }
      
            if (where != "")
            {
                sql += "  WHERE " + where;
            }
            return sql;
        }


        public static void setDataWFProduct(TU_TRACKING_WF_REPORT_MODEL obj, List<TU_TRACKING_WF_PRODUCT_MODEL> listProduct)
        {

            string all_proudct;

            all_proudct = "";
          
            foreach (var product in listProduct)
            {
                if (!string.IsNullOrEmpty(product.PRODUCT_CODE))
                {
                    if (all_proudct.IndexOf(product.PRODUCT_CODE) == -1)
                    {
                        if (!string.IsNullOrEmpty(product.PRODUCT_NAME))
                        {
                            all_proudct += product.PRODUCT_CODE + ":" + product.PRODUCT_NAME + ",";
                        } else
                        {
                            all_proudct += product.PRODUCT_CODE + ",";
                        }
                     
                    }
                }
            }

            if (all_proudct.Length > 0) all_proudct = all_proudct.Substring(0, all_proudct.Length - 1);
         

            obj.PRODUCT_CODE = all_proudct;
      
        }

        //WHERE WF_ID IN(" + IN_WF_ID + ") AND ISNULL(PRODUCT_CODE,'') <> ''

    }

    public class TU_TRACKING_WF_PROCESS_MODEL
    {
        public int WF_ID { get; set; }
        public int WF_SUB_ID { get; set; }
        public string CURRENT_STEP_CODE { get; set; }
        public string CURRENT_STEP_NAME { get; set; }
      
        public string CURRENT_USER_NAME { get; set; }

        public DateTime PROCESS_CREATE_DATE { get; set; }
        public string IS_STEP_DURATION_EXTEND { get; set; }
        public string REMARK_KILLPROCESS { get; set; }
        public Nullable<decimal> DURATION { get; set; }
        public Nullable<decimal> DURATION_EXTEND { get; set; }

        public Nullable<int> CURRENT_STEP_ID { get; set; }
        public Nullable<int> CURRENT_USER_ID { get; set; }

        public Nullable<int> CURRENT_DURATION { get; set; }
        public Nullable<DateTime> CURRENT_DUE_DATE { get; set; }
        public Nullable<DateTime> STEP_END_DATE { get; set; }
        public Nullable<DateTime> DUE_DATE { get; set; }

        public static string getSQLWFProcess(string where = "", string wf_type = "AW")
        {
            string sqlProcess;
            sqlProcess = " SELECT DISTINCT WF_ID, WF_SUB_ID";
            sqlProcess += " ,CURRENT_STEP_CODE,CURRENT_STEP_NAME,CURRENT_USER_NAME,PROCESS_CREATE_DATE,CURRENT_STEP_ID,CURRENT_STEP_CODE,CURRENT_USER_ID";
            sqlProcess += " ,IS_STEP_DURATION_EXTEND,REMARK_KILLPROCESS,DURATION,DURATION_EXTEND,STEP_END_DATE";
            sqlProcess += " ,CURRENT_DURATION, DUE_DATE AS CURRENT_DUE_DATE";
            sqlProcess += " ,ISNULL(DUE_DATE,GETDATE()) AS DUE_DATE";

            if (wf_type == "AW")
            {
                sqlProcess += " FROM V_ART_REPORT_TRACKING_AW_PROCESS";
            }
            else
            {
                sqlProcess += " FROM V_ART_REPORT_TRACKING_MC_PROCESS";
            }

         
            //sqlProcess += " WHERE WF_ID IN (" + IN_WF_ID + ") and CURRENT_STEP_CODE <> 'SEND_PA'";
           
            if (where != "")
            {
                sqlProcess += "  WHERE " + where;
            }
            return sqlProcess;
        }


    }


    public class TU_TRACKING_WF_REPORT_MODEL
    {
        public string CREATOR_NAME { get; set; }
        public string WF_NO { get; set; }
        public string PACKAGING_TYPE { get; set; }
        public string PRIMARY_TYPE_TXT { get; set; }
        public string WF_STATUS { get; set; }
        public string CURRENT_STEP { get; set; }
        public string CURRENT_ASSING { get; set; }
        public Nullable<int> CURRENT_DURATION { get; set; }
        public Nullable<DateTime> CURRENT_DUE_DATE { get; set; }
        public string SOLD_TO { get; set; }
        public string SHIP_TO { get; set; }
        public string PORT { get; set; }
        public string IN_TRANSIT_TO { get; set; }
        public string SO_NO { get; set; }
        public string SO_CREATE_DATE { get; set; }
        public string BRAND_NAME { get; set; }
        public string ADDITIONAL_BRAND { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string PROD_INSP_MEMO { get; set; }
        public string REFERENCE_NO { get; set; }
        public string RDD { get; set; }
        public string VENDOR_RFQ { get; set; }
        public string SELECTED_VENDOR { get; set; }
        public int WF_ID { get; set; }
        public int WF_SUB_ID { get; set; }
        public int REQUEST_ID { get; set; }
        public DateTime PROCESS_CREATE_DATE { get; set; }
        public string IS_END { get; set; }
        public string IS_TERMINATE { get; set; }
        public string IS_STEP_DURATION_EXTEND { get; set; }
        public string REMARK_KILLPROCESS { get; set; }
        public Nullable<decimal> DURATION { get; set; }
        public Nullable<decimal> DURATION_EXTEND { get; set; }
        public bool FIRST_LOAD { get; set; }

        public Nullable<int> CURRENT_USER_ID { get; set; }
        public Nullable<int> CURRENT_STEP_ID { get; set; }
        public string CURRENT_STEP_CODE { get; set; }

        public Nullable<DateTime> STEP_END_DATE { get; set; }
        public Nullable<DateTime> DUE_DATE { get; set; }
        public int WF_SUB_PA_ID { get; set; }


        public string SEARCH_WF_NO { get; set; }
        public string SEARCH_WF_TYPE_X { get; set; }
        public string SEARCH_WF_SUB_TYPE { get; set; }
        public string SEARCH_REQUEST_NO { get; set; }
        public string SEARCH_REQUEST_DATE_FROM { get; set; }
        public string SEARCH_REQUEST_DATE_TO { get; set; }
        public string SEARCH_REFERENCE_FORM_NO { get; set; }
        public string SEARCH_WF_IS_COMPLETED { get; set; }
        public string SEARCH_WF_IN_PROCESS { get; set; }
        public string SEARCH_SO_NO { get; set; }
        public string SEARCH_SO_MATERIAL { get; set; }
        public string SEARCH_SO_CREATE_DATE_FROM { get; set; }
        public string SEARCH_SO_CREATE_DATE_TO { get; set; }
        public Nullable<int> SEARCH_COMPANY_ID { get; set; }
        public Nullable<int> SEARCH_SOLD_TO_ID { get; set; }
        public Nullable<int> SEARCH_SHIP_TO_ID { get; set; }
        public Nullable<int> SEARCH_BRAND_ID { get; set; }
        public string SEARCH_ZONE { get; set; }
        public Nullable<int> SEARCH_COUNTRY_ID { get; set; }
        public Nullable<int> SEARCH_PACKAGING_TYPE_ID { get; set; }
        public string SEARCH_PACKAGING_TYPE { get; set; }
        public string SEARCH_PRIMARY_SIZE { get; set; }
        public string SEARCH_PROJECT_NAME { get; set; }
        public string SEARCH_PRODUCT { get; set; }
        public string SEARCH_REFERENCE_NO { get; set; }
        public string SEARCH_NET_WEIGHT { get; set; }
       
        public Nullable<int> SEARCH_CURRENT_STEP_ID { get; set; }
        public Nullable<int> SEARCH_CREATOR_ID { get; set; }
        public Nullable<int> SEARCH_SUPERVISED_BY_ID { get; set; }
        public Nullable<int> SEARCH_CURRENT_ASSING_ID { get; set; }
        public Nullable<int> SEARCH_WORKING_GROUP_ID { get; set; }
        public string SEARCH_ACTION_BY_ME { get; set; }
        public string SEARCH_WORKFLOW_IS_OVERDUE { get; set; }
        public Nullable<int> SEARCH_LOGIN_USER_ID { get; set; }


        public string GENERATE_EXCEL { get; set; }
        public string VIEW { get; set; }

        public List<TU_TRACKING_WF_REPORT_MODEL> listProcess { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public static void setUserAssignAndDuration(TU_TRACKING_WF_REPORT_MODEL obj)
        {
            if (obj != null)
            {
                if (string.IsNullOrEmpty(obj.IS_END))
                {

                    //--------------------------------------------------- USER_ASSIGN
                    if (string.IsNullOrEmpty(obj.CURRENT_ASSING))
                    {
                        if (string.IsNullOrEmpty(obj.REMARK_KILLPROCESS))
                        {
                            obj.CURRENT_ASSING = "Waiting for accept";
                        }
                        else
                        {
                            obj.CURRENT_ASSING = "-";
                        }
                    }
                    //--------------------------------------------------- USER_ASSIGN

                    ////--------------------------------------------------- DUE_DATE---------exclude saturday,sunday.
                    //decimal duraion = 0;
                    //if (string.IsNullOrEmpty(obj.IS_STEP_DURATION_EXTEND))
                    //{
                    //    duraion = obj.DURATION;
                    //}
                    //else
                    //{
                    //    duraion = obj.DURATION_EXTEND;
                    //}

                    //var process_datetime = obj.PROCESS_CREATE_DATE;
                    //int iDay = 1;

                    //switch (process_datetime.DayOfWeek)
                    //{
                    //    case DayOfWeek.Saturday:
                    //        process_datetime = process_datetime.Date.AddMinutes(1);
                    //        break;
                    //    case DayOfWeek.Sunday:
                    //        process_datetime = process_datetime.Date.AddMinutes(1);
                    //        break;
                    //}



                    //while (iDay <= duraion)
                    //{
                    //    switch (process_datetime.DayOfWeek)
                    //    {
                    //        case DayOfWeek.Saturday:
                    //            process_datetime = process_datetime.AddDays(1);
                    //            break;
                    //        case DayOfWeek.Sunday:
                    //            process_datetime = process_datetime.AddDays(1);
                    //            break;

                    //        default:
                    //            iDay += 1;
                    //            process_datetime = process_datetime.AddDays(1);
                    //            break;
                    //    }
                    //}

                    //switch (process_datetime.DayOfWeek)
                    //{
                    //    case DayOfWeek.Saturday:
                    //        process_datetime = process_datetime.AddDays(2);
                    //        //process_datetime = process_datetime.Date.AddMinutes(1);
                    //        break;
                    //    case DayOfWeek.Sunday:
                    //        process_datetime = process_datetime.AddDays(1);
                    //       // process_datetime = process_datetime.Date.AddMinutes(1);
                    //        break;
                    //    default:
                    //        break;
                    //}
                    //obj.CURRENT_DUE_DATE = process_datetime;
                    ////--------------------------------------------------- DUE_DATE

                    ////--------------------------------------------------- DURATION
                  
                    //int totalDuration = 0;
                    //// process_datetime = obj.PROCESS_CREATE_DATE.Date;


                    //DateTime _duedate;

                 

                    //if (DateTime.Now > obj.CURRENT_DUE_DATE)
                    //{

                    //    //System.TimeSpan diffDays = DateTime.Now - _duedate;
                    //    //totalDays = Convert.ToInt16(Math.Ceiling(diffDays.TotalDays));
                    //    //totalDuration = Convert.ToInt16(Math.Floor(diffDays.TotalDays));
                    //    totalDuration = 0;
                    //    _duedate = process_datetime;

                    //    if (_duedate > _duedate.Date.AddMinutes(1))
                    //    {
                    //        _duedate = _duedate.AddDays(1);
                    //    }
                       
                    //    while (_duedate < DateTime.Now)
                    //    {
                         
                    //        switch (_duedate.DayOfWeek)
                    //        {
                    //            case DayOfWeek.Saturday:
                    //                break;
                    //            case DayOfWeek.Sunday:
                    //                break;
                    //            default:
                    //                totalDuration += 1;
                    //                break;
                    //        }
                    //        _duedate = _duedate.AddDays(1);
                    //    }
                    //}
                    //else
                    //{
                    //    totalDuration = 0;
                       
                    //   _duedate = process_datetime;
                    //    //_duedate = _duedate.Date.AddDays(-1);
                    //    while (_duedate.Date > DateTime.Now.Date )
                    //    {                          
                    //        switch (_duedate.DayOfWeek)
                    //        {
                    //            case DayOfWeek.Saturday:
                    //                break;
                    //            case DayOfWeek.Sunday:
                    //                break;
                    //            default:
                    //                totalDuration += 1;
                    //                break;
                    //        }
                    //        _duedate = _duedate.AddDays(-1);
                    //    }
                    //}

                    //if (DateTime.Now.Date > obj.CURRENT_DUE_DATE.Value.Date)
                    //{
                    //    totalDuration = totalDuration * -1;
                    //}
                    //obj.CURRENT_DURATION = totalDuration.ToString();
                    //// System.TimeSpan diffDays = obj.PROCESS_CREATE_DATE - DateTime.Now;
                    ////--------------------------------------------------- DURATION

                }
                else
                {
                    //obj.CURRENT_DURATION = "";
                    //obj.CURRENT_DUE_DATE = null;
                }
            }

        }


      


    }

    public class TU_TRACKING_WF_REPORT_MODEL_REQUEST : REQUEST_MODEL
    {
        public TU_TRACKING_WF_REPORT_MODEL data { get; set; }
    
    }

    public class TU_TRACKING_WF_REPORT_MODEL_RESULT : RESULT_MODEL
    {

        public List<TU_TRACKING_WF_REPORT_MODEL> data { get; set; }
        public int ORDER_COLUMN { get; set; }
    }

}
