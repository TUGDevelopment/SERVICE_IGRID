using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DAL.Model
{
    public class DASHBOARD_GRAPH_MODEL: RESULT_MODEL
    {
        public int cntWaitingPKG { get; set; }
        public int cntWaitingQuo { get; set; }
        public int cntWaitingSample { get; set; }
        public int cntWaitingCustomer { get; set; }
        public string cntWaitingPKG_txt { get; set; }
        public string cntWaitingQuo_txt { get; set; }
        public string cntWaitingSample_txt { get; set; }
        public string cntWaitingCustomer_txt { get; set; }

        public int cntWaitingQCConfirmation { get; set; }
        public int cntWaitingPrintmaster { get; set; }
        public int cntWaitingCustomerApprovePrintMaster { get; set; }
        public int cntWaitingShadeLimit { get; set; }
        public int cntWaitingCustomerApproveShadeLimit { get; set; }
        public string cntWaitingQCConfirmation_txt { get; set; }
        public string cntWaitingPrintmaster_txt { get; set; }
        public string cntWaitingCustomerApprovePrintMaster_txt { get; set; }
        public string cntWaitingShadeLimit_txt { get; set; }
        public string cntWaitingCustomerApproveShadeLimit_txt { get; set; }

        public int cntWaitingGMMK { get; set; }
        public int cntWaitingCustomerReview { get; set; }
        public string cntWaitingGMMK_txt { get; set; }
        public string cntWaitingCustomerReview_txt { get; set; }

        public int cntIncoming { get; set; }
        public int cntInProgress { get; set; }
        public int cntPool { get; set; }

        public string cntIncoming_txt { get; set; }
        public string cntInProgress_txt { get; set; }
        public string cntPool_txt { get; set; }
    }
}
