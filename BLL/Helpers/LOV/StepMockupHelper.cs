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
    public class StepMockupHelper
    {
        public static ART_M_STEP_MOCKUP_RESULT GetStep(ART_M_STEP_MOCKUP_REQUEST param)
        {
            ART_M_STEP_MOCKUP_RESULT Results = new ART_M_STEP_MOCKUP_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            ART_M_STEP_MOCKUP_2 mockup_2 = new ART_M_STEP_MOCKUP_2();
                            mockup_2.IS_ACTIVE = "X";

                            Results.data = MapperServices.ART_M_STEP_MOCKUP(ART_M_STEP_MOCKUP_SERVICE.GetByItem(MapperServices.ART_M_STEP_MOCKUP(mockup_2), context));
                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.ART_M_STEP_MOCKUP(ART_M_STEP_MOCKUP_SERVICE.GetByItem(MapperServices.ART_M_STEP_MOCKUP(param.data), context));
                        }
                    }
                }
                Results.status = "S";

                if (Results.data.Count > 0)
                {
                    for (int i = 0; i < Results.data.Count; i++)
                    {
                        Results.data[i].ID = Results.data[i].STEP_MOCKUP_ID;
                        Results.data[i].DISPLAY_TXT = Results.data[i].STEP_MOCKUP_DESCRIPTION;
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
