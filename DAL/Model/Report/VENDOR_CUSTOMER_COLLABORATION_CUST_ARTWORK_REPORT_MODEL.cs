using System;
using System.Collections.Generic;

namespace DAL.Model
{
    public partial class VENDOR_CUSTOMER_COLLABORATION_CUST_ARTWORK_REPORT
    {
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
    }
    public class V_ART_REPORT_VENDOR_CUSTOMER_COLLABORATION_CUST_ARTWORK_REQUEST : REQUEST_MODEL
    {
        public VENDOR_CUSTOMER_COLLABORATION_CUST_ARTWORK_REPORT data { get; set; }
    }

    public class V_ART_REPORT_VENDOR_CUSTOMER_COLLABORATION_CUST_ARTWORK_RESULT : RESULT_MODEL
    {
        public List<V_ART_REPORT_VENDOR_CUSTOMER_COLLABORATION_CUST_ARTWORK> data { get; set; }
    }
}
