using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WebServices.Model;

namespace PLL.WS.SAP
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMM71" in both code and config file together.
    [ServiceContract]
    [XmlSerializerFormat]
    public interface IMM71
    {
        [OperationContract]
        SERVICE_RESULT_MODEL MM71_PO_OUTBOUND_IDOC(SAP_M_PO_IDOC_MODEL param);
    }
}
