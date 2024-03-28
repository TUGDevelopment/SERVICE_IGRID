using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebServices.Model
{

    [Serializable]
    public class CHARACTERISTICS
    {
        [XmlArray("CHARACTERISTICS")]
        [XmlArrayItem(typeof(CHARACTERISTIC), ElementName = "CHARACTERISTIC")]
        public List<CHARACTERISTIC> Characteristics { get; set; }
        
    }

    public class CHARACTERISTIC
    {
        public string NAME { get; set; }
        public string ID { get; set; }
        public string Old_ID { get; set; }
        public string VALUE { get; set; }
        public string DESCRIPTION { get; set; }
        public string Changed_Action { get; set; }
    }

 
}