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
    public class CompanyController : ApiController
    {
        [Route("api/lov/company")]
        public SAP_M_COMPANY_RESULT GetCompany([FromUri]SAP_M_COMPANY_REQUEST param)
        {
            return CompanyHelper.GetCompany(param);
        }

        [Route("api/lov/company_com_code")]
        public SAP_M_COMPANY_RESULT GetCompany_COM_CODE([FromUri]SAP_M_COMPANY_REQUEST param)
        {
            return CompanyHelper.GetCompany_COM_CODE(param);
        }

    }
}
