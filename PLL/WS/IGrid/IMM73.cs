using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WebServices.Model;

namespace PLL.WS.IGrid
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IMM73" in both code and config file together.
    [ServiceContract]
    [XmlSerializerFormat]
    public interface IMM73
    {
        [OperationContract]
        SERVICE_RESULT_MODEL MM73_OUTBOUND_MATERIAL_NUMBER(IGRID_OUTBOUND_MODEL param);

      
    }
}
