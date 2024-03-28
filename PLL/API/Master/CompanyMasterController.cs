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
    public class CompanyMasterController : ApiController
    {
        [Route("api/master/company")]
        public ART_M_USER_COMPANY_RESULT PostCheckListRequest(ART_M_USER_COMPANY_REQUEST_LIST param)
        {
            ART_M_USER_COMPANY_RESULT Results = new ART_M_USER_COMPANY_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    foreach (var item in param.data)
                    {
                        var temp = ART_M_USER_COMPANY_SERVICE.GetByItem(new ART_M_USER_COMPANY() { COMPANY_ID = item.COMPANY_ID, USER_ID = item.USER_ID }, context);
                        if (temp.Count == 1)
                        {
                            item.ART_M_USER_COMPANY_ID = temp.FirstOrDefault().ART_M_USER_COMPANY_ID;
                        }

                        if (item.CHECKED)
                        {
                            ART_M_USER_COMPANY_SERVICE.SaveOrUpdate(MapperServices.ART_M_USER_COMPANY(item), context);
                        }
                        else
                        {
                            ART_M_USER_COMPANY_SERVICE.DeleteByART_M_USER_COMPANY_ID(item.ART_M_USER_COMPANY_ID, context);
                        }

                    }
                    Results.status = "S";
                }
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
