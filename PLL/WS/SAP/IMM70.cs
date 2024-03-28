using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WebServices.Model;

namespace PLL.WS.SAP
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMM70" in both code and config file together.
    [ServiceContract]
    [XmlSerializerFormat]
    public interface IMM70
    {
        [OperationContract]
        SERVICE_RESULT_MODEL MM70_OUTBOUND_PLANT(PLANT_MODEL param);
    }
}
