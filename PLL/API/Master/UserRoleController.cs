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
    public class UserRoleController : ApiController
    {
        [Route("api/master/userrole")]
        public ART_M_USER_ROLE_RESULT GetUserRole([FromUri]ART_M_USER_ROLE_REQUEST param)
        {
            param = new ART_M_USER_ROLE_REQUEST();
            param.data = new ART_M_USER_ROLE_2();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    param.data.USER_ID = CNService.getCurrentUser(context);
                }
            }
            return UserRoleHelper.GetUserRole(param);
        }
    }
}
