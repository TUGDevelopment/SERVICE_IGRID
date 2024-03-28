using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    public partial class ART_M_PERMISSION_2 : ART_M_PERMISSION
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }
        public string ACTION { get; set; }
    }

    public class ART_M_PERMISSION_REQUEST : REQUEST_MODEL
    {
        public ART_M_PERMISSION_2 data { get; set; }
    }

    public class ART_M_PERMISSION_RESULT : RESULT_MODEL
    {
        public List<ART_M_PERMISSION_2> data { get; set; }
    }
}