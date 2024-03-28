using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BLL.Services;
using DAL;
using DAL.Model;
using BLL.Enums;
using System.Data.SqlClient;

namespace BLL.Helpers
{
    public class DashboardHelper
    {
        public static V_ART_WF_DASHBOARD_RESULT GetIncomingMockup(V_ART_WF_DASHBOARD_REQUEST param)
        {
            V_ART_WF_DASHBOARD_RESULT Results = new V_ART_WF_DASHBOARD_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var USER_ID = param.data.CURRENT_USER_ID;
                        var getByCreateDateFrom = DateTime.Now;
                        var getByCreateDateTo = DateTime.Now;

                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            getByCreateDateFrom = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_FROM);
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            getByCreateDateTo = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_TO);
                        var qlist = context.Database.SqlQuery<V_ART_WF_DASHBOARD_2>("sp_ART_WF_DASHBOARD_MOCKUP @STEP, @From, @To", 
                            new SqlParameter("@STEP", SEND_PG),new SqlParameter("@From", getByCreateDateFrom), new SqlParameter("@To", getByCreateDateTo)).ToList();
                        var q = qlist.AsQueryable().Where(m => m.CURRENT_USER_ID == null).ToList();
                        //var q = (from m in context.V_ART_WF_DASHBOARD
                        //         where string.IsNullOrEmpty(m.IS_END)
                        //         && m.CURRENT_USER_ID == null
                        //         && m.CURRENT_STEP_ID == SEND_PG
                        //         select new V_ART_WF_DASHBOARD_2()
                        //         {
                        //             CHECK_LIST_ID = m.CHECK_LIST_ID,
                        //             CHECK_LIST_NO = m.CHECK_LIST_NO,
                        //             TRF_REF_NO = m.TRF_REF_NO,
                        //             TYPE_OF_PRODUCT_ID = m.TYPE_OF_PRODUCT_ID,
                        //             COMPANY_ID = m.COMPANY_ID,
                        //             SOLD_TO_ID = m.SOLD_TO_ID,
                        //             SHIP_TO_ID = m.SHIP_TO_ID,
                        //             RD_PERSON_ID = m.RD_PERSON_ID,
                        //             PRIMARY_TYPE_ID = m.PRIMARY_TYPE_ID,
                        //             PRIMARY_TYPE_OTHER = m.PRIMARY_TYPE_OTHER,
                        //             PRIMARY_SIZE_ID = m.PRIMARY_SIZE_ID,
                        //             PRIMARY_SIZE_OTHER = m.PRIMARY_SIZE_OTHER,
                        //             CONTAINER_TYPE_ID = m.CONTAINER_TYPE_ID,
                        //             CONTAINER_TYPE_OTHER = m.CONTAINER_TYPE_OTHER,
                        //             LID_TYPE_ID = m.LID_TYPE_ID,
                        //             LID_TYPE_OTHER = m.LID_TYPE_OTHER,
                        //             PACKING_STYLE_ID = m.PACKING_STYLE_ID,
                        //             PACKING_STYLE_OTHER = m.PACKING_STYLE_OTHER,
                        //             PACK_SIZE_ID = m.PACK_SIZE_ID,
                        //             PACK_SIZE_OTHER = m.PACK_SIZE_OTHER,
                        //             PRIMARY_MATERIAL = m.PRIMARY_MATERIAL,
                        //             REQUEST_DELIVERY_DATE = m.REQUEST_DELIVERY_DATE,
                        //             OTHER_REQUESTS = m.OTHER_REQUESTS,
                        //             BRAND_ID = m.BRAND_ID,
                        //             BRAND_OTHER = m.BRAND_OTHER,
                        //             BRAND_OEM_ID = m.BRAND_OEM_ID,
                        //             BRAND_OEM_OTHER = m.BRAND_OEM_OTHER,
                        //             REQUEST_PHYSICAL_MOCKUP = m.REQUEST_PHYSICAL_MOCKUP,
                        //             CREATOR_ID = m.CREATOR_ID,
                        //             MOCKUP_NO = m.MOCKUP_NO,
                        //             MOCKUP_STATUS = m.MOCKUP_STATUS,
                        //             PACKING_TYPE_ID = m.PACKING_TYPE_ID,
                        //             PRINT_SYSTEM_ID = m.PRINT_SYSTEM_ID,
                        //             NUMBER_OF_COLOR_ID = m.NUMBER_OF_COLOR_ID,
                        //             BOX_COLOR_ID = m.BOX_COLOR_ID,
                        //             COATING_ID = m.COATING_ID,
                        //             PURPOSE_OF = m.PURPOSE_OF,
                        //             STYLE_ID = m.STYLE_ID,
                        //             PACKING_TYPE_OTHER = m.PACKING_TYPE_OTHER,
                        //             PRINT_SYSTEM_OTHER = m.PRINT_SYSTEM_OTHER,
                        //             NUMBER_OF_COLOR_OTHER = m.NUMBER_OF_COLOR_OTHER,
                        //             BOX_COLOR_OTHER = m.BOX_COLOR_OTHER,
                        //             STYLE_OTHER = m.STYLE_OTHER,
                        //             COATING_OTHER = m.COATING_OTHER,
                        //             MOCKUP_ID = m.MOCKUP_ID,
                        //             MOCKUP_SUB_ID = m.MOCKUP_SUB_ID,
                        //             CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                        //             CURRENT_ROLE_ID = m.CURRENT_ROLE_ID,
                        //             CURRENT_USER_ID = m.CURRENT_USER_ID,
                        //             REMARK = m.REMARK,
                        //             CREATE_DATE_PROCESS = m.CREATE_DATE_PROCESS,
                        //             CREATE_DATE_CHECK_LIST = m.CREATE_DATE_CHECK_LIST,
                        //             CREATE_BY_PROCESS = m.CREATE_BY_PROCESS,
                        //             CREATE_BY_CHECK_LIST = m.CREATE_BY_CHECK_LIST,
                        //             IS_END = m.IS_END,
                        //             PARENT_MOCKUP_SUB_ID = m.PARENT_MOCKUP_SUB_ID,
                        //             NODE_ID = m.NODE_ID,
                        //             CHECK_LIST_FOR_DESIGN = m.CHECK_LIST_FOR_DESIGN,
                        //             REQUEST_FOR_DIE_LINE = m.REQUEST_FOR_DIE_LINE,
                        //             PROJECT_NAME = m.PROJECT_NAME,
                        //             REVIEWER = m.REVIEWER,
                        //             TWO_P_ID = m.TWO_P_ID,
                        //             THREE_P_ID = m.THREE_P_ID,
                        //             REF_PRODUCT_CODE = m.REF_PRODUCT_CODE,
                        //             CURRENT_VENDOR_ID = m.CURRENT_VENDOR_ID,
                        //             CURRENT_CUSTOMER_ID = m.CURRENT_CUSTOMER_ID,
                        //             NOMINATED_SPEC = m.NOMINATED_SPEC,
                        //             NOMINATED_CONTAINER_VENDOR = m.NOMINATED_CONTAINER_VENDOR,
                        //             CUSTOMER_OTHER_ID = m.CUSTOMER_OTHER_ID,
                        //             SOLD_TO_DISPLAY_TXT = m.SOLD_TO_DISPLAY_TXT,
                        //             SHIP_TO_DISPLAY_TXT = m.SHIP_TO_DISPLAY_TXT,
                        //             BRAND_DISPLAY_TXT = m.BRAND_DISPLAY_TXT,
                        //             PACKING_TYPE_DISPLAY_TXT = m.PACKING_TYPE_DISPLAY_TXT,
                        //             CREATE_BY_CHECK_LIST_TITLE = m.CREATE_BY_CHECK_LIST_TITLE,
                        //             CREATE_BY_CHECK_LIST_FIRST_NAME = m.CREATE_BY_CHECK_LIST_FIRST_NAME,
                        //             CREATE_BY_CHECK_LIST_LAST_NAME = m.CREATE_BY_CHECK_LIST_LAST_NAME,
                        //             CREATE_BY_PROCESS_TITLE = m.CREATE_BY_PROCESS_TITLE,
                        //             CREATE_BY_PROCESS_FIRST_NAME = m.CREATE_BY_PROCESS_FIRST_NAME,
                        //             CREATE_BY_PROCESS_LAST_NAME = m.CREATE_BY_PROCESS_LAST_NAME,
                        //             CURRENT_STEP_DISPLAY_TXT = m.CURRENT_STEP_DISPLAY_TXT,
                        //             REFERENCE_REQUEST_ID = m.REFERENCE_REQUEST_ID,
                        //             REFERENCE_REQUEST_NO = m.REFERENCE_REQUEST_NO,
                        //             REFERENCE_REQUEST_TYPE = m.REFERENCE_REQUEST_TYPE,
                        //             REASON_ID = m.REASON_ID,
                        //             DURATION = m.DURATION,
                        //             DURATION_EXTEND = m.DURATION_EXTEND,
                        //             STEP_MOCKUP_DESCRIPTION = m.STEP_MOCKUP_DESCRIPTION,
                        //             IS_TERMINATE = m.IS_TERMINATE,
                        //             REMARK_TERMINATE = m.REMARK_TERMINATE,
                        //             IS_DELEGATE = m.IS_DELEGATE,
                        //             STEP_MOCKUP_CODE = m.STEP_MOCKUP_CODE,
                        //             ROLE_ID_RESPONSE = m.ROLE_ID_RESPONSE,
                        //             REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                        //             IS_STEP_DURATION_EXTEND = m.IS_STEP_DURATION_EXTEND,
                        //         });

                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            q = q.AsQueryable().Where(m => Convert.ToDateTime(m.CREATE_DATE_PROCESS).Date >= getByCreateDateFrom.Date).ToList();
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            q = q.AsQueryable().Where(m => Convert.ToDateTime(m.CREATE_DATE_PROCESS).Date <= getByCreateDateTo.Date).ToList();

                        var temp = q.ToList();

