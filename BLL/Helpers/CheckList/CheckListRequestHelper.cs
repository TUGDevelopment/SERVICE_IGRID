using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;

namespace BLL.Helpers
{
    public class CheckListRequestHelper
    {
        public static ART_WF_MOCKUP_CHECK_LIST_RESULT GetCheckListRequest(ART_WF_MOCKUP_CHECK_LIST_REQUEST param)
        {
            string _P_STYLE = ":";
            ART_WF_MOCKUP_CHECK_LIST_RESULT Results = new ART_WF_MOCKUP_CHECK_LIST_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            return Results;
                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_MOCKUP_CHECK_LIST(ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(MapperServices.ART_WF_MOCKUP_CHECK_LIST(param.data), context));
                        }

                        if (param != null)
                        {
                            Results.draw = param.draw;
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                if (param.data.IS_REFERENCE == "X")
                                {
                                    var stepPG = context.ART_M_STEP_MOCKUP.Where(w => w.STEP_MOCKUP_CODE == "SEND_PG").FirstOrDefault();

                                    int checkListID = 0;
                                    checkListID = Results.data[i].CHECK_LIST_ID;

                                    var listMockupID = (from c in context.ART_WF_MOCKUP_CHECK_LIST_ITEM
                                                        where c.CHECK_LIST_ID == checkListID
                                                        select c.MOCKUP_ID).ToList();

                                    var process = (from p in context.ART_WF_MOCKUP_PROCESS
                                                   where listMockupID.Contains(p.MOCKUP_ID)
                                                    && p.CURRENT_STEP_ID == stepPG.STEP_MOCKUP_ID
                                                    && (p.CURRENT_USER_ID != null || p.CURRENT_USER_ID > 0)
                                                   select p).Count();

                                    if (process <= 0)
                                    {
                                        Results.status = "E";
                                        Results.msg = String.Format(MessageHelper.GetMessage("MSG_029", context), Results.data[i].CHECK_LIST_NO);
                                        return Results;
                                    }
                                }

                                if (Results.data[i].CREATOR_ID > 0)
                                {
                                    var user = ART_M_USER_SERVICE.GetByUSER_ID(Results.data[i].CREATOR_ID, context);
                                    if (user != null)
                                    {
                                        Results.data[i].CREATOR_NAME = CNService.GetUserName(user.USER_ID, context);
                                    }
                                }

                                if (Results.data[i].REFERENCE_REQUEST_ID > 0)
                                {
                                    var refNo = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(Results.data[i].REFERENCE_REQUEST_ID, context);
                                    if (refNo != null)
                                    {
                                        Results.data[i].REFERENCE_CHECK_LIST_DISPLAY_TXT = refNo.CHECK_LIST_NO;
                                    }
                                }

                                if (Results.data[i].TYPE_OF_PRODUCT_ID != null)
                                {
                                    var TypeOfProduct = SAP_M_TYPE_OF_PRODUCT_SERVICE.GetByTYPE_OF_PRODUCT_ID(Results.data[i].TYPE_OF_PRODUCT_ID, context);
                                    if (TypeOfProduct != null)
                                    {
                                        Results.data[i].TYPE_OF_PRODUCT_DISPLAY_TXT = TypeOfProduct.TYPE_OF_PRODUCT + ":" + TypeOfProduct.DESCRIPTION;
                                    }
                                }

                                if (Results.data[i].COMPANY_ID != null)
                                {
                                    var Company = SAP_M_COMPANY_SERVICE.GetByCOMPANY_ID(Results.data[i].COMPANY_ID, context);
                                    if (Company != null)
                                    {
                                        Results.data[i].COMPANY_DISPLAY_TXT = Company.COMPANY_CODE + ":" + Company.DESCRIPTION;
                                    }
                                }

                                if (Results.data[i].SOLD_TO_ID != null)
                                {
                                    var Customer = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(Results.data[i].SOLD_TO_ID, context);
                                    if (Customer != null)
                                    {
                                        Results.data[i].SOLD_TO_DISPLAY_TXT = Customer.CUSTOMER_CODE + ":" + Customer.CUSTOMER_NAME;
                                    }
                                }

                                if (Results.data[i].SHIP_TO_ID != null)
                                {
                                    var ShipTo = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(Results.data[i].SHIP_TO_ID, context);
                                    if (ShipTo != null)
                                    {
                                        Results.data[i].SHIP_TO_DISPLAY_TXT = ShipTo.CUSTOMER_CODE + ":" + ShipTo.CUSTOMER_NAME;
                                    }
                                }

                                if (Results.data[i].CUSTOMER_OTHER_ID != null)
                                {
                                    var customerOther = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(Results.data[i].CUSTOMER_OTHER_ID, context);
                                    if (customerOther != null)
                                    {
                                        Results.data[i].CUSTOMER_OTHER_DISPLAY_TXT = customerOther.CUSTOMER_CODE + ":" + customerOther.CUSTOMER_NAME;
                                    }
                                }

                                if (Results.data[i].RD_PERSON_ID != null)
                                {
                                    var RD = ART_M_USER_SERVICE.GetByUSER_ID(Results.data[i].RD_PERSON_ID, context);
                                    if (RD != null)
                                    {
                                        Results.data[i].RD_PERSON_DISPLAY_TXT = RD.TITLE + " " + RD.FIRST_NAME + " " + RD.LAST_NAME;
                                        Results.data[i].RD_PERSON_DISPLAY_TXT = Results.data[i].RD_PERSON_DISPLAY_TXT.Trim();
                                    }
                                }

                                if (Results.data[i].PRIMARY_TYPE_ID != null)
                                {
                                    var PrimaryType = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(Results.data[i].PRIMARY_TYPE_ID, context);
                                    if (PrimaryType != null)
                                    {
                                        Results.data[i].PRIMARY_TYPE_DISPLAY_TXT = PrimaryType.DESCRIPTION;
                                    }
                                }

                                if (Results.data[i].PRIMARY_SIZE_ID != null)
                                {
                                    var SizeType = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(Results.data[i].PRIMARY_SIZE_ID, context);
                                    if (SizeType != null)
                                    {
                                        Results.data[i].PRIMARY_SIZE_DISPLAY_TXT = SizeType.DESCRIPTION;
                                    }
                                }

                                if (Results.data[i].CONTAINER_TYPE_ID != null)
                                {
                                    var ContainerType = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(Results.data[i].CONTAINER_TYPE_ID, context);
                                    if (ContainerType != null)
                                    {
                                        Results.data[i].CONTAINER_TYPE_DISPLAY_TXT = ContainerType.DESCRIPTION;
                                    }
                                }

                                if (Results.data[i].LID_TYPE_ID != null)
                                {
                                    var LIDType = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(Results.data[i].LID_TYPE_ID, context);
                                    if (LIDType != null)
                                    {
                                        Results.data[i].LID_TYPE_DISPLAY_TXT = LIDType.DESCRIPTION;
                                    }
                                }

                                if (Results.data[i].PACKING_STYLE_ID != null)
                                {
                                    var PackingStyle = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(Results.data[i].PACKING_STYLE_ID, context);
                                    if (PackingStyle != null)
                                    {
                                        Results.data[i].PACKING_STYLE_DISPLAY_TXT = PackingStyle.DESCRIPTION;
                                    }
                                }

                                if (Results.data[i].PACK_SIZE_ID != null)
                                {
                                    var PackSize = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(Results.data[i].PACK_SIZE_ID, context);
                                    if (PackSize != null)
                                    {
                                        Results.data[i].PACK_SIZE_DISPLAY_TXT = PackSize.DESCRIPTION;
                                    }
                                }

                                if (Results.data[i].BRAND_ID != null)
                                {
                                    var Brand = SAP_M_BRAND_SERVICE.GetByBRAND_ID(Results.data[i].BRAND_ID, context);
                                    if (Brand != null)
                                    {
                                        Results.data[i].BRAND_DISPLAY_TXT = Brand.MATERIAL_GROUP + ":" + Brand.DESCRIPTION;
                                    }
                                }

                                if (Results.data[i].TWO_P_ID != null)
                                {
                                    var temp2P = SAP_M_2P_SERVICE.GetByTWO_P_ID(Results.data[i].TWO_P_ID, context);
                                    if (temp2P != null)
                                    {
                                        Results.data[i].TWO_P_DISPLAY_TXT = temp2P.PACKING_SYLE_DESCRIPTION + _P_STYLE + temp2P.PACK_SIZE_DESCRIPTION;
                                        Results.data[i].PACK_SIZE_DISPLAY_TXT = temp2P.PACK_SIZE_DESCRIPTION;
                                        Results.data[i].PACKING_STYLE_DISPLAY_TXT = temp2P.PACKING_SYLE_DESCRIPTION;
                                    }
                                }

                                if (Results.data[i].THREE_P_ID != null)
                                {
                                    var temp3P = SAP_M_3P_SERVICE.GetByTHREE_P_ID(Results.data[i].THREE_P_ID, context);
                                    if (temp3P != null)
                                    {
                                        Results.data[i].THREE_P_DISPLAY_TXT = temp3P.PRIMARY_SIZE_DESCRIPTION + _P_STYLE + temp3P.CONTAINER_TYPE_DESCRIPTION + _P_STYLE + temp3P.LID_TYPE_DESCRIPTION;
                                        Results.data[i].PRIMARY_SIZE_DISPLAY_TXT = temp3P.PRIMARY_SIZE_DESCRIPTION;
                                        Results.data[i].CONTAINER_TYPE_DISPLAY_TXT = temp3P.CONTAINER_TYPE_DESCRIPTION;
                                        Results.data[i].LID_TYPE_DISPLAY_TXT = temp3P.LID_TYPE_DESCRIPTION;
                                    }
                                }

                                ART_M_USER created_by = new ART_M_USER();
                                created_by = ART_M_USER_SERVICE.GetByUSER_ID(Results.data[i].CREATOR_ID, context);
                                if (created_by != null) Results.data[i].CREATOR_NAME = created_by.TITLE + " " + created_by.FIRST_NAME + " " + created_by.LAST_NAME;

                                Results.data[i].REVIEWER_DISPLAY_TXT = CNService.GetUserName(Results.data[i].REVIEWER, context);

                                ART_WF_MOCKUP_CHECK_LIST_COUNTRY_2 country_2 = new ART_WF_MOCKUP_CHECK_LIST_COUNTRY_2();
                                country_2.CHECK_LIST_ID = Results.data[i].CHECK_LIST_ID;
                                Results.data[i].COUNTRY = MapperServices.ART_WF_MOCKUP_CHECK_LIST_COUNTRY(ART_WF_MOCKUP_CHECK_LIST_COUNTRY_SERVICE.GetByItem(country_2, context));

                                if (Results.data[i].COUNTRY.Count > 0)
                                {
                                    for (int iCountry = 0; iCountry < Results.data[i].COUNTRY.Count; iCountry++)
                                    {
                                        var listCountry = SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(Results.data[i].COUNTRY[iCountry].COUNTRY_ID, context);
                                        if (listCountry != null)
                                        {
                                            Results.data[i].COUNTRY[iCountry].COUNTRY_DISPLAY_TXT = listCountry.COUNTRY_CODE + ":" + listCountry.NAME;
                                        }
                                    }
                                }

                                ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_2 mailToCust_2 = new ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_2();
                                mailToCust_2.CHECK_LIST_ID = Results.data[i].CHECK_LIST_ID;
                                Results.data[i].MAIL_TO_CUSTOMER = MapperServices.ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER(ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_SERVICE.GetByItem(mailToCust_2, context));

                                if (Results.data[i].MAIL_TO_CUSTOMER.Count > 0)
                                {
                                    for (int iMailTo = 0; iMailTo < Results.data[i].MAIL_TO_CUSTOMER.Count; iMailTo++)
                                    {
                                        var listMailTo = ART_M_USER_SERVICE.GetByUSER_ID(Results.data[i].MAIL_TO_CUSTOMER[iMailTo].CUSTOMER_USER_ID, context);
                                        if (listMailTo != null)
                                        {
                                            Results.data[i].MAIL_TO_CUSTOMER[iMailTo].USER_DISPLAY_TXT = CNService.GetUserName(listMailTo.USER_ID, context) + " (" + ART_M_USER_SERVICE.GetByUSER_ID(listMailTo.USER_ID, context).USERNAME + ")";
                                        }
                                    }
                                }

                                ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_2 plant_2 = new ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_2();
                                plant_2.CHECK_LIST_ID = Results.data[i].CHECK_LIST_ID;
                                Results.data[i].PLANT = MapperServices.ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT(ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_SERVICE.GetByItem(MapperServices.ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT(plant_2), context));

                                for (int iPlant = 0; iPlant < Results.data[i].PLANT.Count; iPlant++)
                                {
                                    var listPlant = SAP_M_PLANT_SERVICE.GetByPLANT_ID(Results.data[i].PLANT[iPlant].PRODUCTION_PLANT_ID, context);
                                    if (listPlant != null)
                                    {
                                        Results.data[i].PLANT[iPlant].PLANT_DISPLAY_TXT = listPlant.PLANT + ":" + listPlant.NAME;
                                    }
                                }

                                ART_WF_MOCKUP_CHECK_LIST_ITEM item = new ART_WF_MOCKUP_CHECK_LIST_ITEM();
                                item.CHECK_LIST_ID = Results.data[i].CHECK_LIST_ID;
                                Results.data[i].ITEM = MapperServices.ART_WF_MOCKUP_CHECK_LIST_ITEM(ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByItem(item, context));

                                var mail_to = ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER() { CHECK_LIST_ID = Results.data[i].CHECK_LIST_ID, MAIL_TO = "X" }, context);
                                foreach (var item2 in mail_to)
                                {
                                    if (string.IsNullOrEmpty(Results.data[i].TO_DISPLAY_TXT))
                                    {
                                        Results.data[i].TO_DISPLAY_TXT = CNService.GetUserName(item2.CUSTOMER_USER_ID, context);
                                    }
                                    else
                                    {
                                        Results.data[i].TO_DISPLAY_TXT += "\n" + CNService.GetUserName(item2.CUSTOMER_USER_ID, context);
                                    }
                                }
                                var mail_cc = ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER() { CHECK_LIST_ID = Results.data[i].CHECK_LIST_ID, MAIL_CC = "X" }, context);
                                foreach (var item2 in mail_cc)
                                {
                                    if (string.IsNullOrEmpty(Results.data[i].CC_DISPLAY_TXT))
                                    {
                                        Results.data[i].CC_DISPLAY_TXT = CNService.GetUserName(item2.CUSTOMER_USER_ID, context);
                                    }
                                    else
                                    {
                                        Results.data[i].CC_DISPLAY_TXT += "\n" + CNService.GetUserName(item2.CUSTOMER_USER_ID, context);
                                    }
                                }

