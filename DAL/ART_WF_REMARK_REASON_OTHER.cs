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
    
    public partial class ART_WF_REMARK_REASON_OTHER
    {
        public int ART_WF_REMARK_REASON_OTHER_ID { get; set; }
        public string WF_TYPE { get; set; }
        public Nullable<int> WF_SUB_ID { get; set; }
        public Nullable<int> WF_STEP { get; set; }
        public string REMARK_REASON { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public int CREATE_BY { get; set; }
        public System.DateTime UPDATE_DATE { get; set; }
        public int UPDATE_BY { get; set; }
    }
}
