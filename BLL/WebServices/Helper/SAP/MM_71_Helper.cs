using BLL.Helpers;
using BLL.Services;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WebServices.Model;

namespace BLL.WebServices.Helper
{
    public static class MM_71_Helper
    {
        private static int UserID = -2;

        public static void SaveLog(SAP_M_PO_IDOC_MODEL param, string GUID)
        {
            try
            {
                using (var dc = new ARTWORKEntities())
                {
                    ART_SYS_LOG Item = new ART_SYS_LOG();
                    Item.ACTION = "Interface Inbound";
                    Item.TABLE_NAME = "SAP_M_PO_IDOC";
                    if (param.PO_IDOCS != null) Item.NEW_VALUE = CNService.SubString(CNService.Serialize(param.PO_IDOCS), 4000);
                    Item.UPDATE_DATE = DateTime.Now;
                    Item.UPDATE_BY = -2;
                    Item.CREATE_DATE = DateTime.Now;
                    Item.CREATE_BY = -2;
                    Item.OLD_VALUE = GUID;
                    dc.ART_SYS_LOG.Add(Item);
                    dc.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }

        public static SERVICE_RESULT_MODEL SavePO(SAP_M_PO_IDOC_MODEL PO_IDOCS_MODEL)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();
            Results.start = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            string guid = Guid.NewGuid().ToString();
            SaveLog(PO_IDOCS_MODEL, guid);

            SAP_M_PO_IDOC po_idoc = new SAP_M_PO_IDOC();
            SAP_M_PO_IDOC po_idocTmp = new SAP_M_PO_IDOC();

            if (PO_IDOCS_MODEL != null && PO_IDOCS_MODEL.PO_IDOCS != null && PO_IDOCS_MODEL.PO_IDOCS.Count > 0)
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        foreach (PO_IDOC iPO in PO_IDOCS_MODEL.PO_IDOCS)
                        {
                            po_idoc = new SAP_M_PO_IDOC();
                            po_idocTmp = new SAP_M_PO_IDOC();

                            po_idoc.PURCHASE_ORDER_NO = iPO.PurchaseOrderNo;
                            po_idocTmp = SAP_M_PO_IDOC_SERVICE.GetByItem(po_idoc, context).FirstOrDefault();
                            po_idoc = MapperPOIDoc(iPO);

                            if (po_idocTmp != null)
                            {
                                po_idoc.PO_IDOC_ID = po_idocTmp.PO_IDOC_ID;
                            }

                            SAP_M_PO_IDOC_SERVICE.SaveOrUpdateNoLog(po_idoc, context);

                            int idocID = po_idoc.PO_IDOC_ID;

                            if (iPO.ITEM != null && iPO.ITEM.Count > 0)
                            {
                                foreach (PO_IDOC_ITEM item in iPO.ITEM)
                                {
                                    SAP_M_PO_IDOC_ITEM po_idoc_item = new SAP_M_PO_IDOC_ITEM();
                                    SAP_M_PO_IDOC_ITEM po_idoc_item_tmp = new SAP_M_PO_IDOC_ITEM();

                                    po_idoc.PURCHASE_ORDER_NO = iPO.PurchaseOrderNo;
                                    po_idocTmp = SAP_M_PO_IDOC_SERVICE.GetByItem(po_idoc, context).FirstOrDefault();

                                    po_idoc_item.PO_IDOC_ID = idocID;
                                    po_idoc_item.PO_ITEM_NO = item.ItemNoOfPO;

                                    po_idoc_item_tmp = SAP_M_PO_IDOC_ITEM_SERVICE.GetByItem(po_idoc_item, context).FirstOrDefault();
                                    if (po_idoc_item_tmp != null)
                                    {
                                        po_idoc_item.PO_IDOC_ITEM_ID = po_idoc_item_tmp.PO_IDOC_ITEM_ID;
                                    }

                                    po_idoc_item.RECORD_TYPE = item.RecordType;
                                    po_idoc_item.DELETION_INDICATOR = item.DeletionIndicator;
                                    po_idoc_item.QUANTITY = item.Quantity;
                                    po_idoc_item.ORDER_UNIT = item.OrderUnit;
                                    po_idoc_item.ORDER_PRICE_UNIT = item.OrderPriceUnit;
                                    po_idoc_item.NET_ORDER_PRICE = item.NetOrderPrice;
                                    po_idoc_item.PRICE_UNIT = item.Priceunit;
                                    po_idoc_item.AMOUNT = item.Amount;
                                    po_idoc_item.MATERIAL_GROUP = item.MaterialGroup;

                                    if (!String.IsNullOrEmpty(item.DenominatorQuantityConversion))
                                    {
                                        Decimal decOut;
                                        if (Decimal.TryParse(item.DenominatorQuantityConversion.Trim(), out decOut))
                                        {
                                            po_idoc_item.DENOMINATOR_QUANTITY_CONVERSION = decOut;
                                        }
                                    }

                                    if (!String.IsNullOrEmpty(item.NumberatorQuantityConversion))
                                    {
                                        Decimal decOut;
                                        if (Decimal.TryParse(item.NumberatorQuantityConversion.Trim(), out decOut))
                                        {
                                            po_idoc_item.NUMERATOR_QUANTITY_CONVERSION = decOut;
                                        }
                                    }

                                    po_idoc_item.PLANT = item.Plant;
                                    po_idoc_item.METERIAL_NUMBER = item.MaterialNumber;
                                    po_idoc_item.SHORT_TEXT = item.ShortText;
                                    po_idoc_item.DELIVERY_DATE = item.DeliveryDate;

                                    if (item.SalesDocumentNo.StartsWith("0"))
                                    {
                                        item.SalesDocumentNo = item.SalesDocumentNo.Substring(1);
                                    }
                                    po_idoc_item.SALES_DOCUMENT_NO = item.SalesDocumentNo;

                                    if (!String.IsNullOrEmpty(item.SalesDocumentItem))
                                    {
                                        Decimal decOut;
                                        if (Decimal.TryParse(item.SalesDocumentItem.Trim(), out decOut))
                                        {
                                            po_idoc_item.SALES_DOCUMENT_ITEM = decOut;
                                        }
                                    }

                                    po_idoc_item.CREATE_BY = UserID;
                                    po_idoc_item.UPDATE_BY = UserID;

                                    SAP_M_PO_IDOC_ITEM_SERVICE.SaveOrUpdateNoLog(po_idoc_item, context);
                                    Results.cnt++;
                                }
                            }
                        }
                    }

                    #region "Mapping PO"
                    foreach (PO_IDOC iPO in PO_IDOCS_MODEL.PO_IDOCS)
                    {
                        MappingPOWithArtwork(iPO.PurchaseOrderNo);
                    }
                    #endregion

                    Results.status = "S";
                    Results.msg = MessageHelper.GetMessage("MSG_001");
                }
                catch (Exception ex)
                {
                    Results.status = "E";
                    Results.msg = CNService.GetErrorMessage(ex);
                }
            }

