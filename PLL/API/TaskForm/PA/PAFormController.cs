using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;
using WebServices.Helper;
using DAL;

namespace PLL.API
{
    public class PAFormController : ApiController
    {
        [Route("api/taskform/igrid/history")]
        public History_RESULT GetHistory([FromUri] History_REQUEST param)
        {
            return IGridFormHelper.GetHistoryForm(param);
        }

        [Route("api/taskform/igrid/primarysize")]
        public PrimarySize_RESULT GetPrimarySize([FromUri] PrimarySize_REQUEST param)
        {
            return IGridFormHelper.GetPrimaryForm(param);
        }
        [Route("api/taskform/igrid/info")]
        public SapMaterial_RESULT GetPAForm([FromUri] SapMaterial_REQUEST param)
        {
            return IGridFormHelper.GetIGridSAPMaterial(param);
        }
        [Route("api/taskform/igrid/info")]
        public SapMaterial_RESULT PostIGridSAPMaterial(SapMaterial_REQUEST param)
        {
            return IGridFormHelper.SaveIGridSAPMaterial(param);
        }

        [Route("api/taskform/igrid/savecompleteinfogroup")]
        public SapMaterial_RESULT PostCompleteInfoGroup(SapMaterial_REQUEST param)
        {
            return IGridFormHelper.saveCompleteInfoGroup(param);
        }



        [Route("api/taskform/pa/info")]
        public ART_WF_ARTWORK_PROCESS_PA_RESULT GetPAForm([FromUri]ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            return PAFormHelper.GetPAForm(param);
        }

        [Route("api/taskform/pa/info")]
        public ART_WF_ARTWORK_PROCESS_PA_RESULT PostPAForm(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            return PAFormHelper.SavePAForm(param);
        }

        [Route("api/taskform/pa/plant/delete")]
        public ART_WF_ARTWORK_PROCESS_PA_PLANT_RESULT DeletePAPlant(ART_WF_ARTWORK_PROCESS_PA_PLANT_REQUEST param)
        {
            return PAFormHelper.DeletePlant(param);
        }

        [Route("api/taskform/pa/product/delete")]
        public ART_WF_ARTWORK_PROCESS_PA_PRODUCT_RESULT DeletePAProduct(ART_WF_ARTWORK_PROCESS_PA_PRODUCT_REQUEST param)
        {
            return PAFormHelper.DeleteProduct(param);
        }

        [Route("api/taskform/pa/faozone/delete")]
        public ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_RESULT DeleteFAOZone(ART_WF_ARTWORK_PROCESS_PA_FAO_ZONE_REQUEST param)
        {
            return PAFormHelper.DeleteFAOZone(param);
        }

        [Route("api/taskform/pa/catchingarea/delete")]
        public ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_RESULT DeleteCatchingArea(ART_WF_ARTWORK_PROCESS_PA_CATCHING_AREA_REQUEST param)
        {
            return PAFormHelper.DeleteCatchingArea(param);
        }
        // ticke#425737 added by aof 
        [Route("api/taskform/pa/catchingmethod/delete")]
        public ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_RESULT DeleteCatchingMethod(ART_WF_ARTWORK_PROCESS_PA_CATCHING_METHOD_REQUEST param)
        {
            return PAFormHelper.DeleteCatchingMethod(param); 
        }
        // ticke#425737 added by aof 
        [Route("api/taskform/pa/symbol/delete")]
        public ART_WF_ARTWORK_PROCESS_PA_SYMBOL_RESULT DeleteSymbol(ART_WF_ARTWORK_PROCESS_PA_SYMBOL_REQUEST param)
        {
            return PAFormHelper.DeleteSymbol(param);
        }
      

        [Route("api/taskform/pa/readycreatepo")]
        public ART_WF_ARTWORK_PROCESS_PA_RESULT PostReadyCreatePO(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            return PAFormHelper.SaveReadyCreatePO(param);
        }

        [Route("api/taskform/pa/shadelimit")]
        public ART_WF_ARTWORK_PROCESS_PA_RESULT PostShadeLimit(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            return PAFormHelper.SaveShadeLimit(param);
        }

        [Route("api/taskform/pa/receiveshadelimit")]
        public ART_WF_ARTWORK_PROCESS_PA_RESULT PostReceiveShadeLimit(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            return PAFormHelper.SaveReceiveShadeLimit(param);
        }

        [Route("api/taskform/pa/changepoint")]
        public ART_WF_ARTWORK_PROCESS_PA_RESULT PostChangePoint(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            return PAFormHelper.SaveChangePoint(param);
        }

