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
    public partial class UsersRoleModel
    {
        public ART_M_ROLE ObjRole_ { get; set; }
        public ART_M_USER ObjUser { get; set; }
        public XECM_M_VENDOR ObjVendorName { get; set; }
        public ART_M_USER_VENDOR ObjVenID { get; set; }
        public XECM_M_VENDOR ObjVenNAME { get; set; }
        public ART_M_USER_CUSTOMER ObjCusID { get; set; }
        public XECM_M_CUSTOMER ObjCusNAME { get; set; }
        public ART_M_USER ObjPosID { get; set; }
        public ART_M_POSITION ObjPosNAME { get; set; }

        public List<SAP_M_TYPE_OF_PRODUCT> ObjTypeProduct { get; set; }
        public List<ART_M_USER_TYPE_OF_PRODUCT> ObjUserTypeProduct { get; set; }

        public List<SAP_M_COMPANY> ObjCompany { get; set; }
        public List<ART_M_USER_COMPANY> ObjUserCompany { get; set; }

        public List<ART_M_USER> ObjSup { get; set; }
        public List<ART_M_USER_UPPER_LEVEL> ObjUserSup { get; set; }

        public List<XECM_M_CUSTOMER> ObjCustomer { get; set; }
        public List<ART_M_USER_CUSTOMER> ObjUserCustomer { get; set; }

        public List<XECM_M_VENDOR> ObjVendor { get; set; }
        public List<ART_M_USER_VENDOR> ObjUserVendor { get; set; }

        public List<ART_M_ROLE> ObjRole { get; set; }
        public List<ART_M_USER_ROLE> ObjUserRole { get; set; }

        public List<SAP_M_CHARACTERISTIC> ObjPack { get; set; }
        public List<ART_M_VENDOR_MATGROUP> ObjVendorPack { get; set; }

        public string ObjPosition { get; set; }
    }
    public class UsersRoleController : Controller
    {
        private ARTWORKEntities db = new ARTWORKEntities();

        // GET: UsersRole
        public ActionResult Index(int userId, string position_)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "USERROLE", context))
                        return View("NoAuth");

                    UsersRoleModel model = new UsersRoleModel();
                    ART_M_USER ObjUser_ = new ART_M_USER();
                    XECM_M_VENDOR ObjVendorName_ = new XECM_M_VENDOR();

                    if (position_ != "VendorRole")
                    {
                        ObjUser_ = ART_M_USER_SERVICE.GetByUSER_ID(userId, context);
                        model.ObjUser = ObjUser_;
                        ViewBag.Header = "Config role to user";
                        ViewBag.Title = "Config role to user";
                    }
                    else
                    {
                        ObjVendorName_ = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(userId, context);
                        ObjUser_ = ART_M_USER_SERVICE.GetAll(context).FirstOrDefault();
                        model.ObjUser = ObjUser_;
                        model.ObjVendorName = ObjVendorName_;
                        model.ObjUser.FIRST_NAME = model.ObjVendorName.VENDOR_CODE;
                        model.ObjUser.LAST_NAME = model.ObjVendorName.VENDOR_NAME;
                        model.ObjUser.USER_ID = model.ObjVendorName.VENDOR_ID;
                        ViewBag.Header = "Config packaging type";
                        ViewBag.Title = "Config packaging type";
                    }


                    var vendor_ = ART_M_USER_VENDOR_SERVICE.GetByItem(new ART_M_USER_VENDOR() { USER_ID = userId }, context);
                    if (vendor_.FirstOrDefault() != null)
                    {
                        model.ObjVenID = vendor_.FirstOrDefault();
                        model.ObjVenNAME = XECM_M_VENDOR_SERVICE.GetByItem(new XECM_M_VENDOR() { VENDOR_ID = model.ObjVenID.VENDOR_ID }, context).FirstOrDefault();
                    }
                    else
                    {
                        model.ObjVenID = new ART_M_USER_VENDOR();
                        model.ObjVenNAME = new XECM_M_VENDOR();
                    }
                    var customer_ = ART_M_USER_CUSTOMER_SERVICE.GetByItem(new ART_M_USER_CUSTOMER() { USER_ID = userId }, context);
                    if (customer_.FirstOrDefault() != null)
                    {
                        model.ObjCusID = customer_.FirstOrDefault();
                        model.ObjCusNAME = XECM_M_CUSTOMER_SERVICE.GetByItem(new XECM_M_CUSTOMER() { CUSTOMER_ID = model.ObjCusID.CUSTOMER_ID }, context).FirstOrDefault();
                    }
                    else
                    {
                        model.ObjCusID = new ART_M_USER_CUSTOMER();
                        model.ObjCusNAME = new XECM_M_CUSTOMER();
                    }

                    var employee_ = ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { USER_ID = userId }, context).FirstOrDefault();
                    if (employee_ != null)
                    {
                        model.ObjPosID = employee_;
                        model.ObjPosNAME = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_ID = Convert.ToInt32(model.ObjPosID.POSITION_ID) }, context).FirstOrDefault();
                    }
                    else
                    {
                        model.ObjPosID = new ART_M_USER();
                        model.ObjPosNAME = new ART_M_POSITION();
                    }

                    int pos = Convert.ToInt32(ObjUser_.POSITION_ID);
                    List<int> listRoleID = new List<int>();
                    listRoleID = ART_M_POSITION_ROLE_SERVICE.GetAll(context).Where(m => m.POSITION_ID == pos).Select(m => m.ROLE_ID).ToList();

                    ART_M_ROLE role = new ART_M_ROLE();
                    List<ART_M_ROLE> listRole = new List<ART_M_ROLE>();
                    foreach (int item in listRoleID)
                    {
                        role = new ART_M_ROLE();
                        role = ART_M_ROLE_SERVICE.GetByROLE_ID(item, context);

                        if (role != null)
                        {
                            listRole.Add(role);
                        }
                    }

                    model.ObjRole = listRole;
                    model.ObjTypeProduct = SAP_M_TYPE_OF_PRODUCT_SERVICE.GetAll(context);
                    model.ObjCompany = SAP_M_COMPANY_SERVICE.GetAll(context);

                    var VENDOR = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "VENDOR" }, context).FirstOrDefault().ART_M_POSITION_ID;
                    var CUSTOMER = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "CUSTOMER" }, context).FirstOrDefault().ART_M_POSITION_ID;
                    model.ObjSup = (from q in context.ART_M_USER
                                    where q.POSITION_ID != VENDOR && q.POSITION_ID != CUSTOMER
                                    select q).ToList();

                    //model.ObjSup = ART_M_USER_SERVICE.GetAll(context).Where(m => m.POSITION_ID != (new ART_M_POSITION { ART_M_POSITION_CODE = "VENDOR" }).ART_M_POSITION_ID || m.POSITION_ID != (new ART_M_POSITION { ART_M_POSITION_CODE = "CUSTOMER" }).ART_M_POSITION_ID).ToList();
                    model.ObjCustomer = XECM_M_CUSTOMER_SERVICE.GetAll(context);
                    model.ObjVendor = XECM_M_VENDOR_SERVICE.GetAll(context);
                    model.ObjPack = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(new SAP_M_CHARACTERISTIC() { NAME = "ZPKG_SEC_GROUP" }, context);
                    model.ObjUserRole = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { USER_ID = userId }, context);
                    model.ObjUserTypeProduct = ART_M_USER_TYPE_OF_PRODUCT_SERVICE.GetByItem(new ART_M_USER_TYPE_OF_PRODUCT() { USER_ID = userId }, context);
                    model.ObjUserCompany = ART_M_USER_COMPANY_SERVICE.GetByItem(new ART_M_USER_COMPANY() { USER_ID = userId }, context);
                    model.ObjUserSup = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { USER_ID = userId }, context);
                    model.ObjUserCustomer = customer_;
                    model.ObjUserVendor = vendor_;
                    model.ObjVendorPack = ART_M_VENDOR_MATGROUP_SERVICE.GetByItem(new ART_M_VENDOR_MATGROUP() { VENDOR_ID = userId }, context);

                    model.ObjPosition = position_;

                    return View(model);
                }
            }
        }

        // GET: UsersRole/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_ROLE aRT_M_ROLE = db.ART_M_ROLE.Find(id);
            if (aRT_M_ROLE == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_ROLE);
        }

        // GET: UsersRole/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsersRole/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ROLE_ID,ROLE_CODE,DESCRIPTION,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_M_ROLE aRT_M_ROLE)
        {
            if (ModelState.IsValid)
            {
                db.ART_M_ROLE.Add(aRT_M_ROLE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aRT_M_ROLE);
        }

        // GET: UsersRole/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_ROLE aRT_M_ROLE = db.ART_M_ROLE.Find(id);
            if (aRT_M_ROLE == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_ROLE);
        }

        // POST: UsersRole/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ROLE_ID,ROLE_CODE,DESCRIPTION,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_M_ROLE aRT_M_ROLE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aRT_M_ROLE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aRT_M_ROLE);
        }

        // GET: UsersRole/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_ROLE aRT_M_ROLE = db.ART_M_ROLE.Find(id);
            if (aRT_M_ROLE == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_ROLE);
        }

        // POST: UsersRole/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ART_M_ROLE aRT_M_ROLE = db.ART_M_ROLE.Find(id);
            db.ART_M_ROLE.Remove(aRT_M_ROLE);
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
