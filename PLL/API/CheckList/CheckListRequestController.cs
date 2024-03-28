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

namespace PLL.API
{
    public class CheckListRequestController : ApiController
    {
        [Route("api/checklist/request")]
        public ART_WF_MOCKUP_CHECK_LIST_RESULT GetCheckListRequest([FromUri]ART_WF_MOCKUP_CHECK_LIST_REQUEST param)
        {
            return CheckListRequestHelper.GetCheckListRequest(param);
        }

        [Route("api/checklist/request")]
        public ART_WF_MOCKUP_CHECK_LIST_RESULT PostCheckListRequest(ART_WF_MOCKUP_CHECK_LIST_REQUEST param)
        {
            var res = CheckListRequestHelper.SaveCheckListRequest(param);

            //#INC-39523 by aof start.
            if (res.status == "E")
            {
                ART_WF_MOCKUP_CHECK_LIST_RESULT Results = new ART_WF_MOCKUP_CHECK_LIST_RESULT();
                Results.data = new List<ART_WF_MOCKUP_CHECK_LIST_2>();
                Results.status = "E";
                Results.msg = res.msg;
                return Results;
            }
            //#INC-39523 by aof end.

            if (!param.data.ENDTASKFORM)
            {
                ART_WF_MOCKUP_CHECK_LIST_RESULT Results = new ART_WF_MOCKUP_CHECK_LIST_RESULT();
                Results.data = new List<ART_WF_MOCKUP_CHECK_LIST_2>();
                Results.status = "S";
                Results.msg = MessageHelper.GetMessage("MSG_001");

                ART_WF_MOCKUP_CHECK_LIST_2 item = new ART_WF_MOCKUP_CHECK_LIST_2();
                item.CHECK_LIST_ID = res.data[0].CHECK_LIST_ID;
                Results.data.Add(item);

                return Results;
            }
            else
            {
                ART_WF_MOCKUP_CHECK_LIST_RESULT Results = new ART_WF_MOCKUP_CHECK_LIST_RESULT();
                Results.data = new List<ART_WF_MOCKUP_CHECK_LIST_2>();
                Results.status = "S";
                return Results;
            }
        }

        [Route("api/checklist/request")]
        public ART_WF_MOCKUP_CHECK_LIST_RESULT DeleteCheckListRequest(ART_WF_MOCKUP_CHECK_LIST_REQUEST param)
        {
            return CheckListRequestHelper.DeleteCheckListRequest(param);
        }
    }
}
