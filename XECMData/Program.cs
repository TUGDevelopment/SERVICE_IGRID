using BLL.Services;
using DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XECMData
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                runAll();
                Console.WriteLine("Completed");
            }
            catch (Exception ex)
            {
                CNService.GetErrorMessage(ex);
            }
        }

        private static void runAll()
        {

            try
            {
                if (DateTime.Now.Hour == 1 || DateTime.Now.Hour == 2 || DateTime.Now.Hour == 3)
                    deleteTableLogContain();
            }
            catch (Exception ex) { CNService.GetErrorMessage(ex); }

            try
            {
                deleteTableLog();
            }
            catch (Exception ex) { CNService.GetErrorMessage(ex); }

            try
            {
                deleteTableLogEmail();
            }
            catch (Exception ex) { CNService.GetErrorMessage(ex); }

            try
            {
                var cntCus = customer();
                saveLog("XECM_M_CUSTOMER", cntCus);
            }
            catch (Exception ex) { CNService.GetErrorMessage(ex); }

            try
            {
                var cntVendor = vendor();
                saveLog("XECM_M_VENDOR", cntVendor);
            }
            catch (Exception ex) { CNService.GetErrorMessage(ex); }

            try
            {
                var cntMat3 = material3();
                saveLog("XECM_M_PRODUCT", cntMat3);
            }
            catch (Exception ex) { CNService.GetErrorMessage(ex); }

            try
            {
                var cntMat5 = material5();
                saveLog("XECM_M_PRODUCT5", cntMat5);
            }
            catch (Exception ex) { CNService.GetErrorMessage(ex); }
        }

        private static void saveLog(string TABLE_NAME, int total)
        {
            using (var context = new ARTWORKEntities())
            {
                ART_SYS_LOG log = new ART_SYS_LOG();
                log.CREATE_BY = -2;
                log.UPDATE_BY = -2;
                log.TABLE_NAME = TABLE_NAME;
                log.ACTION = "Batch XECM Data";
                log.NEW_VALUE = "Found " + total.ToString() + " Records";
                ART_SYS_LOG_SERVICE.SaveNoLog(log, context);
            }
        }

        private static void deleteTableLogContain()
        {
            using (ARTWORKEntities dc = new ARTWORKEntities())
            {
                string queryDeleteLogContain = @"TRUNCATE TABLE [ART_TEMP_CONTAIN]";
                dc.Database.ExecuteSqlCommand(queryDeleteLogContain);
            }
        }

        private static void deleteTableLog()
        {
            using (ARTWORKEntities dc = new ARTWORKEntities())
            {
                string queryDeleteLog = @"DELETE FROM ART_SYS_LOG where convert(date,CREATE_DATE) < convert(date,(GETDATE()-90)) ";
                dc.Database.ExecuteSqlCommand(queryDeleteLog);
            }
        }

        private static void deleteTableLogEmail()
        {
            using (ARTWORKEntities dc = new ARTWORKEntities())
            {
                string queryDeleteLog = @"DELETE FROM ART_SYS_LOG_EMAIL where convert(date,CREATE_DATE) < convert(date,(GETDATE()-90)) ";
                dc.Database.ExecuteSqlCommand(queryDeleteLog);
            }
        }

        private static int customer()
        {
            int i = 0;
            Console.WriteLine("Searching Customer...");

            var CustomerWorkspaceNodeId = Convert.ToInt64(ConfigurationManager.AppSettings["CustomerWorkspaceNodeId"]);
            var PreviousDays = Convert.ToInt32(ConfigurationManager.AppSettings["PreviousDays"]);

            var listAllCustomer = new List<XECM_M_CUSTOMER>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    listAllCustomer = XECM_M_CUSTOMER_SERVICE.GetAll(context);
                }
            }
            while (PreviousDays <= 0)
            {
                List<DTree> listCS = new List<DTree>();
                var listCustomer = GetData(CustomerWorkspaceNodeId, PreviousDays, "cus", null, null, listAllCustomer, null, ref listCS);

                var groupByCustomer = listCustomer.GroupBy(item => item.ID)
                     .Select(group => new { ID = group.Key, Items = group.ToList() })
                     .ToList();

                foreach (var item in groupByCustomer)
                {
                    var CustomerCode = listCustomer.Where(m => m.ID == item.ID && m.AttrID == 2).FirstOrDefault().ValStr;
                    var CustomerName = listCustomer.Where(m => m.ID == item.ID && m.AttrID == 3).FirstOrDefault().ValStr;
                    var CustomerDeleteFlag = listCustomer.Where(m => m.ID == item.ID && m.AttrID == 4).FirstOrDefault().ValStr;

                    if (!string.IsNullOrEmpty(CustomerCode))
                    {
                        XECM_M_CUSTOMER cus = new XECM_M_CUSTOMER();
                        var chk = listAllCustomer.Where(m => m.CUSTOMER_CODE == CustomerCode.Trim()).ToList();
                        if (chk.Count == 1)
                        {
                            cus.CUSTOMER_ID = chk.FirstOrDefault().CUSTOMER_ID;
                        }

                        if (!string.IsNullOrEmpty(CustomerCode)) cus.CUSTOMER_CODE = CustomerCode.Trim();
                        if (!string.IsNullOrEmpty(CustomerName)) cus.CUSTOMER_NAME = CustomerName.Trim();

                        if (!string.IsNullOrEmpty(CustomerDeleteFlag))
                        {
                            if (CustomerDeleteFlag.ToLower() == ("x"))
                                cus.IS_ACTIVE = null;
                            else
                                cus.IS_ACTIVE = "X";
                        }
                        else
                            cus.IS_ACTIVE = "X";

                        cus.CREATE_BY = -2;
                        cus.UPDATE_BY = -2;

                        var templistCS = listCS.Where(m => m.Name.Split('-')[0].Trim() == CustomerCode.Trim()).FirstOrDefault();
                        if (templistCS != null)
                            cus.UPDATE_DATE_CS = templistCS.ModifyDate;
                        else
                            cus.UPDATE_DATE_CS = null;

                        using (var context = new ARTWORKEntities())
                        {
                            XECM_M_CUSTOMER_SERVICE.SaveOrUpdateNoLog(cus, context);
                        }
                        i++;
                    }
                }

                PreviousDays++;
            }
            return i;
        }

        private static int vendor()
        {
            int i = 0;
            Console.WriteLine("Searching Vendor...");

            var VendorWorkspaceNodeId = Convert.ToInt64(ConfigurationManager.AppSettings["VendorWorkspaceNodeId"]);
            var PreviousDays = Convert.ToInt32(ConfigurationManager.AppSettings["PreviousDays"]);

            var listAllVendor = new List<XECM_M_VENDOR>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    listAllVendor = XECM_M_VENDOR_SERVICE.GetAll(context);
                }
            }

            while (PreviousDays <= 0)
            {
                List<DTree> listCS = new List<DTree>();
                var listVendor = GetData(VendorWorkspaceNodeId, PreviousDays, "vendor", null, null, null, listAllVendor, ref listCS);

                var groupByVendor = listVendor.GroupBy(item => item.ID)
                             .Select(group => new { ID = group.Key, Items = group.ToList() })
                             .ToList();

                foreach (var item in groupByVendor)
                {
                    var VendorCode = listVendor.Where(m => m.ID == item.ID && m.AttrID == 2).FirstOrDefault().ValStr;
                    var VendorName = listVendor.Where(m => m.ID == item.ID && m.AttrID == 3).FirstOrDefault().ValStr;
                    var VendorDeleteFlag = listVendor.Where(m => m.ID == item.ID && m.AttrID == 4).FirstOrDefault().ValStr;

                    if (!string.IsNullOrEmpty(VendorCode))
                    {
                        XECM_M_VENDOR vendor = new XECM_M_VENDOR();
                        var chk = listAllVendor.Where(m => m.VENDOR_CODE == VendorCode.Trim()).ToList();
                        if (chk.Count == 1)
                        {
                            vendor.VENDOR_ID = chk.FirstOrDefault().VENDOR_ID;
                        }

                        if (!string.IsNullOrEmpty(VendorCode)) vendor.VENDOR_CODE = VendorCode.Trim();
                        if (!string.IsNullOrEmpty(VendorName)) vendor.VENDOR_NAME = VendorName.Trim();

                        if (!string.IsNullOrEmpty(VendorDeleteFlag))
                        {
                            if (VendorDeleteFlag.ToLower() == ("x"))
                                vendor.IS_ACTIVE = null;
                            else
                                vendor.IS_ACTIVE = "X";
                        }
                        else
                            vendor.IS_ACTIVE = "X";

                        vendor.CREATE_BY = -2;
                        vendor.UPDATE_BY = -2;
                        var templistCS = listCS.Where(m => m.Name.Split('-')[0].Trim() == VendorCode.Trim()).FirstOrDefault();
                        if (templistCS != null)
                            vendor.UPDATE_DATE_CS = templistCS.ModifyDate;
                        else
                            vendor.UPDATE_DATE_CS = null;

                        using (var context = new ARTWORKEntities())
                        {
                            XECM_M_VENDOR_SERVICE.SaveOrUpdateNoLog(vendor, context);
                        }
                        i++;
                    }
                }

                PreviousDays++;
            }
            return i;
        }

        private static int material3()
        {
            int i = 0;
            Console.WriteLine("Searching Material3...");

            var MaterialWorkspaceNodeId_FinishedGoods = Convert.ToInt64(ConfigurationManager.AppSettings["MaterialWorkspaceNodeId_FinishedGoods"]);
            var PreviousDays = Convert.ToInt32(ConfigurationManager.AppSettings["PreviousDays"]);

            var listAllProduct = new List<XECM_M_PRODUCT>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    listAllProduct = XECM_M_PRODUCT_SERVICE.GetAll(context);
                }
            }

            while (PreviousDays <= 0)
            {
                List<DTree> listCS = new List<DTree>();
                var listSecondaryPackaging = GetData(MaterialWorkspaceNodeId_FinishedGoods, PreviousDays, "mat3", listAllProduct, null, null, null, ref listCS);

                var groupBySecondaryPackaging = listSecondaryPackaging.GroupBy(item => item.ID)
                             .Select(group => new { ID = group.Key, Items = group.ToList() })
                             .ToList();

                foreach (var item in groupBySecondaryPackaging)
                {
                    var temp = listSecondaryPackaging.Where(m => m.ID == item.ID);
                    var PRODUCT_CODE = temp.Where(m => m.ID == item.ID && m.AttrID == 2).FirstOrDefault().ValStr;
                    var PRODUCT_DESCRIPTION = temp.Where(m => m.ID == item.ID && m.AttrID == 3).FirstOrDefault().ValStr;
                    var CONTAINER_TYPE = temp.Where(m => m.ID == item.ID && m.AttrID == 16).FirstOrDefault().ValStr;
                    var NET_WEIGHT = temp.Where(m => m.ID == item.ID && m.AttrID == 99).FirstOrDefault().ValStr;
                    var DRAINED_WEIGHT = temp.Where(m => m.ID == item.ID && m.AttrID == 100).FirstOrDefault().ValStr;
                    var PRIMARY_SIZE = temp.Where(m => m.ID == item.ID && m.AttrID == 69).FirstOrDefault().ValStr;
                    var LID_TYPE = temp.Where(m => m.ID == item.ID && m.AttrID == 63).FirstOrDefault().ValStr;
                    var AMBIENT_PACKING_STYLE = temp.Where(m => m.ID == item.ID && m.AttrID == 97).FirstOrDefault().ValStr;
                    var PACK_SIZE = temp.Where(m => m.ID == item.ID && m.AttrID == 98).FirstOrDefault().ValStr;

                    if (!string.IsNullOrEmpty(PRODUCT_CODE))
                    {
                        XECM_M_PRODUCT product = new XECM_M_PRODUCT();
                        var chk = listAllProduct.Where(m => m.PRODUCT_CODE == PRODUCT_CODE.Trim()).ToList();
                        if (chk.Count == 1)
                        {
                            product.XECM_PRODUCT_ID = chk.FirstOrDefault().XECM_PRODUCT_ID;
                        }

                        if (!string.IsNullOrEmpty(PRODUCT_CODE)) product.PRODUCT_CODE = PRODUCT_CODE.Trim();
                        if (!string.IsNullOrEmpty(PRODUCT_DESCRIPTION)) product.PRODUCT_DESCRIPTION = PRODUCT_DESCRIPTION.Trim();
                        if (!string.IsNullOrEmpty(NET_WEIGHT)) product.NET_WEIGHT = NET_WEIGHT.Trim();
                        if (!string.IsNullOrEmpty(DRAINED_WEIGHT)) product.DRAINED_WEIGHT = DRAINED_WEIGHT.Trim();
                        if (!string.IsNullOrEmpty(PRIMARY_SIZE)) product.PRIMARY_SIZE = PRIMARY_SIZE.Trim();
                        if (!string.IsNullOrEmpty(CONTAINER_TYPE)) product.CONTAINER_TYPE = CONTAINER_TYPE.Trim();
                        if (!string.IsNullOrEmpty(LID_TYPE)) product.LID_TYPE = LID_TYPE.Trim();
                        if (!string.IsNullOrEmpty(AMBIENT_PACKING_STYLE)) product.PACKING_STYLE = AMBIENT_PACKING_STYLE.Trim();
                        if (!string.IsNullOrEmpty(PACK_SIZE)) product.PACK_SIZE = PACK_SIZE.Trim();

                        product.CREATE_BY = -2;
                        product.UPDATE_BY = -2;
                        var templistCS = listCS.Where(m => m.Name.Split('-')[0].Trim() == PRODUCT_CODE.Trim()).FirstOrDefault();
                        if (templistCS != null)
                            product.UPDATE_DATE_CS = templistCS.ModifyDate;
                        else
                            product.UPDATE_DATE_CS = null;

                        using (var context = new ARTWORKEntities())
                        {
                            XECM_M_PRODUCT_SERVICE.SaveOrUpdateNoLog(product, context);
                        }
                        i++;
                    }
                }

                PreviousDays++;
            }
            return i;
        }

        private static int material5()
        {
            int i = 0;
            Console.WriteLine("Searching Material5...");

            var MaterialWorkspaceNodeId_SecondaryPackaging = Convert.ToInt64(ConfigurationManager.AppSettings["MaterialWorkspaceNodeId_SecondaryPackaging"]);
            var PreviousDays = Convert.ToInt32(ConfigurationManager.AppSettings["PreviousDays"]);

            var listAllProduct5 = new List<XECM_M_PRODUCT5>();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    listAllProduct5 = XECM_M_PRODUCT5_SERVICE.GetAll(context);
                }
            }

            while (PreviousDays <= 0)
            {
                List<DTree> listCS = new List<DTree>();
                var listSecondaryPackaging = GetData(MaterialWorkspaceNodeId_SecondaryPackaging, PreviousDays, "mat5", null, listAllProduct5, null, null, ref listCS);

                var groupBySecondaryPackaging = listSecondaryPackaging.GroupBy(item => item.ID)
                             .Select(group => new { ID = group.Key, Items = group.ToList() })
                             .ToList();

                foreach (var item in groupBySecondaryPackaging)
                {
                    var temp = listSecondaryPackaging.Where(m => m.ID == item.ID);
                    var PRODUCT_CODE = temp.Where(m => m.ID == item.ID && m.AttrID == 2).FirstOrDefault().ValStr;
                    var PRODUCT_DESCRIPTION = temp.Where(m => m.ID == item.ID && m.AttrID == 3).FirstOrDefault().ValStr;
                    var CONTAINER_TYPE = temp.Where(m => m.ID == item.ID && m.AttrID == 16).FirstOrDefault().ValStr;
                    var NET_WEIGHT = temp.Where(m => m.ID == item.ID && m.AttrID == 99).FirstOrDefault().ValStr;
                    var DRAINED_WEIGHT = temp.Where(m => m.ID == item.ID && m.AttrID == 100).FirstOrDefault().ValStr;
                    var PRIMARY_SIZE = temp.Where(m => m.ID == item.ID && m.AttrID == 69).FirstOrDefault().ValStr;
                    var LID_TYPE = temp.Where(m => m.ID == item.ID && m.AttrID == 63).FirstOrDefault().ValStr;
                    var AMBIENT_PACKING_STYLE = temp.Where(m => m.ID == item.ID && m.AttrID == 66).FirstOrDefault().ValStr;
                    var PACK_SIZE = temp.Where(m => m.ID == item.ID && m.AttrID == 65).FirstOrDefault().ValStr;

                    if (!string.IsNullOrEmpty(PRODUCT_CODE))
                    {
                        XECM_M_PRODUCT5 product = new XECM_M_PRODUCT5();
                        var chk = listAllProduct5.Where(m => m.PRODUCT_CODE == PRODUCT_CODE.Trim()).ToList();
                        if (chk.Count == 1)
                        {
                            product.XECM_PRODUCT5_ID = chk.FirstOrDefault().XECM_PRODUCT5_ID;
                        }

                        if (!string.IsNullOrEmpty(PRODUCT_CODE)) product.PRODUCT_CODE = PRODUCT_CODE.Trim();
                        if (!string.IsNullOrEmpty(PRODUCT_DESCRIPTION)) product.PRODUCT_DESCRIPTION = PRODUCT_DESCRIPTION.Trim();
                        if (!string.IsNullOrEmpty(NET_WEIGHT)) product.NET_WEIGHT = NET_WEIGHT.Trim();
                        if (!string.IsNullOrEmpty(DRAINED_WEIGHT)) product.DRAINED_WEIGHT = DRAINED_WEIGHT.Trim();
                        if (!string.IsNullOrEmpty(PRIMARY_SIZE)) product.PRIMARY_SIZE = PRIMARY_SIZE.Trim();
                        if (!string.IsNullOrEmpty(CONTAINER_TYPE)) product.CONTAINER_TYPE = CONTAINER_TYPE.Trim();
                        if (!string.IsNullOrEmpty(LID_TYPE)) product.LID_TYPE = LID_TYPE.Trim();
                        if (!string.IsNullOrEmpty(AMBIENT_PACKING_STYLE)) product.PACKING_STYLE = AMBIENT_PACKING_STYLE.Trim();
                        if (!string.IsNullOrEmpty(PACK_SIZE)) product.PACK_SIZE = PACK_SIZE.Trim();

                        product.CREATE_BY = -2;
                        product.UPDATE_BY = -2;
                        var templistCS = listCS.Where(m => m.Name.Split('-')[0].Trim() == PRODUCT_CODE.Trim()).FirstOrDefault();
                        if (templistCS != null)
                            product.UPDATE_DATE_CS = templistCS.ModifyDate;
                        else
                            product.UPDATE_DATE_CS = null;

                        using (var context = new ARTWORKEntities())
                        {
                            XECM_M_PRODUCT5_SERVICE.SaveOrUpdateNoLog(product, context);
                        }
                        i++;

                        using (var context = new ARTWORKEntities())
                        {
                            twoPFromMat5(product.PRODUCT_CODE, context, listAllProduct5);
                            threePFromMat5(product.PRODUCT_CODE, context, listAllProduct5);
                        }
                    }
                }

                PreviousDays++;
            }
            return i;
        }

        private static void twoPFromMat5(string mat5, ARTWORKEntities context, List<XECM_M_PRODUCT5> listAllProduct5)
        {
            //var list = XECM_M_PRODUCT5_SERVICE.GetByItem(new XECM_M_PRODUCT5() { PRODUCT_CODE = mat5 }, context);
            var list = listAllProduct5.Where(m => m.PRODUCT_CODE == mat5).ToList();
            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.PACKING_STYLE) && !string.IsNullOrEmpty(item.PACK_SIZE))
                {
                    SAP_M_2P model = new SAP_M_2P();
                    model.IS_ACTIVE = "X";
                    model.PACKING_SYLE_VALUE = item.PACKING_STYLE;
                    model.PACKING_SYLE_DESCRIPTION = item.PACKING_STYLE;

                    model.PACK_SIZE_VALUE = item.PACK_SIZE;
                    model.PACK_SIZE_DESCRIPTION = item.PACK_SIZE;

                    var chk = SAP_M_2P_SERVICE.GetByItem(model, context);
                    if (chk.Count == 0)
                    {
                        model.CREATE_BY = -2;
                        model.UPDATE_BY = -2;

                        SAP_M_2P_SERVICE.SaveNoLog(model, context);
                    }
                }
            }
        }

        private static void threePFromMat5(string mat5, ARTWORKEntities context, List<XECM_M_PRODUCT5> listAllProduct5)
        {
            //var list = XECM_M_PRODUCT5_SERVICE.GetByItem(new XECM_M_PRODUCT5() { PRODUCT_CODE = mat5 }, context);
            var list = listAllProduct5.Where(m => m.PRODUCT_CODE == mat5).ToList();
            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.CONTAINER_TYPE) && !string.IsNullOrEmpty(item.LID_TYPE) && !string.IsNullOrEmpty(item.PRIMARY_SIZE))
                {
                    SAP_M_3P model = new SAP_M_3P();
                    model.IS_ACTIVE = "X";
                    model.CONTAINER_TYPE_VALUE = item.CONTAINER_TYPE;
                    model.CONTAINER_TYPE_DESCRIPTION = item.CONTAINER_TYPE;

                    model.LID_TYPE_VALUE = item.LID_TYPE;
                    model.LID_TYPE_DESCRIPTION = item.LID_TYPE;

                    model.PRIMARY_SIZE_VALUE = item.PRIMARY_SIZE;
                    model.PRIMARY_SIZE_DESCRIPTION = item.PRIMARY_SIZE;

                    var chk = SAP_M_3P_SERVICE.GetByItem(model, context);
                    if (chk.Count == 0)
                    {
                        model.CREATE_BY = -2;
                        model.UPDATE_BY = -2;

                        SAP_M_3P_SERVICE.SaveNoLog(model, context);
                    }
                }
            }
        }

        public static List<LLAttrData> GetData(long nodeId, int PreviousDays, string type
            , List<XECM_M_PRODUCT> listAllProduct, List<XECM_M_PRODUCT5> listAllProduct5, List<XECM_M_CUSTOMER> listAllCustomer, List<XECM_M_VENDOR> listAllVendor
            , ref List<DTree> listCS)
        {
            var date = DateTime.Now.AddDays(PreviousDays);

            listCS = new List<DTree>();
            using (CSTUEntities dc = new CSTUEntities())
            {
                dc.Database.CommandTimeout = 300;
                listCS = (from dtree in dc.DTree
                          where dtree.ParentID == nodeId
                          && DbFunctions.TruncateTime(dtree.ModifyDate) == DbFunctions.TruncateTime(date)
                          select dtree).ToList();
            }

            if (listCS.Count > 0)
            {
                var listDataID = new List<long>();
                if (type == "mat3")
                {
                    listDataID = (from m in listCS
                                  join m2 in listAllProduct on m.Name.Split('-')[0].Trim() equals m2.PRODUCT_CODE
                                  where m.ModifyDate != m2.UPDATE_DATE_CS
                                  select m.DataID).ToList();

                    var listAllProductCode = listAllProduct.Select(m => m.PRODUCT_CODE).ToList();
                    var temp = (from m in listCS
                                where !listAllProductCode.Contains(m.Name.Split('-')[0].Trim())
                                select m.DataID).ToList();

                    listDataID = listDataID.Union(temp).ToList();
                }
                else if (type == "mat5")
                {
                    listDataID = (from m in listCS
                                  join m2 in listAllProduct5 on m.Name.Split('-')[0].Trim() equals m2.PRODUCT_CODE
                                  where m.ModifyDate != m2.UPDATE_DATE_CS
                                  select m.DataID).ToList();

                    var listAllProduct5Code = listAllProduct5.Select(m => m.PRODUCT_CODE).ToList();
                    var temp = (from m in listCS
                                where !listAllProduct5Code.Contains(m.Name.Split('-')[0].Trim())
                                select m.DataID).ToList();

                    listDataID = listDataID.Union(temp).ToList();
                }
                else if (type == "cus")
                {
                    listDataID = (from m in listCS
                                  join m2 in listAllCustomer on m.Name.Split('-')[0].Trim() equals m2.CUSTOMER_CODE
                                  where m.ModifyDate != m2.UPDATE_DATE_CS
                                  select m.DataID).ToList();

                    var listAllCustomerCode = listAllCustomer.Select(m => m.CUSTOMER_CODE).ToList();
                    var temp = (from m in listCS
                                where !listAllCustomerCode.Contains(m.Name.Split('-')[0].Trim())
                                select m.DataID).ToList();

                    listDataID = listDataID.Union(temp).ToList();
                }
                else if (type == "vendor")
                {
                    listDataID = (from m in listCS
                                  join m2 in listAllVendor on m.Name.Split('-')[0].Trim() equals m2.VENDOR_CODE
                                  where m.ModifyDate != m2.UPDATE_DATE_CS
                                  select m.DataID).ToList();

                    var listAllVendorCode = listAllVendor.Select(m => m.VENDOR_CODE).ToList();
                    var temp = (from m in listCS
                                where !listAllVendorCode.Contains(m.Name.Split('-')[0].Trim())
                                select m.DataID).ToList();

                    listDataID = listDataID.Union(temp).ToList();
                }

                if (listDataID.Count == 0)
                {
                    return new List<LLAttrData>();
                }

                using (CSTUEntities dc = new CSTUEntities())
                {
                    dc.Database.CommandTimeout = 300;
                    return (from att in dc.LLAttrData
                            join dtree in dc.DTree on att.ID equals dtree.DataID
                            where dtree.ParentID == nodeId
                            && att.AttrType == -1
                            && (att.AttrID == 2 || att.AttrID == 3 || att.AttrID == 4 || att.AttrID == 16 || att.AttrID == 99 || att.AttrID == 100 || att.AttrID == 69 || att.AttrID == 63 || att.AttrID == 97 || att.AttrID == 98 || att.AttrID == 66 || att.AttrID == 65)
                            && listDataID.Contains(dtree.DataID)
                            select att).OrderBy(m => m.ID).ThenBy(m => m.AttrID).ToList();
                }
            }
            else
            {
                return new List<LLAttrData>();
            }
        }

        public static List<LLAttrData> GetData(long nodeId, int PreviousDays, string type)
        {
            var date = DateTime.Now.AddDays(PreviousDays);

            var listCS = new List<DTree>();
            using (CSTUEntities dc = new CSTUEntities())
            {
                dc.Database.CommandTimeout = 300;
                listCS = (from dtree in dc.DTree
                          where dtree.ParentID == nodeId
                          //&& DbFunctions.TruncateTime(dtree.ModifyDate) == DbFunctions.TruncateTime(date)
                          select dtree).ToList();
            }

            if (listCS.Count > 0)
            {
                var listDataID = new List<long>();
                if (type == "mat3")
                {
                    //using (ARTWORKEntities dc2 = new ARTWORKEntities())
                    {
                        //foreach (var i in listCS)
                        //{
                        //    var productCodeCS = i.Name.Split('-')[0].Trim();
                        //    var updateDateCS = i.ModifyDate;
                        //    var productCodeArtwork = listAllProduct.Where(m => m.PRODUCT_CODE == productCodeCS).FirstOrDefault();
                        //    if (productCodeArtwork != null)
                        //    {
                        //        if (updateDateCS != productCodeArtwork.UPDATE_DATE_CS)
                        //        {
                        //            listDataID.Add(i.DataID);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        listDataID.Add(i.DataID);
                        //    }
                        //}
                    }
                }
                else if (type == "mat5")
                {
                    using (ARTWORKEntities context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            var listArtwork = (from p in context.XECM_M_PRODUCT5 where DbFunctions.TruncateTime(p.UPDATE_DATE_CS) == DbFunctions.TruncateTime(date) select p).ToList();
                            var temp = listArtwork.Select(m => m.PRODUCT_CODE).ToList();
                            listDataID = listCS.Where(m => !temp.Contains(m.Name.Split('-')[0].Trim())).Select(m => m.DataID).ToList();
                        }
                    }
                }
                else if (type == "cus")
                {
                    using (ARTWORKEntities context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            var listArtwork = (from p in context.XECM_M_CUSTOMER where DbFunctions.TruncateTime(p.UPDATE_DATE_CS) == DbFunctions.TruncateTime(date) select p).ToList();
                            var temp = listArtwork.Select(m => m.CUSTOMER_CODE).ToList();
                            listDataID = listCS.Where(m => !temp.Contains(m.Name.Split('-')[0].Trim())).Select(m => m.DataID).ToList();
                        }
                    }
                }
                else if (type == "vendor")
                {
                    using (ARTWORKEntities context = new ARTWORKEntities())
                    {
                        using (CNService.IsolationLevel(context))
                        {
                            var listArtwork = (from p in context.XECM_M_VENDOR where DbFunctions.TruncateTime(p.UPDATE_DATE_CS) == DbFunctions.TruncateTime(date) select p).ToList();
                            var temp = listArtwork.Select(m => m.VENDOR_CODE).ToList();
                            listDataID = listCS.Where(m => !temp.Contains(m.Name.Split('-')[0].Trim())).Select(m => m.DataID).ToList();
                        }
                    }
                }

                using (CSTUEntities context = new CSTUEntities())
                {
                    context.Database.CommandTimeout = 300;
                    return (from att in context.LLAttrData
                            join dtree in context.DTree on att.ID equals dtree.DataID
                            where dtree.ParentID == nodeId
                            && att.AttrType == -1
                            && (att.AttrID == 2 || att.AttrID == 3 || att.AttrID == 4 || att.AttrID == 16 || att.AttrID == 99 || att.AttrID == 100 || att.AttrID == 69 || att.AttrID == 63 || att.AttrID == 97 || att.AttrID == 98 || att.AttrID == 66 || att.AttrID == 65)
                            && DbFunctions.TruncateTime(dtree.ModifyDate) == DbFunctions.TruncateTime(date)
                            && listDataID.Contains(dtree.DataID)
                            select att).OrderBy(m => m.ID).ThenBy(m => m.AttrID).ToList();
                }
            }
            else
            {
                return new List<LLAttrData>();
            }
        }










        public static List<LLAttrData> GetData2(long nodeId, string mat)
        {
            using (CSTUEntities dc = new CSTUEntities())
            {
                var DataID = (from att in dc.LLAttrData
                              join dtree in dc.DTree on att.ID equals dtree.DataID
                              where dtree.ParentID == nodeId
                              && att.AttrType == -1
                              && att.ValStr == mat
                              select dtree.DataID);

                return (from att in dc.LLAttrData
                        join dtree in dc.DTree on att.ID equals dtree.DataID
                        where dtree.ParentID == nodeId
                        && att.AttrType == -1
                        && dtree.DataID == DataID.FirstOrDefault()
                        select att).OrderBy(m => m.ID).ThenBy(m => m.AttrID).ToList();
            }
        }

        private static void material3(string mat3)
        {
            var MaterialWorkspaceNodeId_FinishedGoods = Convert.ToInt64(ConfigurationManager.AppSettings["MaterialWorkspaceNodeId_FinishedGoods"]);
            var listSecondaryPackaging = GetData2(MaterialWorkspaceNodeId_FinishedGoods, mat3);

            var groupBySecondaryPackaging = listSecondaryPackaging.GroupBy(item => item.ID)
                         .Select(group => new { ID = group.Key, Items = group.ToList() })
                         .ToList();

            using (var context = new ARTWORKEntities())
            {
                foreach (var item in groupBySecondaryPackaging)
                {
                    var PRODUCT_CODE = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 2).FirstOrDefault().ValStr;
                    if (PRODUCT_CODE == mat3)
                    {
                        var PRODUCT_DESCRIPTION = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 3).FirstOrDefault().ValStr;

                        var CONTAINER_TYPE = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 16).FirstOrDefault().ValStr;
                        var NET_WEIGHT = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 99).FirstOrDefault().ValStr;
                        var DRAINED_WEIGHT = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 100).FirstOrDefault().ValStr;
                        var PRIMARY_SIZE = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 69).FirstOrDefault().ValStr;
                        var LID_TYPE = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 63).FirstOrDefault().ValStr;

                        var AMBIENT_PACKING_STYLE = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 97).FirstOrDefault().ValStr;
                        var PACK_SIZE = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 98).FirstOrDefault().ValStr;

                        if (!string.IsNullOrEmpty(PRODUCT_CODE))
                        {
                            XECM_M_PRODUCT product = new XECM_M_PRODUCT();
                            var chk = XECM_M_PRODUCT_SERVICE.GetByItem(new XECM_M_PRODUCT() { PRODUCT_CODE = PRODUCT_CODE.Trim() }, context);
                            if (chk.Count == 1)
                            {
                                product.XECM_PRODUCT_ID = chk.FirstOrDefault().XECM_PRODUCT_ID;
                            }

                            if (!string.IsNullOrEmpty(PRODUCT_CODE)) product.PRODUCT_CODE = PRODUCT_CODE.Trim();
                            if (!string.IsNullOrEmpty(PRODUCT_DESCRIPTION)) product.PRODUCT_DESCRIPTION = PRODUCT_DESCRIPTION.Trim();
                            if (!string.IsNullOrEmpty(NET_WEIGHT)) product.NET_WEIGHT = NET_WEIGHT.Trim();
                            if (!string.IsNullOrEmpty(DRAINED_WEIGHT)) product.DRAINED_WEIGHT = DRAINED_WEIGHT.Trim();
                            if (!string.IsNullOrEmpty(PRIMARY_SIZE)) product.PRIMARY_SIZE = PRIMARY_SIZE.Trim();
                            if (!string.IsNullOrEmpty(CONTAINER_TYPE)) product.CONTAINER_TYPE = CONTAINER_TYPE.Trim();
                            if (!string.IsNullOrEmpty(LID_TYPE)) product.LID_TYPE = LID_TYPE.Trim();
                            if (!string.IsNullOrEmpty(AMBIENT_PACKING_STYLE)) product.PACKING_STYLE = AMBIENT_PACKING_STYLE.Trim();
                            if (!string.IsNullOrEmpty(PACK_SIZE)) product.PACK_SIZE = PACK_SIZE.Trim();

                            product.CREATE_BY = -2;
                            product.UPDATE_BY = -2;

                            Console.WriteLine("Product : " + product.PRODUCT_CODE + " " + product.PRODUCT_DESCRIPTION);
                            XECM_M_PRODUCT_SERVICE.SaveOrUpdateNoLog(product, context);
                        }
                    }
                }
            }
        }

        private static void material5(string mat5)
        {
            var MaterialWorkspaceNodeId_SecondaryPackaging = Convert.ToInt64(ConfigurationManager.AppSettings["MaterialWorkspaceNodeId_SecondaryPackaging"]);
            var listSecondaryPackaging = GetData2(MaterialWorkspaceNodeId_SecondaryPackaging, mat5);

            var groupBySecondaryPackaging = listSecondaryPackaging.GroupBy(item => item.ID)
                         .Select(group => new { ID = group.Key, Items = group.ToList() })
                         .ToList();

            using (var context = new ARTWORKEntities())
            {
                foreach (var item in groupBySecondaryPackaging)
                {
                    var PRODUCT_CODE = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 2).FirstOrDefault().ValStr;
                    if (PRODUCT_CODE == mat5)
                    {
                        var PRODUCT_DESCRIPTION = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 3).FirstOrDefault().ValStr;

                        var CONTAINER_TYPE = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 16).FirstOrDefault().ValStr;
                        var NET_WEIGHT = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 99).FirstOrDefault().ValStr;
                        var DRAINED_WEIGHT = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 100).FirstOrDefault().ValStr;
                        var PRIMARY_SIZE = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 69).FirstOrDefault().ValStr;
                        var LID_TYPE = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 63).FirstOrDefault().ValStr;

                        var AMBIENT_PACKING_STYLE = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 66).FirstOrDefault().ValStr;
                        var PACK_SIZE = listSecondaryPackaging.Where(m => m.ID == item.ID && m.AttrID == 65).FirstOrDefault().ValStr;

                        if (!string.IsNullOrEmpty(PRODUCT_CODE))
                        {
                            XECM_M_PRODUCT5 product = new XECM_M_PRODUCT5();
                            var chk = XECM_M_PRODUCT5_SERVICE.GetByItem(new XECM_M_PRODUCT5() { PRODUCT_CODE = PRODUCT_CODE.Trim() }, context);
                            if (chk.Count == 1)
                            {
                                product.XECM_PRODUCT5_ID = chk.FirstOrDefault().XECM_PRODUCT5_ID;
                            }

                            if (!string.IsNullOrEmpty(PRODUCT_CODE)) product.PRODUCT_CODE = PRODUCT_CODE.Trim();
                            if (!string.IsNullOrEmpty(PRODUCT_DESCRIPTION)) product.PRODUCT_DESCRIPTION = PRODUCT_DESCRIPTION.Trim();
                            if (!string.IsNullOrEmpty(NET_WEIGHT)) product.NET_WEIGHT = NET_WEIGHT.Trim();
                            if (!string.IsNullOrEmpty(DRAINED_WEIGHT)) product.DRAINED_WEIGHT = DRAINED_WEIGHT.Trim();
                            if (!string.IsNullOrEmpty(PRIMARY_SIZE)) product.PRIMARY_SIZE = PRIMARY_SIZE.Trim();
                            if (!string.IsNullOrEmpty(CONTAINER_TYPE)) product.CONTAINER_TYPE = CONTAINER_TYPE.Trim();
                            if (!string.IsNullOrEmpty(LID_TYPE)) product.LID_TYPE = LID_TYPE.Trim();
                            if (!string.IsNullOrEmpty(AMBIENT_PACKING_STYLE)) product.PACKING_STYLE = AMBIENT_PACKING_STYLE.Trim();
                            if (!string.IsNullOrEmpty(PACK_SIZE)) product.PACK_SIZE = PACK_SIZE.Trim();

                            product.CREATE_BY = -2;
                            product.UPDATE_BY = -2;

                            Console.WriteLine("Product5 : " + product.PRODUCT_CODE + " " + product.PRODUCT_DESCRIPTION);
                            XECM_M_PRODUCT5_SERVICE.SaveOrUpdateNoLog(product, context);

                            twoPFromMat5(product.PRODUCT_CODE, context, null);
                            threePFromMat5(product.PRODUCT_CODE, context);
                        }
                    }
                }
            }
        }

        private static void twoPFromMat5(string mat5, ARTWORKEntities context)
        {
            var list = XECM_M_PRODUCT5_SERVICE.GetByItem(new XECM_M_PRODUCT5() { PRODUCT_CODE = mat5 }, context);
            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.PACKING_STYLE) && !string.IsNullOrEmpty(item.PACK_SIZE))
                {
                    SAP_M_2P model = new SAP_M_2P();
                    model.IS_ACTIVE = "X";
                    model.PACKING_SYLE_VALUE = item.PACKING_STYLE;
                    model.PACKING_SYLE_DESCRIPTION = item.PACKING_STYLE;

                    model.PACK_SIZE_VALUE = item.PACK_SIZE;
                    model.PACK_SIZE_DESCRIPTION = item.PACK_SIZE;

                    var chk = SAP_M_2P_SERVICE.GetByItem(model, context);
                    if (chk.Count == 0)
                    {
                        model.CREATE_BY = -2;
                        model.UPDATE_BY = -2;

                        SAP_M_2P_SERVICE.SaveOrUpdateNoLog(model, context);
                    }
                }
            }
        }

        private static void threePFromMat5(string mat5, ARTWORKEntities context)
        {
            var list = XECM_M_PRODUCT5_SERVICE.GetByItem(new XECM_M_PRODUCT5() { PRODUCT_CODE = mat5 }, context);
            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.CONTAINER_TYPE) && !string.IsNullOrEmpty(item.LID_TYPE) && !string.IsNullOrEmpty(item.PRIMARY_SIZE))
                {
                    SAP_M_3P model = new SAP_M_3P();
                    model.IS_ACTIVE = "X";
                    model.CONTAINER_TYPE_VALUE = item.CONTAINER_TYPE;
                    model.CONTAINER_TYPE_DESCRIPTION = item.CONTAINER_TYPE;

                    model.LID_TYPE_VALUE = item.LID_TYPE;
                    model.LID_TYPE_DESCRIPTION = item.LID_TYPE;

                    model.PRIMARY_SIZE_VALUE = item.PRIMARY_SIZE;
                    model.PRIMARY_SIZE_DESCRIPTION = item.PRIMARY_SIZE;

                    var chk = SAP_M_3P_SERVICE.GetByItem(model, context);
                    if (chk.Count == 0)
                    {
                        model.CREATE_BY = -2;
                        model.UPDATE_BY = -2;

                        SAP_M_3P_SERVICE.SaveOrUpdateNoLog(model, context);
                    }
                }
            }
        }

    }
}
