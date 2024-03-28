using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DAL;
using Microsoft.AspNet.Identity;
using BLL.Services;
using DAL.Model;

namespace PLL.Controllers
{

    public class UpdateMasterController : Controller
    {
        // GET: UpdateMaster
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "IGRID", context))
                    return View("NoAuth");

                return View();
            }
         
        }
    }
}