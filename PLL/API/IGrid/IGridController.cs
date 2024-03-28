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
//using PLL.Controllers;

namespace PLL.API
{
    public class IGridApiController : ApiController
    {

        //[Route("api/taskform/igrid/result")]
        //public IGRID_RESULT GetResult([FromUri] IGRID_REQUEST param)
        //{
        //    return IGridFormHelper.GetWorkflowPending(param);
        //}

        [Route("api/taskform/igrid/attachment/info")]
        public TblFiles_RESULT GetIGridAttachmentInfo([FromUri]TblFiles_REQUEST param)
        {
          
            return IGridFormHelper.GetIGridAttachmentInfo(param);
        }

        [Route("api/taskform/igrid/attachment/delete")]
        public TblFiles_RESULT DeleteIGridUploadFile(TblFiles_REQUEST param)
        {
            return IGridFormHelper.DeleteIGridAttachmentFile(param);
        }

        [Route("api/taskform/igrid/ProductCodeIGrid")]
        public ProductCode_RESULT GetProductCodeIGrid([FromUri] ProductCode_REQUEST param)
        {
            return IGridFormHelper.GetProductCodeIGrid(param);
        }

        [Route("api/taskform/igrid/packingstylefg")]
        public XECM_M_PRODUCT_RESULT GetPackingStyleByFG([FromUri]  XECM_M_PRODUCT_REQUEST param)
        {
            return IGridFormHelper.GetPackingStyleByFG(param);
        }

        [Route("api/taskform/igrid/lov/PlantRegisteredIGrid")]
        public PlantRegistered_RESULT GetPlantRegisteredIGridLOV([FromUri] PlantRegistered_REQUEST param)
        {
            return IGridFormHelper.GetPlantRegisteredFormLOV(param);
        }
    
        [Route("api/taskform/igrid/lov/CompanyAddressIGrid")]
        public CompanyAddress_RESULT GetCompanyAddressIGridLOV([FromUri] CompanyAddress_REQUEST param)
        {
            return IGridFormHelper.GetCompanyAddressFormLOV(param);
        }


        //[Route("api/taskform/igrid/lov/GetTransApproveIGrid")]
        //public TransApprove_RESULT GetTransApproveIGrid([FromUri] TransApprove_REQUEST param)
        //{
        //    return IGridFormHelper.GetTransApproveIGrid(param);
        //}


    }
}