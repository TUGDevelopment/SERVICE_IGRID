using AutoMapper;
using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class MKByQCHelper
    {


        //ticket 445558 by aof writed GetMKSubID
        public static int? GetMKSubIDFormProcess( int ARTWORK_REQUEST_ID,int ARTWORK_ITEM_ID, ARTWORKEntities context)
        {
           
            var artwork_req = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_ID = ARTWORK_REQUEST_ID }, context).FirstOrDefault();
            int? MKSubID = 0;
            if (artwork_req != null)
            {
                MKSubID = artwork_req.CREATOR_ID;

                //  if (artwork_req.TYPE_OF_ARTWORK == "REPEAT")  //461704 by aof 
                if (!CNService.IsMarketingCreatedArtworkRequest(artwork_req,context)) //461704 by aof 
                {

                    var stepSendMK = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_MK" || w.STEP_ARTWORK_CODE == "SEND_BACK_MK").Select(s => s.STEP_ARTWORK_ID).ToList();

                    if (stepSendMK != null)
                    {
                        var listProcess_SendBack = context.ART_WF_ARTWORK_PROCESS.Where(w => w.ARTWORK_REQUEST_ID == artwork_req.ARTWORK_REQUEST_ID
                                                   && w.CURRENT_STEP_ID != null
                                                   && string.IsNullOrEmpty(w.IS_TERMINATE)
                                                   // && stepSendMK.Contains(w.CURRENT_STEP_ID)
                                                    && w.ARTWORK_ITEM_ID == ARTWORK_ITEM_ID).Where(w=> stepSendMK.Contains(w.CURRENT_STEP_ID.Value)).OrderByDescending(w => w.CREATE_DATE);

                       // listProcess_SendBack = listProcess_SendBack.Where(w => stepSendMK.Contains(w.CURRENT_STEP_ID.GetValueOrDefault(-1))).OrderByDescending(o=>o.CREATE_BY);



                        if (listProcess_SendBack != null)
                        {
                            
                            foreach (ART_WF_ARTWORK_PROCESS process in listProcess_SendBack)
                            {
                                if (CNService.IsMarketing(Convert.ToInt32(process.CURRENT_USER_ID), context) || CNService.IsRoleMK(Convert.ToInt32(process.CURRENT_USER_ID), context))
                                {
                                    MKSubID = process.CURRENT_USER_ID;
                                    break;
                                }
                            }
                           
                        }

                        //if (cntProcess_SendBackMK == 0)
                        //{
                        //    msg = MessageHelper.GetMessage("MSG_036", context);
                        //}
                    }
                }

            }
  
            return MKSubID;
        }
        //ticket 445558 by  by aof writed GetMKSubID


        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT SaveMKbyQC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT Results = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        ART_WF_ARTWORK_PROCESS_RESULT processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
                        if (CNService.GetMarketingMailToArtworkRequest(param.data.ARTWORK_REQUEST_ID, context))
                        {
                            //ticket 150109 by voravut
                            var requestForm = context.ART_WF_ARTWORK_PROCESS.Where(r => r.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).FirstOrDefault();
                            var checkcus = (from c in context.ART_WF_ARTWORK_PROCESS_CUSTOMER
                                            where c.ARTWORK_REQUEST_ID == param.data.ARTWORK_REQUEST_ID && c.ARTWORK_SUB_ID == requestForm.PARENT_ARTWORK_SUB_ID
                                            && (c.DECISION__ADJUST == "0" || c.DECISION__ADJUST == "1")
                                            select c).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();
                            
                            var MKData = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC(param.data);
                            var check = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                            if (check.Count > 0)
                                MKData.ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID = check.FirstOrDefault().ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID;
                            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.SaveOrUpdate(MKData, context);

                            if (param.data.APPROVE == "1")
                            { // QC Approve
                                if (checkcus == null)
                                {
                                    // send customer request ref
                                    var customerList = context.ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER.Where(c => c.ARTWORK_REQUEST_ID == requestForm.ARTWORK_REQUEST_ID).ToList();
                                    string msg = ArtworkProcessHelper.checkDupWF(param.data.PROCESS, context);
                                    if (msg != "")
                                    {
                                        Results.status = "E";
                                        Results.msg = msg;
                                        return Results;
                                    }

                                    processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
                                    processResults.data = new List<ART_WF_ARTWORK_PROCESS_2>();

                                    if (param.data.PROCESS != null)
                                    {
                                        if (customerList != null && customerList.Count > 0)
                                        {
                                            foreach (ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER iCustomer in customerList)
                                            {
                                                ART_WF_ARTWORK_PROCESS_2 process = new ART_WF_ARTWORK_PROCESS_2();

                                                process = new ART_WF_ARTWORK_PROCESS_2();
                                                process = param.data.PROCESS;
                                                process.CURRENT_USER_ID = iCustomer.CUSTOMER_USER_ID;
                                                process.CURRENT_STEP_ID = (from c in context.ART_M_STEP_ARTWORK
                                                                           where c.STEP_ARTWORK_CODE == "SEND_CUS_REQ_REF"
                                                                           select c.STEP_ARTWORK_ID).FirstOrDefault();

                                                var customerUser = context.ART_M_USER_CUSTOMER.Where(c => c.USER_ID == iCustomer.CUSTOMER_USER_ID).FirstOrDefault();
                                                if (customerUser != null)
                                                {
                                                    process.CURRENT_CUSTOMER_ID = customerUser.CUSTOMER_ID;
                                                }

                                                //_processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                                                var temp = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                                                foreach (var itemTemp in temp.data)
                                                {
                                                    processResults.data.Add(itemTemp);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //_processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                                        }
                                        var dataCusbyPA = context.Database.SqlQuery<ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA>("spGetcustomer_by_pa @subid",
                                                new SqlParameter("@subid", param.data.ARTWORK_SUB_ID)).FirstOrDefault();
                                        foreach (var item2 in processResults.data)
                                        {

                                            ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA CustomerData = new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA();
                                            //CustomerData.ARTWORK_SUB_ID = param.data.ARTWORK_REQUEST_ID;
                                            //var config = new MapperConfiguration(cfg => cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2, ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA>());
                                            //var mapper = config.CreateMapper();
                                            //var ud = param.data;
                                            //ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC qc = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC();
                                            //var pc = context.ART_WF_ARTWORK_PROCESS.Where(r => r.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).FirstOrDefault();
                                            //var dataCusbyPA = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = Convert.ToInt32(pc.PARENT_ARTWORK_SUB_ID) }, context).FirstOrDefault();
                                            //CustomerData = MapperServices.ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA(dataCusbyPA);
                                            //CustomerData = mapper.Map<ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA>(dataCusbyPA);                 
                                            CustomerData = dataCusbyPA;
                                            CustomerData.SEND_TO_CUSTOMER_TYPE = "REQ_CUS_REQ_REF";
                                            
                                            //CustomerData = MapperServices.ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA(param.data);
                                            CustomerData.ARTWORK_SUB_ID = item2.ARTWORK_SUB_ID;
                                            ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.SaveNoLog(CustomerData, context);
                                        }
                                        if (param.data.ENDTASKFORM)
                                        {
                                            ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                                        }
                                        dbContextTransaction.Commit();

                                        foreach (var process in processResults.data)
                                            EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO_CUSTOMER", context);

                                        Results.status = "S";
                                        Results.msg = MessageHelper.GetMessage("MSG_001", context);
                                    }
                                }
                                else
                                {
                                    //send qc verify
                                    if (param.data.ENDTASKFORM)
                                    {
                                        ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                                    }
                                    dbContextTransaction.Commit();
                                    Results.status = "S";
                                    Results.msg = MessageHelper.GetMessage("MSG_001", context);
                                }
                                ////end approve
                            }
                            else
                            {
                                // send customer review
                                //var requestForm = context.ART_WF_ARTWORK_PROCESS.Where(r => r.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).FirstOrDefault();
                                //var checkcus = (from c in context.ART_WF_ARTWORK_PROCESS_CUSTOMER
                                //                where c.ARTWORK_REQUEST_ID == param.data.ARTWORK_REQUEST_ID && c.ARTWORK_SUB_ID == requestForm.ARTWORK_REQUEST_ID
                                //                && (c.DECISION__ADJUST == "0" || c.DECISION__ADJUST == "1")
                                //                select c).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();
                                if (checkcus == null)
                                {
                                    //var requestForm = context.ART_WF_ARTWORK_PROCESS.Where(r => r.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).FirstOrDefault();
                                    var customerList = context.ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER.Where(c => c.ARTWORK_REQUEST_ID == requestForm.ARTWORK_REQUEST_ID).ToList();
                                    string msg = ArtworkProcessHelper.checkDupWF(param.data.PROCESS, context);
                                    if (msg != "")
                                    {
                                        Results.status = "E";
                                        Results.msg = msg;
                                        return Results;
                                    }

                                    processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
                                    processResults.data = new List<ART_WF_ARTWORK_PROCESS_2>();

                                    if (param.data.PROCESS != null)
                                    {
                                        if (customerList != null && customerList.Count > 0)
                                        {
                                            foreach (ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER iCustomer in customerList)
                                            {
                                                ART_WF_ARTWORK_PROCESS_2 process = new ART_WF_ARTWORK_PROCESS_2();

                                                process = new ART_WF_ARTWORK_PROCESS_2();
                                                process = param.data.PROCESS;
                                                process.CURRENT_USER_ID = iCustomer.CUSTOMER_USER_ID;
                                                process.CURRENT_STEP_ID = (from c in context.ART_M_STEP_ARTWORK
                                                                           where c.STEP_ARTWORK_CODE == "SEND_CUS_REVIEW"
                                                                           select c.STEP_ARTWORK_ID).FirstOrDefault();

                                                var customerUser = context.ART_M_USER_CUSTOMER.Where(c => c.USER_ID == iCustomer.CUSTOMER_USER_ID).FirstOrDefault();
                                                if (customerUser != null)
                                                {
                                                    process.CURRENT_CUSTOMER_ID = customerUser.CUSTOMER_ID;
                                                }

                                                //_processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                                                var temp = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                                                foreach (var itemTemp in temp.data)
                                                {
                                                    processResults.data.Add(itemTemp);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //_processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                                        }
                                        var dataCusbyPA = context.Database.SqlQuery<ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA>("spGetcustomer_by_pa @subid",
                                                new SqlParameter("@subid", param.data.ARTWORK_SUB_ID)).FirstOrDefault();
                                        foreach (var item2 in processResults.data)
                                        {
                                            ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA CustomerData = new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA();
                                            //CustomerData.ARTWORK_SUB_ID = param.data.ARTWORK_REQUEST_ID;
                                            //var config = new MapperConfiguration(cfg => cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2, ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA>());
                                            //var mapper = config.CreateMapper();
                                            //var ud = param.data;

                                            //ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC qc = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC();
                                            //CustomerData = mapper.Map<ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA>(dataCusbyPA);
                                            CustomerData = dataCusbyPA;
                                            CustomerData.SEND_TO_CUSTOMER_TYPE = "REQ_CUS_REVIEW";
                                            //CustomerData = MapperServices.ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA(param.data);
                                            CustomerData.ARTWORK_SUB_ID = item2.ARTWORK_SUB_ID;
                                            ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.SaveNoLog(CustomerData, context);
                                        }
                                        if (param.data.ENDTASKFORM)
                                        {
                                            ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                                        }
                                        dbContextTransaction.Commit();
                                        foreach (var process in processResults.data)
                                            EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO_CUSTOMER", context);

                                        Results.status = "S";
                                        Results.msg = MessageHelper.GetMessage("MSG_001", context);
                                    }
                                }
                                else
                                {
                                    
                                        //send qc verify
                                    if (param.data.ENDTASKFORM)
                                    {
                                        ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                                    }
                                    dbContextTransaction.Commit();
                                    Results.status = "S";
                                    Results.msg = MessageHelper.GetMessage("MSG_001", context);
                                }
                            }
                        }
                        else
                        {

                            //ART_WF_ARTWORK_PROCESS_RESULT processResults2 = new ART_WF_ARTWORK_PROCESS_RESULT();
                            if (param.data.PROCESS != null)
                            {
                                // ticket 445558 by by aof writed GetMKSubIDFormProcess
                                // var MKSubID = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID }, context).FirstOrDefault().CREATOR_ID;
                                var MKSubID2 = GetMKSubIDFormProcess(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_ITEM_ID, context);
                                // ticket 445558 by by aof writed GetMKSubIDFormProcess
                                var checkMK = CNService.GetPositionCodeUser(MKSubID2, context);
                                var IsRoleMK = CNService.IsRoleMK(MKSubID2.Value, context);
                                if (checkMK == "MK" || IsRoleMK)
                                    param.data.PROCESS.CURRENT_USER_ID = MKSubID2;

                                processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                            }

                            var MKData = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC(param.data);

                            var check = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                            if (check.Count > 0)
                                MKData.ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID = check.FirstOrDefault().ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID;

                            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.SaveOrUpdate(MKData, context);

                            if (param.data.ENDTASKFORM)
                            {
                                ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                            }

                            Results.data = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2>();
                            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2 item = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2();
                            List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2> listItem = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2>();

                            item.ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID = MKData.ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID;
                            listItem.Add(item);

                            Results.data = listItem;

                            dbContextTransaction.Commit();

                            foreach (var process in processResults.data)
                                EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO", context);

                            Results.status = "S";
                            Results.msg = MessageHelper.GetMessage("MSG_001", context);
                            //var checkMK = CNService.GetPositionCodeUser(MKSubID, context);
                            //var IsRoleMK = CNService.IsRoleMK(MKSubID.Value, context);
                            //if (checkMK == "MK" || IsRoleMK)
                            //    param.data.PROCESS.CURRENT_USER_ID = MKSubID;

                            //processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);

                            //var MKData = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC(param.data);

                            //var check = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                            //if (check.Count > 0)
                            //    MKData.ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID = check.FirstOrDefault().ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID;

                            //ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.SaveOrUpdate(MKData, context);

                            //if (param.data.ENDTASKFORM)
                            //{
                            //    ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                            //}

                            //Results.data = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2>();
                            //ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2 item = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2();
                            //List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2> listItem = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2>();

                            //item.ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID = MKData.ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID;
                            //listItem.Add(item);

                            //Results.data = listItem;

                            //dbContextTransaction.Commit();

                            //foreach (var process in processResults.data)
                            //    EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO", context);

                            //Results.status = "S";
                            //Results.msg = MessageHelper.GetMessage("MSG_001", context);
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
       

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT SaveMKbyQC2(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT Results = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        ART_WF_ARTWORK_PROCESS_RESULT processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
                        if (param.data.PROCESS != null)
                        {
                            // ticket 445558 by by aof writed GetMKSubIDFormProcess
                            // var MKSubID = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID }, context).FirstOrDefault().CREATOR_ID;
                            var MKSubID = GetMKSubIDFormProcess(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_ITEM_ID, context);
                            // ticket 445558 by by aof writed GetMKSubIDFormProcess
                            var checkMK = CNService.GetPositionCodeUser(MKSubID, context);
                            var IsRoleMK = CNService.IsRoleMK(MKSubID.Value, context);
                            if (checkMK == "MK" || IsRoleMK)
                                param.data.PROCESS.CURRENT_USER_ID = MKSubID;

                            processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                        }

                        var MKData = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC(param.data);

                        var check = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);
                        if (check.Count > 0)
                            MKData.ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID = check.FirstOrDefault().ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID;

                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.SaveOrUpdate(MKData, context);

                        if (param.data.ENDTASKFORM)
                        {
                            ArtworkProcessHelper.EndTaskForm(param.data.ARTWORK_SUB_ID, param.data.UPDATE_BY, context);
                        }

                        Results.data = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2>();
                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2 item = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2();
                        List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2> listItem = new List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2>();

                        item.ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID = MKData.ARTWORK_PROCESS_AFTER_CUSTOMER_QC_ID;
                        listItem.Add(item);

                        Results.data = listItem;

                        dbContextTransaction.Commit();

                        foreach (var process in processResults.data)
                            EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO", context);

                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_001", context);
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
        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_RESULT GetMKbyQC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_RESULT Results = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.GetAll(context));
                        }
                        else
                        {
                            var ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            param.data.ARTWORK_SUB_ID = 0;

                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.GetByItemContain(MapperServices.ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC(param.data), context));

                            param.data.ARTWORK_SUB_ID = ARTWORK_SUB_ID;
                            Results.data = Results.data.Where(m => CNService.FindArtworkSubId(ARTWORK_SUB_ID, context).Contains(m.ARTWORK_SUB_ID)).ToList();

                        }

                        ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC p = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC();
                        List<ART_WF_ARTWORK_PROCESS> dataCus_gm1 = new List<ART_WF_ARTWORK_PROCESS>();
                        ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA dataCusbyPA_gm = new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA();
                        dataCusbyPA_gm = null;

                        Results.status = "S";

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                var dataCus1 = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                var dataCus2 = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Convert.ToInt32(dataCus1.PARENT_ARTWORK_SUB_ID) }, context).FirstOrDefault();
                                if (dataCus2 != null)
                                {
                                    var dataCusbyPA = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = Convert.ToInt32(dataCus2.PARENT_ARTWORK_SUB_ID) }, context).FirstOrDefault();

                                    if (dataCusbyPA == null)
                                    {
                                        dataCus_gm1 = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = param.data.ARTWORK_ITEM_ID, ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID, CURRENT_STEP_ID = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REVIEW" }, context).FirstOrDefault().STEP_ARTWORK_ID }, context).OrderByDescending(o => o.UPDATE_DATE).ToList();
                                        if (dataCus_gm1 != null)
                                        {
                                            var awSubPA = 0;
                                            for (int x = 0; x < dataCus_gm1.Count; x++)
                                            {
                                                var check = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = dataCus_gm1[x].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                                if (check != null)
                                                {
                                                    awSubPA = check.ARTWORK_SUB_ID;
                                                    break;
                                                }

                                            }
                                            dataCusbyPA_gm = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = awSubPA }, context).FirstOrDefault();
                                        }
                                    }

                                    if (dataCusbyPA != null)
                                    {
                                        //Comment PA
                                        if (dataCusbyPA.COMMENT_ADJUST != null)
                                            Results.data[i].COMMENT_ADJUST = dataCusbyPA.COMMENT_ADJUST;
                                        else
                                            Results.data[i].COMMENT_ADJUST = "-";
                                        if (dataCusbyPA.COMMENT_NONCOMPLIANCE != null)
                                            Results.data[i].COMMENT_NONCOMPLIANCE = dataCusbyPA.COMMENT_NONCOMPLIANCE;
                                        else
                                            Results.data[i].COMMENT_NONCOMPLIANCE = "-";
                                        if (dataCusbyPA.COMMENT_FORM_LABEL != null)
                                            Results.data[i].COMMENT_CHANGE_DETAIL = dataCusbyPA.COMMENT_FORM_LABEL;
                                        else
                                            Results.data[i].COMMENT_CHANGE_DETAIL = "-";


                                        if (dataCusbyPA.IS_FORMLABEL != null)
                                            Results.data[i].IS_FORMLABEL = dataCusbyPA.IS_FORMLABEL;
                                        if (dataCusbyPA.IS_CHANGEDETAIL != null)
                                            Results.data[i].IS_CHANGEDETAIL = dataCusbyPA.IS_CHANGEDETAIL;
                                        if (dataCusbyPA.IS_NONCOMPLIANCE != null)
                                            Results.data[i].IS_NONCOMPLIANCE = dataCusbyPA.IS_CHANGEDETAIL;
                                        if (dataCusbyPA.IS_ADJUST != null)
                                            Results.data[i].IS_ADJUST = dataCusbyPA.IS_ADJUST;

                                        if (dataCusbyPA.NUTRITION_COMMENT != "<p><br></p>" && dataCusbyPA.NUTRITION_COMMENT != null)
                                            Results.data[i].NUTRITION_COMMENT_DISPLAY = dataCusbyPA.NUTRITION_COMMENT;
                                        else
                                            Results.data[i].NUTRITION_COMMENT_DISPLAY = "-";

                                        if (dataCusbyPA.INGREDIENTS_COMMENT != "<p><br></p>" && dataCusbyPA.INGREDIENTS_COMMENT != null)
                                            Results.data[i].INGREDIENTS_COMMENT_DISPLAY = dataCusbyPA.INGREDIENTS_COMMENT;
                                        else
                                            Results.data[i].INGREDIENTS_COMMENT_DISPLAY = "-";
                                        if (dataCusbyPA.ANALYSIS_COMMENT != "<p><br></p>" && dataCusbyPA.ANALYSIS_COMMENT != null)
                                            Results.data[i].ANALYSIS_COMMENT_DISPLAY = dataCusbyPA.ANALYSIS_COMMENT;
                                        else
                                            Results.data[i].ANALYSIS_COMMENT_DISPLAY = "-";
                                        if (dataCusbyPA.HEALTH_CLAIM_COMMENT != "<p><br></p>" && dataCusbyPA.HEALTH_CLAIM_COMMENT != null)
                                            Results.data[i].HEALTH_CLAIM_COMMENT_DISPLAY = dataCusbyPA.HEALTH_CLAIM_COMMENT;
                                        else
                                            Results.data[i].HEALTH_CLAIM_COMMENT_DISPLAY = "-";
                                        if (dataCusbyPA.NUTRIENT_CLAIM_COMMENT != "<p><br></p>" && dataCusbyPA.NUTRIENT_CLAIM_COMMENT != null)
                                            Results.data[i].NUTRIENT_CLAIM_COMMENT_DISPLAY = dataCusbyPA.NUTRIENT_CLAIM_COMMENT;
                                        else
                                            Results.data[i].NUTRIENT_CLAIM_COMMENT_DISPLAY = "-";
                                        if (dataCusbyPA.SPECIES_COMMENT != "<p><br></p>" && dataCusbyPA.SPECIES_COMMENT != null)
                                            Results.data[i].SPECIES_COMMENT_DISPLAY = dataCusbyPA.SPECIES_COMMENT;
                                        else
                                            Results.data[i].SPECIES_COMMENT_DISPLAY = "-";
                                        if (dataCusbyPA.CATCHING_AREA_COMMENT != "<p><br></p>" && dataCusbyPA.CATCHING_AREA_COMMENT != null)
                                            Results.data[i].CATCHING_AREA_COMMENT_DISPLAY = dataCusbyPA.CATCHING_AREA_COMMENT;
                                        else
                                            Results.data[i].CATCHING_AREA_COMMENT_DISPLAY = "-";

                                        if (dataCusbyPA.CHECK_DETAIL_COMMENT != "<p><br></p>" && dataCusbyPA.CHECK_DETAIL_COMMENT != null)
                                            Results.data[i].CHECK_DETAIL_COMMENT_DISPLAY = dataCusbyPA.CHECK_DETAIL_COMMENT;
                                        else
                                            Results.data[i].CHECK_DETAIL_COMMENT_DISPLAY = "-";

                                        if (dataCusbyPA.QC_COMMENT != "" && dataCusbyPA.QC_COMMENT != null)
                                            Results.data[i].QC_COMMENT = dataCusbyPA.QC_COMMENT;
                                        else
                                            Results.data[i].QC_COMMENT = "-";

                                        var SEND_TO_CUSTOMER_TYPE = dataCusbyPA.SEND_TO_CUSTOMER_TYPE;
                                        if (SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REVIEW")
                                        {
                                            if (Results.data[i].APPROVE == "1")
                                                Results.data[i].APPROVE_BY_OTHER = "Approve";
                                            else if (Results.data[i].APPROVE == "0")
                                                Results.data[i].APPROVE_BY_OTHER = "Not Approve";

                                            if (Results.data[i].ACTION_CODE == "SUBMIT")
                                                Results.data[i].ACTION_NAME_BY_OTHER = "Submit";
                                            else if (Results.data[i].ACTION_CODE == "SEND_BACK")
                                                Results.data[i].ACTION_NAME_BY_OTHER = "Send back";


                                            Results.data[i].CREATE_DATE_BY_OTHER = Results.data[i].CREATE_DATE;


                                            var processFormCustomer = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = dataCusbyPA.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                            if (processFormCustomer != null)
                                            {
                                                Results.data[i].COMMENT_FORMLABEL_DISPLAY = processFormCustomer.COMMENT_CHANGE_DETAIL;
                                                Results.data[i].COMMENT_NONCOMPLIANCE_DISPLAY = processFormCustomer.COMMENT_NONCOMPLIANCE;
                                                Results.data[i].COMMENT_ADJUST_DISPLAY = processFormCustomer.COMMENT_ADJUST;

                                                if (processFormCustomer.DECISION__CHANGE_DETAIL == "1")
                                                    Results.data[i].DECISION_FORMLABEL_DISPLAY = "Confirm to change";
                                                else if (processFormCustomer.DECISION__CHANGE_DETAIL == "0")
                                                    Results.data[i].DECISION_FORMLABEL_DISPLAY = "Do not change";
                                                else
                                                    Results.data[i].DECISION_FORMLABEL_DISPLAY = "-";
                                                if (processFormCustomer.DECISION__NONCOMPLIANCE == "1")
                                                    Results.data[i].DECISION_NONCOMPLIANCE_DISPLAY = "Confirm to change";
                                                else if (processFormCustomer.DECISION__NONCOMPLIANCE == "0")
                                                    Results.data[i].DECISION_NONCOMPLIANCE_DISPLAY = "Do not change";
                                                else
                                                    Results.data[i].DECISION_NONCOMPLIANCE_DISPLAY = "-";
                                                if (processFormCustomer.DECISION__ADJUST == "1")
                                                    Results.data[i].DECISION_ADJUST_DISPLAY = "Confirm to change";
                                                else if (processFormCustomer.DECISION__ADJUST == "0")
                                                    Results.data[i].DECISION_ADJUST_DISPLAY = "Do not change";
                                                else
                                                    Results.data[i].DECISION_ADJUST_DISPLAY = "-";
                                            }

                                            var processForm = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = dataCus1.ARTWORK_SUB_ID }, context).FirstOrDefault();

                                            if (processForm != null)
                                            {
                                                if (processForm.REASON_ID != null)
                                                    Results.data[i].REASON_BY_PA = CNService.getReason(processForm.REASON_ID, context);
                                                if (processForm.REMARK != null)
                                                    Results.data[i].COMMENT_BY_PA = processForm.REMARK;
                                                Results.data[i].CREATE_DATE = processForm.CREATE_DATE;

                                            }

                                            var processFormQC = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC() { ARTWORK_SUB_ID = Convert.ToInt32(ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault().PARENT_ARTWORK_SUB_ID) }, context).FirstOrDefault();
                                            if (processFormQC != null)
                                            {
                                                if (processFormQC.APPROVE != null)
                                                {
                                                    ART_SYS_ACTION act = new ART_SYS_ACTION();
                                                    act.ACTION_CODE = processFormQC.ACTION_CODE;

                                                    Results.data[i].ACTION_NAME = ART_SYS_ACTION_SERVICE.GetByItemContain(act, context).FirstOrDefault().ACTION_NAME;
                                                    if (processFormQC.APPROVE == "1")
                                                        Results.data[i].APPROVE_BY_QC = "Approve";
                                                    else if (processFormQC.APPROVE == "0")
                                                        Results.data[i].APPROVE_BY_QC = "Not Approve";

                                                }
                                            }

                                            var PreviousStep = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault().PARENT_ARTWORK_SUB_ID;
                                            if (PreviousStep != null)
                                            {
                                                var CHK_REQ_TYPE = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Convert.ToInt32(PreviousStep) }, context).FirstOrDefault();
                                                if (CHK_REQ_TYPE.CURRENT_STEP_ID == ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REQ_REF" }, context).FirstOrDefault().STEP_ARTWORK_ID)
                                                {
                                                    Results.data[i].SEND_TO_CUSTOMER_TYPE = "REQ_CUS_REQ_REF";
                                                }
                                            }

                                            Results.data[i].PREV_STEP_DISPLAY = "QC";
                                            Results.data[i].PREV_STEP_SHOW = "QC review customer's decision";

                                        }
                                        //else
                                        //{
                                        //    Results.data[i].ACTION_CODE = "delete";
                                        //}
                                    }
                                }

                                if (dataCusbyPA_gm != null)
                                {
                                    //Comment PA
                                    if (dataCusbyPA_gm.COMMENT_ADJUST != null)
                                        Results.data[i].COMMENT_ADJUST = dataCusbyPA_gm.COMMENT_ADJUST;
                                    else
                                        Results.data[i].COMMENT_ADJUST = "-";
                                    if (dataCusbyPA_gm.COMMENT_NONCOMPLIANCE != null)
                                        Results.data[i].COMMENT_NONCOMPLIANCE = dataCusbyPA_gm.COMMENT_NONCOMPLIANCE;
                                    else
                                        Results.data[i].COMMENT_NONCOMPLIANCE = "-";
                                    if (dataCusbyPA_gm.COMMENT_FORM_LABEL != null)
                                        Results.data[i].COMMENT_CHANGE_DETAIL = dataCusbyPA_gm.COMMENT_FORM_LABEL;
                                    else
                                        Results.data[i].COMMENT_CHANGE_DETAIL = "-";


                                    if (dataCusbyPA_gm.IS_FORMLABEL != null)
                                        Results.data[i].IS_FORMLABEL = dataCusbyPA_gm.IS_FORMLABEL;
                                    if (dataCusbyPA_gm.IS_CHANGEDETAIL != null)
                                        Results.data[i].IS_CHANGEDETAIL = dataCusbyPA_gm.IS_CHANGEDETAIL;
                                    if (dataCusbyPA_gm.IS_NONCOMPLIANCE != null)
                                        Results.data[i].IS_NONCOMPLIANCE = dataCusbyPA_gm.IS_CHANGEDETAIL;
                                    if (dataCusbyPA_gm.IS_ADJUST != null)
                                        Results.data[i].IS_ADJUST = dataCusbyPA_gm.IS_ADJUST;

                                    if (dataCusbyPA_gm.NUTRITION_COMMENT != "<p><br></p>" && dataCusbyPA_gm.NUTRITION_COMMENT != null)
                                        Results.data[i].NUTRITION_COMMENT_DISPLAY = dataCusbyPA_gm.NUTRITION_COMMENT;
                                    else
                                        Results.data[i].NUTRITION_COMMENT_DISPLAY = "-";

                                    if (dataCusbyPA_gm.INGREDIENTS_COMMENT != "<p><br></p>" && dataCusbyPA_gm.INGREDIENTS_COMMENT != null)
                                        Results.data[i].INGREDIENTS_COMMENT_DISPLAY = dataCusbyPA_gm.INGREDIENTS_COMMENT;
                                    else
                                        Results.data[i].INGREDIENTS_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA_gm.ANALYSIS_COMMENT != "<p><br></p>" && dataCusbyPA_gm.ANALYSIS_COMMENT != null)
                                        Results.data[i].ANALYSIS_COMMENT_DISPLAY = dataCusbyPA_gm.ANALYSIS_COMMENT;
                                    else
                                        Results.data[i].ANALYSIS_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA_gm.HEALTH_CLAIM_COMMENT != "<p><br></p>" && dataCusbyPA_gm.HEALTH_CLAIM_COMMENT != null)
                                        Results.data[i].HEALTH_CLAIM_COMMENT_DISPLAY = dataCusbyPA_gm.HEALTH_CLAIM_COMMENT;
                                    else
                                        Results.data[i].HEALTH_CLAIM_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA_gm.NUTRIENT_CLAIM_COMMENT != "<p><br></p>" && dataCusbyPA_gm.NUTRIENT_CLAIM_COMMENT != null)
                                        Results.data[i].NUTRIENT_CLAIM_COMMENT_DISPLAY = dataCusbyPA_gm.NUTRIENT_CLAIM_COMMENT;
                                    else
                                        Results.data[i].NUTRIENT_CLAIM_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA_gm.SPECIES_COMMENT != "<p><br></p>" && dataCusbyPA_gm.SPECIES_COMMENT != null)
                                        Results.data[i].SPECIES_COMMENT_DISPLAY = dataCusbyPA_gm.SPECIES_COMMENT;
                                    else
                                        Results.data[i].SPECIES_COMMENT_DISPLAY = "-";
                                    if (dataCusbyPA_gm.CATCHING_AREA_COMMENT != "<p><br></p>" && dataCusbyPA_gm.CATCHING_AREA_COMMENT != null)
                                        Results.data[i].CATCHING_AREA_COMMENT_DISPLAY = dataCusbyPA_gm.CATCHING_AREA_COMMENT;
                                    else
                                        Results.data[i].CATCHING_AREA_COMMENT_DISPLAY = "-";

                                    if (dataCusbyPA_gm.CHECK_DETAIL_COMMENT != "<p><br></p>" && dataCusbyPA_gm.CHECK_DETAIL_COMMENT != null)
                                        Results.data[i].CHECK_DETAIL_COMMENT_DISPLAY = dataCusbyPA_gm.CHECK_DETAIL_COMMENT;
                                    else
                                        Results.data[i].CHECK_DETAIL_COMMENT_DISPLAY = "-";

                                    if (dataCusbyPA_gm.QC_COMMENT != "" && dataCusbyPA_gm.QC_COMMENT != null)
                                        Results.data[i].QC_COMMENT = dataCusbyPA_gm.QC_COMMENT;
                                    else
                                        Results.data[i].QC_COMMENT = "-";

                                    var SEND_TO_CUSTOMER_TYPE = dataCusbyPA_gm.SEND_TO_CUSTOMER_TYPE;
                                    if (SEND_TO_CUSTOMER_TYPE == "REQ_CUS_REVIEW")
                                    {
                                        if (Results.data[i].APPROVE == "1")
                                            Results.data[i].APPROVE_BY_OTHER = "Approve";
                                        else if (Results.data[i].APPROVE == "0")
                                            Results.data[i].APPROVE_BY_OTHER = "Not Approve";
                                        else
                                            Results.data[i].APPROVE_BY_OTHER = "-";

                                        if (Results.data[i].ACTION_CODE == "SUBMIT")
                                            Results.data[i].ACTION_NAME_BY_OTHER = "Submit";
                                        else if (Results.data[i].ACTION_CODE == "SEND_BACK")
                                            Results.data[i].ACTION_NAME_BY_OTHER = "Send back";



                                        Results.data[i].CREATE_DATE_BY_OTHER = Results.data[i].CREATE_DATE;


                                        var processFormCustomer = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = dataCusbyPA_gm.ARTWORK_SUB_ID }, context).FirstOrDefault();
                                        if (processFormCustomer != null)
                                        {
                                            Results.data[i].COMMENT_FORMLABEL_DISPLAY = processFormCustomer.COMMENT_CHANGE_DETAIL;
                                            Results.data[i].COMMENT_NONCOMPLIANCE_DISPLAY = processFormCustomer.COMMENT_NONCOMPLIANCE;
                                            Results.data[i].COMMENT_ADJUST_DISPLAY = processFormCustomer.COMMENT_ADJUST;

                                            if (processFormCustomer.DECISION__CHANGE_DETAIL == "1")
                                                Results.data[i].DECISION_FORMLABEL_DISPLAY = "Confirm to change";
                                            else if (processFormCustomer.DECISION__CHANGE_DETAIL == "0")
                                                Results.data[i].DECISION_FORMLABEL_DISPLAY = "Do not change";
                                            else
                                                Results.data[i].DECISION_FORMLABEL_DISPLAY = "-";
                                            if (processFormCustomer.DECISION__NONCOMPLIANCE == "1")
                                                Results.data[i].DECISION_NONCOMPLIANCE_DISPLAY = "Confirm to change";
                                            else if (processFormCustomer.DECISION__NONCOMPLIANCE == "0")
                                                Results.data[i].DECISION_NONCOMPLIANCE_DISPLAY = "Do not change";
                                            else
                                                Results.data[i].DECISION_NONCOMPLIANCE_DISPLAY = "-";
                                            if (processFormCustomer.DECISION__ADJUST == "1")
                                                Results.data[i].DECISION_ADJUST_DISPLAY = "Confirm to change";
                                            else if (processFormCustomer.DECISION__ADJUST == "0")
                                                Results.data[i].DECISION_ADJUST_DISPLAY = "Do not change";
                                            else
                                                Results.data[i].DECISION_ADJUST_DISPLAY = "-";
                                        }

                                        var processForm = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = dataCus1.ARTWORK_SUB_ID }, context).FirstOrDefault();

                                        if (processForm != null)
                                        {
                                            if (processForm.REASON_ID != null)
                                            {
                                                Results.data[i].REASON_BY_PA = CNService.getReason(processForm.REASON_ID, context);
                                                Results.data[i].COMMENT_BY_PA = processForm.REMARK;
                                                Results.data[i].CREATE_DATE = processForm.CREATE_DATE;
                                            }
                                        }

                                        var processFormQC = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC() { ARTWORK_SUB_ID = Convert.ToInt32(ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault().PARENT_ARTWORK_SUB_ID) }, context).FirstOrDefault();
                                        if (processFormQC != null)
                                        {
                                            if (processFormQC.ACTION_CODE != null)
                                            {
                                                if (processFormQC.ACTION_CODE == "APPROVE")
                                                {
                                                    Results.data[i].ACTION_NAME = "Approve";
                                                    Results.data[i].APPROVE_BY_QC = "-";
                                                }
                                                else if (processFormQC.ACTION_CODE == "NOTAPPROVE")
                                                {
                                                    Results.data[i].ACTION_NAME = "Not approve";
                                                    Results.data[i].APPROVE_BY_QC = "-";
                                                }
                                                else if (processFormQC.ACTION_CODE == "SEND_BACK")
                                                {
                                                    Results.data[i].ACTION_NAME = "Send back";
                                                    Results.data[i].APPROVE_BY_QC = "-";
                                                }
                                                else if (processFormQC.ACTION_CODE == "SUBMIT")
                                                {
                                                    Results.data[i].ACTION_NAME = "Submit";
                                                    Results.data[i].APPROVE_BY_QC = "-";
                                                }

                                                Results.data[i].CREATE_DATE = processFormQC.CREATE_DATE;
                                                Results.data[i].COMMENT_BY_PA = processFormQC.COMMENT;
                                            }
                                            Results.data[i].PREV_STEP_DISPLAY = "QC";
                                            Results.data[i].PREV_STEP_SHOW = "QC review customer's decision";

                                        }


                                        var processFormGMQC = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC() { ARTWORK_SUB_ID = Convert.ToInt32(ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault().PARENT_ARTWORK_SUB_ID) }, context).FirstOrDefault();
                                        if (processFormGMQC != null)
                                        {
                                            if (processFormGMQC.ACTION_CODE != null)
                                            {
                                                if (processFormGMQC.ACTION_CODE == "APPROVE")
                                                {
                                                    Results.data[i].ACTION_NAME = "Approve";
                                                    Results.data[i].APPROVE_BY_QC = "-";
                                                }
                                                else if (processFormGMQC.ACTION_CODE == "NOTAPPROVE")
                                                {
                                                    Results.data[i].ACTION_NAME = "Not approve";
                                                    Results.data[i].APPROVE_BY_QC = "-";
                                                }
                                                else if (processFormGMQC.ACTION_CODE == "SEND_BACK")
                                                {
                                                    Results.data[i].ACTION_NAME = "Send back";
                                                    Results.data[i].APPROVE_BY_QC = "-";
                                                }
                                                else if (processFormGMQC.ACTION_CODE == "SUBMIT")
                                                {
                                                    Results.data[i].ACTION_NAME = "Submit";
                                                    Results.data[i].APPROVE_BY_QC = "-";
                                                }

                                                Results.data[i].CREATE_DATE = processFormGMQC.CREATE_DATE;
                                                Results.data[i].COMMENT_BY_PA = processFormGMQC.COMMENT;
                                            }
                                            Results.data[i].PREV_STEP_DISPLAY = "GM QC";
                                            Results.data[i].PREV_STEP_SHOW = "GM QC review customer's reference letter";

                                        }

                                        var processFormGMMK = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK() { ARTWORK_SUB_ID = Convert.ToInt32(ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault().PARENT_ARTWORK_SUB_ID) }, context).FirstOrDefault();
                                        if (processFormGMMK != null)
                                        {
                                            if (processFormGMMK.ACTION_CODE != null)
                                            {
                                                if (processFormGMMK.ACTION_CODE == "APPROVE")
                                                {
                                                    Results.data[i].ACTION_NAME = "Approve";
                                                    Results.data[i].APPROVE_BY_QC = "-";
                                                }
                                                else if (processFormGMMK.ACTION_CODE == "NOTAPPROVE")
                                                {
                                                    Results.data[i].ACTION_NAME = "Not approve";
                                                    Results.data[i].APPROVE_BY_QC = "-";
                                                }
                                                else if (processFormGMMK.ACTION_CODE == "SEND_BACK")
                                                {
                                                    Results.data[i].ACTION_NAME = "Send back";
                                                    Results.data[i].APPROVE_BY_QC = "-";
                                                }
                                                else if (processFormGMMK.ACTION_CODE == "SUBMIT")
                                                {
                                                    Results.data[i].ACTION_NAME = "Submit";
                                                    Results.data[i].APPROVE_BY_QC = "-";
                                                }
                                                Results.data[i].CREATE_DATE = processFormGMMK.CREATE_DATE;
                                                Results.data[i].COMMENT_BY_PA = processFormGMMK.COMMENT;
                                            }
                                            Results.data[i].PREV_STEP_DISPLAY = "GM MK";
                                            Results.data[i].PREV_STEP_SHOW = "GM MK review customer's reference letter";

                                        }

                                        var PreviousStep = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Results.data[i].ARTWORK_SUB_ID }, context).FirstOrDefault().PARENT_ARTWORK_SUB_ID;
                                        if (PreviousStep != null)
                                        {
                                            var CHK_REQ_TYPE = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Convert.ToInt32(PreviousStep) }, context).FirstOrDefault();
                                            if (CHK_REQ_TYPE.CURRENT_STEP_ID == ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REQ_REF" }, context).FirstOrDefault().STEP_ARTWORK_ID)
                                            {
                                                Results.data[i].SEND_TO_CUSTOMER_TYPE = "REQ_CUS_REQ_REF";
                                            }
                                        }


                                    }
                                    //else
                                    //{
                                    //    Results.data[i].ACTION_CODE = "delete";
                                    //}
                                }
                                dataCusbyPA_gm = null;
                            }
                            //Results.data = Results.data.Where(m => m.ACTION_CODE != "delete").ToList();
                            Results.data = Results.data.OrderByDescending(o => o.UPDATE_DATE).ToList();
                        }

                        var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_MK_VERIFY" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                        var list = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID, CURRENT_STEP_ID = stepId }, context);
                        list = list.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                        var listCheck = list.Where(q => !Results.data.Any(q2 => q2.ARTWORK_SUB_ID == q.ARTWORK_SUB_ID)).FirstOrDefault();
                        if (listCheck != null)
                        {
                            dataCus_gm1 = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = listCheck.ARTWORK_ITEM_ID, ARTWORK_REQUEST_ID = listCheck.ARTWORK_REQUEST_ID, CURRENT_STEP_ID = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REVIEW" }, context).FirstOrDefault().STEP_ARTWORK_ID }, context).OrderByDescending(o => o.UPDATE_DATE).ToList();
                            if (dataCus_gm1 != null)
                            {
                                var awSubPA = 0;
                                for (int x = 0; x < dataCus_gm1.Count; x++)
                                {
                                    var check = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = dataCus_gm1[x].ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (check != null)
                                    {
                                        awSubPA = check.ARTWORK_SUB_ID;
                                        break;
                                    }

                                }

                                var result = ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA() { ARTWORK_SUB_ID = Convert.ToInt32(awSubPA) }, context).FirstOrDefault();
                                if (result != null)
                                {
                                    ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2 item = new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2();

                                    var processFormQC = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC() { ARTWORK_SUB_ID = Convert.ToInt32(listCheck.PARENT_ARTWORK_SUB_ID) }, context).FirstOrDefault();

                                    if (processFormQC != null)
                                    {
                                        if (processFormQC.APPROVE != null)
                                        {
                                            if (processFormQC.APPROVE == "1")
                                                item.APPROVE_BY_QC = "Approve";
                                            else if (processFormQC.APPROVE == "0")
                                                item.APPROVE_BY_QC = "Not Approve";
                                        }
                                        if (processFormQC.ACTION_CODE != null)
                                        {
                                            if (processFormQC.ACTION_CODE == "APPROVE")
                                                item.ACTION_NAME = "Approve";
                                            else if (processFormQC.ACTION_CODE == "NOTAPPROVE")
                                                item.ACTION_NAME = "Not Approve";
                                            else if (processFormQC.ACTION_CODE == "SEND_BACK")
                                                item.ACTION_NAME = "Send back";
                                            else if (processFormQC.ACTION_CODE == "SUBMIT")
                                                item.ACTION_NAME = "Submit";
                                        }
                                        if (processFormQC.COMMENT != null)
                                            item.COMMENT_BY_PA = processFormQC.COMMENT;
                                        item.CREATE_DATE = processFormQC.CREATE_DATE;
                                        item.PREV_STEP_DISPLAY = "QC";
                                        item.PREV_STEP_SHOW = "QC review customer's decision";
                                    }

                                    var processFormGMQC = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC() { ARTWORK_SUB_ID = Convert.ToInt32(listCheck.PARENT_ARTWORK_SUB_ID) }, context).FirstOrDefault();
                                    if (processFormGMQC != null)
                                    {
                                        if (processFormGMQC.ACTION_CODE != null)
                                        {
                                            if (processFormGMQC.ACTION_CODE == "APPROVE")
                                            {
                                                item.ACTION_NAME = "Approve";
                                                item.APPROVE_BY_QC = "-";
                                            }
                                            else if (processFormGMQC.ACTION_CODE == "NOTAPPROVE")
                                            {
                                                item.ACTION_NAME = "Not approve";
                                                item.APPROVE_BY_QC = "-";
                                            }
                                            else if (processFormGMQC.ACTION_CODE == "SEND_BACK")
                                            {
                                                item.ACTION_NAME = "Send back";
                                                item.APPROVE_BY_QC = "-";
                                            }
                                            if (processFormGMQC.COMMENT != null)
                                                item.COMMENT_BY_PA = processFormGMQC.COMMENT;
                                            item.CREATE_DATE = processFormGMQC.CREATE_DATE;
                                            item.PREV_STEP_DISPLAY = "GM QC";
                                            item.PREV_STEP_SHOW = "GM QC review customer's reference letter";
                                        }

                                    }

                                    var processFormGMMK = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK() { ARTWORK_SUB_ID = Convert.ToInt32(listCheck.PARENT_ARTWORK_SUB_ID) }, context).FirstOrDefault();
                                    if (processFormGMMK != null)
                                    {
                                        if (processFormGMMK.ACTION_CODE != null)
                                        {
                                            if (processFormGMMK.ACTION_CODE == "APPROVE")
                                            {
                                                item.ACTION_NAME = "Approve";
                                                item.APPROVE_BY_QC = "-";
                                            }
                                            else if (processFormGMMK.ACTION_CODE == "NOTAPPROVE")
                                            {
                                                item.ACTION_NAME = "Not approve";
                                                item.APPROVE_BY_QC = "-";
                                            }
                                            else if (processFormGMMK.ACTION_CODE == "SEND_BACK")
                                            {
                                                item.ACTION_NAME = "Send back";
                                                item.APPROVE_BY_QC = "-";
                                            }
                                            if (processFormGMMK.COMMENT != null)
                                                item.COMMENT_BY_PA = processFormGMMK.COMMENT;
                                            item.CREATE_DATE = processFormGMMK.CREATE_DATE;
                                            item.PREV_STEP_DISPLAY = "GM MK";
                                            item.PREV_STEP_SHOW = "GM MK review customer's reference letter";
                                        }

                                    }

                                    //Comment PA
                                    item.SEND_TO_CUSTOMER_TYPE = result.SEND_TO_CUSTOMER_TYPE;

                                    if (result.COMMENT_ADJUST != null)
                                        item.COMMENT_ADJUST = result.COMMENT_ADJUST;
                                    else
                                        item.COMMENT_ADJUST = "-";
                                    if (result.COMMENT_NONCOMPLIANCE != null)
                                        item.COMMENT_NONCOMPLIANCE = result.COMMENT_NONCOMPLIANCE;
                                    else
                                        item.COMMENT_NONCOMPLIANCE = "-";
                                    if (result.COMMENT_FORM_LABEL != null)
                                        item.COMMENT_CHANGE_DETAIL = result.COMMENT_FORM_LABEL;
                                    else
                                        item.COMMENT_CHANGE_DETAIL = "-";


                                    if (result.IS_FORMLABEL != null)
                                        item.IS_FORMLABEL = result.IS_FORMLABEL;
                                    if (result.IS_CHANGEDETAIL != null)
                                        item.IS_CHANGEDETAIL = result.IS_CHANGEDETAIL;
                                    if (result.IS_NONCOMPLIANCE != null)
                                        item.IS_NONCOMPLIANCE = result.IS_CHANGEDETAIL;
                                    if (result.IS_ADJUST != null)
                                        item.IS_ADJUST = result.IS_ADJUST;

                                    if (result.NUTRITION_COMMENT != "<p><br></p>" && result.NUTRITION_COMMENT != null)
                                        item.NUTRITION_COMMENT_DISPLAY = result.NUTRITION_COMMENT;
                                    else
                                        item.NUTRITION_COMMENT_DISPLAY = "-";

                                    if (result.INGREDIENTS_COMMENT != "<p><br></p>" && result.INGREDIENTS_COMMENT != null)
                                        item.INGREDIENTS_COMMENT_DISPLAY = result.INGREDIENTS_COMMENT;
                                    else
                                        item.INGREDIENTS_COMMENT_DISPLAY = "-";
                                    if (result.ANALYSIS_COMMENT != "<p><br></p>" && result.ANALYSIS_COMMENT != null)
                                        item.ANALYSIS_COMMENT_DISPLAY = result.ANALYSIS_COMMENT;
                                    else
                                        item.ANALYSIS_COMMENT_DISPLAY = "-";
                                    if (result.HEALTH_CLAIM_COMMENT != "<p><br></p>" && result.HEALTH_CLAIM_COMMENT != null)
                                        item.HEALTH_CLAIM_COMMENT_DISPLAY = result.HEALTH_CLAIM_COMMENT;
                                    else
                                        item.HEALTH_CLAIM_COMMENT_DISPLAY = "-";
                                    if (result.NUTRIENT_CLAIM_COMMENT != "<p><br></p>" && result.NUTRIENT_CLAIM_COMMENT != null)
                                        item.NUTRIENT_CLAIM_COMMENT_DISPLAY = result.NUTRIENT_CLAIM_COMMENT;
                                    else
                                        item.NUTRIENT_CLAIM_COMMENT_DISPLAY = "-";
                                    if (result.SPECIES_COMMENT != "<p><br></p>" && result.SPECIES_COMMENT != null)
                                        item.SPECIES_COMMENT_DISPLAY = result.SPECIES_COMMENT;
                                    else
                                        item.SPECIES_COMMENT_DISPLAY = "-";
                                    if (result.CATCHING_AREA_COMMENT != "<p><br></p>" && result.CATCHING_AREA_COMMENT != null)
                                        item.CATCHING_AREA_COMMENT_DISPLAY = result.CATCHING_AREA_COMMENT;
                                    else
                                        item.CATCHING_AREA_COMMENT_DISPLAY = "-";

                                    if (result.CHECK_DETAIL_COMMENT != "<p><br></p>" && result.CHECK_DETAIL_COMMENT != null)
                                        item.CHECK_DETAIL_COMMENT_DISPLAY = result.CHECK_DETAIL_COMMENT;
                                    else
                                        item.CHECK_DETAIL_COMMENT_DISPLAY = "-";

                                    if (result.QC_COMMENT != "" && result.QC_COMMENT != null)
                                        item.QC_COMMENT = result.QC_COMMENT;
                                    else
                                        item.QC_COMMENT = "-";

                                    //Comment Customer
                                    var processFormCustomer = ART_WF_ARTWORK_PROCESS_CUSTOMER_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_CUSTOMER() { ARTWORK_SUB_ID = result.ARTWORK_SUB_ID }, context).FirstOrDefault();

                                    item.COMMENT_FORMLABEL_DISPLAY = processFormCustomer.COMMENT_CHANGE_DETAIL;
                                    item.COMMENT_NONCOMPLIANCE_DISPLAY = processFormCustomer.COMMENT_NONCOMPLIANCE;
                                    item.COMMENT_ADJUST_DISPLAY = processFormCustomer.COMMENT_ADJUST;

                                    if (processFormCustomer.DECISION__CHANGE_DETAIL == "1")
                                        item.DECISION_FORMLABEL_DISPLAY = "Confirm to change";
                                    else if (processFormCustomer.DECISION__CHANGE_DETAIL == "0")
                                        item.DECISION_FORMLABEL_DISPLAY = "Do not change";
                                    else
                                        item.DECISION_FORMLABEL_DISPLAY = "-";
                                    if (processFormCustomer.DECISION__NONCOMPLIANCE == "1")
                                        item.DECISION_NONCOMPLIANCE_DISPLAY = "Confirm to change";
                                    else if (processFormCustomer.DECISION__NONCOMPLIANCE == "0")
                                        item.DECISION_NONCOMPLIANCE_DISPLAY = "Do not change";
                                    else
                                        item.DECISION_NONCOMPLIANCE_DISPLAY = "-";
                                    if (processFormCustomer.DECISION__ADJUST == "1")
                                        item.DECISION_ADJUST_DISPLAY = "Confirm to change";
                                    else if (processFormCustomer.DECISION__ADJUST == "0")
                                        item.DECISION_ADJUST_DISPLAY = "Do not change";
                                    else
                                        item.DECISION_ADJUST_DISPLAY = "-";

                                    var PreviousStep = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context).FirstOrDefault().PARENT_ARTWORK_SUB_ID;
                                    if (PreviousStep != null)
                                    {
                                        var CHK_REQ_TYPE = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_SUB_ID = Convert.ToInt32(PreviousStep) }, context).FirstOrDefault();
                                        if (CHK_REQ_TYPE.CURRENT_STEP_ID == ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_CUS_REQ_REF" }, context).FirstOrDefault().STEP_ARTWORK_ID)
                                        {
                                            item.SEND_TO_CUSTOMER_TYPE = "REQ_CUS_REQ_REF";
                                        }
                                    }

                                    var processFormMK = ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context).FirstOrDefault();

                                    if (processFormMK != null)
                                    {
                                        if (processFormMK.ACTION_CODE != null)
                                        {
                                            if (processFormMK.ACTION_CODE == "APPROVE")
                                            {
                                                item.ACTION_NAME_BY_OTHER = "Approve";
                                            }
                                            else if (processFormMK.ACTION_CODE == "NOTAPPROVE")
                                            {
                                                item.ACTION_NAME_BY_OTHER = "Not approve";
                                            }
                                            else if (processFormMK.ACTION_CODE == "SEND_BACK")
                                            {
                                                item.ACTION_NAME_BY_OTHER = "Send back";
                                            }
                                            else if (processFormMK.ACTION_CODE == "SUBMIT")
                                            {
                                                item.ACTION_NAME_BY_OTHER = "Submit";
                                            }
                                            if (processFormMK.COMMENT != null)
                                                item.COMMENT = processFormMK.COMMENT;
                                            item.CREATE_DATE_BY_OTHER = processFormMK.CREATE_DATE;
                                        }
                                        if (processFormMK.APPROVE != null)
                                        {
                                            if (processFormQC.APPROVE == "1")
                                                item.APPROVE_BY_OTHER = "Approve";
                                            else if (processFormQC.APPROVE == "0")
                                                item.APPROVE_BY_OTHER = "Not Approve";
                                        }
                                    }
                                    item.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                    item.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;

                                    Results.data = Results.data.Where(m => m.PREV_STEP_DISPLAY == item.PREV_STEP_DISPLAY).ToList();

                                    Results.data.Add(item);
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
    }
}
