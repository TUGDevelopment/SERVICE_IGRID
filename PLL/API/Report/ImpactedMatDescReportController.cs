using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;
using DAL;

namespace PLL.API
{
    public class ImpactedMatDescReportController : ApiController
    {
        // GET: OverviewReport
        [Route("api/report/ImpactedMatDesc")]
        public ImpactedMatDesc_REPORT_RESULT GetImpactedMatDesc([FromUri] ImpactedMatDesc_REPORT_REQUEST param)
        {
            return ImpactedMatDescReportHelper.GetImpactedMatDescReport(param);
        }

        [Route("api/report/ImpactedMatDesc")]
        public ImpactedMatDesc_REPORT_RESULT PostImpactedMatDesc(ImpactedMatDesc_REPORT_REQUEST_LIST param)
        {
            return ImpactedMatDescReportHelper.saveImpactedMatDesc(param);
        }

    }
}