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
    public class VendorPackagingMasterController : ApiController
    {
        [Route("api/master/packagingtype")]
        public ART_M_VENDOR_MATGROUP_RESULT PostPackagingType(ART_M_VENDOR_MATGROUP_REQUEST_LIST param)
        {
            ART_M_VENDOR_MATGROUP_RESULT Results = new ART_M_VENDOR_MATGROUP_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    foreach (var item in param.data)
                    {
                        var temp = ART_M_VENDOR_MATGROUP_SERVICE.GetByItem(new ART_M_VENDOR_MATGROUP() { VENDOR_ID = item.VENDOR_ID, MATGROUP_ID = item.MATGROUP_ID }, context);
                        if (temp.Count == 1)
                        {
                            item.ART_M_VENDOR_MATGROUP_ID = temp.FirstOrDefault().ART_M_VENDOR_MATGROUP_ID;
                        }

                        if (item.CHECKED)
                        {
                            ART_M_VENDOR_MATGROUP_SERVICE.SaveOrUpdate(MapperServices.ART_M_VENDOR_MATGROUP(item), context);
                        }
                        else
                        {
                            ART_M_VENDOR_MATGROUP_SERVICE.DeleteByART_M_VENDOR_MATGROUP_ID(item.ART_M_VENDOR_MATGROUP_ID, context);
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
