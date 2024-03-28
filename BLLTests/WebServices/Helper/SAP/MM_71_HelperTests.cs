using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL.WebServices.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.WebServices.Helper.Tests
{
    [TestClass()]
    public class MM_71_HelperTests
    {
        [TestMethod()]
        public void MappingPOWithArtworkTest()
        {
            MM_71_Helper.MappingPOWithArtwork("4100318667");
          //  MM_71_Helper.MappingPOWithArtwork("4100232204");
        }
    }
}