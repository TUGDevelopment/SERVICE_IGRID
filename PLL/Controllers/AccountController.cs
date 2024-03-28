using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PLL.Models;
using BLL.Services;
using DAL;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace PLL.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //// string userNamec = Request.LogonUserIdentity.Name; ;
            ////string userNamec = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            //string Name = new System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent()).Identity.Name;
            //Name += "xxx" + System.Environment.UserName;
            //Name += "xxx" + Environment.GetEnvironmentVariable("USERNAME");
            //Name += "xxx" + System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            ////var userName = filterContext.RequestContext.HttpContext.User.Identity.Name;

            //var log = new ART_SYS_LOG();
            //log.ACTION = "I";
            //log.CREATE_BY = -1;
            //log.CREATE_DATE = DateTime.Now;
            //log.TABLE_NAME = "Auto Login By";
            //log.NEW_VALUE = Name;
            //log.UPDATE_BY = -1;
            //log.UPDATE_DATE = DateTime.Now;
            //ART_SYS_LOG_SERVICE.SaveNoLog(log);

            //if (BLL.Services.CNService.HasUser(userNamec))
            //{
            //    FormsAuthentication.SetAuthCookie(userNamec, false);
            //}

            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            //var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            //switch (result)
            //{
            //    case SignInStatus.Success:
            //        return RedirectToLocal(returnUrl);
            //    case SignInStatus.LockedOut:
            //        return View("Lockout");
            //    case SignInStatus.RequiresVerification:
            //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            //    case SignInStatus.Failure:
            //    default:
            //        ModelState.AddModelError("", "Invalid login attempt.");
            //        return View(model);
            //}
            using (var context = new ARTWORKEntities())
            {
                bool result = false;
                DAL.ART_M_USER item = new DAL.ART_M_USER();
                using (CNService.IsolationLevel(context))
                {
                    //bool result = false;  // by aof commeted
                    //DAL.ART_M_USER item = new DAL.ART_M_USER();
                    item.USERNAME = model.UserName.ToUpper().Trim();
                    item.PASSWORD = EncryptionService.Encrypt(model.Password.Trim());
                    item.IS_ACTIVE = "X";
                    var user = ART_M_USER_SERVICE.GetByItem(item, context);
                    if (user.Count == 1)
                    {
                        result = true;
                        FormsAuthentication.SetAuthCookie(user[0].USERNAME, model.RememberMe);
                    }

                    //if (result)
                    //    return RedirectToLocal(returnUrl);
                    //else
                    //{

                    //    ModelState.AddModelError("", "The user name or password is incorrect.");
                    //    return View(model);
                    //}
                }

                if (result)
                    return RedirectToLocal(returnUrl);
                else
                {

                    // by aof keep log in case authen fail
                    var log = new ART_SYS_LOG();
                    var dat = DateTime.Now;
                    log.TABLE_NAME = "Login";
                    log.CREATE_BY = -1;
                    log.UPDATE_BY = -1;
                    log.CREATE_DATE = dat;
                    log.UPDATE_DATE = dat;
                    log.ACTION = "Login fail";
                    log.ERROR_MSG = "The user name or password is incorrect.";
                    log.NEW_VALUE = "USERNAME:" + item.USERNAME + ",PASS:" + item.PASSWORD; //User.Identity.GetUserName();
                    log.OLD_VALUE = Request.UserHostAddress + "-" + Request.Browser.Browser + "-" + Request.Browser.Version;
                    ART_SYS_LOG_SERVICE.SaveNoLog(log, context);
                    // by aof keep log in case authen fail

                    ModelState.AddModelError("", "The user name or password is incorrect.");
                    return View(model);
                }
            }
        }

        ////
        //// GET: /Account/VerifyCode
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    // Require that the user has already logged in via username/password or external login
        //    if (!await SignInManager.HasBeenVerifiedAsync())
        //    {
        //        return View("Error");
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // The following code protects for brute force attacks against the two factor codes. 
        //    // If a user enters incorrect codes for a specified amount of time then the user account 
        //    // will be locked out for a specified amount of time. 
        //    // You can configure the account lockout settings in IdentityConfig
        //    var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(model.ReturnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid code.");
        //            return View(model);
        //    }
        //}

        ////
        //// GET: /Account/Register
        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

        //            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
        //            // Send an email with this link
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        //            return RedirectToAction("Index", "Home");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //// GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await UserManager.ConfirmEmailAsync(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        ////
        //// GET: /Account/ForgotPassword
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        //        // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //        // return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //// GET: /Account/ForgotPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}

        //
        // GET: /Account/ResetPassword
        //[AllowAnonymous]
        public ActionResult ChangePassword()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var user = ART_M_USER_SERVICE.GetByUSER_ID(BLL.Services.CNService.GetUserID(User.Identity.GetUserName(), context), context);
                    if (user != null)
                    {
                        if (user.IS_ADUSER == "X")
                        {
                            ModelState.AddModelError("", "Your account is internal user please contact admin for request new password.");
                            return View("CannotChangePassword");
                        }
                    }
                }
            }

            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool result = false;
            using (var context = new ARTWORKEntities())
            {
                var user = ART_M_USER_SERVICE.GetByUSER_ID(BLL.Services.CNService.GetUserID(User.Identity.GetUserName(), context), context);
                if (user != null)
                {
                    if (user.PASSWORD == EncryptionService.Encrypt(model.OldPassword))
                    {
                        result = true;

                        user.UPDATE_BY = CNService.getCurrentUser(context);

                        user.PASSWORD = EncryptionService.Encrypt(model.Password.Trim());
                        ART_M_USER_SERVICE.SaveOrUpdate(user, context);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Current password is incorrect.");
                        return View(model);
                    }
                }
            }
            if (result)
                return RedirectToLocal("");
            else
            {
                //ModelState.AddModelError("", "The user name or password is incorrect.");
                return View(model);
            }

            //var user = await UserManager.FindByNameAsync(model.Email);
            //if (user == null)
            //{
            //    // Don't reveal that the user does not exist
            //    return RedirectToAction("ResetPasswordConfirmation", "Account");
            //}
            //var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            //if (result.Succeeded)
            //{
            //    return RedirectToAction("ResetPasswordConfirmation", "Account");
            //}
            //AddErrors(result);
            //return View();
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ViewBag.isError = false;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool result = false;
            using (var context = new ARTWORKEntities())
            {
                if (model.Username != null) model.Username = model.Username.ToUpper();
                var user = ART_M_USER_SERVICE.GetByItem(new DAL.ART_M_USER() { EMAIL = model.Email, USERNAME = model.Username }, context);
                if (user != null)
                {
                    if (user.Count == 1)
                    {
                        if (user[0].IS_ADUSER == "X")
                        {
                            ModelState.AddModelError("", "Your account is internal user please contact admin for request new password.");
                            return View(model);
                        }

                        result = true;
                        Random r = new Random();
                        var ran = r.Next(0, 1000);
                        var verifyCode = EncryptionService.EncryptAndUrlEncode(user[0].USERNAME + "_XYZ_" + ran.ToString());
                        user[0].VERIFY_CODE = verifyCode;
                        ART_M_USER_SERVICE.SaveOrUpdate(user[0], context);

                        BLL.Services.EmailService.SendEmailForgotPassword(model.Email, user[0].USERNAME, verifyCode);
                    }
                    else if (user.Count > 1)
                    {
                        ModelState.AddModelError("", "There're a lot of email in database please enter Username.");
                        ViewBag.isError = true;
                        return View(model);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Your email is incorrect.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Your email is incorrect.");
                    return View(model);
                }
            }

            if (result)
                return RedirectToLocal("");
            else
            {
                //ModelState.AddModelError("", "The user name or password is incorrect.");
                return View(model);
            }

            //var user = await UserManager.FindByNameAsync(model.Email);
            //if (user == null)
            //{
            //    // Don't reveal that the user does not exist
            //    return RedirectToAction("ResetPasswordConfirmation", "Account");
            //}
            //var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            //if (result.Succeeded)
            //{
            //    return RedirectToAction("ResetPasswordConfirmation", "Account");
            //}
            //AddErrors(result);
            //return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string d)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var userName = EncryptionService.Decrypt(d);
                    userName = Regex.Split(userName, "_XYZ_")[0];
                    if (userName != null) userName = userName.ToUpper();
                    var user = ART_M_USER_SERVICE.GetByItem(new DAL.ART_M_USER() { USERNAME = userName }, context);

                    if (user == null)
                    {
                        return View("NoAuth");
                    }
                    if (user[0].VERIFY_CODE == null)
                    {
                        return View("NoAuth");
                    }
                    else
                    {
                        ViewBag.userName = userName;
                    }

                    return View();
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (var context = new ARTWORKEntities())
            {
                if (model.UserName != null) model.UserName = model.UserName.ToUpper();
                bool result = false;
                var user = ART_M_USER_SERVICE.GetByItem(new DAL.ART_M_USER() { USERNAME = model.UserName }, context);
                if (user == null)
                {
                    return View("NoAuth");
                }
                else
                {
                    result = true;
                    user[0].VERIFY_CODE = null;
                    user[0].PASSWORD = EncryptionService.Encrypt(model.Password.Trim());
                    ART_M_USER_SERVICE.SaveOrUpdate(user[0], context);
                }

                if (result)
                    return RedirectToLocal("");
                else
                {
                    return View(model);
                }
            }
        }

        ////
        //// GET: /Account/ResetPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        ////
        //// GET: /Account/SendCode
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        //{
        //    var userId = await SignInManager.GetVerifiedUserIdAsync();
        //    if (userId == null)
        //    {
        //        return View("Error");
        //    }
        //    var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        //    var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SendCode(SendCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    // Generate the token and send it
        //    if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
        //    {
        //        return View("Error");
        //    }
        //    return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        //}

        ////
        //// GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        //        case SignInStatus.Failure:
        //        default:
        //            // If the user does not have an account, then prompt the user to create an account
        //            ViewBag.ReturnUrl = returnUrl;
        //            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        //    }
        //}

        ////
        //// POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        //
        // POST: /Account/LogOff
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}