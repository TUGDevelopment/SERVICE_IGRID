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
    public class PermissionSetting
    {
        public string CODE { get; set; }
        public string DESCRIPTION { get; set; }
    }
    public partial class PermissionModel
    {
        public List<ART_M_ROLE> ObjRole { get; set; }
        public List<PermissionSetting> ObjWFFunction { get; set; }
        public List<PermissionSetting> ObjReport { get; set; }
        public List<PermissionSetting> ObjMasterData { get; set; }
    }
    public class PermissionController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    if (!BLL.Services.CNService.HasPermission(BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context), "PERMISSION", context))
                        return View("NoAuth");

                    PermissionModel model = new PermissionModel();

                    var employee_ = ART_M_USER_SERVICE.GetByItem(new ART_M_USER() { USER_ID = BLL.Services.CNService.GetUserID2(User.Identity.GetUserName(), context) }, context).FirstOrDefault();
                    int pos = Convert.ToInt32(employee_.POSITION_ID);
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

                    model.ObjWFFunction = new List<PermissionSetting>();
                    model.ObjReport = new List<PermissionSetting>();
                    model.ObjMasterData = new List<PermissionSetting>();

                    model.ObjWFFunction.Add(new PermissionSetting() { CODE = "DELEGATE", DESCRIPTION = "Delegate" });
                    model.ObjWFFunction.Add(new PermissionSetting() { CODE = "REASSIGN", DESCRIPTION = "Re-Assign" });
                    model.ObjWFFunction.Add(new PermissionSetting() { CODE = "REOPEN", DESCRIPTION = "Re-Open" });
                    model.ObjWFFunction.Add(new PermissionSetting() { CODE = "RECALL", DESCRIPTION = "Re-Call" });
                    model.ObjWFFunction.Add(new PermissionSetting() { CODE = "CHANGEOWNER", DESCRIPTION = "Change owner" });

                    model.ObjReport.Add(new PermissionSetting() { CODE = "TRACKING", DESCRIPTION = "Tracking report" });
                    model.ObjReport.Add(new PermissionSetting() { CODE = "ENDTOEND", DESCRIPTION = "End to end report" });
                    model.ObjReport.Add(new PermissionSetting() { CODE = "SUMMARY", DESCRIPTION = "Summary report" });
                    model.ObjReport.Add(new PermissionSetting() { CODE = "KPI", DESCRIPTION = "KPI report" });
                    model.ObjReport.Add(new PermissionSetting() { CODE = "WAREHOUSE", DESCRIPTION = "Warehouse report" });
                    model.ObjReport.Add(new PermissionSetting() { CODE = "COLLABORATION", DESCRIPTION = "Customer vendor collaboration report" });
                    model.ObjReport.Add(new PermissionSetting() { CODE = "OUTSTANDING", DESCRIPTION = "Outstanding report" });
                    model.ObjReport.Add(new PermissionSetting() { CODE = "MATCONTROL", DESCRIPTION = "Lists status of packaging material report" });

                    model.ObjMasterData.Add(new PermissionSetting() { CODE = "USERROLE", DESCRIPTION = "User and role setting" });
                    model.ObjMasterData.Add(new PermissionSetting() { CODE = "PERMISSION", DESCRIPTION = "Permission setting" });
                    model.ObjMasterData.Add(new PermissionSetting() { CODE = "EMAILTEMPLATE", DESCRIPTION = "Email template and subject" });
                    model.ObjMasterData.Add(new PermissionSetting() { CODE = "STEP", DESCRIPTION = "Step duration setting" });
                    model.ObjMasterData.Add(new PermissionSetting() { CODE = "PIC", DESCRIPTION = "Account allocation setting" });
                    model.ObjMasterData.Add(new PermissionSetting() { CODE = "CUSTOMERMASTER", DESCRIPTION = "Customer master and shade limit approval setting" });
                    model.ObjMasterData.Add(new PermissionSetting() { CODE = "VENDORMASTER", DESCRIPTION = "Vendor master and list of vendor by 2nd packaging type" });
                    model.ObjMasterData.Add(new PermissionSetting() { CODE = "WORDINGTEMPLATE", DESCRIPTION = "Copy template" });
                    model.ObjMasterData.Add(new PermissionSetting() { CODE = "ATTACHMENT", DESCRIPTION = "Attachment files" });
                    model.ObjMasterData.Add(new PermissionSetting() { CODE = "DECISION", DESCRIPTION = "Decision Code setting" });
                    model.ObjMasterData.Add(new PermissionSetting() { CODE = "ZONE", DESCRIPTION = "Country and zone setting" });
                    model.ObjMasterData.Add(new PermissionSetting() { CODE = "REVEIVEEMAILNEWCUSTOMER", DESCRIPTION = "Reveive email new customer" });
                    model.ObjMasterData.Add(new PermissionSetting() { CODE = "CUSTOMERINFO", DESCRIPTION = "View customer user infomation" });
                    model.ObjMasterData.Add(new PermissionSetting() { CODE = "VENDORINFO", DESCRIPTION = "View vendor user infomation" });

                    return View(model);
                }
            }
        }
    }
}
