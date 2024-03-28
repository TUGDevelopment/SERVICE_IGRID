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
    public class WordingTemplateController : Controller
    {
        private ARTWORKEntities db = new ARTWORKEntities();

        // GET: WordingTemplate
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "WORDINGTEMPLATE", context))
                        return View("NoAuth");

                    var list = db.ART_M_TEMPLATE.ToList();
                    list = list.Where(m => m.IS_ACTIVE == "X").ToList();
                    foreach (var item in list)
                    {
                        item.DESCRIPTION = item.DESCRIPTION.Replace("<br/>", "\r\n");
                    }
                    return View(list);
                }
            }
        }

        // GET: WordingTemplate/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_TEMPLATE aRT_M_TEMPLATE = db.ART_M_TEMPLATE.Find(id);
            if (aRT_M_TEMPLATE == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_TEMPLATE);
        }

        // GET: WordingTemplate/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: WordingTemplate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "TEMPLATE_ID,TEMPLATE_NAME,DESCRIPTION,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_M_TEMPLATE_2 aRT_M_TEMPLATE)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    aRT_M_TEMPLATE.DESCRIPTION = aRT_M_TEMPLATE.DESCRIPTION.Replace("\r\n", "<br/>");
                    aRT_M_TEMPLATE.IS_ACTIVE = "X";
                    aRT_M_TEMPLATE.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    aRT_M_TEMPLATE.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    ART_M_TEMPLATE_SERVICE.SaveOrUpdate(MapperServices.ART_M_TEMPLATE(aRT_M_TEMPLATE), context);
                    //db.ART_M_TEMPLATE.Add(aRT_M_TEMPLATE);
                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(aRT_M_TEMPLATE);
        }

        // GET: WordingTemplate/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_TEMPLATE aRT_M_TEMPLATE = db.ART_M_TEMPLATE.Find(id);
            aRT_M_TEMPLATE.DESCRIPTION = aRT_M_TEMPLATE.DESCRIPTION.Replace("<br/>", "\r\n");
            if (aRT_M_TEMPLATE == null)
            {
                return HttpNotFound();
            }
            return View(MapperServices.ART_M_TEMPLATE(aRT_M_TEMPLATE));
        }

        // POST: WordingTemplate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "TEMPLATE_ID,TEMPLATE_NAME,DESCRIPTION,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_M_TEMPLATE_2 aRT_M_TEMPLATE)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    aRT_M_TEMPLATE.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    ART_M_TEMPLATE_SERVICE.SaveOrUpdate(MapperServices.ART_M_TEMPLATE(aRT_M_TEMPLATE), context);
                    //db.Entry(aRT_M_TEMPLATE).State = EntityState.Modified;
                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(aRT_M_TEMPLATE);
        }

        // GET: WordingTemplate/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_TEMPLATE aRT_M_TEMPLATE = db.ART_M_TEMPLATE.Find(id);
            if (aRT_M_TEMPLATE == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_TEMPLATE);
        }

        // POST: WordingTemplate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ART_M_TEMPLATE aRT_M_TEMPLATE = db.ART_M_TEMPLATE.Find(id);
            db.ART_M_TEMPLATE.Remove(aRT_M_TEMPLATE);
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
