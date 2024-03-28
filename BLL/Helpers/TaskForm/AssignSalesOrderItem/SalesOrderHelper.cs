using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace BLL.Helpers
{
    public class SalesOrderHelper
    {
        private static string formatDate = "dd/MM/yyyy";

        public static ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT GetSODetail_NewData(ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT Results = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT();
            ART_WF_ARTWORK_PROCESS_SO_DETAIL soDetail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
            ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 soDetail_2 = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
            List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2> listSODetail2 = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>();

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
                        using (CNService.IsolationLevel(context))
                        {
                            soDetail.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;
                            soDetail.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;

                            listSODetail2 = MapperServices.ART_WF_ARTWORK_PROCESS_SO_DETAIL(ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(soDetail, context));

                            //for performance 
                            var listHeader = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
                                              where h.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                              select h).ToList();

                            var listItem = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM
                                            where h.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                            select h).ToList();

                            var listBom = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT
                                           where h.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                           select h).ToList();

                            var listSALES_ORDER_NO2 = listSODetail2.Select(m => m.SALES_ORDER_NO.PadLeft(10, '0')).ToList();
                            var listSAP_M_LONG_TEXT = (from h in context.SAP_M_LONG_TEXT
                                                       where listSALES_ORDER_NO2.Contains(h.TEXT_NAME.Substring(0, 10))
                                                       select h).ToList();

                            var listSAP_M_LONG_TEXT_ASSIGN = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT
                                                              where h.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                                              select h).ToList();

                            if (listSODetail2.Count > 0)
                            {
                                ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2 header2 = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2();
                                ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2 item2 = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2();
                                ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2 component2 = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2();
                                List <ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2> listcomponent2 = new List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2>();
                                string soldtoPO = "";
                                string shiptoPO = "";

                                for (int i = 0; i <= listSODetail2.Count - 1; i++)
                                {
                                    header2 = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_2();
                                    item2 = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_2();
                                    component2 = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2();
                                    listcomponent2 = new List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_2>();
                                    header2 = MapperServices.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER(listHeader.Where(m => m.ARTWORK_SUB_ID == listSODetail2[i].ARTWORK_SUB_ID
                                                                                                            && m.SALES_ORDER_NO == listSODetail2[i].SALES_ORDER_NO
                                                                                                            && m.ARTWORK_PROCESS_SO_ID == listSODetail2[i].ARTWORK_PROCESS_SO_ID).FirstOrDefault());

                                    if (header2 != null && !String.IsNullOrEmpty(listSODetail2[i].SALES_ORDER_ITEM))
                                    {
                                        var SALES_ORDER_ITEM = Convert.ToDecimal(listSODetail2[i].SALES_ORDER_ITEM);
                                        item2 = MapperServices.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM(listItem.Where(m => m.ARTWORK_SUB_ID == listSODetail2[i].ARTWORK_SUB_ID
                                                                                                                    && m.ASSIGN_SO_HEADER_ID == header2.ASSIGN_SO_HEADER_ID
                                                                                                                    && m.ITEM == SALES_ORDER_ITEM
                                                                                                                    && m.ARTWORK_PROCESS_SO_ID == listSODetail2[i].ARTWORK_PROCESS_SO_ID).FirstOrDefault());
                                    }

                                    int bomID = 0;
                                    bomID = Convert.ToInt32(listSODetail2[i].BOM_ID);

                                    if (item2 != null && bomID > 0)
                                    {
                                        //component2 = MapperServices.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT(listBom.Where(m => m.ASSIGN_SO_ITEM_ID == item2.ASSIGN_SO_ITEM_ID).FirstOrDefault());
                                        listcomponent2 = MapperServices.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT(listBom.Where(m => m.ASSIGN_SO_ITEM_ID == item2.ASSIGN_SO_ITEM_ID).ToList());
                                        component2 = listcomponent2.FirstOrDefault();
                                    }

                                    StringBuilder sb = new StringBuilder();

                                    if (header2 != null)
                                    {
                                        listSODetail2[i].SOLD_TO_NAME = header2.SOLD_TO_NAME;
                                        listSODetail2[i].SHIP_TO_NAME = header2.SHIP_TO_NAME;
                                        listSODetail2[i].SALES_ORG = header2.SALES_ORG;
                                        listSODetail2[i].SO_NUMBER = header2.SALES_ORDER_NO;

                                        if (!String.IsNullOrEmpty(header2.RDD.ToString()))
                                        {
                                            DateTime dt = Convert.ToDateTime(header2.RDD.ToString());
                                            listSODetail2[i].RDD_DISPLAY_TXT = dt.ToString(formatDate);
                                        }

                                        //soldtoPO = header2.SOLD_TO_PO;
                                        //shiptoPO = header2.SHIP_TO_PO;
                                        //if (String.IsNullOrEmpty(item2.ITEM_CUSTOM_2))
                                        //{
                                        //    soldtoPO = header2.SOLD_TO_PO;
                                        //}
                                        //else
                                        //{
                                        //    soldtoPO = item2.ITEM_CUSTOM_2;
                                        //}

                                        //if (String.IsNullOrEmpty(item2.ITEM_CUSTOM_3))
                                        //{
                                        //    shiptoPO = header2.SHIP_TO_PO;
                                        //}
                                        //else
                                        //{
                                        //    shiptoPO = item2.ITEM_CUSTOM_3;
                                        //}

                                        //if (CNService.IsDevOrQAS())
                                        //{
                                        soldtoPO = item2.ITEM_CUSTOM_2;
                                        shiptoPO = item2.ITEM_CUSTOM_3;
                                        //}
                                        //else
                                        //{
                                        //    soldtoPO = header2.SOLD_TO_PO;
                                        //    shiptoPO = header2.SHIP_TO_PO;
                                        //}

                                        listSODetail2[i].LC = header2.LAST_SHIPMENT_DATE.ToString();
                                        listSODetail2[i].SOLD_TO_PARTY = header2.SOLD_TO + ":" + header2.SOLD_TO_NAME;
                                        listSODetail2[i].SHIP_TO_PARTY = header2.SHIP_TO + ":" + header2.SHIP_TO_NAME;
                                        listSODetail2[i].SOLD_TO_PO = soldtoPO; //header2.SOLD_TO_PO;
                                        listSODetail2[i].SHIP_TO_PO = shiptoPO;// header2.SHIP_TO_PO;
                                        listSODetail2[i].PAYMENT_TERM = header2.PAYMENT_TERM;
                                        listSODetail2[i].LC_NO = header2.LC_NO;
                                        listSODetail2[i].EXP = header2.EXPIRED_DATE.ToString();

                                        if (header2.CREATE_ON != null)
                                        {
                                            DateTime dt1 = Convert.ToDateTime(header2.CREATE_ON);
                                            listSODetail2[i].CREATE_DATE_DISPLAY_TXT = dt1.ToString(formatDate);
                                        }

                                        string textName = header2.SALES_ORDER_NO.ToString().PadLeft(10, '0');
                                        var temp = listSAP_M_LONG_TEXT_ASSIGN.Where(m => m.TEXT_NAME == textName && m.TEXT_ID == "Z001").OrderBy(m => m.LINE_ID).ToList();
                                        listSODetail2[i].GENERAL_TEXT = GetLongText(temp);
                                    }

                                    if (item2 != null)
                                    {
                                        listSODetail2[i].SO_ITEM_NO = item2.ITEM.ToString();
                                        listSODetail2[i].FOC_ITEM = item2.ITEM_CUSTOM_1;

                                        //if (!String.IsNullOrEmpty(item2.ITEM_CUSTOM_1))
                                        //{
                                        //    listSODetail2[i].QUANTITY = item2.ORDER_QTY.ToString();
                                        //}

                                        if (!String.IsNullOrEmpty(item2.BRAND_ID))
                                        {
                                            SAP_M_BRAND brand = new SAP_M_BRAND();
                                            brand.MATERIAL_GROUP = item2.BRAND_ID;
                                            brand = SAP_M_BRAND_SERVICE.GetByItem(brand, context).FirstOrDefault();

                                            listSODetail2[i].BRAND = "";
                                            if (brand != null)
                                            {
                                                listSODetail2[i].BRAND = brand.MATERIAL_GROUP + ":" + brand.DESCRIPTION;
                                            }
                                        }

                                        listSODetail2[i].MATERIAL_NO = item2.PRODUCT_CODE;
                                        listSODetail2[i].MATERIAL_DESC = item2.MATERIAL_DESCRIPTION;
                                        listSODetail2[i].PORT = item2.PORT;
                                        listSODetail2[i].PRODUCTION_PLANT = item2.PRODUCTION_PLANT;
                                        listSODetail2[i].ORDER_QTY = item2.ORDER_QTY.ToString();

                                        if (item2.ETD_DATE_FROM != null)
                                        {
                                            DateTime dt1 = Convert.ToDateTime(item2.ETD_DATE_FROM);
                                            listSODetail2[i].ETD_DATE_FROM = dt1.ToString(formatDate);
                                        }

                                        if (item2.ETD_DATE_TO != null)
                                        {
                                            DateTime dt2 = Convert.ToDateTime(item2.ETD_DATE_TO);
                                            listSODetail2[i].ETD_DATE_TO = dt2.ToString(formatDate);
                                        }

                                        listSODetail2[i].MATERIAL = item2.PRODUCT_CODE;
                                        listSODetail2[i].OLD_MATERIAL = item2.OLD_MATERIAL_CODE;
                                        listSODetail2[i].PLANT = item2.PLANT;
                                        listSODetail2[i].DRAIN_WEIGHT = item2.SIZE_DRAIN_WT;
                                        listSODetail2[i].INSP_MEMO = item2.PROD_INSP_MEMO;
                                        listSODetail2[i].REASON_REJECTION = item2.REJECTION_DESCRIPTION;
                                        listSODetail2[i].BRAND_ADDITIONAL = item2.ADDITIONAL_BRAND_DESCRIPTION;
                                        listSODetail2[i].VIA = item2.VIA;
                                        listSODetail2[i].IN_TRANSIT_TO = item2.IN_TRANSIT_TO;
                                        listSODetail2[i].PACK_SIZE = item2.PACK_SIZE;

                                        //Get Warehouse Text
                                        string itemNOTmp = "";
                                        string orderNOTmp = "";
                                        string textName = "";

                                        if (header2 != null)
                                        {
                                            listSODetail2[i].ASSIGN_ID = header2.SALES_ORDER_NO + item2.ITEM.ToString() + item2.PRODUCT_CODE;
                                            orderNOTmp = header2.SALES_ORDER_NO.ToString().PadLeft(10, '0');
                                            itemNOTmp = item2.ITEM.ToString().PadLeft(6, '0');
                                            textName = orderNOTmp + itemNOTmp;

                                            //listSODetail2[i].WAREHOUSE_TEXT = GetLongText_Master(_textName, "Z105");
                                            var temp = listSAP_M_LONG_TEXT_ASSIGN.Where(m => m.TEXT_NAME == textName && m.TEXT_ID == "Z105").OrderBy(m => m.LINE_ID).ToList();
                                            listSODetail2[i].WAREHOUSE_TEXT = GetLongText(temp);
                                        }
                                    }

                                    if (item2 != null && component2 != null)
                                    {
                                        listSODetail2[i].ORDER_BOM_NO = component2.COMPONENT_MATERIAL;
                                        listSODetail2[i].ORDER_BOM_DESC = component2.DECRIPTION;

                                        //if (String.IsNullOrEmpty(listSODetail2[i].FOC_ITEM))
                                        //{
                                        //    listSODetail2[i].QUANTITY = component2.QUANTITY.ToString();
                                        //}
                                        //if (component2.QUANTITY != null)
                                        //    listSODetail2[i].QUANTITY = component2.QUANTITY.ToString();
                                        //else if (item2.ORDER_QTY != null)
                                        //    listSODetail2[i].QUANTITY = item2.ORDER_QTY.ToString();
                                        //else
                                        //    listSODetail2[i].QUANTITY = "";
                                        
                                        if (listcomponent2.Count>0)
                                            listSODetail2[i].QUANTITY = string.Format("{0}", component2.QUANTITY.ToString());
                                        else //if (item2.ORDER_QTY != null)
                                            listSODetail2[i].QUANTITY = string.Format("{0}", item2.ORDER_QTY.ToString());
                                        //else
                                        //    listSODetail2[i].QUANTITY = string.Format("{0}", "");
                                        if (string.IsNullOrEmpty(component2.STOCK)) component2.STOCK = "";
                                        if (string.IsNullOrEmpty(item2.STOCK)) item2.STOCK = "";

                                        if (!string.IsNullOrEmpty(component2.STOCK))
                                            listSODetail2[i].STOCK_PO = component2.STOCK;
                                        else if (!string.IsNullOrEmpty(item2.STOCK))
                                            listSODetail2[i].STOCK_PO = item2.STOCK;

                                        if (header2 != null)
                                            listSODetail2[i].ASSIGN_ID = header2.SALES_ORDER_NO + item2.ITEM.ToString() + item2.PRODUCT_CODE + component2.COMPONENT_MATERIAL;
                                    }

                                    listSODetail2[i].GROUPING_DISPLAY_TXT = "";
                                }
                            }
                        }
                    }
                }

                Results.status = "S";
                Results.data = listSODetail2;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT GetSODetail_FOCData(ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT Results = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT();
            List<V_SAP_SALES_ORDER_2> listSOFOC = new List<V_SAP_SALES_ORDER_2>();

            ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 soDetail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
            List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2> listSoDetail = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>();

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
                        using (CNService.IsolationLevel(context))
                        {
                            var soAssigns = (from c in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                             where c.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                             select new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2()
                                             {
                                                 SALES_ORDER_NO = c.SALES_ORDER_NO
                                             }).Distinct().ToList();

                            if (soAssigns != null)
                            {

                                var materialPA = (from m in context.ART_WF_ARTWORK_PROCESS_PA
                                                  where m.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                                  select m.MATERIAL_NO).FirstOrDefault();

                                foreach (var iSOAssign in soAssigns)
                                {
                                    //var listSoAssigns = context.Database.SqlQuery<V_SAP_SALES_ORDER2>(@"SP_SALES_ORDER_ASSIGNS @SALES_ORDER_NO,@materialPA",
                                    //        new SqlParameter("@SALES_ORDER_NO", iSOAssign.SALES_ORDER_NO),
                                    //        new SqlParameter("@materialPA", string.Format("{0}", materialPA))).ToList();
                                    //var vSO = listSoAssigns.Select(v => new V_SAP_SALES_ORDER_2()
                                    //{
                                    //    SALES_ORDER_NO = v.SALES_ORDER_NO,
                                    //    ITEM = v.ITEM,
                                    //    PRODUCT_CODE = v.PRODUCT_CODE,
                                    //    MATERIAL_DESCRIPTION = v.MATERIAL_DESCRIPTION
                                    //}).ToList();
                                    var vSO = (from v in context.V_SAP_SALES_ORDER
                                               where v.SALES_ORDER_NO == iSOAssign.SALES_ORDER_NO
                                               && !String.IsNullOrEmpty(v.ITEM_CUSTOM_1)
                                               && (v.PRODUCT_CODE == materialPA)
                                               && v.SO_ITEM_IS_ACTIVE == "X"
                                               select new V_SAP_SALES_ORDER_2()
                                               {
                                                   SALES_ORDER_NO = v.SALES_ORDER_NO,
                                                   ITEM = v.ITEM,
                                                   PRODUCT_CODE = v.PRODUCT_CODE,
                                                   MATERIAL_DESCRIPTION = v.MATERIAL_DESCRIPTION
                                               }).ToList();
                                    if (vSO != null)
                                    {
                                        listSOFOC.AddRange(vSO);
                                    }
                                }
                            }

                            foreach (var iSOFoc in listSOFOC)
                            {
                                var soExist = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                               where s.SALES_ORDER_NO == iSOFoc.SALES_ORDER_NO
                                                && s.SALES_ORDER_ITEM == iSOFoc.ITEM.ToString()
                                                && s.MATERIAL_NO == iSOFoc.PRODUCT_CODE
                                                && s.BOM_NO.Contains("FOC")
                                               select s).FirstOrDefault();

                                if (soExist == null)
                                {
                                    soDetail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
                                    soDetail.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                    soDetail.SALES_ORDER_NO = iSOFoc.SALES_ORDER_NO;
                                    soDetail.SALES_ORDER_ITEM = iSOFoc.ITEM.ToString();
                                    soDetail.MATERIAL_NO = iSOFoc.PRODUCT_CODE;
                                    soDetail.MATERIAL_DESC = iSOFoc.MATERIAL_DESCRIPTION;
                                    listSoDetail.Add(soDetail);
                                }
                            }
                        }
                    }
                }

                Results.status = "S";
                Results.data = listSoDetail;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        private static string GetLongText(List<SAP_M_LONG_TEXT> list)
        {
            StringBuilder sb = new StringBuilder();
            string result = "";
            foreach (var iLongTxt in list)
            {
                sb.AppendLine(iLongTxt.LINE_TEXT);
            }
            result = sb.ToString().Replace("\r\n", "<br />");
            return result;
        }
        //private static string GetLongText_Master(string TEXT_NAME, string type)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    SAP_M_LONG_TEXT longText = new SAP_M_LONG_TEXT();
        //    List<SAP_M_LONG_TEXT> listLongText = new List<SAP_M_LONG_TEXT>();
        //    string result = "";

        //    longText.TEXT_NAME = TEXT_NAME; //_header2.SALES_ORDER_NO.ToString().PadLeft(10, '0');
        //    longText.TEXT_ID = type; //"Z001";
        //    listLongText = SAP_M_LONG_TEXT_SERVICE.GetByItem(longText);
        //    if (listLongText.Count > 0)
        //    {
        //        foreach (var iLongTxt in listLongText)
        //        {
        //            sb.AppendLine(iLongTxt.LINE_TEXT);
        //        }

        //        result = sb.ToString().Replace("\r\n", "<br />");
        //    }

        //    return result;
        //}
        //private static string GetLongText_Master(string TEXT_NAME, string type, ARTWORKEntities context)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    SAP_M_LONG_TEXT longText = new SAP_M_LONG_TEXT();
        //    List<SAP_M_LONG_TEXT> listLongText = new List<SAP_M_LONG_TEXT>();
        //    string result = "";

        //    longText.TEXT_NAME = TEXT_NAME; //_header2.SALES_ORDER_NO.ToString().PadLeft(10, '0');
        //    longText.TEXT_ID = type; //"Z001";
        //    listLongText = SAP_M_LONG_TEXT_SERVICE.GetByItem(longText, context);
        //    if (listLongText.Count > 0)
        //    {
        //        foreach (var iLongTxt in listLongText)
        //        {
        //            sb.AppendLine(iLongTxt.LINE_TEXT);
        //        }

        //        result = sb.ToString().Replace("\r\n", "<br />");
        //    }

        //    return result;
        //}

        private static string GetLongText(List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT> list)
        {
            StringBuilder sb = new StringBuilder();
            string result = "";
            foreach (var iLongTxt in list)
            {
                sb.AppendLine(iLongTxt.LINE_TEXT);
            }
            result = sb.ToString().Replace("\r\n", "<br />");
            return result;
        }

        //private static string GetLongTextText_Assign(string TEXT_NAME, string type, int artwork_sub_id, ARTWORKEntities context)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT longText = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT();
        //    List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT> listLongText = new List<ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT>();
        //    string result = "";

        //    longText.ARTWORK_SUB_ID = artwork_sub_id;
        //    longText.TEXT_NAME = TEXT_NAME; //_header2.SALES_ORDER_NO.ToString().PadLeft(10, '0');
        //    longText.TEXT_ID = type; //"Z001";
        //    listLongText = ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_SERVICE.GetByItem(longText, context);
        //    if (listLongText.Count > 0)
        //    {
        //        foreach (var iLongTxt in listLongText)
        //        {
        //            sb.AppendLine(iLongTxt.LINE_TEXT);
        //        }

        //        result = sb.ToString().Replace("\r\n", "<br />");
        //    }

        //    return result;
        //}

        public static ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT GetSalesOrderItemPopup(ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT Results = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT();
            var listRes = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        var PROCESS_PA_CHECK = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PA() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context).FirstOrDefault();
                        if (PROCESS_PA_CHECK != null)
                        {
                            var isProductCode = false;
                            var productCode = ART_WF_ARTWORK_REQUEST_PRODUCT_SERVICE.GetByItem(new ART_WF_ARTWORK_REQUEST_PRODUCT() { ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID }, context);
                            if (productCode.Count > 0)
                            {
                                isProductCode = true;
                            }

                            var MATERIAL_GROUP_ID = PROCESS_PA_CHECK.MATERIAL_GROUP_ID;
                            var PRODUCT_CODE_ID = PROCESS_PA_CHECK.PRODUCT_CODE_ID;
                            if (isProductCode)
                            {
                                if (PROCESS_PA_CHECK.MATERIAL_GROUP_ID > 0 && PROCESS_PA_CHECK.PRODUCT_CODE_ID > 0)
                                { }
                                else
                                {
                                    Results.data = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>();
                                    Results.status = "E";
                                    Results.msg = "Please input material group or product code before assign sales order item";
                                    return Results;
                                }
                            }
                            else
                            {
                                if (PROCESS_PA_CHECK.MATERIAL_GROUP_ID > 0)
                                { }
                                else
                                {
                                    Results.data = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>();
                                    Results.status = "E";
                                    Results.msg = "Please input material group before assign sales order item";
                                    return Results;
                                }
                            }
                        }

                        string TYPE_OF_ARTWORK = "";
                        List<V_SAP_SALES_ORDER> tempList = new List<V_SAP_SALES_ORDER>();

                        if (param != null || param.data != null)
                        {
                            if (param.data.ARTWORK_SUB_ID > 0)
                            {
                                var process = (from p in context.ART_WF_ARTWORK_PROCESS
                                               where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                               select p).FirstOrDefault();

                                if (process != null)
                                {
                                    var request = (from qq in context.ART_WF_ARTWORK_REQUEST
                                                   where qq.ARTWORK_REQUEST_ID == process.ARTWORK_REQUEST_ID
                                                   select qq).FirstOrDefault();

                                    var sold_to = "";
                                    var ship_to = "";
                                    var tempSoldTo = XECM_M_CUSTOMER_SERVICE.GetByItem(new XECM_M_CUSTOMER() { CUSTOMER_ID = request.SOLD_TO_ID.GetValueOrDefault() }, context).FirstOrDefault();
                                    if (tempSoldTo != null)
                                    {
                                        sold_to = tempSoldTo.CUSTOMER_CODE;
                                    }
                                    var tempShipTo = XECM_M_CUSTOMER_SERVICE.GetByItem(new XECM_M_CUSTOMER() { CUSTOMER_ID = request.SHIP_TO_ID.GetValueOrDefault() }, context).FirstOrDefault();
                                    if (tempShipTo != null)
                                    {
                                        ship_to = tempShipTo.CUSTOMER_CODE;
                                    }

                                    string brand = "";
                                    if (request.BRAND_ID != -1)
                                    {
                                        var tempBrand = SAP_M_BRAND_SERVICE.GetByItem(new SAP_M_BRAND() { BRAND_ID = request.BRAND_ID.GetValueOrDefault() }, context).FirstOrDefault();
                                        if (tempBrand != null)
                                            brand = tempBrand.MATERIAL_GROUP;
                                    }

                                    var MATERIAL_GROUP_TXT = "";
                                    var MATERIAL_GROUP_VALUE = "";
                                    var ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                    var PROCESS_PA = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PA() { ARTWORK_SUB_ID = ARTWORK_SUB_ID }, context).FirstOrDefault();
                                    if (PROCESS_PA != null)
                                    {
                                        var MATERIAL_GROUP_ID = PROCESS_PA.MATERIAL_GROUP_ID;
                                        if (PROCESS_PA.MATERIAL_GROUP_ID > 0)
                                        {
                                            var matGroup = SAP_M_CHARACTERISTIC_SERVICE.GetByCHARACTERISTIC_ID(MATERIAL_GROUP_ID, context);

                                            MATERIAL_GROUP_TXT = matGroup.DESCRIPTION;
                                            MATERIAL_GROUP_VALUE = matGroup.VALUE;

                                            if (MATERIAL_GROUP_TXT == "Inner Corrugated")
                                                MATERIAL_GROUP_TXT = "INNER CORRUGAT";
                                            if (MATERIAL_GROUP_TXT == "Inner non Corrugated")
                                                MATERIAL_GROUP_TXT = "INNER NON CORR";

                                            MATERIAL_GROUP_TXT = MATERIAL_GROUP_TXT.ToUpper();
                                        }
                                    }

                                    IQueryable<V_SAP_SALES_ORDER> q;
                                    TYPE_OF_ARTWORK = request.TYPE_OF_ARTWORK;
                                    if (request.TYPE_OF_ARTWORK == "REPEAT")
                                    {
                                        string matGroupNO = "";
                                        matGroupNO = "5";
                                        if (!String.IsNullOrEmpty(MATERIAL_GROUP_VALUE))
                                        {
                                            matGroupNO = "5" + MATERIAL_GROUP_VALUE.ToUpper();
                                        }

                                        q = (from p in context.V_SAP_SALES_ORDER2
                                             where string.IsNullOrEmpty(p.IS_MIGRATION) &&
                                        (

                                     //ทั่วไป
                                     (p.PRODUCT_CODE.StartsWith("3") && !p.PRODUCT_CODE.StartsWith("3W") && !string.IsNullOrEmpty(p.COMPONENT_MATERIAL) && p.COMPONENT_MATERIAL.StartsWith(matGroupNO) && p.COMPONENT_MATERIAL.Length == 18 && string.IsNullOrEmpty(p.BOM_ITEM_CUSTOM_1) && p.SO_ITEM_IS_ACTIVE == "X" && p.BOM_IS_ACTIVE == "X")
                                     //3v
                                     || (p.PRODUCT_CODE.StartsWith(matGroupNO) && p.PRODUCT_CODE.Length == 18 && string.IsNullOrEmpty(p.ITEM_CUSTOM_1) && p.SO_ITEM_IS_ACTIVE == "X")
                                     //ของแถมที่ไม่ได้ ref กับใคร
                                     || (p.PRODUCT_CODE.StartsWith(matGroupNO) && p.PRODUCT_CODE.Length == 18 && p.ITEM_CUSTOM_1 == "0" && p.SO_ITEM_IS_ACTIVE == "X")

                                        )
                                             select new V_SAP_SALES_ORDER_2
                                             {
                                                 DECRIPTION = p.DECRIPTION,
                                                 COMPONENT_ITEM = p.COMPONENT_ITEM,
                                                 SALES_ORG = p.SALES_ORG,
                                                 SOLD_TO = p.SOLD_TO,
                                                 SOLD_TO_NAME = p.SOLD_TO_NAME,
                                                 SHIP_TO = p.SHIP_TO,
                                                 SHIP_TO_NAME = p.SHIP_TO_NAME,
                                                 BRAND_ID = p.BRAND_ID,
                                                 BRAND_DESCRIPTION = p.BRAND_DESCRIPTION,
                                                 ADDITIONAL_BRAND_ID = p.ADDITIONAL_BRAND_ID,
                                                 ADDITIONAL_BRAND_DESCRIPTION = p.ADDITIONAL_BRAND_DESCRIPTION,
                                                 ZONE = p.ZONE,
                                                 CREATE_ON = p.CREATE_ON,
                                                 RDD = p.RDD,
                                                 COMPONENT_MATERIAL = p.COMPONENT_MATERIAL,
                                                 PRODUCT_CODE = p.PRODUCT_CODE,
                                                 SALES_ORDER_NO = p.SALES_ORDER_NO,
                                                 ITEM = p.ITEM,
                                                 BOM_ITEM_CUSTOM_1 = p.BOM_ITEM_CUSTOM_1,
                                                 ITEM_CUSTOM_1 = p.ITEM_CUSTOM_1,
                                                 COUNTRY = p.COUNTRY,
                                                 IN_TRANSIT_TO = p.IN_TRANSIT_TO,
                                                 PO_COMPLETE_SO_ITEM_COMPONENT_ID = p.PO_COMPLETE_SO_ITEM_COMPONENT_ID,
                                                 PORT = p.PORT,
                                                 PRODUCTION_PLANT = p.PRODUCTION_PLANT,
                                                 QUANTITY = p.QUANTITY,
                                                 BOM_STOCK = p.BOM_STOCK,
                                                 STOCK = p.STOCK,
                                             });

                                        //Repeat not check Material Group
                                        MATERIAL_GROUP_TXT = "";
                                    }
                                    else
                                    {
                                        q = (from p in context.V_SAP_SALES_ORDER2
                                             where string.IsNullOrEmpty(p.IS_MIGRATION) &&
                                             //new
                                             (
                                    (p.PRODUCT_CODE.StartsWith("3") && !p.PRODUCT_CODE.StartsWith("3W") && !string.IsNullOrEmpty(p.BOM_ITEM_CUSTOM_1) && p.BOM_ITEM_CUSTOM_1.Contains("NEW") && p.SO_ITEM_IS_ACTIVE == "X" && p.BOM_IS_ACTIVE == "X")
                                    || (p.PRODUCT_CODE.StartsWith("5") && p.PRODUCT_CODE.Length == 18 && p.ITEM_CUSTOM_1 == "0" && p.SO_ITEM_IS_ACTIVE == "X")
                                             )
                                             select new V_SAP_SALES_ORDER_2
                                             {
                                                 DECRIPTION = p.DECRIPTION,
                                                 COMPONENT_ITEM = p.COMPONENT_ITEM,
                                                 SALES_ORG = p.SALES_ORG,
                                                 SOLD_TO = p.SOLD_TO,
                                                 SOLD_TO_NAME = p.SOLD_TO_NAME,
                                                 SHIP_TO = p.SHIP_TO,
                                                 SHIP_TO_NAME = p.SHIP_TO_NAME,
                                                 BRAND_ID = p.BRAND_ID,
                                                 BRAND_DESCRIPTION = p.BRAND_DESCRIPTION,
                                                 ADDITIONAL_BRAND_ID = p.ADDITIONAL_BRAND_ID,
                                                 ADDITIONAL_BRAND_DESCRIPTION = p.ADDITIONAL_BRAND_DESCRIPTION,
                                                 ZONE = p.ZONE,
                                                 CREATE_ON = p.CREATE_ON,
                                                 RDD = p.RDD,
                                                 COMPONENT_MATERIAL = p.COMPONENT_MATERIAL,
                                                 PRODUCT_CODE = p.PRODUCT_CODE,
                                                 SALES_ORDER_NO = p.SALES_ORDER_NO,
                                                 ITEM = p.ITEM,
                                                 BOM_ITEM_CUSTOM_1 = p.BOM_ITEM_CUSTOM_1,
                                                 ITEM_CUSTOM_1 = p.ITEM_CUSTOM_1,
                                                 COUNTRY = p.COUNTRY,
                                                 IN_TRANSIT_TO = p.IN_TRANSIT_TO,
                                                 PO_COMPLETE_SO_ITEM_COMPONENT_ID = p.PO_COMPLETE_SO_ITEM_COMPONENT_ID,
                                                 PORT = p.PORT,
                                                 PRODUCTION_PLANT = p.PRODUCTION_PLANT,
                                                 QUANTITY = p.QUANTITY,
                                                 BOM_STOCK = p.BOM_STOCK,
                                                 STOCK = p.STOCK,
                                             });
                                    }

                                    if (!string.IsNullOrEmpty(brand))
                                    {
                                        q = q.Where(m => m.BRAND_ID.Equals(brand));
                                    }
                                    if (!string.IsNullOrEmpty(sold_to))
                                    {
                                        q = q.Where(m => m.SOLD_TO.Equals(sold_to));
                                    }
                                    if (!string.IsNullOrEmpty(ship_to))
                                    {
                                        q = q.Where(m => m.SHIP_TO.Equals(ship_to));
                                    }
                                    if (!string.IsNullOrEmpty(MATERIAL_GROUP_TXT))
                                    {
                                        q = q.Where(m => m.BOM_ITEM_CUSTOM_1.Contains(MATERIAL_GROUP_TXT));
                                    }

                                    tempList = q.ToList();
                                }
                            }
                        }

                        var tempList2 = tempList;
                        var list = new List<V_SAP_SALES_ORDER>();
                        foreach (var item in tempList2)
                        {
                            string orderNO = item.SALES_ORDER_NO;
                            string itemNO = item.ITEM.ToString();

                            var soExist = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                           where s.SALES_ORDER_NO == orderNO
                                            && s.SALES_ORDER_ITEM == itemNO
                                            && s.BOM_ID == item.PO_COMPLETE_SO_ITEM_COMPONENT_ID
                                           select s).FirstOrDefault();

                            if (soExist != null)
                            {
                                if (!String.IsNullOrEmpty(item.BOM_ITEM_CUSTOM_1) && item.BOM_ITEM_CUSTOM_1.Contains("MULTI"))
                                {
                                    list.Add(item);
                                }
                            }
                            else
                            {
                                list.Add(item);
                            }
                        }

                        List<SAP_M_LONG_TEXT> listLongText = new List<SAP_M_LONG_TEXT>();
                        foreach (var iHeader in list)
                        {
                            var soItemPopup = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();

                            soItemPopup.SOLD_TO_NAME = iHeader.SOLD_TO_NAME;
                            soItemPopup.SHIP_TO_NAME = iHeader.SHIP_TO_NAME;
                            soItemPopup.SALES_ORG = iHeader.SALES_ORG;
                            soItemPopup.SO_NUMBER = iHeader.SALES_ORDER_NO;
                            soItemPopup.SO_ITEM_NO = iHeader.ITEM.ToString();
                            soItemPopup.BRAND = iHeader.BRAND_DESCRIPTION;
                            soItemPopup.MATERIAL_NO = iHeader.PRODUCT_CODE;
                            soItemPopup.PORT = iHeader.PORT;
                            soItemPopup.PRODUCTION_PLANT = iHeader.PRODUCTION_PLANT;
                            soItemPopup.COUNTRY = iHeader.COUNTRY;
                            if (iHeader.RDD != null)
                            {
                                DateTime dt = Convert.ToDateTime(iHeader.RDD.ToString());
                                soItemPopup.RDD_DISPLAY_TXT = dt.ToString(formatDate);
                            }
                            soItemPopup.RDD = iHeader.RDD;
                            soItemPopup.ORDER_BOM_ID = Convert.ToInt32(iHeader.PO_COMPLETE_SO_ITEM_COMPONENT_ID);
                            soItemPopup.ORDER_BOM_NO = iHeader.COMPONENT_MATERIAL;
                            soItemPopup.QUANTITY = iHeader.QUANTITY.ToString();
                            soItemPopup.STOCK_PO = iHeader.BOM_STOCK;
                            soItemPopup.ORDER_BOM_DESC = iHeader.DECRIPTION;
                            soItemPopup.ORDER_BOM_ID = Convert.ToInt32(iHeader.PO_COMPLETE_SO_ITEM_COMPONENT_ID);
                            soItemPopup.BOM_ITEM_CUSTOM_1 = iHeader.BOM_ITEM_CUSTOM_1;
                            soItemPopup.ITEM_CUSTOM_1 = iHeader.ITEM_CUSTOM_1;
                            var itemForGroup = new V_SAP_SALES_ORDER();
                            if (iHeader.PRODUCT_CODE.StartsWith("3"))
                            {
                                soItemPopup.ASSIGN_ID = iHeader.SALES_ORDER_NO + iHeader.ITEM.ToString() + iHeader.PRODUCT_CODE + iHeader.COMPONENT_MATERIAL;

                                itemForGroup = iHeader;
                            }
                            else
                            {
                                itemForGroup = list.Where(m => m.SALES_ORDER_NO == iHeader.SALES_ORDER_NO && m.COMPONENT_MATERIAL == iHeader.PRODUCT_CODE).FirstOrDefault();
                                if (itemForGroup == null)
                                {
                                    itemForGroup = iHeader;
                                }
                            }

                            if (itemForGroup != null)
                            {
                                var WHText = "";
                                var GeneralText = "";

                                var tempWHText = listLongText.Where(m => m.TEXT_NAME == itemForGroup.SALES_ORDER_NO && m.TEXT_ID == itemForGroup.ITEM.ToString() && m.TEXT_LANGUAGE == "W").ToList();
                                if (tempWHText.Count() == 1)
                                {
                                    WHText = tempWHText.FirstOrDefault().LINE_TEXT;
                                }
                                else
                                {
                                    WHText = CNService.GetWHText(itemForGroup.SALES_ORDER_NO, itemForGroup.ITEM.ToString(), context);
                                    listLongText.Add(new SAP_M_LONG_TEXT() { LINE_TEXT = WHText, TEXT_NAME = itemForGroup.SALES_ORDER_NO, TEXT_ID = itemForGroup.ITEM.ToString(), TEXT_LANGUAGE = "W" });
                                }

                                var tempGeneralText = listLongText.Where(m => m.TEXT_NAME == itemForGroup.SALES_ORDER_NO && m.TEXT_LANGUAGE == "G").ToList();
                                if (tempGeneralText.Count() == 1)
                                {
                                    GeneralText = tempGeneralText.FirstOrDefault().LINE_TEXT;
                                }
                                else
                                {
                                    GeneralText = CNService.GetGeneralText(itemForGroup.SALES_ORDER_NO, context);
                                    listLongText.Add(new SAP_M_LONG_TEXT() { LINE_TEXT = GeneralText, TEXT_NAME = itemForGroup.SALES_ORDER_NO, TEXT_LANGUAGE = "G" });
                                }

                                soItemPopup.GROUPING = itemForGroup.BOM_ITEM_CUSTOM_1 + itemForGroup.PRODUCT_CODE + itemForGroup.COMPONENT_MATERIAL
                                                  + WHText
                                                  + GeneralText
                                                  + itemForGroup.SHIP_TO + itemForGroup.SOLD_TO + itemForGroup.COUNTRY + itemForGroup.IN_TRANSIT_TO + itemForGroup.ADDITIONAL_BRAND_ID + itemForGroup.BRAND_ID;

                                //soItemPopup.GROUPING = EncryptionService.EncryptGrouping(soItemPopup.GROUPING);
                                //Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                                //soItemPopup.GROUPING = rgx.Replace(soItemPopup.GROUPING, "");

                                soItemPopup.GROUPING_DISPLAY_TXT = "<b>Product code</b> : " + itemForGroup.PRODUCT_CODE + "<br />" +
                                                                     "<b>Country</b> : " + itemForGroup.COUNTRY + "<br />" +
                                                                     "<b>In transit to</b> : " + itemForGroup.IN_TRANSIT_TO + "<br />" +
                                                                     "<b>Additional brand</b> : " + itemForGroup.ADDITIONAL_BRAND_ID + ":" + itemForGroup.ADDITIONAL_BRAND_DESCRIPTION + "<br />" +
                                                                     "<b>Packaging code(BOM component)</b> : " + itemForGroup.COMPONENT_MATERIAL + "<br />" +
                                                                     "<b>Sold to</b> : " + itemForGroup.SOLD_TO + ":" + itemForGroup.SOLD_TO_NAME + "<br />" +
                                                                     "<b>Ship to</b> : " + itemForGroup.SHIP_TO + ":" + itemForGroup.SHIP_TO_NAME + "<br />" +
                                                                     "<b>Brand</b> : " + itemForGroup.BRAND_ID + ":" + itemForGroup.BRAND_DESCRIPTION + "<br />" +
                                                                     "<b>PKG & warehouse text</b> : " + WHText + "<br />" +
                                                                     "<b>General text</b> : " + GeneralText + "<br />";
                            }

                            listRes.Add(soItemPopup);
                        }

                        var tempProcess = (from p in context.ART_WF_ARTWORK_PROCESS
                                           where p.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                           select p).FirstOrDefault();

                        var tempRequest = (from qq in context.ART_WF_ARTWORK_REQUEST
                                           where qq.ARTWORK_REQUEST_ID == tempProcess.ARTWORK_REQUEST_ID
                                           select qq).FirstOrDefault();

                        var listSaleOrder = listRes.Select(m => m.SO_NUMBER).Distinct().ToList();

                        var listSODetail = (from m in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                            where listSaleOrder.Contains(m.SALES_ORDER_NO)
                                            select m).ToList();

                        var tempList22 = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>();
                        foreach (var item in listRes)
                        {
                            tempList22.Add(item);

                            List<V_SAP_SALES_ORDER_2> chkListFOC = new List<V_SAP_SALES_ORDER_2>();
                            if (tempRequest.TYPE_OF_ARTWORK == "REPEAT")
                            {
                                chkListFOC = (from m in context.V_SAP_SALES_ORDER2
                                              where string.IsNullOrEmpty(m.IS_MIGRATION)
                                              && m.SALES_ORDER_NO == item.SO_NUMBER
                                              && m.ITEM_CUSTOM_1 == item.SO_ITEM_NO.ToString()
                                              && m.PRODUCT_CODE == item.ORDER_BOM_NO
                                              && m.SO_ITEM_IS_ACTIVE == "X"
                                              select new V_SAP_SALES_ORDER_2
                                              {
                                                  SOLD_TO_NAME = m.SOLD_TO_NAME,
                                                  SHIP_TO_NAME = m.SHIP_TO_NAME,
                                                  SALES_ORG = m.SALES_ORG,
                                                  SALES_ORDER_NO = m.SALES_ORDER_NO,
                                                  ITEM = m.ITEM,
                                                  BRAND_DESCRIPTION = m.BRAND_DESCRIPTION,
                                                  PRODUCT_CODE = m.PRODUCT_CODE,
                                                  PORT = m.PORT,
                                                  PRODUCTION_PLANT = m.PRODUCTION_PLANT,
                                                  COUNTRY = m.COUNTRY,
                                                  RDD = m.RDD,
                                                  PO_COMPLETE_SO_ITEM_COMPONENT_ID = m.PO_COMPLETE_SO_ITEM_COMPONENT_ID,
                                                  COMPONENT_MATERIAL = m.COMPONENT_MATERIAL,
                                                  QUANTITY = m.QUANTITY,
                                                  STOCK = m.STOCK,
                                                  DECRIPTION = m.DECRIPTION,
                                                  BOM_ITEM_CUSTOM_1 = m.BOM_ITEM_CUSTOM_1,
                                                  ITEM_CUSTOM_1 = m.ITEM_CUSTOM_1,
                                              }).ToList();
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.ORDER_BOM_NO))
                                {
                                    var twoDigitMat5 = CNService.SubString(item.ORDER_BOM_NO, 2);
                                    chkListFOC = (from m in context.V_SAP_SALES_ORDER2
                                                  where string.IsNullOrEmpty(m.IS_MIGRATION)
                                                  && m.SALES_ORDER_NO == item.SO_NUMBER
                                                  && m.ITEM_CUSTOM_1 == item.SO_ITEM_NO.ToString()
                                                  && m.PRODUCT_CODE.StartsWith(twoDigitMat5)
                                                  && m.SO_ITEM_IS_ACTIVE == "X"
                                                  select new V_SAP_SALES_ORDER_2
                                                  {
                                                      SOLD_TO_NAME = m.SOLD_TO_NAME,
                                                      SHIP_TO_NAME = m.SHIP_TO_NAME,
                                                      SALES_ORG = m.SALES_ORG,
                                                      SALES_ORDER_NO = m.SALES_ORDER_NO,
                                                      ITEM = m.ITEM,
                                                      BRAND_DESCRIPTION = m.BRAND_DESCRIPTION,
                                                      PRODUCT_CODE = m.PRODUCT_CODE,
                                                      PORT = m.PORT,
                                                      PRODUCTION_PLANT = m.PRODUCTION_PLANT,
                                                      COUNTRY = m.COUNTRY,
                                                      RDD = m.RDD,
                                                      PO_COMPLETE_SO_ITEM_COMPONENT_ID = m.PO_COMPLETE_SO_ITEM_COMPONENT_ID,
                                                      COMPONENT_MATERIAL = m.COMPONENT_MATERIAL,
                                                      QUANTITY = m.QUANTITY,
                                                      STOCK = m.STOCK,
                                                      DECRIPTION = m.DECRIPTION,
                                                      BOM_ITEM_CUSTOM_1 = m.BOM_ITEM_CUSTOM_1,
                                                      ITEM_CUSTOM_1 = m.ITEM_CUSTOM_1,
                                                  }).ToList();
                                }
                            }

                            foreach (var itemFOC in chkListFOC)
                            {
                                var soItemPopup = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
                                soItemPopup.SOLD_TO_NAME = itemFOC.SOLD_TO_NAME;
                                soItemPopup.SHIP_TO_NAME = itemFOC.SHIP_TO_NAME;
                                soItemPopup.SALES_ORG = itemFOC.SALES_ORG;
                                soItemPopup.SO_NUMBER = itemFOC.SALES_ORDER_NO;
                                soItemPopup.SO_ITEM_NO = itemFOC.ITEM.ToString();
                                soItemPopup.BRAND = itemFOC.BRAND_DESCRIPTION;
                                soItemPopup.MATERIAL_NO = itemFOC.PRODUCT_CODE;
                                soItemPopup.PORT = itemFOC.PORT;
                                soItemPopup.PRODUCTION_PLANT = itemFOC.PRODUCTION_PLANT;
                                soItemPopup.COUNTRY = itemFOC.COUNTRY;
                                if (itemFOC.RDD != null)
                                {
                                    DateTime dt = Convert.ToDateTime(itemFOC.RDD.ToString());
                                    soItemPopup.RDD_DISPLAY_TXT = dt.ToString(formatDate);
                                }
                                soItemPopup.RDD = itemFOC.RDD;
                                soItemPopup.ORDER_BOM_ID = Convert.ToInt32(itemFOC.PO_COMPLETE_SO_ITEM_COMPONENT_ID);
                                soItemPopup.ORDER_BOM_NO = itemFOC.COMPONENT_MATERIAL;
                                soItemPopup.QUANTITY = itemFOC.QUANTITY.ToString();
                                soItemPopup.STOCK_PO = itemFOC.STOCK;
                                soItemPopup.ORDER_BOM_DESC = itemFOC.DECRIPTION;
                                soItemPopup.ORDER_BOM_ID = Convert.ToInt32(itemFOC.PO_COMPLETE_SO_ITEM_COMPONENT_ID);
                                soItemPopup.BOM_ITEM_CUSTOM_1 = itemFOC.BOM_ITEM_CUSTOM_1;
                                soItemPopup.ITEM_CUSTOM_1 = itemFOC.ITEM_CUSTOM_1;
                                soItemPopup.GROUPING = item.GROUPING;
                                soItemPopup.GROUPING_DISPLAY_TXT = item.GROUPING_DISPLAY_TXT;

                                //if (CNService.IsDevOrQAS())
                                //{
                                var cntFOC = (from m in listSODetail
                                              where m.SALES_ORDER_NO == soItemPopup.SO_NUMBER
                                              && m.SALES_ORDER_ITEM == soItemPopup.SO_ITEM_NO
                                              select m).Count();
                                if (cntFOC == 0)
                                    tempList22.Add(soItemPopup);
                                //}
                                //else
                                //{
                                //    tempList22.Add(soItemPopup);
                                //}
                            }
                        }
                        listRes = tempList22;
                    }
                }
                foreach (var item in listRes)
                {
                    item.GROUPINGTEMP = item.GROUPING;
                }

                var cntGroup = 1;
                foreach (var item in listRes)
                {
                    var found = false;
                    var temp = listRes.Where(m => m.GROUPING == item.GROUPINGTEMP).ToList();
                    foreach (var itemTemp in temp)
                    {
                        found = true;
                        itemTemp.GROUPING = cntGroup.ToString();
                    }
                    if (found) cntGroup++;
                }

                Results.data = listRes;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
                throw;
            }
            return Results;
        }

        

        public static ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT ValidateSalesOrderItemPopup(ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST_LIST param)
        {
            ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT Results = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT();

            try
            {
                string msg = "";
                //string value = "";
                List<string> listValue = new List<string>();
                List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2> listItemExist = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>();
                List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2> listItem = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>();
                ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 item = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
                ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 itemExist = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
                
                if (param == null || param.data == null || param.data.Count <= 0)
                {
                    return Results;
                }
                else
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            var ARTWORK_REQUEST_ID = param.data[0].ARTWORK_REQUEST_ID;
                            var ARTWORK_SUB_ID = param.data[0].ARTWORK_SUB_ID;

                            var listRequestCountryCode = (from qq in context.SAP_M_COUNTRY
                                                          join q2 in context.ART_WF_ARTWORK_REQUEST_COUNTRY on qq.COUNTRY_ID equals q2.COUNTRY_ID
                                                          where q2.ARTWORK_REQUEST_ID == ARTWORK_REQUEST_ID
                                                          select qq).ToList();

                            var processPA = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                             where p.ARTWORK_SUB_ID == ARTWORK_SUB_ID
                                             select p).FirstOrDefault();

                            var xProduct = (from d in context.XECM_M_PRODUCT
                                            where d.XECM_PRODUCT_ID == processPA.PRODUCT_CODE_ID
                                            select d).FirstOrDefault();
                            var listProductCode = (from qq in context.XECM_M_PRODUCT
                                                   join q2 in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT on qq.XECM_PRODUCT_ID equals q2.PRODUCT_CODE_ID
                                                   where q2.ARTWORK_SUB_ID == ARTWORK_SUB_ID
                                                   select qq).ToList();

                            var listProductCodeOther = (from qq in context.ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER
                                                        where qq.ARTWORK_SUB_ID == ARTWORK_SUB_ID
                                                        select qq).ToList();
                            //## 468048
                            List<xecm_product> listproduct = new List<xecm_product>();
                            if(xProduct!=null)
                            listproduct.Add( new xecm_product { PRODUCT_CODE =  xProduct.PRODUCT_CODE });
                            if(listProductCode!=null)
                            foreach (var value in listProductCode)
                            {
                                listproduct.Add(new xecm_product { PRODUCT_CODE = value.PRODUCT_CODE });
                            }
                            var product5 = true;
                            var valid_product5 = true;
                            var valid_product = true;
                            var valid_country = true;
                            var valid_production_plint = true;
                            var valid_bulist = true;
                            var valid_orderbom = true;   // by aof 

                            //var listPlant = (from qq in context.ART_WF_ARTWORK_PROCESS_PA_PLANT join q2 in context.SAP_M_PLANT on qq.PLANT_ID equals q2.PLANT_ID
                            //                 where qq.ARTWORK_SUB_ID == ARTWORK_SUB_ID
                            //                            select q2).ToList();
                            var rqplant = CNService.GetProductionPlants(ARTWORK_REQUEST_ID, ARTWORK_SUB_ID, context);
                            var intArray = new[] {
                                rqplant
                                //processPA.PRODICUTION_PLANT_OTHER 
                            };
                            var listPlant = new List<string>(intArray);


                            foreach (var item2 in param.data)
                            {
                                if (item2.MATERIAL_NO.StartsWith("3"))
                                {
                                    if (processPA != null)
                                    {
                                        //if (
                                        //    //#Assign SO
                                        //    xProduct != null &&
                                        //    !String.IsNullOrEmpty(xProduct.PRODUCT_CODE) &&
                                        //    !xProduct.PRODUCT_CODE.Equals(item2.MATERIAL_NO) &&
                                        //    listProductCode.Where(m => m.PRODUCT_CODE == item2.MATERIAL_NO).FirstOrDefault() == null &&
                                        //    listProductCodeOther.Where(m => m.PRODUCT_CODE == item2.MATERIAL_NO).FirstOrDefault() == null)
                                        //{
                                        //    //valid = false;
                                        //    valid_product = false;
                                        //    break;
                                        //}else 
                                        if(listproduct.Where(m => m.PRODUCT_CODE == item2.MATERIAL_NO).FirstOrDefault() == null)
                                        {
                                            valid_product = false;
                                            break;
                                        }
                                    }
                                }
                                else if (item2.MATERIAL_NO.StartsWith("5"))
                                {
                                    if (processPA.MATERIAL_NO != item2.MATERIAL_NO)
                                    {
                                        valid_product5 = false;
                                        break;
                                    }
                                }

                                if (listRequestCountryCode.Where(m => m.COUNTRY_CODE == item2.COUNTRY).FirstOrDefault() == null)
                                {
                                    //valid = false;
                                    valid_country = false;
                                    break;
                                }
                            }
                            var li = param.data;
                            //var groupedPlantList = li.GroupBy(u => u.PRODUCTION_PLANT)
                            //    .Select (grp => grp.ToList())
                            //    .ToList();
                                
                            var buList = from m in li
                                            group m.PRODUCTION_PLANT.Substring(0,3) by m.PRODUCTION_PLANT.Substring(0, 3) into g
                                            select new { Name = g.Key, KeyCols = g.ToList() };
                            if(buList.Count() > 1)
                            {
                                valid_bulist = false;

                            }else if(buList.Count() == 1) {
                                var pname = buList.FirstOrDefault();
                                var p3 = (from s in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM
                                                    where s.ARTWORK_SUB_ID == ARTWORK_SUB_ID
                                                    select s).ToList();

                                if (p3.Count() != 0){
                                    var x3 = p3.Where(s3 => s3.PRODUCTION_PLANT.Substring(0, 3).Equals(pname.Name)).ToList();
                                    if (x3.Count == 0)
                                        valid_bulist = false;
                                }
                            }
                            if (listPlant[0] != "")
                            {

                                var groupedPlantList = from m in li
                                                       group m.PRODUCTION_PLANT by m.PRODUCTION_PLANT into g
                                                       select new { Name = g.Key, KeyCols = g.ToList() };


                                foreach (var p in groupedPlantList)
                                {
                                    //var plantid = (from qq in context.SAP_M_PLANT where  qq.PLANT == p.Name &&
                                    //    qq.PLANT_ID == plantid.PLANT_ID
                                    //    select qq).FirstOrDefault();
                                    //if(plantid == null)
                                    //{
                                    var charArr = listPlant[0].Split(','); 
                                    var p2 = charArr.Where(fl => p.Name.Contains(fl.ToString())).ToList();
                                    //var p2 = listPlant.Where(m => m.PLANT == p.Name).ToList();
                                    if (p2.Count == 0)
                                    {
                                        valid_production_plint = false;
                                        break;
                                    }
                                    //}
                                }
                                //var count = param.data.Select(m => m.PRODUCTION_PLANT != processPA.PRODICUTION_PLANT_ID).ToList();
                            }
                            //
                            var artworkRequest = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(ARTWORK_REQUEST_ID, context);
                            if (artworkRequest.TYPE_OF_ARTWORK == "REPEAT")
                            {
                                var groupedMatList = from m in param.data
                                                     group m.ORDER_BOM_NO by m.ORDER_BOM_NO into g
                                                     select new { Name = g.Key, KeyCols = g.ToList() };
                                var c = groupedMatList.Select(m => m.Name).ToList().Count;
                                if (c > 1)
                                {
                                    product5 = false;
                                }
                                if (product5)
                                {
                                    var listsoitem = (from s in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM
                                                      where s.ARTWORK_SUB_ID == ARTWORK_SUB_ID && s.PRODUCT_CODE.StartsWith("5")
                                                      select s.PRODUCT_CODE).ToList();

                                    //var listsoBom = (from s in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT
                                    //                 where s.ARTWORK_SUB_ID == ARTWORK_SUB_ID && s.COMPONENT_MATERIAL.StartsWith("5")
                                    //                 select s.COMPONENT_MATERIAL).ToList();
                                    listsoitem.AddRange((from s in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT
                                                         where s.ARTWORK_SUB_ID == ARTWORK_SUB_ID && s.COMPONENT_MATERIAL.StartsWith("5")
                                                         select s.COMPONENT_MATERIAL).ToList());
                                    if (listsoitem.Count > 0)
                                    {
                                        foreach (var p in groupedMatList)
                                        {
                                            if (p.Name == null)  // by aof
                                            {
                                                valid_orderbom = false; // by aof
                                                break; // by aof
                                            }
                                            else
                                            {
                                                var p2 = listsoitem.Where(m => p.Name.Equals(m.ToString())).ToList();
                                                if (p2.Count == 0)
                                                {
                                                    valid_product5 = false;
                                                    break;
                                                }
                                            }

                                        }
                                    }
                                }
                            }

                            if (!valid_orderbom)   // by aof 
                            {
                                item.VALIDATE_MESSAGE = "Cannot find ORDER BOM NO.";
                                listItem.Add(item);

                                Results.status = "S";
                                Results.data = listItem;
                                return Results;
                            }

                            if (!valid_product)
                            {
                                item.VALIDATE_MESSAGE = "Product Code not match with PA Data.";
                                listItem.Add(item);

                                Results.status = "S";
                                Results.data = listItem;
                                return Results;
                            }
                            if (!valid_production_plint)
                            {
                                item.VALIDATE_MESSAGE = "Production Plant not match with PA Data.";
                                listItem.Add(item);

                                Results.status = "S";
                                Results.data = listItem;
                                return Results;
                            }

                            if (!valid_product5)
                            {
                                item.VALIDATE_MESSAGE = "Packaging not match with assigned SO.";
                                listItem.Add(item);

                                Results.status = "S";
                                Results.data = listItem;
                                return Results;
                            }

                            if (!product5)
                            {
                                item.VALIDATE_MESSAGE = "Please select only one packaging.";
                                listItem.Add(item);

                                Results.status = "S";
                                Results.data = listItem;
                                return Results;
                            }
                            if (!valid_bulist)
                            {
                                item.VALIDATE_MESSAGE = "Selected orders are not the same production plant.";
                                listItem.Add(item);

                                Results.status = "S";
                                Results.data = listItem;
                                return Results;
                            }
                            //var artworkRequest = ART_WF_ARTWORK_REQUEST_SERVICE.GetByARTWORK_REQUEST_ID(ARTWORK_REQUEST_ID, context);
                            if (artworkRequest.TYPE_OF_ARTWORK == "NEW")
                            {
                                if (!valid_country)
                                {
                                    item.VALIDATE_MESSAGE = "Country not match with artwork request form.";
                                    listItem.Add(item);

                                    Results.status = "S";
                                    Results.data = listItem;
                                    return Results;
                                }
                            }

                            //var artwork_sub_id = param.data[0].ARTWORK_SUB_ID;
                            //itemExist.ARTWORK_SUB_ID = artwork_sub_id;

                            //listItemExist = MapperServices.ART_WF_ARTWORK_PROCESS_SO_DETAIL(ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(itemExist, context).ToList());

                            //listItemExist.AddRange(param.data);

                            //foreach (ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 data in listItemExist)
                            //{
                            //    string matNO = "";
                            //    string soldToName = "";
                            //    string shipToName = "";
                            //    string brand = "";
                            //    string component = "";

                            //    matNO = data.MATERIAL_NO;
                            //    soldToName = data.SOLD_TO_NAME;
                            //    shipToName = data.SHIP_TO_NAME;
                            //    brand = data.BRAND;

                            //    var soHeader = (from s in context.SAP_M_PO_COMPLETE_SO_HEADER
                            //                    where s.SALES_ORDER_NO == data.SALES_ORDER_NO
                            //                    select s).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                            //    if (soHeader != null)
                            //    {
                            //        decimal soItemNo = 0;

                            //        if (!String.IsNullOrEmpty(data.SALES_ORDER_ITEM))
                            //        {
                            //            soItemNo = Convert.ToDecimal(data.SALES_ORDER_ITEM);
                            //        }

                            //        var soItem = (from s in context.SAP_M_PO_COMPLETE_SO_ITEM
                            //                      where s.PO_COMPLETE_SO_HEADER_ID == soHeader.PO_COMPLETE_SO_HEADER_ID
                            //                          && s.ITEM == soItemNo
                            //                      select s).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                            //        if (String.IsNullOrEmpty(data.BOM_ITEM_CUSTOM_1))
                            //        {
                            //            if (data.BOM_ID != null)
                            //            {
                            //                var soComponent = (from c in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                            //                                   where c.PO_COMPLETE_SO_ITEM_ID == soItem.PO_COMPLETE_SO_ITEM_ID
                            //                                       && c.PO_COMPLETE_SO_ITEM_COMPONENT_ID == data.BOM_ID
                            //                                   select c).OrderByDescending(o => o.UPDATE_DATE).FirstOrDefault();

                            //                if (soComponent != null)
                            //                {
                            //                    component = GetBomCustom1Value(soComponent.BOM_ITEM_CUSTOM_1);
                            //                }
                            //            }
                            //        }
                            //        else
                            //        {
                            //            component = GetBomCustom1Value(data.BOM_ITEM_CUSTOM_1);
                            //        }

                            //        soldToName = soHeader.SOLD_TO_NAME;
                            //        shipToName = soHeader.SHIP_TO_NAME;
                            //        brand = soItem.BRAND_DESCRIPTION;
                            //        matNO = soItem.PRODUCT_CODE;
                            //    }

                            //    value = matNO + ":" + soldToName + ":" + shipToName + ":" + brand + ":" + component;
                            //    listValue.Add(value);
                            //    value = "";
                            //}
                        }
                    }
                }

                item.VALIDATE_MESSAGE = msg;
                listItem.Add(item);

                Results.status = "S";
                Results.data = listItem;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT SaveSalesOrderItemDetail(ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST_LIST param)
        {
            ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT Results = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT();
            ART_WF_ARTWORK_PROCESS_SO_DETAIL detail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
            ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 detail2 = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
            List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2> listDetails = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>();
            SAP_M_PO_COMPLETE_SO_HEADER detailHeaderFOC = new SAP_M_PO_COMPLETE_SO_HEADER();
            SAP_M_PO_COMPLETE_SO_ITEM detailItemFOC = new SAP_M_PO_COMPLETE_SO_ITEM();
            List<SAP_M_PO_COMPLETE_SO_ITEM> listItemFOC = new List<SAP_M_PO_COMPLETE_SO_ITEM>();
            ART_WF_ARTWORK_PROCESS_SO_DETAIL detailTmp = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();

            try
            {
                if (param == null || param.data == null || param.data.Count <= 0)
                {
                    return Results;
                }
                else
                {
                    if (param.data.Count > 0)
                    {
                        using (var context = new ARTWORKEntities())
                        {
                            using (var dbContextTransaction = CNService.IsolationLevel(context))
                            {
                                var currentUserId = CNService.getCurrentUser(context);
                                foreach (ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 data in param.data)
                                {
                                    detail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
                                    detailTmp = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();

                                    detail.ARTWORK_REQUEST_ID = data.ARTWORK_REQUEST_ID;
                                    detail.ARTWORK_SUB_ID = data.ARTWORK_SUB_ID;
                                    detail.SALES_ORDER_NO = data.SALES_ORDER_NO;
                                    detail.SALES_ORDER_ITEM = data.SALES_ORDER_ITEM;
                                    detail.MATERIAL_NO = data.MATERIAL_NO;
                                    detail.BOM_NO = null;
                                    if (data.ORDER_BOM_ID > 0) detail.BOM_ID = data.ORDER_BOM_ID;
                                    detailTmp = ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(detail, context).FirstOrDefault();

                                    if (detailTmp != null)
                                    {
                                        detail.ARTWORK_PROCESS_SO_ID = detailTmp.ARTWORK_PROCESS_SO_ID;
                                    }

                                    detail.CREATE_BY = data.CREATE_BY;
                                    detail.UPDATE_BY = data.UPDATE_BY;

                                    if (detail.CREATE_BY == 0) detail.CREATE_BY = currentUserId;
                                    if (detail.UPDATE_BY == 0) detail.UPDATE_BY = currentUserId;
                                    ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.SaveOrUpdateNoLog(detail, context);

                                    #region "Save FOC Item"
                                    detailHeaderFOC = new SAP_M_PO_COMPLETE_SO_HEADER();
                                    detailItemFOC = new SAP_M_PO_COMPLETE_SO_ITEM();

                                    var soHdr = (from s in context.SAP_M_PO_COMPLETE_SO_HEADER
                                                 where s.SALES_ORDER_NO == data.SALES_ORDER_NO
                                                 select s).FirstOrDefault();

                                    if (soHdr != null)
                                    {
                                        var bom = (from f in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                                                   where f.PO_COMPLETE_SO_ITEM_COMPONENT_ID == data.ORDER_BOM_ID
                                                   select f).ToList().FirstOrDefault();
                                        if (bom != null)
                                        {
                                            var foc = (from f in context.SAP_M_PO_COMPLETE_SO_ITEM
                                                       where f.PO_COMPLETE_SO_HEADER_ID == soHdr.PO_COMPLETE_SO_HEADER_ID
                                                       && f.ITEM_CUSTOM_1 == data.SALES_ORDER_ITEM
                                                       && f.PRODUCT_CODE == bom.COMPONENT_MATERIAL
                                                       && f.IS_ACTIVE == "X"
                                                       select f).ToList();

                                            if (foc.Count == 0)
                                            {
                                                if (!string.IsNullOrEmpty(bom.COMPONENT_MATERIAL))
                                                {
                                                    var twoDigitMat5 = CNService.SubString(bom.COMPONENT_MATERIAL, 2);
                                                    foc = (from f in context.SAP_M_PO_COMPLETE_SO_ITEM
                                                           where f.PO_COMPLETE_SO_HEADER_ID == soHdr.PO_COMPLETE_SO_HEADER_ID
                                                           && f.ITEM_CUSTOM_1 == data.SALES_ORDER_ITEM
                                                           && f.PRODUCT_CODE.StartsWith(twoDigitMat5)
                                                           && f.IS_ACTIVE == "X"
                                                           select f).ToList();
                                                }
                                            }

                                            foreach (var item in foc)
                                            {
                                                detail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();

                                                detail.ARTWORK_REQUEST_ID = data.ARTWORK_REQUEST_ID;
                                                detail.ARTWORK_SUB_ID = data.ARTWORK_SUB_ID;
                                                detail.SALES_ORDER_NO = data.SALES_ORDER_NO;
                                                detail.SALES_ORDER_ITEM = item.ITEM.ToString();
                                                detail.MATERIAL_NO = item.PRODUCT_CODE;
                                                detail.BOM_NO = "FOC";
                                                detail.CREATE_BY = data.CREATE_BY;
                                                detail.UPDATE_BY = data.UPDATE_BY;

                                                if (detail.CREATE_BY == 0) detail.CREATE_BY = currentUserId;
                                                if (detail.UPDATE_BY == 0) detail.UPDATE_BY = currentUserId;

                                                //if (CNService.IsDevOrQAS())
                                                //{
                                                var cntFOC = (from m in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                                              where m.SALES_ORDER_NO == detail.SALES_ORDER_NO
                                                              && m.SALES_ORDER_ITEM == detail.SALES_ORDER_ITEM
                                                              select m).Count();
                                                if (cntFOC == 0)
                                                    ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.SaveOrUpdateNoLog(detail, context);

                                                var listsoheader = "";
                                                //
                                                //}
                                                //else
                                                //{
                                                //    ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.SaveOrUpdateNoLog(detail, context);
                                                //}
                                            }
                                        }
                                    }

                                    #endregion

                                }

                                DeleteAssignSalesOrder(param.data[0], context);
                                CopyAssignSalesOrder(param.data[0], context);

                                dbContextTransaction.Commit();
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

        public static ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT SaveSalesOrderFOC(ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST_LIST param)
        {
            ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT Results = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT();
            ART_WF_ARTWORK_PROCESS_SO_DETAIL detail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
            ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 detail2 = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
            List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2> listDetails = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>();

            SAP_M_PO_COMPLETE_SO_HEADER detailHeaderFOC = new SAP_M_PO_COMPLETE_SO_HEADER();
            SAP_M_PO_COMPLETE_SO_ITEM detailItemFOC = new SAP_M_PO_COMPLETE_SO_ITEM();
            List<SAP_M_PO_COMPLETE_SO_ITEM> listItemFOC = new List<SAP_M_PO_COMPLETE_SO_ITEM>();

            ART_WF_ARTWORK_PROCESS_SO_DETAIL detailTmp = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();

            try
            {
                if (param == null || param.data == null || param.data.Count <= 0)
                {
                    return Results;
                }
                else
                {
                    if (param.data.Count > 0)
                    {
                        using (var context = new ARTWORKEntities())
                        {
                            using (var dbContextTransaction = CNService.IsolationLevel(context))
                            {
                                var currentUserId = CNService.getCurrentUser(context);
                                foreach (ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 data in param.data)
                                {
                                    detail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
                                    detailTmp = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();

                                    detail.ARTWORK_REQUEST_ID = data.ARTWORK_REQUEST_ID;
                                    detail.ARTWORK_SUB_ID = data.ARTWORK_SUB_ID;
                                    detail.SALES_ORDER_NO = data.SALES_ORDER_NO;
                                    detail.SALES_ORDER_ITEM = data.SALES_ORDER_ITEM;
                                    detail.MATERIAL_NO = data.MATERIAL_NO;
                                    detail.BOM_NO = "FOC_3V";
                                    // detail.BOM_ID = data.ORDER_BOM_ID;

                                    detailTmp = ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(detail, context).FirstOrDefault();

                                    if (detailTmp != null)
                                    {
                                        detail.ARTWORK_PROCESS_SO_ID = detailTmp.ARTWORK_PROCESS_SO_ID;
                                    }

                                    detail.CREATE_BY = data.CREATE_BY;
                                    detail.UPDATE_BY = data.UPDATE_BY;

                                    if (detail.CREATE_BY == 0) detail.CREATE_BY = currentUserId;
                                    if (detail.UPDATE_BY == 0) detail.UPDATE_BY = currentUserId;
                                    ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.SaveOrUpdateNoLog(detail, context);
                                }

                                DeleteAssignSalesOrder(param.data[0], context);
                                CopyAssignSalesOrder(param.data[0], context);

                                dbContextTransaction.Commit();
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

        public static ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT DeleteSalesOrderItemDetail(ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT Results = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT();
            ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 _detail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
            List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2> _listDetails = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>();

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
                            if (param.data.ARTWORK_PROCESS_SO_ID > 0)
                            {
                                var so = ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByARTWORK_PROCESS_SO_ID(param.data.ARTWORK_PROCESS_SO_ID, context);

                                var requestSORepeat = (from s in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                                       where s.ARTWORK_REQUEST_ID == param.data.ARTWORK_REQUEST_ID
                                                       && s.SALES_ORDER_NO == so.SALES_ORDER_NO
                                                       && s.SALES_ORDER_ITEM == so.SALES_ORDER_ITEM
                                                       select s).FirstOrDefault();

                                if (requestSORepeat != null)
                                {
                                    ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_SERVICE.DeleteByARTWORK_SALES_ORDER_REPEAT_ID(requestSORepeat.ARTWORK_SALES_ORDER_REPEAT_ID, context);
                                }
                                ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.DeleteByARTWORK_PROCESS_SO_ID(so.ARTWORK_PROCESS_SO_ID, context);


                                var temp = (from s in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
                                            where s.ARTWORK_PROCESS_SO_ID == param.data.ARTWORK_PROCESS_SO_ID
                                            select s).FirstOrDefault();
                                if (temp != null)
                                {
                                    var temp2 = (from s in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
                                                 where s.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                                 && s.SALES_ORDER_NO == temp.SALES_ORDER_NO
                                                 select s).ToList();
                                    if (temp2.Count == 0 || temp2.Count == 1)
                                    {
                                        var listLongTxt = (from s in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT
                                                           where s.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                                           select s).ToList();
                                        foreach (var item in listLongTxt)
                                        {
                                            ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_SERVICE.DeleteByASSIGN_SO_LONG_TEXT_ID(item.ASSIGN_SO_LONG_TEXT_ID, context);
                                        }
                                    }
                                }

                                var listHeader = (from s in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
                                                  where s.ARTWORK_PROCESS_SO_ID == param.data.ARTWORK_PROCESS_SO_ID
                                                  select s).ToList();
                                foreach (var item in listHeader)
                                {
                                    ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_SERVICE.DeleteByASSIGN_SO_HEADER_ID(item.ASSIGN_SO_HEADER_ID, context);

                                }

                                var listItem = (from s in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM
                                                where s.ARTWORK_PROCESS_SO_ID == param.data.ARTWORK_PROCESS_SO_ID
                                                select s).ToList();
                                foreach (var item in listItem)
                                {
                                    ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_SERVICE.DeleteByASSIGN_SO_ITEM_ID(item.ASSIGN_SO_ITEM_ID, context);
                                }

                                var listBom = (from s in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT
                                               where s.ARTWORK_PROCESS_SO_ID == param.data.ARTWORK_PROCESS_SO_ID
                                               select s).ToList();
                                foreach (var item in listBom)
                                {
                                    ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_SERVICE.DeleteByASSIGN_SO_ITEM_COMPONENT_ID(item.ASSIGN_SO_ITEM_COMPONENT_ID, context);
                                }

                                //unassign
                                if (listHeader.Count > 0)
                                {
                                    var h = listHeader.FirstOrDefault();
                                    var so_header = (from s in context.SAP_M_PO_COMPLETE_SO_HEADER
                                                     where s.SALES_ORDER_NO == h.SALES_ORDER_NO
                                                     select s).FirstOrDefault();

                                    so_header.IS_ASSIGN = null;
                                    SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.UpdateNoLog(so_header, context);
                                    if (listItem.Count > 0)
                                    {
                                        foreach (var item in listItem)
                                        {
                                            var so_item_value = (from s in context.SAP_M_PO_COMPLETE_SO_ITEM
                                                                 where s.PO_COMPLETE_SO_HEADER_ID == so_header.PO_COMPLETE_SO_HEADER_ID
                                                                 && s.ITEM == item.ITEM
                                                                 select s).FirstOrDefault();

                                            so_item_value.IS_ASSIGN = null;
                                            SAP_M_PO_COMPLETE_SO_ITEM_SERVICE.UpdateNoLog(so_item_value, context);

                                            if (listBom.Count > 0)
                                            {
                                                foreach (var b in listBom)
                                                {
                                                    var so_bom = (from s in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                                                                  where b.COMPONENT_ITEM.Equals(s.COMPONENT_ITEM)
                                                                  && so_item_value.PO_COMPLETE_SO_ITEM_ID.Equals(s.PO_COMPLETE_SO_ITEM_ID)
                                                                  select s).ToList();

                                                    foreach (var bom in so_bom)
                                                    {
                                                        bom.IS_ASSIGN = null;
                                                        SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_SERVICE.UpdateNoLog(bom, context);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            else if (param.data.ARTWORK_REQUEST_ID > 0 && param.data.ARTWORK_SUB_ID > 0)
                            {
                                ART_WF_ARTWORK_PROCESS_SO_DETAIL soDetail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
                                List<ART_WF_ARTWORK_PROCESS_SO_DETAIL> listSODetail = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL>();
                                soDetail.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;
                                soDetail.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                listSODetail = ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(soDetail, context);
                                foreach (ART_WF_ARTWORK_PROCESS_SO_DETAIL item in listSODetail)
                                {
                                    var requestSORepeat = (from s in context.ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT
                                                           where s.ARTWORK_REQUEST_ID == param.data.ARTWORK_REQUEST_ID
                                                           && s.SALES_ORDER_NO == item.SALES_ORDER_NO
                                                           && s.SALES_ORDER_ITEM == item.SALES_ORDER_ITEM
                                                           select s).FirstOrDefault();

                                    if (requestSORepeat != null)
                                    {
                                        ART_WF_ARTWORK_REQUEST_SALES_ORDER_REPEAT_SERVICE.DeleteByARTWORK_SALES_ORDER_REPEAT_ID(requestSORepeat.ARTWORK_SALES_ORDER_REPEAT_ID, context);
                                    }

                                    ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.DeleteByARTWORK_PROCESS_SO_ID(item.ARTWORK_PROCESS_SO_ID, context);
                                }

                                ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 soRequest = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
                                soRequest.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                                soRequest.ARTWORK_REQUEST_ID = param.data.ARTWORK_REQUEST_ID;
                                soRequest.UPDATE_BY = param.data.UPDATE_BY;
                                DeleteAssignSalesOrder(soRequest, context);
                                CopyAssignSalesOrder(soRequest, context);
                            }
                            dbContextTransaction.Commit();
                        }
                    }
                }

                Results.status = "S";
                Results.data = _listDetails;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT AcceptSalesOrderItemChange(ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT Results = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT();
            ART_WF_ARTWORK_PROCESS_SO_DETAIL soDetail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();

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
                            DeleteAssignSalesOrder(param.data, context);
                            CopyAssignSalesOrder(param.data, context);

                            dbContextTransaction.Commit();
                        }
                    }

                    using (var context = new ARTWORKEntities())
                    {
                        ART_SYS_LOG Item = new ART_SYS_LOG();
                        Item.ACTION = "Accept Changes";
                        Item.TABLE_NAME = "";
                        Item.NEW_VALUE = "ARTWORK_SUB_ID : " + param.data.ARTWORK_SUB_ID.ToString();
                        Item.UPDATE_DATE = DateTime.Now;
                        Item.UPDATE_BY = param.data.UPDATE_BY;
                        Item.CREATE_DATE = DateTime.Now;
                        Item.CREATE_BY = param.data.UPDATE_BY;
                        context.ART_SYS_LOG.Add(Item);
                        context.SaveChanges();
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

        public static SALES_ORDER_CHANGE_RESULT CheckSalesOrderChange(SALES_ORDER_CHANGE_REQUEST param)
        {
            SALES_ORDER_CHANGE_RESULT Results = new SALES_ORDER_CHANGE_RESULT();
            ART_WF_ARTWORK_PROCESS_SO_DETAIL soDetail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
            SALES_ORDER_CHANGE soChange = new SALES_ORDER_CHANGE();
            List<SALES_ORDER_CHANGE> soChangeList = new List<SALES_ORDER_CHANGE>();

            Dictionary<string, string> dicFieldsName = new Dictionary<string, string>();

            //Fields Header
            dicFieldsName.Add("SALES_ORDER_NO", "SALES ORDER NO");
            dicFieldsName.Add("SOLD_TO", "SOLD TO");
            dicFieldsName.Add("SOLD_TO_NAME", "SOLD TO NAME");
            dicFieldsName.Add("LAST_SHIPMENT_DATE", "LAST SHIPMENT DATE");
            dicFieldsName.Add("DATE_1_2", "DATE 1 2");
            dicFieldsName.Add("CREATE_ON", "CREATE ON");
            dicFieldsName.Add("RDD", "RDD");
            dicFieldsName.Add("PAYMENT_TERM", "PAYMENT TERM");
            dicFieldsName.Add("LC_NO", "LC NO");
            dicFieldsName.Add("EXPIRED_DATE", "EXPIRED DATE");
            dicFieldsName.Add("SHIP_TO", "SHIP TO");
            dicFieldsName.Add("SHIP_TO_NAME", "SHIP TO NAME");
            dicFieldsName.Add("SOLD_TO_PO", "SOLD TO PO");
            dicFieldsName.Add("SHIP_TO_PO", "SHIP TO PO");
            dicFieldsName.Add("SALES_GROUP", "SALES GROUP");
            dicFieldsName.Add("MARKETING_CO", "MARKETING CO");
            dicFieldsName.Add("MARKETING_CO_NAME", "MARKETING CO NAME");
            dicFieldsName.Add("MARKETING", "MARKETING");
            dicFieldsName.Add("MARKETING_NAME", "MARKETING NAME");
            dicFieldsName.Add("MARKETING_ORDER_SAP", "MARKETING ORDER SAP");
            dicFieldsName.Add("MARKETING_ORDER_SAP_NAME", "MARKETING ORDER SAP NAME");
            dicFieldsName.Add("SALES_ORG", "SALES ORG");
            dicFieldsName.Add("DISTRIBUTION_CHANNEL", "DISTRIBUTION CHANNEL");
            dicFieldsName.Add("DIVITION", "DIVITION");
            dicFieldsName.Add("SALES_ORDER_TYPE", "SALES ORDER TYPE");
            dicFieldsName.Add("HEADER_CUSTOM_1", "HEADER CUSTOM 1");
            dicFieldsName.Add("HEADER_CUSTOM_2", "HEADER CUSTOM 2");
            dicFieldsName.Add("HEADER_CUSTOM_3", "HEADER CUSTOM 3");

            //Fields Item
            dicFieldsName.Add("ITEM", "ITEM");
            dicFieldsName.Add("PRODUCT_CODE", "PRODUCT CODE");
            dicFieldsName.Add("MATERIAL_DESCRIPTION", "MATERIAL DESCRIPTION");
            dicFieldsName.Add("NET_WEIGHT", "NET WEIGHT");
            dicFieldsName.Add("ORDER_QTY", "ORDER QTY");
            dicFieldsName.Add("ORDER_UNIT", "ORDER UNIT");
            dicFieldsName.Add("ETD_DATE_FROM", "ETD DATE FROM");
            dicFieldsName.Add("ETD_DATE_TO", "ETD DATE TO");
            dicFieldsName.Add("PLANT", "PLANT");
            dicFieldsName.Add("OLD_MATERIAL_CODE", "OLD MATERIAL CODE");
            dicFieldsName.Add("PACK_SIZE", "PACK SIZE");
            dicFieldsName.Add("VALUME_PER_UNIT", "VALUME PER UNIT");
            dicFieldsName.Add("VALUME_UNIT", "VALUME UNIT");
            dicFieldsName.Add("SIZE_DRAIN_WT", "SIZE DRAIN WT");
            dicFieldsName.Add("PROD_INSP_MEMO", "PROD INSP MEMO");
            dicFieldsName.Add("REJECTION_CODE", "REJECTION CODE");
            dicFieldsName.Add("REJECTION_DESCRIPTION", "REJECTION DESCRIPTION");
            dicFieldsName.Add("PORT", "PORT");
            dicFieldsName.Add("VIA", "VIA");
            dicFieldsName.Add("IN_TRANSIT_TO", "IN TRANSIT TO");
            dicFieldsName.Add("BRAND_ID", "BRAND ID");
            dicFieldsName.Add("BRAND_DESCRIPTION", "BRAND DESCRIPTION");
            dicFieldsName.Add("ADDITIONAL_BRAND_ID", "ADDITIONAL BRAND ID");
            dicFieldsName.Add("ADDITIONAL_BRAND_DESCRIPTION", "ADDITIONAL BRAND DESCRIPTION");
            dicFieldsName.Add("PRODUCTION_PLANT", "PRODUCTION PLANT");
            dicFieldsName.Add("ZONE", "ZONE");
            dicFieldsName.Add("COUNTRY", "COUNTRY");
            dicFieldsName.Add("PRODUCTION_HIERARCHY", "PRODUCTION HIERARCHY");
            dicFieldsName.Add("MRP_CONTROLLER", "MRP CONTROLLER");
            dicFieldsName.Add("STOCK", "STOCK");
            dicFieldsName.Add("ITEM_CUSTOM_1", "ITEM CUSTOM 1");
            dicFieldsName.Add("ITEM_CUSTOM_2", "SOLD TO PO");
            dicFieldsName.Add("ITEM_CUSTOM_3", "SHIP TO PO");

            //Fields Component
            dicFieldsName.Add("COMPONENT_ITEM", "COMPONENT_ITEM");
            dicFieldsName.Add("COMPONENT_MATERIAL", "COMPONENT_MATERIAL");
            dicFieldsName.Add("DECRIPTION", "DECRIPTION");
            dicFieldsName.Add("QUANTITY", "QUANTITY");
            dicFieldsName.Add("UNIT", "UNIT");
            dicFieldsName.Add("COMPONENT_STOCK", "COMPONENT STOCK");
            dicFieldsName.Add("BOM_ITEM_CUSTOM_1", "BOM ITEM CUSTOM 1");
            dicFieldsName.Add("BOM_ITEM_CUSTOM_2", "BOM ITEM CUSTOM 2");
            dicFieldsName.Add("BOM_ITEM_CUSTOM_3", "BOM ITEM CUSTOM 3");

            dicFieldsName.Add("GENERAL_TEXT", "GENERAL TEXT");
            dicFieldsName.Add("WAREHOUSE_TEXT", "WAREHOUSE TEXT");


            if (param != null && param.data != null)
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT assignSOItemBom = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT();
                            SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT sapSOBom = new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT();

                            soDetail.ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID;
                            var soDetailList = ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(soDetail, context);

                            //for performance 
                            var listSALES_ORDER_NO = soDetailList.Select(m => m.SALES_ORDER_NO).ToList();
                            var listSapSOHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                                   where listSALES_ORDER_NO.Contains(h.SALES_ORDER_NO)
                                                   select h).ToList();

                            var listPO_COMPLETE_SO_HEADER_ID = listSapSOHeader.Select(m => m.PO_COMPLETE_SO_HEADER_ID).ToList();
                            var listsapSOItem = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
                                                 where listPO_COMPLETE_SO_HEADER_ID.Contains(i.PO_COMPLETE_SO_HEADER_ID)
                                                 select i).ToList();

                            var listPO_COMPLETE_SO_ITEM_ID = listsapSOItem.Select(m => m.PO_COMPLETE_SO_ITEM_ID).ToList();
                            var listSapSOBom = (from h in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                                                where listPO_COMPLETE_SO_ITEM_ID.Contains(h.PO_COMPLETE_SO_ITEM_ID)
                                                select h).ToList();


                            var listassignSOHeader = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
                                                      where h.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                                      select h).ToList();

                            var listassignSOItem = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM
                                                    where h.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                                    select h).ToList();

                            var listassignSOItemBom = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT
                                                       where h.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                                       select h).ToList();


                            var listSALES_ORDER_NO2 = soDetailList.Select(m => m.SALES_ORDER_NO.PadLeft(10, '0')).ToList();

                            var listSAP_M_LONG_TEXT = (from h in context.SAP_M_LONG_TEXT
                                                       where listSALES_ORDER_NO2.Contains(h.TEXT_NAME.Substring(0, 10))
                                                       select h).ToList();

                            var listSAP_M_LONG_TEXT_ASSIGN = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT
                                                              where h.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID
                                                              select h).ToList();

                            foreach (ART_WF_ARTWORK_PROCESS_SO_DETAIL iSO in soDetailList)
                            {
                                assignSOItemBom = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT();
                                sapSOBom = new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT();
                                string group = "";

                                group = iSO.SALES_ORDER_NO + "(" + iSO.SALES_ORDER_ITEM.ToString() + ")";

                                var sapSOHeader = listSapSOHeader.Where(m => m.SALES_ORDER_NO == iSO.SALES_ORDER_NO).FirstOrDefault();

                                decimal itemNO = 0;
                                itemNO = Convert.ToDecimal(iSO.SALES_ORDER_ITEM);

                                var sapSOItem = listsapSOItem.Where(m => m.PO_COMPLETE_SO_HEADER_ID == sapSOHeader.PO_COMPLETE_SO_HEADER_ID
                                                                    && m.ITEM == itemNO).FirstOrDefault();

                                if (iSO.BOM_ID > 0)
                                {
                                    sapSOBom = listSapSOBom.Where(m => m.PO_COMPLETE_SO_ITEM_ID == sapSOItem.PO_COMPLETE_SO_ITEM_ID
                                                                    && m.PO_COMPLETE_SO_ITEM_COMPONENT_ID == iSO.BOM_ID).FirstOrDefault();

                                }

                                var assignSOHeader = listassignSOHeader.Where(m => m.ARTWORK_PROCESS_SO_ID == iSO.ARTWORK_PROCESS_SO_ID
                                                                                && m.SALES_ORDER_NO == iSO.SALES_ORDER_NO
                                                                                && m.ARTWORK_SUB_ID == iSO.ARTWORK_SUB_ID).FirstOrDefault();


                                itemNO = Convert.ToDecimal(iSO.SALES_ORDER_ITEM);
                                var assignSOItem = listassignSOItem.Where(m => m.ARTWORK_PROCESS_SO_ID == iSO.ARTWORK_PROCESS_SO_ID
                                                                            && m.ITEM == itemNO
                                                                            && m.ARTWORK_SUB_ID == iSO.ARTWORK_SUB_ID).FirstOrDefault();

                                if (iSO.BOM_ID > 0)
                                {
                                    assignSOItemBom = listassignSOItemBom.Where(m => m.ARTWORK_PROCESS_SO_ID == iSO.ARTWORK_PROCESS_SO_ID && m.ASSIGN_SO_ITEM_ID == assignSOItem.ASSIGN_SO_ITEM_ID && m.ARTWORK_SUB_ID == iSO.ARTWORK_SUB_ID).FirstOrDefault();
                                    //assignSOItemBom = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT
                                    //                   where h.ARTWORK_PROCESS_SO_ID == iSO.ARTWORK_PROCESS_SO_ID
                                    //                    && h.ASSIGN_SO_ITEM_ID == assignSOItem.ASSIGN_SO_ITEM_ID
                                    //                    && h.ARTWORK_SUB_ID == iSO.ARTWORK_SUB_ID
                                    //                   select h).FirstOrDefault();
                                }

                                #region "Header"
                                if (!assignSOHeader.SALES_ORDER_NO.Trim().Equals(sapSOHeader.SALES_ORDER_NO.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["SALES_ORDER_NO"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.SALES_ORDER_NO.Trim();
                                    soChange.NEW_VALUE = sapSOHeader.SALES_ORDER_NO.Trim();
                                }

                                if (!assignSOHeader.SOLD_TO.Trim().Equals(sapSOHeader.SOLD_TO.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["SOLD_TO"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.SOLD_TO.Trim();
                                    soChange.NEW_VALUE = sapSOHeader.SOLD_TO.Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.SOLD_TO_NAME.Trim().Equals(sapSOHeader.SOLD_TO_NAME.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["SOLD_TO_NAME"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.SOLD_TO_NAME.Trim();
                                    soChange.NEW_VALUE = sapSOHeader.SOLD_TO_NAME.Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.LAST_SHIPMENT_DATE.Equals(sapSOHeader.LAST_SHIPMENT_DATE))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["LAST_SHIPMENT_DATE"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.LAST_SHIPMENT_DATE.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.LAST_SHIPMENT_DATE.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.DATE_1_2.Equals(sapSOHeader.DATE_1_2))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["DATE_1_2"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.DATE_1_2.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.DATE_1_2.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.CREATE_ON.Equals(sapSOHeader.CREATE_ON))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["CREATE_ON"].ToString();

                                    if (assignSOHeader.CREATE_ON != null)
                                    {
                                        DateTime dt1 = Convert.ToDateTime(assignSOHeader.CREATE_ON);
                                        soChange.OLD_VALUE = dt1.ToString(formatDate);
                                    }

                                    if (sapSOHeader.CREATE_ON != null)
                                    {
                                        DateTime dt1 = Convert.ToDateTime(sapSOHeader.CREATE_ON);
                                        soChange.NEW_VALUE = dt1.ToString(formatDate);
                                    }

                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.RDD.Equals(sapSOHeader.RDD))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["RDD"].ToString();

                                    if (assignSOHeader.RDD != null)
                                    {
                                        DateTime dt1 = Convert.ToDateTime(assignSOHeader.RDD.ToString());
                                        soChange.OLD_VALUE = dt1.ToString(formatDate);
                                    }

                                    if (sapSOHeader.RDD != null)
                                    {
                                        DateTime dt2 = Convert.ToDateTime(sapSOHeader.RDD.ToString());
                                        soChange.NEW_VALUE = dt2.ToString(formatDate);
                                    }


                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.PAYMENT_TERM.Trim().Equals(sapSOHeader.PAYMENT_TERM.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["PAYMENT_TERM"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.PAYMENT_TERM.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.PAYMENT_TERM.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.LC_NO.Trim().Equals(sapSOHeader.LC_NO.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["LC_NO"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.LC_NO.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.LC_NO.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.EXPIRED_DATE.Equals(sapSOHeader.EXPIRED_DATE))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["EXPIRED_DATE"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.EXPIRED_DATE.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.EXPIRED_DATE.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.SHIP_TO.Trim().Equals(sapSOHeader.SHIP_TO.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["SHIP_TO"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.SHIP_TO.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.SHIP_TO.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.SHIP_TO_NAME.Trim().Equals(sapSOHeader.SHIP_TO_NAME.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["SHIP_TO_NAME"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.SHIP_TO_NAME.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.SHIP_TO_NAME.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.SOLD_TO_PO.Trim().Equals(sapSOHeader.SOLD_TO_PO.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["SOLD_TO_PO"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.SOLD_TO_PO.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.SOLD_TO_PO.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.SHIP_TO_PO.Trim().Equals(sapSOHeader.SHIP_TO_PO.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["SHIP_TO_PO"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.SHIP_TO_PO.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.SHIP_TO_PO.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.SALES_GROUP.Trim().Equals(sapSOHeader.SALES_GROUP.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["SALES_GROUP"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.SALES_GROUP.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.SALES_GROUP.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.MARKETING_CO.Trim().Equals(sapSOHeader.MARKETING_CO.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["MARKETING_CO"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.MARKETING_CO.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.MARKETING_CO.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.MARKETING_CO_NAME.Trim().Equals(sapSOHeader.MARKETING_CO_NAME.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["MARKETING_CO_NAME"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.MARKETING_CO_NAME.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.MARKETING_CO_NAME.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.MARKETING.Trim().Equals(sapSOHeader.MARKETING.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["MARKETING"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.MARKETING.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.MARKETING.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.MARKETING_NAME.Trim().Equals(sapSOHeader.MARKETING_NAME.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["MARKETING_NAME"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.MARKETING_NAME.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.MARKETING_NAME.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.MARKETING_ORDER_SAP.Trim().Equals(sapSOHeader.MARKETING_ORDER_SAP.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["MARKETING_ORDER_SAP"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.MARKETING_ORDER_SAP.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.MARKETING_ORDER_SAP.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.MARKETING_ORDER_SAP_NAME.Equals(sapSOHeader.MARKETING_ORDER_SAP_NAME))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["MARKETING_ORDER_SAP_NAME"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.MARKETING_ORDER_SAP_NAME.ToString();
                                    soChange.NEW_VALUE = sapSOHeader.MARKETING_ORDER_SAP_NAME.ToString();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.DISTRIBUTION_CHANNEL.Trim().Equals(sapSOHeader.DISTRIBUTION_CHANNEL.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["DISTRIBUTION_CHANNEL"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.DISTRIBUTION_CHANNEL.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.DISTRIBUTION_CHANNEL.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.DIVITION.Trim().Equals(sapSOHeader.DIVITION.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["DIVITION"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.DIVITION.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.DIVITION.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.SALES_ORDER_TYPE.Trim().Equals(sapSOHeader.SALES_ORDER_TYPE.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["SALES_ORDER_TYPE"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.SALES_ORDER_TYPE.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.SALES_ORDER_TYPE.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.HEADER_CUSTOM_1.Trim().Equals(sapSOHeader.HEADER_CUSTOM_1.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["HEADER_CUSTOM_1"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.HEADER_CUSTOM_1.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.HEADER_CUSTOM_1.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.HEADER_CUSTOM_2.Equals(sapSOHeader.HEADER_CUSTOM_2))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["HEADER_CUSTOM_2"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.HEADER_CUSTOM_2.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.HEADER_CUSTOM_2.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOHeader.HEADER_CUSTOM_3.Trim().Equals(sapSOHeader.HEADER_CUSTOM_3.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["HEADER_CUSTOM_3"].ToString();
                                    soChange.OLD_VALUE = assignSOHeader.HEADER_CUSTOM_3.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOHeader.HEADER_CUSTOM_3.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }
                                #endregion

                                #region "Item"
                                if (!assignSOItem.ITEM.Equals(sapSOItem.ITEM))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["ITEM"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.ITEM.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.ITEM.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.PRODUCT_CODE.Trim().Equals(sapSOItem.PRODUCT_CODE.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["PRODUCT_CODE"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.PRODUCT_CODE.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.PRODUCT_CODE.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.MATERIAL_DESCRIPTION.Trim().Equals(sapSOItem.MATERIAL_DESCRIPTION.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["MATERIAL_DESCRIPTION"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.MATERIAL_DESCRIPTION.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.MATERIAL_DESCRIPTION.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.NET_WEIGHT.Trim().Equals(sapSOItem.NET_WEIGHT.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["NET_WEIGHT"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.NET_WEIGHT.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.NET_WEIGHT.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.ORDER_QTY.Equals(sapSOItem.ORDER_QTY))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["ORDER_QTY"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.ORDER_QTY.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.ORDER_QTY.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.ORDER_UNIT.Trim().Equals(sapSOItem.ORDER_UNIT.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["ORDER_UNIT"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.ORDER_UNIT.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.ORDER_UNIT.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.ETD_DATE_FROM.Equals(sapSOItem.ETD_DATE_FROM))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["ETD_DATE_FROM"].ToString();

                                    if (assignSOItem.ETD_DATE_FROM != null)
                                    {
                                        DateTime dt1 = Convert.ToDateTime(assignSOItem.ETD_DATE_FROM);
                                        soChange.OLD_VALUE = dt1.ToString(formatDate);
                                    }

                                    if (sapSOItem.ETD_DATE_FROM != null)
                                    {
                                        DateTime dt1 = Convert.ToDateTime(sapSOItem.ETD_DATE_FROM);
                                        soChange.NEW_VALUE = dt1.ToString(formatDate);
                                    }

                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.ETD_DATE_TO.Equals(sapSOItem.ETD_DATE_TO))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["ETD_DATE_TO"].ToString();

                                    if (assignSOItem.ETD_DATE_TO != null)
                                    {
                                        DateTime dt1 = Convert.ToDateTime(assignSOItem.ETD_DATE_TO);
                                        soChange.OLD_VALUE = dt1.ToString(formatDate);
                                    }

                                    if (sapSOItem.ETD_DATE_TO != null)
                                    {
                                        DateTime dt1 = Convert.ToDateTime(sapSOItem.ETD_DATE_TO);
                                        soChange.NEW_VALUE = dt1.ToString(formatDate);
                                    }

                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.PLANT.Trim().Equals(sapSOItem.PLANT.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["PLANT"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.PLANT.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.PLANT.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.OLD_MATERIAL_CODE.Trim().Equals(sapSOItem.OLD_MATERIAL_CODE.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["OLD_MATERIAL_CODE"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.OLD_MATERIAL_CODE.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.OLD_MATERIAL_CODE.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.PACK_SIZE.Trim().Equals(sapSOItem.PACK_SIZE.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["PACK_SIZE"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.PACK_SIZE.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.PACK_SIZE.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.VALUME_PER_UNIT.Trim().Equals(sapSOItem.VALUME_PER_UNIT.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["VALUME_PER_UNIT"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.VALUME_PER_UNIT.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.VALUME_PER_UNIT.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.VALUME_UNIT.Trim().Equals(sapSOItem.VALUME_UNIT.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["VALUME_UNIT"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.VALUME_UNIT.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.VALUME_UNIT.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.SIZE_DRAIN_WT.Trim().Equals(sapSOItem.SIZE_DRAIN_WT.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["SIZE_DRAIN_WT"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.SIZE_DRAIN_WT.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.SIZE_DRAIN_WT.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.PROD_INSP_MEMO.Trim().Equals(sapSOItem.PROD_INSP_MEMO.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["PROD_INSP_MEMO"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.PROD_INSP_MEMO.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.PROD_INSP_MEMO.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.REJECTION_CODE.Trim().Equals(sapSOItem.REJECTION_CODE.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["REJECTION_CODE"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.REJECTION_CODE.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.REJECTION_CODE.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.REJECTION_DESCRIPTION.Trim().Equals(sapSOItem.REJECTION_DESCRIPTION.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["REJECTION_DESCRIPTION"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.REJECTION_DESCRIPTION.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.REJECTION_DESCRIPTION.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.PORT.Trim().Equals(sapSOItem.PORT.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["PORT"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.PORT.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.PORT.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.VIA.Trim().Equals(sapSOItem.VIA.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["VIA"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.VIA.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.VIA.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.IN_TRANSIT_TO.Trim().Equals(sapSOItem.IN_TRANSIT_TO.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["IN_TRANSIT_TO"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.IN_TRANSIT_TO.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.IN_TRANSIT_TO.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.BRAND_ID.Trim().Equals(sapSOItem.BRAND_ID.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["BRAND_ID"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.BRAND_ID.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.BRAND_ID.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.BRAND_DESCRIPTION.Trim().Equals(sapSOItem.BRAND_DESCRIPTION.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["BRAND_DESCRIPTION"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.BRAND_DESCRIPTION.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.BRAND_DESCRIPTION.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.ADDITIONAL_BRAND_ID.Trim().Equals(sapSOItem.ADDITIONAL_BRAND_ID.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["ADDITIONAL_BRAND_ID"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.ADDITIONAL_BRAND_ID.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.ADDITIONAL_BRAND_ID.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.ADDITIONAL_BRAND_DESCRIPTION.Trim().Equals(sapSOItem.ADDITIONAL_BRAND_DESCRIPTION.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["ADDITIONAL_BRAND_DESCRIPTION"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.ADDITIONAL_BRAND_DESCRIPTION.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.ADDITIONAL_BRAND_DESCRIPTION.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.PRODUCTION_PLANT.Trim().Equals(sapSOItem.PRODUCTION_PLANT.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["PRODUCTION_PLANT"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.PRODUCTION_PLANT.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.PRODUCTION_PLANT.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.ZONE.Trim().Equals(sapSOItem.ZONE.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["ZONE"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.ZONE.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.ZONE.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.COUNTRY.Trim().Equals(sapSOItem.COUNTRY.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["COUNTRY"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.COUNTRY.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.COUNTRY.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.PRODUCTION_HIERARCHY.Trim().Equals(sapSOItem.PRODUCTION_HIERARCHY.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["PRODUCTION_HIERARCHY"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.PRODUCTION_HIERARCHY.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.PRODUCTION_HIERARCHY.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.MRP_CONTROLLER.Trim().Equals(sapSOItem.MRP_CONTROLLER.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["MRP_CONTROLLER"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.MRP_CONTROLLER.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.MRP_CONTROLLER.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.STOCK.Trim().Equals(sapSOItem.STOCK.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["STOCK"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.STOCK.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.STOCK.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.ITEM_CUSTOM_1.Trim().Equals(sapSOItem.ITEM_CUSTOM_1.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["ITEM_CUSTOM_1"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.ITEM_CUSTOM_1.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.ITEM_CUSTOM_1.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.ITEM_CUSTOM_2.Trim().Equals(sapSOItem.ITEM_CUSTOM_2.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["ITEM_CUSTOM_2"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.ITEM_CUSTOM_2.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.ITEM_CUSTOM_2.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                if (!assignSOItem.ITEM_CUSTOM_3.Trim().Equals(sapSOItem.ITEM_CUSTOM_3.Trim()))
                                {
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["ITEM_CUSTOM_3"].ToString();
                                    soChange.OLD_VALUE = assignSOItem.ITEM_CUSTOM_3.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOItem.ITEM_CUSTOM_3.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }

                                #endregion

                                #region "Component"
                                if (assignSOItemBom != null && sapSOBom != null)
                                {
                                    if (assignSOItemBom.COMPONENT_ITEM != null && sapSOBom.COMPONENT_ITEM != null
                                        && !assignSOItemBom.COMPONENT_ITEM.Trim().Equals(sapSOBom.COMPONENT_ITEM.Trim()))
                                    {
                                        soChange = new SALES_ORDER_CHANGE();
                                        soChange.GROUPING = group;
                                        soChange.FIELDS_NAME = dicFieldsName["COMPONENT_ITEM"].ToString();
                                        soChange.OLD_VALUE = assignSOItemBom.COMPONENT_ITEM.ToString().Trim();
                                        soChange.NEW_VALUE = sapSOBom.COMPONENT_ITEM.ToString().Trim();
                                        soChangeList.Add(soChange);
                                    }

                                    if (assignSOItemBom.COMPONENT_MATERIAL != null && sapSOBom.COMPONENT_MATERIAL != null
                                       && !assignSOItemBom.COMPONENT_MATERIAL.Trim().Equals(sapSOBom.COMPONENT_MATERIAL.Trim()))
                                    {
                                        soChange = new SALES_ORDER_CHANGE();
                                        soChange.GROUPING = group;
                                        soChange.FIELDS_NAME = dicFieldsName["COMPONENT_MATERIAL"].ToString();
                                        soChange.OLD_VALUE = assignSOItemBom.COMPONENT_MATERIAL.ToString().Trim();
                                        soChange.NEW_VALUE = sapSOBom.COMPONENT_MATERIAL.ToString().Trim();
                                        soChangeList.Add(soChange);
                                    }

                                    if (assignSOItemBom.DECRIPTION != null && sapSOBom.DECRIPTION != null
                                        && !assignSOItemBom.DECRIPTION.Trim().Equals(sapSOBom.DECRIPTION.Trim()))
                                    {
                                        soChange = new SALES_ORDER_CHANGE();
                                        soChange.GROUPING = group;
                                        soChange.FIELDS_NAME = dicFieldsName["DECRIPTION"].ToString();
                                        soChange.OLD_VALUE = assignSOItemBom.DECRIPTION.ToString().Trim();
                                        soChange.NEW_VALUE = sapSOBom.DECRIPTION.ToString().Trim();
                                        soChangeList.Add(soChange);
                                    }

                                    if (assignSOItemBom.QUANTITY != null && sapSOBom.QUANTITY != null
                                        && !assignSOItemBom.QUANTITY.Equals(sapSOBom.QUANTITY))
                                    {
                                        soChange = new SALES_ORDER_CHANGE();
                                        soChange.GROUPING = group;
                                        soChange.FIELDS_NAME = dicFieldsName["QUANTITY"].ToString();
                                        soChange.OLD_VALUE = assignSOItemBom.QUANTITY.ToString().Trim();
                                        soChange.NEW_VALUE = sapSOBom.QUANTITY.ToString().Trim();
                                        soChangeList.Add(soChange);
                                    }

                                    if (assignSOItemBom.UNIT != null && sapSOBom.UNIT != null
                                       && !assignSOItemBom.UNIT.Trim().Equals(sapSOBom.UNIT.Trim()))
                                    {
                                        soChange = new SALES_ORDER_CHANGE();
                                        soChange.GROUPING = group;
                                        soChange.FIELDS_NAME = dicFieldsName["UNIT"].ToString();
                                        soChange.OLD_VALUE = assignSOItemBom.UNIT.ToString().Trim();
                                        soChange.NEW_VALUE = sapSOBom.UNIT.ToString().Trim();
                                        soChangeList.Add(soChange);
                                    }

                                    if (assignSOItemBom.STOCK != null && sapSOBom.STOCK != null
                                       && !assignSOItemBom.STOCK.Trim().Equals(sapSOBom.STOCK.Trim()))
                                    {
                                        soChange = new SALES_ORDER_CHANGE();
                                        soChange.GROUPING = group;
                                        soChange.FIELDS_NAME = dicFieldsName["COMPONENT_STOCK"].ToString();
                                        soChange.OLD_VALUE = assignSOItemBom.STOCK.ToString().Trim();
                                        soChange.NEW_VALUE = sapSOBom.STOCK.ToString().Trim();
                                        soChangeList.Add(soChange);
                                    }

                                    if (assignSOItemBom.BOM_ITEM_CUSTOM_1 != null && sapSOBom.BOM_ITEM_CUSTOM_1 != null
                                       && !assignSOItemBom.BOM_ITEM_CUSTOM_1.Equals(sapSOBom.BOM_ITEM_CUSTOM_1))
                                    {
                                        soChange = new SALES_ORDER_CHANGE();
                                        soChange.GROUPING = group;
                                        soChange.FIELDS_NAME = dicFieldsName["BOM_ITEM_CUSTOM_1"].ToString();
                                        soChange.OLD_VALUE = assignSOItemBom.BOM_ITEM_CUSTOM_1.ToString();
                                        soChange.NEW_VALUE = sapSOBom.BOM_ITEM_CUSTOM_1.ToString();
                                        soChangeList.Add(soChange);
                                    }

                                    if (assignSOItemBom.BOM_ITEM_CUSTOM_2 != null && sapSOBom.BOM_ITEM_CUSTOM_2 != null
                                       && !assignSOItemBom.BOM_ITEM_CUSTOM_2.Trim().Equals(sapSOBom.BOM_ITEM_CUSTOM_2.Trim()))
                                    {
                                        soChange = new SALES_ORDER_CHANGE();
                                        soChange.GROUPING = group;
                                        soChange.FIELDS_NAME = dicFieldsName["BOM_ITEM_CUSTOM_2"].ToString();
                                        soChange.OLD_VALUE = assignSOItemBom.BOM_ITEM_CUSTOM_2.ToString().Trim();
                                        soChange.NEW_VALUE = sapSOBom.BOM_ITEM_CUSTOM_2.ToString().Trim();
                                        soChangeList.Add(soChange);
                                    }

                                    if (assignSOItemBom.BOM_ITEM_CUSTOM_3 != null && sapSOBom.BOM_ITEM_CUSTOM_3 != null
                                       && !assignSOItemBom.BOM_ITEM_CUSTOM_3.Trim().Equals(sapSOBom.BOM_ITEM_CUSTOM_3.Trim()))
                                    {
                                        soChange = new SALES_ORDER_CHANGE();
                                        soChange.GROUPING = group;
                                        soChange.FIELDS_NAME = dicFieldsName["BOM_ITEM_CUSTOM_3"].ToString();
                                        soChange.OLD_VALUE = assignSOItemBom.BOM_ITEM_CUSTOM_3.ToString().Trim();
                                        soChange.NEW_VALUE = sapSOBom.BOM_ITEM_CUSTOM_3.ToString().Trim();
                                        soChangeList.Add(soChange);
                                    }
                                }
                                #endregion

                                #region "General Text"

                                string sapSOGeneralText = "";
                                string assignSOGeneralText = "";
                                string textName = "";

                                textName = sapSOHeader.SALES_ORDER_NO.ToString().PadLeft(10, '0');
                                var temp = listSAP_M_LONG_TEXT.Where(m => m.TEXT_NAME == textName && m.TEXT_ID == "Z001").OrderBy(m => m.LINE_ID).ToList();
                                sapSOGeneralText = GetLongText(temp);

                                textName = assignSOHeader.SALES_ORDER_NO.ToString().PadLeft(10, '0');
                                var temp2 = listSAP_M_LONG_TEXT_ASSIGN.Where(m => m.TEXT_NAME == textName && m.TEXT_ID == "Z001" && m.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).OrderBy(m => m.LINE_ID).ToList();
                                assignSOGeneralText = GetLongText(temp2);

                                if (!assignSOGeneralText.Trim().Equals(sapSOGeneralText.Trim()))
                                {
                                    //GENERAL_TEXT
                                    soChange = new SALES_ORDER_CHANGE();
                                    soChange.GROUPING = group;
                                    soChange.FIELDS_NAME = dicFieldsName["GENERAL_TEXT"].ToString();
                                    soChange.OLD_VALUE = assignSOGeneralText.ToString().Trim();
                                    soChange.NEW_VALUE = sapSOGeneralText.ToString().Trim();
                                    soChangeList.Add(soChange);
                                }
                                #endregion

                                #region "Warehouse Text"
                                string _itemNOTmp = "";
                                string _orderNOTmp = "";
                                string _textName = "";

                                string sapSOWarehouseText = "";
                                string assignSOWarehouseText = "";

                                if (sapSOHeader != null && assignSOHeader != null)
                                {
                                    _orderNOTmp = sapSOHeader.SALES_ORDER_NO.ToString().PadLeft(10, '0');
                                    _itemNOTmp = sapSOItem.ITEM.ToString().PadLeft(6, '0');
                                    _textName = _orderNOTmp + _itemNOTmp;

                                    var temp3 = listSAP_M_LONG_TEXT.Where(m => m.TEXT_NAME == _textName && m.TEXT_ID == "Z105").OrderBy(m => m.LINE_ID).ToList();
                                    sapSOWarehouseText = GetLongText(temp3);

                                    var temp4 = listSAP_M_LONG_TEXT_ASSIGN.Where(m => m.TEXT_NAME == _textName && m.TEXT_ID == "Z105" && m.ARTWORK_SUB_ID == param.data.ARTWORK_SUB_ID).OrderBy(m => m.LINE_ID).ToList();
                                    assignSOWarehouseText = GetLongText(temp4);

                                    if (!assignSOWarehouseText.Trim().Equals(sapSOWarehouseText.Trim()))
                                    {
                                        soChange = new SALES_ORDER_CHANGE();
                                        soChange.GROUPING = group;
                                        soChange.FIELDS_NAME = dicFieldsName["WAREHOUSE_TEXT"].ToString();
                                        soChange.OLD_VALUE = assignSOWarehouseText.ToString().Trim();
                                        soChange.NEW_VALUE = sapSOWarehouseText.ToString().Trim();
                                        soChangeList.Add(soChange);
                                    }
                                }
                                #endregion
                            }
                        }
                    }

                    Results.status = "S";
                    Results.data = soChangeList;
                }
                catch (Exception ex)
                {
                    Results.status = "E";
                    Results.msg = CNService.GetErrorMessage(ex);
                }
            }

            return Results;
        }

        //public static string CheckIsSalesOrderChange(int artworkSubID)
        //{
        //    ART_WF_ARTWORK_PROCESS_SO_DETAIL soDetail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
        //    SALES_ORDER_CHANGE soChange = new SALES_ORDER_CHANGE();
        //    List<SALES_ORDER_CHANGE> soChangeList = new List<SALES_ORDER_CHANGE>();

        //    try
        //    {
        //        using (var context = new ARTWORKEntities())
        //        {
        //            using (CNService.IsolationLevel(context))
        //            {
        //                return CheckIsSalesOrderChange(artworkSubID, context);
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}

        public static string CheckIsSalesOrderChange(int artworkSubID, ARTWORKEntities context)
        {
            string Results = "X";
            ART_WF_ARTWORK_PROCESS_SO_DETAIL soDetail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
            SALES_ORDER_CHANGE soChange = new SALES_ORDER_CHANGE();
            List<SALES_ORDER_CHANGE> soChangeList = new List<SALES_ORDER_CHANGE>();

            try
            {
                ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT assignSOItemBom = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT();
                SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT sapSOBom = new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT();

                soDetail.ARTWORK_SUB_ID = artworkSubID;
                var soDetailList = ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(soDetail, context);

                //for performance 
                var listSALES_ORDER_NO = soDetailList.Select(m => m.SALES_ORDER_NO).ToList();
                var listSapSOHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                       where listSALES_ORDER_NO.Contains(h.SALES_ORDER_NO)
                                       select h).ToList();

                var listPO_COMPLETE_SO_HEADER_ID = listSapSOHeader.Select(m => m.PO_COMPLETE_SO_HEADER_ID).ToList();
                var listsapSOItem = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
                                     where listPO_COMPLETE_SO_HEADER_ID.Contains(i.PO_COMPLETE_SO_HEADER_ID)
                                     select i).ToList();

                var listPO_COMPLETE_SO_ITEM_ID = listsapSOItem.Select(m => m.PO_COMPLETE_SO_ITEM_ID).ToList();
                var listSapSOBom = (from h in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                                    where listPO_COMPLETE_SO_ITEM_ID.Contains(h.PO_COMPLETE_SO_ITEM_ID)
                                    select h).ToList();


                var listassignSOHeader = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
                                          where h.ARTWORK_SUB_ID == artworkSubID
                                          select h).ToList();

                var listassignSOItem = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM
                                        where h.ARTWORK_SUB_ID == artworkSubID
                                        select h).ToList();

                var listassignSOItemBom = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT
                                           where h.ARTWORK_SUB_ID == artworkSubID
                                           select h).ToList();


                var listSALES_ORDER_NO2 = soDetailList.Select(m => m.SALES_ORDER_NO.PadLeft(10, '0')).ToList();
                var listSAP_M_LONG_TEXT = (from h in context.SAP_M_LONG_TEXT
                                           where listSALES_ORDER_NO2.Contains(h.TEXT_NAME.Substring(0, 10))
                                           select h).ToList();

                var listSAP_M_LONG_TEXT_ASSIGN = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT
                                                  where h.ARTWORK_SUB_ID == artworkSubID
                                                  select h).ToList();

                if (soDetailList != null && soDetailList.Count > 0)
                {
                    foreach (ART_WF_ARTWORK_PROCESS_SO_DETAIL iSO in soDetailList)
                    {
                        string group = "";

                        group = iSO.SALES_ORDER_NO + "(" + iSO.SALES_ORDER_ITEM.ToString() + ")";

                        var sapSOHeader = listSapSOHeader.Where(m => m.SALES_ORDER_NO == iSO.SALES_ORDER_NO).FirstOrDefault();
                        //var sapSOHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                        //                   where h.SALES_ORDER_NO == iSO.SALES_ORDER_NO
                        //                   select h).FirstOrDefault();

                        decimal itemNO = 0;
                        itemNO = Convert.ToDecimal(iSO.SALES_ORDER_ITEM);

                        //var sapSOItem = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
                        //                 where i.PO_COMPLETE_SO_HEADER_ID == sapSOHeader.PO_COMPLETE_SO_HEADER_ID
                        //                    && i.ITEM == itemNO
                        //                 select i).FirstOrDefault();
                        var sapSOItem = listsapSOItem.Where(m => m.PO_COMPLETE_SO_HEADER_ID == sapSOHeader.PO_COMPLETE_SO_HEADER_ID && m.ITEM == itemNO).FirstOrDefault();

                        if (iSO.BOM_ID > 0)
                        {
                            sapSOBom = listSapSOBom.Where(m => m.PO_COMPLETE_SO_ITEM_ID == sapSOItem.PO_COMPLETE_SO_ITEM_ID && m.PO_COMPLETE_SO_ITEM_COMPONENT_ID == iSO.BOM_ID).FirstOrDefault();
                            //sapSOBom = (from b in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                            //            where b.PO_COMPLETE_SO_ITEM_ID == sapSOItem.PO_COMPLETE_SO_ITEM_ID
                            //                && b.PO_COMPLETE_SO_ITEM_COMPONENT_ID == iSO.BOM_ID
                            //            select b).FirstOrDefault();
                        }

                        var assignSOHeader = listassignSOHeader.Where(m => m.ARTWORK_PROCESS_SO_ID == iSO.ARTWORK_PROCESS_SO_ID
                                                                          && m.SALES_ORDER_NO == iSO.SALES_ORDER_NO
                                                                          && m.ARTWORK_SUB_ID == iSO.ARTWORK_SUB_ID).FirstOrDefault();

                        //var assignSOHeader = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER
                        //                      where h.ARTWORK_PROCESS_SO_ID == iSO.ARTWORK_PROCESS_SO_ID
                        //                       && h.SALES_ORDER_NO == iSO.SALES_ORDER_NO
                        //                       && h.ARTWORK_SUB_ID == iSO.ARTWORK_SUB_ID
                        //                      select h).FirstOrDefault();

                        itemNO = Convert.ToDecimal(iSO.SALES_ORDER_ITEM);
                        //var assignSOItem = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM
                        //                    where h.ARTWORK_PROCESS_SO_ID == iSO.ARTWORK_PROCESS_SO_ID
                        //                     && h.ITEM == itemNO
                        //                     && h.ARTWORK_SUB_ID == iSO.ARTWORK_SUB_ID
                        //                    select h).FirstOrDefault();

                        var assignSOItem = listassignSOItem.Where(m => m.ARTWORK_PROCESS_SO_ID == iSO.ARTWORK_PROCESS_SO_ID
                                                  && m.ITEM == itemNO
                                                  && m.ARTWORK_SUB_ID == iSO.ARTWORK_SUB_ID).FirstOrDefault();

                        if (iSO.BOM_ID > 0)
                        {
                            //assignSOItemBom = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT
                            //                   where h.ARTWORK_PROCESS_SO_ID == iSO.ARTWORK_PROCESS_SO_ID
                            //                    && h.ARTWORK_SUB_ID == iSO.ARTWORK_SUB_ID
                            //                   select h).FirstOrDefault();

                            assignSOItemBom = listassignSOItemBom.Where(m => m.ARTWORK_PROCESS_SO_ID == iSO.ARTWORK_PROCESS_SO_ID
                                                  && m.ARTWORK_SUB_ID == iSO.ARTWORK_SUB_ID).FirstOrDefault();
                        }

                        #region "Header"
                        if (!assignSOHeader.SALES_ORDER_NO.Trim().Equals(sapSOHeader.SALES_ORDER_NO.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.SOLD_TO.Trim().Equals(sapSOHeader.SOLD_TO.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.SOLD_TO_NAME.Trim().Equals(sapSOHeader.SOLD_TO_NAME.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.LAST_SHIPMENT_DATE.Equals(sapSOHeader.LAST_SHIPMENT_DATE))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.DATE_1_2.Equals(sapSOHeader.DATE_1_2))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.CREATE_ON.Equals(sapSOHeader.CREATE_ON))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.RDD.Equals(sapSOHeader.RDD))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.PAYMENT_TERM.Trim().Equals(sapSOHeader.PAYMENT_TERM.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.LC_NO.Trim().Equals(sapSOHeader.LC_NO.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.EXPIRED_DATE.Equals(sapSOHeader.EXPIRED_DATE))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.SHIP_TO.Trim().Equals(sapSOHeader.SHIP_TO.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.SHIP_TO_NAME.Trim().Equals(sapSOHeader.SHIP_TO_NAME.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.SOLD_TO_PO.Trim().Equals(sapSOHeader.SOLD_TO_PO.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.SHIP_TO_PO.Trim().Equals(sapSOHeader.SHIP_TO_PO.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.SALES_GROUP.Trim().Equals(sapSOHeader.SALES_GROUP.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.MARKETING_CO.Trim().Equals(sapSOHeader.MARKETING_CO.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.MARKETING_CO_NAME.Trim().Equals(sapSOHeader.MARKETING_CO_NAME.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.MARKETING.Trim().Equals(sapSOHeader.MARKETING.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.MARKETING_NAME.Trim().Equals(sapSOHeader.MARKETING_NAME.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.MARKETING_ORDER_SAP.Trim().Equals(sapSOHeader.MARKETING_ORDER_SAP.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.MARKETING_ORDER_SAP_NAME.Trim().Equals(sapSOHeader.MARKETING_ORDER_SAP_NAME.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.DISTRIBUTION_CHANNEL.Trim().Equals(sapSOHeader.DISTRIBUTION_CHANNEL.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.DIVITION.Trim().Equals(sapSOHeader.DIVITION.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.SALES_ORDER_TYPE.Trim().Equals(sapSOHeader.SALES_ORDER_TYPE.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.HEADER_CUSTOM_1.Trim().Equals(sapSOHeader.HEADER_CUSTOM_1.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.HEADER_CUSTOM_2.Trim().Equals(sapSOHeader.HEADER_CUSTOM_2.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOHeader.HEADER_CUSTOM_3.Trim().Equals(sapSOHeader.HEADER_CUSTOM_3.Trim()))
                        {
                            return Results;
                        }
                        #endregion

                        #region "Item"
                        if (!assignSOItem.ITEM.Equals(sapSOItem.ITEM))
                        {
                            return Results;
                        }

                        if (!assignSOItem.PRODUCT_CODE.Trim().Equals(sapSOItem.PRODUCT_CODE.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.MATERIAL_DESCRIPTION.Trim().Equals(sapSOItem.MATERIAL_DESCRIPTION.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.NET_WEIGHT.Trim().Equals(sapSOItem.NET_WEIGHT.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.ORDER_QTY.Equals(sapSOItem.ORDER_QTY))
                        {
                            return Results;
                        }

                        if (!assignSOItem.ORDER_UNIT.Trim().Equals(sapSOItem.ORDER_UNIT.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.ETD_DATE_FROM.Equals(sapSOItem.ETD_DATE_FROM))
                        {
                            return Results;
                        }

                        if (!assignSOItem.ETD_DATE_TO.Equals(sapSOItem.ETD_DATE_TO))
                        {
                            return Results;
                        }

                        if (!assignSOItem.PLANT.Trim().Equals(sapSOItem.PLANT.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.OLD_MATERIAL_CODE.Trim().Equals(sapSOItem.OLD_MATERIAL_CODE.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.PACK_SIZE.Trim().Equals(sapSOItem.PACK_SIZE.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.VALUME_PER_UNIT.Trim().Equals(sapSOItem.VALUME_PER_UNIT.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.VALUME_UNIT.Trim().Equals(sapSOItem.VALUME_UNIT.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.SIZE_DRAIN_WT.Trim().Equals(sapSOItem.SIZE_DRAIN_WT.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.PROD_INSP_MEMO.Trim().Equals(sapSOItem.PROD_INSP_MEMO.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.REJECTION_CODE.Trim().Equals(sapSOItem.REJECTION_CODE.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.REJECTION_DESCRIPTION.Trim().Equals(sapSOItem.REJECTION_DESCRIPTION.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.PORT.Trim().Equals(sapSOItem.PORT.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.VIA.Trim().Equals(sapSOItem.VIA.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.IN_TRANSIT_TO.Equals(sapSOItem.IN_TRANSIT_TO))
                        {
                            return Results;
                        }

                        if (!assignSOItem.BRAND_ID.Trim().Equals(sapSOItem.BRAND_ID.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.BRAND_DESCRIPTION.Trim().Equals(sapSOItem.BRAND_DESCRIPTION.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.ADDITIONAL_BRAND_ID.Trim().Equals(sapSOItem.ADDITIONAL_BRAND_ID.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.ADDITIONAL_BRAND_DESCRIPTION.Trim().Equals(sapSOItem.ADDITIONAL_BRAND_DESCRIPTION.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.PRODUCTION_PLANT.Trim().Equals(sapSOItem.PRODUCTION_PLANT.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.ZONE.Trim().Equals(sapSOItem.ZONE.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.COUNTRY.Trim().Equals(sapSOItem.COUNTRY.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.PRODUCTION_HIERARCHY.Trim().Equals(sapSOItem.PRODUCTION_HIERARCHY.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.MRP_CONTROLLER.Trim().Equals(sapSOItem.MRP_CONTROLLER.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.STOCK.Trim().Equals(sapSOItem.STOCK.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.ITEM_CUSTOM_1.Trim().Equals(sapSOItem.ITEM_CUSTOM_1.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.ITEM_CUSTOM_2.Trim().Equals(sapSOItem.ITEM_CUSTOM_2.Trim()))
                        {
                            return Results;
                        }

                        if (!assignSOItem.ITEM_CUSTOM_3.Trim().Equals(sapSOItem.ITEM_CUSTOM_3.Trim()))
                        {
                            return Results;
                        }

                        #endregion

                        #region "Component"
                        if (iSO.BOM_ID > 0)
                        {
                            if (!assignSOItemBom.COMPONENT_ITEM.Trim().Equals(sapSOBom.COMPONENT_ITEM.Trim()))
                            {
                                return Results;
                            }

                            if (!assignSOItemBom.COMPONENT_MATERIAL.Trim().Equals(sapSOBom.COMPONENT_MATERIAL.Trim()))
                            {
                                return Results;
                            }

                            if (!assignSOItemBom.DECRIPTION.Trim().Equals(sapSOBom.DECRIPTION.Trim()))
                            {
                                return Results;
                            }

                            if (!assignSOItemBom.QUANTITY.Equals(sapSOBom.QUANTITY))
                            {
                                return Results;
                            }

                            if (!assignSOItemBom.UNIT.Trim().Equals(sapSOBom.UNIT.Trim()))
                            {
                                return Results;
                            }

                            if (!assignSOItemBom.STOCK.Trim().Equals(sapSOBom.STOCK.Trim()))
                            {
                                return Results;
                            }

                            if (!assignSOItemBom.BOM_ITEM_CUSTOM_1.Trim().Equals(
                                string.Format("{0}", sapSOBom.BOM_ITEM_CUSTOM_1).Trim()))
                            {
                                return Results;
                            }

                            if (!assignSOItemBom.BOM_ITEM_CUSTOM_2.Trim().Equals(sapSOBom.BOM_ITEM_CUSTOM_2.Trim()))
                            {
                                return Results;
                            }

                            if (!assignSOItemBom.BOM_ITEM_CUSTOM_3.Trim().Equals(sapSOBom.BOM_ITEM_CUSTOM_3.Trim()))
                            {
                                return Results;
                            }
                        }
                        #endregion

                        #region "General Text"

                        //string sapSOGeneralText = "";
                        //string assignSOGeneralText = "";
                        //string textName = "";
                        string textName = sapSOHeader.SALES_ORDER_NO.ToString().PadLeft(10, '0');
                        //sapSOGeneralText = GetLongText_Master(textName, "Z001", context);

                        var temp = listSAP_M_LONG_TEXT.Where(m => m.TEXT_NAME == textName && m.TEXT_ID == "Z001").OrderBy(m => m.LINE_ID).ToList();
                        var sapSOGeneralText = GetLongText(temp);
                        //string sapSOGeneralText = GetLongText(listSAP_M_LONG_TEXT.Where(m => m.TEXT_NAME == textName && m.TEXT_ID == "Z001").ToList());

                        //textName = assignSOHeader.SALES_ORDER_NO.ToString().PadLeft(10, '0');
                        //assignSOGeneralText = GetLongTextText_Assign(textName, "Z001", artworkSubID, context);
                        //string assignSOGeneralText = GetLongText(listSAP_M_LONG_TEXT_ASSIGN.Where(m => m.TEXT_NAME == textName && m.TEXT_ID == "Z001" && m.ARTWORK_SUB_ID == artworkSubID).ToList());
                        var temp2 = listSAP_M_LONG_TEXT_ASSIGN.Where(m => m.TEXT_NAME == textName && m.TEXT_ID == "Z001" && m.ARTWORK_SUB_ID == artworkSubID).OrderBy(m => m.LINE_ID).ToList();
                        var assignSOGeneralText = GetLongText(temp2);

                        if (!assignSOGeneralText.Trim().Equals(sapSOGeneralText.Trim()))
                        //if (!assignSOGeneralText.Equals(sapSOGeneralText))
                        {
                            return Results;
                        }
                        #endregion

                        #region "Warehouse Text"
                        //string _itemNOTmp = "";
                        //string _orderNOTmp = "";
                        //string _textName = "";

                        //string sapSOWarehouseText = "";
                        //string assignSOWarehouseText = "";

                        if (sapSOHeader != null && assignSOHeader != null)
                        {
                            string _orderNOTmp = sapSOHeader.SALES_ORDER_NO.ToString().PadLeft(10, '0');
                            string _itemNOTmp = sapSOItem.ITEM.ToString().PadLeft(6, '0');
                            string _textName = _orderNOTmp + _itemNOTmp;

                            //sapSOWarehouseText = GetLongText_Master(_textName, "Z105", context);
                            //string sapSOWarehouseText = GetLongText(listSAP_M_LONG_TEXT.Where(m => m.TEXT_NAME == _textName && m.TEXT_ID == "Z105").ToList());
                            //_orderNOTmp = sapSOHeader.SALES_ORDER_NO.ToString().PadLeft(10, '0');
                            //_itemNOTmp = sapSOItem.ITEM.ToString().PadLeft(6, '0');
                            //_textName = _orderNOTmp + _itemNOTmp;
                            //string assignSOWarehouseText = GetLongText(listSAP_M_LONG_TEXT_ASSIGN.Where(m => m.TEXT_NAME == _textName && m.TEXT_ID == "Z105" && m.ARTWORK_SUB_ID == artworkSubID).ToList());

                            var temp3 = listSAP_M_LONG_TEXT.Where(m => m.TEXT_NAME == _textName && m.TEXT_ID == "Z105").OrderBy(m => m.LINE_ID).ToList();
                            var temp4 = listSAP_M_LONG_TEXT_ASSIGN.Where(m => m.TEXT_NAME == _textName && m.TEXT_ID == "Z105" && m.ARTWORK_SUB_ID == artworkSubID).OrderBy(m => m.LINE_ID).ToList();
                            var sapSOWarehouseText = GetLongText(temp3);
                            var assignSOWarehouseText = GetLongText(temp4);

                            if (!assignSOWarehouseText.Trim().Equals(sapSOWarehouseText.Trim()))
                            //if (!assignSOWarehouseText.Equals(sapSOWarehouseText))
                            {
                                return Results;
                            }
                        }
                        #endregion
                    }

                    return "";
                }

                return "";

            }
            catch
            {
                return "";
            }
        }

        private static void UpdateArtworkValue(ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 param, ARTWORKEntities context)
        {
            int artwork_sub_id = 0;
            if (param != null)
            {
                artwork_sub_id = param.ARTWORK_SUB_ID;
                ART_WF_ARTWORK_PROCESS_PA processPA = new ART_WF_ARTWORK_PROCESS_PA();
                processPA.ARTWORK_SUB_ID = artwork_sub_id;
                processPA = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(processPA, context).FirstOrDefault();

                if (processPA != null)
                {
                    var boms = (from b in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                where b.ARTWORK_SUB_ID == artwork_sub_id
                                select b.BOM_ID).ToList();

                    var so = (from b in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                              where b.ARTWORK_SUB_ID == artwork_sub_id
                              select b).ToList();

                    if (so != null && so.Count > 0)
                    {
                        string productCode = "";
                        productCode = so[0].MATERIAL_NO;

                        if (!String.IsNullOrEmpty(productCode))
                        {
                            var xProduct = context.XECM_M_PRODUCT.Where(p => p.PRODUCT_CODE == productCode).FirstOrDefault();

                            if (xProduct != null)
                            {
                                processPA.RD_REFERENCE_NO_ID = null;
                                processPA.PRODUCT_CODE_ID = xProduct.XECM_PRODUCT_ID;
                                processPA.IS_LOCK_PRODUCT_CODE = "X";
                                ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);
                            }
                        }
                    }
                    else
                    {
                        // processPA.PRODUCT_CODE_ID = null;
                        processPA.IS_LOCK_PRODUCT_CODE = null;
                        ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);
                    }

                    //if (boms != null && boms.Count > 0)
                    //{
                    //    var bomItem = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                    //                   where boms.Contains(i.PO_COMPLETE_SO_ITEM_COMPONENT_ID)
                    //                   select i.BOM_ITEM_CUSTOM_1).FirstOrDefault();

                    //    if (!String.IsNullOrEmpty(bomItem))
                    //    {
                    //        string bomNew = GetBomCustom1Value(bomItem);
                    //        int id = GetMaterialGroupID(bomNew, context);

                    //        if (id > 0)
                    //        {
                    //            processPA.MATERIAL_GROUP_ID = id;
                    //            ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        processPA.MATERIAL_GROUP_ID = null;
                    //        ART_WF_ARTWORK_PROCESS_PA_SERVICE.SaveOrUpdate(processPA, context);
                    //    }
                    //}
                }
            }
        }

        public static void CopyAssignSalesOrder(ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 param, ARTWORKEntities context)
        {
            if (param != null)
            {
                ART_WF_ARTWORK_PROCESS_SO_DETAIL sodata = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
                sodata.ARTWORK_SUB_ID = param.ARTWORK_SUB_ID;

                List<ART_WF_ARTWORK_PROCESS_SO_DETAIL> soDataList = ART_WF_ARTWORK_PROCESS_SO_DETAIL_SERVICE.GetByItem(sodata, context);

                if (soDataList.Count > 0)
                {
                    var listso = soDataList.Select(m => m.SALES_ORDER_NO).ToList();
                    var listsoHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                        where listso.Contains(h.SALES_ORDER_NO)
                                        select h).ToList();

                    var PO_COMPLETE_SO_HEADER_ID = listsoHeader.Select(m => m.PO_COMPLETE_SO_HEADER_ID).ToList();
                    var listsoItem = (from h in context.SAP_M_PO_COMPLETE_SO_ITEM
                                      where PO_COMPLETE_SO_HEADER_ID.Contains(h.PO_COMPLETE_SO_HEADER_ID)
                                      select h).ToList();

                    var PO_COMPLETE_SO_ITEM_ID = listsoItem.Select(m => m.PO_COMPLETE_SO_ITEM_ID).ToList();
                    var listsoItemBom = (from h in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                                         where PO_COMPLETE_SO_ITEM_ID.Contains(h.PO_COMPLETE_SO_ITEM_ID)
                                         select h).ToList();

                    var listSALES_ORDER_NO2 = soDataList.Select(m => m.SALES_ORDER_NO.PadLeft(10, '0')).ToList();
                    var listSAP_M_LONG_TEXT = (from h in context.SAP_M_LONG_TEXT
                                               where listSALES_ORDER_NO2.Contains(h.TEXT_NAME.Substring(0, 10))
                                               select h).ToList();

                    var listSAP_M_LONG_TEXT_ASSIGN = (from h in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT
                                                      where h.ARTWORK_SUB_ID == param.ARTWORK_SUB_ID
                                                      select h).ToList();

                    foreach (ART_WF_ARTWORK_PROCESS_SO_DETAIL so in soDataList)
                    {
                        if (!String.IsNullOrEmpty(so.SALES_ORDER_NO))
                        {
                            var soHeader = listsoHeader.Where(m => m.SALES_ORDER_NO == so.SALES_ORDER_NO).FirstOrDefault();

                            if (soHeader != null)
                            {
                                ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER assignHeader = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER();

                                assignHeader.ARTWORK_PROCESS_SO_ID = so.ARTWORK_PROCESS_SO_ID;
                                assignHeader.ARTWORK_REQUEST_ID = param.ARTWORK_REQUEST_ID;
                                assignHeader.ARTWORK_SUB_ID = param.ARTWORK_SUB_ID;
                                assignHeader.SALES_ORDER_NO = soHeader.SALES_ORDER_NO.Trim();
                                assignHeader.SOLD_TO = soHeader.SOLD_TO.Trim();
                                assignHeader.SOLD_TO_NAME = soHeader.SOLD_TO_NAME.Trim();
                                assignHeader.LAST_SHIPMENT_DATE = soHeader.LAST_SHIPMENT_DATE;
                                assignHeader.DATE_1_2 = soHeader.DATE_1_2;
                                assignHeader.CREATE_ON = soHeader.CREATE_ON;
                                assignHeader.RDD = soHeader.RDD;
                                assignHeader.PAYMENT_TERM = soHeader.PAYMENT_TERM.Trim();
                                assignHeader.LC_NO = soHeader.LC_NO.Trim();
                                assignHeader.EXPIRED_DATE = soHeader.EXPIRED_DATE;
                                assignHeader.SHIP_TO = soHeader.SHIP_TO.Trim();
                                assignHeader.SHIP_TO_NAME = soHeader.SHIP_TO_NAME.Trim();
                                assignHeader.SOLD_TO_PO = soHeader.SOLD_TO_PO.Trim();
                                assignHeader.SHIP_TO_PO = soHeader.SHIP_TO_PO.Trim();
                                assignHeader.SALES_GROUP = soHeader.SALES_GROUP.Trim();
                                assignHeader.MARKETING_CO = soHeader.MARKETING_CO.Trim();
                                assignHeader.MARKETING_CO_NAME = soHeader.MARKETING_CO_NAME.Trim();
                                assignHeader.MARKETING = soHeader.MARKETING.Trim();
                                assignHeader.MARKETING_NAME = soHeader.MARKETING_NAME.Trim();
                                assignHeader.MARKETING_ORDER_SAP = soHeader.MARKETING_ORDER_SAP.Trim();
                                assignHeader.MARKETING_ORDER_SAP_NAME = soHeader.MARKETING_ORDER_SAP_NAME.Trim();
                                assignHeader.SALES_ORG = soHeader.SALES_ORG.Trim();
                                assignHeader.DISTRIBUTION_CHANNEL = soHeader.DISTRIBUTION_CHANNEL.Trim();
                                assignHeader.DIVITION = soHeader.DIVITION.Trim();
                                assignHeader.SALES_ORDER_TYPE = soHeader.SALES_ORDER_TYPE.Trim();
                                assignHeader.HEADER_CUSTOM_1 = soHeader.HEADER_CUSTOM_1.Trim();
                                assignHeader.HEADER_CUSTOM_2 = soHeader.HEADER_CUSTOM_2.Trim();
                                assignHeader.HEADER_CUSTOM_3 = soHeader.HEADER_CUSTOM_3.Trim();
                                assignHeader.CREATE_BY = param.UPDATE_BY;
                                assignHeader.UPDATE_BY = param.UPDATE_BY;

                                ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER_SERVICE.SaveNoLog(assignHeader, context);

                                #region "Save Long Text - General Text"

                                var tempSO = assignHeader.SALES_ORDER_NO.ToString().PadLeft(10, '0');
                                var temp = (from m in context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT
                                            where m.ARTWORK_SUB_ID == param.ARTWORK_SUB_ID
                                               && m.TEXT_NAME == tempSO
                                               && m.TEXT_ID == "Z001"
                                            select m.ASSIGN_SO_LONG_TEXT_ID).ToList();

                                if (temp.Count == 0)
                                {
                                    var doWork = false;
                                    var listLongText_General = listSAP_M_LONG_TEXT.Where(m => m.TEXT_NAME == assignHeader.SALES_ORDER_NO.ToString().PadLeft(10, '0') && m.TEXT_ID == "Z001").ToList();
                                    foreach (var item in listLongText_General)
                                    {
                                        ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT assignLongText = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT();
                                        assignLongText.ARTWORK_PROCESS_SO_ID = so.ARTWORK_PROCESS_SO_ID;
                                        assignLongText.ARTWORK_REQUEST_ID = param.ARTWORK_REQUEST_ID;
                                        assignLongText.ARTWORK_SUB_ID = param.ARTWORK_SUB_ID;
                                        assignLongText.TEXT_NAME = item.TEXT_NAME.Trim();
                                        assignLongText.TEXT_LANGUAGE = item.TEXT_LANGUAGE.Trim();
                                        assignLongText.TEXT_ID = item.TEXT_ID.Trim();
                                        assignLongText.LINE_ID = item.LINE_ID;
                                        assignLongText.LINE_TEXT = item.LINE_TEXT.Trim();
                                        assignLongText.CREATE_BY = param.UPDATE_BY;
                                        assignLongText.UPDATE_BY = param.UPDATE_BY;
                                        assignLongText.CREATE_DATE = DateTime.Now;
                                        assignLongText.UPDATE_DATE = DateTime.Now;

                                        doWork = true;
                                        context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT.Add(assignLongText);
                                        //ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_SERVICE.SaveNoLog(assignLongText, context);
                                    }
                                    if (doWork) context.SaveChanges();
                                }
                                #endregion

                                decimal itemNo = 0;
                                itemNo = Convert.ToDecimal(so.SALES_ORDER_ITEM);
                                var soItem = listsoItem.Where(m => m.PO_COMPLETE_SO_HEADER_ID == soHeader.PO_COMPLETE_SO_HEADER_ID && m.ITEM == itemNo).FirstOrDefault();

                                if (soItem != null)
                                {
                                    ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM assignItem = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM();
                                    assignItem.ASSIGN_SO_HEADER_ID = assignHeader.ASSIGN_SO_HEADER_ID;
                                    assignItem.ARTWORK_PROCESS_SO_ID = so.ARTWORK_PROCESS_SO_ID;
                                    assignItem.ARTWORK_REQUEST_ID = param.ARTWORK_REQUEST_ID;
                                    assignItem.ARTWORK_SUB_ID = param.ARTWORK_SUB_ID;
                                    assignItem.ITEM = soItem.ITEM;
                                    assignItem.PRODUCT_CODE = soItem.PRODUCT_CODE.Trim();
                                    assignItem.MATERIAL_DESCRIPTION = soItem.MATERIAL_DESCRIPTION.Trim();
                                    assignItem.NET_WEIGHT = soItem.NET_WEIGHT.Trim();
                                    assignItem.ORDER_QTY = soItem.ORDER_QTY;
                                    assignItem.ORDER_UNIT = soItem.ORDER_UNIT.Trim();
                                    assignItem.ETD_DATE_FROM = soItem.ETD_DATE_FROM;
                                    assignItem.ETD_DATE_TO = soItem.ETD_DATE_TO;
                                    assignItem.PLANT = soItem.PLANT.Trim();
                                    assignItem.OLD_MATERIAL_CODE = soItem.OLD_MATERIAL_CODE.Trim();
                                    assignItem.PACK_SIZE = soItem.PACK_SIZE.Trim();
                                    assignItem.VALUME_PER_UNIT = soItem.VALUME_PER_UNIT.Trim();
                                    assignItem.VALUME_UNIT = soItem.VALUME_UNIT.Trim();
                                    assignItem.SIZE_DRAIN_WT = soItem.SIZE_DRAIN_WT.Trim();
                                    assignItem.PROD_INSP_MEMO = soItem.PROD_INSP_MEMO.Trim();
                                    assignItem.REJECTION_CODE = soItem.REJECTION_CODE.Trim();
                                    assignItem.REJECTION_DESCRIPTION = soItem.REJECTION_DESCRIPTION.Trim();
                                    assignItem.PORT = soItem.PORT.Trim();
                                    assignItem.VIA = soItem.VIA.Trim();
                                    assignItem.IN_TRANSIT_TO = soItem.IN_TRANSIT_TO.Trim();
                                    assignItem.BRAND_ID = soItem.BRAND_ID.Trim();
                                    assignItem.BRAND_DESCRIPTION = soItem.BRAND_DESCRIPTION.Trim();
                                    assignItem.ADDITIONAL_BRAND_ID = soItem.ADDITIONAL_BRAND_ID.Trim();
                                    assignItem.ADDITIONAL_BRAND_DESCRIPTION = soItem.ADDITIONAL_BRAND_DESCRIPTION.Trim();
                                    assignItem.PRODUCTION_PLANT = soItem.PRODUCTION_PLANT.Trim();
                                    assignItem.ZONE = soItem.ZONE.Trim();
                                    assignItem.COUNTRY = soItem.COUNTRY.Trim();
                                    assignItem.PRODUCTION_HIERARCHY = soItem.PRODUCTION_HIERARCHY.Trim();
                                    assignItem.MRP_CONTROLLER = soItem.MRP_CONTROLLER.Trim();
                                    assignItem.STOCK = soItem.STOCK.Trim();
                                    assignItem.ITEM_CUSTOM_1 = soItem.ITEM_CUSTOM_1.Trim();
                                    assignItem.ITEM_CUSTOM_2 = soItem.ITEM_CUSTOM_2.Trim();
                                    assignItem.ITEM_CUSTOM_3 = soItem.ITEM_CUSTOM_3.Trim();
                                    assignItem.CREATE_BY = param.UPDATE_BY;
                                    assignItem.UPDATE_BY = param.UPDATE_BY;

                                    ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_SERVICE.SaveNoLog(assignItem, context);
                                    soItem.IS_ASSIGN = "X";
                                    SAP_M_PO_COMPLETE_SO_ITEM_SERVICE.UpdateNoLog(soItem, context);

                                    #region "Save Long Text - Warehouse Text"

                                    string orderNOTmp = assignHeader.SALES_ORDER_NO.ToString().PadLeft(10, '0');
                                    string itemNOTmp = soItem.ITEM.ToString().PadLeft(6, '0');
                                    string textName = orderNOTmp + itemNOTmp;

                                    var doWork = false;
                                    var listLongText_Warehouse = listSAP_M_LONG_TEXT.Where(m => m.TEXT_NAME == textName && m.TEXT_ID == "Z105").ToList();
                                    foreach (SAP_M_LONG_TEXT txt in listLongText_Warehouse)
                                    {
                                        ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT assignLongText = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT();
                                        assignLongText.ARTWORK_PROCESS_SO_ID = so.ARTWORK_PROCESS_SO_ID;
                                        assignLongText.ARTWORK_REQUEST_ID = param.ARTWORK_REQUEST_ID;
                                        assignLongText.ARTWORK_SUB_ID = param.ARTWORK_SUB_ID;
                                        assignLongText.TEXT_NAME = txt.TEXT_NAME.Trim();
                                        assignLongText.TEXT_LANGUAGE = txt.TEXT_LANGUAGE.Trim();
                                        assignLongText.TEXT_ID = txt.TEXT_ID.Trim();
                                        assignLongText.LINE_ID = txt.LINE_ID;
                                        assignLongText.LINE_TEXT = txt.LINE_TEXT.Trim();
                                        assignLongText.CREATE_BY = param.UPDATE_BY;
                                        assignLongText.UPDATE_BY = param.UPDATE_BY;
                                        assignLongText.CREATE_DATE = DateTime.Now;
                                        assignLongText.UPDATE_DATE = DateTime.Now;

                                        doWork = true;
                                        context.ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT.Add(assignLongText);
                                        //ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT_SERVICE.SaveNoLog(assignLongText, context);
                                    }
                                    if (doWork) context.SaveChanges();

                                    #endregion

                                    if (so.BOM_ID > 0)
                                    {
                                        var soItemBom = listsoItemBom.Where(m => m.PO_COMPLETE_SO_ITEM_COMPONENT_ID == so.BOM_ID).FirstOrDefault();
                                        if (soItemBom != null)
                                        {
                                            ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT assignBOM = new ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT();

                                            assignBOM.ASSIGN_SO_ITEM_ID = assignItem.ASSIGN_SO_ITEM_ID;
                                            assignBOM.ARTWORK_PROCESS_SO_ID = so.ARTWORK_PROCESS_SO_ID;
                                            assignBOM.ARTWORK_REQUEST_ID = param.ARTWORK_REQUEST_ID;
                                            assignBOM.ARTWORK_SUB_ID = param.ARTWORK_SUB_ID;
                                            assignBOM.COMPONENT_ITEM = soItemBom.COMPONENT_ITEM.Trim();
                                            assignBOM.COMPONENT_MATERIAL = soItemBom.COMPONENT_MATERIAL.Trim();
                                            assignBOM.DECRIPTION = soItemBom.DECRIPTION.Trim();
                                            assignBOM.QUANTITY = soItemBom.QUANTITY;
                                            assignBOM.UNIT = soItemBom.UNIT.Trim();
                                            assignBOM.STOCK = soItemBom.STOCK.Trim();
                                            assignBOM.BOM_ITEM_CUSTOM_1 = string.Format("{0}", soItemBom.BOM_ITEM_CUSTOM_1).Trim();
                                            assignBOM.BOM_ITEM_CUSTOM_2 = soItemBom.BOM_ITEM_CUSTOM_2.Trim();
                                            assignBOM.BOM_ITEM_CUSTOM_3 = soItemBom.BOM_ITEM_CUSTOM_3.Trim();
                                            assignBOM.CREATE_BY = param.CREATE_BY;
                                            assignBOM.UPDATE_BY = param.UPDATE_BY;

                                            ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT_SERVICE.SaveNoLog(assignBOM, context);
                                            //update component

                                            soItemBom.IS_ASSIGN = "X";
                                            SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_SERVICE.UpdateNoLog(soItemBom, context);
                                            var pitem = CNService.GetAssignsoItem(soItem, context); //#
                                            if (pitem == 0)
                                            {
                                                soItem.IS_ASSIGN = "X";
                                                SAP_M_PO_COMPLETE_SO_ITEM_SERVICE.UpdateNoLog(soItem, context);
                                            }
                                        }
                                    }
                                    //ticket 437485
                                    var listSO = CNService.GetAssignOrder(soHeader.PO_COMPLETE_SO_HEADER_ID, context);
                                    if (listSO == 0)
                                    {
                                        soHeader.IS_ASSIGN = "X";
                                        SAP_M_PO_COMPLETE_SO_HEADER_SERVICE.UpdateNoLog(soHeader, context);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void DeleteAssignSalesOrder(ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 param, ARTWORKEntities context)
        {
            if (param != null)
            {
                int subID = param.ARTWORK_SUB_ID;
                CNService.DeleteAssignOrder(subID, context);

                context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_ASSIGN_SO_HEADER WHERE ARTWORK_SUB_ID  = '" + subID + "'");

                context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM WHERE ARTWORK_SUB_ID  = '" + subID + "'");

                context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_ASSIGN_SO_ITEM_COMPONENT WHERE ARTWORK_SUB_ID  = '" + subID + "'");

                context.Database.ExecuteSqlCommand("DELETE FROM ART_WF_ARTWORK_PROCESS_ASSIGN_SO_LONG_TEXT WHERE ARTWORK_SUB_ID  = '" + subID + "'");

            }
        }

        public static string GetBomCustom1Value(string param)
        {
            string value = "";

            if (String.IsNullOrEmpty(param))
            {
                return "";
            }

            if (param.Contains("MULTI"))
            {
                string[] values = param.Split(new string[] { "MULTI" }, StringSplitOptions.None);

                value = values[1].Trim();
            }
            else
            {
                string[] values = param.Split(new string[] { "NEW" }, StringSplitOptions.None);
                value = values[1].Trim();
            }

            return value;
        }

        private static int GetMaterialGroupID(string param, ARTWORKEntities context)
        {
            int matGroupID = 0;

            SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();
            characteristic.NAME = "ZPKG_SEC_GROUP";
            characteristic.DESCRIPTION = param;
            characteristic = SAP_M_CHARACTERISTIC_SERVICE.GetByItemContain(characteristic, context).FirstOrDefault();

            if (characteristic != null)
            {
                matGroupID = characteristic.CHARACTERISTIC_ID;
            }

            return matGroupID;
        }
    }
}
public class xecm_product
{
    public string PRODUCT_CODE { get; set; }
}
//public class SALES_ORDER_Assigns
//{
//    public string SALES_ORDER_NO { get; set; }
//    public Nullable<decimal> ITEM { get; set; }
//    public string PRODUCT_CODE { get; set; }
//    public string MATERIAL_DESCRIPTION { get; set; }

//}
