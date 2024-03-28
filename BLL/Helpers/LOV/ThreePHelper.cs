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
    public class ThreePHelper
    {


        public static SAP_M_3P_RESULT GetThreePPrimarySizeIGrid(SAP_M_3P_REQUEST param)
        {
            // by aof 202306 for CR#IGRID_REIM----ADD NEW Function@
            SAP_M_3P_RESULT Results = new SAP_M_3P_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {

                        string strWhere = "";
                        if (param == null || param.data == null)
                        {
                            strWhere = "";
                        }
                        else
                        {                        
                                var wCode = CNService.getSQLWhereLikeByConvertString(param.data.CODE, "code", true, true, true);
                                var wCan = CNService.getSQLWhereLikeByConvertString(param.data.CAN, "can", true, true, true);
                                var wCanDesciption = CNService.getSQLWhereLikeByConvertString(param.data.DESCRIPTION, "description", false, true, true); 
                                var wLidType = CNService.getSQLWhereLikeByConvertString(param.data.LIDTYPE, "lidtype", true, true, true);
                                var wContainerType = CNService.getSQLWhereLikeByConvertString(param.data.CONTAINERTYPE, "containertype", false, true, true);
                                var wDescriptionType = CNService.getSQLWhereLikeByConvertString(param.data.DESCRIPTIONTYPE, "descriptiontype", false, true, true);

                                strWhere = CNService.getSQLWhereByJoinStringWithAnd(strWhere, wCode);
                                strWhere = CNService.getSQLWhereByJoinStringWithAnd(strWhere, wCan);
                                strWhere = CNService.getSQLWhereByJoinStringWithAnd(strWhere, wCanDesciption);
                                strWhere = CNService.getSQLWhereByJoinStringWithAnd(strWhere, wLidType);
                                strWhere = CNService.getSQLWhereByJoinStringWithAnd(strWhere, wContainerType);
                                strWhere = CNService.getSQLWhereByJoinStringWithAnd(strWhere, wDescriptionType);                        
                        }

                        var list = context.Database.SqlQuery<SAP_M_3P_2>("sp_ART_IGRID_PACKING_SIZE @where", new System.Data.SqlClient.SqlParameter("@where", strWhere)).ToList();
                        Results.data = list;

                        if (Results.data.Count > 0)
                        {
                            Results.data = Results.data.OrderBy(x => x.CODE).ToList();
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


            public static SAP_M_3P_RESULT GetThreeP(SAP_M_3P_REQUEST param)
        {
            string _P_STYLE = ":";
            SAP_M_3P_RESULT Results = new SAP_M_3P_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            param = new SAP_M_3P_REQUEST();
                            param.data = new SAP_M_3P_2();
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_3P(SAP_M_3P_SERVICE.GetByItem(MapperServices.SAP_M_3P(param.data), context));
                        }
                        else
                        {
                            //param.data.NAME = "ZPKG_SEC_PRIMARY_SIZE";
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_3P(SAP_M_3P_SERVICE.GetByItem(MapperServices.SAP_M_3P(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].THREE_P_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].PRIMARY_SIZE_DESCRIPTION + _P_STYLE + Results.data[i].CONTAINER_TYPE_DESCRIPTION + _P_STYLE + Results.data[i].LID_TYPE_DESCRIPTION;
                                Results.data[i].SEARCH_DISPLAY_TXT = Results.data[i].PRIMARY_SIZE_DESCRIPTION + Results.data[i].CONTAINER_TYPE_DESCRIPTION + Results.data[i].LID_TYPE_DESCRIPTION;
                            }

                            if (param != null && param.data != null && !String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                Results.data = (from u1 in Results.data
                                                where (u1.DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower())
                                                || u1.SEARCH_DISPLAY_TXT.ToLower().Contains(param.data.DISPLAY_TXT.ToLower()))
                                                select u1).ToList();
                            }

                            Results.data = Results.data.OrderBy(x => x.DISPLAY_TXT).ToList();
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
