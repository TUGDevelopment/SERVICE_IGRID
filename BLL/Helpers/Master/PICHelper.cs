using BLL.Services;
using DAL;
using DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Helpers.Master
{
    public class PICHelper
    {
        private static bool CheckIsFilter(ART_M_PIC_REQUEST param)
        {
            foreach (var item in param.columns)
            {
                if (!String.IsNullOrEmpty(item.search.value))
                {
                    return true;
                }
            }


            return false;
        }

        public static ART_M_PIC_RESULT GetPIC(ART_M_PIC_REQUEST param)
        {
            ART_M_PIC_RESULT Results = new ART_M_PIC_RESULT();
            Results.data = new List<ART_M_PIC_2>();

            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }

                var cnt = 0;
                Results.data = QueryPIC(param, ref cnt);
                Results.recordsTotal = cnt;
                Results.recordsFiltered = cnt;

                bool is_filter = false;

                if (param.columns != null)
                {
                    is_filter = CheckIsFilter(param);
                    if (is_filter)
                    {
                        Results.data = FilterPICData(Results.data, param, ref cnt);
                        Results.recordsTotal = cnt;
                        Results.recordsFiltered = cnt;
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

        public static ART_M_PIC_RESULT GetPICEdit(ART_M_PIC_REQUEST param)
        {
            ART_M_PIC_RESULT Results = new ART_M_PIC_RESULT();

            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }

                if (param.data.PIC_ID > 0)
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            var listZone = GetListZone();

                            var pic = (from m in context.ART_M_PIC
                                       join u0 in context.ART_M_USER on m.USER_ID equals u0.USER_ID into uTmp
                                       from u in uTmp.DefaultIfEmpty()
                                       join sold0 in context.XECM_M_CUSTOMER on m.SOLD_TO_CODE equals sold0.CUSTOMER_CODE into soldTmp
                                       from sold in soldTmp.DefaultIfEmpty()
                                       join ship0 in context.XECM_M_CUSTOMER on m.SHIP_TO_CODE equals ship0.CUSTOMER_CODE into shipTmp
                                       from ship in shipTmp.DefaultIfEmpty()
                                       join cc in context.SAP_M_COUNTRY on m.COUNTRY equals cc.COUNTRY_CODE into ct
                                       from c in ct.DefaultIfEmpty()
                                       where m.PIC_ID == param.data.PIC_ID
                                       select new ART_M_PIC_2
                                       {
                                           PIC_ID = m.PIC_ID,
                                           USER_ID = u.USER_ID > 0 ? u.USER_ID : 0,
                                           USER_DISPLAY_TXT = u.USERNAME,
                                           FIRST_NAME_DISPLAY_TXT = u.FIRST_NAME,
                                           LAST_NAME_DISPLAY_TXT = u.LAST_NAME,
                                           SOLD_TO_ID = sold.CUSTOMER_ID > 0 ? sold.CUSTOMER_ID : 0,
                                           SOLD_TO_CODE = m.SOLD_TO_CODE,
                                           SOLD_TO_DISPLAY_TXT = sold.CUSTOMER_NAME,
                                           SHIP_TO_ID = ship.CUSTOMER_ID > 0 ? ship.CUSTOMER_ID : 0,
                                           SHIP_TO_CODE = m.SHIP_TO_CODE,
                                           SHIP_TO_DISPLAY_TXT = ship.CUSTOMER_NAME,
                                           ZONE = m.ZONE,
                                           COUNTRY_ID = c.COUNTRY_ID > 0 ? c.COUNTRY_ID : 0,
                                           COUNTRY = m.COUNTRY,
                                           COUNTRY_DISPLAY_TXT = c.NAME
                                       }).ToList();



                            if (pic != null)
                            {
                                foreach (var itemPIC in pic)
                                {
                                    var zoneIDTmp = listZone.Where(w => w.ZONE == itemPIC.ZONE).Select(s => s.ID).FirstOrDefault();
                                    itemPIC.ZONE_ID = zoneIDTmp;
                                }


                                Results.data = pic;
                            }

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

        public static ART_M_PIC_RESULT SavePICEdit(ART_M_PIC_REQUEST param)
        {
            ART_M_PIC_RESULT Results = new ART_M_PIC_RESULT();

            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }

                //if (param.data.SOLD_TO_ID <= 0 || param.data.SHIP_TO_ID <= 0 || param.data.ZONE_ID <= 0)
                //{
                //    string msg = "";

                //    if (param.data.SOLD_TO_ID <= 0)
                //    { msg += "Sold-to, "; }

                //    if (param.data.SHIP_TO_ID <= 0)
                //    { msg += "Ship-to, "; }

                //    if (param.data.ZONE_ID <= 0)
                //    { msg += "Zone, "; }

                //    msg = msg.Substring(0, msg.Length - 2);

                //    Results.status = "E";
                //    Results.msg = msg + " is Empty.";
                //    return Results;
                //}

                if (param.data.PIC_ID > 0)
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (var dbContextTransaction = CNService.IsolationLevel(context))
                        {
                            ART_M_PIC picEdit = new ART_M_PIC();
                            picEdit = MapperServices.ART_M_PIC(param.data);

                            if (param.data.SOLD_TO_ID > 0)
                            {
                                var sold = context.XECM_M_CUSTOMER.Where(w => w.CUSTOMER_ID == param.data.SOLD_TO_ID).FirstOrDefault();
                                if (sold != null)
                                {
                                    picEdit.SOLD_TO_CODE = sold.CUSTOMER_CODE;
                                }
                            }

                            if (param.data.SHIP_TO_ID > 0)
                            {
                                var ship = context.XECM_M_CUSTOMER.Where(w => w.CUSTOMER_ID == param.data.SHIP_TO_ID).FirstOrDefault();
                                if (ship != null)
                                {
                                    picEdit.SHIP_TO_CODE = ship.CUSTOMER_CODE;
                                }
                            }

                            if (param.data.COUNTRY_ID > 0)
                            {
                                var country = context.SAP_M_COUNTRY.Where(w => w.COUNTRY_ID == param.data.COUNTRY_ID).FirstOrDefault();
                                if (country != null)
                                {
                                    picEdit.COUNTRY = country.COUNTRY_CODE;
                                }
                            }

                            if (param.data.ZONE_ID > 0)
                            {
                                var listZone = GetListZone();

                                if (listZone != null)
                                {
                                    var zone = listZone.Where(w => w.ID == param.data.ZONE_ID).Select(s => s.ZONE).FirstOrDefault();

                                    if (!String.IsNullOrEmpty(zone))
                                    {
                                        picEdit.ZONE = zone;
                                    }
                                }

                            }

                            var picExist = (from p in context.ART_M_PIC
                                            where p.PIC_ID == param.data.PIC_ID
                                            select p).FirstOrDefault();

                            if (picExist != null)
                            {
                                picEdit.PIC_ID = picExist.PIC_ID;
                            }

                            picEdit.UPDATE_BY = param.data.UPDATE_BY;
                            picEdit.IS_ACTIVE = "X";
                        
                            ART_M_PIC_SERVICE.SaveOrUpdate(picEdit, context);
                            dbContextTransaction.Commit();
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

        public static ART_M_PIC_RESULT DeletePICEdit(ART_M_PIC_REQUEST param)
        {
            ART_M_PIC_RESULT Results = new ART_M_PIC_RESULT();

            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }

                if (param.data.PIC_ID > 0)
                {
                    using (var context = new ARTWORKEntities())
                    {
                        using (var dbContextTransaction = CNService.IsolationLevel(context))
                        {
                            ART_M_PIC_SERVICE.DeleteByPIC_ID(param.data.PIC_ID, context);
                            dbContextTransaction.Commit();
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
        public static ART_M_PIC_RESULT DeletePICList(ART_M_PIC_REQUEST_LIST param)
        {
            ART_M_PIC_RESULT Results = new ART_M_PIC_RESULT();

            try
            {
                if (param == null || param.data == null)
                {
                    return Results;
                }

                using (var context = new ARTWORKEntities())
                {
                    using (var dbContextTransaction = CNService.IsolationLevel(context))
                    {
                        foreach (var item in param.data)
                        {
                            ART_M_PIC_SERVICE.DeleteByPIC_ID(item.PIC_ID, context);
                        }
                        dbContextTransaction.Commit();
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

        public static List<ART_M_PIC_2> QueryPIC(ART_M_PIC_REQUEST param, ref int cnt) //
        {
            //bool is_filter = false;
            List<ART_M_PIC_2> listPIC = new List<ART_M_PIC_2>();
            List<int> listSoldTo = new List<int>();
            List<int> listShipTo = new List<int>();
            List<int> listCountry = new List<int>();
            List<int> listUser = new List<int>();
            List<string> listZone = new List<string>();

            //string exportExcel = "";

            if (!String.IsNullOrEmpty(param.data.LIST_SOLD_TO))
            {
                var listSold = param.data.LIST_SOLD_TO.Split(',').ToList();
                foreach (var itemsold in listSold)
                {
                    if (!String.IsNullOrEmpty(itemsold))
                    {
                        listSoldTo.Add(Int32.Parse(itemsold));
                    }
                }
            }
            if (!String.IsNullOrEmpty(param.data.LIST_SHIP_TO))
            {
                var listShip = param.data.LIST_SHIP_TO.Split(',').ToList();
                foreach (var itemship in listShip)
                {
                    if (!String.IsNullOrEmpty(itemship))
                    {
                        listShipTo.Add(Int32.Parse(itemship));
                    }
                }
            }
            if (!String.IsNullOrEmpty(param.data.LIST_ZONE))
            {
                var listZoneTmp = param.data.LIST_ZONE.Split(',').ToList();
                foreach (var itemzone in listZoneTmp)
                {
                    if (!String.IsNullOrEmpty(itemzone))
                    {
                        listZone.Add(itemzone);
                    }
                }
            }
            if (!String.IsNullOrEmpty(param.data.LIST_COUNTRY))
            {
                var listCountryTmp = param.data.LIST_COUNTRY.Split(',').ToList();
                foreach (var itemcountry in listCountryTmp)
                {
                    if (!String.IsNullOrEmpty(itemcountry))
                    {
                        listCountry.Add(Int32.Parse(itemcountry));
                    }
                }
            }
            if (!String.IsNullOrEmpty(param.data.LIST_PERSON))
            {
                var listPersonTmp = param.data.LIST_PERSON.Split(',').ToList();
                foreach (var itemperson in listPersonTmp)
                {
                    if (!String.IsNullOrEmpty(itemperson))
                    {
                        listUser.Add(Int32.Parse(itemperson));
                    }
                }
            }


            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    var q = (from m in context.ART_M_PIC
                             join u0 in context.ART_M_USER on m.USER_ID equals u0.USER_ID into uTmp
                             from u in uTmp.DefaultIfEmpty()
                             join sold0 in context.XECM_M_CUSTOMER on m.SOLD_TO_CODE equals sold0.CUSTOMER_CODE into soldTmp
                             from sold in soldTmp.DefaultIfEmpty()
                             join ship0 in context.XECM_M_CUSTOMER on m.SHIP_TO_CODE equals ship0.CUSTOMER_CODE into shipTmp
                             from ship in shipTmp.DefaultIfEmpty()
                             join cc in context.SAP_M_COUNTRY on m.COUNTRY equals cc.COUNTRY_CODE into ct
                             from c in ct.DefaultIfEmpty()
                             select new ART_M_PIC_2
                             {
                                 PIC_ID = m.PIC_ID,
                                 USER_ID = u.USER_ID > 0 ? u.USER_ID : 0,
                                 USER_DISPLAY_TXT = u.USERNAME,
                                 FIRST_NAME = u.FIRST_NAME,
                                 FIRST_NAME_DISPLAY_TXT = u.FIRST_NAME,
                                 LAST_NAME_DISPLAY_TXT = u.LAST_NAME,
                                 SOLD_TO_ID = sold.CUSTOMER_ID > 0 ? sold.CUSTOMER_ID : 0,
                                 SOLD_TO_CODE = m.SOLD_TO_CODE,
                                 SOLD_TO_DISPLAY_TXT = sold.CUSTOMER_NAME,
                                 SHIP_TO_ID = ship.CUSTOMER_ID > 0 ? ship.CUSTOMER_ID : 0,
                                 SHIP_TO_CODE = m.SHIP_TO_CODE,
                                 SHIP_TO_DISPLAY_TXT = ship.CUSTOMER_NAME,
                                 ZONE = m.ZONE,
                                 COUNTRY_ID = c.COUNTRY_ID > 0 ? c.COUNTRY_ID : 0,
                                 COUNTRY = m.COUNTRY,
                                 COUNTRY_DISPLAY_TXT = c.NAME
                             });

                    if (listSoldTo != null && listSoldTo.Count > 0)
                    {
                        q = (from r in q where listSoldTo.Contains(r.SOLD_TO_ID) select r);
                    }

                    if (listShipTo != null && listShipTo.Count > 0)
                    {
                        q = (from r in q where listShipTo.Contains(r.SHIP_TO_ID) select r);
                    }

                    if (listZone != null && listZone.Count > 0)
                    {
                        q = (from r in q where listZone.Contains(r.ZONE) select r);
                    }

                    if (listCountry != null && listCountry.Count > 0)
                    {
                        q = (from r in q where listCountry.Contains(r.COUNTRY_ID) select r);
                    }

                    if (listUser != null && listUser.Count > 0)
                    {
                        q = (from r in q where listUser.Contains(r.USER_ID.Value) select r);
                    }

                    //is_filter = CheckIsFilter(param);
                    //if (is_filter)
                    //{
                    //    q = FilterPICData2(q, param);
                    //}

                    cnt = q.Distinct().Count();

                    return OrderByPIC(q, param);

                    //listPIC = q.OrderBy(s => s.SOLD_TO_CODE)
                    //            .ThenBy(s => s.SHIP_TO_CODE)
                    //            .ThenBy(s => s.ZONE)
                    //            .ThenBy(s => s.USER_ID)
                    //            .ThenBy(s => s.COUNTRY).ToList();
                }
            }


        }

        public static List<ART_M_PIC_2> OrderByPIC(IQueryable<ART_M_PIC_2> q, ART_M_PIC_REQUEST param)
        {
            var Warehouse = new List<ART_M_PIC_2>();
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
                if (param.data.EXPORT_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.USER_DISPLAY_TXT).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.USER_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.USER_DISPLAY_TXT).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.USER_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.USER_DISPLAY_TXT).ToList();
                }
            }
            if (orderColumn == 2)
            {
                if (param.data.EXPORT_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.FIRST_NAME_DISPLAY_TXT).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.FIRST_NAME_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.FIRST_NAME_DISPLAY_TXT).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.FIRST_NAME_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.FIRST_NAME_DISPLAY_TXT).ToList();
                }
            }
            if (orderColumn == 3)
            {
                if (param.data.EXPORT_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.LAST_NAME_DISPLAY_TXT).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.LAST_NAME_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.LAST_NAME_DISPLAY_TXT).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.LAST_NAME_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.LAST_NAME_DISPLAY_TXT).ToList();
                }
            }
            else if (orderColumn == 4)
            {
                if (param.data.EXPORT_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.SOLD_TO_CODE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.SOLD_TO_CODE).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.SOLD_TO_CODE).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.SOLD_TO_CODE).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.SOLD_TO_CODE).ToList();
                }
            }
            else if (orderColumn == 5)
            {
                if (param.data.EXPORT_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.SOLD_TO_DISPLAY_TXT).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.SOLD_TO_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.SOLD_TO_DISPLAY_TXT).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.SOLD_TO_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.SOLD_TO_DISPLAY_TXT).ToList();
                }
            }
            else if (orderColumn == 6)
            {
                if (param.data.EXPORT_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.SHIP_TO_CODE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.SHIP_TO_CODE).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.SHIP_TO_CODE).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.SHIP_TO_CODE).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.SHIP_TO_CODE).ToList();
                }
            }
            else if (orderColumn == 7)
            {
                if (param.data.EXPORT_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.SHIP_TO_DISPLAY_TXT).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.SHIP_TO_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.SHIP_TO_DISPLAY_TXT).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.SHIP_TO_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.SHIP_TO_DISPLAY_TXT).ToList();
                }
            }
            else if (orderColumn == 8)
            {
                if (param.data.EXPORT_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.ZONE).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.ZONE).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.ZONE).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.ZONE).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.ZONE).ToList();
                }
            }
            else if (orderColumn == 9)
            {
                if (param.data.EXPORT_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.COUNTRY).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.COUNTRY).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.COUNTRY).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.COUNTRY).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.COUNTRY).ToList();
                }
            }
            else if (orderColumn == 10)
            {
                if (param.data.EXPORT_EXCEL == "X")
                {
                    Warehouse = q.Distinct().OrderBy(i => i.COUNTRY_DISPLAY_TXT).ToList();
                }
                else
                {
                    if (orderDir == orderASC)
                        Warehouse = q.OrderBy(i => i.COUNTRY_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().OrderBy(i => i.COUNTRY_DISPLAY_TXT).ToList();
                    else if (orderDir == orderDESC)
                        Warehouse = q.OrderByDescending(i => i.COUNTRY_DISPLAY_TXT).Skip(param.start).Take(param.length).Distinct().OrderByDescending(i => i.COUNTRY_DISPLAY_TXT).ToList();
                }
            }


            return Warehouse;
        }

        private static List<ART_M_PIC_2> FilterPICData(List<ART_M_PIC_2> data, ART_M_PIC_REQUEST param, ref int cnt)
        {
            var filterValue = "";
            List<ART_M_PIC_2> dataResults = new List<ART_M_PIC_2>();

            foreach (var item in param.columns)
            {

                switch (item.data)
                {
                    case "USER_DISPLAY_TXT":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            dataResults = data.Where(m => !string.IsNullOrEmpty(m.USER_DISPLAY_TXT) && m.USER_DISPLAY_TXT.ToUpper().Contains(filterValue.ToUpper())).ToList();
                        }
                        break;

                    case "FIRST_NAME_DISPLAY_TXT":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            dataResults = data.Where(m => !string.IsNullOrEmpty(m.FIRST_NAME_DISPLAY_TXT) && m.FIRST_NAME_DISPLAY_TXT.ToUpper().Contains(filterValue.ToUpper())).ToList();
                        }
                        break;

                    case "LAST_NAME_DISPLAY_TXT":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            dataResults = data.Where(m => !string.IsNullOrEmpty(m.LAST_NAME_DISPLAY_TXT) && m.LAST_NAME_DISPLAY_TXT.ToUpper().Contains(filterValue.ToUpper())).ToList();
                        }
                        break;

                    case "SOLD_TO_CODE":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            dataResults = data.Where(m => !string.IsNullOrEmpty(m.SOLD_TO_CODE) && m.SOLD_TO_CODE.ToUpper().Contains(filterValue.ToUpper())).ToList();
                        }
                        break;

                    case "SOLD_TO_DISPLAY_TXT":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            dataResults = data.Where(m => !string.IsNullOrEmpty(m.SOLD_TO_DISPLAY_TXT) && m.SOLD_TO_DISPLAY_TXT.ToUpper().Contains(filterValue.ToUpper())).ToList();
                        }
                        break;

                    case "SHIP_TO_CODE":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            dataResults = data.Where(m => !string.IsNullOrEmpty(m.SHIP_TO_CODE) && m.SHIP_TO_CODE.ToUpper().Contains(filterValue.ToUpper())).ToList();
                        }
                        break;

                    case "SHIP_TO_DISPLAY_TXT":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            dataResults = data.Where(m => !string.IsNullOrEmpty(m.SHIP_TO_DISPLAY_TXT) && m.SHIP_TO_DISPLAY_TXT.ToUpper().Contains(filterValue.ToUpper())).ToList();
                        }
                        break;

                    case "ZONE":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            dataResults = data.Where(m => !string.IsNullOrEmpty(m.ZONE) && m.ZONE.ToUpper().Contains(filterValue.ToUpper())).ToList();
                        }
                        break;

                    case "COUNTRY":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            dataResults = data.Where(m => !string.IsNullOrEmpty(m.COUNTRY) && m.COUNTRY.ToUpper().Contains(filterValue.ToUpper())).ToList();
                        }
                        break;


                    case "COUNTRY_DISPLAY_TXT":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            dataResults = data.Where(m => !string.IsNullOrEmpty(m.COUNTRY_DISPLAY_TXT) && m.COUNTRY_DISPLAY_TXT.ToUpper().Contains(filterValue.ToUpper())).ToList();
                        }
                        break;

                    default:
                        // data = data.ToList();
                        break;
                }
            }

            cnt = dataResults.Count();

            return dataResults;
        }

        private static IQueryable<ART_M_PIC_2> FilterPICData2(IQueryable<ART_M_PIC_2> q, ART_M_PIC_REQUEST param)
        {
            var filterValue = "";
            List<ART_M_PIC_2> dataResults = new List<ART_M_PIC_2>();

            foreach (var item in param.columns)
            {

                switch (item.data)
                {
                    case "USER_DISPLAY_TXT":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            q = (from r in q where filterValue.ToUpper().Contains(r.USER_DISPLAY_TXT.ToUpper()) select r);
                        }
                        break;

                    case "FIRST_NAME_DISPLAY_TXT":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            q = (from r in q
                                 where !String.IsNullOrEmpty(r.FIRST_NAME_DISPLAY_TXT)
                     && filterValue.ToUpper().Contains(r.FIRST_NAME_DISPLAY_TXT.ToUpper())
                                 select r);
                        }
                        break;

                    case "LAST_NAME_DISPLAY_TXT":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            q = (from r in q where r.LAST_NAME_DISPLAY_TXT.ToUpper().Contains(filterValue.ToUpper()) select r);
                        }
                        break;

                    case "SOLD_TO_CODE":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            q = (from r in q where r.SOLD_TO_CODE.ToUpper().Contains(filterValue.ToUpper()) select r);
                        }
                        break;

                    case "SOLD_TO_DISPLAY_TXT":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            q = (from r in q where r.SOLD_TO_DISPLAY_TXT.ToUpper().Contains(filterValue.ToUpper()) select r);
                        }
                        break;

                    case "SHIP_TO_CODE":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            q = (from r in q where r.SHIP_TO_CODE.ToUpper().Contains(filterValue.ToUpper()) select r);
                        }
                        break;

                    case "SHIP_TO_DISPLAY_TXT":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            q = (from r in q where r.SHIP_TO_DISPLAY_TXT.ToUpper().Contains(filterValue.ToUpper()) select r);
                        }
                        break;

                    case "ZONE":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            q = (from r in q where r.ZONE.ToUpper().Contains(filterValue.ToUpper()) select r);
                        }
                        break;

                    case "COUNTRY":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            q = (from r in q where r.COUNTRY.ToUpper().Contains(filterValue.ToUpper()) select r);
                        }
                        break;


                    case "COUNTRY_DISPLAY_TXT":
                        filterValue = item.search.value;
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            q = (from r in q where r.COUNTRY_DISPLAY_TXT.ToUpper().Contains(filterValue.ToUpper()) select r);
                        }
                        break;

                    default:
                        // data = data.ToList();
                        break;
                }
            }

            return q;
        }

        private static List<SAP_M_COUNTRY_2> GetListZone()
        {
            SAP_M_COUNTRY_RESULT zoneResults = new SAP_M_COUNTRY_RESULT();
            SAP_M_COUNTRY_REQUEST zoneParam = new SAP_M_COUNTRY_REQUEST();
            SAP_M_COUNTRY_2 zoneData = new SAP_M_COUNTRY_2();

            zoneParam.data = zoneData;

            zoneResults = ZoneHelper.GetZone(zoneParam);

            var listZone = (from z in zoneResults.data
                            select new SAP_M_COUNTRY_2
                            {
                                ZONE = z.ZONE,
                                ID = z.ID
                            }).ToList();

            return listZone;
        }
    }
}
