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
    public class SendToVendorByPPController : ApiController
    {
        [Route("api/taskform/pp/vendor/info")]
        public ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT GetVendor([FromUri]ART_WF_ARTWORK_PROCESS_VENDOR_PO_REQUEST param)
        {
            return VendorByPPHelper.GetVendorByPP(param);
        }

        [Route("api/taskform/pp/vendor/info")]
        public ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT PostVendorSendToPA(ART_WF_ARTWORK_PROCESS_VENDOR_PO_REQUEST param)
        {
            return VendorByPPHelper.PostVendorSendToPA(param);
        }

        [Route("api/taskform/pp/vendor/multiconfirm")]
        public ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT PostMultiVendorSendToPA(ART_WF_ARTWORK_PROCESS_VENDOR_PO_REQUEST_LIST param)
        {
            return VendorByPPHelper.PostMultiVendorSendToPA(param);
        }

        [Route("api/taskform/pp/vendor/porelateartwork")]
        public ART_WF_ARTWORK_PROCESS_VENDOR_PO_RESULT PostPORelateArtwork(ART_WF_ARTWORK_PROCESS_VENDOR_PO_REQUEST_LIST param)
        {
            return VendorByPPHelper.PostPORelateArtwork(param);
        }
    }


}
