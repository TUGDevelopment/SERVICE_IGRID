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
    public class GMQCByQCHelper
    {

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_RESULT GetGMQCByQC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_RESULT Results = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_SERVICE.GetAll(context));
                        }
                        else
                        {
                            var ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            param.data.ARTWORK_SUB_ID = 0;

                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_SERVICE.GetByItemContain(MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC(param.data), context));

                            param.data.ARTWORK_SUB_ID = ARTWORK_SUB_ID;
                            Results.data = Results.data.Where(m => CNService.FindArtworkSubId(ARTWORK_SUB_ID, context).Contains(m.ARTWORK_SUB_ID)).ToList();
                        }

                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC p = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC();

                        var stepQCId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_QC_VERIFY" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var stepMKId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_MK_VERIFY" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                        Results.status = "S";

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                ART_SYS_ACTION act = new ART_SYS_ACTION();
                                act.ACTION_CODE = Results.data[i].ACTION_CODE;
                                Results.data[i].ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItem(act, context).FirstOrDefault().ACTION_NAME;

                                var processFormPA = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context);

                                Results.data[i].COMMENT_BY_QC = processFormPA.REMARK;
                                Results.data[i].CREATE_DATE_BY_QC = processFormPA.CREATE_DATE;

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

                                        var getparent_ = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                        if (getparent_ != null)
                                        {
                                            var getparent = getparent_.PARENT_ARTWORK_SUB_ID;
                                            var checkPrevStep = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Convert.ToInt32(getparent) }, context).FirstOrDefault();

                                            if (checkPrevStep.CURRENT_STEP_ID == stepMKId)
                                            {
                                                var processMK = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC() { ARTWORK_SUB_ID = Convert.ToInt32(getparent) }, context).FirstOrDefault();

                                                if (processMK != null)
                                                {
                                                    Results.data[i].PREV_STEP = "Verified by PM/MK/MC";
                                                    if (processMK.ACTION_CODE == "SUBMIT")
                                                        Results.data[i].ACTION_NAME_SENDER = "Submit";
                                                    if (processMK.ACTION_CODE == "SEND_BACK")
                                                        Results.data[i].ACTION_NAME_SENDER = "Send back";
                                                }
                                            }
                                            else if (checkPrevStep.CURRENT_STEP_ID == stepQCId)
                                            {
                                                var processQC = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC() { ARTWORK_SUB_ID = Convert.ToInt32(getparent) }, context).FirstOrDefault();

                                                if (processQC != null)
                                                {
                                                    Results.data[i].PREV_STEP = "Verified by QC";
                                                    if (processQC.ACTION_CODE == "SUBMIT")
                                                        Results.data[i].ACTION_NAME_SENDER = "Submit";
                                                    if (processQC.ACTION_CODE == "SEND_BACK")
                                                        Results.data[i].ACTION_NAME_SENDER = "Send back";
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

                        var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_GM_QC" }, context).FirstOrDefault().STEP_ARTWORK_ID;
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
                                ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_2 item = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_2();

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

                                var getparent_ = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                if (getparent_ != null)
                                {
                                    var getparent = getparent_.PARENT_ARTWORK_SUB_ID;
                                    var checkPrevStep = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Convert.ToInt32(getparent) }, context).FirstOrDefault();

                                    if (checkPrevStep.CURRENT_STEP_ID == stepMKId)
                                    {
                                        var processMK = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC() { ARTWORK_SUB_ID = Convert.ToInt32(getparent) }, context).FirstOrDefault();

                                        if (processMK != null)
                                        {
                                            item.PREV_STEP = "Verified by PM/MK/MC";
                                            if (processMK.ACTION_CODE == "SUBMIT")
                                                item.ACTION_NAME_SENDER = "Submit";
                                            if (processMK.ACTION_CODE == "SEND_BACK")
                                                item.ACTION_NAME_SENDER = "Send back";
                                        }
                                    }
                                    else if (checkPrevStep.CURRENT_STEP_ID == stepQCId)
                                    {
                                        var processQC = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC() { ARTWORK_SUB_ID = Convert.ToInt32(getparent) }, context).FirstOrDefault();

                                        if (processQC != null)
                                        {
                                            item.PREV_STEP = "Verified by QC";
                                            if (processQC.ACTION_CODE == "SUBMIT")
                                                item.ACTION_NAME_SENDER = "Submit";
                                            if (processQC.ACTION_CODE == "SEND_BACK")
                                                item.ACTION_NAME_SENDER = "Send back";
                                        }
                                    }
                                }

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


                                item.CREATE_DATE_BY_QC = result.CREATE_DATE;
                                item.COMMENT_BY_QC = result.REMARK;

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

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_RESULT PostGMQCSendToMK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_RESULT Results = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        ART_WF_ARTWORK_PROCESS_RESULT processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
                        if (param.data.PROCESS != null)
                        {
                            var MKSubID = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID }, context).FirstOrDefault().CREATOR_ID;
                            var checkMK = CNService.GetPositionCodeUser(MKSubID, context);
                            var IsRoleMK = CNService.IsRoleMK(MKSubID.Value, context);
                            if (checkMK == "MK" || IsRoleMK)
                                param.data.PROCESS.CURRENT_USER_ID = MKSubID;
                            else
                            {
                                ART_WF_ARTWORK_PROCESS_MARKETING_2 _mk2 = new ART_WF_ARTWORK_PROCESS_MARKETING_2();
                                List<ART_WF_ARTWORK_PROCESS_MARKETING_2> _listMK2 = new List<ART_WF_ARTWORK_PROCESS_MARKETING_2>();

                                List<int> _listSubID = CNService.FindArtworkSubId(param.data.ARTWORK_SUB_ID, context);

                                var MKStepID = context.ART_M_STEP_ARTWORK.Where(c => c.STEP_ARTWORK_CODE == "SEND_MK").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
                                var MKProcesses = (from p in context.ART_WF_ARTWORK_PROCESS
                                                   where _listSubID.Contains(p.ARTWORK_SUB_ID)
                                                   && p.CURRENT_STEP_ID == MKStepID
                                                   select p).ToList();
                                if (MKProcesses != null && MKProcesses.Count > 0)
                                {
                                    var MKProcess = MKProcesses.OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();
                                    _mk2.ARTWORK_SUB_ID = MKProcess.ARTWORK_SUB_ID;
                                }

                                _listMK2 = MapperServices.ART_WF_ARTWORK_PROCESS_MARKETING(ART_WF_ARTWORK_PROCESS_MARKETING_SERVICE.GetByItem(_mk2, context).ToList());
                                var _listMK_ = 0;
                                if (_listMK2 != null && _listMK2.Count > 0)
                                {
                                    _listMK_ = _listMK2.OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault().UPDATE_BY;
                                }
                                if (_listMK_ != 0)
                                    param.data.PROCESS.CURRENT_USER_ID = _listMK_;
                            }

                            processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                        }

                        var MKData = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC(param.data);

                        var check = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                        if (check.Count > 0)
                            MKData.ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_ID = check.FirstOrDefault().ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_ID;

                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_SERVICE.SaveOrUpdate(MKData, context);

                        if (param.data.ENDTASKFORM)
                        {
                            ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                        }

                        Results.data = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_2>();
                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_2 item = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_2();
                        List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_2> listItem = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_2>();

                        item.ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_ID = MKData.ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_ID;
                        listItem.Add(item);

                        Results.data = listItem;

                        dbContextTransaction.Commit();

                        foreach (var process in processResults.data)
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

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT SaveGMQCByQC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT Results = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        ART_WF_ARTWORK_PROCESS_RESULT processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
                        if (param.data.PROCESS != null)
                        {
                            if (param.data.ACTION_CODE == "SEND_BACK")
                            {
                                ART_WF_ARTWORK_PROCESS_QC_2 _qc2 = new ART_WF_ARTWORK_PROCESS_QC_2();
                                List<ART_WF_ARTWORK_PROCESS_QC_2> _listQC2 = new List<ART_WF_ARTWORK_PROCESS_QC_2>();

                                List<int> _listSubID = CNService.FindArtworkSubId(param.data.ARTWORK_SUB_ID, context);

                                var QCStepID = context.ART_M_STEP_ARTWORK.Where(c => c.STEP_ARTWORK_CODE == "SEND_QC").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
                                var QCProcesses = (from p in context.ART_WF_ARTWORK_PROCESS
                                                   where _listSubID.Contains(p.ARTWORK_SUB_ID)
                                                   && p.CURRENT_STEP_ID == QCStepID
                                                   select p).ToList();
                                if (QCProcesses != null && QCProcesses.Count > 0)
                                {
                                    var QCProcess = QCProcesses.OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();
                                    _qc2.ARTWORK_SUB_ID = QCProcess.ARTWORK_SUB_ID;
                                }

                                _listQC2 = MapperServices.ART_WF_ARTWORK_PROCESS_QC(ART_WF_ARTWORK_PROCESS_QC_SERVICE.GetByItem(_qc2, context).ToList());
                                var _listQC_ = 0;
                                if (_listQC2 != null && _listQC2.Count > 0)
                                {
                                    _listQC_ = _listQC2.OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault().UPDATE_BY;
                                }

                                if (_listQC_ != 0)
                                    param.data.PROCESS.CURRENT_USER_ID = _listQC_;
                            }

                            processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                        }

                        var MKData = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC(param.data);

                        var check = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                        if (check.Count > 0)
                            MKData.ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID = check.FirstOrDefault().ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID;

                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.SaveOrUpdate(MKData, context);

                        if (param.data.ENDTASKFORM)
                            ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);

                        Results.data = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2>();
                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2 item = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2();
                        List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2> listItem = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2>();

                        item.ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID = MKData.ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID;
                        listItem.Add(item);

                        Results.data = listItem;

                        dbContextTransaction.Commit();

                        foreach (var process in processResults.data)
                            if (param.data.ACTION_CODE == "SEND_BACK")
                                EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_BACK", context, param.data.COMMENT);
                            else
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

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_RESULT PostMultiGMQCSendToMK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_REQUEST_LIST param)
        {
            ART_WF_ARTWORK_PROCESS_RESULT temp = new ART_WF_ARTWORK_PROCESS_RESULT();
            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_RESULT Results = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_RESULT();
            try
            {
                if (param != null && param.data != null)
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (var dbContextTransaction = CNService.IsolationLevel(context))
                        {
                            foreach (ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_2 iData in param.data)
                            {
                                var mkData = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC(iData);

                                var check = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC() { ARTWORK_SUB_ID = iData.ARTWORK_SUB_ID }, context);
                                if (check.Count > 0)
                                    mkData.ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_ID = check.FirstOrDefault().ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_ID;

                                ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_SERVICE.SaveOrUpdate(mkData, context);

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

                                    //send to mk
                                    var step = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == "SEND_MK_VERIFY").FirstOrDefault();
                                    ART_WF_ARTWORK_PROCESS_2 process = new ART_WF_ARTWORK_PROCESS_2();
                                    process.ARTWORK_ITEM_ID = iData.ARTWORK_ITEM_ID;
                                    process.ARTWORK_REQUEST_ID = iData.ARTWORK_REQUEST_ID;
                                    process.CREATE_BY = iData.UPDATE_BY;
                                    process.UPDATE_BY = iData.UPDATE_BY;

                                    var MKSubID = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_ID = iData.ARTWORK_REQUEST_ID }, context).FirstOrDefault().CREATOR_ID;
                                    var checkMK = CNService.GetPositionCodeUser(MKSubID, context);
                                    var IsRoleMK = CNService.IsRoleMK(MKSubID.Value, context);
                                    if (checkMK == "MK" || IsRoleMK)
                                        process.CURRENT_USER_ID = MKSubID;
                                    else
                                    {
                                        ART_WF_ARTWORK_PROCESS_MARKETING_2 mk2 = new ART_WF_ARTWORK_PROCESS_MARKETING_2();
                                        List<ART_WF_ARTWORK_PROCESS_MARKETING_2> listMK2 = new List<ART_WF_ARTWORK_PROCESS_MARKETING_2>();

                                        List<int> listSubID = CNService.FindArtworkSubId(iData.ARTWORK_SUB_ID, context);

                                        var MKStepID = context.ART_M_STEP_ARTWORK.Where(c => c.STEP_ARTWORK_CODE == "SEND_MK").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
                                        var MKProcesses = (from p in context.ART_WF_ARTWORK_PROCESS
                                                           where listSubID.Contains(p.ARTWORK_SUB_ID)
                                                           && p.CURRENT_STEP_ID == MKStepID
                                                           select p).ToList();
                                        if (MKProcesses != null && MKProcesses.Count > 0)
                                        {
                                            var MKProcess = MKProcesses.OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();
                                            mk2.ARTWORK_SUB_ID = MKProcess.ARTWORK_SUB_ID;
                                        }

                                        listMK2 = MapperServices.ART_WF_ARTWORK_PROCESS_MARKETING(ART_WF_ARTWORK_PROCESS_MARKETING_SERVICE.GetByItem(mk2, context).ToList());
                                        var tempMK = 0;
                                        if (listMK2 != null && listMK2.Count > 0)
                                        {
                                            tempMK = listMK2.OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault().UPDATE_BY;
                                        }
                                        if (tempMK != 0)
                                            iData.PROCESS.CURRENT_USER_ID = tempMK;
                                    }

                                    process.CURRENT_ROLE_ID = step.ROLE_ID_RESPONSE;
                                    process.CURRENT_STEP_ID = step.STEP_ARTWORK_ID;
                                    process.PARENT_ARTWORK_SUB_ID = iData.ARTWORK_SUB_ID;

                                    temp = ArtworkProcessHelper.SaveProcess(process, context);
                                }
                            }

                            dbContextTransaction.Commit();

                            foreach (var tempProcess in temp.data)
                                EmailService.sendEmailArtwork(tempProcess.ARTWORK_REQUEST_ID, tempProcess.ARTWORK_SUB_ID, "WF_SEND_TO", context);

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
    }
}
