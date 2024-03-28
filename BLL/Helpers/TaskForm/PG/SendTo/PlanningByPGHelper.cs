//using BLL.Services;
//using DAL;
//using DAL.Model;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BLL.Helpers
//{
//    public class PlanningByPGHelper
//    {
//        public static ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_RESULT GetPlanningByPG(ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_REQUEST param)
//        {
//            ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_RESULT Results = new ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_RESULT();

//            try
//            {
//                if (param == null || param.data == null)
//                {
//                    Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG(ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_SERVICE.GetAll());
//                }
//                else
//                {
//                    Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG(ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_SERVICE.GetByItemContain(MapperServices.ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG(param.data)));
//                }

//                ART_WF_MOCKUP_PROCESS p = new ART_WF_MOCKUP_PROCESS();

//                // Results.data[0].Process = MapperServices.ART_WF_MOCKUP_PROCESS(p);

//                Results.status = "S";

//                if (Results.data.Count > 0)
//                {
//                    //var result = Results.data.GroupBy(p => p.)
//                    //   .Select(grp => grp.First())
//                    //   .ToList();

//                    //Results.data = Results.data.OrderBy(x => x.VENDOR_NAME).ToList();

//                    //for (int i = 0; i < Results.data.Count; i++)
//                    //{
//                    //    Results.data[i].FLUTE_FOR_REQ_SAMPLE_DIELINE_DISPLAY_TXT = Results.data[i].FLUTE_FOR_REQ_SAMPLE_DIELINE.ToString();
//                    //    Results.data[i].SPECIFICATION_FOR_REQ_SAMPLE_DIELINE_DISPLAY_TXT = Results.data[i].SPECIFICATION_FOR_REQ_SAMPLE_DIELINE.ToString(); // + " : " + Results.data[i].DESCRIPTION;
//                    //}
//                }
//            }
//            catch (Exception ex)
//            {
//                Results.status = "E";
//                Results.msg = CNService.GetErrorMessage(ex);
//            }

//            return Results;
//        }

//        public static ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_RESULT SavePlanningByPG(ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_REQUEST param)
//        {
//            ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_RESULT Results = new ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_RESULT();
//            try
//            {
//                using (var context = new ARTWORKEntities())
//                {
//                    using (var dbContextTransaction = CNService.IsolationLevel(context))
//                    {
//                        string msg = MockUpProcessHelper.checkDupWF(param.data.PROCESS, context);
//                        if (msg != "")
//                        {
//                            Results.status = "E";
//                            Results.msg = msg;
//                            return Results;
//                        }

//                        ART_WF_MOCKUP_PROCESS_RESULT processResults = new ART_WF_MOCKUP_PROCESS_RESULT();
//                        if (param.data.PROCESS != null)
//                        {
//                            processResults = SaveProcessPlanningByPG(param, context);
//                        }

//                        var planningData = MapperServices.ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG(param.data);

//                        if (processResults != null && processResults.data != null && processResults.data.Count > 0)
//                        {
//                            planningData.MOCKUP_SUB_ID = processResults.data[0].MOCKUP_SUB_ID;
//                        }

//                        ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_SERVICE.SaveOrUpdate(planningData, context);

//                        Results.data = new List<ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_2>();
//                        ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_2 item = new ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_2();
//                        List<ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_2> listItem = new List<ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_2>();

//                        item.MOCKUP_SUB_RD_BY_PLANNING_ID = planningData.MOCKUP_SUB_RD_BY_PLANNING_ID;
//                        listItem.Add(item);

//                        Results.data = listItem;

//                        dbContextTransaction.Commit();

//                        foreach (var process2 in processResults.data)
//                        {
//                            EmailService.sendEmail(process2.MOCKUP_ID, process2.MOCKUP_SUB_ID, "MOCKUP_RECEIVE_WF_TO", context);
//                        }

//                        Results.status = "S";
//                        Results.msg = MessageHelper.GetMessage("MSG_001");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Results.status = "E";
//                Results.msg = CNService.GetErrorMessage(ex);
//            }
//            return Results;
//        }

//        private static ART_WF_MOCKUP_PROCESS_RESULT SaveProcessPlanningByPG(ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_REQUEST param, ARTWORKEntities context)
//        {
//            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();

//            var process = MapperServices.ART_WF_MOCKUP_PROCESS(param.data.PROCESS);

//            ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(process, context);

//            List<ART_WF_MOCKUP_PROCESS_2> listProcess = new List<ART_WF_MOCKUP_PROCESS_2>();

//            listProcess.Add(MapperServices.ART_WF_MOCKUP_PROCESS(process));

//            Results.data = listProcess;

//            return Results;
//        }

//    }



//}
