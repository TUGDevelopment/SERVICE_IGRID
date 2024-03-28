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
    public class PriceTemplateController : ApiController
    {
        [Route("api/taskform/pg/pricetemplate")]
        public ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT GetPriceTemplate([FromUri]ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST param)
        {
            return PriceTemplateHelper.GetPriceTemplate(param);
        }

        [Route("api/taskform/pg/pricetemplateforvendor")]
        public ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT GetPriceTemplateForVendor([FromUri]ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST param)
        {
            return PriceTemplateHelper.GetPriceTemplateForVendor(param);
        }
        
        [Route("api/taskform/pg/pricetemplate")]
        public ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT PostPriceTemplate(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST_LIST param)
        {
            return PriceTemplateHelper.SavePriceTemplate(param);
        }
    }
}
