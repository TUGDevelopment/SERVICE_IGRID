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
    public class SearchDieLineController : ApiController
    {
        [Route("api/taskform/pg/searchdieline")]
        public SEARCH_DIE_LINE_RESULT GetSearchDieLine([FromUri]SEARCH_DIE_LINE_REQUEST param)
        {
            return SearchDieLineHelper.GetDieLine(param);
        }

        [Route("api/taskform/pg/searchdieline2")]
        public V_ART_SEARCH_DIELINE_RESULT GetSearchDieLine2([FromUri]V_ART_SEARCH_DIELINE_REQUEST param)
        {
            return SearchDieLineHelper.GetDieLine2(param);
        }

        [Route("api/taskform/pg/searchdieline_tutuning")]
        public V_ART_SEARCH_DIELINE_RESULT GetSearchDieLine_TUTuning([FromUri]V_ART_SEARCH_DIELINE_REQUEST param)
        {
            return SearchDieLineHelper.GetDieLine_TUTuning(param);
        }
    }
}
