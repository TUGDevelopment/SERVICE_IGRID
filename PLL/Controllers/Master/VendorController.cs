using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BLL.Services;
using DAL;
using DAL.Model;
using Microsoft.AspNet.Identity;

namespace PLL.Controllers
{
    public class VendorController : Controller
    {
        private ARTWORKEntities db = new ARTWORKEntities();

        // GET: Vendor
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "VENDORMASTER", context))
                        return View("NoAuth");

                    return View(db.XECM_M_VENDOR.ToList());
                }
            }
        }

        // GET: Vendor/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    var VendorId = ART_M_USER_VENDOR_SERVICE.GetByItem(new ART_M_USER_VENDOR() { USER_ID = id }, context).FirstOrDefault().VENDOR_ID;
                    XECM_M_VENDOR sAP_M_VENDOR = db.XECM_M_VENDOR.Find(VendorId);
                    if (sAP_M_VENDOR == null)
                    {
                        return HttpNotFound();
                    }
                    return View(sAP_M_VENDOR);
                }
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // GET: Vendor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Vendor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VENDOR_CODE,VENDOR_NAME,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] XECM_M_VENDOR_2 sAP_M_VENDOR)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    if (XECM_M_VENDOR_SERVICE.GetByItem(new XECM_M_VENDOR { VENDOR_NAME = sAP_M_VENDOR.VENDOR_NAME }, context).FirstOrDefault() == null)
                    {
                        var cntVendor = (from p in db.XECM_M_VENDOR where p.VENDOR_CODE.StartsWith("V") select p).ToList();
                        sAP_M_VENDOR.VENDOR_CODE = "V" + (cntVendor.Count + 1).ToString().PadLeft(9, '0');
                        sAP_M_VENDOR.IS_ACTIVE = "X";
                        sAP_M_VENDOR.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                        sAP_M_VENDOR.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                        XECM_M_VENDOR_SERVICE.SaveOrUpdate(MapperServices.XECM_M_VENDOR(sAP_M_VENDOR), context);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The username is duplicate.");
                        return View(sAP_M_VENDOR);
                    }
                }
            }

            return View(sAP_M_VENDOR);
        }

        // GET: Vendor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XECM_M_VENDOR sAP_M_VENDOR = db.XECM_M_VENDOR.Find(id);
            if (sAP_M_VENDOR == null)
            {
                return HttpNotFound();
            }
            return View(MapperServices.XECM_M_VENDOR(sAP_M_VENDOR));
        }

        // POST: Vendor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VENDOR_ID,VENDOR_CODE,VENDOR_NAME,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] XECM_M_VENDOR_2 sAP_M_VENDOR)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    sAP_M_VENDOR.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    sAP_M_VENDOR.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    XECM_M_VENDOR_SERVICE.SaveOrUpdate(MapperServices.XECM_M_VENDOR(sAP_M_VENDOR), context);
                    //db.Entry(sAP_M_VENDOR).State = EntityState.Modified;
                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(sAP_M_VENDOR);
        }

        // GET: Vendor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XECM_M_VENDOR sAP_M_VENDOR = db.XECM_M_VENDOR.Find(id);
            if (sAP_M_VENDOR == null)
            {
                return HttpNotFound();
            }
            return View(sAP_M_VENDOR);
        }

        // POST: Vendor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var context = new ARTWORKEntities())
            {
                var a = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(id, context);
                a.IS_ACTIVE = "";
                a.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                XECM_M_VENDOR_SERVICE.SaveOrUpdate(a, context);
                //SAP_M_VENDOR sAP_M_VENDOR = db.SAP_M_VENDOR.Find(id);
                //db.SAP_M_VENDOR.Remove(sAP_M_VENDOR);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
