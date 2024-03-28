using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services;
using DAL;
using DAL.Model;

namespace BLL.Helpers
{
    public static class WarehouseHelper
    {
        public static ART_WF_MOCKUP_PROCESS_WAREHOUSE_RESULT GetWarehouseForm(ART_WF_MOCKUP_PROCESS_WAREHOUSE_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_WAREHOUSE_RESULT Results = new ART_WF_MOCKUP_PROCESS_WAREHOUSE_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_WAREHOUSE(ART_WF_MOCKUP_PROCESS_WAREHOUSE_SERVICE.GetAll(context));
                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_WAREHOUSE(ART_WF_MOCKUP_PROCESS_WAREHOUSE_SERVICE.GetByItem(MapperServices.ART_WF_MOCKUP_PROCESS_WAREHOUSE(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                ART_SYS_ACTION act = new ART_SYS_ACTION();
                                act.ACTION_CODE = Results.data[i].ACTION_CODE;
                                Results.data[i].ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItem(act, context).FirstOrDefault().ACTION_NAME;

                                var processFormPG = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context);

                                Results.data[i].COMMENT_BY_PG = processFormPG.REMARK;
                                Results.data[i].REASON_BY_PG = CNService.getReason(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).REASON_ID, context);
                                Results.data[i].REASON_BY_OTHER = CNService.getReason(Results.data[i].REASON_ID, context);
                                Results.data[i].CREATE_DATE_BY_PG = processFormPG.CREATE_DATE;
                                Results.data[i].REMARK_REASON_BY_PG = CNService.getRemarkReason(Results.data[i].MOCKUP_SUB_ID, "M", "SEND_PG", context);
                                Results.data[i].REMARK_REASON = CNService.getRemarkReason(Results.data[i].MOCKUP_SUB_ID, "M", "SEND_WH_TEST_PACK", context);

                                var formPG = ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG() { MOCKUP_SUB_ID = Results.data[i].MOCKUP_SUB_ID }, context);
                                if (formPG != null)
                                {
                                    Results.data[i].TEST_PACK_SIZING = formPG.FirstOrDefault().TEST_PACK_SIZING;
                                    if (formPG.FirstOrDefault().TEST_PACK_SIZING == "X")
                                        Results.data[i].TEST_PACK_SIZING_DISPLAY_TXT = "Yes";
                                    Results.data[i].TEST_PACK_HARD_EASY = formPG.FirstOrDefault().TEST_PACK_HARD_EASY;
                                    if (formPG.FirstOrDefault().TEST_PACK_HARD_EASY == "X")
                                        Results.data[i].TEST_PACK_HARD_EASY_DISPLAY_TXT = "Yes";
                                    Results.data[i].SUPPLIER_PRIMARY_CONTAINER = formPG.FirstOrDefault().SUPPLIER_PRIMARY_CONTAINER;
                                    Results.data[i].SUPPLIER_PRIMARY_LID = formPG.FirstOrDefault().SUPPLIER_PRIMARY_LID;
                                    Results.data[i].SHIP_TO_FACTORY = formPG.FirstOrDefault().SHIP_TO_FACTORY;
                                }

                                if (Results.data[i].RECEIVE_PHYSICAL == "X")
                                {
                                    Results.data[i].RECEIVE_PHYSICAL_DISPLAY_TXT = "Received";
                                }
                                else
                                {
                                    Results.data[i].RECEIVE_PHYSICAL_DISPLAY_TXT = "No";
                                }

                                if (Results.data[i].TEST_PACK_RESULT == "0")
                                {
                                    var desc = "";
                                    var temp = ART_M_DECISION_REASON_SERVICE.GetByART_M_DECISION_REASON_ID(Results.data[i].TEST_PACK_FAIL_ID, context);
                                    if (temp != null)
                                    {
                                        desc = " " + temp.DESCRIPTION;
                                    }
                                    Results.data[i].TEST_PACK_RESULT_DISPLAY_TXT = "Fail" + " [" + desc + "]";
                                    Results.data[i].TEST_PACK_FAIL_DISPLAY_TXT = desc;
                                }
                                else if (Results.data[i].TEST_PACK_RESULT == "1")
                                {
                                    Results.data[i].TEST_PACK_RESULT_DISPLAY_TXT = "Pass";
                                }

                                if (Results.data[i].NEED_COMMISSIONING == "1")
                                {
                                    Results.data[i].NEED_COMMISSIONING_DISPLAY_TXT = "Yes";
                                }
                                else if (Results.data[i].NEED_COMMISSIONING == "0")
                                {
                                    Results.data[i].NEED_COMMISSIONING_DISPLAY_TXT = "No";
                                }

                                if (Results.data[i].IS_DIFFICULT == "1")
                                {
                                    Results.data[i].IS_DIFFICULT_DISPLAY_TXT = "ยาก";
                                }
                                else if (Results.data[i].IS_DIFFICULT == "0")
                                {
                                    Results.data[i].IS_DIFFICULT_DISPLAY_TXT = "ง่าย";
                                }

                                Results.data[i].CREATE_BY_DISPLAY_TXT = CNService.GetUserName(Results.data[i].CREATE_BY, context);
                            }
                        }

