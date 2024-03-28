using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;
using DAL;

namespace PLL.API
{
    public class MaterialLockReportController : ApiController
    {

        [Route("api/report/materiallockreportv2")]
            public TU_MATERIAL_LOCK_REPORT_MODEL_RESULT GetMaterialLockV2([FromUri]TU_MATERIAL_LOCK_REPORT_MODEL_REQUEST param)
        {
            return MaterialLockReportHelper.GetMaterialLockReportV2(param);
        }

        //public ART_WF_ARTWORK_MATERIAL_LOCK_RESULT GetMaterialLockV2([FromUri]ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST param)
        //{
        //    return MaterialLockReportHelper.GetMaterialLockReportV2(param);
        //}

        [Route("api/report/materiallockreport")]
        public ART_WF_ARTWORK_MATERIAL_LOCK_RESULT GetMaterialLock([FromUri]ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST param)
        {
            return MaterialLockReportHelper.GetMaterialLockReport(param);
        }

        [Route("api/report/materiallockreport")]
        public ART_WF_ARTWORK_MATERIAL_LOCK_RESULT PostMaterialLock(ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST_LIST param)
        {
            return MaterialLockReportHelper.UpdateMaterialLockReport(param);
        }

        [Route("api/report/materiallockreportviwer")]
        public ART_M_USER_ROLE_RESULT GetRoleViwerMaterialLock([FromUri]ART_M_USER_ROLE_REQUEST param)
        {
            return MaterialLockReportHelper.GetRoleViwerMaterialLockReport(param);
        }


    }
}
