using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
	public class SapMaterial_MODEL
	{
		public int Id { get; set; }
		public string DocumentNo { get; set; }
		public string Material { get; set; }
		public string Description { get; set; }
		public string Brand { get; set; }
		public string Brand_TXT { get; set; }
		public string PrimarySize { get; set; }
		public string Version { get; set; }
		public string ChangePoint { get; set; }
		public string MaterialGroup { get; set; }
		public string CreateBy { get; set; }
		public DateTime CreateOn { get; set; }
		public string StatusApp { get; set; }
		public string SheetSize { get; set; }
		public string Assignee { get; set; }
		public string PackingStyle { get; set; }
		public string Packing { get; set; }
		public string StyleofPrinting { get; set; }
		public string ContainerType { get; set; }
		public string LidType { get; set; }
		public string Condition { get; set; }
		public string ModifyBy { get; set; }
		public DateTime ModifyOn { get; set; }
		public string ProductCode { get; set; }
		public string FAOZone { get; set; }
		public string Plant { get; set; }
		public string Totalcolour { get; set; }
		public string Processcolour { get; set; }
		public string PlantRegisteredNo { get; set; }
		public string CompanyNameAddress { get; set; }
		public string PMScolour { get; set; }
		public string Symbol { get; set; }
		public string CatchingArea { get; set; }
		public string CatchingPeriodDate { get; set; }
		public string Grandof { get; set; }
		public string Flute { get; set; }
		public string Vendor { get; set; }
		public string Dimension { get; set; }
		public string RSC { get; set; }
		public string Accessories { get; set; }
		public string PrintingStyleofPrimary { get; set; }
		public string PrintingStyleofSecondary { get; set; }
		public string CustomerDesign { get; set; }
		public string CustomerSpec { get; set; }
		public string CustomerSize { get; set; }
		public string CustomerVendor { get; set; }
		public string CustomerColor { get; set; }
		public string CustomerScanable { get; set; }
		public string CustomersSpecDetail { get; set; }
		public string CustomersSizeDetail { get; set; }
		public string CustomerBarcodeSpec { get; set; }
		public string CustomersDesignDetail { get; set; }
		public string CustomerNominatesVendorDetail { get; set; }
		public string CustomerNominatesColorPantoneDetail { get; set; }
		public string CustomersBarcodeScanableDetail { get; set; }
		public string CustomersBarcodeSpecDetail { get; set; }
		public string FirstInfoGroup { get; set; }
		public string SO { get; set; }
		public string PICMkt { get; set; }
		public string SOPlant { get; set; }
		public string Destination { get; set; }
		public string Remark { get; set; }
		public string GrossWeight { get; set; }
		public string FinalInfoGroup { get; set; }
		public string Note { get; set; }
		public string Typeof { get; set; }
		public string TypeofCarton2 { get; set; }
		public string DMSNo { get; set; }
		public string TypeofPrimary { get; set; }
		public string PrintingSystem { get; set; }
		public string Direction { get; set; }
		public string RollSheet { get; set; }
		public string RequestType { get; set; }
		public string PlantAddress { get; set; }
		public string Refnumber { get; set; }
		public string Extended_Plant { get; set; }
		public string Fixed_Desc { get; set; }
		public string Inactive { get; set; }
		public string Catching_Method { get; set; }
		public string Scientific_Name { get; set; }
		public string Specie { get; set; }
		public string SustainMaterial { get; set; }
		public string SustainPlastic { get; set; }
		public string SustainReuseable { get; set; }
		public string SustainRecyclable { get; set; }
		public string SustainComposatable { get; set; }
		public string SustainCertification { get; set; }
		public string SustainCertSourcing { get; set; }
		public string SustainOther { get; set; }
		public string SusSecondaryPKGWeight { get; set; }
		public string SusRecycledContent { get; set; }
		public string ReferenceMaterial { get; set; }
		public int PrimarySize_id { get; set; }
		public string MaterialGroup_TXT { get; set; }
		public int	Typeof_ID  { get; set; }
		public string Assignee_TXT { get; set; }
		public string fn { get; set; }
		public string ArtowrkURL { get; set; }
        public string VendorName { get; set; }
		public bool IsApprove_step { get; set; }
		public string IsSaveCompleteInfoGroup { get; set; }

    }
	public class SapMaterial_REQUEST : REQUEST_MODEL
	{
		public string user { get; set; }
		public string Type { get; set; }
		public string Keyword { get; set; }
		public SapMaterial_MODEL data { get; set; }
	}
	public class SapMaterial_REQUEST_LIST : REQUEST_MODEL
	{
		public List<SapMaterial_MODEL> data { get; set; }
	}
	public class SapMaterial_RESULT : RESULT_MODEL
	{
		public List<SapMaterial_MODEL> data { get; set; }
	}

}
