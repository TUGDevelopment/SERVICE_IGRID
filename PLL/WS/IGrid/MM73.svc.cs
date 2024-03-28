using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WebServices.Helper;
using WebServices.Model;

namespace PLL.WS.IGrid
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MM73" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MM73.svc or MM73.svc.cs at the Solution Explorer and start debugging.
    public class MM73 : IMM73
    {
        public SERVICE_RESULT_MODEL MM73_OUTBOUND_MATERIAL_NUMBER(IGRID_OUTBOUND_MODEL param)
        {
            SERVICE_RESULT_MODEL Results = new SERVICE_RESULT_MODEL();

            Results = MM_73_Hepler.SaveMaterial(param);

            return Results;
        }
 
    }
}
