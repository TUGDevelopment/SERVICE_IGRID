using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Model;
using DAL;
using BLL.Services;
using System.Xml.Serialization;
using WebServices.Model;

namespace WebServices.Model
{
    [Serializable]
    public class IGRID_OUTBOUND_MODEL
    {
        [XmlArray("OUTBOUND_HEADERS")]
        [XmlArrayItem(typeof(IGRID_OUTBOUND_HEADER_MODEL), ElementName = "Header")]
        public List<IGRID_OUTBOUND_HEADER_MODEL> OUTBOUND_HEADERS { get; set; }

        [XmlArray("OUTBOUND_ITEMS")]
        [XmlArrayItem(typeof(IGRID_OUTBOUND_ITEM_MODEL), ElementName = "InboundArtwork")]
        public List<IGRID_OUTBOUND_ITEM_MODEL> OUTBOUND_ITEMS { get; set; }
    }
}