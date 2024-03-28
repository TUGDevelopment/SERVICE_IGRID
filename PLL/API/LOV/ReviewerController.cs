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
    public class ReviewerController : ApiController
    {
        [Route("api/lov/reviewer")]
        public ART_M_USER_RESULT GetReviewer([FromUri]ART_M_USER_REQUEST param)
        {
            return ReviewerHelper.GetReviewerHelper(param);
        }

        [Route("api/lov/reviewer_ffc")]
        public ART_M_USER_RESULT GetReviewerFFC([FromUri]ART_M_USER_REQUEST param)
        {
            return ReviewerHelper.GetReviewerHelperFFC(param);
        }

        [Route("api/lov/reviewer_tholding")]
        public ART_M_USER_RESULT GetReviewerTHOLDING([FromUri]ART_M_USER_REQUEST param)
        {
            return ReviewerHelper.GetReviewerHelperTHolding(param);
        }
    }
}
