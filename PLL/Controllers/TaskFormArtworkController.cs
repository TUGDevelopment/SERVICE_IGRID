using BLL.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using BLL.Helpers;

namespace PLL.Controllers
{
    public class TaskFormArtworkController : Controller
    {
        // GET: TaskFormArtwork
        public ActionResult Index(int? id)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var artworkSubId = id.HasValue ? id : 0;
                    ViewBag.ArtworkSubId = artworkSubId;
                    var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(artworkSubId, context);
                    var requestForm = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(process.ARTWORK_REQUEST_ID, context);

                    ViewBag.CreateRequestByFFC = "0";
                    if (CNService.IsFFC(requestForm.CREATE_BY, context))
                        ViewBag.CreateRequestByFFC = "1";

                    ViewBag.ReadOnly = "0";
                    ViewBag.RequestMaterial = "0";
                    var artwork_process_pa = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PA() { ARTWORK_SUB_ID = Convert.ToInt32(artworkSubId) }, context).FirstOrDefault();
                    if (artwork_process_pa != null)
                    {
                        if (artwork_process_pa.REQUEST_MATERIAL_STATUS == "Waiting for approval")
                        {
                            ViewBag.RequestMaterial = "1";
                        }
                    }

                    var UserID = BLL.Services.CNService.GetUserID(User.Identity.GetUserName(), context);
                    var UserID2 = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);

                    var isAdmin = CNService.IsAdmin(UserID2, context);


                    // by aof show btn so change for admin
                    ViewBag.IsAdmin = "0";
                    if (isAdmin)
                    {
                        ViewBag.IsAdmin = "1";                
                    }
                    // by aof show btn so change for admin


                    if (isAdmin)
                    {
                        ViewBag.ReadOnly = "1";
                    }
                    else
                    {
                        if (process.IS_END == "X")
                        {
                            ViewBag.ReadOnly = "1";
                        }

                        if (process.CURRENT_USER_ID != null)
                        {
                            var isCustomer = CNService.IsCustomer(UserID2, context);
                            var isVendor = CNService.IsVendor(UserID2, context);
                            var isTHolding = CNService.IsTHolding(UserID2, context);
                            var listCusDummy = ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER() { ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID, MAIL_CC = "X", CUSTOMER_USER_ID = UserID2 }, context);
                      

                            if (isCustomer)
                            {
                                if (process.IS_END == "X")
                                {
                                    ViewBag.ReadOnly = "1";
                                }
                                if (UserID != process.CURRENT_USER_ID)
                                {
                                    return View("NoAuth");
                                }
                                else
                                {
                                    var isReassign = ART_WF_LOG_REASSIGN_SERVICE.GetByItem(new ART_WF_LOG_REASSIGN() { WF_SUB_ID = process.ARTWORK_SUB_ID, WF_TYPE = "A" }, context);
                                    var listCusto = ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER() { ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID, MAIL_TO = "X", CUSTOMER_USER_ID = UserID2 }, context);
                                    if (listCusto.Count == 0)
                                    {
                                        ViewBag.ReadOnly = "1";
                                        if (isReassign.Count > 0)
                                            if (isReassign.OrderByDescending(q => q.CREATE_DATE).FirstOrDefault().TO_USER_ID == UserID2)
                                                ViewBag.ReadOnly = "0";
                                    }
                                }
                            }
                            else if (isTHolding)
                            {
                                if (process.IS_END == "X")
                                {
                                    ViewBag.ReadOnly = "1";
                                }
                                if (UserID != process.CURRENT_USER_ID)
                                {
                                    ViewBag.ReadOnly = "1";
                                }
                                else
                                {
                                    var isReassign = ART_WF_LOG_REASSIGN_SERVICE.GetByItem(new ART_WF_LOG_REASSIGN() { WF_SUB_ID = process.ARTWORK_SUB_ID, WF_TYPE = "A" }, context);
                                    var listCusto = ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER() { ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID, MAIL_TO = "X", CUSTOMER_USER_ID = UserID2 }, context);
                                    if (listCusto.Count == 0)
                                    {
                                        ViewBag.ReadOnly = "1";
                                        if (isReassign.Count > 0)
                                            if (isReassign.OrderByDescending(q => q.CREATE_DATE).FirstOrDefault().TO_USER_ID == UserID2)
                                                ViewBag.ReadOnly = "0";
                                    }
                                    var currstep = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_ID = (int)process.CURRENT_STEP_ID }, context).FirstOrDefault();
                                    if (currstep != null)
                                        if(!currstep.STEP_ARTWORK_CODE.StartsWith("SEND_CUS"))
                                            ViewBag.ReadOnly = "0";
                                }
                            }
                            else if (isVendor)
                            {
                                if (process.IS_END == "X")
                                {
                                    ViewBag.ReadOnly = "1";
                                }
                                if (UserID != process.CURRENT_USER_ID)
                                {
                                    return View("NoAuth");
                                }
                            }
                            else if (listCusDummy.Count > 0)
                            {
                                ViewBag.ReadOnly = "1";
                            }
                            else
                            {
                                if (UserID != process.CURRENT_USER_ID)
                                {
                                    ViewBag.ReadOnly = "1";
                                }
                            }
                        }
                        else if (process.CURRENT_ROLE_ID != null)
                        {
                            var listRole = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { USER_ID = UserID2 }, context);
                            if (listRole.Where(m => m.ROLE_ID == process.CURRENT_ROLE_ID).Count() == 0)
                            {
                                ViewBag.ReadOnly = "1";
                            }
                            else
                            {
                                var allStepArtwork = ART_M_STEP_ARTWORK_SERVICE.GetAll(context);
                                var listUserCompany = ART_M_USER_COMPANY_SERVICE.GetByItem(new ART_M_USER_COMPANY() { USER_ID = Convert.ToInt32(UserID2) }, context);
                                var listUserTypeofProduct = ART_M_USER_TYPE_OF_PRODUCT_SERVICE.GetByItem(new ART_M_USER_TYPE_OF_PRODUCT() { USER_ID = Convert.ToInt32(UserID2) }, context);

                                var valid = CNService.CheckTypeOfProductAndCompanyArtwork(UserID2, process.ARTWORK_REQUEST_ID, Convert.ToInt32(id), context, allStepArtwork, listUserCompany, listUserTypeofProduct);
                                if (!valid)
                                {
                                    ViewBag.ReadOnly = "1";
                                }
                            }
                        }
                    }

                    ViewBag.MainArtworkSubId = CNService.FindParentArtworkSubId(id.Value, context);

                    ViewBag.CURRENT_STEP_CODE_DISPLAY_TXT = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_ID = Convert.ToInt32(process.CURRENT_STEP_ID) }, context).FirstOrDefault().STEP_ARTWORK_CODE;
                    ViewBag.CURRENT_STEP_CODE_FOR = "Internal";
                    if (ViewBag.CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_SL") ViewBag.CURRENT_STEP_CODE_FOR = "Vendor";
                    if (ViewBag.CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_PM") ViewBag.CURRENT_STEP_CODE_FOR = "Vendor";
                    if (ViewBag.CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_PO") ViewBag.CURRENT_STEP_CODE_FOR = "Vendor";

                    if (ViewBag.CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_CUS_REVIEW") ViewBag.CURRENT_STEP_CODE_FOR = "Customer";
                    if (ViewBag.CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_CUS_PRINT") ViewBag.CURRENT_STEP_CODE_FOR = "Customer";
                    if (ViewBag.CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_CUS_SHADE") ViewBag.CURRENT_STEP_CODE_FOR = "Customer";
                    if (ViewBag.CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_CUS_REQ_REF") ViewBag.CURRENT_STEP_CODE_FOR = "Customer";
                    if (process != null)
                    {
                        ViewBag.ArtworkRequestId = process.ARTWORK_REQUEST_ID;
                        ViewBag.ArtworkItemId = process.ARTWORK_ITEM_ID;
                    }

                    ViewBag.DefaultResonId = ART_M_DECISION_REASON_SERVICE.GetByItem(new ART_M_DECISION_REASON() { IS_DEFAULT = "X" }, context)[0].ART_M_DECISION_REASON_ID;
                    ViewBag.DefaultResonTxt = ART_M_DECISION_REASON_SERVICE.GetByItem(new ART_M_DECISION_REASON() { IS_DEFAULT = "X" }, context)[0].DESCRIPTION;

                    int ARTWORK_ITEM_ID = process.ARTWORK_ITEM_ID;
                    var REQUEST_ITEM_NO = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(ARTWORK_ITEM_ID, context).REQUEST_ITEM_NO;
                    if (!string.IsNullOrEmpty(REQUEST_ITEM_NO))
                    {
                        ViewBag.Title = REQUEST_ITEM_NO;
                    }
                    else
                    {
                        ViewBag.Title = "Draft";
                    }

                    ViewBag.OverDue = ArtworkProcessHelper.CheckOverDue(process, context);
                    //ViewBag.OverDue = "0";
                    //var duration = process.IS_STEP_DURATION_EXTEND.Equals("X") ? ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(process.CURRENT_STEP_ID, context).DURATION_EXTEND : ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(process.CURRENT_STEP_ID, context).DURATION;
                    //DateTime dtReceiveWf = process.CREATE_DATE;
                    //DateTime dtWillFinish = CNService.AddBusinessDays(dtReceiveWf, (int)Math.Ceiling(duration.Value));
                    //if (DateTime.Now.Date > dtWillFinish.Date)
                    //{
                    //    ViewBag.OverDue = "1";
                    //}

                    ViewBag.WFCompletedOrTerminated = "0";
                    if (process.IS_END == "X" || process.IS_TERMINATE == "X" || !string.IsNullOrEmpty(process.REMARK_KILLPROCESS))
                    {
                        ViewBag.WFCompletedOrTerminated = "1";
                    }

                    ViewBag.LockWF = "0";
                    if (CNService.IsLock(id.Value, context))
                    {
                        ViewBag.LockWF = "1";
                    }

                    ViewBag.ShowExtendStepDuration = "0";
                    if (ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(process.CURRENT_STEP_ID, context).DURATION_EXTEND > 0 && process.CURRENT_USER_ID != null)
                    {
                        if ((!string.IsNullOrEmpty(process.IS_STEP_DURATION_EXTEND) && ViewBag.ReadOnly == "1") || (!string.IsNullOrEmpty(process.IS_STEP_DURATION_EXTEND) && ViewBag.ReadOnly == "0") || (string.IsNullOrEmpty(process.IS_STEP_DURATION_EXTEND) && ViewBag.ReadOnly == "0"))
                        {
                            ViewBag.ShowExtendStepDuration = "1";
                        }
                    }
                }
            }

            return View();
        }
    }
}