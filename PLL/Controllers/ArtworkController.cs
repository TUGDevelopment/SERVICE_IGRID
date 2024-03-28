using BLL.Services;
using DAL;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PLL.Controllers
{
    public class ArtworkController : Controller
    {
        public ActionResult Index(int? id)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var UserID2 = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);
                    if (BLL.Services.CNService.IsCustomer(UserID2, context))
                        return View("NoAuth");
                    if (BLL.Services.CNService.IsVendor(UserID2, context))
                        return View("NoAuth");

                    ViewBag.FFCDefaultReviewer_UserID = ConfigurationManager.AppSettings["FFCDefaultReviewer_UserID"];
                    ViewBag.FFCDefaultReviewer_UserName = CNService.GetUserName(Convert.ToInt32(ViewBag.FFCDefaultReviewer_UserID), context);
                    ViewBag.FFCDefaultEmailTo_UserID = ConfigurationManager.AppSettings["FFCDefaultEmailTo_UserID"];
                    ViewBag.FFCDefaultEmailTo_UserName = CNService.GetUserName(Convert.ToInt32(ViewBag.FFCDefaultEmailTo_UserID), context);
                    ViewBag.FFCDefaultEmailCC_UserID = ConfigurationManager.AppSettings["FFCDefaultEmailCC_UserID"];
                    ViewBag.FFCDefaultEmailCC_UserName = CNService.GetUserName(Convert.ToInt32(ViewBag.FFCDefaultEmailCC_UserID), context);
                    ViewBag.ArtworkRequestId = id == null ? 0 : id;
                    ViewBag.ReadOnly = "0";

                    var requestForm = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(id, context);
                    var artworkProcess = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_REQUEST_ID = ViewBag.ArtworkRequestId }, context).ToList();
                    if (artworkProcess.Count > 0)
                    {
                        ViewBag.ReadOnly = "1";
                    }
                    else
                    {
                        if (requestForm.UPLOAD_BY == null)
                        {
                            //repeat
                            if (requestForm.CREATOR_ID != UserID2)
                            {
                                ViewBag.ReadOnly = "1";
                            }
                        }
                        else
                        {
                            //new
                            var RECIPIENT = ART_WF_ARTWORK_REQUEST_RECIPIENT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_RECIPIENT()
                            {
                                ARTWORK_REQUEST_ID = ViewBag.ArtworkRequestId,
                                RECIPIENT_USER_ID = UserID2
                            }, context).ToList();
                            if (RECIPIENT.Count == 0)
                            {
                                ViewBag.ReadOnly = "1";
                            }
                        }
                    }

                    ViewBag.CreateRequestByFFC = "0";
                    if (CNService.IsFFC(requestForm.CREATE_BY, context))
                        ViewBag.CreateRequestByFFC = "1";

                    return View();
                }
            }
        }
    }
}