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
//using Newtonsoft.Json;
//using System.Dynamic;

namespace PLL.API
{
    public class IGridSummaryReportController : ApiController
    {
        [Route("api/report/igridsummaryreport")]
        public IGridSummary_REPORT_RESULT GetIGridSummary([FromUri] IGridSummary_REPORT_REQUEST param)
        {
            return IGridSummaryReportHelper.GetIGridSummaryReport(param);
        }

        [Route("api/report/igridsummarygroupreport")]
        public IGridSummary_Group_REPORT_RESULT GetIGridSummaryGroupReport([FromUri] IGridSummary_REPORT_REQUEST param)
        {
            return IGridSummaryReportHelper.GetIGridSummaryGroupReport(param);
        }


        [Route("api/report/igridcompletereport")]
        public IGridSummary_REPORT_RESULT GetIGridComplete([FromUri] IGridSummary_REPORT_REQUEST param)
        {
            return IGridSummaryReportHelper.GetIGridCompleteReport(param);
        }

        [Route("api/report/igridinfogroupreport")]
        public IGRID_RESULT GetIGridComplete([FromUri] IGRID_REQUEST param)
        {
            return IGridSummaryReportHelper.GetIGridInfoGroupReport(param);
        }


    }
}