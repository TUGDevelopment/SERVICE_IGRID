using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;
using DAL;

namespace BLL.Helpers.Tests
{
    [TestClass()]
    public class MaterialLockReportHelperTests
    {
        [TestMethod()]
        public void GetMaterialLockReportTest()
        {
            ART_WF_ARTWORK_MATERIAL_LOCK_RESULT Results = new ART_WF_ARTWORK_MATERIAL_LOCK_RESULT();
            ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST param = new ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST();

            ART_WF_ARTWORK_MATERIAL_LOCK_2 data = new ART_WF_ARTWORK_MATERIAL_LOCK_2();

            // data.SEARCH_SOLD_TO = "124:ETABLISSEMENTS PAUL PAULET SAS";
            //data.SEARCH_SHIP_TO = "124:ETABLISSEMENTS PAUL PAULET SAS";
           // data.SEARCH_MATERIAL_NO = "5F1EF135N000000300";

            param.data = data;
            param.start = 1;
            param.length = 10;

            Results = MaterialLockReportHelper.GetMaterialLockReport(param);


        }

        [TestMethod()]
        public void UpdateMaterialLockReportTest()
        {
            ART_WF_ARTWORK_MATERIAL_LOCK_RESULT Results = new ART_WF_ARTWORK_MATERIAL_LOCK_RESULT();
            ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST_LIST param = new ART_WF_ARTWORK_MATERIAL_LOCK_REQUEST_LIST();

            List<ART_WF_ARTWORK_MATERIAL_LOCK_2> data = new List<ART_WF_ARTWORK_MATERIAL_LOCK_2>();
            ART_WF_ARTWORK_MATERIAL_LOCK_2 matLock = new ART_WF_ARTWORK_MATERIAL_LOCK_2();

            matLock.MATERIAL_LOCK_ID = 1;
            matLock.STATUS = "O";

            data.Add(matLock);
            // data.SEARCH_SOLD_TO = "124:ETABLISSEMENTS PAUL PAULET SAS";
            //data.SEARCH_SHIP_TO = "124:ETABLISSEMENTS PAUL PAULET SAS";
            // data.SEARCH_MATERIAL_NO = "5F1EF135N000000300";

            param.data = data;
            param.start = 1;
            param.length = 10;

            Results = MaterialLockReportHelper.UpdateMaterialLockReport(param);


        }
    }
}