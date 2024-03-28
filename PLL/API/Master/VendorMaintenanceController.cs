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
    public class VendorMaintenanceController : ApiController
    {
        [Route("api/master/vendor")]
        public ART_M_USER_VENDOR_RESULT PostCheckListRequest(ART_M_USER_VENDOR_REQUEST_LIST param)
        {
            ART_M_USER_VENDOR_RESULT Results = new ART_M_USER_VENDOR_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    if (param != null || param.data != null)
                    {
                        foreach (var item in param.data)
                        {
                            var temp = ART_M_USER_VENDOR_SERVICE.GetByItem(new ART_M_USER_VENDOR() { USER_ID = item.USER_ID }, context).FirstOrDefault();
                            if (item.VENDOR_ID != 0)
                            {
                                if (temp != null)
                                {
                                    item.USER_VENDOR_ID = temp.USER_VENDOR_ID;
                                    ART_M_USER_VENDOR_SERVICE.SaveOrUpdate(MapperServices.ART_M_USER_VENDOR(item), context);
                                }
                                else
                                    ART_M_USER_VENDOR_SERVICE.SaveOrUpdate(MapperServices.ART_M_USER_VENDOR(item), context);
                            }
                            else if (temp != null && item.VENDOR_ID == 0)
                            {
                                item.USER_VENDOR_ID = ART_M_USER_VENDOR_SERVICE.GetByItem(new ART_M_USER_VENDOR() { USER_ID = item.USER_ID }, context).FirstOrDefault().USER_VENDOR_ID;
                                ART_M_USER_VENDOR_SERVICE.DeleteByUSER_VENDOR_ID(item.USER_VENDOR_ID, context);
                            }
                        }
                    }

                    Results.status = "S";
                    //Results.msg = MessageHelper.GetMessage("MSG_001", context);
                }
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
