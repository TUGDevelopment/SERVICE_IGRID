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
    public class PriceTemplateCompareController : ApiController
    {
        [Route("api/taskform/pg/pricetemplatecompare")]
        public ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT GetPriceTemplate([FromUri]ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST param)
        {
            return PriceTemplateCompareHelper.GetPriceTemplateCompare(param);
        }

        [Route("api/taskform/pg/pricetemplatemanual")]
        public ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT PostPriceTemplateManual(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST param)
        {
            return PriceTemplateHelper.PostPriceTemplateManual(param);
        }

        [Route("api/taskform/pg/pricetemplatemanualprice")]
        public ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT PostPriceTemplateManualPrice(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST_LIST param)
        {
            return PriceTemplateHelper.PostPriceTemplateManualPrice(param);
        }

        [Route("api/taskform/pg/pricetemplatemanualprice")]
        public ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT DeletePriceTemplateManualPrice(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST param)
        {
            return PriceTemplateHelper.DeletePriceTemplateManualPrice(param);
        }
    }
}
