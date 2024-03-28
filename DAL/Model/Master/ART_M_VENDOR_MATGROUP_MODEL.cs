using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    public partial class ART_M_VENDOR_MATGROUP_2 : ART_M_VENDOR_MATGROUP
    {
        public bool CHECKED { get; set; }
    }

    public class ART_M_VENDOR_MATGROUP_REQUEST : REQUEST_MODEL
    {
        public ART_M_VENDOR_MATGROUP_2 data { get; set; }
    }


    public class ART_M_VENDOR_MATGROUP_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_M_VENDOR_MATGROUP_2> data { get; set; }
    }

    public class ART_M_VENDOR_MATGROUP_RESULT : RESULT_MODEL
    {
        public List<ART_M_VENDOR_MATGROUP_2> data { get; set; }
    }
}