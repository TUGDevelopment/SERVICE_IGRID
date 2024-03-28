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
    public class TwoPHelper
    {
        public static SAP_M_2P_RESULT GetTwoP(SAP_M_2P_REQUEST param)
        {
            string _P_STYLE = ":";
            SAP_M_2P_RESULT Results = new SAP_M_2P_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        if (param == null || param.data == null)
                        {
                            param = new SAP_M_2P_REQUEST();
                            param.data = new SAP_M_2P_2();
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_2P(SAP_M_2P_SERVICE.GetByItem(MapperServices.SAP_M_2P(param.data), context));
                        }
                        else
                        {
                            param.data.IS_ACTIVE = "X";
                            Results.data = MapperServices.SAP_M_2P(SAP_M_2P_SERVICE.GetByItem(MapperServices.SAP_M_2P(param.data), context));
                        }

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].TWO_P_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].PACKING_SYLE_DESCRIPTION + _P_STYLE + Results.data[i].PACK_SIZE_DESCRIPTION;
                                Results.data[i].SEARCH_DISPLAY_TXT = Results.data[i].PACKING_SYLE_DESCRIPTION + Results.data[i].PACK_SIZE_DESCRIPTION;
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

        public static SAP_M_2P_RESULT GetTwoP_New(SAP_M_2P_REQUEST param)
        {
            string _P_STYLE = ":";
            SAP_M_2P_RESULT Results = new SAP_M_2P_RESULT();

            try
            {
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {

                        int PRIMARY_TYPE_ID = 0;
                        if (param == null || param.data == null)
                        {
                            PRIMARY_TYPE_ID = 0;
                        }
                        else
                        {         
                            PRIMARY_TYPE_ID = param.data.PRIMARY_TYPE_ID;                        
                        }

                        var list = context.Database.SqlQuery<SAP_M_2P_2>("sp_ART_IGRID_PACKING_STYLE @PRIMARY_TYPE_ID", new System.Data.SqlClient.SqlParameter("@PRIMARY_TYPE_ID", PRIMARY_TYPE_ID)).ToList();
                        Results.data = list;

                        if (Results.data.Count > 0)
                        {
                            for (int i = 0; i < Results.data.Count; i++)
                            {
                                Results.data[i].ID = Results.data[i].TWO_P_ID;
                                Results.data[i].DISPLAY_TXT = Results.data[i].PACKING_SYLE_DESCRIPTION + _P_STYLE + Results.data[i].PACK_SIZE_DESCRIPTION;
                                Results.data[i].SEARCH_DISPLAY_TXT = Results.data[i].PACKING_SYLE_DESCRIPTION + Results.data[i].PACK_SIZE_DESCRIPTION;
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
