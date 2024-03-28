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
    public class TrackingReportController : ApiController
    {
        //http://localhost:60448/api/report/trackingreport?data.SOLD_TO_NAME=บจก.ธีร์
        [Route("api/report/trackingreport")]
        public TRACKING_REPORT_RESULT GetTrackingReport([FromUri]TRACKING_REPORT_REQUEST param)
        {
            return TrackingReportHelper.GetTrackingReport(param);
        }

        [Route("api/report/trackingreport_new")]
        public V_ART_ENDTOEND_REPORT_RESULT GetViewTrackingReport([FromUri]V_ART_ENDTOEND_REPORT_REQUEST param)
        {
            return TrackingReportHelper.GetViewTrackingReport(param);
        }


        [Route("api/report/trackingreport_v3")]
        public TU_TRACKING_WF_REPORT_MODEL_RESULT GetViewTrackingReportV3([FromUri]TU_TRACKING_WF_REPORT_MODEL_REQUEST param)
        {
            return TrackingReportHelper.GetViewTrackingReportV3(param);
        }


    }
}
