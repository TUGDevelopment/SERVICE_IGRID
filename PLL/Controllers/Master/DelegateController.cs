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
    public class DelegateController : Controller
    {
        private ARTWORKEntities db = new ARTWORKEntities();

        // GET: Delegate
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "DELEGATE", context))
                        return View("NoAuth");

                    var list = ART_WF_DELEGATE_SERVICE.GetByItem(new ART_WF_DELEGATE() { CURRENT_USER_ID = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), IS_ACTIVE = "X" }, context);
                    return View(list);
                }
            }
        }

        // GET: Delegate/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_WF_DELEGATE aRT_WF_DELEGATE = db.ART_WF_DELEGATE.Find(id);
            if (aRT_WF_DELEGATE == null)
            {
                return HttpNotFound();
            }
            return View(aRT_WF_DELEGATE);
        }

        // GET: Delegate/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Delegate/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ART_WF_DELEGATE_ID,CURRENT_USER_ID,TO_USER_ID,FROM_DATE,TO_DATE,REASON,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_WF_DELEGATE aRT_WF_DELEGATE)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    var listChk = ART_WF_DELEGATE_SERVICE.GetByItem(new ART_WF_DELEGATE() { CURRENT_USER_ID = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), IS_ACTIVE = "X" }, context);
                    bool valid = true;

                    foreach (var item in listChk)
                    {
                        if (aRT_WF_DELEGATE.FROM_DATE >= item.FROM_DATE && aRT_WF_DELEGATE.FROM_DATE <= item.TO_DATE)
                        {
                            valid = false;
                            break;
                        }

                        if (aRT_WF_DELEGATE.TO_DATE >= item.FROM_DATE && aRT_WF_DELEGATE.FROM_DATE <= item.TO_DATE)
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (valid)
                    {
                        aRT_WF_DELEGATE.CURRENT_USER_ID = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                        aRT_WF_DELEGATE.IS_ACTIVE = "X";
                        aRT_WF_DELEGATE.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                        aRT_WF_DELEGATE.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                        ART_WF_DELEGATE_SERVICE.SaveOrUpdate(aRT_WF_DELEGATE, context);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid Delegate date from and to.");
                    }
                }
            }

            return View(aRT_WF_DELEGATE);
        }

        // GET: Delegate/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_WF_DELEGATE aRT_WF_DELEGATE = db.ART_WF_DELEGATE.Find(id);
            if (aRT_WF_DELEGATE == null)
            {
                return HttpNotFound();
            }
            return View(aRT_WF_DELEGATE);
        }

        // POST: Delegate/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ART_WF_DELEGATE_ID,CURRENT_USER_ID,TO_USER_ID,FROM_DATE,TO_DATE,REASON,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_WF_DELEGATE aRT_WF_DELEGATE)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    aRT_WF_DELEGATE.CURRENT_USER_ID = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    aRT_WF_DELEGATE.IS_ACTIVE = "X";
                    aRT_WF_DELEGATE.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    aRT_WF_DELEGATE.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    ART_WF_DELEGATE_SERVICE.SaveOrUpdate(aRT_WF_DELEGATE, context);
                    return RedirectToAction("Index");
                }
            }
            return View(aRT_WF_DELEGATE);
        }

        // GET: Delegate/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_WF_DELEGATE aRT_WF_DELEGATE = db.ART_WF_DELEGATE.Find(id);
            if (aRT_WF_DELEGATE == null)
            {
                return HttpNotFound();
            }
            return View(aRT_WF_DELEGATE);
        }

        // POST: Delegate/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var context = new ARTWORKEntities())
            {
                ART_WF_DELEGATE aRT_WF_DELEGATE = db.ART_WF_DELEGATE.Find(id);
                aRT_WF_DELEGATE.CURRENT_USER_ID = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                aRT_WF_DELEGATE.IS_ACTIVE = null;
                aRT_WF_DELEGATE.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                aRT_WF_DELEGATE.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                ART_WF_DELEGATE_SERVICE.SaveOrUpdate(aRT_WF_DELEGATE, context);
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
