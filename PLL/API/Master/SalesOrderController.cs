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
    public class SalesOrderController : ApiController
    {
        [Route("api/master/salesorder")]
        public SAP_M_PO_COMPLETE_SO_HEADER_RESULT PostSalesOrder(SAP_M_PO_COMPLETE_SO_HEADER_REQUEST_LIST param)
        {
            SAP_M_PO_COMPLETE_SO_HEADER_RESULT Results = new SAP_M_PO_COMPLETE_SO_HEADER_RESULT();
            try
            {

                Results =  SOHelper.GetSalesOrder(param);

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

        [Route("api/master/salesorg")]
        public SAP_M_PO_COMPLETE_SO_HEADER_RESULT GetSalesOrg([FromUri]SAP_M_PO_COMPLETE_SO_HEADER_REQUEST param)
        {
            return SOHelper.GetSalesOrg(param);
            
        }

        [Route("api/master/packagingmaterial")]
        public SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_RESULT GetPackagingMaterial([FromUri]SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT_REQUEST param)
        {
            return SOHelper.GetPackagingMaterial(param);
        }

        [Route("api/master/packagingmaterial_sap")]
        public XECM_M_PRODUCT5_RESULT GetPackagingMaterial_SAP([FromUri]XECM_M_PRODUCT5_REQUEST param)
        {
            return SOHelper.GetPackagingMaterial_SAP(param);

         
        }

    }
}