                                if (Results.data[i].ITEM.Count > 0)
                                {
                                    for (int iItem = 0; iItem < Results.data[i].ITEM.Count; iItem++)
                                    {
                                        var listItemPackaingType = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(Results.data[i].ITEM[iItem].PACKING_TYPE_ID, context);
                                        if (listItemPackaingType != null)
                                        {
                                            Results.data[i].ITEM[iItem].PACKING_TYPE_DISPLAY_TXT = listItemPackaingType.VALUE + ":" + listItemPackaingType.DESCRIPTION;
                                        }

                                        var listItemNumberOfColor = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(Results.data[i].ITEM[iItem].NUMBER_OF_COLOR_ID, context);
                                        if (listItemNumberOfColor != null)
                                        {
                                            Results.data[i].ITEM[iItem].NUMBER_OF_COLOR_DISPLAY_TXT = listItemNumberOfColor.DESCRIPTION;
                                        }

                                        var listItemPrintSystem = SAP_M_CHARACTERISTIC_ITEM_SERVICE.GetByCHARACTERISTIC_ITEM_ID(Results.data[i].ITEM[iItem].PRINT_SYSTEM_ID, context);
                                        if (listItemPrintSystem != null)
                                        {
                                            Results.data[i].ITEM[iItem].PRINT_SYSTEM_DISPLAY_TXT = listItemPrintSystem.DESCRIPTION;
                                        }

                                        var listItemBoxColor = SAP_M_CHARACTERISTIC_ITEM_SERVICE.GetByCHARACTERISTIC_ITEM_ID(Results.data[i].ITEM[iItem].BOX_COLOR_ID, context);
                                        if (listItemBoxColor != null)
                                        {
                                            Results.data[i].ITEM[iItem].BOX_COLOR_DISPLAY_TXT = listItemBoxColor.DESCRIPTION;
                                        }

                                        var listItemCoating = SAP_M_CHARACTERISTIC_ITEM_SERVICE.GetByCHARACTERISTIC_ITEM_ID(Results.data[i].ITEM[iItem].COATING_ID, context);
                                        if (listItemCoating != null)
                                        {
                                            Results.data[i].ITEM[iItem].COATING_DISPLAY_TXT = listItemCoating.DESCRIPTION;
                                        }

                                        var listItemStyle = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(Results.data[i].ITEM[iItem].STYLE_ID, context);
                                        if (listItemStyle != null)
                                        {
                                            Results.data[i].ITEM[iItem].STYLE_DISPLAY_TXT = listItemStyle.DESCRIPTION;
                                        }
                                    }
                                }

