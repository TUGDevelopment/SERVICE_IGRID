using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WebServices.Model;

namespace PLL.WS.IGrid
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMM72" in both code and config file together.
    [ServiceContract]
    [XmlSerializerFormat]
    public interface IMM72
    {
        [OperationContract]
        SERVICE_RESULT_MODEL MM72_OUTBOUND_MATERIAL_CHARACTERISTIC(CHARACTERISTICS param);
    }
}
