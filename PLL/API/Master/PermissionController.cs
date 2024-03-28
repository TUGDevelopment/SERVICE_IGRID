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
    public class PermissionController : ApiController
    {
        [Route("api/permission/info")]
        public ART_M_PERMISSION_RESULT GetPermissionInfo([FromUri]ART_M_PERMISSION_REQUEST param)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    ART_M_PERMISSION_RESULT res = new ART_M_PERMISSION_RESULT();
                    res.data = MapperServices.ART_M_PERMISSION(ART_M_PERMISSION_SERVICE.GetByItem(new ART_M_PERMISSION() { ROLE_ID = param.data.ROLE_ID }, context));

                    res.status = "S";
                    return res;
                }
            }
        }

        [Route("api/permission/info")]
        public ART_M_PERMISSION_RESULT PostPermissionInfo(ART_M_PERMISSION_REQUEST param)
        {
            ART_M_PERMISSION_RESULT res = new ART_M_PERMISSION_RESULT();
            using (var context = new ARTWORKEntities())
            {
                var chk = ART_M_PERMISSION_SERVICE.GetByItem(new ART_M_PERMISSION() { ROLE_ID = param.data.ROLE_ID, PERMISSION_CODE = param.data.PERMISSION_CODE }, context).FirstOrDefault();

                if (param.data.ACTION == "D")
                {
                    ART_M_PERMISSION_SERVICE.DeleteByPERMISSION_ID(chk.PERMISSION_ID, context);
                }
                else if (param.data.ACTION == "I")
                {
                    ART_M_PERMISSION_SERVICE.SaveOrUpdate(MapperServices.ART_M_PERMISSION(param.data), context);
                }
            }
            res.status = "S";
            return res;
        }
    }
}
