using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class SALES_ORDER_CHANGE
    {
        public int ARTWORK_SUB_ID { get; set; }
        public string GROUPING { get; set; }
        public string FIELDS_NAME { get; set; }
        public string OLD_VALUE { get; set; }
        public string NEW_VALUE { get; set; }

    }

    public class SALES_ORDER_CHANGE_METADATA
    {

    }

    public class SALES_ORDER_CHANGE_REQUEST : REQUEST_MODEL
    {
        public SALES_ORDER_CHANGE data { get; set; }
    }

    public class SALES_ORDER_CHANGE_RESULT : RESULT_MODEL
    {
        public List<SALES_ORDER_CHANGE> data { get; set; }
    }

   
}
