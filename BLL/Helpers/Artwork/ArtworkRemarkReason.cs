using BLL.DocumentManagement;
using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace BLL.Helpers
{
    public class ArtworkRemarkReason
    {
        public static ART_WF_REMARK_REASON_OTHER_RESULT SaveRemarkReasonAW(ART_WF_REMARK_REASON_OTHER_REQUEST tempParam)
        {
            ART_WF_REMARK_REASON_OTHER_RESULT Results = new ART_WF_REMARK_REASON_OTHER_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var REASON_DESC = CNService.getReason(tempParam.data.REASON_ID, context);
                        if (REASON_DESC == "อื่นๆ โปรดระบุ (Others)")
                        {
                            Results.status = "E";
                            var PAstepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                            var RDstepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_RD" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                            var QCstepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_QC" }, context).FirstOrDefault().STEP_ARTWORK_ID;

                            ART_WF_REMARK_REASON_OTHER model = new ART_WF_REMARK_REASON_OTHER();
                            if (tempParam.data.ARTWORK_SUB_ID != 0)
                            {
                                model.WF_TYPE = "A";
                                model.WF_STEP = tempParam.data.WF_STEP;
                                if (tempParam.data.IS_SENDER)
                                {
                                    var newsubid = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { PARENT_ARTWORK_SUB_ID = tempParam.data.ARTWORK_SUB_ID, CURRENT_STEP_ID = tempParam.data.WF_STEP }, context).OrderByDescending(m => m.CREATE_DATE).FirstOrDefault().ARTWORK_SUB_ID;
                                    model.WF_SUB_ID = newsubid;
                                    if (tempParam.data.WF_STEP == RDstepId)
                                        model.WF_STEP = QCstepId;
                                    else
                                        model.WF_STEP = PAstepId;
                                }
                                else
                                    model.WF_SUB_ID = tempParam.data.ARTWORK_SUB_ID;
                                var check = ART_WF_REMARK_REASON_OTHER_SERVICE.GetByItem(new ART_WF_REMARK_REASON_OTHER() { WF_SUB_ID = model.WF_SUB_ID, WF_STEP = tempParam.data.WF_STEP }, context);
                                if (check.Count > 0)
                                    model.ART_WF_REMARK_REASON_OTHER_ID = check.FirstOrDefault().ART_WF_REMARK_REASON_OTHER_ID;
                            }

                            model.REMARK_REASON = tempParam.data.REMARK_REASON;
                            model.CREATE_BY = tempParam.data.CREATE_BY;
                            model.UPDATE_BY = tempParam.data.UPDATE_BY;
                            ART_WF_REMARK_REASON_OTHER_SERVICE.SaveOrUpdate(model, context);
                        }
                        dbContextTransaction.Commit();
                        Results.status = "S";
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



      


        public static ART_WF_REMARK_REASON_OTHER_RESULT SaveRemarkReasonMockup(ART_WF_REMARK_REASON_OTHER_REQUEST_LIST tempParam_)
        {
            ART_WF_REMARK_REASON_OTHER_RESULT Results = new ART_WF_REMARK_REASON_OTHER_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        foreach (var tempParam in tempParam_.data)
                        {
                            var REASON_DESC = CNService.getReason(tempParam.REASON_ID, context);
                            if (REASON_DESC == "อื่นๆ โปรดระบุ (Others)")
                            {
                                Results.status = "E";

                                ART_WF_REMARK_REASON_OTHER model = new ART_WF_REMARK_REASON_OTHER();
                                if (tempParam.WF_SUB_ID != 0)
                                {
                                    model.WF_TYPE = "M";
                                    model.WF_STEP = tempParam.WF_STEP;
                                    if (tempParam.IS_SENDER)
                                    {
                                        var newsubid = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { PARENT_MOCKUP_SUB_ID = tempParam.WF_SUB_ID, CURRENT_STEP_ID = tempParam.WF_STEP }, context).OrderByDescending(m => m.CREATE_DATE).FirstOrDefault();
                                        if (newsubid != null)
                                        {
                                            model.WF_SUB_ID = newsubid.MOCKUP_SUB_ID;
                                            model.WF_STEP = context.ART_M_STEP_MOCKUP.Where(w => w.STEP_MOCKUP_CODE == "SEND_PG").FirstOrDefault().STEP_MOCKUP_ID;
                                        }
                                    }
                                    else
                                        model.WF_SUB_ID = tempParam.WF_SUB_ID;
                                    var check = ART_WF_REMARK_REASON_OTHER_SERVICE.GetByItem(new ART_WF_REMARK_REASON_OTHER() { WF_SUB_ID = model.WF_SUB_ID, WF_STEP = tempParam.WF_STEP }, context);
                                    if (check.Count > 0)
                                        model.ART_WF_REMARK_REASON_OTHER_ID = check.FirstOrDefault().ART_WF_REMARK_REASON_OTHER_ID;
                                }

                                model.REMARK_REASON = tempParam.REMARK_REASON;
                                model.CREATE_BY = tempParam.CREATE_BY;
                                model.UPDATE_BY = tempParam.UPDATE_BY;

                                if (model.WF_SUB_ID != null)
                                {
                                    ART_WF_REMARK_REASON_OTHER_SERVICE.SaveOrUpdate(model, context);
                                }
                            }
                        }
                        dbContextTransaction.Commit();
                        Results.status = "S";
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


        //commented by aof #INC-11265
        //public static ART_WF_REMARK_REASON_OTHER_RESULT GetRemarkReason(ART_WF_REMARK_REASON_OTHER_REQUEST param)
        //{
        //    ART_WF_REMARK_REASON_OTHER_RESULT Results = new ART_WF_REMARK_REASON_OTHER_RESULT();

        //    try
        //    {

        //        using (var context = new ARTWORKEntities())
        //        {
        //            using (CNService.IsolationLevel(context))
        //            {

        //                List<ART_WF_REMARK_REASON_OTHER> q;
        //                if (param.data.ARTWORK_SUB_ID > 0)
        //                {
        //                    var WF_SUB_ID = param.data.ARTWORK_SUB_ID;
        //                    param.data.ARTWORK_SUB_ID = 0;
        //                    param.data.WF_SUB_ID = 0;
        //                    param.data.WF_TYPE = "A";

        //                    Results.data = MapperServices.ART_WF_REMARK_REASON_OTHER(ART_WF_REMARK_REASON_OTHER_SERVICE.GetByItemContain(MapperServices.ART_WF_REMARK_REASON_OTHER(param.data), context));

        //                    param.data.ARTWORK_SUB_ID = WF_SUB_ID;
        //                    Results.data = Results.data.Where(m => CNService.FindArtworkSubId(WF_SUB_ID, context).Contains(Convert.ToInt32(m.WF_SUB_ID))).ToList();


        //                }
        //                else
        //                {
        //                    var WF_SUB_ID = param.data.MOCKUP_SUB_ID;
        //                    param.data.WF_SUB_ID = 0;
        //                    param.data.WF_TYPE = "M";
        //                    param.data.MOCKUP_SUB_ID = 0;

        //                    Results.data = MapperServices.ART_WF_REMARK_REASON_OTHER(ART_WF_REMARK_REASON_OTHER_SERVICE.GetByItemContain(MapperServices.ART_WF_REMARK_REASON_OTHER(param.data), context));

        //                    param.data.MOCKUP_SUB_ID = WF_SUB_ID;
        //                    Results.data = Results.data.Where(m => CNService.FindMockupId(WF_SUB_ID, context).Contains(Convert.ToInt32(m.WF_SUB_ID))).ToList();
        //                }
        //            }
        //        }

        //        Results.status = "S";
        //    }
        //    catch (Exception ex)
        //    {
        //        Results.status = "E";
        //        Results.msg = CNService.GetErrorMessage(ex);
        //    }

        //    return Results;
        //}


        //rewrited by aof #INC-11265
        public static ART_WF_REMARK_REASON_OTHER_RESULT GetRemarkReason(ART_WF_REMARK_REASON_OTHER_REQUEST param)

        {
            ART_WF_REMARK_REASON_OTHER_RESULT Results = new ART_WF_REMARK_REASON_OTHER_RESULT();

            try
            {

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {

                        List<ART_WF_REMARK_REASON_OTHER> q;
                        List<ART_WF_REMARK_REASON_OTHER_2> q2;
                        if (param.data.ARTWORK_SUB_ID > 0)
                        {
                            var WF_SUB_ID = param.data.ARTWORK_SUB_ID;
                          
                            var list_WF_SUB_ID = CNService.FindArtworkSubId(WF_SUB_ID, context);

                            q = (from p in context.ART_WF_REMARK_REASON_OTHER
                                 where p.WF_TYPE == "A" && list_WF_SUB_ID.Contains((int)p.WF_SUB_ID) //p.WF_SUB_ID == WF_SUB_ID
                                 select p).ToList();

                            q2 = MapperServices.ART_WF_REMARK_REASON_OTHER(q);

                            if (q2 != null) {

                                foreach (ART_WF_REMARK_REASON_OTHER_2 obj in q2 )
                                {
                                    var stepAW = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_ID == obj.WF_STEP).FirstOrDefault();
                                    switch(stepAW.STEP_ARTWORK_CODE)
                                    {
                                        case "SEND_PA":
                                            break;
                                        case "SEND_PG":
                                            break;
                                        case "SEND_QC":
                                            break;
                                        case "SEND_RD":
                                            break;
                                        case "SEND_WH":
                                            break;
                                        case "SEND_PN":
                                            break;
                                        case "SEND_VN_PM":
                                            obj.REASON_ID = context.ART_WF_ARTWORK_PROCESS_VENDOR.Where(w => w.ARTWORK_SUB_ID == obj.WF_SUB_ID).FirstOrDefault().REASON_ID.GetValueOrDefault(0);
                                            break;
                                        case "SEND_VN_SL":
                                            obj.REASON_ID = context.ART_WF_ARTWORK_PROCESS_VENDOR.Where(w => w.ARTWORK_SUB_ID == obj.WF_SUB_ID).FirstOrDefault().REASON_ID.GetValueOrDefault(0);
                                            break;
                                        case "SEND_PP":
                                            break;
                                        case "SEND_VN_PO":
                                            obj.REASON_ID = context.ART_WF_ARTWORK_PROCESS_VENDOR_PO.Where(w => w.ARTWORK_SUB_ID == obj.WF_SUB_ID).FirstOrDefault().REASON_ID.GetValueOrDefault(0);
                                            break;
                                        case "SEND_MK":
                                            break;
                                        default:
                                            break;

                                    }
                                }                      
                            }

                     
                            Results.data = q2;

                        }
                        else
                        {
                            var WF_SUB_ID = param.data.MOCKUP_SUB_ID;

                            var list_WF_SUB_ID = CNService.FindMockupId(WF_SUB_ID, context);

                            q = (from p in context.ART_WF_REMARK_REASON_OTHER
                                 where p.WF_TYPE == "M" && list_WF_SUB_ID.Contains((int)p.WF_SUB_ID) //p.WF_SUB_ID == WF_SUB_ID
                                 select p).ToList();

                            q2 = MapperServices.ART_WF_REMARK_REASON_OTHER(q);

                            if (q2 != null)
                            {

                                foreach (ART_WF_REMARK_REASON_OTHER_2 obj in q2)
                                {
                                    var stepAW = context.ART_M_STEP_MOCKUP.Where(w => w.STEP_MOCKUP_ID == obj.WF_STEP).FirstOrDefault();
                                    switch (stepAW.STEP_MOCKUP_CODE)
                                    {
                                        case "SEND_PG":
                                            break;
                                        case "SEND_RD_PRI_PKG":
                                            break;
                                        case "SEND_PN_PRI_PKG":
                                            break;
                                        case "SEND_PG_SUP_SEL_VENDOR_NEED_DESIGN":
                                            break;
                                        case "SEND_WH_TEST_PACK":
                                            break;
                                        case "SEND_APP_MATCH_BOARD":
                                            break;

                                            // for vendro
                                        case "SEND_VN_PR":
                                            obj.REASON_ID = context.ART_WF_MOCKUP_PROCESS_VENDOR.Where(w => w.MOCKUP_SUB_ID == obj.WF_SUB_ID).FirstOrDefault().REASON_ID.GetValueOrDefault(0);
                                            break;
                                        case "SEND_VN_RS":
                                            obj.REASON_ID = context.ART_WF_MOCKUP_PROCESS_VENDOR.Where(w => w.MOCKUP_SUB_ID == obj.WF_SUB_ID).FirstOrDefault().REASON_ID.GetValueOrDefault(0);
                                            break;
                                        case "SEND_VN_MB":
                                            obj.REASON_ID = context.ART_WF_MOCKUP_PROCESS_VENDOR.Where(w => w.MOCKUP_SUB_ID == obj.WF_SUB_ID).FirstOrDefault().REASON_ID.GetValueOrDefault(0);
                                            break;
                                        case "SEND_VN_DL":
                                            obj.REASON_ID = context.ART_WF_MOCKUP_PROCESS_VENDOR.Where(w => w.MOCKUP_SUB_ID == obj.WF_SUB_ID).FirstOrDefault().REASON_ID.GetValueOrDefault(0);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }


                            Results.data = q2;


                        }
                    }
                }

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