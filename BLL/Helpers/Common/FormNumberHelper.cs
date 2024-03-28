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
    public static class FormNumberHelper
    {
        private static string splitWord = "-";

        public static string GenMockUpNo(ARTWORKEntities context, int check_list_id)
        {
            string mockup_no = "";

            ART_SYS_RUNNING_NO filter = new ART_SYS_RUNNING_NO();

            string requestType = GetMockupRequestType(check_list_id, context);

            filter.TYPE = requestType;

            ART_SYS_RUNNING_NO iRunning;
            int running_number, running_year;

            UpdateRunningNo(context, filter, out iRunning, out running_number, out running_year);

            mockup_no = iRunning.PREFIX + splitWord + running_year + splitWord + running_number.ToString().PadLeft(8, '0');

            return mockup_no;
        }

        public static string GenCheckListNo(ARTWORKEntities context, int check_list_id)
        {
            string checklist_no = "";

            ART_SYS_RUNNING_NO filter = new ART_SYS_RUNNING_NO();

            string requestType = GetCheckListRequestType(check_list_id, context);

            filter.TYPE = requestType;

            ART_SYS_RUNNING_NO iRunning;
            int running_number, running_year;

            UpdateRunningNo(context, filter, out iRunning, out running_number, out running_year);

            checklist_no = iRunning.PREFIX + splitWord + running_year + splitWord + running_number.ToString().PadLeft(8, '0');

            return checklist_no;
        }

        public static void UpdateRunningNo(ARTWORKEntities context, ART_SYS_RUNNING_NO filter, out ART_SYS_RUNNING_NO iRunning, out int running_number, out int running_year)
        {
            Random random = new Random();
            var sleep = random.Next(500, 2000);
            System.Threading.Thread.Sleep(sleep);
            var query = " select * from ART_SYS_RUNNING_NO with (tablockx) ";
            context.Database.ExecuteSqlCommand(query);

            iRunning = ART_SYS_RUNNING_NO_SERVICE.GetByItem(filter, context).FirstOrDefault();

            var temp = iRunning;
            temp.RUNNING_NO = iRunning.RUNNING_NO + 1;
            temp.UPDATE_DATE = DateTime.Now;
            temp.UPDATE_BY = -1;
            temp.RUNNING_YEAR = DateTime.Now.Year;
            ART_SYS_RUNNING_NO_SERVICE.SaveOrUpdate(temp, context);

            iRunning = ART_SYS_RUNNING_NO_SERVICE.GetByItem(filter, context).FirstOrDefault();
            running_year = iRunning.RUNNING_YEAR;
            running_number = iRunning.RUNNING_NO;
        }

        private static string GetCheckListRequestType(int check_list_id, ARTWORKEntities context)
        {
            var checklist = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(check_list_id, context);
            string forDesign = "";
            string forDieLine = "";
            string requestType = "";

            if (checklist != null)
            {
                forDesign = checklist.CHECK_LIST_FOR_DESIGN;
                forDieLine = checklist.REQUEST_FOR_DIE_LINE;
            }

            if ((String.IsNullOrEmpty(forDesign) && String.IsNullOrEmpty(forDieLine))
                || (forDesign == "0" && forDieLine == "0"))
            {
                requestType = "CHECK_LIST_NORMAL";
            }
            else if (forDesign == "1")
            {
                requestType = "CHECK_LIST_PROJECT";
            }
            else if (forDieLine == "1")
            {
                requestType = "CHECK_LIST_DIE_LINE";
            }
            else
            {
                requestType = "CHECK_LIST_REQUEST";
            }

            return requestType;
        }

        private static string GetMockupRequestType(int check_list_id, ARTWORKEntities context)
        {
            var checklist = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(check_list_id, context);
            string forDesign = "";
            string forDieLine = "";
            string requestType = "";

            if (checklist != null)
            {
                forDesign = checklist.CHECK_LIST_FOR_DESIGN;
                forDieLine = checklist.REQUEST_FOR_DIE_LINE;
            }

            if ((String.IsNullOrEmpty(forDesign) && String.IsNullOrEmpty(forDieLine))
                || (forDesign == "0" && forDieLine == "0"))
            {
                requestType = "MOCKUP_NORMAL";
            }
            else if (forDesign == "1")
            {
                requestType = "MOCKUP_PROJECT";
            }
            else if (forDieLine == "1")
            {
                requestType = "MOCKUP_DIE_LINE";
            }
            else
            {
                requestType = "CHECK_LIST_REQUEST";
            }

            return requestType;
        }

        public static string UpdateCheckListNo(int check_list_id, ARTWORKEntities context)
        {
            var checklist = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(check_list_id, context);

            if (checklist != null)
            {
                string checklist_no = GenCheckListNo(context, check_list_id);

                checklist.CHECK_LIST_NO = checklist_no;

                ART_WF_MOCKUP_CHECK_LIST_SERVICE.SaveOrUpdate(checklist, context);

                return checklist_no;
            }
            return "";
        }

        public static string GenArtworkRequestFormNo(ARTWORKEntities context)
        {
            string artwork_request_no = "";

            ART_SYS_RUNNING_NO filter = new ART_SYS_RUNNING_NO();

            string requestType = "ARTWORK_REQUEST_FORM";

            filter.TYPE = requestType;

            ART_SYS_RUNNING_NO iRunning;
            int running_number, running_year;

            UpdateRunningNo(context, filter, out iRunning, out running_number, out running_year);

            artwork_request_no = iRunning.PREFIX + splitWord + running_year + splitWord + running_number.ToString().PadLeft(8, '0');

            return artwork_request_no;
        }

        public static string GenArtworkTaskFormNo(ARTWORKEntities context, int artworkID)
        {
            string artwork_request_no = "";

            ART_SYS_RUNNING_NO filter = new ART_SYS_RUNNING_NO();

            ART_WF_ARTWORK_REQUEST artworkRequest = new ART_WF_ARTWORK_REQUEST();
            artworkRequest.ARTWORK_REQUEST_ID = artworkID;
            artworkRequest = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(artworkID, context);

            if (artworkRequest != null)
            {
                string requestType = "ARTWORK_NEW";

                string type = artworkRequest.TYPE_OF_ARTWORK;

                if (type == "NEW")
                {
                    requestType = "ARTWORK_NEW";
                }
                else if (type == "REPEATXX")
                {
                    requestType = "ARTWORK_REPEAT";

                    var soRepeats = (from r in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                     where r.ARTWORK_REQUEST_ID == artworkRequest.ARTWORK_REQUEST_ID
                                     && !string.IsNullOrEmpty(r.COMPONENT_MATERIAL)
                                     select r).Distinct().ToList();

                    if (soRepeats.FirstOrDefault() != null)
                    {
                        var mat5 = soRepeats.FirstOrDefault().COMPONENT_MATERIAL;

                        var wfAssignSOComplete = (from m in context.ART_WF_ARTWORK_PROCESS
                                                  join k in context.V_ART_ASSIGNED_SO on m.ARTWORK_SUB_ID equals k.ARTWORK_SUB_ID
                                                  where k.COMPONENT_MATERIAL == mat5 && m.IS_END == "X" && string.IsNullOrEmpty(m.IS_TERMINATE) && string.IsNullOrEmpty(m.REMARK_KILLPROCESS)
                                                  select k.RDD).ToList();

                        if (wfAssignSOComplete.Count > 0)
                        {
                            wfAssignSOComplete = wfAssignSOComplete.OrderByDescending(m => m).ToList();
                            DateTime rdd = Convert.ToDateTime(wfAssignSOComplete[0].Value);
                            DateTime currentDate = DateTime.Now.Date;
                            if (currentDate > rdd)
                            {
                                int diffMonth = GetMonthDifference(currentDate, rdd);
                                if (diffMonth > 6)
                                {
                                    requestType = "ARTWORK_REPEAT_OVER_6_MONTH";
                                }
                            }
                        }
                        else
                        {
                            var listSO = (from m in context.V_SAP_SALES_ORDER
                                          where m.COMPONENT_MATERIAL == mat5 && m.IS_MIGRATION == "X"
                                          select m.RDD).ToList();

                            if (listSO.Count > 0)
                            {
                                listSO = listSO.OrderByDescending(m => m).ToList();
                                DateTime rdd = Convert.ToDateTime(listSO[0].Value);
                                DateTime currentDate = DateTime.Now.Date;
                                if (currentDate > rdd)
                                {
                                    int diffMonth = GetMonthDifference(currentDate, rdd);
                                    if (diffMonth > 6)
                                    {
                                        requestType = "ARTWORK_REPEAT_OVER_6_MONTH";
                                    }
                                }
                            }
                        }
                    }
                }
                else if(type == "REPEAT")
                {
                    requestType = "ARTWORK_REPEAT";

                    var soRepeats = (from r in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                     where r.ARTWORK_REQUEST_ID == artworkRequest.ARTWORK_REQUEST_ID
                                     && !string.IsNullOrEmpty(r.COMPONENT_MATERIAL)
                                     select r).Distinct().ToList();

                    if (soRepeats.FirstOrDefault() != null)
                    {
                        var mat5 = soRepeats.FirstOrDefault().COMPONENT_MATERIAL;
                        var RECHECK_ARTWORK = string.Format("{0}", CNService.GetCheckRDD(mat5));
                        if(RECHECK_ARTWORK=="Yes")
                            requestType = "ARTWORK_REPEAT_OVER_6_MONTH";
                    }
                }
                else
                {
                    requestType = "ARTWORK_NEW";
                }

                filter.TYPE = requestType;

                ART_SYS_RUNNING_NO iRunning;
                int running_number, running_year;

                UpdateRunningNo(context, filter, out iRunning, out running_number, out running_year);

                artwork_request_no = iRunning.PREFIX + splitWord + running_year + splitWord + running_number.ToString().PadLeft(8, '0');
            }

            return artwork_request_no;
        }
        public static string GenArtworkRepeat(ARTWORKEntities context, int artworkID, string requestType)
        {
            string artwork_request_no = "";

            ART_SYS_RUNNING_NO filter = new ART_SYS_RUNNING_NO();

            ART_WF_ARTWORK_REQUEST artworkRequest = new ART_WF_ARTWORK_REQUEST();
            artworkRequest.ARTWORK_REQUEST_ID = artworkID;
            artworkRequest = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(artworkID, context);

            if (artworkRequest != null)
            {
                filter.TYPE = requestType;

                ART_SYS_RUNNING_NO iRunning;
                int running_number, running_year;

                UpdateRunningNo(context, filter, out iRunning, out running_number, out running_year);

                artwork_request_no = iRunning.PREFIX + splitWord + running_year + splitWord + running_number.ToString().PadLeft(8, '0');
            }

            return artwork_request_no;
        }
        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }
    }
}
