using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

    public class MasterObject_MODEL
    {
        public string Changed_Tabname { get; set; }
        public string Changed_Charname { get; set; }
        public string Id { get; set; }
        public string Old_Id { get; set; }
        public string Old_Description { get; set; }
        public string Description { get; set; }
        public string Changed_Action { get; set; }
        public string Changed_By { get; set; }
        public string Active { get; set; }
        public string Material_Group { get; set; }
        public string Material_Type { get; set; }
        public string DescriptionText { get; set; }
        public string Can { get; set; }
        public string LidType { get; set; }
        public string ContainerType { get; set; }
        public string DescriptionType { get; set; }
        public string user_name { get; set; }
        public string fn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Authorize_ChangeMaster { get; set; }
        public string PrimaryCode { get; set; }
        public string GroupStyle { get; set; }
        public string PackingStyle { get; set; }
        public string RefStyle { get; set; }
        public string Packsize { get; set; }
        public string BaseUnit { get; set; }
        public string TypeofPrimary { get; set; }
        public string RegisteredNo { get; set; }
        public string Address { get; set; }
        public string Plant { get; set; }
        public string Product_Group { get; set; }
        public string Product_GroupDesc { get; set; }
        public string PRD_Plant { get; set; }
        public string WHNumber { get; set; }
        public string StorageType { get; set; }
        public string LE_Qty { get; set; }
        public string Storage_UnitType { get; set; }
        public string Changed_Reason { get; set; }
        public string SAP_EDPUsername { get; set; }
        public string Value { get; set; }


    }
    public class MasterObject_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public MasterObject_MODEL data { get; set; }
    }
    public class MasterObject_REQUEST_LIST : REQUEST_MODEL
    {
        public List<MasterObject_MODEL> data { get; set; }
    }
    public class MasterObject_RESULT : RESULT_MODEL
    {
        public List<MasterObject_MODEL> data { get; set; }
    }
}
