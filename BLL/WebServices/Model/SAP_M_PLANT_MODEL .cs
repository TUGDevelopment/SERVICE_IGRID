using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WebServices.Model
{

    [Serializable]
    public class PLANT_MODEL
    {
        [XmlArray("PLANTS")]
        [XmlArrayItem(typeof(PLANT), ElementName = "PLANT")]
        public List<PLANT> Plants { get; set; }
    }

    public class PLANT
    {
        public string PlantCode { get; set; }
        public string PlantDescription { get; set; }
    }
}