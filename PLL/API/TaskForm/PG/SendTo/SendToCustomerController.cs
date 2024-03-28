using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;

namespace PLL.API.TaskForm.PG
{
    public class SendToCustomerController : ApiController
    {
        [Route("api/taskform/pg/sendtocustomer")]
        public ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_RESULT PostSendToCustomer(ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_REQUEST_LIST param)
        {
            return CustomerByPGHelper.SaveCustomerByPG(param);
        }

        [Route("api/taskform/pg/sendtocustomer")]
        public ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_RESULT GetSendToCustomer([FromUri]ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_REQUEST param)
        {
            return CustomerByPGHelper.GetCustomerByPG(param);
        }
    }
}
