using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BLL.Services;
using BLL.Helpers;
using DAL;
using DAL.Model;
using System.Data.Entity;

namespace PLL.API.WFSetting
{
    public class WFSettingInfoController : ApiController
    {
        [Route("api/wfsetting/reassign/info.bak")]  // tuning reassing by aof for backup
        public V_ART_WF_ALL_PROCESS_RESULT GetListReassign([FromUri]V_ART_WF_ALL_PROCESS_REQUEST param)
        {
            V_ART_WF_ALL_PROCESS_RESULT res = new V_ART_WF_ALL_PROCESS_RESULT();
            try
            {
                res.data = new List<V_ART_WF_ALL_PROCESS_2>();
                var list = new List<V_ART_WF_ALL_PROCESS>();

				// by aof reassign
                if (param.data.FIRST_LOAD)
                {
                    res.status = "S";
                    res.draw = param.draw;
                    return res;
                }
				// by aof reassign

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        DateTime REQUEST_DATE_FROM = new DateTime();
                        DateTime REQUEST_DATE_TO = new DateTime();
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_FROM)) REQUEST_DATE_FROM = CNService.ConvertStringToDate(param.data.CREATE_DATE_FROM);
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_TO)) REQUEST_DATE_TO = CNService.ConvertStringToDate(param.data.CREATE_DATE_TO);

                        IQueryable<V_ART_WF_ALL_PROCESS> q = null;

                        q = (from m in context.V_ART_WF_ALL_PROCESS
                             select m);

                        var positionId = ART_M_USER_SERVICE.GetByUSER_ID(param.data.USER_ID, context).POSITION_ID;
                        var positionCode = ART_M_POSITION_SERVICE.GetByART_M_POSITION_ID(positionId, context).ART_M_POSITION_CODE;

                        if (positionCode == "ADMIN")
                        {

                        }
                        else if (positionCode == "PK")
                        {
                            var isPG = false;
                            var isPP = false;
                            var listRole = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { USER_ID = param.data.USER_ID }, context);
                            foreach (var itemRole in listRole)
                            {
                                var roleCode = ART_M_ROLE_SERVICE.GetByROLE_ID(itemRole.ROLE_ID, context).ROLE_CODE;
                                if (roleCode.StartsWith("PG_"))
                                {
                                    isPG = true;
                                }

                                if (roleCode == "PP_STAFF")
                                {
                                    isPP = true;
                                }
                            }

                            if (isPG)
                            {
                                var SEND_PG_ARTWORK = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                                q = (from r in q where r.WF_TYPE == "Mockup" || (r.WF_TYPE == "Artwork" && r.CURRENT_STEP_ID == SEND_PG_ARTWORK) select r);
                            }
                            else if (isPP)
                            {
                                q = (from r in q where r.CURRENT_USER_ID == param.data.USER_ID select r);
                            }
                            else
                            {
                                q = (from r in q where r.WF_TYPE == "Artwork" select r);
                            }

                            #region Sup
                            //else if (positionCode == "PK")
                            //{
                            //    var isPG = false;
                            //    var isSupPG = false;
                            //    var isPA = false;
                            //    var isSupPA = false;
                            //    var isUpper = false;
                            //    var listRole = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { USER_ID = param.data.USER_ID });
                            //    foreach (var itemRole in listRole)
                            //    {
                            //        var roleCode = ART_M_ROLE_SERVICE.GetByROLE_ID(itemRole.ROLE_ID).ROLE_CODE;
                            //        if (roleCode.StartsWith("PK"))
                            //        {
                            //            isUpper = true; break;
                            //        }
                            //        else if (roleCode.StartsWith("PG_"))
                            //        {
                            //            if (roleCode == "PG_TEAM_LEAD" || roleCode == "PG_SUPPERVISOR" || roleCode == "PG_MANAGER")
                            //            {
                            //                isSupPG = true;
                            //                isPG = true;
                            //            }
                            //            else
                            //                isPG = true;
                            //        }
                            //        else if (roleCode.StartsWith("PA_"))
                            //        {
                            //            if (roleCode == "PA_TEAM_LEAD" || roleCode == "PA_SUPERVISOR" || roleCode == "PA_ASS_MANAGER")
                            //            {
                            //                isSupPA = true;
                            //                isPA = true;
                            //            }
                            //            else
                            //                isPA = true;
                            //        }
                            //    }
                            //    var SEND_PG_ARTWORK = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PG" }).FirstOrDefault().STEP_ARTWORK_ID;
                            //    var SEND_PA_ARTWORK = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }).FirstOrDefault().STEP_ARTWORK_ID;

                            //    if (isUpper)
                            //    {
                            //        q = (from r in q where r.WF_TYPE == "Mockup" || (r.WF_TYPE == "Artwork" && (r.CURRENT_STEP_ID == SEND_PG_ARTWORK || r.CURRENT_STEP_ID == SEND_PA_ARTWORK)) select r);
                            //    }
                            //    else if (isPG)
                            //    {
                            //        if (isSupPG)
                            //            q = (from r in q where r.WF_TYPE == "Mockup" || (r.WF_TYPE == "Artwork" && r.CURRENT_STEP_ID == SEND_PG_ARTWORK) select r);
                            //        else
                            //            q = (from r in q where r.WF_TYPE == "Mockup" || (r.WF_TYPE == "Artwork" && r.CURRENT_USER_ID == param.data.USER_ID) select r);
                            //    }
                            //    else if (isPA)
                            //    {
                            //        if (isSupPA)
                            //            q = (from r in q where (r.WF_TYPE == "Artwork" && r.CURRENT_STEP_ID == SEND_PA_ARTWORK) select r);
                            //        else
                            //            q = (from r in q where (r.WF_TYPE == "Artwork" && r.CURRENT_USER_ID == param.data.USER_ID) select r);
                            //    }
                            //    else
                            //    {
                            //        q = (from r in q where r.WF_TYPE == "Artwork" select r);
                            //    }
                            //}
                            //else if (positionCode == "MK")
                            //{
                            //    var isUpper = false;
                            //    var listRole = ART_M_USER_ROLE_SERVICE.GetByItem(new ART_M_USER_ROLE() { USER_ID = param.data.USER_ID });
                            //    foreach (var itemRole in listRole)
                            //    {
                            //        var roleCode = ART_M_ROLE_SERVICE.GetByROLE_ID(itemRole.ROLE_ID).ROLE_CODE;
                            //        if (roleCode == "MK_CD_MC_MANAGER" || roleCode == "MC_AM" || roleCode == "MC_SUPERVISOR" || roleCode == "MK_GM" || roleCode == "MK_CD_AM")
                            //        {
                            //            isUpper = true; break;
                            //        }
                            //    }
                            //    var SEND_MK_ARTWORK = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }).FirstOrDefault().STEP_ARTWORK_ID;

                            //    if (isUpper)
                            //        q = (from r in q where r.CURRENT_STEP_ID == SEND_MK_ARTWORK || r.OWNER_ID == param.data.USER_ID || r.CREATOR_ID == param.data.CREATOR_ID select r);
                            //    else
                            //        q = (from r in q where r.CURRENT_USER_ID == param.data.USER_ID || r.OWNER_ID == param.data.USER_ID || r.CREATOR_ID == param.data.CREATOR_ID select r);
                            //}
                            //else
                            //{
                            //    q = (from r in q where r.CURRENT_USER_ID == param.data.USER_ID select r);
                            //}
                            #endregion

                        }
                        else if (positionCode == "MK")
                        {
                            var SEND_CUS_REQ_REF = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REQ_REF" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_CUS_REF = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REF" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_CUS_REVIEW = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REVIEW" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_CUS_SHADE = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_SHADE" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                            var SEND_CUS_PRINT = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_PRINT" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                            q = (from r in q
                                 where r.CURRENT_USER_ID == param.data.USER_ID || r.OWNER_ID == param.data.USER_ID || r.CREATOR_ID == param.data.CREATOR_ID
                                 || r.CURRENT_STEP_ID == SEND_CUS_REQ_REF || r.CURRENT_STEP_ID == SEND_CUS_REF || r.CURRENT_STEP_ID == SEND_CUS_SHADE
                                 || r.CURRENT_STEP_ID == SEND_CUS_REVIEW || r.CURRENT_STEP_ID == SEND_CUS_PRINT
                                 select r);
                        }
                        else
                        {
                            q = (from r in q where r.CURRENT_USER_ID == param.data.USER_ID select r);
                        }

                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_FROM))
                            q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE) >= REQUEST_DATE_FROM select r);
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_TO))
                            q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE) <= REQUEST_DATE_TO select r);

                        q = (from r in q where string.IsNullOrEmpty(r.IS_END) select r);

                        if (param.data.CURRENT_USER_ID > 0)
                            q = (from r in q where r.CURRENT_USER_ID == param.data.CURRENT_USER_ID select r);

                        if (!string.IsNullOrEmpty(param.data.WF_NO))
                        {
                            q = (from r in q where (r.WF_NO.Contains(param.data.WF_NO.Trim()) || r.REQUEST_NO.Contains(param.data.WF_NO.Trim())) select r);
                        }

                        if (!string.IsNullOrEmpty(param.data.WF_TYPE))
                        {
                            if (param.data.WF_TYPE == "M")
                                q = (from r in q where r.WF_TYPE == "Mockup" select r);
                            else if (param.data.WF_TYPE == "A")
                                q = (from r in q where r.WF_TYPE == "Artwork" select r);
                        }

                        int cnt = q.Count();

                        var orderColumn = 2;
                        var orderDir = "asc";
                        if (param.order != null && param.order.Count > 0)
                        {
                            orderColumn = param.order[0].column;
                            orderDir = param.order[0].dir; //desc ,asc
                        }

                        string orderASC = "asc";
                        string orderDESC = "desc";

                        if (orderColumn == 2)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.WF_TYPE).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.WF_TYPE).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 3)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 4)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.WF_NO).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.WF_NO).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 5)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.CURRENT_STEP_NAME).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.CURRENT_STEP_NAME).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 6)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.CURRENT_USER_NAME).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.CURRENT_USER_NAME).Skip(param.start).Take(param.length).ToList();
                        }

                        res.draw = param.draw;
                        res.recordsTotal = cnt;
                        res.recordsFiltered = cnt;

                        var msg = MessageHelper.GetMessage("MSG_005", context);
                        foreach (var item in list)
                        {
                            var tempProcess = MapperServices.V_ART_WF_ALL_PROCESS(item);
                            tempProcess.CURRENT_STEP_DISPLAY_TXT = item.CURRENT_STEP_NAME;

                            if (item.CURRENT_USER_ID > 0)
                            {
                                if (item.CURRENT_VENDOR_ID > 0)
                                    tempProcess.CURRENT_USER_DISPLAY_TXT = tempProcess.CURRENT_USER_NAME + " [" + CNService.GetVendorName(item.CURRENT_VENDOR_ID, context) + "]";
                                else if (item.CURRENT_CUSTOMER_ID > 0)
                                    tempProcess.CURRENT_USER_DISPLAY_TXT = tempProcess.CURRENT_USER_NAME + " [" + CNService.GetCustomerName(item.CURRENT_CUSTOMER_ID, context) + "]";
                                else
                                    tempProcess.CURRENT_USER_DISPLAY_TXT = tempProcess.CURRENT_USER_NAME + " [" + CNService.GetPositionUser(item.CURRENT_USER_ID, context) + "]";
                            }
                            else
                            {
                                tempProcess.CURRENT_USER_DISPLAY_TXT = msg;
                            }
                            tempProcess.WORKFLOW_SUB_ID = item.WF_SUB_ID;
                            tempProcess.WORKFLOW_NO = item.WF_NO;
                            tempProcess.WORKFLOW_TYPE = item.WF_TYPE;

                            res.data.Add(tempProcess);
                        }
                    }
                }

                res.status = "S";
            }
            catch (Exception ex)
            {
                res.status = "E";
                res.msg = CNService.GetErrorMessage(ex);
            }
            return res;
        }


        //----------------------------------------------- tuning reassing by aof---------------------------------------------------------
        [Route("api/wfsetting/reassign/info")]
        public V_ART_WF_ALL_PROCESS_RESULT GetListReassign_TU([FromUri]V_ART_WF_ALL_PROCESS_REQUEST param)
        {
            V_ART_WF_ALL_PROCESS_RESULT res = new V_ART_WF_ALL_PROCESS_RESULT();
            try
            {
                res.data = new List<V_ART_WF_ALL_PROCESS_2>();
                var list = new List<V_ART_WF_ALL_PROCESS_2>();

                // by aof reassign
                if (param.data.FIRST_LOAD)
                {
                    res.status = "S";
                    res.draw = param.draw;
                    return res;
                }
                // by aof reassign

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        DateTime REQUEST_DATE_FROM = new DateTime();
                        DateTime REQUEST_DATE_TO = new DateTime();
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_FROM)) REQUEST_DATE_FROM = CNService.ConvertStringToDate(param.data.CREATE_DATE_FROM);
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_TO)) REQUEST_DATE_TO = CNService.ConvertStringToDate(param.data.CREATE_DATE_TO);

                        string where = "";
                        string wf_type = param.data.WF_TYPE;
                        var current_user_id = param.data.USER_ID;



                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_FROM))
                            where = CNService.getSQLWhereByJoinStringWithAnd(where, "CONVERT(DATE,CREATE_DATE) >='"+ REQUEST_DATE_FROM.ToString("yyyy-MM-dd") + "'");
                            //q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE) >= REQUEST_DATE_FROM select r);
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_TO))
                            where = CNService.getSQLWhereByJoinStringWithAnd(where, "CONVERT(DATE,CREATE_DATE) <='" + REQUEST_DATE_TO.ToString("yyyy-MM-dd") + "'");
                        // q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE) <= REQUEST_DATE_TO select r);
                        //  q = (from r in q where string.IsNullOrEmpty(r.IS_END) select r);

                        if (param.data.CURRENT_USER_ID > 0)
                            where = CNService.getSQLWhereByJoinStringWithAnd(where, "CURRENT_USER_ID ='" + param.data.CURRENT_USER_ID  + "'");
                        //  q = (from r in q where r.CURRENT_USER_ID == param.data.CURRENT_USER_ID select r);

                        if (!string.IsNullOrEmpty(param.data.WF_NO))
                        { 
                            string wf = CNService.getSQLWhereLikeByConvertString(param.data.WF_NO.Trim(), "WF_NO",true,true,true); // by aof on 01/04/2022
                            string rq = CNService.getSQLWhereLikeByConvertString(param.data.WF_NO.Trim(), "REQUEST_NO",true,true,true); // by aof on 01/04/2022 
                                                                                                                                        // where = CNService.getSQLWhereByJoinStringWithAnd(where, "((WF_NO LIKE '%" + param.data.WF_NO + "%') OR (REQUEST_NO LIKE '%" + param.data.WF_NO + "%'))");
                            if (!string.IsNullOrEmpty(wf) && !string.IsNullOrEmpty(rq))
                            {
                                where = CNService.getSQLWhereByJoinStringWithAnd(where, "((" + wf + ") OR (" + rq + "))");  // by aof on 01/04/2022
                                                                                                                            // q = (from r in q where (r.WF_NO.Contains(param.data.WF_NO.Trim()) || r.REQUEST_NO.Contains(param.data.WF_NO.Trim())) select r);
                            }

                        }

                        if (!string.IsNullOrEmpty(param.data.REMARK))  // by aof on 01/04/2022
                        {
                            where = CNService.getSQLWhereByJoinStringWithAnd(where, "WFTYPE_CURRENT_STEP_ID ='" + param.data.REMARK + "'");
                        }

                        //if (!string.IsNullOrEmpty(param.data.WF_TYPE))
                        //{
                        //    if (param.data.WF_TYPE == "M")
                        //        q = (from r in q where r.WF_TYPE == "Mockup" select r);
                        //    else if (param.data.WF_TYPE == "A")
                        //        q = (from r in q where r.WF_TYPE == "Artwork" select r);
                        //}

                        var q = context.Database.SqlQuery<V_ART_WF_ALL_PROCESS_2>
                       ("sp_ART_SETTING_REASSIGN_WF @where, @wf_type, @current_user_id"
                       , new System.Data.SqlClient.SqlParameter("@where", where)
                       , new System.Data.SqlClient.SqlParameter("@wf_type", wf_type)
                       , new System.Data.SqlClient.SqlParameter("@current_user_id", current_user_id)
                       ).ToList();


                        int cnt = q.Count();

                        var orderColumn = 2;
                        var orderDir = "asc";
                        if (param.order != null && param.order.Count > 0)
                        {
                            orderColumn = param.order[0].column;
                            orderDir = param.order[0].dir; //desc ,asc
                        }

                        string orderASC = "asc";
                        string orderDESC = "desc";

                        if (orderColumn == 2)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.WF_TYPE).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.WF_TYPE).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 3)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 4)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.WF_NO).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.WF_NO).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 5)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.CURRENT_STEP_NAME).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.CURRENT_STEP_NAME).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 6)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.CURRENT_USER_NAME).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.CURRENT_USER_NAME).Skip(param.start).Take(param.length).ToList();
                        }

                        res.draw = param.draw;
                        res.recordsTotal = cnt;
                        res.recordsFiltered = cnt;

                        res.data = list;
                    }
                }

                res.status = "S";
            }
            catch (Exception ex)
            {
                res.status = "E";
                res.msg = CNService.GetErrorMessage(ex);
            }
            return res;
        }
        //----------------------------------------------- tuning reassing by aof---------------------------------------------------------

        [Route("api/wfsetting/reassign/info")]
        public ART_WF_MOCKUP_PROCESS_RESULT PostReassign(ART_WF_MOCKUP_PROCESS_REQUEST_LIST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT res = new ART_WF_MOCKUP_PROCESS_RESULT();
            try
            {


   
                string forReason = "";
                bool multiWF = false;
                string allWF = "";
                string oneWF = "";
                string userto = "";
                int totalWF = 0;

                res.data = new List<ART_WF_MOCKUP_PROCESS_2>();

                using (var context = new ARTWORKEntities())
                {
                    var listProcess = new List<ART_WF_MOCKUP_PROCESS>();
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        foreach (var item in param.data)
                        {
                            if (item.WORKFLOW_TYPE == "Mockup")
                            {
                                //add history
                                var process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(item.WORKFLOW_SUB_ID, context);
                                //var newProcess = MapperServices.ART_WF_MOCKUP_PROCESS(process);
                                //newProcess.IS_END = "X";
                                //newProcess.MOCKUP_SUB_ID = 0;
                                //newProcess.CURRENT_USER_ID = 0;
                                //newProcess.CURRENT_STEP_ID = 0;
                                //newProcess.CREATE_BY = item.CREATE_BY;
                                //newProcess.UPDATE_BY = item.UPDATE_BY;
                                //newProcess.REMARK = "Re-Assign by : " + CNService.GetUserName(item.CREATE_BY);
                                //newProcess.REMARK += "<br/>Re-Assign to : " + CNService.GetUserName(item.CURRENT_USER_ID);
                                //newProcess.REMARK += "<br/>At step : " + ART_M_STEP_MOCKUP_SERVICE.GetBySTEP_MOCKUP_ID(process.CURRENT_STEP_ID).STEP_MOCKUP_NAME;
                                //newProcess.REMARK += "<br/>Remark : " + item.REMARK_REASSIGN;
                                //ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(MapperServices.ART_WF_MOCKUP_PROCESS(newProcess), context);

                                if (item.CURRENT_USER_ID == 0) item.CURRENT_USER_ID = null;

                                ART_WF_LOG_REASSIGN model = new ART_WF_LOG_REASSIGN();
                                model.WF_TYPE = "M";
                                model.WF_SUB_ID = item.WORKFLOW_SUB_ID;
                                model.FROM_USER_ID = process.CURRENT_USER_ID;
                                model.TO_USER_ID = item.CURRENT_USER_ID;
                                model.REASSIGN_BY = item.CREATE_BY;
                                model.STEP_ID = process.CURRENT_STEP_ID;
                                model.REMARK = item.REMARK_REASSIGN;
                                model.CREATE_BY = item.CREATE_BY;
                                model.UPDATE_BY = item.UPDATE_BY;
                                ART_WF_LOG_REASSIGN_SERVICE.SaveOrUpdate(model, context);

                                forReason = item.REMARK_REASSIGN;

                                //update process
                                var process2 = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(item.WORKFLOW_SUB_ID, context);
                                process2.CURRENT_USER_ID = item.CURRENT_USER_ID;
                                process2.UPDATE_BY = item.UPDATE_BY;
                                CNService.CheckDelegateBeforeRounting(process2, context);

                                listProcess.Add(process);

                                var WF_NO = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(process2.MOCKUP_ID, context).MOCKUP_NO;
                                if (allWF == "")
                                {
                                    oneWF = WF_NO;
                                    allWF = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + WF_NO;
                                }
                                else
                                {
                                    multiWF = true;
                                    allWF += "<br/>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + WF_NO;
                                }
                                userto = CNService.GetUserName(item.CURRENT_USER_ID, context);
                                totalWF++;
                            }
                        }

                        dbContextTransaction.Commit();

                        foreach (var process in listProcess)
                        {
                            BLL.Services.EmailService.sendEmailMockup(process.MOCKUP_ID, process.MOCKUP_SUB_ID, "WF_REASSIGN", context, forReason);
                        }
                    }
                }

                using (var context = new ARTWORKEntities())
                {
                    var listProcess = new List<ART_WF_ARTWORK_PROCESS>();
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        foreach (var item in param.data)
                        {
                            if (item.WORKFLOW_TYPE == "Artwork")
                            {
                                //add history
                                var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(item.WORKFLOW_SUB_ID, context);
                                //var newProcess = MapperServices.ART_WF_ARTWORK_PROCESS(process);
                                //newProcess.IS_END = "X";
                                //newProcess.ARTWORK_SUB_ID = 0;
                                //newProcess.CURRENT_USER_ID = 0;
                                //newProcess.CURRENT_STEP_ID = 0;
                                //newProcess.CREATE_BY = item.CREATE_BY;
                                //newProcess.UPDATE_BY = item.UPDATE_BY;
                                ////newProcess.REMARK = "Re-Assign by " + CNService.GetUserName(item.CREATE_BY) + "<br/>Remark " + item.REMARK_REASSIGN;
                                //newProcess.REMARK = "Re-Assign by : " + CNService.GetUserName(item.CREATE_BY);
                                //newProcess.REMARK += "<br/>Re-Assign to : " + CNService.GetUserName(item.CURRENT_USER_ID);
                                //newProcess.REMARK += "<br/>At step : " + ART_M_STEP_ARTWORK_SERVICE.GetBySTEP_ARTWORK_ID(process.CURRENT_STEP_ID).STEP_ARTWORK_NAME;
                                //newProcess.REMARK += "<br/>Remark : " + item.REMARK_REASSIGN;
                                //ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(MapperServices.ART_WF_ARTWORK_PROCESS(newProcess), context);

                                if (item.CURRENT_USER_ID == 0) item.CURRENT_USER_ID = null;

                                ART_WF_LOG_REASSIGN model = new ART_WF_LOG_REASSIGN();
                                model.WF_TYPE = "A";
                                model.WF_SUB_ID = item.WORKFLOW_SUB_ID;
                                model.FROM_USER_ID = process.CURRENT_USER_ID;
                                model.TO_USER_ID = item.CURRENT_USER_ID;
                                model.REASSIGN_BY = item.CREATE_BY;
                                model.STEP_ID = process.CURRENT_STEP_ID;
                                model.REMARK = item.REMARK_REASSIGN;
                                model.CREATE_BY = item.CREATE_BY;
                                model.UPDATE_BY = item.UPDATE_BY;
                                ART_WF_LOG_REASSIGN_SERVICE.SaveOrUpdate(model, context);

                                forReason = item.REMARK_REASSIGN;

                                //update process
                                var process2 = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(item.WORKFLOW_SUB_ID, context);
                                process2.CURRENT_USER_ID = item.CURRENT_USER_ID;
                                process2.UPDATE_BY = item.UPDATE_BY;
                                CNService.CheckDelegateBeforeRountingArtwork(process2, context);

                                listProcess.Add(process);

                                var WF_NO = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(process2.ARTWORK_ITEM_ID, context).REQUEST_ITEM_NO;
                                if (allWF == "")
                                {
                                    oneWF = WF_NO;
                                    allWF = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + WF_NO;
                                }
                                else
                                {
                                    multiWF = true;
                                    allWF += "<br/>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + WF_NO;
                                }
                                userto = CNService.GetUserName(item.CURRENT_USER_ID, context);
                                totalWF++;
                            }
                        }

                        dbContextTransaction.Commit();

                        foreach (var process in listProcess)
                        {
                            BLL.Services.EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_REASSIGN", context, forReason);
                        }
                    }
                }

                res.status = "S";
                res.msg = "Total : &nbsp;" + totalWF + " Workflow <br/>";
                res.msg += "Assiged to : " + userto + "<br/>";

                if (multiWF)
                {
                    res.msg += "Workflow no : " + "<br/>";
                    res.msg += allWF;
                }
                else
                {
                    res.msg += "Workflow no : " + oneWF;
                }
            }
            catch (Exception ex)
            {
                res.status = "E";
                res.msg = CNService.GetErrorMessage(ex);
            }
            return res;
        }

        [Route("api/wfsetting/reopen/info.bak")]
        public V_ART_WF_ALL_PROCESS_RESULT GetListReopen([FromUri]V_ART_WF_ALL_PROCESS_REQUEST param)
        {
            V_ART_WF_ALL_PROCESS_RESULT res = new V_ART_WF_ALL_PROCESS_RESULT();
            try
            {
                res.data = new List<V_ART_WF_ALL_PROCESS_2>();
                var list = new List<V_ART_WF_ALL_PROCESS>();

                // by aof reopen
                if (param.data.FIRST_LOAD)
                {
                    res.status = "S";
                    res.draw = param.draw;
                    return res;
                }
                // by aof reopen


                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;

                        DateTime REQUEST_DATE_FROM = new DateTime();
                        DateTime REQUEST_DATE_TO = new DateTime();
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_FROM)) REQUEST_DATE_FROM = CNService.ConvertStringToDate(param.data.CREATE_DATE_FROM);
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_TO)) REQUEST_DATE_TO = CNService.ConvertStringToDate(param.data.CREATE_DATE_TO);

                        IQueryable<V_ART_WF_ALL_PROCESS> q = null;

                        q = (from m in context.V_ART_WF_ALL_PROCESS
                             where m.CURRENT_STEP_ID == SEND_PG && m.WF_TYPE == "Mockup" && m.IS_END == "X"
                             select m)
                            .Union(from m2 in context.V_ART_WF_ALL_PROCESS
                                   where m2.CURRENT_STEP_ID == SEND_PA && m2.WF_TYPE == "Artwork" && m2.IS_END == "X"
                                   select m2);

                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_FROM))
                            q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE) >= REQUEST_DATE_FROM select r);
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_TO))
                            q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE) <= REQUEST_DATE_TO select r);

                        if (param.data.CURRENT_USER_ID > 0)
                            q = (from r in q where r.CURRENT_USER_ID == param.data.CURRENT_USER_ID select r);

                        if (!string.IsNullOrEmpty(param.data.WF_NO))
                        {
                            q = (from r in q where (r.WF_NO.Contains(param.data.WF_NO.Trim()) || r.REQUEST_NO.Contains(param.data.WF_NO.Trim())) select r);
                        }

                        if (!string.IsNullOrEmpty(param.data.WF_TYPE))
                        {
                            if (param.data.WF_TYPE == "M")
                                q = (from r in q where r.WF_TYPE == "Mockup" select r);
                            else if (param.data.WF_TYPE == "A")
                                q = (from r in q where r.WF_TYPE == "Artwork" select r);
                        }

                        int cnt = q.Count();

                        var orderColumn = 2;
                        var orderDir = "asc";
                        if (param.order != null && param.order.Count > 0)
                        {
                            orderColumn = param.order[0].column;
                            orderDir = param.order[0].dir; //desc ,asc
                        }
                        string orderASC = "asc";
                        string orderDESC = "desc";

                        if (orderColumn == 2)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.WF_TYPE).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.WF_TYPE).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 3)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 4)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.WF_NO).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.WF_NO).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 5)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.CURRENT_STEP_NAME).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.CURRENT_STEP_NAME).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 6)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.CURRENT_USER_NAME).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.CURRENT_USER_NAME).Skip(param.start).Take(param.length).ToList();
                        }

                        res.draw = param.draw;
                        res.recordsTotal = cnt;
                        res.recordsFiltered = cnt;

                        var msg = MessageHelper.GetMessage("MSG_005", context);
                        foreach (var item in list)
                        {
                            var tempProcess = MapperServices.V_ART_WF_ALL_PROCESS(item);
                            tempProcess.CURRENT_STEP_DISPLAY_TXT = item.CURRENT_STEP_NAME + " [Completed]";

                            if (item.IS_TERMINATE == "X")
                                tempProcess.CURRENT_STEP_DISPLAY_TXT = item.CURRENT_STEP_NAME + " [Terminated]";

                            if (item.CURRENT_USER_ID > 0)
                            {
                                if (item.CURRENT_VENDOR_ID > 0)
                                    tempProcess.CURRENT_USER_DISPLAY_TXT = tempProcess.CURRENT_USER_NAME + " [" + CNService.GetVendorName(item.CURRENT_VENDOR_ID, context) + "]";
                                else if (item.CURRENT_CUSTOMER_ID > 0)
                                    tempProcess.CURRENT_USER_DISPLAY_TXT = tempProcess.CURRENT_USER_NAME + " [" + CNService.GetCustomerName(item.CURRENT_CUSTOMER_ID, context) + "]";
                                else
                                    tempProcess.CURRENT_USER_DISPLAY_TXT = tempProcess.CURRENT_USER_NAME + " [" + CNService.GetPositionUser(item.CURRENT_USER_ID, context) + "]";
                            }
                            else
                            {
                                tempProcess.CURRENT_USER_DISPLAY_TXT = msg;
                            }
                            tempProcess.WORKFLOW_SUB_ID = item.WF_SUB_ID;
                            tempProcess.WORKFLOW_NO = item.WF_NO;
                            tempProcess.WORKFLOW_TYPE = item.WF_TYPE;

                            res.data.Add(tempProcess);
                        }
                    }
                }

                res.status = "S";
            }
            catch (Exception ex)
            {
                res.status = "E";
                res.msg = CNService.GetErrorMessage(ex);
            }
            return res;
        }

        //----------------------------------------------- tuning reopen by aof---------------------------------------------------------
        [Route("api/wfsetting/reopen/info")]
        public V_ART_WF_ALL_PROCESS_RESULT GetListReopen_TU([FromUri]V_ART_WF_ALL_PROCESS_REQUEST param)
        {
            V_ART_WF_ALL_PROCESS_RESULT res = new V_ART_WF_ALL_PROCESS_RESULT();
            try
            {
                res.data = new List<V_ART_WF_ALL_PROCESS_2>();
                var list = new List<V_ART_WF_ALL_PROCESS_2>();

                // by aof reopen
                if (param.data.FIRST_LOAD)
                {
                    res.status = "S";
                    res.draw = param.draw;
                    return res;
                }
                // by aof reopen


                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        //var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        //var SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;

                        DateTime REQUEST_DATE_FROM = new DateTime();
                        DateTime REQUEST_DATE_TO = new DateTime();
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_FROM)) REQUEST_DATE_FROM = CNService.ConvertStringToDate(param.data.CREATE_DATE_FROM);
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_TO)) REQUEST_DATE_TO = CNService.ConvertStringToDate(param.data.CREATE_DATE_TO);


                        string where = "";
                        string wf_type = param.data.WF_TYPE;
                        //var current_user_id = param.data.USER_ID;



                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_FROM))
                            where = CNService.getSQLWhereByJoinStringWithAnd(where, "CONVERT(DATE,CREATE_DATE) >='" + REQUEST_DATE_FROM.ToString("yyyy-MM-dd") + "'");
                     
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_TO))
                            where = CNService.getSQLWhereByJoinStringWithAnd(where, "CONVERT(DATE,CREATE_DATE) <='" + REQUEST_DATE_TO.ToString("yyyy-MM-dd") + "'");
                       
                        if (param.data.CURRENT_USER_ID > 0)
                            where = CNService.getSQLWhereByJoinStringWithAnd(where, "CURRENT_USER_ID ='" + param.data.CURRENT_USER_ID + "'");
                     
                        if (!string.IsNullOrEmpty(param.data.WF_NO))
                            where = CNService.getSQLWhereByJoinStringWithAnd(where, "((WF_NO LIKE '%" + param.data.WF_NO + "%') OR (REQUEST_NO LIKE '%" + param.data.WF_NO + "%'))");

                        var q = context.Database.SqlQuery<V_ART_WF_ALL_PROCESS_2>
                             ("sp_ART_SETTING_REOPEN_WF @where, @wf_type"
                            , new System.Data.SqlClient.SqlParameter("@where", where)
                            , new System.Data.SqlClient.SqlParameter("@wf_type", wf_type)                         
                            ).ToList();


                        int cnt = q.Count();

                        var orderColumn = 2;
                        var orderDir = "asc";
                        if (param.order != null && param.order.Count > 0)
                        {
                            orderColumn = param.order[0].column;
                            orderDir = param.order[0].dir; //desc ,asc
                        }
                        string orderASC = "asc";
                        string orderDESC = "desc";

                        if (orderColumn == 2)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.WF_TYPE).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.WF_TYPE).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 3)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 4)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.WF_NO).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.WF_NO).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 5)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.CURRENT_STEP_NAME).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.CURRENT_STEP_NAME).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 6)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.CURRENT_USER_NAME).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.CURRENT_USER_NAME).Skip(param.start).Take(param.length).ToList();
                        }

                        res.draw = param.draw;
                        res.recordsTotal = cnt;
                        res.recordsFiltered = cnt;

                        res.data = list;
                    }
                }

                res.status = "S";
            }
            catch (Exception ex)
            {
                res.status = "E";
                res.msg = CNService.GetErrorMessage(ex);
            }
            return res;
        }
        //----------------------------------------------- tuning reopen by aof---------------------------------------------------------

        [Route("api/wfsetting/reopen/info")]
        public ART_WF_MOCKUP_PROCESS_RESULT PostReopen(ART_WF_MOCKUP_PROCESS_REQUEST_LIST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT res = new ART_WF_MOCKUP_PROCESS_RESULT();
            try
            {
                res.data = new List<ART_WF_MOCKUP_PROCESS_2>();

                string forReason = "";
                bool multiWF = false;
                string allWF = "";
                string oneWF = "";
                int totalWF = 0;

                using (var context = new ARTWORKEntities())
                {
                    var listProcess = new List<ART_WF_MOCKUP_PROCESS>();
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        foreach (var item in param.data)
                        {
                            if (item.WORKFLOW_TYPE == "Mockup")
                            {
                                var process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(item.WORKFLOW_SUB_ID, context);

                                //add history
                                //var newProcess = MapperServices.ART_WF_MOCKUP_PROCESS(process);
                                //newProcess.IS_END = "X";
                                //newProcess.MOCKUP_SUB_ID = 0;
                                //newProcess.CURRENT_USER_ID = 0;
                                //newProcess.CURRENT_STEP_ID = 0;
                                //newProcess.CREATE_BY = item.CREATE_BY;
                                //newProcess.UPDATE_BY = item.UPDATE_BY;
                                //newProcess.REMARK = "Re-Open by " + CNService.GetUserName(item.CREATE_BY) + "<br/>Remark " + item.REMARK_REASSIGN;
                                //ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(MapperServices.ART_WF_MOCKUP_PROCESS(newProcess), context);
                                ART_WF_LOG_REOPEN model = new ART_WF_LOG_REOPEN();
                                model.WF_TYPE = "M";
                                model.WF_SUB_ID = item.WORKFLOW_SUB_ID;
                                //model.FROM_USER_ID = process.CURRENT_USER_ID;
                                //model.TO_USER_ID = item.CURRENT_USER_ID;
                                model.REOPEN_BY = item.CREATE_BY;
                                model.STEP_ID = process.CURRENT_STEP_ID;
                                model.REMARK = item.REMARK_REASSIGN;
                                model.CREATE_BY = item.CREATE_BY;
                                model.UPDATE_BY = item.UPDATE_BY;
                                ART_WF_LOG_REOPEN_SERVICE.SaveOrUpdate(model, context);

                                forReason = item.REMARK_REASSIGN;

                                process.IS_END = null;
                                process.IS_TERMINATE = null;
                                process.REMARK_TERMINATE = null;
                                process.TERMINATE_REASON_CODE = null;
                                process.UPDATE_BY = item.UPDATE_BY;
                                ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(process, context);

                                listProcess.Add(process);

                                var WF_NO = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(process.MOCKUP_ID, context).MOCKUP_NO;
                                if (allWF == "")
                                {
                                    oneWF = WF_NO;
                                    allWF = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + WF_NO;
                                }
                                else
                                {
                                    multiWF = true;
                                    allWF += "<br/>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + WF_NO;
                                }
                                totalWF++;

                            }
                        }

                        dbContextTransaction.Commit();

                        foreach (var process in listProcess)
                        {
                            BLL.Services.EmailService.sendEmailMockup(process.MOCKUP_ID, process.MOCKUP_SUB_ID, "WF_REOPEN", context, forReason);
                        }
                    }
                }

                using (var context = new ARTWORKEntities())
                {
                    var listProcess = new List<ART_WF_ARTWORK_PROCESS>();
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        foreach (var item in param.data)
                        {
                            if (item.WORKFLOW_TYPE == "Artwork")
                            {
                                var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(item.WORKFLOW_SUB_ID, context);

                                //add history
                                //var newProcess = MapperServices.ART_WF_ARTWORK_PROCESS(process);
                                //newProcess.IS_END = "X";
                                //newProcess.ARTWORK_SUB_ID = 0;
                                //newProcess.CURRENT_USER_ID = 0;
                                //newProcess.CURRENT_STEP_ID = 0;
                                //newProcess.CREATE_BY = item.CREATE_BY;
                                //newProcess.UPDATE_BY = item.UPDATE_BY;
                                //newProcess.REMARK = "Re-Open by " + CNService.GetUserName(item.CREATE_BY) + "<br/>Remark " + item.REMARK_REASSIGN;
                                //ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(MapperServices.ART_WF_ARTWORK_PROCESS(newProcess), context);
                                ART_WF_LOG_REOPEN model = new ART_WF_LOG_REOPEN();
                                model.WF_TYPE = "A";
                                model.WF_SUB_ID = item.WORKFLOW_SUB_ID;
                                //model.FROM_USER_ID = process.CURRENT_USER_ID;
                                //model.TO_USER_ID = item.CURRENT_USER_ID;
                                model.REOPEN_BY = item.CREATE_BY;
                                model.STEP_ID = process.CURRENT_STEP_ID;
                                model.REMARK = item.REMARK_REASSIGN;
                                model.CREATE_BY = item.CREATE_BY;
                                model.UPDATE_BY = item.UPDATE_BY;
                                ART_WF_LOG_REOPEN_SERVICE.SaveOrUpdate(model, context);

                                forReason = item.REMARK_REASSIGN;

                                process.IS_TERMINATE = null;
                                process.REMARK_TERMINATE = null;
                                process.TERMINATE_REASON_CODE = null;
                                process.IS_END = null;
                                process.UPDATE_BY = item.UPDATE_BY;
                                ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(process, context);

                                var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                 where p.ARTWORK_SUB_ID == process.ARTWORK_SUB_ID
                                                 select p).FirstOrDefault();

                                if (processPA != null)
                                {
                                    processPA.READY_CREATE_PO = null;
                                    processPA.UPDATE_BY = item.UPDATE_BY;
                                    ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);
                                }


                                listProcess.Add(process);

                                var WF_NO = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(process.ARTWORK_ITEM_ID, context).REQUEST_ITEM_NO;
                                if (allWF == "")
                                {
                                    oneWF = WF_NO;
                                    allWF = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + WF_NO;
                                }
                                else
                                {
                                    multiWF = true;
                                    allWF += "<br/>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + WF_NO;
                                }
                                totalWF++;
                            }
                        }

                        dbContextTransaction.Commit();

                        foreach (var process in listProcess)
                        {
                            BLL.Services.EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_REOPEN", context, forReason);
                        }
                    }
                }

                res.status = "S";
                res.msg = "Total : &nbsp;" + totalWF + " Workflow <br/>";
                if (multiWF)
                {
                    res.msg += "Workflow no : " + "<br/>";
                    res.msg += allWF;
                }
                else
                {
                    res.msg += "Workflow no : " + oneWF;
                }
            }
            catch (Exception ex)
            {
                res.status = "E";
                res.msg = CNService.GetErrorMessage(ex);
            }
            return res;
        }

        [Route("api/wfsetting/recall/info")]
        public V_ART_WF_ALL_PROCESS_RESULT GetListRecall([FromUri]V_ART_WF_ALL_PROCESS_REQUEST param)
        {
            V_ART_WF_ALL_PROCESS_RESULT res = new V_ART_WF_ALL_PROCESS_RESULT();
            try
            {
                res.data = new List<V_ART_WF_ALL_PROCESS_2>();
                var list = new List<V_ART_WF_ALL_PROCESS>();

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;

                        DateTime REQUEST_DATE_FROM = new DateTime();
                        DateTime REQUEST_DATE_TO = new DateTime();
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_FROM)) REQUEST_DATE_FROM = CNService.ConvertStringToDate(param.data.CREATE_DATE_FROM);
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_TO)) REQUEST_DATE_TO = CNService.ConvertStringToDate(param.data.CREATE_DATE_TO);

                        IQueryable<V_ART_WF_ALL_PROCESS> q = null;

                        q = (from m in context.V_ART_WF_ALL_PROCESS
                             where m.CURRENT_STEP_ID == SEND_PG && m.WF_TYPE == "Mockup"
                             select m)
                            .Union(from m2 in context.V_ART_WF_ALL_PROCESS
                                   where m2.CURRENT_STEP_ID == SEND_PA && m2.WF_TYPE == "Artwork"
                                   select m2);

                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_FROM))
                            q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE) >= REQUEST_DATE_FROM select r);
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_TO))
                            q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE) <= REQUEST_DATE_TO select r);

                        q = (from r in q where string.IsNullOrEmpty(r.IS_END) select r);

                        var positionId = ART_M_USER_SERVICE.GetByUSER_ID(param.data.CREATOR_ID, context).POSITION_ID;
                        var positionCode = ART_M_POSITION_SERVICE.GetByART_M_POSITION_ID(positionId, context).ART_M_POSITION_CODE;

                        if (positionCode != "ADMIN")
                            q = (from r in q where r.OWNER_ID == param.data.CREATOR_ID || r.CREATOR_ID == param.data.CREATOR_ID select r);

                        if (param.data.CURRENT_USER_ID > 0)
                            q = (from r in q where r.CURRENT_USER_ID == param.data.CURRENT_USER_ID select r);

                        if (!string.IsNullOrEmpty(param.data.WF_NO))
                        {
                            q = (from r in q where (r.WF_NO.Contains(param.data.WF_NO.Trim()) || r.REQUEST_NO.Contains(param.data.WF_NO.Trim())) select r);
                        }

                        if (!string.IsNullOrEmpty(param.data.WF_TYPE))
                        {
                            if (param.data.WF_TYPE == "M")
                                q = (from r in q where r.WF_TYPE == "Mockup" select r);
                            else if (param.data.WF_TYPE == "A")
                                q = (from r in q where r.WF_TYPE == "Artwork" select r);
                        }

                        int cnt = q.Count();

                        var orderColumn = 2;
                        var orderDir = "asc";
                        if (param.order != null && param.order.Count > 0)
                        {
                            orderColumn = param.order[0].column;
                            orderDir = param.order[0].dir; //desc ,asc
                        }

                        string orderASC = "asc";
                        string orderDESC = "desc";

                        if (orderColumn == 2)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.WF_TYPE).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.WF_TYPE).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 3)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 4)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.WF_NO).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.WF_NO).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 5)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.CURRENT_STEP_NAME).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.CURRENT_STEP_NAME).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 6)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.CURRENT_USER_NAME).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.CURRENT_USER_NAME).Skip(param.start).Take(param.length).ToList();
                        }

                        res.draw = param.draw;
                        res.recordsTotal = cnt;
                        res.recordsFiltered = cnt;

                        var msg = MessageHelper.GetMessage("MSG_005", context);
                        foreach (var item in list)
                        {
                            var tempProcess = MapperServices.V_ART_WF_ALL_PROCESS(item);
                            tempProcess.CURRENT_STEP_DISPLAY_TXT = item.CURRENT_STEP_NAME;

                            if (item.CURRENT_USER_ID > 0)
                            {
                                if (item.CURRENT_VENDOR_ID > 0)
                                    tempProcess.CURRENT_USER_DISPLAY_TXT = tempProcess.CURRENT_USER_NAME + " [" + CNService.GetVendorName(item.CURRENT_VENDOR_ID, context) + "]";
                                else if (item.CURRENT_CUSTOMER_ID > 0)
                                    tempProcess.CURRENT_USER_DISPLAY_TXT = tempProcess.CURRENT_USER_NAME + " [" + CNService.GetCustomerName(item.CURRENT_CUSTOMER_ID, context) + "]";
                                else
                                    tempProcess.CURRENT_USER_DISPLAY_TXT = tempProcess.CURRENT_USER_NAME + " [" + CNService.GetPositionUser(item.CURRENT_USER_ID, context) + "]";
                            }
                            else
                            {
                                tempProcess.CURRENT_USER_DISPLAY_TXT = msg;
                            }
                            tempProcess.WORKFLOW_SUB_ID = item.WF_SUB_ID;
                            tempProcess.WORKFLOW_NO = item.WF_NO;
                            tempProcess.WORKFLOW_TYPE = item.WF_TYPE;

                            res.data.Add(tempProcess);
                        }
                    }
                }

                res.status = "S";
            }
            catch (Exception ex)
            {
                res.status = "E";
                res.msg = CNService.GetErrorMessage(ex);
            }
            return res;
        }

        [Route("api/wfsetting/recall/info")]
        public ART_WF_MOCKUP_PROCESS_RESULT PostRecall(ART_WF_MOCKUP_PROCESS_REQUEST_LIST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT res = new ART_WF_MOCKUP_PROCESS_RESULT();
            try
            {
                string forReason = "";

                res.data = new List<ART_WF_MOCKUP_PROCESS_2>();
                bool multiWF = false;
                string allWF = "";
                string oneWF = "";
                int totalWF = 0;

                using (var context = new ARTWORKEntities())
                {
                    var listProcess = new List<ART_WF_MOCKUP_PROCESS>();
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        foreach (var item in param.data)
                        {
                            if (item.WORKFLOW_TYPE == "Mockup")
                            {
                                //add history
                                var process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(item.WORKFLOW_SUB_ID, context);
                                var newProcess = MapperServices.ART_WF_MOCKUP_PROCESS(process);
                                newProcess.IS_END = "X";
                                newProcess.MOCKUP_SUB_ID = 0;
                                newProcess.CURRENT_USER_ID = 0;
                                newProcess.CURRENT_STEP_ID = 0;
                                newProcess.CREATE_BY = item.CREATE_BY;
                                newProcess.UPDATE_BY = item.UPDATE_BY;
                                newProcess.REMARK = "Re-Call by " + CNService.GetUserName(item.CREATE_BY, context) + "<br/>Remark " + item.REMARK_REASSIGN;
                                ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(MapperServices.ART_WF_MOCKUP_PROCESS(newProcess), context);
                                listProcess.Add(process);

                                forReason = item.REMARK_REASSIGN;

                                var WF_NO = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(newProcess.MOCKUP_ID, context).MOCKUP_NO;
                                if (allWF == "")
                                {
                                    oneWF = WF_NO;
                                    allWF = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + WF_NO; ;
                                }
                                else
                                {
                                    multiWF = true;
                                    allWF += "<br/>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + WF_NO; ;
                                }
                                totalWF++;
                            }
                        }

                        dbContextTransaction.Commit();

                        foreach (var process in listProcess)
                        {
                            BLL.Services.EmailService.sendEmailMockup(process.MOCKUP_ID, process.MOCKUP_SUB_ID, "WF_RECALL", context, forReason);
                        }
                    }
                }

                using (var context = new ARTWORKEntities())
                {
                    var listProcess = new List<ART_WF_ARTWORK_PROCESS>();
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        foreach (var item in param.data)
                        {
                            if (item.WORKFLOW_TYPE == "Artwork")
                            {
                                //add history
                                var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(item.WORKFLOW_SUB_ID, context);
                                var newProcess = MapperServices.ART_WF_ARTWORK_PROCESS(process);
                                newProcess.IS_END = "X";
                                newProcess.ARTWORK_SUB_ID = 0;
                                newProcess.CURRENT_USER_ID = 0;
                                newProcess.CURRENT_STEP_ID = 0;
                                newProcess.CREATE_BY = item.CREATE_BY;
                                newProcess.UPDATE_BY = item.UPDATE_BY;
                                newProcess.REMARK = "Re-Call by " + CNService.GetUserName(item.CREATE_BY, context) + "<br/>Remark " + item.REMARK_REASSIGN;
                                ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(MapperServices.ART_WF_ARTWORK_PROCESS(newProcess), context);
                                listProcess.Add(process);

                                forReason = item.REMARK_REASSIGN;

                                var WF_NO = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(newProcess.ARTWORK_ITEM_ID, context).REQUEST_ITEM_NO;
                                if (allWF == "")
                                {
                                    oneWF = WF_NO;
                                    allWF = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + WF_NO; ;
                                }
                                else
                                {
                                    multiWF = true;
                                    allWF += "<br/>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + WF_NO; ;
                                }
                                totalWF++;
                            }
                        }

                        dbContextTransaction.Commit();

                        foreach (var process in listProcess)
                        {
                            BLL.Services.EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_RECALL", context, forReason);
                        }
                    }
                }

                res.status = "S";
                res.msg = "Total : &nbsp;" + totalWF + " Workflow <br/>";

                if (multiWF)
                {
                    res.msg += "Workflow no : " + "<br/>";
                    res.msg += allWF;
                }
                else
                {
                    res.msg += "Workflow no : " + oneWF;
                }
            }
            catch (Exception ex)
            {
                res.status = "E";
                res.msg = CNService.GetErrorMessage(ex);
            }
            return res;
        }

        [Route("api/wfsetting/reassign2/info")]
        public V_ART_WF_ALL_PROCESS_RESULT GetListReassign2([FromUri]V_ART_WF_ALL_PROCESS_REQUEST param)
        {
            V_ART_WF_ALL_PROCESS_RESULT res = new V_ART_WF_ALL_PROCESS_RESULT();
            try
            {
                res.data = new List<V_ART_WF_ALL_PROCESS_2>();
                var list = new List<V_ART_WF_ALL_PROCESS>();

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        DateTime REQUEST_DATE_FROM = new DateTime();
                        DateTime REQUEST_DATE_TO = new DateTime();
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_FROM)) REQUEST_DATE_FROM = CNService.ConvertStringToDate(param.data.CREATE_DATE_FROM);
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_TO)) REQUEST_DATE_TO = CNService.ConvertStringToDate(param.data.CREATE_DATE_TO);

                        IQueryable<V_ART_WF_ALL_PROCESS> q = null;

                        q = (from m in context.V_ART_WF_ALL_PROCESS
                             where m.CURRENT_STEP_ID == SEND_PG && m.WF_TYPE == "Mockup"
                             select new V_ART_WF_ALL_PROCESS_2()
                             {
                                 REQUEST_NO = m.REQUEST_NO,
                                 REQUEST_ID = m.REQUEST_ID,
                                 CREATOR_ID = m.CREATOR_ID,
                                 OWNER_ID = m.OWNER_ID,
                                 CREATE_DATE = m.CREATE_DATE,
                                 IS_END = m.IS_END,
                                 CURRENT_USER_ID = m.CURRENT_USER_ID,
                                 WF_NO = m.WF_NO,
                                 WF_TYPE = m.WF_TYPE,
                                 CREATE_BY = m.CREATE_BY,
                             }).Distinct()
                            .Union(from m2 in context.V_ART_WF_ALL_PROCESS
                                   where m2.CURRENT_STEP_ID == SEND_PA && m2.WF_TYPE == "Artwork"
                                   select new V_ART_WF_ALL_PROCESS_2()
                                   {
                                       REQUEST_NO = m2.REQUEST_NO,
                                       REQUEST_ID = m2.REQUEST_ID,
                                       CREATOR_ID = m2.CREATOR_ID,
                                       OWNER_ID = m2.OWNER_ID,
                                       CREATE_DATE = m2.CREATE_DATE,
                                       IS_END = m2.IS_END,
                                       CURRENT_USER_ID = m2.CURRENT_USER_ID,
                                       WF_NO = m2.WF_NO,
                                       WF_TYPE = m2.WF_TYPE,
                                       CREATE_BY = m2.CREATE_BY,
                                   }).Distinct();

                        var positionId = ART_M_USER_SERVICE.GetByUSER_ID(param.data.USER_ID, context).POSITION_ID;
                        var positionCode = ART_M_POSITION_SERVICE.GetByART_M_POSITION_ID(positionId, context).ART_M_POSITION_CODE;

                        if (positionCode != "ADMIN")
                            q = (from r in q where r.CREATOR_ID == param.data.USER_ID || r.OWNER_ID == param.data.USER_ID select r);

                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_FROM))
                            q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE) >= REQUEST_DATE_FROM select r);
                        if (!string.IsNullOrEmpty(param.data.CREATE_DATE_TO))
                            q = (from r in q where DbFunctions.TruncateTime(r.CREATE_DATE) <= REQUEST_DATE_TO select r);

                        q = (from r in q where string.IsNullOrEmpty(r.IS_END) select r);

                        if (param.data.CURRENT_USER_ID > 0)
                            q = (from r in q where r.CURRENT_USER_ID == param.data.CURRENT_USER_ID select r);

                        if (!string.IsNullOrEmpty(param.data.WF_NO))
                        {
                            q = (from r in q where (r.WF_NO.Contains(param.data.WF_NO.Trim()) || r.REQUEST_NO.Contains(param.data.WF_NO.Trim())) select r);
                        }

                        if (!string.IsNullOrEmpty(param.data.WF_TYPE))
                        {
                            if (param.data.WF_TYPE == "M")
                                q = (from r in q where r.WF_TYPE == "Mockup" select r);
                            else if (param.data.WF_TYPE == "A")
                                q = (from r in q where r.WF_TYPE == "Artwork" select r);
                        }

                        int cnt = q.Count();

                        var orderColumn = 2;
                        var orderDir = "asc";
                        if (param.order != null && param.order.Count > 0)
                        {
                            orderColumn = param.order[0].column;
                            orderDir = param.order[0].dir; //desc ,asc
                        }

                        string orderASC = "asc";
                        string orderDESC = "desc";

                        if (orderColumn == 2)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.WF_TYPE).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.WF_TYPE).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 3)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 4)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 5)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                        }
                        if (orderColumn == 6)
                        {
                            if (orderDir == orderASC)
                                list = q.OrderBy(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                            else if (orderDir == orderDESC)
                                list = q.OrderByDescending(m => m.REQUEST_NO).Skip(param.start).Take(param.length).ToList();
                        }

                        //string previous_request_no = "";
                        V_ART_WF_ALL_PROCESS_2 tempProcess = new V_ART_WF_ALL_PROCESS_2();
                        foreach (var item in list)
                        {
                            tempProcess = new V_ART_WF_ALL_PROCESS_2();
                            var listWFNO = (from m in context.V_ART_WF_ALL_PROCESS
                                            where m.REQUEST_NO == item.REQUEST_NO && m.PARENT_SUB_ID == null
                                            select new V_ART_WF_ALL_PROCESS_2()
                                            {
                                                WF_NO = m.WF_NO,
                                                WF_SUB_ID = m.WF_SUB_ID,
                                            }).Distinct().ToList();

                            foreach (var item2 in listWFNO)
                            {
                                if (string.IsNullOrEmpty(tempProcess.WORKFLOW_NO))
                                    tempProcess.WORKFLOW_NO = item2.WF_NO + "," + item2.WF_SUB_ID.ToString();
                                else
                                    tempProcess.WORKFLOW_NO += "||" + item2.WF_NO + "," + item2.WF_SUB_ID.ToString();
                            }

                            tempProcess.REQUEST_ID = item.REQUEST_ID;
                            tempProcess.REQUEST_NO = item.REQUEST_NO;
                            tempProcess.WF_TYPE = item.WF_TYPE;
                            tempProcess.WORKFLOW_TYPE = item.WF_TYPE;

                            string wf_type = "A";
                            if (item.WF_TYPE == "Mockup")
                                wf_type = "M";

                            var change_owner = ART_WF_LOG_CHANGE_OWNER_SERVICE.GetByItem(new ART_WF_LOG_CHANGE_OWNER() { WF_TYPE = wf_type, WF_ID = item.REQUEST_ID, IS_ACTIVE = "X" }, context).FirstOrDefault();
                            if (change_owner != null)
                            {
                                if (change_owner.FROM_USER_ID != null)
                                {
                                    tempProcess.ASSIGN_FROM_USER_DISPLAY_TXT = CNService.GetUserName(change_owner.FROM_USER_ID, context) + " [" + CNService.GetPositionUser(change_owner.FROM_USER_ID, context) + "]";
                                }
                                if (change_owner.TO_USER_ID != null)
                                {
                                    tempProcess.ASSIGN_TO_USER_DISPLAY_TXT = CNService.GetUserName(change_owner.TO_USER_ID, context) + " [" + CNService.GetPositionUser(change_owner.TO_USER_ID, context) + "]";
                                }
                            }
                            else
                            {
                                tempProcess.ASSIGN_FROM_USER_DISPLAY_TXT = CNService.GetUserName(item.CREATE_BY, context) + " [" + CNService.GetPositionUser(item.CREATE_BY, context) + "]";
                            }

                            res.data.Add(tempProcess);
                        }

                        res.draw = param.draw;
                        res.recordsTotal = cnt;
                        res.recordsFiltered = cnt;
                    }
                }

                res.status = "S";
            }
            catch (Exception ex)
            {
                res.status = "E";
                res.msg = CNService.GetErrorMessage(ex);
            }
            return res;
        }

        [Route("api/wfsetting/reassign2/info")]
        public ART_WF_MOCKUP_PROCESS_RESULT PostReassign2(ART_WF_MOCKUP_PROCESS_REQUEST_LIST param)
        {
            ART_WF_MOCKUP_PROCESS_RESULT res = new ART_WF_MOCKUP_PROCESS_RESULT();
            try
            {
                string forReason = "";

                res.data = new List<ART_WF_MOCKUP_PROCESS_2>();

                bool multiWF = false;
                string allWF = "";
                string oneWF = "";
                string userto = "";
                int totalWF = 0;

                using (var context = new ARTWORKEntities())
                {
                    var listProcess = new List<ART_WF_MOCKUP_PROCESS>();
                    var listMockUp = new List<ART_WF_MOCKUP_CHECK_LIST_ITEM>();
                    var listAW = new List<ART_WF_ARTWORK_REQUEST_ITEM>();
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        foreach (var item in param.data)
                        {
                            string str_workflow_type = "A";
                            if (item.WORKFLOW_TYPE == "Mockup")
                            {
                                str_workflow_type = "M";
                            }
                            var active_change_owner = ART_WF_LOG_CHANGE_OWNER_SERVICE.GetByItem(new ART_WF_LOG_CHANGE_OWNER() { IS_ACTIVE = "X", WF_ID = item.WORKFLOW_ID, WF_TYPE = str_workflow_type }, context);
                            foreach (var change_ownen_item in active_change_owner)
                            {
                                change_ownen_item.IS_ACTIVE = null;
                                ART_WF_LOG_CHANGE_OWNER_SERVICE.SaveOrUpdate(change_ownen_item, context);
                            }

                            int? fromUserId = 0;
                            if (item.WORKFLOW_TYPE == "Mockup")
                            {
                                var requestForm = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(item.WORKFLOW_ID, context);
                                fromUserId = requestForm.CREATOR_ID;
                            }
                            else
                            {
                                var requestForm = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(item.WORKFLOW_ID, context);
                                fromUserId = requestForm.CREATOR_ID;
                            }

                            ART_WF_LOG_CHANGE_OWNER model = new ART_WF_LOG_CHANGE_OWNER();
                            model.WF_TYPE = str_workflow_type;
                            model.WF_ID = item.WORKFLOW_ID;
                            model.FROM_USER_ID = fromUserId;
                            model.TO_USER_ID = item.CURRENT_USER_ID;
                            model.IS_ACTIVE = "X";
                            model.REMARK = item.REMARK_REASSIGN;
                            model.CREATE_BY = item.CREATE_BY;
                            model.UPDATE_BY = item.UPDATE_BY;
                            ART_WF_LOG_CHANGE_OWNER_SERVICE.SaveOrUpdate(model, context);

                            forReason = item.REMARK_REASSIGN;

                            if (str_workflow_type == "M")
                            {
                                listMockUp = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_ITEM { CHECK_LIST_ID = item.WORKFLOW_ID }, context).ToList();
                                foreach (var i in listMockUp)
                                {
                                    if (allWF == "")
                                    {
                                        oneWF = i.MOCKUP_NO;
                                        allWF = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i.MOCKUP_NO;
                                    }
                                    else
                                    {
                                        multiWF = true;
                                        allWF += "<br/>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i.MOCKUP_NO;
                                    }
                                    totalWF++;
                                }
                            }
                            else if (str_workflow_type == "A")
                            {
                                listAW = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_ITEM { ARTWORK_REQUEST_ID = item.WORKFLOW_ID }, context).ToList();
                                foreach (var i in listAW)
                                {
                                    if (allWF == "")
                                    {
                                        oneWF = i.REQUEST_ITEM_NO;
                                        allWF = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i.REQUEST_ITEM_NO;
                                    }
                                    else
                                    {
                                        multiWF = true;
                                        allWF += "<br/>" + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; " + i.REQUEST_ITEM_NO;
                                    }
                                    totalWF++;
                                }
                            }
                            userto = CNService.GetUserName(item.CURRENT_USER_ID, context);
                        }

                        dbContextTransaction.Commit();

                        var SEND_PA = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                        foreach (var item in param.data)
                        {
                            if (item.WORKFLOW_TYPE == "Artwork")
                            {
                                var listWF = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_ITEM { ARTWORK_REQUEST_ID = item.WORKFLOW_ID }, context).ToList();
                                foreach (var s in listWF)
                                {
                                    var ARTWORK_ITEM_ID = s.ARTWORK_ITEM_ID;
                                    var artworkSubId = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = ARTWORK_ITEM_ID, CURRENT_STEP_ID = SEND_PA }, context).FirstOrDefault().ARTWORK_SUB_ID;

                                    BLL.Services.EmailService.sendEmailChangeOwner(item.WORKFLOW_ID, artworkSubId, item.WORKFLOW_TYPE, item.CREATE_BY, item.CURRENT_USER_ID, "WF_CHANGE_OWNER", context, forReason);
                                }
                            }
                            else
                            {
                                var listWF = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_ITEM { CHECK_LIST_ID = item.WORKFLOW_ID }, context).ToList();
                                foreach (var s in listWF)
                                {
                                    var MOCKUP_ID = s.MOCKUP_ID;
                                    var mockupSubId = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = MOCKUP_ID, CURRENT_STEP_ID = SEND_PG }, context).FirstOrDefault().MOCKUP_SUB_ID;

                                    BLL.Services.EmailService.sendEmailChangeOwner(item.WORKFLOW_ID, mockupSubId, item.WORKFLOW_TYPE, item.CREATE_BY, item.CURRENT_USER_ID, "WF_CHANGE_OWNER", context, forReason);
                                }
                            }
                        }
                    }
                }

                res.status = "S";
                res.msg = "Total : &nbsp;" + totalWF + " Workflow <br/>";
                res.msg += "Assiged to : " + userto + "<br/>";

                if (multiWF)
                {
                    res.msg += "Workflow no : " + "<br/>";
                    res.msg += allWF;
                }
                else
                {
                    res.msg += "Workflow no : " + oneWF;
                }
            }
            catch (Exception ex)
            {
                res.status = "E";
                res.msg = CNService.GetErrorMessage(ex);
            }
            return res;
        }
    }
}
