using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WebServices.Model;

namespace PLL.WS.SAP
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISD127" in both code and config file together.
    [ServiceContract]
    [XmlSerializerFormat]
    public interface ISD127
    {
        [OperationContract]
        SERVICE_RESULT_MODEL SD127_PO_COMPLETE_ORDER(SAP_M_PO_COMPLETE_SO_MODEL param);
    }
}
