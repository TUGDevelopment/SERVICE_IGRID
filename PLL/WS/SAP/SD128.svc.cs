using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Script.Services;
using WebServices.Helper;
using WebServices.Model;

namespace PLL.WS.SAP
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class SD128 : ISD128
    {
        public SERVICE_RESULT_MODEL SD128_OUTBOUND_LONG_TEXT(List<LONG_TXT> LONG_TXTS)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();

            Results = SD_128_Helper.SaveLongText(LONG_TXTS);

            return Results;
        }
    }
}
