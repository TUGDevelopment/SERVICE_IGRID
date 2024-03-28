using BLL.DocumentManagement;
using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace BLL.Helpers
{
    public static class IGridFormHelper
    {
        public static int setOldID(string str_oldid)
        {
            var old_id = 0;

            if (!string.IsNullOrEmpty(str_oldid))
            {
                if (int.TryParse(str_oldid, out old_id))
                {
                    old_id = int.Parse(str_oldid);
                }
            }

            return old_id;
        }



        public static MasterObject_RESULT SaveSaveMasterForm(MasterObject_REQUEST param)
        {
            MasterObject_RESULT Results = new MasterObject_RESULT();
            try
            {


                var packSizeIsAlreaydyInSAP = false;
                var primarySizeIsAlreadyInSAP = false;
                var containerTypeIsAlreadyInSAP = false;
                var descriptionTypeIsAlreadyInSAP = false;



                using (var context = new ARTWORKEntities())
                {


                    if (param.data.Changed_Action == "Insert" || param.data.Changed_Action == "Update")
                    {
                        if (param.data.Changed_Tabname == "MasBrand")
                        {
                            Brand_REQUEST req = new Brand_REQUEST();
                            req.data = new Brand_MODEL();
                            var listbarand = CNService.GetBrand(req);
                            if (param.data.Changed_Action == "Insert")
                            {
                                if (listbarand.Where(w => w.ID == param.data.Id || w.DISPLAY_TXT.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                                {

                                    Results.status = "E";
                                    Results.msg = "Brand (ID or Description) is duplicate.";
                                    return Results;
                                }
                            }
                            else if (param.data.Changed_Action == "Update")

                            {
                                if (listbarand.Where(w => w.ID != param.data.Old_Id && w.DISPLAY_TXT.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                                {

                                    Results.status = "E";
                                    Results.msg = "Brand (ID or Description) is duplicate.";
                                    return Results;

                                }
                            }
                        }

                        else if (param.data.Changed_Tabname == "MasCatchingArea")
                        {

                            CatchingArea_REQUEST req = new CatchingArea_REQUEST();
                            req.data = new CatchingArea_MODEL();
                            var listdata = CNService.GetCatchingArea(req);
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.Description.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                Results.status = "E";
                                Results.msg = "CatchingArea Description is duplicate.";
                                return Results;
                            }
                        }

                        else if (param.data.Changed_Tabname == "MasCatchingMethod")
                        {

                            CatchingMethod_REQUEST req = new CatchingMethod_REQUEST();
                            req.data = new CatchingMethod_MODEL();
                            var listdata = CNService.GetCatchingMethod(req);
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.Description.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                Results.status = "E";
                                Results.msg = "CatchingMethod Description is duplicate.";
                                return Results;
                            }
                        }

                        else if (param.data.Changed_Tabname == "MasCatchingperiodDate")
                        {

                            CatchingPeriod_REQUEST req = new CatchingPeriod_REQUEST();
                            req.data = new CatchingPeriod_MODEL();
                            var listdata = CNService.GetCatchingPeriod(req);
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.DISPLAY_TXT.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                Results.status = "E";
                                Results.msg = "CatchingperiodDate Description is duplicate.";
                                return Results;
                            }
                        }

                        else if (param.data.Changed_Tabname == "MasFAOZone")
                        {

                            FAOZone_REQUEST req = new FAOZone_REQUEST();
                            req.data = new FAOZone_MODEL();
                            var listdata = CNService.GetFAOZone(req);
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.Description.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                Results.status = "E";
                                Results.msg = "FAOZone Description is duplicate.";
                                return Results;
                            }
                        }

                        else if (param.data.Changed_Tabname == "MasFlute")
                        {

                            Flute_REQUEST req = new Flute_REQUEST();
                            req.data = new Flute_MODEL();
                            var listdata = CNService.GetFlute(req);
                            var IsDup = false;
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.DISPLAY_TXT.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                IsDup = true;
                                if (param.data.Changed_Action == "Update")
                                {
                                    if (listdata.Where(w => (w.DISPLAY_TXT + w.MaterialGroup).Trim().ToLower() == (param.data.Description + param.data.Material_Group).Trim().ToLower()).Count() > 0)
                                        IsDup = true;
                                    else
                                        IsDup = false;
                                }
                            }

                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "Flute Description is duplicate.";
                                return Results;
                            }

                        }

                        else if (param.data.Changed_Tabname == "MasGradeofCarton")
                        {

                            Gradeof_REQUEST req = new Gradeof_REQUEST();
                            req.data = new Gradeof_MODEL();
                            var listdata = CNService.GetGradeof(req);
                            var IsDup = false;
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.DISPLAY_TXT.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                IsDup = true;
                                if (param.data.Changed_Action == "Update")
                                {
                                    if (listdata.Where(w => w.MaterialGroup.Trim().ToLower() == param.data.Material_Group.Trim().ToLower()).Count() > 0)
                                        IsDup = true;
                                    else
                                        IsDup = false;
                                }
                            }

                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "GradeofCarton Description is duplicate.";
                                return Results;
                            }

                        }

                        else if (param.data.Changed_Tabname == "MasPackingStyle")
                        {

                            if (param.data.Changed_Action == "Update")
                            {
                                SAP_M_2P m2p = new SAP_M_2P();
                                var table = CNService.builditems("select * from MasPackingStyle where id=" + param.data.Old_Id);
                                foreach (DataRow rw in table.Rows)
                                {
                                    m2p.PACKING_SYLE_VALUE = string.Format("{0}", rw["RefStyle"]);
                                    m2p.PACKING_SYLE_DESCRIPTION = string.Format("{0}", rw["RefStyle"]);
                                    m2p.PACK_SIZE_VALUE = string.Format("{0}", rw["Packsize"]);
                                    m2p.PACK_SIZE_DESCRIPTION = string.Format("{0}", rw["Packsize"]);
                                    var existItem2 = SAP_M_2P_SERVICE.GetByItem(m2p, context).FirstOrDefault();
                                    if (existItem2 != null)
                                    {
                                        m2p.TWO_P_ID = existItem2.TWO_P_ID;
                                        m2p.PACKING_SYLE_DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", rw["RefStyle"]);
                                        m2p.IS_ACTIVE = "X";
                                        m2p.CREATE_BY = -2;
                                        m2p.CREATE_DATE = DateTime.Today;
                                        m2p.UPDATE_BY = -2;
                                        m2p.UPDATE_DATE = DateTime.Today;
                                        SAP_M_2P_SERVICE.SaveOrUpdateNoLog(m2p, context);
                                    }
                                }
                            }
                            PackStyle_REQUEST req = new PackStyle_REQUEST();
                            req.data = new PackStyle_MODEL();
                            var listdata = CNService.GetPackStyle(req);
                            var IsDup = false;
                            //var old_id = getOldID(param.data.Old_Id);
                            var strTemp = (param.data.PackingStyle + param.data.RefStyle + param.data.Packsize + param.data.TypeofPrimary).Trim().ToLower();
                            if (listdata.Where(w => (w.PackingStyle + w.RefStyle + w.PackSize + w.TypeofPrimary).Trim().ToLower() == strTemp).Count() > 0)
                            {
                                IsDup = true;
                            }
                            else
                            {
                                if (listdata.Where(w => w.PackSize == param.data.Packsize).Count() > 0)
                                {
                                    packSizeIsAlreaydyInSAP = true;
                                }

                            }
                            int userID = -2;
                            SAP_M_2P sapm2p = new SAP_M_2P();
                            sapm2p.PACKING_SYLE_VALUE = string.Format("{0}", param.data.RefStyle);
                            sapm2p.PACKING_SYLE_DESCRIPTION = string.Format("{0}", param.data.RefStyle);
                            sapm2p.PACK_SIZE_VALUE = string.Format("{0}", param.data.Packsize);
                            sapm2p.PACK_SIZE_DESCRIPTION = string.Format("{0}", param.data.Packsize);
                            var existItem = SAP_M_2P_SERVICE.GetByItem(sapm2p, context).FirstOrDefault();
                            if (existItem == null)
                            {
                                sapm2p.IS_ACTIVE = "";
                                sapm2p.CREATE_BY = userID;
                                sapm2p.CREATE_DATE = DateTime.Today;
                                sapm2p.UPDATE_BY = userID;
                                sapm2p.UPDATE_DATE = DateTime.Today;
                                SAP_M_2P_SERVICE.SaveOrUpdateNoLog(sapm2p, context);
                            }

                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "PackingStyle is duplicate.";
                                return Results;
                            }
                        }


                        else if (param.data.Changed_Tabname == "MasPlantRegisteredNo")
                        {

                            PlantRegistered_REQUEST req = new PlantRegistered_REQUEST();
                            req.data = new PlantRegistered_MODEL();
                            var listdata = CNService.GetPlantRegistered(req);
                            var IsDup = false;
                            var strTemp = "";


                            switch (param.data.Changed_Action)
                            {
                                case "Update":
                                    strTemp = (param.data.RegisteredNo + param.data.Address + param.data.Plant).Trim().ToLower();
                                    if (listdata.Where(w => (w.RegisteredNo + w.Address + w.Plant).Trim().ToLower() == strTemp).Count() > 0)
                                    {
                                        IsDup = true;
                                    }
                                    break;
                                case "Insert":
                                    strTemp = (param.data.RegisteredNo + param.data.Address).Trim().ToLower();
                                    if (listdata.Where(w => (w.RegisteredNo + w.Address).Trim().ToLower() == strTemp).Count() > 0)
                                    {
                                        IsDup = true;
                                    }
                                    break;
                            }


                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "PlantRegisteredNo is duplicate.";
                                return Results;
                            }

                            
                        }

                        else if (param.data.Changed_Tabname == "MasPMSColour")
                        {

                            PMSColour_REQUEST req = new PMSColour_REQUEST();
                            req.data = new PMSColour_MODEL();
                            var listdata = CNService.GetPMSColour(req);
                            var IsDup = false;
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.DISPLAY_TXT.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                IsDup = true;
                                if (param.data.Changed_Action == "Update")
                                {
                                    if (listdata.Where(w => (w.DISPLAY_TXT + w.MaterialGroup).Trim().ToLower() == (param.data.Description + param.data.Material_Group).Trim().ToLower()).Count() > 0)
                                        IsDup = true;
                                    else
                                        IsDup = false;
                                }
                            }

                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "PMSColour Description is duplicate.";
                                return Results;
                            }

                        }

                        else if (param.data.Changed_Tabname == "MasTypeofPrimary")
                        {

                            PrimaryType_REQUEST req = new PrimaryType_REQUEST();
                            req.data = new PrimaryType_MODEL();
                            var listdata = CNService.GetPrimaryType(req);
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.Description.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                Results.status = "E";
                                Results.msg = "PrimaryType Description is duplicate.";
                                return Results;
                            }
                        }

                        else if (param.data.Changed_Tabname == "MasPrimarySize")
                        {

                            PrimarySize_REQUEST req = new PrimarySize_REQUEST();
                            req.data = new PrimarySize_MODEL();
                            var listdata = CNService.GetPrimarySize(req,"");
                            var IsDup = false;
                            //var old_id = getOldID(param.data.Old_Id);
                            var strTemp = (param.data.Description + param.data.Can + param.data.LidType + param.data.ContainerType + param.data.DescriptionType).Trim().ToLower();
                            if (listdata.Where(w => (w.Description + w.Can + w.LidType + w.ContainerType + w.DescriptionType).Trim().ToLower() == strTemp).Count() > 0)
                            {
                                IsDup = true;
                            }
                            else
                            {
                                if (listdata.Where(w => w.Description.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                                {
                                    primarySizeIsAlreadyInSAP = true;
                                }
                                if (listdata.Where(w => w.ContainerType.Trim().ToLower() == param.data.ContainerType.Trim().ToLower()).Count() > 0)
                                {
                                    containerTypeIsAlreadyInSAP = true;
                                }
                                if (listdata.Where(w => w.DescriptionType.Trim().ToLower() == param.data.DescriptionType.Trim().ToLower()).Count() > 0)
                                {
                                    descriptionTypeIsAlreadyInSAP = true;
                                }
                            }

                            int userID = -2;
                            if (param.data.Changed_Action == "Update") { 
                                var table = CNService.builditems("select * from MasPrimarySize where id=" + param.data.Old_Id);
                                foreach (DataRow rw in table.Rows)
                                {
                                    string sql = string.Format(@"select convert(nvarchar(max),count(*))c from MasPrimarySize where isnull(Inactive,'')='' and Description='{0}' and ContainerType='{1}' and DescriptionType='{2}'"
                                        , rw["Description"], rw["ContainerType"], rw["DescriptionType"]);
                                    if (Convert.ToInt32(CNService.ReadItems(sql)) == 1)
                                    {
                                        SAP_M_3P m3p = new SAP_M_3P();
                                        m3p.PRIMARY_SIZE_VALUE = string.Format("{0}", rw["Description"]);
                                        m3p.PRIMARY_SIZE_DESCRIPTION = string.Format("{0}", rw["Description"]);
                                        m3p.CONTAINER_TYPE_VALUE = string.Format("{0}", rw["ContainerType"]);
                                        m3p.CONTAINER_TYPE_DESCRIPTION = string.Format("{0}", rw["ContainerType"]);
                                        m3p.LID_TYPE_VALUE = string.Format("{0}", rw["DescriptionType"]);
                                        m3p.LID_TYPE_DESCRIPTION = string.Format("{0}", rw["DescriptionType"]);
                                        var existItem3 = SAP_M_3P_SERVICE.GetByItem(m3p, context).FirstOrDefault();
                                        if (existItem3 != null)
                                        {
                                            m3p.THREE_P_ID = existItem3.THREE_P_ID;
                                            m3p.PRIMARY_SIZE_DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", rw["Description"]);
                                            m3p.CONTAINER_TYPE_DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", rw["ContainerType"]);
                                            m3p.LID_TYPE_DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", rw["DescriptionType"]);
                                            m3p.IS_ACTIVE = "";
                                            m3p.CREATE_BY = userID;
                                            m3p.CREATE_DATE = DateTime.Today;
                                            m3p.UPDATE_BY = userID;
                                            m3p.UPDATE_DATE = DateTime.Today;
                                            SAP_M_3P_SERVICE.SaveOrUpdateNoLog(m3p, context);
                                        }
                                    }
                                    // nueng insert artwork
                                }
                            }        
                            SAP_M_3P sapm3p = new SAP_M_3P();
                            sapm3p.PRIMARY_SIZE_VALUE = string.Format("{0}", param.data.Description);
                            sapm3p.PRIMARY_SIZE_DESCRIPTION = string.Format("{0}", param.data.Description);
                            sapm3p.CONTAINER_TYPE_VALUE = string.Format("{0}", param.data.ContainerType);
                            sapm3p.CONTAINER_TYPE_DESCRIPTION = string.Format("{0}", param.data.ContainerType);
                            sapm3p.LID_TYPE_VALUE = string.Format("{0}", param.data.DescriptionType);
                            sapm3p.LID_TYPE_DESCRIPTION = string.Format("{0}", param.data.DescriptionType);
                            var existItem = SAP_M_3P_SERVICE.GetByItem(sapm3p, context).FirstOrDefault();
                            if (existItem == null)
                            {
                                sapm3p.IS_ACTIVE = "X";
                                sapm3p.CREATE_BY = userID;
                                sapm3p.CREATE_DATE = DateTime.Today;
                                sapm3p.UPDATE_BY = userID;
                                sapm3p.UPDATE_DATE = DateTime.Today;
                                SAP_M_3P_SERVICE.SaveOrUpdateNoLog(sapm3p, context);
                            }
                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "PrimarySize is duplicate.";
                                return Results;
                            }

                        }

                        else if (param.data.Changed_Tabname == "MasProcessColour")
                        {

                            ProcessColour_REQUEST req = new ProcessColour_REQUEST();
                            req.data = new ProcessColour_MODEL();
                            var listdata = CNService.GetProcessColour(req);
                            var IsDup = false;
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.DISPLAY_TXT.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                IsDup = true;
                                if (param.data.Changed_Action == "Update")
                                {
                                    if (listdata.Where(w => (w.DISPLAY_TXT + w.MaterialGroup).Trim().ToLower() == (param.data.Description + param.data.Material_Group).Trim().ToLower()).Count() > 0)
                                        IsDup = true;
                                    else
                                        IsDup = false;
                                }
                            }

                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "ProcessColour Description is duplicate.";
                                return Results;
                            }

                        }

                        else if (param.data.Changed_Tabname == "MasProductGroup")
                        {

                            ProductGroup_REQUEST req = new ProductGroup_REQUEST();
                            req.data = new ProductGroup_MODEL();
                            var listdata = CNService.GetProductGroup(req);
                            var IsDup = false;
                            //var old_id = getOldID(param.data.Old_Id);
                            var strTemp = (param.data.Product_Group + param.data.Product_GroupDesc + param.data.PRD_Plant).Trim().ToLower();
                            if (listdata.Where(w => (w.Product_Group + w.Product_GroupDesc + w.PRD_Plant).Trim().ToLower() == strTemp).Count() > 0)
                            {
                                IsDup = true;
                            }

                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "ProductGroup Description is duplicate.";
                                return Results;
                            }

                        }

                        else if (param.data.Changed_Tabname == "MasScientificName")
                        {

                            ScientificName_REQUEST req = new ScientificName_REQUEST();
                            req.data = new ScientificName_MODEL();
                            var listdata = CNService.GetScientificName(req);
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.DISPLAY_TXT.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                Results.status = "E";
                                Results.msg = "ScientificName Description is duplicate.";
                                return Results;
                            }
                        }

                        else if (param.data.Changed_Tabname == "MasSpecie")
                        {

                            Specie_REQUEST req = new Specie_REQUEST();
                            req.data = new Specie_MODEL();
                            var listdata = CNService.GetSpecie(req);
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.DISPLAY_TXT.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                Results.status = "E";
                                Results.msg = "Specie Description is duplicate.";
                                return Results;
                            }
                        }

                        else if (param.data.Changed_Tabname == "MasStyleofPrinting")
                        {

                            StyleofPrinting_REQUEST req = new StyleofPrinting_REQUEST();
                            req.data = new StyleofPrinting_MODEL();
                            var listdata = CNService.GetStyleofPrinting(req);
                            var IsDup = false;
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.DISPLAY_TXT.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                IsDup = true;
                                if (param.data.Changed_Action == "Update")
                                {
                                    if (listdata.Where(w => (w.DISPLAY_TXT + w.MaterialGroup).Trim().ToLower() == (param.data.Description + param.data.Material_Group).Trim().ToLower()).Count() > 0)
                                        IsDup = true;
                                    else
                                        IsDup = false;
                                }
                            }

                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "StyleofPrinting Description is duplicate.";
                                return Results;
                            }

                        }

                        else if (param.data.Changed_Tabname == "MasSymbol")
                        {

                            Symbol_REQUEST req = new Symbol_REQUEST();
                            req.data = new Symbol_MODEL();
                            var listdata = CNService.GetSymbol(req);
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.Description.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                Results.status = "E";
                                Results.msg = "Symbol Description is duplicate.";
                                return Results;
                            }
                        }

                        else if (param.data.Changed_Tabname == "MasTotalColour")
                        {

                            TotalColour_REQUEST req = new TotalColour_REQUEST();
                            req.data = new TotalColour_MODEL();
                            var listdata = CNService.GetTotalColour(req);
                            var IsDup = false;
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.DISPLAY_TXT.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                IsDup = true;
                                if (param.data.Changed_Action == "Update")
                                {
                                    if (listdata.Where(w => (w.DISPLAY_TXT + w.MaterialGroup).Trim().ToLower() == (param.data.Description + param.data.Material_Group).Trim().ToLower()).Count() > 0)
                                        IsDup = true;
                                    else
                                        IsDup = false;
                                }
                            }

                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "TotalColour Description is duplicate.";
                                return Results;
                            }

                        }

                        else if (param.data.Changed_Tabname == "MasTypeofCarton")
                        {

                            TypeOf_REQUEST req = new TypeOf_REQUEST();
                            req.data = new TypeOf_MODEL();
                            var listdata = CNService.GetTypeOf(req);
                            var IsDup = false;
                            //var old_id = getOldID(param.data.Old_Id);

                            if (listdata.Where(w => w.DISPLAY_TXT.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                IsDup = true;
                            }
                            else
                            {
                                if (param.data.Changed_Action == "Update")
                                {
                                    var strTemp = (param.data.Description + param.data.Material_Group + param.data.Material_Type + param.data.DescriptionText).Trim().ToLower();
                                    if (listdata.Where(w => (w.DescriptionText + w.MaterialGroup + w.MaterialType + w.DescriptionText).Trim().ToLower() == param.data.ContainerType.Trim().ToLower()).Count() > 0)
                                    {
                                        IsDup = true;
                                    }
                                }
                            }

                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "type of (1) is duplicate.";
                                return Results;
                            }

                        }

                        else if (param.data.Changed_Tabname == "MasVendor")
                        {

                            Vendor_REQUEST req = new Vendor_REQUEST();
                            req.data = new Vendor_MODEL();
                            var listdata = CNService.GetVendor(req);
                            var IsDup = false;
                            var msg = "";
                            //var old_id = getOldID(param.data.Old_Id);

                            if (param.data.Changed_Action == "Insert")
                            {
                                if (listdata.Where(w => w.Code.Trim().ToLower() == param.data.Id.Trim().ToLower() || w.Name.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                                {
                                    IsDup = true;
                                    msg = "Vendor Code or Name is duplicate.";
                                }

                            }
                            else
                            {
                                int old_id = setOldID(param.data.Old_Id);
                                if (listdata.Where(w => w.Code.Trim().ToLower() == param.data.Id.Trim().ToLower() && w.Id != old_id).Count() > 0)
                                {
                                    IsDup = true;
                                    msg = "Not allowed to change Vendor Code.";
                                }
                                else if (listdata.Where(w => w.Name.Trim().ToLower() == param.data.Description.Trim().ToLower() && w.Id != old_id).Count() > 0)
                                {
                                    IsDup = true;
                                    msg = "Vendor Name is duplicate.";
                                }
                            }

                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = msg;
                                return Results;
                            }

                        }

                        else if (param.data.Changed_Tabname == "ulogin")
                        {

                            ulogin_REQUEST req = new ulogin_REQUEST();
                            req.data = new ulogin_MODEL();
                            var listdata = CNService.Getulogin(req);
                            var IsDup = false;
                            var old_id = setOldID(param.data.Old_Id);


                            if (listdata.Where(w => w.user_name.Trim().ToLower() == param.data.user_name.Trim().ToLower() && w.Id != old_id).Count() > 0)
                            {
                                IsDup = true;
                            }


                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "uLogin is duplicate.";
                                return Results;
                            }

                        }

                        else if (param.data.Changed_Tabname == "MasLogistics")
                        {

                            WHManagement_REQUEST req = new WHManagement_REQUEST();
                            req.data = new WHManagement_MODEL();
                            var listdata = CNService.GetWHManagement(req);
                            var IsDup = false;
                            var old_id = setOldID(param.data.Old_Id);
                            var strtemp = param.data.Product_Group + param.data.PRD_Plant;
                            if (listdata.Where(w => (w.ProductGroup + w.Plant).Trim().ToLower() == strtemp.Trim().ToLower() && w.Id != old_id).Count() > 0)
                            {
                                IsDup = true;
                            }


                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "Logistics is duplicate.";
                                return Results;
                            }

                        }

                        else if (param.data.Changed_Tabname == "MasSustainCertSourcing")
                        {

                            SustainCertSourcing_REQUEST req = new SustainCertSourcing_REQUEST();
                            req.data = new SustainCertSourcing_MODEL();
                            var listdata = CNService.GetSustainCertSourcing(req);
                            var IsDup = false;
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.DISPLAY_TXT.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                IsDup = true;
                                if (param.data.Changed_Action == "Update")
                                {
                                    if (listdata.Where(w => (w.DISPLAY_TXT + w.MaterialGroup).Trim().ToLower() == (param.data.Description + param.data.Material_Group).Trim().ToLower()).Count() > 0)
                                        IsDup = true;
                                    else
                                        IsDup = false;
                                }
                            }

                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "SustainCertSourcing Description is duplicate.";
                                return Results;
                            }

                        }

                        else if (param.data.Changed_Tabname == "MasSustainMaterial")
                        {

                            SustainMaterial_REQUEST req = new SustainMaterial_REQUEST();
                            req.data = new SustainMaterial_MODEL();
                            var listdata = CNService.GetSustainMaterial(req);
                            var IsDup = false;
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.DISPLAY_TXT.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                IsDup = true;
                                if (param.data.Changed_Action == "Update")
                                {
                                    if (listdata.Where(w => (w.DISPLAY_TXT + w.MaterialGroup).Trim().ToLower() == (param.data.Description + param.data.Material_Group).Trim().ToLower()).Count() > 0)
                                        IsDup = true;
                                    else
                                        IsDup = false;
                                }
                            }

                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "SustainMaterial Description is duplicate.";
                                return Results;
                            }

                        }

                        else if (param.data.Changed_Tabname == "MasSustainPlastic")
                        {

                            SustainPlastic_REQUEST req = new SustainPlastic_REQUEST();
                            req.data = new SustainPlastic_MODEL();
                            var listdata = CNService.GetSustainPlastic(req);
                            var IsDup = false;
                            //var old_id = getOldID(param.data.Old_Id);
                            if (listdata.Where(w => w.DISPLAY_TXT.Trim().ToLower() == param.data.Description.Trim().ToLower()).Count() > 0)
                            {
                                IsDup = true;
                                if (param.data.Changed_Action == "Update")
                                {
                                    if (listdata.Where(w => (w.DISPLAY_TXT + w.MaterialGroup).Trim().ToLower() == (param.data.Description + param.data.Material_Group).Trim().ToLower()).Count() > 0)
                                        IsDup = true;
                                    else
                                        IsDup = false;
                                }
                            }

                            if (IsDup)
                            {
                                Results.status = "E";
                                Results.msg = "SustainPlastic Description is duplicate.";
                                return Results;
                            }

                        }

                    }
                    else if (param.data.Changed_Action.Equals("Re-Active"))
                    {
                        int userID = -2;
                        if (string.Format("{0}", param.data.Changed_Tabname).Equals("MasPackingStyle"))
                        {

                            SAP_M_2P sapm2p = new SAP_M_2P();
                            sapm2p.PACKING_SYLE_VALUE = string.Format("{0}", param.data.RefStyle);
                            sapm2p.PACKING_SYLE_DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", param.data.RefStyle);
                            sapm2p.PACK_SIZE_VALUE = string.Format("{0}", param.data.Packsize);
                            sapm2p.PACK_SIZE_DESCRIPTION = string.Format("{0}", param.data.Packsize);
                            var existItem2 = SAP_M_2P_SERVICE.GetByItem(sapm2p, context).FirstOrDefault();
                            if (existItem2 != null)
                            {
                                sapm2p.TWO_P_ID = existItem2.TWO_P_ID;
                                sapm2p.PACKING_SYLE_DESCRIPTION = string.Format("{0}", param.data.RefStyle);
                                sapm2p.IS_ACTIVE = "X";
                                sapm2p.CREATE_BY = userID;
                                sapm2p.CREATE_DATE = DateTime.Today;
                                sapm2p.UPDATE_BY = userID;
                                sapm2p.UPDATE_DATE = DateTime.Today;
                                SAP_M_2P_SERVICE.SaveOrUpdateNoLog(sapm2p, context);
                            }
                        }
                        if (string.Format("{0}", param.data.Changed_Tabname).Equals("MasPrimarySize"))
                        {
                            var table = CNService.builditems("select * from MasPrimarySize where id=" + param.data.Old_Id);
                            foreach (DataRow rw in table.Rows)
                            {
                                string sql = string.Format(@"select convert(nvarchar(max),count(*))c from MasPrimarySize where isnull(Inactive,'')='' and Description='{0}' and ContainerType='{1}' and DescriptionType='{2}'"
                                    , rw["Description"], rw["ContainerType"], rw["DescriptionType"]);
                                if (Convert.ToInt32(CNService.ReadItems(sql)) == 0)
                                {
                                    SAP_M_3P m3p = new SAP_M_3P();
                                    m3p.PRIMARY_SIZE_VALUE = string.Format("{0}", rw["Description"]);
                                    m3p.PRIMARY_SIZE_DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", rw["Description"]);
                                    m3p.CONTAINER_TYPE_VALUE = string.Format("{0}", rw["ContainerType"]);
                                    m3p.CONTAINER_TYPE_DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", rw["ContainerType"]);
                                    m3p.LID_TYPE_VALUE = string.Format("{0}", rw["DescriptionType"]);
                                    m3p.LID_TYPE_DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", rw["DescriptionType"]);
                                    var existItem3 = SAP_M_3P_SERVICE.GetByItem(m3p, context).FirstOrDefault();
                                    if (existItem3 != null)
                                    {
                                        m3p.THREE_P_ID = existItem3.THREE_P_ID;
                                        m3p.PRIMARY_SIZE_DESCRIPTION = string.Format("{0}", rw["Description"]);
                                        m3p.CONTAINER_TYPE_DESCRIPTION = string.Format("{0}", rw["ContainerType"]);
                                        m3p.LID_TYPE_DESCRIPTION = string.Format("{0}", rw["DescriptionType"]);
                                        m3p.IS_ACTIVE = "X";
                                        m3p.CREATE_BY = userID;
                                        m3p.CREATE_DATE = DateTime.Today;
                                        m3p.UPDATE_BY = userID;
                                        m3p.UPDATE_DATE = DateTime.Today;
                                        SAP_M_3P_SERVICE.SaveOrUpdateNoLog(m3p, context);
                                    }
                                }
                            }
                        }
                    }
                    else if (param.data.Changed_Action.Equals("Inactive"))
                    {
                        int userID = -2;
                        if (string.Format("{0}", param.data.Changed_Tabname).Equals("MasPackingStyle"))
                        {

                            SAP_M_2P sapm2p = new SAP_M_2P();
                            sapm2p.PACKING_SYLE_VALUE = string.Format("{0}", param.data.RefStyle);
                            sapm2p.PACKING_SYLE_DESCRIPTION = string.Format("{0}", param.data.RefStyle);
                            sapm2p.PACK_SIZE_VALUE = string.Format("{0}", param.data.Packsize);
                            sapm2p.PACK_SIZE_DESCRIPTION = string.Format("{0}", param.data.Packsize);
                            var existItem2 = SAP_M_2P_SERVICE.GetByItem(sapm2p, context).FirstOrDefault();
                            if (existItem2 != null)
                            {
                                sapm2p.TWO_P_ID = existItem2.TWO_P_ID;
                                sapm2p.PACKING_SYLE_DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", param.data.RefStyle);
                                sapm2p.IS_ACTIVE = "X";
                                sapm2p.CREATE_BY = userID;
                                sapm2p.CREATE_DATE = DateTime.Today;
                                sapm2p.UPDATE_BY = userID;
                                sapm2p.UPDATE_DATE = DateTime.Today;
                                SAP_M_2P_SERVICE.SaveOrUpdateNoLog(sapm2p, context);
                            }
                        }
                        if (string.Format("{0}", param.data.Changed_Tabname).Equals("MasPrimarySize"))
                        {
                            var table = CNService.builditems("select * from MasPrimarySize where id=" + param.data.Old_Id);
                            foreach (DataRow rw in table.Rows)
                            {
                                string sql = string.Format(@"select convert(nvarchar(max),count(*))c from MasPrimarySize where isnull(Inactive,'')='' and Description='{0}' and ContainerType='{1}' and DescriptionType='{2}'"
                                    , rw["Description"], rw["ContainerType"], rw["DescriptionType"]);
                                if (Convert.ToInt32(CNService.ReadItems(sql)) == 1)
                                {
                                    SAP_M_3P m3p = new SAP_M_3P();
                                    m3p.PRIMARY_SIZE_VALUE = string.Format("{0}", rw["Description"]);
                                    m3p.PRIMARY_SIZE_DESCRIPTION = string.Format("{0}", rw["Description"]);
                                    m3p.CONTAINER_TYPE_VALUE = string.Format("{0}", rw["ContainerType"]);
                                    m3p.CONTAINER_TYPE_DESCRIPTION = string.Format("{0}", rw["ContainerType"]);
                                    m3p.LID_TYPE_VALUE = string.Format("{0}", rw["DescriptionType"]);
                                    m3p.LID_TYPE_DESCRIPTION = string.Format("{0}", rw["DescriptionType"]);
                                    var existItem3 = SAP_M_3P_SERVICE.GetByItem(m3p, context).FirstOrDefault();
                                    if (existItem3 != null)
                                    {
                                        m3p.THREE_P_ID = existItem3.THREE_P_ID;
                                        m3p.PRIMARY_SIZE_DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", rw["Description"]);
                                        m3p.CONTAINER_TYPE_DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", rw["ContainerType"]);
                                        m3p.LID_TYPE_DESCRIPTION = string.Format("{0}-XXX Do Not Use XXX", rw["DescriptionType"]);
                                        m3p.IS_ACTIVE = "";
                                        m3p.CREATE_BY = userID;
                                        m3p.CREATE_DATE = DateTime.Today;
                                        m3p.UPDATE_BY = userID;
                                        m3p.UPDATE_DATE = DateTime.Today;
                                        SAP_M_3P_SERVICE.SaveOrUpdateNoLog(m3p, context);
                                    }
                                }
                            }
                        }
                    }
                }

                Results.data = CNService.savemaster2(param);
                //Results.status = "S";
                using (var context = new ARTWORKEntities())
                {
                    Results.status = "S";
                    if (packSizeIsAlreaydyInSAP)
                    {
                        Results.msg = "Completed. {This pack size already exist in SAP.}";
                    } else if (primarySizeIsAlreadyInSAP || containerTypeIsAlreadyInSAP || descriptionTypeIsAlreadyInSAP)
                    {
                        Results.msg = "Completed. {This Primary Size/Container Type/Lid Type already exist in SAP.}";
                    }
                    else
                    {
                        Results.msg = MessageHelper.GetMessage("MSG_001", context);
                    } 
                 
                }
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static PrimarySize_RESULT GetPrimaryForm(PrimarySize_REQUEST param)
        {
            PrimarySize_RESULT Results = new PrimarySize_RESULT();
            try
            {
                Results.data = CNService.GetPrimarySize(param,"");
                               
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static PrimarySize_RESULT GetPrimaryForm2(PrimarySize_REQUEST param)
        {
            PrimarySize_RESULT Results = new PrimarySize_RESULT();
            try
            {
                var whereQuery = "";
                if (param != null && param.data != null)
                {


                    var wCode = CNService.getSQLWhereLikeByConvertString(param.data.Code, "Code", true, true, true);
                    var wCan = CNService.getSQLWhereLikeByConvertString(param.data.Can, "Can", true, true, true);
                    var wCanDesciption = CNService.getSQLWhereLikeByConvertString(param.data.Description, "Description", false, true, true);
                    var wLidType = CNService.getSQLWhereLikeByConvertString(param.data.LidType, "LidType", true, true, true);
                    var wContainerType = CNService.getSQLWhereLikeByConvertString(param.data.ContainerType, "ContainerType", false, true, true);
                    var wDescriptionType = CNService.getSQLWhereLikeByConvertString(param.data.DescriptionType, "DescriptionType", false, true, true);

                    whereQuery = CNService.getSQLWhereByJoinStringWithAnd(whereQuery, wCode);
                    whereQuery = CNService.getSQLWhereByJoinStringWithAnd(whereQuery, wCan);
                    whereQuery = CNService.getSQLWhereByJoinStringWithAnd(whereQuery, wCanDesciption);
                    whereQuery = CNService.getSQLWhereByJoinStringWithAnd(whereQuery, wLidType);
                    whereQuery = CNService.getSQLWhereByJoinStringWithAnd(whereQuery, wContainerType);
                    whereQuery = CNService.getSQLWhereByJoinStringWithAnd(whereQuery, wDescriptionType);
                    whereQuery = CNService.getSQLWhereByJoinStringWithAnd(whereQuery, "isnull(inactive,'') <> 'X'");
                }
                Results.data = CNService.GetPrimarySize(param, whereQuery.Trim());
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static Brand_RESULT GetBrand(Brand_REQUEST param)
        {
            Brand_RESULT Results = new Brand_RESULT();
            try
            {
                Results.data = CNService.GetBrand(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static Brand_RESULT GetBrand2(Brand_REQUEST param)
        {
            Brand_RESULT Results = new Brand_RESULT();
            try
            {
                Results.data = CNService.GetBrand2(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static MaterialClass_RESULT GetMaterialClass(MaterialClass_REQUEST param)
        {
            MaterialClass_RESULT Results = new MaterialClass_RESULT();
            try
            {
                Results.data = CNService.GetMaterialClass(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static TypeofPrimary_RESULT GetTypeofPrimary(TypeofPrimary_REQUEST param)
        {
            TypeofPrimary_RESULT Results = new TypeofPrimary_RESULT();
            try
            {
                Results.data = CNService.GetTypeofPrimary(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static SustainPlastic_RESULT GetSustainPlasticForm(SustainPlastic_REQUEST param)
        {
            SustainPlastic_RESULT Results = new SustainPlastic_RESULT();
            try
            {
                Results.data = CNService.GetSustainPlastic(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static SustainMaterial_RESULT GetSustainMaterialForm(SustainMaterial_REQUEST param)
        {
            SustainMaterial_RESULT Results = new SustainMaterial_RESULT();
            try
            {
                Results.data = CNService.GetSustainMaterial(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static Specie_RESULT GetSpecieForm(Specie_REQUEST param)
        {
            Specie_RESULT Results = new Specie_RESULT();
            try
            {
                Results.data = CNService.GetSpecie(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static ScientificName_RESULT GetScientificNameForm(ScientificName_REQUEST param)
        {
            ScientificName_RESULT Results = new ScientificName_RESULT();
            try
            {
                Results.data = CNService.GetScientificName(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static PrintingSystem_RESULT GetPrintingSystemForm(PrintingSystem_REQUEST param)
        {
            PrintingSystem_RESULT Results = new PrintingSystem_RESULT();
            try
            {
                Results.data = CNService.GetPrintingSystem(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static Direction_RESULT GetDirectionForm(Direction_REQUEST param)
        {
            Direction_RESULT Results = new Direction_RESULT();
            try
            {
                Results.data = CNService.GetDirection(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static CatchingMethod_RESULT GetCatchingMethodForm(CatchingMethod_REQUEST param)
        {
            CatchingMethod_RESULT Results = new CatchingMethod_RESULT();
            try
            {
                Results.data = CNService.GetCatchingMethod(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static WHManagement_RESULT GetWHManagementForm(WHManagement_REQUEST param)
        {
            WHManagement_RESULT Results = new WHManagement_RESULT();
            try
            {
                Results.data = CNService.GetWHManagement(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        
       public static Assign_RESULT GetAssignForm(Assign_REQUEST param)
        {
            Assign_RESULT Results = new Assign_RESULT();
            try
            {
                Results.data = CNService.GetAssign(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static ulogin_RESULT GetuloginForm(ulogin_REQUEST param)
        {
            ulogin_RESULT Results = new ulogin_RESULT();
            try
            {
                Results.data = CNService.Getulogin(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static Vendor_RESULT GetVendorForm(Vendor_REQUEST param)
        {
            Vendor_RESULT Results = new Vendor_RESULT();
            try
            {
                Results.data = CNService.GetVendor(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static PlantRegistered_RESULT GetCompanyAddressForm2(PlantRegistered_REQUEST param)
        {
            PlantRegistered_RESULT Results = new PlantRegistered_RESULT();
            try
            {
                Results.data = CNService.GetSecPlantName(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static PlantRegistered_RESULT GetCompanyNameForm(PlantRegistered_REQUEST param)
        {
            PlantRegistered_RESULT Results = new PlantRegistered_RESULT();
            try
            {
                Results.data = CNService.GetCompanyName(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static PlantRegistered_RESULT GetPlantRegisteredForm(PlantRegistered_REQUEST param)
        {
            // for master
            PlantRegistered_RESULT Results = new PlantRegistered_RESULT();
            try
            {
                Results.data = CNService.GetPlantRegistered(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static PlantRegistered_RESULT GetPlantRegisteredFormLOV(PlantRegistered_REQUEST param)
        {
            // for taskform
            PlantRegistered_RESULT Results = new PlantRegistered_RESULT();
            try
            {
                Results.data = new List<PlantRegistered_MODEL>();

                if (param != null && param.data != null && !string.IsNullOrEmpty(param.data.STR_PRODUCT_CODE))
                {

                    var listProductCode = param.data.STR_PRODUCT_CODE.Split(';');
                    var product_group = "";
                    var firstDigit = "";
                    if (listProductCode.Length > 0 )
                    {

                        List<ProductGroup_MODEL> listProductGroup = CNService.GetProductGroup(new ProductGroup_REQUEST());
                         foreach (var product_code in listProductCode)
                        {
                            if (product_code.Length == 18)
                            {
                                firstDigit = product_code.Substring(0, 1);
                                if (firstDigit == "2" || firstDigit == "3")
                                {
                                    product_group = product_code.Substring(1, 1);
                                    var productGroup = listProductGroup.Where(w => w.Product_Group == product_group).FirstOrDefault();
                                    if (productGroup != null)
                                    {
                                        param.data.Plant = productGroup.PRD_Plant;
                                        var manualQuery = ""; // @"select distinct 0 as ID,RegisteredNo,'' as Address, '' as Plant, '' as Inactive from MasPlantRegisteredNo";
                                        var list = CNService.GetPlantRegistered(param);
                                        if (list != null && list.Count > 0)
                                        {
                                            //Results.data = Results.data.Where(w => string.IsNullOrEmpty(w.Inactive)).Distinct().ToList();

                                            list = (from m in list
                                                    where string.IsNullOrEmpty(m.Inactive)
                                                    group m by new { m.RegisteredNo } into g
                                                    select new PlantRegistered_MODEL()
                                                    {
                                                        ID = 0,
                                                        RegisteredNo = g.Key.RegisteredNo,
                                                        DISPLAY_TXT = g.Key.RegisteredNo
                                                    }).Distinct().ToList();

                                            var tempID = 1;
                                            foreach (var m in list)
                                            {
                                                m.ID = tempID;
                                                tempID++;
                                            }


                                            Results.data = list;
                                            break; // found
                                        }
                                       
                                    }      

                                 
                                }                          
                            }
                          
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

        public static PackStyle_RESULT GetPackStyleForm(PackStyle_REQUEST param)
        {
            PackStyle_RESULT Results = new PackStyle_RESULT();
            try
            {
                Results.data = CNService.GetPackStyle(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static CompanyAddress_RESULT GetCompanyAddressForm(CompanyAddress_REQUEST param)
        {
            CompanyAddress_RESULT Results = new CompanyAddress_RESULT();
            try
            {
                Results.data = CNService.GetCompanyAddress(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static CompanyAddress_RESULT GetCompanyAddressFormLOV(CompanyAddress_REQUEST param)
        {
            CompanyAddress_RESULT Results = new CompanyAddress_RESULT();
            try
            {
                Results.data = new List<CompanyAddress_MODEL>();
                if (param != null && param.data != null && !string.IsNullOrEmpty(param.data.RegisteredNo))
                {
                    //var listPlantRegister = CNService.GetPlantRegistered(null);
                    //var listCompanyAddress = (from m in listPlantRegister
                    //            where m.RegisteredNo == param.data.RegisteredNo 
                    //            group  m by new {m.Address} into g
                    //            select new CompanyAddress_MODEL()
                    //            {
                    //                ID = g.Key.Address,
                    //                DISPLAY_TXT = g.Key.Address
                    //            }

                    //    ).Distinct().ToList();
                    SqlParameter[] arge = { new SqlParameter("@registeredNo", param.data.RegisteredNo),
                    new SqlParameter("@ProductCode", param.data.ProductCode)};
                    DataTable dt = CNService.executeProcedure("spGetCompanyAddress", arge);
                    List<CompanyAddress_MODEL> listCompanyAddress = (from DataRow dr in dt.Rows
                                                        select new CompanyAddress_MODEL
                                                        {
                                                            ID = string.Format("{0}", dr["id"]),
                                                            DISPLAY_TXT = string.Format("{0}", dr["Address"])
                                                        }).ToList();
                    

                    Results.data = listCompanyAddress;
                }

                ///Results.data = CNService.GetCompanyAddress(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static StyleofPrinting_RESULT GetStyleofPrintingForm(StyleofPrinting_REQUEST param)
        {
            StyleofPrinting_RESULT Results = new StyleofPrinting_RESULT();
            try
            {
                Results.data = CNService.GetStyleofPrinting(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static Flute_RESULT GetFluteForm(Flute_REQUEST param)
        {
            Flute_RESULT Results = new Flute_RESULT();
            try
            {
                Results.data = CNService.GetFlute(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static RollSheet_RESULT GetRollSheetForm(RollSheet_REQUEST param)
        {
            RollSheet_RESULT Results = new RollSheet_RESULT();
            try
            {
                Results.data = CNService.GetRollSheet(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static RSCDICUT_RESULT GetdicutForm(RSCDICUT_REQUEST param)
        {
            RSCDICUT_RESULT Results = new RSCDICUT_RESULT();
            try
            {
                Results.data = CNService.GetRSCDICUT(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static AppObject_RESULT assign(AppObject_REQUEST param)
        {
            AppObject_RESULT Results = new AppObject_RESULT();
            string Subject = "", Value = "", MailTo = "", MailCc ="";
            try
            {
                CNService.Assign(param);
                var dt = CNService.builditems("select DocumentNo,Description,RequestType,ID,Assignee from sapmaterial where id=" + param.data.ID);
                foreach (DataRow _r in dt.Rows)
                {
                    var assingname = CNService.Getuser(_r["Assignee"].ToString(), "fullname");


                    Results.status = "S";
                    Subject = "SEC PKG Template is created No. : " + _r["DocumentNo"] + " /" + _r["Description"] + "/" + _r["RequestType"];
                    Value = "TransApprove Where fn in ('PA','PA_Approve') and MatDoc='" + _r["ID"] + "'";
                    string u = CNService.curruser();
                    MailCc = CNService.GetModulEmail(string.Format("{0}", CNService.curruser()));// + ";" + CNService.GetModulEmail(CNService.GetActiveBy(Value));
                    MailTo = CNService.GetModulEmail(string.Format("{0}", _r["Assignee"]));

                    CNService.sendemail(MailTo, MailCc, "SEC PKG Template is created No. : " + _r["DocumentNo"] + " <br /><br /> Mail Assign PG" +
                    "<br />Assinee : " + assingname, Subject,"");
                        }
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static AppObject_RESULT Delete_UnusedJob(AppObject_REQUEST param)
        {
            AppObject_RESULT Results = new AppObject_RESULT();
            bool Check = true;
            try
            {
                if (param.data.ID != "")
                {
                    var dt = CNService.builditems(@"select count(*)'countst' from transapprove where fn like 'PG%' and StatusApp =1 and matdoc ='" + param.data.ID + "'");
                    foreach(DataRow rs1 in dt.Rows)
                    {
                        if (string.Format("{0}", rs1["countst"]) != "0")
                        {
                            Results.status = "E";
                            Results.msg = "Can't delete!! This job already sent to PG.";
                            Check = false;
                        }
                    }
                    if (Check)
                    {
                        Results.status = "S";
                        CNService.Delete_UnusedJob(param);
                    }
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static void SendFilesbuEmail(SapMaterial_REQUEST param)
        {
            if (param.data.Id > 0)
                {
                string bodymail = "";
                string strheader = @"<html><head><style>table, th, td {border: 1px solid black; border-collapse:" +
                "collapse;}</style></head><body style=font-size:8pt;font-family:Century Gothic><table><tr bgcolor='#FF2A2A'><td>";
                strheader = strheader + "Field </td><td>New value</td><td>Old value</td></tr>";
                var table = CNService.builditems(@"select * from sapmaterial where id=" + string.Format("{0}", param.data.Id));
                foreach (DataRow dr in table.Rows)
                {
                    var u = CNService.ReadItems(@"(select abc =STUFF(((SELECT DISTINCT  ',' + A.fn
                                         FROM ulogin A
                                         WHERE A.user_name = '" + CNService.curruser() + "' FOR XML PATH(''))), 1, 1, ''))");

                    if (string.Format("{0}", u).Contains("PA_Approve"))
                    {
                        if (!param.data.PrimarySize.ToLower().Equals(string.Format("{0}", dr["PrimarySize"]).ToLower()))//1
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("PrimarySize</td><td>{0}</td><td>{1}</td></tr>", param.data.PrimarySize, dr["PrimarySize"]);
                            CNService.savechangeresult("PrimarySize", string.Format("{0}", dr["PrimarySize"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.ProductCode.ToLower().Equals(string.Format("{0}", dr["ProductCode"]).ToLower()))//1
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("ProductCode</td><td>{0}</td><td>{1}</td></tr>", param.data.ProductCode, dr["ProductCode"]);
                            CNService.savechangeresult("ProductCode", string.Format("{0}", dr["ProductCode"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.FAOZone.ToLower().Equals(string.Format("{0}", dr["FAOZone"]).ToLower()))//2
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("FAOZone</td><td>{0}</td><td>{1}</td></tr>", param.data.FAOZone, dr["FAOZone"]);
                            CNService.savechangeresult("FAOZone", string.Format("{0}", dr["FAOZone"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.Scientific_Name.ToLower().Equals(string.Format("{0}", dr["Scientific_Name"]).ToLower()))//3
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("Scientific_Name</td><td>{0}</td><td>{1}</td></tr>", param.data.Scientific_Name, dr["Scientific_Name"]);
                            CNService.savechangeresult("Scientific_Name", string.Format("{0}", dr["Scientific_Name"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.Specie.ToLower().Equals(string.Format("{0}", dr["Specie"]).ToLower()))//4
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("Specie</td><td>{0}</td><td>{1}</td></tr>", param.data.Specie, dr["Specie"]);
                            CNService.savechangeresult("Specie", string.Format("{0}", dr["Specie"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.Catching_Method.ToLower().Equals(string.Format("{0}", dr["Catching_Method"]).ToLower()))//5
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("Catching_Method</td><td>{0}</td><td>{1}</td></tr>", param.data.Catching_Method, dr["Catching_Method"]);
                            CNService.savechangeresult("Catching_Method", string.Format("{0}", dr["Catching_Method"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.PlantRegisteredNo.ToLower().Equals(string.Format("{0}", dr["PlantRegisteredNo"]).ToLower()))//6
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("PlantRegisteredNo</td><td>{0}</td><td>{1}</td></tr>", param.data.PlantRegisteredNo, dr["PlantRegisteredNo"]);
                            CNService.savechangeresult("PlantRegisteredNo", string.Format("{0}", dr["PlantRegisteredNo"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.Typeof.ToLower().Equals(string.Format("{0}", dr["Typeof"]).ToLower()))//7
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("Typeof</td><td>{0}</td><td>{1}</td></tr>", param.data.Typeof, dr["Typeof"]);
                            CNService.savechangeresult("Typeof", string.Format("{0}", dr["Typeof"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.PackingStyle.ToLower().Equals(string.Format("{0}", dr["PackingStyle"]).ToLower()))//8
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("PackingStyle</td><td>{0}</td><td>{1}</td></tr>", param.data.PackingStyle, dr["PackingStyle"]);
                            CNService.savechangeresult("PackingStyle", string.Format("{0}", dr["PackingStyle"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.Packing.ToLower().Equals(string.Format("{0}", dr["Packing"]).ToLower()))//9
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("Packing</td><td>{0}</td><td>{1}</td></tr>", param.data.Packing, dr["Packing"]);
                            CNService.savechangeresult("Packing", string.Format("{0}", dr["Packing"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.TypeofPrimary.ToLower().Equals(string.Format("{0}", dr["TypeofPrimary"]).ToLower()))//10
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("TypeofPrimary</td><td>{0}</td><td>{1}</td></tr>", param.data.TypeofPrimary, dr["TypeofPrimary"]);
                            CNService.savechangeresult("TypeofPrimary", string.Format("{0}", dr["TypeofPrimary"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.ContainerType.ToLower().Equals(string.Format("{0}", dr["ContainerType"]).ToLower()))//11
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("ContainerType</td><td>{0}</td><td>{1}</td></tr>", param.data.ContainerType, dr["ContainerType"]);
                            CNService.savechangeresult("ContainerType", string.Format("{0}", dr["ContainerType"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.Totalcolour.ToLower().Equals(string.Format("{0}", dr["Totalcolour"]).ToLower()))//12
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("Totalcolour</td><td>{0}</td><td>{1}</td></tr>", param.data.Totalcolour, dr["Totalcolour"]);
                            CNService.savechangeresult("Totalcolour", string.Format("{0}", dr["Totalcolour"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }

                        //if (!param.data.Processcolour.Equals(dr["Processcolour"]))//11
                        //{
                        //    bodymail = bodymail + "<tr><td>";
                        //    bodymail = bodymail + string.Format("Processcolour</td><td>{0}</td><td>{1}</td></tr>", param.data.Processcolour, dr["Processcolour"]);
                        //    CNService.savechangeresult("Processcolour", string.Format("{0}", dr["Processcolour"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        //}
                        if (!param.data.CatchingArea.ToLower().Equals(string.Format("{0}", dr["CatchingArea"]).ToLower()))//13
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("CatchingArea</td><td>{0}</td><td>{1}</td></tr>", param.data.CatchingArea, dr["CatchingArea"]);
                            CNService.savechangeresult("CatchingArea", string.Format("{0}", dr["CatchingArea"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.CompanyNameAddress.ToLower().Equals(string.Format("{0}", dr["CompanyNameAddress"]).ToLower()))//14
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("CompanyNameAddress</td><td>{0}</td><td>{1}</td></tr>", param.data.CompanyNameAddress, dr["CompanyNameAddress"]);
                            CNService.savechangeresult("CompanyNameAddress", string.Format("{0}", dr["CompanyNameAddress"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.TypeofCarton2.ToLower().Equals(string.Format("{0}", dr["TypeofCarton2"]).ToLower()))//15
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("TypeofCarton2</td><td>{0}</td><td>{1}</td></tr>", param.data.TypeofCarton2, dr["TypeofCarton2"]);
                            CNService.savechangeresult("TypeofCarton2", string.Format("{0}", dr["TypeofCarton2"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.Processcolour.ToLower().Equals(string.Format("{0}", dr["Processcolour"]).ToLower()))//16
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("Process colour</td><td>{0}</td><td>{1}</td></tr>", param.data.Processcolour, dr["Processcolour"]);
                            CNService.savechangeresult("Processcolour", string.Format("{0}", dr["Processcolour"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.Plant.ToLower().Equals(string.Format("{0}", dr["Plant"]).ToLower()))//17
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("Plant</td><td>{0}</td><td>{1}</td></tr>", param.data.Plant, dr["Plant"]);
                            CNService.savechangeresult("Plant", string.Format("{0}", dr["Plant"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.LidType.ToLower().Equals(string.Format("{0}", dr["LidType"]).ToLower()))//18
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("LidType</td><td>{0}</td><td>{1}</td></tr>",  param.data.LidType.Replace("<", "&lt;").Replace(">", "&gt;"), dr["LidType"].ToString().Replace("<", "&lt;").Replace(">", "&gt;"));
                            CNService.savechangeresult("LidType", string.Format("{0}", dr["LidType"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.StyleofPrinting.ToLower().Equals(string.Format("{0}", dr["StyleofPrinting"]).ToLower()))//19
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("StyleofPrinting</td><td>{0}</td><td>{1}</td></tr>", param.data.StyleofPrinting, dr["StyleofPrinting"]);
                            CNService.savechangeresult("StyleofPrinting", string.Format("{0}", dr["StyleofPrinting"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.PMScolour.ToLower().Equals(string.Format("{0}", dr["PMScolour"]).ToLower()))//20
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("PMScolour</td><td>{0}</td><td>{1}</td></tr>", param.data.PMScolour, dr["PMScolour"]);
                            CNService.savechangeresult("PMScolour", string.Format("{0}", dr["PMScolour"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.Symbol.ToLower().Equals(string.Format("{0}", dr["Symbol"]).ToLower()))//21
                        {
                            string[] newSymbol = param.data.Symbol.Split(';');
                            string[] oldSymbol = string.Format("{0}", dr["Symbol"]).Split(';');
                            Array.Sort(newSymbol);
                            Array.Sort(oldSymbol);
                            bool _intsymbol = newSymbol.StructuralEquals(oldSymbol);
                            if (_intsymbol==false)
                            {
                                bodymail = bodymail + "<tr><td>";
                                bodymail = bodymail + string.Format("Symbol</td><td>{0}</td><td>{1}</td></tr>", param.data.Symbol, dr["Symbol"]);
                                CNService.savechangeresult("Symbol", string.Format("{0}", dr["Symbol"]), string.Format("{0}", param.data.Id), CNService.curruser());
                            }
                        }
                        if (!param.data.CatchingPeriodDate.ToLower().Equals(string.Format("{0}", dr["CatchingPeriodDate"]).ToLower()))//22
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("CatchingPeriodDate</td><td>{0}</td><td>{1}</td></tr>", param.data.CatchingPeriodDate, dr["CatchingPeriodDate"]);
                            CNService.savechangeresult("CatchingPeriodDate", string.Format("{0}", dr["CatchingPeriodDate"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.PlantAddress.ToLower().Equals(string.Format("{0}", dr["PlantAddress"]).ToLower()))//23
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("PlantAddress</td><td>{0}</td><td>{1}</td></tr>", param.data.PlantAddress, dr["PlantAddress"]);
                            CNService.savechangeresult("PlantAddress", string.Format("{0}", dr["PlantAddress"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.Direction.ToLower().Equals(string.Format("{0}", dr["Direction"]).ToLower()))//24
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("Direction</td><td>{0}</td><td>{1}</td></tr>", param.data.Direction, dr["Direction"]);
                            CNService.savechangeresult("Direction", string.Format("{0}", dr["Direction"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }

                        //////////
                      
                        if (!param.data.MaterialGroup.ToLower().Equals(string.Format("{0}", dr["MaterialGroup"]).ToLower()))//26
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("MaterialGroup</td><td>{0}</td><td>{1}</td></tr>", param.data.MaterialGroup, dr["MaterialGroup"]);
                            CNService.savechangeresult("MaterialGroup", string.Format("{0}", dr["MaterialGroup"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.Brand.ToLower().Equals(string.Format("{0}", dr["Brand"]).ToLower()))//27
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("Brand</td><td>{0}</td><td>{1}</td></tr>", string.Format("{0}", param.data.Brand), string.Format("{0}", dr["Brand"]));
                            CNService.savechangeresult("Brand", string.Format("{0}", dr["Brand"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.ChangePoint.ToLower().Equals(string.Format("{0}", dr["ChangePoint"]).ToLower()))//28
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("ChangePoint</td><td>{0}</td><td>{1}</td></tr>", param.data.ChangePoint, dr["ChangePoint"]);
                            CNService.savechangeresult("ChangePoint", string.Format("{0}", dr["ChangePoint"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.DMSNo.ToLower().Equals(string.Format("{0}", dr["DMSNo"]).ToLower()))//29
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("DMSNo</td><td>{0}</td><td>{1}</td></tr>", param.data.DMSNo, dr["DMSNo"]);
                            CNService.savechangeresult("DMSNo", string.Format("{0}", dr["DMSNo"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }

                    } else if (string.Format("{0}", u).Contains("PG_Approve"))
                    {
                        if (! string.Format("{0}",param.data.PrintingSystem).ToLower().Equals(string.Format("{0}", dr["PrintingSystem"]).ToLower()))//1
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("Printing System</td><td>{0}</td><td>{1}</td></tr>", param.data.PrintingSystem, dr["PrintingSystem"]);
                            CNService.savechangeresult("PrintingSystem", string.Format("{0}", dr["PrintingSystem"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        //if (string.Compare(param.data.RollSheet, string.Format("{0}", dr["RollSheet"]), true) == 0)
                        if (!String.Equals(param.data.RollSheet.ToLower(), string.Format("{0}", dr["RollSheet"]).ToLower(), StringComparison.OrdinalIgnoreCase)) //param.data.RollSheet.Equals(dr["RollSheet"]))//2
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("RollSheet</td><td>{0}</td><td>{1}</td></tr>", param.data.RollSheet, dr["RollSheet"]);
                            CNService.savechangeresult("RollSheet", string.Format("{0}", dr["RollSheet"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.Grandof.ToLower().Equals(string.Format("{0}", dr["Grandof"]).ToLower()))//3
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("Grandof</td><td>{0}</td><td>{1}</td></tr>", param.data.Grandof, dr["Grandof"]);
                            CNService.savechangeresult("Gradeof", string.Format("{0}", dr["Grandof"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.Flute.ToLower().Equals(string.Format("{0}", dr["Flute"]).ToLower()))//3
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("Flute</td><td>{0}</td><td>{1}</td></tr>", param.data.Flute, dr["Flute"]);
                            CNService.savechangeresult("Flute", string.Format("{0}", dr["Flute"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.RSC.ToLower().Equals(string.Format("{0}", dr["RSC"]).ToLower()))//3
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("RSC</td><td>{0}</td><td>{1}</td></tr>", param.data.RSC, dr["RSC"]);
                            CNService.savechangeresult("RSC", string.Format("{0}", dr["RSC"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.Dimension.ToLower().Equals(string.Format("{0}", dr["Dimension"]).ToLower()))//3
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("Dimension</td><td>{0}</td><td>{1}</td></tr>", param.data.Dimension, dr["Dimension"]);
                            CNService.savechangeresult("Dimension", string.Format("{0}", dr["Dimension"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        //if (!param.data.Accessories.Equals(dr["Accessories"]))//3
                        if (!String.Equals(param.data.Accessories.ToLower(), string.Format("{0}", dr["Accessories"]).ToLower(), StringComparison.OrdinalIgnoreCase))
                        {
                            string[] newAccessories = param.data.Accessories.Split(';');
                            string[] oldAccessories = string.Format("{0}", dr["Accessories"]).Split(';');
                            Array.Sort(newAccessories);
                            Array.Sort(oldAccessories);
                            bool _intAccessories = newAccessories.StructuralEquals(oldAccessories);
                            if (_intAccessories == false)
                            {
                                bodymail = bodymail + "<tr><td>";
                                bodymail = bodymail + string.Format("Accessories</td><td>{0}</td><td>{1}</td></tr>", param.data.Accessories, dr["Accessories"]);
                                CNService.savechangeresult("Accessories", string.Format("{0}", dr["Accessories"]), string.Format("{0}", param.data.Id), CNService.curruser());
                            }
                        }
                        if (!param.data.SheetSize.ToLower().Equals(string.Format("{0}", dr["SheetSize"]).ToLower()))//3
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("SheetSize</td><td>{0}</td><td>{1}</td></tr>", param.data.SheetSize, dr["SheetSize"]);
                            CNService.savechangeresult("SheetSize", string.Format("{0}", dr["SheetSize"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                        if (!param.data.Vendor.ToLower().Equals(string.Format("{0}", dr["Vendor"]).ToLower()))//3
                        {
                            bodymail = bodymail + "<tr><td>";
                            bodymail = bodymail + string.Format("Vendor</td><td>{0}</td><td>{1}</td></tr>", param.data.Vendor, dr["Vendor"]);
                            CNService.savechangeresult("Vendor", string.Format("{0}", dr["Vendor"]), string.Format("{0}", param.data.Id), CNService.curruser());
                        }
                    }

                    if (bodymail.Length > 0)
                    {
                        bodymail = strheader + bodymail + "</table></body></html>";
                        string Subject = "SEC PKG Template is created No. : " + dr["DocumentNo"] + " /" + param.data.Description + " edit value follow";
                        string Value = "TransApprove Where MatDoc='" + dr["id"] + "' and fn in ('PA','PG','PA_Approve','PG_Approve')";
                        string MailTo = CNService.GetModulEmail(CNService.GetActiveBy(Value));
                        string MailCc = CNService.GetModulEmail(string.Format("{0}", CNService.curruser()));
                        CNService.sendemail(MailTo, MailCc, bodymail, Subject, "");
                    }
                }
            }
        }
        public static AppObject_RESULT savedata(AppObject_REQUEST param)
        {
            AppObject_RESULT Results = new AppObject_RESULT();
          
            string bodymail = "", strheader = "", Subject = "", Value = "", MailTo = "",MailCc = "";
            try
            {
                var dt = CNService.builditems("select ID,DocumentNo,Description,RequestType,ChangePoint from sapmaterial where id=" + param.data.ID);
                foreach (DataRow _r in dt.Rows)
                {
                    Subject = @"SEC PKG Template is created No. : " + _r["DocumentNo"] + " /" + param.data.Description + "/";
                    switch (string.Format("{0}", _r["RequestType"])) 
                    {
                        case "Develop":
                            Subject = Subject + string.Format("{0}", _r["ChangePoint"])=="C"?"Change Point": "NO Change Point";
                            break;
                        case "Cost&Assign Vendor":
                            Subject = Subject + "Cost&Assign Vendor";
                            break;
                        default:
                            Subject = Subject + "Create Material";
                            break;
                      }
                    if (param.data.event_log.Equals("Reject"))
                    {
                        Value = "TransApprove Where MatDoc='" + _r["ID"] + "' and statusapp='1'";
                        MailTo = CNService.GetModulEmail(CNService.GetActiveBy(Value));
                        switch (param.data.fn)
                        {
                            case "PG_Approve":
                                 
                                CNService.saveActive(param);
                                //Cancel SapMaterial
                                param.data.StatusApp = "5";
                                CNService.saveActive(param);
                                // reset PG
                                param.data.fn = "PG";
                                param.data.StatusApp = "0";
                                CNService.saveActive(param);
                                //MailTo = CNService.GetModulEmail(CNService.Getusermail("PG"));
                                MailCc = CNService.GetModulEmail(string.Format("{0}", CNService.curruser()));
                                CNService.sendemail(MailTo, MailCc, "System SEC PKG Template is created No. :" + _r["DocumentNo"] + "<br /><br />E-Mail Request PG Reject" +
                                      "<br/>Comment : " + param.data.Remark, Subject, "");
                                break;
                            case "PA_Approve":
                                CNService.saveActive(param);
                                //Cancel SapMaterial
                                param.data.StatusApp = "5";
                                CNService.saveActive(param);
                            
                                MailCc = CNService.GetModulEmail(string.Format("{0}", CNService.curruser()));
                                CNService.sendemail(MailTo, MailCc, "System SEC PKG Template is created No. :" + _r["DocumentNo"] + "<br /><br />E-Mail PA Reject" +
                                "<br/>Comment : " + param.data.Remark, Subject, "");
                                break;
                        }

                        CNService.OutboundArtwork(_r["DocumentNo"].ToString());
                        
                    }
                    else
                    {
                        var values = new[] { "PA_Approve", "PG_Approve" };
                        //param.data.fn = CNService.GetApproveLevel(string.Format("{0}", param.data.ID));
                        if (values.Any(param.data.fn.Equals))
                        {
                            //bodymail = bodymail + "<tr><td>";
                            //bodymail = bodymail + rs2("Data") & "</td><td>" & .text & "</td><td>" & rs(rs2("Data")) & "</td></tr>";
                            //if (bodymail.Length > 0) {
                            CNService.saveActive(param);
                            //bodymail = strheader + bodymail + "</table></body></html>";
                            //Subject = "SEC PKG Template is created No. : " + _r["DocumentNo"] + " /" + _r["Description"] + " edit value follow";
                            
                            MailCc = CNService.GetModulEmail(string.Format("{0}", CNService.curruser()));
                            
                            if (CNService.ReadItems("select convert(nvarchar(max), count(statusapp))c from transapprove where statusapp=1 and fn in ('PG_Approve','PA_Approve') and matdoc=" + param.data.ID) == "2")
                            {
                                Value = "TransApprove Where MatDoc='" + param.data.ID + "' and fn in ('PA','PG','PA_Approve','PG_Approve')";
                                MailTo = CNService.GetModulEmail(CNService.GetActiveBy(Value));
                                CNService.sendemail(MailTo, MailCc, "System SEC PKG Template is created No. :" + _r["DocumentNo"] + "<br /><br />E-Mail Final Approved" +
                                        "<br/>Comment : " + param.data.Remark, Subject, "");
                            }
                            else
                            {
                                if (param.data.fn.Equals("PA_Approve"))
                                    Value = "TransApprove Where MatDoc='" + param.data.ID + "' and fn in  ( 'PA','PG_Approve')";
                                else if (param.data.fn.Equals("PG_Approve"))
                                    Value = "TransApprove Where MatDoc='" + param.data.ID + "' and fn in  ( 'PA','PG','PA_Approve')";

                                MailTo = CNService.GetModulEmail(CNService.GetActiveBy(Value));
                                CNService.sendemail(MailTo, MailCc, "System SEC PKG Template is created No. :" + _r["DocumentNo"] + "<br /><br />E-Mail " + param.data.fn +
                                        "<br/>Comment : " + param.data.Remark, Subject, "");
                            }
                           
                        }
                        else if ((new[] { "PA" }).Any(param.data.fn.Equals))
                        {
                            
                            CNService.saveActive(param);
                            AppObject_REQUEST param2 = new AppObject_REQUEST();
                            param2 = param;
                            param2.data.fn = "PG_Assign";
                            CNService.saveActive(param2);
                            MailTo = CNService.GetModulEmail(CNService.Getusermail("PA_Approve"));
                            MailTo = string.Format("{0};{1}", MailTo , CNService.GetModulEmail(CNService.Getusermail("PG_Approve")));
                            MailCc = CNService.GetModulEmail(string.Format("{0}", CNService.curruser()));
                            CNService.sendemail(MailTo, MailCc, "System SEC PKG Template is created No. : " + _r["DocumentNo"] + "<br /><br />E-Mail PA Input Send Request Approve" +
                            "<br/>Comment : " + param.data.Remark, Subject, "");
                        }
                        else if ((new[] { "PG" }).Any(param.data.fn.Equals))
                        {
                            CNService.saveActive(param);
                            MailTo = CNService.GetModulEmail(CNService.Getusermail("PG_Approve"));
                            MailCc = CNService.GetModulEmail(string.Format("{0}", CNService.curruser()));
                            CNService.sendemail(MailTo, MailCc, "System SEC PKG Template is created No. :" + _r["DocumentNo"] + "<br /><br />E-Mail PG Send Request Approve" +
                             "<br/>Comment : " + param.data.Remark, Subject, "");
                        }
                        else if ((new[] { "PG_Assign" }).Any(param.data.fn.Equals))
                        {
                            Value = "TransApprove Where fn='PG_Assign' and MatDoc='" + param.data.ID + "'";
                            //savedata("PG_Assign", "1")
                            CNService.saveActive(param);
                            MailTo = CNService.GetModulEmail(CNService.Getusermail("PG_Approve"));
                            MailCc = CNService.GetModulEmail(string.Format("{0}", CNService.curruser()));
                            CNService.sendemail(MailTo, MailCc, "System SEC PKG Template is created No. :" + _r["DocumentNo"] + "<br /><br />E-Mail Request PG Assign" +
                                "<br/>Comment : " + param.data.Remark, Subject, "");
                        }
                    }
                }
                AppObject_MODEL appObject = new AppObject_MODEL();
                List<AppObject_MODEL> listappObject = new List<AppObject_MODEL>();
                appObject.ID = param.data.ID;
                listappObject.Add(appObject);
                Results.data = listappObject;
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static TypeOf_RESULT GetDescriptionTextByTypeOfBrand(TypeOf_REQUEST param)
        {
            TypeOf_RESULT Results = new TypeOf_RESULT();
            try
            {

                var param_brand = new Brand_REQUEST();
                List<TypeOf_MODEL> listTypeof = new List<TypeOf_MODEL>();
                var listBrand = CNService.GetBrand(param_brand);
                var brand_txt = "";


                if ( !string.IsNullOrEmpty(param.data.DescriptionText))
                {
                    listTypeof = CNService.GetTypeOf(param);
                }


                if (listBrand != null && listBrand.Count > 0 &&  !string.IsNullOrEmpty(param.data.Brand))
                {
                    brand_txt = listBrand.Where(w => w.ID == param.data.Brand).Select(s => s.DISPLAY_TXT).FirstOrDefault();
                }

                if (listTypeof != null && listTypeof.Count  > 0  && !string.IsNullOrEmpty(param.data.DescriptionText))
                {
                    foreach (var v in listTypeof)
                    {
                        v.DescriptionText = v.DescriptionText + "-" + brand_txt;
                    }
                }


                Results.data = listTypeof;
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }


        public static TypeOf_RESULT GetTypeOfForm(TypeOf_REQUEST param)
        {
            TypeOf_RESULT Results = new TypeOf_RESULT();
            try
            {
                Results.data = CNService.GetTypeOf(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static TotalColour_RESULT GetTotalColourForm(TotalColour_REQUEST param)
        {
            TotalColour_RESULT Results = new TotalColour_RESULT();
            try
            {
                Results.data = CNService.GetTotalColour(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static ProcessColour_RESULT GetProcessColourForm(ProcessColour_REQUEST param)
        {
            ProcessColour_RESULT Results = new ProcessColour_RESULT();
            try
            {
                Results.data = CNService.GetProcessColour(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static PrimaryType_RESULT GetPrimaryTypeForm(PrimaryType_REQUEST param)
        {
            PrimaryType_RESULT Results = new PrimaryType_RESULT();
            try
            {
                Results.data = CNService.GetPrimaryType(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static FAOZone_RESULT GetFAOZoneForm(FAOZone_REQUEST param)
        {
            FAOZone_RESULT Results = new FAOZone_RESULT();
            try
            {
                Results.data = CNService.GetFAOZone(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static Symbol_RESULT GetSymbolForm(Symbol_REQUEST param)
        {
            Symbol_RESULT Results = new Symbol_RESULT();
            try
            {
                Results.data = CNService.GetSymbol(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static CatchingPeriod_RESULT GetCatchingPeriodForm(CatchingPeriod_REQUEST param)
        {
            CatchingPeriod_RESULT Results = new CatchingPeriod_RESULT();
            try
            {
                Results.data = CNService.GetCatchingPeriod(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static CatchingArea_RESULT GetCatchingAreaForm(CatchingArea_REQUEST param)
        {
            CatchingArea_RESULT Results = new CatchingArea_RESULT();
            try
            {
                Results.data = CNService.GetCatchingArea(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static Plant_RESULT GetPlantForm(Plant_REQUEST param)
        {
            Plant_RESULT Results = new Plant_RESULT();
            try
            {
                Results.data = CNService.GetPlant(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static History_RESULT GetHistoryForm(History_REQUEST param)
        {
            History_RESULT Results = new History_RESULT();
            try
            {
                Results.data = CNService.GetHistory(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static PMSColour_RESULT GetPMSColourForm(PMSColour_REQUEST param)
        {
            PMSColour_RESULT Results = new PMSColour_RESULT();
            try
            {
                Results.data = CNService.GetPMSColour(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static Gradeof_RESULT GetGradeofForm(Gradeof_REQUEST param)
        {
            Gradeof_RESULT Results = new Gradeof_RESULT();
            try
            {
                Results.data = CNService.GetGradeof(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static ProductGroup_RESULT GetProductGroupForm(ProductGroup_REQUEST param)
        {
            ProductGroup_RESULT Results = new ProductGroup_RESULT();
            try
            {
                Results.data = CNService.GetProductGroup(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static SustainCertSourcing_RESULT GetSustainCertSourcingForm(SustainCertSourcing_REQUEST param)
        {
            SustainCertSourcing_RESULT Results = new SustainCertSourcing_RESULT();
            try
            {
                Results.data = CNService.GetSustainCertSourcing(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static SapMaterial_RESULT SaveIGridSAPMaterial(SapMaterial_REQUEST param)
        {
            SapMaterial_RESULT Results = new SapMaterial_RESULT();
            try
            {
                if (param.data.IsApprove_step)
                    SendFilesbuEmail(param);
            }
            catch (Exception ex)
            {
                //Results.status = "E";
                //Results.msg = CNService.GetErrorMessage(ex);
                //เชียน write log
            }


            SapMaterial_MODEL PA = new SapMaterial_MODEL();
            List<SapMaterial_MODEL> listPA_2 = new List<SapMaterial_MODEL>();
            try
            {
                var IsCreate = param.data.Id == 0;
                var context = new ARTWORKEntities();
                var data = CNService.updateArtworkNumber(param);
                foreach (DataRow rw in data.Rows)
                {
                    PA.Id = Convert.ToInt32(rw["ID"]);
                    PA.DocumentNo = rw["DocumentNo"].ToString();
                }
                  
                listPA_2.Add(PA);
                Results.data = listPA_2;
                Results.status = "S";
                //Results.msg = MessageHelper.GetMessage("MSG_001", context);
                if (IsCreate)
                    Results.msg = "Created documment already." + PA.DocumentNo + "!!!";
                else
                    Results.msg = "Saved documment already." + PA.DocumentNo + "!!!";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }


        public static SapMaterial_RESULT saveCompleteInfoGroup(SapMaterial_REQUEST param)
        {
            SapMaterial_RESULT Results = new SapMaterial_RESULT();
           //SendFilesbuEmail(param);
            SapMaterial_MODEL PA = new SapMaterial_MODEL();
            List<SapMaterial_MODEL> listPA_2 = new List<SapMaterial_MODEL>();
            try
            {
                var context = new ARTWORKEntities();
                var data = CNService.saveCompleteInfoGroup(param);
                foreach (DataRow rw in data.Rows)
                {
                    PA.Id = Convert.ToInt32(rw["ID"]);
                    PA.DocumentNo = rw["DocumentNo"].ToString();
                }

                listPA_2.Add(PA);
                Results.data = listPA_2;
                Results.status = "S";
                if (param.data.IsSaveCompleteInfoGroup == "X")
                {
                    Results.msg = "Save Complete InfoGroup Already.";
                }
                else
                {
                    Results.msg = "Save Final InfoGroup Already.";
                }
             
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }


        public static Attachment_RESULT GetAttachment(Attachment_REQUEST param)
        {
            Attachment_RESULT Results = new Attachment_RESULT();

            try
            {
                Results.data = CNService.GetAttachment(param);
                Results.status = "S";
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        public static SapMaterial_RESULT GetIGridSAPMaterial(SapMaterial_REQUEST param)
        {
            //CNService.massinfogroup();
            //string _DocumentNo = CNService.ReadItems("select DocumentNo from SapMaterial Where id=23105 and StatusApp<>5");
            //CNService.OutboundArtwork(_DocumentNo.ToString());
            SapMaterial_RESULT Results = new SapMaterial_RESULT();
            try
            {
                Results.data = CNService.GetSapMaterial(param);

                if (Results.data != null && Results.data.Count > 0 )
                {
                    foreach (var d in Results.data)
                    {
                        d.ArtowrkURL = CNService.getArtworkURL(d.Id);

                        //authorization
                        string Value = "select * from (select value from dbo.FNC_SPLIT((SELECT fn from ulogin where [user_name]='" + CNService.curruser() + "'),','))#a";
                        var JSON = CNService.builditems(Value);
                        foreach (DataRow rs in JSON.Rows) {
                            foreach (DataRow rs2 in CNService.builditems(@"select * from TransApprove where MatDoc=" +
                                param.data.Id + " and fn='" + rs["value"] + "' and StatusApp in (0,2)").Rows){
                                d.fn = string.Format("{0}", rs["value"]);
                                goto exjump;
                            }
                        }
                        exjump:
                        //----------------------primary size---------------------
                        PrimarySize_REQUEST ro = new PrimarySize_REQUEST();
                        ro.data = new PrimarySize_MODEL();
                        ro.data.Code = d.PrimarySize;
                        ro.data.IsTop1 = true;

                        DataTable listprimary = new DataTable();
                        string strsql = string.Format("select top 1 * from MasPrimarySize where isnull(Inactive,'')<>'X' and {0}", d.PrimarySize_id != 0 ? "Id='" + d.PrimarySize_id + "'" : "code = '" + ro.data.Code + "'");
                        listprimary = CNService.builditems(strsql);

                        if (listprimary != null)
                        {
                            foreach (DataRow rw in listprimary.Rows)
                            {
                                d.PrimarySize = string.Format("{0}:{1}", rw["Code"] , rw["Description"]);
                                //d.LidType = PrimarySize.LidType + ":" + PrimarySize.DescriptionType;
                                d.LidType = string.Format("{0}",rw["DescriptionType"]);
                            }
                        }
                        //----------------------primary size---------------------

                        //--------------------vendor----------------------------

                        if (!string.IsNullOrEmpty(d.Vendor))
                        {
                            Vendor_REQUEST vendor_request = new Vendor_REQUEST();
                            var listVendorIGrid = CNService.GetVendor(vendor_request);
                            var listVendorString = d.Vendor.Split(';');
                            listVendorIGrid = listVendorIGrid.Where(w => listVendorString.Contains(w.Code)).ToList();
                            d.VendorName = "";
                            foreach (var v in listVendorIGrid)
                            {
                                if (d.VendorName != "") d.Vendor += ";";
                                d.VendorName += v.Code + "-" + v.Name;
                            }
                        }

                        //--------------------vendor----------------------------

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
        public static IGRID_RESULT GetInfoGroup(IGRID_REQUEST param)
        {
            IGRID_RESULT Results = new IGRID_RESULT();

            //List<IGRID_MODEL> listPP = new List<IGRID_MODEL>();
            try
            {
                Results.data = CNService.Getinfogroup(param);
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;

        }
        public static IGRID_RESULT GetWorkflowPending(IGRID_REQUEST param) {
            IGRID_RESULT Results = new IGRID_RESULT();
            //string SubChanged_Id = "4260";
            //DataSet ds = new DataSet();
            //ds = CNService.GetQuery("");
            //string strSQL = " select Id,Changed_Charname,Description,Changed_Action,Old_Description from TransMaster where Changed_id ='" + SubChanged_Id + "'";
            //DataTable dt = CNService.builditems(strSQL);
            //foreach (DataRow dr in dt.Rows)
            //{
            //    string _Id = dr["Id"].ToString();
            //    string _Description = dr["Description"].ToString();
            //    string _Changed_Action = dr["Changed_Action"].ToString();
            //    string _old_id = dr["Old_Description"].ToString();
            //    string[] value = { dr["Changed_Charname"].ToString(), _Description, _Id, _Changed_Action, _old_id };
            //    CNService.master_artwork(value);
            //}

            //CNService.OutboundArtwork("2MMK202305120036");
            //List<IGRID_MODEL> listPP = new List<IGRID_MODEL>();

            // {
            //     Results.status = "S";
            //     Results.data = new List<IGRID_MODEL>();
            //     Results.draw = param.draw;
            //     return Results;
            // }

            try
            {
                Results.data = CNService.Getpersonal(param);
                Results.recordsFiltered = Results.data.ToList().Count;
                Results.recordsTotal = Results.data.ToList().Count;
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;

        }




        public static TblFiles_RESULT GetIGridAttachmentInfo(TblFiles_REQUEST param)
        {
            TblFiles_RESULT Results = new TblFiles_RESULT();
            if (param != null && param.data != null && param.data.matdoc == 0 && param.data.id == 0)
            {
                Results.data = new List<TblFiles_MODEL>();
                Results.status = "S";
                return Results;
            } else
            {

                try
                {

                    var strConn = CNService.getIGridStrConn();
                    var where = "";
                    if (param.data.id > 0)
                    {
                        where = CNService.getSQLWhereByJoinStringWithAnd(where, "id=" + param.data.id);
                    }
                    if (param.data.matdoc > 0)
                    {
                        where = CNService.getSQLWhereByJoinStringWithAnd(where, "matdoc=" + param.data.matdoc);
                    }

                    using (SqlConnection con = new SqlConnection(strConn))
                    {
                        con.Open();
                        string strQuery = @"select id,Name,ContentType,Data,MatDoc,ActiveBy from tblFiles2";
                        if (where !="")
                        {
                            strQuery = strQuery + " where " + where;
                        }
                        


                        DataTable dt = new DataTable();
                        SqlDataAdapter oAdapter = new SqlDataAdapter(strQuery, con);
                        // Fill the dataset.
                        oAdapter.Fill(dt);
                        con.Close();

                        List <TblFiles_MODEL> listdata = new List<TblFiles_MODEL>();

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                var v = new TblFiles_MODEL();
                                v.id = Convert.ToInt32(dr["Id"]);
                                v.name = dr["Name"].ToString();
                                v.contenttype = dr["ContentType"].ToString();
                                //v.data = (byte[])dr["Data"];
                                v.data = !string.IsNullOrEmpty(dr["Data"].ToString()) ? (byte[])dr["Data"] : null;
                                v.matdoc = Convert.ToInt32(dr["MatDoc"]);
                                v.activeby = dr["ActiveBy"].ToString();
                                v.canDelete = true;
                                v.canDownload = true;
                                v.create_by_display_txt = v.activeby;
                                v.size = v.data == null ? 0 : v.data.Length;
                                v.extension = Path.GetExtension(v.name).Replace(".", "");
                                v.nodeid = v.id;
                                v.NODE_ID_TXT = v.id.ToString();
                                listdata.Add(v);
                            }
                        }
   
                        Results.data = listdata;
                    }

                    Results.status = "S";
                }

                catch (Exception ex)
                {
                    Results.status = "E";
                    Results.msg = CNService.GetErrorMessage(ex);
                }

            }



            return Results;
        }


        public static TblFiles_RESULT DeleteIGridAttachmentFile(TblFiles_REQUEST param)
        {
            TblFiles_RESULT Results = new TblFiles_RESULT();

            try
            {
                if (param == null || param.data == null || param.data.id == 0)
                {
                    return Results;
                }
                else
                {
                    var strConn = CNService.getIGridStrConn();
                    using (SqlConnection con = new SqlConnection(strConn))
                    {
                       
                    
                        string qry = "delete from tblFiles2 where id=@id";
                        SqlCommand SqlCom = new SqlCommand(qry, con); 
                        SqlCom.Parameters.Add(new SqlParameter("@id", param.data.id));
                 
                        con.Open();
                        SqlCom.ExecuteNonQuery();
                        con.Close();

                        List<TblFiles_MODEL> listdata = new List<TblFiles_MODEL>();    
                        Results.data = listdata;
                    }

                }


                Results.status = "S";
                Results.msg = MessageHelper.GetMessage("MSG_001");
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        
       public static ProductCode_RESULT GetProductCodeIGrid(ProductCode_REQUEST param)
        {
            ProductCode_RESULT Results = new ProductCode_RESULT();
            try
            {
                List<ProductCode_MODEL> list2 = new List<ProductCode_MODEL>();
                if (param != null && param.data != null && !string.IsNullOrEmpty(param.data.PRODUCT_CODE))
                {
                    using (var context = new ARTWORKEntities())
                    {

                        using (CNService.IsolationLevel(context))
                        {
                            var listProduct = param.data.PRODUCT_CODE.Split(';');

                            if (listProduct != null && listProduct.Length > 0)
                            {
                                foreach (var p in listProduct)
                                {

                                    Results.data = CNService.Getproductcode(param);
                                    //var product = context.XECM_M_PRODUCT.Where(w => w.PRODUCT_CODE == p).ToList().FirstOrDefault();
                                    //if (product != null)
                                    //{
                                    //    //product.PACKING_STYLE
                                    //    list2.Add(MapperServices.XECM_M_PRODUCT(product));
                                    break;
                                    //}
                                }

                            }


                        }

                    }
                }
                //Results.data = list2;
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static XECM_M_PRODUCT_RESULT GetPackingStyleByFG(XECM_M_PRODUCT_REQUEST param)
        {
            XECM_M_PRODUCT_RESULT Results = new XECM_M_PRODUCT_RESULT();
            try
            {
                List<XECM_M_PRODUCT_2> list2 = new List<XECM_M_PRODUCT_2>();
                if (param != null && param.data != null && !string.IsNullOrEmpty(param.data.PRODUCT_CODE))
                {
                    using (var context = new ARTWORKEntities())
                    {

                        using (CNService.IsolationLevel(context))
                        {
                            var listProduct = param.data.PRODUCT_CODE.Split(';');

                            if (listProduct != null && listProduct.Length > 0 )
                            {
                                foreach (var p in listProduct)
                                {

                                    var product = context.XECM_M_PRODUCT.Where(w => w.PRODUCT_CODE == p).ToList().FirstOrDefault();
                                    if (product != null)
                                    {
                                        //product.PACKING_STYLE
                                        list2.Add(MapperServices.XECM_M_PRODUCT(product));
                                        break;
                                    }
                                }
                              
                            }

                           
                        }

                    }
                }
                Results.data = list2;
                Results.status = "S";
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }

        //public static TransApprove_RESULT GetTransApproveIGrid(TransApprove_REQUEST param)
        //{
        //    TransApprove_RESULT Results = new TransApprove_RESULT();
        //    try
        //    {
        //        Results.data = new List<TransApprove_Model>();
        //        if (param != null && param.data != null && param.data.MatDoc > 0 )
        //        {
        //            using (var context = new IGRIDEntities())
        //            {

        //                Results.data = context.Database.SqlQuery<TransApprove_Model>("sp_IGRID_REIM_GetSAPMAterialStep @matdoc_id", new SqlParameter("@matdoc_id", param.data.MatDoc)).ToList();
        //            }
        //        }
        
        //        Results.status = "S";
        //    }

        //    catch (Exception ex)
        //    {
        //        Results.status = "E";
        //        Results.msg = CNService.GetErrorMessage(ex);
        //    }
        //    return Results;
        //}



    }
}
