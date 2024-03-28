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
    public class CustomerByPGHelper
    {
        public static ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_RESULT GetCustomerByPG(ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_RESULT Results = new ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG(ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_SERVICE.GetAll(context));
                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG(ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_SERVICE.GetByItemContain(MapperServices.ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG(param.data), context));
                        }

                        ART_WF_MOCKUP_PROCESS p = new ART_WF_MOCKUP_PROCESS();

                        Results.data[0].PROCESS = MapperServices.ART_WF_MOCKUP_PROCESS(p);

                        Results.status = "S";

                        if (Results.data.Count > 0)
                        {

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

        public static ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_RESULT SaveCustomerByPG(ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_REQUEST_LIST param)
        {
            ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_RESULT Results = new ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_RESULT();
            try
            {
                //int _userCutomerID = 0;
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        //_userCutomerID = CheckSelectedCustomer(context, param);
                        //if (_userCutomerID == 0)
                        //{
                        //    Results.status = "E";
                        //    Results.msg = MessageHelper.GetMessage("MSG_004");
                        //    return Results;
                        //}
                        if (param.data != null)
                        {
                            string msg = MockUpProcessHelper.checkDupWF(param.data[0].PROCESS, context);
                            if (msg != "")
                            {
                                Results.status = "E";
                                Results.msg = msg;
                                return Results;
                            }

                            msg = "";
                            bool error = false;
                            var listProcess = SaveProcessCustomerByPG(param, context, ref error, ref msg);
                            if (error)
                            {
                                Results.status = "E";
                                Results.msg = msg;
                                return Results;
                            }

                            foreach (var process in listProcess)
                            {
                                ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG customerData = new ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG();
                                customerData = MapperServices.ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG(param.data[0]);
                                customerData.MOCKUP_SUB_ID = process.MOCKUP_SUB_ID;
                                ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_SERVICE.SaveOrUpdate(customerData, context);
                            }

                            dbContextTransaction.Commit();

                            if (param.data[0].isProjectNoCus == "0")
                            {
                                foreach (var item in listProcess)
                                    EmailService.sendEmailMockup(item.MOCKUP_ID, item.MOCKUP_SUB_ID, "WF_SEND_TO_CUSTOMER", context);
                            }
                            else
                            {
                                foreach (var item in listProcess)
                                    EmailService.sendEmailMockup(item.MOCKUP_ID, item.MOCKUP_SUB_ID, "WF_SEND_TO", context);
                            }

                            Results.status = "S";
                            Results.msg = MessageHelper.GetMessage("MSG_001", context);
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

        private static int CheckSelectedCustomer(ARTWORKEntities context, ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_REQUEST param)
        {
            int _user_id = 0;

            ART_M_USER_CUSTOMER _userCustomer = new ART_M_USER_CUSTOMER();
            int _currentCustomerID = Convert.ToInt32(param.data.PROCESS.CURRENT_CUSTOMER_ID);

            _userCustomer.CUSTOMER_ID = _currentCustomerID;
            var _userCustomerExist = ART_M_USER_CUSTOMER_SERVICE.GetByItem(_userCustomer, context).FirstOrDefault();

            if (_userCustomerExist != null)
            {
                _user_id = _userCustomerExist.USER_ID;
            }

            return _user_id;
        }

        private static List<ART_WF_MOCKUP_PROCESS> SaveProcessCustomerByPG(ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_REQUEST_LIST param, ARTWORKEntities context, ref bool error, ref string msg)
        {
            List<ART_WF_MOCKUP_PROCESS> listProcess = new List<ART_WF_MOCKUP_PROCESS>();
            var checkListId = CNService.ConvertMockupIdToCheckListId(param.data[0].MOCKUP_ID, context);

            if (param.data[0].isProjectNoCus == "0")
            {
                var listCus = ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER() { CHECK_LIST_ID = checkListId }, context);
                //var listCus = ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER() { CHECK_LIST_ID = checkListId, MAIL_TO = "X" }, context);

                listCus = listCus.OrderByDescending(m => m.MAIL_TO).ToList();

                foreach (var item in listCus)
                {
                    ART_WF_MOCKUP_PROCESS process = new ART_WF_MOCKUP_PROCESS();
                    process = MapperServices.ART_WF_MOCKUP_PROCESS(param.data[0].PROCESS);

                    process.CURRENT_USER_ID = item.CUSTOMER_USER_ID;
                    var temp = ART_M_USER_CUSTOMER_SERVICE.GetByItem(new ART_M_USER_CUSTOMER() { USER_ID = item.CUSTOMER_USER_ID }, context).FirstOrDefault();
                    if (temp != null) process.CURRENT_CUSTOMER_ID = temp.CUSTOMER_ID;

                    //msg = MockUpProcessHelper.checkDupWF(MapperServices.ART_WF_MOCKUP_PROCESS(process), context);
                    //if (msg != "")
                    //{
                    //    error = true;
                    //    return listProcess;
                    //}

                    CNService.CheckDelegateBeforeRounting(process, context);

                    listProcess.Add(process);
                }
            }
            else
            {
                var checkList = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(checkListId, context);

                param.data[0].PROCESS.CURRENT_USER_ID = checkList.CREATOR_ID;

                msg = MockUpProcessHelper.checkDupWF(param.data[0].PROCESS, context);
                if (msg != "")
                {
                    error = true;
                    return listProcess;
                }

                var process = MapperServices.ART_WF_MOCKUP_PROCESS(param.data[0].PROCESS);

                CNService.CheckDelegateBeforeRounting(process, context);

                listProcess.Add(process);
            }

            return listProcess;
        }
    }



}
