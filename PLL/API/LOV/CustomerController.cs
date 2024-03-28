using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;


namespace PLL.API
{
    public class CustomerMasterController : ApiController
    {
        [Route("api/lov/customer")]
        public XECM_M_CUSTOMER_RESULT GetCompany([FromUri] XECM_M_CUSTOMER_REQUEST param)
        {
            return CustomerMasterHelper.GetCustomer(param);
        }
        [Route("api/lov/customerother")]
        public XECM_M_CUSTOMER_RESULT GetCompanyOther([FromUri] XECM_M_CUSTOMER_REQUEST param)
        {
            return CustomerMasterHelper.GetCustomerOther(param);
        }
        [Route("api/lov/customersoldtoso")]
        public XECM_M_CUSTOMER_RESULT GetCompanySoldToSO([FromUri]XECM_M_CUSTOMER_REQUEST param)
        {
            return CustomerMasterHelper.GetCompanySoldToSO(param);
        }

        [Route("api/lov/customershiptoso")]
        public XECM_M_CUSTOMER_RESULT GetCompanyShipToSO([FromUri]XECM_M_CUSTOMER_REQUEST param)
        {
            return CustomerMasterHelper.GetCompanyShipToSO(param);
        }
    }
}
