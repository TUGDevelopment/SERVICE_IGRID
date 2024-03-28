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
    public static class PriceTemplateHelper
    {
        public static ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT GetPriceTemplate(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetAll(context));
                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetByItemContain(MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(param.data), context));
                        }

                        var parentId = (from m in context.ART_WF_MOCKUP_PROCESS where m.MOCKUP_ID == param.data.MOCKUP_ID && m.PARENT_MOCKUP_SUB_ID == null select m.MOCKUP_SUB_ID).FirstOrDefault();
                        Results.data = Results.data.Where(m => m.MOCKUP_SUB_ID == parentId).ToList();

                        var groupBy = Results.data.GroupBy(item => item.SCALE)
                             .Select(group => new { ID = group.Key, Items = group.ToList() })
                             .ToList();

                        List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2> newlist = new List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2>();
                        foreach (var item in groupBy)
                        {
                            newlist.Add(Results.data.Where(m => m.SCALE == item.ID).FirstOrDefault());
                        }

                        Results.data = newlist;
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
        public static ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT GetPriceTemplateForVendor(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetAll(context));
                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetByItemContain(MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(param.data), context));
                        }

                        var groupBy = Results.data.GroupBy(item => item.SCALE)
                           .Select(group => new { ID = group.Key, Items = group.ToList() })
                           .ToList();

                        List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2> newlist = new List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2>();
                        foreach (var item in groupBy)
                        {
                            newlist.Add(Results.data.Where(m => m.SCALE == item.ID).FirstOrDefault());
                        }

                        Results.data = newlist;
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

        public static ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT SavePriceTemplate(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST_LIST param)
        {
            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null && param.data.Count > 0)
                        {
                            int MOCKUP_SUB_ID = param.data[0].MOCKUP_SUB_ID;
                            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE temp = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE();
                            temp.MOCKUP_SUB_ID = MOCKUP_SUB_ID;

                            var existPrice = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetByItemContain(temp, context);
                            foreach (ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE itemExistPrice in existPrice)
                            {
                                if (param.data.Where(m => m.PRICE_TEMPLATE_ID == itemExistPrice.PRICE_TEMPLATE_ID).Count() == 0)
                                    ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.DeleteByPRICE_TEMPLATE_ID(itemExistPrice.PRICE_TEMPLATE_ID, context);
                            }
                        }

                        foreach (ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2 iPrice in param.data)
                        {
                            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE price_template = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE();
                            price_template = MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(iPrice);

                            var user = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetByPRICE_TEMPLATE_ID(iPrice.PRICE_TEMPLATE_ID, context);
                            if (user != null)
                                price_template.USER_ID = user.USER_ID;

                            var vendor = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetByPRICE_TEMPLATE_ID(iPrice.PRICE_TEMPLATE_ID, context);
                            if (vendor != null)
                                price_template.VENDOR_ID = vendor.VENDOR_ID;

                            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.SaveOrUpdate(price_template, context);
                        }

                        if (!string.IsNullOrEmpty(param.COMMENT_BY_VENDOR))
                        {
                            //save remark vendor
                            var temp2 = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR() { MOCKUP_SUB_ID = param.MOCKUP_SUB_ID }, context);
                            if (temp2.FirstOrDefault() == null)
                            {
                                ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR item = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR();
                                item.MOCKUP_ID = param.MOCKUP_ID;
                                item.MOCKUP_SUB_ID = param.MOCKUP_SUB_ID;
                                item.UPDATE_BY = param.UPDATE_BY;
                                item.CREATE_BY = param.CREATE_BY;
                                item.COMMENT_BY_VENDOR = param.COMMENT_BY_VENDOR;
                                ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR_SERVICE.SaveOrUpdate(item, context);
                            }
                            else
                            {
                                ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR item = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR();
                                item = temp2.FirstOrDefault();
                                item.MOCKUP_ID = param.MOCKUP_ID;
                                item.MOCKUP_SUB_ID = param.MOCKUP_SUB_ID;
                                item.UPDATE_BY = param.UPDATE_BY;
                                item.CREATE_BY = param.CREATE_BY;
                                item.COMMENT_BY_VENDOR = param.COMMENT_BY_VENDOR;
                                ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR_SERVICE.SaveOrUpdate(item, context);
                            }
                        }

                        if (param.ENDTASKFORM)
                        {
                            MockUpProcessHelper.EndTaskForm(param.data[0].MOCKUP_SUB_ID, param.data[0].UPDATE_BY, context);

                            //end process for vendor user id not action.
                            var vendorId = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(param.data[0].MOCKUP_SUB_ID, context).CURRENT_VENDOR_ID;
                            var SEND_VN_QUO = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_VN_QUO" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                            var otherProcessVendorQuo = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS()
                            {
                                MOCKUP_ID = param.data[0].MOCKUP_ID,
                                CURRENT_STEP_ID = SEND_VN_QUO,
                                CURRENT_VENDOR_ID = vendorId
                            }, context);

                            otherProcessVendorQuo = otherProcessVendorQuo.Where(m => string.IsNullOrEmpty(m.IS_END)).ToList();
                            foreach (var item in otherProcessVendorQuo)
                            {
                                if (item.MOCKUP_SUB_ID != param.data[0].MOCKUP_SUB_ID)
                                    MockUpProcessHelper.EndTaskForm(item.MOCKUP_SUB_ID, -1, context);
                            }
                        }

                        dbContextTransaction.Commit();

                        if (param.ENDTASKFORM)
                        {
                            //if (param.data.ACTION_CODE == "SEND_BACK")
                            //    EmailService.sendEmail(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "MOCKUP_SEND_BACK", context);
                            //else if (param.data.ACTION_CODE == "SAVE")
                            //    EmailService.sendEmail(param.data.MOCKUP_ID, param.data.MOCKUP_SUB_ID, "MOCKUP_OTHER_SAVE", context);
                            //else
                            EmailService.sendEmailMockup(param.data[0].MOCKUP_ID, param.data[0].MOCKUP_SUB_ID, "WF_OTHER_SUBMIT", context);
                        }

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

        public static ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT PostPriceTemplateManual(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var listUserVendor = ART_M_USER_VENDOR_SERVICE.GetByItem(new ART_M_USER_VENDOR() { VENDOR_ID = param.data.CURRENT_VENDOR_ID }, context);
                        //if (ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_VENDOR_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_VENDOR() { MOCKUP_ID = param.data.MOCKUP_ID, VENDOR_ID = param.data.CURRENT_VENDOR_ID }).Count == 0)
                        //{
                        //    ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_VENDOR vendor = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_VENDOR();
                        //    vendor.MOCKUP_ID = param.data.MOCKUP_ID;
                        //    vendor.MOCKUP_SUB_ID = param.data.MOCKUP_SUB_ID;
                        //    ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_VENDOR_SERVICE.SaveOrUpdate(vendor, context);
                        //}

                        ART_WF_MOCKUP_PROCESS process = new ART_WF_MOCKUP_PROCESS();
                        process.MOCKUP_ID = param.data.MOCKUP_ID;
                        process.PARENT_MOCKUP_SUB_ID = param.data.MOCKUP_SUB_ID;
                        var parentId = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(param.data.MOCKUP_SUB_ID, context).PARENT_MOCKUP_SUB_ID;
                        if (parentId != null)
                        {
                            process.PARENT_MOCKUP_SUB_ID = parentId;
                        }

                        if (listUserVendor.FirstOrDefault() != null) process.CURRENT_USER_ID = listUserVendor.FirstOrDefault().USER_ID;
                        process.CURRENT_VENDOR_ID = param.data.CURRENT_VENDOR_ID;
                        process.CURRENT_STEP_ID = param.data.CURRENT_STEP_ID;
                        process.IS_END = "X";
                        process.REMARK = "Manaul add price template.";
                        process.CREATE_BY = param.data.CREATE_BY;
                        process.UPDATE_BY = param.data.UPDATE_BY;
                        ART_WF_MOCKUP_PROCESS_SERVICE.SaveOrUpdate(process, context);

                        ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE filter = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE();
                        filter.MOCKUP_ID = param.data.MOCKUP_ID;
                        filter.MOCKUP_SUB_ID = param.data.MOCKUP_SUB_ID;
                        if (parentId != null)
                        {
                            filter.MOCKUP_SUB_ID = Convert.ToInt32(parentId);
                        }
                        var listScale = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetByItem(filter, context);

                        foreach (var item in listScale)
                        {
                            var temp = MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(item);
                            var temp2 = MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(temp);

                            temp2.PRICE_TEMPLATE_ID = 0;
                            temp2.PRICE = 0;
                            temp2.MOCKUP_SUB_ID = process.MOCKUP_SUB_ID;
                            temp2.USER_ID = process.CURRENT_USER_ID;
                            temp2.VENDOR_ID = process.CURRENT_VENDOR_ID;
                            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.SaveOrUpdate(temp2, context);
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

        public static ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT PostPriceTemplateManualPrice(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST_LIST param)
        {
            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {

                        foreach (ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2 iPrice in param.data)
                        {
                            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE price_template = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE();

                            price_template = MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(iPrice);

                            var temp = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE() { MOCKUP_SUB_ID = iPrice.MOCKUP_SUB_ID, SCALE = iPrice.SCALE }, context).FirstOrDefault();
                            if (temp != null)
                            {
                                price_template.VENDOR_ID = temp.VENDOR_ID;
                                price_template.USER_ID = temp.USER_ID;
                                price_template.PRICE_TEMPLATE_ID = temp.PRICE_TEMPLATE_ID;
                                price_template.SELECTED = temp.SELECTED;
                            }

                            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.SaveOrUpdate(price_template, context);

                            var chk = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR() { MOCKUP_ID = iPrice.MOCKUP_ID, MOCKUP_SUB_ID = iPrice.MOCKUP_SUB_ID }, context);
                            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR model = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR();
                            if (chk.Count > 0)
                            {
                                model.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_HEADER_ID = chk.FirstOrDefault().ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_HEADER_ID;
                            }
                            model.MOCKUP_ID = iPrice.MOCKUP_ID;
                            model.MOCKUP_SUB_ID = iPrice.MOCKUP_SUB_ID;
                            model.COMMENT_BY_VENDOR = iPrice.COMMENT_BY_VENDOR;
                            model.CREATE_BY = iPrice.CREATE_BY;
                            model.UPDATE_BY = iPrice.UPDATE_BY;
                            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR_SERVICE.SaveOrUpdate(model, context);
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

        public static ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT DeletePriceTemplateManualPrice(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE filter = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE();
                        filter.MOCKUP_SUB_ID = param.data.MOCKUP_SUB_ID;
                        var list = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetByItem(filter, context);

                        foreach (var item in list)
                        {
                            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.DeleteByPRICE_TEMPLATE_ID(item.PRICE_TEMPLATE_ID, context);
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

    }
}
