using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAL.Model
{
    public class ART_WF_DELEGATE_2 : ART_WF_DELEGATE
    {
        public string CURRENT_USER_DISPLAY_TXT { get; set; }
    }

    public class ART_WF_DELEGATE_REQUEST : REQUEST_MODEL
    {
        public ART_WF_DELEGATE_2 data { get; set; }
    }

    public class ART_WF_DELEGATE_RESULT : RESULT_MODEL
    {
        public List<ART_WF_DELEGATE_2> data { get; set; }
    }
}