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
    public class WorkFlowStepHelper
    {
        public static WORK_FLOW_STEP_RESULT GetStep(WORK_FLOW_STEP_REQUEST param)
        {
            WORK_FLOW_STEP_RESULT Results = new WORK_FLOW_STEP_RESULT();
            List<ART_M_STEP_MOCKUP_2> listStepMockup = new List<ART_M_STEP_MOCKUP_2>();

            WORK_FLOW_STEP wfStep = new WORK_FLOW_STEP();
            List<WORK_FLOW_STEP> listWFStep = new List<WORK_FLOW_STEP>();

            string Type = "";
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

                            listStepMockup.AddRange(MapperServices.ART_M_STEP_MOCKUP(ART_M_STEP_MOCKUP_SERVICE.GetByItem(MapperServices.ART_M_STEP_MOCKUP(mockup_2), context)));
                        }
                        else
                        {
                            Type = param.data.WF_TYPE;
                            ART_M_STEP_MOCKUP_2 mockup_2 = new ART_M_STEP_MOCKUP_2();
                            mockup_2.IS_ACTIVE = "X";
                            mockup_2.STEP_MOCKUP_ID = param.data.ID;

                            listStepMockup.AddRange(MapperServices.ART_M_STEP_MOCKUP(ART_M_STEP_MOCKUP_SERVICE.GetByItem(MapperServices.ART_M_STEP_MOCKUP(mockup_2), context)));
                        }
                    }
                }
                Results.status = "S";

                listStepMockup = (from l in listStepMockup where !String.IsNullOrEmpty(l.STEP_MOCKUP_CODE) select l).ToList();

                if (listStepMockup.Count > 0)
                {
                    for (int i = 0; i < listStepMockup.Count; i++)
                    {
                        wfStep = new WORK_FLOW_STEP();
                        wfStep.ID = listStepMockup[i].STEP_MOCKUP_ID;
                        wfStep.DISPLAY_TXT = listStepMockup[i].STEP_MOCKUP_DESCRIPTION;
                        wfStep.WF_TYPE = Type;

                        listWFStep.Add(wfStep);
                    }
                }

                Results.data = listWFStep;
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
