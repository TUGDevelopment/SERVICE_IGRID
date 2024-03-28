using BLL.Services;
using DAL;
using DAL.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Script.Serialization;

namespace PLL
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            config.Filters.Add(new YourFilterAttribute()); // add record
            config.Filters.Add(new AuthorizeAttribute());
        }

        public class YourFilterAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(HttpActionContext actionContext)
            {
                if (actionContext.Request.Method.Method.ToUpper() != "GET")
                {
                    REQUEST_MODEL param = (REQUEST_MODEL)actionContext.ActionArguments["param"];
                    if (param != null && param.MOCKUP_SUB_ID_CHECK > 0)
                    {
                        using (var context = new ARTWORKEntities())
                        {
                            using (CNService.IsolationLevel(context))
                            {
                                var isEnd = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(param.MOCKUP_SUB_ID_CHECK, context).IS_END;
                                if (isEnd == "X")
                                {
                                    RESULT_MODEL model = new RESULT_MODEL();
                                    model.msg = "This WF already processed by another user.<br/>Please refresh your web browser.";
                                    model.status = "E";
                                    if (CNService.IsEncryptJson())
                                    {
                                        var str = CNService.Serialize(model);
                                        var de = EncryptionService.encoding(str);
                                        RESULT_ENCRYPTION_MODEL model2 = new RESULT_ENCRYPTION_MODEL();
                                        model2.str = de.Insert(10, "!");
                                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, model2);
                                    }
                                    else
                                    {
                                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, model);
                                    }
                                }
                            }
                        }
                    }

                    if (param != null && param.ARTWORK_SUB_ID_CHECK > 0)
                    {
                        using (var context = new ARTWORKEntities())
                        {
                            using (CNService.IsolationLevel(context))
                            {
                                var isEnd = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.ARTWORK_SUB_ID_CHECK, context).IS_END;
                                if (isEnd == "X")
                                {
                                    RESULT_MODEL model = new RESULT_MODEL();
                                    model.msg = "This WF already processed by another user.<br/>Please refresh your web browser.";
                                    model.status = "E";
                                    if (CNService.IsEncryptJson())
                                    {
                                        var str = CNService.Serialize(model);
                                        var de = EncryptionService.encoding(str);
                                        RESULT_ENCRYPTION_MODEL model2 = new RESULT_ENCRYPTION_MODEL();
                                        model2.str = de.Insert(10, "!");
                                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, model2);
                                    }
                                    else
                                    {
                                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, model);
                                    }
                                }

                                if (actionContext.Request.RequestUri.AbsolutePath.ToLower().Contains("taskform/pa/killprocess"))
                                {
                                }
                                else
                                {
                                    if (CNService.IsLock(param.ARTWORK_SUB_ID_CHECK, context))
                                    {
                                        RESULT_MODEL model = new RESULT_MODEL();
                                        model.msg = "This workitem is locked. Please contact your PA supervisor.<br/>Please refresh your web browser.";
                                        model.status = "E";

                                        if (CNService.IsEncryptJson())
                                        {
                                            var str = CNService.Serialize(model);
                                            var de = EncryptionService.encoding(str);
                                            RESULT_ENCRYPTION_MODEL model2 = new RESULT_ENCRYPTION_MODEL();
                                            model2.str = de.Insert(10, "!");
                                            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, model2);
                                        }
                                        else
                                        {
                                            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.OK, model);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
            {
                if (CNService.IsEncryptJson())
                {
                    var objectContent = actionExecutedContext.Response.Content as ObjectContent;
                    if (objectContent != null)
                    {
                        var type = objectContent.ObjectType; //type of the returned object
                        var value = objectContent.Value; //holding the returned value
                        var str = CNService.Serialize(value);
                        var de = EncryptionService.encoding(str);
                        RESULT_ENCRYPTION_MODEL model = new RESULT_ENCRYPTION_MODEL();
                        model.str = de.Insert(10, "!");
                        actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, model);
                    }
                }
            }
        }
    }
}
