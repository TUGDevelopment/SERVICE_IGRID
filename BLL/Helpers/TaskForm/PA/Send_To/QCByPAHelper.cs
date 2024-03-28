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
    public class QCByPAHelper
    {
        public static ART_WF_ARTWORK_PROCESS_QC_RESULT GetQCByPA(ART_WF_ARTWORK_PROCESS_QC_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_QC_RESULT Results = new ART_WF_ARTWORK_PROCESS_QC_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_QC(ART_WF_ARTWORK_PROCESS_QC_SERVICE.GetAll(context));
                        }
                        else
                        {
                            var ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            param.data.ARTWORK_SUB_ID = 0;

                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_QC(ART_WF_ARTWORK_PROCESS_QC_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_PROCESS_QC(param.data), context));

                            param.data.ARTWORK_SUB_ID = ARTWORK_SUB_ID;
                            var list_sub = CNService.FindArtworkSubId(ARTWORK_SUB_ID, context);
                            Results.data = Results.data.Where(m => list_sub.Contains(m.ARTWORK_SUB_ID)).ToList();
                        }

                        ART_WF_ARTWORK_PROCESS_QC p = new ART_WF_ARTWORK_PROCESS_QC();
                        var stepRD = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == "SEND_RD").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();

                        Results.status = "S";
                        var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_QC" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var PAstepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;

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
                                    Results.data[i].REMARK_REASON_BY_PA = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = Results.data[i].ARTWORK_SUB_ID, WF_STEP = PAstepId }, context);
                                }
                                if (Results.data[i].REASON_BY_OTHER == "อื่นๆ โปรดระบุ (Others)")
                                {
                                    Results.data[i].REMARK_REASON_BY_OTHER = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = Results.data[i].ARTWORK_SUB_ID, WF_STEP = stepId }, context);
                                }

                                Results.data[i].CREATE_DATE_BY_PA = processFormPA.CREATE_DATE;

                                var formQC = ART_WF_ARTWORK_PROCESS_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_QC() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context);
                                var formPA = ART_WF_ARTWORK_PROCESS_QC_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_QC_BY_PA() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context);

                                var itemID = CNService.FindArtworkItemId(Results.data[i].ARTWORK_SUB_ID, context);
                                var artSubID = Results.data[i].ARTWORK_SUB_ID; //  ticket 462433  added by aof
                                var processRD = (from r in context.ART_WF_ARTWORK_PROCESS
                                                 join q in context.ART_WF_ARTWORK_PROCESS_RD on r.ARTWORK_SUB_ID equals q.ARTWORK_SUB_ID
                                                 where r.ARTWORK_ITEM_ID == itemID
                                                 && r.CURRENT_STEP_ID == stepRD
                                                 && r.PARENT_ARTWORK_SUB_ID == artSubID //  ticket 462433  added by aof
                                                 && q.ACTION_CODE == "SUBMIT"  //  ticket 462433  added by aof
                                                 select r).OrderByDescending(r => r.ARTWORK_SUB_ID).FirstOrDefault();

                                List<ART_WF_ARTWORK_PROCESS_RD> formRD = new List<ART_WF_ARTWORK_PROCESS_RD>();

                                if (processRD != null)
                                {
                                    formRD = ART_WF_ARTWORK_PROCESS_RD_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_RD() { ARTWORK_SUB_ID = processRD.ARTWORK_SUB_ID }, context);
                                }

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

                                    if (formQC.FirstOrDefault() != null)
                                    {
                                        if (formQC.FirstOrDefault().IS_CONFIRED == "1")
                                            Results.data[i].IS_CONFIRED_DISPLAY_TXT = "ยืนยันว่าระบุตามข้างต้นได้ทุกประการ";
                                        else if (formQC.FirstOrDefault().IS_CONFIRED == "0")
                                            Results.data[i].IS_CONFIRED_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";

                                        if (formQC.FirstOrDefault().NUTRITION == "X")
                                            Results.data[i].NUTRITION_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formQC.FirstOrDefault().NUTRITION != "X")
                                            Results.data[i].NUTRITION_QC_DISPLAY_TXT = "-";

                                        if (formQC.FirstOrDefault().HEALTH_CLAIM == "X")
                                            Results.data[i].HEALTH_CLAIM_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formQC.FirstOrDefault().HEALTH_CLAIM != "X")
                                            Results.data[i].HEALTH_CLAIM_QC_DISPLAY_TXT = "-";

                                        if (formQC.FirstOrDefault().CATCHING_AREA == "X")
                                            Results.data[i].CATCHING_AREA_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formQC.FirstOrDefault().CATCHING_AREA != "X")
                                            Results.data[i].CATCHING_AREA_QC_DISPLAY_TXT = "-";

                                        if (formQC.FirstOrDefault().INGREDIENTS == "X")
                                            Results.data[i].INGREDIENTS_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formQC.FirstOrDefault().INGREDIENTS != "X")
                                            Results.data[i].INGREDIENTS_QC_DISPLAY_TXT = "-";

                                        if (formQC.FirstOrDefault().NUTRIENT_CLAIM == "X")
                                            Results.data[i].NUTRIENT_CLAIM_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formQC.FirstOrDefault().NUTRIENT_CLAIM != "X")
                                            Results.data[i].NUTRIENT_CLAIM_QC_DISPLAY_TXT = "-";

                                        if (formQC.FirstOrDefault().ANALYSIS == "X")
                                            Results.data[i].ANALYSIS_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formQC.FirstOrDefault().ANALYSIS != "X")
                                            Results.data[i].ANALYSIS_QC_DISPLAY_TXT = "-";

                                        if (formQC.FirstOrDefault().SPECIES == "X")
                                            Results.data[i].SPECIES_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formQC.FirstOrDefault().SPECIES != "X")
                                            Results.data[i].SPECIES_QC_DISPLAY_TXT = "-";

                                        if (formQC.FirstOrDefault().CHECK_DETAIL == "X")
                                            Results.data[i].CHECK_QC_DETAIL_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                        else if (formQC.FirstOrDefault().CHECK_DETAIL != "X")
                                            Results.data[i].CHECK_QC_DETAIL_DISPLAY_TXT = "-";


                                        //----------------- ticket 462433 by aof on 20210313  start
                                        //Results.data[i].RECOMMENDED_BY_QC_DISPLAY_TXT = formQC.FirstOrDefault().COMMENT;

                                        Results.data[i].DEFALUT_QC_ANALYSIS = formQC.FirstOrDefault().ANALYSIS;
                                        Results.data[i].DEFALUT_QC_ANALYSIS_COMMENT  = formQC.FirstOrDefault().ANALYSIS_COMMENT;
                                        Results.data[i].DEFALUT_QC_CATCHING_AREA  = formQC.FirstOrDefault().CATCHING_AREA;
                                        Results.data[i].DEFALUT_QC_CATCHING_AREA_COMMENT  = formQC.FirstOrDefault().CATCHING_AREA_COMMENT;
                                        Results.data[i].DEFALUT_QC_CHECK_DETAIL  = formQC.FirstOrDefault().CHECK_DETAIL;
                                        Results.data[i].DEFALUT_QC_CHECK_DETAIL_COMMENT = formQC.FirstOrDefault().CHECK_DETAIL_COMMENT;
                                        Results.data[i].DEFALUT_QC_COMMENT  = formQC.FirstOrDefault().COMMENT;
                                        Results.data[i].DEFALUT_QC_HEALTH_CLAIM  = formQC.FirstOrDefault().HEALTH_CLAIM;
                                        Results.data[i].DEFALUT_QC_HEALTH_CLAIM_COMMENT  = formQC.FirstOrDefault().HEALTH_CLAIM_COMMENT;
                                        Results.data[i].DEFALUT_QC_INGREDIENTS  = formQC.FirstOrDefault().INGREDIENTS;
                                        Results.data[i].DEFALUT_QC_INGREDIENTS_COMMENT  = formQC.FirstOrDefault().INGREDIENTS_COMMENT;
                                        Results.data[i].DEFALUT_QC_IS_CONFIRED  = formQC.FirstOrDefault().IS_CONFIRED;
                                        Results.data[i].DEFALUT_QC_NUTRIENT_CLAIM  = formQC.FirstOrDefault().NUTRIENT_CLAIM;
                                        Results.data[i].DEFALUT_QC_NUTRIENT_CLAIM_COMMENT  = formQC.FirstOrDefault().NUTRIENT_CLAIM_COMMENT;
                                        Results.data[i].DEFALUT_QC_NUTRITION = formQC.FirstOrDefault().NUTRITION;
                                        Results.data[i].DEFALUT_QC_NUTRITION_COMMENT  = formQC.FirstOrDefault().NUTRITION_COMMENT;
                                        Results.data[i].DEFALUT_QC_SPECIES  = formQC.FirstOrDefault().SPECIES;
                                        Results.data[i].DEFALUT_QC_SPECIES_COMMENT  = formQC.FirstOrDefault().SPECIES_COMMENT;



                                        if (formRD.FirstOrDefault() != null)
                                        {
                                            if (formRD.FirstOrDefault().UPDATE_DATE > formQC.FirstOrDefault().UPDATE_DATE)
                                            {
                                                // var artworkProcess = context.ART_WF_ARTWORK_PROCESS.Where(w => w.ARTWORK_SUB_ID == formQC.FirstOrDefault().ARTWORK_SUB_ID).FirstOrDefault();
                                                var artwork_sub_id_qc = formQC.FirstOrDefault().ARTWORK_SUB_ID;
                                                var artworkProcess = (from x in context.ART_WF_ARTWORK_PROCESS
                                                                      where x.ARTWORK_SUB_ID == artwork_sub_id_qc
                                                                      select x).OrderByDescending(x => x.ARTWORK_SUB_ID).FirstOrDefault();

                                                if (artworkProcess != null)
                                                {
                                                    if (string.IsNullOrEmpty(artworkProcess.IS_END))
                                                    {

                                                        Results.data[i].DEFALUT_QC_ANALYSIS = formRD.FirstOrDefault().ANALYSIS;
                                                        Results.data[i].DEFALUT_QC_ANALYSIS_COMMENT = formRD.FirstOrDefault().ANALYSIS_COMMENT;
                                                        Results.data[i].DEFALUT_QC_CATCHING_AREA = formRD.FirstOrDefault().CATCHING_AREA;
                                                        Results.data[i].DEFALUT_QC_CATCHING_AREA_COMMENT = formRD.FirstOrDefault().CATCHING_AREA_COMMENT;
                                                        Results.data[i].DEFALUT_QC_CHECK_DETAIL = formRD.FirstOrDefault().CHECK_DETAIL;
                                                        Results.data[i].DEFALUT_QC_CHECK_DETAIL_COMMENT = formRD.FirstOrDefault().CHECK_DETAIL_COMMENT;
                                                        Results.data[i].DEFALUT_QC_COMMENT = formRD.FirstOrDefault().COMMENT;
                                                        Results.data[i].DEFALUT_QC_HEALTH_CLAIM = formRD.FirstOrDefault().HEALTH_CLAIM;
                                                        Results.data[i].DEFALUT_QC_HEALTH_CLAIM_COMMENT = formRD.FirstOrDefault().HEALTH_CLAIM_COMMENT;
                                                        Results.data[i].DEFALUT_QC_INGREDIENTS = formRD.FirstOrDefault().INGREDIENTS;
                                                        Results.data[i].DEFALUT_QC_INGREDIENTS_COMMENT = formRD.FirstOrDefault().INGREDIENTS_COMMENT;
                                                        Results.data[i].DEFALUT_QC_IS_CONFIRED = formRD.FirstOrDefault().IS_CONFIRED;
                                                        Results.data[i].DEFALUT_QC_NUTRIENT_CLAIM = formRD.FirstOrDefault().NUTRIENT_CLAIM;
                                                        Results.data[i].DEFALUT_QC_NUTRIENT_CLAIM_COMMENT = formRD.FirstOrDefault().NUTRIENT_CLAIM_COMMENT;
                                                        Results.data[i].DEFALUT_QC_NUTRITION = formRD.FirstOrDefault().NUTRITION;
                                                        Results.data[i].DEFALUT_QC_NUTRITION_COMMENT = formRD.FirstOrDefault().NUTRITION_COMMENT;
                                                        Results.data[i].DEFALUT_QC_SPECIES = formRD.FirstOrDefault().SPECIES;
                                                        Results.data[i].DEFALUT_QC_SPECIES_COMMENT = formRD.FirstOrDefault().SPECIES_COMMENT;


                                                    }
                                                }
                                            }


                                        }
                                        //----------------- ticket 462433 by aof on 20210313  finish
                                    }



                                    //if (formRD.FirstOrDefault() != null)
                                    //{
                                    //    if (formRD.FirstOrDefault().IS_CONFIRED == "1")
                                    //        Results.data[i].IS_CONFIRED_DISPLAY_TXT = "ยืนยันว่าระบุตามข้างต้นได้ทุกประการ";
                                    //    else if (formRD.FirstOrDefault().IS_CONFIRED == "0")
                                    //        Results.data[i].IS_CONFIRED_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";

                                    //    if (formRD.FirstOrDefault().NUTRITION == "X")
                                    //        Results.data[i].NUTRITION_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    //    else if (formRD.FirstOrDefault().NUTRITION != "X")
                                    //        Results.data[i].NUTRITION_QC_DISPLAY_TXT = "-";

                                    //    if (formRD.FirstOrDefault().HEALTH_CLAIM == "X")
                                    //        Results.data[i].HEALTH_CLAIM_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    //    else if (formRD.FirstOrDefault().HEALTH_CLAIM != "X")
                                    //        Results.data[i].HEALTH_CLAIM_QC_DISPLAY_TXT = "-";

                                    //    if (formRD.FirstOrDefault().CATCHING_AREA == "X")
                                    //        Results.data[i].CATCHING_AREA_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    //    else if (formRD.FirstOrDefault().CATCHING_AREA != "X")
                                    //        Results.data[i].CATCHING_AREA_QC_DISPLAY_TXT = "-";

                                    //    if (formRD.FirstOrDefault().INGREDIENTS == "X")
                                    //        Results.data[i].INGREDIENTS_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    //    else if (formRD.FirstOrDefault().INGREDIENTS != "X")
                                    //        Results.data[i].INGREDIENTS_QC_DISPLAY_TXT = "-";

                                    //    if (formRD.FirstOrDefault().NUTRIENT_CLAIM == "X")
                                    //        Results.data[i].NUTRIENT_CLAIM_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    //    else if (formRD.FirstOrDefault().NUTRIENT_CLAIM != "X")
                                    //        Results.data[i].NUTRIENT_CLAIM_QC_DISPLAY_TXT = "-";

                                    //    if (formRD.FirstOrDefault().ANALYSIS == "X")
                                    //        Results.data[i].ANALYSIS_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    //    else if (formRD.FirstOrDefault().ANALYSIS != "X")
                                    //        Results.data[i].ANALYSIS_QC_DISPLAY_TXT = "-";

                                    //    if (formRD.FirstOrDefault().SPECIES == "X")
                                    //        Results.data[i].SPECIES_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    //    else if (formRD.FirstOrDefault().SPECIES != "X")
                                    //        Results.data[i].SPECIES_QC_DISPLAY_TXT = "-";

                                    //    if (formRD.FirstOrDefault().CHECK_DETAIL == "X")
                                    //        Results.data[i].CHECK_DETAIL_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    //    else if (formRD.FirstOrDefault().CHECK_DETAIL != "X")
                                    //        Results.data[i].CHECK_DETAIL_DISPLAY_TXT = "-";
                                    //}

                                }
                            }
                        }

                        var list = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context), CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var result = list.Where(q => !Results.data.Any(q2 => q2.ARTWORK_SUB_ID == q.ARTWORK_SUB_ID)).FirstOrDefault();
                        if (result != null)
                        {
                            ART_WF_ARTWORK_PROCESS_QC_2 item = new ART_WF_ARTWORK_PROCESS_QC_2();
                            item.CREATE_DATE_BY_PA = result.CREATE_DATE;
                            item.COMMENT_BY_PA = result.REMARK;
                            item.REASON_BY_PA = CNService.getReason(ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(result.ARTWORK_SUB_ID, context).REASON_ID, context);
                            item.REMARK_REASON_BY_PA = "-";
                            if (item.REASON_BY_PA == "อื่นๆ โปรดระบุ (Others)")
                            {
                                item.REMARK_REASON_BY_PA = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = result.ARTWORK_SUB_ID, WF_STEP = PAstepId }, context);
                            }

                            item.ARTWORK_SUB_ID = result.ARTWORK_SUB_ID;
                            item.ARTWORK_REQUEST_ID = result.ARTWORK_REQUEST_ID;

                            var formQC = ART_WF_ARTWORK_PROCESS_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_QC() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context);
                            var formPA = ART_WF_ARTWORK_PROCESS_QC_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_QC_BY_PA() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context);

                            var itemID = CNService.FindArtworkItemId(result.ARTWORK_SUB_ID, context);
                            var artworksubID = item.ARTWORK_SUB_ID; //  ticket 462433  added by aof
                            var processRD = (from r in context.ART_WF_ARTWORK_PROCESS
                                             join q in context.ART_WF_ARTWORK_PROCESS_RD on r.ARTWORK_SUB_ID equals q.ARTWORK_SUB_ID
                                             where r.ARTWORK_ITEM_ID == itemID
                                             && r.CURRENT_STEP_ID == stepRD
                                             && r.PARENT_ARTWORK_SUB_ID == artworksubID //  ticket 462433  added by aof
                                             && q.ACTION_CODE  == "SUBMIT" //  ticket 462433  added by aof
                                             select r).OrderByDescending(r => r.ARTWORK_SUB_ID).FirstOrDefault();

                            List<ART_WF_ARTWORK_PROCESS_RD> formRD = new List<ART_WF_ARTWORK_PROCESS_RD>();

                            if (processRD != null)
                            {
                                formRD = ART_WF_ARTWORK_PROCESS_RD_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_RD() { ARTWORK_SUB_ID = processRD.ARTWORK_SUB_ID }, context);
                            }

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


                                // ticket 462433 by aof on 20210306  start
                                if (formRD.FirstOrDefault() != null)
                                {
                                    //item.RECOMMENDED_BY_QC_DISPLAY_TXT = formRD.FirstOrDefault().COMMENT;
                                    item.DEFALUT_QC_ANALYSIS = formRD.FirstOrDefault().ANALYSIS;
                                    item.DEFALUT_QC_ANALYSIS_COMMENT = formRD.FirstOrDefault().ANALYSIS_COMMENT;
                                    item.DEFALUT_QC_CATCHING_AREA = formRD.FirstOrDefault().CATCHING_AREA;
                                    item.DEFALUT_QC_CATCHING_AREA_COMMENT = formRD.FirstOrDefault().CATCHING_AREA_COMMENT;
                                    item.DEFALUT_QC_CHECK_DETAIL = formRD.FirstOrDefault().CHECK_DETAIL;
                                    item.DEFALUT_QC_CHECK_DETAIL_COMMENT = formRD.FirstOrDefault().CHECK_DETAIL_COMMENT;
                                    item.DEFALUT_QC_COMMENT = formRD.FirstOrDefault().COMMENT;
                                    item.DEFALUT_QC_HEALTH_CLAIM = formRD.FirstOrDefault().HEALTH_CLAIM;
                                    item.DEFALUT_QC_HEALTH_CLAIM_COMMENT = formRD.FirstOrDefault().HEALTH_CLAIM_COMMENT;
                                    item.DEFALUT_QC_INGREDIENTS = formRD.FirstOrDefault().INGREDIENTS;
                                    item.DEFALUT_QC_INGREDIENTS_COMMENT = formRD.FirstOrDefault().INGREDIENTS_COMMENT;
                                    item.DEFALUT_QC_IS_CONFIRED = formRD.FirstOrDefault().IS_CONFIRED;
                                    item.DEFALUT_QC_NUTRIENT_CLAIM = formRD.FirstOrDefault().NUTRIENT_CLAIM;
                                    item.DEFALUT_QC_NUTRIENT_CLAIM_COMMENT = formRD.FirstOrDefault().NUTRIENT_CLAIM_COMMENT;
                                    item.DEFALUT_QC_NUTRITION = formRD.FirstOrDefault().NUTRITION;
                                    item.DEFALUT_QC_NUTRITION_COMMENT = formRD.FirstOrDefault().NUTRITION_COMMENT;
                                    item.DEFALUT_QC_SPECIES = formRD.FirstOrDefault().SPECIES;
                                    item.DEFALUT_QC_SPECIES_COMMENT = formRD.FirstOrDefault().SPECIES_COMMENT;

                                }
                                //ticket 462433 by aof on 20210306  finish

                                if (formQC.FirstOrDefault() != null)
                                {
                                    //ticket 462433 by aof on 20210306 start
                                    //item.COMMENT = formRD.FirstOrDefault().COMMENT;  
                                    item.DEFALUT_QC_ANALYSIS = formQC.FirstOrDefault().ANALYSIS;
                                    item.DEFALUT_QC_ANALYSIS_COMMENT = formQC.FirstOrDefault().ANALYSIS_COMMENT;
                                    item.DEFALUT_QC_CATCHING_AREA = formQC.FirstOrDefault().CATCHING_AREA;
                                    item.DEFALUT_QC_CATCHING_AREA_COMMENT = formQC.FirstOrDefault().CATCHING_AREA_COMMENT;
                                    item.DEFALUT_QC_CHECK_DETAIL = formQC.FirstOrDefault().CHECK_DETAIL;
                                    item.DEFALUT_QC_CHECK_DETAIL_COMMENT = formQC.FirstOrDefault().CHECK_DETAIL_COMMENT;
                                    item.DEFALUT_QC_COMMENT = formQC.FirstOrDefault().COMMENT;
                                    item.DEFALUT_QC_HEALTH_CLAIM = formQC.FirstOrDefault().HEALTH_CLAIM;
                                    item.DEFALUT_QC_HEALTH_CLAIM_COMMENT = formQC.FirstOrDefault().HEALTH_CLAIM_COMMENT;
                                    item.DEFALUT_QC_INGREDIENTS = formQC.FirstOrDefault().INGREDIENTS;
                                    item.DEFALUT_QC_INGREDIENTS_COMMENT = formQC.FirstOrDefault().INGREDIENTS_COMMENT;
                                    item.DEFALUT_QC_IS_CONFIRED = formQC.FirstOrDefault().IS_CONFIRED;
                                    item.DEFALUT_QC_NUTRIENT_CLAIM = formQC.FirstOrDefault().NUTRIENT_CLAIM;
                                    item.DEFALUT_QC_NUTRIENT_CLAIM_COMMENT = formQC.FirstOrDefault().NUTRIENT_CLAIM_COMMENT;
                                    item.DEFALUT_QC_NUTRITION = formQC.FirstOrDefault().NUTRITION;
                                    item.DEFALUT_QC_NUTRITION_COMMENT = formQC.FirstOrDefault().NUTRITION_COMMENT;
                                    item.DEFALUT_QC_SPECIES = formQC.FirstOrDefault().SPECIES;
                                    item.DEFALUT_QC_SPECIES_COMMENT = formQC.FirstOrDefault().SPECIES_COMMENT;
                                    //ticket 462433 by aof on 20210306 finish


                                    if (formQC.FirstOrDefault().IS_CONFIRED == "1")
                                        item.IS_CONFIRED_DISPLAY_TXT = "ยืนยันว่าระบุตามข้างต้นได้ทุกประการ";
                                    else if (formQC.FirstOrDefault().IS_CONFIRED != "1")
                                        item.IS_CONFIRED_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";

                                    if (formQC.FirstOrDefault().NUTRITION == "X")
                                        item.NUTRITION_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    else if (formQC.FirstOrDefault().NUTRITION != "X")
                                        item.NUTRITION_QC_DISPLAY_TXT = "-";

                                    if (formQC.FirstOrDefault().HEALTH_CLAIM == "X")
                                        item.HEALTH_CLAIM_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    else if (formQC.FirstOrDefault().HEALTH_CLAIM != "X")
                                        item.HEALTH_CLAIM_QC_DISPLAY_TXT = "-";

                                    if (formQC.FirstOrDefault().CATCHING_AREA == "X")
                                        item.CATCHING_AREA_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    else if (formQC.FirstOrDefault().CATCHING_AREA != "X")
                                        item.CATCHING_AREA_QC_DISPLAY_TXT = "-";

                                    if (formQC.FirstOrDefault().INGREDIENTS == "X")
                                        item.INGREDIENTS_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    else if (formQC.FirstOrDefault().INGREDIENTS != "X")
                                        item.INGREDIENTS_QC_DISPLAY_TXT = "-";

                                    if (formQC.FirstOrDefault().NUTRIENT_CLAIM == "X")
                                        item.NUTRIENT_CLAIM_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    else if (formQC.FirstOrDefault().NUTRIENT_CLAIM != "X")
                                        item.NUTRIENT_CLAIM_QC_DISPLAY_TXT = "-";

                                    if (formQC.FirstOrDefault().ANALYSIS == "X")
                                        item.ANALYSIS_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    else if (formQC.FirstOrDefault().ANALYSIS != "X")
                                        item.ANALYSIS_QC_DISPLAY_TXT = "-";

                                    if (formQC.FirstOrDefault().SPECIES == "X")
                                        item.SPECIES_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    else if (formQC.FirstOrDefault().SPECIES != "X")
                                        item.SPECIES_QC_DISPLAY_TXT = "-";

                                    if (formQC.FirstOrDefault().CHECK_DETAIL == "X")
                                        item.CHECK_QC_DETAIL_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                    else if (formQC.FirstOrDefault().CHECK_DETAIL != "X")
                                        item.CHECK_QC_DETAIL_DISPLAY_TXT = "-";
                                }

                                //if (formRD.FirstOrDefault() != null)
                                //{
                                //    if (formRD.FirstOrDefault().IS_CONFIRED == "1")
                                //        item.IS_CONFIRED_DISPLAY_TXT = "ยืนยันว่าระบุตามข้างต้นได้ทุกประการ";
                                //    else if (formRD.FirstOrDefault().IS_CONFIRED != "1")
                                //        item.IS_CONFIRED_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";

                                //    if (formRD.FirstOrDefault().NUTRITION == "X")
                                //        item.NUTRITION_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                //    else if (formRD.FirstOrDefault().NUTRITION != "X")
                                //        item.NUTRITION_QC_DISPLAY_TXT = "-";

                                //    if (formRD.FirstOrDefault().HEALTH_CLAIM == "X")
                                //        item.HEALTH_CLAIM_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                //    else if (formRD.FirstOrDefault().HEALTH_CLAIM != "X")
                                //        item.HEALTH_CLAIM_QC_DISPLAY_TXT = "-";

                                //    if (formRD.FirstOrDefault().CATCHING_AREA == "X")
                                //        item.CATCHING_AREA_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                //    else if (formRD.FirstOrDefault().CATCHING_AREA != "X")
                                //        item.CATCHING_AREA_QC_DISPLAY_TXT = "-";

                                //    if (formRD.FirstOrDefault().INGREDIENTS == "X")
                                //        item.INGREDIENTS_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                //    else if (formRD.FirstOrDefault().INGREDIENTS != "X")
                                //        item.INGREDIENTS_QC_DISPLAY_TXT = "-";

                                //    if (formRD.FirstOrDefault().NUTRIENT_CLAIM == "X")
                                //        item.NUTRIENT_CLAIM_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                //    else if (formRD.FirstOrDefault().NUTRIENT_CLAIM != "X")
                                //        item.NUTRIENT_CLAIM_QC_DISPLAY_TXT = "-";

                                //    if (formRD.FirstOrDefault().ANALYSIS == "X")
                                //        item.ANALYSIS_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                //    else if (formRD.FirstOrDefault().ANALYSIS != "X")
                                //        item.ANALYSIS_QC_DISPLAY_TXT = "-";

                                //    if (formRD.FirstOrDefault().SPECIES == "X")
                                //        item.SPECIES_QC_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                //    else if (formRD.FirstOrDefault().SPECIES != "X")
                                //        item.SPECIES_QC_DISPLAY_TXT = "-";

                                //    if (formRD.FirstOrDefault().CHECK_DETAIL == "X")
                                //        item.CHECK_DETAIL_DISPLAY_TXT = "ไม่สามารถระบุตามข้างต้นได้";
                                //    else if (formRD.FirstOrDefault().CHECK_DETAIL != "X")
                                //        item.CHECK_DETAIL_DISPLAY_TXT = "-";
                                //}
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

        public static ART_WF_ARTWORK_PROCESS_QC_BY_PA_RESULT SaveQCByPA(ART_WF_ARTWORK_PROCESS_QC_BY_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_QC_BY_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_QC_BY_PA_RESULT();
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

                        ART_WF_ARTWORK_PROCESS_QC_BY_PA QCData = new ART_WF_ARTWORK_PROCESS_QC_BY_PA();
                        QCData = MapperServices.ART_WF_ARTWORK_PROCESS_QC_BY_PA(param.data);

                        if (processResults != null && processResults.data != null && processResults.data.Count > 0)
                        {
                            QCData.ARTWORK_SUB_ID = processResults.data[0].ARTWORK_SUB_ID;
                        }

                        ART_WF_ARTWORK_PROCESS_QC_BY_PA_SERVICE.SaveOrUpdate(QCData, context);

                        Results.data = new List<ART_WF_ARTWORK_PROCESS_QC_BY_PA_2>();
                        ART_WF_ARTWORK_PROCESS_QC_BY_PA_2 item = new ART_WF_ARTWORK_PROCESS_QC_BY_PA_2();
                        List<ART_WF_ARTWORK_PROCESS_QC_BY_PA_2> listItem = new List<ART_WF_ARTWORK_PROCESS_QC_BY_PA_2>();

                        item.ARTWORK_PROCESS_QC_ID = QCData.ARTWORK_PROCESS_QC_ID;
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

        public static ART_WF_ARTWORK_PROCESS_QC_RESULT SaveQCSendToPA(ART_WF_ARTWORK_PROCESS_QC_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_QC_RESULT Results = new ART_WF_ARTWORK_PROCESS_QC_RESULT();

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
                            var qcData = MapperServices.ART_WF_ARTWORK_PROCESS_QC(param.data);

                            var check = ART_WF_ARTWORK_PROCESS_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_QC() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                            if (check.Count > 0)
                                qcData.ARTWORK_PROCESS_QC_ID = check.FirstOrDefault().ARTWORK_PROCESS_QC_ID;

                            if (param.data.NUTRITION != null && param.data.NUTRITION_COMMENT == "<p><br></p>"
                                || param.data.NUTRIENT_CLAIM != null && param.data.NUTRIENT_CLAIM_COMMENT == "<p><br></p>"
                                || param.data.ANALYSIS != null && param.data.ANALYSIS_COMMENT == "<p><br></p>"
                                || param.data.SPECIES != null && param.data.SPECIES_COMMENT == "<p><br></p>"
                                || param.data.HEALTH_CLAIM != null && param.data.HEALTH_CLAIM_COMMENT == "<p><br></p>"
                                || param.data.CATCHING_AREA != null && param.data.CATCHING_AREA_COMMENT == "<p><br></p>"
                                || param.data.INGREDIENTS != null && param.data.INGREDIENTS_COMMENT == "<p><br></p>"
                                  || param.data.CHECK_DETAIL != null && param.data.CHECK_DETAIL_COMMENT == "<p><br></p>")
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
                                if (param.data.CHECK_DETAIL != null && param.data.CHECK_DETAIL_COMMENT == "<p><br></p>")
                                {
                                    if (Results.msg == "")
                                        Results.msg = "Please fill ตรวจสอบรายละเอียดทุกจุด รวมทั้งรูปภาพ เช่น รูปปลาแต่ละสายพันธุ์ ให้สอดคล้องกับสินค้าและกฎหมายประเทศที่ส่งออกไป comment.";
                                    else
                                        Results.msg += "<br>Please fill cตรวจสอบรายละเอียดทุกจุด รวมทั้งรูปภาพ เช่น รูปปลาแต่ละสายพันธุ์ ให้สอดคล้องกับสินค้าและกฎหมายประเทศที่ส่งออกไป comment.";
                                }

                                return Results;
                            }

                            ART_WF_ARTWORK_PROCESS_QC_SERVICE.SaveOrUpdate(qcData, context);

                            if (param.data.ENDTASKFORM)
                            {
                                ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                            }

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

    }

}
