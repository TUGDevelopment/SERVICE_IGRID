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
    public class DiCutHelper
    {
        public static SAP_M_CHARACTERISTIC_RESULT GetDiCut(SAP_M_CHARACTERISTIC_REQUEST param)
        {
            SAP_M_CHARACTERISTIC_RESULT Results = new SAP_M_CHARACTERISTIC_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            SAP_M_CHARACTERISTIC_2 characteristic = new SAP_M_CHARACTERISTIC_2();
                            characteristic.NAME = "ZPKG_SEC_RSC_DI";
                            characteristic.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItemContain(MapperServices.SAP_M_CHARACTERISTIC(characteristic), context));
                        }
                        else
                        {
                            // --------by aof 202306 for CR#IGRID_REIM-------- start 
                            //param.data.NAME = "ZPKG_SEC_RSC_DI";
                            //param.data.DESCRIPTION = param.data.DISPLAY_TXT;
                            //param.data.IS_ACTIVE = "X";
                            //Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItemContain(MapperServices.SAP_M_CHARACTERISTIC(param.data), context));
                            // by aof commneted
                            //start by aof writhe IGRID
                            if (param.data.PACKAGING_TYPE_ID > 0)
                            {
                                var charPackagingType = (from m in context.SAP_M_CHARACTERISTIC where m.CHARACTERISTIC_ID == param.data.PACKAGING_TYPE_ID select m).ToList().FirstOrDefault();
                                if (charPackagingType != null && charPackagingType.CHARACTERISTIC_ID > 0)
                                {
                                    var strPackagingType = charPackagingType.VALUE + ':' + charPackagingType.DESCRIPTION;
                                    var listConstant = (from m in context.ART_M_CONSTANT
                                                        where m.VARIABLE_NAME == strPackagingType && m.FUNCAREA == "ZPKG_SEC_RSC_DI"
                                                        select m.LOWVALUE).ToList();
                                    var listChar2 = (from m in context.SAP_M_CHARACTERISTIC
                                                     where m.NAME == "ZPKG_SEC_RSC_DI" && m.IS_ACTIVE == "X" && listConstant.Contains(m.VALUE)
                                                     select m).ToList();
                                    Results.data = MapperServices.SAP_M_CHARACTERISTIC(listChar2);
                                }
                                else
                                {
                                    param.data.NAME = "ZPKG_SEC_RSC_DI";
                                    param.data.DESCRIPTION = param.data.DISPLAY_TXT;
                                    param.data.IS_ACTIVE = "X";
                                    Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItemContain(MapperServices.SAP_M_CHARACTERISTIC(param.data), context));
                                }
                            }
                            else
                            {
                                param.data.NAME = "ZPKG_SEC_RSC_DI";
                                param.data.DESCRIPTION = param.data.DISPLAY_TXT;
                                param.data.IS_ACTIVE = "X";
                                Results.data = MapperServices.SAP_M_CHARACTERISTIC(SAP_M_CHARACTERISTIC_SERVICE.GetByItemContain(MapperServices.SAP_M_CHARACTERISTIC(param.data), context));
                            }
                            // --------by aof 202306 for CR#IGRID_REIM-------- end

                        }

                        Results.status = "S";

                        if (Results.data.Count > 0)
                        {
                            var result = Results.data.GroupBy(p => p.VALUE)
                               .Select(grp => grp.First())
                               .ToList();

                            Results.data = result.OrderBy(x => x.VALUE).ToList();

                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                Results.data = (from u1 in Results.data
                                                where (u1.VALUE.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.DESCRIPTION.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                                select u1).ToList();
                            }

                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].CHARACTERISTIC_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].DESCRIPTION;
                            }
                        }
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
