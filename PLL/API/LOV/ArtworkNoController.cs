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
    public class ArtworkNoController : ApiController
    {
        [Route("api/lov/artworkno")]
        public ART_WF_ARTWORK_REQUEST_ITEM_RESULT GetArtworkNo([FromUri]ART_WF_ARTWORK_REQUEST_ITEM_REQUEST param)
        {
            return ArtworkNoHelper.GetArtworkNoByMaterialGroupByStoreProcedure(param);   //475099 by aof change query from store procedure
            // return ArtworkNoHelper.GetArtworkNoByMaterialGroup(param);   //475099  by aof  commeted

        }

    }
}
