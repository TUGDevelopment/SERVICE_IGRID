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

namespace PLL.Controllers
{
    public partial class UsersModel
    {
        public ART_M_USER ObjUser { get; set; }
        public ART_M_USER_2 ObjUserPost { get; set; }
        public List<ART_M_USER_2> ObjUserAll { get; set; }

        public ART_M_USER_VENDOR ObjVenID { get; set; }
        public XECM_M_VENDOR ObjVenNAME { get; set; }
        public ART_M_USER_CUSTOMER ObjCusID { get; set; }
        public XECM_M_CUSTOMER ObjCusNAME { get; set; }

        public ART_M_USER ObjPosID { get; set; }
        public ART_M_POSITION ObjPosNAME { get; set; }

        public string ObjPosition { get; set; }
        public string type2 { get; set; }
        public int CURRENT_USER_ID { get; set; }
    }

    public partial class UsersRequestModel : REQUEST_MODEL
    {
        public UsersModel data { get; set; }
    }
    public partial class UsersResultModel : RESULT_MODEL
    {
        public List<ART_M_USER_2> data { get; set; }
    }


    public class UsersController : Controller
    {
        private ARTWORKEntities db = new ARTWORKEntities();

        // GET: Users
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!CNService.IsAdmin(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), context))
                        return View("NoAuth");

                    return View();
                }
            }
        }
        public ActionResult Vendor()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "USERROLE", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult Customer()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "USERROLE", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult Internal()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "USERROLE", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult CustomerInfo()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "CUSTOMERINFO", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }

        public ActionResult VendorInfo()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "VENDORINFO", context))
                        return View("NoAuth");

                    return View();
                }
            }
        }


        private UsersModel GetList(string type)
        {
            UsersModel model = new UsersModel();

            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    model.ObjUser = ART_M_USER_SERVICE.GetByUSER_ID(BLL.Services.CNService.GetUserID(User.Identity.GetUserName(), context), context);
                    var PosID_Customer = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "CUSTOMER" }, context).FirstOrDefault().ART_M_POSITION_ID;
                    var PosID_VENDOR = ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_CODE = "VENDOR" }, context).FirstOrDefault().ART_M_POSITION_ID;
                    if (type == "Internal")
                    {
                        model.ObjUserAll = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetAll(context));
                        model.ObjUserAll = (from u1 in model.ObjUserAll
                                            where ((u1.POSITION_ID != PosID_Customer || u1.POSITION_ID != PosID_VENDOR) || u1.POSITION_ID == null)
                                            select u1).ToList();
                    }
                    else if (type == "Customer")
                        model.ObjUserAll = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { POSITION_ID = PosID_Customer }, context));
                    else if (type == "Vendor")
                        model.ObjUserAll = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { POSITION_ID = PosID_VENDOR }, context));
                    else if (type == "Admin")
                        model.ObjUserAll = MapperServices.ART_M_USER(ART_M_USER_SERVICE.GetAll(context));

                    List<ART_M_USER_2> _listUser = new List<ART_M_USER_2>();

                    foreach (var userall in model.ObjUserAll)
                    {
                        var listRoleID = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { USER_ID = userall.USER_ID }, context);
                        var listLeadID = ART_M_USER_UPPER_LEVEL_SERVICE.GetByItem(new ART_M_USER_UPPER_LEVEL() { USER_ID = userall.USER_ID }, context);
                        var listCompanyID = ART_M_USER_COMPANY_SERVICE.GetByItem(new ART_M_USER_COMPANY() { USER_ID = userall.USER_ID }, context);
                        var listTypyProductID = ART_M_USER_TYPE_OF_PRODUCT_SERVICE.GetByItem(new ART_M_USER_TYPE_OF_PRODUCT() { USER_ID = userall.USER_ID }, context);
                        var listCustomerID = ART_M_USER_CUSTOMER_SERVICE.GetByItem(new ART_M_USER_CUSTOMER() { USER_ID = userall.USER_ID }, context);
                        var listVendorID = ART_M_USER_VENDOR_SERVICE.GetByItem(new ART_M_USER_VENDOR() { USER_ID = userall.USER_ID }, context);
                        var listPositionID = ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { USER_ID = userall.USER_ID }, context);

                        foreach (var item in listRoleID)
                        {
                            if (ART_M_ROLE_SERVICE.GetByItem(new ART_M_ROLE() { ROLE_ID = item.ROLE_ID }, context).FirstOrDefault() != null)
                                if (string.IsNullOrEmpty(userall.ROLE_DISPLAY_TXT))
                                    userall.ROLE_DISPLAY_TXT = ART_M_ROLE_SERVICE.GetByItem(new ART_M_ROLE() { ROLE_ID = item.ROLE_ID }, context).FirstOrDefault().DESCRIPTION;
                                else
                                    userall.ROLE_DISPLAY_TXT += "<br/>" + ART_M_ROLE_SERVICE.GetByItem(new ART_M_ROLE() { ROLE_ID = item.ROLE_ID }, context).FirstOrDefault().DESCRIPTION;
                        }

                        foreach (var item in listLeadID)
                        {
                            if (ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { USER_ID = item.UPPER_USER_ID }, context).FirstOrDefault() != null)
                                if (string.IsNullOrEmpty(userall.USER_LEADER_DISPLAY_TXT))
                                    userall.USER_LEADER_DISPLAY_TXT = ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { USER_ID = item.UPPER_USER_ID }, context).FirstOrDefault().FIRST_NAME;
                                else
                                    userall.USER_LEADER_DISPLAY_TXT += "<br/>" + CNService.GetUserName(item.UPPER_USER_ID, context);
                        }

                        foreach (var item in listCompanyID)
                        {
                            if (string.IsNullOrEmpty(userall.COMPANY_DISPLAY_TXT))
                                userall.COMPANY_DISPLAY_TXT = SAP_M_COMPANY_SERVICE.GetByItem(new SAP_M_COMPANY() { COMPANY_ID = item.COMPANY_ID }, context).FirstOrDefault().DESCRIPTION;
                            else
                                userall.COMPANY_DISPLAY_TXT += "<br/>" + SAP_M_COMPANY_SERVICE.GetByItem(new SAP_M_COMPANY() { COMPANY_ID = item.COMPANY_ID }, context).FirstOrDefault().DESCRIPTION;
                        }

                        foreach (var item in listTypyProductID)
                        {
                            if (SAP_M_TYPE_OF_PRODUCT_SERVICE.GetByItem(new SAP_M_TYPE_OF_PRODUCT() { TYPE_OF_PRODUCT_ID = item.TYPE_OF_PRODUCT_ID }, context).FirstOrDefault() != null)
                                if (string.IsNullOrEmpty(userall.TYPE_OF_PRODUCT_DISPLAY_TXT))
                                    userall.TYPE_OF_PRODUCT_DISPLAY_TXT = SAP_M_TYPE_OF_PRODUCT_SERVICE.GetByItem(new SAP_M_TYPE_OF_PRODUCT() { TYPE_OF_PRODUCT_ID = item.TYPE_OF_PRODUCT_ID }, context).FirstOrDefault().DESCRIPTION;
                                else
                                    userall.TYPE_OF_PRODUCT_DISPLAY_TXT += "<br/>" + SAP_M_TYPE_OF_PRODUCT_SERVICE.GetByItem(new SAP_M_TYPE_OF_PRODUCT() { TYPE_OF_PRODUCT_ID = item.TYPE_OF_PRODUCT_ID }, context).FirstOrDefault().DESCRIPTION;
                        }

                        foreach (var item in listCustomerID)
                        {
                            if (XECM_M_CUSTOMER_SERVICE.GetByItem(new XECM_M_CUSTOMER() { CUSTOMER_ID = item.CUSTOMER_ID }, context).FirstOrDefault() != null)
                                if (string.IsNullOrEmpty(userall.CUSTOMER_DISPLAY_TXT))
                                    userall.CUSTOMER_DISPLAY_TXT = CNService.GetCustomerCodeName(item.CUSTOMER_ID, context);
                                else
                                    userall.CUSTOMER_DISPLAY_TXT += "<br/>" + CNService.GetCustomerCodeName(item.CUSTOMER_ID, context);
                        }

                        foreach (var item in listVendorID)
                        {
                            if (XECM_M_VENDOR_SERVICE.GetByItem(new XECM_M_VENDOR() { VENDOR_ID = item.VENDOR_ID }, context).FirstOrDefault() != null)
                                if (string.IsNullOrEmpty(userall.VENDOR_DISPLAY_TXT))
                                    userall.VENDOR_DISPLAY_TXT = CNService.GetVendorCodeName(item.VENDOR_ID, context);
                                else
                                    userall.VENDOR_DISPLAY_TXT += "<br/>" + CNService.GetVendorCodeName(item.VENDOR_ID, context);
                        }

                        foreach (var item in listPositionID)
                        {
                            if (ART_M_POSITION_SERVICE.GetByItem(new ART_M_POSITION() { ART_M_POSITION_ID = Convert.ToInt32(item.POSITION_ID) }, context).FirstOrDefault() != null)
                                if (string.IsNullOrEmpty(userall.POSITION_DISPLAY_TXT))
                                    userall.POSITION_DISPLAY_TXT = ART_M_POSITION_SERVICE.GetByART_M_POSITION_ID(item.POSITION_ID, context).ART_M_POSITION_NAME;
                        }

                        _listUser.Add(userall);

                    }

                    model.ObjUserAll = _listUser;
                }
            }
            return model;
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_USER aRT_M_USER = db.ART_M_USER.Find(id);
            if (aRT_M_USER == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_USER);
        }

        // GET: Users/Create
        public ActionResult Create(string position_)
        {
            ViewBag.ObjPosition = position_;

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "USERNAME,PASSWORD,DISPLAY_NAME,TITLE,FIRST_NAME,LAST_NAME,EMAIL,POSITION_ID,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_M_USER_2 aRT_M_USER, string position_)
        {
            if (ModelState.IsValid)
            {
                using (var context = new ARTWORKEntities())
                {
                    if (ART_M_USER_SERVICE.GetByItem(new ART_M_USER { USERNAME = aRT_M_USER.USERNAME.ToUpper() }, context).FirstOrDefault() == null)
                    {
                        string username = aRT_M_USER.USERNAME.ToUpper();
                        if (position_ == "Vendor")
                        {
                            var tempUser = (from c in context.ART_M_USER
                                            where ("V" + c.USERNAME + "_").Contains("V" + username + "_")
                                            select c).ToList();

                            int n = tempUser.Count + 1;
                            if (position_ == "Vendor")
                            {
                                aRT_M_USER.USERNAME = "V" + username + "_" + n.ToString("D3");
                                aRT_M_USER.PASSWORD = "vendor1234";
                            }
                        }
                        else if (position_ == "Customer")
                        {
                            var tempUser = (from c in context.ART_M_USER
                                            where ("C" + c.USERNAME + "_").Contains("C" + username + "_")
                                            select c).ToList();

                            int n = tempUser.Count + 1;
                            if (position_ == "Customer")
                            {
                                aRT_M_USER.USERNAME = "C" + username + "_" + n.ToString("D3");
                                aRT_M_USER.PASSWORD = "customer1234";
                            }
                        }

                        if (aRT_M_USER.PASSWORD != null)
                        {
                            aRT_M_USER.PASSWORD = EncryptionService.Encrypt(aRT_M_USER.PASSWORD.Trim());
                        }

                        if (string.IsNullOrEmpty(aRT_M_USER.TITLE))
                        {
                            aRT_M_USER.TITLE = "";
                        }

                        aRT_M_USER.IS_ACTIVE = "X";
                        aRT_M_USER.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                        aRT_M_USER.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                        var user = MapperServices.ART_M_USER(aRT_M_USER);
                        user.USERNAME = user.USERNAME.ToUpper();
                        ART_M_USER_SERVICE.SaveOrUpdate(user, context);

                        if (position_ == "Internal")
                        {
                            return RedirectToAction("Index", "UsersRole", new { userid = user.USER_ID, position_ = "Internal" });
                        }
                        else if (position_ == "Vendor")
                        {
                            var temp = ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { USERNAME = aRT_M_USER.USERNAME }, context).FirstOrDefault();
                            if (temp != null)
                            {
                                var vendor = XECM_M_VENDOR_SERVICE.GetByItem(new XECM_M_VENDOR() { VENDOR_CODE = username }, context).FirstOrDefault();
                                if (vendor != null)
                                {
                                    ART_M_USER_VENDOR uservendor = new ART_M_USER_VENDOR();
                                    uservendor.USER_ID = temp.USER_ID;
                                    uservendor.VENDOR_ID = vendor.VENDOR_ID;
                                    uservendor.IS_EMAIL_TO = "X";
                                    uservendor.IS_EMAIL_CC = "";
                                    uservendor.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                                    uservendor.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                                    ART_M_USER_VENDOR_SERVICE.SaveOrUpdate(uservendor, context);
                                }
                            }

                            return RedirectToAction("Index", "UsersRole", new { userid = user.USER_ID, position_ = "Vendor" });
                        }
                        else if (position_ == "Customer")
                        {
                            var temp = ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { USERNAME = aRT_M_USER.USERNAME }, context).FirstOrDefault();
                            if (temp != null)
                            {
                                var customer = XECM_M_CUSTOMER_SERVICE.GetByItem(new XECM_M_CUSTOMER() { CUSTOMER_CODE = username }, context).FirstOrDefault();
                                if (customer != null)
                                {
                                    ART_M_USER_CUSTOMER usercustomer = new ART_M_USER_CUSTOMER();
                                    usercustomer.USER_ID = temp.USER_ID;
                                    usercustomer.CUSTOMER_ID = customer.CUSTOMER_ID;
                                    usercustomer.IS_EMAIL_TO = "X";
                                    usercustomer.IS_EMAIL_CC = "";
                                    usercustomer.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                                    usercustomer.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                                    ART_M_USER_CUSTOMER_SERVICE.SaveOrUpdate(usercustomer, context);
                                }
                            }

                            return RedirectToAction("Index", "UsersRole", new { userid = user.USER_ID, position_ = "Customer" });
                        }
                        else if (position_ == "Admin")
                        {
                            return RedirectToAction("Index", "UsersRole", new { userid = user.USER_ID, position_ = "Admin" });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The username is duplicate.");
                        ViewBag.ObjPosition = position_;
                        return View(aRT_M_USER);
                    }
                }
            }
            ViewBag.ObjPosition = position_;

            return View(aRT_M_USER);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id, string position_)
        {
            ViewBag.ObjPosition = position_;
            ART_M_USER aRT_M_USER = db.ART_M_USER.Find(id);
            if (!string.IsNullOrEmpty(aRT_M_USER.PASSWORD)) aRT_M_USER.PASSWORD = EncryptionService.Decrypt(aRT_M_USER.PASSWORD.Trim());
            if (aRT_M_USER == null)
            {
                return HttpNotFound();
            }
            return View(MapperServices.ART_M_USER(aRT_M_USER));
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "USER_ID,USERNAME,PASSWORD,DISPLAY_NAME,TITLE,FIRST_NAME,LAST_NAME,EMAIL,POSITION_ID,IS_ACTIVE,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY")] ART_M_USER_2 aRT_M_USER, string position_)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(aRT_M_USER.TITLE))
                {
                    aRT_M_USER.TITLE = "";
                }
                using (var context = new ARTWORKEntities())
                {
                    var chk = ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { USER_ID = aRT_M_USER.USER_ID }, context).FirstOrDefault();

                    if (aRT_M_USER.PASSWORD != null)
                    {
                        aRT_M_USER.PASSWORD = EncryptionService.Encrypt(aRT_M_USER.PASSWORD.Trim());
                    }
                    else
                    {
                        aRT_M_USER.PASSWORD = chk.PASSWORD;
                    }

                    if (aRT_M_USER.IS_ACTIVE == null)
                        aRT_M_USER.IS_ACTIVE = "";

                    aRT_M_USER.IS_ADUSER = chk.IS_ADUSER;
                    aRT_M_USER.CREATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    aRT_M_USER.UPDATE_BY = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    ART_M_USER_SERVICE.SaveOrUpdate(MapperServices.ART_M_USER(aRT_M_USER), context);
                }
                if (position_ == "Internal")
                    return RedirectToAction("Internal");
                else if (position_ == "Vendor")
                    return RedirectToAction("Vendor");
                else if (position_ == "Customer")
                    return RedirectToAction("Customer");
                else if (position_ == "Admin")
                    return RedirectToAction("Index");
                else
                    return HttpNotFound();
            }
            return View(aRT_M_USER);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id, string position_)
        {
            ViewBag.ObjPosition = position_;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ART_M_USER aRT_M_USER = db.ART_M_USER.Find(id);
            aRT_M_USER.PASSWORD = EncryptionService.Decrypt(aRT_M_USER.PASSWORD.Trim());
            if (aRT_M_USER == null)
            {
                return HttpNotFound();
            }
            return View(aRT_M_USER);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string position_)
        {
            using (var context = new ARTWORKEntities())
            {
                var a = ART_M_USER_SERVICE.GetByUSER_ID(id, context);
                a.IS_ACTIVE = "";
                ART_M_USER_SERVICE.SaveOrUpdate(a, context);
                //ART_M_USER aRT_M_USER = db.ART_M_USER.Find(id);
                //db.ART_M_USER.Remove(aRT_M_USER);
                //db.SaveChanges();
                if (position_ == "Internal")
                    return RedirectToAction("Internal");
                else if (position_ == "Vendor")
                    return RedirectToAction("Vendor");
                else if (position_ == "Customer")
                    return RedirectToAction("Customer");
                else if (position_ == "Admin")
                    return RedirectToAction("Index");
                else
                    return HttpNotFound();
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
