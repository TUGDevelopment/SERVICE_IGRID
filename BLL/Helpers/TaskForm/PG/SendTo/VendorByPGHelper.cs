using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class VendorByPGHelper
    {
        public static XECM_M_VENDOR_RESULT GetVendorMaster(XECM_M_VENDOR_REQUEST param)
        {
            XECM_M_VENDOR_RESULT Results = new XECM_M_VENDOR_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.SAP_M_VENDOR(XECM_M_VENDOR_SERVICE.GetAll(context));
                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_VENDOR(XECM_M_VENDOR_SERVICE.GetByItem(MapperServices.SAP_M_VENDOR(param.data), context));
                        }

                        Results.status = "S";

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].VENDOR_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].VENDOR_CODE + ":" + Results.data[i].VENDOR_NAME; // + " : " + Results.data[i].DESCRIPTION;
                            }

                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                Results.data = (from u1 in Results.data
                                                where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                                select u1).ToList();
                            }

                            Results.data = Results.data.OrderBy(x => x.VENDOR_NAME).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_M_USER_VENDOR_RESULT GetVendorUser(ART_M_USER_VENDOR_REQUEST param)
        {
            ART_M_USER_VENDOR_RESULT Results = new ART_M_USER_VENDOR_RESULT();
            List<ART_M_USER_VENDOR> listUserVendor = new List<ART_M_USER_VENDOR>();
            ART_M_USER_VENDOR userVendor = new ART_M_USER_VENDOR();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            listUserVendor = ART_M_USER_VENDOR_SERVICE.GetAll(context);
                        }
                        else
                        {
                            int vendorID = 0;
                            vendorID = param.data.VENDOR_ID;

                            userVendor = new ART_M_USER_VENDOR();
                            userVendor.VENDOR_ID = vendorID;
                            //userVendor.IS_EMAIL_TO = "X";
                            listUserVendor = ART_M_USER_VENDOR_SERVICE.GetByItem(userVendor, context);
                        }

                        Results.data = new List<ART_M_USER_VENDOR_2>();
                        foreach (ART_M_USER_VENDOR item in listUserVendor)
                        {
                            ART_M_USER_VENDOR_2 item2 = new ART_M_USER_VENDOR_2();
                            var user = ART_M_USER_SERVICE.GetByUSER_ID(item.USER_ID, context);
                            if (user != null)
                            {
                                item2 = MapperServices.ART_M_USER_VENDOR(item);
                                item2.VENDOR_DISPLAY_TXT = CNService.GetVendorName(item.VENDOR_ID, context);
                                item2.USER_DISPLAY_TXT = CNService.GetUserName(item.USER_ID, context);
                                item2.EMAIL = user.EMAIL;
                                Results.data.Add(item2);
                            }
                        }
                    }
                }
                Results.data = Results.data.OrderBy(x => x.VENDOR_DISPLAY_TXT).ToList();
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_RESULT GetVendorByPG(ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_RESULT Results = new ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG(ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_SERVICE.GetAll(context));
                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG(ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_SERVICE.GetByItem(MapperServices.ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG(param.data), context));
                        }

                     
                        Results.status = "S";

                        if (Results.data.Count > 0)
                        {
                            //appended by aof #INC-11265
                            foreach (ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_2 obj in Results.data)
                            {
                                var process_vendor = context.ART_WF_MOCKUP_PROCESS_VENDOR.Where(w => w.MOCKUP_SUB_ID == obj.MOCKUP_SUB_ID).FirstOrDefault();
                                if (process_vendor != null)
                                {
                                    obj.PROCESS_VENDOR = MapperServices.ART_WF_MOCKUP_PROCESS_VENDOR(process_vendor);
                                    obj.PROCESS_VENDOR.REASON_BY_OTHER = CNService.getReason(obj.PROCESS_VENDOR.REASON_ID, context);
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_RESULT SaveVendorByPG(ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_REQUEST_LIST param)
        {
            ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_RESULT Results = new ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_RESULT();

            int vendorID = 0;
            int currentUserIDByVendor = 0;

            if (param.data.Count > 0)
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        ART_WF_MOCKUP_PROCESS_RESULT allProcessResults = new ART_WF_MOCKUP_PROCESS_RESULT();
                        allProcessResults.data = new List<ART_WF_MOCKUP_PROCESS_2>();
                        ART_WF_MOCKUP_PROCESS_RESULT processResults = new ART_WF_MOCKUP_PROCESS_RESULT();
                        using (var dbContextTransaction = CNService.IsolationLevel(context))
                        {
                            foreach (ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_2 iVendorByPG in param.data)
                            {
                               
                                string msg = MockUpProcessHelper.checkDupWF(iVendorByPG.PROCESS, context);
                                if (msg != "")
                                {
                                    Results.status = "E";
                                    Results.msg = msg;
                                    return Results;
                                }

                                vendorID = CheckSelectedVendor(iVendorByPG.MOCKUP_ID, context, ref currentUserIDByVendor);
                                if (vendorID == 0)
                                {
                                    Results.status = "E";
                                    Results.msg = MessageHelper.GetMessage("MSG_002",context);
                                    return Results;
                                }

                                if (iVendorByPG.PROCESS != null)
                                {
                                    processResults = SaveProcessVendorByPG(iVendorByPG.PROCESS, vendorID, currentUserIDByVendor, context);
                                    allProcessResults.data.AddRange(processResults.data);
                                }

                                foreach (var item2 in processResults.data)
                                {
                                    ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG vendorData = new ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG();
                                    vendorData = MapperServices.ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG(iVendorByPG);
                                    vendorData.MOCKUP_SUB_ID = item2.MOCKUP_SUB_ID;
                                    ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_SERVICE.SaveOrUpdate(vendorData, context);

                                    //saveMockupProcessRemarkReason by aof #INC-11265
                                    saveMockupProcessRemarkReason_SendPG(item2.MOCKUP_SUB_ID, item2.REASON_ID.GetValueOrDefault(0), iVendorByPG.REMARK_REASON, iVendorByPG.CREATE_BY, iVendorByPG.UPDATE_BY, context);
                                }
                            }

                            dbContextTransaction.Commit();

                            foreach (var process2 in allProcessResults.data)
                            {
                                EmailService.sendEmailMockup(process2.MOCKUP_ID, process2.MOCKUP_SUB_ID, "WF_SEND_TO_VENDOR", context);
                            }

                            Results.status = "S";
                            Results.msg = MessageHelper.GetMessage("MSG_001",context);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Results.status = "E";
                    Results.msg = CNService.GetErrorMessage(ex);
                }
            }

            return Results;
        }

        //saveMockupProcessRemarkReason by aof #INC-11265
        private static void saveMockupProcessRemarkReason_SendPG(int MOCKUP_SUB_ID,int REASON_ID,string REMARK_REASON,int CREATE_BY,int UPDATE_BY, ARTWORKEntities context) {

            var REASON_DESC = CNService.getReason(REASON_ID, context);
            if (REASON_DESC == "อื่นๆ โปรดระบุ (Others)")
            {

                var WF_STEP = context.ART_M_STEP_MOCKUP.Where(w => w.STEP_MOCKUP_CODE == "SEND_PG").FirstOrDefault().STEP_MOCKUP_ID;

                ART_WF_REMARK_REASON_OTHER model = new ART_WF_REMARK_REASON_OTHER();
                model.WF_TYPE = "M";
                model.WF_STEP = WF_STEP;
                model.WF_SUB_ID = MOCKUP_SUB_ID;


                var check = ART_WF_REMARK_REASON_OTHER_SERVICE.GetByItem(new ART_WF_REMARK_REASON_OTHER() { WF_SUB_ID = model.WF_SUB_ID, WF_STEP = model.WF_STEP }, context);
                if (check.Count > 0)
                {
                    model.ART_WF_REMARK_REASON_OTHER_ID = check.FirstOrDefault().ART_WF_REMARK_REASON_OTHER_ID;
                }

                model.REMARK_REASON = REMARK_REASON;
                model.CREATE_BY = CREATE_BY;
                model.UPDATE_BY = UPDATE_BY;

                if (model.WF_SUB_ID != null)
                {
                    ART_WF_REMARK_REASON_OTHER_SERVICE.SaveOrUpdate(model, context);
                }
            }
        }

        private static int CheckSelectedVendor(int mockupID, ARTWORKEntities context, ref int currentUserIDByVendor)
        {
            int vendor_id = 0;

            var vendorMockup = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE() { MOCKUP_ID = mockupID, SELECTED = "X" }, context).FirstOrDefault();

            if (vendorMockup != null)
            {
                vendor_id = Convert.ToInt32(vendorMockup.VENDOR_ID);
                currentUserIDByVendor = Convert.ToInt32(vendorMockup.USER_ID);
            }

            if (vendor_id == 0)
            {
                var temp = ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR() { MOCKUP_ID = mockupID }, context).OrderByDescending(m => m.CREATE_DATE).FirstOrDefault();
                if (temp != null)
                {
                    vendor_id = temp.VENDOR_ID;
                    currentUserIDByVendor = temp.USER_ID;
                }
            }

            return vendor_id;
        }

        private static ART_WF_MOCKUP_PROCESS_RESULT SaveProcessVendorByPG(ART_WF_MOCKUP_PROCESS_2 process, int vendorID, int currentUserIDByVendor, ARTWORKEntities context)
        {
            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();

            var list = ART_M_USER_VENDOR_SERVICE.GetByItem(new ART_M_USER_VENDOR() { VENDOR_ID = vendorID }, context);
            list = list.OrderByDescending(m => m.IS_EMAIL_TO).ToList();

            List<ART_WF_MOCKUP_PROCESS_2> listProcess = new List<ART_WF_MOCKUP_PROCESS_2>();

            foreach (var item in list)
            {
                ART_WF_MOCKUP_PROCESS tempProcess = new ART_WF_MOCKUP_PROCESS();
                tempProcess = MapperServices.ART_WF_MOCKUP_PROCESS(process);
                tempProcess.CURRENT_VENDOR_ID = vendorID;
                tempProcess.CURRENT_USER_ID = item.USER_ID;
                CNService.CheckDelegateBeforeRounting(tempProcess, context);
                listProcess.Add(MapperServices.ART_WF_MOCKUP_PROCESS(tempProcess));
            }

            Results.data = listProcess;
            return Results;
        }

        public static ART_M_USER_VENDOR_RESULT GetVendorUserQuo(ART_M_USER_VENDOR_REQUEST param)
        {
            ART_M_USER_VENDOR_RESULT Results = new ART_M_USER_VENDOR_RESULT();
            List<ART_M_USER_VENDOR> listUserVendor = new List<ART_M_USER_VENDOR>();
            ART_M_USER_VENDOR userVendor = new ART_M_USER_VENDOR();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            listUserVendor = ART_M_USER_VENDOR_SERVICE.GetAll(context);
                        }
                        else
                        {
                            int vendorID = 0;
                            vendorID = param.data.VENDOR_ID;

                            userVendor = new ART_M_USER_VENDOR();
                            userVendor.VENDOR_ID = vendorID;
                            listUserVendor = ART_M_USER_VENDOR_SERVICE.GetByItem(userVendor, context);

                            var userIdActionQuo = 0;
                            var SEND_VN_QUO = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_VN_QUO" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                            var otherProcessVendorQuo = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID, CURRENT_STEP_ID = SEND_VN_QUO }, context);
                            otherProcessVendorQuo = otherProcessVendorQuo.Where(m => m.UPDATE_BY != -1).ToList();
                            foreach (var item in otherProcessVendorQuo)
                            {
                                if (item.CURRENT_USER_ID > 0)
                                    userIdActionQuo = Convert.ToInt32(item.CURRENT_USER_ID);
                            }

                            if (userIdActionQuo > 0)
                                listUserVendor = listUserVendor.Where(m => m.USER_ID == userIdActionQuo).ToList();

                            if (listUserVendor.Count == 0)
                                listUserVendor = ART_M_USER_VENDOR_SERVICE.GetByItem(userVendor, context);
                        }

                        Results.data = new List<ART_M_USER_VENDOR_2>();
                        foreach (ART_M_USER_VENDOR item in listUserVendor)
                        {
                            ART_M_USER_VENDOR_2 item2 = new ART_M_USER_VENDOR_2();
                            var user = ART_M_USER_SERVICE.GetByUSER_ID(item.USER_ID, context);
                            if (user != null)
                            {
                                item2 = MapperServices.ART_M_USER_VENDOR(item);
                                item2.VENDOR_DISPLAY_TXT = CNService.GetVendorName(item.VENDOR_ID, context);
                                item2.USER_DISPLAY_TXT = CNService.GetUserName(item.USER_ID, context);
                                item2.EMAIL = user.EMAIL;
                                Results.data.Add(item2);
                            }
                        }
                    }
                }
                Results.data = Results.data.OrderBy(x => x.VENDOR_DISPLAY_TXT).ToList();
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static XECM_M_VENDOR_RESULT GetVendorHasUser(XECM_M_VENDOR_REQUEST param)
        {
            XECM_M_VENDOR_RESULT Results = new XECM_M_VENDOR_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            param = new XECM_M_VENDOR_REQUEST();
                            param.data = new XECM_M_VENDOR_2();
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_VENDOR(XECM_M_VENDOR_SERVICE.GetAll(context));
                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_VENDOR(XECM_M_VENDOR_SERVICE.GetByItem(MapperServices.SAP_M_VENDOR(param.data), context));
                        }

                        Results.status = "S";

                        if (Results.data.Count > 0)
                        {
                            var vendorId = ART_M_USER_VENDOR_SERVICE.GetAll(context).Select(m => m.VENDOR_ID);
                            var hasUser = Results.data.Where(m => vendorId.Contains(m.VENDOR_ID)).ToList();

                            for (int i = 0; i < hasUser.Count; i++)
                            {
                                Results.data[i].ID = hasUser[i].VENDOR_ID;
                                Results.data[i].DISPLAY_TXT = hasUser[i].VENDOR_CODE + ":" + hasUser[i].VENDOR_NAME;
                            }

                            var noUser = Results.data.Where(m => !vendorId.Contains(m.VENDOR_ID)).ToList();
                            for (int i = 0; i < noUser.Count; i++)
                            {
                                Results.data[i].disabled = true;
                                Results.data[i].ID = noUser[i].VENDOR_ID;
                                Results.data[i].DISPLAY_TXT = noUser[i].VENDOR_CODE + ":" + noUser[i].VENDOR_NAME + " (No user)";
                            }

                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                Results.data = (from u1 in Results.data
                                                where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                                select u1).ToList();
                            }

                            Results.data = Results.data.OrderBy(x => x.VENDOR_NAME).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static XECM_M_VENDOR_RESULT GetVendorHasUserByMatGroup(XECM_M_VENDOR_REQUEST param)
        {
            XECM_M_VENDOR_RESULT Results = new XECM_M_VENDOR_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            param = new XECM_M_VENDOR_REQUEST();
                            param.data = new XECM_M_VENDOR_2();
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_VENDOR(XECM_M_VENDOR_SERVICE.GetAll(context));
                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_VENDOR(XECM_M_VENDOR_SERVICE.GetByItem(MapperServices.SAP_M_VENDOR(param.data), context));
                        }

                        Results.status = "S";

                        if (Results.data.Count > 0)
                        {
                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                Results.data = (from u1 in Results.data
                                                where (u1.VENDOR_CODE.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                      || u1.VENDOR_NAME.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                                select u1).ToList();
                            }
                            List<int> vendorMatgroup = new List<int>();
                            if (param.data.MATGROUP_ID != 0)
                            {
                                vendorMatgroup = ART_M_VENDOR_MATGROUP_SERVICE.GetByItem(new ART_M_VENDOR_MATGROUP() { MATGROUP_ID = param.data.MATGROUP_ID }, context).Select(m => m.VENDOR_ID).ToList();
                            }
                            else
                            {
                                vendorMatgroup = ART_M_VENDOR_MATGROUP_SERVICE.GetAll(context).Select(m => m.VENDOR_ID).ToList();
                            }
                            Results.data = Results.data.Where(m => vendorMatgroup.Contains(m.VENDOR_ID)).ToList();

                            var vendorId = ART_M_USER_VENDOR_SERVICE.GetAll(context).Select(m => m.VENDOR_ID);
                            var hasUser = Results.data.Where(m => vendorId.Contains(m.VENDOR_ID)).ToList();

                            for (int i = 0; i < hasUser.Count; i++)
                            {
                                Results.data[i].ID = hasUser[i].VENDOR_ID;
                                Results.data[i].DISPLAY_TXT = hasUser[i].VENDOR_CODE + ":" + hasUser[i].VENDOR_NAME;
                            }

                            var noUser = Results.data.Where(m => !vendorId.Contains(m.VENDOR_ID)).ToList();
                            for (int i = 0; i < noUser.Count; i++)
                            {
                                Results.data[i].disabled = true;
                                Results.data[i].ID = noUser[i].VENDOR_ID;
                                Results.data[i].DISPLAY_TXT = noUser[i].VENDOR_CODE + ":" + noUser[i].VENDOR_NAME + " (No user)";
                            }

                            Results.data = Results.data.OrderBy(x => x.VENDOR_NAME).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

    }

}
