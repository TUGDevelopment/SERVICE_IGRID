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
    
    public partial class ART_M_USER_CUSTOMER
    {
        public int USER_CUSTOMER_ID { get; set; }
        public int USER_ID { get; set; }
        public int CUSTOMER_ID { get; set; }
        public string IS_EMAIL_TO { get; set; }
        public string IS_EMAIL_CC { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public int CREATE_BY { get; set; }
        public System.DateTime UPDATE_DATE { get; set; }
        public int UPDATE_BY { get; set; }
    }
}
