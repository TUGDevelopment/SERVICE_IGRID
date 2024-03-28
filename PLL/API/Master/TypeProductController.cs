using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;
using DAL;

namespace PLL.API
{
    public class TypeProductController : ApiController
    {
        [Route("api/master/typeproduct")]
        public ART_M_USER_TYPE_OF_PRODUCT_RESULT PostCheckListRequest(ART_M_USER_TYPE_OF_PRODUCT_REQUEST_LIST param)
        {
            ART_M_USER_TYPE_OF_PRODUCT_RESULT Results = new ART_M_USER_TYPE_OF_PRODUCT_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    foreach (var item in param.data)
                    {
                        var temp = ART_M_USER_TYPE_OF_PRODUCT_SERVICE.GetByItem(new ART_M_USER_TYPE_OF_PRODUCT() { TYPE_OF_PRODUCT_ID = item.TYPE_OF_PRODUCT_ID, USER_ID = item.USER_ID }, context);
                        if (temp.Count == 1)
                        {
                            item.ART_M_USER_TYPE_OF_PRODUCT_ID = temp.FirstOrDefault().ART_M_USER_TYPE_OF_PRODUCT_ID;
                        }

                        if (item.CHECKED)
                        {
                            ART_M_USER_TYPE_OF_PRODUCT_SERVICE.SaveOrUpdate(MapperServices.ART_M_USER_TYPE_OF_PRODUCT(item), context);
                        }
                        else
                        {
                            ART_M_USER_TYPE_OF_PRODUCT_SERVICE.DeleteByART_M_USER_TYPE_OF_PRODUCT_ID(item.ART_M_USER_TYPE_OF_PRODUCT_ID, context);
                        }

                    }
                }
                Results.status = "S";
                //Results.msg = MessageHelper.GetMessage("MSG_001");
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        // ticket 453346  by aof
        [Route("api/master/getusertypeofproduct")]
        public ART_M_USER_TYPE_OF_PRODUCT_RESULT GetUserTypeofProduct(int user_id,bool only_save)
        {
            ART_M_USER_TYPE_OF_PRODUCT_RESULT Results = new ART_M_USER_TYPE_OF_PRODUCT_RESULT();
            try
            {
               
                using (var context = new ARTWORKEntities())
                {

                    var list = (from u in context.ART_M_USER_TYPE_OF_PRODUCT
                             where u.USER_ID == user_id
                             select u).ToList();
                    List< ART_M_USER_TYPE_OF_PRODUCT_2> list2 = MapperServices.ART_M_USER_TYPE_OF_PRODUCT(list);
                    Results.data = list2;
                }

                Results.ONLY_SAVE = only_save;
                Results.status = "S";
               
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        // ticket 453346  by aof
    }
}
