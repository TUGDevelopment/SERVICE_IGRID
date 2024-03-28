using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class ART_SYS_LOG_2 : ART_SYS_LOG
    {
        public bool FIRST_LOAD { get; set; }
    }

    public class ART_SYS_LOG_REQUEST : REQUEST_MODEL
    {
        public ART_SYS_LOG_2 data { get; set; }
    }

    public class ART_SYS_LOG_RESULT : RESULT_MODEL
    {
        public List<ART_SYS_LOG_2> data { get; set; }
    }
}
