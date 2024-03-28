using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{

   public class IGRID_MODEL
    {
        public int ID { get; set; }
        public string CONDITION { get; set; }
        public string REQUESTTYPE { get; set; }
        public string DOCUMENTNO { get; set; }
        public string DMSNO { get; set; }
        public string MATERIAL { get; set; }
        public string DESCRIPTION { get; set; }
        public string MATERIALGROUP { get; set; }
        public string BRAND { get; set; }
        public string ASSIGNEE { get; set; }
        public string CREATEON { get; set; }
        public string ACTIVEBY { get; set; }
        public string STATUSBY { get; set; }
        public string FINALINFOGROUP { get; set; }
        public string ACTION { get; set; }
        public string REFERENCEMATERIAL { get; set; }
        public string VENDER { get; set; }
        public string VENDERDESCRIPTION { get; set; }
        public bool FIRST_LOAD { get; set; }
        public string STATUSAPP { get; set; }

        public string CREATEBY { get; set; }
        public string ASSIGN { get; set; }
    
        public string WHERE { get; set; }

    }
    public class IGRID_REQUEST : REQUEST_MODEL
    {
        public string user { get; set; }
        public string Type { get; set; }
        public string Keyword { get; set; }
        public IGRID_MODEL data { get; set; }
    }
    public class IGRID_REQUEST_LIST : REQUEST_MODEL
    {
        public List<IGRID_MODEL> data { get; set; }
    }
    public class IGRID_RESULT : RESULT_MODEL
    {
        public List<IGRID_MODEL> data { get; set; }
    }
}
