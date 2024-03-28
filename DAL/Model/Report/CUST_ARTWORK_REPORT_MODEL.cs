using System;
using System.Collections.Generic;

namespace DAL.Model
{
    public partial class CUST_ARTWORK_REPORT_2
    {
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public int? CUSTOMER_ID { get; set; }
    }
    public class CUST_ARTWORK_REPORT_REQUEST : REQUEST_MODEL
    {
        public CUST_ARTWORK_REPORT_2 data { get; set; }
    }

    public class CUST_ARTWORK_REPORT_RESULT : RESULT_MODEL
    {
        public List<CUST_ARTWORK_REPORT> data { get; set; }
    }

    public class CUST_ARTWORK_REPORT
    {
        public string Customer { get; set; }
        public Nullable<int> Total { get; set; }
        public Nullable<int> Approve_Artwork { get; set; }
        public Nullable<int> Approve_ShadeLimit { get; set; }
        public Nullable<int> Revise_ChangeOption { get; set; }
        public Nullable<int> Revise_WantToAdjust { get; set; }
        public Nullable<int> Revise_IncorrectArtwork { get; set; }
        public Nullable<int> Cancel { get; set; }
        public Nullable<System.DateTime> Request_Date_Form { get; set; }
    }
}
