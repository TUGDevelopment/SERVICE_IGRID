using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace BLL.Services.Tests
{
    [TestClass()]
    public class CNServiceTests
    {
        [TestMethod()]
        public void FindArtworkSubIdTest()
        {
            //int subID = 189;

            //var SubIDList = CNService.FindArtworkItemId(subID);
        }

        [TestMethod()]
        public void RepairMaterialBOMTest()
        {

            MapperServices.Initialize();
            using (var context = new ARTWORKEntities())
            {
                using (var dbContextTransaction = CNService.IsolationLevel(context))
                {
                    //  CNService.RepairMaterialBOM("5J053246N000000100", context);
                    //CNService.RepairMaterialBOM("5K1UR181N000000301", context);

                    //  dbContextTransaction.Commit();
                }
            }
        }

        //[TestMethod()]
        //public void updateIS_HAS_FILESTest()
        //{
        //    using (var context = new ARTWORKEntities())
        //    {
        //        using (var dbContextTransaction = CNService.IsolationLevel(context))
        //        {
        //            CNService.updateIS_HAS_FILES("5F0NH103N000000200", context);
        //        }
        //    }
        //}

        [TestMethod()]
        public void InsertMaterialLockTest()
        {
            using (var context = new ARTWORKEntities())
            {
                 
                    CNService.InsertMaterialLock("5J0D1153N000006408", "500287632", "3HNNFB2UJ2AARPMMED", context,""); 
            }
            string aa = "";
        }
    }
}