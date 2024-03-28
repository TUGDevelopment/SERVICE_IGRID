using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services;
using DAL;
using DAL.Model;

namespace BLL.Helpers
{
    public class BrandHelper
    {
        public static SAP_M_BRAND_RESULT GetBrand(SAP_M_BRAND_REQUEST param)
        {
            SAP_M_BRAND_RESULT Results = new SAP_M_BRAND_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            SAP_M_BRAND_2 brand_2 = new SAP_M_BRAND_2();
                            brand_2.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_BRAND(SAP_M_BRAND_SERVICE.GetByItem(MapperServices.SAP_M_BRAND(brand_2), context));
                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_BRAND(SAP_M_BRAND_SERVICE.GetByItem(MapperServices.SAP_M_BRAND(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].BRAND_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].MATERIAL_GROUP.Trim() + ":" + Results.data[i].DESCRIPTION.Trim();
                            }

                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                Results.data = (from u1 in Results.data
                                                where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                                select u1).ToList();
                            }

                            Results.data = Results.data.OrderBy(x => x.DISPLAY_TXT).ToList();
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

        public static SAP_M_BRAND_RESULT GetBrandSO(SAP_M_BRAND_REQUEST param)
        {
            SAP_M_BRAND_RESULT Results = new SAP_M_BRAND_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        Results.data = (from p in context.V_SAP_SALES_ORDER where p.BRAND_ID != "" select new SAP_M_BRAND_2() { DISPLAY_TXT = p.BRAND_ID + ":" + p.BRAND_DESCRIPTION }).Distinct().ToList();
                        int i = 1;
                        foreach (var item in Results.data)
                        {
                            item.ID = i;
                            i++;
                        }
                    }
                }

                if (Results.data.Count > 0)
                {
                    if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                    {
                        Results.data = (from u1 in Results.data
                                        where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                        select u1).ToList();
                    }

                    Results.data = Results.data.OrderBy(x => x.DISPLAY_TXT).ToList();
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


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BLL.Services;
//using DAL.Model;

//namespace BLL.Helpers
//{
//    public class BrandHelper
//    {
//        public static SAP_M_CHARACTERISTIC_RESULT GetBrand(SAP_M_CHARACTERISTIC_REQUEST param)
//        {
//            SAP_M_CHARACTERISTIC_RESULT Results = new SAP_M_CHARACTERISTIC_RESULT();

//            try
//            {
//                if (param == null || param.data == null)
//                {
//                    SAP_M_CHARACTERISTIC_2 characteristic = new SAP_M_CHARACTERISTIC_2();
//                    characteristic.NAME = "ZPKG_SEC_BRAND";

//                    Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItem(MapperServices.SAP_M_CHARACTERISTIC(characteristic)));
//                }
//                else
//                {
//                    param.data.NAME = "ZPKG_SEC_BRAND";

//                    Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItemContain(MapperServices.SAP_M_CHARACTERISTIC(param.data)));
//                }

//                Results.status = "S";

//                if (Results.data.Count > 0)
//                {
//                    for (int i = 0; i < Results.data.Count; i++)
//                    {
//                        Results.data[i].ID = Results.data[i].CHARACTERISTIC_ID;
//                        Results.data[i].DISPLAY_TXT =
//                        Results.data[i].VALUE.Trim().ToLower() == Results.data[i].DESCRIPTION.Trim().ToLower() ?
//                        Results.data[i].DESCRIPTION : Results.data[i].VALUE + ":" + Results.data[i].DESCRIPTION;
//                    }

//                    if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
//                    {
//                        Results.data = (from u1 in Results.data
//                                        where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
//                                        select u1).ToList();
//                    }

//                    Results.data = Results.data.OrderBy(x => x.VALUE).ToList();
//                }
//            }
//            catch (Exception ex)
//            {
//                Results.status = "E";
//                Results.msg = CNService.GetErrorMessage(ex);
//            }

//            return Results;
//        }
//    }
//}
