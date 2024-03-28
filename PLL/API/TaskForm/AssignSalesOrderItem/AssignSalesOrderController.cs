using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DAL.Model;
using BLL.Services;
using BLL.Helpers;
using DAL;

namespace PLL.API
{
    public class AssignSalesOrderItemController : ApiController
    {
        [Route("api/taskform/salesorderitem/get")]
        public ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT GetSalesOrderDetail([FromUri]ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST param)
        {
            return SalesOrderHelper.GetSODetail_NewData(param);
        }

        [Route("api/taskform/salesorderitem/popup")]
        public ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT GetSalesOrderItemPopup([FromUri]ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST param)
        {
            using (var context = new ARTWORKEntities())
            {
                using (CNService.IsolationLevel(context))
                {
                    param.data.CURRENT_USER_ID = CNService.getCurrentUser(context);
                }
            }
            return SalesOrderHelper.GetSalesOrderItemPopup(param);
        }

        [Route("api/taskform/salesorderfoc/popup")]
        public ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT GetSalesOrderFOCPopup([FromUri]ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST param)
        {
            //param.data.CURRENT_USER_ID = CNService.getCurrentUser();
            return SalesOrderHelper.GetSODetail_FOCData(param);
        }

        [Route("api/taskform/salesorderitem/save")]
        public ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT PostSalesOrderItemDetail(ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST_LIST param)
        {
            return SalesOrderHelper.SaveSalesOrderItemDetail(param);
        }

        [Route("api/taskform/salesorderfoc/save")]
        public ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT PostSalesOrderFOC(ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST_LIST param)
        {
            return SalesOrderHelper.SaveSalesOrderFOC(param);
        }

        [Route("api/taskform/salesorderitem/delete")]
        public ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT DeleteSalesOrderItemDetail(ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST param)
        {
            return SalesOrderHelper.DeleteSalesOrderItemDetail(param);
        }

        [Route("api/taskform/salesorderitem/validate")]
        public ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT PostSalesOrderItemValidate(ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST_LIST param)
        {
            return SalesOrderHelper.ValidateSalesOrderItemPopup(param);
        }

        [Route("api/taskform/salesorderitem/showchange")]
        public SALES_ORDER_CHANGE_RESULT GetSalesOrderItemChange([FromUri]SALES_ORDER_CHANGE_REQUEST param)
        {
            return SalesOrderHelper.CheckSalesOrderChange(param);
        }

        [Route("api/taskform/salesorderitem/acceptchange")]
        public ART_WF_ARTWORK_PROCESS_SO_DETAIL_RESULT PostAcceptSalesOrderItemChange(ART_WF_ARTWORK_PROCESS_SO_DETAIL_REQUEST param)
        {
            return SalesOrderHelper.AcceptSalesOrderItemChange(param);
        }
    }
}
