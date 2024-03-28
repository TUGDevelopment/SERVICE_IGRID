using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class QCByMKHelper
    {
        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_RESULT SaveQCByMK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_REQUEST param)
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
                        Results.msg = MessageHelper.GetMessage("MSG_001",context);
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

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT GetQCVerify(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT Results = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.GetAll(context));
                        }
                        else
                        {
                            var ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            param.data.ARTWORK_SUB_ID = 0;

                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.GetByItemContain(MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC(param.data), context));

                            param.data.ARTWORK_SUB_ID = ARTWORK_SUB_ID;
                            Results.data = Results.data.Where(m => CNService.FindArtworkSubId(ARTWORK_SUB_ID, context).Contains(m.ARTWORK_SUB_ID)).ToList();
                        }

                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC p = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC();

                        Results.status = "S";

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                var CusID = 0;
                                var CusReview_ = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = param.data.ARTWORK_ITEM_ID, ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID, CURRENT_STEP_ID = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REVIEW" }, context).FirstOrDefault().STEP_ARTWORK_ID }, context).Where(e => e.CREATE_DATE < Results.data[i].CREATE_DATE).OrderByDescending(o => o.UPDATE_DATE).ToList();
                                for (int x = 0; x < CusReview_.Count; x++)
                                {
                                    var check = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = CusReview_[x].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (check != null)
                                    {
                                        CusID = check.ARTWORK_SUB_ID;
                                        break;
                                    }
                                }

                                var PreviousStep = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault().PARENT_ARTWORK_SUB_ID;
                                var PreviousStep_PA = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Convert.ToInt32(PreviousStep) }, context).FirstOrDefault().PARENT_ARTWORK_SUB_ID;
                                if (PreviousStep != null)
                                {
                                    var CHK_REQ_TYPE = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = Convert.ToInt32(PreviousStep) }, context).FirstOrDefault();
                                    var CHK_REQ_TYPE_PA = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = Convert.ToInt32(PreviousStep_PA) }, context).FirstOrDefault();
                                    var CHK_REQ_TYPE_GM_QC = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC() { ARTWORK_SUB_ID = Convert.ToInt32(PreviousStep) }, context).FirstOrDefault();
                                    var CHK_REQ_TYPE_MK = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC() { ARTWORK_SUB_ID = Convert.ToInt32(PreviousStep) }, context).FirstOrDefault();

                                    if (CHK_REQ_TYPE != null)
                                    {
                                        Results.data[i].SEND_TO_CUSTOMER_TYPE = CHK_REQ_TYPE.SEND_TO_CUSTOMER_TYPE;
                                        if (CHK_REQ_TYPE.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REVIEW")
                                            Results.data[i].PREV_STEP_DISPLAY = "Review artwork";
                                        if (CHK_REQ_TYPE.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REQ_REF")
                                            Results.data[i].PREV_STEP_DISPLAY = "Request reference letter";
                                    }
                                    else if (CHK_REQ_TYPE_PA != null)
                                    {
                                        Results.data[i].SEND_TO_CUSTOMER_TYPE = CHK_REQ_TYPE_PA.SEND_TO_CUSTOMER_TYPE;
                                        if (CHK_REQ_TYPE_PA.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REVIEW")
                                            Results.data[i].PREV_STEP_DISPLAY = "Review artwork";
                                        if (CHK_REQ_TYPE_PA.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REQ_REF")
                                            Results.data[i].PREV_STEP_DISPLAY = "Request reference letter";
                                    }
                                    else if (CHK_REQ_TYPE_GM_QC != null)
                                    {
                                        Results.data[i].PREV_STEP_DISPLAY = "Approval GM QC";
                                    }
                                    else if (CHK_REQ_TYPE_MK != null)
                                    {
                                        Results.data[i].PREV_STEP_DISPLAY = "MK review customer's decision";
                                    }
                                }

                                var dataProcess = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                if (dataProcess != null)
                                {
                                    Results.data[i].COMMENT_BY_PA = dataProcess.REMARK;
                                }


                                var dataCusbyPA = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = CusID }, context).FirstOrDefault();

                                if (dataCusbyPA != null)
                                {
                                    //Comment PA
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


                                    if (dataCusbyPA.IS_FORMLABEL != null)
                                        Results.data[i].IS_FORMLABEL = dataCusbyPA.IS_FORMLABEL;
                                    if (dataCusbyPA.IS_CHANGEDETAIL != null)
                                        Results.data[i].IS_CHANGEDETAIL = dataCusbyPA.IS_CHANGEDETAIL;
                                    if (dataCusbyPA.IS_NONCOMPLIANCE != null)
                                        Results.data[i].IS_NONCOMPLIANCE = dataCusbyPA.IS_NONCOMPLIANCE;
                                    if (dataCusbyPA.IS_ADJUST != null)
                                        Results.data[i].IS_ADJUST = dataCusbyPA.IS_ADJUST;

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
                                        ART_SYS_ACTION act = new ART_SYS_ACTION();
                                        act.ACTION_CODE = Results.data[i].ACTION_CODE;
                                        Results.data[i].ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItemContain(act, context).FirstOrDefault().ACTION_NAME;

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

                                        Results.data[i].CREATE_DATE_BY_PA = processFormCustomer.CREATE_DATE;

                                        if (Results.data[i].APPROVE == "1")
                                            Results.data[i].APPROVE = "Approve";
                                        else if (Results.data[i].APPROVE == "0")
                                            Results.data[i].APPROVE = "Not Approve";
                                        else
                                            Results.data[i].APPROVE = "-";
                                    }
                                    //else
                                    //    Results.data[i].ACTION_CODE = "delete";
                                }

                            }
                            //Results.data = Results.data.Where(m => m.ACTION_CODE != "delete").ToList();
                            Results.data = Results.data.OrderByDescending(o => o.UPDATE_DATE).ToList();
                        }

                        var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_QC_VERIFY" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var list = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID, CURRENT_STEP_ID = stepId }, context);
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var listCustomer = list.Where(q => !Results.data.Any(q2 => q2.ARTWORK_SUB_ID == q.ARTWORK_SUB_ID)).FirstOrDefault();
                        if (listCustomer != null)
                        {
                            var CusID = 0;
                            var CusReview_ = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = listCustomer.ARTWORK_ITEM_ID, ARTWORK_REQUEST_ID = listCustomer.ARTWORK_REQUEST_ID, CURRENT_STEP_ID = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REVIEW" }, context).FirstOrDefault().STEP_ARTWORK_ID }, context).OrderByDescending(o => o.UPDATE_DATE).ToList();
                            for (int x = 0; x < CusReview_.Count; x++)
                            {
                                var check = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = CusReview_[x].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                if (check != null)
                                {
                                    CusID = check.ARTWORK_SUB_ID;
                                    break;
                                }
                            }

                            var result = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = CusID }, context).FirstOrDefault();
                            if (result != null)
                            {
                                ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2 item = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2();
                                var artwork_req = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID }, context).FirstOrDefault();
                                if (CNService.GetMarketingCreatedArtworkRequest(artwork_req, context))
                                    item.IS_CHECK_VERIFY = "1";
                                //Comment PA
                                if (result.COMMENT_ADJUST != null)
                                    item.COMMENT_ADJUST = result.COMMENT_ADJUST;
                                else
                                    item.COMMENT_ADJUST = "-";
                                if (result.COMMENT_NONCOMPLIANCE != null)
                                    item.COMMENT_NONCOMPLIANCE = result.COMMENT_NONCOMPLIANCE;
                                else
                                    item.COMMENT_NONCOMPLIANCE = "-";
                                if (result.COMMENT_FORM_LABEL != null)
                                    item.COMMENT_CHANGE_DETAIL = result.COMMENT_FORM_LABEL;
                                else
                                    item.COMMENT_CHANGE_DETAIL = "-";

                                if (result.IS_FORMLABEL != null)
                                    item.IS_FORMLABEL = result.IS_FORMLABEL;
                                if (result.IS_CHANGEDETAIL != null)
                                    item.IS_CHANGEDETAIL = result.IS_CHANGEDETAIL;
                                if (result.IS_NONCOMPLIANCE != null)
                                    item.IS_NONCOMPLIANCE = result.IS_NONCOMPLIANCE;
                                if (result.IS_ADJUST != null)
                                    item.IS_ADJUST = result.IS_ADJUST;


                                if (result.NUTRITION_COMMENT != "<p><br></p>" && result.NUTRITION_COMMENT != null)
                                    item.NUTRITION_COMMENT_DISPLAY = result.NUTRITION_COMMENT;
                                else
                                    item.NUTRITION_COMMENT_DISPLAY = "-";
                                if (result.INGREDIENTS_COMMENT != "<p><br></p>" && result.INGREDIENTS_COMMENT != null)
                                    item.INGREDIENTS_COMMENT_DISPLAY = result.INGREDIENTS_COMMENT;
                                else
                                    item.INGREDIENTS_COMMENT_DISPLAY = "-";
                                if (result.ANALYSIS_COMMENT != "<p><br></p>" && result.ANALYSIS_COMMENT != null)
                                    item.ANALYSIS_COMMENT_DISPLAY = result.ANALYSIS_COMMENT;
                                else
                                    item.ANALYSIS_COMMENT_DISPLAY = "-";
                                if (result.HEALTH_CLAIM_COMMENT != "<p><br></p>" && result.HEALTH_CLAIM_COMMENT != null)
                                    item.HEALTH_CLAIM_COMMENT_DISPLAY = result.HEALTH_CLAIM_COMMENT;
                                else
                                    item.HEALTH_CLAIM_COMMENT_DISPLAY = "-";
                                if (result.NUTRIENT_CLAIM_COMMENT != "<p><br></p>" && result.NUTRIENT_CLAIM_COMMENT != null)
                                    item.NUTRIENT_CLAIM_COMMENT_DISPLAY = result.NUTRIENT_CLAIM_COMMENT;
                                else
                                    item.NUTRIENT_CLAIM_COMMENT_DISPLAY = "-";
                                if (result.SPECIES_COMMENT != "<p><br></p>" && result.SPECIES_COMMENT != null)
                                    item.SPECIES_COMMENT_DISPLAY = result.SPECIES_COMMENT;
                                else
                                    item.SPECIES_COMMENT_DISPLAY = "-";
                                if (result.CATCHING_AREA_COMMENT != "<p><br></p>" && result.CATCHING_AREA_COMMENT != null)
                                    item.CATCHING_AREA_COMMENT_DISPLAY = result.CATCHING_AREA_COMMENT;
                                else
                                    item.CATCHING_AREA_COMMENT_DISPLAY = "-";
                                if (result.CHECK_DETAIL_COMMENT != "<p><br></p>" && result.CHECK_DETAIL_COMMENT != null)
                                    item.CHECK_DETAIL_COMMENT_DISPLAY = result.CHECK_DETAIL_COMMENT;
                                else
                                    item.CHECK_DETAIL_COMMENT_DISPLAY = "-";

                                if (result.QC_COMMENT != "" && result.QC_COMMENT != null)
                                    item.QC_COMMENT = result.QC_COMMENT;
                                else
                                    item.QC_COMMENT = "-";

                                //Comment Customer
                                var processFormCustomer = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault();

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


                                var PreviousStep = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = listCustomer.ARTWORK_SUB_ID }, context).FirstOrDefault().PARENT_ARTWORK_SUB_ID;
                                var PreviousStep_PA = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Convert.ToInt32(PreviousStep) }, context).FirstOrDefault().PARENT_ARTWORK_SUB_ID;
                                if (PreviousStep != null)
                                {
                                    var CHK_REQ_TYPE = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = Convert.ToInt32(PreviousStep) }, context).FirstOrDefault();
                                    var CHK_REQ_TYPE_PA = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = Convert.ToInt32(PreviousStep_PA) }, context).FirstOrDefault();
                                    var CHK_REQ_TYPE_GM_QC = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC() { ARTWORK_SUB_ID = Convert.ToInt32(PreviousStep) }, context).FirstOrDefault();
                                    var CHK_REQ_TYPE_MK = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC() { ARTWORK_SUB_ID = Convert.ToInt32(PreviousStep) }, context).FirstOrDefault();

                                    if (CHK_REQ_TYPE != null)
                                    {
                                        item.SEND_TO_CUSTOMER_TYPE = CHK_REQ_TYPE.SEND_TO_CUSTOMER_TYPE;
                                        if (CHK_REQ_TYPE.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REVIEW")
                                            item.PREV_STEP_DISPLAY = "Review artwork";
                                        if (CHK_REQ_TYPE.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REQ_REF")
                                            item.PREV_STEP_DISPLAY = "Request reference letter";
                                    }
                                    else if (CHK_REQ_TYPE_PA != null)
                                    {
                                        item.SEND_TO_CUSTOMER_TYPE = CHK_REQ_TYPE_PA.SEND_TO_CUSTOMER_TYPE;
                                        if (CHK_REQ_TYPE_PA.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REVIEW")
                                            item.PREV_STEP_DISPLAY = "Review artwork";
                                        if (CHK_REQ_TYPE_PA.SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REQ_REF")
                                            item.PREV_STEP_DISPLAY = "Request reference letter";
                                    }
                                    else if (CHK_REQ_TYPE_GM_QC != null)
                                    {
                                        item.PREV_STEP_DISPLAY = "GM QC review customer's reference letter";
                                    }
                                    else if (CHK_REQ_TYPE_MK != null)
                                    {
                                        item.PREV_STEP_DISPLAY = "MK review customer's decision";
                                    }
                                }


                                item.CREATE_DATE_BY_PA = list.FirstOrDefault().CREATE_DATE;
                                item.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                item.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;
                                if (list.FirstOrDefault() != null)
                                    item.COMMENT_BY_PA = list.FirstOrDefault().REMARK;

                                Results.data = Results.data.Where(m => m.PREV_STEP_DISPLAY == item.PREV_STEP_DISPLAY).ToList();

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
    }

}
