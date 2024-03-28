using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using BLL.Services;
using DAL.Model;

namespace BLL.Helpers.Tests
{
    [TestClass()]
    public class FormNumberHelperTests
    {
        [TestMethod()]
        public void GenCheckListNoTest()
        {
            try
            {
                string value = "";
                string param = "NEW STICKER";

                if (String.IsNullOrEmpty(param))
                {
                    // "";
                }

                if (param.Contains("MULTI"))
                {
                    string[] values = param.Split(new string[] { "MULTI" }, StringSplitOptions.None);

                    value = values[1].Trim();
                }
                else
                {
                    string[] values = param.Split(new string[] { "NEW" }, StringSplitOptions.None);
                    value = values[1].Trim();
                }

                using (var context = new ARTWORKEntities())
                {
                    FormNumberHelper.UpdateCheckListNo(85, context);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [TestMethod()]
        public void GetMonthDifferenceTest()
        {
            DateTime dtNow = DateTime.Now;
            DateTime dtRDD = DateTime.Now;

            dtRDD = Convert.ToDateTime("01/04/2018");
            int month = FormNumberHelper.GetMonthDifference(dtNow, dtRDD);
        }

        [TestMethod()]
        public void StampToPDF()
        {
            //450478 //for 0 degrees
            //449927 //for 90 degrees
            //451619 //for 180 degrees,456051,457369,520630,520629
            var token = CWSService.getAuthToken();
            var downloadStream = CWSService.downloadFile(456051, token);

            var so = "500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100),500148971(100)";
            var mat3 = "3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE/3000000000000003WE";
            var mat5 = "Mat. 5F0RW181N000000300";

            var filePath = CNService.StampToPDF(so, mat3, mat5, downloadStream, 456051);
        }

        [TestMethod()]
        public void InsertMaterialLock()
        {
            //var list = ART_WF_ARTWORK_MATERIAL_LOCK_SERVICE.GetAll();
            //foreach (var item in list)
            //{
            //    var listDetails = ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL_SERVICE.GetByItem(new ART_WF_ARTWORK_MATERIAL_LOCK_DETAIL() { MATERIAL_LOCK_ID = item.MATERIAL_LOCK_ID });
            //    foreach (var itemDetails in listDetails)
            //    {
            //        CNService.InsertMaterialLock(itemDetails.MATERIAL_NO, itemDetails.SALES_ORDER_NO);
            //    }
            //}
            //CNService.InsertMaterialLock("5GZ01280N000000100", "000");


            string dt = DateTime.Now.ToString("ddMMyyyy hhmmss");


            //var mat5 = "5F063236N000000401";
            //var mat5NoVersion = CNService.SubString(mat5, 16);
            //var ss = mat5.Substring(16, 2);

            //var mat5NoVersionN = CNService.SubString(mat5.Remove(8, 1).Insert(8, "N"), 16);
            //var mat5NoVersionC = CNService.SubString(mat5.Remove(8, 1).Insert(8, "C"), 16);

            //var matlockno = "5F063037N000001100,5F063037N000001200,5F063037N000001300,5F063037N000001400,5F063037N000001600,5F063037N000001700,5F063037N000001800,5F063037N000001900,5F063037N000002100,5F063037N000002200,5F063037N000002300,5F063037N000002400,5F063037N000004100,5F063236N000000101,5F063236N000000201,5F063236N000000301,5F063236N000000401,5F081050N000000100,5F081071N000000200,5F0D1153N000005201,5F0DM085N000001100,5F0DM085N000001300,5F0NM085N000000500,5F0NM085N000000600,5F0NZ319N000000100,5F0PE166N000000503,5F28R403N000000102,5FZ01009N000000101,5FZ01050N000000100,5FZ01050N000000700,5FZ01050N000001000,5FZ01054N000000100,5FZ01054N000000300,5FZ01054N000000401,5FZ01153N000000300,5FZ01163N000000100,5FZ01163N000000200,5FZ01166N000000100,5FZ01217N000000100,5FZ01250N000000100,5FZ01347N000000100,5FZ01347N000000200,5FZ01405N000000100,5FZ01405N000000200,5FZ01405N000000300,5FZ01405N000000400,5K063181N000000301,5K081097N000000400,5K081097N000000500,5K0A0097N000000202,5K0D5083N000000100,5K0D5083N000000200,5K0DM085N000000301,5K0DM085N000000401,5K0DM105N000000102,5K0DM105N000000202,5K0DM181N000000102,5K0DM181N000000303,5K0DM181N000000403,5K0DM181N000000603,5K0DM181N000000801,5K0K1129N000000100,5K0NM085N000000500,5K0NM085N000000600,5K0P8088N000000101,5K0P8088N000000201,5K1P7250N000000101,5K26C097N000000102,5K280081N000000701,5K280081N000000801,5K280081N000000901,5K280081N000001001,5K280081N000001101,5K280081N000001201,5K280081N000001901,5K280081N000002001,5K280081N000002101,5K280081N000002201,5K280081N000002301,5K280081N000002401,5K280146N000003101,5K280146N000003201,5K280146N000003301,5K280146N000003401,5K280146N000003501,5K280146N000003601,5KZ01217N000000100,5KZ01250N000000500,5KZ01280N000000100,5KZ01347N000000100,5KZ01347N000000200";
            //var matlockno = "";
            //var listmatlock = matlockno.Split(',');
            //foreach (var i in listmatlock)
            //    CNService.InsertMaterialLock(i, "000");

            //CNService.InsertMaterialLock("5F063236N000000301", "500149480");
            //CNService.InsertMaterialLock("5F063236N000000401", "500149481");

          



        }

        [TestMethod()]
        public void Encrypt()
        {
            //var vendor1234 = EncryptionService.Encrypt("vendor1234");
            //var customer1234 = EncryptionService.Encrypt("customer1234");

            //var init1234 = EncryptionService.Encrypt("init1234");
            //var q1234 = EncryptionService.Encrypt("1234");
        }


        [TestMethod()]
        public void LongTxt()
        {
            List<int> temp = new List<int>();
            MapperServices.Initialize();
            using (var context = new ARTWORKEntities())
            {
                temp = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                        select s.ARTWORK_SUB_ID).Distinct().ToList().OrderByDescending(m => m).ToList();
            }

            foreach (var ss in temp)
            {
                using (var context = new ARTWORKEntities())
                {
                    var soDetails = (from s in context.ART_WF_ARTWORK_PROCESS_SO_DETAIL
                                     where s.ARTWORK_SUB_ID == ss
                                     select s).ToList();

                    var soDetail2 = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
                    soDetail2 = MapperServices.ART_WF_ARTWORK_PROCESS_SO_DETAIL(soDetails[0]);

                    SalesOrderHelper.DeleteAssignSalesOrder(soDetail2, context);
                    SalesOrderHelper.CopyAssignSalesOrder(soDetail2, context);
                }
            }
        }
    }
}

