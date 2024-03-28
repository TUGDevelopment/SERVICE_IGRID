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
using BLL.Helpers.Master;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace PLL.API
{
    public class MasterController : ApiController
    {
        // GET: Master

        [Route("api/lov/authrolizeeditmaster")]
        public ulogin_RESULT GetAuthrolizeEditMaster([FromUri] ulogin_REQUEST param)
        {
            return SelectMasterHelper.GetAuthrolizeEditMaster(param);
        }


        [Route("api/lov/selectmaster")]
        public SelectMaster_RESULT GetSelectMaster([FromUri] SelectMaster_REQUEST param)
        {
            return SelectMasterHelper.BuildSelectMaster(param);
        }
        [Route("api/lov/condition")]
        public Condition_RESULT GetCondition([FromUri] Condition_REQUEST param)
        {
            return SelectMasterHelper.BuildCondition(param);
        }
        [Route("api/lov/by")]
        public Condition_RESULT GetBy([FromUri] Condition_REQUEST param)
        {
            return SelectMasterHelper.GetBy(param);
        }
        [Route("api/lov/byIGrid")]
        public Condition_RESULT GetByIGrid([FromUri] Condition_REQUEST param)
        {
            return SelectMasterHelper.GetByIGrid(param);
        }
        [Route("api/lov/savemaster")]
        public MasterObject_RESULT PostSaveMasterForm(MasterObject_REQUEST param)
        {
            return IGridFormHelper.SaveSaveMasterForm(param);
        }
        [Route("api/lov/primarysizeIGrid")]
        public PrimarySize_RESULT GetprimarysizeIGrid([FromUri] PrimarySize_REQUEST param)
        {
            return IGridFormHelper.GetPrimaryForm(param);
        }
        [Route("api/lov/primarysizeIGrid2")]
        public PrimarySize_RESULT GetprimarysizeIGrid2([FromUri] PrimarySize_REQUEST param)
        {
            return IGridFormHelper.GetPrimaryForm2(param);
        }
        //public IHttpActionResult GetprimarysizeIGrid()
        //{
        //    string strConn = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //    using (SqlConnection con = new SqlConnection(strConn))
        //    {

        //        con.Open();
        //        string strQuery = @"select * from MasPrimarySize";
        //        System.Data.DataSet ds = new System.Data.DataSet();
        //        SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
        //        // Fill the dataset.
        //        oAdapter.Fill(ds);
        //        con.Close();
        //        return Json(ds);
        //    }

        //}
        [Route("api/lov/packtypeIGrid")]
        public MaterialClass_RESULT GetMaterialClass([FromUri] MaterialClass_REQUEST param)
        {
            return IGridFormHelper.GetMaterialClass(param);
        }
        [Route("api/lov/TypeofPrimaryIGrid")]
        public TypeofPrimary_RESULT GetTypeofPrimaryIGrid([FromUri] TypeofPrimary_REQUEST param)
        {
            return IGridFormHelper.GetTypeofPrimary(param);
        }
        [Route("api/lov/brandIGrid")]
        public Brand_RESULT GetBrandIGrid([FromUri] Brand_REQUEST param)
        {
            return IGridFormHelper.GetBrand(param);
        }
        [Route("api/lov/brandIGrid2")]
        public Brand_RESULT GetBrandIGrid2([FromUri] Brand_REQUEST param)
        {
            return IGridFormHelper.GetBrand2(param);
        }
        [Route("api/lov/SustainPlasticIGrid")]
        public SustainPlastic_RESULT GetSustainPlasticIGrid([FromUri] SustainPlastic_REQUEST param)
        {
            return IGridFormHelper.GetSustainPlasticForm(param);
        }

        [Route("api/lov/SustainMaterialIGrid")]
        public SustainMaterial_RESULT GetSustainMaterialIGrid([FromUri] SustainMaterial_REQUEST param)
        {
            return IGridFormHelper.GetSustainMaterialForm(param);
        }
        [Route("api/lov/SustainCertSourcingIGrid")]
        public SustainCertSourcing_RESULT GetSustainCertSourcingIGrid([FromUri] SustainCertSourcing_REQUEST param)
        {
            return IGridFormHelper.GetSustainCertSourcingForm(param);
        }
        [Route("api/lov/SpecieIGrid")]
        public Specie_RESULT GetSpecieIGrid([FromUri] Specie_REQUEST param)
        {
            return IGridFormHelper.GetSpecieForm(param);
        }
        [Route("api/lov/PrintingSystemIGrid")]
        public PrintingSystem_RESULT GetPrintingSystemIGrid([FromUri] PrintingSystem_REQUEST param)
        {
            return IGridFormHelper.GetPrintingSystemForm(param);
        }
        [Route("api/lov/DirectionIGrid")]
        public Direction_RESULT GetDirectionIGrid([FromUri] Direction_REQUEST param)
        {
            return IGridFormHelper.GetDirectionForm(param);
        }
        [Route("api/lov/ScientificNameIGrid")]
        public ScientificName_RESULT GetScientificNameIGrid([FromUri] ScientificName_REQUEST param)
        {
            return IGridFormHelper.GetScientificNameForm(param);
        }
        [Route("api/lov/CatchingMethodIGrid")]
        public CatchingMethod_RESULT GetCatchingMethodIGrid([FromUri] CatchingMethod_REQUEST param)
        {
            return IGridFormHelper.GetCatchingMethodForm(param);
        }
        [Route("api/lov/CatchingAreaIGrid")]
        public CatchingArea_RESULT GetCatchingAreaIGrid([FromUri] CatchingArea_REQUEST param)
        {
            return IGridFormHelper.GetCatchingAreaForm(param);
        }
        [Route("api/lov/WHManagementIGrid")]
        public WHManagement_RESULT GetWHManagementIGrid([FromUri] WHManagement_REQUEST param)
        {
            return IGridFormHelper.GetWHManagementForm(param);
        }
        [Route("api/lov/ProductGroupIGrid")]
        public ProductGroup_RESULT GetProductGroupIGrid([FromUri] ProductGroup_REQUEST param)
        {
            return IGridFormHelper.GetProductGroupForm(param);
        }
        [Route("api/lov/uloginIGrid")]
        public ulogin_RESULT GetuloginIGrid([FromUri] ulogin_REQUEST param)
        {
            return IGridFormHelper.GetuloginForm(param);
        }
        [Route("api/lov/AssignIGrid")]
        public Assign_RESULT GetAssignIGrid([FromUri] Assign_REQUEST param)
        {
            return IGridFormHelper.GetAssignForm(param);
        }
        [Route("api/lov/VendorIGrid")]
        public Vendor_RESULT GetVendorIGrid([FromUri] Vendor_REQUEST param)
        {
            return IGridFormHelper.GetVendorForm(param);
        }
        [Route("api/lov/PlantRegisteredIGrid")]
        public PlantRegistered_RESULT GetPlantRegisteredIGrid([FromUri] PlantRegistered_REQUEST param)
        {
            return IGridFormHelper.GetPlantRegisteredForm(param);
        }
        [Route("api/lov/PackStyleIGrid")]
        public PackStyle_RESULT GetPackStyleIGrid([FromUri] PackStyle_REQUEST param)
        {
            return IGridFormHelper.GetPackStyleForm(param);
        }
 
        [Route("api/lov/CompanyAddressIGrid")]
        public CompanyAddress_RESULT GetCompanyAddressIGrid([FromUri] CompanyAddress_REQUEST param)
        {
            return IGridFormHelper.GetCompanyAddressForm(param);
        }
        [Route("api/lov/StyleofPrintingIGrid")]
        public StyleofPrinting_RESULT GetStyleofPrintingIGrid([FromUri] StyleofPrinting_REQUEST param)
        {
            return IGridFormHelper.GetStyleofPrintingForm(param);
        }
        [Route("api/lov/FluteIGrid")]
        public Flute_RESULT GetFluteIGrid([FromUri] Flute_REQUEST param)
        {
            return IGridFormHelper.GetFluteForm(param);
        }

        [Route("api/lov/TypeOfandBrandIGrid")]
        public TypeOf_RESULT GetTypeOfandBrandIGrid([FromUri] TypeOf_REQUEST param)
        {
            return IGridFormHelper.GetDescriptionTextByTypeOfBrand(param);
        }

        [Route("api/lov/TypeOfIGrid")]
        public TypeOf_RESULT GetTypeOfIGrid([FromUri] TypeOf_REQUEST param)
        {
            return IGridFormHelper.GetTypeOfForm(param);
        }
        [Route("api/lov/typeof2IGrid")]
        public TypeOf_RESULT GetTypeOf2IGrid([FromUri] TypeOf_REQUEST param)
        {
            return IGridFormHelper.GetTypeOfForm(param);
        }
        //
        [Route("api/lov/CompanyNameIGrid")]
        public PlantRegistered_RESULT GetCompanyNameIGrid([FromUri] PlantRegistered_REQUEST param)
        {
            return IGridFormHelper.GetCompanyNameForm(param);
        }
        [Route("api/lov/CompanyAddresIGrid2")]
        public PlantRegistered_RESULT GetCompanyAddresIGrid2([FromUri] PlantRegistered_REQUEST param)
        {
            return IGridFormHelper.GetCompanyAddressForm2(param);
        }
        [Route("api/lov/dicutIGrid")]
        public RSCDICUT_RESULT GetdicutIGrid([FromUri] RSCDICUT_REQUEST param)
        {
            return IGridFormHelper.GetdicutForm(param);
        }
        
        [Route("api/lov/RollSheet")]
        public RollSheet_RESULT GetRollSheet([FromUri] RollSheet_REQUEST param)
        {
            return IGridFormHelper.GetRollSheetForm(param);
        }
        [Route("api/lov/GradeofIGrid")]
        public Gradeof_RESULT GetGradeofIGrid([FromUri] Gradeof_REQUEST param)
        {
            return IGridFormHelper.GetGradeofForm(param);
        }
        [Route("api/lov/TotalColourIGrid")]
        public TotalColour_RESULT GetTotalColourIGrid([FromUri] TotalColour_REQUEST param)
        {
            return IGridFormHelper.GetTotalColourForm(param);
        }
        [Route("api/lov/ProcessColourIGrid")]
        public ProcessColour_RESULT GetProcessColourIGrid([FromUri] ProcessColour_REQUEST param)
        {
            return IGridFormHelper.GetProcessColourForm(param);
        }
        [Route("api/lov/PMSColourIGrid")]
        public PMSColour_RESULT GetPMSColourIGrid([FromUri] PMSColour_REQUEST param)
        {
            return IGridFormHelper.GetPMSColourForm(param);
        }
        [Route("api/lov/PrimaryTypeIGrid")]
        public PrimaryType_RESULT GetPrimaryTypeIGrid([FromUri] PrimaryType_REQUEST param)
        {
            return IGridFormHelper.GetPrimaryTypeForm(param);
        }
        [Route("api/lov/FAOZoneIGrid")]
        public FAOZone_RESULT GetFAOZoneIGrid([FromUri] FAOZone_REQUEST param)
        {
            return IGridFormHelper.GetFAOZoneForm(param);
        }
        [Route("api/lov/SymbolIGrid")]
        public Symbol_RESULT GetSymbolIGrid([FromUri] Symbol_REQUEST param)
        {
            return IGridFormHelper.GetSymbolForm(param);
        }
        [Route("api/lov/CatchingPeriodIGrid")]
        public CatchingPeriod_RESULT GetCatchingPeriodIGrid([FromUri] CatchingPeriod_REQUEST param)
        {
            return IGridFormHelper.GetCatchingPeriodForm(param);
        }

        [Route("api/lov/PlantIGrid")]
        public Plant_RESULT GetPlant([FromUri] Plant_REQUEST param)
        {
            return IGridFormHelper.GetPlantForm(param);
        }
        //[Route("api/lov/ToGenerate")]
        //public History_RESULT GettoGenerate([FromUri] History_REQUEST param)
        //{
        //    return IGridFormHelper.GetHistoryForm(param);

        //}
        //public IHttpActionResult GettoGenerate()
        //{
        //    System.Data.DataSet ds = new System.Data.DataSet();
        //    ds = CNService.builddataset();
        //    return Json(ds);

        //}
    }
}