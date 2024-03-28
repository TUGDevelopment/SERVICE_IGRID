using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class PROCESS_CUSTOMER
    {
        public ART_WF_MOCKUP_CHECK_LIST_2 CHECKLIST_DATA { get; set; }
        public ART_WF_MOCKUP_CHECK_LIST_ITEM_2 CHECKLIST_ITEM_DATA { get; set; }
    }

    public class CUSTOMER_METADATA
    {

    }

    public class PROCESS_CUSTOMER_REQUEST : REQUEST_MODEL
    {
        public PROCESS_CUSTOMER data { get; set; }
    }

    public class PROCESS_CUSTOMER_RESULT : RESULT_MODEL
    {
        public List<PROCESS_CUSTOMER> data { get; set; }
    }
}
