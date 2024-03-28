using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;
using System.Web.Script.Serialization;

namespace PLL.API
{
    public class ArtworkProcessController : ApiController
    {
        [Route("api/taskform/artworkprocess/accepttask")]
        public ART_WF_ARTWORK_PROCESS_RESULT AcceptTask(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            return ArtworkProcessHelper.AcceptTask(param); 
        }

        [Route("api/taskform/artworkprocess/process")]
        public ART_WF_ARTWORK_PROCESS_RESULT GetProcess([FromUri]ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            return ArtworkProcessHelper.GetProcess(param);
        }

        [Route("api/taskform/artworkprocess/endtaskpg")]
        public ART_WF_ARTWORK_PROCESS_RESULT PostEndTaskFormPG(ART_WF_ARTWORK_PROCESS_PG_REQUEST param)
        {
            return ArtworkProcessHelper.EndTaskFormPG(param);
        }

        [Route("api/taskform/artworkprocess/stepdurationextend")]
        public ART_WF_ARTWORK_PROCESS_RESULT PostStepDurationExtend(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            return ArtworkProcessHelper.SaveStepDurationExtend(param);
        }
    }
}
