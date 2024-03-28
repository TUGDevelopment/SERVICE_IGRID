using BLL.Helpers;
using BLL.Services;
using DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WebServices.Model;

namespace BLL.WebServices.Helper
{
    public static class MM_70_Helper
    {
        //private static int UserID = -2;
        //public static SERVICE_RESULT_MODEL ReadFile()
        //{
        //    SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();
        //    SAP_M_PLANT _plant = new SAP_M_PLANT();


        //    try
        //    {
        //        string path = ConfigurationManager.AppSettings["MM70_Path"];
        //        string[] lines = File.ReadAllLines(path);

        //        using (var context = new ARTWORKEntities())
        //        {
        //            using (var dbContextTransaction = CNService.IsolationLevel(context))
        //            {
        //                foreach (string line in lines)
        //                {
        //                    char[] delimiters = new char[] { '\t' };
        //                    string[] _text = line.Split(delimiters,
        //                                     StringSplitOptions.None);

        //                    if (_text.Count() > 0)
        //                    {
        //                        _plant = new SAP_M_PLANT();

        //                        _plant.PLANT = _text[0].Trim();

        //                        var _plantTmp = SAP_M_PLANT_SERVICE.GetByItem(_plant, context).FirstOrDefault();
        //                        if (_plantTmp != null)
        //                        {
        //                            _plant.PLANT_ID = _plantTmp.PLANT_ID;
        //                        }

        //                        _plant.NAME = _text[1].Trim();
        //                        _plant.IS_ACTIVE = "X";
        //                        _plant.CREATE_BY = UserID;
        //                        _plant.UPDATE_BY = UserID;

        //                        SAP_M_PLANT_SERVICE.SaveOrUpdate(_plant, context);
        //                    }
        //                }

        //                dbContextTransaction.Commit();
        //            }
        //        }

        //        Results.status = "S";
        //        Results.msg = MessageHelper.GetMessage("MSG_001");
        //    }
        //    catch (Exception ex)
        //    {
        //        Results.status = "E";
        //        Results.msg = CNService.GetErrorMessage(ex);
        //    }


        //    return Results;
        //}

        public static void SaveLog(PLANT_MODEL param, string GUID)
        {
            try
            {
                using (var dc = new ARTWORKEntities())
                {
                    ART_SYS_LOG Item = new ART_SYS_LOG();
                    Item.ACTION = "Interface Inbound";
                    Item.TABLE_NAME = "SAP_M_PLANT";
                    if (param.Plants != null) Item.NEW_VALUE = CNService.SubString(CNService.Serialize(param.Plants), 4000);
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

        public static SERVICE_RESULT_MODEL SavePlant(PLANT_MODEL PlantModel)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();
            Results.start = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            string guid = Guid.NewGuid().ToString();
            SaveLog(PlantModel, guid);

            SAP_M_PLANT plant = new SAP_M_PLANT();
            SAP_M_PLANT plantTmp = new SAP_M_PLANT();

            int userID = -2;

            if (PlantModel != null && PlantModel.Plants != null && PlantModel.Plants.Count > 0)
            {
                try
                {
                    using (var context = new ARTWORKEntities())
                    {
                        foreach (PLANT iPlant in PlantModel.Plants)
                        {
                            plant = new SAP_M_PLANT();
                            plantTmp = new SAP_M_PLANT();

                            plant.PLANT = iPlant.PlantCode;
                            plantTmp = SAP_M_PLANT_SERVICE.GetByItem(plant, context).FirstOrDefault();

                            if (plantTmp != null)
                            {
                                plant.PLANT_ID = plantTmp.PLANT_ID;
                            }

                            plant.NAME = iPlant.PlantDescription;
                            plant.IS_ACTIVE = "X";
                            plant.CREATE_BY = userID;
                            plant.UPDATE_BY = userID;

                            SAP_M_PLANT_SERVICE.SaveOrUpdateNoLog(plant, context);
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

            CNService.SaveLogReturnInterface(Results, "SAP_M_PLANT", guid,"MM70");

            return Results;
        }
    }
}
