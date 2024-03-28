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
    public class ArtworkProcessHelperTests
    {
        [TestMethod()]
        public void GetProcess()
        {

            ART_WF_ARTWORK_PROCESS_REQUEST param = new ART_WF_ARTWORK_PROCESS_REQUEST();
            ART_WF_ARTWORK_PROCESS_RESULT results = new ART_WF_ARTWORK_PROCESS_RESULT();
            ART_WF_ARTWORK_PROCESS_2 data = new ART_WF_ARTWORK_PROCESS_2();
            data.ARTWORK_SUB_ID = 3553;

            param.data = data;

            results = ArtworkProcessHelper.GetProcess(param);
        }
    }
}