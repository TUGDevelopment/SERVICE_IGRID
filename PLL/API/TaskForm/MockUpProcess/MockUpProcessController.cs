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
    public class MockUpProcessController : ApiController
    {
        [Route("api/taskform/mockupprocess/routing")]
        public ART_WF_MOCKUP_PROCESS_RESULT PostProcessSubmit(ART_WF_MOCKUP_PROCESS_REQUEST_LIST param)
        {
            return MockUpProcessHelper.SubmitProcess(param);
        }

        [Route("api/taskform/mockupprocess/routingandpricetemplate")]
        public ART_WF_MOCKUP_PROCESS_RESULT PostProcessSubmit2(ART_WF_MOCKUP_PROCESS_REQUEST_LIST param)
        {
            return MockUpProcessHelper.SubmitProcessAndPriceTemplate(param);
        }

        [Route("api/taskform/mockupprocess/checklist")]
        public ART_WF_MOCKUP_PROCESS_RESULT PostMockUpProcessForCheckList(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            //for check list request form
            return MockUpProcessHelper.SaveProcess(param);
        }

        [Route("api/taskform/mockupprocess/accepttask")]
        public ART_WF_MOCKUP_PROCESS_RESULT AcceptTask(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            return MockUpProcessHelper.AcceptTask(param);
        }

        [Route("api/taskform/mockupprocess/process")]
        public ART_WF_MOCKUP_PROCESS_RESULT GetProcess([FromUri]ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            return MockUpProcessHelper.GetProcess(param);
        }

        [Route("api/taskform/mockupprocess/endtask")]
        public ART_WF_MOCKUP_PROCESS_RESULT PostEndTaskForm(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            return MockUpProcessHelper.EndTaskForm(param);
        }

        //[Route("api/taskform/mockupprocess/mksendtowh")]
        //public ART_WF_MOCKUP_PROCESS_RESULT PostMKSendToWH(ART_WF_MOCKUP_PROCESS_REQUEST param)
        //{
        //    return MockUpProcessHelper.PostMKSendToWH(param);
        //}

        [Route("api/taskform/mockupprocess/pgsendbackmk")]
        public ART_WF_MOCKUP_PROCESS_RESULT PostPGSendBackMK(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            return MockUpProcessHelper.PostPGSendBackMK(param);
        }

        [Route("api/taskform/mockupprocess/pgcompletewf")]
        public ART_WF_MOCKUP_PROCESS_RESULT PostPGCompleteWF(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            return MockUpProcessHelper.PostPGCompleteWF(param);
        }

        [Route("api/taskform/mockupprocess/terminatewfmockup")]
        public ART_WF_MOCKUP_PROCESS_RESULT PostTerminateWFMockup(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            return MockUpProcessHelper.PostTerminateWFMockup(param);
        }

        [Route("api/taskform/mockupprocess/killwfmockup")]
        public ART_WF_MOCKUP_PROCESS_RESULT PostKillWFMockup(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            return MockUpProcessHelper.PostKillWFMockup(param);
        }

        [Route("api/taskform/mockupprocess/stepdurationextend")]
        public ART_WF_MOCKUP_PROCESS_RESULT PostStepDurationExtend(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            return MockUpProcessHelper.SaveStepDurationExtend(param);
        }
    }
}
