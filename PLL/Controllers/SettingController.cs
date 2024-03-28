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
    public class SettingController : Controller
    {
        // GET: Setting
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (BLL.Services.CNService.HasPermissionMaster(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), context))
                        return View();

                    if (BLL.Services.CNService.HasPermissionWFFunction(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), context))
                        return View();

                    return View("NoAuth");
                }
            }
        }
    }
}