            Results.finish = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            CNService.SaveLogReturnInterface(Results, "SAP_M_PO_IDOC", guid,"MM71");

            return Results;
        }

        private static SAP_M_PO_IDOC MapperPOIDoc(PO_IDOC po_resp)
        {
            SAP_M_PO_IDOC po_idoc = new SAP_M_PO_IDOC();
            po_idoc.PURCHASE_ORDER_NO = po_resp.PurchaseOrderNo;
            po_idoc.CURRENCY = po_resp.Currency;
            po_idoc.DATE = po_resp.CreatedDate;
            po_idoc.TIME = po_resp.CreatedTime;
            po_idoc.PURCHASING_ORG = po_resp.PurchasingOrg;
            po_idoc.COMPANY_CODE = po_resp.CompanyCode;
            po_idoc.VENDOR_NO = po_resp.VendorNo;
            po_idoc.VENDOR_NAME = po_resp.VendorName;
            po_idoc.PURCHASER = po_resp.Purchaser;
            po_idoc.CREATE_BY = UserID;
            po_idoc.UPDATE_BY = UserID;
            return po_idoc;
        }

        public static void MappingPOWithArtwork(string PONumber)
        {
            using (var context = new ARTWORKEntities())
            {
                var POItem = (from m in context.SAP_M_PO_IDOC
                              join m2 in context.SAP_M_PO_IDOC_ITEM on m.PO_IDOC_ID equals m2.PO_IDOC_ID
                              where m.PURCHASE_ORDER_NO == PONumber
                              && m2.RECORD_TYPE != "Deleted"
                              select m2).ToList();


                var f_found_newPOItem = false;  //#INC-38225 by aof. 
                var list_newMappingPO = new List<ART_WF_ARTWORK_MAPPING_PO>();  //#INC-38225 by aof. 

                foreach (var item in POItem)
                {
                    var mat5 = item.METERIAL_NUMBER;
                    var so = item.SALES_DOCUMENT_NO;
                    var so_item = item.SALES_DOCUMENT_ITEM.ToString();

                    var processPA = (from m in context.ART_WF_ARTWORK_PROCESS_PA
                                     join m2 in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL on m.ARTWORK_SUB_ID equals m2.ARTWORK_SUB_ID
                                     where m.MATERIAL_NO == mat5
                                     && m2.SALES_ORDER_NO == so
                                     && m2.SALES_ORDER_ITEM == so_item
                                     select m2).ToList();

                    foreach (var itemPA in processPA)
                    {
                        var process = ART_WF_ARTWORK_PROCESS_SERVICE.GetByARTWORK_SUB_ID(itemPA.ARTWORK_SUB_ID, context);

                        if (process != null)
                        {
                            //var REQUEST_ITEM_NO = ART_WF_ARTWORK_REQUEST_ITEM_SERVICE.GetByARTWORK_ITEM_ID(process.ARTWORK_ITEM_ID, context).REQUEST_ITEM_NO;
                            var REQUEST_ITEM_NO = (from p in context.ART_WF_ARTWORK_REQUEST_ITEM
                                                   where p.ARTWORK_ITEM_ID == process.ARTWORK_ITEM_ID
                                                   select p.REQUEST_ITEM_NO).FirstOrDefault();

                            //var existPO = (from p in context.ART_WF_ARTWORK_MAPPING_PO
                            //               where p.ARTWORK_NO == REQUEST_ITEM_NO
                            //               select p).ToList();

                            //if (existPO != null)
                            //{
                            //    foreach (var iExistPO in existPO)
                            //    {
                            //        iExistPO.IS_ACTIVE = "";
                            //        iExistPO.UPDATE_BY = UserID;
                            //        ART_WF_ARTWORK_MAPPING_PO_SERVICE.SaveOrUpdateNoLog(iExistPO, context);
                            //    }
                            //}

                            var mappingPo = new ART_WF_ARTWORK_MAPPING_PO();
                            mappingPo.ARTWORK_NO = REQUEST_ITEM_NO;
                            mappingPo.PO_NO = PONumber;
                            mappingPo.SO_NO = so;
                            mappingPo.MATERIAL_NO = mat5;
                            mappingPo.PO_ITEM = item.PO_ITEM_NO;
                            mappingPo.SO_ITEM = item.SALES_DOCUMENT_ITEM.Value.ToString();
                            mappingPo.CREATE_BY = -1;
                            mappingPo.UPDATE_BY = -1;
                            mappingPo.IS_ACTIVE = "X";

                            var chkItem = ART_WF_ARTWORK_MAPPING_PO_SERVICE.GetByItem(new ART_WF_ARTWORK_MAPPING_PO()
                            {
                                ARTWORK_NO = REQUEST_ITEM_NO,
                                PO_NO = PONumber,
                                SO_NO = so,
                                PO_ITEM = item.PO_ITEM_NO,
                                SO_ITEM = item.SALES_DOCUMENT_ITEM.Value.ToString(),
                            }, context).FirstOrDefault();
                            if (chkItem != null)
                            {
                                mappingPo.ARTWORK_MAPPING_PO_ID = chkItem.ARTWORK_MAPPING_PO_ID;
                                //#437016
                                mappingPo.IS_ACTIVE = chkItem.IS_ACTIVE;
                            }
                            //#INC-38225 by aof start code. 
                            else
                            {
                                f_found_newPOItem = true;
                                if (list_newMappingPO.Where(w=>w.PO_NO == PONumber && w.ARTWORK_NO == REQUEST_ITEM_NO).ToList().Count() == 0 )
                                {
                                    var newMappingPO = new ART_WF_ARTWORK_MAPPING_PO();
                                    newMappingPO.ARTWORK_NO = REQUEST_ITEM_NO;
                                    newMappingPO.PO_NO = PONumber;
                                    list_newMappingPO.Add(newMappingPO);
                                }                    
                            }
                            //#INC-38225 by aof last code.
                            ART_WF_ARTWORK_MAPPING_PO_SERVICE.SaveOrUpdateNoLog(mappingPo, context);
                        }
                    }
                }

                //#INC-38225 by aof start code.
                if (f_found_newPOItem)
                {

                    if (list_newMappingPO !=null && list_newMappingPO.Count > 0 )
                    {
                        foreach (var newMappingPO in list_newMappingPO)
                        {
                            var list_UpdateMappingPO = context.ART_WF_ARTWORK_MAPPING_PO.Where(w => w.PO_NO == newMappingPO.PO_NO && w.ARTWORK_NO == newMappingPO.ARTWORK_NO ).ToList();
                            if (list_UpdateMappingPO != null && list_UpdateMappingPO.Count > 0)
                            {
                                foreach (var mappingPO in list_UpdateMappingPO)
                                {
                                    mappingPO.IS_ACTIVE = "X";
                                    ART_WF_ARTWORK_MAPPING_PO_SERVICE.SaveOrUpdateNoLog(mappingPO, context);
                                }
                            }
                        }
                    }

                   
                }
                //#INC-38225 by aof end code.


                #region "Delete PO Item mask Delete"
                var existMapPO = (from p in context.ART_WF_ARTWORK_MAPPING_PO
                                  where p.PO_NO == PONumber
                                  select p).ToList();

                if (existMapPO != null)
                {
                    var existPO_NO = existMapPO.Select(s => s.PO_NO).Distinct().ToList();

                    if (existPO_NO != null)
                    {
                        var existPO_ID = (from p in context.SAP_M_PO_IDOC
                                          where existPO_NO.Contains(p.PURCHASE_ORDER_NO)
                                          select p.PO_IDOC_ID).ToList();

                        if (existPO_ID != null)
                        {
                            foreach (var iExistPO in existMapPO)
                            {
                                decimal itemNo = 0;
                                itemNo = Convert.ToDecimal(iExistPO.SO_ITEM);

                                var poItem = (from p in context.SAP_M_PO_IDOC_ITEM
                                              where existPO_ID.Contains(p.PO_IDOC_ID)
                                                    && p.PO_ITEM_NO == iExistPO.PO_ITEM
                                                    && p.METERIAL_NUMBER == iExistPO.MATERIAL_NO
                                                    && p.SALES_DOCUMENT_NO == iExistPO.SO_NO
                                                    && p.SALES_DOCUMENT_ITEM == itemNo
                                              select p).FirstOrDefault();

                                if (poItem != null)
                                {
                                    if (poItem.RECORD_TYPE == "Deleted")
                                    {
                                        ART_WF_ARTWORK_MAPPING_PO_SERVICE.DeleteByARTWORK_MAPPING_PO_ID(iExistPO.ARTWORK_MAPPING_PO_ID, context);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region "Delete Mapping not match Sales Order"
                var existMapPO_2 = (from p in context.ART_WF_ARTWORK_MAPPING_PO
                                  where p.PO_NO == PONumber
                                  select p).ToList();

                if (existMapPO_2 != null)
                {
                    var stepPA = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_PA").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();
                    foreach (var itemMapping in existMapPO_2)
                    {
                        var reqItem = (from p in context.ART_WF_ARTWORK_REQUEST_ITEM
                                       where p.REQUEST_ITEM_NO == itemMapping.ARTWORK_NO
                                       select p.ARTWORK_ITEM_ID).FirstOrDefault();

                        if (reqItem > 0)
                        {
                            var process = (from p in context.ART_WF_ARTWORK_PROCESS
                                           where p.ARTWORK_ITEM_ID == reqItem
                                           && p.CURRENT_STEP_ID == stepPA
                                           select p).FirstOrDefault();

                            if (process != null)
                            {
                                var assignSO = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                                where s.ARTWORK_SUB_ID == process.ARTWORK_SUB_ID
                                                && s.SALES_ORDER_NO == itemMapping.SO_NO
                                                && s.SALES_ORDER_ITEM == itemMapping.SO_ITEM
                                                select s).FirstOrDefault();

                                if (assignSO == null)
                                {
                                    ART_WF_ARTWORK_MAPPING_PO_SERVICE.DeleteByARTWORK_MAPPING_PO_ID(itemMapping.ARTWORK_MAPPING_PO_ID,context);
                                }


                            }

                        }
                    }
                }

                #endregion
            }
        }
    }


}
