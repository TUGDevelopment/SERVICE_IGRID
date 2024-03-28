using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class VENDOR_CUSTOMER_COLLABORATION_VENDOR_MOCKUP_REPORT 
    {
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
    }
    public class V_ART_REPORT_VENDOR_CUSTOMER_COLLABORATION_VENDOR_MOCKUP_REQUEST : REQUEST_MODEL
    {
        public VENDOR_CUSTOMER_COLLABORATION_VENDOR_MOCKUP_REPORT data { get; set; }
    }

    public class V_ART_REPORT_VENDOR_CUSTOMER_COLLABORATION_VENDOR_MOCKUP_RESULT : RESULT_MODEL
    {
        public List<V_ART_REPORT_VENDOR_CUSTOMER_COLLABORATION_VENDOR_MOCKUP> data { get; set; }
    }
}
