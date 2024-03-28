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
    public class EndToEndReportController : ApiController
    {
        //http://localhost:60448/api/report/trackingreport?data.SOLD_TO_NAME=บจก.ธีร์
        [Route("api/report/endtoendreport")]
        public TRACKING_REPORT_RESULT GetEndToEnd([FromUri]TRACKING_REPORT_REQUEST param)
        {
            return EndToEndReportHelper.GetEndToEndReport(param);
        }

        [Route("api/report/endtoendreport_new")]
        public V_ART_ENDTOEND_REPORT_RESULT GetViewEndToEndReport([FromUri]V_ART_ENDTOEND_REPORT_REQUEST param)
        {
            return EndToEndReportHelper.GetViewEndToEndReport(param);
        }

        [Route("api/report/endtoendreport_file")]
        public TRACKING_REPORT_RESULT GetEndToEndReport_File([FromUri]TRACKING_REPORT_REQUEST param)
        {
            return EndToEndReportHelper.GetEndToEndReport_File(param);
        }
    }
}
