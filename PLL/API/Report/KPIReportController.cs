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
    public class KPIReportController : ApiController
    {
        [Route("api/report/kpireport")]
        public KPI_REPORT_MODEL_RESULT GetSummaryReport([FromUri]KPI_REPORT_MODEL_REQUEST param)
        {
            return KPIReportHelper.GetKPIReport(param);
        }
        [Route("api/report/kpisavingreport")]
        public KPI_REPORT_MODEL_RESULT GetKPIPriceTemplateCompare([FromUri]ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST param)
        {
            return KPIReportHelper.GetKPIPriceTemplateCompare(param);
        }
    }
}
