using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Tests
{
    [TestClass()]
    public class EncryptionServiceTests
    {
        [TestMethod()]
        public void DecryptTest()
        {
            var Password_ = EncryptionService.Decrypt("U2Gpzl0tmnzxZ/56kH6uMQ==");
            Assert.Fail();
        }
    }
}