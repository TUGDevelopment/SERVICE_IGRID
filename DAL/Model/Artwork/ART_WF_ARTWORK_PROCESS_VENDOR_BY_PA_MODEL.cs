using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2 : ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA
    {

        public ART_WF_ARTWORK_PROCESS_2 PROCESS { get; set; }

    }

    public class ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2_METADATA
    {
         
    }

    public class ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_VENDOR_BY_PA_2> data { get; set; }
    }
}
