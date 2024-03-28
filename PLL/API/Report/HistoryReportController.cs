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
    public class HistoryReportController : ApiController
    {
        [Route("api/report/historyreport")]
        public HISTORY_REPORT_RESULT GetHistory([FromUri] HISTORY_REPORT_REQUEST param)
        {
            return HistoryReportHelper.GetHistoryReport(param);
        }

        [Route("api/report/historyreport_kpisummary")]
        public KPILog_Summarize_REPORT_RESULT Getkpisummary([FromUri] KPILog_Summarize_REPORT_REQUEST param)
        {
            return HistoryReportHelper.GetkpisummaryReport(param);
        }

        [Route("api/report/historyreport_kpisummary_owner")]
        public KPILog_SummarizeGroup_Report_RESULT GetKPISumerizeOwnereport([FromUri] KPILog_Summarize_REPORT_REQUEST param)
        {
            return HistoryReportHelper.GetKPISumerizeOwnereport(param);
        }

        [Route("api/report/historyreport_kpisummary_approve")]
        public KPILog_SummarizeGroup_Report_RESULT GetKPISumerizeApproveReport([FromUri] KPILog_Summarize_REPORT_REQUEST param)
        {
            return HistoryReportHelper.GetKPISumerizeApproveReport(param);
        }

    }
}