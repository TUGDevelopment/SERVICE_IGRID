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
    public class PriceTemplateVendorController : ApiController
    {
        [Route("api/taskform/pg/pricetemplatevendor")]
        public ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_RESULT GetPriceTemplateVendor([FromUri]ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_REQUEST param)
        {
            return PriceTemplateVendorHelper.GetPriceTemplateVendor(param);
        }

        [Route("api/taskform/pg/pricetemplatevendortran")]
        public ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_RESULT GetPriceTemplateVendorTran([FromUri]ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_REQUEST param)
        {
            return PriceTemplateVendorHelper.GetPriceTemplateVendorTran(param);
        }

        [Route("api/taskform/pg/pricetemplatevendor")]
        public ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_RESULT PostPriceTemplateVendor(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_REQUEST_LIST param)
        {
            return PriceTemplateVendorHelper.SavePriceTemplateVendor(param);
        }

        [Route("api/taskform/pg/selectvendor")]
        public ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT PostSelectVendor(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_REQUEST param)
        {
            return PriceTemplateVendorHelper.PostSelectVendor(param);
        }
    }
}
