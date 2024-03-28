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
    public class PAFormHelperTests
    {
        [TestMethod()]
        public void GetSuggestMaterialTest()
        {
            SAP_M_ORDER_BOM_REQUEST param = new SAP_M_ORDER_BOM_REQUEST();
            SAP_M_ORDER_BOM_2 data = new SAP_M_ORDER_BOM_2();

            data.ARTWORK_SUB_ID = 1299;
            data.ARTWORK_REQUEST_ID = 448;

            param.data = data;

            PAFormHelper.GetSuggestMaterial(param);
            // Assert.Fail();
        }

        [TestMethod()]
        public void GetSuggestMaterialTest1()
        {
            SAP_M_ORDER_BOM_REQUEST param = new SAP_M_ORDER_BOM_REQUEST();
            SAP_M_ORDER_BOM_2 data = new SAP_M_ORDER_BOM_2();

            data.ARTWORK_SUB_ID = 606;
            data.ARTWORK_REQUEST_ID = 192;
            param.data = data;

            PAFormHelper.GetSuggestMaterial(param);
            //606
        }

        [TestMethod()]
        public void CopyPADataFromAWTest()
        {
            ART_WF_ARTWORK_PROCESS_PA_REQUEST param = new ART_WF_ARTWORK_PROCESS_PA_REQUEST();
            ART_WF_ARTWORK_PROCESS_PA_2 data = new ART_WF_ARTWORK_PROCESS_PA_2();

            data.ARTWORK_SUB_ID = 6324;
            data.ARTWORK_NO = "AW-R-2019-00001251";
            data.UPDATE_BY = -1;

            param.data = data;
            PAFormHelper.CopyPADataFromAW(param);
        }
    }
}