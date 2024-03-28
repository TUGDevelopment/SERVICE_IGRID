using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Model;
using BLL.Services;
using WebServices.Model;
using DAL;
using System.Data.Entity;
using BLL.Helpers;
using WebServices.Helper;
using System.Web.Script.Serialization;

namespace WebServices.Helper
{
    public static class MM_72_Hepler
    {
        public static void SaveLog(CHARACTERISTICS param, string GUID)
        {
            try
            {
                using (var dc = new ARTWORKEntities())
                {
                    ART_SYS_LOG Item = new ART_SYS_LOG();
                    Item.ACTION = "Interface Inbound";
                    Item.TABLE_NAME = "SAP_M_CHARACTERISTIC";
                    if (param.Characteristics != null) Item.NEW_VALUE = CNService.SubString(CNService.Serialize(param.Characteristics), 4000);
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

        public static SERVICE_RESULT_MODEL SaveCharacteristics(CHARACTERISTICS param)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();
            Results.start = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            string guid = Guid.NewGuid().ToString();
            SaveLog(param, guid);

            SAP_M_CHARACTERISTIC characteristic = new SAP_M_CHARACTERISTIC();

            if (param.Characteristics.Count > 0)
            {
                int userID = -2;
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        context.Database.CommandTimeout = 600;

                        foreach (CHARACTERISTIC item in param.Characteristics)
                        {
                            Characteristic_Helper.UpdateCharacteristic(item, userID, context);
                            Characteristic_Helper.UpdateBrand(context);
                            Results.cnt++;
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
            }

            Results.finish = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            CNService.SaveLogReturnInterface(Results, "SAP_M_CHARACTERISTIC", guid,"MM72");

            return Results;
        }
    }
}