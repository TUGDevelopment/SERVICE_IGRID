using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServices.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Services;
using DAL.Model;
using System.Text.RegularExpressions;

namespace WebServices.Helper.Tests
{
    [TestClass()]
    public class MM_65_HeplerTests
    {
        [TestMethod()]
        public void GetDataTest()
        {

            MM65_REQUEST req = new MM65_REQUEST();
            MM65_RESULT res = new MM65_RESULT();
            MM65_MODEL model = new MM65_MODEL();

           model.ARTWORK_SUB_ID = 1683;
            model.ACTION = "UPDATE";
            model.CHANGE_POINT = "0";
            model.RECORD_TYPE = "U";
            model.REFERENCE_MATERIAL = "";

            //model.ARTWORK_SUB_ID = 13;
            //model.ACTION = "REQUEST";
            //model.CHANGE_POINT = "0";
            //model.RECORD_TYPE = "I";
            //model.REFERENCE_MATERIAL = "";


            req.data = model;

            res = MM_65_Hepler.RequestMaterial(req);

            ///

            //string dateStr = "";
            //string timeStr = "";
            //dateStr = DateTime.Now.ToString("yyyyMMdd");
            //timeStr = DateTime.Now.ToString("HH:mm:ss");
        }

        [TestMethod()]
        public void GetDataTestList()
        {

            List<int> subIDs = new List<int>();

            subIDs.Add(1797);
            subIDs.Add(1653);
          
   

            MM65_REQUEST req = new MM65_REQUEST();
            MM65_RESULT res = new MM65_RESULT();
            MM65_MODEL model = new MM65_MODEL();

            foreach (var item in subIDs)
            {


                req = new MM65_REQUEST();
                res = new MM65_RESULT();
                model = new MM65_MODEL();

                model.ARTWORK_SUB_ID = item;
                model.ACTION = "REQUEST";
                model.CHANGE_POINT = "0";
                model.RECORD_TYPE = "I";
                model.REFERENCE_MATERIAL = "";


                req.data = model;

                res = MM_65_Hepler.RequestMaterial(req);

                //string dateStr = "";
                ///
            }

            //string dateStr_2 = "";
            //string timeStr = "";
            //dateStr = DateTime.Now.ToString("yyyyMMdd");
            //timeStr = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}