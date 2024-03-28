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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MM71" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MM71.svc or MM71.svc.cs at the Solution Explorer and start debugging.
    public class MM71 : IMM71
    {
        public SERVICE_RESULT_MODEL MM71_PO_OUTBOUND_IDOC(SAP_M_PO_IDOC_MODEL param)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();

            Results = MM_71_Helper.SavePO(param);

            return Results;
        }
    }
}
