using BLL.Helpers;
using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PLL.API
{
    public class LogsController : ApiController
    {
        [Route("api/system/logs")]
        public ART_SYS_LOG_RESULT GetLogs([FromUri]ART_SYS_LOG_REQUEST param)
        {
            ART_SYS_LOG_RESULT Results = new ART_SYS_LOG_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            //Results.data = MapperServices.SAP_M_COMPANY(SAP_M_COMPANY_SERVICE.GetAll());
                        }
                        else
                        {
                            Results.data = MapperServices.ART_SYS_LOG(ART_SYS_LOG_SERVICE.GetByItem(MapperServices.ART_SYS_LOG(param.data), context));
                            Results.data = Results.data.OrderByDescending(i => i.CREATE_DATE).Skip(param.start).Take(param.length).ToList();
                        }

                        Results.recordsFiltered = Results.data.ToList().Count;
                        Results.recordsTotal = Results.data.ToList().Count;
                        Results.status = "S";
                    }
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
