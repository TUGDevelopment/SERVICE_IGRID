using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;

namespace PLL.API.TaskForm.PA
{
    public class PASendToCustomerController : ApiController
    {
        [Route("api/taskform/pa/sendtocustomer")]//cr 150109
        public ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_RESULT PostSendToCustomer(ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_REQUEST param)
        {
            return CustomerByPAHelper.SaveCustomerByPA(param);
        }

        [Route("api/taskform/pa/sendtocustomer")]
        public ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT GetSendToCustomer([FromUri]ART_WF_ARTWORK_PROCESS_CUSTOMER_REQUEST param)
        {
            return CustomerByPAHelper.GetCustomerByPA(param);
        }

        [Route("api/taskform/pa/sendtocustomerreqref")]
        public ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT GetCustomerReviewForReqRef([FromUri]ART_WF_ARTWORK_PROCESS_CUSTOMER_REQUEST param)
        {
            return CustomerByPAHelper.GetCusReq(param);
        }

        [Route("api/taskform/pa/customer/info")]
        public ART_WF_ARTWORK_PROCESS_CUSTOMER_RESULT PostCustomerSendToPA(ART_WF_ARTWORK_PROCESS_CUSTOMER_REQUEST param)
        {
            return CustomerByPAHelper.PostCustomerSendToPA(param);
        }
    }
}
