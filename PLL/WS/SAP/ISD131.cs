using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WebServices.Model;

namespace PLL.WS.SAP
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISD131" in both code and config file together.
    [ServiceContract]
    [XmlSerializerFormat]
    public interface ISD131
    {
        [OperationContract]
        SERVICE_RESULT_MODEL SD131_OUTBOUND_ORDER_BOM(ORDER_BOM_MODEL param);
    }
}
