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
    public class CheckListCreatorController : ApiController
    {
        [Route("api/lov/checklistcreator")]
        public ART_M_USER_RESULT GetCheckListCreator([FromUri]ART_M_USER_REQUEST param)
        {
            return CheckListCreatorHelper.GetCheckListCreator(param);
        }
    }
}
