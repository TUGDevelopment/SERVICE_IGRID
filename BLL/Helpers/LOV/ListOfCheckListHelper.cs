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
    public class ListOfCheckListHelper
    {
        public static ART_WF_MOCKUP_CHECK_LIST_LISTOF_RESULT GetListOfCheckListHelper(ART_WF_MOCKUP_CHECK_LIST_LISTOF_REQUEST param)
        {
            ART_WF_MOCKUP_CHECK_LIST_RESULT Results = new ART_WF_MOCKUP_CHECK_LIST_RESULT();
            ART_WF_MOCKUP_CHECK_LIST_LISTOF_RESULT Results_2 = new ART_WF_MOCKUP_CHECK_LIST_LISTOF_RESULT();

            try
            {
                string displayTXT = "";

                if (param != null)
                {
                    if (param.data != null)
                    {
                        if (!String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                        {
                            displayTXT = param.data.DISPLAY_TXT;
                        }
                    }
                }

                Results_2.status = "S";
                Results.data = MapperServices.ART_WF_MOCKUP_CHECK_LIST(GetLov(displayTXT));

                List<ART_WF_ARTWORK_REQUEST_2> listRF = new List<ART_WF_ARTWORK_REQUEST_2>();
                listRF = GetRFLov(displayTXT);

                ART_WF_MOCKUP_CHECK_LIST_LISTOF lovCheckList = new ART_WF_MOCKUP_CHECK_LIST_LISTOF();

                if (Results.data.Count > 0)
                {
                   
                    Results_2.data = new List<ART_WF_MOCKUP_CHECK_LIST_LISTOF>();

                    for (int i = 0; i < Results.data.Count; i++)
                    {
                        if (!Results.data[i].CHECK_LIST_NO.StartsWith("CL-C"))
                        {
                            lovCheckList = new ART_WF_MOCKUP_CHECK_LIST_LISTOF();
                            lovCheckList.ID = Results.data[i].CHECK_LIST_NO; 
                            lovCheckList.DISPLAY_TXT = Results.data[i].CHECK_LIST_NO;
                            Results_2.data.Add(lovCheckList);
                        }
                    }

                    Results_2.data = Results_2.data.OrderBy(m => Convert.ToInt64(m.DISPLAY_TXT.Substring(5, 13).Replace("-", ""))).ToList();
                }

                if (listRF.Count() > 0)
                {
                    string rfNo = "";
                    if (Results_2.data == null)
                    {
                        Results_2.data = new List<ART_WF_MOCKUP_CHECK_LIST_LISTOF>();
                    }

                    foreach (var itemRF in listRF)
                    {
                        rfNo = itemRF.ARTWORK_REQUEST_NO;

                        lovCheckList = new ART_WF_MOCKUP_CHECK_LIST_LISTOF();
                        lovCheckList.ID = rfNo;
                        lovCheckList.DISPLAY_TXT = rfNo;
                        Results_2.data.Add(lovCheckList);
                    }
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results_2;
        }

        private static List<ART_WF_MOCKUP_CHECK_LIST> GetLov(string displayTXT)
        {
            List<ART_WF_MOCKUP_CHECK_LIST> checklists = new List<ART_WF_MOCKUP_CHECK_LIST>();
            using (ARTWORKEntities context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!String.IsNullOrEmpty(displayTXT))
                    {
                        checklists = (from p in context.ART_WF_MOCKUP_CHECK_LIST
                                      where p.CHECK_LIST_NO.Contains(displayTXT)
                                      select p).ToList();
                    }
                    else
                    {
                        checklists = (from p in context.ART_WF_MOCKUP_CHECK_LIST
                                      where p.CHECK_LIST_NO != null && p.CHECK_LIST_NO != ""
                                      select p).ToList();
                    }
                }
            }

        

            return checklists;
        }

        private static List<ART_WF_ARTWORK_REQUEST_2> GetRFLov(string displayTXT)
        {
            List<ART_WF_ARTWORK_REQUEST_2> checklistsRF = new List<ART_WF_ARTWORK_REQUEST_2>();

            using (ARTWORKEntities context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!String.IsNullOrEmpty(displayTXT))
                    {
                        checklistsRF = (from p in context.ART_WF_ARTWORK_REQUEST
                                        where p.ARTWORK_REQUEST_NO.Contains(displayTXT)
                                        select new ART_WF_ARTWORK_REQUEST_2
                                        {
                                            ARTWORK_REQUEST_ID = p.ARTWORK_REQUEST_ID,
                                            ARTWORK_REQUEST_NO = p.ARTWORK_REQUEST_NO
                                        }).ToList();
                    }
                    else
                    {
                        checklistsRF = (from p in context.ART_WF_ARTWORK_REQUEST
                                        where p.ARTWORK_REQUEST_NO != null && p.ARTWORK_REQUEST_NO != ""
                                        select new ART_WF_ARTWORK_REQUEST_2
                                        {
                                            ARTWORK_REQUEST_ID = p.ARTWORK_REQUEST_ID,
                                            ARTWORK_REQUEST_NO = p.ARTWORK_REQUEST_NO
                                        }).ToList();
                    }
                }
            }

            return checklistsRF;
        }
    }
}
