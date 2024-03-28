using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public partial class CHARTTRACKING_REPORT
    {
        public int count_Status_upd { get; set; }
        public string Status_upd { get; set; }
        public string Role { get; set; }
        public string By { get; set; }
        public string Status { get; set; }
        public string FrDt { get; set; }
        public string ToDt { get; set; }
        public string Keyword { get; set; }

    }
    public class CHARTTRACKING_REPORT_REQUEST : REQUEST_MODEL
    {
        public string name { get; set; }
        
        public string Keyword { get; set; }
        public CHARTTRACKING_REPORT data { get; set; }
    }

    public class CHARTTRACKING_REPORT_REQUEST_LIST : REQUEST_MODEL
    {
        public List<CHARTTRACKING_REPORT> data { get; set; }
    }

    public class CHARTTRACKING_REPORT_RESULT : RESULT_MODEL
    {
        public List<CHARTTRACKING_REPORT> data { get; set; }
    }
}
