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
    public class ListOfArtworkController : ApiController
    {
        [Route("api/lov/listofartwork")]
        public ART_WF_ARTWORK_REQUEST_LISTOF_RESULT GetListOfCheckListController([FromUri]ART_WF_ARTWORK_REQUEST_LISTOF_REQUEST param)
        {
            return ListOfArtworkHelper.GetListOfArtworkHelper(param);
        }
    }
}
