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
    public class WarehouseReportController : ApiController
    {

        [Route("api/report/warehousereportv2")]
        public V_ART_WAREHOUSE_REPORT_RESULT GetWarehouseReportV2([FromUri]V_ART_WAREHOUSE_REPORT_REQUEST param)
        //public List<V_ART_WAREHOUSE_REPORT_2> GetWarehouseReportV2([FromUri]V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
            return WarehouseReportHelper.GetWarehouseReportV2(param);
        }

        [Route("api/report/warehousereport")]
        public V_ART_WAREHOUSE_REPORT_RESULT GetWarehouseReport([FromUri]V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
           return WarehouseReportHelper.GetWarehouseReport(param);
        }

        [Route("api/report/warehousereport_soatt")]
        public V_ART_WAREHOUSE_REPORT_RESULT GetWarehouseReport_SOAtt([FromUri]V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
            return WarehouseReportHelper.GetWarehouseReport_SOAtt(param);
        }

        [Route("api/report/warehousereport_poatt")]
        public V_ART_WAREHOUSE_REPORT_RESULT GetWarehouseReport_POAtt([FromUri]V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
            return WarehouseReportHelper.GetWarehouseReport_POAtt(param);
        }

        [Route("api/report/warehousereport_awatt")]
        public V_ART_WAREHOUSE_REPORT_RESULT GetWarehouseReport_AWAtt([FromUri]V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
            return WarehouseReportHelper.GetWarehouseReport_AWAtt(param);
        }
    }
}
