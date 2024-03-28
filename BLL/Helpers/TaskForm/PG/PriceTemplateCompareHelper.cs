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
    public static class PriceTemplateCompareHelper
    {
        public static ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT GetPriceTemplateCompare(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_REQUEST param)
        {
            ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT Results = new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_RESULT();

            try
            {

                if (param == null || param.data == null)
                {
                    //  Results.data = MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_SERVICE.GetAll());
                }
                else
                {
                    Results.data = GetByItemContain(MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE(param.data));
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

        public static List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2> GetByItemContain(ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE Item)
        {
            List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2> listComparePrice = new List<ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_2>();
            using (ARTWORKEntities context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var listProcess = ART_WF_MOCKUP_PROCESS_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS() { MOCKUP_ID = Item.MOCKUP_ID }, context);
                    listProcess = listProcess.Where(m => m.IS_END == "X" && m.UPDATE_BY != -1 && m.PARENT_MOCKUP_SUB_ID != null).ToList();
                    listProcess = listProcess.Where(m => string.IsNullOrEmpty(m.REMARK_KILLPROCESS)).ToList();
                    var listProcessMockupSubId = listProcess.Select(m => m.MOCKUP_SUB_ID).ToList();

                    listComparePrice = MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE((from p in context.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE
                                                                                               where p.MOCKUP_ID == Item.MOCKUP_ID && p.USER_ID != null
                                                                                               select p).ToList());

                    var tempListComparePrice = MapperServices.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE((from p in context.ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE
                                                                                                       where p.MOCKUP_ID == Item.MOCKUP_ID && p.USER_ID != null
                                                                                                       select p).ToList());

                    listComparePrice = listComparePrice.Where(m => listProcessMockupSubId.Contains(m.MOCKUP_SUB_ID)).ToList();

                    listComparePrice = listComparePrice.OrderBy(m => m.MOCKUP_SUB_ID).ToList();
                    int old = 0;
                    int oldMockupSubId = 0;
                    if (listComparePrice.Count > 0)
                    {
                        ART_WF_MOCKUP_PROCESS tempProcess = new ART_WF_MOCKUP_PROCESS();
                        ART_M_USER_VENDOR vendorObj = new ART_M_USER_VENDOR();
                        for (int i = 0; i < listComparePrice.Count; i++)
                        {
                            if (old != listComparePrice[i].MOCKUP_SUB_ID)
                            {
                                old = listComparePrice[i].MOCKUP_SUB_ID;
                                tempProcess = ART_WF_MOCKUP_PROCESS_SERVICE.GetByMOCKUP_SUB_ID(listComparePrice[i].MOCKUP_SUB_ID, context);
                            }

                            if (string.IsNullOrEmpty(tempProcess.IS_END))
                            {
                                listComparePrice[i].PRICE = 0;
                            }

                            if (tempProcess.REMARK == "Manaul add price template.")
                            {
                                listComparePrice[i].IS_MANUAL = "X";
                            }

                            var vendor = tempProcess;

                            var temp2 = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_SUP_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_PG_SUP() { MOCKUP_ID = listComparePrice[i].MOCKUP_ID }, context).OrderBy(m => m.MOCKUP_SUB_ID).ToList();
                            if (temp2.LastOrDefault() != null)
                            {
                                listComparePrice[i].COMMENT_BY_PG_SUP = temp2.LastOrDefault().COMMENY_BY_PG_SUP;
                            }
                            listComparePrice[i].VENDOR_ID = vendor.CURRENT_VENDOR_ID;
                            listComparePrice[i].VENDOR_DISPLAY_TXT = CNService.GetVendorName(listComparePrice[i].VENDOR_ID, context);
                            listComparePrice[i].USER_DISPLAY_TXT = CNService.GetUserName(vendor.CURRENT_USER_ID, context);
                            var temp = ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR_SERVICE.GetByItem(new ART_WF_MOCKUP_PROCESS_PG_PRICE_TEMPLATE_BY_VENDOR() { MOCKUP_SUB_ID = listComparePrice[i].MOCKUP_SUB_ID }, context).FirstOrDefault();
                            if (temp != null)
                                listComparePrice[i].COMMENT_BY_VENDOR = temp.COMMENT_BY_VENDOR;

                            listComparePrice[i].SELECTED = "";
                            var cnt = tempListComparePrice.Where(m => m.VENDOR_ID == listComparePrice[i].VENDOR_ID && m.SELECTED == "X").Count();
                            if (cnt > 0)
                                listComparePrice[i].SELECTED = "X";
                            else
                                listComparePrice[i].SELECTED = "";

                            if (oldMockupSubId != listComparePrice[i].MOCKUP_SUB_ID)
                            {
                                oldMockupSubId = listComparePrice[i].MOCKUP_SUB_ID;
                                var max = listComparePrice.Where(m => m.VENDOR_ID == listComparePrice[i].VENDOR_ID).Select(m => m.ROUND).Max();
                                listComparePrice[i].ROUND = max + 1;
                            }
                            else
                            {
                                listComparePrice[i].ROUND = listComparePrice.Where(m => m.VENDOR_ID == listComparePrice[i].VENDOR_ID && m.MOCKUP_SUB_ID == oldMockupSubId).Select(m => m.ROUND).FirstOrDefault();
                            }
                        }
                    }
                }
            }

            return listComparePrice.OrderBy(m => m.VENDOR_DISPLAY_TXT).ThenBy(m => m.ROUND).ToList();
        }

    }
}
