using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2 : ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER
    {
        public int DATA_FROM_DB { get; set; }

    }

    public class ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2_METADATA
    {
         
    }

    public class ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_PA_PRODUCT_OTHER_2> data { get; set; }
    }
}
