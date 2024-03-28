using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers
{
    public class PGByPAHelper
    {
        public static ART_WF_ARTWORK_PROCESS_PG_BY_PA_RESULT GetPGByPA(ART_WF_ARTWORK_PROCESS_PG_BY_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PG_BY_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PG_BY_PA_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_PG_BY_PA(ART_WF_ARTWORK_PROCESS_PG_BY_PA_SERVICE.GetAll(context));
                        }
                        else
                        {
                            Results.data = MapperServices.ART_WF_ARTWORK_PROCESS_PG_BY_PA(ART_WF_ARTWORK_PROCESS_PG_BY_PA_SERVICE.GetByItemContain(MapperServices.ART_WF_ARTWORK_PROCESS_PG_BY_PA(param.data), context));
                        }

                        ART_WF_ARTWORK_PROCESS_PG_BY_PA p = new ART_WF_ARTWORK_PROCESS_PG_BY_PA();
                    }
                }
                Results.status = "S";

                if (Results.data.Count > 0)
                {

                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static ART_WF_ARTWORK_PROCESS_PG_BY_PA_RESULT SavePGByPA(ART_WF_ARTWORK_PROCESS_PG_BY_PA_REQUEST param)
        {
            ART_WF_ARTWORK_PROCESS_PG_BY_PA_RESULT Results = new ART_WF_ARTWORK_PROCESS_PG_BY_PA_RESULT();
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        string msg = ArtworkProcessHelper.checkDupWF(param.data.PROCESS, context);
                        if (msg != "")
                        {
                            Results.status = "E";
                            Results.msg = msg;
                            return Results;
                        }

                        string msgError = "Please select required field in PA Data ({0}) before send to PG";
                        var PAData = ART_WF_ARTWORK_PROCESS_PA_SERVICE.GetByItem(new ART_WF_ARTWORK_PROCESS_PA() { ARTWORK_SUB_ID = param.data.ARTWORK_SUB_ID }, context).FirstOrDefault();
                        var label = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(new SAP_M_CHARACTERISTIC() { VALUE = "K", NAME = "ZPKG_SEC_GROUP" }, context).FirstOrDefault().CHARACTERISTIC_ID;
                        var leaflet = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(new SAP_M_CHARACTERISTIC() { VALUE = "L", NAME = "ZPKG_SEC_GROUP" }, context).FirstOrDefault().CHARACTERISTIC_ID;
                        var insertPaper = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(new SAP_M_CHARACTERISTIC() { VALUE = "P", NAME = "ZPKG_SEC_GROUP" }, context).FirstOrDefault().CHARACTERISTIC_ID;
                        var sticker = SAP_M_CHARACTERISTIC_SERVICE.GetByItem(new SAP_M_CHARACTERISTIC() { VALUE = "J", NAME = "ZPKG_SEC_GROUP" }, context).FirstOrDefault().CHARACTERISTIC_ID;
                        if (PAData != null)
                        {
                            msg = "";
                            if (PAData.MATERIAL_GROUP_ID == null)
                            {
                                msg += "Material group";
                            }
                            if (PAData.THREE_P_ID == null)
                            {
                                if (msg.Length > 0) msg += ", ";
                                msg += "Primary size";
                            }
                            if (PAData.MATERIAL_GROUP_ID != label && PAData.MATERIAL_GROUP_ID != leaflet && PAData.MATERIAL_GROUP_ID != insertPaper && PAData.MATERIAL_GROUP_ID != sticker)
                            {
                                if (PAData.TWO_P_ID == null)
                                {
                                    if (msg.Length > 0) msg += ", ";
                                    msg += "Packing style";
                                }
                                if (PAData.PACK_SIZE_ID == null && (PAData.TWO_P_ID == -1 || PAData.TWO_P_ID == null))
                                {
                                    if (msg.Length > 0) msg += ", ";
                                    msg += "Pack size";
                                }
                            }
                            if (PAData.TOTAL_COLOUR_ID == null)
                            {
                                if (msg.Length > 0) msg += ", ";
                                msg += "Total colour";
                            }
                            if (PAData.STYLE_OF_PRINTING_ID == null)
                            {
                                if (msg.Length > 0) msg += ", ";
                                msg += "Style of printing";
                            }
                            if (msg.Length > 0)
                            {
                                Results.status = "E";
                                Results.msg = string.Format(msgError, msg);
                                return Results;
                            }
                        }
                        else
                        {
                            Results.status = "E";
                            Results.msg = string.Format(msgError, "Material group, Primary size, Packing style, Pack size");
                            return Results;
                        }

                        ART_WF_ARTWORK_PROCESS_RESULT processResults = new ART_WF_ARTWORK_PROCESS_RESULT();
                        if (param.data.PROCESS != null)
                        {
                            processResults = ArtworkProcessHelper.SaveProcess(param.data.PROCESS, context);
                        }

                        ART_WF_ARTWORK_PROCESS_PG_BY_PA PGData = new ART_WF_ARTWORK_PROCESS_PG_BY_PA();
                        PGData = MapperServices.ART_WF_ARTWORK_PROCESS_PG_BY_PA(param.data);

                        if (processResults != null && processResults.data != null && processResults.data.Count > 0)
                        {
                            PGData.ARTWORK_SUB_ID = processResults.data[0].ARTWORK_SUB_ID;
                        }

                        ART_WF_ARTWORK_PROCESS_PG_BY_PA_SERVICE.SaveOrUpdate(PGData, context);

                        Results.data = new List<ART_WF_ARTWORK_PROCESS_PG_BY_PA_2>();
                        ART_WF_ARTWORK_PROCESS_PG_BY_PA_2 item = new ART_WF_ARTWORK_PROCESS_PG_BY_PA_2();
                        List<ART_WF_ARTWORK_PROCESS_PG_BY_PA_2> listItem = new List<ART_WF_ARTWORK_PROCESS_PG_BY_PA_2>();

                        item.ARTWORK_PROCESS_PG_ID = PGData.ARTWORK_PROCESS_PG_ID;
                        listItem.Add(item);

                        Results.data = listItem;

                        dbContextTransaction.Commit();

                        foreach (var process in processResults.data)
                            EmailService.sendEmailArtwork(process.ARTWORK_REQUEST_ID, process.ARTWORK_SUB_ID, "WF_SEND_TO", context);

                        Results.status = "S";
                        Results.msg = MessageHelper.GetMessage("MSG_001",context);
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


    }



}
