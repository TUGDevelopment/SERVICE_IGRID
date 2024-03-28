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
    public class RDByQCHelper
    {
        public static ART_WF_ARTWORK_PROCESS_RD_RESULT SaveRDSendToQC(ART_WF_ARTWORK_PROCESS_RD_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_RD_RESULT Results = new ART_WF_ARTWORK_PROCESS_RD_RESULT();

            if (param == null || param.data == null)
            {
                return Results;
            }
            else
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (var dbContextTransaction = CNService.IsolationLevel(context))
                        {
                            var rdData = MapperServices.ART_WF_ARTWORK_PROCESS_RD(param.data);

                            var check = ART_WF_ARTWORK_PROCESS_RD_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_RD() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                            if (check.Count > 0)
                                rdData.ARTWORK_PROCESS_RD_ID = check.FirstOrDefault().ARTWORK_PROCESS_RD_ID;

                            if (param.data.NUTRITION != null && param.data.NUTRITION_COMMENT == "<p><br></p>"
                                || param.data.NUTRIENT_CLAIM != null && param.data.NUTRIENT_CLAIM_COMMENT == "<p><br></p>"
                                || param.data.ANALYSIS != null && param.data.ANALYSIS_COMMENT == "<p><br></p>"
                                || param.data.SPECIES != null && param.data.SPECIES_COMMENT == "<p><br></p>"
                                || param.data.HEALTH_CLAIM != null && param.data.HEALTH_CLAIM_COMMENT == "<p><br></p>"
                                || param.data.CATCHING_AREA != null && param.data.CATCHING_AREA_COMMENT == "<p><br></p>"
                                || param.data.INGREDIENTS != null && param.data.INGREDIENTS_COMMENT == "<p><br></p>")
                            {
                                Results.status = "E";
                                if (param.data.NUTRITION != null && param.data.NUTRITION_COMMENT == "<p><br></p>")
                                {
                                    if (Results.msg == "")
                                        Results.msg = "Please fill nutrition comment.";
                                    else
                                        Results.msg += "<br>Please fill nutrition comment.";
                                }
                                if (param.data.INGREDIENTS != null && param.data.INGREDIENTS_COMMENT == "<p><br></p>")
                                {
                                    if (Results.msg == "")
                                        Results.msg = "Please fill ingredients comment.";
                                    else
                                        Results.msg += "<br>Please fill ingredients comment.";
                                }
                                if (param.data.ANALYSIS != null && param.data.ANALYSIS_COMMENT == "<p><br></p>")
                                {
                                    if (Results.msg == "")
                                        Results.msg = "Please fill analysis comment.";
                                    else
                                        Results.msg += "<br>Please fill analysis comment.";
                                }
                                if (param.data.HEALTH_CLAIM != null && param.data.HEALTH_CLAIM_COMMENT == "<p><br></p>")
                                {
                                    if (Results.msg == "")
                                        Results.msg = "Please fill health claim comment.";
                                    else
                                        Results.msg += "<br>Please fill health claim comment.";
                                }
                                if (param.data.NUTRIENT_CLAIM != null && param.data.NUTRIENT_CLAIM_COMMENT == "<p><br></p>")
                                {
                                    if (Results.msg == "")
                                        Results.msg = "Please fill nutrient claim comment.";
                                    else
                                        Results.msg += "<br>Please fill nutrient claim comment.";
                                }
                                if (param.data.SPECIES != null && param.data.SPECIES_COMMENT == "<p><br></p>")
                                {
                                    if (Results.msg == "")
                                        Results.msg = "Please fill species comment.";
                                    else
                                        Results.msg += "<br>Please fill species comment.";
                                }
                                if (param.data.CATCHING_AREA != null && param.data.CATCHING_AREA_COMMENT == "<p><br></p>")
                                {
                                    if (Results.msg == "")
                                        Results.msg = "Please fill catching area/fao number comment.";
                                    else
                                        Results.msg += "<br>Please fill catching area/fao number comment.";
                                }

                                return Results;
                            }

                            ART_WF_ARTWORK_PROCESS_RD_SERVICE.SaveOrUpdate(rdData, context);

                            if (param.data.ENDTASKFORM)
                                ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);

                            dbContextTransaction.Commit();

                            if (param.data.ACTION_CODE == "SEND_BACK")
                                EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_SEND_BACK", context, param.data.COMMENT);
                            else if (param.data.ACTION_CODE == "SAVE")
                                EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_OTHER_SAVE", context);
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
            }

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_RD_BY_QC_RESULT SaveRDByQC(ART_WF_ARTWORK_PROCESS_RD_BY_QC_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_RD_BY_QC_RESULT Results = new ART_WF_ARTWORK_PROCESS_RD_BY_QC_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        string msg = ArtworkProcessHelper.checkDupWF(param.data.PROCESS, context);
                        if (msg != "")
                        {
                            Results.status = "E";
                            Results.msg = msg;
                            return Results;
                        }

                        ART_WF_ARTWORK_PROCESS_RESULT processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
                        if (param.data.PROCESS != null)
                        {
                            processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                        }

                        ART_WF_ARTWORK_PROCESS_RD_BY_QC QCData = new ART_WF_ARTWORK_PROCESS_RD_BY_QC();
                        QCData = MapperServices.ART_WF_ARTWORK_PROCESS_RD_BY_QC(param.data);

                        if (processResults != null && processResults.data != null && processResults.data.Count > 0)
                        {
                            QCData.ARTWORK_SUB_ID = processResults.data[0].ARTWORK_SUB_ID;
                        }

                        ART_WF_ARTWORK_PROCESS_RD_BY_QC_SERVICE.SaveOrUpdate(QCData, context);

                        Results.data = new List<ART_WF_ARTWORK_PROCESS_RD_BY_QC_2>();
                        ART_WF_ARTWORK_PROCESS_RD_BY_QC_2 item = new ART_WF_ARTWORK_PROCESS_RD_BY_QC_2();
                        List<ART_WF_ARTWORK_PROCESS_RD_BY_QC_2> listItem = new List<ART_WF_ARTWORK_PROCESS_RD_BY_QC_2>();

                        item.ARTWORK_PROCESS_RD_ID = QCData.ARTWORK_PROCESS_RD_ID;
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

        public static ART_WF_ARTWORK_PROCESS_RD_RESULT GetRDByQC(ART_WF_ARTWORK_PROCESS_RD_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_RD_RESULT Results = new ART_WF_ARTWORK_PROCESS_RD_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_RD(ART_WF_ARTWORK_PROCESS_RD_SERVICE.GetAll(context));
                        }
                        else
                        {
                            var ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            param.data.ARTWORK_SUB_ID = 0;

                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_RD(ART_WF_ARTWORK_PROCESS_RD_SERVICE.GetByItemContain(MapperServices.ART_WF_ARTWORK_PROCESS_RD(param.data), context));

                            param.data.ARTWORK_SUB_ID = ARTWORK_SUB_ID;
                            Results.data = Results.data.Where(m => CNService.FindArtworkSubId(ARTWORK_SUB_ID, context).Contains(m.ARTWORK_SUB_ID)).ToList();
                        }

                        ART_WF_ARTWORK_PROCESS_RD p = new ART_WF_ARTWORK_PROCESS_RD();

                        Results.status = "S";
                        var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_RD" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var QCstepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_QC" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                var reason_pa = CNService.getReason(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context).REASON_ID, context);
                                ART_SYS_ACTION act = new ART_SYS_ACTION();
                                act.ACTION_CODE = Results.data[i].ACTION_CODE;
                                Results.data[i].ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItem(act, context).FirstOrDefault().ACTION_NAME;

                                var processFormPA = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(Results.data[i].ARTWORK_SUB_ID, context);

                                Results.data[i].COMMENT_BY_PA = processFormPA.REMARK;
                                Results.data[i].REASON_BY_PA = reason_pa;
                                Results.data[i].REMARK_REASON_BY_PA = "-";
                                Results.data[i].REMARK_REASON_BY_OTHER = "-";
                                Results.data[i].REASON_BY_OTHER = CNService.getReason(Results.data[i].REASON_ID, context);
                                if (reason_pa == "อื่นๆ โปรดระบุ (Others)")
                                {
                                    Results.data[i].REMARK_REASON_BY_PA = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = Results.data[i].ARTWORK_SUB_ID, WF_STEP = QCstepId }, context);
                                }
                                if (Results.data[i].REASON_BY_OTHER == "อื่นๆ โปรดระบุ (Others)")
                                {
                                    Results.data[i].REMARK_REASON_BY_OTHER = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = Results.data[i].ARTWORK_SUB_ID, WF_STEP = stepId }, context);
                                }

                                Results.data[i].CREATE_DATE_BY_PA = processFormPA.CREATE_DATE;
                                Results.data[i].PARENT_RD_ID = Convert.ToInt32(processFormPA.PARENT_ARTWORK_SUB_ID);


                                //var itemRD = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = processFormPA.ARTWORK_ITEM_ID, CURRENT_STEP_ID = stepId, IS_END = "X", REMARK_KILLPROCESS = null }, context);
                                //if (itemRD.Count > 0)
                                //{
                                //    var subRD = itemRD.OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault().ARTWORK_SUB_ID;
                                //    var formRDtoQC = ART_WF_ARTWORK_PROCESS_RD_BY_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_RD_BY_QC() { ARTWORK_SUB_ID = subRD }, context).FirstOrDefault();
                                //    if (formRDtoQC != null)
                                //    {
                                //        Results.data[i].ARTWORK_SUB_ID_RD = formRDtoQC.ARTWORK_SUB_ID;
                                //    }
                                //}

                                var itemRD = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = processFormPA.ARTWORK_ITEM_ID, CURRENT_STEP_ID = stepId, IS_END = "X", REMARK_KILLPROCESS = null }, context);
                                if (itemRD.Count > 0)
                                {
                                    var SortRD = itemRD.OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();
                                    var subRD = SortRD.ARTWORK_SUB_ID;
                                    var parentRD = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = (int)SortRD.PARENT_ARTWORK_SUB_ID, CURRENT_STEP_ID = QCstepId, REMARK_KILLPROCESS = null }, context);
                                    if (parentRD.Count > 0)
                                    {
                                        var hasQCComment = ART_WF_ARTWORK_PROCESS_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_QC() { ARTWORK_SUB_ID = parentRD.FirstOrDefault().ARTWORK_SUB_ID }, context);
                                        if (hasQCComment.Count == 0)
                                        {
                                            var formRDtoQC = ART_WF_ARTWORK_PROCESS_RD_BY_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_RD_BY_QC() { ARTWORK_SUB_ID = subRD }, context).FirstOrDefault();
                                            if (formRDtoQC != null)
                                            {
                                                Results.data[i].ARTWORK_SUB_ID_RD = formRDtoQC.ARTWORK_SUB_ID;
                                            }
                                        }
                                    }
                                }

                                var formRD = ART_WF_ARTWORK_PROCESS_RD_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_RD() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context);
                                var formPA = ART_WF_ARTWORK_PROCESS_RD_BY_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_RD_BY_QC() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context);
                                if (formPA != null)
                                {
                                    if (formPA.FirstOrDefault().NUTRITION == "X")
                                        Results.data[i].NUTRITION_DISPLAY_TXT = "Yes";
                                    else if (formPA.FirstOrDefault().NUTRITION != "X")
                                        Results.data[i].NUTRITION_DISPLAY_TXT = "No";

                                    if (formPA.FirstOrDefault().HEALTH_CLAIM == "X")
                                        Results.data[i].HEALTH_CLAIM_DISPLAY_TXT = "Yes";
                                    else if (formPA.FirstOrDefault().HEALTH_CLAIM != "X")
                                        Results.data[i].HEALTH_CLAIM_DISPLAY_TXT = "No";

                                    if (formPA.FirstOrDefault().CATCHING_AREA == "X")
                                        Results.data[i].CATCHING_AREA_DISPLAY_TXT = "Yes";
                                    else if (formPA.FirstOrDefault().CATCHING_AREA != "X")
                                        Results.data[i].CATCHING_AREA_DISPLAY_TXT = "No";

                                    if (formPA.FirstOrDefault().INGREDIENTS == "X")
                                        Results.data[i].INGREDIENTS_DISPLAY_TXT = "Yes";
                                    else if (formPA.FirstOrDefault().INGREDIENTS != "X")
                                        Results.data[i].INGREDIENTS_DISPLAY_TXT = "No";

                                    if (formPA.FirstOrDefault().NUTRIENT_CLAIM == "X")
                                        Results.data[i].NUTRIENT_CLAIM_DISPLAY_TXT = "Yes";
                                    else if (formPA.FirstOrDefault().NUTRIENT_CLAIM != "X")
                                        Results.data[i].NUTRIENT_CLAIM_DISPLAY_TXT = "No";

                                    if (formPA.FirstOrDefault().ANALYSIS == "X")
                                        Results.data[i].ANALYSIS_DISPLAY_TXT = "Yes";
                                    else if (formPA.FirstOrDefault().ANALYSIS != "X")
                                        Results.data[i].ANALYSIS_DISPLAY_TXT = "No";

                                    if (formPA.FirstOrDefault().SPECIES == "X")
                                        Results.data[i].SPECIES_DISPLAY_TXT = "Yes";
                                    else if (formPA.FirstOrDefault().SPECIES != "X")
                                        Results.data[i].SPECIES_DISPLAY_TXT = "No";

                                    if (formPA.FirstOrDefault().CHECK_DETAIL == "X")
                                        Results.data[i].CHECK_DETAIL_DISPLAY_TXT = "Yes";
                                    else if (formPA.FirstOrDefault().CHECK_DETAIL != "X")
                                        Results.data[i].CHECK_DETAIL_DISPLAY_TXT = "No";

                                    if (formRD.FirstOrDefault() != null)
                                    {
                                        if (formRD.FirstOrDefault().IS_CONFIRED == "1")
                                            Results.data[i].IS_CONFIRED_DISPLAY_TXT = "ยืนยันว่าระบุตามข้างต้นได้ทุกประการ";
                                        else if (formRD.FirstOrDefault().IS_CONFIRED == "0")
                                            Results.data[i].IS_CONFIRED_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";

                                        if (formRD.FirstOrDefault().ACTION_CODE != "SEND_BACK")
                                        {
                                            if (formRD.FirstOrDefault().NUTRITION == "X")
                                                Results.data[i].NUTRITION_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                            else if (formRD.FirstOrDefault().NUTRITION != "X")
                                                Results.data[i].NUTRITION_RD_DISPLAY_TXT = "-";

                                            if (formRD.FirstOrDefault().HEALTH_CLAIM == "X")
                                                Results.data[i].HEALTH_CLAIM_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                            else if (formRD.FirstOrDefault().HEALTH_CLAIM != "X")
                                                Results.data[i].HEALTH_CLAIM_RD_DISPLAY_TXT = "-";

                                            if (formRD.FirstOrDefault().CATCHING_AREA == "X")
                                                Results.data[i].CATCHING_AREA_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                            else if (formRD.FirstOrDefault().CATCHING_AREA != "X")
                                                Results.data[i].CATCHING_AREA_RD_DISPLAY_TXT = "-";

                                            if (formRD.FirstOrDefault().INGREDIENTS == "X")
                                                Results.data[i].INGREDIENTS_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                            else if (formRD.FirstOrDefault().INGREDIENTS != "X")
                                                Results.data[i].INGREDIENTS_RD_DISPLAY_TXT = "-";

                                            if (formRD.FirstOrDefault().NUTRIENT_CLAIM == "X")
                                                Results.data[i].NUTRIENT_CLAIM_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                            else if (formRD.FirstOrDefault().NUTRIENT_CLAIM != "X")
                                                Results.data[i].NUTRIENT_CLAIM_RD_DISPLAY_TXT = "-";

                                            if (formRD.FirstOrDefault().ANALYSIS == "X")
                                                Results.data[i].ANALYSIS_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                            else if (formRD.FirstOrDefault().ANALYSIS != "X")
                                                Results.data[i].ANALYSIS_RD_DISPLAY_TXT = "-";

                                            if (formRD.FirstOrDefault().SPECIES == "X")
                                                Results.data[i].SPECIES_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                            else if (formRD.FirstOrDefault().SPECIES != "X")
                                                Results.data[i].SPECIES_RD_DISPLAY_TXT = "-";

                                            if (formRD.FirstOrDefault().CHECK_DETAIL == "X")
                                                Results.data[i].CHECK_DETAIL_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                            else if (formRD.FirstOrDefault().CHECK_DETAIL != "X")
                                                Results.data[i].CHECK_DETAIL_RD_DISPLAY_TXT = "-";

                                        }
                                    }

                                }
                            }
                        }

                        var list = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context), CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var result = list.Where(q => !Results.data.Any(q2 => q2.ARTWORK_SUB_ID == q.ARTWORK_SUB_ID)).OrderByDescending(o=>o.CREATE_DATE).FirstOrDefault();   //#INC-126736 by aof
                        if (result != null)
                        {
                            ART_WF_ARTWORK_PROCESS_RD_2 item = new ART_WF_ARTWORK_PROCESS_RD_2();
                            item.CREATE_DATE_BY_PA = result.CREATE_DATE;
                            item.COMMENT_BY_PA = result.REMARK;
                            item.REASON_BY_PA = CNService.getReason(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(result.ARTWORK_SUB_ID, context).REASON_ID, context);
                            item.REMARK_REASON_BY_PA = "-";
                            if (item.REASON_BY_PA == "อื่นๆ โปรดระบุ (Others)")
                            {
                                item.REMARK_REASON_BY_PA = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = result.ARTWORK_SUB_ID, WF_STEP = QCstepId }, context);
                            }
                            item.ARTWORK_SUB_ID = result.ARTWORK_SUB_ID;
                            item.ARTWORK_REQUEST_ID = result.ARTWORK_REQUEST_ID;
                            item.PARENT_RD_ID = Convert.ToInt32(result.PARENT_ARTWORK_SUB_ID);

                            //var formRDtoQC = ART_WF_ARTWORK_PROCESS_RD_BY_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_RD_BY_QC() { ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID }, context).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();
                            //if (formRDtoQC != null)
                            //{
                            //    item.ARTWORK_SUB_ID_RD = formRDtoQC.ARTWORK_SUB_ID;
                            //}

                            var formRD = ART_WF_ARTWORK_PROCESS_RD_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_RD() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context);
                            var formPA = ART_WF_ARTWORK_PROCESS_RD_BY_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_RD_BY_QC() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context);
                            if (formPA != null)
                            {
                                if (formPA.FirstOrDefault().NUTRITION == "X")
                                    item.NUTRITION_DISPLAY_TXT = "Yes";
                                else if (formPA.FirstOrDefault().NUTRITION != "X")
                                    item.NUTRITION_DISPLAY_TXT = "No";

                                if (formPA.FirstOrDefault().HEALTH_CLAIM == "X")
                                    item.HEALTH_CLAIM_DISPLAY_TXT = "Yes";
                                else if (formPA.FirstOrDefault().HEALTH_CLAIM != "X")
                                    item.HEALTH_CLAIM_DISPLAY_TXT = "No";

                                if (formPA.FirstOrDefault().CATCHING_AREA == "X")
                                    item.CATCHING_AREA_DISPLAY_TXT = "Yes";
                                else if (formPA.FirstOrDefault().CATCHING_AREA != "X")
                                    item.CATCHING_AREA_DISPLAY_TXT = "No";

                                if (formPA.FirstOrDefault().INGREDIENTS == "X")
                                    item.INGREDIENTS_DISPLAY_TXT = "Yes";
                                else if (formPA.FirstOrDefault().INGREDIENTS != "X")
                                    item.INGREDIENTS_DISPLAY_TXT = "No";

                                if (formPA.FirstOrDefault().NUTRIENT_CLAIM == "X")
                                    item.NUTRIENT_CLAIM_DISPLAY_TXT = "Yes";
                                else if (formPA.FirstOrDefault().NUTRIENT_CLAIM != "X")
                                    item.NUTRIENT_CLAIM_DISPLAY_TXT = "No";

                                if (formPA.FirstOrDefault().ANALYSIS == "X")
                                    item.ANALYSIS_DISPLAY_TXT = "Yes";
                                else if (formPA.FirstOrDefault().ANALYSIS != "X")
                                    item.ANALYSIS_DISPLAY_TXT = "No";

                                if (formPA.FirstOrDefault().SPECIES == "X")
                                    item.SPECIES_DISPLAY_TXT = "Yes";
                                else if (formPA.FirstOrDefault().SPECIES != "X")
                                    item.SPECIES_DISPLAY_TXT = "No";

                                if (formPA.FirstOrDefault().CHECK_DETAIL == "X")
                                    item.CHECK_DETAIL_DISPLAY_TXT = "Yes";
                                else if (formPA.FirstOrDefault().CHECK_DETAIL != "X")
                                    item.CHECK_DETAIL_DISPLAY_TXT = "No";

                                if (formRD.FirstOrDefault() != null)
                                {
                                    if (formRD.FirstOrDefault().IS_CONFIRED == "1")
                                        item.IS_CONFIRED_DISPLAY_TXT = "ยืนยันว่าระบุตามข้างต้นได้ทุกประการ";
                                    else if (formRD.FirstOrDefault().IS_CONFIRED != "1")
                                        item.IS_CONFIRED_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";

                                    if (formRD.FirstOrDefault().ACTION_CODE != "SEND_BACK")
                                    {
                                        if (formRD.FirstOrDefault().NUTRITION == "X")
                                            item.NUTRITION_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formRD.FirstOrDefault().NUTRITION != "X")
                                            item.NUTRITION_RD_DISPLAY_TXT = "-";

                                        if (formRD.FirstOrDefault().HEALTH_CLAIM == "X")
                                            item.HEALTH_CLAIM_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formRD.FirstOrDefault().HEALTH_CLAIM != "X")
                                            item.HEALTH_CLAIM_RD_DISPLAY_TXT = "-";

                                        if (formRD.FirstOrDefault().CATCHING_AREA == "X")
                                            item.CATCHING_AREA_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formRD.FirstOrDefault().CATCHING_AREA != "X")
                                            item.CATCHING_AREA_RD_DISPLAY_TXT = "-";

                                        if (formRD.FirstOrDefault().INGREDIENTS == "X")
                                            item.INGREDIENTS_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formRD.FirstOrDefault().INGREDIENTS != "X")
                                            item.INGREDIENTS_RD_DISPLAY_TXT = "-";

                                        if (formRD.FirstOrDefault().NUTRIENT_CLAIM == "X")
                                            item.NUTRIENT_CLAIM_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formRD.FirstOrDefault().NUTRIENT_CLAIM != "X")
                                            item.NUTRIENT_CLAIM_RD_DISPLAY_TXT = "-";

                                        if (formRD.FirstOrDefault().ANALYSIS == "X")
                                            item.ANALYSIS_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formRD.FirstOrDefault().ANALYSIS != "X")
                                            item.ANALYSIS_RD_DISPLAY_TXT = "-";

                                        if (formRD.FirstOrDefault().SPECIES == "X")
                                            item.SPECIES_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formRD.FirstOrDefault().SPECIES != "X")
                                            item.SPECIES_RD_DISPLAY_TXT = "-";

                                        if (formRD.FirstOrDefault().CHECK_DETAIL == "X")
                                            item.CHECK_DETAIL_RD_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formRD.FirstOrDefault().CHECK_DETAIL != "X")
                                            item.CHECK_DETAIL_RD_DISPLAY_TXT = "-";
                                    }
                                }

                            }

                            Results.data.Add(item);
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
