using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Model;
using DAL;
using BLL.Services;

namespace WebServices.Model
{
   
    public class SAP_M_PO_COMPLETE_SO_MODEL
    { 
        public List<SO_HEADER>  SO_HEADERS { get; set; }
        
    }

    public  class SO_HEADER : SAP_M_PO_COMPLETE_SO_HEADER
    {
        public List<SO_ITEM> SO_ITEMS { get; set; }
      
    }

    public  class SO_ITEM : SAP_M_PO_COMPLETE_SO_ITEM
    { 
       public List<COMPONENT> COMPONENTS { get; set; }
    }

    public class COMPONENT : SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT
    {
        public List<SAP_M_PO_COMPLETE_SO_ITEM_COMPONENT> COMPONENTS { get; set; }
         
    }
}