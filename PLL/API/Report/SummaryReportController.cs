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
    public class SummaryReportController : ApiController
    {
        [Route("api/report/summaryreportchartbystep")]
        public SUMMARY_REPORT_MODEL_RESULT GetSummaryReportChartByStep([FromUri]SUMMARY_REPORT_MODEL_REQUEST param)
        {
            return SummaryReportHelper.GetSummaryReportChartByStep(param);
        }

        [Route("api/report/summaryreportchartbyworkflow")]
        public SUMMARY_REPORT_MODEL_RESULT GetSummaryReportChartByWorkflowType([FromUri]SUMMARY_REPORT_MODEL_REQUEST param)
        {
            return SummaryReportHelper.GetSummaryReportChartByWorkflowType(param);
        }

        [Route("api/report/summaryreportdatabystep")]
        public SUMMARY_REPORT_MODEL_RESULT GetSummaryReportDataByStep([FromUri]SUMMARY_REPORT_MODEL_REQUEST param)
        {
            return SummaryReportHelper.GetSummaryReportDataByStep(param);
        }

        [Route("api/report/summaryreportdatabyworkflowtype")]
        public SUMMARY_REPORT_MODEL_RESULT GetSummaryReportDataByWorkflowType([FromUri]SUMMARY_REPORT_MODEL_REQUEST param)
        {
            return SummaryReportHelper.GetSummaryReportDataByWorkflowType(param);
        }

        [Route("api/report/summaryreportdatadetail")]
        public SUMMARY_REPORT_MODEL_RESULT GetSummaryReportDataDetail([FromUri]SUMMARY_REPORT_MODEL_REQUEST param)
        {
            return SummaryReportHelper.GetSummaryReportDataDetail(param);
        }
    }
}
