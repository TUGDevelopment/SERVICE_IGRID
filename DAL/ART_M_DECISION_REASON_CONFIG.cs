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
    
    public partial class ART_M_DECISION_REASON_CONFIG
    {
        public int DECISION_REASON_CONFIG_ID { get; set; }
        public Nullable<int> ORDERBY { get; set; }
        public string DECISION_REASON_CONFIG_CODE { get; set; }
        public string DECISION_REASON_CONFIG_NAME { get; set; }
        public string WF { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public int CREATE_BY { get; set; }
        public System.DateTime UPDATE_DATE { get; set; }
        public int UPDATE_BY { get; set; }
    }
}
