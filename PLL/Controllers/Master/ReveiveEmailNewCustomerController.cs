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

namespace PLL.Views
{
    public class ReveiveEmailNewCustomerController : Controller
    {
        private ARTWORKEntities db = new ARTWORKEntities();

        // GET: ReveiveEmailNewCustomer
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "REVEIVEEMAILNEWCUSTOMER", context))
                        return View("NoAuth");

                    var list = MapperServices.ART_M_RECEIVE_EMAIL_NEW_CUSTOMER(db.ART_M_RECEIVE_EMAIL_NEW_CUSTOMER.ToList());
                    foreach (var item in list)
                    {
                        item.EMAIL_DISPLAY_TXT = ART_M_USER_SERVICE.GetByUSER_ID(item.USER_ID, context).EMAIL;
                        item.USER_DISPLAY_TXT = CNService.GetUserName(item.USER_ID, context);
                    }

                    return View(list);
                }
            }
        }

        // GET: ReveiveEmailNewCustomer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_RECEIVE_EMAIL_NEW_CUSTOMER aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER = db.ART_M_RECEIVE_EMAIL_NEW_CUSTOMER.Find(id);
            if (aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER);
        }

        // GET: ReveiveEmailNewCustomer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReveiveEmailNewCustomer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ART_M_RECEIVE_EMAIL_ID,USER_ID,CREATE_BY,CREATE_DATE,UPDATE_BY,UPDATE_DATE")] ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_2 aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_SERVICE.SaveOrUpdate(MapperServices.ART_M_RECEIVE_EMAIL_NEW_CUSTOMER(aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER), context);
                    return RedirectToAction("Index");
                }
            }

            return View(aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER);
        }

        // GET: ReveiveEmailNewCustomer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_RECEIVE_EMAIL_NEW_CUSTOMER aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER = db.ART_M_RECEIVE_EMAIL_NEW_CUSTOMER.Find(id);
            if (aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER);
        }

        // POST: ReveiveEmailNewCustomer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ART_M_RECEIVE_EMAIL_ID,USER_ID,CREATE_BY,CREATE_DATE,UPDATE_BY,UPDATE_DATE")] ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_2 aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER);
        }

        // GET: ReveiveEmailNewCustomer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_RECEIVE_EMAIL_NEW_CUSTOMER aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER = db.ART_M_RECEIVE_EMAIL_NEW_CUSTOMER.Find(id);
            if (aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER == null)
            {
                return HttpNotFound();
            }

            using (var context = new ARTWORKEntities())
            {
                var item = MapperServices.ART_M_RECEIVE_EMAIL_NEW_CUSTOMER(aRT_M_RECEIVE_EMAIL_NEW_CUSTOMER);
                item.USER_DISPLAY_TXT = CNService.GetUserName(item.USER_ID, context);
                item.EMAIL_DISPLAY_TXT = ART_M_USER_SERVICE.GetByUSER_ID(item.USER_ID, context).EMAIL;
                return View(item);
            }
        }

        // POST: ReveiveEmailNewCustomer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var context = new ARTWORKEntities())
            {
                ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_SERVICE.DeleteByART_M_RECEIVE_EMAIL_ID(id, context);
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
