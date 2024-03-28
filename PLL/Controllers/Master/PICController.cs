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

namespace PLL.Controllers.Master
{
    public partial class PICModel
    {
        public ART_M_USER ObjUserPIC { get; set; }
        public string ObjName { get; set; }
        public List<ART_M_PIC_2> ObjItemPIC { get; set; }
    }

    public class PICController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "PIC", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

    }
}
