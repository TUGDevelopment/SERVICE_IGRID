using BLL.Helpers;
using BLL.Services;
using DAL;
using DAL.Model;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebServices.Model;
using WebServices.Helper;
using System.Data;
using System.Text;
using System.Xml.Linq;

namespace PLL.API
{
    public class DashboardController : ApiController
    {
        [Route("api/taskform/igrid/result")]
        public IGRID_RESULT GetResult([FromUri] IGRID_REQUEST param)
        {
            return IGridFormHelper.GetWorkflowPending(param);
        }
        [Route("api/taskform/igrid/saveActive")]
        public AppObject_RESULT PostAppObject(AppObject_REQUEST param)
        {
            return IGridFormHelper.savedata(param);
        }
        [Route("api/taskform/igrid/Delete_UnusedJob")]
        public AppObject_RESULT PostDelete_UnusedJob(AppObject_REQUEST param)
        {
            return IGridFormHelper.Delete_UnusedJob(param);
        }
        //[Route("api/taskform/igrid/upload")]
        //public AppObject_RESULT PostUpload(AppObject_REQUEST param)
        //{
        //    return IGridFormHelper.Upload(param);
        //}

        [Route("api/taskform/igrid/assign")]
        public AppObject_RESULT PostAssign(AppObject_REQUEST param)
        {
            return IGridFormHelper.assign(param);
        }
        [Route("api/taskform/igrid/infogroup")]
        public IGRID_RESULT Getinfogroup([FromUri] IGRID_REQUEST param)
        {
            return IGridFormHelper.GetInfoGroup(param);
        }
        [Route("api/dashboard/incomingmockup")]
        public V_ART_WF_DASHBOARD_RESULT GetIncomingMockup([FromUri]V_ART_WF_DASHBOARD_REQUEST param)
        {
            return DashboardHelper.GetIncomingMockup(param);
        }
        [Route("api/dashboard/interface")]
        public void SendBuildInterface(string param)
        {
            string xmlData = HttpContext.Current.Server.MapPath("~/App_Data/SALES_ORDER_NO20210201102532.xml");
 

            //Path of the xml script  
            DataSet ds = new DataSet();//Using dataset to read xml file  
            ds.ReadXml(xmlData);
            //int i = 0;
            //for (i = 0; i <= ds.Tables[3].Rows.Count - 1; i++)
            //{
            //    string a= ds.Tables[0].Rows[i].ItemArray[2].ToString();
            //}
            var component = new List<COMPONENT>();
            component = (from rows in ds.Tables[5].AsEnumerable()
                         select new COMPONENT
                         {
                             COMPONENT_ITEM = string.Format("{0}", rows[0].ToString()),
                             COMPONENT_MATERIAL = string.Format("{0}", rows[1].ToString()),
                             DECRIPTION = string.Format("{0}", rows[2].ToString()),
                             QUANTITY = string.Format("{0}", rows[3].ToString()),
                             UNIT = string.Format("{0}", rows[4].ToString()),
                             STOCK = string.Format("{0}", rows[5].ToString()),
                             BOM_ITEM_CUSTOM_1 = string.Format("{0}", rows[6].ToString()),
                             BOM_ITEM_CUSTOM_2 = string.Format("{0}", rows[7].ToString()),
                             BOM_ITEM_CUSTOM_3 = string.Format("{0}", rows[8].ToString())
                         }).ToList();
            var items = new List<SO_ITEM>();
            items = (from rows in ds.Tables[3].AsEnumerable()
                     select new SO_ITEM
                     {
                         ITEM = string.Format("{0}", rows[0].ToString()),

                         PRODUCT_CODE = string.Format("{0}", rows[1].ToString()),
                         MATERIAL_DESCRIPTION = string.Format("{0}", rows[2].ToString()),
                         NET_WEIGHT = string.Format("{0}", rows[3].ToString()),
                         ORDER_QTY = string.Format("{0}", rows[4].ToString()),
                         ORDER_UNIT = string.Format("{0}", rows[5].ToString()),
                         ETD_DATE_FROM = string.Format("{0}", rows[6].ToString()),
                         ETD_DATE_TO = string.Format("{0}", rows[7].ToString()),
                         PLANT = string.Format("{0}", rows[8].ToString()),
                         OLD_MATERIAL_CODE = string.Format("{0}", rows[9].ToString()),
                         PACK_SIZE = string.Format("{0}", rows[10].ToString()),
                         VALUME_PER_UNIT = string.Format("{0}", rows[11].ToString()),
                         VALUME_UNIT = string.Format("{0}", rows[12].ToString()),
                         SIZE_DRAIN_WT = string.Format("{0}", rows[13].ToString()),
                         PROD_INSP_MEMO = string.Format("{0}", rows[14].ToString()),
                         REJECTION_CODE = string.Format("{0}", rows[15].ToString()),
                         REJECTION_DESCRIPTION = string.Format("{0}", rows[16].ToString()),
                         PORT = string.Format("{0}", rows[18].ToString()),
                         VIA = string.Format("{0}", rows[19].ToString()),
                         IN_TRANSIT_TO = string.Format("{0}", rows[20].ToString()),
                         BRAND_ID = string.Format("{0}", rows[21].ToString()),
                         BRAND_DESCRIPTION = string.Format("{0}", rows[22].ToString()),
                         ADDITIONAL_BRAND_ID = string.Format("{0}", rows[23].ToString()),
                         ADDITIONAL_BRAND_DESCRIPTION = string.Format("{0}", rows[24].ToString()),
                         PRODUCTION_PLANT = string.Format("{0}", rows[25].ToString()),
                         ZONE = string.Format("{0}", rows[26].ToString()),
                         COUNTRY = string.Format("{0}", rows[27].ToString()),
                         PRODUCTION_HIERARCHY = string.Format("{0}", rows[28].ToString()),
                         MRP_CONTROLLER = string.Format("{0}", rows[29].ToString()),
                         STOCK = string.Format("{0}", rows[30].ToString()),
                         ITEM_CUSTOM_1 = string.Format("{0}", rows[31].ToString()),
                         ITEM_CUSTOM_2 = string.Format("{0}", rows[32].ToString()),
                         ITEM_CUSTOM_3 = string.Format("{0}", rows[33].ToString()),
                         COMPONENTS = component
                     }).ToList();
            var products = new List<SO_HEADER>();
            products = (from rows in ds.Tables[1].AsEnumerable()
                        select new SO_HEADER
                        {
                               SALES_ORDER_NO = string.Format("{0}", rows[0].ToString()),
                               SOLD_TO = string.Format("{0}", rows[1].ToString()),
                               SOLD_TO_NAME = string.Format("{0}", rows[2].ToString()),
                               LAST_SHIPMENT_DATE = string.Format("{0}", rows[3].ToString()),
                               DATE_1_2 = string.Format("{0}", rows[4].ToString()),
                               CREATE_ON = string.Format("{0}", rows[5].ToString()),
                               RDD = string.Format("{0}", rows[6].ToString()),
                               PAYMENT_TERM = string.Format("{0}", rows[7].ToString()),
                               LC_NO = string.Format("{0}", rows[8].ToString()),
                               EXPIRED_DATE = string.Format("{0}", rows[9].ToString()),
                               SHIP_TO = string.Format("{0}", rows[10].ToString()),
                               SHIP_TO_NAME = string.Format("{0}", rows[11].ToString()),
                               SOLD_TO_PO = string.Format("{0}", rows[12].ToString()),
                               SHIP_TO_PO = string.Format("{0}", rows[13].ToString()),
                               SALES_GROUP = string.Format("{0}", rows[14].ToString()),
                               MARKETING_CO = string.Format("{0}", rows[15].ToString()),
                               MARKETING_CO_NAME = string.Format("{0}", rows[16].ToString()),
                               MARKETING = string.Format("{0}", rows[17].ToString()),
                               MARKETING_NAME = string.Format("{0}", rows[18].ToString()),
                               MARKETING_ORDER_SAP = string.Format("{0}", rows[19].ToString()),
                               MARKETING_ORDER_SAP_NAME = string.Format("{0}", rows[20].ToString()),
                               SALES_ORG = string.Format("{0}", rows[21].ToString()),
                               DISTRIBUTION_CHANNEL = string.Format("{0}", rows[22].ToString()),
                               DIVITION = string.Format("{0}", rows[23].ToString()),
                               SALES_ORDER_TYPE = string.Format("{0}", rows[24].ToString()),
                               HEADER_CUSTOM_1 = string.Format("{0}", rows[25].ToString()),
                               HEADER_CUSTOM_2 = string.Format("{0}", rows[26].ToString()),
                               HEADER_CUSTOM_3 = string.Format("{0}", rows[27].ToString()),

                            //SALES_ORDER_NO = string.Format("{0}", rows[0].ToString()), //Convert row to int  
                            //SOLD_TO = rows[1].ToString(),
                            //SOLD_TO_NAME = rows[2].ToString(),
                            //LAST_SHIPMENT_DATE = rows[3].ToString(),
                            //DATE_1_2 = rows[4].ToString(),
                            //CREATE_ON = rows[5].ToString(),
                            //RDD = rows[6].ToString(),
                            //PAYMENT_TERM = rows[7].ToString(),
                            //LC_NO = rows[8].ToString(),
                            //EXPIRED_DATE = rows[9].ToString(),
                            //SHIP_TO = rows[10].ToString(),
                            //SHIP_TO_NAME = rows[11].ToString(),
                            //SOLD_TO_PO = rows[12].ToString(),
                            //SHIP_TO_PO = rows[13].ToString(),
                            //SALES_GROUP = rows[14].ToString(),
                            //MARKETING_CO_NAME = rows[15].ToString(),
                            //MARKETING = rows[16].ToString(),
                            //MARKETING_NAME = rows[17].ToString(),
                            //MARKETING_ORDER_SAP = rows[18].ToString(),
                            //MARKETING_ORDER_SAP_NAME = rows[19].ToString(),                   
                            //SALES_ORG = rows[20].ToString(),
                            //DISTRIBUTION_CHANNEL = rows[21].ToString(),
                            //DIVITION = rows[22].ToString(),
                            //SALES_ORDER_TYPE = rows[23].ToString(),
                            //HEADER_CUSTOM_1 = rows[24].ToString(),
                            //HEADER_CUSTOM_2 = rows[25].ToString(),
                            //HEADER_CUSTOM_3 = rows[26].ToString(),
                            SO_ITEMS = items
                        }).ToList();
            //dataset xds = xds.ReadXml(rows[19].ToString());
            SAP_M_PO_COMPLETE_SO_MODEL Results = new SAP_M_PO_COMPLETE_SO_MODEL();
            Results.SO_HEADERS= products;
         
            SD_127_Helper.SavePOCompleteSO(Results);
        }
        [Route("api/dashboard/SOrepeat")]
        public List<Models.Client> SORepeat(string xEdit)
        {
            List<Models.Client> list = new List<Models.Client>();
            using (var context = new ARTWORKEntities())
            {
                var q = (from p in context.V_SAP_SALES_ORDER
                         select new Models.Client
                         {
                             Id = p.NUM,
                             Name = p.SALES_ORDER_NO,
                             Email = p.PAYMENT_TERM
                         }).Take(20);
                list = q.ToList();
            }
            return list;
        }
        [Route("api/dashboard/inbox")]
        public V_ART_WF_DASHBOARD_RESULT GetInbox([FromUri]V_ART_WF_DASHBOARD_REQUEST param)
        {
            if (param == null) param = new V_ART_WF_DASHBOARD_REQUEST();
            if (param.data == null) param.data = new V_ART_WF_DASHBOARD_2();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    param.data.CURRENT_USER_ID = CNService.getCurrentUser(context);
                }
            }
            return DashboardHelper.GetInbox(param);
        }

        [Route("api/dashboard/countmockupandartwork")]
        public DASHBOARD_GRAPH_MODEL GetCountMockupAndArtwork([FromUri]V_ART_WF_DASHBOARD_REQUEST param)
        {
            if (param == null) param = new V_ART_WF_DASHBOARD_REQUEST();
            if (param.data == null) param.data = new V_ART_WF_DASHBOARD_2();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    param.data.USER_ID = CNService.getCurrentUser(context);
                }
            }
            return DashboardHelper.CountMockupAndArtworkTransaction(param);
        }

        [Route("api/dashboard/incomingartwork")]
        public V_ART_WF_DASHBOARD_ARTWORK_RESULT GetIncomingArtwork([FromUri]V_ART_WF_DASHBOARD_ARTWORK_REQUEST param)
        {
            if (param == null) param = new V_ART_WF_DASHBOARD_ARTWORK_REQUEST();
            if (param.data == null) param.data = new V_ART_WF_DASHBOARD_ARTWORK_2();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    param.data.USER_ID = CNService.getCurrentUser(context);
                }
            }
            return DashboardHelper.GetIncomingArtwork(param);
        }

        [Route("api/dashboard/incomingartworkforpg")]
        public V_ART_WF_DASHBOARD_ARTWORK_RESULT GetIncomingArtworkForPG([FromUri]V_ART_WF_DASHBOARD_ARTWORK_REQUEST param)
        {
            return DashboardHelper.GetIncomingArtworkForPG(param);
        }

        [Route("api/dashboard/myincomingartworkforpg")]
        public V_ART_WF_DASHBOARD_ARTWORK_RESULT GetCountIncomingArtworkForPG([FromUri]V_ART_WF_DASHBOARD_ARTWORK_REQUEST param)
        {
            return DashboardHelper.GetCountIncomingArtworkForPG(param);
        }

        [Route("api/dashboard/povendor")]
        public SAP_M_PO_IDOC_RESULT GetPOViewForVendor([FromUri]SAP_M_PO_IDOC_REQUEST param)
        {
            if (param == null) param = new SAP_M_PO_IDOC_REQUEST();
            if (param.data == null) param.data = new SAP_M_PO_IDOC_2();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    param.data.CURRENT_USER_ID = CNService.getCurrentUser(context);
                }
            }
            return DashboardHelper.GetPOViewForVendor(param);
        }

        [Route("api/dashboard/incomingsalesordernew")]
        public V_SAP_SALES_ORDER_RESULT GetIncomingSalesOrderNew([FromUri]V_SAP_SALES_ORDER_REQUEST param)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    param.data.CURRENT_USER_ID = CNService.getCurrentUser(context);
                }
            }
            return DashboardHelper.GetSalesOrderNew(param);
        }

        [Route("api/dashboard/incomingsalesorderrepeat")]
        public V_SAP_SALES_ORDER_RESULT GetIncomingSalesOrderRepeat([FromUri]V_SAP_SALES_ORDER_REQUEST param)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    param.data.CURRENT_USER_ID = CNService.getCurrentUser(context);
                }
            }
            return DashboardHelper.GetSalesOrderRepeat(param);
        }

        [Route("api/dashboard/selectedrepeatso")]
        public ART_WF_ARTWORK_REQUEST_RESULT PostSelectedSORepeat(ART_WF_ARTWORK_REQUEST_REQUEST param)
        {
            var guid = Guid.NewGuid().ToString();
            var USERNAME = "";
            var userId = 0;
            using (ARTWORKEntities context = new ARTWORKEntities())
            {
                userId = CNService.getCurrentUser(context);
                var tempUser = ART_M_USER_SERVICE.GetByUSER_ID(userId, context);
                if (tempUser != null)
                    USERNAME = tempUser.USERNAME;
            }

            ART_WF_ARTWORK_REQUEST_REQUEST tempParam0 = JsonConvert.DeserializeObject<ART_WF_ARTWORK_REQUEST_REQUEST>(JsonConvert.SerializeObject(param));
            ART_WF_ARTWORK_REQUEST_REQUEST tempParam1 = JsonConvert.DeserializeObject<ART_WF_ARTWORK_REQUEST_REQUEST>(JsonConvert.SerializeObject(param));
            ART_WF_ARTWORK_REQUEST_REQUEST tempParam2 = JsonConvert.DeserializeObject<ART_WF_ARTWORK_REQUEST_REQUEST>(JsonConvert.SerializeObject(param));
            ART_WF_ARTWORK_REQUEST_REQUEST tempParam3 = JsonConvert.DeserializeObject<ART_WF_ARTWORK_REQUEST_REQUEST>(JsonConvert.SerializeObject(param));
            ART_WF_ARTWORK_REQUEST_REQUEST tempParam4 = JsonConvert.DeserializeObject<ART_WF_ARTWORK_REQUEST_REQUEST>(JsonConvert.SerializeObject(param));

            var res = DashboardHelper.CreateRFBySORepeat(tempParam0);
            //SaveLog(res.status, 0.ToString(), userId, USERNAME, guid);

            if (res.status == "E")
            {
                Random random = new Random();
                var sleep = random.Next(500, 2000);
                System.Threading.Thread.Sleep(sleep);

                res = DashboardHelper.CreateRFBySORepeat(tempParam1);
                //SaveLog(res.status, 1.ToString(), userId, USERNAME, guid);
            }
            if (res.status == "E")
            {
                Random random = new Random();
                var sleep = random.Next(500, 2000);
                System.Threading.Thread.Sleep(sleep);

                res = DashboardHelper.CreateRFBySORepeat(tempParam2);
                //SaveLog(res.status, 2.ToString(), userId, USERNAME, guid);
            }
            if (res.status == "E")
            {
                Random random = new Random();
                var sleep = random.Next(500, 2000);
                System.Threading.Thread.Sleep(sleep);

                res = DashboardHelper.CreateRFBySORepeat(tempParam3);
                //SaveLog(res.status, 3.ToString(), userId, USERNAME, guid);
            }
            if (res.status == "E")
            {
                Random random = new Random();
                var sleep = random.Next(1000, 3000);
                System.Threading.Thread.Sleep(sleep);

                res = DashboardHelper.CreateRFBySORepeat(tempParam4);
                SaveLog(res.status, 4.ToString(), userId, USERNAME, guid);
            }
            return res;
        }

        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//
        [Route("api/dashboard/selectedrepeatso_tuning")]
        public ART_WF_ARTWORK_REQUEST_RESULT_LIST PostSelectedSORepeat_Tuning(ART_WF_ARTWORK_REQUEST_REQUEST_LIST param)
        {


            var res = new ART_WF_ARTWORK_REQUEST_RESULT_LIST(); // DashboardHelper.CreateRFBySORepeat(tempParam0);

            try
            {
                var guid = Guid.NewGuid().ToString();
                var USERNAME = "";
                var userId = 0;
                var stampdatetime = DateTime.Now.ToString("yyyyMMddHHmmss.ffff");

                using (ARTWORKEntities context = new ARTWORKEntities())
                {
                    userId = CNService.getCurrentUser(context);
                    var tempUser = ART_M_USER_SERVICE.GetByUSER_ID(userId, context);
                    if (tempUser != null)
                        USERNAME = tempUser.USERNAME;
                }

          
               
                res.data = new List<ART_WF_ARTWORK_REQUEST_2>();

                var cnt = 1;
                foreach (var item in param.data)
                {
                    //// start for test delay show the output
                    //System.Threading.Thread.Sleep(5000);   
                    //var objAWRequest = item;

                    //objAWRequest.RESULT_CREATE_WF_WFNO = cnt.ToString() + ":" + objAWRequest.CONTROL_NAME;
                    //objAWRequest.RESULT_CREATE_WF_STATUS = "S";
                    //// end for test delay show the output
                 
                    var objAWRequest = DashboardHelper.CreateRFBySORepeat_Tuning(item, cnt, USERNAME,stampdatetime);
                  
                    res.data.Add(objAWRequest);
                    cnt = cnt + 1;

                }

                //SaveLog(res.status, 0.ToString(), userId, USERNAME, guid);

                if (res.data.Count == 1)
                {
                    if (res.data[0].CONTROL_NAME.IndexOf("SUBMIT") != -1)
                    {
                        if  (res.data[0].RESULT_CREATE_WF_STATUS == "E")
                        {
                            res.status = "E";
                            res.msg = res.data[0].RESULT_CREATE_WF_MESSAGE;
                        }
                        else
                        {
                            res.status = "S";
                        }
                    }
                    else
                        res.status = "S";
                }
                else
                {
                    res.status = "S";
                }
            }
            catch (Exception ex)
            {
                res.status = "E";
                res.msg = CNService.GetErrorMessage_SORepeat(ex, "PostSelectedSORepeat_Tuning");
            }

            return res;
        }
        //---------------------------- tuning performance sorepeat 2022 by aof---------------------------------//


        public void SaveLog(string status, string round, int userId, string USERNAME, string guid)
        {
            using (ARTWORKEntities context2 = new ARTWORKEntities())
            {
                ART_SYS_LOG error = new ART_SYS_LOG();
                error.CREATE_BY = userId;
                error.UPDATE_BY = userId;
                error.NEW_VALUE = USERNAME;
                error.OLD_VALUE = guid;
                if (status == "S") error.ERROR_MSG = "Rerun CreateRFBySORepeat Completed. (" + round + ")";
                if (status == "E") error.ERROR_MSG = "Rerun CreateRFBySORepeat Fail. (" + round + ")";
                error.TABLE_NAME = "Function GetErrorMessage [CNService]";
                error.ACTION = "E";
                ART_SYS_LOG_SERVICE.SaveNoLog(error, context2);
            }
        }

        [Route("api/dashboard/inbox/issoupdate")]
        [HttpPost]
        public V_ART_WF_DASHBOARD_RESULT GetIsSOUpdate(V_ART_WF_DASHBOARD_REQUEST param)
        {
            if (param == null) param = new V_ART_WF_DASHBOARD_REQUEST();
            if (param.data == null) param.data = new V_ART_WF_DASHBOARD_2();
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    param.data.CURRENT_USER_ID = CNService.getCurrentUser(context);
                }
            }
            return DashboardHelper.CheckSOIsUpdate(param);
        }
    }
}
