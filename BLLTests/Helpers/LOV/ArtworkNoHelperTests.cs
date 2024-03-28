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
    public class ArtworkNoHelperTests
    {
        [TestMethod()]
        public void GetArtworkNoTest()
        {
            ART_WF_ARTWORK_REQUEST_ITEM_REQUEST param = new ART_WF_ARTWORK_REQUEST_ITEM_REQUEST();
            ART_WF_ARTWORK_REQUEST_ITEM_RESULT results = new ART_WF_ARTWORK_REQUEST_ITEM_RESULT();
            ART_WF_ARTWORK_REQUEST_ITEM_2 data = new ART_WF_ARTWORK_REQUEST_ITEM_2();

            data.REQUEST_ITEM_NO = "";
            param.data = data;

            results =  ArtworkNoHelper.GetArtworkNo(param);

        }
    }
}