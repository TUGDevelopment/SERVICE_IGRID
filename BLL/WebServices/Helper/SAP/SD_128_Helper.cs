using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Model;
using BLL.Services;
using WebServices.Model;
using DAL;
using System.Data.Entity;
using BLL.Helpers;
using System.Web.Script.Serialization;

namespace WebServices.Helper
{
    public static class SD_128_Helper
    {
        public static void SaveLog(List<LONG_TXT> param, string GUID)
        {
            try
            {
                using (var context = new ARTWORKEntities())
                {
                    ART_SYS_LOG Item = new ART_SYS_LOG();
                    Item.ACTION = "Interface Inbound-SD128";
                    Item.TABLE_NAME = "SAP_M_LONG_TEXT";
                    if (param != null) Item.NEW_VALUE = CNService.SubString(CNService.Serialize(param), 4000);
                    Item.UPDATE_DATE = DateTime.Now;
                    Item.UPDATE_BY = -2;
                    Item.CREATE_DATE = DateTime.Now;
                    Item.CREATE_BY = -2;
                    Item.OLD_VALUE = GUID;
                    context.ART_SYS_LOG.Add(Item);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }

        public static SERVICE_RESULT_MODEL SaveLongText(List<LONG_TXT> param)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();
            Results.start = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string guid = Guid.NewGuid().ToString();
            SaveLog(param, guid);

            try
            {
                try
                {
                    aSaveLongText(param, Results);
                }
                catch
                {
                    try
                    {
                        System.Threading.Thread.Sleep(5000);
                        aSaveLongText(param, Results);
                    }
                    catch
                    {
                        try
                        {
                            System.Threading.Thread.Sleep(5000);
                            aSaveLongText(param, Results);
                        }
                        catch
                        {
                            try
                            {
                                System.Threading.Thread.Sleep(5000);
                                aSaveLongText(param, Results);
                            }
                            catch
                            {
                                try
                                {
                                    System.Threading.Thread.Sleep(5000);
                                    aSaveLongText(param, Results);
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                        }
                    }
                }

                Results.status = "S";
                Results.msg = MessageHelper.GetMessage("MSG_001");
            }
            catch (Exception ex)
            {
                Results.cnt = 0;
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            Results.finish = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            CNService.SaveLogReturnInterface(Results, "SAP_M_LONG_TEXT", guid,"SD128");

            return Results;
        }

        private static void aSaveLongText(List<LONG_TXT> param, SERVICE_RESULT_MODEL Results)
        {
            Results.cnt = 0;
            int userID = -2;
            foreach (LONG_TXT iTXT in param)
            {
                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        context.Database.CommandTimeout = 600;

                        var so = iTXT.TEXT_NAME;
                        var sqlCommandDelete = @" DELETE FROM dbo.SAP_M_LONG_TEXT WHERE TEXT_NAME LIKE '" + so + "%'";

                        try
                        {
                            context.Database.ExecuteSqlCommand(sqlCommandDelete);
                        }
                        catch
                        {
                            try
                            {
                                System.Threading.Thread.Sleep(5000);
                                context.Database.ExecuteSqlCommand(sqlCommandDelete);
                            }
                            catch
                            {
                                try
                                {
                                    System.Threading.Thread.Sleep(5000);
                                    context.Database.ExecuteSqlCommand(sqlCommandDelete);
                                }
                                catch
                                {
                                    try
                                    {
                                        System.Threading.Thread.Sleep(5000);
                                        context.Database.ExecuteSqlCommand(sqlCommandDelete);
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            System.Threading.Thread.Sleep(5000);
                                            context.Database.ExecuteSqlCommand(sqlCommandDelete);
                                        }
                                        catch (Exception ex)
                                        {
                                            throw ex;
                                        }
                                    }
                                }
                            }
                        }

                        foreach (LINE iLine in iTXT.LINES)
                        {
                            string sqlCommand = string.Empty;

                            sqlCommand = @" INSERT INTO dbo.SAP_M_LONG_TEXT (TEXT_NAME,TEXT_ID,TEXT_LANGUAGE,LINE_ID,LINE_TEXT,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY)
                                            VALUES (N'" + SetFormatString(iTXT.TEXT_NAME) + "',N'" + SetFormatString(iTXT.TEXT_ID) + "',N'" + SetFormatString(iTXT.LANGUAGE) + "'," + iLine.ID + ",N'" + SetFormatString(iLine.TEXT) + "',GETDATE()," + userID + ",GETDATE()," + userID + ")";
                            context.Database.ExecuteSqlCommand(sqlCommand);
                            Results.cnt++;
                        }

                        dbContextTransaction.Commit();
                    }
                }
            }
        }

        public static SERVICE_RESULT_MODEL SaveLongText_BK(List<LONG_TXT> param)
        {
            int userID = -2;
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();
            Results.start = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string guid = Guid.NewGuid().ToString();
            SaveLog(param, guid);

            try
            {
                foreach (LONG_TXT iTXT in param)
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (var dbContextTransaction = CNService.IsolationLevel(context))
                        {
                            context.Database.CommandTimeout = 600;

                            var so = iTXT.TEXT_NAME;
                            var sqlCommandDelete = @" DELETE FROM dbo.SAP_M_LONG_TEXT WHERE TEXT_NAME LIKE '" + so + "%'";

                            try
                            {
                                context.Database.ExecuteSqlCommand(sqlCommandDelete);
                            }
                            catch
                            {
                                try
                                {
                                    System.Threading.Thread.Sleep(5000);
                                    context.Database.ExecuteSqlCommand(sqlCommandDelete);
                                }
                                catch
                                {
                                    try
                                    {
                                        System.Threading.Thread.Sleep(5000);
                                        context.Database.ExecuteSqlCommand(sqlCommandDelete);
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            System.Threading.Thread.Sleep(5000);
                                            context.Database.ExecuteSqlCommand(sqlCommandDelete);
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                System.Threading.Thread.Sleep(5000);
                                                context.Database.ExecuteSqlCommand(sqlCommandDelete);
                                            }
                                            catch (Exception ex)
                                            {
                                                throw ex;
                                            }
                                        }
                                    }
                                }
                            }

                            foreach (LINE iLine in iTXT.LINES)
                            {
                                string sqlCommand = string.Empty;

                                //sqlCommand = "SELECT LONG_TEXT_ID FROM dbo.SAP_M_LONG_TEXT WHERE TEXT_NAME = '" + SetFormatString(iTXT.TEXT_NAME) + "' AND TEXT_ID = '" + SetFormatString(iTXT.TEXT_ID) + "' AND TEXT_LANGUAGE = '" + SetFormatString(iTXT.LANGUAGE) + "' AND LINE_ID = " + iLine.ID + "";
                                //int longTxtId = context.Database.SqlQuery<int>(sqlCommand).FirstOrDefault();

                                //if (longTxtId > 0)
                                //{
                                //    sqlCommand = @" UPDATE  dbo.SAP_M_LONG_TEXT
                                //                SET     TEXT_NAME = N'" + SetFormatString(iTXT.TEXT_NAME) + "',TEXT_ID = N'" + SetFormatString(iTXT.TEXT_ID) + "',TEXT_LANGUAGE = N'" + SetFormatString(iTXT.LANGUAGE) + "',LINE_ID = " + iLine.ID + ",LINE_TEXT = N'" + SetFormatString(iLine.TEXT) + "',UPDATE_DATE = GETDATE(),UPDATE_BY = " + userID + "  WHERE LONG_TEXT_ID = " + longTxtId;
                                //    context.Database.ExecuteSqlCommand(sqlCommand);
                                //    Results.cnt++;
                                //}
                                //else
                                //{
                                sqlCommand = @" INSERT INTO dbo.SAP_M_LONG_TEXT (TEXT_NAME,TEXT_ID,TEXT_LANGUAGE,LINE_ID,LINE_TEXT,CREATE_DATE,CREATE_BY,UPDATE_DATE,UPDATE_BY)
                                                VALUES (N'" + SetFormatString(iTXT.TEXT_NAME) + "',N'" + SetFormatString(iTXT.TEXT_ID) + "',N'" + SetFormatString(iTXT.LANGUAGE) + "'," + iLine.ID + ",N'" + SetFormatString(iLine.TEXT) + "',GETDATE()," + userID + ",GETDATE()," + userID + ")";
                                context.Database.ExecuteSqlCommand(sqlCommand);
                                Results.cnt++;
                                //}
                            }

                            dbContextTransaction.Commit();
                        }
                    }
                }

                Results.status = "S";
                Results.msg = MessageHelper.GetMessage("MSG_001");
            }
            catch (Exception ex)
            {
                Results.cnt = 0;
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            Results.finish = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            CNService.SaveLogReturnInterface(Results, "SAP_M_LONG_TEXT", guid,"SD128");

            return Results;
        }

        public static string SetFormatString(string txt)
        {
            return txt.Replace(@"'", @"''").Trim();
        }
    }
}