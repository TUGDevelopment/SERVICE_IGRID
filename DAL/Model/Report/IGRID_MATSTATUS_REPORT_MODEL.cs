using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class IGRID_MATSTATUS_REPORT_MODEL
    {
        public Int64 ID { get; set; }
        public string MATERIAL { get; set; }
        public string DESCRIPTION { get; set; }
        public string STATUS { get; set; }

        public string FIRST_LOAD { get; set; }
        public string SEARCH_BY_STATUS { get; set; }
        public string SEARCH_KEYWORD { get; set; }
    }


    public class IGRID_MATSTATUS_REPORT_MODEL_REQUEST : REQUEST_MODEL
    {
        public IGRID_MATSTATUS_REPORT_MODEL data { get; set; }

    }

    public class IGRID_MATSTATUS_REPORT_MODEL_RESULT : RESULT_MODEL
    {

        public List<IGRID_MATSTATUS_REPORT_MODEL> data { get; set; }
        //public int ORDER_COLUMN { get; set; }
    }

}
