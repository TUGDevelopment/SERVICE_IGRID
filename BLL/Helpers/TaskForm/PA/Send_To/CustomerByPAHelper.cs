using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using BLL.Helpers;

namespace BLL.Helpers
{
    public class CustomerByPAHelper
    {
        public static ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT GetCustomerByPA(ART_WF_ARTWORK_PROCESS_CUSTOMER_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT Results = new ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_CUSTOMER(ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetAll(context));
                        }
                        else
                        {
                            var ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            param.data.ARTWORK_SUB_ID = 0;

                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_CUSTOMER(ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItemContain(MapperServices.ART_WF_ARTWORK_PROCESS_CUSTOMER(param.data), context));

                            param.data.ARTWORK_SUB_ID = ARTWORK_SUB_ID;
                            Results.data = Results.data.Where(m => CNService.FindArtworkSubId(ARTWORK_SUB_ID, context).Contains(m.ARTWORK_SUB_ID)).ToList();
                        }

                        ART_WF_ARTWORK_PROCESS_CUSTOMER p = new ART_WF_ARTWORK_PROCESS_CUSTOMER();

                        Results.status = "S";
                        var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = param.data.STEP_ARTWORK_CODE }, context).FirstOrDefault().STEP_ARTWORK_ID;

                        if (Results.data.Count > 0)
                        {
                            //Completed WF
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                var dataCusbyPA = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault();

                                var SEND_TO_CUSTOMER_TYPE = dataCusbyPA.SEND_TO_CUSTOMER_TYPE;
                                if (SEND_TO_CUSTOMER_TYPE == param.data.SEND_TO_CUSTOMER_TYPE)
                                {
                                    ART_SYS_ACTION act = new ART_SYS_ACTION();
                                    act.ACTION_CODE = Results.data[i].ACTION_CODE;
                                    Results.data[i].ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItem(act, context).FirstOrDefault().ACTION_NAME;

                                    var processFormPA = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context);

                                    Results.data[i].COMMENT_BY_PA = processFormPA.REMARK;
                                    Results.data[i].CREATE_DATE_BY_PA = dataCusbyPA.CREATE_DATE;
                                    Results.data[i].SEND_TO_CUSTOMER_TYPE = dataCusbyPA.SEND_TO_CUSTOMER_TYPE;
                                    Results.data[i].COURIER_NUMBER = dataCusbyPA.COURIER_NAME;
                                    Results.data[i].REMARK_REASON_BY_OTHER = "-";

                                    if (Results.data[i].DECISION__CHANGE_DETAIL == "1")
                                        Results.data[i].DECISION_FORMLABEL_DISPLAY = "Confirm to change";
                                    else if (Results.data[i].DECISION__CHANGE_DETAIL == "0")
                                        Results.data[i].DECISION_FORMLABEL_DISPLAY = "Do not change";
                                    else
                                    {
                                        Results.data[i].DECISION_FORMLABEL_DISPLAY = "-";
                                        Results.data[i].COMMENT_CHANGE_DETAIL = "-";
                                    }
                                    if (Results.data[i].DECISION__NONCOMPLIANCE == "1")
                                        Results.data[i].DECISION_NONCOMPLIANCE_DISPLAY = "Confirm to change";
                                    else if (Results.data[i].DECISION__NONCOMPLIANCE == "0")
                                        Results.data[i].DECISION_NONCOMPLIANCE_DISPLAY = "Do not change";
                                    else
                                    {
                                        Results.data[i].DECISION_NONCOMPLIANCE_DISPLAY = "-";
                                        Results.data[i].COMMENT_NONCOMPLIANCE = "-";
                                    }
                                    if (Results.data[i].DECISION__ADJUST == "1")
                                        Results.data[i].DECISION_ADJUST_DISPLAY = "Confirm to change";
                                    else if (Results.data[i].DECISION__ADJUST == "0")
                                        Results.data[i].DECISION_ADJUST_DISPLAY = "Do not change";
                                    else
                                    {
                                        Results.data[i].DECISION_ADJUST_DISPLAY = "-";
                                        Results.data[i].COMMENT_ADJUST = "-";
                                    }

                                    if (Results.data[i].DECISION_ACTION == "0")
                                    {
                                        Results.data[i].DECISION_ACTION_DISPLAY = "Approve";
                                        Results.data[i].REASON_BY_OTHER = "-";
                                    }
                                    else if (Results.data[i].DECISION_ACTION == "1")
                                    {
                                        Results.data[i].DECISION_ACTION_DISPLAY = "Revise";
                                        if (Results.data[i].REASON_ID != null)
                                        {
                                            Results.data[i].REASON_BY_OTHER = CNService.getReason(Results.data[i].REASON_ID, context);
                                            if (Results.data[i].REASON_BY_OTHER == "อื่นๆ โปรดระบุ (Others)")
                                            {
                                                Results.data[i].REMARK_REASON_BY_OTHER = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = Results.data[i].ARTWORK_SUB_ID, WF_STEP = stepId }, context);
                                            }
                                        }
                                    }
                                    else if (Results.data[i].DECISION_ACTION == "2")
                                    {
                                        Results.data[i].DECISION_ACTION_DISPLAY = "Cancel";
                                        if (Results.data[i].REASON_ID != null)
                                        {
                                            Results.data[i].REASON_BY_OTHER = CNService.getReason(Results.data[i].REASON_ID, context);
                                            if (Results.data[i].REASON_BY_OTHER == "อื่นๆ โปรดระบุ (Others)")
                                            {
                                                Results.data[i].REMARK_REASON_BY_OTHER = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = Results.data[i].ARTWORK_SUB_ID, WF_STEP = stepId }, context);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Results.data[i].DECISION_ACTION_DISPLAY = "-";
                                        Results.data[i].REASON_BY_OTHER = "-";
                                    }

                                    if (Results.data[i].APPROVE_SHADE_LIMIT == "1")
                                        Results.data[i].APPROVE_SHADE_LIMIT_DISPLAY = "Request shade limit for approval before production";
                                    else if (Results.data[i].APPROVE_SHADE_LIMIT == "0")
                                        Results.data[i].APPROVE_SHADE_LIMIT_DISPLAY = "Request shade limit for reference only";
                                    else
                                        Results.data[i].APPROVE_SHADE_LIMIT_DISPLAY = "-";


                                    Results.data[i].CUSTOMER_DISPLAY_TXT = CNService.GetUserName(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).CURRENT_USER_ID, context);
                                    if (ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).CURRENT_CUSTOMER_ID != null)
                                        Results.data[i].CUSTOMER_DISPLAY_TXT += "<br/>" + XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).CURRENT_CUSTOMER_ID, context).CUSTOMER_NAME;

                                    if (dataCusbyPA.IS_FORMLABEL != null)
                                        Results.data[i].IS_FORMLABEL = dataCusbyPA.IS_FORMLABEL;
                                    if (dataCusbyPA.IS_CHANGEDETAIL != null)
                                        Results.data[i].IS_CHANGEDETAIL = dataCusbyPA.IS_CHANGEDETAIL;
                                    if (dataCusbyPA.IS_NONCOMPLIANCE != null)
                                        Results.data[i].IS_NONCOMPLIANCE = dataCusbyPA.IS_NONCOMPLIANCE;
                                    if (dataCusbyPA.IS_ADJUST != null)
                                        Results.data[i].IS_ADJUST = dataCusbyPA.IS_ADJUST;

                                    if (dataCusbyPA.COMMENT_ADJUST != null)
                                        Results.data[i].COMMENT_ADJUST_DISPLAY = dataCusbyPA.COMMENT_ADJUST;
                                    else
                                        Results.data[i].COMMENT_ADJUST_DISPLAY = "-";
                                    if (dataCusbyPA.COMMENT_NONCOMPLIANCE != null)
                                        Results.data[i].COMMENT_NONCOMPLIANCE_DISPLAY = dataCusbyPA.COMMENT_NONCOMPLIANCE;
                                    else
                                        Results.data[i].COMMENT_NONCOMPLIANCE_DISPLAY = "-";
                                    if (dataCusbyPA.COMMENT_FORM_LABEL != null)
                                        Results.data[i].COMMENT_FORMLABEL_DISPLAY = dataCusbyPA.COMMENT_FORM_LABEL;
                                    else
                                        Results.data[i].COMMENT_FORMLABEL_DISPLAY = "-";

                                    if (dataCusbyPA.NUTRITION_COMMENT != "<p><br></p>" && dataCusbyPA.NUTRITION_COMMENT != null)
                                        Results.data[i].NUTRITION_COMMENT_DISPLAY = dataCusbyPA.NUTRITION_COMMENT;
                                    else
                                        Results.data[i].NUTRITION_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.INGREDIENTS_COMMENT != "<p><br></p>" && dataCusbyPA.INGREDIENTS_COMMENT != null)
                                        Results.data[i].INGREDIENTS_COMMENT_DISPLAY = dataCusbyPA.INGREDIENTS_COMMENT;
                                    else
                                        Results.data[i].INGREDIENTS_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.ANALYSIS_COMMENT != "<p><br></p>" && dataCusbyPA.ANALYSIS_COMMENT != null)
                                        Results.data[i].ANALYSIS_COMMENT_DISPLAY = dataCusbyPA.ANALYSIS_COMMENT;
                                    else
                                        Results.data[i].ANALYSIS_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.HEALTH_CLAIM_COMMENT != "<p><br></p>" && dataCusbyPA.HEALTH_CLAIM_COMMENT != null)
                                        Results.data[i].HEALTH_CLAIM_COMMENT_DISPLAY = dataCusbyPA.HEALTH_CLAIM_COMMENT;
                                    else
                                        Results.data[i].HEALTH_CLAIM_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.NUTRIENT_CLAIM_COMMENT != "<p><br></p>" && dataCusbyPA.NUTRIENT_CLAIM_COMMENT != null)
                                        Results.data[i].NUTRIENT_CLAIM_COMMENT_DISPLAY = dataCusbyPA.NUTRIENT_CLAIM_COMMENT;
                                    else
                                        Results.data[i].NUTRIENT_CLAIM_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.SPECIES_COMMENT != "<p><br></p>" && dataCusbyPA.SPECIES_COMMENT != null)
                                        Results.data[i].SPECIES_COMMENT_DISPLAY = dataCusbyPA.SPECIES_COMMENT;
                                    else
                                        Results.data[i].SPECIES_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.CATCHING_AREA_COMMENT != "<p><br></p>" && dataCusbyPA.CATCHING_AREA_COMMENT != null)
                                        Results.data[i].CATCHING_AREA_COMMENT_DISPLAY = dataCusbyPA.CATCHING_AREA_COMMENT;
                                    else
                                        Results.data[i].CATCHING_AREA_COMMENT_DISPLAY = "-";

                                    if (dataCusbyPA.CHECK_DETAIL_COMMENT != "<p><br></p>" && dataCusbyPA.CHECK_DETAIL_COMMENT != null)
                                        Results.data[i].CHECK_DETAIL_COMMENT_DISPLAY = dataCusbyPA.CHECK_DETAIL_COMMENT;
                                    else
                                        Results.data[i].CHECK_DETAIL_COMMENT_DISPLAY = "-";

                                    if (dataCusbyPA.QC_COMMENT != "" && dataCusbyPA.QC_COMMENT != null)
                                        Results.data[i].QC_COMMENT = dataCusbyPA.QC_COMMENT;

                                    if (dataCusbyPA.COMMENT != "" && dataCusbyPA.COMMENT != null)
                                        Results.data[i].COMMENT = dataCusbyPA.COMMENT;

                                    var SODetail = MapperServices.ART_WF_ARTWORK_PROCESS_SO_DETAIL(ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_SO_DETAIL() { ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID, ARTWORK_SUB_ID = param.data.MAIN_ARTWORK_SUB_ID }, context)).FirstOrDefault();
                                    if (SODetail != null)
                                    {
                                        var SOHeader = MapperServices.SAP_M_PO_COMPLETE_SO_HEADER(SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.GetByItem(new SAP_M_PO_COMPLETE_SO_HEADER() { SALES_ORDER_NO = SODetail.SALES_ORDER_NO }, context).FirstOrDefault());
                                        if (SOHeader != null)
                                        {
                                            Results.data[i].SOLD_TO_PO = SOHeader.SOLD_TO_PO;
                                            Results.data[i].SHIP_TO_PO = SOHeader.SHIP_TO_PO;
                                        }
                                    }

                                    var isRepeatsubid = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).ARTWORK_REQUEST_ID;
                                    var isRepeat = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(isRepeatsubid, context);
                                    if (isRepeat != null)
                                    {
                                        if (isRepeat.TYPE_OF_ARTWORK == "REPEAT")
                                            Results.data[i].IS_REPEAT = "X";
                                    }
                                }
                                else
                                {
                                    Results.data[i].SEND_TO_CUSTOMER_TYPE = "delete";
                                }
                            }

                            Results.data = Results.data.Where(m => m.SEND_TO_CUSTOMER_TYPE != "delete").ToList();
                            Results.data = Results.data.OrderByDescending(o => o.UPDATE_DATE).ToList();
                        }

                        var list = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context), CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var results = list.Where(q => !Results.data.Any(q2 => q2.ARTWORK_SUB_ID == q.ARTWORK_SUB_ID));
                        //Not completed WF
                        var requestCustomer = ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER() { ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID }, context);

                        foreach (var result in results)
                        {
                            var isTo = false;
                            var isCC = false;
                            if (ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(result.ARTWORK_SUB_ID, context).CURRENT_CUSTOMER_ID > 0)
                            {
                                if (requestCustomer.Where(m => m.CUSTOMER_USER_ID == result.CURRENT_USER_ID && m.MAIL_TO == "X" && m.ARTWORK_REQUEST_ID == result.ARTWORK_REQUEST_ID).Count() > 0 && result.CURRENT_USER_ID == param.data.CURRENT_USER_ID)
                                    isTo = true;
                                else if (requestCustomer.Where(m => m.CUSTOMER_USER_ID == result.CURRENT_USER_ID && m.MAIL_CC == "X" && m.ARTWORK_REQUEST_ID == result.ARTWORK_REQUEST_ID).Count() > 0)
                                    isCC = true;
                                else if (ART_WF_LOG_REASSIGN_SERVICE.GetByItem(new ART_WF_LOG_REASSIGN { WF_TYPE = "A", WF_SUB_ID = result.ARTWORK_SUB_ID, TO_USER_ID = result.CURRENT_USER_ID }, context).Count() > 0 && result.CURRENT_USER_ID == param.data.CURRENT_USER_ID)
                                    isTo = true;
                            }
                            else
                            {
                                isTo = true;
                            }

                            if (isTo)
                            {
                                var checkcus = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                if (checkcus != null)
                                {
                                    var dataCusbyPA = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    ART_WF_ARTWORK_PROCESS_CUSTOMER_2 item = new ART_WF_ARTWORK_PROCESS_CUSTOMER_2();
                                    item.CREATE_DATE_BY_PA = result.CREATE_DATE;
                                    item.COMMENT_BY_PA = result.REMARK;
                                    item.SEND_TO_CUSTOMER_TYPE = checkcus.SEND_TO_CUSTOMER_TYPE;
                                    item.COURIER_NUMBER = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault().COURIER_NAME;

                                    item.ARTWORK_SUB_ID = result.ARTWORK_SUB_ID;
                                    item.ARTWORK_REQUEST_ID = result.ARTWORK_REQUEST_ID;

                                    item.CUSTOMER_DISPLAY_TXT = CNService.GetUserName(result.CURRENT_USER_ID, context);
                                    if (result.CURRENT_CUSTOMER_ID != null)
                                        item.CUSTOMER_DISPLAY_TXT += "<br/>" + XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(result.CURRENT_CUSTOMER_ID, context).CUSTOMER_NAME;

                                    if (dataCusbyPA.COMMENT_ADJUST != null)
                                        item.COMMENT_ADJUST_DISPLAY = dataCusbyPA.COMMENT_ADJUST;
                                    else
                                        item.COMMENT_ADJUST_DISPLAY = "-";
                                    if (dataCusbyPA.COMMENT_NONCOMPLIANCE != null)
                                        item.COMMENT_NONCOMPLIANCE_DISPLAY = dataCusbyPA.COMMENT_NONCOMPLIANCE;
                                    else
                                        item.COMMENT_NONCOMPLIANCE_DISPLAY = "-";
                                    if (dataCusbyPA.COMMENT_FORM_LABEL != null)
                                        item.COMMENT_FORMLABEL_DISPLAY = dataCusbyPA.COMMENT_FORM_LABEL;
                                    else
                                        item.COMMENT_FORMLABEL_DISPLAY = "-";

                                    if (dataCusbyPA.IS_FORMLABEL != null)
                                        item.IS_FORMLABEL = dataCusbyPA.IS_FORMLABEL;
                                    if (dataCusbyPA.IS_CHANGEDETAIL != null)
                                        item.IS_CHANGEDETAIL = dataCusbyPA.IS_CHANGEDETAIL;
                                    if (dataCusbyPA.IS_NONCOMPLIANCE != null)
                                        item.IS_NONCOMPLIANCE = dataCusbyPA.IS_NONCOMPLIANCE;
                                    if (dataCusbyPA.IS_ADJUST != null)
                                        item.IS_ADJUST = dataCusbyPA.IS_ADJUST;

                                    if (dataCusbyPA.NUTRITION_COMMENT != "<p><br></p>" && dataCusbyPA.NUTRITION_COMMENT != null)
                                        item.NUTRITION_COMMENT_DISPLAY = dataCusbyPA.NUTRITION_COMMENT;
                                    else
                                        item.NUTRITION_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.INGREDIENTS_COMMENT != "<p><br></p>" && dataCusbyPA.INGREDIENTS_COMMENT != null)
                                        item.INGREDIENTS_COMMENT_DISPLAY = dataCusbyPA.INGREDIENTS_COMMENT;
                                    else
                                        item.INGREDIENTS_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.ANALYSIS_COMMENT != "<p><br></p>" && dataCusbyPA.ANALYSIS_COMMENT != null)
                                        item.ANALYSIS_COMMENT_DISPLAY = dataCusbyPA.ANALYSIS_COMMENT;
                                    else
                                        item.ANALYSIS_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.HEALTH_CLAIM_COMMENT != "<p><br></p>" && dataCusbyPA.HEALTH_CLAIM_COMMENT != null)
                                        item.HEALTH_CLAIM_COMMENT_DISPLAY = dataCusbyPA.HEALTH_CLAIM_COMMENT;
                                    else
                                        item.HEALTH_CLAIM_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.NUTRIENT_CLAIM_COMMENT != "<p><br></p>" && dataCusbyPA.NUTRIENT_CLAIM_COMMENT != null)
                                        item.NUTRIENT_CLAIM_COMMENT_DISPLAY = dataCusbyPA.NUTRIENT_CLAIM_COMMENT;
                                    else
                                        item.NUTRIENT_CLAIM_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.SPECIES_COMMENT != "<p><br></p>" && dataCusbyPA.SPECIES_COMMENT != null)
                                        item.SPECIES_COMMENT_DISPLAY = dataCusbyPA.SPECIES_COMMENT;
                                    else
                                        item.SPECIES_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.CATCHING_AREA_COMMENT != "<p><br></p>" && dataCusbyPA.CATCHING_AREA_COMMENT != null)
                                        item.CATCHING_AREA_COMMENT_DISPLAY = dataCusbyPA.CATCHING_AREA_COMMENT;
                                    else
                                        item.CATCHING_AREA_COMMENT_DISPLAY = "-";

                                    if (dataCusbyPA.CHECK_DETAIL_COMMENT != "<p><br></p>" && dataCusbyPA.CHECK_DETAIL_COMMENT != null)
                                        item.CHECK_DETAIL_COMMENT_DISPLAY = dataCusbyPA.CHECK_DETAIL_COMMENT;
                                    else
                                        item.CHECK_DETAIL_COMMENT_DISPLAY = "-";

                                    if (dataCusbyPA.QC_COMMENT != "" && dataCusbyPA.QC_COMMENT != null)
                                        item.QC_COMMENT = dataCusbyPA.QC_COMMENT;

                                    if (dataCusbyPA.COMMENT != "" && dataCusbyPA.COMMENT != null)
                                        item.COMMENT = dataCusbyPA.COMMENT;

                                    var SODetail = MapperServices.ART_WF_ARTWORK_PROCESS_SO_DETAIL(ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_SO_DETAIL() { ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID, ARTWORK_SUB_ID = param.data.MAIN_ARTWORK_SUB_ID }, context)).FirstOrDefault();
                                    if (SODetail != null)
                                    {
                                        var SOHeader = MapperServices.SAP_M_PO_COMPLETE_SO_HEADER(SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.GetByItem(new SAP_M_PO_COMPLETE_SO_HEADER() { SALES_ORDER_NO = SODetail.SALES_ORDER_NO }, context).FirstOrDefault());
                                        if (SOHeader != null)
                                        {
                                            item.SOLD_TO_PO = SOHeader.SOLD_TO_PO;
                                            item.SHIP_TO_PO = SOHeader.SHIP_TO_PO;
                                        }
                                    }

                                    var isRepeatsubid = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(result.ARTWORK_SUB_ID, context).ARTWORK_REQUEST_ID;
                                    var isRepeat = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(isRepeatsubid, context);
                                    if (isRepeat != null)
                                    {
                                        if (isRepeat.TYPE_OF_ARTWORK == "REPEAT")
                                            item.IS_REPEAT = "X";
                                    }

                                    Results.data.Add(item);
                                }
                            }
                            else if (isCC)
                            {
                                var checkcus = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                if (checkcus != null)
                                {
                                    var dataCusbyPA = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    ART_WF_ARTWORK_PROCESS_CUSTOMER_2 item = new ART_WF_ARTWORK_PROCESS_CUSTOMER_2();
                                    item.CREATE_DATE_BY_PA = result.CREATE_DATE;
                                    item.COMMENT_BY_PA = result.REMARK;
                                    item.SEND_TO_CUSTOMER_TYPE = checkcus.SEND_TO_CUSTOMER_TYPE;
                                    item.COURIER_NUMBER = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault().COURIER_NAME;

                                    item.ARTWORK_SUB_ID = result.ARTWORK_SUB_ID;
                                    item.ARTWORK_REQUEST_ID = result.ARTWORK_REQUEST_ID;

                                    item.CUSTOMER_DISPLAY_TXT = CNService.GetUserName(result.CURRENT_USER_ID, context);
                                    if (result.CURRENT_CUSTOMER_ID != null)
                                        item.CUSTOMER_DISPLAY_TXT += "<br/>" + XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(result.CURRENT_CUSTOMER_ID, context).CUSTOMER_NAME;

                                    if (dataCusbyPA.COMMENT_ADJUST != null)
                                        item.COMMENT_ADJUST_DISPLAY = dataCusbyPA.COMMENT_ADJUST;
                                    else
                                        item.COMMENT_ADJUST_DISPLAY = "-";
                                    if (dataCusbyPA.COMMENT_NONCOMPLIANCE != null)
                                        item.COMMENT_NONCOMPLIANCE_DISPLAY = dataCusbyPA.COMMENT_NONCOMPLIANCE;
                                    else
                                        item.COMMENT_NONCOMPLIANCE_DISPLAY = "-";
                                    if (dataCusbyPA.COMMENT_FORM_LABEL != null)
                                        item.COMMENT_FORMLABEL_DISPLAY = dataCusbyPA.COMMENT_FORM_LABEL;
                                    else
                                        item.COMMENT_FORMLABEL_DISPLAY = "-";

                                    if (dataCusbyPA.IS_FORMLABEL != null)
                                        item.IS_FORMLABEL = dataCusbyPA.IS_FORMLABEL;
                                    if (dataCusbyPA.IS_CHANGEDETAIL != null)
                                        item.IS_CHANGEDETAIL = dataCusbyPA.IS_CHANGEDETAIL;
                                    if (dataCusbyPA.IS_NONCOMPLIANCE != null)
                                        item.IS_NONCOMPLIANCE = dataCusbyPA.IS_NONCOMPLIANCE;
                                    if (dataCusbyPA.IS_ADJUST != null)
                                        item.IS_ADJUST = dataCusbyPA.IS_ADJUST;

                                    if (dataCusbyPA.NUTRITION_COMMENT != "<p><br></p>" && dataCusbyPA.NUTRITION_COMMENT != null)
                                        item.NUTRITION_COMMENT_DISPLAY = dataCusbyPA.NUTRITION_COMMENT;
                                    else
                                        item.NUTRITION_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.INGREDIENTS_COMMENT != "<p><br></p>" && dataCusbyPA.INGREDIENTS_COMMENT != null)
                                        item.INGREDIENTS_COMMENT_DISPLAY = dataCusbyPA.INGREDIENTS_COMMENT;
                                    else
                                        item.INGREDIENTS_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.ANALYSIS_COMMENT != "<p><br></p>" && dataCusbyPA.ANALYSIS_COMMENT != null)
                                        item.ANALYSIS_COMMENT_DISPLAY = dataCusbyPA.ANALYSIS_COMMENT;
                                    else
                                        item.ANALYSIS_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.HEALTH_CLAIM_COMMENT != "<p><br></p>" && dataCusbyPA.HEALTH_CLAIM_COMMENT != null)
                                        item.HEALTH_CLAIM_COMMENT_DISPLAY = dataCusbyPA.HEALTH_CLAIM_COMMENT;
                                    else
                                        item.HEALTH_CLAIM_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.NUTRIENT_CLAIM_COMMENT != "<p><br></p>" && dataCusbyPA.NUTRIENT_CLAIM_COMMENT != null)
                                        item.NUTRIENT_CLAIM_COMMENT_DISPLAY = dataCusbyPA.NUTRIENT_CLAIM_COMMENT;
                                    else
                                        item.NUTRIENT_CLAIM_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.SPECIES_COMMENT != "<p><br></p>" && dataCusbyPA.SPECIES_COMMENT != null)
                                        item.SPECIES_COMMENT_DISPLAY = dataCusbyPA.SPECIES_COMMENT;
                                    else
                                        item.SPECIES_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA.CATCHING_AREA_COMMENT != "<p><br></p>" && dataCusbyPA.CATCHING_AREA_COMMENT != null)
                                        item.CATCHING_AREA_COMMENT_DISPLAY = dataCusbyPA.CATCHING_AREA_COMMENT;
                                    else
                                        item.CATCHING_AREA_COMMENT_DISPLAY = "-";

                                    if (dataCusbyPA.CHECK_DETAIL_COMMENT != "<p><br></p>" && dataCusbyPA.CHECK_DETAIL_COMMENT != null)
                                        item.CHECK_DETAIL_COMMENT_DISPLAY = dataCusbyPA.CHECK_DETAIL_COMMENT;
                                    else
                                        item.CHECK_DETAIL_COMMENT_DISPLAY = "-";

                                    if (dataCusbyPA.QC_COMMENT != "" && dataCusbyPA.QC_COMMENT != null)
                                        item.QC_COMMENT = dataCusbyPA.QC_COMMENT;

                                    if (dataCusbyPA.COMMENT != "" && dataCusbyPA.COMMENT != null)
                                        item.COMMENT = dataCusbyPA.COMMENT;

                                    var SODetail = MapperServices.ART_WF_ARTWORK_PROCESS_SO_DETAIL(ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_SO_DETAIL() { ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID, ARTWORK_SUB_ID = param.data.MAIN_ARTWORK_SUB_ID }, context)).FirstOrDefault();
                                    if (SODetail != null)
                                    {
                                        var SOHeader = MapperServices.SAP_M_PO_COMPLETE_SO_HEADER(SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.GetByItem(new SAP_M_PO_COMPLETE_SO_HEADER() { SALES_ORDER_NO = SODetail.SALES_ORDER_NO }, context).FirstOrDefault());
                                        if (SOHeader != null)
                                        {
                                            item.SOLD_TO_PO = SOHeader.SOLD_TO_PO;
                                            item.SHIP_TO_PO = SOHeader.SHIP_TO_PO;
                                        }
                                    }

                                    var isRepeatsubid = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(result.ARTWORK_SUB_ID, context).ARTWORK_REQUEST_ID;
                                    var isRepeat = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(isRepeatsubid, context);
                                    if (isRepeat != null)
                                    {
                                        if (isRepeat.TYPE_OF_ARTWORK == "REPEAT")
                                            item.IS_REPEAT = "X";
                                    }

                                    Results.data.Add(item);
                                }
                            }
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

        public static ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT GetCusReq(ART_WF_ARTWORK_PROCESS_CUSTOMER_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT Results = new ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_CUSTOMER(ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetAll(context));
                        }
                        else
                        {
                            var ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            param.data.ARTWORK_SUB_ID = 0;

                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_CUSTOMER(ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItemContain(MapperServices.ART_WF_ARTWORK_PROCESS_CUSTOMER(param.data), context));

                            param.data.ARTWORK_SUB_ID = ARTWORK_SUB_ID;
                            Results.data = Results.data.Where(m => CNService.FindArtworkSubId(ARTWORK_SUB_ID, context).Contains(m.ARTWORK_SUB_ID)).ToList();
                        }

                        ART_WF_ARTWORK_PROCESS_CUSTOMER p = new ART_WF_ARTWORK_PROCESS_CUSTOMER();

                        Results.status = "S";

                        if (Results.data.Count > 0)
                        {
                            //Completed WF
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                var dataCusbyPA = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                if (dataCusbyPA != null)
                                {
                                    var SEND_TO_CUSTOMER_TYPE = dataCusbyPA.SEND_TO_CUSTOMER_TYPE;
                                    if (SEND_TO_CUSTOMER_TYPE == param.data.SEND_TO_CUSTOMER_TYPE)
                                    {

                                        var processFormPA = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context);

                                        Results.data[i].COMMENT_BY_PA = processFormPA.REMARK;
                                        Results.data[i].CREATE_DATE_BY_PA = processFormPA.CREATE_DATE;
                                        Results.data[i].SEND_TO_CUSTOMER_TYPE = dataCusbyPA.SEND_TO_CUSTOMER_TYPE;
                                        Results.data[i].COURIER_NUMBER = dataCusbyPA.COURIER_NAME;


                                        Results.data[i].CUSTOMER_DISPLAY_TXT = CNService.GetUserName(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).CURRENT_USER_ID, context);
                                        if (ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).CURRENT_CUSTOMER_ID != null)
                                            Results.data[i].CUSTOMER_DISPLAY_TXT += "<br/>" + XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).CURRENT_CUSTOMER_ID, context).CUSTOMER_NAME;

                                        var CusID = 0;
                                        var CusReview_ = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = processFormPA.ARTWORK_ITEM_ID, ARTWORK_REQUEST_ID = processFormPA.ARTWORK_REQUEST_ID, CURRENT_STEP_ID = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REVIEW" }, context).FirstOrDefault().STEP_ARTWORK_ID }, context).Where(e => e.CREATE_DATE < Results.data[i].CREATE_DATE).OrderByDescending(o => o.UPDATE_DATE).ToList();
                                        for (int x = 0; x < CusReview_.Count; x++)
                                        {
                                            var check = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = CusReview_[x].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                            if (check != null)
                                            {
                                                CusID = check.ARTWORK_SUB_ID;
                                                break;
                                            }
                                        }

                                        var resultCusPA = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = CusID }, context).FirstOrDefault();

                                        if (resultCusPA != null)
                                        {
                                            if (resultCusPA.IS_FORMLABEL != null)
                                                Results.data[i].IS_FORMLABEL = resultCusPA.IS_FORMLABEL;
                                            if (resultCusPA.IS_CHANGEDETAIL != null)
                                                Results.data[i].IS_CHANGEDETAIL = resultCusPA.IS_CHANGEDETAIL;
                                            if (resultCusPA.IS_NONCOMPLIANCE != null)
                                                Results.data[i].IS_NONCOMPLIANCE = resultCusPA.IS_CHANGEDETAIL;
                                            if (resultCusPA.IS_ADJUST != null)
                                                Results.data[i].IS_ADJUST = resultCusPA.IS_ADJUST;

                                            if (resultCusPA.COMMENT_ADJUST != null)
                                                Results.data[i].COMMENT_ADJUST_DISPLAY = resultCusPA.COMMENT_ADJUST;
                                            else
                                                Results.data[i].COMMENT_ADJUST_DISPLAY = "-";
                                            if (resultCusPA.COMMENT_NONCOMPLIANCE != null)
                                                Results.data[i].COMMENT_NONCOMPLIANCE_DISPLAY = resultCusPA.COMMENT_NONCOMPLIANCE;
                                            else
                                                Results.data[i].COMMENT_NONCOMPLIANCE_DISPLAY = "-";
                                            if (resultCusPA.COMMENT_FORM_LABEL != null)
                                                Results.data[i].COMMENT_FORMLABEL_DISPLAY = resultCusPA.COMMENT_FORM_LABEL;
                                            else
                                                Results.data[i].COMMENT_FORMLABEL_DISPLAY = "-";

                                            if (resultCusPA.NUTRITION_COMMENT != "<p><br></p>" && resultCusPA.NUTRITION_COMMENT != null)
                                                Results.data[i].NUTRITION_COMMENT_DISPLAY = resultCusPA.NUTRITION_COMMENT;
                                            else
                                                Results.data[i].NUTRITION_COMMENT_DISPLAY = "-";
                                            if (resultCusPA.INGREDIENTS_COMMENT != "<p><br></p>" && resultCusPA.INGREDIENTS_COMMENT != null)
                                                Results.data[i].INGREDIENTS_COMMENT_DISPLAY = resultCusPA.INGREDIENTS_COMMENT;
                                            else
                                                Results.data[i].INGREDIENTS_COMMENT_DISPLAY = "-";
                                            if (resultCusPA.ANALYSIS_COMMENT != "<p><br></p>" && resultCusPA.ANALYSIS_COMMENT != null)
                                                Results.data[i].ANALYSIS_COMMENT_DISPLAY = resultCusPA.ANALYSIS_COMMENT;
                                            else
                                                Results.data[i].ANALYSIS_COMMENT_DISPLAY = "-";
                                            if (resultCusPA.HEALTH_CLAIM_COMMENT != "<p><br></p>" && resultCusPA.HEALTH_CLAIM_COMMENT != null)
                                                Results.data[i].HEALTH_CLAIM_COMMENT_DISPLAY = resultCusPA.HEALTH_CLAIM_COMMENT;
                                            else
                                                Results.data[i].HEALTH_CLAIM_COMMENT_DISPLAY = "-";
                                            if (resultCusPA.NUTRIENT_CLAIM_COMMENT != "<p><br></p>" && resultCusPA.NUTRIENT_CLAIM_COMMENT != null)
                                                Results.data[i].NUTRIENT_CLAIM_COMMENT_DISPLAY = resultCusPA.NUTRIENT_CLAIM_COMMENT;
                                            else
                                                Results.data[i].NUTRIENT_CLAIM_COMMENT_DISPLAY = "-";
                                            if (resultCusPA.SPECIES_COMMENT != "<p><br></p>" && resultCusPA.SPECIES_COMMENT != null)
                                                Results.data[i].SPECIES_COMMENT_DISPLAY = resultCusPA.SPECIES_COMMENT;
                                            else
                                                Results.data[i].SPECIES_COMMENT_DISPLAY = "-";
                                            if (resultCusPA.CATCHING_AREA_COMMENT != "<p><br></p>" && resultCusPA.CATCHING_AREA_COMMENT != null)
                                                Results.data[i].CATCHING_AREA_COMMENT_DISPLAY = resultCusPA.CATCHING_AREA_COMMENT;
                                            else
                                                Results.data[i].CATCHING_AREA_COMMENT_DISPLAY = "-";
                                            if (resultCusPA.CHECK_DETAIL_COMMENT != "<p><br></p>" && resultCusPA.CHECK_DETAIL_COMMENT != null)
                                                Results.data[i].CHECK_DETAIL_COMMENT_DISPLAY = resultCusPA.CHECK_DETAIL_COMMENT;
                                            else
                                                Results.data[i].CHECK_DETAIL_COMMENT_DISPLAY = "-";
                                            if (resultCusPA.QC_COMMENT != "" && resultCusPA.QC_COMMENT != null)
                                                Results.data[i].QC_COMMENT = resultCusPA.QC_COMMENT;
                                            else
                                                Results.data[i].QC_COMMENT = "-";
                                        }

                                        //Comment Customer
                                        var processFormCustomer = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = CusID }, context).FirstOrDefault();
                                        if (processFormCustomer != null)
                                        {
                                            if (processFormCustomer.COMMENT_ADJUST != null)
                                                Results.data[i].COMMENT_ADJUST_DISPLAY = processFormCustomer.COMMENT_ADJUST;
                                            else
                                                Results.data[i].COMMENT_ADJUST_DISPLAY = "-";
                                            if (processFormCustomer.COMMENT_NONCOMPLIANCE != null)
                                                Results.data[i].COMMENT_NONCOMPLIANCE_DISPLAY = processFormCustomer.COMMENT_NONCOMPLIANCE;
                                            else
                                                Results.data[i].COMMENT_NONCOMPLIANCE_DISPLAY = "-";
                                            if (processFormCustomer.COMMENT_CHANGE_DETAIL != null)
                                                Results.data[i].COMMENT_FORMLABEL_DISPLAY = processFormCustomer.COMMENT_CHANGE_DETAIL;
                                            else
                                                Results.data[i].COMMENT_FORMLABEL_DISPLAY = "-";

                                            if (processFormCustomer.DECISION__CHANGE_DETAIL == "1")
                                                Results.data[i].DECISION_FORMLABEL_DISPLAY = "Confirm to change";
                                            else if (processFormCustomer.DECISION__CHANGE_DETAIL == "0")
                                                Results.data[i].DECISION_FORMLABEL_DISPLAY = "Do not change";
                                            else
                                                Results.data[i].DECISION_FORMLABEL_DISPLAY = "-";
                                            if (processFormCustomer.DECISION__NONCOMPLIANCE == "1")
                                                Results.data[i].DECISION_NONCOMPLIANCE_DISPLAY = "Confirm to change";
                                            else if (processFormCustomer.DECISION__NONCOMPLIANCE == "0")
                                                Results.data[i].DECISION_NONCOMPLIANCE_DISPLAY = "Do not change";
                                            else
                                                Results.data[i].DECISION_NONCOMPLIANCE_DISPLAY = "-";
                                            if (processFormCustomer.DECISION__ADJUST == "1")
                                                Results.data[i].DECISION_ADJUST_DISPLAY = "Confirm to change";
                                            else if (processFormCustomer.DECISION__ADJUST == "0")
                                                Results.data[i].DECISION_ADJUST_DISPLAY = "Do not change";
                                            else
                                                Results.data[i].DECISION_ADJUST_DISPLAY = "-";
                                        }

                                        //Comment Customer
                                        var processFormCustomerReqRef = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                        if (processFormCustomerReqRef != null)
                                        {
                                            Results.data[i].CREATE_DATE = processFormCustomerReqRef.CREATE_DATE;
                                            Results.data[i].COMMENT = processFormCustomerReqRef.COMMENT;

                                            ART_SYS_ACTION act = new ART_SYS_ACTION();
                                            act.ACTION_CODE = processFormCustomerReqRef.ACTION_CODE;
                                            Results.data[i].ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItem(act, context).FirstOrDefault().ACTION_NAME;
                                        }

                                        var SODetail = MapperServices.ART_WF_ARTWORK_PROCESS_SO_DETAIL(ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_SO_DETAIL() { ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID, ARTWORK_SUB_ID = param.data.MAIN_ARTWORK_SUB_ID }, context)).FirstOrDefault();
                                        if (SODetail != null)
                                        {
                                            var SOHeader = MapperServices.SAP_M_PO_COMPLETE_SO_HEADER(SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.GetByItem(new SAP_M_PO_COMPLETE_SO_HEADER() { SALES_ORDER_NO = SODetail.SALES_ORDER_NO }, context).FirstOrDefault());
                                            if (SOHeader != null)
                                            {
                                                Results.data[i].SOLD_TO_PO = SOHeader.SOLD_TO_PO;
                                                Results.data[i].SHIP_TO_PO = SOHeader.SHIP_TO_PO;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Results.data[i].SEND_TO_CUSTOMER_TYPE = "delete";
                                    }
                                }

                            }

                            Results.data = Results.data.Where(m => m.SEND_TO_CUSTOMER_TYPE != "delete").ToList();
                            Results.data = Results.data.OrderByDescending(o => o.UPDATE_DATE).ToList();
                        }

                        var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = param.data.STEP_ARTWORK_CODE }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var list = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context), CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var results = list.Where(q => !Results.data.Any(q2 => q2.ARTWORK_SUB_ID == q.ARTWORK_SUB_ID));
                        if (results != null)
                        {
                            var requestCustomer = ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER() { ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID }, context);

                            foreach (var result in results)
                            {
                                var isTo = false;
                                var isCC = false;
                                if (ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(result.ARTWORK_SUB_ID, context).CURRENT_CUSTOMER_ID > 0)
                                {
                                    if (requestCustomer.Where(m => m.CUSTOMER_USER_ID == result.CURRENT_USER_ID && m.MAIL_TO == "X" && m.ARTWORK_REQUEST_ID == result.ARTWORK_REQUEST_ID).Count() > 0 && result.CURRENT_USER_ID == param.data.CURRENT_USER_ID)
                                        isTo = true;
                                    else if (requestCustomer.Where(m => m.CUSTOMER_USER_ID == result.CURRENT_USER_ID && m.MAIL_CC == "X" && m.ARTWORK_REQUEST_ID == result.ARTWORK_REQUEST_ID).Count() > 0)
                                        isCC = true;
                                    else if (ART_WF_LOG_REASSIGN_SERVICE.GetByItem(new ART_WF_LOG_REASSIGN { WF_TYPE = "A", WF_SUB_ID = result.ARTWORK_SUB_ID, TO_USER_ID = result.CURRENT_USER_ID }, context).Count() > 0 && result.CURRENT_USER_ID == param.data.CURRENT_USER_ID)
                                        isTo = true;
                                }
                                else
                                {
                                    isTo = true;
                                }

                                if (isTo)
                                {
                                    var CusID = 0;
                                    var CusReview_ = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = result.ARTWORK_ITEM_ID, ARTWORK_REQUEST_ID = result.ARTWORK_REQUEST_ID, CURRENT_STEP_ID = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REVIEW" }, context).FirstOrDefault().STEP_ARTWORK_ID }, context).OrderByDescending(o => o.UPDATE_DATE).ToList();
                                    for (int x = 0; x < CusReview_.Count; x++)
                                    {
                                        var check = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = CusReview_[x].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                        if (check != null)
                                        {
                                            CusID = check.ARTWORK_SUB_ID;
                                            break;
                                        }
                                    }
                                    var resultCusPA = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = CusID }, context).FirstOrDefault();

                                    if (resultCusPA != null)
                                    {
                                        ART_WF_ARTWORK_PROCESS_CUSTOMER_2 item = new ART_WF_ARTWORK_PROCESS_CUSTOMER_2();

                                        item.CREATE_DATE_BY_PA = result.CREATE_DATE;
                                        item.COMMENT_BY_PA = result.REMARK;
                                        item.SEND_TO_CUSTOMER_TYPE = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault().SEND_TO_CUSTOMER_TYPE;

                                        item.ARTWORK_SUB_ID = result.ARTWORK_SUB_ID;
                                        item.ARTWORK_REQUEST_ID = result.ARTWORK_REQUEST_ID;

                                        item.CUSTOMER_DISPLAY_TXT = CNService.GetUserName(result.CURRENT_USER_ID, context);
                                        if (result.CURRENT_CUSTOMER_ID != null)
                                            item.CUSTOMER_DISPLAY_TXT += "<br/>" + XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(result.CURRENT_CUSTOMER_ID, context).CUSTOMER_NAME;

                                        //Comment PA
                                        if (resultCusPA.COMMENT_ADJUST != null)
                                            item.COMMENT_ADJUST = resultCusPA.COMMENT_ADJUST;
                                        else
                                            item.COMMENT_ADJUST = "-";
                                        if (resultCusPA.COMMENT_NONCOMPLIANCE != null)
                                            item.COMMENT_NONCOMPLIANCE = resultCusPA.COMMENT_NONCOMPLIANCE;
                                        else
                                            item.COMMENT_NONCOMPLIANCE = "-";
                                        if (resultCusPA.COMMENT_FORM_LABEL != null)
                                            item.COMMENT_CHANGE_DETAIL = resultCusPA.COMMENT_FORM_LABEL;
                                        else
                                            item.COMMENT_CHANGE_DETAIL = "-";


                                        if (resultCusPA.IS_FORMLABEL != null)
                                            item.IS_FORMLABEL = resultCusPA.IS_FORMLABEL;
                                        else
                                            item.IS_FORMLABEL = "-";
                                        if (resultCusPA.IS_CHANGEDETAIL != null)
                                            item.IS_CHANGEDETAIL = resultCusPA.IS_CHANGEDETAIL;
                                        else
                                            item.IS_CHANGEDETAIL = "-";
                                        if (resultCusPA.IS_NONCOMPLIANCE != null)
                                            item.IS_NONCOMPLIANCE = resultCusPA.IS_CHANGEDETAIL;
                                        else
                                            item.IS_NONCOMPLIANCE = "-";
                                        if (resultCusPA.IS_ADJUST != null)
                                            item.IS_ADJUST = resultCusPA.IS_ADJUST;
                                        else
                                            item.IS_ADJUST = "-";


                                        if (resultCusPA.NUTRITION_COMMENT != "<p><br></p>" && resultCusPA.NUTRITION_COMMENT != null)
                                            item.NUTRITION_COMMENT_DISPLAY = resultCusPA.NUTRITION_COMMENT;
                                        else
                                            item.NUTRITION_COMMENT_DISPLAY = "-";
                                        if (resultCusPA.INGREDIENTS_COMMENT != "<p><br></p>" && resultCusPA.INGREDIENTS_COMMENT != null)
                                            item.INGREDIENTS_COMMENT_DISPLAY = resultCusPA.INGREDIENTS_COMMENT;
                                        else
                                            item.INGREDIENTS_COMMENT_DISPLAY = "-";
                                        if (resultCusPA.ANALYSIS_COMMENT != "<p><br></p>" && resultCusPA.ANALYSIS_COMMENT != null)
                                            item.ANALYSIS_COMMENT_DISPLAY = resultCusPA.ANALYSIS_COMMENT;
                                        else
                                            item.ANALYSIS_COMMENT_DISPLAY = "-";
                                        if (resultCusPA.HEALTH_CLAIM_COMMENT != "<p><br></p>" && resultCusPA.HEALTH_CLAIM_COMMENT != null)
                                            item.HEALTH_CLAIM_COMMENT_DISPLAY = resultCusPA.HEALTH_CLAIM_COMMENT;
                                        else
                                            item.HEALTH_CLAIM_COMMENT_DISPLAY = "-";
                                        if (resultCusPA.NUTRIENT_CLAIM_COMMENT != "<p><br></p>" && resultCusPA.NUTRIENT_CLAIM_COMMENT != null)
                                            item.NUTRIENT_CLAIM_COMMENT_DISPLAY = resultCusPA.NUTRIENT_CLAIM_COMMENT;
                                        else
                                            item.NUTRIENT_CLAIM_COMMENT_DISPLAY = "-";
                                        if (resultCusPA.SPECIES_COMMENT != "<p><br></p>" && resultCusPA.SPECIES_COMMENT != null)
                                            item.SPECIES_COMMENT_DISPLAY = resultCusPA.SPECIES_COMMENT;
                                        else
                                            item.SPECIES_COMMENT_DISPLAY = "-";
                                        if (resultCusPA.CATCHING_AREA_COMMENT != "<p><br></p>" && resultCusPA.CATCHING_AREA_COMMENT != null)
                                            item.CATCHING_AREA_COMMENT_DISPLAY = resultCusPA.CATCHING_AREA_COMMENT;
                                        else
                                            item.CATCHING_AREA_COMMENT_DISPLAY = "-";

                                        if (resultCusPA.CHECK_DETAIL_COMMENT != "<p><br></p>" && resultCusPA.CHECK_DETAIL_COMMENT != null)
                                            item.CHECK_DETAIL_COMMENT_DISPLAY = resultCusPA.CHECK_DETAIL_COMMENT;
                                        else
                                            item.CHECK_DETAIL_COMMENT_DISPLAY = "-";

                                        if (resultCusPA.QC_COMMENT != "" && resultCusPA.QC_COMMENT != null)
                                            item.QC_COMMENT = resultCusPA.QC_COMMENT;
                                        else
                                            item.QC_COMMENT = "-";

                                        //Comment Customer
                                        var processFormCustomer = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = CusID }, context).FirstOrDefault();

                                        if (processFormCustomer.COMMENT_ADJUST != null)
                                            item.COMMENT_ADJUST_DISPLAY = processFormCustomer.COMMENT_ADJUST;
                                        else
                                            item.COMMENT_ADJUST_DISPLAY = "-";
                                        if (processFormCustomer.COMMENT_NONCOMPLIANCE != null)
                                            item.COMMENT_NONCOMPLIANCE_DISPLAY = processFormCustomer.COMMENT_NONCOMPLIANCE;
                                        else
                                            item.COMMENT_NONCOMPLIANCE_DISPLAY = "-";
                                        if (processFormCustomer.COMMENT_CHANGE_DETAIL != null)
                                            item.COMMENT_FORMLABEL_DISPLAY = processFormCustomer.COMMENT_CHANGE_DETAIL;
                                        else
                                            item.COMMENT_FORMLABEL_DISPLAY = "-";

                                        if (processFormCustomer.DECISION__CHANGE_DETAIL == "1")
                                            item.DECISION_FORMLABEL_DISPLAY = "Confirm to change";
                                        else if (processFormCustomer.DECISION__CHANGE_DETAIL == "0")
                                            item.DECISION_FORMLABEL_DISPLAY = "Do not change";
                                        else
                                            item.DECISION_FORMLABEL_DISPLAY = "-";
                                        if (processFormCustomer.DECISION__NONCOMPLIANCE == "1")
                                            item.DECISION_NONCOMPLIANCE_DISPLAY = "Confirm to change";
                                        else if (processFormCustomer.DECISION__NONCOMPLIANCE == "0")
                                            item.DECISION_NONCOMPLIANCE_DISPLAY = "Do not change";
                                        else
                                            item.DECISION_NONCOMPLIANCE_DISPLAY = "-";
                                        if (processFormCustomer.DECISION__ADJUST == "1")
                                            item.DECISION_ADJUST_DISPLAY = "Confirm to change";
                                        else if (processFormCustomer.DECISION__ADJUST == "0")
                                            item.DECISION_ADJUST_DISPLAY = "Do not change";
                                        else
                                            item.DECISION_ADJUST_DISPLAY = "-";

                                        //Comment Customer
                                        var processFormCustomerReqRef = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                        if (processFormCustomerReqRef != null)
                                        {
                                            item.CREATE_DATE = processFormCustomerReqRef.CREATE_DATE;
                                            item.COMMENT = processFormCustomerReqRef.COMMENT;

                                            ART_SYS_ACTION act = new ART_SYS_ACTION();
                                            act.ACTION_CODE = processFormCustomerReqRef.ACTION_CODE;
                                            item.ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItem(act, context).FirstOrDefault().ACTION_NAME;
                                        }

                                        var SODetail = MapperServices.ART_WF_ARTWORK_PROCESS_SO_DETAIL(ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_SO_DETAIL() { ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID, ARTWORK_SUB_ID = param.data.MAIN_ARTWORK_SUB_ID }, context)).FirstOrDefault();
                                        if (SODetail != null)
                                        {
                                            var SOHeader = MapperServices.SAP_M_PO_COMPLETE_SO_HEADER(SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.GetByItem(new SAP_M_PO_COMPLETE_SO_HEADER() { SALES_ORDER_NO = SODetail.SALES_ORDER_NO }, context).FirstOrDefault());
                                            if (SOHeader != null)
                                            {
                                                item.SOLD_TO_PO = SOHeader.SOLD_TO_PO;
                                                item.SHIP_TO_PO = SOHeader.SHIP_TO_PO;
                                            }
                                        }

                                        Results.data.Add(item);
                                    }
                                }
                                if (isCC)
                                {
                                    var CusID = 0;
                                    var CusReview_ = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = result.ARTWORK_ITEM_ID, ARTWORK_REQUEST_ID = result.ARTWORK_REQUEST_ID, CURRENT_STEP_ID = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REVIEW" }, context).FirstOrDefault().STEP_ARTWORK_ID }, context).OrderByDescending(o => o.UPDATE_DATE).ToList();
                                    for (int x = 0; x < CusReview_.Count; x++)
                                    {
                                        var check = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = CusReview_[x].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                        if (check != null)
                                        {
                                            CusID = check.ARTWORK_SUB_ID;
                                            break;
                                        }

                                    }
                                    var resultCusPA = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = CusID }, context).FirstOrDefault();

                                    if (resultCusPA != null)
                                    {
                                        ART_WF_ARTWORK_PROCESS_CUSTOMER_2 item = new ART_WF_ARTWORK_PROCESS_CUSTOMER_2();

                                        item.CREATE_DATE_BY_PA = result.CREATE_DATE;
                                        item.COMMENT_BY_PA = result.REMARK;
                                        item.SEND_TO_CUSTOMER_TYPE = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault().SEND_TO_CUSTOMER_TYPE;

                                        item.ARTWORK_SUB_ID = result.ARTWORK_SUB_ID;
                                        item.ARTWORK_REQUEST_ID = result.ARTWORK_REQUEST_ID;

                                        item.CUSTOMER_DISPLAY_TXT = CNService.GetUserName(result.CURRENT_USER_ID, context);
                                        if (result.CURRENT_CUSTOMER_ID != null)
                                            item.CUSTOMER_DISPLAY_TXT += "<br/>" + XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(result.CURRENT_CUSTOMER_ID, context).CUSTOMER_NAME;

                                        //Comment PA
                                        if (resultCusPA.COMMENT_ADJUST != null)
                                            item.COMMENT_ADJUST = resultCusPA.COMMENT_ADJUST;
                                        else
                                            item.COMMENT_ADJUST = "-";
                                        if (resultCusPA.COMMENT_NONCOMPLIANCE != null)
                                            item.COMMENT_NONCOMPLIANCE = resultCusPA.COMMENT_NONCOMPLIANCE;
                                        else
                                            item.COMMENT_NONCOMPLIANCE = "-";
                                        if (resultCusPA.COMMENT_FORM_LABEL != null)
                                            item.COMMENT_CHANGE_DETAIL = resultCusPA.COMMENT_FORM_LABEL;
                                        else
                                            item.COMMENT_CHANGE_DETAIL = "-";

                                        if (resultCusPA.IS_FORMLABEL != null)
                                            item.IS_FORMLABEL = resultCusPA.IS_FORMLABEL;
                                        else
                                            item.IS_FORMLABEL = "-";
                                        if (resultCusPA.IS_CHANGEDETAIL != null)
                                            item.IS_CHANGEDETAIL = resultCusPA.IS_CHANGEDETAIL;
                                        else
                                            item.IS_CHANGEDETAIL = "-";
                                        if (resultCusPA.IS_NONCOMPLIANCE != null)
                                            item.IS_NONCOMPLIANCE = resultCusPA.IS_CHANGEDETAIL;
                                        else
                                            item.IS_NONCOMPLIANCE = "-";
                                        if (resultCusPA.IS_ADJUST != null)
                                            item.IS_ADJUST = resultCusPA.IS_ADJUST;
                                        else
                                            item.IS_ADJUST = "-";

                                        if (resultCusPA.NUTRITION_COMMENT != "<p><br></p>" && resultCusPA.NUTRITION_COMMENT != null)
                                            item.NUTRITION_COMMENT_DISPLAY = resultCusPA.NUTRITION_COMMENT;
                                        else
                                            item.NUTRITION_COMMENT_DISPLAY = "-";
                                        if (resultCusPA.INGREDIENTS_COMMENT != "<p><br></p>" && resultCusPA.INGREDIENTS_COMMENT != null)
                                            item.INGREDIENTS_COMMENT_DISPLAY = resultCusPA.INGREDIENTS_COMMENT;
                                        else
                                            item.INGREDIENTS_COMMENT_DISPLAY = "-";
                                        if (resultCusPA.ANALYSIS_COMMENT != "<p><br></p>" && resultCusPA.ANALYSIS_COMMENT != null)
                                            item.ANALYSIS_COMMENT_DISPLAY = resultCusPA.ANALYSIS_COMMENT;
                                        else
                                            item.ANALYSIS_COMMENT_DISPLAY = "-";
                                        if (resultCusPA.HEALTH_CLAIM_COMMENT != "<p><br></p>" && resultCusPA.HEALTH_CLAIM_COMMENT != null)
                                            item.HEALTH_CLAIM_COMMENT_DISPLAY = resultCusPA.HEALTH_CLAIM_COMMENT;
                                        else
                                            item.HEALTH_CLAIM_COMMENT_DISPLAY = "-";
                                        if (resultCusPA.NUTRIENT_CLAIM_COMMENT != "<p><br></p>" && resultCusPA.NUTRIENT_CLAIM_COMMENT != null)
                                            item.NUTRIENT_CLAIM_COMMENT_DISPLAY = resultCusPA.NUTRIENT_CLAIM_COMMENT;
                                        else
                                            item.NUTRIENT_CLAIM_COMMENT_DISPLAY = "-";
                                        if (resultCusPA.SPECIES_COMMENT != "<p><br></p>" && resultCusPA.SPECIES_COMMENT != null)
                                            item.SPECIES_COMMENT_DISPLAY = resultCusPA.SPECIES_COMMENT;
                                        else
                                            item.SPECIES_COMMENT_DISPLAY = "-";
                                        if (resultCusPA.CATCHING_AREA_COMMENT != "<p><br></p>" && resultCusPA.CATCHING_AREA_COMMENT != null)
                                            item.CATCHING_AREA_COMMENT_DISPLAY = resultCusPA.CATCHING_AREA_COMMENT;
                                        else
                                            item.CATCHING_AREA_COMMENT_DISPLAY = "-";

                                        if (resultCusPA.CHECK_DETAIL_COMMENT != "<p><br></p>" && resultCusPA.CHECK_DETAIL_COMMENT != null)
                                            item.CHECK_DETAIL_COMMENT_DISPLAY = resultCusPA.CHECK_DETAIL_COMMENT;
                                        else
                                            item.CHECK_DETAIL_COMMENT_DISPLAY = "-";

                                        if (resultCusPA.QC_COMMENT != "" && resultCusPA.QC_COMMENT != null)
                                            item.QC_COMMENT = resultCusPA.QC_COMMENT;
                                        else
                                            item.QC_COMMENT = "-";

                                        //Comment Customer
                                        var processFormCustomer = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = CusID }, context).FirstOrDefault();

                                        if (processFormCustomer.COMMENT_ADJUST != null)
                                            item.COMMENT_ADJUST_DISPLAY = processFormCustomer.COMMENT_ADJUST;
                                        else
                                            item.COMMENT_ADJUST_DISPLAY = "-";
                                        if (processFormCustomer.COMMENT_NONCOMPLIANCE != null)
                                            item.COMMENT_NONCOMPLIANCE_DISPLAY = processFormCustomer.COMMENT_NONCOMPLIANCE;
                                        else
                                            item.COMMENT_NONCOMPLIANCE_DISPLAY = "-";
                                        if (processFormCustomer.COMMENT_CHANGE_DETAIL != null)
                                            item.COMMENT_FORMLABEL_DISPLAY = processFormCustomer.COMMENT_CHANGE_DETAIL;
                                        else
                                            item.COMMENT_FORMLABEL_DISPLAY = "-";

                                        if (processFormCustomer.DECISION__CHANGE_DETAIL == "1")
                                            item.DECISION_FORMLABEL_DISPLAY = "Confirm to change";
                                        else if (processFormCustomer.DECISION__CHANGE_DETAIL == "0")
                                            item.DECISION_FORMLABEL_DISPLAY = "Do not change";
                                        else
                                            item.DECISION_FORMLABEL_DISPLAY = "-";
                                        if (processFormCustomer.DECISION__NONCOMPLIANCE == "1")
                                            item.DECISION_NONCOMPLIANCE_DISPLAY = "Confirm to change";
                                        else if (processFormCustomer.DECISION__NONCOMPLIANCE == "0")
                                            item.DECISION_NONCOMPLIANCE_DISPLAY = "Do not change";
                                        else
                                            item.DECISION_NONCOMPLIANCE_DISPLAY = "-";
                                        if (processFormCustomer.DECISION__ADJUST == "1")
                                            item.DECISION_ADJUST_DISPLAY = "Confirm to change";
                                        else if (processFormCustomer.DECISION__ADJUST == "0")
                                            item.DECISION_ADJUST_DISPLAY = "Do not change";
                                        else
                                            item.DECISION_ADJUST_DISPLAY = "-";

                                        //Comment Customer
                                        var processFormCustomerReqRef = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                        if (processFormCustomerReqRef != null)
                                        {
                                            item.CREATE_DATE = processFormCustomerReqRef.CREATE_DATE;
                                            item.COMMENT = processFormCustomerReqRef.COMMENT;

                                            ART_SYS_ACTION act = new ART_SYS_ACTION();
                                            act.ACTION_CODE = processFormCustomerReqRef.ACTION_CODE;
                                            item.ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItem(act, context).FirstOrDefault().ACTION_NAME;
                                        }

                                        var SODetail = MapperServices.ART_WF_ARTWORK_PROCESS_SO_DETAIL(ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_SO_DETAIL() { ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID, ARTWORK_SUB_ID = param.data.MAIN_ARTWORK_SUB_ID }, context)).FirstOrDefault();
                                        if (SODetail != null)
                                        {
                                            var SOHeader = MapperServices.SAP_M_PO_COMPLETE_SO_HEADER(SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.GetByItem(new SAP_M_PO_COMPLETE_SO_HEADER() { SALES_ORDER_NO = SODetail.SALES_ORDER_NO }, context).FirstOrDefault());
                                            if (SOHeader != null)
                                            {
                                                item.SOLD_TO_PO = SOHeader.SOLD_TO_PO;
                                                item.SHIP_TO_PO = SOHeader.SHIP_TO_PO;
                                            }
                                        }

                                        Results.data.Add(item);
                                    }
                                }
                            }
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

        public static ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_RESULT SaveCustomerByPA(ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var requestForm = context.ART_WF_ARTWORK_PROCESS.Where(r => r.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).FirstOrDefault();

                        var customerList = context.ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER.Where(c => c.ARTWORK_REQUEST_ID == requestForm.ARTWORK_REQUEST_ID).ToList();

                        string msg = ArtworkProcessHelper.checkDupWF(param.data.PROCESS, context);
                        if (msg != "")
                        {
                            Results.status = "E";
                            Results.msg = msg;
                            return Results;
                        }

                        if (param.data.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_PRINT")
                        {
                            string validateSOChange = "";
                            validateSOChange = SalesOrderHelper.CheckIsSalesOrderChange(param.data.ARTWORK_SUB_ID, context);

                            if (validateSOChange == "X")
                            {
                                Results.status = "E";
                                Results.msg = MessageHelper.GetMessage("MSG_013", context);
                                return Results;
                            }
                        }

                        ART_WF_ARTWORK_PROCESS_RESULT processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
                        processResults.data = new List<ART_WF_ARTWORK_PROCESS_2>();

                        if (param.data.PROCESS != null)
                        {
                            if (customerList != null && customerList.Count > 0)
                            {
                                foreach (ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER iCustomer in customerList)
                                {
                                    ART_WF_ARTWORK_PROCESS_2 process = new ART_WF_ARTWORK_PROCESS_2();

                                    process = new ART_WF_ARTWORK_PROCESS_2();
                                    process = param.data.PROCESS;
                                    process.CURRENT_USER_ID = iCustomer.CUSTOMER_USER_ID;

                                    var customerUser = context.ART_M_USER_CUSTOMER.Where(c => c.USER_ID == iCustomer.CUSTOMER_USER_ID).FirstOrDefault();
                                    if (customerUser != null)
                                    {
                                        process.CURRENT_CUSTOMER_ID = customerUser.CUSTOMER_ID;
                                    }

                                    //_processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                                    var temp = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                                    foreach (var itemTemp in temp.data)
                                    {
                                        processResults.data.Add(itemTemp);
                                    }
                                }
                            }
                            else
                            {
                                //_processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                            }
                        }

                        if (param.data.ENDTASKFORM && param.data.SEND_TO_CUSTOMER_TYPE != "REQ_CUS_REF")
                        {
                            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC MKData = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC();

                            MKData.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;
                            MKData.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            MKData.ACTION_CODE = param.data.ACTION_CODE;
                            MKData.APPROVE = param.data.APPROVE;
                            MKData.COMMENT = param.data.COMMENT;
                            MKData.CREATE_BY = param.data.UPDATE_BY;
                            MKData.UPDATE_BY = param.data.UPDATE_BY;

                            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.SaveOrUpdate(MKData, context);
                        }

                        foreach (var item2 in processResults.data)
                        {
                            ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA CustomerData = new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA();
                            CustomerData = MapperServices.ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA(param.data);
                            CustomerData.ARTWORK_SUB_ID = item2.ARTWORK_SUB_ID;
                            ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.SaveOrUpdate(CustomerData, context);

                            //-----tikcet# 463880 by aof.       
                            msg = SaveUnmarkStatusReadyCreatePO(CustomerData, context);
                            if (msg != "")
                            {
                                Results.status = "E";
                                Results.msg = msg;
                                return Results;
                            }
                            //-----tikcet# 463880 by aof.       
                        }

                        if (param.data.ENDTASKFORM)
                        {
                            if (param.data.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REF")
                            {
                                foreach (var process in processResults.data)
                                    ArtworkProcessHelper.EndTaskForm(process.ARTWORK_SUB_ID, process.UPDATE_BY, context);

                                ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);

                                var tempProcess = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);
                                // start by aof 20220317
                                //ArtworkProcessHelper.moveFileArtworkToMatWorkspace(tempProcess.ARTWORK_ITEM_ID, context, ref error_msg);
                                var error_msg = "";                            
                                var foundPrintMaster = ArtworkProcessHelper.moveFileArtworkToMatWorkspace(tempProcess.ARTWORK_ITEM_ID, context, ref error_msg);
                                if (!foundPrintMaster)
                                {
                                    Results.status = "E";
                                    Results.msg = "Cannot complete workflow"; //, System not found print master file in this workflow.";
                                    if (error_msg != "")  // by aof 20220317
                                    {
                                        Results.msg = Results.msg + ", " + error_msg;
                                    }
                                    return Results;
                                }
                                // end by aof 20220317

                            }
                            else
                            {
                                ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                            }
                        }

                        dbContextTransaction.Commit();

                        if (param.data.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REF")
                        {
                            var first = true;
                            foreach (var process in processResults.data)
                            {
                                if (first)
                                {
                                    first = false;
                                    EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_COMPLETED", context);
                                }
                            }
                        }
                        else
                        {
                            foreach (var process in processResults.data)
                                EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO_CUSTOMER", context);
                        }

                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_001", context);
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

        // start -------------------- tikcet# 463880 by aof.
        public static string  SaveUnmarkStatusReadyCreatePO(ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA customerData,ARTWORKEntities context)
        {
            string msg = "";
            if (customerData.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_PRINT")
            {
                try
                {
                    var itemID = CNService.FindArtworkItemId(customerData.ARTWORK_SUB_ID, context);
                    var stepPA = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_PA").FirstOrDefault();
                    var process_pa = (from p in context.ART_WF_ARTWORK_PROCESS
                                      where p.ARTWORK_ITEM_ID == itemID && p.CURRENT_STEP_ID == stepPA.STEP_ARTWORK_ID
                                      select p).ToList().FirstOrDefault();


                    if (process_pa != null)
                    {
                        var pa = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                  where p.ARTWORK_SUB_ID == process_pa.ARTWORK_SUB_ID
                                  select p).ToList().FirstOrDefault();
                        if (pa != null)
                        {
                            pa.READY_CREATE_PO = null;
                            ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(pa, context);
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg = CNService.GetErrorMessage(ex);
                }

            }

            return msg;
        }
        // end -------------------- tikcet# 463880 by aof.



        public static ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT PostCustomerSendToPA(ART_WF_ARTWORK_PROCESS_CUSTOMER_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT Results = new ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var cusData = MapperServices.ART_WF_ARTWORK_PROCESS_CUSTOMER(param.data);

                        var check = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                        if (check.Count > 0)
                            cusData.ARTWORK_PROCESS_CUSTOMER_ID = check.FirstOrDefault().ARTWORK_PROCESS_CUSTOMER_ID;

                        ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.SaveOrUpdate(cusData, context);

                        if (param.data.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_PRINT")
                        {
                            // update shade limit to PA
                            var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(cusData.ARTWORK_SUB_ID, context);
                            if (process != null)
                            {
                                var pa = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PA() { ARTWORK_SUB_ID = process.PARENT_ARTWORK_SUB_ID.Value }, context).FirstOrDefault();
                                if (pa != null)
                                {
                                    pa.SHADE_LIMIT = cusData.APPROVE_SHADE_LIMIT;
                                    pa.UPDATE_BY = cusData.UPDATE_BY;
                                    ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(pa, context);
                                }
                            }
                        }

                        var processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
                        processResults.data = new List<ART_WF_ARTWORK_PROCESS_2>();
                        if (param.data.ENDTASKFORM)
                        {
                            ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);

                            EndTaskCustomerOtherUser(param, context);

                            if (param.data.DECISION__CHANGE_DETAIL == "0" || param.data.DECISION__NONCOMPLIANCE == "0" || (param.data.DECISION__ADJUST == "0" && (param.data.DECISION__CHANGE_DETAIL != null || param.data.DECISION__NONCOMPLIANCE != null)) || (param.data.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REF" && param.data.ACTION_CODE == "SUBMIT") || (param.data.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REQ_REF" && param.data.ACTION_CODE == "SUBMIT"))
                            {
                                processResults = AutomaticCreateProcessQC(param, context);
                            }
                        }

                        dbContextTransaction.Commit();

                        foreach (var process in processResults.data)
                            EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO", context);  //462588  by aof, change WF_SEND_TO_CUSTOMER to WF_SEND_TO
                        // EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO_CUSTOMER", context);  //462588 by aof, commented

                        if (param.data.ACTION_CODE == "SEND_BACK")
                            EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_SEND_BACK", context, param.data.COMMENT);
                        else if (param.data.ACTION_CODE == "SAVE") { }
                        //EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_OTHER_SAVE", context);
                        else
                            EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_OTHER_SUBMIT", context);

                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_001", context);
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




        public static ART_WF_ARTWORK_PROCESS_RESULT AutomaticCreateProcessQC(ART_WF_ARTWORK_PROCESS_CUSTOMER_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_PROCESS_RESULT processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
            processResults.data = new List<ART_WF_ARTWORK_PROCESS_2>();
            //ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT Results = new ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT();
            try
            {
                //if (param == null || param.data == null)
                //{
                //    return Results;
                //}
                //else
                //{
                string stepName = "SEND_QC_VERIFY";
                var processCust = context.ART_WF_ARTWORK_PROCESS.Where(c => c.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).FirstOrDefault();
                var step = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == stepName).FirstOrDefault();

                ART_WF_ARTWORK_PROCESS_QC_2 qc2 = new ART_WF_ARTWORK_PROCESS_QC_2();
                List<ART_WF_ARTWORK_PROCESS_QC_2> listQC2 = new List<ART_WF_ARTWORK_PROCESS_QC_2>();

                List<int> listSubID = CNService.FindArtworkSubId(param.data.ARTWORK_SUB_ID, context);

                var QCStepID = context.ART_M_STEP_ARTWORK.Where(c => c.STEP_ARTWORK_CODE == "SEND_QC").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
                var QCProcesses = (from p in context.ART_WF_ARTWORK_PROCESS
                                   where listSubID.Contains(p.ARTWORK_SUB_ID)
                                   && p.CURRENT_STEP_ID == QCStepID
                                   select p).ToList();
                if (QCProcesses != null && QCProcesses.Count > 0)
                {
                    var QCProcess = QCProcesses.OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();
                    qc2.ARTWORK_SUB_ID = QCProcess.ARTWORK_SUB_ID;
                }

                listQC2 = MapperServices.ART_WF_ARTWORK_PROCESS_QC(ART_WF_ARTWORK_PROCESS_QC_SERVICE.GetByItem(qc2, context).ToList());
                var listQC = 0;
                if (listQC2 != null && listQC2.Count > 0)
                {
                    listQC = listQC2.OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault().UPDATE_BY;
                }

                ART_WF_ARTWORK_PROCESS_2 process = new ART_WF_ARTWORK_PROCESS_2();
                process.ARTWORK_ITEM_ID = processCust.ARTWORK_ITEM_ID;
                process.ARTWORK_REQUEST_ID = processCust.ARTWORK_REQUEST_ID;
                process.REMARK = param.data.COMMENT;
                process.CREATE_BY = param.data.UPDATE_BY;
                process.UPDATE_BY = param.data.UPDATE_BY;
                if (listQC != 0)
                    process.CURRENT_USER_ID = listQC;
                process.CURRENT_ROLE_ID = step.ROLE_ID_RESPONSE;
                process.CURRENT_STEP_ID = step.STEP_ARTWORK_ID;
                process.PARENT_ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;

                //string msg = ArtworkProcessHelper.checkDupWF(_process, context);
                //if (msg != "")
                //{
                //    Results.status = "E";
                //    Results.msg = msg;
                //    return Results;
                //}
                var temp = ArtworkProcessHelper.SaveProcess(process, context);
                foreach (var itemTemp in temp.data)
                {
                    processResults.data.Add(itemTemp);
                }

                return processResults;
                //foreach (var process in processResults.data)
                //    EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO_CUSTOMER", context);
                // }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                //Results.status = "E";
                //Results.msg = CNService.GetErrorMessage(ex);
            }

            return processResults;

            //return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT EndTaskCustomerOtherUser(ART_WF_ARTWORK_PROCESS_CUSTOMER_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT Results = new ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT();

            List<int> listSubID = new List<int>();

            try
            {
                listSubID = CNService.FindArtworkSubId(param.data.ARTWORK_SUB_ID, context);

                var currentStepID = (from v in context.ART_WF_ARTWORK_PROCESS
                                     where v.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                     select v.CURRENT_STEP_ID).FirstOrDefault();

                var processCustomers = (from v in context.ART_WF_ARTWORK_PROCESS
                                        where listSubID.Contains(v.ARTWORK_SUB_ID)
                                           && v.CURRENT_STEP_ID == currentStepID
                                           && String.IsNullOrEmpty(v.IS_END)
                                        select v).ToList();

                if (processCustomers != null)
                {
                    ART_WF_ARTWORK_PROCESS _processVN = new ART_WF_ARTWORK_PROCESS();
                    foreach (ART_WF_ARTWORK_PROCESS iProcessVN in processCustomers)
                    {
                        iProcessVN.IS_END = "X";
                        iProcessVN.UPDATE_DATE = DateTime.Now;
                        iProcessVN.UPDATE_BY = -1;
                        ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(iProcessVN, context);
                    }
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
    }

}
