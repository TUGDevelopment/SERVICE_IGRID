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
using Newtonsoft.Json;

namespace PLL.API
{
    public class ArtworkRequestController : ApiController
    {
        [Route("api/artwork/artworkrequest")]
        public ART_WF_ARTWORK_REQUEST_RESULT GetArtworkRequest([FromUri]ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            return ArtworkUploadHelper.GetArtworkRequest(param);
        }

        //GetArtworkRequestByArtworkRequestNo
        [Route("api/artwork/artworkrequestbyartworkrequestno")]
        public ART_WF_ARTWORK_REQUEST_RESULT GetArtworkRequestByArtworkRequestNo([FromUri]ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            return ArtworkUploadHelper.GetArtworkRequestByArtworkRequestNo(param);
        }

        [Route("api/artwork/artworkrequest")]
        public ART_WF_ARTWORK_REQUEST_RESULT PostArtworkRequest(ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            return ArtworkUploadHelper.SaveArtworkRequest(param);
        }

        [Route("api/artwork/submitrequest")]
        public ART_WF_ARTWORK_REQUEST_RESULT PostSubmitArtworkRequest(ART_WF_ARTWORK_REQUEST_REQUEST param)
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

            var res = ArtworkRequestHelper.SubmitArtworkRequest(tempParam0, true);
            //SaveLog(res.status, 0.ToString(), userId, USERNAME, guid);

            if (res.status == "E")
            {
                Random random = new Random();
                var sleep = random.Next(500, 2000);
                System.Threading.Thread.Sleep(sleep);

                res = ArtworkRequestHelper.SubmitArtworkRequest(tempParam1, true);
                //SaveLog(res.status, 1.ToString(), userId, USERNAME, guid);
            }
            if (res.status == "E")
            {
                Random random = new Random();
                var sleep = random.Next(500, 2000);
                System.Threading.Thread.Sleep(sleep);

                res = ArtworkRequestHelper.SubmitArtworkRequest(tempParam2, true);
                //SaveLog(res.status, 2.ToString(), userId, USERNAME, guid);
            }
            if (res.status == "E")
            {
                Random random = new Random();
                var sleep = random.Next(500, 2000);
                System.Threading.Thread.Sleep(sleep);

                res = ArtworkRequestHelper.SubmitArtworkRequest(tempParam3, true);
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
                if (status == "S") error.ERROR_MSG = "Rerun PostSubmitArtworkRequest Completed. (" + round + ")";
                if (status == "E") error.ERROR_MSG = "Rerun PostSubmitArtworkRequest Fail. (" + round + ")";
                error.TABLE_NAME = "Function GetErrorMessage [CNService]";
                error.ACTION = "E";
                ART_SYS_LOG_SERVICE.SaveNoLog(error, context2);
            }
        }

        [Route("api/artwork/deletefilerequest")]
        public ART_WF_ARTWORK_REQUEST_ITEM_RESULT DeleteFileArtworkRequest(ART_WF_ARTWORK_REQUEST_ITEM_REQUEST param)
        {
            return ArtworkRequestHelper.DeleteFileArtworkRequest(param);
        }

        [Route("api/artwork/deleteartworkrequest")]
        public ART_WF_ARTWORK_REQUEST_RESULT DeleteArtworkRequest(ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            return ArtworkRequestHelper.DeleteArtworkRequest(param);
        }

        [Route("api/artwork/salesorderrequest")]
        public SALES_ORDER_REQUEST_FORM_RESULT CheckSalesOrderRequest(SALES_ORDER_REQUEST_FORM_REQUEST_LIST param)
        {
            return ArtworkRequestHelper.CheckSalesOrderRequest(param);
        }

    }
}
