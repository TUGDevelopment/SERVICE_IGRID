using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Model;
using BLL.Services;
using WebServices.Model;
using DAL;
using System.Data.Entity;
using BLL.Helpers;

namespace WebServices.Helper.Common
{
    public static class PG_Helper
    {

        public static void UpdatePGData( IGRID_M_OUTBOUND_HEADER paramHeader)
        {
            //ART_WF_MOCKUP_PROCESS_PG _pg = new ART_WF_MOCKUP_PROCESS_PG();
            

            //int? paUserID = 0;
            //int? pgUserID = 0;

            //paUserID = CNService.GetUserID(paramHeader.PA_USER_NAME);
            //pgUserID = CNService.GetUserID(paramHeader.PG_USER_NAME);

            //if (paUserID != null)
            //{
            //    _pg.PA_USER_ID = paUserID;
            //}
            
        }
    }
}