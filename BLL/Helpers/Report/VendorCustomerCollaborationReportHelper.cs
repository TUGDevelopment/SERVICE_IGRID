using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Model;
using BLL.Services;
using DAL;
using System.Data.Entity;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace BLL.Helpers
{
    public class VendorCustomerCollaborationReportHelper
    {
        #region --------------------------------- customer artwork ---------------------------------
        public static CUST_ARTWORK_REPORT_RESULT GetCustomerArtworkReport(CUST_ARTWORK_REPORT_REQUEST param)
        {
            CUST_ARTWORK_REPORT_RESULT Results = new CUST_ARTWORK_REPORT_RESULT();

            try
            {
                var cnt = 0;
                var listResultAll = QuerCustomerArtworkReport(param, ref cnt);

                Results.recordsTotal = cnt;
                Results.recordsFiltered = cnt;

                bool filter = false;
                if (param.columns != null)
                {
                    foreach (var item in param.columns)
                    {
                        if (!string.IsNullOrEmpty(item.search.value))
                        {
                            filter = true;
                            break;
                        }
                    }
                }

                if (filter)
                {
                    listResultAll = FilterDataCustomerArtworkView(param, ref cnt);
                    Results.recordsFiltered = cnt;
                }

                if (param.order != null && param.order.Count > 0)
                {
                    listResultAll = OrderByCustomerArtwork(listResultAll, param);
                }

                Results.status = "S";
                Results.data = listResultAll;
                Results.draw = param.draw;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static List<CUST_ARTWORK_REPORT> QuerCustomerArtworkReport(CUST_ARTWORK_REPORT_REQUEST param, ref int cnt)
        {
            List<CUST_ARTWORK_REPORT> listReport = new List<CUST_ARTWORK_REPORT>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 500;
                    IEnumerable<CUST_ARTWORK_REPORT> custArtworkList = null;
                    try
                    {
                        var sqlCommand = new StringBuilder(@"select * from dbo.UFN_CUSTOMER_ARTWORK(@pDateFrom , @pDateTo , @pCustomerId)");
                        var parameter = new List<SqlParameter>
                        {
                            new SqlParameter
                            {
                                ParameterName = "@pDateFrom",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_FROM
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pDateTo",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_TO
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pCustomerId",
                                DbType = DbType.Int32,
                                Direction = ParameterDirection.Input,
                                Value = param.data.CUSTOMER_ID != null?param.data.CUSTOMER_ID:0
                            }
                        };
                        custArtworkList = context.Database.SqlQuery<CUST_ARTWORK_REPORT>(sqlCommand.ToString(), parameter.ToArray());

                        listReport = custArtworkList.OrderBy(c => c.Customer).ToList();
                        cnt = listReport.Count();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return listReport;
        }

        public static List<CUST_ARTWORK_REPORT> FilterDataCustomerArtworkView(CUST_ARTWORK_REPORT_REQUEST param, ref int cnt)
        {
            List<CUST_ARTWORK_REPORT> result = new List<CUST_ARTWORK_REPORT>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var sqlCommand = new StringBuilder(@"select * from dbo.UFN_CUSTOMER_ARTWORK(@pDateFrom , @pDateTo , @pCustomerId)");
                    var parameter = new List<SqlParameter>
                        {
                            new SqlParameter
                            {
                                ParameterName = "@pDateFrom",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_FROM
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pDateTo",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_TO
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pCustomerId",
                                DbType = DbType.Int32,
                                Direction = ParameterDirection.Input,
                                Value = param.data.CUSTOMER_ID != null?param.data.CUSTOMER_ID:0
                            }
                        };
                    result = context.Database.SqlQuery<CUST_ARTWORK_REPORT>(sqlCommand.ToString(), parameter.ToArray()).ToList();

                    if (param.columns[1] != null)
                    {
                        if (param.columns[1].search.value != null)
                        {
                            string filter = param.columns[1].search.value;
                            result = result.Where(m => m.Customer.ToLower().Contains(filter.ToLower())).ToList();
                        }
                    }

                    if (param.columns[2] != null)
                    {
                        if (param.columns[2].search.value != null)
                        {
                            if (ValidNumeric(param.columns[2].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[2].search.value);
                                result = result.Where(m => m.Total == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[3] != null)
                    {
                        if (param.columns[3].search.value != null)
                        {
                            if (ValidNumeric(param.columns[3].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[3].search.value);
                                result = result.Where(m => m.Approve_Artwork == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[4] != null)
                    {
                        if (param.columns[4].search.value != null)
                        {
                            if (ValidNumeric(param.columns[4].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[4].search.value);
                                result = result.Where(m => m.Approve_ShadeLimit == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[5] != null)
                    {
                        if (param.columns[5].search.value != null)
                        {
                            if (ValidNumeric(param.columns[5].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[5].search.value);
                                result = result.Where(m => m.Revise_ChangeOption == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[6] != null)
                    {
                        if (param.columns[6].search.value != null)
                        {
                            if (ValidNumeric(param.columns[6].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[6].search.value);
                                result = result.Where(m => m.Revise_WantToAdjust == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[7] != null)
                    {
                        if (param.columns[7].search.value != null)
                        {
                            if (ValidNumeric(param.columns[7].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[7].search.value);
                                result = result.Where(m => m.Revise_IncorrectArtwork == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[8] != null)
                    {
                        if (param.columns[8].search.value != null)
                        {
                            if (ValidNumeric(param.columns[8].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[8].search.value);
                                result = result.Where(m => m.Cancel == filter).ToList();
                            }
                        }
                    }

                    cnt = result.Count();
                    return OrderByCustomerArtwork(result, param);
                }
            }
        }

        public static List<CUST_ARTWORK_REPORT> OrderByCustomerArtwork(List<CUST_ARTWORK_REPORT> q, CUST_ARTWORK_REPORT_REQUEST param)
        {
            var customerArtwork = new List<CUST_ARTWORK_REPORT>();
            var orderColumn = 1;
            var orderDir = "asc";
            if (param.order != null && param.order.Count > 0)
            {
                orderColumn = param.order[0].column;
                orderDir = param.order[0].dir; //desc ,asc
            }

            string orderASC = "asc";
            string orderDESC = "desc";

            if (orderColumn == 1)
            {
                if (orderDir == orderASC)
                    customerArtwork = q.OrderBy(i => i.Customer).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Customer).ToList();
                else if (orderDir == orderDESC)
                    customerArtwork = q.OrderByDescending(i => i.Customer).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Customer).ToList();

            }
            else if (orderColumn == 2)
            {
                if (orderDir == orderASC)
                    customerArtwork = q.OrderBy(i => i.Total).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Total).ToList();
                else if (orderDir == orderDESC)
                    customerArtwork = q.OrderByDescending(i => i.Total).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Total).ToList();
            }
            else if (orderColumn == 3)
            {
                if (orderDir == orderASC)
                    customerArtwork = q.OrderBy(i => i.Approve_Artwork).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Approve_Artwork).ToList();
                else if (orderDir == orderDESC)
                    customerArtwork = q.OrderByDescending(i => i.Approve_Artwork).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Approve_Artwork).ToList();
            }
            else if (orderColumn == 4)
            {
                if (orderDir == orderASC)
                    customerArtwork = q.OrderBy(i => i.Approve_ShadeLimit).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Approve_ShadeLimit).ToList();
                else if (orderDir == orderDESC)
                    customerArtwork = q.OrderByDescending(i => i.Approve_ShadeLimit).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Approve_ShadeLimit).ToList();
            }
            else if (orderColumn == 5)
            {
                if (orderDir == orderASC)
                    customerArtwork = q.OrderBy(i => i.Revise_ChangeOption).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_ChangeOption).ToList();
                else if (orderDir == orderDESC)
                    customerArtwork = q.OrderByDescending(i => i.Revise_ChangeOption).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_ChangeOption).ToList();
            }
            else if (orderColumn == 6)
            {
                if (orderDir == orderASC)
                    customerArtwork = q.OrderBy(i => i.Revise_WantToAdjust).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_WantToAdjust).ToList();
                else if (orderDir == orderDESC)
                    customerArtwork = q.OrderByDescending(i => i.Revise_WantToAdjust).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_WantToAdjust).ToList();
            }
            else if (orderColumn == 7)
            {
                if (orderDir == orderASC)
                    customerArtwork = q.OrderBy(i => i.Revise_IncorrectArtwork).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_IncorrectArtwork).ToList();
                else if (orderDir == orderDESC)
                    customerArtwork = q.OrderByDescending(i => i.Revise_IncorrectArtwork).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_IncorrectArtwork).ToList();
            }
            else if (orderColumn == 8)
            {
                if (orderDir == orderASC)
                    customerArtwork = q.OrderBy(i => i.Cancel).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Cancel).ToList();
                else if (orderDir == orderDESC)
                    customerArtwork = q.OrderByDescending(i => i.Cancel).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Cancel).ToList();
            }
            return customerArtwork;
        }

        #endregion

        #region --------------------------------- customer mockup ---------------------------------
        public static CUST_MOCKUP_REPORT_RESULT GetCustomerMockupReport(CUST_MOCKUP_REPORT_REQUEST param)
        {
            CUST_MOCKUP_REPORT_RESULT Results = new CUST_MOCKUP_REPORT_RESULT();

            try
            {
                bool filter = false;
                if (param.columns != null)
                {
                    foreach (var item in param.columns)
                    {
                        if (!string.IsNullOrEmpty(item.search.value))
                        {
                            filter = true;
                            break;
                        }
                    }
                }

                var cnt = 0;
                var listResultAll = new List<CUST_MOCKUP_REPORT>();
                if (filter)
                {
                    listResultAll = FilterDataCustomerMockupView(param, ref cnt);
                }
                else
                {
                    listResultAll = QuerCustomerMockupReport(param, ref cnt);
                }
                Results.recordsTotal = cnt;
                Results.recordsFiltered = cnt;

                if (param.order != null && param.order.Count > 0)
                {
                    listResultAll = OrderByCustomerMockup(listResultAll, param);
                }

                Results.status = "S";
                Results.data = listResultAll;
                Results.draw = param.draw;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static List<CUST_MOCKUP_REPORT> QuerCustomerMockupReport(CUST_MOCKUP_REPORT_REQUEST param, ref int cnt)
        {
            List<CUST_MOCKUP_REPORT> listReport = new List<CUST_MOCKUP_REPORT>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 500;
                    IEnumerable<CUST_MOCKUP_REPORT> custMockupList = null;
                    try
                    {
                        var sqlCommand = new StringBuilder(@"select * from dbo.UFN_CUSTOMER_MOCKUP(@pDateFrom , @pDateTo , @pCustomerId)");
                        var parameter = new List<SqlParameter>
                        {
                            new SqlParameter
                            {
                                ParameterName = "@pDateFrom",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_FROM
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pDateTo",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_TO
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pCustomerId",
                                DbType = DbType.Int32,
                                Direction = ParameterDirection.Input,
                                Value = param.data.CUSTOMER_ID != null?param.data.CUSTOMER_ID:0
                            }
                        };

                        custMockupList = context.Database.SqlQuery<CUST_MOCKUP_REPORT>(sqlCommand.ToString(), parameter.ToArray());

                        listReport = custMockupList.OrderBy(c => c.CustomerCode).ToList();
                        cnt = listReport.Count();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return listReport;
        }

        public static List<CUST_MOCKUP_REPORT> FilterDataCustomerMockupView(CUST_MOCKUP_REPORT_REQUEST param, ref int cnt)
        {
            List<CUST_MOCKUP_REPORT> result = new List<CUST_MOCKUP_REPORT>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var sqlCommand = new StringBuilder(@"select * from dbo.UFN_CUSTOMER_MOCKUP(@pDateFrom , @pDateTo , @pCustomerId)");
                    var parameter = new List<SqlParameter>
                        {
                            new SqlParameter
                            {
                                ParameterName = "@pDateFrom",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_FROM
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pDateTo",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_TO
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pCustomerId",
                                DbType = DbType.Int32,
                                Direction = ParameterDirection.Input,
                                Value = param.data.CUSTOMER_ID != null?param.data.CUSTOMER_ID:0
                            }
                        };

                    result = context.Database.SqlQuery<CUST_MOCKUP_REPORT>(sqlCommand.ToString(), parameter.ToArray()).ToList();

                    if (param.columns[1] != null)
                    {
                        if (param.columns[1].search.value != null)
                        {
                            string filter = param.columns[1].search.value;
                            result = result.Where(m => m.CustomerCode.ToLower().Contains(filter.ToLower())).ToList();
                        }
                    }

                    if (param.columns[2] != null)
                    {
                        if (param.columns[2].search.value != null)
                        {
                            if (ValidNumeric(param.columns[2].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[2].search.value);
                                result = result.Where(m => m.Total == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[3] != null)
                    {
                        if (param.columns[3].search.value != null)
                        {
                            if (ValidNumeric(param.columns[3].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[3].search.value);
                                result = result.Where(m => m.ApproveDieLine_NoArtwork == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[4] != null)
                    {
                        if (param.columns[4].search.value != null)
                        {
                            if (ValidNumeric(param.columns[4].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[4].search.value);
                                result = result.Where(m => m.ApproveDieLine_Artwork == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[5] != null)
                    {
                        if (param.columns[5].search.value != null)
                        {
                            if (ValidNumeric(param.columns[5].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[5].search.value);
                                result = result.Where(m => m.ApprovePhysical_Mockup == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[6] != null)
                    {
                        if (param.columns[6].search.value != null)
                        {
                            if (ValidNumeric(param.columns[6].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[6].search.value);
                                result = result.Where(m => m.Revise_WanttoAdjust == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[7] != null)
                    {
                        if (param.columns[7].search.value != null)
                        {
                            if (ValidNumeric(param.columns[7].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[7].search.value);
                                result = result.Where(m => m.Revise_IncorrectMockup == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[8] != null)
                    {
                        if (param.columns[8].search.value != null)
                        {
                            if (ValidNumeric(param.columns[8].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[8].search.value);
                                result = result.Where(m => m.Cancel == filter).ToList();
                            }
                        }
                    }

                    cnt = result.Count();
                    return OrderByCustomerMockup(result, param);
                }
            }
        }

        public static List<CUST_MOCKUP_REPORT> OrderByCustomerMockup(List<CUST_MOCKUP_REPORT> q, CUST_MOCKUP_REPORT_REQUEST param)
        {
            var customerMockup = new List<CUST_MOCKUP_REPORT>();
            var orderColumn = 1;
            var orderDir = "asc";
            if (param.order != null && param.order.Count > 0)
            {
                orderColumn = param.order[0].column;
                orderDir = param.order[0].dir; //desc ,asc
            }

            string orderASC = "asc";
            string orderDESC = "desc";

            if (orderColumn == 1)
            {
                if (orderDir == orderASC)
                    customerMockup = q.OrderBy(i => i.CustomerCode).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.CustomerCode).ToList();
                else if (orderDir == orderDESC)
                    customerMockup = q.OrderByDescending(i => i.CustomerCode).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.CustomerCode).ToList();

            }
            else if (orderColumn == 2)
            {
                if (orderDir == orderASC)
                    customerMockup = q.OrderBy(i => i.Total).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Total).ToList();
                else if (orderDir == orderDESC)
                    customerMockup = q.OrderByDescending(i => i.Total).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Total).ToList();
            }
            else if (orderColumn == 3)
            {
                if (orderDir == orderASC)
                    customerMockup = q.OrderBy(i => i.ApproveDieLine_NoArtwork).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.ApproveDieLine_NoArtwork).ToList();
                else if (orderDir == orderDESC)
                    customerMockup = q.OrderByDescending(i => i.ApproveDieLine_NoArtwork).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.ApproveDieLine_NoArtwork).ToList();
            }
            else if (orderColumn == 4)
            {
                if (orderDir == orderASC)
                    customerMockup = q.OrderBy(i => i.ApproveDieLine_Artwork).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.ApproveDieLine_Artwork).ToList();
                else if (orderDir == orderDESC)
                    customerMockup = q.OrderByDescending(i => i.ApproveDieLine_Artwork).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.ApproveDieLine_Artwork).ToList();
            }
            else if (orderColumn == 5)
            {
                if (orderDir == orderASC)
                    customerMockup = q.OrderBy(i => i.ApprovePhysical_Mockup).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.ApprovePhysical_Mockup).ToList();
                else if (orderDir == orderDESC)
                    customerMockup = q.OrderByDescending(i => i.ApprovePhysical_Mockup).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.ApprovePhysical_Mockup).ToList();
            }
            else if (orderColumn == 6)
            {
                if (orderDir == orderASC)
                    customerMockup = q.OrderBy(i => i.Revise_WanttoAdjust).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_WanttoAdjust).ToList();
                else if (orderDir == orderDESC)
                    customerMockup = q.OrderByDescending(i => i.Revise_WanttoAdjust).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_WanttoAdjust).ToList();
            }
            else if (orderColumn == 7)
            {
                if (orderDir == orderASC)
                    customerMockup = q.OrderBy(i => i.Revise_IncorrectMockup).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_IncorrectMockup).ToList();
                else if (orderDir == orderDESC)
                    customerMockup = q.OrderByDescending(i => i.Revise_IncorrectMockup).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_IncorrectMockup).ToList();
            }
            else if (orderColumn == 8)
            {
                if (orderDir == orderASC)
                    customerMockup = q.OrderBy(i => i.Cancel).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Cancel).ToList();
                else if (orderDir == orderDESC)
                    customerMockup = q.OrderByDescending(i => i.Cancel).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Cancel).ToList();
            }
            return customerMockup;
        }

        #endregion

        #region --------------------------------- vendor artwork ---------------------------------
        public static VENDOR_ARTWORK_REPORT_RESULT GetVendorArtworkReport(VENDOR_ARTWORK_REPORT_REQUEST param)
        {
            VENDOR_ARTWORK_REPORT_RESULT Results = new VENDOR_ARTWORK_REPORT_RESULT();

            try
            {
                bool filter = false;
                if (param.columns != null)
                {
                    foreach (var item in param.columns)
                    {
                        if (!string.IsNullOrEmpty(item.search.value))
                        {
                            filter = true;
                            break;
                        }
                    }
                }

                var cnt = 0;
                var listResultAll = new List<VENDOR_ARTWORK_REPORT>();
                if (filter)
                {
                    listResultAll = FilterDataVendorArtworkView(param, ref cnt);
                }
                else
                {
                    listResultAll = QuerVendorArtworkReport(param, ref cnt);
                }
                Results.recordsTotal = cnt;
                Results.recordsFiltered = cnt;

                if (param.order != null && param.order.Count > 0)
                {
                    listResultAll = OrderByVendorArtwork(listResultAll, param);
                }

                Results.status = "S";
                Results.data = listResultAll;
                Results.draw = param.draw;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static List<VENDOR_ARTWORK_REPORT> QuerVendorArtworkReport(VENDOR_ARTWORK_REPORT_REQUEST param, ref int cnt)
        {
            List<VENDOR_ARTWORK_REPORT> listReport = new List<VENDOR_ARTWORK_REPORT>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 0;//500;
                    IEnumerable<VENDOR_ARTWORK_REPORT> vendorArtworkList = null;
                    try
                    {
                        var sqlCommand = new StringBuilder(@"select * from dbo.UFN_VENDOR_ARTWORK(@pDateFrom , @pDateTo , @pVendorId)");
                        var parameter = new List<SqlParameter>
                        {
                            new SqlParameter
                            {
                                ParameterName = "@pDateFrom",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_FROM
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pDateTo",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_TO
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pVendorId",
                                DbType = DbType.Int32,
                                Direction = ParameterDirection.Input,
                                Value = param.data.VENDOR_ID != null?param.data.VENDOR_ID:0
                            }
                        };

                        vendorArtworkList = context.Database.SqlQuery<VENDOR_ARTWORK_REPORT>(sqlCommand.ToString(), parameter.ToArray());

                        listReport = vendorArtworkList.OrderBy(v => v.Vendor).ToList();
                        cnt = listReport.Count();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return listReport;
        }

        public static List<VENDOR_ARTWORK_REPORT> FilterDataVendorArtworkView(VENDOR_ARTWORK_REPORT_REQUEST param, ref int cnt)
        {
            List<VENDOR_ARTWORK_REPORT> result = new List<VENDOR_ARTWORK_REPORT>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var sqlCommand = new StringBuilder(@"select * from dbo.UFN_VENDOR_ARTWORK(@pDateFrom , @pDateTo , @pVendorId)");
                    var parameter = new List<SqlParameter>
                        {
                            new SqlParameter
                            {
                                ParameterName = "@pDateFrom",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_FROM
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pDateTo",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_TO
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pVendorId",
                                DbType = DbType.Int32,
                                Direction = ParameterDirection.Input,
                                Value = param.data.VENDOR_ID != null?param.data.VENDOR_ID:0
                            }
                        };
                    result = context.Database.SqlQuery<VENDOR_ARTWORK_REPORT>(sqlCommand.ToString(), parameter.ToArray()).ToList();

                    if (param.columns[1] != null)
                    {
                        if (param.columns[1].search.value != null)
                        {
                            string filter = param.columns[1].search.value;
                            result = result.Where(m => m.Vendor.ToLower().Contains(filter.ToLower())).ToList();
                        }
                    }

                    if (param.columns[2] != null)
                    {
                        if (param.columns[2].search.value != null)
                        {
                            if (ValidNumeric(param.columns[2].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[2].search.value);
                                result = result.Where(m => m.Total == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[3] != null)
                    {
                        if (param.columns[3].search.value != null)
                        {
                            if (ValidNumeric(param.columns[3].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[3].search.value);
                                result = result.Where(m => m.Approve == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[4] != null)
                    {
                        if (param.columns[4].search.value != null)
                        {
                            if (ValidNumeric(param.columns[4].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[4].search.value);
                                result = result.Where(m => m.Revise_PA == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[5] != null)
                    {
                        if (param.columns[5].search.value != null)
                        {
                            if (ValidNumeric(param.columns[5].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[5].search.value);
                                result = result.Where(m => m.Revise_PG == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[6] != null)
                    {
                        if (param.columns[6].search.value != null)
                        {
                            if (ValidNumeric(param.columns[6].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[6].search.value);
                                result = result.Where(m => m.Revise_QC == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[7] != null)
                    {
                        if (param.columns[7].search.value != null)
                        {
                            if (ValidNumeric(param.columns[7].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[7].search.value);
                                result = result.Where(m => m.Revise_Customer == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[8] != null)
                    {
                        if (param.columns[8].search.value != null)
                        {
                            if (ValidNumeric(param.columns[8].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[8].search.value);
                                result = result.Where(m => m.Revise_Marketing == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[9] != null)
                    {
                        if (param.columns[9].search.value != null)
                        {
                            if (ValidNumeric(param.columns[9].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[9].search.value);
                                result = result.Where(m => m.Revise_Vendor == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[10] != null)
                    {
                        if (param.columns[10].search.value != null)
                        {
                            if (ValidNumeric(param.columns[10].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[10].search.value);
                                result = result.Where(m => m.Revise_Warehouse == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[11] != null)
                    {
                        if (param.columns[11].search.value != null)
                        {
                            if (ValidNumeric(param.columns[11].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[11].search.value);
                                result = result.Where(m => m.Revise_Planning == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[12] != null)
                    {
                        if (param.columns[12].search.value != null)
                        {
                            if (ValidNumeric(param.columns[12].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[12].search.value);
                                result = result.Where(m => m.Day_Send_Print_All == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[13] != null)
                    {
                        if (param.columns[13].search.value != null)
                        {
                            if (ValidNumeric(param.columns[13].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[13].search.value);
                                result = result.Where(m => m.Day_Send_Print_Ontime == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[14] != null)
                    {
                        if (param.columns[14].search.value != null)
                        {
                            if (ValidNumeric(param.columns[14].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[14].search.value);
                                result = result.Where(m => m.Day_Confirm_PO_All == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[15] != null)
                    {
                        if (param.columns[15].search.value != null)
                        {
                            if (ValidNumeric(param.columns[15].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[15].search.value);
                                result = result.Where(m => m.Day_Confirm_PO_Ontime == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[16] != null)
                    {
                        if (param.columns[16].search.value != null)
                        {
                            if (ValidNumeric(param.columns[16].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[16].search.value);
                                result = result.Where(m => m.Day_Send_Shade_All == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[17] != null)
                    {
                        if (param.columns[17].search.value != null)
                        {
                            if (ValidNumeric(param.columns[17].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[17].search.value);
                                result = result.Where(m => m.Day_Send_Shade_Ontime == filter).ToList();
                            }
                        }
                    }

                    cnt = result.ToList().Count();
                    return OrderByVendorArtwork(result.ToList(), param);
                }
            }
        }

        public static List<VENDOR_ARTWORK_REPORT> OrderByVendorArtwork(List<VENDOR_ARTWORK_REPORT> q, VENDOR_ARTWORK_REPORT_REQUEST param)
        {
            var vendorArtwork = new List<VENDOR_ARTWORK_REPORT>();
            var orderColumn = 1;
            var orderDir = "asc";
            if (param.order != null && param.order.Count > 0)
            {
                orderColumn = param.order[0].column;
                orderDir = param.order[0].dir; //desc ,asc
            }

            string orderASC = "asc";
            string orderDESC = "desc";

            if (orderColumn == 1)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Vendor).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Vendor).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Vendor).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Vendor).ToList();

            }
            else if (orderColumn == 2)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Total).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Total).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Total).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Total).ToList();
            }
            else if (orderColumn == 3)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Approve).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Approve).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Approve).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Approve).ToList();
            }
            else if (orderColumn == 4)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Revise_PA).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_PA).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Revise_PA).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_PA).ToList();
            }
            else if (orderColumn == 5)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Revise_PG).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_PG).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Revise_PG).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_PG).ToList();
            }
            else if (orderColumn == 6)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Revise_QC).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_QC).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Revise_QC).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_QC).ToList();
            }
            else if (orderColumn == 7)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Revise_Customer).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_Customer).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Revise_Customer).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_Customer).ToList();
            }
            else if (orderColumn == 8)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Revise_Marketing).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_Marketing).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Revise_Marketing).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_Marketing).ToList();
            }
            else if (orderColumn == 9)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Revise_Vendor).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_Vendor).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Revise_Vendor).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_Vendor).ToList();
            }
            else if (orderColumn == 10)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Revise_Warehouse).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_Warehouse).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Revise_Warehouse).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_Warehouse).ToList();
            }
            else if (orderColumn == 11)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Revise_Planning).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_Planning).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Revise_Planning).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_Planning).ToList();
            }
            else if (orderColumn == 12)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Day_Send_Print_All).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Day_Send_Print_All).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Day_Send_Print_All).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Day_Send_Print_All).ToList();
            }
            else if (orderColumn == 13)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Day_Send_Print_Ontime).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Day_Send_Print_Ontime).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Day_Send_Print_Ontime).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Day_Send_Print_Ontime).ToList();
            }
            else if (orderColumn == 14)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Day_Confirm_PO_All).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Day_Confirm_PO_All).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Day_Confirm_PO_All).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Day_Confirm_PO_All).ToList();
            }
            else if (orderColumn == 15)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Day_Confirm_PO_Ontime).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Day_Confirm_PO_Ontime).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Day_Confirm_PO_Ontime).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Day_Confirm_PO_Ontime).ToList();
            }
            else if (orderColumn == 16)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Day_Send_Shade_All).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Day_Send_Shade_All).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Day_Send_Shade_All).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Day_Send_Shade_All).ToList();
            }
            else if (orderColumn == 17)
            {
                if (orderDir == orderASC)
                    vendorArtwork = q.OrderBy(i => i.Day_Send_Shade_Ontime).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Day_Send_Shade_Ontime).ToList();
                else if (orderDir == orderDESC)
                    vendorArtwork = q.OrderByDescending(i => i.Day_Send_Shade_Ontime).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Day_Send_Shade_Ontime).ToList();
            }
            return vendorArtwork;
        }

        #endregion

        #region --------------------------------- vendor mockup ---------------------------------
        public static VENDOR_MOCKUP_REPORT_RESULT GetVendorMockupReport(VENDOR_MOCKUP_REPORT_REQUEST param)
        {
            VENDOR_MOCKUP_REPORT_RESULT Results = new VENDOR_MOCKUP_REPORT_RESULT();

            try
            {
                bool filter = false;
                if (param.columns != null)
                {
                    foreach (var item in param.columns)
                    {
                        if (!string.IsNullOrEmpty(item.search.value))
                        {
                            filter = true;
                            break;
                        }
                    }
                }

                var cnt = 0;
                var listResultAll = new List<VENDOR_MOCKUP_REPORT>();
                if (filter)
                {
                    listResultAll = FilterDataVendorMockupView(param, ref cnt);
                }
                else
                {
                    listResultAll = QuerVendorMockupReport(param, ref cnt);
                }
                Results.recordsTotal = cnt;
                Results.recordsFiltered = cnt;

                if (param.order != null && param.order.Count > 0)
                {
                    listResultAll = OrderByVendorMockup(listResultAll, param);
                }

                Results.status = "S";
                Results.data = listResultAll;
                Results.draw = param.draw;
            }
            catch (Exception ex)
            {
                Results.status = "E";
                Results.msg = CNService.GetErrorMessage(ex);
            }

            return Results;
        }

        public static List<VENDOR_MOCKUP_REPORT> QuerVendorMockupReport(VENDOR_MOCKUP_REPORT_REQUEST param, ref int cnt)
        {
            List<VENDOR_MOCKUP_REPORT> listReport = new List<VENDOR_MOCKUP_REPORT>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    context.Database.CommandTimeout = 500;
                    IEnumerable<VENDOR_MOCKUP_REPORT> vendorMockupList = null;
                    try
                    {
                        var sqlCommand = new StringBuilder(@"select * from dbo.UFN_VENDOR_MOCKUP(@pDateFrom , @pDateTo , @pVendorId)");
                        var parameter = new List<SqlParameter>
                        {
                            new SqlParameter
                            {
                                ParameterName = "@pDateFrom",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_FROM
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pDateTo",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_TO
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pVendorId",
                                DbType = DbType.Int32,
                                Direction = ParameterDirection.Input,
                                Value = param.data.VENDOR_ID != null?param.data.VENDOR_ID:0
                            }
                        };
                        vendorMockupList = context.Database.SqlQuery<VENDOR_MOCKUP_REPORT>(sqlCommand.ToString(), parameter.ToArray());

                        listReport = vendorMockupList.OrderBy(v => v.Vendor).ToList();
                        cnt = listReport.Count();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }

            return listReport;
        }

        public static List<VENDOR_MOCKUP_REPORT> FilterDataVendorMockupView(VENDOR_MOCKUP_REPORT_REQUEST param, ref int cnt)
        {
            List<VENDOR_MOCKUP_REPORT> result = new List<VENDOR_MOCKUP_REPORT>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var sqlCommand = new StringBuilder(@"select * from dbo.UFN_VENDOR_MOCKUP(@pDateFrom , @pDateTo , @pVendorId)");
                    var parameter = new List<SqlParameter>
                        {
                            new SqlParameter
                            {
                                ParameterName = "@pDateFrom",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_FROM
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pDateTo",
                                DbType = DbType.DateTime,
                                Direction = ParameterDirection.Input,
                                Value = param.data.DATE_TO
                            },
                            new SqlParameter
                            {
                                ParameterName = "@pVendorId",
                                DbType = DbType.Int32,
                                Direction = ParameterDirection.Input,
                                Value = param.data.VENDOR_ID != null?param.data.VENDOR_ID:0
                            }
                        };
                    result = context.Database.SqlQuery<VENDOR_MOCKUP_REPORT>(sqlCommand.ToString(), parameter.ToArray()).ToList();

                    if (param.columns[1] != null)
                    {
                        if (param.columns[1].search.value != null)
                        {
                            string filter = param.columns[1].search.value;
                            result = result.Where(m => m.Vendor.ToLower().Contains(filter.ToLower())).ToList();
                        }
                    }

                    if (param.columns[2] != null)
                    {
                        if (param.columns[2].search.value != null)
                        {
                            if (ValidNumeric(param.columns[2].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[2].search.value);
                                result = result.Where(m => m.Total == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[3] != null)
                    {
                        if (param.columns[3].search.value != null)
                        {
                            if (ValidNumeric(param.columns[3].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[3].search.value);
                                result = result.Where(m => m.Approve == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[4] != null)
                    {
                        if (param.columns[4].search.value != null)
                        {
                            if (ValidNumeric(param.columns[4].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[4].search.value);
                                result = result.Where(m => m.Revise_Vendor == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[5] != null)
                    {
                        if (param.columns[5].search.value != null)
                        {
                            if (ValidNumeric(param.columns[5].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[5].search.value);
                                result = result.Where(m => m.Revise_PG == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[6] != null)
                    {
                        if (param.columns[6].search.value != null)
                        {
                            if (ValidNumeric(param.columns[6].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[6].search.value);
                                result = result.Where(m => m.Revise_Customer == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[7] != null)
                    {
                        if (param.columns[7].search.value != null)
                        {
                            if (ValidNumeric(param.columns[7].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[7].search.value);
                                result = result.Where(m => m.Revise_Warehouse == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[8] != null)
                    {
                        if (param.columns[8].search.value != null)
                        {
                            if (ValidNumeric(param.columns[8].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[8].search.value);
                                result = result.Where(m => m.Revise_Marketing == filter).ToList();
                            }
                        }
                    }
                    /*----------------------------------------------------------------------------*/

                    if (param.columns[9] != null)
                    {
                        if (param.columns[9].search.value != null)
                        {
                            if (ValidNumeric(param.columns[9].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[9].search.value);
                                result = result.Where(m => m.Revise_Planning == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[10] != null)
                    {
                        if (param.columns[10].search.value != null)
                        {
                            if (ValidNumeric(param.columns[10].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[10].search.value);
                                result = result.Where(m => m.Revise_RD == filter).ToList();
                            }
                        }
                    }

                    /*----------------------------------------------------------------------------*/

                    if (param.columns[11] != null)
                    {
                        if (param.columns[11].search.value != null)
                        {
                            if (ValidNumeric(param.columns[11].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[11].search.value);
                                result = result.Where(m => m.Day_Quotations_All == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[12] != null)
                    {
                        if (param.columns[12].search.value != null)
                        {
                            if (ValidNumeric(param.columns[12].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[12].search.value);
                                result = result.Where(m => m.Day_Quotations_Ontime == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[13] != null)
                    {
                        if (param.columns[13].search.value != null)
                        {
                            if (ValidNumeric(param.columns[13].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[13].search.value);
                                result = result.Where(m => m.Day_Mockup_All == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[14] != null)
                    {
                        if (param.columns[14].search.value != null)
                        {
                            if (ValidNumeric(param.columns[14].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[14].search.value);
                                result = result.Where(m => m.Day_Mockup_Ontime == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[15] != null)
                    {
                        if (param.columns[15].search.value != null)
                        {
                            if (ValidNumeric(param.columns[15].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[15].search.value);
                                result = result.Where(m => m.Day_Dieline_All == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[16] != null)
                    {
                        if (param.columns[16].search.value != null)
                        {
                            if (ValidNumeric(param.columns[16].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[16].search.value);
                                result = result.Where(m => m.Day_Dieline_Ontime == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[17] != null)
                    {
                        if (param.columns[17].search.value != null)
                        {
                            if (ValidNumeric(param.columns[17].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[17].search.value);
                                result = result.Where(m => m.Day_Matchboard_All == filter).ToList();
                            }
                        }
                    }

                    if (param.columns[18] != null)
                    {
                        if (param.columns[18].search.value != null)
                        {
                            if (ValidNumeric(param.columns[18].search.value))
                            {
                                int filter = Convert.ToInt32(param.columns[18].search.value);
                                result = result.Where(m => m.Day_Matchboard_Ontime == filter).ToList();
                            }
                        }
                    }

                    cnt = result.Count();
                    return OrderByVendorMockup(result, param);
                }
            }
        }

        public static List<VENDOR_MOCKUP_REPORT> OrderByVendorMockup(List<VENDOR_MOCKUP_REPORT> q, VENDOR_MOCKUP_REPORT_REQUEST param)
        {
            var vendorMockup = new List<VENDOR_MOCKUP_REPORT>();
            var orderColumn = 1;
            var orderDir = "asc";
            if (param.order != null && param.order.Count > 0)
            {
                orderColumn = param.order[0].column;
                orderDir = param.order[0].dir; //desc ,asc
            }

            string orderASC = "asc";
            string orderDESC = "desc";

            if (orderColumn == 1)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Vendor).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Vendor).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Vendor).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Vendor).ToList();

            }
            else if (orderColumn == 2)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Total).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Total).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Total).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Total).ToList();
            }
            else if (orderColumn == 3)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Approve).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Approve).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Approve).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Approve).ToList();
            }
            else if (orderColumn == 4)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Revise_Vendor).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_Vendor).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Revise_Vendor).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_Vendor).ToList();
            }
            else if (orderColumn == 5)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Revise_PG).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_PG).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Revise_PG).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_PG).ToList();
            }
            else if (orderColumn == 6)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Revise_Customer).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_Customer).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Revise_Customer).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_Customer).ToList();
            }
            else if (orderColumn == 7)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Revise_Warehouse).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_Warehouse).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Revise_Warehouse).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_Warehouse).ToList();
            }
            else if (orderColumn == 8)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Revise_Marketing).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_Marketing).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Revise_Marketing).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_Marketing).ToList();
            }
            else if (orderColumn == 9)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Revise_Planning).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_Planning).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Revise_Planning).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_Planning).ToList();
            }
            else if (orderColumn == 10)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Revise_RD).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Revise_RD).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Revise_RD).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Revise_RD).ToList();
            }
            else if (orderColumn == 11)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Day_Quotations_All).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Day_Quotations_All).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Day_Quotations_All).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Day_Quotations_All).ToList();
            }
            else if (orderColumn == 12)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Day_Quotations_Ontime).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Day_Quotations_Ontime).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Day_Quotations_Ontime).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Day_Quotations_Ontime).ToList();
            }
            else if (orderColumn == 13)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Day_Mockup_All).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Day_Mockup_All).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Day_Mockup_All).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Day_Mockup_All).ToList();
            }
            else if (orderColumn == 14)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Day_Mockup_Ontime).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Day_Mockup_Ontime).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Day_Mockup_Ontime).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Day_Mockup_Ontime).ToList();
            }
            else if (orderColumn == 15)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Day_Dieline_All).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Day_Dieline_All).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Day_Dieline_All).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Day_Dieline_All).ToList();
            }
            else if (orderColumn == 16)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Day_Dieline_Ontime).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Day_Dieline_Ontime).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Day_Dieline_Ontime).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Day_Dieline_Ontime).ToList();
            }
            else if (orderColumn == 17)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Day_Matchboard_All).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Day_Matchboard_All).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Day_Matchboard_All).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Day_Matchboard_All).ToList();
            }
            else if (orderColumn == 18)
            {
                if (orderDir == orderASC)
                    vendorMockup = q.OrderBy(i => i.Day_Matchboard_Ontime).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.Day_Matchboard_Ontime).ToList();
                else if (orderDir == orderDESC)
                    vendorMockup = q.OrderByDescending(i => i.Day_Matchboard_Ontime).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.Day_Matchboard_Ontime).ToList();
            }
            return vendorMockup;
        }

        #endregion

        public static bool ValidNumeric(string input)
        {
            int test;
            return int.TryParse(input, out test);
        }
    }
}



