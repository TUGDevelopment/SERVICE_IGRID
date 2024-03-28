using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;
using Newtonsoft.Json;
using DAL;

namespace PLL.API
{
    public class UploadRequestController : ApiController
    {
        
        [Route("api/artwork/upload")]
        public ART_WF_ARTWORK_REQUEST_RESULT GetUploadRequest([FromUri]ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            return ArtworkUploadHelper.GetArtworkRequest(param);
        }

        [Route("api/artwork/upload")]
        public ART_WF_ARTWORK_REQUEST_RESULT PostUploadRequest(ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            return ArtworkUploadHelper.SaveUploadRequestForm(param);
        }

        [Route("api/artwork/deleteuploadfile")]
        public ART_WF_ARTWORK_REQUEST_ITEM_RESULT DeleteUploadFile(ART_WF_ARTWORK_REQUEST_ITEM_REQUEST param)
        {
            return ArtworkUploadHelper.DeleteArtworkFile(param);
        }

        [Route("api/artwork/deleteuploadform")]
        public ART_WF_ARTWORK_REQUEST_RESULT DeleteUploadRequestForm(ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            return ArtworkUploadHelper.DeleteArtworkUploadFormOperation(param);
        }

        [Route("api/artwork/submitupload")]
        public ART_WF_ARTWORK_REQUEST_RESULT PostSubmitUploadRequest(ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            var guid = Guid.NewGuid().ToString();
            var USERNAME = "";
            var userId = 0;
            using (ARTWORKEntities context = new ARTWORKEntities())
            {
                userId = CNService.getCurrentUser(context);
                var tempUser = ART_M_USER_SERVICE.GetByUSER_ID(userId, context);
                if (tempUser != null)
                    USERNAME = tempUser.USERNAME;
            }

            ART_WF_ARTWORK_REQUEST_REQUEST tempParam0 = JsonConvert.DeserializeObject<ART_WF_ARTWORK_REQUEST_REQUEST>(JsonConvert.SerializeObject(param));
            ART_WF_ARTWORK_REQUEST_REQUEST tempParam1 = JsonConvert.DeserializeObject<ART_WF_ARTWORK_REQUEST_REQUEST>(JsonConvert.SerializeObject(param));
            ART_WF_ARTWORK_REQUEST_REQUEST tempParam2 = JsonConvert.DeserializeObject<ART_WF_ARTWORK_REQUEST_REQUEST>(JsonConvert.SerializeObject(param));
            ART_WF_ARTWORK_REQUEST_REQUEST tempParam3 = JsonConvert.DeserializeObject<ART_WF_ARTWORK_REQUEST_REQUEST>(JsonConvert.SerializeObject(param));

            var res = ArtworkUploadHelper.SubmitUploadRequestForm(tempParam0);
            //SaveLog(res.status, 0.ToString(), userId, USERNAME, guid);

            if (res.status == "E")
            {
                Random random = new Random();
                var sleep = random.Next(500, 2000);
                System.Threading.Thread.Sleep(sleep);

                res = ArtworkUploadHelper.SubmitUploadRequestForm(tempParam1);
                //SaveLog(res.status, 1.ToString(), userId, USERNAME, guid);
            }
            if (res.status == "E")
            {
                Random random = new Random();
                var sleep = random.Next(500, 2000);
                System.Threading.Thread.Sleep(sleep);

                res = ArtworkUploadHelper.SubmitUploadRequestForm(tempParam2);
                //SaveLog(res.status, 2.ToString(), userId, USERNAME, guid);
            }
            if (res.status == "E")
            {
                Random random = new Random();
                var sleep = random.Next(500, 2000);
                System.Threading.Thread.Sleep(sleep);

                res = ArtworkUploadHelper.SubmitUploadRequestForm(tempParam3);
                SaveLog(res.status, 3.ToString(), userId, USERNAME, guid);
            }
            return res;
        }

        public void SaveLog(string status, string round, int userId, string USERNAME, string guid)
        {
            using (ARTWORKEntities context2 = new ARTWORKEntities())
            {
                ART_SYS_LOG error = new ART_SYS_LOG();
                error.CREATE_BY = userId;
                error.UPDATE_BY = userId;
                error.NEW_VALUE = USERNAME;
                error.OLD_VALUE = guid;
                if (status == "S") error.ERROR_MSG = "Rerun PostSubmitUploadRequest Completed. (" + round + ")";
                if (status == "E") error.ERROR_MSG = "Rerun PostSubmitUploadRequest Fail. (" + round + ")";
                error.TABLE_NAME = "Function GetErrorMessage [CNService]";
                error.ACTION = "E";
                ART_SYS_LOG_SERVICE.SaveNoLog(error, context2);
            }
        }
    }
}
