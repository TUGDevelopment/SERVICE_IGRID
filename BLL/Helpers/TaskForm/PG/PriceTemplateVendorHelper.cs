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
    public static class PriceTemplateVendorHelper
    {
        public static ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_RESULT GetPriceTemplateVendor(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_RESULT();

            try
            {
                List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2> listVendor2 = new List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2>();
                List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2> listVendor2_tmp = new List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2>();
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                        }
                        else
                        {
                            listVendor2 = MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_SERVICE.GetByItem(MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG(param.data), context));
                        }

                        if (listVendor2.Count > 0)
                        {
                            foreach (ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2 itemVendor in listVendor2)
                            {
                                // itemVendor.VENDOR_ID

                                string VENDOR_NAME = "";
                                string EMAIL = "";
                                if (itemVendor.VENDOR_ID > 0) VENDOR_NAME = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(itemVendor.VENDOR_ID, context).VENDOR_NAME;
                                if (itemVendor.USER_ID > 0) EMAIL = ART_M_USER_SERVICE.GetByUSER_ID(itemVendor.USER_ID, context).EMAIL;

                                itemVendor.VENDOR_DISPLAY_TXT = VENDOR_NAME;
                                itemVendor.USER_DISPLAY_TXT = CNService.GetUserName(itemVendor.USER_ID, context);
                                itemVendor.EMAIL = EMAIL;

                                itemVendor.STYLE_OF_PRINTING_OTHER_DISPLAY_TXT = itemVendor.STYLE_OF_PRINTING_OTHER;

                                itemVendor.STYLE_OF_PRINTING_DISPLAY_TXT = "";
                                itemVendor.DIRECTION_OF_STICKER_DISPLAY_TXT = "";
                                if (itemVendor.STYLE_OF_PRINTING > 0)
                                {
                                    itemVendor.STYLE_OF_PRINTING_DISPLAY_TXT = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(itemVendor.STYLE_OF_PRINTING, context).DESCRIPTION;
                                }
                                if (itemVendor.DIRECTION_OF_STICKER > 0)
                                {
                                    itemVendor.DIRECTION_OF_STICKER_DISPLAY_TXT = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(itemVendor.DIRECTION_OF_STICKER, context).DESCRIPTION;
                                }
                                listVendor2_tmp.Add(itemVendor);
                            }
                        }
                    }
                }

                Results.data = listVendor2_tmp.OrderBy(m => m.VENDOR_ID).ToList();
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_RESULT GetPriceTemplateVendorTran(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_RESULT();

            try
            {
                List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2> listVendor2 = new List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2>();
                List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2> listVendor2_tmp = new List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2>();
                List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE> list = new List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE>();
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                        }
                        else
                        {
                            list = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE() { MOCKUP_ID = param.data.MOCKUP_ID }, context);
                        }

                        list = list.OrderByDescending(m => m.UPDATE_DATE).ToList();

                        foreach (var item in list)
                        {
                            var process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(item.MOCKUP_SUB_ID, context);
                            if (process.CURRENT_VENDOR_ID != null)
                            {
                                if (listVendor2_tmp.Where(m => m.VENDOR_ID == process.CURRENT_VENDOR_ID).ToList().Count == 0)
                                {
                                    ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2 itemVendor = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2();
                                    itemVendor.MOCKUPSUBID2 = item.MOCKUP_SUB_ID;
                                    itemVendor.PRICE_TEMPLATE_ID = item.PRICE_TEMPLATE_ID;

                                    string VENDOR_NAME = "";
                                    string EMAIL = "";

                                    var SEND_VN_QUO = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_VN_QUO" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                                    var listProcess = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID, CURRENT_STEP_ID = SEND_VN_QUO }, context);
                                    listProcess = listProcess.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();

                                    if (listProcess.Where(m => m.CURRENT_VENDOR_ID == process.CURRENT_VENDOR_ID && m.UPDATE_BY != -1 && m.IS_END == "X").Count() > 0)
                                    {
                                        itemVendor.CANSELECT = "1";
                                    }
                                    else
                                    {
                                        itemVendor.CANSELECT = "0";
                                    }

                                    if (process.CURRENT_VENDOR_ID > 0) VENDOR_NAME = XECM_M_VENDOR_SERVICE.GetByVENDOR_ID(process.CURRENT_VENDOR_ID, context).VENDOR_NAME;
                                    if (process.CURRENT_USER_ID > 0) EMAIL = ART_M_USER_SERVICE.GetByUSER_ID(process.CURRENT_USER_ID, context).EMAIL;

                                    itemVendor.VENDOR_DISPLAY_TXT = VENDOR_NAME;
                                    if (process.CURRENT_USER_ID > 0) itemVendor.USER_DISPLAY_TXT = CNService.GetUserName(process.CURRENT_USER_ID, context);
                                    itemVendor.EMAIL = EMAIL;
                                    itemVendor.VENDOR_ID = Convert.ToInt32(process.CURRENT_VENDOR_ID);
                                    listVendor2_tmp.Add(itemVendor);
                                }
                            }
                        }
                    }
                }
                Results.data = listVendor2_tmp.OrderBy(m => m.VENDOR_ID).ToList();
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_RESULT SavePriceTemplateVendor(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_REQUEST_LIST param)
        {
            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        foreach (ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2 iVendor in param.data)
                        {
                            var price_template_vendor = MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG(iVendor);

                            var temp = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG() { MOCKUP_ID = price_template_vendor.MOCKUP_ID }, context).FirstOrDefault();
                            if (temp != null) price_template_vendor.PRICE_TEMPLATE_VENDOR_ID = temp.PRICE_TEMPLATE_VENDOR_ID;
                            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_SERVICE.SaveOrUpdate(price_template_vendor, context);
                        }

                        dbContextTransaction.Commit();

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

        public static ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT PostSelectVendor(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    var listProcessNoSelect = new List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE>();
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var list = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE() { MOCKUP_ID = param.data.MOCKUP_ID }, context);
                        foreach (var item in list)
                        {
                            if (item.MOCKUP_SUB_ID == param.data.MOCKUPSUBID2)
                            {
                                item.SELECTED = "X";
                            }
                            else
                            {
                                item.SELECTED = null;
                                if (item.VENDOR_ID > 0) listProcessNoSelect.Add(item);
                            }

                            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.SaveOrUpdate(item, context);
                        }

                        ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_SUP model = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_SUP();
                        model.MOCKUP_ID = param.data.MOCKUP_ID;
                        model.MOCKUP_SUB_ID = param.data.MOCKUP_SUB_ID;
                        model.COMMENY_BY_PG_SUP = param.data.COMMENT_BY_PG_SUP;
                        ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_SUP_SERVICE.SaveOrUpdate(model, context);

                        if (param.data.ACTION_CODE == "SUBMIT")
                        {
                            //auto update filed vendor in tab pg
                            var parent_mockup_sub_id = Convert.ToInt32(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(param.data.MOCKUP_SUB_ID, context).PARENT_MOCKUP_SUB_ID);
                            var temp = ART_WF_MOCKUP_PROCESS_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG() { MOCKUP_SUB_ID = parent_mockup_sub_id }, context).FirstOrDefault();
                            if (temp != null)
                            {
                                temp.VENDOR = param.data.VENDOR_ID;
                                ART_WF_MOCKUP_PROCESS_PG_SERVICE.SaveOrUpdate(temp, context);
                            }

                            MockUpProcessHelper.EndTaskForm(param.data.MOCKUP_SUB_ID, param.data.UPDATE_BY, context);

                            dbContextTransaction.Commit();
                            var manualvendor = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(param.data.MOCKUPSUBID2, context);

                            if (manualvendor.REMARK != "Manaul add price template.")
                                EmailService.sendEmailMockup(param.data.MOCKUP_ID, param.data.MOCKUPSUBID2, "WF_SEND_TO_VENDOR_QUO_SELECTED", context);
                            foreach (var s in listProcessNoSelect.Select(m => m.MOCKUP_SUB_ID).Distinct().ToList())
                            {
                                var process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(s, context);
                                if (process.PARENT_MOCKUP_SUB_ID > 0)
                                {
                                    if (process.UPDATE_BY != -1 && process.REMARK != "Manaul add price template.")
                                        EmailService.sendEmailMockup(param.data.MOCKUP_ID, Convert.ToInt32(s), "WF_SEND_TO_VENDOR_QUO_NO_SELECTED", context);
                                }
                                //var updateBy = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(s, context).UPDATE_BY;
                                //var manualnoselectvendor = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(s, context);

                                //if (updateBy != -1 && manualnoselectvendor.REMARK != "Manaul add price template.")
                                //    EmailService.sendEmailMockup(param.data.MOCKUP_ID, Convert.ToInt32(s), "WF_SEND_TO_VENDOR_QUO_NO_SELECTED", context);
                            }
                        }
                        else
                        {
                            //auto clear vendor in tab pg
                            var parent_mockup_sub_id = Convert.ToInt32(ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(param.data.MOCKUP_SUB_ID, context).PARENT_MOCKUP_SUB_ID);
                            var temp = ART_WF_MOCKUP_PROCESS_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG() { MOCKUP_SUB_ID = parent_mockup_sub_id }, context).FirstOrDefault();
                            if (temp != null)
                            {
                                temp.VENDOR = null;
                                ART_WF_MOCKUP_PROCESS_PG_SERVICE.SaveOrUpdate(temp, context);
                            }

                            MockUpProcessHelper.EndTaskForm(param.data.MOCKUP_SUB_ID, param.data.UPDATE_BY, context);

                            dbContextTransaction.Commit();
                        }

                        if (param.data.ACTION_CODE == "SEND_BACK")
                            EmailService.sendEmailMockup(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "WF_SEND_BACK", context, param.data.COMMENT_BY_PG_SUP);
                        else if (param.data.ACTION_CODE == "SAVE")
                            EmailService.sendEmailMockup(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "WF_OTHER_SAVE", context);
                        else
                            EmailService.sendEmailMockup(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "WF_OTHER_SUBMIT", context);

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
    }
}
