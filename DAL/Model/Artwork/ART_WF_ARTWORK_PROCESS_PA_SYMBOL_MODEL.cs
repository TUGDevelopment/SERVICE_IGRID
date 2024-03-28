using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{ 
    public partial class ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2 : ART_WF_ARTWORK_PROCESS_PA_SYMBOL
    {
        public string DISPLAY_TXT { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2_METADATA
    {
         
    }

    public class ART_WF_ARTWORK_PROCESS_PA_SYMBOL_REQUEST : REQUEST_MODEL
    {
        public ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2 data { get; set; }
    }

    public class ART_WF_ARTWORK_PROCESS_PA_SYMBOL_RESULT : RESULT_MODEL
    {
        public List<ART_WF_ARTWORK_PROCESS_PA_SYMBOL_2> data { get; set; }
    }
}
