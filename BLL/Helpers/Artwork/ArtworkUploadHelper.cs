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
    public class ArtworkUploadHelper
    {
        public static ART_WF_ARTWORK_REQUEST_RESULT DeleteArtworkRequestOperation(ART_WF_ARTWORK_REQUEST_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();
            ART_WF_ARTWORK_REQUEST request = new ART_WF_ARTWORK_REQUEST();

            try
            {
                if (param == null || param.data == null)
                {
                    //Results.data = MapperServices.ART_WF_MOCKUP_CHECK_LIST(ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetAll());
                    return Results;
                }
                else
                {
                    if (param != null && param.data != null)
                    {
                        request = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(param.data.ARTWORK_REQUEST_ID, context);

                        if (request != null)
                        {
                            DeleteMailToCustomerByArtworkID(context, request.ARTWORK_REQUEST_ID);
                            DeleteRequestSOByArtworkID(context, request.ARTWORK_REQUEST_ID);
                            DeleteRequestSORepeatByArtworkID(context, request.ARTWORK_REQUEST_ID);
                            DeleteRequestReferenceByArtworkID(context, request.ARTWORK_REQUEST_ID);
                            DeleteRequestProductionPlantByArtworkID(context, request.ARTWORK_REQUEST_ID);
                            DeleteRequestProductByArtworkID(context, request.ARTWORK_REQUEST_ID);
                            DeleteRequestCountryByArtworkID(context, request.ARTWORK_REQUEST_ID);
                            DeleteRequestRecipientByArtworkID(context, request.ARTWORK_REQUEST_ID);
                            DeleteRequestItemByArtworkID(context, request.ARTWORK_REQUEST_ID);

                            DeleteRequestSODetailByArtworkID(context, request.ARTWORK_REQUEST_ID);
                            DeleteRequestSOAssignByArtworkID(context, request.ARTWORK_REQUEST_ID);

                            if (request.REQUEST_FORM_FOLDER_NODE_ID != null && request.REQUEST_FORM_FOLDER_NODE_ID > 0)
                            {
                                long nodeID = Convert.ToInt64(request.REQUEST_FORM_FOLDER_NODE_ID);
                                var token = CWSService.getAuthToken();
                                CWSService.deleteNode(nodeID, token);
                            }

                            ART_WF_ARTWORK_REQUEST_SERVICE.DeleteByARTWORK_REQUEST_ID(request.ARTWORK_REQUEST_ID, context);
                        }
                    }

                    Results.status = "S";
                    Results.msg = MessageHelper.GetMessage("MSG_001", context);
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_REQUEST_RESULT SaveUploadRequestForm(ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();

            using (var context = new ARTWORKEntities())
            {
                using (var dbContextTransaction = CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 300;

                    Results = aSaveUploadRequestForm(param, context);

                    if (Results.status != "E")
                    {
                        dbContextTransaction.Commit();
                    }
                }
            }
            return Results;
        }

        public static ART_WF_ARTWORK_REQUEST_RESULT aSaveUploadRequestForm(ART_WF_ARTWORK_REQUEST_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();
            ART_WF_ARTWORK_REQUEST artwork = new ART_WF_ARTWORK_REQUEST();
            ART_WF_ARTWORK_REQUEST_2 artwork2 = new ART_WF_ARTWORK_REQUEST_2();
            List<ART_WF_ARTWORK_REQUEST_2> listArtwork2 = new List<ART_WF_ARTWORK_REQUEST_2>();

            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }
                else
                {

                    if (param != null && param.data != null)
                    {
                        if (param.data.ARTWORK_REQUEST_ID <= 0)
                        {
                            SaveArtworkRequest(param, out artwork, out artwork2, context);

                            long folderID = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkRequestFormNodeID"]);
                            long templateID = Convert.ToInt64(ConfigurationManager.AppSettings["ArtworkTemplateNodeID"]);

                            var token = CWSService.getAuthToken();
                            var nodeTemplate = CWSService.copyNode(artwork.ARTWORK_REQUEST_ID.ToString(), templateID, folderID, token);
                            param.data.REQUEST_FORM_FOLDER_NODE_ID = nodeTemplate.ID;
                            param.data.ARTWORK_REQUEST_ID = artwork.ARTWORK_REQUEST_ID;
                            SaveRequest(param.data, context);
                        }
                        else
                        {
                            if (param.data.REQUEST_RECIPIENT != null)
                            {
                                artwork2 = param.data;
                                artwork2.REQUEST_RECIPIENT = param.data.REQUEST_RECIPIENT;
                                SaveRequestRecipientOperation(artwork2, context, param.data.ARTWORK_REQUEST_ID);
                            }
                        }

                        listArtwork2.Add(artwork2);

                        Results.data = listArtwork2;
                    }

                    Results.status = "S";
                    Results.msg = MessageHelper.GetMessage("MSG_001", context);
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_REQUEST_RESULT GetArtworkRequest(ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            string P_STYLE = ":";
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();
            ART_WF_ARTWORK_REQUEST artwork = new ART_WF_ARTWORK_REQUEST();
            ART_WF_ARTWORK_REQUEST_2 artwork2 = new ART_WF_ARTWORK_REQUEST_2();
            List<ART_WF_ARTWORK_REQUEST_2> listArtwork2 = new List<ART_WF_ARTWORK_REQUEST_2>();

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
                            Results.data = MapperServices.ART_WF_ARTWORK_REQUEST(ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                if (Results.data[i].CREATOR_ID > 0)
                                {
                                    Results.data[i].CREATOR_NAME = CNService.GetUserName(Results.data[i].CREATOR_ID, context);
                                }

                                if (Results.data[i].UPLOAD_BY > 0)
                                {
                                    Results.data[i].ARTWORK_UPLOADED_BY_DISPLAY_TXT = CNService.GetUserName(Results.data[i].UPLOAD_BY, context);
                                }

                                Results.data[i].IS_FFC = CNService.IsFFC(Results.data[i].CREATE_BY, context);
                                if (!Results.data[i].IS_FFC) Results.data[i].IS_FFC = CNService.IsFFC(Results.data[i].UPDATE_BY, context);

                                if (Results.data[i].REFERENCE_REQUEST_ID > 0)
                                {
                                    var refRequest = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(Results.data[i].REFERENCE_REQUEST_ID, context);
                                    if (refRequest != null)
                                    {
                                        Results.data[i].REFERENCE_ARTWORK_REQUEST_DISPLAY_TXT = refRequest.ARTWORK_REQUEST_NO;
                                    }
                                }

                                if (Results.data[i].COMPANY_ID > 0)
                                {
                                    var company = SAP_M_COMPANY_SERVICE.GetByCOMPANY_ID(Results.data[i].COMPANY_ID, context);
                                    if (company != null)
                                    {
                                        Results.data[i].COMPANY_DISPLAY_TXT = company.COMPANY_CODE + ":" + company.DESCRIPTION;
                                    }
                                }

                                if (Results.data[i].SOLD_TO_ID > 0)
                                {
                                    var soldTo = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(Results.data[i].SOLD_TO_ID, context);
                                    if (soldTo != null)
                                    {
                                        Results.data[i].SOLD_TO_DISPLAY_TXT = soldTo.CUSTOMER_CODE + ":" + soldTo.CUSTOMER_NAME;
                                    }
                                }

                                if (Results.data[i].SHIP_TO_ID > 0)
                                {
                                    var shipTo = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(Results.data[i].SHIP_TO_ID, context);
                                    if (shipTo != null)
                                    {
                                        Results.data[i].SHIP_TO_DISPLAY_TXT = shipTo.CUSTOMER_CODE + ":" + shipTo.CUSTOMER_NAME;
                                    }
                                }

                                if (Results.data[i].BRAND_ID > 0)
                                {
                                    var brand = SAP_M_BRAND_SERVICE.GetByBRAND_ID(Results.data[i].BRAND_ID, context);
                                    if (brand != null)
                                    {
                                        Results.data[i].BRAND_DISPLAY_TXT = brand.MATERIAL_GROUP + ":" + brand.DESCRIPTION;
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

                                if (Results.data[i].REVIEWER_ID > 0)
                                {
                                    Results.data[i].REVIEWER_DISPLAY_TXT = CNService.GetUserName(Results.data[i].REVIEWER_ID, context);
                                }

                                if (Results.data[i].TYPE_OF_PRODUCT_ID > 0)
                                {
                                    var typeOfProduct = SAP_M_TYPE_OF_PRODUCT_SERVICE.GetByTYPE_OF_PRODUCT_ID(Results.data[i].TYPE_OF_PRODUCT_ID, context);
                                    if (typeOfProduct != null)
                                    {
                                        Results.data[i].TYPE_OF_PRODUCT_DISPLAY_TXT = typeOfProduct.TYPE_OF_PRODUCT + ":" + typeOfProduct.DESCRIPTION;
                                    }
                                }

                                ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER mailToCust_2 = new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER();
                                mailToCust_2.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;
                                Results.data[i].MAIL_TO_CUSTOMER = MapperServices.ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER(ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.GetByItem(mailToCust_2, context));

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

                                ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2 plant_2 = new ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2();
                                plant_2.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;
                                Results.data[i].PRODUCTION_PLANT = MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT(ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT(plant_2), context));

                                for (int iPlant = 0; iPlant < Results.data[i].PRODUCTION_PLANT.Count; iPlant++)
                                {
                                    var listPlant = SAP_M_PLANT_SERVICE.GetByPLANT_ID(Results.data[i].PRODUCTION_PLANT[iPlant].PRODUCTION_PLANT_ID, context);
                                    if (listPlant != null)
                                    {
                                        Results.data[i].PRODUCTION_PLANT[iPlant].PRODUCTION_PLANT_DISPLAY_TXT = listPlant.PLANT + ":" + listPlant.NAME;
                                    }
                                }

                                ART_WF_ARTWORK_REQUEST_COUNTRY_2 country_2 = new ART_WF_ARTWORK_REQUEST_COUNTRY_2();
                                country_2.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;
                                Results.data[i].COUNTRY = MapperServices.ART_WF_ARTWORK_REQUEST_COUNTRY(ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST_COUNTRY(country_2), context));

                                for (int iCountry = 0; iCountry < Results.data[i].COUNTRY.Count; iCountry++)
                                {
                                    var listCountry = SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(Results.data[i].COUNTRY[iCountry].COUNTRY_ID, context);
                                    if (listCountry != null)
                                    {
                                        Results.data[i].COUNTRY[iCountry].COUNTRY_DISPLAY_TXT = listCountry.COUNTRY_CODE + ":" + listCountry.NAME;
                                    }
                                }

                                if (Results.data[i].PRIMARY_TYPE_ID > 0)
                                {
                                    Results.data[i].PRIMARY_TYPE_DISPLAY_TXT = CNService.GetCharacteristicDescription(Results.data[i].PRIMARY_TYPE_ID, context);
                                }

                                if (Results.data[i].THREE_P_ID > 0)
                                {
                                    var temp3p = SAP_M_3P_SERVICE.GetByTHREE_P_ID(Results.data[i].THREE_P_ID, context);
                                    if (temp3p != null)
                                    {
                                        Results.data[i].THREE_P_DISPLAY_TXT = temp3p.PRIMARY_SIZE_DESCRIPTION + P_STYLE + temp3p.CONTAINER_TYPE_DESCRIPTION + P_STYLE + temp3p.LID_TYPE_DESCRIPTION;
                                        Results.data[i].PRIMARY_SIZE_DISPLAY_TXT = temp3p.PRIMARY_SIZE_DESCRIPTION;
                                        Results.data[i].CONTAINER_TYPE_DISPLAY_TXT = temp3p.CONTAINER_TYPE_DESCRIPTION;
                                        Results.data[i].LID_TYPE_DISPLAY_TXT = temp3p.LID_TYPE_DESCRIPTION;
                                    }
                                }
                                else
                                {
                                    Results.data[i].PRIMARY_SIZE_DISPLAY_TXT = CNService.GetCharacteristicDescription(Results.data[i].PRIMARY_SIZE_ID, context);
                                    Results.data[i].CONTAINER_TYPE_DISPLAY_TXT = CNService.GetCharacteristicDescription(Results.data[i].CONTAINER_TYPE_ID, context);
                                    Results.data[i].LID_TYPE_DISPLAY_TXT = CNService.GetCharacteristicDescription(Results.data[i].LID_TYPE_ID, context);
                                }

                                if (Results.data[i].TWO_P_ID > 0)
                                {
                                    var temp2p = SAP_M_2P_SERVICE.GetByTWO_P_ID(Results.data[i].TWO_P_ID, context);
                                    if (temp2p != null)
                                    {
                                        Results.data[i].TWO_P_DISPLAY_TXT = temp2p.PACKING_SYLE_DESCRIPTION + P_STYLE + temp2p.PACK_SIZE_DESCRIPTION;
                                        Results.data[i].PACKING_STYLE_DISPLAY_TXT = temp2p.PACKING_SYLE_DESCRIPTION;
                                        Results.data[i].PACK_SIZE_DISPLAY_TXT = temp2p.PACK_SIZE_DESCRIPTION;
                                    }
                                }
                                else
                                {
                                    Results.data[i].PACKING_STYLE_DISPLAY_TXT = CNService.GetCharacteristicDescription(Results.data[i].PACKING_STYLE_ID, context);
                                    Results.data[i].PACK_SIZE_DISPLAY_TXT = CNService.GetCharacteristicDescription(Results.data[i].PACK_SIZE_ID, context);
                                }

                                #region "Reference"
                                ART_WF_ARTWORK_REQUEST_REFERENCE_2 ref_2 = new ART_WF_ARTWORK_REQUEST_REFERENCE_2();
                                ref_2.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;

                                var references = MapperServices.ART_WF_ARTWORK_REQUEST_REFERENCE(ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST_REFERENCE(ref_2), context));
                                Results.data[i].REFERENCE = references;
                                #endregion

                                #region "Product"
                                ART_WF_ARTWORK_REQUEST_PRODUCT_2 product_2 = new ART_WF_ARTWORK_REQUEST_PRODUCT_2();
                                product_2.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;

                                var products = MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCT(ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCT(product_2), context));
                                List<XECM_M_PRODUCT_2> listProduct = new List<XECM_M_PRODUCT_2>();
                                if (products != null && products.Count > 0)
                                {
                                    XECM_M_PRODUCT product = new XECM_M_PRODUCT();
                                    XECM_M_PRODUCT_2 product2 = new XECM_M_PRODUCT_2();

                                    for (int iP = 0; iP <= products.Count - 1; iP++)
                                    {
                                        product = new XECM_M_PRODUCT();
                                        product.XECM_PRODUCT_ID = products[iP].PRODUCT_CODE_ID;
                                        
                                        var xProduct = XECM_M_PRODUCT_SERVICE.GetByItem(product, context);
                                        if (xProduct != null && xProduct.Count > 0)
                                        {

                                            for (int j = 0; j <= xProduct.Count - 1; j++)
                                            {
                                                product2 = new XECM_M_PRODUCT_2();
                                                product2 = MapperServices.XECM_M_PRODUCT(xProduct[j]);
                                                product2.PRODUCT_CODE_ID = xProduct[j].XECM_PRODUCT_ID;
                                                product2.PRODUCT_TYPE = products[iP].PRODUCT_TYPE;
                                                listProduct.Add(product2);
                                            }
                                        }
                                    }
                                }
                                Results.data[i].PRODUCT = listProduct;
                                #endregion

                                ART_WF_ARTWORK_REQUEST_SALES_ORDER_2 so_2 = new ART_WF_ARTWORK_REQUEST_SALES_ORDER_2();
                                so_2.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;
                                Results.data[i].SALES_ORDER = MapperServices.ART_WF_ARTWORK_REQUEST_SALES_ORDER(ART_WF_ARTWORK_REQUEST_SALES_ORDER_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST_SALES_ORDER(so_2), context));

                                ART_WF_ARTWORK_REQUEST_RECIPIENT_2 recipient_2 = new ART_WF_ARTWORK_REQUEST_RECIPIENT_2();
                                recipient_2.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;
                                Results.data[i].REQUEST_RECIPIENT = MapperServices.ART_WF_ARTWORK_REQUEST_RECIPIENT(ART_WF_ARTWORK_REQUEST_RECIPIENT_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST_RECIPIENT(recipient_2), context));

                                for (int iRecipient = 0; iRecipient < Results.data[i].REQUEST_RECIPIENT.Count; iRecipient++)
                                {
                                    ART_M_USER user = new ART_M_USER();
                                    user = ART_M_USER_SERVICE.GetByUSER_ID(Results.data[i].REQUEST_RECIPIENT[iRecipient].RECIPIENT_USER_ID, context);

                                    Results.data[i].REQUEST_RECIPIENT[iRecipient].RECIPIENT_USER_DISPLAY_TXT = CNService.GetUserName(Results.data[i].REQUEST_RECIPIENT[iRecipient].RECIPIENT_USER_ID, context) + " (" + user.EMAIL + ")";
                                    Results.data[i].REQUEST_RECIPIENT[iRecipient].RECIPIENT_EMAIL = user.EMAIL;
                                    Results.data[i].REQUEST_RECIPIENT[iRecipient].RECIPIENT_POSITION_DISPLAY_TXT = ART_M_POSITION_SERVICE.GetByART_M_POSITION_ID(user.POSITION_ID, context).ART_M_POSITION_NAME;
                                    Results.data[i].REQUEST_RECIPIENT[iRecipient].RECIPIENT_POSITION_ID = Convert.ToInt32(user.POSITION_ID);
                                }

                                ART_WF_ARTWORK_REQUEST_ITEM item = new ART_WF_ARTWORK_REQUEST_ITEM();
                                item.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;

                                var items = MapperServices.ART_WF_ARTWORK_REQUEST_ITEM(ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(item, context));
                                if (items != null && items.Count > 0)
                                {
                                    ART_WF_ARTWORK_PROCESS processTmp = new ART_WF_ARTWORK_PROCESS();
                                    foreach (ART_WF_ARTWORK_REQUEST_ITEM_2 item2 in items)
                                    {
                                        item2.CREATE_BY_DISPLAY_TXT = CNService.GetUserName(item2.CREATE_BY, context);
                                        processTmp = new ART_WF_ARTWORK_PROCESS();
                                        processTmp.ARTWORK_ITEM_ID = item2.ARTWORK_ITEM_ID;

                                        item2.NODE_ID_TXT = EncryptionService.EncryptAndUrlEncode(item2.REQUEST_FORM_FILE_NODE_ID.ToString());

                                        var temp = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(processTmp, context).FirstOrDefault();
                                        if (temp != null)
                                            item2.ARTWORK_SUB_ID = temp.ARTWORK_SUB_ID;
                                    }
                                }
                                Results.data[i].REQUEST_ITEMS = items;
                            }
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

        public static ART_WF_ARTWORK_REQUEST_RESULT GetArtworkRequestForRepeat(ART_WF_ARTWORK_REQUEST_REQUEST param, ARTWORKEntities context)
        {
            string P_STYLE = ":";
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();
            ART_WF_ARTWORK_REQUEST artwork = new ART_WF_ARTWORK_REQUEST();
            ART_WF_ARTWORK_REQUEST_2 artwork2 = new ART_WF_ARTWORK_REQUEST_2();
            List<ART_WF_ARTWORK_REQUEST_2> listArtwork2 = new List<ART_WF_ARTWORK_REQUEST_2>();

            try
            {
                //using (var context = new ARTWORKEntities())
                {
                    //using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            return Results;
                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_REQUEST(ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                if (Results.data[i].CREATOR_ID > 0)
                                {
                                    Results.data[i].CREATOR_NAME = CNService.GetUserName(Results.data[i].CREATOR_ID, context);
                                }

                                if (Results.data[i].UPLOAD_BY > 0)
                                {
                                    Results.data[i].ARTWORK_UPLOADED_BY_DISPLAY_TXT = CNService.GetUserName(Results.data[i].UPLOAD_BY, context);
                                }

                                Results.data[i].IS_FFC = CNService.IsFFC(Results.data[i].CREATE_BY, context);
                                if (!Results.data[i].IS_FFC) Results.data[i].IS_FFC = CNService.IsFFC(Results.data[i].UPDATE_BY, context);

                                if (Results.data[i].REFERENCE_REQUEST_ID > 0)
                                {
                                    var refRequest = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(Results.data[i].REFERENCE_REQUEST_ID, context);
                                    if (refRequest != null)
                                    {
                                        Results.data[i].REFERENCE_ARTWORK_REQUEST_DISPLAY_TXT = refRequest.ARTWORK_REQUEST_NO;
                                    }
                                }

                                if (Results.data[i].COMPANY_ID > 0)
                                {
                                    var company = SAP_M_COMPANY_SERVICE.GetByCOMPANY_ID(Results.data[i].COMPANY_ID, context);
                                    if (company != null)
                                    {
                                        Results.data[i].COMPANY_DISPLAY_TXT = company.COMPANY_CODE + ":" + company.DESCRIPTION;
                                    }
                                }

                                if (Results.data[i].SOLD_TO_ID > 0)
                                {
                                    var soldTo = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(Results.data[i].SOLD_TO_ID, context);
                                    if (soldTo != null)
                                    {
                                        Results.data[i].SOLD_TO_DISPLAY_TXT = soldTo.CUSTOMER_CODE + ":" + soldTo.CUSTOMER_NAME;
                                    }
                                }

                                if (Results.data[i].SHIP_TO_ID > 0)
                                {
                                    var shipTo = XECM_M_CUSTOMER_SERVICE.GetByCUSTOMER_ID(Results.data[i].SHIP_TO_ID, context);
                                    if (shipTo != null)
                                    {
                                        Results.data[i].SHIP_TO_DISPLAY_TXT = shipTo.CUSTOMER_CODE + ":" + shipTo.CUSTOMER_NAME;
                                    }
                                }

                                if (Results.data[i].BRAND_ID > 0)
                                {
                                    var brand = SAP_M_BRAND_SERVICE.GetByBRAND_ID(Results.data[i].BRAND_ID, context);
                                    if (brand != null)
                                    {
                                        Results.data[i].BRAND_DISPLAY_TXT = brand.MATERIAL_GROUP + ":" + brand.DESCRIPTION;
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

                                if (Results.data[i].REVIEWER_ID > 0)
                                {
                                    Results.data[i].REVIEWER_DISPLAY_TXT = CNService.GetUserName(Results.data[i].REVIEWER_ID, context);
                                }

                                if (Results.data[i].TYPE_OF_PRODUCT_ID > 0)
                                {
                                    var typeOfProduct = SAP_M_TYPE_OF_PRODUCT_SERVICE.GetByTYPE_OF_PRODUCT_ID(Results.data[i].TYPE_OF_PRODUCT_ID, context);
                                    if (typeOfProduct != null)
                                    {
                                        Results.data[i].TYPE_OF_PRODUCT_DISPLAY_TXT = typeOfProduct.TYPE_OF_PRODUCT + ":" + typeOfProduct.DESCRIPTION;
                                    }
                                }

                                ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER mailToCust_2 = new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER();
                                mailToCust_2.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;
                                Results.data[i].MAIL_TO_CUSTOMER = MapperServices.ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER(ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.GetByItem(mailToCust_2, context));

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

                                ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2 plant_2 = new ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2();
                                plant_2.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;
                                Results.data[i].PRODUCTION_PLANT = MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT(ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT(plant_2), context));

                                for (int iPlant = 0; iPlant < Results.data[i].PRODUCTION_PLANT.Count; iPlant++)
                                {
                                    var listPlant = SAP_M_PLANT_SERVICE.GetByPLANT_ID(Results.data[i].PRODUCTION_PLANT[iPlant].PRODUCTION_PLANT_ID, context);
                                    if (listPlant != null)
                                    {
                                        Results.data[i].PRODUCTION_PLANT[iPlant].PRODUCTION_PLANT_DISPLAY_TXT = listPlant.PLANT + ":" + listPlant.NAME;
                                    }
                                }

                                ART_WF_ARTWORK_REQUEST_COUNTRY_2 country_2 = new ART_WF_ARTWORK_REQUEST_COUNTRY_2();
                                country_2.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;
                                Results.data[i].COUNTRY = MapperServices.ART_WF_ARTWORK_REQUEST_COUNTRY(ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST_COUNTRY(country_2), context));

                                for (int iCountry = 0; iCountry < Results.data[i].COUNTRY.Count; iCountry++)
                                {
                                    var listCountry = SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(Results.data[i].COUNTRY[iCountry].COUNTRY_ID, context);
                                    if (listCountry != null)
                                    {
                                        Results.data[i].COUNTRY[iCountry].COUNTRY_DISPLAY_TXT = listCountry.COUNTRY_CODE + ":" + listCountry.NAME;
                                    }
                                }

                                if (Results.data[i].PRIMARY_TYPE_ID > 0)
                                {
                                    Results.data[i].PRIMARY_TYPE_DISPLAY_TXT = CNService.GetCharacteristicDescription(Results.data[i].PRIMARY_TYPE_ID, context);
                                }

                                if (Results.data[i].THREE_P_ID > 0)
                                {
                                    var temp3p = SAP_M_3P_SERVICE.GetByTHREE_P_ID(Results.data[i].THREE_P_ID, context);
                                    if (temp3p != null)
                                    {
                                        Results.data[i].THREE_P_DISPLAY_TXT = temp3p.PRIMARY_SIZE_DESCRIPTION + P_STYLE + temp3p.CONTAINER_TYPE_DESCRIPTION + P_STYLE + temp3p.LID_TYPE_DESCRIPTION;
                                        Results.data[i].PRIMARY_SIZE_DISPLAY_TXT = temp3p.PRIMARY_SIZE_DESCRIPTION;
                                        Results.data[i].CONTAINER_TYPE_DISPLAY_TXT = temp3p.CONTAINER_TYPE_DESCRIPTION;
                                        Results.data[i].LID_TYPE_DISPLAY_TXT = temp3p.LID_TYPE_DESCRIPTION;
                                    }
                                }
                                else
                                {
                                    Results.data[i].PRIMARY_SIZE_DISPLAY_TXT = CNService.GetCharacteristicDescription(Results.data[i].PRIMARY_SIZE_ID, context);
                                    Results.data[i].CONTAINER_TYPE_DISPLAY_TXT = CNService.GetCharacteristicDescription(Results.data[i].CONTAINER_TYPE_ID, context);
                                    Results.data[i].LID_TYPE_DISPLAY_TXT = CNService.GetCharacteristicDescription(Results.data[i].LID_TYPE_ID, context);
                                }

                                if (Results.data[i].TWO_P_ID > 0)
                                {
                                    var temp2p = SAP_M_2P_SERVICE.GetByTWO_P_ID(Results.data[i].TWO_P_ID, context);
                                    if (temp2p != null)
                                    {
                                        Results.data[i].TWO_P_DISPLAY_TXT = temp2p.PACKING_SYLE_DESCRIPTION + P_STYLE + temp2p.PACK_SIZE_DESCRIPTION;
                                        Results.data[i].PACKING_STYLE_DISPLAY_TXT = temp2p.PACKING_SYLE_DESCRIPTION;
                                        Results.data[i].PACK_SIZE_DISPLAY_TXT = temp2p.PACK_SIZE_DESCRIPTION;
                                    }
                                }
                                else
                                {
                                    Results.data[i].PACKING_STYLE_DISPLAY_TXT = CNService.GetCharacteristicDescription(Results.data[i].PACKING_STYLE_ID, context);
                                    Results.data[i].PACK_SIZE_DISPLAY_TXT = CNService.GetCharacteristicDescription(Results.data[i].PACK_SIZE_ID, context);
                                }

                                #region "Reference"
                                ART_WF_ARTWORK_REQUEST_REFERENCE_2 ref_2 = new ART_WF_ARTWORK_REQUEST_REFERENCE_2();
                                ref_2.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;

                                var references = MapperServices.ART_WF_ARTWORK_REQUEST_REFERENCE(ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST_REFERENCE(ref_2), context));
                                Results.data[i].REFERENCE = references;
                                #endregion

                                #region "Product"
                                ART_WF_ARTWORK_REQUEST_PRODUCT_2 product_2 = new ART_WF_ARTWORK_REQUEST_PRODUCT_2();
                                product_2.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;

                                var products = MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCT(ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCT(product_2), context));
                                List<XECM_M_PRODUCT_2> listProduct = new List<XECM_M_PRODUCT_2>();
                                if (products != null && products.Count > 0)
                                {
                                    XECM_M_PRODUCT product = new XECM_M_PRODUCT();
                                    XECM_M_PRODUCT_2 product2 = new XECM_M_PRODUCT_2();

                                    for (int iP = 0; iP <= products.Count - 1; iP++)
                                    {
                                        product = new XECM_M_PRODUCT();
                                        product.XECM_PRODUCT_ID = products[iP].PRODUCT_CODE_ID;

                                        var xProduct = XECM_M_PRODUCT_SERVICE.GetByItem(product, context);
                                        if (xProduct != null && xProduct.Count > 0)
                                        {

                                            for (int j = 0; j <= xProduct.Count - 1; j++)
                                            {
                                                product2 = new XECM_M_PRODUCT_2();
                                                product2 = MapperServices.XECM_M_PRODUCT(xProduct[j]);
                                                product2.PRODUCT_CODE_ID = xProduct[j].XECM_PRODUCT_ID;

                                                listProduct.Add(product2);
                                            }
                                        }
                                    }
                                }
                                Results.data[i].PRODUCT = listProduct;
                                #endregion

                                ART_WF_ARTWORK_REQUEST_SALES_ORDER_2 so_2 = new ART_WF_ARTWORK_REQUEST_SALES_ORDER_2();
                                so_2.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;
                                Results.data[i].SALES_ORDER = MapperServices.ART_WF_ARTWORK_REQUEST_SALES_ORDER(ART_WF_ARTWORK_REQUEST_SALES_ORDER_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST_SALES_ORDER(so_2), context));

                                ART_WF_ARTWORK_REQUEST_RECIPIENT_2 recipient_2 = new ART_WF_ARTWORK_REQUEST_RECIPIENT_2();
                                recipient_2.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;
                                Results.data[i].REQUEST_RECIPIENT = MapperServices.ART_WF_ARTWORK_REQUEST_RECIPIENT(ART_WF_ARTWORK_REQUEST_RECIPIENT_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST_RECIPIENT(recipient_2), context));

                                for (int iRecipient = 0; iRecipient < Results.data[i].REQUEST_RECIPIENT.Count; iRecipient++)
                                {
                                    ART_M_USER user = new ART_M_USER();
                                    user = ART_M_USER_SERVICE.GetByUSER_ID(Results.data[i].REQUEST_RECIPIENT[iRecipient].RECIPIENT_USER_ID, context);

                                    Results.data[i].REQUEST_RECIPIENT[iRecipient].RECIPIENT_USER_DISPLAY_TXT = CNService.GetUserName(Results.data[i].REQUEST_RECIPIENT[iRecipient].RECIPIENT_USER_ID, context) + " (" + user.EMAIL + ")";
                                    Results.data[i].REQUEST_RECIPIENT[iRecipient].RECIPIENT_EMAIL = user.EMAIL;
                                    Results.data[i].REQUEST_RECIPIENT[iRecipient].RECIPIENT_POSITION_DISPLAY_TXT = ART_M_POSITION_SERVICE.GetByART_M_POSITION_ID(user.POSITION_ID, context).ART_M_POSITION_NAME;
                                    Results.data[i].REQUEST_RECIPIENT[iRecipient].RECIPIENT_POSITION_ID = Convert.ToInt32(user.POSITION_ID);
                                }

                                ART_WF_ARTWORK_REQUEST_ITEM item = new ART_WF_ARTWORK_REQUEST_ITEM();
                                item.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;

                                var items = MapperServices.ART_WF_ARTWORK_REQUEST_ITEM(ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(item, context));
                                if (items != null && items.Count > 0)
                                {
                                    ART_WF_ARTWORK_PROCESS processTmp = new ART_WF_ARTWORK_PROCESS();
                                    foreach (ART_WF_ARTWORK_REQUEST_ITEM_2 item2 in items)
                                    {
                                        item2.CREATE_BY_DISPLAY_TXT = CNService.GetUserName(item2.CREATE_BY, context);
                                        processTmp = new ART_WF_ARTWORK_PROCESS();
                                        processTmp.ARTWORK_ITEM_ID = item2.ARTWORK_ITEM_ID;

                                        var temp = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(processTmp, context).FirstOrDefault();
                                        if (temp != null)
                                            item2.ARTWORK_SUB_ID = temp.ARTWORK_SUB_ID;
                                    }
                                }
                                Results.data[i].REQUEST_ITEMS = items;
                            }
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

        public static ART_WF_ARTWORK_REQUEST_RESULT SaveArtworkRequest(ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();
            ART_WF_ARTWORK_REQUEST artwork = new ART_WF_ARTWORK_REQUEST();
            ART_WF_ARTWORK_REQUEST_2 artwork2 = new ART_WF_ARTWORK_REQUEST_2();
            List<ART_WF_ARTWORK_REQUEST_2> listArtwork2 = new List<ART_WF_ARTWORK_REQUEST_2>();

            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }
                else
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (var dbContextTransaction = CNService.IsolationLevel(context))
                        {
                            context.Database.CommandTimeout = 300;

                            if (param != null && param.data != null)
                            {
                                SaveArtworkRequest(param, out artwork, out artwork2, context);

                                if (param.data.ENDTASKFORM)
                                {
                                    var SEND_BACK_MK = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_BACK_MK" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                                    var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);
                                    if (process.CURRENT_STEP_ID == SEND_BACK_MK)
                                    {
                                        //find ref
                                        var tempChecklist = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(artwork.ARTWORK_REQUEST_ID, context);
                                        //if (!string.IsNullOrEmpty(tempChecklist.REFERENCE_REQUEST_NO))
                                        {
                                            var tempListRequest = new List<ART_WF_ARTWORK_REQUEST>();
                                            if (tempChecklist.REFERENCE_REQUEST_ID == null)
                                            {
                                                tempListRequest = (from h in context.ART_WF_ARTWORK_REQUEST
                                                                   where h.REFERENCE_REQUEST_TYPE == "ARTWORK"
                                                                   && (h.REFERENCE_REQUEST_ID == artwork.ARTWORK_REQUEST_ID)
                                                                   select h).ToList();
                                            }
                                            else
                                            {
                                                tempListRequest = (from h in context.ART_WF_ARTWORK_REQUEST
                                                                   where (h.REFERENCE_REQUEST_TYPE == "ARTWORK" && h.REFERENCE_REQUEST_ID == tempChecklist.REFERENCE_REQUEST_ID)
                                                                   || (h.ARTWORK_REQUEST_ID == tempChecklist.REFERENCE_REQUEST_ID)
                                                                   select h).ToList();
                                            }

                                            foreach (var itemTempChecklist in tempListRequest)
                                            {
                                                if (itemTempChecklist.ARTWORK_REQUEST_ID != artwork.ARTWORK_REQUEST_ID)
                                                {
                                                    var oldCheklist = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(itemTempChecklist.ARTWORK_REQUEST_ID, context);
                                                    ART_WF_ARTWORK_REQUEST_2 tempArtwork2 = artwork2;
                                                    tempArtwork2.ARTWORK_REQUEST_ID = oldCheklist.ARTWORK_REQUEST_ID;
                                                    tempArtwork2.ARTWORK_REQUEST_NO = oldCheklist.ARTWORK_REQUEST_NO;
                                                    tempArtwork2.REFERENCE_REQUEST_ID = oldCheklist.REFERENCE_REQUEST_ID;
                                                    tempArtwork2.REFERENCE_REQUEST_NO = oldCheklist.REFERENCE_REQUEST_NO;
                                                    tempArtwork2.REFERENCE_REQUEST_TYPE = oldCheklist.REFERENCE_REQUEST_TYPE;
                                                    tempArtwork2.BRAND_ID = oldCheklist.BRAND_ID;
                                                    tempArtwork2.BRAND_OTHER = oldCheklist.BRAND_OTHER;

                                                    SaveRequest(tempArtwork2, context);
                                                    if (tempArtwork2.COUNTRY != null)
                                                    {
                                                        SaveRequestCountryOperation(tempArtwork2, context, itemTempChecklist.ARTWORK_REQUEST_ID);
                                                    }

                                                    if (tempArtwork2.PRODUCTION_PLANT != null)
                                                    {
                                                        SaveRequestProductionPlantOperation(tempArtwork2, context, itemTempChecklist.ARTWORK_REQUEST_ID);
                                                    }

                                                    if (tempArtwork2.PRODUCT != null)
                                                    {
                                                        SaveRequestProductOperation(tempArtwork2, context, itemTempChecklist.ARTWORK_REQUEST_ID);
                                                    }

                                                    if (tempArtwork2.REFERENCE != null)
                                                    {
                                                        SaveRequestReferenceOperation(tempArtwork2, context, itemTempChecklist.ARTWORK_REQUEST_ID);
                                                    }

                                                    if (tempArtwork2.MAIL_TO_CUSTOMER != null)
                                                    {
                                                        SaveRequestMailToCustomerOperation(tempArtwork2, context, itemTempChecklist.ARTWORK_REQUEST_ID);
                                                    }
                                                }
                                            }
                                            artwork2.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;
                                        }
                                        //end find ref
                                    }
                                }

                                listArtwork2.Add(artwork2);

                                dbContextTransaction.Commit();

                                Results.data = listArtwork2;
                            }

                            if (param.data.ENDTASKFORM)
                                EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_OTHER_SUBMIT", context);

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

        public static ART_WF_ARTWORK_REQUEST_RESULT SaveArtworkRequestForRepeat(ART_WF_ARTWORK_REQUEST_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();
            ART_WF_ARTWORK_REQUEST artwork = new ART_WF_ARTWORK_REQUEST();
            ART_WF_ARTWORK_REQUEST_2 artwork2 = new ART_WF_ARTWORK_REQUEST_2();
            List<ART_WF_ARTWORK_REQUEST_2> listArtwork2 = new List<ART_WF_ARTWORK_REQUEST_2>();

            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }
                else
                {
                    //using (var context = new ARTWORKEntities())
                    {
                        //using (var dbContextTransaction = CNService.IsolationLevel(context))
                        {
                            //context.Database.CommandTimeout = 300;

                            if (param != null && param.data != null)
                            {
                                SaveArtworkRequest(param, out artwork, out artwork2, context);

                                //if (param.data.ENDTASKFORM)
                                //{
                                //    var SEND_BACK_MK = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_BACK_MK" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                                //    var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);
                                //    if (process.CURRENT_STEP_ID == SEND_BACK_MK)
                                //    {
                                //        //find ref
                                //        var tempChecklist = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(artwork.ARTWORK_REQUEST_ID, context);
                                //        //if (!string.IsNullOrEmpty(tempChecklist.REFERENCE_REQUEST_NO))
                                //        {
                                //            var tempListRequest = new List<ART_WF_ARTWORK_REQUEST>();
                                //            if (tempChecklist.REFERENCE_REQUEST_ID == null)
                                //            {
                                //                tempListRequest = (from h in context.ART_WF_ARTWORK_REQUEST
                                //                                   where h.REFERENCE_REQUEST_TYPE == "ARTWORK"
                                //                                   && (h.REFERENCE_REQUEST_ID == artwork.ARTWORK_REQUEST_ID)
                                //                                   select h).ToList();
                                //            }
                                //            else
                                //            {
                                //                tempListRequest = (from h in context.ART_WF_ARTWORK_REQUEST
                                //                                   where (h.REFERENCE_REQUEST_TYPE == "ARTWORK" && h.REFERENCE_REQUEST_ID == tempChecklist.REFERENCE_REQUEST_ID)
                                //                                   || (h.ARTWORK_REQUEST_ID == tempChecklist.REFERENCE_REQUEST_ID)
                                //                                   select h).ToList();
                                //            }

                                //            foreach (var itemTempChecklist in tempListRequest)
                                //            {
                                //                if (itemTempChecklist.ARTWORK_REQUEST_ID != artwork.ARTWORK_REQUEST_ID)
                                //                {
                                //                    var oldCheklist = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(itemTempChecklist.ARTWORK_REQUEST_ID, context);
                                //                    ART_WF_ARTWORK_REQUEST_2 tempArtwork2 = artwork2;
                                //                    tempArtwork2.ARTWORK_REQUEST_ID = oldCheklist.ARTWORK_REQUEST_ID;
                                //                    tempArtwork2.ARTWORK_REQUEST_NO = oldCheklist.ARTWORK_REQUEST_NO;
                                //                    tempArtwork2.REFERENCE_REQUEST_ID = oldCheklist.REFERENCE_REQUEST_ID;
                                //                    tempArtwork2.REFERENCE_REQUEST_NO = oldCheklist.REFERENCE_REQUEST_NO;
                                //                    tempArtwork2.REFERENCE_REQUEST_TYPE = oldCheklist.REFERENCE_REQUEST_TYPE;
                                //                    tempArtwork2.BRAND_ID = oldCheklist.BRAND_ID;
                                //                    tempArtwork2.BRAND_OTHER = oldCheklist.BRAND_OTHER;

                                //                    SaveRequest(tempArtwork2, context);
                                //                    if (tempArtwork2.COUNTRY != null)
                                //                    {
                                //                        SaveRequestCountryOperation(tempArtwork2, context, itemTempChecklist.ARTWORK_REQUEST_ID);
                                //                    }

                                //                    if (tempArtwork2.PRODUCTION_PLANT != null)
                                //                    {
                                //                        SaveRequestProductionPlantOperation(tempArtwork2, context, itemTempChecklist.ARTWORK_REQUEST_ID);
                                //                    }

                                //                    if (tempArtwork2.PRODUCT != null)
                                //                    {
                                //                        SaveRequestProductOperation(tempArtwork2, context, itemTempChecklist.ARTWORK_REQUEST_ID);
                                //                    }

                                //                    if (tempArtwork2.REFERENCE != null)
                                //                    {
                                //                        SaveRequestReferenceOperation(tempArtwork2, context, itemTempChecklist.ARTWORK_REQUEST_ID);
                                //                    }

                                //                    if (tempArtwork2.MAIL_TO_CUSTOMER != null)
                                //                    {
                                //                        SaveRequestMailToCustomerOperation(tempArtwork2, context, itemTempChecklist.ARTWORK_REQUEST_ID);
                                //                    }
                                //                }
                                //            }
                                //            artwork2.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;
                                //        }
                                //        //end find ref
                                //    }
                                //}

                                listArtwork2.Add(artwork2);

                                //dbContextTransaction.Commit();

                                Results.data = listArtwork2;
                            }

                            //if (param.data.ENDTASKFORM)
                            //    EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_OTHER_SUBMIT", context);

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

        public static void SaveArtworkRequest(ART_WF_ARTWORK_REQUEST_REQUEST param, out ART_WF_ARTWORK_REQUEST artwork, out ART_WF_ARTWORK_REQUEST_2 artwork2, ARTWORKEntities context)
        {
            artwork2 = param.data;

            artwork = SaveRequest(artwork2, context);
            artwork2 = MapperServices.ART_WF_ARTWORK_REQUEST(artwork);

            if (param.data.REQUEST_RECIPIENT != null)
            {
                artwork2.REQUEST_RECIPIENT = param.data.REQUEST_RECIPIENT;
                SaveRequestRecipientOperation(artwork2, context, artwork.ARTWORK_REQUEST_ID);
            }

            if (param.data.COUNTRY != null)
            {
                artwork2.COUNTRY = param.data.COUNTRY;
                SaveRequestCountryOperation(artwork2, context, artwork.ARTWORK_REQUEST_ID);
            }

            if (param.data.PRODUCTION_PLANT != null)
            {
                artwork2.PRODUCTION_PLANT = param.data.PRODUCTION_PLANT;
                SaveRequestProductionPlantOperation(artwork2, context, artwork.ARTWORK_REQUEST_ID);
            }

            if (param.data.PRODUCT != null)
            {
                artwork2.PRODUCT = param.data.PRODUCT;
                SaveRequestProductOperation(artwork2, context, artwork.ARTWORK_REQUEST_ID);
            }

            if (param.data.REFERENCE != null)
            {
                artwork2.REFERENCE = param.data.REFERENCE;
                SaveRequestReferenceOperation(artwork2, context, artwork.ARTWORK_REQUEST_ID);
            }

            if (param.data.SALES_ORDER != null)
            {
                artwork2.SALES_ORDER = param.data.SALES_ORDER;
                SaveRequestSalesOrderOperation(artwork2, context, artwork.ARTWORK_REQUEST_ID);
            }

            if (param.data.SALES_ORDER_REPEAT != null)
            {
                artwork2.SALES_ORDER_REPEAT = param.data.SALES_ORDER_REPEAT;
                SaveRequestSalesOrderRepeatOperation(artwork2, context, artwork.ARTWORK_REQUEST_ID);
            }

            if (param.data.MAIL_TO_CUSTOMER != null)
            {
                artwork2.MAIL_TO_CUSTOMER = param.data.MAIL_TO_CUSTOMER;
                SaveRequestMailToCustomerOperation(artwork2, context, artwork.ARTWORK_REQUEST_ID);
            }
        }

        public static ART_WF_ARTWORK_REQUEST_RESULT SubmitUploadRequestForm(ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();
            using (var context = new ARTWORKEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    context.Database.CommandTimeout = 300;

                    var artworkRequestId = 0;
                    Results = aSubmitUploadRequestForm(param, context, ref artworkRequestId);

                    if (Results.status != "E")
                    {
                        dbContextTransaction.Commit();
                        EmailService.sendEmailArtworkRequest(artworkRequestId, "WF_SEND_TO", context);
                    }
                }
            }

            return Results;
        }

        public static ART_WF_ARTWORK_REQUEST_RESULT aSubmitUploadRequestForm(ART_WF_ARTWORK_REQUEST_REQUEST param, ARTWORKEntities context, ref int artoworkRequestId)
        {
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();
            ART_WF_ARTWORK_REQUEST artwork = new ART_WF_ARTWORK_REQUEST();
            ART_WF_ARTWORK_REQUEST_2 artwork2 = new ART_WF_ARTWORK_REQUEST_2();
            List<ART_WF_ARTWORK_REQUEST_2> listArtwork2 = new List<ART_WF_ARTWORK_REQUEST_2>();
            var tempFormNO = "";

            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }
                else
                {
                    if (param != null && param.data != null)
                    {
                        int requestID = 0;
                        ART_WF_ARTWORK_REQUEST_RESULT RequestResults = new ART_WF_ARTWORK_REQUEST_RESULT();

                        if (param.data.ARTWORK_REQUEST_ID == 0)
                        {
                            RequestResults = aSaveUploadRequestForm(param, context);
                            if (RequestResults.data != null && RequestResults.data.Count > 0)
                            {
                                requestID = RequestResults.data[0].ARTWORK_REQUEST_ID;
                            }
                        }
                        else
                        {
                            requestID = param.data.ARTWORK_REQUEST_ID;
                        }

                        string formNO = FormNumberHelper.GenArtworkRequestFormNo(context);
                        tempFormNO = formNO;
                        artwork2 = MapperServices.ART_WF_ARTWORK_REQUEST(ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(requestID, context));

                        artwork2.ARTWORK_REQUEST_NO = formNO;
                        artwork = SaveRequest(artwork2, context);
                        artwork2 = MapperServices.ART_WF_ARTWORK_REQUEST(artwork);

                        listArtwork2.Add(artwork2);

                        if (param.data.REQUEST_RECIPIENT != null)
                        {
                            artwork2.REQUEST_RECIPIENT = param.data.REQUEST_RECIPIENT;
                            SaveRequestRecipientOperation(artwork2, context, artwork.ARTWORK_REQUEST_ID);
                        }

                        //Rename folder name in CS.
                        long nodeID = 0;
                        var nodeIDTmp = artwork2.REQUEST_FORM_FOLDER_NODE_ID;

                        if (nodeIDTmp > 0)
                        {
                            nodeID = Convert.ToInt64(nodeIDTmp);
                            var token = CWSService.getAuthToken();
                            CWSService.renameFolder(nodeID, formNO, token);
                        }

                        artoworkRequestId = requestID;
                        Results.requestFormNo = tempFormNO;

                        Results.data = listArtwork2;
                        Results.status = "S";
                        Results.msg = "Request Form No : " + formNO;
                    }
                }
            }
            catch (Exception ex)
            {
                Results.requestFormNo = tempFormNO;
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_REQUEST SaveRequest(ART_WF_ARTWORK_REQUEST_2 artRequest2, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_REQUEST itemTmp = MapperServices.ART_WF_ARTWORK_REQUEST(artRequest2);

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

            var uploadby = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(itemTmp.ARTWORK_REQUEST_ID, context);
            if (uploadby != null)
            {
                itemTmp.UPLOAD_BY = uploadby.UPLOAD_BY;
                itemTmp.REQUEST_FORM_CREATE_DATE = uploadby.REQUEST_FORM_CREATE_DATE;

                //ticket 445558  by aof
                if (uploadby.TYPE_OF_ARTWORK == "REPEAT")
                //if (!CNService.IsMarketingCreatedArtworkRequest(uploadby, context)) //461704 by aof 
                {
                    if (uploadby != null)
                    {
                        itemTmp.CREATOR_ID = uploadby.CREATOR_ID;
                    }
                }
                //ticket 445558  by aof
            }

            if (itemTmp.CREATOR_ID == null)
            {
                itemTmp.CREATOR_ID = CNService.getCurrentUser(context);
            }

         
            ART_WF_ARTWORK_REQUEST_SERVICE.SaveOrUpdateNoLog(itemTmp, context);

            return itemTmp;
        }

        public static ART_WF_ARTWORK_REQUEST_ITEM_RESULT DeleteArtworkFile(ART_WF_ARTWORK_REQUEST_ITEM_REQUEST param)
        {
            ART_WF_ARTWORK_REQUEST_ITEM_RESULT Results = new ART_WF_ARTWORK_REQUEST_ITEM_RESULT();
            ART_WF_ARTWORK_REQUEST_ITEM itemFile = new ART_WF_ARTWORK_REQUEST_ITEM();


            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }
                else
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (var dbContextTransaction = CNService.IsolationLevel(context))
                        {
                            long node_id = 0;
                            node_id = Convert.ToInt64(param.data.REQUEST_FORM_FILE_NODE_ID);

                            itemFile.REQUEST_FORM_FILE_NODE_ID = node_id;
                            itemFile = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(itemFile, context).FirstOrDefault();

                            if (itemFile != null)
                            {
                                ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.DeleteByARTWORK_ITEM_ID(itemFile.ARTWORK_ITEM_ID, context);
                            }
                            //Delete file on CS
                            var token = CWSService.getAuthToken();
                            CWSService.deleteNode(node_id, token);

                            dbContextTransaction.Commit();
                        }
                    }
                }

                Results.status = "S";
                Results.msg = MessageHelper.GetMessage("MSG_001");
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static ART_WF_ARTWORK_REQUEST_RESULT DeleteArtworkUploadFormOperation(ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();

            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }
                else
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (var dbContextTransaction = CNService.IsolationLevel(context))
                        {
                            if (param != null && param.data != null)
                            {
                                int artworkID = 0;

                                artworkID = param.data.ARTWORK_REQUEST_ID;
                                if (artworkID > 0)
                                {
                                    DeleteRequestSOByArtworkID(context, artworkID);
                                    DeleteRequestSORepeatByArtworkID(context, artworkID);
                                    DeleteRequestReferenceByArtworkID(context, artworkID);
                                    DeleteRequestProductionPlantByArtworkID(context, artworkID);
                                    DeleteRequestProductByArtworkID(context, artworkID);
                                    DeleteRequestCountryByArtworkID(context, artworkID);
                                    DeleteRequestItemByArtworkID(context, artworkID);
                                    DeleteRequestRecipientByArtworkID(context, artworkID);
                                    DeleteRequestUploadByArtworkID(context, artworkID, param.data.CREATE_BY);

                                    dbContextTransaction.Commit();
                                }
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

        private static void SaveRequestRecipientOperation(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            DeleteRequestRecipientByArtworkID(context, artworkID);

            SaveRequestRecipient(Item, context, artworkID);
        }

        private static void SaveRequestCountryOperation(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            DeleteRequestCountryByArtworkID(context, artworkID);

            SaveRequestCountry(Item, context, artworkID);
        }

        private static void SaveRequestMailToCustomerOperation(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            DeleteMailToCustomerByArtworkID(context, artworkID);

            SaveRequestMailToCustomer(Item, context, artworkID);
        }

        private static void SaveRequestProductionPlantOperation(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            DeleteRequestProductionPlantByArtworkID(context, artworkID);

            SaveRequestProductionPlant(Item, context, artworkID);
        }

        private static void SaveRequestProductOperation(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            //#INC-122795 commeted by aof 
            //DeleteRequestProductByArtworkID(context, artworkID);

            //SaveRequestProduct(Item, context, artworkID);
            //#INC-122795 rewrite by aof 
            SaveRequestProduct2(Item, context, artworkID);
        }

        private static void SaveRequestReferenceOperation(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {

            //#INC-122795 commeted by aof 
           //DeleteRequestReferenceByArtworkID(context, artworkID);

            //SaveRequestReference(Item, context, artworkID);
            //#INC-122795 rewrite by aof 
            SaveRequestReference2(Item, context, artworkID);
        }

        private static void SaveRequestSalesOrderOperation(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            DeleteRequestSOByArtworkID(context, artworkID);

            SaveRequestSalesOrder(Item, context, artworkID);
        }

        private static void SaveRequestSalesOrderRepeatOperation(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            DeleteRequestSORepeatByArtworkID(context, artworkID);

            SaveRequestSalesOrderRepeat(Item, context, artworkID);
        }

        private static void DeleteRequestUploadByArtworkID(ARTWORKEntities context, int artworkID, int UserId)
        {
            ART_WF_ARTWORK_REQUEST request = new ART_WF_ARTWORK_REQUEST();
            request.ARTWORK_REQUEST_ID = artworkID;

            var listRequest = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(request, context);

            if (listRequest != null && listRequest.Count > 0)
            {
                foreach (ART_WF_ARTWORK_REQUEST item in listRequest)
                {
                    ART_WF_ARTWORK_REQUEST_SERVICE.DeleteByARTWORK_REQUEST_ID(item.ARTWORK_REQUEST_ID, context);

                    DeleteFileCSByArtworkID(context, item.ARTWORK_REQUEST_ID, Convert.ToInt64(item.REQUEST_FORM_FOLDER_NODE_ID), UserId);
                }
            }
        }

        private static void DeleteFileCSByArtworkID(ARTWORKEntities context, int artworkID, long nodeID, int UserId)
        {
            var request = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(artworkID, context);

            if (nodeID > 0)
            {
                var token = CWSService.getAuthToken();
                CWSService.deleteNode(nodeID, token);
            }
        }

        private static void DeleteRequestItemByArtworkID(ARTWORKEntities context, int artworkID)
        {
            //ART_WF_ARTWORK_REQUEST_ITEM requestItem = new ART_WF_ARTWORK_REQUEST_ITEM();
            //requestItem.ARTWORK_REQUEST_ID = artworkID;

            //var listRequestItem = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(requestItem, context);

            //if (listRequestItem != null && listRequestItem.Count > 0)
            //{
            //    foreach (ART_WF_ARTWORK_REQUEST_ITEM item in listRequestItem)
            //    {
            //        ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.DeleteByARTWORK_ITEM_ID(item.ARTWORK_ITEM_ID, context);
            //    }
            //}

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_REQUEST_ITEM WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");
        }

        private static void DeleteRequestRecipientByArtworkID(ARTWORKEntities context, int artworkID)
        {
            //ART_WF_ARTWORK_REQUEST_RECIPIENT requestMarketing = new ART_WF_ARTWORK_REQUEST_RECIPIENT();
            //requestMarketing.ARTWORK_REQUEST_ID = artworkID;

            //var listRequestMarketing = ART_WF_ARTWORK_REQUEST_RECIPIENT_SERVICE.GetByItem(requestMarketing, context);

            //if (listRequestMarketing != null && listRequestMarketing.Count > 0)
            //{
            //    foreach (ART_WF_ARTWORK_REQUEST_RECIPIENT item in listRequestMarketing)
            //    {
            //        ART_WF_ARTWORK_REQUEST_RECIPIENT_SERVICE.DeleteByARTWORK_RECIPIENT_ID(item.ARTWORK_RECIPIENT_ID, context);
            //    }
            //}

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_REQUEST_RECIPIENT WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");
        }

        private static void DeleteRequestCountryByArtworkID(ARTWORKEntities context, int artworkID)
        {
            //ART_WF_ARTWORK_REQUEST_COUNTRY requestCountry = new ART_WF_ARTWORK_REQUEST_COUNTRY();
            //requestCountry.ARTWORK_REQUEST_ID = artworkID;

            //var listRequestCountry = ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.GetByItem(requestCountry, context);

            //if (listRequestCountry != null && listRequestCountry.Count > 0)
            //{
            //    foreach (ART_WF_ARTWORK_REQUEST_COUNTRY item in listRequestCountry)
            //    {
            //        ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.DeleteByARTWORK_COUNTRY_ID(item.ARTWORK_COUNTRY_ID, context);
            //    }
            //}

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_REQUEST_COUNTRY WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");
        }

        private static void DeleteRequestProductByArtworkID(ARTWORKEntities context, int artworkID)
        {
            //ART_WF_ARTWORK_REQUEST_PRODUCT ProductItem = new ART_WF_ARTWORK_REQUEST_PRODUCT();
            //ProductItem.ARTWORK_REQUEST_ID = artworkID;

            //var listRequestProduct = ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(ProductItem, context);

            //if (listRequestProduct != null && listRequestProduct.Count > 0)
            //{
            //    foreach (ART_WF_ARTWORK_REQUEST_PRODUCT item in listRequestProduct)
            //    {
            //        ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.DeleteByARTWORK_PRODUCT_ID(item.ARTWORK_PRODUCT_ID, context);
            //    }
            //}

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_REQUEST_PRODUCT WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");
        }

        private static void DeleteRequestProductionPlantByArtworkID(ARTWORKEntities context, int artworkID)
        {
            //ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT ProductPlantItem = new ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT();
            //ProductPlantItem.ARTWORK_REQUEST_ID = artworkID;

            //var listRequestProductPlant = ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_SERVICE.GetByItem(ProductPlantItem, context);

            //if (listRequestProductPlant != null && listRequestProductPlant.Count > 0)
            //{
            //    foreach (ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT item in listRequestProductPlant)
            //    {
            //        ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_SERVICE.DeleteByARTWORK_PRODUCTION_PLANT_ID(item.ARTWORK_PRODUCTION_PLANT_ID, context);
            //    }
            //}

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");
        }

        private static void DeleteRequestReferenceByArtworkID(ARTWORKEntities context, int artworkID)
        {
            //ART_WF_ARTWORK_REQUEST_REFERENCE referenceItem = new ART_WF_ARTWORK_REQUEST_REFERENCE();
            //referenceItem.ARTWORK_REQUEST_ID = artworkID;

            //var listRequestReference = ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItem(referenceItem, context);

            //if (listRequestReference != null && listRequestReference.Count > 0)
            //{
            //    foreach (ART_WF_ARTWORK_REQUEST_REFERENCE item in listRequestReference)
            //    {
            //        ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.DeleteByARTWORK_REFERENCE_ID(item.ARTWORK_REFERENCE_ID, context);
            //    }
            //}

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_REQUEST_REFERENCE WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");
        }

        private static void DeleteRequestSOByArtworkID(ARTWORKEntities context, int artworkID)
        {
            //ART_WF_ARTWORK_REQUEST_SALES_ORDER soItem = new ART_WF_ARTWORK_REQUEST_SALES_ORDER();
            //soItem.ARTWORK_REQUEST_ID = artworkID;

            //var listRequestSO = ART_WF_ARTWORK_REQUEST_SALES_ORDER_SERVICE.GetByItem(soItem, context);

            //if (listRequestSO != null && listRequestSO.Count > 0)
            //{
            //    foreach (ART_WF_ARTWORK_REQUEST_SALES_ORDER item in listRequestSO)
            //    {
            //        ART_WF_ARTWORK_REQUEST_SALES_ORDER_SERVICE.DeleteByARTWORK_SALES_ORDER_ID(item.ARTWORK_SALES_ORDER_ID, context);
            //    }
            //}

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_REQUEST_SALES_ORDER WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");
        }

        public static void DeleteRequestSORepeatByArtworkID(ARTWORKEntities context, int artworkID)
        {
            //ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT soItem = new ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT();
            //soItem.ARTWORK_REQUEST_ID = artworkID;

            //var listRequestSO = ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_SERVICE.GetByItem(soItem, context);

            //if (listRequestSO != null && listRequestSO.Count > 0)
            //{
            //    foreach (ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT item in listRequestSO)
            //    {
            //        ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_SERVICE.DeleteByARTWORK_SALES_ORDER_REPEAT_ID(item.ARTWORK_SALES_ORDER_REPEAT_ID, context);
            //    }
            //}

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");
        }

        public static void DeleteRequestSODetailByArtworkID(ARTWORKEntities context, int artworkID)
        {
            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_SO_DETAIL WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");
        }

        public static void DeleteRequestSOAssignByArtworkID(ARTWORKEntities context, int artworkID)
        {
            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");
        }

        private static void DeleteMailToCustomerByArtworkID(ARTWORKEntities context, int artworkID)
        {
            //ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER mailToCust = new ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER();
            //mailToCust.ARTWORK_REQUEST_ID = artworkID;

            //var listMailToCust = ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.GetByItem(mailToCust, context);

            //if (listMailToCust != null && listMailToCust.Count > 0)
            //{
            //    foreach (ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER itemMailToCust in listMailToCust)
            //    {
            //        ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.DeleteByARTWORK_OTHER_CUSTOMER_ID(itemMailToCust.ARTWORK_OTHER_CUSTOMER_ID, context);
            //    }
            //}

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");
        }

        private static void SaveRequestRecipient(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            if (Item.REQUEST_RECIPIENT != null && Item.REQUEST_RECIPIENT.Count > 0)
            {
                foreach (ART_WF_ARTWORK_REQUEST_RECIPIENT_2 item in Item.REQUEST_RECIPIENT)
                {
                    item.ARTWORK_REQUEST_ID = artworkID;
                    ART_WF_ARTWORK_REQUEST_RECIPIENT_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_REQUEST_MARKETING(item), context);
                }
            }
        }

        private static void SaveRequestCountry(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            if (Item.COUNTRY != null && Item.COUNTRY.Count > 0)
            {
                foreach (ART_WF_ARTWORK_REQUEST_COUNTRY_2 item in Item.COUNTRY)
                {
                    item.ARTWORK_REQUEST_ID = artworkID;
                    ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_REQUEST_COUNTRY(item), context);
                }
            }
        }

        private static void SaveRequestProduct(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            if (Item.PRODUCT != null && Item.PRODUCT.Count > 0)
            {
                ART_WF_ARTWORK_REQUEST_PRODUCT product = new ART_WF_ARTWORK_REQUEST_PRODUCT();
                foreach (XECM_M_PRODUCT_2 item in Item.PRODUCT)
                {
                    product = new ART_WF_ARTWORK_REQUEST_PRODUCT();
                    product.PRODUCT_CODE_ID = item.PRODUCT_CODE_ID;
                    product.PRODUCT_TYPE = item.PRODUCT_TYPE;
                    product.ARTWORK_REQUEST_ID = artworkID;
                    product.CREATE_BY = item.CREATE_BY;
                    product.UPDATE_BY = item.UPDATE_BY;
                    ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.SaveOrUpdateNoLog(product, context);
                }
            }
        }

        private static void SaveRequestProduct2(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
              //#INC-122795 rewrite by aof 
            ART_WF_ARTWORK_REQUEST_PRODUCT ProductItemOld = new ART_WF_ARTWORK_REQUEST_PRODUCT();
            ProductItemOld.ARTWORK_REQUEST_ID = artworkID;

            var listRequestProductOld = ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(ProductItemOld, context);



            if (Item.PRODUCT != null && Item.PRODUCT.Count > 0)
            {
                ART_WF_ARTWORK_REQUEST_PRODUCT product = new ART_WF_ARTWORK_REQUEST_PRODUCT();
                List<int> listID = new List<int>();
                listID.Add(-99);
                foreach (XECM_M_PRODUCT_2 item in Item.PRODUCT)
                {
                    var productIsExist = listRequestProductOld.Where(w => w.PRODUCT_CODE_ID == item.PRODUCT_CODE_ID && w.PRODUCT_TYPE == item.PRODUCT_TYPE).FirstOrDefault();

                    product = new ART_WF_ARTWORK_REQUEST_PRODUCT();

                    if (productIsExist != null && productIsExist.ARTWORK_PRODUCT_ID > 0)
                    {
                        product.ARTWORK_PRODUCT_ID = productIsExist.ARTWORK_PRODUCT_ID;
                        product.PRODUCT_CODE_ID = productIsExist.PRODUCT_CODE_ID;
                        product.PRODUCT_TYPE = productIsExist.PRODUCT_TYPE;
                        product.ARTWORK_REQUEST_ID = artworkID;
                        product.CREATE_BY = productIsExist.CREATE_BY;
                        product.UPDATE_BY = productIsExist.UPDATE_BY;

                        listID.Add(productIsExist.ARTWORK_PRODUCT_ID);
                    }
                    else

                    {
                        product.PRODUCT_CODE_ID = item.PRODUCT_CODE_ID;
                        product.PRODUCT_TYPE = item.PRODUCT_TYPE;
                        product.ARTWORK_REQUEST_ID = artworkID;
                        product.CREATE_BY = item.CREATE_BY;
                        product.UPDATE_BY = item.UPDATE_BY;
                    }                   
                    ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.SaveOrUpdateNoLog(product, context);
                }

                if (listRequestProductOld != null && listRequestProductOld.Count() > 0)
                {
                    var listDeleteProduct = listRequestProductOld.Where(w => !listID.Contains(w.ARTWORK_PRODUCT_ID)).ToList();
                    if (listDeleteProduct != null && listDeleteProduct.Count > 0)
                    {
                        foreach (var delProduct in listDeleteProduct)
                        {
                            ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.DeleteByARTWORK_PRODUCT_ID(delProduct.ARTWORK_PRODUCT_ID, context);
                        }
                    }
                }

            }
            else
            {
                context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_REQUEST_PRODUCT WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");
            }
        }

        private static void SaveRequestReference(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            if (Item.REFERENCE != null && Item.REFERENCE.Count > 0)
            {
                ART_WF_ARTWORK_REQUEST_REFERENCE tempRef = new ART_WF_ARTWORK_REQUEST_REFERENCE();

                foreach (ART_WF_ARTWORK_REQUEST_REFERENCE_2 item in Item.REFERENCE)
                {
                    tempRef = new ART_WF_ARTWORK_REQUEST_REFERENCE();
                    tempRef.REFERENCE_NO = item.REFERENCE_NO;
                    tempRef.PRODUCT_TYPE = item.PRODUCT_TYPE;
                    tempRef.DRAINED_WEIGHT = item.DRAINED_WEIGHT;
                    tempRef.NET_WEIGHT = item.NET_WEIGHT;
                    tempRef.ARTWORK_REQUEST_ID = artworkID;
                    tempRef.CREATE_BY = item.CREATE_BY;
                    tempRef.UPDATE_BY = item.UPDATE_BY;
                    ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.SaveOrUpdateNoLog(tempRef, context);
                }
            }
        }

        private static void SaveRequestReference2(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            // BY AOF
            ART_WF_ARTWORK_REQUEST_REFERENCE referenceItemOld = new ART_WF_ARTWORK_REQUEST_REFERENCE();
            referenceItemOld.ARTWORK_REQUEST_ID = artworkID;

            var listRequestReferenceOld = ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItem(referenceItemOld, context);

       
            if (Item.REFERENCE != null && Item.REFERENCE.Count > 0)
            {
                ART_WF_ARTWORK_REQUEST_REFERENCE tempRef = new ART_WF_ARTWORK_REQUEST_REFERENCE();

                List<int> listRefID = new List<int>();
                listRefID.Add(-99);
                foreach (ART_WF_ARTWORK_REQUEST_REFERENCE_2 item in Item.REFERENCE)
                {
                    tempRef = new ART_WF_ARTWORK_REQUEST_REFERENCE();


                    var refIsExist = listRequestReferenceOld.Where(w => w.REFERENCE_NO == item.REFERENCE_NO && w.NET_WEIGHT == item.NET_WEIGHT && w.DRAINED_WEIGHT == item.DRAINED_WEIGHT).FirstOrDefault();
                    if (refIsExist != null && refIsExist.ARTWORK_REFERENCE_ID > 0)
                    {
                        tempRef.ARTWORK_REFERENCE_ID = refIsExist.ARTWORK_REFERENCE_ID;
                        tempRef.REFERENCE_NO = refIsExist.REFERENCE_NO;
                        tempRef.PRODUCT_TYPE = refIsExist.PRODUCT_TYPE;
                        tempRef.DRAINED_WEIGHT = refIsExist.DRAINED_WEIGHT;
                        tempRef.NET_WEIGHT = refIsExist.NET_WEIGHT;
                        tempRef.ARTWORK_REQUEST_ID = artworkID;
                        tempRef.CREATE_BY = refIsExist.CREATE_BY;
                        tempRef.UPDATE_BY = refIsExist.UPDATE_BY;

                        listRefID.Add(refIsExist.ARTWORK_REFERENCE_ID);
                    }
                    else
                    {

                        tempRef.REFERENCE_NO = item.REFERENCE_NO;
                        tempRef.PRODUCT_TYPE = item.PRODUCT_TYPE;
                        tempRef.DRAINED_WEIGHT = item.DRAINED_WEIGHT;
                        tempRef.NET_WEIGHT = item.NET_WEIGHT;
                        tempRef.ARTWORK_REQUEST_ID = artworkID;
                        tempRef.CREATE_BY = item.CREATE_BY;
                        tempRef.UPDATE_BY = item.UPDATE_BY;
                    }
                    ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.SaveOrUpdateNoLog(tempRef, context);

                }

                if (listRequestReferenceOld != null && listRequestReferenceOld.Count() > 0)
                {
                    var listDeleteRef = listRequestReferenceOld.Where(w => !listRefID.Contains(w.ARTWORK_REFERENCE_ID)).ToList();
                    if (listDeleteRef !=null && listDeleteRef.Count > 0 )
                    {
                        foreach (var delRef in listDeleteRef)
                        {
                            ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.DeleteByARTWORK_REFERENCE_ID(delRef.ARTWORK_REFERENCE_ID, context);
                        }
                    }         
                }
            }
            else
            {
                context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_REQUEST_REFERENCE WHERE ARTWORK_REQUEST_ID  = '" + artworkID + "'");

            }
        }

        private static void SaveRequestMailToCustomer(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            if (Item.MAIL_TO_CUSTOMER != null && Item.MAIL_TO_CUSTOMER.Count > 0)
            {
                foreach (ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_2 itemMailToCust in Item.MAIL_TO_CUSTOMER)
                {
                    itemMailToCust.ARTWORK_REQUEST_ID = artworkID;
                    ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER(itemMailToCust), context);
                }
            }
        }

        private static void SaveRequestProductionPlant(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            if (Item.PRODUCTION_PLANT != null && Item.PRODUCTION_PLANT.Count > 0)
            {
                foreach (ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2 itemProductionPlant in Item.PRODUCTION_PLANT)
                {
                    itemProductionPlant.ARTWORK_REQUEST_ID = artworkID;
                    ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT(itemProductionPlant), context);
                }
            }
        }

        private static void SaveRequestSalesOrder(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            if (Item.SALES_ORDER != null && Item.SALES_ORDER.Count > 0)
            {
                foreach (ART_WF_ARTWORK_REQUEST_SALES_ORDER_2 itemSalesOrder in Item.SALES_ORDER)
                {
                    itemSalesOrder.ARTWORK_REQUEST_ID = artworkID;
                    ART_WF_ARTWORK_REQUEST_SALES_ORDER_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_REQUEST_SALES_ORDER(itemSalesOrder), context);
                }
            }
        }

        private static void SaveRequestSalesOrderRepeat(ART_WF_ARTWORK_REQUEST_2 Item, ARTWORKEntities context, int artworkID)
        {
            if (Item.SALES_ORDER_REPEAT != null && Item.SALES_ORDER_REPEAT.Count > 0)
            {
                foreach (ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2 itemSalesOrder in Item.SALES_ORDER_REPEAT)
                {
                    itemSalesOrder.ARTWORK_REQUEST_ID = artworkID;
                    ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_SERVICE.SaveOrUpdateNoLog(MapperServices.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT(itemSalesOrder), context);
                }
            }
        }

        public static ART_WF_ARTWORK_REQUEST_RESULT GetArtworkRequestByArtworkRequestNo(ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            ART_WF_ARTWORK_REQUEST_RESULT Results = new ART_WF_ARTWORK_REQUEST_RESULT();

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
                            Results.data = MapperServices.ART_WF_ARTWORK_REQUEST(ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                ART_WF_ARTWORK_REQUEST_ITEM item = new ART_WF_ARTWORK_REQUEST_ITEM();
                                item.ARTWORK_REQUEST_ID = Results.data[i].ARTWORK_REQUEST_ID;

                                var items = MapperServices.ART_WF_ARTWORK_REQUEST_ITEM(ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(item, context));

                                Results.data[i].REQUEST_ITEMS = items;
                            }
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