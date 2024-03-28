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
    public class CheckListMailToCustomerController : ApiController
    {
        [Route("api/checklist/MailToCustomer")]
        public ART_M_USER_RESULT GetCheckListMailToCustomer([FromUri]ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_REQUEST param) //
        {
            return CheckListCustomerOtherHelper.GetCheckListMailCustomerOtherUser(param);
        }

        [Route("api/checklist/MailToCCFFC")]
        public ART_M_USER_RESULT GetCheckListMailToFFC([FromUri]ART_M_USER_REQUEST param) //
        {
            return CheckListCustomerOtherHelper.MailToCCFFC(param);
        }

        [Route("api/checklist/MailToCCTHolding")]
        public ART_M_USER_RESULT GetCheckListMailToTHolding([FromUri]ART_M_USER_REQUEST param) //
        {
            return CheckListCustomerOtherHelper.MailToCCTHolding(param);
        }
    }
}
