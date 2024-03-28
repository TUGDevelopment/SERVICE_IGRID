using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;
using WebServices.Helper;

namespace PLL.API
{
    public class POMappingAWController : ApiController
    {
        /*
      * ARTWORK_NO
      */
        [Route("api/taskform/pomappingaw/info")]
        public ART_WF_ARTWORK_MAPPING_PO_RESULT GetPOMappingAW([FromUri]ART_WF_ARTWORK_MAPPING_PO_REQUEST param)
        {
            return POMappingAWHelper.GetPOMappingAW(param);
        }


        /*
         * PO_NO
         * IS_ACTIVE
         */
        [Route("api/taskform/pomappingaw/info")]
        public ART_WF_ARTWORK_MAPPING_PO_RESULT PostPOMappingAW(ART_WF_ARTWORK_MAPPING_PO_REQUEST param)
        {
            return POMappingAWHelper.SavePOMappingAW(param);
        }

    }
}
