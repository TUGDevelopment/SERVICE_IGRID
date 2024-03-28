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
    public class RDPersonController : ApiController
    {
        [Route("api/lov/rdperson")]
        public ART_M_USER_RESULT GetRDPerson([FromUri]ART_M_USER_REQUEST param)
        {
            return RDPersonHelper.GetRDPerson(param);
        }

        [Route("api/lov/rdperson_ffc")]
        public ART_M_USER_RESULT GetRDPersonFFC([FromUri]ART_M_USER_REQUEST param)
        {
            return RDPersonHelper.GetRDPerson(param);
        }

        [Route("api/lov/rdperson_tholding")]
        public ART_M_USER_RESULT GetRDPersonTHolding([FromUri]ART_M_USER_REQUEST param)
        {
            return RDPersonHelper.GetRDPerson(param);
        }
    }
}
