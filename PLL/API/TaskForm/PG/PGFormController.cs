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
    public class PGFormController : ApiController
    {
        [Route("api/taskform/pg/info")]
        public ART_WF_MOCKUP_PROCESS_PG_RESULT GetPGForm([FromUri]ART_WF_MOCKUP_PROCESS_PG_REQUEST param)
        {
            return PGFormHelper.GetPGForm(param);
        }

        [Route("api/taskform/pg/info")]
        public ART_WF_MOCKUP_PROCESS_PG_RESULT PostPGForm(ART_WF_MOCKUP_PROCESS_PG_REQUEST param)
        {
            return PGFormHelper.SavePGForm(param);
        }

        [Route("api/taskform/internal/pg/info")]
        public ART_WF_ARTWORK_PROCESS_PG_RESULT GetSendToPGInfo([FromUri]ART_WF_ARTWORK_PROCESS_PG_REQUEST param)
        {
            return PGFormHelper.GetSendToPGInfo(param);
        }

        [Route("api/taskform/internal/pg/info")]
        public ART_WF_ARTWORK_PROCESS_PG_RESULT POSTSendToMKInfo(ART_WF_ARTWORK_PROCESS_PG_REQUEST param)
        {
            return PGFormHelper.SavePGInfo(param);
        }

        [Route("api/taskform/internal/pg/submit")]
        public ART_WF_ARTWORK_PROCESS_PG_RESULT POSTSubmitPG(ART_WF_ARTWORK_PROCESS_PG_REQUEST param)
        {
            return PGFormHelper.SaveSendToPGInfo(param);
        }

        [Route("api/taskform/internal/pg/copydielinefiles")]
        public ART_WF_ARTWORK_PROCESS_PG_RESULT POSTCopyDielineFile(ART_WF_ARTWORK_PROCESS_PG_REQUEST param)
        {
            //ART_WF_ARTWORK_PROCESS_PG_RESULT Result = new ART_WF_ARTWORK_PROCESS_PG_RESULT();

            //Result.status = "S";
            //Result.msg = "";

            //return Result;
            return PGFormHelper.CopyDielineFileToArtwork(param);
        }


        //---------------------------------------------------ticket# 473360 by aof ----------------------------------------------------------
        [Route("api/taskform/internal/pg/checkdielinefiles")]
        public ART_WF_ARTWORK_PROCESS_PG_RESULT CheckCopyDielineFile(ART_WF_ARTWORK_PROCESS_PG_REQUEST param)
        {
            return PGFormHelper.CheckDielineFileToArtwork(param);
        }
        //---------------------------------------------------ticket# 473360 by aof ----------------------------------------------------------
    }
}
