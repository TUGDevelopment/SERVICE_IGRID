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
    public class ListOfCheckListController : ApiController
    {
        [Route("api/lov/listofchecklist")]
        public ART_WF_MOCKUP_CHECK_LIST_LISTOF_RESULT GetListOfCheckListController([FromUri]ART_WF_MOCKUP_CHECK_LIST_LISTOF_REQUEST param)
        {
            return ListOfCheckListHelper.GetListOfCheckListHelper(param);
        }
    }
}
