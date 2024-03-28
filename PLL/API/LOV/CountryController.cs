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
    public class CountryController : ApiController
    {
        [Route("api/lov/country")]
        public SAP_M_COUNTRY_RESULT GetCountry([FromUri]SAP_M_COUNTRY_REQUEST param)
        {
            return CountryHelper.GetCountry(param);
        }

        [Route("api/lov/pic/country")]
        public SAP_M_COUNTRY_RESULT GetCountryPIC([FromUri]SAP_M_COUNTRY_REQUEST param)
        {
            return CountryHelper.GetCountryPIC(param);
        }

        [Route("api/lov/countryso")]
        public SAP_M_COUNTRY_RESULT GetCountrySO([FromUri]SAP_M_COUNTRY_REQUEST param)
        {
            return CountryHelper.GetCountrySO(param);
        }
    }
}