                                ART_WF_MOCKUP_CHECK_LIST_PRODUCT product = new ART_WF_MOCKUP_CHECK_LIST_PRODUCT();
                                product.CHECK_LIST_ID = Results.data[i].CHECK_LIST_ID;
                                Results.data[i].PRODUCT = MapperServices.ART_WF_MOCKUP_CHECK_LIST_PRODUCT(ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(product, context));

                                if (Results.data[i].PRODUCT != null || Results.data[i].PRODUCT.Count > 0)
                                {
                                    string productCode = "";
                                    XECM_M_PRODUCT xProduct = new XECM_M_PRODUCT();
                                    for (int j = 0; j <= Results.data[i].PRODUCT.Count - 1; j++)
                                    {
                                        productCode = Results.data[i].PRODUCT[j].PRODUCT_CODE;

                                        xProduct = new XECM_M_PRODUCT();
                                        xProduct.PRODUCT_CODE = productCode;
                                        var xProductTmp = XECM_M_PRODUCT_SERVICE.GetByItem(xProduct, context).FirstOrDefault();

                                        if (xProductTmp != null)
                                        {
                                            Results.data[i].PRODUCT[j].PRODUCT_CODE_ID = xProductTmp.XECM_PRODUCT_ID;
                                            Results.data[i].PRODUCT[j].PRODUCT_DESCRIPTION = xProductTmp.PRODUCT_DESCRIPTION;
                                        }
                                    }
                                }

