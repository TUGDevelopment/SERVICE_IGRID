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
    public class UpperLevelController : ApiController
    {
        [Route("api/master/upperlevel")]
        public ART_M_USER_UPPER_LEVEL_RESULT PostCheckListRequest(ART_M_USER_UPPER_LEVEL_REQUEST_LIST param)
        {
            ART_M_USER_UPPER_LEVEL_RESULT Results = new ART_M_USER_UPPER_LEVEL_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    foreach (var item in param.data)
                    {
                        var temp = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { UPPER_USER_ID = item.UPPER_USER_ID, USER_ID = item.USER_ID }, context);
                        if (temp.Count == 1)
                        {
                            item.ART_M_USER_UPPER_LEVEL_ID = temp.FirstOrDefault().ART_M_USER_UPPER_LEVEL_ID;
                        }

                        if (item.CHECKED)
                        {
                            ART_M_USER_UPPER_LEVEL_SERVICE.SaveOrUpdate(MapperServices.ART_M_USER_UPPER_LEVEL(item), context);
                        }
                        else
                        {
                            ART_M_USER_UPPER_LEVEL_SERVICE.DeleteByART_M_USER_UPPER_LEVEL_ID(item.ART_M_USER_UPPER_LEVEL_ID, context);
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
