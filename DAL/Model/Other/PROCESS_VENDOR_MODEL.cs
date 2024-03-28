using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class PROCESS_VENDOR
    {
        public ART_WF_MOCKUP_CHECK_LIST_2 CHECKLIST_DATA { get; set; }
        public ART_WF_MOCKUP_PROCESS_PG_2 PG_DATA { get; set; }
    }

    public class VENDOR_METADATA
    {

    }

    public class PROCESS_VENDOR_REQUEST : REQUEST_MODEL
    {
        public PROCESS_VENDOR data { get; set; }
    }

    public class PROCESS_VENDOR_RESULT : RESULT_MODEL
    {
        public List<PROCESS_VENDOR> data { get; set; }
    }
}
