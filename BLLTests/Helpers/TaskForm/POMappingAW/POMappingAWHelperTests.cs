using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Model;

namespace BLL.Helpers.Tests
{
    [TestClass()]
    public class POMappingAWHelperTests
    {
        [TestMethod()]
        public void GetPOMappingAWTest()
        {
            ART_WF_ARTWORK_MAPPING_PO_REQUEST param = new ART_WF_ARTWORK_MAPPING_PO_REQUEST();
            ART_WF_ARTWORK_MAPPING_PO_2 data = new ART_WF_ARTWORK_MAPPING_PO_2();

            data.ARTWORK_NO = "AW-R-2018-00000038";
            param.data = data;

            POMappingAWHelper.GetPOMappingAW(param);
        }
    }
}