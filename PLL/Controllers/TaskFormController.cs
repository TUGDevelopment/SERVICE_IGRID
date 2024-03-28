using BLL.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using BLL.Helpers;

namespace PLL.Controllers
{
    public class TaskFormController : Controller
    {
        public ActionResult Index(int? id)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(id, context);
                    var checkListId = CNService.ConvertMockupIdToCheckListId(process.MOCKUP_ID, context);
                    var checkList = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(checkListId, context);
                    ViewBag.ReadOnly = "0";
                    ViewBag.HasAuth = "1";

                    var UserID = BLL.Services.CNService.GetUserID(User.Identity.GetUserName(), context);
                    var UserID2 = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context);

                    var isAdmin = CNService.IsAdmin(UserID2, context);
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
                        ViewBag.ShowExtendDuration = process.CURRENT_USER_ID != null ? "1" : "0";
                        if (process.CURRENT_USER_ID != null)
                        {
                            var isCustomer = CNService.IsCustomer(UserID2, context);
                            var isVendor = CNService.IsVendor(UserID2, context);
                            var isTHolding = CNService.IsTHolding(UserID2, context);
                            //ticket 150109 by voravut
                            var listCusDummy = ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER() { CHECK_LIST_ID = checkListId, MAIL_CC = "X", CUSTOMER_USER_ID = UserID2 }, context);

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
                                    var isReassign = ART_WF_LOG_REASSIGN_SERVICE.GetByItem(new ART_WF_LOG_REASSIGN() { WF_SUB_ID = process.MOCKUP_SUB_ID, WF_TYPE = "M" }, context);
                                    var listCuscc = ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER() { CHECK_LIST_ID = checkListId, MAIL_CC = "X", CUSTOMER_USER_ID = UserID2 }, context);
                                    if (listCuscc.Count > 0)
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
                                    var isReassign = ART_WF_LOG_REASSIGN_SERVICE.GetByItem(new ART_WF_LOG_REASSIGN() { WF_SUB_ID = process.MOCKUP_SUB_ID, WF_TYPE = "M" }, context);
                                    var listCuscc = ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER() { CHECK_LIST_ID = checkListId, MAIL_CC = "X", CUSTOMER_USER_ID = UserID2 }, context);
                                    if (listCuscc.Count > 0)
                                    {
                                        ViewBag.ReadOnly = "1";
                                        if (isReassign.Count > 0)
                                            if (isReassign.OrderByDescending(q => q.CREATE_DATE).FirstOrDefault().TO_USER_ID == UserID2)
                                                ViewBag.ReadOnly = "0";
                                    }
                                    var currstep = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_ID = (int)process.CURRENT_STEP_ID }, context).FirstOrDefault();
                                    if (currstep != null)
                                        if (!currstep.STEP_ARTWORK_CODE.StartsWith("SEND_CUS"))
                                            ViewBag.ReadOnly = "0";
                                }
                            }
                            else if (listCusDummy.Count > 0)
                            {
                                ViewBag.ReadOnly = "1";
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
                            else
                            {
                                if (UserID != process.CURRENT_USER_ID)
                                {
                                    if (CNService.IsPG(UserID2, context))
                                    {
                                        ViewBag.ReadOnly = "1";
                                    }
                                    else
                                    {
                                        ViewBag.HasAuth = "0";
                                        ViewBag.ReadOnly = "1";
                                    }
                                }
                            }
                        }
                        else if (process.CURRENT_ROLE_ID != null)
                        {
                            if (!CNService.IsPG(UserID2, context))
                            {
                                ViewBag.HasAuth = "0";
                            }
                            var listRole = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { USER_ID = UserID2 }, context);
                            if (listRole.Where(m => m.ROLE_ID == process.CURRENT_ROLE_ID).Count() == 0)
                            {
                                ViewBag.ReadOnly = "1";
                            }
                            else
                            {
                                var allStepMockup = ART_M_STEP_MOCKUP_SERVICE.GetAll(context);
                                var listUserCompany = ART_M_USER_COMPANY_SERVICE.GetByItem(new ART_M_USER_COMPANY() { USER_ID = Convert.ToInt32(UserID2) }, context);
                                var listUserTypeofProduct = ART_M_USER_TYPE_OF_PRODUCT_SERVICE.GetByItem(new ART_M_USER_TYPE_OF_PRODUCT() { USER_ID = Convert.ToInt32(UserID2) }, context);

                                var valid = CNService.CheckTypeOfProductAndCompanyMockup(UserID2, checkListId, Convert.ToInt32(id), context, allStepMockup, listUserCompany, listUserTypeofProduct);
                                if (!valid)
                                {
                                    ViewBag.ReadOnly = "1";
                                }
                            }
                        }
                    }

                    int mockupId = process.MOCKUP_ID;
                    var mockupNo = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(mockupId, context).MOCKUP_NO;
                    if (!string.IsNullOrEmpty(mockupNo))
                    {
                        ViewBag.Title = mockupNo;
                    }
                    else
                    {
                        ViewBag.Title = "Draft";
                    }

                    ViewBag.DefaultResonId = ART_M_DECISION_REASON_SERVICE.GetByItem(new ART_M_DECISION_REASON() { IS_DEFAULT = "X" }, context)[0].ART_M_DECISION_REASON_ID;
                    ViewBag.DefaultResonTxt = ART_M_DECISION_REASON_SERVICE.GetByItem(new ART_M_DECISION_REASON() { IS_DEFAULT = "X" }, context)[0].DESCRIPTION;
                    ViewBag.MockupSubId = id == null ? 0 : id;
                    ViewBag.MockupId = mockupId;
                    ViewBag.CheckListId = checkListId;
                    ViewBag.CURRENT_STEP_CODE_DISPLAY_TXT = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_ID = Convert.ToInt32(process.CURRENT_STEP_ID) }, context).FirstOrDefault().STEP_MOCKUP_CODE;
                    ViewBag.CURRENT_STEP_CODE_FOR = "Internal";
                    if (ViewBag.CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_QUO") ViewBag.CURRENT_STEP_CODE_FOR = "VendorQuo";
                    if (ViewBag.CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_MB") ViewBag.CURRENT_STEP_CODE_FOR = "Vendor";
                    if (ViewBag.CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_DL") ViewBag.CURRENT_STEP_CODE_FOR = "Vendor";
                    if (ViewBag.CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_RS") ViewBag.CURRENT_STEP_CODE_FOR = "Vendor";
                    if (ViewBag.CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_VN_PR") ViewBag.CURRENT_STEP_CODE_FOR = "Vendor";
                    if (ViewBag.CURRENT_STEP_CODE_DISPLAY_TXT == "SEND_CUS_APP") ViewBag.CURRENT_STEP_CODE_FOR = "Customer";

                    var MainMockupSubId = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(id, context).PARENT_MOCKUP_SUB_ID;
                    ViewBag.MainMockupSubId = id;
                    if (MainMockupSubId > 0)
                    {
                        ViewBag.MainMockupSubId = MainMockupSubId;
                    }

                    ViewBag.CanRequestQuo = "1";
                    ViewBag.WaitingPGSUPSelectQuo = "0";
                    var SEND_PG_SUP_SEL_VENDOR = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG_SUP_SEL_VENDOR" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                    var checkSendQuo = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { CURRENT_STEP_ID = SEND_PG_SUP_SEL_VENDOR, MOCKUP_ID = mockupId }, context).ToList();
                    if (checkSendQuo.Where(m => string.IsNullOrEmpty(m.IS_END)).ToList().Count > 0)
                    {
                        ViewBag.WaitingPGSUPSelectQuo = "1";
                    }

                    ViewBag.OverDue = MockUpProcessHelper.CheckOverDue(process, context);
                    //ViewBag.OverDue = "0";
                    //var duration = ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(process.CURRENT_STEP_ID, context).DURATION;
                    //DateTime dtReceiveWf = process.CREATE_DATE;
                    //DateTime dtWillFinish = CNService.AddBusinessDays(dtReceiveWf, (int)Math.Ceiling(duration.Value));
                    //if (DateTime.Now.Date > dtWillFinish.Date)
                    //{
                    //    ViewBag.OverDue = "1";
                    //}

                    ViewBag.CreateByFFC = "0";
                    if (CNService.IsFFC(ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(checkListId, context).CREATE_BY, context))
                    {
                        ViewBag.CreateByFFC = "1";
                    }

                    ViewBag.WFCompletedOrTerminated = "0";
                    if (process.IS_END == "X" || process.IS_TERMINATE == "X" || !string.IsNullOrEmpty(process.REMARK_KILLPROCESS))
                    {
                        ViewBag.WFCompletedOrTerminated = "1";
                    }

                    ViewBag.ShowExtendStepDuration = "0";
                    if (ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(process.CURRENT_STEP_ID, context).DURATION_EXTEND > 0 && process.CURRENT_USER_ID != null)
                    {
                        if ((!string.IsNullOrEmpty(process.IS_STEP_DURATION_EXTEND) && ViewBag.ReadOnly == "1") || (!string.IsNullOrEmpty(process.IS_STEP_DURATION_EXTEND) && ViewBag.ReadOnly == "0") || (string.IsNullOrEmpty(process.IS_STEP_DURATION_EXTEND) && ViewBag.ReadOnly == "0"))
                        {
                            ViewBag.ShowExtendStepDuration = "1";
                        }
                    }

                    return View();
                }
            }
        }
    }
}