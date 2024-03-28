using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public  class END_TO_END_REPORT
    {
        public string NO { get; set; }
        public string WF_NO { get; set; }
        public string PACKAGING_TYPE { get; set; }
        public string ORDER_BOM_COMPONENT { get; set; }
        public string CURRENT_STEP { get; set; }
        public string SO_NO { get; set; }
        public string SO_ITEM { get; set; }
        public string BRAND { get; set; }
        public string ADDITIONAL_BRAND { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string PROD_MEMO { get; set; }
        public string RD_REFERENCE_NO { get; set; }
        public string PRODUCTION_PLANT { get; set; }
        public string SOLD_TO { get; set; }
        public string SHIP_TO { get; set; }
        public string COUNTRY { get; set; }
        public string PORT { get; set; }
        public string IN_TRANSIT_TO { get; set; }
        public string SO_CREATE_DATE { get; set; }
        public string RDD { get; set; }
        public string CURRENT_ASSIGN { get; set; }
        public string REQUEST_NO { get; set; }
        public string PA_OWNER { get; set; }
        public string PG_OWNER { get; set; }
        public string STEP { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
        public string STEP_DURATION { get; set; }
        public string TOTAL_DAY { get; set; }
        public string REASON_CODE { get; set; }
        public string MARKETING_NAME { get; set; }
        public string PROJECT_NAME { get; set; }
        public string CREATOR { get; set; }
        public string GENERATE_EXCEL { get; set; }

    }

    public class END_TO_END_REPORT_REQUEST : REQUEST_MODEL
    {
        public END_TO_END_REPORT data { get; set; }
    }

    public class END_TO_END_REPORT_RESULT : RESULT_MODEL
    {
        public List<END_TO_END_REPORT> data { get; set; }
    }
}
