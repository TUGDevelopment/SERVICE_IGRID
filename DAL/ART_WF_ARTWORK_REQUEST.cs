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
    
    public partial class ART_WF_ARTWORK_REQUEST
    {
        public int ARTWORK_REQUEST_ID { get; set; }
        public Nullable<int> REFERENCE_REQUEST_ID { get; set; }
        public string REFERENCE_REQUEST_NO { get; set; }
        public string REFERENCE_REQUEST_TYPE { get; set; }
        public string TYPE_OF_ARTWORK { get; set; }
        public string ARTWORK_REQUEST_NO { get; set; }
        public string PROJECT_NAME { get; set; }
        public Nullable<int> TYPE_OF_PRODUCT_ID { get; set; }
        public Nullable<int> REVIEWER_ID { get; set; }
        public Nullable<int> CREATOR_ID { get; set; }
        public Nullable<int> COMPANY_ID { get; set; }
        public Nullable<int> SOLD_TO_ID { get; set; }
        public Nullable<int> SHIP_TO_ID { get; set; }
        public Nullable<int> CUSTOMER_OTHER_ID { get; set; }
        public Nullable<int> PRIMARY_TYPE_ID { get; set; }
        public string PRIMARY_TYPE_OTHER { get; set; }
        public Nullable<int> PRIMARY_SIZE_ID { get; set; }
        public string PRIMARY_SIZE_OTHER { get; set; }
        public Nullable<int> CONTAINER_TYPE_ID { get; set; }
        public string CONTAINER_TYPE_OTHER { get; set; }
        public Nullable<int> LID_TYPE_ID { get; set; }
        public string LID_TYPE_OTHER { get; set; }
        public Nullable<int> PACKING_STYLE_ID { get; set; }
        public string PACKING_STYLE_OTHER { get; set; }
        public Nullable<int> PACK_SIZE_ID { get; set; }
        public string PACK_SIZE_OTHER { get; set; }
        public Nullable<int> BRAND_ID { get; set; }
        public string BRAND_OTHER { get; set; }
        public Nullable<System.DateTime> REQUEST_DELIVERY_DATE { get; set; }
        public string SPECIAL_REQUIREMENT { get; set; }
        public string OTHER_REQUEST { get; set; }
        public Nullable<long> REQUEST_FORM_FOLDER_NODE_ID { get; set; }
        public Nullable<int> TWO_P_ID { get; set; }
        public Nullable<int> THREE_P_ID { get; set; }
        public Nullable<int> UPLOAD_BY { get; set; }
        public Nullable<System.DateTime> REQUEST_FORM_CREATE_DATE { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public int CREATE_BY { get; set; }
        public System.DateTime UPDATE_DATE { get; set; }
        public int UPDATE_BY { get; set; }
        public string IN_TRANSIT_TO { get; set; }
        public string VIA { get; set; }
        public string ARTWORK_REQUEST_TYPE { get; set; }
    }
}
