using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BLL.Services;
using DAL;
using Microsoft.AspNet.Identity;

namespace PLL.Controllers
{
    public class ZoneController : Controller
    {
        private ARTWORKEntities db = new ARTWORKEntities();

        // GET: Zone
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "ZONE", context))
                        return View("NoAuth");

                    return View(db.SAP_M_COUNTRY.ToList());
                }
            }
        }

        // GET: Zone/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SAP_M_COUNTRY sAP_M_COUNTRY = db.SAP_M_COUNTRY.Find(id);
            if (sAP_M_COUNTRY == null)
            {
                return HttpNotFound();
            }
            return View(sAP_M_COUNTRY);
        }

        // GET: Zone/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Zone/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "COUNTRY_ID,COUNTRY_CODE,NAME,ZONE,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] SAP_M_COUNTRY sAP_M_COUNTRY)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    if (SAP_M_COUNTRY_SERVICE.GetByItem(new SAP_M_COUNTRY { COUNTRY_CODE = sAP_M_COUNTRY.COUNTRY_CODE }, context).FirstOrDefault() == null)
                    {
                        sAP_M_COUNTRY.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                        sAP_M_COUNTRY.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                        SAP_M_COUNTRY_SERVICE.SaveOrUpdate(sAP_M_COUNTRY, context);
                        //db.SAP_M_COUNTRY.Add(sAP_M_COUNTRY);
                        //db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The country code is duplicate.");
                        return View(sAP_M_COUNTRY);
                    }
                }
            }

            return View(sAP_M_COUNTRY);
        }

        // GET: Zone/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SAP_M_COUNTRY sAP_M_COUNTRY = db.SAP_M_COUNTRY.Find(id);
            if (sAP_M_COUNTRY == null)
            {
                return HttpNotFound();
            }
            return View(sAP_M_COUNTRY);
        }

        // POST: Zone/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "COUNTRY_ID,COUNTRY_CODE,NAME,ZONE,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] SAP_M_COUNTRY sAP_M_COUNTRY)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    sAP_M_COUNTRY.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    SAP_M_COUNTRY_SERVICE.SaveOrUpdate(sAP_M_COUNTRY, context);
                    //db.Entry(sAP_M_COUNTRY).State = EntityState.Modified;
                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(sAP_M_COUNTRY);
        }

        // GET: Zone/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SAP_M_COUNTRY sAP_M_COUNTRY = db.SAP_M_COUNTRY.Find(id);
            if (sAP_M_COUNTRY == null)
            {
                return HttpNotFound();
            }
            return View(sAP_M_COUNTRY);
        }

        // POST: Zone/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var context = new ARTWORKEntities())
            {
                var country = SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(id, context);
                country.IS_ACTIVE = "";
                country.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                SAP_M_COUNTRY_SERVICE.SaveOrUpdate(country, context);
                //SAP_M_COUNTRY sAP_M_COUNTRY = db.SAP_M_COUNTRY.Find(id);
                //db.SAP_M_COUNTRY.Remove(sAP_M_COUNTRY);
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