                                ART_WF_MOCKUP_CHECK_LIST_REFERENCE reference = new ART_WF_MOCKUP_CHECK_LIST_REFERENCE();
                                reference.CHECK_LIST_ID = Results.data[i].CHECK_LIST_ID;
                                Results.data[i].REFERENCE = MapperServices.ART_WF_MOCKUP_CHECK_LIST_REFERENCE(ART_WF_MOCKUP_CHECK_LIST_REFERENCE_SERVICE.GetByItem(reference, context));
                            }
                        }
                    }
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

        public static ART_WF_MOCKUP_CHECK_LIST_RESULT SaveCheckListRequest(ART_WF_MOCKUP_CHECK_LIST_REQUEST param)
        {
            ART_WF_MOCKUP_CHECK_LIST_RESULT Results = new ART_WF_MOCKUP_CHECK_LIST_RESULT();
            int checklistID = 0;
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        //check email to and cc
                        var allUnique = param.data.MAIL_TO_CUSTOMER.GroupBy(x => x.CUSTOMER_USER_ID).All(g => g.Count() == 1);
                        if (!allUnique)
                        {
                            Results.status = "E";
                            Results.msg = "Cannot set duplicate email to and cc customer.";
                            return Results;
                        }

                        //#INC-39523 by aof start.
                        if (param.MOCKUP_SUB_ID_CHECK == 0)
                        {
                            if (param.data.CHECK_LIST_ID > 0)
                            {
                                var mockup = context.ART_WF_MOCKUP_CHECK_LIST_ITEM.Where(w => w.CHECK_LIST_ID == param.data.CHECK_LIST_ID).ToList().FirstOrDefault();
                                if (mockup != null)
                                {
                                    if (!string.IsNullOrEmpty(mockup.MOCKUP_NO))
                                    {
                                        Results.status = "E";
                                        Results.msg = "This CheckListNo already processed.<br/>Please refresh your web browser.";
                                        return Results;
                                    }
                                }
                            }
                            
                        }
                        //#INC-39523 by aof end.


                        ART_WF_MOCKUP_CHECK_LIST checklist = new ART_WF_MOCKUP_CHECK_LIST();

                        if (param.data.CHECK_LIST_ID > 0)
                        {
                            param.data.CHECK_LIST_NO = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(param.data.CHECK_LIST_ID, context).CHECK_LIST_NO;
                        }

                        checklist = SaveMockUp(param.data, context);
                        checklistID = checklist.CHECK_LIST_ID;

                        SaveCountryOperation(param.data, context, checklistID);
                        SaveMailToCustomerOperation(param.data, context, checklistID);
                        SavePlantOperation(param.data, context, checklistID);
                        SaveProductOperation(param.data, context, checklistID);
                        SaveReferenceOperation(param.data, context, checklistID);
                        SaveItemOperation(param.data, context, checklistID);

                        Results.data = new List<ART_WF_MOCKUP_CHECK_LIST_2>();
                        ART_WF_MOCKUP_CHECK_LIST_2 item = new ART_WF_MOCKUP_CHECK_LIST_2();
                        item.CHECK_LIST_ID = checklistID;
                        Results.data.Add(item);

                        if (param.data.ENDTASKFORM)
                        {
                            var SEND_BACK_MK = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_BACK_MK" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                            var process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(param.data.MOCKUP_SUB_ID, context);
                            if (process.CURRENT_STEP_ID == SEND_BACK_MK)
                            {
                                //find ref
                                var tempChecklist = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(checklistID, context);
                                if (!string.IsNullOrEmpty(tempChecklist.CHECK_LIST_NO))
                                {
                                    var tempListChecklist = new List<ART_WF_MOCKUP_CHECK_LIST>();
                                    if (tempChecklist.REFERENCE_REQUEST_ID == null)
                                    {
                                        tempListChecklist = (from h in context.ART_WF_MOCKUP_CHECK_LIST
                                                             where h.REFERENCE_REQUEST_TYPE == "CHECKLIST"
                                                             && (h.REFERENCE_REQUEST_ID == checklistID)
                                                             select h).ToList();
                                    }
                                    else
                                    {
                                        //# INC-99131 start by aof commented
                                        //tempListChecklist = (from h in context.ART_WF_MOCKUP_CHECK_LIST
                                        //                     where (h.REFERENCE_REQUEST_TYPE == "CHECKLIST" && h.REFERENCE_REQUEST_ID == tempChecklist.REFERENCE_REQUEST_ID)
                                        //                     || (h.CHECK_LIST_ID == tempChecklist.REFERENCE_REQUEST_ID)
                                        //                     select h).ToList();
                                        //# INC-99131 end by aof commented
                                    }

                                    foreach (var itemTempChecklist in tempListChecklist)
                                    {
                                        if (itemTempChecklist.CHECK_LIST_ID != checklistID)
                                        {
                                            var oldCheklist = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(itemTempChecklist.CHECK_LIST_ID, context);
                                            ART_WF_MOCKUP_CHECK_LIST_2 itemChecklist = param.data;
                                            itemChecklist.CHECK_LIST_ID = oldCheklist.CHECK_LIST_ID;
                                            itemChecklist.CHECK_LIST_NO = oldCheklist.CHECK_LIST_NO;
                                            itemChecklist.REFERENCE_REQUEST_ID = oldCheklist.REFERENCE_REQUEST_ID;
                                            itemChecklist.REFERENCE_REQUEST_NO = oldCheklist.REFERENCE_REQUEST_NO;
                                            itemChecklist.REFERENCE_REQUEST_TYPE = oldCheklist.REFERENCE_REQUEST_TYPE;
                                            itemChecklist.REQUEST_FOR_DIE_LINE = oldCheklist.REQUEST_FOR_DIE_LINE;
                                            itemChecklist.CHECK_LIST_FOR_DESIGN = oldCheklist.CHECK_LIST_FOR_DESIGN;
                                            itemChecklist.CREATOR_ID = oldCheklist.CREATOR_ID;     //by aof #INC-98813

                                            SaveMockUp(itemChecklist, context);
                                            SaveCountryOperation(itemChecklist, context, itemTempChecklist.CHECK_LIST_ID);
                                            SaveMailToCustomerOperation(itemChecklist, context, itemTempChecklist.CHECK_LIST_ID);
                                            SavePlantOperation(itemChecklist, context, itemTempChecklist.CHECK_LIST_ID);
                                            SaveProductOperation(itemChecklist, context, itemTempChecklist.CHECK_LIST_ID);
                                            SaveReferenceOperation(itemChecklist, context, itemTempChecklist.CHECK_LIST_ID);
                                        }
                                    }
                                }
                                //end find ref
                            }
                        }

                        if (param.data.ENDTASKFORM)
                        {
                            //copy to pg tab
                            var tempCheckList = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(checklistID, context);
                            var tempCheckListItem = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(param.data.MOCKUP_ID, context);
                            CheckListRequestHelper.CopyCheckListToPG(tempCheckList, tempCheckListItem, context);

                            MockUpProcessHelper.EndTaskForm(param.data.MOCKUP_SUB_ID, param.data.UPDATE_BY, context);

                            var SEND_MK_UPD_PACK_STYLE = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_MK_UPD_PACK_STYLE" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                            var process = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(param.data.MOCKUP_SUB_ID, context);
                            if (Convert.ToInt32(process.CURRENT_STEP_ID) == SEND_MK_UPD_PACK_STYLE)
                            {
                                //if (param.data.TWO_P_ID > 0)
                                {
                                    //complete wf
                                    var SEND_PG = ART_M_STEP_MOCKUP_SERVICE.GetByItem(new ART_M_STEP_MOCKUP() { STEP_MOCKUP_CODE = "SEND_PG" }, context).FirstOrDefault().STEP_MOCKUP_ID;
                                    var MOCKUP_SUB_ID = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = param.data.MOCKUP_ID, CURRENT_STEP_ID = SEND_PG }, context).FirstOrDefault().MOCKUP_SUB_ID;
                                    MockUpProcessHelper.EndTaskForm(MOCKUP_SUB_ID, param.data.UPDATE_BY, context);
                                }
                            }
                        }

                        dbContextTransaction.Commit();

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
            return Results;
        }

        public static ART_WF_MOCKUP_CHECK_LIST_RESULT DeleteCheckListRequest(ART_WF_MOCKUP_CHECK_LIST_REQUEST Item)
        {
            ART_WF_MOCKUP_CHECK_LIST_RESULT Results = new ART_WF_MOCKUP_CHECK_LIST_RESULT();
            int checklistID = 0;
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        //checklistID = SaveMockUp(Item.data, context);
                        checklistID = Item.data.CHECK_LIST_ID;

                        DeleteCheckListRequest(context, checklistID);
                        DeleteMailToCustomerByCheckID(context, checklistID);
                        DeleteCountryByCheckID(context, checklistID);
                        DeletePlantByCheckID(context, checklistID);
                        DeleteProductByCheckID(context, checklistID);
                        DeleteReferenceByCheckID(context, checklistID);
                        DeleteItemByCheckID(context, checklistID);

                        dbContextTransaction.Commit();

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
            return Results;
        }

        private static string ValidateItem(ART_WF_MOCKUP_CHECK_LIST_REQUEST Item)
        {
            int cCountry = 0;
            int cPlant = 0;
            int cProduct = 0;
            int cReference = 0;
            int cItem = 0;

            cCountry = Item.data.COUNTRY.Count;
            cPlant = Item.data.PLANT.Count;
            cProduct = Item.data.PRODUCT.Count;
            cReference = Item.data.REFERENCE.Count;
            cItem = Item.data.ITEM.Count;

            StringBuilder sb = new StringBuilder();

            //if (cCountry == 0)
            //{
            //    sb.AppendLine(String.Format("The {0} field is required.", "Country"));
            //}

            //if (cProduct == 0 && cReference == 0)
            //{
            //    sb.AppendLine(String.Format("The {0} field is required.", "Product or Reference"));
            //}

            if (cPlant == 0)
            {
                // sb.AppendLine(String.Format("The {0} field is required.", "Plant"));
            }

            if (cItem == 0)
            {
                sb.AppendLine(String.Format("The {0} field is required.", "Item"));
            }

            return sb.ToString();
        }

        private static ART_WF_MOCKUP_CHECK_LIST SaveMockUp(ART_WF_MOCKUP_CHECK_LIST_2 itemChecklist, ARTWORKEntities context)
        {
            ART_WF_MOCKUP_CHECK_LIST itemTmp = MapperServices.ART_WF_MOCKUP_CHECK_LIST(itemChecklist);
            if (!string.IsNullOrEmpty(itemTmp.REFERENCE_REQUEST_NO))
            {
                if (itemTmp.REFERENCE_REQUEST_NO.StartsWith("CL-"))
                {
                    itemTmp.REFERENCE_REQUEST_TYPE = "CHECKLIST";
                    itemTmp.REFERENCE_REQUEST_ID = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST() { CHECK_LIST_NO = itemTmp.REFERENCE_REQUEST_NO }, context).FirstOrDefault().CHECK_LIST_ID;
                }
                else
                {
                    itemTmp.REFERENCE_REQUEST_TYPE = "ARTWORK";
                    itemTmp.REFERENCE_REQUEST_ID = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST() { ARTWORK_REQUEST_NO = itemTmp.REFERENCE_REQUEST_NO }, context).FirstOrDefault().ARTWORK_REQUEST_ID;
                }
            }

            ART_WF_MOCKUP_CHECK_LIST_SERVICE.SaveOrUpdateNoLog(itemTmp, context);

            return itemTmp;
        }

        private static void DeleteMockUp(ART_WF_MOCKUP_CHECK_LIST_2 itemChecklist, ARTWORKEntities context)
        {
            ART_WF_MOCKUP_CHECK_LIST itemTmp = MapperServices.ART_WF_MOCKUP_CHECK_LIST(itemChecklist);
            ART_WF_MOCKUP_CHECK_LIST_SERVICE.DeleteByCHECK_LIST_ID(itemTmp.CHECK_LIST_ID, context);
        }

        private static void SaveMailToCustomerOperation(ART_WF_MOCKUP_CHECK_LIST_2 Item, ARTWORKEntities context, int checklistID)
        {
            DeleteMailToCustomerByCheckID(context, checklistID);

            SaveMailToCustomer(Item, context, checklistID);
        }

        private static void SaveCountryOperation(ART_WF_MOCKUP_CHECK_LIST_2 Item, ARTWORKEntities context, int checklistID)
        {
            DeleteCountryByCheckID(context, checklistID);

            SaveCountry(Item, context, checklistID);
        }

        private static void SavePlantOperation(ART_WF_MOCKUP_CHECK_LIST_2 Item, ARTWORKEntities context, int checklistID)
        {
            DeletePlantByCheckID(context, checklistID);

            SavePlant(Item, context, checklistID);
        }

        private static void SaveItemOperation(ART_WF_MOCKUP_CHECK_LIST_2 Item, ARTWORKEntities context, int checklistID)
        {
            if (Item.ITEM.Count == 0 || Item.ITEM[0].MOCKUP_ID == 0)
                DeleteItemByCheckID(context, checklistID);

            SaveItem(Item, context, checklistID);
        }

        private static void SaveProductOperation(ART_WF_MOCKUP_CHECK_LIST_2 Item, ARTWORKEntities context, int checklistID)
        {
            DeleteProductByCheckID(context, checklistID);

            SaveProduct(Item, context, checklistID);
        }

        private static void SaveReferenceOperation(ART_WF_MOCKUP_CHECK_LIST_2 Item, ARTWORKEntities context, int checklistID)
        {
            DeleteReferenceByCheckID(context, checklistID);

            SaveReference(Item, context, checklistID);
        }

        #region "Save Data"

        private static void SaveMailToCustomer(ART_WF_MOCKUP_CHECK_LIST_2 Item, ARTWORKEntities context, int checklistID)
        {
            if (Item.MAIL_TO_CUSTOMER != null && Item.MAIL_TO_CUSTOMER.Count > 0)
            {
                foreach (ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_2 itemMailToCust in Item.MAIL_TO_CUSTOMER)
                {
                    itemMailToCust.CHECK_LIST_ID = checklistID;
                    ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER(itemMailToCust), context);
                }
            }
        }

        private static void SaveCountry(ART_WF_MOCKUP_CHECK_LIST_2 Item, ARTWORKEntities context, int checklistID)
        {
            if (Item.COUNTRY != null && Item.COUNTRY.Count > 0)
            {
                foreach (ART_WF_MOCKUP_CHECK_LIST_COUNTRY_2 itemCountry in Item.COUNTRY)
                {
                    itemCountry.CHECK_LIST_ID = checklistID;
                    ART_WF_MOCKUP_CHECK_LIST_COUNTRY_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_MOCKUP_CHECK_LIST_COUNTRY(itemCountry), context);
                }
            }
        }

        private static void SaveProduct(ART_WF_MOCKUP_CHECK_LIST_2 Item, ARTWORKEntities context, int checklistID)
        {
            if (Item.PRODUCT != null && Item.PRODUCT.Count > 0)
            {
                foreach (ART_WF_MOCKUP_CHECK_LIST_PRODUCT_2 itemProduct in Item.PRODUCT)
                {
                    itemProduct.CHECK_LIST_ID = checklistID;

                    ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_MOCKUP_CHECK_LIST_PRODUCT(itemProduct), context);
                }
            }
        }

        private static void SavePlant(ART_WF_MOCKUP_CHECK_LIST_2 Item, ARTWORKEntities context, int checklistID)
        {
            if (Item.PLANT != null && Item.PLANT.Count > 0)
            {
                foreach (ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_2 itemPlant in Item.PLANT)
                {
                    itemPlant.CHECK_LIST_ID = checklistID;
                    ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT(itemPlant), context);
                }
            }
        }

        private static void SaveItem(ART_WF_MOCKUP_CHECK_LIST_2 Item, ARTWORKEntities context, int checklistID)
        {
            if (Item.ITEM != null && Item.ITEM.Count > 0)
            {
                foreach (ART_WF_MOCKUP_CHECK_LIST_ITEM_2 item in Item.ITEM)
                {
                    item.CHECK_LIST_ID = checklistID;
                    ART_WF_MOCKUP_CHECK_LIST_ITEM checklistItem = new ART_WF_MOCKUP_CHECK_LIST_ITEM();
                    checklistItem = MapperServices.ART_WF_MOCKUP_CHECK_LIST_ITEM(item);

                    if (Item.ITEM[0].MOCKUP_ID > 0)
                    {
                        checklistItem.NODE_ID = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(Item.ITEM[0].MOCKUP_ID, context).NODE_ID;
                        checklistItem.MOCKUP_NO = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByMOCKUP_ID(Item.ITEM[0].MOCKUP_ID, context).MOCKUP_NO;
                    }
                    ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.SaveOrUpdateNoLog(checklistItem, context);
                }
            }
        }

        private static void SaveReference(ART_WF_MOCKUP_CHECK_LIST_2 Item, ARTWORKEntities context, int checklistID)
        {
            if (Item.REFERENCE != null && Item.REFERENCE.Count > 0)
            {
                foreach (ART_WF_MOCKUP_CHECK_LIST_REFERENCE_2 reference in Item.REFERENCE)
                {
                    reference.CHECK_LIST_ID = checklistID;
                    ART_WF_MOCKUP_CHECK_LIST_REFERENCE_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_MOCKUP_CHECK_LIST_REFERENCE(reference), context);
                }
            }
        }
        #endregion

        #region "Delete Data"

        private static void DeleteCheckListRequest(ARTWORKEntities context, int checklistID)
        {
            ART_WF_MOCKUP_CHECK_LIST_SERVICE.DeleteByCHECK_LIST_ID(checklistID, context);
        }

        private static void DeleteMailToCustomerByCheckID(ARTWORKEntities context, int checklistID)
        {
            //ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER mailToCust = new ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER();
            //mailToCust.CHECK_LIST_ID = checklistID;

            //var listMailToCust = ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_SERVICE.GetByItem(mailToCust, context);

            //if (listMailToCust != null && listMailToCust.Count > 0)
            //{
            //    foreach (ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER itemMailToCust in listMailToCust)
            //    {
            //        ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_SERVICE.DeleteByCHECK_LIST_OTHER_CUSTOMER_ID(itemMailToCust.CHECK_LIST_OTHER_CUSTOMER_ID, context);
            //    }
            //}

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER WHERE CHECK_LIST_ID  = '" + checklistID + "'");
        }

        private static void DeleteCountryByCheckID(ARTWORKEntities context, int checklistID)
        {
            //ART_WF_MOCKUP_CHECK_LIST_COUNTRY country = new ART_WF_MOCKUP_CHECK_LIST_COUNTRY();
            //country.CHECK_LIST_ID = checklistID;

            //var listCountry = ART_WF_MOCKUP_CHECK_LIST_COUNTRY_SERVICE.GetByItem(country, context);

            //if (listCountry != null && listCountry.Count > 0)
            //{
            //    foreach (ART_WF_MOCKUP_CHECK_LIST_COUNTRY itemCountry in listCountry)
            //    {
            //        ART_WF_MOCKUP_CHECK_LIST_COUNTRY_SERVICE.DeleteByCHECK_LIST_COUNTRY_ID(itemCountry.CHECK_LIST_COUNTRY_ID, context);
            //    }
            //}

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_MOCKUP_CHECK_LIST_COUNTRY WHERE CHECK_LIST_ID  = '" + checklistID + "'");
        }

        private static void DeletePlantByCheckID(ARTWORKEntities context, int checklistID)
        {
            //ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT plant = new ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT();
            //plant.CHECK_LIST_ID = checklistID;

            //var listPlant = ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_SERVICE.GetByItem(plant, context);

            //if (listPlant != null && listPlant.Count > 0)
            //{
            //    foreach (ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT itemPlant in listPlant)
            //    {
            //        ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_SERVICE.DeleteByCHECK_LIST_PLANT_ID(itemPlant.CHECK_LIST_PLANT_ID, context);
            //    }
            //}

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT WHERE CHECK_LIST_ID  = '" + checklistID + "'");
        }

        private static void DeleteItemByCheckID(ARTWORKEntities context, int checklistID)
        {
            //ART_WF_MOCKUP_CHECK_LIST_ITEM item = new ART_WF_MOCKUP_CHECK_LIST_ITEM();
            //item.CHECK_LIST_ID = checklistID;

            //var listItem = ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.GetByItem(item, context);

            //if (listItem != null && listItem.Count > 0)
            //{
            //    foreach (ART_WF_MOCKUP_CHECK_LIST_ITEM _item in listItem)
            //    {
            //        ART_WF_MOCKUP_CHECK_LIST_ITEM_SERVICE.DeleteByMOCKUP_ID(_item.MOCKUP_ID, context);
            //    }
            //}

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_MOCKUP_CHECK_LIST_ITEM WHERE CHECK_LIST_ID  = '" + checklistID + "'");
        }

        private static void DeleteProductByCheckID(ARTWORKEntities context, int checklistID)
        {
            //ART_WF_MOCKUP_CHECK_LIST_PRODUCT product = new ART_WF_MOCKUP_CHECK_LIST_PRODUCT();
            //product.CHECK_LIST_ID = checklistID;

            //var listProduct = ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.GetByItem(product, context);

            //if (listProduct != null && listProduct.Count > 0)
            //{
            //    foreach (ART_WF_MOCKUP_CHECK_LIST_PRODUCT _product in listProduct)
            //    {
            //        ART_WF_MOCKUP_CHECK_LIST_PRODUCT_SERVICE.DeleteByCHECK_LIST_PRODUCT_ID(_product.CHECK_LIST_PRODUCT_ID, context);
            //    }
            //}

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_MOCKUP_CHECK_LIST_PRODUCT WHERE CHECK_LIST_ID  = '" + checklistID + "'");
        }

        private static void DeleteReferenceByCheckID(ARTWORKEntities context, int checklistID)
        {
            //ART_WF_MOCKUP_CHECK_LIST_REFERENCE reference = new ART_WF_MOCKUP_CHECK_LIST_REFERENCE();
            //reference.CHECK_LIST_ID = checklistID;

            //var listReference = ART_WF_MOCKUP_CHECK_LIST_REFERENCE_SERVICE.GetByItem(reference, context);

            //if (listReference != null && listReference.Count > 0)
            //{
            //    foreach (ART_WF_MOCKUP_CHECK_LIST_REFERENCE _ref in listReference)
            //    {
            //        ART_WF_MOCKUP_CHECK_LIST_REFERENCE_SERVICE.DeleteByCHECK_LIST_REFERENCE_ID(_ref.CHECK_LIST_REFERENCE_ID, context);
            //    }
            //}

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_MOCKUP_CHECK_LIST_REFERENCE WHERE CHECK_LIST_ID  = '" + checklistID + "'");
        }

        #endregion

        #region "Check List data to PG"

        public static void CopyCheckListToPG(ART_WF_MOCKUP_CHECK_LIST checkList,
                                                ART_WF_MOCKUP_CHECK_LIST_ITEM checkListItem,
                                                ARTWORKEntities context)
        {
            ART_WF_MOCKUP_CHECK_LIST_PG checkListPG = new ART_WF_MOCKUP_CHECK_LIST_PG();

            var chk = ART_WF_MOCKUP_CHECK_LIST_PG_SERVICE.GetByItem(new ART_WF_MOCKUP_CHECK_LIST_PG() { MOCKUP_ID = checkListItem.MOCKUP_ID }, context).FirstOrDefault();
            if (chk != null)
            {
                checkListPG.CHECK_LIST_PG_ID = chk.CHECK_LIST_PG_ID;
            }

            if (checkList != null)
            {
                checkListPG.CHECK_LIST_ID = checkList.CHECK_LIST_ID;
                checkListPG.PRIMARY_TYPE_ID = checkList.PRIMARY_TYPE_ID;
                checkListPG.PRIMARY_TYPE_OTHER = checkList.PRIMARY_TYPE_OTHER;
                checkListPG.PRIMARY_SIZE_ID = checkList.PRIMARY_SIZE_ID;
                checkListPG.PRIMARY_SIZE_OTHER = checkList.PRIMARY_SIZE_OTHER;
                checkListPG.CONTAINER_TYPE_ID = checkList.CONTAINER_TYPE_ID;
                checkListPG.CONTAINER_TYPE_OTHER = checkList.CONTAINER_TYPE_OTHER;
                checkListPG.LID_TYPE_ID = checkList.LID_TYPE_ID;
                checkListPG.LID_TYPE_OTHER = checkList.LID_TYPE_OTHER;
                checkListPG.PACKING_STYLE_ID = checkList.PACKING_STYLE_ID;
                checkListPG.PACKING_STYLE_OTHER = checkList.PACKING_STYLE_OTHER;
                checkListPG.PACK_SIZE_ID = checkList.PACK_SIZE_ID;
                checkListPG.PACK_SIZE_OTHER = checkList.PACK_SIZE_OTHER;
                checkListPG.PRIMARY_MATERIAL = checkList.PRIMARY_MATERIAL;
                checkListPG.THREE_P_ID = checkList.THREE_P_ID;
                checkListPG.TWO_P_ID = checkList.TWO_P_ID;

                if (checkListItem != null)
                {
                    checkListPG.MOCKUP_ID = checkListItem.MOCKUP_ID;

                    checkListPG.PACKING_TYPE_ID = checkListItem.PACKING_TYPE_ID;
                    checkListPG.PRINT_SYSTEM_ID = checkListItem.PRINT_SYSTEM_ID;
                    checkListPG.NUMBER_OF_COLOR_ID = checkListItem.NUMBER_OF_COLOR_ID;
                    checkListPG.BOX_COLOR_ID = checkListItem.BOX_COLOR_ID;
                    checkListPG.COATING_ID = checkListItem.COATING_ID;
                    checkListPG.PURPOSE_OF = checkListItem.PURPOSE_OF;
                    checkListPG.STYLE_ID = checkListItem.STYLE_ID;
                    checkListPG.PACKING_TYPE_OTHER = checkListItem.PACKING_TYPE_OTHER;
                    checkListPG.PRINT_SYSTEM_OTHER = checkListItem.PRINT_SYSTEM_OTHER;
                    checkListPG.NUMBER_OF_COLOR_OTHER = checkListItem.NUMBER_OF_COLOR_OTHER;
                    checkListPG.BOX_COLOR_OTHER = checkListItem.BOX_COLOR_OTHER;
                    checkListPG.COATING_OTHER = checkListItem.COATING_OTHER;
                    checkListPG.STYLE_OTHER = checkListItem.STYLE_OTHER;
                    checkListPG.REMARK = checkListItem.REMARK;
                    checkListPG.CREATE_BY = checkListItem.CREATE_BY;
                    checkListPG.UPDATE_BY = checkListItem.UPDATE_BY;
                }

                if (checkListPG != null)
                {
                    ART_WF_MOCKUP_CHECK_LIST_PG_SERVICE.SaveOrUpdateNoLog(checkListPG, context);
                }
            }
        }

        //public static void DeleteCheckListPG(int checkListID, int mockupID, ARTWORKEntities context)
        //{
        //    ART_WF_MOCKUP_CHECK_LIST_PG _checkListPG = new ART_WF_MOCKUP_CHECK_LIST_PG();
        //    List<ART_WF_MOCKUP_CHECK_LIST_PG> _listCheckListPG = new List<ART_WF_MOCKUP_CHECK_LIST_PG>();

        //    _checkListPG.CHECK_LIST_ID = checkListID;
        //    _checkListPG.MOCKUP_ID = mockupID;
        //    _listCheckListPG = ART_WF_MOCKUP_CHECK_LIST_PG_SERVICE.GetByItem(_checkListPG, context);

        //    if (_listCheckListPG.Count > 0)
        //    {
        //        foreach (ART_WF_MOCKUP_CHECK_LIST_PG itemPG in _listCheckListPG)
        //        {
        //            ART_WF_MOCKUP_CHECK_LIST_PG_SERVICE.DeleteByCHECK_LIST_PG_ID(itemPG.CHECK_LIST_PG_ID, context);
        //        }
        //    }
        //}
        #endregion

    }
}
