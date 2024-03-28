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
using System.Data.SqlClient;
using System.Data;

namespace BLL.Helpers
{
    public class EndToEndReportHelper
    {
        public static V_ART_ENDTOEND_REPORT_RESULT GetViewEndToEndReport(V_ART_ENDTOEND_REPORT_REQUEST param)
        {
            V_ART_ENDTOEND_REPORT_RESULT Results = new V_ART_ENDTOEND_REPORT_RESULT();
            Results.data = new List<V_ART_ENDTOEND_REPORT_2>();
            Results.dataExcel = new List<V_ART_ENDTOEND_REPORT_2>();
            //if (string.IsNullOrEmpty(param.data.WORKFLOW_TYPE)){
            //    Results.status = "E";
            //    Results.msg = "Please select Workflow type.";
            //    return Results;
            //}
            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.draw = param.draw;
                return Results;
            }

            try
            {
                QueryEndToEndReport(param, ref Results);// q.OrderBy(i => i.REQUEST_NO).ThenBy(i => i.WF_NO).ThenBy(i => i.STEP_CREATE_DATE).ToList();

                Results.status = "S";
                Results.draw = param.draw;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }
        public static List<V_ART_ENDTOEND_REPORT_2> QueryEndToEndReport(V_ART_ENDTOEND_REPORT_REQUEST param, ref V_ART_ENDTOEND_REPORT_RESULT Results)
        {
            List<V_ART_ENDTOEND_REPORT_2> q = new List<V_ART_ENDTOEND_REPORT_2>();

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;
                    DateTime REQUEST_DATE_FROM = DateTime.MinValue;
                    DateTime REQUEST_DATE_TO = DateTime.MaxValue;
                    var currDate = DateTime.Now;

                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_FROM)) REQUEST_DATE_FROM = CNService.ConvertStringToDate(param.data.REQUEST_DATE_FROM);
                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_TO)) REQUEST_DATE_TO = CNService.ConvertStringToDate(param.data.REQUEST_DATE_TO);
                    string f = string.Format("{0}", Convert.ToDateTime(REQUEST_DATE_FROM).ToString("yyyyMMdd"));
                    //SqlParameter[] px = { new SqlParameter("@requestdatefrom", string.Format("{0}", Convert.ToDateTime(REQUEST_DATE_FROM).ToString("yyyyMMdd"))),
                    //new SqlParameter("@requestdateTo", string.Format("{0}", Convert.ToDateTime(REQUEST_DATE_TO).ToString("yyyyMMdd"))),
                    //new SqlParameter("@stepcreatedatefrom", string.Format("{0}", param.data.STEP_DATE_FROM)),
                    //new SqlParameter("@stepcreatedateto", string.Format("{0}", param.data.STEP_DATE_TO)),
                    //new SqlParameter("@workflow_no_2", string.Format("{0}", param.data.WORKFLOW_NO_2)),
                    //new SqlParameter("@workflow_no", string.Format("{0}", param.data.WORKFLOW_NO)),
                    //new SqlParameter("@TypeWorkflow", string.Format("{0}", param.data.WORKFLOW_TYPE))};
                    //DataTable dt = CNService.GetRelatedResources("spQueryEndToEndReport", px);
                    //var query = DataTableMappingtoModel.MappingToEntity<V_ART_ENDTOEND_REPORT_2>(dt);

                    var query = context.Database.SqlQuery<V_ART_ENDTOEND_REPORT_2>(@"spQueryEndToEndReport @requestdatefrom,@requestdateTo,
                        @stepcreatedatefrom,
                        @stepcreatedateto,@workflow_no_2,@workflow_no,
                        @TypeWorkflow,@ref_wf_no,@supervised,@sold_to,@ship_to,@_process,@_terminated,@_completed",
                        new SqlParameter("@requestdatefrom", f),
                        new SqlParameter("@requestdateTo", string.Format("{0}", Convert.ToDateTime(REQUEST_DATE_TO).ToString("yyyyMMdd"))),
                        new SqlParameter("@stepcreatedatefrom", string.Format("{0}", param.data.STEP_DATE_FROM)),
                        new SqlParameter("@stepcreatedateto", string.Format("{0}", param.data.STEP_DATE_TO)),
                        new SqlParameter("@workflow_no_2", string.Format("{0}", param.data.WORKFLOW_NO_2)),
                        new SqlParameter("@workflow_no", string.Format("{0}", param.data.WORKFLOW_NO)),
                        new SqlParameter("@TypeWorkflow", string.Format("{0}", param.data.WORKFLOW_TYPE)),
                        new SqlParameter("@ref_wf_no", string.Format("{0}", param.data.REF_WF_NO)), 
                        new SqlParameter("@supervised", string.Format("{0}", param.data.SUPERVISED_ID)),
                        new SqlParameter("@sold_to", string.Format("{0}", param.data.SOLD_TO_ID)),
                        new SqlParameter("@ship_to", string.Format("{0}", param.data.SHIP_TO_ID)),
                        new SqlParameter("@_process", string.Format("{0}", (param.data.workflow_process)?"X":"")),
                        new SqlParameter("@_terminated", string.Format("{0}", (param.data.workflow_terminated)?"X":"")),
                        new SqlParameter("@_completed", string.Format("{0}", (param.data.WORKFLOW_COMPLETED) ? "X" : ""))
                        ).ToList();

                    q = query.Select(m => new V_ART_ENDTOEND_REPORT_2()
                    {
                        PARENT_SUB_ID = m.PARENT_SUB_ID,
                        DUE_DATE = m.DUE_DATE,
                        WF_NO = m.WF_NO,
                        CURRENT_USER_ID = m.CURRENT_USER_ID,
                        REFERENCE_REQUEST_NO = m.REFERENCE_REQUEST_NO,
                        REQUEST_NO = m.REQUEST_NO,
                        IS_END = m.IS_END,
                        IS_TERMINATE = m.IS_TERMINATE,
                        SOLD_TO_ID = m.SOLD_TO_ID,
                        SHIP_TO_ID = m.SHIP_TO_ID,
                        SOLD_TO = m.SOLD_TO,
                        SHIP_TO = m.SHIP_TO,
                        COUNTRY = m.COUNTRY,
                        REQUEST_ID = m.REQUEST_ID,
                        COMPANY_ID = m.COMPANY_ID,
                        BRAND_ID = m.BRAND_ID,
                        PROJECT_NAME = m.PROJECT_NAME,
                        PRODUCT_CODE = m.PRODUCT_CODE,
                        WF_ID = m.WF_ID,
                        REFERENCE_NO = m.REFERENCE_NO,
                        PACKAGING_TYPE = m.PACKAGING_TYPE,
                        CREATOR_ID = m.CREATOR_ID,
                        THREE_P_ID = m.THREE_P_ID,
                        CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                        SALES_ORDER_NO = m.SALES_ORDER_NO,
                        ORDER_BOM_COMPONENT = m.ORDER_BOM_COMPONENT,
                        WF_STAUTS = m.WF_STAUTS,
                        CURRENT_STEP_NAME_1 = m.CURRENT_STEP_NAME_1,
                        CURRENT_USER_NAME_1 = m.CURRENT_USER_NAME_1,
                        DUE_DATE_1 = m.DUE_DATE_1,
                        BRAND_NAME = m.BRAND_NAME,
                        ADDITIONAL_BRAND_NAME = m.ADDITIONAL_BRAND_NAME,
                        PROD_INSP_MEMO = m.PROD_INSP_MEMO,
                        PLANT = m.PLANT,
                        PORT = m.PORT,
                        IN_TRANSIT_TO = m.IN_TRANSIT_TO,
                        RDD = m.RDD,
                        PA_NAME = m.PA_NAME,
                        PG_NAME = m.PG_NAME,
                        CURRENT_STEP_NAME = m.CURRENT_STEP_NAME,
                        CURRENT_USER_NAME = m.CURRENT_USER_NAME,
                        STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                        STEP_END_DATE = m.STEP_END_DATE,
                        DURATION = m.DURATION,
                        REASON = m.REASON,
                        TOTALDAY = m.TOTALDAY,
                        MARKETTING = m.MARKETTING,
                        CREATOR_NAME = m.CREATOR_NAME,
                        CREATE_ON = m.CREATE_ON,
                        USEDAY = m.USEDAY,
                        WF_SUB_ID = m.WF_SUB_ID,
                        STEP_CREATE_DATE_ORDERBY = m.STEP_CREATE_DATE,
                        WF_TYPE = m.WF_TYPE,
                        IS_STEP_DURATION_EXTEND = m.IS_STEP_DURATION_EXTEND == "X" ? "Yes" : m.IS_STEP_DURATION_EXTEND,
                        DURATION_STANDARD = m.DURATION_STANDARD,
                        STEP_END_DATE_STATIC = m.STEP_END_DATE == null ? currDate : m.STEP_END_DATE,
                        RECEIVER_REASON = m.RECEIVER_REASON,   //CR#19743 by aof
                        RECEIVER_COMMENT = m.RECEIVER_COMMENT, //CR#19743 by aof
                        TERMINATE_REASON = m.TERMINATE_REASON, //#INC-55439 by aof
                        TERMINATE_COMMENT = m.TERMINATE_COMMENT, //#INC-55439 by aof
                    }).ToList();

                    if (param.data.CUSTOMER_APPROVE_FROM > 0)
                    {
                        if (param.data.WORKFLOW_TYPE.Contains("AW"))
                        {
                            var SEND_CUS_PRINT = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_CUS_PRINT").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
                            q = q.AsQueryable().Where(e => e.USEDAY >= param.data.CUSTOMER_APPROVE_FROM && e.CURRENT_STEP_ID == SEND_CUS_PRINT).ToList();
                        }
                        if (param.data.WORKFLOW_TYPE.Contains("MO"))
                        {
                            var SEND_CUS_APP = context.ART_M_STEP_MOCKUP.Where(w => w.STEP_MOCKUP_CODE == "SEND_CUS_APP").Select(s => s.STEP_MOCKUP_ID).FirstOrDefault();
                            q = q.AsQueryable().Where(e => e.USEDAY >= param.data.CUSTOMER_APPROVE_FROM && e.CURRENT_STEP_ID == SEND_CUS_APP).ToList();
                        }

                    }

                    if (param.data.CUSTOMER_APPROVE_TO > 0)
                    {
                        if (param.data.WORKFLOW_TYPE.Contains("AW"))
                        {
                            var SEND_CUS_PRINT = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_CUS_PRINT").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
                            q = q.AsQueryable().Where(e => e.USEDAY <= param.data.CUSTOMER_APPROVE_TO && e.CURRENT_STEP_ID == SEND_CUS_PRINT).ToList();
                        }
                        if (param.data.WORKFLOW_TYPE.Contains("MO"))
                        {
                            var SEND_CUS_APP = context.ART_M_STEP_MOCKUP.Where(w => w.STEP_MOCKUP_CODE == "SEND_CUS_APP").Select(s => s.STEP_MOCKUP_ID).FirstOrDefault();
                            q = q.AsQueryable().Where(e => e.USEDAY <= param.data.CUSTOMER_APPROVE_TO && e.CURRENT_STEP_ID == SEND_CUS_APP).ToList();
                        }
                    }
                    if (param.data.WORKFLOW_OVERDUE)
                    {
                        q = q.AsQueryable().Where(e => e.DUE_DATE < e.STEP_END_DATE_STATIC).ToList();
                    }

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_TYPE))
                    {
                        q = q.AsQueryable().Where(e => e.WF_NO.StartsWith(param.data.WORKFLOW_TYPE)).ToList();
                    }

                    if (param.data.CURRENT_ASSIGN_ID > 0)
                    {
                        q = q.AsQueryable().Where(e => e.CURRENT_USER_ID == param.data.CURRENT_ASSIGN_ID).ToList();
                    }
                    //+++++++
                    //if (param.data.SUPERVISED_ID > 0)
                    //{
                    //    var listUserId = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { UPPER_USER_ID = (int)param.data.SUPERVISED_ID }, context).Select(m => m.USER_ID).ToList();
                    //    //q = q.Where(e => e.CURRENT_USER_ID != null && listUserId.Contains((int)e.CURRENT_USER_ID));
                    //    //q = q.AsQueryable().Where(e => (from ee in context.V_ART_ENDTOEND_REPORT
                    //    //                                where listUserId.Contains((int)ee.CURRENT_USER_ID)
                    //    //                                && string.IsNullOrEmpty(ee.REMARK_KILLPROCESS)
                    //    //                                && DbFunctions.TruncateTime(ee.REQUEST_CREATE_DATE) >= REQUEST_DATE_FROM && DbFunctions.TruncateTime(ee.REQUEST_CREATE_DATE) <= REQUEST_DATE_TO
                    //    //                                select ee.WF_ID).Contains(e.WF_ID)).ToList();
                    //    var listcurrentid = (from e in context.ART_WF_ARTWORK_PROCESS
                    //                         where listUserId.Contains((int)e.CURRENT_USER_ID)
                    //                        select e.ARTWORK_ITEM_ID).ToList();

                    //    q = q.AsQueryable().Where(ee => listcurrentid.Contains((int)ee.WF_ID)).ToList();
                    //}

                    if (!string.IsNullOrEmpty(param.data.REF_WF_NO))
                    {
                        //q = q.Where(e => !string.IsNullOrEmpty(e.REFERENCE_REQUEST_NO).Equals(param.data.REF_WF_NO.Trim()));
                        q = q.AsQueryable().Where(e => ((!string.IsNullOrEmpty(e.REFERENCE_REQUEST_NO) && e.REFERENCE_REQUEST_NO.Contains(param.data.REF_WF_NO.Trim()))
                        || (string.IsNullOrEmpty(e.REFERENCE_REQUEST_NO) && e.REQUEST_NO.Contains(param.data.REF_WF_NO.Trim())))).ToList();
                    }

                    //if (!string.IsNullOrEmpty(param.data.WORKFLOW_NO))
                    //{
                    //    q = q.AsQueryable().Where(e => e.WF_NO.Contains(param.data.WORKFLOW_NO.Trim())).ToList();
                    //}

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_NO_2))
                    {
                        q = q.AsQueryable().Where(e => e.REQUEST_NO.Contains(param.data.WORKFLOW_NO_2.Trim())).ToList();
                    }
                    //if (param.data.workflow_process) {
                    //    q = q.AsQueryable().Where(e => string.IsNullOrEmpty(e.IS_END) && e.CURRENT_STEP_ID == 2 && e.PARENT_SUB_ID == null).ToList();
                    //}
                    //if (param.data.workflow_terminated) {
                    //    q = q.AsQueryable().Where(e => e.IS_TERMINATE == "X" && e.CURRENT_STEP_ID == 2 && e.PARENT_SUB_ID == null).ToList();
                    //}
                    //if (param.data.WORKFLOW_COMPLETED)
                    //{
                    //    //var tempListCompleted = (from e in context.V_ART_ENDTOEND_REPORT
                    //    //                         where e.IS_END == "X"
                    //    //                         && string.IsNullOrEmpty(e.IS_TERMINATE)
                    //    //                         && DbFunctions.TruncateTime(e.REQUEST_CREATE_DATE) >= REQUEST_DATE_FROM && DbFunctions.TruncateTime(e.REQUEST_CREATE_DATE) <= REQUEST_DATE_TO
                    //    //                         && e.PARENT_SUB_ID == null
                    //    //                         select e.WF_ID).ToList();
                    //    //q = q.Where(e => tempListCompleted.Contains(e.WF_ID));
                    //    q = q.AsQueryable().Where(e => e.IS_END == "X" && string.IsNullOrEmpty(e.IS_TERMINATE) && e.PARENT_SUB_ID == null).ToList();
                    //}
                    //if (param.data.SOLD_TO_ID > 0)
                    //{
                    //    q = q.AsQueryable().Where(e => e.SOLD_TO_ID == (int)(param.data.SOLD_TO_ID)).ToList();
                    //}

                    //if (param.data.SHIP_TO_ID > 0)
                    //{
                    //    q = q.AsQueryable().Where(e => e.SHIP_TO_ID == (int)(param.data.SHIP_TO_ID)).ToList();
                    //}

                    if (param.data.COUNTRY_ID > 0)
                    {
                        var country = SAP_M_COUNTRY_SERVICE.GetByItem(new SAP_M_COUNTRY() { COUNTRY_ID = (int)param.data.COUNTRY_ID }, context).FirstOrDefault();
                        q = q.AsQueryable().Where(e => e.COUNTRY.Contains(country.COUNTRY_CODE + ":" + country.NAME)).ToList();
                    }

                    if (!string.IsNullOrEmpty(param.data.ZONE_TXT))
                    {
                        var listCountryId = SAP_M_COUNTRY_SERVICE.GetByItem(new SAP_M_COUNTRY() { ZONE = param.data.ZONE_TXT }, context).Select(m => m.COUNTRY_ID);
                        if (param.data.WORKFLOW_TYPE.Contains("MO")) { //mockup
                            var listCheckListId = (from r in context.ART_WF_MOCKUP_CHECK_LIST_COUNTRY
                                               where listCountryId.Contains(r.COUNTRY_ID)
                                               select r.CHECK_LIST_ID).ToList();
                            q = q.AsQueryable().Where(e => listCheckListId.Contains(e.REQUEST_ID)).ToList();
                        }
                        if (param.data.WORKFLOW_TYPE.Contains("AW")){//artwork
                            var listARTWORK_REQUEST_ID = (from r in context.ART_WF_ARTWORK_REQUEST_COUNTRY
                                                          where listCountryId.Contains(r.COUNTRY_ID)
                                                          select r.ARTWORK_REQUEST_ID).ToList();

                            q = q.AsQueryable().Where(e => listARTWORK_REQUEST_ID.Contains(e.REQUEST_ID)).ToList();
                        }
                    }

                    if (param.data.COMPANY_ID > 0)
                    {
                        q = q.AsQueryable().Where(e => e.COMPANY_ID == (int)(param.data.COMPANY_ID)).ToList();
                    }

                    if (param.data.BRAND_ID > 0)
                    {
                        q = q.AsQueryable().Where(e => e.BRAND_ID == (int)(param.data.BRAND_ID)).ToList();
                    }

                    if (!string.IsNullOrEmpty(param.data.PROJECT_NAME))
                    {
                        q = q.AsQueryable().Where(e => e.PROJECT_NAME.Contains(param.data.PROJECT_NAME)).ToList();
                    }

                    if (!string.IsNullOrEmpty(param.data.PRODUCT_CODE))//********************* multi *********************
                    {
                        if (param.data.WORKFLOW_TYPE.Contains("AW")) { 
                        var arrProductCode = param.data.PRODUCT_CODE.Replace(" ", "").Split(',');
                        List<int> listArtworkRequestId = context.Database.SqlQuery<int>(@"spGetENDProduct @Product",
                        new SqlParameter("@Product", string.Format("{0}", param.data.PRODUCT_CODE))).ToList();
                        q = q.AsQueryable().Where(r => listArtworkRequestId.Contains(r.WF_ID)).ToList();
                        }
                        if (param.data.WORKFLOW_TYPE.Contains("MO"))
                        {
                            List<int> listMockProduct = context.Database.SqlQuery<int>(@"spGetmockupprod @Product",
                                 new SqlParameter("@Product", string.Format("{0}", param.data.PRODUCT_CODE))).ToList();
                            q = q.AsQueryable().Where(r => listMockProduct.Contains(r.REQUEST_ID)).ToList();
                        }
                    }

                if (!string.IsNullOrEmpty(param.data.RD_NUMBER))
                {
                    q = q.AsQueryable().Where(e => !string.IsNullOrEmpty(e.REFERENCE_NO) && e.REFERENCE_NO.Contains(param.data.RD_NUMBER.Trim())).ToList();
                }

                if (param.data.PACKAGING_TYPE_ID > 0)
                {
                    var packaging = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(new SAP_M_CHARACTERISTIC() { CHARACTERISTIC_ID = (int)param.data.PACKAGING_TYPE_ID }, context).FirstOrDefault();
                    //q = q.Where(e => !string.IsNullOrEmpty(e.PACKAGING_TYPE) && e.PACKAGING_TYPE.Contains(packaging.VALUE + ":" + packaging.DESCRIPTION));
                    q = q.AsQueryable().Where(e => !string.IsNullOrEmpty(e.PACKAGING_TYPE) && e.PACKAGING_TYPE.Contains(packaging.VALUE + ":" + packaging.DESCRIPTION)).ToList();
                }

                if (param.data.CREATOR_ID > 0)
                {
                    //q = q.Where(e => e.CREATOR_ID == param.data.CREATOR_ID);
                    q = q.AsQueryable().Where(e => e.CREATOR_ID == param.data.CREATOR_ID).ToList();
                }

                if (!string.IsNullOrEmpty(param.data.PRIMARY_SIZE_TXT))
                {
                    List<int> listThreePID = context.SAP_M_3P.Where(s => s.PRIMARY_SIZE_VALUE.Replace(" ","").Contains(param.data.PRIMARY_SIZE_TXT.Replace(" ", "").Trim())).Select(s => s.THREE_P_ID).ToList();
                    //search from SAP_M_3P
                    if (param.data.WORKFLOW_TYPE.Contains("MO"))
                    { //search from request id
                        List<int> listChecklistId = context.ART_WF_MOCKUP_CHECK_LIST_PRODUCT.Where(p => p.PRIMARY_SIZE.Replace(" ", "").Contains(param.data.PRIMARY_SIZE_TXT.Replace(" ", "").Trim())).Select(p => p.CHECK_LIST_ID).ToList();
                        q = q.AsQueryable().Where(e => (e.THREE_P_ID != null && listThreePID.Contains((int)e.THREE_P_ID) || listChecklistId.Contains(e.REQUEST_ID))).ToList();
                    }
                    else if (param.data.WORKFLOW_TYPE.Contains("AW"))
                    {
                        List<int> listProductId = context.XECM_M_PRODUCT.Where(s => s.PRIMARY_SIZE.Replace(" ", "").Contains(param.data.PRIMARY_SIZE_TXT.Replace(" ", "").Trim()) && s.PRIMARY_SIZE.Replace(" ", "") != null).Select(s => s.XECM_PRODUCT_ID).ToList();
                        List<int> listRequestId = context.ART_WF_ARTWORK_REQUEST_PRODUCT.Where(p => listProductId.Contains(p.PRODUCT_CODE_ID)).Select(p => p.ARTWORK_REQUEST_ID).ToList();
                        List<int> listRequest3PId = context.ART_WF_ARTWORK_REQUEST.Where(p => listThreePID.Contains((int)p.THREE_P_ID)).Select(p => p.ARTWORK_REQUEST_ID).ToList();
                        //q = q.Where(e => (e.THREE_P_ID != null && listThreePID.Contains((int)e.THREE_P_ID)) || listChecklistId.Contains(e.REQUEST_ID) || listRequestId.Contains(e.REQUEST_ID));
                        q = q.AsQueryable().Where(e => (e.THREE_P_ID != null && listThreePID.Contains((int)e.THREE_P_ID)) || listRequestId.Contains(e.REQUEST_ID) || listRequest3PId.Contains(e.REQUEST_ID)).ToList();
                    }
                }
                if (!string.IsNullOrEmpty(param.data.NET_WEIGHT_TXT))
                {
                    if (param.data.WORKFLOW_TYPE.Contains("AW"))
                    {
                        List<int> listNetProduct = context.Database.SqlQuery<int>(@"spGetnet_weight @net_weight",
                        new SqlParameter("@net_weight", string.Format("{0}", param.data.NET_WEIGHT_TXT))).ToList();
                        q = q.AsQueryable().Where(e => listNetProduct.Contains(e.REQUEST_ID)).ToList();
                            //List<int> listAWNetWeight = context.ART_WF_ARTWORK_REQUEST_REFERENCE.Where(w => w.NET_WEIGHT.Contains(param.data.NET_WEIGHT_TXT.Trim())).Select(s => s.ARTWORK_REQUEST_ID).ToList();
                            //q = q.AsQueryable().Where(e => listAWNetWeight.Contains(e.REQUEST_ID) || listNetProduct.Contains(e.REQUEST_ID)).ToList();
                        }
                    if (param.data.WORKFLOW_TYPE.Contains("MO"))
                    {
                        List<int> listMockNetWeight = context.Database.SqlQuery<int>(@"spGetmockupnetweight @netweight",
                             new SqlParameter("@netweight", string.Format("{0}", param.data.NET_WEIGHT_TXT))).ToList();
                        //q = q.Where(e => listAWNetWeight.Contains(e.REQUEST_ID) || listMockNetWeight.Contains(e.REQUEST_ID));
                        q = q.AsQueryable().Where(e => listMockNetWeight.Contains(e.REQUEST_ID)).ToList();
                    }
                }

                if (!string.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE))
                {
                    if (param.data.CURRENT_STEP_WF_TYPE.StartsWith("M"))
                    {
                        var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP_WF_TYPE.Replace("M", ""));
                        q = q.AsQueryable().Where(e => e.CURRENT_STEP_ID == current_step_id && e.WF_TYPE.Equals("Mockup")).ToList();
                    }
                    else
                    {
                        var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP_WF_TYPE.Replace("A", ""));
                        q = q.AsQueryable().Where(e => e.CURRENT_STEP_ID == current_step_id && e.WF_TYPE.Equals("Artwork")).ToList();
                    }
                }

                //if (param.data.CURRENT_STEP_ID != null)
                //{
                //    listReport = listReport.Where(e => e.CURRENT_STEP_ID == Convert.ToInt32(param.data.CURRENT_STEP_ID)).ToList();
                //}

                if (!string.IsNullOrEmpty(param.data.SEARCH_SO))//********************* multi *********************
                {
                    //string[] arrSO = param.data.SEARCH_SO.Split(',');
                    //arrSO = arrSO.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    //List<int> arrWFId = new List<int>();
                    //for (var i = 0; i < arrSO.Length; i++)
                    //{
                    //    string so = arrSO[i];
                    //    arrWFId.InsertRange(0, q.Where(e => !string.IsNullOrEmpty(e.SALES_ORDER_NO) && e.SALES_ORDER_NO.Contains(so)).Select(e => e.WF_ID).ToList());
                    //}
                    //q = q.Where(e => arrWFId.Contains(e.WF_ID));
                    //List<string> arrSO = param.data.SEARCH_SO.Replace(" ", "").Split(',').ToList();
                    //q = q.AsQueryable().Where(e => !string.IsNullOrEmpty(e.SALES_ORDER_NO) &&
                    //e.SALES_ORDER_NO.Split(',').Where(p => arrSO.Contains(p.Trim().Substring(0,9))).ToList().Count() > 0).ToList();
                    var arrSO = param.data.SEARCH_SO.Replace(" ", "").Split(',');
                    var listArtworkRequestId = (from m in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
                                                join m2 in context.ART_WF_ARTWORK_PROCESS on m.ARTWORK_SUB_ID equals m2.ARTWORK_SUB_ID
                                                where arrSO.Contains(m.SALES_ORDER_NO)
                                                select m2.ARTWORK_ITEM_ID).ToList();
                    q = q.AsQueryable().Where(r => listArtworkRequestId.Contains(r.WF_ID)).ToList();
                }

                if (!string.IsNullOrEmpty(param.data.SEARCH_ORDER_BOM))//********************* multi *********************
                {
                    //string[] arrOrderBOM = param.data.SEARCH_ORDER_BOM.Split(',');
                    //arrOrderBOM = arrOrderBOM.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    //List<int> arrWFId = new List<int>();
                    //for (var i = 0; i < arrOrderBOM.Length; i++)
                    //{
                    //    string orderBom = arrOrderBOM[i];
                    //    arrWFId.InsertRange(0, q.Where(e => !string.IsNullOrEmpty(e.ORDER_BOM_COMPONENT) && e.ORDER_BOM_COMPONENT.Contains(orderBom)).Select(e => e.WF_ID).ToList());
                    //}
                    //q = q.Where(e => arrWFId.Contains(e.WF_ID));
                    var arrOrderBOM = param.data.SEARCH_ORDER_BOM.Replace(" ", "").Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                    var listArtworkRequestId = (from m in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT
                                                join m2 in context.ART_WF_ARTWORK_PROCESS on m.ARTWORK_SUB_ID equals m2.ARTWORK_SUB_ID
                                                where arrOrderBOM.Contains(m.COMPONENT_MATERIAL)
                                                select m2.ARTWORK_ITEM_ID).ToList();

                    //q = (from r in q
                    //     where listArtworkRequestId.Contains(r.WF_ID)
                    //     select r);
                    q = q.AsQueryable().Where(r => listArtworkRequestId.Contains(r.WF_ID)).ToList();

                }

                if (!string.IsNullOrEmpty(param.data.SO_CREATE_DATE_FROM))
                {
                    q = q.AsQueryable().Where(e => !string.IsNullOrEmpty(e.CREATE_ON) &&
                    e.CREATE_ON.Split(',').Where(p => CNService.ConvertStringToDate(p) >= CNService.ConvertStringToDate(param.data.SO_CREATE_DATE_FROM)).ToList().Count() > 0).ToList();
                }

                if (!string.IsNullOrEmpty(param.data.SO_CREATE_DATE_TO))
                {
                    q = q.AsQueryable().Where(e => !string.IsNullOrEmpty(e.CREATE_ON) &&
                    e.CREATE_ON.Split(',').Where(p => CNService.ConvertStringToDate(p) <= CNService.ConvertStringToDate(param.data.SO_CREATE_DATE_TO)).ToList().Count() > 0).ToList();
                }


                if (param.data.END_TO_END_FROM > 0)
                {
                    q = q.AsQueryable().Where(e => e.TOTALDAY >= param.data.END_TO_END_FROM).ToList();
                }

                if (param.data.END_TO_END_TO > 0)
                {
                    q = q.AsQueryable().Where(e => e.TOTALDAY <= param.data.END_TO_END_TO).ToList();
                }


                if (!string.IsNullOrEmpty(param.data.STEP_DATE_FROM))
                {
                    DateTime STEP_DATE_FROM = new DateTime();
                    STEP_DATE_FROM = CNService.ConvertStringToDate(param.data.STEP_DATE_FROM);
                    q = q.AsQueryable().Where(p => new DateTime(p.STEP_CREATE_DATE.Year, p.STEP_CREATE_DATE.Month, p.STEP_CREATE_DATE.Day) >= STEP_DATE_FROM).AsEnumerable().ToList();
                }

                if (!string.IsNullOrEmpty(param.data.STEP_DATE_TO))
                {
                    DateTime STEP_DATE_TO = new DateTime();
                    STEP_DATE_TO = CNService.ConvertStringToDate(param.data.STEP_DATE_TO);
                    q = q.AsQueryable().Where(p => new DateTime(p.STEP_CREATE_DATE.Year, p.STEP_CREATE_DATE.Month, p.STEP_CREATE_DATE.Day) <= STEP_DATE_TO).AsEnumerable().ToList();
                }
            
                    //Results.data = q;
                    OrderByEndToEndReport(param, q, ref Results);
                }
            }

            //foreach (var item in Results.data)
            //{
            //    item.COUNTRY = CNService.removeLastComma(item.COUNTRY);
            //    item.PORT = CNService.removeLastComma(item.PORT);
            //    item.IN_TRANSIT_TO = CNService.removeLastComma(item.IN_TRANSIT_TO);
            //    item.PLANT = CNService.removeLastComma(item.PLANT);
            //    item.PROD_INSP_MEMO = CNService.removeLastComma(item.PROD_INSP_MEMO);
            //    item.PRODUCT_CODE = CNService.removeLastComma(item.PRODUCT_CODE);
            //    item.ADDITIONAL_BRAND_NAME = CNService.removeLastComma(item.ADDITIONAL_BRAND_NAME);
            //    item.SALES_ORDER_NO = CNService.removeLastComma(item.SALES_ORDER_NO);
            //    item.RD_NUMBER = CNService.removeLastComma(item.RD_NUMBER);
            //    item.RDD = CNService.removeLastComma(item.RDD);
            //    item.CREATE_ON = CNService.removeLastComma(item.CREATE_ON);

            //    if (item.IS_STEP_DURATION_EXTEND == "X")
            //    {
            //        item.IS_STEP_DURATION_EXTEND = "Yes";
            //    }
            //}

            //foreach (var item in Results.dataExcel)
            //{
            //    item.COUNTRY = CNService.removeLastComma(item.COUNTRY);
            //    item.PORT = CNService.removeLastComma(item.PORT);
            //    item.IN_TRANSIT_TO = CNService.removeLastComma(item.IN_TRANSIT_TO);
            //    item.PLANT = CNService.removeLastComma(item.PLANT);
            //    item.PROD_INSP_MEMO = CNService.removeLastComma(item.PROD_INSP_MEMO);
            //    item.PRODUCT_CODE = CNService.removeLastComma(item.PRODUCT_CODE);
            //    item.ADDITIONAL_BRAND_NAME = CNService.removeLastComma(item.ADDITIONAL_BRAND_NAME);
            //    item.SALES_ORDER_NO = CNService.removeLastComma(item.SALES_ORDER_NO);
            //    item.RD_NUMBER = CNService.removeLastComma(item.RD_NUMBER);
            //    item.RDD = CNService.removeLastComma(item.RDD);
            //    item.CREATE_ON = CNService.removeLastComma(item.CREATE_ON);

            //    if (item.IS_STEP_DURATION_EXTEND == "X")
            //    {
            //        item.IS_STEP_DURATION_EXTEND = "Yes";
            //    }
            //}

            return Results.data;
        }
        public static List<V_ART_ENDTOEND_REPORT_2> QueryEndToEndReport2(V_ART_ENDTOEND_REPORT_REQUEST param, ref V_ART_ENDTOEND_REPORT_RESULT Results)
        {
            IQueryable<V_ART_ENDTOEND_REPORT_2> q;

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;

                    DateTime REQUEST_DATE_FROM = DateTime.MinValue;
                    DateTime REQUEST_DATE_TO = DateTime.MaxValue;
                    var currDate = DateTime.Now;

                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_FROM)) REQUEST_DATE_FROM = CNService.ConvertStringToDate(param.data.REQUEST_DATE_FROM);
                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_TO)) REQUEST_DATE_TO = CNService.ConvertStringToDate(param.data.REQUEST_DATE_TO);

                    q = (from m in context.V_ART_ENDTOEND_REPORT
                         where DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) >= REQUEST_DATE_FROM && DbFunctions.TruncateTime(m.REQUEST_CREATE_DATE) <= REQUEST_DATE_TO
                         select new V_ART_ENDTOEND_REPORT_2
                         {
                             PARENT_SUB_ID = m.PARENT_SUB_ID,
                             DUE_DATE = m.DUE_DATE,
                             WF_NO = m.WF_NO,
                             CURRENT_USER_ID = m.CURRENT_USER_ID,
                             REFERENCE_REQUEST_NO = m.REFERENCE_REQUEST_NO,
                             REQUEST_NO = m.REQUEST_NO,
                             IS_END = m.IS_END,
                             IS_TERMINATE = m.IS_TERMINATE,
                             SOLD_TO_ID = m.SOLD_TO_ID,
                             SHIP_TO_ID = m.SHIP_TO_ID,
                             SOLD_TO = m.SOLD_TO,
                             SHIP_TO = m.SHIP_TO,
                             COUNTRY = m.COUNTRY,
                             REQUEST_ID = m.REQUEST_ID,
                             COMPANY_ID = m.COMPANY_ID,
                             BRAND_ID = m.BRAND_ID,
                             PROJECT_NAME = m.PROJECT_NAME,
                             PRODUCT_CODE = m.PRODUCT_CODE,
                             WF_ID = m.WF_ID,
                             REFERENCE_NO = m.REFERENCE_NO,
                             PACKAGING_TYPE = m.PACKAGING_TYPE,
                             CREATOR_ID = m.CREATOR_ID,
                             THREE_P_ID = m.THREE_P_ID,
                             CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                             SALES_ORDER_NO = m.SALES_ORDER_NO,
                             ORDER_BOM_COMPONENT = m.ORDER_BOM_COMPONENT,
                             WF_STAUTS = m.WF_STAUTS,
                             CURRENT_STEP_NAME_1 = m.CURRENT_STEP_NAME_1,
                             CURRENT_USER_NAME_1 = m.CURRENT_USER_NAME_1,
                             DUE_DATE_1 = m.DUE_DATE_1,
                             BRAND_NAME = m.BRAND_NAME,
                             ADDITIONAL_BRAND_NAME = m.ADDITIONAL_BRAND_NAME,
                             PROD_INSP_MEMO = m.PROD_INSP_MEMO,
                             PLANT = m.PLANT,
                             PORT = m.PORT,
                             IN_TRANSIT_TO = m.IN_TRANSIT_TO,
                             RDD = m.RDD,
                             PA_NAME = m.PA_NAME,
                             PG_NAME = m.PG_NAME,
                             CURRENT_STEP_NAME = m.CURRENT_STEP_NAME,
                             CURRENT_USER_NAME = m.CURRENT_USER_NAME,
                             STEP_CREATE_DATE = m.STEP_CREATE_DATE,
                             STEP_END_DATE = m.STEP_END_DATE,
                             DURATION = m.DURATION,
                             REASON = m.REASON,
                             TOTALDAY = m.TOTAL_DAY,
                             MARKETTING = m.MARKETTING,
                             CREATOR_NAME = m.CREATOR_NAME,
                             CREATE_ON = m.CREATE_ON,
                             USEDAY = m.USE_DAY,
                             WF_SUB_ID = m.WF_SUB_ID,
                             STEP_CREATE_DATE_ORDERBY = m.STEP_CREATE_DATE,
                             WF_TYPE = m.WF_TYPE,
                             IS_STEP_DURATION_EXTEND = m.IS_STEP_DURATION_EXTEND,
                             DURATION_STANDARD = m.DURATION_STANDARD,
                             STEP_END_DATE_STATIC = m.STEP_END_DATE == null ? currDate : m.STEP_END_DATE,
                         });


                    if (param.data.WORKFLOW_OVERDUE)
                    {
                        q = q.Where(e => e.DUE_DATE < e.STEP_END_DATE_STATIC);
                    }

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_TYPE))
                    {
                        q = q.Where(e => e.WF_NO.StartsWith(param.data.WORKFLOW_TYPE));
                    }

                    if (param.data.CURRENT_ASSIGN_ID > 0)
                    {
                        q = q.Where(e => e.CURRENT_USER_ID == param.data.CURRENT_ASSIGN_ID);
                    }

                    if (param.data.SUPERVISED_ID > 0)
                    {
                        var listUserId = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { UPPER_USER_ID = (int)param.data.SUPERVISED_ID }, context).Select(m => m.USER_ID).ToList();
                        //q = q.Where(e => e.CURRENT_USER_ID != null && listUserId.Contains((int)e.CURRENT_USER_ID));

                        q = q.Where(e => (from ee in context.V_ART_ENDTOEND_REPORT
                                          where listUserId.Contains((int)ee.CURRENT_USER_ID)
                                          && string.IsNullOrEmpty(ee.REMARK_KILLPROCESS)
                                          && DbFunctions.TruncateTime(ee.REQUEST_CREATE_DATE) >= REQUEST_DATE_FROM && DbFunctions.TruncateTime(ee.REQUEST_CREATE_DATE) <= REQUEST_DATE_TO
                                          select ee.WF_ID).Contains(e.WF_ID));
                    }

                    if (!string.IsNullOrEmpty(param.data.REF_WF_NO))
                    {
                        //q = q.Where(e => !string.IsNullOrEmpty(e.REFERENCE_REQUEST_NO).Equals(param.data.REF_WF_NO.Trim()));
                        q = q.Where(e => ((!string.IsNullOrEmpty(e.REFERENCE_REQUEST_NO) && e.REFERENCE_REQUEST_NO.Contains(param.data.REF_WF_NO.Trim()))
                        || (string.IsNullOrEmpty(e.REFERENCE_REQUEST_NO) && e.REQUEST_NO.Contains(param.data.REF_WF_NO.Trim()))));
                    }

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_NO))
                    {
                        q = q.Where(e => e.WF_NO.Contains(param.data.WORKFLOW_NO.Trim()));
                    }

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_NO_2))
                    {
                        q = q.Where(e => e.REQUEST_NO.Contains(param.data.WORKFLOW_NO_2.Trim()));
                    }

                    if (param.data.WORKFLOW_COMPLETED)
                    {
                        //var tempListCompleted = (from e in context.V_ART_ENDTOEND_REPORT
                        //                         where e.IS_END == "X"
                        //                         && string.IsNullOrEmpty(e.IS_TERMINATE)
                        //                         && DbFunctions.TruncateTime(e.REQUEST_CREATE_DATE) >= REQUEST_DATE_FROM && DbFunctions.TruncateTime(e.REQUEST_CREATE_DATE) <= REQUEST_DATE_TO
                        //                         && e.PARENT_SUB_ID == null
                        //                         select e.WF_ID).ToList();
                        //q = q.Where(e => tempListCompleted.Contains(e.WF_ID));
                        q = q.Where(e => e.IS_END == "X" && string.IsNullOrEmpty(e.IS_TERMINATE) && e.PARENT_SUB_ID == null);
                    }

                    if (param.data.SOLD_TO_ID > 0)
                    {
                        q = q.Where(e => e.SOLD_TO_ID == (int)(param.data.SOLD_TO_ID));
                    }

                    if (param.data.SHIP_TO_ID > 0)
                    {
                        q = q.Where(e => e.SHIP_TO_ID == (int)(param.data.SHIP_TO_ID));
                    }

                    if (param.data.COUNTRY_ID > 0)
                    {
                        var country = SAP_M_COUNTRY_SERVICE.GetByItem(new SAP_M_COUNTRY() { COUNTRY_ID = (int)param.data.COUNTRY_ID }, context).FirstOrDefault();
                        q = q.Where(e => e.COUNTRY.Contains(country.COUNTRY_CODE + ":" + country.NAME));
                    }

                    if (!string.IsNullOrEmpty(param.data.ZONE_TXT))
                    {
                        var listCountryId = SAP_M_COUNTRY_SERVICE.GetByItem(new SAP_M_COUNTRY() { ZONE = param.data.ZONE_TXT }, context).Select(m => m.COUNTRY_ID);
                        //mockup
                        var listCheckListId = (from r in context.ART_WF_MOCKUP_CHECK_LIST_COUNTRY
                                               where listCountryId.Contains(r.COUNTRY_ID)
                                               select r.CHECK_LIST_ID).ToList();

                        //artwork
                        var listARTWORK_REQUEST_ID = (from r in context.ART_WF_ARTWORK_REQUEST_COUNTRY
                                                      where listCountryId.Contains(r.COUNTRY_ID)
                                                      select r.ARTWORK_REQUEST_ID).ToList();

                        q = q.Where(e => listCheckListId.Contains(e.REQUEST_ID) || listARTWORK_REQUEST_ID.Contains(e.REQUEST_ID));
                    }

                    if (param.data.COMPANY_ID > 0)
                    {
                        q = q.Where(e => e.COMPANY_ID == (int)(param.data.COMPANY_ID));
                    }

                    if (param.data.BRAND_ID > 0)
                    {
                        q = q.Where(e => e.BRAND_ID == (int)(param.data.BRAND_ID));
                    }

                    if (!string.IsNullOrEmpty(param.data.PROJECT_NAME))
                    {
                        q = q.Where(e => e.PROJECT_NAME.Contains(param.data.PROJECT_NAME));
                    }

                    if (!string.IsNullOrEmpty(param.data.PRODUCT_CODE))//********************* multi *********************
                    {
                            var arrProductCode = param.data.PRODUCT_CODE.Replace(" ", "").Split(',');
                            var listArtworkRequestId = (from m in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM
                                                        join m2 in context.ART_WF_ARTWORK_PROCESS on m.ARTWORK_SUB_ID equals m2.ARTWORK_SUB_ID
                                                        where arrProductCode.Contains(m.PRODUCT_CODE)
                                                        select m2.ARTWORK_ITEM_ID).ToList();
                            q = (from r in q
                                 where listArtworkRequestId.Contains(r.WF_ID)
                                 select r);

                    }

                    if (!string.IsNullOrEmpty(param.data.RD_NUMBER))
                    {
                        q = q.Where(e => !string.IsNullOrEmpty(e.REFERENCE_NO) && e.REFERENCE_NO.Contains(param.data.RD_NUMBER.Trim()));
                    }

                    if (param.data.PACKAGING_TYPE_ID > 0)
                    {
                        var packaging = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(new SAP_M_CHARACTERISTIC() { CHARACTERISTIC_ID = (int)param.data.PACKAGING_TYPE_ID }, context).FirstOrDefault();
                        q = q.Where(e => !string.IsNullOrEmpty(e.PACKAGING_TYPE) && e.PACKAGING_TYPE.Contains(packaging.VALUE + ":" + packaging.DESCRIPTION));
                    }

                    if (param.data.CREATOR_ID > 0)
                    {
                        q = q.Where(e => e.CREATOR_ID == param.data.CREATOR_ID);
                    }

                    if (!string.IsNullOrEmpty(param.data.PRIMARY_SIZE_TXT))
                    {
                        //search from SAP_M_3P
                        List<int> listThreePID = SAP_M_3P_SERVICE.GetAll(context).Where(s => s.PRIMARY_SIZE_VALUE.Contains(param.data.PRIMARY_SIZE_TXT.Trim())).Select(s => s.THREE_P_ID).ToList();

                        //search from request id
                        List<int> listChecklistId = ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetAll(context).Where(p => p.PRIMARY_SIZE.Contains(param.data.PRIMARY_SIZE_TXT.Trim())).Select(p => p.CHECK_LIST_ID).ToList();
                        List<int> listProductId = XECM_M_PRODUCT_SERVICE.GetAll(context).Where(p => p.PRIMARY_SIZE != null && p.PRIMARY_SIZE.Contains(param.data.PRIMARY_SIZE_TXT.Trim())).Select(p => p.XECM_PRODUCT_ID).ToList();
                        List<int> listRequestId = ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetAll(context).Where(p => listProductId.Contains(p.PRODUCT_CODE_ID)).Select(p => p.ARTWORK_REQUEST_ID).ToList();

                        q = q.Where(e => (e.THREE_P_ID != null && listThreePID.Contains((int)e.THREE_P_ID)) || listChecklistId.Contains(e.REQUEST_ID) || listRequestId.Contains(e.REQUEST_ID));
                    }

                    if (!string.IsNullOrEmpty(param.data.NET_WEIGHT_TXT))
                    {
                        List<int> listAWNetWeight = ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_REFERENCE() { NET_WEIGHT = param.data.NET_WEIGHT_TXT.Trim() }, context).Select(e => e.ARTWORK_REQUEST_ID).ToList();
                        List<int> listMockNetWeight = ART_WF_MOCKUP_CHECK_LIST_REFERENCE_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_REFERENCE() { NET_WEIGHT = param.data.NET_WEIGHT_TXT.Trim() }, context).Select(e => e.CHECK_LIST_ID).ToList();

                        q = q.Where(e => listAWNetWeight.Contains(e.REQUEST_ID) || listMockNetWeight.Contains(e.REQUEST_ID));
                    }

                    if (!string.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE))
                    {
                        if (param.data.CURRENT_STEP_WF_TYPE.StartsWith("M"))
                        {
                            var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP_WF_TYPE.Replace("M", ""));
                            q = q.Where(e => e.CURRENT_STEP_ID == current_step_id && e.WF_TYPE.Equals("Mockup"));
                        }
                        else
                        {
                            var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP_WF_TYPE.Replace("A", ""));
                            q = q.Where(e => e.CURRENT_STEP_ID == current_step_id && e.WF_TYPE.Equals("Artwork"));
                        }
                    }

                    //if (param.data.CURRENT_STEP_ID != null)
                    //{
                    //    listReport = listReport.Where(e => e.CURRENT_STEP_ID == Convert.ToInt32(param.data.CURRENT_STEP_ID)).ToList();
                    //}

                    if (!string.IsNullOrEmpty(param.data.SEARCH_SO))//********************* multi *********************
                    {
                        //string[] arrSO = param.data.SEARCH_SO.Split(',');
                        //arrSO = arrSO.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        //List<int> arrWFId = new List<int>();
                        //for (var i = 0; i < arrSO.Length; i++)
                        //{
                        //    string so = arrSO[i];
                        //    arrWFId.InsertRange(0, q.Where(e => !string.IsNullOrEmpty(e.SALES_ORDER_NO) && e.SALES_ORDER_NO.Contains(so)).Select(e => e.WF_ID).ToList());
                        //}
                        //q = q.Where(e => arrWFId.Contains(e.WF_ID));

                        var arrSO = param.data.SEARCH_SO.Replace(" ", "").Split(',');
                        var listArtworkRequestId = (from m in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
                                                    join m2 in context.ART_WF_ARTWORK_PROCESS on m.ARTWORK_SUB_ID equals m2.ARTWORK_SUB_ID
                                                    where arrSO.Contains(m.SALES_ORDER_NO)
                                                    select m2.ARTWORK_ITEM_ID).ToList();
                        q = (from r in q
                             where listArtworkRequestId.Contains(r.WF_ID)
                             select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_ORDER_BOM))//********************* multi *********************
                    {
                        //string[] arrOrderBOM = param.data.SEARCH_ORDER_BOM.Split(',');
                        //arrOrderBOM = arrOrderBOM.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                        //List<int> arrWFId = new List<int>();
                        //for (var i = 0; i < arrOrderBOM.Length; i++)
                        //{
                        //    string orderBom = arrOrderBOM[i];
                        //    arrWFId.InsertRange(0, q.Where(e => !string.IsNullOrEmpty(e.ORDER_BOM_COMPONENT) && e.ORDER_BOM_COMPONENT.Contains(orderBom)).Select(e => e.WF_ID).ToList());
                        //}
                        //q = q.Where(e => arrWFId.Contains(e.WF_ID));

                        var arrOrderBOM = param.data.SEARCH_ORDER_BOM.Replace(" ", "").Split(',');
                        var listArtworkRequestId = (from m in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT
                                                    join m2 in context.ART_WF_ARTWORK_PROCESS on m.ARTWORK_SUB_ID equals m2.ARTWORK_SUB_ID
                                                    where arrOrderBOM.Contains(m.COMPONENT_MATERIAL)
                                                    select m2.ARTWORK_ITEM_ID).ToList();

                        q = (from r in q
                             where listArtworkRequestId.Contains(r.WF_ID)
                             select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.SO_CREATE_DATE_FROM))
                    {
                        //q = q.Where(e => !string.IsNullOrEmpty(e.CREATE_ON) && CNService.ConvertStringToDate(e.CREATE_ON) >= CNService.ConvertStringToDate(param.data.SO_CREATE_DATE_FROM));
                    }

                    if (!string.IsNullOrEmpty(param.data.SO_CREATE_DATE_TO))
                    {
                        //q = q.Where(e => !string.IsNullOrEmpty(e.CREATE_ON) && CNService.ConvertStringToDate(e.CREATE_ON) <= CNService.ConvertStringToDate(param.data.SO_CREATE_DATE_TO));
                    }

                    if (param.data.CUSTOMER_APPROVE_FROM > 0)
                    {
       
                            var SEND_CUS_PRINT = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_CUS_PRINT").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
                        q = q.Where(e => e.USEDAY >= param.data.CUSTOMER_APPROVE_FROM && e.CURRENT_STEP_ID == SEND_CUS_PRINT);
 

                    }

                    if (param.data.CUSTOMER_APPROVE_TO > 0)
                    {
    
                            var SEND_CUS_PRINT = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_CUS_PRINT").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
                        q = q.Where(e => e.USEDAY <= param.data.CUSTOMER_APPROVE_TO && e.CURRENT_STEP_ID == SEND_CUS_PRINT);
       
                }

                    if (param.data.END_TO_END_FROM > 0)
                    {
                        q = q.Where(e => e.TOTALDAY >= param.data.END_TO_END_FROM);
                    }

                    if (param.data.END_TO_END_TO > 0)
                    {
                        q = q.Where(e => e.TOTALDAY <= param.data.END_TO_END_TO);
                    }


                    if (!string.IsNullOrEmpty(param.data.STEP_DATE_FROM))
                    {
                        DateTime STEP_DATE_FROM = new DateTime();
                        STEP_DATE_FROM = CNService.ConvertStringToDate(param.data.STEP_DATE_FROM);
                        q = (from p in q
                             where DbFunctions.TruncateTime(p.STEP_CREATE_DATE) >= STEP_DATE_FROM
                             select p);
                    }

                    if (!string.IsNullOrEmpty(param.data.STEP_DATE_TO))
                    {
                        DateTime STEP_DATE_TO = new DateTime();
                        STEP_DATE_TO = CNService.ConvertStringToDate(param.data.STEP_DATE_TO);
                        q = (from p in q
                             where DbFunctions.TruncateTime(p.STEP_CREATE_DATE) <= STEP_DATE_TO
                             select p);
                    }


                    OrderByEndToEndReport(param, q.ToList(), ref Results);
                }
            }

            foreach (var item in Results.data)
            {
                item.COUNTRY = CNService.removeLastComma(item.COUNTRY);
                item.PORT = CNService.removeLastComma(item.PORT);
                item.IN_TRANSIT_TO = CNService.removeLastComma(item.IN_TRANSIT_TO);
                item.PLANT = CNService.removeLastComma(item.PLANT);
                item.PROD_INSP_MEMO = CNService.removeLastComma(item.PROD_INSP_MEMO);
                item.PRODUCT_CODE = CNService.removeLastComma(item.PRODUCT_CODE);
                item.ADDITIONAL_BRAND_NAME = CNService.removeLastComma(item.ADDITIONAL_BRAND_NAME);
                item.SALES_ORDER_NO = CNService.removeLastComma(item.SALES_ORDER_NO);
                item.RD_NUMBER = CNService.removeLastComma(item.RD_NUMBER);
                item.RDD = CNService.removeLastComma(item.RDD);
                item.CREATE_ON = CNService.removeLastComma(item.CREATE_ON);

                if (item.IS_STEP_DURATION_EXTEND == "X")
                {
                    item.IS_STEP_DURATION_EXTEND = "Yes";
                }
            }

            foreach (var item in Results.dataExcel)
            {
                item.COUNTRY = CNService.removeLastComma(item.COUNTRY);
                item.PORT = CNService.removeLastComma(item.PORT);
                item.IN_TRANSIT_TO = CNService.removeLastComma(item.IN_TRANSIT_TO);
                item.PLANT = CNService.removeLastComma(item.PLANT);
                item.PROD_INSP_MEMO = CNService.removeLastComma(item.PROD_INSP_MEMO);
                item.PRODUCT_CODE = CNService.removeLastComma(item.PRODUCT_CODE);
                item.ADDITIONAL_BRAND_NAME = CNService.removeLastComma(item.ADDITIONAL_BRAND_NAME);
                item.SALES_ORDER_NO = CNService.removeLastComma(item.SALES_ORDER_NO);
                item.RD_NUMBER = CNService.removeLastComma(item.RD_NUMBER);
                item.RDD = CNService.removeLastComma(item.RDD);
                item.CREATE_ON = CNService.removeLastComma(item.CREATE_ON);

                if (item.IS_STEP_DURATION_EXTEND == "X")
                {
                    item.IS_STEP_DURATION_EXTEND = "Yes";
                }
            }

            return Results.data;
        }

        private static void OrderByEndToEndReport(V_ART_ENDTOEND_REPORT_REQUEST param, List<V_ART_ENDTOEND_REPORT_2> q, ref V_ART_ENDTOEND_REPORT_RESULT Results)
        {
            if (!string.IsNullOrEmpty(param.data.GENERATE_EXCEL))
            {
                Results.dataExcel = q.OrderBy(m => m.WF_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
            }
            else
            {

                var orderColumn = 3;
                var orderDir = "asc";
                Results.ORDER_COLUMN = 3;
                if (param.order != null && param.order.Count > 0)
                {
                    orderColumn = param.order[0].column;
                    orderDir = param.order[0].dir; //desc ,asc
                    Results.ORDER_COLUMN = param.order[0].column;
                }
                string orderASC = "asc";
                string orderDESC = "desc";
                List<string> temp = new List<string>();

                if (orderColumn == 3)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.WF_NO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.WF_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.WF_NO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.WF_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 4)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.PACKAGING_TYPE).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PACKAGING_TYPE).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.PACKAGING_TYPE).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PACKAGING_TYPE).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 5)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.WF_STAUTS).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.WF_STAUTS).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.WF_STAUTS).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.WF_STAUTS).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 6)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.ORDER_BOM_COMPONENT).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.ORDER_BOM_COMPONENT).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.ORDER_BOM_COMPONENT).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.ORDER_BOM_COMPONENT).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 7)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.CURRENT_STEP_NAME_1).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.CURRENT_STEP_NAME_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.CURRENT_STEP_NAME_1).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.CURRENT_STEP_NAME_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 8)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.CURRENT_USER_NAME_1).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.CURRENT_USER_NAME_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {

                        temp = q.OrderByDescending(m => m.CURRENT_USER_NAME_1).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.CURRENT_USER_NAME_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 9)
                {
                    if (orderDir == orderASC)
                    {

                        temp = q.OrderBy(m => m.DUE_DATE_1).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.DUE_DATE_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.DUE_DATE_1).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.DUE_DATE_1).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 10)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.SALES_ORDER_NO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.SALES_ORDER_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {

                        temp = q.OrderByDescending(m => m.SALES_ORDER_NO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.SALES_ORDER_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 11)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.BRAND_NAME).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.BRAND_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.BRAND_NAME).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.BRAND_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 12)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.ADDITIONAL_BRAND_NAME).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.ADDITIONAL_BRAND_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.ADDITIONAL_BRAND_NAME).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.ADDITIONAL_BRAND_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 13)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.PRODUCT_CODE).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PRODUCT_CODE).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.PRODUCT_CODE).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PRODUCT_CODE).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 14)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.PROD_INSP_MEMO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PROD_INSP_MEMO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.PROD_INSP_MEMO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PROD_INSP_MEMO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 15)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.REFERENCE_NO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.REFERENCE_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.REFERENCE_NO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.REFERENCE_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 16)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.PLANT).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PLANT).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.PLANT).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PLANT).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 17)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.SOLD_TO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.SOLD_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.SOLD_TO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.SOLD_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 18)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.SHIP_TO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.SHIP_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.SHIP_TO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.SHIP_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 19)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.COUNTRY).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.COUNTRY).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.COUNTRY).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.COUNTRY).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 20)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.PORT).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PORT).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.PORT).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PORT).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 21)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.IN_TRANSIT_TO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.IN_TRANSIT_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.IN_TRANSIT_TO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.IN_TRANSIT_TO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 22)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.CREATE_ON).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.CREATE_ON).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.CREATE_ON).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.CREATE_ON).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 23)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.RDD).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.RDD).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.RDD).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.RDD).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 24)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.REQUEST_NO).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.REQUEST_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.REQUEST_NO).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.REQUEST_NO).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 25)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.PA_NAME).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PA_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.PA_NAME).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PA_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 26)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.PG_NAME).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PG_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.PG_NAME).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PG_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 27)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.CURRENT_STEP_NAME).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.CURRENT_STEP_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.CURRENT_STEP_NAME).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.CURRENT_STEP_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 28)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.CURRENT_USER_NAME).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.CURRENT_USER_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.CURRENT_USER_NAME).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.CURRENT_USER_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 29)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.STEP_CREATE_DATE).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.STEP_CREATE_DATE).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.STEP_CREATE_DATE).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.STEP_CREATE_DATE).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 30)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.STEP_END_DATE).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.STEP_END_DATE).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.STEP_END_DATE).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.STEP_END_DATE).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 31)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.DURATION_STANDARD).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.DURATION_STANDARD).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.DURATION_STANDARD).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.DURATION_STANDARD).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 32)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.IS_STEP_DURATION_EXTEND).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.IS_STEP_DURATION_EXTEND).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.IS_STEP_DURATION_EXTEND).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.IS_STEP_DURATION_EXTEND).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 33)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.REASON).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.REASON).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.REASON).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.REASON).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 34)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.TOTALDAY).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.TOTALDAY).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.TOTALDAY).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.TOTALDAY).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 35)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.MARKETTING).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.MARKETTING).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.MARKETTING).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.MARKETTING).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 36)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.PROJECT_NAME).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.PROJECT_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.PROJECT_NAME).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.PROJECT_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }
                if (orderColumn == 37)
                {
                    if (orderDir == orderASC)
                    {
                        temp = q.OrderBy(m => m.CREATOR_NAME).Select(m => m.WF_NO).Distinct().OrderBy(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderBy(m => m.CREATOR_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                    else if (orderDir == orderDESC)
                    {
                        temp = q.OrderByDescending(m => m.CREATOR_NAME).Select(m => m.WF_NO).Distinct().OrderByDescending(m => m).Skip(param.start).Take(param.length).ToList();
                        Results.data = q.Where(m => temp.Contains(m.WF_NO)).OrderByDescending(m => m.CREATOR_NAME).ThenBy(m => m.STEP_CREATE_DATE_ORDERBY).ToList();
                    }
                }

                Results.recordsTotal = q.Select(m => m.WF_NO).Distinct().Count();
                Results.recordsFiltered = Results.recordsTotal;
            }
        }

        public static TRACKING_REPORT_RESULT GetEndToEndReport(TRACKING_REPORT_REQUEST param)
        {
            TRACKING_REPORT_RESULT Results = new TRACKING_REPORT_RESULT();

            try
            {
                var listResult = new List<TRACKING_REPORT>();
                var msg = MessageHelper.GetMessage("MSG_005");

                if (String.IsNullOrEmpty(param.data.REQUEST_DATE_FROM) && String.IsNullOrEmpty(param.data.REQUEST_DATE_TO))
                {
                    Results.status = "E";
                    Results.msg = MessageHelper.GetMessage("MSG_022");
                    return Results;
                }

                var searchOnlyArtwork = false;
                if (!string.IsNullOrEmpty(param.data.SEARCH_SO))
                {
                    searchOnlyArtwork = true;
                }

                if (!string.IsNullOrEmpty(param.data.SEARCH_ORDER_BOM))
                {
                    searchOnlyArtwork = true;
                }

                List<V_ART_WF_DASHBOARD> allMockupTrans = new List<V_ART_WF_DASHBOARD>();
                if (!searchOnlyArtwork)
                    allMockupTrans = QueryTrackingReportMockup(param, false);

                var allArtworkTrans = QueryTrackingReportArtwork(param, false);
                Results.recordsTotal = allMockupTrans.Count + allArtworkTrans.Count;

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var allStepMockup = ART_M_STEP_MOCKUP_SERVICE.GetAll(context);


                        var tempMOCKUP_ID = allMockupTrans.Select(m => m.MOCKUP_ID);
                        var listDashboardMockup = (from p in context.V_ART_WF_DASHBOARD
                                                   where tempMOCKUP_ID.Contains(p.MOCKUP_ID)
                                                   select p).ToList();

                        var listPackagingType = (from p in context.SAP_M_CHARACTERISTIC
                                                 where p.NAME == "ZPKG_SEC_GROUP"
                                                 select p).ToList();

                        var mkRoleIDs_MO = (from m in context.ART_M_ROLE
                                            where m.ROLE_CODE.Contains("MK_")
                                               || m.ROLE_CODE.Contains("MC")
                                            select m.ROLE_ID).ToList();

                        var mkUserIDs_MO = (from m in context.ART_M_USER_ROLE
                                            where mkRoleIDs_MO.Contains(m.ROLE_ID)
                                            select m.USER_ID).ToList();

                        var CHECK_LIST_ID_TMP_LIST = allMockupTrans.Select(s => s.CHECK_LIST_ID).ToList();

                        var checklist = (from c in context.ART_WF_MOCKUP_CHECK_LIST
                                         where CHECK_LIST_ID_TMP_LIST.Contains(c.CHECK_LIST_ID)
                                         select c).FirstOrDefault();

                        foreach (var item in allMockupTrans)
                        {
                            TRACKING_REPORT itemResult = new TRACKING_REPORT();
                            itemResult.CHECK_LIST_ID = item.CHECK_LIST_ID;
                            itemResult.MOCKUP_SUB_ID = item.MOCKUP_SUB_ID;

                            itemResult.IS_STEP_DURATION_EXTEND = item.IS_STEP_DURATION_EXTEND;
                            itemResult.DURATION_EXTEND = item.DURATION_EXTEND;
                            itemResult.DURATION = item.DURATION;

                            if (item.MOCKUP_SUB_ID == 0)
                            {
                                if (!String.IsNullOrEmpty(item.CHECK_LIST_NO))
                                {
                                    itemResult.WORKFLOW_NUMBER = item.CHECK_LIST_NO;
                                    itemResult.REQUEST_NUMBER = item.CHECK_LIST_NO;
                                }
                            }
                            else
                            {
                                itemResult.WORKFLOW_NUMBER = item.MOCKUP_NO;
                                itemResult.REQUEST_NUMBER = item.CHECK_LIST_NO;
                            }

                            itemResult.CREATOR_DISPLAY_TXT = item.CREATE_BY_PROCESS_FIRST_NAME + " " + item.CREATE_BY_PROCESS_LAST_NAME;

                            if (checklist != null)
                            {
                                if (mkUserIDs_MO.Contains(checklist.CREATE_BY))
                                {
                                    itemResult.MARKETING_NAME = CNService.GetUserName(checklist.CREATE_BY, context);
                                }
                            }

                            var PACKING_TYPE = listPackagingType.Where(w => w.CHARACTERISTIC_ID == item.PACKING_TYPE_ID).FirstOrDefault();
                            if (PACKING_TYPE != null) itemResult.PACKING_TYPE_DISPLAY_TXT = PACKING_TYPE.VALUE + ":" + PACKING_TYPE.DESCRIPTION;
                            itemResult.PIC_DISPLAY_TXT = item.CREATE_BY_CHECK_LIST_TITLE + " " + item.CREATE_BY_CHECK_LIST_FIRST_NAME + " " + item.CREATE_BY_CHECK_LIST_LAST_NAME;
                            if (item.IS_END == "X")
                            {
                                if (item.IS_TERMINATE == "X")
                                {
                                    itemResult.CURRENT_STEP_DISPLAY_TXT = "Terminated";
                                }
                                else
                                {
                                    itemResult.CURRENT_STEP_DISPLAY_TXT = "Completed";
                                }
                            }
                            else
                            {
                                itemResult.CURRENT_STEP_DISPLAY_TXT = "In progress";
                            }

                            itemResult.BRAND_DISPLAY_TXT = item.BRAND_DISPLAY_TXT;
                            itemResult.SOLD_TO_DISPLAY_TXT = item.SOLD_TO_DISPLAY_TXT;
                            itemResult.SHIP_TO_DISPLAY_TXT = item.SHIP_TO_DISPLAY_TXT;

                            if (item.REQUEST_DELIVERY_DATE != null)
                            {
                                itemResult.RDD_DISPLAY_TXT = Convert.ToDateTime(item.REQUEST_DELIVERY_DATE).ToString("dd.MM.yyyy");
                            }

                            itemResult.VENDOR_RFQ = CNService.GetVendorRFQ(item.MOCKUP_ID, item.MOCKUP_SUB_ID, context);
                            itemResult.SELECTED_VENDOR = CNService.GetVendorSelected(item.MOCKUP_ID, item.MOCKUP_SUB_ID, context);

                            string table = "";
                            if (item.MOCKUP_ID > 0)
                            {
                                var temp_dashboard = listDashboardMockup.Where(m => m.MOCKUP_ID == item.MOCKUP_ID).ToList();
                                var list_waiting = temp_dashboard.Where(m => string.IsNullOrEmpty(m.IS_END)).ToList();

                                if (!string.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE))
                                {
                                    if (param.data.CURRENT_STEP_WF_TYPE.StartsWith("M"))
                                    {
                                        var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP_WF_TYPE.Replace("M", ""));
                                        list_waiting = list_waiting.Where(m => m.CURRENT_STEP_ID == current_step_id).ToList();
                                    }
                                }
                                if (param.data.CURRENT_ASSIGN_ID > 0)
                                {
                                    list_waiting = list_waiting.Where(m => m.CURRENT_USER_ID == param.data.CURRENT_ASSIGN_ID).ToList();
                                }

                                if (param.data.WORKING_GROUP_ID > 0)
                                {
                                    list_waiting = list_waiting.Where(m => m.CURRENT_USER_ID == param.data.CURRENT_ASSIGN_ID).ToList();
                                }

                                if (list_waiting.Count > 0)
                                    table += "<table class='cls_table_in_table' style='min-width:750px;'>";

                                foreach (var item_waiting in list_waiting)
                                {
                                    table += "<tr>";
                                    var tempMockup = allStepMockup.Where(m => m.STEP_MOCKUP_ID == item_waiting.CURRENT_STEP_ID).FirstOrDefault();
                                    if (tempMockup != null)
                                    {
                                        table += "<td style='max-width:330px;width:330px;min-width:330px;white-space:initial;'>";
                                        table += "<a target='_blank' href='" + System.Configuration.ConfigurationManager.AppSettings["suburl"] + "/TaskForm/" + item_waiting.MOCKUP_SUB_ID + "'>" + tempMockup.STEP_MOCKUP_NAME + "</a>";

                                        if (CNService.GetUserName(item_waiting.CURRENT_USER_ID, context) != "")
                                            table += "<td style='max-width:250px;width:250px;min-width:250px;white-space:initial;'>" + CNService.GetUserName(item_waiting.CURRENT_USER_ID, context) + "</td>";
                                        else
                                            table += "<td style='max-width:250px;width:250px;min-width:250px;white-space:initial;'>" + msg + " </td>";
                                    }

                                    if (item_waiting.CREATE_DATE_PROCESS != null)
                                    {
                                        DateTime? dtReceiveWf = item_waiting.CREATE_DATE_PROCESS;
                                        DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION.Value));

                                        if (!string.IsNullOrEmpty(item.IS_STEP_DURATION_EXTEND))// item.IS_STEP_DURATION_EXTEND.Equals("X"))
                                        {
                                            dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION_EXTEND.Value));
                                        }

                                        table += "<td style='text-align:right;max-width:170px;width:170px;min-width:170px;white-space:initial;'>" + CNService.GetWorkingDays(DateTime.Now, dtWillFinish.Value) + " [" + dtWillFinish.Value.ToString("dd/MM/yyyy HH:mm") + "]" + "</td>";
                                    }

                                    table += "</tr>";
                                }

                                if (list_waiting.Count > 0)
                                    itemResult.CURRENT_STATUS_DISPLAY_TXT = table;


                                var total_day = 0;
                                List<DateTime> start_date2 = new List<DateTime>();
                                List<DateTime> end_date2 = new List<DateTime>();

                                if (param.data.GENERATE_EXCEL == "X")
                                {
                                    string step = "";
                                    string step_assign = "";
                                    string start_date = "";
                                    string end_date = "";
                                    string step_duration = "";
                                    string reason = "";

                                    GetAllStepColumnMockup_Excel(temp_dashboard, allStepMockup, msg, context, ref total_day
                                   , ref step
                                   , ref step_assign
                                   , ref start_date
                                   , ref end_date
                                   , ref step_duration
                                   , ref reason);

                                    itemResult.ALL_STEP_DISPLAY_TXT = step;
                                    itemResult.ALL_STEP_ASSIGN_DISPLAY_TXT = step_assign;
                                    itemResult.ALL_STEP_START_DATE_DISPLAY_TXT = start_date;
                                    itemResult.ALL_STEP_END_DATE_DISPLAY_TXT = end_date;
                                    itemResult.ALL_STEP_DURATION_DISPLAY_TXT = step_duration;
                                    itemResult.ALL_STEP_REASON_DISPLAY_TXT = reason;
                                }
                                else
                                {
                                    itemResult.ALL_STEP_DISPLAY_TXT = GetAllStepColumnMockup(temp_dashboard, allStepMockup, msg, context, ref total_day, ref start_date2, ref end_date2);
                                }

                                if (total_day == 0)
                                    itemResult.TOTAL_DAY_DISPLAY_TXT = "";
                                else
                                {
                                    if (start_date2.Count > 0 && end_date2.Count > 0)
                                    {
                                        DateTime minDate = start_date2.OrderBy(a => a).First();
                                        DateTime maxDate = end_date2.OrderByDescending(a => a).First();
                                        itemResult.TOTAL_DAY_DISPLAY_TXT = (CNService.GetWorkingDays(minDate, maxDate) + 1).ToString();
                                    }
                                    else
                                    {
                                        itemResult.TOTAL_DAY_DISPLAY_TXT = "";
                                    }
                                }
                            }

                            var tempProduct = MapperServices.ART_WF_MOCKUP_CHECK_LIST_PRODUCT(ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_PRODUCT() { CHECK_LIST_ID = item.CHECK_LIST_ID }, context));
                            itemResult.PRODUCT_CODE_DISPLAY_TXT = tempProduct.Aggregate("", (a, b) => a + ((a.Length > 0 && b.PRODUCT_CODE != null && b.PRODUCT_CODE.Length > 0) ? "<br/>" : "") + b.PRODUCT_CODE);

                            var tempRD = MapperServices.ART_WF_MOCKUP_CHECK_LIST_REFERENCE(ART_WF_MOCKUP_CHECK_LIST_REFERENCE_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_REFERENCE() { CHECK_LIST_ID = item.CHECK_LIST_ID }, context));
                            itemResult.RD_NUMBER_DISPLAY_TXT = tempRD.Aggregate("", (a, b) => a + ((a.Length > 0 && b.REFERENCE_NO != null && b.REFERENCE_NO.Length > 0) ? "<br />" : "") + b.REFERENCE_NO);

                            if (param.data.GENERATE_EXCEL == "X")
                            {
                                itemResult.PRODUCT_CODE_DISPLAY_TXT = itemResult.PRODUCT_CODE_DISPLAY_TXT.Replace("<br />", ", ");

                                itemResult.RD_NUMBER_DISPLAY_TXT = itemResult.RD_NUMBER_DISPLAY_TXT.Replace("<br />", ", ");

                            }
                            if (item.PRIMARY_SIZE_ID > 0)
                            {
                                var PRIMARY_SIZE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PRIMARY_SIZE_ID, context);
                                if (PRIMARY_SIZE != null)
                                    itemResult.PRIMARY_SIZE_DISPLAY_TXT = PRIMARY_SIZE.DESCRIPTION;
                            }
                            else
                            {
                                var temp = SAP_M_3P_SERVICE.GetByTHREE_P_ID(item.THREE_P_ID, context);
                                if (temp != null)
                                    itemResult.PRIMARY_SIZE_DISPLAY_TXT = temp.PRIMARY_SIZE_DESCRIPTION;
                            }

                            if (string.IsNullOrEmpty(itemResult.PRIMARY_SIZE_DISPLAY_TXT))
                            {
                                var temp = ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_PRODUCT() { CHECK_LIST_ID = item.CHECK_LIST_ID }, context).FirstOrDefault();
                                if (temp != null) itemResult.PRIMARY_SIZE_DISPLAY_TXT = temp.PRIMARY_SIZE;
                            }

                            listResult.Add(itemResult);
                        }
                    }
                }

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var allStepArtwork = ART_M_STEP_ARTWORK_SERVICE.GetAll(context);

                        var tempARTWORK_ITEM_ID = allArtworkTrans.Select(m => m.ARTWORK_ITEM_ID);
                        var tempARTWORK_SUB_ID = allArtworkTrans.Select(m => m.ARTWORK_SUB_ID);
                        var listDashboardArtwork = (from p in context.V_ART_WF_DASHBOARD_ARTWORK
                                                    where tempARTWORK_ITEM_ID.Contains(p.ARTWORK_ITEM_ID)
                                                    select p).ToList();

                        var listAssignedSOHeader = (from p in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
                                                    where tempARTWORK_SUB_ID.Contains(p.ARTWORK_SUB_ID)
                                                    select p).ToList();

                        var listAssignedSOItem = (from p in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM
                                                  where tempARTWORK_SUB_ID.Contains(p.ARTWORK_SUB_ID)
                                                  select p).ToList();

                        foreach (var item in allArtworkTrans)
                        {
                            TRACKING_REPORT itemResult = new TRACKING_REPORT();

                            itemResult.CHECK_LIST_ID = item.ARTWORK_REQUEST_ID;
                            itemResult.MOCKUP_SUB_ID = item.ARTWORK_SUB_ID;

                            itemResult.IS_STEP_DURATION_EXTEND = item.IS_STEP_DURATION_EXTEND;
                            itemResult.DURATION_EXTEND = item.DURATION_EXTEND;
                            itemResult.DURATION = item.DURATION;

                            if (item.ARTWORK_SUB_ID == 0)
                            {
                                if (!String.IsNullOrEmpty(item.ARTWORK_REQUEST_NO))
                                {
                                    itemResult.WORKFLOW_NUMBER = item.ARTWORK_REQUEST_NO;
                                    itemResult.REQUEST_NUMBER = item.ARTWORK_REQUEST_NO;
                                    itemResult.CREATOR_DISPLAY_TXT = item.CREATE_BY_ARTWORK_REQUEST_FIRST_NAME + " " + item.CREATE_BY_PROCESS_LAST_NAME;

                                    itemResult.PROJECT_NAME = item.PROJECT_NAME;
                                }
                            }
                            else
                            {
                                itemResult.REQUEST_NUMBER = item.ARTWORK_REQUEST_NO;
                                itemResult.WORKFLOW_NUMBER = item.REQUEST_ITEM_NO;
                                itemResult.CREATOR_DISPLAY_TXT = item.CREATE_BY_ARTWORK_REQUEST_FIRST_NAME + " " + item.CREATE_BY_PROCESS_LAST_NAME;

                                itemResult.PROJECT_NAME = item.PROJECT_NAME;
                            }

                            var plants = (from p in context.ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT
                                          where p.ARTWORK_REQUEST_ID == item.ARTWORK_REQUEST_ID
                                          select p.PRODUCTION_PLANT_ID).ToList();

                            var countries = (from p in context.ART_WF_ARTWORK_REQUEST_COUNTRY
                                             where p.ARTWORK_REQUEST_ID == item.ARTWORK_REQUEST_ID
                                             select p.COUNTRY_ID).ToList();

                            if (plants != null && plants.Count > 0)
                            {
                                var mPlants = (from m in context.SAP_M_PLANT
                                               where plants.Contains(m.PLANT_ID)
                                               select m.NAME).ToArray();
                                if (mPlants != null)
                                {
                                    itemResult.PLANT = CNService.ConcatArray(mPlants);
                                }
                            }

                            if (countries != null && countries.Count > 0)
                            {
                                var mCounties = (from m in context.SAP_M_COUNTRY
                                                 where countries.Contains(m.COUNTRY_ID)
                                                 select m.NAME).ToArray();
                                if (mCounties != null)
                                {
                                    itemResult.COUNTRY = CNService.ConcatArray(mCounties);
                                }
                            }

                            var processPA = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PA() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                            if (processPA != null)
                            {
                                var PACKING_TYPE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(processPA.MATERIAL_GROUP_ID, context);
                                if (PACKING_TYPE != null) itemResult.PACKING_TYPE_DISPLAY_TXT = PACKING_TYPE.VALUE + ":" + PACKING_TYPE.DESCRIPTION;

                                itemResult.PA_OWNER = CNService.GetUserName(processPA.PA_USER_ID, context);
                                itemResult.PG_OWNER = CNService.GetUserName(processPA.PG_USER_ID, context);

                                var allProcessSub = CNService.FindArtworkSubId(processPA.ARTWORK_SUB_ID, context);
                                if (allProcessSub != null)
                                {
                                    var mkRoleIDs = (from m in context.ART_M_ROLE
                                                     where m.ROLE_CODE.Contains("MK_")
                                                        || m.ROLE_CODE.Contains("MC")
                                                     select m.ROLE_ID).ToList();

                                    var mkUserIDs = (from m in context.ART_M_USER_ROLE
                                                     where mkRoleIDs.Contains(m.ROLE_ID)
                                                     select m.USER_ID).ToList();

                                    var processes = (from k in context.ART_WF_ARTWORK_PROCESS
                                                     where allProcessSub.Contains(k.ARTWORK_SUB_ID)
                                                     select k).OrderByDescending(o => o.UPDATE_DATE).ToList();
                                    if (processes != null)
                                    {
                                        var processMK = processes.Where(p => mkUserIDs.Contains(p.UPDATE_BY)).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                                        if (processMK != null)
                                        {
                                            itemResult.MARKETING_NAME = CNService.GetUserName(processMK.UPDATE_BY, context);
                                        }
                                        else
                                        {
                                            var reqItemIDs = processes.Select(s => s.ARTWORK_ITEM_ID).Distinct().ToList();
                                            var reqRequestIDs = processes.Select(s => s.ARTWORK_REQUEST_ID).Distinct().ToList();

                                            var reqItem = (from r in context.ART_WF_ARTWORK_REQUEST_ITEM
                                                           where reqItemIDs.Contains(r.ARTWORK_ITEM_ID)
                                                            && mkUserIDs.Contains(r.UPDATE_BY)
                                                           select r).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                                            if (reqItem != null)
                                            {
                                                itemResult.MARKETING_NAME = CNService.GetUserName(reqItem.UPDATE_BY, context);
                                            }
                                            else
                                            {
                                                var reqRequest = (from r in context.ART_WF_ARTWORK_REQUEST
                                                                  where reqRequestIDs.Contains(r.ARTWORK_REQUEST_ID)
                                                                  select r).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                                                if (reqRequest != null)
                                                {
                                                    itemResult.MARKETING_NAME = CNService.GetUserName(reqRequest.CREATOR_ID, context);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            itemResult.PIC_DISPLAY_TXT = item.CREATE_BY_ARTWORK_REQUEST_TITLE + " " + item.CREATE_BY_ARTWORK_REQUEST_FIRST_NAME + " " + item.CREATE_BY_ARTWORK_REQUEST_LAST_NAME;
                            if (item.IS_END == "X")
                            {
                                if (item.IS_TERMINATE == "X")
                                {
                                    itemResult.CURRENT_STEP_DISPLAY_TXT = "Terminated";
                                }
                                else
                                {
                                    itemResult.CURRENT_STEP_DISPLAY_TXT = "Completed";
                                }
                            }
                            else
                            {
                                itemResult.CURRENT_STEP_DISPLAY_TXT = "In progress";
                            }

                            itemResult.BRAND_DISPLAY_TXT = item.BRAND_DISPLAY_TXT;
                            itemResult.SOLD_TO_DISPLAY_TXT = item.SOLD_TO_DISPLAY_TXT;
                            itemResult.SHIP_TO_DISPLAY_TXT = item.SHIP_TO_DISPLAY_TXT;
                            itemResult.RDD = item.REQUEST_DELIVERY_DATE;

                            string table = "";
                            if (item.ARTWORK_ITEM_ID > 0)
                            {
                                var temp_dashboard = listDashboardArtwork.Where(m => m.ARTWORK_ITEM_ID == item.ARTWORK_ITEM_ID).ToList();
                                var list_waiting = temp_dashboard.Where(m => string.IsNullOrEmpty(m.IS_END)).ToList();

                                if (!string.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE))
                                {
                                    if (param.data.CURRENT_STEP_WF_TYPE.StartsWith("A"))
                                    {
                                        var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP_WF_TYPE.Replace("A", ""));
                                        list_waiting = list_waiting.Where(m => m.CURRENT_STEP_ID == current_step_id).ToList();
                                    }
                                }
                                if (param.data.CURRENT_ASSIGN_ID > 0)
                                {
                                    list_waiting = list_waiting.Where(m => m.CURRENT_USER_ID == param.data.CURRENT_ASSIGN_ID).ToList();
                                }

                                if (param.data.WORKING_GROUP_ID > 0)
                                {
                                    list_waiting = list_waiting.Where(m => m.CURRENT_USER_ID == param.data.CURRENT_ASSIGN_ID).ToList();
                                }

                                if (param.data.GENERATE_EXCEL == "X")
                                {
                                    StringBuilder sbCurrentStep = new StringBuilder();
                                    StringBuilder sbCurrentAssign = new StringBuilder();
                                    StringBuilder sbCurrentDueDate = new StringBuilder();

                                    foreach (var item_waiting in list_waiting)
                                    {

                                        var tempMockup = allStepArtwork.Where(m => m.STEP_ARTWORK_ID == item_waiting.CURRENT_STEP_ID).FirstOrDefault();
                                        if (tempMockup != null)
                                        {
                                            sbCurrentStep.AppendLine(tempMockup.STEP_ARTWORK_NAME + System.Environment.NewLine);

                                            if (CNService.GetUserName(item_waiting.CURRENT_USER_ID, context) != "")
                                            {
                                                sbCurrentAssign.AppendLine(CNService.GetUserName(item_waiting.CURRENT_USER_ID, context));
                                            }
                                            else
                                            {
                                                sbCurrentAssign.AppendLine(msg);
                                            }
                                        }

                                        if (item_waiting.CREATE_DATE_PROCESS != null)
                                        {
                                            DateTime? dtReceiveWf = item_waiting.CREATE_DATE_PROCESS;
                                            DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION.Value));

                                            if (!string.IsNullOrEmpty(item.IS_STEP_DURATION_EXTEND))
                                            {
                                                dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION_EXTEND.Value));
                                            }

                                            sbCurrentDueDate.AppendLine(CNService.GetWorkingDays(DateTime.Now, dtWillFinish.Value) + " (" + dtWillFinish.Value.ToString("dd/MM/yyyy HH:mm") + ")");
                                        }
                                    }

                                    if (list_waiting.Count > 0)
                                    {
                                        itemResult.CURRENT_STATUS_DISPLAY_TXT = sbCurrentStep.ToString();
                                        itemResult.CURRENT_ASSIGN_DISPLAY_TXT = sbCurrentAssign.ToString();
                                        itemResult.DUEDATE_DISPLAY_TXT = sbCurrentDueDate.ToString();
                                    }
                                }
                                else
                                {
                                    if (list_waiting.Count > 0)
                                        table += "<table class='cls_table_in_table' style='min-width:750px;'>";

                                    foreach (var item_waiting in list_waiting)
                                    {
                                        table += "<tr>";
                                        var stepAwWaiting = allStepArtwork.Where(m => m.STEP_ARTWORK_ID == item_waiting.CURRENT_STEP_ID).FirstOrDefault();
                                        if (stepAwWaiting != null)
                                        {
                                            table += "<td style='max-width:330px;width:330px;min-width:330px;white-space:initial;'>";
                                            table += "<a target='_blank' href='" + System.Configuration.ConfigurationManager.AppSettings["suburl"] + "/TaskFormArtwork/" + item_waiting.ARTWORK_SUB_ID + "'>" + stepAwWaiting.STEP_ARTWORK_NAME + "</a>";

                                            if (CNService.GetUserName(item_waiting.CURRENT_USER_ID, context) != "")
                                                table += "<td style='max-width:250px;width:250px;min-width:250px;white-space:initial;'>" + CNService.GetUserName(item_waiting.CURRENT_USER_ID, context) + "</td>";
                                            else
                                                table += "<td style='max-width:250px;width:250px;min-width:250px;white-space:initial;'>" + msg + " </td>";
                                        }

                                        if (item_waiting.CREATE_DATE_PROCESS != null)
                                        {
                                            DateTime? dtReceiveWf = item_waiting.CREATE_DATE_PROCESS;
                                            DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(stepAwWaiting.DURATION.Value));

                                            if (!string.IsNullOrEmpty(item.IS_STEP_DURATION_EXTEND))
                                            {
                                                dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION_EXTEND.Value));
                                            }

                                            table += "<td style='text-align:right;max-width:170px;width:170px;min-width:170px;white-space:initial;'>" + CNService.GetWorkingDays(DateTime.Now, dtWillFinish.Value) + " [" + dtWillFinish.Value.ToString("dd/MM/yyyy HH:mm") + "]" + "</td>";
                                        }

                                        table += "</tr>";
                                    }

                                    if (list_waiting.Count > 0)
                                        itemResult.CURRENT_STATUS_DISPLAY_TXT = table;
                                }

                                var total_day = 0;
                                List<DateTime> start_date2 = new List<DateTime>();
                                List<DateTime> end_date2 = new List<DateTime>();

                                if (param.data.GENERATE_EXCEL == "X")
                                {
                                    string step = "";
                                    string step_assign = "";
                                    string start_date = "";
                                    string end_date = "";
                                    string step_duration = "";
                                    string reason = "";

                                    GetAllStepColumnArtwork_Excel(temp_dashboard, allStepArtwork, msg, context, ref total_day
                                   , ref step
                                   , ref step_assign
                                   , ref start_date
                                   , ref end_date
                                   , ref step_duration
                                   , ref reason);

                                    itemResult.ALL_STEP_DISPLAY_TXT = step;
                                    itemResult.ALL_STEP_ASSIGN_DISPLAY_TXT = step_assign;
                                    itemResult.ALL_STEP_START_DATE_DISPLAY_TXT = start_date;
                                    itemResult.ALL_STEP_END_DATE_DISPLAY_TXT = end_date;
                                    itemResult.ALL_STEP_DURATION_DISPLAY_TXT = step_duration;
                                    itemResult.ALL_STEP_REASON_DISPLAY_TXT = reason;

                                }
                                else
                                {
                                    itemResult.ALL_STEP_DISPLAY_TXT = GetAllStepColumnArtwork(temp_dashboard, allStepArtwork, msg, context, ref total_day, ref start_date2, ref end_date2);

                                }

                                if (total_day == 0)
                                    itemResult.TOTAL_DAY_DISPLAY_TXT = "";
                                else
                                {
                                    if (start_date2.Count > 0 && end_date2.Count > 0)
                                    {
                                        DateTime minDate = start_date2.OrderBy(a => a).First();
                                        DateTime maxDate = end_date2.OrderByDescending(a => a).First();
                                        itemResult.TOTAL_DAY_DISPLAY_TXT = (CNService.GetWorkingDays(minDate, maxDate) + 1).ToString();
                                    }
                                }
                            }

                            var tempProduct = MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCT(ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_PRODUCT() { ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID }, context));
                            foreach (var itemProduct in tempProduct)
                            {
                                var temp = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(itemProduct.PRODUCT_CODE_ID, context);
                                if (temp != null)
                                    if (string.IsNullOrEmpty(itemResult.PRODUCT_CODE_DISPLAY_TXT))
                                        itemResult.PRODUCT_CODE_DISPLAY_TXT = temp.PRODUCT_CODE + " : " + temp.PRODUCT_DESCRIPTION;
                                    else
                                        itemResult.PRODUCT_CODE_DISPLAY_TXT += "<br/>" + temp.PRODUCT_CODE + " : " + temp.PRODUCT_DESCRIPTION; ;
                            }

                            var tempRD = MapperServices.ART_WF_ARTWORK_REQUEST_REFERENCE(ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_REFERENCE() { ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID }, context));
                            itemResult.RD_NUMBER_DISPLAY_TXT = tempRD.Aggregate("", (a, b) => a + ((a.Length > 0 && b.REFERENCE_NO != null && b.REFERENCE_NO.Length > 0) ? "<br/>" : "") + b.REFERENCE_NO);

                            if (item.PRIMARY_SIZE_ID > 0)
                            {
                                var PRIMARY_SIZE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PRIMARY_SIZE_ID, context);
                                if (PRIMARY_SIZE != null)
                                    itemResult.PRIMARY_SIZE_DISPLAY_TXT = PRIMARY_SIZE.DESCRIPTION;
                            }
                            else
                            {
                                var temp = SAP_M_3P_SERVICE.GetByTHREE_P_ID(item.THREE_P_ID, context);
                                if (temp != null)
                                    itemResult.PRIMARY_SIZE_DISPLAY_TXT = temp.PRIMARY_SIZE_DESCRIPTION;
                            }

                            var tempListAssignedSOHeader = listAssignedSOHeader.Where(m => m.ARTWORK_SUB_ID == item.ARTWORK_SUB_ID).ToList();
                            var tempListAssignedSOItem = listAssignedSOItem.Where(m => m.ARTWORK_SUB_ID == item.ARTWORK_SUB_ID).ToList();

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

                            itemResult.IN_TRANSIT_TO_DISPLAY_TXT = CNService.ConcatArray(tempListAssignedSOItem.Select(m => m.IN_TRANSIT_TO).ToArray());

                            if (param.data.GENERATE_EXCEL != "X")
                            {
                                if (listAssignSO != null && listAssignSO.Count > 0)
                                {
                                    itemResult.SALES_ORDER_NO = CNService.ConcatArrayEnterLine(listAssignSO.ToArray());
                                }
                                else
                                {
                                    itemResult.SALES_ORDER_NO = CNService.ConcatArrayEnterLine(tempListAssignedSOHeader.Select(m => m.SALES_ORDER_NO).ToArray());
                                }
                            }
                            else
                            {
                                if (listAssignSO != null && listAssignSO.Count > 0)
                                {
                                    itemResult.SALES_ORDER_NO = CNService.ConcatArray(listAssignSO.ToArray());
                                }
                                else
                                {
                                    itemResult.SALES_ORDER_NO = CNService.ConcatArray(tempListAssignedSOHeader.Select(m => m.SALES_ORDER_NO).ToArray());
                                }
                            }

                            itemResult.SALES_ORDER_CREATE_DATE = CNService.ConcatArray(tempListAssignedSOHeader.Select(m => m.CREATE_ON).Distinct().ToArray());
                            itemResult.SALES_ORDER_ITEM = CNService.ConcatArray(tempListAssignedSOItem.Select(m => m.ITEM).ToArray());
                            itemResult.ADDITIONAL_BRAND_DISPLAY_TXT = CNService.ConcatArray(tempListAssignedSOItem.Select(m => m.ADDITIONAL_BRAND_DESCRIPTION).Distinct().ToArray());
                            itemResult.PRODUCTION_MEMO_DISPLAY_TXT = CNService.ConcatArray(tempListAssignedSOItem.Select(m => m.PROD_INSP_MEMO).Distinct().ToArray());
                            itemResult.PORT = CNService.ConcatArray(tempListAssignedSOItem.Select(m => m.PORT).Distinct().ToArray());
                            itemResult.RDD_DISPLAY_TXT = CNService.ConcatArray(tempListAssignedSOHeader.Select(m => m.RDD).Distinct().ToArray());

                            var itemID = CNService.FindArtworkItemId(item.ARTWORK_SUB_ID, context);
                            var stepPA = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_PA").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();

                            var process = (from p in context.ART_WF_ARTWORK_PROCESS
                                           where p.ARTWORK_ITEM_ID == itemID
                                            && p.CURRENT_STEP_ID == stepPA
                                           select p).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                            if (process != null)
                            {
                                var processPA_Tmp = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                     where p.ARTWORK_SUB_ID == process.ARTWORK_SUB_ID
                                                     select p).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                                if (processPA_Tmp != null)
                                {
                                    if (!String.IsNullOrEmpty(processPA_Tmp.MATERIAL_NO))
                                    {
                                        itemResult.SALES_ORDER_ITEM_COMPONENT = processPA_Tmp.MATERIAL_NO;
                                    }
                                }
                            }

                            listResult.Add(itemResult);
                        }
                    }
                }

                string orderDir = "";
                if (param.columns != null)
                {
                    listResult = FilterDataPGView(listResult, param);
                }

                if (param.order != null && param.order.Count > 0)
                {
                    var orderColumn = param.order[0].column;
                    orderDir = param.order[0].dir; //desc ,asc
                    listResult = OrderByDataPGView(listResult, orderColumn, orderDir);
                }

                Results.recordsFiltered = listResult.Count;
                if (param.data.GENERATE_EXCEL != "X")
                {
                    listResult = listResult.Skip(param.start).Take(param.length).ToList();
                }

                Results.status = "S";
                Results.data = listResult;
                Results.draw = param.draw;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        private static List<TRACKING_REPORT> FilterDataPGView(List<TRACKING_REPORT> data, TRACKING_REPORT_REQUEST param)
        {
            var filterValue = param.columns[3].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.WORKFLOW_NUMBER) && m.WORKFLOW_NUMBER.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[4].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PACKING_TYPE_DISPLAY_TXT) && m.PACKING_TYPE_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[5].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.CURRENT_STEP_DISPLAY_TXT) && m.CURRENT_STEP_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();


            filterValue = param.columns[6].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SALES_ORDER_ITEM_COMPONENT) && m.SALES_ORDER_ITEM_COMPONENT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[7].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.CURRENT_STATUS_DISPLAY_TXT) && m.CURRENT_STATUS_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[8].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SALES_ORDER_NO) && m.SALES_ORDER_NO.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[9].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.BRAND_DISPLAY_TXT) && m.BRAND_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[10].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.ADDITIONAL_BRAND_DISPLAY_TXT) && m.ADDITIONAL_BRAND_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[11].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PRODUCT_CODE_DISPLAY_TXT) && m.PRODUCT_CODE_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[12].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PRODUCTION_MEMO_DISPLAY_TXT) && m.PRODUCTION_MEMO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[13].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.RD_NUMBER_DISPLAY_TXT) && m.RD_NUMBER_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[14].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PLANT) && m.PLANT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[15].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SOLD_TO_DISPLAY_TXT) && m.SOLD_TO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[16].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SHIP_TO_DISPLAY_TXT) && m.SHIP_TO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[17].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.COUNTRY) && m.COUNTRY.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[18].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PORT) && m.PORT.ToLower().Contains(filterValue.ToLower())).ToList();

            //filterValue = param.columns[17].search.value;
            //if (!string.IsNullOrEmpty(filterValue))
            //    data = data.Where(m => m.RDD != null && m.RDD.Value.ToString("dd/MM/yyyy").ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[19].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.IN_TRANSIT_TO_DISPLAY_TXT) && m.IN_TRANSIT_TO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[20].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.SALES_ORDER_CREATE_DATE) && m.SALES_ORDER_CREATE_DATE.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[21].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.RDD_DISPLAY_TXT) && m.RDD_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[22].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.REQUEST_NUMBER) && m.REQUEST_NUMBER.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[23].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PA_OWNER) && m.PA_OWNER.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[24].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PG_OWNER) && m.PG_OWNER.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[25].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.ALL_STEP_DISPLAY_TXT) && m.ALL_STEP_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[26].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.TOTAL_DAY_DISPLAY_TXT) && m.TOTAL_DAY_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[27].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.MARKETING_NAME) && m.MARKETING_NAME.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[28].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.PROJECT_NAME) && m.PROJECT_NAME.ToLower().Contains(filterValue.ToLower())).ToList();

            filterValue = param.columns[29].search.value;
            if (!string.IsNullOrEmpty(filterValue))
                data = data.Where(m => !string.IsNullOrEmpty(m.CREATOR_DISPLAY_TXT) && m.CREATOR_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

            return data;
        }

        private static List<TRACKING_REPORT> OrderByDataPGView(List<TRACKING_REPORT> data, int orderColumn, string orderDir)
        {
            string orderASC = "asc";
            string orderDESC = "desc";

            if (orderColumn == 3)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.WORKFLOW_NUMBER).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.WORKFLOW_NUMBER).ToList();
            }

            if (orderColumn == 4)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PACKING_TYPE_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PACKING_TYPE_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 5)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.CURRENT_STEP_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.CURRENT_STEP_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 6)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.SALES_ORDER_ITEM_COMPONENT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.SALES_ORDER_ITEM_COMPONENT).ToList();
            }

            if (orderColumn == 7)
            {

            }

            if (orderColumn == 8)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.SALES_ORDER_NO).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.SALES_ORDER_NO).ToList();
            }

            if (orderColumn == 9)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.BRAND_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.BRAND_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 10)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.ADDITIONAL_BRAND_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.ADDITIONAL_BRAND_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 11)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PRODUCT_CODE_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PRODUCT_CODE_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 12)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PRODUCTION_MEMO_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PRODUCTION_MEMO_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 13)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.RD_NUMBER_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.RD_NUMBER_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 14)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PLANT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PLANT).ToList();
            }

            if (orderColumn == 15)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.SOLD_TO_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.SOLD_TO_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 16)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.SHIP_TO_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.SHIP_TO_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 17)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.COUNTRY).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.COUNTRY).ToList();
            }

            if (orderColumn == 18)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PORT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PORT).ToList();
            }

            if (orderColumn == 19)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.IN_TRANSIT_TO_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.IN_TRANSIT_TO_DISPLAY_TXT).ToList();
            }

            if (orderColumn == 20)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.SALES_ORDER_CREATE_DATE).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.SALES_ORDER_CREATE_DATE).ToList();
            }

            if (orderColumn == 21)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.RDD_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.RDD_DISPLAY_TXT).ToList();
            }


            if (orderColumn == 22)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.REQUEST_NUMBER).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.REQUEST_NUMBER).ToList();
            }
            if (orderColumn == 23)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PA_OWNER).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PA_OWNER).ToList();
            }
            if (orderColumn == 24)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PG_OWNER).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PG_OWNER).ToList();
            }
            if (orderColumn == 25)
            {

            }
            if (orderColumn == 26)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.TOTAL_DAY_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.TOTAL_DAY_DISPLAY_TXT).ToList();
            }
            if (orderColumn == 27)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.MARKETING_NAME).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.MARKETING_NAME).ToList();
            }
            if (orderColumn == 28)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.PROJECT_NAME).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.PROJECT_NAME).ToList();
            }
            if (orderColumn == 29)
            {
                if (orderDir == orderASC)
                    data = data.OrderBy(i => i.CREATOR_DISPLAY_TXT).ToList();
                else if (orderDir == orderDESC)
                    data = data.OrderByDescending(i => i.CREATOR_DISPLAY_TXT).ToList();
            }
            return data;
        }

        //private static List<TRACKING_REPORT> FilterDataMKView(List<TRACKING_REPORT> data, TRACKING_REPORT_REQUEST param)
        //{
        //    var filterValue = param.columns[1].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.PIC_DISPLAY_TXT) && m.PIC_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[2].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.WORKFLOW_NUMBER) && m.WORKFLOW_NUMBER.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[3].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.PACKING_TYPE_DISPLAY_TXT) && m.PACKING_TYPE_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[4].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.PRIMARY_SIZE_DISPLAY_TXT) && m.PRIMARY_SIZE_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[5].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.CURRENT_STEP_DISPLAY_TXT) && m.CURRENT_STEP_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[6].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.CURRENT_STATUS_DISPLAY_TXT) && m.CURRENT_STATUS_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[7].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.SOLD_TO_DISPLAY_TXT) && m.SOLD_TO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[8].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.SHIP_TO_DISPLAY_TXT) && m.SHIP_TO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[9].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.ROUTE_DESC_DISPLAY_TXT) && m.ROUTE_DESC_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[10].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.IN_TRANSIT_TO_DISPLAY_TXT) && m.IN_TRANSIT_TO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[11].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.SALES_ORDER_NO) && m.SALES_ORDER_NO.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[12].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.SALES_ORDER_CREATE_DATE) && m.SALES_ORDER_CREATE_DATE.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[13].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.SALES_ORDER_ITEM) && m.SALES_ORDER_ITEM.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[14].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.BRAND_DISPLAY_TXT) && m.BRAND_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[15].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.ADDITIONAL_BRAND_DISPLAY_TXT) && m.ADDITIONAL_BRAND_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[16].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.PRODUCT_CODE_DISPLAY_TXT) && m.PRODUCT_CODE_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[17].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.PRODUCTION_MEMO_DISPLAY_TXT) && m.PRODUCTION_MEMO_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[18].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => !string.IsNullOrEmpty(m.RD_NUMBER_DISPLAY_TXT) && m.RD_NUMBER_DISPLAY_TXT.ToLower().Contains(filterValue.ToLower())).ToList();

        //    filterValue = param.columns[19].search.value;
        //    if (!string.IsNullOrEmpty(filterValue))
        //        data = data.Where(m => m.RDD != null && m.RDD.Value.ToString("dd/MM/yyyy").ToLower().Contains(filterValue.ToLower())).ToList();

        //    return data;
        //}
        //private static List<TRACKING_REPORT> OrderByDataMKView(List<TRACKING_REPORT> data, int orderColumn, string orderDir)
        //{
        //    string orderASC = "asc";
        //    string orderDESC = "desc";

        //    if (orderColumn == 1)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.PIC_DISPLAY_TXT).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.PIC_DISPLAY_TXT).ToList();
        //    }

        //    if (orderColumn == 2)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.WORKFLOW_NUMBER).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.WORKFLOW_NUMBER).ToList();
        //    }

        //    if (orderColumn == 3)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.PACKING_TYPE_DISPLAY_TXT).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.PACKING_TYPE_DISPLAY_TXT).ToList();
        //    }

        //    if (orderColumn == 4)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.PRIMARY_SIZE_DISPLAY_TXT).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.PRIMARY_SIZE_DISPLAY_TXT).ToList();
        //    }

        //    if (orderColumn == 5)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.CURRENT_STEP_DISPLAY_TXT).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.CURRENT_STEP_DISPLAY_TXT).ToList();
        //    }

        //    if (orderColumn == 6)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.CURRENT_STATUS_DISPLAY_TXT).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.CURRENT_STATUS_DISPLAY_TXT).ToList();
        //    }

        //    if (orderColumn == 7)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.SOLD_TO_DISPLAY_TXT).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.SOLD_TO_DISPLAY_TXT).ToList();
        //    }

        //    if (orderColumn == 8)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.SHIP_TO_DISPLAY_TXT).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.SHIP_TO_DISPLAY_TXT).ToList();
        //    }

        //    if (orderColumn == 9)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.ROUTE_DESC_DISPLAY_TXT).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.ROUTE_DESC_DISPLAY_TXT).ToList();
        //    }

        //    if (orderColumn == 10)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.IN_TRANSIT_TO_DISPLAY_TXT).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.IN_TRANSIT_TO_DISPLAY_TXT).ToList();
        //    }

        //    if (orderColumn == 11)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.SALES_ORDER_NO).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.SALES_ORDER_NO).ToList();
        //    }

        //    if (orderColumn == 12)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.SALES_ORDER_CREATE_DATE).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.SALES_ORDER_CREATE_DATE).ToList();
        //    }

        //    if (orderColumn == 13)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.SALES_ORDER_ITEM).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.SALES_ORDER_ITEM).ToList();
        //    }

        //    if (orderColumn == 14)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.BRAND_DISPLAY_TXT).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.BRAND_DISPLAY_TXT).ToList();
        //    }

        //    if (orderColumn == 15)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.ADDITIONAL_BRAND_DISPLAY_TXT).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.ADDITIONAL_BRAND_DISPLAY_TXT).ToList();
        //    }

        //    if (orderColumn == 16)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.PRODUCT_CODE_DISPLAY_TXT).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.PRODUCT_CODE_DISPLAY_TXT).ToList();
        //    }

        //    if (orderColumn == 17)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.PRODUCTION_MEMO_DISPLAY_TXT).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.PRODUCTION_MEMO_DISPLAY_TXT).ToList();
        //    }

        //    if (orderColumn == 18)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.RD_NUMBER_DISPLAY_TXT).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.RD_NUMBER_DISPLAY_TXT).ToList();
        //    }

        //    if (orderColumn == 19)
        //    {
        //        if (orderDir == orderASC)
        //            data = data.OrderBy(i => i.RDD).ToList();
        //        else if (orderDir == orderDESC)
        //            data = data.OrderByDescending(i => i.RDD).ToList();
        //    }

        //    return data;
        //}

        public static List<V_ART_WF_DASHBOARD> QueryTrackingReportMockup(TRACKING_REPORT_REQUEST param, bool excel)
        {
            List<V_ART_WF_DASHBOARD> allMockupTrans = new List<V_ART_WF_DASHBOARD>();
            if (!String.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE) && param.data.CURRENT_STEP_WF_TYPE.StartsWith("A"))
            {
                return allMockupTrans;
            }

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;

                    DateTime REQUEST_DATE_FROM = new DateTime();
                    DateTime REQUEST_DATE_TO = new DateTime();
                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_FROM)) REQUEST_DATE_FROM = CNService.ConvertStringToDate(param.data.REQUEST_DATE_FROM);
                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_TO)) REQUEST_DATE_TO = CNService.ConvertStringToDate(param.data.REQUEST_DATE_TO);

                    IQueryable<V_ART_WF_DASHBOARD> q = null;

                    if (excel)
                        q = (from m in context.V_ART_WF_DASHBOARD
                             select m);
                    else
                        q = (from m in context.V_ART_WF_DASHBOARD
                             where m.CURRENT_STEP_ID == SEND_PG
                             select m);

                    if (param.data.WORKFLOW_OVERDUE)
                    {
                        var tempDashboard = (from m in context.V_ART_WF_DASHBOARD
                                             where string.IsNullOrEmpty(m.IS_END)
                                             select m).ToList();
                        //var tempDashboard = V_ART_WF_DASHBOARD_SERVICE.GetAll();

                        var newTemp = new List<V_ART_WF_DASHBOARD>();
                        foreach (var item in tempDashboard)
                        {
                            if (string.IsNullOrEmpty(item.IS_END))
                            {
                                if (item.CREATE_DATE_PROCESS != null)
                                {
                                    DateTime? dtReceiveWf = item.CREATE_DATE_PROCESS;
                                    DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION.Value));

                                    if (!string.IsNullOrEmpty(item.IS_STEP_DURATION_EXTEND))
                                    {
                                        dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION_EXTEND.Value));
                                    }

                                    var overdue = CNService.GetWorkingDays(DateTime.Now, dtWillFinish.Value);
                                    if (overdue < 0)
                                    {
                                        newTemp.Add(item);
                                    }
                                }
                            }
                        }
                        if (newTemp.Count > 0)
                        {
                            var listMockupId = newTemp.Select(m => m.MOCKUP_ID).ToList();
                            q = (from r in q
                                 where listMockupId.Contains(r.MOCKUP_ID)
                                 select r);
                        }
                        else
                        {
                            q = (from r in q
                                 where 1 == 2
                                 select r);
                        }
                    }

                    if (param.data.WORKFLOW_ACTION_BY_ME)
                    {
                        param.data.CURRENT_ASSIGN_ID = param.data.CURRENT_USER_ID;
                    }

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_TYPE))
                        q = (from r in q where r.MOCKUP_NO.StartsWith(param.data.WORKFLOW_TYPE) select r);

                    if (param.data.CURRENT_ASSIGN_ID > 0)
                    {
                        var temp = V_ART_WF_DASHBOARD_SERVICE.GetByItem(new V_ART_WF_DASHBOARD() { CURRENT_USER_ID = param.data.CURRENT_ASSIGN_ID }, context);
                        var listMockupId = temp.Where(m => string.IsNullOrEmpty(m.IS_END)).Select(m => m.MOCKUP_ID);
                        q = (from r in q
                             where listMockupId.Contains(r.MOCKUP_ID)
                             select r);
                    }

                    if (param.data.WORKING_GROUP_ID > 0)
                    {
                        var listMockupId = V_ART_WF_DASHBOARD_SERVICE.GetByItem(new V_ART_WF_DASHBOARD() { CURRENT_USER_ID = param.data.WORKING_GROUP_ID }, context).ToList().Select(m => m.MOCKUP_ID);
                        var listCheckListId = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST() { CREATOR_ID = param.data.WORKING_GROUP_ID }, context).ToList().Select(m => m.CHECK_LIST_ID);

                        q = (from r in q
                             where listMockupId.Contains(r.MOCKUP_ID) || listCheckListId.Contains(r.CHECK_LIST_ID)
                             select r);
                    }

                    if (param.data.SUPERVISED_ID > 0)
                    {
                        var listuserlowerId = new List<int>();
                        //tier1
                        var listUserId = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { UPPER_USER_ID = param.data.SUPERVISED_ID }, context).Select(m => m.USER_ID).ToList();
                        if (listUserId != null)
                        {
                            foreach (var item1 in listUserId)
                            {
                                //tier2
                                var listuserlowerId2 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item1)).Select(t => t.USER_ID).ToList();
                                if (listuserlowerId2.Count() > 0)
                                {
                                    foreach (var item2 in listuserlowerId2)
                                    {
                                        listuserlowerId.Add(item2);
                                        //tier3
                                        var listuserlowerId3 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item2)).Select(t => t.USER_ID).ToList();
                                        if (listuserlowerId3.Count() > 0)
                                        {
                                            foreach (var item3 in listuserlowerId3)
                                            {
                                                listuserlowerId.Add(item3);
                                                //tier4
                                                var listuserlowerId4 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item3)).Select(t => t.USER_ID).ToList();
                                                if (listuserlowerId4.Count() > 0)
                                                {
                                                    foreach (var item4 in listuserlowerId4)
                                                    {
                                                        listuserlowerId.Add(item4);
                                                        //tier5
                                                        var listuserlowerId5 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item4)).Select(t => t.USER_ID).ToList();
                                                        if (listuserlowerId5.Count() > 0)
                                                        {
                                                            foreach (var item5 in listuserlowerId5)
                                                            {
                                                                listuserlowerId.Add(item5);
                                                                //tier6
                                                                var listuserlowerId6 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item5)).Select(t => t.USER_ID).ToList();
                                                                if (listuserlowerId6.Count() > 0)
                                                                {
                                                                    foreach (var item6 in listuserlowerId6)
                                                                    {
                                                                        listuserlowerId.Add(item6);
                                                                        //tier7
                                                                        var listuserlowerId7 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item6)).Select(t => t.USER_ID).ToList();
                                                                        if (listuserlowerId7.Count() > 0)
                                                                        {
                                                                            foreach (var item7 in listuserlowerId7)
                                                                            {
                                                                                listuserlowerId.Add(item7);
                                                                                //tier8
                                                                                var listuserlowerId8 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item7)).Select(t => t.USER_ID).ToList();
                                                                                if (listuserlowerId8.Count() > 0)
                                                                                {
                                                                                    foreach (var item8 in listuserlowerId8)
                                                                                    {
                                                                                        listuserlowerId.Add(item8);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }


                                    }
                                }
                            }
                            foreach (var a in listuserlowerId)
                                listUserId.Add(a);
                        }
                        var Final = listUserId.Distinct().ToList();

                        var listMockupId = (from w in context.V_ART_WF_DASHBOARD
                                            where Final.Contains(w.CURRENT_USER_ID.Value) && string.IsNullOrEmpty(w.IS_END)
                                            select w.MOCKUP_ID);

                        var listCheckListId = (from e in context.ART_WF_MOCKUP_CHECK_LIST
                                               where Final.Contains(e.CREATOR_ID.Value)
                                               select e.CHECK_LIST_ID);

                        q = (from r in q
                             where listMockupId.Contains(r.MOCKUP_ID) || listCheckListId.Contains(r.CHECK_LIST_ID)
                             select r);

                        //q = (from r in q
                        //     where listUserId.Contains(r.CURRENT_USER_ID.Value) || listUserId.Contains(r.CREATOR_ID.Value)
                        //     select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.REF_WF_NO))
                    {
                        var list = new List<ART_WF_MOCKUP_CHECK_LIST>();
                        var checklist = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST() { CHECK_LIST_NO = param.data.REF_WF_NO.Trim() }, context).FirstOrDefault();
                        if (checklist != null)
                        {
                            list.Add(new ART_WF_MOCKUP_CHECK_LIST() { CHECK_LIST_NO = checklist.CHECK_LIST_NO });
                            var refChecklist = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST() { REFERENCE_REQUEST_NO = param.data.REF_WF_NO.Trim() }, context).ToList();

                            refChecklist.ForEach(r =>
                            {
                                list.Add(new ART_WF_MOCKUP_CHECK_LIST() { CHECK_LIST_NO = r.CHECK_LIST_NO });
                            });

                            if (!string.IsNullOrEmpty(checklist.REFERENCE_REQUEST_NO))
                            {
                                var temp = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST() { CHECK_LIST_NO = checklist.REFERENCE_REQUEST_NO }, context).FirstOrDefault();
                                if (temp != null)
                                {
                                    list.Add(new ART_WF_MOCKUP_CHECK_LIST() { CHECK_LIST_NO = temp.CHECK_LIST_NO });
                                }
                            }
                        }

                        var tempList = list.Select(m => m.CHECK_LIST_NO).ToList();
                        q = (from r in q where tempList.Contains(r.CHECK_LIST_NO) select r);

                        //q = (from r in q where r.REFERENCE_REQUEST_NO.Contains(param.data.REF_WF_NO.Trim()) || r.CHECK_LIST_NO.Contains(param.data.REF_WF_NO.Trim()) select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_NO))
                        q = (from r in q where r.MOCKUP_NO.Contains(param.data.WORKFLOW_NO.Trim()) select r);

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_NO_2))
                        q = (from r in q where r.CHECK_LIST_NO.Contains(param.data.WORKFLOW_NO_2.Trim()) select r);

                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_FROM))
                        q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE_CHECK_LIST) >= REQUEST_DATE_FROM select r);

                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_TO))
                        q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE_CHECK_LIST) <= REQUEST_DATE_TO select r);



                    if (param.data.WORKFLOW_COMPLETED)
                        q = (from r in q where r.IS_END == "X" && string.IsNullOrEmpty(r.IS_TERMINATE) select r);

                    if (param.data.SOLD_TO_ID > 0)
                        q = (from r in q where r.SOLD_TO_ID == param.data.SOLD_TO_ID select r);

                    if (param.data.SHIP_TO_ID > 0)
                        q = (from r in q where r.SHIP_TO_ID == param.data.SHIP_TO_ID select r);

                    if (param.data.COUNTRY_ID > 0)
                        q = (from r in q join j in context.ART_WF_MOCKUP_CHECK_LIST_COUNTRY on r.CHECK_LIST_ID equals j.CHECK_LIST_ID where j.COUNTRY_ID == param.data.COUNTRY_ID select r);

                    if (!string.IsNullOrEmpty(param.data.ZONE_TXT))
                    {
                        var listCountryId = SAP_M_COUNTRY_SERVICE.GetByItem(new SAP_M_COUNTRY() { ZONE = param.data.ZONE_TXT }, context).Select(m => m.COUNTRY_ID);
                        var listCheckListId = (from r in context.ART_WF_MOCKUP_CHECK_LIST_COUNTRY
                                               where listCountryId.Contains(r.COUNTRY_ID)
                                               select r.CHECK_LIST_ID).ToList();

                        q = (from r in q
                             where listCheckListId.Contains(r.CHECK_LIST_ID)
                             select r);
                    }

                    if (param.data.COMPANY_ID > 0)
                        q = (from r in q where r.COMPANY_ID == param.data.COMPANY_ID select r);

                    if (param.data.BRAND_ID > 0)
                        q = (from r in q where r.BRAND_ID == param.data.BRAND_ID select r);

                    if (!string.IsNullOrEmpty(param.data.PROJECT_NAME))
                        q = (from r in q where r.PROJECT_NAME.Contains(param.data.PROJECT_NAME.Trim()) select r);

                    if (!string.IsNullOrEmpty(param.data.PRODUCT_CODE))
                    {
                        var arrPRODUCT_CODE = param.data.PRODUCT_CODE.Replace(" ", "").Split(',');
                        var listCheckListID = (from m in context.ART_WF_MOCKUP_CHECK_LIST_PRODUCT
                                               where arrPRODUCT_CODE.Contains(m.PRODUCT_CODE.Trim())
                                               select m.CHECK_LIST_ID);

                        q = (from r in q
                             where listCheckListID.Contains(r.CHECK_LIST_ID)
                             select r);
                        //q = (from r in q join j in context.ART_WF_MOCKUP_CHECK_LIST_PRODUCT on r.CHECK_LIST_ID equals j.CHECK_LIST_ID where param.data.PRODUCT_CODE.Contains(j.PRODUCT_CODE) select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.RD_NUMBER))
                    {
                        var listCheckListID = (from m in context.ART_WF_MOCKUP_CHECK_LIST_REFERENCE
                                               where param.data.RD_NUMBER.Contains(m.REFERENCE_NO.Trim())
                                               select m.CHECK_LIST_ID);
                        q = (from r in q
                             where listCheckListID.Contains(r.CHECK_LIST_ID)
                             select r);
                        //q = (from r in q join j in context.ART_WF_MOCKUP_CHECK_LIST_REFERENCE on r.CHECK_LIST_ID equals j.CHECK_LIST_ID where param.data.RD_NUMBER.Contains(j.REFERENCE_NO) select r);
                    }

                    if (param.data.PACKAGING_TYPE_ID > 0)
                        q = (from r in q join j in context.ART_WF_MOCKUP_CHECK_LIST_ITEM on r.MOCKUP_NO equals j.MOCKUP_NO where !string.IsNullOrEmpty(r.MOCKUP_NO) && j.PACKING_TYPE_ID == param.data.PACKAGING_TYPE_ID select r);

                    if (param.data.CREATOR_ID > 0)
                        q = (from r in q where r.CREATOR_ID == param.data.CREATOR_ID select r);

                    if (!string.IsNullOrEmpty(param.data.PRIMARY_SIZE_TXT))
                    {
                        q = (from r in q
                             join j in context.SAP_M_3P on r.THREE_P_ID equals j.THREE_P_ID into ps
                             from j in ps.DefaultIfEmpty()
                             join j2 in context.ART_WF_MOCKUP_CHECK_LIST_PRODUCT on r.CHECK_LIST_ID equals j2.CHECK_LIST_ID into ps2
                             from j2 in ps2.DefaultIfEmpty()
                             where (j.PRIMARY_SIZE_VALUE.Contains(param.data.PRIMARY_SIZE_TXT.Trim()) || j2.PRIMARY_SIZE.Contains(param.data.PRIMARY_SIZE_TXT.Trim()))
                             select r).Distinct();
                    }

                    if (!string.IsNullOrEmpty(param.data.NET_WEIGHT_TXT))
                    {
                        q = (from r in q
                             join j in context.ART_WF_MOCKUP_CHECK_LIST_PRODUCT on r.CHECK_LIST_ID equals j.CHECK_LIST_ID into ps
                             from j in ps.DefaultIfEmpty()
                             join j2 in context.ART_WF_MOCKUP_CHECK_LIST_REFERENCE on r.CHECK_LIST_ID equals j2.CHECK_LIST_ID into ps2
                             from j2 in ps2.DefaultIfEmpty()
                             where (j2.NET_WEIGHT.Contains(param.data.NET_WEIGHT_TXT.Trim()) || j.NET_WEIGHT.Contains(param.data.NET_WEIGHT_TXT.Trim()))
                             select r).Distinct();
                    }

                    if (!string.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE))
                    {
                        if (param.data.CURRENT_STEP_WF_TYPE.StartsWith("M"))
                        {
                            var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP_WF_TYPE.Replace("M", ""));
                            var temp = V_ART_WF_DASHBOARD_SERVICE.GetByItem(new V_ART_WF_DASHBOARD() { CURRENT_STEP_ID = current_step_id }, context);
                            var listMockupId = temp.Where(m => string.IsNullOrEmpty(m.IS_END)).Select(m => m.MOCKUP_ID);
                            q = (from r in q
                                 where listMockupId.Contains(r.MOCKUP_ID)
                                 select r);
                        }
                    }

                    allMockupTrans = q.OrderBy(i => i.MOCKUP_NO).ToList();
                }
            }

            return allMockupTrans;
        }

        public static List<V_ART_WF_DASHBOARD_ARTWORK> QueryTrackingReportArtwork(TRACKING_REPORT_REQUEST param, bool excel)
        {
            List<V_ART_WF_DASHBOARD_ARTWORK> allArtworkTrans = new List<V_ART_WF_DASHBOARD_ARTWORK>();
            if (!String.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE) && param.data.CURRENT_STEP_WF_TYPE.StartsWith("M"))
            {
                return allArtworkTrans;
            }

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                    DateTime REQUEST_DATE_FROM = new DateTime();
                    DateTime REQUEST_DATE_TO = new DateTime();
                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_FROM)) REQUEST_DATE_FROM = CNService.ConvertStringToDate(param.data.REQUEST_DATE_FROM);
                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_TO)) REQUEST_DATE_TO = CNService.ConvertStringToDate(param.data.REQUEST_DATE_TO);

                    IQueryable<V_ART_WF_DASHBOARD_ARTWORK> q = null;

                    if (excel)
                        q = (from m in context.V_ART_WF_DASHBOARD_ARTWORK
                             select m);
                    else
                        q = (from m in context.V_ART_WF_DASHBOARD_ARTWORK
                             where m.CURRENT_STEP_ID == SEND_PA
                             select m);

                    if (param.data.WORKFLOW_OVERDUE)
                    {
                        var tempDashboard = (from m in context.V_ART_WF_DASHBOARD_ARTWORK
                                             where string.IsNullOrEmpty(m.IS_END)
                                             select m).ToList();

                        //var tempDashboard = V_ART_WF_DASHBOARD_ARTWORK_SERVICE.GetAll();

                        var newTemp = new List<V_ART_WF_DASHBOARD_ARTWORK>();
                        foreach (var item in tempDashboard)
                        {
                            if (string.IsNullOrEmpty(item.IS_END))
                            {
                                if (item.CREATE_DATE_PROCESS != null)
                                {
                                    DateTime? dtReceiveWf = item.CREATE_DATE_PROCESS;
                                    DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION.Value));

                                    if (!string.IsNullOrEmpty(item.IS_STEP_DURATION_EXTEND))
                                    {
                                        dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item.DURATION_EXTEND.Value));
                                    }

                                    var overdue = CNService.GetWorkingDays(DateTime.Now, dtWillFinish.Value);
                                    if (overdue < 0)
                                    {
                                        newTemp.Add(item);
                                    }
                                }
                            }
                        }
                        if (newTemp.Count > 0)
                        {
                            var listArtworkItemId = newTemp.Select(m => m.ARTWORK_ITEM_ID).ToList();
                            q = (from r in q
                                 where listArtworkItemId.Contains(r.ARTWORK_ITEM_ID)
                                 select r);
                        }
                        else
                        {
                            q = (from r in q
                                 where 1 == 2
                                 select r);
                        }
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_SO))
                    {
                        var arrSO = param.data.SEARCH_SO.Replace(" ", "").Split(',');
                        var listArtworkSubId = (from m in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
                                                where arrSO.Contains(m.SALES_ORDER_NO)
                                                select m.ARTWORK_SUB_ID);
                        //var temp = ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_SERVICE.GetByItemContain(new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER() { SALES_ORDER_NO = param.data.SEARCH_SO });
                        //var listArtworkSubId = temp.Select(m => m.ARTWORK_SUB_ID);
                        q = (from r in q
                             where listArtworkSubId.Contains(r.ARTWORK_SUB_ID)
                             select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.SEARCH_ORDER_BOM))
                    {
                        var arrSEARCH_ORDER_BOM = param.data.SEARCH_ORDER_BOM.Replace(" ", "").Split(',');
                        var listArtworkSubId = (from m in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT
                                                where arrSEARCH_ORDER_BOM.Contains(m.COMPONENT_MATERIAL)
                                                select m.ARTWORK_SUB_ID)
                                                .Union(from m in context.ART_WF_ARTWORK_PROCESS_PA
                                                       where arrSEARCH_ORDER_BOM.Contains(m.MATERIAL_NO)
                                                       select m.ARTWORK_SUB_ID);
                        //var temp = ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_SERVICE.GetByItemContain(new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT() { COMPONENT_MATERIAL = param.data.SEARCH_ORDER_BOM });
                        //var listArtworkSubId = temp.Select(m => m.ARTWORK_SUB_ID);
                        q = (from r in q
                             where listArtworkSubId.Contains(r.ARTWORK_SUB_ID)
                             select r);

                    }

                    if (!string.IsNullOrEmpty(param.data.CURRENT_STEP_WF_TYPE))
                    {
                        if (param.data.CURRENT_STEP_WF_TYPE.StartsWith("A"))
                        {
                            var current_step_id = Convert.ToInt32(param.data.CURRENT_STEP_WF_TYPE.Replace("A", ""));
                            var temp = V_ART_WF_DASHBOARD_ARTWORK_SERVICE.GetByItem(new V_ART_WF_DASHBOARD_ARTWORK() { CURRENT_STEP_ID = current_step_id }, context);
                            var listArtworkItemId = temp.Where(m => string.IsNullOrEmpty(m.IS_END)).Select(m => m.ARTWORK_ITEM_ID);
                            q = (from r in q
                                 where listArtworkItemId.Contains(r.ARTWORK_ITEM_ID)
                                 select r);
                        }
                    }

                    if (param.data.WORKFLOW_ACTION_BY_ME)
                    {
                        param.data.CURRENT_ASSIGN_ID = param.data.CURRENT_USER_ID;
                    }

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_TYPE))
                        q = (from r in q where r.REQUEST_ITEM_NO.StartsWith(param.data.WORKFLOW_TYPE) select r);

                    if (param.data.CURRENT_ASSIGN_ID > 0)
                    {
                        var temp = V_ART_WF_DASHBOARD_ARTWORK_SERVICE.GetByItem(new V_ART_WF_DASHBOARD_ARTWORK() { CURRENT_USER_ID = param.data.CURRENT_ASSIGN_ID }, context);
                        var listArtworkItemId = temp.Where(m => string.IsNullOrEmpty(m.IS_END)).Select(m => m.ARTWORK_ITEM_ID);
                        q = (from r in q
                             where listArtworkItemId.Contains(r.ARTWORK_ITEM_ID)
                             select r);
                    }

                    if (param.data.WORKING_GROUP_ID > 0)
                    {
                        var listArtworkItemId = V_ART_WF_DASHBOARD_ARTWORK_SERVICE.GetByItem(new V_ART_WF_DASHBOARD_ARTWORK() { CURRENT_USER_ID = param.data.WORKING_GROUP_ID }, context).ToList().Select(m => m.ARTWORK_ITEM_ID);
                        var listRequestId = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { CREATOR_ID = param.data.WORKING_GROUP_ID }, context).ToList().Select(m => m.ARTWORK_REQUEST_ID);

                        q = (from r in q
                             where listArtworkItemId.Contains(r.ARTWORK_ITEM_ID) || listRequestId.Contains(r.ARTWORK_REQUEST_ID)
                             select r);
                    }

                    if (param.data.SUPERVISED_ID > 0)
                    {
                        var listuserlowerId = new List<int>();
                        //tier1
                        var listUserId = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { UPPER_USER_ID = param.data.SUPERVISED_ID }, context).Select(m => m.USER_ID).ToList();
                        if (listUserId != null)
                        {
                            foreach (var item1 in listUserId)
                            {
                                //tier2
                                var listuserlowerId2 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item1)).Select(t => t.USER_ID).ToList();
                                if (listuserlowerId2.Count() > 0)
                                {
                                    foreach (var item2 in listuserlowerId2)
                                    {
                                        listuserlowerId.Add(item2);
                                        //tier3
                                        var listuserlowerId3 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item2)).Select(t => t.USER_ID).ToList();
                                        if (listuserlowerId3.Count() > 0)
                                        {
                                            foreach (var item3 in listuserlowerId3)
                                            {
                                                listuserlowerId.Add(item3);
                                                //tier4
                                                var listuserlowerId4 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item3)).Select(t => t.USER_ID).ToList();
                                                if (listuserlowerId4.Count() > 0)
                                                {
                                                    foreach (var item4 in listuserlowerId4)
                                                    {
                                                        listuserlowerId.Add(item4);
                                                        //tier5
                                                        var listuserlowerId5 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item4)).Select(t => t.USER_ID).ToList();
                                                        if (listuserlowerId5.Count() > 0)
                                                        {
                                                            foreach (var item5 in listuserlowerId5)
                                                            {
                                                                listuserlowerId.Add(item5);
                                                                //tier6
                                                                var listuserlowerId6 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item5)).Select(t => t.USER_ID).ToList();
                                                                if (listuserlowerId6.Count() > 0)
                                                                {
                                                                    foreach (var item6 in listuserlowerId6)
                                                                    {
                                                                        listuserlowerId.Add(item6);
                                                                        //tier7
                                                                        var listuserlowerId7 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item6)).Select(t => t.USER_ID).ToList();
                                                                        if (listuserlowerId7.Count() > 0)
                                                                        {
                                                                            foreach (var item7 in listuserlowerId7)
                                                                            {
                                                                                listuserlowerId.Add(item7);
                                                                                //tier8
                                                                                var listuserlowerId8 = context.ART_M_USER_UPPER_LEVEL.Where(t => t.UPPER_USER_ID.Equals(item7)).Select(t => t.USER_ID).ToList();
                                                                                if (listuserlowerId8.Count() > 0)
                                                                                {
                                                                                    foreach (var item8 in listuserlowerId8)
                                                                                    {
                                                                                        listuserlowerId.Add(item8);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }


                                    }
                                }
                            }
                            foreach (var a in listuserlowerId)
                                listUserId.Add(a);
                        }
                        var Final = listUserId.Distinct().ToList();
                        //q = (from r in q
                        //     where Final.Contains(r.CURRENT_USER_ID.Value) || Final.Contains(r.CREATE_BY_ARTWORK_REQUEST)
                        //     select r);
                        //var listArtworkItemId = V_ART_WF_DASHBOARD_ARTWORK_SERVICE.GetByItem(new V_ART_WF_DASHBOARD_ARTWORK() { CURRENT_USER_ID = param.data.WORKING_GROUP_ID }, context).ToList().Select(m => m.ARTWORK_ITEM_ID);
                        //var listRequestId = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { CREATOR_ID = param.data.WORKING_GROUP_ID }, context).ToList().Select(m => m.ARTWORK_REQUEST_ID);

                        var listArtworkItemId = (from w in context.V_ART_WF_DASHBOARD_ARTWORK
                                                 where Final.Contains(w.CURRENT_USER_ID.Value) && string.IsNullOrEmpty(w.IS_END)
                                                 select w.ARTWORK_ITEM_ID);

                        var listRequestId = (from e in context.ART_WF_ARTWORK_REQUEST
                                             where Final.Contains(e.CREATOR_ID.Value)
                                             select e.ARTWORK_REQUEST_ID);

                        q = (from r in q
                             where listArtworkItemId.Contains(r.ARTWORK_ITEM_ID) || listRequestId.Contains(r.ARTWORK_REQUEST_ID)
                             select r);
                    }


                    if (!string.IsNullOrEmpty(param.data.REF_WF_NO))
                    {
                        var list = new List<ART_WF_ARTWORK_REQUEST>();
                        var checklist = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_NO = param.data.REF_WF_NO.Trim() }, context).FirstOrDefault();
                        if (checklist != null)
                        {
                            list.Add(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_NO = checklist.ARTWORK_REQUEST_NO });
                            var refChecklist = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { REFERENCE_REQUEST_NO = param.data.REF_WF_NO.Trim() }, context).FirstOrDefault();
                            if (refChecklist != null)
                            {
                                list.Add(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_NO = refChecklist.ARTWORK_REQUEST_NO });
                            }
                            if (!string.IsNullOrEmpty(checklist.REFERENCE_REQUEST_NO))
                            {
                                var temp = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_NO = checklist.REFERENCE_REQUEST_NO }, context).FirstOrDefault();
                                if (temp != null)
                                {
                                    list.Add(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_NO = temp.ARTWORK_REQUEST_NO });
                                }
                            }
                        }

                        var tempList = list.Select(m => m.ARTWORK_REQUEST_NO).ToList();
                        q = (from r in q where tempList.Contains(r.ARTWORK_REQUEST_NO) select r);

                        //q = (from r in q where r.REFERENCE_REQUEST_NO.Contains(param.data.REF_WF_NO.Trim()) || r.ARTWORK_REQUEST_NO.Contains(param.data.REF_WF_NO.Trim()) select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_NO))
                        q = (from r in q where r.REQUEST_ITEM_NO.Contains(param.data.WORKFLOW_NO.Trim()) select r);

                    if (!string.IsNullOrEmpty(param.data.WORKFLOW_NO_2))
                        q = (from r in q where r.ARTWORK_REQUEST_NO.Contains(param.data.WORKFLOW_NO_2.Trim()) select r);

                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_FROM))
                        q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE_ARTWORK_REQUEST) >= REQUEST_DATE_FROM select r);

                    if (!string.IsNullOrEmpty(param.data.REQUEST_DATE_TO))
                        q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE_ARTWORK_REQUEST) <= REQUEST_DATE_TO select r);

                    if (param.data.WORKFLOW_COMPLETED)
                        q = (from r in q where r.IS_END == "X" && string.IsNullOrEmpty(r.IS_TERMINATE) select r);

                    if (param.data.SOLD_TO_ID > 0)
                        q = (from r in q where r.SOLD_TO_ID == param.data.SOLD_TO_ID select r);

                    if (param.data.SHIP_TO_ID > 0)
                        q = (from r in q where r.SHIP_TO_ID == param.data.SHIP_TO_ID select r);

                    if (param.data.COUNTRY_ID > 0)
                        q = (from r in q join j in context.ART_WF_ARTWORK_REQUEST_COUNTRY on r.ARTWORK_REQUEST_ID equals j.ARTWORK_REQUEST_ID where j.COUNTRY_ID == param.data.COUNTRY_ID select r);

                    if (!string.IsNullOrEmpty(param.data.ZONE_TXT))
                    {
                        var listCountryId = SAP_M_COUNTRY_SERVICE.GetByItem(new SAP_M_COUNTRY() { ZONE = param.data.ZONE_TXT }, context).Select(m => m.COUNTRY_ID);
                        var listARTWORK_REQUEST_ID = (from r in context.ART_WF_ARTWORK_REQUEST_COUNTRY
                                                      where listCountryId.Contains(r.COUNTRY_ID)
                                                      select r.ARTWORK_REQUEST_ID).ToList();

                        q = (from r in q
                             where listARTWORK_REQUEST_ID.Contains(r.ARTWORK_REQUEST_ID)
                             select r);
                    }

                    if (param.data.COMPANY_ID > 0)
                        q = (from r in q where r.COMPANY_ID == param.data.COMPANY_ID select r);

                    if (param.data.BRAND_ID > 0)
                        q = (from r in q where r.BRAND_ID == param.data.BRAND_ID select r);

                    if (!string.IsNullOrEmpty(param.data.PROJECT_NAME))
                        q = (from r in q where r.PROJECT_NAME.Contains(param.data.PROJECT_NAME.Trim()) select r);

                    if (!string.IsNullOrEmpty(param.data.PRODUCT_CODE))
                    {
                        var arrPRODUCT_CODE = param.data.PRODUCT_CODE.Replace(" ", "").Split(',');
                        var listArtworkSubId = (from r in q
                                                join j2 in context.ART_WF_ARTWORK_REQUEST_PRODUCT on r.ARTWORK_REQUEST_ID equals j2.ARTWORK_REQUEST_ID into ps2
                                                from j2 in ps2.DefaultIfEmpty()
                                                join j3 in context.XECM_M_PRODUCT on j2.PRODUCT_CODE_ID equals j3.XECM_PRODUCT_ID into ps3
                                                from j3 in ps3.DefaultIfEmpty()
                                                where arrPRODUCT_CODE.Contains(j3.PRODUCT_CODE)
                                                select r.ARTWORK_SUB_ID).Distinct();
                        q = (from r in q
                             where listArtworkSubId.Contains(r.ARTWORK_SUB_ID)
                             select r);


                        //q = (from r in q
                        //     join j2 in context.ART_WF_ARTWORK_REQUEST_PRODUCT on r.ARTWORK_REQUEST_ID equals j2.ARTWORK_REQUEST_ID into ps2
                        //     from j2 in ps2.DefaultIfEmpty()
                        //     join j3 in context.XECM_M_PRODUCT on j2.PRODUCT_CODE_ID equals j3.XECM_PRODUCT_ID into ps3
                        //     from j3 in ps3.DefaultIfEmpty()
                        //     where (j3.PRODUCT_CODE.Contains(param.data.PRODUCT_CODE.Trim()))
                        //     select r).Distinct();
                        //var listCheckListID = (from m in context.ART_WF_ARTWORK_REQUEST_PRODUCT
                        //                       where param.data.PRODUCT_CODE.Contains(m.PRODUCT_CODE.Trim())
                        //                       select m.CHECK_LIST_ID);
                        //q = (from r in q
                        //     where listCheckListID.Contains(r.CHECK_LIST_ID)
                        //     select r);
                        //q = (from r in q join j in context.ART_WF_MOCKUP_CHECK_LIST_PRODUCT on r.CHECK_LIST_ID equals j.CHECK_LIST_ID where param.data.PRODUCT_CODE.Contains(j.PRODUCT_CODE) select r);
                    }

                    if (!string.IsNullOrEmpty(param.data.RD_NUMBER))
                    {
                        var listArtworkRequestID = (from m in context.ART_WF_ARTWORK_REQUEST_REFERENCE
                                                    where param.data.RD_NUMBER.Contains(m.REFERENCE_NO.Trim())
                                                    select m.ARTWORK_REQUEST_ID);
                        q = (from r in q
                             where listArtworkRequestID.Contains(r.ARTWORK_REQUEST_ID)
                             select r);
                        //q = (from r in q join j in context.ART_WF_MOCKUP_CHECK_LIST_REFERENCE on r.CHECK_LIST_ID equals j.CHECK_LIST_ID where param.data.RD_NUMBER.Contains(j.REFERENCE_NO) select r);
                    }

                    if (param.data.PACKAGING_TYPE_ID > 0)
                    {
                        var listARTWORK_SUB_ID = (from m in context.ART_WF_ARTWORK_PROCESS_PA
                                                  where m.MATERIAL_GROUP_ID != null && m.MATERIAL_GROUP_ID == param.data.PACKAGING_TYPE_ID
                                                  select m.ARTWORK_SUB_ID);

                        q = (from r in q
                             where listARTWORK_SUB_ID.Contains(r.ARTWORK_SUB_ID)
                             select r);
                    }

                    if (param.data.CREATOR_ID > 0)
                        q = (from r in q where r.CREATE_BY_ARTWORK_REQUEST == param.data.CREATOR_ID select r);

                    if (!string.IsNullOrEmpty(param.data.PRIMARY_SIZE_TXT))
                    {
                        q = (from r in q
                             join j in context.SAP_M_3P on r.THREE_P_ID equals j.THREE_P_ID into ps
                             from j in ps.DefaultIfEmpty()
                             join j2 in context.ART_WF_ARTWORK_REQUEST_PRODUCT on r.ARTWORK_REQUEST_ID equals j2.ARTWORK_REQUEST_ID into ps2
                             from j2 in ps2.DefaultIfEmpty()
                             join j3 in context.XECM_M_PRODUCT on j2.PRODUCT_CODE_ID equals j3.XECM_PRODUCT_ID into ps3
                             from j3 in ps3.DefaultIfEmpty()
                             where (j.PRIMARY_SIZE_VALUE.Contains(param.data.PRIMARY_SIZE_TXT.Trim()) || j3.PRIMARY_SIZE.Contains(param.data.PRIMARY_SIZE_TXT.Trim()))
                             select r).Distinct();
                    }

                    if (!string.IsNullOrEmpty(param.data.NET_WEIGHT_TXT))
                    {
                        q = (from r in q
                             join j in context.ART_WF_ARTWORK_REQUEST_PRODUCT on r.ARTWORK_REQUEST_ID equals j.ARTWORK_REQUEST_ID into ps
                             from j in ps.DefaultIfEmpty()
                             join j3 in context.XECM_M_PRODUCT on j.PRODUCT_CODE_ID equals j3.XECM_PRODUCT_ID into ps3
                             from j3 in ps3.DefaultIfEmpty()
                             join j2 in context.ART_WF_ARTWORK_REQUEST_REFERENCE on r.ARTWORK_REQUEST_ID equals j2.ARTWORK_REQUEST_ID into ps2
                             from j2 in ps2.DefaultIfEmpty()
                             where (j2.NET_WEIGHT.Contains(param.data.NET_WEIGHT_TXT.Trim()) || j3.NET_WEIGHT.Contains(param.data.NET_WEIGHT_TXT.Trim()))
                             select r).Distinct();
                    }

                    allArtworkTrans = q.OrderBy(i => i.REQUEST_ITEM_NO).ToList();
                }
            }
            return allArtworkTrans;
        }

        private static string GetAllStepColumnMockup(List<V_ART_WF_DASHBOARD> temp_dashboard, List<ART_M_STEP_MOCKUP> allStepMockup, string msg, ARTWORKEntities context, ref int total_day, ref List<DateTime> start_date, ref List<DateTime> end_date)
        {
            StringBuilder sbMOStep = new StringBuilder();

            if (temp_dashboard.Count > 0)
                sbMOStep.AppendLine("<table class='cls_table_in_table' style='min-width:1200px;'>");

            var tempReason = (from p in context.ART_M_DECISION_REASON
                              select new ART_M_DECISION_REASON_2
                              {
                                  ART_M_DECISION_REASON_ID = p.ART_M_DECISION_REASON_ID,
                                  DESCRIPTION = p.DESCRIPTION
                              }).ToList();

            foreach (var item_allStep in temp_dashboard)
            {
                sbMOStep.AppendLine("<tr>");
                var tempMockup = allStepMockup.Where(m => m.STEP_MOCKUP_ID == item_allStep.CURRENT_STEP_ID).FirstOrDefault();
                if (tempMockup != null)
                {
                    sbMOStep.AppendLine("<td style='max-width:330px;width:330px;min-width:330px;white-space:initial;'>");
                    sbMOStep.AppendLine("<a target='_blank' href='" + System.Configuration.ConfigurationManager.AppSettings["suburl"] + "/TaskForm/" + item_allStep.MOCKUP_SUB_ID + "'>" + tempMockup.STEP_MOCKUP_NAME + "</a></td>");

                    if (CNService.GetUserName(item_allStep.CURRENT_USER_ID, context) != "")
                        sbMOStep.AppendLine("<td style='max-width:270px;width:270px;min-width:270px;white-space:initial;'>" + CNService.GetUserName(item_allStep.CURRENT_USER_ID, context) + "</td>");
                    else
                        sbMOStep.AppendLine("<td style='max-width:270px;width:270px;min-width:270px;white-space:initial;'>" + msg + " </td>");
                }

                sbMOStep.AppendLine("<td style='max-width:80px;width:80px;min-width:80px;white-space:initial;'>" + item_allStep.CREATE_DATE_PROCESS.Value.ToString("dd/MM/yyyy") + "</td>");

                var startdate = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(item_allStep.MOCKUP_SUB_ID, context).CREATE_DATE;
                var finishdate = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(item_allStep.MOCKUP_SUB_ID, context).UPDATE_DATE;
                DateTime realFinishdate = DateTime.Now;
                if (item_allStep.IS_END == "X")
                {
                    realFinishdate = finishdate;
                    sbMOStep.AppendLine("<td style='max-width:80px;width:80px;min-width:80px;white-space:initial;'>" + finishdate.ToString("dd/MM/yyyy") + "</td>");
                }
                else
                    sbMOStep.AppendLine("<td style='max-width:80px;width:80px;min-width:80px;white-space:initial;'></td>");

                if (item_allStep.CREATE_DATE_PROCESS != null)
                {
                    DateTime? dtReceiveWf = item_allStep.CREATE_DATE_PROCESS;
                    DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item_allStep.DURATION.Value));

                    if (!string.IsNullOrEmpty(item_allStep.IS_STEP_DURATION_EXTEND))
                    {
                        dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item_allStep.DURATION_EXTEND.Value));
                    }

                    if (item_allStep.IS_END == "X")
                    {
                        total_day += (CNService.GetWorkingDays(startdate, realFinishdate) + 1);
                        start_date.Add(startdate);
                        end_date.Add(realFinishdate);
                        sbMOStep.AppendLine("<td style='text-align:right;max-width:90px;width:90px;min-width:90px;white-space:initial;'>" + (CNService.GetWorkingDays(startdate, realFinishdate) + 1) + " [" + item_allStep.DURATION.Value + "]" + "</td>");
                    }
                    else
                    {
                        var duration = item_allStep.DURATION.Value;

                        if (!string.IsNullOrEmpty(item_allStep.IS_STEP_DURATION_EXTEND))
                        {
                            duration = item_allStep.DURATION_EXTEND.Value;
                        }

                        sbMOStep.AppendLine("<td style='text-align:right;max-width:90px;width:90px;min-width:90px;white-space:initial;'>" + "" + " [" + duration + "]" + "</td>");
                    }
                }

                if (item_allStep.REASON_ID > 0)
                    sbMOStep.AppendLine("<td style='max-width:170px;width:170px;min-width:170px;white-space:initial;'>" + tempReason.Where(w => w.ART_M_DECISION_REASON_ID == item_allStep.REASON_ID).FirstOrDefault().DESCRIPTION + " </td>");
                else
                    sbMOStep.AppendLine("<td style='max-width:170px;width:170px;min-width:170px;white-space:initial;'></td>");

                sbMOStep.AppendLine("</tr>");
            }

            return sbMOStep.ToString();
        }

        private static void GetAllStepColumnMockup_Excel(List<V_ART_WF_DASHBOARD> temp_dashboard, List<ART_M_STEP_MOCKUP> allStepMockup, string msg, ARTWORKEntities context, ref int total_day
             , ref string step
            , ref string step_assign
            , ref string start_date
            , ref string end_date
            , ref string step_duration
            , ref string reason)
        {

            StringBuilder sbStep = new StringBuilder();
            StringBuilder sbStepAssign = new StringBuilder();
            StringBuilder sbStartDate = new StringBuilder();
            StringBuilder sbEndDate = new StringBuilder();
            StringBuilder sbStepDuration = new StringBuilder();
            StringBuilder sbReason = new StringBuilder();

            foreach (var item_allStep in temp_dashboard)
            {
                var tempMockup = allStepMockup.Where(m => m.STEP_MOCKUP_ID == item_allStep.CURRENT_STEP_ID).FirstOrDefault();
                if (tempMockup != null)
                {

                    sbStep.AppendLine(tempMockup.STEP_MOCKUP_NAME);

                    if (CNService.GetUserName(item_allStep.CURRENT_USER_ID, context) != "")
                        sbStepAssign.AppendLine(CNService.GetUserName(item_allStep.CURRENT_USER_ID, context));
                    else
                        sbStepAssign.AppendLine(msg);
                }

                sbStartDate.AppendLine(item_allStep.CREATE_DATE_PROCESS.Value.ToString("dd/MM/yyyy"));

                var startdate = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(item_allStep.MOCKUP_SUB_ID, context).CREATE_DATE;
                var finishdate = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(item_allStep.MOCKUP_SUB_ID, context).UPDATE_DATE;
                DateTime realFinishdate = DateTime.Now;
                if (item_allStep.IS_END == "X")
                {
                    realFinishdate = finishdate;
                    sbEndDate.AppendLine(finishdate.ToString("dd/MM/yyyy"));
                }
                else
                {
                    sbEndDate.AppendLine("");
                }

                if (item_allStep.CREATE_DATE_PROCESS != null)
                {
                    DateTime? dtReceiveWf = item_allStep.CREATE_DATE_PROCESS;
                    DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item_allStep.DURATION.Value));
                    var duration = item_allStep.DURATION.Value;

                    if (!string.IsNullOrEmpty(item_allStep.IS_STEP_DURATION_EXTEND))
                    {
                        dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item_allStep.DURATION_EXTEND.Value));
                        duration = item_allStep.DURATION_EXTEND.Value;
                    }

                    if (item_allStep.IS_END == "X")
                    {
                        total_day += (CNService.GetWorkingDays(startdate, realFinishdate) + 1);
                        sbStepDuration.AppendLine((CNService.GetWorkingDays(startdate, realFinishdate) + 1) + " [" + duration + "]");
                    }
                    else
                    {
                        sbStepDuration.AppendLine(" [" + duration + "]");
                    }
                }

                if (item_allStep.REASON_ID > 0)
                    sbReason.AppendLine(CNService.getReason(item_allStep.REASON_ID, context));
                else
                    sbReason.AppendLine("");


            }

            step = sbStep.ToString();
            step_assign = sbStepAssign.ToString();
            start_date = sbStartDate.ToString();
            end_date = sbEndDate.ToString();
            step_duration = sbStepDuration.ToString();
            reason = sbReason.ToString();

        }

        private static string GetAllStepColumnArtwork(List<V_ART_WF_DASHBOARD_ARTWORK> temp_dashboard, List<ART_M_STEP_ARTWORK> allStepMockup, string msg, ARTWORKEntities context, ref int total_day, ref List<DateTime> start_date, ref List<DateTime> end_date)
        {
            var tableAllStep = "";

            if (temp_dashboard.Count > 0)
                tableAllStep += "<table class='cls_table_in_table' style='min-width:1200px;'>";

            string stepDisplayTxt = "";

            foreach (var item_allStep in temp_dashboard)
            {
                tableAllStep += "<tr>";
                var tempMockup = allStepMockup.Where(m => m.STEP_ARTWORK_ID == item_allStep.CURRENT_STEP_ID).FirstOrDefault();
                if (tempMockup != null)
                {
                    stepDisplayTxt = tempMockup.STEP_ARTWORK_NAME;

                    if (item_allStep.IS_TERMINATE == "X" || (item_allStep.IS_END == "X" && !String.IsNullOrEmpty(item_allStep.REMARK_KILLPROCESS)))
                    {
                        stepDisplayTxt = tempMockup.STEP_ARTWORK_NAME + " [Terminated]";
                    }

                    tableAllStep += "<td style='max-width:330px;width:330px;min-width:330px;white-space:initial;'>";
                    tableAllStep += "<a target='_blank' href='" + System.Configuration.ConfigurationManager.AppSettings["suburl"] + "/TaskFormArtwork/" + item_allStep.ARTWORK_SUB_ID + "'>" + stepDisplayTxt + "</a></td>";

                    if (CNService.GetUserName(item_allStep.CURRENT_USER_ID, context) != "")
                        tableAllStep += "<td style='max-width:270px;width:270px;min-width:270px;white-space:initial;'>" + CNService.GetUserName(item_allStep.CURRENT_USER_ID, context) + "</td>";
                    else
                        tableAllStep += "<td style='max-width:270px;width:270px;min-width:270px;white-space:initial;'>" + msg + " </td>";
                }

                tableAllStep += "<td style='max-width:80px;width:80px;min-width:80px;white-space:initial;'>" + item_allStep.CREATE_DATE_PROCESS.Value.ToString("dd/MM/yyyy") + "</td>";

                var startdate = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(item_allStep.ARTWORK_SUB_ID, context).CREATE_DATE;
                var finishdate = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(item_allStep.ARTWORK_SUB_ID, context).UPDATE_DATE;
                DateTime realFinishdate = DateTime.Now;
                if (item_allStep.IS_END == "X")
                {
                    realFinishdate = finishdate;
                    tableAllStep += "<td style='max-width:80px;width:80px;min-width:80px;white-space:initial;'>" + finishdate.ToString("dd/MM/yyyy") + "</td>";
                }
                else
                    tableAllStep += "<td style='max-width:80px;width:80px;min-width:80px;white-space:initial;'></td>";

                if (item_allStep.CREATE_DATE_PROCESS != null)
                {
                    DateTime? dtReceiveWf = item_allStep.CREATE_DATE_PROCESS;
                    DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item_allStep.DURATION.Value));
                    var duration = item_allStep.DURATION.Value;

                    if (!string.IsNullOrEmpty(item_allStep.IS_STEP_DURATION_EXTEND))
                    {
                        dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item_allStep.DURATION_EXTEND.Value));
                        duration = item_allStep.DURATION_EXTEND.Value;
                    }

                    if (item_allStep.IS_END == "X")
                    {
                        total_day += (CNService.GetWorkingDays(startdate, realFinishdate) + 1);
                        start_date.Add(startdate);
                        end_date.Add(realFinishdate);
                        tableAllStep += "<td style='text-align:right;max-width:90px;width:90px;min-width:90px;white-space:initial;'>" + (CNService.GetWorkingDays(startdate, realFinishdate) + 1) + " [" + duration + "]" + "</td>";
                    }
                    else
                    {
                        tableAllStep += "<td style='text-align:right;max-width:90px;width:90px;min-width:90px;white-space:initial;'>" + "" + " [" + duration + "]" + "</td>";
                    }
                }

                if (item_allStep.REASON_ID > 0)
                    tableAllStep += "<td style='max-width:170px;width:170px;min-width:170px;white-space:initial;'>" + CNService.getReason(item_allStep.REASON_ID, context) + " </td>";
                else
                    tableAllStep += "<td style='max-width:170px;width:170px;min-width:170px;white-space:initial;'></td>";

                tableAllStep += "</tr>";
            }

            return tableAllStep;
        }

        private static void GetAllStepColumnArtwork_Excel(List<V_ART_WF_DASHBOARD_ARTWORK> temp_dashboard, List<ART_M_STEP_ARTWORK> allStepMockup, string msg, ARTWORKEntities context, ref int total_day
            , ref string step
            , ref string step_assign
            , ref string start_date
            , ref string end_date
            , ref string step_duration
            , ref string reason)
        {
            //var tableAllStep = "";

            StringBuilder sbStep = new StringBuilder();
            StringBuilder sbStepAssign = new StringBuilder();
            StringBuilder sbStartDate = new StringBuilder();
            StringBuilder sbEndDate = new StringBuilder();
            StringBuilder sbStepDuration = new StringBuilder();
            StringBuilder sbReason = new StringBuilder();
            string stepDisplayTxt = "";

            foreach (var item_allStep in temp_dashboard)
            {
                var tempMockup = allStepMockup.Where(m => m.STEP_ARTWORK_ID == item_allStep.CURRENT_STEP_ID).FirstOrDefault();
                if (tempMockup != null)
                {
                    stepDisplayTxt = tempMockup.STEP_ARTWORK_NAME;
                    if (!String.IsNullOrEmpty(item_allStep.REMARK_KILLPROCESS))
                    {
                        stepDisplayTxt = tempMockup.STEP_ARTWORK_NAME + " [Terminated]";
                    }

                    sbStep.AppendLine(stepDisplayTxt);

                    if (CNService.GetUserName(item_allStep.CURRENT_USER_ID, context) != "")
                        sbStepAssign.AppendLine(CNService.GetUserName(item_allStep.CURRENT_USER_ID, context));
                    else
                        sbStepAssign.AppendLine(msg);
                }

                sbStartDate.AppendLine(item_allStep.CREATE_DATE_PROCESS.Value.ToString("dd/MM/yyyy"));

                var startdate = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(item_allStep.ARTWORK_SUB_ID, context).CREATE_DATE;
                var finishdate = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(item_allStep.ARTWORK_SUB_ID, context).UPDATE_DATE;
                DateTime realFinishdate = DateTime.Now;
                if (item_allStep.IS_END == "X")
                {
                    realFinishdate = finishdate;
                    sbStartDate.AppendLine(finishdate.ToString("dd/MM/yyyy"));
                }
                else
                {
                    sbStartDate.AppendLine("");
                }

                if (item_allStep.CREATE_DATE_PROCESS != null)
                {
                    DateTime? dtReceiveWf = item_allStep.CREATE_DATE_PROCESS;
                    DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item_allStep.DURATION.Value));
                    var duration = item_allStep.DURATION.Value;

                    if (!string.IsNullOrEmpty(item_allStep.IS_STEP_DURATION_EXTEND))
                    {
                        dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(item_allStep.DURATION_EXTEND.Value));
                        duration = item_allStep.DURATION_EXTEND.Value;
                    }

                    if (item_allStep.IS_END == "X")
                    {
                        total_day += (CNService.GetWorkingDays(startdate, realFinishdate) + 1);
                        sbStepDuration.AppendLine((CNService.GetWorkingDays(startdate, realFinishdate) + 1) + " [" + duration + "]");
                    }
                    else
                    {
                        sbStepDuration.AppendLine(" [" + duration + "]");
                    }
                }

                if (item_allStep.REASON_ID > 0)
                    sbReason.AppendLine(CNService.getReason(item_allStep.REASON_ID, context));
                else
                    sbReason.AppendLine("");


            }

            step = sbStep.ToString();
            step_assign = sbStepAssign.ToString();
            start_date = sbStartDate.ToString();
            end_date = sbEndDate.ToString();
            step_duration = sbStepDuration.ToString();
            reason = sbReason.ToString();


        }

        public static TRACKING_REPORT_RESULT GetEndToEndReport_File(TRACKING_REPORT_REQUEST param)
        {


            TRACKING_REPORT_RESULT Results = new TRACKING_REPORT_RESULT();
            List<TRACKING_REPORT> data = new List<TRACKING_REPORT>();
            try
            {
                string workflowNo = "";

                if (param.data.WORKFLOW_NUMBER.StartsWith("A"))
                {
                    workflowNo = param.data.WORKFLOW_NUMBER;
                    data = GetAWFile(workflowNo);
                }
                else
                {
                    workflowNo = param.data.WORKFLOW_NUMBER;
                    data = GetMOFile(workflowNo);
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

        private static List<TRACKING_REPORT> GetMOFile(string workflowNo)
        {
            List<TRACKING_REPORT> data = new List<TRACKING_REPORT>();
            TRACKING_REPORT attach = new TRACKING_REPORT();
            var parentMOFolder = ConfigurationManager.AppSettings["MockUpNodeID"];
            var folderDieline = ConfigurationManager.AppSettings["MockupFolderNameDieline"];
            var token = CWSService.getAuthToken();
            Node nodeParentSPMO = CWSService.getNodeByName(Convert.ToInt64(parentMOFolder), workflowNo, token);

            if (nodeParentSPMO != null)
            {
                Node nodeMO = CWSService.getNodeByName(nodeParentSPMO.ID, folderDieline, token);

                if (nodeMO != null)
                {
                    Node[] nodeSPMOFiles = CWSService.getAllNodeInFolder(nodeMO.ID, token);
                    if (nodeSPMOFiles != null && nodeSPMOFiles.Count() > 0)
                    {
                        foreach (Node iMOFile in nodeSPMOFiles)
                        {
                            attach = new TRACKING_REPORT()
                            {
                                FILE_NAME = iMOFile.Name,
                                CREATED_DATE = iMOFile.CreateDate,
                                TITLE = "Final Die line",
                                NODE_ID_TXT = EncryptionService.EncryptAndUrlEncode(iMOFile.ID.ToString())

                            };
                            data.Add(attach);
                        }
                    }
                }
            }


            return data;
        }

        //private static List<TRACKING_REPORT> GetAWFile_BAK(string workflowNo)
        //{

        //    List<TRACKING_REPORT> data = new List<TRACKING_REPORT>();
        //    TRACKING_REPORT attach = new TRACKING_REPORT();

        //    var parentSecondaryPackagingID = ConfigurationManager.AppSettings["SecondaryPackagingNodeID"];
        //    var folderSPAW = ConfigurationManager.AppSettings["SecondaryPkgArtworkFolderName"];

        //    using (var context = new ARTWORKEntities())
        //    {
        //        using (CNService.IsolationLevel(context))
        //        {
        //            var itemID = context.ART_WF_ARTWORK_REQUEST_ITEM
        //                        .Where(i => i.REQUEST_ITEM_NO == workflowNo)
        //                        .Select(s => s.ARTWORK_ITEM_ID).FirstOrDefault();
        //            var subIDs = context.ART_WF_ARTWORK_PROCESS
        //                            .Where(d => d.ARTWORK_ITEM_ID == itemID)
        //                            .Select(i => i.ARTWORK_SUB_ID).ToList();

        //            var soDetails = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
        //                             where subIDs.Contains(s.ARTWORK_SUB_ID)
        //                             select s).ToList();

        //            if (soDetails != null && soDetails.Count > 0)
        //            {
        //                string matDesc = "";
        //                foreach (ART_WF_ARTWORK_PROCESS_SO_DETAIL iComponent in soDetails)
        //                {

        //                    var component = context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
        //                                        .Where(c => c.PO_COMPLETE_SO_ITEM_COMPONENT_ID == iComponent.BOM_ID)
        //                                        .FirstOrDefault();

        //                    decimal itemNO = 0;
        //                    itemNO = Convert.ToDecimal(iComponent.SALES_ORDER_ITEM);
        //                    var soHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
        //                                    where h.SALES_ORDER_NO == iComponent.SALES_ORDER_NO
        //                                    select h).FirstOrDefault();

        //                    var soItem = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
        //                                  where i.PO_COMPLETE_SO_HEADER_ID == soHeader.PO_COMPLETE_SO_HEADER_ID
        //                                    && i.ITEM == itemNO
        //                                  select i).FirstOrDefault();

        //                    var soComponent = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
        //                                       where i.PO_COMPLETE_SO_ITEM_ID == soItem.PO_COMPLETE_SO_ITEM_ID
        //                                            && i.COMPONENT_MATERIAL.StartsWith("5")
        //                                       select i).ToList();

        //                    if (soComponent != null)
        //                    {
        //                        foreach (var iComp in soComponent)
        //                        {


        //                            matDesc = iComp.COMPONENT_MATERIAL + " - " + iComp.DECRIPTION;

        //                            Node nodeParentSPAW = CWSService.getNodeByName(Convert.ToInt64(parentSecondaryPackagingID), matDesc);
        //                            if (nodeParentSPAW != null)
        //                            {
        //                                Node node = CWSService.getNodeByName(nodeParentSPAW.ID * (-1), folderSPAW);
        //                                if (node != null)
        //                                {
        //                                    Node[] nodeSPAWFiles = CWSService.getAllNodeInFolder(node.ID);
        //                                    if (nodeSPAWFiles != null && nodeSPAWFiles.Count() > 0)
        //                                    {
        //                                        foreach (Node iPOFile in nodeSPAWFiles)
        //                                        {
        //                                            attach = new TRACKING_REPORT()
        //                                            {
        //                                                FILE_NAME = iPOFile.Name,
        //                                                CREATED_DATE = iPOFile.CreateDate,
        //                                                TITLE = "Final Artwork",
        //                                                NODE_ID_TXT = EncryptionService.Encrypt(iPOFile.ID.ToString())

        //                                            };
        //                                            data.Add(attach);
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return data;
        //}

        private static List<TRACKING_REPORT> GetAWFile(string workflowNo)
        {
            List<TRACKING_REPORT> data = new List<TRACKING_REPORT>();
            TRACKING_REPORT attach = new TRACKING_REPORT();

            var parentSecondaryPackagingID = ConfigurationManager.AppSettings["SecondaryPackagingNodeID"];
            var folderSPAW = ConfigurationManager.AppSettings["SecondaryPkgArtworkFolderName"];
            var token = CWSService.getAuthToken();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var itemID = context.ART_WF_ARTWORK_REQUEST_ITEM
                                .Where(i => i.REQUEST_ITEM_NO == workflowNo)
                                .Select(s => s.ARTWORK_ITEM_ID).FirstOrDefault();

                    var subIDs = context.ART_WF_ARTWORK_PROCESS
                                    .Where(d => d.ARTWORK_ITEM_ID == itemID)
                                    .Select(i => i.ARTWORK_SUB_ID).ToList();

                    var stepPA = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_PA").FirstOrDefault();

                    var process = (from p in context.ART_WF_ARTWORK_PROCESS
                                   where subIDs.Contains(p.ARTWORK_SUB_ID)
                                   && p.CURRENT_STEP_ID == stepPA.STEP_ARTWORK_ID
                                   select p).OrderByDescending(o => o.ARTWORK_SUB_ID).FirstOrDefault();

                    var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                     where p.ARTWORK_SUB_ID == process.ARTWORK_SUB_ID
                                     select p).FirstOrDefault();

                    if (!String.IsNullOrEmpty(processPA.MATERIAL_NO))
                    {
                        var mat = (from m in context.XECM_M_PRODUCT5
                                   where m.PRODUCT_CODE == processPA.MATERIAL_NO
                                   select m).FirstOrDefault();

                        string matDesc = "";
                        matDesc = processPA.MATERIAL_NO;

                        if (mat != null)
                        {
                            matDesc = processPA.MATERIAL_NO + " - " + mat.PRODUCT_DESCRIPTION;
                        }

                        Node nodeParentSPAW = CWSService.getNodeByName(Convert.ToInt64(parentSecondaryPackagingID), matDesc, token);
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
                                        attach = new TRACKING_REPORT()
                                        {
                                            FILE_NAME = iPOFile.Name,
                                            CREATED_DATE = iPOFile.CreateDate,
                                            TITLE = "Final Artwork",
                                            NODE_ID_TXT = EncryptionService.EncryptAndUrlEncode(iPOFile.ID.ToString())

                                        };
                                        data.Add(attach);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return data;
        }
    }
}


