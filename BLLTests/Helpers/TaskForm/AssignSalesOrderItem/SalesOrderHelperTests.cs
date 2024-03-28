using Microsoft.VisualStudio.TestTools.UnitTesting;
using BLL.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using DAL.Model;
using BLL.Services;

namespace BLL.Helpers.Tests
{
    [TestClass()]
    public class SalesOrderHelperTests
    {
        [TestMethod()]
        public void SaveSalesOrderItemDetailTest()
        {

        }

        [TestMethod()]
        public void SaveAssignSalesOrder_Manual()
        {
            string salesOrdrNo = "";
            string artworkNo = "";

            using (var context = new ARTWORKEntities())
            {
                using (var dbContextTransaction = CNService.IsolationLevel(context))
                {
                    var artworkItemID = (from i in context.ART_WF_ARTWORK_REQUEST_ITEM
                                         where i.REQUEST_ITEM_NO == artworkNo
                                         select i.ARTWORK_ITEM_ID).FirstOrDefault();

                    var stepPAID = context.ART_M_STEP_ARTWORK.Where(w => w.STEP_ARTWORK_CODE == "SEND_PA").Select(s => s.STEP_ARTWORK_ID).FirstOrDefault();

                    var process = (from p in context.ART_WF_ARTWORK_PROCESS
                                   where p.ARTWORK_ITEM_ID == artworkItemID
                                      && p.CURRENT_STEP_ID == stepPAID
                                   select p).FirstOrDefault();

                    var soHeader = (from h in context.SAP_M_PO_COMPLETE_SO_HEADER
                                    where h.SALES_ORDER_NO == salesOrdrNo
                                    select h).FirstOrDefault();

                    var soItem = (from i in context.SAP_M_PO_COMPLETE_SO_ITEM
                                  where i.PO_COMPLETE_SO_HEADER_ID == soHeader.PO_COMPLETE_SO_HEADER_ID
                                  select i).FirstOrDefault();

                    ART_WF_ARTWORK_PROCESS_SO_DETAIL soDetail = new ART_WF_ARTWORK_PROCESS_SO_DETAIL();
                    //soDetail.ARTWORK_REQUEST_ID = process.ARTWORK_REQUEST_ID;
                    //soDetail.ARTWORK_SUB_ID = process.ARTWORK_SUB_ID;
                    //soDetail.SALES_ORDER_NO = soHeader.SALES_ORDER_NO;
                    //soDetail.SALES_ORDER_ITEM = soItem.ITEM.ToString();
                    //soDetail.MATERIAL_NO = soItem.PRODUCT_CODE;
                    //soDetail.BOM_NO
                    //soDetail.BOM_ID
                    //soDetail.CREATE_BY
                    //soDetail.UPDATE_BY

                }
            }
        }

        [TestMethod()]
        public void GetSODetail_FOCDataTest()
        {
            ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST req = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST();
            ART_WF_ARTWORK_PROCESS_SO_DETAIL_2 so = new ART_WF_ARTWORK_PROCESS_SO_DETAIL_2();
            List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2> data = new List<ART_WF_ARTWORK_PROCESS_SO_DETAIL_2>();

            so.ARTWORK_SUB_ID = 4820;
            data.Add(so);

            req.data = so;

            SalesOrderHelper.GetSODetail_FOCData(req);
        }
    }
}