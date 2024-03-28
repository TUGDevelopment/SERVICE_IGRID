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
    public class PackagingSecController : ApiController
    {
        [Route("api/lov/packaging_sec_material")]
        public SAP_M_CHARACTERISTIC_RESULT GetPackaingSecMaterial([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return PackagingSecHelper.GetPackaingSecMaterial(param);
        }

        [Route("api/lov/packaging_sec_plastic_type")]
        public SAP_M_CHARACTERISTIC_RESULT GetPackaingSecPlasticType([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return PackagingSecHelper.GetPackaingSecPlasticType(param);
        }

        [Route("api/lov/packaging_sec_cert_source")]
        public SAP_M_CHARACTERISTIC_RESULT GetPackaingSecCertSource([FromUri]SAP_M_CHARACTERISTIC_REQUEST param)
        {
            return PackagingSecHelper.GetPackaingSecCertSource(param);
        }

    }
}