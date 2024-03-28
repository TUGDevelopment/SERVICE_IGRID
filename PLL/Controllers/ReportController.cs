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
    public class ReportController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermissionReport(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult Tracking()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "TRACKING", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult TrackingV2()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "TRACKING", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult TrackingV3()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "TRACKING", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult Outstanding()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "OUTSTANDING", context))
                        return View("NoAuth");

                    var currentUser = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    ViewBag.PICId = currentUser;
                    ViewBag.PICName = CNService.GetUserName(currentUser, context);

                    return View();
                }
            }
        }

        public ActionResult Warehouse_old()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "WAREHOUSE", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult GetReportWarehouse(DAL.Model.V_ART_WAREHOUSE_REPORT_REQUEST param)
        {
    
            var list = BLL.Helpers.WarehouseReportHelper.GetWarehouseReportV2_List(param);

            return Json(new { data = list }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Warehouse()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "WAREHOUSE", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult EndToEnd()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "ENDTOEND", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult EndToEndV2()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "ENDTOEND", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult EndToEndV3()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "ENDTOEND", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }


        public ActionResult VendorCustomerCollaboration()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "COLLABORATION", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult ListsStatusofPackagingMaterial_old()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "MATCONTROL", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult ListsStatusofPackagingMaterial()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "MATCONTROL", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }


        public ActionResult DisplaySO()
        {
            using (var context = new ARTWORKEntities())
            {
                //if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "DISPLAYSO", context))
                //    return View("NoAuth");

                return View();
            }
        }

        public ActionResult Summary()
        {
            using (var context = new ARTWORKEntities())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM ART_TEMP_CONTAIN WHERE [CREATE_DATE] < getdate()-1");
            }

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "SUMMARY", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult KPI()
        {
            using (var context = new ARTWORKEntities())
            {
                //if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "DISPLAYSO", context))
                //    return View("NoAuth");

                return View();
            }
        }

        public ActionResult Overview()
        {
            using (var context = new ARTWORKEntities())
            {
                if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "IGRID", context))
                    return View("NoAuth");

                return View();
            }
        }
        public ActionResult ImpactedMat_Desc()
        {
            using (var context = new ARTWORKEntities())
            {
                if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "IGRID", context))
                    return View("NoAuth");
                return View();
            }
        }
        public ActionResult IGridSummary()
        {
            using (var context = new ARTWORKEntities())
            {
                if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "IGRID", context))
                    return View("NoAuth");
                return View();
            }
        }
        public ActionResult OverallMaster()
        {
            using (var context = new ARTWORKEntities())
            {
                if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "IGRID", context))
                    return View("NoAuth");
                return View();
            }
        }
        public ActionResult MatStatus()
        {
            using (var context = new ARTWORKEntities())
            {
                if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "IGRID", context))
                    return View("NoAuth"); 

                return View();
            }
        }
        public ActionResult TrackingIGrid()
        {
            using (var context = new ARTWORKEntities())
            {
                if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "IGRID", context))
                    return View("NoAuth");

                return View();
            }
        }

        public ActionResult History()
        {
            using (var context = new ARTWORKEntities())
            {
                if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "IGRID", context))
                    return View("NoAuth");

                return View();
            }
        }

        public ActionResult MasterDataChangeLog()
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