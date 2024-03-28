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

namespace PLL.API.Report
{
    public class IGRIDMatStatusReportController : ApiController
    {
        [Route("api/report/igrid_matstatus_report")]
        public IGRID_MATSTATUS_REPORT_MODEL_RESULT GetMatStatusReport([FromUri]IGRID_MATSTATUS_REPORT_MODEL_REQUEST param)
        {
            return IGRIDMatStatusReportHelper.GetMatStatusReport(param);
        }


        [Route("api/report/igrid_matstatus_reactive")]
        public IGRID_MATSTATUS_REPORT_MODEL_RESULT PostMatStatusReactive(IGRID_MATSTATUS_REPORT_MODEL_REQUEST param)
        {
            return IGRIDMatStatusReportHelper.PostMatStatusReactive(param);
        }



    }
}