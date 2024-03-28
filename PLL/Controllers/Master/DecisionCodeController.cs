using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DAL;
using Microsoft.AspNet.Identity;
using BLL.Services;
using DAL.Model;

namespace PLL.Controllers.Master
{
    public partial class DecisionReasonModel
    {
        public ART_M_USER ObjUser { get; set; }
        public List<ART_M_DECISION_REASON_2> ObjItem { get; set; }


    }
    public class DecisionCodeController : Controller
    {
        private ARTWORKEntities db = new ARTWORKEntities();

        // GET: ART_M_DECISION_REASON
        public ActionResult Index()
        {
            DecisionReasonModel model = new DecisionReasonModel();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "DECISION", context))
                        return View("NoAuth");

                    model.ObjUser = ART_M_USER_SERVICE.GetByUSER_ID(BLL.Services.CNService.GetUserID(User.Identity.GetUserName(), context), context);
                    model.ObjItem = MapperServices.ART_M_DECISION_REASON(ART_M_DECISION_REASON_SERVICE.GetAll(context));
                    List<ART_M_DECISION_REASON_2> listSteps = new List<ART_M_DECISION_REASON_2>();

                    foreach (var allstep in model.ObjItem)
                    {
                        var temp = ART_M_DECISION_REASON_CONFIG_SERVICE.GetByItem(new ART_M_DECISION_REASON_CONFIG() { DECISION_REASON_CONFIG_CODE = allstep.STEP_CODE }, context).FirstOrDefault();
                        if (temp != null)
                            allstep.DISPLAY_TXT = temp.DECISION_REASON_CONFIG_NAME;

                        listSteps.Add(allstep);
                    }

                    model.ObjItem = listSteps;

                    return View(model);
                }
            }
        }

        // GET: ART_M_DECISION_REASON/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    ART_M_DECISION_REASON aRT_M_DECISION_REASON = db.ART_M_DECISION_REASON.Find(id);
        //    if (aRT_M_DECISION_REASON == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(aRT_M_DECISION_REASON);
        //}

        // GET: ART_M_DECISION_REASON/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ART_M_DECISION_REASON/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WF,STEP_CODE,DESCRIPTION,IS_OVERDUE,IS_DEFAULT,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_M_DECISION_REASON_2 aRT_M_DECISION_REASON)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    if (aRT_M_DECISION_REASON.IS_DEFAULT == "X")
                    {
                        var list = ART_M_DECISION_REASON_SERVICE.GetAll(context);
                        foreach (var item in list)
                        {
                            item.IS_DEFAULT = null; //put null all item in db
                            ART_M_DECISION_REASON_SERVICE.SaveOrUpdate(item, context);
                        }
                        aRT_M_DECISION_REASON.WF = "-";
                        aRT_M_DECISION_REASON.STEP_CODE = null;
                    }

                    aRT_M_DECISION_REASON.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    aRT_M_DECISION_REASON.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    ART_M_DECISION_REASON_SERVICE.SaveOrUpdate(MapperServices.ART_M_DECISION_REASON(aRT_M_DECISION_REASON), context);
                    return RedirectToAction("Index");
                }
            }

            return View(aRT_M_DECISION_REASON);
        }

        // GET: ART_M_DECISION_REASON/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_DECISION_REASON aRT_M_DECISION_REASON = db.ART_M_DECISION_REASON.Find(id);
            if (aRT_M_DECISION_REASON == null)
            {
                return HttpNotFound();
            }
            return View(MapperServices.ART_M_DECISION_REASON(aRT_M_DECISION_REASON));
        }

        // POST: ART_M_DECISION_REASON/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ART_M_DECISION_REASON_ID,WF,STEP_CODE,DESCRIPTION,IS_OVERDUE,IS_DEFAULT,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_M_DECISION_REASON_2 aRT_M_DECISION_REASON)
        {
            var DataDefault = aRT_M_DECISION_REASON.IS_DEFAULT;
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    if (DataDefault == "X") //has x in param
                    {
                        aRT_M_DECISION_REASON.IS_DEFAULT = "X";
                        aRT_M_DECISION_REASON.WF = "-";
                        aRT_M_DECISION_REASON.STEP_CODE = null;
                        aRT_M_DECISION_REASON.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                        ART_M_DECISION_REASON_SERVICE.SaveOrUpdate(MapperServices.ART_M_DECISION_REASON(aRT_M_DECISION_REASON), context);
                    }
                    else
                    {
                        aRT_M_DECISION_REASON.IS_DEFAULT = null;
                        aRT_M_DECISION_REASON.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                        ART_M_DECISION_REASON_SERVICE.SaveOrUpdate(MapperServices.ART_M_DECISION_REASON(aRT_M_DECISION_REASON), context);
                    }
                }
                return RedirectToAction("Index");
            }
            return View(aRT_M_DECISION_REASON);
        }

        // GET: ART_M_DECISION_REASON/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_DECISION_REASON aRT_M_DECISION_REASON = db.ART_M_DECISION_REASON.Find(id);
            if (aRT_M_DECISION_REASON == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_DECISION_REASON);
        }

        // POST: ART_M_DECISION_REASON/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var context = new ARTWORKEntities())
            {
                var reason = ART_M_DECISION_REASON_SERVICE.GetByART_M_DECISION_REASON_ID(id, context);
                reason.IS_ACTIVE = "";
                reason.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                ART_M_DECISION_REASON_SERVICE.SaveOrUpdate(reason, context);
                //ART_M_DECISION_REASON aRT_M_DECISION_REASON = db.ART_M_DECISION_REASON.Find(id);
                //db.ART_M_DECISION_REASON.Remove(aRT_M_DECISION_REASON);
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