                        var stepId = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_WH_TEST_PACK" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var list = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID, CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var result = list.Where(p => !Results.data.Any(p2 => p2.MOCKUP_SUB_ID == p.MOCKUP_SUB_ID)).FirstOrDefault();
                        if (result != null)
                        {
                            ART_WF_MOCKUP_PROCESS_WAREHOUSE_2 item = new ART_WF_MOCKUP_PROCESS_WAREHOUSE_2();
                            item.CREATE_DATE_BY_PG = result.CREATE_DATE;
                            item.COMMENT_BY_PG = result.REMARK;
                            item.REASON_BY_PG = CNService.getReason(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(result.MOCKUP_SUB_ID, context).REASON_ID, context);
                            item.REMARK_REASON_BY_PG = CNService.getRemarkReason(result.MOCKUP_SUB_ID, "M", "SEND_PG", context);

                            item.MOCKUP_SUB_ID = result.MOCKUP_SUB_ID;
                            item.MOCKUP_ID = result.MOCKUP_ID;
                            var formPG = ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG() { MOCKUP_SUB_ID = result.MOCKUP_SUB_ID }, context);
                            if (formPG != null)
                            {
                                item.TEST_PACK_SIZING = formPG.FirstOrDefault().TEST_PACK_SIZING;
                                if (formPG.FirstOrDefault().TEST_PACK_SIZING == "X")
                                    item.TEST_PACK_SIZING_DISPLAY_TXT = "Yes";
                                item.TEST_PACK_HARD_EASY = formPG.FirstOrDefault().TEST_PACK_HARD_EASY;
                                if (formPG.FirstOrDefault().TEST_PACK_HARD_EASY == "X")
                                    item.TEST_PACK_HARD_EASY_DISPLAY_TXT = "Yes";
                                item.SUPPLIER_PRIMARY_CONTAINER = formPG.FirstOrDefault().SUPPLIER_PRIMARY_CONTAINER;
                                item.SUPPLIER_PRIMARY_LID = formPG.FirstOrDefault().SUPPLIER_PRIMARY_LID;
                                item.SHIP_TO_FACTORY = formPG.FirstOrDefault().SHIP_TO_FACTORY;
                            }

                            Results.data.Add(item);
                        }
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

        public static ART_WF_MOCKUP_PROCESS_WAREHOUSE_RESULT SaveWarehouseForm(ART_WF_MOCKUP_PROCESS_WAREHOUSE_REQUEST_LIST param_)
        {
            ART_WF_MOCKUP_PROCESS_WAREHOUSE_RESULT Results = new ART_WF_MOCKUP_PROCESS_WAREHOUSE_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        try
                        {
                            foreach (var param in param_.data)
                            {
                                ART_WF_MOCKUP_PROCESS_WAREHOUSE warehouseData = new ART_WF_MOCKUP_PROCESS_WAREHOUSE();
                                warehouseData = MapperServices.ART_WF_MOCKUP_PROCESS_WAREHOUSE(param);

                                var check = ART_WF_MOCKUP_PROCESS_WAREHOUSE_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_WAREHOUSE() { MOCKUP_SUB_ID = param.MOCKUP_SUB_ID }, context);
                                if (check.Count > 0)
                                    warehouseData.MOCKUP_SUB_WAREHOUSE_ID = check.FirstOrDefault().MOCKUP_SUB_WAREHOUSE_ID;

                                ART_WF_MOCKUP_PROCESS_WAREHOUSE_SERVICE.SaveOrUpdate(warehouseData, context);

                                if (param.ENDTASKFORM)
                                    MockUpProcessHelper.EndTaskForm(param.MOCKUP_SUB_ID, param.UPDATE_BY, context);

                                dbContextTransaction.Commit();

                                if (param.ACTION_CODE == "SEND_BACK")
                                    EmailService.sendEmailMockup(param.MOCKUP_ID, param.MOCKUP_SUB_ID, "WF_SEND_BACK", context, param.COMMENT);
                                else if (param.ACTION_CODE == "SAVE")
                                    EmailService.sendEmailMockup(param.MOCKUP_ID, param.MOCKUP_SUB_ID, "WF_OTHER_SAVE", context);
                                else
                                    EmailService.sendEmailMockup(param.MOCKUP_ID, param.MOCKUP_SUB_ID, "WF_OTHER_SUBMIT", context);
                            }
                            Results.status = "S";
                            Results.msg = MessageHelper.GetMessage("MSG_001", context);
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw ex;
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
