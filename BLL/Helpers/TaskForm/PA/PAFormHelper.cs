using BLL.DocumentManagement;
using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using WebServices.Helper;



namespace BLL.Helpers
{
    public class PAFormHelper
    {
        public static ART_WF_ARTWORK_PROCESS_PA_RESULT GetPAForm(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            string P_STYLE = ":";
            string msgMatchingWarning = "";

            ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();
            ART_WF_ARTWORK_PROCESS process = new ART_WF_ARTWORK_PROCESS();
            ART_WF_ARTWORK_REQUEST request = new ART_WF_ARTWORK_REQUEST();
            ART_WF_ARTWORK_PROCESS_PA paData = new ART_WF_ARTWORK_PROCESS_PA();
            ART_WF_ARTWORK_PROCESS_PA_2 paData_2 = new ART_WF_ARTWORK_PROCESS_PA_2();
            List<ART_WF_ARTWORK_PROCESS_PA_2> listPAData_2 = new List<ART_WF_ARTWORK_PROCESS_PA_2>();
            ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2 paData_Product_2 = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2();
            List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2> listPAData_Product_2 = new List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2>();
            List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2> listPAData_ProductOther_2 = new List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2>();
         

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        paData = MapperServices.ART_WF_ARTWORK_PROCESS_PA(param.data);
                        paData_2 = MapperServices.ART_WF_ARTWORK_PROCESS_PA(ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(paData, context).FirstOrDefault());

                        var reqID = CNService.FindArtworkRequestId(paData.ARTWORK_SUB_ID, context);

                        if (reqID > 0)
                        {
                            var artworkRequest = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(reqID, context);

                            if (artworkRequest != null)
                            {
                                if (artworkRequest.TYPE_OF_ARTWORK == "REPEAT" && paData_2.IS_RETRIEVE_BY_AW_REPEAT != "X")
                                {
                                    string materialNo = "";
                                    string artworkNo = "";

                                    materialNo = paData_2.MATERIAL_NO;

                                    if (String.IsNullOrEmpty(paData_2.ARTWORK_NO))
                                    {
                                        var tempProcess = (from p in context.ART_WF_ARTWORK_PROCESS
                                                           where p.ARTWORK_SUB_ID == paData_2.ARTWORK_SUB_ID
                                                           select p).FirstOrDefault();

                                        var requestItem = (from r in context.ART_WF_ARTWORK_REQUEST_ITEM
                                                           where r.ARTWORK_ITEM_ID == tempProcess.ARTWORK_ITEM_ID
                                                           select r).FirstOrDefault();

                                        artworkNo = requestItem.REQUEST_ITEM_NO;
                                    }
                                    else
                                    {
                                        artworkNo = paData_2.ARTWORK_NO;
                                    }

                                    ART_WF_ARTWORK_PROCESS_PA_REQUEST paramTmp = new ART_WF_ARTWORK_PROCESS_PA_REQUEST();
                                    ART_WF_ARTWORK_PROCESS_PA_RESULT resultTmp = new ART_WF_ARTWORK_PROCESS_PA_RESULT();
                                    ART_WF_ARTWORK_PROCESS_PA_2 dataTmp = new ART_WF_ARTWORK_PROCESS_PA_2();

                                    dataTmp.ARTWORK_SUB_ID = paData_2.ARTWORK_SUB_ID;
                                    dataTmp.MATERIAL_NO = materialNo;
                                    dataTmp.ARTWORK_NO = artworkNo;

                                    paramTmp.data = dataTmp;

                                    resultTmp = RetriveMaterial_Repeat(paramTmp, context);

                                    if (resultTmp.status == "S")
                                    {
                                        paData_2 = new ART_WF_ARTWORK_PROCESS_PA_2();
                                        paData_2 = MapperServices.ART_WF_ARTWORK_PROCESS_PA(ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(paData, context).FirstOrDefault());

                                        paData_2.IS_RETRIEVE_BY_AW_REPEAT = "X";
                                        ART_WF_ARTWORK_PROCESS_PA paRetrieve = new ART_WF_ARTWORK_PROCESS_PA();
                                        paRetrieve = MapperServices.ART_WF_ARTWORK_PROCESS_PA(paData_2);

                                        ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(paRetrieve, context);
                                    }
                                }
                            }
                        }

                        dbContextTransaction.Commit();
                    }
                }

                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            paData_2 = new ART_WF_ARTWORK_PROCESS_PA_2();
                            paData_2 = MapperServices.ART_WF_ARTWORK_PROCESS_PA(ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(paData, context).FirstOrDefault());

