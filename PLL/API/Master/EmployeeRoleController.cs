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
    public class UsersRoleController : ApiController
    {
        [Route("api/master/employeerole")]
        public ART_M_USER_ROLE_RESULT PostCheckListRequest(ART_M_USER_ROLE_REQUEST_LIST param)
        {
            ART_M_USER_ROLE_RESULT Results = new ART_M_USER_ROLE_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    foreach (var item in param.data)
                    {
                        var temp = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { ROLE_ID = item.ROLE_ID, USER_ID = item.USER_ID }, context);
                        if (temp.Count == 1)
                        {
                            item.USER_ROLE_ID = temp.FirstOrDefault().USER_ROLE_ID;
                        }

                        if (item.CHECKED)
                        {
                            ART_M_USER_ROLE_SERVICE.SaveOrUpdate(MapperServices.ART_M_USER_ROLE(item), context);
                        }
                        else
                        {
                            ART_M_USER_ROLE_SERVICE.DeleteByUSER_ROLE_ID(item.USER_ROLE_ID, context);
                        }
                    }
                }
                Results.status = "S";
                //Results.msg = MessageHelper.GetMessage("MSG_001");
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
    }
}
