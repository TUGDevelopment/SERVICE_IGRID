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
    public class EmailTemplateController : Controller
    {
        private ARTWORKEntities db = new ARTWORKEntities();

        // GET: EmailTemplate
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "EMAILTEMPLATE", context))
                        return View("NoAuth");

                    return View(db.ART_M_EMAIL_TEMPLATE.ToList());
                }
            }
        }

        // GET: EmailTemplate/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_EMAIL_TEMPLATE aRT_M_EMAIL_TEMPLATE = db.ART_M_EMAIL_TEMPLATE.Find(id);
            if (aRT_M_EMAIL_TEMPLATE == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_EMAIL_TEMPLATE);
        }

        // GET: EmailTemplate/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmailTemplate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "EMAIL_TEMPLATE_ID,EMAIL_TEMPLATE_CODE,M_SUBJECT,M_DEAR,M_BODY_01,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_M_EMAIL_TEMPLATE_2 aRT_M_EMAIL_TEMPLATE)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    aRT_M_EMAIL_TEMPLATE.IS_ACTIVE = "X";
                    aRT_M_EMAIL_TEMPLATE.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    aRT_M_EMAIL_TEMPLATE.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    ART_M_EMAIL_TEMPLATE_SERVICE.SaveOrUpdate(MapperServices.ART_M_EMAIL_TEMPLATE(aRT_M_EMAIL_TEMPLATE), context);
                    //db.ART_M_EMAIL_TEMPLATE.Add(aRT_M_EMAIL_TEMPLATE);
                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(aRT_M_EMAIL_TEMPLATE);
        }

        // GET: EmailTemplate/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_EMAIL_TEMPLATE aRT_M_EMAIL_TEMPLATE = db.ART_M_EMAIL_TEMPLATE.Find(id);
            if (aRT_M_EMAIL_TEMPLATE == null)
            {
                return HttpNotFound();
            }
            return View(MapperServices.ART_M_EMAIL_TEMPLATE(aRT_M_EMAIL_TEMPLATE));
        }

        // POST: EmailTemplate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "EMAIL_TEMPLATE_ID,EMAIL_TEMPLATE_CODE,M_SUBJECT,M_DEAR,M_BODY_01,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_M_EMAIL_TEMPLATE_2 aRT_M_EMAIL_TEMPLATE)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    aRT_M_EMAIL_TEMPLATE.IS_ACTIVE = "X";
                    aRT_M_EMAIL_TEMPLATE.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    aRT_M_EMAIL_TEMPLATE.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    ART_M_EMAIL_TEMPLATE_SERVICE.SaveOrUpdate(MapperServices.ART_M_EMAIL_TEMPLATE(aRT_M_EMAIL_TEMPLATE), context);
                    //db.Entry(aRT_M_EMAIL_TEMPLATE).State = EntityState.Modified;
                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(aRT_M_EMAIL_TEMPLATE);
        }

        // GET: EmailTemplate/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_EMAIL_TEMPLATE aRT_M_EMAIL_TEMPLATE = db.ART_M_EMAIL_TEMPLATE.Find(id);
            if (aRT_M_EMAIL_TEMPLATE == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_EMAIL_TEMPLATE);
        }

        // POST: EmailTemplate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ART_M_EMAIL_TEMPLATE aRT_M_EMAIL_TEMPLATE = db.ART_M_EMAIL_TEMPLATE.Find(id);
            db.ART_M_EMAIL_TEMPLATE.Remove(aRT_M_EMAIL_TEMPLATE);
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