                            process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);

                            if (process != null)
                            {
                                request = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(process.ARTWORK_REQUEST_ID, context);
                            }

                            if (paData_2 != null)
                            {
                                #region "Get Display Text"

                                var boms = (from b in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                            where b.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                                && b.BOM_ID > 0
                                            select b).FirstOrDefault();

                                if (boms != null)
                                {
                                    if (boms.BOM_ID > 0)
                                    {
                                        var bomDesc = (from d in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                                                       where d.PO_COMPLETE_SO_ITEM_COMPONENT_ID == boms.BOM_ID
                                                       select d).FirstOrDefault();

                                        if (bomDesc != null)
                                        {
                                            string newItem = bomDesc.BOM_ITEM_CUSTOM_1;

                                            newItem = SalesOrderHelper.GetBomCustom1Value(newItem);
                                            var characList = (from c in context.SAP_M_CHARACTERISTIC
                                                              where c.NAME == "ZPKG_SEC_GROUP"
                                                              select c).Distinct().ToList();

                                            var charac = characList.Where(c => newItem.Contains(c.DESCRIPTION)).FirstOrDefault();

                                            if (charac != null)
                                            {
                                                paData_2.MATERIAL_GROUP_ID = charac.CHARACTERISTIC_ID;
                                                paData_2.MATERIAL_GROUP_FROMSO_DISPLAY_TXT = CNService.GetCharacteristicCodeAndDescription(charac.CHARACTERISTIC_ID, context);
                                                paData_2.MATERIAL_GROUP_CODE = CNService.GetCharacteristicCode(charac.CHARACTERISTIC_ID, context);
                                            }
                                            else
                                            {
                                                paData_2.MATERIAL_GROUP_DISPLAY_TXT = CNService.GetCharacteristicCodeAndDescription(paData_2.MATERIAL_GROUP_ID, context);
                                                paData_2.MATERIAL_GROUP_CODE = CNService.GetCharacteristicCode(paData_2.MATERIAL_GROUP_ID, context);
                                            }
                                        }
                                        else
                                        {
                                            paData_2.MATERIAL_GROUP_DISPLAY_TXT = CNService.GetCharacteristicCodeAndDescription(paData_2.MATERIAL_GROUP_ID, context);
                                            paData_2.MATERIAL_GROUP_CODE = CNService.GetCharacteristicCode(paData_2.MATERIAL_GROUP_ID, context);
                                        }
                                    }
                                    else
                                    {
                                        paData_2.MATERIAL_GROUP_DISPLAY_TXT = CNService.GetCharacteristicCodeAndDescription(paData_2.MATERIAL_GROUP_ID, context);
                                        paData_2.MATERIAL_GROUP_CODE = CNService.GetCharacteristicCode(paData_2.MATERIAL_GROUP_ID, context);
                                    }
                                }
                                else
                                {
                                    paData_2.MATERIAL_GROUP_DISPLAY_TXT = CNService.GetCharacteristicCodeAndDescription(paData_2.MATERIAL_GROUP_ID, context);
                                    paData_2.MATERIAL_GROUP_CODE = CNService.GetCharacteristicCode(paData_2.MATERIAL_GROUP_ID, context);
                                }

                                if (paData_2.RD_REFERENCE_NO_ID != null && paData_2.RD_REFERENCE_NO_ID > 0)
                                {
                                    var RD_REFERENCE_NO = ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByARTWORK_REFERENCE_ID(paData_2.RD_REFERENCE_NO_ID, context);

                                    if (RD_REFERENCE_NO != null)
                                    {
                                        paData_2.RD_REFERENCE_NO_DISPLAY_TXT = RD_REFERENCE_NO.REFERENCE_NO; //CNService.GetCharacteristicDescription(_paData_2.RD_REFERENCE_NO_ID, context);
                                        paData_2.PRODUCT_TYPE = RD_REFERENCE_NO.PRODUCT_TYPE;
                                    }
                                }

                                paData_2.PA_DISPLAY_TXT = CNService.GetUserName(paData_2.PA_USER_ID, context);
                                paData_2.PG_DISPLAY_TXT = CNService.GetUserName(paData_2.PG_USER_ID, context);
                                paData_2.PRODUCT_CODE_DISPLAY_TXT = "";
                                paData_2.TYPE_OF_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.TYPE_OF_ID, context);
                                paData_2.TYPE_OF_2_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.TYPE_OF_2_ID, context);

                                if (paData_2.PRODICUTION_PLANT_ID == -1)
                                {
                                    paData_2.PRODICUTION_PLANT_DISPLAY_TXT = paData_2.PRODICUTION_PLANT_OTHER;
                                }
                                else
                                {
                                    paData_2.PRODICUTION_PLANT_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.PRODICUTION_PLANT_ID, context);

                                }
                                paData_2.PMS_COLOUR_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.PMS_COLOUR_ID, context);
                                paData_2.COMPANY_ADDRESS_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.COMPANY_ADDRESS_ID, context);
                                paData_2.CATCHING_PERIOD_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.CATCHING_PERIOD_ID, context);
                                paData_2.TOTAL_COLOUR_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.TOTAL_COLOUR_ID, context);
                                paData_2.CATCHING_METHOD_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.CATCHING_METHOD_ID, context);
                                paData_2.STYLE_OF_PRINTING_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.STYLE_OF_PRINTING_ID, context);
                                paData_2.SCIENTIFIC_NAME_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.SCIENTIFIC_NAME_ID, context);
                                paData_2.DIRECTION_OF_STICKER_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.DIRECTION_OF_STICKER_ID, context);
                                paData_2.SPECIE_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.SPECIE_ID, context);
                                paData_2.PROCESS_COLOUR_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.PROCESS_COLOUR_ID, context);
                                paData_2.PLANT_REGISTERED_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.PLANT_REGISTERED_ID, context);
                                paData_2.PRIMARY_SIZE_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.PRIMARY_SIZE_ID, context);
                                paData_2.CONTAINER_TYPE_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.CONTAINER_TYPE_ID, context);
                                paData_2.LID_TYPE_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.LID_TYPE_ID, context);
                                paData_2.PACKING_STYLE_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.PACKING_STYLE_ID, context);
                                paData_2.PACK_SIZE_DISPLAY_TXT = CNService.GetCharacteristicDescription(paData_2.PACK_SIZE_ID, context);
                                paData_2.VENDOR_BY_MIGRATION_DISPLAY_TXT = "";

                                if (!String.IsNullOrEmpty(paData_2.REFERENCE_MATERIAL))
                                {
                                    XECM_M_VENDOR vendorXECM = new XECM_M_VENDOR();

                                    var igrid = (from g in context.IGRID_M_OUTBOUND_HEADER
                                                 where g.MATERIAL_NUMBER == paData_2.REFERENCE_MATERIAL
                                                 select g).OrderByDescending(o => o.IGRID_OUTBOUND_HEADER_ID).FirstOrDefault();

                                    if (igrid != null)
                                    {
                                        var igrid_vendor = (from g in context.IGRID_M_OUTBOUND_ITEM
                                                            where g.ARTWORK_NO == igrid.ARTWORK_NO
                                                            && g.DATE == igrid.DATE
                                                            && g.TIME == igrid.TIME
                                                            && g.CHARACTERISTIC_NAME == "ZPKG_SEC_VENDOR"
                                                            select g).FirstOrDefault();

                                        if (igrid_vendor != null)
                                        {
                                            vendorXECM = context.XECM_M_VENDOR.Where(w => w.VENDOR_CODE == igrid_vendor.CHARACTERISTIC_VALUE).FirstOrDefault();

                                            if (vendorXECM != null)
                                            {
                                                paData_2.VENDOR_BY_MIGRATION_DISPLAY_TXT = vendorXECM.VENDOR_CODE + " : " + vendorXECM.VENDOR_NAME;
                                            }
                                        }
                                    }

                                    if (String.IsNullOrEmpty(paData_2.VENDOR_BY_MIGRATION_DISPLAY_TXT))
                                    {
                                        vendorXECM = CNService.GetVendorMigrationByMaterial(paData_2.REFERENCE_MATERIAL, context);
                                        if (vendorXECM != null)
                                        {
                                            paData_2.VENDOR_BY_MIGRATION_DISPLAY_TXT = vendorXECM.VENDOR_CODE + " : " + vendorXECM.VENDOR_NAME;
                                        }
                                    }
                                }

                                if (!String.IsNullOrEmpty(paData_2.MATERIAL_NO))
                                {
                                    XECM_M_VENDOR vendorXECM = new XECM_M_VENDOR();

                                    vendorXECM = CNService.GetVendorMigrationByMaterial(paData_2.MATERIAL_NO, context);
                                    if (vendorXECM != null)
                                    {
                                        paData_2.VENDOR_BY_MIGRATION_DISPLAY_TXT = vendorXECM.VENDOR_CODE + " : " + vendorXECM.VENDOR_NAME;
                                    }

                                }

                                if (paData_2.THREE_P_ID != null && paData_2.THREE_P_ID > 0)
                                {
                                    SAP_M_3P_2 temp3p = new SAP_M_3P_2();
                                    temp3p = MapperServices.SAP_M_3P(SAP_M_3P_SERVICE.GetByTHREE_P_ID(paData_2.THREE_P_ID, context));

                                    if (temp3p != null)
                                    {
                                        paData_2.THREE_P_DISPLAY_TXT = temp3p.PRIMARY_SIZE_DESCRIPTION + P_STYLE + temp3p.CONTAINER_TYPE_DESCRIPTION + P_STYLE + temp3p.LID_TYPE_DESCRIPTION;
                                        paData_2.PRIMARY_SIZE_DISPLAY_TXT = temp3p.PRIMARY_SIZE_DESCRIPTION;
                                        paData_2.CONTAINER_TYPE_DISPLAY_TXT = temp3p.CONTAINER_TYPE_DESCRIPTION;
                                        paData_2.LID_TYPE_DISPLAY_TXT = temp3p.LID_TYPE_DESCRIPTION;
                                    }
                                }

                                if (paData_2.TWO_P_ID != null && paData_2.TWO_P_ID > 0)
                                {
                                    SAP_M_2P_2 temp2p = new SAP_M_2P_2();
                                    temp2p = MapperServices.SAP_M_2P(SAP_M_2P_SERVICE.GetByTWO_P_ID(paData_2.TWO_P_ID, context));

                                    if (temp2p != null)
                                    {
                                        paData_2.TWO_P_DISPLAY_TXT = temp2p.PACKING_SYLE_DESCRIPTION + P_STYLE + temp2p.PACK_SIZE_DESCRIPTION;
                                        paData_2.PACKING_STYLE_DISPLAY_TXT = temp2p.PACKING_SYLE_DESCRIPTION;
                                        paData_2.PACK_SIZE_DISPLAY_TXT = temp2p.PACK_SIZE_DESCRIPTION;
                                    }
                                }

                                if (request != null)
                                {
                                    if (String.IsNullOrEmpty(request.BRAND_OTHER))
                                    {
                                        SAP_M_BRAND_2 brand2 = new SAP_M_BRAND_2();
                                        if (request.BRAND_ID != null)
                                        {
                                            if (request.BRAND_ID > 0)
                                            {
                                                brand2 = MapperServices.SAP_M_BRAND(SAP_M_BRAND_SERVICE.GetByBRAND_ID(request.BRAND_ID, context));
                                                paData_2.BRAND_DISPLAY_TXT = brand2.MATERIAL_GROUP + ":" + brand2.DESCRIPTION;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        paData_2.BRAND_DISPLAY_TXT = request.BRAND_OTHER;
                                        paData_2.BRAND_WARNING_DISPLAY_TXT = MessageHelper.GetMessage("MSG_023", context);
                                    }

                                    if (String.IsNullOrEmpty(request.PACKING_STYLE_OTHER))
                                    {
                                        if (request.TWO_P_ID != null)
                                        {
                                            if (request.TWO_P_ID > 0)
                                            {
                                                SAP_M_2P_2 packingStyle2P = new SAP_M_2P_2();
                                                packingStyle2P = MapperServices.SAP_M_2P(SAP_M_2P_SERVICE.GetByTWO_P_ID(request.TWO_P_ID, context));
                                                if (packingStyle2P != null)
                                                {
                                                    paData_2.PACKING_STYLE_REQUESTFORM = packingStyle2P.PACKING_SYLE_DESCRIPTION;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ART_WF_ARTWORK_REQUEST_PRODUCT_2 product_2 = new ART_WF_ARTWORK_REQUEST_PRODUCT_2();
                                            product_2.ARTWORK_REQUEST_ID = request.ARTWORK_REQUEST_ID;

                                            var products = MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCT(ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(MapperServices.ART_WF_ARTWORK_REQUEST_PRODUCT(product_2), context));

                                            if (products != null && products.Count > 0)
                                            {
                                                XECM_M_PRODUCT product = new XECM_M_PRODUCT();
                                                product = new XECM_M_PRODUCT();
                                                product.XECM_PRODUCT_ID = products[0].PRODUCT_CODE_ID;

                                                var _xProduct = XECM_M_PRODUCT_SERVICE.GetByItem(product, context);
                                                if (_xProduct != null && _xProduct.Count > 0)
                                                {
                                                    paData_2.PACKING_STYLE_REQUESTFORM = _xProduct[0].PACKING_STYLE;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        paData_2.PACKING_STYLE_REQUESTFORM = request.PACKING_STYLE_OTHER;
                                    }

                                }

                                string strCountry = "";
                                string strBrand = "";

                                ART_WF_ARTWORK_PROCESS_SO_DETAIL soDetail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
                                List<ART_WF_ARTWORK_PROCESS_SO_DETAIL> listSODetail = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL>();
                                soDetail.ARTWORK_SUB_ID = paData_2.ARTWORK_SUB_ID;
                                listSODetail = ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(soDetail, context);

                                if (listSODetail.Count > 0)
                                {
                                    SAP_M_PO_COMPLETE_SO_ITEM soItem = new SAP_M_PO_COMPLETE_SO_ITEM();
                                    SAP_M_PO_COMPLETE_SO_HEADER soHeader = new SAP_M_PO_COMPLETE_SO_HEADER();

                                    var listSALES_ORDER_NO = listSODetail.Select(m => m.SALES_ORDER_NO).ToList();
                                    var listHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                                      where listSALES_ORDER_NO.Contains(h.SALES_ORDER_NO)
                                                      select h).ToList();

                                    var listPO_COMPLETE_SO_HEADER_ID = listHeader.Select(m => m.PO_COMPLETE_SO_HEADER_ID).ToList();
                                    var listItem = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
                                                    where listPO_COMPLETE_SO_HEADER_ID.Contains(i.PO_COMPLETE_SO_HEADER_ID)
                                                    select i).ToList();

                                    foreach (ART_WF_ARTWORK_PROCESS_SO_DETAIL iSO in listSODetail)
                                    {
                                        decimal item = 0;
                                        soItem = new SAP_M_PO_COMPLETE_SO_ITEM();
                                        soHeader = new SAP_M_PO_COMPLETE_SO_HEADER();
                                        soHeader = listHeader.Where(m => m.SALES_ORDER_NO == iSO.SALES_ORDER_NO).FirstOrDefault();
                                        if (soHeader != null)
                                        {
                                            item = Convert.ToDecimal(iSO.SALES_ORDER_ITEM);
                                            soItem = listItem.Where(m => m.PO_COMPLETE_SO_HEADER_ID == soHeader.PO_COMPLETE_SO_HEADER_ID && m.ITEM == item).FirstOrDefault();
                                            if (soItem != null)
                                            {
                                                if (!strCountry.Contains(soItem.COUNTRY))
                                                {
                                                    var country = context.SAP_M_COUNTRY.Where(c => c.COUNTRY_CODE == soItem.COUNTRY).FirstOrDefault();

                                                    strCountry += country.COUNTRY_CODE + ":" + country.NAME + ", ";
                                                }

                                                if (!String.IsNullOrEmpty(soItem.BRAND_ID))
                                                {
                                                    if (!strBrand.Contains(soItem.BRAND_ID))
                                                    {
                                                        strBrand += soItem.BRAND_ID + "-";
                                                        SAP_M_BRAND_2 brand2 = new SAP_M_BRAND_2();
                                                        brand2.MATERIAL_GROUP = soItem.BRAND_ID;
                                                        brand2 = MapperServices.SAP_M_BRAND(SAP_M_BRAND_SERVICE.GetByItem(brand2, context).FirstOrDefault());

                                                        if (brand2 != null)
                                                        {
                                                            paData_2.BRAND_DISPLAY_TXT = brand2.MATERIAL_GROUP + ":" + brand2.DESCRIPTION;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (!String.IsNullOrEmpty(strCountry))
                                    {
                                        strCountry = strCountry.Substring(0, strCountry.Length - 2);
                                        paData_2.COUNTRY = strCountry;
                                    }
                                }
                                else
                                {
                                    if (request != null)
                                    {
                                        ART_WF_ARTWORK_REQUEST_COUNTRY country = new ART_WF_ARTWORK_REQUEST_COUNTRY();
                                        List<ART_WF_ARTWORK_REQUEST_COUNTRY> listCountry = new List<ART_WF_ARTWORK_REQUEST_COUNTRY>();
                                        country.ARTWORK_REQUEST_ID = request.ARTWORK_REQUEST_ID;
                                        listCountry = ART_WF_ARTWORK_REQUEST_COUNTRY_SERVICE.GetByItem(country, context);

                                        if (listCountry.Count > 0)
                                        {
                                            SAP_M_COUNTRY mCountry = new SAP_M_COUNTRY();

                                            foreach (ART_WF_ARTWORK_REQUEST_COUNTRY iCountry in listCountry)
                                            {
                                                mCountry = new SAP_M_COUNTRY();
                                                mCountry = SAP_M_COUNTRY_SERVICE.GetByCOUNTRY_ID(iCountry.COUNTRY_ID, context);

                                                strCountry += mCountry.COUNTRY_CODE + ":" + mCountry.NAME + ", ";
                                            }

                                            if (!String.IsNullOrEmpty(strCountry))
                                            {
                                                strCountry = strCountry.Substring(0, strCountry.Length - 2);
                                                paData_2.COUNTRY = strCountry;
                                            }
                                        }
                                    }
                                }

                                if (paData_2.PRINTING_STYLE_OF_PRIMARY_ID != null && paData_2.PRINTING_STYLE_OF_PRIMARY_ID > 0)
                                {
                                    ART_M_PRINTING_STYLE print = new ART_M_PRINTING_STYLE();
                                    print = MapperServices.ART_M_PRINTING_STYLE(ART_M_PRINTING_STYLE_SERVICE.GetByPRINTING_STYLE_ID(paData_2.PRINTING_STYLE_OF_PRIMARY_ID, context));
                                    paData_2.PRINTING_STYLE_OF_PRIMARY_DISPLAY_TXT = print.PRINTING_STYLE_DESCRIPTION;
                                }

                                if (paData_2.PRINTING_STYLE_OF_SECONDARY_ID != null && paData_2.PRINTING_STYLE_OF_SECONDARY_ID > 0)
                                {
                                    ART_M_PRINTING_STYLE print = new ART_M_PRINTING_STYLE();
                                    print = MapperServices.ART_M_PRINTING_STYLE(ART_M_PRINTING_STYLE_SERVICE.GetByPRINTING_STYLE_ID(paData_2.PRINTING_STYLE_OF_SECONDARY_ID, context));
                                    paData_2.PRINTING_STYLE_OF_SECONDARY_DISPLAY_TXT = print.PRINTING_STYLE_DESCRIPTION;
                                }

                                #endregion

                                #region "Get Data Detail"

                                var plantListTmp = context.ART_WF_ARTWORK_PROCESS_PA_PLANT.Where(p => p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).ToList();
                                if (plantListTmp != null && plantListTmp.Count > 0)
                                {
                                    List<ART_WF_ARTWORK_PROCESS_PA_PLANT_2> listPAPlant = new List<ART_WF_ARTWORK_PROCESS_PA_PLANT_2>();
                                    listPAPlant = MapperServices.ART_WF_ARTWORK_PROCESS_PA_PLANT(plantListTmp);
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_PLANT_2 iPlant in listPAPlant)
                                    {
                                        if (iPlant.PLANT_ID > 0)
                                        {
                                            SAP_M_PLANT_2 plant2 = new SAP_M_PLANT_2();
                                            plant2 = MapperServices.SAP_M_PLANT(SAP_M_PLANT_SERVICE.GetByPLANT_ID(iPlant.PLANT_ID, context));
                                            iPlant.DISPLAY_TXT = plant2.PLANT + " : " + plant2.NAME;
                                        }
                                        else if (!String.IsNullOrEmpty(iPlant.PLANT_OTHER))
                                        {
                                            iPlant.DISPLAY_TXT = iPlant.PLANT_OTHER;
                                        }
                                    }

                                    paData_2.PLANTS = listPAPlant;
                                }

                                var faoListTmp = context.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE.Where(p => p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).ToList();
                                if (faoListTmp != null && faoListTmp.Count > 0)
                                {
                                    List<ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_2> listPAFAO = new List<ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_2>();
                                    listPAFAO = MapperServices.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE(faoListTmp);
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_2 iFAO in listPAFAO)
                                    {
                                        if (iFAO.FAO_ZONE_ID > 0)
                                        {
                                            iFAO.DISPLAY_TXT = CNService.GetCharacteristicDescription(iFAO.FAO_ZONE_ID, context);
                                        }
                                        else if (!String.IsNullOrEmpty(iFAO.FAO_ZONE_OTHER))
                                        {
                                            iFAO.DISPLAY_TXT = iFAO.FAO_ZONE_OTHER;
                                        }
                                    }

                                    paData_2.FAOS = listPAFAO;
                                }

                                var catchingListTmp = context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA.Where(p => p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).ToList();
                                if (catchingListTmp != null && catchingListTmp.Count > 0)
                                {
                                    List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_2> listPACatching = new List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_2>();
                                    listPACatching = MapperServices.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA(catchingListTmp);
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_2 iCatch in listPACatching)
                                    {
                                        if (iCatch.CATCHING_AREA_ID > 0)
                                        {
                                            iCatch.DISPLAY_TXT = CNService.GetCharacteristicDescription(iCatch.CATCHING_AREA_ID, context);
                                        }
                                        else if (!String.IsNullOrEmpty(iCatch.CATCHING_AREA_OTHER))
                                        {
                                            iCatch.DISPLAY_TXT = iCatch.CATCHING_AREA_OTHER;
                                        }
                                    }

                                    paData_2.CATCHING_AREAS = listPACatching;
                                }

                                // ticke#425737 comented by aof 
                                var methodListTemp = context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD.Where(p => p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).ToList();
                                if (methodListTemp != null && methodListTemp.Count > 0)
                                {
                                    List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_2> listPAMethod = new List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_2>();
                                    listPAMethod = MapperServices.ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD(methodListTemp);
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_2 iMethod in listPAMethod)
                                    {
                                        if (iMethod.CATCHING_METHOD_ID >0)
                                        {
                                            iMethod.DISPLAY_TXT = CNService.GetCharacteristicDescription(iMethod.CATCHING_METHOD_ID, context);
                                        }
                                        else if (!String.IsNullOrEmpty(iMethod.CATCHING_METHOD_OTHER ))
                                        {
                                            iMethod.DISPLAY_TXT = iMethod.CATCHING_METHOD_OTHER;
                                        }
                                    }
                                    paData_2.CATCHING_METHODS = listPAMethod;
                                }

                                // ticke#425737 comented by aof 

                                var symbalListTmp = context.ART_WF_ARTWORK_PROCESS_PA_SYMBOL.Where(p => p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).ToList();
                                if (symbalListTmp != null && symbalListTmp.Count > 0)
                                {
                                    List<ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2> listPASymbol = new List<ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2>();
                                    listPASymbol = MapperServices.ART_WF_ARTWORK_PROCESS_PA_SYMBOL(symbalListTmp);
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2 iSymbol in listPASymbol)
                                    {
                                        if (iSymbol.SYMBOL_ID > 0)
                                        {
                                            iSymbol.DISPLAY_TXT = CNService.GetCharacteristicDescription(iSymbol.SYMBOL_ID, context);
                                        }
                                        else if (!String.IsNullOrEmpty(iSymbol.SYMBOL_OTHER))
                                        {
                                            iSymbol.DISPLAY_TXT = iSymbol.SYMBOL_OTHER;
                                        }
                                    }

                                    paData_2.SYMBOLS = listPASymbol;
                                }

                                if (paData_2.PRODUCT_CODE_ID != null && paData_2.PRODUCT_CODE_ID > 0)
                                {
                                    ART_WF_ARTWORK_REQUEST_PRODUCT productReq = new ART_WF_ARTWORK_REQUEST_PRODUCT();
                                    XECM_M_PRODUCT xProduct = new XECM_M_PRODUCT();
                                    xProduct = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(paData_2.PRODUCT_CODE_ID, context);

                                    if (productReq != null)
                                    {
                                        if (productReq.PRODUCT_CODE_ID > 0)
                                        {
                                            xProduct.XECM_PRODUCT_ID = productReq.PRODUCT_CODE_ID;
                                        }
                                    }
                                    else
                                    {
                                        if (paData_2.PRODUCT_CODE_ID != null)
                                        {
                                            xProduct.XECM_PRODUCT_ID = Convert.ToInt32(paData_2.PRODUCT_CODE_ID);
                                        }
                                    }

                                    xProduct = XECM_M_PRODUCT_SERVICE.GetByItem(xProduct, context).FirstOrDefault();

                                    if (xProduct != null)
                                    {
                                        paData_2.PRODUCT_CODE_DISPLAY_TXT = xProduct.PRODUCT_CODE;
                                        paData_2.PRODUCT_DESCRIPTION_DISPLAY_TXT = xProduct.PRODUCT_DESCRIPTION;
                                        paData_2.NET_WEIGHT_DISPLAY_TXT = xProduct.NET_WEIGHT;
                                        paData_2.DRAIN_WEIGHT_DISPLAY_TXT = xProduct.DRAINED_WEIGHT;

                                        //start by aof 20230121_3V_SOREPAT INC-93118
                                        var artwork_request_id = CNService.FindArtworkRequestId(param.data.ARTWORK_SUB_ID, context);
                                        var isVAP = CNService.checkRequestFormIsVAP(artwork_request_id, context);
                                        if (isVAP)
                                        {
                                            paData_2.PRODUCT_TYPE = CNService.Getcheck_product_vap(xProduct.PRODUCT_CODE, "3");
                                        }
                                        
                                        //end by aof 20230121_3V_SOREPAT INC-93118

                                        ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2 productTmp = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2();
                                        productTmp.PRODUCT_CODE = xProduct.PRODUCT_CODE;
                                        productTmp.PRODUCT_DESCRIPTION = xProduct.PRODUCT_DESCRIPTION;
                                        productTmp.NET_WEIGHT = xProduct.NET_WEIGHT;
                                        productTmp.DRAINED_WEIGHT = xProduct.DRAINED_WEIGHT;
                                        productTmp.PRODUCT_CODE_ID = Convert.ToInt32(paData_2.PRODUCT_CODE_ID);
                                        productTmp.PRODUCT_TYPE = paData_2.PRODUCT_TYPE;
                                        listPAData_Product_2.Add(productTmp);
                                    }
                                }

                                if (paData_2.RD_REFERENCE_NO_ID != null && paData_2.RD_REFERENCE_NO_ID > 0)
                                {
                                    ART_WF_ARTWORK_REQUEST_REFERENCE tempRef = new ART_WF_ARTWORK_REQUEST_REFERENCE();

                                    if (request != null)
                                    {
                                        if (paData_2.RD_REFERENCE_NO_ID != null && paData_2.RD_REFERENCE_NO_ID > 0)
                                        {
                                            tempRef.ARTWORK_REQUEST_ID = request.ARTWORK_REQUEST_ID;
                                            tempRef.ARTWORK_REFERENCE_ID = Convert.ToInt32(paData_2.RD_REFERENCE_NO_ID);
                                            
                                            tempRef = ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItem(tempRef, context).FirstOrDefault();

                                            if (tempRef != null)
                                            {
                                                paData_2.RD_REFERENCE_NO_DISPLAY_TXT = tempRef.REFERENCE_NO;
                                                paData_2.NET_WEIGHT_DISPLAY_TXT = tempRef.NET_WEIGHT;
                                                paData_2.DRAIN_WEIGHT_DISPLAY_TXT = tempRef.DRAINED_WEIGHT;
                                                paData_2.PRODUCT_TYPE = tempRef.PRODUCT_TYPE;
                                                ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2 productTmp = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2();

                                                productTmp.PRODUCT_CODE = tempRef.REFERENCE_NO;
                                                productTmp.PRODUCT_TYPE = tempRef.PRODUCT_TYPE;
                                                productTmp.NET_WEIGHT = tempRef.NET_WEIGHT;
                                                productTmp.DRAINED_WEIGHT = tempRef.DRAINED_WEIGHT;
                                                listPAData_Product_2.Add(productTmp);
                                            }
                                        }
                                    }
                                }

                                paData_Product_2.ARTWORK_SUB_PA_ID = paData_2.ARTWORK_SUB_PA_ID;

                                if (listPAData_Product_2.Count > 0)
                                {
                                    listPAData_Product_2.AddRange(MapperServices.ART_WF_ARTWORK_PROCESS_PA_PRODUCT(ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.GetByItem(paData_Product_2, context)));

                                }
                                else
                                {
                                    listPAData_Product_2 = MapperServices.ART_WF_ARTWORK_PROCESS_PA_PRODUCT(ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.GetByItem(paData_Product_2, context));
                                }

                                if (listPAData_Product_2.Count > 0)
                                {
                                    for (int i = 0; i <= listPAData_Product_2.Count - 1; i++)
                                    {
                                        XECM_M_PRODUCT xProduct = new XECM_M_PRODUCT();
                                        xProduct = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(listPAData_Product_2[i].PRODUCT_CODE_ID, context);
                                        if (xProduct != null)
                                        {
                                            listPAData_Product_2[i].DATA_FROM_DB = listPAData_Product_2[i].ARTWORK_SUB_PA_PRODUCT_ID;
                                            listPAData_Product_2[i].RF_PRODUCT_RD_REF_ID = listPAData_Product_2[i].RF_PRODUCT_RD_REF_ID;
                                            listPAData_Product_2[i].PRODUCT_CODE = xProduct.PRODUCT_CODE;
                                            listPAData_Product_2[i].PRODUCT_DESCRIPTION = xProduct.PRODUCT_DESCRIPTION;
                                            listPAData_Product_2[i].NET_WEIGHT = xProduct.NET_WEIGHT;
                                            listPAData_Product_2[i].DRAINED_WEIGHT = xProduct.DRAINED_WEIGHT;
                                            listPAData_Product_2[i].PRODUCT_TYPE = listPAData_Product_2[i].PRODUCT_TYPE;
                                            if(listPAData_Product_2[i].PRODUCT_TYPE=="VAP" && CNService.Getcheck_product_vap(xProduct.PRODUCT_CODE, "3")=="VAP_FIX")
                                            listPAData_Product_2[i].PRODUCT_TYPE = "VAP_FIX";
                                            else if(listPAData_Product_2[i].PRODUCT_TYPE == "") {//
                                                var PRODUCT_TYPE = CNService.Getcheck_product_vap(xProduct.PRODUCT_CODE, "3");
                                                listPAData_Product_2[i].PRODUCT_TYPE = PRODUCT_TYPE == "VAP" ? listPAData_Product_2[i].PRODUCT_TYPE : "NON";
                                            }
                                        }
                                    }
                                }

                                paData_2.PRODUCTS = listPAData_Product_2;

                                ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER product_other = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();
                                product_other.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                listPAData_ProductOther_2 = MapperServices.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER(ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.GetByItem(product_other, context));
                                for (int i = 0; i <= listPAData_ProductOther_2.Count - 1; i++)
                                {
                                    if (listPAData_ProductOther_2[i].RF_PRODUCT_RD_REF_ID != null)
                                    {
                                        int ref_ID = Convert.ToInt32(listPAData_ProductOther_2[i].RF_PRODUCT_RD_REF_ID);
                                        var xref = ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByARTWORK_REFERENCE_ID(ref_ID, context);
                                        listPAData_ProductOther_2[i].NET_WEIGHT = xref.NET_WEIGHT;
                                        listPAData_ProductOther_2[i].DRAINED_WEIGHT = xref.DRAINED_WEIGHT;
                                    }
                                        listPAData_ProductOther_2[i].DATA_FROM_DB = listPAData_ProductOther_2[i].ARTWORK_SUB_PA_PRODUCT_OTHER_ID;
                                        listPAData_ProductOther_2[i].PRODUCT_TYPE = listPAData_ProductOther_2[i].PRODUCT_TYPE;
                                }
                                    paData_2.PRODUCT_OTHERS = listPAData_ProductOther_2;

                                var customerStepID = context.ART_M_STEP_ARTWORK.Where(c => c.STEP_ARTWORK_CODE == "SEND_CUS_REVIEW").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
                                List<int> listSubID = CNService.FindArtworkSubId(param.data.ARTWORK_SUB_ID, context);

                                var customerProcesses = (from p in context.ART_WF_ARTWORK_PROCESS
                                                         where listSubID.Contains(p.ARTWORK_SUB_ID)
                                                          && p.CURRENT_STEP_ID == customerStepID
                                                          && p.REMARK_KILLPROCESS == null
                                                         select p).ToList();
                                if (customerProcesses != null || customerProcesses.Count > 0)
                                {
                                    var customerProcess = customerProcesses.OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();
                                    if (customerProcess != null)
                                    {
                                        var customer = (from c in context.ART_WF_ARTWORK_PROCESS_CUSTOMER
                                                        where c.ARTWORK_SUB_ID == customerProcess.ARTWORK_SUB_ID
                                                        select c).FirstOrDefault();
                                        if (customer != null)
                                        {
                                            paData_2.SHADE_LIMIT = customer.APPROVE_SHADE_LIMIT;
                                        }
                                    }
                                }

                                paData_2.DECISION_DEFAULT = ART_M_DECISION_REASON_SERVICE.GetByItem(new ART_M_DECISION_REASON { IS_DEFAULT = "X" }, context).FirstOrDefault().ART_M_DECISION_REASON_ID;

                                listPAData_2.Add(paData_2);
                                Results.data = listPAData_2;

                                #endregion
                            }

                            if (paData_2 != null)
                            {
                                var productcode = ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_PRODUCT { ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID }, context);
                                var refno = ART_WF_ARTWORK_REQUEST_REFERENCE_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_REFERENCE { ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID }, context);
                                var pa = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PA { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context);

                                if (pa != null)
                                {
                                    paData_2.CHECK_DIFFERNT_REQ_PA = false;
                                    if (productcode != null && productcode.Count > 0 && pa.FirstOrDefault().RD_REFERENCE_NO_ID != null)
                                        paData_2.CHECK_DIFFERNT_REQ_PA = true;

                                    if (refno != null && refno.Count > 0 && pa.FirstOrDefault().PRODUCT_CODE_ID != null)
                                        paData_2.CHECK_DIFFERNT_REQ_PA = true;
                                }
                            }
                        }

                        dbContextTransaction.Commit();
                    }
                }

                Results.status = "S";
                Results.msg = msgMatchingWarning;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_PA_RESULT GetChangeMaterialGroup(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();
            try
            {
                string message = "";

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        message = CheckMaterialGroupChange(param, context);
                    }
                }

                Results.status = "S";

                if (!String.IsNullOrEmpty(message))
                {
                    Results.status = "E";
                    Results.msg = message;
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }


            return Results;
        }

        public static string CheckMatchingSO_RequestForm(int artwork_sub_id, ARTWORKEntities context)
        {
            string Result = "";

            var salesorders = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                               where s.ARTWORK_SUB_ID == artwork_sub_id
                               select new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 { SALES_ORDER_NO = s.SALES_ORDER_NO, SALES_ORDER_ITEM = s.SALES_ORDER_ITEM }).FirstOrDefault();

            if (salesorders != null)
            {
                var soHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                where h.SALES_ORDER_NO == salesorders.SALES_ORDER_NO
                                select new SAP_M_PO_COMPLETE_SO_HEADER_2 { PO_COMPLETE_SO_HEADER_ID = h.PO_COMPLETE_SO_HEADER_ID, SOLD_TO = h.SOLD_TO, SHIP_TO = h.SHIP_TO }).FirstOrDefault();

                decimal itemNO = 0;
                itemNO = Convert.ToDecimal(salesorders.SALES_ORDER_ITEM);

                var soItem = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
                              where i.PO_COMPLETE_SO_HEADER_ID == soHeader.PO_COMPLETE_SO_HEADER_ID
                                && i.ITEM == itemNO
                              select new SAP_M_PO_COMPLETE_SO_ITEM_2 { BRAND_ID = i.BRAND_ID }).FirstOrDefault();

                var requestID = (from r in context.ART_WF_ARTWORK_PROCESS
                                 where r.ARTWORK_SUB_ID == artwork_sub_id
                                 select r.ARTWORK_REQUEST_ID).FirstOrDefault();

                //var ffcPosition = context.ART_M_POSITION.Where(w => w.ART_M_POSITION_CODE == "FFC").Select(s => s.ART_M_POSITION_ID).FirstOrDefault();

                var requestForm = (from r in context.ART_WF_ARTWORK_REQUEST
                                   where r.ARTWORK_REQUEST_ID == requestID
                                   select new ART_WF_ARTWORK_REQUEST_2 { SOLD_TO_ID = r.SOLD_TO_ID, SHIP_TO_ID = r.SHIP_TO_ID, BRAND_ID = r.BRAND_ID, TYPE_OF_ARTWORK = r.TYPE_OF_ARTWORK }).FirstOrDefault();

                if (soHeader != null && soItem != null && requestForm != null)
                {
                    string reqSoldTo = "";
                    string reqShipTo = "";
                    string reqBrand = "";

                    if (requestForm.SOLD_TO_ID != null)
                    {
                        reqSoldTo = (from s in context.XECM_M_CUSTOMER
                                     where s.CUSTOMER_ID == requestForm.SOLD_TO_ID
                                     select s.CUSTOMER_CODE).FirstOrDefault();
                    }

                    if (requestForm.SHIP_TO_ID != null)
                    {
                        reqShipTo = (from s in context.XECM_M_CUSTOMER
                                     where s.CUSTOMER_ID == requestForm.SHIP_TO_ID
                                     select s.CUSTOMER_CODE).FirstOrDefault();
                    }

                    if (requestForm.BRAND_ID != null)
                    {
                        reqBrand = (from s in context.SAP_M_BRAND
                                    where s.BRAND_ID == requestForm.BRAND_ID
                                    select s.MATERIAL_GROUP).FirstOrDefault();
                    }

                    var reqContriesID = (from c in context.ART_WF_ARTWORK_REQUEST_COUNTRY
                                         where c.ARTWORK_REQUEST_ID == requestID
                                         select c.COUNTRY_ID).Distinct().ToList();

                    var reqContriesCode = (from c in context.SAP_M_COUNTRY
                                           where reqContriesID.Contains(c.COUNTRY_ID)
                                           select c.COUNTRY_CODE).Distinct().ToList();

                    //var listSO = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                    //              where s.ARTWORK_SUB_ID == artwork_sub_id
                    //              select s).ToList();

                    if (requestForm.SOLD_TO_ID != null
                        && requestForm.SOLD_TO_ID > 0
                        && !String.IsNullOrEmpty(soHeader.SOLD_TO)
                        && !soHeader.SOLD_TO.Equals(reqSoldTo))
                    {
                        Result += "- <b>Sold To</b> <br>";
                    }

                    if (requestForm.SHIP_TO_ID != null
                      && requestForm.SHIP_TO_ID > 0
                      && !String.IsNullOrEmpty(soHeader.SHIP_TO)
                      && !soHeader.SHIP_TO.Equals(reqShipTo))
                    {
                        Result += "- <b>Ship To</b> <br>";
                    }

                    if (requestForm.BRAND_ID != null
                        && requestForm.BRAND_ID > 0
                        && !String.IsNullOrEmpty(soItem.BRAND_ID)
                        && !soItem.BRAND_ID.Equals(reqBrand))
                    {
                        Result += "- <b>Brand</b> <br>";
                    }

                    if (requestForm.TYPE_OF_ARTWORK.Equals("NEW"))
                    {
                        var listAssignSO = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                            where s.ARTWORK_SUB_ID == artwork_sub_id
                                            select new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 { SALES_ORDER_NO = s.SALES_ORDER_NO, SALES_ORDER_ITEM = s.SALES_ORDER_ITEM }).ToList();

                        if (listAssignSO != null && listAssignSO.Count > 0)
                        {
                            if (reqContriesCode != null && reqContriesCode.Count > 0)
                            {
                                foreach (var iSO in listAssignSO)
                                {
                                    var soHeader_C = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                                      where h.SALES_ORDER_NO == iSO.SALES_ORDER_NO
                                                      select new SAP_M_PO_COMPLETE_SO_HEADER_2 { PO_COMPLETE_SO_HEADER_ID = h.PO_COMPLETE_SO_HEADER_ID }).FirstOrDefault();

                                    itemNO = Convert.ToDecimal(iSO.SALES_ORDER_ITEM);

                                    var soItem_C = (from h in context.SAP_M_PO_COMPLETE_SO_ITEM
                                                    where h.PO_COMPLETE_SO_HEADER_ID == soHeader_C.PO_COMPLETE_SO_HEADER_ID
                                                      && h.ITEM == itemNO
                                                    select new SAP_M_PO_COMPLETE_SO_ITEM_2 { COUNTRY = h.COUNTRY }).FirstOrDefault();

                                    if (soItem_C != null && !String.IsNullOrEmpty(soItem_C.COUNTRY))
                                    {
                                        if (!String.IsNullOrEmpty(soItem_C.COUNTRY) && !reqContriesCode.Contains(soItem_C.COUNTRY))
                                        {
                                            Result += "- <b>Country</b> <br>";
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(Result))
                    {
                        Result = MessageHelper.GetMessage("MSG_024", context) + "<br>" + Result;
                        Result = Result.Substring(0, Result.Length - 4);
                    }
                }
            }

            return Result;
        }

        public static string CheckBrandRefMaterial_RequestForm(int artwork_sub_id, ARTWORKEntities context)
        {
            string Result = "";
            string refBrand = "";
            string reqBrand = "";
            string msg = "";

            DateTime dateNow = DateTime.Now.Date;
            msg = MessageHelper.GetMessage("MSG_035", context);

            var refMaterial = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                               where p.ARTWORK_SUB_ID == artwork_sub_id
                               select p.REFERENCE_MATERIAL).FirstOrDefault();

            if (!String.IsNullOrEmpty(refMaterial))
            {
                refBrand = (from p in context.SAP_M_ORDER_BOM
                            where p.MATERIAL_NUMBER == refMaterial
                                 && p.START_DATE <= dateNow
                                 && p.END_DATE >= dateNow
                                 && p.CHANGE_TYPE != "D"
                            select p.BRAND_ID).FirstOrDefault();
            }

            var requestID = CNService.FindArtworkRequestId(artwork_sub_id, context);

            var requestBrandID = (from p in context.ART_WF_ARTWORK_REQUEST
                                  where p.ARTWORK_REQUEST_ID == requestID
                                  select p.BRAND_ID).FirstOrDefault();

            if (requestBrandID != null)
            {
                reqBrand = (from p in context.SAP_M_BRAND
                            where p.BRAND_ID == requestBrandID
                            select p.MATERIAL_GROUP).FirstOrDefault();
            }

            if (!String.IsNullOrEmpty(refBrand) && !String.IsNullOrEmpty(reqBrand))
            {
                if (!reqBrand.Equals(refBrand))
                {
                    Result = msg;
                }
            }

            return Result;
        }

        private static string CheckMaterialGroupChange(ART_WF_ARTWORK_PROCESS_PA_REQUEST param, ARTWORKEntities context)
        {
            string Result = "";
            int artwork_sub_id = 0;
            int material_group_id = 0;

            artwork_sub_id = param.data.ARTWORK_SUB_ID;
            material_group_id = Convert.ToInt32(param.data.MATERIAL_GROUP_ID);

            var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                             where p.ARTWORK_SUB_ID == artwork_sub_id
                             select p).FirstOrDefault();

            var matGroupValue = CNService.GetCharacteristicCode(material_group_id, context);
            var matGroupDescription = CNService.GetCharacteristicDescription(material_group_id, context);

            var soAssign = (from p in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                            where p.ARTWORK_SUB_ID == artwork_sub_id
                            select p).FirstOrDefault();

            var soAssignBOMs = (from p in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                where p.ARTWORK_SUB_ID == artwork_sub_id
                                    && p.BOM_ID != null
                                select p.BOM_ID).ToList();

            var soBOM = (from b in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                         where soAssignBOMs.Contains(b.PO_COMPLETE_SO_ITEM_COMPONENT_ID)
                         select b).ToList();

            if (soBOM != null && soBOM.Count > 0)
            {
                string isValidateBom = "";
                var soBOMCutom = soBOM.Where(w => !String.IsNullOrEmpty(w.BOM_ITEM_CUSTOM_1)).Select(s => s.BOM_ITEM_CUSTOM_1).ToList();

                if (soBOMCutom != null)
                {
                    foreach (string item in soBOMCutom)
                    {
                        if (!String.IsNullOrEmpty(item) && (!item.Contains(matGroupDescription.ToUpper())))
                        {
                            Result += MessageHelper.GetMessage("MSG_025", context) + "<br>";
                            isValidateBom = "X";
                            break;
                        }
                    }
                }

                if (isValidateBom != "X")
                {
                    int bomID = soBOM[0].PO_COMPLETE_SO_ITEM_COMPONENT_ID;

                    string BOM_NO = CNService.GetBOMNo(bomID, context);

                    if (!String.IsNullOrEmpty(BOM_NO))
                    {
                        string bomMatGroup = "";
                        bomMatGroup = BOM_NO.Substring(1, 1);

                        if (!matGroupValue.ToUpper().Equals(bomMatGroup.ToUpper()))
                        {
                            Result += MessageHelper.GetMessage("MSG_025", context) + "<br>";
                        }
                    }
                }
            }

            var stepPG = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_PG").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();

            var itemID = CNService.FindArtworkItemId(artwork_sub_id, context);

            var processPGs = (from p in context.ART_WF_ARTWORK_PROCESS
                              where p.ARTWORK_ITEM_ID == itemID
                                 && p.CURRENT_STEP_ID == stepPG
                                 && p.REMARK_KILLPROCESS == null
                              select p).OrderByDescending(o => o.UPDATE_DATE).ToList();

            if (processPGs != null)
            {
                ART_WF_ARTWORK_PROCESS processPG_Tmp = new ART_WF_ARTWORK_PROCESS();
                if (processPGs.Count > 1)
                {
                    processPG_Tmp = processPGs.Where(w => w.IS_END == "X").OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();
                }
                else
                {
                    processPG_Tmp = processPGs.OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                }

                if (processPG_Tmp != null)
                {
                    var processPGData = (from p in context.ART_WF_ARTWORK_PROCESS_PG
                                         where p.ARTWORK_SUB_ID == processPG_Tmp.ARTWORK_SUB_ID
                                         select p).FirstOrDefault();

                    if (processPGData != null)
                    {
                        var checkListItem = (from c in context.ART_WF_MOCKUP_CHECK_LIST_ITEM
                                             where c.MOCKUP_ID == processPGData.DIE_LINE_MOCKUP_ID
                                             select c).FirstOrDefault();

                        if (checkListItem != null)
                        {
                            if (!material_group_id.Equals(checkListItem.PACKING_TYPE_ID))
                            {
                                Result += MessageHelper.GetMessage("MSG_026", context) + "<br>";
                            }
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(Result))
            {
                Result = Result.Substring(0, Result.Length - 4);
            }

            return Result;
        }

        public static ART_WF_ARTWORK_PROCESS_PA_RESULT SavePAForm(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();
            List<ART_WF_ARTWORK_PROCESS_PA_2> listPA_2 = new List<ART_WF_ARTWORK_PROCESS_PA_2>();
            ART_WF_ARTWORK_PROCESS_PA paData = new ART_WF_ARTWORK_PROCESS_PA();
            ART_WF_ARTWORK_PROCESS_PA_2 paData2 = new ART_WF_ARTWORK_PROCESS_PA_2();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            paData2 = param.data;
                            paData = MapperServices.ART_WF_ARTWORK_PROCESS_PA(paData2);

                            ART_WF_ARTWORK_PROCESS_PA paTmp = new ART_WF_ARTWORK_PROCESS_PA();
                            paTmp = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByARTWORK_SUB_PA_ID(param.data.ARTWORK_SUB_PA_ID, context);

                            if (paTmp != null)
                            {
                                paData.MATERIAL_NO = paTmp.MATERIAL_NO;
                                paData.REFERENCE_MATERIAL = paTmp.REFERENCE_MATERIAL;
                                paData.PG_USER_ID = paTmp.PG_USER_ID;
                                paData.REQUEST_MATERIAL_STATUS = paTmp.REQUEST_MATERIAL_STATUS;
                                paData.IS_RETRIEVE_BY_AW_REPEAT = paTmp.IS_RETRIEVE_BY_AW_REPEAT;
                                paData.SHADE_LIMIT = paTmp.SHADE_LIMIT;
                                paData.PRODUCT_TYPE = param.data.PRODUCT_TYPE; 
                                paData.PA_USER_ID = param.data.UPDATE_BY;

                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(paData, context);
                            }

                            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_PA_PRODUCT WHERE ARTWORK_SUB_ID  = '" + paData.ARTWORK_SUB_ID + "'");
                            if (paData2.PRODUCTS != null && paData2.PRODUCTS.Count > 0)
                            {
                                foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2 iProduct2 in paData2.PRODUCTS)
                                {
                                    var product = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                                    product = MapperServices.ART_WF_ARTWORK_PROCESS_PA_PRODUCT(iProduct2);
                                    product.ARTWORK_SUB_PA_ID = paData.ARTWORK_SUB_PA_ID;
                                    product.CREATE_BY = param.data.UPDATE_BY;
                                    product.PRODUCT_TYPE = iProduct2.PRODUCT_TYPE;
                                    product.UPDATE_BY = param.data.UPDATE_BY;
                                    ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.SaveNoLog(product, context);
                                }
                            }

                            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER WHERE ARTWORK_SUB_ID  = '" + paData.ARTWORK_SUB_ID + "'");
                            if (paData2.PRODUCT_OTHERS != null && paData2.PRODUCT_OTHERS.Count > 0)
                            {
                                foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2 iProductOther in paData2.PRODUCT_OTHERS)
                                {
                                    var product_other = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();
                                    product_other = MapperServices.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER(iProductOther);
                                    product_other.ARTWORK_SUB_PA_ID = paData.ARTWORK_SUB_PA_ID;
                                    product_other.PRODUCT_TYPE = iProductOther.PRODUCT_TYPE;
                                    product_other.CREATE_BY = param.data.UPDATE_BY;
                                    product_other.UPDATE_BY = param.data.UPDATE_BY;
                                    ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.SaveNoLog(product_other, context);
                                }
                            }

                            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_PA_PLANT WHERE ARTWORK_SUB_ID  = '" + paData.ARTWORK_SUB_ID + "'");
                            if (paData2.PLANTS != null && paData2.PLANTS.Count > 0)
                            {
                                foreach (ART_WF_ARTWORK_PROCESS_PA_PLANT_2 iProduct2 in paData2.PLANTS)
                                {
                                    var plant = new ART_WF_ARTWORK_PROCESS_PA_PLANT();
                                    plant = MapperServices.ART_WF_ARTWORK_PROCESS_PA_PLANT(iProduct2);
                                    plant.ARTWORK_SUB_PA_ID = paData.ARTWORK_SUB_PA_ID;
                                    plant.CREATE_BY = param.data.UPDATE_BY;
                                    plant.UPDATE_BY = param.data.UPDATE_BY;
                                    ART_WF_ARTWORK_PROCESS_PA_PLANT_SERVICE.SaveNoLog(plant, context);
                                }
                            }

                            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE WHERE ARTWORK_SUB_ID  = '" + paData.ARTWORK_SUB_ID + "'");
                            if (paData2.FAOS != null && paData2.FAOS.Count > 0)
                            {
                                foreach (ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_2 iFAO2 in paData2.FAOS)
                                {
                                    var fao = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                                    fao = MapperServices.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE(iFAO2);
                                    fao.ARTWORK_SUB_PA_ID = paData.ARTWORK_SUB_PA_ID;
                                    fao.CREATE_BY = param.data.UPDATE_BY;
                                    fao.UPDATE_BY = param.data.UPDATE_BY;
                                    ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.SaveNoLog(fao, context);
                                }
                            }

                            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA WHERE ARTWORK_SUB_ID  = '" + paData.ARTWORK_SUB_ID + "'");
                            if (paData2.CATCHING_AREAS != null && paData2.CATCHING_AREAS.Count > 0)
                            {
                                foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_2 iCatching2 in paData2.CATCHING_AREAS)
                                {
                                    var catching = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                                    catching = MapperServices.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA(iCatching2);
                                    catching.ARTWORK_SUB_PA_ID = paData.ARTWORK_SUB_PA_ID;
                                    catching.CREATE_BY = param.data.UPDATE_BY;
                                    catching.UPDATE_BY = param.data.UPDATE_BY;
                                    ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE.SaveNoLog(catching, context);
                                }
                            }

                            // ticke#425737 commented by aof 
                            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD WHERE ARTWORK_SUB_ID  = '" + paData.ARTWORK_SUB_ID + "'");
                            if (paData2.CATCHING_METHODS != null && paData2.CATCHING_METHODS.Count > 0)
                            {
                                foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_2 iMethod2 in paData2.CATCHING_METHODS)
                                {
                                    var method = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                                    method = MapperServices.ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD(iMethod2);
                                    method.ARTWORK_SUB_PA_ID = paData.ARTWORK_SUB_PA_ID;
                                    method.CREATE_BY = param.data.UPDATE_BY;
                                    method.UPDATE_BY = param.data.UPDATE_BY;
                                    ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE.SaveNoLog(method, context);
                                }
                            }
                            // ticke#425737 commented by aof 

                            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_PA_SYMBOL WHERE ARTWORK_SUB_ID  = '" + paData.ARTWORK_SUB_ID + "'");
                            if (paData2.SYMBOLS != null && paData2.SYMBOLS.Count > 0)
                            {
                                foreach (ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2 iSymbol2 in paData2.SYMBOLS)
                                {
                                    var symbol = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                                    symbol = MapperServices.ART_WF_ARTWORK_PROCESS_PA_SYMBOL(iSymbol2);
                                    symbol.ARTWORK_SUB_PA_ID = paData.ARTWORK_SUB_PA_ID;
                                    symbol.CREATE_BY = param.data.UPDATE_BY;
                                    symbol.UPDATE_BY = param.data.UPDATE_BY;
                                    ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE.SaveNoLog(symbol, context);
                                }
                            }

                            //update cat
                            var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(paData.ARTWORK_SUB_ID, context);
                            var ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                            var ARTWORK_ITEM_ID = process.ARTWORK_ITEM_ID;
                            if (ARTWORK_REQUEST_ID > 0)
                            {
                                var ARTWORK_REQUEST_NO = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(ARTWORK_REQUEST_ID, context).ARTWORK_REQUEST_NO;
                                var REQUEST_ITEM_NO = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(ARTWORK_ITEM_ID, context).REQUEST_ITEM_NO;
                                if (!string.IsNullOrEmpty(ARTWORK_REQUEST_NO) && !string.IsNullOrEmpty(REQUEST_ITEM_NO))
                                {
                                    var token = CWSService.getAuthToken();
                                    CWSService.updateCategoryArtwork(ARTWORK_REQUEST_NO, REQUEST_ITEM_NO, ARTWORK_REQUEST_ID, ARTWORK_ITEM_ID, token);
                                }
                            }
                        }

                        listPA_2.Add(MapperServices.ART_WF_ARTWORK_PROCESS_PA(paData));

                        Results.data = listPA_2;

                        dbContextTransaction.Commit();

                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_001", context);
                    }
                }

                CNService.UpdateMaterialLock(param.data.ARTWORK_SUB_ID);
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_PA_RESULT DeleteSuggestMaterial(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                             where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                             select p).FirstOrDefault();

                            if (processPA != null)
                            {
                                processPA.REFERENCE_MATERIAL = "";
                                processPA.UPDATE_BY = param.data.UPDATE_BY;

                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);
                            }
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

        public static ART_WF_ARTWORK_PROCESS_RESULT TerminatePAForm(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_RESULT Results = new ART_WF_ARTWORK_PROCESS_RESULT();
            ART_WF_ARTWORK_PROCESS process = new ART_WF_ARTWORK_PROCESS();
            List<ART_WF_ARTWORK_PROCESS> listProcess = new List<ART_WF_ARTWORK_PROCESS>();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            var itemProcesses = (from p in context.ART_WF_ARTWORK_PROCESS where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID select p).FirstOrDefault();

                            var processes = (from p in context.ART_WF_ARTWORK_PROCESS where p.ARTWORK_ITEM_ID == itemProcesses.ARTWORK_ITEM_ID select p).ToList();

                            processes = processes.Where(m => string.IsNullOrEmpty(m.IS_END)).ToList();

                            if (processes != null && processes.Count > 0)
                            {
                                foreach (ART_WF_ARTWORK_PROCESS iProcess in processes)
                                {
                                    iProcess.IS_END = "X";
                                    iProcess.TERMINATE_REASON_CODE = param.data.TERMINATE_REASON_CODE;
                                    if (iProcess.PARENT_ARTWORK_SUB_ID == null)
                                    {
                                        iProcess.IS_TERMINATE = "X";

                                        if (!String.IsNullOrEmpty(param.data.REMARK_TERMINATE))
                                        {
                                            iProcess.REMARK_TERMINATE = param.data.REMARK_TERMINATE;
                                        }
                                    }

                                    iProcess.UPDATE_BY = param.data.UPDATE_BY;

                                    ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(iProcess, context);
                                    //#437016 cancal
                                    //if (iProcess.CURRENT_STEP_ID == 2 && iProcess.IS_END == "X")
                                    //    CNService.CompletePOForm(param, context);
                                }

                                DeleteAssignSalesOrder(itemProcesses.ARTWORK_SUB_ID, context);
                                ArtworkUploadHelper.DeleteRequestSORepeatByArtworkID(context, itemProcesses.ARTWORK_REQUEST_ID);

                                var requestItem = (from p in context.ART_WF_ARTWORK_REQUEST_ITEM where p.ARTWORK_ITEM_ID == itemProcesses.ARTWORK_ITEM_ID select p).FirstOrDefault();
                                if (requestItem != null)
                                {
                                    var listMappingPO = (from p in context.ART_WF_ARTWORK_MAPPING_PO where p.ARTWORK_NO == requestItem.REQUEST_ITEM_NO select p).ToList();
                                    foreach (var mappingPO in listMappingPO)
                                        ART_WF_ARTWORK_MAPPING_PO_SERVICE.DeleteByARTWORK_MAPPING_PO_ID(mappingPO.ARTWORK_MAPPING_PO_ID, context);
                                }
                            }

                            dbContextTransaction.Commit();

                            EmailService.sendEmailArtwork(processes.FirstOrDefault().ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_TEMINATED", context, param.data.REMARK_TERMINATE.Replace("<p>", "").Replace("</p>", ""));
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

        public static ART_WF_ARTWORK_PROCESS_RESULT KillProcessHistory(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_RESULT Results = new ART_WF_ARTWORK_PROCESS_RESULT();
            ART_WF_ARTWORK_PROCESS process = new ART_WF_ARTWORK_PROCESS();
            List<ART_WF_ARTWORK_PROCESS> listProcess = new List<ART_WF_ARTWORK_PROCESS>();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            if (param.data.REMARK_KILLPROCESS == "<p><br></p>" || param.data.REMARK_KILLPROCESS == null)
                            {
                                Results.status = "E";
                                Results.msg = "Please fill remark for terminate this workflow.";
                                return Results;
                            }

                            var temp = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);

                            temp.IS_END = "X";
                            temp.REMARK_KILLPROCESS = param.data.REMARK_KILLPROCESS;
                            temp.UPDATE_BY = param.data.UPDATE_BY;

                            ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(temp, context);
                        }

                        dbContextTransaction.Commit();

                        EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_TEMINATED_STEP", context, param.data.REMARK_KILLPROCESS.Replace("<p>", "").Replace("</p>", ""));

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

        public static ART_WF_ARTWORK_PROCESS_RESULT CompletePAForm(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_RESULT Results = new ART_WF_ARTWORK_PROCESS_RESULT();
            ART_WF_ARTWORK_PROCESS process = new ART_WF_ARTWORK_PROCESS();
            List<ART_WF_ARTWORK_PROCESS> listProcess = new List<ART_WF_ARTWORK_PROCESS>();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            var processes = (from p in context.ART_WF_ARTWORK_PROCESS
                                             where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                              || p.PARENT_ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                              && string.IsNullOrEmpty(p.IS_END)
                                             select p).ToList();

                            if (processes != null && processes.Count > 0)
                            {
                                var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                                foreach (ART_WF_ARTWORK_PROCESS iProcess in processes)
                                {
                                    iProcess.IS_END = "X";
                                    if (iProcess.CURRENT_STEP_ID != stepId && iProcess.ARTWORK_SUB_ID != param.data.ARTWORK_SUB_ID)
                                        iProcess.REMARK_KILLPROCESS = "Completed workflow by PA";
                                    iProcess.UPDATE_BY = param.data.UPDATE_BY;
                                    ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(iProcess, context);
                                    //#437016
                                    if (iProcess.CURRENT_STEP_ID == 2 && iProcess.IS_END == "X")
                                        CNService.CompletePOForm(param, context);
                                }
                            }
                        }

                        var ARTWORK_REQUEST_ID = (from p in context.ART_WF_ARTWORK_PROCESS
                                                  where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                                  select p.ARTWORK_REQUEST_ID).FirstOrDefault();

                        var TYPE_OF_ARTWORK = (from p in context.ART_WF_ARTWORK_REQUEST
                                               where p.ARTWORK_REQUEST_ID == ARTWORK_REQUEST_ID
                                               select p.TYPE_OF_ARTWORK).FirstOrDefault();

                        if (TYPE_OF_ARTWORK == "NEW")
                        {
                            var error_msg = "";  // by aof 20220317
                            var tempProcess = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);
                            var foundPrintMaster = ArtworkProcessHelper.moveFileArtworkToMatWorkspace(tempProcess.ARTWORK_ITEM_ID, context,ref error_msg);
                            if (!foundPrintMaster)
                            {
                                Results.status = "E";
                                Results.msg = "Cannot complete workflow"; //, System not found print master file in this workflow.";
                                if (error_msg != "")  // by aof 20220317
                                {
                                    Results.msg = Results.msg + ", " + error_msg;
                                }
                                return Results;
                            }
                        }

                        dbContextTransaction.Commit();

                        EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_COMPLETED", context);

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

        public static ART_WF_ARTWORK_PROCESS_RESULT CompletePAFormForRepeat(ART_WF_ARTWORK_PROCESS_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_PROCESS_RESULT Results = new ART_WF_ARTWORK_PROCESS_RESULT();
            ART_WF_ARTWORK_PROCESS process = new ART_WF_ARTWORK_PROCESS();
            List<ART_WF_ARTWORK_PROCESS> listProcess = new List<ART_WF_ARTWORK_PROCESS>();

            try
            {
                //using (var context = new ARTWORKEntities())
                {
                    //using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            var processes = (from p in context.ART_WF_ARTWORK_PROCESS
                                             where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                              || p.PARENT_ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                              && string.IsNullOrEmpty(p.IS_END)
                                             select p).ToList();

                            if (processes != null && processes.Count > 0)
                            {
                                var stepId = ART_M_STEP_ARTWORK_SERVICE.GetByItem(new ART_M_STEP_ARTWORK() { STEP_ARTWORK_CODE = "SEND_PA" }, context).FirstOrDefault().STEP_ARTWORK_ID;
                                foreach (ART_WF_ARTWORK_PROCESS iProcess in processes)
                                {
                                    iProcess.IS_END = "X";
                                    if (iProcess.CURRENT_STEP_ID != stepId && iProcess.ARTWORK_SUB_ID != param.data.ARTWORK_SUB_ID)
                                        iProcess.REMARK_KILLPROCESS = "Completed workflow by PA";
                                    iProcess.UPDATE_BY = param.data.UPDATE_BY;
                                    ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdate(iProcess, context);
                                    //#437016
                                    if (iProcess.CURRENT_STEP_ID == 2 && iProcess.IS_END == "X")
                                        CNService.CompletePOForm(param, context);
                                }
                            }
                        }

                        //var ARTWORK_REQUEST_ID = (from p in context.ART_WF_ARTWORK_PROCESS
                        //                          where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                        //                          select p.ARTWORK_REQUEST_ID).FirstOrDefault();

                        //var TYPE_OF_ARTWORK = (from p in context.ART_WF_ARTWORK_REQUEST
                        //                       where p.ARTWORK_REQUEST_ID == ARTWORK_REQUEST_ID
                        //                       select p.TYPE_OF_ARTWORK).FirstOrDefault();

                        //if (TYPE_OF_ARTWORK == "NEW")
                        //{
                        //    var tempProcess = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(param.data.ARTWORK_SUB_ID, context);
                        //    var foundPrintMaster = ArtworkProcessHelper.moveFileArtworkToMatWorkspace(tempProcess.ARTWORK_ITEM_ID, context);
                        //    if (!foundPrintMaster)
                        //    {
                        //        Results.status = "E";
                        //        Results.msg = "Cannot complete workflow, System not found print master file in this workflow.";
                        //        return Results;
                        //    }
                        //}

                        //dbContextTransaction.Commit();

                        //EmailService.sendEmailArtwork(param.data.ARTWORK_REQUEST_ID, param.data.ARTWORK_SUB_ID, "WF_COMPLETED", context);

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

        public static ART_WF_ARTWORK_PROCESS_PA_RESULT SaveReadyCreatePO(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();
            ART_WF_ARTWORK_PROCESS_PA pa = new ART_WF_ARTWORK_PROCESS_PA();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            pa.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            pa = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(pa, context).FirstOrDefault();

                            if (pa != null)
                            {
                                pa.READY_CREATE_PO = param.data.READY_CREATE_PO;
                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(pa, context);
                            }
                            else
                            {
                                pa = new ART_WF_ARTWORK_PROCESS_PA();
                                pa.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                pa.READY_CREATE_PO = param.data.READY_CREATE_PO;
                                pa.CREATE_BY = param.data.UPDATE_BY;
                                pa.UPDATE_BY = param.data.UPDATE_BY;
                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(pa, context);
                            }
                        }
                        else
                        {
                            return Results;
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

        public static ART_WF_ARTWORK_PROCESS_PA_RESULT SaveShadeLimit(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();
            ART_WF_ARTWORK_PROCESS_PA pa = new ART_WF_ARTWORK_PROCESS_PA();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            pa.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            pa = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(pa, context).FirstOrDefault();

                            if (pa != null)
                            {
                                pa.SHADE_LIMIT = param.data.SHADE_LIMIT;
                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(pa, context);
                            }
                            else
                            {
                                pa = new ART_WF_ARTWORK_PROCESS_PA();
                                pa.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                pa.SHADE_LIMIT = param.data.SHADE_LIMIT;
                                pa.CREATE_BY = param.data.UPDATE_BY;
                                pa.UPDATE_BY = param.data.UPDATE_BY;
                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(pa, context);
                            }
                        }
                        else
                        {
                            return Results;
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

        public static ART_WF_ARTWORK_PROCESS_PA_RESULT SaveReceiveShadeLimit(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();
            ART_WF_ARTWORK_PROCESS_PA pa = new ART_WF_ARTWORK_PROCESS_PA();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            pa.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            pa = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(pa, context).FirstOrDefault();

                            if (pa != null)
                            {
                                pa.RECEIVE_SHADE_LIMIT = param.data.RECEIVE_SHADE_LIMIT;
                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(pa, context);
                            }
                            else
                            {
                                pa = new ART_WF_ARTWORK_PROCESS_PA();
                                pa.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                pa.RECEIVE_SHADE_LIMIT = param.data.RECEIVE_SHADE_LIMIT;
                                pa.CREATE_BY = param.data.UPDATE_BY;
                                pa.UPDATE_BY = param.data.UPDATE_BY;
                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(pa, context);
                            }
                        }
                        else
                        {
                            return Results;
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

        public static ART_WF_ARTWORK_PROCESS_PA_RESULT SaveChangePoint(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();
            ART_WF_ARTWORK_PROCESS_PA pa = new ART_WF_ARTWORK_PROCESS_PA();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            pa.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            pa = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(pa, context).FirstOrDefault();

                            if (pa != null)
                            {
                                pa.CHANGE_POINT = param.data.CHANGE_POINT;
                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(pa, context);
                            }
                            else
                            {
                                pa = new ART_WF_ARTWORK_PROCESS_PA();
                                pa.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                pa.CHANGE_POINT = param.data.CHANGE_POINT;
                                pa.CREATE_BY = param.data.UPDATE_BY;
                                pa.UPDATE_BY = param.data.UPDATE_BY;
                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(pa, context);
                            }
                        }
                        else
                        {
                            return Results;
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

        public static PP_RESULT GetWorkflowForPP(PP_REQUEST param)
        {
            PP_RESULT Results = new PP_RESULT();
            List<ART_WF_ARTWORK_PROCESS_PA> listPA = new List<ART_WF_ARTWORK_PROCESS_PA>();

            List<PP_MODEL> listPP = new List<PP_MODEL>();
            PP_MODEL pp = new PP_MODEL();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var stepPG = (from s in context.ART_M_STEP_ARTWORK where s.STEP_ARTWORK_CODE == "SEND_PG" select s).FirstOrDefault();
                        var stepPA = (from s in context.ART_M_STEP_ARTWORK where s.STEP_ARTWORK_CODE == "SEND_PA" select s).FirstOrDefault();
                        var stepPP = (from s in context.ART_M_STEP_ARTWORK where s.STEP_ARTWORK_CODE == "SEND_PP" select s).FirstOrDefault();
                        var stepVendor = (from s in context.ART_M_STEP_ARTWORK where s.STEP_ARTWORK_CODE == "SEND_VN_PO" select s).FirstOrDefault();

                        var procesesPP = (from p in context.ART_WF_ARTWORK_PROCESS
                                          where p.CURRENT_STEP_ID == stepPP.STEP_ARTWORK_ID
                                          && p.REMARK_KILLPROCESS == null
                                          && string.IsNullOrEmpty(p.IS_END)
                                          select p.PARENT_ARTWORK_SUB_ID).ToList();

                        var procesesVendor = (from p in context.ART_WF_ARTWORK_PROCESS
                                              where p.CURRENT_STEP_ID == stepVendor.STEP_ARTWORK_ID
                                              && p.REMARK_KILLPROCESS == null
                                              && string.IsNullOrEmpty(p.IS_END)
                                              select p.ARTWORK_ITEM_ID).Distinct().ToList();

                        var proceses = (from p in context.ART_WF_ARTWORK_PROCESS
                                        join m in context.ART_WF_ARTWORK_PROCESS_PA on p.ARTWORK_SUB_ID equals m.ARTWORK_SUB_ID
                                        where !procesesPP.Contains(p.ARTWORK_SUB_ID)
                                        && !procesesVendor.Contains(p.ARTWORK_ITEM_ID)
                                        && m.READY_CREATE_PO == "X"
                                        && p.CURRENT_USER_ID == param.data.CURRENT_USER_ID
                                        && p.CURRENT_STEP_ID == stepPA.STEP_ARTWORK_ID
                                        && string.IsNullOrEmpty(p.IS_END)
                                        select p).ToList();

                        foreach (ART_WF_ARTWORK_PROCESS iProcess in proceses)
                        {
                            pp = new PP_MODEL();
                            var paTmp = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                         where p.ARTWORK_SUB_ID == iProcess.ARTWORK_SUB_ID
                                              && p.READY_CREATE_PO == "X"
                                         select p).FirstOrDefault();

                            if (paTmp != null)
                            {
                                var isLock = CNService.IsLock(iProcess.ARTWORK_SUB_ID, context);
                                if (isLock) pp.IS_LOCK = "X";

                                var request = (from r in context.ART_WF_ARTWORK_REQUEST
                                               where r.ARTWORK_REQUEST_ID == iProcess.ARTWORK_REQUEST_ID
                                               select r).FirstOrDefault();

                                pp.ARTWORK_REQUEST_ID = iProcess.ARTWORK_REQUEST_ID;
                                pp.ARTWORK_SUB_ID = iProcess.ARTWORK_SUB_ID;
                                pp.ARTWORK_ITEM_ID = iProcess.ARTWORK_ITEM_ID;

                                if (request != null)
                                {
                                    var requestItem = (from r in context.ART_WF_ARTWORK_REQUEST_ITEM
                                                       where r.ARTWORK_ITEM_ID == iProcess.ARTWORK_ITEM_ID
                                                       select r).FirstOrDefault();

                                    if (request.SOLD_TO_ID != null && request.SOLD_TO_ID > 0)
                                    {
                                        pp.SOLD_TO_ID = Convert.ToInt32(request.SOLD_TO_ID);

                                        var customer = (from p in context.XECM_M_CUSTOMER
                                                        where p.CUSTOMER_ID == request.SOLD_TO_ID
                                                        select p).FirstOrDefault();
                                        if (customer != null)
                                        {
                                            pp.SOLD_TO_DISPLAY_TXT = customer.CUSTOMER_CODE + ":" + customer.CUSTOMER_NAME;
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    if (request.SOLD_TO_ID != null && request.SHIP_TO_ID > 0)
                                    {
                                        pp.SHIP_TO_ID = Convert.ToInt32(request.SHIP_TO_ID);

                                        var customer = (from p in context.XECM_M_CUSTOMER
                                                        where p.CUSTOMER_ID == request.SHIP_TO_ID
                                                        select p).FirstOrDefault();
                                        if (customer != null)
                                        {
                                            pp.SHIP_TO_DISPLAY_TXT = customer.CUSTOMER_CODE + ":" + customer.CUSTOMER_NAME;
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    pp.GROUPING = pp.SOLD_TO_ID.ToString() + pp.SHIP_TO_ID.ToString();

                                    if (request.REQUEST_DELIVERY_DATE != null)
                                    {
                                        pp.RDD = request.REQUEST_DELIVERY_DATE;//Convert.ToDateTime(request.REQUEST_DELIVERY_DATE).ToString("dd/MM/yyyy");
                                    }

                                    if (!String.IsNullOrEmpty(request.BRAND_OTHER))
                                    {
                                        pp.BRAND_DISPLAY_TXT = request.BRAND_OTHER;
                                    }
                                    else if (request.BRAND_ID != null)
                                    {
                                        pp.BRAND_ID = Convert.ToInt32(request.BRAND_ID);

                                        var brand = (from b in context.SAP_M_BRAND
                                                     where b.BRAND_ID == request.BRAND_ID
                                                     select b).FirstOrDefault();

                                        if (brand != null)
                                        {
                                            pp.BRAND_DISPLAY_TXT = brand.MATERIAL_GROUP + ":" + brand.DESCRIPTION;
                                        }
                                    }

                                    if (paTmp.MATERIAL_GROUP_ID != null)
                                    {
                                        pp.PKG_TYPE_ID = Convert.ToInt32(paTmp.MATERIAL_GROUP_ID);
                                        pp.PKG_TYPE_DISPLAY_TXT = CNService.GetCharacteristicDescription(paTmp.MATERIAL_GROUP_ID, context);
                                    }

                                    if (!String.IsNullOrEmpty(requestItem.REQUEST_ITEM_NO))
                                    {
                                        pp.WORKFLOW_NO = requestItem.REQUEST_ITEM_NO;
                                    }

                                    var processParentTmp = (from g in context.ART_WF_ARTWORK_PROCESS
                                                            where g.PARENT_ARTWORK_SUB_ID == iProcess.ARTWORK_SUB_ID
                                                            && g.CURRENT_STEP_ID == stepPG.STEP_ARTWORK_ID
                                                            select g.ARTWORK_SUB_ID).ToList();

                                    if (processParentTmp != null && processParentTmp.Count > 0)
                                    {
                                        var processPG = (from g in context.ART_WF_ARTWORK_PROCESS_PG
                                                         where processParentTmp.Contains(g.ARTWORK_SUB_ID)
                                                         && g.ACTION_CODE == "SUBMIT"
                                                         orderby g.UPDATE_DATE descending
                                                         select g).FirstOrDefault();

                                        if (processPG != null)
                                        {
                                            if (processPG.DIE_LINE_MOCKUP_ID != null)
                                            {
                                                var process = (from p in context.ART_WF_MOCKUP_PROCESS
                                                               where p.MOCKUP_ID == processPG.DIE_LINE_MOCKUP_ID
                                                               select p).FirstOrDefault();

                                                if (process != null)
                                                {
                                                    var processMoPG = (from pg in context.ART_WF_MOCKUP_PROCESS_PG
                                                                       where pg.MOCKUP_SUB_ID == process.MOCKUP_SUB_ID
                                                                       select pg).OrderByDescending(o => o.MOCKUP_SUB_PG_ID).FirstOrDefault();

                                                    if (processPG != null)
                                                    {
                                                        if (processMoPG.VENDOR > 0)
                                                        {
                                                            pp.VENDOR_DISPLAY_TXT = CNService.GetVendorCodeName(processMoPG.VENDOR, context);
                                                        }
                                                        else
                                                        {
                                                            pp.VENDOR_DISPLAY_TXT = processMoPG.VENDOR_OTHER;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (string.IsNullOrEmpty(pp.VENDOR_DISPLAY_TXT))
                                    {
                                        var parentId = CNService.FindParentArtworkSubId(iProcess.ARTWORK_SUB_ID, context);
                                        var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                         where p.ARTWORK_SUB_ID == parentId
                                                         select p).FirstOrDefault();

                                        if (processPA != null)
                                        {
                                            if (!string.IsNullOrEmpty(processPA.MATERIAL_NO))
                                            {
                                                var SAP_M_MATERIAL_CONVERSION = (from p in context.SAP_M_MATERIAL_CONVERSION
                                                                                 where p.MATERIAL_NO == processPA.MATERIAL_NO
                                                                                 && p.CHAR_NAME == "ZPKG_SEC_VENDOR"
                                                                                 select p).FirstOrDefault();
                                                if (SAP_M_MATERIAL_CONVERSION != null)
                                                {
                                                    var vendorMaster = XECM_M_VENDOR_SERVICE.GetByItem(new XECM_M_VENDOR() { VENDOR_CODE = SAP_M_MATERIAL_CONVERSION.CHAR_VALUE }, context).FirstOrDefault();
                                                    if (vendorMaster != null)
                                                    {
                                                        pp.VENDOR_DISPLAY_TXT = vendorMaster.VENDOR_CODE + ":" + vendorMaster.VENDOR_NAME;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    var soDetails = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                                     where s.ARTWORK_SUB_ID == iProcess.ARTWORK_SUB_ID
                                                     select s).ToList();

                                    if (soDetails != null && soDetails.Count > 0)
                                    {
                                        string so_display = "";
                                        string so_displayTmp = "";
                                        string mat_display = "";
                                        string bom_display = "";

                                        foreach (ART_WF_ARTWORK_PROCESS_SO_DETAIL so in soDetails)
                                        {
                                            so_displayTmp = so.SALES_ORDER_NO + "(" + so.SALES_ORDER_ITEM.ToString().Trim() + ")";
                                            so_display += so_displayTmp + ", <br> ";

                                            if (!String.IsNullOrEmpty(so.MATERIAL_NO) && !mat_display.Contains(so.MATERIAL_NO))
                                            {
                                                mat_display += so.MATERIAL_NO + ", <br> ";
                                            }

                                            if (so.BOM_ID > 0)
                                            {
                                                bom_display += CNService.GetBOMNo(Convert.ToInt32(so.BOM_ID), context) + ", <br> ";
                                            }
                                        }

                                        if (!String.IsNullOrEmpty(so_display))
                                        {
                                            pp.SALES_ORDER_ITEM = so_display.Substring(0, so_display.Length - 7);
                                        }

                                        if (!String.IsNullOrEmpty(mat_display))
                                        {
                                            pp.PRODUCT_CODE = mat_display.Substring(0, mat_display.Length - 7);
                                        }

                                        if (!String.IsNullOrEmpty(paTmp.MATERIAL_NO))
                                        {
                                            pp.PKG_CODE = paTmp.MATERIAL_NO;
                                        }

                                        if (String.IsNullOrEmpty(pp.PKG_CODE))
                                        {
                                            if (!String.IsNullOrEmpty(bom_display))
                                            {
                                                pp.PKG_CODE = bom_display.Substring(0, bom_display.Length - 7);
                                            }
                                        }

                                        pp.RECEIVE_DATE = iProcess.CREATE_DATE;//.ToString("dd/MM/yyyy HH:mm:ss");

                                    }
                                }

                                pp.IS_SALES_ORDER_CHANGE = SalesOrderHelper.CheckIsSalesOrderChange(pp.ARTWORK_SUB_ID, context);

                                listPP.Add(pp);
                            }

                        }
                    }
                }

                Results.status = "S";
                Results.data = listPP;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_RESULT SendBackMK(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_RESULT Results = new ART_WF_ARTWORK_PROCESS_RESULT();
            ART_WF_ARTWORK_PROCESS process = new ART_WF_ARTWORK_PROCESS();

            try
            {
                process = MapperServices.ART_WF_ARTWORK_PROCESS(param.data);

                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        string msg = ArtworkProcessHelper.checkDupWF(param.data, context);
                        if (msg != "")
                        {
                            Results.status = "E";
                            Results.msg = msg;
                            return Results;
                        }

                        if (process.CURRENT_USER_ID == null)
                        {
                            process.CURRENT_USER_ID = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(process.ARTWORK_REQUEST_ID, context).CREATOR_ID;
                        }

                        CNService.CheckDelegateBeforeRountingArtwork(process, context);

                        dbContextTransaction.Commit();
                    }

                    EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_BACK", context, param.data.REMARK.Replace("<p>", "").Replace("</p>", ""));
                }

                Results.msg = MessageHelper.GetMessage("MSG_001");
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.msg = CNService.GetErrorMessage(ex);
                Results.status = "E";
            }
            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_PA_PRODUCT_RESULT DeleteProduct(ART_WF_ARTWORK_PROCESS_PA_PRODUCT_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_PRODUCT_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.DeleteByARTWORK_SUB_PA_PRODUCT_ID(param.data.ARTWORK_SUB_PA_PRODUCT_ID, context);
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            return Results;
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

        public static ART_WF_ARTWORK_PROCESS_PA_PLANT_RESULT DeletePlant(ART_WF_ARTWORK_PROCESS_PA_PLANT_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_PLANT_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_PLANT_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            ART_WF_ARTWORK_PROCESS_PA_PLANT_SERVICE.DeleteByARTWORK_SUB_PA_PLANT_ID(param.data.ARTWORK_SUB_PA_PLANT_ID, context);
                            dbContextTransaction.Commit();

                        }
                        else
                        {
                            return Results;
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

        public static ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_RESULT DeleteFAOZone(ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.DeleteByARTWORK_SUB_PA_FAO_ID(param.data.ARTWORK_SUB_PA_FAO_ID, context);
                            dbContextTransaction.Commit();

                        }
                        else
                        {
                            return Results;
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

        public static ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_RESULT DeleteCatchingArea(ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            //  ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.DeleteByARTWORK_SUB_PA_FAO_ID(param.data.ARTWORK_SUB_PA_CATCHING_AREA_ID, context);  ticke#425737 added by aof  COMMETNED BY AOF
                            ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE.DeleteByARTWORK_SUB_PA_CATCHING_AREA_ID(param.data.ARTWORK_SUB_PA_CATCHING_AREA_ID, context);                    
                            dbContextTransaction.Commit();

                        }
                        else
                        {
                            return Results;
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

        // ticke#425737 added by aof 
        public static ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_RESULT DeleteCatchingMethod(ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_RESULT();
         
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE.DeleteByARTWORK_SUB_PA_CATCHING_METHOD_ID(param.data.ARTWORK_SUB_PA_CATCHING_METHOD_ID, context);
                            dbContextTransaction.Commit();

                        }
                        else
                        {
                            return Results;
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
        // ticke#425737 added by aof 

        public static ART_WF_ARTWORK_PROCESS_PA_SYMBOL_RESULT DeleteSymbol(ART_WF_ARTWORK_PROCESS_PA_SYMBOL_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_SYMBOL_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null)
                        {
                            ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE.DeleteByARTWORK_SUB_PA_SYMBOL_ID(param.data.ARTWORK_SUB_PA_SYMBOL_ID, context);
                            dbContextTransaction.Commit();

                        }
                        else
                        {
                            return Results;
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


        //ticket#437764 added by aof on 30/03/2021
        public static ART_WF_ARTWORK_PROCESS_PA_RESULT GetArtWorkProcessPA(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();
            List<ART_WF_ARTWORK_PROCESS_PA_2> listPAData2 = new List<ART_WF_ARTWORK_PROCESS_PA_2>();


            try
            {
                //ART_WF_ARTWORK_PROCESS_PA_2 processpa = new ART_WF_ARTWORK_PROCESS_PA_2();

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;
                        var processPA = context.ART_WF_ARTWORK_PROCESS_PA.Where(r => r.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).FirstOrDefault();
                        if (processPA !=null)
                        {
                            var processPA2 = MapperServices.ART_WF_ARTWORK_PROCESS_PA(processPA);
                            var listPAProduct = context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT.Where(w => w.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID).ToList();

                       
                          
                            processPA2.PRODUCTS = MapperServices.ART_WF_ARTWORK_PROCESS_PA_PRODUCT(listPAProduct);

                            var matGroupValue = (from m in context.SAP_M_CHARACTERISTIC
                                                 where m.CHARACTERISTIC_ID == processPA2.MATERIAL_GROUP_ID
                                                 select m.VALUE).FirstOrDefault();

                            string matGroup = "5" + matGroupValue;

                            processPA2.MATERIAL_GROUP_CODE = matGroup;


                            XECM_M_PRODUCT xProduct = new XECM_M_PRODUCT();
                            xProduct = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(processPA2.PRODUCT_CODE_ID, context);
                            if (xProduct != null)
                            {
                                processPA2.PRODUCT_CODE_DISPLAY_TXT = xProduct.PRODUCT_CODE;
                            }

                            if (processPA2.PRODUCTS != null)
                            {
                                foreach (var p in processPA2.PRODUCTS)
                                {
                                    xProduct = new XECM_M_PRODUCT();
                                    xProduct = XECM_M_PRODUCT_SERVICE.GetByXECM_PRODUCT_ID(p.PRODUCT_CODE_ID, context);
                                    if (xProduct != null)
                                    {
                                        p.PRODUCT_CODE = xProduct.PRODUCT_CODE;
                                    }

                                }
                            }

                            listPAData2.Add(processPA2);
                            Results.data = listPAData2;
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

        public static string getSQLWhereByJoinStringWithAnd(string curWhere, string newWhere)
        {
            string retWhere = curWhere;

            if (!string.IsNullOrEmpty(newWhere))
            {
                if (!string.IsNullOrEmpty(retWhere))
                {
                    retWhere += " and (" + newWhere + ")";
                }
                else
                {
                    retWhere = newWhere;
                }
            }

            return retWhere;
        }

        public static string getSQLWhereLikeByConvertString(string strPattern, string fldname)
        {
            string where = "";

            if (!string.IsNullOrEmpty(strPattern))
            {

                var arrStr = strPattern.Replace(" ", "").Split(',');

                // var arrStr = strPattern.Split(',');

                if (arrStr != null)
                {
                    if (arrStr.Length > 0)
                    {
                        foreach (string s in arrStr)
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                // where += ",'" + s + "'";
                                if (string.IsNullOrEmpty(where))
                                {
                                    where = "(" + fldname + " like '" + s + "%')";
                                }
                                else
                                {
                                    where += " or (" + fldname + " like '" + s + "%')";
                                }
                            }
                        }
                    }
                }

            }

            if (!string.IsNullOrEmpty(where))
            {
                where = "(" + where + ")";
            }

            return where;
        }

        //ticket#437764 added by aof on 30/03/2021



        public static SAP_M_ORDER_BOM_RESULT GetSuggestMaterial(SAP_M_ORDER_BOM_REQUEST param)
        {
            SAP_M_ORDER_BOM_RESULT Results = new SAP_M_ORDER_BOM_RESULT();
            List<SAP_M_ORDER_BOM_2> listOrderBoms_2 = new List<SAP_M_ORDER_BOM_2>();
            List<SAP_M_ORDER_BOM> listOrderBoms = new List<SAP_M_ORDER_BOM>();
            List<SAP_M_ORDER_BOM> listOrderBoms_Results = new List<SAP_M_ORDER_BOM>();

            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }
                else
                {
                    if (param.data.ALL_DATA == "1")
                    {
                        using (var context = new ARTWORKEntities())
                        {
                            using (CNService.IsolationLevel(context))
                            {
                                context.Database.CommandTimeout = 300;

                                DateTime dateNow = DateTime.Now.Date;

                                var where = "";
                                if (param.data.SEARCH_MAT_FG != null) {
                                    where = getSQLWhereByJoinStringWithAnd(where, getSQLWhereLikeByConvertString(param.data.SEARCH_MAT_FG, "o.MATERIAL"));
                                }
                                if (param.data.SEARCH_MAT_PK != null)
                                {
                                    where = getSQLWhereByJoinStringWithAnd(where, getSQLWhereLikeByConvertString(param.data.SEARCH_MAT_PK, "o.MATERIAL_NUMBER"));
                                }
                                if (param.data.SEARCH_SOLD_TO_ID  != 0 && param.data.SEARCH_SOLD_TO_ID != null)
                                {
                                    where = getSQLWhereByJoinStringWithAnd(where, "s.CUSTOMER_ID=" + param.data.SEARCH_SOLD_TO_ID);
                                }
                                if (param.data.SEARCH_SHIP_TO_ID != 0 && param.data.SEARCH_SHIP_TO_ID != null)
                                {
                                    where = getSQLWhereByJoinStringWithAnd(where, "z.CUSTOMER_ID=" + param.data.SEARCH_SHIP_TO_ID);
                                }
                                if (param.data.SEARCH_BRAND_ID != 0 && param.data.SEARCH_BRAND_ID != null)
                                {
                                    where = getSQLWhereByJoinStringWithAnd(where, "b.BRAND_ID=" + param.data.SEARCH_BRAND_ID);
                                }


                                var q = context.Database.SqlQuery<SAP_M_ORDER_BOM_2>("sp_ART_ORDER_BOM @where", new System.Data.SqlClient.SqlParameter("@where", where)).ToList();

                                listOrderBoms_2 = q.ToList();
                                foreach (var item in listOrderBoms_2)
                                {
                                    if (item.SOLD_TO_DISPLAY_TXT == ":") item.SOLD_TO_DISPLAY_TXT = "";
                                    if (item.SHIP_TO_DISPLAY_TXT == ":") item.SHIP_TO_DISPLAY_TXT = "";
                                    if (item.BRAND_DISPLAY_TXT == ":") item.BRAND_DISPLAY_TXT = "";
                                }


                                //ticket#437764 commented by aof on 30/03/2021.. start
                                //var q = (from b in context.V_SAP_M_ORDER_BOM
                                //         join m in context.SAP_M_BRAND on b.BRAND_ID equals m.MATERIAL_GROUP into ps
                                //         from ps1 in ps.DefaultIfEmpty()
                                //         join m2 in context.XECM_M_CUSTOMER on b.SHIP_TO_PARTY equals DbFunctions.Right("0000000000" + m2.CUSTOMER_CODE, 10) into ps2
                                //         from ps21 in ps2.DefaultIfEmpty()
                                //         join m3 in context.XECM_M_CUSTOMER on b.SOLD_TO_PARTY equals DbFunctions.Right("0000000000" + m3.CUSTOMER_CODE, 10) into ps3
                                //         from ps31 in ps3.DefaultIfEmpty()
                                //         where b.START_DATE <= dateNow
                                //         && b.END_DATE >= dateNow
                                //         && b.CHANGE_TYPE != "D"
                                //         && b.LAST_UPDATE == "X"
                                //         select new SAP_M_ORDER_BOM_2
                                //         {
                                //             BRAND_DISPLAY_TXT = b.BRAND_ID + ":" + ps1.DESCRIPTION,
                                //             SOLD_TO_DISPLAY_TXT = b.SOLD_TO_PARTY + ":" + ps31.CUSTOMER_NAME,
                                //             SHIP_TO_DISPLAY_TXT = b.SHIP_TO_PARTY + ":" + ps21.CUSTOMER_NAME,
                                //             MATERIAL = b.MATERIAL,
                                //             MATERIAL_NUMBER = b.MATERIAL_NUMBER,
                                //             START_DATE = b.START_DATE,
                                //             END_DATE = b.END_DATE,
                                //         }).Distinct();

                                //if (!string.IsNullOrEmpty(param.search.value))
                                //{
                                //    param.search.value = param.search.value.Trim();

                                //    q = q.Where(m => m.BRAND_DISPLAY_TXT.Contains(param.search.value)
                                //    || m.SOLD_TO_DISPLAY_TXT.Contains(param.search.value)
                                //    || m.SHIP_TO_DISPLAY_TXT.Contains(param.search.value)
                                //    || m.MATERIAL.Contains(param.search.value)
                                //    || m.MATERIAL_NUMBER.Contains(param.search.value));
                                //}

                                //Results.draw = param.draw;
                                //Results.recordsTotal = q.Distinct().Count();
                                //Results.recordsFiltered = Results.recordsTotal;

                                //var orderColumn = 1;
                                //var orderDir = "asc";
                                //if (param.order != null && param.order.Count > 0)
                                //{
                                //    orderColumn = param.order[0].column;
                                //    orderDir = param.order[0].dir; //desc ,asc
                                //}
                                //string orderASC = "asc";
                                //string orderDESC = "desc";
                                //if (orderColumn == 1)
                                //{
                                //    if (orderDir == orderASC)
                                //        listOrderBoms_2 = q.OrderBy(i => i.MATERIAL).Skip(param.start).Take(param.length).Distinct().ToList();
                                //    else if (orderDir == orderDESC)
                                //        listOrderBoms_2 = q.OrderByDescending(i => i.MATERIAL).Skip(param.start).Take(param.length).Distinct().ToList();
                                //}
                                //if (orderColumn == 2)
                                //{
                                //    if (orderDir == orderASC)
                                //        listOrderBoms_2 = q.OrderBy(i => i.MATERIAL_NUMBER).Skip(param.start).Take(param.length).Distinct().ToList();
                                //    else if (orderDir == orderDESC)
                                //        listOrderBoms_2 = q.OrderByDescending(i => i.MATERIAL_NUMBER).Skip(param.start).Take(param.length).Distinct().ToList();
                                //}
                                //if (orderColumn == 3)
                                //{
                                //    if (orderDir == orderASC)
                                //        listOrderBoms_2 = q.OrderBy(i => i.BRAND_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().ToList();
                                //    else if (orderDir == orderDESC)
                                //        listOrderBoms_2 = q.OrderByDescending(i => i.BRAND_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().ToList();
                                //}
                                //if (orderColumn == 4)
                                //{
                                //    if (orderDir == orderASC)
                                //        listOrderBoms_2 = q.OrderBy(i => i.SOLD_TO_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().ToList();
                                //    else if (orderDir == orderDESC)
                                //        listOrderBoms_2 = q.OrderByDescending(i => i.SOLD_TO_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().ToList();
                                //}
                                //if (orderColumn == 5)
                                //{
                                //    if (orderDir == orderASC)
                                //        listOrderBoms_2 = q.OrderBy(i => i.SHIP_TO_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().ToList();
                                //    else if (orderDir == orderDESC)
                                //        listOrderBoms_2 = q.OrderByDescending(i => i.SHIP_TO_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().ToList();
                                //}
                                //if (orderColumn == 6)
                                //{
                                //    if (orderDir == orderASC)
                                //        listOrderBoms_2 = q.OrderBy(i => i.START_DATE).Skip(param.start).Take(param.length).Distinct().ToList();
                                //    else if (orderDir == orderDESC)
                                //        listOrderBoms_2 = q.OrderByDescending(i => i.START_DATE).Skip(param.start).Take(param.length).Distinct().ToList();
                                //}
                                //if (orderColumn == 7)
                                //{
                                //    if (orderDir == orderASC)
                                //        listOrderBoms_2 = q.OrderBy(i => i.END_DATE).Skip(param.start).Take(param.length).Distinct().ToList();
                                //    else if (orderDir == orderDESC)
                                //        listOrderBoms_2 = q.OrderByDescending(i => i.END_DATE).Skip(param.start).Take(param.length).Distinct().ToList();
                                //}

                                ////listOrderBoms_2 = q;
                                //foreach (var item in listOrderBoms_2)
                                //{
                                //    if (item.SOLD_TO_DISPLAY_TXT == ":") item.SOLD_TO_DISPLAY_TXT = "";
                                //    if (item.SHIP_TO_DISPLAY_TXT == ":") item.SHIP_TO_DISPLAY_TXT = "";
                                //    if (item.BRAND_DISPLAY_TXT == ":") item.BRAND_DISPLAY_TXT = "";
                                //}

                                //ticket#437764 commented by aof on 30/03/2021.. last
                            }
                        }
                    }
                    else
                    {
                        using (var context = new ARTWORKEntities())
                        {
                            using (CNService.IsolationLevel(context))
                            {
                                context.Database.CommandTimeout = 300;

                                var request = context.ART_WF_ARTWORK_REQUEST.Where(r => r.ARTWORK_REQUEST_ID == param.data.ARTWORK_REQUEST_ID).FirstOrDefault();
                                var processPA = context.ART_WF_ARTWORK_PROCESS_PA.Where(r => r.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).FirstOrDefault();

                                if (request == null || processPA == null)
                                {
                                    return Results;
                                }
                                string validateMsg = "";
                                int soldToID = 0;
                                int shipToID = 0;
                                int brandID = 0;
                                string materialCode = "";

                                validateMsg = MessageHelper.GetMessage("MSG_009", context);

                                if (request.SOLD_TO_ID != null)
                                {
                                    soldToID = Convert.ToInt32(request.SOLD_TO_ID);
                                }
                                else
                                {
                                    Results.status = "E";
                                    Results.msg = "Sold To " + validateMsg;
                                    return Results;
                                }

                                if (request.SHIP_TO_ID != null)
                                {
                                    shipToID = Convert.ToInt32(request.SHIP_TO_ID);
                                }
                                else
                                {
                                    Results.status = "E";
                                    Results.msg = "Ship To " + validateMsg;
                                    return Results;
                                }

                                if (request.BRAND_ID != null)
                                {
                                    brandID = Convert.ToInt32(request.BRAND_ID);
                                }
                                else
                                {
                                    Results.status = "E";
                                    Results.msg = "Brand " + validateMsg;
                                    return Results;
                                }


                                //// //ticket#437764 added by aof on 30/03/2021 start
                                var f_found_product = false;
                                List<string> listProduct = new List<string>();
                                int productID = 0;
                                if (processPA.PRODUCT_CODE_ID != null)
                                {
                                    productID = 0; 
                                    productID = Convert.ToInt32(processPA.PRODUCT_CODE_ID);
                                     
                                    if (productID > 0)
                                    {
                                        materialCode = context.XECM_M_PRODUCT.Where(p => p.XECM_PRODUCT_ID == productID)
                                                                                .Select(s => s.PRODUCT_CODE)
                                                                                .FirstOrDefault();
                                        listProduct.Add(materialCode);
                                        f_found_product = true;
                                    }
                                   
                                }


                                var listProcessPAProduct = context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT.Where(r => r.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID).ToList();
                                if (listProcessPAProduct != null && listProcessPAProduct.Count > 0) {
                                    foreach (var x in listProcessPAProduct) {
                                        if (x.PRODUCT_CODE_ID > 0) {
                                            materialCode = context.XECM_M_PRODUCT.Where(p => p.XECM_PRODUCT_ID == x.PRODUCT_CODE_ID)
                                                                              .Select(s => s.PRODUCT_CODE)
                                                                              .FirstOrDefault();
                                            listProduct.Add(materialCode);
                                            f_found_product = true;
                                        }
                                    }
                                }


                                if (f_found_product==false) {
                                    Results.status = "E";
                                    Results.msg = "Product Code " + validateMsg;
                                    return Results;
                                }

                                //}


                                //if (processPA.PRODUCT_CODE_ID != null)
                                //{
                                //    int productID = 0;
                                //    productID = Convert.ToInt32(processPA.PRODUCT_CODE_ID);

                                //    if (productID > 0)
                                //    {
                                //        materialCode = context.XECM_M_PRODUCT.Where(p => p.XECM_PRODUCT_ID == productID)
                                //                                                .Select(s => s.PRODUCT_CODE)
                                //                                                .FirstOrDefault();
                                //    }
                                //}
                                //else
                                //{
                                //    Results.status = "E";
                                //    Results.msg = "Product Code " + validateMsg;
                                //    return Results;
                                //}

                                // //ticket#437764 added by aof on 30/03/2021 last

                                string soldToCode = "";
                                string shipToCode = "";
                                string brandCode = "";
                                DateTime dateNow = DateTime.Now.Date;

                                var xCustomer_Sold = context.XECM_M_CUSTOMER.Where(c => c.CUSTOMER_ID == soldToID).FirstOrDefault();
                                var xCustomer_Ship = context.XECM_M_CUSTOMER.Where(c => c.CUSTOMER_ID == shipToID).FirstOrDefault();
                                var sBrand = context.SAP_M_BRAND.Where(c => c.BRAND_ID == brandID).FirstOrDefault();

                                soldToCode = xCustomer_Sold.CUSTOMER_CODE;
                                shipToCode = xCustomer_Ship.CUSTOMER_CODE;

                                if (brandID > 0 && sBrand != null)
                                {
                                    brandCode = sBrand.MATERIAL_GROUP;
                                }

                                var matGroupValue = (from m in context.SAP_M_CHARACTERISTIC
                                                     where m.CHARACTERISTIC_ID == processPA.MATERIAL_GROUP_ID
                                                     select m.VALUE).FirstOrDefault();

                                string matGroup = "5" + matGroupValue;

                                //List<string> listAdditionalBrand = new List<string>();

                                //var soAssign = (from d in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                //                where d.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                //                select d).ToList();

                                //if (soAssign != null && soAssign.Count > 0)
                                //{
                                //    foreach (ART_WF_ARTWORK_PROCESS_SO_DETAIL item in soAssign)
                                //    {
                                //        var soHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                //                        where h.SALES_ORDER_NO == item.SALES_ORDER_NO
                                //                        select h).FirstOrDefault();

                                //        decimal itemNo = 0;
                                //        itemNo = Convert.ToDecimal(item.SALES_ORDER_ITEM);

                                //        var soItem = (from h in context.SAP_M_PO_COMPLETE_SO_ITEM
                                //                      where h.PO_COMPLETE_SO_HEADER_ID == soHeader.PO_COMPLETE_SO_HEADER_ID
                                //                          && h.ITEM == itemNo
                                //                      select h).FirstOrDefault();

                                //        //ticket#437764 added by aof on 30/03/2021 start
                                //        //comment by aof start
                                //        //var orderBoms_SO = ((from b in context.SAP_M_ORDER_BOM
                                //        //                     where b.SOLD_TO_PARTY.Contains(soHeader.SOLD_TO)
                                //        //                         && b.SHIP_TO_PARTY.Contains(soHeader.SHIP_TO)
                                //        //                         && b.BRAND_ID == soItem.BRAND_ID
                                //        //                         && b.ADDITIONAL_BRAND_ID == soItem.ADDITIONAL_BRAND_ID
                                //        //                         && b.COUNTRY_KEY == soItem.COUNTRY
                                //        //                         && b.SALES_ORGANIZATION == soHeader.SALES_ORG
                                //        //                         && b.PLANT == soItem.PLANT
                                //        //                         && listProduct.Contains(b.MATERIAL) // && b.MATERIAL == materialCode ticket#437764 by aof
                                //        //                         && b.START_DATE <= dateNow
                                //        //                         && b.END_DATE >= dateNow
                                //        //                         && b.MATERIAL_NUMBER.Contains(matGroup)
                                //        //                     select b)).ToList();
                                //        //comment by aof last
                                //        //new by aof start
                                //        var orderBoms_SO = ((from b in context.SAP_M_ORDER_BOM
                                //                             join m in context.V_SAP_M_ORDER_BOM on b.ORDER_BOM_ID equals m.ORDER_BOM_ID
                                //                             where b.SOLD_TO_PARTY.Contains(soHeader.SOLD_TO)
                                //                                 && b.SHIP_TO_PARTY.Contains(soHeader.SHIP_TO)
                                //                                 && b.BRAND_ID == soItem.BRAND_ID
                                //                                 && b.ADDITIONAL_BRAND_ID == soItem.ADDITIONAL_BRAND_ID
                                //                                 && b.COUNTRY_KEY == soItem.COUNTRY
                                //                                 && b.SALES_ORGANIZATION == soHeader.SALES_ORG
                                //                                 && b.PLANT == soItem.PLANT
                                //                                 && listProduct.Contains(b.MATERIAL) // && b.MATERIAL == materialCode ticket#437764 by aof
                                //                                 && b.START_DATE <= dateNow
                                //                                 && b.END_DATE >= dateNow
                                //                                 && b.MATERIAL_NUMBER.Contains(matGroup)
                                //                                 && m.LAST_UPDATE == "X"
                                //                             select b)).ToList();
                                //        //new by aof start
                                //        //ticket#437764 added by aof on 30/03/2021 last

                                //        if (orderBoms_SO != null)
                                //        {
                                //            listOrderBoms.AddRange(orderBoms_SO);
                                //        }
                                //    }
                                //}
                                //else
                                //{
                                //    //ticket#437764 added by aof on 30/03/2021 start
                                //    //start comment by aof
                                //    //listOrderBoms = (from b in context.SAP_M_ORDER_BOM
                                //    //                 where b.SOLD_TO_PARTY.Contains(soldToCode)
                                //    //                     && b.SHIP_TO_PARTY.Contains(shipToCode)
                                //    //                     && b.BRAND_ID == brandCode
                                //    //                     && listProduct.Contains(b.MATERIAL) // && b.MATERIAL == materialCode ticket#437764 by aof
                                //    //                     && b.START_DATE <= dateNow
                                //    //                     && b.END_DATE >= dateNow
                                //    //                     && b.MATERIAL_NUMBER.Contains(matGroup)
                                //    //                 select b).ToList();
                                //    //last comment by aof

                                //    //start new by aof
                                //    //listOrderBoms = (from b in context.SAP_M_ORDER_BOM
                                //    //                 join m in context.V_SAP_M_ORDER_BOM on b.ORDER_BOM_ID equals m.ORDER_BOM_ID
                                //    //                 where b.SOLD_TO_PARTY.Contains(soldToCode)
                                //    //                     && b.SHIP_TO_PARTY.Contains(shipToCode)
                                //    //                     && b.BRAND_ID == brandCode
                                //    //                     && listProduct.Contains(b.MATERIAL) // && b.MATERIAL == materialCode ticket#437764 by aof
                                //    //                     && b.START_DATE <= dateNow
                                //    //                     && b.END_DATE >= dateNow
                                //    //                     && b.MATERIAL_NUMBER.Contains(matGroup)
                                //    //                     && m.LAST_UPDATE == "X"
                                //    //                 select b).ToList();
                                //    //last new by aof
                                //    //ticket#437764 added by aof on 30/03/2021 last

                                //}


                                listOrderBoms = (from b in context.SAP_M_ORDER_BOM
                                                 join m in context.V_SAP_M_ORDER_BOM on b.ORDER_BOM_ID equals m.ORDER_BOM_ID
                                                 where b.SOLD_TO_PARTY.Contains(soldToCode)
                                                     && b.SHIP_TO_PARTY.Contains(shipToCode)
                                                     && b.BRAND_ID == brandCode
                                                     && listProduct.Contains(b.MATERIAL) // && b.MATERIAL == materialCode ticket#437764 by aof
                                                     && b.START_DATE <= dateNow
                                                     && b.END_DATE >= dateNow
                                                     && b.MATERIAL_NUMBER.StartsWith(matGroup)  // && b.MATERIAL_NUMBER.Contains(matGroup) //by aof INC-107174   
                                                     && m.LAST_UPDATE == "X"
                                                     && m.CHANGE_TYPE != "D"   // added by aof on 20211208
                                                 select b).ToList();

                                // var orderBoms = listOrderBoms.Select(s => new { s.MATERIAL, s.MATERIAL_NUMBER,s.SOLD_TO_PARTY,s.SHIP_TO_PARTY,s.BRAND_ID,s.PLANT,s.SALES_ORGANIZATION,s.COUNTRY_KEY  }).Distinct().ToList();   //ticket#437764 added by aof on 30/03/2021 last
                                //var orderBoms = listOrderBoms.Select(s => new { s.MATERIAL, s.MATERIAL_NUMBER }).Distinct().ToList();   //ticket#437764 added by aof on 30/03/2021 last


                                //if (orderBoms != null && orderBoms.Count > 0)
                                //{
                                //    SAP_M_ORDER_BOM_2 orderBOM_2 = new SAP_M_ORDER_BOM_2();

                                //    foreach (var iOrderBom in orderBoms)
                                //    {
                                //        var orderBOMTmp = (from b in context.SAP_M_ORDER_BOM
                                //                           where b.MATERIAL == iOrderBom.MATERIAL
                                //                               && b.MATERIAL_NUMBER == iOrderBom.MATERIAL_NUMBER
                                //                           select b).ToList();

                                //        var groupCounters = orderBOMTmp.GroupBy(g => g.COUNTER).ToList();

                                //        if (groupCounters != null)
                                //        {
                                //            List<SAP_M_ORDER_BOM> listMat = new List<SAP_M_ORDER_BOM>();

                                //            foreach (var itemGroup in groupCounters)
                                //            {
                                //                var mat = itemGroup.ToList().OrderByDescending(o => long.Parse(o.DATE + o.TIME.Replace(":", ""))).FirstOrDefault();

                                //                if (mat != null)
                                //                {
                                //                    if (mat.CHANGE_TYPE != "D")
                                //                        listMat.Add(mat);
                                //                }
                                //            }

                                //            var matTmp = (from m in listMat
                                //                          where m.START_DATE <= dateNow
                                //                            && m.END_DATE >= dateNow
                                //                            && m.CHANGE_TYPE != "D"
                                //                          select m).FirstOrDefault();

                                //            if (matTmp != null)
                                //            {
                                //                listOrderBoms_Results.Add(matTmp);
                                //            }
                                //        }
                                //    }
                                //}
                            }
                        }
                        if (listOrderBoms != null)
                        {
                            listOrderBoms_2 = MapperServices.SAP_M_ORDER_BOM(listOrderBoms);
                            using (var context = new ARTWORKEntities())
                            {
                                using (CNService.IsolationLevel(context))
                                {
                                    foreach (var item in listOrderBoms_2)
                                    {
                                        var mBrand = context.SAP_M_BRAND.Where(w => w.MATERIAL_GROUP == item.BRAND_ID).FirstOrDefault();
                                        if (mBrand != null && !String.IsNullOrEmpty(mBrand.DESCRIPTION))
                                        {
                                            item.BRAND_DISPLAY_TXT = item.BRAND_ID + ":" + mBrand.DESCRIPTION;
                                        }

                                        string shiptoCode = item.SHIP_TO_PARTY;
                                        string soldtoCode = item.SOLD_TO_PARTY;

                                        //if (shiptoCode.Length > 8 && shiptoCode.StartsWith("00"))
                                        //{
                                        //    shiptoCode = shiptoCode.Substring(2, 8);
                                        //}
                                        //if (soldtoCode.Length > 8 && soldtoCode.StartsWith("00"))
                                        //{
                                        //    soldtoCode = soldtoCode.Substring(2, 8);
                                        //}

                                       
                                        var mSold = context.XECM_M_CUSTOMER.Where(w => DbFunctions.Right("0000000000" + w.CUSTOMER_CODE, 10) == soldtoCode).FirstOrDefault();
                                        if (mSold != null && !String.IsNullOrEmpty(mSold.CUSTOMER_NAME))
                                        {
                                            item.SOLD_TO_DISPLAY_TXT = item.SOLD_TO_PARTY + ":" + mSold.CUSTOMER_NAME;
                                        }

                                        var mShip = context.XECM_M_CUSTOMER.Where(w => DbFunctions.Right("0000000000" + w.CUSTOMER_CODE, 10) == shiptoCode).FirstOrDefault();
                                        //var mShip = context.XECM_M_CUSTOMER.Where(w => w.CUSTOMER_CODE == shiptoCode).FirstOrDefault();
                                        if (mShip != null && !String.IsNullOrEmpty(mShip.CUSTOMER_NAME))
                                        {
                                            item.SHIP_TO_DISPLAY_TXT = item.SHIP_TO_PARTY + ":" + mShip.CUSTOMER_NAME;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //ticket#437764 added by aof on 30/03/2021 last
                if (listOrderBoms_2 != null)
                {
                    listOrderBoms_2 = listOrderBoms_2.OrderBy(o => (o.MATERIAL, o.MATERIAL_NUMBER, o.BRAND_ID, o.SOLD_TO_DISPLAY_TXT , o.SHIP_TO_DISPLAY_TXT, o.COUNTRY_KEY, o.PLANT)).ToList();
                }
                //ticket#437764 added by aof on 30/03/2021 last

                Results.data = listOrderBoms_2;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static SAP_M_ORDER_BOM_RESULT SaveSuggestMaterial(SAP_M_ORDER_BOM_REQUEST param)
        {
            SAP_M_ORDER_BOM_RESULT Results = new SAP_M_ORDER_BOM_RESULT();
            ART_WF_ARTWORK_PROCESS_PA processPA = new ART_WF_ARTWORK_PROCESS_PA();
            List<SAP_M_ORDER_BOM_2> listResult = new List<SAP_M_ORDER_BOM_2>();
            SAP_M_ORDER_BOM_2 data = new SAP_M_ORDER_BOM_2();

            if (param == null || param.data == null)
            {
                return Results;
            }

            int subID = param.data.ARTWORK_SUB_ID;

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        processPA.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                        processPA = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(processPA, context).FirstOrDefault();

                        if (processPA != null)
                        {
                            processPA.REFERENCE_MATERIAL = param.data.MATERIAL_NUMBER;
                            processPA.UPDATE_BY = param.data.UPDATE_BY;
                            ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);
                        }
                        else
                        {
                            processPA = new ART_WF_ARTWORK_PROCESS_PA();
                            processPA.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            processPA.REFERENCE_MATERIAL = param.data.MATERIAL_NUMBER;
                            processPA.CREATE_BY = param.data.UPDATE_BY;
                            processPA.UPDATE_BY = param.data.UPDATE_BY;
                            ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);
                        }

                        data.MATERIAL_NUMBER = param.data.MATERIAL_NUMBER;
                        listResult.Add(data);

                        ART_WF_ARTWORK_PROCESS_PA_REQUEST paRequest = new ART_WF_ARTWORK_PROCESS_PA_REQUEST();
                        ART_WF_ARTWORK_PROCESS_PA_RESULT paResult = new ART_WF_ARTWORK_PROCESS_PA_RESULT();
                        ART_WF_ARTWORK_PROCESS_PA_2 paRequestData = new ART_WF_ARTWORK_PROCESS_PA_2();

                        processPA = new ART_WF_ARTWORK_PROCESS_PA();
                        processPA.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                        processPA = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(processPA, context).FirstOrDefault();

                        if (processPA != null)
                        {
                            paRequestData = MapperServices.ART_WF_ARTWORK_PROCESS_PA(processPA);

                            if (String.IsNullOrEmpty(paRequestData.ARTWORK_NO))
                            {
                                var process = context.ART_WF_ARTWORK_PROCESS.Where(w => w.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).FirstOrDefault();
                                var artworkItem = context.ART_WF_ARTWORK_REQUEST_ITEM.Where(w => w.ARTWORK_ITEM_ID == process.ARTWORK_ITEM_ID).FirstOrDefault();

                                paRequestData.ARTWORK_NO = artworkItem.REQUEST_ITEM_NO;
                            }

                            paRequestData.RETRIVE_TYPE = "SUGGEST";
                            paRequestData.MATERIAL_NO = param.data.MATERIAL_NUMBER;
                            paRequest.data = paRequestData;

                            paResult = RetriveMaterial(paRequest, context);
                        }

                        dbContextTransaction.Commit();
                    }
                }

                Results.data = listResult;
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

        public static ART_WF_ARTWORK_PROCESS_PA_RESULT RetriveMaterial(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();

            try
            {
                if (param == null && param.data == null)
                {
                    return Results;
                }

                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        param.data.RETRIVE_TYPE = "RETRIVE";
                        Results = RetriveMaterial(param, context);
                        dbContextTransaction.Commit();

                        Results.status = "S";
                    }
                }
                CNService.UpdateMaterialLock(param.data.ARTWORK_SUB_ID);
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_PA_RESULT RetriveMaterial(ART_WF_ARTWORK_PROCESS_PA_REQUEST param, ARTWORKEntities context)
        {
            var currentUserId = CNService.getCurrentUser(context);
            ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();

            Dictionary<string, string> dicTypeOf = new Dictionary<string, string>();
            Dictionary<string, string> dicTypeOf2 = new Dictionary<string, string>();
            Dictionary<string, string> dicPlantRegister = new Dictionary<string, string>();
            Dictionary<string, string> dicPMSColour = new Dictionary<string, string>();
            Dictionary<string, string> dicCompany = new Dictionary<string, string>();
            Dictionary<string, string> dicProcessColour = new Dictionary<string, string>();
            Dictionary<string, string> dicCatchingPeriod = new Dictionary<string, string>();
            Dictionary<string, string> dicTotalColour = new Dictionary<string, string>();
            Dictionary<string, string> dicCatchingMethod = new Dictionary<string, string>();
            Dictionary<string, string> dicStyleOfPrinting = new Dictionary<string, string>();
            Dictionary<string, string> dicScientificName = new Dictionary<string, string>();
            Dictionary<string, string> dicDirectionOfSticker = new Dictionary<string, string>();
            Dictionary<string, string> dicSpecie = new Dictionary<string, string>();

            dicTypeOf.Add("C", "ZPKG_SEC_CARDBOARD_TYPE_1");
            dicTypeOf.Add("D", "ZPKG_SEC_DISPLAYER_TYPE_1");
            dicTypeOf.Add("F", "ZPKG_SEC_CARTON_TYPE_1");
            dicTypeOf.Add("G", "ZPKG_SEC_TRAY_TYPE");
            dicTypeOf.Add("H", "ZPKG_SEC_SLEEVE_BOX_TYPE");
            dicTypeOf.Add("J", "ZPKG_SEC_STICKER_TYPE");
            dicTypeOf.Add("K", "ZPKG_SEC_LABEL_TYPE");
            dicTypeOf.Add("L", "ZPKG_SEC_LEAFTLET_TYPE");
            dicTypeOf.Add("M", "ZPKG_SEC_STYLE_PLASTIC");
            dicTypeOf.Add("N", "ZPKG_SEC_INNER_TYPE_1");
            dicTypeOf.Add("P", "ZPKG_SEC_INSERT_TYPE");
            dicTypeOf.Add("R", "ZPKG_SEC_INNER_NON_TYPE");

            dicTypeOf2.Add("C", "ZPKG_SEC_CARDBOARD_TYPE_2");
            dicTypeOf2.Add("D", "ZPKG_SEC_DISPLAYER_TYPE_2");
            dicTypeOf2.Add("F", "ZPKG_SEC_CARTON_TYPE_2");
            dicTypeOf2.Add("G", "ZPKG_SEC_TRAY_CARTON_TYPE");
            dicTypeOf2.Add("N", "ZPKG_SEC_INNER_TYPE_2");
            dicTypeOf2.Add("R", "ZPKG_SEC_INNER_TYPE_2");

            dicPMSColour.Add("F", "ZPKG_SEC_CAR_PMS_COLOUR");
            dicPMSColour.Add("C", "ZPKG_SEC_CARD_PMS_COLOUR");
            dicPMSColour.Add("D", "ZPKG_SEC_DISP_PMS_COLOUR");
            dicPMSColour.Add("R", "ZPKG_SEC_INN_NO_PMS_COLOUR");
            dicPMSColour.Add("N", "ZPKG_SEC_INNER_PMS_COLOUR");
            dicPMSColour.Add("P", "ZPKG_SEC_INST_PMS_COLOUR");
            dicPMSColour.Add("K", "ZPKG_SEC_LABE_PMS_COLOUR");
            dicPMSColour.Add("L", "ZPKG_SEC_LEA_PMS_COLOUR");
            dicPMSColour.Add("H", "ZPKG_SEC_SLEV_PMS_COLOUR");
            dicPMSColour.Add("J", "ZPKG_SEC_STKC_PMS_COLOUR");
            dicPMSColour.Add("G", "ZPKG_SEC_TRAY_PMS_COLOUR");

            dicProcessColour.Add("F", "ZPKG_SEC_CAR_PROCESS_COLOUR");
            dicProcessColour.Add("C", "ZPKG_SEC_CARD_PROCESS_COLOUR");
            dicProcessColour.Add("D", "ZPKG_SEC_DISP_PROCESS_COLOUR");
            dicProcessColour.Add("R", "ZPKG_SEC_INN_NO_PROCESS_COLOUR");
            dicProcessColour.Add("N", "ZPKG_SEC_INNER_PROCESS_COLOUR");
            dicProcessColour.Add("P", "ZPKG_SEC_INST_PROCESS_COLOUR");
            dicProcessColour.Add("K", "ZPKG_SEC_LABE_PROCESS_COLOUR");
            dicProcessColour.Add("L", "ZPKG_SEC_LEA_PROCESS_COLOUR");
            dicProcessColour.Add("H", "ZPKG_SEC_SLEV_PROCESS_COLOUR");
            dicProcessColour.Add("J", "ZPKG_SEC_STKC_PROCESS_COLOUR");
            dicProcessColour.Add("G", "ZPKG_SEC_TRAY_PROCESS_COLOUR");

            dicTotalColour.Add("F", "ZPKG_SEC_CAR_TOTAL_COLOUR");
            dicTotalColour.Add("C", "ZPKG_SEC_CARD_TOTAL_COLOUR");
            dicTotalColour.Add("D", "ZPKG_SEC_DISP_TOTAL_COLOUR");
            dicTotalColour.Add("R", "ZPKG_SEC_INN_NO_TOTAL_COLOUR");
            dicTotalColour.Add("N", "ZPKG_SEC_INNER_TOTAL_COLOUR");
            dicTotalColour.Add("P", "ZPKG_SEC_INST_TOTAL_COLOUR");
            dicTotalColour.Add("K", "ZPKG_SEC_LABE_TOTAL_COLOUR");
            dicTotalColour.Add("L", "ZPKG_SEC_LEA_TOTAL_COLOUR");
            dicTotalColour.Add("H", "ZPKG_SEC_SLEV_TOTAL_COLOUR");
            dicTotalColour.Add("J", "ZPKG_SEC_STKC_TOTAL_COLOUR");
            dicTotalColour.Add("G", "ZPKG_SEC_TRAY_TOTAL_COLOUR");
            dicTotalColour.Add("M", "ZPKG_SEC_PLAST_TOTAL_COLOUR");

            dicStyleOfPrinting.Add("F", "ZPKG_SEC_CAR_PRINTING_STYLE");
            dicStyleOfPrinting.Add("C", "ZPKG_SEC_CARD_PRINTING_STYLE");
            dicStyleOfPrinting.Add("D", "ZPKG_SEC_DISP_PRINTING_STYLE");
            dicStyleOfPrinting.Add("R", "ZPKG_SEC_INN_NO_PRINTING_STYLE");
            dicStyleOfPrinting.Add("N", "ZPKG_SEC_INNER_PRINTING_STYLE");
            dicStyleOfPrinting.Add("P", "ZPKG_SEC_INST_PRINTING_STYLE");
            dicStyleOfPrinting.Add("K", "ZPKG_SEC_LABE_PRINTING_STYLE");
            dicStyleOfPrinting.Add("L", "ZPKG_SEC_LEA_PRINTING_STYLE");
            dicStyleOfPrinting.Add("H", "ZPKG_SEC_SLEV_PRINTING_STYLE");
            dicStyleOfPrinting.Add("J", "ZPKG_SEC_STKC_PRINTING_STYLE");
            dicStyleOfPrinting.Add("G", "ZPKG_SEC_TRAY_PRINTING_STYLE");

            ART_WF_ARTWORK_PROCESS_PA processPA = new ART_WF_ARTWORK_PROCESS_PA();

            if (param == null && param.data == null)
            {
                return Results;
            }

            if (!String.IsNullOrEmpty(param.data.ARTWORK_NO) && !String.IsNullOrEmpty(param.data.MATERIAL_NO))
            {
                string artwork_no = param.data.ARTWORK_NO;

                var header = (from h in context.IGRID_M_OUTBOUND_HEADER
                              where h.MATERIAL_NUMBER == param.data.MATERIAL_NO
                              select h).OrderByDescending(o => o.IGRID_OUTBOUND_HEADER_ID).FirstOrDefault();

                processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                             where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                             select p).OrderByDescending(o => o.ARTWORK_SUB_PA_ID).FirstOrDefault();

                if (header != null)
                {
                    var items = (from h in context.IGRID_M_OUTBOUND_ITEM
                                 where h.ARTWORK_NO == header.ARTWORK_NO
                                    && h.DATE == header.DATE
                                    && h.TIME == header.TIME
                                 select h).ToList();

                    if (processPA != null)
                    {
                        processPA.MATERIAL_NO = param.data.MATERIAL_NO;
                        processPA.CUSTOMER_DESIGN = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_DESIGN);
                        processPA.CUSTOMER_DESIGN_OTHER = header.CUSTOMER_DESIGN_DETAIL;
                        processPA.CUSTOMER_SPEC = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_SPEC);
                        processPA.CUSTOMER_SPEC_OTHER = header.CUSTOMER_SPEC_DETAIL;
                        processPA.CUSTOMER_SIZE = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_SIZE);
                        processPA.CUSTOMER_SIZE_OTHER = header.CUSTOMER_SIZE_DETAIL;
                        processPA.CUSTOMER_NOMINATES_VENDOR = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_NOMINATES_VENDOR);
                        processPA.CUSTOMER_NOMINATES_VENDOR_OTHER = header.CUSTOMER_NOMINATES_VENDOR_DETAIL;
                        processPA.CUSTOMER_NOMINATES_COLOR = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_NOMINATES_COLOR);
                        processPA.CUSTOMER_NOMINATES_COLOR_OTHER = header.CUSTOMER_NOMINATES_COLOR_DETAIL;
                        processPA.CUSTOMER_BARCODE_SCANABLE = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_BARCODE_SCANABLE);
                        processPA.CUSTOMER_BARCODE_SCANABLE_OTHER = header.CUSTOMER_BARCODE_SCANABLE_DETAIL;
                        processPA.CUSTOMER_BARCODE_SPEC = MaterialIGridHelper.ConvertYesNoValue(header.CUSTOMER_BARCODE_SPEC);
                        processPA.CUSTOMER_BARCODE_SPEC_OTHER = header.CUSTOMER_BARCODE_SPEC_DETAIL;
                        processPA.FIRST_INFOGROUP_OTHER = header.FIRST_INFO_GROUP;
                        processPA.NOTE_OF_PA = header.NOTE_OF_PA;
                        processPA.FIRST_INFOGROUP_OTHER = header.FINAL_INFO_GROUP;
                        processPA.COMPLETE_INFOGROUP = header.COMPLETE_INFO_GROUP;
                        processPA.PRODUCTION_EXPIRY_DATE_SYSTEM = header.EXPIRY_DATE_SYSTEM;
                        processPA.SERIOUSNESS_OF_COLOR_PRINTING = MaterialIGridHelper.ConvertYesNoValue(header.SERIOUSNESS_OF_COLOR_PRINTING);
                        processPA.NUTRITION_ANALYSIS = MaterialIGridHelper.ConvertYesNoValue(header.ANALYSIS);
                        processPA.PACKAGE_QUANTITY = header.PACKAGE_QUANTITY.ToString();
                        processPA.WASTE_PERCENT = header.WASTE_PERCENT.ToString();
                        processPA.REQUEST_MATERIAL_STATUS = header.STATUS;

                        if (param.data.RETRIVE_TYPE == "RETRIVE")
                        {
                            processPA.REQUEST_MATERIAL_STATUS = "Completed";
                        }

                        var ZPKG_SEC_GROUP = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_GROUP", context);
                        if (ZPKG_SEC_GROUP != null)
                        {
                            processPA.MATERIAL_GROUP_ID = ZPKG_SEC_GROUP;

                            var matG = context.SAP_M_CHARACTERISTIC.Where(m => m.CHARACTERISTIC_ID == ZPKG_SEC_GROUP).FirstOrDefault();

                            string typeOf = "";// dicTypeOf[matG.VALUE];
                            if (dicTypeOf.TryGetValue(matG.VALUE, out typeOf))
                            {
                                typeOf = dicTypeOf[matG.VALUE];
                            }

                            string typeOf2 = "";// dicTypeOf2[matG.VALUE];
                            if (dicTypeOf2.TryGetValue(matG.VALUE, out typeOf2))
                            {
                                typeOf2 = dicTypeOf2[matG.VALUE];
                            }

                            string pmsColour = "";// dicPMSColour[matG.VALUE];
                            if (dicPMSColour.TryGetValue(matG.VALUE, out pmsColour))
                            {
                                pmsColour = dicPMSColour[matG.VALUE];
                            }

                            string processColour = "";// dicProcessColour[matG.VALUE];
                            if (dicProcessColour.TryGetValue(matG.VALUE, out processColour))
                            {
                                processColour = dicProcessColour[matG.VALUE];
                            }

                            string totalColour = "";// dicTotalColour[matG.VALUE];
                            if (dicTotalColour.TryGetValue(matG.VALUE, out totalColour))
                            {
                                totalColour = dicTotalColour[matG.VALUE];
                            }

                            string stylePrinting = "";// dicStyleOfPrinting[matG.VALUE];
                            if (dicStyleOfPrinting.TryGetValue(matG.VALUE, out stylePrinting))
                            {
                                stylePrinting = dicStyleOfPrinting[matG.VALUE];
                            }

                            if (!String.IsNullOrEmpty(typeOf))
                            {
                                var TYPE_OF = MaterialIGridHelper.GetPACharacteristicID(items, typeOf, context);
                                if (TYPE_OF != null)
                                {
                                    processPA.TYPE_OF_ID = TYPE_OF;
                                }
                                else
                                {
                                    processPA.TYPE_OF_ID = null;
                                    processPA.TYPE_OF_OTHER = null;
                                }
                            }

                            if (!String.IsNullOrEmpty(typeOf2))
                            {
                                var TYPE_OF_2 = MaterialIGridHelper.GetPACharacteristicID(items, typeOf2, context);
                                if (TYPE_OF_2 != null)
                                {
                                    processPA.TYPE_OF_2_ID = TYPE_OF_2;
                                }
                                else
                                {
                                    processPA.TYPE_OF_2_ID = null;
                                    processPA.TYPE_OF_2_OTHER = null;
                                }
                            }

                            if (!String.IsNullOrEmpty(pmsColour))
                            {
                                var PMS_COLOUR = MaterialIGridHelper.GetPACharacteristicID(items, pmsColour, context);
                                if (PMS_COLOUR != null)
                                {
                                    processPA.PMS_COLOUR_ID = PMS_COLOUR;
                                }
                                else
                                {
                                    processPA.PMS_COLOUR_ID = null;
                                    processPA.PMS_COLOUR_OTHER = null;
                                }
                            }

                            if (!String.IsNullOrEmpty(processColour))
                            {
                                var PROCESS_COLOUR = MaterialIGridHelper.GetPACharacteristicID(items, processColour, context);
                                if (PROCESS_COLOUR != null)
                                {
                                    processPA.PROCESS_COLOUR_ID = PROCESS_COLOUR;
                                }
                                else
                                {
                                    processPA.PROCESS_COLOUR_ID = null;
                                    processPA.PROCESS_COLOUR_OTHER = null;
                                }
                            }

                            if (!String.IsNullOrEmpty(totalColour))
                            {
                                var TOTAL_COLOUR = MaterialIGridHelper.GetPACharacteristicID(items, totalColour, context);
                                if (TOTAL_COLOUR != null)
                                {
                                    processPA.TOTAL_COLOUR_ID = TOTAL_COLOUR;
                                }
                                else
                                {
                                    processPA.TOTAL_COLOUR_ID = null;
                                    processPA.TOTAL_COLOUR_OTHER = null;
                                }
                            }

                            if (!String.IsNullOrEmpty(stylePrinting))
                            {
                                var STYLE_PRINTING = MaterialIGridHelper.GetPACharacteristicID(items, stylePrinting, context);
                                if (STYLE_PRINTING != null)
                                {
                                    processPA.STYLE_OF_PRINTING_ID = STYLE_PRINTING;
                                }
                                else
                                {
                                    processPA.STYLE_OF_PRINTING_ID = null;
                                    processPA.STYLE_OF_PRINTING_OTHER = null;
                                }
                            }
                        }

                        var ZPKG_SEC_PRIMARY_SIZE_VALUE = MaterialIGridHelper.GetIgridItem(items, "ZPKG_SEC_PRIMARY_SIZE", context);
                        var ZPKG_SEC_CONTAINER_TYPE_VALUE = MaterialIGridHelper.GetIgridItem(items, "ZPKG_SEC_CONTAINER_TYPE", context);
                        var ZPKG_SEC_LID_TYPE_VALUE = MaterialIGridHelper.GetIgridItem(items, "ZPKG_SEC_LID_TYPE", context);

                        if ((ZPKG_SEC_PRIMARY_SIZE_VALUE != null && !String.IsNullOrEmpty(ZPKG_SEC_PRIMARY_SIZE_VALUE.CHARACTERISTIC_DESCRIPTION))
                           && (ZPKG_SEC_CONTAINER_TYPE_VALUE != null && !String.IsNullOrEmpty(ZPKG_SEC_CONTAINER_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION))
                           && (ZPKG_SEC_LID_TYPE_VALUE != null && !String.IsNullOrEmpty(ZPKG_SEC_LID_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION)))
                        {
                            var threeP = (from p in context.SAP_M_3P
                                          where p.PRIMARY_SIZE_VALUE == ZPKG_SEC_PRIMARY_SIZE_VALUE.CHARACTERISTIC_DESCRIPTION
                                          && p.CONTAINER_TYPE_VALUE == ZPKG_SEC_CONTAINER_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION
                                          && p.LID_TYPE_VALUE == ZPKG_SEC_LID_TYPE_VALUE.CHARACTERISTIC_DESCRIPTION
                                          select p).FirstOrDefault();

                            if (threeP != null)
                            {
                                processPA.THREE_P_ID = threeP.THREE_P_ID;
                            }
                            else
                            {
                                processPA.THREE_P_ID = null;
                                processPA.PRIMARY_SIZE_ID = null;
                                processPA.CONTAINER_TYPE_ID = null;
                                processPA.LID_TYPE_ID = null;

                                processPA.PRIMARY_SIZE_OTHER = null;
                                processPA.CONTAINER_TYPE_OTHER = null;
                                processPA.LID_TYPE_OTHER = null;

                                var ZPKG_SEC_PRIMARY_SIZE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PRIMARY_SIZE", context);
                                if (ZPKG_SEC_PRIMARY_SIZE != null)
                                {
                                    processPA.PRIMARY_SIZE_ID = ZPKG_SEC_PRIMARY_SIZE;
                                }

                                var ZPKG_SEC_CONTAINER_TYPE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_CONTAINER_TYPE", context);
                                if (ZPKG_SEC_CONTAINER_TYPE != null)
                                {
                                    processPA.CONTAINER_TYPE_ID = ZPKG_SEC_CONTAINER_TYPE;
                                }

                                var ZPKG_SEC_LID_TYPE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_LID_TYPE", context);
                                if (ZPKG_SEC_LID_TYPE != null)
                                {
                                    processPA.LID_TYPE_ID = ZPKG_SEC_LID_TYPE;
                                }
                            }
                        }
                        else
                        {
                            processPA.THREE_P_ID = null;
                            processPA.PRIMARY_SIZE_ID = null;
                            processPA.CONTAINER_TYPE_ID = null;
                            processPA.LID_TYPE_ID = null;

                            processPA.PRIMARY_SIZE_OTHER = null;
                            processPA.CONTAINER_TYPE_OTHER = null;
                            processPA.LID_TYPE_OTHER = null;

                            var ZPKG_SEC_PRIMARY_SIZE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PRIMARY_SIZE", context);
                            if (ZPKG_SEC_PRIMARY_SIZE != null)
                            {
                                processPA.PRIMARY_SIZE_ID = ZPKG_SEC_PRIMARY_SIZE;
                            }

                            var ZPKG_SEC_CONTAINER_TYPE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_CONTAINER_TYPE", context);
                            if (ZPKG_SEC_CONTAINER_TYPE != null)
                            {
                                processPA.CONTAINER_TYPE_ID = ZPKG_SEC_CONTAINER_TYPE;
                            }

                            var ZPKG_SEC_LID_TYPE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_LID_TYPE", context);
                            if (ZPKG_SEC_LID_TYPE != null)
                            {
                                processPA.LID_TYPE_ID = ZPKG_SEC_LID_TYPE;
                            }
                        }

                        var ZPKG_SEC_PACKING_STYLE_VALUE = MaterialIGridHelper.GetIgridItem(items, "ZPKG_SEC_PACKING_STYLE", context);
                        var ZPKG_SEC_PACKING_VALUE = MaterialIGridHelper.GetIgridItem(items, "ZPKG_SEC_PACKING", context);

                        if ((ZPKG_SEC_PACKING_STYLE_VALUE != null && !String.IsNullOrEmpty(ZPKG_SEC_PACKING_STYLE_VALUE.CHARACTERISTIC_DESCRIPTION))
                           && (ZPKG_SEC_PACKING_VALUE != null && !String.IsNullOrEmpty(ZPKG_SEC_PACKING_VALUE.CHARACTERISTIC_DESCRIPTION)))
                        {
                            var twoP = (from p in context.SAP_M_2P
                                        where p.PACKING_SYLE_VALUE == ZPKG_SEC_PACKING_STYLE_VALUE.CHARACTERISTIC_DESCRIPTION
                                        && p.PACK_SIZE_VALUE == ZPKG_SEC_PACKING_VALUE.CHARACTERISTIC_DESCRIPTION
                                        select p).FirstOrDefault();

                            if (twoP != null)
                            {
                                processPA.TWO_P_ID = twoP.TWO_P_ID;
                            }
                            else
                            {
                                processPA.TWO_P_ID = null;
                                processPA.PACKING_STYLE_ID = null;
                                processPA.PACK_SIZE_ID = null;

                                processPA.PACKING_STYLE_OTHER = null;
                                processPA.PACK_SIZE_OTHER = null;

                                var ZPKG_SEC_PACKING_STYLE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PACKING_STYLE", context);
                                if (ZPKG_SEC_PACKING_STYLE != null)
                                {
                                    processPA.PACKING_STYLE_ID = ZPKG_SEC_PACKING_STYLE;
                                }

                                var ZPKG_SEC_PACKING = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PACKING", context);
                                if (ZPKG_SEC_PACKING != null)
                                {
                                    processPA.PACK_SIZE_ID = ZPKG_SEC_PACKING;
                                }

                            }
                        }
                        else
                        {
                            processPA.TWO_P_ID = null;
                            processPA.PACKING_STYLE_ID = null;
                            processPA.PACK_SIZE_ID = null;

                            processPA.PACKING_STYLE_OTHER = null;
                            processPA.PACK_SIZE_OTHER = null;

                            var ZPKG_SEC_PACKING_STYLE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PACKING_STYLE", context);
                            if (ZPKG_SEC_PACKING_STYLE != null)
                            {
                                processPA.PACKING_STYLE_ID = ZPKG_SEC_PACKING_STYLE;
                            }

                            var ZPKG_SEC_PACKING = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PACKING", context);
                            if (ZPKG_SEC_PACKING != null)
                            {
                                processPA.PACK_SIZE_ID = ZPKG_SEC_PACKING;
                            }
                        }

                        var ZPKG_SEC_PLANT_REGISTER = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_PLANT_REGISTER", context);
                        if (ZPKG_SEC_PLANT_REGISTER != null)
                        {
                            processPA.PLANT_REGISTERED_ID = ZPKG_SEC_PLANT_REGISTER;
                        }
                        else
                        {
                            processPA.PLANT_REGISTERED_ID = null;
                            processPA.PLANT_REGISTERED_OTHER = null;
                        }

                        var ZPKG_SEC_COMPANY_ADR = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_COMPANY_ADR", context);
                        if (ZPKG_SEC_COMPANY_ADR != null)
                        {
                            processPA.COMPANY_ADDRESS_ID = ZPKG_SEC_COMPANY_ADR;
                        }
                        else
                        {
                            processPA.COMPANY_ADDRESS_ID = null;
                            processPA.COMPANY_ADDRESS_OTHER = null;
                        }

                        var ZPKG_SEC_CATCHING_PERIOD = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_CATCHING_PERIOD", context);
                        if (ZPKG_SEC_CATCHING_PERIOD != null)
                        {
                            processPA.CATCHING_PERIOD_ID = ZPKG_SEC_CATCHING_PERIOD;
                        }
                        else
                        {
                            processPA.CATCHING_PERIOD_ID = null;
                            processPA.CATCHING_PERIOD_OTHER = null;
                        }

                        var ZPKG_SEC_CATCHING_METHOD = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_CATCHING_METHOD", context);
                        if (ZPKG_SEC_CATCHING_METHOD != null)
                        {
                            processPA.CATCHING_METHOD_ID = ZPKG_SEC_CATCHING_METHOD;
                        }
                        else
                        {
                            processPA.CATCHING_METHOD_ID = null;
                            processPA.CATCHING_METHOD_OTHER = null;
                        }

                        var ZPKG_SEC_SCIENTIFIC_NAME = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_SCIENTIFIC_NAME", context);
                        if (ZPKG_SEC_SCIENTIFIC_NAME != null)
                        {
                            processPA.SCIENTIFIC_NAME_ID = ZPKG_SEC_SCIENTIFIC_NAME;
                        }
                        else
                        {
                            processPA.SCIENTIFIC_NAME_ID = null;
                            processPA.SCIENTIFIC_NAME_OTHER = null;
                        }

                        var ZPKG_SEC_DIRECTION = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_DIRECTION", context);
                        if (ZPKG_SEC_DIRECTION != null)
                        {
                            processPA.DIRECTION_OF_STICKER_ID = ZPKG_SEC_DIRECTION;
                        }

                        var ZPKG_SEC_SPECIE = MaterialIGridHelper.GetPACharacteristicID(items, "ZPKG_SEC_SPECIE", context);
                        if (ZPKG_SEC_SPECIE != null)
                        {
                            processPA.SPECIE_ID = ZPKG_SEC_SPECIE;
                        }
                        else
                        {
                            processPA.SPECIE_ID = null;
                            processPA.SPECIE_OTHER = null;
                        }

                        if (param.data.RETRIVE_TYPE == "SUGGEST")
                        {
                            processPA.MATERIAL_NO = "";
                            processPA.REQUEST_MATERIAL_STATUS = "";
                        }

                        var productionPlants = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_PRODUCTION_PLANT").ToList();

                        if (productionPlants != null)
                        {
                            string productionPlant = "";
                            foreach (var iProductPlant in productionPlants)
                            {
                                if (!String.IsNullOrEmpty(iProductPlant.CHARACTERISTIC_VALUE) && !productionPlant.Contains(iProductPlant.CHARACTERISTIC_VALUE))
                                {
                                    if (String.IsNullOrEmpty(productionPlant))
                                    {
                                        productionPlant = iProductPlant.CHARACTERISTIC_VALUE;
                                    }
                                    else
                                    {
                                        productionPlant += "," + iProductPlant.CHARACTERISTIC_VALUE;
                                    }
                                }
                            }

                            if (!String.IsNullOrEmpty(productionPlant))
                            {
                                processPA.PRODICUTION_PLANT_ID = -1;
                                processPA.PRODICUTION_PLANT_OTHER = productionPlant;
                            }
                        }
                        else
                        {
                            processPA.PRODICUTION_PLANT_ID = null;
                            processPA.PRODICUTION_PLANT_OTHER = null;
                        }

                        ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA, context);

                        #region "PLANT"
                        var plant = context.ART_WF_ARTWORK_PROCESS_PA_PLANT
                                    .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                    .ToList();
                        if (plant != null && plant.Count > 0)
                        {
                            foreach (ART_WF_ARTWORK_PROCESS_PA_PLANT iPlant in plant)
                            {
                                ART_WF_ARTWORK_PROCESS_PA_PLANT_SERVICE.DeleteByARTWORK_SUB_PA_PLANT_ID(iPlant.ARTWORK_SUB_PA_PLANT_ID, context);
                            }
                        }

                        if (!String.IsNullOrEmpty(header.PLANT))
                        {
                            List<string> listPlants = new List<string>();

                            listPlants = header.PLANT.Split(new string[] { ";" }, StringSplitOptions.None).ToList();

                            if (listPlants.Count > 0)
                            {
                                ART_WF_ARTWORK_PROCESS_PA_PLANT plantNew = new ART_WF_ARTWORK_PROCESS_PA_PLANT();
                                foreach (string iPlant in listPlants)
                                {
                                    var plantTmp = (from p in context.SAP_M_PLANT
                                                    where p.PLANT == iPlant
                                                    select p).FirstOrDefault();

                                    if (plantTmp != null)
                                    {
                                        plantNew = new ART_WF_ARTWORK_PROCESS_PA_PLANT();
                                        plantNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        plantNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        plantNew.PLANT_ID = plantTmp.PLANT_ID;
                                        plantNew.CREATE_BY = currentUserId;
                                        plantNew.UPDATE_BY = currentUserId;
                                        ART_WF_ARTWORK_PROCESS_PA_PLANT_SERVICE.SaveOrUpdateNoLog(plantNew, context);
                                    }
                                }
                            }
                        }


                        #endregion

                        #region "FAO_ZONE"
                        var fao = context.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE
                                    .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                    .ToList();
                        if (fao != null && fao.Count > 0)
                        {
                            foreach (ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE iFAO in fao)
                            {
                                ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.DeleteByARTWORK_SUB_PA_FAO_ID(iFAO.ARTWORK_SUB_PA_FAO_ID, context);
                            }
                        }

                        var faoNews = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_FAO").ToList();
                        if (faoNews != null && faoNews.Count > 0)
                        {
                            ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE fao_new = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                            foreach (var iFAONew in faoNews)
                            {
                                var fap_tmp = (from c in context.SAP_M_CHARACTERISTIC
                                               where c.NAME == "ZPKG_SEC_FAO"
                                                 && c.VALUE == iFAONew.CHARACTERISTIC_VALUE
                                               select c).FirstOrDefault();

                                if (fap_tmp != null)
                                {
                                    fao_new = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                                    fao_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                    fao_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                    fao_new.FAO_ZONE_ID = fap_tmp.CHARACTERISTIC_ID;
                                    fao_new.CREATE_BY = currentUserId;
                                    fao_new.UPDATE_BY = currentUserId;
                                    ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.SaveOrUpdateNoLog(fao_new, context);
                                }
                            }
                        }

                        #endregion

                        #region "CATCHING_AREA"
                        var catchingArea = context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA
                                  .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                  .ToList();
                        if (catchingArea != null && catchingArea.Count > 0)
                        {
                            foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA iChatching in catchingArea)
                            {
                                ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE.DeleteByARTWORK_SUB_PA_CATCHING_AREA_ID(iChatching.ARTWORK_SUB_PA_CATCHING_AREA_ID, context);
                            }
                        }

                        var catchingNews = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_CATCHING_AREA").ToList();

                        if (catchingNews != null && catchingNews.Count > 0)
                        {
                            ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA catching_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                            foreach (var iCatchingNew in catchingNews)
                            {
                                var catching_tmp = (from c in context.SAP_M_CHARACTERISTIC
                                                    where c.NAME == "ZPKG_SEC_CATCHING_AREA"
                                                      && c.VALUE == iCatchingNew.CHARACTERISTIC_VALUE
                                                    select c).FirstOrDefault();

                                if (catching_tmp != null)
                                {
                                    catching_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                                    catching_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                    catching_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                    catching_new.CATCHING_AREA_ID = catching_tmp.CHARACTERISTIC_ID;
                                    catching_new.CREATE_BY = currentUserId;
                                    catching_new.UPDATE_BY = currentUserId;
                                    ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE.SaveOrUpdateNoLog(catching_new, context);
                                }
                            }
                        }

                        #endregion


                        #region "CATCHING_METHOD"
                        // ticke#425737 added by aof 
                        var catchingMethod = context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD
                                  .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                  .ToList();
                        if (catchingMethod != null && catchingMethod.Count > 0)
                        {
                            foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD iMethod in catchingMethod)
                            {
                                ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE.DeleteByARTWORK_SUB_PA_CATCHING_METHOD_ID(iMethod.ARTWORK_SUB_PA_CATCHING_METHOD_ID, context);
                            }
                        }

                        var methodNews = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_CATCHING_METHOD").ToList();

                        if (methodNews != null && methodNews.Count > 0)
                        {
                            ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD method_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                            foreach (var iMethodNew in methodNews)
                            {
                                var method_tmp = (from c in context.SAP_M_CHARACTERISTIC
                                                    where c.NAME == "ZPKG_SEC_CATCHING_METHOD"
                                                      && c.VALUE == iMethodNew.CHARACTERISTIC_VALUE
                                                    select c).FirstOrDefault();

                                if (method_tmp != null)
                                {
                                    method_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                                    method_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                    method_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                    method_new.CATCHING_METHOD_ID = method_tmp.CHARACTERISTIC_ID;
                                    method_new.CREATE_BY = currentUserId;
                                    method_new.UPDATE_BY = currentUserId;
                                    ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE.SaveOrUpdateNoLog(method_new, context);
                                }
                            }
                        }
                        // ticke#425737 added by aof 
                        #endregion


                        #region "SYMBOL"
                        var symbol = context.ART_WF_ARTWORK_PROCESS_PA_SYMBOL
                                  .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                  .ToList();
                        if (symbol != null && symbol.Count > 0)
                        {
                            foreach (ART_WF_ARTWORK_PROCESS_PA_SYMBOL iChatching in symbol)
                            {
                                ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE.DeleteByARTWORK_SUB_PA_SYMBOL_ID(iChatching.ARTWORK_SUB_PA_SYMBOL_ID, context);
                            }
                        }

                        var symbolNews = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_SYMBOL").ToList();

                        if (symbolNews != null && symbolNews.Count > 0)
                        {
                            ART_WF_ARTWORK_PROCESS_PA_SYMBOL symbol_new = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                            foreach (var iSymbolNew in symbolNews)
                            {
                                var symbol_tmp = (from c in context.SAP_M_CHARACTERISTIC
                                                  where c.NAME == "ZPKG_SEC_SYMBOL"
                                                    && c.VALUE == iSymbolNew.CHARACTERISTIC_VALUE
                                                  select c).FirstOrDefault();

                                if (symbol_tmp != null)
                                {
                                    symbol_new = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                                    symbol_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                    symbol_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                    symbol_new.SYMBOL_ID = symbol_tmp.CHARACTERISTIC_ID;
                                    symbol_new.CREATE_BY = currentUserId;
                                    symbol_new.UPDATE_BY = currentUserId;
                                    ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE.SaveOrUpdateNoLog(symbol_new, context);
                                }
                            }
                        }

                        #endregion

                        #region "PRODUCT"
                        var product = context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT
                                    .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                    .ToList();

                        var productOther = context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER
                                  .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                  .ToList();

                        if (product != null && product.Count > 0)
                        {
                            foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT iProduct in product)
                            {
                                ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.DeleteByARTWORK_SUB_PA_PRODUCT_ID(iProduct.ARTWORK_SUB_PA_PRODUCT_ID, context);
                            }

                            foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER iProduct in productOther)
                            {
                                ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.DeleteByARTWORK_SUB_PA_PRODUCT_OTHER_ID(iProduct.ARTWORK_SUB_PA_PRODUCT_OTHER_ID, context);
                            }

                        }

                        var listProduct = items.Where(i => i.CHARACTERISTIC_NAME == "ZPKG_SEC_PRODUCT_CODE").ToList();

                        if (listProduct != null)
                        {
                            var listProductCode = listProduct.Select(s => s.CHARACTERISTIC_VALUE).ToList();

                            if (listProductCode != null)
                            {
                                var xProduct = (from p in context.XECM_M_PRODUCT
                                                where listProductCode.Contains(p.PRODUCT_CODE)
                                                select p).ToList();

                                ART_WF_ARTWORK_PROCESS_PA_PRODUCT productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                                foreach (IGRID_M_OUTBOUND_ITEM iProduct in listProduct)
                                {
                                    int productID = xProduct.Where(w => w.PRODUCT_CODE == iProduct.CHARACTERISTIC_VALUE)
                                                            .Select(s => s.XECM_PRODUCT_ID).FirstOrDefault();

                                    if (productID > 0)
                                    {
                                        productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                                        productNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        productNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        productNew.PRODUCT_CODE_ID = productID;
                                        productNew.CREATE_BY = currentUserId;
                                        productNew.UPDATE_BY = currentUserId;
                                        ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.SaveOrUpdateNoLog(productNew, context);
                                    }
                                    else
                                    {
                                        ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER productOtherNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();

                                        productOtherNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();
                                        productOtherNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        productOtherNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        productOtherNew.PRODUCT_CODE = iProduct.CHARACTERISTIC_VALUE;
                                        productOtherNew.CREATE_BY = currentUserId;
                                        productOtherNew.UPDATE_BY = currentUserId;
                                        ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.SaveOrUpdateNoLog(productOtherNew, context);
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        GetDataConversion(param, dicTypeOf, dicTypeOf2, dicPMSColour, dicProcessColour, dicStyleOfPrinting, processPA, context, dicTotalColour);
                    }
                }
                else
                {
                    GetDataConversion(param, dicTypeOf, dicTypeOf2, dicPMSColour, dicProcessColour, dicStyleOfPrinting, processPA, context, dicTotalColour);
                }

            }

            return Results;
        }

        private static void GetDataConversion(ART_WF_ARTWORK_PROCESS_PA_REQUEST param, Dictionary<string, string> dicTypeOf, Dictionary<string, string> dicTypeOf2, Dictionary<string, string> dicPMSColour, Dictionary<string, string> dicProcessColour, Dictionary<string, string> dicStyleOfPrinting, ART_WF_ARTWORK_PROCESS_PA processPA, ARTWORKEntities context, Dictionary<string, string> dicTotalColour)
        {
            var currentUserId = CNService.getCurrentUser(context);
            string matNO = "";

            matNO = processPA.MATERIAL_NO;

            if (param.data.RETRIVE_TYPE == "SUGGEST")
            {
                matNO = processPA.REFERENCE_MATERIAL;
            }
            else if (param.data.RETRIVE_TYPE == "RETRIVE")
            {
                matNO = param.data.MATERIAL_NO;
                processPA.REQUEST_MATERIAL_STATUS = "Completed";
            }

            if (!String.IsNullOrEmpty(matNO))
            {
                var matConversion = (from m in context.SAP_M_MATERIAL_CONVERSION
                                     where m.MATERIAL_NO == matNO
                                     select m).ToList();

                SAP_M_CHARACTERISTIC charMat = new SAP_M_CHARACTERISTIC();

                if (matConversion.Count > 0)
                {
                    var matG = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_GROUP").FirstOrDefault();

                    var ZPKG_SEC_GROUP = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_GROUP").FirstOrDefault();
                    if (ZPKG_SEC_GROUP != null)
                    {
                        charMat = new SAP_M_CHARACTERISTIC();
                        charMat = CNService.GetCharacteristicData(ZPKG_SEC_GROUP.CHAR_NAME, ZPKG_SEC_GROUP.CHAR_VALUE, context);

                        if (charMat != null)
                        {
                            processPA.MATERIAL_GROUP_ID = charMat.CHARACTERISTIC_ID;
                        }
                    }
                    else
                    {
                        processPA.MATERIAL_GROUP_ID = null;
                    }

                    var ZPKG_SEC_PRIMARY_SIZE = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_PRIMARY_SIZE").FirstOrDefault();
                    var ZPKG_SEC_CONTAINER_TYPE = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_CONTAINER_TYPE").FirstOrDefault();
                    var ZPKG_SEC_LID_TYPE = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_LID_TYPE").FirstOrDefault();

                    if (ZPKG_SEC_PRIMARY_SIZE != null && ZPKG_SEC_CONTAINER_TYPE != null && ZPKG_SEC_LID_TYPE != null)
                    {
                        var three_p = (from p in context.SAP_M_3P
                                       where p.PRIMARY_SIZE_VALUE == ZPKG_SEC_PRIMARY_SIZE.CHAR_VALUE
                                         && p.CONTAINER_TYPE_VALUE == ZPKG_SEC_CONTAINER_TYPE.CHAR_VALUE
                                         && p.LID_TYPE_VALUE == ZPKG_SEC_LID_TYPE.CHAR_VALUE
                                       select p).FirstOrDefault();

                        if (three_p != null)
                        {
                            processPA.THREE_P_ID = three_p.THREE_P_ID;
                        }
                        else
                        {
                            processPA.THREE_P_ID = null;
                            processPA.PRIMARY_SIZE_ID = null;
                            processPA.PRIMARY_SIZE_OTHER = null;
                            processPA.CONTAINER_TYPE_ID = null;
                            processPA.CONTAINER_TYPE_OTHER = null;
                            processPA.LID_TYPE_ID = null;
                            processPA.LID_TYPE_OTHER = null;

                            if (ZPKG_SEC_PRIMARY_SIZE != null)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(ZPKG_SEC_PRIMARY_SIZE.CHAR_NAME, ZPKG_SEC_PRIMARY_SIZE.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    processPA.PRIMARY_SIZE_ID = charMat.CHARACTERISTIC_ID;
                                }
                                else
                                {
                                    processPA.THREE_P_ID = -1;
                                    processPA.PRIMARY_SIZE_ID = -1;
                                    processPA.PRIMARY_SIZE_OTHER = ZPKG_SEC_PRIMARY_SIZE.CHAR_VALUE;
                                }
                            }

                            if (ZPKG_SEC_CONTAINER_TYPE != null)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(ZPKG_SEC_CONTAINER_TYPE.CHAR_NAME, ZPKG_SEC_CONTAINER_TYPE.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    processPA.CONTAINER_TYPE_ID = charMat.CHARACTERISTIC_ID;
                                }
                                else
                                {
                                    processPA.CONTAINER_TYPE_ID = -1;
                                    processPA.CONTAINER_TYPE_OTHER = ZPKG_SEC_CONTAINER_TYPE.CHAR_VALUE;
                                }
                            }

                            if (ZPKG_SEC_LID_TYPE != null)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(ZPKG_SEC_LID_TYPE.CHAR_NAME, ZPKG_SEC_LID_TYPE.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    processPA.LID_TYPE_ID = charMat.CHARACTERISTIC_ID;
                                }
                                else
                                {
                                    processPA.LID_TYPE_ID = -1;
                                    processPA.LID_TYPE_OTHER = ZPKG_SEC_LID_TYPE.CHAR_VALUE;
                                }
                            }
                        }
                    }
                    else
                    {
                        processPA.THREE_P_ID = null;
                        processPA.PRIMARY_SIZE_ID = null;
                        processPA.PRIMARY_SIZE_OTHER = null;
                        processPA.CONTAINER_TYPE_ID = null;
                        processPA.CONTAINER_TYPE_OTHER = null;
                        processPA.LID_TYPE_ID = null;
                        processPA.LID_TYPE_OTHER = null;

                        if (ZPKG_SEC_PRIMARY_SIZE != null)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(ZPKG_SEC_PRIMARY_SIZE.CHAR_NAME, ZPKG_SEC_PRIMARY_SIZE.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                processPA.PRIMARY_SIZE_ID = charMat.CHARACTERISTIC_ID;
                            }
                            else
                            {
                                processPA.THREE_P_ID = -1;
                                processPA.PRIMARY_SIZE_ID = -1;
                                processPA.PRIMARY_SIZE_OTHER = ZPKG_SEC_PRIMARY_SIZE.CHAR_VALUE;
                            }
                        }

                        if (ZPKG_SEC_CONTAINER_TYPE != null)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(ZPKG_SEC_CONTAINER_TYPE.CHAR_NAME, ZPKG_SEC_CONTAINER_TYPE.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                processPA.CONTAINER_TYPE_ID = charMat.CHARACTERISTIC_ID;
                            }
                            else
                            {
                                processPA.CONTAINER_TYPE_ID = -1;
                                processPA.CONTAINER_TYPE_OTHER = ZPKG_SEC_CONTAINER_TYPE.CHAR_VALUE;
                            }
                        }

                        if (ZPKG_SEC_LID_TYPE != null)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(ZPKG_SEC_LID_TYPE.CHAR_NAME, ZPKG_SEC_LID_TYPE.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                processPA.LID_TYPE_ID = charMat.CHARACTERISTIC_ID;
                            }
                            else
                            {
                                processPA.LID_TYPE_ID = -1;
                                processPA.LID_TYPE_OTHER = ZPKG_SEC_LID_TYPE.CHAR_VALUE;
                            }
                        }
                    }

                    var ZPKG_SEC_PACKING_STYLE = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_PACKING_STYLE").FirstOrDefault();
                    var ZPKG_SEC_PACKING_SIZE = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_PACKING").FirstOrDefault();

                    if (ZPKG_SEC_PACKING_STYLE != null && ZPKG_SEC_PACKING_SIZE != null)
                    {
                        var two_p = (from p in context.SAP_M_2P
                                     where p.PACKING_SYLE_VALUE == ZPKG_SEC_PACKING_STYLE.CHAR_VALUE
                                      && p.PACK_SIZE_VALUE == ZPKG_SEC_PACKING_SIZE.CHAR_VALUE
                                     select p).FirstOrDefault();

                        if (two_p != null)
                        {
                            processPA.TWO_P_ID = two_p.TWO_P_ID;
                        }
                        else
                        {

                            processPA.TWO_P_ID = null;
                            processPA.PACKING_STYLE_ID = null;
                            processPA.PACKING_STYLE_OTHER = null;
                            processPA.PACK_SIZE_ID = null;
                            processPA.PACK_SIZE_OTHER = null;

                            if (ZPKG_SEC_PACKING_STYLE != null)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(ZPKG_SEC_PACKING_STYLE.CHAR_NAME, ZPKG_SEC_PACKING_STYLE.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    processPA.PACKING_STYLE_ID = charMat.CHARACTERISTIC_ID;
                                }
                                else
                                {
                                    processPA.TWO_P_ID = -1;
                                    processPA.PACKING_STYLE_ID = -1;
                                    processPA.PACKING_STYLE_OTHER = ZPKG_SEC_PACKING_STYLE.CHAR_VALUE;
                                }
                            }

                            if (ZPKG_SEC_PACKING_SIZE != null)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(ZPKG_SEC_PACKING_SIZE.CHAR_NAME, ZPKG_SEC_PACKING_SIZE.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    processPA.PACK_SIZE_ID = charMat.CHARACTERISTIC_ID;
                                }
                                else
                                {
                                    processPA.PACK_SIZE_ID = -1;
                                    processPA.PACK_SIZE_OTHER = ZPKG_SEC_PACKING_SIZE.CHAR_VALUE;
                                }
                            }
                        }
                    }
                    else
                    {
                        processPA.TWO_P_ID = null;
                        processPA.PACKING_STYLE_ID = null;
                        processPA.PACKING_STYLE_OTHER = null;
                        processPA.PACK_SIZE_ID = null;
                        processPA.PACK_SIZE_OTHER = null;

                        if (ZPKG_SEC_PACKING_STYLE != null)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(ZPKG_SEC_PACKING_STYLE.CHAR_NAME, ZPKG_SEC_PACKING_STYLE.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                processPA.PACKING_STYLE_ID = charMat.CHARACTERISTIC_ID;
                            }
                            else
                            {
                                processPA.TWO_P_ID = -1;
                                processPA.PACKING_STYLE_ID = -1;
                                processPA.PACKING_STYLE_OTHER = ZPKG_SEC_PACKING_STYLE.CHAR_VALUE;
                            }
                        }

                        if (ZPKG_SEC_PACKING_SIZE != null)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(ZPKG_SEC_PACKING_SIZE.CHAR_NAME, ZPKG_SEC_PACKING_SIZE.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                processPA.PACK_SIZE_ID = charMat.CHARACTERISTIC_ID;
                            }
                            else
                            {
                                processPA.PACK_SIZE_ID = -1;
                                processPA.PACK_SIZE_OTHER = ZPKG_SEC_PACKING_SIZE.CHAR_VALUE;
                            }
                        }
                    }

                    //var ZPKG_SEC_PRODUCT_CODE = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_PRODUCT_CODE").ToList();
                    //if (ZPKG_SEC_PRODUCT_CODE != null)
                    //{
                    //    ART_WF_ARTWORK_PROCESS_PA_PRODUCT productConv = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                    //    ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER productOtherConv = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();

                    //    foreach (var iProduct in ZPKG_SEC_PRODUCT_CODE)
                    //    {
                    //        if (!String.IsNullOrEmpty(iProduct.CHAR_VALUE))
                    //        {
                    //            var xecmProduct_P = (from p in context.XECM_M_PRODUCT
                    //                                 where p.PRODUCT_CODE == iProduct.CHAR_VALUE
                    //                                 select p).FirstOrDefault();

                    //            if (xecmProduct_P != null)
                    //            {
                    //                productConv = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                    //                productConv.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                    //                productConv.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                    //                productConv.PRODUCT_CODE_ID = xecmProduct_P.XECM_PRODUCT_ID;
                    //                productConv.CREATE_BY = currentUserId;
                    //                productConv.UPDATE_BY = currentUserId;
                    //                ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.SaveOrUpdateNoLog(productConv, context);
                    //            }
                    //            else
                    //            {
                    //                productOtherConv = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();
                    //                productOtherConv.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                    //                productOtherConv.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                    //                productOtherConv.PRODUCT_CODE = iProduct.CHAR_VALUE;
                    //                productOtherConv.CREATE_BY = currentUserId;
                    //                productOtherConv.UPDATE_BY = currentUserId;
                    //                ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.SaveOrUpdateNoLog(productOtherConv, context);
                    //            }
                    //        }
                    //    }
                    //}

                    var ZPKG_SEC_PLANT_REGISTER = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_PLANT_REGISTER").FirstOrDefault();
                    if (ZPKG_SEC_PLANT_REGISTER != null)
                    {
                        charMat = new SAP_M_CHARACTERISTIC();
                        charMat = CNService.GetCharacteristicData(ZPKG_SEC_PLANT_REGISTER.CHAR_NAME, ZPKG_SEC_PLANT_REGISTER.CHAR_VALUE, context);

                        if (charMat != null)
                        {
                            processPA.PLANT_REGISTERED_ID = charMat.CHARACTERISTIC_ID;
                        }
                        else
                        {
                            processPA.PLANT_REGISTERED_ID = -1;
                            processPA.PLANT_REGISTERED_OTHER = ZPKG_SEC_PLANT_REGISTER.CHAR_VALUE;
                        }
                    }
                    else
                    {
                        processPA.PLANT_REGISTERED_ID = null;
                        processPA.PLANT_REGISTERED_OTHER = null;
                    }

                    var ZPKG_SEC_COMPANY_ADR = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_COMPANY_ADR").FirstOrDefault();
                    if (ZPKG_SEC_COMPANY_ADR != null)
                    {
                        charMat = new SAP_M_CHARACTERISTIC();
                        charMat = CNService.GetCharacteristicData(ZPKG_SEC_COMPANY_ADR.CHAR_NAME, ZPKG_SEC_COMPANY_ADR.CHAR_VALUE, context);

                        if (charMat != null)
                        {
                            processPA.COMPANY_ADDRESS_ID = charMat.CHARACTERISTIC_ID;
                        }
                        else
                        {
                            processPA.COMPANY_ADDRESS_ID = -1;
                            processPA.COMPANY_ADDRESS_OTHER = ZPKG_SEC_COMPANY_ADR.CHAR_VALUE;
                        }
                    }
                    else
                    {
                        processPA.COMPANY_ADDRESS_ID = null;
                        processPA.COMPANY_ADDRESS_OTHER = null;
                    }

                    var ZPKG_SEC_CATCHING_PERIOD = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_CATCHING_PERIOD").FirstOrDefault();
                    if (ZPKG_SEC_CATCHING_PERIOD != null)
                    {
                        charMat = new SAP_M_CHARACTERISTIC();
                        charMat = CNService.GetCharacteristicData(ZPKG_SEC_CATCHING_PERIOD.CHAR_NAME, ZPKG_SEC_CATCHING_PERIOD.CHAR_VALUE, context);

                        if (charMat != null)
                        {
                            processPA.CATCHING_PERIOD_ID = charMat.CHARACTERISTIC_ID;
                        }
                        else
                        {
                            processPA.CATCHING_PERIOD_ID = -1;
                            processPA.CATCHING_PERIOD_OTHER = ZPKG_SEC_CATCHING_PERIOD.CHAR_VALUE;
                        }
                    }
                    else
                    {
                        processPA.CATCHING_PERIOD_ID = null;
                        processPA.CATCHING_PERIOD_OTHER = null;
                    }

                    var ZPKG_SEC_CHANGE_POINT = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_CHANGE_POINT").FirstOrDefault();
                    if (ZPKG_SEC_CHANGE_POINT != null)
                    {
                        if (!String.IsNullOrEmpty(ZPKG_SEC_CHANGE_POINT.CHAR_VALUE))
                        {
                            processPA.CHANGE_POINT = ZPKG_SEC_CHANGE_POINT.CHAR_VALUE;
                        }
                    }
                    else
                    {
                        processPA.CHANGE_POINT = null;
                    }

                    var ZPKG_SEC_CATCHING_METHOD = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_CATCHING_METHOD").FirstOrDefault();
                    if (ZPKG_SEC_CATCHING_METHOD != null)
                    {
                        charMat = new SAP_M_CHARACTERISTIC();
                        charMat = CNService.GetCharacteristicData(ZPKG_SEC_CATCHING_METHOD.CHAR_NAME, ZPKG_SEC_CATCHING_METHOD.CHAR_VALUE, context);

                        if (charMat != null)
                        {
                            processPA.CATCHING_METHOD_ID = charMat.CHARACTERISTIC_ID;
                        }
                        else
                        {
                            processPA.CATCHING_METHOD_ID = -1;
                            processPA.CATCHING_METHOD_OTHER = ZPKG_SEC_CATCHING_METHOD.CHAR_VALUE;
                        }
                    }
                    else
                    {
                        processPA.CATCHING_METHOD_ID = null;
                        processPA.CATCHING_METHOD_ID = null;
                    }

                    var ZPKG_SEC_SCIENTIFIC_NAME = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_SCIENTIFIC_NAME").FirstOrDefault();
                    if (ZPKG_SEC_SCIENTIFIC_NAME != null)
                    {
                        charMat = new SAP_M_CHARACTERISTIC();
                        charMat = CNService.GetCharacteristicData(ZPKG_SEC_SCIENTIFIC_NAME.CHAR_NAME, ZPKG_SEC_SCIENTIFIC_NAME.CHAR_VALUE, context);

                        if (charMat != null)
                        {
                            processPA.SCIENTIFIC_NAME_ID = charMat.CHARACTERISTIC_ID;
                        }
                        else
                        {
                            processPA.SCIENTIFIC_NAME_ID = -1;
                            processPA.SCIENTIFIC_NAME_OTHER = ZPKG_SEC_SCIENTIFIC_NAME.CHAR_VALUE;
                        }
                    }
                    else
                    {
                        processPA.SCIENTIFIC_NAME_ID = null;
                        processPA.SCIENTIFIC_NAME_OTHER = null;
                    }

                    var ZPKG_SEC_DIRECTION = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_DIRECTION").FirstOrDefault();
                    if (ZPKG_SEC_DIRECTION != null)
                    {
                        charMat = new SAP_M_CHARACTERISTIC();
                        charMat = CNService.GetCharacteristicData(ZPKG_SEC_DIRECTION.CHAR_NAME, ZPKG_SEC_DIRECTION.CHAR_VALUE, context);

                        if (charMat != null)
                        {
                            processPA.DIRECTION_OF_STICKER_ID = charMat.CHARACTERISTIC_ID;
                        }
                        else
                        {
                            processPA.DIRECTION_OF_STICKER_ID = -1;
                            processPA.DIRECTION_OF_STICKER_OTHER = ZPKG_SEC_DIRECTION.CHAR_VALUE;
                        }
                    }
                    else
                    {
                        processPA.DIRECTION_OF_STICKER_ID = null;
                        processPA.DIRECTION_OF_STICKER_OTHER = null;
                    }

                    var ZPKG_SEC_SPECIE = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_SPECIE").FirstOrDefault();
                    if (ZPKG_SEC_SPECIE != null)
                    {
                        charMat = new SAP_M_CHARACTERISTIC();
                        charMat = CNService.GetCharacteristicData(ZPKG_SEC_SPECIE.CHAR_NAME, ZPKG_SEC_SPECIE.CHAR_VALUE, context);

                        if (charMat != null)
                        {
                            processPA.SPECIE_ID = charMat.CHARACTERISTIC_ID;
                        }
                        else
                        {
                            processPA.SPECIE_ID = -1;
                            processPA.SPECIE_OTHER = ZPKG_SEC_SPECIE.CHAR_VALUE;
                        }
                    }
                    else
                    {
                        processPA.SPECIE_ID = null;
                        processPA.SPECIE_OTHER = null;
                    }

                    if (matG != null)
                    {
                        string typeOf = "";// dicTypeOf[matG.VALUE];
                        if (dicTypeOf.TryGetValue(matG.CHAR_VALUE, out typeOf))
                        {
                            typeOf = dicTypeOf[matG.CHAR_VALUE];
                            var TYPE_OF = matConversion.Where(w => w.CHAR_NAME == typeOf).FirstOrDefault();
                            if (TYPE_OF != null)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(TYPE_OF.CHAR_NAME, TYPE_OF.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    processPA.TYPE_OF_ID = charMat.CHARACTERISTIC_ID;
                                }
                                else
                                {
                                    processPA.TYPE_OF_ID = -1;
                                    processPA.TYPE_OF_OTHER = TYPE_OF.CHAR_VALUE;
                                }
                            }
                            else
                            {
                                processPA.TYPE_OF_ID = null;
                                processPA.TYPE_OF_OTHER = null;
                            }
                        }

                        string typeOf2 = "";
                        if (dicTypeOf2.TryGetValue(matG.CHAR_VALUE, out typeOf2))
                        {
                            typeOf2 = dicTypeOf2[matG.CHAR_VALUE];
                            var TYPE_OF_2 = matConversion.Where(w => w.CHAR_NAME == typeOf2).FirstOrDefault();
                            if (TYPE_OF_2 != null)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(TYPE_OF_2.CHAR_NAME, TYPE_OF_2.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    processPA.TYPE_OF_2_ID = charMat.CHARACTERISTIC_ID;
                                }
                                else
                                {
                                    processPA.TYPE_OF_2_ID = -1;
                                    processPA.TYPE_OF_2_OTHER = TYPE_OF_2.CHAR_VALUE;
                                }
                            }
                            else
                            {
                                processPA.TYPE_OF_2_ID = null;
                                processPA.TYPE_OF_2_OTHER = null;
                            }
                        }

                        string styleOfPrinting = "";
                        if (dicStyleOfPrinting.TryGetValue(matG.CHAR_VALUE, out styleOfPrinting))
                        {
                            styleOfPrinting = dicStyleOfPrinting[matG.CHAR_VALUE];
                            var STYLE_OF_PRINTING = matConversion.Where(w => w.CHAR_NAME == styleOfPrinting).FirstOrDefault();
                            if (STYLE_OF_PRINTING != null)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(STYLE_OF_PRINTING.CHAR_NAME, STYLE_OF_PRINTING.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    processPA.STYLE_OF_PRINTING_ID = charMat.CHARACTERISTIC_ID;
                                }
                                else
                                {
                                    processPA.STYLE_OF_PRINTING_ID = -1;
                                    processPA.STYLE_OF_PRINTING_OTHER = STYLE_OF_PRINTING.CHAR_VALUE;
                                }
                            }
                            else
                            {
                                processPA.STYLE_OF_PRINTING_ID = null;
                                processPA.STYLE_OF_PRINTING_OTHER = null;
                            }
                        }

                        string pmsColour = "";
                        if (dicPMSColour.TryGetValue(matG.CHAR_VALUE, out pmsColour))
                        {
                            pmsColour = dicPMSColour[matG.CHAR_VALUE];
                            var PMS_COLOUR = matConversion.Where(w => w.CHAR_NAME == pmsColour).FirstOrDefault();
                            if (PMS_COLOUR != null)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(PMS_COLOUR.CHAR_NAME, PMS_COLOUR.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    processPA.PMS_COLOUR_ID = charMat.CHARACTERISTIC_ID;
                                }
                                else
                                {
                                    processPA.PMS_COLOUR_ID = -1;
                                    processPA.PMS_COLOUR_OTHER = PMS_COLOUR.CHAR_VALUE;
                                }
                            }
                            else
                            {
                                processPA.PMS_COLOUR_ID = null;
                                processPA.PMS_COLOUR_OTHER = null;
                            }
                        }

                        string processColour = "";
                        if (dicProcessColour.TryGetValue(matG.CHAR_VALUE, out processColour))
                        {
                            processColour = dicProcessColour[matG.CHAR_VALUE];
                            var PROCESS_COLOUR = matConversion.Where(w => w.CHAR_NAME == processColour).FirstOrDefault();
                            if (PROCESS_COLOUR != null)
                            {
                                charMat = new SAP_M_CHARACTERISTIC();
                                charMat = CNService.GetCharacteristicData(PROCESS_COLOUR.CHAR_NAME, PROCESS_COLOUR.CHAR_VALUE, context);

                                if (charMat != null)
                                {
                                    processPA.PROCESS_COLOUR_ID = charMat.CHARACTERISTIC_ID;
                                }
                                else
                                {
                                    processPA.PROCESS_COLOUR_ID = -1;
                                    processPA.PROCESS_COLOUR_OTHER = PROCESS_COLOUR.CHAR_VALUE;
                                }
                            }
                            else
                            {
                                processPA.PROCESS_COLOUR_ID = null;
                                processPA.PROCESS_COLOUR_OTHER = null;
                            }
                        }

                        string totalColour = "";
                        if (dicTotalColour.TryGetValue(matG.CHAR_VALUE, out totalColour))
                        {
                            totalColour = dicTotalColour[matG.CHAR_VALUE];
                            var TOTAL_COLOUR = matConversion.Where(w => w.CHAR_NAME == totalColour).FirstOrDefault();
                            if (TOTAL_COLOUR !=null)     // ticket#438889 when debug found TOTAL_COLOUR is null then TOTAL_COLOUR.CHAR_NAME.Equals(totalColour) is error.
                            {
                                if (TOTAL_COLOUR.CHAR_NAME.Equals(totalColour))
                                {
                                    charMat = new SAP_M_CHARACTERISTIC();
                                    charMat = CNService.GetCharacteristicData(TOTAL_COLOUR.CHAR_NAME, TOTAL_COLOUR.CHAR_VALUE, context);

                                    if (charMat != null)
                                    {
                                        processPA.TOTAL_COLOUR_ID = charMat.CHARACTERISTIC_ID;
                                    }
                                    else
                                    {
                                        processPA.TOTAL_COLOUR_ID = -1;
                                        processPA.TOTAL_COLOUR_OTHER = TOTAL_COLOUR.CHAR_VALUE;
                                    }
                                }
                                else
                                {
                                    processPA.TOTAL_COLOUR_ID = null;
                                    processPA.TOTAL_COLOUR_OTHER = null;
                                }
                            }
                            else
                            {
                                processPA.TOTAL_COLOUR_ID = null;
                                processPA.TOTAL_COLOUR_OTHER = null;
                            }
                            
                        }
                    }

                    processPA.MATERIAL_NO = matNO;
                    if (param.data.RETRIVE_TYPE == "SUGGEST")
                    {
                        processPA.MATERIAL_NO = "";
                        processPA.REQUEST_MATERIAL_STATUS = "";
                    }

                    var productionPlants = matConversion.Where(i => i.CHAR_NAME == "ZPKG_SEC_PRODUCTION_PLANT").ToList();
                    if (productionPlants != null)
                    {
                        string productionPlant = "";
                        foreach (var iProductPlant in productionPlants)
                        {
                            if (!String.IsNullOrEmpty(iProductPlant.CHAR_VALUE) && !productionPlant.Contains(iProductPlant.CHAR_VALUE))
                            {
                                if (String.IsNullOrEmpty(productionPlant))
                                {
                                    productionPlant = iProductPlant.CHAR_VALUE;
                                }
                                else
                                {
                                    productionPlant += "," + iProductPlant.CHAR_VALUE;
                                }
                            }
                        }

                        if (!String.IsNullOrEmpty(productionPlant))
                        {
                            processPA.PRODICUTION_PLANT_ID = -1;
                            processPA.PRODICUTION_PLANT_OTHER = productionPlant;
                        }
                        else
                        {
                            processPA.PRODICUTION_PLANT_ID = null;
                            processPA.PRODICUTION_PLANT_OTHER = null;
                        }
                    }

                    ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA, context);

                    #region "FAO_ZONE"
                    var matFAO = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_FAO").ToList();

                    if (matFAO != null && matFAO.Count > 0)
                    {
                        var fao = context.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE
                                    .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                    .ToList();
                        if (fao != null && fao.Count > 0)
                        {
                            foreach (ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE iFAO in fao)
                            {
                                ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.DeleteByARTWORK_SUB_PA_FAO_ID(iFAO.ARTWORK_SUB_PA_FAO_ID, context);
                            }
                        }


                        ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE fao_new = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                        foreach (var iFAONew in matFAO)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(iFAONew.CHAR_NAME, iFAONew.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                fao_new = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                                fao_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                fao_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                fao_new.FAO_ZONE_ID = charMat.CHARACTERISTIC_ID;
                                fao_new.CREATE_BY = currentUserId;
                                fao_new.UPDATE_BY = currentUserId;
                                ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.SaveOrUpdateNoLog(fao_new, context);
                            }
                        }

                    }
                    #endregion

                    #region "CATCHING_AREA"

                    var matCatchingArea = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_CATCHING_AREA").ToList();

                    if (matCatchingArea != null && matCatchingArea.Count > 0)
                    {
                        var catchingArea = context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA
                              .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                              .ToList();
                        if (catchingArea != null && catchingArea.Count > 0)
                        {
                            foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA iChatching in catchingArea)
                            {
                                ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE.DeleteByARTWORK_SUB_PA_CATCHING_AREA_ID(iChatching.ARTWORK_SUB_PA_CATCHING_AREA_ID, context);
                            }
                        }

                        ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA catching_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                        foreach (var iCatchingNew in matCatchingArea)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(iCatchingNew.CHAR_NAME, iCatchingNew.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                catching_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                                catching_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                catching_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                catching_new.CATCHING_AREA_ID = charMat.CHARACTERISTIC_ID;
                                catching_new.CREATE_BY = currentUserId;
                                catching_new.UPDATE_BY = currentUserId;
                                ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE.SaveOrUpdateNoLog(catching_new, context);
                            }
                        }
                    }

                    #endregion

                    #region "CATCHING_METHOD"
                    // ticke#425737 added by aof 
                    var matCatchingMethod = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_CATCHING_METHOD").ToList();

                    if (matCatchingMethod != null && matCatchingMethod.Count > 0)
                    {
                        var catchingMethod = context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD
                              .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                              .ToList();
                        if (catchingMethod != null && catchingMethod.Count > 0)
                        {
                            foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD iMethod in catchingMethod)
                            {
                                ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE.DeleteByARTWORK_SUB_PA_CATCHING_METHOD_ID(iMethod.ARTWORK_SUB_PA_CATCHING_METHOD_ID, context);
                            }
                        }

                        ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD method_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                        foreach (var iMathodNew in matCatchingMethod)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(iMathodNew.CHAR_NAME, iMathodNew.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                method_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                                method_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                method_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                method_new.CATCHING_METHOD_ID = charMat.CHARACTERISTIC_ID;
                                method_new.CREATE_BY = currentUserId;
                                method_new.UPDATE_BY = currentUserId;
                                ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE.SaveOrUpdateNoLog(method_new, context);
                            }
                        }
                    }
                    // ticke#425737 added by aof 
                    #endregion

                    #region "SYMBOL"

                    var matSymbol = matConversion.Where(w => w.CHAR_NAME == "ZPKG_SEC_SYMBOL").ToList();

                    if (matSymbol != null && matSymbol.Count > 0)
                    {
                        var symbol = context.ART_WF_ARTWORK_PROCESS_PA_SYMBOL
                              .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                              .ToList();
                        if (symbol != null && symbol.Count > 0)
                        {
                            foreach (ART_WF_ARTWORK_PROCESS_PA_SYMBOL iChatching in symbol)
                            {
                                ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE.DeleteByARTWORK_SUB_PA_SYMBOL_ID(iChatching.ARTWORK_SUB_PA_SYMBOL_ID, context);
                            }
                        }

                        ART_WF_ARTWORK_PROCESS_PA_SYMBOL symbol_new = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                        foreach (var iSymbolNew in matSymbol)
                        {
                            charMat = new SAP_M_CHARACTERISTIC();
                            charMat = CNService.GetCharacteristicData(iSymbolNew.CHAR_NAME, iSymbolNew.CHAR_VALUE, context);

                            if (charMat != null)
                            {
                                symbol_new = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                                symbol_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                symbol_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                symbol_new.SYMBOL_ID = charMat.CHARACTERISTIC_ID;
                                symbol_new.CREATE_BY = currentUserId;
                                symbol_new.UPDATE_BY = currentUserId;
                                ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE.SaveOrUpdate(symbol_new, context);
                            }
                        }
                    }
                    #endregion

                    #region "PRODUCT"
                    var product = context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT
                                .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                .ToList();

                    var productOther = context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER
                              .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                              .ToList();

                    if (product != null && product.Count > 0)
                    {
                        foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT iProduct in product)
                        {
                            ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.DeleteByARTWORK_SUB_PA_PRODUCT_ID(iProduct.ARTWORK_SUB_PA_PRODUCT_ID, context);
                        }

                        foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER iProduct in productOther)
                        {
                            ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.DeleteByARTWORK_SUB_PA_PRODUCT_OTHER_ID(iProduct.ARTWORK_SUB_PA_PRODUCT_OTHER_ID, context);
                        }

                    }

                    var listProduct = matConversion.Where(i => i.CHAR_NAME == "ZPKG_SEC_PRODUCT_CODE").ToList();

                    if (listProduct != null)
                    {
                        var listProductCode = listProduct.Select(s => s.CHAR_VALUE).ToList();

                        if (listProductCode != null)
                        {
                            var xProduct = (from p in context.XECM_M_PRODUCT
                                            where listProductCode.Contains(p.PRODUCT_CODE)
                                            select p).ToList();

                            ART_WF_ARTWORK_PROCESS_PA_PRODUCT productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                            foreach (SAP_M_MATERIAL_CONVERSION iProduct in listProduct)
                            {
                                int productID = xProduct.Where(w => w.PRODUCT_CODE == iProduct.CHAR_VALUE)
                                                        .Select(s => s.XECM_PRODUCT_ID).FirstOrDefault();

                                if (productID > 0)
                                {
                                    productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                                    productNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                    productNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                    productNew.PRODUCT_CODE_ID = productID;
                                    productNew.CREATE_BY = currentUserId;
                                    productNew.UPDATE_BY = currentUserId;
                                    ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.SaveOrUpdateNoLog(productNew, context);
                                }
                                else
                                {
                                    ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER productOtherNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();

                                    productOtherNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();
                                    productOtherNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                    productOtherNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                    productOtherNew.PRODUCT_CODE = iProduct.CHAR_VALUE;
                                    productOtherNew.CREATE_BY = currentUserId;
                                    productOtherNew.UPDATE_BY = currentUserId;
                                    ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.SaveOrUpdateNoLog(productOtherNew, context);
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
        }

        public static ART_WF_ARTWORK_PROCESS_PA_RESULT RetriveMaterial_Repeat(ART_WF_ARTWORK_PROCESS_PA_REQUEST param, ARTWORKEntities context)
        {
            var currentUserId = CNService.getCurrentUser(context);

            ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();
            ART_WF_ARTWORK_PROCESS_PA processPA = new ART_WF_ARTWORK_PROCESS_PA();
            ART_WF_ARTWORK_PROCESS_PA processPAByMaterial = new ART_WF_ARTWORK_PROCESS_PA();

            try
            {
                if (param == null && param.data == null)
                {
                    return Results;
                }
                if (!String.IsNullOrEmpty(param.data.MATERIAL_NO))
                {

                    processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                 where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                 select p).OrderByDescending(o => o.UPDATE_BY).FirstOrDefault();

                    var stepPA = (from p in context.ART_M_STEP_ARTWORK
                                  where p.STEP_ARTWORK_CODE == "SEND_PA"
                                  select p.STEP_ARTWORK_ID).FirstOrDefault();

                    var processPAByMaterial_ALL = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                   where p.MATERIAL_NO == param.data.MATERIAL_NO
                                                    && p.ARTWORK_SUB_ID != param.data.ARTWORK_SUB_ID
                                                   select p.ARTWORK_SUB_ID).ToList();

                    if (processPAByMaterial_ALL != null)
                    {
                        var processEnd = (from p in context.ART_WF_ARTWORK_PROCESS
                                          where processPAByMaterial_ALL.Contains(p.ARTWORK_SUB_ID)
                                          && p.IS_END == "X"
                                          && String.IsNullOrEmpty(p.IS_TERMINATE)
                                          select p).OrderByDescending(o => o.ARTWORK_SUB_ID).FirstOrDefault();

                        if (processEnd != null)
                        {
                            processPAByMaterial = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                   where p.ARTWORK_SUB_ID == processEnd.ARTWORK_SUB_ID
                                                   select p).OrderByDescending(o => o.ARTWORK_SUB_ID).FirstOrDefault();

                            if (processPAByMaterial != null)
                            {
                                processPA.CHANGE_POINT = processPAByMaterial.CHANGE_POINT;
                                processPA.PA_USER_ID = processPAByMaterial.PA_USER_ID;
                                processPA.THREE_P_ID = processPAByMaterial.THREE_P_ID;
                                processPA.TWO_P_ID = processPAByMaterial.TWO_P_ID;
                                processPA.MATERIAL_GROUP_ID = processPAByMaterial.MATERIAL_GROUP_ID;
                                processPA.PRODUCT_CODE_ID = processPAByMaterial.PRODUCT_CODE_ID;
                                // processPA.PA_USER_ID = CNService.ConvertStringToInt(_header.PA_USER_NAME);
                                processPA.REFERENCE_MATERIAL = processPAByMaterial.REFERENCE_MATERIAL;
                                // processPA.PRINTING_STYLE_OF_PRIMARY_ID
                                // processPA.PRINTING_STYLE_OF_SECONDARY_ID
                                processPA.CUSTOMER_DESIGN = processPAByMaterial.CUSTOMER_DESIGN;
                                processPA.CUSTOMER_DESIGN_OTHER = processPAByMaterial.CUSTOMER_DESIGN_OTHER;
                                processPA.CUSTOMER_SPEC = processPAByMaterial.CUSTOMER_SPEC;
                                processPA.CUSTOMER_SPEC_OTHER = processPAByMaterial.CUSTOMER_SPEC_OTHER;
                                processPA.CUSTOMER_SIZE = processPAByMaterial.CUSTOMER_SIZE;
                                processPA.CUSTOMER_SIZE_OTHER = processPAByMaterial.CUSTOMER_SIZE_OTHER;
                                processPA.CUSTOMER_NOMINATES_VENDOR = processPAByMaterial.CUSTOMER_NOMINATES_VENDOR;
                                processPA.CUSTOMER_NOMINATES_VENDOR_OTHER = processPAByMaterial.CUSTOMER_NOMINATES_VENDOR_OTHER;
                                processPA.CUSTOMER_NOMINATES_COLOR = processPAByMaterial.CUSTOMER_NOMINATES_COLOR;
                                processPA.CUSTOMER_NOMINATES_COLOR_OTHER = processPAByMaterial.CUSTOMER_NOMINATES_COLOR_OTHER;
                                processPA.CUSTOMER_BARCODE_SCANABLE = processPAByMaterial.CUSTOMER_BARCODE_SCANABLE;
                                processPA.CUSTOMER_BARCODE_SCANABLE_OTHER = processPAByMaterial.CUSTOMER_BARCODE_SCANABLE_OTHER;
                                processPA.CUSTOMER_BARCODE_SPEC = processPAByMaterial.CUSTOMER_BARCODE_SPEC;
                                processPA.CUSTOMER_BARCODE_SPEC_OTHER = processPAByMaterial.CUSTOMER_BARCODE_SPEC_OTHER;
                                processPA.FIRST_INFOGROUP_OTHER = processPAByMaterial.FIRST_INFOGROUP_OTHER;
                                processPA.NOTE_OF_PA = processPAByMaterial.NOTE_OF_PA;
                                processPA.FIRST_INFOGROUP_OTHER = processPAByMaterial.FIRST_INFOGROUP_OTHER;
                                processPA.COMPLETE_INFOGROUP = processPAByMaterial.COMPLETE_INFOGROUP;
                                processPA.PRODUCTION_EXPIRY_DATE_SYSTEM = processPAByMaterial.PRODUCTION_EXPIRY_DATE_SYSTEM;
                                processPA.SERIOUSNESS_OF_COLOR_PRINTING = processPAByMaterial.SERIOUSNESS_OF_COLOR_PRINTING;
                                processPA.NUTRITION_ANALYSIS = processPAByMaterial.NUTRITION_ANALYSIS;
                                processPA.PACKAGE_QUANTITY = processPAByMaterial.PACKAGE_QUANTITY;
                                processPA.WASTE_PERCENT = processPAByMaterial.WASTE_PERCENT;
                                // processPA.REQUEST_MATERIAL_STATUS = processPAByMaterial.STATUS;
                                processPA.TYPE_OF_ID = processPAByMaterial.TYPE_OF_ID;
                                processPA.TYPE_OF_OTHER = processPAByMaterial.TYPE_OF_OTHER;
                                processPA.TYPE_OF_2_ID = processPAByMaterial.TYPE_OF_2_ID;
                                processPA.TYPE_OF_2_OTHER = processPAByMaterial.TYPE_OF_2_OTHER;
                                processPA.PMS_COLOUR_ID = processPAByMaterial.PMS_COLOUR_ID;
                                processPA.PMS_COLOUR_OTHER = processPAByMaterial.PMS_COLOUR_OTHER;
                                processPA.PROCESS_COLOUR_ID = processPAByMaterial.PROCESS_COLOUR_ID;
                                processPA.PROCESS_COLOUR_OTHER = processPAByMaterial.PROCESS_COLOUR_OTHER;
                                processPA.TOTAL_COLOUR_ID = processPAByMaterial.TOTAL_COLOUR_ID;
                                processPA.TOTAL_COLOUR_OTHER = processPAByMaterial.TOTAL_COLOUR_OTHER;
                                processPA.STYLE_OF_PRINTING_ID = processPAByMaterial.STYLE_OF_PRINTING_ID;
                                processPA.STYLE_OF_PRINTING_OTHER = processPAByMaterial.STYLE_OF_PRINTING_OTHER;
                                processPA.PRIMARY_SIZE_ID = processPAByMaterial.PRIMARY_SIZE_ID;
                                processPA.PRIMARY_SIZE_OTHER = processPAByMaterial.PRIMARY_SIZE_OTHER;
                                processPA.CONTAINER_TYPE_ID = processPAByMaterial.CONTAINER_TYPE_ID;
                                processPA.CONTAINER_TYPE_OTHER = processPAByMaterial.CONTAINER_TYPE_OTHER;
                                processPA.LID_TYPE_ID = processPAByMaterial.LID_TYPE_ID;
                                processPA.LID_TYPE_OTHER = processPAByMaterial.LID_TYPE_OTHER;
                                processPA.PLANT_REGISTERED_ID = processPAByMaterial.PLANT_REGISTERED_ID;
                                processPA.PLANT_REGISTERED_OTHER = processPAByMaterial.PLANT_REGISTERED_OTHER;
                                processPA.COMPANY_ADDRESS_ID = processPAByMaterial.COMPANY_ADDRESS_ID;
                                processPA.COMPANY_ADDRESS_OTHER = processPAByMaterial.COMPANY_ADDRESS_OTHER;
                                processPA.PRODICUTION_PLANT_ID = processPAByMaterial.PRODICUTION_PLANT_ID;  //------by aof 
                                processPA.PRODICUTION_PLANT_OTHER = processPAByMaterial.PRODICUTION_PLANT_OTHER;    //------by aof 
                                processPA.CATCHING_PERIOD_ID = processPAByMaterial.CATCHING_PERIOD_ID;
                                processPA.CATCHING_PERIOD_OTHER = processPAByMaterial.CATCHING_PERIOD_OTHER;
                                processPA.CATCHING_METHOD_ID = processPAByMaterial.CATCHING_METHOD_ID;
                                processPA.CATCHING_METHOD_OTHER = processPAByMaterial.CATCHING_METHOD_OTHER;
                                processPA.SCIENTIFIC_NAME_ID = processPAByMaterial.SCIENTIFIC_NAME_ID;
                                processPA.SCIENTIFIC_NAME_OTHER = processPAByMaterial.SCIENTIFIC_NAME_OTHER;
                                processPA.DIRECTION_OF_STICKER_ID = processPAByMaterial.DIRECTION_OF_STICKER_ID;
                                processPA.DIRECTION_OF_STICKER_OTHER = processPAByMaterial.DIRECTION_OF_STICKER_OTHER;
                                processPA.SPECIE_ID = processPAByMaterial.SPECIE_ID;
                                processPA.SPECIE_OTHER = processPAByMaterial.SPECIE_OTHER;
                                processPA.PRINTING_STYLE_OF_PRIMARY_ID = processPAByMaterial.PRINTING_STYLE_OF_PRIMARY_ID;
                                processPA.PRINTING_STYLE_OF_PRIMARY_OTHER = processPAByMaterial.PRINTING_STYLE_OF_PRIMARY_OTHER;
                                processPA.PRINTING_STYLE_OF_SECONDARY_ID = processPAByMaterial.PRINTING_STYLE_OF_SECONDARY_ID;
                                processPA.PRINTING_STYLE_OF_SECONDARY_OTHER = processPAByMaterial.PRINTING_STYLE_OF_SECONDARY_OTHER;
                                processPA.PIC_MKT = processPAByMaterial.PIC_MKT;
                                processPA.COURIER_NO = processPAByMaterial.COURIER_NO;
                                processPA.STYLE_OF_PRINTING_ID = processPAByMaterial.STYLE_OF_PRINTING_ID;
                                processPA.STYLE_OF_PRINTING_OTHER = processPAByMaterial.STYLE_OF_PRINTING_OTHER;
                                processPA.DIRECTION_OF_STICKER_ID = processPAByMaterial.DIRECTION_OF_STICKER_ID;
                                processPA.DIRECTION_OF_STICKER_OTHER = processPAByMaterial.DIRECTION_OF_STICKER_OTHER;

                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);

                                #region "PLANT"
                                var plant = context.ART_WF_ARTWORK_PROCESS_PA_PLANT
                                            .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                            .ToList();
                                if (plant != null && plant.Count > 0)
                                {
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_PLANT iPlant in plant)
                                    {
                                        ART_WF_ARTWORK_PROCESS_PA_PLANT_SERVICE.DeleteByARTWORK_SUB_PA_PLANT_ID(iPlant.ARTWORK_SUB_PA_PLANT_ID, context);
                                    }
                                }

                                var listPlants = (from p in context.ART_WF_ARTWORK_PROCESS_PA_PLANT
                                                  where p.ARTWORK_SUB_PA_ID == processPAByMaterial.ARTWORK_SUB_PA_ID
                                                  select p).ToList();

                                if (listPlants.Count > 0)
                                {
                                    ART_WF_ARTWORK_PROCESS_PA_PLANT plantNew = new ART_WF_ARTWORK_PROCESS_PA_PLANT();
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_PLANT iPlant in listPlants)
                                    {
                                        plantNew = new ART_WF_ARTWORK_PROCESS_PA_PLANT();
                                        plantNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        plantNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        plantNew.PLANT_ID = iPlant.PLANT_ID;
                                        plantNew.PLANT_OTHER = iPlant.PLANT_OTHER;
                                        plantNew.CREATE_BY = currentUserId;
                                        plantNew.UPDATE_BY = currentUserId;
                                        ART_WF_ARTWORK_PROCESS_PA_PLANT_SERVICE.SaveOrUpdateNoLog(plantNew, context);
                                    }
                                }
                                #endregion

                                #region "FAO_ZONE"
                                var fao = context.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE
                                            .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                            .ToList();
                                if (fao != null && fao.Count > 0)
                                {
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE iFAO in fao)
                                    {
                                        ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.DeleteByARTWORK_SUB_PA_FAO_ID(iFAO.ARTWORK_SUB_PA_FAO_ID, context);
                                    }
                                }

                                var faoNews = (from f in context.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE
                                               where f.ARTWORK_SUB_PA_ID == processPAByMaterial.ARTWORK_SUB_PA_ID
                                               select f).ToList();

                                if (faoNews != null && faoNews.Count > 0)
                                {
                                    ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE fao_new = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE iFAONew in faoNews)
                                    {
                                        fao_new = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                                        fao_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        fao_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        fao_new.FAO_ZONE_ID = iFAONew.FAO_ZONE_ID;
                                        fao_new.FAO_ZONE_OTHER = iFAONew.FAO_ZONE_OTHER;
                                        fao_new.CREATE_BY = currentUserId;
                                        fao_new.UPDATE_BY = currentUserId;
                                        ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.SaveOrUpdateNoLog(fao_new, context);
                                    }
                                }

                                #endregion

                                #region "CATCHING_AREA"
                                var catchingArea = context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA
                                          .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                          .ToList();
                                if (catchingArea != null && catchingArea.Count > 0)
                                {
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA iChatching in catchingArea)
                                    {
                                        ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE.DeleteByARTWORK_SUB_PA_CATCHING_AREA_ID(iChatching.ARTWORK_SUB_PA_CATCHING_AREA_ID, context);
                                    }
                                }

                                var catchingNews = (from c in context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA
                                                    where c.ARTWORK_SUB_PA_ID == processPAByMaterial.ARTWORK_SUB_PA_ID
                                                    select c).ToList();

                                if (catchingNews != null && catchingNews.Count > 0)
                                {
                                    ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA catching_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA iCatchingNew in catchingNews)
                                    {
                                        catching_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                                        catching_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        catching_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        catching_new.CATCHING_AREA_ID = iCatchingNew.CATCHING_AREA_ID;
                                        catching_new.CATCHING_AREA_OTHER = iCatchingNew.CATCHING_AREA_OTHER;
                                        catching_new.CREATE_BY = currentUserId;
                                        catching_new.UPDATE_BY = currentUserId;
                                        ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE.SaveOrUpdateNoLog(catching_new, context);
                                    }
                                }

                                #endregion

                                #region "CATCHING_METHOD"
                                // ticke#425737 added by aof 
                                var catchingMethod = context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD
                                          .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                          .ToList();
                                if (catchingMethod != null && catchingMethod.Count > 0)
                                {
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD iMethod in catchingMethod)
                                    {
                                        ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE.DeleteByARTWORK_SUB_PA_CATCHING_METHOD_ID(iMethod.ARTWORK_SUB_PA_CATCHING_METHOD_ID, context);
                                    }
                                }

                                var methodNews = (from c in context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD
                                                  where c.ARTWORK_SUB_PA_ID == processPAByMaterial.ARTWORK_SUB_PA_ID
                                                    select c).ToList();

                                if (methodNews != null && methodNews.Count > 0)
                                {
                                    ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD method_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD iMethodNew in methodNews)
                                    {
                                        method_new = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                                        method_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        method_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        method_new.CATCHING_METHOD_ID = iMethodNew.CATCHING_METHOD_ID;
                                        method_new.CATCHING_METHOD_OTHER = iMethodNew.CATCHING_METHOD_OTHER;
                                        method_new.CREATE_BY = currentUserId;
                                        method_new.UPDATE_BY = currentUserId;
                                        ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE.SaveOrUpdateNoLog(method_new, context);
                                    }
                                }
                                // ticke#425737 added by aof 
                                #endregion

                                #region "SYMBOL"
                                var symbol = context.ART_WF_ARTWORK_PROCESS_PA_SYMBOL
                                          .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                          .ToList();
                                if (symbol != null && symbol.Count > 0)
                                {
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_SYMBOL iChatching in symbol)
                                    {
                                        ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE.DeleteByARTWORK_SUB_PA_SYMBOL_ID(iChatching.ARTWORK_SUB_PA_SYMBOL_ID, context);
                                    }
                                }

                                var symbolNews = (from s in context.ART_WF_ARTWORK_PROCESS_PA_SYMBOL
                                                  where s.ARTWORK_SUB_PA_ID == processPAByMaterial.ARTWORK_SUB_PA_ID
                                                  select s).ToList();

                                if (symbolNews != null && symbolNews.Count > 0)
                                {
                                    ART_WF_ARTWORK_PROCESS_PA_SYMBOL symbol_new = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                                    foreach (var iSymbolNew in symbolNews)
                                    {
                                        symbol_new = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                                        symbol_new.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        symbol_new.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        symbol_new.SYMBOL_ID = iSymbolNew.SYMBOL_ID;
                                        symbol_new.SYMBOL_OTHER = iSymbolNew.SYMBOL_OTHER;
                                        symbol_new.CREATE_BY = currentUserId;
                                        symbol_new.UPDATE_BY = currentUserId;
                                        ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE.SaveOrUpdateNoLog(symbol_new, context);
                                    }

                                }

                                #endregion

                                #region "PRODUCT"
                                var product = context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT
                                            .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                            .ToList();
                                if (product != null && product.Count > 0)
                                {
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT iProduct in product)
                                    {
                                        ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.DeleteByARTWORK_SUB_PA_PRODUCT_ID(iProduct.ARTWORK_SUB_PA_PRODUCT_ID, context);
                                    }
                                }

                                var listProduct = (from p in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT
                                                   where p.ARTWORK_SUB_PA_ID == processPAByMaterial.ARTWORK_SUB_PA_ID
                                                   select p).ToList();

                                if (listProduct.Count > 0)
                                {
                                    ART_WF_ARTWORK_PROCESS_PA_PRODUCT productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT iProduct in listProduct)
                                    {
                                        productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                                        productNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        productNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        productNew.PRODUCT_CODE_ID = iProduct.PRODUCT_CODE_ID;
                                        productNew.CREATE_BY = param.data.UPDATE_BY;
                                        productNew.UPDATE_BY = param.data.UPDATE_BY;
                                        productNew.CREATE_BY = currentUserId;
                                        productNew.UPDATE_BY = currentUserId;
                                        ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.SaveOrUpdateNoLog(productNew, context);
                                    }
                                }
                                #endregion

                                #region "PRODUCT OTHER"
                                var productOther = context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER
                                            .Where(f => f.ARTWORK_SUB_PA_ID == processPA.ARTWORK_SUB_PA_ID)
                                            .ToList();
                                if (productOther != null && productOther.Count > 0)
                                {
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER iProduct in productOther)
                                    {
                                        ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.DeleteByARTWORK_SUB_PA_PRODUCT_OTHER_ID(iProduct.ARTWORK_SUB_PA_PRODUCT_OTHER_ID, context);
                                    }
                                }

                                var listProductOther = (from p in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER
                                                        where p.ARTWORK_SUB_PA_ID == processPAByMaterial.ARTWORK_SUB_PA_ID
                                                        select p).ToList();

                                if (listProductOther.Count > 0)
                                {
                                    ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();
                                    foreach (ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER iProduct in listProductOther)
                                    {
                                        productNew = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();
                                        productNew.ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                        productNew.ARTWORK_SUB_PA_ID = processPA.ARTWORK_SUB_PA_ID;
                                        productNew.PRODUCT_CODE = iProduct.PRODUCT_CODE;
                                        productNew.NET_WEIGHT = iProduct.NET_WEIGHT;
                                        productNew.DRAINED_WEIGHT = iProduct.DRAINED_WEIGHT;
                                        productNew.CREATE_BY = currentUserId;
                                        productNew.UPDATE_BY = currentUserId;
                                        ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.SaveOrUpdate(productNew, context);
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                param.data.RETRIVE_TYPE = "RETRIVE";
                                RetriveMaterial(param, context);
                            }
                        }
                        else
                        {
                            param.data.RETRIVE_TYPE = "RETRIVE";
                            RetriveMaterial(param, context);
                        }
                    }
                    else
                    {
                        param.data.RETRIVE_TYPE = "RETRIVE";
                        RetriveMaterial(param, context);
                    }

                    Results.status = "S";
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        private static MM65_RESULT UpdateMaterialToIGrid(MM65_REQUEST param)
        {
            MM65_RESULT Results = new MM65_RESULT();

            Results = MM_65_Hepler.RequestMaterial(param);

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_PA_RESULT RepeatSOUpdatePAData(ART_WF_ARTWORK_PROCESS_PA_REQUEST param, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();
            try
            {
                if (param != null && param.data != null)
                {
                    var process = (from p in context.ART_WF_ARTWORK_PROCESS
                                   where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                   select p).FirstOrDefault();

                    var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                     where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                     select p).FirstOrDefault();

                    var reqItem = (from p in context.ART_WF_ARTWORK_REQUEST_ITEM
                                   where p.ARTWORK_ITEM_ID == process.ARTWORK_ITEM_ID
                                   select p).FirstOrDefault();

                    if (process != null)
                    {
                        if (reqItem != null)
                        {
                            processPA.REQUEST_MATERIAL_STATUS = "Completed";
                            processPA.MATERIAL_NO = reqItem.REPEAT_SO_MATERIAL_NO;

                            ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA, context);
                        }

                        var reqSORepeats = (from r in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                            join h in context.SAP_M_PO_COMPLETE_SO_HEADER on r.SALES_ORDER_NO equals h.SALES_ORDER_NO
                                            where r.ARTWORK_REQUEST_ID == process.ARTWORK_REQUEST_ID
                                            select new { r.PRODUCT_CODE, r.SALES_ORDER_NO, r.SALES_ORDER_ITEM, r.COMPONENT_MATERIAL, r.COMPONENT_ITEM, h.PO_COMPLETE_SO_HEADER_ID }).ToList();

                        if (reqSORepeats != null && reqSORepeats.Count > 0)
                        {
                            string productCode = "";
                            ART_WF_ARTWORK_PROCESS_SO_DETAIL soDetail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();

                            var listPOCompleteHeaderId = reqSORepeats.Select(h => h.PO_COMPLETE_SO_HEADER_ID).ToList().Distinct();
                            var listComponentMatherial = reqSORepeats.Select(h => h.COMPONENT_MATERIAL).ToList().Distinct();
                            var listComponentItem = reqSORepeats.Select(h => h.COMPONENT_ITEM).ToList().Distinct();
                            var listSaleOrderNo = reqSORepeats.Select(h => h.SALES_ORDER_NO).ToList().Distinct();
                            var listSaleOrderItem = reqSORepeats.Select(h => h.SALES_ORDER_ITEM).ToList().Distinct();

                            //Item
                            var listItem = (from h in context.SAP_M_PO_COMPLETE_SO_ITEM
                                            where listPOCompleteHeaderId.Contains(h.PO_COMPLETE_SO_HEADER_ID)
                                            select new { h.ITEM, h.PO_COMPLETE_SO_HEADER_ID, h.PO_COMPLETE_SO_ITEM_ID }).ToList();

                            //Bom
                            var listBom = (from b in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                                           where listComponentMatherial.Contains(b.COMPONENT_MATERIAL) && listComponentItem.Contains(b.COMPONENT_ITEM)
                                           select new { b.PO_COMPLETE_SO_ITEM_ID, b.COMPONENT_MATERIAL, b.COMPONENT_ITEM, b.PO_COMPLETE_SO_ITEM_COMPONENT_ID }).ToList();

                            //PO complete header
                            var listSoHdr = (from s in context.SAP_M_PO_COMPLETE_SO_HEADER
                                             where listSaleOrderNo.Contains(s.SALES_ORDER_NO)
                                             select new { s.SALES_ORDER_NO }).ToList();

                            //Sales order
                            var listFOC = (from f in context.V_SAP_SALES_ORDER
                                           where listSaleOrderNo.Contains(f.SALES_ORDER_NO)
                                                && listSaleOrderItem.Contains(f.ITEM_CUSTOM_1)
                                                && listComponentMatherial.Contains(f.PRODUCT_CODE)
                                                && f.SO_ITEM_IS_ACTIVE == "X"
                                           select new { f.SALES_ORDER_NO, f.ITEM_CUSTOM_1, f.PRODUCT_CODE, f.ITEM }).ToList();

                            int currentUser = CNService.getCurrentUser(context);
                            foreach (var iSORepeat in reqSORepeats)
                            {
                                productCode = iSORepeat.PRODUCT_CODE;

                                soDetail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
                                soDetail.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                                soDetail.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                                soDetail.SALES_ORDER_NO = iSORepeat.SALES_ORDER_NO;
                                soDetail.SALES_ORDER_ITEM = iSORepeat.SALES_ORDER_ITEM;
                                soDetail.MATERIAL_NO = iSORepeat.PRODUCT_CODE;

                                if (!String.IsNullOrEmpty(productCode))
                                {
                                    soDetail.BOM_NO = null;
                                    if (iSORepeat.PO_COMPLETE_SO_HEADER_ID > 0)
                                    {
                                        decimal itemNO = 0;
                                        itemNO = Convert.ToDecimal(iSORepeat.SALES_ORDER_ITEM);
                                        var item = listItem.Where(i => i.ITEM == itemNO && i.PO_COMPLETE_SO_HEADER_ID == iSORepeat.PO_COMPLETE_SO_HEADER_ID).FirstOrDefault();
                                        if (item != null)
                                        {
                                            if (productCode.StartsWith("3"))
                                            {
                                                var bom = listBom.Where(b => b.PO_COMPLETE_SO_ITEM_ID == item.PO_COMPLETE_SO_ITEM_ID && b.COMPONENT_MATERIAL == iSORepeat.COMPONENT_MATERIAL && b.COMPONENT_ITEM == iSORepeat.COMPONENT_ITEM).FirstOrDefault();
                                                if (bom != null)
                                                {
                                                    soDetail.BOM_ID = bom.PO_COMPLETE_SO_ITEM_COMPONENT_ID;
                                                }
                                            }
                                        }
                                    }
                                }

                                soDetail.CREATE_BY = param.data.UPDATE_BY;
                                soDetail.UPDATE_BY = param.data.UPDATE_BY;

                                if (soDetail.CREATE_BY == 0) soDetail.CREATE_BY = currentUser;
                                if (soDetail.UPDATE_BY == 0) soDetail.UPDATE_BY = currentUser;
                                ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.SaveOrUpdateNoLog(soDetail, context);

                                #region "Save FOC Item"
                                SAP_M_PO_COMPLETE_SO_HEADER _detailHeaderFOC = new SAP_M_PO_COMPLETE_SO_HEADER();
                                SAP_M_PO_COMPLETE_SO_ITEM _detailItemFOC = new SAP_M_PO_COMPLETE_SO_ITEM();

                                var soHdr = listSoHdr.Where(s => s.SALES_ORDER_NO == iSORepeat.SALES_ORDER_NO).FirstOrDefault();
                                if (soHdr != null)
                                {
                                    var saleOrder = listFOC.Where(f => f.SALES_ORDER_NO == iSORepeat.SALES_ORDER_NO && f.ITEM_CUSTOM_1 == iSORepeat.SALES_ORDER_ITEM && f.PRODUCT_CODE == iSORepeat.COMPONENT_MATERIAL).ToList();

                                    foreach (var item in saleOrder)
                                    {
                                        ART_WF_ARTWORK_PROCESS_SO_DETAIL detail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();

                                        detail.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                                        detail.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                                        detail.SALES_ORDER_NO = iSORepeat.SALES_ORDER_NO;
                                        detail.SALES_ORDER_ITEM = item.ITEM.ToString();
                                        detail.MATERIAL_NO = item.PRODUCT_CODE;
                                        detail.BOM_NO = "FOC";
                                        detail.CREATE_BY = param.data.CREATE_BY;
                                        detail.UPDATE_BY = param.data.UPDATE_BY;

                                        if (detail.CREATE_BY == 0) detail.CREATE_BY = currentUser;
                                        if (detail.UPDATE_BY == 0) detail.UPDATE_BY = currentUser;

                                        //if (CNService.IsDevOrQAS())
                                        //{
                                        var cntFOC = (from m in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                                      where m.SALES_ORDER_NO == detail.SALES_ORDER_NO
                                                      && m.SALES_ORDER_ITEM == detail.SALES_ORDER_ITEM
                                                      select m).Count();
                                        if (cntFOC == 0)
                                            ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.SaveOrUpdateNoLog(detail, context);
                                        //}
                                        //else
                                        //{
                                        //    ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.SaveOrUpdateNoLog(detail, context);
                                        //}
                                    }
                                }

                                #endregion
                            }

                            ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 soDetail_2 = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
                            soDetail_2 = MapperServices.ART_WF_ARTWORK_PROCESS_SO_DETAIL(soDetail);
                            SalesOrderHelper.CopyAssignSalesOrder(soDetail_2, context);
                        }
                    }

                    var processPATmp = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                        where p.ARTWORK_SUB_ID != param.data.ARTWORK_SUB_ID
                                          && p.MATERIAL_NO == reqItem.REPEAT_SO_MATERIAL_NO
                                        select p).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                    if (processPATmp != null)
                    {
                        int stepPGID = 0;
                        stepPGID = context.ART_M_STEP_ARTWORK.Where(s => s.STEP_ARTWORK_CODE == "SEND_PG")
                                                                .Select(l => l.STEP_ARTWORK_ID)
                                                                .FirstOrDefault();

                        var listAllSubID = CNService.FindArtworkSubId(processPATmp.ARTWORK_SUB_ID, context);

                        var processPG = (from p in context.ART_WF_ARTWORK_PROCESS_PG
                                         where listAllSubID.Contains(p.ARTWORK_SUB_ID)
                                         && p.ACTION_CODE == "SUBMIT"
                                         select p).OrderByDescending(o => o.ARTWORK_SUB_PA_ID).FirstOrDefault();

                        if (processPG != null)
                        {
                            if (processPG.DIE_LINE_MOCKUP_ID != null)
                            {
                                ART_WF_ARTWORK_PROCESS processNew = new ART_WF_ARTWORK_PROCESS();
                                processNew.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                                processNew.ARTWORK_ITEM_ID = process.ARTWORK_ITEM_ID;
                                processNew.PARENT_ARTWORK_SUB_ID = processPA.ARTWORK_SUB_ID;
                                processNew.CURRENT_USER_ID = -1;
                                processNew.UPDATE_BY = -1;
                                processNew.CREATE_BY = -1;
                                processNew.IS_END = "X";
                                processNew.CURRENT_STEP_ID = stepPGID;
                                ART_WF_ARTWORK_PROCESS_SERVICE.SaveOrUpdateNoLog(processNew, context);

                                ART_WF_ARTWORK_PROCESS_PG processPGNew = new ART_WF_ARTWORK_PROCESS_PG();
                                processPGNew.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                                processPGNew.ACTION_CODE = "SUBMIT";
                                processPGNew.ARTWORK_SUB_ID = processNew.ARTWORK_SUB_ID;
                                processPGNew.DIE_LINE_MOCKUP_ID = processPG.DIE_LINE_MOCKUP_ID;
                                processPGNew.CREATE_BY = -1;
                                processPGNew.UPDATE_BY = -1;
                                ART_WF_ARTWORK_PROCESS_PG_SERVICE.SaveOrUpdateNoLog(processPGNew, context);

                                var processPA2 = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                  where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                                  select p).FirstOrDefault();

                                if (processPA2 != null)
                                {
                                    int stepMOPGID = 0;
                                    stepMOPGID = context.ART_M_STEP_MOCKUP.Where(s => s.STEP_MOCKUP_CODE == "SEND_PG")
                                                                            .Select(l => l.STEP_MOCKUP_ID)
                                                                            .FirstOrDefault();

                                    var mockupProcess = (from m in context.ART_WF_MOCKUP_PROCESS
                                                         where m.MOCKUP_ID == processPG.DIE_LINE_MOCKUP_ID
                                                           && m.CURRENT_STEP_ID == stepMOPGID
                                                         select m).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                                    var mockupProcessPG = (from g in context.ART_WF_MOCKUP_PROCESS_PG
                                                           where g.MOCKUP_SUB_ID == mockupProcess.MOCKUP_SUB_ID
                                                           select g).OrderByDescending(o => o.MOCKUP_SUB_PG_ID).FirstOrDefault();

                                    processPA2.PG_USER_ID = mockupProcessPG.UPDATE_BY;
                                    processPA2.CHANGE_POINT = processPATmp.CHANGE_POINT;

                                    if (processPATmp.CHANGE_POINT == null)
                                    {
                                        var material_conversion = (from p in context.SAP_M_MATERIAL_CONVERSION
                                                                   where p.MATERIAL_NO == processPA2.MATERIAL_NO
                                                                    && p.CHAR_NAME == "ZPKG_SEC_CHANGE_POINT"
                                                                   select p).FirstOrDefault();

                                        if (material_conversion != null)
                                        {
                                            if (material_conversion.CHAR_VALUE == "N")
                                            {
                                                processPA2.CHANGE_POINT = "0";
                                            }
                                            else
                                            {
                                                processPA2.CHANGE_POINT = "1";
                                            }
                                        }
                                    }

                                    ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA2, context);
                                }
                            }
                        }
                    }
                    else
                    {
                        var processPA2 = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                          where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                          select p).FirstOrDefault();

                        if (processPA2 != null)
                        {
                            if (processPA2.CHANGE_POINT == null)
                            {
                                var material_conversion = (from p in context.SAP_M_MATERIAL_CONVERSION
                                                           where p.MATERIAL_NO == processPA2.MATERIAL_NO
                                                            && p.CHAR_NAME == "ZPKG_SEC_CHANGE_POINT"
                                                           select p).FirstOrDefault();

                                if (material_conversion != null)
                                {
                                    if (material_conversion.CHAR_VALUE == "N")
                                    {
                                        processPA2.CHANGE_POINT = "0";
                                    }
                                    else
                                    {
                                        processPA2.CHANGE_POINT = "1";
                                    }

                                    ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdateNoLog(processPA2, context);
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

        private static void DeleteAssignSalesOrder(int artwork_sub_id, ARTWORKEntities context)
        {
            ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 temp = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
            temp.ARTWORK_SUB_ID = artwork_sub_id;
            SalesOrderHelper.DeleteAssignSalesOrder(temp, context);

            context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_SO_DETAIL WHERE ARTWORK_SUB_ID  = '" + artwork_sub_id + "'");
        }

        public static ART_WF_ARTWORK_PROCESS_PA_RESULT CopyPADataFromAW(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();

            try
            {
                if (param == null && param.data == null)
                {
                    return Results;
                }
                if (param.data.ARTWORK_ITEM_ID_COPY == null && param.data.ARTWORK_SUB_ID <= 0)
                {
                    return Results;
                }

                int subIDDest = 0;
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        var stepPA = (from p in context.ART_M_STEP_ARTWORK
                                      where p.STEP_ARTWORK_CODE == "SEND_PA"
                                      select p.STEP_ARTWORK_ID).FirstOrDefault();

                        var awItemID = (from p in context.ART_WF_ARTWORK_REQUEST_ITEM
                                        where p.ARTWORK_ITEM_ID == param.data.ARTWORK_ITEM_ID_COPY
                                        select p.ARTWORK_ITEM_ID).FirstOrDefault();

                        if (awItemID > 0)
                        {
                            var processDest = (from p in context.ART_WF_ARTWORK_PROCESS
                                               where p.ARTWORK_ITEM_ID == awItemID
                                               && p.CURRENT_STEP_ID == stepPA
                                               select p.ARTWORK_SUB_ID).FirstOrDefault();

                            if (processDest > 0)
                            {
                                var processPADest_1 = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                       where p.ARTWORK_SUB_ID == processDest
                                                       select p).FirstOrDefault();

                                subIDDest = processPADest_1.ARTWORK_SUB_ID;

                                var processPASrc = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                    where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                                    select p).FirstOrDefault();

                                var processSrc = (from p in context.ART_WF_ARTWORK_PROCESS
                                                  where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                                  select p).FirstOrDefault();


                                var awRequestSrc = (from p in context.ART_WF_ARTWORK_REQUEST
                                                    where p.ARTWORK_REQUEST_ID == processSrc.ARTWORK_REQUEST_ID
                                                    select p).FirstOrDefault();

                                if (processPADest_1 != null && processPASrc != null)
                                {

                                    ART_WF_ARTWORK_PROCESS_PA paData = new ART_WF_ARTWORK_PROCESS_PA();

                                    paData.ARTWORK_SUB_PA_ID = processPASrc.ARTWORK_SUB_PA_ID;
                                    paData.ARTWORK_SUB_ID = processPASrc.ARTWORK_SUB_ID;
                                    paData.CHANGE_POINT = processPASrc.CHANGE_POINT;
                                    paData.MATERIAL_NO = processPASrc.MATERIAL_NO;
                                    paData.CREATE_BY = param.data.UPDATE_BY;
                                    paData.UPDATE_BY = param.data.UPDATE_BY;
                                    paData.REQUEST_MATERIAL_STATUS = processPASrc.REQUEST_MATERIAL_STATUS;

                                    paData.PA_USER_ID = processPASrc.PA_USER_ID;
                                    paData.PG_USER_ID = processPASrc.PG_USER_ID;
                                    paData.MATERIAL_GROUP_ID = processPADest_1.MATERIAL_GROUP_ID;
                                    paData.MATERIAL_NO = processPASrc.MATERIAL_NO;
                                    paData.RD_REFERENCE_NO_ID = processPASrc.RD_REFERENCE_NO_ID;
                                    paData.PRODUCT_CODE_ID = processPASrc.PRODUCT_CODE_ID;
                                    paData.TYPE_OF_ID = processPADest_1.TYPE_OF_ID;
                                    paData.TYPE_OF_OTHER = processPADest_1.TYPE_OF_OTHER;
                                    paData.PRIMARY_SIZE_ID = processPADest_1.PRIMARY_SIZE_ID;
                                    paData.PRIMARY_SIZE_OTHER = processPADest_1.PRIMARY_SIZE_OTHER;
                                    paData.CONTAINER_TYPE_ID = processPADest_1.CONTAINER_TYPE_ID;
                                    paData.CONTAINER_TYPE_OTHER = processPADest_1.CONTAINER_TYPE_OTHER;
                                    paData.LID_TYPE_ID = processPADest_1.LID_TYPE_ID;
                                    paData.LID_TYPE_OTHER = processPADest_1.LID_TYPE_OTHER;
                                    paData.TYPE_OF_2_ID = processPADest_1.TYPE_OF_2_ID;
                                    paData.TYPE_OF_2_OTHER = processPADest_1.TYPE_OF_2_OTHER;
                                    paData.PACKING_STYLE_ID = processPADest_1.PACKING_STYLE_ID;
                                    paData.PACKING_STYLE_OTHER = processPADest_1.PACKING_STYLE_OTHER;
                                    paData.PACK_SIZE_ID = processPADest_1.PACK_SIZE_ID;
                                    paData.PACK_SIZE_OTHER = processPADest_1.PACK_SIZE_OTHER;
                                    paData.PLANT_REGISTERED_ID = processPADest_1.PLANT_REGISTERED_ID;
                                    paData.PLANT_REGISTERED_OTHER = processPADest_1.PLANT_REGISTERED_OTHER;
                                    paData.COMPANY_ADDRESS_ID = processPADest_1.COMPANY_ADDRESS_ID;
                                    paData.COMPANY_ADDRESS_OTHER = processPADest_1.COMPANY_ADDRESS_OTHER;
                                    paData.PRODICUTION_PLANT_ID = processPADest_1.PRODICUTION_PLANT_ID;
                                    paData.PRODICUTION_PLANT_OTHER = processPADest_1.PRODICUTION_PLANT_OTHER;
                                    paData.CATCHING_PERIOD_ID = processPADest_1.CATCHING_PERIOD_ID;
                                    paData.CATCHING_PERIOD_OTHER = processPADest_1.CATCHING_PERIOD_OTHER;
                                    paData.CATCHING_METHOD_ID = processPADest_1.CATCHING_METHOD_ID;
                                    paData.CATCHING_METHOD_OTHER = processPADest_1.CATCHING_METHOD_OTHER;
                                    paData.SCIENTIFIC_NAME_ID = processPADest_1.SCIENTIFIC_NAME_ID;
                                    paData.SCIENTIFIC_NAME_OTHER = processPADest_1.SCIENTIFIC_NAME_OTHER;
                                    paData.SPECIE_ID = processPADest_1.SPECIE_ID;
                                    paData.SPECIE_OTHER = processPADest_1.SPECIE_OTHER;
                                    paData.PMS_COLOUR_ID = processPADest_1.PMS_COLOUR_ID;
                                    paData.PMS_COLOUR_OTHER = processPADest_1.PMS_COLOUR_OTHER;
                                    paData.PROCESS_COLOUR_ID = processPADest_1.PROCESS_COLOUR_ID;
                                    paData.PROCESS_COLOUR_OTHER = processPADest_1.PROCESS_COLOUR_OTHER;
                                    paData.TOTAL_COLOUR_ID = processPADest_1.TOTAL_COLOUR_ID;
                                    paData.TOTAL_COLOUR_OTHER = processPADest_1.TOTAL_COLOUR_OTHER;
                                    paData.STYLE_OF_PRINTING_ID = processPADest_1.STYLE_OF_PRINTING_ID;
                                    paData.STYLE_OF_PRINTING_OTHER = processPADest_1.STYLE_OF_PRINTING_OTHER;
                                    paData.DIRECTION_OF_STICKER_ID = processPADest_1.DIRECTION_OF_STICKER_ID;
                                    paData.DIRECTION_OF_STICKER_OTHER = processPADest_1.DIRECTION_OF_STICKER_OTHER;
                                    paData.PRINTING_STYLE_OF_PRIMARY_ID = processPADest_1.PRINTING_STYLE_OF_PRIMARY_ID;
                                    paData.PRINTING_STYLE_OF_PRIMARY_OTHER = processPADest_1.PRINTING_STYLE_OF_PRIMARY_OTHER;
                                    paData.CUSTOMER_DESIGN = processPADest_1.CUSTOMER_DESIGN;
                                    paData.CUSTOMER_DESIGN_OTHER = processPADest_1.CUSTOMER_DESIGN_OTHER;
                                    paData.CUSTOMER_SPEC = processPADest_1.CUSTOMER_SPEC;
                                    paData.CUSTOMER_SPEC_OTHER = processPADest_1.CUSTOMER_SPEC_OTHER;
                                    paData.CUSTOMER_SIZE = processPADest_1.CUSTOMER_SIZE;
                                    paData.CUSTOMER_SIZE_OTHER = processPADest_1.CUSTOMER_SIZE_OTHER;
                                    paData.CUSTOMER_NOMINATES_VENDOR = processPADest_1.CUSTOMER_NOMINATES_VENDOR;
                                    paData.CUSTOMER_NOMINATES_VENDOR_OTHER = processPADest_1.CUSTOMER_NOMINATES_VENDOR_OTHER;
                                    paData.CUSTOMER_NOMINATES_COLOR = processPADest_1.CUSTOMER_NOMINATES_COLOR;
                                    paData.CUSTOMER_NOMINATES_COLOR_OTHER = processPADest_1.CUSTOMER_NOMINATES_COLOR_OTHER;
                                    paData.CUSTOMER_BARCODE_SCANABLE = processPADest_1.CUSTOMER_BARCODE_SCANABLE;
                                    paData.CUSTOMER_BARCODE_SCANABLE_OTHER = processPADest_1.CUSTOMER_BARCODE_SCANABLE_OTHER;
                                    paData.CUSTOMER_BARCODE_SPEC = processPADest_1.CUSTOMER_BARCODE_SPEC;
                                    paData.CUSTOMER_BARCODE_SPEC_OTHER = processPADest_1.CUSTOMER_BARCODE_SPEC_OTHER;
                                    paData.PRINTING_STYLE_OF_SECONDARY_ID = processPADest_1.PRINTING_STYLE_OF_SECONDARY_ID;
                                    paData.PRINTING_STYLE_OF_SECONDARY_OTHER = processPADest_1.PRINTING_STYLE_OF_SECONDARY_OTHER;
                                    paData.FIRST_INFOGROUP_ID = processPADest_1.FIRST_INFOGROUP_ID;
                                    paData.FIRST_INFOGROUP_OTHER = processPADest_1.FIRST_INFOGROUP_OTHER;
                                    paData.PIC_MKT = processPADest_1.PIC_MKT;
                                    paData.COMPLETE_INFOGROUP = processPADest_1.COMPLETE_INFOGROUP;
                                    paData.NOTE_OF_PA = processPADest_1.NOTE_OF_PA;
                                    paData.PACKAGE_QUANTITY = processPADest_1.PACKAGE_QUANTITY;
                                    paData.WASTE_PERCENT = processPADest_1.WASTE_PERCENT;
                                    paData.COURIER_NO = processPADest_1.COURIER_NO;
                                    paData.PRODUCTION_EXPIRY_DATE_SYSTEM = processPADest_1.PRODUCTION_EXPIRY_DATE_SYSTEM;
                                    paData.SERIOUSNESS_OF_COLOR_PRINTING = processPADest_1.SERIOUSNESS_OF_COLOR_PRINTING;
                                    paData.SHADE_LIMIT = processPADest_1.SHADE_LIMIT;
                                    paData.NUTRITION_ANALYSIS = processPADest_1.NUTRITION_ANALYSIS;
                                    paData.THREE_P_ID = processPADest_1.THREE_P_ID;
                                    paData.TWO_P_ID = processPADest_1.TWO_P_ID;
                                    //paData.READY_CREATE_PO = processPADest_1.READY_CREATE_PO;
                                    //paData.RECEIVE_SHADE_LIMIT = processPADest_1.RECEIVE_SHADE_LIMIT;
                                    paData.REFERENCE_MATERIAL = processPASrc.REFERENCE_MATERIAL;
                                    // paData.REQUEST_MATERIAL_STATUS = processPADest_1.REQUEST_MATERIAL_STATUS;
                                    //paData.CHANGE_POINT = processPADest_1.CHANGE_POINT;
                                    paData.IS_LOCK_PRODUCT_CODE = processPADest_1.IS_LOCK_PRODUCT_CODE;

                                    if (awRequestSrc.TYPE_OF_ARTWORK == "REPEAT")
                                    {
                                        paData.IS_RETRIEVE_BY_AW_REPEAT = "X"; //processPADest_1.IS_RETRIEVE_BY_AW_REPEAT;
                                    }

                                    ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(paData, context);

                                    #region "FAO"

                                    context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE WHERE ARTWORK_SUB_ID  = '" + param.data.ARTWORK_SUB_ID + "'");

                                    var faoDest = (from f in context.ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE
                                                   where f.ARTWORK_SUB_ID == subIDDest
                                                   select f).ToList();

                                    if (faoDest != null)
                                    {
                                        ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE fao = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                                        foreach (var itemFAO in faoDest)
                                        {
                                            fao = new ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE();
                                            fao.ARTWORK_SUB_ID = processPASrc.ARTWORK_SUB_ID;
                                            fao.ARTWORK_SUB_PA_ID = processPASrc.ARTWORK_SUB_PA_ID;
                                            fao.FAO_ZONE_ID = itemFAO.FAO_ZONE_ID;
                                            fao.FAO_ZONE_OTHER = itemFAO.FAO_ZONE_OTHER;
                                            fao.CREATE_BY = param.data.UPDATE_BY;
                                            fao.UPDATE_BY = param.data.UPDATE_BY;
                                            ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_SERVICE.SaveOrUpdate(fao, context);
                                        }
                                    }
                                    #endregion

                                    #region "Symbol"

                                    context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_PA_SYMBOL WHERE ARTWORK_SUB_ID  = '" + param.data.ARTWORK_SUB_ID + "'");


                                    var symbolDest = (from f in context.ART_WF_ARTWORK_PROCESS_PA_SYMBOL
                                                      where f.ARTWORK_SUB_ID == subIDDest
                                                      select f).ToList();

                                    if (symbolDest != null)
                                    {
                                        ART_WF_ARTWORK_PROCESS_PA_SYMBOL symbol = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                                        foreach (var itemSymbol in symbolDest)
                                        {
                                            symbol = new ART_WF_ARTWORK_PROCESS_PA_SYMBOL();
                                            symbol.ARTWORK_SUB_ID = processPASrc.ARTWORK_SUB_ID;
                                            symbol.ARTWORK_SUB_PA_ID = processPASrc.ARTWORK_SUB_PA_ID;
                                            symbol.SYMBOL_ID = itemSymbol.SYMBOL_ID;
                                            symbol.SYMBOL_OTHER = itemSymbol.SYMBOL_OTHER;
                                            symbol.CREATE_BY = param.data.UPDATE_BY;
                                            symbol.UPDATE_BY = param.data.UPDATE_BY;
                                            ART_WF_ARTWORK_PROCESS_PA_SYMBOL_SERVICE.SaveOrUpdate(symbol, context);
                                        }
                                    }
                                    #endregion

                                    #region "Catching Area"

                                    context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA WHERE ARTWORK_SUB_ID  = '" + param.data.ARTWORK_SUB_ID + "'");


                                    var catchingAreaDest = (from f in context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA
                                                            where f.ARTWORK_SUB_ID == subIDDest
                                                            select f).ToList();

                                    if (catchingAreaDest != null)
                                    {
                                        ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA catchingArea = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                                        foreach (var itemCatchingArea in catchingAreaDest)
                                        {
                                            catchingArea = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA();
                                            catchingArea.ARTWORK_SUB_ID = processPASrc.ARTWORK_SUB_ID;
                                            catchingArea.ARTWORK_SUB_PA_ID = processPASrc.ARTWORK_SUB_PA_ID;
                                            catchingArea.CATCHING_AREA_ID = itemCatchingArea.CATCHING_AREA_ID;
                                            catchingArea.CATCHING_AREA_OTHER = itemCatchingArea.CATCHING_AREA_OTHER;
                                            catchingArea.CREATE_BY = param.data.UPDATE_BY;
                                            catchingArea.UPDATE_BY = param.data.UPDATE_BY;
                                            ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_SERVICE.SaveOrUpdate(catchingArea, context);
                                        }
                                    }
                                    #endregion



                                    #region "Catching Medthod"
                                    // ticke#425737 added by aof 
                                    context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD WHERE ARTWORK_SUB_ID  = '" + param.data.ARTWORK_SUB_ID + "'");


                                    var catchingMethodDest = (from f in context.ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD
                                                            where f.ARTWORK_SUB_ID == subIDDest
                                                            select f).ToList();

                                    if (catchingMethodDest != null)
                                    {
                                        ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD catchingMethod = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                                        foreach (var itemCatchingMethod in catchingMethodDest)
                                        {
                                            catchingMethod = new ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD();
                                            catchingMethod.ARTWORK_SUB_ID = processPASrc.ARTWORK_SUB_ID;
                                            catchingMethod.ARTWORK_SUB_PA_ID = processPASrc.ARTWORK_SUB_PA_ID;
                                            catchingMethod.CATCHING_METHOD_ID = itemCatchingMethod.CATCHING_METHOD_ID;
                                            catchingMethod.CATCHING_METHOD_OTHER = itemCatchingMethod.CATCHING_METHOD_OTHER;
                                            catchingMethod.CREATE_BY = param.data.UPDATE_BY;
                                            catchingMethod.UPDATE_BY = param.data.UPDATE_BY;
                                            ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_SERVICE.SaveOrUpdate(catchingMethod, context);
                                        }
                                    }
                                    // ticke#425737 added by aof 
                                    #endregion


                                    #region "Plant"

                                    //var plantOld = (from f in context.ART_WF_ARTWORK_PROCESS_PA_PLANT
                                    //                       where f.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                    //                       select f).ToList();

                                    //if (plantOld != null)
                                    //{
                                    //    foreach (var itemPlantOld in plantOld)
                                    //    {
                                    //        ART_WF_ARTWORK_PROCESS_PA_PLANT_SERVICE.DeleteByARTWORK_SUB_PA_PLANT_ID(itemPlantOld.ARTWORK_SUB_PA_PLANT_ID, context);
                                    //    }
                                    //}

                                    context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_PA_PLANT WHERE ARTWORK_SUB_ID  = '" + param.data.ARTWORK_SUB_ID + "'");

                                    var plantDest = (from f in context.ART_WF_ARTWORK_PROCESS_PA_PLANT
                                                     where f.ARTWORK_SUB_ID == subIDDest
                                                     select f).ToList();

                                    if (plantDest != null)
                                    {
                                        ART_WF_ARTWORK_PROCESS_PA_PLANT plant = new ART_WF_ARTWORK_PROCESS_PA_PLANT();
                                        foreach (var itemPlant in plantDest)
                                        {
                                            plant = new ART_WF_ARTWORK_PROCESS_PA_PLANT();
                                            plant.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                            plant.ARTWORK_SUB_PA_ID = processPASrc.ARTWORK_SUB_PA_ID;
                                            plant.PLANT_ID = itemPlant.PLANT_ID;
                                            plant.PLANT_OTHER = itemPlant.PLANT_OTHER;
                                            plant.CREATE_BY = param.data.UPDATE_BY;
                                            plant.UPDATE_BY = param.data.UPDATE_BY;
                                            ART_WF_ARTWORK_PROCESS_PA_PLANT_SERVICE.SaveOrUpdate(plant, context);
                                        }
                                    }
                                    #endregion

                                    #region "Product"

                                    //context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_PA_PRODUCT WHERE ARTWORK_SUB_ID  = '" + param.data.ARTWORK_SUB_ID + "'");

                                    //var productDest = (from f in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT
                                    //                   where f.ARTWORK_SUB_ID == subIDDest
                                    //                   select f).ToList();

                                    //if (productDest != null)
                                    //{
                                    //    ART_WF_ARTWORK_PROCESS_PA_PRODUCT product = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                                    //    foreach (var itemProduct in productDest)
                                    //    {
                                    //        product = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT();
                                    //        product.ARTWORK_SUB_ID = processPASrc.ARTWORK_SUB_ID;
                                    //        product.ARTWORK_SUB_PA_ID = processPASrc.ARTWORK_SUB_PA_ID;
                                    //        product.PRODUCT_CODE_ID = itemProduct.PRODUCT_CODE_ID;

                                    //        product.CREATE_BY = param.data.UPDATE_BY;
                                    //        product.UPDATE_BY = param.data.UPDATE_BY;
                                    //        ART_WF_ARTWORK_PROCESS_PA_PRODUCT_SERVICE.SaveOrUpdate(product, context);
                                    //    }
                                    //}
                                    #endregion

                                    #region "Product Other"

                                    //context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER WHERE ARTWORK_SUB_ID  = '" + param.data.ARTWORK_SUB_ID + "'");

                                    //var productOtherDest = (from f in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER
                                    //                        where f.ARTWORK_SUB_ID == subIDDest
                                    //                        select f).ToList();

                                    //if (productOtherDest != null)
                                    //{
                                    //    ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER productOther = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();
                                    //    foreach (var itemProductOther in productOtherDest)
                                    //    {
                                    //        productOther = new ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER();
                                    //        productOther.ARTWORK_SUB_ID = processPASrc.ARTWORK_SUB_ID;
                                    //        productOther.ARTWORK_SUB_PA_ID = processPASrc.ARTWORK_SUB_PA_ID;
                                    //        productOther.PRODUCT_CODE = itemProductOther.PRODUCT_CODE;
                                    //        productOther.NET_WEIGHT = itemProductOther.NET_WEIGHT;
                                    //        productOther.DRAINED_WEIGHT = itemProductOther.DRAINED_WEIGHT;
                                    //        productOther.CREATE_BY = param.data.UPDATE_BY;
                                    //        productOther.UPDATE_BY = param.data.UPDATE_BY;
                                    //        ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_SERVICE.SaveOrUpdate(productOther, context);
                                    //    }
                                    //}
                                    #endregion



                                    Results.status = "S";
                                    Results.msg = MessageHelper.GetMessage("MSG_001", context);
                                }
                            }
                        }

                        dbContextTransaction.Commit();

                    }
                }

            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
                return Results;
            }

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_PA_RESULT ReplaceMat(string wfno, string mat)
        {
            ART_WF_ARTWORK_PROCESS_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PA_RESULT();
            try
            {
                if (CNService.IsDevOrQAS())
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (var dbContextTransaction = CNService.IsolationLevel(context))
                        {
                            var item = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_ITEM() { REQUEST_ITEM_NO = wfno }, context).FirstOrDefault();
                            var listProcess = ART_WF_ARTWORK_PROCESS_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS() { ARTWORK_ITEM_ID = item.ARTWORK_ITEM_ID }, context).ToList();
                            var process = listProcess.Where(m => m.PARENT_ARTWORK_SUB_ID == null).FirstOrDefault();

                            var processPA = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PA() { ARTWORK_SUB_ID = process.ARTWORK_SUB_ID }, context).FirstOrDefault();
                            processPA.MATERIAL_NO = mat;
                            processPA.REQUEST_MATERIAL_STATUS = "Completed";
                            ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);

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
    }
}
