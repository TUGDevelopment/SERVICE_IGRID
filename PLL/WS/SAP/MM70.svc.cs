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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MM70" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MM70.svc or MM70.svc.cs at the Solution Explorer and start debugging.
    public class MM70 : IMM70
    {
        public SERVICE_RESULT_MODEL MM70_OUTBOUND_PLANT(PLANT_MODEL param)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();

            Results = MM_70_Helper.SavePlant(param);

            return Results;
        }
    }
}
