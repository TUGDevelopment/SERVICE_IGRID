using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Services;
using WebServices.Helper;
using WebServices.Model;

namespace PLL.WS.IGrid
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MM72" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MM72.svc or MM72.svc.cs at the Solution Explorer and start debugging.
    public class MM72 : IMM72
    {
        public SERVICE_RESULT_MODEL MM72_OUTBOUND_MATERIAL_CHARACTERISTIC(CHARACTERISTICS param)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();

            Results = MM_72_Hepler.SaveCharacteristics(param);

            return Results;
        }

        //[WebMethod]
        //public string test()
        //{
        //    return "Hello";
        //}
    }

}
