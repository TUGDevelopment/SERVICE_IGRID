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
    public class DesisionController : ApiController
    {
        [Route("api/common/desisioncancel")]
        public ART_M_DECISION_REASON_RESULT GetDesisionCancel([FromUri]ART_M_DECISION_REASON_REQUEST param)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    ART_M_DECISION_REASON_RESULT res = new ART_M_DECISION_REASON_RESULT();
                    res.data = MapperServices.ART_M_DECISION_REASON(ART_M_DECISION_REASON_SERVICE.GetByItem(param.data, context));
                    foreach (var item in res.data)
                    {
                        item.ID = item.ART_M_DECISION_REASON_ID;
                        item.DISPLAY_TXT = item.DESCRIPTION;
                    }

                    res.status = "S";
                    return res;
                }
            }
        }

        [Route("api/common/desisionrevise")]
        public ART_M_DECISION_REASON_RESULT GetDesisionRevise([FromUri]ART_M_DECISION_REASON_REQUEST param)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    ART_M_DECISION_REASON_RESULT res = new ART_M_DECISION_REASON_RESULT();
                    res.data = MapperServices.ART_M_DECISION_REASON(ART_M_DECISION_REASON_SERVICE.GetByItem(param.data, context));
                    foreach (var item in res.data)
                    {
                        item.ID = item.ART_M_DECISION_REASON_ID;
                        item.DISPLAY_TXT = item.DESCRIPTION;
                    }
                    res.status = "S";
                    return res;
                }
            }
        }

        [Route("api/common/desisiontestpackfail")]
        public ART_M_DECISION_REASON_RESULT GetDesisionTestPackFail([FromUri]ART_M_DECISION_REASON_REQUEST param)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    ART_M_DECISION_REASON_RESULT res = new ART_M_DECISION_REASON_RESULT();
                    res.data = MapperServices.ART_M_DECISION_REASON(ART_M_DECISION_REASON_SERVICE.GetByItem(param.data, context));
                    foreach (var item in res.data)
                    {
                        item.ID = item.ART_M_DECISION_REASON_ID;
                        item.DISPLAY_TXT = item.DESCRIPTION;
                    }
                    res.status = "S";
                    return res;
                }
            }
        }
    }
}
