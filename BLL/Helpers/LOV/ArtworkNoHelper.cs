using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services;
using DAL;
using DAL.Model;

namespace BLL.Helpers
{
    public class ArtworkNoHelper
    {
        public static ART_WF_ARTWORK_REQUEST_ITEM_RESULT GetArtworkNo(ART_WF_ARTWORK_REQUEST_ITEM_REQUEST param)
        {
            ART_WF_ARTWORK_REQUEST_ITEM_RESULT Results = new ART_WF_ARTWORK_REQUEST_ITEM_RESULT();

            try
            {
                int selectTake = 1000;
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        if (param == null || param.data == null)
                        {
                            var aw = (from a in context.ART_WF_ARTWORK_REQUEST_ITEM
                                      select new ART_WF_ARTWORK_REQUEST_ITEM_2()
                                      {
                                          ID = a.ARTWORK_ITEM_ID,
                                          DISPLAY_TXT = a.REQUEST_ITEM_NO
                                      }
                                      ).ToList().OrderByDescending(o => o.DISPLAY_TXT);

                            Results.data = aw.Take(selectTake).ToList();
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                                var aw = (from a in context.ART_WF_ARTWORK_REQUEST_ITEM
                                          where a.REQUEST_ITEM_NO.Contains(param.data.DISPLAY_TXT)
                                          select new ART_WF_ARTWORK_REQUEST_ITEM_2()
                                          {
                                              ID = a.ARTWORK_ITEM_ID,
                                              DISPLAY_TXT = a.REQUEST_ITEM_NO
                                          }
                                    ).ToList().OrderByDescending(o => o.DISPLAY_TXT);

                                Results.data = aw.Take(selectTake).ToList();
                            }
                            else
                            {

                                var aw = (from a in context.ART_WF_ARTWORK_REQUEST_ITEM
                                          select new ART_WF_ARTWORK_REQUEST_ITEM_2()
                                          {
                                              ID = a.ARTWORK_ITEM_ID,
                                              DISPLAY_TXT = a.REQUEST_ITEM_NO
                                          }
                                    ).ToList().OrderByDescending(o => o.DISPLAY_TXT);

                                Results.data = aw.Take(selectTake).ToList();
                            }
                        }

                        Results.status = "S";
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

        public static ART_WF_ARTWORK_REQUEST_ITEM_RESULT GetArtworkNoByMaterialGroup(ART_WF_ARTWORK_REQUEST_ITEM_REQUEST param)
        {
            ART_WF_ARTWORK_REQUEST_ITEM_RESULT Results = new ART_WF_ARTWORK_REQUEST_ITEM_RESULT();

            try
            {
                int selectTake = 200;
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        if (param == null || param.data == null)
                        {
                            Results.status = "S";
                            Results.data = new List<ART_WF_ARTWORK_REQUEST_ITEM_2>();
                            return Results;
                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(param.data.DISPLAY_TXT) && param.data.MATERIAL_GROUP_ID > 0)
                            {
                                var subIdList = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                 where p.MATERIAL_GROUP_ID == param.data.MATERIAL_GROUP_ID
                                                 select p.ARTWORK_SUB_ID).ToList();

                                if (subIdList != null)
                                {
                                    var itemIDList = (from p in context.ART_WF_ARTWORK_PROCESS
                                                      where subIdList.Contains(p.ARTWORK_SUB_ID)
                                                      select p.ARTWORK_ITEM_ID).ToList();

                                    var aw = (from a in context.ART_WF_ARTWORK_REQUEST_ITEM
                                              where itemIDList.Contains(a.ARTWORK_ITEM_ID)
                                             && a.REQUEST_ITEM_NO.Contains(param.data.DISPLAY_TXT)
                                              select new ART_WF_ARTWORK_REQUEST_ITEM_2()
                                              {
                                                  ID = a.ARTWORK_ITEM_ID,
                                                  DISPLAY_TXT = a.REQUEST_ITEM_NO
                                              }
                                        ).ToList().OrderByDescending(o => o.DISPLAY_TXT);

                                    Results.data = aw.Take(selectTake).ToList();
                                }
                            }
                            else
                            {
                                if (param.data.MATERIAL_GROUP_ID > 0)
                                {
                                    var subIdList = (from p in context.ART_WF_ARTWORK_PROCESS_PA
                                                     where p.MATERIAL_GROUP_ID == param.data.MATERIAL_GROUP_ID
                                                     select p.ARTWORK_SUB_ID).ToList();

                                    if (subIdList != null)
                                    {
                                        var itemIDList = (from p in context.ART_WF_ARTWORK_PROCESS
                                                          where subIdList.Contains(p.ARTWORK_SUB_ID)
                                                          select p.ARTWORK_ITEM_ID).ToList();

                                        var aw = (from a in context.ART_WF_ARTWORK_REQUEST_ITEM
                                                  where itemIDList.Contains(a.ARTWORK_ITEM_ID)
                                                  select new ART_WF_ARTWORK_REQUEST_ITEM_2()
                                                  {
                                                      ID = a.ARTWORK_ITEM_ID,
                                                      DISPLAY_TXT = a.REQUEST_ITEM_NO
                                                  }
                                        ).ToList().OrderByDescending(o => o.DISPLAY_TXT);

                                        Results.data = aw.Take(selectTake).ToList();
                                    }
                                }
                            }
                        }

                        Results.status = "S";
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




        //-----------------------------------475099 by aof modify GetArtworkNoByMaterialGroup to GetArtworkNoByMaterialGroupByStoreProcedure-------------------------------------------------------

        public static ART_WF_ARTWORK_REQUEST_ITEM_RESULT GetArtworkNoByMaterialGroupByStoreProcedure(ART_WF_ARTWORK_REQUEST_ITEM_REQUEST param)
        {
            ART_WF_ARTWORK_REQUEST_ITEM_RESULT Results = new ART_WF_ARTWORK_REQUEST_ITEM_RESULT();

            try
            {
                //int selectTake = 1000;
                using (var context = new ARTWORKEntities())
                {
                    using (CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 300;

                        if (param == null || param.data == null)
                        {
                            Results.status = "S";
                            Results.data = new List<ART_WF_ARTWORK_REQUEST_ITEM_2>();
                            return Results;
                        }
                        else
                        {

                            string where = "";

                            if (!String.IsNullOrEmpty(param.data.DISPLAY_TXT))
                            {
                  
                                where = CNService.getSQLWhereByJoinStringWithAnd(where, "WF_NO LIKE '" + param.data.DISPLAY_TXT + "%'");
                            }



                            if (param.data.MATERIAL_GROUP_ID > 0)
                            {

                                where = CNService.getSQLWhereByJoinStringWithAnd(where, "MATERIAL_GROUP_ID = " + param.data.MATERIAL_GROUP_ID + "");


                                var list = context.Database.SqlQuery<ART_WF_ARTWORK_REQUEST_ITEM_2>("sp_ART_WF_ARTWORK_BY_MATERIAL_GROUP @WHERE", new System.Data.SqlClient.SqlParameter("@WHERE", where)).ToList();


                                Results.data = list.OrderByDescending(o => o.DISPLAY_TXT).ToList();
                            }

 
                         
                        }

                        Results.status = "S";
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
        //-----------------------------------475099 by aof modify GetArtworkNoByMaterialGroup to GetArtworkNoByMaterialGroupByStoreProcedure-------------------------------------------------------
    }
}



