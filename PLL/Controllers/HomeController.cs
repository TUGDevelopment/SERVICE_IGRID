using BLL.Services;
using DAL;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL.Model;
using PLL.Models;

namespace PLL.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                var log = new ART_SYS_LOG();
                var dat = DateTime.Now;
                log.TABLE_NAME = "";
                log.CREATE_BY = -1;
                log.UPDATE_BY = -1;
                log.CREATE_DATE = dat;
                log.UPDATE_DATE = dat;
                log.ACTION = "Open Dashboard";
                log.NEW_VALUE = User.Identity.GetUserName();
                log.OLD_VALUE = Request.UserHostAddress + "-" + Request.Browser.Browser + "-" + Request.Browser.Version;
                ART_SYS_LOG_SERVICE.SaveNoLog(log, context);

                var UserID2 = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                var isCustomer = CNService.IsCustomer(UserID2, context);
                var isVendor = CNService.IsVendor(UserID2, context);

                ViewBag.POSITION_DISPLAT_TXT = "Internal";
                if (isCustomer) ViewBag.POSITION_DISPLAT_TXT = "Customer";
                if (isVendor) ViewBag.POSITION_DISPLAT_TXT = "Vendor";

                return View();
            }
        }
    }
}