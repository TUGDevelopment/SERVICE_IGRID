using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class MapperServices
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<V_ART_SEARCH_DIELINE_2, V_ART_SEARCH_DIELINE>();
                cfg.CreateMap<V_ART_SEARCH_DIELINE, V_ART_SEARCH_DIELINE_2>();

                cfg.CreateMap<ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_2, ART_M_RECEIVE_EMAIL_NEW_CUSTOMER>();
                cfg.CreateMap<ART_M_RECEIVE_EMAIL_NEW_CUSTOMER, ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_2>();

                cfg.CreateMap<ART_M_EMAIL_TEMPLATE_2, ART_M_EMAIL_TEMPLATE>();
                cfg.CreateMap<ART_M_EMAIL_TEMPLATE, ART_M_EMAIL_TEMPLATE_2>();

                cfg.CreateMap<ART_WF_ARTWORK_MAPPING_PO_2, ART_WF_ARTWORK_MAPPING_PO>();
                cfg.CreateMap<ART_WF_ARTWORK_MAPPING_PO, ART_WF_ARTWORK_MAPPING_PO_2>();

                cfg.CreateMap<ART_WF_ARTWORK_MATERIAL_LOCK_2, ART_WF_ARTWORK_MATERIAL_LOCK>();
                cfg.CreateMap<ART_WF_ARTWORK_MATERIAL_LOCK, ART_WF_ARTWORK_MATERIAL_LOCK_2>();

                cfg.CreateMap<ART_WF_ARTWORK_MATERIAL_LOCK_LOG_2, ART_WF_ARTWORK_MATERIAL_LOCK_LOG>();
                cfg.CreateMap<ART_WF_ARTWORK_MATERIAL_LOCK_LOG, ART_WF_ARTWORK_MATERIAL_LOCK_LOG_2>();

                cfg.CreateMap<ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_2, ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL>();
                cfg.CreateMap<ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL, ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_2>();

                cfg.CreateMap<ART_WF_ARTWORK_MATERIAL_LOCK_2, ART_WF_ARTWORK_MATERIAL_LOCK>();
                cfg.CreateMap<ART_WF_ARTWORK_MATERIAL_LOCK, ART_WF_ARTWORK_MATERIAL_LOCK_2>();

                cfg.CreateMap<XECM_M_VENDOR_2, XECM_M_VENDOR>();
                cfg.CreateMap<XECM_M_VENDOR, XECM_M_VENDOR_2>();

                cfg.CreateMap<XECM_M_CUSTOMER_2, XECM_M_CUSTOMER>();
                cfg.CreateMap<XECM_M_CUSTOMER, XECM_M_CUSTOMER_2>();

                cfg.CreateMap<ART_M_POSITION_ROLE_2, ART_M_POSITION_ROLE>();
                cfg.CreateMap<ART_M_POSITION_ROLE, ART_M_POSITION_ROLE_2>();

                cfg.CreateMap<ART_M_PERMISSION_2, ART_M_PERMISSION>();
                cfg.CreateMap<ART_M_PERMISSION, ART_M_PERMISSION_2>();

                cfg.CreateMap<ART_M_PIC_2, ART_M_PIC>();
                cfg.CreateMap<ART_M_PIC, ART_M_PIC_2>();

                cfg.CreateMap<V_ART_ASSIGNED_SO_2, V_ART_ASSIGNED_SO>();
                cfg.CreateMap<V_ART_ASSIGNED_SO, V_ART_ASSIGNED_SO_2>();

                cfg.CreateMap<V_SAP_SALES_ORDER_2, V_SAP_SALES_ORDER>();
                cfg.CreateMap<V_SAP_SALES_ORDER, V_SAP_SALES_ORDER_2>();

                cfg.CreateMap<V_ART_WF_ALL_PROCESS_2, V_ART_WF_ALL_PROCESS>();
                cfg.CreateMap<V_ART_WF_ALL_PROCESS, V_ART_WF_ALL_PROCESS_2>();

                cfg.CreateMap<SAP_M_PO_IDOC_2, SAP_M_PO_IDOC>();
                cfg.CreateMap<SAP_M_PO_IDOC, SAP_M_PO_IDOC_2>();

                cfg.CreateMap<IGRID_M_OUTBOUND_HEADER_2, IGRID_M_OUTBOUND_HEADER>();
                cfg.CreateMap<IGRID_M_OUTBOUND_HEADER, IGRID_M_OUTBOUND_HEADER_2>();

                cfg.CreateMap<IGRID_M_OUTBOUND_ITEM_2, IGRID_M_OUTBOUND_ITEM>();
                cfg.CreateMap<IGRID_M_OUTBOUND_ITEM, IGRID_M_OUTBOUND_ITEM_2>();

                cfg.CreateMap<SAP_M_ORDER_BOM_2, SAP_M_ORDER_BOM>();
                cfg.CreateMap<SAP_M_ORDER_BOM, SAP_M_ORDER_BOM_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_2, ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA, ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_CUSTOMER_2, ART_WF_ARTWORK_PROCESS_CUSTOMER>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_CUSTOMER, ART_WF_ARTWORK_PROCESS_CUSTOMER_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2, ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA, ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_VENDOR_2, ART_WF_ARTWORK_PROCESS_VENDOR>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_VENDOR, ART_WF_ARTWORK_PROCESS_VENDOR_2>();

                cfg.CreateMap<ART_M_VENDOR_MATGROUP_2, ART_M_VENDOR_MATGROUP>();
                cfg.CreateMap<ART_M_VENDOR_MATGROUP, ART_M_VENDOR_MATGROUP_2>();

                cfg.CreateMap<ART_WF_DELEGATE_2, ART_WF_DELEGATE>();
                cfg.CreateMap<ART_WF_DELEGATE, ART_WF_DELEGATE_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_2, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC_2, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_2, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK_2, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_2, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK_2, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK_2, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK, ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK_2>();

                cfg.CreateMap<ART_WF_ARTWORK_ATTACHMENT_2, ART_WF_ARTWORK_ATTACHMENT>();
                cfg.CreateMap<ART_WF_ARTWORK_ATTACHMENT, ART_WF_ARTWORK_ATTACHMENT_2>();

                cfg.CreateMap<ART_M_PRINTING_STYLE_2, ART_M_PRINTING_STYLE>();
                cfg.CreateMap<ART_M_PRINTING_STYLE, ART_M_PRINTING_STYLE_2>();

                cfg.CreateMap<ART_SYS_LOG_2, ART_SYS_LOG>();
                cfg.CreateMap<ART_SYS_LOG, ART_SYS_LOG_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_2, ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA, ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_MARKETING_2, ART_WF_ARTWORK_PROCESS_MARKETING>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_MARKETING, ART_WF_ARTWORK_PROCESS_MARKETING_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PG_BY_PA_2, ART_WF_ARTWORK_PROCESS_PG_BY_PA>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PG_BY_PA, ART_WF_ARTWORK_PROCESS_PG_BY_PA_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_2, ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA, ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PP_BY_PA_2, ART_WF_ARTWORK_PROCESS_PP_BY_PA>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PP_BY_PA, ART_WF_ARTWORK_PROCESS_PP_BY_PA_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_QC_BY_PA_2, ART_WF_ARTWORK_PROCESS_QC_BY_PA>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_QC_BY_PA, ART_WF_ARTWORK_PROCESS_QC_BY_PA_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_RD_2, ART_WF_ARTWORK_PROCESS_RD>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_RD, ART_WF_ARTWORK_PROCESS_RD_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_WH_BY_PA_2, ART_WF_ARTWORK_PROCESS_WH_BY_PA>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_WH_BY_PA, ART_WF_ARTWORK_PROCESS_WH_BY_PA_2>();

                cfg.CreateMap<ART_M_POSITION_2, ART_M_POSITION>();
                cfg.CreateMap<ART_M_POSITION, ART_M_POSITION_2>();

                cfg.CreateMap<ART_M_USER_CUSTOMER_2, ART_M_USER_CUSTOMER>();
                cfg.CreateMap<ART_M_USER_CUSTOMER, ART_M_USER_CUSTOMER_2>();

                cfg.CreateMap<ART_M_USER_UPPER_LEVEL_2, ART_M_USER_UPPER_LEVEL>();
                cfg.CreateMap<ART_M_USER_UPPER_LEVEL, ART_M_USER_UPPER_LEVEL_2>();

                cfg.CreateMap<ART_M_USER_COMPANY_2, ART_M_USER_COMPANY>();
                cfg.CreateMap<ART_M_USER_COMPANY, ART_M_USER_COMPANY_2>();

                cfg.CreateMap<ART_M_USER_TYPE_OF_PRODUCT_2, ART_M_USER_TYPE_OF_PRODUCT>();
                cfg.CreateMap<ART_M_USER_TYPE_OF_PRODUCT, ART_M_USER_TYPE_OF_PRODUCT_2>();

                cfg.CreateMap<ART_M_DECISION_REASON_2, ART_M_DECISION_REASON>();
                cfg.CreateMap<ART_M_DECISION_REASON, ART_M_DECISION_REASON_2>();

                cfg.CreateMap<ART_M_USER_2, ART_M_USER>();
                cfg.CreateMap<ART_M_USER, ART_M_USER_2>();

                cfg.CreateMap<ART_M_MESSAGE_2, ART_M_MESSAGE>();
                cfg.CreateMap<ART_M_MESSAGE, ART_M_MESSAGE_2>();

                cfg.CreateMap<SAP_M_BRAND, SAP_M_BRAND_2>();
                cfg.CreateMap<SAP_M_BRAND_2, SAP_M_BRAND>();

                cfg.CreateMap<SAP_M_TYPE_OF_PRODUCT, SAP_M_TYPE_OF_PRODUCT_2>();
                cfg.CreateMap<SAP_M_TYPE_OF_PRODUCT_2, SAP_M_TYPE_OF_PRODUCT>();

                cfg.CreateMap<SAP_M_COMPANY, SAP_M_COMPANY_2>();
                cfg.CreateMap<SAP_M_COMPANY_2, SAP_M_COMPANY>();

                cfg.CreateMap<SAP_M_COUNTRY, SAP_M_COUNTRY_2>();
                cfg.CreateMap<SAP_M_COUNTRY_2, SAP_M_COUNTRY>();

                cfg.CreateMap<SAP_M_PLANT, SAP_M_PLANT_2>();
                cfg.CreateMap<SAP_M_PLANT_2, SAP_M_PLANT>();

                cfg.CreateMap<SAP_M_PO_IDOC_ITEM, SAP_M_PO_IDOC_ITEM_2>();
                cfg.CreateMap<SAP_M_PO_IDOC_ITEM_2, SAP_M_PO_IDOC_ITEM>();

                cfg.CreateMap<XECM_M_CUSTOMER, XECM_M_CUSTOMER_2>();
                cfg.CreateMap<XECM_M_CUSTOMER_2, XECM_M_CUSTOMER>();

                cfg.CreateMap<ART_WF_MOCKUP_ATTACHMENT, ART_WF_MOCKUP_ATTACHMENT_2>();
                cfg.CreateMap<ART_WF_MOCKUP_ATTACHMENT_2, ART_WF_MOCKUP_ATTACHMENT>();

                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST, ART_WF_MOCKUP_CHECK_LIST_2>();
                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_2, ART_WF_MOCKUP_CHECK_LIST>();

                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_COUNTRY, ART_WF_MOCKUP_CHECK_LIST_COUNTRY_2>();
                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_COUNTRY_2, ART_WF_MOCKUP_CHECK_LIST_COUNTRY>();

                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT, ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_2>();
                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_2, ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT>();

                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_PRODUCT, ART_WF_MOCKUP_CHECK_LIST_PRODUCT_2>();
                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_PRODUCT_2, ART_WF_MOCKUP_CHECK_LIST_PRODUCT>();

                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_REFERENCE, ART_WF_MOCKUP_CHECK_LIST_REFERENCE_2>();
                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_REFERENCE_2, ART_WF_MOCKUP_CHECK_LIST_REFERENCE>();

                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_ITEM, ART_WF_MOCKUP_CHECK_LIST_ITEM_2>();
                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_ITEM_2, ART_WF_MOCKUP_CHECK_LIST_ITEM>();

                cfg.CreateMap<SAP_M_CHARACTERISTIC, SAP_M_CHARACTERISTIC_2>();
                cfg.CreateMap<SAP_M_CHARACTERISTIC_2, SAP_M_CHARACTERISTIC>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS, ART_WF_MOCKUP_PROCESS_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_2, ART_WF_MOCKUP_PROCESS>();

                cfg.CreateMap<SAP_M_CHARACTERISTIC_ITEM, SAP_M_CHARACTERISTIC_ITEM_2>();
                cfg.CreateMap<SAP_M_CHARACTERISTIC_ITEM_2, SAP_M_CHARACTERISTIC_ITEM>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_PG, ART_WF_MOCKUP_PROCESS_PG_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_PG_2, ART_WF_MOCKUP_PROCESS_PG>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_RD, ART_WF_MOCKUP_PROCESS_RD_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_RD_2, ART_WF_MOCKUP_PROCESS_RD>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_PLANNING, ART_WF_MOCKUP_PROCESS_PLANNING_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_PLANNING_2, ART_WF_MOCKUP_PROCESS_PLANNING>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_WAREHOUSE, ART_WF_MOCKUP_PROCESS_WAREHOUSE_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_WAREHOUSE_2, ART_WF_MOCKUP_PROCESS_WAREHOUSE>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG, ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2, ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE, ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2, ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG, ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2, ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG>();

                cfg.CreateMap<V_ART_WF_DASHBOARD, V_ART_WF_DASHBOARD_2>();
                cfg.CreateMap<V_ART_WF_DASHBOARD_2, V_ART_WF_DASHBOARD>();

                cfg.CreateMap<V_ART_WF_DASHBOARD_ARTWORK, V_ART_WF_DASHBOARD_ARTWORK_2>();
                cfg.CreateMap<V_ART_WF_DASHBOARD_ARTWORK_2, V_ART_WF_DASHBOARD_ARTWORK>();

                cfg.CreateMap<XECM_M_VENDOR, XECM_M_VENDOR_2>();
                cfg.CreateMap<XECM_M_VENDOR_2, XECM_M_VENDOR>();

                cfg.CreateMap<ART_M_USER_VENDOR, ART_M_USER_VENDOR_2>();
                cfg.CreateMap<ART_M_USER_VENDOR_2, ART_M_USER_VENDOR>();

                cfg.CreateMap<ART_M_STEP_MOCKUP, ART_M_STEP_MOCKUP_2>();
                cfg.CreateMap<ART_M_STEP_MOCKUP_2, ART_M_STEP_MOCKUP>();

                cfg.CreateMap<ART_M_STEP_ARTWORK, ART_M_STEP_ARTWORK_2>();
                cfg.CreateMap<ART_M_STEP_ARTWORK_2, ART_M_STEP_ARTWORK>();

                cfg.CreateMap<SAP_M_3P, SAP_M_3P_2>();
                cfg.CreateMap<SAP_M_3P_2, SAP_M_3P>();

                cfg.CreateMap<SAP_M_2P, SAP_M_2P_2>();
                cfg.CreateMap<SAP_M_2P_2, SAP_M_2P>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_VENDOR, ART_WF_MOCKUP_PROCESS_VENDOR_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_VENDOR_2, ART_WF_MOCKUP_PROCESS_VENDOR>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_CUSTOMER, ART_WF_MOCKUP_PROCESS_CUSTOMER_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_CUSTOMER_2, ART_WF_MOCKUP_PROCESS_CUSTOMER>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG, ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_2, ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG, ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_2, ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG, ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2, ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG>();

                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST, ART_WF_MOCKUP_CHECK_LIST_PG>();
                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_PG, ART_WF_MOCKUP_CHECK_LIST>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_RD_BY_PG, ART_WF_MOCKUP_PROCESS_RD_BY_PG_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_RD_BY_PG_2, ART_WF_MOCKUP_PROCESS_RD_BY_PG>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG, ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_2, ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG>();

                cfg.CreateMap<ART_M_TEMPLATE, ART_M_TEMPLATE_2>();
                cfg.CreateMap<ART_M_TEMPLATE_2, ART_M_TEMPLATE>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR, ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_2, ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR>();

                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD, ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_2>();
                cfg.CreateMap<ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_2, ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD>();

                cfg.CreateMap<XECM_M_PRODUCT, XECM_M_PRODUCT_2>();
                cfg.CreateMap<XECM_M_PRODUCT_2, XECM_M_PRODUCT>();

                cfg.CreateMap<ART_M_USER_ROLE, ART_M_USER_ROLE_2>();
                cfg.CreateMap<ART_M_USER_ROLE_2, ART_M_USER_ROLE>();

                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER, ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_2>();
                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_2, ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER>();

                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_2, ART_WF_ARTWORK_REQUEST>();
                cfg.CreateMap<ART_WF_ARTWORK_REQUEST, ART_WF_ARTWORK_REQUEST_2>();

                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_ITEM_2, ART_WF_ARTWORK_REQUEST_ITEM>();
                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_ITEM, ART_WF_ARTWORK_REQUEST_ITEM_2>();

                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_RECIPIENT_2, ART_WF_ARTWORK_REQUEST_RECIPIENT>();
                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_RECIPIENT, ART_WF_ARTWORK_REQUEST_RECIPIENT_2>();

                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_PG, ART_WF_MOCKUP_CHECK_LIST_PG_2>();
                cfg.CreateMap<ART_WF_MOCKUP_CHECK_LIST_PG_2, ART_WF_MOCKUP_CHECK_LIST_PG>();

                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_COUNTRY, ART_WF_ARTWORK_REQUEST_COUNTRY_2>();
                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_COUNTRY_2, ART_WF_ARTWORK_REQUEST_COUNTRY>();

                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_PRODUCT, ART_WF_ARTWORK_REQUEST_PRODUCT_2>();
                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_PRODUCT_2, ART_WF_ARTWORK_REQUEST_PRODUCT>();

                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT, ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2>();
                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2, ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT>();

                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_REFERENCE, ART_WF_ARTWORK_REQUEST_REFERENCE_2>();
                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_REFERENCE_2, ART_WF_ARTWORK_REQUEST_REFERENCE>();

                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_SALES_ORDER, ART_WF_ARTWORK_REQUEST_SALES_ORDER_2>();
                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_SALES_ORDER_2, ART_WF_ARTWORK_REQUEST_SALES_ORDER>();

                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER, ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_2>();
                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_2, ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA_PRODUCT, ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2, ART_WF_ARTWORK_PROCESS_PA_PRODUCT>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA, ART_WF_ARTWORK_PROCESS_PA_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA_2, ART_WF_ARTWORK_PROCESS_PA>();

                cfg.CreateMap<SAP_M_PO_COMPLETE_SO_HEADER, SAP_M_PO_COMPLETE_SO_HEADER_2>();
                cfg.CreateMap<SAP_M_PO_COMPLETE_SO_HEADER_2, SAP_M_PO_COMPLETE_SO_HEADER>();

                cfg.CreateMap<SAP_M_PO_COMPLETE_SO_ITEM, SAP_M_PO_COMPLETE_SO_ITEM_2>();
                cfg.CreateMap<SAP_M_PO_COMPLETE_SO_ITEM_2, SAP_M_PO_COMPLETE_SO_ITEM>();

                cfg.CreateMap<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT, SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2>();
                cfg.CreateMap<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2, SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT>();
                
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS, ART_WF_ARTWORK_PROCESS_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_2, ART_WF_ARTWORK_PROCESS>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_SO_DETAIL, ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2, ART_WF_ARTWORK_PROCESS_SO_DETAIL>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_QC, ART_WF_ARTWORK_PROCESS_QC_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_QC_2, ART_WF_ARTWORK_PROCESS_QC>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PLANNING, ART_WF_ARTWORK_PROCESS_PLANNING_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PLANNING_2, ART_WF_ARTWORK_PROCESS_PLANNING>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_WH, ART_WF_ARTWORK_PROCESS_WH_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_WH_2, ART_WF_ARTWORK_PROCESS_WH>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_RD_BY_QC, ART_WF_ARTWORK_PROCESS_RD_BY_QC_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_RD_BY_QC_2, ART_WF_ARTWORK_PROCESS_RD_BY_QC>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PG, ART_WF_ARTWORK_PROCESS_PG_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PG_2, ART_WF_ARTWORK_PROCESS_PG>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT, ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2, ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM, ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2, ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER, ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2, ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA, ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_2, ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA>();

                // ticke#425737 added by aof 
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD, ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_2, ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD>();
                // ticke#425737 added by aof 

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE, ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_2, ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA_SYMBOL, ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2, ART_WF_ARTWORK_PROCESS_PA_SYMBOL>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA_PLANT, ART_WF_ARTWORK_PROCESS_PA_PLANT_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA_PLANT_2, ART_WF_ARTWORK_PROCESS_PA_PLANT>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PP, ART_WF_ARTWORK_PROCESS_PP_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PP_2, ART_WF_ARTWORK_PROCESS_PP>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP, ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2, ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_VENDOR_PO, ART_WF_ARTWORK_PROCESS_VENDOR_PO_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_VENDOR_PO_2, ART_WF_ARTWORK_PROCESS_VENDOR_PO>();

                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT, ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2>();
                cfg.CreateMap<ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2, ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT>();

                cfg.CreateMap<V_ART_WAREHOUSE_REPORT_2, V_ART_WAREHOUSE_REPORT>();
                cfg.CreateMap<V_ART_WAREHOUSE_REPORT, V_ART_WAREHOUSE_REPORT_2>();

                cfg.CreateMap<SAP_M_MATERIAL_CONVERSION_2, SAP_M_MATERIAL_CONVERSION>();
                cfg.CreateMap<SAP_M_MATERIAL_CONVERSION, SAP_M_MATERIAL_CONVERSION_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2, ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER, ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2>();

                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT, ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2>();
                cfg.CreateMap<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2, ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT>();

                cfg.CreateMap<XECM_M_PRODUCT5, XECM_M_PRODUCT5_2>();
                cfg.CreateMap<XECM_M_PRODUCT5_2, XECM_M_PRODUCT5>();

                cfg.CreateMap<ART_WF_REMARK_REASON_OTHER, ART_WF_REMARK_REASON_OTHER_2>();
                cfg.CreateMap<ART_WF_REMARK_REASON_OTHER_2, ART_WF_REMARK_REASON_OTHER>();

                cfg.CreateMap<V_SAP_SALES_ORDER_ALL, V_SAP_SALES_ORDER_ALL_2>();
                cfg.CreateMap<V_SAP_SALES_ORDER_ALL_2, V_SAP_SALES_ORDER_ALL>();

            });
        }

        public static V_ART_SEARCH_DIELINE V_ART_SEARCH_DIELINE(V_ART_SEARCH_DIELINE_2 item)
        {
            return AutoMapper.Mapper.Map<V_ART_SEARCH_DIELINE>(item);
        }
        public static V_ART_SEARCH_DIELINE_2 V_ART_SEARCH_DIELINE(V_ART_SEARCH_DIELINE item)
        {
            return AutoMapper.Mapper.Map<V_ART_SEARCH_DIELINE_2>(item);
        }
        public static List<V_ART_SEARCH_DIELINE_2> V_ART_SEARCH_DIELINE(List<V_ART_SEARCH_DIELINE> item)
        {
            return AutoMapper.Mapper.Map<List<V_ART_SEARCH_DIELINE_2>>(item);
        }

        public static ART_M_MESSAGE ART_M_MESSAGE(ART_M_MESSAGE_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_MESSAGE>(item);
        }
        public static ART_M_MESSAGE_2 ART_M_MESSAGE(ART_M_MESSAGE item)
        {
            return AutoMapper.Mapper.Map<ART_M_MESSAGE_2>(item);
        }
        public static List<ART_M_MESSAGE_2> ART_M_MESSAGE(List<ART_M_MESSAGE> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_MESSAGE_2>>(item);
        }

        public static ART_M_STEP_MOCKUP ART_M_STEP_MOCKUP(ART_M_STEP_MOCKUP_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_STEP_MOCKUP>(item);
        }
        public static ART_M_STEP_MOCKUP_2 ART_M_STEP_MOCKUP(ART_M_STEP_MOCKUP item)
        {
            return AutoMapper.Mapper.Map<ART_M_STEP_MOCKUP_2>(item);
        }
        public static List<ART_M_STEP_MOCKUP_2> ART_M_STEP_MOCKUP(List<ART_M_STEP_MOCKUP> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_STEP_MOCKUP_2>>(item);
        }

        public static V_ART_ASSIGNED_SO V_ART_ASSIGNED_SO(V_ART_ASSIGNED_SO_2 item)
        {
            return AutoMapper.Mapper.Map<V_ART_ASSIGNED_SO>(item);
        }
        public static V_ART_ASSIGNED_SO_2 V_ART_ASSIGNED_SO(V_ART_ASSIGNED_SO item)
        {
            return AutoMapper.Mapper.Map<V_ART_ASSIGNED_SO_2>(item);
        }
        public static List<V_ART_ASSIGNED_SO_2> V_ART_ASSIGNED_SO(List<V_ART_ASSIGNED_SO> item)
        {
            return AutoMapper.Mapper.Map<List<V_ART_ASSIGNED_SO_2>>(item);
        }

        public static ART_M_STEP_ARTWORK ART_M_STEP_ARTWORK(ART_M_STEP_ARTWORK_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_STEP_ARTWORK>(item);
        }
        public static ART_M_STEP_ARTWORK_2 ART_M_STEP_ARTWORK(ART_M_STEP_ARTWORK item)
        {
            return AutoMapper.Mapper.Map<ART_M_STEP_ARTWORK_2>(item);
        }
        public static List<ART_M_STEP_ARTWORK_2> ART_M_STEP_ARTWORK(List<ART_M_STEP_ARTWORK> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_STEP_ARTWORK_2>>(item);
        }

        public static XECM_M_VENDOR XECM_M_VENDOR(XECM_M_VENDOR_2 item)
        {
            return AutoMapper.Mapper.Map<XECM_M_VENDOR>(item);
        }
        public static XECM_M_VENDOR_2 XECM_M_VENDOR(XECM_M_VENDOR item)
        {
            return AutoMapper.Mapper.Map<XECM_M_VENDOR_2>(item);
        }
        public static List<XECM_M_VENDOR_2> XECM_M_VENDOR(List<XECM_M_VENDOR> item)
        {
            return AutoMapper.Mapper.Map<List<XECM_M_VENDOR_2>>(item);
        }

        public static XECM_M_CUSTOMER XECM_M_CUSTOMER(XECM_M_CUSTOMER_2 item)
        {
            return AutoMapper.Mapper.Map<XECM_M_CUSTOMER>(item);
        }
        public static XECM_M_CUSTOMER_2 XECM_M_CUSTOMER(XECM_M_CUSTOMER item)
        {
            return AutoMapper.Mapper.Map<XECM_M_CUSTOMER_2>(item);
        }
        public static List<XECM_M_CUSTOMER_2> XECM_M_CUSTOMER(List<XECM_M_CUSTOMER> item)
        {
            return AutoMapper.Mapper.Map<List<XECM_M_CUSTOMER_2>>(item);
        }

        public static ART_M_USER ART_M_USER(ART_M_USER_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_USER>(item);
        }
        public static ART_M_USER_2 ART_M_USER(ART_M_USER item)
        {
            return AutoMapper.Mapper.Map<ART_M_USER_2>(item);
        }
        public static List<ART_M_USER_2> ART_M_USER(List<ART_M_USER> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_USER_2>>(item);
        }
        public static ART_M_USER_VENDOR ART_M_USER_VENDOR(ART_M_USER_VENDOR_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_USER_VENDOR>(item);
        }
        public static ART_M_USER_VENDOR_2 ART_M_USER_VENDOR(ART_M_USER_VENDOR item)
        {
            return AutoMapper.Mapper.Map<ART_M_USER_VENDOR_2>(item);
        }
        public static List<ART_M_USER_VENDOR_2> ART_M_USER_VENDOR(List<ART_M_USER_VENDOR> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_USER_VENDOR_2>>(item);
        }

        public static ART_WF_MOCKUP_CHECK_LIST ART_WF_MOCKUP_CHECK_LIST(ART_WF_MOCKUP_CHECK_LIST_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST>(item);
        }
        public static ART_WF_MOCKUP_CHECK_LIST_2 ART_WF_MOCKUP_CHECK_LIST(ART_WF_MOCKUP_CHECK_LIST item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST_2>(item);
        }
        public static List<ART_WF_MOCKUP_CHECK_LIST_2> ART_WF_MOCKUP_CHECK_LIST(List<ART_WF_MOCKUP_CHECK_LIST> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_CHECK_LIST_2>>(item);
        }
        public static ART_WF_MOCKUP_CHECK_LIST_COUNTRY ART_WF_MOCKUP_CHECK_LIST_COUNTRY(ART_WF_MOCKUP_CHECK_LIST_COUNTRY_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST_COUNTRY>(item);
        }
        public static ART_WF_MOCKUP_CHECK_LIST_COUNTRY_2 ART_WF_MOCKUP_CHECK_LIST_COUNTRY(ART_WF_MOCKUP_CHECK_LIST_COUNTRY item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST_COUNTRY_2>(item);
        }
        public static List<ART_WF_MOCKUP_CHECK_LIST_COUNTRY_2> ART_WF_MOCKUP_CHECK_LIST_COUNTRY(List<ART_WF_MOCKUP_CHECK_LIST_COUNTRY> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_CHECK_LIST_COUNTRY_2>>(item);
        }
        public static ART_WF_MOCKUP_CHECK_LIST_ITEM ART_WF_MOCKUP_CHECK_LIST_ITEM(ART_WF_MOCKUP_CHECK_LIST_ITEM_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST_ITEM>(item);
        }
        public static ART_WF_MOCKUP_CHECK_LIST_ITEM_2 ART_WF_MOCKUP_CHECK_LIST_ITEM(ART_WF_MOCKUP_CHECK_LIST_ITEM item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST_ITEM_2>(item);
        }
        public static List<ART_WF_MOCKUP_CHECK_LIST_ITEM_2> ART_WF_MOCKUP_CHECK_LIST_ITEM(List<ART_WF_MOCKUP_CHECK_LIST_ITEM> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_CHECK_LIST_ITEM_2>>(item);
        }
        public static ART_WF_MOCKUP_CHECK_LIST_PRODUCT ART_WF_MOCKUP_CHECK_LIST_PRODUCT(ART_WF_MOCKUP_CHECK_LIST_PRODUCT_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST_PRODUCT>(item);
        }
        public static ART_WF_MOCKUP_CHECK_LIST_PRODUCT_2 ART_WF_MOCKUP_CHECK_LIST_PRODUCT(ART_WF_MOCKUP_CHECK_LIST_PRODUCT item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST_PRODUCT_2>(item);
        }
        public static List<ART_WF_MOCKUP_CHECK_LIST_PRODUCT_2> ART_WF_MOCKUP_CHECK_LIST_PRODUCT(List<ART_WF_MOCKUP_CHECK_LIST_PRODUCT> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_CHECK_LIST_PRODUCT_2>>(item);
        }
        public static ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT(ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT>(item);
        }
        public static ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_2 ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT(ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_2>(item);
        }
        public static List<ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_2> ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT(List<ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_CHECK_LIST_PRODUCTION_PLANT_2>>(item);
        }
        public static ART_WF_MOCKUP_CHECK_LIST_REFERENCE ART_WF_MOCKUP_CHECK_LIST_REFERENCE(ART_WF_MOCKUP_CHECK_LIST_REFERENCE_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST_REFERENCE>(item);
        }
        public static ART_WF_MOCKUP_CHECK_LIST_REFERENCE_2 ART_WF_MOCKUP_CHECK_LIST_REFERENCE(ART_WF_MOCKUP_CHECK_LIST_REFERENCE item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST_REFERENCE_2>(item);
        }
        public static List<ART_WF_MOCKUP_CHECK_LIST_REFERENCE_2> ART_WF_MOCKUP_CHECK_LIST_REFERENCE(List<ART_WF_MOCKUP_CHECK_LIST_REFERENCE> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_CHECK_LIST_REFERENCE_2>>(item);
        }
        public static ART_WF_MOCKUP_PROCESS ART_WF_MOCKUP_PROCESS(ART_WF_MOCKUP_PROCESS_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_2 ART_WF_MOCKUP_PROCESS(ART_WF_MOCKUP_PROCESS item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_2> ART_WF_MOCKUP_PROCESS(List<ART_WF_MOCKUP_PROCESS> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_2>>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_PG ART_WF_MOCKUP_PROCESS_PG(ART_WF_MOCKUP_PROCESS_PG_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_PG>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_PG_2 ART_WF_MOCKUP_PROCESS_PG(ART_WF_MOCKUP_PROCESS_PG item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_PG_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_PG_2> ART_WF_MOCKUP_PROCESS_PG(List<ART_WF_MOCKUP_PROCESS_PG> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_PG_2>>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2 ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2> ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2>>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2 ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2> ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG(List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_2>>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_PLANNING ART_WF_MOCKUP_PROCESS_PLANNING(ART_WF_MOCKUP_PROCESS_PLANNING_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_PLANNING>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_PLANNING_2 ART_WF_MOCKUP_PROCESS_PLANNING(ART_WF_MOCKUP_PROCESS_PLANNING item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_PLANNING_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_PLANNING_2> ART_WF_MOCKUP_PROCESS_PLANNING(List<ART_WF_MOCKUP_PROCESS_PLANNING> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_PLANNING_2>>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_RD ART_WF_MOCKUP_PROCESS_RD(ART_WF_MOCKUP_PROCESS_RD_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_RD>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_RD_2 ART_WF_MOCKUP_PROCESS_RD(ART_WF_MOCKUP_PROCESS_RD item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_RD_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_RD_2> ART_WF_MOCKUP_PROCESS_RD(List<ART_WF_MOCKUP_PROCESS_RD> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_RD_2>>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_WAREHOUSE ART_WF_MOCKUP_PROCESS_WAREHOUSE(ART_WF_MOCKUP_PROCESS_WAREHOUSE_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_WAREHOUSE>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_WAREHOUSE_2 ART_WF_MOCKUP_PROCESS_WAREHOUSE(ART_WF_MOCKUP_PROCESS_WAREHOUSE item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_WAREHOUSE_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_WAREHOUSE_2> ART_WF_MOCKUP_PROCESS_WAREHOUSE(List<ART_WF_MOCKUP_PROCESS_WAREHOUSE> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_WAREHOUSE_2>>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG ART_WF_MOCKUP_PROCESS_WAREHOUSE_SENDTO(ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2 ART_WF_MOCKUP_PROCESS_WAREHOUSE_SENDTO(ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2> ART_WF_MOCKUP_PROCESS_WAREHOUSE_SENDTO(List<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2>>(item);
        }

        public static ART_M_PERMISSION ART_M_PERMISSION(ART_M_PERMISSION_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_PERMISSION>(item);
        }
        public static ART_M_PERMISSION_2 ART_M_PERMISSION(ART_M_PERMISSION item)
        {
            return AutoMapper.Mapper.Map<ART_M_PERMISSION_2>(item);
        }
        public static List<ART_M_PERMISSION_2> ART_M_PERMISSION(List<ART_M_PERMISSION> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_PERMISSION_2>>(item);
        }

        public static ART_M_EMAIL_TEMPLATE ART_M_EMAIL_TEMPLATE(ART_M_EMAIL_TEMPLATE_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_EMAIL_TEMPLATE>(item);
        }
        public static ART_M_EMAIL_TEMPLATE_2 ART_M_EMAIL_TEMPLATE(ART_M_EMAIL_TEMPLATE item)
        {
            return AutoMapper.Mapper.Map<ART_M_EMAIL_TEMPLATE_2>(item);
        }
        public static List<ART_M_EMAIL_TEMPLATE_2> ART_M_EMAIL_TEMPLATE(List<ART_M_EMAIL_TEMPLATE> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_EMAIL_TEMPLATE_2>>(item);
        }

        public static SAP_M_BRAND SAP_M_BRAND(SAP_M_BRAND_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_BRAND>(item);
        }
        public static SAP_M_BRAND_2 SAP_M_BRAND(SAP_M_BRAND item)
        {
            return AutoMapper.Mapper.Map<SAP_M_BRAND_2>(item);
        }
        public static List<SAP_M_BRAND_2> SAP_M_BRAND(List<SAP_M_BRAND> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_BRAND_2>>(item);
        }

        public static ART_M_RECEIVE_EMAIL_NEW_CUSTOMER ART_M_RECEIVE_EMAIL_NEW_CUSTOMER(ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_RECEIVE_EMAIL_NEW_CUSTOMER>(item);
        }
        public static ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_2 ART_M_RECEIVE_EMAIL_NEW_CUSTOMER(ART_M_RECEIVE_EMAIL_NEW_CUSTOMER item)
        {
            return AutoMapper.Mapper.Map<ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_2>(item);
        }
        public static List<ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_2> ART_M_RECEIVE_EMAIL_NEW_CUSTOMER(List<ART_M_RECEIVE_EMAIL_NEW_CUSTOMER> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_RECEIVE_EMAIL_NEW_CUSTOMER_2>>(item);
        }

        //public static SAP_M_BRAND_OEM SAP_M_BRAND_OEM(SAP_M_BRAND_OEM_2 item)
        //{
        //    return AutoMapper.Mapper.Map<SAP_M_BRAND_OEM>(item);
        //}
        //public static SAP_M_BRAND_OEM_2 SAP_M_BRAND_OEM(SAP_M_BRAND_OEM item)
        //{
        //    return AutoMapper.Mapper.Map<SAP_M_BRAND_OEM_2>(item);
        //}
        //public static List<SAP_M_BRAND_OEM_2> SAP_M_BRAND_OEM(List<SAP_M_BRAND_OEM> item)
        //{
        //    return AutoMapper.Mapper.Map<List<SAP_M_BRAND_OEM_2>>(item);
        //}
        public static SAP_M_CHARACTERISTIC SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_CHARACTERISTIC>(item);
        }
        public static SAP_M_CHARACTERISTIC_2 SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC item)
        {
            return AutoMapper.Mapper.Map<SAP_M_CHARACTERISTIC_2>(item);
        }
        public static List<SAP_M_CHARACTERISTIC_2> SAP_M_CHARACTERISTIC(List<SAP_M_CHARACTERISTIC> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_CHARACTERISTIC_2>>(item);
        }
        public static SAP_M_CHARACTERISTIC_ITEM SAP_M_CHARACTERISTIC_ITEM(SAP_M_CHARACTERISTIC_ITEM_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_CHARACTERISTIC_ITEM>(item);
        }
        public static SAP_M_CHARACTERISTIC_ITEM_2 SAP_M_CHARACTERISTIC_ITEM(SAP_M_CHARACTERISTIC_ITEM item)
        {
            return AutoMapper.Mapper.Map<SAP_M_CHARACTERISTIC_ITEM_2>(item);
        }
        public static List<SAP_M_CHARACTERISTIC_ITEM_2> SAP_M_CHARACTERISTIC_ITEM(List<SAP_M_CHARACTERISTIC_ITEM> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_CHARACTERISTIC_ITEM_2>>(item);
        }
        public static SAP_M_COMPANY SAP_M_COMPANY(SAP_M_COMPANY_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_COMPANY>(item);
        }
        public static SAP_M_COMPANY_2 SAP_M_COMPANY(SAP_M_COMPANY item)
        {
            return AutoMapper.Mapper.Map<SAP_M_COMPANY_2>(item);
        }
        public static List<SAP_M_COMPANY_2> SAP_M_COMPANY(List<SAP_M_COMPANY> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_COMPANY_2>>(item);
        }
        public static SAP_M_COUNTRY SAP_M_COUNTRY(SAP_M_COUNTRY_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_COUNTRY>(item);
        }
        public static SAP_M_COUNTRY_2 SAP_M_COUNTRY(SAP_M_COUNTRY item)
        {
            return AutoMapper.Mapper.Map<SAP_M_COUNTRY_2>(item);
        }
        public static List<SAP_M_COUNTRY_2> SAP_M_COUNTRY(List<SAP_M_COUNTRY> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_COUNTRY_2>>(item);
        }
        public static XECM_M_CUSTOMER SAP_M_CUSTOMER(XECM_M_CUSTOMER_2 item)
        {
            return AutoMapper.Mapper.Map<XECM_M_CUSTOMER>(item);
        }
        public static XECM_M_CUSTOMER_2 SAP_M_CUSTOMER(XECM_M_CUSTOMER item)
        {
            return AutoMapper.Mapper.Map<XECM_M_CUSTOMER_2>(item);
        }
        public static List<XECM_M_CUSTOMER_2> SAP_M_CUSTOMER(List<XECM_M_CUSTOMER> item)
        {
            return AutoMapper.Mapper.Map<List<XECM_M_CUSTOMER_2>>(item);
        }
        public static SAP_M_PLANT SAP_M_PLANT(SAP_M_PLANT_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_PLANT>(item);
        }
        public static SAP_M_PLANT_2 SAP_M_PLANT(SAP_M_PLANT item)
        {
            return AutoMapper.Mapper.Map<SAP_M_PLANT_2>(item);
        }
        public static List<SAP_M_PLANT_2> SAP_M_PLANT(List<SAP_M_PLANT> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_PLANT_2>>(item);
        }
        public static SAP_M_TYPE_OF_PRODUCT SAP_M_TYPE_OF_PRODUCT(SAP_M_TYPE_OF_PRODUCT_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_TYPE_OF_PRODUCT>(item);
        }
        public static SAP_M_TYPE_OF_PRODUCT_2 SAP_M_TYPE_OF_PRODUCT(SAP_M_TYPE_OF_PRODUCT item)
        {
            return AutoMapper.Mapper.Map<SAP_M_TYPE_OF_PRODUCT_2>(item);
        }
        public static List<SAP_M_TYPE_OF_PRODUCT_2> SAP_M_TYPE_OF_PRODUCT(List<SAP_M_TYPE_OF_PRODUCT> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_TYPE_OF_PRODUCT_2>>(item);
        }
        public static XECM_M_VENDOR SAP_M_VENDOR(XECM_M_VENDOR_2 item)
        {
            return AutoMapper.Mapper.Map<XECM_M_VENDOR>(item);
        }
        public static XECM_M_VENDOR_2 SAP_M_VENDOR(XECM_M_VENDOR item)
        {
            return AutoMapper.Mapper.Map<XECM_M_VENDOR_2>(item);
        }
        public static List<XECM_M_VENDOR_2> SAP_M_VENDOR(List<XECM_M_VENDOR> item)
        {
            return AutoMapper.Mapper.Map<List<XECM_M_VENDOR_2>>(item);
        }
        public static V_ART_WF_DASHBOARD V_ART_WF_DASHBOARD(V_ART_WF_DASHBOARD_2 item)
        {
            return AutoMapper.Mapper.Map<V_ART_WF_DASHBOARD>(item);
        }
        public static V_ART_WF_DASHBOARD_2 V_ART_WF_DASHBOARD(V_ART_WF_DASHBOARD item)
        {
            return AutoMapper.Mapper.Map<V_ART_WF_DASHBOARD_2>(item);
        }
        public static List<V_ART_WF_DASHBOARD_2> V_ART_WF_DASHBOARD(List<V_ART_WF_DASHBOARD> item)
        {
            return AutoMapper.Mapper.Map<List<V_ART_WF_DASHBOARD_2>>(item);
        }

        public static V_ART_WF_DASHBOARD_ARTWORK V_ART_WF_DASHBOARD_ARTWORK(V_ART_WF_DASHBOARD_ARTWORK_2 item)
        {
            return AutoMapper.Mapper.Map<V_ART_WF_DASHBOARD_ARTWORK>(item);
        }
        public static V_ART_WF_DASHBOARD_ARTWORK_2 V_ART_WF_DASHBOARD_ARTWORK(V_ART_WF_DASHBOARD_ARTWORK item)
        {
            return AutoMapper.Mapper.Map<V_ART_WF_DASHBOARD_ARTWORK_2>(item);
        }
        public static List<V_ART_WF_DASHBOARD_ARTWORK_2> V_ART_WF_DASHBOARD_ARTWORK(List<V_ART_WF_DASHBOARD_ARTWORK> item)
        {
            return AutoMapper.Mapper.Map<List<V_ART_WF_DASHBOARD_ARTWORK_2>>(item);
        }

        public static ART_M_VENDOR_MATGROUP ART_M_VENDOR_MATGROUP(ART_M_VENDOR_MATGROUP_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_VENDOR_MATGROUP>(item);
        }
        public static ART_M_VENDOR_MATGROUP_2 ART_M_VENDOR_MATGROUP(ART_M_VENDOR_MATGROUP item)
        {
            return AutoMapper.Mapper.Map<ART_M_VENDOR_MATGROUP_2>(item);
        }
        public static List<ART_M_VENDOR_MATGROUP_2> ART_M_VENDOR_MATGROUP(List<ART_M_VENDOR_MATGROUP> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_VENDOR_MATGROUP_2>>(item);
        }

        public static ART_WF_MOCKUP_ATTACHMENT ART_WF_MOCKUP_ATTACHMENT(ART_WF_MOCKUP_ATTACHMENT_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_ATTACHMENT>(item);
        }
        public static ART_WF_MOCKUP_ATTACHMENT_2 ART_WF_MOCKUP_ATTACHMENT(ART_WF_MOCKUP_ATTACHMENT item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_ATTACHMENT_2>(item);
        }
        public static List<ART_WF_MOCKUP_ATTACHMENT_2> ART_WF_MOCKUP_ATTACHMENT(List<ART_WF_MOCKUP_ATTACHMENT> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_ATTACHMENT_2>>(item);
        }

        public static SAP_M_3P SAP_M_3P(SAP_M_3P_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_3P>(item);
        }
        public static SAP_M_3P_2 SAP_M_3P(SAP_M_3P item)
        {
            return AutoMapper.Mapper.Map<SAP_M_3P_2>(item);
        }
        public static List<SAP_M_3P_2> SAP_M_3P(List<SAP_M_3P> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_3P_2>>(item);
        }

        public static ART_SYS_LOG ART_SYS_LOG(ART_SYS_LOG_2 item)
        {
            return AutoMapper.Mapper.Map<ART_SYS_LOG>(item);
        }
        public static ART_SYS_LOG_2 ART_SYS_LOG(ART_SYS_LOG item)
        {
            return AutoMapper.Mapper.Map<ART_SYS_LOG_2>(item);
        }
        public static List<ART_SYS_LOG_2> ART_SYS_LOG(List<ART_SYS_LOG> item)
        {
            return AutoMapper.Mapper.Map<List<ART_SYS_LOG_2>>(item);
        }

        public static ART_WF_MOCKUP_PROCESS_VENDOR ART_WF_MOCKUP_PROCESS_VENDOR(ART_WF_MOCKUP_PROCESS_VENDOR_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_VENDOR>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_VENDOR_2 ART_WF_MOCKUP_PROCESS_VENDOR(ART_WF_MOCKUP_PROCESS_VENDOR item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_VENDOR_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_VENDOR_2> ART_WF_MOCKUP_PROCESS_VENDOR(List<ART_WF_MOCKUP_PROCESS_VENDOR> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_VENDOR_2>>(item);
        }

        public static ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG(ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_2 ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG(ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_2> ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG(List<ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_VENDOR_BY_PG_2>>(item);
        }

        public static ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG(ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_2 ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG(ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_2> ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG(List<ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_CUSTOMER_BY_PG_2>>(item);
        }



        public static ART_WF_MOCKUP_PROCESS_CUSTOMER ART_WF_MOCKUP_PROCESS_CUSTOMER(ART_WF_MOCKUP_PROCESS_CUSTOMER_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_CUSTOMER>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_CUSTOMER_2 ART_WF_MOCKUP_PROCESS_CUSTOMER(ART_WF_MOCKUP_PROCESS_CUSTOMER item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_CUSTOMER_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_CUSTOMER_2> ART_WF_MOCKUP_PROCESS_CUSTOMER(List<ART_WF_MOCKUP_PROCESS_CUSTOMER> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_CUSTOMER_2>>(item);
        }

        public static ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG(ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2 ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG(ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2> ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG(List<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_WAREHOUSE_BY_PG_2>>(item);
        }

        public static ART_WF_DELEGATE ART_WF_DELEGATE(ART_WF_DELEGATE_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_DELEGATE>(item);
        }
        public static ART_WF_DELEGATE_2 ART_WF_DELEGATE(ART_WF_DELEGATE item)
        {
            return AutoMapper.Mapper.Map<ART_WF_DELEGATE_2>(item);
        }
        public static List<ART_WF_DELEGATE_2> ART_WF_DELEGATE(List<ART_WF_DELEGATE> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_DELEGATE_2>>(item);
        }

        public static ART_WF_MOCKUP_PROCESS_RD_BY_PG ART_WF_MOCKUP_PROCESS_RD_BY_PG(ART_WF_MOCKUP_PROCESS_RD_BY_PG_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_RD_BY_PG>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_RD_BY_PG_2 ART_WF_MOCKUP_PROCESS_RD_BY_PG(ART_WF_MOCKUP_PROCESS_RD_BY_PG item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_RD_BY_PG_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_RD_BY_PG_2> ART_WF_MOCKUP_PROCESS_RD_BY_PG(List<ART_WF_MOCKUP_PROCESS_RD_BY_PG> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_RD_BY_PG_2>>(item);
        }

        public static ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG(ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_2 ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG(ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_2> ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG(List<ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_PLANNING_BY_PG_2>>(item);
        }

        public static ART_M_TEMPLATE ART_M_TEMPLATE(ART_M_TEMPLATE_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_TEMPLATE>(item);
        }
        public static PP_MODEL2 PPVIEW_TEMPLATE(PP_MODEL item)
        {
            return AutoMapper.Mapper.Map<PP_MODEL2>(item);
        }
        public static ART_M_TEMPLATE_2 ART_M_TEMPLATE(ART_M_TEMPLATE item)
        {
            return AutoMapper.Mapper.Map<ART_M_TEMPLATE_2>(item);
        }
        public static List<ART_M_TEMPLATE_2> ART_M_TEMPLATE(List<ART_M_TEMPLATE> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_TEMPLATE_2>>(item);
        }

        public static ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR(ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_2 ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR(ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_2> ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR(List<ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_PG_SELECT_VENDOR_2>>(item);
        }

        public static ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD(ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD>(item);
        }
        public static ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_2 ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD(ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_2>(item);
        }
        public static List<ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_2> ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD(List<ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_PROCESS_APPROVE_MATCHBOARD_2>>(item);
        }


        public static SAP_M_2P SAP_M_2P(SAP_M_2P_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_2P>(item);
        }
        public static SAP_M_2P_2 SAP_M_2P(SAP_M_2P item)
        {
            return AutoMapper.Mapper.Map<SAP_M_2P_2>(item);
        }
        public static List<SAP_M_2P_2> SAP_M_2P(List<SAP_M_2P> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_2P_2>>(item);
        }

        public static XECM_M_PRODUCT XECM_M_PRODUCT(XECM_M_PRODUCT_2 item)
        {
            return AutoMapper.Mapper.Map<XECM_M_PRODUCT>(item);
        }
        public static XECM_M_PRODUCT_2 XECM_M_PRODUCT(XECM_M_PRODUCT item)
        {
            return AutoMapper.Mapper.Map<XECM_M_PRODUCT_2>(item);
        }
        public static List<XECM_M_PRODUCT_2> XECM_M_PRODUCT(List<XECM_M_PRODUCT> item)
        {
            return AutoMapper.Mapper.Map<List<XECM_M_PRODUCT_2>>(item);
        }

        //public static XECM_M_REFERENCE XECM_M_REFERENCE(XECM_M_REFERENCE_2 item)
        //{
        //    return AutoMapper.Mapper.Map<XECM_M_REFERENCE>(item);
        //}
        //public static XECM_M_REFERENCE_2 XECM_M_REFERENCE(XECM_M_REFERENCE item)
        //{
        //    return AutoMapper.Mapper.Map<XECM_M_REFERENCE_2>(item);
        //}
        //public static List<XECM_M_REFERENCE_2> XECM_M_REFERENCE(List<XECM_M_REFERENCE> item)
        //{
        //    return AutoMapper.Mapper.Map<List<XECM_M_REFERENCE_2>>(item);
        //}

        public static ART_M_USER_ROLE ART_M_USER_ROLE(ART_M_USER_ROLE_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_USER_ROLE>(item);
        }
        public static ART_M_USER_ROLE_2 ART_M_USER_ROLE(ART_M_USER_ROLE item)
        {
            return AutoMapper.Mapper.Map<ART_M_USER_ROLE_2>(item);
        }
        public static List<ART_M_USER_ROLE_2> ART_M_USER_ROLE(List<ART_M_USER_ROLE> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_USER_ROLE_2>>(item);
        }

        public static ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER(ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER>(item);
        }
        public static ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_2 ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER(ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_2>(item);
        }
        public static List<ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_2> ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER(List<ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_CHECK_LIST_CUSTOMER_OTHER_2>>(item);
        }

        public static ART_M_DECISION_REASON ART_M_DECISION_REASON(ART_M_DECISION_REASON_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_DECISION_REASON>(item);
        }
        public static ART_M_DECISION_REASON_2 ART_M_DECISION_REASON(ART_M_DECISION_REASON item)
        {
            return AutoMapper.Mapper.Map<ART_M_DECISION_REASON_2>(item);
        }
        public static List<ART_M_DECISION_REASON_2> ART_M_DECISION_REASON(List<ART_M_DECISION_REASON> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_DECISION_REASON_2>>(item);
        }

        public static ART_WF_ARTWORK_REQUEST ART_WF_ARTWORK_REQUEST(ART_WF_ARTWORK_REQUEST_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST>(item);
        }
        public static ART_WF_ARTWORK_REQUEST_2 ART_WF_ARTWORK_REQUEST(ART_WF_ARTWORK_REQUEST item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_2>(item);
        }
        public static List<ART_WF_ARTWORK_REQUEST_2> ART_WF_ARTWORK_REQUEST(List<ART_WF_ARTWORK_REQUEST> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_REQUEST_2>>(item);
        }

        public static ART_WF_ARTWORK_REQUEST_ITEM ART_WF_ARTWORK_REQUEST_ITEM(ART_WF_ARTWORK_REQUEST_ITEM_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_ITEM>(item);
        }
        public static ART_WF_ARTWORK_REQUEST_ITEM_2 ART_WF_ARTWORK_REQUEST_ITEM(ART_WF_ARTWORK_REQUEST_ITEM item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_ITEM_2>(item);
        }
        public static List<ART_WF_ARTWORK_REQUEST_ITEM_2> ART_WF_ARTWORK_REQUEST_ITEM(List<ART_WF_ARTWORK_REQUEST_ITEM> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_REQUEST_ITEM_2>>(item);
        }

        public static ART_WF_ARTWORK_REQUEST_RECIPIENT ART_WF_ARTWORK_REQUEST_MARKETING(ART_WF_ARTWORK_REQUEST_RECIPIENT_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_RECIPIENT>(item);
        }
        public static ART_WF_ARTWORK_REQUEST_RECIPIENT_2 ART_WF_ARTWORK_REQUEST_MARKETING(ART_WF_ARTWORK_REQUEST_RECIPIENT item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_RECIPIENT_2>(item);
        }
        public static List<ART_WF_ARTWORK_REQUEST_RECIPIENT_2> ART_WF_ARTWORK_REQUEST_MARKETING(List<ART_WF_ARTWORK_REQUEST_RECIPIENT> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_REQUEST_RECIPIENT_2>>(item);
        }


        public static ART_WF_MOCKUP_CHECK_LIST_PG ART_WF_MOCKUP_CHECK_LIST_PG(ART_WF_MOCKUP_CHECK_LIST_PG_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST_PG>(item);
        }
        public static ART_WF_MOCKUP_CHECK_LIST_PG_2 ART_WF_MOCKUP_CHECK_LIST_PG(ART_WF_MOCKUP_CHECK_LIST_PG item)
        {
            return AutoMapper.Mapper.Map<ART_WF_MOCKUP_CHECK_LIST_PG_2>(item);
        }
        public static List<ART_WF_MOCKUP_CHECK_LIST_PG_2> ART_WF_MOCKUP_CHECK_LIST_PG(List<ART_WF_MOCKUP_CHECK_LIST_PG> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_MOCKUP_CHECK_LIST_PG_2>>(item);
        }

        public static ART_M_USER_TYPE_OF_PRODUCT ART_M_USER_TYPE_OF_PRODUCT(ART_M_USER_TYPE_OF_PRODUCT_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_USER_TYPE_OF_PRODUCT>(item);
        }
        public static ART_M_USER_TYPE_OF_PRODUCT_2 ART_M_USER_TYPE_OF_PRODUCT(ART_M_USER_TYPE_OF_PRODUCT item)
        {
            return AutoMapper.Mapper.Map<ART_M_USER_TYPE_OF_PRODUCT_2>(item);
        }
        public static List<ART_M_USER_TYPE_OF_PRODUCT_2> ART_M_USER_TYPE_OF_PRODUCT(List<ART_M_USER_TYPE_OF_PRODUCT> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_USER_TYPE_OF_PRODUCT_2>>(item);
        }

        public static ART_M_USER_COMPANY ART_M_USER_COMPANY(ART_M_USER_COMPANY_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_USER_COMPANY>(item);
        }
        public static ART_M_USER_COMPANY_2 ART_M_USER_COMPANY(ART_M_USER_COMPANY item)
        {
            return AutoMapper.Mapper.Map<ART_M_USER_COMPANY_2>(item);
        }
        public static List<ART_M_USER_COMPANY_2> ART_M_USER_COMPANY(List<ART_M_USER_COMPANY> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_USER_COMPANY_2>>(item);
        }

        public static ART_M_USER_CUSTOMER ART_M_USER_CUSTOMER(ART_M_USER_CUSTOMER_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_USER_CUSTOMER>(item);
        }
        public static ART_M_USER_CUSTOMER_2 ART_M_USER_CUSTOMER(ART_M_USER_CUSTOMER item)
        {
            return AutoMapper.Mapper.Map<ART_M_USER_CUSTOMER_2>(item);
        }
        public static List<ART_M_USER_CUSTOMER_2> ART_M_USER_CUSTOMER(List<ART_M_USER_CUSTOMER> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_USER_CUSTOMER_2>>(item);
        }

        public static ART_M_USER_UPPER_LEVEL ART_M_USER_UPPER_LEVEL(ART_M_USER_UPPER_LEVEL_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_USER_UPPER_LEVEL>(item);
        }
        public static ART_M_USER_UPPER_LEVEL_2 ART_M_USER_UPPER_LEVEL(ART_M_USER_UPPER_LEVEL item)
        {
            return AutoMapper.Mapper.Map<ART_M_USER_UPPER_LEVEL_2>(item);
        }
        public static List<ART_M_USER_UPPER_LEVEL_2> ART_M_USER_UPPER_LEVEL(List<ART_M_USER_UPPER_LEVEL> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_USER_UPPER_LEVEL_2>>(item);
        }

        public static ART_WF_ARTWORK_REQUEST_COUNTRY ART_WF_ARTWORK_REQUEST_COUNTRY(ART_WF_ARTWORK_REQUEST_COUNTRY_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_COUNTRY>(item);
        }
        public static ART_WF_ARTWORK_REQUEST_COUNTRY_2 ART_WF_ARTWORK_REQUEST_COUNTRY(ART_WF_ARTWORK_REQUEST_COUNTRY item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_COUNTRY_2>(item);
        }
        public static List<ART_WF_ARTWORK_REQUEST_COUNTRY_2> ART_WF_ARTWORK_REQUEST_COUNTRY(List<ART_WF_ARTWORK_REQUEST_COUNTRY> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_REQUEST_COUNTRY_2>>(item);
        }

        public static ART_WF_ARTWORK_REQUEST_PRODUCT ART_WF_ARTWORK_REQUEST_PRODUCT(ART_WF_ARTWORK_REQUEST_PRODUCT_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_PRODUCT>(item);
        }
        public static ART_WF_ARTWORK_REQUEST_PRODUCT_2 ART_WF_ARTWORK_REQUEST_PRODUCT(ART_WF_ARTWORK_REQUEST_PRODUCT item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_PRODUCT_2>(item);
        }
        public static List<ART_WF_ARTWORK_REQUEST_PRODUCT_2> ART_WF_ARTWORK_REQUEST_PRODUCT(List<ART_WF_ARTWORK_REQUEST_PRODUCT> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_REQUEST_PRODUCT_2>>(item);
        }

        public static V_SAP_SALES_ORDER V_SAP_SALES_ORDER(V_SAP_SALES_ORDER_2 item)
        {
            return AutoMapper.Mapper.Map<V_SAP_SALES_ORDER>(item);
        }
        public static V_SAP_SALES_ORDER_2 V_SAP_SALES_ORDER(V_SAP_SALES_ORDER item)
        {
            return AutoMapper.Mapper.Map<V_SAP_SALES_ORDER_2>(item);
        }
        public static List<V_SAP_SALES_ORDER_2> V_SAP_SALES_ORDER(List<V_SAP_SALES_ORDER> item)
        {
            return AutoMapper.Mapper.Map<List<V_SAP_SALES_ORDER_2>>(item);
        }

        public static V_SAP_SALES_ORDER_ALL V_SAP_SALES_ORDER_ALL(V_SAP_SALES_ORDER_ALL_2 item)
        {
            return AutoMapper.Mapper.Map<V_SAP_SALES_ORDER_ALL>(item);
        }
        public static V_SAP_SALES_ORDER_ALL_2 V_SAP_SALES_ORDER_ALL(V_SAP_SALES_ORDER_ALL item)
        {
            return AutoMapper.Mapper.Map<V_SAP_SALES_ORDER_ALL_2>(item);
        }
        public static List<V_SAP_SALES_ORDER_ALL_2> V_SAP_SALES_ORDER_ALL(List<V_SAP_SALES_ORDER_ALL> item)
        {
            return AutoMapper.Mapper.Map<List<V_SAP_SALES_ORDER_ALL_2>>(item);
        }

        public static V_ART_WF_ALL_PROCESS V_ART_WF_ALL_PROCESS(V_ART_WF_ALL_PROCESS_2 item)
        {
            return AutoMapper.Mapper.Map<V_ART_WF_ALL_PROCESS>(item);
        }
        public static V_ART_WF_ALL_PROCESS_2 V_ART_WF_ALL_PROCESS(V_ART_WF_ALL_PROCESS item)
        {
            return AutoMapper.Mapper.Map<V_ART_WF_ALL_PROCESS_2>(item);
        }
        public static List<V_ART_WF_ALL_PROCESS_2> V_ART_WF_ALL_PROCESS(List<V_ART_WF_ALL_PROCESS> item)
        {
            return AutoMapper.Mapper.Map<List<V_ART_WF_ALL_PROCESS_2>>(item);
        }

        public static ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT(ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT>(item);
        }
        public static ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2 ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT(ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2>(item);
        }
        public static List<ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2> ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT(List<ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_REQUEST_PRODUCTION_PLANT_2>>(item);
        }

        public static ART_WF_ARTWORK_REQUEST_REFERENCE ART_WF_ARTWORK_REQUEST_REFERENCE(ART_WF_ARTWORK_REQUEST_REFERENCE_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_REFERENCE>(item);
        }
        public static ART_WF_ARTWORK_REQUEST_REFERENCE_2 ART_WF_ARTWORK_REQUEST_REFERENCE(ART_WF_ARTWORK_REQUEST_REFERENCE item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_REFERENCE_2>(item);
        }
        public static List<ART_WF_ARTWORK_REQUEST_REFERENCE_2> ART_WF_ARTWORK_REQUEST_REFERENCE(List<ART_WF_ARTWORK_REQUEST_REFERENCE> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_REQUEST_REFERENCE_2>>(item);
        }

        public static ART_WF_ARTWORK_REQUEST_SALES_ORDER ART_WF_ARTWORK_REQUEST_SALES_ORDER(ART_WF_ARTWORK_REQUEST_SALES_ORDER_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_SALES_ORDER>(item);
        }
        public static ART_WF_ARTWORK_REQUEST_SALES_ORDER_2 ART_WF_ARTWORK_REQUEST_SALES_ORDER(ART_WF_ARTWORK_REQUEST_SALES_ORDER item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_SALES_ORDER_2>(item);
        }
        public static List<ART_WF_ARTWORK_REQUEST_SALES_ORDER_2> ART_WF_ARTWORK_REQUEST_SALES_ORDER(List<ART_WF_ARTWORK_REQUEST_SALES_ORDER> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_REQUEST_SALES_ORDER_2>>(item);
        }

        public static ART_WF_ARTWORK_REQUEST_RECIPIENT ART_WF_ARTWORK_REQUEST_RECIPIENT(ART_WF_ARTWORK_REQUEST_RECIPIENT_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_RECIPIENT>(item);
        }
        public static ART_WF_ARTWORK_REQUEST_RECIPIENT_2 ART_WF_ARTWORK_REQUEST_RECIPIENT(ART_WF_ARTWORK_REQUEST_RECIPIENT item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_RECIPIENT_2>(item);
        }
        public static List<ART_WF_ARTWORK_REQUEST_RECIPIENT_2> ART_WF_ARTWORK_REQUEST_RECIPIENT(List<ART_WF_ARTWORK_REQUEST_RECIPIENT> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_REQUEST_RECIPIENT_2>>(item);
        }

        public static ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER(ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER>(item);
        }
        public static ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_2 ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER(ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_2>(item);
        }
        public static List<ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_2> ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER(List<ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_REQUEST_CUSTOMER_OTHER_2>>(item);
        }


        public static ART_M_POSITION ART_M_POSITION(ART_M_POSITION_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_POSITION>(item);
        }
        public static ART_M_POSITION_2 ART_M_POSITION(ART_M_POSITION item)
        {
            return AutoMapper.Mapper.Map<ART_M_POSITION_2>(item);
        }
        public static List<ART_M_POSITION_2> ART_M_POSITION(List<ART_M_POSITION> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_POSITION_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_PA ART_WF_ARTWORK_PROCESS_PA(ART_WF_ARTWORK_PROCESS_PA_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_PA_2 ART_WF_ARTWORK_PROCESS_PA(ART_WF_ARTWORK_PROCESS_PA item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_PA_2> ART_WF_ARTWORK_PROCESS_PA(List<ART_WF_ARTWORK_PROCESS_PA> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_PA_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_PA_PRODUCT ART_WF_ARTWORK_PROCESS_PA_PRODUCT(ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA_PRODUCT>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2 ART_WF_ARTWORK_PROCESS_PA_PRODUCT(ART_WF_ARTWORK_PROCESS_PA_PRODUCT item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2> ART_WF_ARTWORK_PROCESS_PA_PRODUCT(List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_2>>(item);
        }

        public static SAP_M_PO_COMPLETE_SO_HEADER SAP_M_PO_COMPLETE_SO_HEADER(SAP_M_PO_COMPLETE_SO_HEADER_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_PO_COMPLETE_SO_HEADER>(item);
        }
        public static SAP_M_PO_COMPLETE_SO_HEADER_2 SAP_M_PO_COMPLETE_SO_HEADER(SAP_M_PO_COMPLETE_SO_HEADER item)
        {
            return AutoMapper.Mapper.Map<SAP_M_PO_COMPLETE_SO_HEADER_2>(item);
        }
        public static List<SAP_M_PO_COMPLETE_SO_HEADER_2> SAP_M_PO_COMPLETE_SO_HEADER(List<SAP_M_PO_COMPLETE_SO_HEADER> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_PO_COMPLETE_SO_HEADER_2>>(item);
        }


        public static SAP_M_PO_COMPLETE_SO_ITEM SAP_M_PO_COMPLETE_SO_ITEM(SAP_M_PO_COMPLETE_SO_ITEM_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_PO_COMPLETE_SO_ITEM>(item);
        }
        public static SAP_M_PO_COMPLETE_SO_ITEM_2 SAP_M_PO_COMPLETE_SO_ITEM(SAP_M_PO_COMPLETE_SO_ITEM item)
        {
            return AutoMapper.Mapper.Map<SAP_M_PO_COMPLETE_SO_ITEM_2>(item);
        }
        public static List<SAP_M_PO_COMPLETE_SO_ITEM_2> SAP_M_PO_COMPLETE_SO_ITEM(List<SAP_M_PO_COMPLETE_SO_ITEM> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_PO_COMPLETE_SO_ITEM_2>>(item);
        }


        public static SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT(SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT>(item);
        }
        public static SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2 SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT(SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT item)
        {
            return AutoMapper.Mapper.Map<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2>(item);
        }
        public static List<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2> SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT(List<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS ART_WF_ARTWORK_PROCESS(ART_WF_ARTWORK_PROCESS_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_2 ART_WF_ARTWORK_PROCESS(ART_WF_ARTWORK_PROCESS item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_2> ART_WF_ARTWORK_PROCESS(List<ART_WF_ARTWORK_PROCESS> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_2>>(item);
        }


        public static ART_WF_ARTWORK_PROCESS_WH_BY_PA ART_WF_ARTWORK_PROCESS_WH_BY_PA(ART_WF_ARTWORK_PROCESS_WH_BY_PA_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_WH_BY_PA>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_WH_BY_PA_2 ART_WF_ARTWORK_PROCESS_WH_BY_PA(ART_WF_ARTWORK_PROCESS_WH_BY_PA item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_WH_BY_PA_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_WH_BY_PA_2> ART_WF_ARTWORK_PROCESS_WH_BY_PA(List<ART_WF_ARTWORK_PROCESS_WH_BY_PA> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_WH_BY_PA_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA(ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_2 ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA(ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_2> ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA(List<ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_MARKETING_BY_PA_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_MARKETING ART_WF_ARTWORK_PROCESS_MARKETING(ART_WF_ARTWORK_PROCESS_MARKETING_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_MARKETING>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_MARKETING_2 ART_WF_ARTWORK_PROCESS_MARKETING(ART_WF_ARTWORK_PROCESS_MARKETING item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_MARKETING_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_MARKETING_2> ART_WF_ARTWORK_PROCESS_MARKETING(List<ART_WF_ARTWORK_PROCESS_MARKETING> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_MARKETING_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_PG_BY_PA ART_WF_ARTWORK_PROCESS_PG_BY_PA(ART_WF_ARTWORK_PROCESS_PG_BY_PA_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PG_BY_PA>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_PG_BY_PA_2 ART_WF_ARTWORK_PROCESS_PG_BY_PA(ART_WF_ARTWORK_PROCESS_PG_BY_PA item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PG_BY_PA_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_PG_BY_PA_2> ART_WF_ARTWORK_PROCESS_PG_BY_PA(List<ART_WF_ARTWORK_PROCESS_PG_BY_PA> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_PG_BY_PA_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA(ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_2 ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA(ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_2> ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA(List<ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_PLANNING_BY_PA_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_PP_BY_PA ART_WF_ARTWORK_PROCESS_PP_BY_PA(ART_WF_ARTWORK_PROCESS_PP_BY_PA_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PP_BY_PA>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_PP_BY_PA_2 ART_WF_ARTWORK_PROCESS_PP_BY_PA(ART_WF_ARTWORK_PROCESS_PP_BY_PA item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PP_BY_PA_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_PP_BY_PA_2> ART_WF_ARTWORK_PROCESS_PP_BY_PA(List<ART_WF_ARTWORK_PROCESS_PP_BY_PA> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_PP_BY_PA_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_QC_BY_PA ART_WF_ARTWORK_PROCESS_QC_BY_PA(ART_WF_ARTWORK_PROCESS_QC_BY_PA_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_QC_BY_PA>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_QC_BY_PA_2 ART_WF_ARTWORK_PROCESS_QC_BY_PA(ART_WF_ARTWORK_PROCESS_QC_BY_PA item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_QC_BY_PA_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_QC_BY_PA_2> ART_WF_ARTWORK_PROCESS_QC_BY_PA(List<ART_WF_ARTWORK_PROCESS_QC_BY_PA> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_QC_BY_PA_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_SO_DETAIL ART_WF_ARTWORK_PROCESS_SO_DETAIL(ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_SO_DETAIL>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 ART_WF_ARTWORK_PROCESS_SO_DETAIL(ART_WF_ARTWORK_PROCESS_SO_DETAIL item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2> ART_WF_ARTWORK_PROCESS_SO_DETAIL(List<ART_WF_ARTWORK_PROCESS_SO_DETAIL> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>>(item);
        }

        public static ART_M_PRINTING_STYLE ART_M_PRINTING_STYLE(ART_M_PRINTING_STYLE_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_PRINTING_STYLE>(item);
        }
        public static ART_M_PRINTING_STYLE_2 ART_M_PRINTING_STYLE(ART_M_PRINTING_STYLE item)
        {
            return AutoMapper.Mapper.Map<ART_M_PRINTING_STYLE_2>(item);
        }
        public static List<ART_M_PRINTING_STYLE_2> ART_M_PRINTING_STYLE(List<ART_M_PRINTING_STYLE> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_PRINTING_STYLE_2>>(item);
        }


        public static ART_WF_ARTWORK_PROCESS_QC ART_WF_ARTWORK_PROCESS_QC(ART_WF_ARTWORK_PROCESS_QC_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_QC>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_QC_2 ART_WF_ARTWORK_PROCESS_QC(ART_WF_ARTWORK_PROCESS_QC item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_QC_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_QC_2> ART_WF_ARTWORK_PROCESS_QC(List<ART_WF_ARTWORK_PROCESS_QC> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_QC_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_PLANNING ART_WF_ARTWORK_PROCESS_PLANNING(ART_WF_ARTWORK_PROCESS_PLANNING_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PLANNING>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_PLANNING_2 ART_WF_ARTWORK_PROCESS_PLANNING(ART_WF_ARTWORK_PROCESS_PLANNING item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PLANNING_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_PLANNING_2> ART_WF_ARTWORK_PROCESS_PLANNING(List<ART_WF_ARTWORK_PROCESS_PLANNING> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_PLANNING_2>>(item);
        }


        public static ART_WF_ARTWORK_PROCESS_WH ART_WF_ARTWORK_PROCESS_WH(ART_WF_ARTWORK_PROCESS_WH_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_WH>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_WH_2 ART_WF_ARTWORK_PROCESS_WH(ART_WF_ARTWORK_PROCESS_WH item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_WH_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_WH_2> ART_WF_ARTWORK_PROCESS_WH(List<ART_WF_ARTWORK_PROCESS_WH> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_WH_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_RD_BY_QC ART_WF_ARTWORK_PROCESS_RD_BY_QC(ART_WF_ARTWORK_PROCESS_RD_BY_QC_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_RD_BY_QC>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_RD_BY_QC_2 ART_WF_ARTWORK_PROCESS_RD_BY_QC(ART_WF_ARTWORK_PROCESS_RD_BY_QC item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_RD_BY_QC_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_RD_BY_QC_2> ART_WF_ARTWORK_PROCESS_RD_BY_QC(List<ART_WF_ARTWORK_PROCESS_RD_BY_QC> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_RD_BY_QC_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_RD ART_WF_ARTWORK_PROCESS_RD(ART_WF_ARTWORK_PROCESS_RD_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_RD>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_RD_2 ART_WF_ARTWORK_PROCESS_RD(ART_WF_ARTWORK_PROCESS_RD item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_RD_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_RD_2> ART_WF_ARTWORK_PROCESS_RD(List<ART_WF_ARTWORK_PROCESS_RD> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_RD_2>>(item);
        }

        public static ART_WF_ARTWORK_ATTACHMENT ART_WF_ARTWORK_ATTACHMENT(ART_WF_ARTWORK_ATTACHMENT_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_ATTACHMENT>(item);
        }
        public static ART_WF_ARTWORK_ATTACHMENT_2 ART_WF_ARTWORK_ATTACHMENT(ART_WF_ARTWORK_ATTACHMENT item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_ATTACHMENT_2>(item);
        }
        public static List<ART_WF_ARTWORK_ATTACHMENT_2> ART_WF_ARTWORK_ATTACHMENT(List<ART_WF_ARTWORK_ATTACHMENT> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_ATTACHMENT_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_PG ART_WF_ARTWORK_PROCESS_PG(ART_WF_ARTWORK_PROCESS_PG_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PG>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_PG_2 ART_WF_ARTWORK_PROCESS_PG(ART_WF_ARTWORK_PROCESS_PG item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PG_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_PG_2> ART_WF_ARTWORK_PROCESS_PG(List<ART_WF_ARTWORK_PROCESS_PG> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_PG_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK_2 ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK_2> ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK(List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_BY_MK_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_2 ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_2> ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA(List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK_2 ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK_2> ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK(List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_PA_BY_MK_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2 ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2> ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC(List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK_2 ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK_2> ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK(List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_QC_BY_MK_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2 ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2> ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC(List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_BY_QC_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_2 ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_2> ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK(List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_MK_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC_2 ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC_2> ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC(List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_BY_QC_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_2 ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_2> ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC(List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_QC_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2 ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK(ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2> ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK(List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_AFTER_CUSTOMER_GM_MK_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER(ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2 ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER(ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2> ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER(List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2>>(item);
        }


        public static ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM(ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2 ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM(ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2> ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM(List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2>>(item);
        }


        public static ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT(ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2 ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT(ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2> ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT(List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA(ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_2 ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA(ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_2> ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA(List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_2>>(item);
        }

        // ticke#425737 commented by aof 
        public static ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD(ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_2 ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD(ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_2> ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD(List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_2>>(item);
        }
        // ticke#425737 comented by aof 

        public static ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE(ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_2 ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE(ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_2> ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE(List<ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_2>>(item);
        }


        public static ART_WF_ARTWORK_PROCESS_PA_PLANT ART_WF_ARTWORK_PROCESS_PA_PLANT(ART_WF_ARTWORK_PROCESS_PA_PLANT_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA_PLANT>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_PA_PLANT_2 ART_WF_ARTWORK_PROCESS_PA_PLANT(ART_WF_ARTWORK_PROCESS_PA_PLANT item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA_PLANT_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_PA_PLANT_2> ART_WF_ARTWORK_PROCESS_PA_PLANT(List<ART_WF_ARTWORK_PROCESS_PA_PLANT> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_PA_PLANT_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_PA_SYMBOL ART_WF_ARTWORK_PROCESS_PA_SYMBOL(ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA_SYMBOL>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2 ART_WF_ARTWORK_PROCESS_PA_SYMBOL(ART_WF_ARTWORK_PROCESS_PA_SYMBOL item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2> ART_WF_ARTWORK_PROCESS_PA_SYMBOL(List<ART_WF_ARTWORK_PROCESS_PA_SYMBOL> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA(ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2 ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA(ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2> ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA(List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_VENDOR ART_WF_ARTWORK_PROCESS_VENDOR(ART_WF_ARTWORK_PROCESS_VENDOR_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_VENDOR>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_VENDOR_2 ART_WF_ARTWORK_PROCESS_VENDOR(ART_WF_ARTWORK_PROCESS_VENDOR item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_VENDOR_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_VENDOR_2> ART_WF_ARTWORK_PROCESS_VENDOR(List<ART_WF_ARTWORK_PROCESS_VENDOR> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_VENDOR_2>>(item);
        }

        public static SAP_M_PO_IDOC SAP_M_PO_IDOC(SAP_M_PO_IDOC_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_PO_IDOC>(item);
        }
        public static SAP_M_PO_IDOC_2 SAP_M_PO_IDOC(SAP_M_PO_IDOC item)
        {
            return AutoMapper.Mapper.Map<SAP_M_PO_IDOC_2>(item);
        }
        public static List<SAP_M_PO_IDOC_2> SAP_M_PO_IDOC(List<SAP_M_PO_IDOC> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_PO_IDOC_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA(ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_2 ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA(ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_2> ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA(List<ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_CUSTOMER_BY_PA_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_CUSTOMER ART_WF_ARTWORK_PROCESS_CUSTOMER(ART_WF_ARTWORK_PROCESS_CUSTOMER_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_CUSTOMER>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_CUSTOMER_2 ART_WF_ARTWORK_PROCESS_CUSTOMER(ART_WF_ARTWORK_PROCESS_CUSTOMER item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_CUSTOMER_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_CUSTOMER_2> ART_WF_ARTWORK_PROCESS_CUSTOMER(List<ART_WF_ARTWORK_PROCESS_CUSTOMER> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_CUSTOMER_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_PP ART_WF_ARTWORK_PROCESS_PP(ART_WF_ARTWORK_PROCESS_PP_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PP>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_PP_2 ART_WF_ARTWORK_PROCESS_PP(ART_WF_ARTWORK_PROCESS_PP item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PP_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_PP_2> ART_WF_ARTWORK_PROCESS_PP(List<ART_WF_ARTWORK_PROCESS_PP> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_PP_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP(ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2 ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP(ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2> ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP(List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PP_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_VENDOR_PO ART_WF_ARTWORK_PROCESS_VENDOR_PO(ART_WF_ARTWORK_PROCESS_VENDOR_PO_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_VENDOR_PO>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_VENDOR_PO_2 ART_WF_ARTWORK_PROCESS_VENDOR_PO(ART_WF_ARTWORK_PROCESS_VENDOR_PO item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_VENDOR_PO_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_VENDOR_PO_2> ART_WF_ARTWORK_PROCESS_VENDOR_PO(List<ART_WF_ARTWORK_PROCESS_VENDOR_PO> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_VENDOR_PO_2>>(item);
        }

        public static SAP_M_ORDER_BOM SAP_M_ORDER_BOM(SAP_M_ORDER_BOM_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_ORDER_BOM>(item);
        }
        public static SAP_M_ORDER_BOM_2 SAP_M_ORDER_BOM(SAP_M_ORDER_BOM item)
        {
            return AutoMapper.Mapper.Map<SAP_M_ORDER_BOM_2>(item);
        }
        public static List<SAP_M_ORDER_BOM_2> SAP_M_ORDER_BOM(List<SAP_M_ORDER_BOM> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_ORDER_BOM_2>>(item);
        }

        public static ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT(ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT>(item);
        }
        public static ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2 ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT(ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2>(item);
        }
        public static List<ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2> ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT(List<ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_2>>(item);
        }

        public static IGRID_M_OUTBOUND_HEADER IGRID_M_OUTBOUND_HEADER(IGRID_M_OUTBOUND_HEADER_2 item)
        {
            return AutoMapper.Mapper.Map<IGRID_M_OUTBOUND_HEADER>(item);
        }
        public static IGRID_M_OUTBOUND_HEADER_2 IGRID_M_OUTBOUND_HEADER(IGRID_M_OUTBOUND_HEADER item)
        {
            return AutoMapper.Mapper.Map<IGRID_M_OUTBOUND_HEADER_2>(item);
        }
        public static List<IGRID_M_OUTBOUND_HEADER_2> IGRID_M_OUTBOUND_HEADER(List<IGRID_M_OUTBOUND_HEADER> item)
        {
            return AutoMapper.Mapper.Map<List<IGRID_M_OUTBOUND_HEADER_2>>(item);
        }


        public static ART_M_PIC ART_M_PIC(ART_M_PIC_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_PIC>(item);
        }
        public static ART_M_PIC_2 ART_M_PIC(ART_M_PIC item)
        {
            return AutoMapper.Mapper.Map<ART_M_PIC_2>(item);
        }
        public static List<ART_M_PIC_2> ART_M_PIC(List<ART_M_PIC> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_PIC_2>>(item);
        }

        public static IGRID_M_OUTBOUND_ITEM IGRID_M_OUTBOUND_ITEM(IGRID_M_OUTBOUND_ITEM_2 item)
        {
            return AutoMapper.Mapper.Map<IGRID_M_OUTBOUND_ITEM>(item);
        }
        public static IGRID_M_OUTBOUND_ITEM_2 IGRID_M_OUTBOUND_ITEM(IGRID_M_OUTBOUND_ITEM item)
        {
            return AutoMapper.Mapper.Map<IGRID_M_OUTBOUND_ITEM_2>(item);
        }
        public static List<IGRID_M_OUTBOUND_ITEM_2> IGRID_M_OUTBOUND_ITEM(List<IGRID_M_OUTBOUND_ITEM> item)
        {
            return AutoMapper.Mapper.Map<List<IGRID_M_OUTBOUND_ITEM_2>>(item);
        }

        //public static V_ART_ENDTOEND_REPORT V_ART_ENDTOEND_REPORT(V_ART_ENDTOEND_REPORT_2 item)
        //{
        //    return AutoMapper.Mapper.Map<V_ART_ENDTOEND_REPORT>(item);
        //}
        //public static V_ART_ENDTOEND_REPORT_2 V_ART_ENDTOEND_REPORT(V_ART_ENDTOEND_REPORT item)
        //{
        //    return AutoMapper.Mapper.Map<V_ART_ENDTOEND_REPORT_2>(item);
        //}
        //public static List<V_ART_ENDTOEND_REPORT_2> V_ART_ENDTOEND_REPORT(List<V_ART_ENDTOEND_REPORT> item)
        //{
        //    return AutoMapper.Mapper.Map<List<V_ART_ENDTOEND_REPORT_2>>(item);
        //}
        
        public static V_ART_WAREHOUSE_REPORT V_ART_WAREHOUSE_REPORT(V_ART_WAREHOUSE_REPORT_2 item)
        {
            return AutoMapper.Mapper.Map<V_ART_WAREHOUSE_REPORT>(item);
        }
        public static V_ART_WAREHOUSE_REPORT_2 V_ART_WAREHOUSE_REPORT(V_ART_WAREHOUSE_REPORT item)
        {
            return AutoMapper.Mapper.Map<V_ART_WAREHOUSE_REPORT_2>(item);
        }
        public static List<V_ART_WAREHOUSE_REPORT_2> V_ART_WAREHOUSE_REPORT(List<V_ART_WAREHOUSE_REPORT> item)
        {
            return AutoMapper.Mapper.Map<List<V_ART_WAREHOUSE_REPORT_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT(ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2 ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT(ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2> ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT(List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_2>>(item);
        }

        public static ART_M_POSITION_ROLE ART_M_POSITION_ROLE(ART_M_POSITION_ROLE_2 item)
        {
            return AutoMapper.Mapper.Map<ART_M_POSITION_ROLE>(item);
        }
        public static ART_M_POSITION_ROLE_2 ART_M_POSITION_ROLE(ART_M_POSITION_ROLE item)
        {
            return AutoMapper.Mapper.Map<ART_M_POSITION_ROLE_2>(item);
        }
        public static List<ART_M_POSITION_ROLE_2> ART_M_POSITION_ROLE(List<ART_M_POSITION_ROLE> item)
        {
            return AutoMapper.Mapper.Map<List<ART_M_POSITION_ROLE_2>>(item);
        }

        public static SAP_M_MATERIAL_CONVERSION SAP_M_MATERIAL_CONVERSION(SAP_M_MATERIAL_CONVERSION_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_MATERIAL_CONVERSION>(item);
        }
        public static SAP_M_MATERIAL_CONVERSION_2 SAP_M_MATERIAL_CONVERSION(SAP_M_MATERIAL_CONVERSION item)
        {
            return AutoMapper.Mapper.Map<SAP_M_MATERIAL_CONVERSION_2>(item);
        }
        public static List<SAP_M_MATERIAL_CONVERSION_2> SAP_M_MATERIAL_CONVERSION(List<SAP_M_MATERIAL_CONVERSION> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_MATERIAL_CONVERSION_2>>(item);
        }

        public static ART_WF_ARTWORK_MATERIAL_LOCK ART_WF_ARTWORK_MATERIAL_LOCK(ART_WF_ARTWORK_MATERIAL_LOCK_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_MATERIAL_LOCK>(item);
        }
        public static ART_WF_ARTWORK_MATERIAL_LOCK_2 ART_WF_ARTWORK_MATERIAL_LOCK(ART_WF_ARTWORK_MATERIAL_LOCK item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_MATERIAL_LOCK_2>(item);
        }
        public static List<ART_WF_ARTWORK_MATERIAL_LOCK_2> ART_WF_ARTWORK_MATERIAL_LOCK(List<ART_WF_ARTWORK_MATERIAL_LOCK> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_MATERIAL_LOCK_2>>(item);
        }


        public static ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL(ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL>(item);
        }
        public static ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_2 ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL(ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_2>(item);
        }
        public static List<ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_2> ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL(List<ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_2>>(item);
        }

        public static ART_WF_ARTWORK_MATERIAL_LOCK_LOG ART_WF_ARTWORK_MATERIAL_LOCK_LOG(ART_WF_ARTWORK_MATERIAL_LOCK_LOG_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_MATERIAL_LOCK_LOG>(item);
        }
        public static ART_WF_ARTWORK_MATERIAL_LOCK_LOG_2 ART_WF_ARTWORK_MATERIAL_LOCK_LOG(ART_WF_ARTWORK_MATERIAL_LOCK_LOG item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_MATERIAL_LOCK_LOG_2>(item);
        }
        public static List<ART_WF_ARTWORK_MATERIAL_LOCK_LOG_2> ART_WF_ARTWORK_MATERIAL_LOCK_LOG(List<ART_WF_ARTWORK_MATERIAL_LOCK_LOG> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_MATERIAL_LOCK_LOG_2>>(item);
        }

        public static ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER(ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER>(item);
        }
        public static ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2 ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER(ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2>(item);
        }
        public static List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2> ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER(List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2>>(item);
        }

        public static SAP_M_PO_IDOC_ITEM SAP_M_PO_IDOC_ITEM(SAP_M_PO_IDOC_ITEM_2 item)
        {
            return AutoMapper.Mapper.Map<SAP_M_PO_IDOC_ITEM>(item);
        }
        public static SAP_M_PO_IDOC_ITEM_2 SAP_M_PO_IDOC_ITEM(SAP_M_PO_IDOC_ITEM item)
        {
            return AutoMapper.Mapper.Map<SAP_M_PO_IDOC_ITEM_2>(item);
        }
        public static List<SAP_M_PO_IDOC_ITEM_2> SAP_M_PO_IDOC_ITEM(List<SAP_M_PO_IDOC_ITEM> item)
        {
            return AutoMapper.Mapper.Map<List<SAP_M_PO_IDOC_ITEM_2>>(item);
        }

        public static XECM_M_PRODUCT5 XECM_M_PRODUCT5(XECM_M_PRODUCT5_2 item)
        {
            return AutoMapper.Mapper.Map<XECM_M_PRODUCT5>(item);
        }
        public static XECM_M_PRODUCT5_2 XECM_M_PRODUCT5(XECM_M_PRODUCT5 item)
        {
            return AutoMapper.Mapper.Map<XECM_M_PRODUCT5_2>(item);
        }
        public static List<XECM_M_PRODUCT5_2> XECM_M_PRODUCT5(List<XECM_M_PRODUCT5> item)
        {
            return AutoMapper.Mapper.Map<List<XECM_M_PRODUCT5_2>>(item);
        }

        public static ART_WF_ARTWORK_MAPPING_PO ART_WF_ARTWORK_MAPPING_PO(ART_WF_ARTWORK_MAPPING_PO_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_MAPPING_PO>(item);
        }
        public static ART_WF_ARTWORK_MAPPING_PO_2 ART_WF_ARTWORK_MAPPING_PO(ART_WF_ARTWORK_MAPPING_PO item)
        {
            return AutoMapper.Mapper.Map<ART_WF_ARTWORK_MAPPING_PO_2>(item);
        }
        public static List<ART_WF_ARTWORK_MAPPING_PO_2> ART_WF_ARTWORK_MAPPING_PO(List<ART_WF_ARTWORK_MAPPING_PO> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_ARTWORK_MAPPING_PO_2>>(item);
        }

        public static ART_WF_REMARK_REASON_OTHER ART_WF_REMARK_REASON_OTHER(ART_WF_REMARK_REASON_OTHER_2 item)
        {
            return AutoMapper.Mapper.Map<ART_WF_REMARK_REASON_OTHER>(item);
        }
        public static ART_WF_REMARK_REASON_OTHER_2 ART_WF_REMARK_REASON_OTHER(ART_WF_REMARK_REASON_OTHER item)
        {
            return AutoMapper.Mapper.Map<ART_WF_REMARK_REASON_OTHER_2>(item);
        }
        public static List<ART_WF_REMARK_REASON_OTHER_2> ART_WF_REMARK_REASON_OTHER(List<ART_WF_REMARK_REASON_OTHER> item)
        {
            return AutoMapper.Mapper.Map<List<ART_WF_REMARK_REASON_OTHER_2>>(item);
        }

    }
}


//public static ART_M_USER_ROLE ART_M_USER_ROLE(ART_M_USER_ROLE_2 item)
//{
//    return AutoMapper.Mapper.Map<ART_M_USER_ROLE>(item);
//}
//public static ART_M_USER_ROLE_2 ART_M_USER_ROLE(ART_M_USER_ROLE item)
//{
//    return AutoMapper.Mapper.Map<ART_M_USER_ROLE_2>(item);
//}
//public static List<ART_M_USER_ROLE_2> ART_M_USER_ROLE(List<ART_M_USER_ROLE> item)
//{
//    return AutoMapper.Mapper.Map<List<ART_M_USER_ROLE_2>>(item);
//}














































