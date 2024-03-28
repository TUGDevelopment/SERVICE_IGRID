using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    public partial class ART_M_POSITION_ROLE_2 : ART_M_POSITION_ROLE
    {
      
    }

    public class ART_M_POSITION_ROLE_REQUEST : REQUEST_MODEL
    {
        public ART_M_POSITION_ROLE_2 data { get; set; }
    }

    public class ART_M_POSITION_ROLE_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_M_POSITION_ROLE_2> data { get; set; }
    }

    public class ART_M_POSITION_ROLE_RESULT : RESULT_MODEL
    {
        public List<ART_M_POSITION_ROLE_2> data { get; set; }
    }
}