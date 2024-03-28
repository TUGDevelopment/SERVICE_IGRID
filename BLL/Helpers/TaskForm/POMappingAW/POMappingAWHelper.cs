using BLL.DocumentManagement;
using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using WebServices.Helper;

namespace BLL.Helpers
{
    public class POMappingAWHelper
    {
        public static ART_WF_ARTWORK_MAPPING_PO_RESULT GetPOMappingAW(ART_WF_ARTWORK_MAPPING_PO_REQUEST param)
        {
            ART_WF_ARTWORK_MAPPING_PO_RESULT Results = new ART_WF_ARTWORK_MAPPING_PO_RESULT();

            try
            {

                if (param == null || param.data == null)
                {
                    return Results;
                }
                else
                {
                    if (!String.IsNullOrEmpty(param.data.ARTWORK_NO))
                    {
                        using (var context = new ARTWORKEntities())
                        {
                            using (CNService.IsolationLevel(context))
                            {
                                var listPO = (from p in context.ART_WF_ARTWORK_MAPPING_PO
                                              where p.ARTWORK_NO == param.data.ARTWORK_NO
                                              select new ART_WF_ARTWORK_MAPPING_PO_2()
                                              {
                                                  PO_NO = p.PO_NO,
                                                  ARTWORK_NO = p.ARTWORK_NO,
                                                  IS_ACTIVE = p.IS_ACTIVE

                                              }).Distinct().ToList();

                                if (listPO != null)
                                {
                                    List<string> listPOnember = new List<string>();
                                    List<SAP_M_COMPANY> listCompany = new List<SAP_M_COMPANY>();

                                    listPOnember = listPO.Select(s => s.PO_NO).ToList();

                                    var poDetail = (from p in context.SAP_M_PO_IDOC
                                                    where listPOnember.Contains(p.PURCHASE_ORDER_NO)
                                                    select p).ToList();

                                    //var companiesPO = poDetail.Select(s => s.COMPANY_CODE).Distinct().ToList();

                                    //if (companiesPO != null)
                                    //{
                                    //    listCompany = (from c in context.SAP_M_COMPANY
                                    //                   where companiesPO.Contains(c.COMPANY_CODE)
                                    //                   select c).ToList();
                                    //}

                                    for (int i = 0; i < listPO.Count; i++)
                                    {
                                        var poTmp = poDetail.Where(s => s.PURCHASE_ORDER_NO == listPO[i].PO_NO).FirstOrDefault();
                                        if (poTmp != null)
                                        {
                                            // var comp = listCompany.Where(s => s.COMPANY_CODE == poTmp.COMPANY_CODE).FirstOrDefault();

                                            //listPO[i].COMPANY_DISPLAY_TXT = poTmp.COMPANY_CODE;

                                            //if (comp != null)
                                            //{
                                            //    listPO[i].COMPANY_DISPLAY_TXT = poTmp.COMPANY_CODE + ":" + comp.DESCRIPTION;
                                            //}

                                            listPO[i].CURRENCY_DISPLAY_TXT = poTmp.CURRENCY;
                                            listPO[i].PURCHASING_ORG_DISPLAY_TXT = poTmp.PURCHASING_ORG;
                                            //listPO[i].COMPANY_DISPLAY_TXT = poTmp.COMPANY_CODE;
                                            listPO[i].VENDOR_DISPLAY_TXT = poTmp.VENDOR_NO + ":" + poTmp.VENDOR_NAME;
                                            listPO[i].PURCHASER_DISPLAY_TXT = poTmp.PURCHASER;
                                            listPO[i].PO_NO2 = EncryptionService.EncryptAndUrlEncode(listPO[i].PO_NO);
                                        }
                                    }

                                    Results.data = listPO;
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

        public static ART_WF_ARTWORK_MAPPING_PO_RESULT SavePOMappingAW(ART_WF_ARTWORK_MAPPING_PO_REQUEST param)
        {
            ART_WF_ARTWORK_MAPPING_PO_RESULT Results = new ART_WF_ARTWORK_MAPPING_PO_RESULT();
            ART_WF_ARTWORK_MAPPING_PO poMap = new ART_WF_ARTWORK_MAPPING_PO();

            try
            {

                if (param == null || param.data == null)
                {
                    return Results;
                }
                else
                {
                    if (!String.IsNullOrEmpty(param.data.PO_NO))
                    {
                        using (var context = new ARTWORKEntities())
                        {
                            using (var dbContextTransaction = CNService.IsolationLevel(context))
                            {

                                var listPO = (from o in context.ART_WF_ARTWORK_MAPPING_PO
                                              where o.PO_NO == param.data.PO_NO
                                              select o).ToList();

                                if (listPO != null)
                                {
                                    foreach (var iPO in listPO)
                                    {
                                        iPO.IS_ACTIVE = param.data.IS_ACTIVE;
                                        ART_WF_ARTWORK_MAPPING_PO_SERVICE.SaveOrUpdate(iPO, context);
                                    }
                                }


                                dbContextTransaction.Commit();
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
    }
}
