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
    public class StepArtworkController : Controller
    {
        private ARTWORKEntities db = new ARTWORKEntities();

        // GET: ART_M_STEP_ARTWORK
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "STEP", context))
                        return View("NoAuth");

                    return View(db.ART_M_STEP_ARTWORK.ToList());
                }
            }
        }

        // GET: ART_M_STEP_ARTWORK/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_STEP_ARTWORK aRT_M_STEP_ARTWORK = db.ART_M_STEP_ARTWORK.Find(id);
            if (aRT_M_STEP_ARTWORK == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_STEP_ARTWORK);
        }

        // GET: ART_M_STEP_ARTWORK/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ART_M_STEP_ARTWORK/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "STEP_ARTWORK_ID,STEP_ARTWORK_CODE,STEP_ARTWORK_NAME,STEP_ARTWORK_DESCRIPTION,ROLE_ID_RESPONSE,DURATION,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_M_STEP_ARTWORK aRT_M_STEP_ARTWORK)
        {
            if (ModelState.IsValid)
            {
                if (aRT_M_STEP_ARTWORK.DURATION_EXTEND == null) aRT_M_STEP_ARTWORK.DURATION_EXTEND = 0;
                db.ART_M_STEP_ARTWORK.Add(aRT_M_STEP_ARTWORK);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aRT_M_STEP_ARTWORK);
        }

        // GET: ART_M_STEP_ARTWORK/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_STEP_ARTWORK aRT_M_STEP_ARTWORK = db.ART_M_STEP_ARTWORK.Find(id);
            if (aRT_M_STEP_ARTWORK == null)
            {
                return HttpNotFound();
            }
            return View(MapperServices.ART_M_STEP_ARTWORK(aRT_M_STEP_ARTWORK));
        }

        // POST: ART_M_STEP_ARTWORK/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "STEP_ARTWORK_ID,STEP_ARTWORK_CODE,STEP_ARTWORK_NAME,STEP_ARTWORK_DESCRIPTION,ROLE_ID_RESPONSE,DURATION,DURATION_EXTEND,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_M_STEP_ARTWORK_2 aRT_M_STEP_ARTWORK)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    if (aRT_M_STEP_ARTWORK.DURATION_EXTEND == null) aRT_M_STEP_ARTWORK.DURATION_EXTEND = 0;
                    aRT_M_STEP_ARTWORK.IS_ACTIVE = "X";
                    aRT_M_STEP_ARTWORK.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    ART_M_STEP_ARTWORK_SERVICE.SaveOrUpdate(MapperServices.ART_M_STEP_ARTWORK(aRT_M_STEP_ARTWORK), context);
                    //db.Entry(aRT_M_STEP_ARTWORK).State = EntityState.Modified;
                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(aRT_M_STEP_ARTWORK);
        }

        // GET: ART_M_STEP_ARTWORK/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_STEP_ARTWORK aRT_M_STEP_ARTWORK = db.ART_M_STEP_ARTWORK.Find(id);
            if (aRT_M_STEP_ARTWORK == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_STEP_ARTWORK);
        }

        // POST: ART_M_STEP_ARTWORK/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ART_M_STEP_ARTWORK aRT_M_STEP_ARTWORK = db.ART_M_STEP_ARTWORK.Find(id);
            db.ART_M_STEP_ARTWORK.Remove(aRT_M_STEP_ARTWORK);
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