                        //Results.data = MapperServices.V_ART_WF_DASHBOARD(temp);
                        Results.data = temp;
                    }
                }

                if (param != null)
                {
                    Results.draw = param.draw;
                }

                Results = BindDataIncomingMockup(Results);

                Results.recordsFiltered = Results.data.ToList().Count;
                Results.recordsTotal = Results.data.ToList().Count;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        private static V_ART_WF_DASHBOARD_RESULT BindDataIncomingMockup(V_ART_WF_DASHBOARD_RESULT Results)
        {
            if (Results.data.Count > 0)
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        List<SAP_M_COUNTRY> list_country = new List<SAP_M_COUNTRY>();
                        list_country = SAP_M_COUNTRY_SERVICE.GetAll(context);

                        var listChecklistId = Results.data.Select(m => m.CHECK_LIST_ID).Distinct().ToList();
                        var listPRIMARY_SIZE_ID = Results.data.Select(m => m.PRIMARY_SIZE_ID).Distinct().ToList();
                        var listTHREE_P_ID = Results.data.Select(m => m.THREE_P_ID).Distinct().ToList();
                        var listAllChecklistCountry = (from m in context.ART_WF_MOCKUP_CHECK_LIST_COUNTRY where listChecklistId.Contains(m.CHECK_LIST_ID) select m).ToList();
                        var listAllChecklistProduct = (from m in context.ART_WF_MOCKUP_CHECK_LIST_PRODUCT where listChecklistId.Contains(m.CHECK_LIST_ID) select m).ToList();
                        var listAllPRIMARY_SIZE = (from m in context.SAP_M_CHARACTERISTIC where listPRIMARY_SIZE_ID.Contains(m.CHARACTERISTIC_ID) select m).ToList();
                        var listAllTHREE_P = (from m in context.SAP_M_3P where listTHREE_P_ID.Contains(m.THREE_P_ID) select m).ToList();

                        for (int i = 0; i < Results.data.Count; i++)
                        {
                            //ART_WF_MOCKUP_CHECK_LIST_COUNTRY chklist_country = new ART_WF_MOCKUP_CHECK_LIST_COUNTRY();
                            //chklist_country.CHECK_LIST_ID = Results.data[i].CHECK_LIST_ID;
                            //var tempCountry = MapperServices.ART_WF_MOCKUP_CHECK_LIST_COUNTRY(ART_WF_MOCKUP_CHECK_LIST_COUNTRY_SERVICE.GetByItem(chklist_country, context));
                            var tempCountry = listAllChecklistCountry.Where(m => m.CHECK_LIST_ID == Results.data[i].CHECK_LIST_ID).ToList();

                            var country = tempCountry.Join(
                                list_country,
                                lc => lc.COUNTRY_ID,
                                c => c.COUNTRY_ID,
                                (lc, c) => new
                                {
                                    COUNTRY_ID = lc.COUNTRY_ID,
                                    COUNTRY_CODE = c.COUNTRY_CODE
                                }
                            );
                            Results.data[i].COUNTRY_CODE_SET = country.Aggregate("", (a, b) => a + ((a.Length > 0 && b.COUNTRY_CODE != null && b.COUNTRY_CODE.Length > 0) ? ", " : "") + b.COUNTRY_CODE);

                            //ART_WF_MOCKUP_CHECK_LIST_PRODUCT product = new ART_WF_MOCKUP_CHECK_LIST_PRODUCT();
                            //product.CHECK_LIST_ID = Results.data[i].CHECK_LIST_ID;
                            //var tempProduct = MapperServices.ART_WF_MOCKUP_CHECK_LIST_PRODUCT(ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(product, context));
                            var tempProduct = listAllChecklistProduct.Where(m => m.CHECK_LIST_ID == Results.data[i].CHECK_LIST_ID).ToList();

                            Results.data[i].PRODUCT_CODE_SET = tempProduct.Aggregate("", (a, b) => a + ((a.Length > 0 && b.PRODUCT_CODE != null && b.PRODUCT_CODE.Length > 0) ? ", " : "") + b.PRODUCT_CODE);
                            if (Results.data[i].MOCKUP_NO.Substring(0, 1) == "M")
                            {
                                Results.data[i].WORKFLOW_TYPE = "Mockup";
                            }
                            else if (Results.data[i].REQUEST_FOR_DIE_LINE == "1")
                            {
                                Results.data[i].WORKFLOW_TYPE = "Request Die line";
                            }

                            Results.data[i].CREATE_BY_CHECK_LIST_DISPLAY_TXT = Results.data[i].CREATE_BY_CHECK_LIST_TITLE + " " + Results.data[i].CREATE_BY_CHECK_LIST_FIRST_NAME + " " + Results.data[i].CREATE_BY_CHECK_LIST_LAST_NAME;
                            Results.data[i].CREATE_BY_PROCESS_DISPLAY_TXT = Results.data[i].CREATE_BY_PROCESS_TITLE + " " + Results.data[i].CREATE_BY_PROCESS_FIRST_NAME + " " + Results.data[i].CREATE_BY_PROCESS_LAST_NAME;

                            if (Results.data[i].PRIMARY_SIZE_ID > 0)
                            {
                                //var PRIMARY_SIZE = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(Results.data[i].PRIMARY_SIZE_ID, context);
                                var PRIMARY_SIZE = listAllPRIMARY_SIZE.Where(m => m.CHARACTERISTIC_ID == Results.data[i].PRIMARY_SIZE_ID).FirstOrDefault();
                                if (PRIMARY_SIZE != null)
                                    Results.data[i].PRIMARY_SIZE_DISPLAY_TXT = PRIMARY_SIZE.DESCRIPTION;
                            }
                            else
                            {
                                //var temp = SAP_M_3P_SERVICE.GetByTHREE_P_ID(Results.data[i].THREE_P_ID, context);
                                var temp = listAllTHREE_P.Where(m => m.THREE_P_ID == Results.data[i].THREE_P_ID).FirstOrDefault();
                                if (temp != null)
                                    Results.data[i].PRIMARY_SIZE_DISPLAY_TXT = temp.PRIMARY_SIZE_DESCRIPTION;
                            }

                            if (string.IsNullOrEmpty(Results.data[i].PRIMARY_SIZE_DISPLAY_TXT))
                            {
                                //var temp = ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_PRODUCT() { CHECK_LIST_ID = Results.data[i].CHECK_LIST_ID }, context).FirstOrDefault();
                                var temp = listAllChecklistProduct.Where(m => m.CHECK_LIST_ID == Results.data[i].CHECK_LIST_ID).FirstOrDefault();
                                if (temp != null) Results.data[i].PRIMARY_SIZE_DISPLAY_TXT = temp.PRIMARY_SIZE;
                            }
                        }
                    }
                }
            }

            return Results;
        }

        public static V_ART_WF_DASHBOARD_RESULT GetInbox(V_ART_WF_DASHBOARD_REQUEST param)
        {
            //test interface
            //CNService.getSchSendMail("");
            //CNService.buildinterface();
            V_ART_WF_DASHBOARD_RESULT Results = new V_ART_WF_DASHBOARD_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var getByCreateDateFrom = DateTime.Now;
                        var getByCreateDateTo = DateTime.Now;

                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            getByCreateDateFrom = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_FROM);
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            getByCreateDateTo = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_TO);

                        var allStepMockup = ART_M_STEP_MOCKUP_SERVICE.GetAll(context);
                        var allStepArtwork = ART_M_STEP_ARTWORK_SERVICE.GetAll(context);
                        var listUserCompany = ART_M_USER_COMPANY_SERVICE.GetByItem(new ART_M_USER_COMPANY() { USER_ID = Convert.ToInt32(param.data.CURRENT_USER_ID) }, context);
                        var listUserTypeofProduct = ART_M_USER_TYPE_OF_PRODUCT_SERVICE.GetByItem(new ART_M_USER_TYPE_OF_PRODUCT() { USER_ID = Convert.ToInt32(param.data.CURRENT_USER_ID) }, context);

                        var SEND_PG_ARTWORK = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_PG").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_PA = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_PA").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_VN_PO = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_VN_PO").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_PP = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_PP").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_PG = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_PG").FirstOrDefault().STEP_MOCKUP_ID;

                        var SEND_QC = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_QC").FirstOrDefault().STEP_ARTWORK_ID;   //#TSK-1511 #SR-70695 by aof in 09/2022
                        var SEND_RD = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_RD").FirstOrDefault().STEP_ARTWORK_ID;   //#TSK-1511 #SR-70695 by aof in 09/2022

                        var listRole = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { USER_ID = Convert.ToInt32(param.data.CURRENT_USER_ID) }, context);
                        var listRole2 = listRole.Select(m => m.ROLE_ID);



                        //var qMockup = CNService.BuildIncomingMockup(USER_ID,
                        //    SEND_PG, getByCreateDateFrom, getByCreateDateTo, context);
                        var qMockup = context.Database.SqlQuery<V_ART_WF_DASHBOARD_2>("sp_ART_WF_DASHBOARD @Step, @USER, @From, @To",
                              new SqlParameter("@Step", "1"),
                              new SqlParameter("@USER", (int?)param.data.CURRENT_USER_ID),
                              new SqlParameter("@From", getByCreateDateFrom), new SqlParameter("@To", getByCreateDateTo)).ToList();
                        //var qMockup = (from m in context.V_ART_WF_DASHBOARD
                        //               where string.IsNullOrEmpty(m.IS_END)
                        //               select new V_ART_WF_DASHBOARD_2()
                        //               {
                        //                   CHECK_LIST_ID = m.CHECK_LIST_ID,
                        //                   CHECK_LIST_NO = m.CHECK_LIST_NO,
                        //                   TRF_REF_NO = m.TRF_REF_NO,
                        //                   TYPE_OF_PRODUCT_ID = m.TYPE_OF_PRODUCT_ID,
                        //                   COMPANY_ID = m.COMPANY_ID,
                        //                   SOLD_TO_ID = m.SOLD_TO_ID,
                        //                   SHIP_TO_ID = m.SHIP_TO_ID,
                        //                   RD_PERSON_ID = m.RD_PERSON_ID,
                        //                   PRIMARY_TYPE_ID = m.PRIMARY_TYPE_ID,
                        //                   PRIMARY_TYPE_OTHER = m.PRIMARY_TYPE_OTHER,
                        //                   PRIMARY_SIZE_ID = m.PRIMARY_SIZE_ID,
                        //                   PRIMARY_SIZE_OTHER = m.PRIMARY_SIZE_OTHER,
                        //                   CONTAINER_TYPE_ID = m.CONTAINER_TYPE_ID,
                        //                   CONTAINER_TYPE_OTHER = m.CONTAINER_TYPE_OTHER,
                        //                   LID_TYPE_ID = m.LID_TYPE_ID,
                        //                   LID_TYPE_OTHER = m.LID_TYPE_OTHER,
                        //                   PACKING_STYLE_ID = m.PACKING_STYLE_ID,
                        //                   PACKING_STYLE_OTHER = m.PACKING_STYLE_OTHER,
                        //                   PACK_SIZE_ID = m.PACK_SIZE_ID,
                        //                   PACK_SIZE_OTHER = m.PACK_SIZE_OTHER,
                        //                   PRIMARY_MATERIAL = m.PRIMARY_MATERIAL,
                        //                   REQUEST_DELIVERY_DATE = m.REQUEST_DELIVERY_DATE,
                        //                   OTHER_REQUESTS = m.OTHER_REQUESTS,
                        //                   BRAND_ID = m.BRAND_ID,
                        //                   BRAND_OTHER = m.BRAND_OTHER,
                        //                   BRAND_OEM_ID = m.BRAND_OEM_ID,
                        //                   BRAND_OEM_OTHER = m.BRAND_OEM_OTHER,
                        //                   REQUEST_PHYSICAL_MOCKUP = m.REQUEST_PHYSICAL_MOCKUP,
                        //                   CREATOR_ID = m.CREATOR_ID,
                        //                   MOCKUP_NO = m.MOCKUP_NO,
                        //                   MOCKUP_STATUS = m.MOCKUP_STATUS,
                        //                   PACKING_TYPE_ID = m.PACKING_TYPE_ID,
                        //                   PRINT_SYSTEM_ID = m.PRINT_SYSTEM_ID,
                        //                   NUMBER_OF_COLOR_ID = m.NUMBER_OF_COLOR_ID,
                        //                   BOX_COLOR_ID = m.BOX_COLOR_ID,
                        //                   COATING_ID = m.COATING_ID,
                        //                   PURPOSE_OF = m.PURPOSE_OF,
                        //                   STYLE_ID = m.STYLE_ID,
                        //                   PACKING_TYPE_OTHER = m.PACKING_TYPE_OTHER,
                        //                   PRINT_SYSTEM_OTHER = m.PRINT_SYSTEM_OTHER,
                        //                   NUMBER_OF_COLOR_OTHER = m.NUMBER_OF_COLOR_OTHER,
                        //                   BOX_COLOR_OTHER = m.BOX_COLOR_OTHER,
                        //                   STYLE_OTHER = m.STYLE_OTHER,
                        //                   COATING_OTHER = m.COATING_OTHER,
                        //                   MOCKUP_ID = m.MOCKUP_ID,
                        //                   MOCKUP_SUB_ID = m.MOCKUP_SUB_ID,
                        //                   CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                        //                   CURRENT_ROLE_ID = m.CURRENT_ROLE_ID,
                        //                   CURRENT_USER_ID = m.CURRENT_USER_ID,
                        //                   REMARK = m.REMARK,
                        //                   CREATE_DATE_PROCESS = m.CREATE_DATE_PROCESS,
                        //                   CREATE_DATE_CHECK_LIST = m.CREATE_DATE_CHECK_LIST,
                        //                   CREATE_BY_PROCESS = m.CREATE_BY_PROCESS,
                        //                   CREATE_BY_CHECK_LIST = m.CREATE_BY_CHECK_LIST,
                        //                   IS_END = m.IS_END,
                        //                   PARENT_MOCKUP_SUB_ID = m.PARENT_MOCKUP_SUB_ID,
                        //                   NODE_ID = m.NODE_ID,
                        //                   CHECK_LIST_FOR_DESIGN = m.CHECK_LIST_FOR_DESIGN,
                        //                   REQUEST_FOR_DIE_LINE = m.REQUEST_FOR_DIE_LINE,
                        //                   PROJECT_NAME = m.PROJECT_NAME,
                        //                   REVIEWER = m.REVIEWER,
                        //                   TWO_P_ID = m.TWO_P_ID,
                        //                   THREE_P_ID = m.THREE_P_ID,
                        //                   REF_PRODUCT_CODE = m.REF_PRODUCT_CODE,
                        //                   CURRENT_VENDOR_ID = m.CURRENT_VENDOR_ID,
                        //                   CURRENT_CUSTOMER_ID = m.CURRENT_CUSTOMER_ID,
                        //                   NOMINATED_SPEC = m.NOMINATED_SPEC,
                        //                   NOMINATED_CONTAINER_VENDOR = m.NOMINATED_CONTAINER_VENDOR,
                        //                   CUSTOMER_OTHER_ID = m.CUSTOMER_OTHER_ID,
                        //                   SOLD_TO_DISPLAY_TXT = m.SOLD_TO_DISPLAY_TXT,
                        //                   SHIP_TO_DISPLAY_TXT = m.SHIP_TO_DISPLAY_TXT,
                        //                   BRAND_DISPLAY_TXT = m.BRAND_DISPLAY_TXT,
                        //                   PACKING_TYPE_DISPLAY_TXT = m.PACKING_TYPE_DISPLAY_TXT,
                        //                   CREATE_BY_CHECK_LIST_TITLE = m.CREATE_BY_CHECK_LIST_TITLE,
                        //                   CREATE_BY_CHECK_LIST_FIRST_NAME = m.CREATE_BY_CHECK_LIST_FIRST_NAME,
                        //                   CREATE_BY_CHECK_LIST_LAST_NAME = m.CREATE_BY_CHECK_LIST_LAST_NAME,
                        //                   CREATE_BY_PROCESS_TITLE = m.CREATE_BY_PROCESS_TITLE,
                        //                   CREATE_BY_PROCESS_FIRST_NAME = m.CREATE_BY_PROCESS_FIRST_NAME,
                        //                   CREATE_BY_PROCESS_LAST_NAME = m.CREATE_BY_PROCESS_LAST_NAME,
                        //                   CURRENT_STEP_DISPLAY_TXT = m.CURRENT_STEP_DISPLAY_TXT,
                        //                   REFERENCE_REQUEST_ID = m.REFERENCE_REQUEST_ID,
                        //                   REFERENCE_REQUEST_NO = m.REFERENCE_REQUEST_NO,
                        //                   REFERENCE_REQUEST_TYPE = m.REFERENCE_REQUEST_TYPE,
                        //                   REASON_ID = m.REASON_ID,
                        //                   DURATION = m.DURATION,
                        //                   DURATION_EXTEND = m.DURATION_EXTEND,
                        //                   STEP_MOCKUP_DESCRIPTION = m.STEP_MOCKUP_DESCRIPTION,
                        //                   IS_TERMINATE = m.IS_TERMINATE,
                        //                   REMARK_TERMINATE = m.REMARK_TERMINATE,
                        //                   IS_DELEGATE = m.IS_DELEGATE,
                        //                   STEP_MOCKUP_CODE = m.STEP_MOCKUP_CODE,
                        //                   ROLE_ID_RESPONSE = m.ROLE_ID_RESPONSE,
                        //                   REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                        //                   IS_STEP_DURATION_EXTEND = m.IS_STEP_DURATION_EXTEND,
                        //               });

                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            qMockup = qMockup.AsQueryable().Where(m => Convert.ToDateTime(m.CREATE_DATE_CHECK_LIST).Date >= getByCreateDateFrom.Date || Convert.ToDateTime(m.CREATE_DATE_PROCESS).Date >= getByCreateDateFrom.Date).ToList();
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            qMockup = qMockup.AsQueryable().Where(m => Convert.ToDateTime(m.CREATE_DATE_CHECK_LIST).Date <= getByCreateDateTo.Date || Convert.ToDateTime(m.CREATE_DATE_PROCESS).Date <= getByCreateDateTo.Date).ToList();
                        var tempDASHBOARD = qMockup.Where(p => string.IsNullOrEmpty(p.IS_END)).ToList();
                        //var tempDASHBOARD = qMockup.Where(p => string.IsNullOrEmpty(p.IS_END) &&
                        // (
                        //                                 //draft
                        //                                 (p.CURRENT_STEP_ID == null && p.CREATOR_ID == param.data.CURRENT_USER_ID)
                        //                                 // other step pool
                        //                                 || (p.CURRENT_ROLE_ID != null && p.CURRENT_STEP_ID != SEND_PG && listRole2.Contains(p.CURRENT_ROLE_ID.Value) && p.CURRENT_USER_ID == null)
                        //                                 //pg accept task
                        //                                 || (p.CURRENT_STEP_ID == SEND_PG && p.CURRENT_USER_ID == param.data.CURRENT_USER_ID)
                        //                                 // other step my task
                        //                                 || (p.CURRENT_STEP_ID != SEND_PG && p.CURRENT_USER_ID == param.data.CURRENT_USER_ID)
                        // )).ToList();

                        //Results.data = MapperServices.V_ART_WF_DASHBOARD(tempDASHBOARD);
                        Results.data = tempDASHBOARD;

                        List<V_ART_WF_DASHBOARD_2> temp = new List<V_ART_WF_DASHBOARD_2>();
                        foreach (var item in Results.data)
                        {
                            item.WORKFLOW_TYPE = "Mockup";
                            item.REFERENCE_REQUEST_ID = item.REFERENCE_REQUEST_ID;
                            item.REFERENCE_REQUEST_NO = !string.IsNullOrEmpty(item.REFERENCE_REQUEST_NO) ? item.REFERENCE_REQUEST_NO : item.CHECK_LIST_NO;
                            item.PARENT_MOCKUP_SUB_ID = item.PARENT_MOCKUP_SUB_ID;
                            if (!string.IsNullOrEmpty(item.MOCKUP_NO))
                            {
                                if (item.MOCKUP_NO.StartsWith("M"))
                                {
                                    if (item.REQUEST_FOR_DIE_LINE == "1")
                                    {
                                        item.WORKFLOW_TYPE = "Request Die line";
                                    }
                                }
                            }

                            if (item.CURRENT_USER_ID == param.data.CURRENT_USER_ID)
                            {
                                temp.Add(item);
                            }
                            else
                            {
                                var valid = CNService.CheckTypeOfProductAndCompanyMockup(Convert.ToInt32(param.data.CURRENT_USER_ID), item.CHECK_LIST_ID, item.MOCKUP_SUB_ID, context, allStepMockup, listUserCompany, listUserTypeofProduct);
                                if (valid)
                                {
                                    temp.Add(item);
                                }
                            }
                        }

                        Results.data = temp;

                        //var qArtwork = CNService.BuildIncomingArtwork(param.data.CURRENT_USER_ID.ToString(), getByCreateDateFrom, getByCreateDateTo,
                        //context,"");
                        var qArtwork = context.Database.SqlQuery<V_ART_WF_DASHBOARD_ARTWORK_2>("sp_ART_WF_DASHBOARD  @Step, @USER, @From, @To",
                            new SqlParameter("@Step", "0"),
                            new SqlParameter("@USER", (int?)param.data.CURRENT_USER_ID),
                            new SqlParameter("@From", getByCreateDateFrom), new SqlParameter("@To", getByCreateDateTo)).ToList();
                        //var qArtwork = (from m in context.V_ART_WF_DASHBOARD_ARTWORK
                        //                where string.IsNullOrEmpty(m.IS_END)
                        //                select new V_ART_WF_DASHBOARD_ARTWORK_2()
                        //                {
                        //                    REFERENCE_REQUEST_ID = m.REFERENCE_REQUEST_ID,
                        //                    REFERENCE_REQUEST_NO = m.REFERENCE_REQUEST_NO,
                        //                    REFERENCE_REQUEST_TYPE = m.REFERENCE_REQUEST_TYPE,
                        //                    TYPE_OF_ARTWORK = m.TYPE_OF_ARTWORK,
                        //                    ARTWORK_REQUEST_NO = m.ARTWORK_REQUEST_NO,
                        //                    PROJECT_NAME = m.PROJECT_NAME,
                        //                    TYPE_OF_PRODUCT_ID = m.TYPE_OF_PRODUCT_ID,
                        //                    REVIEWER_ID = m.REVIEWER_ID,
                        //                    COMPANY_ID = m.COMPANY_ID,
                        //                    SOLD_TO_ID = m.SOLD_TO_ID,
                        //                    SHIP_TO_ID = m.SHIP_TO_ID,
                        //                    CUSTOMER_OTHER_ID = m.CUSTOMER_OTHER_ID,
                        //                    PRIMARY_TYPE_ID = m.PRIMARY_TYPE_ID,
                        //                    PRIMARY_TYPE_OTHER = m.PRIMARY_TYPE_OTHER,
                        //                    PRIMARY_SIZE_ID = m.PRIMARY_SIZE_ID,
                        //                    PRIMARY_SIZE_OTHER = m.PRIMARY_SIZE_OTHER,
                        //                    CONTAINER_TYPE_ID = m.CONTAINER_TYPE_ID,
                        //                    CONTAINER_TYPE_OTHER = m.CONTAINER_TYPE_OTHER,
                        //                    LID_TYPE_ID = m.LID_TYPE_ID,
                        //                    LID_TYPE_OTHER = m.LID_TYPE_OTHER,
                        //                    PACKING_STYLE_ID = m.PACKING_STYLE_ID,
                        //                    PACKING_STYLE_OTHER = m.PACKING_STYLE_OTHER,
                        //                    PACK_SIZE_ID = m.PACK_SIZE_ID,
                        //                    PACK_SIZE_OTHER = m.PACK_SIZE_OTHER,
                        //                    BRAND_ID = m.BRAND_ID,
                        //                    BRAND_OTHER = m.BRAND_OTHER,
                        //                    REQUEST_DELIVERY_DATE = m.REQUEST_DELIVERY_DATE,
                        //                    SPECIAL_REQUIREMENT = m.SPECIAL_REQUIREMENT,
                        //                    OTHER_REQUEST = m.OTHER_REQUEST,
                        //                    TWO_P_ID = m.TWO_P_ID,
                        //                    THREE_P_ID = m.THREE_P_ID,
                        //                    CREATE_DATE_ARTWORK_REQUEST = m.CREATE_DATE_ARTWORK_REQUEST,
                        //                    CREATE_BY_ARTWORK_REQUEST = m.CREATE_BY_ARTWORK_REQUEST,
                        //                    ARTWORK_ITEM_ID = m.ARTWORK_ITEM_ID,
                        //                    REQUEST_ITEM_NO = m.REQUEST_ITEM_NO,
                        //                    SOLD_TO_DISPLAY_TXT = m.SOLD_TO_DISPLAY_TXT,
                        //                    SHIP_TO_DISPLAY_TXT = m.SHIP_TO_DISPLAY_TXT,
                        //                    BRAND_DISPLAY_TXT = m.BRAND_DISPLAY_TXT,
                        //                    CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                        //                    CURRENT_USER_ID = m.CURRENT_USER_ID,
                        //                    CURRENT_ROLE_ID = m.CURRENT_ROLE_ID,
                        //                    CURRENT_VENDOR_ID = m.CURRENT_VENDOR_ID,
                        //                    CURRENT_CUSTOMER_ID = m.CURRENT_CUSTOMER_ID,
                        //                    REMARK = m.REMARK,
                        //                    IS_END = m.IS_END,
                        //                    REASON_ID = m.REASON_ID,
                        //                    CREATE_BY_PROCESS_TITLE = m.CREATE_BY_PROCESS_TITLE,
                        //                    CREATE_BY_PROCESS_FIRST_NAME = m.CREATE_BY_PROCESS_FIRST_NAME,
                        //                    CREATE_BY_PROCESS_LAST_NAME = m.CREATE_BY_PROCESS_LAST_NAME,
                        //                    CURRENT_STEP_DISPLAY_TXT = m.CURRENT_STEP_DISPLAY_TXT,
                        //                    CREATE_BY_ARTWORK_REQUEST_TITLE = m.CREATE_BY_ARTWORK_REQUEST_TITLE,
                        //                    CREATE_BY_ARTWORK_REQUEST_FIRST_NAME = m.CREATE_BY_ARTWORK_REQUEST_FIRST_NAME,
                        //                    CREATE_BY_ARTWORK_REQUEST_LAST_NAME = m.CREATE_BY_ARTWORK_REQUEST_LAST_NAME,
                        //                    ARTWORK_SUB_ID = m.ARTWORK_SUB_ID,
                        //                    PARENT_ARTWORK_SUB_ID = m.PARENT_ARTWORK_SUB_ID,
                        //                    ARTWORK_REQUEST_ID = m.ARTWORK_REQUEST_ID,
                        //                    CREATE_DATE_PROCESS = m.CREATE_DATE_PROCESS,
                        //                    CREATE_BY_PROCESS = m.CREATE_BY_PROCESS,
                        //                    REMARK_TERMINATE = m.REMARK_TERMINATE,
                        //                    IS_DELEGATE = m.IS_DELEGATE,
                        //                    IS_TERMINATE = m.IS_TERMINATE,
                        //                    DURATION = m.DURATION,
                        //                    DURATION_EXTEND = m.DURATION_EXTEND,
                        //                    STEP_ARTWORK_DESCRIPTION = m.STEP_ARTWORK_DESCRIPTION,
                        //                    STEP_ARTWORK_CODE = m.STEP_ARTWORK_CODE,
                        //                    ROLE_ID_RESPONSE = m.ROLE_ID_RESPONSE,
                        //                    REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                        //                    IS_STEP_DURATION_EXTEND = m.IS_STEP_DURATION_EXTEND,
                        //                });
                        //qArtwork = qArtwork.AsQueryable().Where(m => m.NUM == 10).ToList();
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            qArtwork = qArtwork.AsQueryable().Where(m => Convert.ToDateTime(m.CREATE_DATE_ARTWORK_REQUEST).Date >= getByCreateDateFrom.Date || 
                            Convert.ToDateTime(m.CREATE_DATE_PROCESS).Date >= getByCreateDateFrom.Date).ToList();
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            qArtwork = qArtwork.AsQueryable().Where(m => Convert.ToDateTime(m.CREATE_DATE_ARTWORK_REQUEST).Date <= getByCreateDateTo.Date ||
                            Convert.ToDateTime(m.CREATE_DATE_PROCESS).Date <= getByCreateDateTo.Date).ToList();

                        //if (CNService.IsPA(Convert.ToInt32(param.data.CURRENT_USER_ID), context))
                        //{
                        //    var processPP = (from m in context.ART_WF_ARTWORK_PROCESS
                        //                     join t2 in context.ART_WF_ARTWORK_PROCESS_PP on m.ARTWORK_SUB_ID equals t2.ARTWORK_SUB_ID into ps
                        //                     from t2 in ps.DefaultIfEmpty()
                        //                     where m.CURRENT_STEP_ID == SEND_PP && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                        //                     && t2.ACTION_CODE != "SEND_BACK"
                        //                     select m.ARTWORK_ITEM_ID).ToList();

                        //    if (param.data.SENT_PP)
                        //    {
                        //        qArtwork = qArtwork.Where(m => processPP.Contains(m.ARTWORK_ITEM_ID));
                        //    }
                        //    else
                        //    {
                        //        qArtwork = qArtwork.Where(m => !processPP.Contains(m.ARTWORK_ITEM_ID));
                        //    }
                        //}

                        if (CNService.IsPA(Convert.ToInt32(param.data.CURRENT_USER_ID), context))
                        {
                            if (param.data.SENT_PP)
                            {
                                qArtwork = qArtwork.Where(m =>
                                            (from m2 in context.ART_WF_ARTWORK_PROCESS
                                             join t2 in context.ART_WF_ARTWORK_PROCESS_PP on m2.ARTWORK_SUB_ID equals t2.ARTWORK_SUB_ID into ps
                                             from t2 in ps.DefaultIfEmpty()
                                             where m2.CURRENT_STEP_ID == SEND_PP && string.IsNullOrEmpty(m2.REMARK_KILLPROCESS)
                                             && t2.ACTION_CODE != "SEND_BACK"
                                             select m2.ARTWORK_ITEM_ID).Contains(m.ARTWORK_ITEM_ID)).ToList();
                            }
                            else
                            {
                                qArtwork = qArtwork.Where(m =>
                                             !(from m2 in context.ART_WF_ARTWORK_PROCESS
                                               join t2 in context.ART_WF_ARTWORK_PROCESS_PP on m2.ARTWORK_SUB_ID equals t2.ARTWORK_SUB_ID into ps
                                               from t2 in ps.DefaultIfEmpty()
                                               where m2.CURRENT_STEP_ID == SEND_PP && string.IsNullOrEmpty(m2.REMARK_KILLPROCESS)
                                               && t2.ACTION_CODE != "SEND_BACK"
                                               select m2.ARTWORK_ITEM_ID).Contains(m.ARTWORK_ITEM_ID)).ToList();
                            }
                        }

                        var tempDASHBOARDArtwork = qArtwork.Where(m => string.IsNullOrEmpty(m.IS_END)
                                                &&
                                                (
                                                    (m.CURRENT_STEP_ID == null && m.CREATE_BY_ARTWORK_REQUEST == param.data.CURRENT_USER_ID && string.IsNullOrEmpty(m.ARTWORK_REQUEST_NO))
                                                    //draft mk
                                                    || (m.CURRENT_STEP_ID == null && (from m2 in context.ART_WF_ARTWORK_REQUEST_RECIPIENT where m2.RECIPIENT_USER_ID == param.data.CURRENT_USER_ID.Value && m2.ARTWORK_REQUEST_ID == m.ARTWORK_REQUEST_ID select m2).Count() > 0 && string.IsNullOrEmpty(m.REQUEST_ITEM_NO))
                                                    // other step pool
                                                    || ((m.CURRENT_ROLE_ID != null && m.CURRENT_STEP_ID != SEND_PA && m.CURRENT_STEP_ID != SEND_VN_PO && m.CURRENT_STEP_ID != SEND_PG_ARTWORK && m.CURRENT_STEP_ID != SEND_PP) && listRole2.Contains(m.CURRENT_ROLE_ID.Value) && m.CURRENT_USER_ID == null)
                                                    //pa accept task
                                                    || (m.CURRENT_STEP_ID == SEND_PA && m.CURRENT_USER_ID == param.data.CURRENT_USER_ID && (from o in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL select o.ARTWORK_SUB_ID).Contains(m.ARTWORK_SUB_ID))
                                                    // other step my task
                                                    || ((m.CURRENT_STEP_ID != SEND_PA && m.CURRENT_STEP_ID != SEND_VN_PO) && m.CURRENT_USER_ID == param.data.CURRENT_USER_ID)
                                                )
                                                ).ToList();

                        //var listArtwork = MapperServices.V_ART_WF_DASHBOARD_ARTWORK(tempDASHBOARDArtwork);
                        var listArtwork = tempDASHBOARDArtwork;

                        List<V_ART_WF_DASHBOARD_ARTWORK_2> tempListArtwork = new List<V_ART_WF_DASHBOARD_ARTWORK_2>();
                        foreach (var item in listArtwork)
                        {
                            if (item.CURRENT_USER_ID == param.data.CURRENT_USER_ID)
                            {
                                tempListArtwork.Add(item);
                            }
                            else
                            {
                                var valid = CNService.CheckTypeOfProductAndCompanyArtwork(Convert.ToInt32(param.data.CURRENT_USER_ID), item.ARTWORK_REQUEST_ID, item.ARTWORK_SUB_ID, context, allStepArtwork, listUserCompany, listUserTypeofProduct);
                                if (valid)
                                {
                                    tempListArtwork.Add(item);
                                }
                            }
                        }

                        var tempArtwork = new List<V_ART_WF_DASHBOARD_2>();
                        foreach (var artwork in tempListArtwork)
                        {
                            var doWork = false;
                            if (artwork.ARTWORK_SUB_ID == 0)
                            {
                                if (tempArtwork.Where(m => m.CHECK_LIST_ID == artwork.ARTWORK_REQUEST_ID).ToList().Count == 0)
                                {
                                    doWork = true;
                                }
                            }
                            else
                            {
                                doWork = true;
                            }

                            if (doWork)
                            {
                                V_ART_WF_DASHBOARD_2 item = new V_ART_WF_DASHBOARD_2();
                                item.CHECK_LIST_ID = artwork.ARTWORK_REQUEST_ID;
                                item.MOCKUP_ID = artwork.ARTWORK_ITEM_ID;
                                item.MOCKUP_SUB_ID = artwork.ARTWORK_SUB_ID;
                                item.PARENT_MOCKUP_SUB_ID = artwork.PARENT_ARTWORK_SUB_ID;
                                item.REQUEST_ITEM_NO = artwork.REQUEST_ITEM_NO;
                                item.REQUEST_FORM_NO = artwork.ARTWORK_REQUEST_NO;
                                item.REFERENCE_REQUEST_ID = artwork.REFERENCE_REQUEST_ID;
                                item.REFERENCE_REQUEST_NO = !string.IsNullOrEmpty(artwork.REFERENCE_REQUEST_NO) ? artwork.REFERENCE_REQUEST_NO : artwork.ARTWORK_REQUEST_NO;
                                item.MOCKUP_NO = artwork.REQUEST_ITEM_NO;
                                item.CHECK_LIST_NO = artwork.ARTWORK_REQUEST_NO;
                                item.DURATION = artwork.DURATION;

                                if (!String.IsNullOrEmpty(artwork.IS_STEP_DURATION_EXTEND) && artwork.IS_STEP_DURATION_EXTEND.Equals("X"))
                                {
                                    item.DURATION = artwork.DURATION_EXTEND;
                                }

                                item.CURRENT_STEP_ID = artwork.CURRENT_STEP_ID;
                                item.CURRENT_STEP_DISPLAY_TXT = artwork.CURRENT_STEP_DISPLAY_TXT;

                                if (artwork.ARTWORK_SUB_ID == 0)
                                {
                                    if (string.IsNullOrEmpty(item.REQUEST_FORM_NO))
                                    {
                                        item.CURRENT_STEP_DISPLAY_TXT = "";
                                    }
                                    else
                                    {
                                        item.CURRENT_STEP_DISPLAY_TXT = "Assigned MK";
                                    }
                                }
                                item.CURRENT_ASSIGN = artwork.CURRENT_ASSIGN;
                                item.PLANT = artwork.PLANT;
                                item.CREATE_DATE_PROCESS = artwork.CREATE_DATE_PROCESS;
                                item.BRAND_DISPLAY_TXT = artwork.BRAND_DISPLAY_TXT;
                                item.SOLD_TO_DISPLAY_TXT = artwork.SOLD_TO_DISPLAY_TXT;
                                item.SHIP_TO_DISPLAY_TXT = artwork.SHIP_TO_DISPLAY_TXT;
                                item.PRIMARY_SIZE_ID = artwork.PRIMARY_SIZE_ID;
                                item.THREE_P_ID = artwork.THREE_P_ID;
                                item.REQUEST_DELIVERY_DATE = artwork.REQUEST_DELIVERY_DATE;
                                item.WORKFLOW_TYPE = "Artwork";
                                item.CREATE_DATE_CHECK_LIST = artwork.CREATE_DATE_ARTWORK_REQUEST;


                                //start #TSK-1511 #SR-70695 by aof in 09/2022
                                if (artwork.CURRENT_STEP_ID == SEND_QC)
                                {
                                    if (artwork.CURRENT_USER_ID is null || artwork.CURRENT_USER_ID <= 0)
                                    {
                                        item.WF_STATUS = "Wait for accept";
                                    }
                                    else
                                    {
                                        var ProcessRD = context.ART_WF_ARTWORK_PROCESS
                                                        .Where(w => w.PARENT_ARTWORK_SUB_ID == item.MOCKUP_SUB_ID && w.CURRENT_STEP_ID == SEND_RD)
                                                        .Select(s => s)
                                                        .OrderByDescending(o=>o.CREATE_DATE).FirstOrDefault();
                                        if (ProcessRD == null)
                                        {
                                            item.WF_STATUS = "";
                                        }
                                        else
                                        {
                                            if (ProcessRD.IS_END == null)
                                            {
                                                item.WF_STATUS = "Wait for RD";
                                            }
                                            else
                                            {
                                                item.WF_STATUS = "Received from RD";
                                            }
                                        }
                                    }
                                }
                                //end #TSK-1511 #SR-70695 by aof in 09/2022

                                tempArtwork.Add(item);
                            }
                        }

                        foreach (var item in tempArtwork)
                        {
                            Results.data.Add(item);
                        }

                        if (param != null)
                        {
                            Results.draw = param.draw;
                        }

                        Results = BindDataInbox(Results);

                        Results.recordsFiltered = Results.data.ToList().Count;
                        Results.recordsTotal = Results.data.ToList().Count;
                        Results.status = "S";
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

        private static V_ART_WF_DASHBOARD_RESULT BindDataInbox(V_ART_WF_DASHBOARD_RESULT Results)
        {
            if (Results.data.Count > 0)
            {
                var msg = MessageHelper.GetMessage("MSG_005");
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var listAllUsers = ART_M_USER_SERVICE.GetAll(context);

                        var listMockupId = Results.data.Where(m => m.WORKFLOW_TYPE == "Mockup").Select(m => m.MOCKUP_ID).Distinct().ToList();
                        var allDashboard = (from m in context.ART_WF_MOCKUP_PROCESS where listMockupId.Contains(m.MOCKUP_ID) select m).ToList();

                        var listItemId = Results.data.Where(m => m.WORKFLOW_TYPE == "Artwork").Select(m => m.MOCKUP_ID).Distinct().ToList();

                        var listArtworkSubId = Results.data.Where(m => m.WORKFLOW_TYPE == "Artwork" && m.PARENT_MOCKUP_SUB_ID != null).Select(m => m.PARENT_MOCKUP_SUB_ID).Distinct().ToList();
                        var listArtworkSubId2 = Results.data.Where(m => m.WORKFLOW_TYPE == "Artwork" && m.PARENT_MOCKUP_SUB_ID == null).Select(m => m.MOCKUP_SUB_ID).Distinct().ToList();

                        var allDashboardArtwork = (from m in context.ART_WF_ARTWORK_PROCESS where listItemId.Contains(m.ARTWORK_ITEM_ID) select m).ToList();

                        var allProcessPA = (from m in context.ART_WF_ARTWORK_PROCESS_PA where listArtworkSubId.Contains(m.ARTWORK_SUB_ID) || listArtworkSubId2.Contains(m.ARTWORK_SUB_ID) select m).ToList();

                        var allStepMockup = ART_M_STEP_MOCKUP_SERVICE.GetAll(context);
                        var allStepArtwork = ART_M_STEP_ARTWORK_SERVICE.GetAll(context);
                        List<SAP_M_COUNTRY> list_country = new List<SAP_M_COUNTRY>();

                        var SEND_PA = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_PA").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_PG = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_PG").FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_PP = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_PP").FirstOrDefault().STEP_ARTWORK_ID;

                        var listTHREE_P_ID = Results.data.Select(m => m.THREE_P_ID).Distinct().ToList();
                        var listAllThreeP = (from m in context.SAP_M_3P where listTHREE_P_ID.Contains(m.THREE_P_ID) select m).ToList();

                        var listPRIMARY_SIZE_ID = Results.data.Select(m => m.PRIMARY_SIZE_ID).Distinct().ToList();
                        var listAllPRIMARY_SIZE = (from m in context.SAP_M_CHARACTERISTIC where listPRIMARY_SIZE_ID.Contains(m.CHARACTERISTIC_ID) select m).ToList();

                        var listChecklistId = Results.data.Where(m => m.WORKFLOW_TYPE == "Mockup").Select(m => m.CHECK_LIST_ID).Distinct().ToList();
                        var listAllChecklistProduct = (from m in context.ART_WF_MOCKUP_CHECK_LIST_PRODUCT where listChecklistId.Contains(m.CHECK_LIST_ID) select m).ToList();

                        var listRequestId = Results.data.Where(m => m.WORKFLOW_TYPE == "Artwork").Select(m => m.CHECK_LIST_ID).Distinct().ToList();
                        var listAllRequestProduct = (from m in context.ART_WF_ARTWORK_REQUEST_PRODUCT where listRequestId.Contains(m.ARTWORK_REQUEST_ID) select m).ToList();

                        var listProductId = listAllRequestProduct.Select(m => m.PRODUCT_CODE_ID).Distinct().ToList();
                        var listAllProduct = (from m in context.XECM_M_PRODUCT where listProductId.Contains(m.XECM_PRODUCT_ID) select m).ToList();

                        var listProductIdPA = allProcessPA.Select(m => m.PRODUCT_CODE_ID).Distinct().ToList();
                        var listAllProductPA = (from m in context.XECM_M_PRODUCT where listProductIdPA.Contains(m.XECM_PRODUCT_ID) select m).ToList();

                        var listProduct5 = allProcessPA.Select(m => m.MATERIAL_NO).Distinct().ToList();
                        var listAllProduct5 = (from m in context.XECM_M_PRODUCT5 where listProduct5.Contains(m.PRODUCT_CODE) select m).ToList();

                        var allListHeader = (from p in context.V_ART_ASSIGNED_SO
                                             where listArtworkSubId.Contains(p.ARTWORK_SUB_ID) || listArtworkSubId2.Contains(p.ARTWORK_SUB_ID)
                                             select new V_ART_ASSIGNED_SO_2()
                                             {
                                                 SALES_ORG = p.SALES_ORG,
                                                 PORT = p.PORT,
                                                 SALES_ORDER_NO = p.SALES_ORDER_NO,
                                                 ITEM = p.ITEM,
                                                 ARTWORK_SUB_ID = p.ARTWORK_SUB_ID,
                                                 PRODUCTION_PLANT = p.PRODUCTION_PLANT,
                                             }).ToList();

                        var listSubIdPP = allDashboardArtwork.Where(m => m.CURRENT_STEP_ID == SEND_PP).Select(m => m.ARTWORK_SUB_ID).ToList();
                        var listAllProcessPP = (from m in context.ART_WF_ARTWORK_PROCESS_PP where listSubIdPP.Contains(m.ARTWORK_SUB_ID) select m).ToList();
                        var listAllReason = (from m in context.ART_M_DECISION_REASON select m).ToList();
                        var listAllReasonOther = (from m in context.ART_WF_REMARK_REASON_OTHER where m.WF_TYPE == "A" && listSubIdPP.Contains((int)m.WF_SUB_ID) select m).ToList();

                        for (int i = 0; i < Results.data.Count; i++)
                        {
                            if (Results.data[i].CREATE_DATE_PROCESS != null)
                            {
                                DateTime? dtReceiveWf = Results.data[i].CREATE_DATE_PROCESS;
                                DateTime? dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(Results.data[i].DURATION.Value));

                                if (!String.IsNullOrEmpty(Results.data[i].IS_STEP_DURATION_EXTEND) && Results.data[i].IS_STEP_DURATION_EXTEND.Equals("X"))
                                {
                                    dtWillFinish = CNService.AddBusinessDays(dtReceiveWf.Value, (int)Math.Ceiling(Results.data[i].DURATION_EXTEND.Value));
                                }

                                Results.data[i].DUEDATE_DISPLAY_TXT = CNService.GetWorkingDays(DateTime.Now, dtWillFinish.Value) + " (" + dtWillFinish.Value.ToString("dd/MM/yyyy HH:mm") + ")";
                            }

                            string type = "";
                            if (string.IsNullOrEmpty(Results.data[i].MOCKUP_NO))
                            {
                                Results.data[i].MOCKUP_NO = "";
                            }
                            if (string.IsNullOrEmpty(Results.data[i].CHECK_LIST_NO))
                            {
                                Results.data[i].CHECK_LIST_NO = "Draft";
                            }

                            if (Results.data[i].WORKFLOW_TYPE == "Artwork") type = "A";
                            else type = "M";

                            if (type == "M")
                            {
                                if (Results.data[i].MOCKUP_ID > 0)
                                {
                                    //var tempProcess = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = Results.data[i].MOCKUP_ID }, context);
                                    var tempProcess = allDashboard.Where(m => m.MOCKUP_ID == Results.data[i].MOCKUP_ID).ToList();
                                    var temp = tempProcess.Where(m => m.CURRENT_STEP_ID == SEND_PG).FirstOrDefault();
                                    if (temp != null)
                                    {
                                        Results.data[i].PA_PG_DISPLAY_TXT = CNService.GetUserName(temp.CURRENT_USER_ID, listAllUsers);
                                    }

                                    var temp2 = tempProcess.OrderByDescending(m => m.UPDATE_DATE).FirstOrDefault();
                                    if (temp2 != null)
                                    {
                                        Results.data[i].LAST_UPDATE_DATE_WF = temp2.UPDATE_DATE;
                                    }
                                }
                            }
                            else if (type == "A")
                            {
                                if (Results.data[i].MOCKUP_ID > 0)
                                {
                                    //var tempProcess = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = Results.data[i].MOCKUP_ID }, context);
                                    var tempProcess = allDashboardArtwork.Where(m => m.ARTWORK_ITEM_ID == Results.data[i].MOCKUP_ID).ToList();
                                    var temp = tempProcess.Where(m => m.CURRENT_STEP_ID == SEND_PA).FirstOrDefault();
                                    if (temp != null)
                                    {
                                        Results.data[i].PA_PG_DISPLAY_TXT = CNService.GetUserName(temp.CURRENT_USER_ID, listAllUsers);
                                    }

                                    var temp2 = tempProcess.OrderByDescending(m => m.UPDATE_DATE).FirstOrDefault();
                                    if (temp2 != null)
                                    {
                                        Results.data[i].LAST_UPDATE_DATE_WF = temp2.UPDATE_DATE;
                                    }
                                }
                            }

                            Results.data[i].CREATE_BY_CHECK_LIST_DISPLAY_TXT = Results.data[i].CREATE_BY_CHECK_LIST_TITLE + " " + Results.data[i].CREATE_BY_CHECK_LIST_FIRST_NAME + " " + Results.data[i].CREATE_BY_CHECK_LIST_LAST_NAME;
                            Results.data[i].CREATE_BY_PROCESS_DISPLAY_TXT = Results.data[i].CREATE_BY_PROCESS_TITLE + " " + Results.data[i].CREATE_BY_PROCESS_FIRST_NAME + " " + Results.data[i].CREATE_BY_PROCESS_LAST_NAME;

                            if (Results.data[i].CURRENT_STEP_DISPLAY_TXT == null)
                                Results.data[i].CURRENT_STEP_DISPLAY_TXT = "";

                            if (Results.data[i].THREE_P_ID > 0)
                            {
                                var temp = listAllThreeP.Where(m => m.THREE_P_ID == Results.data[i].THREE_P_ID).FirstOrDefault();
                                if (temp != null)
                                    Results.data[i].PRIMARY_SIZE_DISPLAY_TXT = temp.PRIMARY_SIZE_DESCRIPTION;
                            }
                            else if (Results.data[i].PRIMARY_SIZE_ID > 0)
                            {
                                var PRIMARY_SIZE = listAllPRIMARY_SIZE.Where(m => m.CHARACTERISTIC_ID == Results.data[i].PRIMARY_SIZE_ID).FirstOrDefault();
                                if (PRIMARY_SIZE != null)
                                    Results.data[i].PRIMARY_SIZE_DISPLAY_TXT = PRIMARY_SIZE.DESCRIPTION;
                            }
                            else
                            {
                                Results.data[i].PRIMARY_SIZE_DISPLAY_TXT = Results.data[i].PRIMARY_SIZE_OTHER;
                            }

                            if (type == "M")
                            {
                                if (Results.data[i].CREATE_DATE_PROCESS == null)
                                    Results.data[i].CREATE_DATE_PROCESS = Results.data[i].CREATE_DATE_CHECK_LIST;

                                if (string.IsNullOrEmpty(Results.data[i].PRIMARY_SIZE_DISPLAY_TXT))
                                {
                                    //var temp = ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_PRODUCT() { CHECK_LIST_ID = Results.data[i].CHECK_LIST_ID }, context).FirstOrDefault();
                                    var temp = listAllChecklistProduct.Where(m => m.CHECK_LIST_ID == Results.data[i].CHECK_LIST_ID).FirstOrDefault();
                                    if (temp != null)
                                        Results.data[i].PRIMARY_SIZE_DISPLAY_TXT = temp.PRIMARY_SIZE;
                                }

                                //ธง
                                if (Results.data[i].MOCKUP_ID > 0)
                                {
                                    var temp2 = allStepMockup.Where(m => m.STEP_MOCKUP_ID == Results.data[i].CURRENT_STEP_ID).FirstOrDefault();
                                    if (temp2 != null)
                                    {
                                        if (temp2.STEP_MOCKUP_CODE == "SEND_PG")
                                        {
                                            var temp_dashboardWaiting = allDashboard.Where(m => m.MOCKUP_ID == Results.data[i].MOCKUP_ID && string.IsNullOrEmpty(m.IS_END)).ToList();
                                            Results.data[i].CNT_TOTAL_SUB_WF_NOT_END = temp_dashboardWaiting.Count() - 1;
                                            Results.data[i].WAITING_STEP = "";
                                            temp_dashboardWaiting = temp_dashboardWaiting.OrderBy(m => m.CREATE_DATE).ToList();
                                            foreach (var item in temp_dashboardWaiting)
                                            {
                                                var tempMockup = allStepMockup.Where(m => m.STEP_MOCKUP_ID == item.CURRENT_STEP_ID).FirstOrDefault();
                                                if (tempMockup != null)
                                                {
                                                    if (tempMockup.STEP_MOCKUP_CODE != "SEND_PG")
                                                    {
                                                        if (item.CURRENT_USER_ID != null)
                                                        {
                                                            if (string.IsNullOrEmpty(Results.data[i].WAITING_STEP))
                                                                Results.data[i].WAITING_STEP += tempMockup.STEP_MOCKUP_NAME + " [" + CNService.GetUserName(item.CURRENT_USER_ID, listAllUsers) + "]";
                                                            else
                                                                Results.data[i].WAITING_STEP += "<br/>" + tempMockup.STEP_MOCKUP_NAME + " [" + CNService.GetUserName(item.CURRENT_USER_ID, listAllUsers) + "]";
                                                        }
                                                        else
                                                        {
                                                            if (string.IsNullOrEmpty(Results.data[i].WAITING_STEP))
                                                                Results.data[i].WAITING_STEP += tempMockup.STEP_MOCKUP_NAME + " [" + msg + "]";
                                                            else
                                                                Results.data[i].WAITING_STEP += "<br/>" + tempMockup.STEP_MOCKUP_NAME + " [" + msg + "]";
                                                        }
                                                    }
                                                }
                                            }

                                            var temp_dashboardEnd = allDashboard.Where(m => m.MOCKUP_ID == Results.data[i].MOCKUP_ID && m.IS_END == "X").ToList();
                                            temp_dashboardEnd = temp_dashboardEnd.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                                            Results.data[i].CNT_TOTAL_SUB_WF_END = temp_dashboardEnd.Count();
                                            Results.data[i].END_STEP = "";
                                            temp_dashboardEnd = temp_dashboardEnd.OrderBy(m => m.CREATE_DATE).ToList();
                                            foreach (var item in temp_dashboardEnd)
                                            {
                                                var tempMockup = allStepMockup.Where(m => m.STEP_MOCKUP_ID == item.CURRENT_STEP_ID).FirstOrDefault();
                                                if (tempMockup != null)
                                                {
                                                    if (tempMockup.STEP_MOCKUP_CODE != "SEND_PG")
                                                    {
                                                        if (item.CURRENT_USER_ID != null)
                                                        {
                                                            if (string.IsNullOrEmpty(Results.data[i].END_STEP))
                                                                Results.data[i].END_STEP += tempMockup.STEP_MOCKUP_NAME + " [" + CNService.GetUserName(item.CURRENT_USER_ID, listAllUsers) + "]";
                                                            else
                                                                Results.data[i].END_STEP += "<br/>" + tempMockup.STEP_MOCKUP_NAME + " [" + CNService.GetUserName(item.CURRENT_USER_ID, listAllUsers) + "]";
                                                        }
                                                        else
                                                        {
                                                            if (string.IsNullOrEmpty(Results.data[i].END_STEP))
                                                                Results.data[i].END_STEP += tempMockup.STEP_MOCKUP_NAME + " [" + msg + "]";
                                                            else
                                                                Results.data[i].END_STEP += "<br/>" + tempMockup.STEP_MOCKUP_NAME + " [" + msg + "]";
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Results.data[i].CURRENT_STEP_ID == SEND_PA)
                                {
                                    var lastProcessPP = allDashboardArtwork.Where(m => m.ARTWORK_ITEM_ID == Results.data[i].MOCKUP_ID && m.CURRENT_STEP_ID == SEND_PP).OrderByDescending(m => m.CREATE_DATE).ToList();
                                    if (lastProcessPP.Count > 0)
                                    {
                                        var tempLastProcessPP = lastProcessPP.FirstOrDefault();
                                        if (tempLastProcessPP != null)
                                        {
                                            var processPP = listAllProcessPP.Where(m => m.ARTWORK_SUB_ID == tempLastProcessPP.ARTWORK_SUB_ID).FirstOrDefault();
                                            if (processPP != null)
                                            {
                                                if (processPP.ACTION_CODE == "SEND_BACK")
                                                {
                                                    Results.data[i].PP_SEND_BACK = true;

                                                    var PP_SEND_BACK_COMMENT = "";
                                                    var reason = listAllReason.Where(m => m.ART_M_DECISION_REASON_ID == processPP.REASON_ID).FirstOrDefault();
                                                    if (reason != null)
                                                    {
                                                        PP_SEND_BACK_COMMENT = reason.DESCRIPTION;
                                                    }

                                                    var reasonOther = listAllReasonOther.Where(m => m.WF_SUB_ID == processPP.ARTWORK_SUB_ID).FirstOrDefault();
                                                    if (reasonOther != null)
                                                    {
                                                        if (string.IsNullOrEmpty(PP_SEND_BACK_COMMENT))
                                                            PP_SEND_BACK_COMMENT = reasonOther.REMARK_REASON;
                                                        else
                                                            PP_SEND_BACK_COMMENT += ", " + reasonOther.REMARK_REASON;
                                                    }

                                                    if (string.IsNullOrEmpty(PP_SEND_BACK_COMMENT))
                                                        PP_SEND_BACK_COMMENT = processPP.COMMENT;
                                                    else
                                                        PP_SEND_BACK_COMMENT += ", " + processPP.COMMENT;

                                                    Results.data[i].PP_SEND_BACK_COMMENT = PP_SEND_BACK_COMMENT;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (Results.data[i].CREATE_DATE_PROCESS == null)
                                    Results.data[i].CREATE_DATE_PROCESS = Results.data[i].CREATE_DATE_CHECK_LIST;

                                if (string.IsNullOrEmpty(Results.data[i].PRIMARY_SIZE_DISPLAY_TXT))
                                {
                                    //var temp2 = ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_PRODUCT() { ARTWORK_REQUEST_ID = Results.data[i].CHECK_LIST_ID }, context).FirstOrDefault();
                                    var temp2 = listAllRequestProduct.Where(m => m.ARTWORK_REQUEST_ID == Results.data[i].CHECK_LIST_ID).FirstOrDefault();
                                    if (temp2 != null)
                                    {
                                        var temp3 = listAllProduct.Where(m => m.XECM_PRODUCT_ID == temp2.PRODUCT_CODE_ID).FirstOrDefault();
                                        //var temp3 = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(temp2.PRODUCT_CODE_ID, context);
                                        if (temp3 != null)
                                            Results.data[i].PRIMARY_SIZE_DISPLAY_TXT = temp3.PRIMARY_SIZE;
                                    }
                                }

                                var parent = allDashboardArtwork.Where(m => m.ARTWORK_ITEM_ID == Results.data[i].MOCKUP_ID && m.PARENT_ARTWORK_SUB_ID == null).FirstOrDefault();
                                if (parent != null)
                                {
                                    var temp = allProcessPA.Where(m => m.ARTWORK_SUB_ID == parent.ARTWORK_SUB_ID).FirstOrDefault();
                                    if (temp != null)
                                    {
                                        if (!string.IsNullOrEmpty(temp.MATERIAL_NO))
                                        {
                                            var tempMat5 = listAllProduct5.Where(m => m.PRODUCT_CODE == temp.MATERIAL_NO).FirstOrDefault();
                                            //var tempMat5 = (from p in context.XECM_M_PRODUCT5
                                            //                where p.PRODUCT_CODE == temp.MATERIAL_NO
                                            //                select new XECM_M_PRODUCT5_2()
                                            //                {
                                            //                    PRODUCT_DESCRIPTION = p.PRODUCT_DESCRIPTION,
                                            //                }).FirstOrDefault();

                                            if (tempMat5 != null)
                                                Results.data[i].MAT5 = temp.MATERIAL_NO + ":" + tempMat5.PRODUCT_DESCRIPTION;
                                            else
                                                Results.data[i].MAT5 = temp.MATERIAL_NO;
                                        }

                                        Results.data[i].PACKING_TYPE_DISPLAY_TXT = CNService.GetCharacteristicCodeAndDescription(temp.MATERIAL_GROUP_ID, context);

                                        if (temp.PRODUCT_CODE_ID > 0)
                                        {
                                            //var product = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(temp.PRODUCT_CODE_ID, context);
                                            var product = listAllProductPA.Where(m => m.XECM_PRODUCT_ID == temp.PRODUCT_CODE_ID).FirstOrDefault();
                                            if (product != null)
                                            {
                                                Results.data[i].PRODUCT_CODE = product.PRODUCT_CODE + ":" + product.PRODUCT_DESCRIPTION;
                                            }
                                        }
                                    }

                                    var tempListHeader = allListHeader.Where(m => m.ARTWORK_SUB_ID == parent.ARTWORK_SUB_ID).ToList();

                                    foreach (var item2 in tempListHeader.Select(m => m.PORT).Distinct().ToList())
                                    {
                                        if (string.IsNullOrEmpty(Results.data[i].DESTINATION))
                                        {
                                            Results.data[i].DESTINATION = item2.Trim();
                                        }
                                        else
                                        {
                                            Results.data[i].DESTINATION += ", " + item2.Trim();
                                        }
                                    }
                                    foreach (var item2 in tempListHeader.ToList())
                                    {
                                        if (string.IsNullOrEmpty(Results.data[i].SO_AND_ITEM_NO))
                                        {
                                            Results.data[i].SO_AND_ITEM_NO = item2.SALES_ORDER_NO + "(" + item2.ITEM + ")";
                                        }
                                        else
                                        {
                                            Results.data[i].SO_AND_ITEM_NO += ", " + item2.SALES_ORDER_NO + "(" + item2.ITEM + ")";
                                        }
                                    }

                                    foreach (var item2 in tempListHeader.Select(m => m.SALES_ORG).Distinct().ToList())
                                    {
                                        if (string.IsNullOrEmpty(Results.data[i].SALES_ORG))
                                        {
                                            Results.data[i].SALES_ORG = item2;
                                        }
                                        else
                                        {
                                            Results.data[i].SALES_ORG += ", " + item2;
                                        }
                                    }
                                }

                                //ธง
                                if (Results.data[i].MOCKUP_ID > 0)
                                {
                                    var temp2 = allStepArtwork.Where(m => m.STEP_ARTWORK_ID == Results.data[i].CURRENT_STEP_ID).FirstOrDefault();
                                    if (temp2 != null)
                                    {
                                        if (temp2.STEP_ARTWORK_CODE == "SEND_PA")
                                        {
                                            var temp_dashboardWaiting = allDashboardArtwork.Where(m => m.ARTWORK_ITEM_ID == Results.data[i].MOCKUP_ID && string.IsNullOrEmpty(m.IS_END));
                                            Results.data[i].CNT_TOTAL_SUB_WF_NOT_END = temp_dashboardWaiting.Count() - 1;
                                            Results.data[i].WAITING_STEP = "";
                                            temp_dashboardWaiting = temp_dashboardWaiting.OrderBy(m => m.CREATE_DATE).ToList();
                                            foreach (var item in temp_dashboardWaiting)
                                            {
                                                var tempArtwork = allStepArtwork.Where(m => m.STEP_ARTWORK_ID == item.CURRENT_STEP_ID).FirstOrDefault();
                                                if (tempArtwork != null)
                                                {
                                                    if (tempArtwork.STEP_ARTWORK_CODE != "SEND_PA")
                                                    {
                                                        if (item.CURRENT_USER_ID != null)
                                                        {
                                                            if (string.IsNullOrEmpty(Results.data[i].WAITING_STEP))
                                                                Results.data[i].WAITING_STEP += tempArtwork.STEP_ARTWORK_NAME + " [" + CNService.GetUserName(item.CURRENT_USER_ID, listAllUsers) + "]";
                                                            else
                                                                Results.data[i].WAITING_STEP += "<br/>" + tempArtwork.STEP_ARTWORK_NAME + " [" + CNService.GetUserName(item.CURRENT_USER_ID, listAllUsers) + "]";
                                                        }
                                                        else
                                                        {
                                                            if (string.IsNullOrEmpty(Results.data[i].WAITING_STEP))
                                                                Results.data[i].WAITING_STEP += tempArtwork.STEP_ARTWORK_NAME + " [" + msg + "]";
                                                            else
                                                                Results.data[i].WAITING_STEP += "<br/>" + tempArtwork.STEP_ARTWORK_NAME + " [" + msg + "]";
                                                        }
                                                    }
                                                }
                                            }

                                            var temp_dashboardEnd = allDashboardArtwork.Where(m => m.ARTWORK_ITEM_ID == Results.data[i].MOCKUP_ID
                                                                                         && m.IS_END == "X"
                                                                                         && m.CREATE_BY != -1).ToList();

                                            temp_dashboardEnd = temp_dashboardEnd.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                                            Results.data[i].CNT_TOTAL_SUB_WF_END = temp_dashboardEnd.Count();
                                            Results.data[i].END_STEP = "";
                                            temp_dashboardEnd = temp_dashboardEnd.OrderBy(m => m.CREATE_DATE).ToList();
                                            foreach (var item in temp_dashboardEnd)
                                            {
                                                var tempArtwork = allStepArtwork.Where(m => m.STEP_ARTWORK_ID == item.CURRENT_STEP_ID).FirstOrDefault();
                                                if (tempArtwork != null)
                                                {
                                                    if (tempArtwork.STEP_ARTWORK_CODE != "SEND_PA")
                                                    {
                                                        if (item.CURRENT_USER_ID != null)
                                                        {
                                                            if (string.IsNullOrEmpty(Results.data[i].END_STEP))
                                                                Results.data[i].END_STEP += tempArtwork.STEP_ARTWORK_NAME + " [" + CNService.GetUserName(item.CURRENT_USER_ID, listAllUsers) + "]";
                                                            else
                                                                Results.data[i].END_STEP += "<br/>" + tempArtwork.STEP_ARTWORK_NAME + " [" + CNService.GetUserName(item.CURRENT_USER_ID, listAllUsers) + "]";
                                                        }
                                                        else
                                                        {
                                                            if (string.IsNullOrEmpty(Results.data[i].END_STEP))
                                                                Results.data[i].END_STEP += tempArtwork.STEP_ARTWORK_NAME + " [" + msg + "]";
                                                            else
                                                                Results.data[i].END_STEP += "<br/>" + tempArtwork.STEP_ARTWORK_NAME + " [" + msg + "]";
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

            return Results;
        }

        public static V_ART_WF_DASHBOARD_ARTWORK_RESULT GetIncomingArtwork(V_ART_WF_DASHBOARD_ARTWORK_REQUEST param)
        {
            V_ART_WF_DASHBOARD_ARTWORK_RESULT Results = new V_ART_WF_DASHBOARD_ARTWORK_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                        var getByCreateDateFrom = DateTime.Now;
                        var getByCreateDateTo = DateTime.Now;

                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            getByCreateDateFrom = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_FROM);
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            getByCreateDateTo = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_TO);
                        //var q = CNService.BuildIncomingArtwork(param.data.USER_ID, SEND_PA, getByCreateDateFrom, getByCreateDateTo,
                        //    context);
                        //var q = listartwork.AsQueryable().Where(m => m.CURRENT_STEP_ID == SEND_PA && m.CURRENT_USER_ID == param.data.USER_ID
                        //         && !(from o in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL select new { X1 = o.ARTWORK_SUB_ID }).Contains(new { X1 = m.ARTWORK_SUB_ID })).ToList();

                        var q = (from m in context.V_ART_WF_DASHBOARD_ARTWORK
                                 where string.IsNullOrEmpty(m.IS_END)
                                 && m.CURRENT_STEP_ID == SEND_PA
                                 && m.CURRENT_USER_ID == param.data.USER_ID
                                 && !(from o in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL select new { X1 = o.ARTWORK_SUB_ID }).Contains(new { X1 = m.ARTWORK_SUB_ID })
                                 select new V_ART_WF_DASHBOARD_ARTWORK_2()
                                 {
                                     REFERENCE_REQUEST_ID = m.REFERENCE_REQUEST_ID,
                                     REFERENCE_REQUEST_NO = m.REFERENCE_REQUEST_NO,
                                     REFERENCE_REQUEST_TYPE = m.REFERENCE_REQUEST_TYPE,
                                     TYPE_OF_ARTWORK = m.TYPE_OF_ARTWORK,
                                     ARTWORK_REQUEST_NO = m.ARTWORK_REQUEST_NO,
                                     PROJECT_NAME = m.PROJECT_NAME,
                                     TYPE_OF_PRODUCT_ID = m.TYPE_OF_PRODUCT_ID,
                                     REVIEWER_ID = m.REVIEWER_ID,
                                     COMPANY_ID = m.COMPANY_ID,
                                     SOLD_TO_ID = m.SOLD_TO_ID,
                                     SHIP_TO_ID = m.SHIP_TO_ID,
                                     CUSTOMER_OTHER_ID = m.CUSTOMER_OTHER_ID,
                                     PRIMARY_TYPE_ID = m.PRIMARY_TYPE_ID,
                                     PRIMARY_TYPE_OTHER = m.PRIMARY_TYPE_OTHER,
                                     PRIMARY_SIZE_ID = m.PRIMARY_SIZE_ID,
                                     PRIMARY_SIZE_OTHER = m.PRIMARY_SIZE_OTHER,
                                     CONTAINER_TYPE_ID = m.CONTAINER_TYPE_ID,
                                     CONTAINER_TYPE_OTHER = m.CONTAINER_TYPE_OTHER,
                                     LID_TYPE_ID = m.LID_TYPE_ID,
                                     LID_TYPE_OTHER = m.LID_TYPE_OTHER,
                                     PACKING_STYLE_ID = m.PACKING_STYLE_ID,
                                     PACKING_STYLE_OTHER = m.PACKING_STYLE_OTHER,
                                     PACK_SIZE_ID = m.PACK_SIZE_ID,
                                     PACK_SIZE_OTHER = m.PACK_SIZE_OTHER,
                                     BRAND_ID = m.BRAND_ID,
                                     BRAND_OTHER = m.BRAND_OTHER,
                                     REQUEST_DELIVERY_DATE = m.REQUEST_DELIVERY_DATE,
                                     SPECIAL_REQUIREMENT = m.SPECIAL_REQUIREMENT,
                                     OTHER_REQUEST = m.OTHER_REQUEST,
                                     TWO_P_ID = m.TWO_P_ID,
                                     THREE_P_ID = m.THREE_P_ID,
                                     CREATE_DATE_ARTWORK_REQUEST = m.CREATE_DATE_ARTWORK_REQUEST,
                                     CREATE_BY_ARTWORK_REQUEST = m.CREATE_BY_ARTWORK_REQUEST,
                                     ARTWORK_ITEM_ID = m.ARTWORK_ITEM_ID,
                                     REQUEST_ITEM_NO = m.REQUEST_ITEM_NO,
                                     SOLD_TO_DISPLAY_TXT = m.SOLD_TO_DISPLAY_TXT,
                                     SHIP_TO_DISPLAY_TXT = m.SHIP_TO_DISPLAY_TXT,
                                     BRAND_DISPLAY_TXT = m.BRAND_DISPLAY_TXT,
                                     CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                                     CURRENT_USER_ID = m.CURRENT_USER_ID,
                                     CURRENT_ROLE_ID = m.CURRENT_ROLE_ID,
                                     CURRENT_VENDOR_ID = m.CURRENT_VENDOR_ID,
                                     CURRENT_CUSTOMER_ID = m.CURRENT_CUSTOMER_ID,
                                     REMARK = m.REMARK,
                                     IS_END = m.IS_END,
                                     REASON_ID = m.REASON_ID,
                                     CREATE_BY_PROCESS_TITLE = m.CREATE_BY_PROCESS_TITLE,
                                     CREATE_BY_PROCESS_FIRST_NAME = m.CREATE_BY_PROCESS_FIRST_NAME,
                                     CREATE_BY_PROCESS_LAST_NAME = m.CREATE_BY_PROCESS_LAST_NAME,
                                     CURRENT_STEP_DISPLAY_TXT = m.CURRENT_STEP_DISPLAY_TXT,
                                     CREATE_BY_ARTWORK_REQUEST_TITLE = m.CREATE_BY_ARTWORK_REQUEST_TITLE,
                                     CREATE_BY_ARTWORK_REQUEST_FIRST_NAME = m.CREATE_BY_ARTWORK_REQUEST_FIRST_NAME,
                                     CREATE_BY_ARTWORK_REQUEST_LAST_NAME = m.CREATE_BY_ARTWORK_REQUEST_LAST_NAME,
                                     ARTWORK_SUB_ID = m.ARTWORK_SUB_ID,
                                     PARENT_ARTWORK_SUB_ID = m.PARENT_ARTWORK_SUB_ID,
                                     ARTWORK_REQUEST_ID = m.ARTWORK_REQUEST_ID,
                                     CREATE_DATE_PROCESS = m.CREATE_DATE_PROCESS,
                                     CREATE_BY_PROCESS = m.CREATE_BY_PROCESS,
                                     REMARK_TERMINATE = m.REMARK_TERMINATE,
                                     IS_DELEGATE = m.IS_DELEGATE,
                                     IS_TERMINATE = m.IS_TERMINATE,
                                     DURATION = m.DURATION,
                                     DURATION_EXTEND = m.DURATION_EXTEND,
                                     STEP_ARTWORK_DESCRIPTION = m.STEP_ARTWORK_DESCRIPTION,
                                     STEP_ARTWORK_CODE = m.STEP_ARTWORK_CODE,
                                     ROLE_ID_RESPONSE = m.ROLE_ID_RESPONSE,
                                     REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                                     IS_STEP_DURATION_EXTEND = m.IS_STEP_DURATION_EXTEND,
                                 });

                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            q = q.Where(m => DbFunctions.TruncateTime(m.CREATE_DATE_PROCESS) >= getByCreateDateFrom.Date);
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            q = q.Where(m => DbFunctions.TruncateTime(m.CREATE_DATE_PROCESS) <= getByCreateDateTo.Date);

                        var temp = q.ToList();
                        foreach (var t in temp)
                        {

                            var allListPlant = (from p2 in context.ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT
                                                 join s2 in context.SAP_M_PLANT on p2.PRODUCTION_PLANT_ID equals s2.PLANT_ID into ps
                                                 from t2 in ps.DefaultIfEmpty()
                                                 where p2.ARTWORK_REQUEST_ID == t.ARTWORK_REQUEST_ID
                                                 select t2.PLANT ).ToList();
                            foreach (var item2 in allListPlant.ToList())
                            {
                                if (string.IsNullOrEmpty(t.PLANT)){
                                    t.PLANT = string.Format("{0}",item2);
                                }
                                else
                                {
                                    t.PLANT += ", " + string.Format("{0}", item2);
                                }
                            }
                        }
                        //Results.data = MapperServices.V_ART_WF_DASHBOARD_ARTWORK(temp);
                        Results.data = temp;
                    }
                }


                if (param != null)
                {
                    Results.draw = param.draw;
                }

                Results = BindDataIncomingArtwork(Results);

                Results.recordsFiltered = Results.data.ToList().Count;
                Results.recordsTotal = Results.data.ToList().Count;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static SAP_M_PO_IDOC_RESULT GetPOViewForVendor(SAP_M_PO_IDOC_REQUEST param)
        {
            SAP_M_PO_IDOC_RESULT Results = new SAP_M_PO_IDOC_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var getByCreateDateFrom = DateTime.Now;
                        var getByCreateDateTo = DateTime.Now;

                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            getByCreateDateFrom = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_FROM);
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            getByCreateDateTo = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_TO);

                        var stepPP = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_VN_PO").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();

                        var q = (from p in context.ART_WF_ARTWORK_PROCESS
                                 where p.CURRENT_STEP_ID == stepPP
                                    && p.IS_END != "X"
                                    && p.CURRENT_USER_ID == param.data.CURRENT_USER_ID
                                 select p);

                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            q = q.Where(m => DbFunctions.TruncateTime(m.CREATE_DATE) >= getByCreateDateFrom.Date);
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            q = q.Where(m => DbFunctions.TruncateTime(m.CREATE_DATE) <= getByCreateDateTo.Date);

                        var processes = q.ToList();

                        if (processes != null && processes.Count > 0)
                        {
                            var requestItemIDs = processes.Select(w => w.ARTWORK_ITEM_ID).ToList();

                            var requestItem = (from r in context.ART_WF_ARTWORK_REQUEST_ITEM
                                               where requestItemIDs.Contains(r.ARTWORK_ITEM_ID)
                                               select r).ToList();

                            if (requestItem != null && requestItem.Count > 0)
                            {
                                var requestItemNOs = requestItem.Select(s => s.REQUEST_ITEM_NO).Distinct().ToList();

                                var mappingAWs = (from a in context.ART_WF_ARTWORK_MAPPING_PO
                                                  where requestItemNOs.Contains(a.ARTWORK_NO)
                                                    && !String.IsNullOrEmpty(a.ARTWORK_NO)
                                                    && a.IS_ACTIVE == "X"
                                                  select a).ToList();

                                if (mappingAWs != null && mappingAWs.Count > 0)
                                {
                                    var mappingPOs = mappingAWs.Select(s => s.PO_NO).Distinct().ToList();

                                    var poIDOCs = (from p in context.SAP_M_PO_IDOC
                                                   where mappingPOs.Contains(p.PURCHASE_ORDER_NO)
                                                   select p).Select(s => s.PURCHASE_ORDER_NO).Distinct().ToList();

                                    if (poIDOCs != null && poIDOCs.Count > 0)
                                    {
                                        List<SAP_M_PO_IDOC> listPO = new List<SAP_M_PO_IDOC>();

                                        foreach (var item in poIDOCs)
                                        {
                                            var po = (from p in context.SAP_M_PO_IDOC
                                                      where p.PURCHASE_ORDER_NO == item
                                                      select p).OrderByDescending(o => o.DATE).ThenByDescending(o => o.TIME).FirstOrDefault();

                                            if (po != null)
                                            {
                                                listPO.Add(po);
                                            }
                                        }

                                        Results.data = MapperServices.SAP_M_PO_IDOC(listPO);
                                    }
                                }
                            }
                        }
                    }
                }

                if (param != null)
                {
                    Results.draw = param.draw;
                }

                if (Results.data != null)
                {
                    foreach (var item in Results.data)
                    {
                        item.PO_NO2 = EncryptionService.EncryptAndUrlEncode(item.PURCHASE_ORDER_NO);
                    }

                    Results.recordsFiltered = Results.data.ToList().Count;
                    Results.recordsTotal = Results.data.ToList().Count;
                }
                else
                {
                    List<SAP_M_PO_IDOC_2> emptyArray = new List<SAP_M_PO_IDOC_2>();
                    Results.data = emptyArray;
                    Results.recordsFiltered = 0;
                    Results.recordsTotal = 0;
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

        public static V_ART_WF_DASHBOARD_ARTWORK_RESULT GetIncomingArtworkForPG(V_ART_WF_DASHBOARD_ARTWORK_REQUEST param)
        {
            V_ART_WF_DASHBOARD_ARTWORK_RESULT Results = new V_ART_WF_DASHBOARD_ARTWORK_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var SEND_PG = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                        var getByCreateDateFrom = DateTime.Now;
                        var getByCreateDateTo = DateTime.Now;

                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            getByCreateDateFrom = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_FROM);
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            getByCreateDateTo = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_TO);
                        var q = context.Database.SqlQuery<V_ART_WF_DASHBOARD_ARTWORK_2>("sp_ART_WF_DASHBOARD_ARTWORK_PG @STEP, @From, @To",
                        new SqlParameter("@STEP", SEND_PG), new SqlParameter("@From", getByCreateDateFrom), new SqlParameter("@To", getByCreateDateTo)).ToList();
                        //var q = qlist.AsQueryable().Where(m => m.CURRENT_USER_ID == null).ToList();
                        //var q = (from m in context.V_ART_WF_DASHBOARD_ARTWORK
                        //         where string.IsNullOrEmpty(m.IS_END)
                        //         && m.CURRENT_STEP_ID == SEND_PG
                        //         && m.CURRENT_USER_ID == null
                        //         select new V_ART_WF_DASHBOARD_ARTWORK_2()
                        //         {
                        //             REFERENCE_REQUEST_ID = m.REFERENCE_REQUEST_ID,
                        //             REFERENCE_REQUEST_NO = m.REFERENCE_REQUEST_NO,
                        //             REFERENCE_REQUEST_TYPE = m.REFERENCE_REQUEST_TYPE,
                        //             TYPE_OF_ARTWORK = m.TYPE_OF_ARTWORK,
                        //             ARTWORK_REQUEST_NO = m.ARTWORK_REQUEST_NO,
                        //             PROJECT_NAME = m.PROJECT_NAME,
                        //             TYPE_OF_PRODUCT_ID = m.TYPE_OF_PRODUCT_ID,
                        //             REVIEWER_ID = m.REVIEWER_ID,
                        //             COMPANY_ID = m.COMPANY_ID,
                        //             SOLD_TO_ID = m.SOLD_TO_ID,
                        //             SHIP_TO_ID = m.SHIP_TO_ID,
                        //             CUSTOMER_OTHER_ID = m.CUSTOMER_OTHER_ID,
                        //             PRIMARY_TYPE_ID = m.PRIMARY_TYPE_ID,
                        //             PRIMARY_TYPE_OTHER = m.PRIMARY_TYPE_OTHER,
                        //             PRIMARY_SIZE_ID = m.PRIMARY_SIZE_ID,
                        //             PRIMARY_SIZE_OTHER = m.PRIMARY_SIZE_OTHER,
                        //             CONTAINER_TYPE_ID = m.CONTAINER_TYPE_ID,
                        //             CONTAINER_TYPE_OTHER = m.CONTAINER_TYPE_OTHER,
                        //             LID_TYPE_ID = m.LID_TYPE_ID,
                        //             LID_TYPE_OTHER = m.LID_TYPE_OTHER,
                        //             PACKING_STYLE_ID = m.PACKING_STYLE_ID,
                        //             PACKING_STYLE_OTHER = m.PACKING_STYLE_OTHER,
                        //             PACK_SIZE_ID = m.PACK_SIZE_ID,
                        //             PACK_SIZE_OTHER = m.PACK_SIZE_OTHER,
                        //             BRAND_ID = m.BRAND_ID,
                        //             BRAND_OTHER = m.BRAND_OTHER,
                        //             REQUEST_DELIVERY_DATE = m.REQUEST_DELIVERY_DATE,
                        //             SPECIAL_REQUIREMENT = m.SPECIAL_REQUIREMENT,
                        //             OTHER_REQUEST = m.OTHER_REQUEST,
                        //             TWO_P_ID = m.TWO_P_ID,
                        //             THREE_P_ID = m.THREE_P_ID,
                        //             CREATE_DATE_ARTWORK_REQUEST = m.CREATE_DATE_ARTWORK_REQUEST,
                        //             CREATE_BY_ARTWORK_REQUEST = m.CREATE_BY_ARTWORK_REQUEST,
                        //             ARTWORK_ITEM_ID = m.ARTWORK_ITEM_ID,
                        //             REQUEST_ITEM_NO = m.REQUEST_ITEM_NO,
                        //             SOLD_TO_DISPLAY_TXT = m.SOLD_TO_DISPLAY_TXT,
                        //             SHIP_TO_DISPLAY_TXT = m.SHIP_TO_DISPLAY_TXT,
                        //             BRAND_DISPLAY_TXT = m.BRAND_DISPLAY_TXT,
                        //             CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                        //             CURRENT_USER_ID = m.CURRENT_USER_ID,
                        //             CURRENT_ROLE_ID = m.CURRENT_ROLE_ID,
                        //             CURRENT_VENDOR_ID = m.CURRENT_VENDOR_ID,
                        //             CURRENT_CUSTOMER_ID = m.CURRENT_CUSTOMER_ID,
                        //             REMARK = m.REMARK,
                        //             IS_END = m.IS_END,
                        //             REASON_ID = m.REASON_ID,
                        //             CREATE_BY_PROCESS_TITLE = m.CREATE_BY_PROCESS_TITLE,
                        //             CREATE_BY_PROCESS_FIRST_NAME = m.CREATE_BY_PROCESS_FIRST_NAME,
                        //             CREATE_BY_PROCESS_LAST_NAME = m.CREATE_BY_PROCESS_LAST_NAME,
                        //             CURRENT_STEP_DISPLAY_TXT = m.CURRENT_STEP_DISPLAY_TXT,
                        //             CREATE_BY_ARTWORK_REQUEST_TITLE = m.CREATE_BY_ARTWORK_REQUEST_TITLE,
                        //             CREATE_BY_ARTWORK_REQUEST_FIRST_NAME = m.CREATE_BY_ARTWORK_REQUEST_FIRST_NAME,
                        //             CREATE_BY_ARTWORK_REQUEST_LAST_NAME = m.CREATE_BY_ARTWORK_REQUEST_LAST_NAME,
                        //             ARTWORK_SUB_ID = m.ARTWORK_SUB_ID,
                        //             PARENT_ARTWORK_SUB_ID = m.PARENT_ARTWORK_SUB_ID,
                        //             ARTWORK_REQUEST_ID = m.ARTWORK_REQUEST_ID,
                        //             CREATE_DATE_PROCESS = m.CREATE_DATE_PROCESS,
                        //             CREATE_BY_PROCESS = m.CREATE_BY_PROCESS,
                        //             REMARK_TERMINATE = m.REMARK_TERMINATE,
                        //             IS_DELEGATE = m.IS_DELEGATE,
                        //             IS_TERMINATE = m.IS_TERMINATE,
                        //             DURATION = m.DURATION,
                        //             DURATION_EXTEND = m.DURATION_EXTEND,
                        //             STEP_ARTWORK_DESCRIPTION = m.STEP_ARTWORK_DESCRIPTION,
                        //             STEP_ARTWORK_CODE = m.STEP_ARTWORK_CODE,
                        //             ROLE_ID_RESPONSE = m.ROLE_ID_RESPONSE,
                        //             REMARK_KILLPROCESS = m.REMARK_KILLPROCESS,
                        //             IS_STEP_DURATION_EXTEND = m.IS_STEP_DURATION_EXTEND,
                        //         });

                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            q = q.AsQueryable().Where(m => Convert.ToDateTime(m.CREATE_DATE_PROCESS).Date >= getByCreateDateFrom.Date).ToList();
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            q = q.AsQueryable().Where(m => Convert.ToDateTime(m.CREATE_DATE_PROCESS).Date <= getByCreateDateTo.Date).ToList();

                        var temp = q.ToList();

                        //Results.data = MapperServices.V_ART_WF_DASHBOARD_ARTWORK(temp);
                        Results.data = temp;
                    }
                }

                if (param != null)
                {
                    Results.draw = param.draw;
                }

                Results = BindDataIncomingArtwork(Results);

                Results.recordsFiltered = Results.data.ToList().Count;
                Results.recordsTotal = Results.data.ToList().Count;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        private static V_ART_WF_DASHBOARD_ARTWORK_RESULT BindDataIncomingArtwork(V_ART_WF_DASHBOARD_ARTWORK_RESULT Results)
        {
            if (Results.data.Count > 0)
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var msg = MessageHelper.GetMessage("MSG_005");
                        var allStepArtwork = ART_M_STEP_ARTWORK_SERVICE.GetAll(context);
                        var listAllUsers = ART_M_USER_SERVICE.GetAll(context);

                        var listRequestId = Results.data.Select(m => m.ARTWORK_REQUEST_ID).Distinct().ToList();
                        var listItemId = Results.data.Select(m => m.ARTWORK_ITEM_ID).Distinct().ToList();
                        var allDashboardArtwork = (from m in context.ART_WF_ARTWORK_PROCESS where listItemId.Contains(m.ARTWORK_ITEM_ID) select m).ToList();

                        var SEND_PP = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_PP").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_PA = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_PA").FirstOrDefault().STEP_ARTWORK_ID;

                        var list_country = SAP_M_COUNTRY_SERVICE.GetAll(context);
                        var listAllRequestCountry = (from m in context.ART_WF_ARTWORK_REQUEST_COUNTRY where listRequestId.Contains(m.ARTWORK_REQUEST_ID) select m).ToList();
                        var listAllRequestProduct = (from m in context.ART_WF_ARTWORK_REQUEST_PRODUCT where listRequestId.Contains(m.ARTWORK_REQUEST_ID) select m).ToList();

                        var listRequestProductId = listAllRequestProduct.Select(m => m.PRODUCT_CODE_ID).ToList();
                        var listAllProduct = (from m in context.XECM_M_PRODUCT where listRequestProductId.Contains(m.XECM_PRODUCT_ID) select m).ToList();

                        var listSubIdPP = allDashboardArtwork.Where(m => m.CURRENT_STEP_ID == SEND_PP).Select(m => m.ARTWORK_SUB_ID).ToList();
                        var listAllProcessPP = (from m in context.ART_WF_ARTWORK_PROCESS_PP where listSubIdPP.Contains(m.ARTWORK_SUB_ID) select m).ToList();
                        var listAllReason = (from m in context.ART_M_DECISION_REASON select m).ToList();
                        var listAllReasonOther = (from m in context.ART_WF_REMARK_REASON_OTHER where m.WF_TYPE == "A" && listSubIdPP.Contains((int)m.WF_SUB_ID) select m).ToList();

                        //start #INC-65918 by aof 20220711
                        var lisSubidPA = allDashboardArtwork.Where(m => m.CURRENT_STEP_ID == SEND_PA).Select(m => m.ARTWORK_SUB_ID).ToList();
                        var listAllProcessPA = (from m in context.ART_WF_ARTWORK_PROCESS_PA where lisSubidPA.Contains(m.ARTWORK_SUB_ID) select m).ToList();
                        //end #INC-65918 by aof 20220711

                        for (int i = 0; i < Results.data.Count; i++)
                        {
                            //
                          

                            //ART_WF_ARTWORK_REQUEST_COUNTRY chklist_country = new ART_WF_ARTWORK_REQUEST_COUNTRY();
                            //chklist_country.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;
                            //var tempCountry = MapperServices.ART_WF_ARTWORK_REQUEST_COUNTRY(ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.GetByItem(chklist_country, context));
                            var tempCountry = listAllRequestCountry.Where(m => m.ARTWORK_REQUEST_ID == Results.data[i].ARTWORK_REQUEST_ID).ToList();

                            var country = tempCountry.Join(
                                list_country,
                                lc => lc.COUNTRY_ID,
                                c => c.COUNTRY_ID,
                                (lc, c) => new
                                {
                                    COUNTRY_ID = lc.COUNTRY_ID,
                                    COUNTRY_CODE = c.COUNTRY_CODE
                                }
                            );
                            Results.data[i].COUNTRY_CODE_SET = country.Aggregate("", (a, b) => a + ((a.Length > 0 && b.COUNTRY_CODE != null && b.COUNTRY_CODE.Length > 0) ? ", " : "") + b.COUNTRY_CODE);

                            //ART_WF_ARTWORK_REQUEST_PRODUCT product = new ART_WF_ARTWORK_REQUEST_PRODUCT();
                            //product.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;
                            //var tempProduct = MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCT(ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(product, context));
                            var tempProduct = listAllRequestProduct.Where(m => m.ARTWORK_REQUEST_ID == Results.data[i].ARTWORK_REQUEST_ID).ToList();

                            foreach (var product2 in tempProduct)
                            {
                                var temp = listAllProduct.Where(m => m.XECM_PRODUCT_ID == product2.PRODUCT_CODE_ID).FirstOrDefault();
                                if (temp != null)
                                {
                                    if (string.IsNullOrEmpty(Results.data[i].PRODUCT_CODE_SET))
                                    {
                                        Results.data[i].PRODUCT_CODE_SET = temp.PRODUCT_CODE;
                                    }
                                    else
                                    {
                                        Results.data[i].PRODUCT_CODE_SET += ", " + temp.PRODUCT_CODE;
                                    }
                                }
                            }

                            Results.data[i].CREATE_BY_PROCESS_DISPLAY_TXT = Results.data[i].CREATE_BY_PROCESS_TITLE + " " + Results.data[i].CREATE_BY_PROCESS_FIRST_NAME + " " + Results.data[i].CREATE_BY_PROCESS_LAST_NAME;
                            Results.data[i].CREATE_BY_ARTWORK_REQUEST_DISPLAY_TXT = Results.data[i].CREATE_BY_ARTWORK_REQUEST_TITLE + " " + Results.data[i].CREATE_BY_ARTWORK_REQUEST_FIRST_NAME + " " + Results.data[i].CREATE_BY_ARTWORK_REQUEST_LAST_NAME;

                            //start #INC-65918 by aof 20220711
                            var tempPA = listAllProcessPA.Where(m => m.ARTWORK_SUB_ID == Results.data[i].PARENT_ARTWORK_SUB_ID || m.ARTWORK_SUB_ID == Results.data[i].ARTWORK_SUB_ID).FirstOrDefault();
                            if (tempPA != null)
                            {

                                Results.data[i].PACKING_TYPE_DISPLAY_TXT = CNService.GetCharacteristicCodeAndDescription(tempPA.MATERIAL_GROUP_ID, context);
                            }
                            //end #INC-65918 by aof 20220711

                            if (Results.data[i].CURRENT_STEP_ID == SEND_PA)
                            {
                                var lastProcessPP = allDashboardArtwork.Where(m => m.ARTWORK_ITEM_ID == Results.data[i].ARTWORK_ITEM_ID && m.CURRENT_STEP_ID == SEND_PP).OrderByDescending(m => m.CREATE_DATE).ToList();
                                if (lastProcessPP.Count > 0)
                                {
                                    var tempLastProcessPP = lastProcessPP.FirstOrDefault();
                                    if (tempLastProcessPP != null)
                                    {
                                        var processPP = listAllProcessPP.Where(m => m.ARTWORK_SUB_ID == tempLastProcessPP.ARTWORK_SUB_ID).FirstOrDefault();
                                        if (processPP != null)
                                        {
                                            if (processPP.ACTION_CODE == "SEND_BACK")
                                            {
                                                Results.data[i].PP_SEND_BACK = true;

                                                var PP_SEND_BACK_COMMENT = "";
                                                var reason = listAllReason.Where(m => m.ART_M_DECISION_REASON_ID == processPP.REASON_ID).FirstOrDefault();
                                                if (reason != null)
                                                {
                                                    PP_SEND_BACK_COMMENT = reason.DESCRIPTION;
                                                }

                                                var reasonOther = listAllReasonOther.Where(m => m.WF_SUB_ID == processPP.ARTWORK_SUB_ID).FirstOrDefault();
                                                if (reasonOther != null)
                                                {
                                                    if (string.IsNullOrEmpty(PP_SEND_BACK_COMMENT))
                                                        PP_SEND_BACK_COMMENT = reasonOther.REMARK_REASON;
                                                    else
                                                        PP_SEND_BACK_COMMENT += ", " + reasonOther.REMARK_REASON;
                                                }

                                                if (string.IsNullOrEmpty(PP_SEND_BACK_COMMENT))
                                                    PP_SEND_BACK_COMMENT = processPP.COMMENT;
                                                else
                                                    PP_SEND_BACK_COMMENT += ", " + processPP.COMMENT;

                                                Results.data[i].PP_SEND_BACK_COMMENT = PP_SEND_BACK_COMMENT;
                                            }
                                        }
                                    }
                                }
                            }

                            //ธง
                            if (Results.data[i].ARTWORK_ITEM_ID > 0)
                            {
                                var temp2 = allStepArtwork.Where(m => m.STEP_ARTWORK_ID == Results.data[i].CURRENT_STEP_ID).FirstOrDefault();
                                if (temp2 != null)
                                {
                                    if (temp2.STEP_ARTWORK_CODE == "SEND_PA")
                                    {
                                        var temp_dashboardWaiting = allDashboardArtwork.Where(m => m.ARTWORK_ITEM_ID == Results.data[i].ARTWORK_ITEM_ID && string.IsNullOrEmpty(m.IS_END));
                                        Results.data[i].CNT_TOTAL_SUB_WF_NOT_END = temp_dashboardWaiting.Count() - 1;
                                        Results.data[i].WAITING_STEP = "";
                                        temp_dashboardWaiting = temp_dashboardWaiting.OrderBy(m => m.CREATE_DATE).ToList();
                                        foreach (var item in temp_dashboardWaiting)
                                        {
                                            var tempArtwork = allStepArtwork.Where(m => m.STEP_ARTWORK_ID == item.CURRENT_STEP_ID).FirstOrDefault();
                                            if (tempArtwork != null)
                                            {
                                                if (tempArtwork.STEP_ARTWORK_CODE != "SEND_PA")
                                                {
                                                    if (item.CURRENT_USER_ID != null)
                                                    {
                                                        if (string.IsNullOrEmpty(Results.data[i].WAITING_STEP))
                                                            Results.data[i].WAITING_STEP += tempArtwork.STEP_ARTWORK_NAME + " [" + CNService.GetUserName(item.CURRENT_USER_ID, listAllUsers) + "]";
                                                        else
                                                            Results.data[i].WAITING_STEP += "<br/>" + tempArtwork.STEP_ARTWORK_NAME + " [" + CNService.GetUserName(item.CURRENT_USER_ID, listAllUsers) + "]";
                                                    }
                                                    else
                                                    {
                                                        if (string.IsNullOrEmpty(Results.data[i].WAITING_STEP))
                                                            Results.data[i].WAITING_STEP += tempArtwork.STEP_ARTWORK_NAME + " [" + msg + "]";
                                                        else
                                                            Results.data[i].WAITING_STEP += "<br/>" + tempArtwork.STEP_ARTWORK_NAME + " [" + msg + "]";
                                                    }
                                                }
                                            }
                                        }

                                        var temp_dashboardEnd = allDashboardArtwork.Where(m => m.ARTWORK_ITEM_ID == Results.data[i].ARTWORK_ITEM_ID
                                                                                     && m.IS_END == "X"
                                                                                     && m.CREATE_BY != -1).ToList();

                                        temp_dashboardEnd = temp_dashboardEnd.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                                        Results.data[i].CNT_TOTAL_SUB_WF_END = temp_dashboardEnd.Count();
                                        Results.data[i].END_STEP = "";
                                        temp_dashboardEnd = temp_dashboardEnd.OrderBy(m => m.CREATE_DATE).ToList();
                                        foreach (var item in temp_dashboardEnd)
                                        {
                                            var tempArtwork = allStepArtwork.Where(m => m.STEP_ARTWORK_ID == item.CURRENT_STEP_ID).FirstOrDefault();
                                            if (tempArtwork != null)
                                            {
                                                if (tempArtwork.STEP_ARTWORK_CODE != "SEND_PA")
                                                {
                                                    if (item.CURRENT_USER_ID != null)
                                                    {
                                                        if (string.IsNullOrEmpty(Results.data[i].END_STEP))
                                                            Results.data[i].END_STEP += tempArtwork.STEP_ARTWORK_NAME + " [" + CNService.GetUserName(item.CURRENT_USER_ID, listAllUsers) + "]";
                                                        else
                                                            Results.data[i].END_STEP += "<br/>" + tempArtwork.STEP_ARTWORK_NAME + " [" + CNService.GetUserName(item.CURRENT_USER_ID, listAllUsers) + "]";
                                                    }
                                                    else
                                                    {
                                                        if (string.IsNullOrEmpty(Results.data[i].END_STEP))
                                                            Results.data[i].END_STEP += tempArtwork.STEP_ARTWORK_NAME + " [" + msg + "]";
                                                        else
                                                            Results.data[i].END_STEP += "<br/>" + tempArtwork.STEP_ARTWORK_NAME + " [" + msg + "]";
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

            return Results;
        }

        public static DASHBOARD_GRAPH_MODEL CountMockupAndArtworkTransaction(V_ART_WF_DASHBOARD_REQUEST param)
        {
            DASHBOARD_GRAPH_MODEL Results = new DASHBOARD_GRAPH_MODEL();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var allStepMockup = ART_M_STEP_MOCKUP_SERVICE.GetAll(context);
                        var allStepArtwork = ART_M_STEP_ARTWORK_SERVICE.GetAll(context);
                        var listUserCompany = ART_M_USER_COMPANY_SERVICE.GetByItem(new ART_M_USER_COMPANY() { USER_ID = Convert.ToInt32(param.data.USER_ID) }, context);
                        var listUserTypeofProduct = ART_M_USER_TYPE_OF_PRODUCT_SERVICE.GetByItem(new ART_M_USER_TYPE_OF_PRODUCT() { USER_ID = Convert.ToInt32(param.data.USER_ID) }, context);

                        var SEND_RD_PRI_PKG = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_RD_PRI_PKG").FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_PN_PRI_PKG = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_PN_PRI_PKG").FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_VN_QUO = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_VN_QUO").FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_VN_MB = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_VN_MB").FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_VN_DL = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_VN_DL").FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_VN_RS = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_VN_RS").FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_VN_PR = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_VN_PR").FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_CUS_APP = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_CUS_APP").FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_PG = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_PG").FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_APP_MATCH_BOARD = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_APP_MATCH_BOARD").FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_PG_SUP_SEL_VENDOR = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_PG_SUP_SEL_VENDOR").FirstOrDefault().STEP_MOCKUP_ID;
                        var SEND_WH_TEST_PACK = allStepMockup.Where(m => m.STEP_MOCKUP_CODE == "SEND_WH_TEST_PACK").FirstOrDefault().STEP_MOCKUP_ID;

                        var listUserRole = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { USER_ID = param.data.USER_ID }, context);

                        var listAllMockup = (from p in context.ART_WF_MOCKUP_PROCESS
                                             where string.IsNullOrEmpty(p.IS_END)
                                             select p).ToList();

                        var listAllARTWORK = (from p in context.ART_WF_ARTWORK_PROCESS
                                              where string.IsNullOrEmpty(p.IS_END)
                                              select p).ToList();

                        var MockupPool = listAllMockup.Where(m => m.CURRENT_USER_ID == null
                                && string.IsNullOrEmpty(m.IS_END)
                                && listUserRole.Select(o => o.ROLE_ID).ToList().Contains(Convert.ToInt32(m.CURRENT_ROLE_ID))).ToList();

                        var ArtworkPool = listAllARTWORK.Where(m => m.CURRENT_USER_ID == null
                               && string.IsNullOrEmpty(m.IS_END)
                               && listUserRole.Select(o => o.ROLE_ID).ToList().Contains(Convert.ToInt32(m.CURRENT_ROLE_ID))).ToList();

                        var TempMockupPool = new List<ART_WF_MOCKUP_PROCESS>();
                        foreach (var item in MockupPool)
                        {
                            if (item.MOCKUP_ID > 0)
                            {
                                var checkListId = CNService.ConvertMockupIdToCheckListId(item.MOCKUP_ID, context);
                                if (checkListId > 0)
                                {
                                    var valid = CNService.CheckTypeOfProductAndCompanyMockup(param.data.USER_ID, checkListId, item.MOCKUP_SUB_ID, context, allStepMockup, listUserCompany, listUserTypeofProduct);
                                    if (valid)
                                    {
                                        TempMockupPool.Add(item);
                                    }
                                }
                            }
                        }
                        MockupPool = TempMockupPool;

                        var SEND_PA = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_PA").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_QC = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_QC").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_RD = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_RD").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_WH = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_WH").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_PN = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_PN").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_VN_PM = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_VN_PM").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_VN_SL = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_VN_SL").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_CUS_PRINT = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_CUS_PRINT").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_CUS_SHADE = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_CUS_SHADE").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_GM_MK = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_GM_MK").FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_CUS_REVIEW = allStepArtwork.Where(m => m.STEP_ARTWORK_CODE == "SEND_CUS_REVIEW").FirstOrDefault().STEP_ARTWORK_ID;

                        var TempArtworkPool = new List<ART_WF_ARTWORK_PROCESS>();
                        foreach (var item in ArtworkPool)
                        {
                            var valid = CNService.CheckTypeOfProductAndCompanyArtwork(param.data.USER_ID, item.ARTWORK_REQUEST_ID, item.ARTWORK_SUB_ID, context, allStepArtwork, listUserCompany, listUserTypeofProduct);
                            if (valid)
                            {
                                TempArtworkPool.Add(item);
                            }
                        }
                        ArtworkPool = TempArtworkPool;

                        var MockupInprogress = listAllMockup.Where(m => (m.CURRENT_USER_ID == param.data.USER_ID) && string.IsNullOrEmpty(m.IS_END)
                            ).Select(m => m.MOCKUP_ID).Distinct().ToList();
                        var ArtworkInprogress = listAllARTWORK.Where(m => (m.CURRENT_USER_ID == param.data.USER_ID) && string.IsNullOrEmpty(m.IS_END)
                            ).Select(m => m.ARTWORK_ITEM_ID).Distinct().ToList();

                        Results.cntInProgress = MockupInprogress.Count + ArtworkInprogress.Count;
                        Results.cntIncoming = MockupPool.Count + ArtworkPool.Count;

                        Results.cntIncoming_txt = "<div style='margin:3px;padding:3px;'>";
                        Results.cntIncoming_txt += "<div style='white-space: nowrap;font-weight:bold;'>Incoming " + Results.cntIncoming + "</div>";
                        Results.cntIncoming_txt += "</div>";

                        Results.cntInProgress_txt = "<div style='margin:3px;padding:3px;'>";
                        Results.cntInProgress_txt += "<div style='white-space: nowrap;font-weight:bold;'>In progress " + (MockupInprogress.Count + ArtworkInprogress.Count) + "</div>";
                        Results.cntInProgress_txt += "</div>";

                        var listMOCKUP_ID = listAllMockup.Where(m => m.CURRENT_USER_ID == param.data.USER_ID).Select(m => m.MOCKUP_ID).Distinct().ToList();
                        var checklist = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST() { CREATOR_ID = param.data.USER_ID }, context);
                        foreach (var itemChecklist in checklist)
                        {
                            var checklistItem = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_ITEM() { CHECK_LIST_ID = itemChecklist.CHECK_LIST_ID }, context);
                            foreach (var itemChecklist2 in checklistItem)
                            {
                                listMOCKUP_ID.Add(itemChecklist2.MOCKUP_ID);
                            }
                        }
                        listMOCKUP_ID = listMOCKUP_ID.Distinct().ToList();
                        var listAllMockup3 = listAllMockup.Where(m => listMOCKUP_ID.Contains(m.MOCKUP_ID)).ToList();
                        var listAllMockup2 = new List<ART_WF_MOCKUP_PROCESS>();
                        foreach (var item in listAllMockup3)
                        {
                            if (item.CURRENT_CUSTOMER_ID > 0)
                            {
                                var checklistId = CNService.ConvertMockupIdToCheckListId(item.MOCKUP_ID, context);
                                var mailCus = ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER() { CHECK_LIST_ID = checklistId }, context);
                                if (mailCus.Where(m => m.CUSTOMER_USER_ID == item.CURRENT_USER_ID && m.MAIL_CC == "X").ToList().Count == 0)
                                {
                                    listAllMockup2.Add(item);
                                }
                            }
                            else
                                listAllMockup2.Add(item);
                        }
                        Results.cntWaitingPKG = listAllMockup2.Where(m => (m.CURRENT_STEP_ID == SEND_RD_PRI_PKG || m.CURRENT_STEP_ID == SEND_PN_PRI_PKG) && string.IsNullOrEmpty(m.IS_END)).Select(m => m.MOCKUP_ID).Distinct().ToList().Count;
                        Results.cntWaitingQuo = listAllMockup2.Where(m => m.CURRENT_STEP_ID == SEND_VN_QUO && string.IsNullOrEmpty(m.IS_END)).Select(m => m.MOCKUP_ID).Distinct().ToList().Count;
                        Results.cntWaitingSample = listAllMockup2.Where(m => (m.CURRENT_STEP_ID == SEND_VN_MB
                          || m.CURRENT_STEP_ID == SEND_VN_DL
                          || m.CURRENT_STEP_ID == SEND_VN_RS
                          || m.CURRENT_STEP_ID == SEND_VN_PR) && string.IsNullOrEmpty(m.IS_END)).Select(m => m.MOCKUP_ID).Distinct().ToList().Count;

                        Results.cntWaitingCustomer = listAllMockup2.Where(m => m.CURRENT_STEP_ID == SEND_CUS_APP && string.IsNullOrEmpty(m.IS_END)).Select(m => m.MOCKUP_ID).Distinct().ToList().Count;

                        Results.cntWaitingPKG_txt = "<div style='margin:3px;padding:3px;'>";
                        Results.cntWaitingPKG_txt += "<div style='white-space: nowrap;font-weight:bold;'>Waiting for primary PKG " + (Results.cntWaitingPKG) + "</div>";
                        Results.cntWaitingPKG_txt += "</div>";

                        Results.cntWaitingQuo_txt = "<div style='margin:3px;padding:3px;'>";
                        Results.cntWaitingQuo_txt += "<div style='white-space: nowrap;font-weight:bold;'>Waiting for quotation " + (Results.cntWaitingQuo) + "</div>";
                        Results.cntWaitingQuo_txt += "</div>";

                        Results.cntWaitingSample_txt = "<div style='margin:3px;padding:3px;'>";
                        Results.cntWaitingSample_txt += "<div style='white-space: nowrap;font-weight:bold;'>Waiting for sample " + (Results.cntWaitingSample) + "</div>";
                        Results.cntWaitingSample_txt += "</div>";

                        Results.cntWaitingCustomer_txt = "<div style='margin:3px;padding:3px;'>";
                        Results.cntWaitingCustomer_txt += "<div style='white-space: nowrap;font-weight:bold;'>Waiting for customer approval (mockup) " + (Results.cntWaitingCustomer) + "</div>";
                        Results.cntWaitingCustomer_txt += "</div>";

                        // artwork
                        var listARTWORK_ITEM_ID = listAllARTWORK.Where(m => m.CURRENT_USER_ID == param.data.USER_ID).Select(m => m.ARTWORK_ITEM_ID).Distinct().ToList();
                        var artworkRequest = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { CREATOR_ID = param.data.USER_ID }, context);
                        foreach (var itemartworkRequest in artworkRequest)
                        {
                            var itemartworkRequestItem = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_ITEM() { ARTWORK_REQUEST_ID = itemartworkRequest.ARTWORK_REQUEST_ID }, context);
                            foreach (var itemartworkRequestItem2 in itemartworkRequestItem)
                            {
                                listARTWORK_ITEM_ID.Add(itemartworkRequestItem2.ARTWORK_ITEM_ID);
                            }
                        }
                        listARTWORK_ITEM_ID = listARTWORK_ITEM_ID.Distinct().ToList();

                        var listAllARTWORK3 = listAllARTWORK.Where(m => listARTWORK_ITEM_ID.Contains(m.ARTWORK_ITEM_ID)).ToList();
                        var listAllARTWORK2 = new List<ART_WF_ARTWORK_PROCESS>();
                        foreach (var item in listAllARTWORK3)
                        {
                            if (item.CURRENT_CUSTOMER_ID > 0)
                            {
                                var mailCus = ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER() { ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID }, context);
                                if (mailCus.Where(m => m.CUSTOMER_USER_ID == item.CURRENT_USER_ID && m.MAIL_CC == "X").ToList().Count == 0)
                                {
                                    listAllARTWORK2.Add(item);
                                }
                            }
                            else
                                listAllARTWORK2.Add(item);
                        }
                        Results.cntWaitingQCConfirmation = listAllARTWORK2.Where(m => (m.CURRENT_STEP_ID == SEND_QC) && string.IsNullOrEmpty(m.IS_END)).Select(m => m.ARTWORK_ITEM_ID).Distinct().ToList().Count;
                        Results.cntWaitingPrintmaster = listAllARTWORK2.Where(m => (m.CURRENT_STEP_ID == SEND_VN_PM) && string.IsNullOrEmpty(m.IS_END)).Select(m => m.ARTWORK_ITEM_ID).Distinct().ToList().Count;
                        Results.cntWaitingCustomerApprovePrintMaster = listAllARTWORK2.Where(m => (m.CURRENT_STEP_ID == SEND_CUS_PRINT) && string.IsNullOrEmpty(m.IS_END)).Select(m => m.ARTWORK_ITEM_ID).Distinct().ToList().Count;
                        Results.cntWaitingShadeLimit = listAllARTWORK2.Where(m => (m.CURRENT_STEP_ID == SEND_VN_SL) && string.IsNullOrEmpty(m.IS_END)).Select(m => m.ARTWORK_ITEM_ID).Distinct().ToList().Count;
                        Results.cntWaitingCustomerApproveShadeLimit = listAllARTWORK2.Where(m => (m.CURRENT_STEP_ID == SEND_CUS_SHADE) && string.IsNullOrEmpty(m.IS_END)).Select(m => m.ARTWORK_ITEM_ID).Distinct().ToList().Count;
                        Results.cntWaitingGMMK = listAllARTWORK2.Where(m => (m.CURRENT_STEP_ID == SEND_GM_MK) && string.IsNullOrEmpty(m.IS_END)).Select(m => m.ARTWORK_ITEM_ID).Distinct().ToList().Count;
                        Results.cntWaitingCustomerReview = listAllARTWORK2.Where(m => (m.CURRENT_STEP_ID == SEND_CUS_REVIEW) && string.IsNullOrEmpty(m.IS_END)).Select(m => m.ARTWORK_ITEM_ID).Distinct().ToList().Count;

                        Results.cntWaitingQCConfirmation_txt = "<div style='margin:3px;padding:3px;'>";
                        Results.cntWaitingQCConfirmation_txt += "<div style='white-space: nowrap;font-weight:bold;'>Waiting for QC confirmation " + (Results.cntWaitingQCConfirmation) + "</div>";
                        Results.cntWaitingQCConfirmation_txt += "</div>";

                        Results.cntWaitingPrintmaster_txt = "<div style='margin:3px;padding:3px;'>";
                        Results.cntWaitingPrintmaster_txt += "<div style='white-space: nowrap;font-weight:bold;'>Waiting for print master " + (Results.cntWaitingPrintmaster) + "</div>";
                        Results.cntWaitingPrintmaster_txt += "</div>";

                        Results.cntWaitingShadeLimit_txt = "<div style='margin:3px;padding:3px;'>";
                        Results.cntWaitingShadeLimit_txt += "<div style='white-space: nowrap;font-weight:bold;'>Waiting for shade limit (vendor) " + (Results.cntWaitingShadeLimit) + "</div>";
                        Results.cntWaitingShadeLimit_txt += "</div>";

                        Results.cntWaitingCustomerApprovePrintMaster_txt = "<div style='margin:3px;padding:3px;'>";
                        Results.cntWaitingCustomerApprovePrintMaster_txt += "<div style='white-space: nowrap;font-weight:bold;'>Waiting for customer approval print master " + (Results.cntWaitingCustomerApprovePrintMaster) + "</div>";
                        Results.cntWaitingCustomerApprovePrintMaster_txt += "</div>";

                        Results.cntWaitingCustomerApproveShadeLimit_txt = "<div style='margin:3px;padding:3px;'>";
                        Results.cntWaitingCustomerApproveShadeLimit_txt += "<div style='white-space: nowrap;font-weight:bold;'>Waiting for customer approval shade limit " + (Results.cntWaitingCustomerApproveShadeLimit) + "</div>";
                        Results.cntWaitingCustomerApproveShadeLimit_txt += "</div>";

                        Results.cntWaitingGMMK_txt = "<div style='margin:3px;padding:3px;'>";
                        Results.cntWaitingGMMK_txt += "<div style='white-space: nowrap;font-weight:bold;'>Waiting for approval (GM MK) " + (Results.cntWaitingGMMK) + "</div>";
                        Results.cntWaitingGMMK_txt += "</div>";

                        Results.cntWaitingCustomerReview_txt = "<div style='margin:3px;padding:3px;'>";
                        Results.cntWaitingCustomerReview_txt += "<div style='white-space: nowrap;font-weight:bold;'>Waiting for customer review " + (Results.cntWaitingCustomerReview) + "</div>";
                        Results.cntWaitingCustomerReview_txt += "</div>";

                        Results.status = "S";
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
        public static V_SAP_SALES_ORDER_RESULT GetSalesOrderNew(V_SAP_SALES_ORDER_REQUEST param)
        {
            V_SAP_SALES_ORDER_RESULT Results = new V_SAP_SALES_ORDER_RESULT();
            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<V_SAP_SALES_ORDER_2>();
                Results.draw = param.draw;
                return Results;
            }
            try
            {
                var getByCreateDateFrom = DateTime.Now;
                var getByCreateDateTo = DateTime.Now;
                var GET_BY_RDD_FROM = DateTime.Now;
                var GET_BY_RDD_TO = DateTime.Now;

                if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                    getByCreateDateFrom = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_FROM);
                if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                    getByCreateDateTo = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_TO);

                if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_FROM))
                    GET_BY_RDD_FROM = CNService.ConvertStringToDate(param.data.GET_BY_RDD_FROM);
                if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_TO))
                    GET_BY_RDD_TO = CNService.ConvertStringToDate(param.data.GET_BY_RDD_TO);

                var getByPackangingType = param.data.GET_BY_PACKAGING_TYPE;
                var getBySoldTo = param.data.GET_BY_SOLD_TO;
                var getByShipTo = param.data.GET_BY_SHIP_TO;
                var getByBrand = param.data.GET_BY_BRAND;

                SAP_M_CHARACTERISTIC_RESULT resultPackaging = new SAP_M_CHARACTERISTIC_RESULT();
                if (getByPackangingType != null)
                {
                    SAP_M_CHARACTERISTIC_REQUEST req = new SAP_M_CHARACTERISTIC_REQUEST();
                    req.data = new SAP_M_CHARACTERISTIC_2();
                    req.data.VALUE = getByPackangingType.Substring(0, 1);
                    resultPackaging = ItemPackingTypeHelper.GetPackType(req);
                }

                List<V_SAP_SALES_ORDER_2> list = new List<V_SAP_SALES_ORDER_2>();
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;
                        var listsoNewPIC = (from m in context.ART_M_PIC
                                            where m.USER_ID == param.data.CURRENT_USER_ID && string.IsNullOrEmpty(m.SOLD_TO_CODE)
                                            select m.PIC_ID).Count();

                        var q = (from p in context.V_SAP_SALES_ORDER2
                                 where string.IsNullOrEmpty(p.IS_MIGRATION) &&
                                 (
                                    (p.PRODUCT_CODE.StartsWith("3") && !p.PRODUCT_CODE.StartsWith("3W") && !string.IsNullOrEmpty(p.BOM_ITEM_CUSTOM_1) && p.BOM_ITEM_CUSTOM_1.Contains("NEW") && p.SO_ITEM_IS_ACTIVE == "X" && p.BOM_IS_ACTIVE == "X")
                                    || (p.PRODUCT_CODE.StartsWith("5") && p.PRODUCT_CODE.Length == 18 && p.ITEM_CUSTOM_1 == "0" && p.SO_ITEM_IS_ACTIVE == "X")
                                 )
                                && (listsoNewPIC > 0 ?
                                  !(from m in context.ART_M_PIC
                                    where m.USER_ID != param.data.CURRENT_USER_ID
                                    select new
                                    {
                                        X1 = string.IsNullOrEmpty(m.SOLD_TO_CODE) ? p.SOLD_TO : m.SOLD_TO_CODE,
                                        X2 = string.IsNullOrEmpty(m.SHIP_TO_CODE) ? p.SHIP_TO : m.SHIP_TO_CODE,
                                        X3 = string.IsNullOrEmpty(m.ZONE) ? (
                                        p.ZONE != null ? p.ZONE.Substring(0, 2) : p.ZONE) : m.ZONE,
                                        X4 = string.IsNullOrEmpty(m.COUNTRY) ? p.COUNTRY : m.COUNTRY
                                    }).Contains(new { X1 = p.SOLD_TO, X2 = p.SHIP_TO, X3 = !string.IsNullOrEmpty(p.ZONE) ? p.ZONE.Substring(0, 2) : p.ZONE, X4 = p.COUNTRY }) :
                                    (from m in context.ART_M_PIC
                                     where m.USER_ID == param.data.CURRENT_USER_ID
                                     select new
                                     {
                                         X1 = string.IsNullOrEmpty(m.SOLD_TO_CODE) ? p.SOLD_TO : m.SOLD_TO_CODE,
                                         X2 = string.IsNullOrEmpty(m.SHIP_TO_CODE) ? p.SHIP_TO : m.SHIP_TO_CODE,
                                         X3 = string.IsNullOrEmpty(m.ZONE) ? (
                                         p.ZONE != null ? p.ZONE.Substring(0, 2) : p.ZONE) : m.ZONE,
                                         X4 = string.IsNullOrEmpty(m.COUNTRY) ? p.COUNTRY : m.COUNTRY
                                     }).Contains(new { X1 = p.SOLD_TO, X2 = p.SHIP_TO, X3 = !string.IsNullOrEmpty(p.ZONE) ? p.ZONE.Substring(0, 2) : p.ZONE, X4 = p.COUNTRY }))

                                 && !(from o in context.V_ART_ASSIGNED_SO
                                      where !o.BOM_ITEM_CUSTOM_1.Contains("MULTI")
                                      select new { X1 = o.SALES_ORDER_NO, X2 = o.ITEM, X3 = o.BOM_ITEM_CUSTOM_1 }).Contains(new { X1 = p.SALES_ORDER_NO, X2 = p.ITEM, X3 = p.BOM_ITEM_CUSTOM_1 })

                                 select new V_SAP_SALES_ORDER_2
                                 {
                                     DECRIPTION = p.DECRIPTION,
                                     COMPONENT_ITEM = p.COMPONENT_ITEM,
                                     SALES_ORG = p.SALES_ORG,
                                     SOLD_TO = p.SOLD_TO,
                                     SOLD_TO_NAME = p.SOLD_TO_NAME,
                                     SHIP_TO = p.SHIP_TO,
                                     SHIP_TO_NAME = p.SHIP_TO_NAME,
                                     BRAND_ID = p.BRAND_ID,
                                     BRAND_DESCRIPTION = p.BRAND_DESCRIPTION,
                                     ADDITIONAL_BRAND_ID = p.ADDITIONAL_BRAND_ID,
                                     ADDITIONAL_BRAND_DESCRIPTION = p.ADDITIONAL_BRAND_DESCRIPTION,
                                     ZONE = p.ZONE,
                                     CREATE_ON = p.CREATE_ON,
                                     RDD = p.RDD,
                                     COMPONENT_MATERIAL = p.COMPONENT_MATERIAL,
                                     PRODUCT_CODE = p.PRODUCT_CODE,
                                     SALES_ORDER_NO = p.SALES_ORDER_NO,
                                     ITEM = p.ITEM,
                                     BOM_ITEM_CUSTOM_1 = p.BOM_ITEM_CUSTOM_1,
                                     ITEM_CUSTOM_1 = p.ITEM_CUSTOM_1,
                                     COUNTRY = p.COUNTRY,
                                     IN_TRANSIT_TO = p.IN_TRANSIT_TO,
                                     PRODUCTION_PLANT = p.PRODUCTION_PLANT,
                                 });

                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            q = q.Where(m => DbFunctions.TruncateTime(m.CREATE_ON) >= getByCreateDateFrom.Date);
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            q = q.Where(m => DbFunctions.TruncateTime(m.CREATE_ON) <= getByCreateDateTo.Date);

                        if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_FROM))
                            q = q.Where(m => DbFunctions.TruncateTime(m.RDD) >= GET_BY_RDD_FROM.Date);
                        if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_TO))
                            q = q.Where(m => DbFunctions.TruncateTime(m.RDD) <= GET_BY_RDD_TO.Date);

                        if (resultPackaging.data != null)
                        {
                            //filter by material 
                            string filterType = "5" + resultPackaging.data.FirstOrDefault().VALUE;

                            //filter by new bom item
                            string description = resultPackaging.data.FirstOrDefault().DESCRIPTION.Replace(" ", "");
                            var packagingTypeMapping = EnumExtension<PackagingTypeMapping>.GetEnumByPackagingType(description);
                            string bomMulti = packagingTypeMapping.GetAttributeBomItemMulti();
                            string bomNonMulti = packagingTypeMapping.GetAttributeBomItemNonMulti();

                            q = q.Where(m => m.COMPONENT_MATERIAL.StartsWith(filterType) || m.BOM_ITEM_CUSTOM_1.Contains(bomMulti) || m.BOM_ITEM_CUSTOM_1.Contains(bomNonMulti));
                        }

                        if (!string.IsNullOrEmpty(getBySoldTo))
                            q = q.Where(m => m.SOLD_TO + ":" + m.SOLD_TO_NAME == getBySoldTo);
                        if (!string.IsNullOrEmpty(getByShipTo))
                            q = q.Where(m => m.SHIP_TO + ":" + m.SHIP_TO_NAME == getByShipTo);
                        if (!string.IsNullOrEmpty(getByBrand))
                            q = q.Where(m => m.BRAND_ID + ":" + m.BRAND_DESCRIPTION == getByBrand);

                        list = q.ToList();

                        //var allPICConfig = ART_M_PIC_SERVICE.GetAll(context);
                        //List<V_SAP_SALES_ORDER_2> tempList = new List<V_SAP_SALES_ORDER_2>();
                        //foreach (var item in list)
                        //{
                        //    var PICUserId = CNService.CheckPICSO2(allPICConfig, item.SOLD_TO, item.SHIP_TO, item.ZONE);
                        //    if (param.data.CURRENT_USER_ID == PICUserId)
                        //    {
                        //        tempList.Add(item);
                        //    }
                        //}
                        //list = tempList;

                        List<SAP_M_LONG_TEXT> listLongText = new List<SAP_M_LONG_TEXT>();
                        if (param.data.GROUPING != "yes")
                        {
                            foreach (var item in list)
                            {
                                item.SOLD_TO_DISPLAY_TXT = item.SOLD_TO + ":" + item.SOLD_TO_NAME;
                                item.SHIP_TO_DISPLAY_TXT = item.SHIP_TO + ":" + item.SHIP_TO_NAME;
                                item.BRAND_DISPLAY_TXT = item.BRAND_ID + ":" + item.BRAND_DESCRIPTION;
                                item.ADDITIONAL_BRAND_DISPLAY_TXT = item.ADDITIONAL_BRAND_ID + ":" + item.ADDITIONAL_BRAND_DESCRIPTION;

                                if (item.SOLD_TO_DISPLAY_TXT.Trim() == ":") item.SOLD_TO_DISPLAY_TXT = "";
                                if (item.SHIP_TO_DISPLAY_TXT.Trim() == ":") item.SHIP_TO_DISPLAY_TXT = "";
                                if (item.BRAND_DISPLAY_TXT.Trim() == ":") item.BRAND_DISPLAY_TXT = "";
                                if (item.ADDITIONAL_BRAND_DISPLAY_TXT.Trim() == ":") item.ADDITIONAL_BRAND_DISPLAY_TXT = "";
                            }
                        }
                        else
                        {
                            foreach (var item in list)
                            {
                                item.SOLD_TO_DISPLAY_TXT = item.SOLD_TO + ":" + item.SOLD_TO_NAME;
                                item.SHIP_TO_DISPLAY_TXT = item.SHIP_TO + ":" + item.SHIP_TO_NAME;
                                item.BRAND_DISPLAY_TXT = item.BRAND_ID + ":" + item.BRAND_DESCRIPTION;
                                item.ADDITIONAL_BRAND_DISPLAY_TXT = item.ADDITIONAL_BRAND_ID + ":" + item.ADDITIONAL_BRAND_DESCRIPTION;

                                if (item.SOLD_TO_DISPLAY_TXT.Trim() == ":") item.SOLD_TO_DISPLAY_TXT = "";
                                if (item.SHIP_TO_DISPLAY_TXT.Trim() == ":") item.SHIP_TO_DISPLAY_TXT = "";
                                if (item.BRAND_DISPLAY_TXT.Trim() == ":") item.BRAND_DISPLAY_TXT = "";
                                if (item.ADDITIONAL_BRAND_DISPLAY_TXT.Trim() == ":") item.ADDITIONAL_BRAND_DISPLAY_TXT = "";

                                var itemForGroup = new V_SAP_SALES_ORDER_2();
                                if (item.PRODUCT_CODE.StartsWith("3"))
                                {
                                    itemForGroup = item;
                                }
                                else
                                {
                                    itemForGroup = list.Where(m => m.SALES_ORDER_NO == item.SALES_ORDER_NO && m.COMPONENT_MATERIAL == item.PRODUCT_CODE).FirstOrDefault();
                                    if (itemForGroup == null)
                                    {
                                        itemForGroup = item;
                                    }
                                }

                                if (itemForGroup != null)
                                {
                                    var WHText = "";
                                    var GeneralText = "";

                                    var tempWHText = listLongText.Where(m => m.TEXT_NAME == itemForGroup.SALES_ORDER_NO && m.TEXT_ID == itemForGroup.ITEM.ToString() && m.TEXT_LANGUAGE == "W").ToList();
                                    if (tempWHText.Count() == 1)
                                    {
                                        WHText = tempWHText.FirstOrDefault().LINE_TEXT;
                                    }
                                    else
                                    {
                                        WHText = CNService.GetWHText(itemForGroup.SALES_ORDER_NO, itemForGroup.ITEM.ToString(), context);
                                        listLongText.Add(new SAP_M_LONG_TEXT() { LINE_TEXT = WHText, TEXT_NAME = itemForGroup.SALES_ORDER_NO, TEXT_ID = itemForGroup.ITEM.ToString(), TEXT_LANGUAGE = "W" });
                                    }

                                    var tempGeneralText = listLongText.Where(m => m.TEXT_NAME == itemForGroup.SALES_ORDER_NO && m.TEXT_LANGUAGE == "G").ToList();
                                    if (tempGeneralText.Count() == 1)
                                    {
                                        GeneralText = tempGeneralText.FirstOrDefault().LINE_TEXT;
                                    }
                                    else
                                    {
                                        GeneralText = CNService.GetGeneralText(itemForGroup.SALES_ORDER_NO, context);
                                        listLongText.Add(new SAP_M_LONG_TEXT() { LINE_TEXT = GeneralText, TEXT_NAME = itemForGroup.SALES_ORDER_NO, TEXT_LANGUAGE = "G" });
                                    }

                                    item.GROUPING = itemForGroup.BOM_ITEM_CUSTOM_1 + itemForGroup.PRODUCT_CODE + itemForGroup.COMPONENT_MATERIAL
                                                      + WHText
                                                      + GeneralText
                                                      + itemForGroup.SHIP_TO + itemForGroup.SOLD_TO + itemForGroup.COUNTRY + itemForGroup.IN_TRANSIT_TO + itemForGroup.ADDITIONAL_BRAND_ID + itemForGroup.BRAND_ID;

                                    //item.GROUPING = EncryptionService.EncryptGrouping(item.GROUPING);
                                    //Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                                    //item.GROUPING = rgx.Replace(item.GROUPING, "");

                                    item.GROUPING_DISPLAY_TXT = "<b>Product code</b> : " + itemForGroup.PRODUCT_CODE + "<br />" +
                                                                         "<b>Country</b> : " + itemForGroup.COUNTRY + "<br />" +
                                                                         "<b>In transit to</b> : " + itemForGroup.IN_TRANSIT_TO + "<br />" +
                                                                         "<b>Additional brand</b> : " + itemForGroup.ADDITIONAL_BRAND_ID + ":" + itemForGroup.ADDITIONAL_BRAND_DESCRIPTION + "<br />" +
                                                                         "<b>Packaging code(BOM component)</b> : " + itemForGroup.COMPONENT_MATERIAL + "<br />" +
                                                                         "<b>Sold to</b> : " + itemForGroup.SOLD_TO + ":" + itemForGroup.SOLD_TO_NAME + "<br />" +
                                                                         "<b>Ship to</b> : " + itemForGroup.SHIP_TO + ":" + itemForGroup.SHIP_TO_NAME + "<br />" +
                                                                         "<b>Brand</b> : " + itemForGroup.BRAND_ID + ":" + itemForGroup.BRAND_DESCRIPTION + "<br />" +
                                                                         "<b>PKG & warehouse text</b> : " + WHText + "<br />" +
                                                                         "<b>General text</b> : " + GeneralText + "<br />";
                                }
                            }

                            var tempList2 = new List<V_SAP_SALES_ORDER_2>();
                            foreach (var item in list)
                            {
                                tempList2.Add(item);
                                if (!string.IsNullOrEmpty(item.COMPONENT_MATERIAL))
                                {
                                    var twoDigitMat5 = CNService.SubString(item.COMPONENT_MATERIAL, 2);

                                    var chkListFOC = (from m in context.V_SAP_SALES_ORDER2
                                                      where string.IsNullOrEmpty(m.IS_MIGRATION)
                                                      && m.SALES_ORDER_NO == item.SALES_ORDER_NO
                                                      && m.ITEM_CUSTOM_1 == item.ITEM.ToString()
                                                      && m.PRODUCT_CODE.StartsWith(twoDigitMat5)
                                                      && m.SO_ITEM_IS_ACTIVE == "X"
                                                      select new V_SAP_SALES_ORDER_2
                                                      {
                                                          ITEM = m.ITEM,
                                                          PRODUCT_CODE = m.PRODUCT_CODE,
                                                          SALES_ORDER_NO = m.SALES_ORDER_NO
                                                      }).ToList();

                                    foreach (var itemFOC in chkListFOC)
                                    {
                                        var tempItemFOC = itemFOC;
                                        tempItemFOC.SALES_ORG = item.SALES_ORG;
                                        tempItemFOC.SOLD_TO_DISPLAY_TXT = item.SOLD_TO + ":" + item.SOLD_TO_NAME;
                                        tempItemFOC.SHIP_TO_DISPLAY_TXT = item.SHIP_TO + ":" + item.SHIP_TO_NAME;
                                        tempItemFOC.BRAND_DISPLAY_TXT = item.BRAND_ID + ":" + item.BRAND_DESCRIPTION;
                                        tempItemFOC.ADDITIONAL_BRAND_DISPLAY_TXT = item.ADDITIONAL_BRAND_ID + ":" + item.ADDITIONAL_BRAND_DESCRIPTION;

                                        if (tempItemFOC.SOLD_TO_DISPLAY_TXT.Trim() == ":") tempItemFOC.SOLD_TO_DISPLAY_TXT = "";
                                        if (tempItemFOC.SHIP_TO_DISPLAY_TXT.Trim() == ":") tempItemFOC.SHIP_TO_DISPLAY_TXT = "";
                                        if (tempItemFOC.BRAND_DISPLAY_TXT.Trim() == ":") tempItemFOC.BRAND_DISPLAY_TXT = "";
                                        if (tempItemFOC.ADDITIONAL_BRAND_DISPLAY_TXT.Trim() == ":") tempItemFOC.ADDITIONAL_BRAND_DISPLAY_TXT = "";

                                        var itemForGroup = item;

                                        tempItemFOC.GROUPING = itemForGroup.GROUPING;
                                        tempItemFOC.GROUPING_DISPLAY_TXT = itemForGroup.GROUPING_DISPLAY_TXT;

                                        tempList2.Add(tempItemFOC);
                                    }
                                }
                            }
                            list = tempList2;
                        }

                        foreach (var item in list)
                        {
                            item.GROUPINGTEMP = item.GROUPING;
                        }

                        var cntGroup = 1;
                        foreach (var item in list)
                        {
                            var found = false;
                            var temp = list.Where(m => m.GROUPING == item.GROUPINGTEMP).ToList();
                            foreach (var itemTemp in temp)
                            {
                                found = true;
                                itemTemp.GROUPING = cntGroup.ToString();
                            }
                            if (found) cntGroup++;
                        }
                    }
                }
                Results.data = list;
                Results.recordsFiltered = Results.data.ToList().Count;
                Results.recordsTotal = Results.data.ToList().Count;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static V_SAP_SALES_ORDER_RESULT GetSalesOrderRepeat(V_SAP_SALES_ORDER_REQUEST param)
        {
            V_SAP_SALES_ORDER_RESULT Results = new V_SAP_SALES_ORDER_RESULT();
            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<V_SAP_SALES_ORDER_2>();
                Results.draw = param.draw;
                return Results;
            }
            try
            {
                var getByCreateDateFrom = DateTime.Now;
                var getByCreateDateTo = DateTime.Now;
                var GET_BY_RDD_FROM = DateTime.Now;
                var GET_BY_RDD_TO = DateTime.Now;

                if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                    getByCreateDateFrom = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_FROM);
                if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                    getByCreateDateTo = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_TO);

                if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_FROM))
                    GET_BY_RDD_FROM = CNService.ConvertStringToDate(param.data.GET_BY_RDD_FROM);
                if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_TO))
                    GET_BY_RDD_TO = CNService.ConvertStringToDate(param.data.GET_BY_RDD_TO);

                var getByPackangingType = param.data.GET_BY_PACKAGING_TYPE;
                var getBySoldTo = param.data.GET_BY_SOLD_TO;
                var getByShipTo = param.data.GET_BY_SHIP_TO;
                var getByBrand = param.data.GET_BY_BRAND;

                SAP_M_CHARACTERISTIC_RESULT resultPackaging = new SAP_M_CHARACTERISTIC_RESULT();
                if (getByPackangingType != null)
                {
                    SAP_M_CHARACTERISTIC_REQUEST req = new SAP_M_CHARACTERISTIC_REQUEST();
                    req.data = new SAP_M_CHARACTERISTIC_2();
                    req.data.VALUE = getByPackangingType.Substring(0, 1);
                    resultPackaging = ItemPackingTypeHelper.GetPackType(req);
                }
                List<V_SAP_SALES_ORDER_2> list = new List<V_SAP_SALES_ORDER_2>();
                //var listDataSO = CNService.GetDataSO(param.data.CURRENT_USER_ID);

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        //var xx = context.V_SAP_SALES_ORDER2.Where(w=>w.SALES_ORDER_NO== "500003480" && w.BOM_QTY_STOCK == "2012").ToList();

                        var listPIC = (from m in context.ART_M_PIC
                                       where m.USER_ID == param.data.CURRENT_USER_ID && string.IsNullOrEmpty(m.SOLD_TO_CODE)
                                       select m.PIC_ID).Count();

                        var q = (from p in context.V_SAP_SALES_ORDER2
                                 where string.IsNullOrEmpty(p.IS_MIGRATION) &&
                                 (
                                     //ทั่วไป
                                     (p.PRODUCT_CODE.StartsWith("3") && !p.PRODUCT_CODE.StartsWith("3W") && !string.IsNullOrEmpty(p.COMPONENT_MATERIAL) && p.COMPONENT_MATERIAL.StartsWith("5") && p.COMPONENT_MATERIAL.Length == 18 && string.IsNullOrEmpty(p.BOM_ITEM_CUSTOM_1) && p.SO_ITEM_IS_ACTIVE == "X" && p.BOM_IS_ACTIVE == "X")
                                     //3v
                                     || (p.PRODUCT_CODE.StartsWith("5") && p.PRODUCT_CODE.Length == 18 && string.IsNullOrEmpty(p.ITEM_CUSTOM_1) && p.SO_ITEM_IS_ACTIVE == "X")
                                     //ของแถมที่ไม่ได้ ref กับใคร
                                     || (p.PRODUCT_CODE.StartsWith("5") && p.PRODUCT_CODE.Length == 18 && p.ITEM_CUSTOM_1 == "0" && p.SO_ITEM_IS_ACTIVE == "X")
                                 ) &&
                                 (listPIC > 0 ?
                                  !(from m in context.ART_M_PIC
                                    where m.USER_ID != param.data.CURRENT_USER_ID
                                    select new
                                    {
                                        X1 = string.IsNullOrEmpty(m.SOLD_TO_CODE) ? p.SOLD_TO : m.SOLD_TO_CODE,
                                        X2 = string.IsNullOrEmpty(m.SHIP_TO_CODE) ? p.SHIP_TO : m.SHIP_TO_CODE,
                                        X3 = string.IsNullOrEmpty(m.ZONE) ? (
                                        p.ZONE != null ? p.ZONE.Substring(0, 2) : p.ZONE) : m.ZONE,
                                        X4 = string.IsNullOrEmpty(m.COUNTRY) ? p.COUNTRY : m.COUNTRY
                                    }).Contains(new { X1 = p.SOLD_TO, X2 = p.SHIP_TO, X3 = !string.IsNullOrEmpty(p.ZONE) ? p.ZONE.Substring(0, 2) : p.ZONE, X4 = p.COUNTRY }) :
                                    (from m in context.ART_M_PIC
                                     where m.USER_ID == param.data.CURRENT_USER_ID
                                     select new
                                     {
                                         X1 = string.IsNullOrEmpty(m.SOLD_TO_CODE) ? p.SOLD_TO : m.SOLD_TO_CODE,
                                         X2 = string.IsNullOrEmpty(m.SHIP_TO_CODE) ? p.SHIP_TO : m.SHIP_TO_CODE,
                                         X3 = string.IsNullOrEmpty(m.ZONE) ? (
                                         p.ZONE != null ? p.ZONE.Substring(0, 2) : p.ZONE) : m.ZONE,
                                         X4 = string.IsNullOrEmpty(m.COUNTRY) ? p.COUNTRY : m.COUNTRY
                                     }).Contains(new { X1 = p.SOLD_TO, X2 = p.SHIP_TO, X3 = !string.IsNullOrEmpty(p.ZONE) ? p.ZONE.Substring(0, 2) : p.ZONE, X4 = p.COUNTRY }))

                                 && !(from o in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                      select new { X1 = o.SALES_ORDER_NO, X2 = o.SALES_ORDER_ITEM, X3 = o.PRODUCT_CODE, X4 = o.COMPONENT_MATERIAL }).Contains(new { X1 = p.SALES_ORDER_NO, X2 = p.ITEM.ToString(), X3 = p.PRODUCT_CODE, X4 = p.COMPONENT_MATERIAL })

                                 && !(from o in context.V_ART_ASSIGNED_SO
                                      select new { X1 = o.SALES_ORDER_NO, X2 = o.ITEM, X3 = o.COMPONENT_MATERIAL }).Contains(new { X1 = p.SALES_ORDER_NO, X2 = p.ITEM, X3 = p.COMPONENT_MATERIAL })

                                 //&& p.SALES_ORDER_NO.Equals("500631691")

                                 select new V_SAP_SALES_ORDER_2
                                 {
                                     DECRIPTION = p.DECRIPTION,
                                     COMPONENT_ITEM = p.COMPONENT_ITEM,
                                     SALES_ORG = p.SALES_ORG,
                                     SOLD_TO = p.SOLD_TO,
                                     SOLD_TO_NAME = p.SOLD_TO_NAME,
                                     SHIP_TO = p.SHIP_TO,
                                     SHIP_TO_NAME = p.SHIP_TO_NAME,
                                     BRAND_ID = p.BRAND_ID,
                                     BRAND_DESCRIPTION = p.BRAND_DESCRIPTION,
                                     ADDITIONAL_BRAND_ID = p.ADDITIONAL_BRAND_ID,
                                     ADDITIONAL_BRAND_DESCRIPTION = p.ADDITIONAL_BRAND_DESCRIPTION,
                                     ZONE = p.ZONE,
                                     CREATE_ON = p.CREATE_ON,
                                     RDD = p.RDD,
                                     COMPONENT_MATERIAL = p.COMPONENT_MATERIAL,
                                     PRODUCT_CODE = p.PRODUCT_CODE,
                                     SALES_ORDER_NO = p.SALES_ORDER_NO,
                                     ITEM = p.ITEM,
                                     BOM_ITEM_CUSTOM_1 = p.BOM_ITEM_CUSTOM_1,
                                     ITEM_CUSTOM_1 = p.ITEM_CUSTOM_1,
                                     COUNTRY = p.COUNTRY,
                                     IN_TRANSIT_TO = p.IN_TRANSIT_TO,
                                     BOM_STOCK = p.BOM_STOCK,
                                     STOCK = p.STOCK,
                                     BOM_QTY_STOCK = p.BOM_QTY_STOCK.Replace(" ", ""),
                                     QUANTITY = p.QUANTITY > 0 ? p.QUANTITY : p.ORDER_QTY,
                                     PRODUCTION_PLANT = p.PRODUCTION_PLANT,
                                 });
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            q = q.Where(m => DbFunctions.TruncateTime(m.CREATE_ON) >= getByCreateDateFrom.Date);
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            q = q.Where(m => DbFunctions.TruncateTime(m.CREATE_ON) <= getByCreateDateTo.Date);

                        if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_FROM))
                            q = q.Where(m => DbFunctions.TruncateTime(m.RDD) >= GET_BY_RDD_FROM.Date);
                        if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_TO))
                            q = q.Where(m => DbFunctions.TruncateTime(m.RDD) <= GET_BY_RDD_TO.Date);

                        if (resultPackaging.data != null)
                        {
                            string filterType = "5" + resultPackaging.data.FirstOrDefault().VALUE;
                            q = q.Where(m => m.COMPONENT_MATERIAL.StartsWith(filterType));
                        }

                        if (!string.IsNullOrEmpty(getBySoldTo))
                            q = q.Where(m => m.SOLD_TO + ":" + m.SOLD_TO_NAME == getBySoldTo);
                        if (!string.IsNullOrEmpty(getByShipTo))
                            q = q.Where(m => m.SHIP_TO + ":" + m.SHIP_TO_NAME == getByShipTo);
                        if (!string.IsNullOrEmpty(getByBrand))
                            q = q.Where(m => m.BRAND_ID + ":" + m.BRAND_DESCRIPTION == getByBrand);

                        list = q.ToList();
                        //list = (from p in list  where listDataSO.Equals(p.PO_COMPLETE_SO_HEADER_ID)
                        //         select p).ToList();

                        //var allPICConfig = ART_M_PIC_SERVICE.GetAll(context);
                        //List<V_SAP_SALES_ORDER_2> tempList = new List<V_SAP_SALES_ORDER_2>();
                        //foreach (var item in list)
                        //{
                        //    var PICUserId = CNService.CheckPICSO2(allPICConfig, item.SOLD_TO, item.SHIP_TO, item.ZONE);
                        //    if (param.data.CURRENT_USER_ID == PICUserId)
                        //    {
                        //        tempList.Add(item);
                        //    }
                        //}
                        //list = tempList;

                        List<SAP_M_LONG_TEXT> listLongText = new List<SAP_M_LONG_TEXT>();
                        //foreach (var item in list)
                        //{
                        //    item.RECHECK_ARTWORK = "";

                        //    var mat5 = item.COMPONENT_MATERIAL;
                        //    var wfAssignSOComplete = (from m in context.ART_WF_ARTWORK_PROCESS
                        //                              join k in context.V_ART_ASSIGNED_SO on m.ARTWORK_SUB_ID equals k.ARTWORK_SUB_ID
                        //                              where k.COMPONENT_MATERIAL == mat5 && m.IS_END == "X" && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                        //                              select k.RDD).ToList();

                        //    if (wfAssignSOComplete.Count > 0)
                        //    {
                        //        wfAssignSOComplete = wfAssignSOComplete.OrderByDescending(m => m).ToList();
                        //        DateTime rdd = Convert.ToDateTime(wfAssignSOComplete[0].Value);
                        //        DateTime currentDate = DateTime.Now.Date;
                        //        if (currentDate > rdd)
                        //        {
                        //            int diffMonth = FormNumberHelper.GetMonthDifference(currentDate, rdd);
                        //            if (diffMonth > 6)
                        //            {
                        //                item.RECHECK_ARTWORK = "Yes";
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        var listSO = (from m in context.V_SAP_SALES_ORDER
                        //                      where m.COMPONENT_MATERIAL == mat5 && m.IS_MIGRATION == "X"
                        //                      select m.RDD).ToList();
                        //        if (listSO.Count > 0)
                        //        {
                        //            listSO = listSO.OrderByDescending(m => m).ToList();
                        //            if (listSO[0] != null)
                        //            {
                        //                DateTime rdd = Convert.ToDateTime(listSO[0].Value);
                        //                DateTime currentDate = DateTime.Now.Date;
                        //                if (currentDate > rdd)
                        //                {
                        //                    int diffMonth = FormNumberHelper.GetMonthDifference(currentDate, rdd);
                        //                    if (diffMonth > 6)
                        //                    {
                        //                        item.RECHECK_ARTWORK = "Yes";
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }                            
                        if (param.data.GROUPING != "yes")
                        {
                            foreach (var item in list)
                            {
                                DateTime currentDate = DateTime.Now.Date;
                                var mat5 = item.COMPONENT_MATERIAL;
                                item.RECHECK_ARTWORK = "";
                                if (!string.IsNullOrEmpty(item.COMPONENT_MATERIAL))
                                {
                                    if (item.COMPONENT_MATERIAL.StartsWith("5"))
                                        item.RECHECK_ARTWORK = string.Format("{0}", CNService.GetCheckRDD(mat5));
                                }
                                if (!string.IsNullOrEmpty(item.BOM_QTY_STOCK) && CNService.IsNumeric(item.BOM_QTY_STOCK))
                                {
                                    item.ACTION = Convert.ToDecimal(item.BOM_QTY_STOCK) - item.QUANTITY >= 0 ? "Complete WF" : "Send to PP";
                                }
                                else if (item.BOM_QTY_STOCK.Contains("-"))
                                    item.ACTION = "Check Stock";
                                else
                                    item.ACTION = "Send to PP";

                                item.SOLD_TO_DISPLAY_TXT = item.SOLD_TO + ":" + item.SOLD_TO_NAME;
                                item.SHIP_TO_DISPLAY_TXT = item.SHIP_TO + ":" + item.SHIP_TO_NAME;
                                item.BRAND_DISPLAY_TXT = item.BRAND_ID + ":" + item.BRAND_DESCRIPTION;
                                item.ADDITIONAL_BRAND_DISPLAY_TXT = item.ADDITIONAL_BRAND_ID + ":" + item.ADDITIONAL_BRAND_DESCRIPTION;
 
                                if (item.SOLD_TO_DISPLAY_TXT.Trim() == ":") item.SOLD_TO_DISPLAY_TXT = "";
                                if (item.SHIP_TO_DISPLAY_TXT.Trim() == ":") item.SHIP_TO_DISPLAY_TXT = "";
                                if (item.BRAND_DISPLAY_TXT.Trim() == ":") item.BRAND_DISPLAY_TXT = "";
                                if (item.ADDITIONAL_BRAND_DISPLAY_TXT.Trim() == ":") item.ADDITIONAL_BRAND_DISPLAY_TXT = "";

                                if (string.IsNullOrEmpty(item.STOCK)) item.STOCK = "";
                                if (string.IsNullOrEmpty(item.BOM_STOCK)) item.BOM_STOCK = "";

                                if (!string.IsNullOrEmpty(item.BOM_STOCK))
                                    item.BOM_STOCK = item.BOM_STOCK;
                                else
                                    item.BOM_STOCK = item.STOCK;
                            }
                        }
                        else
                        {
                            foreach (var item in list)
                            {
                                DateTime currentDate = DateTime.Now.Date;
                                var mat5 = item.COMPONENT_MATERIAL;
                                item.RECHECK_ARTWORK = "";
                                if (!string.IsNullOrEmpty(item.COMPONENT_MATERIAL))
                                {
                                    if (item.COMPONENT_MATERIAL.StartsWith("5"))
                                        item.RECHECK_ARTWORK = string.Format("{0}", CNService.GetCheckRDD(mat5));
                                }
                                if (!string.IsNullOrEmpty(item.BOM_QTY_STOCK) && CNService.IsNumeric(item.BOM_QTY_STOCK))
                                {
                                    item.ACTION = Convert.ToDecimal(item.BOM_QTY_STOCK) - item.QUANTITY >= 0 ? "Complete WF" : "Send to PP";
                                }
                                else if (item.BOM_QTY_STOCK.Contains("-"))
                                    item.ACTION = "Check Stock";
                                else
                                    item.ACTION = "Send to PP";

                                item.SOLD_TO_DISPLAY_TXT = item.SOLD_TO + ":" + item.SOLD_TO_NAME;
                                item.SHIP_TO_DISPLAY_TXT = item.SHIP_TO + ":" + item.SHIP_TO_NAME;
                                item.BRAND_DISPLAY_TXT = item.BRAND_ID + ":" + item.BRAND_DESCRIPTION;
                                item.ADDITIONAL_BRAND_DISPLAY_TXT = item.ADDITIONAL_BRAND_ID + ":" + item.ADDITIONAL_BRAND_DESCRIPTION;

                                if (item.SOLD_TO_DISPLAY_TXT.Trim() == ":") item.SOLD_TO_DISPLAY_TXT = "";
                                if (item.SHIP_TO_DISPLAY_TXT.Trim() == ":") item.SHIP_TO_DISPLAY_TXT = "";
                                if (item.BRAND_DISPLAY_TXT.Trim() == ":") item.BRAND_DISPLAY_TXT = "";
                                if (item.ADDITIONAL_BRAND_DISPLAY_TXT.Trim() == ":") item.ADDITIONAL_BRAND_DISPLAY_TXT = "";

                                if (string.IsNullOrEmpty(item.STOCK)) item.STOCK = "";
                                if (string.IsNullOrEmpty(item.BOM_STOCK)) item.BOM_STOCK = "";

                                if (!string.IsNullOrEmpty(item.BOM_STOCK))
                                    item.BOM_STOCK = item.BOM_STOCK;
                                else
                                    item.BOM_STOCK = item.STOCK;

                                var itemForGroup = new V_SAP_SALES_ORDER_2();
                                if (item.PRODUCT_CODE.StartsWith("3"))
                                {
                                    itemForGroup = item;
                                }
                                else
                                {
                                    itemForGroup = list.Where(m => m.SALES_ORDER_NO == item.SALES_ORDER_NO && m.COMPONENT_MATERIAL == item.PRODUCT_CODE).FirstOrDefault();
                                    if (itemForGroup == null)
                                    {
                                        itemForGroup = item;
                                    }
                                }

                                if (itemForGroup != null)
                                {
                                    var WHText = "";
                                    var GeneralText = "";

                                    var tempWHText = listLongText.Where(m => m.TEXT_NAME == itemForGroup.SALES_ORDER_NO && m.TEXT_ID == itemForGroup.ITEM.ToString() && m.TEXT_LANGUAGE == "W").ToList();
                                    if (tempWHText.Count() == 1)
                                    {
                                        WHText = tempWHText.FirstOrDefault().LINE_TEXT;
                                    }
                                    else
                                    {
                                        WHText = CNService.GetWHText(itemForGroup.SALES_ORDER_NO, itemForGroup.ITEM.ToString(), context);
                                        listLongText.Add(new SAP_M_LONG_TEXT() { LINE_TEXT = WHText, TEXT_NAME = itemForGroup.SALES_ORDER_NO, TEXT_ID = itemForGroup.ITEM.ToString(), TEXT_LANGUAGE = "W" });
                                    }

                                    var tempGeneralText = listLongText.Where(m => m.TEXT_NAME == itemForGroup.SALES_ORDER_NO && m.TEXT_LANGUAGE == "G").ToList();
                                    if (tempGeneralText.Count() == 1)
                                    {
                                        GeneralText = tempGeneralText.FirstOrDefault().LINE_TEXT;
                                    }
                                    else
                                    {
                                        GeneralText = CNService.GetGeneralText(itemForGroup.SALES_ORDER_NO, context);
                                        listLongText.Add(new SAP_M_LONG_TEXT() { LINE_TEXT = GeneralText, TEXT_NAME = itemForGroup.SALES_ORDER_NO, TEXT_LANGUAGE = "G" });
                                    }

                                    item.GROUPING = itemForGroup.BOM_ITEM_CUSTOM_1 + itemForGroup.PRODUCT_CODE + itemForGroup.COMPONENT_MATERIAL
                                                      + WHText
                                                      + GeneralText
                                                      + itemForGroup.SHIP_TO + itemForGroup.SOLD_TO + itemForGroup.COUNTRY + itemForGroup.IN_TRANSIT_TO + itemForGroup.ADDITIONAL_BRAND_ID + itemForGroup.BRAND_ID;

                                    //item.GROUPING = EncryptionService.EncryptGrouping(item.GROUPING);
                                    //Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                                    //item.GROUPING = rgx.Replace(item.GROUPING, "");

                                    item.GROUPING_DISPLAY_TXT = "<b>Product code</b> : " + itemForGroup.PRODUCT_CODE + "<br />" +
                                                                         "<b>Country</b> : " + itemForGroup.COUNTRY + "<br />" +
                                                                         "<b>In transit to</b> : " + itemForGroup.IN_TRANSIT_TO + "<br />" +
                                                                         "<b>Additional brand</b> : " + itemForGroup.ADDITIONAL_BRAND_ID + ":" + itemForGroup.ADDITIONAL_BRAND_DESCRIPTION + "<br />" +
                                                                         "<b>Packaging code(BOM component)</b> : " + itemForGroup.COMPONENT_MATERIAL + "<br />" +
                                                                         "<b>Sold to</b> : " + itemForGroup.SOLD_TO + ":" + itemForGroup.SOLD_TO_NAME + "<br />" +
                                                                         "<b>Ship to</b> : " + itemForGroup.SHIP_TO + ":" + itemForGroup.SHIP_TO_NAME + "<br />" +
                                                                         "<b>Brand</b> : " + itemForGroup.BRAND_ID + ":" + itemForGroup.BRAND_DESCRIPTION + "<br />" +
                                                                         "<b>PKG & warehouse text</b> : " + WHText + "<br />" +
                                                                         "<b>General text</b> : " + GeneralText + "<br />";
                                }
                            }
                        }
                        var listSaleOrder = list.Select(m => m.SALES_ORDER_NO).Distinct().ToList();

                        var listSODetail = (from m in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                            where listSaleOrder.Contains(m.SALES_ORDER_NO)
                                            select m).ToList();

                        var listSAPSO = (from m in context.V_SAP_SALES_ORDER2
                                         where listSaleOrder.Contains(m.SALES_ORDER_NO)
                                         select new V_SAP_SALES_ORDER_2
                                         {
                                             ITEM_CUSTOM_1 = m.ITEM_CUSTOM_1,
                                             ITEM = m.ITEM,
                                             PRODUCT_CODE = m.PRODUCT_CODE,
                                             SALES_ORDER_NO = m.SALES_ORDER_NO,
                                             SO_ITEM_IS_ACTIVE = m.SO_ITEM_IS_ACTIVE,
                                             IS_MIGRATION = m.IS_MIGRATION
                                         }).ToList();

                        var tempList2 = new List<V_SAP_SALES_ORDER_2>();
                        foreach (var item in list)
                        {
                            tempList2.Add(item);

                            var chkListFOC = (from m in listSAPSO
                                              where string.IsNullOrEmpty(m.IS_MIGRATION)
                                              && m.SALES_ORDER_NO == item.SALES_ORDER_NO
                                              && m.ITEM_CUSTOM_1 == item.ITEM.ToString()
                                              && m.PRODUCT_CODE == item.COMPONENT_MATERIAL
                                              && m.SO_ITEM_IS_ACTIVE == "X"
                                              select new V_SAP_SALES_ORDER_2
                                              {
                                                  ITEM_CUSTOM_1 = m.ITEM_CUSTOM_1,
                                                  ITEM = m.ITEM,
                                                  PRODUCT_CODE = m.PRODUCT_CODE,
                                                  SALES_ORDER_NO = m.SALES_ORDER_NO
                                              }).ToList();

                            foreach (var itemFOC in chkListFOC)
                            {
                                var tempItemFOC = itemFOC;
                                tempItemFOC.ITEM_CUSTOM_1 = itemFOC.ITEM_CUSTOM_1;
                                tempItemFOC.SALES_ORG = item.SALES_ORG;
                                tempItemFOC.SOLD_TO_DISPLAY_TXT = item.SOLD_TO + ":" + item.SOLD_TO_NAME;
                                tempItemFOC.SHIP_TO_DISPLAY_TXT = item.SHIP_TO + ":" + item.SHIP_TO_NAME;
                                tempItemFOC.BRAND_DISPLAY_TXT = item.BRAND_ID + ":" + item.BRAND_DESCRIPTION;
                                tempItemFOC.ADDITIONAL_BRAND_DISPLAY_TXT = item.ADDITIONAL_BRAND_ID + ":" + item.ADDITIONAL_BRAND_DESCRIPTION;

                                if (tempItemFOC.SOLD_TO_DISPLAY_TXT.Trim() == ":") tempItemFOC.SOLD_TO_DISPLAY_TXT = "";
                                if (tempItemFOC.SHIP_TO_DISPLAY_TXT.Trim() == ":") tempItemFOC.SHIP_TO_DISPLAY_TXT = "";
                                if (tempItemFOC.BRAND_DISPLAY_TXT.Trim() == ":") tempItemFOC.BRAND_DISPLAY_TXT = "";
                                if (tempItemFOC.ADDITIONAL_BRAND_DISPLAY_TXT.Trim() == ":") tempItemFOC.ADDITIONAL_BRAND_DISPLAY_TXT = "";

                                var itemForGroup = item;

                                tempItemFOC.GROUPING = itemForGroup.GROUPING;
                                tempItemFOC.GROUPING_DISPLAY_TXT = itemForGroup.GROUPING_DISPLAY_TXT;

                                var SALES_ORDER_ITEM = Convert.ToInt32(itemFOC.ITEM).ToString();
                                //if (CNService.IsDevOrQAS())
                                //{
                                var cntFOC = (from m in listSODetail
                                              where m.SALES_ORDER_NO == itemFOC.SALES_ORDER_NO
                                              && m.SALES_ORDER_ITEM == SALES_ORDER_ITEM
                                              select m).Count();
                                if (cntFOC == 0)
                                    tempList2.Add(tempItemFOC);
                                //}
                                //else
                                //{
                                //    tempList2.Add(tempItemFOC);
                                //}
                            }
                        }
                        list = tempList2;

                        foreach (var item in list)
                        {
                            item.GROUPINGTEMP = item.GROUPING;
                        }

                        var cntGroup = 1;
                        foreach (var item in list)
                        {
                            var found = false;
                            var temp = list.Where(m => m.GROUPING == item.GROUPINGTEMP).ToList();
                            foreach (var itemTemp in temp)
                            {
                                found = true;
                                itemTemp.GROUPING = cntGroup.ToString();
                            }
                            if (found) cntGroup++;
                        }

                        int curentGroup = 1;
                        List<int> listGrouping = list.GroupBy(g => new { Grouping = g.GROUPING }).Select(g => Convert.ToInt32(g.Key.Grouping)).ToList();

                        foreach (var item in listGrouping)
                        {
                            curentGroup = item > 1 ? item : curentGroup;
                            var listCurrentGroup = list.Where(m => m.GROUPING == curentGroup.ToString()).ToList();
                            var listBeforeGroup = list.Where(m => m.GROUPING == (curentGroup - 1).ToString()).ToList();

                            //case group > 1
                            if (listBeforeGroup.Count() > 0)
                            {
                                int maxBeforeGroup = listBeforeGroup.Select(m => m.GROUP_MAX_ROW).FirstOrDefault();
                                listCurrentGroup.ForEach(a =>
                                {
                                    a.GROUP_MIN_ROW = maxBeforeGroup + 1;
                                    a.GROUP_MAX_ROW = maxBeforeGroup + listCurrentGroup.Count();
                                    //a.SELECTED_GROUP = 0;
                                });
                            }
                            //case group = 1
                            else
                            {
                                listCurrentGroup.ForEach(a =>
                                {
                                    a.GROUP_MIN_ROW = curentGroup;
                                    a.GROUP_MAX_ROW = listCurrentGroup.Count();
                                    //a.SELECTED_GROUP = 0;
                                });
                            }
                        }
                    }
                }
                Results.data = list;
                Results.recordsFiltered = Results.data.ToList().Count;
                Results.recordsTotal = Results.data.ToList().Count;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static V_SAP_SALES_ORDER_RESULT GetSalesOrderNew2(V_SAP_SALES_ORDER_REQUEST param)
        {
            V_SAP_SALES_ORDER_RESULT Results = new V_SAP_SALES_ORDER_RESULT();
            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<V_SAP_SALES_ORDER_2>();
                Results.draw = param.draw;
                return Results;
            }
            try
            {
                var getByCreateDateFrom = DateTime.Now;
                var getByCreateDateTo = DateTime.Now;
                var GET_BY_RDD_FROM = DateTime.Now;
                var GET_BY_RDD_TO = DateTime.Now;

                if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                    getByCreateDateFrom = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_FROM);
                if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                    getByCreateDateTo = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_TO);

                if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_FROM))
                    GET_BY_RDD_FROM = CNService.ConvertStringToDate(param.data.GET_BY_RDD_FROM);
                if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_TO))
                    GET_BY_RDD_TO = CNService.ConvertStringToDate(param.data.GET_BY_RDD_TO);

                var getByPackangingType = param.data.GET_BY_PACKAGING_TYPE;
                var getBySoldTo = param.data.GET_BY_SOLD_TO;
                var getByShipTo = param.data.GET_BY_SHIP_TO;
                var getByBrand = param.data.GET_BY_BRAND;

                SAP_M_CHARACTERISTIC_RESULT resultPackaging = new SAP_M_CHARACTERISTIC_RESULT();
                if (getByPackangingType != null)
                {
                    SAP_M_CHARACTERISTIC_REQUEST req = new SAP_M_CHARACTERISTIC_REQUEST();
                    req.data = new SAP_M_CHARACTERISTIC_2();
                    req.data.VALUE = getByPackangingType.Substring(0, 1);
                    resultPackaging = ItemPackingTypeHelper.GetPackType(req);
                }

                List<V_SAP_SALES_ORDER_2> list = new List<V_SAP_SALES_ORDER_2>();
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;
                        var listsoNewPIC = (from m in context.ART_M_PIC
                                            where m.USER_ID == param.data.CURRENT_USER_ID && string.IsNullOrEmpty(m.SOLD_TO_CODE)
                                            select m.PIC_ID).Count();
                        var qOrderNew = context.Database.SqlQuery<V_SAP_SALES_ORDER_2>(@"sp_SAP_SALES_ORDER3 @CreateDateFrom,
                            @CreateDateTo,@RDD_FROM,@RDD_TO,@COMPONENT_MATERIAL,@SOLD_TO,@SHIP_TO,@BRAND_ID",
                            new SqlParameter("@CreateDateFrom", !string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM)?string.Format("{0}", getByCreateDateFrom.Date):""),
                            new SqlParameter("@CreateDateTo", !string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO)?string.Format("{0}", getByCreateDateTo.Date):""),
                            new SqlParameter("@RDD_FROM", !string.IsNullOrEmpty(param.data.GET_BY_RDD_FROM)?string.Format("{0}", GET_BY_RDD_FROM.Date):""),
                            new SqlParameter("@RDD_TO", !string.IsNullOrEmpty(param.data.GET_BY_RDD_TO)?string.Format("{0}", GET_BY_RDD_TO.Date):""),
                            new SqlParameter("@COMPONENT_MATERIAL", getByPackangingType != null ? string.Format("5{0}",resultPackaging.data.FirstOrDefault().VALUE):""),
                                                new SqlParameter("@SOLD_TO", string.Format("{0}", getBySoldTo)),
                                                new SqlParameter("@SHIP_TO", string.Format("{0}", getByShipTo)),
                                                new SqlParameter("@BRAND_ID", string.Format("{0}", getByBrand))).ToList();

                        var q = qOrderNew.Select(p => new V_SAP_SALES_ORDER_2
                        {
                            DECRIPTION = p.DECRIPTION,
                            COMPONENT_ITEM = p.COMPONENT_ITEM,
                            SALES_ORG = p.SALES_ORG,
                            SOLD_TO = p.SOLD_TO,
                            SOLD_TO_NAME = p.SOLD_TO_NAME,
                            SHIP_TO = p.SHIP_TO,
                            SHIP_TO_NAME = p.SHIP_TO_NAME,
                            BRAND_ID = p.BRAND_ID,
                            BRAND_DESCRIPTION = p.BRAND_DESCRIPTION,
                            ADDITIONAL_BRAND_ID = p.ADDITIONAL_BRAND_ID,
                            ADDITIONAL_BRAND_DESCRIPTION = p.ADDITIONAL_BRAND_DESCRIPTION,
                            ZONE = p.ZONE,
                            CREATE_ON = p.CREATE_ON,
                            RDD = p.RDD,
                            COMPONENT_MATERIAL = p.COMPONENT_MATERIAL,
                            PRODUCT_CODE = p.PRODUCT_CODE,
                            SALES_ORDER_NO = p.SALES_ORDER_NO,
                            ITEM = p.ITEM,
                            BOM_ITEM_CUSTOM_1 = p.BOM_ITEM_CUSTOM_1,
                            ITEM_CUSTOM_1 = p.ITEM_CUSTOM_1,
                            COUNTRY = p.COUNTRY,
                            IN_TRANSIT_TO = p.IN_TRANSIT_TO,
                            SO_ITEM_IS_ACTIVE = p.SO_ITEM_IS_ACTIVE,
                            IS_MIGRATION = p.IS_MIGRATION,
                            PLANT = p.PRODUCTION_PLANT,
                        }).ToList();
                        q = q.AsQueryable().Where(p => string.IsNullOrEmpty(p.IS_MIGRATION) &&
                                 (
                                    (p.PRODUCT_CODE.StartsWith("3") && !p.PRODUCT_CODE.StartsWith("3W") && !string.IsNullOrEmpty(p.BOM_ITEM_CUSTOM_1) && p.BOM_ITEM_CUSTOM_1.Contains("NEW") && p.SO_ITEM_IS_ACTIVE == "X" && p.BOM_IS_ACTIVE == "X")
                                    || (p.PRODUCT_CODE.StartsWith("5") && p.PRODUCT_CODE.Length == 18 && p.ITEM_CUSTOM_1 == "0" && p.SO_ITEM_IS_ACTIVE == "X")
                                 )
                                && (listsoNewPIC > 0 ?
                                  !(from m in context.ART_M_PIC
                                    where m.USER_ID != param.data.CURRENT_USER_ID
                                    select new
                                    {
                                        X1 = string.IsNullOrEmpty(m.SOLD_TO_CODE) ? p.SOLD_TO : m.SOLD_TO_CODE,
                                        X2 = string.IsNullOrEmpty(m.SHIP_TO_CODE) ? p.SHIP_TO : m.SHIP_TO_CODE,
                                        X3 = string.IsNullOrEmpty(m.ZONE) ? (
                                        p.ZONE != null ? p.ZONE.Substring(0, 2) : p.ZONE) : m.ZONE,
                                        X4 = string.IsNullOrEmpty(m.COUNTRY) ? p.COUNTRY : m.COUNTRY
                                    }).Contains(new { X1 = p.SOLD_TO, X2 = p.SHIP_TO, X3 = !string.IsNullOrEmpty(p.ZONE) ? p.ZONE.Substring(0, 2) : p.ZONE, X4 = p.COUNTRY }) :
                                    (from m in context.ART_M_PIC
                                     where m.USER_ID == param.data.CURRENT_USER_ID
                                     select new
                                     {
                                         X1 = string.IsNullOrEmpty(m.SOLD_TO_CODE) ? p.SOLD_TO : m.SOLD_TO_CODE,
                                         X2 = string.IsNullOrEmpty(m.SHIP_TO_CODE) ? p.SHIP_TO : m.SHIP_TO_CODE,
                                         X3 = string.IsNullOrEmpty(m.ZONE) ? (
                                         p.ZONE != null ? p.ZONE.Substring(0, 2) : p.ZONE) : m.ZONE,
                                         X4 = string.IsNullOrEmpty(m.COUNTRY) ? p.COUNTRY : m.COUNTRY
                                     }).Contains(new { X1 = p.SOLD_TO, X2 = p.SHIP_TO, X3 = !string.IsNullOrEmpty(p.ZONE) ? p.ZONE.Substring(0, 2) : p.ZONE, X4 = p.COUNTRY }))

                                 && !(from o in context.V_ART_ASSIGNED_SO
                                      where !o.BOM_ITEM_CUSTOM_1.Contains("MULTI")
                                      select new { X1 = o.SALES_ORDER_NO, X2 = o.ITEM, X3 = o.BOM_ITEM_CUSTOM_1 }).Contains(new { X1 = p.SALES_ORDER_NO, X2 = p.ITEM, X3 = p.BOM_ITEM_CUSTOM_1 })).ToList();



                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            q = q.AsQueryable().Where(m => DbFunctions.TruncateTime(m.CREATE_ON) >= getByCreateDateFrom.Date).ToList();
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            q = q = q.AsQueryable().Where(m => DbFunctions.TruncateTime(m.CREATE_ON) <= getByCreateDateTo.Date).ToList();

                        if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_FROM))
                            q = q.AsQueryable().Where(m => DbFunctions.TruncateTime(m.RDD) >= GET_BY_RDD_FROM.Date).ToList();
                        if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_TO))
                            q = q.AsQueryable().Where(m => DbFunctions.TruncateTime(m.RDD) <= GET_BY_RDD_TO.Date).ToList();

                        if (resultPackaging.data != null)
                        {
                            //filter by material 
                            string filterType = "5" + resultPackaging.data.FirstOrDefault().VALUE;

                            //filter by new bom item
                            string description = resultPackaging.data.FirstOrDefault().DESCRIPTION.Replace(" ", "");
                            var packagingTypeMapping = EnumExtension<PackagingTypeMapping>.GetEnumByPackagingType(description);
                            string bomMulti = packagingTypeMapping.GetAttributeBomItemMulti();
                            string bomNonMulti = packagingTypeMapping.GetAttributeBomItemNonMulti();

                            q = q.AsQueryable().Where(m => m.COMPONENT_MATERIAL.StartsWith(filterType) || m.BOM_ITEM_CUSTOM_1.Contains(bomMulti) || m.BOM_ITEM_CUSTOM_1.Contains(bomNonMulti)).ToList();
                        }

                        if (!string.IsNullOrEmpty(getBySoldTo))
                            q = q.AsQueryable().Where(m => m.SOLD_TO + ":" + m.SOLD_TO_NAME == getBySoldTo).ToList();
                        if (!string.IsNullOrEmpty(getByShipTo))
                            q = q.AsQueryable().Where(m => m.SHIP_TO + ":" + m.SHIP_TO_NAME == getByShipTo).ToList();
                        if (!string.IsNullOrEmpty(getByBrand))
                            q = q.AsQueryable().Where(m => m.BRAND_ID + ":" + m.BRAND_DESCRIPTION == getByBrand).ToList();

                        list = q.ToList();

                        //var allPICConfig = ART_M_PIC_SERVICE.GetAll(context);
                        //List<V_SAP_SALES_ORDER_2> tempList = new List<V_SAP_SALES_ORDER_2>();
                        //foreach (var item in list)
                        //{
                        //    var PICUserId = CNService.CheckPICSO2(allPICConfig, item.SOLD_TO, item.SHIP_TO, item.ZONE);
                        //    if (param.data.CURRENT_USER_ID == PICUserId)
                        //    {
                        //        tempList.Add(item);
                        //    }
                        //}
                        //list = tempList;

                        List<SAP_M_LONG_TEXT> listLongText = new List<SAP_M_LONG_TEXT>();
                        if (param.data.GROUPING != "yes")
                        {
                            foreach (var item in list)
                            {
                                item.SOLD_TO_DISPLAY_TXT = item.SOLD_TO + ":" + item.SOLD_TO_NAME;
                                item.SHIP_TO_DISPLAY_TXT = item.SHIP_TO + ":" + item.SHIP_TO_NAME;
                                item.BRAND_DISPLAY_TXT = item.BRAND_ID + ":" + item.BRAND_DESCRIPTION;
                                item.ADDITIONAL_BRAND_DISPLAY_TXT = item.ADDITIONAL_BRAND_ID + ":" + item.ADDITIONAL_BRAND_DESCRIPTION;

                                if (item.SOLD_TO_DISPLAY_TXT.Trim() == ":") item.SOLD_TO_DISPLAY_TXT = "";
                                if (item.SHIP_TO_DISPLAY_TXT.Trim() == ":") item.SHIP_TO_DISPLAY_TXT = "";
                                if (item.BRAND_DISPLAY_TXT.Trim() == ":") item.BRAND_DISPLAY_TXT = "";
                                if (item.ADDITIONAL_BRAND_DISPLAY_TXT.Trim() == ":") item.ADDITIONAL_BRAND_DISPLAY_TXT = "";
                            }
                        }
                        else
                        {
                            foreach (var item in list)
                            {
                                item.SOLD_TO_DISPLAY_TXT = item.SOLD_TO + ":" + item.SOLD_TO_NAME;
                                item.SHIP_TO_DISPLAY_TXT = item.SHIP_TO + ":" + item.SHIP_TO_NAME;
                                item.BRAND_DISPLAY_TXT = item.BRAND_ID + ":" + item.BRAND_DESCRIPTION;
                                item.ADDITIONAL_BRAND_DISPLAY_TXT = item.ADDITIONAL_BRAND_ID + ":" + item.ADDITIONAL_BRAND_DESCRIPTION;

                                if (item.SOLD_TO_DISPLAY_TXT.Trim() == ":") item.SOLD_TO_DISPLAY_TXT = "";
                                if (item.SHIP_TO_DISPLAY_TXT.Trim() == ":") item.SHIP_TO_DISPLAY_TXT = "";
                                if (item.BRAND_DISPLAY_TXT.Trim() == ":") item.BRAND_DISPLAY_TXT = "";
                                if (item.ADDITIONAL_BRAND_DISPLAY_TXT.Trim() == ":") item.ADDITIONAL_BRAND_DISPLAY_TXT = "";

                                var itemForGroup = new V_SAP_SALES_ORDER_2();
                                if (item.PRODUCT_CODE.StartsWith("3"))
                                {
                                    itemForGroup = item;
                                }
                                else
                                {
                                    itemForGroup = list.Where(m => m.SALES_ORDER_NO == item.SALES_ORDER_NO && m.COMPONENT_MATERIAL == item.PRODUCT_CODE).FirstOrDefault();
                                    if (itemForGroup == null)
                                    {
                                        itemForGroup = item;
                                    }
                                }

                                if (itemForGroup != null)
                                {
                                    var WHText = "";
                                    var GeneralText = "";

                                    var tempWHText = listLongText.Where(m => m.TEXT_NAME == itemForGroup.SALES_ORDER_NO && m.TEXT_ID == itemForGroup.ITEM.ToString() && m.TEXT_LANGUAGE == "W").ToList();
                                    if (tempWHText.Count() == 1)
                                    {
                                        WHText = tempWHText.FirstOrDefault().LINE_TEXT;
                                    }
                                    else
                                    {
                                        WHText = CNService.GetWHText(itemForGroup.SALES_ORDER_NO, itemForGroup.ITEM.ToString(), context);
                                        listLongText.Add(new SAP_M_LONG_TEXT() { LINE_TEXT = WHText, TEXT_NAME = itemForGroup.SALES_ORDER_NO, TEXT_ID = itemForGroup.ITEM.ToString(), TEXT_LANGUAGE = "W" });
                                    }

                                    var tempGeneralText = listLongText.Where(m => m.TEXT_NAME == itemForGroup.SALES_ORDER_NO && m.TEXT_LANGUAGE == "G").ToList();
                                    if (tempGeneralText.Count() == 1)
                                    {
                                        GeneralText = tempGeneralText.FirstOrDefault().LINE_TEXT;
                                    }
                                    else
                                    {
                                        GeneralText = CNService.GetGeneralText(itemForGroup.SALES_ORDER_NO, context);
                                        listLongText.Add(new SAP_M_LONG_TEXT() { LINE_TEXT = GeneralText, TEXT_NAME = itemForGroup.SALES_ORDER_NO, TEXT_LANGUAGE = "G" });
                                    }

                                    item.GROUPING = itemForGroup.BOM_ITEM_CUSTOM_1 + itemForGroup.PRODUCT_CODE + itemForGroup.COMPONENT_MATERIAL
                                                      + WHText
                                                      + GeneralText
                                                      + itemForGroup.SHIP_TO + itemForGroup.SOLD_TO + itemForGroup.COUNTRY + itemForGroup.IN_TRANSIT_TO + itemForGroup.ADDITIONAL_BRAND_ID + itemForGroup.BRAND_ID;

                                    //item.GROUPING = EncryptionService.EncryptGrouping(item.GROUPING);
                                    //Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                                    //item.GROUPING = rgx.Replace(item.GROUPING, "");

                                    item.GROUPING_DISPLAY_TXT = "<b>Product code</b> : " + itemForGroup.PRODUCT_CODE + "<br />" +
                                                                         "<b>Country</b> : " + itemForGroup.COUNTRY + "<br />" +
                                                                         "<b>In transit to</b> : " + itemForGroup.IN_TRANSIT_TO + "<br />" +
                                                                         "<b>Additional brand</b> : " + itemForGroup.ADDITIONAL_BRAND_ID + ":" + itemForGroup.ADDITIONAL_BRAND_DESCRIPTION + "<br />" +
                                                                         "<b>Packaging code(BOM component)</b> : " + itemForGroup.COMPONENT_MATERIAL + "<br />" +
                                                                         "<b>Sold to</b> : " + itemForGroup.SOLD_TO + ":" + itemForGroup.SOLD_TO_NAME + "<br />" +
                                                                         "<b>Ship to</b> : " + itemForGroup.SHIP_TO + ":" + itemForGroup.SHIP_TO_NAME + "<br />" +
                                                                         "<b>Brand</b> : " + itemForGroup.BRAND_ID + ":" + itemForGroup.BRAND_DESCRIPTION + "<br />" +
                                                                         "<b>PKG & warehouse text</b> : " + WHText + "<br />" +
                                                                         "<b>General text</b> : " + GeneralText + "<br />";
                                }
                            }

                            var tempList2 = new List<V_SAP_SALES_ORDER_2>();
                            foreach (var item in list)
                            {
                                tempList2.Add(item);
                                if (!string.IsNullOrEmpty(item.COMPONENT_MATERIAL))
                                {
                                    var twoDigitMat5 = CNService.SubString(item.COMPONENT_MATERIAL, 2);
                                    var listfoc = context.Database.SqlQuery<V_SAP_SALES_ORDER2>(@"SP_SALES_ORDER_FOC @SALES_ORDER_NO,@ITEM_CUSTOM_1,@PRODUCT_CODE",
                                                new SqlParameter("@SALES_ORDER_NO", item.SALES_ORDER_NO),
                                                new SqlParameter("@ITEM_CUSTOM_1", string.Format("{0}", item.ITEM)),
                                                new SqlParameter("@PRODUCT_CODE", string.Format("{0}", twoDigitMat5))).ToList();
                                    var chkListFOC = listfoc.Select(m => new V_SAP_SALES_ORDER_2
                                    {
                                        ITEM = m.ITEM,
                                        PRODUCT_CODE = m.PRODUCT_CODE,
                                        SALES_ORDER_NO = m.SALES_ORDER_NO
                                    }).ToList();
                                    //var chkListFOC = (from m in context.V_SAP_SALES_ORDER2
                                    //                  where string.IsNullOrEmpty(m.IS_MIGRATION)
                                    //                  && m.SALES_ORDER_NO == item.SALES_ORDER_NO
                                    //                  && m.ITEM_CUSTOM_1 == item.ITEM.ToString()
                                    //                  && m.PRODUCT_CODE.StartsWith(twoDigitMat5)
                                    //                  && m.SO_ITEM_IS_ACTIVE == "X"
                                    //                  select new V_SAP_SALES_ORDER_2
                                    //                  {
                                    //                      ITEM = m.ITEM,
                                    //                      PRODUCT_CODE = m.PRODUCT_CODE,
                                    //                      SALES_ORDER_NO = m.SALES_ORDER_NO
                                    //                  }).ToList();

                                    foreach (var itemFOC in chkListFOC)
                                    {
                                        var tempItemFOC = itemFOC;
                                        tempItemFOC.SALES_ORG = item.SALES_ORG;
                                        tempItemFOC.SOLD_TO_DISPLAY_TXT = item.SOLD_TO + ":" + item.SOLD_TO_NAME;
                                        tempItemFOC.SHIP_TO_DISPLAY_TXT = item.SHIP_TO + ":" + item.SHIP_TO_NAME;
                                        tempItemFOC.BRAND_DISPLAY_TXT = item.BRAND_ID + ":" + item.BRAND_DESCRIPTION;
                                        tempItemFOC.ADDITIONAL_BRAND_DISPLAY_TXT = item.ADDITIONAL_BRAND_ID + ":" + item.ADDITIONAL_BRAND_DESCRIPTION;

                                        if (tempItemFOC.SOLD_TO_DISPLAY_TXT.Trim() == ":") tempItemFOC.SOLD_TO_DISPLAY_TXT = "";
                                        if (tempItemFOC.SHIP_TO_DISPLAY_TXT.Trim() == ":") tempItemFOC.SHIP_TO_DISPLAY_TXT = "";
                                        if (tempItemFOC.BRAND_DISPLAY_TXT.Trim() == ":") tempItemFOC.BRAND_DISPLAY_TXT = "";
                                        if (tempItemFOC.ADDITIONAL_BRAND_DISPLAY_TXT.Trim() == ":") tempItemFOC.ADDITIONAL_BRAND_DISPLAY_TXT = "";

                                        var itemForGroup = item;

                                        tempItemFOC.GROUPING = itemForGroup.GROUPING;
                                        tempItemFOC.GROUPING_DISPLAY_TXT = itemForGroup.GROUPING_DISPLAY_TXT;

                                        tempList2.Add(tempItemFOC);
                                    }
                                }
                            }
                            list = tempList2;
                        }

                        foreach (var item in list)
                        {
                            item.GROUPINGTEMP = item.GROUPING;
                        }

                        var cntGroup = 1;
                        foreach (var item in list)
                        {
                            var found = false;
                            var temp = list.Where(m => m.GROUPING == item.GROUPINGTEMP).ToList();
                            foreach (var itemTemp in temp)
                            {
                                found = true;
                                itemTemp.GROUPING = cntGroup.ToString();
                            }
                            if (found) cntGroup++;
                        }
                    }
                }
                Results.data = list;
                Results.recordsFiltered = Results.data.ToList().Count;
                Results.recordsTotal = Results.data.ToList().Count;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static V_SAP_SALES_ORDER_RESULT GetSalesOrderRepeat2(V_SAP_SALES_ORDER_REQUEST param)
        {
            V_SAP_SALES_ORDER_RESULT Results = new V_SAP_SALES_ORDER_RESULT();
            if (param.data.FIRST_LOAD)
            {
                Results.status = "S";
                Results.data = new List<V_SAP_SALES_ORDER_2>();
                Results.draw = param.draw;
                return Results;
            }
            try
            {
                var getByCreateDateFrom = DateTime.Now;
                var getByCreateDateTo = DateTime.Now;
                var GET_BY_RDD_FROM = DateTime.Now;
                var GET_BY_RDD_TO = DateTime.Now;

                if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                    getByCreateDateFrom = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_FROM);
                if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                    getByCreateDateTo = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_TO);

                if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_FROM))
                    GET_BY_RDD_FROM = CNService.ConvertStringToDate(param.data.GET_BY_RDD_FROM);
                if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_TO))
                    GET_BY_RDD_TO = CNService.ConvertStringToDate(param.data.GET_BY_RDD_TO);

                var getByPackangingType = param.data.GET_BY_PACKAGING_TYPE;
                var getBySoldTo = param.data.GET_BY_SOLD_TO;
                var getByShipTo = param.data.GET_BY_SHIP_TO;
                var getByBrand = param.data.GET_BY_BRAND;

                SAP_M_CHARACTERISTIC_RESULT resultPackaging = new SAP_M_CHARACTERISTIC_RESULT();
                if (getByPackangingType != null)
                {
                    SAP_M_CHARACTERISTIC_REQUEST req = new SAP_M_CHARACTERISTIC_REQUEST();
                    req.data = new SAP_M_CHARACTERISTIC_2();
                    req.data.VALUE = getByPackangingType.Substring(0, 1);
                    resultPackaging = ItemPackingTypeHelper.GetPackType(req);
                }
                List<V_SAP_SALES_ORDER_2> list = new List<V_SAP_SALES_ORDER_2>();
                //var listDataSO = CNService.GetDataSO(param.data.CURRENT_USER_ID);

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;
                        var listPIC = (from m in context.ART_M_PIC
                                       where m.USER_ID == param.data.CURRENT_USER_ID && string.IsNullOrEmpty(m.SOLD_TO_CODE)
                                       select m.PIC_ID).Count();
                        var qOrder = context.Database.SqlQuery<V_SAP_SALES_ORDER_2>(@"spGetSalesOrderRepeat @CreateDateFrom,
                            @CreateDateTo,@RDD_FROM,@RDD_TO,@COMPONENT_MATERIAL,@SOLD_TO,@SHIP_TO,@BRAND_ID",
                            new SqlParameter("@CreateDateFrom", !string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM)?string.Format("{0}", getByCreateDateFrom.Date) : ""),
                            new SqlParameter("@CreateDateTo", !string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO)?string.Format("{0}", getByCreateDateTo.Date):""),
                            new SqlParameter("@RDD_FROM", !string.IsNullOrEmpty(param.data.GET_BY_RDD_FROM)?string.Format("{0}", GET_BY_RDD_FROM.Date):""),
                            new SqlParameter("@RDD_TO", !string.IsNullOrEmpty(param.data.GET_BY_RDD_TO)?string.Format("{0}", GET_BY_RDD_TO.Date):""),
                            new SqlParameter("@COMPONENT_MATERIAL", getByPackangingType != null?string.Format ("5{0}" ,resultPackaging.data.FirstOrDefault().VALUE):""),
                                                new SqlParameter("@SOLD_TO", string.Format("{0}", getBySoldTo)),
                                                new SqlParameter("@SHIP_TO", string.Format("{0}", getByShipTo)),
                                                new SqlParameter("@BRAND_ID", string.Format("{0}", getByBrand))).ToList();
                        var q = qOrder.Select(p => new V_SAP_SALES_ORDER_2
                        {
                            DECRIPTION = p.DECRIPTION,
                            COMPONENT_ITEM = p.COMPONENT_ITEM,
                            SALES_ORG = p.SALES_ORG,
                            SOLD_TO = p.SOLD_TO,
                            SOLD_TO_NAME = p.SOLD_TO_NAME,
                            SHIP_TO = p.SHIP_TO,
                            SHIP_TO_NAME = p.SHIP_TO_NAME,
                            BRAND_ID = p.BRAND_ID,
                            BRAND_DESCRIPTION = p.BRAND_DESCRIPTION,
                            ADDITIONAL_BRAND_ID = p.ADDITIONAL_BRAND_ID,
                            ADDITIONAL_BRAND_DESCRIPTION = p.ADDITIONAL_BRAND_DESCRIPTION,
                            ZONE = p.ZONE,
                            CREATE_ON = p.CREATE_ON,
                            RDD = p.RDD,
                            COMPONENT_MATERIAL = p.COMPONENT_MATERIAL,
                            PRODUCT_CODE = p.PRODUCT_CODE,
                            SALES_ORDER_NO = p.SALES_ORDER_NO,
                            ITEM = p.ITEM,
                            BOM_ITEM_CUSTOM_1 = p.BOM_ITEM_CUSTOM_1,
                            ITEM_CUSTOM_1 = p.ITEM_CUSTOM_1,
                            COUNTRY = p.COUNTRY,
                            IN_TRANSIT_TO = p.IN_TRANSIT_TO,
                            BOM_STOCK = p.BOM_STOCK,
                            STOCK = p.STOCK,
                            QUANTITY = p.QUANTITY > 0 ? p.QUANTITY : p.ORDER_QTY,
                            PLANT = p.PRODUCTION_PLANT,
                        }).ToList();
                        q = q.AsQueryable().Where(p => string.IsNullOrEmpty(p.IS_MIGRATION) &&
                                (
                                    //ทั่วไป
                                    (p.PRODUCT_CODE.StartsWith("3") && !p.PRODUCT_CODE.StartsWith("3W") && !string.IsNullOrEmpty(p.COMPONENT_MATERIAL) && p.COMPONENT_MATERIAL.StartsWith("5") && p.COMPONENT_MATERIAL.Length == 18 && string.IsNullOrEmpty(p.BOM_ITEM_CUSTOM_1) && p.SO_ITEM_IS_ACTIVE == "X" && p.BOM_IS_ACTIVE == "X")
                                    //3v
                                    || (p.PRODUCT_CODE.StartsWith("5") && p.PRODUCT_CODE.Length == 18 && string.IsNullOrEmpty(p.ITEM_CUSTOM_1) && p.SO_ITEM_IS_ACTIVE == "X")
                                    //ของแถมที่ไม่ได้ ref กับใคร
                                    || (p.PRODUCT_CODE.StartsWith("5") && p.PRODUCT_CODE.Length == 18 && p.ITEM_CUSTOM_1 == "0" && p.SO_ITEM_IS_ACTIVE == "X")
                                ) &&
                                (listPIC > 0 ?
                                 !(from m in context.ART_M_PIC
                                   where m.USER_ID != param.data.CURRENT_USER_ID
                                   select new
                                   {
                                       X1 = string.IsNullOrEmpty(m.SOLD_TO_CODE) ? p.SOLD_TO : m.SOLD_TO_CODE,
                                       X2 = string.IsNullOrEmpty(m.SHIP_TO_CODE) ? p.SHIP_TO : m.SHIP_TO_CODE,
                                       X3 = string.IsNullOrEmpty(m.ZONE) ? (
                                       p.ZONE != null ? p.ZONE.Substring(0, 2) : p.ZONE) : m.ZONE,
                                       X4 = string.IsNullOrEmpty(m.COUNTRY) ? p.COUNTRY : m.COUNTRY
                                   }).Contains(new { X1 = p.SOLD_TO, X2 = p.SHIP_TO, X3 = !string.IsNullOrEmpty(p.ZONE) ? p.ZONE.Substring(0, 2) : p.ZONE, X4 = p.COUNTRY }) :
                                   (from m in context.ART_M_PIC
                                    where m.USER_ID == param.data.CURRENT_USER_ID
                                    select new
                                    {
                                        X1 = string.IsNullOrEmpty(m.SOLD_TO_CODE) ? p.SOLD_TO : m.SOLD_TO_CODE,
                                        X2 = string.IsNullOrEmpty(m.SHIP_TO_CODE) ? p.SHIP_TO : m.SHIP_TO_CODE,
                                        X3 = string.IsNullOrEmpty(m.ZONE) ? (
                                        p.ZONE != null ? p.ZONE.Substring(0, 2) : p.ZONE) : m.ZONE,
                                        X4 = string.IsNullOrEmpty(m.COUNTRY) ? p.COUNTRY : m.COUNTRY
                                    }).Contains(new { X1 = p.SOLD_TO, X2 = p.SHIP_TO, X3 = !string.IsNullOrEmpty(p.ZONE) ? p.ZONE.Substring(0, 2) : p.ZONE, X4 = p.COUNTRY }))

                                && !(from o in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                     select new { X1 = o.SALES_ORDER_NO, X2 = o.SALES_ORDER_ITEM, X3 = o.PRODUCT_CODE, X4 = o.COMPONENT_MATERIAL }).Contains(new { X1 = p.SALES_ORDER_NO, X2 = p.ITEM.ToString(), X3 = p.PRODUCT_CODE, X4 = p.COMPONENT_MATERIAL })

                                && !(from o in context.V_ART_ASSIGNED_SO
                                     select new { X1 = o.SALES_ORDER_NO, X2 = o.ITEM, X3 = o.COMPONENT_MATERIAL }).Contains(new { X1 = p.SALES_ORDER_NO, X2 = p.ITEM, X3 = p.COMPONENT_MATERIAL })).ToList();


                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            q = q.AsQueryable().Where(m => DbFunctions.TruncateTime(m.CREATE_ON) >= getByCreateDateFrom.Date).ToList();
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            q = q.AsQueryable().Where(m => DbFunctions.TruncateTime(m.CREATE_ON) <= getByCreateDateTo.Date).ToList();

                        if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_FROM))
                            q = q.AsQueryable().Where(m => DbFunctions.TruncateTime(m.RDD) >= GET_BY_RDD_FROM.Date).ToList();
                        if (!string.IsNullOrEmpty(param.data.GET_BY_RDD_TO))
                            q = q.AsQueryable().Where(m => DbFunctions.TruncateTime(m.RDD) <= GET_BY_RDD_TO.Date).ToList();

                        if (resultPackaging.data != null)
                        {
                            string filterType = "5" + resultPackaging.data.FirstOrDefault().VALUE;
                            q = q.AsQueryable().Where(m => m.COMPONENT_MATERIAL.StartsWith(filterType)).ToList();
                        }

                        if (!string.IsNullOrEmpty(getBySoldTo))
                            q = q.AsQueryable().Where(m => m.SOLD_TO + ":" + m.SOLD_TO_NAME == getBySoldTo).ToList();
                        if (!string.IsNullOrEmpty(getByShipTo))
                            q = q.AsQueryable().Where(m => m.SHIP_TO + ":" + m.SHIP_TO_NAME == getByShipTo).ToList();
                        if (!string.IsNullOrEmpty(getByBrand))
                            q = q.AsQueryable().Where(m => m.BRAND_ID + ":" + m.BRAND_DESCRIPTION == getByBrand).ToList();

                        list = q.ToList();
                        
                        //list = (from p in list  where listDataSO.Equals(p.PO_COMPLETE_SO_HEADER_ID)
                        //         select p).ToList();

                        //var allPICConfig = ART_M_PIC_SERVICE.GetAll(context);
                        //List<V_SAP_SALES_ORDER_2> tempList = new List<V_SAP_SALES_ORDER_2>();
                        //foreach (var item in list)
                        //{
                        //    var PICUserId = CNService.CheckPICSO2(allPICConfig, item.SOLD_TO, item.SHIP_TO, item.ZONE);
                        //    if (param.data.CURRENT_USER_ID == PICUserId)
                        //    {
                        //        tempList.Add(item);
                        //    }
                        //}
                        //list = tempList;

                        List <SAP_M_LONG_TEXT> listLongText = new List<SAP_M_LONG_TEXT>();
                        //foreach (var item in list)
                        //{
                        //    item.RECHECK_ARTWORK = "";

                        //    var mat5 = item.COMPONENT_MATERIAL;
                        //    var wfAssignSOComplete = (from m in context.ART_WF_ARTWORK_PROCESS
                        //                              join k in context.V_ART_ASSIGNED_SO on m.ARTWORK_SUB_ID equals k.ARTWORK_SUB_ID
                        //                              where k.COMPONENT_MATERIAL == mat5 && m.IS_END == "X" && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                        //                              select k.RDD).ToList();

                        //    if (wfAssignSOComplete.Count > 0)
                        //    {
                        //        wfAssignSOComplete = wfAssignSOComplete.OrderByDescending(m => m).ToList();
                        //        DateTime rdd = Convert.ToDateTime(wfAssignSOComplete[0].Value);
                        //        DateTime currentDate = DateTime.Now.Date;
                        //        if (currentDate > rdd)
                        //        {
                        //            int diffMonth = FormNumberHelper.GetMonthDifference(currentDate, rdd);
                        //            if (diffMonth > 6)
                        //            {
                        //                item.RECHECK_ARTWORK = "Yes";
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        var listSO = (from m in context.V_SAP_SALES_ORDER
                        //                      where m.COMPONENT_MATERIAL == mat5 && m.IS_MIGRATION == "X"
                        //                      select m.RDD).ToList();
                        //        if (listSO.Count > 0)
                        //        {
                        //            listSO = listSO.OrderByDescending(m => m).ToList();
                        //            if (listSO[0] != null)
                        //            {
                        //                DateTime rdd = Convert.ToDateTime(listSO[0].Value);
                        //                DateTime currentDate = DateTime.Now.Date;
                        //                if (currentDate > rdd)
                        //                {
                        //                    int diffMonth = FormNumberHelper.GetMonthDifference(currentDate, rdd);
                        //                    if (diffMonth > 6)
                        //                    {
                        //                        item.RECHECK_ARTWORK = "Yes";
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    }                            
                        if (param.data.GROUPING != "yes")
                        {
                            foreach (var item in list)
                            {
                                DateTime currentDate = DateTime.Now.Date;
                                var mat5 = item.COMPONENT_MATERIAL;
                                item.RECHECK_ARTWORK = "";
                                if (!string.IsNullOrEmpty(item.COMPONENT_MATERIAL))
                                {
                                    if (item.COMPONENT_MATERIAL.StartsWith("5"))
                                        item.RECHECK_ARTWORK = string.Format("{0}", CNService.GetCheckRDD(mat5));
                                }
                                item.SOLD_TO_DISPLAY_TXT = item.SOLD_TO + ":" + item.SOLD_TO_NAME;
                                item.SHIP_TO_DISPLAY_TXT = item.SHIP_TO + ":" + item.SHIP_TO_NAME;
                                item.BRAND_DISPLAY_TXT = item.BRAND_ID + ":" + item.BRAND_DESCRIPTION;
                                item.ADDITIONAL_BRAND_DISPLAY_TXT = item.ADDITIONAL_BRAND_ID + ":" + item.ADDITIONAL_BRAND_DESCRIPTION;

                                if (item.SOLD_TO_DISPLAY_TXT.Trim() == ":") item.SOLD_TO_DISPLAY_TXT = "";
                                if (item.SHIP_TO_DISPLAY_TXT.Trim() == ":") item.SHIP_TO_DISPLAY_TXT = "";
                                if (item.BRAND_DISPLAY_TXT.Trim() == ":") item.BRAND_DISPLAY_TXT = "";
                                if (item.ADDITIONAL_BRAND_DISPLAY_TXT.Trim() == ":") item.ADDITIONAL_BRAND_DISPLAY_TXT = "";

                                if (string.IsNullOrEmpty(item.STOCK)) item.STOCK = "";
                                if (string.IsNullOrEmpty(item.BOM_STOCK)) item.BOM_STOCK = "";

                                if (!string.IsNullOrEmpty(item.BOM_STOCK))
                                    item.BOM_STOCK = item.BOM_STOCK;
                                else
                                    item.BOM_STOCK = item.STOCK;
                            }
                        }
                        else
                        {
                            foreach (var item in list)
                            {
                                DateTime currentDate = DateTime.Now.Date;
                                var mat5 = item.COMPONENT_MATERIAL;
                                item.RECHECK_ARTWORK = "";
                                if (!string.IsNullOrEmpty(item.COMPONENT_MATERIAL))
                                {
                                    if (item.COMPONENT_MATERIAL.StartsWith("5"))
                                        item.RECHECK_ARTWORK = string.Format("{0}", CNService.GetCheckRDD(mat5));
                                }
                                item.SOLD_TO_DISPLAY_TXT = item.SOLD_TO + ":" + item.SOLD_TO_NAME;
                                item.SHIP_TO_DISPLAY_TXT = item.SHIP_TO + ":" + item.SHIP_TO_NAME;
                                item.BRAND_DISPLAY_TXT = item.BRAND_ID + ":" + item.BRAND_DESCRIPTION;
                                item.ADDITIONAL_BRAND_DISPLAY_TXT = item.ADDITIONAL_BRAND_ID + ":" + item.ADDITIONAL_BRAND_DESCRIPTION;

                                if (item.SOLD_TO_DISPLAY_TXT.Trim() == ":") item.SOLD_TO_DISPLAY_TXT = "";
                                if (item.SHIP_TO_DISPLAY_TXT.Trim() == ":") item.SHIP_TO_DISPLAY_TXT = "";
                                if (item.BRAND_DISPLAY_TXT.Trim() == ":") item.BRAND_DISPLAY_TXT = "";
                                if (item.ADDITIONAL_BRAND_DISPLAY_TXT.Trim() == ":") item.ADDITIONAL_BRAND_DISPLAY_TXT = "";

                                if (string.IsNullOrEmpty(item.STOCK)) item.STOCK = "";
                                if (string.IsNullOrEmpty(item.BOM_STOCK)) item.BOM_STOCK = "";

                                if (!string.IsNullOrEmpty(item.BOM_STOCK))
                                    item.BOM_STOCK = item.BOM_STOCK;
                                else
                                    item.BOM_STOCK = item.STOCK;

                                var itemForGroup = new V_SAP_SALES_ORDER_2();
                                if (item.PRODUCT_CODE.StartsWith("3"))
                                {
                                    itemForGroup = item;
                                }
                                else
                                {
                                    itemForGroup = list.Where(m => m.SALES_ORDER_NO == item.SALES_ORDER_NO && m.COMPONENT_MATERIAL == item.PRODUCT_CODE).FirstOrDefault();
                                    if (itemForGroup == null)
                                    {
                                        itemForGroup = item;
                                    }
                                }

                                if (itemForGroup != null)
                                {
                                    var WHText = "";
                                    var GeneralText = "";

                                    var tempWHText = listLongText.Where(m => m.TEXT_NAME == itemForGroup.SALES_ORDER_NO && m.TEXT_ID == itemForGroup.ITEM.ToString() && m.TEXT_LANGUAGE == "W").ToList();
                                    if (tempWHText.Count() == 1)
                                    {
                                        WHText = tempWHText.FirstOrDefault().LINE_TEXT;
                                    }
                                    else
                                    {
                                        WHText = CNService.GetWHText(itemForGroup.SALES_ORDER_NO, itemForGroup.ITEM.ToString(), context);
                                        listLongText.Add(new SAP_M_LONG_TEXT() { LINE_TEXT = WHText, TEXT_NAME = itemForGroup.SALES_ORDER_NO, TEXT_ID = itemForGroup.ITEM.ToString(), TEXT_LANGUAGE = "W" });
                                    }

                                    var tempGeneralText = listLongText.Where(m => m.TEXT_NAME == itemForGroup.SALES_ORDER_NO && m.TEXT_LANGUAGE == "G").ToList();
                                    if (tempGeneralText.Count() == 1)
                                    {
                                        GeneralText = tempGeneralText.FirstOrDefault().LINE_TEXT;
                                    }
                                    else
                                    {
                                        GeneralText = CNService.GetGeneralText(itemForGroup.SALES_ORDER_NO, context);
                                        listLongText.Add(new SAP_M_LONG_TEXT() { LINE_TEXT = GeneralText, TEXT_NAME = itemForGroup.SALES_ORDER_NO, TEXT_LANGUAGE = "G" });
                                    }

                                    item.GROUPING = itemForGroup.BOM_ITEM_CUSTOM_1 + itemForGroup.PRODUCT_CODE + itemForGroup.COMPONENT_MATERIAL
                                                      + WHText
                                                      + GeneralText
                                                      + itemForGroup.SHIP_TO + itemForGroup.SOLD_TO + itemForGroup.COUNTRY + itemForGroup.IN_TRANSIT_TO + itemForGroup.ADDITIONAL_BRAND_ID + itemForGroup.BRAND_ID;

                                    //item.GROUPING = EncryptionService.EncryptGrouping(item.GROUPING);
                                    //Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                                    //item.GROUPING = rgx.Replace(item.GROUPING, "");

                                    item.GROUPING_DISPLAY_TXT = "<b>Product code</b> : " + itemForGroup.PRODUCT_CODE + "<br />" +
                                                                         "<b>Country</b> : " + itemForGroup.COUNTRY + "<br />" +
                                                                         "<b>In transit to</b> : " + itemForGroup.IN_TRANSIT_TO + "<br />" +
                                                                         "<b>Additional brand</b> : " + itemForGroup.ADDITIONAL_BRAND_ID + ":" + itemForGroup.ADDITIONAL_BRAND_DESCRIPTION + "<br />" +
                                                                         "<b>Packaging code(BOM component)</b> : " + itemForGroup.COMPONENT_MATERIAL + "<br />" +
                                                                         "<b>Sold to</b> : " + itemForGroup.SOLD_TO + ":" + itemForGroup.SOLD_TO_NAME + "<br />" +
                                                                         "<b>Ship to</b> : " + itemForGroup.SHIP_TO + ":" + itemForGroup.SHIP_TO_NAME + "<br />" +
                                                                         "<b>Brand</b> : " + itemForGroup.BRAND_ID + ":" + itemForGroup.BRAND_DESCRIPTION + "<br />" +
                                                                         "<b>PKG & warehouse text</b> : " + WHText + "<br />" +
                                                                         "<b>General text</b> : " + GeneralText + "<br />";
                                }
                            }
                        }
                        var listSaleOrder = list.Select(m => m.SALES_ORDER_NO).Distinct().ToList();

                            var listSODetail = (from m in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                                where listSaleOrder.Contains(m.SALES_ORDER_NO)
                                                select m).ToList();
                        //var listSAPSO = qOrder.Where(m => listSaleOrder.Contains(m.SALES_ORDER_NO)).ToList();
                        var listSAPSO = (from m in context.V_SAP_SALES_ORDER2
                                         where listSaleOrder.Contains(m.SALES_ORDER_NO)
                                         select new V_SAP_SALES_ORDER_2
                                         {
                                             ITEM_CUSTOM_1 = m.ITEM_CUSTOM_1,
                                             ITEM = m.ITEM,
                                             PRODUCT_CODE = m.PRODUCT_CODE,
                                             SALES_ORDER_NO = m.SALES_ORDER_NO,
                                             SO_ITEM_IS_ACTIVE = m.SO_ITEM_IS_ACTIVE,
                                             IS_MIGRATION = m.IS_MIGRATION
                                         }).ToList();
                        //listSAPSO = listSAPSO.Select(m => new V_SAP_SALES_ORDER_2
                        //{
                        //    ITEM_CUSTOM_1 = m.ITEM_CUSTOM_1,
                        //    ITEM = m.ITEM,
                        //    PRODUCT_CODE = m.PRODUCT_CODE,
                        //    SALES_ORDER_NO = m.SALES_ORDER_NO,
                        //    SO_ITEM_IS_ACTIVE = m.SO_ITEM_IS_ACTIVE,
                        //    IS_MIGRATION = m.IS_MIGRATION
                        //}).ToList();
                        var tempList2 = new List<V_SAP_SALES_ORDER_2>();
                            foreach (var item in list)
                            {
                                tempList2.Add(item);

                                var chkListFOC = (from m in listSAPSO
                                                  where string.IsNullOrEmpty(m.IS_MIGRATION)
                                                  && m.SALES_ORDER_NO == item.SALES_ORDER_NO
                                                  && m.ITEM_CUSTOM_1 == item.ITEM.ToString()
                                                  && m.PRODUCT_CODE == item.COMPONENT_MATERIAL
                                                  && m.SO_ITEM_IS_ACTIVE == "X"
                                                  select new V_SAP_SALES_ORDER_2
                                                  {
                                                      ITEM_CUSTOM_1 = m.ITEM_CUSTOM_1,
                                                      ITEM = m.ITEM,
                                                      PRODUCT_CODE = m.PRODUCT_CODE,
                                                      SALES_ORDER_NO = m.SALES_ORDER_NO
                                                  }).ToList();

                                foreach (var itemFOC in chkListFOC)
                                {
                                    var tempItemFOC = itemFOC;
                                    tempItemFOC.ITEM_CUSTOM_1 = itemFOC.ITEM_CUSTOM_1;
                                    tempItemFOC.SALES_ORG = item.SALES_ORG;
                                    tempItemFOC.SOLD_TO_DISPLAY_TXT = item.SOLD_TO + ":" + item.SOLD_TO_NAME;
                                    tempItemFOC.SHIP_TO_DISPLAY_TXT = item.SHIP_TO + ":" + item.SHIP_TO_NAME;
                                    tempItemFOC.BRAND_DISPLAY_TXT = item.BRAND_ID + ":" + item.BRAND_DESCRIPTION;
                                    tempItemFOC.ADDITIONAL_BRAND_DISPLAY_TXT = item.ADDITIONAL_BRAND_ID + ":" + item.ADDITIONAL_BRAND_DESCRIPTION;

                                    if (tempItemFOC.SOLD_TO_DISPLAY_TXT.Trim() == ":") tempItemFOC.SOLD_TO_DISPLAY_TXT = "";
                                    if (tempItemFOC.SHIP_TO_DISPLAY_TXT.Trim() == ":") tempItemFOC.SHIP_TO_DISPLAY_TXT = "";
                                    if (tempItemFOC.BRAND_DISPLAY_TXT.Trim() == ":") tempItemFOC.BRAND_DISPLAY_TXT = "";
                                    if (tempItemFOC.ADDITIONAL_BRAND_DISPLAY_TXT.Trim() == ":") tempItemFOC.ADDITIONAL_BRAND_DISPLAY_TXT = "";

                                    var itemForGroup = item;

                                    tempItemFOC.GROUPING = itemForGroup.GROUPING;
                                    tempItemFOC.GROUPING_DISPLAY_TXT = itemForGroup.GROUPING_DISPLAY_TXT;

                                    var SALES_ORDER_ITEM = Convert.ToInt32(itemFOC.ITEM).ToString();
                                    //if (CNService.IsDevOrQAS())
                                    //{
                                    var cntFOC = (from m in listSODetail
                                                  where m.SALES_ORDER_NO == itemFOC.SALES_ORDER_NO
                                                  && m.SALES_ORDER_ITEM == SALES_ORDER_ITEM
                                                  select m).Count();
                                    if (cntFOC == 0)
                                        tempList2.Add(tempItemFOC);
                                    //}
                                    //else
                                    //{
                                    //    tempList2.Add(tempItemFOC);
                                    //}
                                }
                            }
                            list = tempList2;

                            foreach (var item in list)
                            {
                                item.GROUPINGTEMP = item.GROUPING;
                            }

                            var cntGroup = 1;
                            foreach (var item in list)
                            {
                                var found = false;
                                var temp = list.Where(m => m.GROUPING == item.GROUPINGTEMP).ToList();
                                foreach (var itemTemp in temp)
                                {
                                    found = true;
                                    itemTemp.GROUPING = cntGroup.ToString();
                                }
                                if (found) cntGroup++;
                            }

                            int curentGroup = 1;
                            List<int> listGrouping = list.GroupBy(g => new { Grouping = g.GROUPING }).Select(g => Convert.ToInt32(g.Key.Grouping)).ToList();

                            foreach (var item in listGrouping)
                            {
                                curentGroup = item > 1 ? item : curentGroup;
                                var listCurrentGroup = list.Where(m => m.GROUPING == curentGroup.ToString()).ToList();
                                var listBeforeGroup = list.Where(m => m.GROUPING == (curentGroup - 1).ToString()).ToList();

                                //case group > 1
                                if (listBeforeGroup.Count() > 0)
                                {
                                    int maxBeforeGroup = listBeforeGroup.Select(m => m.GROUP_MAX_ROW).FirstOrDefault();
                                    listCurrentGroup.ForEach(a =>
                                    {
                                        a.GROUP_MIN_ROW = maxBeforeGroup + 1;
                                        a.GROUP_MAX_ROW = maxBeforeGroup + listCurrentGroup.Count();
                                    //a.SELECTED_GROUP = 0;
                                });
                                }
                                //case group = 1
                                else
                                {
                                    listCurrentGroup.ForEach(a =>
                                    {
                                        a.GROUP_MIN_ROW = curentGroup;
                                        a.GROUP_MAX_ROW = listCurrentGroup.Count();
                                    //a.SELECTED_GROUP = 0;
                                });
                                }
                            }
                    }
                }
                Results.data = list;
                Results.recordsFiltered = Results.data.ToList().Count;
                Results.recordsTotal = Results.data.ToList().Count;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        //---------------------------- start tuning performance sorepeat 2022 by aof-------------------------------------------------------------------------//

        public static ART_WF_ARTWORK_REQUEST_2 CreateRFBySORepeat_Tuning(ART_WF_ARTWORK_REQUEST_2 objAWRequest,int cnt,string username,string stampdatetime)
        {

            //var requestFormNoError = "";
            //List<long> listAWNodeIdError = new List<long>();

            CNService.SaveLogAction("SORepeatTuning", "START", username, objAWRequest.CONTROL_NAME, stampdatetime);
         

            try
            {
                var token = CWSService.getAuthToken();
                List<string> lisDistinctMaterialDesciption = new List<string>();
                var msg = "";

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                     
                        //Validating
                        msg = ArtworkRequestHelper.ValidatingSaleOrderBySORepeat(objAWRequest, context,token, ref lisDistinctMaterialDesciption);
                        if (!string.IsNullOrEmpty(msg))
                        {
                            objAWRequest.RESULT_CREATE_WF_MESSAGE = msg;
                            objAWRequest.RESULT_CREATE_WF_STATUS = "E";
                            CNService.SaveLogAction("SORepeatTuning", "END", username, objAWRequest.CONTROL_NAME, msg);
                            return objAWRequest;
                        }

                        //Create Node in CS
                        msg = ArtworkRequestHelper.CreateOpentextNodeRequestFormbySoRepeat(ref objAWRequest, context, token, lisDistinctMaterialDesciption, username);
                        if (!string.IsNullOrEmpty(msg))
                        {
                            deleteCS(objAWRequest.REQUEST_FROM_NO_ERROR, objAWRequest.LIST_AW_NODE_ID);
                            objAWRequest.RESULT_CREATE_WF_MESSAGE = msg;
                            objAWRequest.RESULT_CREATE_WF_STATUS = "E";
                            CNService.SaveLogAction("SORepeatTuning", "END", username, objAWRequest.CONTROL_NAME, msg);
                            return objAWRequest;
                        }


                        //Prepare Requset Form
                        msg = ArtworkRequestHelper.PrepareArtworkRequestbySORepeat(ref objAWRequest, context);
                        if (!string.IsNullOrEmpty(msg))
                        {
                            deleteCS(objAWRequest.REQUEST_FROM_NO_ERROR, objAWRequest.LIST_AW_NODE_ID);
                            objAWRequest.RESULT_CREATE_WF_MESSAGE = msg;
                            objAWRequest.RESULT_CREATE_WF_STATUS = "E";
                            CNService.SaveLogAction("SORepeatTuning", "END", username, objAWRequest.CONTROL_NAME, msg);
                            return objAWRequest;
                        }

                        //IF Submit type IS_COMPLETE or IS_SEND_TO_PP
                        if (objAWRequest.IS_COMPLETE || objAWRequest.IS_SEND_TO_PP)
                        {
                            //Prepare Workflow and process
                            msg = ArtworkRequestHelper.PrepareArtworkItemProcessbySORepeat(ref objAWRequest, context);
                            if (!string.IsNullOrEmpty(msg))
                            {
                                deleteCS(objAWRequest.REQUEST_FROM_NO_ERROR, objAWRequest.LIST_AW_NODE_ID);
                                objAWRequest.RESULT_CREATE_WF_MESSAGE = msg;
                                objAWRequest.RESULT_CREATE_WF_STATUS = "E";
                                CNService.SaveLogAction("SORepeatTuning", "END", username, objAWRequest.CONTROL_NAME, msg);
                                return objAWRequest;
                            }
                        }

                    }
                }

                #region "COMMENT STEP SAVE TABLE"
                //step save
                //-------------------------------------------------------------------------------------------------------
                // save request form
                //ART_WF_ARTWORK_REQUEST
                //ART_WF_ARTWORK_REQUEST_RECIPIENT
                //ART_WF_ARTWORK_REQUEST_SALES_ORDER
                //ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                //ART_WF_ARTWORK_REQUEST_ITEM
                //ART_WF_ARTWORK_REQUEST_PRODUCT
                // copy product code from last wf
                //ART_WF_ARTWORK_REQUEST_CUSTOMER
                //ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT
                //ART_WF_ARTWORK_REQUEST_COUNTRY
                //-------------------------------------------------------------------------------------------------------
                // start wf (is_complete or is_send_to_pp)
                //ART_WF_ARTWORK_REQUEST_ITEM   start WF and Create node WF
                //ART_WF_ARTWORK_PROCESS
                //ART_WF_ARTWORK_PROCESS_PA -> all table
                //ART_WF_ARTWORK_PROCESS_PG
                //ART_WF_ARTWORK_PROCESS_SO_DETAIL

                #endregion

                // SAVE BY Transaction
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        var f_sendmail_request = false;
                        //var f_sendmail_pp = false;
                        context.Database.CommandTimeout = 300;
                    
                       //Save Artwork Request
                        msg = ArtworkRequestHelper.saveArtworkRequestbySORepeat(ref objAWRequest, context, token);
                        if (!string.IsNullOrEmpty(msg))
                        {
                            deleteCS(objAWRequest.REQUEST_FROM_NO_ERROR, objAWRequest.LIST_AW_NODE_ID);
                            objAWRequest.RESULT_CREATE_WF_STATUS = "E";
                            objAWRequest.RESULT_CREATE_WF_MESSAGE = msg;
                            CNService.SaveLogAction("SORepeatTuning", "END", username, objAWRequest.CONTROL_NAME, msg);
                            return objAWRequest;
                        }


                        //IF Submit type IS_COMPLETE or IS_SEND_TO_PP
                        if (objAWRequest.IS_COMPLETE || objAWRequest.IS_SEND_TO_PP)
                        {
                            //Save Workflow and Process
                            msg = ArtworkRequestHelper.saveWorkFlowProcessbySORepeat(ref objAWRequest, context, token);
                            if (!string.IsNullOrEmpty(msg))
                            {
                                deleteCS(objAWRequest.REQUEST_FROM_NO_ERROR, objAWRequest.LIST_AW_NODE_ID);
                                objAWRequest.RESULT_CREATE_WF_STATUS = "E";
                                objAWRequest.RESULT_CREATE_WF_MESSAGE = msg;
                                CNService.SaveLogAction("SORepeatTuning", "END", username, objAWRequest.CONTROL_NAME, msg);
                                return objAWRequest;
                            }                     
                            
                        }
                        else
                        {
                            f_sendmail_request = true;
                        }                 
                        dbContextTransaction.Commit();


                        //update material lock
                        try
                        {
                            if (objAWRequest.REQUEST_ITEMS != null && objAWRequest.REQUEST_ITEMS.Count() >0)
                            {
                                foreach (var item in objAWRequest.REQUEST_ITEMS)
                                {
                                    if (item.PROCESS_STEP_PA != null)
                                    {
                                        if (item.PROCESS_STEP_PA.PROCESS_PA != null)
                                        {
                                            if (item.PROCESS_STEP_PA.PROCESS_PA.ARTWORK_SUB_ID > 0 )
                                            {
                                                CNService.UpdateMaterialLock(item.PROCESS_STEP_PA.PROCESS_PA.ARTWORK_SUB_ID);
                                            }                                        
                                        }
                                    }
                                 
                                }
                            }
                                                
                        }
                        catch (Exception ex)
                        {
                            CNService.GetErrorMessage_SORepeat(ex, "CNService.UpdateMaterialLock");
                        }

                        //sendmail
                        if (f_sendmail_request)
                        {
                            //send mail request form
                            EmailService.sendEmailArtworkRequest(objAWRequest.ARTWORK_REQUEST_ID, "WF_SEND_TO", context);
                        }

                        if (objAWRequest.IS_SEND_TO_PP)
                        {
                            if (objAWRequest.REQUEST_ITEMS != null && objAWRequest.REQUEST_ITEMS.Count > 0)
                            {
                                foreach (var item in objAWRequest.REQUEST_ITEMS)
                                {
                                    if (item.PROCESS_STEP_PP != null)
                                    {
                                        //send mail process PP
                                        EmailService.sendEmailArtwork(item.PROCESS_STEP_PP.ARTWORK_REQUEST_ID, item.PROCESS_STEP_PP.ARTWORK_SUB_ID, "WF_SEND_TO", context);
                                    }
                                }
                            }
                        }


                    }
                }
                                   
                //objAWRequest.RESULT_CREATE_WF_WFNO = cnt.ToString() + ":" + objAWRequest.ARTWORK_REQUEST_NO;
                objAWRequest.RESULT_CREATE_WF_STATUS = "S";
                CNService.SaveLogAction("SORepeatTuning", "END", username, objAWRequest.CONTROL_NAME, "Creating Successfully");
            }
            catch (Exception ex)
            {
                deleteCS(objAWRequest.REQUEST_FROM_NO_ERROR, objAWRequest.LIST_AW_NODE_ID);
                objAWRequest.RESULT_CREATE_WF_STATUS  = "E";
                objAWRequest.RESULT_CREATE_WF_MESSAGE = CNService.GetErrorMessage_SORepeat(ex, "CreateRFBySORepeat_Tuning") + "{Log}";
            }
          
            return objAWRequest;
        }


        //---------------------------- end tuning performance sorepeat 2022 by aof-------------------------------------------------------------------------//


        //public static void SaveOrUpdateNoLog(SAP_M_PO_COMPLETE_SO_ITEM Item, ARTWORKEntities dc)
        //{
        //    if (GetChkByPO_COMPLETE_SO_ITEM_ID(Item.PO_COMPLETE_SO_ITEM_ID, dc) == null)
        //    {
        //        Item.CREATE_DATE = DateTime.Now;
        //        Item.UPDATE_DATE = DateTime.Now
        //              dc.SAP_M_PO_COMPLETE_SO_ITEM.Add(Item);
        //        dc.SaveChanges();
        //    }
        //    else
        //    {
        //        var temp = GetByPO_COMPLETE_SO_ITEM_ID(Item.PO_COMPLETE_SO_ITEM_ID, dc);
        //        Item.CREATE_DATE = temp.CREATE_DATE;
        //        Item.CREATE_BY = temp.CREATE_BY;
        //        Item.UPDATE_DATE = DateTime.Now;
        //        var local = dc.Set<SAP_M_PO_COMPLETE_SO_ITEM>().Local.FirstOrDefault(f => f.PO_COMPLETE_SO_ITEM_ID == Item.PO_COMPLETE_SO_ITEM_ID);
        //        if (local != null)
        //        {
        //            dc.Entry(local).State = EntityState.Detached;
        //        }
        //        dc.Entry(Item).State = EntityState.Modified;
        //        dc.SaveChanges();
        //    }
        //}
        //private static int? GetChkByPO_COMPLETE_SO_ITEM_ID(int? PO_COMPLETE_SO_ITEM_ID, ARTWORKEntities dc)
        //{
        //    if (PO_COMPLETE_SO_ITEM_ID == null || PO_COMPLETE_SO_ITEM_ID == 0)
        //        return null;
        //    else
        //    {
        //        var temp = (from p in dc.SAP_M_PO_COMPLETE_SO_ITEM where p.PO_COMPLETE_SO_ITEM_ID == PO_COMPLETE_SO_ITEM_ID select p.PO_COMPLETE_SO_ITEM_ID).FirstOrDefault();
        //        if (temp == 0) return null;
        //        else return 1;
        //    }
        //}

        public static ART_WF_ARTWORK_REQUEST_RESULT CreateRFBySORepeat(ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();
            var requestFormNoError = "";
            List<long> listAWNodeIdError = new List<long>();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        context.Database.CommandTimeout = 300;

                        //case send to pp and created request form success
                        if (param.data.ARTWORK_SUB_ID != 0 && param.data.IS_SEND_TO_PP)
                        {
                            //send to pp
                            var stepArtwork = ART_M_STEP_ARTWORK_SERVICE.GetAll(context).Where(s => s.STEP_ARTWORK_CODE.Equals("SEND_PP")).FirstOrDefault();
                            ART_WF_ARTWORK_PROCESS_PP_BY_PA_REQUEST paramPP = new ART_WF_ARTWORK_PROCESS_PP_BY_PA_REQUEST();
                            var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);

                            if (process != null)
                            {
                                paramPP.data = new ART_WF_ARTWORK_PROCESS_PP_BY_PA_2();
                                paramPP.data.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                                paramPP.data.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                                paramPP.data.REQUEST_SHADE_LIMIT = "";
                                paramPP.data.CREATE_BY = param.data.UPDATE_BY;
                                paramPP.data.UPDATE_BY = param.data.UPDATE_BY;

                                paramPP.data.PROCESS = new ART_WF_ARTWORK_PROCESS_2();
                                paramPP.data.PROCESS.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                                paramPP.data.PROCESS.ARTWORK_ITEM_ID = process.ARTWORK_ITEM_ID;
                                paramPP.data.PROCESS.PARENT_ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                                paramPP.data.PROCESS.CURRENT_ROLE_ID = stepArtwork.ROLE_ID_RESPONSE;
                                paramPP.data.PROCESS.CURRENT_STEP_ID = stepArtwork.STEP_ARTWORK_ID;
                                paramPP.data.PROCESS.REMARK = param.data.COMMENT;
                                paramPP.data.PROCESS.CREATE_BY = param.data.UPDATE_BY;
                                paramPP.data.PROCESS.UPDATE_BY = param.data.UPDATE_BY;

                                ART_WF_ARTWORK_PROCESS_PP_BY_PA_RESULT resultPP = PPByPAHelper.SavePPByPA(paramPP);
                                if (resultPP.status == "E")
                                {
                                    Results.status = resultPP.status;
                                    Results.msg = resultPP.msg;
                                    Results.param = param;
                                    return Results;
                                }
                            }
                        }
                        else
                        {
                            var msg = ArtworkRequestHelper.ValidateMatAndSaleOrder(param, context);
                            if (!string.IsNullOrEmpty(msg))
                            {
                                Results.status = "E";
                                Results.msg = msg;
                                Results.param = param;
                                return Results;
                            }

                            Results = ArtworkRequestHelper.ValidateMaterialBySalesOrder(param, context);
                            if (Results.status == "E")
                            {
                                Results.param = param;
                                return Results;
                            }

                            Results = ArtworkUploadHelper.aSaveUploadRequestForm(param, context);
                            if (Results.status == "E")
                            {
                                Results.param = param;
                                return Results;
                            }

                            Results = new ART_WF_ARTWORK_REQUEST_RESULT();
                            Results = ArtworkRequestHelper.GetMaterialBySalesOrder(param, context);
                            if (Results.status == "E")
                            {
                                Results.param = param;
                                return Results;
                            }

                            var artworkRequestId = 0;
                            Results = new ART_WF_ARTWORK_REQUEST_RESULT();
                            Results = ArtworkUploadHelper.aSubmitUploadRequestForm(param, context, ref artworkRequestId);
                            requestFormNoError = Results.requestFormNo;
                            if (Results.status == "E")
                            {
                                deleteCS(requestFormNoError, listAWNodeIdError);
                                Results.param = param;
                                return Results;
                            }

                            try
                            {
                                ArtworkRequestHelper.CopyRequestProductCode(param.data.ARTWORK_REQUEST_ID, context, param);
                            }
                            catch (Exception ex)
                            {
                                CNService.GetErrorMessage(ex);
                                deleteCS(requestFormNoError, listAWNodeIdError);
                                throw ex;
                            }

                            if (param.data.IS_COMPLETE || param.data.IS_SEND_TO_PP)
                            {
                                ART_WF_ARTWORK_REQUEST_REQUEST paramReq = new ART_WF_ARTWORK_REQUEST_REQUEST();
                                paramReq.data = new ART_WF_ARTWORK_REQUEST_2();
                                paramReq.data.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;

                                //get data request form
                                ART_WF_ARTWORK_REQUEST_RESULT requestForm = ArtworkUploadHelper.GetArtworkRequestForRepeat(paramReq, context);

                                if (requestForm.data.Count > 0)
                                {
                                    //bind data to obj for save
                                    paramReq.data = requestForm.data[0];

                                    //save request form
                                    var resultReq = new ART_WF_ARTWORK_REQUEST_RESULT();
                                    resultReq = ArtworkUploadHelper.SaveArtworkRequestForRepeat(paramReq, context);

                                    if (resultReq.status == "E")
                                    {
                                        Results.status = resultReq.status;
                                        Results.msg = resultReq.msg;
                                        Results.param = param;
                                        return Results;
                                    }

                                    //start work flow
                                    resultReq = ArtworkRequestHelper.SubmitArtworkRequestForRepeat(paramReq, false, context);
                                    listAWNodeIdError = resultReq.listAWNodeId;
                                    Results.WF_NO = resultReq.msg;
                                    if (resultReq.status == "E")
                                    {
                                        deleteCS(requestFormNoError, listAWNodeIdError);

                                        Results.status = resultReq.status;
                                        Results.msg = resultReq.msg;
                                        Results.param = param;
                                        return Results;
                                    }

                                    if (resultReq.data != null)
                                    {
                                        Results.ARTWORK_SUB_ID = resultReq.data[0].ARTWORK_SUB_ID;

                                        //accept task
                                        ART_WF_ARTWORK_PROCESS_REQUEST paramProcess = new ART_WF_ARTWORK_PROCESS_REQUEST();
                                        paramProcess.data = new ART_WF_ARTWORK_PROCESS_2();
                                        paramProcess.data.ARTWORK_REQUEST_ID = paramReq.data.ARTWORK_REQUEST_ID;
                                        paramProcess.data.ARTWORK_SUB_ID = resultReq.data[0].ARTWORK_SUB_ID;
                                        paramProcess.data.CURRENT_USER_ID = paramReq.data.UPDATE_BY;
                                        paramProcess.data.UPDATE_BY = paramReq.data.UPDATE_BY;

                                        var resultProcess = new ART_WF_ARTWORK_PROCESS_RESULT();
                                        resultProcess = ArtworkProcessHelper.AcceptTaskForRepeat(paramProcess, context);

                                        if (resultProcess.status == "E")
                                        {
                                            deleteCS(requestFormNoError, listAWNodeIdError);

                                            Results.status = resultProcess.status;
                                            Results.msg = resultProcess.msg;
                                            Results.param = param;
                                            return Results;
                                        }

                                        //complete work flow
                                        if (param.data.IS_COMPLETE)
                                        {
                                            resultProcess = PAFormHelper.CompletePAFormForRepeat(paramProcess, context);

                                            if (resultProcess.status == "E")
                                            {
                                                deleteCS(requestFormNoError, listAWNodeIdError);

                                                Results.status = resultProcess.status;
                                                Results.msg = resultProcess.msg;
                                                Results.param = param;
                                                return Results;
                                            }
                                        }

                                        //copy pa data
                                        ART_WF_ARTWORK_PROCESS_PA_REQUEST paramTmp = new ART_WF_ARTWORK_PROCESS_PA_REQUEST();
                                        ART_WF_ARTWORK_PROCESS_PA_RESULT resultTmp = new ART_WF_ARTWORK_PROCESS_PA_RESULT();
                                        ART_WF_ARTWORK_PROCESS_PA_2 dataTmp = new ART_WF_ARTWORK_PROCESS_PA_2();

                                        var ARTWORK_SUB_ID = resultReq.data[0].ARTWORK_SUB_ID;
                                        var ARTWORK_ITEM_ID = (from m in context.ART_WF_ARTWORK_PROCESS where m.ARTWORK_SUB_ID == ARTWORK_SUB_ID select m.ARTWORK_ITEM_ID).FirstOrDefault();
                                        var MATERIAL_NO = (from m in context.ART_WF_ARTWORK_PROCESS_PA where m.ARTWORK_SUB_ID == ARTWORK_SUB_ID select m.MATERIAL_NO).FirstOrDefault();
                                        var ARTWORK_NO = (from m in context.ART_WF_ARTWORK_REQUEST_ITEM where m.ARTWORK_ITEM_ID == ARTWORK_ITEM_ID select m.REQUEST_ITEM_NO).FirstOrDefault();

                                        dataTmp.ARTWORK_SUB_ID = ARTWORK_SUB_ID;
                                        dataTmp.MATERIAL_NO = MATERIAL_NO;
                                        dataTmp.ARTWORK_NO = ARTWORK_NO;

                                        paramTmp.data = dataTmp;
                                        resultTmp = PAFormHelper.RetriveMaterial_Repeat(paramTmp, context);
                                        if (resultTmp.status == "S")
                                        {
                                            var processPA = (from m in context.ART_WF_ARTWORK_PROCESS_PA where m.ARTWORK_SUB_ID == ARTWORK_SUB_ID select m).FirstOrDefault();
                                            processPA.IS_RETRIEVE_BY_AW_REPEAT = "X";
                                            ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);
                                        }
                                        else if (resultTmp.status == "E")
                                        {
                                            deleteCS(requestFormNoError, listAWNodeIdError);
                                            Results.status = "E";
                                            Results.msg = resultTmp.msg;
                                            Results.param = param;
                                            return Results;
                                        }
                                    }
                                }

                                dbContextTransaction.Commit();
                            }
                            else
                            {
                                dbContextTransaction.Commit();
                                EmailService.sendEmailArtworkRequest(artworkRequestId, "WF_SEND_TO", context);
                            }
                        }
                    }
                }

                Results.status = "S";
                Results.param = param;
            }
            catch (Exception ex)
            {
                deleteCS(requestFormNoError, listAWNodeIdError);
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        private static void deleteCS(string requestFormNoError, List<long> listAWNodeIdError)
        {
            var token = CWSService.getAuthToken();
            if (!string.IsNullOrEmpty(requestFormNoError))
            {
                try
                {
                    long folderID = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkRequestFormNodeID"]);
                    var tempNode = CWSService.getNodeByName(folderID, requestFormNoError, token);
                    if (tempNode != null) CWSService.deleteNode(tempNode.ID, token);
                }
                catch { }
            }

            foreach (var s in listAWNodeIdError)
            {
                try
                {
                    CWSService.deleteNode(s, token);
                }
                catch { }
            }
        }

        public static V_ART_WF_DASHBOARD_ARTWORK_RESULT GetCountIncomingArtworkForPG(V_ART_WF_DASHBOARD_ARTWORK_REQUEST param)
        {
            V_ART_WF_DASHBOARD_ARTWORK_RESULT Results = new V_ART_WF_DASHBOARD_ARTWORK_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var SEND_PG = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                        var getByCreateDateFrom = DateTime.Now;
                        var getByCreateDateTo = DateTime.Now;

                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            getByCreateDateFrom = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_FROM);
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            getByCreateDateTo = CNService.ConvertStringToDate(param.data.GET_BY_CREATE_DATE_TO);
                        var listPG = context.Database.SqlQuery<V_ART_WF_DASHBOARD_ARTWORK_2>("sp_ART_WF_DASHBOARD_ARTWORK_PG @STEP, @From, @To",
                            new SqlParameter("@STEP", SEND_PG.ToString()),
                            new SqlParameter("@From", getByCreateDateFrom), new SqlParameter("@To", getByCreateDateTo)).ToList();
                        var q = listPG.AsQueryable().Where(m => m.CURRENT_USER_ID == null && string.IsNullOrEmpty(m.IS_END)).ToList();
                        //var q = (from m in context.V_ART_WF_DASHBOARD_ARTWORK
                        //         where string.IsNullOrEmpty(m.IS_END)
                        //         && m.CURRENT_STEP_ID == SEND_PG
                        //         && m.CURRENT_USER_ID == null
                        //         select new V_ART_WF_DASHBOARD_ARTWORK_2()
                        //         {
                        //             IS_END = m.IS_END,
                        //             CURRENT_STEP_ID = m.CURRENT_STEP_ID,
                        //             CURRENT_USER_ID = m.CURRENT_USER_ID,
                        //             ARTWORK_SUB_ID = m.ARTWORK_SUB_ID,
                        //             CREATE_DATE_PROCESS = m.CREATE_DATE_PROCESS,
                        //             ARTWORK_REQUEST_NO = m.ARTWORK_REQUEST_NO
                        //         });

                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_FROM))
                            q = q.AsQueryable().Where(m => Convert.ToDateTime(m.CREATE_DATE_PROCESS).Date >= getByCreateDateFrom.Date).ToList();
                        if (!string.IsNullOrEmpty(param.data.GET_BY_CREATE_DATE_TO))
                            q = q.AsQueryable().Where(m => Convert.ToDateTime(m.CREATE_DATE_PROCESS).Date <= getByCreateDateTo.Date).ToList();
                        if (!string.IsNullOrEmpty(param.data.ARTWORK_REQUEST_NO))
                            q = q.AsQueryable().Where(m => m.ARTWORK_REQUEST_NO.Equals(param.data.ARTWORK_REQUEST_NO)).ToList();

                        var temp = q.ToList();

                        //Results.data = MapperServices.V_ART_WF_DASHBOARD_ARTWORK(temp);
                        Results.data = temp;
                    }
                }

                if (param != null)
                {
                    Results.draw = param.draw;
                }

                Results.recordsFiltered = Results.data.ToList().Count;
                Results.recordsTotal = Results.data.ToList().Count;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static V_ART_WF_DASHBOARD_RESULT CheckSOIsUpdate(V_ART_WF_DASHBOARD_REQUEST param)
        {
            V_ART_WF_DASHBOARD_RESULT Results = new V_ART_WF_DASHBOARD_RESULT();

            try
            {
                Results.data = new List<V_ART_WF_DASHBOARD_2>();
                if (param.data != null)
                {
                    param.data.LIST_ARTWORK_SUB_ID.ForEach(artworkSubId =>
                    {
                        using (var context = new ARTWORKEntities())
                        {
                            using (CNService.IsolationLevel(context))
                            {
                                context.Database.CommandTimeout = 300;

                                string isUpdate = SalesOrderHelper.CheckIsSalesOrderChange(artworkSubId, context);
                                if (isUpdate.Length > 0)
                                {
                                    Results.data.Add(new V_ART_WF_DASHBOARD_2 { MOCKUP_SUB_ID = artworkSubId });
                                }
                            }
                        }
                    });
                }
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
