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
    public class IGRIDOverallMasterReporController : ApiController
    {

        [Route("api/report/igrid_overall_master_report")]
        public IGRID_OVERALL_MASTER_REPORT_MODEL_RESULT GetOverallMasterReport([FromUri]IGRID_OVERALL_MASTER_REPORT_MODEL_REQUEST param)
        {
            return IGRIDOverallMasterReportHelper.GetOverallMasterReport(param);
        }



        [Route("api/report/igrid_overall_get_master")]
        public IGRID_CBB_DATA_MODEL_RESULT GetMasterData([FromUri]IGRID_CBB_DATA_MODEL_REQUEST param)
        {
            return IGRIDOverallMasterReportHelper.GetMasterData(param);
        }
    }
}