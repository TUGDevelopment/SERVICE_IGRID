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
    public class PGSelectVendorController : ApiController
    {
        [Route("api/taskform/pg/selectvendor_noqua")]
        public ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_RESULT GetPGSelectVendor([FromUri]ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_REQUEST param)
        {
            return SelectVendor.Get(param);
        }

        [Route("api/taskform/pg/selectvendor_noqua_log")]
        public ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_RESULT GetPGSelectVendorLog([FromUri]ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_REQUEST param)
        {
            return SelectVendor.GetLog(param);
        }

        [Route("api/taskform/pg/selectvendor_noqua")]
        public ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_RESULT PostPGSelectVendor(ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_REQUEST_LIST param)
        {
            return SelectVendor.Save(param);
        }
    }
}
