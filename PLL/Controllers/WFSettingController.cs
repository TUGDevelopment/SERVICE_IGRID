using BLL.Services;
using DAL;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PLL.Controllers
{
    public class WFSettingController : Controller
    {
        // GET: Setting
        public ActionResult Reassign()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "REASSIGN", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult Reopen()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "REOPEN", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult Recall()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "RECALL", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult Reassign2()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "CHANGEOWNER", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }
    }
}