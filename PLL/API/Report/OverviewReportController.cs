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
    public class OverviewReportController : ApiController
    {
        // GET: OverviewReport
        [Route("api/report/overviewreport")]
        public OVERVIEW_REPORT_RESULT GetOverview([FromUri] OVERVIEW_REPORT_REQUEST param)
        {
            return OverviewReportHelper.GetOverviewReport(param);
        }
        [Route("api/report/overviewreport")]
        public OVERVIEW_REPORT_RESULT PostOverview(OVERVIEW_REPORT_REQUEST param)
        {
            return OverviewReportHelper.PostOverviewReport(param);
        }

        [Route("api/report/overviewreport_createdoc")]
        public OVERVIEW_REPORT_RESULT PostCreateDocByPA(OVERVIEW_REPORT_REQUEST param)
        {
            return OverviewReportHelper.PostCreateDocByPA(param);
        }

        [Route("api/report/overviewreport_getrole")]
        public ulogin_RESULT GetOverviewRole(ulogin_RESULT param)
        {
            return OverviewReportHelper.GetOverviewRole(param);
        }

    }
}