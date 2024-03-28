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
    public class CustomerController : Controller
    {
        private ARTWORKEntities db = new ARTWORKEntities();

        // GET: SAP_M_CUSTOMER
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "CUSTOMERMASTER", context))
                        return View("NoAuth");

                    return View(db.XECM_M_CUSTOMER.ToList());
                }
            }
        }

        // GET: SAP_M_CUSTOMER/Details/5
        public ActionResult Details(int id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var CustomerId = ART_M_USER_CUSTOMER_SERVICE.GetByItem(new ART_M_USER_CUSTOMER() { USER_ID = id }, context).FirstOrDefault().CUSTOMER_ID;
                        XECM_M_CUSTOMER sAP_M_CUSTOMER = db.XECM_M_CUSTOMER.Find(CustomerId);
                        if (sAP_M_CUSTOMER == null)
                        {
                            return HttpNotFound();
                        }
                        return View(sAP_M_CUSTOMER);
                    }
                }
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }

        // GET: SAP_M_CUSTOMER/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SAP_M_CUSTOMER/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CUSTOMER_CODE,CUSTOMER_NAME,IS_SHADE_LIMIT,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] XECM_M_CUSTOMER_2 sAP_M_CUSTOMER)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    if (XECM_M_CUSTOMER_SERVICE.GetByItem(new XECM_M_CUSTOMER { CUSTOMER_NAME = sAP_M_CUSTOMER.CUSTOMER_NAME }, context).FirstOrDefault() == null)
                    {
                        var cntCustomer = (from p in db.XECM_M_CUSTOMER where p.CUSTOMER_CODE.StartsWith("D") select p).ToList();
                        sAP_M_CUSTOMER.CUSTOMER_CODE = "D" + (cntCustomer.Count + 1).ToString().PadLeft(9, '0');
                        sAP_M_CUSTOMER.IS_ACTIVE = "X";
                        sAP_M_CUSTOMER.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                        sAP_M_CUSTOMER.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                        XECM_M_CUSTOMER_SERVICE.SaveOrUpdate(MapperServices.XECM_M_CUSTOMER(sAP_M_CUSTOMER), context);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The username is duplicate.");
                        return View(sAP_M_CUSTOMER);
                    }
                }
            }

            return View(sAP_M_CUSTOMER);
        }

        // GET: SAP_M_CUSTOMER/Edit/5
        public ActionResult Edit(int? id)
        {
            //ViewBag.ObjUser = ART_M_USER_CUSTOMER_SERVICE.GetByUSER_CUSTOMER_ID(id);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XECM_M_CUSTOMER sAP_M_CUSTOMER = db.XECM_M_CUSTOMER.Find(id);
            if (sAP_M_CUSTOMER == null)
            {
                return HttpNotFound();
            }
            return View(MapperServices.XECM_M_CUSTOMER(sAP_M_CUSTOMER));
        }

        // POST: SAP_M_CUSTOMER/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CUSTOMER_ID,CUSTOMER_CODE,CUSTOMER_NAME,IS_SHADE_LIMIT,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] XECM_M_CUSTOMER_2 sAP_M_CUSTOMER)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    sAP_M_CUSTOMER.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    sAP_M_CUSTOMER.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    XECM_M_CUSTOMER_SERVICE.SaveOrUpdate(MapperServices.XECM_M_CUSTOMER(sAP_M_CUSTOMER), context);
                    //db.Entry(sAP_M_CUSTOMER).State = EntityState.Modified;
                    //db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(sAP_M_CUSTOMER);
        }

        // GET: SAP_M_CUSTOMER/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            XECM_M_CUSTOMER sAP_M_CUSTOMER = db.XECM_M_CUSTOMER.Find(id);
            if (sAP_M_CUSTOMER == null)
            {
                return HttpNotFound();
            }
            return View(sAP_M_CUSTOMER);
        }

        // POST: SAP_M_CUSTOMER/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var context = new ARTWORKEntities())
            {
                var a = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(id, context);
                a.IS_ACTIVE = "";
                XECM_M_CUSTOMER_SERVICE.SaveOrUpdate(a, context);
            }
            //SAP_M_CUSTOMER sAP_M_CUSTOMER = db.SAP_M_CUSTOMER.Find(id);
            //db.SAP_M_CUSTOMER.Remove(sAP_M_CUSTOMER);
            //db.SaveChanges();
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
