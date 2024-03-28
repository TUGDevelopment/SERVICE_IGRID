using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace BLL.Helpers
{
    public class CheckListInfoHelper
    {
        public static ART_WF_MOCKUP_CHECK_LIST_RESULT GetCheckListInfo(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            ART_WF_MOCKUP_CHECK_LIST_RESULT Results = new ART_WF_MOCKUP_CHECK_LIST_RESULT();

            try
            {
                int mock_id = 0;
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param.data != null && param.data.MOCKUP_SUB_ID > 0)
                        {
                            param.data.MOCKUP_ID = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_SUB_ID = param.data.MOCKUP_SUB_ID }, context).FirstOrDefault().MOCKUP_ID;
                            mock_id = param.data.MOCKUP_ID;

                            var item = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(param.data.MOCKUP_ID, context);

                            if (item != null)
                            {
                                ART_WF_MOCKUP_CHECK_LIST_2 checklist = new ART_WF_MOCKUP_CHECK_LIST_2();
                                ART_WF_MOCKUP_CHECK_LIST_REQUEST checklistParam = new ART_WF_MOCKUP_CHECK_LIST_REQUEST();

                                checklist.CHECK_LIST_ID = item.CHECK_LIST_ID;

                                checklistParam.data = checklist;
                                Results = CheckListRequestHelper.GetCheckListRequest(checklistParam);

                                List<ART_WF_MOCKUP_CHECK_LIST_2> listChecklist2 = new List<ART_WF_MOCKUP_CHECK_LIST_2>();

                                listChecklist2 = Results.data;

                                for (int i = 0; i < listChecklist2.Count; i++)
                                {
                                    var tempItem = Results.data[i].ITEM;

                                    var listItem = (from s in tempItem
                                                    where s.MOCKUP_ID == mock_id
                                                    select s).ToList();

                                    Results.data[i].MOCKUP_NO_DISPLAY_TXT = item.MOCKUP_NO;
                                    Results.data[i].ITEM = listItem;
                                }
                            }
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


    }
}
