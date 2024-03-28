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
    public class CustomerController : ApiController
    {

        [Route("api/taskform/customer/info")]
        public ART_WF_MOCKUP_PROCESS_CUSTOMER_RESULT GetCustomerForm([FromUri]ART_WF_MOCKUP_PROCESS_CUSTOMER_REQUEST param)
        {
            return CustomerHelper.GetCustomer(param);
        }

        //http://localhost:60449/api/taskform/vendor/header?data.mockup_id=7&data.mockup_sub_id=7
        //[Route("api/taskform/vendor/header")]
        //public PROCESS_VENDOR_RESULT GetVendorHeader([FromUri]ART_WF_MOCKUP_PROCESS_VENDOR_REQUEST param)
        //{
        //    return VendorHelper.GetVendorHeader(param);
        //}


        [Route("api/taskform/customer/info")]
        public ART_WF_MOCKUP_PROCESS_CUSTOMER_RESULT PostCustomerForm(ART_WF_MOCKUP_PROCESS_CUSTOMER_REQUEST param)
        {
            return CustomerHelper.SaveCustomer(param);
        }
    }
}
