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
    public class PGFormHelperTests
    {
        [TestMethod()]
        public void CopyDielineFileToArtworkTest()
        {
            ART_WF_ARTWORK_PROCESS_PG_REQUEST param = new ART_WF_ARTWORK_PROCESS_PG_REQUEST();
            ART_WF_ARTWORK_PROCESS_PG_2 data = new ART_WF_ARTWORK_PROCESS_PG_2();

            ART_WF_ARTWORK_PROCESS_PG_RESULT Results = new ART_WF_ARTWORK_PROCESS_PG_RESULT();

            // data.ARTWORK_SUB_ID = 1784;

            data.ARTWORK_SUB_ID = 1778;
            param.data = data;

            Results = PGFormHelper.CopyDielineFileToArtwork(param);
            //using (var context = new ARTWORKEntities())
            //{
            //    PGFormHelper.CopyDielineFileToArtwork(param, context);
            //}
        }
    }
}