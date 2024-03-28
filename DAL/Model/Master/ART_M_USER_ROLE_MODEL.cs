using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    public partial class ART_M_USER_ROLE_2 : ART_M_USER_ROLE
    {
        public bool CHECKED { get; set; }
        public string ROLE_CODE { get; set; }

        public string POSITION_CODE { get; set; }   //#TSK-1511 #SR-70695 by aof in 09/2022 

        public string IGRID_USER_FN { get; set; }   //  IGRID REIM by aof in 08/2023
    }

    public class ART_M_USER_ROLE_REQUEST : REQUEST_MODEL
    {
        public ART_M_USER_ROLE_2 data { get; set; }
    }

    public class ART_M_USER_ROLE_REQUEST_LIST : REQUEST_MODEL
    {
        public List<ART_M_USER_ROLE_2> data { get; set; }
    }

    public class ART_M_USER_ROLE_RESULT : RESULT_MODEL
    {
        public List<ART_M_USER_ROLE_2> data { get; set; }
    }
}