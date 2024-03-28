using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class VENDOR_ARTWORK_REPORT_2
    {
        public string DATE_FROM { get; set; }
        public string DATE_TO { get; set; }
        public int? VENDOR_ID { get; set; }
    }
    public class VENDOR_ARTWORK_REPORT_REQUEST : REQUEST_MODEL
    {
        public VENDOR_ARTWORK_REPORT_2 data { get; set; }
    }

    public class VENDOR_ARTWORK_REPORT_RESULT : RESULT_MODEL
    {
        public List<VENDOR_ARTWORK_REPORT> data { get; set; }
    }

    public class VENDOR_ARTWORK_REPORT
    {
        public string Vendor { get; set; }
        public Nullable<int> Total { get; set; }
        public Nullable<int> Approve { get; set; }
        public Nullable<int> Revise_PA { get; set; }
        public Nullable<int> Revise_PG { get; set; }
        public Nullable<int> Revise_QC { get; set; }
        public Nullable<int> Revise_Customer { get; set; }
        public Nullable<int> Revise_Marketing { get; set; }
        public Nullable<int> Revise_Vendor { get; set; }
        public Nullable<int> Revise_Warehouse { get; set; }
        public Nullable<int> Revise_Planning { get; set; }
        public Nullable<int> Day_Send_Print_All { get; set; }
        public Nullable<int> Day_Send_Print_Ontime { get; set; }
        public Nullable<int> Day_Confirm_PO_All { get; set; }
        public Nullable<int> Day_Confirm_PO_Ontime { get; set; }
        public Nullable<int> Day_Send_Shade_All { get; set; }
        public Nullable<int> Day_Send_Shade_Ontime { get; set; }
        public Nullable<System.DateTime> Request_Date_Form { get; set; }

    }
}
