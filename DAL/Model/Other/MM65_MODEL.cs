using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Model
{
    public class MM65_MODEL
    {
        public int ARTWORK_SUB_ID { get; set; }
        public string RECORD_TYPE { get; set; }
        public string REFERENCE_MATERIAL { get; set; }
        public string CHANGE_POINT { get; set; }
        public string ACTION { get; set; }
    }

    public class MM65_METADATA
    {

    }

    public class MM65_REQUEST : REQUEST_MODEL
    {
        public MM65_MODEL data { get; set; }
    }

    public class MM65_RESULT : RESULT_MODEL
    {
        public List<MM65_MODEL> data { get; set; }
    }
}
