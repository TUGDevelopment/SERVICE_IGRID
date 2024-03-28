using BLL.DocumentManagement;
using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class GMMKByMKHelper
    {
        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_RESULT SaveGMMKByMK_(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_RESULT Results = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        ART_WF_ARTWORK_PROCESS_RESULT _processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
                        if (param.data.PROCESS != null)
                        {
                            _processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                        }

                        var MKData = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC(param.data);

                        var check = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                        if (check.Count > 0)
                            MKData.ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_ID = check.FirstOrDefault().ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_ID;

                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.SaveOrUpdate(MKData, context);

                        if (param.data.ENDTASKFORM)
                        {
                            ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                        }

                        Results.data = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2>();
                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2 item = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2();
                        List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2> listItem = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2>();

                        item.ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_ID = MKData.ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_ID;
                        listItem.Add(item);

                        Results.data = listItem;

                        dbContextTransaction.Commit();

                        foreach (var process in _processResults.data)
                            EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO", context);

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

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_RESULT GetGMMKByMK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_RESULT Results = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_SERVICE.GetAll(context));
                        }
                        else
                        {
                            var ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            param.data.ARTWORK_SUB_ID = 0;

                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_SERVICE.GetByItemContain(MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK(param.data), context));

                            param.data.ARTWORK_SUB_ID = ARTWORK_SUB_ID;
                            Results.data = Results.data.Where(m => CNService.FindArtworkSubId(ARTWORK_SUB_ID, context).Contains(m.ARTWORK_SUB_ID)).ToList();
                        }

                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK p = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK();

                        var stepIdGMQC = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_GM_QC" }, context).FirstOrDefault().STEP_ARTWORK_ID;


                        Results.status = "S";

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                ART_SYS_ACTION act = new ART_SYS_ACTION();
                                act.ACTION_CODE = Results.data[i].ACTION_CODE;
                                Results.data[i].ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItem(act, context).FirstOrDefault().ACTION_NAME;

                                var processFormPA = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context);

                                Results.data[i].COMMENT_BY_MK = processFormPA.REMARK;
                                Results.data[i].CREATE_DATE_BY_MK = processFormPA.CREATE_DATE;

                                var CusID = 0;
                                var CusReview_ = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = param.data.ARTWORK_ITEM_ID, ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID, CURRENT_STEP_ID = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REVIEW" }, context).FirstOrDefault().STEP_ARTWORK_ID }, context).OrderByDescending(o => o.UPDATE_DATE).ToList();
                                for (int x = 0; x < CusReview_.Count; x++)
                                {
                                    var check = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = CusReview_[x].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (check != null)
                                    {
                                        CusID = check.ARTWORK_SUB_ID;
                                        break;
                                    }

                                }

                                var dataCusbyPA = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = CusID }, context).FirstOrDefault();

                                if (dataCusbyPA != null)
                                {
                                    if (dataCusbyPA.IS_FORMLABEL != null)
                                        Results.data[i].IS_FORMLABEL = dataCusbyPA.IS_FORMLABEL;
                                    if (dataCusbyPA.IS_CHANGEDETAIL != null)
                                        Results.data[i].IS_CHANGEDETAIL = dataCusbyPA.IS_CHANGEDETAIL;
                                    if (dataCusbyPA.IS_NONCOMPLIANCE != null)
                                        Results.data[i].IS_NONCOMPLIANCE = dataCusbyPA.IS_CHANGEDETAIL;
                                    if (dataCusbyPA.IS_ADJUST != null)
                                        Results.data[i].IS_ADJUST = dataCusbyPA.IS_ADJUST;

                                    if (dataCusbyPA.COMMENT_ADJUST != null)
                                        Results.data[i].COMMENT_ADJUST = dataCusbyPA.COMMENT_ADJUST;
                                    else
                                        Results.data[i].COMMENT_ADJUST = "-";
                                    if (dataCusbyPA.COMMENT_NONCOMPLIANCE != null)
                                        Results.data[i].COMMENT_NONCOMPLIANCE = dataCusbyPA.COMMENT_NONCOMPLIANCE;
                                    else
                                        Results.data[i].COMMENT_NONCOMPLIANCE = "-";
                                    if (dataCusbyPA.COMMENT_FORM_LABEL != null)
                                        Results.data[i].COMMENT_CHANGE_DETAIL = dataCusbyPA.COMMENT_FORM_LABEL;
                                    else
                                        Results.data[i].COMMENT_CHANGE_DETAIL = "-";

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
                                    else
                                        Results.data[i].QC_COMMENT = "-";


                                    var SEND_TO_CUSTOMER_TYPE = dataCusbyPA.SEND_TO_CUSTOMER_TYPE;
                                    if (SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REVIEW")
                                    {

                                        var processFormCustomer = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = CusID }, context).FirstOrDefault();

                                        Results.data[i].COMMENT_FORMLABEL_DISPLAY = processFormCustomer.COMMENT_CHANGE_DETAIL;
                                        Results.data[i].COMMENT_NONCOMPLIANCE_DISPLAY = processFormCustomer.COMMENT_NONCOMPLIANCE;
                                        Results.data[i].COMMENT_ADJUST_DISPLAY = processFormCustomer.COMMENT_ADJUST;

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

                                        var processForm = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault();

                                        if (processForm != null)
                                        {
                                            if (processForm.REASON_ID != null)
                                                Results.data[i].REASON_BY_OTHER = CNService.getReason(processForm.REASON_ID, context);
                                            Results.data[i].COMMENT = processForm.REMARK;
                                        }

                                        var idGMQC = 0;
                                        var counterid = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                        if (counterid != null)
                                        {
                                            var tempGMQC = counterid.PARENT_ARTWORK_SUB_ID;
                                            while (idGMQC == 0)
                                            {
                                                var check = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Convert.ToInt32(tempGMQC) }, context).FirstOrDefault();
                                                if (check.CURRENT_STEP_ID == stepIdGMQC)
                                                {
                                                    idGMQC = check.ARTWORK_SUB_ID;
                                                    break;
                                                }
                                                else
                                                    tempGMQC = Convert.ToInt32(check.PARENT_ARTWORK_SUB_ID);
                                            }

                                            //var tempGMQC = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                            //tempGMQC = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Convert.ToInt32(tempGMQC.PARENT_ARTWORK_SUB_ID) }, context).FirstOrDefault();
                                            //tempGMQC = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Convert.ToInt32(tempGMQC.PARENT_ARTWORK_SUB_ID), CURRENT_STEP_ID = stepIdGMQC }, context).FirstOrDefault();

                                            //var processFormGMQC = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC() { ARTWORK_SUB_ID = tempGMQC.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                            var processFormGMQC = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC() { ARTWORK_SUB_ID = idGMQC }, context).FirstOrDefault();

                                            if (processFormGMQC != null)
                                            {
                                                Results.data[i].COMMENT_GM_QC_DISPLAY = processFormGMQC.COMMENT;
                                                if (processFormGMQC.ACTION_CODE == "APPROVE")
                                                {
                                                    Results.data[i].APPROVAL_GM_QC_DISPLAY = "Approve";
                                                }
                                                else if (processFormGMQC.ACTION_CODE == "NOTAPPROVE")
                                                {
                                                    Results.data[i].APPROVAL_GM_QC_DISPLAY = "Not approve";
                                                }
                                            }
                                        }

                                        var processFormGMMK = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault();

                                        if (processFormGMMK != null)
                                        {
                                            Results.data[i].COMMENT = processFormGMMK.COMMENT;
                                            Results.data[i].CREATE_DATE = processFormGMMK.CREATE_DATE;
                                            if (processFormGMMK.ACTION_CODE == "APPROVE")
                                            {
                                                Results.data[i].ACTION_NAME = "Approve";
                                            }
                                            else if (processFormGMMK.ACTION_CODE == "NOTAPPROVE")
                                            {
                                                Results.data[i].ACTION_NAME = "Not approve";
                                            }
                                            else if (processFormGMMK.ACTION_CODE == "SEND_BACK")
                                            {
                                                Results.data[i].ACTION_NAME = "Send back";
                                            }
                                            else if (processFormGMMK.ACTION_CODE == "SUBMIT")
                                            {
                                                Results.data[i].ACTION_NAME = "Submit";
                                            }
                                        }

                                        var PreviousStep = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault().PARENT_ARTWORK_SUB_ID;
                                        if (PreviousStep != null)
                                        {
                                            var CHK_ACTION_MK = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC() { ARTWORK_SUB_ID = Convert.ToInt32(PreviousStep) }, context).FirstOrDefault();
                                            if (CHK_ACTION_MK != null)
                                            {
                                                if (CHK_ACTION_MK.ACTION_CODE == "SEND_BACK")
                                                {
                                                    Results.data[i].ACTION_NAME_MK = "Send back";
                                                }
                                                else if (CHK_ACTION_MK.ACTION_CODE == "SUBMIT")
                                                {
                                                    Results.data[i].ACTION_NAME_MK = "Submit";
                                                }
                                            }
                                        }
                                    }
                                    else
                                        Results.data[i].ACTION_CODE = "delete";
                                }
                            }
                            Results.data = Results.data.Where(m => m.ACTION_CODE != "delete").ToList();
                            Results.data = Results.data.OrderByDescending(o => o.UPDATE_DATE).ToList();
                        }

                        var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_GM_MK" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var list = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context), CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var result = list.Where(q => !Results.data.Any(q2 => q2.ARTWORK_SUB_ID == q.ARTWORK_SUB_ID)).FirstOrDefault();
                        if (result != null)
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

                            var resultCustomer = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = CusID }, context).FirstOrDefault();
                            if (resultCustomer != null)
                            {
                                ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2 item = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2();

                                //Comment PA
                                if (resultCustomer.COMMENT_ADJUST != null)
                                    item.COMMENT_ADJUST = resultCustomer.COMMENT_ADJUST;
                                else
                                    item.COMMENT_ADJUST = "-";
                                if (resultCustomer.COMMENT_NONCOMPLIANCE != null)
                                    item.COMMENT_NONCOMPLIANCE = resultCustomer.COMMENT_NONCOMPLIANCE;
                                else
                                    item.COMMENT_NONCOMPLIANCE = "-";
                                if (resultCustomer.COMMENT_FORM_LABEL != null)
                                    item.COMMENT_CHANGE_DETAIL = resultCustomer.COMMENT_FORM_LABEL;
                                else
                                    item.COMMENT_CHANGE_DETAIL = "-";

                                if (resultCustomer.IS_FORMLABEL != null)
                                    item.IS_FORMLABEL = resultCustomer.IS_FORMLABEL;
                                if (resultCustomer.IS_CHANGEDETAIL != null)
                                    item.IS_CHANGEDETAIL = resultCustomer.IS_CHANGEDETAIL;
                                if (resultCustomer.IS_NONCOMPLIANCE != null)
                                    item.IS_NONCOMPLIANCE = resultCustomer.IS_CHANGEDETAIL;
                                if (resultCustomer.IS_ADJUST != null)
                                    item.IS_ADJUST = resultCustomer.IS_ADJUST;


                                if (resultCustomer.NUTRITION_COMMENT != "<p><br></p>" && resultCustomer.NUTRITION_COMMENT != null)
                                    item.NUTRITION_COMMENT_DISPLAY = resultCustomer.NUTRITION_COMMENT;
                                else
                                    item.NUTRITION_COMMENT_DISPLAY = "-";
                                if (resultCustomer.INGREDIENTS_COMMENT != "<p><br></p>" && resultCustomer.INGREDIENTS_COMMENT != null)
                                    item.INGREDIENTS_COMMENT_DISPLAY = resultCustomer.INGREDIENTS_COMMENT;
                                else
                                    item.INGREDIENTS_COMMENT_DISPLAY = "-";
                                if (resultCustomer.ANALYSIS_COMMENT != "<p><br></p>" && resultCustomer.ANALYSIS_COMMENT != null)
                                    item.ANALYSIS_COMMENT_DISPLAY = resultCustomer.ANALYSIS_COMMENT;
                                else
                                    item.ANALYSIS_COMMENT_DISPLAY = "-";
                                if (resultCustomer.HEALTH_CLAIM_COMMENT != "<p><br></p>" && resultCustomer.HEALTH_CLAIM_COMMENT != null)
                                    item.HEALTH_CLAIM_COMMENT_DISPLAY = resultCustomer.HEALTH_CLAIM_COMMENT;
                                else
                                    item.HEALTH_CLAIM_COMMENT_DISPLAY = "-";
                                if (resultCustomer.NUTRIENT_CLAIM_COMMENT != "<p><br></p>" && resultCustomer.NUTRIENT_CLAIM_COMMENT != null)
                                    item.NUTRIENT_CLAIM_COMMENT_DISPLAY = resultCustomer.NUTRIENT_CLAIM_COMMENT;
                                else
                                    item.NUTRIENT_CLAIM_COMMENT_DISPLAY = "-";
                                if (resultCustomer.SPECIES_COMMENT != "<p><br></p>" && resultCustomer.SPECIES_COMMENT != null)
                                    item.SPECIES_COMMENT_DISPLAY = resultCustomer.SPECIES_COMMENT;
                                else
                                    item.SPECIES_COMMENT_DISPLAY = "-";
                                if (resultCustomer.CATCHING_AREA_COMMENT != "<p><br></p>" && resultCustomer.CATCHING_AREA_COMMENT != null)
                                    item.CATCHING_AREA_COMMENT_DISPLAY = resultCustomer.CATCHING_AREA_COMMENT;
                                else
                                    item.CATCHING_AREA_COMMENT_DISPLAY = "-";

                                if (resultCustomer.CHECK_DETAIL_COMMENT != "<p><br></p>" && resultCustomer.CHECK_DETAIL_COMMENT != null)
                                    item.CHECK_DETAIL_COMMENT_DISPLAY = resultCustomer.CHECK_DETAIL_COMMENT;
                                else
                                    item.CHECK_DETAIL_COMMENT_DISPLAY = "-";

                                if (resultCustomer.QC_COMMENT != "" && resultCustomer.QC_COMMENT != null)
                                    item.QC_COMMENT = resultCustomer.QC_COMMENT;
                                else
                                    item.QC_COMMENT = "-";

                                //Comment Customer
                                var processFormCustomer = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = resultCustomer.ARTWORK_SUB_ID }, context).FirstOrDefault();

                                item.COMMENT_FORMLABEL_DISPLAY = processFormCustomer.COMMENT_CHANGE_DETAIL;
                                item.COMMENT_NONCOMPLIANCE_DISPLAY = processFormCustomer.COMMENT_NONCOMPLIANCE;
                                item.COMMENT_ADJUST_DISPLAY = processFormCustomer.COMMENT_ADJUST;

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

                                //var tempGMQC = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                //tempGMQC = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Convert.ToInt32(tempGMQC.PARENT_ARTWORK_SUB_ID) }, context).FirstOrDefault();
                                //tempGMQC = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Convert.ToInt32(tempGMQC.PARENT_ARTWORK_SUB_ID), CURRENT_STEP_ID = stepIdGMQC }, context).FirstOrDefault();
                                //var processFormGMQC = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC() { ARTWORK_SUB_ID = tempGMQC.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                var idGMQC = 0;
                                var counterid = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                if (counterid != null)
                                {
                                    var tempGMQC = counterid.PARENT_ARTWORK_SUB_ID;
                                    while (idGMQC == 0)
                                    {
                                        var check = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Convert.ToInt32(tempGMQC) }, context).FirstOrDefault();
                                        if (check.CURRENT_STEP_ID == stepIdGMQC)
                                        {
                                            idGMQC = check.ARTWORK_SUB_ID;
                                            break;
                                        }
                                        else
                                            tempGMQC = Convert.ToInt32(check.PARENT_ARTWORK_SUB_ID);
                                    }

                                    var processFormGMQC = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC() { ARTWORK_SUB_ID = idGMQC }, context).FirstOrDefault();

                                    if (processFormGMQC != null)
                                    {
                                        item.COMMENT_GM_QC_DISPLAY = processFormGMQC.COMMENT;
                                        if (processFormGMQC.ACTION_CODE == "APPROVE")
                                        {
                                            item.APPROVAL_GM_QC_DISPLAY = "Approve";
                                        }
                                        else if (processFormGMQC.ACTION_CODE == "NOTAPPROVE")
                                        {
                                            item.APPROVAL_GM_QC_DISPLAY = "Not approve";
                                        }
                                    }
                                }

                                var processFormGMMK = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault();

                                if (processFormGMMK != null)
                                {
                                    item.COMMENT = processFormGMMK.COMMENT;
                                    item.CREATE_DATE = processFormGMMK.CREATE_DATE;
                                    if (processFormGMMK.ACTION_CODE == "APPROVE")
                                    {
                                        item.ACTION_NAME = "Approve";
                                    }
                                    else if (processFormGMMK.ACTION_CODE == "NOTAPPROVE")
                                    {
                                        item.ACTION_NAME = "Not approve";
                                    }
                                    else if (processFormGMMK.ACTION_CODE == "SEND_BACK")
                                    {
                                        item.ACTION_NAME = "Send back";
                                    }
                                    else if (processFormGMMK.ACTION_CODE == "SUBMIT")
                                    {
                                        item.ACTION_NAME = "Submit";
                                    }
                                }

                                var PreviousStep = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault().PARENT_ARTWORK_SUB_ID;
                                if (PreviousStep != null)
                                {
                                    var CHK_ACTION_MK = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC() { ARTWORK_SUB_ID = Convert.ToInt32(PreviousStep) }, context).FirstOrDefault();
                                    if (CHK_ACTION_MK != null)
                                    {
                                        if (CHK_ACTION_MK.ACTION_CODE == "SEND_BACK")
                                        {
                                            item.ACTION_NAME_MK = "Send back";
                                        }
                                        else if (CHK_ACTION_MK.ACTION_CODE == "SUBMIT")
                                        {
                                            item.ACTION_NAME_MK = "Submit";
                                        }
                                    }
                                }

                                item.CREATE_DATE_BY_MK = result.CREATE_DATE;
                                item.COMMENT_BY_MK = result.REMARK;

                                item.ARTWORK_SUB_ID = result.ARTWORK_SUB_ID;
                                item.ARTWORK_REQUEST_ID = result.ARTWORK_REQUEST_ID;

                                Results.data.Add(item);
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

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_RESULT PostGMMKSendToPA(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_RESULT Results = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var mkData = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK(param.data);

                        var check = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                        if (check.Count > 0)
                            mkData.ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_ID = check.FirstOrDefault().ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_ID;

                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_SERVICE.SaveOrUpdate(mkData, context);

                        var processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
                        processResults.data = new List<ART_WF_ARTWORK_PROCESS_2>();
                        if (param.data.ENDTASKFORM)
                        {
                            ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                            if (param.data.ACTION_CODE == "NOTAPPROVE" && param.data.IS_ADJUST == null || param.data.ACTION_CODE == "SEND_BACK")
                            {
                                processResults = AutomaticCreateProcessMK(param, context);
                            }
                        }

                        Results.data = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2>();
                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2 item = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2();
                        List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2> listItem = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2>();

                        item.ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_ID = mkData.ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_ID;
                        listItem.Add(item);

                        Results.data = listItem;

                        dbContextTransaction.Commit();

                        foreach (var process in processResults.data)
                        {
                            if (param.data.ACTION_CODE == "SEND_BACK")
                                EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_BACK", context, param.data.COMMENT);
                            else
                                EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO", context);
                        }

                        if (param.data.ACTION_CODE == "APPROVE" || param.data.IS_ADJUST == "X")
                        {
                            var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                            var ID_PA = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context), ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID, CURRENT_STEP_ID = stepId }, context).FirstOrDefault();

                            EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, ID_PA.ARTWORK_SUB_ID, "WF_SEND_TO", context);
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

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_RESULT PostGMMKSendToPAMulti(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_REQUEST_LIST param)
        {
            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_RESULT Results = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_RESULT();
            try
            {
                if (param != null && param.data != null)
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (var dbContextTransaction = CNService.IsolationLevel(context))
                        {
                            if (param.data.FirstOrDefault().ACTION_CODE == "APPROVE")
                            {
                                if (DuplicateReferenceLetter(param, context))
                                {
                                    Results.status = "E";
                                    Results.msg = MessageHelper.GetMessage("MSG_011", context);
                                }
                            }

                            foreach (ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2 iData in param.data)
                            {

                                var mkData = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK(iData);

                                var check = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK() { ARTWORK_SUB_ID = iData.ARTWORK_SUB_ID }, context);
                                if (check.Count > 0)
                                    mkData.ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_ID = check.FirstOrDefault().ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_ID;

                                ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_SERVICE.SaveOrUpdate(mkData, context);

                                ART_WF_ARTWORK_PROCESS_REQUEST aw_process_request = new ART_WF_ARTWORK_PROCESS_REQUEST();
                                var itemAcceptTask = new ART_WF_ARTWORK_PROCESS_2();
                                itemAcceptTask.ARTWORK_REQUEST_ID = iData.ARTWORK_REQUEST_ID;
                                itemAcceptTask.ARTWORK_SUB_ID = iData.ARTWORK_SUB_ID;
                                itemAcceptTask.UPDATE_BY = iData.UPDATE_BY;
                                itemAcceptTask.CURRENT_USER_ID = iData.UPDATE_BY;
                                aw_process_request.data = itemAcceptTask;
                                ArtworkProcessHelper.AcceptTask(aw_process_request);

                                if (iData.ENDTASKFORM)
                                {
                                    ArtworkProcessHelper.EndTaskForm(iData.ARTWORK_SUB_ID, iData.UPDATE_BY, context);
                                }
                            }

                            dbContextTransaction.Commit();

                            foreach (var process in param.data)
                            {
                                var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                                var ID_PA = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = CNService.FindArtworkItemId(process.ARTWORK_SUB_ID, context), ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID, CURRENT_STEP_ID = stepId }, context).FirstOrDefault();

                                EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, ID_PA.ARTWORK_SUB_ID, "WF_SEND_TO", context);
                            }

                            Results.status = "S";
                            Results.msg = MessageHelper.GetMessage("MSG_001", context);
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

        private static bool DuplicateReferenceLetter(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_REQUEST_LIST param, ARTWORKEntities context)
        {
            if (param.data.Count > 1)
            {
                var isFiles = false;
                Node[] nodeFiles = new Node[0];
                List<ART_WF_ARTWORK_ATTACHMENT> listAttTo = new List<ART_WF_ARTWORK_ATTACHMENT>();
                var token = CWSService.getAuthToken();
                var folderRLName = ConfigurationManager.AppSettings["ArtworkFolderNameReferenceLetter"];
                int updated_by = 0;
                foreach (ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2 iData in param.data)
                {
                    int? parent_artworkSubId = CNService.FindParentArtworkSubId(iData.ARTWORK_SUB_ID, context);
                    if (parent_artworkSubId != null)
                    {
                        var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(parent_artworkSubId, context);
                        if (process != null)
                        {
                            Node nodeAW = CWSService.getNodeByName(Convert.ToInt64(process.ARTWORK_FOLDER_NODE_ID), folderRLName, token);
                            if (nodeAW != null)
                            {
                                Node[] nodeRLFiles = CWSService.getAllNodeInFolder(nodeAW.ID, token);
                                if (nodeRLFiles != null)
                                {
                                    if (!isFiles && nodeRLFiles.Length > 0)
                                    {
                                        isFiles = true;
                                        nodeFiles = nodeRLFiles;
                                        updated_by = iData.UPDATE_BY;
                                    }
                                    else if (nodeRLFiles.Length > 0)
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    listAttTo.Add(new ART_WF_ARTWORK_ATTACHMENT()
                                    {
                                        ARTWORK_REQUEST_ID = iData.ARTWORK_REQUEST_ID,
                                        ARTWORK_SUB_ID = iData.ARTWORK_SUB_ID,
                                        NODE_ID = nodeAW.ID
                                    });
                                }
                            }
                        }
                    }
                }

                if (isFiles)
                {
                    if (nodeFiles.Length > 0)
                    {
                        foreach (var ifile in nodeFiles)
                        {
                            foreach (var d in listAttTo)
                            {
                                var node = CWSService.copyNode(ifile.Name, ifile.ID, d.NODE_ID.GetValueOrDefault(), token);
                                var l = ART_WF_ARTWORK_ATTACHMENT_SERVICE.GetByItem(new ART_WF_ARTWORK_ATTACHMENT() { NODE_ID = ifile.ID }, context).FirstOrDefault();
                                if (l != null)
                                {
                                    ART_WF_ARTWORK_ATTACHMENT attachmentData = new ART_WF_ARTWORK_ATTACHMENT();
                                    attachmentData.ARTWORK_REQUEST_ID = d.ARTWORK_REQUEST_ID;
                                    attachmentData.ARTWORK_SUB_ID = d.ARTWORK_SUB_ID;
                                    attachmentData.NODE_ID = node.ID;
                                    attachmentData.SIZE = l.SIZE;
                                    attachmentData.CONTENT_TYPE = l.CONTENT_TYPE;
                                    attachmentData.FILE_NAME = l.FILE_NAME;
                                    attachmentData.EXTENSION = l.EXTENSION;
                                    attachmentData.CREATE_BY = l.CREATE_BY;
                                    attachmentData.UPDATE_BY = l.UPDATE_BY;
                                    attachmentData.ROLE_ID = l.ROLE_ID;

                                    attachmentData.VERSION2 = l.VERSION2;
                                    attachmentData.VERSION = l.VERSION;
                                    attachmentData.IS_INTERNAL = l.IS_INTERNAL;
                                    attachmentData.IS_CUSTOMER = l.IS_CUSTOMER;
                                    attachmentData.IS_VENDOR = l.IS_VENDOR;
                                    attachmentData.IS_SYSTEM = "X";
                                    attachmentData.STEP_ARTWORK_ID = l.STEP_ARTWORK_ID;

                                    ART_WF_ARTWORK_ATTACHMENT_SERVICE.SaveOrUpdate(attachmentData, context);
                                }

                            }
                        }
                    }

                }

            }
            return false;
        }

        public static ART_WF_ARTWORK_PROCESS_RESULT AutomaticCreateProcessMK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_PROCESS_RESULT processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
            processResults.data = new List<ART_WF_ARTWORK_PROCESS_2>();
            //ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_RESULT Results = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_RESULT();
            try
            {
                //if (param == null || param.data == null)
                //{
                //    return Results;
                //}
                //else
                //{
                string stepName = "SEND_MK_VERIFY";
                var processCust = context.ART_WF_ARTWORK_PROCESS.Where(c => c.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).FirstOrDefault();
                var step = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == stepName).FirstOrDefault();

                ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2 mk2 = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2();
                List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2> listMK2 = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2>();

                List<int> listSubID = CNService.FindArtworkSubId(param.data.ARTWORK_SUB_ID, context);

                var MKStepID = context.ART_M_STEP_ARTWORK.Where(c => c.STEP_ARTWORK_CODE == "SEND_MK_VERIFY").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
                var MKProcesses = (from p in context.ART_WF_ARTWORK_PROCESS
                                   where listSubID.Contains(p.ARTWORK_SUB_ID)
                                   && p.CURRENT_STEP_ID == MKStepID
                                   select p).ToList();
                if (MKProcesses != null && MKProcesses.Count > 0)
                {
                    var MKProcess = MKProcesses.OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();
                    mk2.ARTWORK_SUB_ID = MKProcess.ARTWORK_SUB_ID;
                }

                listMK2 = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.GetByItem(mk2, context).ToList());
                var listMK = 0;
                if (listMK2 != null && listMK2.Count > 0)
                {
                    listMK = listMK2.OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault().UPDATE_BY;
                }

                ART_WF_ARTWORK_PROCESS_2 process = new ART_WF_ARTWORK_PROCESS_2();
                process.ARTWORK_ITEM_ID = processCust.ARTWORK_ITEM_ID;
                process.ARTWORK_REQUEST_ID = processCust.ARTWORK_REQUEST_ID;
                process.REMARK = param.data.COMMENT;
                process.CREATE_BY = param.data.UPDATE_BY;
                process.UPDATE_BY = param.data.UPDATE_BY;
                if (listMK != 0)
                    process.CURRENT_USER_ID = listMK;
                process.CURRENT_ROLE_ID = step.ROLE_ID_RESPONSE;
                process.CURRENT_STEP_ID = step.STEP_ARTWORK_ID;
                process.PARENT_ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;

                var temp = ArtworkProcessHelper.SaveProcess(process, context);

                foreach (var itemTemp in temp.data)
                {
                    processResults.data.Add(itemTemp);
                }

                //foreach (var process in processResults.data)
                //    if (param.data.ACTION_CODE == "SEND_BACK")
                //        EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_BACK", context, param.data.COMMENT);
                //    else
                //        EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO", context);
                //}
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
                //Results.status = "E";
                //Results.msg = CNService.GetErrorMessage(ex);
            }

            return processResults;
        }
    }
}
