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
using DAL.Model;
using Microsoft.AspNet.Identity;

namespace PLL.Controllers
{
    public class StepMockupController : Controller
    {
        private ARTWORKEntities db = new ARTWORKEntities();

        // GET: ART_M_STEP_MOCKUP
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "STEP", context))
                        return View("NoAuth");

                    return View(db.ART_M_STEP_MOCKUP.ToList());
                }
            }
        }

        // GET: ART_M_STEP_MOCKUP/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_STEP_MOCKUP aRT_M_STEP_MOCKUP = db.ART_M_STEP_MOCKUP.Find(id);
            if (aRT_M_STEP_MOCKUP == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_STEP_MOCKUP);
        }

        // GET: ART_M_STEP_MOCKUP/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ART_M_STEP_MOCKUP/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "STEP_MOCKUP_ID,STEP_MOCKUP_NAME,STEP_MOCKUP_CODE,STEP_MOCKUP_DESCRIPTION,ROLE_ID_RESPONSE,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_M_STEP_MOCKUP aRT_M_STEP_MOCKUP)
        {
            if (ModelState.IsValid)
            {
                if (aRT_M_STEP_MOCKUP.DURATION_EXTEND == null) aRT_M_STEP_MOCKUP.DURATION_EXTEND = 0;
                db.ART_M_STEP_MOCKUP.Add(aRT_M_STEP_MOCKUP);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aRT_M_STEP_MOCKUP);
        }

        // GET: ART_M_STEP_MOCKUP/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_STEP_MOCKUP aRT_M_STEP_MOCKUP = db.ART_M_STEP_MOCKUP.Find(id);

            if (aRT_M_STEP_MOCKUP == null)
            {
                return HttpNotFound();
            }
            return View(MapperServices.ART_M_STEP_MOCKUP(aRT_M_STEP_MOCKUP));
        }

        // POST: ART_M_STEP_MOCKUP/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "STEP_MOCKUP_ID,STEP_MOCKUP_NAME,STEP_MOCKUP_CODE,STEP_MOCKUP_DESCRIPTION,ROLE_ID_RESPONSE,DURATION,DURATION_EXTEND,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_M_STEP_MOCKUP_2 aRT_M_STEP_MOCKUP)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    if (aRT_M_STEP_MOCKUP.DURATION_EXTEND == null) aRT_M_STEP_MOCKUP.DURATION_EXTEND = 0;
                    aRT_M_STEP_MOCKUP.IS_ACTIVE = "X";
                    aRT_M_STEP_MOCKUP.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    ART_M_STEP_MOCKUP_SERVICE.SaveOrUpdate(MapperServices.ART_M_STEP_MOCKUP(aRT_M_STEP_MOCKUP), context);
                    return RedirectToAction("Index");
                }
            }
            return View(aRT_M_STEP_MOCKUP);
        }

        // GET: ART_M_STEP_MOCKUP/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_STEP_MOCKUP aRT_M_STEP_MOCKUP = db.ART_M_STEP_MOCKUP.Find(id);
            if (aRT_M_STEP_MOCKUP == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_STEP_MOCKUP);
        }

        // POST: ART_M_STEP_MOCKUP/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ART_M_STEP_MOCKUP aRT_M_STEP_MOCKUP = db.ART_M_STEP_MOCKUP.Find(id);
            db.ART_M_STEP_MOCKUP.Remove(aRT_M_STEP_MOCKUP);
            db.SaveChanges();
            return RedirectToAction("Index");
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
