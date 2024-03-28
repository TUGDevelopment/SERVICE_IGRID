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
    public class OutstandingReportController : ApiController
    {
        [Route("api/report/outstandingreport")]
        public V_ART_ASSIGNED_SO_RESULT GetOutstandingReport([FromUri]V_ART_ASSIGNED_SO_REQUEST param)
        {
            return OutstandingReportHelper.GetOutstandingReport(param);
        }
    }
}
