using BLL.Services;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace BLL.Helpers
{
    public class HistoryHelper
    {
        public static ART_WF_MOCKUP_PROCESS_RESULT GetTaskFormHistory(ART_WF_MOCKUP_PROCESS_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT Results = new ART_WF_MOCKUP_PROCESS_RESULT();

            try
            {
                if (param == null || param.data == null)
                {

                }
                else
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            param.data.MOCKUP_ID = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_SUB_ID = param.data.MOCKUP_SUB_ID }, context).FirstOrDefault().MOCKUP_ID;
                            var checkListId = CNService.ConvertMockupIdToCheckListId(param.data.MOCKUP_ID, context);

                            var temp = MapperServices.ART_WF_MOCKUP_PROCESS(param.data);
                            var list = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID }, context);
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS(list);

                            var allStep = ART_M_STEP_MOCKUP_SERVICE.GetAll(context);
                            var SEND_VN_MB = allStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_VN_MB").FirstOrDefault().STEP_MOCKUP_ID;
                            var SEND_VN_PR = allStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_VN_PR").FirstOrDefault().STEP_MOCKUP_ID;
                            var SEND_VN_DL = allStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_VN_DL").FirstOrDefault().STEP_MOCKUP_ID;
                            var SEND_VN_RS = allStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_VN_RS").FirstOrDefault().STEP_MOCKUP_ID;
                            var SEND_RD_PRI_PKG = allStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_RD_PRI_PKG").FirstOrDefault().STEP_MOCKUP_ID;
                            var SEND_PN_PRI_PKG = allStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_PN_PRI_PKG").FirstOrDefault().STEP_MOCKUP_ID;
                            var SEND_VN_QUO = allStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_VN_QUO").FirstOrDefault().STEP_MOCKUP_ID;
                            var SEND_PG_SUP_SEL_VENDOR = allStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_PG_SUP_SEL_VENDOR").FirstOrDefault().STEP_MOCKUP_ID;
                            var SEND_WH_TEST_PACK = allStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_WH_TEST_PACK").FirstOrDefault().STEP_MOCKUP_ID;
                            var SEND_APP_MATCH_BOARD = allStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_APP_MATCH_BOARD").FirstOrDefault().STEP_MOCKUP_ID;
                            var SEND_CUS_APP = allStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_CUS_APP").FirstOrDefault().STEP_MOCKUP_ID;
                            var SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN = allStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN").FirstOrDefault().STEP_MOCKUP_ID;
                            var SEND_BACK_MK = allStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_BACK_MK").FirstOrDefault().STEP_MOCKUP_ID;
                            var SEND_PG = allStep.Where(m => m.STEP_MOCKUP_CODE == "SEND_PG").FirstOrDefault().STEP_MOCKUP_ID;

                            foreach (ART_WF_MOCKUP_PROCESS_2 item in Results.data)
                            {
                                var temp_ = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_ITEM() { MOCKUP_ID = item.MOCKUP_ID }, context).FirstOrDefault();
                                var CG_OWNER = ART_WF_LOG_CHANGE_OWNER_SERVICE.GetByItem(new ART_WF_LOG_CHANGE_OWNER() { WF_ID = temp_.CHECK_LIST_ID, WF_TYPE = "M", IS_ACTIVE = "X" }, context).OrderByDescending(q => q.UPDATE_DATE).FirstOrDefault();
                                if (CG_OWNER != null)
                                {
                                    item.CG_OWNER_REASON = CG_OWNER.REMARK;
                                    if (CG_OWNER.FROM_USER_ID > 0) item.OLD_OWNER_CG_OWNER = CNService.GetUserName(CG_OWNER.FROM_USER_ID, context);
                                    else item.OLD_OWNER_CG_OWNER = "-";
                                    if (CG_OWNER.TO_USER_ID > 0) item.NEW_OWNER_CG_OWNER = CNService.GetUserName(CG_OWNER.TO_USER_ID, context);
                                    else item.NEW_OWNER_CG_OWNER = "-";
                                    if (CG_OWNER.CREATE_BY > 0) item.CG_OWNER_BY = CNService.GetUserName(CG_OWNER.CREATE_BY, context);
                                    else item.CG_OWNER_BY = "-";
                                    if (item.CURRENT_USER_ID == CG_OWNER.TO_USER_ID) item.IS_CG_OWNER = "X";
                                }


                                var REASSIGN = ART_WF_LOG_REASSIGN_SERVICE.GetByItem(new ART_WF_LOG_REASSIGN() { WF_SUB_ID = item.MOCKUP_SUB_ID, WF_TYPE = "M" }, context).OrderByDescending(q => q.UPDATE_DATE).FirstOrDefault();
                                if (REASSIGN != null)
                                {
                                    item.REASSIGNREASON = REASSIGN.REMARK;
                                    if (REASSIGN.FROM_USER_ID > 0) item.OLD_OWNER_REASSIGN = CNService.GetUserName(REASSIGN.FROM_USER_ID, context);
                                    else item.OLD_OWNER_REASSIGN = "-";
                                    if (REASSIGN.TO_USER_ID > 0) item.NEW_OWNER_REASSIGN = CNService.GetUserName(REASSIGN.TO_USER_ID, context);
                                    else item.NEW_OWNER_REASSIGN = "-";
                                    if (REASSIGN.REASSIGN_BY > 0) item.REASSIGNBY = CNService.GetUserName(REASSIGN.REASSIGN_BY, context);
                                    else item.REASSIGNBY = "-";
                                    item.IS_REASSIGN = "X";
                                }

                                var REOPEN = ART_WF_LOG_REOPEN_SERVICE.GetByItem(new ART_WF_LOG_REOPEN() { WF_SUB_ID = item.MOCKUP_SUB_ID, WF_TYPE = "M" }, context).OrderByDescending(q => q.UPDATE_DATE).FirstOrDefault();
                                if (REOPEN != null)
                                {
                                    item.REOPENREASON = REOPEN.REMARK;
                                    if (REOPEN.REOPEN_BY > 0) item.REOPENBY = CNService.GetUserName(REOPEN.REOPEN_BY, context);
                                    else item.REOPENBY = "-";
                                    item.IS_REOPEN = "X";
                                }

                                var DELEGATE = ART_WF_LOG_DELEGATE_SERVICE.GetByItem(new ART_WF_LOG_DELEGATE() { WF_SUB_ID = item.MOCKUP_SUB_ID, WF_TYPE = "M" }, context).OrderByDescending(q => q.UPDATE_DATE).FirstOrDefault();
                                if (DELEGATE != null)
                                {
                                    item.DELEGATEREASON = DELEGATE.REMARK;
                                    if (DELEGATE.FROM_USER_ID > 0) item.OLD_OWNER_DELEGATE = CNService.GetUserName(DELEGATE.FROM_USER_ID, context);
                                    else item.OLD_OWNER_DELEGATE = "-";
                                    if (DELEGATE.TO_USER_ID > 0) item.NEW_OWNER_DELEGATE = CNService.GetUserName(DELEGATE.TO_USER_ID, context);
                                    else item.NEW_OWNER_DELEGATE = "-";
                                    if (DELEGATE.DELEGATE_BY > 0) item.DELEGATEBY = CNService.GetUserName(DELEGATE.DELEGATE_BY, context);
                                    else item.DELEGATEBY = "-";
                                    item.IS_DELEGATE_ = "X";
                                }

                                if (item.CURRENT_STEP_ID == SEND_PG)
                                {
                                    if (item.IS_TERMINATE == "X")
                                        item.REMARK = "[Terminated] " + item.REMARK_TERMINATE;
                                    else
                                        item.REMARK = "";
                                }

                                if (item.CURRENT_STEP_ID == SEND_VN_MB || item.CURRENT_STEP_ID == SEND_VN_DL || item.CURRENT_STEP_ID == SEND_VN_RS || item.CURRENT_STEP_ID == SEND_VN_PR)
                                {
                                    var temp2 = ART_WF_MOCKUP_PROCESS_VENDOR_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_VENDOR() { MOCKUP_SUB_ID = item.MOCKUP_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }
                                if (item.CURRENT_STEP_ID == SEND_RD_PRI_PKG)
                                {
                                    var temp2 = ART_WF_MOCKUP_PROCESS_RD_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_RD() { MOCKUP_SUB_ID = item.MOCKUP_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }
                                if (item.CURRENT_STEP_ID == SEND_PN_PRI_PKG)
                                {
                                    var temp2 = ART_WF_MOCKUP_PROCESS_PLANNING_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PLANNING() { MOCKUP_SUB_ID = item.MOCKUP_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }
                                if (item.CURRENT_STEP_ID == SEND_PG_SUP_SEL_VENDOR)
                                {
                                    var temp2 = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_SUP_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_SUP() { MOCKUP_SUB_ID = item.MOCKUP_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENY_BY_PG_SUP;
                                }
                                if (item.CURRENT_STEP_ID == SEND_APP_MATCH_BOARD)
                                {
                                    var temp2 = ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD() { MOCKUP_SUB_ID = item.MOCKUP_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }
                                if (item.CURRENT_STEP_ID == SEND_CUS_APP)
                                {
                                    var temp2 = ART_WF_MOCKUP_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_CUSTOMER() { MOCKUP_SUB_ID = item.MOCKUP_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }
                                if (item.CURRENT_STEP_ID == SEND_WH_TEST_PACK)
                                {
                                    var temp2 = ART_WF_MOCKUP_PROCESS_WAREHOUSE_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_WAREHOUSE() { MOCKUP_SUB_ID = item.MOCKUP_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }
                                if (item.CURRENT_STEP_ID == SEND_VN_QUO)
                                {
                                    var temp2 = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR() { MOCKUP_SUB_ID = item.MOCKUP_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT_BY_VENDOR;
                                }
                                if (item.CURRENT_STEP_ID == SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN)
                                {
                                    var temp2 = ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR() { MOCKUP_SUB_ID = item.MOCKUP_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.REMARK;
                                }
                                if (item.CURRENT_STEP_ID == SEND_BACK_MK)
                                {
                                    var temp2 = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_SUB_ID = item.MOCKUP_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK = temp2.REMARK;
                                }
                                if (item.CURRENT_STEP_ID > 0) item.CURRENT_STEP_DISPLAY_TXT = ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(item.CURRENT_STEP_ID, context).STEP_MOCKUP_NAME;

                                if (item.CURRENT_STEP_ID == SEND_CUS_APP)
                                {
                                    var listCuscc = ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER() { CHECK_LIST_ID = checkListId, MAIL_CC = "X", CUSTOMER_USER_ID = Convert.ToInt32(item.CURRENT_USER_ID) }, context);
                                    if (listCuscc.Count > 0)
                                    {
                                        item.CURRENT_USER_DISPLAY_TXT = "[CC] " + CNService.GetUserName(item.CURRENT_USER_ID, context);
                                        item.CURRENT_USER_DISPLAY_TXT += "<br/>" + CNService.GetCustomerName(item.CURRENT_CUSTOMER_ID, context);
                                    }
                                    else
                                    {
                                        item.CURRENT_USER_DISPLAY_TXT = "[TO] " + CNService.GetUserName(item.CURRENT_USER_ID, context);
                                        item.CURRENT_USER_DISPLAY_TXT += "<br/>" + CNService.GetCustomerName(item.CURRENT_CUSTOMER_ID, context);
                                    }
                                }
                                else if (item.CURRENT_STEP_ID == SEND_VN_MB || item.CURRENT_STEP_ID == SEND_VN_DL || item.CURRENT_STEP_ID == SEND_VN_RS || item.CURRENT_STEP_ID == SEND_VN_PR)
                                {
                                    item.CURRENT_USER_DISPLAY_TXT = CNService.GetUserName(item.CURRENT_USER_ID, context);
                                    item.CURRENT_USER_DISPLAY_TXT += "<br/>" + CNService.GetVendorName(item.CURRENT_VENDOR_ID, context);
                                }
                                else
                                {
                                    item.CURRENT_USER_DISPLAY_TXT = CNService.GetUserName(item.CURRENT_USER_ID, context);
                                }

                                if (item.REMARK_KILLPROCESS != null)
                                {
                                    item.REMARK += "[Terminated] " + item.REMARK_KILLPROCESS;
                                }
                            }

                            var checkList = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(param.data.MOCKUP_ID, context);
                            var change_owner_check = ART_WF_LOG_CHANGE_OWNER_SERVICE.GetByItem(new ART_WF_LOG_CHANGE_OWNER { WF_ID = checkList.CHECK_LIST_ID, WF_TYPE = "M", FROM_USER_ID = checkList.CREATE_BY, IS_ACTIVE = "X" }, context).FirstOrDefault();

                            ART_WF_MOCKUP_PROCESS_2 tempCheckList = new ART_WF_MOCKUP_PROCESS_2();
                            tempCheckList.CURRENT_STEP_DISPLAY_TXT = "Opened";
                            if (change_owner_check != null)
                            {
                                tempCheckList.CURRENT_USER_DISPLAY_TXT = CNService.GetUserName(change_owner_check.CREATE_BY, context);
                                tempCheckList.IS_CG_OWNER = "X";
                            }
                            else
                                tempCheckList.CURRENT_USER_DISPLAY_TXT = CNService.GetUserName(checkList.CREATE_BY, context);
                            tempCheckList.CREATE_DATE = checkList.CREATE_DATE;
                            tempCheckList.REMARK = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(checkList.CHECK_LIST_ID, context).OTHER_REQUESTS;
                            tempCheckList.CHECK_LIST_ID = checkList.CHECK_LIST_ID;
                            tempCheckList.IS_END = "X";
                            Results.data.Add(tempCheckList);
                        }
                    }
                    Results.data = Results.data.OrderBy(m => m.CREATE_DATE).ToList();
                }

                Results.recordsFiltered = Results.data.ToList().Count;
                Results.recordsTotal = Results.data.ToList().Count;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_RESULT GetTaskFormArtworkHistory(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_RESULT Results = new ART_WF_ARTWORK_PROCESS_RESULT();

            try
            {
                if (param == null || param.data == null)
                {

                }
                else
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            param.data.ARTWORK_ITEM_ID = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context).FirstOrDefault().ARTWORK_ITEM_ID;
                            param.data.ARTWORK_REQUEST_ID = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context).FirstOrDefault().ARTWORK_REQUEST_ID;

                            var allStep = ART_M_STEP_ARTWORK_SERVICE.GetAll(context);
                            var SEND_MK = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_MK").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_PP = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_PP").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_PN = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_PN").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_WH = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_WH").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_QC = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_QC").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_RD = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_RD").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_PG = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_PG").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_PA = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_PA").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_MK_VERIFY = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_MK_VERIFY").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_GM_MK = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_GM_MK").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_VN_PM = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_VN_PM").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_VN_PO = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_VN_PO").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_VN_SL = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_VN_SL").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_CUS_REQ_REF = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_CUS_REQ_REF").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_CUS_REVIEW = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_CUS_REVIEW").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_QC_VERIFY = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_QC_VERIFY").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_GM_QC = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_GM_QC").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_CUS_PRINT = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_CUS_PRINT").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_CUS_SHADE = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_CUS_SHADE").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_CUS_REF = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_CUS_REF").FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_BACK_MK = allStep.Where(m => m.STEP_ARTWORK_CODE == "SEND_BACK_MK").FirstOrDefault().STEP_ARTWORK_ID;

                            var list = ((from p in context.ART_WF_ARTWORK_PROCESS
                                         where p.ARTWORK_ITEM_ID == param.data.ARTWORK_ITEM_ID
                                             && p.CURRENT_STEP_ID != SEND_PG
                                         select p).Union
                                        (from p in context.ART_WF_ARTWORK_PROCESS
                                         where p.ARTWORK_ITEM_ID == param.data.ARTWORK_ITEM_ID
                                             && p.CURRENT_STEP_ID == SEND_PG
                                             && p.CREATE_BY > 0
                                         select p)).ToList();

                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS(list);

                            foreach (ART_WF_ARTWORK_PROCESS_2 item in Results.data)
                            {
                                var CG_OWNER = ART_WF_LOG_CHANGE_OWNER_SERVICE.GetByItem(new ART_WF_LOG_CHANGE_OWNER() { WF_ID = item.ARTWORK_REQUEST_ID, WF_TYPE = "A", IS_ACTIVE = "X" }, context).OrderByDescending(q => q.UPDATE_DATE).FirstOrDefault();
                                if (CG_OWNER != null)
                                {
                                    item.CG_OWNER_REASON = CG_OWNER.REMARK;
                                    if (CG_OWNER.FROM_USER_ID > 0) item.OLD_OWNER_CG_OWNER = CNService.GetUserName(CG_OWNER.FROM_USER_ID, context);
                                    else item.OLD_OWNER_CG_OWNER = "-";
                                    if (CG_OWNER.TO_USER_ID > 0) item.NEW_OWNER_CG_OWNER = CNService.GetUserName(CG_OWNER.TO_USER_ID, context);
                                    else item.NEW_OWNER_CG_OWNER = "-";
                                    if (CG_OWNER.CREATE_BY > 0) item.CG_OWNER_BY = CNService.GetUserName(CG_OWNER.CREATE_BY, context);
                                    else item.CG_OWNER_BY = "-";
                                    if (item.CURRENT_USER_ID == CG_OWNER.TO_USER_ID) item.IS_CG_OWNER = "X";
                                }

                                var REASSIGN = ART_WF_LOG_REASSIGN_SERVICE.GetByItem(new ART_WF_LOG_REASSIGN() { WF_SUB_ID = item.ARTWORK_SUB_ID, WF_TYPE = "A" }, context).OrderByDescending(q => q.UPDATE_DATE).FirstOrDefault();
                                if (REASSIGN != null)
                                {
                                    item.REASSIGNREASON = REASSIGN.REMARK;
                                    if (REASSIGN.FROM_USER_ID > 0) item.OLD_OWNER_REASSIGN = CNService.GetUserName(REASSIGN.FROM_USER_ID, context);
                                    else item.OLD_OWNER_REASSIGN = "-";
                                    if (REASSIGN.TO_USER_ID > 0) item.NEW_OWNER_REASSIGN = CNService.GetUserName(REASSIGN.TO_USER_ID, context);
                                    else item.NEW_OWNER_REASSIGN = "-";
                                    if (REASSIGN.REASSIGN_BY > 0) item.REASSIGNBY = CNService.GetUserName(REASSIGN.REASSIGN_BY, context);
                                    else item.REASSIGNBY = "-";
                                    item.IS_REASSIGN = "X";
                                }

                                var REOPEN = ART_WF_LOG_REOPEN_SERVICE.GetByItem(new ART_WF_LOG_REOPEN() { WF_SUB_ID = item.ARTWORK_SUB_ID, WF_TYPE = "A" }, context).OrderByDescending(q => q.UPDATE_DATE).FirstOrDefault();
                                if (REOPEN != null)
                                {
                                    item.REOPENREASON = REOPEN.REMARK;
                                    if (REOPEN.REOPEN_BY > 0) item.REOPENBY = CNService.GetUserName(REOPEN.REOPEN_BY, context);
                                    else item.REOPENBY = "-";
                                    item.IS_REOPEN = "X";
                                }

                                var DELEGATE = ART_WF_LOG_DELEGATE_SERVICE.GetByItem(new ART_WF_LOG_DELEGATE() { WF_SUB_ID = item.ARTWORK_SUB_ID, WF_TYPE = "A" }, context).OrderByDescending(q => q.UPDATE_DATE).FirstOrDefault();
                                if (DELEGATE != null)
                                {
                                    item.DELEGATEREASON = DELEGATE.REMARK;
                                    if (DELEGATE.FROM_USER_ID > 0) item.OLD_OWNER_DELEGATE = CNService.GetUserName(DELEGATE.FROM_USER_ID, context);
                                    else item.OLD_OWNER_DELEGATE = "-";
                                    if (DELEGATE.TO_USER_ID > 0) item.NEW_OWNER_DELEGATE = CNService.GetUserName(DELEGATE.TO_USER_ID, context);
                                    else item.NEW_OWNER_DELEGATE = "-";
                                    if (DELEGATE.DELEGATE_BY > 0) item.DELEGATEBY = CNService.GetUserName(DELEGATE.DELEGATE_BY, context);
                                    else item.DELEGATEBY = "-";
                                    item.IS_DELEGATE_ = "X";
                                }

                                item.REMARK_OTHERS = "";
                                if (item.CURRENT_STEP_ID == SEND_PA)
                                {
                                    if (item.IS_TERMINATE == "X")
                                        item.REMARK = "[Terminated] " + item.REMARK_TERMINATE;
                                    else
                                        item.REMARK = "";
                                }
                                if (item.CURRENT_STEP_ID == SEND_PG)
                                {
                                    var temp2 = ART_WF_ARTWORK_PROCESS_PG_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PG() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }
                                if (item.CURRENT_STEP_ID == SEND_RD)
                                {
                                    var temp2 = ART_WF_ARTWORK_PROCESS_RD_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_RD() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }
                                if (item.CURRENT_STEP_ID == SEND_WH)
                                {
                                    var temp2 = ART_WF_ARTWORK_PROCESS_WH_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_WH() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }
                                if (item.CURRENT_STEP_ID == SEND_QC)
                                {
                                    var temp2 = ART_WF_ARTWORK_PROCESS_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_QC() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                  
                                    if (temp2 != null)
                                    {

                                        // ticket#437531 by aof
                                        //   item.REMARK_OTHERS = temp2.COMMENT;  
                                       item.REMARK_OTHERS = CNService.getReasonRemark(new ART_WF_REMARK_REASON_OTHER { WF_SUB_ID = temp2.ARTWORK_SUB_ID, WF_STEP = item.CURRENT_STEP_ID }, context);
                                        // ticket#437531 by aof

                                        // ticket#19383 by aof on 23/10/2021--start
                                        var reason = (from p in context.ART_M_DECISION_REASON
                                                      where p.ART_M_DECISION_REASON_ID == temp2.REASON_ID
                                                      select p).FirstOrDefault();
                                        if (reason != null)
                                        {

                                            if (reason.DESCRIPTION != "อื่นๆ โปรดระบุ (Others)")
                                            {
                                                item.REMARK_OTHERS = reason.DESCRIPTION + " " + item.REMARK_OTHERS;
                                            }

                                            
                                            item.REMARK_OTHERS = item.REMARK_OTHERS.Trim();
                                        }
                                        // ticket#19383 by aof on 23/10/2021--end
                                    }
                                }
                                if (item.CURRENT_STEP_ID == SEND_PN)
                                {
                                    var temp2 = ART_WF_ARTWORK_PROCESS_PLANNING_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PLANNING() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }
                                if (item.CURRENT_STEP_ID == SEND_PP)
                                {
                                    var temp2 = ART_WF_ARTWORK_PROCESS_PP_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PP() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }
                                if ((item.CURRENT_STEP_ID == SEND_VN_PM) || (item.CURRENT_STEP_ID == SEND_VN_SL))
                                {
                                    var temp2 = ART_WF_ARTWORK_PROCESS_VENDOR_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_VENDOR() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }

                                if (item.CURRENT_STEP_ID == SEND_VN_PO)
                                {
                                    var temp2 = ART_WF_ARTWORK_PROCESS_VENDOR_PO_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_VENDOR_PO() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }

                                if (item.CURRENT_STEP_ID == SEND_CUS_REQ_REF
                                    || item.CURRENT_STEP_ID == SEND_CUS_REVIEW
                                    || item.CURRENT_STEP_ID == SEND_CUS_PRINT
                                    || item.CURRENT_STEP_ID == SEND_CUS_SHADE
                                    || item.CURRENT_STEP_ID == SEND_CUS_REF
                                    )
                                {
                                    var temp2 = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }

                                if (item.CURRENT_STEP_ID == SEND_GM_QC)
                                {
                                    var temp2 = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }


                                if (item.CURRENT_STEP_ID == SEND_QC_VERIFY)
                                {
                                    var temp2 = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }

                                if (item.CURRENT_STEP_ID == SEND_MK_VERIFY)
                                {
                                    var temp2 = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }

                                if (item.CURRENT_STEP_ID == SEND_GM_MK)
                                {
                                    var temp2 = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }

                                if (item.CURRENT_STEP_ID == SEND_MK)
                                {
                                    var temp2 = ART_WF_ARTWORK_PROCESS_MARKETING_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_MARKETING() { ARTWORK_SUB_ID = item.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (temp2 != null)
                                        item.REMARK_OTHERS = temp2.COMMENT;
                                }

                                if (item.CURRENT_STEP_ID > 0)
                                {
                                    var stepArtwork = ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(item.CURRENT_STEP_ID, context);
                                    if (stepArtwork != null)
                                    {
                                        item.CURRENT_STEP_CODE_DISPLAY_TXT = stepArtwork.STEP_ARTWORK_CODE;
                                        item.CURRENT_STEP_DISPLAY_TXT = stepArtwork.STEP_ARTWORK_NAME;
                                    }
                                }

                                item.CURRENT_USER_DISPLAY_TXT = CNService.GetUserName(item.CURRENT_USER_ID, context);

                                var allCusStepID = (from c in context.ART_M_STEP_ARTWORK
                                                    where c.STEP_ARTWORK_CODE.Contains("SEND_CUS")
                                                        && c.IS_ACTIVE == "X"
                                                    select c.STEP_ARTWORK_ID).ToList();

                                if (item.CURRENT_STEP_ID != null)
                                {
                                    int currentStepID = 0;
                                    currentStepID = Convert.ToInt32(item.CURRENT_STEP_ID);

                                    if (allCusStepID.Contains(currentStepID))
                                    {
                                        var listCuscc = ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER() { ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID, MAIL_CC = "X", CUSTOMER_USER_ID = Convert.ToInt32(item.CURRENT_USER_ID) }, context);
                                        if (listCuscc.Count > 0)
                                        {
                                            item.CURRENT_USER_DISPLAY_TXT = "[CC] " + CNService.GetUserName(item.CURRENT_USER_ID, context);
                                        }
                                        else
                                        {
                                            item.CURRENT_USER_DISPLAY_TXT = "[TO] " + CNService.GetUserName(item.CURRENT_USER_ID, context);
                                        }
                                    }
                                    else
                                    {
                                        item.CURRENT_USER_DISPLAY_TXT = CNService.GetUserName(item.CURRENT_USER_ID, context);
                                    }
                                }

                                if (item.REMARK_KILLPROCESS != null)
                                {
                                    item.REMARK += "[Terminated]" + item.REMARK_KILLPROCESS;
                                }
                            }

                            var request = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(param.data.ARTWORK_REQUEST_ID, context);
                            var change_owner_open = ART_WF_LOG_CHANGE_OWNER_SERVICE.GetByItem(new ART_WF_LOG_CHANGE_OWNER { WF_ID = request.ARTWORK_REQUEST_ID, FROM_USER_ID = request.CREATE_BY, WF_TYPE = "A", IS_ACTIVE = "X" }, context).FirstOrDefault();
                            var change_owner_creator = ART_WF_LOG_CHANGE_OWNER_SERVICE.GetByItem(new ART_WF_LOG_CHANGE_OWNER { WF_ID = request.ARTWORK_REQUEST_ID, FROM_USER_ID = request.CREATOR_ID, WF_TYPE = "A", IS_ACTIVE = "X" }, context).FirstOrDefault();

                            ART_WF_ARTWORK_PROCESS_2 tempRequest = new ART_WF_ARTWORK_PROCESS_2();
                            tempRequest.CURRENT_STEP_DISPLAY_TXT = "Opened";
                            if (change_owner_open != null)
                            {
                                tempRequest.CURRENT_USER_DISPLAY_TXT = CNService.GetUserName(change_owner_open.TO_USER_ID, context);
                                tempRequest.CG_OWNER_REASON = change_owner_open.REMARK;
                                if (change_owner_open.FROM_USER_ID > 0) tempRequest.OLD_OWNER_CG_OWNER = CNService.GetUserName(change_owner_open.FROM_USER_ID, context);
                                else tempRequest.OLD_OWNER_CG_OWNER = "-";
                                if (change_owner_open.TO_USER_ID > 0) tempRequest.NEW_OWNER_CG_OWNER = CNService.GetUserName(change_owner_open.TO_USER_ID, context);
                                else tempRequest.NEW_OWNER_CG_OWNER = "-";
                                if (change_owner_open.CREATE_BY > 0) tempRequest.CG_OWNER_BY = CNService.GetUserName(change_owner_open.CREATE_BY, context);
                                else tempRequest.CG_OWNER_BY = "-";
                                tempRequest.IS_CG_OWNER = "X";
                            }
                            else
                            tempRequest.CURRENT_USER_DISPLAY_TXT = CNService.GetUserName(request.CREATE_BY, context);
                            tempRequest.CREATE_DATE = request.CREATE_DATE;
                            tempRequest.REMARK = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(request.ARTWORK_REQUEST_ID, context).OTHER_REQUEST;
                            tempRequest.ARTWORK_REQUEST_ID = request.ARTWORK_REQUEST_ID;
                            tempRequest.IS_END = "X";
                            Results.data.Add(tempRequest);


                            //start rewrite on 19/11/2020 for ticket 445558 by aof 
                            //  if (request.TYPE_OF_ARTWORK == "REPEAT") 
                            if (!CNService.IsMarketingCreatedArtworkRequest(request,context)) //461704 by aof 
                            {
                                var stepSendBackMK = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_BACK_MK").FirstOrDefault();
                               
                                if (stepSendBackMK != null)
                                {
                                    var list_process_SendBackMK = context.ART_WF_ARTWORK_PROCESS.Where(w => w.ARTWORK_REQUEST_ID == request.ARTWORK_REQUEST_ID
                                                                && w.CURRENT_STEP_ID == stepSendBackMK.STEP_ARTWORK_ID
                                                                && w.ARTWORK_ITEM_ID == param.data.ARTWORK_ITEM_ID).OrderByDescending(w=>w.CREATE_DATE);

                                    //var process_LastSendBackMK = context.ART_WF_ARTWORK_PROCESS.Where(w => w.ARTWORK_REQUEST_ID == request.ARTWORK_REQUEST_ID
                                    //                           && w.CURRENT_STEP_ID == stepSendBackMK.STEP_ARTWORK_ID
                                    //                           && w.ARTWORK_ITEM_ID == param.data.ARTWORK_ITEM_ID).OrderByDescending(w => w.CREATE_DATE).FirstOrDefault();

                                    if (list_process_SendBackMK != null)
                                    {

                                        foreach (ART_WF_ARTWORK_PROCESS process in list_process_SendBackMK)
                                        {
                                            if (CNService.IsMarketing(Convert.ToInt32(process.CURRENT_USER_ID), context) || CNService.IsRoleMK(Convert.ToInt32(process.CURRENT_USER_ID), context))
                                            {
                                                if (process != null)
                                                {
                                                    ART_WF_ARTWORK_PROCESS_2 tempCreator = new ART_WF_ARTWORK_PROCESS_2();
                                                    tempCreator.CURRENT_STEP_DISPLAY_TXT = "Assigned MK";

                                                    tempCreator.CURRENT_USER_DISPLAY_TXT = CNService.GetUserName(process.CURRENT_USER_ID, context);
                                                    tempCreator.CREATE_DATE = Convert.ToDateTime(process.CREATE_DATE);
                                                    //tempCreator.UPDATE_DATE = Convert.ToDateTime(process_LastSendBackMK.UPDATE_DATE);
                                                    tempCreator.ARTWORK_REQUEST_ID = request.ARTWORK_REQUEST_ID;
                                                    tempCreator.IS_END = "X";
                                                    Results.data.Add(tempCreator);
                                                    break;
                                                }
                                            }
                                        }  
                                    }            
                                }
                            }
                            else
                            {
                                if (CNService.IsMarketing(Convert.ToInt32(request.CREATOR_ID), context) || CNService.IsRoleMK(Convert.ToInt32(request.CREATOR_ID), context))
                                {
                                    ART_WF_ARTWORK_PROCESS_2 tempCreator = new ART_WF_ARTWORK_PROCESS_2();
                                    tempCreator.CURRENT_STEP_DISPLAY_TXT = "Assigned MK";
                                    if (change_owner_creator != null)
                                    {
                                        tempCreator.CURRENT_USER_DISPLAY_TXT = CNService.GetUserName(change_owner_creator.TO_USER_ID, context);
                                        tempCreator.CG_OWNER_REASON = change_owner_creator.REMARK;
                                        if (change_owner_creator.FROM_USER_ID > 0) tempCreator.OLD_OWNER_CG_OWNER = CNService.GetUserName(change_owner_creator.FROM_USER_ID, context);
                                        else tempCreator.OLD_OWNER_CG_OWNER = "-";
                                        if (change_owner_creator.TO_USER_ID > 0) tempCreator.NEW_OWNER_CG_OWNER = CNService.GetUserName(change_owner_creator.TO_USER_ID, context);
                                        else tempCreator.NEW_OWNER_CG_OWNER = "-";
                                        if (change_owner_creator.CREATE_BY > 0) tempCreator.CG_OWNER_BY = CNService.GetUserName(change_owner_creator.CREATE_BY, context);
                                        else tempCreator.CG_OWNER_BY = "-";
                                        tempCreator.IS_CG_OWNER = "X";
                                    }
                                    else
                                    tempCreator.CURRENT_USER_DISPLAY_TXT = CNService.GetUserName(request.CREATOR_ID, context);
                                    tempCreator.CREATE_DATE = Convert.ToDateTime(request.REQUEST_FORM_CREATE_DATE);
                                    tempCreator.ARTWORK_REQUEST_ID = request.ARTWORK_REQUEST_ID;
                                    tempCreator.IS_END = "X";
                                    Results.data.Add(tempCreator);
                                }
                            }
                           // last rewrite on 19/11/2020 for ticket 445558 by aof on 19/11/2020

                            ////start comment on 19/11/2020 for ticket 445558  by aof comment by aof and move to above 
                            //if (CNService.IsMarketing(Convert.ToInt32(request.CREATOR_ID), context) || CNService.IsRoleMK(Convert.ToInt32(request.CREATOR_ID), context))
                            //{
                            //    ART_WF_ARTWORK_PROCESS_2 tempCreator = new ART_WF_ARTWORK_PROCESS_2();
                            //    tempCreator.CURRENT_STEP_DISPLAY_TXT = "Assigned MK";
                            //    if (change_owner_creator != null)
                            //    {
                            //        tempCreator.CURRENT_USER_DISPLAY_TXT = CNService.GetUserName(change_owner_creator.TO_USER_ID, context);
                            //        tempCreator.CG_OWNER_REASON = change_owner_creator.REMARK;
                            //        if (change_owner_creator.FROM_USER_ID > 0) tempCreator.OLD_OWNER_CG_OWNER = CNService.GetUserName(change_owner_creator.FROM_USER_ID, context);
                            //        else tempCreator.OLD_OWNER_CG_OWNER = "-";
                            //        if (change_owner_creator.TO_USER_ID > 0) tempCreator.NEW_OWNER_CG_OWNER = CNService.GetUserName(change_owner_creator.TO_USER_ID, context);
                            //        else tempCreator.NEW_OWNER_CG_OWNER = "-";
                            //        if (change_owner_creator.CREATE_BY > 0) tempCreator.CG_OWNER_BY = CNService.GetUserName(change_owner_creator.CREATE_BY, context);
                            //        else tempCreator.CG_OWNER_BY = "-";
                            //        tempCreator.IS_CG_OWNER = "X";
                            //    }
                            //    else
                            //        tempCreator.CURRENT_USER_DISPLAY_TXT = CNService.GetUserName(request.CREATOR_ID, context);
                            //    tempCreator.CREATE_DATE = Convert.ToDateTime(request.REQUEST_FORM_CREATE_DATE);
                            //    tempCreator.ARTWORK_REQUEST_ID = request.ARTWORK_REQUEST_ID;
                            //    tempCreator.IS_END = "X";
                            //    Results.data.Add(tempCreator);
                            //}
                            ////last comment on 19/11/2020 for ticket 445558  by aof comment by aof and move to above
                        }
                    }

                    Results.data = Results.data.OrderBy(m => m.CREATE_DATE).ToList();
                }

                Results.recordsFiltered = Results.data.ToList().Count;
                Results.recordsTotal = Results.data.ToList().Count;
                Results.status = "S";
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
