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
    
    public partial class ART_WF_ARTWORK_PROCESS_SO_DETAIL
    {
        public int ARTWORK_PROCESS_SO_ID { get; set; }
        public int ARTWORK_REQUEST_ID { get; set; }
        public int ARTWORK_SUB_ID { get; set; }
        public string SALES_ORDER_NO { get; set; }
        public string SALES_ORDER_ITEM { get; set; }
        public string MATERIAL_NO { get; set; }
        public string BOM_NO { get; set; }
        public Nullable<int> BOM_ID { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public int CREATE_BY { get; set; }
        public System.DateTime UPDATE_DATE { get; set; }
        public int UPDATE_BY { get; set; }
    }
}
