//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class ART_WF_MOCKUP_PROCESS_PG
    {
        public int MOCKUP_SUB_PG_ID { get; set; }
        public int MOCKUP_SUB_ID { get; set; }
        public Nullable<int> PA_USER_ID { get; set; }
        public Nullable<int> PG_USER_ID { get; set; }
        public string REMARK { get; set; }
        public Nullable<int> GRADE_OF { get; set; }
        public string GRADE_OF_OTHER { get; set; }
        public Nullable<int> DI_CUT { get; set; }
        public string DI_CUT_OTHER { get; set; }
        public string SHEET_SIZE { get; set; }
        public string ACCESSORIES { get; set; }
        public Nullable<int> VENDOR { get; set; }
        public string VENDOR_OTHER { get; set; }
        public string PRINT_SYSTEM { get; set; }
        public Nullable<int> FLUTE { get; set; }
        public string FLUTE_OTHER { get; set; }
        public string DIMENSION_OF { get; set; }
        public string ROLL_SHEET { get; set; }
        public string ROLL_SHEET_OTHER { get; set; }
        public string REMARK_PG { get; set; }
        public string FINAL_INFO { get; set; }
        public string ID_MM { get; set; }
        public string CUSTOMER_DESIGN { get; set; }
        public string CUSTOMER_DESIGN_REMARK { get; set; }
        public string CUSTOMER_SPEC { get; set; }
        public string CUSTOMER_SPEC_REMARK { get; set; }
        public string CUSTOMER_SIZE { get; set; }
        public string CUSTOMER_SIZE_REMARK { get; set; }
        public string CUSTOMER_NOMINATES_VENDOR { get; set; }
        public string CUSTOMER_NOMINATES_VENDOR_REMARK { get; set; }
        public Nullable<int> DIE_LINE_MOCKUP_ID { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public int CREATE_BY { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public int UPDATE_BY { get; set; }
        public Nullable<int> SUSTAIN_MATERIAL { get; set; }
        public Nullable<int> PLASTIC_TYPE { get; set; }
        public string REUSEABLE { get; set; }
        public string RECYCLABLE { get; set; }
        public string COMPOSATABLE { get; set; }
        public Nullable<int> RECYCLE_CONTENT { get; set; }
        public string CERT { get; set; }
        public Nullable<int> CERT_SOURCE { get; set; }
        public Nullable<decimal> PKG_WEIGHT { get; set; }
        public string SUSTAIN_OTHER { get; set; }
    }
}
