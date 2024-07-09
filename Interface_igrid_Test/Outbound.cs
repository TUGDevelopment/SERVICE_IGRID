using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
//using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections;
using System.Data;
//using Interface_igrid;



namespace Interface_igrid_Test
{
    public class DoesNotThrow { }
    public class IsNotEmpty { }

    [TestClass]
    public class Outbound
    {
        [TestMethod]
        public void SQ01_ListMAT()
        {
            // Arrange
            DataTable results = new DataTable();
            results.Columns.Add("Characteristic Name txtSP$00005-LOW.text", typeof(string));
            results.Columns.Add("txtSP$00003 - LOW.text", typeof(string));            
            results.Rows.Add("test","test2");

          

            const string INTERFACE_MAME = "SQ01_ListMAT";

            try
            {


                // Act and Assert
                Interface_igrid.Program.SQ01_ListMAT(results);

                Assert.IsNotNull(results);

                //Assert.AreEqual("expectedValue", results); // Replace "expectedValue" with the expected result


            }
            catch (Exception ex)
            {
                throw new Exception("Error Export Service: " + ex.Message + "<br />" + ex.StackTrace);
            }

        }
        [TestMethod]
        public void CT04()
        {
        }
        [TestMethod]
        public void MM01_CreateMAT_ExtensionPlant()
        {
        }
        [TestMethod]
        public void BAPI_UpdateMATCharacteristics()
        {
        }
        [TestMethod]
        public void CLMM_ChangeMatClass()
        {
        }
        [TestMethod]
        public void MM02_ImpactMatDesc()
        {
        }
                           
        [TestMethod]
        public void Import_SQ01_Should_Return_Valid_String()
        {
            // Arrange
            string file = "testFile.txt";

            // Act
            //string result = Program.Import_SQ01(file);

            //// Assert
            //Assert.IsNotNull(result);
            ////Assert.IsNotEmpty(result);
            //Assert.AreEqual("expectedValue", result); // Replace "expectedValue" with the expected result
        }

        [TestMethod]
        public void SQ01_ListMAT_Should_Not_Throw_Exception()
        {
            // Arrange
            DataTable results = new DataTable();

            // Act and Assert
            //Assert.DoesNotThrow(() => Program.SQ01_ListMAT(results));
        }

        [TestMethod]
        public void CT04_Should_Not_Throw_Exception()
        {
            // Arrange
            DataTable results = new DataTable();

            // Act and Assert
            //Assert.DoesNotThrow(() => Program.CT04(results));
        }

        [TestMethod]
        public void MM01_CreateMAT_ExtensionPlant_Should_Not_Throw_Exception()
        {
            // Arrange
            DataTable results = new DataTable();

            // Act and Assert
            //Assert.DoesNotThrow(() => Program.MM01_CreateMAT_ExtensionPlant(results));
        }

        [TestMethod]
        public void BAPI_UpdateMATCharacteristics_Should_Not_Throw_Exception()
        {
            // Arrange
            DataTable results = new DataTable();

            // Act and Assert
            //Assert.DoesNotThrow(() => Program.BAPI_UpdateMATCharacteristics(results));
        }

        [TestMethod]
        public void CLMM_ChangeMatClass_Should_Not_Throw_Exception()
        {
            // Arrange
            DataTable results = new DataTable();

            // Act and Assert
            //Assert.DoesNotThrow(() => Program.CLMM_ChangeMatClass(results));
        }

        [TestMethod]
        public void MM02_ImpactMatDesc_Should_Not_Throw_Exception()
        {
            // Arrange
            DataTable results = new DataTable();

            // Act and Assert
            //Assert.DoesNotThrow(() => Program.MM02_ImpactMatDesc(results));
        }

        [TestMethod]
        public void GetData_Should_Return_Valid_DataSet()
        {
            //// Arrange
            //string sp = "spQuery";
            //string field = "@Material";
            //string sName = "X";

            //// Act
            //DataSet result = Program.GetData(sp, field, sName);

            //// Assert
            //Assert.IsNotNull(result);
            ////Assert.IsInstanceOf<DataSet>(result);
            //Assert.AreEqual(1, result.Tables.Count);
        }

        [TestMethod]
        public void builditems_Should_Return_Valid_DataTable()
        {
            //// Arrange
            //string data = "testData";

            //// Act
            //DataTable result = Program.builditems(data);

            //// Assert
            //Assert.IsNotNull(result);
            ////Assert.IsInstanceOf<DataTable>(result);
        }

        [TestMethod]
        public void ToCSV_Should_Not_Throw_Exception()
        {
            // Arrange
            DataTable dtDataTable = new DataTable();
            string strFilePath = "testFile.csv";

            // Act and Assert
            //Assert.DoesNotThrow(() => Program.ToCSV(dtDataTable, strFilePath));
        }

        [TestMethod]
        public void CT04_Inbound_Should_Not_Throw_Exception()
        {
            // Arrange
            string data = "testData";

            // Act and Assert
            //Assert.DoesNotThrow(() => Program.CT04_Inbound(data));
        }

        [TestMethod]
        public void Inbound_Should_Not_Throw_Exception()
        {
            // Arrange
            string data = "testData";

            // Act and Assert
            //Assert.DoesNotThrow(() => Program.Inbound(data));
        }

        [TestMethod]
        public void CLMM_ChangeMatClassold_Should_Not_Throw_Exception()
        {
            // Arrange
            DataTable results = new DataTable();

            // Act and Assert
            //Assert.DoesNotThrow(() => Program.CLMM_ChangeMatClassold(results));
        }
    

}
}
