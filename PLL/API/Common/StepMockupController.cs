using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BLL.Services;
using BLL.Helpers;
using DAL;
using DAL.Model;

namespace PLL.API.Common
{
    public class StepMockupController : ApiController
    {
        [Route("api/common/stepmockup")]
        public ART_M_STEP_MOCKUP_RESULT GetStepMockup()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    ART_M_STEP_MOCKUP_RESULT res = new ART_M_STEP_MOCKUP_RESULT();
                    res.data = MapperServices.ART_M_STEP_MOCKUP(ART_M_STEP_MOCKUP_SERVICE.GetAll(context));
                    res.status = "S";
                    return res;
                }
            }
        }

        [Route("api/common/stepartwork")]
        public ART_M_STEP_ARTWORK_RESULT GetStepArtwork()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    ART_M_STEP_ARTWORK_RESULT res = new ART_M_STEP_ARTWORK_RESULT();
                    res.data = MapperServices.ART_M_STEP_ARTWORK(ART_M_STEP_ARTWORK_SERVICE.GetAll(context));
                    res.status = "S";
                    return res;
                }
            }
        }

        [Route("api/common/stepmockupandpartwork")]
        public WORK_FLOW_STEP_RESULT GetStepMockupAndArtwork([FromUri]WORK_FLOW_STEP_REQUEST param)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var stepMockup = MapperServices.ART_M_STEP_MOCKUP(ART_M_STEP_MOCKUP_SERVICE.GetAll(context));
                    var stepArtwork = MapperServices.ART_M_STEP_ARTWORK(ART_M_STEP_ARTWORK_SERVICE.GetAll(context));

                    var res = new WORK_FLOW_STEP_RESULT();
                    res.data = new List<WORK_FLOW_STEP>();

                    int i = 1;
                    foreach (var item in stepMockup)
                    {
                        var resItem = new WORK_FLOW_STEP();
                        resItem.ID = i;
                        resItem.DISPLAY_TXT = item.STEP_MOCKUP_NAME;
                        resItem.WF_TYPE = "M" + item.STEP_MOCKUP_ID;
                        resItem.STEP_ID = item.STEP_MOCKUP_ID;   // by aof for report end to end
                        res.data.Add(resItem);
                        i++;
                    }

                    foreach (var item in stepArtwork)
                    {
                        var resItem = new WORK_FLOW_STEP();
                        resItem.ID = i;
                        resItem.DISPLAY_TXT = item.STEP_ARTWORK_NAME;
                        resItem.WF_TYPE = "A" + item.STEP_ARTWORK_ID;
                        resItem.STEP_ID = item.STEP_ARTWORK_ID;  // by aof forreport end to end
                        res.data.Add(resItem);
                        i++;
                    }

                    if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                    {
                        res.data = (from u1 in res.data
                                    where u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                    select u1).ToList();
                    }

                    res.status = "S";
                    return res;
                }
            }
        }
    }
}
