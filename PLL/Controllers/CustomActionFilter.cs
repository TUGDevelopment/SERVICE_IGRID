using DAL;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace PLL
{
    public class CustomActionFilter : ActionFilterAttribute, IActionFilter
    {
        //private static bool SkipAuthorization(HttpActionContext actionContext)
        //{
        //    Contract.Assert(actionContext != null);

        //    return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
        //               || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        //}
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            //var homeController = filterContext.Controller as Controller;
            //string actionName = filterContext.ActionDescriptor.ActionName;
            //string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            //bool AllowAnonymousAttribute = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
            //             || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);

            ////filterContext.Result = new RedirectToRouteResult(
            ////    new RouteValueDictionary
            ////    {
            ////                { "controller", "Home" },
            ////                { "action", "Result" }
            ////    });
            //if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            //{
            //    if (controllerName.ToUpper() != "ACCOUNT" && actionName != "LOGIN")
            //    {
            //        string userNamec = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //        //var userName = filterContext.RequestContext.HttpContext.User.Identity.Name;

            //        var log = new ART_SYS_LOG();
            //        log.ACTION = "I";
            //        log.CREATE_BY = -1;
            //        log.CREATE_DATE = DateTime.Now;
            //        log.TABLE_NAME = "Auto Login By";
            //        log.NEW_VALUE = userNamec;
            //        log.UPDATE_BY = -1;
            //        log.UPDATE_DATE = DateTime.Now;
            //        ART_SYS_LOG_SERVICE.SaveNoLog(log);

            //        if (!BLL.Services.CNService.HasUser(userNamec))
            //        {
            //            //var result = new ViewResult
            //            //{
            //            //    ViewName = "~/Views/Shared/NoUser.cshtml",
            //            //};
            //            //filterContext.Result = result;
            //            //base.OnActionExecuting(filterContext);
            //            filterContext.Result = new RedirectToRouteResult(
            //            new RouteValueDictionary
            //            {
            //                        { "controller", "Account" },
            //                        { "action", "Login" }
            //            });
            //            base.OnActionExecuting(filterContext);
            //        }
            //        else
            //        {
            //            FormsAuthentication.SetAuthCookie(userNamec, false);
            //            filterContext.Result = new RedirectToRouteResult(
            //                new RouteValueDictionary
            //                {
            //                        { "controller", "Home" },
            //                        { "action", "Index" }
            //                });
            //            base.OnActionExecuting(filterContext);
            //        }
            //    }
            //}
            //var response = filterContext.HttpContext.Response;

            //response.Write(filterContext.ActionDescriptor.ActionName);
            //response.Write("<br/>");

            //var mockup_sub_id = 0;
            //foreach (var parameter in filterContext.ActionParameters)
            //{
            //    if (parameter.Key == "id")
            //    {
            //        mockup_sub_id = Convert.ToInt32(parameter.Value);
            //    }
            //    //response.Write(string.Format("{0}: {1}", parameter.Key, parameter.Value));
            //}

            //if (mockup_sub_id > 0)
            //    if (ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(mockup_sub_id).IS_END == "X")
            //    {
            //        filterContext.Result = new RedirectResult("/");
            //        return;
            //    }

            //filterContext.Result = new EmptyResult();

            // TODO: Add your action filter's tasks here

            //// Log Action Filter call
            //using (MusicStoreEntities storeDb = new MusicStoreEntities())
            //{
            //    ActionLog log = new ActionLog()
            //    {
            //        Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
            //        Action = string.Concat(filterContext.ActionDescriptor.ActionName, " (Logged By: Custom Action Filter)"),
            //        IP = filterContext.HttpContext.Request.UserHostAddress,
            //        DateTime = filterContext.HttpContext.Timestamp
            //    };
            //    storeDb.ActionLogs.Add(log);
            //    storeDb.SaveChanges();
            //    OnActionExecuting(filterContext);
            //}
        }
    }

    //public class MyAuthorizeAttribute : AuthorizeAttribute
    //{
    //    protected override bool AuthorizeCore(HttpContextBase httpContext)
    //    {
    //        var isAuthorized = false;
    //        var username = httpContext.User.Identity.Name;
    //        // Some code to find the user in the database...
    //        var user = ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { USERNAME = username });
    //        if (user != null)
    //        {
    //            isAuthorized = true;
    //        }


    //        return isAuthorized;
    //    }

    //    public override void OnAuthorization(AuthorizationContext filterContext)
    //    {
    //        if (filterContext == null)
    //        {
    //            throw new ArgumentNullException("filterContext");
    //        }

    //        if (AuthorizeCore(filterContext.HttpContext))
    //        {
    //            SetCachePolicy(filterContext);
    //        }
    //        else
    //        {

    //            var log = new ART_SYS_LOG();
    //            log.ACTION = "I";
    //            log.CREATE_BY = -1;
    //            log.CREATE_DATE = DateTime.Now;
    //            log.TABLE_NAME = "Auto Login Baay";
    //            log.NEW_VALUE = filterContext.HttpContext.User.Identity.Name;
    //            log.UPDATE_BY = -1;
    //            log.UPDATE_DATE = DateTime.Now;
    //            ART_SYS_LOG_SERVICE.SaveNoLog(log);



    //            // If not authorized, redirect to the Login action 
    //            // of the Account controller... 
    //            filterContext.Result = new RedirectToRouteResult(
    //              new System.Web.Routing.RouteValueDictionary {
    //           {"controller", "Account"}, {"action", "Login"}
    //              }
    //            );
    //        }
    //    }

    //    protected void SetCachePolicy(AuthorizationContext filterContext)
    //    {
    //        // ** IMPORTANT **
    //        // Since we're performing authorization at the action level, 
    //        // the authorization code runs after the output caching module. 
    //        // In the worst case this could allow an authorized user 
    //        // to cause the page to be cached, then an unauthorized user would later 
    //        // be served the cached page. We work around this by telling proxies not to 
    //        // cache the sensitive page, then we hook our custom authorization code into 
    //        // the caching mechanism so that we have the final say on whether a page 
    //        // should be served from the cache.
    //        HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
    //        cachePolicy.SetProxyMaxAge(new TimeSpan(0));
    //        cachePolicy.AddValidationCallback(CacheValidationHandler, null /* data */);
    //    }

    //    public void CacheValidationHandler(HttpContext context,
    //                                        object data,
    //                                        ref HttpValidationStatus validationStatus)
    //    {
    //        validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
    //    }
    //}
}