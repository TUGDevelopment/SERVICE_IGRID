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
    public class VendorCustomerCollaborationReportController : ApiController
    {
        [Route("api/report/customerartworkreport")]
        public CUST_ARTWORK_REPORT_RESULT GetCustomerArtworkReport([FromUri]CUST_ARTWORK_REPORT_REQUEST param)
        {
            return VendorCustomerCollaborationReportHelper.GetCustomerArtworkReport(param);
        }

        [Route("api/report/customermockupreport")]
        public CUST_MOCKUP_REPORT_RESULT GetCustomerMockupReport([FromUri]CUST_MOCKUP_REPORT_REQUEST param)
        {
            return VendorCustomerCollaborationReportHelper.GetCustomerMockupReport(param);
        }

        [Route("api/report/vendorartworkreport")]
        public VENDOR_ARTWORK_REPORT_RESULT GetVendorArtworkReport([FromUri]VENDOR_ARTWORK_REPORT_REQUEST param)
        {
            return VendorCustomerCollaborationReportHelper.GetVendorArtworkReport(param);
        }

        [Route("api/report/vendormockuppreport")]
        public VENDOR_MOCKUP_REPORT_RESULT GetVendorMockupReport([FromUri]VENDOR_MOCKUP_REPORT_REQUEST param)
        {
            return VendorCustomerCollaborationReportHelper.GetVendorMockupReport(param);
        }
    }
}
