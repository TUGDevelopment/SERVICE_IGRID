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
    
    public partial class ART_WF_ARTWORK_MATERIAL_LOCK
    {
        public int MATERIAL_LOCK_ID { get; set; }
        public string MATERIAL_NO { get; set; }
        public string MATERIAL_DESCRIPTION { get; set; }
        public string STATUS { get; set; }
        public Nullable<System.DateTime> UNLOCK_DATE_FROM { get; set; }
        public Nullable<System.DateTime> UNLOCK_DATE_TO { get; set; }
        public string PIC { get; set; }
        public string REQUEST_FORM_NO { get; set; }
        public string ARTWORK_NO { get; set; }
        public string MOCKUP_NO { get; set; }
        public Nullable<int> REQUEST_FORM_ID { get; set; }
        public Nullable<int> ARTWORK_ID { get; set; }
        public Nullable<int> MOCKUP_ID { get; set; }
        public string PACKAGING_TYPE { get; set; }
        public string PRIMARY_TYPE { get; set; }
        public string PRIMARY_SIZE { get; set; }
        public string PACKAGING_STYLE { get; set; }
        public string PACK_SIZE { get; set; }
        public string PG_OWNER { get; set; }
        public string IS_HAS_FILES { get; set; }
        public string IS_ACTIVE { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public int CREATE_BY { get; set; }
        public System.DateTime UPDATE_DATE { get; set; }
        public int UPDATE_BY { get; set; }
        public string REMARK_UNLOCK { get; set; }
        public string REMARK_LOCK { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE_LOCK { get; set; }
        public Nullable<int> UPDATE_BY_LOCK { get; set; }
    }
}
