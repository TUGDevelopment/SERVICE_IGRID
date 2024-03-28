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
    public class WarehouseByPAHelper
    {
        public static ART_WF_ARTWORK_PROCESS_WH_RESULT GetWarehouseByPA(ART_WF_ARTWORK_PROCESS_WH_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_WH_RESULT Results = new ART_WF_ARTWORK_PROCESS_WH_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_WH(ART_WF_ARTWORK_PROCESS_WH_SERVICE.GetAll(context));
                        }
                        else
                        {
                            var ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            param.data.ARTWORK_SUB_ID = 0;

                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_WH(ART_WF_ARTWORK_PROCESS_WH_SERVICE.GetByItemContain(MapperServices.ART_WF_ARTWORK_PROCESS_WH(param.data), context));

                            param.data.ARTWORK_SUB_ID = ARTWORK_SUB_ID;
                            Results.data = Results.data.Where(m => CNService.FindArtworkSubId(ARTWORK_SUB_ID, context).Contains(m.ARTWORK_SUB_ID)).ToList();
                        }

                        ART_WF_ARTWORK_PROCESS_WH p = new ART_WF_ARTWORK_PROCESS_WH();

                        Results.status = "S";
                        var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_WH" }, context).FirstOrDefault().STEP_ARTWORK_ID;
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

                                var formPA = ART_WF_ARTWORK_PROCESS_WH_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_WH_BY_PA() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context);
                                if (formPA != null)
                                {
                                    if (formPA.FirstOrDefault().INKJET_STAMP_AREA.ToUpper().Trim() == "X")
                                        Results.data[i].INKJET_STAMP_AREA_DISPLAY_TXT = "Yes";
                                    else if (formPA.FirstOrDefault().INKJET_STAMP_AREA.ToUpper().Trim() != "X")
                                        Results.data[i].INKJET_STAMP_AREA_DISPLAY_TXT = "No";

                                    if (formPA.FirstOrDefault().ROLL_DIRECTION.ToUpper().Trim() == "X")
                                        Results.data[i].ROLL_DIRECTION_DISPLAY_TXT = "Yes";
                                    else if (formPA.FirstOrDefault().ROLL_DIRECTION.ToUpper().Trim() != "X")
                                        Results.data[i].ROLL_DIRECTION_DISPLAY_TXT = "No";

                                }
                            }
                        }

                        var list = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = CNService.FindArtworkItemId(param.data.ARTWORK_SUB_ID, context), CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var result = list.Where(q => !Results.data.Any(q2 => q2.ARTWORK_SUB_ID == q.ARTWORK_SUB_ID)).FirstOrDefault();
                        if (result != null)
                        {
                            ART_WF_ARTWORK_PROCESS_WH_2 item = new ART_WF_ARTWORK_PROCESS_WH_2();
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
                            var formPA = ART_WF_ARTWORK_PROCESS_WH_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_WH_BY_PA() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context);
                            if (formPA != null)
                            {
                                if (formPA.FirstOrDefault().INKJET_STAMP_AREA == "X")
                                    item.INKJET_STAMP_AREA_DISPLAY_TXT = "Yes";
                                else if (formPA.FirstOrDefault().INKJET_STAMP_AREA != "X")
                                    item.INKJET_STAMP_AREA_DISPLAY_TXT = "No";

                                if (formPA.FirstOrDefault().ROLL_DIRECTION == "X")
                                    item.ROLL_DIRECTION_DISPLAY_TXT = "Yes";
                                else if (formPA.FirstOrDefault().ROLL_DIRECTION != "X")
                                    item.ROLL_DIRECTION_DISPLAY_TXT = "No";
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

        public static ART_WF_ARTWORK_PROCESS_WH_BY_PA_RESULT SaveWarehouseByPA(ART_WF_ARTWORK_PROCESS_WH_BY_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_WH_BY_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_WH_BY_PA_RESULT();
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

                        ART_WF_ARTWORK_PROCESS_WH_BY_PA warehouseData = new ART_WF_ARTWORK_PROCESS_WH_BY_PA();
                        warehouseData = MapperServices.ART_WF_ARTWORK_PROCESS_WH_BY_PA(param.data);

                        if (processResults != null && processResults.data != null && processResults.data.Count > 0)
                        {
                            warehouseData.ARTWORK_SUB_ID = processResults.data[0].ARTWORK_SUB_ID;
                        }

                        ART_WF_ARTWORK_PROCESS_WH_BY_PA_SERVICE.SaveOrUpdate(warehouseData, context);

                        Results.data = new List<ART_WF_ARTWORK_PROCESS_WH_BY_PA_2>();
                        ART_WF_ARTWORK_PROCESS_WH_BY_PA_2 item = new ART_WF_ARTWORK_PROCESS_WH_BY_PA_2();
                        List<ART_WF_ARTWORK_PROCESS_WH_BY_PA_2> listItem = new List<ART_WF_ARTWORK_PROCESS_WH_BY_PA_2>();

                        item.ARTWORK_PROCESS_WH_ID = warehouseData.ARTWORK_PROCESS_WH_ID;
                        listItem.Add(item);

                        Results.data = listItem;

                        dbContextTransaction.Commit();

                        foreach (var process in processResults.data)
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

        public static ART_WF_ARTWORK_PROCESS_WH_RESULT SaveWarehouse(ART_WF_ARTWORK_PROCESS_WH_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_WH_RESULT Results = new ART_WF_ARTWORK_PROCESS_WH_RESULT();

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
                            var whData = MapperServices.ART_WF_ARTWORK_PROCESS_WH(param.data);

                            var check = ART_WF_ARTWORK_PROCESS_WH_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_WH() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                            if (check.Count > 0)
                                whData.ARTWORK_PROCESS_WH_ID = check.FirstOrDefault().ARTWORK_PROCESS_WH_ID;

                            ART_WF_ARTWORK_PROCESS_WH_SERVICE.SaveOrUpdate(whData, context);

                            if (param.data.ENDTASKFORM)
                                ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);

                            dbContextTransaction.Commit();

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
            }

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_WH_RESULT PostWHSendToPA(ART_WF_ARTWORK_PROCESS_WH_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_WH_RESULT Results = new ART_WF_ARTWORK_PROCESS_WH_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var whData = MapperServices.ART_WF_ARTWORK_PROCESS_WH(param.data);

                        var check = ART_WF_ARTWORK_PROCESS_WH_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_WH() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                        if (check.Count > 0)
                            whData.ARTWORK_PROCESS_WH_ID = check.FirstOrDefault().ARTWORK_PROCESS_WH_ID;

                        ART_WF_ARTWORK_PROCESS_WH_SERVICE.SaveOrUpdate(whData, context);

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
    }



}
