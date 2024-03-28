using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace DAL.Model
{
    public class CN_VAP_MODEL
    {
        public int ID { get; set; }
        public string DISPLAY_TXT { get; set; }

    }
    public class CN_VAP_MODEL_REQUEST : REQUEST_MODEL
    {
        public CN_VAP_MODEL data { get; set; }
    }

    public class CN_VAP_MODEL_RESULT : RESULT_MODEL
    {
        public List<CN_VAP_MODEL> data { get; set; }
    }
}
