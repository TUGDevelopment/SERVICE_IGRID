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
    public class UserController : ApiController
    {
        [Route("api/lov/user")]
        public ART_M_USER_RESULT GetUser([FromUri]ART_M_USER_REQUEST param)
        {
            return UserHelper.GetUser(param);
        }

        [Route("api/lov/userwithparam")]
        public ART_M_USER_RESULT GetUserWithParam([FromUri]ART_M_USER_REQUEST param)
        {
            return UserHelper.GetUserWithParam(param);
        }

        [Route("api/lov/userpic")]
        public ART_M_USER_RESULT GetUserPIC([FromUri]ART_M_USER_REQUEST param)
        {
            return UserHelper.GetUserPIC(param);
        }

        [Route("api/lov/userpicso")]
        public ART_M_USER_RESULT GetUserPICSO([FromUri]ART_M_USER_REQUEST param)
        {
            return UserHelper.GetUserPICSO(param);
        }

        [Route("api/lov/userreassign")]
        public ART_M_USER_RESULT GetUserReassign([FromUri]ART_M_USER_REQUEST param)
        {
            return UserHelper.GetUserReassign(param);
        }

        [Route("api/lov/userpackaging")]
        public ART_M_USER_RESULT GetUserPackaging([FromUri]ART_M_USER_REQUEST param)
        {
            return UserHelper.GetUserPackaging(param);
        }
    }
}
