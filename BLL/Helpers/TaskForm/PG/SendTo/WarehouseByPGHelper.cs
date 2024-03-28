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
    public class WarehouseByPGHelper
    {
        public static ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_RESULT GetWarehouseByPG(ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_RESULT Results = new ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG(ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_SERVICE.GetAll(context));
                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG(ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_SERVICE.GetByItemContain(MapperServices.ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG(param.data), context));
                        }

                        ART_WF_MOCKUP_PROCESS p = new ART_WF_MOCKUP_PROCESS();
                        Results.status = "S";

                        if (Results.data.Count > 0)
                        {

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

        public static ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_RESULT SaveWarehouseByPG(ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_RESULT Results = new ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        string msg = MockUpProcessHelper.checkDupWF(param.data.PROCESS, context);
                        if (msg != "")
                        {
                            Results.status = "E";
                            Results.msg = msg;
                            return Results;
                        }


                        var isFinalSample = true;
                        var SEND_RD_PRI_PKG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_RD_PRI_PKG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        var listProcess = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID, CURRENT_STEP_ID = SEND_RD_PRI_PKG }, context);
                        listProcess = listProcess.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        listProcess = listProcess.OrderByDescending(m => m.CREATE_DATE).ToList();
                        var process = listProcess.FirstOrDefault();

                        if (param.data.TEST_PACK_SIZING == "X")
                        {
                            if (process != null)
                            {
                                if (process.IS_END == "X")
                                {
                                    var rd = ART_WF_MOCKUP_PROCESS_RD_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_RD() { MOCKUP_SUB_ID = process.MOCKUP_SUB_ID }, context).OrderByDescending(m => m.CREATE_DATE).FirstOrDefault();
                                    if (rd != null)
                                    {
                                        if (rd.IS_FINAL_SAMPLE == "1")
                                        {

                                        }
                                        else
                                        {
                                            isFinalSample = false;
                                        }
                                    }
                                }
                                else
                                {
                                    isFinalSample = false;
                                }
                            }
                        }

                        if (!isFinalSample)
                        {
                            Results.status = "E";
                            Results.msg = "Cannot send to warehouse for \"เพื่อยืนยันขนาด\".(Primary packaging is not final sample.)";
                            return Results;
                        }

                        ART_WF_MOCKUP_PROCESS_RESULT processResults = new ART_WF_MOCKUP_PROCESS_RESULT();
                        if (param.data.PROCESS != null)
                        {
                            processResults = SaveProcessWarehouseByPG(param, context);
                        }

                        var warehouseData = MapperServices.ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG(param.data);

                        if (processResults != null && processResults.data != null && processResults.data.Count > 0)
                        {
                            warehouseData.MOCKUP_SUB_ID = processResults.data[0].MOCKUP_SUB_ID;
                        }

                        ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_SERVICE.SaveOrUpdate(warehouseData, context);

                        Results.data = new List<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2>();
                        ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2 item = new ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2();
                        List<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2> listItem = new List<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2>();

                        item.MOCKUP_PROCESS_WAREHOUSE_BY_PG_ID = warehouseData.MOCKUP_PROCESS_WAREHOUSE_BY_PG_ID;
                        listItem.Add(item);

                        Results.data = listItem;

                        dbContextTransaction.Commit();

                        foreach (var process2 in processResults.data)
                        {
                            EmailService.sendEmailMockup(process2.MOCKUP_ID, process2.MOCKUP_SUB_ID, "WF_SEND_TO", context);
                        }

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

        private static ART_WF_MOCKUP_PROCESS_RESULT SaveProcessWarehouseByPG(ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();

            var process = MapperServices.ART_WF_MOCKUP_PROCESS(param.data.PROCESS);

            process.CURRENT_USER_ID = CNService.GetLastestAction(process, context);
            CNService.CheckDelegateBeforeRounting(process, context);

            List<ART_WF_MOCKUP_PROCESS_2> listProcess = new List<ART_WF_MOCKUP_PROCESS_2>();

            listProcess.Add(MapperServices.ART_WF_MOCKUP_PROCESS(process));

            Results.data = listProcess;

            return Results;
        }

    }



}
