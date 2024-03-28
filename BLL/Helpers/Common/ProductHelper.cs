using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace BLL.Helpers
{
    public class ProductHelper
    {
        public static XECM_M_PRODUCT_RESULT GetProduct(XECM_M_PRODUCT_REQUEST_LIST param)
        {
            XECM_M_PRODUCT_RESULT Results = new XECM_M_PRODUCT_RESULT();
            List<XECM_M_PRODUCT_2> listProduct2 = new List<XECM_M_PRODUCT_2>();
            XECM_M_PRODUCT_2 product2 = new XECM_M_PRODUCT_2();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param != null && param.data != null && param.data.Count > 0)
                        {
                            if (param.data.Select(m => m.PRODUCT_CODE).Distinct().ToList().Count != param.data.Select(m => m.PRODUCT_CODE).ToList().Count)
                            {
                                Results.status = "E";
                                Results.msg = "Cannot input duplicate product code.";
                                return Results;
                            }
                            
                            foreach (XECM_M_PRODUCT_2 itemProduct in param.data)
                            {
                                var tmpResults = MapperServices.XECM_M_PRODUCT(XECM_M_PRODUCT_SERVICE.GetByItem(MapperServices.XECM_M_PRODUCT(itemProduct), context));

                                foreach (XECM_M_PRODUCT_2 itemTmpResult in tmpResults)
                                {
                                    product2 = new XECM_M_PRODUCT_2();
                                    product2 = itemTmpResult;
                                    product2.ROW = itemProduct.ROW;
                                    product2.PRODUCT_CODE_ID = itemTmpResult.XECM_PRODUCT_ID;
                                    product2.PRODUCT_DESCRIPTION = itemTmpResult.PRODUCT_DESCRIPTION;
                                    //product2.PRODUCT_TYPE = "";
                                    //var arrplant = param.data.Select(m => m.PRODUCTION_PLANT).ToList();
                                    //string[] split = param.data[0].PRODUCTION_PLANT.ToString().Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                    //foreach (string s in split)
                                    //{
                                    //    if (s.Trim() != "") {
                                    //        var check_product_vap = SAP_M_PRODUCT_VAP_SERVICE.GetByItem(new SAP_M_PRODUCT_VAP { PRODUCT_CODE = itemTmpResult.PRODUCT_CODE.Substring(2, 1), PLANT = s }, context).FirstOrDefault();
                                    //        if(check_product_vap!=null)
                                    //            product2.PRODUCT_TYPE = check_product_vap.PRODUCT_TYPE;
                                    //    }
                                    //}
                                    if (param.data[0].PRODUCT_TYPE=="FFC" & itemTmpResult.PRODUCT_CODE.Substring(1,1)!="E")
                                    {
                                        Results.status = "E";
                                        Results.msg = itemTmpResult.PRODUCT_CODE + " Product code is not FFC";
                                        return Results;
                                    }
                                    if(param.data[0].PRODUCTION_PLANT !=null)
                                    product2.PRODUCT_TYPE = CNService.Getcheck_product_vap(itemTmpResult.PRODUCT_CODE, param.data[0].PRODUCTION_PLANT.ToString());
                                    listProduct2.Add(product2);
                                }


                            }
                        }
                    }
                }
                Results.data = listProduct2;
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static CN_VAP_MODEL_RESULT GetProduct_vap(XECM_M_PRODUCT_REQUEST_LIST param)
        {
            CN_VAP_MODEL_RESULT Results = new CN_VAP_MODEL_RESULT();
            try
            {
                //Results.data = listProduct2;
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