        [Route("api/taskform/pa/sendpp/info")]
        public PP_RESULT GetPP([FromUri]PP_REQUEST param)
        {
            if (param == null) param = new PP_REQUEST();
            if (param.data == null) param.data = new PP_MODEL();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    param.data.CURRENT_USER_ID = CNService.getCurrentUser(context);
                }
            }
            return PAFormHelper.GetWorkflowForPP(param);
        }

        [Route("api/taskform/pa/sendpp/submit")]
        public ART_WF_ARTWORK_PROCESS_PP_BY_PA_RESULT PostSendToPP(ART_WF_ARTWORK_PROCESS_PP_BY_PA_REQUEST_LIST param)
        {
            return PPByPAHelper.SaveMultiPPByPA(param);
        }

        [Route("api/taskform/pa/sendbackmk")]
        public ART_WF_ARTWORK_PROCESS_RESULT PostSendBackToMK(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            return PAFormHelper.SendBackMK(param);
        }

        [Route("api/taskform/pa/terminate")]
        public ART_WF_ARTWORK_PROCESS_RESULT PostTerminateWF(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            return PAFormHelper.TerminatePAForm(param);
        }

        [Route("api/taskform/pa/killprocess")]
        public ART_WF_ARTWORK_PROCESS_RESULT KillProcessHistory(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            return PAFormHelper.KillProcessHistory(param);
        }

        [Route("api/taskform/pa/complete")]
        public ART_WF_ARTWORK_PROCESS_RESULT PostCompleteWF(ART_WF_ARTWORK_PROCESS_REQUEST param)
        {
            return PAFormHelper.CompletePAForm(param);
        }

        [Route("api/taskform/pa/requestmaterial")]
        public MM65_RESULT PostCompleteWF(MM65_REQUEST param)
        {
            //return MM_65_Hepler.RequestMaterial(param);  
            return MM_65_Hepler.RequestMaterial2(param);
        }

        [Route("api/taskform/pa/validatebrandrefmatwithrequestform")]
        public MM65_RESULT GetValidateBrandRefMatWithRequestForm([FromUri]MM65_REQUEST param)
        {
            return MM_65_Hepler.ValidateBrandRefMatWithRequestForm(param);
        }

        [Route("api/taskform/pa/suggestmaterial/info")]
        public SAP_M_ORDER_BOM_RESULT GetSuggestMaterial([FromUri]SAP_M_ORDER_BOM_REQUEST param)
        {
            return PAFormHelper.GetSuggestMaterial(param);
        }


        //ticket#437764 added by aof on 30/03/2021
        [Route("api/taskform/pa/suggestmaterial/getartworkprocesspa")]
        public ART_WF_ARTWORK_PROCESS_PA_RESULT GetArtowrkProcessPA([FromUri]ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            return PAFormHelper.GetArtWorkProcessPA(param);
        }
        //ticket#437764 added by aof on 30/03/2021

        [Route("api/taskform/pa/suggestmaterial/delete")]
        public ART_WF_ARTWORK_PROCESS_PA_RESULT DeleteSuggestMaterial(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            return PAFormHelper.DeleteSuggestMaterial(param);
        }

        [Route("api/taskform/pa/suggestmaterial/selected")]
        public SAP_M_ORDER_BOM_RESULT PostSuggestMaterial(SAP_M_ORDER_BOM_REQUEST param)
        {
            return PAFormHelper.SaveSuggestMaterial(param);
        }

        [Route("api/taskform/pa/retrivematerial")]
        public ART_WF_ARTWORK_PROCESS_PA_RESULT PostRetriveMaterial(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            return PAFormHelper.RetriveMaterial(param);
        }

        [Route("api/taskform/pa/changematerialgroup")]
        public ART_WF_ARTWORK_PROCESS_PA_RESULT GetChangeMaterialGroup([FromUri]ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            return PAFormHelper.GetChangeMaterialGroup(param);
        }

        [Route("api/taskform/pa/copypadata")]
        public ART_WF_ARTWORK_PROCESS_PA_RESULT PostCopyPADataFromAW(ART_WF_ARTWORK_PROCESS_PA_REQUEST param)
        {
            return PAFormHelper.CopyPADataFromAW(param);
        }

        [Route("api/replacemat")]
        public ART_WF_ARTWORK_PROCESS_PA_RESULT GetReplaceMat(string wfno, string mat)
        {
            return PAFormHelper.ReplaceMat(wfno, mat);
        }
    }
}
