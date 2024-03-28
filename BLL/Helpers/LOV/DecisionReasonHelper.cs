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
    public class DecisionReasonHelper
    {
        public static ART_M_DECISION_REASON_RESULT GetDecisionReason(ART_M_DECISION_REASON_REQUEST param)
        {
            ART_M_DECISION_REASON_RESULT Results = new ART_M_DECISION_REASON_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        param.data.IS_ACTIVE = "X";

                        Results.data = MapperServices.ART_M_DECISION_REASON(ART_M_DECISION_REASON_SERVICE.GetByItem(param.data, context));
                        if (string.IsNullOrEmpty(param.data.IS_OVERDUE))
                        {
                            Results.data = Results.data.Where(m => string.IsNullOrEmpty(m.IS_OVERDUE)).ToList();
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].ART_M_DECISION_REASON_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].DESCRIPTION;
                            }
                            Results.data = Results.data.OrderBy(x => x.DISPLAY_TXT).ToList();
                        }

                        ART_M_DECISION_REASON_2 item = new ART_M_DECISION_REASON_2();
                        item.ID = ART_M_DECISION_REASON_SERVICE.GetByItem(new ART_M_DECISION_REASON() { IS_DEFAULT = "X" }, context)[0].ART_M_DECISION_REASON_ID;
                        item.DISPLAY_TXT = ART_M_DECISION_REASON_SERVICE.GetByItem(new ART_M_DECISION_REASON() { IS_DEFAULT = "X" }, context)[0].DESCRIPTION;
                        Results.data.Insert(0, item);

                        if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                        {
                            Results.data = (from u1 in Results.data
                                            where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                            select u1).ToList();
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
