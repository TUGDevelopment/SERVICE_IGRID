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
    public class ImpactedMatDescReportHelper
    {
        public static ImpactedMatDesc_REPORT_RESULT saveImpactedMatDesc(ImpactedMatDesc_REPORT_REQUEST_LIST param)
        {
            ImpactedMatDesc_REPORT_RESULT Results = new ImpactedMatDesc_REPORT_RESULT();
            try
            {
                CNService.saveImpactedMatDesc(param);
                Results.status = "S";
                //using (var context = new ARTWORKEntities())
                //{
                //    Results.status = "S";
                //    Results.msg = MessageHelper.GetMessage("MSG_001", context);
                //}
            }

            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }
            return Results;
        }
        public static ImpactedMatDesc_REPORT_RESULT GetImpactedMatDescReport(ImpactedMatDesc_REPORT_REQUEST param)
        {
            ImpactedMatDesc_REPORT_RESULT Results = new ImpactedMatDesc_REPORT_RESULT();

            try
            {
                
                //Results.data = CNService.GetTrackingReport(param);
                //Results.draw = param.draw;
                using (var context = new IGRIDEntities())
                {
                    context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);


                    var qlist = context.Database.SqlQuery<ImpactedMatDesc_REPORT>
                  ("spGetImpactedMatDesc @Material, @User, @FrDt, @ToDt, @Status, @MasterName, @Action"
                  , new SqlParameter("@Material", string.Format("{0}", param.data.Keyword))
                  
                  , new SqlParameter("@User", string.Format("{0}", param.data.User)=="null" || string.Format("{0}", param.data.User) == "" ? "All": string.Format("{0}", param.data.User))
                  , new SqlParameter("@FrDt", Convert.ToDateTime(param.data.FrDt).ToString("yyyyMMdd"))
                  , new SqlParameter("@ToDt", Convert.ToDateTime(param.data.ToDt).ToString("yyyyMMdd"))
                  , new SqlParameter("@Status", string.Format("{0}", param.data.Status))
                  , new SqlParameter("@MasterName", string.Format("{0}", param.data.MasterName))
                  , new SqlParameter("@Action", param.data.Action == null ? "All": string.Format("{0}", param.data.Action))

                  ).ToList();



                    if (qlist != null && qlist.Count > 0)
                    {
                        foreach (var q in qlist)
                        {

                            if (q.Changed_Action == "Inactive" || q.Changed_Action == "Master Inactive")
                            {
                                if (q.Changed_Tabname == "MasBrand")
                                {
                                    q.Action = "Master Inactive";
                                    q.Char_Description = "";
                                }
                                else
                                {
                                    if (q.Char_NewValue == "")
                                    {
                                        q.Action = "Master Inactive";
                                        q.Char_Description = "";
                    
                                    }
                                    else
                                    {
                                        q.Action = "Update Characteristic Master";
                                        q.Char_Description = q.Char_NewValue;
                                    }

                                }
                            }
                            else if (q.Changed_Action == "Re-Active" || q.Changed_Action == "Master Re-Active")
                            {
                                q.Action = "Master Re-Active";
                                q.Char_Description = "";
              
                            }
                            else if (q.Changed_Action == "Update Characteristic Master")
                            {
                                q.Char_Description = q.Char_NewValue;
                            }
                            else if (q.Changed_Action == "Update" || q.Changed_Action == "Update Characteristic Master")
                            {
                                if (q.Changed_Tabname == "MasBrand")
                                {
                                    q.Action = "Update Characteristic Master";
              
                                }
                                else
                                {
                                    q.Action = "Update Characteristic Master";
                   
                                }
                            }
                            else
                            {
                                 q.Char_Description = q.Char_Description;
                            }
                        }
                    }



                    Results.status = "S";
                    Results.data = qlist;


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