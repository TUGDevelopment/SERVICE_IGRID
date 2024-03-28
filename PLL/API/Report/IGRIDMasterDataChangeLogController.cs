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
    public class IGRIDMasterDataChangeLogController : ApiController
    {
        [Route("api/report/igrid_master_data_report")]
        public IGRID_CBB_DATA_MODEL_RESULT GetMasterDataChangeLog([FromUri]IGRID_CBB_DATA_MODEL_REQUEST param)
        {
            return IGRIDMasterDataChangeLogReportHelper.GetMasterData(param);
        }

        [Route("api/report/igrid_user_master_data_report")]
        public IGRID_CBB_DATA_MODEL_RESULT GetUserMasterDataChangeLog([FromUri]IGRID_CBB_DATA_MODEL_REQUEST param)
        {
            return IGRIDMasterDataChangeLogReportHelper.GetUserMasterData(param);
        }

        [Route("api/report/igrid_master_data_change_log_report")]
        public IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL_RESULT GetMasterDataChangeLogReport([FromUri]IGRID_MASTER_DATA_CHANGE_LOG_REPORT_MODEL_REQUEST param)
        {
            return IGRIDMasterDataChangeLogReportHelper.GetUserMasterChangeLogReport(param);
        }
    }
}