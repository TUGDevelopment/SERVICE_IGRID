using BLL.Services;
using DAL;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PLL.Controllers
{
    public class CheckListController : Controller
    {
        public ActionResult Index(int? id)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var UserID2 = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    if (BLL.Services.CNService.IsCustomer(UserID2, context))
                        return View("NoAuth");
                    if (BLL.Services.CNService.IsVendor(UserID2, context))
                        return View("NoAuth");

                    ViewBag.FFCDefaultReviewer_UserID = ConfigurationManager.AppSettings["FFCDefaultReviewer_UserID"];
                    ViewBag.FFCDefaultReviewer_UserName = CNService.GetUserName(Convert.ToInt32(ViewBag.FFCDefaultReviewer_UserID), context);
                    ViewBag.FFCDefaultEmailTo_UserID = ConfigurationManager.AppSettings["FFCDefaultEmailTo_UserID"];
                    ViewBag.FFCDefaultEmailTo_UserName = CNService.GetUserName(Convert.ToInt32(ViewBag.FFCDefaultEmailTo_UserID), context);
                    ViewBag.FFCDefaultEmailCC_UserID = ConfigurationManager.AppSettings["FFCDefaultEmailCC_UserID"];
                    ViewBag.FFCDefaultEmailCC_UserName = CNService.GetUserName(Convert.ToInt32(ViewBag.FFCDefaultEmailCC_UserID), context);

                    ViewBag.CheckListID = id == null ? 0 : id;

                    return View();
                }
            }
        }
    }
}