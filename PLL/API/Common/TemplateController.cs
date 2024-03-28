using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BLL.Services;
using BLL.Helpers;
using DAL;
using DAL.Model;

namespace PLL.API.Common
{
    public class TemplateController : ApiController
    {
        [Route("api/common/template")]
        public ART_M_TEMPLATE_RESULT GetTemplate([FromUri]ART_M_TEMPLATE_REQUEST param)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    ART_M_TEMPLATE_RESULT res = new ART_M_TEMPLATE_RESULT();

                    if (param == null || param.data == null)
                        res.data = MapperServices.ART_M_TEMPLATE(ART_M_TEMPLATE_SERVICE.GetAll(context));
                    else
                        res.data = MapperServices.ART_M_TEMPLATE(ART_M_TEMPLATE_SERVICE.GetByItem(MapperServices.ART_M_TEMPLATE(param.data), context));

                    res.data = res.data.Where(m => m.IS_ACTIVE == "X").ToList();
                    foreach (var item in res.data)
                    {
                        item.ID = item.TEMPLATE_ID;
                        item.DISPLAY_TXT = item.TEMPLATE_NAME;
                    }

                    if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                    {
                        res.data = (from u1 in res.data
                                    where u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                    select u1).ToList();
                    }

                    res.status = "S";
                    return res;
                }
            }
        }
    }
}
