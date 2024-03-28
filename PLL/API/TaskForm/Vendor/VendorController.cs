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
    public class VendorController : ApiController
    {

        [Route("api/taskform/vendor/info")]
        public ART_WF_MOCKUP_PROCESS_VENDOR_RESULT GetVendorForm([FromUri]ART_WF_MOCKUP_PROCESS_VENDOR_REQUEST param)
        {
            return VendorHelper.GetVendor(param);
        }

        //http://localhost:60449/api/taskform/vendor/header?data.mockup_id=7&data.mockup_sub_id=7
        //[Route("api/taskform/vendor/header")]
        //public PROCESS_VENDOR_RESULT GetVendorHeader([FromUri]ART_WF_MOCKUP_PROCESS_VENDOR_REQUEST param)
        //{
        //    return VendorHelper.GetVendorHeader(param);
        //}


        //commentted by aof #INC-11265
        //[Route("api/taskform/vendor/info")]
        //public ART_WF_MOCKUP_PROCESS_VENDOR_RESULT PostVendorForm(ART_WF_MOCKUP_PROCESS_VENDOR_REQUEST param)
        //{
        //    return VendorHelper.SaveVendor(param);
        //}

        //rewrited by aof #INC-11265
        [Route("api/taskform/vendor/info")]
        public ART_WF_MOCKUP_PROCESS_VENDOR_RESULT PostVendorForm(ART_WF_MOCKUP_PROCESS_VENDOR_REQUEST_LIST param)
        {
            return VendorHelper.SaveVendor(param);
        }


    }
}
