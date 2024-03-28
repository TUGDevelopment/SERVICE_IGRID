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
    public class SelectVendor
    {
        public static ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_RESULT Get(ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {

                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR(ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_SERVICE.GetByItem(MapperServices.ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR(param.data), context));
                        }

                        Results.status = "S";

                        if (Results.data.Count > 0)
                        {
                            foreach (ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_2 itemVendor in Results.data)
                            {
                                itemVendor.VENDOR_DISPLAY_TXT = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(itemVendor.VENDOR_ID, context).VENDOR_NAME;
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

        public static ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_RESULT GetLog(ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {

                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR(ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_SERVICE.GetByItem(MapperServices.ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                ART_SYS_ACTION act = new ART_SYS_ACTION();
                                act.ACTION_CODE = Results.data[i].ACTION_CODE;
                                Results.data[i].ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItem(act, context).FirstOrDefault().ACTION_NAME;
                                Results.data[i].CREATE_BY_DISPLAY_TXT = CNService.GetUserName(Results.data[i].CREATE_BY, context);
                                Results.data[i].COMMENT_BY_PG = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).REMARK;
                                Results.data[i].REASON_BY_PG = CNService.getReason(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).REASON_ID, context);
                                Results.data[i].REASON_BY_OTHER = CNService.getReason(Results.data[i].REASON_ID, context);
                                Results.data[i].CREATE_DATE_BY_PG = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(Results.data[i].MOCKUP_SUB_ID, context).CREATE_DATE;
                                Results.data[i].VENDOR_DISPLAY_TXT = CNService.GetVendorName(Results.data[i].VENDOR_ID, context);
                                Results.data[i].REMARK_REASON_BY_PG = CNService.getRemarkReason(Results.data[i].MOCKUP_SUB_ID, "M", "SEND_PG", context);
                                Results.data[i].REMARK_REASON = CNService.getRemarkReason(Results.data[i].MOCKUP_SUB_ID, "M", "SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN", context);
                            }
                        }

                        var stepId = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var list = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID, CURRENT_STEP_ID = stepId }, context).ToList();
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var result = list.Where(p => !Results.data.Any(p2 => p2.MOCKUP_SUB_ID == p.MOCKUP_SUB_ID)).FirstOrDefault();
                        if (result != null)
                        {
                            ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_2 item = new ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_2();
                            item.CREATE_DATE_BY_PG = result.CREATE_DATE;
                            item.COMMENT_BY_PG = result.REMARK;
                            item.REASON_BY_PG = CNService.getReason(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(result.MOCKUP_SUB_ID, context).REASON_ID, context);
                            item.REMARK_REASON_BY_PG = CNService.getRemarkReason(result.MOCKUP_SUB_ID, "M", "SEND_PG", context);
                            item.REMARK_REASON = CNService.getRemarkReason(result.MOCKUP_SUB_ID, "M", "SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN", context);
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



        public static ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_RESULT Save(ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_REQUEST_LIST param)
        {
            ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_RESULT();

            try
            {
                var remark = "";
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        foreach (var item in param.data)
                        {
                            remark = item.REMARK;
                            var check = ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR() { MOCKUP_SUB_ID = item.MOCKUP_SUB_ID }, context);
                            if (check.Count > 0)
                                item.PG_SELECT_VENDOR_ID = check.FirstOrDefault().PG_SELECT_VENDOR_ID;

                            ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_SERVICE.SaveOrUpdate(MapperServices.ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR(item), context);

                            //auto update filed vendor in tab pg
                            var parent_mockup_sub_id = Convert.ToInt32(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(item.MOCKUP_SUB_ID, context).PARENT_MOCKUP_SUB_ID);
                            var temp = ART_WF_MOCKUP_PROCESS_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG() { MOCKUP_SUB_ID = parent_mockup_sub_id }, context).FirstOrDefault();
                            if (temp != null)
                            {
                                temp.VENDOR = item.VENDOR_ID;
                                ART_WF_MOCKUP_PROCESS_PG_SERVICE.SaveOrUpdate(temp, context);
                            }
                        }

                        if (param.ENDTASKFORM)
                        {
                            MockUpProcessHelper.EndTaskForm(param.data[0].MOCKUP_SUB_ID, param.data[0].UPDATE_BY, context);
                        }

                        dbContextTransaction.Commit();

                        if (param.data[0].ACTION_CODE == "SEND_BACK")
                            EmailService.sendEmailMockup(param.data[0].MOCKUP_ID, param.data[0].MOCKUP_SUB_ID, "WF_SEND_BACK", context, remark);
                        else if (param.data[0].ACTION_CODE == "SAVE")
                            EmailService.sendEmailMockup(param.data[0].MOCKUP_ID, param.data[0].MOCKUP_SUB_ID, "WF_OTHER_SAVE", context);
                        else
                            EmailService.sendEmailMockup(param.data[0].MOCKUP_ID, param.data[0].MOCKUP_SUB_ID, "WF_OTHER_SUBMIT", context);

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

    }
}
