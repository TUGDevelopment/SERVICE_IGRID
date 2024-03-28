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
    public class SOReportController : ApiController
    {
        [Route("api/report/soreport")]
        public V_ART_ASSIGNED_SO_RESULT GetAssignSOReport([FromUri]V_ART_ASSIGNED_SO_REQUEST param)
        {
            return SOReportHelper.GetAssignSOReport(param);
        }

        [Route("api/report/sapsoreport")]
        public V_SAP_SALES_ORDER_ALL_RESULT GetSAPSOReport([FromUri]V_SAP_SALES_ORDER_ALL_REQUEST param)
        {
            return SOReportHelper.GetSAPSOReport(param);
        }

        [Route("api/report/idocreport")]
        public IDOC_RESULT GetIDOCReport([FromUri]SAP_M_PO_IDOC_REQUEST param)
        {
            return SOReportHelper.GetIDOCReport(param);
        }

        [Route("api/report/mat3report")]
        public XECM_M_PRODUCT_RESULT GetMAT3Report([FromUri]XECM_M_PRODUCT_REQUEST param)
        {
            return SOReportHelper.GetMAT3Report(param);
        }

        [Route("api/report/mat5report")]
        public XECM_M_PRODUCT5_RESULT GetMAT5Report([FromUri]XECM_M_PRODUCT5_REQUEST param)
        {
            return SOReportHelper.GetMAT5Report(param);
        }

        [Route("api/report/syslogreport")]
        public ART_SYS_LOG_RESULT GetSYSLogReport([FromUri]ART_SYS_LOG_REQUEST param)
        {
            return SOReportHelper.GetSYSLogReport(param);
        }

        [Route("api/report/bommaster")]
        public SAP_M_ORDER_BOM_RESULT GetBomMaster([FromUri]SAP_M_ORDER_BOM_REQUEST param)
        {
            return SOReportHelper.GetBomMaster(param);
        }
    }
}
