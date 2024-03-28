using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class IGRID_M_OUTBOUND_HEADER_2 : IGRID_M_OUTBOUND_HEADER
    {
        public string ID { get; set; }
        public string DISPLAY_TXT { get; set; }
    }

    public class IGRID_M_OUTBOUND_HEADER_2_METADATA
    {
      
    }

    public class IGRID_M_OUTBOUND_HEADER_REQUEST : REQUEST_MODEL
    {
        public IGRID_M_OUTBOUND_HEADER_2 data { get; set; }
    }

    public class IGRID_M_OUTBOUND_HEADER_RESULT : RESULT_MODEL
    {
        public List<IGRID_M_OUTBOUND_HEADER_2> data { get; set; }
    }
}
