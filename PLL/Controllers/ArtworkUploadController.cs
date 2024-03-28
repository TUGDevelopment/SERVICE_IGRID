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
    public class ArtworkUploadController : Controller
    {
        public ActionResult Index(int? id)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (BLL.Services.CNService.IsVendor(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), context))
                        return View("NoAuth");

                    ViewBag.ArtworkRequestId = id == null ? 0 : id;

                    return View();
                }
            }
        }
    }
}