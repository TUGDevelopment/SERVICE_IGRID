//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;
//using DAL.Model;
//using BLL.Services;
//using BLL.Helpers;

//namespace PLL.API.TaskForm.PG
//{
//    public class SendToRDController : ApiController
//    {
//        [Route("api/taskform/pg/sendtord")]
//        public ART_WF_MOCKUP_PROCESS_RD_BY_PG_RESULT PostSendToRD(ART_WF_MOCKUP_PROCESS_RD_BY_PG_REQUEST param)
//        {
//            return RDByPGHelper.SaveRDByPG(param);
//        }

//        [Route("api/taskform/pg/sendtord")]
//        public ART_WF_MOCKUP_PROCESS_RD_BY_PG_RESULT GetSendToRD([FromUri]ART_WF_MOCKUP_PROCESS_RD_BY_PG_REQUEST param)
//        {
//            return RDByPGHelper.GetRDByPG(param);
//        }
//    }
//}
