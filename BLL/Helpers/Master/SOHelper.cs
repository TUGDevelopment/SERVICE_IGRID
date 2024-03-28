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
    public class SOHelper
    {
        public static SAP_M_PO_COMPLETE_SO_HEADER_RESULT GetSalesOrder(SAP_M_PO_COMPLETE_SO_HEADER_REQUEST_LIST param)
        {
            SAP_M_PO_COMPLETE_SO_HEADER_RESULT Results = new SAP_M_PO_COMPLETE_SO_HEADER_RESULT();
            List<SAP_M_PO_COMPLETE_SO_HEADER_2> listSOResult = new List<SAP_M_PO_COMPLETE_SO_HEADER_2>();
            SAP_M_PO_COMPLETE_SO_HEADER_2 soHeader2 = new SAP_M_PO_COMPLETE_SO_HEADER_2();

            List<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2> listComponents = new List<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2>();
            List<SAP_M_PO_COMPLETE_SO_ITEM_2> listItems = new List<SAP_M_PO_COMPLETE_SO_ITEM_2>();

            try
            {
                if (param != null || param.data != null)
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            foreach (var iParam in param.data)
                            {
                                var soHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                                where h.SALES_ORDER_NO == iParam.SALES_ORDER_NO
                                                select h).FirstOrDefault();

                                soHeader2 = MapperServices.SAP_M_PO_COMPLETE_SO_HEADER(soHeader);

                                if (soHeader != null)
                                {
                                    var _soItems = (from t in context.SAP_M_PO_COMPLETE_SO_ITEM
                                                    where t.PO_COMPLETE_SO_HEADER_ID == soHeader.PO_COMPLETE_SO_HEADER_ID
                                                    select t).ToList();

                                    listItems = new List<SAP_M_PO_COMPLETE_SO_ITEM_2>();

                                    listItems = MapperServices.SAP_M_PO_COMPLETE_SO_ITEM(_soItems);

                                    soHeader2.ITEMS = listItems;

                                    if (listItems != null && listItems.Count > 0)
                                    {
                                        for (int i = 0; i <= listItems.Count - 1; i++)
                                        {
                                            int item_id = listItems[i].PO_COMPLETE_SO_ITEM_ID;
                                            string productCode = listItems[i].PRODUCT_CODE;

                                            var xecm = (from e in context.XECM_M_PRODUCT
                                                        where e.PRODUCT_CODE == productCode
                                                        select e).FirstOrDefault();

                                            if (xecm != null)
                                            {
                                                listItems[i].XECM_CONTAINER_TYPE = xecm.CONTAINER_TYPE;
                                                listItems[i].XECM_DRAINED_WEIGHT = xecm.DRAINED_WEIGHT;
                                                listItems[i].XECM_LID_TYPE = xecm.LID_TYPE;
                                                listItems[i].XECM_NET_WEIGHT = xecm.NET_WEIGHT;
                                                listItems[i].XECM_PACKING_STYLE = xecm.PACKING_STYLE;
                                                listItems[i].XECM_PACK_SIZE = xecm.PACK_SIZE;
                                                listItems[i].XECM_PRIMARY_SIZE = xecm.PRIMARY_SIZE;
                                            }

                                            var components = (from c in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                                                              where c.PO_COMPLETE_SO_ITEM_ID == item_id
                                                              select c).ToList();

                                            listItems[i].COMPONENTS = MapperServices.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT(components);
                                        }
                                    }
                                    listSOResult.Add(soHeader2);
                                }
                            }
                        }
                    }
                }

                Results.data = listSOResult;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
                return Results;
            }

            return Results;
        }

        public static SAP_M_PO_COMPLETE_SO_HEADER_RESULT GetSalesOrg(SAP_M_PO_COMPLETE_SO_HEADER_REQUEST param)
        {
            SAP_M_PO_COMPLETE_SO_HEADER_RESULT Results = new SAP_M_PO_COMPLETE_SO_HEADER_RESULT();
            List<SAP_M_PO_COMPLETE_SO_HEADER_2> listSO_2 = new List<SAP_M_PO_COMPLETE_SO_HEADER_2>();
            SAP_M_PO_COMPLETE_SO_HEADER_2 so_2 = new SAP_M_PO_COMPLETE_SO_HEADER_2();
            try
            {

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        var soHeader = context.SAP_M_PO_COMPLETE_SO_HEADER.GroupBy(so => so.SALES_ORG).Select(so => so.Key).ToList();
                        foreach (var i in soHeader)
                        {
                            so_2 = new SAP_M_PO_COMPLETE_SO_HEADER_2
                            {
                                ID = Convert.ToInt32(i),
                                DISPLAY_TXT = i
                            };

                            listSO_2.Add(so_2);
                        }

                        Results.data = listSO_2.OrderBy(so => so.DISPLAY_TXT).ToList();
                        if (param != null && param.data != null)
                        {
                            if (param.data.DISPLAY_TXT != null && param.data.DISPLAY_TXT != "")
                            {
                                var filteredUserList = listSO_2.Where(s => (s.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))).OrderBy(so => so.DISPLAY_TXT).ToList();
                                Results.data = filteredUserList;
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

        public static SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_RESULT GetPackagingMaterial(SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_REQUEST param)
        {
            SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_RESULT Results = new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_RESULT();
            List<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2> listSOItem_2 = new List<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2>();
            SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2 soitem_2 = new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2();
            try
            {

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        listSOItem_2 = (from p in context.SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
                                        where p.COMPONENT_MATERIAL.StartsWith("5") && p.COMPONENT_MATERIAL.Length == 18
                                        select new SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_2() { ID = p.COMPONENT_MATERIAL, DISPLAY_TXT = p.COMPONENT_MATERIAL + ":" + p.DECRIPTION }).Distinct().OrderBy(p => p.DISPLAY_TXT).ToList();

                        Results.data = listSOItem_2;
                        if (param != null && param.data != null)
                        {
                            if (param.data.DISPLAY_TXT != null && param.data.DISPLAY_TXT != "")
                            {
                                var filteredUserList = listSOItem_2.Where(s => (s.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))).ToList();
                                Results.data = filteredUserList;
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

        public static XECM_M_PRODUCT5_RESULT GetPackagingMaterial_SAP(XECM_M_PRODUCT5_REQUEST param)
        {
            XECM_M_PRODUCT5_RESULT Results = new XECM_M_PRODUCT5_RESULT();
            List<XECM_M_PRODUCT5_2> listItem = new List<XECM_M_PRODUCT5_2>();
            XECM_M_PRODUCT5_2 item = new XECM_M_PRODUCT5_2();
            try
            {

                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        listItem = (from p in context.XECM_M_PRODUCT5
                                    where p.PRODUCT_CODE.StartsWith("5") && p.PRODUCT_CODE.Length == 18
                                        select new XECM_M_PRODUCT5_2() { ID = p.PRODUCT_CODE, DISPLAY_TXT = p.PRODUCT_CODE + ":" + p.PRODUCT_DESCRIPTION }).Distinct().OrderBy(p => p.DISPLAY_TXT).ToList();

                        Results.data = listItem;
                        if (param != null && param.data != null)
                        {
                            if (param.data.DISPLAY_TXT != null && param.data.DISPLAY_TXT != "")
                            {
                                var filteredUserList = listItem.Where(s => (s.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))).ToList();
                                Results.data = filteredUserList;
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
