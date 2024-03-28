using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;
using BLL.Services;
using DAL;
using System.Globalization;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Configuration;
using BLL.DocumentManagement;
using System.Data.SqlClient;
using System.Data;

namespace BLL.Helpers
{
    public class IGridSummaryReportHelper
    {



        public static IGRID_RESULT GetIGridInfoGroupReport(IGRID_REQUEST param)
        {
            IGRID_RESULT Results = new IGRID_RESULT();
            if (param != null && param.data != null && param.data.FIRST_LOAD == true)
            {

                Results.status = "S";
                Results.data = new List<IGRID_MODEL>();
                Results.draw = param.draw;
                return Results;

            }
            try
            {
                using (var context = new IGRIDEntities())
                {
                    context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                    var q = context.Database.SqlQuery<IGRID_MODEL>("spinfogroup  @where", new SqlParameter("@where", string.Format("{0}", param.data.WHERE))).ToList();

                    Results.status = "S";
                    Results.data = q;
                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }


        public static IGridSummary_REPORT_RESULT GetIGridCompleteReport(IGridSummary_REPORT_REQUEST param)
        {
            IGridSummary_REPORT_RESULT Results = new IGridSummary_REPORT_RESULT();

            try
            {
                using (var context = new IGRIDEntities())
                {
                    context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                    var q = context.Database.SqlQuery<IGridSummary_REPORT>
                  ("spGetComplete @where"
                  , new SqlParameter("@where", string.Format("{0}", param.data.where))

                  ).ToList();

                    Results.status = "S";
                    Results.data = q;


                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }




        public static IGridSummary_REPORT_RESULT GetIGridSummaryReport(IGridSummary_REPORT_REQUEST param)
        {
            IGridSummary_REPORT_RESULT Results = new IGridSummary_REPORT_RESULT();
            if (param != null && param.data != null && param.data.first_load == "1")
            {

                Results.status = "S";
                Results.data = new List<IGridSummary_REPORT>();
                Results.draw = param.draw;
                return Results;

            }
            try
            {
                using (var context = new IGRIDEntities())
                {
                    context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                    var q = context.Database.SqlQuery<IGridSummary_REPORT>
                  ("spGetselectall @user, @where"
                  , new SqlParameter("@user", string.Format("{0}", CNService.curruser()))
                  , new SqlParameter("@where", string.Format("{0}", param.data.where))

                  ).ToList();

                    Results.status = "S";
                    Results.data = q;


                }
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static IGridSummary_Group_REPORT_RESULT GetIGridSummaryGroupReport(IGridSummary_REPORT_REQUEST param)
        {
            IGridSummary_Group_REPORT_RESULT Results = new IGridSummary_Group_REPORT_RESULT();
            if (param != null && param.data != null && param.data.first_load == "1")
            {

                Results.status = "S";
                Results.data = new List<IGridSummary_Group_REPORT>();
                Results.draw = param.draw;
                return Results;

            }
            try
            {
                using (var context = new IGRIDEntities())
                {
                    context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);

                    var qList = context.Database.SqlQuery<IGridSummary_REPORT>
                  ("spGetselectall @user, @where"
                  , new SqlParameter("@user", string.Format("{0}", CNService.curruser()))
                  , new SqlParameter("@where", string.Format("{0}", param.data.where))

                  ).ToList();



                    var prev_DocumentNo = "";
                    var listGroup = new List<IGridSummary_Group_REPORT>();
                    var group = new IGridSummary_Group_REPORT();
                    var seq = 1;
                    foreach (var q in qList)
                    {
                        if (q.DocumentNo != prev_DocumentNo)
                        {
                            group = new IGridSummary_Group_REPORT();
                            group.data = new List<IGridSummary_REPORT>();
                            group.Condition = q.DocumentNo;
                            //group.RequestType = q.DocumentNo;
                            group.DocumentNo = q.DocumentNo;
                            //group.DMSNo = q.DocumentNo;
                            //group.Material = q.DocumentNo;
                            //group.Description = q.DocumentNo;
                            //group.MaterialGroup = q.DocumentNo;
                            //group.Brand = q.DocumentNo;
                            //group.fn = q.DocumentNo;
                            //group.CreateOn = q.DocumentNo;
                            //group.ActiveBy = q.DocumentNo;
                            //group.StatusApp = q.DocumentNo;

                            listGroup.Add(group);
                           seq = 1;
                        }

                        seq += 1;
                        q.seq = seq;
                        if (q.Brand == null)
                        {
                            q.Brand = "";
                        }
                        group.data.Add(q);
                        if (!string.IsNullOrEmpty(param.data.export_excel))
                        {
                            group.data = group.data.OrderBy(o => o.seq).ToList();
                        } else
                        {
                            group.data = group.data.OrderByDescending(o => o.seq).ToList();
                        }
                     
                        prev_DocumentNo = q.DocumentNo;
                    }




                    Results.status = "S";
                    Results.data = listGroup;


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