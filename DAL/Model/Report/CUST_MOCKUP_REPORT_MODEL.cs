using System;
using System.Collections.Generic;

namespace DAL.Model
{
    public partial class CUST_MOCKUP_REPORT_2
    {
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public int? CUSTOMER_ID { get; set; }
    }
    public class CUST_MOCKUP_REPORT_REQUEST : REQUEST_MODEL
    {
        public CUST_MOCKUP_REPORT_2 data { get; set; }
    }

    public class CUST_MOCKUP_REPORT_RESULT : RESULT_MODEL
    {
        public List<CUST_MOCKUP_REPORT> data { get; set; }
    }

    public class CUST_MOCKUP_REPORT
    {
        public string CustomerCode { get; set; }
        public Nullable<int> Total { get; set; }
        public Nullable<int> ApproveDieLine_NoArtwork { get; set; }
        public Nullable<int> ApproveDieLine_Artwork { get; set; }
        public Nullable<int> ApprovePhysical_Mockup { get; set; }
        public Nullable<int> Revise_WanttoAdjust { get; set; }
        public Nullable<int> Revise_IncorrectMockup { get; set; }
        public Nullable<int> Cancel { get; set; }
        public Nullable<System.DateTime> Request_Date_Form { get; set; }
    }
}
