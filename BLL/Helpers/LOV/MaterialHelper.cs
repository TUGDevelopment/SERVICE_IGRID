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
    public class MaterialHelper
    {
        public static XECM_M_PRODUCT_RESULT GetProductSO(XECM_M_PRODUCT_REQUEST param)
        {
            XECM_M_PRODUCT_RESULT Results = new XECM_M_PRODUCT_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        Results.data = (from p in context.V_SAP_SALES_ORDER where (p.PRODUCT_CODE.StartsWith("3") && !p.PRODUCT_CODE.StartsWith("3W")) || ((p.PRODUCT_CODE.StartsWith("5") && p.PRODUCT_CODE.Length == 18)) select new XECM_M_PRODUCT_2() { DISPLAY_TXT = p.PRODUCT_CODE + ":" + p.MATERIAL_DESCRIPTION }).Distinct().ToList();
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

        public static XECM_M_PRODUCT_RESULT GetBomSO(XECM_M_PRODUCT_REQUEST param)
        {
            XECM_M_PRODUCT_RESULT Results = new XECM_M_PRODUCT_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        Results.data = (from p in context.V_SAP_SALES_ORDER where p.COMPONENT_MATERIAL.StartsWith("5") && p.COMPONENT_MATERIAL.Length == 18 select new XECM_M_PRODUCT_2() { DISPLAY_TXT = p.COMPONENT_MATERIAL + ":" + p.DECRIPTION }).Distinct().ToList();
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

