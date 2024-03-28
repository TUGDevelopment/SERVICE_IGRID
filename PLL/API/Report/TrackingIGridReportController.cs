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
    public class TrackingIGridReportController : ApiController
    {
        // GET: OverviewReport
        [Route("api/report/trackingigridreport")]
        public TRACKINGIGRID_REPORT_RESULT GetTrackingIGrid([FromUri] TRACKINGIGRID_REPORT_REQUEST param)
        {
            return TrackingIGridReportHelper.GetTrackingIGridReport(param);
        }
        [Route("api/report/counttrackingigrid")]
        public CHARTTRACKING_REPORT_RESULT GetCountTrackingIGrid([FromUri] CHARTTRACKING_REPORT_REQUEST param)
        {
            return TrackingIGridReportHelper.GetCountTrackingIGridReport(param);
        }
        [Route("api/report/trackingigridreport")]
        public TRACKINGIGRID_REPORT_RESULT PostTrackingIGrid(TRACKINGIGRID_REPORT_REQUEST param)
        {
            return TrackingIGridReportHelper.saveImpactedMatDesc(param);
        }
    }
}