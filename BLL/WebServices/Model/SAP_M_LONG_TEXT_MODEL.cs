using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Model;
using DAL;
using BLL.Services;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace WebServices.Model
{
    [Serializable]
    public class SAP_M_LONG_TEXT_MODEL
    {
        [XmlArray("LONG_TXTS")]
        [XmlArrayItem(typeof(LONG_TXT), ElementName = "LONG_TXT")]
        public List<LONG_TXT> LONG_TXTS { get; set; }
        
    }
    
   //[Serializable]
    public class LONG_TXT
    {
        public string TEXT_NAME { get; set; }
        public string TEXT_ID { get; set; }
        public string LANGUAGE { get; set; }
        [XmlArray("LINES")]
        [XmlArrayItem(typeof(LINE), ElementName = "LINE")]
        public List<LINE> LINES { get; set; }
    }
    
   //[Serializable]
    public class LINE
    {
        public int ID { get; set; }
        public string TEXT { get; set; }
    }
}