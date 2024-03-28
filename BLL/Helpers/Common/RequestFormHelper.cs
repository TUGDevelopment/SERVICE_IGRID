using BLL.Helpers;
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
    public class RequestFormHelper
    {
        public static SEARCH_REQUEST_FORM_RESULT GetRequestFormExist(SEARCH_REQUEST_FORM_REQUEST param)
        {
            SEARCH_REQUEST_FORM_RESULT Results = new SEARCH_REQUEST_FORM_RESULT();
            SEARCH_REQUEST_FORM searchModel = new SEARCH_REQUEST_FORM();
            List<SEARCH_REQUEST_FORM> listSearchModel = new List<SEARCH_REQUEST_FORM>();
            List<ART_WF_MOCKUP_CHECK_LIST> listChecklist = new List<ART_WF_MOCKUP_CHECK_LIST>();
            List<ART_WF_ARTWORK_REQUEST> listArtwork = new List<ART_WF_ARTWORK_REQUEST>();

            try
            {
                if (param == null || param.data == null)
                {
                    Results.status = "E";
                    Results.msg = MessageHelper.GetMessage("MSG_003");
                    return Results;
                }
                else
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            ART_WF_MOCKUP_CHECK_LIST checklist = new ART_WF_MOCKUP_CHECK_LIST();

                            checklist.SOLD_TO_ID = param.data.SOLD_TO_ID;
                            checklist.SHIP_TO_ID = param.data.SHIP_TO_ID;
                            checklist.BRAND_ID = param.data.BRAND_ID;

                            if (param.data.CREATOR_ID != null)
                            {
                                checklist.CREATE_BY = Convert.ToInt32(param.data.CREATOR_ID);
                            }

                            if (param.data.COUNTRY_ID == null)
                            {
                                listChecklist = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(checklist, context);
                            }
                            else
                            {
                                ART_WF_MOCKUP_CHECK_LIST_COUNTRY country = new ART_WF_MOCKUP_CHECK_LIST_COUNTRY();
                                country.COUNTRY_ID = Convert.ToInt32(param.data.COUNTRY_ID);
                                var checklistByCountry = ART_WF_MOCKUP_CHECK_LIST_COUNTRY_SERVICE.GetByItem(country, context);
                                if (checklistByCountry.Count > 0)
                                {
                                    foreach (ART_WF_MOCKUP_CHECK_LIST_COUNTRY iCountry in checklistByCountry)
                                    {
                                        var temp = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByCHECK_LIST_ID(iCountry.CHECK_LIST_ID, context);
                                        if (temp != null)
                                            listChecklist.Add(temp);
                                    }

                                    listChecklist = CriteriaCheckList(listChecklist.AsQueryable(), checklist).ToList();
                                }
                                else
                                {
                                    listChecklist = ART_WF_MOCKUP_CHECK_LIST_SERVICE.GetByItem(checklist, context);
                                }
                            }

                            foreach (ART_WF_MOCKUP_CHECK_LIST item in listChecklist)
                            {
                                searchModel = new SEARCH_REQUEST_FORM();
                                if (!String.IsNullOrEmpty(item.CHECK_LIST_NO) && !item.CHECK_LIST_NO.StartsWith("CL-C"))
                                {
                                    searchModel.ID = item.CHECK_LIST_NO;
                                    searchModel.CHECK_LIST_ID = item.CHECK_LIST_ID;
                                    searchModel.REQUEST_FORM_NO = item.CHECK_LIST_NO;
                                    searchModel.PROJECT_NAME = item.PROJECT_NAME;
                                    searchModel.SOLD_TO_ID = param.data.SOLD_TO_ID;
                                    searchModel.SHIP_TO_ID = param.data.SHIP_TO_ID;
                                    searchModel.BRAND_ID = param.data.BRAND_ID;

                                    if (param.data.CREATOR_ID != null)
                                    {
                                        searchModel.CREATOR_ID = Convert.ToInt32(param.data.CREATOR_ID);
                                    }

                                    if (item.COMPANY_ID != null)
                                    {
                                        var temp = SAP_M_COMPANY_SERVICE.GetByCOMPANY_ID(item.COMPANY_ID, context);
                                        if (temp != null)
                                            searchModel.COMPANY_DISPLAY_TXT = temp.COMPANY_CODE + ":" + temp.DESCRIPTION;
                                    }
                                    searchModel.TYPE = "Check List";
                                    if (item.PRIMARY_TYPE_ID != null)
                                    {
                                        var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PRIMARY_TYPE_ID, context);
                                        if (temp != null)
                                            searchModel.PRIMARY_TYPE_DISPLAY_TXT = temp.DESCRIPTION;
                                    }

                                    if (item.PRIMARY_SIZE_ID != null)
                                    {
                                        var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PRIMARY_SIZE_ID, context);
                                        if (temp != null)
                                            searchModel.PRIMARY_SIZE_DISPLAY_TXT = temp.DESCRIPTION;
                                    }

                                    if (item.PACKING_STYLE_ID != null)
                                    {
                                        var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PRIMARY_SIZE_ID, context);
                                        if (temp != null)
                                            searchModel.PACKING_STYLE_DISPLAY_TXT = temp.DESCRIPTION;
                                    }

                                    if (item.PACK_SIZE_ID != null)
                                    {
                                        var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PRIMARY_SIZE_ID, context);
                                        if (temp != null)
                                            searchModel.PACK_SIZE_DISPLAY_TXT = temp.DESCRIPTION;
                                    }

                                    if (item.THREE_P_ID != null)
                                    {
                                        var temp3P = SAP_M_3P_SERVICE.GetByTHREE_P_ID(item.THREE_P_ID, context);
                                        if (temp3P != null)
                                            searchModel.PRIMARY_SIZE_DISPLAY_TXT = temp3P.PRIMARY_SIZE_DESCRIPTION;
                                    }

                                    if (item.TWO_P_ID != null)
                                    {
                                        var temp2P = SAP_M_2P_SERVICE.GetByTWO_P_ID(item.TWO_P_ID, context);
                                        if (temp2P != null)
                                        {
                                            searchModel.PACKING_STYLE_DISPLAY_TXT = temp2P.PACKING_SYLE_DESCRIPTION;
                                            searchModel.PACK_SIZE_DISPLAY_TXT = temp2P.PACK_SIZE_DESCRIPTION;
                                        }
                                    }

                                    listSearchModel.Add(searchModel);
                                }
                            }

                            #region "Artwork"
                            ART_WF_ARTWORK_REQUEST artwork = new ART_WF_ARTWORK_REQUEST();
                            artwork.SOLD_TO_ID = param.data.SOLD_TO_ID;
                            artwork.SHIP_TO_ID = param.data.SHIP_TO_ID;
                            artwork.BRAND_ID = param.data.BRAND_ID;

                            if (param.data.CREATOR_ID != null)
                            {
                                artwork.UPDATE_BY = Convert.ToInt32(param.data.CREATOR_ID);
                            }

                            if (param.data.COUNTRY_ID == null)
                            {
                                listArtwork = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(artwork, context);
                            }
                            else
                            {
                                ART_WF_ARTWORK_REQUEST_COUNTRY country = new ART_WF_ARTWORK_REQUEST_COUNTRY();
                                country.COUNTRY_ID = Convert.ToInt32(param.data.COUNTRY_ID);

                                var artworkByCountry = ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.GetByItem(country, context);
                                if (artworkByCountry.Count > 0)
                                {
                                    foreach (ART_WF_ARTWORK_REQUEST_COUNTRY iCountry in artworkByCountry)
                                    {
                                        listArtwork.Add(ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(iCountry.ARTWORK_REQUEST_ID, context));
                                    }

                                    listArtwork = CriteriaArtwork(listArtwork.AsQueryable(), artwork).ToList();
                                }
                                else
                                {
                                    listArtwork = ART_WF_ARTWORK_REQUEST_SERVICE.GetByItem(artwork, context);
                                }
                            }

                            foreach (ART_WF_ARTWORK_REQUEST item in listArtwork)
                            {
                                searchModel = new SEARCH_REQUEST_FORM();
                                if (!String.IsNullOrEmpty(item.ARTWORK_REQUEST_NO))
                                {
                                    searchModel.ID = item.ARTWORK_REQUEST_NO;
                                    searchModel.ARTWORK_ID = item.ARTWORK_REQUEST_ID;
                                    searchModel.REQUEST_FORM_NO = item.ARTWORK_REQUEST_NO;
                                    searchModel.PROJECT_NAME = item.PROJECT_NAME;

                                    searchModel.SOLD_TO_ID = param.data.SOLD_TO_ID;
                                    searchModel.SHIP_TO_ID = param.data.SHIP_TO_ID;
                                    searchModel.BRAND_ID = param.data.BRAND_ID;

                                    if (param.data.CREATOR_ID != null)
                                    {
                                        searchModel.CREATOR_ID = Convert.ToInt32(param.data.CREATOR_ID);
                                    }

                                    if (item.COMPANY_ID != null)
                                    {
                                        var temp = SAP_M_COMPANY_SERVICE.GetByCOMPANY_ID(item.COMPANY_ID, context);
                                        if (temp != null)
                                            searchModel.COMPANY_DISPLAY_TXT = temp.COMPANY_CODE + ":" + temp.DESCRIPTION;
                                    }

                                    searchModel.TYPE = "Artwork";

                                    if (item.PRIMARY_TYPE_ID != null)
                                    {
                                        var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PRIMARY_TYPE_ID, context);
                                        if (temp != null)
                                            searchModel.PRIMARY_TYPE_DISPLAY_TXT = temp.DESCRIPTION;
                                    }

                                    if (item.PRIMARY_SIZE_ID != null)
                                    {
                                        var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PRIMARY_SIZE_ID, context);
                                        if (temp != null)
                                            searchModel.PRIMARY_SIZE_DISPLAY_TXT = temp.DESCRIPTION;
                                    }

                                    if (item.PACKING_STYLE_ID != null)
                                    {
                                        var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PACKING_STYLE_ID, context);
                                        if (temp != null)
                                            searchModel.PACKING_STYLE_DISPLAY_TXT = temp.DESCRIPTION;
                                    }

                                    if (item.PACK_SIZE_ID != null)
                                    {
                                        var temp = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(item.PACK_SIZE_ID, context);
                                        if (temp != null)
                                            searchModel.PACK_SIZE_DISPLAY_TXT = temp.DESCRIPTION;
                                    }

                                    if (item.THREE_P_ID != null)
                                    {
                                        var temp3P = SAP_M_3P_SERVICE.GetByTHREE_P_ID(item.THREE_P_ID, context);
                                        if (temp3P != null)
                                            searchModel.PRIMARY_SIZE_DISPLAY_TXT = temp3P.PRIMARY_SIZE_DESCRIPTION;
                                    }

                                    if (item.TWO_P_ID != null)
                                    {
                                        var temp2P = SAP_M_2P_SERVICE.GetByTWO_P_ID(item.TWO_P_ID, context);
                                        if (temp2P != null)
                                        {
                                            searchModel.PACKING_STYLE_DISPLAY_TXT = temp2P.PACKING_SYLE_DESCRIPTION;
                                            searchModel.PACK_SIZE_DISPLAY_TXT = temp2P.PACK_SIZE_DESCRIPTION;
                                        }
                                    }

                                    ART_WF_ARTWORK_REQUEST_PRODUCT product = new ART_WF_ARTWORK_REQUEST_PRODUCT();
                                    product.ARTWORK_REQUEST_ID = item.ARTWORK_REQUEST_ID;
                                    product = ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(product, context).FirstOrDefault();

                                    if (product != null)
                                    {
                                        XECM_M_PRODUCT xProduct = new XECM_M_PRODUCT();
                                        xProduct = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(product.PRODUCT_CODE_ID, context);

                                        if (xProduct != null)
                                        {
                                            searchModel.PACKING_STYLE_DISPLAY_TXT = xProduct.PACKING_STYLE;
                                            searchModel.PACK_SIZE_DISPLAY_TXT = xProduct.PACK_SIZE;
                                            searchModel.PRIMARY_SIZE_DISPLAY_TXT = xProduct.PRIMARY_SIZE;
                                        }
                                    }

                                    listSearchModel.Add(searchModel);
                                }
                            }
                            #endregion
                        }
                    }
                }

                Results.data = listSearchModel;
                Results.recordsFiltered = listSearchModel.Count;
                Results.recordsTotal = listSearchModel.Count;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        private static IQueryable<ART_WF_MOCKUP_CHECK_LIST> CriteriaCheckList(IQueryable<ART_WF_MOCKUP_CHECK_LIST> q, ART_WF_MOCKUP_CHECK_LIST criteria)
        {
            if (!CheckValue.EqualsDefaultValue(criteria.SOLD_TO_ID))
            {
                q = (from r in q where r.SOLD_TO_ID == criteria.SOLD_TO_ID select r);
            }

            if (!CheckValue.EqualsDefaultValue(criteria.SHIP_TO_ID))
            {
                q = (from r in q where r.SHIP_TO_ID == criteria.SHIP_TO_ID select r);
            }

            if (!CheckValue.EqualsDefaultValue(criteria.BRAND_ID))
            {
                q = (from r in q where r.BRAND_ID == criteria.BRAND_ID select r);
            }

            if (!CheckValue.EqualsDefaultValue(criteria.CREATE_BY))
            {
                q = (from r in q where r.CREATE_BY == criteria.CREATE_BY select r);
            }

            return q;
        }

        private static IQueryable<ART_WF_ARTWORK_REQUEST> CriteriaArtwork(IQueryable<ART_WF_ARTWORK_REQUEST> q, ART_WF_ARTWORK_REQUEST criteria)
        {
            if (!CheckValue.EqualsDefaultValue(criteria.SOLD_TO_ID))
            {
                q = (from r in q where r.SOLD_TO_ID == criteria.SOLD_TO_ID select r);
            }

            if (!CheckValue.EqualsDefaultValue(criteria.SHIP_TO_ID))
            {
                q = (from r in q where r.SHIP_TO_ID == criteria.SHIP_TO_ID select r);
            }

            if (!CheckValue.EqualsDefaultValue(criteria.BRAND_ID))
            {
                q = (from r in q where r.BRAND_ID == criteria.BRAND_ID select r);
            }

            if (!CheckValue.EqualsDefaultValue(criteria.CREATE_BY))
            {
                q = (from r in q where r.UPDATE_BY == criteria.CREATE_BY select r);
            }

            return q;
        }


    }
}
