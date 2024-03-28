using BLL.WebServices.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WebServices.Model;

namespace PLL.WS.SAP
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SD131" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SD131.svc or SD131.svc.cs at the Solution Explorer and start debugging.
    public class SD131 : ISD131
    {
        public SERVICE_RESULT_MODEL SD131_OUTBOUND_ORDER_BOM(ORDER_BOM_MODEL param) //
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();

            Results = SD_131_Helper.SaveOrderBom(param);

            return Results;
        }
    }
}
