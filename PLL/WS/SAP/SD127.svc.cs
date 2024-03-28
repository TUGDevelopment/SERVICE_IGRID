using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WebServices.Helper;
using WebServices.Model;

namespace PLL.WS.SAP
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SD127" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SD127.svc or SD127.svc.cs at the Solution Explorer and start debugging.
    public class SD127 : ISD127
    {
        public SERVICE_RESULT_MODEL SD127_PO_COMPLETE_ORDER(SAP_M_PO_COMPLETE_SO_MODEL param)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();

            Results = SD_127_Helper.SavePOCompleteSO(param);

            return Results;
        }
    }
}
