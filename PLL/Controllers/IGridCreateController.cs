using BLL.Services;
using DAL;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace PLL.Controllers
{
    public class IGridCreateController : Controller
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

                    ViewBag.IGridSAPMaterialId = id.HasValue ? id : 0;// id.Value;
                   
                }
            }
            return View();
        }

    }
}